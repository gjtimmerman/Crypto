// EncryptAsymmetricKey.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <ncrypt.h>

#define KEYLENGTH 16
#define RSAMODULUSLEN 512
#define BLOCKSIZE 16

int evaluateStatus(SECURITY_STATUS status)
{
	if (status == ERROR_SUCCESS)
		return 0;
	switch (status)
	{
	case NTE_INVALID_HANDLE:
		fprintf(stderr, "Cryptographic functionr returned error code: NTE_INVALID_HANDLE");
		return 1;
	case NTE_INVALID_PARAMETER:
		fprintf(stderr, "Cryptographic function returned error code: NTE_INVALID_PARAMETER");
		return 1;
	case NTE_BAD_FLAGS:
		fprintf(stderr, "Cryptographic function returned error code: NTE_BAD_FLAGS");
		return 1;

	default:
		fprintf(stderr, "Cryptographic function returned unknown error code");
		return 1;

	}

}

int evaluateBStatus(NTSTATUS status)
{
	if (status == ERROR_SUCCESS)
		return 0;
	switch (status)
	{
	case STATUS_INVALID_HANDLE:
		fprintf(stderr, "Cryptographic functionr returned error code: STATUS_INVALID_HANDLE");
		return 1;
	case STATUS_INVALID_PARAMETER:
		fprintf(stderr, "Cryptographic function returned error code: STATUS_INVALID_PARAMETER");
		return 1;
	case STATUS_NO_MEMORY:
		fprintf(stderr, "Cryptographic function returned error code: STATUS_NO_MEMORY");
		return 1;

	default:
		fprintf(stderr, "Cryptographic function returned unknown error code");
		return 1;

	}

}

void encrypt(char* fileName, char* keyName, char* keyFileName)
{
	SECURITY_STATUS status;
	NTSTATUS bStatus;
	NCRYPT_PROV_HANDLE provHandle;
	NCRYPT_KEY_HANDLE keyHandle;
	wchar_t *keyNameWide = malloc((strlen(keyName) + 1) * sizeof(wchar_t));
	if (keyNameWide == NULL)
		return;
	mbstowcs(keyNameWide, keyName, strlen(keyName)+1);
	status = NCryptOpenStorageProvider(&provHandle, NULL, 0);
	if (evaluateStatus(status) != 0)
		return;
	status = NCryptOpenKey(provHandle,&keyHandle,keyNameWide,AT_KEYEXCHANGE,0);
	if (status == NTE_BAD_KEYSET)
	{
		status = NCryptCreatePersistedKey(provHandle, &keyHandle, NCRYPT_RSA_ALGORITHM,keyNameWide, AT_KEYEXCHANGE, 0);
		if (evaluateStatus(status) != 0)
			return;
		status = NCryptFinalizeKey(keyHandle, 0);
		if (evaluateStatus(status) != 0)
			return;
	}
	else if (evaluateStatus(status) != 0)
		return;
	free(keyNameWide);
	BCRYPT_ALG_HANDLE algHandle;
	bStatus = BCryptOpenAlgorithmProvider(&algHandle, BCRYPT_RNG_ALGORITHM, NULL, 0);
	if (evaluateBStatus(bStatus) != 0)
		return;
	BCRYPT_KEY_DATA_BLOB_HEADER *pKeyDataBlobHeader = malloc(sizeof(BCRYPT_KEY_DATA_BLOB_HEADER) + KEYLENGTH);
	if (pKeyDataBlobHeader == NULL)
		return;
	pKeyDataBlobHeader->dwMagic = BCRYPT_KEY_DATA_BLOB_MAGIC;
	pKeyDataBlobHeader->dwVersion = BCRYPT_KEY_DATA_BLOB_VERSION1;
	pKeyDataBlobHeader->cbKeyData = KEYLENGTH;
	bStatus = BCryptGenRandom(algHandle, ((PUCHAR)pKeyDataBlobHeader) + sizeof(BCRYPT_KEY_DATA_BLOB_HEADER), KEYLENGTH, 0);
	if (evaluateBStatus(bStatus) != 0)
		return;
	bStatus = BCryptCloseAlgorithmProvider(algHandle, 0);
	if (evaluateBStatus(bStatus) != 0)
		return;
	char encryptedKey[RSAMODULUSLEN];
	ULONG cbEncryptedKeyLen;
	BCRYPT_OAEP_PADDING_INFO paddingInfo;
	paddingInfo.cbLabel = 0;
	paddingInfo.pbLabel = NULL;
	paddingInfo.pszAlgId = NCRYPT_SHA256_ALGORITHM;
	status = NCryptEncrypt(keyHandle, ((PUCHAR)pKeyDataBlobHeader) + sizeof(BCRYPT_KEY_DATA_BLOB_HEADER), KEYLENGTH, &paddingInfo, encryptedKey, RSAMODULUSLEN, &cbEncryptedKeyLen, NCRYPT_PAD_OAEP_FLAG);
	if (evaluateStatus(status) != 0)
		return;
	status = NCryptFreeObject(keyHandle);
	if (evaluateStatus(status) != 0)
		return;
	status = NCryptFreeObject(provHandle);
	if (evaluateStatus(status) != 0)
		return;
	FILE* keyFile = fopen(keyFileName, "wb");
	if (keyFile == NULL)
		return;
	fwrite(encryptedKey, 1, cbEncryptedKeyLen, keyFile);
	fclose(keyFile);

	BCRYPT_KEY_HANDLE symKeyHandle;

	bStatus = BCryptOpenAlgorithmProvider(&algHandle, BCRYPT_AES_ALGORITHM, NULL, 0);
	if (evaluateBStatus(bStatus) != 0)
		return;
	bStatus = BCryptImportKey(algHandle, NULL, BCRYPT_KEY_DATA_BLOB, &symKeyHandle, NULL, 0, (PUCHAR)pKeyDataBlobHeader, sizeof(BCRYPT_KEY_DATA_BLOB_HEADER) + KEYLENGTH, 0);
	if (evaluateBStatus(bStatus) != 0)
		return;
	free(pKeyDataBlobHeader);
	FILE* inputFile = fopen(fileName, "rb");
	if (inputFile == NULL)
	{
		status = BCryptDestroyKey(symKeyHandle);
		if (evaluateBStatus(status) != 0)
			return;
		return;
	}
	char* outputFileName = malloc(strlen(fileName) + strlen(".encrypted") + 1);
	strcpy(outputFileName, fileName);
	strcat(outputFileName, ".encrypted");
	FILE* outputFile = fopen(outputFileName, "wb");
	free(outputFileName);
	if (outputFile == NULL)
	{
		fclose(inputFile);
		status = BCryptDestroyKey(symKeyHandle);
		if (evaluateBStatus(status) != 0)
			return;
		return;
	}
	char buffer[BLOCKSIZE];
	size_t cb = fread(buffer, 1, BLOCKSIZE, inputFile);
	char encrypted[BLOCKSIZE];
	DWORD cbResult;
	while (cb == BLOCKSIZE)
	{
		status = BCryptEncrypt(symKeyHandle, buffer, BLOCKSIZE, NULL, NULL, 0, encrypted, BLOCKSIZE, &cbResult, 0);
		if (evaluateBStatus(status) != 0)
		{
			fclose(inputFile);
			fclose(outputFile);
			status = BCryptDestroyKey(symKeyHandle);
			if (evaluateBStatus(status) != 0)
				return;
			return;
		}
		fwrite(encrypted, 1, BLOCKSIZE, outputFile);
		cb = fread(buffer, 1, BLOCKSIZE, inputFile);
	}
	status = BCryptEncrypt(symKeyHandle, buffer, (ULONG)cb, NULL, NULL, 0, encrypted, BLOCKSIZE, &cbResult, BCRYPT_BLOCK_PADDING);
	if (evaluateBStatus(status) != 0)
	{
		fclose(inputFile);
		fclose(outputFile);
		status = BCryptDestroyKey(symKeyHandle);
		if (evaluateBStatus(status) != 0)
			return;
		return;
	}
	fwrite(encrypted, 1, cbResult, outputFile);
	fclose(inputFile);
	fclose(outputFile);
	status = BCryptDestroyKey(symKeyHandle);
	if (evaluateBStatus(status) != 0)
		return;
	status = BCryptCloseAlgorithmProvider(algHandle,0);
	if (evaluateBStatus(status) != 0)
		return;

}

void decrypt(char* fileName, char* keyName, char* keyFileName)
{
	SECURITY_STATUS status;
	NCRYPT_PROV_HANDLE provHandle;
	NCRYPT_KEY_HANDLE keyHandle;

	wchar_t* keyNameWide = malloc((strlen(keyName) + 1) * sizeof(wchar_t));
	if (keyNameWide == NULL)
		return;
	mbstowcs(keyNameWide, keyName, strlen(keyName) + 1);
	status = NCryptOpenStorageProvider(&provHandle, NULL, 0);
	if (evaluateStatus(status) != 0)
		return;
	status = NCryptOpenKey(provHandle, &keyHandle, keyNameWide, AT_KEYEXCHANGE, 0);
	if (status == NTE_BAD_KEYSET)
	{
		fprintf(stderr, "Key does not exist!");
	}
	else if (evaluateStatus(status) != 0)
		return;
	free(keyNameWide);

	FILE* keyFile = fopen(keyFileName, "rb");
	if (keyFile == NULL)
		return;
	size_t cbRead;
	char encryptedKey[RSAMODULUSLEN];
	cbRead = fread(encryptedKey, 1, RSAMODULUSLEN, keyFile);
	fclose(keyFile);

	BCRYPT_KEY_DATA_BLOB_HEADER* pKeyDataBlobHeader = malloc(sizeof(BCRYPT_KEY_DATA_BLOB_HEADER) + KEYLENGTH);
	if (pKeyDataBlobHeader == NULL)
		return;
	pKeyDataBlobHeader->dwMagic = BCRYPT_KEY_DATA_BLOB_MAGIC;
	pKeyDataBlobHeader->dwVersion = BCRYPT_KEY_DATA_BLOB_VERSION1;
	pKeyDataBlobHeader->cbKeyData = KEYLENGTH;

	BCRYPT_OAEP_PADDING_INFO paddingInfo;
	paddingInfo.cbLabel = 0;
	paddingInfo.pbLabel = NULL;
	paddingInfo.pszAlgId = NCRYPT_SHA256_ALGORITHM;
	ULONG cbDecryptedKeyLen;
	status = NCryptDecrypt(keyHandle, encryptedKey, (DWORD)cbRead, &paddingInfo,((PBYTE)pKeyDataBlobHeader) + sizeof(BCRYPT_KEY_DATA_BLOB_HEADER), KEYLENGTH, &cbDecryptedKeyLen, NCRYPT_PAD_OAEP_FLAG);
	if (evaluateStatus(status) != 0)
		return;
	status = NCryptFreeObject(keyHandle);
	if (evaluateStatus(status) != 0)
		return;
	status = NCryptFreeObject(provHandle);
	if (evaluateStatus(status) != 0)
		return;
	NTSTATUS bStatus;
	BCRYPT_ALG_HANDLE algHandle;
	BCRYPT_KEY_HANDLE symKeyHandle;

	bStatus = BCryptOpenAlgorithmProvider(&algHandle, BCRYPT_AES_ALGORITHM, NULL, 0);
	if (evaluateBStatus(bStatus) != 0)
		return;
	bStatus = BCryptImportKey(algHandle, NULL, BCRYPT_KEY_DATA_BLOB, &symKeyHandle, NULL, 0, (PUCHAR)pKeyDataBlobHeader, sizeof(BCRYPT_KEY_DATA_BLOB_HEADER) + KEYLENGTH, 0);
	if (evaluateBStatus(bStatus) != 0)
		return;
	free(pKeyDataBlobHeader);

	FILE* inputFile = fopen(fileName, "rb");
	if (inputFile == NULL)
	{
		status = BCryptDestroyKey(symKeyHandle);
		if (evaluateBStatus(status) != 0)
			return;
		return;
	}
	char* outputFileName = malloc(strlen(fileName) + strlen(".decrypted") + 1);
	strcpy(outputFileName, fileName);
	strcat(outputFileName, ".decrypted");
	FILE* outputFile = fopen(outputFileName, "wb");
	free(outputFileName);
	if (outputFile == NULL)
	{
		fclose(inputFile);
		status = BCryptDestroyKey(symKeyHandle);
		if (evaluateBStatus(status) != 0)
			return;
		return;
	}
	char buffer1[BLOCKSIZE];
	char buffer2[BLOCKSIZE];
	char* pBuffer1 = buffer1;
	char* pBuffer2 = buffer2;
	char decrypted[BLOCKSIZE];
	DWORD cbResult;
	size_t cb = fread(pBuffer1, 1, BLOCKSIZE, inputFile);
	if (cb == BLOCKSIZE)
	{
		cb = fread(pBuffer2, 1, BLOCKSIZE, inputFile);
		while (cb == BLOCKSIZE)
		{
			status = BCryptDecrypt(symKeyHandle, pBuffer1, BLOCKSIZE, NULL, NULL, 0, decrypted, BLOCKSIZE, &cbResult, 0);
			if (evaluateBStatus(status) != 0)
			{
				fclose(inputFile);
				fclose(outputFile);
				status = BCryptDestroyKey(symKeyHandle);
				if (evaluateBStatus(status) != 0)
					return;
				return;
			}
			fwrite(decrypted, 1, BLOCKSIZE, outputFile);
			char* tmp = pBuffer1;
			pBuffer1 = pBuffer2;
			pBuffer2 = tmp;
			cb = fread(pBuffer2, 1, BLOCKSIZE, inputFile);
		}
		if (cb != 0)
		{
			fprintf(stderr, "Invalid padding");
			fclose(inputFile);
			fclose(outputFile);
			status = BCryptDestroyKey(symKeyHandle);
			if (evaluateBStatus(status) != 0)
				return;
			return;
		}
	}
	status = BCryptDecrypt(symKeyHandle, pBuffer1, BLOCKSIZE, NULL, NULL, 0, decrypted, BLOCKSIZE, &cbResult, BCRYPT_BLOCK_PADDING);
	if (evaluateBStatus(status) != 0)
	{
		status = BCryptDestroyKey(symKeyHandle);
		fclose(inputFile);
		fclose(outputFile);
		return;
	}
	fwrite(decrypted, 1, cbResult, outputFile);
	fclose(inputFile);
	fclose(outputFile);
	status = BCryptDestroyKey(symKeyHandle);
	if (evaluateBStatus(status) != 0)
		return;
	status = BCryptCloseAlgorithmProvider(algHandle, 0);
	if (evaluateBStatus(status) != 0)
		return;

}

int main(int argc, char **argv)
{
	if (argc < 5)
	{
		fprintf(stderr, "Too little arguments!\nUsage: %s -e/-d FileName KeyName KeyFileName", argv[0]);
		return 1;
	}
	if (argc > 5)
	{
		fprintf(stderr, "Too many arguments!\nUsage: %s -e/-d FileName KeyName KeyFileName", argv[0]);
		return 1;
	}
	if (argv[1][0] != '-')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -e/-d FileName KeyName KeyFileName", argv[1], argv[0]);
		return 1;
	}
	if (argv[1][1] != 'e' && argv[1][1] != 'd')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -e/-d FileName KeyName KeyFileName", argv[1], argv[0]);
		return 1;
	}
	if (argv[1][1] == 'e')
		encrypt(argv[2], argv[3], argv[4]);
	else
		decrypt(argv[2], argv[3], argv[4]);
	return 0;
}

