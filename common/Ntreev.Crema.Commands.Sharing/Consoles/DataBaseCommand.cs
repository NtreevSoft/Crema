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
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        [ImportingConstructor]
        public DataBaseCommand()
            : base("database")
        {

        }

        public override string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor, string find)
        {
            switch (methodDescriptor.DescriptorName)
            {
                case nameof(Load):
                case nameof(Unload):
                case nameof(Rename):
                case nameof(Delete):
                case nameof(Copy):
                case nameof(Info):
                case nameof(Log):
                    {
                        if (memberDescriptor.DescriptorName == dataBaseNameParameter)
                        {
                            return this.GetDataBaseNames();
                        }
                    }
                    break;
            }

            return base.GetCompletions(methodDescriptor, memberDescriptor, find);
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Comment))]
        public void Create(string dataBaseName)
        {
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                var authentication = this.CommandContext.GetAuthentication(this);
                this.CremaHost.DataBases.AddNewDataBase(authentication, dataBaseName, this.Comment);
            });
        }

        [CommandMethod]
        public void Rename(string dataBaseName, string newDataBaseName)
        {
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.GetDataBase(dataBaseName);
                var authentication = this.CommandContext.GetAuthentication(this);
                dataBase.Rename(authentication, newDataBaseName);
            });
        }

        [CommandMethod]
        public void Delete(string dataBaseName)
        {
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.GetDataBase(dataBaseName);
                var authentication = this.CommandContext.GetAuthentication(this);
                if (this.CommandContext.ConfirmToDelete() == true)
                    dataBase.Delete(authentication);
            });
        }

        [CommandMethod]
        [CommandMethodProperty(nameof(Comment), nameof(CopyForce))]
        public void Copy(string dataBaseName, string newDataBaseName)
        {
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) => this.Out.Write(".");
            timer.Start();
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                try
                {
                    var dataBase = GetDataBase(dataBaseName);
                    var authentication = this.CommandContext.GetAuthentication(this);
                    dataBase.Copy(authentication, newDataBaseName, this.Comment, this.CopyForce);
                }
                finally
                {
                    timer.Stop();
                }
            });
        }

        [CommandMethod]
        public void Load(string dataBaseName)
        {
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.GetDataBase(dataBaseName);
                var authentication = this.CommandContext.GetAuthentication(this);
                dataBase.Load(authentication);
            });
        }

        [CommandMethod]
        public void Unload(string dataBaseName)
        {
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.GetDataBase(dataBaseName);
                var authentication = this.CommandContext.GetAuthentication(this);
                dataBase.Unload(authentication);
            });
        }

        [CommandMethod]
        public void Lock(string dataBaseName, string comment)
        {
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.GetDataBase(dataBaseName);
                var authentication = this.CommandContext.GetAuthentication(this);
                dataBase.Lock(authentication, comment);
            });
        }

        [CommandMethod]
        public void Unlock(string dataBaseName)
        {
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                var dataBase = this.GetDataBase(dataBaseName);
                var authentication = this.CommandContext.GetAuthentication(this);
                dataBase.Unlock(authentication);
            });
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(FilterProperties))]
        public void List()
        {
            var items = this.CremaHost.Dispatcher.Invoke(() =>
            {
                var query = from item in this.CremaHost.DataBases
                            where StringUtility.GlobMany(item.Name, FilterProperties.FilterExpression)
                            select new ItemObject(item.Name, item.IsLoaded);
                return query.ToArray();
            });

            this.Out.Print(items, (o, a) => o.Print(a));
        }

        [CommandMethod]
        public void Info(string dataBaseName)
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
        public void Revert(string dataBaseName, string revision)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            this.CremaHost.Dispatcher.Invoke(() =>
            {
                var authentication = this.CommandContext.GetAuthentication(this);
                dataBase.Revert(authentication, revision);
            });
        }

        [CommandMethod]
        [CommandMethodStaticProperty(typeof(LogProperties))]
        public void Log(string dataBaseName)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            var authentication = this.CommandContext.GetAuthentication(this);
            var logs = dataBase.Dispatcher.Invoke(() => dataBase.GetLog(authentication));

            LogProperties.Print(this.Out, logs);
        }

        //[CommandMethod]
        //public void Export(string dataBaseName)
        //{
        //    var dataBase = this.GetDataBase(dataBaseName);

        //    dataBase.Dispatcher.Invoke(() =>
        //    {
        //        if (dataBase.IsLoaded == false)
        //            dataBase.Load(authentication);
        //        dataBase.Enter(authentication);
        //    });

        //    using (DataBaseUsing.Set(dataBase, authentication))
        //    {
        //        var logs = dataBase.Dispatcher.Invoke(() => dataBase.GetLog(authentication));

        //        var path = @"C:\Users\s2quake\Desktop\새 폴더 (3)";
        //        foreach (var item in logs)
        //        {
        //            var dataSet = dataBase.Dispatcher.Invoke(() => dataBase.Preview(authentication, item.Revision));
        //            var dir = Path.Combine(path, $"{item.Revision}");
        //            DirectoryUtility.Prepare(dir);
        //            dataSet.WriteToDirectory(dir);
        //        }
        //    }
        //}

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

        public ICremaHost CremaHost
        {
            get { return this.cremaHost.Value; }
        }

        public override bool IsEnabled => this.CommandContext.Drive is DataBasesConsoleDrive;

        private string[] GetDataBaseNames()
        {
            return this.CremaHost.Dispatcher.Invoke(() =>
            {
                var query = from item in this.CremaHost.DataBases
                            select item.Name;
                return query.ToArray();
            });
        }

        private IDataBase GetDataBase(string dataBaseName)
        {
            var dataBase = this.CremaHost.Dispatcher.Invoke(GetDataBase);
            if (dataBase == null)
                throw new DataBaseNotFoundException(dataBaseName);
            return dataBase;

            IDataBase GetDataBase()
            {
                return this.CremaHost.DataBases[dataBaseName];
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
