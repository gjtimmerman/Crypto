package exercise2;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.Key;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.PublicKey;
import java.security.UnrecoverableKeyException;
import java.security.cert.Certificate;
import java.security.cert.CertificateException;

import javax.crypto.Cipher;
import javax.crypto.CipherInputStream;
import javax.crypto.CipherOutputStream;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.KeyGenerator;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;

public class AsymmetricExchange {
	
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

	public static void main(String[] args) throws KeyStoreException, NoSuchAlgorithmException, CertificateException, IOException, UnrecoverableKeyException, NoSuchPaddingException, InvalidKeyException, IllegalBlockSizeException, InvalidAlgorithmParameterException {
		if (args.length < 6)
		{
			System.out.println("Usage: -e/-d FileName KeyStore StorePass KeyAlias Keypass");
			return;
		}
		final String algWrapName = "RSA" ;
		final String algEncrName = "AES";
		final String algModePaddingName = algEncrName + "/CBC/PKCS5Padding";

		if (args[0].equals("-e")){
			PublicKey myKey = null;
			KeyStore myKeyStore = OpenKeyStore(args[2],args[3].toCharArray());
			if (myKeyStore == null)
				return;

			if (myKeyStore.isKeyEntry(args[4]))
			{
				Certificate myCert = myKeyStore.getCertificate(args[4]);
				myKey = myCert.getPublicKey();
			}
			else
			{
				System.out.println("Key not found!");
				return;
			}
			KeyGenerator myKeyGenerator = KeyGenerator.getInstance("AES");
			myKeyGenerator.init(128);
			SecretKey mySecretKey = myKeyGenerator.generateKey();
			Cipher myWrapCipher = Cipher.getInstance(algWrapName);
			myWrapCipher.init(Cipher.WRAP_MODE, myKey);
			byte [] wrappedKey = myWrapCipher.wrap(mySecretKey);
			FileOutputStream fosKey = new FileOutputStream("SecretKey");
			fosKey.write(wrappedKey);
			fosKey.close();
			FileInputStream fisSource = new FileInputStream(args[1]);
			Cipher myEncrCipher = Cipher.getInstance(algModePaddingName);
			myEncrCipher.init(Cipher.ENCRYPT_MODE, mySecretKey);
			byte [] myIV = myEncrCipher.getIV();
			FileOutputStream fosDestFile = new FileOutputStream(args[1] + ".encrypted");
			DataOutputStream dosDestFile = new DataOutputStream(fosDestFile);
			dosDestFile.writeInt(myIV.length);
			dosDestFile.write(myIV);
			dosDestFile.flush();
			CipherOutputStream cosDestFile = new CipherOutputStream(fosDestFile,myEncrCipher);
			byte [] buffer = new byte[4096];
			int numBytes = fisSource.read(buffer);
			while (numBytes != -1)
			{
				cosDestFile.write(buffer);
				numBytes = fisSource.read(buffer);
			}
			cosDestFile.close();
			fisSource.close();
			System.out.println("File Encrypted");
		}
		else if (args[0].equals("-d"))
		{
			Key myKey = null;
			
			KeyStore myKeyStore = OpenKeyStore(args[2],args[3].toCharArray());
			if (myKeyStore == null)
				return;
			if (myKeyStore.isKeyEntry(args[4]))
			{
				myKey = myKeyStore.getKey(args[4], args[5].toCharArray());				
			}
			else
			{
				System.out.println("Key not found!");
				return;
			}
			File wrappedKeyFile = new File("SecretKey");
			if (!wrappedKeyFile.exists())
			{
				System.out.println("SecretKey file does not exist!");
				return;
			}
			byte [] wrappedKey = new byte[(int)wrappedKeyFile.length()];
			FileInputStream fisKey = new FileInputStream(wrappedKeyFile);
			fisKey.read(wrappedKey);
			fisKey.close();
			Cipher myWrapCipher = Cipher.getInstance(algWrapName);
			myWrapCipher.init(Cipher.UNWRAP_MODE, myKey);
			SecretKey mySecretKey = (SecretKey)myWrapCipher.unwrap(wrappedKey, algEncrName, Cipher.SECRET_KEY);
			FileInputStream fisSourceFile = new FileInputStream(args[1]+".encrypted");
			DataInputStream disSourceFile = new DataInputStream(fisSourceFile);
			int myIVLen = disSourceFile.readInt();
			byte [] myIV = new byte[myIVLen];
			disSourceFile.read(myIV);
			IvParameterSpec myIVParmSpec = new IvParameterSpec(myIV);
			Cipher myEncrCipher = Cipher.getInstance(algModePaddingName);
			myEncrCipher.init(Cipher.DECRYPT_MODE, mySecretKey, myIVParmSpec);
			CipherInputStream cisSourceFile = new CipherInputStream(fisSourceFile,myEncrCipher);
			FileOutputStream fosDestFile = new FileOutputStream(args[1]+".decrypted");
			byte []buffer = new byte[4096];
			int numRead = cisSourceFile.read(buffer);
			while (numRead != -1)
			{
				fosDestFile.write(buffer);
				numRead = cisSourceFile.read(buffer);
			}
			fosDestFile.close();
			cisSourceFile.close();
			disSourceFile.close();
			System.out.println("File decrypted!");
		}
		else
		{
			System.out.println("Usage: -e/-d FileName KeyStore StorePass KeyAlias Keypass");
			return;			
		}
		
	}

}
