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

namespace Ntreev.Crema.Services.Data
{
    public abstract class DataServiceBase<T> : IPlugin where T : DataServiceItemBase
    {
        private readonly string name;
        private Dictionary<IDataBase, T> items = new Dictionary<IDataBase, T>();

        protected DataServiceBase(ICremaHost cremaHost, Guid pluginID, string name)
        {
            this.CremaHost = cremaHost;
            this.ID = pluginID;
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.Dispatcher = new CremaDispatcher(this);
            this.CremaHost.Closing += CremaHost_Closing;
        }

        public void Initialize(Authentication authentication)
        {
            this.Authentication = authentication;
            foreach (var item in this.CremaHost.DataBases)
            {
                var serviceItem = this.CreateItem(item, this.Dispatcher, authentication);
                this.items.Add(item, serviceItem);
            }
            this.CremaHost.DataBases.ItemsCreated += DataBases_ItemCreated;
            this.CremaHost.DataBases.ItemsDeleted += DataBases_ItemsDeleted;
        }

        public void Release()
        {

        }

        public virtual string Name => this.name;

        public Guid ID { get; }

        public CremaDispatcher Dispatcher { get; }

        public ICremaHost CremaHost { get; }

        protected Authentication Authentication { get; private set; }

        protected abstract T CreateItem(IDataBase dataBase, CremaDispatcher Dispatcher, Authentication authentication);

        private void CremaHost_Closing(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() => this.Dispatcher.Dispose());
        }

        private void DataBases_ItemCreated(object sender, ItemsCreatedEventArgs<IDataBase> e)
        {
            foreach (var item in e.Items)
            {
                var serviceItem = this.CreateItem(item, this.Dispatcher, Authentication);
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
