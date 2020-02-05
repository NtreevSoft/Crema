﻿//Released under the MIT License.
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

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Ntreev.Crema.Services.Users
{
    public class UserAuthenticationCollection : IUserAuthenticationCollection, INotifyCollectionChanged
    {
        private readonly IDictionary<Guid, IUserAuthentication> dictionary = new ConcurrentDictionary<Guid, IUserAuthentication>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(sender, e);
        }

        public IEnumerator<KeyValuePair<Guid, IUserAuthentication>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<Guid, IUserAuthentication> item)
        {
            this.dictionary.Add(item);
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { item.Value }));

            foreach (var value in this.dictionary.Values)
            {
                var user = (User)value.User;
                user.OnUserStateChanged(EventArgs.Empty);
            }
        }

        public void Clear()
        {
            var user = this.dictionary.Values.Select(o => o.User).Cast<User>().FirstOrDefault();
            var authentications = this.dictionary.Values.ToArray();

            if (user != null)
            {
                this.dictionary.Clear();
                this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null));
                user.OnUserStateChanged(EventArgs.Empty);
            }
        }

        public bool Contains(KeyValuePair<Guid, IUserAuthentication> item)
        {
            return this.dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<Guid, IUserAuthentication>[] array, int arrayIndex)
        {
            this.dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<Guid, IUserAuthentication> item)
        {
            var user = this.dictionary.Values.Select(o => o.User).Cast<User>().FirstOrDefault();
            var authentications = this.dictionary.Values.Where(auth => auth.Authentication.Token == item.Key).ToArray();
            var result = dictionary.Remove(item);

            if (user != null)
            {
                this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, authentications));
                user.OnUserStateChanged(EventArgs.Empty);

            }

            return result;
        }

        public int Count => this.dictionary.Count;
        public bool IsReadOnly => this.dictionary.IsReadOnly;

        public bool ContainsKey(Guid key)
        {
            return this.dictionary.ContainsKey(key);
        }

        public void Add(Guid key, IUserAuthentication value)
        {
            this.dictionary.Add(key, value);
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { value }));

            foreach (var val in this.dictionary.Values)
            {
                var user = (User)val.User;
                user.OnUserStateChanged(EventArgs.Empty);
            }
        }

        public bool Remove(Guid key)
        {
            var user = this.dictionary.Values.Select(o => o.User).Cast<User>().FirstOrDefault();
            var authentications = this.dictionary.Values.Where(auth => auth.Authentication.Token == key).ToArray();
            var result = this.dictionary.Remove(key);

            if (user != null)
            {
                this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, authentications));
                user.OnUserStateChanged(EventArgs.Empty);
            }

            return result;
        }

        public bool TryGetValue(Guid key, out IUserAuthentication value)
        {
            return this.dictionary.TryGetValue(key, out value);
        }

        public IUserAuthentication this[Guid key]
        {
            get => this.dictionary[key];
            set => this.dictionary[key] = value;
        }

        public ICollection<Guid> Keys => this.dictionary.Keys;
        public ICollection<IUserAuthentication> Values => this.dictionary.Values;
    }
}