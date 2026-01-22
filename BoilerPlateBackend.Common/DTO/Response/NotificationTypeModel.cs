using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class NotificationTypeModel
    {
        public NotificationTypes Id { get; set; }
        public string Name { get; set; }
    }
}
