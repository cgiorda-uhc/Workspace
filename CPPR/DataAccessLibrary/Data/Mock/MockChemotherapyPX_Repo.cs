
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.Shared;

namespace DataAccess.Data.Mock;
public class ChemotherapyPX_Data : IChemotherapyPX_Repo
{
	private readonly IRelationalDataAccess _db;

	public ChemotherapyPX_Data(IRelationalDataAccess db)
	{
		_db = db;
	}

	public Task<IEnumerable<ChemotherapyPXModel>> GetAllChemotherapyPX() =>
		//RETURN TYPE
		_db.LoadData<ChemotherapyPXModel, dynamic>(storedProcedure: "dbo.sp_ChemotherapyPX_GetAll", new { });

	public async Task<ChemotherapyPXModel?> GetChemotherapyPX(int id)
	{
		var results = await _db.LoadData<ChemotherapyPXModel, dynamic>(storedProcedure: "dbo.sp_ChemotherapyPX_Get", new { Id = id });

		return results.FirstOrDefault();
	}
	//public Task<int> InsertChemotherapyPX(ChemotherapyPXModel chemPX) =>
	//	_db.SaveData(storedProcedure: "dbo.sp_ChemotherapyPX_Insert",
	//		new
	//		{
	//			chemPX.CODE,
	//			chemPX.CODE_DESC,
	//			chemPX.GENERIC_NAME,
	//			chemPX.TRADE_NAME,
	//			chemPX.CKPT_INHIB_IND,
	//			chemPX.ANTI_EMETIC_IND,
	//			chemPX.CODE_TYPE,
	//			chemPX.CODE_EFF_DT,
	//			chemPX.CODE_END_DT,
	//			chemPX.NHNR_CANCER_THERAPY,
	//			chemPX.CODE_CATEGORY,
	//			chemPX.ASP_CATEGORY,
	//			chemPX.DRUG_ADM_MODE,
	//			chemPX.PA_DRUGS,
	//			chemPX.PA_EFF_DT,
	//			chemPX.PA_END_DT,
	//			chemPX.CEP_PAY_CD,
	//			chemPX.CEP_ENROLL_CD,
	//			chemPX.CEP_ENROLL_EXCL_DESC,
	//			chemPX.NOVEL_STATUS_IND,
	//			chemPX.FIRST_NOVEL_MNTH,
	//			chemPX.SOURCE,
	//			chemPX.UPDATE_DT,
	//			chemPX.Is_Archived
	//		});


    public async Task<int> InsertChemotherapyPX(ChemotherapyPXModel chemPX)
    {
        //IMPLEMNET ME!!!
        throw new ArgumentNullException(nameof(chemPX));
    }

    public Task InsertManyChemotherapyPX(ChemotherapyPXModel chemPX)
    {
        //IMPLEMNET ME!!!
        throw new ArgumentNullException(nameof(chemPX));
    }

    public Task UpdateChemotherapyPX(ChemotherapyPXModel chemPX) =>
		_db.SaveData(storedProcedure: "dbo.sp_ChemotherapyPX_Update", chemPX);


	public Task DeleteChemotherapyPX(int id) =>
		_db.SaveData(storedProcedure: "dbo.sp_ChemotherapyPX_Delete", new { Id = id });

    public Task<IEnumerable<ChemotherapyPXFilters>> GetAllFilters() { throw new NotImplementedException(); }
    public Task<IEnumerable<ProcCodesModel>> GetAllProcCodes() { throw new NotImplementedException(); }

    public Task<IEnumerable<Code_Category_Model>> GetAllCodeCategory() { throw new NotImplementedException(); }

    public Task<IEnumerable<ASP_Category_Model>> GetAllASPCategory() { throw new NotImplementedException(); }

    public Task<IEnumerable<Drug_Adm_Mode_Model>> GetAllDrugAdmMode() { throw new NotImplementedException(); }


    public Task<IEnumerable<PA_Drugs_Model>> GetAllPADrugs() { throw new NotImplementedException(); }


    public Task<IEnumerable<CEP_Pay_Cd_Model>> GetAllCEPPayCd() { throw new NotImplementedException(); }

    public Task<IEnumerable<CEP_Enroll_Cd_Model>> GetAllCEPEnrollCd() { throw new NotImplementedException(); }

    public Task<IEnumerable<string>> GetSource()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetCEP_Enroll_Excl_Desc()
    {
        throw new NotImplementedException();
    }

    public Task InsertChemotherapyPXTracking(List<ChemotherapyPX_Tracking_CUD_Dto> chemPX)
    {
        throw new NotImplementedException();
    }
}
