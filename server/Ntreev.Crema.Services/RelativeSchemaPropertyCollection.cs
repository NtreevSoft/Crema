using Ntreev.Crema.Data;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

        public string RelativePath => this[nameof(RelativePath)] as string;
    }
}
