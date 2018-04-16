#include "internal_utils.h"
#include "../include/crema/iniutils.h"
#include "../include/crema/inireader.h"
#include <vector>
#include <stdarg.h>

namespace CremaCode { namespace reader { namespace internal
{
	std::string string_resource::invalid_type("잘못된 타입입니다.");

    std::string empty_string = "";

    internal_util::internal_util()
    {

    }

    internal_util::~internal_util()
    {
        std::list<CremaReader*> readers;
        readers.assign(m_readers.begin(), m_readers.end());

		for(std::list<CremaReader*>::iterator itor = readers.begin() ; itor != readers.end() ; itor++)
        {
            (*itor)->destroy();
        }
    }

    void string_resource::read(std::istream& stream)
    {
        int stringCount;
        stream.read((char*)&stringCount, sizeof(int));
		for (int i=0 ; i<stringCount ; i++)
		{
			int length, id;
			stream.read((char*)&id, sizeof(int));
			stream.read((char*)&length, sizeof(int));

            if (m_strings.find(id) == m_strings.end())
            {
                std::string text;
                if (length != 0)
                {
                    std::vector<char> buffer(length+1, 0);
                    stream.read(&buffer.front(), length);
                    text = iniutil::utf8_to_string(&buffer.front());
                }

                m_strings[id] = text;
            }
            else
            {
#ifdef _DEBUG
                std::streamoff off = stream.tellg();
#endif
                stream.seekg(length, std::ios::cur);
            }
		}
    }

    const std::string& string_resource::get(int id)
    {
		if (id == 0)
			return empty_string;
        std::map<int, std::string>::const_iterator itor = m_strings.find(id);
        return itor->second;
    }

    void string_resource::add_ref()
    {
        m_ref++;
    }
     
    void string_resource::remove_ref()
    {
        m_ref--;

        if (m_ref == 0)
            m_strings.clear();
    }
} /*namespace internal*/ } /*namespace CremaCode*/ } /*namespace reader*/

