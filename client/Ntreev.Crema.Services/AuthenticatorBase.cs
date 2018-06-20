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
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using System;
using System.Reflection;

namespace Ntreev.Crema.Services
{
    public abstract class AuthenticatorBase : IPlugin
    {
        private readonly string name;
        private readonly Guid pluginID;
        private Authentication authentication;

        protected AuthenticatorBase()
        {
            if (this.GetType().Attributes.HasFlag(TypeAttributes.Public) == true)
                throw new InvalidOperationException(Resources.Exception_NotAllowedAsPublic);
            this.name = this.GetType().Assembly.FullName;
            this.pluginID = GuidUtility.FromName(name);
        }

        protected AuthenticatorBase(string name)
        {
            if (this.GetType().Attributes.HasFlag(TypeAttributes.Public) == false)
                throw new InvalidOperationException(Resources.Exception_NotAllowedAsPublic);
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            if (this.name == string.Empty)
                throw new ArgumentException(Resources.Exception_InvalidName);
            this.pluginID = GuidUtility.FromName(name);
        }

        public string Name => this.authentication.Name;

        public string ID => this.authentication.ID;

        public Authority Authority => this.authentication.Authority;

        public bool IsOpened => this.authentication != null;

        public static implicit operator Authentication(AuthenticatorBase authenticator)
        {
            return authenticator?.authentication;
        }

        protected virtual void OnInitialize(Authentication authentication)
        {

        }

        protected virtual void OnRelease()
        {

        }

        #region IPlugin

        void IPlugin.Initialize(Authentication authentication)
        {
            this.authentication = authentication;
            this.OnInitialize(authentication);
        }

        void IPlugin.Release()
        {
            this.authentication = null;
            this.OnRelease();
        }

        Guid IPlugin.ID => this.pluginID;

        string IPlugin.Name => this.name;

        #endregion
    }
}
