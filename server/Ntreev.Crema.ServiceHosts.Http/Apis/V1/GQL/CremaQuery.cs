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
using System.ComponentModel.Composition;
using System.Linq;
using GraphQL.Types;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL.Models;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL.Types;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL
{
    [Export(typeof(CremaQuery))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class CremaQuery : ObjectGraphType
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public CremaQuery(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
            this.Name = "Query";

            Field<DataBaseType>()
                .Name("Database")
                .Argument<StringGraphType>("name", "")
                .Argument<StringGraphType>("tags", "")
                .Resolve(context =>
                {
                    var dataBaseName = context.GetArgument<string>("name");
                    var tags = context.GetArgument<string>("tags");
                    var database = this.GetDataBase(dataBaseName);

                    return this.cremaHost.Dispatcher.Invoke(() =>
                    {
                        var info = database.DataBaseInfo;
                        if (string.IsNullOrWhiteSpace(tags))
                        {
                            return database.Dispatcher.Invoke(() => DataBaseModel.ConvertFrom(database, info));
                        }
                        else
                        {
                            info.Tags = (TagInfo)tags;
                            info.Paths = this.GetItems(database, (TagInfo)tags).ToArray();
                            info.TypesHashValue = this.GetTypesHashValue(database, (TagInfo)tags);
                            info.TablesHashValue = this.GetTablesHashValue(database, (TagInfo)tags);
                            return DataBaseModel.ConvertFrom(database, info);
                        }
                    });
                });

            Field<ListGraphType<DataBaseType>>()
                .Name("Databases")
                .Argument<StringGraphType, string>("name", "", "*")
                .Argument<StringGraphType>("tags", "")
                .Resolve(context =>
                {
                    var dataBaseName = context.GetArgument<string>("name");
                    var tags = context.GetArgument<string>("tags");
                    var databases = this.GetDataBases(dataBaseName);

                    return this.cremaHost.Dispatcher.Invoke(() =>
                    {
                        if (string.IsNullOrWhiteSpace(tags))
                        {
                            return databases.Select(database =>
                            {
                                return database.Dispatcher.Invoke(() => DataBaseModel.ConvertFrom(database, database.DataBaseInfo));
                            }).ToArray();
                        }

                        return databases.Select(database =>
                        {
                            return database.Dispatcher.Invoke(() =>
                            {
                                var info = database.DataBaseInfo;
                                info.Tags = (TagInfo)tags;
                                info.Paths = this.GetItems(database, (TagInfo)tags).ToArray();
                                info.TypesHashValue = this.GetTypesHashValue(database, (TagInfo)tags);
                                info.TablesHashValue = this.GetTablesHashValue(database, (TagInfo)tags);
                                return DataBaseModel.ConvertFrom(database, info);
                            });
                        }).ToArray();
                    });
                });
        }

        private IDataBase GetDataBase(string databaseName)
        {
            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));

            return this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(databaseName) == false)
                    throw new DataBaseNotFoundException(databaseName);
                return this.cremaHost.DataBases[databaseName];
            });
        }

        private IDataBase[] GetDataBases(string databaseName)
        {
            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));

            return this.cremaHost.Dispatcher.Invoke(() =>
                {
                    return this.cremaHost.DataBases.ToArray()
                        .Where(database => database.Dispatcher.Invoke(() => database.Name.Glob(databaseName)));
                }).ToArray();
        }

        private string GetTypesHashValue(IDataBase dataBase, TagInfo tags)
        {
            var typeInfoList = new List<TypeInfo>();
            if (dataBase.TypeContext != null)
            {
                foreach (var item in dataBase.TypeContext.Types.OrderBy(item => item.Name))
                {
                    if ((item.TypeInfo.Tags & tags) != TagInfo.Unused)
                    {
                        var typeInfo = item.TypeInfo;
                        typeInfoList.Add(typeInfo);
                    }
                }
            }

            return CremaDataSet.GenerateHashValue(typeInfoList.ToArray());
        }

        private string GetTablesHashValue(IDataBase dataBase, TagInfo tags)
        {
            var tableInfoList = new List<TableInfo>();
            if (dataBase.TableContext != null)
            {
                foreach (var item in dataBase.TableContext.Tables.OrderBy(item => item.Name))
                {
                    if ((item.TableInfo.Tags & tags) != TagInfo.Unused)
                    {
                        var tableInfo = item.TableInfo.Filter(tags);
                        tableInfoList.Add(tableInfo);
                    }
                }
            }

            return CremaDataSet.GenerateHashValue(tableInfoList.ToArray());
        }

        private IEnumerable<string> GetItems(IDataBase dataBase, TagInfo tags)
        {
            yield return PathUtility.Separator;

            if (dataBase.TypeContext != null)
            {
                foreach (var item in dataBase.TypeContext)
                {
                    if (item is IType type && (type.TypeInfo.Tags & tags) == TagInfo.Unused)
                    {
                        continue;
                    }

                    yield return PathUtility.Separator + Crema.Data.Xml.Schema.CremaSchema.TypeDirectory + item.Path;
                }
            }

            if (dataBase.TableContext != null)
            {
                foreach (var item in dataBase.TableContext)
                {
                    if (item is ITable table && (table.TableInfo.DerivedTags & tags) == TagInfo.Unused)
                    {
                        continue;
                    }

                    yield return PathUtility.Separator + Crema.Data.Xml.Schema.CremaSchema.TableDirectory + item.Path;
                }
            }
        }
    }
}
