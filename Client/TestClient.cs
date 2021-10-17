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
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    
                        MessageBox.Show("reseive answer from server1");
                        Byte[] receiveByte = new byte[16384];
                        sendSocket.Receive(receiveByte);
                        MessageBox.Show("reseive answer from server2");

                        DataSet dt = null;
                        
                    using (MemoryStream ms = new MemoryStream(receiveByte))
                    {
                        BinaryFormatter bFormat = new BinaryFormatter();
                        info = bFormat.Deserialize(ms) as Info;
                        if (info.Msg == "load tests")
                        {
                            if (info.Buffer != null)
                            {
                                using (MemoryStream ms1 = new MemoryStream(info.Buffer))
                                {
                                    MessageBox.Show("reseive answer from server3\n"+info.Buffer.Length);
                                    BinaryFormatter bFormat1 = new BinaryFormatter();
                                    dt = bFormat1.Deserialize(ms1) as DataSet;
                                }
                                if (dt != null)
                                {
                                    MessageBox.Show("Tests");
                                    dataGridView1.Invoke(new Action(()=>dataGridView1.DataSource = dt.Tables[0]));
                                    //dataGridView1.ColumnCount = dt.Columns.Count;
                                    //dataGridView1.RowCount = dt.Rows.Count;
                                    //for (int i = 0; i < dataGridView1.RowCount; i++)
                                    //{
                                    //    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                                    //    {
                                    //        dataGridView1[j, i].Value = dt.Rows[i][j].ToString();
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                }
            });
        }
        static private void SendMes(string m)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] sendByte = new byte[1024];
            info.Msg = m;
            info.IdTest = 3;
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, info);
                sendByte = ms.ToArray();
            }
            socket.Send(sendByte);//send message
            MessageBox.Show("send load tests from client");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //load tests
            SendMes("load tests");
        }
    }
}
