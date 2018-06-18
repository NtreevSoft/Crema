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
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using System;
using System.IO;

namespace Ntreev.Crema.Services.Data
{
    static class AccessInfoExtensions
    {
        private const string extension = ".acs";

        //public static string GetAccessInfoPath(this ITableItem tableItem)
        //{
        //    var tableContext = GetTableContext(tableItem);
        //    var uriString = UriUtility.Combine(tableContext.BasePath, tableItem.Path.TrimStart(PathUtility.SeparatorChar) + extension);
        //    var uri = new Uri(uriString);
        //    return uri.LocalPath;
        //}

        //public static string GetAccessInfoPath(this ITableItem tableItem, string categoryPath)
        //{
        //    var tableContext = GetTableContext(tableItem);
        //    var uriString = UriUtility.Combine(tableContext.BasePath, categoryPath.TrimStart(PathUtility.SeparatorChar) + tableItem.Name + extension);
        //    var uri = new Uri(uriString);
        //    return uri.LocalPath;
        //}

        //public static void WriteAccessInfo(this ITableItem tableItem, string accessInfoPath, AccessInfo accessInfo)
        //{
        //    if (tableItem is Table table)
        //    {
        //        WriteAccessInfo(accessInfoPath, accessInfo);
        //    }
        //    else if (tableItem is TableCategory category)
        //    {
        //        WriteAccessInfo(accessInfoPath, accessInfo);
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public static void ReadAccessInfo(this ITableItem tableItem, string accessInfoPath)
        //{
        //    if (tableItem is Table table)
        //    {
        //        if (ReadAccessInfo(accessInfoPath, out var accessInfo))
        //        {
        //            table.SetAccessInfo(accessInfo);
        //        }
        //    }
        //    else if (tableItem is TableCategory category)
        //    {
        //        if (ReadAccessInfo(accessInfoPath, out var accessInfo))
        //        {
        //            category.SetAccessInfo(accessInfo);
        //        }
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public static string GetAccessInfoPath(this ITypeItem typeItem)
        //{
        //    var typeContext = GetTypeContext(typeItem);
        //    var uriString = UriUtility.Combine(typeContext.BasePath, typeItem.Path.TrimStart(PathUtility.SeparatorChar) + extension);
        //    var uri = new Uri(uriString);
        //    return uri.LocalPath;
        //}

        //public static string GetAccessInfoPath(this ITypeItem typeItem, string categoryPath)
        //{
        //    var typeContext = GetTypeContext(typeItem);
        //    var uriString = UriUtility.Combine(typeContext.BasePath, categoryPath.TrimStart(PathUtility.SeparatorChar) + typeItem.Name + extension);
        //    var uri = new Uri(uriString);
        //    return uri.LocalPath;
        //}

        //public static void WriteAccessInfo(this ITypeItem typeItem, string accessInfoPath, AccessInfo accessInfo)
        //{
        //    if (typeItem is Type type)
        //    {
        //        WriteAccessInfo(accessInfoPath, accessInfo);
        //    }
        //    else if (typeItem is TypeCategory category)
        //    {
        //        WriteAccessInfo(accessInfoPath, accessInfo);
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public static void ReadAccessInfo(this ITypeItem typeItem, string accessInfoPath)
        //{
        //    if (typeItem is Type type)
        //    {
        //        if (ReadAccessInfo(accessInfoPath, out var accessInfo))
        //        {
        //            type.SetAccessInfo(accessInfo);
        //        }
        //    }
        //    else if (typeItem is TypeCategory category)
        //    {
        //        if (ReadAccessInfo(accessInfoPath, out var accessInfo))
        //        {
        //            category.SetAccessInfo(accessInfo);
        //        }
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public static string GetAccessInfoPath(this DataBase dataBase)
        //{
        //    return Path.Combine(dataBase.BasePath, extension);
        //}

        //public static void WriteAccessInfo(this DataBase dataBase, string accessInfoPath, AccessInfo accessInfo)
        //{
        //    WriteAccessInfo(accessInfoPath, accessInfo);
        //}

        //public static void ReadAccessInfo(this DataBase dataBase, string accessInfoPath)
        //{
        //    if (ReadAccessInfo(accessInfoPath, out var accessInfo))
        //    {
        //        dataBase.SetAccessInfo(accessInfo);
        //    }
        //}

        //private static void WriteAccessInfo(string accessInfoPath, AccessInfo accessInfo)
        //{
        //    JsonSerializerUtility.Write(accessInfoPath, (AccessSerializationInfo)accessInfo, true);
        //}

        //private static bool ReadAccessInfo(string accessInfoPath, out AccessInfo accessInfo)
        //{
        //    accessInfo = AccessInfo.Empty;
        //    if (File.Exists(accessInfoPath) == false)
        //        return false;

        //    accessInfo = (AccessInfo)JsonSerializerUtility.Read<AccessSerializationInfo>(accessInfoPath);
        //    return true;
        //}

        //private static TableContext GetTableContext(this ITableItem tableItem)
        //{
        //    if (tableItem is Table table)
        //    {
        //        return table.Context;
        //    }
        //    else if (tableItem is TableCategory category)
        //    {
        //        return category.Context;
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //private static TypeContext GetTypeContext(this ITypeItem typeItem)
        //{
        //    if (typeItem is Type type)
        //    {
        //        return type.Context;
        //    }
        //    else if (typeItem is TypeCategory category)
        //    {
        //        return category.Context;
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}
