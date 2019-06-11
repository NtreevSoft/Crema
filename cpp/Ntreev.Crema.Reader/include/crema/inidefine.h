#pragma once

#define _CREMA_READER_VER 209
//#define _IGNORE_BOOST

#ifdef _MSC_VER
#ifdef _WINDLL
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT 
#endif

#if _MSC_VER < 1700
#define nullptr NULL
#endif
#else
#define DLL_EXPORT 
#define abstract
#define nullptr NULL
#define _IGNORE_BOOST

#include <string>
namespace std
{
	typedef basic_string<wchar_t, char_traits<wchar_t>, allocator<wchar_t> > wstring;
}
#endif

#ifndef WINVER
#define WINVER NTDDI_VISTA
#endif

#ifndef _WIN32_WINNT
#define _WIN32_WINNT NTDDI_VISTA
#endif

#ifndef BOOST_USE_WINAPI_VERSION
#define BOOST_USE_WINAPI_VERSION NTDDI_VISTA 
#endif


