using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleckEc2KdInterface.Entity
{
    class PurOrder
    {
        public List<PurStockInfo> PurOrderHead { get; set; }
        public List<PurStockItem> PurOrderDetail { get; set; }
    }
}
