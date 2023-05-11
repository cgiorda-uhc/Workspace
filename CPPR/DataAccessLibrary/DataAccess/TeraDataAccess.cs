using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Teradata.Client.Provider;
using MongoDB.Driver.Core.Configuration;

namespace DataAccessLibrary.DataAccess;

public class TeraDataAccess : IRelationalDataAccess
{
    private readonly IConfiguration _config;
    public TeraDataAccess()
    {

    }


    public TeraDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<T>> LoadData<T>(string sql, CancellationToken token, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<T>> LoadData<T>(string connectionString, string sql)
    {
        //CAUSE TERADATA SUCKS
        if (sql.Contains("{$vti}"))
        {
            return await getTeraDataVTTDataTable<T>(connectionString, sql);
        }


        using (IDbConnection connection = new TdConnection(connectionString))
        {

            var cmd = new CommandDefinition(sql, commandTimeout: 120);
            var result = await connection.QueryAsync<T>(cmd);
            return result;

        }
    }

    private async Task<IEnumerable<T>> getTeraDataVTTDataTable<T>(string connectionString, string sql)
    {
        string[] strArrDVT = null;

        using (IDbConnection connection = new TdConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {

                strArrDVT = sql.Split(new string[] { "{$vti}" }, StringSplitOptions.None);
                await connection.ExecuteAsync(strArrDVT[0] + "ET;", transaction);

                strArrDVT = strArrDVT[1].Split(new string[] { "{$vtc}" }, StringSplitOptions.None);
                await connection.ExecuteAsync(strArrDVT[0] , transaction);

                strArrDVT = strArrDVT[1].Split(new string[] { "{$vts}" }, StringSplitOptions.None);
                await connection.ExecuteAsync(strArrDVT[0] , transaction);

                strArrDVT = strArrDVT[1].Split(new string[] { "{$dvt}" }, StringSplitOptions.None);
                var cmd = new CommandDefinition(strArrDVT[0], commandTimeout: 99999);
                var result = await connection.QueryAsync<T>(cmd);


                await connection.ExecuteAsync(strArrDVT[1], transaction);

                //transaction.Commit();


                return result;

 
            }


        }


    }



    public async Task<DataTable> LoadDataTable(string connectionString, string sql)
    {
        throw new NotImplementedException();
    }


    public async Task<IDataReader> LoadData(string connectionString, string sql)
    {

        throw new NotImplementedException();
    }


    public async Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<T>> LoadData<T, U>(string connectionString, string storedProcedure, U parameters, string connectionId = "VCT_DB")
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

    public async Task BulkSave<T>(string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();
    }



    public async Task BulkSave<T>(string connectionString, string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000)
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
}
