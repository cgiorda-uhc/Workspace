using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using FastMember;
using System.Text.RegularExpressions;
using System.Reflection;

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

        var cmd = new CommandDefinition(sql, cancellationToken: token, commandTimeout:120);
        var result = await connection.QueryAsync<T>(cmd); 
        return result;
    }


    public async Task<IEnumerable<T>> LoadData<T>(string connectionString, string sql  )
    {
        using IDbConnection connection = new SqlConnection(connectionString);

        var cmd = new CommandDefinition(sql, commandTimeout: 1200);
        var result = await connection.QueryAsync<T>(cmd);
        return result;
    }



    public async Task<IEnumerable<T>> LoadData<T>( string sql, string connectionStringId = "VCT_DB", bool has_connectionstring = false)
    {

        using IDbConnection connection =new SqlConnection((has_connectionstring ? connectionStringId :  _config.GetConnectionString(connectionStringId)));

        var cmd = new CommandDefinition(sql, commandTimeout: 120);
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


    public async Task<object> ExecuteScalar<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

        var result = await connection.ExecuteScalarAsync<object>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        return result;

    }

    public async Task<object> Execute(string connectionString, string sql)
    {
        using IDbConnection connection = new SqlConnection(connectionString);

        var result = await connection.ExecuteAsync(sql, commandType: CommandType.Text);
        return result;

    }

    public async Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB")
	{
		using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

		//DAPPER CALL
		await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
	}

    public async Task BulkSave<T>(string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000, string connectionId = "VCT_DB")
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



    public async Task BulkSave<T>(string connectionString, string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000)
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




    public async Task BulkSave(string connectionString, DataTable table, int bulkTimeout = 120, int batchSize = 5000, int notifyAfter = 120, bool includeMapping = true)
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

    public async Task BulkSave(string connectionString, string destination, IDataReader dr, int bulkTimeout = 120, int batchSize = 5000, int notifyAfter = 120)
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
