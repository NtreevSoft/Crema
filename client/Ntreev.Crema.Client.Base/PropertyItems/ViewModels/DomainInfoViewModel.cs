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

using Ntreev.Crema.Client.Base.Properties;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Base.PropertyItems.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [RequiredAuthority(Authority.Guest)]
    [ParentType(typeof(PropertyService))]
    class DomainInfoViewModel : PropertyItemBase
    {
        private IDomainDescriptor descriptor;
        private DomainInfo domainInfo;
        private string dataBaseName;

        [Import]
        private ICremaAppHost cremaAppHost = null;

        public DomainInfoViewModel()
        {
            this.DisplayName = Resources.Title_DomainInfo;
        }

        public override bool CanSupport(object obj)
        {
            return obj is IDomainDescriptor;
        }

        public override void SelectObject(object obj)
        {
            this.descriptor = obj as IDomainDescriptor;

            if (this.descriptor != null)
            {
                this.DomainInfo = this.descriptor.DomainInfo;
            }

            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
        }

        public override bool IsVisible => this.descriptor != null;

        public override object SelectedObject => this.descriptor;

        public DomainInfo DomainInfo
        {
            get => this.domainInfo;
            set
            {
                this.domainInfo = value;
                if (this.cremaAppHost.DataBases.FirstOrDefault(item => item.DataBaseInfo.ID == this.domainInfo.DataBaseID) is IDataBaseDescriptor descriptor)
                {
                    this.dataBaseName = descriptor.Name;
                }
                else
                {
                    this.dataBaseName = null;
                }
                this.NotifyOfPropertyChange(nameof(this.DomainInfo));
                this.NotifyOfPropertyChange(nameof(this.DataBaseName));
            }
        }

        public string DataBaseName => this.dataBaseName ?? string.Empty;
    }
}
