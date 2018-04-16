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
    public static class TypeContextExtensions
    {
        static TypeContextExtensions()
        {
            MinTypeCount = 1;
            MaxTypeCount = 20;
            MinTypeCategoryCount = 1;
            MaxTypeCategoryCount = 20;
        }

        public static void AddRandomItems(this ITypeContext typeContext, Authentication authentication)
        {
            AddRandomCategories(typeContext, authentication);
            AddRandomTypes(typeContext, authentication);
        }

        public static void AddRandomCategories(this ITypeContext typeContext, Authentication authentication)
        {
            AddRandomCategories(typeContext, authentication, RandomUtility.Next(MinTypeCategoryCount, MaxTypeCategoryCount));
        }

        public static void AddRandomCategories(this ITypeContext typeContext, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                typeContext.AddRandomCategory(authentication);
            }
        }

        public static ITypeCategory AddRandomCategory(this ITypeCategory category, Authentication authentication)
        {
            var categoryName = RandomUtility.NextIdentifier();
            return category.AddNewCategory(authentication, categoryName);
        }

        public static ITypeCategory AddRandomCategory(this ITypeContext typeContext, Authentication authentication)
        {
            if (RandomUtility.Within(33) == true)
            {
                return typeContext.Root.AddRandomCategory(authentication);
            }
            else
            {
                var category = typeContext.Categories.Random();
                if (GetLevel(category, (i) => i.Parent) > 4)
                    return null;
                return category.AddRandomCategory(authentication);
            }
        }

        public static void AddRandomTypes(this ITypeContext typeContext, Authentication authentication)
        {
            AddRandomTypes(typeContext, authentication, RandomUtility.Next(MinTypeCount, MaxTypeCount));
        }

        public static void AddRandomTypes(this ITypeContext typeContext, Authentication authentication, int tryCount)
        {
            for (var i = 0; i < tryCount; i++)
            {
                AddRandomType(typeContext, authentication);
            }
        }

        public static IType AddRandomType(this ITypeContext typeContext, Authentication authentication)
        {
            var category = typeContext.Categories.Random();
            return AddRandomType(category, authentication);
        }

        public static IType AddRandomType(this ITypeCategory category, Authentication authentication)
        {
            var template = category.NewType(authentication);
            template.InitializeRandom(authentication);
            template.EndEdit(authentication);
            return template.Type;
        }

        private static int GetLevel<T>(T category, Func<T, T> parentFunc)
        {
            var level = 0;

            var parent = parentFunc(category);
            while (parent != null)
            {
                level++;
                parent = parentFunc(parent);
            }
            return level;
        }

        public static int MinTypeCount { get; set; }

        public static int MaxTypeCount { get; set; }

        public static int MinTypeCategoryCount { get; set; }

        public static int MaxTypeCategoryCount { get; set; }
    }
}
