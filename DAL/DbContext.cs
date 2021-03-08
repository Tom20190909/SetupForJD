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
        public SqlSugarClient TBDb { get; set; }
        public DbContext(bool  istb)
        {
            if (istb)
            {
               
                TBDb = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["sqlconnstrtb"].ConnectionString,
                    DbType = SqlSugar.DbType.SqlServer,
                    IsAutoCloseConnection = true
                });
                TBDb.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Logger.Info(sql);
                    Logger.Info(TBDb.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                };
            }
            else
            {
                Db = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["sqlconnstr"].ConnectionString,
                    DbType = SqlSugar.DbType.SqlServer,
                    IsAutoCloseConnection = true
                });
                Db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Logger.Info(sql);
                    Logger.Info(Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                };
            }
        }
        


        public DbSet<T_Sale_DeliverInfo> DeliverInfoDb { get { return new DbSet<T_Sale_DeliverInfo>(Db); } }
        public DbSet<T_Sale_DeliverItem> DeliverItemDb { get { return new DbSet<T_Sale_DeliverItem>(Db); } }
        public DbSet<PurStockInfo> PurStockInfoDb { get { return new DbSet<PurStockInfo>(Db); } }
        public DbSet<PurStockItem> PurStocItemDb { get { return new DbSet<PurStockItem>(Db); } }
        public DbSet<T_Sale_StockOutInfo> SaleStockOutInfoDb { get { return new DbSet<T_Sale_StockOutInfo>(Db); } }
        public DbSet<T_Sale_StockOutItem> SaleStockOutItemDb { get { return new DbSet<T_Sale_StockOutItem>(Db); } }
        public DbSet<T_Sale_ReturnStockInfo> SaleReturnStockInfoDb { get { return new DbSet<T_Sale_ReturnStockInfo>(Db); } }
        public DbSet<T_Sale_ReturnStockItem> SaleReturnStockItemDb { get { return new DbSet<T_Sale_ReturnStockItem>(Db); } }


        public DbSet<detailslist> detailslistTBDb { get { return new DbSet<detailslist>(TBDb); } }
        public DbSet<wdtstockout> wdtstockoutTBDb { get { return new DbSet<wdtstockout>(TBDb); } }
      //  public DbSet<wdtthOrder> wdtthOrderTBDb { get { return new DbSet<wdtthOrder>(TBDb); } }
      //  public DbSet<wdtthOrderinfo> wdtthOrderinfoTBDb { get { return new DbSet<wdtthOrderinfo>(TBDb); } }
        public DbSet<wdtthstockin> wdtthOrderTBDb { get { return new DbSet<wdtthstockin>(TBDb); } }
        public DbSet<wdtthdetailslist> wdtthOrderinfoTBDb { get { return new DbSet<wdtthdetailslist>(TBDb); } }
       
    }
}
