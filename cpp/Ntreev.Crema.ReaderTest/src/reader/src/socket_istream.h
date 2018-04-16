#pragma once

#include "../include/crema/inidefine.h"
#ifndef _IGNORE_BOOST
#include "socketbuf.h"

namespace CremaReader { namespace internal
{
    class socket_istream : public std::istream
    {
    public:
        socket_istream(const std::string& address = "127.0.0.1", int port = 4004, const std::string& name = "default")
            : m_buf(address, port, name), std::istream(&m_buf)
        {

        }

    private:
        socketbuf m_buf;
    };
} /*namespace internal*/ }

#endif

