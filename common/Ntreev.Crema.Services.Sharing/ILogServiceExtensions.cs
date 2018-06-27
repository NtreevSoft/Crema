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

namespace Ntreev.Crema.Services
{
    public static partial class ILogServiceExtensions
    {
        public static void Debug(this ILogService logService, string format, params object[] args)
        {
            logService.Debug(string.Format(format, args));
        }

        public static void Info(this ILogService logService, string format, params object[] args)
        {
            logService.Info(string.Format(format, args));
        }

        public static void Error(this ILogService logService, string format, params object[] args)
        {
            logService.Error(string.Format(format, args));
        }

        public static void Error(this ILogService logService, Exception e)
        {
            logService.Error(e);
        }

        public static void Warn(this ILogService logService, string format, params object[] args)
        {
            logService.Warn(string.Format(format, args));
        }

        public static void Fatal(this ILogService logService, string format, params object[] args)
        {
            logService.Fatal(string.Format(format, args));
        }

        internal static void DebugMethod(this ILogService logService, Authentication authentication, object target, string methodName, params object[] args)
        {
            logService.Debug(EventLogBuilder.Build(authentication, target, methodName, args));
        }

        internal static void InfoMethod(this ILogService logService, Authentication authentication, object target, string methodName, params object[] args)
        {
            logService.Info(EventLogBuilder.Build(authentication, target, methodName, args));
        }

        internal static void DebugMethodMany(this ILogService logService, Authentication authentication, object target, string methodName, params object[][] items)
        {
            logService.Debug(EventLogBuilder.BuildMany(authentication, target, methodName, items));
        }

        internal static void InfoMethodMany(this ILogService logService, Authentication authentication, object target, string methodName, params object[][] items)
        {
            logService.Info(EventLogBuilder.BuildMany(authentication, target, methodName, items));
        }
    }
}
