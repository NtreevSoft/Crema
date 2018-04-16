#include "../include/crema/iniutils.h"
#include "internal_utils.h"
#include <time.h>
#include <iostream>
#include <string>
#include <sstream>
#include <algorithm>
#include <iterator>
#include <stdlib.h>
#include <stdarg.h>
#include <locale>
#include <codecvt>

#ifndef _IGNORE_BOOST
#include <boost/locale.hpp>
#endif

namespace CremaCode { namespace reader
{
	using namespace internal;
	class enum_data
	{
	public:
		static const enum_info* get_enum_info(const std::string& name)
		{
			return (*enum_data::enum_infos.find(name)).second;
		}

	public:
		static std::map<std::string, const enum_info*> enum_infos;
	};

	std::map<std::string, const enum_info*> enum_data::enum_infos;

	enum_info::enum_info(bool isflag)
		: m_isflag(isflag)
	{
		m_values = new std::map<std::string, int>();
		m_names = new std::map<int, std::string>();
	}

	enum_info::~enum_info()
	{
		delete m_values;
		delete m_names;
	}

	void enum_info::add(const std::string& name, int value)
	{
		m_values->insert(std::map<std::string, int>::value_type(name, value));
		m_names->insert(std::map<int, std::string>::value_type(value, name));
	}

	int enum_info::value(const std::string& name) const
	{
		return (*m_values->find(name)).second;
	}

	std::string enum_info::name(int value) const
	{
		std::map<int, std::string>::const_iterator itor = m_names->find(value);
		if(itor == m_names->end())
			return string_resource::empty_string;
		return itor->second;
	}

	std::vector<std::string> enum_info::names() const
	{
		std::vector<std::string> n;
		n.reserve(m_values->size());

		for(std::map<std::string, int>::const_iterator itor = m_values->begin() ; itor != m_values->end() ; itor++)
		{
			n.push_back(itor->first);
		}

		return n;
	}

	int enum_info::parse(const std::string& text) const
	{
		if (m_isflag == false)
			return this->value(text);

		int value = 0;
		std::istringstream iss(text);
		std::string buffer;
		while (std::getline(iss, buffer, ' '))
		{
			value |= this->value(buffer);
		}
		return value;
	}

	std::string enum_info::to_string(int value) const
	{
		std::map<int, std::string>::const_iterator itor = m_names->find(value);

		if(itor != m_names->end())
			return itor->second;

		std::stringstream ss;

		if(m_isflag == false)
		{
			ss << value;
			return ss.str();
		}

		int value2 = 0;
		std::vector<std::string> items;
		items.reserve(m_names->size());
		for(std::map<int, std::string>::iterator itor = m_names->begin() ; itor != m_names->end() ; itor++)
		{
			int mask = itor->first;
			if(value != 0 && mask == 0)
				continue;

			if((value & mask) == mask)
			{
				items.push_back(itor->second);
				value2 |= itor->first;
			}
		}

		if(value2 == value)
		{
			for(size_t i=0 ; i<items.size() ; i++)
			{
				if(i != 0)
				{
					ss << " ";
				}
				ss << items[i];
			}
		}
		else
		{
			ss << value;
		}

		return ss.str();
	}

	bool enum_info::is_flag() const
	{
		return m_isflag;
	}

	int enum_util::parse(const std::type_info& typeinfo, const std::string& text)
	{
		const enum_info* enum_info = enum_data::get_enum_info(typeinfo.name());
		if(enum_info == nullptr)
			throw std::invalid_argument(string_resource::invalid_type);
		return enum_info->parse(text);
	}

	void enum_util::add(const std::type_info& typeinfo, const enum_info* penum_info)
	{
		enum_data::enum_infos.insert(std::make_pair(typeinfo.name(), penum_info));
	}

	bool enum_util::contains(const std::type_info& typeinfo)
	{
		return enum_data::enum_infos.find(typeinfo.name()) != enum_data::enum_infos.end();
	}

	std::string enum_util::name(const std::type_info& typeinfo, int value)
	{
		const enum_info* enum_info = enum_data::get_enum_info(typeinfo.name());
		if(enum_info == nullptr)
			throw std::invalid_argument(string_resource::invalid_type);
		return enum_info->name(value);
	}

	std::vector<std::string> enum_util::names(const std::type_info& typeinfo)
	{
		const enum_info* enum_info = enum_data::get_enum_info(typeinfo.name());
		if(enum_info == nullptr)
			throw std::invalid_argument(string_resource::invalid_type);
		return enum_info->names();
	}

	std::string enum_util::to_string(const std::type_info& typeinfo, int value)
	{
		const enum_info* enum_info = enum_data::get_enum_info(typeinfo.name());
		if(enum_info == nullptr)
			throw std::invalid_argument(string_resource::invalid_type);
		return enum_info->to_string(value);
	}

	bool enum_util::is_flag(const std::type_info& typeinfo)
	{
		const enum_info* enum_info = enum_data::get_enum_info(typeinfo.name());
		if(enum_info == nullptr)
			throw std::invalid_argument(string_resource::invalid_type);
		return enum_info->is_flag();
	}

	short convert::to_int16(const std::string& text)
	{
		return (short)to_int32(text);
	}

	int convert::to_int32(const std::string& text)
	{
		return atoi(text.c_str());
	}

	long long convert::to_int64(const std::string& text)
	{
#ifdef _MSC_VER
		return _atoi64(text.c_str());
#else
		return atoll(text.c_str());
#endif
	}

	unsigned int convert::to_uint32(const std::string& text)
	{
		return atoi(text.c_str());
	}

	float convert::to_float(const std::string& text)
	{
		return (float)atof(text.c_str());
	}

	bool convert::to_boolean(const std::string& text)
	{
		if (text == "true")
			return true;
		return false;
	}

#ifndef _IGNORE_BOOST
	std::string iniutil::utf8_to_string(const char* text)
	{
		return boost::locale::conv::from_utf<char>(text, "euc-kr");
	}
#else
	std::string iniutil::utf8_to_string(const char* text)
	{
		return text;
	}
#endif

	const std::type_info& iniutil::name_to_type(const std::string& typeName)
	{
		if (typeName.compare("boolean") == 0)
			return typeid(bool);
		else if (typeName.compare("byte") == 0)
			return typeid(char);
		else if (typeName.compare("unsignedByte") == 0)
			return typeid(unsigned char);
		else if (typeName.compare("short") == 0)
			return typeid(short);
		else if (typeName.compare("unsignedShort") == 0)
			return typeid(unsigned short);
		else if (typeName.compare("int") == 0)
			return typeid(int);
		else if (typeName.compare("unsignedInt") == 0)
			return typeid(unsigned int);
		else if (typeName.compare("long") == 0)
			return typeid(long long);
		else if (typeName.compare("unsignedLong") == 0)
			return typeid(unsigned long long);
		else if (typeName.compare("float") == 0)
			return typeid(float);
		else if (typeName.compare("double") == 0)
			return typeid(double);
		else if (typeName.compare("dateTime") == 0)
			return typeid(long long);
		else if (typeName.compare("duration") == 0)
			return typeid(int);
		else if (typeName.compare("dictionary") == 0)
			return typeid(std::string);
		else if (typeName.compare("string") == 0)
			return typeid(std::string);

		return typeid(int);
	}

	std::wstring iniutil::string_to_wstring(const std::string& text)
	{
		if (text.length() == 0)
			return L"";
		setlocale(LC_ALL, "");
		size_t len;
#ifdef _MSC_VER
        mbstowcs_s(&len, NULL, NULL, text.c_str(), text.length());
        std::vector<wchar_t> buffer(len+1);
        buffer[len] = 0;
        mbstowcs_s(&len, &buffer.front(), len, text.c_str(), text.length());
#else
        len = mbstowcs(NULL, text.c_str(), text.length());
        std::vector<wchar_t> buffer(len+1);
        buffer[len] = 0;
        mbstowcs(&buffer.front(), text.c_str(), text.length());
#endif


		std::wstring retVal(&buffer.front());
		return retVal;
	}

	std::string iniutil::to_lower(const std::string& text)
	{
		std::string data = text; 
		std::transform(data.begin(), data.end(), data.begin(), ::tolower);
		return data;
	}

	//unsigned long hash(const std::string& str)
	//{
	//	unsigned long hash = 5381;
	//	for (size_t i = 0; i < str.size(); ++i)
	//		hash = 33 * hash + (unsigned char)str[i];
	//	return hash;
	//}

	int iniutil::get_hash_code(const std::string& text)
	{
		std::wstring wtext = string_to_wstring(text);
		const wchar_t* chPtr = wtext.c_str();

		int num = 0x15051505;
		int num2 = num;
		int* numPtr = ( int* ) chPtr;
		for ( int i = (int)wtext.length(); i > 0; i -= 4 )
		{
			num = ( ( ( num << 5 ) + num ) + ( num >> 0x1b ) ) ^ numPtr[ 0 ];
			if ( i <= 2 )
				break;
			num2 = ( ( ( num2 << 5 ) + num2 ) + ( num2 >> 0x1b) ) ^ numPtr[ 1 ];
			numPtr += 2;
		}
		return num + ( num2 * 0x5d588b65 );
	}

	long iniutil::generate_hash_core(size_t count, size_t keysize ...)
	{
		va_list vl;
		
		va_start(vl, keysize);
		size_t offset = 0;
		std::vector<char> fields(keysize);
		const std::collate<char>& coll = std::use_facet< std::collate<char> >(std::locale());

		for (size_t i=0 ; i<count ; i++)
		{
			const std::type_info& typeinfo = *va_arg(vl, const std::type_info*);
#ifdef _DEBUG
			const char* name = typeinfo.name();
#endif

			if (typeinfo == typeid(bool))
			{
				internal_util::set_field_value(&fields.front(), offset, !!va_arg(vl, int));
			}
			else if (typeinfo == typeid(char))
			{
				internal_util::set_field_value(&fields.front(), offset, (char)va_arg(vl, int));
			}
			else if (typeinfo == typeid(unsigned char))
			{
				internal_util::set_field_value(&fields.front(), offset, (unsigned char)va_arg(vl, int));
			}
			else if (typeinfo == typeid(short))
			{
				internal_util::set_field_value(&fields.front(), offset, (short)va_arg(vl, int));
			}
			else if (typeinfo == typeid(unsigned short))
			{
				internal_util::set_field_value(&fields.front(), offset, (unsigned short)va_arg(vl, int));
			}
			else if (typeinfo == typeid(int))
			{
				internal_util::set_field_value(&fields.front(), offset, (int)va_arg(vl, int));
			}
			else if (typeinfo == typeid(unsigned int))
			{
				internal_util::set_field_value(&fields.front(), offset, (unsigned int)va_arg(vl, int));
			}
			else if (typeinfo == typeid(float))
			{
				internal_util::set_field_value(&fields.front(), offset, (float)va_arg(vl, double));
			}
			else if (typeinfo == typeid(double))
			{
				internal_util::set_field_value(&fields.front(), offset, (double)va_arg(vl, double));
			}
			else if (typeinfo == typeid(long long))
			{
				internal_util::set_field_value(&fields.front(), offset, (long long)va_arg(vl, long long));
			}
			else if (typeinfo == typeid(unsigned long long))
			{
				internal_util::set_field_value(&fields.front(), offset, (unsigned long long)va_arg(vl, long long));
			}
			else if (typeinfo == typeid(char*) || typeinfo == typeid(const char*))
			{
				int stringID = iniutil::get_hash_code(va_arg(vl, const char*));
				internal_util::set_field_value(&fields.front(), offset, stringID);
			}
			else if (typeinfo == typeid(std::string))
			{
				std::string text = (std::string)va_arg(vl, std::string);
				int stringID = iniutil::get_hash_code(text);
				internal_util::set_field_value(&fields.front(), offset, stringID);
			}
			else
			{
				internal_util::set_field_value(&fields.front(), offset, (int)va_arg(vl, int));
			}
		}
		va_end(vl);

		long hash = coll.hash(&fields.front(), &fields.front() + fields.size());

		return hash;
	}

	size_t iniutil::keys_size(size_t count, ...)
	{
		va_list vl;
		va_start(vl, count);
		size_t size = 0;

        for (size_t i=0 ; i<count ; i++)
        {
            const std::type_info& typeinfo = *va_arg(vl, const std::type_info*);
            if (typeinfo == typeid(bool))
            {
                size += sizeof(int);
            }
			else if (typeinfo == typeid(char))
            {
                size += sizeof(int);
            }
			else if (typeinfo == typeid(unsigned char))
            {
                size += sizeof(int);
            }
            else if (typeinfo == typeid(short))
            {
                size += sizeof(int);
            }
			else if (typeinfo == typeid(unsigned short))
            {
                size += sizeof(int);
            }
			else if (typeinfo == typeid(int))
            {
                size += sizeof(int);
            }
			else if (typeinfo == typeid(unsigned int))
            {
                size += sizeof(int);
            }
            else if (typeinfo == typeid(float))
            {
				size += sizeof(float);
            }
			else if (typeinfo == typeid(double))
			{
				size += sizeof(double);
			}
            else if (typeinfo == typeid(long long))
            {
				size += sizeof(long long);
            }
			else if (typeinfo == typeid(unsigned long long))
            {
                size += sizeof(long long);
            }
			else if (typeinfo == typeid(std::string))
            {
                size += sizeof(int);
            }
			else
			{
				size += sizeof(int);
			}
        }
		return size;
	}
} /*namespace CremaCode*/ } /*namespace reader*/

