using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Host.HttpListener;

namespace Ntreev.Crema.ServiceHosts.Http
{
    using LoggerType = Func<TraceEventType, int, object, Exception, Func<object, Exception, string>, bool>;
    
    public static class OwinServerFactory
    {
        private static readonly object LockObj = new object();
        private static readonly Assembly OwinHttpListenerAssembly = AppDomain.CurrentDomain.GetAssemblies().First(asm => asm.GetName().Name == "Microsoft.Owin.Host.HttpListener");
        private static readonly Type LogHelperType = OwinHttpListenerAssembly.GetType("Microsoft.Owin.Host.HttpListener.LogHelper");
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public static void Initialize(IDictionary<string, object> properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            
            properties["owin.Version"] = "1.0";
            var dictionary = properties.Get<IDictionary<string, object>>("server.Capabilities") ?? new Dictionary<string, object>();
            properties["server.Capabilities"] = dictionary;
            DetectWebSocketSupport(properties);
            var owinHttpListener = (OwinHttpListener) typeof(OwinHttpListener)
                .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null)
                ?.Invoke(null);
            
            FixCrashBug(owinHttpListener);
            
            properties[typeof(OwinHttpListener).FullName] = owinHttpListener;
            properties[typeof(HttpListener).FullName] = owinHttpListener.Listener;
        }

        private static void FixCrashBug(OwinHttpListener owinHttpListener)
        {
            var httpListener = owinHttpListener.Listener;
            var currentOutstandingAccepts = 0;
            var currentOutstandingRequests = 0;
            var owinHttpListenerType = owinHttpListener.GetType();
            var owinHttpListenerOffloadStartNextRequestMethodInfo = owinHttpListenerType.GetMethod("OffloadStartNextRequest", DefaultBindingFlags);
            var owinHttpListenerOffloadStartNextRequest = (Action) Delegate.CreateDelegate(typeof(Action), owinHttpListener, owinHttpListenerOffloadStartNextRequestMethodInfo);
            var owinHttpListenerCanAcceptMoreRequestsPropertyInfo = owinHttpListener.GetType().GetProperty("CanAcceptMoreRequests", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new MissingMemberException("CanAcceptMoreRequests");
            var owinHttpListenerProcessRequestAsyncMethodInfo = owinHttpListener.GetType().GetMethod("ProcessRequestAsync", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof(HttpListenerContext)}, null);
            var owinHttpListenerProcessRequestAsync = (Func<HttpListenerContext, Task>) Delegate.CreateDelegate(typeof(Func<HttpListenerContext, Task>), owinHttpListener, owinHttpListenerProcessRequestAsyncMethodInfo ?? throw new MissingMethodException("ProcessRequestAsync"));
            var logHelperLogExceptionMethodInfo = LogHelperType.GetMethod("LogException", BindingFlags.NonPublic | BindingFlags.Static);
            var logHelperLogException = (Action<LoggerType, string, Exception>) Delegate.CreateDelegate(typeof(Action<LoggerType, string, Exception>), null, logHelperLogExceptionMethodInfo ?? throw new MissingMethodException("LogException"));
            LoggerType owinHttpListenerLogger = null;

            var newStartNextRequestAsync = new Action(async () =>
            {
                if (owinHttpListenerLogger == null)
                {
                    lock (LockObj)
                    {
                        owinHttpListenerLogger = (LoggerType) owinHttpListenerType
                            .GetField("_logger", DefaultBindingFlags)
                            ?.GetValue(owinHttpListener);
                    }
                }
                
                while (httpListener.IsListening && (bool) owinHttpListenerCanAcceptMoreRequestsPropertyInfo.GetValue(owinHttpListener))
                {
                    Interlocked.Increment(ref currentOutstandingAccepts);
                    HttpListenerContext context;
                    try
                    {
                        context = await httpListener.GetContextAsync();
                        if (IsEmptyPayloadAndContentLength(context))
                        {
                            Interlocked.Decrement(ref currentOutstandingAccepts);
                            owinHttpListenerOffloadStartNextRequest();
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Decrement(ref currentOutstandingAccepts);
                        logHelperLogException(owinHttpListenerLogger, "Accept", ex);
                        continue;
                    }

                    Interlocked.Decrement(ref currentOutstandingAccepts);
                    Interlocked.Increment(ref currentOutstandingRequests);
                    owinHttpListenerOffloadStartNextRequest();
                    await owinHttpListenerProcessRequestAsync(context);
                }
            });
            owinHttpListenerType
                .GetField("_startNextRequestAsync", DefaultBindingFlags)
                ?.SetValue(owinHttpListener, newStartNextRequestAsync);
        }
        
        private static bool IsEmptyPayloadAndContentLength(HttpListenerContext context)
        {
            return context.Response.StatusCode == (int) HttpStatusCode.LengthRequired;
        }

        private static void DetectWebSocketSupport(IDictionary<string, object> properties)
        {
            var logHelperLogInfoMethodInfo = LogHelperType.GetMethod("LogInfo", BindingFlags.NonPublic | BindingFlags.Static);
            var logHelperLogInfo = (Action<LoggerType, string>) Delegate.CreateDelegate(typeof(Action<LoggerType, string>), null, logHelperLogInfoMethodInfo ?? throw new MissingMethodException("LogInfo"));

            if (Environment.OSVersion.Version >= new Version(6, 2))
            {
                properties.Get<IDictionary<string, object>>("server.Capabilities")["websocket.Version"] = "1.0";
            }
            else
            {
                var logHelperCreateLoggerMethodInfo = LogHelperType.GetMethod("CreateLogger", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new MissingMethodException("CreateLogger");
                var logger = logHelperCreateLoggerMethodInfo.Invoke(null, new object[]
                {
                    properties.Get<Func<string, Func<TraceEventType, int, object, Exception, Func<object, Exception, string>, bool>>>("server.LoggerFactory"), typeof(OwinServerFactory)
                });
                logHelperLogInfo((LoggerType)logger, "No WebSocket support detected, Windows 8 or later is required.");
            }
        }

        public static IDisposable Create(Func<IDictionary<string, object>, Task> app, IDictionary<string, object> properties)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            var owinHttpListener = properties.Get<OwinHttpListener>(typeof(OwinHttpListener).FullName);
            var owinHttpListenerType = owinHttpListener.GetType();
            var listener = properties.Get<HttpListener>(typeof(HttpListener).FullName) ?? new HttpListener();
            var addresses = properties.Get<IList<IDictionary<string, object>>>("host.Addresses") ?? new List<IDictionary<string, object>>();
            var capabilities = properties.Get<IDictionary<string, object>>("server.Capabilities") ?? new Dictionary<string, object>();
            var loggerFactory = properties.Get<Func<string, Func<TraceEventType, int, object, Exception, Func<object, Exception, string>, bool>>>("server.LoggerFactory");
            owinHttpListenerType
                .GetMethod("Start", DefaultBindingFlags)
                ?.Invoke(owinHttpListener, new object[] {listener, app, addresses, capabilities, loggerFactory});
            return owinHttpListener;
        }
    }

    internal static class DictionaryExtension
    {
        public static T Get<T>(this IDictionary<string, object> dictionary, string name)
        {
            return dictionary.ContainsKey(name) ? (T)dictionary[name] : default;
        }
    }
}