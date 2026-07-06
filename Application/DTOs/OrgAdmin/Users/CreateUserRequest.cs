namespace Application.DTOs.OrgAdmin.Users
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Guid? TeamId { get; set; }
        public Guid? RoleId { get; set; }
    }
}
