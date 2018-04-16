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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.Random;
using Ntreev.Library;
using Ntreev.Crema.ServiceModel;
using System.Threading;
using System.Reflection;
using Ntreev.Crema.Services;
using System.Security;

namespace Ntreev.Crema.Bot
{
    public abstract class AutobotBase : IDisposable
    {
        private readonly static object error = new object();
        private readonly string autobotID;
        private readonly TaskContext taskContext = new TaskContext();
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private IEnumerable<ITaskProvider> taskProviders;

        public AutobotBase(string autobotID)
        {
            this.autobotID = autobotID;
            this.MinSleepTime = 1;
            this.MaxSleepTime = 10;
        }

        public void Cancel()
        {
            this.cancelTokenSource.Cancel();
        }

        public Authentication Login()
        {
            this.taskContext.Authentication = this.OnLogin();
            //this.taskContext.Authentication.Expired += (s, e) =>
            //{
            //    this.taskContext.Authentication = null;
            //};
            return this.taskContext.Authentication;
        }

        public void Logout()
        {
            this.OnLogout(this.taskContext.Authentication);
            this.taskContext.Authentication = null;
        }

        public int MinSleepTime
        {
            get; set;
        }

        public int MaxSleepTime
        {
            get; set;
        }

        public bool IsOnline
        {
            get { return this.taskContext.Authentication != null; }
        }

        public string AutobotID
        {
            get { return this.autobotID; }
        }

        public abstract ICremaHost CremaHost
        {
            get;
        }

        public abstract AutobotServiceBase Service
        {
            get;
        }

        public bool AllowException
        {
            get { return this.taskContext.AllowException; }
            set { this.taskContext.AllowException = value; }
        }

        public Task ExecuteAsync(IEnumerable<ITaskProvider> taskProviders)
        {
            this.taskProviders = taskProviders;
            this.taskContext.Push(this);
            return Task.Run(() => this.Execute(taskProviders));
        }

        public event EventHandler Disposed;

        protected virtual void OnDisposed(EventArgs e)
        {
            this.Disposed?.Invoke(this, e);
        }

        protected abstract Authentication OnLogin();

        protected abstract void OnLogout(Authentication authentication);

        private void InvokeTask(MethodInfo method, ITaskProvider taskProvider, object target)
        {
            try
            {
                if (method != null)
                {
                    method.Invoke(taskProvider, new object[] { target, this.taskContext });
                }
            }
            catch
            {

            }
            finally
            {
                this.taskContext.DoTask();
            }
        }

        private void Execute(IEnumerable<ITaskProvider> taskProviders)
        {
            while (this.cancelTokenSource.IsCancellationRequested == false)
            {
                if (this.taskContext.Target == null)
                {
                    this.taskContext.Push(this);
                }

                var taskProvider = RandomTaskProvider(this.taskContext.Target);

                try
                {
                    if (this.cancelTokenSource.IsCancellationRequested == true)
                        break;
                    taskProvider.InvokeTask(this.taskContext);
                }
                catch
                {
                    this.taskContext.Complete(this.taskContext.Target);
                    continue;
                }

                if (this.taskContext.Target != null && taskProvider.TargetType.IsAssignableFrom(this.taskContext.Target.GetType()) == true)
                {
                    var method = RandomMethod(taskProvider);
                    if (this.cancelTokenSource.IsCancellationRequested == true)
                        break;
                    this.InvokeTask(method, taskProvider, this.taskContext.Target);
                }
            }
        }

        private static MethodInfo RandomMethod(ITaskProvider taskProvider)
        {
            var weight = RandomUtility.Next(100) + 1;
            var methods = taskProvider.GetType().GetMethods();
            return methods.RandomOrDefault((item) => Predicate(item));

            bool Predicate(MethodInfo methodInfo)
            {
                if (methodInfo.IsStatic == true)
                    return false;

                if (methodInfo.ReturnType != typeof(void))
                    return false;

                var attr = methodInfo.GetCustomAttribute<TaskMethodAttribute>();
                if (attr == null)
                    return false;

                if (attr.Weight < weight)
                    return false;

                var parameters = methodInfo.GetParameters();
                if (parameters.Count() != 2)
                    return false;

                return parameters[1].ParameterType == typeof(TaskContext);
            }
        }

        private ITaskProvider RandomTaskProvider(object target)
        {
            return this.taskProviders.WeightedRandom(SelectWeight, Predicate);

            bool Predicate(ITaskProvider taskProvider)
            {
                if (taskProvider.TargetType.IsAssignableFrom(target.GetType()) == false)
                    return false;
                return true;
            }

            int SelectWeight(ITaskProvider predicate)
            {
                var attr = Attribute.GetCustomAttribute(predicate.GetType(), typeof(TaskClassAttribute)) as TaskClassAttribute;
                if (attr == null)
                    return 100;
                return attr.Weight;
            }
        }

        #region IDisposable

        void IDisposable.Dispose()
        {
            this.OnDisposed(EventArgs.Empty);
        }

        #endregion
    }
}