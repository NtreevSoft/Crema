#include "binary_reader.h"
#include "internal_utils.h"
#include "../include/crema/iniutils.h"
#include <algorithm>
#include "../include/crema/iniexception.h"

namespace CremaReader { namespace internal { namespace binary
{
    binary_reader::binary_reader()
		: m_tables(*this)
    {

    }

    binary_reader::~binary_reader()
	{
		
	}

    void binary_reader::destroy()
    {
        delete this;
    }

	void binary_reader::read_core(std::istream& stream, ReadFlag flag)
	{
        file_header fileHeader;
        m_stream = &stream;
        m_flag = flag;

        stream.seekg(0, std::ios_base::beg);
        stream.read((char*)&fileHeader, sizeof(file_header));
        m_tableIndexes.assign(fileHeader.tableCount, table_index());
        if (fileHeader.tableCount == 0)
            return;
        stream.read((char*)&m_tableIndexes.front(), sizeof(table_index) * fileHeader.tableCount);
		m_version = fileHeader.version;

        stream.seekg(fileHeader.stringResourcesOffset);
        string_resource::read(stream);
		m_name = string_resource::get(fileHeader.name);

        this->m_tables.set_size(m_tableIndexes);
		this->m_tables.set_flag(flag);

        if ((flag & ReadFlag_lazy_loading) == false)
        {
            for (size_t i=0 ; i<m_tableIndexes.size() ; i++)
            {
                const table_index& tableIndex = m_tableIndexes[i];

                binary_table* table = read_table(stream, tableIndex.offset, flag);
                this->m_tables.set(i, table);
            }
        }
	}

    binary_table* binary_reader::read_table(size_t index)
    {
        const table_index& table_index = m_tableIndexes.at(index);
        binary_table* table = binary_reader::read_table(*m_stream, table_index.offset, m_flag);
        this->m_tables.set(index, table);
        return table;
    }

    bool binary_reader::findif::operator() (const table_index& index)
    {
        if ((m_flag & ReadFlag_case_sensitive) != 0)
            return m_tableName == string_resource::get(index.tableName);
        return m_tableName == iniutil::to_lower(string_resource::get(index.tableName));
    }

    binary_table* binary_reader::read_table(const std::string& tableName)
    {
        std::vector<table_index>::const_iterator itor = std::find_if(m_tableIndexes.begin(), m_tableIndexes.end(), findif (m_flag, tableName));

        if (itor == m_tableIndexes.end())
            throw keynotfoundexception(tableName, "tables");

        size_t index = itor - m_tableIndexes.begin();

        binary_table* table = binary_reader::read_table(*m_stream, itor->offset, m_flag);
        this->m_tables.set(index, table);
        return table;
    }

    binary_table* binary_reader::read_table(std::istream& stream, std::streamoff offset, ReadFlag flag)
    {
        table_header tableHeader;
		table_info tableInfo;
        
        stream.seekg(offset, std::ios::beg);
		stream.read((char*)&tableHeader, sizeof(table_header));

        stream.seekg(tableHeader.tableInfoOffset + offset, std::ios::beg);
		stream.read((char*)&tableInfo, sizeof(table_info));

        binary_table* table = new binary_table(this, tableInfo.columnCount, tableInfo.rowCount);

        stream.seekg(tableHeader.stringResourcesOffset + offset, std::ios::beg);
        string_resource::read(stream);

        stream.seekg(tableHeader.columnsOffset + offset);
        binary_reader::read_columns(stream, *table, tableInfo.columnCount, flag);

        stream.seekg(tableHeader.rowsOffset + offset, std::ios::beg);
        binary_reader::read_rows(stream, *table, tableInfo.rowCount);
        
        table->m_tableName = string_resource::get(tableInfo.tableName);
        table->m_categoryName = string_resource::get(tableInfo.categoryName);
		
        return table;
    }

    void binary_reader::read_columns(std::istream& stream, binary_table& table, size_t columnCount, ReadFlag flag)
    {
		table.m_columns.set_flag(flag);

        for (size_t i=0 ; i<columnCount ; i++)
        {
            column_info columninfo;
            stream.read((char*)&columninfo, sizeof(column_info));

            const std::string& columnName = string_resource::get(columninfo.columnName);
            const std::string& typeName = string_resource::get(columninfo.dataType);
            bool isKey = columninfo.iskey == 0 ? false : true;

            binary_column* column = new binary_column(columnName, iniutil::name_to_type(typeName), isKey);

            table.m_columns.set(i, column);

            if (isKey == true)
                table.m_keys.add(column);

            column->set_table(table);
        }
    }

    void binary_reader::read_rows(std::istream& stream, binary_table& table, size_t rowCount)
    {
        for (size_t i=0 ; i<rowCount; i++)
        {
            binary_row& dataRow = table.m_rows.at(i);

            int length;
            stream.read((char*)&length, sizeof(int));

            dataRow.reserve_fields_ptr(length);
            stream.read(dataRow.fields_ptr(), length);
            dataRow.set_table(table);
            table.m_rows.generate_key(i);
        }
    }
} /*namespace binary*/ } /*namespace internal*/ }

