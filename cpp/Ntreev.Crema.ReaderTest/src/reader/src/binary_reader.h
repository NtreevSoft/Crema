#pragma once
#include "../include/crema/inireader.h"
#include "binary_type.h"
#include "binary_data.h"
#include <iostream>
#include <fstream>

namespace CremaReader { namespace internal { namespace binary
{
    class binary_reader : public CremaReader
    {
    public:
        binary_reader();
        virtual ~binary_reader();

        virtual void read_core(std::istream& stream, ReadFlag flag);
        virtual void destroy();

        binary_table* read_table(const std::string& tableName);
        binary_table* read_table(size_t index);

		virtual const itable_array& tables() const { return m_tables; }
		virtual const std::string& name() const { return m_name; }
		virtual int version() const { return m_version; }

        binary_table_array m_tables;

    private:
        binary_table* read_table(std::istream& stream, std::streamoff offset, ReadFlag flag);
        void read_columns(std::istream& stream, binary_table& dataTable, size_t columnCount, ReadFlag flag);
        void read_rows(std::istream& stream, binary_table& dataTable, size_t rowCount);

        class findif
        {
        public:
            findif (ReadFlag flag, const std::string& tableName) : m_tableName(tableName), m_flag(flag) {}
            bool operator() (const table_index& index);
        private:
            std::string m_tableName;
            ReadFlag m_flag;
        };

    private:
        std::istream* m_stream;
        std::vector<table_index> m_tableIndexes;
        ReadFlag m_flag;
		std::string m_name;
		int m_version;
    };
} /*namespace binary*/ } /*namespace internal*/ }

