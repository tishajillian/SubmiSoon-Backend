using Microsoft.EntityFrameworkCore;
using SubmiSoonProject.Models;

namespace SubmiSoonProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Faculty> Faculties => Set<Faculty>();
        public DbSet<ProgramStudy> ProgramStudies => Set<ProgramStudy>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Lecturer> Lecturers => Set<Lecturer>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<AcademicTerm> AcademicTerms => Set<AcademicTerm>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Class> Classes => Set<Class>();
        public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();
        public DbSet<Assessment> Assessments => Set<Assessment>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<McqOption> McqOptions => Set<McqOption>();
        public DbSet<UserAssessment> UserAssessments => Set<UserAssessment>();
        public DbSet<Answer> Answers => Set<Answer>();
        public DbSet<FileEntity> Files => Set<FileEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Name).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).IsRequired();
            });

            // Faculty Configuration
            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.HasKey(f => f.FacultyId);
                entity.HasIndex(f => f.Name).IsUnique();
                entity.Property(f => f.Name).IsRequired();
            });

            // ProgramStudy Configuration
            modelBuilder.Entity<ProgramStudy>(entity =>
            {
                entity.HasKey(ps => ps.ProgramStudyId);
                entity.HasIndex(ps => ps.Name).IsUnique();
                entity.Property(ps => ps.Name).IsRequired();
                entity.HasOne(ps => ps.Faculty)
                      .WithMany()
                      .HasForeignKey(ps => ps.FacultyId);
            });

            // Student Configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.UserId);
                entity.HasIndex(s => s.StudentId).IsUnique();
                entity.Property(s => s.StudentId).IsRequired();
                entity.HasOne(s => s.User)
                      .WithOne()
                      .HasForeignKey<Student>(s => s.UserId);
                entity.HasOne(s => s.ProgramStudy)
                      .WithMany()
                      .HasForeignKey(s => s.ProgramStudyId);
            });

            // Lecturer Configuration
            modelBuilder.Entity<Lecturer>(entity =>
            {
                entity.HasKey(l => l.UserId);
                entity.HasIndex(l => l.LecturerId).IsUnique();
                entity.Property(l => l.LecturerId).IsRequired();
                entity.HasOne(l => l.User)
                      .WithOne()
                      .HasForeignKey<Lecturer>(l => l.UserId);
                entity.HasOne(l => l.ProgramStudy)
                      .WithMany()
                      .HasForeignKey(l => l.ProgramStudyId);
            });

            // RefreshToken Configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.RefreshTokenId);
                entity.Property(rt => rt.TokenHash).IsRequired();
                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId);
            });

            // AcademicTerm Configuration
            modelBuilder.Entity<AcademicTerm>(entity =>
            {
                entity.HasKey(at => at.AcademicTermId);
                entity.HasIndex(at => new { at.Year, at.Semester }).IsUnique();
            });

            // Course Configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.CourseId);
                entity.HasIndex(c => c.Name).IsUnique();
                entity.Property(c => c.Name).IsRequired();
                entity.Property(c => c.CourseCode).IsRequired();
            });

            // Class Configuration
            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasKey(c => c.ClassId);
                entity.Property(c => c.ClassCode).IsRequired();
                entity.HasOne(c => c.Course)
                      .WithMany()
                      .HasForeignKey(c => c.CourseId);
                entity.HasOne(c => c.Lecturer)
                      .WithMany()
                      .HasForeignKey(c => c.LecturerId);
                entity.HasOne(c => c.AcademicTerm)
                      .WithMany()
                      .HasForeignKey(c => c.AcademicTermId);
            });

            // StudentEnrollment Configuration
            modelBuilder.Entity<StudentEnrollment>(entity =>
            {
                entity.HasKey(se => se.StudentEnrollmentId);
                entity.HasIndex(se => new { se.StudentId, se.ClassId }).IsUnique();
                entity.HasOne(se => se.Student)
                      .WithMany()
                      .HasForeignKey(se => se.StudentId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(se => se.Class)
                      .WithMany()
                      .HasForeignKey(se => se.ClassId);
            });

            // Assessment Configuration
            modelBuilder.Entity<Assessment>(entity =>
            {
                entity.HasKey(a => a.AssessmentId);
                entity.Property(a => a.Title).IsRequired();
                entity.HasOne(a => a.Class)
                      .WithMany()
                      .HasForeignKey(a => a.ClassId);
            });

            // Question Configuration
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(q => q.QuestionId);
                entity.Property(q => q.Content).IsRequired();
                entity.HasOne(q => q.Assessment)
                      .WithMany()
                      .HasForeignKey(q => q.AssessmentId);
                entity.HasMany(q => q.McqOptions)
                      .WithOne(mo => mo.Question)
                      .HasForeignKey(mo => mo.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // McqOption Configuration
            modelBuilder.Entity<McqOption>(entity =>
            {
                entity.HasKey(mo => mo.OptionId);
                entity.HasIndex(mo => new { mo.QuestionId, mo.OptionLabel }).IsUnique();
                entity.Property(mo => mo.OptionText).IsRequired();
                // Relationship already configured from Question side
            });

            // UserAssessment Configuration
            modelBuilder.Entity<UserAssessment>(entity =>
            {
                entity.HasKey(ua => ua.UserAssessmentId);
                entity.HasOne(ua => ua.User)
                      .WithMany()
                      .HasForeignKey(ua => ua.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(ua => ua.Assessment)
                      .WithMany()
                      .HasForeignKey(ua => ua.AssessmentId);
                entity.HasMany(ua => ua.Answers)
                      .WithOne(a => a.UserAssessment)
                      .HasForeignKey(a => a.UserAssessmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Answer Configuration
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(a => a.AnswerId);
                entity.HasIndex(a => new { a.UserAssessmentId, a.QuestionId }).IsUnique();
                // UserAssessment relationship configured from UserAssessment side
                entity.HasOne(a => a.Question)
                      .WithMany()
                      .HasForeignKey(a => a.QuestionId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(a => a.McqOption)
                      .WithMany()
                      .HasForeignKey(a => a.SelectedOptionId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(a => a.File)
                      .WithMany()
                      .HasForeignKey(a => a.FileId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // FileEntity Configuration
            modelBuilder.Entity<FileEntity>(entity =>
            {
                entity.HasKey(f => f.FileId);
                entity.Property(f => f.OriginalFilename).IsRequired();
                entity.Property(f => f.StoredFilename).IsRequired();
                entity.Property(f => f.FilePath).IsRequired();
                entity.Property(f => f.FileExtension).IsRequired();
                entity.HasOne(f => f.User)
                      .WithMany()
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(f => f.Assessment)
                      .WithMany()
                      .HasForeignKey(f => f.AssessmentId);
            });
        }
    }
}
