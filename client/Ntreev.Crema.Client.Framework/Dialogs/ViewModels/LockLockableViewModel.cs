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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Crema.Services;
using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Framework.Properties;
using Ntreev.ModernUI.Framework;
using System.Windows.Threading;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Client.Framework.Dialogs.Views;

namespace Ntreev.Crema.Client.Framework.Dialogs.ViewModels
{
    public class LockLockableViewModel : LockAsyncViewModel
    {
        private readonly Authentication authentication;
        private readonly ILockable lockable;

        private LockLockableViewModel(Authentication authentication, ILockable lockable)
        {
            this.authentication = authentication;
            this.lockable = lockable;
            if (this.lockable is IDispatcherObject dispatcherObject)
            {
                dispatcherObject.Dispatcher.VerifyAccess();
            }
        }

        public static async Task<LockLockableViewModel> CreateInstanceAsync(Authentication authentication, ILockableDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ILockable lockable)
            {
                var dispatcher = lockable is IDispatcherObject dispatcherObject ? dispatcherObject.Dispatcher : Application.Current.Dispatcher;
                return await dispatcher.InvokeAsync(() =>
                {
                    return new LockLockableViewModel(authentication, lockable);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        protected override Task LockAsync(string comment)
        {
            var dispatcher = lockable is IDispatcherObject dispatcherObject ? dispatcherObject.Dispatcher : Application.Current.Dispatcher;
            return dispatcher.InvokeAsync(() => this.lockable.Lock(this.authentication, comment)).Task;
        }

        protected override void VerifyLock(string comment, Action<bool> isValid)
        {
            isValid(true);
        }
    }
}
