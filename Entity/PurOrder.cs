using System.Collections.Generic;

namespace SleckEc2KdInterface.Entity
{
    class PurOrder
    {
        public List<PurStockInfo> PurOrderHead { get; set; }
        public List<PurStockItem> PurOrderDetail { get; set; }
    }
}
