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
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.ServiceModel.Properties;

namespace Ntreev.Crema.ServiceModel
{
    internal abstract class DomainCategoryBase<_I, _C, _IC, _CC, _CT> : CategoryBase<_I, _C, _IC, _CC, _CT>
        where _I : DomainBase<_I, _C, _IC, _CC, _CT>
        where _C : DomainCategoryBase<_I, _C, _IC, _CC, _CT>, new()
        where _IC : ItemContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CC : CategoryContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CT : ItemContext<_I, _C, _IC, _CC, _CT>
    {
        public DomainCategoryBase()
        {

        }

        public IContainer<_I> Domains
        {
            get { return this.Items; }
        }
    }
}