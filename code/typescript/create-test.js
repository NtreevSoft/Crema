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
var CreateTest = /** @class */ (function () {
    function CreateTest() {
    }
    CreateTest.main = function () {
        var dataBaseName = "test";
        var token = login("admin", "admin");
        var index = 0;
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
            var domainID = beginTypeCreate(dataBaseName, "/");
            setTypeTemplateProperty(domainID, TypeProperties.Name, "BaseType");
            addTypeTemplateMember(domainID, "None", 0, "");
            addTypeTemplateMember(domainID, "A", 1, "");
            addTypeTemplateMember(domainID, "B", 2, "");
            addTypeTemplateMember(domainID, "C", 3, "");
            endTypeTemplateEdit(domainID);
        }
        {
            var domainID = beginTypeCreate(dataBaseName, "/");
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
        var tableName = "";
        {
            var domainID = beginTableCreate(dataBaseName, "/");
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
            var domainID = beginTableContentEdit(dataBaseName, tableName);
            enterTableContentEdit(domainID);
            for (var i = 0; i < 100; i++) {
                var fields = {};
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
    };
    return CreateTest;
}());
CreateTest.main();
