using System;
using System.Collections.Generic;
using System.Text;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class StatusTypeModel
    {
        public StatusTypes Id { get; set; }
        public string Name { get; set; }
    }
}
