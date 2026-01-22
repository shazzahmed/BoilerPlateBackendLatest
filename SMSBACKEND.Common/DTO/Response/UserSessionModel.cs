using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO.Response
{
    public class UserSessionModel
    {
        public int Id { get; set; }
        public bool IsMobile { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string IpAddress { get; set; }
        public string Region { get; set; }
        public string RegionName { get; set; }
        public string Status { get; set; }
        public string Timezone { get; set; }
        public string Zip { get; set; }
        public DateTime LastActivity { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string BrowserName { get; set; }
        public string DeviceName { get; set; }
        public string Platform { get; set; }
        public string Guid { get; set; }
        public bool IsRemembered { get; set; }
        public bool IsActive { get; set; }
        public UserSessionModel()
        {
            IsActive = true;
        }
    }

    public class LastActivitiesModel
    {
        public string IpAddress { get; set; }

        public DateTime LastActivity { get; set; }
    }
}
