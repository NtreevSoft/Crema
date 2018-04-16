#pragma once
#include <istream>
#include <map>
#include <list>

namespace CremaReader
{
	class CremaReader;
} /*namespace CremaReader*/

namespace CremaReader {
	namespace internal
	{
		class internal_util
		{
		public:
			internal_util();
			~internal_util();

			template<typename _type>
			static void set_field_value(const char* buffer, size_t& offset, _type value)
			{
				_type* value_ptr = (_type*)(buffer + offset);
				*value_ptr = value;
				offset += sizeof(_type);
			}

			std::list<CremaReader*> m_readers;


		};

		class string_resource
		{
		public:
			static void read(std::istream& stream);
			static const std::string& get(int id);

			static void add_ref();
			static void remove_ref();

		public:
			static std::string empty_string;
			static std::string invalid_type;
		private:
			static std::map<int, std::string> m_strings;
			static int m_ref;
		};
	} /*namespace internal*/
} /*namespace CremaReader*/
