namespace Common.DTO.Response
{
    public class UpdateUserInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Picture { get; set; }
    }
}
