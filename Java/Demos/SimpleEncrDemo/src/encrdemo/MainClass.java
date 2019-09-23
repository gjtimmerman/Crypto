package encrdemo;

import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.spec.InvalidKeySpecException;

import javax.crypto.BadPaddingException;
import javax.crypto.Cipher;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.KeyGenerator;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SecretKey;

public class MainClass {
	static void printArray(byte [] bytes)
	{
		for (byte b : bytes)
			System.out.printf("%02x",b);
		System.out.println();
	}

	public static void main(String[] args) throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeySpecException, InvalidKeyException, IllegalBlockSizeException, BadPaddingException {
		String algName = "AES";
		String padding = "/ECB/NoPadding";
		Cipher ciph = Cipher.getInstance(algName+padding);
		KeyGenerator keyGen = KeyGenerator.getInstance(algName);
		keyGen.init(128);
		SecretKey secKey = keyGen.generateKey();
		ciph.init(Cipher.ENCRYPT_MODE, secKey);
		byte [] clearText = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
		byte [] cipherText = ciph.update(clearText);
		printArray(cipherText);
		cipherText = ciph.update(clearText);
		printArray(cipherText);
		cipherText = ciph.update(clearText);
		printArray(cipherText);
		cipherText = ciph.doFinal();
		printArray(cipherText);
	}

}
