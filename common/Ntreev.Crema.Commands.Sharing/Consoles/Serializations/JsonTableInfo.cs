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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema;
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Ntreev.Crema.Commands.Consoles.Serializations
{
    struct JsonTableInfo
    {
        private string comment;
        private string tags;
        private JsonTableColumnInfo[] members;

        [JsonProperty(Required = Required.Always)]
        [RegularExpression(IdentifierValidator.IdentiFierPattern)]
        public string TableName { get; set; }

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue("All")]
        public string Tags
        {
            get => this.tags ?? "All";
            set => this.tags = value;
        }

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue("")]
        public string Comment
        {
            get => this.comment ?? string.Empty;
            set => this.comment = value;
        }

        [JsonProperty]
        public JsonTableColumnInfo[] Columns
        {
            get => this.members ?? new JsonTableColumnInfo[] { };
            set => this.members = value;
        }

        public static readonly JsonTableInfo Default = new JsonTableInfo()
        {
            TableName = string.Empty,
            Tags = $"{TagInfoUtility.All}",
            Comment = string.Empty,
            Columns = new JsonTableColumnInfo[]
            {
                new JsonTableColumnInfo() { Name = "Key", IsKey = true, IsUnique = true, Comment = string.Empty, DisallowNull = true,  },
                new JsonTableColumnInfo() { Name = "Value", Comment = string.Empty }
            },
        };

        public struct JsonTableColumnInfo
        {
            private string comment;
            private string dataType;
            private string tags;

            [JsonProperty(Required = Required.Always)]
            public string Name { get; set; }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue(false)]
            public bool IsKey { get; set; }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue("string")]
            public string DataType
            {
                get => this.dataType ?? "string";
                set => this.dataType = value;
            }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue("")]
            public string Comment
            {
                get => this.comment ?? string.Empty;
                set => this.comment = value;
            }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue(false)]
            public bool IsUnique { get; set; }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue(false)]
            public bool AutoIncrement { get; set; }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue(null)]
            public string DefaultValue { get; set; }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue("All")]
            public string Tags
            {
                get => this.tags ?? "All";
                set => this.tags = value;
            }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue(false)]
            public bool IsReadOnly { get; set; }

            [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue(false)]
            public bool DisallowNull { get; set; }
        }
    }
}
