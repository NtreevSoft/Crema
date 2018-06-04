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

using log4net;
using Ntreev.Crema.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Ntreev.Library;
using log4net.Repository.Hierarchy;
using log4net.Layout;
using log4net.Appender;
using System.IO;
using log4net.Core;

namespace Ntreev.Crema.Services
{
    class LogService : IDisposable
    {
        private const string conversionPattern = "%message%newline%exception";
        private readonly ILog log;
        private LogVerbose verbose = LogVerbose.Info;
        private ConsoleAppender consoleAppender;
        private RollingFileAppender rollingAppender;
        private Hierarchy hierarchy;
        private TextWriter redirectionWriter;
        private TextWriterAppender textAppender;
        private string name;

        public LogService()
        {
            var repositoryName = AppUtility.ProductName;
            var name = "global";

            var repository = LogManager.GetAllRepositories().Where(item => item.Name == repositoryName).FirstOrDefault();
            if (repository != null)
                throw new InvalidOperationException();

            this.hierarchy = (Hierarchy)LogManager.CreateRepository(repositoryName);

            var xmlLayout = new XmlLayoutSchemaLog4j()
            {
                LocationInfo = true
            };
            xmlLayout.ActivateOptions();

            this.rollingAppender = new RollingFileAppender()
            {
                AppendToFile = true,
                File = Path.Combine(AppUtility.UserAppDataPath, "logs", "log"),
                DatePattern = "_yyyy-MM-dd'.xml'",
                Layout = xmlLayout,
                Encoding = Encoding.UTF8,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "1GB",
                RollingStyle = RollingFileAppender.RollingMode.Date,
                StaticLogFileName = false,
                LockingModel = new RollingFileAppender.MinimalLock()
            };
            this.rollingAppender.ActivateOptions();

            var patternLayout = new PatternLayout()
            {
                ConversionPattern = conversionPattern
            };
            patternLayout.ActivateOptions();

            this.consoleAppender = new ConsoleAppender()
            {
                Layout = patternLayout
            };
            this.consoleAppender.AddFilter(new log4net.Filter.LevelRangeFilter()
            {
                LevelMin = GetLevel(this.verbose),
                LevelMax = Level.Fatal,
            });
            this.consoleAppender.ActivateOptions();

            this.hierarchy.Root.AddAppender(this.consoleAppender);
            this.hierarchy.Root.AddAppender(this.rollingAppender);

            this.hierarchy.Root.Level = Level.All;
            this.hierarchy.Root.Additivity = false;
            this.hierarchy.Configured = true;

            this.log = log4net.LogManager.GetLogger(repositoryName, name);
            this.name = name;
        }

        public LogService(string name, string path)
        {
            this.hierarchy = (Hierarchy)LogManager.GetRepository();

            var xmlLayout = new XmlLayoutSchemaLog4j()
            {
                LocationInfo = true
            };
            xmlLayout.ActivateOptions();

            this.rollingAppender = new RollingFileAppender()
            {
                AppendToFile = true,
                File = Path.Combine(path, "log"),
                DatePattern = "_yyyy-MM-dd'.xml'",
                Layout = xmlLayout,
                Encoding = Encoding.UTF8,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "1GB",
                RollingStyle = RollingFileAppender.RollingMode.Date,
                StaticLogFileName = false
            };
            this.rollingAppender.ActivateOptions();

            var patternLayout = new PatternLayout()
            {
                ConversionPattern = conversionPattern
            };
            patternLayout.ActivateOptions();

            this.consoleAppender = new ConsoleAppender()
            {
                Layout = patternLayout
            };
            this.consoleAppender.AddFilter(new log4net.Filter.LevelRangeFilter()
            {
                LevelMin = GetLevel(this.verbose),
                LevelMax = Level.Fatal,
            });
            this.consoleAppender.ActivateOptions();

            this.hierarchy.Root.AddAppender(this.consoleAppender);
            this.hierarchy.Root.AddAppender(this.rollingAppender);

            this.hierarchy.Root.Level = Level.All;
            this.hierarchy.Configured = true;

            this.log = log4net.LogManager.GetLogger(name);
            this.name = name;
        }

        public LogService(string repositoryName, string name, string path)
        {
            var repository = LogManager.GetAllRepositories().Where(item => item.Name == repositoryName).FirstOrDefault();
            if (repository != null)
            {
                //this.log = log4net.LogManager.GetLogger(repositoryName, name);
                this.hierarchy = (Hierarchy)LogManager.GetRepository(repositoryName);
            }
            else
            {
                this.hierarchy = (Hierarchy)LogManager.CreateRepository(repositoryName);
            }

            var xmlLayout = new XmlLayoutSchemaLog4j()
            {
                LocationInfo = true
            };
            xmlLayout.ActivateOptions();

            this.rollingAppender = new RollingFileAppender()
            {
                AppendToFile = true,
                File = Path.Combine(path, @"logs", $"{repositoryName}_{name}"),
                DatePattern = "_yyyy-MM-dd'.xml'",
                Layout = xmlLayout,
                Encoding = Encoding.UTF8,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "1GB",
                RollingStyle = RollingFileAppender.RollingMode.Date,
                StaticLogFileName = false
            };
            this.rollingAppender.ActivateOptions();

            var patternLayout = new PatternLayout()
            {
                ConversionPattern = conversionPattern
            };
            patternLayout.ActivateOptions();

            this.consoleAppender = new ConsoleAppender()
            {
                Layout = patternLayout
            };
            this.consoleAppender.AddFilter(new log4net.Filter.LevelRangeFilter()
            {
                LevelMin = GetLevel(this.verbose),
                LevelMax = Level.Fatal,
            });
            this.consoleAppender.ActivateOptions();

            this.hierarchy.Root.AddAppender(this.consoleAppender);
            this.hierarchy.Root.AddAppender(this.rollingAppender);

            this.hierarchy.Root.Level = Level.All;
            this.hierarchy.Configured = true;

            this.log = log4net.LogManager.GetLogger(repositoryName, name);
            this.name = name;
        }

        public void Debug(object message)
        {
            this.log.Debug(message);
        }

        public void Info(object message)
        {
            this.log.Info(message);
        }

        public void Error(object message)
        {
            this.log.Error(message);
        }

        public void Warn(object message)
        {
            this.log.Warn(message);
        }

        public void Fatal(object message)
        {
            this.log.Fatal(message);
        }

        public void Dispose()
        {
            this.consoleAppender = null;
            this.redirectionWriter = null;
            this.hierarchy?.Shutdown();
            this.hierarchy = null;
        }

        public LogVerbose Verbose
        {
            get { return this.verbose; }
            set
            {
                if (this.verbose == value)
                    return;
                this.verbose = value;
                this.consoleAppender.ClearFilters();
                this.consoleAppender.AddFilter(new log4net.Filter.LevelRangeFilter()
                {
                    LevelMin = GetLevel(this.verbose),
                    LevelMax = Level.Fatal,
                });
                this.consoleAppender.ActivateOptions();
            }
        }

        public TextWriter RedirectionWriter
        {
            get { return this.redirectionWriter; }
            set
            {
                if (this.redirectionWriter == value)
                    return;

                var oldWriter = this.redirectionWriter;

                this.redirectionWriter = value;

                if (this.redirectionWriter != null)
                {
                    var patternLayout = new PatternLayout()
                    {
                        ConversionPattern = conversionPattern
                    };
                    patternLayout.ActivateOptions();

                    this.textAppender = new TextWriterAppender()
                    {
                        Layout = patternLayout,
                        Writer = this.redirectionWriter,
                    };
                    this.textAppender.ActivateOptions();
                    this.hierarchy.Root.AddAppender(this.textAppender);
                }
                else
                {
                    if (this.textAppender != null)
                    {
                        this.hierarchy.Root.RemoveAppender(this.textAppender);
                    }
                }
            }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string FileName
        {
            get
            {
                if (this.rollingAppender == null)
                    return string.Empty;
                return this.rollingAppender.File;
            }
        }

        private static Level GetLevel(LogVerbose verbose)
        {
            if (verbose == LogVerbose.Debug)
                return Level.Debug;
            else if (verbose == LogVerbose.Info)
                return Level.Info;
            else if (verbose == LogVerbose.Error)
                return Level.Error;
            else if (verbose == LogVerbose.Warn)
                return Level.Warn;
            else
                return Level.Fatal;
        }
    }
}
