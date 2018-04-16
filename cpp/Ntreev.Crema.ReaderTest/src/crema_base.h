#pragma once
#include <algorithm>
#include "reader/include/crema/inireader.h"
#include "reader/include/crema/iniutils.h"
#include <map>

namespace cremacode
{
	class CremaData
	{

	};

	class CremaRow
	{

	private:
		std::string _relationID;

		long _key;

	protected:
		CremaRow(CremaReader::irow& row)
		{
			std::string tableName = this->GetPureName(row.table().name());
			std::ostringstream stringStream;
			stringStream << tableName << "Id";
			std::string GetRelationID = stringStream.str();

			for (auto itor = row.table().columns().begin(); itor != row.table().columns().end(); itor++)
			{
				if ((*itor).name() == GetRelationID)
				{
					_relationID = row.value<std::string>(*itor);
				}
			}
		}

	protected:
		template<typename keytype>
		void SetKey(keytype keyvalue)
		{
			_key = CremaReader::iniutil::generate_hash(keyvalue);
		}

		template<typename keytype1, typename keytype2>
		void SetKey(keytype1 keyvalue1, keytype2 keyvalue2)
		{
			_key = CremaReader::iniutil::generate_hash(keyvalue1, keyvalue2);
		}

		template<typename keytype1, typename keytype2, typename keytype3>
		void SetKey(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3)
		{
			_key = CremaReader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3);
		}

		template<typename keytype1, typename keytype2, typename keytype3, typename keytype4>
		void SetKey(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3, keytype4 keyvalue4)
		{
			_key = CremaReader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3, keyvalue4);
		}

		template<typename keytype1, typename keytype2, typename keytype3, typename keytype4, typename keytype5>
		void SetKey(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3, keytype4 keyvalue4, keytype5 keyvalue5)
		{
			_key = CremaReader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3, keyvalue4, keyvalue5);
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
		std::string GetPureName(const std::string& tableName) const
		{
			std::vector<std::string> elems;
			std::stringstream ss(tableName);
			std::string item;
			while (std::getline(ss, item, '.')) 
			{
				elems.push_back(item);
			}

			if (elems.size() == 1)
				return tableName;
			return elems[1];
		}

		friend long GetKey(CremaRow* target)
		{
			return target->_key;
		}

		friend const std::string& GetRelationID(CremaRow* target)
		{
			return target->_relationID;
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

	protected:
		void ReadFromTable(CremaReader::itable& table)
		{
			_rows.reserve(table.rows().size());
			for (size_t i = 0; i < table.rows().size(); i++)
			{
				CremaReader::irow& item = table.rows().at(i);
				T* row = (T*)(this->CreateRow(item, this));
				_keyToRow.insert(std::map<long, T*>::value_type(GetKey(row), row));
				_rows.push_back(row);
			}
		}

		void ReadFromRows(const std::vector<T*>& rows)
		{
			if (rows.size() == 0)
				return;

			_rows.reserve(rows.size());
			for each (auto item in rows)
			{
				_keyToRow.insert(std::map<long, T*>::value_type(GetKey(item), item));
				_rows.push_back(item);
			}
		}

		virtual void* CreateRow(CremaReader::irow& row, void* table) = 0;

		template<typename U = CremaRow>
		void SetRelations(const std::vector<U*>& childs, void(*setChildsAction)(T*, const std::vector<U*>&))
		{
			std::map<std::string, T*> relationToRow;

			for each (auto item in this->Rows)
			{
				relationToRow.insert(std::map<std::string, T*>::value_type(GetRelationID(item), item));
			}

			std::map<T*, std::vector<U*>> rowToChilds;

			for each (auto item in childs)
			{
				auto itor = relationToRow.find(GetRelationID(item));
				if (itor == relationToRow.end())
					continue;

				auto parent = itor->second;

				if (rowToChilds.find(parent) == rowToChilds.end())
					rowToChilds.insert(std::map<T*, std::vector<U*>>::value_type(parent, std::vector<U*>()));

				rowToChilds[parent].push_back(item);
			}

			for each (auto item in rowToChilds)
			{
				setChildsAction(item.first, item.second);
			}
		}

		template<typename keytype>
		const T* FindRow(keytype keyvalue) const
		{
			long key = CremaReader::iniutil::generate_hash(keyvalue);
			return _keyToRow.find(key)->second;
		}

		template<typename keytype1, typename keytype2>
		const T* FindRow(keytype1 keyvalue1, keytype2 keyvalue2) const
		{
			long key = CremaReader::iniutil::generate_hash(keyvalue1, keyvalue2);
			return _keyToRow.find(key)->second;
		}

		template<typename keytype1, typename keytype2, typename keytype3>
		const T* FindRow(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3) const
		{
			long key = CremaReader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3);
			return _keyToRow.find(key)->second;
		}

		template<typename keytype1, typename keytype2, typename keytype3, typename keytype4>
		const T* FindRow(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3, keytype4 keyvalue4) const
		{
			long key = CremaReader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3, keyvalue4);
			return _keyToRow.find(key)->second;
		}

		template<typename keytype1, typename keytype2, typename keytype3, typename keytype4, typename keytype5>
		const T* FindRow(keytype1 keyvalue1, keytype2 keyvalue2, keytype3 keyvalue3, keytype4 keyvalue4, keytype5 keyvalue5) const
		{
			long key = CremaReader::iniutil::generate_hash(keyvalue1, keyvalue2, keyvalue3, keyvalue4, keyvalue5);
			return _keyToRow.find(key)->second;
		}

		friend class CremaRow;
	};
} /* namespace cremacode*/

