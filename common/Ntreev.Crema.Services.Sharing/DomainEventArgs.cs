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
    public class DomainEventArgs : EventArgs
    {
        private readonly SignatureDate signatureDate;
        private readonly IDomain domain;
        private readonly DomainInfo domainInfo;
        private readonly DomainState domainState;

        public DomainEventArgs(Authentication authentication, IDomain domain)
        {
            this.domain = domain;
            this.domainInfo = domain.DomainInfo;
            this.domainState = domain.DomainState;
            this.signatureDate = authentication.SignatureDate;
            this.UserToken = authentication.Token;
        }

        public IDomain Domain
        {
            get { return this.domain; }
        }

        public DomainInfo DomainInfo
        {
            get { return this.domainInfo; }
        }

        public DomainState DomainState
        {
            get { return this.domainState; }
        }

        public string UserID
        {
            get { return this.signatureDate.ID; }
        }

        public Guid UserToken { get; private set; }


        public DateTime DateTime
        {
            get { return this.signatureDate.DateTime; }
        }

        public SignatureDate SignatureDate
        {
            get { return this.signatureDate; }
        }
    }
}
