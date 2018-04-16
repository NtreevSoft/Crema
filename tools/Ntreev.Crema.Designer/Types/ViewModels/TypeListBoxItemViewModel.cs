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
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ntreev.Crema.Designer.Types.ViewModels
{
    class TypeListBoxItemViewModel : ListBoxItemViewModel, ITypeDescriptor
    {
        private readonly TypeTreeViewItemViewModel descriptor;
        private readonly ICommand selectInBrowserCommand;
        [Import]
        private IServiceProvider serviceProvider = null;

        public TypeListBoxItemViewModel(TypeTreeViewItemViewModel descriptor)
        {
            this.Target = descriptor;
            this.descriptor = descriptor;
            if (this.descriptor is INotifyPropertyChanged)
            {
                (this.descriptor as INotifyPropertyChanged).PropertyChanged += Descriptor_PropertyChanged;
            }
            this.selectInBrowserCommand = new DelegateCommand(item => this.SelectInBrowser());
        }

        public async void SelectInBrowser()
        {
            var type = this.descriptor.Target;
            var browser = this.serviceProvider.GetService(typeof(TypeBrowserViewModel)) as TypeBrowserViewModel;
            var shell = this.serviceProvider.GetService(typeof(IShell)) as IShell;
            var typeService = this.serviceProvider.GetService(typeof(TypesViewModel)) as TypesViewModel;
            await this.Dispatcher.InvokeAsync(() => shell.SelectedContent = typeService, DispatcherPriority.Background);
            await this.Dispatcher.InvokeAsync(() => browser.SelectedItem = this.descriptor, DispatcherPriority.Background);
        }

        public override string DisplayName
        {
            get { return this.TypeInfo.Name; }
        }

        public string Path
        {
            get { return this.TypeInfo.CategoryPath + this.TypeInfo.Name; }
        }

        public TypeAttribute TypeAttribute
        {
            get { return this.descriptor.TypeAttribute; }
        }

        public TypeInfo TypeInfo
        {
            get { return this.descriptor.TypeInfo; }
        }

        public TypeState TypeState
        {
            get { return this.descriptor.TypeState; }
        }

        public bool IsBeingEdited
        {
            get { return this.TypeState.HasFlag(TypeState.IsBeingEdited); }
        }

        public bool IsFlag
        {
            get { return this.TypeInfo.IsFlag; }
        }

        public ICommand SelectInBrowserCommand
        {
            get { return this.selectInBrowserCommand; }
        }

        private void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyOfPropertyChange(e.PropertyName);
        }

        #region ITypeObject

        CremaDataType ITypeDescriptor.Target
        {
            get { return this.descriptor.Target as CremaDataType; }
        }

        ITypeDescriptor ITypeDescriptor.Parent
        {
            get { return null; }
        }

        IEnumerable<ITypeDescriptor> ITypeDescriptor.Childs
        {
            get { yield break; }
        }

        #endregion
    }
}
