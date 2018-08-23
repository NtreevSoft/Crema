/// <reference path="./crema.d.ts" />
/// <reference path="./random.ts" />

class CreateTest {
    public static main(): void {
        let dataBaseName: string = "test";
        let token: string = login("admin", "admin");

        let index: number = 0;
        while (containsDataBase(dataBaseName + index)) {
            index++;
        }
        dataBaseName = dataBaseName + index;
        createDataBase(dataBaseName, "create test");

        if (isDataBaseLoaded(dataBaseName) === false) {
            loadDataBase(dataBaseName);
        }

        if (isDataBaseEntered(dataBaseName) === false) {
            enterDataBase(dataBaseName);
        }

        {
            let domainID: string = beginTypeCreate(dataBaseName, "/");
            setTypeTemplateProperty(domainID, TypeProperties.Name, "BaseType");
            addTypeTemplateMember(domainID, "None", 0, "");
            addTypeTemplateMember(domainID, "A", 1, "");
            addTypeTemplateMember(domainID, "B", 2, "");
            addTypeTemplateMember(domainID, "C", 3, "");
            endTypeTemplateEdit(domainID);
        }

        {
            let domainID: string = beginTypeCreate(dataBaseName, "/");
            setTypeTemplateProperty(domainID, TypeProperties.Name, "FlagTyoe");
            setTypeTemplateProperty(domainID, TypeProperties.IsFlag, true);
            addTypeTemplateMember(domainID, "None", 0, "");
            addTypeTemplateMember(domainID, "A", 1, "");
            addTypeTemplateMember(domainID, "B", 2, "");
            addTypeTemplateMember(domainID, "C", 4, "");
            addTypeTemplateMember(domainID, "D", 8, "");
            addTypeTemplateMember(domainID, "All", 15, "");
            endTypeTemplateEdit(domainID);
        }

        createTableCategory(dataBaseName, "/", "category");

        let tableName: string = "";
        {
            let domainID: string = beginTableCreate(dataBaseName, "/");
            addTableTemplateColumn(domainID, "index", "string", "key column", true);
            setTableTemplateColumnProperty(domainID, "index", TableColumnProperties.AutoIncrement, true);
            addTableTemplateColumn(domainID, "boolean", "boolean", "");
            addTableTemplateColumn(domainID, "string", "string", "");
            addTableTemplateColumn(domainID, "float", "float", "");
            addTableTemplateColumn(domainID, "double", "double", "");
            addTableTemplateColumn(domainID, "int8", "int8", "");
            addTableTemplateColumn(domainID, "uint8", "uint8", "");
            addTableTemplateColumn(domainID, "int16", "int16", "");
            addTableTemplateColumn(domainID, "uint16", "uint16", "");
            addTableTemplateColumn(domainID, "int32", "int32", "");
            addTableTemplateColumn(domainID, "uint32", "uint32", "");
            addTableTemplateColumn(domainID, "int64", "int64", "");
            addTableTemplateColumn(domainID, "uint64", "uint64", "");
            addTableTemplateColumn(domainID, "datetime", "datetime", "");
            addTableTemplateColumn(domainID, "duration", "duration", "");
            addTableTemplateColumn(domainID, "guid", "guid", "");
            tableName = endTableTemplateEdit(domainID);
        }

        {
            let domainID: string = beginTableContentEdit(dataBaseName, tableName);
            enterTableContentEdit(domainID);
            for (let i: number = 0; i < 100; i++) {
                let fields: { [key: string]: any; } = {};
                fields.boolean = RandomUtility.nextBoolean();
                fields.string = RandomUtility.nextString();
                fields.float = RandomUtility.nextFloat();
                fields.double = RandomUtility.nextDouble();
                fields.int8 = RandomUtility.nextInt8();
                fields.uint8 = RandomUtility.nextUInt8();
                fields.int16 = RandomUtility.nextInt16();
                fields.uint16 = RandomUtility.nextUInt16();
                fields.int32 = RandomUtility.nextInt32();
                fields.uint32 = RandomUtility.nextUInt32();
                fields.int64 = RandomUtility.nextInt64();
                fields.uint64 = RandomUtility.nextUInt64();
                fields.datetime = RandomUtility.nextDateTime();
                fields.duration = RandomUtility.nextDuration();
                fields.guid = RandomUtility.nextGuid();
                addTableContentRow(domainID, tableName, fields);
            }
            leaveTableContentEdit(domainID);
            endTableContentEdit(domainID);
        }
        leaveDataBase(dataBaseName);
        unloadDataBase(dataBaseName);
    }
}

CreateTest.main();