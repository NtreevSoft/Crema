// ------------------------------------------------------------------------------
// dataBase: default
// revision: 412
// requested revision: -1
// devmode: False
// hash value: 4b1da7ae630f37090a4cee6e3e446893c6ad9297
// tags: All
// ------------------------------------------------------------------------------
declare module 'crema-code' {

    export class CremaRow {
    }

    export class CremaTable<T extends CremaRow> {

        public rows: T[];
    }

    export class CremaData {
    }

    // creator: admin
    // createdDateTime: 2017-10-25 오전 8:40:06
    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 8:15:33
    export enum KindType {

        // creator: admin
        // createdDateTime: 2017-10-25 오전 8:40:25
        // modifier: admin
        // modifiedDateTime: 2017-10-25 오전 8:40:25
        Common = 0,

        // creator: admin
        // createdDateTime: 2017-10-25 오전 8:40:28
        // modifier: admin
        // modifiedDateTime: 2017-10-25 오전 8:40:28
        Client = 1,

        // creator: admin
        // createdDateTime: 2017-10-25 오전 8:40:31
        // modifier: admin
        // modifiedDateTime: 2017-10-25 오전 8:40:31
        Server = 2,

        // creator: admin
        // createdDateTime: 2017-10-25 오전 8:40:36
        // modifier: admin
        // modifiedDateTime: 2017-10-25 오전 8:40:36
        Tools = 3,

        // creator: admin
        // createdDateTime: 2017-10-25 오전 8:41:45
        // modifier: admin
        // modifiedDateTime: 2017-10-25 오전 8:41:45
        SubModule = 4,

        // creator: admin
        // createdDateTime: 2017-11-01 오전 8:15:33
        // modifier: admin
        // modifiedDateTime: 2017-11-01 오전 8:15:33
        Executable = 5,
    }

    // creator: admin
    // createdDateTime: 2017-11-01 오전 8:18:26
    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 8:21:51
    export enum ProjectType {

        // creator: admin
        // createdDateTime: 2017-11-01 오전 8:19:07
        // modifier: admin
        // modifiedDateTime: 2017-11-01 오전 8:19:07
        Library = 0,

        // creator: admin
        // createdDateTime: 2017-11-01 오전 8:21:30
        // modifier: admin
        // modifiedDateTime: 2017-11-01 오전 8:21:50
        Executable = 1,

        // creator: admin
        // createdDateTime: 2017-11-01 오전 8:21:24
        // modifier: admin
        // modifiedDateTime: 2017-11-01 오전 8:21:51
        SharedLibrary = 2,
    }

    // creator: admin
    // createdDateTime: 2017-10-23 오전 6:51:02
    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 8:26:13
    export enum StringType {

        // creator: admin
        // createdDateTime: 2017-10-24 오전 4:56:17
        // modifier: admin
        // modifiedDateTime: 2017-10-24 오전 4:58:56
        None = 0,

        // creator: admin
        // createdDateTime: 2017-10-23 오전 6:51:13
        // modifier: admin
        // modifiedDateTime: 2017-10-24 오전 4:58:57
        Text = 1,

        // creator: admin
        // createdDateTime: 2017-10-23 오전 6:59:47
        // modifier: admin
        // modifiedDateTime: 2017-10-24 오전 4:58:58
        Title = 2,

        // creator: admin
        // createdDateTime: 2017-10-23 오전 7:00:01
        // modifier: admin
        // modifiedDateTime: 2017-10-24 오전 4:58:58
        Label = 3,

        // creator: admin
        // createdDateTime: 2017-10-23 오전 7:00:19
        // modifier: admin
        // modifiedDateTime: 2017-10-24 오전 4:58:58
        Button = 4,

        // creator: admin
        // createdDateTime: 2017-10-23 오전 7:00:23
        // modifier: admin
        // modifiedDateTime: 2017-10-24 오전 4:58:59
        Comment = 5,

        // creator: admin
        // createdDateTime: 2017-10-23 오전 7:00:27
        // modifier: admin
        // modifiedDateTime: 2017-10-24 오전 4:58:59
        Message = 6,

        // creator: admin
        // createdDateTime: 2017-10-25 오전 4:50:40
        // modifier: admin
        // modifiedDateTime: 2017-10-25 오전 4:50:40
        MenuItem = 7,

        // creator: admin
        // createdDateTime: 2017-10-25 오전 5:55:13
        // modifier: admin
        // modifiedDateTime: 2017-10-25 오전 5:55:13
        Exception = 8,

        // creator: admin
        // createdDateTime: 2017-10-25 오전 7:54:19
        // modifier: admin
        // modifiedDateTime: 2017-10-25 오전 7:54:19
        Command = 9,

        // creator: admin
        // createdDateTime: 2017-11-01 오전 7:27:01
        // modifier: admin
        // modifiedDateTime: 2017-11-01 오전 7:27:01
        Summary = 10,

        // creator: admin
        // createdDateTime: 2017-11-01 오전 8:26:13
        // modifier: admin
        // modifiedDateTime: 2017-11-01 오전 8:26:13
        Tooltip = 11,
    }

    export class ProjectInfoExportInfoRow extends CremaRow {

        public table: ProjectInfoExportInfo;

        public ID: number;

        public fileName: string;

        public tableName: string;

        public parent: ProjectInfoRow;
    }

    // createdDateTime: 2017-10-25 오전 8:34:43
    // modifiedDateTime: 2017-10-26 오전 8:06:01
    // contentsModifier: s2quake
    // contentsModifiedDateTime: 2018-03-27 오전 5:13:04
    export class ProjectInfoExportInfo extends CremaTable<ProjectInfoExportInfoRow> {

        public find(ID: number): ProjectInfoExportInfoRow;
    }

    export class ProjectInfoRow extends CremaRow {

        public table: ProjectInfo;

        public kindType: KindType;

        public projectType: ProjectType;

        public name: string;

        public projectPath: string;

        public exportInfo: ProjectInfoExportInfo;
    }

    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 8:22:24
    // contentsModifier: s2quake
    // contentsModifiedDateTime: 2018-03-27 오전 5:12:57
    export class ProjectInfo extends CremaTable<ProjectInfoRow> {

        public exportInfo: ProjectInfoExportInfo;

        public find(kindType: KindType, projectType: ProjectType, name: string): ProjectInfoRow;
    }

    export class StringTableRow extends CremaRow {

        public table: StringTable;

        public type: StringType;

        public name: string;

        public comment: string;

        public value: string;

        public ko_KR: string;
    }

    // creator: admin
    // createdDateTime: 2017-10-23 오전 7:16:01
    // modifier: admin
    // modifiedDateTime: 2017-10-25 오전 5:30:21
    export class StringTable extends CremaTable<StringTableRow> {

        public find(type: StringType, name: string): StringTableRow;
    }

    export class TestTableRow extends CremaRow {

        public table: TestTable;

        public key: number;

        public value: string;

        public dateTimePicker: Date;

        public timePicker: number;
    }

    // modifier: admin
    // modifiedDateTime: 2017-11-01 오전 4:10:41
    // contentsModifier: admin
    // contentsModifiedDateTime: 2017-11-01 오전 4:21:18
    export class TestTable extends CremaTable<TestTableRow> {

        public find(key: number): TestTableRow;
    }

    export class CremaDataSet extends CremaData {

        public name: string;

        public revision: number;

        public clientApplicationHost: StringTable;

        public clientBase: StringTable;

        public clientConsole: StringTable;

        public clientConverters: StringTable;

        public clientDifferences: StringTable;

        public clientFramework: StringTable;

        public clientServices: StringTable;

        public clientSmartSet: StringTable;

        public clientTables: StringTable;

        public clientTypes: StringTable;

        public clientUsers: StringTable;

        public commonPresentation: StringTable;

        public commonServiceModel: StringTable;

        public commonServiceModel_Event: StringTable;

        public cremaData: StringTable;

        public cremaProjectInfo: ProjectInfo;

        public modernUIFramework: StringTable;

        public ntreevLibrary: StringTable;

        public projectInfo: ProjectInfo;

        public serverServiceHosts: StringTable;

        public stringTable: StringTable;

        public stringTable11: StringTable;

        public testTable: TestTable;

        public static createFromFile(filename: string, verifyRevision: boolean): CremaDataSet;

        protected readFromFile(filename: string, verifyRevision: boolean): void;
    }
}
