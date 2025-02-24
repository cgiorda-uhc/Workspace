﻿
using Azure;
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using MongoDB.Driver.Core.Configuration;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.ETGFactSymmetry;
using VCPortal_Models.Models.Shared;

namespace DataAccessLibrary.Data.Concrete.ChemoPx;
public class ChemotherapyPX_Repo : IChemotherapyPX_Repo
{
    private readonly IRelationalDataAccess _db;

    public ChemotherapyPX_Repo(IRelationalDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<ChemotherapyPXModel>> GetAllChemotherapyPX() =>
        //RETURN TYPE
        _db.LoadData<ChemotherapyPXModel, dynamic>(storedProcedure: "chemopx.sp_ChemotherapyPX_GetAll", new { });

    public async Task<ChemotherapyPXModel?> GetChemotherapyPX(int id)
    {
        var results = await _db.LoadData<ChemotherapyPXModel, dynamic>(storedProcedure: "chemopx.sp_ChemotherapyPX_Get", new { Id = id });

        return results.FirstOrDefault();
    }
     

    public async Task InsertChemotherapyPXTracking( List<ChemotherapyPX_Tracking_CUD_Dto> chemPX)
    {
        string[] columns = typeof(ChemotherapyPX_Tracking_CUD_Dto).GetProperties().Select(p => p.Name).ToArray().Where(x => x != "IsValid").ToArray();
        //string[] columns = typeof(ChemotherapyPX_Tracking_CUD_Dto).GetProperties().Select(p => p.Name).ToArray();
        await _db.BulkSave<ChemotherapyPX_Tracking_CUD_Dto>("chemopx.ChemotherapyPX_Tracking", chemPX, columns);

        var param = chemPX.FirstOrDefault();
        var update_date = param.UPDATE_DT;
        var username = param.UPDATE_USER;

       await _db.SaveData<dynamic>(storedProcedure: "chemopx.sp_ChemotherapyPX_BulkCUD", new
        {
            username,
            update_date
        });

    }

    public Task<IEnumerable<ChemotherapyPX_Tracking_ReadDto>> GetChemotherapyPXTrackingAsync()
    {

        string strSQL = "SELECT * FROM [chemopx].[vw_GetChemoTracking] v ORDER BY  v.[Tracking_Id];";

        var results = _db.LoadData<ChemotherapyPX_Tracking_ReadDto>(sql: strSQL, connectionStringId: "VCT_DB", has_connectionstring : false) ; 

        return results;
    }

    public Task<IEnumerable<ChemotherapyPXFilters>> GetAllFilters() =>
    //RETURN TYPE
    _db.LoadData<ChemotherapyPXFilters, dynamic>(storedProcedure: "chemopx.sp_ChemotherapyPX_GetAllFilters", new { });



    public Task<IEnumerable<ProcCodesModel>> GetAllProcCodes() =>
    //RETURN TYPE
    _db.LoadData<ProcCodesModel, dynamic>(storedProcedure: "vct.sp_Proc_Codes_GetAll", new { });




    public Task<IEnumerable<Code_Category_Model>> GetAllCodeCategory() =>
       //RETURN TYPE
       _db.LoadData<Code_Category_Model, dynamic>(storedProcedure: "chemopx.sp_Code_Category_GetAll", new { });

    public Task<IEnumerable<ASP_Category_Model>> GetAllASPCategory() =>
       //RETURN TYPE
       _db.LoadData<ASP_Category_Model, dynamic>(storedProcedure: "chemopx.sp_ASP_Category_GetAll", new { });

    public Task<IEnumerable<Drug_Adm_Mode_Model>> GetAllDrugAdmMode() =>
       //RETURN TYPE
       _db.LoadData<Drug_Adm_Mode_Model, dynamic>(storedProcedure: "chemopx.sp_Drug_Adm_Mode_GetAll", new { });


    public Task<IEnumerable<PA_Drugs_Model>> GetAllPADrugs() =>
       //RETURN TYPE
       _db.LoadData<PA_Drugs_Model, dynamic>(storedProcedure: "chemopx.sp_PA_Drugs_GetAll", new { });


    public Task<IEnumerable<CEP_Pay_Cd_Model>> GetAllCEPPayCd() =>
       //RETURN TYPE
       _db.LoadData<CEP_Pay_Cd_Model, dynamic>(storedProcedure: "chemopx.sp_CEP_Pay_Cd_GetAll", new { });

    public Task<IEnumerable<CEP_Enroll_Cd_Model>> GetAllCEPEnrollCd() =>
   //RETURN TYPE
   _db.LoadData<CEP_Enroll_Cd_Model, dynamic>(storedProcedure: "chemopx.sp_CEP_Enroll_Cd_GetAll", new { });


    public Task<IEnumerable<string>> GetSource() =>
//RETURN TYPE
_db.LoadData<string, dynamic>(storedProcedure: "chemopx.sp_Source_GetAll", new { });


    public Task<IEnumerable<string>> GetCEP_Enroll_Excl_Desc() =>
//RETURN TYPE
_db.LoadData<string, dynamic>(storedProcedure: "chemopx.sp_CEP_Enroll_Excl_Desc_GetAll", new { });

}
