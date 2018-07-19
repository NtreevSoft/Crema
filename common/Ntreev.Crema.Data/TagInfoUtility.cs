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

using Ntreev.Library;

namespace Ntreev.Crema.Data
{
    public static class TagInfoUtility
    {
        private const string serverString = "Server";
        private const string clientString = "Client";
        private const string unusedString = "Unused";

        private static string[] names;

        static TagInfoUtility()
        {
            TagInfo.SetColor(serverString, "#FFFBBEBE");
            TagInfo.SetColor(clientString, "#FF99B7FB");
            TagInfo.SetColor(unusedString, "#717171");
        }

        public readonly static TagInfo All = TagInfo.All;

        public readonly static TagInfo Server = new TagInfo(serverString);

        public readonly static TagInfo Client = new TagInfo(clientString);

        public readonly static TagInfo Unused = TagInfo.Unused;

        public static string[] Names
        {
            get
            {
                if (names == null)
                {
                    names = new string[] { nameof(All), nameof(Server), nameof(Client), nameof(Unused) };
                }
                return names;
            }
        }
    }
}
