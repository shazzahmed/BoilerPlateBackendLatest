using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO.Response
{
    public class PreviousPasswordModel
    {
        public string PasswordHash { get; set; }
        public string UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime CreateDateUtc { get; set; }


        //public virtual ApplicationUser User { get; set; }
    }
}
