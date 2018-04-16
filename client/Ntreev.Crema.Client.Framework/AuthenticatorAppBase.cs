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
using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Client.Framework
{
    public abstract class AuthenticatorAppBase : AuthenticatorBase
    {
        private readonly ICremaAppHost cremaAppHost;
        private IDataBase dataBase;

        protected AuthenticatorAppBase(ICremaAppHost cremaAppHost)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
        }

        protected AuthenticatorAppBase(ICremaAppHost cremaAppHost, string name)
            : base(name)
        {
            this.cremaAppHost = cremaAppHost;
            this.cremaAppHost.Loaded += CremaAppHost_Loaded;
            this.cremaAppHost.Unloaded += CremaAppHost_Unloaded;
        }

        private async void CremaAppHost_Loaded(object sender, EventArgs e)
        {
            if (this.cremaAppHost.GetService(typeof(IDataBase)) is IDataBase dataBase)
            {
                this.dataBase = dataBase;
                await this.dataBase.Dispatcher.InvokeAsync(() =>
                {
                    this.dataBase.Enter(this);
                });
            }
        }

        private async void CremaAppHost_Unloaded(object sender, EventArgs e)
        {
            await this.dataBase?.Dispatcher.InvokeAsync(() =>
            {
                if ((Authentication)this is Authentication authentication)
                {
                    this.dataBase.Leave(authentication);
                    this.dataBase = null;
                }
            });
        }
    }
}
