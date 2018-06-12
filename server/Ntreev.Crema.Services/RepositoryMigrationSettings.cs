using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services
{
    public class RepositoryMigrationSettings
    {
        private const string migrationString = "migration";
        private string fileType;
        private string repositoryModule;
        private string[] dataBaseNames;

        public string BasePath
        {
            get; set;
        }

        public string FileType
        {
            get => this.fileType ?? CremaSettings.DefaultFileType;
            set => this.fileType = value;
        }

        public LogVerbose Verbose
        {
            get; set;
        }

        public string RepositoryModule
        {
            get => this.repositoryModule ?? CremaSettings.DefaultRepositoryModule;
            set => this.repositoryModule = value;
        }

        public bool Force
        {
            get; set;
        }

        public string[] DataBaseNames
        {
            get => this.dataBaseNames ?? new string[] { };
            set => this.dataBaseNames = value;
        }

        public string TempPath
        {
            get; set;
        }

        public readonly static RepositoryMigrationSettings Default = new RepositoryMigrationSettings();

        internal string GetTempPath(string repositoryName)
        {
            if (this.TempPath == null)
            {
                return Path.Combine(this.BasePath, migrationString, repositoryName);
            }
            return Path.Combine(this.TempPath, repositoryName);
        }

        internal string GetTempPath()
        {
            if (this.TempPath == null)
            {
                return Path.Combine(this.BasePath, migrationString);
            }
            return this.TempPath;
        }
    }
}
