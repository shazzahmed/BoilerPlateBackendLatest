using System;

namespace Domain.Entities
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; } // New auto-incrementing primary key
        public string Id { get; set; } = string.Empty; // User ID (foreign key)
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty; // New property for multi-device support
        public bool Revoked { get; set; }
        public DateTime Issued { get; set; } = DateTime.Now;
        public DateTime Expired { get; set; }
    }
}
