namespace Common.Utilities.Constants
{
    public static class UserStatusCode
    {
        public const string Preactive = "101";
        public const string Active = "102";
        public const string Inactive = "103";
        public const string Canceled = "104";
        public const string Frozen = "105";
        public const string Blocked = "106";
    }

    public static class NotificationStatusCode
    {
        public const string Created = "201";
        public const string Queued = "202";
        public const string Succeeded = "203";
        public const string Failed = "204";
    }
    public static class LeadStatusCode
    {
        public const string Visit = "1001";
        public const string Pipeline = "1002";
        public const string Intrested = "1003";
        public const string FollowUp = "1004";
        public const string NotIntrested = "1005";
        public const string BudgetOut = "1006";
        public const string LocationIssue = "1007";
        public const string Agent = "1008";
        public const string Pending = "1009";
        public const string WrongNo = "1010";
        public const string Done = "1011";
    }
}
