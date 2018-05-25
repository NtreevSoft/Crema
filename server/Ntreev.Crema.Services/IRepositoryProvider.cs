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
    public interface IRepositoryProvider
    {
        void InitializeRepository(string basePath, string repositoryPath);

        void CreateRepository(string basePath, string initPath, string comment, params LogPropertyInfo[] properties);

        void CopyRepository(string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties);

        void RenameRepository(string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties);

        void DeleteRepository(string basePath, string[] repositoryNames, string comment, params LogPropertyInfo[] properties);

        IRepository CreateInstance(string basePath, string repositoryName, string workingPath);

        IEnumerable<string> GetRepositories(string basePath);

        string GetRevision(string basePath, string repositoryName);

        RepositoryInfo GetRepositoryInfo(string basePath, string repositoryName);

        string[] GetRepositoryItemList(string basePath, string repositoryName);

        LogInfo[] GetLog(string basePath, string repositoryName, string revision, int count);

        string Name { get; }
    }
}