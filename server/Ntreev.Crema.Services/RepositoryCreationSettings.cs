namespace Ntreev.Crema.Services
{
    public class RepositoryCreationSettings
    {
        private string fileType;
        private string repositoryModule;

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

        public readonly static RepositoryCreationSettings Default = new RepositoryCreationSettings();
    }
}
