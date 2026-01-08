using Microsoft.EntityFrameworkCore;
using SubmiSoonProject.Data;
using SubmiSoonProject.Models;

namespace SubmiSoonProject.Repositories
{
    public interface IAssessmentRepository
    {
        /// Gets incomplete assessments filtered by enrolled classes and excluded assessment IDs
        /// Filters in the database for better performance
        /// Used by: GetIncompleteAssessmentsAsync
        Task<List<Assessment>> GetIncompleteAssessmentsByClassIdsAsync(
            List<int> classIds,
            List<int> excludedAssessmentIds,
            DateTime afterEndDate,
            int? academicTermId = null);

        /// Gets a single assessment with its class navigation property
        /// Used by: GetInocmpleteAssessmentDetailAsync, GetCompletedAssessmentDetailAsync
        Task<Assessment?> GetAssessmentWithClassAsync(int assessmentId);

        /// Gets completed assessments with full details (class, lecturer, user, course, term)
        /// Filters by enrolled classes and included assessment IDs
        /// Used by: GetCompletedAssessmentsAsync
        Task<List<Assessment>> GetCompletedAssessmentsWithDetailsAsync(
            List<int> classIds,
            List<int> includedAssessmentIds,
            int? academicTermId = null);
    }

    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly AppDbContext _context;

        public AssessmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Assessment>> GetIncompleteAssessmentsByClassIdsAsync(
            List<int> classIds,
            List<int> excludedAssessmentIds,
            DateTime afterEndDate,
            int? academicTermId = null)
        {
            var query = _context.Assessments
                .Include(a => a.Class)
                    .ThenInclude(c => c.Lecturer)
                        .ThenInclude(l => l.User)
                .Where(a => classIds.Contains(a.ClassId))
                .Where(a => !excludedAssessmentIds.Contains(a.AssessmentId))
                .Where(a => a.EndDate >= afterEndDate);

            // Apply semester filter if provided
            if (academicTermId.HasValue)
            {
                query = query.Where(a => a.Class.AcademicTermId == academicTermId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Assessment?> GetAssessmentWithClassAsync(int assessmentId)
        {
            return await _context.Assessments
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.AssessmentId == assessmentId);
        }

        public async Task<List<Assessment>> GetCompletedAssessmentsWithDetailsAsync(
            List<int> classIds,
            List<int> includedAssessmentIds,
            int? academicTermId = null)
        {
            var query = _context.Assessments
                .Include(a => a.Class)
                    .ThenInclude(c => c.Lecturer)
                        .ThenInclude(l => l.User)
                .Include(a => a.Class.Course)
                .Include(a => a.Class.AcademicTerm)
                .Where(a => classIds.Contains(a.ClassId))
                .Where(a => includedAssessmentIds.Contains(a.AssessmentId));

            // Apply semester filter if provided
            if (academicTermId.HasValue)
            {
                query = query.Where(a => a.Class.AcademicTermId == academicTermId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
