/// <reference path="./crema.d.ts" />
/// <reference path="./random.ts" />

class RandomDataBaseUtility {

    public static getTable(dataBaseName: string): string {
        return RandomUtility.nextItem(getTableList(dataBaseName));
    }

    public static getTableCategory(dataBaseName: string): string {
        return RandomUtility.nextItem(getTableCategoryList(dataBaseName));
    }

    public static getTableItem(dataBaseName: string): string {
        return RandomUtility.nextItem(getTableItemList(dataBaseName));
    }

    public static getType(dataBaseName: string): string {
        return RandomUtility.nextItem(getTypeList(dataBaseName));
    }

    public static getTypeCategory(dataBaseName: string): string {
        return RandomUtility.nextItem(getTypeCategoryList(dataBaseName));
    }

    public static getTypeItem(dataBaseName: string): string {
        return RandomUtility.nextItem(getTypeItemList(dataBaseName));
    }

    public static createTableCategory(dataBaseName: string): string {
        let categoryName: string = RandomUtility.nextIdentifier();
        let parentPath: string = RandomDataBaseUtility.getTableCategory(dataBaseName);
        let path: string = parentPath + categoryName + "/";
        if (!containsTableItem(dataBaseName, path)) {
            return createTableCategory(dataBaseName, parentPath, categoryName);
        }
        return null;
    }

    public static createTypeCategory(dataBaseName: string): string {
        let categoryName: string = RandomUtility.nextIdentifier();
        let parentPath: string = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        let path: string = parentPath + categoryName + "/";
        if (!containsTypeItem(dataBaseName, path)) {
            return createTypeCategory(dataBaseName, parentPath, categoryName);
        }
        return null;
    }

    public static createTable(dataBaseName: string): string {
        let tableName: string = RandomUtility.nextIdentifier();
        if (!containsTable(dataBaseName, tableName)) {
            let parentPath: string = RandomDataBaseUtility.getTableItem(dataBaseName);
            let domainID: string = beginTableCreate(dataBaseName, parentPath);
            try {
                setTableTemplateProperty(domainID, TableProperties.Name, tableName);
                for (let i: number = 0; i < RandomUtility.nextInRange(2, 15); i++) {
                    RandomTableTemplateUtility.createColumn(domainID);
                }
                if (RandomUtility.Within(50)) {
                    RandomTableTemplateUtility.setComment(domainID);
                }
                endTableTemplateEdit(domainID);
                return tableName;
            } catch (e) {
                cancelTableTemplateEdit(domainID);
                throw e;
            }
        }
        return null;
    }

    public static inheritTable(dataBaseName: string): string {
        let table: string = RandomDataBaseUtility.getTable(dataBaseName);
        let tableName: string = RandomUtility.nextIdentifier();
        let categoryPath: string = RandomDataBaseUtility.getTableCategory(dataBaseName);
        let copyContent: boolean = RandomUtility.Within(25) ? true : null;
        inheritTable(dataBaseName, table, tableName, categoryPath, copyContent);
        return tableName;
    }

    public static createType(dataBaseName: string): string {
        let typeName: string = RandomUtility.nextIdentifier();
        if (!containsType(dataBaseName, typeName)) {
            let categoryPath: string = RandomDataBaseUtility.getTypeCategory(dataBaseName);
            let domainID: string = beginTypeCreate(dataBaseName, categoryPath);
            try {
                setTypeTemplateProperty(domainID, TypeProperties.Name, typeName);
                for (let i: number = 0; i < RandomUtility.nextInRange(2, 15); i++) {
                    RandomTypeTemplateUtility.createMember(domainID);
                }
                if (RandomUtility.Within(50)) {
                    RandomTypeTemplateUtility.setComment(domainID);
                }
                endTypeTemplateEdit(domainID);
                return typeName;
            } catch (e) {
                cancelTypeTemplateEdit(domainID);
                throw e;
            }
        }
        return null;
    }

    public static changeTableTemplate(dataBaseName: string, tableName: string): string {
        let domainID: string = beginTableTemplateEdit(dataBaseName, tableName);
        try {
            for (let i: number = 0; i < RandomUtility.nextInRange(2, 15); i++) {
                try {
                    let percent: number = RandomUtility.next(100);
                    if (percent < 10) {
                        RandomTableTemplateUtility.deleteColumn(domainID);
                    } else if (percent < 30) {
                        RandomTableTemplateUtility.renameColumn(domainID);
                    } else if (percent < 40) {
                        RandomTableTemplateUtility.changeColumnIndex(domainID);
                    } else if (percent < 50) {
                        RandomTableTemplateUtility.changeIsKey(domainID);
                    } else if (percent < 60) {
                        RandomTableTemplateUtility.changeIsUnique(domainID);
                    } else if (percent < 70) {
                        RandomTableTemplateUtility.changeDataType(domainID);
                    } else if (percent < 80) {
                        RandomTableTemplateUtility.changeComment(domainID);
                    } else if (percent < 90) {
                        RandomTableTemplateUtility.createColumn(domainID);
                    } else if (percent < 95) {
                        RandomTableTemplateUtility.setComment(domainID);
                    } else {
                        throw "cancelTableTemplateEdit";
                    }
                } catch (e) {
                    log(e);
                }
            }
            endTableTemplateEdit(domainID);
            return tableName;
        } catch (e) {
            cancelTableTemplateEdit(domainID);
            throw e;
        }
    }

    public static changeTableContent(dataBaseName: string, tableName: string): string {
        let domainID: string = beginTableContentEdit(dataBaseName, tableName);
        enterTableContentEdit(domainID);
        try {
            for (let i: number = 0; i < RandomUtility.nextInRange(2, 100); i++) {
                try {
                    let percent: number = RandomUtility.next(100);
                    if (percent < 100) {
                        RandomTableContentUtility.createRow(domainID, dataBaseName, tableName);
                    } else if (percent < 90) {
                        RandomTableContentUtility.editRow(domainID, dataBaseName, tableName);
                    } else if (percent < 99) {
                        RandomTableContentUtility.deleteRow(domainID, dataBaseName, tableName);
                    } else {
                        throw "cancelTableContentEdit";
                    }
                } catch (e) {
                    log(e);
                }
            }
            leaveTableContentEdit(domainID);
            endTableContentEdit(domainID);
            return tableName;
        } catch (e) {
            cancelTableContentEdit(domainID);
            throw e;
        }
    }

    public static changeType(dataBaseName: string, typeName: string): string {
        let domainID: string = beginTypeTemplateEdit(dataBaseName, typeName);
        try {
            for (let i: number = 0; i < RandomUtility.nextInRange(2, 15); i++) {
                try {
                    let percent: number = RandomUtility.next(100);
                    if (percent < 10) {
                        RandomTypeTemplateUtility.deleteMember(domainID);
                    } else if (percent < 30) {
                        RandomTypeTemplateUtility.renameMember(domainID);
                    } else if (percent < 50) {
                        RandomTypeTemplateUtility.changeMemberValue(domainID);
                    } else if (percent < 70) {
                        RandomTypeTemplateUtility.changeMemberComment(domainID);
                    } else if (percent < 90) {
                        RandomTypeTemplateUtility.createMember(domainID);
                    } else if (percent < 95) {
                        RandomTypeTemplateUtility.setComment(domainID);
                    } else {
                        throw "cancelTypeTemplateEdit";
                    }
                } catch (e) {
                    log(e);
                }
            }
            endTypeTemplateEdit(domainID);
            return typeName;
        } catch (e) {
            cancelTypeTemplateEdit(domainID);
            throw e;
        }
    }

    public static renameTable(dataBaseName: string, tableName: string): string {
        let newTableName: string = RandomUtility.nextIdentifier();
        renameTable(dataBaseName, tableName, newTableName);
        return newTableName;
    }

    public static renameType(dataBaseName: string, typeName: string): string {
        let newTypeName: string = RandomUtility.nextIdentifier();
        renameType(dataBaseName, typeName, newTypeName);
        return newTypeName;
    }

    public static moveTable(dataBaseName: string, tableName: string): string {
        let parentPath: string = RandomDataBaseUtility.getTableCategory(dataBaseName);
        moveTable(dataBaseName, tableName, parentPath);
        return parentPath + tableName;
    }

    public static moveType(dataBaseName: string, typeName: string): string {
        let parentPath: string = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        moveType(dataBaseName, typeName, parentPath);
        return parentPath + typeName;
    }

    public static renameTableCategory(dataBaseName: string, categoryPath: string): string {
        let categoryName: string = RandomUtility.nextIdentifier();
        return renameTableCategory(dataBaseName, categoryPath, categoryName);
    }

    public static renameTypeCategory(dataBaseName: string, typeName: string): string {
        let newTypeCategoryName: string = RandomUtility.nextIdentifier();
        return renameTypeCategory(dataBaseName, typeName, newTypeCategoryName);
    }

    public static moveTableCategory(dataBaseName: string, categoryPath: string): string {
        let parentPath: string = RandomDataBaseUtility.getTableCategory(dataBaseName);
        return moveTableCategory(dataBaseName, categoryPath, parentPath);
    }

    public static moveTypeCategory(dataBaseName: string, typeName: string): string {
        let parentPath: string = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        return moveTypeCategory(dataBaseName, typeName, parentPath);
    }
}

class RandomTypeTemplateUtility {

    public static getMember(domainID: string): string {
        let members: string[] = getTypeTemplateMembers(domainID);
        if (members.length > 0) {
            return RandomUtility.nextItem(members);
        }
        return null;
    }

    public static setComment(domainID: string): void {
        if (RandomUtility.Within(75)) {
            setTypeTemplateProperty(domainID, TypeProperties.Comment, RandomUtility.nextString());
        } else {
            setTypeTemplateProperty(domainID, TypeProperties.Comment, null);
        }
    }

    public static createMember(domainID: string): string {
        let name: string = RandomUtility.nextIdentifier();
        let comment: string = null;
        if (RandomUtility.Within(25)) {
            comment = RandomUtility.nextString();
        }
        let value: number = RandomUtility.next(1000);
        addTypeTemplateMember(domainID, name, value, comment);
        return name;
    }

    public static deleteMember(domainID: string): void {
        let name: string = RandomTypeTemplateUtility.getMember(domainID);
        if (name) {
            deleteTypeTemplateMember(domainID, name);
        }
    }

    public static renameMember(domainID: string): void {
        let name: string = RandomTypeTemplateUtility.getMember(domainID);
        if (name) {
            let newName: string = RandomUtility.nextIdentifier();
            setTypeTemplateMemberProperty(domainID, name, TypeMemberProperties.Name, newName);
        }
    }

    public static changeMemberValue(domainID: string): void {
        let name: string = RandomTypeTemplateUtility.getMember(domainID);
        if (name) {
            let value: number = RandomUtility.next(1000);
            setTypeTemplateMemberProperty(domainID, name, TypeMemberProperties.Value, value);
        }
    }

    public static changeMemberComment(domainID: string): void {
        let name: string = RandomTypeTemplateUtility.getMember(domainID);
        if (name) {
            if (RandomUtility.Within(75)) {
                let comment: string = RandomUtility.nextString();
                setTypeTemplateMemberProperty(domainID, name, TypeMemberProperties.Comment, comment);
            } else {
                setTypeTemplateMemberProperty(domainID, name, TypeMemberProperties.Comment, null);
            }
        }
    }
}

class RandomTableTemplateUtility {

    public static getColumn(domainID: string): string {
        let columns: string[] = getTableTemplateColumns(domainID);
        if (columns.length > 0) {
            return RandomUtility.nextItem(columns);
        }
        return null;
    }

    public static setComment(domainID: string): void {
        if (RandomUtility.Within(75)) {
            setTableTemplateProperty(domainID, TableProperties.Comment, RandomUtility.nextString());
        } else {
            setTableTemplateProperty(domainID, TableProperties.Comment, null);
        }
    }

    public static createColumn(domainID: string): string {
        let name: string = RandomUtility.nextIdentifier();
        let comment: string = null;
        if (RandomUtility.Within(25)) {
            comment = RandomUtility.nextString();
        }
        let isKey: boolean = RandomUtility.Within(10) ? true : null;
        let typeName: string = RandomUtility.nextItem(getTableTemplateSelectableTypes(domainID));
        addTableTemplateColumn(domainID, name, typeName, comment, isKey);
        return name;
    }

    public static deleteColumn(domainID: string): void {
        let column: string = RandomTableTemplateUtility.getColumn(domainID);
        if (column) {
            deleteTableTemplateColumn(domainID, column);
        }
    }

    public static renameColumn(domainID: string): void {
        let column: string = RandomTableTemplateUtility.getColumn(domainID);
        if (column) {
            let newName: string = RandomUtility.nextIdentifier();
            setTableTemplateColumnProperty(domainID, column, TableColumnProperties.Name, newName);
        }
    }

    public static changeColumnIndex(domainID: string): void {
        let name: string = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            let columns: string[] = getTableTemplateColumns(domainID);
            let newIndex: number = RandomUtility.next(columns.length);
            setTableTemplateColumnProperty(domainID, name, TableColumnProperties.Index, newIndex);
        }
    }

    public static changeIsKey(domainID: string): void {
        let name: string = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            let isKey: boolean = RandomUtility.Within(10);
            setTableTemplateColumnProperty(domainID, name, TableColumnProperties.IsKey, isKey);
        }
    }

    public static changeIsUnique(domainID: string): void {
        let name: string = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            let isUnique: boolean = RandomUtility.Within(15);
            setTableTemplateColumnProperty(domainID, name, TableColumnProperties.IsUnique, isUnique);
        }
    }

    public static changeDataType(domainID: string): void {
        let name: string = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            let typeName: string = RandomUtility.nextItem(getTableTemplateSelectableTypes(domainID));
            setTableTemplateColumnProperty(domainID, name, TableColumnProperties.DataType, typeName);
        }
    }

    public static changeComment(domainID: string): void {
        let name: string = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            if (RandomUtility.Within(75)) {
                let comment: string = RandomUtility.nextString();
                setTableTemplateColumnProperty(domainID, name, TableColumnProperties.Comment, comment);
            } else {
                setTableTemplateColumnProperty(domainID, name, TableColumnProperties.Comment, null);
            }
        }
    }
}

class RandomTableContentUtility {
    public static createRow(domainID: string, dataBaseName: string, tableName: string): void {
        let tableInfo: { [key: string]: any; } = getTableInfo(dataBaseName, tableName);
        let columns: { [key: string]: any; }[] = tableInfo.Columns;
        let fields: { [key: string]: any; } = {};

        columns.forEach(element => {
            if (element.IsKey) {
                fields[element.Name] = RandomUtility.nextByType(element.DataType);
            } else if (!element.AllowNull) {
                fields[element.Name] = RandomUtility.nextByType(element.DataType);
            } else if (RandomUtility.Within(50)) {
                fields[element.Name] = RandomUtility.nextByType(element.DataType);
            }
        });

        addTableContentRow(domainID, tableName, fields);
    }

    public static editRow(domainID: string, dataBaseName: string, tableName: string): void {



    }

    public static deleteRow(domainID: string, dataBaseName: string, tableName: string): void {



    }

    public static editField(domainID: string, dataBaseName: string, tableName: string, fieldName: string): void {



    }
}

class RandomTask {

    public static createTable(dataBaseName: string): void {
        RandomDataBaseUtility.createTable(dataBaseName);
    }

    public static inheritTable(dataBaseName: string): void {
        RandomDataBaseUtility.inheritTable(dataBaseName);
    }

    public static createType(dataBaseName: string): void {
        RandomDataBaseUtility.createType(dataBaseName);
    }

    public static renameTable(dataBaseName: string): void {
        let tableName: string = RandomDataBaseUtility.getTable(dataBaseName);
        if (tableName) {
            RandomDataBaseUtility.renameTable(dataBaseName, tableName);
        }
    }

    public static renameType(dataBaseName: string): void {
        let typeName: string = RandomDataBaseUtility.getType(dataBaseName);
        if (typeName) {
            RandomDataBaseUtility.renameType(dataBaseName, typeName);
        }
    }

    public static moveTable(dataBaseName: string): void {
        let tableName: string = RandomDataBaseUtility.getTable(dataBaseName);
        if (tableName) {
            RandomDataBaseUtility.moveTable(dataBaseName, tableName);
        }
    }

    public static moveType(dataBaseName: string): void {
        let typeName: string = RandomDataBaseUtility.getType(dataBaseName);
        if (typeName) {
            RandomDataBaseUtility.moveType(dataBaseName, typeName);
        }
    }

    public static changeTableTemplate(dataBaseName: string): void {
        let tableName: string = RandomDataBaseUtility.getTable(dataBaseName);
        if (tableName) {
            RandomDataBaseUtility.changeTableTemplate(dataBaseName, tableName);
        }
    }

    public static changeTableContent(dataBaseName: string): void {
        let tableName: string = RandomDataBaseUtility.getTable(dataBaseName);
        if (tableName) {
            RandomDataBaseUtility.changeTableContent(dataBaseName, tableName);
        }
    }

    public static changeType(dataBaseName: string): void {
        let typeName: string = RandomDataBaseUtility.getType(dataBaseName);
        if (typeName) {
            RandomDataBaseUtility.changeType(dataBaseName, typeName);
        }
    }

    public static createTableCategory(dataBaseName: string): void {
        RandomDataBaseUtility.createTableCategory(dataBaseName);
    }

    public static createTypeCategory(dataBaseName: string): void {
        RandomDataBaseUtility.createTypeCategory(dataBaseName);
    }

    public static renameTableCategory(dataBaseName: string): void {
        let category: string = RandomDataBaseUtility.getTableCategory(dataBaseName);
        if (category) {
            RandomDataBaseUtility.renameTableCategory(dataBaseName, category);
        }
    }

    public static renameTypeCategory(dataBaseName: string): void {
        let category: string = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        if (category) {
            RandomDataBaseUtility.renameTypeCategory(dataBaseName, category);
        }
    }

    public static moveTableCategory(dataBaseName: string): void {
        let category: string = RandomDataBaseUtility.getTableCategory(dataBaseName);
        if (category) {
            RandomDataBaseUtility.moveTableCategory(dataBaseName, category);
        }
    }

    public static moveTypeCategory(dataBaseName: string): void {
        let category: string = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        if (category) {
            RandomDataBaseUtility.moveTypeCategory(dataBaseName, category);
        }
    }

    public static run(dataBaseName: string, count: number): void {
        let funcs: ((dataBaseName: string) => void)[] = [
            RandomTask.createTable,
            RandomTask.inheritTable,
            RandomTask.createType,
            RandomTask.renameTable,
            RandomTask.renameType,
            RandomTask.moveTable,
            RandomTask.moveType,
            RandomTask.changeTableTemplate,
            RandomTask.changeTableContent,
            RandomTask.changeType,
            RandomTask.createTableCategory,
            RandomTask.createTypeCategory,
            RandomTask.renameTableCategory,
            RandomTask.renameTypeCategory,
            RandomTask.moveTableCategory,
            RandomTask.moveTypeCategory
        ];
        for (let i: number = 0; i < count; i++) {
            try {
                let index: number = RandomUtility.next(funcs.length);
                funcs[index](dataBaseName);
            } catch (e) {
                log(e);
            }
        }
    }
}

class Program {
    public static main(): void {
        let dataBaseName: string = "default";
        let token: string = login("admin", "admin");

        if (isDataBaseLoaded(dataBaseName) === false) {
            loadDataBase(dataBaseName);
        }

        if (isDataBaseEntered(dataBaseName) === false) {
            enterDataBase(dataBaseName);
        }

        for (let i: number = 0; i < 10; i++) {
            let count: number = RandomUtility.nextInRange(10, 100);
            try {
                Program.doTask(dataBaseName, count);
            } finally {
                leaveDataBase(dataBaseName);
                logout(token);
            }
        }
    }

    private static doTask(dataBaseName: string, count: number): void {
        let transactionID: string = beginDataBaseTransaction(dataBaseName);
        try {
            RandomTask.run(dataBaseName, count);
            endDataBaseTransaction(transactionID);
        } catch (e) {
            cancelDataBaseTransaction(transactionID);
            throw e;
        }
    }
}

Program.main();