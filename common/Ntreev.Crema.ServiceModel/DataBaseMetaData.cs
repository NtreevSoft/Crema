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

using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.Data;
using Ntreev.Library;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using System.Xml.Schema;

namespace Ntreev.Crema.ServiceModel
{
    [DataContract(Namespace = SchemaUtility.Namespace)]
    public struct DataBaseMetaData
    {
        [DataMember]
        public DataBaseInfo DataBaseInfo { get; set; }

        [DataMember]
        public DataBaseState DataBaseState { get; set; }

        [DataMember]
        public AccessInfo AccessInfo { get; set; }

        [DataMember]
        public LockInfo LockInfo { get; set; }

        [DataMember]
        public TypeMetaData[] Types { get; set; }

        [DataMember]
        public CategoryMetaData[] TypeCategories { get; set; }

        [DataMember]
        public TableMetaData[] Tables { get; set; }

        [DataMember]
        public CategoryMetaData[] TableCategories { get; set; }

        [DataMember]
        public AuthenticationInfo[] Authentications { get; set; }

        public readonly static DataBaseMetaData Empty = new DataBaseMetaData()
        {
            DataBaseInfo = DataBaseInfo.Empty,
            AccessInfo = AccessInfo.Empty,
            LockInfo = LockInfo.Empty,
            Types = new TypeMetaData[] { },
            TypeCategories = new CategoryMetaData[] { },
            Tables = new TableMetaData[] { },
            TableCategories = new CategoryMetaData[] { },
            Authentications = new AuthenticationInfo[] { },
        };
    }
}