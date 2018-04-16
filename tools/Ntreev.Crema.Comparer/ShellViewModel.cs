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
using Ntreev.Crema.Data.Diff;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using Ntreev.Crema.Comparer.Dialogs.ViewModels;
using System.Windows;
using Ntreev.Library;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System.Threading;

namespace Ntreev.Crema.Comparer
{
    [Export(typeof(IShell))]
    [Export]
    class ShellViewModel : ScreenBase, IShell
    {
        private readonly ObservableCollection<IContent> contents;
        private DiffDataSet dataSet;
        private bool isLoaded;
        private IContent selectedContent;
        private double left = double.NaN;
        private double top = double.NaN;
        private double width = 800.0;
        private double height = 600.0;
        private WindowState windowState;

        [Import]
        private IAppConfiguration configs = null;
        [Import]
        private IServiceProvider serviceProvider = null;

        [ImportingConstructor]
        public ShellViewModel([ImportMany]IEnumerable<IContent> contents)
        {
            this.contents = new ObservableCollection<IContent>();
            foreach (var item in contents)
            {
                this.contents.Add(item);
            }

            this.selectedContent = this.contents.First();
            this.DisplayName = "Crema Compare";
        }

        public async void Open(string path1, string path2, string filterExpression)
        {
            try
            {
                this.BeginProgress("loading data");
                this.dataSet = await Task.Run(async () =>
                {
                    var filter = filterExpression == string.Empty ? null : filterExpression;
                    var tasks = new Task<CremaDataSet>[]
                    {
                        Task.Run(() => CremaDataSet.ReadFromDirectory(path1, filter)),
                        Task.Run(() => CremaDataSet.ReadFromDirectory(path2, filter)),
                    };

                    Task.WaitAll(tasks);
                    var dataSet1 = tasks[0].Result;
                    var dataSet2 = tasks[1].Result;

                    dataSet1.ExtendedProperties["location"] = Path.GetFileNameWithoutExtension(path1);
                    dataSet2.ExtendedProperties["location"] = Path.GetFileNameWithoutExtension(path2);
                    await this.Dispatcher.InvokeAsync(() => this.ProgressMessage = "comparing data");
                    return this.Compare(dataSet1, dataSet2);
                });
                this.EndProgress();
                this.OnLoaded(EventArgs.Empty);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e.Message);
                this.EndProgress();
                return;
            }
        }

        public async void Open(CremaDataSet dataSet1, CremaDataSet dataSet2)
        {
            try
            {
                this.BeginProgress();
                var location1 = dataSet1.ExtendedProperties.Contains("location") ? $"{dataSet1.ExtendedProperties["location"]}" : "Before";
                var location2 = dataSet2.ExtendedProperties.Contains("location") ? $"{dataSet2.ExtendedProperties["location"]}" : "Current";
                this.dataSet = await Task.Run(() => this.Compare(dataSet1, dataSet2));
                this.EndProgress();
                this.OnLoaded(EventArgs.Empty);
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e.Message);
                this.EndProgress();
                return;
            }
        }

        public async void Open()
        {
            var dialog = new OpenPathViewModel();
            this.configs.Update(dialog);
            if (dialog.ShowDialog() == true)
            {
                if (this.dataSet != null)
                {
                    await this.CloseDocumentsAsync(true);

                    this.dataSet = null;
                    this.IsLoaded = false;
                    this.OnUnloaded(EventArgs.Empty);
                }

                this.Open(dialog.Path1, dialog.Path2, dialog.FilterExpression);
                this.configs.Commit(dialog);
            }
        }

        public async void Close()
        {
            await this.CloseDocumentsAsync(true);

            this.dataSet = null;
            this.IsLoaded = false;
            this.OnUnloaded(EventArgs.Empty);
        }

        public bool CanOpen
        {
            get { return this.IsProgressing == false; }
        }

        public new bool CanClose
        {
            get { return this.dataSet != null; }
        }

        public IContent SelectedContent
        {
            get { return this.selectedContent; }
            set
            {
                if (this.selectedContent == value)
                    return;
                if (this.selectedContent != null)
                    this.selectedContent.IsVisible = false;
                this.selectedContent = this.contents.SingleOrDefault(item => item == value);
                if (this.selectedContent != null)
                    this.selectedContent.IsVisible = true;
                this.NotifyOfPropertyChange(() => this.SelectedContent);
            }
        }

        [ConfigurationProperty]
        public string SelectedContentName
        {
            get
            {
                if (this.selectedContent != null)
                    return this.selectedContent.DisplayName;
                return string.Empty;
            }
            set
            {
                foreach (var item in this.contents)
                {
                    if (item.DisplayName == value)
                    {
                        this.SelectedContent = item;
                        break;
                    }
                }
            }
        }

        [ConfigurationProperty]
        public double Left
        {
            get { return this.left; }
            set
            {
                if (this.WindowState == WindowState.Maximized)
                    return;

                this.left = value;
                this.NotifyOfPropertyChange(nameof(this.Left));
            }
        }

        [ConfigurationProperty]
        public double Top
        {
            get { return this.top; }
            set
            {
                if (this.WindowState == WindowState.Maximized)
                    return;

                this.top = value;
                this.NotifyOfPropertyChange(nameof(this.Top));
            }
        }

        [ConfigurationProperty]
        public double Width
        {
            get { return this.width; }
            set
            {
                if (this.WindowState == WindowState.Maximized)
                    return;

                this.width = value;
                this.NotifyOfPropertyChange(nameof(this.Width));
            }
        }

        [ConfigurationProperty]
        public double Height
        {
            get { return this.height; }
            set
            {
                if (this.WindowState == WindowState.Maximized)
                    return;

                this.height = value;
                this.NotifyOfPropertyChange(nameof(this.Height));
            }
        }

        [ConfigurationProperty]
        public WindowState WindowState
        {
            get { return this.windowState; }
            set
            {
                this.windowState = value;
                this.NotifyOfPropertyChange(nameof(this.WindowState));
            }
        }

        public ObservableCollection<IContent> Contents
        {
            get { return this.contents; }
        }

        public DiffDataSet DataSet
        {
            get { return this.dataSet; }
        }

        public bool IsLoaded
        {
            get { return this.isLoaded; }
            private set
            {
                this.isLoaded = value;
                this.NotifyOfPropertyChange(nameof(this.IsLoaded));
            }
        }

        public event EventHandler Loaded;

        public event EventHandler Unloaded;

        public event EventHandler Closing;

        public event EventHandler Closed;

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            this.configs.Update(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close == true)
            {
                this.configs.Commit(this);
                this.OnClosed(EventArgs.Empty);
            }
        }

        protected virtual void OnLoaded(EventArgs e)
        {
            this.Loaded?.Invoke(this, e);
        }

        protected virtual void OnUnloaded(EventArgs e)
        {
            this.Unloaded?.Invoke(this, e);
        }

        protected virtual void OnClosing(EventArgs e)
        {
            this.Closing?.Invoke(this, e);
        }

        protected virtual void OnClosed(EventArgs e)
        {
            this.Closed?.Invoke(this, e);
        }

        protected override void OnProgress()
        {
            base.OnProgress();
            this.NotifyOfPropertyChange(nameof(this.CanOpen));
            this.NotifyOfPropertyChange(nameof(this.CanClose));
        }

        protected override void OnClose()
        {
            base.OnClose();
            this.OnClosing(EventArgs.Empty);
        }

        private async Task CloseDocumentsAsync(bool save)
        {
            var documentServices = this.serviceProvider.GetService(typeof(IEnumerable<IDocumentService>)) as IEnumerable<IDocumentService>;
            var query = from documentService in documentServices
                        from document in documentService.Documents
                        select document;

            var documentList = query.ToList();
            foreach (var item in documentList.ToArray())
            {
                item.Disposed += (s, e) => documentList.Remove(item);
                if (item.IsModified == true && save == false)
                    item.IsModified = false;
                await this.Dispatcher.InvokeAsync(() => item.Dispose());
                await Task.Delay(1);
            }

            while (documentList.Any())
            {
                await Task.Delay(1);
            }
        }

        private DiffDataSet Compare(CremaDataSet dataSet1, CremaDataSet dataSet2)
        {
            var location1 = dataSet1.ExtendedProperties.Contains("location") ? $"{dataSet1.ExtendedProperties["location"]}" : "Before";
            var location2 = dataSet2.ExtendedProperties.Contains("location") ? $"{dataSet2.ExtendedProperties["location"]}" : "Current";
            var diffSet = new DiffDataSet(dataSet1, dataSet2)
            {
                Header1 = location1,
                Header2 = location2,
            };
            return diffSet;
        }
    }
}
