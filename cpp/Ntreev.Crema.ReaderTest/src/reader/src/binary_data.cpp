#include "binary_data.h"
#include "binary_reader.h"
#include "../include/crema/iniexception.h"
#include "internal_utils.h"
#include "../include/crema/iniutils.h"
#include <stdarg.h>
#include <locale>

namespace CremaReader { namespace internal { namespace binary
{
    binary_column::binary_column(const std::string& columnName, const std::type_info& dataType, bool isKey)
        : m_columnName(columnName), m_dataType(dataType), m_iskey(isKey)
    {
#ifdef _DEBUG
        this->typeName = m_dataType.name();
#endif
    }

    binary_column::~binary_column()
    {

    }

    const std::string& binary_column::name() const
    {
        return m_columnName;
    }

    const std::type_info& binary_column::datatype() const
    {
        return m_dataType;
    }

    bool binary_column::is_key() const
    {
        return m_iskey;
    }

    size_t binary_column::index() const
    {
        return m_columnIndex;
    }

    void binary_column::set_index(size_t index)
    {
        m_columnIndex = index;
    }

    itable& binary_column::table() const
    {
        return *m_table;
    }

    void binary_column::set_table(itable& table)
    {
        m_table = &table;
    }

	binary_row::binary_row()
	{

	}
	
	binary_row::~binary_row()
	{

	}

    const void* binary_row::value_core(const inicolumn& column) const
    {
        static long long nullvalue = 0;
        const int* offsets = (const int*)&m_fields.front();
        int offset = offsets[column.index()];

        const char* valuePtr = &m_fields.front() + offset;
        const std::type_info& typeinfo = column.datatype();

        if (typeinfo == typeid(std::string))
        { 
            if (offset == 0)
                return &string_resource::empty_string;
            int id = *(int*)valuePtr;
            return &string_resource::get(id);
        }
        else 
        {
            if (offset == 0)
                return &nullvalue;
            return valuePtr;
        }
    }

	bool binary_row::has_value_core(const inicolumn& column) const
	{
        const int* offsets = (const int*)&m_fields.front();
        int offset = offsets[column.index()];
		return offset != 0;
	}

    void binary_row::set_value(const std::string& /*columnName*/, const std::string& /*text*/)
    {
        throw std::invalid_argument("지원되지 않습니다");
    }

    itable& binary_row::table() const
    {
        return *m_table;
    }

	long binary_row::hash() const
	{
		return m_hash;
	}

    void binary_row::reserve_fields_ptr(size_t size)
    {
        m_fields.resize(size, 0);
    }
     
    char* binary_row::fields_ptr()
    {
        return &m_fields.front();
    }

    void binary_row::set_table(binary_table& table)
    {
        m_table = &table;
    }

	void binary_row::set_hash(long hash)
	{
		m_hash = hash;
	}

	bool binary_row::equals_key(va_list& vl)
	{
#if _MSC_VER >= 1700
        for (inicolumn& item : m_table->m_keys)
        {
#else
        for (binary_key_array::iterator itor = m_table->keys.begin() ; itor != m_table->keys.end() ; itor++)
        {
            inicolumn& item = *itor;
#endif
            const std::type_info& typeinfo = *va_arg(vl, const std::type_info*);
			if (typeinfo == typeid(bool))
            {
				if(this->value<bool>(item.name()) != !!va_arg(vl, int))
					return false;
            }
			else if (typeinfo == typeid(char))
            {
                if(this->value<char>(item.name()) != (char)va_arg(vl, int))
					return false;
            }
			else if (typeinfo == typeid(unsigned char))
            {
                if(this->value<unsigned char>(item.name()) != (unsigned char)va_arg(vl, int))
					return false;
            }
            else if (typeinfo == typeid(short))
            {
                if(this->value<short>(item.name()) != (short)va_arg(vl, int))
					return false;
            }
			else if (typeinfo == typeid(unsigned short))
            {
                if(this->value<unsigned short>(item.name()) != (unsigned short)va_arg(vl, int))
					return false;
            }
			else if (typeinfo == typeid(int))
            {
                if(this->value<int>(item.name()) != (int)va_arg(vl, int))
					return false;
            }
			else if (typeinfo == typeid(unsigned int))
            {
                if(this->value<unsigned int>(item.name()) != (unsigned int)va_arg(vl, int))
					return false;
            }
            else if (typeinfo == typeid(float))
            {
				if(this->value<float>(item.name()) != (float)va_arg(vl, double))
					return false;
            }
			else if (typeinfo == typeid(double))
			{
				if (this->value<double>(item.name()) != (float)va_arg(vl, double))
					return false;
			}
            else if (typeinfo == typeid(long long))
            {
                if(this->value<long long>(item.name()) != (long long)va_arg(vl, long long))
					return false;
            }
			else if (typeinfo == typeid(unsigned long long))
            {
                if(this->value<unsigned long long>(item.name()) != (unsigned long long)va_arg(vl, long long))
					return false;
            }
            else if (typeinfo == typeid(char*) || typeinfo == typeid(const char*))
            {
				std::string text = va_arg(vl, const char*);
				if(this->value<std::string>(item.name()) != text)
					return false;
            }
			else if (typeinfo == typeid(std::string))
			{
				std::string text = (std::string)va_arg(vl, std::string);
				if (this->value<std::string>(item.name()) != text)
					return false;
			}
        }
		return true;
	}

    binary_key_array::binary_key_array()
    {

    }

	binary_key_array::~binary_key_array()
    {

    }

    size_t binary_key_array::size() const
    {
        return m_columns.size();
    }
     
    inicolumn& binary_key_array::at(size_t index) const
    {
        return *m_columns[index];
    }
 
    void binary_key_array::add(binary_column* column)
    {
        m_columns.push_back(column);
    }

    binary_column_array::binary_column_array(size_t count)
        : m_columns(count), m_caseSensitive(false)
    {
		
    }

    binary_column_array::~binary_column_array()
    {
#if _MSC_VER >= 1700
        for (binary_column* item : m_columns)
        {
#else
        for (std::vector<binary_column*>::iterator itor = m_columns().begin() ; itor != m_columns().end() ; itor++)
        {
            binary_column* item = *itor;
#endif
            delete item;
        }
    }

    size_t binary_column_array::size() const
    {
        return m_columns.size();
    }
     
    inicolumn& binary_column_array::at(size_t index) const
    {
        return *m_columns[index];
    }
 
    inicolumn& binary_column_array::at(const std::string& columnName) const
    {
        std::map<std::string, binary_column*>::const_iterator itor = m_nameToColumn.find(conv_string(columnName));
        if (itor == m_nameToColumn.end())
            throw keynotfoundexception(columnName, "columns");
        return *itor->second;
    }

    bool binary_column_array::contains(const std::string& columnName) const
    {
        return m_nameToColumn.find(conv_string(columnName)) != m_nameToColumn.end();
    }

    void binary_column_array::set(size_t index, binary_column* column)
    {
        m_columns[index] = column;
        m_nameToColumn[conv_string(column->name())] = column;
        column->set_index(index);
    }

    void binary_column_array::set_flag(ReadFlag flag)
    {
        m_caseSensitive = (flag & ReadFlag_case_sensitive) != 0;
    }

    std::string binary_column_array::conv_string(const std::string& text) const
    {
        if (m_caseSensitive == true)
            return text;
        return iniutil::to_lower(text);
    }

    binary_row_array::binary_row_array(size_t count)
        : m_rows(count)
    {
		m_keyTorow.get_allocator().allocate(count);
    }

	binary_row_array::~binary_row_array()
	{

	}

    size_t binary_row_array::size() const
    {
        return m_rows.size();
    }

    binary_row& binary_row_array::at(size_t index) const
    {
        const binary_row& row = m_rows[index];
        return const_cast<binary_row&>(row);
    }

    binary_row& binary_row_array::at(size_t index)
    {
        return m_rows[index];
    }

    void binary_row_array::generate_key(size_t index)
    {
        size_t keysize = this->keys_size(), offset = 0;

        if (keysize == 0)
            return;

        binary_row& row = this->at(index);
        const std::collate<char>& coll = std::use_facet< std::collate<char> >(std::locale());
        std::vector<char> fields(keysize);
#if _MSC_VER >= 1700
        for (inicolumn& item : m_table->m_keys)
        {
#else
        for (binary_key_array::iterator itor = m_table->keys.begin() ; itor != m_table->keys.end() ; itor++)
        {
            inicolumn& item = *itor;
#endif
            const std::type_info& typeinfo = item.datatype();
			if (typeinfo == typeid(bool))
            {
                this->set_field_value(&fields.front(), offset, row.value<bool>(item));
            }
			else if (typeinfo == typeid(char))
            {
                this->set_field_value(&fields.front(), offset, row.value<char>(item));
            }
            else if (typeinfo == typeid(unsigned char))
            {
                this->set_field_value(&fields.front(), offset, row.value<unsigned char>(item));
            }
			else if (typeinfo == typeid(short))
            {
                this->set_field_value(&fields.front(), offset, row.value<short>(item));
            }
            else if (typeinfo == typeid(unsigned short))
            {
                this->set_field_value(&fields.front(), offset, row.value<unsigned short>(item));
            }
			else if (typeinfo == typeid(int))
            {
                this->set_field_value(&fields.front(), offset, row.value<int>(item));
            }
            else if (typeinfo == typeid(unsigned int))
            {
                this->set_field_value(&fields.front(), offset, row.value<unsigned int>(item));
            }
            else if (typeinfo == typeid(long long))
            {
                this->set_field_value(&fields.front(), offset, row.value<long long>(item));
            }
            else if (typeinfo == typeid(unsigned long long))
            {
                this->set_field_value(&fields.front(), offset, row.value<unsigned long long>(item));
            }
			else if (typeinfo == typeid(float))
            {
                this->set_field_value(&fields.front(), offset, row.value<float>(item));
            }
            else if (typeinfo == typeid(std::string))
            {
                const std::string& text = *(const std::string*)row.value_core(item);
                this->set_field_value(&fields.front(), offset, iniutil::get_hash_code(text));
            }
        }

        long hash = coll.hash(&fields.front(), &fields.front() + fields.size());

		row.set_hash(hash);

		m_keyTorow.insert(std::multimap<long, binary_row*>::value_type(hash, &row));
    }

    void binary_row_array::set_table(binary_table& table)
    {
        m_table = &table;
    }

    binary_table& binary_row_array::table() const
    {
        return *m_table;
    }

    binary_row_array::iterator binary_row_array::find_core(size_t count, ...)
    {
        if (count != m_table->m_keys.size())
            throw std::invalid_argument("인자의 갯수가 키의 갯수랑 같지 않습니다.");
        va_list vl;
        size_t keysize = this->keys_size();
        va_start(vl, count);

        size_t offset = 0;
        std::vector<char> fields(keysize);
        const std::collate<char>& coll = std::use_facet< std::collate<char> >(std::locale());

        for (size_t i=0 ; i<count ; i++)
        {
            const std::type_info& typeinfo = *va_arg(vl, const std::type_info*);
			if (typeinfo == typeid(bool))
            {
                this->set_field_value(&fields.front(), offset, !!va_arg(vl, int));
            }
			else if (typeinfo == typeid(char))
            {
                this->set_field_value(&fields.front(), offset, (char)va_arg(vl, int));
            }
			else if (typeinfo == typeid(unsigned char))
            {
                this->set_field_value(&fields.front(), offset, (unsigned char)va_arg(vl, int));
            }
            else if (typeinfo == typeid(short))
            {
                this->set_field_value(&fields.front(), offset, (short)va_arg(vl, int));
            }
			else if (typeinfo == typeid(unsigned short))
            {
                this->set_field_value(&fields.front(), offset, (unsigned short)va_arg(vl, int));
            }
			else if (typeinfo == typeid(int))
            {
                this->set_field_value(&fields.front(), offset, (int)va_arg(vl, int));
            }
			else if (typeinfo == typeid(unsigned int))
            {
                this->set_field_value(&fields.front(), offset, (unsigned int)va_arg(vl, int));
            }
            else if (typeinfo == typeid(float))
            {
				this->set_field_value(&fields.front(), offset, (float)va_arg(vl, double));
            }
			else if (typeinfo == typeid(double))
			{
				this->set_field_value(&fields.front(), offset, (float)va_arg(vl, double));
			}
            else if (typeinfo == typeid(long long))
            {
                this->set_field_value(&fields.front(), offset, (long long)va_arg(vl, long long));
            }
			else if (typeinfo == typeid(unsigned long long))
            {
                this->set_field_value(&fields.front(), offset, (unsigned long long)va_arg(vl, long long));
            }
            else if (typeinfo == typeid(char*) || typeinfo == typeid(const char*))
            {
                int stringID = iniutil::get_hash_code(va_arg(vl, const char*));
                this->set_field_value(&fields.front(), offset, stringID);
            }
			else if (typeinfo == typeid(std::string))
			{
				std::string text = (std::string)va_arg(vl, std::string);
				int stringID = iniutil::get_hash_code(text);
				this->set_field_value(&fields.front(), offset, stringID);
			}
        }
        va_end(vl);

        long hash = coll.hash(&fields.front(), &fields.front() + fields.size());
		std::pair <std::multimap<long,binary_row*>::iterator, std::multimap<long,binary_row*>::iterator> ret = m_keyTorow.equal_range(hash);
		
		size_t len = std::distance(ret.first, ret.second);

		if(len == 1)
		{
			size_t index = ret.first->second - (binary_row*)&m_rows.front();
			return iterator(this, index);
		}

		for (std::multimap<long,binary_row*>::iterator itor=ret.first; itor!=ret.second; ++itor)
		{
			va_list vl1;
			va_start(vl1, count);
			if(itor->second->equals_key(vl1) == true)
			{
				size_t index = itor->second - (binary_row*)&m_rows.front();
				return iterator(this, index);
			}
			va_end(vl1);
		}

		return iterator(this);
    }

    size_t binary_row_array::keys_size() const
    {
        size_t size = 0;

#if _MSC_VER >= 1700
        for (inicolumn& item : m_table->m_keys)
        {
#else
        for (binary_key_array::iterator itor = m_table->keys.begin() ; itor != m_table->keys.end() ; itor++)
        {
            inicolumn& item = *itor;
#endif
            const std::type_info& typeinfo = item.datatype();
#ifdef _DEBUG
			const char* name = typeinfo.name();
#endif

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
        }

        return size;
    }

	binary_table::binary_table(binary_reader* reader, size_t columnCount, size_t rowCount)
		: m_columns(columnCount), m_rows(rowCount)
    {
		this->m_reader = reader;
        this->m_rows.set_table(*this);
    }

    std::string binary_table::category() const
    {
        return m_categoryName;
    }

    std::string binary_table::name() const
    {
        return m_tableName;
    }

    size_t binary_table::index() const
    {
        return m_index;
    }

    void binary_table::set_index(size_t index)
    {
        m_index = index;
    }

	idataset& binary_table::dataset() const
	{
		return *m_reader;
	}

    binary_table::~binary_table()
    {
        
    }

    binary_table_array::binary_table_array(binary_reader& reader)
        : m_reader(reader)
    {

    }

    binary_table_array::~binary_table_array()
    {
#if _MSC_VER >= 1700
        for (binary_table* item : m_tables)
        {
#else
        for (std::vector<binary_table*>::iterator itor = m_tables.begin() ; itor != m_tables.end() ; itor++)
        {
            binary_table* item = *itor;
#endif
            delete item;
        }
    }

    size_t binary_table_array::size() const
    {
        return m_tables.size();
    }

    itable& binary_table_array::at(size_t index) const throw()
    {
        itable* table = m_tables.at(index);
        if (table == NULL)
            return *const_cast<binary_table_array*>(this)->m_reader.read_table(index);
        return *table;
    }

    itable& binary_table_array::at(const std::string& tableName) const throw()
    {
        std::map<std::string, binary_table*>::const_iterator itor = m_nameToTable.find(conv_string(tableName));
        if (itor == m_nameToTable.end())
        {
            return *const_cast<binary_table_array*>(this)->m_reader.read_table(conv_string(tableName));
        }
        return *itor->second;
    }

    bool binary_table_array::contains(const std::string& tableName) const
    {
        return m_nameToTable.find(conv_string(tableName)) != m_nameToTable.end();
    }

    void binary_table_array::set(size_t index, binary_table* table)
    {
        std::string tableName;
        m_nameToTable[conv_string(table->name())] = table;
        m_tables[index] = table;
        dynamic_cast<binary_table*>(table)->set_index(index);

#ifdef _DEBUG
        //std::cout << table->name() << " is loaded : " << index << std::endl;
#endif
    }

    void binary_table_array::set_size(const std::vector<table_index>& indexes)
    {
        m_tables.assign(indexes.size(), NULL);

        m_tableNames.reserve(indexes.size());
        for (std::vector<table_index>::const_iterator itor = indexes.begin() ; itor != indexes.end() ; itor++)
        {
            const std::string& tableName = string_resource::get(itor->tableName);
            m_tableNames.push_back(tableName);
        }
    }

    void binary_table_array::set_flag(ReadFlag flag)
    {
        m_caseSensitive = (flag & ReadFlag_case_sensitive) != 0;
    }

    std::string binary_table_array::conv_string(const std::string& text) const
    {
        if (m_caseSensitive == true)
            return text;
        return iniutil::to_lower(text);
    }

    const itableNameArray& binary_table_array::names() const
    {
        return m_tableNames;
    }
        
    bool binary_table_array::is_table_loaded(const std::string& tableName) const
    {
        std::map<std::string, binary_table*>::const_iterator itor = m_nameToTable.find(conv_string(tableName));
        return itor != m_nameToTable.end();
    }

    void binary_table_array::load_table(const std::string& tableName)
    {
        if (this->is_table_loaded(tableName) == true)
            return;

        m_reader.read_table(conv_string(tableName));
    }

    void binary_table_array::release_table(const std::string& tableName)
    {
        std::map<std::string, binary_table*>::const_iterator itor = m_nameToTable.find(conv_string(tableName));
        if (itor == m_nameToTable.end())
            return;
        binary_table* table = itor->second;
		m_nameToTable.erase(itor->first);
        m_tables[table->index()] = nullptr;
        delete table;
    }
} /*namespace binary*/ } /*namespace internal*/ }

