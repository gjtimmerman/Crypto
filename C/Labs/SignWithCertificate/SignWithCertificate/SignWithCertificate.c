// SignWithCertificate.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <stdio.h>
#include <windows.h>
#include <wincrypt.h>

int main()
{
	HCERTSTORE store = CertOpenStore(CERT_STORE_PROV_SYSTEM, 0, 0, CERT_SYSTEM_STORE_CURRENT_USER, L"My");
//	PCCERT_CONTEXT prevContext = NULL;
	PCCERT_CONTEXT context = CertEnumCertificatesInStore(store, NULL);
	while (context != NULL)
	{
//		prevContext = context;
		PCERT_INFO certInfo = context->pCertInfo;
		wchar_t subject[200];
		DWORD cb = CertNameToStr(context->dwCertEncodingType, &(certInfo->Subject), CERT_SIMPLE_NAME_STR, subject, 200);
		if (wcscmp(subject, L"LONDON") == 0)
		{
			printf("Gevonden: %S!\n", subject);
			break;
		}
		context = CertEnumCertificatesInStore(store, context);
	}
	CRYPT_KEY_PROV_INFO *certContext = (CRYPT_KEY_PROV_INFO*)malloc(0xd6);
	DWORD cbData = 0xd6;
	BOOL ret = CertGetCertificateContextProperty(context, CERT_KEY_PROV_INFO_PROP_ID, certContext, &cbData);
	if (!ret)
	{
		DWORD error = GetLastError();
		if (error == CRYPT_E_NOT_FOUND)
			printf("CRYPT_E_NOT_FOUND");
	}
	else
	{
		wprintf(certContext->pwszContainerName);
		wprintf(certContext->pwszProvName);
	}
}
