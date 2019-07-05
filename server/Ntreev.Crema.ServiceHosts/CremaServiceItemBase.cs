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
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Reflection;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using System.Threading;
using Ntreev.Library;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System.Windows.Threading;
using System.Diagnostics;

namespace Ntreev.Crema.ServiceHosts
{
    public abstract class CremaServiceItemBase : IDisposable
    {
        private string sessionID;
        protected ILogService logService;
        private string ownerID;
        protected IContextChannel channel;
        protected ServiceHostBase host;

        protected CremaServiceItemBase(ILogService logService)
        {
            this.logService = logService;
            this.host = OperationContext.Current.Host;
            this.channel = OperationContext.Current.Channel;
            this.sessionID = OperationContext.Current.Channel.SessionId;
        }

        public event EventHandler Disposed;

        protected void InvokeEvent(string userID, string exceptionUserID, Action action)
        {
            CremaService.Dispatcher?.InvokeAsync(() =>
            {
                if (this.sessionID == null || (userID != null && userID == exceptionUserID))
                    return;
                if (this.channel != null)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        this.logService.Error(e);
                    }
                }
            });
        }

        protected virtual void OnDisposed(EventArgs e)
        {
            this.Disposed?.Invoke(this, e);
        }

        protected IContextChannel Channel
        {
            get { return this.channel; }
        }

        protected abstract void OnServiceClosed(SignatureDate signatureDate, CloseInfo closeInfo);

        protected string OwnerID
        {
            get { return this.ownerID; }
            set { this.ownerID = value; }
        }



        #region IDisposable

        public virtual void Dispose()
        {
            this.host = null;
            this.channel = null;
            this.OnDisposed(EventArgs.Empty);
            this.logService.Debug($"[{this.OwnerID}] {this.GetType().Name} {nameof(IDisposable.Dispose)}");
        }

        #endregion
    }
    public abstract class CremaServiceItemBase<T> : CremaServiceItemBase, IDisposable
    {
        private T callback;

        protected CremaServiceItemBase(ILogService logService) : base(logService)
        {
            this.callback = OperationContext.Current.GetCallbackChannel<T>();
            OperationContext.Current.Host.Closing += Host_Closing;
        }

        protected T Callback
        {
            get { return this.callback; }
        }

        private void Host_Closing(object sender, EventArgs e)
        {
            if (this.callback != null)
            {
                if (this.channel.State == CommunicationState.Opened)
                {
                    this.OnServiceClosed(SignatureDate.Empty, CloseInfo.Empty);
                }
                this.callback = default(T);
            }
        }

        void IDisposable.Dispose()
        {
            this.host.Closing -= Host_Closing;
            this.callback = default(T);
            base.Dispose();
        }
    }
}
