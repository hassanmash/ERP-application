namespace Application.DTOs.Dashboard
{
    public class DashboardSummaryResponse
    {
        public int TotalLeads { get; set; }

        public int NewLeads { get; set; }

        public int QualifiedLeads { get; set; }

        public int WonLeads { get; set; }

        public int LostLeads { get; set; }

        public int TodaysActivities { get; set; }

        public int OverdueActivities { get; set; }
    }
}
