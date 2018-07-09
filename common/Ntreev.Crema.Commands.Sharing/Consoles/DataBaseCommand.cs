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

using Ntreev.Crema.Commands.Consoles.Properties;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Commands;
using Ntreev.Library.IO;
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
    class DataBaseCommand : ConsoleCommandMethodBase, IConsoleCommand
    {
        private const string dataBaseNameParameter = "dataBaseName";

        private readonly ICremaHost cremaHost;
        private IDataBaseCollection dataBases;

        [ImportingConstructor]
        public DataBaseCommand(ICremaHost cremaHost)
            : base("database")
        {
            this.cremaHost = cremaHost;
            this.cremaHost.Opened += (s, e) => this.dataBases = this.cremaHost.DataBases;
            this.cremaHost.Closed += (s, e) => this.dataBases = null;
        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            return base.GetCompletions(methodDescriptor, memberDescriptor, find);
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Comment))]
        public void Create(string dataBaseName)
        {
            var authentication = this.CommandContext.GetAuthentication(this);
            this.dataBases.Dispatcher.Invoke(() =>
            {
                this.dataBases.AddNewDataBase(authentication, dataBaseName, this.Comment);
            });
        }

        [CommandMethod]
        public void Rename([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName, string newDataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            dataBase.Dispatcher.Invoke(() =>
            {
                dataBase.Rename(authentication, newDataBaseName);
            });
        }

        [CommandMethod]
        public void Delete([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            if (this.CommandContext.ConfirmToDelete() == true)
            {
                dataBase.Dispatcher.Invoke(() => dataBase.Delete(authentication));
            }
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Comment), nameof(CopyForce))]
        public void Copy([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName, string newDataBaseName)
        {
            var dataBase = GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            dataBase.Dispatcher.Invoke(() => dataBase.Copy(authentication, newDataBaseName, this.Comment, this.CopyForce));
        }

        [CommandMethod]
        public void Load([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            dataBase.Dispatcher.Invoke(() => dataBase.Load(authentication));
        }

        [CommandMethod]
        public void Unload([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            dataBase.Dispatcher.Invoke(() => dataBase.Unload(authentication));
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Comment))]
        public void Lock([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            dataBase.Dispatcher.Invoke(() => dataBase.Lock(authentication, this.Comment));
        }

        [CommandMethod]
        public void Unlock([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            dataBase.Dispatcher.Invoke(() => dataBase.Unlock(authentication));
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        public void List()
        {
            var items = this.dataBases.Dispatcher.Invoke(() =>
            {
                var query = from item in this.dataBases
                            where StringUtility.GlobMany(item.Name, FilterProperties.FilterExpression)
                            select new ItemObject(item.Name, item.IsLoaded);
                return query.ToArray();
            });

            this.Out.Print(items, (o, a) => o.Print(a));
        }

        [CommandMethod]
        public void Info([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var dataBaseInfo = dataBase.Dispatcher.Invoke(() => dataBase.DataBaseInfo);

            var items = new Dictionary<string, object>
            {
                { $"{nameof(dataBaseInfo.ID)}", dataBaseInfo.ID },
                { $"{nameof(dataBaseInfo.Name)}", dataBaseInfo.Name },
                { $"{nameof(dataBaseInfo.Comment)}", dataBaseInfo.Comment },
                { $"{nameof(dataBaseInfo.Revision)}", dataBaseInfo.Revision },
                { $"{nameof(dataBaseInfo.CreationInfo)}", dataBaseInfo.CreationInfo.ToLocalValue() },
                { $"{nameof(dataBaseInfo.ModificationInfo)}", dataBaseInfo.ModificationInfo.ToLocalValue() }
            };

            this.Out.Print<object>(items);
        }

        [CommandMethod]
        public void Revert([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName, string revision)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            dataBase.Dispatcher.Invoke(() => dataBase.Revert(authentication, revision));
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(LogProperties))]
        public void Log([CommandCompletion(nameof(GetDataBaseNames))]string dataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var logs = dataBase.Dispatcher.Invoke(() => dataBase.GetLog(authentication));

            LogProperties.Print(this.Out, logs);
        }

        [CommandProperty('f', true)]
        public bool CopyForce
        {
            get; set;
        }

        [CommandProperty("display")]
        [DefaultValue("")]
        public string[] DisplayArguments
        {
            get; set;
        }

        [CommandProperty('m', IsRequired = true)]
        public string Comment
        {
            get; set;
        }

        public override bool IsEnabled => this.CommandContext.Drive is DataBasesConsoleDrive;

        private string[] GetDataBaseNames()
        {
            return this.dataBases.Dispatcher.Invoke(() =>
            {
                var query = from item in this.dataBases
                            select item.Name;
                return query.ToArray();
            });
        }

        private IDataBase GetDataBase(string dataBaseName)
        {
            var dataBase = this.dataBases.Dispatcher.Invoke(GetDataBase);
            if (dataBase == null)
                throw new DataBaseNotFoundException(dataBaseName);
            return dataBase;

            IDataBase GetDataBase()
            {
                return this.dataBases[dataBaseName];
            }
        }

        #region classes

        class ItemObject
        {
            private bool isLoaded;
            private string name;

            public ItemObject(string name, bool isLoaded)
            {
                this.name = name;
                this.isLoaded = isLoaded;
            }

            public bool IsLoaded => this.isLoaded;

            public void Print(Action action)
            {
                if (this.isLoaded == false)
                {
                    using (TerminalColor.SetForeground(ConsoleColor.DarkGray))
                    {
                        action();
                    }
                }
                else
                {
                    action();
                }
            }

            public override string ToString()
            {
                return this.name;
            }
        }

        #endregion
    }
}
