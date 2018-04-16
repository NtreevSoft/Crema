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

using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Ntreev.Crema.Client.Base.Services.ViewModels
{
    [Obsolete]
    class ServerItemCollection : ObservableCollection<ConnectionItemViewModel>
    {
        private const string versionString = "3.0";
        private static readonly XmlSerializer serializer;

        private string filename;

        static ServerItemCollection()
        {
            serializer = XmlSerializer.FromTypes(new Type[] { typeof(ServerInfo[]), }).First();
        }

        private ServerItemCollection(string filename)
            : this(filename, Enumerable.Empty<ServerInfo>())
        {

        }

        private ServerItemCollection(string filename, IEnumerable<ServerInfo> items)
        {
            this.filename = filename;
            foreach (var item in items)
            {
                var version = item.Version;
                var itemViewModel = new ConnectionItemViewModel()
                {
                    Name = item.Name,
                    Address = item.Address,
                    DataBaseName = item.DataBase,
                    ID = item.ID,
                    ThemeColor = item.ThemeColor,
                    Theme = item.Theme ?? "Dark",
                };

                if (item.Password != null)
                    itemViewModel.Password = version == null ? StringUtility.Encrypt(item.Password, item.ID) : item.Password;

                base.Add(itemViewModel);
            }
            this.CollectionChanged += ServerInfoCollection_CollectionChanged;
        }

        public new void Add(ConnectionItemViewModel viewModel)
        {
            this.RemoveEquale(viewModel);
            this.Insert(0, viewModel);
            //viewModel.Owner = this;
        }

        public void RemoveEquale(ConnectionItemViewModel serverInfo)
        {
            for (var i = this.Count - 1; i >= 0; i--)
            {
                var recentItem = this[i];
                if (recentItem.Equals(serverInfo) == true)
                {
                    this.RemoveAt(i);
                }
            }
        }

        public void Write()
        {
            DirectoryUtility.PrepareFile(this.filename);

            var serverInfoList = new List<ServerInfo>(this.Count);

            foreach (var item in this)
            {
                var serverInfo = new ServerInfo()
                {
                    Version = versionString,
                    Name = item.Name,
                    Address = item.Address,
                    DataBase = item.DataBaseName,
                    ID = item.ID,
                    Password = item.Password,
                    ThemeColor = item.ThemeColor,
                    Theme = item.Theme,
                };
                serverInfoList.Add(serverInfo);
            }

            FileUtility.Backup(this.filename);
            using (var stream = new FileStream(this.filename, FileMode.Create))
            {
                serializer.Serialize(stream, serverInfoList.ToArray());
            }
        }

        public static ServerItemCollection Read(string filename)
        {
            try
            {
                using (var stream = new FileStream(filename, FileMode.Open))
                {
                    var items = serializer.Deserialize(stream) as ServerInfo[];
                    var collection = new ServerItemCollection(filename, items);
                    return collection;
                }
            }
            catch
            {
                return new ServerItemCollection(filename);
            }
        }

        private void ServerInfoCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Write();
        }
    }
}
