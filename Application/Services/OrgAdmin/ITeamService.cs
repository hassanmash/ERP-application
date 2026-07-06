using Application.Common;
using Application.DTOs.OrgAdmin.Teams;

namespace Application.Services.OrgAdmin
{
    public interface ITeamService
    {
        Task<ServiceResult<List<TeamResponse>>> GetAllAsync();

        Task<ServiceResult<TeamResponse>> GetByIdAsync(Guid id);

        Task<ServiceResult<TeamResponse>> CreateAsync(CreateTeamRequest request);

        Task<ServiceResult<TeamResponse>> UpdateAsync(Guid id, UpdateTeamRequest request);

        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}
