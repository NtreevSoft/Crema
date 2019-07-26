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

using Ntreev.Library;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Library.ObjectModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Ntreev.Crema.ServiceModel.Properties;
using System.Collections.Specialized;
using Ntreev.Crema.Data.Xml.Schema;
using System.Text.RegularExpressions;
using Ntreev.Library.IO;

namespace Ntreev.Crema.ServiceModel
{
    internal abstract class UserBase<_I, _C, _IC, _CC, _CT> : ItemBase<_I, _C, _IC, _CC, _CT>
        where _I : UserBase<_I, _C, _IC, _CC, _CT>
        where _C : UserCategoryBase<_I, _C, _IC, _CC, _CT>, new()
        where _IC : ItemContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CC : CategoryContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CT : ItemContext<_I, _C, _IC, _CC, _CT>
    {
        private UserInfo userInfo;
        private BanInfo banInfo = BanInfo.Empty;
        private bool isLoaded;

        public UserBase()
        {
        }

        public void UpdateUserInfo(UserInfo userInfo)
        {
            this.userInfo = userInfo;
            this.OnUserInfoChanged(EventArgs.Empty);
        }

        public void Initialize(UserInfo userInfo, BanInfo banInfo)
        {
            if (this.isLoaded == true)
                throw new InvalidOperationException();
            this.userInfo = userInfo;
            this.banInfo = banInfo;

            this.InitializeUserInfo(userInfo);

            this.isLoaded = true;
            this.OnUserInfoChanged(EventArgs.Empty);
            this.OnUserBanInfoChanged(EventArgs.Empty);
        }

        protected abstract void InitializeUserInfo(UserInfo userInfo);

        public UserInfo UserInfo
        {
            get { return this.userInfo; }
        }

        public BanInfo BanInfo
        {
            get
            {
                if (this.banInfo.UserID != string.Empty)
                {
                    this.banInfo.Path = this.Path;
                }
                return this.banInfo;
            }
            protected set
            {
                this.banInfo = value;
                this.OnUserBanInfoChanged(EventArgs.Empty);
            }
        }

        public Authority Authority
        {
            get { return this.userInfo.Authority; }
        }

        public event EventHandler UserInfoChanged;

        public event EventHandler UserStateChanged;

        public event EventHandler UserBanInfoChanged;

        protected void Move(IAuthentication authentication, string categoryPath)
        {
            this.Category = this.Context.Categories[categoryPath];
        }

        protected void Delete(IAuthentication authentication)
        {
            base.Dispose();
        }

        protected void Ban(IAuthentication authentication, BanInfo banInfo)
        {
            this.banInfo = banInfo;
            this.OnUserBanInfoChanged(EventArgs.Empty);
        }

        protected void Unban(IAuthentication authentication)
        {
            this.banInfo = BanInfo.Empty;
            this.OnUserBanInfoChanged(EventArgs.Empty);
        }

        internal virtual void OnUserInfoChanged(EventArgs e)
        {
            this.UserInfoChanged?.Invoke(this, e);
        }

        internal virtual void OnUserStateChanged(EventArgs e)
        {
            this.UserStateChanged?.Invoke(this, e);
        }

        internal virtual void OnUserBanInfoChanged(EventArgs e)
        {
            this.UserBanInfoChanged?.Invoke(this, e);
        }

        protected override void OnPathChanged(string oldPath, string newPath)
        {
            base.OnPathChanged(oldPath, newPath);

            if (this.Category != null)
            {
                this.userInfo.CategoryPath = this.Category == null ? PathUtility.Separator : this.Category.Path;
                this.OnUserInfoChanged(EventArgs.Empty);
            }

            if (this.banInfo.UserID != string.Empty)
            {
                this.banInfo.Path = Regex.Replace(this.banInfo.Path, "^" + oldPath, newPath);
                this.OnUserBanInfoChanged(EventArgs.Empty);
            }
        }

        internal void OnPathChangeInternal(string oldPath, string newPath) => this.OnPathChanged(oldPath, newPath);

        protected void ValidateMove(IAuthentication authentication, string categoryPath)
        {
            if (this.Category.Path == categoryPath)
                throw new ArgumentException(Resources.Exception_CannotMoveToSameFolder, nameof(categoryPath));
            var category = this.Context.Categories[categoryPath];
            if (category == null)
                throw new CategoryNotFoundException(categoryPath);
            base.ValidateMove(category);
        }
    }
}
