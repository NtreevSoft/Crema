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

using Ntreev.Crema.ServiceModel.Properties;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Ntreev.Crema.ServiceModel
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct AccessInfo
    {
        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string ParentPath { get; set; }

        [DataMember]
        public SignatureDate SignatureDate { get; set; }

        [DataMember]
        public AccessMemberInfo[] Members { get; set; }

        [IgnoreDataMember]
        public string UserID { get { return this.SignatureDate.ID; } }

        [IgnoreDataMember]
        public DateTime DateTime { get { return this.SignatureDate.DateTime; } }

        public static bool operator ==(AccessInfo x, AccessInfo y)
        {
            return x.Path == y.Path &&
                   x.ParentPath == y.ParentPath &&
                   x.SignatureDate == y.SignatureDate &&
                   HashUtility.Equals(x.Members, y.Members);
        }

        public static bool operator !=(AccessInfo x, AccessInfo y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (obj is AccessInfo == false)
            {
                return false;
            }

            var accessInfo = (AccessInfo)obj;
            return accessInfo.Path == this.Path &&
                   accessInfo.ParentPath == this.ParentPath &&
                   accessInfo.SignatureDate == this.SignatureDate &&
                   HashUtility.Equals(accessInfo.Members, this.Members);
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(this.Path, this.ParentPath, this.SignatureDate, this.Members);
        }

        public AccessType GetAccessType(string memberID)
        {
            if (this.UserID == string.Empty)
                return AccessType.Owner;
            foreach (var item in this.Members)
            {
                if (NameValidator.VerifyCategoryPath(item.UserID) == true)
                {
                    if (memberID.StartsWith(item.UserID) == true)
                        return item.AccessType;
                }
                else
                {
                    if (item.UserID == memberID)
                        return item.AccessType;
                }
            }
            return AccessType.None;
        }

        public bool VerifyAccessType(string memberID, AccessType accessType)
        {
            return this.GetAccessType(memberID).HasFlag(accessType);
        }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { $"{nameof(this.Path)}", this.Path },
                { $"{nameof(this.ParentPath)}", this.ParentPath },
                { $"{nameof(this.UserID)}", this.UserID },
                { $"{nameof(this.DateTime)}", this.DateTime },
                { $"{nameof(this.Members)}", this.GetMembersInfo() },
            };
            return props;
        }

        public bool IsOwner(string memberID)
        {
            if (this.IsPrivate == false)
                return false;
            return this.OwnerID == memberID;
        }

        public bool IsMember(string memberID)
        {
            if (this.IsPrivate == false)
                return false;
            return this.Contains(memberID);
        }

        public bool CanPrivate(string userID)
        {
            if (this.IsPrivate == false)
                return true;
            if (this.IsInherited == false)
                return true;
            return this.IsOwner(userID);
        }

        public bool CanPublic(string userID)
        {
            if (this.IsPrivate == false)
                return false;
            if (this.IsInherited == true)
                return false;
            return this.IsOwner(userID);
        }

        public bool CanSet(string userID)
        {
            if (this.IsPrivate == false)
                return false;
            if (this.IsInherited == true)
                return false;
            return this.GetAccessType(userID) >= AccessType.Master;
        }

        public static readonly AccessInfo Empty = new AccessInfo()
        {
            Path = string.Empty,
            ParentPath = string.Empty,
            Members = new AccessMemberInfo[] { },
            SignatureDate = SignatureDate.Empty
        };

        [IgnoreDataMember]
        public bool IsInherited
        {
            get { return this.ParentPath != string.Empty; }
        }

        [IgnoreDataMember]
        public bool IsPrivate
        {
            get { return this.UserID != string.Empty; }
        }

        private object[] GetMembersInfo()
        {
            var items = new object[this.Members.Length];
            for (var i = 0; i < this.Members.Length; i++)
            {
                items[i] = this.Members[i].ToDictionary();
            }
            return items;
        }

        internal void SetPublic()
        {
            this = AccessInfo.Empty;
        }

        internal void SetPrivate(string path, SignatureDate signatureDate)
        {
            this.SignatureDate = signatureDate;
            this.Path = path;
            this.ParentPath = string.Empty;
            this.Add(signatureDate, signatureDate.ID, AccessType.Owner);
        }

        internal void Add(SignatureDate signatureDate, string memberID, AccessType accessType)
        {
            if (this.Contains(memberID) == true)
                throw new ArgumentException();

            var members = this.Members.ToList();
            members.Add(new AccessMemberInfo() { SignatureDate = new SignatureDate(memberID, signatureDate.DateTime), AccessType = accessType });
            this.Members = members.ToArray();
        }

        internal void Set(SignatureDate signatureDate, string memberID, AccessType accessType)
        {
            if (this.Contains(memberID) == false)
                throw new ArgumentException();

            var ownerID = this.OwnerID;

            for (var i = 0; i < this.Members.Length; i++)
            {
                if (this.Members[i].UserID == memberID)
                {
                    this.Members[i].AccessType = accessType;
                    this.Members[i].SignatureDate = new SignatureDate(memberID, signatureDate.DateTime);
                    break;
                }
            }

            if (accessType != AccessType.Owner || ownerID == memberID)
                return;

            for (var i = 0; i < this.Members.Length; i++)
            {
                if (this.Members[i].UserID == ownerID)
                {
                    this.Members[i].AccessType = AccessType.Master;
                    this.Members[i].SignatureDate = new SignatureDate(ownerID, signatureDate.DateTime);
                    break;
                }
            }
        }

        internal void Remove(SignatureDate signatureDate, string memberID)
        {
            if (this.Contains(memberID) == false)
                throw new ArgumentException();

            var member = this.Members.First(item => item.UserID == memberID);
            if (member.AccessType == AccessType.Owner)
                throw new ArgumentException();

            this.Members = this.Members.Where(item => item.UserID != memberID).ToArray();
        }

        internal bool Contains(string memberID)
        {
            for (var i = 0; i < this.Members.Length; i++)
            {
                if (this.Members[i].UserID == memberID)
                {
                    return true;
                }
            }
            return false;
        }

        internal string OwnerID
        {
            get
            {
                foreach (var item in this.Members)
                {
                    if (item.AccessType == AccessType.Owner)
                        return item.UserID;
                }
                throw new InvalidOperationException();
            }
            set
            {
                for (var i = 0; i < this.Members.Length; i++)
                {
                    if (this.Members[i].UserID == value)
                    {
                        this.Members[i].AccessType = AccessType.Owner;
                        return;
                    }
                }
                throw new InvalidOperationException();
            }
        }
    }
}
