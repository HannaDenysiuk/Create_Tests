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
        static IGenericRepository<Group> groups;
        static IGenericRepository<TestGroup> testGroups;
        static  IGenericRepository<Answer> answers;
        static IGenericRepository<Result> res;
        static IGenericRepository<UserAnswer> userAnswers;
        User user1 = null;
        static Socket listeningSocket;// only for listen
       
        static List<Client> ListOfClients = new List<Client>();//хто буде проходити тести
       
        public Form1()
        {
            InitializeComponent();
            work = new GenericUnitOfWork(new Context(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            repoUser = work.Repository<User>();
            tests = work.Repository<Test>();
            groups = work.Repository<Group>();
            userAnswers = work.Repository<UserAnswer>();
            testGroups = work.Repository<TestGroup>();
            answers = work.Repository<Answer>();
            res = work.Repository<Result>();
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
                db.Show();//форма для роботи з базою даних
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
           
            while (true)//постійно читає (чекаємо на повідомлення клієнта)
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
                    info.UserId = us.Id;
                    info.Lname = us.LName;
                    info.Fname = us.FName;
                    info.ListOfGroups = new List<string>();
                    info.ListOfGroups.AddRange(us.Groups.Select(item => item.GroupName));//всі групи в яких є користувач
                    //посилаємо список груп в яких є користувач

                    info.Msg = "isUser"; //посилаємо сигнал щоб відкрилася інша форма
                    
                    using (var ms = new MemoryStream())
                    {
                        bf.Serialize(ms, info);
                        sendByte = ms.ToArray();
                    }
                    cl.ClientSocket.Send(sendByte);
                    return;
                }
                if (info.Msg == "load tests")//натиснув кнопку показати тести які він має здати
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Id", typeof(Int32));
                    dt.Columns.Add("Author", typeof(string));
                    dt.Columns.Add("TestName", typeof(string));
                    dt.Columns.Add("QuestionCount", typeof(Int32));

                    //дістаємо id групи яку хоче користувач подивитися здачу тестів
                    int grId = groups.FirstOrDefault(g => g.GroupName == info.Group).Id;
                    try
                    {
                        foreach (TestGroup i in testGroups.GetAllData().Where(g => g.GroupId == grId))
                        {
                            //створюємо табличку- список тестів для проходження
                            DAL_TestSystem.Test item = tests.FirstOrDefault(t => t.Id == i.TestId);
                            dt.Rows.Add(item.Id, item.Author, item.TestName, item.QuestionCount);
                        }
                    }
                    catch { }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, ds);
                        info.Buffer = ms.ToArray();
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //відправляємо тести
                        bf.Serialize(ms, info);
                        sendByte = ms.ToArray();
                    }
                    cl.ClientSocket.Send(sendByte);//send message
                    return;
                }
                if (info.Msg == "pass test")
                {
                    //здача тесту
                    //тест який обрав користувач
                    try
                    {
                        DAL_TestSystem.Test test1 = tests.FirstOrDefault(t => t.Id == info.IdTest);

                        info.Answers = new Dictionary<int, List<string>>();
                        info.Questions = new Dictionary<int, string>();

                        if (test1 != null)
                        {
                            foreach (var item in test1.Questions)
                            {
                                var answ = answers.GetAllData().Where(a => a.QuestionId == item.Id);
                                List<string> anss = new List<string>();

                                foreach (var i in answ)
                                {
                                    anss.Add(i.Description);
                                }
                                info.Answers.Add(item.Id, anss);

                                info.Questions.Add(item.Id, item.Description);
                            }
                        }
                    }
                    catch { }
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
                    if (info.UserAnswers.Count > 0)
                    {
                        //додаємо відповіді користувача в базу
                        foreach (var item in info.UserAnswers)
                        {
                            userAnswers.Add(new UserAnswer()
                            {
                                User = us,
                                Answer = answers.FirstOrDefault(a => a.Description == item),
                                Date = DateTime.Now
                            });
                        }
                        work.SaveChanges();

                        //рахуємо результат
                        CalculateMark(info);

                        //результат здачі тесту треба брати по самій свіжій даті
                        info.Mark = res.GetAllData().OrderByDescending(d => d.Date).FirstOrDefault(u => u.UserId == info.UserId && u.TestId == info.IdTest).Rate;

                        using (var ms = new MemoryStream())
                        {
                            //відправляємо результат
                            bf.Serialize(ms, info);
                            sendByte = ms.ToArray();
                        }
                        cl.ClientSocket.Send(sendByte);//send message
                        return;
                    }
                }
            }
            else
            {
               info.Msg = "You aren't registered.\nTry again.";
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, info);
                    sendByte = ms.ToArray();
                }
                cl.ClientSocket.Send(sendByte);   //відправляємо що він не зареєстрований
            }
        }
        private static void CalculateMark(Info inf)
        {
            double mark = 0;
            int value = 0, failed = 0;
            DAL_TestSystem.Test test = tests.FirstOrDefault(t => t.Id == inf.IdTest);
            
            foreach (var item in test.Questions)
            {
                value += item.Difficalty; //додаємо вагу всіх питань

                //шукаємо id вірної відповіді
                int id1 = answers.FirstOrDefault(a => a.QuestionId == item.Id && a.IsRight == true).Id;
                //треба брати по самій свіжій даті id - відповіть користувача
                int id2 = userAnswers.GetAllData().OrderByDescending(d => d.Date).FirstOrDefault(u => u.Answer.QuestionId == item.Id).Answer.Id;
                if (id1 != id2)
                {
                    failed += item.Difficalty;//рахуємо вагу невірних відповідей
                }
            }

            //рахуємо результат
            mark = (value - failed) * 100 / value;

            //add Result to data base
            res.Add(new Result()
            {
                Date = DateTime.Now,
                Test = test,
                User = repoUser.FirstOrDefault(u => u.Login == inf.Login && u.Password == inf.Password),
                Rate = mark
            });
            work.SaveChanges();
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