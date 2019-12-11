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

using Ntreev.Crema.Client.Framework;
using Ntreev.Crema.Client.Tables.Properties;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ntreev.Library;

namespace Ntreev.Crema.Client.Tables.Dialogs.ViewModels
{
    public class ViewTemplateViewModel : ModalDialogAppBase
    {
        private readonly Authentication authentication;
        private CremaTemplate template;
        private string tableName;
        private string comment;
        private TagInfo tags;
        private bool ignoreCaseSensitive;

        public ViewTemplateViewModel(Authentication authentication, CremaTemplate template)
        {
            this.DisplayName = Resources.Title_ViewTableTemplate;
            this.authentication = authentication;
            this.template = template;
            this.tableName = this.template.TableName;
            this.comment = this.template.Comment;
            this.tags = this.template.Tags;
            this.ignoreCaseSensitive = this.template.IgnoreCaseSensitive;
        }

        public static Task<ViewTemplateViewModel> CreateInstanceAsync(Authentication authentication, ITableDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.Target is ITable table)
            {
                return table.Dispatcher.InvokeAsync(() =>
                {
                    var dataSet = table.GetDataSet(authentication, -1);
                    var dataTable = dataSet.Tables[table.Name];
                    var template = new CremaTemplate(dataTable);
                    return new ViewTemplateViewModel(authentication, template);
                });
            }
            else
            {
                throw new ArgumentException("Invalid Target of Descriptor", nameof(descriptor));
            }
        }

        public object Source
        {
            get { return this.template; }
        }

        public string TableName
        {
            get { return this.tableName ?? string.Empty; }
        }

        public string Comment
        {
            get { return this.comment ?? string.Empty; }
        }

        public TagInfo Tags
        {
            get { return this.tags; }
        }

        public bool IgnoreCaseSensitive
        {
            get { return this.ignoreCaseSensitive; }
        }
    }
}
