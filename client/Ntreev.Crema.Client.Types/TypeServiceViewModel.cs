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
using Ntreev.Crema.Client.Types.Dialogs.ViewModels;
using Ntreev.Crema.Client.Types.Documents.ViewModels;
using Ntreev.Crema.Client.Types.Properties;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Ntreev.Library;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Ntreev.Crema.Services.Domains;

namespace Ntreev.Crema.Client.Types
{
    [Export(typeof(IContentService))]
    [InheritedExport(typeof(TypeServiceViewModel))]
    class TypeServiceViewModel : ScreenBase, IContentService
    {
        private readonly ICremaAppHost cremaAppHost;

        [Import]
        private IBrowserService browserService = null;
        [Import]
        private TypeDocumentServiceViewModel documentService = null;
        [Import]
        private IPropertyService propertyService = null;
        [Import]
        private Lazy<IShell> shell = null;

        private bool isBrowserExpanded = true;
        private bool isPropertyExpanded = true;

        private double browserDistance = 250.0;
        private double propertyDistance = 250.0;

        private bool isFirst;
        private bool isVisible;

        [Import]
        private Authenticator authenticator = null;

        [Import]
        private IAppConfiguration configs = null;

        [ImportingConstructor]
        public TypeServiceViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += CremaAppHost_Opened;
            this.cremaAppHost.Closed += CremaAppHost_Closed;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
            this.DisplayName = Resources.Title_Type;
        }

        public override string ToString()
        {
            return Resources.Title_Type;
        }

        public void Dispose()
        {
            this.documentService.TryClose();
        }

        public IBrowserService BrowserService
        {
            get { return this.browserService; }
        }

        public TypeDocumentServiceViewModel DocumentService
        {
            get { return this.documentService; }
        }

        public IPropertyService PropertyService
        {
            get { return this.propertyService; }
        }

        [ConfigurationProperty("isBrowserExpanded")]
        [DefaultValue(true)]
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
        [DefaultValue(true)]
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
        [DefaultValue(250.0)]
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
        [DefaultValue(250.0)]
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

        //private async void CremaHost_Opened(object sender, EventArgs e)
        //{
        //    await this.Dispatcher.InvokeAsync(() =>
        //    {
        //        this.shell.Value.ServiceChanged += Shell_ServiceChanged;
        //        this.configs.Update(this);
        //        this.Refresh();
        //    });
        //}

        //private async void CremaHost_Closing(object sender, EventArgs e)
        //{
        //    if (this.cremaHost.Address == null)
        //        return;
        //    await this.Dispatcher.InvokeAsync(() =>
        //    {
        //        this.configs.Commit(this);
        //    });
        //}

        private async void CremaHost_Closed(object sender, EventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.shell.Value.ServiceChanged -= Shell_ServiceChanged;
            });
        }

        private void CremaAppHost_Opened(object sender, EventArgs e)
        {
            this.shell.Value.ServiceChanged += Shell_ServiceChanged;
            this.configs.Update(this);
            this.Refresh();
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {
            this.configs.Commit(this);
            this.Shell.ServiceChanged -= Shell_ServiceChanged;
        }


        private void CremaAppHost_Loaded(object sender, EventArgs e)
        {
            this.isFirst = false;
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
                if (this.cremaAppHost.GetService(typeof(IDataBase)) is IDataBase dataBase)
                {
                    this.Dispatcher.InvokeAsync(() => this.Restore(dataBase), DispatcherPriority.Background);
                }
            }
        }

        private async void Restore(IDataBase dataBase)
        {
            if (this.isFirst == true)
                return;

            this.isFirst = true;

            var domainContext = dataBase.GetService(typeof(IDomainContext)) as IDomainContext;
            var items = await domainContext.Dispatcher.InvokeAsync(() =>
            {
                var restoreList = new List<System.Action>();
                var domains = domainContext.Domains.Where(item => item.DataBaseID == dataBase.ID).ToArray();

                foreach (var item in domains)
                {
                    var users = item.Users.Select(o => o.DomainUserInfo.UserID);
                    if (users.Contains(this.authenticator.ID) == false)
                        continue;

                    var itemPath = item.DomainInfo.ItemPath;
                    var itemType = item.DomainInfo.ItemType;

                    if (item.Host is ITypeTemplate template)
                    {
                        if (itemType == "NewTypeTemplate")
                        {
                            var category = dataBase.TypeContext[itemPath] as ITypeCategory;
                            var dialog = new NewTypeViewModel(this.authenticator, category, template);
                            restoreList.Add(new System.Action(() => {
                                if (item.Host is IDomainHost host)
                                {
                                    host.OnRestoredEvent((Domain)item);
                                }

                                dialog.ShowDialog(); }));
                        }
                        else if (itemType == "TypeTemplate")
                        {
                            var type = dataBase.TypeContext[itemPath] as IType;
                            var dialog = new EditTemplateViewModel(this.authenticator, type, template);
                            restoreList.Add(new System.Action(() => {
                                if (item.Host is IDomainHost host)
                                {
                                    host.OnRestoredEvent((Domain)item);
                                }

                                dialog.ShowDialog();
                            }));
                        }
                    }
                }
                return restoreList.ToArray();
            });

            foreach (var item in items)
            {
                if (this.cremaAppHost.IsLoaded == false)
                    return;

                item();
            }
        }

        private IShell Shell => this.shell.Value;
    }
}
