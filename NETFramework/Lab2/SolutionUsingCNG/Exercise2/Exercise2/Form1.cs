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
            RSACng myRSAProv;
            if (CngKey.Exists(KeyContainerEncryptTextBox.Text))
                myRSAProv = new RSACng(CngKey.Open(KeyContainerEncryptTextBox.Text));
            else
                myRSAProv = new RSACng(CngKey.Create(CngAlgorithm.Rsa, KeyContainerEncryptTextBox.Text));


            RSAOAEPKeyExchangeFormatter myKeyFormatter = new RSAOAEPKeyExchangeFormatter(myRSAProv);
            AesCng mySymmProv = new AesCng();
            byte[] encrKey = myKeyFormatter.CreateKeyExchange(mySymmProv.Key);
            byte[] data = File.ReadAllBytes(FileEncryptTextBox.Text);
            using (FileStream fsout = new FileStream(FileEncryptTextBox.Text + ".encrypted", FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fsout))
                {
                    bw.Write(encrKey.Length);
                    bw.Write(encrKey, 0, encrKey.Length);
                    bw.Write(mySymmProv.IV.Length);
                    bw.Write(mySymmProv.IV, 0, mySymmProv.IV.Length);
                    bw.Flush();
                    using (CryptoStream cs = new CryptoStream(fsout, mySymmProv.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (BinaryWriter sw = new BinaryWriter(cs))
                        {
                            sw.Write(data);
                        }
                    }
                }
            }
            MessageBox.Show("Encryption completed.");
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
            RSA myRSAProv;
            if (CngKey.Exists(KeyContainerEncryptTextBox.Text))
                myRSAProv = new RSACng(CngKey.Open(KeyContainerDecryptTextBox.Text));
            else
                myRSAProv = new RSACng(CngKey.Create(CngAlgorithm.Rsa, KeyContainerDecryptTextBox.Text));

            RSAOAEPKeyExchangeDeformatter myKeyFormatter = new RSAOAEPKeyExchangeDeformatter(myRSAProv);
            AesCng mySymmProv = new AesCng();
            using (FileStream fs = new FileStream(FileDecryptTextBox.Text, FileMode.Open))
            {
                int keyLength;
                byte[] encrKey;
                int ivLength;
                byte[] IV;
                using (BinaryReader br = new BinaryReader(fs))
                {
                    keyLength = br.ReadInt32();
                    encrKey = new byte[keyLength];
                    br.Read(encrKey, 0, keyLength);
                    ivLength = br.ReadInt32();
                    IV = new byte[ivLength];
                    br.Read(IV, 0, ivLength);

                    byte[] Key = myKeyFormatter.DecryptKeyExchange(encrKey);
                    mySymmProv.Key = Key;
                    mySymmProv.IV = IV;
                    using (CryptoStream cs = new CryptoStream(fs, mySymmProv.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        int dataLen = (int)fs.Length - 4 - keyLength - 4 - ivLength;
                        byte[] data = new byte[dataLen];
                        int len = cs.Read(data, 0, dataLen);
                        using (FileStream outStream = File.Open(FileDecryptTextBox.Text.Replace("encrypted", "decrypted"), FileMode.OpenOrCreate))
                        {
                            outStream.Write(data, 0, len);
                        }
                    }
                }
            }
            MessageBox.Show("Decryption completed.");
        }
    }
}
