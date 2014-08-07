using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DBEngine
{
    public class DatabaseEngine
    {
        public bool IsConnected = false;
        private string ConnStr;
        //OleDbConnection conn;
        //MySqlConnection conn;
        SqlConnection conn;

        public DatabaseEngine()
        {

        }
       
        //for access database
        public DatabaseEngine(string DBAddress, string DBPassword)       
        {
            this.ConnStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                            + DBAddress + ";Jet OLEDB:Database Password=" + DBPassword;
        }

        //for mysql
        public DatabaseEngine(string Server,string port,string database,string user,string password)
        {
            this.ConnStr = @"Server="+Server+";"+
                            "Port="+port+";"+
                            "Database="+database+";"+
                            "Uid="+user+";"+
                            "password="+password+";";
        }
       //For SQL server

        public DatabaseEngine(string Server, string myDataBase, string user, string password)
        {
            this.ConnStr = @"Data Source=" + Server + ";" +
                            "Initial Catalog=" + myDataBase + ";" +
                            "User ID=" + user + ";" +
                            "Password=" + password + ";";
        }


        #region MySQL part

        public bool Connect()
        {
            if (this.IsConnected) return true;
            
            try
            {
                //conn = new MySqlConnection();
                conn = new SqlConnection();
                conn.ConnectionString = ConnStr;
                conn.Open();
                this.IsConnected = true;
                return true;
            }
            catch (Exception)
            {
                this.IsConnected = false;
                return false;
            }
        }

        public bool Connect(string Server, string myDataBase, string user, string password)
        {
            this.ConnStr = @"Data Source=" + Server + ";" +
                            "Initial Catalog=" + myDataBase + ";" +
                            "User ID=" + user + ";" +
                            "Password=" + password + ";";

            if (this.IsConnected) return true;

            try
            {
                //conn = new MySqlConnection();
                conn = new SqlConnection();
                conn.ConnectionString = ConnStr;
                conn.Open();
                this.IsConnected = true;
                return true;
            }
            catch (Exception)
            {
                this.IsConnected = false;
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                conn.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                this.IsConnected = false;
            } 
        }

        public DataSet Query(string SQL)
        {
            DataSet ds = new DataSet();

            /*
            try
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, conn);
                adapter.Fill(ds);
            }
            catch (Exception)
            {

            }
             */
            try
            {
                //MySqlDataAdapter adapter = new MySqlDataAdapter();
                //adapter.SelectCommand = new MySqlCommand(SQL, conn);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(SQL, conn);
                adapter.Fill(ds);
            }
            catch (Exception)
            {                
                
            }
            return ds;
        }

        public bool InsertBySQL(string SQL)
        {
            try
            {      
                //MySqlCommand cmd = new MySqlCommand(SQL, conn);
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }        
        }

        public bool DeleteBySQL(string SQL)
        {
            try
            {
                //MySqlCommand cmd = new MySqlCommand(SQL, conn);
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {                
                return false;
            }

        }
        #endregion

        //public string[] iColumns()
        //{

        //    return;
        //}

        public Scripting.Dictionary replaceKey(Hashtable dbHasTable, Scripting.Dictionary iParameters)
        {
            Scripting.Dictionary tmpDic = new Scripting.Dictionary();
            object[] lKeys = (object[])iParameters.Keys();
            object[] lItems = (object[])iParameters.Items();
            for (int i = 0; i < iParameters.Count; i++)
            {
                if (dbHasTable.ContainsKey((string)lKeys[i]))
                {
                    lKeys[i] = dbHasTable[lKeys[i]];
                }
            }

            for (int j = 0; j < iParameters.Count; j++)
			{
			    tmpDic.Add(lKeys[j],lItems[j]);
			}
            return tmpDic;
        }

        public void InsertData(string iTableName, Scripting.Dictionary iParameters)
        {
            DBMap dbmap = new DBMap();
            dbmap.initMap("dbmap.txt");
            Scripting.Dictionary dic2 = new Scripting.Dictionary();
            dic2 = replaceKey(dbmap.dbHasTable, iParameters);

            object[] lKeys = (object[])dic2.Keys();
            List<string> list_Columns = new List<string>();
            string tmp;
            for (int i = 0; i < dic2.Count; i++)
            {
                tmp = ((string)lKeys[i]).Split('@')[1];
                list_Columns.Add(tmp);
                tmp = null;
            }

            Insert(iTableName, list_Columns, dic2);
        }


        private void Insert(string iTableName, List<string> iColumns, Scripting.Dictionary iParameters)
        {
           // SqlConnection sqlConn = new SqlConnection(iConnectionString);

            string sqlCommand = String.Format("INSERT INTO {0} (", iTableName);
            foreach (string column in iColumns)
            {
                sqlCommand += string.Format("{0},", column);
            }
            sqlCommand = sqlCommand.Remove(sqlCommand.Length - 1, 1);
            sqlCommand += ")";

            sqlCommand += " VALUES (";

            //ArrayList lKeys = new ArrayList();
            object[] lKeys = (object[])iParameters.Keys();
            object[] lItems = (object[])iParameters.Items();


            for (int i = 0; i < iParameters.Count; i++)
            {
                sqlCommand += String.Format("{0},", lKeys[i]);
            }
           
            sqlCommand = sqlCommand.Remove(sqlCommand.Length - 1, 1);
            sqlCommand += ");";

            SqlCommand myCommand = new SqlCommand(sqlCommand, conn);

             for (int i = 0; i < iParameters.Count; i++)
            {
                myCommand.Parameters.AddWithValue((string)lKeys[i], lItems[i]);
            }

            //foreach (var parameter in iParameters)
            //{
            //    myCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            //}

            try
            {
                if ((conn.State != ConnectionState.Open))
                {
                    conn.Open();
                }
                myCommand.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        //private void InsertData(string iTableName, string iConnectionString, string[] iColumns, IDictionary<string, dynamic> iParameters)
        //{
        //    SqlConnection sqlConn = new SqlConnection(iConnectionString);

        //    string sqlCommand = String.Format("INSERT INTO {0} (", iTableName);
        //    foreach (string column in iColumns)
        //    {
        //        sqlCommand += string.Format("{0},", column);
        //    }
        //    sqlCommand = sqlCommand.Remove(sqlCommand.Length - 1, 1);
        //    sqlCommand += ")";

        //    sqlCommand += " VALUES (";
        //    foreach (var pair in iParameters)
        //    {
        //        sqlCommand += String.Format("{0},", pair.Key);
        //    }
        //    sqlCommand = sqlCommand.Remove(sqlCommand.Length - 1, 1);
        //    sqlCommand += ");";

        //    SqlCommand myCommand = new SqlCommand(sqlCommand, sqlConn);

        //    foreach (var parameter in iParameters)
        //    {
        //        myCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
        //    }

        //    try
        //    {
        //        if ((sqlConn.State != ConnectionState.Open))
        //        {
        //            sqlConn.Open();
        //        }
        //        myCommand.ExecuteNonQuery();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (sqlConn.State == ConnectionState.Open)
        //        {
        //            sqlConn.Close();
        //        }
        //    }
        //}
    }

}
