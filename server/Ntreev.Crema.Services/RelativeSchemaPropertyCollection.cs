using Ntreev.Library;

namespace Ntreev.Crema.Services
{
    class RelativeSchemaPropertyCollection : SerializationPropertyCollection
    {
        public RelativeSchemaPropertyCollection(string path, string templatedPath)
        {
            if (templatedPath != null)
            {
                var relativeUri = UriUtility.MakeRelative(path, templatedPath);
                this.Add(nameof(RelativePath), relativeUri);
            }
        }

        public string RelativePath => this[nameof(RelativePath)] as string ?? string.Empty;
    }
}
