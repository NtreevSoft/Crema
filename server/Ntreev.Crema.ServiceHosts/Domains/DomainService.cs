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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Ntreev.Crema.Services.Properties;
using System.Windows.Threading;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Domains
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class DomainService : CremaServiceItemBase<IDomainEventCallback>, IDomainService, ICremaServiceItem
    {
        private readonly ICremaHost cremaHost;
        private readonly ILogService logService;
        private readonly IDomainContext domainContext;
        private readonly IDataBaseCollection dataBases;
        private readonly IUserContext userContext;

        private Authentication authentication;
        private readonly HashSet<Guid> domains = new HashSet<Guid>();
        private readonly HashSet<Guid> resettings = new HashSet<Guid>();

        public DomainService(ICremaHost cremaHost)
            : base(cremaHost.GetService(typeof(ILogService)) as ILogService)
        {
            this.cremaHost = cremaHost;
            this.logService = cremaHost.GetService(typeof(ILogService)) as ILogService;
            this.userContext = cremaHost.GetService(typeof(IUserContext)) as IUserContext;
            this.domainContext = cremaHost.GetService(typeof(IDomainContext)) as IDomainContext;
            this.dataBases = cremaHost.GetService(typeof(IDataBaseCollection)) as IDataBaseCollection;

            this.logService.Debug($"{nameof(DomainService)} Constructor");
        }

        public ResultBase<DomainContextMetaData> Subscribe(Guid authenticationToken)
        {
            var result = new ResultBase<DomainContextMetaData>();
            try
            {
                this.userContext.Dispatcher.Invoke(() =>
                {
                    this.authentication = this.userContext.Authenticate(authenticationToken);
                    this.authentication.AddRef(this);
                    this.OwnerID = this.authentication.ID;
                    this.userContext.UsersLoggedOut += UserContext_UsersLoggedOut;
                });
                result.Value = this.domainContext.Dispatcher.Invoke(() =>
                {
                    this.AttachEventHandlers();
                    this.logService.Debug($"[{this.OwnerID}] {nameof(DomainService)} {nameof(Subscribe)}");

                    var metaData = this.domainContext.GetMetaData(this.authentication);
                    foreach (var item in metaData.Domains)
                    {
                        if (item.Users.Any(i => i.DomainUserInfo.UserID == this.OwnerID) == true)
                        {
                            this.domains.Add(item.DomainID);
                        }
                    }
                    return metaData;
                });
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        public ResultBase Unsubscribe()
        {
            var result = new ResultBase();
            try
            {
                this.domainContext.Dispatcher.Invoke(() =>
                {
                    this.DetachEventHandlers();
                });
                this.userContext.Dispatcher.Invoke(() =>
                {
                    this.userContext.UsersLoggedOut -= UserContext_UsersLoggedOut;
                    this.authentication.RemoveRef(this);
                    this.authentication = null;
                });
                result.SignatureDate = new SignatureDateProvider(this.OwnerID).Provide();
                this.logService.Debug($"[{this.OwnerID}] {nameof(DomainService)} {nameof(Unsubscribe)}");
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        public ResultBase<DomainContextMetaData> GetMetaData()
        {
            var result = new ResultBase<DomainContextMetaData>();
            try
            {
                result.Value = this.domainContext.Dispatcher.Invoke(() => this.domainContext.GetMetaData(this.authentication));
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault(e);
            }
            return result;
        }

        public ResultBase<DomainRowInfo[]> SetRow(Guid domainID, DomainRowInfo[] rows)
        {
            return this.Invoke(domainID, (domain) =>
            {
                return domain.SetRow(this.authentication, rows);
            });
        }

        public ResultBase SetProperty(Guid domainID, string propertyName, object value)
        {
            return this.Invoke(domainID, (domain) =>
            {
                domain.SetProperty(this.authentication, propertyName, value);
            });
        }

        public ResultBase BeginUserEdit(Guid domainID, DomainLocationInfo location)
        {
            return this.Invoke(domainID, (domain) =>
            {
                domain.BeginUserEdit(this.authentication, location);
            });
        }

        public ResultBase EndUserEdit(Guid domainID)
        {
            return this.Invoke(domainID, (domain) =>
            {
                domain.EndUserEdit(this.authentication);
            });
        }

        public ResultBase<DomainUserInfo> Kick(Guid domainID, string userID, string comment)
        {
            return this.Invoke(domainID, (domain) =>
            {
                return domain.Kick(this.authentication, userID, comment);
            });
        }

        public ResultBase SetOwner(Guid domainID, string userID)
        {
            return this.Invoke(domainID, (domain) =>
            {
                domain.SetOwner(this.authentication, userID);
            });
        }

        public ResultBase SetUserLocation(Guid domainID, DomainLocationInfo location)
        {
            return this.Invoke(domainID, (domain) =>
            {
                domain.SetUserLocation(this.authentication, location);
            });
        }

        public ResultBase<DomainRowInfo[]> NewRow(Guid domainID, DomainRowInfo[] rows)
        {
            return this.Invoke(domainID, (domain) =>
            {
                return domain.NewRow(this.authentication, rows);
            });
        }

        public ResultBase RemoveRow(Guid domainID, DomainRowInfo[] rows)
        {
            return this.Invoke(domainID, (domain) =>
            {
                domain.RemoveRow(this.authentication, rows);
            });
        }

        public ResultBase DeleteDomain(Guid domainID, bool force)
        {
            return this.Invoke(domainID, (domain) =>
            {
                domain.Delete(this.authentication, force);
            });
        }

        public bool IsAlive()
        {
            if (this.authentication == null)
                return false;
            this.logService.Debug($"[{this.authentication}] {nameof(DomainService)}.{nameof(IsAlive)} : {DateTime.Now}");
            this.authentication.Ping();
            return true;
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);
            this.domainContext.Dispatcher.Invoke(() =>
            {
                if (this.authentication != null)
                {
                    this.DetachEventHandlers();
                }
            });
            this.userContext.Dispatcher.Invoke(() =>
            {
                this.userContext.UsersLoggedOut -= UserContext_UsersLoggedOut;
                if (this.authentication != null)
                {
                    if (this.authentication.RemoveRef(this) == 0)
                    {
                        this.userContext.Logout(this.authentication);
                    }
                    this.authentication = null;
                }
            });
        }

        protected override void OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo)
        {
            this.Callback.OnServiceClosed(signatureDate, closeInfo);
        }

        private IDomain GetDomain(Guid domainID)
        {
            return this.domainContext.Dispatcher.Invoke(() => this.domainContext.Domains[domainID]);
        }

        private void AttachEventHandlers()
        {
            this.domainContext.Dispatcher.VerifyAccess();

            this.domainContext.Domains.DomainCreated += DomainContext_DomainCreated;
            this.domainContext.Domains.DomainDeleted += DomainContext_DomainDeleted;
            this.domainContext.Domains.DomainInfoChanged += DomainContext_DomainInfoChanged;
            this.domainContext.Domains.DomainStateChanged += DomainContext_DomainStateChanged;
            this.domainContext.Domains.DomainUserAdded += DomainContext_DomainUserAdded;
            this.domainContext.Domains.DomainUserRemoved += DomainContext_DomainUserRemoved;
            this.domainContext.Domains.DomainUserChanged += DomainContext_DomainUserChanged;
            this.domainContext.Domains.DomainRowAdded += DomainContext_DomainRowAdded;
            this.domainContext.Domains.DomainRowChanged += DomainContext_DomainRowChanged;
            this.domainContext.Domains.DomainRowRemoved += DomainContext_DomainRowRemoved;
            this.domainContext.Domains.DomainPropertyChanged += DomainContext_DomainPropertyChanged;
            this.dataBases.ItemsResetting += DataBases_ItemsResetting;
            this.dataBases.ItemsReset += DataBases_ItemsReset;

            this.logService.Debug($"[{this.OwnerID}] {nameof(DomainService)} {nameof(AttachEventHandlers)}");
        }

        private void DetachEventHandlers()
        {
            this.domainContext.Dispatcher.VerifyAccess();

            this.domainContext.Domains.DomainCreated -= DomainContext_DomainCreated;
            this.domainContext.Domains.DomainDeleted -= DomainContext_DomainDeleted;
            this.domainContext.Domains.DomainInfoChanged -= DomainContext_DomainInfoChanged;
            this.domainContext.Domains.DomainStateChanged -= DomainContext_DomainStateChanged;
            this.domainContext.Domains.DomainUserRemoved -= DomainContext_DomainUserRemoved;
            this.domainContext.Domains.DomainUserAdded -= DomainContext_DomainUserAdded;
            this.domainContext.Domains.DomainUserChanged -= DomainContext_DomainUserChanged;
            this.domainContext.Domains.DomainRowAdded -= DomainContext_DomainRowAdded;
            this.domainContext.Domains.DomainRowChanged -= DomainContext_DomainRowChanged;
            this.domainContext.Domains.DomainRowRemoved -= DomainContext_DomainRowRemoved;
            this.domainContext.Domains.DomainPropertyChanged -= DomainContext_DomainPropertyChanged;
            this.dataBases.ItemsResetting -= DataBases_ItemsResetting;
            this.dataBases.ItemsReset -= DataBases_ItemsReset;

            this.logService.Debug($"[{this.OwnerID}] {nameof(DomainService)} {nameof(DetachEventHandlers)}");
        }

        private void UserContext_UsersLoggedOut(object sender, ItemsEventArgs<IUser> e)
        {
            var actionUserID = e.UserID;
            var contains = e.Items.Any(item => item.ID == this.authentication.ID);
            var closeInfo = (CloseInfo)e.MetaData;
            if (actionUserID != this.authentication.ID && contains == true)
            {
                this.InvokeEvent(null, null, () => this.Callback.OnServiceClosed(e.SignatureDate, (CloseInfo)e.MetaData));
            }
        }

        private void DomainContext_DomainCreated(object sender, DomainEventArgs e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainInfo = e.DomainInfo;
            var domainState = e.DomainState;
            if (this.resettings.Contains(domainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDomainCreated(e.SignatureDate, domainInfo, domainState));
        }

        private void DomainContext_DomainDeleted(object sender, DomainDeletedEventArgs e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var isCanceled = e.IsCanceled;
            this.domains.Remove(domainID);
            if (this.resettings.Contains(e.DomainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDomainDeleted(signatureDate, domainID, isCanceled));
        }

        private void DomainContext_DomainInfoChanged(object sender, DomainEventArgs e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var domainInfo = e.DomainInfo;
            if (this.resettings.Contains(domainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDomainInfoChanged(signatureDate, domainID, domainInfo));
        }

        private void DomainContext_DomainStateChanged(object sender, DomainEventArgs e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var domainState = e.DomainState;
            if (this.resettings.Contains(e.DomainInfo.DataBaseID))
                return;
            System.Diagnostics.Trace.WriteLine("DomainContext_DomainStateChanged");
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnDomainStateChanged(signatureDate, domainID, domainState));
        }

        private void DomainContext_DomainUserAdded(object sender, DomainUserEventArgs e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var domainUserInfo = e.DomainUserInfo;
            var domainUserState = e.DomainUserState;
            if (domainUserInfo.UserID == this.OwnerID)
                this.domains.Add(domainID);
            if (this.resettings.Contains(e.DomainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUserAdded(signatureDate, domainID, domainUserInfo, domainUserState));
        }

        private void DomainContext_DomainUserChanged(object sender, DomainUserEventArgs e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var domainUserState = e.DomainUserState;
            var domainUserInfo = e.DomainUserInfo;
            if (this.domains.Contains(domainID) == false)
                domainUserInfo.Location = DomainLocationInfo.Empty;
            if (this.resettings.Contains(e.DomainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUserChanged(signatureDate, domainID, domainUserInfo, domainUserState));
        }

        private void DomainContext_DomainUserRemoved(object sender, DomainUserRemovedEventArgs e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var domainUserInfo = e.DomainUserInfo;
            var removeInfo = e.RemoveInfo;
            if (domainUserInfo.UserID == this.OwnerID)
                this.domains.Remove(domainID);
            if (this.resettings.Contains(e.DomainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnUserRemoved(signatureDate, domainID, domainUserInfo, removeInfo));
        }

        private void DomainContext_DomainRowAdded(object sender, DomainRowEventArgs e)
        {
            if (this.domains.Contains(e.DomainInfo.DomainID) == false)
                return;
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var rows = e.Rows;
            if (this.resettings.Contains(e.DomainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnRowAdded(signatureDate, domainID, rows));
        }

        private void DomainContext_DomainRowChanged(object sender, DomainRowEventArgs e)
        {
            if (this.domains.Contains(e.DomainInfo.DomainID) == false)
                return;
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var rows = e.Rows;
            if (this.resettings.Contains(e.DomainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnRowChanged(signatureDate, domainID, rows));
        }

        private void DomainContext_DomainRowRemoved(object sender, DomainRowEventArgs e)
        {
            if (this.domains.Contains(e.DomainInfo.DomainID) == false)
                return;
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var rows = e.Rows;
            if (this.resettings.Contains(e.DomainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnRowRemoved(signatureDate, domainID, rows));
        }

        private void DomainContext_DomainPropertyChanged(object sender, DomainPropertyEventArgs e)
        {
            var userID = this.authentication.ID;
            var exceptionUserID = e.UserID;
            var signatureDate = e.SignatureDate;
            var domainID = e.DomainInfo.DomainID;
            var propertyName = e.PropertyName;
            var value = e.Value;
            if (this.resettings.Contains(e.DomainInfo.DataBaseID))
                return;
            this.InvokeEvent(userID, exceptionUserID, () => this.Callback.OnPropertyChanged(signatureDate, domainID, propertyName, value));
        }

        private void DataBases_ItemsResetting(object sender, ItemsEventArgs<IDataBase> e)
        {
            foreach (var item in e.Items)
            {
                this.resettings.Add(item.ID);
            }
        }

        private void DataBases_ItemsReset(object sender, ItemsEventArgs<IDataBase> e)
        {
            foreach (var item in e.Items)
            {
                this.resettings.Remove(item.ID);
            }
        }

        private ResultBase<T> Invoke<T>(Guid domainID, Func<IDomain, T> func)
        {
            var result = new ResultBase<T>();
            try
            {
                var domain = this.GetDomain(domainID);
                if (domain == null)
                    throw new DomainNotFoundException(domainID);
                result.Value = domain.Dispatcher.Invoke(() => func(domain));
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault() { ExceptionType = e.GetType().Name, Message = e.Message };
            }
            return result;
        }

        private ResultBase Invoke(Guid domainID, Action<IDomain> action)
        {
            var result = new ResultBase();
            try
            {
                var domain = this.GetDomain(domainID);
                if (domain == null)
                    throw new DomainNotFoundException(domainID);
                domain.Dispatcher.Invoke(() => action(domain));
                result.SignatureDate = this.authentication.SignatureDate;
            }
            catch (Exception e)
            {
                result.Fault = new CremaFault() { ExceptionType = e.GetType().Name, Message = e.Message };
            }
            return result;
        }

        #region ICremaServiceItem

        void ICremaServiceItem.Abort(bool disconnect)
        {
            this.domainContext.Dispatcher.Invoke(() =>
            {
                this.DetachEventHandlers();
            });
            this.userContext.Dispatcher.Invoke(() =>
            {
                this.userContext.UsersLoggedOut -= UserContext_UsersLoggedOut;
                this.authentication = null;
            });
            CremaService.Dispatcher.Invoke(() =>
            {
                if (disconnect == false)
                {
                    this.Callback?.OnServiceClosed(SignatureDate.Empty, CloseInfo.Empty);
                    try
                    {
                        this.Channel?.Close(TimeSpan.FromSeconds(10));
                    }
                    catch
                    {
                        this.Channel?.Abort();
                    }
                }
                else
                {
                    this.Channel?.Abort();
                }
            });
        }

        #endregion
    }
}
