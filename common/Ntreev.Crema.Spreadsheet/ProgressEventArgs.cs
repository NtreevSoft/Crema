using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Spreadsheet
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(object target, int index, int count)
        {
            this.Target = target;
            this.Index = index;
            this.Count = count;
        }

        public object Target { get; }

        public int Index { get; }

        public int Count { get; }
    }
}
