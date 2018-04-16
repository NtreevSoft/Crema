#pragma once
#include "inidefine.h"
#include <vector>
#include <map>
#include <string>
#include <typeinfo>
#include <stdexcept>

namespace CremaCode { namespace reader
{
    class DLL_EXPORT enum_info
    {
    public:
        enum_info(bool isFlag);
        ~enum_info();

        void add(const std::string& name, int value);
        int value(const std::string& name) const;
        std::string name(int value) const;
		std::vector<std::string> names() const;
		int parse(const std::string& text) const;
		std::string to_string(int value) const;
        bool is_flag() const;

    private:
        std::map<std::string, int>* m_values;
        std::map<int, std::string>* m_names;
        bool m_isflag;
    };

    class DLL_EXPORT enum_util
    {
    public:
        static int parse(const std::type_info& typeinfo, const std::string& text);
        static void add(const std::type_info& typeinfo, const enum_info* penum_info);
        static bool contains(const std::type_info& typeinfo);
		static std::string name(const std::type_info& typeinfo, int value);
		static std::vector<std::string> names(const std::type_info& typeinfo);
		static std::string to_string(const std::type_info& typeinfo, int value);
		static bool is_flag(const std::type_info& typeinfo);

		template<typename T>
		static std::string name(T value)
		{
			return name(typeid(T), (int)value);
		}

		static std::string name(int /*value*/)
		{
			throw std::invalid_argument("타입을 지정하세요");
		}

		template<typename T>
		static std::vector<std::string> names()
		{
			return names(typeid(T));
		}

		template<typename T>
		static T parse(const std::string& text)
		{
			return (T)parse(typeid(T), text);
		}
    };

    class DLL_EXPORT convert
    {
    public:
        static short to_int16(const std::string& text);
        static int to_int32(const std::string& text);
        static long long to_int64(const std::string& text);
        static unsigned int to_uint32(const std::string& text);
        static float to_float(const std::string& text);
        static bool to_boolean(const std::string& text);
    };

    class DLL_EXPORT iniutil
    {
    public:
        static std::string utf8_to_string(const char* text);
        static std::wstring string_to_wstring(const std::string& text);
        static const std::type_info& name_to_type(const std::string& typeName);
        static int get_hash_code(const std::string& text);
        static std::string to_lower(const std::string& text);
		
		static int get_type_size(const std::type_info& typeinfo);

		template<typename key_type>
        static long generate_hash(key_type key_value) 
        {
			size_t size = keys_size(1, &typeid(key_type));
            return generate_hash_core(1, size, &typeid(key_type), key_value);
        }

        template<typename key_type1, typename key_type2>
        static long generate_hash(key_type1 key_value1, key_type2 key_value2) 
        {
			size_t size = keys_size(2, &typeid(key_type1), &typeid(key_type2));
            return generate_hash_core(2, size,
                &typeid(key_type1), key_value1, 
                &typeid(key_type2), key_value2);
        }

        template<typename key_type1, typename key_type2, typename key_type3>
        static long generate_hash(key_type1 key_value1, key_type2 key_value2, key_type3 key_value3) 
        {
			size_t size = keys_size(3, &typeid(key_type1), &typeid(key_type2), &typeid(key_type3));
            return generate_hash_core(3, size,
                &typeid(key_type1), key_value1, 
                &typeid(key_type2), key_value2,
                &typeid(key_type3), key_value3);
        }

        template<typename key_type1, typename key_type2, typename key_type3, typename key_type4>
        static long generate_hash(key_type1 key_value1, key_type2 key_value2, key_type3 key_value3, key_type4 key_value4) 
        {
			size_t size = keys_size(4, &typeid(key_type1), &typeid(key_type2), &typeid(key_type3), &typeid(key_type4));
            return generate_hash_core(4, size, 
                &typeid(key_type1), key_value1, 
                &typeid(key_type2), key_value2, 
                &typeid(key_type3), key_value3, 
                &typeid(key_type4), key_value4);
        }

		template<typename key_type1, typename key_type2, typename key_type3, typename key_type4, typename key_type5>
        static long generate_hash(key_type1 key_value1, key_type2 key_value2, key_type3 key_value3, key_type4 key_value4, key_type5 key_value5) 
        {
			size_t size = keys_size(5, &typeid(key_type1), &typeid(key_type2), &typeid(key_type3), &typeid(key_type4), &typeid(key_type5));
            return generate_hash_core(5, size,
                &typeid(key_type1), key_value1, 
                &typeid(key_type2), key_value2, 
                &typeid(key_type3), key_value3, 
                &typeid(key_type4), key_value4,
				&typeid(key_type5), key_value5);
        }

	private:
		static long generate_hash_core(size_t count, size_t keysize, ...);
		static size_t keys_size(size_t count, ...);

		template<typename _type>
        static void set_field_value(const char* buffer, size_t& offset, _type value)
        {
            _type* value_ptr = (_type*)(buffer + offset);
            *value_ptr = value;
            offset += sizeof(_type);
        }
    };
} /*namespace CremaCode*/ } /*namespace reader*/

