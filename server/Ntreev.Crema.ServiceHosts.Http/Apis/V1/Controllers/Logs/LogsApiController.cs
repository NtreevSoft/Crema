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
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting.Services;
using Ntreev.Crema.ServiceHosts.Http.Apis.Infrastructures.ActionResults;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.Responses.Logs;
using Ntreev.Crema.Services;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.Controllers.Logs
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/v1/logs")]
    public class LogsApiController : CremaApiController
    {
        private readonly ICremaHost cremaHost;

        [ImportingConstructor]
        public LogsApiController(ICremaHost cremaHost) : base(cremaHost)
        {
            this.cremaHost = cremaHost;
        }

        [HttpGet]
        [Route("")]
        public Task<LogNameResponse[]> GetLogsAsync()
        {
            var logs = GetLogServices();
            return Task.FromResult(LogNameResponse.ConvertFrom(logs));
        }

        [HttpGet]
        [Route("{logName}/files")]
        [CremaAuthorize(Roles = "Admin")]
        public Task<string[]> GetLogFilesAsync(string logName)
        {
            if (string.IsNullOrWhiteSpace(logName))
                throw new ArgumentException(nameof(logName));

            var log = GetLogServices().FirstOrDefault(service => service.Name == logName);
            if (log == null)
                throw new KeyNotFoundException(nameof(log));

            var dir = Path.GetDirectoryName(log.FileName);
            return Task.FromResult(Directory.GetFiles(dir).Select(Path.GetFileName).ToArray());
        }

        [HttpGet]
        [Route("{logName}/files/{filename}")]
        [CremaAuthorize(Roles = "Admin")]
        public Task<IHttpActionResult> GetLogContentAsync(string logName, string filename, bool download = false)
        {
            if (string.IsNullOrWhiteSpace(logName))
                throw new ArgumentException(nameof(logName));

            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException(nameof(filename));

            var log = GetLogServices().FirstOrDefault(service => service.Name == logName);
            if (log == null)
                throw new KeyNotFoundException(nameof(log));

            var dir = Path.GetDirectoryName(log.FileName);
            var path = Path.Combine(dir, filename);
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            return download
                ? Task.FromResult((IHttpActionResult)new FileResult(File.OpenRead(path), filename, "application/xml"))
                : Task.FromResult((IHttpActionResult)Ok(File.ReadAllText(path)));
        }

        private IEnumerable<ILogService> GetLogServices()
        {
            return cremaHost.GetService<IEnumerable<ILogService>>();
        }
    }
}
