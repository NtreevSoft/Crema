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
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Javascript.Methods
{
    abstract class DataBaseEventListenerHost
    {
        private readonly DataBaseEvents eventName;
        private readonly Dictionary<IDataBase, DataBaseEventListenerCollection> dataBaseToListeners = new Dictionary<IDataBase, DataBaseEventListenerCollection>();

        protected DataBaseEventListenerHost(DataBaseEvents eventName)
        {
            this.eventName = eventName;
        }

        public void Dispose()
        {
            foreach (var item in this.dataBaseToListeners)
            {
                this.OnUnsubscribe(item.Key);
            }
            this.dataBaseToListeners.Clear();
        }

        public void Subscribe(IDataBase dataBase, DataBaseEventListener listener)
        {
            if (this.dataBaseToListeners.ContainsKey(dataBase) == false)
            {
                this.dataBaseToListeners[dataBase] = new DataBaseEventListenerCollection();
            }
            var listeners = this.dataBaseToListeners[dataBase];
            if (listeners.Any() == false)
            {
                this.OnSubscribe(dataBase);
            }
            listeners.Add(listener);
        }

        public void Unsubscribe(IDataBase dataBase, DataBaseEventListener listener)
        {
            var listeners = this.dataBaseToListeners[dataBase];
            listeners.Remove(listener);
            if (listeners.Any() == false)
            {
                this.OnUnsubscribe(dataBase);
                this.dataBaseToListeners.Remove(dataBase);
            }
        }

        public void Subscribe(IDataBase dataBase, IEnumerable<DataBaseEventListener> listeners)
        {
            this.dataBaseToListeners[dataBase] = new DataBaseEventListenerCollection(listeners);
            this.OnSubscribe(dataBase);
        }

        public void Unsubscribe(IDataBase dataBase)
        {
            this.OnUnsubscribe(dataBase);
            this.dataBaseToListeners.Remove(dataBase);
        }

        public CremaDispatcher Dispatcher
        {
            get; set;
        }

        public DataBaseEvents EventName => this.eventName;

        protected void InvokeAsync(IDataBase dataBase, IDictionary<string, object> properties)
        {
            var dataBaseName = dataBase.Name;
            var listeners = this.dataBaseToListeners[dataBase].ToArray();
            this.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in listeners)
                {
                    item.Invoke(dataBaseName, properties);
                }
            });
        }

        protected abstract void OnSubscribe(IDataBase dataBase);

        protected abstract void OnUnsubscribe(IDataBase dataBase);

        
    }
}
