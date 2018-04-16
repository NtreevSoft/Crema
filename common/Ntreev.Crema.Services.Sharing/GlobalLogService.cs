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
using System.Text;

namespace Ntreev.Crema.Services.Log
{
    [Export(typeof(ILogService))]
    class GlobalLogService : ILogService
    {
        public void Debug(object message)
        {
            CremaLog.Debug(message);
        }

        public void Error(object message)
        {
            CremaLog.Error(message);
        }

        public void Fatal(object message)
        {
            CremaLog.Fatal(message);
        }

        public void Info(object message)
        {
            CremaLog.Info(message);
        }

        public void Warn(object message)
        {
            CremaLog.Warn(message);
        }

        public LogVerbose Verbose
        {
            get => CremaLog.LogService.Verbose;
            set => CremaLog.LogService.Verbose = value;
        }

        public TextWriter RedirectionWriter
        {
            get => CremaLog.LogService.RedirectionWriter;
            set => CremaLog.LogService.RedirectionWriter = value;
        }

        public string Name => CremaLog.LogService.Name;

        public string FileName => CremaLog.LogService.FileName;

        public bool IsEnabled => true;
    }
}
