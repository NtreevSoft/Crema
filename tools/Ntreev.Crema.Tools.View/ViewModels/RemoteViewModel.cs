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

using Ntreev.Crema.Reader;
using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Crema.RuntimeService;
using Ntreev.Crema.Tools.Framework;
using Ntreev.Crema.Tools.View.Dialogs.ViewModels;
using Ntreev.Crema.Tools.View.Views;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ntreev.Crema.Tools.View.ViewModels
{
    [View(typeof(ItemsView))]
    class RemoteViewModel : ContentBase, IDisposable
    {
        private readonly DataViewModel dataViewModel;
        private readonly IRuntimeService service;
        private readonly IDataSerializer serializer;
        private IDataSet dataSet;
        private ObservableCollection<ItemViewModel> tables = new ObservableCollection<ItemViewModel>();

        public RemoteViewModel(DataViewModel dataViewModel, IRuntimeService service, IDataSerializer serializer)
        {
            this.DisplayName = string.Empty;
            this.GroupName = "View";
            this.dataViewModel = dataViewModel;
            this.service = service;
            this.serializer = serializer;
        }

        public async void Connect(string address, string dataBaseName, string tags, string filterExpression, bool isDevmode)
        {
            this.DisplayName = "connecting...";

            try
            {
                this.BeginProgress();
                var metaData = await Task.Run(() => this.service.GetDataGenerationData(address, dataBaseName, tags, filterExpression, isDevmode, null));
                using (var stream = new MemoryStream())
                {
                    this.serializer.Serialize(stream, metaData);
                    stream.Position = 0;
                    this.dataSet = CremaReader.Read(stream);
                }

                this.tables.Clear();

                foreach (var item in this.dataSet.Tables.OrderBy(i => i.Name))
                {
                    this.tables.Add(new ItemViewModel(item));
                }
                this.EndProgress();
            }
            catch (Exception e)
            {
                AppMessageBox.ShowError(e);
                this.EndProgress();
                this.Dispose();
                return;
            }

            this.DisplayName = address;
            this.NotifyOfPropertyChange(() => this.ItemsSource);
        }

        public void Dispose()
        {
            if (this.IsProgressing == true)
                return;
            this.OnDisposed(EventArgs.Empty);
        }

        public IEnumerable ItemsSource
        {
            get { return this.tables; }
        }

        public ICommand LoadCommand
        {
            get { return this.dataViewModel.LoadCommand; }
        }

        public event EventHandler Disposed;

        protected virtual void OnDisposed(EventArgs e)
        {
            this.Disposed?.Invoke(this, e);
        }
    }
}
