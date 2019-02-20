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
                    using (FileStream fskeyfile = new FileStream(args[2], FileMode.Open))
                    {
                        byte[] key = new byte[fskeyfile.Length];
                        fskeyfile.Read(key, 0, key.Length);
                        myHashAlg.Key = key;
                   }
                }
                else
                {
                    using (FileStream fskeyfile = new FileStream(args[2], FileMode.Create))
                    {
                        fskeyfile.Write(myHashAlg.Key, 0, myHashAlg.Key.Length);
                    }
                }
                using (FileStream inFile = new FileStream(args[1], FileMode.Open))
                {
                    byte[] hValue = myHashAlg.ComputeHash(inFile);
                    using (FileStream hFile = new FileStream(args[1] + ".signature", FileMode.Create))
                    {
                        hFile.Write(hValue, 0, hValue.Length);
                    }
                }
            }
            else if (args[0] == "-v")
            {
                HMACSHA256 myHashAlg = new HMACSHA256();
                using (FileStream fskeyfile = new FileStream(args[2], FileMode.Open))
                {
                    byte[] key = new byte[fskeyfile.Length];
                    fskeyfile.Read(key, 0, key.Length);
                    myHashAlg.Key = key;
                }
                byte[] hValue;
                using (FileStream inFile = new FileStream(args[1], FileMode.Open))
                {
                    hValue = myHashAlg.ComputeHash(inFile);
                }
                byte[] orghValue;
                using (FileStream hFile = new FileStream(args[1] + ".signature", FileMode.Open))
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