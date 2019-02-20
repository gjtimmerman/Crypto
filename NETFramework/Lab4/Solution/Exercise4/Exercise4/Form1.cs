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
using System.Security.Cryptography.X509Certificates;

namespace Exercise4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private X509Certificate2 myCertificate;

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
            RSA myProv;
            if (myCertificate == null)
                MessageBox.Show("No Certificate selected");
            if (myCertificate.PrivateKey is RSA)
                myProv = (RSA)myCertificate.PrivateKey;
            else
            {
                myProv = RSA.Create();
                myProv.FromXmlString(myCertificate.PrivateKey.ToXmlString(true));
            }
            byte[] hash;
            using (FileStream fsin = new FileStream(FileSignTextBox.Text, FileMode.Open))
            {
                hash = myProv.SignData(fsin, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
            using (FileStream fshash = new FileStream(SignatureFileTextBox.Text, FileMode.Create))
            {
                fshash.Write(hash, 0, hash.Length);
            }

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
            RSA myProv;
            if (myCertificate == null)
                MessageBox.Show("No Certificate selected");
            if (myCertificate.PrivateKey is RSA)
                myProv = (RSA)myCertificate.PublicKey.Key;
            else
            {
                myProv = RSA.Create();
                myProv.FromXmlString(myCertificate.PublicKey.Key.ToXmlString(false));
            }
            byte[] sign;
            using (FileStream fssign = new FileStream(SignatureFileVerifyTextBox.Text, FileMode.Open))
            {
                sign = new byte[fssign.Length];
                fssign.Read(sign, 0, sign.Length);
            }
            using (FileStream fsin = new FileStream(FileVerifyTextBox.Text, FileMode.Open))
            {
                if (myProv.VerifyData(fsin, sign, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1))
                    MessageBox.Show("Signature is valid");
                else
                    MessageBox.Show("Signature is not valid");
            }

        }

        private void ChooseCertificateSignButton_Click(object sender, EventArgs e)
        {
            X509Store myStore = new X509Store(StoreName.My,StoreLocation.CurrentUser);
            myStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection myCollection = X509Certificate2UI.SelectFromCollection(myStore.Certificates, "Certificates", "Please select a Certificate for signing", X509SelectionFlag.SingleSelection);
            myStore.Close();
            if (myCollection.Count > 0)
                myCertificate = myCollection[0];
            else
                MessageBox.Show("No Certificate selected");
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
            X509Store myStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            myStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection myCollection = X509Certificate2UI.SelectFromCollection(myStore.Certificates, "Certificates", "Please select a Certificate for signing", X509SelectionFlag.SingleSelection);
            myStore.Close();
            if (myCollection.Count > 0)
                myCertificate = myCollection[0];
            else
                MessageBox.Show("No Certificate selected");

        }
    }
}
