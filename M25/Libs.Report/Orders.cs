using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class Orders
    {
        public long Id { get; set; }
        public string No { get; set; } 
        public string Name { get; set; }
        public string ProviderCode { get; set; } 
        public int Status { get; set; }
        public int ReturnValue { get; set; }
        public Orders()
        {

        }
        public string GenOrderCode()
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,1,2,3,4,5,6,7,8,9").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i <= 7; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return tmp.ToUpper() + timeSpan.ToString();
        }
        public Orders Get()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            return db.GetInstanceSP<Orders>("sp_Orders_Select"
                , new SqlParameter("@Id", Id));
        }
        /// <summary>
        ///   Orders.Id = Pảck Count
        ///   Orders.ReturnValue = Card Count
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public Orders GetCountCardPacket(int? OrderId, int? PacketId)
        {
          
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
                pars[0] = OrderId == null ? new SqlParameter("@OrderId", DBNull.Value) : new SqlParameter("@OrderId", OrderId);
                pars[1] = PacketId == null ? new SqlParameter("@PacketId", DBNull.Value) : new SqlParameter("@PacketId", PacketId);
            return db.GetInstanceSP<Orders>("[sp_GetCount_CardPacket]" , pars);
        }
        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            db.ExecuteNonQuerySP("sp_Orders_Delete"
                , new SqlParameter("@Id", Id));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5]; 
            pars[0] = new SqlParameter("@No", No);
            pars[1] = new SqlParameter("@Name", Name);
            pars[2] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[3] = new SqlParameter("@Status", Status);
            pars[4] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            db.ExecuteNonQuerySP("sp_Orders_Insert", pars);
            Id = Convert.ToInt32(pars[4].Value);
        } 
        // Lấy báo cáo theo nhà cung cấp
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@Id", Id);
            pars[1] = new SqlParameter("@No", No);
            pars[2] = new SqlParameter("@Name", Name);
            pars[3] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("sp_Orders_Update", pars);
            ReturnValue = Convert.ToInt32(pars[4].Value);
        }
        public DataTable GetTable(int? OrderId, string No, string Name, string ProviderCode, int? Status, int? PageNumber, int? PageSize, out int TotalRecord)
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = OrderId == null ? new SqlParameter("@OrderId", DBNull.Value) : new SqlParameter("@OrderId", OrderId);
            pars[1] = string.IsNullOrEmpty(No) ? new SqlParameter("@No", DBNull.Value) : new SqlParameter("@No","'"+No+"'");
            pars[2] = string.IsNullOrEmpty(Name) ? new SqlParameter("@Name", DBNull.Value) : new SqlParameter("@Name", "'" + Name + "'");
            pars[3] = string.IsNullOrEmpty(ProviderCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", "'" + ProviderCode + "'");
            pars[4] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[5] = PageNumber == null ? new SqlParameter("@PageNumber", 1) : new SqlParameter("@PageNumber", PageNumber);
            pars[6] = PageSize == null ? new SqlParameter("@PageSize", int.MaxValue) : new SqlParameter("@PageSize", PageSize);
            pars[7] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };  
            var data = db.GetDataTableSP("sp_Orders_SelectList", pars);
            TotalRecord = Convert.ToInt32(pars[7].Value);
            return data;
        }
        public List<CardStore> GetListPage(int? OrderId, string No, string Name, string ProviderCode, int? Status, int? PageNumber, int? PageSize, out int TotalRecord)
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = OrderId == null ? new SqlParameter("@OrderId", DBNull.Value) : new SqlParameter("@OrderId", OrderId);
            pars[1] = string.IsNullOrEmpty(No) ? new SqlParameter("@No", DBNull.Value) : new SqlParameter("@No", "'" + No + "'");
            pars[2] = string.IsNullOrEmpty(Name) ? new SqlParameter("@Name", DBNull.Value) : new SqlParameter("@Name", "'" + Name + "'");
            pars[3] = string.IsNullOrEmpty(ProviderCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", "'" + ProviderCode + "'");
            pars[4] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[5] = PageNumber == null ? new SqlParameter("@PageNumber", 1) : new SqlParameter("@PageNumber", PageNumber);
            pars[6] = PageSize == null ? new SqlParameter("@PageSize", int.MaxValue) : new SqlParameter("@PageSize", PageSize);
            pars[7] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var data = db.GetListSP<CardStore>("sp_Orders_SelectList", pars);
            TotalRecord = Convert.ToInt32(pars[7].Value);
            return data;
        }

    }
}
