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
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;

namespace Ntreev.Crema.Services.Domains
{
    class DomainCategory : DomainCategoryBase<Domain, DomainCategory, DomainCollection, DomainCategoryCollection, DomainContext>,
        IDomainCategory, IDomainItem
    {
        private IDataBase dataBase;

        public CremaDispatcher Dispatcher => this.Context.Dispatcher;

        public IDataBase DataBase
        {
            get
            {
                if (this.dataBase != null)
                    return this.dataBase;
                if (this.Parent != null)
                    return this.Parent.DataBase;
                return null;
            }
            set
            {
                this.dataBase = value;
            }
        }

        #region IDomainCategory

        IDomainCategory IDomainCategory.Parent
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Parent;
            }
        }

        IContainer<IDomain> IDomainCategory.Domains
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Items;
            }
        }

        IContainer<IDomainCategory> IDomainCategory.Categories
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Categories;
            }
        }

        #endregion

        #region IDomainItem

        IDomainItem IDomainItem.Parent
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                return this.Parent;
            }
        }

        IEnumerable<IDomainItem> IDomainItem.Childs
        {
            get
            {
                this.Dispatcher.VerifyAccess();
                foreach (var item in this.Categories)
                {
                    yield return item;
                }
                foreach (var item in this.Items)
                {
                    yield return item;
                }
            }
        }

        #endregion

        #region IServiceProvider

        object IServiceProvider.GetService(System.Type serviceType)
        {
            if (serviceType == typeof(IDataBase))
            {
                return this.DataBase;
            }
            return (this.Context as IServiceProvider).GetService(serviceType);
        }

        #endregion
    }
}
