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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Requests.CremaDev
{
    public class DataRequest
    {
        [Required]
        [DefaultValue("bin")]
        public string OutputType { get; set; } = "bin";

        [Required]
        [DefaultValue(false)]
        public bool IsDevMode { get; set; } = false;

        [Required]
        [DefaultValue(-1)]
        public int Revision { get; set; } = -1;

        [Required]
        [DefaultValue("All")]
        public string Tags { get; set; } = "All";

        [Required]
        [DefaultValue(false)]
        public bool Split { get; set; } = false;

        [Required]
        [DefaultValue("dat")]
        public string Ext { get; set; } = "dat";

        [Required]
        [DefaultValue("")]
        public string FilterExpression { get; set; } = "";

        [DefaultValue(null)]
        public long? ReplaceRevision { get; set; } = null;

        [DefaultValue(null)]
        public string ReplaceHashValue { get; set; } = null;

        [Required]
        [DefaultValue("crema.dat")]
        public string ResponseFileName { get; set; } = "crema.dat";
    }
}
