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

namespace Ntreev.Crema.Bot.Tasks
{
    [Export(typeof(ITaskProvider))]
    [Export(typeof(IDataBaseCollectionTask))]
    [Export(typeof(IConfigurationPropertyProvider))]
    [TaskClass(Weight = 10)]
    public class IDataBaseCollectionTask : ITaskProvider, IConfigurationPropertyProvider
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        [ImportingConstructor]
        public IDataBaseCollectionTask()
        {
            
        }

        public void InvokeTask(TaskContext context)
        {
            var dataBases = context.Target as IDataBaseCollection;
            if (context.IsCompleted(dataBases) == true)
            {
                context.Pop(dataBases);
            }
            else 
            {
                context.Complete(dataBases);
            }
        }

        public CremaDispatcher Dispatcher
        {
            get { return this.CremaHost.Dispatcher; }
        }

        public Type TargetType
        {
            get { return typeof(IDataBaseCollection); }
        }

        [TaskMethod(Weight = 10)]
        public void AddNewDataBase(IDataBaseCollection dataBases, TaskContext context)
        {
            this.Dispatcher.Invoke(() =>
            {
                var dataBaseName = RandomUtility.NextIdentifier();
                var comment = RandomUtility.NextString();
                dataBases.AddNewDataBase(context.Authentication, dataBaseName, comment);
            });
        }

        [ConfigurationProperty(ScopeType = typeof(ICremaConfiguration))]
        public bool IsEnabled
        {
            get; set;
        }

        private ICremaHost CremaHost
        {
            get => this.cremaHost.Value;
        }

        #region IConfigurationPropertyProvider

        string IConfigurationPropertyProvider.Name => "bot.databaseCollection";

        #endregion
    }
}
