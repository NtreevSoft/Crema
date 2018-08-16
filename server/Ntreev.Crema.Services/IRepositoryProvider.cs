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
        void InitializeRepository(string basePath, string initPath);

        void CreateRepository(string author, string basePath, string initPath, string comment, params LogPropertyInfo[] properties);

        void CopyRepository(string author, string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties);

        void RenameRepository(string author, string basePath, string repositoryName, string newRepositoryName, string comment, params LogPropertyInfo[] properties);

        void DeleteRepository(string author, string basePath, string repositoryName, string comment, params LogPropertyInfo[] properties);

        void RevertRepository(string author, string basePath, string repositoryName, string revision, string comment);

        IRepository CreateInstance(RepositorySettings settings);

        string[] GetRepositories(string basePath);

        string GetRevision(string basePath, string repositoryName);

        RepositoryInfo GetRepositoryInfo(string basePath, string repositoryName);

        string[] GetRepositoryItemList(string basePath, string repositoryName);

        /// <summary>
        /// 해당 저장소의 로그 목록을 가져옵니다. 지정된 revision 부터 과거 순입니다.
        /// revision 이 null 값일때는 최신 로그를 가져옵니다. 
        /// 만약 로그 갯수가 많아서 개수가 제한될때는 마지막에 LogInfo.Empty 의 유무를 확인해 로그 기록이 더 있음을 확인할 수 있습니다.
        /// </summary>
        LogInfo[] GetLog(string basePath, string repositoryName, string revision);

        string Name { get; }
    }
}