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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    public abstract class DataServiceBase<T> : IPlugin where T : DataServiceItemBase
    {
        private readonly ICremaHost cremaHost;
        private readonly Guid pluginID;
        private readonly string name;
        private readonly CremaDispatcher dispatcher;
        private Authentication authentication;

        private Dictionary<IDataBase, T> items = new Dictionary<IDataBase, T>();

        protected DataServiceBase(ICremaHost cremaHost, Guid pluginID, string name)
        {
            this.cremaHost = cremaHost;
            this.pluginID = pluginID;
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.dispatcher = new CremaDispatcher(this);
            this.cremaHost.Closing += CremaHost_Closing;
        }

        public void Initialize(Authentication authentication)
        {
            this.authentication = authentication;
            foreach (var item in this.cremaHost.DataBases)
            {
                var serviceItem = this.CreateItem(item, this.dispatcher, authentication);
                this.items.Add(item, serviceItem);
            }
            this.cremaHost.DataBases.ItemsCreated += DataBases_ItemCreated;
            this.cremaHost.DataBases.ItemsDeleted += DataBases_ItemsDeleted;
        }

        public void Release()
        {

        }

        public virtual string Name
        {
            get { return this.name; }
        }

        public Guid ID
        {
            get { return this.pluginID; }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        public ICremaHost CremaHost
        {
            get { return this.cremaHost; }
        }

        protected Authentication Authentication
        {
            get { return this.authentication; }
        }

        protected abstract T CreateItem(IDataBase dataBase, CremaDispatcher Dispatcher, Authentication authentication);

        private void CremaHost_Closing(object sender, EventArgs e)
        {
            this.dispatcher.Invoke(() => this.dispatcher.Dispose());
        }

        private void DataBases_ItemCreated(object sender, ItemsCreatedEventArgs<IDataBase> e)
        {
            foreach (var item in e.Items)
            {
                var serviceItem = this.CreateItem(item, this.dispatcher, authentication);
                this.items.Add(item, serviceItem);
            }
        }

        private void DataBases_ItemsDeleted(object sender, ItemsDeletedEventArgs<IDataBase> e)
        {
            foreach (var item in e.Items)
            {
                var obj = this.items[item];
                obj.Dispose();
                this.items.Remove(item);
            }
        }
    }
}
