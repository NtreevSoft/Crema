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
using Ntreev.Crema.Services.DataBaseCollectionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    class DataBaseTransaction : ITransaction
    {
        private Authentication authentication;
        private readonly DataBase dataBase;
        private readonly IDataBaseCollectionService service;
        private readonly Guid transactionID;

        public DataBaseTransaction(Authentication authentication, DataBase dataBase, IDataBaseCollectionService service, Guid transactionID)
        {
            this.authentication = authentication;
            this.dataBase = dataBase;
            this.service = service;
            this.transactionID = transactionID;
            this.dataBase.ExtendedProperties[this.transactionID] = this;
        }

        public void Commit(Authentication authentication)
        {
            this.dataBase.VerifyAccess(authentication);
            if (this.authentication == null)
                throw new InvalidOperationException();
            var result = this.service.EndTransaction(this.dataBase.Name, $"{this.transactionID}");
            this.Sign(authentication, result);
            this.dataBase.ExtendedProperties.Remove(this.transactionID);
        }

        public void Rollback(Authentication authentication)
        {
            this.dataBase.VerifyAccess(authentication);
            if (this.authentication == null)
                throw new InvalidOperationException();
            var result = this.service.EndTransaction(this.dataBase.Name, $"{this.transactionID}");
            this.Sign(authentication, result);
            this.dataBase.ExtendedProperties.Remove(this.transactionID);
        }

        public void Dispose()
        {
            this.authentication = null;
        }

        public CremaDispatcher Dispatcher => this.dataBase.Dispatcher;

        public Guid ID => this.transactionID;

        private void Sign(Authentication authentication, ResultBase result)
        {
            result.Validate(authentication);
        }

        private void Sign<T>(Authentication authentication, ResultBase<T> result)
        {
            result.Validate(authentication);
        }
    }
}
