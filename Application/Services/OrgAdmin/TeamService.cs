using Application.Common;
using Application.DTOs.OrgAdmin.Teams;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Repositories.Contracts;

namespace Application.Services.OrgAdmin
{
    public class TeamService(ITeamRepository teamRepository, IUserRepository userRepository, ICurrentTenantService tenant) : ITeamService
    {
        private readonly ITeamRepository _teamRepository = teamRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICurrentTenantService _tenant = tenant;

        public async Task<ServiceResult<List<TeamResponse>>> GetAllAsync()
        {
            var teams = await _teamRepository.GetAllByOrganizationAsync();
            return ServiceResult<List<TeamResponse>>.Success([.. teams.Select(MapToResponse)]);
        }

        public async Task<ServiceResult<TeamResponse>> GetByIdAsync(Guid id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team is null)
            {
                return ServiceResult<TeamResponse>.Failure("Team not found.", ServiceResultStatus.NotFound);
            }

            return ServiceResult<TeamResponse>.Success(MapToResponse(team));
        }

        public async Task<ServiceResult<TeamResponse>> CreateAsync(CreateTeamRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ServiceResult<TeamResponse>.Failure("Team name is required.", ServiceResultStatus.BadRequest);
            }

            if (_tenant.OrganizationId is null)
            {
                return ServiceResult<TeamResponse>.Failure(
                    "Teams must be created within an organization context.", ServiceResultStatus.BadRequest);
            }

            var trimmedName = request.Name.Trim();

            if (await _teamRepository.NameExistsInOrganizationAsync(trimmedName))
            {
                return ServiceResult<TeamResponse>.Failure(
                    $"A team named '{trimmedName}' already exists in this organization.",
                    ServiceResultStatus.Conflict);
            }

            if (request.LeadUserId.HasValue)
            {
                var leadValidation = await ValidateLeadUserAsync(request.LeadUserId.Value);
                if (leadValidation is not null)
                {
                    return ServiceResult<TeamResponse>.Failure(leadValidation, ServiceResultStatus.BadRequest);
                }
            }

            var team = new Team
            {
                Id = Guid.NewGuid(),
                OrganizationId = _tenant.OrganizationId.Value,
                Name = trimmedName,
                LeadUserId = request.LeadUserId,
                CreatedAt = DateTime.UtcNow
            };

            _teamRepository.Add(team);
            await _teamRepository.SaveChangesAsync();

            // Re-fetch with LeadUser/Members included rather than trusting the
            // in-memory entity, which has LeadUserId set but no loaded LeadUser
            // navigation — MapToResponse needs the name, not just the id.
            var created = await _teamRepository.GetByIdAsync(team.Id);
            return ServiceResult<TeamResponse>.Success(MapToResponse(created!), ServiceResultStatus.Created);
        }

        public async Task<ServiceResult<TeamResponse>> UpdateAsync(Guid id, UpdateTeamRequest request)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team is null)
            {
                return ServiceResult<TeamResponse>.Failure("Team not found.", ServiceResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ServiceResult<TeamResponse>.Failure("Team name is required.", ServiceResultStatus.BadRequest);
            }

            var trimmedName = request.Name.Trim();

            if (!string.Equals(team.Name, trimmedName, StringComparison.Ordinal)
                && await _teamRepository.NameExistsInOrganizationAsync(trimmedName))
            {
                return ServiceResult<TeamResponse>.Failure(
                    $"A team named '{trimmedName}' already exists in this organization.",
                    ServiceResultStatus.Conflict);
            }

            if (request.LeadUserId.HasValue)
            {
                var leadValidation = await ValidateLeadUserAsync(request.LeadUserId.Value);
                if (leadValidation is not null)
                {
                    return ServiceResult<TeamResponse>.Failure(leadValidation, ServiceResultStatus.BadRequest);
                }
            }

            team.Name = trimmedName;
            team.LeadUserId = request.LeadUserId; // null clears the lead — same field doubles as "assign" and "unassign"

            await _teamRepository.SaveChangesAsync();

            var updated = await _teamRepository.GetByIdAsync(id);
            return ServiceResult<TeamResponse>.Success(MapToResponse(updated!));
        }

        public async Task<ServiceResult<string>> DeleteAsync(Guid id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team is null)
            {
                return ServiceResult<string>.Failure("Team not found.", ServiceResultStatus.NotFound);
            }

            var memberCount = team.Members?.Count ?? 0;

            _teamRepository.Remove(team);
            await _teamRepository.SaveChangesAsync();

            var message = memberCount > 0
                ? $"Team deleted. {memberCount} member(s) had this team and are now unassigned — reassign them as needed."
                : "Team deleted.";

            return ServiceResult<string>.Success(message);
        }

        /// <summary>
        /// Returns null if valid, or an error message if not. Checked here
        /// rather than relying solely on the FK constraint, for two reasons:
        /// (1) a user from a DIFFERENT organization could otherwise be set as a
        /// team lead — the FK alone wouldn't catch that since Users.Id is
        /// globally unique, only the application layer enforces same-org;
        /// (2) a clear 400 with a real message beats an opaque FK violation.
        /// </summary>
        private async Task<string?> ValidateLeadUserAsync(Guid leadUserId)
        {
            var user = await _userRepository.GetByIdAsync(leadUserId);
            if (user is null)
            {
                return "The specified lead user does not exist.";
            }

            // GetByIdAsync already goes through the tenant query filter, so a
            // result here is guaranteed to be in the caller's own org already —
            // this check is technically redundant given that filter, but kept
            // as an explicit, readable assertion rather than relying silently
            // on filter behavior a future reader might not immediately notice.
            if (user.OrganizationId != _tenant.OrganizationId)
            {
                return "The specified lead user does not belong to this organization.";
            }

            return null;
        }

        private TeamResponse MapToResponse(Team team)
        {
            return new TeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                LeadUserId = team.LeadUserId,
                LeadUserName = team.LeadUser?.Name,
                MemberCount = team.Members?.Count ?? 0,
                CreatedAt = team.CreatedAt
            };
        }
    }
}
