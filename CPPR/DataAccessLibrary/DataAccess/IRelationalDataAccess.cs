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


    public List<object> LoadDataMultiple<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40>(string sql, CancellationToken token,
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
                                string connectionId = "VCT_DB");
    Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB");

    Task BulkSave<T>(string connectionString, string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000, bool truncate = false);

    Task BulkSave<T>(string table, IEnumerable<T> data, string[] columns, int bulkTimeout = 120, int batchSize = 5000, string connectionId = "VCT_DB", bool truncate = false);


    Task<object> ExecuteScalar<T>(string storedProcedure, T parameters, string connectionId = "VCT_DB");

    Task<object> ExecuteScalar(string connectionString, string sql);

    Task<object> Execute(string connectionString, string sql);

    Task BulkSave(string connectionString, DataTable table, int bulkTimeout = 120, int batchSize = 5000, int notifyAfter = 120, bool includeMapping = true);

    Task BulkSave(string connectionString, string destination, IDataReader dr, int bulkTimeout = 120, int batchSize = 5000, int notifyAfter = 120);

}