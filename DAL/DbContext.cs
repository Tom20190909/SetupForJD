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
        public SqlSugarClient Db { get; set; }
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["sqlconnstr"].ConnectionString,
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = true
            });
        }

        public DbSet<T_Sale_DeliverInfo> DeliverInfoDb { get { return new DbSet<T_Sale_DeliverInfo>(Db); } }
        public DbSet<T_Sale_DeliverItem> DeliverItemDb { get { return new DbSet<T_Sale_DeliverItem>(Db); } }
        public DbSet<PurStockInfo> PurStockInfoDb { get { return new DbSet<PurStockInfo>(Db); } }
        public DbSet<PurStockItem> PurStocItemDb { get { return new DbSet<PurStockItem>(Db); } }
        public DbSet<T_Sale_StockOutInfo> SaleStockOutInfoDb { get { return new DbSet<T_Sale_StockOutInfo>(Db); } }
        public DbSet<T_Sale_StockOutItem> SaleStockOutItemDb { get { return new DbSet<T_Sale_StockOutItem>(Db); } }
        public DbSet<T_Sale_ReturnStockInfo> SaleReturnStockInfoDb { get { return new DbSet<T_Sale_ReturnStockInfo>(Db); } }
        public DbSet<T_Sale_ReturnStockItem> SaleReturnStockItemDb { get { return new DbSet<T_Sale_ReturnStockItem>(Db); } }

    }
}
