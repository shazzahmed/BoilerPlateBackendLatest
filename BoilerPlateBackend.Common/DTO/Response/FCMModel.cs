using Common.DTO.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO.Response
{
   public class FCMModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Platform { get; set; }
        public string Token { get; set; }
        public virtual ApplicationUserModel AppUser { get; set; }

    }
}
