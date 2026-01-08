namespace SubmiSoonProject.DTOs.Leaderboard
{
    public class LeaderboardEntryDto
    {
        public string Name { get; set; } = null!;
        public int TotalAssessmentsDone { get; set; }
        public int TotalAssessmentsRemaining { get; set; }
    }
}
