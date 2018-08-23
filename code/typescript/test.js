var RandomUtility = /** @class */ (function () {
    function RandomUtility() {
    }
    RandomUtility.next = function (maxValue) {
        return Math.floor((Math.random() * maxValue));
    };
    RandomUtility.nextInRange = function (minValue, maxValue) {
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    };
    RandomUtility.nextIdentifier = function () {
        var text = "";
        var first = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var count = RandomUtility.nextInRange(5, 15);
        text += possible.charAt(RandomUtility.next(first.length));
        for (var i = 0; i < count; i++) {
            text += possible.charAt(RandomUtility.next(possible.length));
        }
        return text;
    };
    RandomUtility.nextItem = function (items) {
        var index = RandomUtility.next(items.length);
        return items[index];
    };
    RandomUtility.Within = function (percent) {
        return percent >= RandomUtility.next(100);
    };
    RandomUtility.nextBoolean = function () {
        return RandomUtility.next(2) === 0;
    };
    RandomUtility.nextString = function () {
        var count = RandomUtility.nextInRange(2, 5);
        var list = [];
        for (var i = 0; i < count; i++) {
            list.push(RandomUtility.nextIdentifier());
        }
        return list.join(" ");
    };
    RandomUtility.nextFloat = function () {
        var minValue = -3.40282347E+38;
        var maxValue = 3.40282347E+38;
        return Math.random() * (maxValue - minValue) + minValue;
    };
    RandomUtility.nextDouble = function () {
        return Math.random();
    };
    RandomUtility.nextInt8 = function () {
        var minValue = -128;
        var maxValue = 127;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    };
    RandomUtility.nextUInt8 = function () {
        var minValue = 0;
        var maxValue = 255;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    };
    RandomUtility.nextInt16 = function () {
        var minValue = -32768;
        var maxValue = 32767;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    };
    RandomUtility.nextUInt16 = function () {
        var minValue = 0;
        var maxValue = 65535;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    };
    RandomUtility.nextInt32 = function () {
        var minValue = -2147483648;
        var maxValue = 2147483647;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    };
    RandomUtility.nextUInt32 = function () {
        var minValue = 0;
        var maxValue = 4294967295;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    };
    RandomUtility.nextInt64 = function () {
        var minValue = -9223372036854775808;
        var maxValue = 9223372036854775807;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    };
    RandomUtility.nextUInt64 = function () {
        var minValue = 0;
        var maxValue = 18446744073709551615;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    };
    RandomUtility.nextDateTime = function () {
        return new Date(RandomUtility.next(253402300800000));
    };
    RandomUtility.nextDuration = function () {
        var minValue = -922337203685.47754;
        var maxValue = 922337203685.47754;
        return Math.random() * (maxValue - minValue) + minValue;
    };
    RandomUtility.nextGuid = function () {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        }
        return s4() + s4() + "-" + s4() + "-" + s4() + "-" + s4() + "-" + s4() + s4() + s4();
    };
    RandomUtility.nextByType = function (typeName) {
        switch (typeName) {
            case "boolean":
                return RandomUtility.nextBoolean();
            case "string":
                return RandomUtility.nextString();
            case "float":
                return RandomUtility.nextFloat();
            case "double":
                return RandomUtility.nextDouble();
            case "int8":
                return RandomUtility.nextInt8();
            case "uint8":
                return RandomUtility.nextUInt8();
            case "int16":
                return RandomUtility.nextInt16();
            case "uint16":
                return RandomUtility.nextUInt16();
            case "int32":
                return RandomUtility.nextInt32();
            case "uint32":
                return RandomUtility.nextUInt32();
            case "int64":
                return RandomUtility.nextInt64();
            case "uint64":
                return RandomUtility.nextUInt64();
            case "datetime":
                return RandomUtility.nextDateTime();
            case "duration":
                return RandomUtility.nextDuration();
            case "guid":
                return RandomUtility.nextGuid();
            default:
                throw "not implemented.";
        }
    };
    return RandomUtility;
}());
/// <reference path="./crema.d.ts" />
/// <reference path="./random.ts" />
var RandomDataBaseUtility = /** @class */ (function () {
    function RandomDataBaseUtility() {
    }
    RandomDataBaseUtility.getTable = function (dataBaseName) {
        return RandomUtility.nextItem(getTableList(dataBaseName));
    };
    RandomDataBaseUtility.getTableCategory = function (dataBaseName) {
        return RandomUtility.nextItem(getTableCategoryList(dataBaseName));
    };
    RandomDataBaseUtility.getTableItem = function (dataBaseName) {
        return RandomUtility.nextItem(getTableItemList(dataBaseName));
    };
    RandomDataBaseUtility.getType = function (dataBaseName) {
        return RandomUtility.nextItem(getTypeList(dataBaseName));
    };
    RandomDataBaseUtility.getTypeCategory = function (dataBaseName) {
        return RandomUtility.nextItem(getTypeCategoryList(dataBaseName));
    };
    RandomDataBaseUtility.getTypeItem = function (dataBaseName) {
        return RandomUtility.nextItem(getTypeItemList(dataBaseName));
    };
    RandomDataBaseUtility.createTableCategory = function (dataBaseName) {
        var categoryName = RandomUtility.nextIdentifier();
        var parentPath = RandomDataBaseUtility.getTableCategory(dataBaseName);
        var path = parentPath + categoryName + "/";
        if (!containsTableItem(dataBaseName, path)) {
            return createTableCategory(dataBaseName, parentPath, categoryName);
        }
        return null;
    };
    RandomDataBaseUtility.createTypeCategory = function (dataBaseName) {
        var categoryName = RandomUtility.nextIdentifier();
        var parentPath = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        var path = parentPath + categoryName + "/";
        if (!containsTypeItem(dataBaseName, path)) {
            return createTypeCategory(dataBaseName, parentPath, categoryName);
        }
        return null;
    };
    RandomDataBaseUtility.createTable = function (dataBaseName) {
        var tableName = RandomUtility.nextIdentifier();
        if (!containsTable(dataBaseName, tableName)) {
            var parentPath = RandomDataBaseUtility.getTableItem(dataBaseName);
            var domainID = beginTableCreate(dataBaseName, parentPath);
            try {
                setTableTemplateProperty(domainID, TableProperties.Name, tableName);
                for (var i = 0; i < RandomUtility.nextInRange(2, 15); i++) {
                    RandomTableTemplateUtility.createColumn(domainID);
                }
                if (RandomUtility.Within(50)) {
                    RandomTableTemplateUtility.setComment(domainID);
                }
                endTableTemplateEdit(domainID);
                return tableName;
            }
            catch (e) {
                cancelTableTemplateEdit(domainID);
                throw e;
            }
        }
        return null;
    };
    RandomDataBaseUtility.inheritTable = function (dataBaseName) {
        var table = RandomDataBaseUtility.getTable(dataBaseName);
        var tableName = RandomUtility.nextIdentifier();
        var categoryPath = RandomDataBaseUtility.getTableCategory(dataBaseName);
        var copyContent = RandomUtility.Within(25) ? true : null;
        inheritTable(dataBaseName, table, tableName, categoryPath, copyContent);
        return tableName;
    };
    RandomDataBaseUtility.createType = function (dataBaseName) {
        var typeName = RandomUtility.nextIdentifier();
        if (!containsType(dataBaseName, typeName)) {
            var categoryPath = RandomDataBaseUtility.getTypeCategory(dataBaseName);
            var domainID = beginTypeCreate(dataBaseName, categoryPath);
            try {
                setTypeTemplateProperty(domainID, TypeProperties.Name, typeName);
                for (var i = 0; i < RandomUtility.nextInRange(2, 15); i++) {
                    RandomTypeTemplateUtility.createMember(domainID);
                }
                if (RandomUtility.Within(50)) {
                    RandomTypeTemplateUtility.setComment(domainID);
                }
                endTypeTemplateEdit(domainID);
                return typeName;
            }
            catch (e) {
                cancelTypeTemplateEdit(domainID);
                throw e;
            }
        }
        return null;
    };
    RandomDataBaseUtility.changeTableTemplate = function (dataBaseName, tableName) {
        var domainID = beginTableTemplateEdit(dataBaseName, tableName);
        try {
            for (var i = 0; i < RandomUtility.nextInRange(2, 15); i++) {
                try {
                    var percent = RandomUtility.next(100);
                    if (percent < 10) {
                        RandomTableTemplateUtility.deleteColumn(domainID);
                    }
                    else if (percent < 30) {
                        RandomTableTemplateUtility.renameColumn(domainID);
                    }
                    else if (percent < 40) {
                        RandomTableTemplateUtility.changeColumnIndex(domainID);
                    }
                    else if (percent < 50) {
                        RandomTableTemplateUtility.changeIsKey(domainID);
                    }
                    else if (percent < 60) {
                        RandomTableTemplateUtility.changeIsUnique(domainID);
                    }
                    else if (percent < 70) {
                        RandomTableTemplateUtility.changeDataType(domainID);
                    }
                    else if (percent < 80) {
                        RandomTableTemplateUtility.changeComment(domainID);
                    }
                    else if (percent < 90) {
                        RandomTableTemplateUtility.createColumn(domainID);
                    }
                    else if (percent < 95) {
                        RandomTableTemplateUtility.setComment(domainID);
                    }
                    else {
                        throw "cancelTableTemplateEdit";
                    }
                }
                catch (e) {
                    log(e);
                }
            }
            endTableTemplateEdit(domainID);
            return tableName;
        }
        catch (e) {
            cancelTableTemplateEdit(domainID);
            throw e;
        }
    };
    RandomDataBaseUtility.changeTableContent = function (dataBaseName, tableName) {
        var domainID = beginTableContentEdit(dataBaseName, tableName);
        enterTableContentEdit(domainID);
        try {
            for (var i = 0; i < RandomUtility.nextInRange(2, 100); i++) {
                try {
                    var percent = RandomUtility.next(100);
                    if (percent < 100) {
                        RandomTableContentUtility.createRow(domainID, dataBaseName, tableName);
                    }
                    else if (percent < 90) {
                        RandomTableContentUtility.editRow(domainID, dataBaseName, tableName);
                    }
                    else if (percent < 99) {
                        RandomTableContentUtility.deleteRow(domainID, dataBaseName, tableName);
                    }
                    else {
                        throw "cancelTableContentEdit";
                    }
                }
                catch (e) {
                    log(e);
                }
            }
            leaveTableContentEdit(domainID);
            endTableContentEdit(domainID);
            return tableName;
        }
        catch (e) {
            cancelTableContentEdit(domainID);
            throw e;
        }
    };
    RandomDataBaseUtility.changeType = function (dataBaseName, typeName) {
        var domainID = beginTypeTemplateEdit(dataBaseName, typeName);
        try {
            for (var i = 0; i < RandomUtility.nextInRange(2, 15); i++) {
                try {
                    var percent = RandomUtility.next(100);
                    if (percent < 10) {
                        RandomTypeTemplateUtility.deleteMember(domainID);
                    }
                    else if (percent < 30) {
                        RandomTypeTemplateUtility.renameMember(domainID);
                    }
                    else if (percent < 50) {
                        RandomTypeTemplateUtility.changeMemberValue(domainID);
                    }
                    else if (percent < 70) {
                        RandomTypeTemplateUtility.changeMemberComment(domainID);
                    }
                    else if (percent < 90) {
                        RandomTypeTemplateUtility.createMember(domainID);
                    }
                    else if (percent < 95) {
                        RandomTypeTemplateUtility.setComment(domainID);
                    }
                    else {
                        throw "cancelTypeTemplateEdit";
                    }
                }
                catch (e) {
                    log(e);
                }
            }
            endTypeTemplateEdit(domainID);
            return typeName;
        }
        catch (e) {
            cancelTypeTemplateEdit(domainID);
            throw e;
        }
    };
    RandomDataBaseUtility.renameTable = function (dataBaseName, tableName) {
        var newTableName = RandomUtility.nextIdentifier();
        renameTable(dataBaseName, tableName, newTableName);
        return newTableName;
    };
    RandomDataBaseUtility.renameType = function (dataBaseName, typeName) {
        var newTypeName = RandomUtility.nextIdentifier();
        renameType(dataBaseName, typeName, newTypeName);
        return newTypeName;
    };
    RandomDataBaseUtility.moveTable = function (dataBaseName, tableName) {
        var parentPath = RandomDataBaseUtility.getTableCategory(dataBaseName);
        moveTable(dataBaseName, tableName, parentPath);
        return parentPath + tableName;
    };
    RandomDataBaseUtility.moveType = function (dataBaseName, typeName) {
        var parentPath = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        moveType(dataBaseName, typeName, parentPath);
        return parentPath + typeName;
    };
    RandomDataBaseUtility.renameTableCategory = function (dataBaseName, categoryPath) {
        var categoryName = RandomUtility.nextIdentifier();
        return renameTableCategory(dataBaseName, categoryPath, categoryName);
    };
    RandomDataBaseUtility.renameTypeCategory = function (dataBaseName, typeName) {
        var newTypeCategoryName = RandomUtility.nextIdentifier();
        return renameTypeCategory(dataBaseName, typeName, newTypeCategoryName);
    };
    RandomDataBaseUtility.moveTableCategory = function (dataBaseName, categoryPath) {
        var parentPath = RandomDataBaseUtility.getTableCategory(dataBaseName);
        return moveTableCategory(dataBaseName, categoryPath, parentPath);
    };
    RandomDataBaseUtility.moveTypeCategory = function (dataBaseName, typeName) {
        var parentPath = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        return moveTypeCategory(dataBaseName, typeName, parentPath);
    };
    return RandomDataBaseUtility;
}());
var RandomTypeTemplateUtility = /** @class */ (function () {
    function RandomTypeTemplateUtility() {
    }
    RandomTypeTemplateUtility.getMember = function (domainID) {
        var members = getTypeTemplateMembers(domainID);
        if (members.length > 0) {
            return RandomUtility.nextItem(members);
        }
        return null;
    };
    RandomTypeTemplateUtility.setComment = function (domainID) {
        if (RandomUtility.Within(75)) {
            setTypeTemplateProperty(domainID, TypeProperties.Comment, RandomUtility.nextString());
        }
        else {
            setTypeTemplateProperty(domainID, TypeProperties.Comment, null);
        }
    };
    RandomTypeTemplateUtility.createMember = function (domainID) {
        var name = RandomUtility.nextIdentifier();
        var comment = null;
        if (RandomUtility.Within(25)) {
            comment = RandomUtility.nextString();
        }
        var value = RandomUtility.next(1000);
        addTypeTemplateMember(domainID, name, value, comment);
        return name;
    };
    RandomTypeTemplateUtility.deleteMember = function (domainID) {
        var name = RandomTypeTemplateUtility.getMember(domainID);
        if (name) {
            deleteTypeTemplateMember(domainID, name);
        }
    };
    RandomTypeTemplateUtility.renameMember = function (domainID) {
        var name = RandomTypeTemplateUtility.getMember(domainID);
        if (name) {
            var newName = RandomUtility.nextIdentifier();
            setTypeTemplateMemberProperty(domainID, name, TypeMemberProperties.Name, newName);
        }
    };
    RandomTypeTemplateUtility.changeMemberValue = function (domainID) {
        var name = RandomTypeTemplateUtility.getMember(domainID);
        if (name) {
            var value = RandomUtility.next(1000);
            setTypeTemplateMemberProperty(domainID, name, TypeMemberProperties.Value, value);
        }
    };
    RandomTypeTemplateUtility.changeMemberComment = function (domainID) {
        var name = RandomTypeTemplateUtility.getMember(domainID);
        if (name) {
            if (RandomUtility.Within(75)) {
                var comment = RandomUtility.nextString();
                setTypeTemplateMemberProperty(domainID, name, TypeMemberProperties.Comment, comment);
            }
            else {
                setTypeTemplateMemberProperty(domainID, name, TypeMemberProperties.Comment, null);
            }
        }
    };
    return RandomTypeTemplateUtility;
}());
var RandomTableTemplateUtility = /** @class */ (function () {
    function RandomTableTemplateUtility() {
    }
    RandomTableTemplateUtility.getColumn = function (domainID) {
        var columns = getTableTemplateColumns(domainID);
        if (columns.length > 0) {
            return RandomUtility.nextItem(columns);
        }
        return null;
    };
    RandomTableTemplateUtility.setComment = function (domainID) {
        if (RandomUtility.Within(75)) {
            setTableTemplateProperty(domainID, TableProperties.Comment, RandomUtility.nextString());
        }
        else {
            setTableTemplateProperty(domainID, TableProperties.Comment, null);
        }
    };
    RandomTableTemplateUtility.createColumn = function (domainID) {
        var name = RandomUtility.nextIdentifier();
        var comment = null;
        if (RandomUtility.Within(25)) {
            comment = RandomUtility.nextString();
        }
        var isKey = RandomUtility.Within(10) ? true : null;
        var typeName = RandomUtility.nextItem(getTableTemplateSelectableTypes(domainID));
        addTableTemplateColumn(domainID, name, typeName, comment, isKey);
        return name;
    };
    RandomTableTemplateUtility.deleteColumn = function (domainID) {
        var column = RandomTableTemplateUtility.getColumn(domainID);
        if (column) {
            deleteTableTemplateColumn(domainID, column);
        }
    };
    RandomTableTemplateUtility.renameColumn = function (domainID) {
        var column = RandomTableTemplateUtility.getColumn(domainID);
        if (column) {
            var newName = RandomUtility.nextIdentifier();
            setTableTemplateColumnProperty(domainID, column, TableColumnProperties.Name, newName);
        }
    };
    RandomTableTemplateUtility.changeColumnIndex = function (domainID) {
        var name = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            var columns = getTableTemplateColumns(domainID);
            var newIndex = RandomUtility.next(columns.length);
            setTableTemplateColumnProperty(domainID, name, TableColumnProperties.Index, newIndex);
        }
    };
    RandomTableTemplateUtility.changeIsKey = function (domainID) {
        var name = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            var isKey = RandomUtility.Within(10);
            setTableTemplateColumnProperty(domainID, name, TableColumnProperties.IsKey, isKey);
        }
    };
    RandomTableTemplateUtility.changeIsUnique = function (domainID) {
        var name = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            var isUnique = RandomUtility.Within(15);
            setTableTemplateColumnProperty(domainID, name, TableColumnProperties.IsUnique, isUnique);
        }
    };
    RandomTableTemplateUtility.changeDataType = function (domainID) {
        var name = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            var typeName = RandomUtility.nextItem(getTableTemplateSelectableTypes(domainID));
            setTableTemplateColumnProperty(domainID, name, TableColumnProperties.DataType, typeName);
        }
    };
    RandomTableTemplateUtility.changeComment = function (domainID) {
        var name = RandomTableTemplateUtility.getColumn(domainID);
        if (name) {
            if (RandomUtility.Within(75)) {
                var comment = RandomUtility.nextString();
                setTableTemplateColumnProperty(domainID, name, TableColumnProperties.Comment, comment);
            }
            else {
                setTableTemplateColumnProperty(domainID, name, TableColumnProperties.Comment, null);
            }
        }
    };
    return RandomTableTemplateUtility;
}());
var RandomTableContentUtility = /** @class */ (function () {
    function RandomTableContentUtility() {
    }
    RandomTableContentUtility.createRow = function (domainID, dataBaseName, tableName) {
        var tableInfo = getTableInfo(dataBaseName, tableName);
        var columns = tableInfo.Columns;
        var fields = {};
        columns.forEach(function (element) {
            if (element.IsKey) {
                fields[element.Name] = RandomUtility.nextByType(element.DataType);
            }
            else if (!element.AllowNull) {
                fields[element.Name] = RandomUtility.nextByType(element.DataType);
            }
            else if (RandomUtility.Within(50)) {
                fields[element.Name] = RandomUtility.nextByType(element.DataType);
            }
        });
        addTableContentRow(domainID, tableName, fields);
    };
    RandomTableContentUtility.editRow = function (domainID, dataBaseName, tableName) {
    };
    RandomTableContentUtility.deleteRow = function (domainID, dataBaseName, tableName) {
    };
    RandomTableContentUtility.editField = function (domainID, dataBaseName, tableName, fieldName) {
    };
    return RandomTableContentUtility;
}());
var RandomTask = /** @class */ (function () {
    function RandomTask() {
    }
    RandomTask.createTable = function (dataBaseName) {
        RandomDataBaseUtility.createTable(dataBaseName);
    };
    RandomTask.inheritTable = function (dataBaseName) {
        RandomDataBaseUtility.inheritTable(dataBaseName);
    };
    RandomTask.createType = function (dataBaseName) {
        RandomDataBaseUtility.createType(dataBaseName);
    };
    RandomTask.renameTable = function (dataBaseName) {
        var tableName = RandomDataBaseUtility.getTable(dataBaseName);
        if (tableName) {
            RandomDataBaseUtility.renameTable(dataBaseName, tableName);
        }
    };
    RandomTask.renameType = function (dataBaseName) {
        var typeName = RandomDataBaseUtility.getType(dataBaseName);
        if (typeName) {
            RandomDataBaseUtility.renameType(dataBaseName, typeName);
        }
    };
    RandomTask.moveTable = function (dataBaseName) {
        var tableName = RandomDataBaseUtility.getTable(dataBaseName);
        if (tableName) {
            RandomDataBaseUtility.moveTable(dataBaseName, tableName);
        }
    };
    RandomTask.moveType = function (dataBaseName) {
        var typeName = RandomDataBaseUtility.getType(dataBaseName);
        if (typeName) {
            RandomDataBaseUtility.moveType(dataBaseName, typeName);
        }
    };
    RandomTask.changeTableTemplate = function (dataBaseName) {
        var tableName = RandomDataBaseUtility.getTable(dataBaseName);
        if (tableName) {
            RandomDataBaseUtility.changeTableTemplate(dataBaseName, tableName);
        }
    };
    RandomTask.changeTableContent = function (dataBaseName) {
        var tableName = RandomDataBaseUtility.getTable(dataBaseName);
        if (tableName) {
            RandomDataBaseUtility.changeTableContent(dataBaseName, tableName);
        }
    };
    RandomTask.changeType = function (dataBaseName) {
        var typeName = RandomDataBaseUtility.getType(dataBaseName);
        if (typeName) {
            RandomDataBaseUtility.changeType(dataBaseName, typeName);
        }
    };
    RandomTask.createTableCategory = function (dataBaseName) {
        RandomDataBaseUtility.createTableCategory(dataBaseName);
    };
    RandomTask.createTypeCategory = function (dataBaseName) {
        RandomDataBaseUtility.createTypeCategory(dataBaseName);
    };
    RandomTask.renameTableCategory = function (dataBaseName) {
        var category = RandomDataBaseUtility.getTableCategory(dataBaseName);
        if (category) {
            RandomDataBaseUtility.renameTableCategory(dataBaseName, category);
        }
    };
    RandomTask.renameTypeCategory = function (dataBaseName) {
        var category = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        if (category) {
            RandomDataBaseUtility.renameTypeCategory(dataBaseName, category);
        }
    };
    RandomTask.moveTableCategory = function (dataBaseName) {
        var category = RandomDataBaseUtility.getTableCategory(dataBaseName);
        if (category) {
            RandomDataBaseUtility.moveTableCategory(dataBaseName, category);
        }
    };
    RandomTask.moveTypeCategory = function (dataBaseName) {
        var category = RandomDataBaseUtility.getTypeCategory(dataBaseName);
        if (category) {
            RandomDataBaseUtility.moveTypeCategory(dataBaseName, category);
        }
    };
    RandomTask.run = function (dataBaseName, count) {
        var funcs = [
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
        for (var i = 0; i < count; i++) {
            try {
                var index = RandomUtility.next(funcs.length);
                funcs[index](dataBaseName);
            }
            catch (e) {
                log(e);
            }
        }
    };
    return RandomTask;
}());
var Program = /** @class */ (function () {
    function Program() {
    }
    Program.main = function () {
        var dataBaseName = "default";
        var token = login("admin", "admin");
        if (isDataBaseLoaded(dataBaseName) === false) {
            loadDataBase(dataBaseName);
        }
        if (isDataBaseEntered(dataBaseName) === false) {
            enterDataBase(dataBaseName);
        }
        for (var i = 0; i < 10; i++) {
            var count = RandomUtility.nextInRange(10, 100);
            try {
                Program.doTask(dataBaseName, count);
            }
            finally {
                leaveDataBase(dataBaseName);
                logout(token);
            }
        }
    };
    Program.doTask = function (dataBaseName, count) {
        var transactionID = beginDataBaseTransaction(dataBaseName);
        try {
            RandomTask.run(dataBaseName, count);
            endDataBaseTransaction(transactionID);
        }
        catch (e) {
            cancelDataBaseTransaction(transactionID);
            throw e;
        }
    };
    return Program;
}());
Program.main();
