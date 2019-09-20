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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.Infrastructures.ActionResults
{
    public class FileResult : IHttpActionResult
    {
        private readonly Stream stream;
        private readonly string filename;
        private readonly string contentType;

        public FileResult(Stream stream, string filename = "untitled", string contentType = "application/octet-stream")
        {
            this.stream = stream;
            this.filename = filename;
            this.contentType = contentType;

            this.stream.Seek(0, SeekOrigin.Begin);
        }

        public virtual Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StreamContent(this.stream);
            message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            message.Content.Headers.ContentDisposition.FileName = this.filename;
            message.Content.Headers.ContentType = new MediaTypeHeaderValue(this.contentType);

            return Task.FromResult(message);
        }
    }
}
