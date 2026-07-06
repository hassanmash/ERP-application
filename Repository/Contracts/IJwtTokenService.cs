using Domain.Entities;

namespace Infrastructure.Contracts
{
    public interface IJwtTokenService
    {
        /// <summary>
        /// Issues a JWT for the given user. The claim set differs based on
        /// IsPlatformAdmin: platform admins get no org_id claim at all (absence,
        /// not an empty string), org users get org_id + role_id. The downstream
        /// tenant-resolution middleware branches on whether org_id is present.
        /// </summary>
        string GenerateToken(User user);
    }
}
