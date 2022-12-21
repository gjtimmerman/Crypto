// EncryptPasswordSymmetricKey.c : This file contains the 'main' function. Program execution begins and ends there.
//

#include <stdio.h>
#include <windows.h>
#include <bcrypt.h>

#define KEYLENGTH 16
#define KEYSTRUCTSIZE (sizeof(BCRYPT_KEY_DATA_BLOB_HEADER) + KEYLENGTH)
#define BLOCKSIZE 16
#define SALTSIZE 32
#define ITERATIONCOUNT 1000

char iv[] = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 };
char salt[] = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31 };

int evaluateStatus(NTSTATUS status)
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


void encrypt(const char* fileName, const char* password)
{
	NTSTATUS status;
	BCRYPT_ALG_HANDLE algProvider;
	char* pkeyData = malloc(KEYSTRUCTSIZE);
	BCRYPT_KEY_HANDLE keyHandle;
	status = BCryptOpenAlgorithmProvider(&algProvider, BCRYPT_SHA256_ALGORITHM, 0, BCRYPT_ALG_HANDLE_HMAC_FLAG);
	if (evaluateStatus(status) != 0)
		return;
	status = BCryptDeriveKeyPBKDF2(algProvider, (PUCHAR)password, (ULONG)(strlen(password) + 1), salt, SALTSIZE, ITERATIONCOUNT, pkeyData, KEYLENGTH, 0);
	if (evaluateStatus(status) != 0)
		return;
	status = BCryptCloseAlgorithmProvider(algProvider,0);
	if (evaluateStatus(status) != 0)
		return;
	status = BCryptOpenAlgorithmProvider(&algProvider, BCRYPT_AES_ALGORITHM, 0, 0);
	if (evaluateStatus(status) != 0)
		return;
	BCRYPT_KEY_DATA_BLOB_HEADER* pkeyDataHeader = (BCRYPT_KEY_DATA_BLOB_HEADER*)pkeyData;
	pkeyDataHeader->dwMagic = BCRYPT_KEY_DATA_BLOB_MAGIC;
	pkeyDataHeader->dwVersion = BCRYPT_KEY_DATA_BLOB_VERSION1;
	pkeyDataHeader->cbKeyData = KEYLENGTH;

	status = BCryptImportKey(algProvider, 0, BCRYPT_KEY_DATA_BLOB, &keyHandle,NULL,0, (PBYTE)pkeyDataHeader, KEYSTRUCTSIZE,0);
	free(pkeyData);
	if (evaluateStatus(status) != 0)
		return;
	FILE* inputFile = fopen(fileName, "rb");
	if (inputFile == NULL)
	{
		status = BCryptDestroyKey(keyHandle);
		if (evaluateStatus(status) != 0)
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
		status = BCryptDestroyKey(keyHandle);
		if (evaluateStatus(status) != 0)
			return;
		return;
	}
	char buffer[BLOCKSIZE];
	size_t cb = fread(buffer, 1, BLOCKSIZE, inputFile);
	char encrypted[BLOCKSIZE];
	DWORD cbResult;
	while (cb == BLOCKSIZE)
	{
		status = BCryptEncrypt(keyHandle, buffer, BLOCKSIZE, NULL, NULL, 0, encrypted, BLOCKSIZE, &cbResult, 0);
		if (evaluateStatus(status) != 0)
		{
			fclose(inputFile);
			fclose(outputFile);
			status = BCryptDestroyKey(keyHandle);
			if (evaluateStatus(status) != 0)
				return;
			return;
		}
		fwrite(encrypted, 1, BLOCKSIZE, outputFile);
		cb = fread(buffer, 1, BLOCKSIZE, inputFile);
	}
	status = BCryptEncrypt(keyHandle, buffer,(ULONG) cb, NULL, NULL, 0, encrypted, BLOCKSIZE, &cbResult, BCRYPT_BLOCK_PADDING);
	if (evaluateStatus(status) != 0)
	{
		fclose(inputFile);
		fclose(outputFile);
		status = BCryptDestroyKey(keyHandle);
		if (evaluateStatus(status) != 0)
			return;
		return;
	}
	fwrite(encrypted, 1, cbResult, outputFile);
	fclose(inputFile);
	fclose(outputFile);
	status = BCryptDestroyKey(keyHandle);
	if (evaluateStatus(status) != 0)
		return;
}

void decrypt(const char* fileName, const char* password)
{
	BCRYPT_ALG_HANDLE algProvider;
	NTSTATUS status;
	status = BCryptOpenAlgorithmProvider(&algProvider, BCRYPT_SHA256_ALGORITHM, 0, BCRYPT_ALG_HANDLE_HMAC_FLAG);
	if (evaluateStatus(status) != 0)
		return;
	char* pkeyData = malloc(KEYSTRUCTSIZE);
	status = BCryptDeriveKeyPBKDF2(algProvider, (PUCHAR)password, (ULONG)(strlen(password) + 1), salt, SALTSIZE, ITERATIONCOUNT, pkeyData, KEYLENGTH, 0);
	if (evaluateStatus(status) != 0)
		return;
	status = BCryptCloseAlgorithmProvider(algProvider, 0);
	if (evaluateStatus(status) != 0)
		return;
	status = BCryptOpenAlgorithmProvider(&algProvider, BCRYPT_AES_ALGORITHM, 0, 0);
	if (evaluateStatus(status) != 0)
		return;

	BCRYPT_KEY_HANDLE keyHandle;
	BCRYPT_KEY_DATA_BLOB_HEADER* pkeyDataHeader = (BCRYPT_KEY_DATA_BLOB_HEADER*)pkeyData;
	pkeyDataHeader->dwMagic = BCRYPT_KEY_DATA_BLOB_MAGIC;
	pkeyDataHeader->dwVersion = BCRYPT_KEY_DATA_BLOB_VERSION1;
	pkeyDataHeader->cbKeyData = KEYLENGTH;
	status = BCryptImportKey(algProvider, 0, BCRYPT_KEY_DATA_BLOB, &keyHandle, NULL, 0, (PBYTE)pkeyDataHeader, KEYSTRUCTSIZE, 0);
	free(pkeyData);
	if (evaluateStatus(status) != 0)
		return;
	FILE* inputFile = fopen(fileName, "rb");
	if (inputFile == NULL)
	{
		status = BCryptDestroyKey(keyHandle);
		if (evaluateStatus(status) != 0)
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
		status = BCryptDestroyKey(keyHandle);
		if (evaluateStatus(status) != 0)
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
			status = BCryptDecrypt(keyHandle, pBuffer1, BLOCKSIZE, NULL, NULL, 0, decrypted, BLOCKSIZE, &cbResult, 0);
			if (evaluateStatus(status) != 0)
			{
				fclose(inputFile);
				fclose(outputFile);
				status = BCryptDestroyKey(keyHandle);
				if (evaluateStatus(status) != 0)
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
			status = BCryptDestroyKey(keyHandle);
			if (evaluateStatus(status) != 0)
				return;
			return;
		}
	}
	status = BCryptDecrypt(keyHandle, pBuffer1, BLOCKSIZE, NULL, NULL, 0, decrypted, BLOCKSIZE, &cbResult, BCRYPT_BLOCK_PADDING);
	if (evaluateStatus(status) != 0)
	{
		fclose(inputFile);
		fclose(outputFile);
		return;
	}
	fwrite(decrypted, 1, cbResult, outputFile);
	fclose(inputFile);
	fclose(outputFile);
	status = BCryptDestroyKey(keyHandle);
	if (evaluateStatus(status) != 0)
		return;
}


int main(int argc, char **argv)
{
	if (argc < 4)
	{
		fprintf(stderr, "Too little arguments!\nUsage: %s -e/-d FileName Password", argv[0]);
		return 1;
	}
	if (argc > 4)
	{
		fprintf(stderr, "Too many arguments!\nUsage: %s -e/-d FileName Password", argv[0]);
		return 1;
	}
	if (argv[1][0] != '-')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -e/-d FileName Password", argv[1],argv[0]);
		return 1;
	}
	if (argv[1][1] != 'e' && argv[1][1] != 'd')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -e/-d FileName Password", argv[1], argv[0]);
		return 1;
	}
	if (argv[1][1] == 'e')
		encrypt(argv[2], argv[3]);
	else
		decrypt(argv[2], argv[3]);
	return 0;
}

