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

using Ntreev.Crema.Services;
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

namespace Ntreev.Crema.Client.Framework
{
    public abstract class DescriptorListItemBase<T> : ListBoxItemViewModel where T : DescriptorBase
    {
        protected readonly Authentication authentication;
        protected readonly T descriptor;
        private readonly DescriptorPropertyNotifier notifier;
        private bool isDescriptorDisposed;

        protected DescriptorListItemBase(Authentication authentication, T descriptor, object owner)
        {
            this.authentication = authentication;
            this.descriptor = descriptor;
            this.descriptor.Host = this;
            this.descriptor.PropertyChanged += Descriptor_PropertyChanged;
            this.descriptor.Disposed += Descriptor_Disposed;
            this.notifier = new DescriptorPropertyNotifier(this, item => this.NotifyOfPropertyChange(item));
            this.Dispatcher.InvokeAsync(this.notifier.Save);
            this.Target = descriptor.Target;
            this.Owner = owner;
        }

        public void Dispose()
        {
            this.OnDisposed(EventArgs.Empty);
        }

        public event EventHandler Disposed;

        public T Descriptor => this.descriptor;

        protected virtual void OnDisposed(EventArgs e)
        {
            if (this.isDescriptorDisposed == false)
            {
                this.descriptor.PropertyChanged -= Descriptor_PropertyChanged;
                this.descriptor.Disposed -= Descriptor_Disposed;
                this.descriptor.Dispose();
            }
            this.notifier?.Dispose();
            this.isDescriptorDisposed = true;
            this.Disposed?.Invoke(this, e);
        }

        private async void Descriptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.isDescriptorDisposed == true)
                return;
            await this.notifier.RefreshAsync();
            this.NotifyOfPropertyChange(e.PropertyName);
        }

        private void Descriptor_Disposed(object sender, EventArgs e)
        {
            this.isDescriptorDisposed = true;
            this.Dispose();
        }
    }
}
