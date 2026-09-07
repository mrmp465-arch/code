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
    public class ContactsService : IContactsService
    {

        public Contacts Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Contacts>("SP_Contact_Get",
                                                                                                   new SqlParameter("@Id", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Contacts();
            }
        }



        public List<Contacts> GetFilter(int groupId, string keyword, string createUser, int page, int pageSize, ref int total)
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
            if (!string.IsNullOrEmpty(createUser))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [CreatedUser]='" + createUser + "'";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " (Name LIKE N'%" + keyword + "%' OR Number LIKE N'%" + keyword + "%' ) ";
            }

            return GetList(select, where, order, page, pageSize, ref total);
        }
        public List<Contacts> GetList(string select, string where, string order, int page, int pageSize, ref int total)
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
                var data = new DBHelper(Config.MainConnectionString).GetListSP<Contacts>("sp_Contact_SelectPagedDynamic", pars);
                total = Convert.ToInt32(pars[5].Value);
                return data;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                total = 0;
                return new List<Contacts>();
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
        public int InsertUpdate(Contacts functions)
        {
            try
            {
                var pars = new SqlParameter[15];
                pars[0] = new SqlParameter("@_Id", functions.Id);
                pars[1] = new SqlParameter("@_Name", functions.Name+"");
                pars[2] = new SqlParameter("@_Number", functions.Number);
                pars[4] = new SqlParameter("@_Email", functions.Email + "");
                pars[5] = new SqlParameter("@_CreatedUser", functions.CreatedUser);
                pars[6] = new SqlParameter("@_Group", functions.Group);
                pars[7] = new SqlParameter("@_Gender", functions.Gender);
                pars[8] = new SqlParameter("@_Option1", functions.Option1+"");
                pars[9] = new SqlParameter("@_Option2", functions.Option2 + "");
                pars[10] = new SqlParameter("@_Option3", functions.Option3 + "");
                pars[11] = new SqlParameter("@_Option4", functions.Option4 + "");
                pars[12] = new SqlParameter("@_Option5", functions.Option5 + "");
                pars[13] = new SqlParameter("@_Birthday", functions.Birthday);
                pars[14] = new SqlParameter("@_Telco", functions.Telco);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Contact_Update", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }


       

        public int Delete(int functionId)
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

                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_Contact_DeleteDynamic", pars);
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
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Contact_UpdateDynamic", pars);
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

