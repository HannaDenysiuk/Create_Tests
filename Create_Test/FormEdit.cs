using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Xml2CSharp;

namespace Create_Test
{
    public partial class FormEdit : Form
    {
        public FormEdit()
        {
            InitializeComponent();
        }
        Question question;
        XmlSerializer xs = new XmlSerializer(typeof(Test));
        Test test;
        string path = @"D:\IT_Step_Academy\Exam_CreateTests\Create_Tests\CreateNewTest\bin\Debug\Tests\";
        private void file_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = path;
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Filter = "xml files (*.xml)|*.xml";

            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
                this.Text = path;
            }
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                test = (Test)xs.Deserialize(fs);
            }
            if (test != null)
            {
                foreach (var item in test.Question)
                {
                    listBox1.Items.Add(item.Description);
                }
                countQuestion.Value = Convert.ToInt32(test.QuestionCount);
                textBox1.Text = test.Author;
                textBox2.Text = test.TestName;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            textBox5.Clear();
            textBox4.Clear();

            editQuestion.Enabled = true;
            button1.Enabled = true;
            question = test.Question[listBox1.SelectedIndex];
            textBox4.Text = question.Description;
            difficulty.Value = Convert.ToInt32(question.Difficulty);
            foreach (var item in question.Answer)
            {
                listBox2.Items.Add(item.Description);
            }
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox5.Clear();
            button4.Enabled = true;
            button3.Enabled = true;
            checkBox1.Checked = false;
            Answer answer = question.Answer.FirstOrDefault(a => a.Description == listBox2.SelectedItem.ToString());

            textBox5.Text = answer.Description;
            if (answer.IsRight.ToString().Equals("True"))
                checkBox1.Checked = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {

                question.Answer[listBox2.SelectedIndex].Description = textBox5.Text;
                question.Answer[listBox2.SelectedIndex].IsRight = checkBox1.Checked.ToString();
                listBox2.SelectedItem = textBox5.Text;

                Answer an = question.Answer.FirstOrDefault(a => a.IsRight == "True");
                if (an == null) //якщо нема жодної правильної відповіді
                    question.Answer[listBox2.SelectedIndex].IsRight = "True";
               
            }
            else MessageBox.Show("Answer cann't be null");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (question.Answer.Count > 1)
            {
                textBox5.Clear();
                int index = listBox2.SelectedIndex;
                question.Answer.RemoveAt(index);
                listBox2.Items.RemoveAt(index);
            }
            else
                MessageBox.Show("you cann't remove last answer of question");
        }

        private void nextQuestion_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                test.Question[listBox1.SelectedIndex].Description = textBox4.Text;
                test.Question[listBox1.SelectedIndex].Difficulty = difficulty.Value.ToString();
                listBox1.SelectedItem = textBox4.Text;
                //question.Answer[listBox2.SelectedIndex].Description = textBox5.Text;
               
            }
            else MessageBox.Show("Question cann't be null");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (test.Question.Count > 1)
            {
                int index = listBox1.SelectedIndex;
                test.Question.RemoveAt(index);
                textBox4.Clear();
                difficulty.Value = 0;
                countQuestion.Value -= 1;
            }
            else
                MessageBox.Show("you cann't remove last question of test");
        }

        private void save_Click(object sender, EventArgs e)
        {

            test.Author = textBox1.Text;
            test.TestName = textBox2.Text;
            test.QuestionCount = countQuestion.Value.ToString();

            if (File.Exists(path))
                using (FileStream fs = new FileStream(path, FileMode.Truncate))
                {
                    xs.Serialize(fs, test);
                }
            else
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    xs.Serialize(fs, test);
                }
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox5.Clear();
            textBox4.Clear();

        }
    }
}
