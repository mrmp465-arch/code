using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }       // Mã giao dịch của Đại lý  – gồm các ký tự dạng số, chiều dài 6 (ví dụ: 000123)
        public string UserName { get; set; }           // Mã code của Đại lý
        public string Mobile { get; set; }           // Mã ngân hàng
        public string PassWord { get; set; }            // Mã dịch vụ thanh toán
        public string EmailServer { get; set; }
        public string Email { get; set; }                // Mã giao dịch của hệ thống BankNet
        public string EmailPass { get; set; }
        public string BankCode { get; set; }
        public long Balance { get; set; }
        public int Status { get; set; }
        public int ReturnValue { get; set; }


        public List<BankAccount> Get(int status)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<BankAccount>("sp_BankAccount_Select", new SqlParameter("@Status", status));
        }

    }
}
