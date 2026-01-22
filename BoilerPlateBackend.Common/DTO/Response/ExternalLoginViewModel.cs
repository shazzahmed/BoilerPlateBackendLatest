namespace Common.DTO.Response
{
    public class ExternalLoginViewModel
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public bool IsPersistent { get; set; }
        public bool BypassTwoFactor { get; set; }
    }
}
