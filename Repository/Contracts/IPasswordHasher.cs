namespace Infrastructure.Contracts
{
    public interface IPasswordHasher
    {
        string Hash(string plainTextPassword);
        bool Verify(string plainTextPassword, string hash);
    }
}
