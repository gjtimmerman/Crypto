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
                RSA myProv;
                if (myCertificate.GetRSAPrivateKey() is RSA)
                    myProv = (RSA)myCertificate.GetRSAPrivateKey();
                else
                {
                    myProv = RSA.Create();
                    myProv.ImportParameters(myCertificate.GetRSAPrivateKey().ExportParameters(true));
                }
                byte[] hash;
                using (FileStream fsin = new FileStream(args[1], FileMode.Open))
                {
                    hash = myProv.SignData(fsin, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
                using (FileStream fshash = new FileStream(args[1] + ".signature", FileMode.Create))
                {
                    fshash.Write(hash, 0, hash.Length);
                }

            }
            else if (args[0] == "-v")
            {
                X509Certificate2Collection myColl = MySelectCertificate(StoreName.My, StoreLocation.CurrentUser, args[2]);
                if (myColl.Count == 0)
                    return;
                X509Certificate2 myCertificate = myColl[0];
                RSA myProv;
                if (myCertificate.GetRSAPublicKey() is RSA)
                    myProv = (RSA)myCertificate.GetRSAPublicKey();
                else
                {
                    myProv = RSA.Create();
                    myProv.ImportParameters(myCertificate.GetRSAPublicKey().ExportParameters(false));
                }
                byte[] sign;
                using (FileStream fssign = new FileStream(args[1] + ".signature", FileMode.Open))
                {
                    sign = new byte[fssign.Length];
                    fssign.Read(sign, 0, sign.Length);
                }
                using (FileStream fsin = new FileStream(args[1], FileMode.Open))
                {
                    if (myProv.VerifyData(fsin, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                        Console.WriteLine("Signature is valid");
                    else
                        Console.WriteLine("Signature is not valid");
                }
            }
            else
            {
                Console.WriteLine("Usage: -s/-v Filename CertificateName");
                return;
            }
        }
    }
}