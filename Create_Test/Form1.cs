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
    public partial class FormTests : Form
    {
        public FormTests()
        {
            InitializeComponent();
        }

        private void add_Click(object sender, EventArgs e)
        {
            FormAdd Add = new FormAdd();
            Add.ShowDialog();
        }

        private void edit_Click(object sender, EventArgs e)
        {
            FormEdit edit = new FormEdit();
            edit.ShowDialog();
        }
    }
}
