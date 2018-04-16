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
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Ntreev.Library.IO;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.ObjectModel;

namespace Ntreev.Crema.Commands.Consoles
{
    [Export(typeof(IConsoleDrive))]
    public sealed class DomainsConsoleDrive : ConsoleDriveBase
    {
        [Import]
        private Lazy<ICremaHost> cremaHost = null;

        internal DomainsConsoleDrive()
            : base("domains")
        {

        }

        public override object GetObject(Authentication authentication, string path)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate(Authentication authentication, string path, string name)
        {
            throw new NotImplementedException();
        }

        protected override void OnMove(Authentication authentication, string path, string newPath)
        {
            throw new NotImplementedException();
        }

        protected override void OnDelete(Authentication authentication, string path)
        {
            throw new NotImplementedException();
        }

        protected override void OnSetPath(Authentication authentication, string path)
        {
            
        }

        public override string[] GetPaths()
        {
            return this.DomainContext.Dispatcher.Invoke(() => this.DomainContext.Select(item => item.Path).ToArray());
        }

        private IDomainItem GetObject(string path)
        {
            if (NameValidator.VerifyCategoryPath(path) == true)
            {
                return this.DomainContext[path];
            }
            else
            {
                var itemName = new ItemName(path);
                var category = this.DomainContext.Categories[itemName.CategoryPath];
                if (category.Categories.ContainsKey(itemName.Name) == true)
                    return category.Categories[itemName.Name] as IDomainItem;
                if (category.Domains.ContainsKey(itemName.Name) == true)
                    return category.Domains[itemName.Name] as IDomainItem;
                return null;
            }
        }

        private IDomainContext DomainContext => this.cremaHost.Value.GetService(typeof(IDomainContext)) as IDomainContext;
    }
}
