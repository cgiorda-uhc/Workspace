
using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.Data.Context;
using Microsoft.EntityFrameworkCore;
using VCPortal_Models.Dtos.ChemoPx;
using VCPortal_Models.Models.ChemoPx;
using VCPortal_Models.Models.Shared;

namespace DataAccessLibrary.Data.Concrete.ChemoPx;
public class ChemotherapyPX_EF_Repo : IChemotherapyPX_Repo
{
    private readonly ChemotherapyPX_Context _context;

    public ChemotherapyPX_EF_Repo(ChemotherapyPX_Context context)
    {
        _context = context;
    }


    public Task<IEnumerable<ChemotherapyPXModel>> GetAllChemotherapyPX()
    {
        var results = _context.ChemotherapyPXContext.AsEnumerable();
        return Task.FromResult(results);
    }


    public async Task<ChemotherapyPXModel?> GetChemotherapyPX(int id)
    {
        var results = await _context.ChemotherapyPXContext.FirstOrDefaultAsync(p => p.Id == id);

        return results;
    }

    public async Task<int?> InsertChemotherapyPX(ChemotherapyPXModel chemPX)
    {
        if (chemPX == null)
        {
            throw new ArgumentNullException(nameof(chemPX));
        }

        _context.Add(chemPX);
        _context.SaveChanges();

        //return Task.CompletedTask;
        return chemPX.Id;
    }

    public Task InsertManyChemotherapyPX(ChemotherapyPXModel chemPX)
    {
        //IMPLEMNET ME!!!
        throw new ArgumentNullException(nameof(chemPX));
    }


    public async Task UpdateChemotherapyPX(ChemotherapyPXModel chemPX)
    {
        var results = await _context.ChemotherapyPXContext.FirstOrDefaultAsync(p => p.Id == chemPX.Id);
        if (results == null)
            throw new KeyNotFoundException();//NOT THIS?

        _context.Entry(results).CurrentValues.SetValues(chemPX);
        _context.SaveChanges();

        return;
    }

    public async Task DeleteChemotherapyPX(int id)
    {
        var results = await _context.ChemotherapyPXContext.FirstOrDefaultAsync(p => p.Id == id);
        if (results == null)
            throw new KeyNotFoundException();//NOT THIS?

        _context.ChemotherapyPXContext.Remove(results);
        _context.SaveChanges();

        return;
    }

    //public bool SaveChanges()
    //{
    //    return (_context.SaveChanges() >= 0);
    //}
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

    public Task<IEnumerable<ChemotherapyPX_Tracking_ReadDto>> GetChemotherapyPXTrackingAsync()
    {
        throw new NotImplementedException();
    }
}
