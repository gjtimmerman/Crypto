package exercise1;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.security.spec.InvalidKeySpecException;

import javax.crypto.Cipher;
import javax.crypto.CipherInputStream;
import javax.crypto.CipherOutputStream;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SecretKey;
import javax.crypto.SecretKeyFactory;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.PBEKeySpec;
import javax.crypto.spec.PBEParameterSpec;

public class SymmetricPasswEncr {

	public static void main(String[] args) throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeySpecException, InvalidKeyException, InvalidAlgorithmParameterException, IOException {
		if (args.length < 3)
		{
			System.out.println("Usage: -e/-d FileName Password");
			return;
		}
		else if (args[0].equals("-e"))
		{
			String algName = "PBEWithHmacSHA1AndAES_128" ;
			String algModePaddingName = algName + "/CBC/PKCS5Padding";
			Cipher myCiph = Cipher.getInstance(algModePaddingName);
			int saltSize = myCiph.getBlockSize();
			byte [] salt = new byte [saltSize];
			SecureRandom rn = new SecureRandom();
			rn.nextBytes(salt);
			
			PBEParameterSpec pbeParmSpec = new PBEParameterSpec(salt,1000);
			PBEKeySpec pbeKeySpec = new PBEKeySpec(args[2].toCharArray());
			SecretKeyFactory secrKeyFactory = SecretKeyFactory.getInstance(algName);
			SecretKey secretKey = secrKeyFactory.generateSecret(pbeKeySpec);
			myCiph.init(Cipher.ENCRYPT_MODE, secretKey, pbeParmSpec);
			byte [] myIV = myCiph.getIV();
			FileInputStream fis = new FileInputStream(args[1]);
			FileOutputStream fos = new FileOutputStream(args[1]+".encrypted");
			DataOutputStream dos = new DataOutputStream(fos);
			dos.writeInt(saltSize);
			dos.write(salt);
			dos.writeInt(myIV.length);
			dos.write(myIV);
			dos.flush();
			CipherOutputStream cos = new CipherOutputStream(fos, myCiph);
			byte [] buffer = new byte[4092];
			int numBytes = fis.read(buffer);
			while (numBytes != -1)
			{
				cos.write(buffer);
				numBytes = fis.read(buffer);
			}
			cos.close();
			fis.close();			
		}
		else if(args[0].equals("-d"))
		{
			String algName = "PBEWithHmacSHA1AndAES_128" ;
			String algModePaddingName = algName + "/CBC/PKCS5Padding";
			FileInputStream fis = new FileInputStream(args[1] + ".encrypted");
			DataInputStream dis = new DataInputStream(fis);
			int saltSize = dis.readInt();
			byte [] salt = new byte[saltSize];
			dis.read(salt);
			int ivSize = dis.readInt();
			byte [] myIV = new byte[ivSize];
			dis.read(myIV);
			Cipher myCiph = Cipher.getInstance(algModePaddingName);
			IvParameterSpec myIVParmSpec = new IvParameterSpec(myIV);
			PBEParameterSpec myParmSpec = new PBEParameterSpec(salt,1000,myIVParmSpec);
			PBEKeySpec myKeySpec = new PBEKeySpec(args[2].toCharArray());
			SecretKeyFactory mySecretKeyFactory = SecretKeyFactory.getInstance(algName);
			SecretKey mySecretKey = mySecretKeyFactory.generateSecret(myKeySpec);
			myCiph.init(Cipher.DECRYPT_MODE, mySecretKey,myParmSpec);
			CipherInputStream cis = new CipherInputStream(fis,myCiph);
			FileOutputStream fos = new FileOutputStream(args[1]+ ".decrypted");
			byte [] buffer = new byte[4096];
			int bytesRead = cis.read(buffer);
			while(bytesRead != -1)
			{
				fos.write(buffer);
				bytesRead = cis.read(buffer);
			}
			fos.close();
			cis.close();		
		}
		else
		{
			System.out.println("Usage: -e/-d FileName Password");
			return;			
		}
		
	}

}
