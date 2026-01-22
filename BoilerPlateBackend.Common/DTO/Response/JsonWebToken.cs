using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO.Response
{
    public class JsonWebToken
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Issue { get; set; }
        public DateTime Expires { get; set; }
    }
}
