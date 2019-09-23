package mycrypto;

import java.io.UnsupportedEncodingException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.security.InvalidKeyException;
import java.security.Key;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;

import javax.crypto.*;
import javax.crypto.spec.SecretKeySpec;


public class Rfc2898DeriveBytes {
	public Rfc2898DeriveBytes(byte [] password, byte [] salt, int numIterations) throws NoSuchAlgorithmException
	{
		this.password = password;
		this.salt = salt;
		this.iterationCount = numIterations;
		this.macProvider = Mac.getInstance("HmacSHA1");
		
	}
	public Rfc2898DeriveBytes(String password, byte [] salt) throws UnsupportedEncodingException, NoSuchAlgorithmException
	{
		this(password.getBytes("UTF-8"),salt,1000);
	}
	public Rfc2898DeriveBytes(String password, byte [] salt, int numIterations) throws UnsupportedEncodingException, NoSuchAlgorithmException
	{
		this(password.getBytes("UTF-8"),salt,numIterations);
	}
	public Rfc2898DeriveBytes(String password, int saltSize) throws UnsupportedEncodingException, NoSuchAlgorithmException
	{
		this(password.getBytes("UTF-8"),generateSalt(saltSize),1000);
	}
	static private byte[] generateSalt(int saltSize)
	{
		if (saltSize < 8)
			throw new IllegalArgumentException("Salt too Small");
		byte [] tmpSalt = new byte[saltSize];
		SecureRandom sr = new SecureRandom();
		sr.nextBytes(tmpSalt);
		return tmpSalt;
	}
	public byte [] getSalt() {
		return salt;
	}
	public void setSalt(byte [] salt) {
		this.salt = salt;
	}
	public int getIterationCount() {
		return iterationCount;
	}
	public void setIterationCount(int iterationCount) {
		this.iterationCount = iterationCount;
	}
	public byte [] getBytes(int keyLength) throws InvalidKeyException
	{
		byte [] result =  null;
		if (keyLength < 1)
			throw new IllegalArgumentException("keyLength too small");
		int hLen = macProvider.getMacLength();
		int l;
		int r;
		if (keyLength % hLen == 0)
		{
			l = keyLength / hLen;
			r = hLen;
		}
		else
		{
			l = keyLength / hLen + 1;
			r = keyLength - (l - 1) * hLen;
		}
		byte [][] rawResult = new byte[l][];
		for (int i = 0; i < l; i++)
			rawResult[i] = getRawPart(i+1, hLen);
		
		result = copyToResult(hLen, l, r, rawResult);
		return result;
	}
	private static byte[] copyToResult(int hLen, int l, int r, byte[][] rawResult) {
		byte [] result = new byte[(l-1)*hLen+r];
		for (int i = 0; i < l-1; i++)
			copyTo(result, rawResult[i], i*hLen, hLen);
		copyTo(result, rawResult[l-1],(l-1)*hLen,r);
		return result;
	}
	private static void copyTo( byte[] result,byte[] source,int index, int count) {
		for (int j = 0; j < count; j++)
			result[index+j] = source[j];;
	}
	private byte [] getRawPart(int i, int hLen) throws InvalidKeyException
	{
		byte [] result = new byte[hLen];
		ByteBuffer byteBuf = ByteBuffer.allocate(4);
		byteBuf.order(ByteOrder.BIG_ENDIAN);
		byteBuf.putInt(0, i);
		byte [] text = new byte [salt.length+4];
		copyTo(text,salt,0,salt.length);
		byteBuf.get(text, salt.length, 4);
		Key myKey = new SecretKeySpec(password,"AES");
		macProvider.init(myKey);
		for (int k = 0; k < iterationCount; k++)
		{
			byte []tmp = macProvider.doFinal(text);
			xorResult(result,tmp);
			text = tmp;
			macProvider.reset();
		}
		return result;
	}
	private void xorResult(byte [] l, byte [] r)
	{
		if (l.length != r.length)
			throw new IllegalArgumentException();
		int len = l.length;
		for (int i = 0; i < len; i++)
			l[i] ^= r[i];
	}
	private byte [] salt;
	private byte [] password;
	private int iterationCount;
	private Mac macProvider;
}
