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
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Ntreev.Crema.Javascript
{
    public abstract class ScriptMethodBase : IScriptMethod
    {
        private readonly string name;
        private Delegate delegateVariable;

        protected ScriptMethodBase()
        {
            this.name = ToCamelCase(this.GetType().Name);
        }

        protected ScriptMethodBase(string name)
        {
            this.name = name;
        }

        public string Name => this.name;

        public Delegate Delegate
        {
            get
            {
                if (this.delegateVariable == null)
                {
                    this.delegateVariable = this.CreateDelegate();
                }
                return this.delegateVariable;
            }
        }

        public IScriptMethodContext Context
        {
            get;
            set;
        }

        protected abstract Delegate CreateDelegate();

        protected virtual void OnDisposed()
        {

        }

        protected virtual void OnInitialized()
        {

        }

        private static string ToCamelCase(string text)
        {
            var name = Regex.Replace(text, @"^([A-Z])", MatchEvaluator);

            return Regex.Replace(name, @"(Method)$", string.Empty);
        }

        private static string MatchEvaluator(Match match)
        {
            return match.Value.ToLower();
        }

        internal void Dispose()
        {
            this.OnDisposed();
        }

        internal void Initialize()
        {
            this.OnInitialized();
        }
    }
}
