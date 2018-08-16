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
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;

namespace Ntreev.Crema.Services
{
    public interface IRepository : IDisposable
    {
        void Add(string path);

        void Move(string srcPath, string toPath);

        void Delete(string path);

        void Copy(string srcPath, string toPath);

        void Commit(string author, string comment, params LogPropertyInfo[] properties);

        void Revert();

        void BeginTransaction(string author, string name);

        void EndTransaction();

        void CancelTransaction();

        LogInfo[] GetLog(string[] paths, string revision);

        Uri GetUri(string path, string revision);

        string Export(Uri uri, string exportPath);

        RepositoryItem[] Status(params string[] paths);

        RepositoryInfo RepositoryInfo { get; }

        string BasePath { get; }
    }
}
