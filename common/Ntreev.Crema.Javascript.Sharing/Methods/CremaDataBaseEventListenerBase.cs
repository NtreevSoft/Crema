using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.Javascript.Methods
{
    abstract class CremaDataBaseEventListenerBase
    {
        private readonly CremaDataBaseEvents eventName;
        private readonly Dictionary<IDataBase, List<CremaDataBaseEventListener>> dataBases = new Dictionary<IDataBase, List<CremaDataBaseEventListener>>();
        //private readonly Dictionary<int, CremaDataBaseEventListener> hashes = new Dictionary<int, CremaDataBaseEventListener>();

        protected CremaDataBaseEventListenerBase(CremaDataBaseEvents eventName)
        {
            this.eventName = eventName;
        }

        public void Dispose()
        {
            foreach (var item in this.dataBases)
            {
                this.OnUnsubscribe(item.Key);
            }
            this.dataBases.Clear();
        }

        public int Subscribe(IDataBase dataBase, CremaDataBaseEventListener listener)
        {
            if (this.dataBases.ContainsKey(dataBase) == false)
            {
                this.dataBases[dataBase] = new List<CremaDataBaseEventListener>();
            }
            var listenerList = this.dataBases[dataBase];
            if (listenerList.Any() == false)
            {
                this.OnSubscribe(dataBase);
            }
            listenerList.Add(listener);
            return listener.GetHashCode();
        }

        public void Unsubscribe(IDataBase dataBase, CremaDataBaseEventListener listener)
        {
            var listenerList = this.dataBases[dataBase];

            for (var i = 0; i < listenerList.Count; i++)
            {
                if (this.GetHashCode(listenerList[i]) == this.GetHashCode(listener))
                {
                    listenerList.RemoveAt(i);
                    break;
                }
            }
            if (listenerList.Any() == false)
            {
                this.OnUnsubscribe(dataBase);
                this.dataBases.Remove(dataBase);
            }
        }

        public CremaDispatcher Dispatcher
        {
            get; set;
        }

        public CremaDataBaseEvents EventName => this.eventName;

        protected void Invoke(IDictionary<string, object> properties)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in this.dataBases)
                {
                    //item.Invoke(properties);
                }
            });
        }

        protected abstract void OnSubscribe(IDataBase dataBase);

        protected abstract void OnUnsubscribe(IDataBase dataBase);

        //private void DataBase_Unloaded(object sender, EventArgs e)
        //{
        //    if (sender is IDataBase dataBase)
        //    {
        //        var listenerList = this.dataBases[dataBase];
        //        foreach (var item in listenerList)
        //        {
        //            this.hashes.Remove(item.GetHashCode());
        //        }
        //        this.dataBases.Remove(dataBase);
        //    }
        //}

        private int GetHashCode(CremaDataBaseEventListener listener)
        {
            if (listener.Target is System.Runtime.CompilerServices.Closure c)
            {
                return c.Constants[0].GetHashCode();
            }
            throw new NotImplementedException();
        }
    }
}
