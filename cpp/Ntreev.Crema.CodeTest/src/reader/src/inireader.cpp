#include "../include/crema/inireader.h"
#include "../include/crema/iniutils.h"
#include "binary_reader.h"
#include "internal_utils.h"
#include <iostream>
#include <fstream>
#include <time.h>
#include "socket_istream.h"


namespace CremaCode { namespace reader
{
    using namespace internal;
    using namespace internal::binary;

    const unsigned int s_magic_value_obsolete = 0x8d31269e;
	const int s_magic_value = 0x03050000;

    std::map<int, std::string> string_resource::m_strings;
    std::string string_resource::empty_string;
    int string_resource::m_ref = 0;
    internal_util static_data;

    CremaReader::CremaReader()
        : m_stream(nullptr)
    {
        string_resource::add_ref();
        static_data.m_readers.push_back(this);
    }

    CremaReader::~CremaReader()
    {
        static_data.m_readers.remove(this);
        string_resource::remove_ref();
        if (m_stream != nullptr)
        {
            delete m_stream;
            m_stream = nullptr;
        }
    }

#ifndef _IGNORE_BOOST
    CremaReader& CremaReader::read(const std::string& address, int port, const std::string& name, ReadFlag flag)
    {
        socket_istream* stream = new socket_istream(address, port+1, name);
        CremaReader& reader = CremaReader::read(*stream, flag);
        reader.m_stream = stream;
        return reader;
    }

	CremaReader& CremaReader::read(const std::string& address, int port, const std::string& database, DataLocation datalocation, ReadFlag flag)
	{
		std::string dl;
		switch (datalocation)
		{
		case DataLocation_both:
			dl = "Both";
			break;
		case DataLocation_server:
			dl = "Server";
			break;
		case DataLocation_client:
			dl = "Client";
			break;
		default:
			break;
		}
		std::ostringstream ss;
		ss << "type=bin;data=" << dl << ";database=" << database << ";";
		return read(address, port, ss.str(), flag);
	}
#endif

    CremaReader& CremaReader::read(std::istream& stream, ReadFlag flag)
    {
        std::ifstream* fstream = dynamic_cast<std::ifstream*>(&stream);
        if (fstream != nullptr && fstream->is_open() == false)
            throw std::invalid_argument("파일이 열리지 않았습니다.");

        unsigned int magicValue;
        stream.read((char*)&magicValue, sizeof(int));

        if (magicValue == s_magic_value || magicValue == s_magic_value_obsolete)
        {
            binary_reader* reader = new binary_reader();
            reader->read_core(stream, flag);
            return *reader;
        }

        throw std::exception();

        //libxml2_reader* reader = new libxml2_reader();
        //reader->read_core(fstream, flag);
        //return *reader;

    }

    CremaReader& CremaReader::read(const std::string& filename, ReadFlag flag)
    {
#ifdef _MSC_VER
        std::ifstream* stream = new std::ifstream(filename, std::ios::binary);
#else
		std::ifstream* stream = new std::ifstream(filename.c_str(), std::ios::binary);
#endif
        CremaReader& reader = CremaReader::read(*stream, flag);
        reader.m_stream = stream;
        return reader;
    }
} /*namespace CremaCode*/ } /*namespace reader*/

