#include "crema_tables.h"

namespace CremaCode
{
    ProjectInfoExportInfoRow::ProjectInfoExportInfoRow(CremaCode::reader::irow& row, ProjectInfoExportInfoTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->ID = row.to_int32(0);
        if (row.has_value(1))
        {
            this->FileName = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->TableName = row.to_string(2);
        }
        this->SetKey(this->ID);
    }
    
    ProjectInfoExportInfoTable::ProjectInfoExportInfoTable()
    {
    }
    
    ProjectInfoExportInfoTable::ProjectInfoExportInfoTable(CremaCode::reader::itable& table)
    {
        if ((table.hash_value() != "06e32bd7a3bb2e2ca4e256312e847b3413b4bb5a"))
        {
            throw new std::exception("ProjectInfo.ExportInfo 테이블과 데이터의 형식이 맞지 않습니다.");
        }
        this->ReadFromTable(table);
    }
    
    ProjectInfoExportInfoTable::ProjectInfoExportInfoTable(std::string name, std::vector<ProjectInfoExportInfoRow*> rows)
    {
        this->ReadFromRows(name, rows);
    }
    
    ProjectInfoExportInfoTable::~ProjectInfoExportInfoTable()
    {
    }
    
    void* ProjectInfoExportInfoTable::CreateRow(CremaCode::reader::irow& row, void* table)
    {
        return new ProjectInfoExportInfoRow(row, ((ProjectInfoExportInfoTable*)(table)));
    }
    
    const ProjectInfoExportInfoRow* ProjectInfoExportInfoTable::Find(int ID) const
    {
        return this->FindRow(ID);
    }
    
    ProjectInfoExportInfoTable ProjectInfoRow::ExportInfoEmpty;
    
    ProjectInfoRow::ProjectInfoRow(CremaCode::reader::irow& row, ProjectInfoTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->KindType = ((CremaCode::KindType)(row.to_int32(0)));
        this->ProjectType = ((CremaCode::ProjectType)(row.to_int32(1)));
        this->Name = row.to_string(2);
        if (row.has_value(3))
        {
            this->ProjectPath = row.to_string(3);
        }
        if ((this->ExportInfo == nullptr))
        {
            this->ExportInfo = &(ProjectInfoRow::ExportInfoEmpty);
        }
        this->SetKey(this->KindType, this->ProjectType, this->Name);
    }
    
    void ProjectInfoSetExportInfo(ProjectInfoRow* target, const std::string& childName, const std::vector<ProjectInfoExportInfoRow*>& childs)
    {
        SetParent(target, childs);
        target->ExportInfo = new ProjectInfoExportInfoTable(childName, childs);
    }
    
    ProjectInfoTable::ProjectInfoTable()
    {
    }
    
    ProjectInfoTable::ProjectInfoTable(CremaCode::reader::itable& table)
    {
        if ((table.hash_value() != "b576d4aab4aaa0f758549efab982de6ceb5b7b66"))
        {
            throw new std::exception("ProjectInfo 테이블과 데이터의 형식이 맞지 않습니다.");
        }
        this->ExportInfo = new ProjectInfoExportInfoTable(table.dataset().tables()[(table.name() + ".ExportInfo")]);
        this->ReadFromTable(table);
        this->SetRelations((table.name() + ".ExportInfo"), this->ExportInfo->Rows, ProjectInfoSetExportInfo);
    }
    
    ProjectInfoTable::~ProjectInfoTable()
    {
        delete this->ExportInfo;
    }
    
    void* ProjectInfoTable::CreateRow(CremaCode::reader::irow& row, void* table)
    {
        return new ProjectInfoRow(row, ((ProjectInfoTable*)(table)));
    }
    
    const ProjectInfoRow* ProjectInfoTable::Find(CremaCode::KindType KindType, CremaCode::ProjectType ProjectType, const std::string& Name) const
    {
        return this->FindRow(KindType, ProjectType, Name);
    }
    
    StringTableRow::StringTableRow(CremaCode::reader::irow& row, StringTableTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Type = ((CremaCode::StringType)(row.to_int32(0)));
        this->Name = row.to_string(1);
        if (row.has_value(2))
        {
            this->Comment = row.to_string(2);
        }
        if (row.has_value(3))
        {
            this->Value = row.to_string(3);
        }
        if (row.has_value(4))
        {
            this->ko_KR = row.to_string(4);
        }
        this->SetKey(this->Type, this->Name);
    }
    
    StringTableTable::StringTableTable()
    {
    }
    
    StringTableTable::StringTableTable(CremaCode::reader::itable& table)
    {
        if ((table.hash_value() != "6c66b01d950836901feeeed701a0b36f5f3d58c1"))
        {
            throw new std::exception("StringTable 테이블과 데이터의 형식이 맞지 않습니다.");
        }
        this->ReadFromTable(table);
    }
    
    StringTableTable::~StringTableTable()
    {
    }
    
    void* StringTableTable::CreateRow(CremaCode::reader::irow& row, void* table)
    {
        return new StringTableRow(row, ((StringTableTable*)(table)));
    }
    
    const StringTableRow* StringTableTable::Find(CremaCode::StringType Type, const std::string& Name) const
    {
        return this->FindRow(Type, Name);
    }
    
    TestTableRow::TestTableRow(CremaCode::reader::irow& row, TestTableTable* table)
        : CremaRow(row)
    {
        this->Table = table;
        this->Key = row.to_int32(0);
        if (row.has_value(1))
        {
            this->Value = row.to_string(1);
        }
        if (row.has_value(2))
        {
            this->DateTimePicker = row.to_datetime(2);
        }
        if (row.has_value(3))
        {
            this->TimePicker = row.to_duration(3);
        }
        this->SetKey(this->Key);
    }
    
    TestTableTable::TestTableTable()
    {
    }
    
    TestTableTable::TestTableTable(CremaCode::reader::itable& table)
    {
        if ((table.hash_value() != "148d8eee57fb716dc0914b646b2f965325693a55"))
        {
            throw new std::exception("TestTable 테이블과 데이터의 형식이 맞지 않습니다.");
        }
        this->ReadFromTable(table);
    }
    
    TestTableTable::~TestTableTable()
    {
    }
    
    void* TestTableTable::CreateRow(CremaCode::reader::irow& row, void* table)
    {
        return new TestTableRow(row, ((TestTableTable*)(table)));
    }
    
    const TestTableRow* TestTableTable::Find(int Key) const
    {
        return this->FindRow(Key);
    }
    
    CremaDataSet::CremaDataSet(CremaCode::reader::idataset& dataSet, bool verifyRevision)
    {
        this->ClientApplicationHost = nullptr;
        this->ClientBase = nullptr;
        this->ClientConsole = nullptr;
        this->ClientConverters = nullptr;
        this->ClientDifferences = nullptr;
        this->ClientFramework = nullptr;
        this->ClientServices = nullptr;
        this->ClientSmartSet = nullptr;
        this->ClientTables = nullptr;
        this->ClientTypes = nullptr;
        this->ClientUsers = nullptr;
        this->CommonPresentation = nullptr;
        this->CommonServiceModel = nullptr;
        this->CommonServiceModel_Event = nullptr;
        this->CremaData = nullptr;
        this->CremaProjectInfo = nullptr;
        this->ModernUIFramework = nullptr;
        this->NtreevLibrary = nullptr;
        this->ProjectInfo = nullptr;
        this->ServerServiceHosts = nullptr;
        this->StringTable = nullptr;
        this->StringTable11 = nullptr;
        this->TestTable = nullptr;
        this->Load(dataSet, verifyRevision);
    }
    
    CremaDataSet::CremaDataSet(const std::string& filename, bool verifyRevision)
        : CremaDataSet(CremaCode::reader::CremaReader::ReadFromFile(filename), verifyRevision)
    {
    }
    
    CremaDataSet::~CremaDataSet()
    {
        delete this->ProjectInfo;
        delete this->StringTable;
        delete this->TestTable;
    }
    
    std::string CremaDataSet::name()
    {
        return this->_name;
    }
    
    long long CremaDataSet::revision()
    {
        return this->_revision;
    }
    
    std::string CremaDataSet::typesHashValue()
    {
        return this->_typesHashValue;
    }
    
    std::string CremaDataSet::tablesHashValue()
    {
        return this->_tablesHashValue;
    }
    
    std::string CremaDataSet::tags()
    {
        return this->_tags;
    }
    
    void CremaDataSet::Load(const std::string& filename, bool verifyRevision)
    {
        this->Load(CremaCode::reader::CremaReader::ReadFromFile(filename), verifyRevision);
    }
    
    void CremaDataSet::Load(CremaCode::reader::idataset& dataSet, bool verifyRevision)
    {
        try
        {
            if (("default" != dataSet.name()))
            {
                throw std::exception("데이터의 이름이 코드 이름(default)과 다릅니다.");
            }
        }
        catch (std::exception& e)
        {
            if ((CremaCode::CremaData::InvokeErrorOccured(e) == false))
            {
                throw e;
            }
        }
        try
        {
            if (((verifyRevision == true) 
                        && (((int)(412)) != dataSet.revision())))
            {
                throw std::exception("데이터의 리비전 코드 리비전(412)과 다릅니다.");
            }
        }
        catch (std::exception& e)
        {
            if ((CremaCode::CremaData::InvokeErrorOccured(e) == false))
            {
                throw e;
            }
        }
        try
        {
            if (("4b1da7ae630f37090a4cee6e3e446893c6ad9297" != dataSet.types_hash_value()))
            {
                throw std::exception("타입 해시값이 잘못되었습니다.");
            }
        }
        catch (std::exception& e)
        {
            if ((CremaCode::CremaData::InvokeErrorOccured(e) == false))
            {
                throw e;
            }
        }
        try
        {
            if (("f52bdea30268729d366eaeb9bd320df505a16dbf" != dataSet.tables_hash_value()))
            {
                throw std::exception("테이블 해시값이 잘못되었습니다.");
            }
        }
        catch (std::exception& e)
        {
            if ((CremaCode::CremaData::InvokeErrorOccured(e) == false))
            {
                throw e;
            }
        }
        this->_name = dataSet.name();
        this->_revision = dataSet.revision();
        this->_typesHashValue = dataSet.types_hash_value();
        this->_tablesHashValue = dataSet.tables_hash_value();
        this->_tags = dataSet.tags();
        if ((this->ClientApplicationHost != nullptr))
        {
            delete this->ClientApplicationHost;
        }
        this->ClientApplicationHost = new CremaCode::StringTableTable(dataSet.tables()["ClientApplicationHost"]);
        if ((this->ClientBase != nullptr))
        {
            delete this->ClientBase;
        }
        this->ClientBase = new CremaCode::StringTableTable(dataSet.tables()["ClientBase"]);
        if ((this->ClientConsole != nullptr))
        {
            delete this->ClientConsole;
        }
        this->ClientConsole = new CremaCode::StringTableTable(dataSet.tables()["ClientConsole"]);
        if ((this->ClientConverters != nullptr))
        {
            delete this->ClientConverters;
        }
        this->ClientConverters = new CremaCode::StringTableTable(dataSet.tables()["ClientConverters"]);
        if ((this->ClientDifferences != nullptr))
        {
            delete this->ClientDifferences;
        }
        this->ClientDifferences = new CremaCode::StringTableTable(dataSet.tables()["ClientDifferences"]);
        if ((this->ClientFramework != nullptr))
        {
            delete this->ClientFramework;
        }
        this->ClientFramework = new CremaCode::StringTableTable(dataSet.tables()["ClientFramework"]);
        if ((this->ClientServices != nullptr))
        {
            delete this->ClientServices;
        }
        this->ClientServices = new CremaCode::StringTableTable(dataSet.tables()["ClientServices"]);
        if ((this->ClientSmartSet != nullptr))
        {
            delete this->ClientSmartSet;
        }
        this->ClientSmartSet = new CremaCode::StringTableTable(dataSet.tables()["ClientSmartSet"]);
        if ((this->ClientTables != nullptr))
        {
            delete this->ClientTables;
        }
        this->ClientTables = new CremaCode::StringTableTable(dataSet.tables()["ClientTables"]);
        if ((this->ClientTypes != nullptr))
        {
            delete this->ClientTypes;
        }
        this->ClientTypes = new CremaCode::StringTableTable(dataSet.tables()["ClientTypes"]);
        if ((this->ClientUsers != nullptr))
        {
            delete this->ClientUsers;
        }
        this->ClientUsers = new CremaCode::StringTableTable(dataSet.tables()["ClientUsers"]);
        if ((this->CommonPresentation != nullptr))
        {
            delete this->CommonPresentation;
        }
        this->CommonPresentation = new CremaCode::StringTableTable(dataSet.tables()["CommonPresentation"]);
        if ((this->CommonServiceModel != nullptr))
        {
            delete this->CommonServiceModel;
        }
        this->CommonServiceModel = new CremaCode::StringTableTable(dataSet.tables()["CommonServiceModel"]);
        if ((this->CommonServiceModel_Event != nullptr))
        {
            delete this->CommonServiceModel_Event;
        }
        this->CommonServiceModel_Event = new CremaCode::StringTableTable(dataSet.tables()["CommonServiceModel_Event"]);
        if ((this->CremaData != nullptr))
        {
            delete this->CremaData;
        }
        this->CremaData = new CremaCode::StringTableTable(dataSet.tables()["CremaData"]);
        if ((this->CremaProjectInfo != nullptr))
        {
            delete this->CremaProjectInfo;
        }
        this->CremaProjectInfo = new CremaCode::ProjectInfoTable(dataSet.tables()["CremaProjectInfo"]);
        if ((this->ModernUIFramework != nullptr))
        {
            delete this->ModernUIFramework;
        }
        this->ModernUIFramework = new CremaCode::StringTableTable(dataSet.tables()["ModernUIFramework"]);
        if ((this->NtreevLibrary != nullptr))
        {
            delete this->NtreevLibrary;
        }
        this->NtreevLibrary = new CremaCode::StringTableTable(dataSet.tables()["NtreevLibrary"]);
        if ((this->ProjectInfo != nullptr))
        {
            delete this->ProjectInfo;
        }
        this->ProjectInfo = new CremaCode::ProjectInfoTable(dataSet.tables()["ProjectInfo"]);
        if ((this->ServerServiceHosts != nullptr))
        {
            delete this->ServerServiceHosts;
        }
        this->ServerServiceHosts = new CremaCode::StringTableTable(dataSet.tables()["ServerServiceHosts"]);
        if ((this->StringTable != nullptr))
        {
            delete this->StringTable;
        }
        this->StringTable = new CremaCode::StringTableTable(dataSet.tables()["StringTable"]);
        if ((this->StringTable11 != nullptr))
        {
            delete this->StringTable11;
        }
        this->StringTable11 = new CremaCode::StringTableTable(dataSet.tables()["StringTable11"]);
        if ((this->TestTable != nullptr))
        {
            delete this->TestTable;
        }
        this->TestTable = new CremaCode::TestTableTable(dataSet.tables()["TestTable"]);
    }
    
}/*namespace CremaCode*/


