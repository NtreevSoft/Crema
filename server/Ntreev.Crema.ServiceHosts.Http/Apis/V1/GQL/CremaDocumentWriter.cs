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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using GraphQL.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ntreev.Crema.ServiceHosts.Http.Apis.Infrastructures.JsonConverters;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL
{
    [Export(typeof(IDocumentWriter))]
    class CremaDocumentWriter : DocumentWriter
    {
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>()
            {
                new StringEnumConverter(false),
                new FloatPointFormattingConverter()
            }
        };

        public CremaDocumentWriter() : base(Formatting.None, jsonSerializerSettings)
        {
        }
    }
}
