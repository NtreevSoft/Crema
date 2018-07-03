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
            repositoryProvider.CreateRepository(authentication.ID, basePath, initPath, comment, new LogPropertyInfo[] { });
        }

        public static void RenameRepository(this IRepositoryProvider repositoryProvider, Authentication authentication, string basePath, string repositoryName, string newRepositoryName, string comment)
        {
            repositoryProvider.RenameRepository(authentication.ID, basePath, repositoryName, newRepositoryName, comment, new LogPropertyInfo[] { });
        }

        public static void CopyRepository(this IRepositoryProvider repositoryProvider, Authentication authentication, string basePath, string repositoryName, string newRepositoryName, string comment)
        {
            repositoryProvider.CopyRepository(authentication.ID, basePath, repositoryName, newRepositoryName, comment, new LogPropertyInfo[] { });
        }

        public static void DeleteRepository(this IRepositoryProvider repositoryProvider, Authentication authentication, string basePath, string repositoryName, string comment)
        {
            repositoryProvider.DeleteRepository(authentication.ID, basePath, repositoryName, comment, new LogPropertyInfo[] { });
        }
    }
}