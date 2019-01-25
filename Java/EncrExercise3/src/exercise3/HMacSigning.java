package exercise3;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.security.InvalidKeyException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.UnrecoverableKeyException;
import java.security.cert.CertificateException;
import java.util.Arrays;

import javax.crypto.KeyGenerator;
import javax.crypto.Mac;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SecretKey;

public class HMacSigning {

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

	public static void main(String[] args) throws KeyStoreException, NoSuchAlgorithmException, CertificateException, IOException, UnrecoverableKeyException, NoSuchPaddingException, InvalidKeyException {
		String algName = "HMacSHA256";
		if (args.length < 6)
		{
			System.out.println("Usage: -s/-v FileName KeyStore StorePass KeyAlias Keypass");
			return;
		}
		if (args[0].equals("-s"))
		{
			SecretKey myKey;
			
			KeyStore myKeyStore = OpenKeyStore(args[2],args[3].toCharArray());
			if (myKeyStore == null)
			{
				myKeyStore = KeyStore.getInstance(KeyStore.getDefaultType());
				myKeyStore.load(null, null);
			}
			if (myKeyStore.isKeyEntry(args[4]))
			{
				myKey = (SecretKey)myKeyStore.getKey(args[4], args[5].toCharArray());
				System.out.println("Key found!");
			}
			else
			{
				System.out.println("Key not found!");
				KeyGenerator myKeyGenerator = KeyGenerator.getInstance(algName);
				myKey = myKeyGenerator.generateKey();
				KeyStore.SecretKeyEntry mySecretKeyEntry = new KeyStore.SecretKeyEntry(myKey);
				myKeyStore.setEntry(args[4], mySecretKeyEntry,new KeyStore.PasswordProtection(args[5].toCharArray()));
				FileOutputStream fosKeyStore = new FileOutputStream(args[2]);
				myKeyStore.store(fosKeyStore, args[3].toCharArray());
				fosKeyStore.close();
			}
			Mac myMac = Mac.getInstance(algName);
			myMac.init(myKey);
			FileInputStream fisSource = new FileInputStream(args[1]);
			byte [] buffer = new byte[4096];
			int numRead = fisSource.read(buffer);
			while (numRead != -1)
			{
				myMac.update(buffer);
				numRead = fisSource.read(buffer);
			}
			byte [] signature = myMac.doFinal();
			fisSource.close();
			FileOutputStream fosSignature = new FileOutputStream(args[1]+".signature");
			fosSignature.write(signature);
			fosSignature.close();
			
		}
		else if (args[0].equals("-v"))
		{
			SecretKey myKey = null;
			KeyStore myKeyStore = OpenKeyStore(args[2],args[3].toCharArray());
			if (myKeyStore == null)
			{
				System.out.println("KeyStore not found!");
				return;
			}
			if (myKeyStore.isKeyEntry(args[4]))
			{
				myKey = (SecretKey)myKeyStore.getKey(args[4], args[5].toCharArray());
			}
			else
			{
				System.out.println("Key not found!");
				return;
			}
			Mac myMac = Mac.getInstance(algName);
			myMac.init(myKey);
			FileInputStream fisSource = new FileInputStream(args[1]);
			byte [] buffer = new byte[4096];
			int numRead = fisSource.read(buffer);
			while (numRead != -1)
			{
				myMac.update(buffer);
				numRead = fisSource.read(buffer);
			}
			byte [] signature = myMac.doFinal();
			fisSource.close();
			File fileSign = new File(args[1]+".signature");
			FileInputStream fisSignature = new FileInputStream(fileSign);
			byte [] origSignature = new byte[(int)fileSign.length()];
			fisSignature.read(origSignature);
			fisSignature.close();
			if (Arrays.equals(signature, origSignature))
			{
				System.out.println("Signature valid!");
			}
			else
			{
				System.out.println("Signature not valid!");
			}
			
				
		}
	}

}
