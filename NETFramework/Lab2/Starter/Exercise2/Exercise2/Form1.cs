using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BrowseEncryptButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            if (openFiledialog.ShowDialog() == DialogResult.OK)
            {
                FileEncryptTextBox.Text = openFiledialog.FileName;
            }
        }

        private void EncryptButton_Click(object sender, EventArgs e)
        {

        }

        private void BrowseDecryptButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            openFiledialog.Filter = "Encrypted Files|*.encrypted";
            if (openFiledialog.ShowDialog() == DialogResult.OK)
            {
                FileDecryptTextBox.Text = openFiledialog.FileName;
            }
        }

        private void DecryptButton_Click(object sender, EventArgs e)
        {

        }
    }
}
