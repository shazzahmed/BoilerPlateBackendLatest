using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using static Common.Utilities.Enums;

namespace Common.DTO.Request
{
    public class RegisterUserModel
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "The {0} must contain less than {1} characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "The {0} must contain less than {1} characters.")]
        public string LastName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string CNIC { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        //[Remote("IsUserExists", "Account", ErrorMessage = "Email address is already in use.")]
        [RegularExpression("[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})", 
         ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Invalid phone number.")]
        ////[Required(AllowEmptyStrings = false, ErrorMessage = "We need the cell phone to continue.")]
        //[Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(25, ErrorMessage = "The password must be between {2} and {1} characters long.", MinimumLength = 3)]
        //[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{3,25}$",ErrorMessage = "At least 3 characters, one uppercase and lowercase")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new List<string>();
        public string Picture { get; set; } = string.Empty;
        public string ParentId { get; set; }
        //public int CountryId { get; set; }
        //public int CityId { get; set; }
        //[MaxLength(100, ErrorMessage = "Address must be less than 100 characters.")]
        //public string Address { get; set; } = string.Empty;
        //public string ZipCode { get; set; } = string.Empty;
        //public bool IsWorkerHasAgency { get; set; }
        //[Required(ErrorMessage = "Worker agency is requied.")]
        //public string AgencyId { get; set; } = string.Empty;
        //public string WorkerType { get; set; } = string.Empty;
        //[Range(typeof(bool), "true", "true", ErrorMessage = "The field Agree Terms must be checked.")]
        //public bool AgreeTerms { get; set; }
        //public string? ConfirmEmailURL { get; set; }
        //public string? WorkerEmailURL { get; set; }
        //public List<ExternalLoginListViewModel> ExternalLoginURLs { get; set; }
    }
}
