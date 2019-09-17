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

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Controllers.Commands
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1/commands/databases/{databaseName}")]
    public class TypesApiController : CremaApiController
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public TypesApiController(ICremaHost cremaHost) : base(cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        [HttpGet]
        [Route("types")]
        public string[] GetTypeList(string databaseName, string tags = null)
        {
            var dataBase = this.GetDataBase(databaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrWhiteSpace(tags))
                {
                    return dataBase.TypeContext.Types.Select(item => item.Name).ToArray();
                }

                var types = dataBase.TypeContext.Types;
                var query = from item in types
                    where (item.TypeInfo.DerivedTags & (TagInfo)tags) != TagInfo.Unused
                    select item.Name;
                return query.ToArray();
            });
        }

        [HttpGet]
        [Route("types/{typeName}")]
        public IDictionary<int, object> GetTypeData(string databaseName, string typeName, long revision = -1)
        {
            var type = this.GetType(databaseName, typeName);

            return type.Dispatcher.Invoke(() =>
            {
                var dataSet = type.GetDataSet(this.Authentication, revision);
                var dataType = dataSet.Types[typeName];
                return this.GetTypeMembers(dataType);
            });
        }

        [HttpGet]
        [Route("types/*/info")]
        public GetTypeInfoResponse[] GetTypeInfo(string databaseName)
        {
            var types = this.GetTypes(databaseName);

            return types.Select(type => type.Dispatcher.Invoke(() =>
            {
                var typeInfo = type.TypeInfo;
                return GetTypeInfoResponse.ConvertFrom(typeInfo);
            })).ToArray();
        }

        [HttpPost]
        [Route("types/*/info")]
        public GetTypeInfoResponse[] GetTypeInfoByTypeName(string databaseName, [FromBody] GetTypeInfoByTypeNameRequest request)
        {
            var types = this.GetTypes(databaseName);
            var intersectTypes = types.Select(type => type.Dispatcher.Invoke(() => type.Name))
                .Intersect(request.TypeNames, StringComparer.OrdinalIgnoreCase);

            return intersectTypes.Select(typeName =>
            {
                var type = types.First(o => o.Dispatcher.Invoke(() => o.Name == typeName));
                var typeInfo = type.Dispatcher.Invoke(() => type.TypeInfo);
                return GetTypeInfoResponse.ConvertFrom(typeInfo);
            }).ToArray();
        }

        [HttpGet]
        [Route("types/{typeName}/info")]
        public GetTypeInfoResponse GetTypeInfo(string databaseName, string typeName)
        {
            var type = this.GetType(databaseName, typeName);

            return type.Dispatcher.Invoke(() =>
            {
                var typeInfo = type.TypeInfo;
                return GetTypeInfoResponse.ConvertFrom(typeInfo);
            });
        }

        [HttpPost]
        [Route("types/{typeName}/copy")]
        public void CopyType(string databaseName, string typeName, [FromBody] CopyTypeRequest request)
        {
            var type = this.GetType(databaseName, typeName);
            type.Dispatcher.Invoke(() => type.Copy(this.Authentication, request.NewTypeName, request.CategoryPath));
        }

        [HttpPut]
        [Route("types/{typeName}/rename")]
        public void RenameType(string databaseName, string typeName, [FromBody] RenameTypeRequest request)
        {
            var type = this.GetType(databaseName, typeName);
            type.Dispatcher.Invoke(() => type.Rename(this.Authentication, request.NewTypeName));
        }

        [HttpPut]
        [Route("types/{typeName}/move")]
        public void MoveType(string databaseName, string typeName, [FromBody] MoveTypeRequest request)
        {
            var type = this.GetType(databaseName, typeName);
            type.Dispatcher.Invoke(() => type.Move(this.Authentication, request.CategoryPath));
        }

        [HttpDelete]
        [Route("types/{typeName}")]
        public void DeleteType(string databaseName, string typeName)
        {
            var type = this.GetType(databaseName, typeName);
            type.Dispatcher.Invoke(() => type.Delete(this.Authentication));
        }

        [HttpGet]
        [Route("types/{typeName}/contains")]
        public ContainsTypeResponse ContainsType(string databaseName, string typeName)
        {
            var dataBase = this.GetDataBase(databaseName);
            var contains = dataBase.Dispatcher.Invoke(() => dataBase.TypeContext.Types.Contains(typeName));
            return new ContainsTypeResponse
            {
                Contains = contains
            };
        }

        [HttpGet]
        [Route("type-items")]
        public string[] GetTypeItemList(string databaseName, string tags = null)
        {
            var dataBase = this.GetDataBase(databaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                if (string.IsNullOrWhiteSpace(tags))
                {
                    return dataBase.TypeContext.Select(item => item.Path).ToArray();
                }

                var types = dataBase.TypeContext.Types;
                var query = from item in types
                    where (item.TypeInfo.DerivedTags & (TagInfo)tags) != TagInfo.Unused
                    select item.Name;
                return query.ToArray();
            });
        }

        [HttpPut]
        [Route("type-items/delete")]
        public void DeleteTypeItem(string databaseName, [FromBody] DeleteTypeItemRequest request)
        {
            var typeItem = this.GetTypeItem(databaseName, request.TypeItemPath);
            typeItem.Dispatcher.Invoke(() => typeItem.Delete(this.Authentication));
        }

        [HttpPut]
        [Route("type-items/move")]
        public void MoveTypeItem(string databaseName, [FromBody] MoveTypeItemRequest request)
        {
            var typeItem = this.GetTypeItem(databaseName, request.TypeItemPath);
            typeItem.Dispatcher.Invoke(() => typeItem.Move(this.Authentication, request.ParentPath));
        }

        [HttpPut]
        [Route("type-items/rename")]
        public void RenameTypeItem(string databaseName, [FromBody] RenameTypeItemRequest request)
        {
            var typeItem = this.GetTypeItem(databaseName, request.TypeItemPath);
            typeItem.Dispatcher.Invoke(() => typeItem.Rename(this.Authentication, request.NewName));
        }

        [HttpPost]
        [Route("type-items/contains")]
        public ContainsTypeItemResponse ContainsTypeItem(string databaseName, [FromBody] ContainsTypeItemRequest request)
        {
            var dataBase = this.GetDataBase(databaseName);
            var contains = dataBase.Dispatcher.Invoke(() => dataBase.TypeContext.Contains(request.TypeItemPath));
            return new ContainsTypeItemResponse
            {
                Contains = contains
            };
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

        private IType[] GetTypes(string dataBaseName)
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
                var types = dataBase.TypeContext.Types;
                return types.ToArray();
            }).ToArray();
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
                var type = dataBase.TypeContext.Types[typeName];
                if (type == null)
                    throw new TypeNotFoundException(typeName);
                return type;
            });
        }

        private ITypeItem GetTypeItem(string dataBaseName, string typeItemPath)
        {
            if (dataBaseName == null)
                throw new ArgumentNullException(nameof(dataBaseName));
            if (typeItemPath == null)
                throw new ArgumentNullException(nameof(typeItemPath));
            var dataBase = this.cremaHost.Dispatcher.Invoke(() =>
            {
                if (this.cremaHost.DataBases.Contains(dataBaseName) == false)
                    throw new DataBaseNotFoundException(dataBaseName);
                return this.cremaHost.DataBases[dataBaseName];
            });

            return dataBase.Dispatcher.Invoke(() =>
            {
                var typeItem = dataBase.TypeContext[typeItemPath];
                if (typeItem == null)
                    throw new ItemNotFoundException(typeItemPath);
                return typeItem;
            });
        }

        private IDictionary<int, object> GetTypeMembers(CremaDataType dataType)
        {
            var props = new Dictionary<int, object>();
            for (var i = 0; i < dataType.Members.Count; i++)
            {
                var typeMember = dataType.Members[i];
                props.Add(i, this.GetTypeMember(typeMember));
            }
            return props;
        }

        private IDictionary<string, object> GetTypeMember(CremaDataTypeMember typeMember)
        {
            var props = new Dictionary<string, object>
            {
                { nameof(typeMember.Name), typeMember.Name },
                { nameof(typeMember.Value), typeMember.Value },
                { nameof(typeMember.Comment), typeMember.Comment }
            };
            return props;
        }
    }
}
