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

using Ntreev.Crema.Comparer.Properties;
using Ntreev.Crema.Comparer.Types.Views;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Diff;
using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Comparer.Types.ViewModels
{
    [Export(typeof(IPropertyItem))]
    [ParentType(typeof(TypePropertyViewModel))]
    [View(typeof(TypeInfoView))]
    [Order(1)]
    class TypeInfoViewModel2 : TypeInfoViewModel
    {
        protected override string GetHeader(TypeTreeViewItemViewModel item)
        {
            return item.Header2;
        }

        protected override TypeInfo? GetTypeInfo(TypeTreeViewItemViewModel item)
        {
            if (item.DiffState == DiffState.Deleted && item.IsResolved == false)
                return null;
            return item.Source2.TypeInfo;
        }
    }
}