using System;
using System.Linq;
using System.Text;

namespace SlekGift.Models
{
    ///<summary>
    ///
    ///</summary>
    public partial class V_CUST
    {
        public V_CUST()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int FCUSTID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string FNUMBER { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int FUSEORGID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string FNAME { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string F_FC_Assistant { get; set; }



        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string F_FC_Assistant1 { get; set; }

        public string FSALDEPTID { get; set; }
        public string FSALGROUPID { get; set; }
        public string FSELLER { get; set; }

        public string CUSTCHANNEL { get; set; }

        public string CONTACTNAME { get; set; }
        public string CUSTAREA { get; set; }
        public string CUSTPROVINCE { get; set; }
        public string FDHType { get; set; }
        public string FSELLENUMBER { get; set; }

        public string FSALENUMBER { get; set; }


        public string FSELLEName { get; set; }

        public string FORGNUMBER { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string FDATAVALUE { get; set; }
        public string FAreaDATAVALUE { get; set; }

        public string FFREEZESTATUS { get; set; }
        public string FDOCUMENTSTATUS { get; set; }

        public string FPASSWORD { get; set; }

        public string FSendIsInCus { get; set; }

    }
}
