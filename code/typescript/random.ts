
class RandomUtility {
    public static next(maxValue: number): number {
        return Math.floor((Math.random() * maxValue));
    }

    public static nextInRange(minValue: number, maxValue: number): number {
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    }

    public static nextIdentifier(): string {
        let text: string = "";
        let first: string = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        let possible: string = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        let count: number = RandomUtility.nextInRange(5, 15);

        text += possible.charAt(RandomUtility.next(first.length));
        for (let i: number = 0; i < count; i++) {
            text += possible.charAt(RandomUtility.next(possible.length));
        }
        return text;
    }

    public static nextItem<T>(items: T[]): T {
        let index: number = RandomUtility.next(items.length);
        return items[index];
    }

    public static Within(percent: number): boolean {
        return percent >= RandomUtility.next(100);
    }

    public static nextBoolean(): boolean {
        return RandomUtility.next(2) === 0;
    }

    public static nextString(): string {
        let count: number = RandomUtility.nextInRange(2, 5);
        let list: Array<string> = [];
        for (let i: number = 0; i < count; i++) {
            list.push(RandomUtility.nextIdentifier());
        }
        return list.join(" ");
    }

    public static nextFloat(): number {
        let minValue: number = -3.40282347E+38;
        let maxValue: number = 3.40282347E+38;
        return Math.random() * (maxValue - minValue) + minValue;
    }

    public static nextDouble(): number {
        return Math.random();
    }

    public static nextInt8(): number {
        let minValue: number = -128;
        let maxValue: number = 127;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    }

    public static nextUInt8(): number {
        let minValue: number = 0;
        let maxValue: number = 255;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    }

    public static nextInt16(): number {
        let minValue: number = -32768;
        let maxValue: number = 32767;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    }

    public static nextUInt16(): number {
        let minValue: number = 0;
        let maxValue: number = 65535;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    }

    public static nextInt32(): number {
        let minValue: number = -2147483648;
        let maxValue: number = 2147483647;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    }

    public static nextUInt32(): number {
        let minValue: number = 0;
        let maxValue: number = 4294967295;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    }

    public static nextInt64(): number {
        let minValue: number = -9223372036854775808;
        let maxValue: number = 9223372036854775807;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    }

    public static nextUInt64(): number {
        let minValue: number = 0;
        let maxValue: number = 18446744073709551615;
        return Math.floor(Math.random() * (maxValue - minValue)) + minValue;
    }

    public static nextDateTime(): Date {
        return new Date(RandomUtility.next(253402300800000));
    }

    public static nextDuration(): number {
        let minValue: number = -922337203685.47754;
        let maxValue: number = 922337203685.47754;
        return Math.random() * (maxValue - minValue) + minValue;
    }

    public static nextGuid(): string {
        function s4(): string {
            return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        }
        return s4() + s4() + "-" + s4() + "-" + s4() + "-" + s4() + "-" + s4() + s4() + s4();
    }

    public static nextByType(typeName: string): any {
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
    }
}