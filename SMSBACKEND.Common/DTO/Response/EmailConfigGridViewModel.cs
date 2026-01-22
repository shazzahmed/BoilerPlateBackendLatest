using Common.DTO.Response;
using System;

namespace Common.DTO.Response
{
    public class EmailConfigGridViewModel : EntityBase
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Hostname { get; set; }
        public string Port { get; set; }
    }
}

