using Infrastructure.Contracts;

namespace Infrastructure.Hashers
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        // Work factor 12 is a reasonable default as of 2026 — high enough to be
        // slow for brute force, low enough not to noticeably delay login.
        // Bump this over time as hardware gets faster; BCrypt hashes embed the
        // work factor used, so old hashes keep validating correctly even after
        // you raise this for new ones.
        private const int WorkFactor = 12;

        public string Hash(string plainTextPassword)
            => BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: WorkFactor);

        public bool Verify(string plainTextPassword, string hash)
            => BCrypt.Net.BCrypt.Verify(plainTextPassword, hash);
    }
}
