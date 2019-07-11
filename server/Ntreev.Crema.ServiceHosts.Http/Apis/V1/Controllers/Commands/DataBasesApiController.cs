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
using System.Web.Http;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Requests.Commands;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.Commands;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Controllers.Commands
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1/commands/databases")]
    public class DataBasesApiController : CremaApiController
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBasesApiController(ICremaHost cremaHost) : base(cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public string[] GetDataBaseList()
        {
            return this.cremaHost.Dispatcher.Invoke(() =>
            {
                return this.cremaHost.DataBases.Select(o => o.Name).ToArray();
            });
        }

        [HttpGet]
        [Route("{databaseName}/info")]
        [AllowAnonymous]
        public DataBaseInfo GetDataBaseInfo(string databaseName, string tags = null)
        {
            var database = this.GetDataBase(databaseName);
            return database.Dispatcher.Invoke(() =>
            {
                var info = database.DataBaseInfo;
                if (string.IsNullOrWhiteSpace(tags))
                {
                    return info;
                }
                else
                {
                    info.Tags = (TagInfo) tags;
                    info.Paths = this.GetItems(database, (TagInfo) tags).ToArray();
                    info.TypesHashValue = this.GetTypesHashValue(database, (TagInfo) tags);
                    info.TablesHashValue = this.GetTablesHashValue(database, (TagInfo) tags);

                    return info;
                }
            });
        }

        [HttpGet]
        [Route("{databaseName}/log")]
        public LogInfo[] GetDataBaseLogInfo(string databaseName, string tags = null)
        {
            var database = this.GetDataBase(databaseName);
            return database.Dispatcher.Invoke(() => database.GetLog(null));
        }

        [HttpPost]
        [Route("{databaseName}/enter")]
        public void EnterDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Enter(this.Authentication));
        }

        [HttpPut]
        [Route("{databaseName}/leave")]
        public void LeaveDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Leave(this.Authentication));
        }

        [HttpPut]
        [Route("{databaseName}/load")]
        public LoadDataBaseResponse LoadDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            return database.Dispatcher.Invoke(() =>
            {
                if (database.IsLoaded)
                {
                    return new LoadDataBaseResponse { Status = LoadDataBaseStatus.AlreadyLoaded };
                }

                database.Load(this.Authentication);
                return new LoadDataBaseResponse
                {
                    Status = LoadDataBaseStatus.Loaded
                };
            });
        }

        [HttpPut]
        [Route("{databaseName}/unload")]
        public UnloadDataBaseResponse UnloadDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            return database.Dispatcher.Invoke(() =>
            {
                if (!database.IsLoaded)
                {
                    return new UnloadDataBaseResponse
                    {
                        Status = UnloadDataBaseStatus.AlreadyUnloaded
                    };
                }

                database.Unload(this.Authentication);
                return new UnloadDataBaseResponse
                {
                    Status = UnloadDataBaseStatus.Unloaded
                };
            });
        }

        [HttpGet]
        [Route("{databaseName}/entered")]
        public IsDataBaseEnteredResponse IsDataBaseEntered(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var isEntered = database.Dispatcher.Invoke(() => database.Contains(this.Authentication));

            return new IsDataBaseEnteredResponse
            {
                IsEntered = isEntered
            };
        }

        [HttpGet]
        [Route("{databaseName}/loaded")]
        [AllowAnonymous]
        public IsDataBaseLoadedResponse IsDataBaseLoaded(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var isLoaded = database.Dispatcher.Invoke(() => database.IsLoaded);

            return new IsDataBaseLoadedResponse
            {
                IsLoaded = isLoaded
            };
        }

        [HttpGet]
        [Route("{databaseName}/contains")]
        [AllowAnonymous]
        public ContainsDataBaseResponse ContainsDataBase(string databaseName)
        {
            var contains = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases.Contains(databaseName));

            return new ContainsDataBaseResponse
            {
                Contains = contains
            };
        }

        [HttpPost]
        [Route("{databaseName}/copy")]
        public void CopyDataBase(string databaseName, [FromBody] CopyDataBaseRequest request)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Copy(this.Authentication, request.NewDataBaseName, request.Comment, request.Force));
        }

        [HttpDelete]
        [Route("{databaseName}")]
        public void DeleteDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Delete(this.Authentication));
        }

        [HttpPost]
        [Route("{databaseName}/create")]
        public void CreateDataBase(string databaseName, [FromBody] CreateDataBaseRequest request)
        {
            this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases.AddNewDataBase(this.Authentication, databaseName, request.Comment));
        }

        [HttpPut]
        [Route("{databaseName}/rename")]
        public void RenameDataBase(string databaseName, [FromBody] RenameDataBaseRequest request)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Rename(this.Authentication, request.NewDataBaseName));
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

        private string GetTypesHashValue(IDataBase dataBase, TagInfo tags)
        {
            var typeInfoList = new List<TypeInfo>();
            foreach (var item in dataBase.TypeContext.Types.OrderBy(item => item.Name))
            {
                if ((item.TypeInfo.Tags & tags) != TagInfo.Unused)
                {
                    var typeInfo = item.TypeInfo;
                    typeInfoList.Add(typeInfo);
                }
            }
            return CremaDataSet.GenerateHashValue(typeInfoList.ToArray());
        }

        private string GetTablesHashValue(IDataBase dataBase, TagInfo tags)
        {
            var tableInfoList = new List<TableInfo>();
            foreach (var item in dataBase.TableContext.Tables.OrderBy(item => item.Name))
            {
                if ((item.TableInfo.Tags & tags) != TagInfo.Unused)
                {
                    var tableInfo = item.TableInfo.Filter(tags);
                    tableInfoList.Add(tableInfo);
                }
            }
            return CremaDataSet.GenerateHashValue(tableInfoList.ToArray());
        }

        private IEnumerable<string> GetItems(IDataBase dataBase, TagInfo tags)
        {
            yield return PathUtility.Separator;

            foreach (var item in dataBase.TypeContext)
            {
                if (item is IType type && (type.TypeInfo.Tags & tags) == TagInfo.Unused)
                {
                    continue;
                }
                yield return PathUtility.Separator + CremaSchema.TypeDirectory + item.Path;
            }

            foreach (var item in dataBase.TableContext)
            {
                if (item is ITable table && (table.TableInfo.DerivedTags & tags) == TagInfo.Unused)
                {
                    continue;
                }
                yield return PathUtility.Separator + CremaSchema.TableDirectory + item.Path;
            }
        }
    }
}
