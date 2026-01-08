using Microsoft.EntityFrameworkCore;
using SubmiSoonProject.Data;
using SubmiSoonProject.Exceptions;
using SubmiSoonProject.Models;

namespace SubmiSoonProject.Services
{
    public interface IFileService
    {
        Task ValidateFileAsync(IFormFile file, int questionId);
        Task<FileEntity> SaveFileAsync(IFormFile file, int userId, int assessmentId);
        Task<string> GetFilePathAsync(int fileId);
        Task DeleteFileAsync(int fileId);
    }

    public class FileService : IFileService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 2097152; // 2MB in bytes
        private static readonly List<string> AllowedExtensions = new() { ".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png" };

        public FileService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task ValidateFileAsync(IFormFile file, int questionId)
        {
            // Check file size
            if (file.Length > MaxFileSize)
            {
                throw new FileTooLargeException(
                    $"File size for question {questionId} exceeds the limit",
                    questionId,
                    file.Length,
                    MaxFileSize
                );
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidFileTypeException(
                    $"File type for question {questionId} is not allowed",
                    questionId,
                    extension,
                    AllowedExtensions
                );
            }

            await Task.CompletedTask;
        }

        public async Task<FileEntity> SaveFileAsync(IFormFile file, int userId, int assessmentId)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storedFilename = $"{Guid.NewGuid()}_{DateTime.Now.Ticks}{extension}";
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", assessmentId.ToString(), userId.ToString());

            // Create directory if it doesn't exist
            Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, storedFilename);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create file entity
            var fileEntity = new FileEntity
            {
                UserId = userId,
                AssessmentId = assessmentId,
                OriginalFilename = file.FileName,
                StoredFilename = storedFilename,
                FilePath = filePath,
                FileExtension = extension.TrimStart('.'),
                FileSize = (int)file.Length,
                CreatedAt = DateTime.Now
            };

            _context.Files.Add(fileEntity);
            await _context.SaveChangesAsync();

            return fileEntity;
        }

        public async Task<string> GetFilePathAsync(int fileId)
        {
            var file = await _context.Files.FindAsync(fileId);
            if (file == null)
            {
                throw new NotFoundException($"File with ID {fileId} not found");
            }

            return file.FilePath;
        }

        public async Task DeleteFileAsync(int fileId)
        {
            var file = await _context.Files.FindAsync(fileId);
            if (file != null)
            {
                // Delete from disk
                if (File.Exists(file.FilePath))
                {
                    File.Delete(file.FilePath);
                }

                // Delete from database
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();
            }
        }
    }
}
