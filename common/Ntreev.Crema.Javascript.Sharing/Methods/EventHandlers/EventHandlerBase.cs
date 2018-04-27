using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.EventHandlers
{
    abstract class EventHandlerBase
    {
        private readonly CremaEvents eventName;
        private readonly Action<IDictionary<string, object>> action;
        private readonly string key;

        protected EventHandlerBase(CremaEvents eventName, Action<IDictionary<string, object>> action)
        {
            this.eventName = eventName;
            this.action = action;
            this.key = $"{eventName}_{action.GetHashCode()}";
        }

        public string Key => this.key;

        protected void Invoke(IDictionary<string, object> properties)
        {
            this.action(properties);
        }

        public abstract void Subscribe();

        public abstract void Unsubscribe();
    }
}
