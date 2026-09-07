using Libs.Db;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.API
{
    public class MomoOCRDictionary
    {
        public long Id { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public DateTime UpdateTime { get; set; }
        public int? Count { get; set; }
        public int? Status { get; set; }

        public List<MomoOCRDictionary> GetList(int top, string source, string destination, int? status)
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@Top", top);
            pars[1] = string.IsNullOrEmpty(source) ? new SqlParameter("@Source", DBNull.Value) : new SqlParameter("@Source", source);
            pars[2] = string.IsNullOrEmpty(destination) ? new SqlParameter("@Destination", DBNull.Value) : new SqlParameter("@Destination", destination);
            pars[3] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
            return db.GetListSP<MomoOCRDictionary>("sp_OCRDictionary_CMS_SelectList", pars);
        }

        public int Add()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Source", Source);
                pars[2] = new SqlParameter("@Destination", Destination);
                pars[3] = new SqlParameter("@Status", Status);
                db.ExecuteNonQuerySP("sp_OCRDictionary_CMS_Add", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", "sp_OCRDictionary_CMS_Add", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }

        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            db.ExecuteNonQuerySP("sp_OCRDictionary_CMS_Delete", new SqlParameter("@Id", Id));
        }

        public void Update()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", Id);
                pars[2] = string.IsNullOrEmpty(Source) ? new SqlParameter("@Source", DBNull.Value) : new SqlParameter("@Source", Source);
                pars[3] = string.IsNullOrEmpty(Destination) ? new SqlParameter("@Destination", DBNull.Value) : new SqlParameter("@Destination", Source);
                pars[4] = Count == null ? new SqlParameter("@Count", DBNull.Value) : new SqlParameter("@Count", Count);
                pars[5] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status); 
                db.ExecuteNonQuerySP("sp_OCRDictionary_CMS_Update", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", "sp_OCRDictionary_CMS_Update", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }

        }
    }
}
