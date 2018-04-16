#pragma once
#include "reader/include/crema/inidata.h"
#include "crema_types.h"
#include "crema_base.h"

namespace CremaCode
{
    class ProjectInfoExportInfoRow : public CremaCode::CremaRow
    {
    public: 
        // Creator: admin
        // CreatedDateTime: 2017-10-26 오전 6:21:11
        // Modifier: admin
        // ModifiedDateTime: 2017-10-26 오전 6:21:41
        int ID = ((int)(0));
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:35:20
        // Modifier: admin
        // ModifiedDateTime: 2017-10-26 오전 8:42:54
        std::string FileName;
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:35:02
        // Modifier: admin
        // ModifiedDateTime: 2017-10-26 오전 6:20:29
        std::string TableName;
        const class ProjectInfoExportInfoTable* Table;
        const class ProjectInfoRow* Parent;
    public: 
        ProjectInfoExportInfoRow(CremaCode::reader::irow& row, ProjectInfoExportInfoTable* table);
    };
    // CreatedDateTime: 2017-10-25 오전 8:34:43
    // ModifiedDateTime: 2017-10-26 오전 8:06:01
    // ContentsModifier: s2quake
    // ContentsModifiedDateTime: 2018-03-27 오전 5:13:04
    class ProjectInfoExportInfoTable : public CremaCode::CremaTable<ProjectInfoExportInfoRow>
    {
    public: 
        ProjectInfoExportInfoTable();
        ProjectInfoExportInfoTable(CremaCode::reader::itable& table);
        ProjectInfoExportInfoTable(std::string name, std::vector<class ProjectInfoExportInfoRow*> rows);
    public: 
        virtual ~ProjectInfoExportInfoTable();
    protected: 
        virtual void* CreateRow(CremaCode::reader::irow& row, void* table);
    public: 
        const ProjectInfoExportInfoRow* Find(int ID) const;
    };
    class ProjectInfoRow : public CremaCode::CremaRow
    {
    public: 
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:40:56
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 8:22:11
        CremaCode::KindType KindType = ((CremaCode::KindType)(0));
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 8:22:24
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 8:23:24
        CremaCode::ProjectType ProjectType = ((CremaCode::ProjectType)(0));
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:38:54
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 8:42:24
        std::string Name;
        // Creator: admin
        // CreatedDateTime: 2017-10-25 오전 8:25:15
        // Modifier: admin
        // ModifiedDateTime: 2017-10-25 오전 8:42:25
        std::string ProjectPath;
        const class ProjectInfoTable* Table;
        const ProjectInfoExportInfoTable* ExportInfo;
    private: 
        static ProjectInfoExportInfoTable ExportInfoEmpty;
    public: 
        ProjectInfoRow(CremaCode::reader::irow& row, ProjectInfoTable* table);
    friend static void ProjectInfoSetExportInfo(ProjectInfoRow* target, const std::string& childName, const std::vector<ProjectInfoExportInfoRow*>& childs);
    };
    // Modifier: admin
    // ModifiedDateTime: 2017-11-01 오전 8:22:24
    // ContentsModifier: s2quake
    // ContentsModifiedDateTime: 2018-03-27 오전 5:12:57
    class ProjectInfoTable : public CremaCode::CremaTable<ProjectInfoRow>
    {
    public: 
        const ProjectInfoExportInfoTable* ExportInfo;
    public: 
        ProjectInfoTable();
        ProjectInfoTable(CremaCode::reader::itable& table);
    public: 
        virtual ~ProjectInfoTable();
    protected: 
        virtual void* CreateRow(CremaCode::reader::irow& row, void* table);
    public: 
        const ProjectInfoRow* Find(CremaCode::KindType KindType, CremaCode::ProjectType ProjectType, const std::string& Name) const;
    };
    class StringTableRow : public CremaCode::CremaRow
    {
    public: 
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 7:16:08
        // Modifier: admin
        // ModifiedDateTime: 2017-10-23 오전 7:16:24
        CremaCode::StringType Type = ((CremaCode::StringType)(0));
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 7:16:14
        // Modifier: admin
        // ModifiedDateTime: 2017-10-23 오전 7:16:25
        std::string Name;
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 7:16:18
        // Modifier: admin
        // ModifiedDateTime: 2017-10-23 오전 7:16:18
        std::string Comment;
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 7:16:22
        // Modifier: admin
        // ModifiedDateTime: 2017-10-23 오전 7:16:22
        std::string Value;
        // Creator: admin
        // CreatedDateTime: 2017-10-23 오전 9:20:19
        // Modifier: admin
        // ModifiedDateTime: 2017-10-23 오전 9:20:19
        std::string ko_KR;
        const class StringTableTable* Table;
    public: 
        StringTableRow(CremaCode::reader::irow& row, StringTableTable* table);
    };
    // Creator: admin
    // CreatedDateTime: 2017-10-23 오전 7:16:01
    // Modifier: admin
    // ModifiedDateTime: 2017-10-25 오전 5:30:21
    class StringTableTable : public CremaCode::CremaTable<StringTableRow>
    {
    public: 
        StringTableTable();
        StringTableTable(CremaCode::reader::itable& table);
    public: 
        virtual ~StringTableTable();
    protected: 
        virtual void* CreateRow(CremaCode::reader::irow& row, void* table);
    public: 
        const class StringTableRow* Find(CremaCode::StringType Type, const std::string& Name) const;
    };
    class TestTableRow : public CremaCode::CremaRow
    {
    public: 
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 2:37:44
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 2:38:12
        int Key = ((int)(0));
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 2:37:57
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 2:37:57
        std::string Value;
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 2:38:09
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 2:38:09
        time_t DateTimePicker = ((time_t)(0));
        // Creator: admin
        // CreatedDateTime: 2017-11-01 오전 4:10:41
        // Modifier: admin
        // ModifiedDateTime: 2017-11-01 오전 4:10:41
        int TimePicker = ((int)(0));
        const class TestTableTable* Table;
    public: 
        TestTableRow(CremaCode::reader::irow& row, TestTableTable* table);
    };
    // Modifier: admin
    // ModifiedDateTime: 2017-11-01 오전 4:10:41
    // ContentsModifier: admin
    // ContentsModifiedDateTime: 2017-11-01 오전 4:21:18
    class TestTableTable : public CremaCode::CremaTable<TestTableRow>
    {
    public: 
        TestTableTable();
        TestTableTable(CremaCode::reader::itable& table);
    public: 
        virtual ~TestTableTable();
    protected: 
        virtual void* CreateRow(CremaCode::reader::irow& row, void* table);
    public: 
        const class TestTableRow* Find(int Key) const;
    };
    class CremaDataSet : public CremaCode::CremaData
    {
    public: 
        const StringTableTable* ClientApplicationHost;
        const StringTableTable* ClientBase;
        const StringTableTable* ClientConsole;
        const StringTableTable* ClientConverters;
        const StringTableTable* ClientDifferences;
        const StringTableTable* ClientFramework;
        const StringTableTable* ClientServices;
        const StringTableTable* ClientSmartSet;
        const StringTableTable* ClientTables;
        const StringTableTable* ClientTypes;
        const StringTableTable* ClientUsers;
        const StringTableTable* CommonPresentation;
        const StringTableTable* CommonServiceModel;
        const StringTableTable* CommonServiceModel_Event;
        const StringTableTable* CremaData;
        const ProjectInfoTable* CremaProjectInfo;
        const StringTableTable* ModernUIFramework;
        const StringTableTable* NtreevLibrary;
        const ProjectInfoTable* ProjectInfo;
        const StringTableTable* ServerServiceHosts;
        const StringTableTable* StringTable;
        const StringTableTable* StringTable11;
        const TestTableTable* TestTable;
    private: 
        std::string _name;
        long long _revision;
        std::string _typesHashValue;
        std::string _tablesHashValue;
        std::string _tags;
    public: 
        CremaDataSet(CremaCode::reader::idataset& dataSet, bool verifyRevision);
        CremaDataSet(const std::string& filename, bool verifyRevision);
    public: 
        virtual ~CremaDataSet();
    public: 
        std::string name();
        long long revision();
        std::string typesHashValue();
        std::string tablesHashValue();
        std::string tags();
        void Load(const std::string& filename, bool verifyRevision);
        void Load(CremaCode::reader::idataset& dataSet, bool verifyRevision);
    };
}/*namespace CremaCode*/

