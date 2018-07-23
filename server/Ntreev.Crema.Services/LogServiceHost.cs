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

using System.IO;

namespace Ntreev.Crema.Services
{
    class LogServiceHost : ILogService
    {
        private readonly LogService logService;

        public LogServiceHost(LogService logService)
        {
            this.logService = logService;
        }

        public LogServiceHost(string name, string path, bool isSingle)
        {
            this.logService = new LogService(name, path, isSingle);
        }

        public LogVerbose Verbose
        {
            get => this.logService.Verbose;
            set => this.logService.Verbose = value;
        }

        public TextWriter RedirectionWriter
        {
            get => this.logService.RedirectionWriter;
            set => this.logService.RedirectionWriter = value;
        }

        public string Name => this.logService.Name;

        public string FileName => this.logService.FileName;

        public bool IsEnabled => true;

        public void Debug(object message)
        {
            this.logService.Debug(message);
        }

        public void Error(object message)
        {
            this.logService.Error(message);
        }

        public void Fatal(object message)
        {
            this.logService.Fatal(message);
        }

        public void Info(object message)
        {
            this.logService.Info(message);
        }

        public void Warn(object message)
        {
            this.logService.Warn(message);
        }
    }
}
