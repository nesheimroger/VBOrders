using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBOrders
{
    public class VismaBusinessColumnAttribute : Attribute
    {
        public VismaBusinessColumnAttribute(long columnId)
        {
            ColumnId = columnId;
        }

        public long ColumnId { get; set; }
    }
}
