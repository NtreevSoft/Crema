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

using Ntreev.Crema.Services;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ntreev.Crema.Services.Random
{
    public static class TypeMemberExtensions
    {
        public static void InitializeRandom(this ITypeMember typeMember, Authentication authentication)
        {
            if (typeMember.Template.IsFlag == true)
                typeMember.SetValue(authentication, RandomUtility.NextBit());
            else if (RandomUtility.Within(95) == true)
                typeMember.SetValue(authentication, (long)typeMember.Template.Count);
            else
                typeMember.SetValue(authentication, RandomUtility.NextLong(long.MaxValue));

            if (RandomUtility.Within(50) == true)
                typeMember.SetComment(authentication, RandomUtility.NextString());
        }

        public static void ModifyRandomValue(this ITypeMember typeMember, Authentication authentication)
        {
            if (RandomUtility.Within(75) == true)
            {
                SetRandomName(typeMember, authentication);
            }
            else if (RandomUtility.Within(75) == true)
            {
                SetRandomValue(typeMember, authentication);
            }
            else
            {
                SetRandomComment(typeMember, authentication);
            }
        }

        public static void ExecuteRandomTask(this ITypeMember typeMember, Authentication authentication)
        {
            if (RandomUtility.Within(75) == true)
            {
                SetRandomName(typeMember, authentication);
            }
            else if (RandomUtility.Within(75) == true)
            {
                SetRandomValue(typeMember, authentication);
            }
            else
            {
                SetRandomComment(typeMember, authentication);
            }
        }

        public static void SetRandomName(this ITypeMember typeMember, Authentication authentication)
        {
            var newName = RandomUtility.NextIdentifier();
            typeMember.SetName(authentication, newName);
        }

        public static void SetRandomValue(this ITypeMember typeMember, Authentication authentication)
        {
            if (typeMember.Template.IsFlag == true)
            {
                typeMember.SetValue(authentication, RandomUtility.NextBit());
            }
            else
            {
                typeMember.SetValue(authentication, RandomUtility.NextLong(long.MaxValue));
            }
        }

        public static void SetRandomComment(this ITypeMember typeMember, Authentication authentication)
        {
            if (RandomUtility.Within(50) == true)
            {
                typeMember.SetComment(authentication, RandomUtility.NextString());
            }
            else
            {
                typeMember.SetComment(authentication, string.Empty);
            }
        }
    }
}
