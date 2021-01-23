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
        StockinOrderQueryPurchase stockinOrderQueryPurchase = new StockinOrderQueryPurchase();
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                stockinOrderQueryPurchase.GetPurData();
                stockinOrderQueryPurchase.GetSaleOutData();
                stockinOrderQueryPurchase.GetSaleReturnData();

            });
        }
    }
}
