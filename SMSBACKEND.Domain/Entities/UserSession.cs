using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserSession 
    {
        [Key]
        public int Id { get; set; }
        public bool IsMobile { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string RegionName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string BrowserName { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Guid { get; set; } = string.Empty;
        public bool IsRemembered { get; set; }
        public bool IsActive { get; set; }
        public string RefreshTokenId { get; set; } = string.Empty;
        [Column(TypeName = "datetime2")]
        public DateTime LastActivity { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime LastActivityUtc { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
