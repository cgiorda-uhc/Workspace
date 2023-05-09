
using Azure;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Driver.Core.Connections;
using System.Data;
using System.Text;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.ETGFactSymmetry;
using VCPortal_Models.Models.MHP;
using VCPortal_Models.Parameters.MHP;

namespace DataAccessLibrary.Data.Concrete.ETGFactSymmetry;

public class ETGFactSymmetry_Repo : IETGFactSymmetry_Repo
{


    private readonly IRelationalDataAccess _db;

    public ETGFactSymmetry_Repo(IRelationalDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<ETGFactSymmetry_ReadDto>> GetETGFactSymmetryDisplayAsync(CancellationToken token)
    {

        string strSQL = "SELECT * FROM [dbo].[VW_ETG_Symmetry_Main_Interface] ORDER BY Premium_Specialty, ETG_Description;";

        var results = _db.LoadData<ETGFactSymmetry_ReadDto>(sql: strSQL, token, connectionId: "ETGSymmetry");

        return results;
    }



    public Task<IEnumerable<ETGFactSymmetry_Tracking_ReadDto>> GetETGTrackingAsync(CancellationToken token)
    {

        string strSQL = "SELECT * FROM [dbo].[vw_GetETGSymmetryTracking] v ORDER BY  v.Tracker_Id;";

        var results = _db.LoadData<ETGFactSymmetry_Tracking_ReadDto>(sql: strSQL, token, connectionId: "ETGSymmetry");

        return results;
    }


    public Task<IEnumerable<ETGPatientCentricConfig>> GetETGPatientCentricConfigAsync(CancellationToken token)
    {

        string strSQL = "SELECT * FROM [dbo].[VW_ETG_Symmetry_PATIENT_CENTRIC_CONFIG] v ORDER BY  v.[Base_ETG],v.[Premium_Specialty];";

        var results = _db.LoadData<ETGPatientCentricConfig>(sql: strSQL, token, connectionId: "ETGSymmetry");

        return results;
    }

    public Task<IEnumerable<ETGPopEpisodeConfig>> GetETGPopEpisodeConfigAsync(CancellationToken token)
    {

        string strSQL = "SELECT * FROM [dbo].[VW_ETG_Symmetry_POP_EPISODE_CONFIG] v  ORDER BY  v.[Base_ETG],v.[Premium_Specialty];";

        var results = _db.LoadData<ETGPopEpisodeConfig>(sql: strSQL, token, connectionId: "ETGSymmetry");

        return results;
    }


    public Task<IEnumerable<ETGRxNrxConfig>> GetETGRxNrxConfigAsync(CancellationToken token)
    {

        string strSQL = "SELECT * FROM [dbo].[VW_ETG_Symmetry_RX_NRX_CONFIG] v  ORDER BY  v.[Base_ETG],v.[Premium_Specialty];";

        var results = _db.LoadData<ETGRxNrxConfig>(sql: strSQL, token, connectionId: "ETGSymmetry");

        return results;
    }



    public async Task InsertETGFactSymmetryTracking(List<ETGFactSymmetry_Tracking_UpdateDto> ETG, string connectionId)
    {
        string[] columns = typeof(ETGFactSymmetry_Tracking_UpdateDto).GetProperties().Select(p => p.Name).ToArray();
        await _db.BulkSave<ETGFactSymmetry_Tracking_UpdateDto>("dbo.ETG_Fact_Symmetry_Update_Tracker", ETG, columns, connectionId : connectionId);

        var param = ETG.FirstOrDefault();
        var update_date = param.update_date;
        var username = param.username;

        await _db.SaveData<dynamic>(storedProcedure: "dbo.sp_ETGFactSymmetry_BulkUpdate", new
        {
            username,
            update_date
        }, connectionId: connectionId);

    }



    public Task UpdateETGFactSymmetry(ETGFactSymmetry_UpdateDto etg) =>
        _db.SaveData<dynamic>(storedProcedure: "dbo.sp_ETGSymmetry_Update", new
        {
            etg.ETG_Fact_Symmetry_id,
            etg.Current_Patient_Centric_Mapping,
            etg.Previous_Patient_Centric_Mapping,
            etg.Current_Mapping,
            etg.Previous_Mapping,
            etg.Current_Mapping_Orginal,
            etg.Previous_Mapping_Orginal,
            etg.Current_Episode_Cost_Treatment_Indicator,
            etg.Previous_Episode_Cost_Treatment_Indicator,
            etg.Current_Attribution,
            etg.Previous_Attribution,
            etg.Pop_Cost_Current_Treatment_Indicator,
            etg.Pop_Cost_Previous_Treatment_Indicator,
            etg.LOBCurrentString,
            etg.LOBPreviousString,
            etg.Has_Commercial,
            etg.Has_Medicare,
            etg.Has_Medicaid,
            etg.Pop_Cost_Change_Comments,
            etg.Episode_Cost_Change_Comments,
            etg.Patient_Centric_Change_Comments,
            etg.User
    });
}