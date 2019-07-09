using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceHosts.Http.Extensions;
using Ntreev.Crema.ServiceHosts.Http.Requests.Commands;
using Ntreev.Crema.ServiceHosts.Http.Responses.Commands;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ServiceHosts.Http.Controllers.Apis.V1.Commands
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1/commands/databases")]
    public class DataBaseApiController : CremaApiController
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DataBaseApiController(ICremaHost cremaHost) : base(cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        [HttpGet]
        [Route("list")]
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

        [HttpGet]
        [Route("{databaseName}/enter")]
        public void EnterDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Enter(this.Authentication));
        }

        [HttpGet]
        [Route("{databaseName}/leave")]
        public void LeaveDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Leave(this.Authentication));
        }

        [HttpGet]
        [Route("{databaseName}/load")]
        public void LoadDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Load(this.Authentication));
        }

        [HttpGet]
        [Route("{databaseName}/unload")]
        public void UnloadDataBase(string databaseName)
        {
            var database = this.GetDataBase(databaseName);
            database.Dispatcher.Invoke(() => database.Unload(this.Authentication));
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

        [HttpGet]
        [Route("{databaseName}/delete")]
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

        [HttpPost]
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
