using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class PartnerProvider
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public int ProviderId { get; set; }
        public string ProviderCode { get; set; }
        public int ReturnValue { get; set; }
        public PartnerProvider()
        {

        }
        //public PartnerProvider Get(int UserId)
        //{
        //    DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
        //    return db.GetInstanceSP<PartnerProvider>("sp_UserPartner_Select"
        //        , new SqlParameter("@UserId", UserId));
        //}

        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            var oCommand = new SqlCommand("sp_PartnerProvider_Delete");
            oCommand.CommandType = CommandType.StoredProcedure;
            oCommand.Parameters.Add(new SqlParameter("@Id", this.Id));
            db.ExecuteNonQuery(oCommand);
        }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@PartnerId", PartnerId);
            pars[2] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[3] = new SqlParameter("@ProviderId", ProviderId);
            pars[4] = new SqlParameter("@ProviderCode", ProviderCode);

            db.ExecuteNonQuerySP("sp_PartnerProvider_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        //public void Update()
        //{
        //    DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
        //    SqlParameter[] pars = new SqlParameter[4];
        //    pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //    pars[1] = new SqlParameter("@UserId", UserId);  
        //    pars[2] = new SqlParameter("@PartnerId", PartnerId);
        //    pars[3] = new SqlParameter("@PartnerCode", PartnerCode); 
        //    db.ExecuteNonQuerySP("sp_UserPartner_Update", pars);
        //    ReturnValue = Convert.ToInt32(pars[0].Value);
        //}
        public List<PartnerProvider> GetList(int partnerId, int providerId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<PartnerProvider>("sp_PartnerProvider_SelectList"
                , partnerId == 0 ? new SqlParameter("@PartnerID", DBNull.Value) : new SqlParameter("@PartnerID", partnerId)
                , providerId == 0 ? new SqlParameter("@ProviderID", DBNull.Value) : new SqlParameter("@ProviderID", providerId)
                );

            
        }

        //public List<PartnerProvider> GetListByUser(int UserId)
        //{
        //    DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
        //    return db.GetListSP<PartnerProvider>("sp_UserPartner_Select" , new SqlParameter("@UserId", UserId));
        //}
    }
}
