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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shell;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.ApplicationHost.Properties;
using System.Collections.ObjectModel;
using System.Threading;

namespace Ntreev.Crema.ApplicationHost.ViewModels
{
    [Export(typeof(IStatusBarService))]
    class StatusBarViewModel : PropertyChangedBase, IStatusBarService
    {
        private readonly ICremaAppHost cremaAppHost;

        private TaskbarItemInfo taskbarItemInfo;
        private string message;
        private ILineInfo lineInfo;
        private ObservableCollection<BackgroundTaskItemViewModel> taskItemList = new ObservableCollection<BackgroundTaskItemViewModel>();
        private ReadOnlyObservableCollection<BackgroundTaskItemViewModel> taskItems;
        private BackgroundTaskItemViewModel selectedTask;

        [ImportingConstructor]
        public StatusBarViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
            this.taskItems = new ReadOnlyObservableCollection<BackgroundTaskItemViewModel>(this.taskItemList);
            this.taskbarItemInfo = new TaskbarItemInfo();
            this.taskbarItemInfo.Description = "Crema Desc";
        }

        public void AddTask(IBackgroundTask task)
        {
            var taskViewModel = new BackgroundTaskItemViewModel(task);
            this.taskItemList.Add(taskViewModel);
            if (this.selectedTask == null)
            {
                this.SelectedTask = taskViewModel;
            }
            taskViewModel.Disposed += TaskViewModel_Disposed;
        }

        public TaskbarItemInfo TaskbarItemInfo
        {
            get { return taskbarItemInfo; }
        }

        public string Message
        {
            get { return this.message ?? Resources.Label_Ready; }
            set
            {
                this.message = value;
                this.NotifyOfPropertyChange(nameof(this.Message));
            }
        }

        public ILineInfo LineInfo
        {
            get { return this.lineInfo; }
            set
            {
                this.lineInfo = value;
                this.NotifyOfPropertyChange(nameof(this.LineInfo));
            }
        }

        public ReadOnlyObservableCollection<BackgroundTaskItemViewModel> TaskItems
        {
            get { return this.taskItems; }
        }

        public BackgroundTaskItemViewModel SelectedTask
        {
            get { return this.selectedTask; }
            set
            {
                this.selectedTask = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedTask));
            }
        }

        private void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            this.LineInfo = null;
        }

        private void CremaAppHost_Loaded(object sender, EventArgs e)
        {

        }

        private void TaskViewModel_Disposed(object sender, EventArgs e)
        {
            if (sender is BackgroundTaskItemViewModel taskViewModel)
            {
                this.taskItemList.Remove(taskViewModel);
                if (this.selectedTask == taskViewModel)
                {
                    this.SelectedTask = null;
                }
            }
        }
    }
}
