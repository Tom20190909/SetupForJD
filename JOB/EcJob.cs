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
                stockOutInOrderQuery.GetPurData();
                stockOutInOrderQuery.GetSaleOutData();
                stockOutInOrderQuery.GetSaleReturnData();

            });
        }
    }
}
