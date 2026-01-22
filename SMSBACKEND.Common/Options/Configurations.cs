using System;
using System.Text;
using System.Collections.Generic;

namespace Common.Options
{
    public class AppOptions
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string WebUrl { get; set; } = string.Empty;
    }
    public class JwtOptions
    {
        public string SecretKey { get; set; }
        public int ExpiryMinutes { get; set; }
        public string Issuer { get; set; }
    }
    public class RedisOptions
    {
        public string ConnectionString { get; set; }
    }
    public class BackendNET6
    {
        public string WebApiUrl { get; set; }
    }

    #region ComponentOptions
    public class ComponentOptions
    {
        public Security? Security { get; set; }
        //public string Communication { get; set; }
        public Communication? Communication { get; set; }
    }

    public class Security
    {
        public string SecurityService { get; set; } = string.Empty;
        public string EncryptionService { get; set; } = string.Empty;
    }

    public class Communication
    {
        public string EmailService { get; set; } = string.Empty;
        public string SmsService { get; set; } = string.Empty;
        public bool TestMode { get; set; } = false;
        public string TestEmail { get; set; } = string.Empty;
    }
    #endregion ComponentOptions


    public class InfrastructureOptions
    {
        public string Documentation { get; set; } = string.Empty;
    }

    public class SecurityOptions
    {
        public int PasswordLength { get; set; }
        public int AccountLockoutTimeSpan { get; set; }
        public int AccountLoginMaximumAttempts { get; set; }
        public int PreviousPasswordValidationLimit { get; set; }
        public string Authority { get; set; } = string.Empty;
        public string RequiredScope { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string AuthenticatorUriFormat { get; set; } = string.Empty;
        public int NumberOfRecoveryCodes { get; set; }
        public string Scopes { get; set; } = string.Empty;
        public string AdminUsername { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
        public int EncryptionIterationSize { get; set; }
        public string EncryptionPassword { get; set; } = string.Empty;
        public string EncryptionSaltKey { get; set; } = string.Empty;
        public string EncryptionVIKey { get; set; } = string.Empty;
        public bool MicrosoftAuthenticationAdded { get; set; }
        public bool GoogleAuthenticationAdded { get; set; }
        public bool ZohoAuthenticationAdded { get; set; }
        public bool TwitterAuthenticationAdded { get; set; }
        public bool FacebookAuthenticationAdded { get; set; }
    }

    public class GoogleOptions
    {
        public string FromName { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }

    public class ZohoOptions
    {
        public string FromName { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }

    public class OutlookOptions
    {
        public string FromName { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string ApplicationId { get; set; } = string.Empty;
        public string ApplicationSecret { get; set; } = string.Empty;
    }

    public class FacebookOptions
    {
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty;
    }

    public class TwitterOptions
    {
        public string ConsumerKey { get; set; } = string.Empty;
        public string ConsumerSecret { get; set; } = string.Empty;
    }
}
