// declaration for Crema Server

declare enum TableColumnProperties {
    Index = 0,
    IsKey = 1,
    IsUnique = 2,
    Name = 3,
    DataType = 4,
    DefaultValue = 5,
    Comment = 6,
    AutoIncrement = 7,
    Tags = 8,
    IsReadOnly = 9,
    AllowNull = 10
}
declare enum TableProperties {
    TableName = 0,
    Tags = 1,
    Comment = 2
}
declare enum TypeMemberProperties {
    Name = 0,
    Value = 1,
    Comment = 2
}
declare enum TypeProperties {
    Name = 0,
    IsFlag = 1,
    Comment = 2
}

// Misc
declare function addEventListener (value: string, action: (e1: { [key: string]: any; }) => void): void;
declare function log (value: any): void;
declare function sleep (millisecondsTimeout: number): void;

// DataBase
/** @returns transactionID */
declare function beginDataBaseTransaction (dataBaseName: string): string;
declare function cancelDataBaseTransaction (transactionID: string): void;
declare function containsDataBase (dataBaseName: string): boolean;
declare function containsTable (dataBaseName: string, tableName: string): boolean;
declare function containsTableItem (dataBaseName: string, tableItemPath: string): boolean;
declare function containsType (dataBaseName: string, typeName: string): boolean;
declare function containsTypeItemItem (dataBaseName: string, typeItemPath: string): boolean;
declare function deleteTable (dataBaseName: string, tableName: string): void;
declare function deleteTableItem (dataBaseName: string, tableItemPath: string): void;
declare function deleteType (dataBaseName: string, typeName: string): void;
declare function deleteTypeItem (dataBaseName: string, typeItemPath: string): void;
declare function endDataBaseTransaction (transactionID: string): void;
declare function enterDataBase (dataBaseName: string): void;
declare function getDataBaseInfo (dataBaseName: string): { [key: string]: any; };
declare function getDataBaseInfoByTags (dataBaseName: string, tags: string): { [key: string]: any; };
declare function getDataBaseList (): string[];
declare function getDataBaseLogInfo (dataBaseName: string): { [key: string]: any; }[];
declare function getTableData (dataBaseName: string, tableName: string, revision?: number): { [key: number]: any; };
declare function getTableInfo (dataBaseName: string, tableName: string): { [key: string]: any; };
declare function getTableInfoByTags (dataBaseName: string, tableName: string, tags: string): { [key: string]: any; };
declare function getTableItemList (dataBaseName: string): string[];
declare function getTableItemLogInfo (dataBaseName: string, tableItemPath: string): { [key: string]: any; }[];
declare function getTableList (dataBaseName: string): string[];
declare function getTableListByTags (dataBaseName: string, tags: string): string[];
declare function getTypeData (dataBaseName: string, typeName: string, revision?: number): { [key: number]: any; };
declare function getTypeInfo (dataBaseName: string, typeName: string): { [key: string]: any; };
declare function getTypeInfoByTags (dataBaseName: string, typeName: string, tags: string): { [key: string]: any; };
declare function getTypeItemList (dataBaseName: string): string[];
declare function getTypeList (dataBaseName: string): string[];
declare function getTypeListByTags (dataBaseName: string, tags: string): string[];
declare function isDataBaseEntered (dataBaseName: string): boolean;
declare function isDataBaseLoaded (dataBaseName: string): boolean;
declare function leaveDataBase (dataBaseName: string): void;
declare function loadDataBase (dataBaseName: string): void;
declare function moveType (dataBaseName: string, typeName: string, categoryPath: string): void;
declare function moveTypeItem (dataBaseName: string, typeItemPath: string, parentPath: string): void;
declare function renameType (dataBaseName: string, typeName: string, newTypeName: string): void;
declare function renameTypeItem (dataBaseName: string, typeItemPath: string, newName: string): void;
declare function unloadDataBase (dataBaseName: string): void;

// Domain
declare function containsDomain (domainID: string): boolean;
declare function containsDomainUser (domainID: string, userID: string): boolean;
declare function deleteDomain (domainID: string, isCancel: boolean): void;
declare function getDomainInfo (domainID: string): { [key: string]: any; };
declare function getDomainList (): string[];
declare function getDomainUserList (domainID: string): string[];

// IO
/** @returns fullName */
declare function createDirectory (path: string): string;
declare function directoryExists (path: string): boolean;
declare function fileExists (filename: string): boolean;
declare function readFile (filename: string): any;
declare function readFileText (filename: string): string;
declare function writeFile (filename: string, value: any): void;
declare function writeFileText (filename: string, text: string): void;

// Permission
declare function isTableItemLocked (dataBaseName: string, tableItemPath: string): boolean;
declare function isTableItemPrivate (dataBaseName: string, tableItemPath: string): boolean;
declare function isTypeItemLocked (dataBaseName: string, tableItemPath: string): boolean;
declare function isTypeItemPrivate (dataBaseName: string, typeItemPath: string): boolean;
declare function lockTableItem (dataBaseName: string, tableItemPath: string, comment: string): void;
declare function privateTableItem (dataBaseName: string, tableItemPath: string): void;
declare function privateTypeItem (dataBaseName: string, typeItemPath: string): void;
declare function publicTableItem (dataBaseName: string, tableItemPath: string): void;
declare function publicTypeItem (dataBaseName: string, typeItemPath: string): void;
declare function unlockTableItem (dataBaseName: string, tableItemPath: string): void;

// TableContent
/** @returns keys */
declare function addTableContentRow (domainID: string, fields: { [key: string]: any; }): any[];
/** @returns domainID */
declare function beginTableContentEdit (dataBaseName: string, tableName: string): string;
declare function cancelTableContentEdit (domainID: string): void;
declare function containsTableContentRow (domainID: string, keys: any[]): boolean;
declare function endTableContentEdit (domainID: string): void;
declare function enterTableContentEdit (domainID: string): void;
/** @returns domainID */
declare function getTableContentDomain (dataBaseName: string, tableName: string): string;
declare function getTableContentRowCount (domainID: string, keys: any[], columnName: string): any;
declare function getTableContentRowField (domainID: string, keys: any[], columnName: string): any;
declare function getTableContentRowFields (domainID: string, keys: any[]): { [key: string]: any; };
declare function leaveTableContentEdit (domainID: string): void;
declare function setTableContentRowField (domainID: string, keys: any[], columnName: string, value: any): void;
declare function setTableContentRowFields (domainID: string, keys: any[], fields: { [key: string]: any; }): void;

// TableTemplate
/** @param string boolean, string, int, float, double, dateTime, unsignedInt, long, short, unsignedLong, unsignedByte, duration, unsignedShort, byte, guid or typePath(e.g., /categoryPath/typeName) */
declare function addTableTemplateColumn (domainID: string, columnName: string, typeName: string, comment: string, isKey?: boolean): void;
/** @returns domainID */
declare function beginChildTableCreate (dataBaseName: string, tableName: string): string;
/** @returns domainID */
declare function beginTableCreate (dataBaseName: string, categoryPath: string): string;
/** @returns domainID */
declare function beginTableTemplateEdit (dataBaseName: string, tableName: string): string;
declare function cancelTableTemplateEdit (domainID: string): void;
declare function containsTableTemplateColumn (domainID: string, columName: string): boolean;
declare function deleteTableTemplateColumn (domainID: string, columnName: string): void;
/** @returns tableName */
declare function endTableTemplateEdit (domainID: string): string;
declare function getTableTemplateColumnProperties (domainID: string, columnName: string): { [key: string]: any; };
declare function getTableTemplateColumnProperty (domainID: string, columnName: string, propertyName: TableColumnProperties): any;
/** @returns domainID */
declare function getTableTemplateDomain (dataBaseName: string, tableName: string): string;
declare function getTableTemplateProperties (domainID: string, columnName: string): { [key: string]: any; };
declare function getTableTemplateProperty (domainID: string, columnName: string, propertyName: TableProperties): any;
declare function setTableTemplateColumnProperty (domainID: string, columnName: string, propertyName: TableColumnProperties, value: any): void;
declare function setTableTemplateProperty (domainID: string, propertyName: TableProperties, value: any): void;

// TypeTemplate
declare function addTypeTemplateMember (domainID: string, memberName: string, value: number, comment: string): void;
/** @returns domainID */
declare function beginTypeCreate (dataBaseName: string, categoryPath: string): string;
/** @returns domainID */
declare function beginTypeTemplateEdit (dataBaseName: string, typeName: string): string;
declare function cancelTypeTemplateEdit (domainID: string): void;
declare function containsTypeTemplateMember (domainID: string, memberName: string): boolean;
declare function deleteTypeTemplateMember (domainID: string, memberName: string): void;
/** @returns typeName */
declare function endTypeTemplateEdit (domainID: string): string;
/** @returns domainID */
declare function getTypeTemplateDomain (dataBaseName: string, typeName: string): string;
declare function getTypeTemplateMemberProperties (domainID: string, memberName: string): { [key: string]: any; };
declare function getTypeTemplateMemberProperty (domainID: string, memberName: string, propertyName: TypeMemberProperties): any;
declare function getTypeTemplateProperties (domainID: string): { [key: string]: any; };
declare function getTypeTemplateProperty (domainID: string, propertyName: TypeProperties): any;
declare function setTypeTemplateMemberProperty (domainID: string, memberName: string, propertyName: TypeMemberProperties, value: any): void;
declare function setTypeTemplateProperty (domainID: string, propertyName: TypeProperties, value: any): void;

// User
declare function banUser (userID: string, comment: string): void;
declare function containsUser (userID: string): boolean;
declare function containsUserItem (userItemPath: string): boolean;
/** @returns userID */
declare function getCurrentUser (): string;
declare function getUserBanInfo (userID: string): { [key: string]: any; };
declare function getUserInfo (userID: string): { [key: string]: any; };
declare function getUserItemList (): string[];
declare function getUserList (): string[];
declare function getUserState (userID: string): string;
declare function isUserBanned (userID: string): boolean;
declare function isUserOnline (userID: string): boolean;
declare function kickUser (userID: string, comment: string): void;
declare function login (userID: string, password: string): string;
declare function logout (token: string): void;
declare function notify (message: string): void;
declare function sendMessage (userID: string, message: string): void;
declare function unbanUser (userID: string): void;

