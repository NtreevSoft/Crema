using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Javascript.Methods
{
    abstract class CremaEventListenerHost
    {
        private readonly CremaEvents eventName;
        private readonly CremaEventListenerCollection listeners = new CremaEventListenerCollection();

        protected CremaEventListenerHost(CremaEvents eventName)
        {
            this.eventName = eventName;
        }

        public void Dispose()
        {
            if (this.listeners.Any() == true)
            {
                this.OnUnsubscribe();
            }
            this.listeners.Clear();
        }

        public void Subscribe(CremaEventListener listener)
        {
            if (this.listeners.Any() == false)
            {
                this.OnSubscribe();
            }

            this.listeners.Add(listener);
        }

        public void Unsubscribe(CremaEventListener listener)
        {
            this.listeners.Remove(listener);

            if (this.listeners.Any() == false)
            {
                this.OnUnsubscribe();
            }
        }

        public CremaDispatcher Dispatcher
        {
            get; set;
        }

        public CremaEvents EventName => this.eventName;

        protected void Invoke(IDictionary<string, object> properties)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in this.listeners)
                {
                    item.Invoke(properties);
                }
            });
        }

        protected abstract void OnSubscribe();

        protected abstract void OnUnsubscribe();
    }
}
