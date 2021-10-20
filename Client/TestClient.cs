using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using TestServer;

namespace Client
{
    public partial class TestClient : Form
    {
        public TestClient()
        {
            InitializeComponent();
        } 
        static Info info = null;
        static Socket socket;
        public TestClient( Socket sendSocket,Info info1)
        {
            InitializeComponent();
            info = info1;
            socket = sendSocket;
            label1.Text = info.Lname + "  " + info.Fname;
            if (info.ListOfGroups != null)
            {

                foreach (var item in info.ListOfGroups)
                {
                    comboBox1.Items.Add(item);
                }
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.SelectedIndex = 0;
                    info.Group = comboBox1.SelectedItem.ToString();
                }
            }
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                        Byte[] receiveByte = new byte[16384];
                        sendSocket.Receive(receiveByte);

                    using (MemoryStream ms = new MemoryStream(receiveByte))
                    {
                        BinaryFormatter bFormat = new BinaryFormatter();
                        info = bFormat.Deserialize(ms) as Info;
                        if (info.Msg == "load tests")
                        {
                            if (info.Buffer != null)
                            {
                                DataSet ds = null;
                                using (MemoryStream ms1 = new MemoryStream(info.Buffer))
                                {
                                    ds = bFormat.Deserialize(ms1) as DataSet;
                                }
                                if (ds!=null)
                                {
                                    dataGridView1.Invoke(new Action(() => dataGridView1.DataSource = ds.Tables[0]));
                                    button1.Invoke(new Action(() => button1.Enabled = true));
                                }
                            }
                        }
                        if(info.Msg=="pass test")
                        {
                            if (info.Questions.Count > 0)
                            {
                                PassTest pt = new PassTest(info);
                                pt.ShowDialog();
                                //send answers
                                SendMes("get result");
                            }
                        }
                        if(info.Msg=="get result" && info.Mark!=null)
                        {
                            info.Msg = "";
                            MessageBox.Show("Your result: "+info.Mark+"%");
                        }
                    }
                }
            });
        }
        private void SendMes(string m)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] sendByte = new byte[16384];
            info.Msg = m;
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, info);
                sendByte = ms.ToArray();
            }
            socket.Send(sendByte);//send message
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //load tests
           
            if (comboBox1.Items.Count > 0)
            {
                button1.Invoke(new Action(()=>button1.Enabled = false));
             
                info.Group = comboBox1.SelectedItem.ToString();

                SendMes("load tests");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            info.IdTest = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
            SendMes("pass test");
        }
    }
}
