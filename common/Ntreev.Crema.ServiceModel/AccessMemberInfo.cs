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
    public struct AccessMemberInfo
    {
        [DataMember]
        public SignatureDate SignatureDate { get; set; }

        [DataMember]
        public AccessType AccessType { get; set; }

        [IgnoreDataMember]
        public string UserID { get { return this.SignatureDate.ID; } }

        [IgnoreDataMember]
        public DateTime DateTime { get { return this.SignatureDate.DateTime; } }

        public static bool operator ==(AccessMemberInfo x, AccessMemberInfo y)
        {
            return x.SignatureDate == y.SignatureDate &&
                   x.AccessType == y.AccessType;
        }

        public static bool operator !=(AccessMemberInfo x, AccessMemberInfo y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (obj is AccessMemberInfo == false)
            {
                return false;
            }

            var memberInfo = (AccessMemberInfo)obj;
            return memberInfo.SignatureDate == this.SignatureDate &&
                   memberInfo.AccessType == this.AccessType;
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(this.SignatureDate, this.AccessType);
        }

        public IDictionary<string, object> ToDictionary()
        {
            var props = new Dictionary<string, object>
            {
                { nameof(this.UserID), this.UserID },
                { nameof(this.DateTime), this.DateTime },
                { nameof(this.AccessType), this.AccessType },
            };
            return props;
        }

        public static readonly AccessMemberInfo Empty = new AccessMemberInfo()
        {
            AccessType = AccessType.Owner,
            SignatureDate = SignatureDate.Empty
        };
    }
}
