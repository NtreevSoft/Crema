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
using Ntreev.Crema.Data;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Users;

namespace Ntreev.Crema.Services
{
    static class IRepositoryProviderExtensions
    {
        public static void CreateRepository(this IRepositoryProvider repositoryProvider, Authentication authentication, string basePath, string initPath, string comment)
        {
            var props = new List<LogPropertyInfo>
            {
                new LogPropertyInfo() { Key = LogPropertyInfo.UserIDKey, Value = authentication.ID, }
            };
            repositoryProvider.CreateRepository(basePath, initPath, comment, props.ToArray());
        }

        public static void RenameRepository(this IRepositoryProvider repositoryProvider, Authentication authentication, string basePath, string repositoryName, string newRepositoryName, string comment)
        {
            var props = new List<LogPropertyInfo>
            {
                new LogPropertyInfo() { Key = LogPropertyInfo.UserIDKey, Value = authentication.ID, }
            };
            repositoryProvider.RenameRepository(basePath, repositoryName, newRepositoryName, comment, props.ToArray());
        }

        public static void CopyRepository(this IRepositoryProvider repositoryProvider, Authentication authentication, string basePath, string repositoryName, string newRepositoryName, string comment)
        {
            var props = new List<LogPropertyInfo>
            {
                new LogPropertyInfo() { Key = LogPropertyInfo.UserIDKey, Value = authentication.ID, }
            };
            repositoryProvider.CopyRepository(basePath, repositoryName, newRepositoryName, comment, props.ToArray());
        }

        public static void DeleteRepository(this IRepositoryProvider repositoryProvider, string basePath, string[] repositoryNames, string comment, Authentication authentication, string eventLog)
        {
            var props = new List<LogPropertyInfo>
            {
                new LogPropertyInfo() { Key = LogPropertyInfo.EventLogKey, Value = eventLog, },
                new LogPropertyInfo() { Key = LogPropertyInfo.UserIDKey, Value = authentication.ID, }
            };
            repositoryProvider.DeleteRepository(basePath, repositoryNames, comment, props.ToArray());
        }


    }
}