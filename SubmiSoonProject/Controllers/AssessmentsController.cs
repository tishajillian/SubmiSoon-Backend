using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubmiSoonProject.DTOs.Assessment;
using SubmiSoonProject.DTOs.Common;
using SubmiSoonProject.Exceptions;
using SubmiSoonProject.Services;
using System.Security.Claims;

namespace SubmiSoonProject.Controllers
{
    [ApiController]
    [Route("")]
    public class AssessmentsController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentsController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        [Authorize(Roles = "student")]
        [HttpGet("assessments/incomplete")]
        public async Task<IActionResult> GetIncompleteAssessments(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? semester = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _assessmentService.GetIncompleteAssessmentsAsync(userId, page, size, semester);
            return Ok(result);
        }

        [Authorize(Roles = "student")]
        [HttpGet("assessments/incomplete/{id}")]
        public async Task<IActionResult> GetIncompleteAssessmentDetail(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var result = await _assessmentService.GetIncompleteAssessmentDetailAsync(id, userId);
                return Ok(new { data = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ASSESSMENT_NOT_FOUND",
                        Message = ex.Message
                    }
                });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ACCESS_DENIED",
                        Message = ex.Message
                    }
                });
            }
            catch (AssessmentExpiredException ex)
            {
                return StatusCode(410, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ASSESSMENT_EXPIRED",
                        Message = ex.Message,
                        Details = new { endDate = ex.EndDate }
                    }
                });
            }
        }

        [Authorize(Roles = "student")]
        [HttpPut("assessments/incomplete/{id}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(SaveDraftResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 410)]
        [ProducesResponseType(typeof(ApiErrorResponse), 413)]
        [RequestFormLimits(MultipartBodyLengthLimit = 10485760)] // 10MB limit
        public async Task<IActionResult> SaveDraft(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                // Parse multipart form data manually
                var form = await Request.ReadFormAsync();
                var request = new SaveDraftRequest();

                // Group answers by index
                var answerIndices = new HashSet<int>();
                foreach (var key in form.Keys)
                {
                    if (key.StartsWith("answers[") && key.Contains("]"))
                    {
                        var indexStr = key.Substring(8, key.IndexOf(']') - 8);
                        if (int.TryParse(indexStr, out int index))
                        {
                            answerIndices.Add(index);
                        }
                    }
                }

                // Parse each answer
                foreach (var index in answerIndices.OrderBy(i => i))
                {
                    var questionIdKey = $"answers[{index}].question_id";
                    var answerTypeKey = $"answers[{index}].answer_type";
                    var textKey = $"answers[{index}].text";
                    var optionIdKey = $"answers[{index}].option_id";
                    var fileKey = $"answers[{index}].file";

                    if (!form.ContainsKey(questionIdKey) || !form.ContainsKey(answerTypeKey))
                    {
                        continue; // Skip invalid entries
                    }

                    var answerInput = new AnswerInput
                    {
                        QuestionId = int.Parse(form[questionIdKey]!),
                        AnswerType = form[answerTypeKey]!.ToString().ToLower()
                    };

                    // Add optional fields based on answer type
                    if (form.ContainsKey(textKey) && !string.IsNullOrEmpty(form[textKey]))
                    {
                        answerInput.Text = form[textKey];
                    }

                    if (form.ContainsKey(optionIdKey) && !string.IsNullOrEmpty(form[optionIdKey]))
                    {
                        answerInput.OptionId = int.Parse(form[optionIdKey]!);
                    }

                    if (form.Files.Any(f => f.Name == fileKey))
                    {
                        answerInput.File = form.Files.GetFile(fileKey);
                    }

                    request.Answers.Add(answerInput);
                }

                var result = await _assessmentService.SaveDraftAsync(id, userId, request);
                return Ok(new { data = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ASSESSMENT_NOT_FOUND",
                        Message = ex.Message
                    }
                });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ACCESS_DENIED",
                        Message = ex.Message
                    }
                });
            }
            catch (AssessmentExpiredException ex)
            {
                return StatusCode(410, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ASSESSMENT_EXPIRED",
                        Message = ex.Message,
                        Details = new { endDate = ex.EndDate }
                    }
                });
            }
            catch (AlreadySubmittedException ex)
            {
                return Conflict(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ALREADY_SUBMITTED",
                        Message = ex.Message,
                        Details = new { submittedAt = ex.SubmittedAt, status = ex.Status }
                    }
                });
            }
            catch (EmptyAnswerException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "EMPTY_ANSWER",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, answerType = ex.AnswerType }
                    }
                });
            }
            catch (MissingAnswerDataException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "MISSING_ANSWER_DATA",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, answerType = ex.AnswerType, missingField = ex.MissingField }
                    }
                });
            }
            catch (InvalidOptionException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "INVALID_OPTION",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, selectedOptionId = ex.SelectedOptionId, validOptionIds = ex.ValidOptionIds }
                    }
                });
            }
            catch (InvalidFileTypeException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "INVALID_FILE_TYPE",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, receivedExtension = ex.ReceivedExtension, allowedExtensions = ex.AllowedExtensions }
                    }
                });
            }
            catch (FileTooLargeException ex)
            {
                return StatusCode(413, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "FILE_TOO_LARGE",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, fileSize = ex.FileSize, maxSize = ex.MaxSize }
                    }
                });
            }
            catch (AnswerTypeMismatchException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ANSWER_TYPE_MISMATCH",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, expectedType = ex.ExpectedType, receivedType = ex.ReceivedType }
                    }
                });
            }
        }

        [Authorize(Roles = "student")]
        [HttpPost("assessments/incomplete/{id}/submit")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(SubmitAssessmentResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 401)]
        [ProducesResponseType(typeof(ApiErrorResponse), 403)]
        [ProducesResponseType(typeof(ApiErrorResponse), 410)]
        [ProducesResponseType(typeof(ApiErrorResponse), 413)]
        [RequestFormLimits(MultipartBodyLengthLimit = 10485760)] // 10MB limit
        public async Task<IActionResult> SubmitAssessment(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                // Parse multipart form data manually
                var form = await Request.ReadFormAsync();
                var request = new SaveDraftRequest();

                // Group answers by index
                var answerIndices = new HashSet<int>();
                foreach (var key in form.Keys)
                {
                    if (key.StartsWith("answers[") && key.Contains("]"))
                    {
                        var indexStr = key.Substring(8, key.IndexOf(']') - 8);
                        if (int.TryParse(indexStr, out int index))
                        {
                            answerIndices.Add(index);
                        }
                    }
                }

                // Parse each answer
                foreach (var index in answerIndices.OrderBy(i => i))
                {
                    var questionIdKey = $"answers[{index}].question_id";
                    var answerTypeKey = $"answers[{index}].answer_type";
                    var textKey = $"answers[{index}].text";
                    var optionIdKey = $"answers[{index}].option_id";
                    var fileKey = $"answers[{index}].file";

                    if (!form.ContainsKey(questionIdKey) || !form.ContainsKey(answerTypeKey))
                    {
                        continue; // Skip invalid entries
                    }

                    var answerInput = new AnswerInput
                    {
                        QuestionId = int.Parse(form[questionIdKey]!),
                        AnswerType = form[answerTypeKey]!.ToString().ToLower()
                    };

                    // Add optional fields based on answer type
                    if (form.ContainsKey(textKey) && !string.IsNullOrEmpty(form[textKey]))
                    {
                        answerInput.Text = form[textKey];
                    }

                    if (form.ContainsKey(optionIdKey) && !string.IsNullOrEmpty(form[optionIdKey]))
                    {
                        answerInput.OptionId = int.Parse(form[optionIdKey]!);
                    }

                    if (form.Files.Any(f => f.Name == fileKey))
                    {
                        answerInput.File = form.Files.GetFile(fileKey);
                    }

                    request.Answers.Add(answerInput);
                }

                var result = await _assessmentService.SubmitAssessmentAsync(id, userId, request);
                return Ok(new { data = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ASSESSMENT_NOT_FOUND",
                        Message = ex.Message
                    }
                });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ACCESS_DENIED",
                        Message = ex.Message
                    }
                });
            }
            catch (AssessmentExpiredException ex)
            {
                return StatusCode(410, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ASSESSMENT_EXPIRED",
                        Message = ex.Message,
                        Details = new { endDate = ex.EndDate }
                    }
                });
            }
            catch (AlreadySubmittedException ex)
            {
                return Conflict(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ALREADY_SUBMITTED",
                        Message = ex.Message,
                        Details = new { submittedAt = ex.SubmittedAt, status = ex.Status }
                    }
                });
            }
            catch (IncompleteAssessmentException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "INCOMPLETE_ASSESSMENT",
                        Message = ex.Message,
                        Details = new { totalQuestions = ex.TotalQuestions, answeredQuestions = ex.AnsweredQuestions, unansweredQuestions = ex.UnansweredQuestions }
                    }
                });
            }
            catch (EmptyAnswerException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "EMPTY_ANSWER",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, answerType = ex.AnswerType }
                    }
                });
            }
            catch (MissingAnswerDataException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "MISSING_ANSWER_DATA",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, answerType = ex.AnswerType, missingField = ex.MissingField }
                    }
                });
            }
            catch (InvalidOptionException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "INVALID_OPTION",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, selectedOptionId = ex.SelectedOptionId, validOptionIds = ex.ValidOptionIds }
                    }
                });
            }
            catch (InvalidFileTypeException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "INVALID_FILE_TYPE",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, receivedExtension = ex.ReceivedExtension, allowedExtensions = ex.AllowedExtensions }
                    }
                });
            }
            catch (FileTooLargeException ex)
            {
                return StatusCode(413, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "FILE_TOO_LARGE",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, fileSize = ex.FileSize, maxSize = ex.MaxSize }
                    }
                });
            }
            catch (AnswerTypeMismatchException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ANSWER_TYPE_MISMATCH",
                        Message = ex.Message,
                        Details = new { questionId = ex.QuestionId, expectedType = ex.ExpectedType, receivedType = ex.ReceivedType }
                    }
                });
            }
        }

        [Authorize(Roles = "student")]
        [HttpGet("assessments")]
        public async Task<IActionResult> GetCompletedAssessments(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? semester = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _assessmentService.GetCompletedAssessmentsAsync(userId, page, size, semester);
            return Ok(result);
        }

        [Authorize(Roles = "student")]
        [HttpGet("assessments/{id}")]
        public async Task<IActionResult> GetCompletedAssessmentDetail(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var result = await _assessmentService.GetCompletedAssessmentDetailAsync(id, userId);
                return Ok(new { data = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ASSESSMENT_NOT_FOUND",
                        Message = ex.Message
                    }
                });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new ApiErrorResponse
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "ACCESS_DENIED",
                        Message = ex.Message
                    }
                });
            }
        }
    }
}
