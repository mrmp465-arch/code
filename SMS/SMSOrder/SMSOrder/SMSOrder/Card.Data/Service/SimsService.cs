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
    public interface ISimsService
    {
        int UpdateActive(long Id);
        Sims Get(int Id);
        List<Sims> GetFilter(int groupId, string keyword, string telco, int status, int page, int pageSize, ref int total);
        int DeleteDynamic(string where);
        int InsertUpdate(Sims group);
        int Delete(long Id);
        int UpdateDynamic(string where, string updatest);
    }
    public class SimsService : ISimsService
    {

        public Sims Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Sims>("SP_Sim_Get",
                                                                                                   new SqlParameter("@Id", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Sims();
            }
        }



        public List<Sims> GetFilter(int groupId, string keyword, string telco,int status, int page, int pageSize, ref int total)
        {
            var select = "*";
              var order = "Id DESC";
            var where = "";
            if (groupId > 0)
            {     
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";

                where += " [Group]=" + groupId.ToString();
            }
            if (!string.IsNullOrEmpty(telco))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [Telco]='" + telco + "'";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " ( Number LIKE N'%" + keyword + "%' ) ";
            }
            if (status >= 0)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [Status]="+status;
            }
            return GetList(select, where, order, page, pageSize, ref total);
        }
        public int UpdateActive(long Id)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Id", Id);
                pars[1] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Sim_UpdateActive", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public List<Sims> GetList(string select, string where, string order, int page, int pageSize, ref int total)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                pars[3] = new SqlParameter("@PageIndex", page);
                pars[4] = new SqlParameter("@PageSize", pageSize);
                pars[5] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var data = new DBHelper(Config.MainConnectionString).GetListSP<Sims>("sp_Sim_SelectPagedDynamic", pars);
                total = Convert.ToInt32(pars[5].Value);
                return data;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                total = 0;
                return new List<Sims>();
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
        public int InsertUpdate(Sims functions)
        {
            try
            {
                var pars = new SqlParameter[11];
                pars[0] = new SqlParameter("@_Id", functions.Id);
                pars[1] = new SqlParameter("@_Balance", functions.Balance);
                pars[2] = new SqlParameter("@_Number", functions.Number);
                pars[4] = new SqlParameter("@_RealBalance", functions.RealBalance);
                pars[5] = new SqlParameter("@_Note", functions.Note);
                pars[6] = new SqlParameter("@_Group", functions.Group);
                pars[7] = new SqlParameter("@_Status", functions.Status);
                pars[8] = new SqlParameter("@_Password", functions.Password+"");
                pars[9] = new SqlParameter("@_ExpriteDate", functions.ExpriteDate);
                pars[10] = new SqlParameter("@_Telco", functions.Telco);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Sim_Update", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }


       

        public int Delete(long functionId)
        {
            try
            {
                var where = "ID=" + functionId;
                return DeleteDynamic(where);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int DeleteDynamic(string where)
        {
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@WhereCondition", where);

                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_Sim_DeleteDynamic", pars);
                return 1;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UpdateDynamic(string where, string updatest)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@UpdateCondition", updatest);
                pars[1] = new SqlParameter("@WhereCondition", where);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Sim_UpdateDynamic", pars);
                return 1;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }

    }
}

