﻿using Dapper;
using VCPortal_Models.Models.ProcCodeTrends;
using VCPortal_Models.Parameters.ProcCodeTrends;

namespace DataAccessLibrary.Data.Abstract
{
    public interface IProcCodeTrends_Repo
    {
        Task<IEnumerable<MM_FINAL_Model>> GetMM_FINAL_Async(CancellationToken token);

        Task<IEnumerable<string>> GetPROC_CD_Async(CancellationToken token);

        Task<IEnumerable<CLM_PHYS_Model>> GetCLM_PHYS_Async(ProcCodeTrends_Parameters pct_param, CancellationToken token);


        Task<IEnumerable<CLM_OP_Model>> GetCLM_OP_Async(ProcCodeTrends_Parameters pct_param, CancellationToken token);

        Task<IEnumerable<DateSpan_Model>> GetDateSpan_Async( CancellationToken token);

        Task<CLM_OP_Report_Model> GetMainPCTReport_Async(ProcCodeTrends_Parameters pct_param, CancellationToken token);
    }
}