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

        static CremaDataTypeUtility()
        {
            baseTypes.Add("boolean", typeof(System.Boolean));
            baseTypes.Add("string", typeof(System.String));
            baseTypes.Add("int", typeof(System.Int32));
            baseTypes.Add("float", typeof(System.Single));
            baseTypes.Add("double", typeof(System.Double));
            baseTypes.Add("dateTime", typeof(System.DateTime));
            baseTypes.Add("unsignedInt", typeof(System.UInt32));
            baseTypes.Add("long", typeof(System.Int64));
            baseTypes.Add("short", typeof(System.Int16));
            baseTypes.Add("unsignedLong", typeof(System.UInt64));
            baseTypes.Add("unsignedByte", typeof(System.Byte));
            baseTypes.Add("duration", typeof(System.TimeSpan));
            baseTypes.Add("unsignedShort", typeof(System.UInt16));
            baseTypes.Add("byte", typeof(System.SByte));
            baseTypes.Add("guid", typeof(System.Guid));

            typeToName.Add(typeof(System.Boolean), "boolean");
            typeToName.Add(typeof(System.String), "string");
            typeToName.Add(typeof(System.Int32), "int");
            typeToName.Add(typeof(System.Single), "float");
            typeToName.Add(typeof(System.Double), "double");
            typeToName.Add(typeof(System.DateTime), "dateTime");
            typeToName.Add(typeof(System.UInt32), "unsignedInt");
            typeToName.Add(typeof(System.Int64), "long");
            typeToName.Add(typeof(System.Int16), "short");
            typeToName.Add(typeof(System.UInt64), "unsignedLong");
            typeToName.Add(typeof(System.Byte), "unsignedByte");
            typeToName.Add(typeof(System.TimeSpan), "duration");
            typeToName.Add(typeof(System.UInt16), "unsignedShort");
            typeToName.Add(typeof(System.SByte), "byte");
            typeToName.Add(typeof(System.Guid), "guid");
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
