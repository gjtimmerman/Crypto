// SignWithCertificate.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <wincrypt.h>
#include <ncrypt.h>

#define BLOCKSIZE 1024
#define HASHSIZE 32
#define SIGNATURESIZE 256

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
	case NTE_BAD_KEYSET:
		fprintf(stderr, "Cryptographic function returned error code: NTE_BAD_KEYSET");
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


NCRYPT_KEY_HANDLE openKeyFromCertificate(char* subjectName)
{
	size_t subjectNameLen = strlen(subjectName);
	wchar_t* wideSubjectName = malloc((subjectNameLen + 1) * sizeof(wchar_t));
	mbstowcs(wideSubjectName, subjectName, subjectNameLen + 1);
	HCERTSTORE store = CertOpenStore(CERT_STORE_PROV_SYSTEM, 0, 0, CERT_SYSTEM_STORE_CURRENT_USER, L"My");
	PCCERT_CONTEXT context = CertFindCertificateInStore(store, X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,0, CERT_FIND_SUBJECT_STR, wideSubjectName,NULL);
	free(wideSubjectName);
	CRYPT_KEY_PROV_INFO* pCertContext = NULL;
	DWORD cbData = 0;

	BOOL ret = CertGetCertificateContextProperty(context, CERT_KEY_PROV_INFO_PROP_ID, pCertContext, &cbData);
	pCertContext = (CRYPT_KEY_PROV_INFO*)malloc(cbData);
	ret = CertGetCertificateContextProperty(context, CERT_KEY_PROV_INFO_PROP_ID, pCertContext, &cbData);
	SECURITY_STATUS status;
	NCRYPT_PROV_HANDLE provHandle;
	NCRYPT_KEY_HANDLE keyHandle;
	status = NCryptOpenStorageProvider(&provHandle, pCertContext->pwszProvName, 0);
	if (evaluateStatus(status) != 0)
	{
		return 0;
	}
	status = NCryptOpenKey(provHandle, &keyHandle, pCertContext->pwszContainerName, AT_KEYEXCHANGE, 0);
	if (evaluateStatus(status) != 0)
	{
		return 0;
	}
	status = NCryptFreeObject(provHandle);
	if (evaluateStatus(status) != 0)
	{
		return 0;
	}
	return keyHandle;
}

void sign(char* inputFileName, char* subjectName)
{
	NCRYPT_KEY_HANDLE keyHandle;
	keyHandle = openKeyFromCertificate(subjectName);
	BCRYPT_ALG_HANDLE algHandle;
	NTSTATUS bStatus;
	bStatus = BCryptOpenAlgorithmProvider(&algHandle, BCRYPT_SHA256_ALGORITHM, NULL, 0);
	if (evaluateBStatus(bStatus) != 0)
	{
		return;
	}
	ULONG hashLength;
	ULONG cbHashLength = sizeof(ULONG);
	ULONG cbHashLengthReturned;
	bStatus = BCryptGetProperty(algHandle, BCRYPT_HASH_LENGTH, (PUCHAR) & hashLength, cbHashLength, &cbHashLengthReturned, 0);
	if (evaluateBStatus(bStatus) != 0)
	{
		return;
	}
	if (hashLength != HASHSIZE)
	{
		printf("Wrong hash size!");
		return;
	}
	BCRYPT_HASH_HANDLE hashHandle;
	bStatus = BCryptCreateHash(algHandle, &hashHandle, NULL, 0, NULL, 0, 0);
	if (evaluateBStatus(bStatus) != 0)
	{
		return;
	}
	bStatus = BCryptCloseAlgorithmProvider(algHandle,0);
	if (evaluateBStatus(bStatus) != 0)
	{
		return;
	}
	char buffer[BLOCKSIZE];
	FILE* inputFile = fopen(inputFileName, "rb");
	if (inputFile == NULL)
	{
		printf("Input file: %s does not exist!", inputFileName);
		return;
	}
	size_t cb = fread(buffer, 1, BLOCKSIZE, inputFile);
	while (cb == BLOCKSIZE)
	{
		bStatus = BCryptHashData(hashHandle, buffer, BLOCKSIZE, 0);
		if (evaluateBStatus(bStatus) != 0)
		{
			return;
		}
		cb = fread(buffer, 1, BLOCKSIZE, inputFile);
	}
	bStatus = BCryptHashData(hashHandle, buffer, (ULONG)cb, 0);
	if (evaluateBStatus(bStatus) != 0)
	{
		return;
	}
	char hash[HASHSIZE];
	bStatus = BCryptFinishHash(hashHandle, hash, HASHSIZE, 0);
	if (evaluateBStatus(bStatus) != 0)
	{
		return;
	}
	bStatus = BCryptDestroyHash(hashHandle);
	if (evaluateBStatus(bStatus) != 0)
	{
		return;
	}
	char signature[SIGNATURESIZE];
	ULONG cbResult;
	BCRYPT_PKCS1_PADDING_INFO paddingInfo;
	paddingInfo.pszAlgId = BCRYPT_SHA256_ALGORITHM;
	SECURITY_STATUS status = NCryptSignHash(keyHandle, &paddingInfo, hash, HASHSIZE, signature, SIGNATURESIZE, &cbResult, BCRYPT_PAD_PKCS1);
	if (evaluateStatus(status) != 0)
	{
		return;
	}
	status = NCryptFreeObject(keyHandle);
	if (evaluateStatus(status) != 0)
	{
		return;
	}
	char* signatureFileName = malloc(strlen(inputFileName) + strlen(".signature") + 1);
	strcpy(signatureFileName, inputFileName);
	strcat(signatureFileName, ".signature");
	FILE* signatureFile = fopen(signatureFileName, "wb");
	fwrite(signature, 1, SIGNATURESIZE, signatureFile);
	fclose(signatureFile);

}

int verify(char* inputFileName, char* subjectName)
{
	return 0;
}

int main(int argc, char **argv)
{
	if (argc < 4)
	{
		fprintf(stderr, "Too little arguments!\nUsage: %s -s/-v FileName SubjectName", argv[0]);
		return 1;
	}
	if (argc > 4)
	{
		fprintf(stderr, "Too many arguments!\nUsage: %s -s/-v FileName SubjectName", argv[0]);
		return 1;
	}
	if (argv[1][0] != '-')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -s/-v FileName SubjectName", argv[1], argv[0]);
		return 1;
	}
	if (argv[1][1] != 's' && argv[1][1] != 'v')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -s/-v FileName SubjectName", argv[1], argv[0]);
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
