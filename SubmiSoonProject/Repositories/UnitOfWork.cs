using SubmiSoonProject.Data;

namespace SubmiSoonProject.Repositories
{
    /// Coordinates multiple repository operations in a single transaction -> created to write into database
    /// Service layer accesses all repositories through this interface
    /// Ensures all changes are committed or rolled back together
    public interface IUnitOfWork : IDisposable
    {
        // Repository properties - lazy-loaded on first access
        IAssessmentRepository Assessments { get; }
        IStudentEnrollmentRepository StudentEnrollments { get; }
        IUserAssessmentRepository UserAssessments { get; }
        IQuestionRepository Questions { get; }
        IAnswerRepository Answers { get; }

        /// Commits all changes to the database in a single transaction
        /// Returns the number of entities affected
        Task<int> CommitAsync();

        /// Rolls back all changes
        void Rollback();
    }

    /// Creates repositories on-demand (lazy-loading) and shares the same DbContext across all of them
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        
        // Lazy-loaded repositories (created only when accessed)
        private IAssessmentRepository? _assessments;
        private IStudentEnrollmentRepository? _studentEnrollments;
        private IUserAssessmentRepository? _userAssessments;
        private IQuestionRepository? _questions;
        private IAnswerRepository? _answers;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IAssessmentRepository Assessments
        {
            get
            {
                _assessments ??= new AssessmentRepository(_context);
                return _assessments;
            }
        }

        public IStudentEnrollmentRepository StudentEnrollments
        {
            get
            {
                _studentEnrollments ??= new StudentEnrollmentRepository(_context);
                return _studentEnrollments;
            }
        }

        public IUserAssessmentRepository UserAssessments
        {
            get
            {
                _userAssessments ??= new UserAssessmentRepository(_context);
                return _userAssessments;
            }
        }

        public IQuestionRepository Questions
        {
            get
            {
                _questions ??= new QuestionRepository(_context);
                return _questions;
            }
        }

        public IAnswerRepository Answers
        {
            get
            {
                _answers ??= new AnswerRepository(_context);
                return _answers;
            }
        }

        /// Commits all tracked changes to the database
        /// This is the ONLY place where SaveChangesAsync is called -> ensuring atomic transactions
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// Rolls back all changes by disposing the context without saving
        public void Rollback()
        {
            // discard all changes
            Dispose();
        }

        /// Disposes the DbContext
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
