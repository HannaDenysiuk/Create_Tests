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
    public partial class FormAdd : Form
    {
        Test test = new Test();
        Question question;
        XmlSerializer xs = new XmlSerializer(typeof(Test));
        public FormAdd()
        {
            InitializeComponent();
            test.Question = new List<Question>();
        }
       
        private void countQuestion_ValueChanged(object sender, EventArgs e)
        {
           addQuestion.Enabled = true;
        }

        private void addQuestion_Click(object sender, EventArgs e)
        {
            if (textboxQuestion.Text != "")
            {
                question = new Question();
                question.Answer = new List<Answer>();
                question.Description = textboxQuestion.Text;
                question.Difficulty = difficulty.Value.ToString();

                addQuestion.Enabled = false;
                nextQuestion.Enabled = true;
                button1.Enabled = true;
                checkBox1.Enabled = true;
                checkBox1.Checked = false;
            }
        }

        private void button1_Click(object sender, EventArgs e) //add answer
        {
            if (textBox3.Text != "")
            {
                Answer answer = new Answer();
                listBox1.Items.Add(textBox3.Text); //додаємо в список збоку щоб бачити існуючя відповіді
                answer.Description = textBox3.Text;
                answer.IsRight = checkBox1.Checked.ToString();
                question.Answer.Add(answer);// додаємо в список відповідей в клас
                textBox3.Clear();
                if (checkBox1.Checked)
                {
                    checkBox1.Enabled = false;
                    checkBox1.Checked = false;
                }
                save.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void nextQuestion_Click(object sender, EventArgs e)
        {
            Answer an = question.Answer.FirstOrDefault(a => a.IsRight == "True");
            if (an != null) //якщо нема жодної правильної відповіді
            {
                //додаємо зформоване запитаття в тест
                test.Question.Add(question);

                textboxQuestion.Clear();
                textBox3.Clear();
                difficulty.Value = 0;
                listBox1.Items.Clear();
                addQuestion.Enabled = true;
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else MessageBox.Show("set right variant of asnwers");
        }
        private void save_Click(object sender, EventArgs e)
        {
            nextQuestion_Click(sender, e); //щоб зберегти останнє запитання

            test.QuestionCount = countQuestion.Value.ToString();
            saveFileDialog1.Filter = "xml files (*.xml)|*.xml";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate))
                {
                    xs.Serialize(fs, test);
                }
            }
            //очищаємо інші поля
            textBox1.Clear();
            textBox2.Clear();
            countQuestion.Value = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                question.Answer.FirstOrDefault(a => a.IsRight == "True").IsRight = "False";
            }
            catch { }
            checkBox1.Enabled = false;
            checkBox1.Checked = false;
            question.Answer[listBox1.SelectedIndex].IsRight = "True";
        }
    }
}
