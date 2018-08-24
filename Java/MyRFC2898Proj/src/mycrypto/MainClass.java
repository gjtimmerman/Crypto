package mycrypto;

import java.io.UnsupportedEncodingException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.spec.InvalidKeySpecException;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

public class MainClass {

	public static void main(String[] args) throws UnsupportedEncodingException, NoSuchAlgorithmException, InvalidKeyException, InvalidKeySpecException, NoSuchPaddingException, IllegalBlockSizeException, BadPaddingException, InvalidAlgorithmParameterException {
		byte [] salt = new byte [12];
		for (int i = 0; i < 12; i++)
			salt[i] = (byte)i;
		Rfc2898DeriveBytes myDerBytes = new Rfc2898DeriveBytes("MyPassword",salt);
		byte [] key = myDerBytes.getBytes(16);
		for(byte b : key)
			System.out.print((b & 0x00FF) + " ");
		System.out.println();
		
		
	}

}
