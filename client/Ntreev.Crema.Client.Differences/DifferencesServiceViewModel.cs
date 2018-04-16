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

using Ntreev.Crema.Client.Differences.Documents.ViewModels;
using Ntreev.Crema.Client.Differences.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Ntreev.Crema.Client.Differences
{
    [Export(typeof(IContentService))]
    [InheritedExport(typeof(DifferencesServiceViewModel))]
    [Order(50)]
    class DifferencesServiceViewModel : ScreenBase, IContentService, IPartImportsSatisfiedNotification
    {
        private readonly ICremaAppHost cremaAppHost;

        [Import]
        private IBrowserService browserService = null;
        [Import]
        private DocumentServiceViewModel documentService = null;
        [Import]
        private IPropertyService propertyService = null;
        [Import]
        private Lazy<IShell> shell = null;

        private bool isBrowserExpanded = true;
        private bool isPropertyExpanded = true;

        private double browserDistance = 250.0;
        private double propertyDistance = 250.0;

        private bool isVisible;

        [Import]
        private IAppConfiguration configs = null;

        [ImportingConstructor]
        public DifferencesServiceViewModel(ICremaAppHost cremaAppHost, IBrowserService browserService, DocumentServiceViewModel contentsService, IPropertyService propertyService)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += CremaAppHost_Opened;
            this.cremaAppHost.Closed += CremaAppHost_Closed;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
            this.DisplayName = Resources.Title_Differences;
        }

        public override string ToString()
        {
            return Resources.Title_Differences;
        }

        public void Dispose()
        {
            this.documentService.TryClose();
        }

        public IBrowserService BrowserService
        {
            get { return this.browserService; }
        }

        public DocumentServiceViewModel DocumentService
        {
            get { return this.documentService; }
        }

        public IPropertyService PropertyService
        {
            get { return this.propertyService; }
        }

        [ConfigurationProperty("isBrowserExpanded")]
        public bool IsBrowserExpanded
        {
            get { return this.isBrowserExpanded; }
            set
            {
                this.isBrowserExpanded = value;
                this.NotifyOfPropertyChange(nameof(this.IsBrowserExpanded));
            }
        }

        [ConfigurationProperty("isPropertyExpanded")]
        public bool IsPropertyExpanded
        {
            get { return this.isPropertyExpanded; }
            set
            {
                this.isPropertyExpanded = value;
                this.NotifyOfPropertyChange(nameof(this.IsPropertyExpanded));
            }
        }

        [ConfigurationProperty("browserDistance")]
        public double BrowserDistance
        {
            get { return this.browserDistance; }
            set
            {
                this.browserDistance = value;
                this.NotifyOfPropertyChange(nameof(this.BrowserDistance));
            }
        }

        [ConfigurationProperty("propertyDistance")]
        public double PropertyDistance
        {
            get { return this.propertyDistance; }
            set
            {
                this.propertyDistance = value;
                this.NotifyOfPropertyChange(nameof(this.PropertyDistance));
            }
        }

        public bool IsVisible
        {
            get { return this.isVisible; }
            set
            {
                this.isVisible = value;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        private void CremaAppHost_Opened(object sender, EventArgs e)
        {
            this.configs.Update(this);
            this.Refresh();
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {
            this.configs.Commit(this);
        }

        private void CremaAppHost_Loaded(object sender, EventArgs e)
        {
            this.IsVisible = true;
        }

        private void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            this.IsVisible = false;
        }

        private void Shell_ServiceChanged(object sender, EventArgs e)
        {
            if (this.Shell.SelectedService == this)
            {
                //await this.Dispatcher.InvokeAsync(() => this.Restore(), DispatcherPriority.Background);
            }
        }

        private IShell Shell => this.shell.Value;

        #region IPartImportsSatisfiedNotification

        async void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.Shell.ServiceChanged += Shell_ServiceChanged;
            });
        }

        #endregion
    }
}
