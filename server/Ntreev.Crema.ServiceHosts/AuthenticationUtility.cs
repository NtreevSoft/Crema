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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Ntreev.Crema.ServiceHosts
{
    static class AuthenticationUtility
    {
        private readonly static TimeSpan pingTimeout = new TimeSpan(0, 1, 0);
        private static Dictionary<Authentication, Description> authentications = new Dictionary<Authentication, Description>();

        private static Timer timer;

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var descriptionList = new List<Description>();
            lock (authentications)
            {
                var dateTime = DateTime.Now;
                foreach (var item in authentications.ToArray())
                {
                    var authentication = item.Key;
                    var description = item.Value;
                    if (dateTime - description.DateTime > pingTimeout)
                    {
                        descriptionList.Add(item.Value);
                        authentications.Remove(item.Key);
                    }
                }
            }

            foreach (var item in descriptionList)
            {
                item.Dispose();
            }
        }

        public static int AddRef(this Authentication authentication, ICremaServiceItem obj)
        {
            lock (authentications)
            {
                if (authentications.ContainsKey(authentication) == false)
                {
                    throw new ArgumentException(nameof(authentication));
                }

                var description = authentications[authentication];
                description.ServiceItems.Add(obj);

                return description.ServiceItems.Count;
            }
        }

        public static int AddRef(this Authentication authentication, ICremaServiceItem obj, Action<Authentication> action)
        {
            lock (authentications)
            {
                if (authentications.Any() == false && timer == null)
                {
                    timer = new Timer(30000);
                    timer.Elapsed += Timer_Elapsed;
                    timer.Start();
                }

                if (authentications.ContainsKey(authentication) == false)
                {
                    authentications[authentication] = new Description(authentication, action);
                }

                var description = authentications[authentication];
                description.ServiceItems.Add(obj);
                return description.ServiceItems.Count;
            }
        }

        public static int RemoveRef(this Authentication authentication, ICremaServiceItem obj)
        {
            lock (authentications)
            {
                var description = authentications[authentication];
                description.ServiceItems.Remove(obj);

                try
                {
                    if (description.ServiceItems.Any() == false)
                    {
                        authentications.Remove(authentication);
                        description.Dispose();
                    }

                    return description.ServiceItems.Count;
                }
                finally
                {
                    if (authentications.Any() == false && timer != null)
                    {
                        timer.Stop();
                        timer.Dispose();
                        timer = null;
                    }
                }
            }
        }

        public static void Ping(this Authentication authentication)
        {
            lock (authentications)
            {
                if (authentications.ContainsKey(authentication) == true)
                {
                    authentications[authentication].Ping();
                }
            }
        }

        #region classes

        class Description
        {
            private readonly List<ICremaServiceItem> serviceItems = new List<ICremaServiceItem>();
            private readonly Authentication authentication;
            private readonly Action<Authentication> action;

            public Description(Authentication authentication, Action<Authentication> action)
            {
                this.authentication = authentication;
                this.authentication.Expired += Authentication_Expired;
                this.action = action;
                this.DateTime = DateTime.Now;
            }

            private void Authentication_Expired(object sender, EventArgs e)
            {
                this.authentication.Expired -= Authentication_Expired;
                lock (authentications)
                {
                    authentications.Remove(authentication);
                }
                this.AbortServieItems(false);
            }

            public void Ping()
            {
                this.DateTime = DateTime.Now;
            }

            public void Dispose()
            {
                this.authentication.Expired -= Authentication_Expired;
                this.AbortServieItems(true);
                if (this.serviceItems.Any() == true)
                    this.action(this.authentication);
            }

            public Authentication Authentication
            {
                get { return this.authentication; }
            }

            public List<ICremaServiceItem> ServiceItems
            {
                get { return this.serviceItems; }
            }

            public DateTime DateTime
            {
                get; private set;
            }

            private void AbortServieItems(bool disconnect)
            {
                var items = this.serviceItems.ToArray().Reverse();
                foreach (var item in items)
                {
                    Task.Run(() => item.Abort(disconnect));
                }
            }
        }

        #endregion
    }
}
