﻿using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ETGFactSymmetry;
using VCPortal_Models.Models.ETGFactSymmetry.Configs;
using VCPortal_Models.Models.ETGFactSymmetry.Dataloads;
using VCPortal_Models.Parameters.MHP;

namespace DataAccessLibrary.Data.Abstract;

public interface IETGFactSymmetry_Repo
{
    Task<IEnumerable<ETGFactSymmetry_ReadDto>> GetETGFactSymmetryDisplayAsync(CancellationToken token);


    Task<IEnumerable<ETG_PEC_Summary_Final>> GetETGFactSymmetryPECDisplayAsync(CancellationToken token);


    Task<IEnumerable<ETGSummaryAdhocConfig>> GetETGFactSymmetryAdhocAsync(int version);

    Task<IEnumerable<ETGFactSymmetry_Tracking_ReadDto>> GetETGTrackingAsync(CancellationToken token);

    Task<IEnumerable<ETGPatientCentricConfig>> GetETGPatientCentricConfigAsync(CancellationToken token);

    Task<IEnumerable<ETGPopEpisodeConfig>> GetETGPopEpisodeConfigAsync(CancellationToken token);

    Task<IEnumerable<ETGRxNrxConfig>> GetETGRxNrxConfigAsync(CancellationToken token);

    Task<IEnumerable<ETG_CNFG_ETG_NRX_EXCLD>> GetETG_CNFG_ETG_NRX_EXCLD(CancellationToken token);

    Task<IEnumerable<ETG_CNFG_ETG_NRX_COMPARE>> GetETG_CNFG_ETG_NRX_COMPARE(CancellationToken token);

    Task<IEnumerable<ETG_CNFG_ETG_SPCL>> GetETG_CNFG_ETG_SPCL(CancellationToken token);


    Task<IEnumerable<ETG_CNFG_PC_ETG_NRX>> GetETG_CNFG_PC_ETG_NRX(CancellationToken token);

    Task<IEnumerable<ETG_PTC_Modeling_Model>> GetETG_PTC_Modeling_Model(CancellationToken token);

    Task<IEnumerable<ETG_UGAP_CFG_Model>> GetETG_UGAP_CFG_Model(CancellationToken token);

    Task<IEnumerable<ETGSummaryPECFinalConfig>> GetETGSummaryFinalAsync(CancellationToken token);

    Task<IEnumerable<ETGVersion_Model>> GetPDVersionsAsync(CancellationToken token);

    Task UpdateETGFactSymmetry(ETGFactSymmetry_UpdateDto etg);

    Task InsertETGFactSymmetryTracking(List<ETGFactSymmetry_Tracking_UpdateDto> ETG, string connectionId);

    Task<IEnumerable<ETGSummaryFinalConfig>> GetETGSummaryPTCFinalAsync(CancellationToken token);
    Task<IEnumerable<ETGFactSymmetry_ReadDto>> GetETGFactSymmetryPTCDisplayAsync(CancellationToken token);

    Task<IEnumerable<ETG_PTC_Summary_Final>> GetETGFactSymmetryPTCConfigAsync(CancellationToken token);

    Task<IEnumerable<ETG_Lastest_Model>> GetETGLatestAsync(CancellationToken token);
}