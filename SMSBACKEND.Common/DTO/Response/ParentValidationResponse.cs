using Common.DTO.Response;
using Common.DTO.Request;

namespace Common.DTO.Response
{
    public class ParentValidationResponse
    {
        public bool IsValid { get; set; }
        public bool ParentExists { get; set; }
        public string Message { get; set; } = string.Empty;
        public ApplicationUserModel? ExistingParent { get; set; }
        public ParentValidationResult ValidationResult { get; set; }
    }

    public class ParentValidationResult
    {
        public string ValidationMethod { get; set; } = string.Empty; // "CNIC", "Phone+Name"
        public bool IsUnique { get; set; }
        public string SuggestedAction { get; set; } = string.Empty; // "CREATE_NEW", "LINK_EXISTING", "UPDATE_EXISTING"
    }
}
