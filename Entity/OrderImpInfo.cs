using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleckEc2KdInterface.Entity
{
    public class OrderImpInfo
    {
        public string 店铺名称 { get; set; }
        public string 发货仓库 { get; set; }
        public string 收件人 { get; set; }
        public string 手机 { get; set; }

        public string 地址 { get; set; }
        public string 商家编码 { get; set; }
        public string 货品数量 { get; set; }
        public string 平台单号 { get; set; }
    }
}
