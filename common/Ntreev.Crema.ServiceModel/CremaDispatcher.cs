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

#pragma warning disable 0612
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Ntreev.Crema.ServiceModel
{
    public sealed class CremaDispatcher
    {
        private Dispatcher dispatcher;
        private DispatcherFrame dispatcherFrame;
        private readonly object owner;

        public CremaDispatcher(object owner)
        {
            var eventSet = new ManualResetEvent(false);
            this.owner = owner;
            var thread = new Thread(() =>
            {
                this.dispatcher = Dispatcher.CurrentDispatcher;
                this.dispatcher.UnhandledException += Dispatcher_UnhandledException;
                this.dispatcherFrame = new DispatcherFrame(true);
                eventSet.Set();
                try
                {
                    Dispatcher.PushFrame(this.dispatcherFrame);
                }
                catch
                {
                    this.dispatcher = null;
                    this.dispatcherFrame = null;
                }
            })
            {
                Name = owner.ToString()
            };
            thread.Start();
            eventSet.WaitOne();
        }

        public CremaDispatcher(object owner, Dispatcher dispatcher)
        {
            this.owner = owner;
            this.dispatcher = dispatcher;
        }

        public void VerifyAccess()
        {
            this.dispatcher.VerifyAccess();
        }

        public bool CheckAccess()
        {
            return this.dispatcher.CheckAccess();
        }

        public void Invoke(Action action)
        {
            this.Invoke(action, DispatcherPriority.Send);
        }

        [Obsolete]
        public void Invoke(Action action, DispatcherPriority priority)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                this.InvokeUnix(action, priority);
            }
            else
            {
                this.InvokeDefault(action, priority);
            }
        }

        public Task InvokeAsync(Action action)
        {
            return this.InvokeAsync(action, DispatcherPriority.Send);
        }

        [Obsolete]
        public Task InvokeAsync(Action action, DispatcherPriority priority)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return this.InvokeAsyncUnix(action, priority);
            }
            else
            {
                return this.InvokeAsyncDefault(action, priority);
            }
        }

        public TResult Invoke<TResult>(Func<TResult> callback)
        {
            return this.Invoke<TResult>(callback, DispatcherPriority.Send);
        }

        [Obsolete]
        public TResult Invoke<TResult>(Func<TResult> callback, DispatcherPriority priority)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return this.InvokeUnix<TResult>(callback, priority);
            }
            else
            {
                return this.InvokeDefault<TResult>(callback, priority);
            }
        }

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> callback)
        {
            return this.InvokeAsync(callback, DispatcherPriority.Send);
        }

        [Obsolete]
        public Task<TResult> InvokeAsync<TResult>(Func<TResult> callback, DispatcherPriority priority)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return this.InvokeAsyncUnix<TResult>(callback, priority);
            }
            else
            {
                return this.InvokeAsyncDefault<TResult>(callback, priority);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool sync)
        {
            if (this.dispatcher != null)
            {
                if (this.dispatcherFrame != null)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        this.dispatcherFrame.Continue = false;
                        this.dispatcher.InvokeShutdown();
                        this.dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => { }));
                    }
                    else
                    {
                        if (sync == true)
                        {
                            this.dispatcher.InvokeShutdown();
                        }
                        else
                        {
                            this.dispatcherFrame.Continue = false;
                        }
                    }
                    this.dispatcherFrame = null;
                }
                this.dispatcher = null;
            }
        }

        public string Name
        {
            get { return this.owner.ToString(); }
        }

        public static implicit operator Dispatcher(CremaDispatcher dispatcher)
        {
            return dispatcher.dispatcher;
        }

        private void InvokeDefault(Action action, DispatcherPriority priority)
        {
            this.dispatcher.Invoke(action, priority);
        }

        private Task InvokeAsyncDefault(Action action, DispatcherPriority priority)
        {
            return this.dispatcher.InvokeAsync(action, priority).Task;
        }

        private TResult InvokeDefault<TResult>(Func<TResult> callback, DispatcherPriority priority)
        {
            return this.dispatcher.Invoke<TResult>(callback, priority);
        }

        private Task<TResult> InvokeAsyncDefault<TResult>(Func<TResult> callback, DispatcherPriority priority)
        {
            return this.dispatcher.InvokeAsync<TResult>(callback, priority).Task;
        }

        private void InvokeUnix(Action action, DispatcherPriority priority)
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                action();
                return;
            }
            else
            {
                var func = new Func<object>(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        return e;
                    }
                    return null;
                });
                var eventSet = new ManualResetEvent(false);
                var result = this.dispatcher.BeginInvoke(priority, func);
                result.Completed += (s, e) => eventSet.Set();
                if (result.Status != DispatcherOperationStatus.Completed)
                    eventSet.WaitOne();
                if (result.Result is Exception ex)
                    throw ex;
            }
        }

        private Task InvokeAsyncUnix(Action action, DispatcherPriority priority)
        {
            return Task.Run(() => this.Invoke(action, priority));
        }

        private TResult InvokeUnix<TResult>(Func<TResult> callback, DispatcherPriority priority)
        {
            if (this.dispatcher.CheckAccess() == true)
            {
                return callback();
            }
            else
            {
                var func = new Func<object>(() =>
                {
                    try
                    {
                        return callback();
                    }
                    catch (Exception e)
                    {
                        return new ExceptionHost(e);
                    }
                });
                var eventSet = new ManualResetEvent(false);
                var result = this.dispatcher.BeginInvoke(priority, func);
                result.Completed += (s, e) => eventSet.Set();
                if (result.Status != DispatcherOperationStatus.Completed)
                    eventSet.WaitOne();
                if (result.Result is ExceptionHost host)
                    throw host.Exception;
                return (TResult)result.Result;
            }
        }

        private Task<TResult> InvokeAsyncUnix<TResult>(Func<TResult> callback, DispatcherPriority priority)
        {
            return Task<TResult>.Run(() => this.Invoke<TResult>(callback, priority));
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

        #region classes

        class ExceptionHost
        {
            private readonly Exception exception;

            public ExceptionHost(Exception exception)
            {
                this.exception = exception;
            }

            public Exception Exception
            {
                get { return this.exception; }
            }
        }

        #endregion
    }
}
