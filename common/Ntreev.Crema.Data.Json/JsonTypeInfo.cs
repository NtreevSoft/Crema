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
