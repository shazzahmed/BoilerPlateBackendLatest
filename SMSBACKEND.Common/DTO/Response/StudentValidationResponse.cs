namespace Common.DTO.Response
{
    public class StudentValidationResponse
    {
        public bool IsValid { get; set; }
        public bool StudentExists { get; set; }
        public string Message { get; set; } = string.Empty;
        public StudentValidationResult ValidationResult { get; set; }
        public string? GeneratedAdmissionNo { get; set; }
    }

    public class StudentValidationResult
    {
        public bool AdmissionNoValid { get; set; }
        public bool RollNoValid { get; set; }
        public bool AgeValid { get; set; }
        public string AdmissionNoMessage { get; set; } = string.Empty;
        public string RollNoMessage { get; set; } = string.Empty;
        public string AgeMessage { get; set; } = string.Empty;
    }
}
