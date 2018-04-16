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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServiceModel
{
    public static class AddressUtility
    {
        public const int DefaultPort = 4004;

        public static string GetDisplayAddress(string address)
        {
            var match = Regex.Match(address, "(?<name>[^:]*)[:]*(?<port>\\d*)", RegexOptions.ExplicitCapture);
            if (match.Groups["port"].Value != string.Empty)
            {
                var port = int.Parse(match.Groups["port"].Value);
                if (port == DefaultPort)
                    return match.Groups["name"].Value;
                return address;
            }
            return address;
        }

        public static string ConnectionAddress(string address)
        {
            var match = Regex.Match(address, "(?<name>[^:]*)[:]*(?<port>\\d*)", RegexOptions.ExplicitCapture);
            if (match.Groups["port"].Value == string.Empty)
            {
                return address + ":" + 4004;
            }
            return address;
        }

        public static int GetPort(string address)
        {
            var match = Regex.Match(address, "(?<name>[^:]*)[:]*(?<port>\\d*)", RegexOptions.ExplicitCapture);
            if (match.Groups["port"].Value == string.Empty)
            {
                return DefaultPort;
            }
            return int.Parse(match.Groups["port"].Value);
        }

        public static string GetIPAddress(string address)
        {
            var match = Regex.Match(address, "(?<name>[^:]*)[:]*(?<port>\\d*)", RegexOptions.ExplicitCapture);
            return match.Groups["name"].Value;
        }
    }
}
