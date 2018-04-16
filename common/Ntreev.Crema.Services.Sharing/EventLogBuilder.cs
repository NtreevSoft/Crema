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

namespace Ntreev.Crema.Services
{
    static class EventLogBuilder
    {
        public static string Build(Authentication authentication, object target, string methodName, params object[] args)
        {
            if (args.Any() == true)
                return $"[{authentication}] {target.GetType().Name}.{methodName} : {string.Join(", ", args.Select(i => i ?? "null"))}";
            else
                return $"[{authentication}] {target.GetType().Name}.{methodName}";
        }

        public static string BuildMany(Authentication authentication, object target, string methodName, object[] items, params object[] args)
        {
            try
            {
                var sb = new StringBuilder();
                for (var i = 0; i < items.Length; i++)
                {
                    if (i > 0)
                        sb.AppendLine();
                    sb.Append($"[{authentication}] {target.GetType().Name}.{methodName} : {string.Join(", ", Select(i))}");
                }
                return $"{sb}";
            }
            catch (Exception e)
            {
                return $"{e}";
            }

            IEnumerable<string> Select(int index)
            {
                yield return $"{items[index]}";
                foreach (var item in args)
                {
                    yield return $"{item}";
                }
            }
        }

        public static string BuildMany(Authentication authentication, object target, string methodName, params object[][] items)
        {
            try
            {
                var sb = new StringBuilder();
                for (var i = 0; i < items[0].Length; i++)
                {
                    if (i > 0)
                        sb.AppendLine();
                    sb.Append($"[{authentication}] {target.GetType().Name}.{methodName} : {string.Join(", ", Select(i))}");
                }
                return $"{sb}";
            }
            catch (Exception e)
            {
                return $"{e}";
            }

            IEnumerable<string> Select(int index)
            {
                for (var i = 0; i < items.Length; i++)
                {
                    yield return $"{items[i][index]}";
                }
            }
        }
    }
}