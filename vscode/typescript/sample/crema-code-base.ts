import reader = require("./crema-code-reader");

export abstract class CremaData {
    public error_occured: (error: Error) => any;

    protected onErrorOccured(error: Error): boolean {
        if (this.error_occured == null) {
            return false;
        }
        return this.error_occured(error);
    }
}

export abstract class CremaRow {
    private _relationID: string;
    private _parentID: string;
    private _key: string;
    private _parentInternal: CremaRow;

    constructor(row: reader.IRow) {
        for (let i in row.table.columns) {
            if (row.table.columns.hasOwnProperty(i)) {
                let column: reader.IColumn = row.table.columns[i];
                if (column.name === "__RelationID__") {
                    this._relationID = row.getValue(column.index).toString();
                } else if (column.name === "__ParentID__") {
                    this._parentID = row.getValue(column.index).toString();
                }
            }
        }
    }

    public getRelationID(): string {
        return this._relationID;
    }

    public getParentID(): string {
        return this._parentID;
    }

    public getKey(): any {
        return this._key;
    }

    protected setKey(...keys: any[]): void {
        this._key = CremaUtility.generateHashCode(keys);
    }

    protected static setParent<T extends CremaRow, U extends CremaRow>(parent: T, childs: Array<U>): void {
        for (let i: number = 0; i < childs.length; i++) {
            let item: U = childs[i];
            item._parentInternal = parent;
        }
    }

    protected get parentInternal(): CremaRow {
        return this._parentInternal;
    }
}

export abstract class CremaTable<T extends CremaRow> {
    private _rows: Array<T>;
    private _keyToRow: { [key: string]: T; } = {};
    private _name: string;
    private _tableName: string;

    public get rows(): Array<T> {
        if (this._rows == null) {
            return [];
        }
        return this._rows;
    }

    /** @description 테이블의 이름을 나타냅니다. 자식 테이블의 이름이 a.b. 일 경우 a.b를 반환합니다.
      */
    public get name(): string {
        return this._name;
    }

    /** @description 부모 이름을 제외한 테이블의 이름을 나타냅니다. 만약 자식 테이블의 이름이 a.b 일 경우 b를 반환합니다.
      */
    public get tableName(): string {
        return this._tableName;
    }

    protected readFromTable(table: reader.ITable): void {
        this._rows = new Array<T>(table.rows.length);
        this._name = table.name;
        this._tableName = this.getTableName(table.name);
        for (let i: number = 0; i < table.rows.length; i++) {
            let item: reader.IRow = table.rows[i];
            let row: T = this.createRowInstance(item, this);
            this._keyToRow[row.getKey()] = row;
            this._rows[i] = row;
        }
    }

    protected readFromRows(name: string, rows: Array<T>): void {
        this._name = name;
        this._tableName = this.getTableName(name);
        if (rows.length === 0) {
            return;
        }

        for (let i: number = 0; i < rows.length; i++) {
            let item: T = rows[i];
            this._keyToRow[item.getKey()] = item;
        }
        this._rows = rows;
    }

    protected abstract createRowInstance(row: reader.IRow, table: any): T;

    public findRow(...keys: any[]): T {
        if (this._keyToRow == null) {
            return null;
        }

        let hashCode: string = CremaUtility.generateHashCode(keys);

        if (this._keyToRow[hashCode] == null) {
            return null;
        }
        return this._keyToRow[hashCode];
    }

    protected setRelations<U extends CremaRow>(childName: string, childs: Array<U>, setChildsAction: (target: T, childName: string, childs: Array<U>) => void): void {
        var relationToRow: { [key: string]: T; } = {};

        for (let i: number = 0; i < this.rows.length; i++) {
            let item: T = this.rows[i];
            relationToRow[item.getRelationID()] = item;
        }

        let rowToChilds: { [key: string]: Array<U>; } = {};
        let count: number = 0;

        for (let i: number = 0; i < childs.length; i++) {
            let child: U = childs[i];
            let parent: T = relationToRow[child.getParentID()];
            if (parent == null) {
                continue;
            }

            let arr: U[] = rowToChilds[child.getParentID()];
            if (arr == null) {
                rowToChilds[child.getParentID()] = new Array<U>();
                count++;
            }

            rowToChilds[child.getParentID()].push(child);
        }

        for (var key in rowToChilds) {
            if (rowToChilds.hasOwnProperty(key)) {
                setChildsAction(relationToRow[key], childName, rowToChilds[key]);
            }
        }
    }

    private getTableName(name: string): string {
        let value: string[] = name.split(".");
        if (value.length === 1) {
            return name;
        }
        return value[1];
    }
}

class CremaUtility {
    public static generateHashCode(...args: any[]): string {
        let hash: string = args.map(i => i.toString()).join(", ");
        return hash;
    }
}
