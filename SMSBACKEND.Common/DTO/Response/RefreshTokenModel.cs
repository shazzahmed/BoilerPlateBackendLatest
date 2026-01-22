using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO.Response
{
    public class RefreshTokenModel
    {
        public int RefreshTokenId { get; set; } // New auto-incrementing primary key
        public string Id { get; set; } // User ID (foreign key)
        public string Username { get; set; }
        public string Token { get; set; }
        public string DeviceId { get; set; } // New property for multi-device support
        public bool Revoked { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expired { get; set; }
    }
}
