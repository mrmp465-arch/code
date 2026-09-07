using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SMS.Data.Service;
using SMS.Data.DTO;
using SMS.Utility;
using System.Linq;

namespace SMS.Data.Service
{
    public class SMSOutboxService : ISMSOutboxService
    {


        public List<SMSOutbox> GetFilter(int port, string sender)
        {
            var select = "Top 100 *";
            var order = "Id DESC";
            var where = "";
            if (port > 0)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";

                where += " [Port]=" + port.ToString();
            }
            if (!string.IsNullOrEmpty(sender))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [Sender]='" + sender + "'";
            }
            
            return GetList(select, where, order);
        }
        public List<SMSOutbox> GetList(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                return new DBHelper(Config.MainConnectionString).GetListSP<SMSOutbox>("SP_SMSOutbox_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSOutbox>();
            }
        }


        /// <summary>
        /// Insert Fucntion
        /// </summary>
        /// <param name="functions"></param>
        /// <returns> >0 : thanh cong
        ///			-1: da ton tai
        ///			-99: loi he thong
        /// </returns>
        public int InsertUpdate(SMSOutbox functions)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_Id", functions.Id);
                pars[1] = new SqlParameter("@_Number", functions.Number);
                pars[2] = new SqlParameter("@_Sender", functions.Sender);
                pars[4] = new SqlParameter("@_Port", functions.Port);
                pars[5] = new SqlParameter("@_Content", functions.Content);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_SMSOutbox_Update", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }


        

    }
}

