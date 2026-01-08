using Microsoft.EntityFrameworkCore;
using SubmiSoonProject.Data;
using SubmiSoonProject.Models;

namespace SubmiSoonProject.Repositories
{
    /// Repository for StudentEnrollment queries
    /// Handles enrollment-related data access
    public interface IStudentEnrollmentRepository
    {
        /// Gets all active enrolled class IDs for a student
        /// Used by: GetEnrolledClassesAsync
        Task<List<int>> GetActiveEnrolledClassIdsAsync(int studentId);

        /// Checks if a student is actively enrolled in a specific class (for authorization)
        /// Used by: Authorization checks in controllers/services (IsStudentEnrolledInClass)
        Task<bool> IsStudentEnrolledInClassAsync(int studentId, int classId);
    }

    public class StudentEnrollmentRepository : IStudentEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public StudentEnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetActiveEnrolledClassIdsAsync(int studentId)
        {
            return await _context.StudentEnrollments
                .Where(se => se.StudentId == studentId && 
                            se.Status == EnrollmentStatus.active)
                .Select(se => se.ClassId)
                .ToListAsync();
        }

        public async Task<bool> IsStudentEnrolledInClassAsync(int studentId, int classId)
        {
            return await _context.StudentEnrollments
                .AnyAsync(se => se.StudentId == studentId && 
                               se.ClassId == classId && 
                               se.Status == EnrollmentStatus.active);
        }
    }
}
