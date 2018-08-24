using System;
using System.IO;
using System.Security.Cryptography;

namespace AsymmetricKeyExchangeNetCore
{
    class MyRSAKeyFormatter
    {
        public MyRSAKeyFormatter(RSA prov)
        {
            provider = prov;
        }
        public byte [] CreateKeyExchange(byte []symKey)
        {
            return provider.Encrypt(symKey, RSAEncryptionPadding.OaepSHA1);
        }
        private RSA provider;
    }
    class MyRSAKeyDeformatter
    {
        public MyRSAKeyDeformatter(RSA prov)
        {
            provider = prov;
        }
        public byte[] DecryptKeyExchange(byte[] hash)
        {
            return provider.Decrypt(hash, RSAEncryptionPadding.OaepSHA1);
        }
        private RSA provider;
    }
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
                CspParameters myCspParms = new CspParameters();
                myCspParms.KeyContainerName = args[2];
                RSACryptoServiceProvider myRSAProv = new RSACryptoServiceProvider(myCspParms);
                MyRSAKeyFormatter myKeyFormatter = new MyRSAKeyFormatter(myRSAProv);
                Aes mySymmProv = Aes.Create();
                byte[] encrKey = myKeyFormatter.CreateKeyExchange(mySymmProv.Key);
                byte[] data = File.ReadAllBytes(args[1]);
                FileStream fsout = new FileStream(args[1] + ".encrypted", FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fsout);
                bw.Write(encrKey.Length);
                bw.Write(encrKey, 0, encrKey.Length);
                bw.Write(mySymmProv.IV.Length);
                bw.Write(mySymmProv.IV, 0, mySymmProv.IV.Length);
                bw.Flush();
                CryptoStream cs = new CryptoStream(fsout, mySymmProv.CreateEncryptor(), CryptoStreamMode.Write);
                BinaryWriter sw = new BinaryWriter(cs);
                sw.Write(data);
                sw.Flush();
                sw.Dispose();
                cs.Dispose();
                bw.Dispose();
                fsout.Dispose();

            }
            else if (args[0] == "-d")
            {
                CspParameters myCspParms = new CspParameters();
                myCspParms.KeyContainerName = args[2];
                RSACryptoServiceProvider myRSAProv = new RSACryptoServiceProvider(myCspParms);
                MyRSAKeyDeformatter myKeyDeformatter = new MyRSAKeyDeformatter(myRSAProv);
                Aes mySymmProv = Aes.Create();
                FileStream fs = new FileStream(args[1], FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                int keyLength = br.ReadInt32();
                byte[] encrKey = new byte[keyLength];
                br.Read(encrKey, 0, keyLength);
                int ivLength = br.ReadInt32();
                byte[] IV = new byte[ivLength];
                br.Read(IV, 0, ivLength);
                byte[] Key = myKeyDeformatter.DecryptKeyExchange(encrKey);
                mySymmProv.Key = Key;
                mySymmProv.IV = IV;
                CryptoStream cs = new CryptoStream(fs, mySymmProv.CreateDecryptor(), CryptoStreamMode.Read);
                int dataLen = (int)fs.Length - 4 - keyLength - 4 - ivLength;
                byte[] data = new byte[dataLen];
                cs.Read(data, 0, dataLen);
                while (data[dataLen - 1] == 0)      // To Remove padding
                    dataLen--;
                FileStream outStream = File.Open(args[1].Replace("encrypted", "decrypted"), FileMode.OpenOrCreate);

                outStream.Write(data, 0, dataLen);
                cs.Dispose();
                fs.Dispose();
                outStream.Flush();
                outStream.Dispose();

            }
            else
            {
                Console.WriteLine("Usage: -e/-d Filename KeyContainer");
                return;
            }

        }
    }
}