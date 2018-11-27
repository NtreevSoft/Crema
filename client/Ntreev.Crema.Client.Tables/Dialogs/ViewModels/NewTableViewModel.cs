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
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Client.Framework;

namespace Ntreev.Crema.Client.Tables.Dialogs.ViewModels
{
    public class NewTableViewModel : TemplateViewModel
    {
        private readonly ITableCategory category;
        private readonly ITableContext tableContext;

        public NewTableViewModel(Authentication authentication, ITableCategory category, ITableTemplate template)
            : base(authentication, template, true)
        {
            this.category = category;
            this.category.Dispatcher.VerifyAccess();
            this.tableContext = category.GetService(typeof(ITableContext)) as ITableContext;
            this.DisplayName = Resources.Title_NewTable;
        }

        public static Task<NewTableViewModel> CreateInstanceAsync(Authentication authentication, ITableCategoryDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITableCategory category)
            {
                return category.Dispatcher.InvokeAsync(() =>
                {
                    var template = category.NewTable(authentication);
                    return new NewTableViewModel(authentication, category, template);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        protected async override void Verify(Action<bool> isVerify)
        {
            var result = await this.category.Dispatcher.InvokeAsync(() =>
            {
                if (this.TableName == string.Empty)
                    return false;
                if (NameValidator.VerifyName(this.TableName) == false)
                    return false;
                if (this.category.Categories.ContainsKey(this.TableName))
                    return false;

                return this.category.Dispatcher.Invoke(() => this.tableContext.Tables.Contains(this.TableName) == false);
            });
            isVerify(result);
        }
    }
}