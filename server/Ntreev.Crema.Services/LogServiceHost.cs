using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    class LogServiceHost : ILogService
    {
        private readonly LogService logService;

        public LogServiceHost(LogService logService)
        {
            this.logService = logService;
        }

        public LogServiceHost(string name, string path)
        {
            this.logService = new LogService(name, path);
        }

        public LogVerbose Verbose
        {
            get => this.logService.Verbose;
            set => this.logService.Verbose = value;
        }

        public TextWriter RedirectionWriter
        {
            get => this.logService.RedirectionWriter;
            set => this.logService.RedirectionWriter = value;
        }

        public string Name => this.logService.Name;

        public string FileName => this.logService.FileName;

        public bool IsEnabled => true;

        public void Debug(object message)
        {
            this.logService.Debug(message);
        }

        public void Error(object message)
        {
            this.logService.Error(message);
        }

        public void Fatal(object message)
        {
            this.logService.Fatal(message);
        }

        public void Info(object message)
        {
            this.logService.Info(message);
        }

        public void Warn(object message)
        {
            this.logService.Warn(message);
        }
    }
}
