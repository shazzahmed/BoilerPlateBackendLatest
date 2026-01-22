
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
            Parent,
            Student,
            Teacher,
            Accountant,
            Receptionist,
        }
        public enum UserTypes
        {
            Student = 1,
            Parent = 2,
        }
        public enum SubjectTypes
        {
            Theory,
            Practical,
        }
        public enum FeeFrequency
        {
            OneTime = 0,
            Annually = 1,
            HalfYearly = 2,
            Quaterly = 3,
            Monthly = 4,
            HalfMonthly = 5,
            Weekly = 6,
            Daily = 7,
        }
        public enum FinePolicyType
        {
            None,
            Fixed,
            Percentage
        }
        public enum DiscountType
        {
            Fixed,
            Percentage
        }
        public enum FeeStatus
        {
            Pending,
            Partial,
            Paid,
            Overdue
        }
        public enum TransactionStatus
        {
            Pending,
            Completed,
            Failed,
            Refunded
        }
        public enum AttendanceType
        {
            Present,
            Absent,
            NotMarked,
            Late,
            HalfDay,
            Holiday,
            Excused,
        }
        public enum MessageType
        {
            Error,
            Info,
            Success,
            Warning
        }
        public enum MenuEnum
        {
            Dashboard = 1,
            Students,
            Staff,
            Teachers,
            Department,
            Cources,
            Settings,
        }
        public enum MenuType
        {
            Admin,
            School,
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
        public enum DashboardTypes
        {
            Ecommerce = 1,
            Inventory = 2,
            Both = 3,
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
            ConfirmClientAgency = 13,
            
            // Payment & Fee Management Templates
            EmailPaymentReceipt = 14,
            SmsPaymentReceipt = 15,
            EmailPaymentConfirmation = 16,
            SmsPaymentConfirmation = 17,
            EmailOverdueReminder = 18,
            SmsOverdueReminder = 19,
            EmailFeeDueReminder = 20,
            SmsFeeDueReminder = 21,
            EmailFineWaiverApproved = 22,
            SmsFineWaiverApproved = 23,
            EmailFineWaiverRejected = 24,
            SmsFineWaiverRejected = 25,
        }

        // Exam Module Enums
        public enum ExamType
        {
            MidTerm = 1,
            FinalTerm = 2,
            Monthly = 3,
            Quiz = 4,
            Mock = 5,
            Annual = 6,
            Weekly = 7,
            Unit = 8
        }

        public enum ExamStatus
        {
            Draft = 1,
            Scheduled = 2,
            InProgress = 3,
            Completed = 4,
            Published = 5,
            Cancelled = 6
        }

        public enum Term
        {
            First = 1,
            Second = 2,
            Third = 3,
            Annual = 4
        }

        public enum MarksStatus
        {
            Draft = 1,
            Submitted = 2,
            Verified = 3,
            Published = 4
        }
    }
}
