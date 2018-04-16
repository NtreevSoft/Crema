#include "stdafx.h"
#include <typeinfo>
#include <iostream>
#include <direct.h>
#include <stdlib.h>
#include <crema/inireader.h>
#include <memory>
#include <locale>
#include <codecvt>
#include <stdarg.h>

int _tmain(int argc, _TCHAR* argv[])
{
	CremaReader::CremaReader& reader = CremaReader::CremaReader::ReadFromFile("..\\..\\crema.dat");
	auto s = reader.tables().size();
	return 0;
}

