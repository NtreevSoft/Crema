using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    [Export(typeof(IObjectSerializer))]
    class XmlObjectSerializer : IObjectSerializer
    {
        public string Name => "xml";

        public object Deserialize(Type type, string itemPath, object state)
        {
            if (type == typeof(CremaDataTable))
            {
                throw new NotImplementedException();
            }
            else if (type == typeof(CremaDataSet))
            {
                var info = (Data.DataSetDeserializationInfo)state;
                var dataSet = CremaDataSet.Create(info.SignatureDateProvider);

                var typePaths = info.TypePaths.Select(item => item + CremaSchema.SchemaExtension).ToArray();
                var tablePaths = info.TablePaths.Select(item => item + CremaSchema.XmlExtension).ToArray();

                dataSet.ReadMany(typePaths, tablePaths);
                dataSet.AcceptChanges();

                return dataSet;
            }
            else
            {
                var filename = itemPath + CremaSchema.XmlExtension;
                return DataContractSerializerUtility.Read(filename, type);
            }
        }

        public string[] Serialize(object obj, string itemPath, object state)
        {
            if (obj is CremaDataTable dataTable)
            {
                return this.SerializeDataTable(dataTable, itemPath, state);
            }
            else if(obj is CremaDataType dataType)
            {
                return this.SerializeDataType(dataType, itemPath, state);
            }
            else
            {
                return this.SerializeObject(obj, itemPath);
            }
        }

        public string[] VerifyPath(Type type, string itemPath, object state)
        {
            if (type == typeof(CremaDataTable))
            {
                var schemaPath = itemPath + CremaSchema.SchemaExtension;
                var xmlPath = itemPath + CremaSchema.XmlExtension;
                var ns = state as string;
                if (string.IsNullOrEmpty(ns) == true)
                {
                    return new string[] { schemaPath, xmlPath, };
                }
                else
                {
                    return new string[] { xmlPath, };
                }
            }
            else if (type == typeof(CremaDataType))
            {
                var filename = itemPath + CremaSchema.SchemaExtension;
                return new string[] { filename };
            }
            else
            {
                var filename = itemPath + CremaSchema.XmlExtension;
                return new string[] { filename };
            }
        }

        public string[] GetItemPaths(string path, Type type, object state)
        {
            if (type == typeof(CremaDataTable))
            {
                throw new NotImplementedException();
            }
            else if (type == typeof(CremaDataType))
            {
                throw new NotImplementedException();
            }
            else
            {
                var directories = DirectoryUtility.GetAllDirectories(path, "*", true);
                var files = DirectoryUtility.GetAllFiles(path, "*.xml").Select(item => FileUtility.RemoveExtension(item));
                return directories.Concat(files).OrderBy(item => item).ToArray();
            }
        }

        public static readonly XmlObjectSerializer Default = new XmlObjectSerializer();

        private string[] SerializeDataTable(CremaDataTable dataTable, string itemPath, object state)
        {
            var schemaPath = itemPath + CremaSchema.SchemaExtension;
            var xmlPath = itemPath + CremaSchema.XmlExtension;
            var ns = state as string;
            if (string.IsNullOrEmpty(ns) == true)
            {
                File.WriteAllText(schemaPath, dataTable.GetXmlSchema(), Encoding.UTF8);
                File.WriteAllText(xmlPath, dataTable.GetXml(), Encoding.UTF8);

                return new string[] { schemaPath, xmlPath, };
            }
            else
            {
                File.WriteAllText(xmlPath, dataTable.GetXml(), Encoding.UTF8);
                return new string[] { xmlPath, };
            }
        }

        private string[] SerializeDataType(CremaDataType dataType, string itemPath, object state)
        {
            var schemaPath = itemPath + CremaSchema.SchemaExtension;
            File.WriteAllText(schemaPath, dataType.GetXmlSchema(), Encoding.UTF8);
            return new string[] { schemaPath, };
        }

        private string[] SerializeObject(object obj, string itemPath)
        {
            var filename = itemPath + CremaSchema.XmlExtension;
            DataContractSerializerUtility.Write(filename, obj, true);
            return new string[] { filename };
        }
    }
}
