namespace Common.DTO.Response
{
    public class BulkImportResponse
    {
        public string JobId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "Started", "Processing", "Completed", "Failed"
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public object? ValidationResults { get; set; } // For validation-only requests
    }

    public class ImportJobStatus
    {
        public string JobId { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "Validating", "Validated", "Processing", "Completed", "Failed"
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<ImportError> DetailedErrors { get; set; } = new List<ImportError>();
        public string CurrentProcessingRecord { get; set; } = string.Empty;
        public double ProgressPercentage => TotalRecords > 0 ? (double)ProcessedRecords / TotalRecords * 100 : 0;
    }

    public class ImportError
    {
        public int RowNumber { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
    }

    public class ImportHistoryItem
    {
        public string JobId { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
    }
}

