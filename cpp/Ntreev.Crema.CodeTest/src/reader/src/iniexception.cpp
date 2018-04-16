#include "../include/crema/iniexception.h"

namespace CremaCode { namespace reader
{
#ifdef _MSC_VER
    keynotfoundexception::keynotfoundexception(const std::string& key, const std::string& container_name)
        : exception(format_string(key, container_name).c_str())
    {

    }
#else
	keynotfoundexception::keynotfoundexception(const std::string& /*key*/, const std::string& /*container_name*/)
    {

    }
#endif

    std::string keynotfoundexception::format_string(const std::string& key, const std::string& container_name)
    {
        std::stringstream text;
        text << container_name << "에 " << key << "라는 항목은 존재하지 않습니다.";
        return text.str();
    }
} /*namespace CremaCode*/ } /*namespace reader*/

