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

using Newtonsoft.Json;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Json
{
    public struct JsonTypeInfo
    {
        private string comment;
        private JsonTypeMemberInfo[] members;

        [JsonProperty(Required = Required.Always)]
        [RegularExpression(IdentifierValidator.IdentiFierPattern)]
        public string TypeName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsFlag { get; set; }

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue("")]
        public string Comment
        {
            get => this.comment ?? string.Empty;
            set => this.comment = value;
        }

        [JsonProperty]
        public JsonTypeMemberInfo[] Members
        {
            get => this.members ?? new JsonTypeMemberInfo[] { };
            set => this.members = value;
        }

        public static readonly JsonTypeInfo Default = new JsonTypeInfo()
        {
            TypeName = string.Empty,
            IsFlag = false,
            Comment = string.Empty,
            Members = new JsonTypeMemberInfo[] { new JsonTypeMemberInfo() { Name = "Member0", Value = 0, Comment = string.Empty } },
        };

        public struct JsonTypeMemberInfo
        {
            private string comment;

            [JsonProperty(Required = Required.Always)]
            [Description("qwer")]
            [RegularExpression(IdentifierValidator.IdentiFierPattern)]
            public string Name { get; set; }

            [JsonProperty(Required = Required.Always)]
            public long Value { get; set; }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue("")]
            public string Comment
            {
                get => this.comment ?? string.Empty;
                set => this.comment = value;
            }
        }
    }
}
