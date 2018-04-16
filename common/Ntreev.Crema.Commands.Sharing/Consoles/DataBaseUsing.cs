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

namespace Ntreev.Crema.Commands.Consoles
{
    public class DataBaseUsing : IDisposable
    {
        private readonly Action action;

        private DataBaseUsing(Action action)
        {
            this.action = action;
        }

        public static DataBaseUsing Set(IDataBase dataBase, Authentication authentication)
        {
            return Set(dataBase, authentication, false);
        }

        public static DataBaseUsing Set(IDataBase dataBase, Authentication authentication, bool dispatch)
        {
            Load();
            var contains = Contains();
            Enter();
            return new DataBaseUsing(Leave);

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

        #region IDisposable

        void IDisposable.Dispose()
        {
            this.action();
        }

        #endregion
    }
}
