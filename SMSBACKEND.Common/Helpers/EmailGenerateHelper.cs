using System.Collections.ObjectModel;

namespace Common.Helpers
{
    public static class EmailGenerateHelper
    {
        public static string GenerateEmail(string name, string domain = "yourdomain.com")
        {
            var baseEmail = name.Replace(" ", "").ToLower();
            // Optionally add a unique suffix here
            return $"{baseEmail}@{domain}";
        }
    }
}
