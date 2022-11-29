// EncryptPasswordSymmetricKey.c : This file contains the 'main' function. Program execution begins and ends there.
//

#include <stdio.h>
#include <windows.h>
#include <ncrypt.h>

char iv[] = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 };
char salt[] = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31 };

int evaluateStatus(SECURITY_STATUS status)
{
	if (status == ERROR_SUCCESS)
		return 0;
	switch (status)
	{
	case NTE_BAD_FLAGS:
		fprintf(stderr,"Cryptographic function returned error code: NTE_BAD_FLAGS");
		return 1;
	case NTE_BAD_KEY_STATE:
		fprintf(stderr, "Cryptographic function returned error code: NTE_BAD_KEY_STATE");
		return 1;
	case NTE_BAD_TYPE:
		fprintf(stderr, "Cryptographic function returned error code: NTE_BAD_TYPE");
		return 1;
	case NTE_INVALID_HANDLE:
		fprintf(stderr, "Cryptographic functionr returned error code: NTE_INVALID_HANDLE");
		return 1;
	case NTE_INVALID_PARAMETER:
		fprintf(stderr, "Cryptographic function returned error code: NTE_INVALID_PARAMETER");
		return 1;
	case NTE_NOT_SUPPORTED:
		fprintf(stderr, "Cryptographic function returned error code: NTE_NOT_SUPPORTED");
		return 1;

	default:
		fprintf(stderr, "Cryptographic function returned unknown error code");
		return 1;

	}

}

void deriveKey(const char* password, char *keyBuffer, NCRYPT_PROV_HANDLE provHandle)
{
	NCRYPT_KEY_HANDLE keyHandle;
	SECURITY_STATUS status;
	status = NCryptCreatePersistedKey(provHandle, &keyHandle, NCRYPT_PBKDF2_ALGORITHM, NULL, 0, 0);
	if (evaluateStatus(status) != 0)
		return;
	status = NCryptSetProperty(keyHandle, NCRYPT_KDF_SECRET_VALUE, (PBYTE)password, (DWORD)strlen(password) + 1, 0);
	if (evaluateStatus(status) != 0)
		return;
	NCryptBufferDesc bufferDesc;
	NCryptBuffer cryptBuffers[3];
	cryptBuffers[0].BufferType = KDF_HASH_ALGORITHM;
	cryptBuffers[0].pvBuffer = (PVOID)NCRYPT_SHA256_ALGORITHM;
	cryptBuffers[0].cbBuffer = (DWORD)(wcslen(NCRYPT_SHA256_ALGORITHM) + 1) * sizeof(wchar_t);
	cryptBuffers[1].BufferType = KDF_SALT;
	cryptBuffers[1].cbBuffer = 32;
	cryptBuffers[1].pvBuffer = salt;
	unsigned long long iterationCount = 10000;
	cryptBuffers[2].BufferType = KDF_ITERATION_COUNT;
	cryptBuffers[2].cbBuffer = sizeof(unsigned long long);
	cryptBuffers[2].pvBuffer = &iterationCount;
	bufferDesc.ulVersion = NCRYPTBUFFER_VERSION;
	bufferDesc.cBuffers = 3ul;
	bufferDesc.pBuffers = cryptBuffers;
	DWORD cbResult;
	status = NCryptKeyDerivation(keyHandle, &bufferDesc, keyBuffer, 32, &cbResult, 0);
	if (evaluateStatus(status) != 0)
		return;
	status = NCryptFreeObject(keyHandle);
	if (evaluateStatus(status) != 0)
		return;
	return;
}

void encrypt(const char* fileName, const char* password)
{
	NCRYPT_PROV_HANDLE provHandle;
	NCRYPT_KEY_HANDLE keyHandle;
	SECURITY_STATUS status;
	status = NCryptOpenStorageProvider(&provHandle, 0, 0);
	if (evaluateStatus(status) != 0)
		return;
	char keyBuffer[32];
	deriveKey(password, keyBuffer, provHandle);
	NCryptBufferDesc bufferDesc;
	BCRYPT_KEY_DATA_BLOB_HEADER keyDataHeader;
	status = NCryptImportKey(provHandle, 0, BCRYPT_KEY_DATA_BLOB, &bufferDesc, &keyHandle,(PBYTE) & keyDataHeader, sizeof(BCRYPT_KEY_DATA_BLOB_HEADER), 0);
	if (evaluateStatus(status) != 0)
		return;

}

void decrypt(const char* filename, const char* password)
{

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

