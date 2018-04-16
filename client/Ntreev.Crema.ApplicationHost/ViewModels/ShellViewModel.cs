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

using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using System.Windows;
using Ntreev.Crema.ServiceModel;
using System.ComponentModel;
using System.Xml.Serialization;
using System;
using System.Threading.Tasks;
using System.Collections;
using Ntreev.Library;
using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using System.Collections.ObjectModel;
using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework;
using System.Diagnostics;

#pragma warning disable 0649
namespace Ntreev.Crema.ApplicationHost.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ScreenBase, IShell
    {
        private readonly ICremaHost cremaHost;
        private readonly ICremaAppHost cremaAppHost;
        [ImportMany]
        private IEnumerable<IContentService> contentServices;
        [ImportMany]
        private IEnumerable<IBrowserItem> browsers;
        [Import]
        private IStatusBarService statusbarService;
        [Import]
        private Lazy<IMenuService> menuService;
        [Import]
        private IServiceProvider serviceProvider = null;
        private object selectedService;

        private string selectedServiceType;

        [ImportingConstructor]
        public ShellViewModel(ICremaHost cremaHost, ICremaAppHost cremaAppHost)
        {
            this.cremaHost = cremaHost;
            this.cremaHost.Opened += (s, e) =>
            {
                if (this.cremaHost.Configs.TryParse<string>(this.GetType(), "selectedServiceType", out var selectedServiceType) == true)
                {
                    this.selectedServiceType = selectedServiceType;
                }
            };
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += CremaAppHost_Opened;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
            this.cremaAppHost.Closed += CremaAppHost_Closed;
        }

        public void Close()
        {
            this.TryClose();
        }

        public double Left
        {
            get { return Ntreev.Crema.ApplicationHost.Properties.Settings.Default.X; }
            set
            {
                if (this.WindowState == WindowState.Maximized)
                    return;

                Ntreev.Crema.ApplicationHost.Properties.Settings.Default.X = value;
            }
        }

        public double Top
        {
            get { return Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Y; }
            set
            {
                if (this.WindowState == WindowState.Maximized)
                    return;

                Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Y = value;
            }
        }

        public double Width
        {
            get { return Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Width; }
            set
            {
                if (this.WindowState == WindowState.Maximized)
                    return;

                Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Width = value;
            }
        }

        public double Height
        {
            get { return Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Height; }
            set
            {
                if (this.WindowState == WindowState.Maximized)
                    return;

                Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Height = value;
            }
        }

        public WindowState WindowState
        {
            get { return Ntreev.Crema.ApplicationHost.Properties.Settings.Default.WindowState; }
            set
            {
                Ntreev.Crema.ApplicationHost.Properties.Settings.Default.WindowState = value;
            }
        }

        public string Title
        {
            get
            {
                string productName = AppUtility.ProductName;

                if (cremaHost.IsOpened == true)
                    return $"{productName} - {this.cremaAppHost.DataBaseName} ({cremaHost.Address} - {cremaHost.UserID})";
                else
                    return productName;
            }
        }

        public IEnumerable Browsers
        {
            get { return this.browsers; }
        }

        public IStatusBarService StatusBarService
        {
            get { return this.statusbarService; }
        }

        public IMenuService MenuService
        {
            get { return this.menuService.Value; }
        }

        public IEnumerable Services
        {
            get
            {
                //yield return this.cremaAppHost;
                foreach (var item in EnumerableUtility.OrderByAttribute(this.contentServices))
                {
                    yield return item;
                }
            }
        }

        public object SelectedService
        {
            get { return this.selectedService; }
            set
            {
                if (this.selectedService == value)
                    return;

                this.selectedService = value;
                this.NotifyOfPropertyChange(nameof(this.SelectedService));

                if (this.selectedService is IContentService && this.cremaAppHost.IsLoaded == true)
                {
                    var contentService = this.selectedService as IContentService;
                    //this.ActivateItem(contentService);
                    this.selectedServiceType = contentService.GetType().FullName;
                    this.cremaHost.Configs[this.GetType(), "selectedServiceType"] = this.selectedServiceType;
                }

                this.OnServiceChanged(EventArgs.Empty);
            }
        }

        public object Content
        {
            get { return this.cremaAppHost; }
        }

        public virtual IEnumerable<IToolBarItem> ToolBarItems
        {
            get
            {
                if (this.serviceProvider == null)
                    return Enumerable.Empty<IToolBarItem>();
                return ToolBarItemUtility.GetToolBarItems(this, this.serviceProvider);
            }
        }

        protected async override Task CloseAsync()
        {
            var closed = false;
            if (this.cremaAppHost.IsOpened == true)
            {
                this.cremaAppHost.Closed += (s, e) => closed = true;
                this.cremaAppHost.Logout();
                while (closed == false)
                {
                    await Task.Delay(1);
                }
            }
        }

        public event EventHandler Loaded;

        public event EventHandler Closed;

        public event EventHandler ServiceChanged;

        protected virtual void OnLoaded(EventArgs e)
        {
            this.Loaded?.Invoke(this, e);
        }

        protected virtual void OnClosed(EventArgs e)
        {
            this.Closed?.Invoke(this, e);
        }

        protected virtual void OnServiceChanged(EventArgs e)
        {
            this.ServiceChanged?.Invoke(this, e);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.OnLoaded(EventArgs.Empty);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.selectedService = this.cremaAppHost;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            Ntreev.Crema.ApplicationHost.Properties.Settings.Default.ThemeColor = this.cremaAppHost.ThemeColor;
            Ntreev.Crema.ApplicationHost.Properties.Settings.Default.Save();
            this.OnClosed(EventArgs.Empty);
        }

        private void CremaAppHost_Opened(object sender, EventArgs e)
        {

        }

        private void CremaAppHost_Loaded(object sender, EventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(this.Title));
            var selectedService = this.contentServices.FirstOrDefault(item => item.GetType().FullName == this.selectedServiceType);
            if (selectedService == null)
            {
                selectedService = EnumerableUtility.OrderByAttribute(this.contentServices).First();
            }
            this.SelectedService = selectedService;
        }

        private void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(this.Title));
            this.SelectedService = null;
            this.SelectedService = this.cremaAppHost;
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {
            this.SelectedService = this.cremaAppHost;
        }
    }
}
