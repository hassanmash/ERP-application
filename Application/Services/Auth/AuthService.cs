using Application.Common;
using Application.DTOs.Auth;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Repositories.Contracts;

namespace Application.Services.Auth
{
    public class AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService tokenService,
        IMapper mapper) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtTokenService _tokenService = tokenService;
        private readonly IMapper _mapper = mapper;

        public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return ServiceResult<LoginResponse>.Failure(
                    "Email and password are required.", ServiceResultStatus.BadRequest);
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailIgnoringTenantAsync(normalizedEmail);

            // Same message for "no such user" and "wrong password" — never
            // reveal which one, to avoid leaking which emails are registered.
            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return ServiceResult<LoginResponse>.Failure(
                    "Invalid email or password.", ServiceResultStatus.Unauthorized);
            }

            if (!user.IsActive)
            {
                return ServiceResult<LoginResponse>.Failure(
                    "This account has been deactivated.", ServiceResultStatus.Unauthorized);
            }

            if (user.Organization is not null && user.Organization.Status == OrganizationStatus.Suspended)
            {
                return ServiceResult<LoginResponse>.Failure(
                    "This organization has been suspended.", ServiceResultStatus.Unauthorized);
            }

            var token = _tokenService.GenerateToken(user);

            var response = _mapper.Map<LoginResponse>(user);
            response.Token = token; // set explicitly — not part of the entity, ignored in the mapping profile

            return ServiceResult<LoginResponse>.Success(response);
        }
    }
}
