namespace Application.DTOs.OrgAdmin.Teams
{
    public class CreateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public Guid? LeadUserId { get; set; }
    }
}
