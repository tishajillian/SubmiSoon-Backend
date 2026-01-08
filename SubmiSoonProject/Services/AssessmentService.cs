using Microsoft.EntityFrameworkCore;
using SubmiSoonProject.Data;
using SubmiSoonProject.DTOs.Assessment;
using SubmiSoonProject.DTOs.Common;
using SubmiSoonProject.Exceptions;
using SubmiSoonProject.Models;
using SubmiSoonProject.Repositories;

namespace SubmiSoonProject.Services
{
    public interface IAssessmentService
    {
        Task<ApiResponse<List<IncompleteAssessmentDto>>> GetIncompleteAssessmentsAsync(int userId, int page, int size, string? semester);
        Task<AssessmentDetailDto> GetIncompleteAssessmentDetailAsync(int assessmentId, int userId);
        Task<SaveDraftResponse> SaveDraftAsync(int assessmentId, int userId, SaveDraftRequest request);
        Task<SubmitAssessmentResponse> SubmitAssessmentAsync(int assessmentId, int userId, SaveDraftRequest request);
        Task<ApiResponse<List<CompletedAssessmentDto>>> GetCompletedAssessmentsAsync(int userId, int page, int size, string? semester);
        Task<AssessmentDetailDto> GetCompletedAssessmentDetailAsync(int assessmentId, int userId);
    }
    public class AssessmentService : IAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlSigningService _urlSigningService;

        public AssessmentService(IUnitOfWork unitOfWork, IFileService fileService, IHttpContextAccessor httpContextAccessor, IUrlSigningService urlSigningService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
            _urlSigningService = urlSigningService;
        }

        /// Helper method to generate signed file URLs for download and preview
        /// URLs are valid for 3 minutes and include cryptographic signature
        private (string downloadUrl, string previewUrl) GenerateFileUrls(int fileId, int userId)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                // Fallback if HttpContext is not available
                return ($"/api/files/{fileId}?download=true", $"/api/files/{fileId}");
            }

            // Generate signed token (valid for 3 minutes)
            var (token, expires) = _urlSigningService.GenerateFileAccessToken(fileId, userId, validMinutes: 3);

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var downloadUrl = $"{baseUrl}/api/files/{fileId}?token={Uri.EscapeDataString(token)}&userId={userId}&expires={expires}&download=true";
            var previewUrl = $"{baseUrl}/api/files/{fileId}?token={Uri.EscapeDataString(token)}&userId={userId}&expires={expires}";

            return (downloadUrl, previewUrl);
        }

        public async Task<ApiResponse<List<IncompleteAssessmentDto>>> GetIncompleteAssessmentsAsync(
            int userId, int page, int size, string? semester)
        {
            // Use repository: Get enrolled class IDs
            var enrolledClassIds = await _unitOfWork.StudentEnrollments
                .GetActiveEnrolledClassIdsAsync(userId);

            if (!enrolledClassIds.Any())
            {
                return new ApiResponse<List<IncompleteAssessmentDto>>
                {
                    Data = new List<IncompleteAssessmentDto>(),
                    Paging = new PagingDto { Page = page, Size = size, TotalItem = 0, TotalPage = 0 },
                    Success = true
                };
            }

            // Use repository: Get completed/on_review assessment IDs
            var completedAssessmentIds = await _unitOfWork.UserAssessments
                .GetCompletedOrReviewAssessmentIdsAsync(userId);

            // Parse semester filter
            int? semesterId = null;
            if (!string.IsNullOrEmpty(semester) && int.TryParse(semester, out int parsedSemester))
            {
                semesterId = parsedSemester;
            }

            // Use repository: Get assessments with database filtering
            var assessments = await _unitOfWork.Assessments
                .GetIncompleteAssessmentsByClassIdsAsync(
                    enrolledClassIds,
                    completedAssessmentIds,
                    afterEndDate: DateTime.Now,
                    academicTermId: semesterId);

            // Pagination and mapping in-memory
            var totalItem = assessments.Count;
            var totalPage = (int)Math.Ceiling(totalItem / (double)size);

            var result = assessments
                .OrderBy(a => a.EndDate)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(a => new IncompleteAssessmentDto
                {
                    Id = a.AssessmentId,
                    Name = a.Title,
                    Class = a.Class.ClassCode,
                    LecturerName = a.Class.Lecturer.User.Name,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate
                })
                .ToList();

            return new ApiResponse<List<IncompleteAssessmentDto>>
            {
                Data = result,
                Paging = new PagingDto
                {
                    Page = page,
                    Size = size,
                    TotalItem = totalItem,
                    TotalPage = totalPage
                },
                Success = true
            };
        }

        public async Task<AssessmentDetailDto> GetIncompleteAssessmentDetailAsync(int assessmentId, int userId)
        {
            // Use repository: Get assessment with class info
            var assessment = await _unitOfWork.Assessments
                .GetAssessmentWithClassAsync(assessmentId);

            if (assessment == null)
            {
                throw new NotFoundException($"Assessment with ID {assessmentId} not found");
            }

            // Check if assessment expired
            if (assessment.EndDate < DateTime.Now)
            {
                throw new AssessmentExpiredException(
                    "The deadline for this assessment has passed",
                    assessment.EndDate
                );
            }

            // Use repository: Check if student is enrolled
            var isEnrolled = await _unitOfWork.StudentEnrollments
                .IsStudentEnrolledInClassAsync(userId, assessment.ClassId);

            if (!isEnrolled)
            {
                throw new ForbiddenException("You don't have permission to access this assessment");
            }

            // Use repository: Get existing user assessment (if any)
            var userAssessment = await _unitOfWork.UserAssessments
                .GetUserAssessmentWithAnswersAsync(userId, assessmentId);

            // Use repository: Get all questions with options
            var questions = await _unitOfWork.Questions
                .GetQuestionsWithOptionsAsync(assessmentId);

            // Map to DTOs
            var questionDtos = questions.Select(q =>
            {
                var existingAnswer = userAssessment?.Answers.FirstOrDefault(a => a.QuestionId == q.QuestionId);

                AnswerDto? answerDto = null;
                if (existingAnswer != null)
                {
                    answerDto = new AnswerDto();

                    if (q.Type == QuestionType.essay)
                    {
                        answerDto.Text = existingAnswer.AnswerText;
                    }
                    else if (q.Type == QuestionType.mcq && existingAnswer.McqOption != null)
                    {
                        answerDto.Mcq = new McqSelectionDto
                        {
                            OptionId = existingAnswer.McqOption.OptionId,
                            Label = existingAnswer.McqOption.OptionLabel
                        };
                    }
                    else if (q.Type == QuestionType.file && existingAnswer.File != null)
                    {
                        var (downloadUrl, previewUrl) = GenerateFileUrls(existingAnswer.File.FileId, userId);
                        answerDto.File = new FileInfoDto
                        {
                            FileId = existingAnswer.File.FileId,
                            Filename = existingAnswer.File.OriginalFilename,
                            Extension = existingAnswer.File.FileExtension,
                            Size = existingAnswer.File.FileSize,
                            DownloadUrl = downloadUrl,
                            PreviewUrl = previewUrl
                        };
                    }
                }

                return new QuestionDto
                {
                    Id = q.QuestionId,
                    Question = q.Content,
                    AnswerType = q.Type.ToString().ToLower(),
                    Answer = answerDto,
                    Options = q.Type == QuestionType.mcq
                        ? q.McqOptions.Select(o => new McqOptionDto
                        {
                            OptionId = o.OptionId,
                            Label = o.OptionLabel,
                            Text = o.OptionText
                        }).ToList()
                        : null
                };
            }).ToList();

            return new AssessmentDetailDto
            {
                Assessment = new AssessmentInfoDto
                {
                    Id = assessment.AssessmentId,
                    Title = assessment.Title,
                    Status = userAssessment?.Status.ToString().ToLower(),
                    Score = userAssessment?.Score,
                    UpdatedAt = userAssessment?.UpdatedAt
                },
                Questions = questionDtos
            };
        }

        public async Task<SaveDraftResponse> SaveDraftAsync(int assessmentId, int userId, SaveDraftRequest request)
        {
            // Use repository: Get assessment with class info
            var assessment = await _unitOfWork.Assessments
                .GetAssessmentWithClassAsync(assessmentId);

            if (assessment == null)
            {
                throw new NotFoundException($"Assessment with ID {assessmentId} not found");
            }

            // Check if assessment expired
            if (assessment.EndDate < DateTime.Now)
            {
                throw new AssessmentExpiredException(
                    "The deadline for this assessment has passed",
                    assessment.EndDate
                );
            }

            // Use repository: Check if student is enrolled
            var isEnrolled = await _unitOfWork.StudentEnrollments
                .IsStudentEnrolledInClassAsync(userId, assessment.ClassId);

            if (!isEnrolled)
            {
                throw new ForbiddenException("You don't have permission to access this assessment");
            }

            // Use repository: Get or create UserAssessment
            var userAssessment = await _unitOfWork.UserAssessments
                .GetUserAssessmentWithAnswersAsync(userId, assessmentId);

            if (userAssessment == null)
            {
                userAssessment = new UserAssessment
                {
                    UserId = userId,
                    AssessmentId = assessmentId,
                    Status = AssessmentStatus.draft,
                    CreatedAt = DateTime.Now
                };
                // Use repository: Add (tracked, not saved yet)
                _unitOfWork.UserAssessments.Add(userAssessment);
                // Commit through UnitOfWork
                await _unitOfWork.CommitAsync();
            }

            // Check if assessment was already submitted
            if (userAssessment.Status == AssessmentStatus.completed || userAssessment.Status == AssessmentStatus.on_review)
            {
                throw new AlreadySubmittedException(
                    "This assessment has already been submitted and cannot be modified",
                    userAssessment.UpdatedAt ?? userAssessment.CreatedAt,
                    userAssessment.Status.ToString().ToLower()
                );
            }

            // Use repository: Get all questions for validation
            var questions = await _unitOfWork.Questions
                .GetQuestionsWithOptionsAsync(assessmentId);

            // Track uploaded files for response
            var uploadedFiles = new List<UploadedFileDto>();

            // Process each answer
            foreach (var answerInput in request.Answers)
            {
                var question = questions.FirstOrDefault(q => q.QuestionId == answerInput.QuestionId);
                if (question == null)
                {
                    throw new NotFoundException($"Question with ID {answerInput.QuestionId} not found");
                }

                // Validate answer type matches question type
                var expectedType = question.Type.ToString().ToLower();
                if (answerInput.AnswerType != expectedType)
                {
                    throw new AnswerTypeMismatchException(
                        $"Answer type for question {question.QuestionId} does not match the question type",
                        answerInput.QuestionId,
                        expectedType,
                        answerInput.AnswerType
                    );
                }

                // Get or create answer
                var answer = userAssessment.Answers.FirstOrDefault(a => a.QuestionId == answerInput.QuestionId);
                if (answer == null)
                {
                    answer = new Answer
                    {
                        UserAssessmentId = userAssessment.UserAssessmentId,
                        QuestionId = answerInput.QuestionId,
                        CreatedAt = DateTime.Now
                    };
                    // Use repository: Add answer
                    _unitOfWork.Answers.Add(answer);
                }

                // Process based on answer type
                switch (question.Type)
                {
                    case QuestionType.essay:
                        if (string.IsNullOrWhiteSpace(answerInput.Text))
                        {
                            throw new EmptyAnswerException(
                                $"Essay answer for question {question.QuestionId} cannot be empty",
                                answerInput.QuestionId,
                                "essay"
                            );
                        }
                        // else: No new text but existing text exists - keep it unchanged
                        break;

                    case QuestionType.mcq:
                        // Check if new option is provided
                        if (answerInput.OptionId != null)
                        {
                            // New/updated option provided - validate it
                            var option = question.McqOptions.FirstOrDefault(o => o.OptionId == answerInput.OptionId);
                            if (option == null)
                            {
                                var validOptionIds = question.McqOptions.Select(o => o.OptionId).ToList();
                                throw new InvalidOptionException(
                                    $"Option {answerInput.OptionId} does not belong to question {question.QuestionId}",
                                    answerInput.QuestionId,
                                    answerInput.OptionId.Value,
                                    validOptionIds
                                );
                            }
                            answer.SelectedOptionId = answerInput.OptionId;
                            answer.UpdatedAt = DateTime.Now;
                        }
                        else if (answer.SelectedOptionId == null)
                        {
                            // No new option AND no existing option - validation error
                            throw new MissingAnswerDataException(
                                $"Option ID is required for MCQ question {question.QuestionId}",
                                answerInput.QuestionId,
                                "mcq",
                                "option_id"
                            );
                        }
                        // else: No new option but existing option exists - keep it unchanged
                        break;

                    case QuestionType.file:
                        if (answerInput.File == null)
                        {
                            throw new MissingAnswerDataException(
                                $"File is required for file upload question {question.QuestionId}",
                                answerInput.QuestionId,
                                "file",
                                "file"
                            );
                        }

                        // Validate file
                        await _fileService.ValidateFileAsync(answerInput.File, question.QuestionId);

                        // Delete old file if exists
                        if (answer.FileId != null)
                        {
                            await _fileService.DeleteFileAsync(answer.FileId.Value);
                        }

                        // Save new file
                        var fileEntity = await _fileService.SaveFileAsync(answerInput.File, userId, assessmentId);
                        answer.FileId = fileEntity.FileId;
                        answer.UpdatedAt = DateTime.Now;

                        // Add to uploaded files list for response
                        var (downloadUrl, previewUrl) = GenerateFileUrls(fileEntity.FileId, userId);
                        uploadedFiles.Add(new UploadedFileDto
                        {
                            QuestionId = question.QuestionId,
                            FileId = fileEntity.FileId,
                            Filename = fileEntity.OriginalFilename,
                            Size = fileEntity.FileSize,
                            DownloadUrl = downloadUrl,
                            PreviewUrl = previewUrl
                        });
                        break;
                }
            }

            // Update user assessment timestamp
            userAssessment.UpdatedAt = DateTime.Now;
            userAssessment.Status = AssessmentStatus.draft;

            // Commit all changes through UnitOfWork (one transaction)
            await _unitOfWork.CommitAsync();

            // Return response with uploaded files info
            return new SaveDraftResponse
            {
                AssessmentId = assessmentId,
                Status = "draft",
                UpdatedAt = userAssessment.UpdatedAt.Value,
                SavedAnswers = request.Answers.Count,
                UploadedFiles = uploadedFiles
            };
        }

        public async Task<SubmitAssessmentResponse> SubmitAssessmentAsync(int assessmentId, int userId, SaveDraftRequest request)
        {
            // Use repository: Get assessment with class info
            var assessment = await _unitOfWork.Assessments
                .GetAssessmentWithClassAsync(assessmentId);

            if (assessment == null)
            {
                throw new NotFoundException($"Assessment with ID {assessmentId} not found");
            }

            // Check if assessment expired
            if (assessment.EndDate < DateTime.Now)
            {
                throw new AssessmentExpiredException(
                    "The deadline for this assessment has passed",
                    assessment.EndDate
                );
            }

            // Use repository: Check if student is enrolled
            var isEnrolled = await _unitOfWork.StudentEnrollments
                .IsStudentEnrolledInClassAsync(userId, assessment.ClassId);

            if (!isEnrolled)
            {
                throw new ForbiddenException("You don't have permission to access this assessment");
            }

            // Use repository: Get or create UserAssessment
            var userAssessment = await _unitOfWork.UserAssessments
                .GetUserAssessmentWithAnswersAsync(userId, assessmentId);

            if (userAssessment == null)
            {
                userAssessment = new UserAssessment
                {
                    UserId = userId,
                    AssessmentId = assessmentId,
                    Status = AssessmentStatus.draft,
                    CreatedAt = DateTime.Now
                };
                // Use repository: Add
                _unitOfWork.UserAssessments.Add(userAssessment);
                // Commit through UnitOfWork
                await _unitOfWork.CommitAsync();
            }

            // Check if assessment was already submitted
            if (userAssessment.Status == AssessmentStatus.completed || userAssessment.Status == AssessmentStatus.on_review)
            {
                throw new AlreadySubmittedException(
                    "This assessment has already been submitted and cannot be modified",
                    userAssessment.UpdatedAt ?? userAssessment.CreatedAt,
                    userAssessment.Status.ToString().ToLower()
                );
            }

            // Use repository: Get all questions for validation
            var questions = await _unitOfWork.Questions
                .GetQuestionsWithOptionsAsync(assessmentId);

            // Track uploaded files for response
            var uploadedFiles = new List<UploadedFileDto>();

            // Process each answer (similar to SaveDraft but with strict validation)
            foreach (var answerInput in request.Answers)
            {
                var question = questions.FirstOrDefault(q => q.QuestionId == answerInput.QuestionId);
                if (question == null)
                {
                    throw new NotFoundException($"Question with ID {answerInput.QuestionId} not found");
                }

                // Validate answer type matches question type
                var expectedType = question.Type.ToString().ToLower();
                if (answerInput.AnswerType != expectedType)
                {
                    throw new AnswerTypeMismatchException(
                        $"Answer type for question {question.QuestionId} does not match the question type",
                        answerInput.QuestionId,
                        expectedType,
                        answerInput.AnswerType
                    );
                }

                // Get or create answer
                var answer = userAssessment.Answers.FirstOrDefault(a => a.QuestionId == answerInput.QuestionId);
                if (answer == null)
                {
                    answer = new Answer
                    {
                        UserAssessmentId = userAssessment.UserAssessmentId,
                        QuestionId = answerInput.QuestionId,
                        CreatedAt = DateTime.Now
                    };
                    // Use repository: Add answer
                    _unitOfWork.Answers.Add(answer);
                }

                // Process based on answer type (with strict validation for submit)
                switch (question.Type)
                {
                    case QuestionType.essay:
                        // Check if new text is provided
                        if (!string.IsNullOrWhiteSpace(answerInput.Text))
                        {
                            // New/updated essay text provided
                            answer.AnswerText = answerInput.Text;
                            answer.UpdatedAt = DateTime.Now;
                        }
                        else if (string.IsNullOrWhiteSpace(answer.AnswerText))
                        {
                            // No new text AND no existing text - validation error
                            throw new EmptyAnswerException(
                                $"Essay answer for question {question.QuestionId} cannot be empty",
                                answerInput.QuestionId,
                                "essay"
                            );
                        }
                        // else: No new text but existing text exists - keep it unchanged
                        break;

                    case QuestionType.mcq:
                        if (answerInput.OptionId == null)
                        {
                            throw new MissingAnswerDataException(
                                $"Option ID is required for MCQ question {question.QuestionId}",
                                answerInput.QuestionId,
                                "mcq",
                                "option_id"
                            );
                        }

                        // Validate option belongs to question
                        var option = question.McqOptions.FirstOrDefault(o => o.OptionId == answerInput.OptionId);
                        if (option == null)
                        {
                            var validOptionIds = question.McqOptions.Select(o => o.OptionId).ToList();
                            throw new InvalidOptionException(
                                $"Option {answerInput.OptionId} does not belong to question {question.QuestionId}",
                                answerInput.QuestionId,
                                answerInput.OptionId.Value,
                                validOptionIds
                            );
                        }

                        // else: No new option but existing option exists - keep it unchanged
                        break;

                    case QuestionType.file:
                        // Check if new file is provided
                        if (answerInput.File != null)
                        {
                            // New file provided - validate and save it
                            await _fileService.ValidateFileAsync(answerInput.File, question.QuestionId);

                            // Delete old file if exists
                            if (answer.FileId != null)
                            {
                                await _fileService.DeleteFileAsync(answer.FileId.Value);
                            }

                            // Save new file
                            var fileEntity = await _fileService.SaveFileAsync(answerInput.File, userId, assessmentId);
                            answer.FileId = fileEntity.FileId;
                            answer.UpdatedAt = DateTime.Now;

                            // Add to uploaded files list for response
                            var (downloadUrl, previewUrl) = GenerateFileUrls(fileEntity.FileId, userId);
                            uploadedFiles.Add(new UploadedFileDto
                            {
                                QuestionId = question.QuestionId,
                                FileId = fileEntity.FileId,
                                Filename = fileEntity.OriginalFilename,
                                Size = fileEntity.FileSize,
                                DownloadUrl = downloadUrl,
                                PreviewUrl = previewUrl
                            });
                        }
                        else if (answer.FileId == null)
                        {
                            // No new file AND no existing file - validation error
                            throw new MissingAnswerDataException(
                                $"File is required for file upload question {question.QuestionId}",
                                answerInput.QuestionId,
                                "file",
                                "file"
                            );
                        }
                        // else: No new file but existing file exists - keep it unchanged
                        break;
                }
            }

            // Ensure all questions are answered (either from draft or this submit request)
            var allQuestionIds = questions.Select(q => q.QuestionId).ToList();
            var unansweredQuestions = new List<int>();

            foreach (var questionId in allQuestionIds)
            {
                var existingAnswer = userAssessment.Answers.FirstOrDefault(a => a.QuestionId == questionId);
                var question = questions.First(q => q.QuestionId == questionId);
                
                // Check if question has valid answer data
                bool hasValidAnswer = false;
                
                if (existingAnswer != null)
                {
                    switch (question.Type)
                    {
                        case QuestionType.essay:
                            hasValidAnswer = !string.IsNullOrWhiteSpace(existingAnswer.AnswerText);
                            break;
                        case QuestionType.mcq:
                            hasValidAnswer = existingAnswer.SelectedOptionId != null;
                            break;
                        case QuestionType.file:
                            hasValidAnswer = existingAnswer.FileId != null;
                            break;
                    }
                }
                
                if (!hasValidAnswer)
                {
                    unansweredQuestions.Add(questionId);
                }
            }

            if (unansweredQuestions.Any())
            {
                throw new IncompleteAssessmentException(
                    "All questions must be answered before submission",
                    allQuestionIds.Count,
                    allQuestionIds.Count - unansweredQuestions.Count,
                    unansweredQuestions
                );
            }

            // Track answered questions for response (all questions are answered if we reach here)
            var answeredQuestionIds = allQuestionIds;

            // Calculate score for MCQ questions and determine status
            var mcqQuestions = questions.Where(q => q.Type == QuestionType.mcq).ToList();
            var hasNonMcqQuestions = questions.Any(q => q.Type != QuestionType.mcq);

            int? calculatedScore = null;
            if (mcqQuestions.Any())
            {
                var correctAnswers = 0;
                foreach (var mcqQuestion in mcqQuestions)
                {
                    var answer = userAssessment.Answers.FirstOrDefault(a => a.QuestionId == mcqQuestion.QuestionId);
                    if (answer?.SelectedOptionId != null)
                    {
                        var selectedOption = mcqQuestion.McqOptions.FirstOrDefault(o => o.OptionId == answer.SelectedOptionId);
                        if (selectedOption?.IsCorrect == true)
                        {
                            correctAnswers++;
                        }
                    }
                }

                // Calculate percentage score
                calculatedScore = (int)Math.Round((double)correctAnswers / questions.Count * 100);
            }

            // Set status to completed for all submissions
            userAssessment.Status = AssessmentStatus.completed;
            
            // If has non-MCQ questions and no MCQ score calculated, generate random score
            if (hasNonMcqQuestions && calculatedScore == null)
            {
                // Generate random score between 0-100 for essay/file questions
                var random = new Random();
                userAssessment.Score = random.Next(0, 101); // 0 to 100 inclusive
            }
            else
            {
                // Use calculated MCQ score or mixed score
                userAssessment.Score = calculatedScore;
            }

            userAssessment.UpdatedAt = DateTime.Now;

            // Commit all changes through UnitOfWork (one transaction)
            await _unitOfWork.CommitAsync();

            // Return response with submission details
            return new SubmitAssessmentResponse
            {
                AssessmentId = assessmentId,
                Status = userAssessment.Status.ToString().ToLower(),
                SubmittedAt = userAssessment.UpdatedAt.Value,
                TotalQuestions = allQuestionIds.Count,
                AnsweredQuestions = answeredQuestionIds.Count,
                UploadedFiles = uploadedFiles
            };
        }

        public async Task<ApiResponse<List<CompletedAssessmentDto>>> GetCompletedAssessmentsAsync(
            int userId, int page, int size, string? semester)
        {
            // Use repository: Get enrolled class IDs
            var enrolledClassIds = await _unitOfWork.StudentEnrollments
                .GetActiveEnrolledClassIdsAsync(userId);

            // Use repository: Get completed user assessments
            var completedUserAssessments = await _unitOfWork.UserAssessments
                .GetCompletedUserAssessmentsAsync(userId);

            var completedAssessmentIds = completedUserAssessments.Select(ua => ua.AssessmentId).ToList();

            if (!completedAssessmentIds.Any())
            {
                return new ApiResponse<List<CompletedAssessmentDto>>
                {
                    Data = new List<CompletedAssessmentDto>(),
                    Paging = new PagingDto { Page = page, Size = size, TotalItem = 0, TotalPage = 0 },
                    Success = true
                };
            }

            // Parse semester filter
            int? semesterId = null;
            if (!string.IsNullOrEmpty(semester) && int.TryParse(semester, out int parsedSemester))
            {
                semesterId = parsedSemester;
            }

            // Use repository: Get assessments with details
            var assessments = await _unitOfWork.Assessments
                .GetCompletedAssessmentsWithDetailsAsync(
                    enrolledClassIds,
                    completedAssessmentIds,
                    academicTermId: semesterId);

            // Calculate pagination
            var totalItem = assessments.Count;
            var totalPage = (int)Math.Ceiling(totalItem / (double)size);

            // Apply ordering, pagination, and mapping
            var result = assessments
                .OrderByDescending(a => a.EndDate)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(a =>
                {
                    var userAssessment = completedUserAssessments.First(ua => ua.AssessmentId == a.AssessmentId);
                    return new CompletedAssessmentDto
                    {
                        Id = a.AssessmentId,
                        Name = a.Title,
                        Class = a.Class.ClassCode,
                        LecturerName = a.Class.Lecturer.User.Name,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        Status = userAssessment.Status.ToString().ToLower(),
                        Score = userAssessment.Score,
                        SubmittedAt = userAssessment.UpdatedAt
                    };
                })
                .ToList();

            return new ApiResponse<List<CompletedAssessmentDto>>
            {
                Data = result,
                Paging = new PagingDto
                {
                    Page = page,
                    Size = size,
                    TotalItem = totalItem,
                    TotalPage = totalPage
                },
                Success = true
            };
        }

        public async Task<AssessmentDetailDto> GetCompletedAssessmentDetailAsync(int assessmentId, int userId)
        {
            // Use repository: Get assessment with class info
            var assessment = await _unitOfWork.Assessments
                .GetAssessmentWithClassAsync(assessmentId);

            if (assessment == null)
            {
                throw new NotFoundException($"Assessment with ID {assessmentId} not found");
            }

            // Use repository: Check if student is enrolled
            var isEnrolled = await _unitOfWork.StudentEnrollments
                .IsStudentEnrolledInClassAsync(userId, assessment.ClassId);

            if (!isEnrolled)
            {
                throw new ForbiddenException("You don't have permission to access this assessment");
            }

            // Use repository: Get completed user assessment
            var userAssessment = await _unitOfWork.UserAssessments
                .GetCompletedUserAssessmentWithAnswersAsync(userId, assessmentId);

            if (userAssessment == null)
            {
                throw new NotFoundException($"Completed assessment with ID {assessmentId} not found for this user");
            }

            // Use repository: Get all questions with options
            var questions = await _unitOfWork.Questions
                .GetQuestionsWithOptionsAsync(assessmentId);

            // Map to DTOs (similar to incomplete but read-only)
            var questionDtos = questions.Select(q =>
            {
                var existingAnswer = userAssessment.Answers.FirstOrDefault(a => a.QuestionId == q.QuestionId);

                AnswerDto? answerDto = null;
                if (existingAnswer != null)
                {
                    answerDto = new AnswerDto();

                    if (q.Type == QuestionType.essay)
                    {
                        answerDto.Text = existingAnswer.AnswerText;
                    }
                    else if (q.Type == QuestionType.mcq && existingAnswer.McqOption != null)
                    {
                        answerDto.Mcq = new McqSelectionDto
                        {
                            OptionId = existingAnswer.McqOption.OptionId,
                            Label = existingAnswer.McqOption.OptionLabel
                        };
                    }
                    else if (q.Type == QuestionType.file && existingAnswer.File != null)
                    {
                        var (downloadUrl, previewUrl) = GenerateFileUrls(existingAnswer.File.FileId, userId);
                        answerDto.File = new FileInfoDto
                        {
                            FileId = existingAnswer.File.FileId,
                            Filename = existingAnswer.File.OriginalFilename,
                            Extension = existingAnswer.File.FileExtension,
                            Size = existingAnswer.File.FileSize,
                            DownloadUrl = downloadUrl,
                            PreviewUrl = previewUrl
                        };
                    }
                }

                return new QuestionDto
                {
                    Id = q.QuestionId,
                    Question = q.Content,
                    AnswerType = q.Type.ToString().ToLower(),
                    Answer = answerDto,
                    Options = q.Type == QuestionType.mcq
                        ? q.McqOptions.Select(o => new McqOptionDto
                        {
                            OptionId = o.OptionId,
                            Label = o.OptionLabel,
                            Text = o.OptionText
                        }).ToList()
                        : null
                };
            }).ToList();

            return new AssessmentDetailDto
            {
                Assessment = new AssessmentInfoDto
                {
                    Id = assessment.AssessmentId,
                    Title = assessment.Title,
                    Status = userAssessment.Status.ToString().ToLower(),
                    Score = userAssessment.Score,
                    UpdatedAt = userAssessment.UpdatedAt
                },
                Questions = questionDtos
            };
        }
    }
}
