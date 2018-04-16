//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#pragma once
#include <algorithm>
#include "reader/include/crema/inireader.h"
#include "reader/include/crema/iniutils.h"
#include <map>

namespace CremaCode
{
	typedef bool(*ErrorOccured) (const std::exception& e);

	class CremaData
	{
	public:
		static bool InvokeErrorOccured(const std::exception& e)
		{
			if (CremaData::ErrorOccured == NULL)
				return false;
			return CremaData::ErrorOccured(e);
		}

		static ErrorOccured ErrorOccured;
	};

	class CremaRow
	{

	private:
		std::string _relationID;
		std::string _parentID;

		long _key;

	protected:
		CremaRow(reader::irow& row)
		{
			std::ostringstream stringStream;

			for (auto itor = row.table().columns().begin(); itor != row.table().columns().end(); itor++)
			{
				if ((*itor).name() == "__RelationID__")
				{
					_relationID = row.value<std::string>(*itor);
				}
				else if ((*itor).name() == "__ParentID__")
				{
					_parentID = row.value<std::string>(*itor);
				}
			}
		}

	protected:
		template<typename keytype>
		void SetKey(keytype keyvalue)
		{
			_key = reader::iniutil::generate_hash(keyvalue);
		}

		template<typename keytype1, typename keytype2>
		void SetKey(keytype1 keyvalue1, keytype2 keyvalue2)
		{
			_key = reader::iniutil::generate_hash(keyvalue1, keyvalue2);
		}

		template<typename keytype1, typename keytype2, typename keytype3>
		void SetKey(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3)
		{
			_key = reader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3);
		}

		template<typename keytype1, typename keytype2, typename keytype3, typename keytype4>
		void SetKey(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3, keytype4 keyvalue4)
		{
			_key = reader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3, keyvalue4);
		}

		template<typename keytype1, typename keytype2, typename keytype3, typename keytype4, typename keytype5>
		void SetKey(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3, keytype4 keyvalue4, keytype5 keyvalue5)
		{
			_key = reader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3, keyvalue4, keyvalue5);
		}

		template<typename T = CremaRow, typename U = CremaRow>
		friend void SetParent(T* parent, const std::vector<U*>& childs)
		{
			for each (auto item in childs)
			{
				item->Parent = parent;
			}
		}

	private:
		friend long GetKey(CremaRow* target)
		{
			return target->_key;
		}

		friend const std::string& GetRelationID(CremaRow* target)
		{
			return target->_relationID;
		}

		friend const std::string& GetParentID(CremaRow* target)
		{
			return target->_parentID;
		}
	};

	template<class T = CremaRow>
	class CremaTable
	{
	public:
		const std::vector<T*>& Rows;

	private:
		std::map<long, T*> _keyToRow;
		std::vector<T*> _rows;
		std::string _name;
		std::string _tableName;

	public:
		CremaTable()
			: Rows(_rows)
		{

		}

		virtual ~CremaTable()
		{
			for each(auto item in _rows)
			{
				delete item;
			}
			_rows.clear();
		}

		const std::string& name() const
		{
			return _name;
		}

		const std::string& tableName() const
		{
			return _tableName;
		}

	protected:
		void ReadFromTable(reader::itable& table)
		{
			_name = table.name();
			_tableName = this->GetTableName(table.name());
			_rows.reserve(table.rows().size());
			for (size_t i = 0; i < table.rows().size(); i++)
			{
				reader::irow& item = table.rows().at(i);
				T* row = (T*)(this->CreateRow(item, this));
				_keyToRow.insert(std::map<long, T*>::value_type(GetKey(row), row));
				_rows.push_back(row);
			}
		}

		void ReadFromRows(const std::string& name, const std::vector<T*>& rows)
		{
			_name = name;
			_tableName = this->GetTableName(name);
			if (rows.size() == 0)
				return;

			_rows.reserve(rows.size());
			for each (auto item in rows)
			{
				_keyToRow.insert(std::map<long, T*>::value_type(GetKey(item), item));
				_rows.push_back(item);
			}
		}

		virtual void* CreateRow(reader::irow& row, void* table) = 0;

		template<typename U = CremaRow>
		void SetRelations(const std::string& childName, const std::vector<U*>& childs, void(*setChildsAction)(T*, const std::string&, const std::vector<U*>&))
		{
			std::map<std::string, T*> relationToRow;

			for each (auto item in this->Rows)
			{
				relationToRow.insert(std::map<std::string, T*>::value_type(GetRelationID(item), item));
			}

			std::map<T*, std::vector<U*>> rowToChilds;

			for each (auto item in childs)
			{
				auto itor = relationToRow.find(GetParentID(item));
				if (itor == relationToRow.end())
					continue;

				auto parent = itor->second;

				if (rowToChilds.find(parent) == rowToChilds.end())
					rowToChilds.insert(std::map<T*, std::vector<U*>>::value_type(parent, std::vector<U*>()));

				rowToChilds[parent].push_back(item);
			}

			for each (auto item in rowToChilds)
			{
				setChildsAction(item.first, childName, item.second);
			}
		}

		template<typename keytype>
		const T* FindRow(keytype keyvalue) const
		{
			long key = reader::iniutil::generate_hash(keyvalue);
			return _keyToRow.find(key)->second;
		}

		template<typename keytype1, typename keytype2>
		const T* FindRow(keytype1 keyvalue1, keytype2 keyvalue2) const
		{
			long key = reader::iniutil::generate_hash(keyvalue1, keyvalue2);
			return _keyToRow.find(key)->second;
		}

		template<typename keytype1, typename keytype2, typename keytype3>
		const T* FindRow(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3) const
		{
			long key = reader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3);
			return _keyToRow.find(key)->second;
		}

		template<typename keytype1, typename keytype2, typename keytype3, typename keytype4>
		const T* FindRow(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3, keytype4 keyvalue4) const
		{
			long key = reader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3, keyvalue4);
			return _keyToRow.find(key)->second;
		}

		template<typename keytype1, typename keytype2, typename keytype3, typename keytype4, typename keytype5>
		const T* FindRow(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3, keytype4 keyvalue4, keytype5 keyvalue5) const
		{
			long key = reader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3, keyvalue4, keyvalue5);
			return _keyToRow.find(key)->second;
		}

		friend class CremaRow;

	private:
		std::string GetTableName(const std::string& name) const
		{
			std::vector<std::string> elems;
			std::stringstream ss(name);
			std::string item;
			while (std::getline(ss, item, '.'))
			{
				elems.push_back(item);
			}

			if (elems.size() == 1)
				return name;
			return elems[1];
		}
	};
} /*namespace CremaCode*/
