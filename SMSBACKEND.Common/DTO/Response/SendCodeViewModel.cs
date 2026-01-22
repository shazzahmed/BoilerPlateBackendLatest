using Microsoft.AspNetCore.Mvc.Rendering;

namespace Common.DTO.Response
{
    public class SendCodeViewModel
    {
        public string UserName { get; set; }
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }
}
