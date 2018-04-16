using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Bot
{
    public class TaskStack : Stack<object>
    {
        public object Current
        {
            get { return this.Peek(); }
        }
    }
}
