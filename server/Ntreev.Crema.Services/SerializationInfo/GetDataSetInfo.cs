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
    class GetDataSetInfo : PropertyCollection
    {
        public GetDataSetInfo(SignatureDateProvider signatureDateProvider, string revision, IRepository repository)
        {
            this.Add(nameof(SignatureDateProvider), signatureDateProvider);
            this.Add(nameof(Revision), revision);
            this.Add(nameof(Repository), repository);
        }

        public SignatureDateProvider SignatureDateProvider => this[nameof(SignatureDateProvider)] as SignatureDateProvider;

        public string Revision => this[nameof(Revision)] as string;

        public IRepository Repository => this[nameof(Repository)] as IRepository;
    }
}
