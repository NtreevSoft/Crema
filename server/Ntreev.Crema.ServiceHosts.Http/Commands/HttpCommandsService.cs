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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceHosts.Http.Extensions;
using Ntreev.Crema.ServiceHosts.Http.Requests.Commands;
using Ntreev.Crema.ServiceHosts.Http.Responses.Commands;
using Ntreev.Crema.ServiceHosts.Http.Users;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ServiceHosts.Http.Commands
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class HttpCommandsService : CremaServiceItemBase, IHttpCommandsService, ICremaServiceItem
    {
        private readonly ICremaHost cremaHost;
        private readonly CremaService cremaService;
        private readonly ILogService logService;
        private readonly IUserContext userContext;

        public HttpCommandsService(ICremaHost cremaHost, CremaService cremaService)
            : base(cremaHost.GetService(typeof(ILogService)) as ILogService)
        {
            this.cremaHost = cremaHost;
            this.cremaService = cremaService;
            this.logService = cremaHost.GetService(typeof(ILogService)) as ILogService;
            this.userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
            this.logService.Debug($"{nameof(HttpCommandsService)} Constructor");
        }

        protected override void OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
        }

        public void Abort(bool disconnect)
        {
            CremaService.Dispatcher.Invoke(() =>
            {
                if (disconnect == false)
                {
                    try
                    {
                        this.Channel?.Close(TimeSpan.FromSeconds(10));
                    }
                    catch
                    {
                        this.Channel?.Abort();
                    }
                }
                else
                {
                    this.Channel?.Abort();
                }
            });
        }

        public Message Login(string userId, string password)
        {
            var authentication = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Login(userId, password.ToSecureString()));
            return new LoginResponse
            {
                Token = authentication.Token.ToString()
            }.ToJsonMessage();
        }

        public void Logout()
        {
            var authentication = this.GetAuthentication();
            this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.Logout(authentication));
        }

        public Message GetDataBaseList()
        {
            return this.cremaHost.Dispatcher.Invoke(() =>
                    {
                        return this.cremaHost.DataBases.Select(o => o.Name).ToArray();
                    })
                .ToJsonMessage();
        }

        public Message GetDataBaseInfo(string databaseName, string tags)
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
                    info.Tags = (TagInfo)tags;
                    info.Paths = this.GetItems(database, (TagInfo) tags).ToArray();
                    info.TypesHashValue = this.GetTypesHashValue(database, (TagInfo) tags);
                    info.TablesHashValue = this.GetTablesHashValue(database, (TagInfo) tags);

                    return info;
                }
            }).ToJsonMessage();
        }

        public Message GetDataBaseLogInfo(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var authentication = this.GetAuthentication();
            return database.Dispatcher.Invoke(() => database.GetLog(authentication)).ToJsonMessage();
        }

        public void EnterDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var authentication = this.GetAuthentication();
            database.Dispatcher.Invoke(() => database.Enter(authentication));
        }

        public void LeaveDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var authentication = this.GetAuthentication();
            database.Dispatcher.Invoke(() => database.Leave(authentication));
        }

        public void LoadDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var authentication = this.GetAuthentication();
            database.Dispatcher.Invoke(() => database.Load(authentication));
        }

        public void UnloadDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var authentication = this.GetAuthentication();
            database.Dispatcher.Invoke(() => database.Unload(authentication));
        }

        public Message IsDataBaseEntered(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var authentication = this.GetAuthentication();
            var isEntered = database.Dispatcher.Invoke(() => database.Contains(authentication));

            return new IsDataBaseEnteredResponse()
            {
                IsEntered = isEntered
            }.ToJsonMessage();
        }

        public Message IsDataBaseLoaded(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var isLoaded = database.Dispatcher.Invoke(() => database.IsLoaded);

            return new IsDataBaseLoadedResponse
            {
                IsLoaded = isLoaded
            }.ToJsonMessage();
        }

        public Message ContainsDataBase(string databaseName)
        {
            var contains = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases.Contains(databaseName));
            
            return new ContainsDataBaseResponse
            {
                Contains = contains
            }.ToJsonMessage();
        }

        public void CopyDataBase(string databaseName, CopyDataBaseRequest request)
        {
            var database = this.GetDataBase(databaseName);
            var authentication = this.GetAuthentication();
            database.Dispatcher.Invoke(() =>
                database.Copy(authentication, request.NewDataBaseName, request.Comment, request.Force));
        }

        public void DeleteDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            var authentication = this.GetAuthentication();
            database.Dispatcher.Invoke(() => database.Delete(authentication));
        }

        public void CreateDataBase(string databaseName, CreateDataBaseRequest request)
        {
            var authentication = this.GetAuthentication();
            this.cremaHost.Dispatcher.Invoke(() =>
                this.cremaHost.DataBases.AddNewDataBase(authentication, databaseName, request.Comment));
        }

        public void RenameDataBase(string databaseName, RenameDataBaseRequest request)
        {
            var database = this.GetDataBase(databaseName);
            var authentication = this.GetAuthentication();
            database.Dispatcher.Invoke(() => database.Rename(authentication, request.NewDataBaseName));
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

        private Authentication GetAuthentication()
        {
            var token = WebOperationContext.Current?.IncomingRequest.Headers["token"];
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("'token' is not include in http header.");
            }
            return this.cremaHost.Dispatcher.Invoke(() =>
            {
                return this.userContext.Dispatcher.Invoke(() => this.userContext.Authenticate(Guid.Parse(token)));
            });
        }
    }
}
