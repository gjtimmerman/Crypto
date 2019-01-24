package exercise4;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.security.InvalidKeyException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.Signature;
import java.security.SignatureException;
import java.security.UnrecoverableKeyException;
import java.security.cert.Certificate;
import java.security.cert.CertificateException;

public class CertificateSigning {
	public static KeyStore OpenKeyStore(String KeyStoreFileName, char [] KeyStorePassword) throws KeyStoreException, NoSuchAlgorithmException, CertificateException, IOException
	{
		KeyStore myKeyStore = KeyStore.getInstance(KeyStore.getDefaultType());
		File f = new File(KeyStoreFileName);
		FileInputStream fis;
		if (f.exists())
		{
			 fis = new FileInputStream(f);
		}
		else
		{
			System.out.println("Keystore not found");
			return null;
		}
		myKeyStore.load(fis, KeyStorePassword);
		if (fis != null)
			fis.close();
		return myKeyStore;
	}

	public static void main(String[] args) throws KeyStoreException, NoSuchAlgorithmException, CertificateException, IOException, UnrecoverableKeyException, InvalidKeyException, SignatureException {
		String algName = "SHA256WithRSA";
		KeyStore myKeyStore;
		if (args.length < 6)
		{
			System.out.println("Usage: -s/-v FileName KeyStore StorePass CertificateAlias Keypass");
			return;
		}
		else if (args[0].equals("-s"))
		{
			PrivateKey myKey;
			myKeyStore = OpenKeyStore(args[2],args[3].toCharArray());
			if (myKeyStore == null)
			{
				System.out.println("KeyStore not found!");
				return;
			}
			if (myKeyStore.isKeyEntry(args[4]))
			{
				myKey = (PrivateKey)myKeyStore.getKey(args[4], args[5].toCharArray());
				Signature mySignature = Signature.getInstance(algName);
				mySignature.initSign(myKey);
				FileInputStream fisSource = new FileInputStream(args[1]);
				byte [] buffer = new byte [4096];
				int numRead = fisSource.read(buffer);
				while (numRead != -1)
				{
					mySignature.update(buffer);
					numRead = fisSource.read(buffer);
				}
				byte [] mySign = mySignature.sign();
				fisSource.close();
				FileOutputStream fosSignature = new FileOutputStream(args[1]+".signature");
				fosSignature.write(mySign);
				fosSignature.close();
			}
			else
			{
				System.out.println("No Key found");
			}
		}
		else if (args[0].equals("-v"))
		{
			myKeyStore = OpenKeyStore(args[2],args[3].toCharArray());
			if (myKeyStore == null)
			{
				System.out.println("KeyStore not found!");
				return;
			}
			if (myKeyStore.isKeyEntry(args[4]))
			{
				Certificate myCert = myKeyStore.getCertificate(args[4]);
				System.out.println(myCert.toString());
				Signature mySignature = Signature.getInstance(algName);
				mySignature.initVerify(myCert);
				File fileSign = new File(args[1]+".signature");
				FileInputStream fisSignature = new FileInputStream(fileSign);
				byte [] signature = new byte[(int)fileSign.length()];
				fisSignature.read(signature);
				fisSignature.close();
				FileInputStream fisSource = new FileInputStream(args[1]);
				byte [] buffer = new byte[4096];
				int numRead = fisSource.read(buffer);
				while (numRead != -1)
				{
					mySignature.update(buffer);
					numRead = fisSource.read(buffer);
				}
				fisSource.close();
				if (mySignature.verify(signature))
				{
					System.out.println("Signature valid!");
				}
				else
				{
					System.out.println("Signature not valid!");
				}
			}
			else
			{
				System.out.println("Certificate not found!");
			}
		}
		else
		{
			System.out.println("Usage: -s/-v FileName KeyStore StorePass CertificateAlias Keypass");
			return;
		}
		
	}

}
