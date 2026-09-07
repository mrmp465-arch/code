using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace SMS.Data
{
    public class DBHelper
    {
        private string _cnnString;
        private SqlConnection _ConnectionToDB;

        public DBHelper()
        {
            this._cnnString = "";
        }

        public DBHelper(string connStr)
        {
            this._cnnString = "";
            this._cnnString = connStr;
        }

        public void Close()
        {
            this.CloseConnection(this._ConnectionToDB);
        }

        public void CloseConnection(SqlConnection mySqlConnection)
        {
            try
            {
                if ((mySqlConnection != null) && (mySqlConnection.State == ConnectionState.Open))
                {
                    mySqlConnection.Close();
                }
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public int ExecuteNonQuery(SqlCommand sqlCommand)
        {
            SqlConnection mySqlConnection = null;
            int num;
            try
            {
                if (this._ConnectionToDB == null)
                {
                    mySqlConnection = this.OpenConnection();
                    sqlCommand.Connection = mySqlConnection;
                }
                else
                {
                    sqlCommand.Connection = this._ConnectionToDB;
                }
                num = sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (mySqlConnection != null)
                {
                    this.CloseConnection(mySqlConnection);
                }
            }
            return num;
        }

        public int ExecuteNonQuery(string strSQL)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            return this.ExecuteNonQuery(sqlCommand);
        }

        public int ExecuteNonQuery(SqlCommand sqlCommand, params SqlParameter[] Parameters)
        {
            sqlCommand.Parameters.AddRange(Parameters);
            return this.ExecuteNonQuery(sqlCommand);
        }

        public int ExecuteNonQuery(string strSQL, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            return this.ExecuteNonQuery(sqlCommand, Parameters);
        }

        public int ExecuteNonQuerySP(string SPName)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.ExecuteNonQuery(sqlCommand);
        }

        public int ExecuteNonQuerySP(string SPName, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.ExecuteNonQuery(sqlCommand, Parameters);
        }

        public object ExecuteScalar(SqlCommand sqlCommand)
        {
            SqlConnection mySqlConnection = null;
            object obj2;
            try
            {
                if (this._ConnectionToDB == null)
                {
                    mySqlConnection = this.OpenConnection();
                    sqlCommand.Connection = mySqlConnection;
                }
                else
                {
                    sqlCommand.Connection = this._ConnectionToDB;
                }
                obj2 = sqlCommand.ExecuteScalar();
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (mySqlConnection != null)
                {
                    this.CloseConnection(mySqlConnection);
                }
            }
            return obj2;
        }

        public object ExecuteScalar(string strSQL)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            return this.ExecuteScalar(sqlCommand);
        }

        public object ExecuteScalar(SqlCommand sqlCommand, params SqlParameter[] Parameters)
        {
            sqlCommand.Parameters.AddRange(Parameters);
            return this.ExecuteScalar(sqlCommand);
        }

        public object ExecuteScalar(string strSQL, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            return this.ExecuteScalar(sqlCommand, Parameters);
        }

        public object ExecuteScalarSP(string SPName)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.ExecuteScalar(sqlCommand);
        }

        public object ExecuteScalarSP(string SPName, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.ExecuteScalar(sqlCommand, Parameters);
        }

        public string FixCNN(string connStr, bool Pooling)
        {
            string[] strArray = connStr.Split(new char[] { ';' });
            string str = "";
            for (int i = 0; i < strArray.Length; i++)
            {
                if ((((!strArray[i].ToLower().StartsWith("pooling=") && !strArray[i].ToLower().StartsWith("min pool size=")) && !strArray[i].ToLower().StartsWith("max pool size=")) && !strArray[i].ToLower().StartsWith("connect timeout=")) && !strArray[i].Equals(""))
                {
                    str = str + strArray[i] + ";";
                }
            }
            if (Pooling)
            {
                return (str + "Pooling=true;Min Pool Size=1;Max Pool Size=15;Connect Timeout=2;");
            }
            return (str + "Pooling=false;Connect Timeout=45;");
        }

        public DataTable GetDataTable(SqlCommand sqlCommand)
        {
            SqlConnection mySqlConnection = null;
            DataTable table2;
            try
            {
                if (this._ConnectionToDB == null)
                {
                    mySqlConnection = this.OpenConnection();
                    sqlCommand.Connection = mySqlConnection;
                }
                else
                {
                    sqlCommand.Connection = this._ConnectionToDB;
                }
                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                adapter.Dispose();
                table2 = dataTable;
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (mySqlConnection != null)
                {
                    this.CloseConnection(mySqlConnection);
                }
            }
            return table2;
        }

        public DataTable GetDataTable(string strSQL)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            return this.GetDataTable(sqlCommand);
        }

        public DataTable GetDataTable(SqlCommand sqlCommand, params SqlParameter[] Parameters)
        {
            sqlCommand.Parameters.AddRange(Parameters);
            return this.GetDataTable(sqlCommand);
        }

        public DataTable GetDataTable(string strSQL, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            return this.GetDataTable(sqlCommand, Parameters);
        }

        public DataTable GetDataTableSP(string SPName)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.GetDataTable(sqlCommand);
        }

        public DataTable GetDataTableSP(string SPName, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.GetDataTable(sqlCommand, Parameters);
        }

        public DataTable GetDataTableSP(string SPName, int timeOut, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = timeOut
            };
            return this.GetDataTable(sqlCommand, Parameters);
        }

        public T GetInstance<T>(SqlCommand sqlCommand)
        {
            Func<PropertyInfo, string> selector = null;
            T local2;
            try
            {
                T local = default(T);
                sqlCommand.Connection = this._ConnectionToDB ?? this.OpenConnection();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    Func<PropertyInfo, bool> predicate = null;
                    string pName;
                    int fieldCount = reader.FieldCount;
                    Type type = typeof(T);
                    PropertyInfo[] properties = type.GetProperties();
                    List<T> list = new List<T>();
                    object obj2 = Activator.CreateInstance(type);
                    for (int i = 0; i < fieldCount; i++)
                    {
                        pName = reader.GetName(i);
                        if (predicate == null)
                        {
                            predicate = a => a.Name == pName;
                        }
                        if (selector == null)
                        {
                            selector = a => a.Name;
                        }
                        if (properties.Where<PropertyInfo>(predicate).Select<PropertyInfo, string>(selector).Count<string>() > 0)
                        {
                            if (reader[i] != DBNull.Value)
                            {
                                type.GetProperty(pName).SetValue(obj2, reader[i], null);
                            }
                            else
                            {
                                type.GetProperty(pName).SetValue(obj2, null, null);
                            }
                        }
                    }
                    reader.Close();
                    return (T)obj2;
                }
                local2 = local;
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                this.CloseConnection(sqlCommand.Connection);
            }
            return local2;
        }

        public T GetInstance<T>(string strSQL)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            return this.GetInstance<T>(sqlCommand);
        }

        public T GetInstance<T>(SqlCommand sqlCommand, params SqlParameter[] Parameters)
        {
            sqlCommand.Parameters.AddRange(Parameters);
            return this.GetInstance<T>(sqlCommand);
        }

        public T GetInstance<T>(string strSQL, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            sqlCommand.Parameters.AddRange(Parameters);
            return this.GetInstance<T>(sqlCommand);
        }

        public T GetInstanceSP<T>(string SPName)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.GetInstance<T>(sqlCommand);
        }

        public T GetInstanceSP<T>(string SPName, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.GetInstance<T>(sqlCommand, Parameters);
        }

        public List<T> GetList<T>(SqlCommand sqlCommand)
        {
            Func<PropertyInfo, string> selector = null;
            List<T> list2;
            try
            {
                Func<PropertyInfo, bool> predicate = null;
                string pName;
                sqlCommand.Connection = this._ConnectionToDB ?? this.OpenConnection();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if ((reader == null) || (reader.FieldCount == 0))
                {
                    return null;
                }
                int fieldCount = reader.FieldCount;
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();
                List<T> list = new List<T>();
                while (reader.Read())
                {
                    object obj2 = Activator.CreateInstance(type);
                    for (int i = 0; i < fieldCount; i++)
                    {
                        pName = reader.GetName(i);
                        if (predicate == null)
                        {
                            predicate = a => a.Name == pName;
                        }
                        if (selector == null)
                        {
                            selector = a => a.Name;
                        }
                        if (properties.Where<PropertyInfo>(predicate).Select<PropertyInfo, string>(selector).Count<string>() > 0)
                        {
                            if (reader[i] != DBNull.Value)
                            {
                                type.GetProperty(pName).SetValue(obj2, reader[i], null);
                            }
                            else
                            {
                                type.GetProperty(pName).SetValue(obj2, null, null);
                            }
                        }
                    }
                    list.Add((T)obj2);
                }
                reader.Close();
                list2 = list;
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                this.CloseConnection(sqlCommand.Connection);
            }
            return list2;
        }

        public List<T> GetList<T>(string strSQL)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            return this.GetList<T>(sqlCommand);
        }

        public List<T> GetList<T>(SqlCommand sqlCommand, params SqlParameter[] Parameters)
        {
            sqlCommand.Parameters.AddRange(Parameters);
            return this.GetList<T>(sqlCommand);
        }

        public List<T> GetList<T>(string strSQL, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(strSQL);
            sqlCommand.Parameters.AddRange(Parameters);
            return this.GetList<T>(sqlCommand);
        }

        public List<T> GetListSP<T>(string SPName)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.GetList<T>(sqlCommand);
        }

        public List<T> GetListSP<T>(string SPName, params SqlParameter[] Parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(SPName)
            {
                CommandType = CommandType.StoredProcedure
            };
            return this.GetList<T>(sqlCommand, Parameters);
        }

        public void Open()
        {
            if (this._cnnString == "")
            {
                throw new Exception("Connection String can not null");
            }
            this._ConnectionToDB = this.OpenConnection();
        }

        public SqlConnection OpenConnection()
        {
            SqlConnection connection;
            if (this._cnnString == "")
            {
                throw new Exception("Connection String can not null");
            }
            try
            {
                connection = new SqlConnection(this.FixCNN(this._cnnString, true));
                connection.Open();
                return connection;
            }
            catch (Exception)
            {
                connection = new SqlConnection(this.FixCNN(this._cnnString, false));
                connection.Open();
                return connection;
            }
        }

        public SqlConnection OpenConnection(string connectionString)
        {
            SqlConnection connection;
            try
            {
                this._cnnString = connectionString;
                connection = this.OpenConnection();
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message);
            }
            return connection;
        }

        public string cnnString
        {
            get
            {
                return this._cnnString;
            }
            set
            {
                this._cnnString = value;
            }
        }

        public SqlConnection ConnectionToDB
        {
            get
            {
                return this._ConnectionToDB;
            }
            set
            {
                this._ConnectionToDB = value;
            }
        }
    }
}