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
    public partial class FormDataBase : Form
    {
        public FormDataBase()
        {
            InitializeComponent();
        }
        GenericUnitOfWork work;
        User user1 = null;
        IGenericRepository<Group> group;
        IGenericRepository<User> users;
        IGenericRepository<Test> tests;
        IGenericRepository<Result> results;
        public FormDataBase(GenericUnitOfWork work, User user1)
        {
            InitializeComponent();
            this.work = work;
            this.user1 = user1;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            group = work.Repository<Group>();
            users = work.Repository<User>();
            tests = work.Repository<Test>();
            results = work.Repository<Result>();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //show groups
            dataGridView1.DataSource = group.GetAllData().Select(g => new { id = g.Id, name_of_group = g.GroupName }).ToList();
        }

        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //show all users
            dataGridView1.DataSource = users.GetAllData().Select(u => new
            {
                id = u.Id,
                name = u.FName,
                surname = u.LName,
                login = u.Login,
                password = u.Password,
                isAdmin = u.IsAdmin
            }).ToList();

        }

        private void loadTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //show tests
            dataGridView1.DataSource = tests.GetAllData().Select(t => new
            {
                id = t.Id,
                author = t.Author,
                title = t.TestName,
                count_of_questions = t.QuestionCount
            }).ToList();
        }

        private void loadTestToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Test test;
            string path;
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
                XmlSerializer xs = new XmlSerializer(typeof(Test));
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    test = (Test)xs.Deserialize(fs);

                    tests.Add(test);
                    work.SaveChanges();
                }
            }
        }
    }
}
