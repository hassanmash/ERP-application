using Application.Common;
using Application.DTOs.Sales;
using Application.Services.Security;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Contracts;
using Infrastructure.Repositories.Contracts;

namespace Application.Services.Sales
{
    public class LeadService(
        IPermissionService permissionService,
        ILeadRepository leadRepository,
        ILeadStatusHistoryRepository leadStatusHistoryRepository,
        IUserRepository userRepository,
        ICurrentTenantService tenant)
        : ILeadService
    {
        private readonly IPermissionService _permissionService = permissionService;
        private readonly ILeadRepository _leadRepository = leadRepository;
        private readonly ILeadStatusHistoryRepository _leadStatusHistoryRepository = leadStatusHistoryRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICurrentTenantService _tenant = tenant;

        public async Task<ServiceResult<List<LeadResponse>>> GetAllAsync()
        {
            var scope = await _permissionService.GetLeadViewScopeAsync();

            var leads = await _leadRepository.GetAccessibleLeadsAsync(
                scope,
                _tenant.UserId!.Value);

            return ServiceResult<List<LeadResponse>>.Success([.. leads.Select(MapToResponse)]);

            //var leads = await _leadRepository.GetAllAsync();

            //return ServiceResult<List<LeadResponse>>
            //    .Success([.. leads.Select(MapToResponse)]);
        }

        public async Task<ServiceResult<LeadResponse>> GetByIdAsync(Guid id)
        {
            var lead = await _leadRepository.GetByIdAsync(id);

            if (lead is null)
            {
                return ServiceResult<LeadResponse>.Failure(
                    "Lead not found.",
                    ServiceResultStatus.NotFound);
            }

            return ServiceResult<LeadResponse>.Success(
                MapToResponse(lead));
        }

        public async Task<ServiceResult<LeadResponse>> CreateAsync(CreateLeadRequest request)
        {
            if (_tenant.OrganizationId is null)
            {
                return ServiceResult<LeadResponse>.Failure(
                    "Lead must belong to an organization.",
                    ServiceResultStatus.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.MobileNumber) ||
                string.IsNullOrWhiteSpace(request.Source) ||
                string.IsNullOrWhiteSpace(request.Project))
            {
                return ServiceResult<LeadResponse>.Failure(
                    "All required fields are required.",
                    ServiceResultStatus.BadRequest);
            }

            var assignedUser = await _userRepository.GetByIdAsync(request.AssignedUserId);

            if (assignedUser is null)
            {
                return ServiceResult<LeadResponse>.Failure(
                    "Assigned user not found.",
                    ServiceResultStatus.BadRequest);
            }

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                OrganizationId = _tenant.OrganizationId.Value,
                Name = request.Name.Trim(),
                MobileNumber = request.MobileNumber.Trim(),
                Email = request.Email?.Trim(),
                Source = request.Source.Trim(),
                Project = request.Project.Trim(),
                AssignedUserId = request.AssignedUserId,
                Status = LeadStatus.New,
                CreatedAt = DateTime.UtcNow
            };

            _leadRepository.Add(lead);

            var history = new LeadStatusHistory
            {
                Id = Guid.NewGuid(),
                OrganizationId = lead.OrganizationId,
                LeadId = lead.Id,
                FromStatus = LeadStatus.New,
                ToStatus = LeadStatus.New,
                ChangedByUserId = _tenant.UserId!.Value,
                CreatedAt = DateTime.UtcNow
            };

            _leadStatusHistoryRepository.Add(history);

            await _leadRepository.SaveChangesAsync();

            var created = await _leadRepository.GetByIdAsync(lead.Id);

            return ServiceResult<LeadResponse>.Success(
                MapToResponse(created!),
                ServiceResultStatus.Created);
        }

        public async Task<ServiceResult<LeadResponse>> UpdateAsync(Guid id, UpdateLeadRequest request)
        {
            var lead =
                await _leadRepository.GetByIdAsync(id);

            if (lead is null)
            {
                return ServiceResult<LeadResponse>.Failure(
                    "Lead not found.",
                    ServiceResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.MobileNumber) ||
                string.IsNullOrWhiteSpace(request.Source) ||
                string.IsNullOrWhiteSpace(request.Project))
            {
                return ServiceResult<LeadResponse>.Failure(
                    "All required fields must be supplied.",
                    ServiceResultStatus.BadRequest);
            }

            var assignedUser =
                await _userRepository.GetByIdAsync(request.AssignedUserId);

            if (assignedUser is null)
            {
                return ServiceResult<LeadResponse>.Failure(
                    "Assigned user not found.",
                    ServiceResultStatus.BadRequest);
            }

            lead.Name = request.Name.Trim();
            lead.MobileNumber = request.MobileNumber.Trim();
            lead.Email = request.Email?.Trim();
            lead.Source = request.Source.Trim();
            lead.Project = request.Project.Trim();
            lead.AssignedUserId = request.AssignedUserId;

            await _leadRepository.SaveChangesAsync();

            var updated =
                await _leadRepository.GetByIdAsync(id);

            return ServiceResult<LeadResponse>.Success(
                MapToResponse(updated!));
        }

        public async Task<ServiceResult> ChangeStatusAsync(
            Guid id,
            ChangeLeadStatusRequest request)
        {
            var lead = await _leadRepository.GetByIdAsync(id);

            if (lead is null)
            {
                return ServiceResult.Failure(
                    "Lead not found.",
                    ServiceResultStatus.NotFound);
            }

            var previousStatus = lead.Status;

            lead.Status = request.Status;

            var history = new LeadStatusHistory
            {
                Id = Guid.NewGuid(),
                OrganizationId = lead.OrganizationId,
                LeadId = lead.Id,
                FromStatus = previousStatus,
                ToStatus = request.Status,
                ChangedByUserId = _tenant.UserId!.Value,
                CreatedAt = DateTime.UtcNow
            };

            _leadStatusHistoryRepository.Add(history);

            await _leadRepository.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(Guid id)
        {
            var lead =
                await _leadRepository.GetByIdAsync(id);

            if (lead is null)
            {
                return ServiceResult.Failure(
                    "Lead not found.",
                    ServiceResultStatus.NotFound);
            }

            _leadRepository.Delete(lead);

            await _leadRepository.SaveChangesAsync();

            return ServiceResult.Success();
        }

        private static LeadResponse MapToResponse(Lead lead)
        {
            return new LeadResponse
            {
                Id = lead.Id,
                Name = lead.Name,
                MobileNumber = lead.MobileNumber,
                Email = lead.Email,
                Source = lead.Source,
                Project = lead.Project,
                Status = lead.Status,
                AssignedUserId = lead.AssignedUserId,
                AssignedUserName = lead.AssignedUser.Name,
                CreatedAt = lead.CreatedAt
            };
        }
    }
}
