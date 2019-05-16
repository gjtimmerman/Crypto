using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Encryption
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
            AesCng algorithm = new AesCng();
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            // salt has to be as big as the blocksize. Blocksize is in bits, salt size is in bytes!
            byte[] salt = new byte[algorithm.BlockSize / 8];
            randomNumberGenerator.GetBytes(salt);
            Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(PasswordEncryptTextBox.Text, salt);
            //key size is in bits!
            byte[] key = keyGenerator.GetBytes(algorithm.KeySize / 8);
            byte[] iv = algorithm.IV;
            algorithm.Key = key;

            ICryptoTransform cryptoTransformer = algorithm.CreateEncryptor();
            byte[] data = File.ReadAllBytes(FileEncryptTextBox.Text);
            using (FileStream fStream = File.Open(FileEncryptTextBox.Text + ".encrypted", FileMode.OpenOrCreate))
            {
                using (BinaryWriter bWriter = new BinaryWriter(fStream))
                {
                    bWriter.Write(salt.Length);
                    bWriter.Write(salt);
                    bWriter.Write(iv.Length);
                    bWriter.Write(iv);

                    using (CryptoStream cStream = new CryptoStream(fStream, cryptoTransformer, CryptoStreamMode.Write))
                    {

                        using (BinaryWriter sWriter = new BinaryWriter(cStream))
                        {
                            sWriter.Write(data);
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
            using (FileStream fStream = File.OpenRead(FileDecryptTextBox.Text))
            {
                byte[] salt;
                int saltLength;
                byte[] iv;
                int ivLength;
                using (BinaryReader reader = new BinaryReader(fStream))
                {
                    saltLength = reader.ReadInt32();
                    salt = reader.ReadBytes(saltLength);
                    ivLength = reader.ReadInt32();
                    iv = reader.ReadBytes(ivLength);


                    AesCng algorithm = new AesCng();
                    Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(PasswordDecryptTextBox.Text, salt);
                    byte[] key = keyGenerator.GetBytes(algorithm.KeySize / 8);

                    ICryptoTransform cryptoTransformer = algorithm.CreateDecryptor(key, iv);

                    using (CryptoStream cStream = new CryptoStream(fStream, cryptoTransformer, CryptoStreamMode.Read))
                    {
                        using (BinaryReader sReader = new BinaryReader(cStream))
                        {
                            byte[] data = sReader.ReadBytes((int)fStream.Length - 4 - saltLength - 4 - ivLength);
                            using (FileStream outStream = File.Open(FileDecryptTextBox.Text.Replace("encrypted", "decrypted"), FileMode.OpenOrCreate))
                            {
                                outStream.Write(data, 0, data.Length);
                            }
                        }
                    }
                }
            }
            MessageBox.Show("Decryption completed.");
        }
    }
}
