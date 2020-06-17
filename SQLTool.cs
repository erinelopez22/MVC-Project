using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace Snapshot_API.Helpers
{
    public class SQLTool
    {
        private string Server { get; set; }
        private string Database { get; set; }
        private string DbUser { get; set; }
        private string DbPass { get; set; }

        private bool disposed = false;
        System.Runtime.InteropServices.SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);
        private string sap = "Data Source=_SERVER;Initial Catalog=_DB;Integrated Security=False;UID=_UID;PWD=_PWD";
        private SqlConnection cn;
        private SqlCommand cmd;
        private SqlDataAdapter da;

        public SQLTool(string server, string database, string user, string pass)
        {
            Server = server;
            Database = database;
            DbUser = user;
            DbPass = pass;
            sap = sap.Replace("_SERVER", Server);
            sap = sap.Replace("_DB", Database);
            sap = sap.Replace("_UID", DbUser);
            sap = sap.Replace("_PWD", DbPass);

            cn = new SqlConnection(sap);
            if (cn.State == ConnectionState.Closed) cn.Open();
        }
        public void BeginTran()
        {
            string str = "begin tran";
            cmd = new SqlCommand(str, cn);
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
        public void Commit()
        {
            string str = "commit tran";
            cmd = new SqlCommand(str, cn);
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
        public void Rollback()
        {
            string str = "if @@trancount > 0 rollback tran";
            if (cn.State == ConnectionState.Open)
            {
                cmd = new SqlCommand(str, cn);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }
        public DateTime GetDate()
        {
            DateTime today = DateTime.Now;
            string str = "SELECT GETDATE()";
            cmd = new SqlCommand(str, cn);
            cmd.CommandTimeout = 0;
            today = Convert.ToDateTime(cmd.ExecuteScalar());
            cmd.Dispose();
            return today;
        }
        public DataTable sqlDTsp(string StoredProcedureName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            cmd = new SqlCommand();
            da = new SqlDataAdapter(cmd);
            try
            {
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = StoredProcedureName;
                cmd.CommandTimeout = 0;
                foreach (SqlParameter item in parameters)
                {
                    cmd.Parameters.Add(item);
                }

                da.Fill(dt);
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return dt;
        }
        //With parameter(s)
        public int sqlExecute(string query, params SqlParameter[] parameters)
        {
            int i = 0;
            try
            {

                cmd = new SqlCommand(query, cn);
                cmd.CommandTimeout = 0;
                foreach (SqlParameter item in parameters)
                {
                    cmd.Parameters.Add(item);
                }

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return i;
        }
        //Without parameter
        public int sqlExecute(string query)
        {
            int i = 0;
            try
            {
                cmd = new SqlCommand(query, cn);
                cmd.CommandTimeout = 0;
                i = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return i;
        }
        //With parameter(s)
        public string sqlScalar(string query, params SqlParameter[] parameters)
        {
            string str = "";
            try
            {
                cmd = new SqlCommand(query, cn);
                cmd.CommandTimeout = 0;
                foreach (SqlParameter item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
                str = Convert.ToString(cmd.ExecuteScalar());
                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                cmd.Dispose();
                throw new ApplicationException(ex.Message);
            }
            return str;
        }
        //Without parameter
        public string sqlScalar(string query)
        {
            string str = "";
            try
            {
                cmd = new SqlCommand(query, cn);
                cmd.CommandTimeout = 0;
                str = Convert.ToString(cmd.ExecuteScalar());
                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                ex.Message.ToString();
                return null;
            }
            return str;
        }
        //With parameter(s)
        public DataTable sqlDT(string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new SqlCommand(query, cn);
                cmd.CommandTimeout = 0;
                da = new SqlDataAdapter(cmd);
                foreach (SqlParameter item in parameters) cmd.Parameters.Add(item);

                da.Fill(dt);
                cmd.Dispose();
                da.Dispose();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return dt;
        }
        //Without parameter
        public DataTable sqlDT(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new SqlCommand(query, cn);
                cmd.CommandTimeout = 0;
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                cmd.Dispose();
                da.Dispose();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return dt;
        }

        public MemoryStream SQLFile(string query, params SqlParameter[] parameters)
        {
            try
            {
                MemoryStream fileStream = new MemoryStream();
                byte[] file;
                cmd = new SqlCommand(query, cn);
                cmd.CommandTimeout = 0;
                foreach (SqlParameter item in parameters) cmd.Parameters.Add(item);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    file = (byte[])dr[0];
                    fileStream.Write(file, 0, file.Length);
                    fileStream.Position = 0;
                    dr.Close();
                    cmd.Dispose();
                    return fileStream;
                }
                else
                {
                    dr.Close();
                    cmd.Dispose();
                    throw new FileNotFoundException();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public string SQLImage(string query, params SqlParameter[] parameters)
        {
            byte[] img;
            string imgSrc = "";
            using (SqlConnection cn = new SqlConnection(sap))
            {
                try
                {
                    if (cn.State == ConnectionState.Closed) cn.Open();
                    cmd = new SqlCommand(query, cn);
                    cmd.CommandTimeout = 0;
                    foreach (SqlParameter item in parameters) cmd.Parameters.Add(item);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        img = (byte[])dr[0];
                        var base64 = Convert.ToBase64String(img);
                        //imgSrc = String.Format("data:image/jpg;base64,{0}", base64);
                        imgSrc = base64;
                    }
                    cmd.Dispose();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            return imgSrc;
        }

        public string vXCol(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                if (cn.State == ConnectionState.Open) cn.Dispose();
                handle.Dispose();
            }

            disposed = true;
        }
    }
}