using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleckEc2KdInterface.Entity
{
    internal class OrderOutdata
    {

        public string cust_id { get; set; }
        public DateTime fdate { get; set; }
        public string src_tid { get; set; }
        public decimal totalnum { get; set; }
        public decimal share_amount { get; set; }
    }
}
