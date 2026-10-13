using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Db;
using System.Data;
using System.Data.SqlClient;
using Libs.Utils;

namespace Libs.API
{
    [Serializable]
    public class CardOrder
    {
        public DateTime Time { get; set; }
        public int Status { get; set; }
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string PartnerCode { get; set; }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@OrderNo", OrderNo);
            pars[2] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[3] = new SqlParameter("@Status", Status);
            db.ExecuteNonQuerySP("sp_CardOrder_Insert", pars);
            Id = Convert.ToInt32(pars[0].Value);
        }
        public DataTable GetTop(int top)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetDataTableSP("sp_CardOrder_SelectList", new SqlParameter("@Top", top));
        }
        public void Update()
        {

            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = new SqlParameter("@Status", Status);
            db.ExecuteNonQuerySP("sp_CardOrder_Update", pars);
            
        }
        public CardOrder Get(int Id)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<CardOrder>("sp_CardOrder_Select", new SqlParameter("@Id", Id));
        }
        public void Delete()
        {

            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[1];
            //pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[0] = new SqlParameter("@Id", Id);
           

            db.ExecuteNonQuerySP("sp_CardOrder_Delete", pars);

        }
    }
}
