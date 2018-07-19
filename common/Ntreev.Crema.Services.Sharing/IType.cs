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
using System;
using System.Data;
using System.Threading.Tasks;
using Ntreev.Library;

namespace Ntreev.Crema.Services
{
    public interface IType : IAccessible, ILockable, IPermission, IServiceProvider, IDispatcherObject, IExtendedProperties
    {
        void Rename(Authentication authentication, string newName);

        void Move(Authentication authentication, string categoryPath);

        void Delete(Authentication authentication);

        IType Copy(Authentication authentication, string newTypeName, string categoryPath);

        CremaDataSet GetDataSet(Authentication authentication, string revision);

        LogInfo[] GetLog(Authentication authentication);

        FindResultInfo[] Find(Authentication authentication, string text, FindOptions options);

        string Name { get; }

        string Path { get; }

        bool IsLocked { get; }

        bool IsPrivate { get; }

        TypeInfo TypeInfo { get; }

        TypeState TypeState { get; }

        ITypeCategory Category { get; }

        ITypeTemplate Template { get; }

        event EventHandler Renamed;

        event EventHandler Moved;

        event EventHandler Deleted;

        event EventHandler LockChanged;

        event EventHandler AccessChanged;

        event EventHandler TypeInfoChanged;

        event EventHandler TypeStateChanged;
    }
}
