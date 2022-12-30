// SignUsingHMAC.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <stdio.h>
#include <windows.h>
#include <bcrypt.h>

#define KEYLENGTH 64
#define BUFFERSIZE 1024
#define HASHSIZE 32

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

void computeHash(FILE *inputFile, char symKey[], char hashValue[])
{
	NTSTATUS status;
	BCRYPT_ALG_HANDLE algHandle;
	BCRYPT_HASH_HANDLE hashHandle;
	status = BCryptOpenAlgorithmProvider(&algHandle, BCRYPT_SHA256_ALGORITHM, NULL, BCRYPT_ALG_HANDLE_HMAC_FLAG);
	if (evaluateStatus(status) != 0)
	{
		return;
	}
	status = BCryptCreateHash(algHandle, &hashHandle, NULL, 0, symKey, KEYLENGTH, 0);
	if (evaluateStatus(status) != 0)
	{
		BCryptCloseAlgorithmProvider(algHandle, 0);
		return;
	}
	ULONG cbHash;
	ULONG cbResult;
	status = BCryptGetProperty(hashHandle, BCRYPT_HASH_LENGTH, (PUCHAR)&cbHash, sizeof(ULONG), &cbResult, 0);
	if (evaluateStatus(status) != 0)
	{
		BCryptDestroyHash(hashHandle);
		BCryptCloseAlgorithmProvider(algHandle, 0);
		return;
	}
	if (cbHash != HASHSIZE)
	{
		fprintf(stderr, "Hash size not correct!");
		BCryptCloseAlgorithmProvider(algHandle, 0);
		BCryptDestroyHash(hashHandle);
		return;

	}
	char buffer[BUFFERSIZE];
	size_t cb = fread(buffer, 1, BUFFERSIZE, inputFile);
	while (cb == BUFFERSIZE)
	{
		status = BCryptHashData(hashHandle, buffer, BUFFERSIZE, 0);
		if (evaluateStatus(status) != 0)
		{
			BCryptDestroyHash(hashHandle);
			BCryptCloseAlgorithmProvider(algHandle, 0);
			return;
		}
		cb = fread(buffer, 1, BUFFERSIZE, inputFile);
	}
	status = BCryptHashData(hashHandle, buffer, (ULONG)cb, 0);
	if (evaluateStatus(status) != 0)
	{
		BCryptDestroyHash(hashHandle);
		BCryptCloseAlgorithmProvider(algHandle, 0);
		return;
	}
	status = BCryptFinishHash(hashHandle, hashValue, HASHSIZE, 0);
	if (evaluateStatus(status) != 0)
	{
		BCryptDestroyHash(hashHandle);
		BCryptCloseAlgorithmProvider(algHandle, 0);
		return;
	}
	status = BCryptDestroyHash(hashHandle);
	if (evaluateStatus(status) != 0)
	{
		BCryptCloseAlgorithmProvider(algHandle, 0);
		return;
	}
	status = BCryptCloseAlgorithmProvider(algHandle, 0);
	if (evaluateStatus(status) != 0)
	{
		return;
	}

	return;
}

void sign(char* fileName, char* keyFileName)
{
	FILE* keyFile = fopen(keyFileName, "rb");
	char symKey[KEYLENGTH];
	NTSTATUS status;
	BCRYPT_ALG_HANDLE algHandle;
	if (keyFile == NULL)
	{
		keyFile = fopen(keyFileName, "wb");
		if (keyFile == NULL)
		{
			fprintf(stderr, "Cannot create key file!");
			return;
		}
		status = BCryptOpenAlgorithmProvider(&algHandle, BCRYPT_RNG_ALGORITHM, NULL, 0);
		if (evaluateStatus(status) != 0)
		{
			fclose(keyFile);
			return;
		}
		status = BCryptGenRandom(algHandle, symKey, KEYLENGTH, 0);
		if (evaluateStatus(status) != 0)
		{
			status = BCryptCloseAlgorithmProvider(algHandle, 0);
			if (evaluateStatus(status) != 0)
			{
				fclose(keyFile);
				return;
			}
			fclose(keyFile);
			return;
		}
		status = BCryptCloseAlgorithmProvider(algHandle, 0);
		if (evaluateStatus(status) != 0)
		{
			fclose(keyFile);
			return;
		}
		fwrite(symKey, 1, KEYLENGTH, keyFile);
		fclose(keyFile);
	}
	else
	{
		size_t cb = fread(symKey, 1, KEYLENGTH, keyFile);
		if (cb != KEYLENGTH)
		{
			fprintf(stderr, "Key in keyfile not the right size!");
			fclose(keyFile);
			return;
		}
		fclose(keyFile);
	}
	FILE* inputFile = fopen(fileName, "rb");
	if (inputFile == NULL)
	{
		fprintf(stderr, "Cannot open file: %s", fileName);
		return;
	}
	char hashValue[HASHSIZE];
	computeHash(inputFile, symKey, hashValue);
	fclose(inputFile);
	char* signatureFileName = malloc(strlen(fileName) + strlen(".signature") + 1);
	if (signatureFileName == NULL)
	{
		fprintf(stderr, "Out of memory");
		return;
	}
	strcpy(signatureFileName, fileName);
	strcat(signatureFileName, ".signature");
	FILE* signatureFile = fopen(signatureFileName,"wb");
	if (signatureFile == NULL)
	{
		fprintf(stderr, "Cannot open file: %s", signatureFileName);
		free(signatureFileName);
		return;
	}
	free(signatureFileName);
	fwrite(hashValue, 1, HASHSIZE, signatureFile);
	fclose(signatureFile);

}

int verify(char* fileName, char* keyFileName)
{
	FILE* keyFile = fopen(keyFileName,"rb");
	if (keyFile == NULL)
	{
		fprintf(stderr, "Keyfile: %s does not exist!", keyFileName);
		return 0;
	}
	char symKey[KEYLENGTH];
	size_t cb = fread(symKey, 1, KEYLENGTH, keyFile);
	fclose(keyFile);
	if (cb != KEYLENGTH)
	{
		fprintf(stderr, "Key in Keyfile : %s has incorrect size!", keyFileName);
		return 0;
	}
	FILE* inputFile = fopen(fileName, "rb");
	if (inputFile == NULL)
	{
		fprintf(stderr, "Inputfile: %s does not exist!", fileName);
		return 0;
	} 
	char hashValue[HASHSIZE];
	char* signatureFileName = malloc(strlen(fileName) + strlen(".signature") + 1);
	if (signatureFileName == NULL)
	{
		fprintf(stderr, "Out of memory");
		fclose(inputFile);
		return 0;
	}

	strcpy(signatureFileName, fileName);
	strcat(signatureFileName, ".signature");
	FILE* signatureFile = fopen(signatureFileName, "rb");
	if (signatureFile == NULL)
	{
		fprintf(stderr, "Signature file: %s does not exist!", signatureFileName);
		free(signatureFileName);
		return 0;
	}
	char signatureValue[HASHSIZE];
	cb = fread(signatureValue, 1, HASHSIZE, signatureFile);
	fclose(signatureFile);
	if (cb != HASHSIZE)
	{
		fprintf(stderr, "Signature in signature file : %s has incorrect size!", signatureFileName);
		return 0;
	}
	free(signatureFileName);
	computeHash(inputFile, symKey, hashValue);
	fclose(inputFile);
	for (int i = 0; i < HASHSIZE; i++)
	{
		if (hashValue[i] != signatureValue[i])
			return 0;
	}
	return 1;

}

int main(int argc, char **argv)
{
	if (argc < 4)
	{
		fprintf(stderr, "Too little arguments!\nUsage: %s -s/-v FileName KeyFileName", argv[0]);
		return 1;
	}
	if (argc > 4)
	{
		fprintf(stderr, "Too many arguments!\nUsage: %s -s/-v FileName KeyFileName", argv[0]);
		return 1;
	}
	if (argv[1][0] != '-')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -s/-v FileName KeyFileName", argv[1], argv[0]);
		return 1;
	}
	if (argv[1][1] != 's' && argv[1][1] != 'v')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -s/-v FileName KeyFileName", argv[1], argv[0]);
		return 1;
	}
	if (argv[1][1] == 's')
		sign(argv[2], argv[3]);
	else
		if (verify(argv[2], argv[3]))
			printf("Signature is valid!");
		else
			printf("Signature is invalid!");

}

