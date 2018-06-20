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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    /// <summary>
    /// 경로로 표현되는 객체를 읽고 쓰도록 하는 인터페이스입니다.
    /// 경로는 itemPath로 나타내며 확장자가 포함되지 않는 순수 경로를 나타냅니다. 
    /// 폴더를 일 경우에는 경우 마지막에 경로 문자(window는 \, linux는 /)가 있게 됩니다.
    /// 또한 실제 접근 가능한 경로로 표현됩니다.
    /// C:\\repo-svn\database\tables\
    /// C:\\repo-svn\database\tables\table1
    /// </summary>
    public interface IObjectSerializer
    {
        string[] Serialize(string itemPath, object obj, ObjectSerializerSettings settings);

        object Deserialize(string itemPath, Type type, ObjectSerializerSettings settings);

        string[] GetPath(string itemPath, Type type, ObjectSerializerSettings settings);

        string[] GetReferencedPath(string itemPath, Type type, ObjectSerializerSettings settings);

        string[] GetItemPaths(string path, Type type, ObjectSerializerSettings settings);

        void Validate(string itemPath, Type type, ObjectSerializerSettings settings);

        bool Exists(string itemPath, Type type, ObjectSerializerSettings settings);

        string Name { get; }
    }
}
