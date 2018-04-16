#pragma once
#include "inidefine.h"
#include <string>
#include <sstream>
#include <typeinfo>
#include <stdexcept>

namespace CremaCode { namespace reader
{
	class DLL_EXPORT keynotfoundexception : public std::exception
    {
    public:
        keynotfoundexception(const std::string& key, const std::string& container_name);

    private:
        std::string format_string(const std::string& key, const std::string& container_name);

	private:
    };
} /*namespace CremaCode*/ } /*namespace reader*/

