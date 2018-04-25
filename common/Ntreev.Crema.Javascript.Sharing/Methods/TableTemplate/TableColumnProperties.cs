using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Ntreev.Crema.Javascript.Methods.TableTemplate
{
    [Category(nameof(TableTemplate))]
    enum TableColumnProperties
    {
        Index,

        IsKey,

        IsUnique,

        Name,

        DataType,

        DefaultValue,

        Comment,

        AutoIncrement,

        Tags,

        IsReadOnly,

        AllowNull
    }
}
