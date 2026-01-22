using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO.Response
{
    public class VerificationCodeModel
    {
        public bool isActive;
        public string code;
        public int remainingValidTime;
    }
}
