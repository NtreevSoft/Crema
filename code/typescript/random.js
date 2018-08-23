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
    // boolean
    // string
    // single
    // double
    // int8
    // uint8
    // int16
    // uint16
    // int32
    // uint32
    // int64
    // uint64
    // datetime
    // duration
    // guid
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
    RandomUtility.nextSingle = function () {
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
            case "single":
                return RandomUtility.nextSingle();
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
                throw "not implementaion";
        }
    };
    return RandomUtility;
}());
