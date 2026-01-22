using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class VerifyCodeModel
    {
        public string UserName { get; set; } = string.Empty;

        //[Required]
        public string Provider { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        [Display(Name = "Remember this machine?")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
