using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFunctions.DataBases
{
    public class SqlServerExample1
    {
        SqlServer db;

        public SqlServerExample1()
        {
            Connection conn = new Connection();
            conn.Server = "192.168.10.180";
            conn.Database = "HDTSS";
            conn.UID = "ekontrol";
            conn.PWD = "ek123456";

            db = new SqlServer(conn.ToString());
        }

        public DataTable GetTable()
        {
            DataTable dt = db.GetDataSet("Select * from MES_Share_比对条码").Tables[0];
            return dt;
        }

        public bool InsertOneRow()
        {
            return db.Execute("insert into MES_Share_比对条码 (测试条码,工位编号,测试项,测试项类型,测试结果,员工编号,测试时间) values('P-0008-EKH21701003-1805230001-04.62.60.01.0101','OP_01_08_06','条码比对','检验','目标版本:04.62.60.01.0101;实际版本:04.62.60.01.0201;比对结果:不合格','8888','2018-06-08 10:35:05');");
        }

        public bool UpdateData()
        {
            return db.Execute("UPDATE MES_Share_比对条码 SET 员工编号 = 'OP_01_08_06', 测试时间 = '2018-06-08 09:30:01' where 序号='1'");
        }
    }
}
