#pragma once
#include "../include/crema/inidefine.h"

namespace CremaReader { namespace internal { namespace binary
{
    const int magic_value = 0x6cfc4a14;

    struct table_header
    {
        int magicValue;
        int version;
        long long modifiedTime;
        long long tableInfoOffset;
        long long columnsOffset;
        long long rowsOffset;
        long long stringResourcesOffset;
        long long userOffset;
    };

	struct table_info
    {
        int tableName;
        int categoryName;
        int columnCount;
        int rowCount;
    };

    struct column_info
    {
        int columnName;
        int dataType;
        int iskey;
    };

    struct file_header
    {
        int magicValue;
        int version;
        long long buildTime;
        int tableCount;
		int name;
        long long indexOffset;
        long long tablesOffset;
        long long stringResourcesOffset;
    };

    struct table_index
    {
        int tableName;
		int dummy;
        long long offset;
    };
} /*namespace binary*/ } /*namespace internal*/ }

