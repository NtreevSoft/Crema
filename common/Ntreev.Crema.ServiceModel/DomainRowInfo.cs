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
using System.Xml.Serialization;
using Ntreev.Crema.Data.Xml;
using System.Collections.Generic;
using System;
using System.Linq;
using Ntreev.Library;
using System.Xml.Schema;
using System.Xml;
using Ntreev.Library.Serialization;
using System.Text;
using System.IO;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.ServiceModel
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct DomainRowInfo
    {
        private object[] fields;
        private object[] keys;

        [DataMember]
        public string TableName { get; set; }

        [IgnoreDataMember]
        public object[] Fields
        {
            get { return this.fields; }
            set { this.fields = value; }
        }

        [IgnoreDataMember]
        public object[] Keys
        {
            get { return this.keys; }
            set { this.keys = value; }
        }

        [DataMember]
        public DomainFieldInfo[] FieldInfos
        {
            get { return this.fields != null ? this.fields.Select(item => new DomainFieldInfo(item)).ToArray() : new DomainFieldInfo[] { }; }
            set { this.fields = value?.Select(item => item.ToValue()).ToArray(); }
        }

        [DataMember]
        public DomainFieldInfo[] KeyInfos
        {
            get { return this.keys != null ? this.keys.Select(item => new DomainFieldInfo(item)).ToArray() : new DomainFieldInfo[] { }; }
            set { this.keys = value?.Select(item => item.ToValue()).ToArray(); }
        }

        public static readonly DomainRowInfo Empty;

        internal static readonly string[] ClearKey = new string[] { "6B924529-8134-463D-A040-1632BCE6813A", "F1405371-7961-4A6D-9DDA-D66838617F41" };
    }
}
