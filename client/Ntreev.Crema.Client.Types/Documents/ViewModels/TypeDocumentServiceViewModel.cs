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

using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Types.BrowserItems.ViewModels;
using Ntreev.Crema.Client.Types.Properties;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Client.Types.Documents.ViewModels
{
    [Export(typeof(ITypeDocumentService))]
    [InheritedExport(typeof(TypeDocumentServiceViewModel))]
    class TypeDocumentServiceViewModel : DocumentServiceBase<IDocument>, ITypeDocumentService
    {
        private readonly ICremaAppHost cremaAppHost;

        [Import]
        private ICompositionService compositionService = null;

        [ImportingConstructor]
        public TypeDocumentServiceViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
            this.cremaAppHost.Resetting += CremaAppHost_Resetting;
            this.cremaAppHost.Reset += CremaAppHost_Reset;
            this.DisplayName = Resources.Title_Types;
        }

        public void AddFinder(Authentication authentication, ITypeItemDescriptor descriptor)
        {
            this.AddFinder(authentication, descriptor.Path);
        }

        public void MoveToType(Authentication authentication, ITypeDescriptor descriptor, string columnName, int row)
        {
            if (descriptor.Target is IType type)
            {
                this.MoveToType(authentication, type, columnName, row);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void OpenType(Authentication authentication, ITypeDescriptor descriptor)
        {
            if (descriptor.Target is IType type)
            {
                this.OpenType(authentication, type);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void AddFinder(Authentication authentication, string itemPath)
        {
            if (this.cremaAppHost.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                var document = new TypeDataFinderViewModel(authentication, dataBase, itemPath);
                this.compositionService?.SatisfyImportsOnce(document);
                this.Items.Add(document);
                this.ActivateItem(document);
            }
        }

        public void MoveToType(Authentication authentication, IType type, string columnName, int row)
        {
            var document = this.Items.OfType<TypeViewModel>().FirstOrDefault(item => item.Target == type);
            if (document == null)
            {
                document = new TypeViewModel(authentication, type);
                this.compositionService?.SatisfyImportsOnce(document);
                this.Items.Add(document);
            }

            this.ActivateItem(document);
            new SelectFieldStrategy(document, document.Source.TypeName, columnName, row);
        }

        public void OpenType(Authentication authentication, IType type)
        {
            var document = this.Items.OfType<TypeViewModel>().FirstOrDefault(item => item.Target == type);
            if (document == null)
            {
                document = new TypeViewModel(authentication, type);
                this.compositionService?.SatisfyImportsOnce(document);
                this.Items.Add(document);
            }

            this.ActivateItem(document);
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            if (this.SelectedDocument is TypeViewModel document)
            {
                var type = document.Target;
                var browser = type.GetService(typeof(ITypeBrowser)) as ITypeBrowser;
                var viewModel = type.ExtendedProperties[browser];
                browser.SelectedItem = viewModel;
            }
        }

        private void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync(() => this.Items.Clear());
        }

        private void CremaAppHost_Resetting(object sender, EventArgs e)
        {
            this.Items.Clear();
        }

        private void CremaAppHost_Reset(object sender, EventArgs e)
        {

        }
    }
}
