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

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public class UsingDataBase : IDisposable
    {
        private readonly Action action;
        private IDataBase dataBase;
        
        private UsingDataBase(Action action)
        {
            this.action = action;
        }

        public static UsingDataBase Set(ICremaHost cremaHost, string dataBaseName, Authentication authentication)
        {
            return Set(cremaHost, dataBaseName, authentication, false);
        }

        public static UsingDataBase Set(ICremaHost cremaHost, string dataBaseName, Authentication authentication, bool dispatch)
        {
            var dataBase = GetDataBase();
            if (dataBase == null)
                throw new DataBaseNotFoundException(dataBaseName);

            return Set(dataBase, authentication, dispatch);

            IDataBase GetDataBase()
            {
                if (dispatch == true)
                {
                    return cremaHost.Dispatcher.Invoke(() => cremaHost.DataBases[dataBaseName]);
                }
                else
                {
                    return cremaHost.DataBases[dataBaseName];
                }
            }
        }

        public static UsingDataBase Set(IDataBase dataBase, Authentication authentication)
        {
            return Set(dataBase, authentication, false);
        }

        public static UsingDataBase Set(IDataBase dataBase, Authentication authentication, bool dispatch)
        {
            Load();
            var contains = Contains();
            Enter();
            return new UsingDataBase(Leave) { dataBase = dataBase };

            void Load()
            {
                if (dispatch == true)
                {
                    dataBase.Dispatcher.Invoke(() =>
                    {
                        if (dataBase.IsLoaded == false)
                            dataBase.Load(authentication);
                    });
                }
                else if (dataBase.IsLoaded == false)
                {
                    dataBase.Load(authentication);
                }
            }

            bool Contains()
            {
                if (dispatch == true)
                    return dataBase.Dispatcher.Invoke(() => dataBase.Contains(authentication));
                else
                    return dataBase.Contains(authentication);
            }

            void Enter()
            {
                if (contains == false)
                {
                    if (dispatch == true)
                        dataBase.Dispatcher.Invoke(() => dataBase.Enter(authentication));
                    else
                        dataBase.Enter(authentication);
                }
            }

            void Leave()
            {
                if (contains == false)
                {
                    if (dispatch == true)
                        dataBase.Dispatcher.Invoke(() => dataBase.Leave(authentication));
                    else
                        dataBase.Leave(authentication);
                }
            }
        }

        public static UsingDataBase Set(IServiceProvider serviceProvider, Authentication authentication)
        {
            return Set(serviceProvider, authentication, false);
        }

        public static UsingDataBase Set(IServiceProvider serviceProvider, Authentication authentication, bool dispatch)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            return Set(serviceProvider.GetService(typeof(IDataBase)) as IDataBase, authentication, dispatch);            
        }

        public IDataBase DataBase
        {
            get { return this.dataBase; }
        }
        
        #region IDisposable

        void IDisposable.Dispose()
        {
            this.action();
        }

        #endregion
    }
}
