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

using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Ntreev.Crema.Data.Xml;
using System.Collections.Generic;
using System;
using System.Linq;
using Ntreev.Library;
using System.Xml.Schema;
using Ntreev.Library.Serialization;
using System.Text;
using System.IO;

namespace Ntreev.Crema.ServiceModel
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    [XmlInclude(typeof(DBNull))]
    [XmlInclude(typeof(TimeSpan))]
    public struct DomainLocationInfo
    {
        private object[] keys;

        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public string ColumnName { get; set; }

        [IgnoreDataMember]
        public object[] Keys
        {
            get { return this.keys; }
            set { this.keys = value; }
        }

        [DataMember]
        public DomainFieldInfo[] KeyInfos
        {
            get { return this.keys != null ? this.keys.Select(item => new DomainFieldInfo(item)).ToArray() : new DomainFieldInfo[] { }; }
            set { this.keys = value?.Select(item => item.ToValue()).ToArray(); }
        }

        public static readonly DomainLocationInfo Empty = new DomainLocationInfo() { Keys = new object[] { } };

        public static bool operator ==(DomainLocationInfo left, DomainLocationInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DomainLocationInfo left, DomainLocationInfo right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DomainLocationInfo))
            {
                return false;
            }
            var location = (DomainLocationInfo)obj;
            return HashUtility.Equals(location.Keys, this.Keys) &&
                   location.TableName == this.TableName &&
                   location.ColumnName == this.ColumnName;
        }

        public override int GetHashCode()
        {
            return HashUtility.GetHashCode(this.Keys, this.ColumnName, this.TableName);
        }
    }
}
