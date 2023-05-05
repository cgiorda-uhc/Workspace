using VCPortal_Models.Dtos.ETGFactSymmetry;
using VCPortal_Models.Models.ETGFactSymmetry;
using VCPortal_Models.Parameters.MHP;

namespace DataAccessLibrary.Data.Abstract;

public interface IETGFactSymmetry_Repo
{
    Task<IEnumerable<ETGFactSymmetry_ReadDto>> GetETGFactSymmetryDisplayAsync(CancellationToken token);

    Task<IEnumerable<ETGPatientCentricConfig>> GetETGPatientCentricConfigAsync(CancellationToken token);
    Task<IEnumerable<ETGPopEpisodeConfig>> GetETGPopEpisodeConfigAsync(CancellationToken token);

    Task<IEnumerable<ETGRxNrxConfig>> GetETGRxNrxConfigAsync(CancellationToken token);

    Task UpdateETGFactSymmetry(ETGFactSymmetry_UpdateDto etg);

    Task InsertETGFactSymmetryTracking(List<ETGFactSymmetry_Tracking_UpdateDto> ETG, string connectionId);
}