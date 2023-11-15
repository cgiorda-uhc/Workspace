using Dapper;
using System.Data;
using System.Dynamic;
using static Dapper.SqlMapper;

namespace DataAccessLibrary.DataAccess;

public interface IRelationalDataAccess
{
    Task<IEnumerable<T>> LoadData<T>(string sql, CancellationToken token, string connectionId = "VCT_DB");

    Task<IEnumerable<T>> LoadData<T>(string connectionString, string sql);

    Task<IEnumerable<T>> LoadData<T>(string sql, string connectionStringId = "VCT_DB", bool has_connectionstring = false);
    Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "VCT_DB");
    Task<IEnumerable<T>> LoadData<T, U>(string connectionString, string storedProcedure, U parameters, string connectionId = "VCT_DB");
    Task<IDataReader> LoadData(string connectionString, string sql);

    Task<DataTable> LoadDataTable(string connectionString, string sql);


    Task<SqlMapper.GridReader> LoadDataMultiple(string sql, CancellationToken token, string connectionId = "VCT_DB");

    Tuple<IEnumerable<T1>, IEnumerable<T2>> GetMultiple<T1, T2>(string sql, CancellationToken token,
                                    Func<GridReader, IEnumerable<T1>> func1,
                                    Func<GridReader, IEnumerable<T2>> func2, string connectionId = "VCT_DB");



    Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> GetMultiple<T1, T2, T3>(string sql, CancellationToken token,
                                    Func<GridReader, IEnumerable<T1>> func1,
                                    Func<GridReader, IEnumerable<T2>> func2,
                                    Func<GridReader, IEnumerable<T3>> func3, string connectionId = "VCT_DB");

    Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB");

    Task BulkSave<T>(string connectionString, string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000, bool truncate = false);

    Task BulkSave<T>(string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000, string connectionId = "VCT_DB", bool truncate = false);


    Task<object> ExecuteScalar<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB");

    Task<object> ExecuteScalar(string connectionString, string sql);

    Task<object> Execute(string connectionString, string sql);

    Task BulkSave(string connectionString, DataTable table, int bulkTimeout = 120, int batchSize = 5000, int notifyAfter = 120, bool includeMapping = true);

    Task BulkSave(string connectionString, string destination, IDataReader dr, int bulkTimeout = 120, int batchSize = 5000, int notifyAfter = 120);

}