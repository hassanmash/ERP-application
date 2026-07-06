namespace Application.DTOs.OrgAdmin.Teams
{
    public class TeamResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? LeadUserId { get; set; }
        public string? LeadUserName { get; set; }
        public int MemberCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
