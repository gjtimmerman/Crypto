// EncryptAsymmetricKey.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <stdio.h>
#include <windows.h>
#include <ncrypt.h>

void encrypt(char* fileName, char* keyName)
{

}

void decrypt(char* fileName, char* keyName)
{

}

int main(int argc, char **argv)
{
	if (argc < 4)
	{
		fprintf(stderr, "Too little arguments!\nUsage: %s -e/-d FileName KeyName", argv[0]);
		return 1;
	}
	if (argc > 4)
	{
		fprintf(stderr, "Too many arguments!\nUsage: %s -e/-d FileName KeyName", argv[0]);
		return 1;
	}
	if (argv[1][0] != '-')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -e/-d FileName KeyName", argv[1], argv[0]);
		return 1;
	}
	if (argv[1][1] != 'e' && argv[1][1] != 'd')
	{
		fprintf(stderr, "Invalid flag: %s\nUsage: %s -e/-d FileName KeyName", argv[1], argv[0]);
		return 1;
	}
	if (argv[1][1] == 'e')
		encrypt(argv[2], argv[3]);
	else
		decrypt(argv[2], argv[3]);
	return 0;
}

