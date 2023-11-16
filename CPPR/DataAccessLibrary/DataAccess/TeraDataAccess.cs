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

            var cmd = new CommandDefinition(sql, commandTimeout: 20000);
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

    public async Task<object> ExecuteScalar(string connectionString, string sql)
    {
        using IDbConnection connection = new TdConnection(connectionString);

        var result = await connection.ExecuteScalarAsync(sql, commandType: CommandType.Text);
        return result;

    }




    public async Task<object> Execute(string connectionString, string sql)
    {

        using IDbConnection connection = new TdConnection(connectionString);

        var result = await connection.ExecuteAsync(sql, commandType: CommandType.Text);
        return result;

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



    public List<object> LoadDataMultiple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40>(string sql, CancellationToken token, Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2, Func<SqlMapper.GridReader, IEnumerable<T3>> func3 = null, Func<SqlMapper.GridReader, IEnumerable<T4>> func4 = null, Func<SqlMapper.GridReader, IEnumerable<T5>> func5 = null, Func<SqlMapper.GridReader, IEnumerable<T6>> func6 = null, Func<SqlMapper.GridReader, IEnumerable<T7>> func7 = null, Func<SqlMapper.GridReader, IEnumerable<T8>> func8 = null, Func<SqlMapper.GridReader, IEnumerable<T9>> func9 = null, Func<SqlMapper.GridReader, IEnumerable<T10>> func10 = null, Func<SqlMapper.GridReader, IEnumerable<T11>> func11 = null, Func<SqlMapper.GridReader, IEnumerable<T12>> func12 = null, Func<SqlMapper.GridReader, IEnumerable<T13>> func13 = null, Func<SqlMapper.GridReader, IEnumerable<T14>> func14 = null, Func<SqlMapper.GridReader, IEnumerable<T15>> func15 = null, Func<SqlMapper.GridReader, IEnumerable<T16>> func16 = null, Func<SqlMapper.GridReader, IEnumerable<T17>> func17 = null, Func<SqlMapper.GridReader, IEnumerable<T18>> func18 = null, Func<SqlMapper.GridReader, IEnumerable<T19>> func19 = null, Func<SqlMapper.GridReader, IEnumerable<T20>> func20 = null, Func<SqlMapper.GridReader, IEnumerable<T21>> func21 = null, Func<SqlMapper.GridReader, IEnumerable<T22>> func22 = null, Func<SqlMapper.GridReader, IEnumerable<T23>> func23 = null, Func<SqlMapper.GridReader, IEnumerable<T24>> func24 = null, Func<SqlMapper.GridReader, IEnumerable<T25>> func25 = null, Func<SqlMapper.GridReader, IEnumerable<T26>> func26 = null, Func<SqlMapper.GridReader, IEnumerable<T27>> func27 = null, Func<SqlMapper.GridReader, IEnumerable<T28>> func28 = null, Func<SqlMapper.GridReader, IEnumerable<T29>> func29 = null, Func<SqlMapper.GridReader, IEnumerable<T30>> func30 = null, Func<SqlMapper.GridReader, IEnumerable<T31>> func31 = null, Func<SqlMapper.GridReader, IEnumerable<T32>> func32 = null, Func<SqlMapper.GridReader, IEnumerable<T33>> func33 = null, Func<SqlMapper.GridReader, IEnumerable<T34>> func34 = null, Func<SqlMapper.GridReader, IEnumerable<T35>> func35 = null, Func<SqlMapper.GridReader, IEnumerable<T36>> func36 = null, Func<SqlMapper.GridReader, IEnumerable<T37>> func37 = null, Func<SqlMapper.GridReader, IEnumerable<T38>> func38 = null, Func<SqlMapper.GridReader, IEnumerable<T39>> func39 = null, Func<SqlMapper.GridReader, IEnumerable<T40>> func40 = null, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();
    }
}
