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

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Ntreev.Crema.Services;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    /// <summary>
    /// 로그를 기록할 수 있는 도구입니다. 
    /// 이 정적 클래스는 전역으로 기록되는 로그이므로
    /// 저장소별로 로그를 기록하기 위해서는 ICremaHost 에서 ILogService 형태의 서비스를 취득하여 사용하시기 바랍니다.
    /// 전역 로그 파일은 AppData\Roaming\NtreevSoft\cremaservice
    /// 서비스 실행시 전역 로그 파일은 C:\Windows\System32\config\systemprofile\AppData\Roaming\NtreevSoft\cremaservice
    /// </summary>
    public static class CremaLog
    {
        private static LogService log;

        static CremaLog()
        {
            
        }

        public static void Release()
        {
            log4net.LogManager.Shutdown();
        }

        public static void Debug(object message)
        {
            LogService.Debug(message);
        }

        public static void Info(object message)
        {
            LogService.Info(message);
        }

        public static void Error(object message)
        {
            LogService.Error(message);
        }

        public static void Warn(object message)
        {
            LogService.Warn(message);
        }

        public static void Fatal(object message)
        {
            LogService.Fatal(message);
        }

        public static void Debug(string format, params object[] args)
        {
            LogService.Debug(string.Format(format, args));
        }

        public static void Info(string format, params object[] args)
        {
            LogService.Info(string.Format(format, args));
        }

        public static void Error(string format, params object[] args)
        {
            LogService.Error(string.Format(format, args));
        }

        public static void Error(Exception e)
        {
            LogService.Error(e);
            LogService.Debug(e);
        }

        public static void Warn(string format, params object[] args)
        {
            LogService.Warn(string.Format(format, args));
        }

        public static void Fatal(string format, params object[] args)
        {
            LogService.Fatal(string.Format(format, args));
        }

        public static TextWriter RedirectionWriter
        {
            get { return LogService.RedirectionWriter; }
            set { LogService.RedirectionWriter = value; }
        }

        public static LogVerbose Verbose
        {
            get { return LogService.Verbose; }
            set { LogService.Verbose = value; }
        }

        internal static LogService LogService
        {
            get
            {
                if (log == null)
                    log = new LogService();
                return log;
            }
        }
    }
}
