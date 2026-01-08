using Microsoft.AspNetCore.Mvc;
using SubmiSoonProject.Data;
using SubmiSoonProject.DTOs.Common;
using SubmiSoonProject.Services;
using System.Security.Claims;

namespace SubmiSoonProject.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUrlSigningService _urlSigningService;
        private readonly AppDbContext _context;

        public FilesController(IFileService fileService, IUrlSigningService urlSigningService, AppDbContext context)
        {
            _fileService = fileService;
            _urlSigningService = urlSigningService;
            _context = context;
        }
        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetFile(
            int fileId, 
            [FromQuery] bool download = false,
            [FromQuery] string? token = null,
            [FromQuery] int? userId = null,
            [FromQuery] long? expires = null)
        {
            int currentUserId;

            // Check if signed token is provided (for direct browser access)
            if (!string.IsNullOrEmpty(token) && userId.HasValue && expires.HasValue)
            {
                // Validate signed token
                if (!_urlSigningService.ValidateFileAccessToken(fileId, userId.Value, expires.Value, token))
                {
                    // Check if token is expired for user-friendly message
                    if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expires.Value)
                    {
                        return StatusCode(401, new ApiErrorResponse
                        {
                            Success = false,
                            Error = new ErrorDetails
                            {
                                Code = "TOKEN_EXPIRED",
                                Message = "This download link has expired. Please return to the assessment page to generate a new link."
                            }
                        });
                    }

                    return StatusCode(401, new ApiErrorResponse
                    {
                        Success = false,
                        Error = new ErrorDetails
                        {
                            Code = "INVALID_TOKEN",
                            Message = "Invalid file access link. Please request a new link from the assessment page."
                        }
                    });
                }
                currentUserId = userId.Value;
            }
            else
            {
                // Fallback to Bearer token authentication
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    return StatusCode(401, new ApiErrorResponse
                    {
                        Success = false,
                        Error = new ErrorDetails
                        {
                            Code = "UNAUTHORIZED",
                            Message = "Authentication required. Please login or use a valid download link."
                        }
                    });
                }
                currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            }

            try
            {
                // Get file metadata from database
                var file = await _context.Files.FindAsync(fileId);

                if (file == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        Error = new ErrorDetails
                        {
                            Code = "FILE_NOT_FOUND",
                            Message = $"File with ID {fileId} not found"
                        }
                    });
                }

                // Access control: Only file owner can access
                if (file.UserId != currentUserId)
                {
                    return StatusCode(403, new ApiErrorResponse
                    {
                        Success = false,
                        Error = new ErrorDetails
                        {
                            Code = "ACCESS_DENIED",
                            Message = "You don't have permission to access this file"
                        }
                    });
                }

                // Verify file exists on disk
                if (!System.IO.File.Exists(file.FilePath))
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Success = false,
                        Error = new ErrorDetails
                        {
                            Code = "FILE_NOT_FOUND",
                            Message = "File not found on server"
                        }
                    });
                }

                // Determine content type based on extension
                var contentType = GetContentType(file.FileExtension);

                // Determine content disposition
                // For PDFs and images: default to inline (preview in browser)
                // For documents: default to attachment (force download)
                // If download=true query param is set, always force download
                var contentDisposition = download || ShouldForceDownload(file.FileExtension)
                    ? "attachment"
                    : "inline";

                // Set Content-Disposition header manually to support inline preview
                // Note: Passing fileDownloadName to PhysicalFile always sets "attachment"
                Response.Headers["Content-Disposition"] = $"{contentDisposition}; filename=\"{file.OriginalFilename}\"";

                // Return file without fileDownloadName parameter to just use Content-Disposition header
                return PhysicalFile(
                    file.FilePath,
                    contentType,
                    enableRangeProcessing: true
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "INTERNAL_ERROR",
                        Message = "An error occurred while retrieving the file"
                    }
                });
            }
        }

        /// Get MIME type based on file extension
        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                "pdf" => "application/pdf",
                "jpg" => "image/jpeg",
                "jpeg" => "image/jpeg",
                "png" => "image/png",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }


        /// Determine if file type should force download by default
        /// PDFs and images can preview "inline", documents should "download"
        private bool ShouldForceDownload(string extension)
        {
            var previewableExtensions = new[] { "pdf", "jpg", "jpeg", "png" };
            return !previewableExtensions.Contains(extension.ToLower());
        }
    }
}
