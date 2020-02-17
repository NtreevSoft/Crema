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
    public static class CremaFeatures
    {
        public static bool SupportsToastMessage(Version clientVersion)
        {
            if (clientVersion == null) return false;

            if (clientVersion.Major == 3 && clientVersion.Minor == 6)
            {
                return clientVersion > Version.Parse("3.6.19273.1701");
            }
            else if (clientVersion.Major == 3 && clientVersion.Minor == 7)
            {
                return clientVersion > Version.Parse("3.7.19273.1701");
            }
            else if (clientVersion.Major > 3)
            {
                return true;
            }

            return false;
        }

        public static bool SupportsTableDetailInfo(Version clientVersion)
        {
            if (clientVersion == null) return false;

            if (clientVersion.Major == 3 && clientVersion.Minor == 6)
            {
                return clientVersion > Version.Parse("3.6.20037.1540");
            }
            else if (clientVersion.Major == 3 && clientVersion.Minor == 7)
            {
                return clientVersion > Version.Parse("3.7.20036.1328");
            }
            else if (clientVersion.Major > 3)
            {
                return true;
            }

            return false;
        }
    }
}
