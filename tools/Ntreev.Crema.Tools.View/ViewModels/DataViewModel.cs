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

using Microsoft.WindowsAPICodePack.Dialogs;
using Ntreev.Crema.Runtime.Serialization;
using Ntreev.Crema.RuntimeService;
using Ntreev.Crema.Tools.Framework;
using Ntreev.Crema.Tools.View.Dialogs.ViewModels;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ntreev.Crema.Tools.View.ViewModels
{
    [Export(typeof(IContent))]
    class DataViewModel : ContentBase
    {
        [Import]
        private IRuntimeService service = null;
        [Import]
        private IAppConfiguration configs = null;
        [ImportMany]
        private IEnumerable<IDataSerializer> serializers = null;
        [Import]
        private Lazy<IContentService> contentService = null;

        private ICommand loadCommand;

        [ImportingConstructor]
        public DataViewModel()
        {
            this.DisplayName = "New View...";
            this.GroupName = "View";
            this.loadCommand = new DelegateCommand((p) => this.Load(p as string));
        }

        public void Load()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("crema data", "*.dat"));

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.Load(dialog.FileName);
            }
        }

        public void Load(string filename)
        {
            var contentService = this.contentService.Value;
            var viewModel = new FileViewModel(this);
            contentService.Contents.Add(viewModel);
            contentService.SelectedContent = viewModel;
            viewModel.Disposed += ViewModel_Disposed;
            viewModel.Open(filename);
        }

        public void Connect()
        {
            var dialog = new ConnectionViewModel();
            this.configs.Update(dialog);

            if (dialog.ShowDialog() == true)
            {
                var contentService = this.contentService.Value;
                var viewModel = new RemoteViewModel(this, this.service, this.serializers.First(item => item.Name == "bin"));
                
                contentService.Contents.Add(viewModel);
                contentService.SelectedContent = viewModel;
                viewModel.Disposed += ViewModel_Disposed;
                viewModel.Connect(dialog.Address, dialog.DataBase, dialog.Tags, dialog.FilterExpression, dialog.IsDevmode);
                this.configs.Commit(dialog);
            }
        }

        public ICommand LoadCommand
        {
            get { return this.loadCommand; }
        }

        private void ViewModel_Disposed(object sender, EventArgs e)
        {
            var contentService = this.contentService.Value;
            contentService.Contents.Remove(sender);
            contentService.SelectedContent = this;
        }
    }
}
