
namespace Common.Utilities
{
    public class Enums
    {
        public enum ApplicationType
        {
            CoreApi = 1,
            Web = 2,
            BackgroundJob = 3
        }

        public enum UserStatus
        {
            Active = 1,
            Preactive = 2,
            Inactive = 3,
            Canceled = 4,
            Frozen = 5,
            Blocked = 6,
            Created = 7,
            Queued =8,
            Succeeded = 9,
            Failed = 10,
        }
        public enum UserRoles
        {
            SuperAdmin = 1,
            Admin,
            User,
        }
        public enum MessageType
        {
            Error,
            Info,
            Success,
            Warning
        }
        public enum MenuType
        {
            Admin,
            SubMenu,
            Feature,
        }
        public enum Gender
        {
            Male,
            Female
        }

        public enum ResponseTypes
        {
            SUCCESS,
            FAILURE,
        }

        public enum StatusTypes
        {
            UserStatus = 1,
        }
        public enum ResponseType
        {
            Success,
            Error
        }
        public enum LoginStatus
        {
            Locked = 0,
            AccountLocked,
            EmailNotConfirmed,
            UserInActive,
            InvalidCredential,
            Succeded,
            TimeoutLocked,
            Failed,
            RequiresTwoFactor,
            SomeThingWentWrong,
            UserAlreadyExits,
            InvalidPassword,
            InvalidOldPassword,
            UserNotFound,
            UnAuthorized,
            NotValid,
            PleaseEnterFile,
            YourTokenIsExpire,
            Success,
        }
        public enum NotificationTypes
        {
            Email = 1,
            Sms = 2,
            Site = 3,
            Push = 4
        }

        public enum NotificationTemplates
        {
            EmailUserRegisteration = 1,
            SmsUserRegisteration = 2,
            EmailForgotPassword = 3,
            SmsForgotPassword = 4,
            EmailSetPassword = 5,
            SmsSetPassword = 6,
            EmailChangePassword = 7,
            SmsChangePassword = 8,
            EmailTwoFactorToken = 9,
            SmsTwoFactorToken = 10,
            EmailUserStatusChange = 11,
            SmsUserStatusChange = 12,
            ConfirmClientAgency = 13
        }
    }
}
