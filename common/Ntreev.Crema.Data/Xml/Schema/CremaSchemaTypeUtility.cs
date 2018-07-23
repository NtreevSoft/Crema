using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Xml.Schema
{
    public static class CremaSchemaTypeUtility
    {
        public static string GetSchemaTypeName(this Type type)
        {
            switch (type.GetTypeName())
            {
                case CremaDataTypeUtility.booleanType:
                    return "boolean";
                case CremaDataTypeUtility.stringType:
                    return "string";
                case CremaDataTypeUtility.floatType:
                    return "float";
                case CremaDataTypeUtility.doubleType:
                    return "double";
                case CremaDataTypeUtility.int8Type:
                    return "byte";
                case CremaDataTypeUtility.uint8Type:
                    return "unsignedByte";
                case CremaDataTypeUtility.int16Type:
                    return "short";
                case CremaDataTypeUtility.uint16Type:
                    return "unsignedShort";
                case CremaDataTypeUtility.int32Type:
                    return "int";
                case CremaDataTypeUtility.uint32Type:
                    return "unsignedInt";
                case CremaDataTypeUtility.int64Type:
                    return "long";
                case CremaDataTypeUtility.uint64Type:
                    return "unsignedLong";
                case CremaDataTypeUtility.datetimeType:
                    return "dateTime";
                case CremaDataTypeUtility.durationType:
                    return "duration";
                case CremaDataTypeUtility.guidType:
                    return "guid";
                default:
                    throw new NotImplementedException();
            }
        }

        public static Type GetType(string typeName)
        {
            switch (typeName)
            {
                case "boolean":
                    return typeof(bool);
                case "string":
                    return typeof(string);
                case "float":
                    return typeof(float);
                case "double":
                    return typeof(double);
                case "byte":
                    return typeof(sbyte);
                case "unsignedByte":
                    return typeof(byte);
                case "short":
                    return typeof(short);
                case "unsignedShort":
                    return typeof(ushort);
                case "int":
                    return typeof(int);
                case "unsignedInt":
                    return typeof(uint);
                case "long":
                    return typeof(long);
                case "unsignedLong":
                    return typeof(ulong);
                case "dateTime":
                    return typeof(DateTime);
                case "duration":
                    return typeof(TimeSpan);
                case "guid":
                    return typeof(Guid);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
