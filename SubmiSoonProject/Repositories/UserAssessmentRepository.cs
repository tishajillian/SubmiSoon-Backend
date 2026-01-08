using Microsoft.EntityFrameworkCore;
using SubmiSoonProject.Data;
using SubmiSoonProject.Models;

namespace SubmiSoonProject.Repositories
{
    public interface IUserAssessmentRepository
    {
        /// Gets assessment IDs that are completed or on_review for a student
        /// Used by: GetIncompleteAssessmentsAsync
        Task<List<int>> GetCompletedOrReviewAssessmentIdsAsync(int userId);

        /// Gets UserAssessment with all answers, MCQ options, and files
        /// Used by: GetInocmpleteAssessmentDetailAsync, SaveDraftAsync, SubmitAssessmentAsync
        Task<UserAssessment?> GetUserAssessmentWithAnswersAsync(int userId, int assessmentId);

        /// Gets all completed UserAssessments for a student
        /// Used by: GetCompletedAssessmentsAsync
        Task<List<UserAssessment>> GetCompletedUserAssessmentsAsync(int userId);

        /// Gets completed UserAssessment with answers (for completed detail view)
        /// Used by: GetCompletedAssessmentDetailAsync
        Task<UserAssessment?> GetCompletedUserAssessmentWithAnswersAsync(int userId, int assessmentId);

        /// Adds a new UserAssessment to the context (tracked, not saved)
        /// Used by: SaveDraftAsync, SubmitAssessmentAsync
        void Add(UserAssessment userAssessment);

        /// Updates an existing UserAssessment (marks as modified)
        /// Used by: SaveDraftAsync, SubmitAssessmentAsync
        void Update(UserAssessment userAssessment);
    }

    public class UserAssessmentRepository : IUserAssessmentRepository
    {
        private readonly AppDbContext _context;

        public UserAssessmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<int>> GetCompletedOrReviewAssessmentIdsAsync(int userId)
        {
            return await _context.UserAssessments
                .Where(ua => ua.UserId == userId &&
                            (ua.Status == AssessmentStatus.completed ||
                             ua.Status == AssessmentStatus.on_review))
                .Select(ua => ua.AssessmentId)
                .ToListAsync();
        }

        public async Task<UserAssessment?> GetUserAssessmentWithAnswersAsync(int userId, int assessmentId)
        {
            return await _context.UserAssessments
                .Include(ua => ua.Answers)
                    .ThenInclude(a => a.McqOption)
                .Include(ua => ua.Answers)
                    .ThenInclude(a => a.File)
                .FirstOrDefaultAsync(ua => ua.UserId == userId && 
                                          ua.AssessmentId == assessmentId);
        }

        public async Task<List<UserAssessment>> GetCompletedUserAssessmentsAsync(int userId)
        {
            return await _context.UserAssessments
                .Where(ua => ua.UserId == userId && 
                            (ua.Status == AssessmentStatus.completed || 
                             ua.Status == AssessmentStatus.on_review))
                .ToListAsync();
        }

        public async Task<UserAssessment?> GetCompletedUserAssessmentWithAnswersAsync(
            int userId, 
            int assessmentId)
        {
            return await _context.UserAssessments
                .Include(ua => ua.Answers)
                    .ThenInclude(a => a.McqOption)
                .Include(ua => ua.Answers)
                    .ThenInclude(a => a.File)
                .FirstOrDefaultAsync(ua => ua.UserId == userId && 
                                          ua.AssessmentId == assessmentId && 
                                          (ua.Status == AssessmentStatus.completed || 
                                           ua.Status == AssessmentStatus.on_review));
        }
        public void Add(UserAssessment userAssessment)
        {
            _context.UserAssessments.Add(userAssessment);
        }

        public void Update(UserAssessment userAssessment)
        {
            _context.UserAssessments.Update(userAssessment);
        }
    }
}
