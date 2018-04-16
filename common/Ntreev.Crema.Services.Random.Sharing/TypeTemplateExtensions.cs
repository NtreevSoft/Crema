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

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Services;
using Ntreev.Library.ObjectModel;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ntreev.Crema.Services.Random
{
    public static class TypeTemplateExtensions
    {
        static TypeTemplateExtensions()
        {
            MinMemberCount = 5;
            MaxMemberCount = 50;
        }

        public static void InitializeRandom(this ITypeTemplate template, Authentication authentication)
        {
            var typeName = RandomUtility.NextIdentifier();
            template.SetTypeName(authentication, typeName);
            if (RandomUtility.Within(50) == true)
                template.SetIsFlag(authentication, RandomUtility.NextBoolean());
            if (RandomUtility.Within(50) == true)
                template.SetComment(authentication, RandomUtility.NextString());
            template.AddRandomMembers(authentication);
        }

        public static ITypeMember AddRandomMember(this ITypeTemplate template, Authentication authentication)
        {
            var member = template.AddNew(authentication);
            member.InitializeRandom(authentication);
            template.EndNew(authentication, member);
            return member;
        }

        public static void RemoveRandomMember(this ITypeTemplate template, Authentication authentication)
        {
            var member = template.RandomOrDefault();
            member?.Delete(authentication);
        }

        public static void ModifyRandomMember(this ITypeTemplate template, Authentication authentication)
        {
            var member = template.RandomOrDefault();
            member?.ModifyRandomValue(authentication);
        }

        public static void AddRandomMembers(this ITypeTemplate template, Authentication authentication)
        {
            AddRandomMembers(template, authentication, RandomUtility.Next(MinMemberCount, MaxMemberCount));
        }

        public static void AddRandomMembers(this ITypeTemplate template, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                AddRandomMember(template, authentication);
            }
        }

        //public static object GetRandomValue(ITypeCollection types, ColumnInfo columnInfo)
        //{
        //    if (columnInfo.AllowNull == true && RandomUtility.Next(4) == 0)
        //    {
        //        return null;
        //    }
        //    else if (CremaDataTypeUtility.IsBaseType(columnInfo.DataType))
        //    {
        //        return GetRandomValue(columnInfo.DataType);
        //    }
        //    else
        //    {
        //        var itemName = new ItemName(columnInfo.DataType);
        //        return GetRandomValue(types[itemName.Name]);
        //    }
        //}

        //public static object GetRandomValue(this IType type)
        //{
        //    var typeInfo = type.TypeInfo;
        //    if (typeInfo.Members.Length == 0)
        //        throw new Exception(type.Name);

        //    if (typeInfo.IsFlag == true)
        //    {
        //        long value = 0;
        //        int count = RandomUtility.Next(1, typeInfo.Members.Length);
        //        for (var i = 0; i < count; i++)
        //        {
        //            var index = RandomUtility.Next(typeInfo.Members.Length);
        //            value |= typeInfo.Members[index].Value;
        //        }
        //        var textvalue = typeInfo.ConvertToString(value);
        //        if (textvalue == string.Empty)
        //            throw new Exception();
        //        return textvalue;
        //    }
        //    else
        //    {
        //        return typeInfo.Members.Random().Name;
        //    }
        //}

        //public static object GetRandomValue(string typeName)
        //{
        //    var type = CremaDataTypeUtility.GetType(typeName);
        //    return RandomUtility.Next(type);
        //}

        public static int MinMemberCount { get; set; }

        public static int MaxMemberCount { get; set; }
    }
}
