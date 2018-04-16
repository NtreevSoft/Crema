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

using FirstFloor.ModernUI.Presentation;
using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ntreev.Crema.Client.Framework
{
    public static class IDomainUserExtensions
    {
        private static List<Color> colors = new List<Color>() {
            Color.FromRgb(0xa4, 0xc4, 0x00),   // lime
            Color.FromRgb(0x60, 0xa9, 0x17),   // green
            Color.FromRgb(0x00, 0x8a, 0x00),   // emerald
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x1b, 0xa1, 0xe2),   // cyan
            Color.FromRgb(0x00, 0x50, 0xef),   // cobalt
            Color.FromRgb(0x6a, 0x00, 0xff),   // indigo
            Color.FromRgb(0xaa, 0x00, 0xff),   // violet
            Color.FromRgb(0xf4, 0x72, 0xd0),   // pink
            Color.FromRgb(0xd8, 0x00, 0x73),   // magenta
            Color.FromRgb(0xa2, 0x00, 0x25),   // crimson
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xfa, 0x68, 0x00),   // orange
            Color.FromRgb(0xf0, 0xa3, 0x0a),   // amber
            Color.FromRgb(0xe3, 0xc8, 0x00),   // yellow
            Color.FromRgb(0x82, 0x5a, 0x2c),   // brown
            Color.FromRgb(0x6d, 0x87, 0x64),   // olive
            Color.FromRgb(0x64, 0x76, 0x87),   // steel
            Color.FromRgb(0x76, 0x60, 0x8a),   // mauve
            Color.FromRgb(0x87, 0x79, 0x4e),   // taupe
        };

        private static List<Color> emptyColors = new List<Color>();
        private static Random random = new Random(DateTime.Now.Millisecond);

        private static Dictionary<string, Color> idToColor = new Dictionary<string, Color>();

        public static Color GetColor(this IDomainUser domainUser)
        {
            if (idToColor.ContainsKey(domainUser.ID) == false)
            {
                if (emptyColors.Count == 0)
                {
                    emptyColors.AddRange(colors);
                    emptyColors.Remove(AppearanceManager.Current.AccentColor);
                }

                int index = random.Next() % emptyColors.Count;
                idToColor.Add(domainUser.ID, emptyColors[index]);
                emptyColors.RemoveAt(index);
                
            }
            return idToColor[domainUser.ID];
        }

        public static Color GetColor(this DomainUserInfo domainUserInfo)
        {
            if (idToColor.ContainsKey(domainUserInfo.UserID) == false)
            {
                if (emptyColors.Count == 0)
                {
                    emptyColors.AddRange(colors);
                    emptyColors.Remove(AppearanceManager.Current.AccentColor);
                }

                int index = random.Next() % emptyColors.Count;
                idToColor.Add(domainUserInfo.UserID, emptyColors[index]);
                emptyColors.RemoveAt(index);

            }
            return idToColor[domainUserInfo.UserID];
        }

        public static void Clear()
        {
            idToColor.Clear();
            emptyColors.Clear();
        }

        public static void Start(string clientUserID)
        {
            idToColor.Add(clientUserID, AppearanceManager.Current.AccentColor);
        }
    }
}
