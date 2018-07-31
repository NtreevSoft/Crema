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
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleCommand))]
    [ResourceDescription("Resources", IsShared = true)]
    class DomainCommand : ConsoleCommandMethodBase, IConsoleCommand
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public DomainCommand(ICremaHost cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(IsCancelled), nameof(IsForce))]
        public void Delete([CommandCompletion(nameof(GetDomainIDs))]Guid domainID)
        {
            var domain = this.GetDomain(domainID);
            var dataBase = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases.FirstOrDefault(item => item.ID == domain.DataBaseID));
            var isLoaded = dataBase.Dispatcher.Invoke(() => dataBase.IsLoaded);

            if (isLoaded == false && this.IsForce == false)
                throw new ArgumentException($"'{dataBase}' database is not loaded.");

            var authentication = this.CommandContext.GetAuthentication(this);
            domain.Dispatcher.Invoke(() => domain.Delete(authentication, this.IsCancelled));
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(IsCancelled), nameof(IsForce))]
        public void DeleteAll([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName)
        {
            var dataBase = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases[dataBaseName]);
            if (dataBase == null)
                throw new DataBaseNotFoundException(dataBaseName);

            var isLoaded = dataBase.Dispatcher.Invoke(() => dataBase.IsLoaded);
            if (isLoaded == false && this.IsForce == false)
                throw new ArgumentException($"'{dataBase}' database is not loaded.");

            var domains = this.DomainContext.Dispatcher.Invoke(() => this.DomainContext.Domains.Where(item => item.DataBaseID == dataBase.ID).ToArray());
            var authentication = this.CommandContext.GetAuthentication(this);

            foreach (var item in domains)
            {
                item.Dispatcher.Invoke(() => item.Delete(authentication, this.IsCancelled));
            }
        }

        [CommandMethod("list")]
        [CommandMethodProperty(nameof(DataBaseName))]
        public void List()
        {
            var domainInfos = this.DomainContext.Dispatcher.Invoke(() =>
            {
                var domainInfoList = new List<DomainInfo>();
                foreach (var item in this.DomainContext.Domains)
                {
                    var domainInfo = item.Dispatcher.Invoke(() => item.DomainInfo);
                    domainInfoList.Add(domainInfo);
                }
                return domainInfoList.ToArray();
            });

            var dataBaseInfos = this.cremaHost.Dispatcher.Invoke(() =>
            {
                return this.cremaHost.DataBases.Select(item => item.DataBaseInfo).ToArray();
            });

            var query = from domainInfo in domainInfos
                        join dataBaseInfo in dataBaseInfos on domainInfo.DataBaseID equals dataBaseInfo.ID
                        where this.DataBaseName == string.Empty || (dataBaseInfo.Name == this.DataBaseName)
                        group domainInfo.DomainID by dataBaseInfo.Name into g
                        select g;

            if (query.Any())
            {
                foreach (var item in query)
                {
                    this.CommandContext.WriteLine($"{item.Key}:");
                    this.CommandContext.WriteList(item.ToArray());
                    this.CommandContext.WriteLine();
                }
            }
            else
            {
                this.CommandContext.WriteLine("no domains");
            }
        }

        [CommandMethod]
        public void Info([CommandCompletion(nameof(GetDomainIDs))]Guid domainID)
        {
            var domain = this.GetDomain(domainID);
            var authentication = this.CommandContext.GetAuthentication(this);
            var metaData = domain.Dispatcher.Invoke(() => domain.GetMetaData(authentication));
            var domainInfo = metaData.DomainInfo;
            var dataBaseName = this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases[domainInfo.DataBaseID].Name);

            var items = new Dictionary<string, object>
            {
                { $"{nameof(domainInfo.DomainID)}", domainInfo.DomainID },
                { $"{nameof(domainInfo.DataBaseID)}", domainInfo.DataBaseID },
                { $"DataBaseName", dataBaseName },
                { $"{nameof(domainInfo.DomainType)}", domainInfo.DomainType },
                { $"{nameof(domainInfo.ItemType)}", domainInfo.ItemType},
                { $"{nameof(domainInfo.ItemPath)}", domainInfo.ItemPath },
                { $"{nameof(domainInfo.CreationInfo)}", domainInfo.CreationInfo.ToLocalValue() },
                { $"{nameof(domainInfo.ModificationInfo)}", domainInfo.ModificationInfo.ToLocalValue() },
                { $"UserList", string.Empty },
            };
            this.CommandContext.WriteLine();
            this.Out.Print<object>(items);

            var tableDataBuilder = new TableDataBuilder(nameof(DomainUserInfo.UserID), nameof(DomainUserInfo.UserName), nameof(DomainUserState), nameof(DomainUserInfo.AccessType));
            foreach (var item in metaData.Users)
            {
                tableDataBuilder.Add(item.DomainUserInfo.UserID, item.DomainUserInfo.UserName, item.DomainUserState, item.DomainUserInfo.AccessType);
            }
            this.Out.PrintTableData(tableDataBuilder.Data, true);

            this.CommandContext.WriteLine();
        }

        [CommandProperty("cancel", 'c')]
        public bool IsCancelled
        {
            get; set;
        }

        [CommandProperty("force", 'f')]
        public bool IsForce
        {
            get; set;
        }

        [CommandProperty("database")]
        [DefaultValue("")]
        [CommandCompletion(nameof(GetDataBaseNames))]
        public string DataBaseName
        {
            get; set;
        }

        public override bool IsEnabled => this.CommandContext.Drive is DomainsConsoleDrive;

        public IDomainContext DomainContext
        {
            get { return this.cremaHost.GetService(typeof(IDomainContext)) as IDomainContext; }
        }

        private string[] GetDataBaseNames()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var query = from item in this.CremaHost.DataBases
                            select item.Name;
                return query.ToArray();
            });
        }

        private string[] GetDomainIDs()
        {
            return this.DomainContext.Dispatcher.Invoke(() =>
            {
                var query = from item in this.DomainContext.Domains
                            let domainID = item.ID.ToString()
                            select domainID;
                return query.ToArray();
            });
        }

        private IDomain GetDomain(Guid domainID)
        {
            var domain = this.DomainContext.Dispatcher.Invoke(() => this.DomainContext.Domains[domainID]);
            if (domain == null)
                throw new DomainNotFoundException(domainID);
            return domain;
        }

        private ICremaHost CremaHost
        {
            get => this.cremaHost;
        }
    }
}
