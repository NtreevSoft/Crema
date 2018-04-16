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

using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Framework.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.Client.Framework.MenuItems.Domains
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(DomainTreeItemBase))]
    [ParentType(typeof(DomainListItemBase))]
    class CancelDomainEditMenuItem : MenuItemBase
    {
        [Import]
        private Authenticator authenticator = null;

        public CancelDomainEditMenuItem()
        {
            this.DisplayName = Resources.MenuItem_CancelEdit;
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (parameter is IDomainDescriptor descriptor && descriptor.Target is IDomain && this.authenticator.Authority == Authority.Admin)
            {
                return descriptor.IsActivated;
            }
            return false;
        }

        protected async override void OnExecute(object parameter)
        {
            if (parameter is IDomainDescriptor descriptor && descriptor.Target is IDomain domain && this.authenticator.Authority == Authority.Admin)
            {
                if (AppMessageBox.ShowProceed("편집을 취소합니다. 저장되지 않는 항목은 사라집니다. 계속하시겠습니까?") == false)
                    return;

                try
                {
                    await domain.Dispatcher.InvokeAsync(() => domain.Delete(this.authenticator, true));
                }
                catch (Exception e)
                {
                    AppMessageBox.ShowError(e);
                }
            }
        }
    }
}
