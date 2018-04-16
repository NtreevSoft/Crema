#include "../include/crema/inidata.h"
#include <time.h>

namespace CremaReader
{
	inicolumn& inikey_array::operator [] (size_t index) const
	{
		return this->at(index);
	}

	inicolumn& icolumn_array::operator [] (const std::string& columnName) const
	{
		return this->at(columnName);
	}

	inicolumn& icolumn_array::operator [] (size_t index) const
	{
		return this->at(index);
	}

	irow& irow_array::operator [] (size_t index) const
	{
		return this->at(index);
	}

	itable& itable_array::operator [] (const std::string& tableName) const
	{
		return this->at(tableName);
	}

	itable& itable_array::operator [] (size_t index) const
	{
		return this->at(index);
	}

	bool irow::has_value(const std::string& columnName) const
	{
		return this->has_value_core(this->table().columns().at(columnName));
	}

	bool irow::has_value(size_t index) const
	{
		return this->has_value_core(this->table().columns().at(index));
	}

	bool irow::has_value(const inicolumn& column) const
	{
		return this->has_value_core(column);
	}

	bool irow::to_boolean(const std::string& columnName) const
	{
		return this->value<bool>(this->table().columns().at(columnName));
	}

	bool irow::to_boolean(size_t index) const
	{
		return this->value<bool>(this->table().columns().at(index));
	}

	bool irow::to_boolean(const inicolumn& column) const
	{
		return this->value<bool>(column);
	}

	const std::string& irow::to_string(const std::string& columnName) const
	{
		return this->value<std::string>(this->table().columns().at(columnName));
	}

	const std::string& irow::to_string(size_t index) const
	{
		return this->value<std::string>(this->table().columns().at(index));
	}

	const std::string& irow::to_string(const inicolumn& column) const
	{
		return this->value<std::string>(column);
	}

	float irow::to_single(const std::string& columnName) const
	{
		return this->value<float>(this->table().columns().at(columnName));
	}

	float irow::to_single(size_t index) const
	{
		return this->value<float>(this->table().columns().at(index));
	}

	float irow::to_single(const inicolumn& column) const
	{
		return this->value<float>(column);
	}

	double irow::to_double(const std::string& columnName) const
	{
		return this->value<double>(this->table().columns().at(columnName));
	}

	double irow::to_double(size_t index) const
	{
		return this->value<double>(this->table().columns().at(index));
	}

	double irow::to_double(const inicolumn& column) const
	{
		return this->value<double>(column);
	}

	char irow::to_int8(const std::string& columnName) const
	{
		return this->value<char>(this->table().columns().at(columnName));
	}

	char irow::to_int8(size_t index) const
	{
		return this->value<char>(this->table().columns().at(index));
	}

	char irow::to_int8(const inicolumn& column) const
	{
		return this->value<char>(column);
	}

	unsigned char irow::to_uint8(const std::string& columnName) const
	{
		return this->value<unsigned char>(this->table().columns().at(columnName));
	}

	unsigned char irow::to_uint8(size_t index) const
	{
		return this->value<unsigned char>(this->table().columns().at(index));
	}

	unsigned char irow::to_uint8(const inicolumn& column) const
	{
		return this->value<unsigned char>(column);
	}

	short irow::to_int16(const std::string& columnName) const
	{
		return this->value<short>(this->table().columns().at(columnName));
	}

	short irow::to_int16(size_t index) const
	{
		return this->value<short>(this->table().columns().at(index));
	}

	short irow::to_int16(const inicolumn& column) const
	{
		return this->value<short>(column);
	}

	unsigned short irow::to_uint16(const std::string& columnName) const
	{
		return this->value<unsigned short>(this->table().columns().at(columnName));
	}

	unsigned short irow::to_uint16(size_t index) const
	{
		return this->value<unsigned short>(this->table().columns().at(index));
	}

	unsigned short irow::to_uint16(const inicolumn& column) const
	{
		return this->value<unsigned short>(column);
	}

	int irow::to_int32(const std::string& columnName) const
	{
		return this->value<int>(this->table().columns().at(columnName));
	}

	int irow::to_int32(size_t index) const
	{
		return this->value<int>(this->table().columns().at(index));
	}

	int irow::to_int32(const inicolumn& column) const
	{
		return this->value<int>(column);
	}

	unsigned int irow::to_uint32(const std::string& columnName) const
	{
		return this->value<unsigned int>(this->table().columns().at(columnName));
	}

	unsigned int irow::to_uint32(size_t index) const
	{
		return this->value<unsigned int>(this->table().columns().at(index));
	}

	unsigned int irow::to_uint32(const inicolumn& column) const
	{
		return this->value<unsigned int>(column);
	}

	long long irow::to_int64(const std::string& columnName) const
	{
		return this->value<long long>(this->table().columns().at(columnName));
	}

	long long irow::to_int64(size_t index) const
	{
		return this->value<long long>(this->table().columns().at(index));
	}

	long long irow::to_int64(const inicolumn& column) const
	{
		return this->value<long long>(column);
	}

	unsigned long long irow::to_uint64(const std::string& columnName) const
	{
		return this->value<unsigned long long>(this->table().columns().at(columnName));
	}

	unsigned long long irow::to_uint64(size_t index) const
	{
		return this->value<unsigned long long>(this->table().columns().at(index));
	}

	unsigned long long irow::to_uint64(const inicolumn& column) const
	{
		return this->value<unsigned long long>(column);
	}


	time_t irow::to_datetime(const std::string& columnName) const
	{
		return (time_t)this->value<long long>(columnName);
	}

	time_t irow::to_datetime(size_t index) const
	{
		return (time_t)this->value<long long>(index);
	}

	time_t irow::to_datetime(const inicolumn& column) const
	{
		return (time_t)this->value<long long>(column);
	}

	int irow::to_duration(const std::string& columnName) const
	{
		return this->value<int>(columnName);
	}

	int irow::to_duration(size_t index) const
	{
		return this->value<int>(index);
	}

	int irow::to_duration(const inicolumn& column) const
	{
		return this->value<int>(column);
	}
}

