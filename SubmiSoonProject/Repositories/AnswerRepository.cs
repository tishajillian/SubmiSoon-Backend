using SubmiSoonProject.Data;
using SubmiSoonProject.Models;

namespace SubmiSoonProject.Repositories
{
    public interface IAnswerRepository
    {
        /// Adds a new Answer to the context (tracked, not saved)
        /// Used by: SaveDraftAsync, SubmitAssessmentAsync
        void Add(Answer answer);

        /// Updates an existing Answer (marks as modified)
        /// Used by: SaveDraftAsync, SubmitAssessmentAsync
        void Update(Answer answer);
    }

    public class AnswerRepository : IAnswerRepository
    {
        private readonly AppDbContext _context;

        public AnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Answer answer)
        {
            _context.Answers.Add(answer);
        }

        public void Update(Answer answer)
        {
            _context.Answers.Update(answer);
        }
    }
}
