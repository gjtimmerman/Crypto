using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;


namespace ConsoleAppNetCoreEncr
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Too little arguments. Usage: -e/-d Filename Password");
                return;
            }
            if (args[0] == "-e")
            {

                Aes algorithm = Aes.Create();
                algorithm.Padding = PaddingMode.PKCS7;
                RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                byte[] salt = new byte[32];
                randomNumberGenerator.GetBytes(salt);
                Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(args[2]), salt, 1000,HashAlgorithmName.SHA256);
                //key size is in bits!
                algorithm.KeySize = 128;
                byte[] key = keyGenerator.GetBytes(algorithm.KeySize / 8);
                byte[] iv = algorithm.IV;
                algorithm.Key = key;

                ICryptoTransform cryptoTransformer = algorithm.CreateEncryptor();
                byte[] data = File.ReadAllBytes(args[1]);
                using (FileStream fStream = File.Open(args[1] + ".encrypted", FileMode.OpenOrCreate))
                {
                    using (BinaryWriter bWriter = new BinaryWriter(fStream))
                    {
                        bWriter.Write(salt.Length);
                        bWriter.Write(salt);
                        bWriter.Write(iv.Length);
                        bWriter.Write(iv);
                        bWriter.Flush();

                        using (CryptoStream cStream = new CryptoStream(fStream, cryptoTransformer, CryptoStreamMode.Write))
                        {

                            using (BinaryWriter sWriter = new BinaryWriter(cStream))
                            {
                                sWriter.Write(data);
                            }
                        }
                    }
                }
            }
            else if (args[0] == "-d")
            {
                using (FileStream fStream = File.OpenRead(args[1]))
                {
                    using (BinaryReader reader = new BinaryReader(fStream))
                    {
                        int saltLength = reader.ReadInt32();
                        byte[] salt = reader.ReadBytes(saltLength);
                        int ivLength = reader.ReadInt32();
                        byte[] iv = reader.ReadBytes(ivLength);

                        Aes algorithm = Aes.Create();
                        algorithm.KeySize = 128;
                        algorithm.Padding = PaddingMode.PKCS7;
                        byte[] passwordBytes = Encoding.ASCII.GetBytes(args[2]);
                        Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(args[2]), salt, 1000, HashAlgorithmName.SHA256);
                        byte[] key = keyGenerator.GetBytes(16);

                        ICryptoTransform cryptoTransformer = algorithm.CreateDecryptor(key, iv);

                        using (CryptoStream cStream = new CryptoStream(fStream, cryptoTransformer, CryptoStreamMode.Read))
                        {

                            using (BinaryReader sReader = new BinaryReader(cStream))
                            {

                                byte[] data = sReader.ReadBytes((int)fStream.Length - 4 - saltLength - 4 - ivLength);

                                using (FileStream outStream = File.Open(args[1].Replace("encrypted", "decrypted"), FileMode.OpenOrCreate))
                                {

                                    outStream.Write(data, 0, data.Length);
                                    outStream.Flush();
                                    Console.WriteLine("Decryption completed.");
                                }
               			 	}
						}
					}
				}
            }
            else
            {
                Console.WriteLine("Usage: -e/-d Filename Password");
                return;
            }
        }
    }
}