using Common.DTO.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.DTO.Response
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string CNIC { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        //[Remote("IsUserExists", "Account", ErrorMessage = "Email address is already in use.")]
        [RegularExpression("[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})",
         ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid phone number.")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "We need the cell phone to continue.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
