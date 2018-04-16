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

using Ntreev.Crema.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ntreev.Crema.Commands.Consoles
{
    public abstract class ConsoleDriveBase : IConsoleDrive
    {
        private readonly string name;

        protected ConsoleDriveBase(string name)
        {
            this.name = name;
        }

        public abstract string[] GetPaths();

        public void Create(Authentication authentication, string path, string name)
        {
            this.OnCreate(authentication, path, name);
        }

        public void Move(Authentication authentication, string path, string newPath)
        {
            this.OnMove(authentication, path, newPath);
        }

        public void Delete(Authentication authentication, string path)
        {
            this.OnDelete(authentication, path);
        }

        public void SetPath(Authentication authentication, string path)
        {
            this.OnSetPath(authentication, path);
        }

        public abstract object GetObject(Authentication authentication, string path);

        public string Name => this.name;

        public ConsoleCommandContextBase CommandContext
        {
            get;
            internal set;
        }

        protected abstract void OnCreate(Authentication authentication, string path, string name);

        protected abstract void OnMove(Authentication authentication, string path, string newPath);

        protected abstract void OnDelete(Authentication authentication, string path);

        protected abstract void OnSetPath(Authentication authentication, string path);
    }
}
