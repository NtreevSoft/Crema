#include "socketbuf.h"
#include <algorithm>

#ifndef _IGNORE_BOOST

namespace CremaReader { namespace internal
{
    using namespace boost::asio::ip;

    socketbuf::socketbuf(const std::string& address, int port, const std::string& name)
        : m_putBack(8), m_name(name)
    {
        std::string port_string;
        std::stringstream out;
        out << port;
        port_string = out.str();

        tcp::resolver resolver(m_io_service);
        tcp::resolver::query query(address, port_string);
        tcp::resolver::iterator endpoint_iterator = resolver.resolve(query);
        tcp::resolver::iterator end;

        m_socket = new tcp::socket(m_io_service);
        boost::system::error_code error = boost::asio::error::host_not_found;

        while (error && endpoint_iterator != end)
        {
            m_socket->close();
            m_socket->connect(*endpoint_iterator++, error);
        }

        if (error)
            throw boost::system::system_error(error);

        header_type header = header_type_size;
        this->write_value(header);
        this->write_string(m_name);

        m_socket->read_some(boost::asio::buffer(&m_len, sizeof(off_type)), error);

        size_t count = (size_t)ceilf((float)m_len / buffer_size);
        m_buffers.assign(count, NULL);
    }

    socketbuf::~socketbuf()
    {
        for(std::vector<char*>::iterator itor = m_buffers.begin() ; itor != m_buffers.end() ; itor++)
        {
            char* buffer = *itor;
            if(buffer != NULL)
                delete [] buffer;
        }
        delete m_socket;
    }

    char* socketbuf::read_buffer(off_type pos)
    {
        size_t index = (size_t)(pos / buffer_size);

        char* buffer = m_buffers[index];
        if(buffer == NULL)
        {
            size_t bfs = buffer_size;
            if((index+1) * buffer_size > (size_t)m_len)
                bfs = m_len % buffer_size;
            buffer = new char[bfs];
            soket_read(buffer, index * buffer_size, bfs);
            m_buffers[index] = buffer;
        }
        return buffer;
    }

    size_t socketbuf::soket_read(char* dest, off_type pos, size_t size)
    {
        boost::system::error_code error;
        header_type header = header_type_buffer;

        buffer_info bufferinfo;
        bufferinfo.pos = pos;
        bufferinfo.size = size;

        this->write_value(header);
        this->write_value(bufferinfo);

        //size_t len = m_socket->read_some(boost::asio::buffer(dest, size), error);
        //return len;
        size_t read = 0;

        for (;;)
        {
            boost::system::error_code error;

            size_t len = m_socket->read_some(boost::asio::buffer(m_buffer), error);

            if (error == boost::asio::error::eof)
                break; // Connection closed cleanly by peer.
            else if (error)
                throw boost::system::system_error(error);

            std::memcpy(dest + read, m_buffer.data(), len);
            read += len;

            if(read == size)
                break;
        }

        return read;
    }

    std::streamsize socketbuf::xsgetn(char * _Ptr, std::streamsize _Count)
    {
        m_count = _Count;
        std::streamsize size = std::streambuf::xsgetn(_Ptr, _Count);
        m_p += size;
        return size;
    }

    std::streambuf::int_type socketbuf::underflow()
    {
        if (gptr() < egptr())
            return traits_type::to_int_type(*gptr());

        char* end = egptr();
        char* next = gptr();

        pos_type si = m_p;
        if(next == end)
            si = (m_p + m_count) / buffer_size * buffer_size;

        set_buffer(si);

        return traits_type::to_int_type(*gptr());
    }

    void socketbuf::set_buffer(off_type pos)
    {
        char* buffer = read_buffer(pos);
        size_t offset = pos % buffer_size;
        size_t index = (size_t)(pos / buffer_size);
        size_t bfs = buffer_size;
        if((index+1) * buffer_size > (size_t)m_len)
            bfs = m_len % buffer_size;

        setg(buffer, buffer + offset, buffer + bfs);
    }

    socketbuf::pos_type socketbuf::seekoff(off_type o, std::ios_base::seekdir dir, std::ios_base::openmode)
    {
        switch(dir)
        {
        case std::ios_base::beg:
            m_p = o;
            break;
        case std::ios_base::end:
            m_p = m_len - o;
            break;
        default:
            m_p = m_p + o;
            break;
        }

        set_buffer(m_p);

        return pos_type(m_p);
    }

    socketbuf::pos_type socketbuf::seekpos(pos_type p, std::ios_base::openmode mode)
    {
        return seekoff(p, std::ios_base::beg, mode);
    }

    void socketbuf::write_string(const std::string& /*text*/)
    {
        boost::system::error_code error;
        size_t len = m_name.length();
        unsigned char byte;
        while (len >= 0x80)
        {
            byte = (unsigned char)(len | 0x80);
            m_socket->write_some(boost::asio::buffer(&byte, sizeof(unsigned char)), error);
            len = len >> 7;
        }
        byte = (unsigned char)len;
        m_socket->write_some(boost::asio::buffer(&byte, sizeof(unsigned char)), error);

        m_socket->write_some(boost::asio::buffer(m_name, m_name.length()), error);
    }
} /*namespace internal*/ }

#endif

