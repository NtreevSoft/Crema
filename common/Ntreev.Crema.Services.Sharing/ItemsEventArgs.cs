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
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public class ItemsEventArgs<T> : EventArgs
    {
        private readonly SignatureDate signatureDate;
        private readonly T[] items;
        private readonly object metaData;

        public ItemsEventArgs(Authentication authentication, IEnumerable<T> items)
            : this(authentication, items, null)
        {

        }

        public ItemsEventArgs(Authentication authentication, IEnumerable<T> items, object metaData)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (authentication.SignatureDate.ID == string.Empty)
                throw new ArgumentException(ServiceModel.Properties.Resources.Exception_AuthenticationDoesNotSigned, nameof(authentication));
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            this.signatureDate = authentication.SignatureDate;
            this.items = items.ToArray();
            this.metaData = metaData;
        }

        public string UserID
        {
            get { return this.signatureDate.ID; }
        }

        public SignatureDate SignatureDate
        {
            get { return this.signatureDate; }
        }

        public DateTime DateTime
        {
            get { return this.signatureDate.DateTime; }
        }

        public T[] Items
        {
            get { return this.items; }
        }

        public object MetaData
        {
            get { return this.metaData; }
        }
    }
}
