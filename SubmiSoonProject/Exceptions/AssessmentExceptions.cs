namespace SubmiSoonProject.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }

    public class AssessmentExpiredException : Exception
    {
        public DateTime EndDate { get; set; }

        public AssessmentExpiredException(string message, DateTime endDate) : base(message)
        {
            EndDate = endDate;
        }
    }

    public class InvalidFileTypeException : Exception
    {
        public int QuestionId { get; set; }
        public string ReceivedExtension { get; set; } = null!;
        public List<string> AllowedExtensions { get; set; } = new();

        public InvalidFileTypeException(string message, int questionId, string receivedExtension, List<string> allowedExtensions) : base(message)
        {
            QuestionId = questionId;
            ReceivedExtension = receivedExtension;
            AllowedExtensions = allowedExtensions;
        }
    }

    public class FileTooLargeException : Exception
    {
        public int QuestionId { get; set; }
        public long FileSize { get; set; }
        public long MaxSize { get; set; }

        public FileTooLargeException(string message, int questionId, long fileSize, long maxSize) : base(message)
        {
            QuestionId = questionId;
            FileSize = fileSize;
            MaxSize = maxSize;
        }
    }

    public class AnswerTypeMismatchException : Exception
    {
        public int QuestionId { get; set; }
        public string ExpectedType { get; set; } = null!;
        public string ReceivedType { get; set; } = null!;

        public AnswerTypeMismatchException(string message, int questionId, string expectedType, string receivedType) : base(message)
        {
            QuestionId = questionId;
            ExpectedType = expectedType;
            ReceivedType = receivedType;
        }
    }

    public class MissingAnswerDataException : Exception
    {
        public int QuestionId { get; set; }
        public string AnswerType { get; set; } = null!;
        public string MissingField { get; set; } = null!;

        public MissingAnswerDataException(string message, int questionId, string answerType, string missingField) : base(message)
        {
            QuestionId = questionId;
            AnswerType = answerType;
            MissingField = missingField;
        }
    }

    public class InvalidOptionException : Exception
    {
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
        public List<int> ValidOptionIds { get; set; } = new();

        public InvalidOptionException(string message, int questionId, int selectedOptionId, List<int> validOptionIds) : base(message)
        {
            QuestionId = questionId;
            SelectedOptionId = selectedOptionId;
            ValidOptionIds = validOptionIds;
        }
    }

    public class IncompleteAssessmentException : Exception
    {
        public int TotalQuestions { get; set; }
        public int AnsweredQuestions { get; set; }
        public List<int> UnansweredQuestions { get; set; } = new();

        public IncompleteAssessmentException(string message, int totalQuestions, int answeredQuestions, List<int> unansweredQuestions) : base(message)
        {
            TotalQuestions = totalQuestions;
            AnsweredQuestions = answeredQuestions;
            UnansweredQuestions = unansweredQuestions;
        }
    }

    public class AlreadySubmittedException : Exception
    {
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; } = null!;

        public AlreadySubmittedException(string message, DateTime submittedAt, string status) : base(message)
        {
            SubmittedAt = submittedAt;
            Status = status;
        }
    }

    public class EmptyAnswerException : Exception
    {
        public int QuestionId { get; set; }
        public string AnswerType { get; set; } = null!;

        public EmptyAnswerException(string message, int questionId, string answerType) : base(message)
        {
            QuestionId = questionId;
            AnswerType = answerType;
        }
    }
}
