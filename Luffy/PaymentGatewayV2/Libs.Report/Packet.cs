using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class Packet
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public string Name { get; set; }
        public string ProviderCode { get; set; }
        public int NumberCard { get; set; }
        public int NumberCardSole { get; set; }
        public int NumberCardUp { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpireDate { get; set; }
        public int CardValue { get; set; }
        public string CardType { get; set; }
        public int Status { get; set; }
        public int ReturnValue { get; set; }

        public Packet() { }
        public Packet Get()
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            return db.GetInstanceSP<Packet>("sp_Packet_Select"
                , new SqlParameter("@Id", Id));
        }
        public Packet GetbyCard(string CardSerial, string CardCode)
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = string.IsNullOrEmpty(CardSerial) ? new SqlParameter("@CardSerial", DBNull.Value) : new SqlParameter("@CardSerial", CardSerial);
            pars[1] = string.IsNullOrEmpty(CardCode) ? new SqlParameter("@CardCode", DBNull.Value) : new SqlParameter("@CardCode", CardCode);
            return db.GetInstanceSP<Packet>("sp_Packet_Select_byCardStore", pars);
        }
        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            db.ExecuteNonQuerySP("sp_Packet_Delete"
                , new SqlParameter("@Id", Id));
        }
        public void DeletebyOrderId(long OrderId)
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            db.ExecuteNonQuerySP("sp_Packet_Delete_byOrderId"
                , new SqlParameter("@OrderId", OrderId));
        }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[13];            
            pars[0] = new SqlParameter("@OrderId", OrderId);
            pars[1] = new SqlParameter("@OrderNo", OrderNo);
            pars[2] = new SqlParameter("@Name", Name);
            pars[3] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[4] = new SqlParameter("@NumberCard", NumberCard);
            pars[5] = new SqlParameter("@NumberCardSole", NumberCardSole);
            pars[6] = new SqlParameter("@NumberCardUp", NumberCardUp);
            pars[7] = new SqlParameter("@IsActive", IsActive);
            pars[8] = new SqlParameter("@ExpireDate", ExpireDate);
            pars[9] = new SqlParameter("@CardValue", CardValue);
            pars[10] = new SqlParameter("@CardType", CardType);
            pars[11] = new SqlParameter("@Status", Status);
            pars[12] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            db.ExecuteNonQuerySP("sp_Packet_Insert", pars);
            Id = Convert.ToInt32(pars[12].Value);
        } 
        // Lấy báo cáo theo nhà cung cấp
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[14];
            pars[0] = new SqlParameter("@Id", Id);
            pars[1] = new SqlParameter("@OrderId", OrderId);
            pars[2] = new SqlParameter("@OrderNo", OrderNo);
            pars[3] = new SqlParameter("@Name", Name);
            pars[4] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[5] = new SqlParameter("@NumberCard", NumberCard);
            pars[6] = new SqlParameter("@NumberCardSole", NumberCardSole);
            pars[7] = new SqlParameter("@NumberCardUp", NumberCardUp);
            pars[8] = new SqlParameter("@IsActive", IsActive);
            pars[9] = new SqlParameter("@ExpireDate", ExpireDate);
            pars[10] = new SqlParameter("@CardValue", CardValue);
            pars[11] = new SqlParameter("@CardType", CardType);
            pars[12] = new SqlParameter("@Status", Status);
            pars[13] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("sp_Packet_Update", pars);
            ReturnValue = Convert.ToInt32(pars[13].Value);
        }
        public int Update_byOrderId(long OrderId, string ProviderCode, int Status)
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3]; 
            pars[0] = new SqlParameter("@OrderId", OrderId); 
            pars[1] = new SqlParameter("@ProviderCode", ProviderCode);  
            pars[2] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output }; 
            db.ExecuteNonQuerySP("sp_Packet_Update_byOrderId", pars);
            return Convert.ToInt32(pars[2].Value);
        }

        public DataTable GetTable(int? PackId, int? OrderId, string OrderNo, string ProviderCode, int? NumberCard, int? NumberCardSole, bool? IsActive, int? CardValue, string CardType, int? Status, int? PageNumber, int? PageSize, out int TotalRecord)
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = PackId == null ? new SqlParameter("@PackId", DBNull.Value) : new SqlParameter("@PackId", PackId);
            pars[1] = OrderId == null ? new SqlParameter("@OrderId", DBNull.Value) : new SqlParameter("@OrderId", OrderId);
            pars[2] = string.IsNullOrEmpty(OrderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", "'" + OrderNo + "'");
            pars[3] = string.IsNullOrEmpty(ProviderCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", "'" + ProviderCode + "'");
            pars[4] = NumberCard == null ? new SqlParameter("@NumberCard", DBNull.Value) : new SqlParameter("@NumberCard", NumberCard);
            pars[5] = NumberCardSole == null ? new SqlParameter("@NumberCardSole", DBNull.Value) : new SqlParameter("@NumberCardSole", NumberCardSole);
            pars[6] = IsActive == null ? new SqlParameter("@IsActive", DBNull.Value) : new SqlParameter("@IsActive", IsActive);
            pars[7] = CardValue == null ? new SqlParameter("@CardValue", DBNull.Value) : new SqlParameter("@CardValue", CardValue);
            pars[8] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", "'" + CardType + "'");
            pars[9] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[10] = PageNumber == null ? new SqlParameter("@PageNumber", 1) : new SqlParameter("@PageNumber", PageNumber);
            pars[11] = PageSize == null ? new SqlParameter("@PageSize", int.MaxValue) : new SqlParameter("@PageSize", PageSize);
            pars[12] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }; var data = db.GetDataTableSP("sp_Packet_SelectList", pars);
            TotalRecord = Convert.ToInt32(pars[12].Value);
            return data;
        }
        public List<Packet> GetListPage(int? PackId, long? OrderId, string OrderNo, string ProviderCode, int? NumberCard, int? NumberCardSole, bool? IsActive, int? CardValue, string CardType, int? Status, int? PageNumber, int? PageSize, out int TotalRecord)
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = PackId == null ? new SqlParameter("@PackId", DBNull.Value) : new SqlParameter("@PackId", PackId);
            pars[1] = OrderId == null ? new SqlParameter("@OrderId", DBNull.Value) : new SqlParameter("@OrderId", OrderId);
            pars[2] = string.IsNullOrEmpty(OrderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", "'" + OrderNo + "'");
            pars[3] = string.IsNullOrEmpty(ProviderCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", "'" + ProviderCode + "'");
            pars[4] = NumberCard == null ? new SqlParameter("@NumberCard", DBNull.Value) : new SqlParameter("@NumberCard", NumberCard);
            pars[5] = NumberCardSole == null ? new SqlParameter("@NumberCardSole", DBNull.Value) : new SqlParameter("@NumberCardSole", NumberCardSole);
            pars[6] = IsActive == null ? new SqlParameter("@IsActive", DBNull.Value) : new SqlParameter("@IsActive", IsActive);
            pars[7] = CardValue == null ? new SqlParameter("@CardValue", DBNull.Value) : new SqlParameter("@CardValue", CardValue);
            pars[8] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", "'" + CardType + "'");
            pars[9] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[10] = PageNumber == null ? new SqlParameter("@PageNumber", 1) : new SqlParameter("@PageNumber", PageNumber);
            pars[11] = PageSize == null ? new SqlParameter("@PageSize", int.MaxValue) : new SqlParameter("@PageSize", PageSize);
            pars[12] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var data = db.GetListSP<Packet>("sp_Packet_SelectList", pars);
            TotalRecord = Convert.ToInt32(pars[12].Value);
            return data;
        }

        public List<Packet> GetListMonitorPage(int? status, string orderNo, string providerCode, string name, string cardType, int? PageNumber, int? PageSize, out int TotalRecord)
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
            pars[1] = string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@TransactionID", providerCode);
            pars[2] = string.IsNullOrEmpty(providerCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", providerCode);
            pars[3] = string.IsNullOrEmpty(name) ? new SqlParameter("@Name", DBNull.Value) : new SqlParameter("@Name", name);
            pars[4] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@Name", cardType);
            pars[5] = PageNumber == null ? new SqlParameter("@PageNumber", 1) : new SqlParameter("@PageNumber", PageNumber);
            pars[6] = PageSize == null ? new SqlParameter("@PageSize", int.MaxValue) : new SqlParameter("@PageSize", PageSize);
            pars[7] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var data = db.GetListSP<Packet>("sp_Packet_Monitor_SelectList", pars);
            TotalRecord = Convert.ToInt32(pars[7].Value);
            return data;
        }

        public List<Packet> GetListAutoBuy(int? PageNumber, int? PageSize, out int TotalRecord)
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = PageNumber == null ? new SqlParameter("@PageNumber", 1) : new SqlParameter("@PageNumber", PageNumber);
            pars[1] = PageSize == null ? new SqlParameter("@PageSize", int.MaxValue) : new SqlParameter("@PageSize", PageSize);
            pars[2] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var data = db.GetListSP<Packet>("sp_Packet_AutoBuy_SelectList", pars);
            TotalRecord = Convert.ToInt32(pars[2].Value);
            return data;
        }
    }
}
