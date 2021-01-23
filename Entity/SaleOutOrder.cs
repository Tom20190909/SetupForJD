using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleckEc2KdInterface.Entity
{
    class SaleOutOrder
    {
        public List<T_Sale_StockOutInfo> SaleStockOutInfoHead { get; set; }
        public List<T_Sale_StockOutItem> SaleStockOutInfoDetail { get; set; }
    }
}
