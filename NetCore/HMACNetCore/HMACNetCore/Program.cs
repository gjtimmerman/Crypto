using System;
using System.IO;
using System.Security.Cryptography;

namespace HMACNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: -s/-v Filename KeyFile");
                return;
            }
            if (args[0] == "-s")
            {
                HMACSHA256 myHashAlg = new HMACSHA256();
                if (File.Exists(args[2]))
                {
                    FileStream fskeyfile = new FileStream(args[2], FileMode.Open);
                    byte[] key = new byte[fskeyfile.Length];
                    fskeyfile.Read(key, 0, key.Length);
                    fskeyfile.Dispose();
                    myHashAlg.Key = key;
                }
                else
                {
                    FileStream fskeyfile = new FileStream(args[2], FileMode.Create);
                    fskeyfile.Write(myHashAlg.Key, 0, myHashAlg.Key.Length);
                    fskeyfile.Flush();
                    fskeyfile.Dispose();
                }
                FileStream inFile = new FileStream(args[1], FileMode.Open);
                byte[] hValue = myHashAlg.ComputeHash(inFile);
                inFile.Dispose();
                FileStream hFile = new FileStream(args[1] + ".signature", FileMode.Create);
                hFile.Write(hValue, 0, hValue.Length);
                hFile.Flush();
                hFile.Dispose();
            }
            else if (args[0] == "-v")
            {
                FileStream fskeyfile = new FileStream(args[2], FileMode.Open);
                byte[] key = new byte[fskeyfile.Length];
                fskeyfile.Read(key, 0, key.Length);
                fskeyfile.Dispose();
                HMACSHA256 myHashAlg = new HMACSHA256();
                myHashAlg.Key = key;
                FileStream inFile = new FileStream(args[1], FileMode.Open);
                byte[] hValue = myHashAlg.ComputeHash(inFile);
                inFile.Dispose();
                FileStream hFile = new FileStream(args[1] + ".signature", FileMode.Open);
                byte[] orghValue = new byte[hFile.Length];
                hFile.Read(orghValue, 0, orghValue.Length);
                hFile.Dispose();
                bool identical = true;
                for (int i = 0; i < hValue.Length; i++)
                    if (hValue[i] != orghValue[i])
                    {
                        identical = false;
                        break;
                    }
                if (identical)
                    Console.WriteLine("Files are identical!");
                else
                    Console.WriteLine("Files are not identical!");

            }
            else
            {
                Console.WriteLine("Usage: -s/-v Filename KeyFile");
                return;
            }

        }
    }
}