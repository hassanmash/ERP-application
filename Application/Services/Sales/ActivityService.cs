using Application.Common;
using Application.DTOs.Sales;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Repositories.Contracts;

namespace Application.Services.Sales
{
    public class ActivityService(
        IActivityRepository activityRepository,
        ILeadRepository leadRepository,
        IUserRepository userRepository,
        ICurrentTenantService tenant)
        : IActivityService
    {
        private readonly IActivityRepository _activityRepository = activityRepository;
        private readonly ILeadRepository _leadRepository = leadRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICurrentTenantService _tenant = tenant;

        public async Task<ServiceResult<List<ActivityResponse>>> GetByLeadIdAsync(Guid leadId)
        {
            var lead = await _leadRepository.GetByIdAsync(leadId);

            if (lead is null)
            {
                return ServiceResult<List<ActivityResponse>>.Failure(
                    "Lead not found.",
                    ServiceResultStatus.NotFound);
            }

            var activities = await _activityRepository.GetByLeadIdAsync(leadId);

            return ServiceResult<List<ActivityResponse>>.Success(
                activities.Select(MapToResponse).ToList());
        }

        public async Task<ServiceResult<ActivityResponse>> GetByIdAsync(Guid id)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity is null)
            {
                return ServiceResult<ActivityResponse>.Failure(
                    "Activity not found.",
                    ServiceResultStatus.NotFound);
            }

            return ServiceResult<ActivityResponse>.Success(
                MapToResponse(activity));
        }

        public async Task<ServiceResult<ActivityResponse>> CreateAsync(CreateActivityRequest request)
        {
            var lead = await _leadRepository.GetByIdAsync(request.LeadId);

            if (lead is null)
            {
                return ServiceResult<ActivityResponse>.Failure(
                    "Lead not found.",
                    ServiceResultStatus.BadRequest);
            }

            var currentUser = await _userRepository.GetByIdAsync(_tenant.UserId!.Value);

            if (currentUser is null)
            {
                return ServiceResult<ActivityResponse>.Failure(
                    "Current user not found.",
                    ServiceResultStatus.BadRequest);
            }

            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                OrganizationId = lead.OrganizationId,
                LeadId = request.LeadId,
                Type = request.Type,
                Notes = request.Notes.Trim(),
                ActivityDateTime = request.ActivityDateTime,
                CreatedByUserId = currentUser.Id,
                CreatedAt = DateTime.UtcNow
            };

            _activityRepository.Add(activity);

            await _activityRepository.SaveChangesAsync();

            var created = await _activityRepository.GetByIdAsync(activity.Id);

            return ServiceResult<ActivityResponse>.Success(
                MapToResponse(created!),
                ServiceResultStatus.Created);
        }

        public async Task<ServiceResult<ActivityResponse>> UpdateAsync(Guid id, UpdateActivityRequest request)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity is null)
            {
                return ServiceResult<ActivityResponse>.Failure(
                    "Activity not found.",
                    ServiceResultStatus.NotFound);
            }

            activity.Type = request.Type;
            activity.Notes = request.Notes.Trim();
            activity.ActivityDateTime = request.ActivityDateTime;

            await _activityRepository.SaveChangesAsync();

            var updated = await _activityRepository.GetByIdAsync(id);

            return ServiceResult<ActivityResponse>.Success(
                MapToResponse(updated!));
        }

        public async Task<ServiceResult> DeleteAsync(Guid id)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity is null)
            {
                return ServiceResult.Failure(
                    "Activity not found.",
                    ServiceResultStatus.NotFound);
            }

            _activityRepository.Delete(activity);

            await _activityRepository.SaveChangesAsync();

            return ServiceResult.Success();
        }

        private static ActivityResponse MapToResponse(Activity activity)
        {
            return new ActivityResponse
            {
                Id = activity.Id,
                LeadId = activity.LeadId,
                Type = activity.Type,
                Notes = activity.Notes,
                ActivityDateTime = activity.ActivityDateTime,
                CreatedByUserId = activity.CreatedByUserId,
                CreatedByUserName = activity.CreatedByUser.Name,
                CreatedAt = activity.CreatedAt
            };
        }
    }
}
