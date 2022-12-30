using System;
using System.IO;
using System.Security.Cryptography;

namespace AsymmetricKeyExchangeNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: -e/-d Filename KeyContainer");
                return;
            }
            if (args[0] == "-e")
            {
                CngKey myKey;
                if (CngKey.Exists(args[2]))
                {
                    myKey = CngKey.Open(args[2]);
                }
                else
                {
                    myKey = CngKey.Create(CngAlgorithm.Rsa, args[2]);                   
                }

                RSACng myRSAProv= new RSACng(myKey);
                Aes mySymmProv = AesCng.Create();
                mySymmProv.KeySize = 128;
                mySymmProv.Padding = PaddingMode.PKCS7;
                byte []encrKey = myRSAProv.Encrypt(mySymmProv.Key, RSAEncryptionPadding.OaepSHA256);
                byte[] data = File.ReadAllBytes(args[1]);
                using (FileStream fsout = new FileStream(args[1] + ".encrypted", FileMode.Create))
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
                                sw.Flush();
                            }
                        }
                    }
                }

            }
            else if (args[0] == "-d")
            {
                CngKey myKey = CngKey.Open(args[2]);
                RSACng myRSAProv = new RSACng(myKey);

                Aes mySymmProv = AesCng.Create();
                using (FileStream fs = new FileStream(args[1], FileMode.Open))
                { 
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        int keyLength = br.ReadInt32();
                        byte[] encrKey = new byte[keyLength];
                        br.Read(encrKey, 0, keyLength);
                        int ivLength = br.ReadInt32();
                        byte[] IV = new byte[ivLength];
                        br.Read(IV, 0, ivLength);
                        byte[] Key = myRSAProv.Decrypt(encrKey,RSAEncryptionPadding.OaepSHA256);
                        mySymmProv.KeySize = 128;
                        mySymmProv.Key = Key;
                        mySymmProv.IV = IV;
                        mySymmProv.Padding = PaddingMode.PKCS7;
                        using (CryptoStream cs = new CryptoStream(fs, mySymmProv.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (BinaryReader bs = new BinaryReader(cs))
                            {
                                int dataLen = (int)(fs.Length - 4 - keyLength - 4 - ivLength);
                                byte[] data = bs.ReadBytes(dataLen);
                                using (FileStream outStream = File.Open(args[1].Replace("encrypted", "decrypted"), FileMode.OpenOrCreate))
                                {
                                    outStream.Write(data, 0, data.Length);
                                    outStream.Flush();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Usage: -e/-d Filename KeyContainer");
                return;
            }

        }
    }
}