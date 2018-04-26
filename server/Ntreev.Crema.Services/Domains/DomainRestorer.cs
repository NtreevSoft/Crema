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
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Ntreev.Crema.Services;
using System.ComponentModel;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Domains.Actions;
using Ntreev.Crema.Services.Users;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using Ntreev.Library;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Ntreev.Crema.Services.Domains
{
    class DomainRestorer
    {
        private readonly static XmlReaderSettings readerSettings = new XmlReaderSettings()
        {
            ConformanceLevel = ConformanceLevel.Fragment,
            IgnoreWhitespace = true,
        };

        private readonly Authentication authentication;
        private readonly DomainContext domainContext;
        private readonly string workingPath;

        private readonly HashSet<long> completedActions = new HashSet<long>();
        private readonly List<DomainActionBase> postedActions = new List<DomainActionBase>();
        private Dictionary<string, Authentication> authentications;
        private Domain domain;
        private DateTime dateTime;

        private long lastID;

        public DomainRestorer(Authentication authentication, DomainContext domainContext, string workingPath)
        {
            this.authentication = authentication;
            this.domainContext = domainContext;
            this.workingPath = workingPath;
        }

        public void Restore()
        {
            try
            {
                this.CollectCompletedActions();
                this.CollectPostedActions();
                this.DeserializeDomain();
                this.CollectAuthentications();
                this.RestoreDomain();
            }
            finally
            {
                this.ExpireAuthetications();
            }
        }

        private void CollectCompletedActions()
        {
            var completedPath = Path.Combine(this.workingPath, DomainLogger.CompletedFileName);

            using (var reader = XmlReader.Create(completedPath, readerSettings))
            {
                while (reader.Read() == true)
                {
                    var id = long.Parse(reader.GetAttribute("ID"));
                    this.completedActions.Add(id);
                }
            }
        }

        private void CollectPostedActions()
        {
            var postedPath = Path.Combine(this.workingPath, DomainLogger.PostedFileName);

            using (var reader = XmlReader.Create(postedPath, readerSettings))
            {
                reader.Read();
                while (reader.EOF != true)
                {
                    DomainActionBase actionObject = null;
                    if (reader.Name == typeof(NewRowAction).Name)
                    {
                        actionObject = DataContractSerializerUtility.Read<NewRowAction>(reader);
                    }
                    else if (reader.Name == typeof(RemoveRowAction).Name)
                    {
                        actionObject = DataContractSerializerUtility.Read<RemoveRowAction>(reader);
                    }
                    else if (reader.Name == typeof(SetRowAction).Name)
                    {
                        actionObject = DataContractSerializerUtility.Read<SetRowAction>(reader);
                    }
                    else if (reader.Name == typeof(SetPropertyAction).Name)
                    {
                        actionObject = DataContractSerializerUtility.Read<SetPropertyAction>(reader);
                    }
                    else if (reader.Name == typeof(JoinAction).Name)
                    {
                        actionObject = DataContractSerializerUtility.Read<JoinAction>(reader);
                    }
                    else if (reader.Name == typeof(DisjoinAction).Name)
                    {
                        actionObject = DataContractSerializerUtility.Read<DisjoinAction>(reader);
                    }
                    else if (reader.Name == typeof(KickAction).Name)
                    {
                        actionObject = DataContractSerializerUtility.Read<KickAction>(reader);
                    }
                    else if (reader.Name == typeof(SetOwnerAction).Name)
                    {
                        actionObject = DataContractSerializerUtility.Read<SetOwnerAction>(reader);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    this.lastID = actionObject.ID;

                    if (this.completedActions.Contains(actionObject.ID) == false)
                        continue;

                    this.postedActions.Add(actionObject);
                }
            }
        }

        private void DeserializeDomain()
        {
            var path = Path.Combine(this.workingPath, DomainLogger.HeaderFileName);
            var formatter = new BinaryFormatter() { Context = new StreamingContext(StreamingContextStates.CrossAppDomain, this.domainContext.CremaHost) };
            using (var stream = File.OpenRead(path))
            {
                this.domain = formatter.Deserialize(stream) as Domain;
                this.domain.Logger = new DomainLogger(this.workingPath);
                this.domainContext.Domains.Restore(this.authentication, this.domain);
            }
        }

        private void CollectAuthentications()
        {
            var creatorID = this.domain.DomainInfo.CreationInfo.ID;
            var userContext = this.domainContext.CremaHost.UserContext;
            var userIDs = this.postedActions.Select(item => item.UserID).Concat(Enumerable.Repeat(creatorID, 1)).Distinct();

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

                foreach (var item in this.postedActions)
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
                            dummyHost.AccessType = joinAction.AccessType;
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

                this.domain.Logger.ID = this.lastID + 1;
                this.domain.Logger.IsEnabled = true;
            });
            this.domain.Host = null;
        }

        private void ExpireAuthetications()
        {
            if (this.authentications == null)
                return;
            foreach (var item in this.authentications)
            {
                item.Value.InvokeExpiredEvent(Authentication.SystemID);
            }
        }

        private DateTime GetTime()
        {
            return this.dateTime;
        }
    }
}
