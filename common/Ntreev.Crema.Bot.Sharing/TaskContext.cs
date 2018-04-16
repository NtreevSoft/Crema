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
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Bot
{
    public sealed class TaskContext
    {
        private readonly Stack<TaskItem> stacks = new Stack<TaskItem>();
        private Authentication authentication;

        internal TaskContext()
        {

        }

        public Authentication Authentication
        {
            get { return this.authentication; }
            internal set { this.authentication = value; }
        }

        public void Push(object target)
        {
            this.Push(target, RandomUtility.Next(10));
        }

        public void Push(object target, int count)
        {
            this.stacks.Push(new TaskItem()
            {
                Target = target,
                Count = count,
            });
        }

        public void DoTask()
        {
            this.stacks.Peek().Count--;
        }

        public void Complete(object target)
        {
            if (this.stacks.Peek().Target != target)
                throw new Exception();
            this.stacks.Peek().Count = 0;
        }

        public void Pop(object target)
        {
            if (this.stacks.Peek().Target != target)
                throw new Exception();
            this.stacks.Pop();
            this.DoTask();
        }

        public bool IsCompleted(object target)
        {
            if (this.stacks.Peek().Target != target)
                throw new ArgumentException();
            return this.stacks.Peek().Count <= 0;
        }

        public object Target
        {
            get
            {
                if (this.stacks.Any())
                    return this.stacks.Peek().Target;
                return null;
            }
        }

        public object State
        {
            get; set;
        }

        public bool AllowException
        {
            get; set;
        }

        #region classes

        class TaskItem
        {
            public object Target { get; set; }

            public int Count { get; set; }

            public override string ToString()
            {
                return this.Target.ToString();
            }
        }

        #endregion
    }
}
