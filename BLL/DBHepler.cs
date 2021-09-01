using System;
using System.Data;
using System.Data.SqlClient;

using System.Text;
using System.Configuration;

namespace SleckWDTInterface.JOB
{
    public class DBHelper
    {
        public static SqlConnection Connection;
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["sqlconnstr"].ToString();

        public static string jdConnectionString = ConfigurationManager.ConnectionStrings["sqljdconnstr"].ToString();


        public static string CloudExecuteSQL(string SqlString)
        {
            string message = "";
            CloudOpen();
            SqlCommand command = new SqlCommand();
            SqlTransaction transaction = Connection.BeginTransaction();
            command.Connection = Connection;
            command.Transaction = transaction;
            try
            {
                command.CommandText = SqlString;
                int Row = command.ExecuteNonQuery();
                if (Row <= 0)
                {
                    message = "";
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                transaction.Rollback();
            }
            finally
            {
                Connection.Close();
            }
            return message;
        }


        public static object CloudGetSingle(string SQLString)
        {
            object obj3;
            using (SqlConnection connection = new SqlConnection(jdConnectionString))
            {
                SqlCommand command = new SqlCommand(SQLString, connection);
                try
                {
                    connection.Open();
                    object objA = command.ExecuteScalar();
                    if (Equals(objA, null) || Equals(objA, DBNull.Value))
                    {
                        connection.Close();
                        return null;
                    }
                    connection.Close();
                    return objA;
                }
                catch (SqlException)
                {
                    connection.Close();
                    return null;
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return obj3;
        }

        public static void Close()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }
        public static int ExecuteSql(string SQLString, params SqlParameter[] cmdParms)
        {
            int num2;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                try
                {
                    cmd.Parameters.Clear();
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    int num = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    connection.Close();
                    return num;
                }
                catch (SqlException)
                {
                    connection.Close();
                    return 0;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            return num2;
        }

        public static string ExecuteSQL(string SqlString)
        {
            string message = "";
            Open();
            SqlCommand command = new SqlCommand();
            SqlTransaction transaction = Connection.BeginTransaction();
            command.Connection = Connection;
            command.Transaction = transaction;
            try
            {
                command.CommandText = SqlString;
                int Row = command.ExecuteNonQuery();
                if (Row <= 0)
                {
                    message = "";
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                transaction.Rollback();
            }
            finally
            {
                Connection.Close();
            }
            return message;
        }

        public static bool ExecuteSQL(string[] SqlStrings)
        {
            bool flag = true;
            Open();
            SqlCommand command = new SqlCommand();
            SqlTransaction transaction = Connection.BeginTransaction();
            command.Connection = Connection;
            command.Transaction = transaction;
            try
            {
                foreach (string str in SqlStrings)
                {
                    if (str.Length > 0)
                    {
                        command.CommandText = str;
                        _ = command.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            catch
            {
                flag = false;
                transaction.Rollback();
            }
            finally
            {
                Connection.Close();
            }
            return flag;
        }

        public static bool ExecuteSqlTran(string SQLString, params SqlParameter[] cmdParms)
        {
            bool flag = true;
            Open();
            SqlCommand cmd = new SqlCommand();
            SqlTransaction transaction = Connection.BeginTransaction();
            cmd.Connection = Connection;
            cmd.Transaction = transaction;
            try
            {
                PrepareCommand(cmd, Connection, null, SQLString, cmdParms);
                cmd.CommandText = SQLString;
                _ = cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                flag = false;
                transaction.Rollback();
            }
            finally
            {
                Connection.Close();
            }
            return flag;
        }

        public static bool Exists(string strSql)
        {
            int num;
            object single = GetSingle(strSql);
            num = Equals(single, null) || Equals(single, DBNull.Value) ? 0 : int.Parse(single.ToString());
            return num != 0;
        }

        public static bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            int num;
            object single = GetSingle(strSql, cmdParms);
            if (object.Equals(single, null) || object.Equals(single, DBNull.Value))
            {
                num = 0;
            }
            else
            {
                num = int.Parse(single.ToString());
            }
            if (num == 0)
            {
                return false;
            }
            return true;
        }

        public static DataRow GetDataRow(string SqlString)
        {
            DataSet dataSet = GetDataSet(SqlString);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                return dataSet.Tables[0].Rows[0];
            }
            return null;
        }

        public static DataSet GetDataSet(string SqlString)
        {
            DataSet set2;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(SqlString, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        try
                        {
                            adapter.Fill(dataSet, "ds");
                            command.Parameters.Clear();
                        }
                        catch (SqlException)
                        {
                        }
                        connection.Close();
                        set2 = dataSet;
                    }
                }
            }
            return set2;
        }

        public static DataTable GetDataforStoredProcedure(string SqlString)
        {
            DataSet set2;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(SqlString);
                cmd.CommandType = CommandType.StoredProcedure;                        //
                                                                                      //   cmd.Parameters.AddRange(sqlParameter);
                cmd.Parameters.Add("@F_SDate", SqlDbType.Date).Value = "2023-02-01";   //

                cmd.Parameters.Add("@F_EDate", SqlDbType.Date).Value = "2023-03-10";    //
                cmd.Parameters.AddWithValue("@F_APPROVETIMEDate", SqlDbType.DateTime).Value = "2023-02-01 00:00:00"; ;
                cmd.Parameters.Add("@F_OutDate", SqlDbType.Date).Value = "2023-03-10";    //
                cmd.Connection= connection;
             
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        try
                        {
                            adapter.Fill(dataSet, "ds");
                           
                        }
                        catch (SqlException)
                        {
                          
                        }
                        connection.Close();
                        set2 = dataSet;
                    }
               
            }
            return set2.Tables[0];
        }

        public static DataTable GetDataTable(string SqlString)
        {
            return GetDataSet(SqlString).Tables[0];
        }

        //分页查询
        public DataTable getOrder(string strSql, string strWhere, string orderby, int startIndex, int endIndex, out int total)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("select * from (SELECT ROW_NUMBER() OVER (order by Row asc) AS Row1,* FROM ( ");
            builder.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                builder.Append("order by " + orderby);
            }
            builder.Append(")AS Row, " + strSql);
            builder.Append(" ) TT");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                builder.Append(" WHERE " + strWhere + ") dd ");
            }
            DataSet set = DBHelper.Query(builder.ToString(), new SqlParameter[0]);
            total = set.Tables[0].Rows.Count;
            builder.AppendFormat(" WHERE Row1 between {0} and {1}", startIndex, endIndex);
            return DBHelper.Query(builder.ToString(), new SqlParameter[0]).Tables[0];
        }

        public static int GetMaxID(string FieldName, string TableName)
        {
            int num = 0;
            DataSet dataSet = GetDataSet("select max(" + FieldName + ") from " + TableName);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                num = int.Parse(dataSet.Tables[0].Rows[0][0].ToString());
            }
            return num;
        }



        public static string GetSHSLInt(string SqlString)
        {
            DataSet dataSet = GetDataSet(SqlString);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                return Convert.ToString(dataSet.Tables[0].Rows[0][0].ToString());
            }
            return "0";
        }

        public static object GetSingle(string SQLString)
        {
            object obj3;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(SQLString, connection);
                try
                {
                    connection.Open();
                    object objA = command.ExecuteScalar();
                    if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                    {
                        connection.Close();
                        return null;
                    }
                    connection.Close();
                    return objA;
                }
                catch (SqlException)
                {
                    connection.Close();
                    return null;
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return obj3;
        }

        public static object GetSingle(string SQLString, params SqlParameter[] cmdParms)
        {
            object obj3;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    object objA = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                    {
                        connection.Close();
                        return null;
                    }
                    connection.Close();
                    return objA;
                }
                catch (SqlException)
                {
                    connection.Close();
                    return null;
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            return obj3;
        }

        public static object GetSingle(string SQLString, int Times)
        {
            object obj3;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(SQLString, connection);
                try
                {
                    connection.Open();
                    command.CommandTimeout = Times;
                    object objA = command.ExecuteScalar();
                    if (object.Equals(objA, null) || object.Equals(objA, DBNull.Value))
                    {
                        connection.Close();
                        return null;
                    }
                    connection.Close();
                    return objA;
                }
                catch (SqlException)
                {
                    connection.Close();
                    return null;
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }
            return obj3;
        }

        public static DataSet GetStatic(string start, string end)
        {
            DataSet set2;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("GetStatic", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@startdate", SqlDbType.NVarChar));
                    command.Parameters["@startdate"].Value = start;
                    command.Parameters.Add(new SqlParameter("@enddate", SqlDbType.NVarChar));
                    command.Parameters["@enddate"].Value = end;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        try
                        {
                            adapter.Fill(dataSet);
                            command.Parameters.Clear();
                        }
                        catch (SqlException)
                        {
                        }
                        connection.Close();
                        set2 = dataSet;
                    }
                }
            }
            return set2;
        }

        public static string GetStringList(string SqlString)
        {
            string str = string.Empty;
            DataSet dataSet = GetDataSet(SqlString);
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                if (str.Length > 0)
                {
                    str = str + "|" + dataSet.Tables[0].Rows[i][0].ToString();
                }
                else
                {
                    str = dataSet.Tables[0].Rows[i][0].ToString();
                }
            }
            return str;
        }


        public static void Open()
        {
            if (Connection == null)
            {
                Connection = new SqlConnection(ConnectionString);
            }
            if (Connection.State.Equals(ConnectionState.Closed))
            {
                Connection.Open();
            }
        }

        public static void CloudOpen()
        {
            if (Connection == null)
            {
                Connection = new SqlConnection(jdConnectionString);
            }
            if (Connection.State.Equals(ConnectionState.Closed))
            {
                Connection.Open();
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if (((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Input)) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            DataSet set2;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        adapter.Fill(dataSet, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SqlException)
                    {
                    }
                    connection.Close();
                    set2 = dataSet;
                }
            }
            return set2;
        }
    }
}
