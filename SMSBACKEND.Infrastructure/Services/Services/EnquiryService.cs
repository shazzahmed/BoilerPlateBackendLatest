
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Common.DTO;
using Hangfire;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Text;

namespace Infrastructure.Services.Services
{

    public class EnquiryService : BaseService<EnquiryModel, Enquiry, int>, IEnquiryService
    {
        private readonly IEnquiryRepository _enquiryRepository;
        private readonly IBackgroundJobClient _jobs;
        private readonly ILogger<EnquiryService> _logger;
        
        // In-memory storage for import jobs (in production, use Redis or database)
        private static readonly Dictionary<string, ImportJobStatus> _importJobs = new Dictionary<string, ImportJobStatus>();
        
        public EnquiryService(
            IMapper mapper, 
            IEnquiryRepository enquiryRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            IBackgroundJobClient jobs,
            ILogger<EnquiryService> logger
            ) : base(mapper, enquiryRepository, unitOfWork, sseService, cacheProvider)
        {
            _enquiryRepository = enquiryRepository;
            _jobs = jobs;
            _logger = logger;
        }

        // Bulk import methods
        public async Task<BaseModel> ValidateBulkImportAsync(BulkEnquiryImportRequest request)
        {
            try
            {
                var jobId = Guid.NewGuid().ToString();
                
                // Initialize job status
                _importJobs[jobId] = new ImportJobStatus
                {
                    JobId = jobId,
                    JobType = "EnquiryValidation",
                    Status = "Validating",
                    TotalRecords = request.Enquiries.Count,
                    ProcessedRecords = 0,
                    SuccessfulRecords = 0,
                    FailedRecords = 0,
                    StartTime = DateTime.UtcNow,
                    Errors = new List<string>()
                };

                // Validate all records
                await ValidateEnquiryImportDataAsync(request.Enquiries, jobId);

                var validationResults = request.Enquiries.Select(e => new
                {
                    RowNumber = e.RowNumber,
                    IsValid = e.IsValid,
                    Errors = e.ValidationErrors,
                    Data = e
                }).ToList();

                return BaseModel.Succeed(validationResults, validationResults.Count, "Validation completed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating enquiries: {ex.Message}");
                return BaseModel.Failed($"Error validating enquiries: {ex.Message}");
            }
        }

        public async Task<BaseModel> ImportEnquiriesAsync(BulkEnquiryImportRequest request)
        {
            try
            {
                var jobId = Guid.NewGuid().ToString();
                
                // Initialize job status
                _importJobs[jobId] = new ImportJobStatus
                {
                    JobId = jobId,
                    JobType = "EnquiryImport",
                    Status = "Validating",
                    TotalRecords = request.Enquiries.Count,
                    ProcessedRecords = 0,
                    SuccessfulRecords = 0,
                    FailedRecords = 0,
                    StartTime = DateTime.UtcNow,
                    Errors = new List<string>()
                };

                // Validate all records first
                await ValidateEnquiryImportDataAsync(request.Enquiries, jobId);

                // If validation only, return results
                if (request.ValidateOnly)
                {
                    var validationResults = request.Enquiries.Select(e => new
                    {
                        RowNumber = e.RowNumber,
                        IsValid = e.IsValid,
                        Errors = e.ValidationErrors,
                        Data = e
                    }).ToList();

                    return BaseModel.Succeed(validationResults, validationResults.Count, "Validation completed");
                }

                // Start background processing
                _jobs.Enqueue(() => ProcessEnquiryImportAsync(request, jobId));

                return BaseModel.Succeed(new { JobId = jobId }, 0, "Import job started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting enquiry import: {ex.Message}");
                return BaseModel.Failed($"Error starting import: {ex.Message}");
            }
        }

        public async Task<BaseModel> GetImportStatusAsync(string jobId)
        {
            if (_importJobs.TryGetValue(jobId, out var jobStatus))
            {
                return BaseModel.Succeed(jobStatus, 0, "Import status retrieved");
            }

            return BaseModel.Failed("Import job not found");
        }

        public async Task<BaseModel> GetImportHistoryAsync()
        {
            var history = _importJobs.Values.OrderByDescending(j => j.StartTime).ToList();
            return BaseModel.Succeed(history, history.Count, "Import history retrieved");
        }

        public async Task<byte[]> DownloadTemplateAsync()
        {
            // Create a comprehensive CSV template with detailed instructions
            var templateData = new StringBuilder();
            
            // Header with instructions
            templateData.AppendLine("# ENQUIRY IMPORT TEMPLATE");
            templateData.AppendLine("# Instructions:");
            templateData.AppendLine("# 1. Fill in the data below (remove these instruction lines)");
            templateData.AppendLine("# 2. Required fields: FullName, ApplyingClass");
            templateData.AppendLine("# 3. Date format: YYYY-MM-DD (e.g., 2010-05-15)");
            templateData.AppendLine("# 4. Phone format: +1234567890 or 1234567890");
            templateData.AppendLine("# 5. Email format: user@domain.com");
            templateData.AppendLine("# 6. Class numbers: 1-12 (or your school's class structure)");
            templateData.AppendLine("# 7. Status options: New, Contacted, Interested, Not Interested, Enrolled");
            templateData.AppendLine("# 8. Source options: Website, Referral, Advertisement, Walk-in, Other");
            templateData.AppendLine("");
            
            // Column headers (using lowercase for consistency with frontend parsing)
            templateData.AppendLine("FullName,Dob,ApplyingClass,Phone,Email,Address,Source,Notes,Status");
            
            // Sample data with various scenarios
            templateData.AppendLine("John Doe,2010-05-15,1,+1234567890,john.doe@email.com,\"123 Main St, City\",Website,\"Interested in admission, visited school\",New");
            templateData.AppendLine("Jane Smith,2011-03-20,2,+1234567891,jane.smith@email.com,\"456 Oak Ave, City\",Referral,\"Previous student's sibling\",Contacted");
            templateData.AppendLine("Mike Johnson,2009-12-10,3,9876543210,mike.j@email.com,\"789 Pine St, City\",Advertisement,\"Saw school ad in newspaper\",Interested");
            templateData.AppendLine("Sarah Wilson,2012-07-08,1,,sarah.wilson@email.com,\"321 Elm St, City\",Walk-in,\"Visited school with parents\",New");
            templateData.AppendLine("David Brown,2010-11-25,2,+1555123456,,,\"456 Maple Ave, City\",Other,\"Recommended\",New");

            return System.Text.Encoding.UTF8.GetBytes(templateData.ToString());
        }

        // Private helper methods
        private async Task ValidateEnquiryImportDataAsync(List<EnquiryImportRow> enquiries, string jobId)
        {
            var jobStatus = _importJobs[jobId];
            
            foreach (var enquiry in enquiries)
            {
                var errors = new List<string>();

                // Required field validations
                if (string.IsNullOrWhiteSpace(enquiry.FullName))
                    errors.Add("Full Name is required");

                if (enquiry.ApplyingClass <= 0)
                    errors.Add("Applying Class is required and must be valid");

                // Date validations
                if (enquiry.Dob.HasValue && enquiry.Dob.Value > DateTime.Now)
                    errors.Add("Date of Birth cannot be in the future");

                // Email validation
                if (!string.IsNullOrWhiteSpace(enquiry.Email) && !IsValidEmail(enquiry.Email))
                    errors.Add("Invalid email format");

                // Phone validation
                if (!string.IsNullOrWhiteSpace(enquiry.Phone) && !IsValidPhone(enquiry.Phone))
                    errors.Add("Invalid phone number format");

                enquiry.ValidationErrors = errors;
                enquiry.IsValid = errors.Count == 0;

                if (!enquiry.IsValid)
                {
                    jobStatus.FailedRecords++;
                    jobStatus.Errors.AddRange(errors.Select(e => $"Row {enquiry.RowNumber}: {e}"));
                }
                else
                {
                    jobStatus.SuccessfulRecords++;
                }

                jobStatus.ProcessedRecords++;
            }

            jobStatus.Status = jobStatus.FailedRecords > 0 ? "ValidationFailed" : "Validated";
            jobStatus.EndTime = DateTime.UtcNow;
        }

        public async Task ProcessEnquiryImportAsync(BulkEnquiryImportRequest request, string jobId)
        {
            if (_importJobs.Count > 0)
            {
                var jobStatus = _importJobs[jobId];
                jobStatus.Status = "Processing";
                jobStatus.ProcessedRecords = 0;
                jobStatus.SuccessfulRecords = 0;
                jobStatus.FailedRecords = 0;

                try
                {
                    var validEnquiries = request.Enquiries.Where(e => e.IsValid).ToList();
                    const int batchSize = 50; // Process 50 records at a time
                    var totalBatches = (int)Math.Ceiling((double)validEnquiries.Count / batchSize);

                    for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                    {
                        var batch = validEnquiries.Skip(batchIndex * batchSize).Take(batchSize).ToList();

                        try
                        {
                            await ProcessBatchAsync(batch, jobStatus, batchIndex + 1, totalBatches);

                            // Small delay between batches to prevent database overload
                            if (batchIndex < totalBatches - 1)
                            {
                                await Task.Delay(100); // 100ms delay between batches
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error processing batch {batchIndex + 1}: {ex.Message}");
                            jobStatus.Errors.Add($"Batch {batchIndex + 1} failed: {ex.Message}");

                            // Mark all records in this batch as failed
                            foreach (var enquiry in batch)
                            {
                                jobStatus.FailedRecords++;
                                jobStatus.ProcessedRecords++;
                            }
                        }
                    }

                    jobStatus.Status = "Completed";
                    jobStatus.EndTime = DateTime.UtcNow;
                    _logger.LogInformation($"Import completed. Success: {jobStatus.SuccessfulRecords}, Failed: {jobStatus.FailedRecords}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Import process failed: {ex.Message}");
                    jobStatus.Status = "Failed";
                    jobStatus.Errors.Add($"Import failed: {ex.Message}");
                    jobStatus.EndTime = DateTime.UtcNow;
                }
            }
        }

        private async Task ProcessBatchAsync(List<EnquiryImportRow> batch, ImportJobStatus jobStatus, int batchNumber, int totalBatches)
        {
            
            var enquiryModels = new List<EnquiryModel>();
            
            // Convert batch to models
            foreach (var enquiryRow in batch)
            {
                var enquiryModel = new EnquiryModel
                {
                    FullName = enquiryRow.FullName,
                    Dob = enquiryRow.Dob,
                    ApplyingClass = enquiryRow.ApplyingClass,
                    Phone = enquiryRow.Phone,
                    Email = enquiryRow.Email,
                    Address = enquiryRow.Address,
                    Source = enquiryRow.Source,
                    Notes = enquiryRow.Notes,
                    Status = enquiryRow.Status,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                enquiryModels.Add(enquiryModel);
            }

            try
            {
                // Use batch insert for better performance
                var results = await AddRange(enquiryModels);
                
                if (results != null && results.Count > 0)
                {
                    jobStatus.SuccessfulRecords += results.Count;
                }
                else
                {
                    // If batch insert fails, try individual inserts
                    await ProcessBatchIndividuallyAsync(batch, jobStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Batch insert failed for batch {batchNumber}, falling back to individual processing: {ex.Message}");
                // Fallback to individual processing
                await ProcessBatchIndividuallyAsync(batch, jobStatus);
            }

            jobStatus.ProcessedRecords += batch.Count;
        }

        private async Task ProcessBatchIndividuallyAsync(List<EnquiryImportRow> batch, ImportJobStatus jobStatus)
        {
            foreach (var enquiryRow in batch)
            {
                try
                {
                    var enquiryModel = new EnquiryModel
                    {
                        FullName = enquiryRow.FullName,
                        Dob = enquiryRow.Dob,
                        ApplyingClass = enquiryRow.ApplyingClass,
                        Phone = enquiryRow.Phone,
                        Email = enquiryRow.Email,
                        Address = enquiryRow.Address,
                        Source = enquiryRow.Source,
                        Notes = enquiryRow.Notes,
                        Status = enquiryRow.Status,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var result = await Add(enquiryModel);
                    
                    if (result != null)
                    {
                        jobStatus.SuccessfulRecords++;
                    }
                    else
                    {
                        jobStatus.FailedRecords++;
                        jobStatus.Errors.Add($"Row {enquiryRow.RowNumber}: Failed to create enquiry");
                    }
                }
                catch (Exception ex)
                {
                    jobStatus.FailedRecords++;
                    jobStatus.Errors.Add($"Row {enquiryRow.RowNumber}: {ex.Message}");
                    _logger.LogError($"Error processing enquiry row {enquiryRow.RowNumber}: {ex.Message}");
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Basic phone validation - can be enhanced
            return !string.IsNullOrWhiteSpace(phone) && phone.Length >= 10;
        }

        // ==================== WORKFLOW INTEGRATION METHODS ====================

        /// <summary>
        /// Process imported enquiries through the workflow based on their status
        /// </summary>
        public async Task<BaseModel> ProcessImportedEnquiriesWorkflowAsync(List<int> enquiryIds, string processedBy)
        {
            try
            {
                var processedCount = 0;
                var errors = new List<string>();

                foreach (var enquiryId in enquiryIds)
                {
                    try
                    {
                        var enquiry = await FirstOrDefaultAsync(e => e.Id == enquiryId);
                        if (enquiry != null)
                        {
                            await ProcessEnquiryWorkflowStepAsync(enquiry, processedBy);
                            processedCount++;
                        }
                        else
                        {
                            errors.Add($"Enquiry with ID {enquiryId} not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing enquiry {enquiryId} through workflow: {ex.Message}");
                        errors.Add($"Enquiry {enquiryId}: {ex.Message}");
                    }
                }

                var message = $"Processed {processedCount} enquiries through workflow";
                if (errors.Any())
                {
                    message += $". {errors.Count} errors occurred: {string.Join(", ", errors)}";
                }

                return BaseModel.Succeed(new { ProcessedCount = processedCount, Errors = errors }, processedCount, message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing imported enquiries through workflow: {ex.Message}");
                return BaseModel.Failed($"Failed to process enquiries through workflow: {ex.Message}");
            }
        }

        /// <summary>
        /// Process a single enquiry through the appropriate workflow step
        /// </summary>
        private async Task ProcessEnquiryWorkflowStepAsync(EnquiryModel enquiry, string processedBy)
        {
            switch (enquiry.Status?.ToLower())
            {
                case "contacted":
                    await ProcessContactedEnquiryAsync(enquiry, processedBy);
                    break;
                case "interested":
                    await ProcessInterestedEnquiryAsync(enquiry, processedBy);
                    break;
                case "enrolled":
                    await ProcessEnrolledEnquiryAsync(enquiry, processedBy);
                    break;
                default:
                    // For "new" status or unknown status, just log
                    _logger.LogInformation($"Enquiry {enquiry.Id} with status '{enquiry.Status}' - no workflow processing required");
                    break;
            }
        }

        /// <summary>
        /// Process contacted enquiry - could trigger follow-up notifications
        /// </summary>
        private async Task ProcessContactedEnquiryAsync(EnquiryModel enquiry, string processedBy)
        {
            // Add any specific logic for contacted enquiries
            // For example: schedule follow-up, send confirmation email, etc.
            _logger.LogInformation($"Processing contacted enquiry {enquiry.Id} by {processedBy}");
            
            // You could add notification logic here
            // await _notificationService.SendContactedConfirmationAsync(enquiry);
        }

        /// <summary>
        /// Process interested enquiry - could create pre-admission entry
        /// </summary>
        private async Task ProcessInterestedEnquiryAsync(EnquiryModel enquiry, string processedBy)
        {
            _logger.LogInformation($"Processing interested enquiry {enquiry.Id} by {processedBy}");
            
            // For interested enquiries, you might want to:
            // 1. Create a pre-admission entry
            // 2. Send admission information
            // 3. Schedule an interview
            
            // Example: Create pre-admission if not exists
            // await _preAdmissionService.CreateFromEnquiryAsync(enquiry.Id, processedBy);
        }

        /// <summary>
        /// Process enrolled enquiry - complete the workflow
        /// </summary>
        private async Task ProcessEnrolledEnquiryAsync(EnquiryModel enquiry, string processedBy)
        {
            _logger.LogInformation($"Processing enrolled enquiry {enquiry.Id} by {processedBy}");
            
            // For enrolled enquiries, you might want to:
            // 1. Create student record
            // 2. Generate admission number
            // 3. Send enrollment confirmation
            // 4. Update enquiry status to "Completed"
            
            // Example: Update enquiry status
            enquiry.Status = "Completed";
            await Update(enquiry);
        }

        /// <summary>
        /// Get workflow statistics for imported enquiries
        /// </summary>
        public async Task<BaseModel> GetImportedEnquiriesWorkflowStatsAsync()
        {
            try
            {
                var stats = new
                {
                    TotalImported = await GetCount(e => e.Source == "Import"),
                    ByStatus = new
                    {
                        New = await GetCount(e => e.Source == "Import" && e.Status == "New"),
                        Contacted = await GetCount(e => e.Source == "Import" && e.Status == "Contacted"),
                        Interested = await GetCount(e => e.Source == "Import" && e.Status == "Interested"),
                        Enrolled = await GetCount(e => e.Source == "Import" && e.Status == "Enrolled"),
                        Completed = await GetCount(e => e.Source == "Import" && e.Status == "Completed")
                    },
                    RecentImports = await GetRecentImportedEnquiriesAsync()
                };

                return BaseModel.Succeed(stats, 0, "Workflow statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting imported enquiries workflow stats: {ex.Message}");
                return BaseModel.Failed($"Failed to get workflow statistics: {ex.Message}");
            }
        }

        /// <summary>
        /// Get recently imported enquiries for workflow monitoring
        /// </summary>
        private async Task<object> GetRecentImportedEnquiriesAsync()
        {
            var recentImports = await Get<object>(
                pageNumber: 1,
                pageSize: 10,
                where: e => e.Source == "Import",
                orderBy: q => q.OrderByDescending(e => e.CreatedAt)
            );

            return recentImports.items.Select(e => new
            {
                Id = ((EnquiryModel)e).Id,
                FullName = ((EnquiryModel)e).FullName,
                Status = ((EnquiryModel)e).Status,
                CreatedAt = ((EnquiryModel)e).CreatedAt,
                WorkflowProgress = GetWorkflowProgress((EnquiryModel)e)
            }).ToList();
        }

        /// <summary>
        /// Calculate workflow progress for an enquiry
        /// </summary>
        private string GetWorkflowProgress(EnquiryModel enquiry)
        {
            return enquiry.Status?.ToLower() switch
            {
                "new" => "0% - Just Started",
                "contacted" => "25% - Initial Contact",
                "interested" => "50% - Pre-Admission",
                "enrolled" => "75% - Admission Process",
                "completed" => "100% - Enrolled",
                _ => "Unknown Status"
            };
        }
    }
}
