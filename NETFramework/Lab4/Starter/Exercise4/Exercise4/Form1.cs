using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BrowseSignatureFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFiledialog = new SaveFileDialog();
            if (saveFiledialog.ShowDialog() == DialogResult.OK)
            {
                SignatureFileTextBox.Text = saveFiledialog.FileName;
            }
        }

        private void SignButton_Click(object sender, EventArgs e)
        {

        }

        private void BrowseSignatureFileVerifyButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            if (openFiledialog.ShowDialog() == DialogResult.OK)
            {
                SignatureFileVerifyTextBox.Text = openFiledialog.FileName;
            }

        }

        private void VerifyButton_Click(object sender, EventArgs e)
        {

        }

        private void ChooseCertificateSignButton_Click(object sender, EventArgs e)
        {

        }

        private void BrowseFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            if (openFiledialog.ShowDialog() == DialogResult.OK)
            {
                FileSignTextBox.Text = openFiledialog.FileName;
            }

        }

 
        private void BrowseFileVerifyButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            if (openFiledialog.ShowDialog() == DialogResult.OK)
            {
                FileVerifyTextBox.Text = openFiledialog.FileName;
            }

        }

        private void ChooseCertificateVerifyButton_Click(object sender, EventArgs e)
        {

        }
    }
}
