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

using Ntreev.Crema.Client.Framework.Dialogs.ViewModels;
using Ntreev.Crema.Client.Framework.Properties;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ntreev.Crema.Client.Framework
{
    public static class DomainUserDescriptorUtility
    {
        public static bool IsOnline(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            return descriptor.DomainUserState.HasFlag(DomainUserState.Online);
        }

        public static bool IsAdmin(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            return authentication.Authority == Authority.Admin;
        }

        public static bool IsModified(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            return descriptor.DomainUserState.HasFlag(DomainUserState.IsModified);
        }

        public static bool IsOwner(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            return descriptor.DomainUserState.HasFlag(DomainUserState.IsOwner);
        }

        public static bool CanRead(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            return descriptor.DomainUserInfo.AccessType.HasFlag(DomainAccessType.Read);
        }

        public static bool CanWrite(Authentication authentication, IDomainUserDescriptor descriptor)
        {
            if (authentication == null)
                throw new ArgumentNullException(nameof(authentication));
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));
            return descriptor.DomainUserInfo.AccessType.HasFlag(DomainAccessType.ReadWrite);
        }

        //public static bool CanSetOwner(Authentication authentication, IDomainUserDescriptor descriptor)
        //{
        //    if (authentication == null)
        //        throw new ArgumentNullException(nameof(authentication));
        //    if (descriptor == null)
        //        throw new ArgumentNullException(nameof(descriptor));
        //    if (IsOwner(authentication, descriptor) == false)
        //        return false;
        //    return authentication.Authority == Authority.Admin;
        //}

        //public static bool CanKick(Authentication authentication, IDomainUserDescriptor descriptor)
        //{
        //    if (authentication == null)
        //        throw new ArgumentNullException(nameof(authentication));
        //    if (descriptor == null)
        //        throw new ArgumentNullException(nameof(descriptor));
        //    if (IsOwner(authentication, descriptor) == true)
        //        return false;
        //    return authentication.Authority == Authority.Admin;
        //}
    }
}
