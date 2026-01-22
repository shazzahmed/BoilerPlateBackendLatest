using System;
using System.Text.Json;

namespace Common.Helpers
{
    public static class AuditTrailHelper
    {
        public static string LogStudentOperation(string operation, string userId, object oldData = null, object newData = null, string additionalInfo = null)
        {
            var auditLog = new
            {
                Timestamp = DateTime.UtcNow,
                Operation = operation,
                UserId = userId,
                OldData = oldData,
                NewData = newData,
                AdditionalInfo = additionalInfo,
                CorrelationId = Guid.NewGuid().ToString()
            };

            // TODO: Implement actual logging to database or file system
            // For now, return the correlation ID for tracing
            return auditLog.CorrelationId;
        }

        public static string LogStudentCreate(string userId, object studentData)
        {
            return LogStudentOperation("CREATE", userId, null, studentData, "Student created successfully");
        }

        public static string LogStudentUpdate(string userId, object oldData, object newData)
        {
            return LogStudentOperation("UPDATE", userId, oldData, newData, "Student updated successfully");
        }

        public static string LogStudentDelete(string userId, object studentData)
        {
            return LogStudentOperation("DELETE", userId, studentData, null, "Student soft deleted successfully");
        }

        public static string LogStudentRestore(string userId, object studentData)
        {
            return LogStudentOperation("RESTORE", userId, null, studentData, "Student restored successfully");
        }
    }
}

