using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class BaseModel
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public object SupportData { get; set; }
        public string Message { get; set; }
        public int Total { get; set; }
        public int LastId { get; set; }
        public string CorrelationId { get; set; }

        public BaseModel()
        {

        }

        public BaseModel(bool success, object data = null, string message = "", int total = 0, int lastId = 0, object supportData = null, string correlationId = null)
        {
            Success = success;
            Data = data;
            Message = message;
            Total = total;
            LastId = lastId;
            SupportData = supportData;
            CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        }

        public static BaseModel Create(bool success, object data = null, string message = "", int total = 0, int lastId = 0, object supportData = null)
        {
            return new BaseModel() { Success = success, Data = data, Message = message, Total = total, LastId = lastId, SupportData = supportData, CorrelationId = Guid.NewGuid().ToString() };
        }

        public static BaseModel Failed(string message, object data = null, int total = 0, int lastId = 0)
        {
            return new BaseModel(false, data, message, total, lastId, null, Guid.NewGuid().ToString());
        }

        public static BaseModel Succeed(object data = null, int total = 0, string message = "", int lastId = 0, object supportData = null)
        {
            return new BaseModel(true, data, message, total, lastId, supportData, Guid.NewGuid().ToString());
        }
    }

    public class BaseModel<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public int Total { get; set; }
        public int LastId { get; set; }
        public string CorrelationId { get; set; }
    }

    public class LoginResponseModel
    {
        public LoginStatus Status { get; set; }
        public bool Success { get; set; }
        public System.Security.Claims.ClaimsPrincipal Data { get; set; }
        public string Message { get; set; }
        public int Total { get; set; }
    }
}