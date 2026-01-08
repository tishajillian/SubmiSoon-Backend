using Microsoft.EntityFrameworkCore;
using SubmiSoonProject.Data;
using SubmiSoonProject.Models;

namespace SubmiSoonProject.Repositories
{
    public interface IQuestionRepository
    {
        /// Gets all questions with MCQ options for an assessment
        /// Used by: GetInocmpleteAssessmentDetailAsync, GetCompletedAssessmentDetailAsync
        Task<List<Question>> GetQuestionsWithOptionsAsync(int assessmentId);
    }

    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetQuestionsWithOptionsAsync(int assessmentId)
        {
            return await _context.Questions
                .Include(q => q.McqOptions)
                .Where(q => q.AssessmentId == assessmentId)
                .OrderBy(q => q.QuestionId)
                .ToListAsync();
        }
    }
}
