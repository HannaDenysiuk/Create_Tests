using DAL_TestSystem;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestServer
{
    public partial class Form1 : Form
    {
        GenericUnitOfWork work;
        IGenericRepository<User> repoUser;

        User user1 = null;
        public Form1()
        {
            InitializeComponent();
            work = new GenericUnitOfWork(new Context(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            repoUser = work.Repository<User>();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            work.Dispose();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            user1 = repoUser.FirstOrDefault(u => u.Login == textBox1.Text && u.Password == textBox2.Text);
            if (user1 != null && user1.IsAdmin == true)
            {
                DataBase db = new DataBase(work);
                db.ShowDialog();
                work.SaveChanges();
            }
            else
                MessageBox.Show("you aren't admin");
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}