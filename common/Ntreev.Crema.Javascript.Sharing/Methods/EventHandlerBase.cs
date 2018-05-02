using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Ntreev.Crema.Javascript.Methods
{
    abstract class EventHandlerBase
    {
        private readonly CremaEvents eventName;
        private readonly List<Action<IDictionary<string, object>>> callbackList = new List<Action<IDictionary<string, object>>>();
        private readonly Dictionary<int, Action<IDictionary<string, object>>> hashCodeToCallback = new Dictionary<int, Action<IDictionary<string, object>>>();

        protected EventHandlerBase(CremaEvents eventName)
        {
            this.eventName = eventName;
        }

        public void Dispose()
        {
            if (this.callbackList.Any() == true)
            {
                this.OnUnsubscribe();
            }
            this.callbackList.Clear();
        }

        public int Subscribe(Action<IDictionary<string, object>> callback)
        {
            if (this.callbackList.Any() == false)
            {
                this.OnSubscribe();
            }
            this.callbackList.Add(callback);
            this.hashCodeToCallback.Add(callback.GetHashCode(), callback);
            return callback.GetHashCode();
        }

        public void Unsubscribe(Action<IDictionary<string, object>> callback)
        {
            this.callbackList.Remove(callback);
            if (this.callbackList.Any() == false)
            {
                this.OnUnsubscribe();
            }
        }

        public void Unsubscribe(int hashCode)
        {
            var callback = this.hashCodeToCallback[hashCode];
            this.hashCodeToCallback.Remove(hashCode);
            this.callbackList.Remove(callback);
            if (this.callbackList.Any() == false)
            {
                this.OnUnsubscribe();
            }
        }

        public CremaEvents EventName => this.eventName;

        protected void Invoke(IDictionary<string, object> properties)
        {
            foreach (var item in this.callbackList)
            {
                item.Invoke(properties);
            }
        }

        protected abstract void OnSubscribe();

        protected abstract void OnUnsubscribe();
    }
}
