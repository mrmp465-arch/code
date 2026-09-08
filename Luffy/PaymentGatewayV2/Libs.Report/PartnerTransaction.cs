using Libs.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Report
{
    public class PartnerTransaction
    {

        public DateTime CreatedTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public string PartnerCode { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int Id { get; set; }
        public long Amount { get; set; }
        public long Balance { get; set; }
        public List<PartnerTransaction> GetList(string partnerCode,int type)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@PartnerCodes", partnerCode);
            pars[1] = new SqlParameter("@Type", type);
            //DataTable dt = db.GetDataTableSP("sp_PartnersDiscount_Select", pars);           
            //return dt;
            return db.GetListSP<PartnerTransaction>("sp_PartnerTransaction_Select", pars);
        }
        public long Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[2] = new SqlParameter("@Amount", Amount);
            pars[3] = new SqlParameter("@Note", Note);
            pars[4] = new SqlParameter("@Type", Type);
            db.ExecuteNonQuerySP("sp_PartnerTransaction_Insert", pars);
            return Convert.ToInt64(pars[0].Value);
        }
        public PartnerTransaction Get(int partnerID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<PartnerTransaction>("sp_PartnerTransaction_Get"
                , new SqlParameter("@Id", partnerID));
        }
        public int Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = new SqlParameter("@Status", Status);
            db.ExecuteNonQuerySP("sp_PartnerTransaction_Update", pars);
            return Convert.ToInt32(pars[0].Value);
        }

        public int Confirm()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
           
            db.ExecuteNonQuerySP("sp_PartnerTransaction_Confirm", pars);
            return Convert.ToInt32(pars[0].Value);
        }
    }

}
