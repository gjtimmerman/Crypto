using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SignWithCertificateNetCore
{
    class Program
    {
        static X509Certificate2Collection MySelectCertificate(StoreName sn, StoreLocation sl, string subjectName)
        {
            X509Store myStore = new X509Store(sn, sl);
            myStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection myColl = myStore.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);
            if (myColl.Count == 0)
            {
                Console.WriteLine("No certificate available with SubjectName: {0}", subjectName);
                Console.WriteLine("Available Certificates are:");
                foreach (X509Certificate2 cert in myStore.Certificates)
                {
                    Console.WriteLine(cert.Subject);
                }
                myStore.Dispose();
            }
            return myColl;
        }
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: -s/-v Filename CertificateName");
                return;
            }
            if (args[0] == "-s")
            {
                X509Certificate2Collection myColl = MySelectCertificate(StoreName.My, StoreLocation.CurrentUser,args[2]);
                if (myColl.Count == 0)
                    return;
                X509Certificate2 myCertificate = myColl[0];
                RSACryptoServiceProvider myProv;
                if (myCertificate.GetRSAPrivateKey() is RSACryptoServiceProvider)
                    myProv = (RSACryptoServiceProvider)myCertificate.GetRSAPrivateKey();
                else
                {
                    myProv = new RSACryptoServiceProvider();
                    myProv.ImportParameters(myCertificate.GetRSAPrivateKey().ExportParameters(true));
                }
                FileStream fsin = new FileStream(args[1], FileMode.Open);
                byte[] hash = myProv.SignData(fsin, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                fsin.Dispose();
                FileStream fshash = new FileStream(args[1] + ".signature", FileMode.Create);
                fshash.Write(hash, 0, hash.Length);
                fshash.Flush();
                fshash.Dispose();

            }
            else if (args[0] == "-v")
            {
                X509Certificate2Collection myColl = MySelectCertificate(StoreName.My, StoreLocation.CurrentUser, args[2]);
                if (myColl.Count == 0)
                    return;
                X509Certificate2 myCertificate = myColl[0];
                RSACryptoServiceProvider myProv;
                if (myCertificate.GetRSAPublicKey() is RSACryptoServiceProvider)
                    myProv = (RSACryptoServiceProvider)myCertificate.GetRSAPublicKey();
                else
                {
                    myProv = new RSACryptoServiceProvider();
                    myProv.ImportParameters(myCertificate.GetRSAPublicKey().ExportParameters(false));
                }
                FileStream fssign = new FileStream(args[1] + ".signature", FileMode.Open);
                byte[] sign = new byte[fssign.Length];
                fssign.Read(sign, 0, sign.Length);
                fssign.Dispose();
                FileStream fsin = new FileStream(args[1], FileMode.Open);
                if (myProv.VerifyData(fsin, sign, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1))
                    Console.WriteLine("Signature is valid");
                else
                    Console.WriteLine("Signature is not valid");
                fsin.Dispose();
            }
            else
            {
                Console.WriteLine("Usage: -s/-v Filename CertificateName");
                return;
            }
        }
    }
}