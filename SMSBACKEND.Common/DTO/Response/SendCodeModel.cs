namespace Common.DTO.Response
{
    public class SendCodeModel
    {
        public string SelectedProvider { get; set; }

        //public ICollection<System.Web.Mvc.SelectListItem> Provider { get; set; }
        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}
