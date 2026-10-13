using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Libs.Db;

namespace Libs.SMS
{
    public class MessageIn
    {
        public long Id { get; set; }
        public string Provider { get; set; }
        public int Type { get; set; }
        public long SenderId { get; set; }
        public string SenderNumber { get; set; }
        public long ReceiverId { get; set; }
        public string ReceiverNumber { get; set; }
        public string Subject { get; set; }
        public int PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerCommand { get; set; } //Same command code
        public string Content { get; set; }
        public DateTime SentTime { get; set; }
        public DateTime ReceivedTime { get; set; }
        public string RefTranId { get; set; }
        public long Amount { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        //add More
        public string error_code { get; set; }
        public string error_message { get; set; }
        public string telco { get; set; }



        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            const string spName = "sp_MessageIn_Insert";
            var parameters = new[]
            {
                new SqlParameter("@Id", Id),
                new SqlParameter("@SenderId", SenderId),
                new SqlParameter("@Provider", Provider),
                new SqlParameter("@SenderNumber", SenderNumber),
                new SqlParameter("@ReceiverId", ReceiverId),
                new SqlParameter("@ReceiverNumber", ReceiverNumber),
                new SqlParameter("@Subject", Subject),
                new SqlParameter("@PartnerId", PartnerId),
                new SqlParameter("@PartnerCode", PartnerCode),
                new SqlParameter("@PartnerCommand", PartnerCommand),
                new SqlParameter("@Content", Content),
                new SqlParameter("@refTranId", RefTranId),
                new SqlParameter("@Amount", Amount),
                new SqlParameter("@ReceivedTime", ReceivedTime),
            };
            SqlParameter p = parameters[0];
            p.Direction = ParameterDirection.Output;
            db.ExecuteScalarSP(spName, parameters);
            var id = (long)p.Value;
            if (id > 0)
            {
                Id = id;
            }
            else
            {
                Id = 0;
            }

        }

       public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[14];
            var objParamArray = new[]
            {
                new SqlParameter("@Id", Id),
                new SqlParameter("@Provider", Provider),
                new SqlParameter("@SenderNumber", SenderNumber),
                new SqlParameter("@ReceiverNumber", ReceiverNumber),
                new SqlParameter("@Subject", Subject),
                new SqlParameter("@PartnerId ", PartnerId ),
                new SqlParameter("@PartnerCode ", PartnerCode ),
                new SqlParameter("@PartnerCommand ", PartnerCommand ),
                new SqlParameter("@Content ", Content ),
                new SqlParameter("@RefTranId ", RefTranId ),
                new SqlParameter("@Amount ", Amount ),
                new SqlParameter("@Status ", Status ),
                new SqlParameter("@Description ", Description ),
                new SqlParameter("@ReceivedTime ", ReceivedTime ),
            };
            db.ExecuteNonQuerySP("sp_MessageIn_Update", objParamArray);
           

           
        }

    }


}
