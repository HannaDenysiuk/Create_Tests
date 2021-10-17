using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestServer;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }
       static bool flag = false;
        static Socket sendSocket;
        IPAddress iPAddress;
        TestClient tc;
        private void ok_Click(object sender, EventArgs e)
        {
            sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPHostEntry iPHost = Dns.GetHostEntry("localhost");

            iPAddress = iPHost.AddressList[1];//Мережева картка 
            
            try
            {
                int port = 33000;
                IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);

                sendSocket.Connect(iPEndPoint);

                BinaryFormatter bf = new BinaryFormatter();
                byte[] sendByte = new byte[1024];
                Info info = new Info();
                info.Login= textBox1.Text;
                info.Password = textBox2.Text;
                info.Msg = "";
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, info);
                    sendByte = ms.ToArray();
                }
                sendSocket.Send(sendByte);


                //with Task
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Byte[] receiveByte = new byte[16384];
                        sendSocket.Receive(receiveByte);
                        if(!flag)
                        { string mes;
                            using (var memStream = new MemoryStream())
                            {
                                var binForm = new BinaryFormatter();
                                memStream.Write(receiveByte, 0, receiveByte.Length);
                                memStream.Seek(0, SeekOrigin.Begin);
                                mes = (string)binForm.Deserialize(memStream);
                            }
                            if (mes == "isUser")//якщо він є в базі
                            {
                                MessageBox.Show("isUser");
                                break;
                            }
                        }
                    }//відкриваємо наступне вікно
                    tc = new TestClient(sendSocket, info);
                    tc.ShowDialog();
                    flag = true;
                    //return;
                });
            }
            catch { MessageBox.Show("Something wrong"); }
        }
        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] sendByte = new byte[1024];
            using (var ms = new MemoryStream())
            {
                Info p = new Info() { Msg = "close" };

                bf.Serialize(ms, p);
                sendByte = ms.ToArray();
            }
            sendSocket.Send(sendByte);//send message
        }
    }
}
