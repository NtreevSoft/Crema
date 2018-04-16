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

using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Threading.Tasks;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System.Windows;
using System.ComponentModel;
using Ntreev.Crema.Client.Types.Properties;

namespace Ntreev.Crema.Client.Types.PropertyItems.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [RequiredAuthority(Authority.Guest)]
    [Dependency(typeof(TypeInfoViewModel))]
    [ParentType(typeof(PropertyService))]
    class AccessInfoViewModel : PropertyItemBase
    {
        private IAccessibleDescriptor descriptor;
        private AccessInfo accessInfo;

        public AccessInfoViewModel()
        {
            this.DisplayName = Resources.Title_AccessInfo;
        }

        public override bool CanSupport(object obj)
        {
            return obj is IAccessibleDescriptor;
        }

        public override void SelectObject(object obj)
        {
            this.Detach();
            this.descriptor = obj as IAccessibleDescriptor;
            this.Attach();
        }

        public override object SelectedObject
        {
            get { return this.descriptor; }
        }

        public override bool IsVisible
        {
            get
            {
                if (this.descriptor == null)
                    return false;
                return this.AccessInfo.UserID != string.Empty;
            }
        }

        public AccessInfo AccessInfo
        {
            get { return this.accessInfo; }
            private set
            {
                this.accessInfo = value;
                this.NotifyOfPropertyChange(nameof(this.AccessInfo));
            }
        }

        private void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IAccessibleDescriptor.AccessInfo) || e.PropertyName == string.Empty)
            {
                this.AccessInfo = this.descriptor.AccessInfo;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        private void Attach()
        {
            if (this.descriptor != null)
            {
                if (this.descriptor is INotifyPropertyChanged)
                {
                    (this.descriptor as INotifyPropertyChanged).PropertyChanged += Descriptor_PropertyChanged;
                }
                this.AccessInfo = this.descriptor.AccessInfo;
            }
            this.NotifyOfPropertyChange(nameof(this.IsVisible));
            this.NotifyOfPropertyChange(nameof(this.SelectedObject));
        }

        private void Detach()
        {
            if (this.descriptor != null)
            {
                if (this.descriptor is INotifyPropertyChanged)
                {
                    (this.descriptor as INotifyPropertyChanged).PropertyChanged -= Descriptor_PropertyChanged;
                }
            }
        }
    }
}
