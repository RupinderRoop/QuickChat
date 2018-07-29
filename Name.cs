using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatApplication
{
    public partial class Name : Form
    {
        public Name()
        {
            InitializeComponent();
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            if(NameTextBox.Text == String.Empty)
            {
                MessageBox.Show("Enter your name", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MainWindow mWindow = new MainWindow();
            Data.Name = NameTextBox.Text.ToString();
            mWindow.Show();
            this.Hide();
        }
    }
}
