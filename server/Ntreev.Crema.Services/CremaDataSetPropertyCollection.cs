using Ntreev.Library;

namespace Ntreev.Crema.Services
{
    class CremaDataSetPropertyCollection : SerializationPropertyCollection
    {
        public CremaDataSetPropertyCollection(string[] typePaths, string[] tablePaths)
        {
            this.Add(nameof(SignatureDateProvider), SignatureDateProvider.Default);
            this.Add(nameof(TypePaths), typePaths ?? new string[] { });
            this.Add(nameof(TablePaths), tablePaths ?? new string[] { });
        }

        public CremaDataSetPropertyCollection(Authentication authentication, string[] typePaths, string[] tablePaths)
        {
            this.Add(nameof(SignatureDateProvider), new SignatureDateProvider(authentication.ID));
            this.Add(nameof(TypePaths), typePaths ?? new string[] { });
            this.Add(nameof(TablePaths), tablePaths ?? new string[] { });
        }

        public SignatureDateProvider SignatureDateProvider => this[nameof(SignatureDateProvider)] as SignatureDateProvider;

        public string[] TypePaths => this[nameof(TypePaths)] as string[];

        public string[] TablePaths => this[nameof(TablePaths)] as string[];

        public bool SchemaOnly
        {
            get
            {
                if (this[nameof(SchemaOnly)] is bool value)
                    return value;
                return false;
            }
            set => this[nameof(SchemaOnly)] = value;
        }
    }
}
