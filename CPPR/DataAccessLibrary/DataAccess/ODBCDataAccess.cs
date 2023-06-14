using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using System.Data.Odbc;
using System.Data.Common;

namespace DataAccessLibrary.DataAccess;


public class ODBCDataAccess : IRelationalDataAccess
{
    private readonly IConfiguration _config;
    public ODBCDataAccess()
    {

    }


    public ODBCDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<T>> LoadData<T>(string sql, CancellationToken token, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<T>> LoadData<T>(string connectionString, string sql)
    {
        throw new NotImplementedException();
    }

    public async Task<IDataReader> LoadData(string connectionString, string sql)
    {

       var connection = new OdbcConnection(connectionString);
        try
        {
            connection.Open();
            OdbcCommand command = new OdbcCommand(sql, connection);
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


    public async Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> LoadData<T, U>(string connectionString, string storedProcedure, U parameters, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();
    }


    public async Task<DataTable> LoadDataTable(string connectionString, string sql)
    {
        throw new NotImplementedException();
    }
    public async Task<object> ExecuteScalar<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();

    }

    public async Task<object> Execute(string connectionString, string sql)
    {
        throw new NotImplementedException();

    }

    public async Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();
    }

    public async Task BulkSave<T>(string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000, string connectionId = "VCT_DB", bool truncate = false)
    {
        throw new NotImplementedException();
    }



    public async Task BulkSave<T>(string connectionString, string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000, bool truncate = false)
    {
        throw new NotImplementedException();
    }


    public async Task BulkSave(string connectionString, DataTable table, int bulkTimeout = 120, int batchSize = 5000, int notifyAfter = 120, bool includeMapping = true)
    {
        throw new NotImplementedException();
    }

    public async Task BulkSave(string connectionString, string destination, IDataReader dr, int bulkTimeout = 120, int batchSize = 5000, int notifyAfter = 120)
    {

        throw new NotImplementedException();

    }

    public Task<IEnumerable<T>> LoadData<T>(string sql, string connectionStringId = "VCT_DB", bool has_connectionstring = false)
    {
        throw new NotImplementedException();
    }

    public Task<object> ExecuteScalar(string connectionString, string sql)
    {
        throw new NotImplementedException();
    }
}
