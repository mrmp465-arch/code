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
    public class CardOrderPacket
    {
        public DateTime Time { get; set; }
        public int Status { get; set; }
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public int NumberCard { get; set; }
        public int CardValue { get; set; }
        public string CardType { get; set; }
        public string Data { get; set; }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@OrderNo", OrderNo);
            pars[2] = new SqlParameter("@OrderId", OrderId);
            pars[3] = new SqlParameter("@Status", Status);
            pars[4] = new SqlParameter("@CardType", CardType);
            pars[5] = new SqlParameter("@CardValue", CardValue);
            pars[6] = new SqlParameter("@NumberCard", NumberCard);
            db.ExecuteNonQuerySP("sp_CardOrderPacket_Insert", pars);
            Id = Convert.ToInt32(pars[0].Value);
        }
        public DataTable GetTable()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetDataTableSP("sp_CardOrderPacket_SelectList");
        }
        public void Update()
        {

            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@Data", Data);
            pars[4] = new SqlParameter("@CardType", CardType);
            pars[5] = new SqlParameter("@CardValue", CardValue);
            pars[6] = new SqlParameter("@NumberCard", NumberCard);
            db.ExecuteNonQuerySP("sp_CardOrderPacket_Update", pars);
            
        }
       
        public CardOrderPacket Get(int Id)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<CardOrderPacket>("sp_CardOrderPacket_Select", new SqlParameter("@Id", Id));
        }
        public List<CardOrderPacket> GetList(int OrderId)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetListSP<CardOrderPacket>("sp_CardOrderPacket_SelectList", new SqlParameter("@OrderId", OrderId));
        }
        public void Delete()
        {

            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[1];
            //pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[0] = new SqlParameter("@Id", Id);
           

            db.ExecuteNonQuerySP("sp_CardOrderPacket_Delete", pars);

        }
    }
}
