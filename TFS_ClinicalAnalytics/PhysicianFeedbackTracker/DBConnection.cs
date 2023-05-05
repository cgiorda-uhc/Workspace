using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Linq;
using System.Collections.Generic;

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




}