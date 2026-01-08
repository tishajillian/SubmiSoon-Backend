using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SubmiSoonProject.Filters
{
    /// Custom Swagger operation filter to document multipart/form-data endpoints
    /// Adds proper schema for file upload endpoints with dynamic array structures
    public class MultipartFormDataOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check if endpoint consumes multipart/form-data
            var consumesMultipart = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<Microsoft.AspNetCore.Mvc.ConsumesAttribute>()
                .Any(c => c.ContentTypes.Contains("multipart/form-data"));

            if (!consumesMultipart)
                return;

            // Check if this is SaveDraft or SubmitAssessment endpoint
            var actionName = context.ApiDescription.ActionDescriptor.DisplayName ?? string.Empty;
            if (!actionName.Contains("SaveDraft") && !actionName.Contains("SubmitAssessment"))
                return;

            // Define the multipart form schema for assessment answers
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Description = "Submit answers in multipart/form-data format. Add multiple answers by incrementing the index: answers[0], answers[1], etc.",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                // Answer 0
                                ["answers[0].question_id"] = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Description = "Question ID (required for first answer)"
                                },
                                ["answers[0].answer_type"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Enum = new List<IOpenApiAny>
                                    {
                                        new OpenApiString("mcq"),
                                        new OpenApiString("essay"),
                                        new OpenApiString("file")
                                    },
                                    Description = "Answer type: 'mcq', 'essay', or 'file' (required)"
                                },
                                ["answers[0].text"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "Essay answer text (required for answer_type=essay)",
                                    Nullable = true
                                },
                                ["answers[0].option_id"] = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Description = "Selected option ID (required for answer_type=mcq)",
                                    Nullable = true
                                },
                                ["answers[0].file"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "File upload (required for answer_type=file, max 2MB, allowed: .doc, .docx, .pdf, .jpg, .jpeg, .png)",
                                    Nullable = true
                                },
                                
                                // Answer 1 (example for multiple answers)
                                ["answers[1].question_id"] = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Description = "Question ID (second answer - optional, add more by incrementing index)"
                                },
                                ["answers[1].answer_type"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Enum = new List<IOpenApiAny>
                                    {
                                        new OpenApiString("mcq"),
                                        new OpenApiString("essay"),
                                        new OpenApiString("file")
                                    },
                                    Description = "Answer type for second answer"
                                },
                                ["answers[1].text"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "Essay text for second answer",
                                    Nullable = true
                                },
                                ["answers[1].option_id"] = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Description = "Option ID for second answer",
                                    Nullable = true
                                },
                                ["answers[1].file"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "File for second answer",
                                    Nullable = true
                                }
                            },
                            Required = new HashSet<string>
                            {
                                "answers[0].question_id",
                                "answers[0].answer_type"
                            }
                        },
                        Encoding = new Dictionary<string, OpenApiEncoding>
                        {
                            ["answers[0].file"] = new OpenApiEncoding
                            {
                                ContentType = "application/octet-stream",
                                Style = ParameterStyle.Form
                            },
                            ["answers[1].file"] = new OpenApiEncoding
                            {
                                ContentType = "application/octet-stream",
                                Style = ParameterStyle.Form
                            }
                        }
                    }
                }
            };
        }
    }
}
