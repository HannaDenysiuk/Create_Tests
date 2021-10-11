using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xml2CSharp;

namespace Create_Test
{
    public partial class FormAdd : Form
    {
        Test test = new Test();
        Answer answer = new Answer();
       // List<Answer> answers = new List<Answer>();
        Question question = new Question();
        //List<Question> questions = new List<Question>();
        public FormAdd()
        {
            InitializeComponent();
            test.Question = new List<Question>();
            question.Answer = new List<Answer>();
        }
       
        private void countQuestion_ValueChanged(object sender, EventArgs e)
        {
           test.QuestionCount = countQuestion.Value.ToString();
           addQuestion.Enabled = true;
        }

        private void addQuestion_Click(object sender, EventArgs e)
        {
            if (textboxQuestion.Text != "")
            {
                question.Description = textboxQuestion.Text;
                question.Difficulty = difficulty.Value.ToString();


                button1.Enabled = true;
                checkBox1.Enabled = true;
                checkBox1.Checked = false;
            }
        }

        private void button1_Click(object sender, EventArgs e) //add answer
        {
            if (textBox3.Text != "")
            {
                listBox1.Items.Add(textBox3.Text);
                answer.Description = textBox3.Text;
                answer.IsRight = checkBox1.Checked.ToString();
                question.Answer.Add(answer);
               
                if (checkBox1.Checked)
                {
                    checkBox1.Enabled = false;
                    checkBox1.Checked = false;
                }
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (nameOfFile.Text != "")
            {
                test.Author = textBox1.Text;
                test.TestName = textBox2.Text;
                string file = nameOfFile.Text + ".xml";




            }
        }

        private void nextQuestion_Click(object sender, EventArgs e)
        {
            
            //додаємо зформоване запитаття в тест
            test.Question.Add(question);

            textboxQuestion.Clear();
            textBox3.Clear();
            difficulty.Value = 0;

            addQuestion.Enabled = true;
            button1.Enabled = false;
        }
    }
}
