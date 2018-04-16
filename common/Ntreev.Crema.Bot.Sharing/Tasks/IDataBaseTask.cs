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

using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(IDataBaseTask))]
    [Export(typeof(IConfigurationPropertyProvider))]
    [TaskClass]
    public class IDataBaseTask : ITaskProvider, IConfigurationPropertyProvider
    {
        public void InvokeTask(TaskContext context)
        {
            var dataBase = context.Target as IDataBase;
            dataBase.Dispatcher.Invoke(() =>
            {
                if (context.IsCompleted(dataBase) == true)
                {
                    context.Pop(dataBase);
                }
                else
                {
                    var item = SelectItem();
                    if (item != null)
                    {
                        context.Push(item);
                    }
                    object SelectItem()
                    {
                        if (dataBase.Contains(context.Authentication) == true)
                        {
                            if (RandomUtility.Within(35) == true)
                                return dataBase.TypeContext.Random();
                            return dataBase.TableContext.Random();
                        }
                        return null;
                    }
                }
            });
        }

        public Type TargetType
        {
            get { return typeof(IDataBase); }
        }

        [TaskMethod]
        public void Enter(IDataBase dataBase, TaskContext context)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                if (dataBase.Contains(context.Authentication) == false)
                {
                    if (dataBase.IsLoaded == false)
                        dataBase.Load(context.Authentication);
                    dataBase.Enter(context.Authentication);
                }
            });
        }

        [TaskMethod]
        public void Leave(IDataBase dataBase, TaskContext context)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                if (context.IsCompleted(dataBase) == true)
                {
                    dataBase.Leave(context.Authentication);
                    context.Pop(dataBase);
                }
            });
        }

        [TaskMethod(Weight = 10)]
        public void Rename(IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var dataBaseName = RandomUtility.NextIdentifier();
                dataBase.Rename(authentication, dataBaseName);
            });
        }

        [TaskMethod(Weight = 1)]
        public void Delete(IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                //dataBase.Delete(authentication);
            });
        }

        [TaskMethod(Weight = 5)]
        public void Copy(IDataBase dataBase, Authentication authentication)
        {
            dataBase.Dispatcher.Invoke(() =>
            {
                var dataBaseName = RandomUtility.NextIdentifier();
                var comment = RandomUtility.NextString();
                var force = RandomUtility.NextBoolean();
                dataBase.Copy(authentication, dataBaseName, comment, force);
            });
        }

        [ConfigurationProperty(ScopeType = typeof(ICremaConfiguration))]
        public bool IsEnabled
        {
            get; set;
        }

        #region IConfigurationPropertyProvider

        string IConfigurationPropertyProvider.Name => "bot.database";

        #endregion
    }
}