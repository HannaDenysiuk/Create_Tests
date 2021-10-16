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
                work.SaveChanges();


                MessageBox.Show("Test");

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
                byte[] receivebyte = new byte[1024];

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
            //дивимося чи існує такий користувач
            BinaryFormatter bf = new BinaryFormatter();
            byte[] sendByte = new byte[1024];
            DAL_TestSystem.User us = repoUser.FirstOrDefault(u => u.Login == info.User.Login && u.Password == info.User.Password);
            if (us != null)
            {
                //будемо дивитися на повідомлення що хоче отримати користувач
                if (info.Msg=="")
                {
                    info.IsRegistered = true;
                    info.Msg = "load tests";
                    using (var ms = new MemoryStream())
                    {
                        bf.Serialize(ms, info);
                        sendByte = ms.ToArray();
                    }
                    cl.ClientSocket.Send(sendByte);
                    return;
                }    
                if (info.Msg == "load tests")
                {
                    info.Msg = "pass test";
                    tests = work.Repository<Test>();
                    info.ListOfTests= tests.GetAllData().Select(t => new
                    {
                        id = t.Id,
                        author = t.Author,
                        title = t.TestName,
                        count_of_questions = t.QuestionCount
                    }).ToList();
                    using (var ms = new MemoryStream())
                    {
                        //відправляємо назви список тестів
                         bf.Serialize(ms, info);
                        sendByte = ms.ToArray();
                    }
                    cl.ClientSocket.Send(sendByte);//send message
                    return;
                }
                if (info.Msg == "pass test")
                {
                    //здача тесту
                    info.Msg = "get result";
                    tests = work.Repository<Test>();
                    info.Test = tests.FirstOrDefault(t => t.Id == info.IdTest);
                    using (var ms = new MemoryStream())
                    {
                        //відправляємо назви список тестів
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
                   //
                    //результат здачі тесту
                    IGenericRepository<Result> res;
                    res = work.Repository<Result>();
                    info.Msg = res.FirstOrDefault(u => u.UserId == info.User.Id && u.TestId==info.IdTest).Rate.ToString();
                    info.Test = null;
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
                info.IsRegistered = false;
                info.Msg = "You aren't registered.";
                info.User = null;
                using (var ms = new MemoryStream())
                {
                     bf.Serialize(ms, info);
                    sendByte = ms.ToArray();
                }
                cl.ClientSocket.Send(sendByte);   //відправляємо що він не зареєстрований
                //
                ListOfClients.Remove(cl);
                cl.Dispose(); //перериваємо з ним зв`язок
            }
        }
        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}