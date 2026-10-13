using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class CardStore
    {
        public long Id { get; set; } 
        public int PacketId { get; set; }
        public string ProviderCode { get; set; }
        public string CardType { get; set; }
        public string CardSerial { get; set; } 
        public string CardCode { get; set; } 
        public DateTime ExpireDate { get; set; } 
        public int CardValue { get; set; }
        public bool IsSold { get; set; }
        public bool IsActive { get; set; }  
        public int ReturnValue { get; set; }
        
        public CardStore()
        {  
        }
        public CardStore Get()
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            return db.GetInstanceSP<CardStore>("sp_CardStore_Select"
                , new SqlParameter("@Id", Id));
        }
        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            db.ExecuteNonQuerySP("sp_CardStore_Delete"
                , new SqlParameter("@Id", Id));
        }
        public void DeletebyPacketId(long PacketId)
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            db.ExecuteNonQuerySP("sp_CardStore_Delete_byPacketId"
                , new SqlParameter("@PacketId", PacketId));
        }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[10];            
            pars[0] = new SqlParameter("@PacketId", PacketId);
            pars[1] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[2] = new SqlParameter("@CardType", CardType);
            pars[3] = new SqlParameter("@CardSerial", CardSerial);
            pars[4] = new SqlParameter("@CardCode", CardCode);
            pars[5] = new SqlParameter("@ExpireDate", ExpireDate);
            pars[6] = new SqlParameter("@CardValue", CardValue);
            pars[7] = new SqlParameter("@IsSold", IsSold);
            pars[8] = new SqlParameter("@IsActive", IsActive); 
            pars[9] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            db.ExecuteNonQuerySP("sp_CardStore_Insert", pars);
            Id = Convert.ToInt32(pars[9].Value);
        }
        /// Lấy danh sách
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="status">null==all</param> 
        /// <returns></returns>
      
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = new SqlParameter("@Id", Id);
            pars[1] = new SqlParameter("@PacketId", PacketId);
            pars[2] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[3] = new SqlParameter("@CardType", CardType);
            pars[4] = new SqlParameter("@CardSerial", CardSerial);
            pars[5] = new SqlParameter("@CardCode", CardCode);
            pars[6] = new SqlParameter("@ExpireDate", ExpireDate);
            pars[7] = new SqlParameter("@CardValue", CardValue);
            pars[8] = new SqlParameter("@IsSold", IsSold);
            pars[9] = new SqlParameter("@IsActive", IsActive);
          
            pars[10] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output }; 
            db.ExecuteNonQuerySP("sp_CardStore_Update", pars);
            ReturnValue = Convert.ToInt32(pars[10].Value);
        }
        public int Update_byPacketId(long PacketId,string ProviderCode,string CardType, DateTime ExpireDate, int CardValue,bool IsActive)
        {
            DBHelper db = new DBHelper(Configs.CardStoreConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7]; 
            pars[0] = new SqlParameter("@PacketId", PacketId);
            pars[1] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[2] = new SqlParameter("@CardType", CardType); 
            pars[3] = new SqlParameter("@ExpireDate", ExpireDate);
            pars[4] = new SqlParameter("@CardValue", CardValue);
            pars[5] = new SqlParameter("@IsActive", IsActive);
            pars[6] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            db.ExecuteNonQuerySP("sp_CardStore_Update_byPacketId", pars);
            return Convert.ToInt32(pars[6].Value);
        }
        public DataTable GetTable(int? PacketId, string ProviderCode, string CardType, string CardSerial, string CardCode, int? CardValue, bool? IsSold, bool? IsActive, int? PageNumber, int? PageSize, out int TotalRecord)
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = PacketId == null ? new SqlParameter("@PacketId", DBNull.Value) : new SqlParameter("@PacketId", PacketId);
            pars[1] = string.IsNullOrEmpty(ProviderCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", "'" + ProviderCode + "'");
            pars[2] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", "'" + CardType + "'");
            pars[3] = string.IsNullOrEmpty(CardSerial) ? new SqlParameter("@CardSerial", DBNull.Value) : new SqlParameter("@CardSerial", "'" + CardSerial + "'");
            pars[4] = string.IsNullOrEmpty(CardCode) ? new SqlParameter("@CardCode", DBNull.Value) : new SqlParameter("@CardCode", "'" + CardCode + "'");
            pars[5] = CardValue == null ? new SqlParameter("@CardValue", DBNull.Value) : new SqlParameter("@CardValue", CardValue);
            pars[6] = IsSold == null ? new SqlParameter("@IsSold", DBNull.Value) : new SqlParameter("@IsSold", IsSold);
            pars[7] = IsActive == null ? new SqlParameter("@IsActive", DBNull.Value) : new SqlParameter("@IsActive", IsActive);
            pars[8] = PageNumber == null ? new SqlParameter("@PageNumber", 1) : new SqlParameter("@PageNumber", PageNumber);
            pars[9] = PageSize == null ? new SqlParameter("@PageSize", int.MaxValue) : new SqlParameter("@PageSize", PageSize);
            pars[10] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var data = db.GetDataTableSP("sp_CardStore_SelectList", pars);
            TotalRecord = Convert.ToInt32(pars[10].Value);
            return data;
        }
        public List<CardStore> GetListPage(int? PacketId, string ProviderCode, string CardType, string CardSerial, string CardCode, int? CardValue, bool? IsSold, bool? IsActive, int? PageNumber, int? PageSize, out int TotalRecord)
        {
            DBHelper db = new DBHelper(Configs.CardStoreReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = PacketId == null ? new SqlParameter("@PacketId", DBNull.Value) : new SqlParameter("@PacketId", PacketId);
            pars[1] = string.IsNullOrEmpty(ProviderCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", "'"+ ProviderCode + "'");
            pars[2] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", "'" + CardType + "'");
            pars[3] = string.IsNullOrEmpty(CardSerial) ? new SqlParameter("@CardSerial", DBNull.Value) : new SqlParameter("@CardSerial", "'" + CardSerial + "'");
            pars[4] = string.IsNullOrEmpty(CardCode) ? new SqlParameter("@CardCode", DBNull.Value) : new SqlParameter("@CardCode", "'" + CardCode + "'"); 
            pars[5] = CardValue == null ? new SqlParameter("@CardValue", DBNull.Value) : new SqlParameter("@CardValue", CardValue);
            pars[6] = IsSold == null ? new SqlParameter("@IsSold", DBNull.Value) : new SqlParameter("@IsSold", IsSold);
            pars[7] = IsActive == null ? new SqlParameter("@IsActive", DBNull.Value) : new SqlParameter("@IsActive", IsActive);
            pars[8] = PageNumber == null ? new SqlParameter("@PageNumber", 1) : new SqlParameter("@PageNumber", PageNumber);
            pars[9] = PageSize == null ? new SqlParameter("@PageSize", int.MaxValue) : new SqlParameter("@PageSize", PageSize);
            pars[10] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var data = db.GetListSP<CardStore>("sp_CardStore_SelectList", pars);
            TotalRecord = Convert.ToInt32(pars[10].Value); 
            return data;
        }


    }
}
