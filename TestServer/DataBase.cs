using DAL_TestSystem;
using Repository;
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

namespace TestServer
{
    public partial class DataBase : Form
    {
        public DataBase()
        {
            InitializeComponent();
        }
        GenericUnitOfWork work;
        IGenericRepository<DAL_TestSystem.Group> groups;
        IGenericRepository<DAL_TestSystem.User> users;
        IGenericRepository<DAL_TestSystem.Test> tests;
        IGenericRepository<DAL_TestSystem.Result> results;
        IGenericRepository<TestGroup> testsGroups;
        public DataBase(GenericUnitOfWork work)
        {
            InitializeComponent();
            this.work = work;
            groups = work.Repository<DAL_TestSystem.Group>();
            users = work.Repository<DAL_TestSystem.User>();
            tests = work.Repository<DAL_TestSystem.Test>();
            results = work.Repository<DAL_TestSystem.Result>();
            testsGroups = work.Repository<TestGroup>();
            tabControl2.SelectedIndex = 0;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            //add new group
            if (textBox4NameGroup.Text != "")
            {
                DAL_TestSystem.Group group = new Group() { GroupName = textBox4NameGroup.Text };
                groups.Add(group);
                work.SaveChanges();

                dataGridView2.DataSource = groups.GetAllData().Select(g => new
                { id = g.Id, nameGroup = g.GroupName }).ToList();
                textBox4NameGroup.Clear();
            }
            else
                MessageBox.Show("name of group cann't be null");
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tabControl2.SelectedIndex == 0)
            {
                dataGridView1.DataSource = groups.GetAllData().Select(g => new
                { id = g.Id, nameGroup = g.GroupName }).ToList();
            }
            if (tabControl2.SelectedIndex == 1)
            {
                dataGridView2.DataSource = groups.GetAllData().Select(g => new
                { id = g.Id, nameGroup = g.GroupName }).ToList();
            }
            if (tabControl2.SelectedIndex == 2)
            {
                dataGridView3.DataSource = groups.GetAllData().Select(g => new
                { id = g.Id, nameGroup = g.GroupName }).ToList();
            }
            if (tabControl2.SelectedIndex == 3)
            {
                ////add user to group

                dataGridView6.DataSource = users.GetAllData().Select(u => new
                {//show all users
                    id = u.Id,
                    name = u.FName,
                    surname = u.LName,
                    login = u.Login,
                    password = u.Password,
                    isAdmin = u.IsAdmin
                }).ToList();

                var names = groups.GetAllData().Select(n => n.GroupName).ToList();
                foreach (var item in names)
                {
                    comboBox2.Items.Add(item);
                }
                if (comboBox2.Items.Count > 0)
                    comboBox2.SelectedIndex = 0;
            }
            if (tabControl2.SelectedIndex == 4)
            {
                var names = groups.GetAllData().Select(n => n.GroupName).ToList();
                comboBox1.Items.Clear();
                foreach (var item in names)
                {
                    comboBox1.Items.Add(item);
                }
                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
        }

        private void EditGroup_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                try
                {
                    int id = (int)dataGridView3.SelectedRows[0].Cells[0].Value;
                    groups.FindById(id).GroupName = textBox4.Text;
                    work.SaveChanges();

                    dataGridView3.DataSource = groups.GetAllData().Select(g => new
                    { id = g.Id, nameGroup = g.GroupName }).ToList();
                    textBox4NameGroup.Clear();
                }
                catch { MessageBox.Show("choose Group"); }
            }
            else
                MessageBox.Show("name of group cann't be null");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView4.DataSource = groups.FindAll(g => g.GroupName == comboBox1.SelectedItem.ToString()).
               Select(x => x.Users).FirstOrDefault().Select(u => new
               {
                   id = u.Id,
                   name = u.FName,
                   surname = u.LName,
                   login = u.Login,
                   password = u.Password,
                   isAdmin = u.IsAdmin
               }).ToList();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView5.DataSource = groups.FindAll(g => g.GroupName == comboBox2.SelectedItem.ToString()).
                Select(x => x.Users).FirstOrDefault().Select(u => new
                {
                    id = u.Id,
                    name = u.FName,
                    surname = u.LName,
                    login = u.Login,
                    password = u.Password,
                    isAdmin = u.IsAdmin
                }).ToList();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dataGridView6.SelectedRows[0].Cells[0].Value;

                groups.FirstOrDefault(g => g.GroupName == comboBox2.SelectedItem.ToString()).Users.Add(users.FirstOrDefault(u => u.Id == id));
                work.SaveChanges();

                comboBox2_SelectedIndexChanged(sender,e);

            }
            catch { MessageBox.Show("you should select row"); }
        }

        private void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count > 0)
                textBox4.Text = dataGridView3.SelectedRows[0].Cells[1].Value.ToString();
        }

        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl3.SelectedIndex == 0)
            {
                dataGridView7.DataSource = users.GetAllData().Select(u => new
                {//show all users
                    id = u.Id,
                    name = u.FName,
                    surname = u.LName,
                    login = u.Login,
                    password = u.Password,
                    isAdmin = u.IsAdmin
                }).ToList();
            }
            if (tabControl3.SelectedIndex == 1)
            {
                dataGridView8.DataSource = users.GetAllData().Select(u => new
                {//show all users
                    id = u.Id,
                    name = u.FName,
                    surname = u.LName,
                    login = u.Login,
                    password = u.Password,
                    isAdmin = u.IsAdmin
                }).ToList();
            }
            if (tabControl3.SelectedIndex == 2)
            {
                dataGridView9.DataSource = users.GetAllData().Select(u => new
                {//show all users
                    id = u.Id,
                    name = u.FName,
                    surname = u.LName,
                    login = u.Login,
                    password = u.Password,
                    isAdmin = u.IsAdmin
                }).ToList();
            }
        }

        private void AddUser_Click(object sender, EventArgs e)
        {

            if (name.Text != "" && surname.Text != "" && login.Text != "" && password.Text != "")
            {
                try
                {
                    users.Add(new User()
                    {
                        FName = name.Text,
                        LName = surname.Text,
                        Login = login.Text,
                        Password = password.Text,
                        IsAdmin = checkBox1.Checked
                    });
                    work.SaveChanges();


                    name.Clear();
                    surname.Clear();
                    login.Clear();
                    password.Clear();
                    dataGridView8.DataSource = users.GetAllData().Select(u => new
                    {//show all users
                        id = u.Id,
                        name = u.FName,
                        surname = u.LName,
                        login = u.Login,
                        password = u.Password,
                        isAdmin = u.IsAdmin
                    }).ToList();
                }
                catch { }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox6.Text != "" && textBox7.Text != "" && textBox8.Text != "" && textBox5.Text != "")
            {
                try
                {
                    int id = (int)dataGridView9.SelectedRows[0].Cells[0].Value;

                    users.FindById(id).FName = textBox6.Text;
                    users.FindById(id).LName = textBox7.Text;
                    users.FindById(id).Login = textBox8.Text;
                    users.FindById(id).Password = textBox5.Text;
                    users.FindById(id).IsAdmin = Convert.ToBoolean(checkBox1.Checked);

                    work.SaveChanges();


                    textBox6.Clear();
                    textBox7.Clear();
                    textBox8.Clear();
                    textBox5.Clear();
                    dataGridView9.DataSource = users.GetAllData().Select(u => new
                    {//show all users
                        id = u.Id,
                        name = u.FName,
                        surname = u.LName,
                        login = u.Login,
                        password = u.Password,
                        isAdmin = u.IsAdmin
                    }).ToList();
                }
                catch { }
            }
        }

        private void dataGridView9_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView9.SelectedRows.Count > 0)
            {
                textBox6.Text = dataGridView9.SelectedRows[0].Cells[1].Value.ToString();
                textBox7.Text = dataGridView9.SelectedRows[0].Cells[2].Value.ToString();
                textBox8.Text = dataGridView9.SelectedRows[0].Cells[3].Value.ToString();
                textBox5.Text = dataGridView9.SelectedRows[0].Cells[4].Value.ToString();
                checkBox2.Checked = Convert.ToBoolean(dataGridView9.SelectedRows[0].Cells[5].Value);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 3)
            {
                dataGridView10.DataSource = results.GetAllData().Select(r => new
                {
                    id = r.Id,
                    date = r.Date.ToShortDateString(),
                    testId = r.TestId,
                    userId = r.UserId,
                    rate = r.Rate
                }).ToList();
            }
        }

        private void tabControl4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl4.SelectedIndex == 0)
            {
                dataGridView11.DataSource = tests.GetAllData().Select(t => new
                {
                    id = t.Id,
                    author = t.Author,
                    title = t.TestName,
                    count_of_questions = t.QuestionCount,
                }).ToList();
            }
            if (tabControl4.SelectedIndex == 1)
            {
                dataGridView13.DataSource = testsGroups.GetAllData().Select(tg => new
                {
                    id=tg.Id,
                    testId=tg.TestId,
                    groupId=tg.GroupId,
                    date = tg.Date.ToShortDateString()
                }).ToList();
            }
            if (tabControl4.SelectedIndex == 2)
            {
                dataGridView12.DataSource = tests.GetAllData().Select(t => new
                {
                    id = t.Id,
                    author = t.Author,
                    title = t.TestName,
                    count_of_questions = t.QuestionCount,
                }).ToList();
            }
            if (tabControl4.SelectedIndex == 3)
            {
                dataGridView14.DataSource = tests.GetAllData().Select(t => new
                {
                    id = t.Id,
                    author = t.Author,
                    title = t.TestName,
                    count_of_questions = t.QuestionCount,
                }).ToList();
                var names = groups.GetAllData().Select(n => n.GroupName).ToList();
                comboBox3.Items.Clear();
                foreach (var item in names)
                {
                    comboBox3.Items.Add(item);
                }
                if (comboBox3.Items.Count > 0)
                    comboBox3.SelectedIndex = 0;
            }
        }

        XmlSerializer xs = new XmlSerializer(typeof(Xml2CSharp.Test));
        Xml2CSharp.Test test;
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "xml files (*.xml)|*.xml";

            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    test = (Xml2CSharp.Test)xs.Deserialize(fs);
                }
                if (test != null)
                {
                    textBox2.Text = test.Author;
                    textBox3.Text = test.TestName;
                    textBox9.Text = test.QuestionCount;
                }
            }
        }

        private void loadTest_Click(object sender, EventArgs e)
        {
            if (test != null)
            {
                //створюю тест
                DAL_TestSystem.Test tdb= new DAL_TestSystem.Test()
                { Author = test.Author, 
                    TestName = test.TestName,
                    QuestionCount = Convert.ToInt32(test.QuestionCount)
                };

                tests.Add(tdb);//додаю
                work.SaveChanges();//зберігаю зміни
                //тягну його id
                int idTEst = tests.GetAllData().Max(t => t.Id);

                IGenericRepository<DAL_TestSystem.Question> questions;
                IGenericRepository<DAL_TestSystem.Answer> answers;

                questions = work.Repository<DAL_TestSystem.Question>();
                answers = work.Repository<DAL_TestSystem.Answer>();

                foreach (var item in test.Question)
                {//записую всі по черзі питання з тесту
                    DAL_TestSystem.Question q = new DAL_TestSystem.Question()
                    {
                        Description = item.Description,
                        Difficalty = Convert.ToInt32(item.Difficulty),
                        TestId = idTEst,
                    };
                    questions.Add(q);//додаю
                    work.SaveChanges();//зберігаю зміни
                                       //тягну його id
                    int idq = questions.GetAllData().Max(x => x.Id);

                    foreach (var i in item.Answer)
                    {//додаю всі варіанти відповідей конкретного питання
                        DAL_TestSystem.Answer answer = new DAL_TestSystem.Answer()
                        {
                            Description = i.Description,
                            IsRight = Convert.ToBoolean(i.IsRight),
                            QuestionId = idq
                        };
                        answers.Add(answer);//додаю
                        work.SaveChanges();//зберігаю зміни
                    }
                }

                dataGridView12.DataSource = tests.GetAllData().Select(t => new
                {
                    id = t.Id,
                    author = t.Author,
                    title = t.TestName,
                    count_of_questions = t.QuestionCount,
                }).ToList();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = groups.FirstOrDefault(g => g.GroupName == comboBox3.SelectedItem.ToString()).Id;

            dataGridView15.DataSource = testsGroups.FindAll(g => g.GroupId == id).
                Select(tg => new
                {
                    id = tg.Id,
                    testId = tg.TestId,
                    groupId = tg.GroupId,
                    date = tg.Date.ToShortDateString()
                }).ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dataGridView14.SelectedRows[0].Cells[0].Value;
                testsGroups.Add(new TestGroup()
                {
                    Date = DateTime.Now,
                    Test = tests.FindById(id),
                    Group = groups.FirstOrDefault(n => n.GroupName == comboBox3.SelectedItem.ToString())
                });
                work.SaveChanges();

                comboBox3_SelectedIndexChanged(sender, e);
            }
            catch { MessageBox.Show("you should select row"); }
        }
    }
}