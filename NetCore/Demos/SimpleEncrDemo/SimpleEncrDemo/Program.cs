using System;
using System.Security.Cryptography;

namespace SimpleEncrDemo
{
    class Program
    {
        static void dumpArray(byte [] bytes)
        {
            foreach (byte b in bytes)
                Console.Write("{0:x2}", b);
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            Aes myAes = Aes.Create();
            byte[] plainText = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
            byte[] outputBuffer = new byte[plainText.Length];
            byte[] finalOutput;
            myAes.Mode = CipherMode.ECB;
            myAes.Padding = PaddingMode.None;

            ICryptoTransform myEncryptor = myAes.CreateEncryptor();

            //myEncryptor.TransformBlock(plainText,0,plainText.Length,outputBuffer,0);
            //dumpArray(outputBuffer);
            //myEncryptor.TransformBlock(plainText, 0, plainText.Length, outputBuffer, 0);
            //dumpArray(outputBuffer);
            //myEncryptor.TransformBlock(plainText, 0, plainText.Length, outputBuffer, 0);
            //dumpArray(outputBuffer);
            finalOutput = myEncryptor.TransformFinalBlock(plainText, 0, plainText.Length);
            dumpArray(finalOutput);
        }
    }
}
