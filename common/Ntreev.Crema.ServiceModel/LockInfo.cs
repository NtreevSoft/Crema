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
    public struct LockInfo
    {
        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string ParentPath { get; set; }

        [DataMember]
        public SignatureDate SignatureDate { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [IgnoreDataMember]
        public string UserID { get { return this.SignatureDate.ID; } }

        [IgnoreDataMember]
        public DateTime DateTime { get { return this.SignatureDate.DateTime; } }

        public static bool operator ==(LockInfo x, LockInfo y)
        {
            return ((x.Path == y.Path) &&
                    (x.ParentPath == y.ParentPath) &&
                    (x.SignatureDate == y.SignatureDate) &&
                    (x.Comment == y.Comment));
        }

        public static bool operator !=(LockInfo x, LockInfo y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (obj is LockInfo == false)
            {
                return false;
            }

            var lockInfo = (LockInfo)obj;
            return ((lockInfo.Path == this.Path) &&
                    (lockInfo.ParentPath == this.ParentPath) &&
                    (lockInfo.SignatureDate == this.SignatureDate) &&
                    (lockInfo.Comment == this.Comment));
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(this.Path, this.ParentPath, this.SignatureDate, this.Comment);
        }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.Path), this.Path },
                { nameof(this.ParentPath), this.ParentPath },
                { nameof(this.Comment), this.Comment },
                { nameof(this.UserID), this.UserID },
                { nameof(this.DateTime), this.DateTime },
            };
            return props;
        }

        public bool IsOwner(string userID)
        {
            return this.UserID == userID;
        }

        public bool CanLock(string userID)
        {
            if (this.IsLocked == false)
                return true;
            if (this.IsInherited == false)
                return true;
            return this.IsOwner(userID);
        }

        public bool CanUnlock(string userID)
        {
            if (this.IsLocked == false)
                return false;
            if (this.IsInherited == true)
                return false;
            return this.IsOwner(userID);
        }

        public static readonly LockInfo Empty = new LockInfo()
        {
            Path = string.Empty,
            ParentPath = string.Empty,
            Comment = string.Empty,
            SignatureDate = SignatureDate.Empty
        };

        [IgnoreDataMember]
        public bool IsInherited
        {
            get { return this.ParentPath != string.Empty; }
        }

        [IgnoreDataMember]
        public bool IsLocked
        {
            get { return this.UserID != string.Empty; }
        }
    }
}
