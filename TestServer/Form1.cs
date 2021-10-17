using DAL_TestSystem;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestServer
{
    public partial class Form1 : Form
    {
        static GenericUnitOfWork work;
        static IGenericRepository<User> repoUser;
        static IGenericRepository<Test> tests;

        User user1 = null;
        static Socket listeningSocket;// only for listen
       
        static List<Client> ListOfClients = new List<Client>();//хто буде проходити тести

        public Form1()
        {
            InitializeComponent();
            work = new GenericUnitOfWork(new Context(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            repoUser = work.Repository<User>();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            work.Dispose();
            listeningSocket.Close();
            foreach (var item in ListOfClients)
            {
                item.Dispose();
            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            user1 = repoUser.FirstOrDefault(u => u.Login == textBox1.Text && u.Password == textBox2.Text);
            if (user1 != null && user1.IsAdmin == true)
            {
                DataBase db = new DataBase(work);
                db.Show();
               // MessageBox.Show("Test");
                work.SaveChanges();


                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //сервер завжди сідає на локал хост
                IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
                IPAddress ip = iPHostEntry.AddressList[1]; //ip our comp. 
                int port = 33000;                     //number of port
                try
                {   //create server
                    //create end point
                    IPEndPoint iPEndPoint = new IPEndPoint(ip, port);

                    listeningSocket.Bind(iPEndPoint);//садимо на конкретний порт

                    Task.Factory.StartNew(() => ListenThread(listeningSocket));
                }
                catch { MessageBox.Show("the server is not available"); }
            }
            else
                MessageBox.Show("you aren't admin");
        }
        static private void ListenThread(Socket listeningSocket)// прослуховування під'єднань
        {
            listeningSocket.Listen(2);//2 на наш розсуд кількість одночасних прослуховувань 
            while (true)// прослуховування під'єднать користувачів
            {
                Socket clientSocket = listeningSocket.Accept();//block method поки не завершить роботу Accept();
                                                                   //код виконуватися далы не буде
                Client client = new Client()
                {
                    ClientSocket = clientSocket
                };
                    ListOfClients.Add(client);

                Thread receiveThread = new Thread(ReceveThreadFunction);
                    receiveThread.IsBackground = true;
                    receiveThread.Start(client);
            }
        }
        static private void ReceveThreadFunction(object sender)
        {
            Client client = sender as Client;
            if (client == null)
                throw new ArgumentException();
           
            while (true)//постійно читає( чекаємо на повідомлення клієнта)
            {
                Socket receiveSocket = client.ClientSocket;
                byte[] receivebyte = new byte[16384];

                // reading
                receiveSocket.Receive(receivebyte);// block func. поки не буде повідомлення
                Info obj;
                using (var memStream = new MemoryStream())
                {
                    var binForm = new BinaryFormatter();
                    memStream.Write(receivebyte, 0, receivebyte.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    obj = (Info)binForm.Deserialize(memStream);
                   //отримали запит від клієнта
                }
                if (obj.Msg == "close")
                    {
                        ListOfClients.Remove(client);
                        client.Dispose();
                        break; // вихід з вайлу
                }
                SendMes(obj, client);
            }

        }
        static private void SendMes(Info info,Client cl)
        {
           
            BinaryFormatter bf = new BinaryFormatter();
            byte[] sendByte = new byte[1024];

            //дивимося чи існує такий користувач
            DAL_TestSystem.User us = repoUser.FirstOrDefault(u => u.Login == info.Login && u.Password == info.Password);
            if (us != null)
            {
                //будемо дивитися на повідомлення що хоче отримати користувач
                if (info.Msg == "")//щойно зайшов
                {
                    MessageBox.Show("isUser");
                    info.IsRegistered = true;
                    info.UserId = us.Id;
                    cl.UserId = us.Id;
                    string mes = "isUser"; //посилаємо сигнал щоб відкрилася інша форма
                    using (var ms = new MemoryStream())
                    {
                        bf.Serialize(ms, mes);
                        sendByte = ms.ToArray();
                    }
                    cl.ClientSocket.Send(sendByte);
                    return;
                }
                if (info.Msg == "load tests")//натиснув кнопку показати тести які він має здати
                {
                    MessageBox.Show("load tests");
                    info.Mark = -1;
                    
                    tests = work.Repository<Test>();
                    //
                    var list = tests.GetAllData().Select(t => new
                    {
                        id = t.Id,
                        author = t.Author,
                        title = t.TestName,
                        count_of_questions = t.QuestionCount
                    }).ToList();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Id", typeof(Int32));
                    dt.Columns.Add("Author", typeof(string));
                    dt.Columns.Add("TestName", typeof(string));
                    dt.Columns.Add("QuestionCount", typeof(Int32));

                    foreach (var item in list)
                    {
                        dt.Rows.Add(item.id, item.author, item.title, item.count_of_questions);
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    BinaryFormatter bFormat = new BinaryFormatter();
                    byte[] outList = null;
                    //dt.RemotingFormat = SerializationFormat.Binary;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bFormat.Serialize(ms, ds);
                        outList = ms.ToArray();
                    }
                    info.Buffer = outList;
                    using (var ms = new MemoryStream())
                    {
                        //відправляємо тест
                        bf.Serialize(ms, info);
                        sendByte = ms.ToArray();
                    }
                    cl.ClientSocket.Send(sendByte);//send message
                    MessageBox.Show("send list of Tests\n"+outList.Length);
                    return;
                }
                if (info.Msg == "pass test")
                {
                    MessageBox.Show("TestServer");
                    //здача тесту
                    info.Msg = "get result";
                    tests = work.Repository<Test>();
                   var test1 = tests.FirstOrDefault(t => t.Id == info.IdTest);
                    //info.Test = new Xml2CSharp.Test() { Author = test1.Author, QuestionCount = test1.QuestionCount.ToString() };

                    using (var ms = new MemoryStream())
                    {
                        //відправляємо тест
                        bf.Serialize(ms, info);
                        sendByte = ms.ToArray();
                    }
                    cl.ClientSocket.Send(sendByte);//send message
                    return;
                }
                if (info.Msg == "get result")
                {
                    // тут треба визначити оцінку і записати в базу даних

                    //
                    //додаємо відповіді користувача в базу
                    IGenericRepository<UserAnswer> userAnswers;
                    userAnswers = work.Repository<UserAnswer>();
                    //foreach (var item in info.Answers)
                    //{
                    //    userAnswers.Add(new UserAnswer()
                    //    {
                    //        User = info.User,
                    //        Answer = item,
                    //        Date = DateTime.Now
                    //    });
                    //}
                    //рахуємо результат
                    CalculateMark(info);
                    //результат здачі тесту
                    IGenericRepository<Result> res;
                    res = work.Repository<Result>();
                    //info.Mark = res.FirstOrDefault(u => u.UserId == info.User.Id && u.TestId == info.IdTest).Rate;
                   // info.Test = null;
                    //info.Answers = null;
                    info.Msg = "load tests";
                    info.IdTest = -1;
                    using (var ms = new MemoryStream())
                    {
                        //відправляємо назви список тестів
                        bf.Serialize(ms, info);
                        sendByte = ms.ToArray();
                    }
                    cl.ClientSocket.Send(sendByte);//send message
                    return;
                }
            }
            else
            {
                MessageBox.Show("else");
                string mes = "You aren't registered.\nTry again.";
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, mes);
                    sendByte = ms.ToArray();
                }
                cl.ClientSocket.Send(sendByte);   //відправляємо що він не зареєстрований
                //
                ListOfClients.Remove(cl);
                cl.Dispose(); //перериваємо з ним зв'язок
            }
        }
        private static void CalculateMark(Info inf)
        {
            double mark = 0;
            int value = 0, failed = 0;
            //foreach (var item in inf.Test.Questions)
            //{
            //    value += item.Difficalty;
            //}
            IGenericRepository<Answer> answers;
            answers = work.Repository<Answer>();
            //foreach (var item in inf.Answers)
            //{
            //    var ans = answers.FirstOrDefault(a => a.QuestionId == item.QuestionId);
            //    if (item.IsRight != ans.IsRight)
            //    {
            //        failed += ans.Question.Difficalty;
            //    }
            //}
            //рахуємо результат
            mark = ((value = failed) * 100) / value;
            //add to data base
            IGenericRepository<Result> res;
            res = work.Repository<Result>();
            res.Add(new Result()
            {
                Date = DateTime.Now,
                ///Test = inf.Test,
               // User = inf.User,
                Rate = mark
            });
        }
        private void cancel_Click(object sender, EventArgs e)
        {
            work.Dispose();
            listeningSocket.Close();
            foreach (var item in ListOfClients)
            {
                item.Dispose();
            }
            this.Close();
        }
    }
}