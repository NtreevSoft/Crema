#pragma once
#include "../include/crema/inidata.h"
#include "../include/crema/initype.h"
#include "binary_type.h"
#include <map>
#include <vector>
#include <string>

namespace CremaCode { namespace reader { namespace internal { namespace binary
{
    class binary_table;
    class binary_reader;

    class binary_column : public inicolumn
    {
    public:
        binary_column(const std::string& columnName, const std::type_info& dataType, bool iskey);
        virtual ~binary_column();

        virtual const std::string& name() const;
        virtual const std::type_info& datatype() const;
        virtual bool is_key() const;
        virtual size_t index() const;

        virtual itable& table() const;

        void set_table(itable& table);
        void set_index(size_t index);

        binary_column& operator=(const binary_column&) { return *this; }

    private:
        std::string m_columnName;
        const std::type_info& m_dataType;
        bool m_iskey;
        size_t m_columnIndex;
        itable* m_table;
    };

    class binary_row : public irow
    {
    public:
		binary_row();
		virtual ~binary_row();

        virtual const void* value_core(const inicolumn& column) const;
		virtual bool has_value_core(const inicolumn& column) const;
        virtual void set_value(const std::string& columnName, const std::string& text);
        virtual itable& table() const;
		virtual long hash() const;

        void reserve_fields_ptr(size_t size);
        char* fields_ptr();
        void set_table(binary_table& table);
		void set_hash(long hash);
		bool equals_key(va_list& vl);

    private:
        std::vector<char> m_fields;
        binary_table* m_table;
		long m_hash;
    };

    class binary_column_array : public icolumn_array
    {
    public:
        binary_column_array(size_t count);
        virtual ~binary_column_array();

        virtual size_t size() const;
        virtual inicolumn& at(size_t index) const;
        virtual inicolumn& at(const std::string& columnName) const;
        virtual bool contains(const std::string& columnName) const;

        void set(size_t index, binary_column* column);
        void set_flag(ReadFlag flag);

    private:
        std::string conv_string(const std::string& text) const;

    private:
        std::vector<binary_column*> m_columns;
        std::map<std::string, binary_column*> m_nameToColumn;
        bool m_caseSensitive;
    };

    class binary_key_array : public inikey_array
    {
    public:
        binary_key_array();
		virtual ~binary_key_array();

        virtual size_t size() const;
        virtual inicolumn& at(size_t index) const;

        void add(binary_column* column);

    private:
        std::vector<binary_column*> m_columns;
    };

    class binary_row_array : public irow_array
    {
    public:
        binary_row_array(size_t count);
		virtual ~binary_row_array();

        virtual size_t size() const;
        virtual binary_row& at(size_t index) const;
        
        binary_row& at(size_t index);
        void generate_key(size_t index);
        void set_table(binary_table& table);
        binary_table& table() const;

        iterator find_core(size_t count, ...);

    private:
        size_t keys_size() const;
        template<typename _type>
        void set_field_value(const char* buffer, size_t& offset, _type value)
        {
            _type* value_ptr = (_type*)(buffer + offset);
            *value_ptr = value;
            offset += sizeof(_type);
        }

    private:
        std::vector<binary_row> m_rows;
        std::multimap<long, binary_row*> m_keyTorow;
        binary_table* m_table;
    };

    class binary_table : public itable
    {
    public:
        binary_table(binary_reader* reader, size_t columnCount, size_t rowCount);
        virtual ~binary_table();

        virtual std::string category() const;
        virtual std::string name() const;
        virtual size_t index() const;
		virtual std::string hash_value() const;

        void set_index(size_t index);

		virtual const inikey_array& keys() const { return m_keys; }
		virtual const icolumn_array& columns() const { return m_columns; }
		virtual const irow_array& rows() const { return m_rows; }

		virtual idataset& dataset() const;

        binary_key_array m_keys;
		binary_column_array m_columns;
		binary_row_array m_rows;

    private:
        std::string m_tableName;
        std::string m_categoryName;
        size_t m_index;
		std::string m_hashValue;
		binary_reader* m_reader;

        friend class binary_reader;
    };

    class binary_table_array : public itable_array
    {
    public:
        binary_table_array(binary_reader& reader);
        virtual ~binary_table_array();

        virtual size_t size() const;
        virtual itable& at(size_t index) const throw();
        virtual itable& at(const std::string& tableName) const throw();
        virtual bool contains(const std::string& tableName) const;
        virtual const itableNameArray& names() const;
        virtual bool is_table_loaded(const std::string& tableName) const;
        virtual void load_table(const std::string& tableName);
        virtual void release_table(const std::string& tableName);


        void set(size_t index, binary_table* dataTable);
        void set_size(const std::vector<table_index>& indexes);
        void set_flag(ReadFlag flag);

        binary_table_array& operator=(const binary_table_array&) { return *this; }

    private:
        std::string conv_string(const std::string& text) const;

    private:
        std::map<std::string, binary_table*> m_nameToTable;
        std::vector<binary_table*> m_tables;
        itableNameArray m_tableNames;
        binary_reader& m_reader;
        bool m_caseSensitive;
    };
} /*namespace binary*/ } /*namespace internal*/ } /*namespace CremaCode*/ } /*namespace reader*/

