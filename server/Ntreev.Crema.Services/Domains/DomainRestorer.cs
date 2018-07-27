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

using Ntreev.Crema.Services.Domains.Actions;
using Ntreev.Crema.Services.Domains.Serializations;
using Ntreev.Crema.Services.Users;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;

namespace Ntreev.Crema.Services.Domains
{
    class DomainRestorer : IDisposable
    {
        private readonly static XmlReaderSettings readerSettings = new XmlReaderSettings()
        {
            ConformanceLevel = ConformanceLevel.Fragment,
            IgnoreWhitespace = true,
        };

        private readonly Authentication authentication;
        private readonly DomainContext domainContext;
        private readonly string workingPath;

        private readonly List<DomainPostItemSerializationInfo> postedList = new List<DomainPostItemSerializationInfo>();
        private readonly Dictionary<long, DomainCompleteItemSerializationInfo> completedList = new Dictionary<long, DomainCompleteItemSerializationInfo>();
        private readonly List<DomainActionBase> actionList = new List<DomainActionBase>();
        private Dictionary<string, Authentication> authentications;
        private Domain domain;
        private DateTime dateTime;

        private long lastID;

        private DomainRestorer(Authentication authentication, DomainContext domainContext, string workingPath)
        {
            this.authentication = authentication;
            this.domainContext = domainContext;
            this.workingPath = workingPath;
        }

        public static void Restore(Authentication authentication, DomainContext domainContext, string workingPath)
        {
            using (var restorer = new DomainRestorer(authentication, domainContext, workingPath))
            {
                restorer.CollectCompletedActions();
                restorer.CollectPostedActions();
                restorer.DeserializeDomain();
                restorer.CollectAuthentications();
                restorer.RestoreDomain();
            }
        }

        private void CollectCompletedActions()
        {
            var itemPath = Path.Combine(this.workingPath, DomainLogger.CompletedItemPath);
            var items = File.ReadAllLines(itemPath);
            foreach (var item in items)
            {
                var info = DomainCompleteItemSerializationInfo.Parse(item);
                this.completedList.Add(info.ID, info);
            }
        }

        private void CollectPostedActions()
        {
            var itemPath = Path.Combine(this.workingPath, DomainLogger.PostedItemPath);
            var items = File.ReadAllLines(itemPath);
            foreach (var item in items)
            {
                var info = DomainPostItemSerializationInfo.Parse(item);
                if (this.completedList.ContainsKey(info.ID) == true)
                {
                    var type = Type.GetType(info.Type);
                    var path = Path.Combine(this.workingPath, $"{info.ID}");
                    var action = (DomainActionBase)this.Serializer.Deserialize(path, type, ObjectSerializerSettings.Empty);
                    this.actionList.Add(action);
                }
            }
        }

        private void DeserializeDomain()
        {
            var headerItemPath = Path.Combine(this.workingPath, DomainLogger.HeaderItemPath);
            var sourceItemPath = Path.Combine(this.workingPath, DomainLogger.SourceItemPath);
            var domainSerializationInfo = (DomainSerializationInfo)this.Serializer.Deserialize(headerItemPath, typeof(DomainSerializationInfo), ObjectSerializerSettings.Empty);
            var sourceType = Type.GetType(domainSerializationInfo.SourceType);
            var source = this.Serializer.Deserialize(sourceItemPath, sourceType, ObjectSerializerSettings.Empty);
            var domainType = Type.GetType(domainSerializationInfo.DomainType);
            this.domain = (Domain)Activator.CreateInstance(domainType, domainSerializationInfo, source);
            this.domain.Logger = new DomainLogger(this.domainContext.Serializer, this.workingPath);
            this.domainContext.Domains.Restore(this.authentication, this.domain);
        }

        private void CollectAuthentications()
        {
            var creatorID = this.domain.DomainInfo.CreationInfo.ID;
            var userContext = this.domainContext.CremaHost.UserContext;
            var userIDs = this.actionList.Select(item => item.UserID).Concat(Enumerable.Repeat(creatorID, 1)).Distinct();

            var users = userContext.Dispatcher.Invoke(() =>
            {
                var query = from userID in userIDs
                            join User user in userContext.Users on userID equals user.ID
                            select new Authentication(new UserAuthenticationProvider(user, true));
                return query.ToArray();
            });

            this.authentications = users.ToDictionary(item => item.ID);
        }

        private void RestoreDomain()
        {
            var dummyHost = new DummyDomainHost(this.domain);
            this.domain.Host = dummyHost;
            this.domain.Dispatcher.Invoke(() =>
            {
                this.domain.Logger.IsEnabled = false;

                foreach (var item in this.actionList)
                {
                    var authentication = this.authentications[item.UserID];
                    try
                    {
                        var action = item as DomainActionBase;
                        if (action == null)
                            throw new Exception();

                        this.domain.DateTimeProvider = this.GetTime;
                        this.dateTime = action.AcceptTime;

                        if (item is NewRowAction newRowAction)
                        {
                            this.domain.NewRow(authentication, newRowAction.Rows);
                        }
                        else if (item is RemoveRowAction removeRowAction)
                        {
                            this.domain.RemoveRow(authentication, removeRowAction.Rows);
                        }
                        else if (item is SetRowAction setRowAction)
                        {
                            this.domain.SetRow(authentication, setRowAction.Rows);
                        }
                        else if (item is SetPropertyAction setPropertyAction)
                        {
                            this.domain.SetProperty(authentication, setPropertyAction.PropertyName, setPropertyAction.Value);
                        }
                        else if (item is JoinAction joinAction)
                        {
                            this.domain.AddUser(authentication, joinAction.AccessType);
                        }
                        else if (item is DisjoinAction disjoinAction)
                        {
                            this.domain.RemoveUser(authentication);
                        }
                        else if (item is KickAction kickAction)
                        {
                            this.domain.Kick(authentication, kickAction.TargetID, kickAction.Comment);
                        }
                        else if (item is SetOwnerAction setOwnerAction)
                        {
                            this.domain.SetOwner(authentication, setOwnerAction.TargetID);
                        }
                        else
                        {
                            throw new NotImplementedException(item.GetType().Name);
                        }
                    }
                    finally
                    {
                        this.domain.DateTimeProvider = null;
                        // 데이터 베이스 Reset에 의해서 복구가 되었을때 클라이언트에 이벤트 전달 순서가 꼬이는 경우가 생김
                        Thread.Sleep(1);
                    }
                }

                this.domain.Logger.ID = this.postedList.Count;
                this.domain.Logger.IsEnabled = true;
            });
            this.domain.Host = null;
        }

        private DateTime GetTime()
        {
            return this.dateTime;
        }

        private IObjectSerializer Serializer => this.domainContext.Serializer;

        void IDisposable.Dispose()
        {
            if (this.authentications == null)
                return;
            foreach (var item in this.authentications)
            {
                item.Value.InvokeExpiredEvent(Authentication.SystemID);
            }
        }
    }
}
