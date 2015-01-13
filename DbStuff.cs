using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Oracle.DataAccess.Client;

namespace ClearProcess
{
    public static class DbStuff
    {
        public static void MakeConnection()
        {
            OracleConnection connOra = new OracleConnection();

            try
            {
                // Connect to the database
                connOra.ConnectionString = MakeConn();
                connOra.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source);
            }
            finally
            {
                // No matterv what, close and dispose
                connOra.Close();
                connOra.Dispose();
            }
        }

        public static bool DeleteSfrracl(string pidm, string term)
        {
            string sql = "DELETE FROM SATURN.SFRRACL WHERE SFRRACL_PIDM = '{0}' AND SFRRACL_TERM_CODE = '{1}'";
            return WriteData(String.Format(sql, pidm, term));
        }

        public static bool DeleteSftregs(string pidm, string term)
        {
            string sql = "DELETE FROM SATURN.SFTREGS WHERE SFTREGS_PIDM = '{0}' AND SFTREGS_TERM_CODE = '{1}'";
            return WriteData(String.Format(sql, pidm, term));
        }

        public static bool DeleteSftarea(string pidm, string term)
        {
            string sql = "DELETE FROM SATURN.SFTAREA WHERE SFTAREA_PIDM = '{0}' AND SFTAREA_TERM_CODE = '{1}'";
            return WriteData(String.Format(sql, pidm, term));
        }

        public static string CheckForError(string pidm, string term)
        {
            string sql = "SELECT * FROM SFTREGS WHERE SFTAREA_PIDM = '{0}' AND SFTAREA_TERM_CODE = '{1}'";
            string error_code = "";
            DataSet res = FetchDataSet(String.Format(sql, pidm, term));
            if (res.Tables.Count == 0 || res.Tables[0].Rows.Count == 0) { return ""; }
            foreach(DataRow dr in res.Tables[0].Rows)
            {
                if (dr["SFTREGS_ERROR_CODE"].ToString().Length > 0)
                {
                    error_code = error_code + dr["SFTREGS_ERROR_CODE"].ToString() + " ";
                }
            }
            return error_code.Trim();
        }

        public static string LoadThisTerm()
        {
            string sql = "SELECT ROVTERM_CODE FROM rovterm where rovterm_end_date > sysdate and rownum=1 order by rovterm_end_date asc";
            DataSet res = FetchDataSet(sql);
            if (res.Tables.Count == 0 || res.Tables[0].Rows.Count == 0) { return ""; }
            return res.Tables[0].Rows[0]["ROVTERM_CODE"].ToString();
        }

        public static string LoadStudentPidm(string id)
        {
            string sql = String.Format("select SPRIDEN.SPRIDEN_PIDM from spriden where spriden_change_ind is null and spriden_id='{0}'", id);
            DataSet res = FetchDataSet(sql);
            if (res.Tables.Count == 0 || res.Tables[0].Rows.Count == 0) { return ""; }
            return res.Tables[0].Rows[0]["SPRIDEN_PIDM"].ToString();
        }

        public static DataSet FetchDataSet(string queryA)
        {
            DataSet ds = new DataSet();
            OracleCommand cmd;
            OracleConnection connOra = new OracleConnection();
            
            try
            {
                // Reopen the connection
                connOra.ConnectionString = MakeConn();
                connOra.Open();
                // Create the command
                cmd = new OracleCommand(queryA);
                cmd.Connection = connOra;
                cmd.CommandType = CommandType.Text;
                // Execute
                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source);
            }
            finally
            {
                connOra.Close();
                connOra.Dispose();
            }

            // Check for data
            if (ds.Tables.Count == 0) { return new DataSet(); }
            else { if (ds.Tables[0].Rows.Count == 0) { return new DataSet(); } }
            // Return
            return ds;
        }

        public static bool WriteData(string queryB)
        {
            bool yesno = false;
            OracleCommand cmd;
            OracleConnection connOra = new OracleConnection();

            try
            {
                // Reopen the connection
                connOra.ConnectionString = MakeConn();
                connOra.Open();
                // Create the command
                cmd = new OracleCommand(queryB);
                cmd.Connection = connOra;
                cmd.CommandType = CommandType.Text;
                // Execute
                cmd.ExecuteNonQuery();
                // This far means success
                yesno = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source);
            }
            finally
            {
                connOra.Close();
                connOra.Dispose();
            }

            return yesno;
        }

        public static string MakeConn()
        {
            // Setup connection string
            if (Program.c.Id.ToString().Length == 0) { throw new Exception("No id"); }
            if (Program.c.Pw.ToString().Length == 0) { throw new Exception("No password"); }
            if (Program.c.Server.ToString().Length == 0) { throw new Exception("No server"); }
            if (Program.c.Database.ToString().Length == 0) { throw new Exception("No database"); }
            return @"User ID=" + Program.c.Id + @";Password=" + Program.c.Pw + @";Data Source=" + Program.c.Server + @":1521/" + Program.c.Database;
        }

        public static void ErrMsg(string msg)
        {
            try
            {
                if (msg.Length == 0) { throw new Exception("No message passed for error"); }
                MessageBox.Show(msg, "ERROR", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                ErrMsg(ex);
            }
        }

        public static void ErrMsg(Exception ex)
        {
            ErrMsg(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + ex.Source);
        }
    }
}
