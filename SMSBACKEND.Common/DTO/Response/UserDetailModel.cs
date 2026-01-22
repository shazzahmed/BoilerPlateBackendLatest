namespace Common.DTO.Response
{
    public class UserDetailModel
    {
        public UserDetailModel()
        {
            Roles = new List<string>();
        }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string NicNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Picture { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool HasPassword { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFactorType { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public List<string> Roles { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
    }
}
