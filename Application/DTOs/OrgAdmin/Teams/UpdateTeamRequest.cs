namespace Application.DTOs.OrgAdmin.Teams
{
    public class UpdateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public Guid? LeadUserId { get; set; }
    }
}
