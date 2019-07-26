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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.ServiceModel.Properties;
using System.Xml;
using System.IO;
using Ntreev.Library.ObjectModel;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.Data.Xml.Schema;
using System.ComponentModel;
using Ntreev.Library;
using System.Text.RegularExpressions;

namespace Ntreev.Crema.ServiceModel
{
    internal abstract class PermissionItemBase<_I, _C, _IC, _CC, _CT> : ItemBase<_I, _C, _IC, _CC, _CT>, ILockParent, IAccessParent
        where _I : PermissionItemBase<_I, _C, _IC, _CC, _CT>
        where _C : PermissionCategoryBase<_I, _C, _IC, _CC, _CT>, new()
        where _IC : ItemContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CC : CategoryContainer<_I, _C, _IC, _CC, _CT>, new()
        where _CT : ItemContext<_I, _C, _IC, _CC, _CT>
    {
        internal AccessInfo accessInfo = AccessInfo.Empty;
        private IAccessParent accessParent;

        internal LockInfo lockInfo = LockInfo.Empty;
        private ILockParent lockParent;

        public PermissionItemBase()
        {

        }

        public AccessType GetAccessType(IAuthentication authentication)
        {
            if (authentication.IsSystem == true)
                return AccessType.System;
            if (authentication.Types == AuthenticationType.None)
                return AccessType.Guest;
            if (authentication.Types.HasFlag(AuthenticationType.ReadOnly))
                return AccessType.Guest;
            if (this.IsLocked == true && authentication.IsOwnerOf(this.LockInfo) == false)
                return AccessType.None;
            if (this.IsLocked == true && authentication.IsOwnerOf(this.LockInfo) == true)
                return AccessType.System;
            if (this.IsPublic == true)
                return AccessType.Owner;
            return this.AccessInfo.GetAccessType(authentication.ID);
        }

        public bool VerifyAccessType(IAuthentication authentication, AccessType accessType)
        {
            return this.GetAccessType(authentication).HasFlag(accessType);
        }

        protected void SetPublic(IAuthentication authentication)
        {
            this.accessInfo.SetPublic();
            this.UpdateAccessParent(this.accessParent);
            this.OnAccessChanged(EventArgs.Empty);
        }

        protected void SetPrivate(IAuthentication authentication)
        {
            this.accessInfo.SetPrivate(this.Path, authentication.SignatureDate);
            this.UpdateAccessParent(this);
            this.OnAccessChanged(EventArgs.Empty);
        }

        protected void AddAccessMember(IAuthentication authentication, string memberID, AccessType accessType)
        {
            this.accessInfo.Add(authentication.SignatureDate, memberID, accessType);
            this.UpdateAccessParent(this);
            this.OnAccessChanged(EventArgs.Empty);
        }

        protected void SetAccessMember(IAuthentication authentication, string memberID, AccessType accessType)
        {
            this.accessInfo.Set(authentication.SignatureDate, memberID, accessType);
            this.UpdateAccessParent(this);
            this.OnAccessChanged(EventArgs.Empty);
        }

        protected void RemoveAccessMember(IAuthentication authentication, string memberID)
        {
            this.accessInfo.Remove(authentication.SignatureDate, memberID);
            this.UpdateAccessParent(this);
            this.OnAccessChanged(EventArgs.Empty);
        }

        protected void Lock(IAuthentication authentication, string comment)
        {
            this.lockInfo = new LockInfo()
            {
                Path = this.Path,
                ParentPath = string.Empty,
                SignatureDate = new SignatureDate(authentication.ID, authentication.Token),
                Comment = comment
            };
            this.UpdateLockParent(this);
            this.OnLockChanged(EventArgs.Empty);
        }

        protected void Unlock(IAuthentication authentication)
        {
            this.lockInfo = LockInfo.Empty;
            this.UpdateLockParent(this.lockParent);
            this.OnLockChanged(EventArgs.Empty);
        }

        public bool IsPublic
        {
            get { return this.IsPrivate == false; }
        }

        public bool IsPrivate
        {
            get
            {
                if (this.accessInfo.UserID != string.Empty)
                    return true;
                return this.accessParent != null;
            }
        }

        public bool IsLocked
        {
            get
            {
                if (this.lockInfo.UserID != string.Empty)
                    return true;
                return this.lockParent != null;
            }
        }

        public LockInfo LockInfo
        {
            get
            {
                if (this.lockInfo.UserID != string.Empty)
                {
                    return this.lockInfo;
                }

                if (this.lockParent == null)
                {
                    return LockInfo.Empty;
                }

                var lockInfo = this.lockParent.LockInfo;
                lockInfo.ParentPath = this.lockParent.Path;
                lockInfo.Path = this.Path;
                return lockInfo;
            }
            protected set
            {
                if (this.lockInfo == value)
                    return;
                this.lockInfo = value;
                if (this.lockInfo.UserID != string.Empty)
                    this.UpdateLockParent(this);
                else
                    this.UpdateLockParent(this.lockParent);
                this.OnLockChanged(EventArgs.Empty);
            }
        }

        public AccessInfo AccessInfo
        {
            get
            {
                if (this.accessInfo.UserID != string.Empty)
                {
                    return this.accessInfo;
                }

                if (this.accessParent == null)
                {
                    return AccessInfo.Empty;
                }

                var accessInfo = this.accessParent.AccessInfo;
                accessInfo.ParentPath = accessInfo.Path;
                accessInfo.Path = this.Path;
                return accessInfo;
            }
            protected set
            {
                if (this.accessInfo == value)
                    return;
                this.accessInfo = value;
                if (this.accessInfo.UserID != string.Empty)
                    this.UpdateAccessParent(this);
                else
                    this.UpdateAccessParent(this.accessParent);
                this.OnAccessChanged(EventArgs.Empty);
            }
        }

        public ILockParent LockParent
        {
            get { return this.lockParent; }
            set
            {
                this.lockParent = value;
                this.OnLockChanged(EventArgs.Empty);
            }
        }

        public IAccessParent AccessParent
        {
            get { return this.accessParent; }
            set
            {
                this.accessParent = value;
                this.OnAccessChanged(EventArgs.Empty);
            }
        }

        public event EventHandler LockChanged;

        public event EventHandler AccessChanged;

        protected override void OnRenamed(EventArgs e)
        {
            this.UpdateAccessInfo();
            this.UpdateLockInfo();
            base.OnRenamed(e);
        }

        protected override void OnMoved(EventArgs e)
        {
            this.UpdateAccessInfo();
            this.UpdateLockInfo();
            base.OnMoved(e);
        }

        protected override void OnDeleted(EventArgs e)
        {
            base.OnDeleted(e);
        }

        protected virtual void OnLockChanged(EventArgs e)
        {
            this.LockChanged?.Invoke(this, e);
        }

        protected virtual void OnAccessChanged(EventArgs e)
        {
            this.AccessChanged?.Invoke(this, e);
        }

        public void ValidateAccessType(IAuthentication authentication, AccessType accessType)
        {
            if (this.VerifyAccessType(authentication, accessType) == false)
                throw new PermissionDeniedException();
        }

        protected void ValidateSetPublic(IAuthentication authentication)
        {
            if (this.AccessInfo.IsPrivate == false || this.AccessInfo.IsInherited == true)
                throw new PermissionException(Resources.Exception_AlreadyPublic);
            if (this.VerifyAccessType(authentication, AccessType.Owner) == false)
                throw new PermissionDeniedException();

            this.OnValidateSetPublic(authentication, this);
        }

        protected void ValidateSetPrivate(IAuthentication authentication)
        {
            if (this.AccessInfo.IsPrivate == true && this.AccessInfo.IsInherited == false)
                throw new PermissionException(Resources.Exception_AlreadyPrivate);
            if (this.VerifyAccessType(authentication, AccessType.Owner) == false)
                throw new PermissionDeniedException();

            this.OnValidateSetPrivate(authentication, this);
        }

        protected void ValidateAddAccessMember(IAuthentication authentication, string memberID, AccessType accessType)
        {
            if (this.AccessInfo.IsPrivate == false)
                throw new PermissionException(Resources.Exception_CannotAddToPublic);
            if (this.AccessInfo.IsInherited == true)
                throw new PermissionException(Resources.Exception_InheritedPrivateItemCannotAdd);
            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();
            if (NameValidator.VerifyName(memberID) == false && NameValidator.VerifyCategoryPath(memberID) == false)
                throw new PermissionException(Resources.Exception_InvalidIDorPath);
            if (accessType == AccessType.Owner)
                throw new PermissionException($"'{AccessType.Owner}' 권한은 사용할 수 없습니다.");
            if (accessType == AccessType.System)
                throw new PermissionException($"'{AccessType.System}' 권한은 사용할 수 없습니다.");
            if (this.AccessInfo.Contains(memberID) == true)
                throw new PermissionException("이미 추가된 구성원입니다.");

            this.OnValidateAddAccessMember(authentication, this, memberID, accessType);
        }

        protected void ValidateSetAccessMember(IAuthentication authentication, string memberID, AccessType accessType)
        {
            if (this.AccessInfo.IsPrivate == false)
                throw new PermissionException(Resources.Exception_CannotAddToPublic);
            if (this.AccessInfo.IsInherited == true)
                throw new PermissionException(Resources.Exception_InheritedPrivateItemCannotAdd);
            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();
            if (NameValidator.VerifyName(memberID) == false && NameValidator.VerifyCategoryPath(memberID) == false)
                throw new PermissionException(Resources.Exception_InvalidIDorPath);
            if (this.AccessInfo.GetAccessType(memberID) == accessType)
                throw new PermissionException($"'{memberID}' 은(는) 이미 '{accessType}' 으로 설정되어 있습니다.");
            if (accessType == AccessType.Owner && this.AccessInfo.GetAccessType(memberID) != AccessType.Master)
                throw new PermissionException($"'{AccessType.Owner}' 타입은 '{AccessType.Master}' 권한을 가진 구성원에게만 설정이 가능합니다.");
            if (accessType == AccessType.System)
                throw new PermissionException($"'{AccessType.System}' 권한은 사용할 수 없습니다.");
            if (this.AccessInfo.GetAccessType(memberID) == AccessType.Owner)
                throw new PermissionException($"'{memberID}' 은(는) '{AccessType.Owner}' 이므로 다른 권한으로 변경할 수 없습니다.");
            if (this.VerifyAccessType(authentication, AccessType.System) == false && this.AccessInfo.GetAccessType(memberID) >= this.AccessInfo.GetAccessType(authentication.ID))
                throw new PermissionException("자신과 권한이 같거나 높은 구성원의 권한은 변경할 수 없습니다.");
            if (this.AccessInfo.Contains(memberID) == false)
                throw new PermissionException("구성원이 아니기 때문에 변경할 수 없습니다.");

            this.OnValidateSetAccessMember(authentication, this, memberID, accessType);
        }

        protected void ValidateRemoveAccessMember(IAuthentication authentication, string memberID)
        {
            if (this.AccessInfo.IsPrivate == false)
                throw new PermissionException(Resources.Exception_CannotRemoveToPublic);
            if (this.AccessInfo.IsInherited == true)
                throw new PermissionException(Resources.Exception_InheritedPrivateItemCannotRemove);
            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();
            if (NameValidator.VerifyName(memberID) == false && NameValidator.VerifyCategoryPath(memberID) == false)
                throw new PermissionException(Resources.Exception_InvalidIDorPath);
            if (this.AccessInfo.GetAccessType(memberID) >= AccessType.Owner)
                throw new PermissionException($"'{AccessType.Owner}' 등급은 제거할 수 없습니다.");
            if (this.AccessInfo.GetAccessType(memberID) >= this.GetAccessType(authentication))
                throw new PermissionException("자신과 권한이 같거나 높은 구성원는 제거할 수 없습니다.");
            if (this.AccessInfo.Contains(memberID) == false)
                throw new PermissionException("구성원이 아니기 때문에 삭제할 수 없습니다.");

            this.OnValidateRemoveAccessMember(authentication, this);
        }

        protected void ValidateLock(IAuthentication authentication)
        {
            if (this.LockInfo.IsLocked == true && this.LockInfo.IsInherited == false)
                throw new PermissionException(Resources.Exception_AlreadyLocked);
            if (authentication.IsAdmin == false && this.VerifyAccessType(authentication, AccessType.Editor) == false)
                throw new PermissionDeniedException();
            this.OnValidateLock(authentication, this);
        }

        protected void ValidateUnlock(IAuthentication authentication)
        {
            if (this.LockInfo.IsLocked == false || this.LockInfo.IsInherited == true)
                throw new PermissionException(Resources.Exception_NotLocked);
            if (authentication.IsSystem == false && authentication.IsOwnerOf(this.LockInfo) == false)
                throw new PermissionDeniedException();
            this.OnValidateUnlock(authentication, this);
        }

        protected void ValidateRename(IAuthentication authentication, string name)
        {
            if (this.Name == name)
                throw new ArgumentException(Resources.Exception_CannotRename, nameof(name));
            this.OnValidateRename(authentication, this, this.Path, new ItemName(this.Category.Path, name));
        }

        protected void ValidateMove(IAuthentication authentication, string categoryPath)
        {
            if (this.Category.Path == categoryPath)
                throw new ArgumentException(Resources.Exception_CannotMoveToSameFolder, nameof(categoryPath));
            var category = this.Context.Categories[categoryPath];
            if (category == null)
                throw new CategoryNotFoundException(categoryPath);
            base.ValidateMove(category);

            this.OnValidateMove(authentication, this, this.Path, new ItemName(categoryPath, this.Name));
        }

        protected void ValidateDelete(IAuthentication authentication)
        {
            this.OnValidateDelete(authentication, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateSetPublic(IAuthentication authentication, object target)
        {
            //if (this.IsLocked == false && this.VerifyAccessType(authentication, AccessType.Owner) == false)
            //    throw new PermissionDeniedException();
            //if (this.IsLocked == true && this.LockInfo.UserID != authentication.ID && authentication.IsAdmin == false)
            //    throw new PermissionDeniedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateSetPrivate(IAuthentication authentication, object target)
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateAddAccessMember(IAuthentication authentication, object target, string memberID, AccessType accessType)
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateSetAccessMember(IAuthentication authentication, object target, string memberID, AccessType accessType)
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateRemoveAccessMember(IAuthentication authentication, object target)
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateLock(IAuthentication authentication, object target)
        {
            var lockInfo = this.LockInfo;
            if (target != this && lockInfo.IsLocked == true && lockInfo.IsOwner(authentication.ID) == false)
                throw new PermissionDeniedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateUnlock(IAuthentication authentication, object target)
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateRename(IAuthentication authentication, object target, string oldPath, string newPath)
        {
            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateMove(IAuthentication authentication, object target, string oldPath, string newPath)
        {
            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateDelete(IAuthentication authentication, object target)
        {
            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();
        }

        protected virtual void OnUpdateLockParent(ILockParent lockParent)
        {

        }

        protected virtual void OnUpdateAccessParent(IAccessParent accessParent)
        {

        }

        protected override void OnPathChanged(string oldPath, string newPath)
        {
            base.OnPathChanged(oldPath, newPath);

            if (this.lockInfo.UserID != string.Empty)
            {
                this.lockInfo.Path = Regex.Replace(this.lockInfo.Path, "^" + oldPath, newPath);
                this.OnLockChanged(EventArgs.Empty);
            }

            if (this.accessInfo.UserID != string.Empty)
            {
                this.accessInfo.Path = Regex.Replace(this.accessInfo.Path, "^" + oldPath, newPath);
                this.OnAccessChanged(EventArgs.Empty);
            }
        }

        public virtual void UpdateAccessInfo()
        {

        }

        public virtual void UpdateLockInfo()
        {

        }

        internal void UpdateLockParent(ILockParent lockParent)
        {
            this.OnUpdateLockParent(lockParent);
        }

        internal void UpdateAccessParent(IAccessParent accessParent)
        {
            this.OnUpdateAccessParent(accessParent);
        }

        internal void InvokeAccessChanged(EventArgs e)
        {
            this.OnAccessChanged(e);
        }

        #region IAccessParent

        //AccessType IAccessParent.GetAccessType(IAuthentication authentication)
        //{
        //    return this.GetAccessType(authentication);
        //}

        #endregion
    }
}
