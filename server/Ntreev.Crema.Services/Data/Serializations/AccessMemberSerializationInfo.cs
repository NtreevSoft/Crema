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

using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using System;
using System.Runtime.Serialization;

namespace Ntreev.Crema.Services.Data.Serializations
{
    [DataContract(Name = "AccessMemberInfo", Namespace = SchemaUtility.Namespace)]
    [KnownType(typeof(AccessType))]
    struct AccessMemberSerializationInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public SignatureDate SignatureDate { get; set; }

        [IgnoreDataMember]
        public AccessType AccessType { get; set; }

        [DataMember(Name = nameof(AccessType))]
        public string AccessTypeText
        {
            get { return this.AccessType.ToString(); }
            set
            {
                this.AccessType = (AccessType)Enum.Parse(typeof(AccessType), value);
            }
        }

        public static explicit operator AccessMemberInfo(AccessMemberSerializationInfo value)
        {
            var obj = new AccessMemberInfo()
            {
                SignatureDate = value.SignatureDate,
                AccessType = value.AccessType,
            };
            if (obj.SignatureDate.ID == null)
                obj.SignatureDate = new SignatureDate(string.Empty, obj.SignatureDate.DateTime);
            return obj;
        }

        public static explicit operator AccessMemberSerializationInfo(AccessMemberInfo value)
        {
            var obj = new AccessMemberSerializationInfo()
            {
                SignatureDate = value.SignatureDate,
                AccessType = value.AccessType,
            };
            if (obj.SignatureDate.ID == string.Empty && obj.SignatureDate.DateTime == DateTime.MinValue)
                obj.SignatureDate = new SignatureDate();
            return obj;
        }
    }
}
