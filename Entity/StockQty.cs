using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleckEc2KdInterface.Entity
{
    public class StockQty
    {
        public string goods_no { get; set; }
        public string spec_no { get; set; }
        public string barcode { get; set; }

        public string warehouse_no { get; set; }
        public string warehouse_name { get; set; }
        public decimal stock_num { get; set; }
    }
}



