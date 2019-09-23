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
                CspParameters myCspParms = new CspParameters { KeyContainerName = args[2] };

                RSACryptoServiceProvider myRSAProv = new RSACryptoServiceProvider(myCspParms);
                RSAOAEPKeyExchangeFormatter myKeyFormatter = new RSAOAEPKeyExchangeFormatter(myRSAProv);

                Aes mySymmProv = Aes.Create();
                byte[] encrKey = myKeyFormatter.CreateKeyExchange(mySymmProv.Key);
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
                CspParameters myCspParms = new CspParameters { KeyContainerName = args[2] };

                RSACryptoServiceProvider myRSAProv = new RSACryptoServiceProvider(myCspParms);
                RSAOAEPKeyExchangeDeformatter myKeyDeformatter = new RSAOAEPKeyExchangeDeformatter(myRSAProv);

                Aes mySymmProv = Aes.Create();
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
                        byte[] Key = myKeyDeformatter.DecryptKeyExchange(encrKey);
                        mySymmProv.Key = Key;
                        mySymmProv.IV = IV;
                        using (CryptoStream cs = new CryptoStream(fs, mySymmProv.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            int dataLen = (int)fs.Length - 4 - keyLength - 4 - ivLength;
                            byte[] data = new byte[dataLen];
                            int len = cs.Read(data, 0, dataLen);
                            using (FileStream outStream = File.Open(args[1].Replace("encrypted", "decrypted"), FileMode.OpenOrCreate))
                            {
                                outStream.Write(data, 0, len);
                                outStream.Flush();
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