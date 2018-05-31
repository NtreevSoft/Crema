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
using Ntreev.Crema.Data.Xml.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    class DataTypeCollection : Dictionary<CremaDataType, Type>
    {
        private readonly DataBase dataBase;

        public DataTypeCollection(CremaDataSet dataSet, DataBase dataBase)
        {
            this.dataBase = dataBase;

            foreach (var item in dataSet.Types)
            {
                var table = dataBase.TypeContext.Types[item.Name, item.CategoryPath];
                this.Add(item, table);
            }
        }

        public void Modify(DataBaseRepositoryHost repository)
        {
            this.Modify(repository, item => true);
        }

        public void Modify(DataBaseRepositoryHost repository, Func<Type, bool> predicate)
        {
            var uri = new Uri(this.dataBase.TypeContext.BasePath);

            foreach (var item in this)
            {
                var dataType = item.Key;
                var type = item.Value;

                if (predicate(type) == false)
                    continue;

                var path1 = type.Path;
                var path2 = dataType.CategoryPath + dataType.Name;

                var schemaUri = new Uri(uri.ToString() + path1 + CremaSchema.SchemaExtension);

                if (dataType.DataSet == null)
                {
                    repository.Delete(schemaUri.LocalPath);
                }
                else
                {
                    File.WriteAllText(schemaUri.LocalPath, dataType.GetXmlSchema(), Encoding.UTF8);

                    if (path1 != path2)
                    {
                        var targetSchemaUri = new Uri(uri.ToString() + path2 + CremaSchema.SchemaExtension);

                        repository.Move(schemaUri.LocalPath, targetSchemaUri.LocalPath);
                    }
                }
            }
        }
    }
}
