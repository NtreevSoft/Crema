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
using Ntreev.Library;
using Ntreev.Library.Serialization;
using System;
using System.Runtime.Serialization;

namespace Ntreev.Crema.Services.Data.Serializations
{
    [DataContract(Name = "Row", Namespace = SchemaUtility.Namespace)]
    struct FindRowSerializationInfo
    {
        public FindRowSerializationInfo(CremaDataRow row, CremaDataColumn[] columns)
            : this()
        {
            var values = new string[columns.Length];

            this.Tags = row.DerivedTags.ToString();
            this.IsEnabled = row.IsEnabled;
            this.ModificationInfo = row.ModificationInfo;

            for (var i = 0; i < columns.Length; i++)
            {
                var value = row[i];
                if (value == DBNull.Value)
                {
                    values[i] = null;
                }
                else
                {
                    values[i] = value.ToString();
                }
            }

            this.Values = new SerializationItemCollection<string>(values);
        }

        [DataMember]
        public SerializationItemCollection<string> Values { get; set; }

        [DataMember]
        public string Tags { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public SignatureDate ModificationInfo { get; set; }
    }
}
