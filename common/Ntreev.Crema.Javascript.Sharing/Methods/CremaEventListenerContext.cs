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
    class CremaEventListenerContext
    {
        private readonly IDictionary<CremaEvents, CremaEventListenerHost> eventListeners;
        private readonly CremaDispatcher dispatcher;

        public CremaEventListenerContext(CremaEventListenerHost[] eventListener)
        {
            this.eventListeners = eventListener.ToDictionary(item => item.EventName);
            this.dispatcher = new CremaDispatcher(this);
            foreach (var item in eventListener)
            {
                item.Dispatcher = this.dispatcher;
            }
        }

        public void Dispose()
        {
            this.dispatcher.Dispose();
            foreach (var item in this.eventListeners)
            {
                item.Value.Dispose();
            }
        }

        public void AddEventListener(CremaEvents eventName, CremaEventListener listener)
        {
            if (this.eventListeners.ContainsKey(eventName) == true)
            {
                var eventHandler = this.eventListeners[eventName];
                eventHandler.Subscribe(listener);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void RemoveEventListener(CremaEvents eventName, CremaEventListener listener)
        {
            if (this.eventListeners.ContainsKey(eventName) == true)
            {
                var eventHandler = this.eventListeners[eventName];
                eventHandler.Unsubscribe(listener);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
