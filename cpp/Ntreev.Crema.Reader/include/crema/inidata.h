#pragma once
#include "inidefine.h"
#include <string>
#include <typeinfo>
#include <vector>
#include <algorithm>
#include "iniexception.h"
#ifdef _MSC_VER
#include <type_traits>
#endif


namespace CremaReader
{
	class idataset;

	template<typename _inicontainer, typename _initype>
	class DLL_EXPORT cremaiterator : public std::iterator<std::input_iterator_tag, _initype>
	{
	public:
		cremaiterator(_inicontainer* c) : i(c->size()) { }
		cremaiterator(_inicontainer* c, size_t i) : i(i), c(c) { }
		cremaiterator(const cremaiterator& mit) : i(mit.i), c(mit.c) { }

		cremaiterator& operator++() { i = std::min(c->size(), ++i); return *this; }
		cremaiterator operator++(int) { cremaiterator tmp(*this); operator++(); return tmp; }
		bool operator==(const cremaiterator& rhs) { return i == rhs.i; }
		bool operator!=(const cremaiterator& rhs) { return i != rhs.i; }
		_initype& operator*() { return c->at(i); }
		_initype* operator->() { return &c->at(i); }

	private:
		size_t i;
		_inicontainer* c;
	};

	template<typename _inicontainer, typename _initype>
	class DLL_EXPORT const_cremaiterator : public std::iterator<std::input_iterator_tag, _initype>
	{
	public:
		const_cremaiterator(const _inicontainer* c) : i(c->size()) { }
		const_cremaiterator(const _inicontainer* c, size_t i) : i(i), c(c) { }
		const_cremaiterator(const const_cremaiterator& mit) : i(mit.i), c(mit.c) { }

		const_cremaiterator& operator++() { i = std::min(c->size(), ++i); return *this; }
		const_cremaiterator operator++(int) { const_cremaiterator tmp(*this); operator++(); return tmp; }
		bool operator==(const const_cremaiterator& rhs) { return i == rhs.i; }
		bool operator!=(const const_cremaiterator& rhs) { return i != rhs.i; }
		const _initype& operator*() { return c->at(i); }
		const _initype* operator->() { return &c->at(i); }
	private:
		size_t i;
		const _inicontainer* c;
	};

	class itable;

	class DLL_EXPORT inicolumn abstract
	{
	public:
		inicolumn() {};

		virtual const std::string& name() const = 0;
		virtual const std::type_info& datatype() const = 0;
		virtual bool is_key() const = 0;
		virtual size_t index() const = 0;
		virtual itable& table() const = 0;

	protected:
		virtual ~inicolumn() {};

#ifdef _DEBUG
	protected:
		const char* typeName;
#endif

	};

	class DLL_EXPORT irow abstract
	{
	public:
		virtual ~irow() {};

		virtual void set_value(const std::string& columnName, const std::string& text) = 0;
		virtual itable& table() const = 0;
		virtual long hash() const = 0;

		template<typename T>
		const T& value(const std::string& columnName) const;

		template<typename T>
		const T& value(size_t index) const;

		template<typename T>
		const T& value(const inicolumn& column) const;

		bool has_value(const std::string& columnName) const;
		bool has_value(size_t index) const;
		bool has_value(const inicolumn& column) const;

		bool to_boolean(const std::string& columnName) const;
		bool to_boolean(size_t index) const;
		bool to_boolean(const inicolumn& column) const;

		const std::string& to_string(const std::string& columnName) const;
		const std::string& to_string(size_t index) const;
		const std::string& to_string(const inicolumn& column) const;

		float to_single(const std::string& columnName) const;
		float to_single(size_t index) const;
		float to_single(const inicolumn& column) const;

		double to_double(const std::string& columnName) const;
		double to_double(size_t index) const;
		double to_double(const inicolumn& column) const;

		char to_int8(const std::string& columnName) const;
		char to_int8(size_t index) const;
		char to_int8(const inicolumn& column) const;

		unsigned char to_uint8(const std::string& columnName) const;
		unsigned char to_uint8(size_t index) const;
		unsigned char to_uint8(const inicolumn& column) const;

		short to_int16(const std::string& columnName) const;
		short to_int16(size_t index) const;
		short to_int16(const inicolumn& column) const;

		unsigned short to_uint16(const std::string& columnName) const;
		unsigned short to_uint16(size_t index) const;
		unsigned short to_uint16(const inicolumn& column) const;

		int to_int32(const std::string& columnName) const;
		int to_int32(size_t index) const;
		int to_int32(const inicolumn& column) const;

		unsigned int to_uint32(const std::string& columnName) const;
		unsigned int to_uint32(size_t index) const;
		unsigned int to_uint32(const inicolumn& column) const;

		long long to_int64(const std::string& columnName) const;
		long long to_int64(size_t index) const;
		long long to_int64(const inicolumn& column) const;

		unsigned long long to_uint64(const std::string& columnName) const;
		unsigned long long to_uint64(size_t index) const;
		unsigned long long to_uint64(const inicolumn& column) const;

		time_t to_datetime(const std::string& columnName) const;
		time_t to_datetime(size_t index) const;
		time_t to_datetime(const inicolumn& column) const;

		int to_duration(const std::string& columnName) const;
		int to_duration(size_t index) const;
		int to_duration(const inicolumn& column) const;

	protected:
		virtual const void* value_core(const inicolumn& column) const = 0;
		virtual bool has_value_core(const inicolumn& column) const = 0;
	};

	class DLL_EXPORT inikey_array abstract
	{
	public:
		typedef cremaiterator<inikey_array, inicolumn> iterator;
		typedef const_cremaiterator<inikey_array, inicolumn> const_iterator;

		inikey_array() {};
		virtual ~inikey_array() {};

		virtual size_t size() const = 0;
		virtual inicolumn& at(size_t index) const = 0;

		inicolumn& operator [] (size_t index) const;

		iterator begin() { return iterator(this, 0); }
		iterator end() { return iterator(this); }
		const_iterator begin() const { return const_iterator(this, 0); }
		const_iterator end() const { return const_iterator(this); }
	};

	class DLL_EXPORT icolumn_array abstract
	{
	public:
		typedef cremaiterator<icolumn_array, inicolumn> iterator;
		typedef const_cremaiterator<icolumn_array, inicolumn> const_iterator;

		icolumn_array() {};
		virtual ~icolumn_array() {};

		virtual size_t size() const = 0;
		virtual inicolumn& at(size_t index) const = 0;
		virtual inicolumn& at(const std::string& columnName) const = 0;
		virtual bool contains(const std::string& columnName) const = 0;

		inicolumn& operator [] (const std::string& columnName) const;
		inicolumn& operator [] (size_t index) const;

		iterator begin() { return iterator(this, 0); }
		iterator end() { return iterator(this); }
		const_iterator begin() const { return const_iterator(this, 0); }
		const_iterator end() const { return const_iterator(this); }
	};

	class DLL_EXPORT irow_array abstract
	{
	public:
		typedef cremaiterator<irow_array, irow> iterator;
		typedef const_cremaiterator<irow_array, irow> const_iterator;

		irow_array() {};
		virtual ~irow_array() {};

		virtual size_t size() const = 0;
		virtual irow& at(size_t index) const = 0;

		irow& operator [] (size_t index) const;

		iterator begin() { return iterator(this, 0); }
		iterator end() { return iterator(this); }
		const_iterator begin() const { return const_iterator(this, 0); }
		const_iterator end() const { return const_iterator(this); }

		template<typename key_type>
		iterator find(key_type key_value)
		{
			this->type_validation<key_type>();
			return this->find_core(1, &typeid(key_type), key_value);
		}

		template<typename key_type1, typename key_type2>
		iterator find(key_type1 key_value1, key_type2 key_value2)
		{
			this->type_validation<key_type1>();
			this->type_validation<key_type2>();
			return this->find_core(2,
				&typeid(key_type1), key_value1,
				&typeid(key_type2), key_value2);
		}

		template<typename key_type1, typename key_type2, typename key_type3>
		iterator find(key_type1 key_value1, key_type2 key_value2, key_type3 key_value3)
		{
			this->type_validation<key_type1>();
			this->type_validation<key_type2>();
			this->type_validation<key_type3>();
			return this->find_core(3,
				&typeid(key_type1), key_value1,
				&typeid(key_type2), key_value2,
				&typeid(key_type3), key_value3);
		}

		template<typename key_type1, typename key_type2, typename key_type3, typename key_type4>
		iterator find(key_type1 key_value1, key_type2 key_value2, key_type3 key_value3, key_type4 key_value4)
		{
			this->type_validation<key_type1>();
			this->type_validation<key_type2>();
			this->type_validation<key_type3>();
			this->type_validation<key_type4>();
			return this->find_core(4,
				&typeid(key_type1), key_value1,
				&typeid(key_type2), key_value2,
				&typeid(key_type3), key_value3,
				&typeid(key_type4), key_value4);
		}

	protected:
		virtual iterator find_core(size_t count, ...) = 0;

	private:
		template<typename type>
		void type_validation()
		{
#ifdef _MSC_VER
			bool value = std::is_integral<type>::value ||
				std::is_floating_point<type>::value ||
				typeid(type) == typeid(const char*) ||
				typeid(type) == typeid(char*) ||
				typeid(type) == typeid(std::string) ||
				typeid(type) == typeid(const std::string);
			if (value == false)
			{
				std::ostringstream stream;
				const char* werewr = typeid(type).name();
				stream << typeid(type).name() << "은(는) 올바른 타입이 아닙니다.";
				throw std::invalid_argument(stream.str());
			}
#endif
		}
	};

	class DLL_EXPORT itable abstract
	{
	public:
		virtual std::string category() const = 0;
		virtual std::string name() const = 0;
		virtual size_t index() const = 0;
		virtual std::string hash_value() const = 0;

		virtual const inikey_array& keys() const = 0;
		virtual const icolumn_array& columns() const = 0;
		virtual const irow_array& rows() const = 0;

		virtual idataset& dataset() const = 0;

	protected:
		itable & operator=(const itable&) { return *this; }
		itable() {};
		virtual ~itable() {};
	};

	typedef std::vector<std::string> itableNameArray;

	class DLL_EXPORT itable_array abstract
	{
	public:
		typedef cremaiterator<itable_array, itable> iterator;
		typedef const_cremaiterator<itable_array, itable> const_iterator;

		itable_array() {};
		virtual ~itable_array() {};

		virtual size_t size() const = 0;
		virtual itable& at(size_t index) const throw() = 0;
		virtual itable& at(const std::string& tableName) const throw() = 0;
		virtual bool contains(const std::string& tableName) const = 0;
		virtual const itableNameArray& names() const = 0;
		virtual bool is_table_loaded(const std::string& tableName) const = 0;
		virtual void load_table(const std::string& tableName) = 0;
		virtual void release_table(const std::string& tableName) = 0;

		itable& operator [] (const std::string& tableName) const;
		itable& operator [] (size_t index) const;

		iterator begin() { return iterator(this, 0); }
		iterator end() { return iterator(this); }
		const_iterator begin() const { return const_iterator(this, 0); }
		const_iterator end() const { return const_iterator(this); }
	};

	template<typename T>
	const T& irow::value(const std::string& columnName) const
	{
		return this->value<T>(this->table().columns().at(columnName));
	}

	template<typename T>
	const T& irow::value(size_t index) const
	{
		return this->value<T>(this->table().columns().at(index));
	}

	template<typename T>
	const T& irow::value(const inicolumn& column) const
	{
		if (column.datatype() != typeid(T))
		{
			std::ostringstream stream;
			stream << column.datatype().name() << " 에서 " << typeid(T).name() << " 으로 변환할 수 없습니다. ";

			throw std::invalid_argument(stream.str());
		}
		return *(T*)this->value_core(column);
	}
} /*namespace CremaReader*/
