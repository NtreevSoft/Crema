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
using System.ComponentModel.Composition;
using System.Linq;
using GraphQL.Types;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL.Models;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL.Types
{
    [Export(typeof(DataBaseType))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataBaseType : ObjectGraphType<DataBaseModel>
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseType(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;

            Field(_ => _.Id, type: typeof(IdGraphType)).Name("ID");
            Field(_ => _.Name);
            Field(_ => _.Comment);
            Field(_ => _.CreatedDateTime, type: typeof(DateTimeGraphType));
            Field(_ => _.Creator);
            Field(_ => _.ModifiedDateTime, type: typeof(DateTimeGraphType));
            Field(_ => _.Modifier);
            Field(_ => _.Paths);
            Field(_ => _.Revision);
            Field(_ => _.TablesHashValue, nullable: true);
            Field(_ => _.TypesHashValue, nullable: true);
            Field(_ => _.Tags);

            Field<BooleanGraphType>()
                .Name("IsLoaded")
                .Resolve(context =>
                {
                    return this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases[context.Source.Name].IsLoaded);
                });

            Field<BooleanGraphType>()
                .Name("IsEntered")
                .Resolve(context =>
                {
                    return this.cremaHost.Dispatcher.Invoke(() =>
                    {
                        var authentication = context.UserContext as Authentication;
                        var database = this.cremaHost.DataBases[context.Source.Name];
                        return database.Contains(authentication);
                    });
                });

            Field<TableType>()
                .Name("Table")
                .Argument<StringGraphType>("name", "")
                .Resolve(context =>
                {
                    var tableName = context.GetArgument<string>("name");
                    var table = this.GetTable(context.Source.Name, tableName);

                    return table.Dispatcher.Invoke(() =>
                    {
                        var tableInfo = table.TableInfo;
                        return TableModel.ConvertFrom(context.Source.DataBase, table, tableInfo);
                    });
                });

            Field<ListGraphType<TableType>>()
                .Name("Tables")
                .Argument<StringGraphType, string>("name", "", "*")
                .Argument<StringGraphType>("tags", "")
                .Resolve(context =>
                {
                    var tableName = context.GetArgument<string>("name");
                    var tags = context.GetArgument<string>("tags");
                    var tables = this.GetTables(context.Source.Name, tableName, tags);

                    return tables?.Select(table =>
                    {
                        return table.Dispatcher.Invoke(() =>
                        {
                            var tableInfo = table.TableInfo;
                            return TableModel.ConvertFrom(context.Source.DataBase, table, tableInfo);
                        });
                    }).ToArray();
                });

            Field<TypeType>()
                .Name("Type")
                .Argument<StringGraphType>("name", "")
                .Resolve(context =>
                {
                    var typeName = context.GetArgument<string>("name");
                    var type = this.GetType(context.Source.Name, typeName);

                    return type?.Dispatcher.Invoke(() =>
                    {
                        var typeInfo = type.TypeInfo;
                        return TypeModel.ConvertFrom(context.Source.DataBase, type, typeInfo);
                    });
                });

            Field<ListGraphType<TypeType>>()
                .Name("Types")
                .Argument<StringGraphType, string>("name", "", "*")
                .Resolve(context =>
                {
                    var typeName = context.GetArgument<string>("name");
                    var types = this.GetTypes(context.Source.Name, typeName);

                    return types?.Select(type =>
                    {
                        return type.Dispatcher.Invoke(() =>
                        {
                            var typeInfo = type.TypeInfo;
                            return TypeModel.ConvertFrom(context.Source.DataBase, type, typeInfo);
                        });
                    }).ToArray();
                });

            Field<ListGraphType<LogInfoType>>()
                .Name("Logs")
                .Resolve(context =>
                {
                    var authentication = context.UserContext as Authentication;
                    var database = context.Source.DataBase;
                    var logs = database.Dispatcher.Invoke(() => database.GetLog(authentication));

                    return LogInfoModel.ConvertFrom(logs);
                });
        }

        private ITable[] GetTables(string dataBaseName, string tableName, string tags = null)
        {
            if (dataBaseName == null)
                throw new ArgumentNullException(nameof(dataBaseName));

            var dataBase = this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(dataBaseName) == false)
                    throw new DataBaseNotFoundException(dataBaseName);
                return this.cremaHost.DataBases[dataBaseName];
            });

            return dataBase.Dispatcher.Invoke(() =>
            {
                if (dataBase.TableContext == null) return null;

                var tables = dataBase.TableContext?.Tables.ToArray()
                    .Where(table => table.Dispatcher.Invoke(() => table.Name.Glob(tableName)));

                if (string.IsNullOrWhiteSpace(tags)) return tables.ToArray();

                var tagInfo = (TagInfo) tags;
                return tables.Where(table =>
                {
                    return table.Dispatcher.Invoke(() => (table.TableInfo.DerivedTags & tagInfo) != TagInfo.Unused);
                }).ToArray();
            });
        }

        private ITable GetTable(string dataBaseName, string tableName)
        {
            if (dataBaseName == null)
                throw new ArgumentNullException(nameof(dataBaseName));

            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));

            var dataBase = this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(dataBaseName) == false)
                    throw new DataBaseNotFoundException(dataBaseName);
                return this.cremaHost.DataBases[dataBaseName];
            });

            return dataBase.Dispatcher.Invoke(() =>
            {
                var table = dataBase.TableContext.Tables[tableName];
                if (table == null)
                    throw new TableNotFoundException(tableName);
                return table;
            });
        }

        private IType GetType(string dataBaseName, string typeName)
        {
            if (dataBaseName == null)
                throw new ArgumentNullException(nameof(dataBaseName));

            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            var dataBase = this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(dataBaseName) == false)
                    throw new DataBaseNotFoundException(dataBaseName);
                return this.cremaHost.DataBases[dataBaseName];
            });

            return dataBase.Dispatcher.Invoke(() =>
            {
                if (dataBase.TypeContext == null) return null;

                var type = dataBase.TypeContext.Types[typeName];
                if (type == null)
                    throw new TableNotFoundException(typeName);
                return type;
            });
        }

        private IType[] GetTypes(string dataBaseName, string typeName)
        {
            if (dataBaseName == null)
                throw new ArgumentNullException(nameof(dataBaseName));

            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));

            var dataBase = this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(dataBaseName) == false)
                    throw new DataBaseNotFoundException(dataBaseName);
                return this.cremaHost.DataBases[dataBaseName];
            });

            return dataBase.Dispatcher.Invoke(() =>
            {
                if (dataBase.TypeContext == null) return null;

                var types = dataBase.TypeContext?.Types.ToArray()
                    .Where(table => table.Dispatcher.Invoke(() => table.Name.Glob(typeName)));
                return types.ToArray();
            });
        }
    }
}
