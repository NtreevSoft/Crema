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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.Services.Properties;

namespace Ntreev.Crema.Services.Data
{
    class DataTableCollection : Dictionary<CremaDataTable, Table>
    {
        private readonly DataBase dataBase;

        public DataTableCollection(CremaDataSet dataSet, DataBase dataBase)
        {
            this.dataBase = dataBase;

            foreach (var item in dataSet.Tables.Where(item => item.Parent == null))
            {
                var table = dataBase.TableContext.Tables[item.TableName, item.CategoryPath];
                this.Add(item, table);
            }
            this.Validate(dataSet, dataBase);
        }

        public void Validate(CremaDataSet dataSet, DataBase dataBase)
        {
            foreach (var item in dataSet.Types)
            {
                var type = dataBase.TypeContext.Types[item.Name];
                if (type == null)
                    throw new ItemNotFoundException(item.Name);
                if (type.Name == item.Name)
                {
                    if (type.Category.Path != item.CategoryPath)
                        throw new InvalidOperationException(string.Format(Resources.Exception_ItemPathChanged_Format, item.Name, type.Category.Path, item.CategoryPath));
                }
            }
        }

        public void Modify(IObjectSerializer serializer)
        {
            var uri = new Uri(this.dataBase.TableContext.BasePath);

            foreach (var item in this)
            {
                var dataTable = item.Key;
                var table = item.Value;

                var path1 = table.Path;
                var path2 = dataTable.CategoryPath + dataTable.TableName;

                var itemPath = this.dataBase.TableContext.GenerateTablePath(table.Category.Path, table.Name);
                //var schemaUri = new Uri(uri.ToString() + path1 + CremaSchema.SchemaExtension);
                //var xmlUri = new Uri(uri.ToString() + path1 + CremaSchema.XmlExtension);

                var templateNamespace = table.TemplatedParent?.Name;
                serializer.Serialize(dataTable, itemPath, templateNamespace);

                //if (table.TemplatedParent == null)
                //    repository.Modify(schemaUri.LocalPath, dataTable.GetXmlSchema());
                //repository.Modify(xmlUri.LocalPath, dataTable.GetXml());
            }
        }

        public void Move(DataBaseRepositoryHost repository, IObjectSerializer serializer)
        {
            var uri = new Uri(this.dataBase.TableContext.BasePath);

            foreach (var item in this)
            {
                var dataTable = item.Key;
                var table = item.Value;

                var path1 = table.Path;
                var path2 = dataTable.CategoryPath + dataTable.TableName;

                if (path1 == path2)
                    continue;

                var itemPath1 = this.dataBase.TableContext.GenerateTablePath(table.Category.Path, table.Name);
                var itemPath2 = this.dataBase.TableContext.GenerateTablePath(dataTable.CategoryPath, dataTable.Name);

                var templateNamespace = table.TemplatedParent?.Name;
                var items1 = serializer.VerifyPath(typeof(CremaDataTable), itemPath1, templateNamespace);
                var items2 = serializer.VerifyPath(typeof(CremaDataTable), itemPath2, templateNamespace);

                for (var i = 0; i < items1.Length; i++)
                {
                    repository.Move(items1[i], items2[i]);
                }

                //var schemaUri = new Uri(uri.ToString() + path1 + CremaSchema.SchemaExtension);
                //var xmlUri = new Uri(uri.ToString() + path1 + CremaSchema.XmlExtension);

                //var targetSchemaUri = new Uri(uri.ToString() + path2 + CremaSchema.SchemaExtension);
                //var targetXmlUri = new Uri(uri.ToString() + path2 + CremaSchema.XmlExtension);

                //if (table.TemplatedParent == null)
                //    repository.Move(schemaUri.LocalPath, targetSchemaUri.LocalPath);
                //repository.Move(xmlUri.LocalPath, targetXmlUri.LocalPath);
            }
        }
    }
}
