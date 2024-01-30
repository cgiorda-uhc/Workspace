using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using FastMember;
using System.Text.RegularExpressions;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using MongoDB.Driver.Core.Configuration;
using Teradata.Client.Provider;
using static Dapper.SqlMapper;
using System;

namespace DataAccessLibrary.DataAccess;
public class SqlDataAccess : IRelationalDataAccess
{
	private readonly IConfiguration _config;


    public SqlDataAccess()
    {

    }


    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<T>> LoadData<T>(string sql, CancellationToken token, string connectionId = "VCT_DB")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

        var cmd = new CommandDefinition(sql, cancellationToken: token, commandTimeout:12000);
        var result = await connection.QueryAsync<T>(cmd); 
        return result;
    }


    public async Task<IEnumerable<T>> LoadData<T>(string connectionString, string sql  )
    {
        using IDbConnection connection = new SqlConnection(connectionString);

        var cmd = new CommandDefinition(sql, commandTimeout: 12000);
        var result = await connection.QueryAsync<T>(cmd);
        return result;
    }



    public async Task<IEnumerable<T>> LoadData<T>( string sql, string connectionStringId = "VCT_DB", bool has_connectionstring = false)
    {

        using IDbConnection connection =new SqlConnection((has_connectionstring ? connectionStringId :  _config.GetConnectionString(connectionStringId)));

        var cmd = new CommandDefinition(sql, commandTimeout: 12000);
        var result = await connection.QueryAsync<T>(cmd);
        return result;
    }


    public async Task<IDataReader> LoadData(string connectionString, string sql)
    {

        var connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 9999999;
            var dr = await command.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
            connection = null;
            return dr;
        }
        finally
        {
            if (connection != null)
                connection.Dispose();
        }


    }


    public async Task<DataTable> LoadDataTable(string connectionString, string sql)
    {

        using var connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 9999999;
            using (SqlDataReader rdr = await command.ExecuteReaderAsync())
            {
                DataTable dt = new DataTable();
                dt.Load(rdr);
                connection.Close();
                return dt;
            }
        }
        finally
        {
            if (connection != null)
                connection.Dispose();
        }


    }


    public async Task<IEnumerable<T>> LoadData<T, U>(string connectionString, string storedProcedure, U parameters, string connectionId = "VCT_DB")
    {
        using IDbConnection connection = new SqlConnection(connectionString);

        //DAPPER CALL
        return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }



    public async Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "VCT_DB")
	{
		using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

		//DAPPER CALL
		return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
	}



    //public async Task<SqlMapper.GridReader> LoadDataMultiple(string sql, CancellationToken token, string connectionId = "VCT_DB")
    //{
    //    using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

    //    var cmd = new CommandDefinition(sql, cancellationToken: token, commandTimeout: 12000);
    //    var result = await connection.QueryMultipleAsync(cmd);
    //    return result;
    //}


    //public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> LoadDataMultiple<T1, T2, T3, T4, T5, T6, T7>(string sql, CancellationToken token,
    //                            Func<GridReader, IEnumerable<T1>> func1,
    //                            Func<GridReader, IEnumerable<T2>> func2,
    //                            Func<GridReader, IEnumerable<T3>> func3 = null,
    //                            Func<GridReader, IEnumerable<T4>> func4 = null,
    //                            Func<GridReader, IEnumerable<T5>> func5 = null,
    //                            Func<GridReader, IEnumerable<T6>> func6 = null,
    //                            Func<GridReader, IEnumerable<T7>> func7 = null,
    //                            string connectionId = "VCT_DB")
    //{
    //    var objs = getMultiple(sql, connectionId, func1, func2, func3, func4, func5, func6, func7);
    //    return Tuple.Create(objs[0] as IEnumerable<T1>, objs[1] as IEnumerable<T2>, objs[2] as IEnumerable<T3>, objs[3] as IEnumerable<T4>, objs[4] as IEnumerable<T5>, objs[5] as IEnumerable<T6>, objs[6] as IEnumerable<T7>);
    //}



    public List<object> LoadDataMultiple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40  >(string sql, CancellationToken token,
                                Func<GridReader, IEnumerable<T1>> func1,
                                Func<GridReader, IEnumerable<T2>> func2,
                                Func<GridReader, IEnumerable<T3>> func3 = null,
                                Func<GridReader, IEnumerable<T4>> func4 = null,
                                Func<GridReader, IEnumerable<T5>> func5 = null,
                                Func<GridReader, IEnumerable<T6>> func6 = null,
                                Func<GridReader, IEnumerable<T7>> func7 = null,
                                Func<GridReader, IEnumerable<T8>> func8 = null,
                                Func<GridReader, IEnumerable<T9>> func9 = null,
                                Func<GridReader, IEnumerable<T10>> func10 = null,
                                Func<GridReader, IEnumerable<T11>> func11 = null,
                                Func<GridReader, IEnumerable<T12>> func12 = null,
                                Func<GridReader, IEnumerable<T13>> func13 = null,
                                Func<GridReader, IEnumerable<T14>> func14 = null,
                                Func<GridReader, IEnumerable<T15>> func15 = null,
                                Func<GridReader, IEnumerable<T16>> func16 = null,
                                Func<GridReader, IEnumerable<T17>> func17 = null,
                                Func<GridReader, IEnumerable<T18>> func18 = null,
                                Func<GridReader, IEnumerable<T19>> func19 = null,
                                Func<GridReader, IEnumerable<T20>> func20 = null,
                                Func<GridReader, IEnumerable<T21>> func21 = null,
                                Func<GridReader, IEnumerable<T22>> func22 = null,
                                Func<GridReader, IEnumerable<T23>> func23 = null,
                                Func<GridReader, IEnumerable<T24>> func24 = null,
                                Func<GridReader, IEnumerable<T25>> func25 = null,
                                Func<GridReader, IEnumerable<T26>> func26 = null,
                                Func<GridReader, IEnumerable<T27>> func27 = null,
                                Func<GridReader, IEnumerable<T28>> func28 = null,
                                Func<GridReader, IEnumerable<T29>> func29 = null,
                                Func<GridReader, IEnumerable<T30>> func30 = null,
                                Func<GridReader, IEnumerable<T31>> func31 = null,
                                Func<GridReader, IEnumerable<T32>> func32 = null,
                                Func<GridReader, IEnumerable<T33>> func33 = null,
                                Func<GridReader, IEnumerable<T34>> func34 = null,
                                Func<GridReader, IEnumerable<T35>> func35 = null,
                                Func<GridReader, IEnumerable<T36>> func36 = null,
                                Func<GridReader, IEnumerable<T37>> func37 = null,
                                Func<GridReader, IEnumerable<T38>> func38 = null,
                                Func<GridReader, IEnumerable<T39>> func39 = null,
                                Func<GridReader, IEnumerable<T40>> func40 = null,
                                string connectionId = "VCT_DB")
    {
        var objs = getMultiple(sql, connectionId, func1, func2, func3, func4, func5, func6, func7, func8, func9, func10, func11, func12, func13, func14, func15, func16, func17, func18, func19, func20, func21, func22, func23, func24, func25, func26, func27, func28, func29, func30, func31, func32, func33, func34, func35, func36, func37, func38, func39, func40);
        return objs;
    }
    private List<object> getMultiple(string sql, string connectionId, params Func<GridReader, object>[] readerFuncs)
    {
        var returnResults = new List<object>();
        using (IDbConnection db = new SqlConnection(_config.GetConnectionString(connectionId)))
        {
            var gridReader = db.QueryMultiple(sql, commandTimeout : 10000000);

            foreach (var readerFunc in readerFuncs)
            {
                if(gridReader.Command != null)
                {
                    var obj = readerFunc(gridReader);
                    returnResults.Add(obj);
                }
                else
                {
                    returnResults.Add(null);
                }
            }
        }

        return returnResults;
    }

    public async Task<object> ExecuteScalar<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

        var result = await connection.ExecuteScalarAsync<object>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        return result;

    }


    public async Task<object> ExecuteScalar(string connectionString, string sql)
    {
        using IDbConnection connection = new SqlConnection(connectionString);

        var result = await connection.ExecuteScalarAsync(sql, commandType: CommandType.Text);
        return result;

    }


    public async Task<object> Execute(string connectionString, string sql)
    {
        using IDbConnection connection = new SqlConnection(connectionString);

        var result = await connection.ExecuteAsync(sql, commandType: CommandType.Text, commandTimeout: 50000);
        return result;

    }

    public async Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB")
	{
		using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

		//DAPPER CALL
		await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
	}

    public async Task BulkSave<T>(string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 12000, int batchSize = 5000, string connectionId = "VCT_DB", bool truncate = false)
    {

        // data is an IEnumerable<T>           
        using (var bcp = new SqlBulkCopy(_config.GetConnectionString(connectionId), SqlBulkCopyOptions.TableLock))
        using (var reader = ObjectReader.Create(data, columns))
        {
            foreach (var c in columns)
            {
                bcp.ColumnMappings.Add(new SqlBulkCopyColumnMapping { DestinationColumn = c, SourceColumn = c });
            }

            bcp.BulkCopyTimeout = bulkTimeout;
            bcp.BatchSize = batchSize;
            bcp.DestinationTableName = table;


            if (truncate)
            {
                using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

                var result = await connection.ExecuteAsync("TRUNCATE TABLE " + table, commandType: CommandType.Text);

            }


            await bcp.WriteToServerAsync(reader);
            //try
            //{
                
            //}
            //catch(Exception e)
            //{
            //    var s = e;
            //}

        }
    }



    public async Task BulkSave<T>(string connectionString, string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 12000, int batchSize = 5000, bool truncate = false)
    {

        // data is an IEnumerable<T>           
        using (var bcp = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.TableLock))
        using (var reader = ObjectReader.Create(data, columns))
        {
            foreach (var c in columns)
            {
                bcp.ColumnMappings.Add(new SqlBulkCopyColumnMapping { DestinationColumn =c, SourceColumn =c });
            }

            bcp.BulkCopyTimeout = bulkTimeout;
            bcp.BatchSize = batchSize;
            bcp.DestinationTableName = table;


            try
            {

                if(truncate)
                {
                    using IDbConnection connection = new SqlConnection(connectionString);

                    var result = await connection.ExecuteAsync("TRUNCATE TABLE " + table, commandType: CommandType.Text);

                }
                
                
                
                
                await bcp.WriteToServerAsync(reader);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                {
                    string pattern = @"\d+";
                    Match match = Regex.Match(ex.Message.ToString(), pattern);
                    var index = Convert.ToInt32(match.Value) - 1;

                    FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sortedColumns = fi.GetValue(bcp);
                    var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                    FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                    var metadata = itemdata.GetValue(items[index]);

                    var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    throw new Exception(String.Format("Column: {0} contains data with a length greater than: {1}", column, length));
                }

                throw;
            }

        }
    }




    public async Task BulkSave(string connectionString, DataTable table, int bulkTimeout = 12000, int batchSize = 5000, int notifyAfter = 120, bool includeMapping = true)
    {

        // data is an IEnumerable<T>           
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            // OPEN THE CONNECTION
            connection.Open();

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection.ConnectionString))
            {
                bulkCopy.BatchSize = batchSize;
                bulkCopy.BulkCopyTimeout = bulkTimeout;
                bulkCopy.NotifyAfter = notifyAfter;
                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                bulkCopy.DestinationTableName = table.TableName;
                if (includeMapping)
                    MapColumns(table, bulkCopy);
                await bulkCopy.WriteToServerAsync(table);
            }
        }
    }

    public async Task BulkSave(string connectionString, string destination, IDataReader dr, int bulkTimeout = 12000, int batchSize = 5000, int notifyAfter = 120)
    {

        // data is an IEnumerable<T>           
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            // OPEN THE CONNECTION
            connection.Open();

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection.ConnectionString))
            {
                bulkCopy.BatchSize = batchSize;
                bulkCopy.BulkCopyTimeout = bulkTimeout;
                bulkCopy.NotifyAfter = notifyAfter;
                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                bulkCopy.DestinationTableName = destination;

                // Write from the source to the destination.
                await bulkCopy.WriteToServerAsync(dr);
            }
        }
    }



    public static event SqlRowsCopiedEventHandler handle_SQLRowCopied;
    private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
    {
        if (handle_SQLRowCopied != null)
            handle_SQLRowCopied(sender, e);

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

}
