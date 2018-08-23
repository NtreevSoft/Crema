// declaration for Crema Server

declare enum DataBaseEvents {
    TableStateChanged = 0,
    TableChanged = 1,
    TableItemCreated = 2,
    TableItemRenamed = 3,
    TableItemMoved = 4,
    TableItemDeleted = 5,
    TableItemAccessChanged = 6,
    TableItemLockChanged = 7,
    TypeStateChanged = 8,
    TypeChanged = 9,
    TypeItemCreated = 10,
    TypeItemRenamed = 11,
    TypeItemMoved = 12,
    TypeItemDeleted = 13,
    TypeItemAccessChanged = 14,
    TypeItemLockChanged = 15
}
declare enum CremaEvents {
    UserStateChanged = 0,
    UserChanged = 1,
    UserItemCreated = 2,
    UserItemRenamed = 3,
    UserItemMoved = 4,
    UserItemDeleted = 5,
    UserLoggedIn = 6,
    UserLoggedOut = 7,
    UserKicked = 8,
    UserBanChanged = 9,
    MessageReceived = 10,
    DomainCreated = 11,
    DomainDeleted = 12,
    DomainInfoChanged = 13,
    DomainStateChanged = 14,
    DomainUserAdded = 15,
    DomainUserRemoved = 16,
    DomainUserChanged = 17,
    DomainRowAdded = 18,
    DomainRowChanged = 19,
    DomainRowRemoved = 20,
    DomainPropertyChanged = 21,
    DataBaseCreated = 22,
    DataBaseRenamed = 23,
    DataBaseDeleted = 24,
    DataBaseLoaded = 25,
    DataBaseUnloaded = 26,
    DataBaseResetting = 27,
    DataBaseReset = 28,
    DataBaseAuthenticationEntered = 29,
    DataBaseAuthenticationLeft = 30,
    DataBaseInfoChanged = 31,
    DataBaseStateChanged = 32,
    DataBaseAccessChanged = 33,
    DataBaseLockChanged = 34
}
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
    Name = 0,
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

type DataBaseEventListener = (dataBaseName: string, e: { [key: string]: any; }) => void;
type CremaEventListener = (e: { [key: string]: any; }) => void;

// Misc
declare function addDataBaseEventListener(eventName: DataBaseEvents, listener: DataBaseEventListener): void;
declare function addEventListener(eventName: CremaEvents, listener: CremaEventListener): void;
declare function log(value: any): void;
declare function removeDataBaseEventListener(eventName: DataBaseEvents, listener: DataBaseEventListener): void;
declare function removeEventListener(eventName: CremaEvents, listener: CremaEventListener): void;
declare function sleep(millisecondsTimeout: number): void;

// DataBase
/** @returns transactionID */
declare function beginDataBaseTransaction(dataBaseName: string): string;
declare function cancelDataBaseTransaction(transactionID: string): void;
declare function containsDataBase(dataBaseName: string): boolean;
declare function containsTable(dataBaseName: string, tableName: string): boolean;
declare function containsTableItem(dataBaseName: string, tableItemPath: string): boolean;
declare function containsType(dataBaseName: string, typeName: string): boolean;
declare function containsTypeItem(dataBaseName: string, typeItemPath: string): boolean;
declare function copyDataBase(dataBaseName: string, newDataBaseName: string, comment: string, force?: boolean): void;
declare function copyTable(dataBaseName: string, tableName: string, newTableName: string, categoryPath: string, copyContent?: boolean): void;
declare function copyType(dataBaseName: string, typeName: string, newTypeName: string, categoryPath: string): void;
declare function createDataBase(dataBaseName: string, comment: string): void;
/** @returns categoryPath */
declare function createTableCategory(dataBaseName: string, parentPath: string, categoryName: string): string;
/** @returns categoryPath */
declare function createTypeCategory(dataBaseName: string, parentPath: string, categoryName: string): string;
declare function deleteDataBase(dataBaseName: string): void;
declare function deleteTable(dataBaseName: string, tableName: string): void;
declare function deleteTableCategory(dataBaseName: string, categoryPath: string): void;
declare function deleteTableItem(dataBaseName: string, tableItemPath: string): void;
declare function deleteType(dataBaseName: string, typeName: string): void;
declare function deleteTypeCategory(dataBaseName: string, categoryPath: string): void;
declare function deleteTypeItem(dataBaseName: string, typeItemPath: string): void;
declare function endDataBaseTransaction(transactionID: string): void;
declare function enterDataBase(dataBaseName: string): void;
declare function getDataBaseData(dataBaseName: string, revision: string): string;
declare function getDataBaseInfo(dataBaseName: string): { [key: string]: any; };
declare function getDataBaseInfoByTags(dataBaseName: string, tags: string): { [key: string]: any; };
declare function getDataBaseList(): string[];
declare function getDataBaseLogInfo(dataBaseName: string): { [key: string]: any; }[];
declare function getTableCategoryList(dataBaseName: string): string[];
declare function getTableData(dataBaseName: string, tableName: string, revision: string): { [key: number]: any; };
declare function getTableInfo(dataBaseName: string, tableName: string): { [key: string]: any; };
declare function getTableInfoByTags(dataBaseName: string, tableName: string, tags: string): { [key: string]: any; };
declare function getTableItemData(dataBaseName: string, tableItemPath: string, revision: string): { [key: string]: { [key: number]: any; }; };
declare function getTableItemList(dataBaseName: string): string[];
declare function getTableItemLogInfo(dataBaseName: string, tableItemPath: string): { [key: string]: any; }[];
declare function getTableList(dataBaseName: string): string[];
declare function getTableListByTags(dataBaseName: string, tags: string): string[];
declare function getTypeCategoryList(dataBaseName: string): string[];
declare function getTypeData(dataBaseName: string, typeName: string, revision: string): { [key: number]: any; };
declare function getTypeInfo(dataBaseName: string, typeName: string): { [key: string]: any; };
declare function getTypeInfoByTags(dataBaseName: string, typeName: string, tags: string): { [key: string]: any; };
declare function getTypeItemData(dataBaseName: string, typeItemPath: string, revision: string): { [key: string]: { [key: number]: any; }; };
declare function getTypeItemList(dataBaseName: string): string[];
declare function getTypeItemLogInfo(dataBaseName: string, typeItemPath: string): { [key: string]: any; }[];
declare function getTypeList(dataBaseName: string): string[];
declare function getTypeListByTags(dataBaseName: string, tags: string): string[];
declare function inheritTable(dataBaseName: string, tableName: string, newTableName: string, categoryPath: string, copyContent?: boolean): void;
declare function isDataBaseEntered(dataBaseName: string): boolean;
declare function isDataBaseLoaded(dataBaseName: string): boolean;
declare function leaveDataBase(dataBaseName: string): void;
declare function loadDataBase(dataBaseName: string): void;
declare function moveTable(dataBaseName: string, tableName: string, categoryPath: string): void;
/** @returns categoryPath */
declare function moveTableCategory(dataBaseName: string, categoryPath: string, parentPath: string): string;
/** @returns path */
declare function moveTableItem(dataBaseName: string, tableItemPath: string, parentPath: string): string;
declare function moveType(dataBaseName: string, typeName: string, categoryPath: string): void;
/** @returns categoryPath */
declare function moveTypeCategory(dataBaseName: string, categoryPath: string, parentPath: string): string;
/** @returns path */
declare function moveTypeItem(dataBaseName: string, typeItemPath: string, parentPath: string): string;
declare function renameDataBase(dataBaseName: string, newDataBaseName: string): void;
declare function renameTable(dataBaseName: string, tableName: string, newTableName: string): void;
/** @returns categoryPath */
declare function renameTableCategory(dataBaseName: string, categoryPath: string, newName: string): string;
/** @returns path */
declare function renameTableItem(dataBaseName: string, tableItemPath: string, newName: string): string;
declare function renameType(dataBaseName: string, typeName: string, newTypeName: string): void;
/** @returns categoryPath */
declare function renameTypeCategory(dataBaseName: string, categoryPath: string, newName: string): string;
/** @returns path */
declare function renameTypeItem(dataBaseName: string, typeItemPath: string, newName: string): string;
declare function unloadDataBase(dataBaseName: string): void;

// Domain
declare function containsDomain(domainID: string): boolean;
declare function containsDomainUser(domainID: string, userID: string): boolean;
declare function deleteDomain(domainID: string, isCancel: boolean): void;
declare function getDomainInfo(domainID: string): { [key: string]: any; };
declare function getDomainList(): string[];
declare function getDomainUserList(domainID: string): string[];

// IO
/** @returns fullName */
declare function createDirectory(path: string): string;
declare function directoryExists(path: string): boolean;
declare function fileExists(filename: string): boolean;
declare function readFile(filename: string): any;
declare function readFileText(filename: string): string;
declare function writeFile(filename: string, value: any): void;
declare function writeFileText(filename: string, text: string): void;

// Permission
declare function isDataBaseLocked(dataBaseName: string): boolean;
declare function isDataBasePrivate(dataBaseName: string): boolean;
declare function isTableItemLocked(dataBaseName: string, tableItemPath: string): boolean;
declare function isTableItemPrivate(dataBaseName: string, tableItemPath: string): boolean;
declare function isTypeItemLocked(dataBaseName: string, typeItemPath: string): boolean;
declare function isTypeItemPrivate(dataBaseName: string, typeItemPath: string): boolean;
declare function lockDataBase(dataBaseName: string, comment: string): void;
declare function lockTableItem(dataBaseName: string, tableItemPath: string, comment: string): void;
declare function lockTypeItem(dataBaseName: string, typeItemPath: string, comment: string): void;
declare function privateTableItem(dataBaseName: string, tableItemPath: string): void;
declare function privateTypeItem(dataBaseName: string, typeItemPath: string): void;
declare function publicTableItem(dataBaseName: string, tableItemPath: string): void;
declare function publicTypeItem(dataBaseName: string, typeItemPath: string): void;
declare function unlockDataBase(dataBaseName: string): void;
declare function unlockTableItem(dataBaseName: string, tableItemPath: string): void;
declare function unlockTypeItem(dataBaseName: string, typeItemPath: string): void;

// TableContent
/** @returns keys */
declare function addTableContentRow(domainID: string, tableName: string, fields: { [key: string]: any; }): any[];
/** @returns domainID */
declare function beginTableContentEdit(dataBaseName: string, tableName: string): string;
declare function cancelTableContentEdit(domainID: string): void;
declare function containsTableContentRow(domainID: string, tableName: string, keys: any[]): boolean;
declare function endTableContentEdit(domainID: string): void;
declare function enterTableContentEdit(domainID: string): void;
/** @returns domainID */
declare function getTableContentDomain(dataBaseName: string, tableName: string): string;
declare function getTableContentRowCount(domainID: string, tableName: string, keys: any[], columnName: string): any;
declare function getTableContentRowField(domainID: string, tableName: string, keys: any[], columnName: string): any;
declare function getTableContentRowFields(domainID: string, tableName: string, keys: any[]): { [key: string]: any; };
declare function getTableContentTables(domainID: string): string[];
declare function leaveTableContentEdit(domainID: string): void;
declare function setTableContentRowField(domainID: string, tableName: string, keys: any[], columnName: string, value: any): void;
declare function setTableContentRowFields(domainID: string, tableName: string, keys: any[], fields: { [key: string]: any; }): void;

// TableTemplate
/** @param typeName boolean, string, single, double, int8, uint8, int16, uint16, int32, uint32, int64, uint64, datetime, duration, guid or typePath(e.g., /categoryPath/typeName) */
declare function addTableTemplateColumn(domainID: string, columnName: string, typeName: string, comment: string, isKey?: boolean): void;
/** @returns domainID */
declare function beginTableCreate(dataBaseName: string, parentPath: string): string;
/** @returns domainID */
declare function beginTableTemplateEdit(dataBaseName: string, tableName: string): string;
declare function cancelTableTemplateEdit(domainID: string): void;
declare function containsTableTemplateColumn(domainID: string, columName: string): boolean;
declare function deleteTableTemplateColumn(domainID: string, columnName: string): void;
/** @returns tableName */
declare function endTableTemplateEdit(domainID: string): string;
declare function getTableTemplateColumnProperties(domainID: string, columnName: string): { [key: string]: any; };
declare function getTableTemplateColumnProperty(domainID: string, columnName: string, propertyName: TableColumnProperties): any;
declare function getTableTemplateColumns(domainID: string): string[];
/** @returns domainID */
declare function getTableTemplateDomain(dataBaseName: string, tableName: string): string;
declare function getTableTemplateProperties(domainID: string, columnName: string): { [key: string]: any; };
declare function getTableTemplateProperty(domainID: string, columnName: string, propertyName: TableProperties): any;
declare function getTableTemplateSelectableTypes(domainID: string): string[];
declare function setTableTemplateColumnProperty(domainID: string, columnName: string, propertyName: TableColumnProperties, value: any): void;
declare function setTableTemplateProperty(domainID: string, propertyName: TableProperties, value: any): void;

// TypeTemplate
declare function addTypeTemplateMember(domainID: string, memberName: string, value: number, comment: string): void;
/** @returns domainID */
declare function beginTypeCreate(dataBaseName: string, categoryPath: string): string;
/** @returns domainID */
declare function beginTypeTemplateEdit(dataBaseName: string, typeName: string): string;
declare function cancelTypeTemplateEdit(domainID: string): void;
declare function containsTypeTemplateMember(domainID: string, memberName: string): boolean;
declare function deleteTypeTemplateMember(domainID: string, memberName: string): void;
/** @returns typeName */
declare function endTypeTemplateEdit(domainID: string): string;
/** @returns domainID */
declare function getTypeTemplateDomain(dataBaseName: string, typeName: string): string;
declare function getTypeTemplateMemberProperties(domainID: string, memberName: string): { [key: string]: any; };
declare function getTypeTemplateMemberProperty(domainID: string, memberName: string, propertyName: TypeMemberProperties): any;
declare function getTypeTemplateMembers(domainID: string): string[];
declare function getTypeTemplateProperties(domainID: string): { [key: string]: any; };
declare function getTypeTemplateProperty(domainID: string, propertyName: TypeProperties): any;
declare function setTypeTemplateMemberProperty(domainID: string, memberName: string, propertyName: TypeMemberProperties, value: any): void;
declare function setTypeTemplateProperty(domainID: string, propertyName: TypeProperties, value: any): void;

// User
declare function banUser(userID: string, comment: string): void;
declare function containsUser(userID: string): boolean;
declare function containsUserItem(userItemPath: string): boolean;
/** @returns categoryPath */
declare function createUserCategory(parentPath: string, categoryName: string): string;
declare function deleteUser(userID: string): void;
declare function deleteUserCategory(userItemPath: string): void;
declare function deleteUserItem(userItemPath: string): void;
/** @returns userID */
declare function getCurrentUser(): string;
declare function getUserBanInfo(userID: string): { [key: string]: any; };
declare function getUserCategoryList(): string[];
declare function getUserInfo(userID: string): { [key: string]: any; };
declare function getUserItemList(): string[];
declare function getUserList(): string[];
declare function getUserState(userID: string): string;
declare function isUserBanned(userID: string): boolean;
declare function isUserOnline(userID: string): boolean;
declare function kickUser(userID: string, comment: string): void;
declare function login(userID: string, password: string): string;
declare function logout(token: string): void;
/** @returns userPath */
declare function moveUser(userID: string, parentPath: string): string;
/** @returns categoryPath */
declare function moveUserCategory(categoryPath: string, parentPath: string): string;
/** @returns path */
declare function moveUserItem(userItemPath: string, parentPath: string): string;
declare function notify(message: string): void;
/** @returns categoryPath */
declare function renameUserCategory(categoryPath: string, newName: string): string;
/** @returns path */
declare function renameUserItem(userItemPath: string, newName: string): string;
declare function sendMessage(userID: string, message: string): void;
declare function unbanUser(userID: string): void;

