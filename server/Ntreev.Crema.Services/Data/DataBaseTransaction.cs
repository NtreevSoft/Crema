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

using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    class DataBaseTransaction : ITransaction
    {
        private readonly Authentication authentication;
        private readonly DataBase dataBase;
        private readonly DataBaseRepositoryHost repository;
        private readonly TypeInfo[] typeInfos;
        private readonly TableInfo[] tableInfos;
        private readonly Guid transactionID;

        public DataBaseTransaction(Authentication authentication, DataBase dataBase, DataBaseRepositoryHost repository)
            : this(authentication, dataBase, repository, Guid.NewGuid())
        {

        }

        public DataBaseTransaction(Authentication authentication, DataBase dataBase, DataBaseRepositoryHost repository, Guid transactionID)
        {
            if (transactionID == Guid.Empty)
                throw new ArgumentException("transactionID is not valid", nameof(transactionID));
            this.authentication = authentication;
            this.dataBase = dataBase;
            this.repository = repository;
            this.typeInfos = dataBase.TypeContext.Types.Select((Type item) => item.TypeInfo).ToArray();
            this.tableInfos = dataBase.TableContext.Tables.Select((Table item) => item.TableInfo).ToArray();
            this.transactionID = transactionID;
            this.repository.BeginTransaction(dataBase.Name);
            this.authentication.Expired += Authentication_Expired;
            this.dataBase.ExtendedProperties[this.transactionID] = this;
        }

        public void Commit(Authentication authentication)
        {
            this.dataBase.VerifyAccess(authentication);
            this.Sign(authentication);
            this.repository.EndTransaction();
            this.dataBase.ExtendedProperties.Remove(this.transactionID);
            this.authentication.Expired -= Authentication_Expired;
            if (this.dataBase.LockInfo.Comment == $"{this.transactionID}" && this.dataBase.IsLocked == true)
            {
                this.dataBase.Unlock(authentication);
            }
        }

        public void Rollback(Authentication authentication)
        {
            this.dataBase.VerifyAccess(authentication);
            this.Sign(authentication);
            this.repository.CancelTransaction();
            this.dataBase.ClearDomains(authentication);
            this.dataBase.Reset(authentication, this.typeInfos, this.tableInfos);
            this.dataBase.ExtendedProperties.Remove(this.transactionID);
            this.authentication.Expired -= Authentication_Expired;
            if (this.dataBase.LockInfo.Comment == $"{this.transactionID}" && this.dataBase.IsLocked == true)
            {
                this.dataBase.Unlock(authentication);
            }
        }

        public CremaDispatcher Dispatcher => this.dataBase.Dispatcher;

        public Guid ID => this.transactionID;

        private async void Authentication_Expired(object sender, EventArgs e)
        {
            this.authentication.Expired -= Authentication_Expired;
            await this.dataBase.Dispatcher.InvokeAsync(() =>
            {
                this.Rollback(this.authentication);
            });
        }

        private void Sign(Authentication authentication)
        {
            authentication.Sign();
        }
    }
}
