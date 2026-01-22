using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO.Response
{
    public class DayModel
    {
        public int DayId { get; set; }
        public string DayName { get; set; }
        public string FullDayName { get; set; }
        //public virtual ICollection<UserDeliveryDayModel> UserDeliveryDays { get; set; }
    }
}
