using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public class RepositorySettings
    {
        public string BasePath
        {
            get; set;
        }

        public string RepositoryName
        {
            get; set;
        }

        public string WorkingPath
        {
            get; set;
        }

        public string TransactionPath
        {
            get; set;
        }

        public ILogService LogService
        {
            get; set;
        }

        public static readonly RepositorySettings Default = new RepositorySettings();
    }
}
