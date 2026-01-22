using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class AuthenticationResponse
    {
        public ResponseType ResponseType { get; set; }

        public string Data;

        public AuthenticationResponse()
        {

        }
        public AuthenticationResponse(ResponseType responseType, string data)
        {
            ResponseType = responseType;
            Data = data;
        }
        public static AuthenticationResponse Create(ResponseType responseType, string data)
        {
            return new AuthenticationResponse(responseType, data);
        }

        public static AuthenticationResponse Error(string data)
        {
            return new AuthenticationResponse(ResponseType.Error, data);
        }

        public static AuthenticationResponse Success(string data)
        {
            return new AuthenticationResponse(ResponseType.Success, data);
        }
    }

    public class LoginResponse
    {
        public String? Status { get; set; }
        public List<string>? Roles { get; set; }
        public List<string>? Permissions { get; set; }
        public string? UserId { get; set; }
        public string? RoleId { get; set; }
        public object? userinfo { get; set; }
        public string? Message { get; set; }
        public string? token { get; set; }
        public object? Data { get; set; }
        public string? DeviceId { get; set; } // New property for device identification
        public DeviceInfo? DeviceInfo { get; set; } // New property for device details
        
        // MULTI-TENANCY: Tenant information for the logged-in user
        public int TenantId { get; set; }
        public TenantInfo? TenantInfo { get; set; }
    }
    
    /// <summary>
    /// Tenant information returned during login
    /// Provides branding, subscription, and configuration details to the frontend
    /// </summary>
    public class TenantInfo
    {
        public int TenantId { get; set; }
        public string? TenantCode { get; set; }
        public string? SchoolName { get; set; }
        public string? ShortName { get; set; }
        public string? Domain { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string? Website { get; set; }
        public string? Logo { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public DateTime SubscriptionStartDate { get; set; } = DateTime.UtcNow;
        public DateTime? SubscriptionEndDate { get; set; }
        public string SubscriptionTier { get; set; } = "Basic";
        public bool IsActive { get; set; } = true;
        public bool IsSubscriptionValid { get; set; } = true;
        public int MaxStudents { get; set; } = 500;
        public int CurrentStudentCount { get; set; } = 0;
        public int MaxStaff { get; set; } = 50;
        public int CurrentStaffCount { get; set; } = 0;
        public decimal StorageLimitGB { get; set; } = 10;
        public decimal CurrentStorageUsedGB { get; set; } = 0;
        public string? TimeZone { get; set; }
        public string? Currency { get; set; }
        public string? CurrencySymbol { get; set; }
        public string? Language { get; set; }
        public string? DateFormat { get; set; }
        public string? InstitutionType { get; set; }
        public string? EducationLevel { get; set; }
        public int AcademicYearStartMonth { get; set; } = 9; // September
        public int AcademicYearEndMonth { get; set; } = 6; // June
        //public string? Logo { get; set; }
        //public string? PrimaryColor { get; set; }
        //public string? SecondaryColor { get; set; }
        //public string? SubscriptionTier { get; set; }
        //public DateTime? SubscriptionEndDate { get; set; }
        //public bool IsSubscriptionValid { get; set; }
        //public string? Currency { get; set; }
        //public string? CurrencySymbol { get; set; }
        //public string? Language { get; set; }
        //public string? DateFormat { get; set; }
        //public string? TimeZone { get; set; }

        //// Resource limits for UI validation
        //public int MaxStudents { get; set; }
        //public int CurrentStudentCount { get; set; }
        //public int MaxStaff { get; set; }
        //public int CurrentStaffCount { get; set; }
        public List<string>? EnabledFeatures { get; set; }
        public string? PrimaryAdminUserId { get; set; }
        public string? PrimaryContactName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "System";
        public string UpdatedBy { get; set; } = "System";
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public string? Notes { get; set; }
    }

    public class DeviceInfo
    {
        public string? DeviceId { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsCurrentDevice { get; set; }
    }
    public class LoggedUser
    {
        public string? userId { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
    }

    public class ExternalLoginsModel
    {
        public IList<UserLoginInfo>? CurrentLogins { get; set; }

        public IList<ExternalAuthenticationProvider>? OtherLogins { get; set; }
    }

    public class UserAuthenticationInfo
    {
        public string? UserId { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool HasPassword { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? TwoFactorType { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public int RecoveryCodesLeft { get; set; }
        public IList<UserLoginInfo>? Logins { get; set; }
        public IList<AuthenticationScheme>? OtherLogins { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class UserClaims
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? ClientId { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? BirthDate { get; set; }
        public string? Picture { get; set; }
    }

    public class UserDetail
    {
        public UserDetail()
        {
            Roles = new List<string>();
        }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Picture { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool HasPassword { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFactorType { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public List<string> Roles { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
    }

    public static class Claims
    {
        public const string Id = "id";
        public const string Name = "name";
        public const string FirstName = "firstname";
        public const string LastName = "lastname";
        public const string Email = "email";
        public const string Role = "role";
        public const string ClientId = "client_id";
        public const string Address = "address";
        public const string Gender = "gender";
        public const string BirthDate = "birthdate";
        public const string Picture = "picture";
    }
}
