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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TableTemplate
{
    [Export(typeof(IScriptMethod))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Category(nameof(TableTemplate))]
    class BeginTableCreateMethod : DataBaseScriptMethodBase
    {
        [ImportingConstructor]
        public BeginTableCreateMethod(ICremaHost cremaHost)
            : base(cremaHost)
        {

        }

        protected override Delegate CreateDelegate()
        {
            return new Func<string, string, string>(this.BeginTableCreate);
        }

        [ReturnParameterName("domainID")]
        private string BeginTableCreate(string dataBaseName, string parentPath)
        {
            var dataBase = this.GetDataBase(dataBaseName);
            return dataBase.Dispatcher.Invoke(() =>
            {
                if (NameValidator.VerifyCategoryPath(parentPath) == true)
                {
                    var category = dataBase.TableContext.Categories[parentPath];
                    if (category == null)
                        throw new CategoryNotFoundException(parentPath);
                    var authentication = this.Context.GetAuthentication(this);
                    var template = category.NewTable(authentication);
                    return $"{template.Domain.ID}";
                }
                else if (NameValidator.VerifyItemPath(parentPath) == true)
                {
                    var table = dataBase.TableContext[parentPath] as ITable;
                    if (table == null)
                        throw new CategoryNotFoundException(parentPath);
                    var authentication = this.Context.GetAuthentication(this);
                    var template = table.NewTable(authentication);
                    return $"{template.Domain.ID}";
                }
                else
                {
                    var table = dataBase.TableContext.Tables[parentPath] as ITable;
                    if (table == null)
                        throw new CategoryNotFoundException(parentPath);
                    var authentication = this.Context.GetAuthentication(this);
                    var template = table.NewTable(authentication);
                    return $"{template.Domain.ID}";
                }
            });
        }
    }
}
