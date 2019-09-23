package ecctest;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.NoSuchProviderException;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.Signature;
import java.security.SignatureException;
import java.security.UnrecoverableKeyException;
import java.security.cert.CertificateException;
import java.security.spec.ECGenParameterSpec;

public class MainClass {

	public static KeyStore OpenKeyStore(String KeyStoreFileName, char [] KeyStorePassword) throws KeyStoreException, NoSuchAlgorithmException, CertificateException, IOException
	{
		KeyStore myKeyStore = KeyStore.getInstance("PKCS12");
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

	
	public static void main(String[] args) throws NoSuchAlgorithmException, NoSuchProviderException, InvalidAlgorithmParameterException, InvalidKeyException, SignatureException, KeyStoreException, CertificateException, IOException, UnrecoverableKeyException {
		KeyPairGenerator kpg = KeyPairGenerator.getInstance("EC");
		ECGenParameterSpec egp = new ECGenParameterSpec("secp256k1");
		kpg.initialize(egp);
		KeyPair kp = kpg.genKeyPair();
		PrivateKey privKey = kp.getPrivate();
		PublicKey pubKey = kp.getPublic();
//		KeyStore ks = OpenKeyStore("C:/Users/Instructor/.keystore","Password".toCharArray());
//		PrivateKey privKey = (PrivateKey)ks.getKey("myeckey", "Password".toCharArray());
		Signature mySignature = Signature.getInstance("SHA256withECDSA");
		byte [] source = "Dit is een tekst".getBytes("UTF-8");
		mySignature.initSign(privKey);
		mySignature.update(source);
		byte [] sign = mySignature.sign();
		
		Signature mySignature2 = Signature.getInstance("SHA256withECDSA");
//		PublicKey pubKey = ks.getCertificate("myeckey").getPublicKey();
		mySignature2.initVerify(pubKey);
		mySignature2.update(source);
		if (mySignature2.verify(sign))
			System.out.println("Signature correct");
		else
			System.out.println("Signature NOT correct");
			
	}

}
