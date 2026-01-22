using Domain.Entities;

namespace SMSBACKEND.Domain.Entities.School.Pending;

public class Language : BaseEntity
{
    public int Id { get; set; }
    public string LanguageName { get; set; }
    public string ShortCode { get; set; }
    public string CountryCode { get; set; }
}
