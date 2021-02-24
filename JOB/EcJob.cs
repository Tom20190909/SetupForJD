using Quartz;
using SleckEc2KdInterface.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleckEc2KdInterface.JOB
{
    [DisallowConcurrentExecution]
    class EcJob : IJob
    {
        StockOutInOrderQuery stockOutInOrderQuery = new StockOutInOrderQuery();
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                //采购入库单
               stockOutInOrderQuery.GetPurData();
                //销售出库单
               stockOutInOrderQuery.GetSaleOutData();
                //销售退货单
               stockOutInOrderQuery.GetSaleReturnData();
                //奇门销售出库单
               stockOutInOrderQuery.GetTBSaleOutData();
                //奇门销售退货单


            });
        }
    }
}
