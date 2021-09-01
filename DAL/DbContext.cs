using log4net;
using SleckEc2KdInterface.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleckEc2KdInterface.DAL
{

    public class DbContext
    {
        private static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlSugarClient Db { get; set; }
        public SqlSugarClient EcDb { get; set; }

        public SqlSugarClient TBDb { get; set; }
        public DbContext(bool  isEc)
        {
            if (!isEc)
            {
               
                Db = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["sqlconnstr"].ConnectionString,
                    DbType = SqlSugar.DbType.SqlServer,
                    IsAutoCloseConnection = true
                });
               
            }
            else
            {
                EcDb = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["sqljdconnstr"].ConnectionString,
                    DbType = SqlSugar.DbType.SqlServer,
                    IsAutoCloseConnection = true
                });
                TBDb = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["sqljdconnstr"].ConnectionString,
                    DbType = SqlSugar.DbType.SqlServer,
                    IsAutoCloseConnection = true
                });

            }
        }
        


        public DbSet<T_Sale_DeliverInfo> DeliverInfoDb { get { return new DbSet<T_Sale_DeliverInfo>(Db); } }
        public DbSet<T_Sale_DeliverItem> DeliverItemDb { get { return new DbSet<T_Sale_DeliverItem>(Db); } }
        public DbSet<PurStockInfo> PurStockInfoDb { get { return new DbSet<PurStockInfo>(Db); } }

        public DbSet<U_Plans_Wdtlogistics> u_Plans_Wdtlogistics { get { return new DbSet<U_Plans_Wdtlogistics>(Db); } }
        public DbSet<PurStockItem> PurStocItemDb { get { return new DbSet<PurStockItem>(Db); } }
        public DbSet<T_Sale_StockOutInfo> SaleStockOutInfoDb { get { return new DbSet<T_Sale_StockOutInfo>(EcDb); } }
        public DbSet<T_Sale_StockOutItem> SaleStockOutItemDb { get { return new DbSet<T_Sale_StockOutItem>(EcDb); } }
        public DbSet<T_Sale_ReturnStockInfo> SaleReturnStockInfoDb { get { return new DbSet<T_Sale_ReturnStockInfo>(EcDb); } }
        public DbSet<T_Sale_ReturnStockItem> SaleReturnStockItemDb { get { return new DbSet<T_Sale_ReturnStockItem>(EcDb); } }

       

        public DbSet<detailslist> detailslistTBDb { get { return new DbSet<detailslist>(TBDb); } }
        public DbSet<wdtstockout> wdtstockoutTBDb { get { return new DbSet<wdtstockout>(TBDb); } }
        //  public DbSet<wdtthOrder> wdtthOrderTBDb { get { return new DbSet<wdtthOrder>(TBDb); } }
        //  public DbSet<wdtthOrderinfo> wdtthOrderinfoTBDb { get { return new DbSet<wdtthOrderinfo>(TBDb); } }
        public DbSet<wdtthstockin> wdtthOrderTBDb { get { return new DbSet<wdtthstockin>(TBDb); } }
        public DbSet<wdtthdetailslist> wdtthOrderinfoTBDb { get { return new DbSet<wdtthdetailslist>(TBDb); } }


    }
}
