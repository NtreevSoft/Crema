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
using Newtonsoft.Json.Schema.Generation;
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
    struct JsonUserInfo
    {
        [JsonProperty(Required = Required.Always)]
        [RegularExpression(IdentifierValidator.IdentiFierPattern)]
        public string UserID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Password { get; set; }

        [JsonProperty(Required = Required.Always)]
        [EnumDataType(typeof(Authority))]
        public string Authority { get; set; }

        [JsonProperty(Required = Required.Always)]
        [MinLength(1)]
        public string UserName { get; set; }

        [JsonProperty]
        public bool AllowMultiLogin { get; set; }

        [JsonProperty(Required = Required.DisallowNull, DefaultValueHandling = DefaultValueHandling.Ignore)]
        //[JSchemaGenerationProvider(typeof(EnumStringGenerationProvider), new object[] { nameof(CategoryPaths) })]
        public string CategoryPath { get; set; }

        //public static string[] CategoryPaths { get; set; }

        public static readonly JsonUserInfo Default = new JsonUserInfo()
        {
            UserID = string.Empty,
            Password = string.Empty,
            Authority = $"{ServiceModel.Authority.Member}",
            UserName = string.Empty,
            AllowMultiLogin = false
        };
    }
}
