using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Exercise3
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
            HMAC myHashAlg = HMAC.Create();
            if (File.Exists(KeyFileTextBox.Text))
            {
                using (FileStream fskeyfile = new FileStream(KeyFileTextBox.Text, FileMode.Open))
                {
                    byte[] key = new byte[fskeyfile.Length];
                    fskeyfile.Read(key, 0, key.Length);
                    myHashAlg.Key = key;
                }
            }
            else
            {
                using (FileStream fskeyfile = new FileStream(KeyFileTextBox.Text, FileMode.Create))
                {
                    fskeyfile.Write(myHashAlg.Key, 0, myHashAlg.Key.Length);
                }
            }
            byte[] hValue;
            using (FileStream inFile = new FileStream(FileSignTextBox.Text, FileMode.Open))
            {
                hValue = myHashAlg.ComputeHash(inFile);
            }
            using (FileStream hFile = new FileStream(SignatureFileTextBox.Text, FileMode.Create))
            {
                hFile.Write(hValue, 0, hValue.Length);
            }
            MessageBox.Show("File signed!");
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
            HMAC myHashAlg = HMAC.Create();
            using (FileStream fskeyfile = new FileStream(KeyFileVerifyTextBox.Text, FileMode.Open))
            {
                byte[] key = new byte[fskeyfile.Length];
                fskeyfile.Read(key, 0, key.Length);
                fskeyfile.Close();
                myHashAlg.Key = key;
            }
            byte[] hValue;
            using (FileStream inFile = new FileStream(FileVerifyTextBox.Text, FileMode.Open))
            {
                hValue = myHashAlg.ComputeHash(inFile);
                inFile.Close();
            }
            byte[] orghValue;
            using (FileStream hFile = new FileStream(SignatureFileVerifyTextBox.Text, FileMode.Open))
            {
                orghValue = new byte[hFile.Length];
                hFile.Read(orghValue, 0, orghValue.Length);
            }
            bool identical = true;
            for (int i = 0; i < hValue.Length; i++)
                if (hValue[i] != orghValue[i])
                {
                    identical = false;
                    break;
                }
            if (identical)
                MessageBox.Show("Files are identical!");
            else
                MessageBox.Show("Files are not identical!");


        }

        private void BrowseKeyFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFiledialog = new SaveFileDialog();
            if (saveFiledialog.ShowDialog() == DialogResult.OK)
            {
                KeyFileTextBox.Text = saveFiledialog.FileName;
            }

        }

        private void BrowseFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            if (openFiledialog.ShowDialog() == DialogResult.OK)
            {
                FileSignTextBox.Text = openFiledialog.FileName;
            }

        }

        private void BrowseKeyFileVerifyButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiledialog = new OpenFileDialog();
            if (openFiledialog.ShowDialog() == DialogResult.OK)
            {
                KeyFileVerifyTextBox.Text = openFiledialog.FileName;
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
    }
}
