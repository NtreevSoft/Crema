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

using System.Collections.ObjectModel;
using System.Linq;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System;
using Ntreev.Crema.Data;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Ntreev.ModernUI.Framework;
using Ntreev.Crema.Client.Tables.Properties;
using Ntreev.Crema.Client.Framework;

namespace Ntreev.Crema.Client.Tables.Dialogs.ViewModels
{
    public class NewChildTableViewModel : TemplateViewModel
    {
        private readonly ITable parent;

        public NewChildTableViewModel(Authentication authentication, ITable parent, ITableTemplate template)
            : base(authentication, template, true)
        {
            this.parent = parent;
            this.parent.Dispatcher.VerifyAccess();
            this.DisplayName = Resources.Title_NewChildTable;
        }

        public static async Task<NewChildTableViewModel> CreateInstanceAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITable table)
            {
                try
                {
                    return await table.Dispatcher.InvokeAsync(() =>
                    {
                        var template = table.NewTable(authentication);
                        return new NewChildTableViewModel(authentication, table, template);
                    });
                }
                catch (Exception e)
                {
                    AppMessageBox.ShowError(e);
                    return null;
                }
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        protected async override void Verify(Action<bool> isVerify)
        {
            var result = await this.parent.Dispatcher.InvokeAsync(() =>
            {
                return this.parent.Childs.Any(item => item.Name == this.TableName) == false;
            });
            isVerify(result);
        }
    }
}