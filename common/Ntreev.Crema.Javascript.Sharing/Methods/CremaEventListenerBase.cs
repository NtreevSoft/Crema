using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Javascript.Methods
{
    abstract class CremaEventListenerBase
    {
        private readonly CremaEvents eventName;
        private readonly List<CremaEventListener> listenerList = new List<CremaEventListener>();
        private readonly Dictionary<int, CremaEventListener> hashTolistener = new Dictionary<int, CremaEventListener>();

        protected CremaEventListenerBase(CremaEvents eventName)
        {
            this.eventName = eventName;
        }

        public void Dispose()
        {
            if (this.listenerList.Any() == true)
            {
                this.OnUnsubscribe();
            }
            this.listenerList.Clear();
        }

        private int GetHashCode(CremaEventListener listener)
        {
            if (listener.Target is System.Runtime.CompilerServices.Closure c)
            {
                return c.Constants[0].GetHashCode();
            }
            throw new NotImplementedException();
        }

        public int Subscribe(CremaEventListener listener)
        {
            if (this.listenerList.Any() == false)
            {
                this.OnSubscribe();
            }

            this.listenerList.Add(listener);
            this.hashTolistener.Add(listener.GetHashCode(), listener);
            return listener.GetHashCode();
        }

        public void Unsubscribe(CremaEventListener listener)
        {
            for (var i = 0; i < this.listenerList.Count; i++)
            {
                if (this.GetHashCode(this.listenerList[i]) == this.GetHashCode(listener))
                {
                    this.listenerList.RemoveAt(i);
                    break;
                }
            }

            if (this.listenerList.Any() == false)
            {
                this.OnUnsubscribe();
            }
        }

        public void Unsubscribe(int hashCode)
        {
            var callback = this.hashTolistener[hashCode];
            this.hashTolistener.Remove(hashCode);
            this.listenerList.Remove(callback);
            if (this.listenerList.Any() == false)
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
                foreach (var item in this.listenerList)
                {
                    item.Invoke(properties);
                }
            });
        }

        protected abstract void OnSubscribe();

        protected abstract void OnUnsubscribe();
    }
}
