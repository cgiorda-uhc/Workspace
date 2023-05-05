using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Teradata.Client.Provider;

public class DBConnection
{



    public static string[] getMSSQLToArraySP(string strConnectionString, string strSQL, Hashtable htParameters, string strAppendText = null)
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
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rdr);
                    conn.Close();


                    var result = dt.Rows.Cast<DataRow>()
                    .Select(row => (String.IsNullOrEmpty(strAppendText ) ? "" : strAppendText + ": ") + row[0].ToString())
                    .ToArray();


                    return result;
                }
            }
        }
    }

    public static List<string> getMSSQLToStringListSP(string strConnectionString, string strSQL, Hashtable htParameters, string firstValue = null)
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
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(rdr);
                    conn.Close();


                    List<string> result = dt.Rows.Cast<DataRow>()
                    .Select(row =>  row[0].ToString())
                    .ToList<string>();


                  if(!String.IsNullOrEmpty(firstValue))
                        result.Insert(0, firstValue);


                    return result;
                }
            }
        }
    }


    public static DataTable getMSSQLDataTableSP(string strConnectionString, string strSQL, Hashtable htParameters)
    {
        using (SqlConnection conn = new SqlConnection(strConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter sParam = default(SqlParameter);

                if(htParameters != null)
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


    public static object getMSSQLExecuteScalar(string strConnectionString, string strSQL, CancellationToken cancellationToken = default(CancellationToken))
    {
        object objResult = null;
        using (SqlConnection conn = new SqlConnection(strConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 99999;
                conn.Open();

                if(cancellationToken != CancellationToken.None)
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    catch (System.OperationCanceledException)
                    {
                        //throw;
                        return null;
                    }
                }
                    

                objResult = cmd.ExecuteScalar();
            }
        }
        return objResult;
    }


    public static DataTable getMSSQLDataTable(string strConnectionString, string strSQL, CancellationToken cancellationToken = default(CancellationToken))
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
                    
                    if (cancellationToken != CancellationToken.None)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                        catch (System.OperationCanceledException)
                        {
                            if(conn.State == ConnectionState.Open)
                                conn.Close();

                            return null;
                        }
                    }

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



    public static object ExecuteMSSQL(string strConnectionString, string strSQL)
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



    public static DataTable getTeraDataDataTable(string strConnectionString, string strSQL)
    {
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




    public static object getTeraDataExecuteScalar(string strConnectionString, string strSQL, CancellationToken cancellationToken = default(CancellationToken))
    {
        object objResult = null;
        using (TdConnection conn = new TdConnection(strConnectionString))
        {
            using (TdCommand cmd = new TdCommand(strSQL, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 99999;
                conn.Open();

                if (cancellationToken != CancellationToken.None)
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    catch (System.OperationCanceledException)
                    {
                        //throw;
                        return null;
                    }
                }


                objResult = cmd.ExecuteScalar();
            }
        }
        return objResult;
    }




    public static void SQLServerBulkImport(string strSourcenConnectionString, string strDestinationConnectionString, string strSQL, string strTableName, int intBatchSize = 500)
    {

        // GET THE SOURCE DATA
        using (TdConnection sourceConnection = new TdConnection(strSourcenConnectionString))
        {
            TdCommand myCommand =
                new TdCommand(strSQL, sourceConnection);
            sourceConnection.Open();
            TdDataReader reader = myCommand.ExecuteReader();

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
                    bulkCopy.NotifyAfter = 1;
                    bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                    bulkCopy.DestinationTableName = strTableName;
                    bulkCopy.WriteToServer(reader);
                }
            }
            reader.Close();
        }

    }

    public static event SqlRowsCopiedEventHandler handle_SQLRowCopied;
    private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
    {
        if (handle_SQLRowCopied != null)
            handle_SQLRowCopied(sender, e);

    }





    ////////////////////////////////////////THREADING SECTION//////////////////////////////////////////////////////////////////



    //private async Task TryTask()
    //{
    //    CancellationTokenSource source = new CancellationTokenSource();
    //    source.CancelAfter(TimeSpan.FromSeconds(1));
    //    Task<int> task = Task.Run(() => slowFunc(1, 2, source.Token), source.Token);

    //    // (A canceled task will raise an exception when awaited).
    //    await task;
    //}

    //private int slowFunc(int a, int b, CancellationToken cancellationToken)
    //{
    //    string someString = string.Empty;
    //    for (int i = 0; i < 200000; i++)
    //    {
    //        someString += "a";
    //        if (i % 1000 == 0)
    //            cancellationToken.ThrowIfCancellationRequested();
    //    }

    //    return a + b;
    //}




    //public static object getMSSQLExecuteScalar(string strConnectionString, string strSQL, CancellationToken cancellationToken)
    //{
    //    object objResult = null;
    //    using (SqlConnection conn = new SqlConnection(strConnectionString))
    //    {
    //        using (SqlCommand cmd = new SqlCommand(strSQL, conn))
    //        {
    //            cmd.CommandType = System.Data.CommandType.Text;
    //            cmd.CommandTimeout = 99999;
    //            conn.Open();
    //            cancellationToken.ThrowIfCancellationRequested();
    //            objResult = cmd.ExecuteScalar();
    //        }
    //    }
    //    return objResult;
    //}


    //private CancellationTokenSource cts;
    //private async void TestSqlServerCancelSprocExecution()
    //{
    //    cts = new CancellationTokenSource();
    //    try
    //    {
    //        await Task.Run(() =>
    //        {
    //            using (SqlConnection conn = new SqlConnection("connStr"))
    //            {
    //                conn.InfoMessage += conn_InfoMessage;
    //                conn.FireInfoMessageEventOnUserErrors = true;
    //                conn.Open();

    //                var cmd = conn.CreateCommand();
    //                cts.Token.Register(() => cmd.Cancel());
    //                cmd.CommandType = CommandType.StoredProcedure;
    //                cmd.CommandText = "dbo.[CancelSprocTest]";
    //                cmd.ExecuteNonQuery();
    //            }
    //        });
    //    }
    //    catch (SqlException)
    //    {
    //        // sproc was cancelled
    //    }
    //}

    //private void cancelButton_Click(object sender, EventArgs e)
    //{
    //    cts.Cancel();
    //}





















}