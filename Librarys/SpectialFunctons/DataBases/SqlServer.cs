using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFunctions.DataBases
{
    public class SqlServer
    {
        string connString = string.Empty;
        public SqlServer(string dbString)
        {
            connString = dbString;
        }

        public object GetValue(string sql)
        {
            try
            {
                object obj = null;
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(sql, conn);
                    obj = cmd.ExecuteScalar();
                    conn.Close();
                }
                return obj;
            }
            catch
            {
                return null;
            }
        }

        public DataSet GetDataSet(string sql)
        {
            try
            {
                using (var conn = new SqlConnection(connString))
                {
                    var da = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public bool Execute(string sql)
        {
            try
            {
                int i = 0;
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(sql, conn);
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }

    public class Connection
    {
        public string Server;
        public string Database;
        public string UID;
        public string PWD;

        public override string ToString()
        {
            return string.Format("server={0};database={1};uid={2};pwd={3};"
                , Server, Database, UID, PWD);
        }
    }
}
