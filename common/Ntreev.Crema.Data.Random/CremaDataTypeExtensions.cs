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

using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Random
{
    public static class CremaDataTypeExtensions
    {
        static CremaDataTypeExtensions()
        {
            MinMemberCount = 1;
            MaxMemberCount = 20;
        }

        public static CremaDataType CreateRandomType()
        {
            return CreateRandomType(string.Empty, string.Empty);
        }

        public static CremaDataType CreateRandomType(string prefix, string postfix)
        {
            var dataType = new CremaDataType(IdentifierUtility.Next(prefix, postfix));
            dataType.IsFlag = RandomUtility.Within(25);
            dataType.Comment = RandomUtility.Within(25) ? string.Empty : RandomUtility.NextString();
            dataType.AddRandomMembers(RandomUtility.Next(CremaDataTypeExtensions.MinMemberCount, CremaDataTypeExtensions.MaxMemberCount));
            return dataType;
        }

        public static CremaDataType CreateRandomEmptyType(string prefix, string postfix)
        {
            var dataType = new CremaDataType(IdentifierUtility.Next(prefix, postfix));
            dataType.IsFlag = RandomUtility.Within(25);
            dataType.Comment = RandomUtility.Within(25) ? string.Empty : RandomUtility.NextString();
            return dataType;
        }

        public static CremaDataTypeMember AddRandomMember(this CremaDataType dataType)
        {
            var member = dataType.NewMember();
            member.InitializeRandom();
            dataType.Members.Add(member);
            return member;
        }

        public static CremaDataTypeMember AddRandomMember(this CremaDataType dataType, string memberName)
        {
            var member = dataType.NewMember();
            member.InitializeRandom();
            member.Name = memberName;
            dataType.Members.Add(member);
            return member;
        }

        public static void AddRandomMembers(this CremaDataType dataType)
        {
            AddRandomMembers(dataType, RandomUtility.Next(MinMemberCount, MaxMemberCount));
        }

        public static void AddRandomMembers(this CremaDataType dataType, int count)
        {
            while (dataType.Members.Count < count)
            {
                try
                {
                    AddRandomMember(dataType);
                }
                catch
                {

                }
            }
        }

        public static void ModifyRandomMember(this CremaDataType dataType)
        {
            if (RandomUtility.Within(90) == true)
            {
                var typeMember = dataType.Members.RandomOrDefault();
                if (typeMember != null)
                {
                    typeMember.SetRandomValue();
                    if (RandomUtility.Within(5) == true)
                        typeMember.IsEnabled = RandomUtility.NextBoolean();
                }
            }
            else if (RandomUtility.Within(75) == true)
            {
                AddRandomMember(dataType);
            }
            else
            {
                var dataMember = dataType.Members.RandomOrDefault();
                if (dataMember != null)
                {
                    if (RandomUtility.Within(50) == true)
                        dataMember.Delete();
                    else
                        dataType.Members.Remove(dataMember);
                }
            }
        }

        public static void ModifyRandomMembers(this CremaDataType dataType, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                try
                {
                    ModifyRandomMember(dataType);
                }
                catch
                {

                }
            }
        }

        public static string GetRandomValue(this CremaDataType dataType)
        {
            if (dataType.Members.Any() == false)
                throw new Exception(dataType.TypeName);

            if (dataType.IsFlag == true)
            {
                long value = 0;
                var count = RandomUtility.Next(1, dataType.Members.Count);
                for (var i = 0; i < count; i++)
                {
                    var index = RandomUtility.Next(dataType.Members.Count);
                    value |= dataType.Members[index].Value;
                }
                var textvalue = dataType.ConvertToString(value);
                if (textvalue == string.Empty)
                    throw new Exception();
                return textvalue;
            }
            else
            {
                return dataType.Members.Random().Name;
            }
        }

        public static int MinMemberCount { get; set; }

        public static int MaxMemberCount { get; set; }
    }
}
