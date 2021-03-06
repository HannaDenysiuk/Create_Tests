using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestServer;

namespace Client
{
    public partial class PassTest : Form
    {
        public PassTest()
        {
            InitializeComponent();
        }
        static Info info;
        static int num = 0;
        public PassTest(Info inf)
        {
            InitializeComponent();
            info = inf;
            num = inf.Questions.Count();
            info.UserAnswers = new List<string>();
            textBox1.Text = info.Questions.First().Value.ToString();
            foreach (var item in info.Answers.First().Value)
            {
                listBox1.Items.Add(item.ToString());
            }
            label3.Text = "1 / " + num.ToString();
        }
        private void NextQuestion_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem!=null)
            {
                label3.Text = info.Questions.Count().ToString() + " / " + num.ToString();

                //дописувати в інфо відповіді
                info.UserAnswers.Add(listBox1.SelectedItem.ToString());

                if (info.Questions.Count > 1)
                {
                    info.Questions.Remove(info.Questions.First().Key);
                    textBox1.Text = info.Questions.First().Value.ToString();
                    listBox1.Items.Clear();

                    foreach (var item in info.Answers[info.Questions.First().Key])
                        listBox1.Items.Add(item.ToString());
                }
                else
                {
                    //якщо питань більше немає
                    this.Close();
                }
            }
        }
    }
}
