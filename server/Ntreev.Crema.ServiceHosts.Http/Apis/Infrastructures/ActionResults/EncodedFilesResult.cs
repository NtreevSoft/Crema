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

using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.CremaDev;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.Infrastructures.ActionResults
{
    public class EncodedFilesResult : IHttpActionResult
    {
        private readonly ZipFileResultEntry[] entries;
        private readonly MediaTypeFormatter formatter;

        public EncodedFilesResult(ZipFileResultEntry[] entries, MediaTypeFormatter formatter)
        {
            this.entries = entries;
            this.formatter = formatter;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new DataResponse();

            foreach(var entry in this.entries)
            {
                using (var ms = new MemoryStream())
                {
                    entry.Stream.Seek(0, SeekOrigin.Begin);
                    entry.Stream.CopyTo(ms);
                    response.Files.Add(new DataFileResponse
                    {
                        Path = entry.FileName,
                        Data = Convert.ToBase64String(ms.ToArray())
                    });
                }
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<DataResponse>(response, this.formatter)
            });
        }
    }
}
