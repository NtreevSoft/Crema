#pragma once
#include "inidata.h"
#include "initype.h"
#include <vector>

namespace CremaCode { namespace reader
{
	class DLL_EXPORT idataset abstract
	{
	public:
		virtual const itable_array& tables() const = 0;
		virtual const std::string& name() const = 0;
		virtual int revision() const = 0;
		virtual const std::string& types_hash_value() const = 0;
		virtual const std::string& tables_hash_value() const = 0;
		virtual const std::string& tags() const = 0;
	};

	class DLL_EXPORT CremaReader abstract : public idataset
	{
	public:
#ifndef _IGNORE_BOOST
		//static CremaReader& read(const std::string& address, int port, const std::string& name = "default", ReadFlag flag = ReadFlag_none);
		static CremaReader& read(const std::string& address, int port, const std::string& database, DataLocation datalocation, ReadFlag flag = ReadFlag_none);
#endif
		static CremaReader& read(const std::string& filename, ReadFlag flag = ReadFlag_none);
		static CremaReader& read(std::istream& stream, ReadFlag flag = ReadFlag_none);

		static CremaReader& ReadFromFile(const std::string& filename, ReadFlag flag = ReadFlag_none)
		{
			return read(filename, flag);
		}

		virtual void destroy() = 0;

		//virtual const itable_array& tables() const = 0;

	protected:
		CremaReader();
		virtual ~CremaReader();
		CremaReader& operator=(const CremaReader&) { return *this; }
		virtual void read_core(std::istream& stream, ReadFlag flag) = 0;

	private:
		std::istream* m_stream;
	};
} /*namespace CremaCode*/ } /*namespace reader*/

