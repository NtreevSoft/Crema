
declare module 'crema-reader' {

    export function readFromFile(filename: string): IDataSet;

    export class CremaReader {
        public static readFromFile(filename: string): IDataSet;
    }

    export interface IDataSet {
        tables?: { [key: string]: ITable; }
        revision?: number;
        name?: string;
        typesHashValue?: string;
        tablesHashValue?: string;
        tags?: string;
    }

    export interface ITable {
        dataSet?: IDataSet;
        name?: string;
        category?: string;
        index?: number;
        version?: number;
        hashValue?: string;
        keys?: IColumn[];
        columns?: { [key: string]: IColumn; };
        rows?: IRow[];
    }

    export interface IColumn {
        table?: ITable;
        name?: string;
        dataType?: string;
        isKey?: boolean;
        index?: number;
    }

    export interface IRow {
        table?: ITable;
        getValue(index: number): string | number | boolean
        hasValue(index: number): boolean;
        toBoolean(index: number): boolean
        toString(index: number): string;
        toSingle(index: number): number;
        toDouble(index: number): number;
        toInt8(index: number): number;
        toUInt8(index: number): number;
        toInt16(index: number): number;
        toUInt16(index: number): number;
        toInt32(index: number): number;
        toUInt32(index: number): number;
        toInt64(index: number): number;
        toUInt64(index: number): number;
        toDateTime(index: number): Date;
        toDuration(index: number): number;

        getValueByString(columnName: string): string | number | boolean | Date;
        hasValueByString(columnName: string): boolean;
        toBooleanByString(columnName: string): boolean
        toStringByString(columnName: string): string;
        toSingleByString(columnName: string): number;
        toDoubleByString(columnName: string): number;
        toInt8ByString(columnName: string): number;
        toUInt8ByString(columnName: string): number;
        toInt16ByString(columnName: string): number;
        toUInt16ByString(columnName: string): number;
        toInt32ByString(columnName: string): number;
        toUInt32ByString(columnName: string): number;
        toInt64ByString(columnName: string): number;
        toUInt64ByString(columnName: string): number;
        toDateTimeByString(columnName: string): Date;
        toDurationByString(columnName: string): number;
    }
}
