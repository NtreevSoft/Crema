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

using Ntreev.Crema.Client.Base.Services.ViewModels;
using Ntreev.Crema.Client.Framework;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Base
{
    //[Export(typeof(IContentService))]
    [Export(typeof(DataBaseServiceViewModel))]
    class DataBaseServiceViewModel : ScreenBase
    {
        private readonly ICremaAppHost cremaAppHost;

        private bool isBrowserExpanded = true;
        private bool isPropertyExpanded = true;

        private double browserDistance = 250.0;
        private double propertyDistance = 250.0;

        [Import]
        private IAppConfiguration configs = null;

        [Import]
        private BrowserService browserService = null;
        [Import]
        private DataBaseListViewModel contentService = null;
        [Import]
        private PropertyService propertyService = null;

        [ImportingConstructor]
        public DataBaseServiceViewModel(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Opened += CremaAppHost_Opened;
            this.cremaAppHost.Closed += CremaAppHost_Closed;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
        }

        public override string ToString()
        {
            return "데이터 베이스";
        }

        public IBrowserService BrowserService
        {
            get { return this.browserService; }
        }

        public DataBaseListViewModel ContentService
        {
            get { return this.contentService; }
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

        public bool IsVisible => this.cremaAppHost.IsLoaded;

        private void CremaAppHost_Opened(object sender, EventArgs e)
        {
            this.configs.Update(this);
            this.NotifyOfPropertyChange(nameof(this.IsVisible));
        }

        private void CremaAppHost_Closed(object sender, EventArgs e)
        {
            this.configs.Commit(this);
            this.NotifyOfPropertyChange(nameof(this.IsVisible));
        }

        private void CremaAppHost_Loaded(object sender, EventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(this.IsVisible));
        }

        private void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(this.IsVisible));
        }
    }
}
