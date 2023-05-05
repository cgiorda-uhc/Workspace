
using System;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using Teradata.Client.Provider;
using Snowflake.Data.Client;

public class DBConnection32
{



    public static DataTable getMSSQLDataTableSP(string strConnectionString, string strSQL, Hashtable htParameters)
    {
        using (SqlConnection conn = new SqlConnection(strConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter sParam = default(SqlParameter);
                if (htParameters != null)
                {
                    foreach (DictionaryEntry element in htParameters)
                    {
                        sParam = cmd.CreateParameter();
                        sParam.Value = element.Value;
                        sParam.ParameterName = (string)element.Key;
                        cmd.Parameters.Add(sParam);
                    }
                }

                cmd.CommandTimeout = 99999;
                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rdr);
                    conn.Close();
                    return dt;
                }
            }
        }
    }


    public static void getMSSQLExecuteSP(string strConnectionString, string strSQL, Hashtable htParameters)
    {
        using (SqlConnection conn = new SqlConnection(strConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter sParam = default(SqlParameter);
                foreach (DictionaryEntry element in htParameters)
                {
                    sParam = cmd.CreateParameter();
                    sParam.Value = element.Value;
                    sParam.ParameterName = (string)element.Key;
                    cmd.Parameters.Add(sParam);
                }
                cmd.CommandTimeout = 99999;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }




    public static object getMSSQLExecuteScalarSP(string strConnectionString, string strSQL, Hashtable htParameters)
    {
        object objResult = null;
        using (SqlConnection conn = new SqlConnection(strConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter sParam = default(SqlParameter);
                foreach (DictionaryEntry element in htParameters)
                {
                    sParam = cmd.CreateParameter();
                    sParam.Value = element.Value;
                    sParam.ParameterName = (string)element.Key;
                    cmd.Parameters.Add(sParam);
                }
                cmd.CommandTimeout = 99999;
                conn.Open();
                objResult = cmd.ExecuteScalar();
            }
        }
        return objResult;
    }


    public static object getMSSQLExecuteScalar(string strConnectionString, string strSQL)
    {
        object objResult = null;
        using (SqlConnection conn = new SqlConnection(strConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 99999;
                conn.Open();
                objResult = cmd.ExecuteScalar();
            }
        }
        return objResult;
    }


    public static DataTable getMSSQLDataTable(string strConnectionString, string strSQL)
    {
        using (SqlConnection conn = new SqlConnection(strConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 99999;

                conn.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rdr);
                    conn.Close();
                    return dt;
                }
            }
        }
    }
    public static SqlDataReader GetMSSQLRecordSet(string strConnectionString, string strSQL)
    {
        SqlConnection connection = new SqlConnection(strConnectionString);
        connection.Open();
        SqlCommand command = new SqlCommand(strSQL, connection);
        SqlDataReader sqlDr = null;

        try
        {
            command.CommandTimeout = 9999999;
            sqlDr = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return sqlDr;
    }


    public static DataTable GetSnowflakeDataTable(string strConnectionString, string strSQL)
    {
        using (SnowflakeDbConnection conn = new SnowflakeDbConnection())
        {
            conn.ConnectionString = strConnectionString;
            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 99999;

                conn.Open();
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rdr);
                    conn.Close();
                    return dt;
                }
            }
        }
    }



    public static IDataReader GetSnowflakeRecordSet(string strConnectionString, string strSQL)
    {

        SnowflakeDbConnection connection = new SnowflakeDbConnection();
        connection.ConnectionString = strConnectionString;
        connection.Open();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = strSQL;
        IDataReader sqlDr = null;

        try
        {
            command.CommandTimeout = 9999999;
            sqlDr = command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return sqlDr;
    }




    //public static object ExecuteMSSQL(string strConnectionString, string strSQL)
    //{
    //    object objResult = null; 
    //    using (SqlConnection conn = new SqlConnection(strConnectionString))
    //    { 
    //        using (SqlCommand cmd = new SqlCommand(strSQL, conn)) 
    //        { 
    //            cmd.CommandType = System.Data.CommandType.Text; 
    //            cmd.CommandTimeout = 99999; 
    //            conn.Open(); 
    //            objResult = cmd.ExecuteScalar(); 
    //        } 
    //    } 
    //    return objResult;
    //}



    public static int ExecuteMSSQL(string strConnectionString, string strSQL)
    {
        int intResult = 0;
        using (SqlConnection conn = new SqlConnection(strConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 99999;
                conn.Open();
                intResult = cmd.ExecuteNonQuery();
            }
        }
        return intResult;
    }


    public static int ExecuteMSSQLWithResults(string strConnectionString, string strSQL)
    {
        int intResult = 0;
        using (SqlConnection conn = new SqlConnection(strConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 99999;
                conn.Open();
                intResult = (int)cmd.ExecuteScalar();
            }
        }
        return intResult;
    }






    public static TdDataReader GetTeraDataRecordSet(string strConnectionString, string strSQL)
    {

        TdConnection connection = new TdConnection(strConnectionString);
        connection.Open();
        TdCommand command = new TdCommand(strSQL, connection);
        TdDataReader tdDr = null;

        try
        {
            command.CommandTimeout = 9999999;
            tdDr = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return tdDr;
    }



    public static DataTable getTeraDataDataTable(string strConnectionString, string strSQL, string strVTTCheck = "{$vti}")
    {

        if (strSQL.Contains(strVTTCheck))
        {
            //CAUSE TERADATA SUCKS
            return getTeraDataVTTDataTable(strConnectionString, strSQL);
        }


        using (TdConnection conn = new TdConnection(strConnectionString))
        {
            using (TdCommand cmd = new TdCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 99999;
                conn.Open();
                using (TdDataReader rdr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rdr);
                    conn.Close();
                    return dt;
                }
            }
        }
    }


    public static int ExecuteTeraData(string strConnectionString, string strSQL)
    {
        int intResult = 0;
        using (TdConnection conn = new TdConnection(strConnectionString))
        {
            using (TdCommand cmd = new TdCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 99999;
                conn.Open();
                intResult = cmd.ExecuteNonQuery();
            }
        }
        return intResult;
    }


    public static DataTable getTeraDataDataTableERROR_TEST_NEW(string strConnectionString)
    {
        using (TdConnection conn = new TdConnection(strConnectionString))
        {
            using (TdCommand cmd = new TdCommand("create volatile table TMP_REGX as ( select distinct TD_SYSFNLIB.REGEXP_REPLACE(mem.MBR_FST_Nm, '[^a-zA-Z]+', '') AS MBR_FST_NM, TD_SYSFNLIB.REGEXP_REPLACE(mem.MBR_LST_NM, '[^a-zA-Z]+', '') AS MBR_LST_NM, CAST(( mem.BTH_DT (FORMAT 'yyyy-mm-dd')) AS CHAR(10)) AS BTH_DT from UHCDM001.HP_member mem WHERE INDV_SYS_ID > 0 AND ( (MBR_FST_Nm = 'JAXXON' AND MBR_LST_NM = 'CANNON' AND mem.BTH_DT = '2018-12-11') OR (MBR_FST_Nm = 'JAYA' AND MBR_LST_NM = 'NEESE' AND mem.BTH_DT = '2011-10-11') OR (MBR_FST_Nm = 'JAYANA' AND MBR_LST_NM = 'ROLLINS' AND mem.BTH_DT = '2016-08-15') AND MBR_LST_NM = 'HAY' AND mem.BTH_DT = '2018-07-29') OR (MBR_FST_Nm = 'KADINE' AND MBR_LST_NM = 'JOHNSON' AND mem.BTH_DT = '1995-12-01') ) with data PRIMARY INDEX ( MBR_FST_Nm, MBR_LST_NM, BTH_DT) ON COMMIT PRESERVE ROWS; ET; SELECT DISTINCT mem.INDV_SYS_ID, mem.SBSCR_MEDCD_RCIP_NBR, mem.SBSCR_NBR, TR.MBR_FST_Nm,TR.MBR_LST_NM,/*CAST(( mem.BTH_DT (FORMAT 'yyyy-mm-dd')) AS CHAR(10)) AS*/ TR.BTH_DT, /*GKIRAN11 changed to VT Columns */ TD_SYSFNLIB.REGEXP_REPLACE(mem.MBR_FST_Nm, '[^a-zA-Z]+', '') AS MBR_FST_NM_Reg, TD_SYSFNLIB.REGEXP_REPLACE(mem.MBR_LST_NM, '[^a-zA-Z]+', '') AS MBR_LST_NM_Reg FROM UHCDM001.HP_member AS mem Left Join TMP_REGX as TR on mem.MBR_FST_Nm = TR.MBR_FST_Nm and mem.MBR_LST_NM = TR.MBR_LST_NM WHERE INDV_SYS_ID > 0 AND (TR.MBR_FST_Nm = 'JAXXON' /*GKIRAN11 changed to VT Columns */ AND TR.MBR_LST_NM = 'CANNON' AND TR.BTH_DT = '2018-12-11') OR (TR.MBR_FST_Nm = 'JAYA' AND TR.MBR_LST_NM = 'NEESE' AND TR.BTH_DT = '2011-10-11') OR (TR.MBR_FST_Nm = 'JAYANA' AND TR.MBR_LST_NM = 'ROLLINS' AND TR.BTH_DT = '2016-08-15') OR (TR.MBR_FST_Nm = 'JAYCE' AND TR.MBR_LST_NM = 'RUSH' AND TR.BTH_DT = '2018-03-09') OR (TR.MBR_FST_Nm = 'JAYCEAN' AND TR.MBR_LST_NM = 'LANE ORTIZ' AND TR.BTH_DT = '2013-04-06') OR (TR.MBR_FST_Nm = 'JAYDEN' AND TR.MBR_LST_NM = 'BOSWELL' AND TR.BTH_DT = '2010-01-06') OR (TR.MBR_FST_Nm = 'JAYDEN' AND TR.MBR_LST_NM = 'ESTEP' AND TR.BTH_DT = '2005-05-25') OR (TR.MBR_FST_Nm = 'JAYDEN' AND TR.MBR_LST_NM = 'FELICIANO' AND TR.BTH_DT = '2009-03-10');", conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 99999;
                conn.Open();
                using (TdDataReader rdr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rdr);
                    conn.Close();
                    return dt;
                }
            }
        }
    }






    public static DataTable getTeraDataDataTableERROR_TEST2(string strConnectionString)
    {

        DataTable dt = null;
        try
        {
            TdConnection conn = new TdConnection(strConnectionString);

            conn.Open();


            TdCommand cmd = new TdCommand("create volatile table TMP_REGX as ( select * FROM (select 1 as c1, 2 as c2, 3 as c3) as tmp union select * FROM (select 4 as c1, 5 as c2, 6 as c3) as tmp UNION select * FROM (select 7 as c1, 8 as c2, 9 as c3) as tmp ) with data PRIMARY INDEX ( c1, c2, c3) ON COMMIT PRESERVE ROWS; ET;  SELECT TR.* FROM  TMP_REGX as TR ; ET;", conn);


            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            using (TdDataReader rdr = cmd.ExecuteReader()) //THROWS ERROR: [Teradata Database] [3932] Only an ET or null statement is legal after a DDL Statement
            {
                dt = new DataTable();
                dt.Load(rdr);
                conn.Close();
            }

        }
        catch (Exception ex)
        {
        }

        return dt;

    }







    public static DataTable TDImportTest(string strConnectionString)
    {
        TdTransaction trans = null;
        DataTable dt = null;
        TdConnection conn = null;
        TdCommand cmd = null;
        string[] strArrDVT = null;
        try
        {
            conn = new TdConnection(strConnectionString);

            conn.Open();

            trans = conn.BeginTransaction();

            cmd = new TdCommand(strArrDVT[0] + "ET;", conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();

            cmd = new TdCommand(strArrDVT[0], conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();

            cmd = new TdCommand(strArrDVT[0], conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();


            cmd = new TdCommand(strArrDVT[0], conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();

            cmd = new TdCommand(strArrDVT[0], conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            using (TdDataReader rdr = cmd.ExecuteReader())
            {
                dt = new DataTable();
                dt.Load(rdr);
            }


            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();


            cmd.Dispose();
        }
        catch (Exception ex)
        {
        }
        finally
        {
            if (conn != null)
            {
                trans.Close();
                trans.Dispose();
                trans = null;

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Dispose();
                conn = null;
            }
        }

        return dt;

    }



















    //using Teradata.Client.Provider v 16.20.4.0
    private static DataTable getTeraDataVTTDataTable(string strConnectionString, string strSQL)
    {
        TdTransaction trans = null;
        DataTable dt = null;
        TdConnection conn = null;
        TdCommand cmd = null;
        string[] strArrDVT = null;
        try
        {
            conn = new TdConnection(strConnectionString);

            conn.Open();

            trans = conn.BeginTransaction();

            //BECAUSE TERADATA REALLY SUCKS!
            strArrDVT = strSQL.Split(new string[] { "{$vti}" }, StringSplitOptions.None);
            cmd = new TdCommand(strArrDVT[0] + "ET;", conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();

            //BECAUSE TERADATA REALLY SUCKS!
            strArrDVT = strArrDVT[1].Split(new string[] { "{$vtc}" }, StringSplitOptions.None);
            cmd = new TdCommand(strArrDVT[0], conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();

            //BECAUSE TERADATA REALLY SUCKS!
            strArrDVT = strArrDVT[1].Split(new string[] { "{$vts}" }, StringSplitOptions.None);
            cmd = new TdCommand(strArrDVT[0], conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();


            //BECAUSE TERADATA REALLY SUCKS!
            strArrDVT = strArrDVT[1].Split(new string[] { "{$dvt}" }, StringSplitOptions.None);
            cmd = new TdCommand(strArrDVT[0], conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            using (TdDataReader rdr = cmd.ExecuteReader())
            {
                dt = new DataTable();
                dt.Load(rdr);
            }

            //BECAUSE TERADATA REALLY SUCKS!
            cmd = new TdCommand(strArrDVT[1], conn, trans);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 99999;
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (conn != null)
            {
                trans.Close();
                trans.Dispose();
                trans = null;

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Dispose();
                conn = null;
            }
        }

        return dt;

    }








    public static DataTable getMSSQLDataTable2(string strConnectionString, string strSQL)
    {
        DataTable t = null;
        using (SqlConnection c = new SqlConnection(strConnectionString))
        {
            c.Open();

            using (SqlDataAdapter a = new SqlDataAdapter(
               strSQL, c))
            {
                // 3
                // Use DataAdapter to fill DataTable
                t = new DataTable();
                a.Fill(t);


            }
        }

        return t;

    }






    public static DataTable getOLEDBDataTable(string strConnectionString, string strSQL)
    {
        DataTable t = null;
        using (OleDbConnection c = new OleDbConnection(strConnectionString))
        {
            c.Open();

            using (OleDbDataAdapter a = new OleDbDataAdapter(strSQL, c))
            {
                // 3
                // Use DataAdapter to fill DataTable
                t = new DataTable();
                a.Fill(t);

            }
        }

        return t;

    }

    public static DataTable getODBCDataTable(string strConnectionString, string strSQL)
    {
        DataTable t = null;
        using (OdbcConnection c = new OdbcConnection(strConnectionString))
        {
            c.Open();

            using (OdbcDataAdapter a = new OdbcDataAdapter("SHOW TABLES;", c))
            {
                // 3
                // Use DataAdapter to fill DataTable
                t = new DataTable();
                a.Fill(t);

            }
        }

        return t;

    }


    public static OdbcDataReader getODBCDataReader(string strConnectionString, string strSQL)
    {

        OdbcDataReader oleDr = null;
        OdbcConnection conn = null;
        OdbcCommand cmd = null;

        //try
        //{

        conn = new OdbcConnection();
        conn.ConnectionString = strConnectionString;
        conn.Open();

        cmd = new OdbcCommand();
        cmd.Connection = conn;
        cmd.CommandText = strSQL;
        cmd.CommandTimeout = 9999999;
        oleDr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.ToString());
        //    Console.Read();
        //}
        //finally
        //{

        //}

        return oleDr;


    }
    //PARALLEL!!!
    //https://www.adathedev.co.uk/2011/01/sqlbulkcopy-to-sql-server-in-parallel.html?m=1

    public static void SQLServerBulkImportDT(DataTable dtSource, string strDestinationConnectionString, int intBatchSize = 5000, bool blIncludeMapping = true)
    {
        // OPEN THE DESTINATION DATA
        using (SqlConnection destinationConnection =
                    new SqlConnection(strDestinationConnectionString))
        {
            // OPEN THE CONNECTION
            destinationConnection.Open();

            using (SqlBulkCopy bulkCopy =
            new SqlBulkCopy(destinationConnection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null))
            {
                bulkCopy.BatchSize = intBatchSize;
                bulkCopy.NotifyAfter = (intBatchSize * 2);
                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                bulkCopy.DestinationTableName = dtSource.TableName;
                if (blIncludeMapping)
                    MapColumns(dtSource, bulkCopy);
                bulkCopy.WriteToServer(dtSource);
            }
        }

    }



    public static void SQLServerBulkImportDTOLD(DataTable dtSource, string strDestinationConnectionString, int intBatchSize = 500, bool blIncludeMapping = true)
    {
        // OPEN THE DESTINATION DATA
        using (SqlConnection destinationConnection =
                    new SqlConnection(strDestinationConnectionString))
        {
            // OPEN THE CONNECTION
            destinationConnection.Open();

            using (SqlBulkCopy bulkCopy =
            new SqlBulkCopy(destinationConnection.ConnectionString))
            {
                bulkCopy.BatchSize = intBatchSize;
                bulkCopy.NotifyAfter = (intBatchSize/2);
                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                bulkCopy.DestinationTableName = dtSource.TableName;
                if(blIncludeMapping)
                    MapColumns(dtSource, bulkCopy);
                bulkCopy.WriteToServer(dtSource);
            }
        }

    }


    private static void MapColumns(DataTable infoTable,
  SqlBulkCopy bulkCopy)
    {

        foreach (DataColumn dc in infoTable.Columns)
        {
            bulkCopy.ColumnMappings.Add(dc.ColumnName,
              dc.ColumnName);
        }
    }

    public static event SqlRowsCopiedEventHandler handle_SQLRowCopied;
    private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
    {
        if (handle_SQLRowCopied != null)
            handle_SQLRowCopied(sender, e);

    }









    static OleDbConnection connGlobal = null;
    public static DataTable getOleDbDataTableGlobal(string strConnectionString, string strSQL)
    {
        DataTable dt = new DataTable();

        try
        {

            if (connGlobal == null)
                connGlobal = new OleDbConnection();


            if(connGlobal.State == ConnectionState.Closed)
            {
                connGlobal.ConnectionString = strConnectionString;
                connGlobal.Open();
            }
               

            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = connGlobal;
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 9999999;
                using (OleDbDataReader rdr = cmd.ExecuteReader())
                {

                    dt.Load(rdr);
                    //conn.Close();
                }
            }


        }
        catch (Exception ex)
        {
            throw ex;
            //Console.WriteLine(ex.ToString());
            //Console.Read();
        }
        finally
        {

        }

        return dt;

    }





    public static object getOleDbExecuteScalar(string strConnectionString, string strSQL)
    {
        object objResult = null;
        if (connGlobal == null)
            connGlobal = new OleDbConnection();


        if (connGlobal.State == ConnectionState.Closed)
        {
            connGlobal.ConnectionString = strConnectionString;
            connGlobal.Open();
        }


        using (OleDbCommand cmd = new OleDbCommand(strSQL, connGlobal))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 99999;
                objResult = cmd.ExecuteScalar();
            }

        return objResult;
    }






    public static void getOleDbDataTableGlobalClose()
    {

        if (connGlobal != null)
        {
            if (connGlobal.State != ConnectionState.Closed)
            {
                connGlobal.Close();
            }

            connGlobal = null;
        }

    }





        public static DataTable getOleDbDataTable(string strConnectionString, string strSQL)
    {
        DataTable dt = new DataTable();

        try
        {

            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = strConnectionString;
                conn.Open();

                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = strSQL;
                    cmd.CommandTimeout = 9999999;
                    using (OleDbDataReader rdr = cmd.ExecuteReader())
                    {

                        dt.Load(rdr);
                        conn.Close();

                    }
                }
            }

        }
        catch (Exception ex)
        {
            throw ex;
            //Console.WriteLine(ex.ToString());
            //Console.Read();
        }
        finally
        {
           
        }

        return dt;

    }



    public static int ExecuteOLEDB(string strConnectionString, string strSQL)
    {
        int i = -9;
        try
        {


            if (connGlobal == null)
                connGlobal = new OleDbConnection();


            if (connGlobal.State == ConnectionState.Closed)
            {
                connGlobal.ConnectionString = strConnectionString;
                connGlobal.Open();
            }


            using (OleDbCommand cmd = new OleDbCommand())
            {
                cmd.Connection = connGlobal;
                cmd.CommandText = strSQL;
                cmd.CommandTimeout = 9999999;
                i = cmd.ExecuteNonQuery();
            }


            return i;
        }
        catch (Exception ex)
        {
            throw ex;
            //Console.WriteLine(ex.ToString());
            //Console.Read();
        }
        finally
        {

        }
    }


    public static OleDbDataReader getOleDbDataReader(string strConnectionString, string strSQL)
    {

        OleDbDataReader oleDr = null;
        OleDbConnection conn = null;
        OleDbCommand cmd = null;

        //try
        //{

            conn = new OleDbConnection();
            conn.ConnectionString = strConnectionString;
            conn.Open();

            cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = strSQL;
            cmd.CommandTimeout = 9999999;
            oleDr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.ToString());
        //    Console.Read();
        //}
        //finally
        //{

        //}

        return oleDr;


    }


    

}
