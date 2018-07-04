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
    [DataContract(Name = nameof(DataBaseInfo), Namespace = SchemaUtility.Namespace)]
    struct DataBaseSerializationInfo
    {
        public const string Extension = ".info";

        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string Revision { get; set; }

        [DataMember]
        public string[] Paths { get; set; }

        [DataMember]
        public SignatureDate CreationInfo { get; set; }

        [DataMember]
        public SignatureDate ModificationInfo { get; set; }

        public static explicit operator DataBaseInfo(DataBaseSerializationInfo obj)
        {
            return new DataBaseInfo()
            {
                ID = obj.ID,
                Name = obj.Name,
                Comment = obj.Comment,
                Revision = obj.Revision,
                Paths = obj.Paths,
                CreationInfo = obj.CreationInfo,
                ModificationInfo = obj.ModificationInfo,
            };
        }

        public static explicit operator DataBaseSerializationInfo(DataBaseInfo obj)
        {
            return new DataBaseSerializationInfo()
            {
                ID = obj.ID,
                Name = obj.Name,
                Comment = obj.Comment,
                Revision = obj.Revision,
                Paths = obj.Paths,
                CreationInfo = obj.CreationInfo,
                ModificationInfo = obj.ModificationInfo,
            };
        }

        public static readonly ObjectSerializerSettings Settings = new ObjectSerializerSettings() { Extension = Extension };
    }
}
