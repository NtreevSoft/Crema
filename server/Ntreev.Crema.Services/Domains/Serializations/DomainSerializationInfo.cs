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
using System.Linq;
using System.Runtime.Serialization;

namespace Ntreev.Crema.Services.Domains.Serializations
{
    [DataContract(Name = "DomainHeaderInfo", Namespace = SchemaUtility.Namespace)]
    struct DomainSerializationInfo
    {
        [DataMember]
        public string DomainType { get; set; }

        [DataMember]
        public string SourceType { get; set; }

        [DataMember]
        public DomainInfo DomainInfo { get; set; }

        [DataMember]
        public DomainUserInfo[] UserInfos { get; set; }

        [DataMember]
        public DomainPropertySerializationInfo[] Properties { get; set; }

        public void AddProperty(string name, object value)
        {
            var property = new DomainPropertySerializationInfo(name, value);
            var propertyList = (this.Properties ?? new DomainPropertySerializationInfo[] { }).ToList();
            propertyList.Add(property);
            this.Properties = propertyList.ToArray();
        }

        public bool ContainsProperty(string name)
        {
            if (this.Properties == null)
                return false;

            foreach (var item in this.Properties)
            {
                if (item.Name == name)
                    return true;
            }
            return false;
        }

        public object GetProperty(string name)
        {
            if (this.Properties == null)
                throw new InvalidOperationException();

            foreach (var item in this.Properties)
            {
                if (item.Name == name)
                {
                    return item.ToValue();
                }
            }
            throw new InvalidOperationException();
        }
    }
}
