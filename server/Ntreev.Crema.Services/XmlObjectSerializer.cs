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

using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Ntreev.Crema.Services
{
    [Export(typeof(IObjectSerializer))]
    class XmlObjectSerializer : IObjectSerializer
    {
        public string Name => "xml";

        public object Deserialize(string itemPath, Type type, ObjectSerializerSettings settings)
        {
            if (type == typeof(CremaDataTable))
            {
                throw new NotImplementedException();
            }
            else if (type == typeof(CremaDataType))
            {
                var extension = settings.Extension != string.Empty ? settings.Extension : CremaSchema.SchemaExtension;
                var filename = itemPath + extension;
                var dataSet = new CremaDataSet();
                var dataType = CremaDataType.ReadSchema(filename);
                dataSet.Types.Add(dataType);
                return dataType;
            }
            else if (type == typeof(CremaDataSet))
            {
                if (settings is CremaDataSetSerializerSettings props)
                {
                    var dataSet = CremaDataSet.Create(props.SignatureDateProvider);
                    var typePaths = props.TypePaths.Select(item => item + CremaSchema.SchemaExtension).ToArray();
                    var tablePaths = props.TablePaths.Select(item => item + CremaSchema.XmlExtension).ToArray();

                    dataSet.ReadMany(typePaths, tablePaths, props.SchemaOnly);
                    dataSet.AcceptChanges();

                    return dataSet;
                }
                else
                {
                    return CremaDataSet.ReadFromDirectory(itemPath);
                }
            }
            else
            {
                var extension = settings.Extension != string.Empty ? settings.Extension : CremaSchema.XmlExtension;
                var filename = itemPath + extension;
                return DataContractSerializerUtility.Read(filename, type);
            }
        }

        public string[] Serialize(string itemPath, object obj, ObjectSerializerSettings settings)
        {
            if (obj is CremaDataTable dataTable)
            {
                return this.SerializeDataTable(dataTable, itemPath, settings);
            }
            else if (obj is CremaDataType dataType)
            {
                return this.SerializeDataType(dataType, itemPath, settings);
            }
            else if (obj is CremaDataSet dataSet)
            {
                dataSet.WriteToDirectory(itemPath);
                var items1 = DirectoryUtility.GetAllFiles(itemPath, "*.xml");
                var items2 = DirectoryUtility.GetAllFiles(itemPath, "*.xsd");
                return items1.Concat(items2).OrderBy(item => item).ToArray();
            }
            else
            {
                return this.SerializeObject(obj, itemPath, settings);
            }
        }

        public string[] GetPath(string itemPath, Type type, ObjectSerializerSettings settings)
        {
            if (type == typeof(CremaDataTable))
            {
                var xmlPath = itemPath + CremaSchema.XmlExtension;
                if (settings is CremaDataTableSerializerSettings prop && prop.RelativePath != string.Empty)
                {
                    return new string[] { xmlPath };
                }
                else
                {
                    if (File.Exists(xmlPath) == true)
                    {
                        var xmlInfo = new CremaXmlReadInfo(xmlPath);
                        return new string[] { xmlPath, xmlInfo.SchemaPath, };
                    }
                    else
                    {
                        var schemaPath = itemPath + CremaSchema.SchemaExtension;
                        return new string[] { xmlPath, schemaPath, };
                    }
                }
            }
            else if (type == typeof(CremaDataType))
            {
                var filename = itemPath + CremaSchema.SchemaExtension;
                return new string[] { filename };
            }
            else
            {
                var extension = settings.Extension != string.Empty ? settings.Extension : CremaSchema.XmlExtension;
                var filename = itemPath + extension;
                return new string[] { filename };
            }
        }

        public string[] GetReferencedPath(string itemPath, Type type, ObjectSerializerSettings settings)
        {
            if (type == typeof(CremaDataTable))
            {
                var fileList = new List<string>();
                var xmlPath = itemPath + CremaSchema.XmlExtension;
                var xmlInfo = new CremaXmlReadInfo(xmlPath);
                if (xmlInfo.ItemName != null)
                    fileList.Add(xmlInfo.SchemaPath);
                var schemaInfo = new CremaSchemaReadInfo(xmlInfo.SchemaPath);
                fileList.AddRange(schemaInfo.LocalTypePaths);
                return fileList.ToArray();
            }
            else if (type == typeof(CremaDataType))
            {
                return new string[] { };
            }
            else
            {
                return new string[] { };
            }
        }

        public string[] GetItemPaths(string path, Type type, ObjectSerializerSettings settings)
        {
            if (type == typeof(CremaDataTable))
            {
                return DirectoryUtility.GetAllFiles(path, "*" + CremaSchema.XmlExtension).Select(item => FileUtility.RemoveExtension(item)).ToArray();
            }
            else if (type == typeof(CremaDataType))
            {
                return DirectoryUtility.GetAllFiles(path, "*" + CremaSchema.SchemaExtension).Select(item => FileUtility.RemoveExtension(item)).ToArray();
            }
            else
            {
                var extension = settings.Extension != string.Empty ? settings.Extension : CremaSchema.XmlExtension;
                var directories = DirectoryUtility.GetAllDirectories(path, "*", true);
                var files = DirectoryUtility.GetAllFiles(path, $"*{extension}").Select(item => FileUtility.RemoveExtension(item));
                return files.OrderBy(item => item).ToArray();
            }
        }

        public void Validate(string itemPath, Type type, ObjectSerializerSettings settings)
        {
            if (type == typeof(CremaDataSet))
            {
                var sb = new StringBuilder();
                CremaDataSet.ValidateDirectory(itemPath, null, DefaultValidationEventHandler);
                if (sb.ToString() != string.Empty)
                    throw new XmlSchemaValidationException(sb.ToString());

                void DefaultValidationEventHandler(object sender, ValidationEventArgs item)
                {
                    sb.AppendLine(item.Exception.SourceUri);

                    if (item.Severity == XmlSeverityType.Error)
                    {
                        sb.Append("error: ");
                        sb.AppendLine(new XmlException(item.Message, item.Exception, item.Exception.LineNumber, item.Exception.LinePosition).Message);
                    }
                    else
                    {
                        sb.Append("warning: ");
                        sb.AppendLine(item.Message);
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static readonly XmlObjectSerializer Default = new XmlObjectSerializer();

        private string[] SerializeDataTable(CremaDataTable dataTable, string itemPath, ObjectSerializerSettings settings)
        {
            var xmlPath = itemPath + CremaSchema.XmlExtension;

            if (settings is CremaDataTableSerializerSettings prop && prop.RelativePath != string.Empty)
            {
                File.WriteAllText(xmlPath, dataTable.GetXml(), Encoding.UTF8);
                return new string[] { xmlPath };
            }
            else
            {
                var schemaPath = itemPath + CremaSchema.SchemaExtension;
                File.WriteAllText(schemaPath, dataTable.GetXmlSchema(), Encoding.UTF8);
                File.WriteAllText(xmlPath, dataTable.GetXml(), Encoding.UTF8);

                return new string[] { xmlPath, schemaPath };
            }
        }

        private string[] SerializeDataType(CremaDataType dataType, string itemPath, object state)
        {
            var schemaPath = itemPath + CremaSchema.SchemaExtension;
            File.WriteAllText(schemaPath, dataType.GetXmlSchema(), Encoding.UTF8);
            return new string[] { schemaPath };
        }

        private string[] SerializeObject(object obj, string itemPath, ObjectSerializerSettings settings)
        {
            var extension = settings.Extension != string.Empty ? settings.Extension : CremaSchema.XmlExtension;
            var filename = itemPath + extension;
            DataContractSerializerUtility.Write(filename, obj, true);
            return new string[] { filename };
        }

        public bool Exists(string itemPath, Type type, ObjectSerializerSettings settings)
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
                var extension = settings.Extension != string.Empty ? settings.Extension : CremaSchema.XmlExtension;
                var filename = itemPath + extension;
                return File.Exists(filename);
            }
        }
    }
}
