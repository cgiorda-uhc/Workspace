using DataAccessLibrary.Data.Abstract;
using DataAccessLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCPortal_WPF_ViewModel.Shared;
public class DBRepoModel
{

    public IRelationalDataAccess db_sql {  get; set; }
    public IChemotherapyPX_Repo chemo_sql { get; set; }
    public IMHPUniverse_Repo mhp_sql { get; set; }
    public IProcCodeTrends_Repo pct_db { get; set; }
    public IEDCAdhoc_Repo edc_db { get; set; }
    public IETGFactSymmetry_Repo etg_db { get; set; }
}
