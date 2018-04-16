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
using Ntreev.Library;

namespace Ntreev.Crema.Services.Random
{
    public static class TableTemplateExtensions
    {
        static TableTemplateExtensions()
        {
            MinColumnCount = 1;
            MaxColumnCount = 20;
        }

        public static void InitializeRandom(this ITableTemplate template, Authentication authentication)
        {
            var tableName = RandomUtility.NextIdentifier();
            template.SetTableName(authentication, tableName);
            if (RandomUtility.Within(50) == true)
                template.SetTags(authentication, (TagInfo)TagInfoUtility.Names.Random());
            if (RandomUtility.Within(50) == true)
                template.SetComment(authentication, RandomUtility.NextString());
            template.AddRandomColumns(authentication);
        }

        public static ITableColumn AddRandomColumn(this ITableTemplate template, Authentication authentication)
        {
            var column = template.AddNew(authentication);
            column.InitializeRandom(authentication);
            template.EndNew(authentication, column);
            return column;
        }

        public static void RemoveRandomColumn(this ITableTemplate template, Authentication authentication)
        {
            var column = template.RandomOrDefault();
            column?.Delete(authentication);
        }

        public static void ModifyRandomColumn(this ITableTemplate template, Authentication authentication)
        {
            var column = template.RandomOrDefault();
            column?.ModifyRandomValue(authentication);
        }

        public static void AddRandomColumns(this ITableTemplate template, Authentication authentication)
        {
            AddRandomColumns(template, authentication, RandomUtility.Next(MinColumnCount, MaxColumnCount));
        }

        public static void AddRandomColumns(this ITableTemplate template, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                AddRandomColumn(template, authentication);
            }
        }

        public static int MinColumnCount { get; set; }

        public static int MaxColumnCount { get; set; }
    }
}
