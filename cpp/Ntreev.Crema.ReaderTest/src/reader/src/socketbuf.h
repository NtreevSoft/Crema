#pragma once

#include "../include/crema/inidefine.h"
#ifndef _IGNORE_BOOST
#include <iostream>
#include <algorithm>
#include <boost/asio.hpp>
#include <boost/array.hpp>

namespace CremaReader { namespace internal
{
    class socketbuf : public std::streambuf
    {
    public:
        socketbuf(const std::string& address, int port, const std::string& name);
        virtual ~socketbuf();

    protected:
        enum header_type
        {
            header_type_identify,
            header_type_size,
            header_type_buffer,
            header_type_compare,
        };

        struct buffer_info
        {
            off_type pos;
            size_t size;
        };

        std::streamsize xsgetn(char * _Ptr, std::streamsize _Count);
        std::streambuf::int_type underflow();
        pos_type seekoff(off_type o, std::ios_base::seekdir dir, std::ios_base::openmode = std::ios_base::in | std::ios_base::out);
        pos_type seekpos(pos_type p, std::ios_base::openmode = std::ios_base::in | std::ios_base::out);

    private:
        size_t soket_read(char* dest, off_type pos, size_t size);
        void write_string(const std::string& text);

        char* read_buffer(off_type pos);
        void set_buffer(off_type pos);

        template<typename T>
        void write_value(const T& data)
        {
            boost::system::error_code error;
            m_socket->write_some(boost::asio::buffer(&data, sizeof(T)), error);
        }

    private:
        static const size_t buffer_size = 1024 * 256;

        pos_type m_p;
        std::streamsize m_count;
        //pos_type m_pos;
        off_type m_len;

        boost::asio::io_service m_io_service;
        boost::asio::ip::tcp::socket* m_socket;
        const std::size_t m_putBack;
        //std::vector<char> m_buffer;
        boost::array<char, buffer_size> m_buffer;
        std::string m_name;
        std::vector<char*> m_buffers;

        
    };
} /*namespace internal*/ }

#endif

