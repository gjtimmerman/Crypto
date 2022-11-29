// MyPBETest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//


#include <iostream>
#include <windows.h>
#include <ncrypt.h>

void PbeBcrypt()
{
	unsigned char hash[32];
	for (unsigned char i = 0; i < 32; i++)
		hash[i] = i;
	SECURITY_STATUS status;
	BCRYPT_ALG_HANDLE algHandle;
	status = BCryptOpenAlgorithmProvider(&algHandle, BCRYPT_PBKDF2_ALGORITHM, NULL, 0);
	if (status != ERROR_SUCCESS)
		throw std::runtime_error("BCryptKeyDerivation returned errorcode");
	BCRYPT_KEY_HANDLE bkeyHandle;
	const wchar_t* password = L"MyPassWord";
	status = BCryptGenerateSymmetricKey(algHandle, &bkeyHandle, NULL, 0, (PBYTE)password, (DWORD)(wcslen(password) + 1) * sizeof(wchar_t), 0);
	if (status != ERROR_SUCCESS)
		throw std::runtime_error("BCryptKeyDerivation returned errorcode");
	BCryptBufferDesc bufferDesc;
	BCryptBuffer cryptBuffers[3];
	cryptBuffers[0].BufferType = KDF_HASH_ALGORITHM;
	cryptBuffers[0].pvBuffer = (PVOID)BCRYPT_SHA256_ALGORITHM;
	cryptBuffers[0].cbBuffer = 14;
	cryptBuffers[1].BufferType = KDF_SALT;
	cryptBuffers[1].cbBuffer = 32;
	cryptBuffers[1].pvBuffer = hash;
	unsigned long long iterationCount = 1000;
	cryptBuffers[2].BufferType = KDF_ITERATION_COUNT;
	cryptBuffers[2].cbBuffer = sizeof(unsigned long long);
	cryptBuffers[2].pvBuffer = &iterationCount;
	bufferDesc.ulVersion = BCRYPTBUFFER_VERSION;
	bufferDesc.cBuffers = 3;
	bufferDesc.pBuffers = cryptBuffers;
	unsigned char derivedKey[32];
	DWORD cbResult;
	status = BCryptKeyDerivation(bkeyHandle, &bufferDesc, derivedKey, 32, &cbResult, 0);
	if (status != ERROR_SUCCESS)
		throw std::runtime_error("BCryptKeyDerivation returned errorcode");
	status = BCryptDestroyKey(bkeyHandle);
	if (status != ERROR_SUCCESS)
		throw std::runtime_error("BCryptDestroyKey returned errorcode");
	std::cout << cbResult << std::endl;
	std::cout << std::endl;
	for (unsigned int i = 0; i < cbResult; i++)
	{
		std::cout.width(2);
		std::cout.fill('0');
		std::cout << std::hex << (unsigned short)derivedKey[i];
	}
	std::cout << std::endl;

}



int main()
{
	unsigned char hash[32];
	for (unsigned char i = 0; i < 32; i++)
		hash[i] = i;
	SECURITY_STATUS status;
	NCRYPT_PROV_HANDLE provHandle;
	status = NCryptOpenStorageProvider(&provHandle, 0, 0);
	if (status != ERROR_SUCCESS)
		throw std::runtime_error("NCryptOpenStorageProvider returned errorcode");
	NCRYPT_KEY_HANDLE keyHandle;
	status = NCryptCreatePersistedKey(provHandle, &keyHandle, NCRYPT_PBKDF2_ALGORITHM,0, 0, 0);
	if (status != ERROR_SUCCESS)
		throw std::runtime_error("NCryptCreatePersistedKey returned errorcode");
	const wchar_t *password = L"MyPassWord";
	status = NCryptSetProperty(keyHandle, NCRYPT_KDF_SECRET_VALUE, (PBYTE)password,(DWORD) (wcslen(password) + 1) * sizeof(wchar_t), 0);
	if (status != ERROR_SUCCESS)
		throw std::runtime_error("NCryptSetProperty returned errorcode");
	NCryptBufferDesc bufferDesc;
	NCryptBuffer cryptBuffers[3];
	cryptBuffers[0].BufferType = KDF_HASH_ALGORITHM;
	cryptBuffers[0].pvBuffer = (PVOID)NCRYPT_SHA256_ALGORITHM;
	cryptBuffers[0].cbBuffer = (DWORD)(wcslen(NCRYPT_SHA256_ALGORITHM) + 1) * sizeof(wchar_t);
	cryptBuffers[1].BufferType = KDF_SALT;
	cryptBuffers[1].cbBuffer = 32;
	cryptBuffers[1].pvBuffer = hash;
	unsigned long long iterationCount = 1000;
	cryptBuffers[2].BufferType = KDF_ITERATION_COUNT;
	cryptBuffers[2].cbBuffer = sizeof(unsigned long long);
	cryptBuffers[2].pvBuffer = &iterationCount;
	bufferDesc.ulVersion = NCRYPTBUFFER_VERSION;
	bufferDesc.cBuffers = 3ul;
	bufferDesc.pBuffers = cryptBuffers;
	unsigned char derivedKey[32];
	DWORD cbResult;
	status = NCryptKeyDerivation(keyHandle, &bufferDesc, derivedKey, 32, &cbResult, 0);
	if (status != ERROR_SUCCESS)
		throw std::runtime_error("NCryptKeyDerivation returned errorcode");
	status = NCryptFreeObject(keyHandle);
	if (status != ERROR_SUCCESS)
		throw std::runtime_error("NCryptDeleteKey returned errorcode");
	std::cout << cbResult << std::endl;
	std::cout << std::endl;
	for (unsigned int i = 0; i < cbResult; i++)
	{
		std::cout.width(2);
		std::cout.fill('0');
		std::cout << std::hex << (unsigned short)derivedKey[i];
	}
	std::cout << std::endl;
//	PbeBcrypt();
}

