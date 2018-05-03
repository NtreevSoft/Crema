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

using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Jint.Native;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Javascript.Methods
{
    class CremaDataBaseEventListenerContext
    {
        private readonly ICremaHost cremaHost;
        private readonly IDictionary<CremaDataBaseEvents, CremaDataBaseEventListenerBase> eventListeners;
        private readonly Dictionary<CremaDataBaseEvents, List<CremaDataBaseEventListener>> listeners = new Dictionary<CremaDataBaseEvents, List<CremaDataBaseEventListener>>();
        private readonly CremaDispatcher dispatcher;

        public CremaDataBaseEventListenerContext(ICremaHost cremaHost, CremaDataBaseEventListenerBase[] eventListener)
        {
            this.cremaHost = cremaHost;

            this.eventListeners = eventListener.ToDictionary(item => item.EventName);
            this.dispatcher = new CremaDispatcher(this);
            foreach (var item in eventListener)
            {
                item.Dispatcher = this.dispatcher;
            }
            this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases.ItemsLoaded += DataBases_ItemsLoaded);
            this.cremaHost.Dispatcher.Invoke(() => this.cremaHost.DataBases.ItemsUnloaded += DataBases_ItemsUnloaded);
        }

        public void Dispose()
        {
            this.dispatcher.Dispose();
            foreach (var item in this.eventListeners)
            {
                item.Value.Dispose();
            }
        }

        public void AddEventListener(CremaDataBaseEvents eventName, CremaDataBaseEventListener listener)
        {
            if (this.eventListeners.ContainsKey(eventName) == true)
            {
                this.cremaHost.Dispatcher.Invoke(() =>
                {
                    var dataBases = this.cremaHost.DataBases.Where(item => item.IsLoaded).ToArray();
                    var eventHandler = this.eventListeners[eventName];
                    foreach (var item in dataBases)
                    {
                        eventHandler.Subscribe(item, listener);

                    }
                });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void RemoveEventListener(CremaDataBaseEvents eventName, CremaDataBaseEventListener listener)
        {
            if (this.eventListeners.ContainsKey(eventName) == true)
            {
                this.cremaHost.Dispatcher.Invoke(() =>
                {
                    var dataBases = this.cremaHost.DataBases.Where(item => item.IsLoaded).ToArray();
                    var eventHandler = this.eventListeners[eventName];
                    foreach (var item in dataBases)
                    {
                        eventHandler.Unsubscribe(item, listener);
                    }
                });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void DataBases_ItemsLoaded(object sender, ItemsEventArgs<IDataBase> e)
        {

        }

        private void DataBases_ItemsUnloaded(object sender, ItemsEventArgs<IDataBase> e)
        {
            //foreach (var item in this.eventListeners)
            //{
            //    foreach (var dataBase in e.Items)
            //    {
            //        item.Value.Subscribe(dataBase, item.Value);
            //    }
            //}
        }

        #region classes

        class ListenerItem
        {
            public CremaDataBaseEvents EventName { get; set; }

            public CremaDataBaseEventListener Listener { get; set; }
        }

        #endregion
    }
}
