﻿using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using System.Data.Odbc;
using System.Data.Common;
using Snowflake.Data.Client;

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
        using IDbConnection connection = new OdbcConnection(connectionString);

        var cmd = new CommandDefinition(sql, commandTimeout: 12000);
        var result = await connection.QueryAsync<T>(cmd);
        return result;
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
        using OdbcConnection connection = new OdbcConnection(connectionString);

        var table = new DataTable();
        OdbcCommand command = new OdbcCommand(sql, connection);
        command.CommandTimeout = 9999999;
        using (var da = new OdbcDataAdapter(command))
        {
            await Task.Run(() => da.Fill(table));
        }

        return table;
    }
    public async Task<object> ExecuteScalar<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();

    }

    public async Task<object> Execute(string connectionString, string sql)
    {
        using IDbConnection connection = new OdbcConnection(connectionString);

        var cmd = new CommandDefinition(sql, commandTimeout: 12000);
        var result = await connection.ExecuteAsync(cmd);
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


    public async Task BulkSave(string connectionString, DataTable table, int bulkTimeout = 120, int batchSize = 5000, int notifyAfter = 120, bool includeMapping = true, bool truncate = false)
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


 


    public List<object> LoadDataMultiple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40>(string sql, CancellationToken token, Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2, Func<SqlMapper.GridReader, IEnumerable<T3>> func3 = null, Func<SqlMapper.GridReader, IEnumerable<T4>> func4 = null, Func<SqlMapper.GridReader, IEnumerable<T5>> func5 = null, Func<SqlMapper.GridReader, IEnumerable<T6>> func6 = null, Func<SqlMapper.GridReader, IEnumerable<T7>> func7 = null, Func<SqlMapper.GridReader, IEnumerable<T8>> func8 = null, Func<SqlMapper.GridReader, IEnumerable<T9>> func9 = null, Func<SqlMapper.GridReader, IEnumerable<T10>> func10 = null, Func<SqlMapper.GridReader, IEnumerable<T11>> func11 = null, Func<SqlMapper.GridReader, IEnumerable<T12>> func12 = null, Func<SqlMapper.GridReader, IEnumerable<T13>> func13 = null, Func<SqlMapper.GridReader, IEnumerable<T14>> func14 = null, Func<SqlMapper.GridReader, IEnumerable<T15>> func15 = null, Func<SqlMapper.GridReader, IEnumerable<T16>> func16 = null, Func<SqlMapper.GridReader, IEnumerable<T17>> func17 = null, Func<SqlMapper.GridReader, IEnumerable<T18>> func18 = null, Func<SqlMapper.GridReader, IEnumerable<T19>> func19 = null, Func<SqlMapper.GridReader, IEnumerable<T20>> func20 = null, Func<SqlMapper.GridReader, IEnumerable<T21>> func21 = null, Func<SqlMapper.GridReader, IEnumerable<T22>> func22 = null, Func<SqlMapper.GridReader, IEnumerable<T23>> func23 = null, Func<SqlMapper.GridReader, IEnumerable<T24>> func24 = null, Func<SqlMapper.GridReader, IEnumerable<T25>> func25 = null, Func<SqlMapper.GridReader, IEnumerable<T26>> func26 = null, Func<SqlMapper.GridReader, IEnumerable<T27>> func27 = null, Func<SqlMapper.GridReader, IEnumerable<T28>> func28 = null, Func<SqlMapper.GridReader, IEnumerable<T29>> func29 = null, Func<SqlMapper.GridReader, IEnumerable<T30>> func30 = null, Func<SqlMapper.GridReader, IEnumerable<T31>> func31 = null, Func<SqlMapper.GridReader, IEnumerable<T32>> func32 = null, Func<SqlMapper.GridReader, IEnumerable<T33>> func33 = null, Func<SqlMapper.GridReader, IEnumerable<T34>> func34 = null, Func<SqlMapper.GridReader, IEnumerable<T35>> func35 = null, Func<SqlMapper.GridReader, IEnumerable<T36>> func36 = null, Func<SqlMapper.GridReader, IEnumerable<T37>> func37 = null, Func<SqlMapper.GridReader, IEnumerable<T38>> func38 = null, Func<SqlMapper.GridReader, IEnumerable<T39>> func39 = null, Func<SqlMapper.GridReader, IEnumerable<T40>> func40 = null, string connectionId = "VCT_DB")
    {
        throw new NotImplementedException();
    }
}
