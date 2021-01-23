using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleckEc2KdInterface.Entity
{
    class SaleReturnOrder
    {
        public List<T_Sale_ReturnStockInfo> SaleReturnHead = new List<T_Sale_ReturnStockInfo>();
        public List<T_Sale_ReturnStockItem> SaleReturnDetail = new List<T_Sale_ReturnStockItem>();
    }
}
