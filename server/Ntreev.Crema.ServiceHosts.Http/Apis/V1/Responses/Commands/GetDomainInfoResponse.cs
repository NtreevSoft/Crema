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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceModel;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.Commands
{
    public class GetDomainInfoResponse
    {
        public Guid DomainId { get; set; }
        public Guid DataBaseId { get; set; }
        public string ItemPath { get; set; }
        public string ItemType { get; set; }
        public string DomainType { get; set; }
        public string CategoryPath { get; set; }
        [JsonProperty(CremaSchema.Creator)]
        public string Creator { get; set; }
        [JsonProperty(CremaSchema.CreatedDateTime)]
        public DateTime CreatedDateTime { get; set; }
        [JsonProperty(CremaSchema.Modifier)]
        public string Modifier { get; set; }
        [JsonProperty(CremaSchema.ModifiedDateTime)]
        public DateTime ModifiedDateTime { get; set; }

        public static GetDomainInfoResponse ConvertFrom(DomainInfo domainInfo)
        {
            return new GetDomainInfoResponse
            {
                DomainId = domainInfo.DomainID,
                DataBaseId = domainInfo.DataBaseID,
                ItemPath = domainInfo.ItemPath,
                ItemType = domainInfo.ItemType,
                DomainType = domainInfo.DomainType,
                CategoryPath = domainInfo.CategoryPath,
                Creator = domainInfo.CreationInfo.ID,
                CreatedDateTime = domainInfo.CreationInfo.DateTime,
                Modifier = domainInfo.ModificationInfo.ID,
                ModifiedDateTime = domainInfo.ModificationInfo.DateTime
            };
        }
    }
}
