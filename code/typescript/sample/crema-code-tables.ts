import base = require("./crema-code-base");
import reader = require("./crema-code-reader");
import types = require("./crema-code-types");

// ------------------------------------------------------------------------------
// dataBase: default
// revision: 412
// requested revision: -1
// devmode: False
// hash value: f52bdea30268729d366eaeb9bd320df505a16dbf
// tags: All
// ------------------------------------------------------------------------------

export class ProjectInfoExportInfoRow extends base.CremaRow {

    private tableExportInfo: ProjectInfoExportInfo;

    private fieldID: number;

    private fieldFileName: string;

    private fieldTableName: string;

    public constructor(row: reader.IRow, table: ProjectInfoExportInfo) {
        super(row);
        this.tableExportInfo = table;
        this.fieldID = row.toInt32(0);
        if (row.hasValue(1)) {
            this.fieldFileName = row.toString(1);
        }
        if (row.hasValue(2)) {
            this.fieldTableName = row.toString(2);
        }
        this.setKey(this.fieldID);
    }

    // creator: admin
    // createdDateTime: 2017-10-26 오전 6:21:11
    // modifier: admin
    // modifiedDateTime: 2017-10-26 오전 6:21:41
    public get ID(): number {
        return this.fieldID;
    }

    // creator: admin
    // createdDateTime: 2017-10-25 오전 8:35:20
    // modifier: admin
    // modifiedDateTime: 2017-10-26 오전 8:42:54
    public get fileName(): string {
        return this.fieldFileName;
    }

    // creator: admin
    // createdDateTime: 2017-10-25 오전 8:35:02
    // modifier: admin
    // modifiedDateTime: 2017-10-26 오전 6:20:29
    public get tableName(): string {
        return this.fieldTableName;
    }

    public get table(): ProjectInfoExportInfo {
        return this.tableExportInfo;
    }

    public get parent(): ProjectInfoRow {
        return (<ProjectInfoRow>(this.parentInternal));
    }
}

// createdDateTime: 2017-10-25 오전 8:34:43
// modifiedDateTime: 2017-10-26 오전 8:06:01
// contentsModifier: s2quake
// contentsModifiedDateTime: 2018-03-27 오전 5:13:04
export class ProjectInfoExportInfo extends base.CremaTable<ProjectInfoExportInfoRow> {

    public static createFromTable(table: reader.ITable): ProjectInfoExportInfo {
        if ((table.hashValue !== "06e32bd7a3bb2e2ca4e256312e847b3413b4bb5a")) {
            throw new Error("ProjectInfo.ExportInfo 테이블과 데이터의 형식이 맞지 않습니다.");
        }
        let instance: ProjectInfoExportInfo = new ProjectInfoExportInfo();
        instance.readFromTable(table);
        return instance;
    }

    public static createFromRows(name: string, rows: ProjectInfoExportInfoRow[]): ProjectInfoExportInfo {
        let instance: ProjectInfoExportInfo = new ProjectInfoExportInfo();
        instance.readFromRows(name, rows);
        return instance;
    }

    protected readFromRows(name: string, rows: ProjectInfoExportInfoRow[]): void {
        super.readFromRows(name, rows);
    }

    public find(ID: number): ProjectInfoExportInfoRow {
        return super.findRow(ID);
    }

    protected createRowInstance(row: reader.IRow, table: any): ProjectInfoExportInfoRow {
        return new ProjectInfoExportInfoRow(row, (<ProjectInfoExportInfo>(table)));
    }

    protected readFromTable(table: reader.ITable): void {
        super.readFromTable(table);
    }
}

export class ProjectInfoRow extends base.CremaRow {

    private static tableExportInfoEmpty: ProjectInfoExportInfo = new ProjectInfoExportInfo();

    private tableProjectInfo: ProjectInfo;

    private fieldKindType: types.KindType;

    private fieldProjectType: types.ProjectType;

    private fieldName: string;

    private fieldProjectPath: string;

    private tableExportInfo: ProjectInfoExportInfo;

    public constructor(row: reader.IRow, table: ProjectInfo) {
        super(row);
        this.tableProjectInfo = table;
        this.fieldKindType = row.toInt32(0);
        this.fieldProjectType = row.toInt32(1);
        this.fieldName = row.toString(2);
        if (row.hasValue(3)) {
            this.fieldProjectPath = row.toString(3);
        }
        this.setKey(this.fieldKindType, this.fieldProjectType, this.fieldName);
    }

    // creator: admin
    // createdDateTime: 2017-10-25 오전 8:40:56
    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 8:22:11
    public get kindType(): types.KindType {
        return this.fieldKindType;
    }

    // creator: admin
    // createdDateTime: 2017-11-01 오전 8:22:24
    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 8:23:24
    public get projectType(): types.ProjectType {
        return this.fieldProjectType;
    }

    // creator: admin
    // createdDateTime: 2017-10-25 오전 8:38:54
    // modifier: admin
    // modifiedDateTime: 2017-10-25 오전 8:42:24
    public get name(): string {
        return this.fieldName;
    }

    // creator: admin
    // createdDateTime: 2017-10-25 오전 8:25:15
    // modifier: admin
    // modifiedDateTime: 2017-10-25 오전 8:42:25
    public get projectPath(): string {
        return this.fieldProjectPath;
    }

    public get exportInfo(): ProjectInfoExportInfo {
        if ((this.tableExportInfo == null)) {
            return ProjectInfoRow.tableExportInfoEmpty;
        }
        return this.tableExportInfo;
    }

    public get table(): ProjectInfo {
        return this.tableProjectInfo;
    }

    static setExportInfo(target: ProjectInfoRow, childName: string, childs: ProjectInfoExportInfoRow[]): void {
        ProjectInfoRow.setParent<ProjectInfoRow, ProjectInfoExportInfoRow>(target, childs);
        target.tableExportInfo = ProjectInfoExportInfo.createFromRows(childName, childs);
    }
}

// modifier: admin
// modifiedDateTime: 2017-11-01 오전 8:22:24
// contentsModifier: s2quake
// contentsModifiedDateTime: 2018-03-27 오전 5:12:57
export class ProjectInfo extends base.CremaTable<ProjectInfoRow> {

    private tableExportInfo: ProjectInfoExportInfo;

    public get exportInfo(): ProjectInfoExportInfo {
        return this.tableExportInfo;
    }

    public static createFromTable(table: reader.ITable): ProjectInfo {
        if ((table.hashValue !== "b576d4aab4aaa0f758549efab982de6ceb5b7b66")) {
            throw new Error("ProjectInfo 테이블과 데이터의 형식이 맞지 않습니다.");
        }
        let instance: ProjectInfo = new ProjectInfo();
        instance.readFromTable(table);
        return instance;
    }

    public static createFromRows(name: string, rows: ProjectInfoRow[]): ProjectInfo {
        let instance: ProjectInfo = new ProjectInfo();
        instance.readFromRows(name, rows);
        return instance;
    }

    public find(kindType: types.KindType, projectType: types.ProjectType, name: string): ProjectInfoRow {
        return super.findRow(kindType, projectType, name);
    }

    protected createRowInstance(row: reader.IRow, table: any): ProjectInfoRow {
        return new ProjectInfoRow(row, (<ProjectInfo>(table)));
    }

    protected readFromTable(table: reader.ITable): void {
        super.readFromTable(table);
        this.tableExportInfo = ProjectInfoExportInfo.createFromTable(table.dataSet.tables[(table.name + ".ExportInfo")]);
        this.setRelations((table.name + ".ExportInfo"), this.tableExportInfo.rows, ProjectInfoRow.setExportInfo);
    }
}

export class StringTableRow extends base.CremaRow {

    private tableStringTable: StringTable;

    private fieldType: types.StringType;

    private fieldName: string;

    private fieldComment: string;

    private fieldValue: string;

    private fieldko_KR: string;

    public constructor(row: reader.IRow, table: StringTable) {
        super(row);
        this.tableStringTable = table;
        this.fieldType = row.toInt32(0);
        this.fieldName = row.toString(1);
        if (row.hasValue(2)) {
            this.fieldComment = row.toString(2);
        }
        if (row.hasValue(3)) {
            this.fieldValue = row.toString(3);
        }
        if (row.hasValue(4)) {
            this.fieldko_KR = row.toString(4);
        }
        this.setKey(this.fieldType, this.fieldName);
    }

    // creator: admin
    // createdDateTime: 2017-10-23 오전 7:16:08
    // modifier: admin
    // modifiedDateTime: 2017-10-23 오전 7:16:24
    public get type(): types.StringType {
        return this.fieldType;
    }

    // creator: admin
    // createdDateTime: 2017-10-23 오전 7:16:14
    // modifier: admin
    // modifiedDateTime: 2017-10-23 오전 7:16:25
    public get name(): string {
        return this.fieldName;
    }

    // creator: admin
    // createdDateTime: 2017-10-23 오전 7:16:18
    // modifier: admin
    // modifiedDateTime: 2017-10-23 오전 7:16:18
    public get comment(): string {
        return this.fieldComment;
    }

    // creator: admin
    // createdDateTime: 2017-10-23 오전 7:16:22
    // modifier: admin
    // modifiedDateTime: 2017-10-23 오전 7:16:22
    public get value(): string {
        return this.fieldValue;
    }

    // creator: admin
    // createdDateTime: 2017-10-23 오전 9:20:19
    // modifier: admin
    // modifiedDateTime: 2017-10-23 오전 9:20:19
    public get ko_KR(): string {
        return this.fieldko_KR;
    }

    public get table(): StringTable {
        return this.tableStringTable;
    }
}

// creator: admin
// createdDateTime: 2017-10-23 오전 7:16:01
// modifier: admin
// modifiedDateTime: 2017-10-25 오전 5:30:21
export class StringTable extends base.CremaTable<StringTableRow> {

    public static createFromTable(table: reader.ITable): StringTable {
        if ((table.hashValue !== "6c66b01d950836901feeeed701a0b36f5f3d58c1")) {
            throw new Error("StringTable 테이블과 데이터의 형식이 맞지 않습니다.");
        }
        let instance: StringTable = new StringTable();
        instance.readFromTable(table);
        return instance;
    }

    public static createFromRows(name: string, rows: StringTableRow[]): StringTable {
        let instance: StringTable = new StringTable();
        instance.readFromRows(name, rows);
        return instance;
    }

    public find(type: types.StringType, name: string): StringTableRow {
        return super.findRow(type, name);
    }

    protected createRowInstance(row: reader.IRow, table: any): StringTableRow {
        return new StringTableRow(row, (<StringTable>(table)));
    }

    protected readFromTable(table: reader.ITable): void {
        super.readFromTable(table);
    }
}

export class TestTableRow extends base.CremaRow {

    private tableTestTable: TestTable;

    private fieldKey: number;

    private fieldValue: string;

    private fieldDateTimePicker: Date;

    private fieldTimePicker: number;

    public constructor(row: reader.IRow, table: TestTable) {
        super(row);
        this.tableTestTable = table;
        this.fieldKey = row.toInt32(0);
        if (row.hasValue(1)) {
            this.fieldValue = row.toString(1);
        }
        if (row.hasValue(2)) {
            this.fieldDateTimePicker = row.toDateTime(2);
        }
        if (row.hasValue(3)) {
            this.fieldTimePicker = row.toDuration(3);
        }
        this.setKey(this.fieldKey);
    }

    // creator: admin
    // createdDateTime: 2017-11-01 오전 2:37:44
    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 2:38:12
    public get key(): number {
        return this.fieldKey;
    }

    // creator: admin
    // createdDateTime: 2017-11-01 오전 2:37:57
    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 2:37:57
    public get value(): string {
        return this.fieldValue;
    }

    // creator: admin
    // createdDateTime: 2017-11-01 오전 2:38:09
    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 2:38:09
    public get dateTimePicker(): Date {
        return this.fieldDateTimePicker;
    }

    // creator: admin
    // createdDateTime: 2017-11-01 오전 4:10:41
    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 4:10:41
    public get timePicker(): number {
        return this.fieldTimePicker;
    }

    public get table(): TestTable {
        return this.tableTestTable;
    }
}

// modifier: admin
// modifiedDateTime: 2017-11-01 오전 4:10:41
// contentsModifier: admin
// contentsModifiedDateTime: 2017-11-01 오전 4:21:18
export class TestTable extends base.CremaTable<TestTableRow> {

    public static createFromTable(table: reader.ITable): TestTable {
        if ((table.hashValue !== "148d8eee57fb716dc0914b646b2f965325693a55")) {
            throw new Error("TestTable 테이블과 데이터의 형식이 맞지 않습니다.");
        }
        let instance: TestTable = new TestTable();
        instance.readFromTable(table);
        return instance;
    }

    public static createFromRows(name: string, rows: TestTableRow[]): TestTable {
        let instance: TestTable = new TestTable();
        instance.readFromRows(name, rows);
        return instance;
    }

    public find(key: number): TestTableRow {
        return super.findRow(key);
    }

    protected createRowInstance(row: reader.IRow, table: any): TestTableRow {
        return new TestTableRow(row, (<TestTable>(table)));
    }

    protected readFromTable(table: reader.ITable): void {
        super.readFromTable(table);
    }
}

export class CremaDataSet extends base.CremaData {

    private _name: string;

    private _revision: number;

    private _typesHashValue: string;

    private _tablesHashValue: string;

    private _tags: string;

    private tableClientApplicationHost: StringTable;

    private tableClientBase: StringTable;

    private tableClientConsole: StringTable;

    private tableClientConverters: StringTable;

    private tableClientDifferences: StringTable;

    private tableClientFramework: StringTable;

    private tableClientServices: StringTable;

    private tableClientSmartSet: StringTable;

    private tableClientTables: StringTable;

    private tableClientTypes: StringTable;

    private tableClientUsers: StringTable;

    private tableCommonPresentation: StringTable;

    private tableCommonServiceModel: StringTable;

    private tableCommonServiceModel_Event: StringTable;

    private tableCremaData: StringTable;

    private tableCremaProjectInfo: ProjectInfo;

    private tableModernUIFramework: StringTable;

    private tableNtreevLibrary: StringTable;

    private tableProjectInfo: ProjectInfo;

    private tableServerServiceHosts: StringTable;

    private tableStringTable: StringTable;

    private tableStringTable11: StringTable;

    private tableTestTable: TestTable;

    public get name(): string {
        return this._name;
    }

    public get revision(): number {
        return this._revision;
    }

    public get typesHashValue(): string {
        return this._typesHashValue;
    }

    public get tablesHashValue(): string {
        return this._tablesHashValue;
    }

    public get tags(): string {
        return this._tags;
    }

    public get clientApplicationHost(): StringTable {
        return this.tableClientApplicationHost;
    }

    public get clientBase(): StringTable {
        return this.tableClientBase;
    }

    public get clientConsole(): StringTable {
        return this.tableClientConsole;
    }

    public get clientConverters(): StringTable {
        return this.tableClientConverters;
    }

    public get clientDifferences(): StringTable {
        return this.tableClientDifferences;
    }

    public get clientFramework(): StringTable {
        return this.tableClientFramework;
    }

    public get clientServices(): StringTable {
        return this.tableClientServices;
    }

    public get clientSmartSet(): StringTable {
        return this.tableClientSmartSet;
    }

    public get clientTables(): StringTable {
        return this.tableClientTables;
    }

    public get clientTypes(): StringTable {
        return this.tableClientTypes;
    }

    public get clientUsers(): StringTable {
        return this.tableClientUsers;
    }

    public get commonPresentation(): StringTable {
        return this.tableCommonPresentation;
    }

    public get commonServiceModel(): StringTable {
        return this.tableCommonServiceModel;
    }

    public get commonServiceModel_Event(): StringTable {
        return this.tableCommonServiceModel_Event;
    }

    public get cremaData(): StringTable {
        return this.tableCremaData;
    }

    public get cremaProjectInfo(): ProjectInfo {
        return this.tableCremaProjectInfo;
    }

    public get modernUIFramework(): StringTable {
        return this.tableModernUIFramework;
    }

    public get ntreevLibrary(): StringTable {
        return this.tableNtreevLibrary;
    }

    public get projectInfo(): ProjectInfo {
        return this.tableProjectInfo;
    }

    public get serverServiceHosts(): StringTable {
        return this.tableServerServiceHosts;
    }

    public get stringTable(): StringTable {
        return this.tableStringTable;
    }

    public get stringTable11(): StringTable {
        return this.tableStringTable11;
    }

    public get testTable(): TestTable {
        return this.tableTestTable;
    }

    public static createFromDataSet(dataSet: reader.IDataSet, verifyRevision: boolean): CremaDataSet {
        let instance: CremaDataSet = new CremaDataSet();
        instance.readFromDataSet(dataSet, verifyRevision);
        return instance;
    }

    public static createFromFile(filename: string, verifyRevision: boolean): CremaDataSet {
        let instance: CremaDataSet = new CremaDataSet();
        instance.readFromFile(filename, verifyRevision);
        return instance;
    }

    public readFromDataSet(dataSet: reader.IDataSet, verifyRevision: boolean): void {
        if (("default" !== dataSet.name)) {
            throw new Error("데이터의 이름이 코드 이름(default)과 다릅니다.");
        }
        if (("4b1da7ae630f37090a4cee6e3e446893c6ad9297" !== dataSet.typesHashValue)) {
            throw new Error("타입 해시값이 \'4b1da7ae630f37090a4cee6e3e446893c6ad9297\'이 아닙니다.");
        }
        if (("f52bdea30268729d366eaeb9bd320df505a16dbf" !== dataSet.tablesHashValue)) {
            throw new Error("테이블 해시값이 \'f52bdea30268729d366eaeb9bd320df505a16dbf\'이 아닙니다.");
        }
        if (((verifyRevision === true) && ((<number>(412)) !== dataSet.revision))) {
            throw new Error("데이터의 리비전 코드 리비전(412)과 다릅니다.");
        }
        this._name = dataSet.name;
        this._revision = dataSet.revision;
        this._typesHashValue = dataSet.typesHashValue;
        this._tablesHashValue = dataSet.tablesHashValue;
        this._tags = dataSet.tags;
        this.tableClientApplicationHost = StringTable.createFromTable(dataSet.tables.ClientApplicationHost);
        this.tableClientBase = StringTable.createFromTable(dataSet.tables.ClientBase);
        this.tableClientConsole = StringTable.createFromTable(dataSet.tables.ClientConsole);
        this.tableClientConverters = StringTable.createFromTable(dataSet.tables.ClientConverters);
        this.tableClientDifferences = StringTable.createFromTable(dataSet.tables.ClientDifferences);
        this.tableClientFramework = StringTable.createFromTable(dataSet.tables.ClientFramework);
        this.tableClientServices = StringTable.createFromTable(dataSet.tables.ClientServices);
        this.tableClientSmartSet = StringTable.createFromTable(dataSet.tables.ClientSmartSet);
        this.tableClientTables = StringTable.createFromTable(dataSet.tables.ClientTables);
        this.tableClientTypes = StringTable.createFromTable(dataSet.tables.ClientTypes);
        this.tableClientUsers = StringTable.createFromTable(dataSet.tables.ClientUsers);
        this.tableCommonPresentation = StringTable.createFromTable(dataSet.tables.CommonPresentation);
        this.tableCommonServiceModel = StringTable.createFromTable(dataSet.tables.CommonServiceModel);
        this.tableCommonServiceModel_Event = StringTable.createFromTable(dataSet.tables.CommonServiceModel_Event);
        this.tableCremaData = StringTable.createFromTable(dataSet.tables.CremaData);
        this.tableCremaProjectInfo = ProjectInfo.createFromTable(dataSet.tables.CremaProjectInfo);
        this.tableModernUIFramework = StringTable.createFromTable(dataSet.tables.ModernUIFramework);
        this.tableNtreevLibrary = StringTable.createFromTable(dataSet.tables.NtreevLibrary);
        this.tableProjectInfo = ProjectInfo.createFromTable(dataSet.tables.ProjectInfo);
        this.tableServerServiceHosts = StringTable.createFromTable(dataSet.tables.ServerServiceHosts);
        this.tableStringTable = StringTable.createFromTable(dataSet.tables.StringTable);
        this.tableStringTable11 = StringTable.createFromTable(dataSet.tables.StringTable11);
        this.tableTestTable = TestTable.createFromTable(dataSet.tables.TestTable);
    }

    public readFromFile(filename: string, verifyRevision: boolean): void {
        this.readFromDataSet(reader.CremaReader.readFromFile(filename), verifyRevision);
    }
}
