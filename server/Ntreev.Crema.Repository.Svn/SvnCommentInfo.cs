using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Repository.Svn
{
    struct SvnCommentInfo
    {
        public string Comment { get; set; }

        public LogPropertyInfo[] Properties { get; set; }
    }
}
