using System;
using System.Collections.Generic;
using System.Text;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class UserDetailsModel
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? phoneNumber { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? address { get; set; }
    }
}
