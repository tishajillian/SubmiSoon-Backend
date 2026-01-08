using Microsoft.EntityFrameworkCore;
using SubmiSoonProject.Data;
using SubmiSoonProject.DTOs.Leaderboard;
using SubmiSoonProject.Models;
using System.Threading.Tasks;

namespace SubmiSoonProject.Repositories
{
    public interface IStudentRepository
    {
        Task<List<LeaderboardEntryDto>> GetStudentsLeaderboardAsync(); // should've return Model Entity type of data, but i let it be since this is aggregations
    }
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;
        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LeaderboardEntryDto>> GetStudentsLeaderboardAsync()
        {
            return await _context.Students
                .Include(s => s.User)  // use navigation prop
                .Select(s => new LeaderboardEntryDto
                {
                    Name = s.User.Name,  // direct via navigation props
                    TotalAssessmentsDone = _context.UserAssessments
                        .Count(ua => ua.UserId == s.UserId && ua.Status == AssessmentStatus.completed),  // subquery aggregation
                    TotalAssessmentsRemaining = _context.StudentEnrollments
                        .Where(se => se.StudentId == s.UserId && se.Status == EnrollmentStatus.active)
                        .SelectMany(se => _context.Assessments.Where(a => a.ClassId == se.ClassId))
                        .Distinct()
                        .Count()  // subquery aggregation
                })
                .ToListAsync();
        }
    }
}
