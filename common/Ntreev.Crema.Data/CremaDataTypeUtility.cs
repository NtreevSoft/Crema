//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ntreev.Crema.Data
{
    public static class CremaDataTypeUtility
    {
        private static Dictionary<string, Type> baseTypes = new Dictionary<string, Type>();
        private static Dictionary<Type, string> typeToName = new Dictionary<Type, string>();

        public const string booleanType = "boolean";
        public const string stringType = "string";
        public const string floatType = "float";
        public const string doubleType = "double";
        public const string int8Type = "int8";
        public const string uint8Type = "uint8";
        public const string int16Type = "int16";
        public const string uint16Type = "uint16";
        public const string int32Type = "int32";
        public const string uint32Type = "uint32";
        public const string int64Type = "int64";
        public const string uint64Type = "uint64";
        public const string datetimeType = "datetime";
        public const string durationType = "duration";
        public const string guidType = "guid";

        static CremaDataTypeUtility()
        {
            baseTypes.Add(booleanType, typeof(System.Boolean));
            baseTypes.Add(stringType, typeof(System.String));
            baseTypes.Add(floatType, typeof(System.Single));
            baseTypes.Add(doubleType, typeof(System.Double));
            baseTypes.Add(int8Type, typeof(System.SByte));
            baseTypes.Add(uint8Type, typeof(System.Byte));
            baseTypes.Add(int16Type, typeof(System.Int16));
            baseTypes.Add(uint16Type, typeof(System.UInt16));
            baseTypes.Add(int32Type, typeof(System.Int32));
            baseTypes.Add(uint32Type, typeof(System.UInt32));
            baseTypes.Add(int64Type, typeof(System.Int64));
            baseTypes.Add(uint64Type, typeof(System.UInt64));
            baseTypes.Add(datetimeType, typeof(System.DateTime));
            baseTypes.Add(durationType, typeof(System.TimeSpan));
            baseTypes.Add(guidType, typeof(System.Guid));

            typeToName.Add(typeof(System.Boolean), booleanType);
            typeToName.Add(typeof(System.String), stringType);
            typeToName.Add(typeof(System.Single), floatType);
            typeToName.Add(typeof(System.Double), doubleType);
            typeToName.Add(typeof(System.SByte), int8Type);
            typeToName.Add(typeof(System.Byte), uint8Type);
            typeToName.Add(typeof(System.Int16), int16Type);
            typeToName.Add(typeof(System.UInt16), uint16Type);
            typeToName.Add(typeof(System.Int32), int32Type);
            typeToName.Add(typeof(System.UInt32), uint32Type);
            typeToName.Add(typeof(System.Int64), int64Type);
            typeToName.Add(typeof(System.UInt64), uint64Type);
            typeToName.Add(typeof(System.DateTime), datetimeType);
            typeToName.Add(typeof(System.TimeSpan), durationType);
            typeToName.Add(typeof(System.Guid), guidType);
        }

        public static Type GetType(string typeName)
        {
            if (baseTypes.ContainsKey(typeName) == true)
                return baseTypes[typeName];

            throw new InvalidOperationException();
        }

        public static string[] GetBaseTypeNames()
        {
            var query = from item in baseTypes
                        where item.Value.IsEnum == false
                        select item.Key;

            return query.ToArray();
        }

        public static Type[] GetBaseTypes()
        {
            return typeToName.Keys.ToArray();
        }

        public static bool IsBaseType(string typeName)
        {
            return baseTypes.ContainsKey(typeName);
        }

        public static bool IsBaseType(Type type)
        {
            return typeToName.ContainsKey(type);
        }

        public static string GetTypeName(this Type type)
        {
            if (typeToName.ContainsKey(type) == true)
                return typeToName[type];

            throw new InvalidOperationException();
        }

        public static bool CanUseAutoIncrement(this Type type)
        {
            if (type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
                return true;
            return false;
        }

        public static bool CanUseAutoIncrement(string typeName)
        {
            if (baseTypes.ContainsKey(typeName) == false)
            {
                return false;
            }
            return CanUseAutoIncrement(baseTypes[typeName]);
        }
    }
}
