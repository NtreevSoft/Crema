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
using System.Data;

namespace Ntreev.Crema.ServiceModel
{
    internal abstract class DataBaseBase<_I1, _C1, _IC1, _CC1, _CT1, _I2, _C2, _IC2, _CC2, _CT2> : IAccessParent, ILockParent
        where _I1 : TypeBase<_I1, _C1, _IC1, _CC1, _CT1>
        where _C1 : TypeCategoryBase<_I1, _C1, _IC1, _CC1, _CT1>, new()
        where _IC1 : ItemContainer<_I1, _C1, _IC1, _CC1, _CT1>, new()
        where _CC1 : CategoryContainer<_I1, _C1, _IC1, _CC1, _CT1>, new()
        where _CT1 : ItemContext<_I1, _C1, _IC1, _CC1, _CT1>
        where _I2 : TableBase<_I2, _C2, _IC2, _CC2, _CT2>
        where _C2 : TableCategoryBase<_I2, _C2, _IC2, _CC2, _CT2>, new()
        where _IC2 : ItemContainer<_I2, _C2, _IC2, _CC2, _CT2>, new()
        where _CC2 : CategoryContainer<_I2, _C2, _IC2, _CC2, _CT2>, new()
        where _CT2 : ItemContext<_I2, _C2, _IC2, _CC2, _CT2>
    {
        private AccessInfo accessInfo = AccessInfo.Empty;
        private LockInfo lockInfo = LockInfo.Empty;
        private string name;

        private DataBaseInfo dataBaseInfo;
        private DataBaseState dataBaseState;

        private PropertyCollection extendedProperties;

        protected DataBaseBase()
        {

        }

        public void ValidateInvokeMethod(IAuthentication authentication)
        {
            if (this.LockInfo.UserID != string.Empty && this.LockInfo.UserID != authentication.ID)
                throw new PermissionDeniedException();
        }

        public bool IsPublic
        {
            get { return this.IsPrivate == false; }
        }

        public bool IsPrivate
        {
            get { return this.accessInfo.UserID != string.Empty; }
        }

        public bool IsLocked
        {
            get { return this.lockInfo.UserID != string.Empty; }
        }

        public LockInfo LockInfo
        {
            get { return this.lockInfo; }
            protected set
            {
                if (this.lockInfo == value)
                    return;
                this.lockInfo = value;
                if (this.lockInfo.UserID != string.Empty)
                    this.UpdateLockParent(this);
                else
                    this.UpdateLockParent(null);
                this.OnLockChanged(EventArgs.Empty);
            }
        }

        public AccessInfo AccessInfo
        {
            get { return this.accessInfo; }
            protected set
            {
                if (this.accessInfo == value)
                    return;
                this.accessInfo = value;
                if (this.accessInfo.UserID != string.Empty)
                    this.UpdateAccessParent(this);
                else
                    this.UpdateAccessParent(null);
                this.OnAccessChanged(EventArgs.Empty);
            }
        }

        public string Name
        {
            get { return this.name ?? string.Empty; }
            set
            {
                this.name = value;
                this.dataBaseInfo.Name = value;
                this.OnRenamed(EventArgs.Empty);
                if (this.accessInfo.UserID != string.Empty)
                {
                    this.accessInfo.Path = this.name;
                    this.OnAccessChanged(EventArgs.Empty);
                }
                if (this.lockInfo.UserID != string.Empty)
                {
                    this.lockInfo.Path = this.name;
                    this.OnLockChanged(EventArgs.Empty);
                }
            }
        }

        public abstract TypeCategoryBase<_I1, _C1, _IC1, _CC1, _CT1> TypeCategory
        {
            get;
        }

        public abstract TableCategoryBase<_I2, _C2, _IC2, _CC2, _CT2> TableCategory
        {
            get;
        }

        public DataBaseInfo DataBaseInfo
        {
            get { return this.dataBaseInfo; }
            set
            {
                this.dataBaseInfo = value;
                this.OnDataBaseInfoChanged(EventArgs.Empty);
            }
        }

        public DataBaseState DataBaseState
        {
            get { return this.dataBaseState; }
            set
            {
                this.dataBaseState = value;
                this.OnDataBaseStateChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public PropertyCollection ExtendedProperties
        {
            get
            {
                if (this.extendedProperties == null)
                {
                    this.extendedProperties = new PropertyCollection();
                }
                return this.extendedProperties;
            }
        }

        public event EventHandler Renamed;

        public event EventHandler Deleted;

        public event EventHandler Loaded;

        public event EventHandler Unloaded;

        public event EventHandler Resetting;

        public event EventHandler Reset;

        public event EventHandler LockChanged;

        public event EventHandler AccessChanged;

        public event EventHandler DataBaseInfoChanged;

        public event EventHandler DataBaseStateChanged;

        protected AccessType GetAccessType(IAuthentication authentication)
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

        protected bool VerifyAccessType(IAuthentication authentication, AccessType accessType)
        {
            return this.GetAccessType(authentication).HasFlag(accessType);
        }

        protected void SetPublic(IAuthentication authentication)
        {
            this.accessInfo.SetPublic();
            this.UpdateAccessParent(null);
            this.OnAccessChanged(EventArgs.Empty);
        }

        protected void SetPrivate(IAuthentication authentication)
        {
            this.accessInfo.SetPrivate(this.Name, authentication.SignatureDate);
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

        protected void Delete(IAuthentication authentication)
        {
            this.OnDeleted(EventArgs.Empty);
        }

        protected void Lock(IAuthentication authentication, string comment)
        {
            this.lockInfo = new LockInfo()
            {
                Path = this.Name,
                ParentPath = string.Empty,
                SignatureDate = new SignatureDate(authentication.ID),
                Comment = comment,
            };
            this.UpdateLockParent(this);
            this.OnLockChanged(EventArgs.Empty);
        }

        protected void Unlock(IAuthentication authentication)
        {
            this.lockInfo = LockInfo.Empty;
            this.UpdateLockParent(null);
            this.OnLockChanged(EventArgs.Empty);
        }

        protected void Load(IAuthentication authentication)
        {
            this.OnLoaded(EventArgs.Empty);
        }

        protected void Unload(IAuthentication authentication)
        {
            this.OnUnloaded(EventArgs.Empty);
        }

        protected void ResettingDataBase(IAuthentication authentication)
        {
            this.OnResetting(EventArgs.Empty);
        }

        protected void ResetDataBase(IAuthentication authentication)
        {
            this.OnReset(EventArgs.Empty);
        }

        protected virtual void OnRenamed(EventArgs e)
        {
            this.Renamed?.Invoke(this, e);
        }

        protected virtual void OnDeleted(EventArgs e)
        {
            this.Deleted?.Invoke(this, e);
        }

        protected virtual void OnLoaded(EventArgs e)
        {
            this.Loaded?.Invoke(this, e);
        }

        protected virtual void OnUnloaded(EventArgs e)
        {
            this.Unloaded?.Invoke(this, e);
        }

        protected virtual void OnResetting(EventArgs e)
        {
            this.Resetting?.Invoke(this, e);
        }

        protected virtual void OnReset(EventArgs e)
        {
            this.Reset?.Invoke(this, e);
        }

        protected virtual void OnLockChanged(EventArgs e)
        {
            this.LockChanged?.Invoke(this, e);
        }

        protected virtual void OnAccessChanged(EventArgs e)
        {
            this.AccessChanged?.Invoke(this, e);
        }

        protected virtual void OnDataBaseInfoChanged(EventArgs e)
        {
            this.DataBaseInfoChanged?.Invoke(this, e);
        }

        protected virtual void OnDataBaseStateChanged(EventArgs e)
        {
            this.DataBaseStateChanged?.Invoke(this, e);
        }

        public void ValidateAccessType(IAuthentication authentication, AccessType accessType)
        {
            if (this.VerifyAccessType(authentication, accessType) == false)
                throw new PermissionDeniedException();
        }

        protected void ValidateSetPublic(IAuthentication authentication)
        {
            if (authentication.IsAdmin == false)
                throw new PermissionDeniedException();
            if (this.AccessInfo.IsPrivate == false || this.AccessInfo.IsInherited == true)
                throw new PermissionException(Resources.Exception_AlreadyPublic);
            if (this.VerifyAccessType(authentication, AccessType.Owner) == false)
                throw new PermissionDeniedException();

            this.OnValidateSetPublic(authentication, this);
        }

        protected void ValidateSetPrivate(IAuthentication authentication)
        {
            if (authentication.IsAdmin == false)
                throw new PermissionDeniedException();
            if (this.AccessInfo.IsPrivate == true && this.AccessInfo.IsInherited == false)
                throw new PermissionException(Resources.Exception_AlreadyPrivate);
            if (this.VerifyAccessType(authentication, AccessType.Owner) == false)
                throw new PermissionDeniedException();

            this.OnValidateSetPrivate(authentication, this);
        }

        protected void ValidateAddAccessMember(IAuthentication authentication, string memberID, AccessType accessType)
        {
            if (authentication.IsAdmin == false)
                throw new PermissionDeniedException();
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
            if (authentication.IsAdmin == false)
                throw new PermissionDeniedException();
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
            if (authentication.IsAdmin == false)
                throw new PermissionDeniedException();
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
            if (authentication.IsSystem == false && authentication.IsAdmin == false)
                throw new PermissionDeniedException();
            if (this.LockInfo.IsLocked == true && this.LockInfo.IsInherited == false)
                throw new PermissionException(Resources.Exception_AlreadyLocked);
            if (authentication.IsAdmin == false && this.VerifyAccessType(authentication, AccessType.Editor) == false)
                throw new PermissionDeniedException();
            this.OnValidateLock(authentication, this);
        }

        protected void ValidateUnlock(IAuthentication authentication)
        {
            if (authentication.IsSystem == false && authentication.IsAdmin == false)
                throw new PermissionDeniedException();
            if (this.LockInfo.IsLocked == false || this.LockInfo.IsInherited == true)
                throw new PermissionException(Resources.Exception_NotLocked);
            if (authentication.IsSystem == false && authentication.IsOwnerOf(this.LockInfo) == false)
                throw new PermissionDeniedException();
            this.OnValidateUnlock(authentication, this);
        }

        protected void ValidateRename(IAuthentication authentication, string name)
        {
            if (this.Name == name)
                throw new CremaException(Resources.Exception_CannotRename);
            this.OnValidateRename(authentication, this, this.Name, name);
        }

        protected void ValidateDelete(IAuthentication authentication)
        {
            this.OnValidateDelete(authentication, this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateSetPublic(IAuthentication authentication, object target)
        {
            this.ValidateInvokeMethod(authentication);

            if (target != this)
            {
                if (this.IsPrivate == true && this.VerifyAccessType(authentication, AccessType.Owner) == false)
                    throw new PermissionDeniedException();
            }

            if (this.TypeCategory != null)
            {
                this.TypeCategory.OnValidateSetPublic(authentication, target);
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.OnValidateSetPublic(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateSetPrivate(IAuthentication authentication, object target)
        {
            this.ValidateInvokeMethod(authentication);

            if (target != this)
            {
                if (this.IsPrivate == true && this.VerifyAccessType(authentication, AccessType.Owner) == false)
                    throw new PermissionDeniedException();
            }

            if (this.TypeCategory != null)
            {
                this.TypeCategory.OnValidateSetPrivate(authentication, target);
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.OnValidateSetPrivate(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateAddAccessMember(IAuthentication authentication, object target, string memberID, AccessType accessType)
        {
            this.ValidateInvokeMethod(authentication);

            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();

            if (this.TypeCategory != null)
            {
                this.TypeCategory.OnValidateAddAccessMember(authentication, target, memberID, accessType);
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.OnValidateAddAccessMember(authentication, target, memberID, accessType);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateSetAccessMember(IAuthentication authentication, object target, string memberID, AccessType accessType)
        {
            this.ValidateInvokeMethod(authentication);

            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();

            if (this.TypeCategory != null)
            {
                this.TypeCategory.OnValidateSetAccessMember(authentication, target, memberID, accessType);
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.OnValidateSetAccessMember(authentication, target, memberID, accessType);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateRemoveAccessMember(IAuthentication authentication, object target)
        {
            this.ValidateInvokeMethod(authentication);

            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();

            if (this.TypeCategory != null)
            {
                this.TypeCategory.OnValidateRemoveAccessMember(authentication, target);
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.OnValidateRemoveAccessMember(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateLock(IAuthentication authentication, object target)
        {
            if (target != this && this.IsLocked == true)
                throw new PermissionDeniedException();

            if (this.TypeCategory != null)
            {
                this.TypeCategory.OnValidateLock(authentication, target);
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.OnValidateLock(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateUnlock(IAuthentication authentication, object target)
        {
            if (this.TypeCategory != null)
            {
                this.TypeCategory.OnValidateUnlock(authentication, target);
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.OnValidateUnlock(authentication, target);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateLoad(IAuthentication authentication, object target)
        {
            this.ValidateInvokeMethod(authentication);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateUnload(IAuthentication authentication, object target)
        {
            this.ValidateInvokeMethod(authentication);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateEnter(IAuthentication authentication, object target)
        {
            this.ValidateInvokeMethod(authentication);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateLeave(IAuthentication authentication, object target)
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateRename(IAuthentication authentication, object target, string oldName, string newName)
        {
            this.ValidateInvokeMethod(authentication);
            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();

            if (this.TypeCategory != null)
            {
                this.TypeCategory.OnValidateRename(authentication, target, oldName, newName);
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.OnValidateRename(authentication, target, oldName, newName);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnValidateDelete(IAuthentication authentication, object target)
        {
            this.ValidateInvokeMethod(authentication);
            if (authentication.IsAdmin == false)
                throw new PermissionDeniedException();

            if (this.VerifyAccessType(authentication, AccessType.Master) == false)
                throw new PermissionDeniedException();

            if (this.TypeCategory != null)
            {
                this.TypeCategory.OnValidateDelete(authentication, target);
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.OnValidateDelete(authentication, target);
            }
        }

        protected void UpdateLockParent()
        {
            if (this.LockInfo.UserID != string.Empty)
                this.UpdateLockParent(this);
            else
                this.UpdateLockParent(null);
        }

        protected void UpdateLockParent(ILockParent lockParent)
        {
            if (this.TypeCategory != null)
            {
                this.TypeCategory.LockParent = lockParent;
                if (this.TypeCategory.lockInfo.UserID == string.Empty)
                {
                    this.TypeCategory?.UpdateLockParent(lockParent);
                }
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.LockParent = lockParent;
                if (this.TableCategory.lockInfo.UserID == string.Empty)
                {
                    this.TableCategory.UpdateLockParent(lockParent);
                }
            }
        }

        protected void UpdateAccessParent()
        {
            if (this.accessInfo.UserID != string.Empty)
                this.UpdateAccessParent(this);
            else
                this.UpdateAccessParent(null);
        }

        protected void UpdateAccessParent(IAccessParent accessParent)
        {
            if (this.TypeCategory != null)
            {
                this.TypeCategory.AccessParent = accessParent;
                if (this.TypeCategory.accessInfo.UserID == string.Empty)
                {
                    this.TypeCategory.UpdateAccessParent(accessParent);
                }
                else
                {
                    this.TypeCategory.InvokeAccessChanged(EventArgs.Empty);
                }
            }

            if (this.TableCategory != null)
            {
                this.TableCategory.AccessParent = accessParent;
                if (this.TableCategory.accessInfo.UserID == string.Empty)
                {
                    this.TableCategory.UpdateAccessParent(accessParent);
                }
                else
                {
                    this.TableCategory.InvokeAccessChanged(EventArgs.Empty);
                }
            }
        }

        internal void InvokeAccessChanged(EventArgs e)
        {
            this.OnAccessChanged(e);
        }

        #region ILockParent

        string ILockParent.Path
        {
            get { return this.Name; }
        }

        #endregion
    }
}
