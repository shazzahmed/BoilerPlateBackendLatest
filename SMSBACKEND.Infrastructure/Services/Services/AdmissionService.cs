
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using Common.DTO;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Services
{

    public class AdmissionService : BaseService<AdmissionModel, Admission, int>, IAdmissionService
    {
        private readonly IAdmissionRepository _admissionRepository;
        private readonly IPreAdmissionRepository _preAdmissionRepository;
        private readonly IEnquiryRepository _enquiryRepository;
        
        public AdmissionService(
            IMapper mapper, 
            IAdmissionRepository admissionRepository,
            IPreAdmissionRepository preAdmissionRepository,
            IEnquiryRepository enquiryRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<AdmissionService> logger
            ) : base(mapper, admissionRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _admissionRepository = admissionRepository;
            _preAdmissionRepository = preAdmissionRepository;
            _enquiryRepository = enquiryRepository;
        }

        public async Task<BaseModel> CreateAdmissionAsync(AdmissionModel model)
        {
            // Validate that no admission already exists for this entry test
            var existingAdmission = await _admissionRepository.FirstOrDefaultAsync(
                x => x.EntryTestId == model.EntryTestId && !x.IsDeleted);
            
            if (existingAdmission != null)
            {
                return BaseModel.Failed($"Admission already exists for Entry Test ID {model.EntryTestId}");
            }

            // Generate sequential admission number if not provided
            if (string.IsNullOrEmpty(model.AdmissionNumber))
            {
                model.AdmissionNumber = await GetNextAdmissionNumberAsync();
            }

            try
            {
                var result = await Add(model);
                return BaseModel.Succeed(result, 0, "Admission created successfully");
            }
            catch (Exception ex)
            {
                return BaseModel.Failed($"Error creating admission: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if student can be created for this admission
        /// </summary>
        public async Task<bool> CanCreateStudent(int admissionId)
        {
            var admission = await _admissionRepository.FirstOrDefaultAsync(
                x => x.Id == admissionId && !x.IsDeleted);
            return admission != null && !admission.IsStudentCreated;
        }

        /// <summary>
        /// Mark admission as having student created
        /// </summary>
        public async Task<BaseModel> MarkStudentCreated(int admissionId, int studentId, string AdmissionNo)
        {
            try
            {
                // Load admission with Student navigation property to get student name
                var admission = await _admissionRepository.FirstOrDefaultAsync(
                    x => x.Id == admissionId && !x.IsDeleted,
                    includeProperties: x => x.Include(a => a.Student)
                        .ThenInclude(s => s.StudentInfo));
                
                if (admission == null)
                {
                    return BaseModel.Failed("Admission not found");
                }

                admission.IsStudentCreated = true;
                admission.StudentId = studentId;
                admission.AdmissionNumber = AdmissionNo;
                
                // ✅ Update the admission (this will trigger AutoMapper mapping)
                var admissionModel = mapper.Map<AdmissionModel>(admission);
                
                // ✅ StudentName will be mapped by AutoMapper from Student.FullName
                await Update(admissionModel);

                _logger.LogInformation($"[AdmissionService] Marked admission {admissionId} as student created (StudentId: {studentId})");
                return BaseModel.Succeed(null, 0, "Student marked as created for admission");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionService] Error marking student as created: {ex.Message}");
                return BaseModel.Failed($"Error marking student as created: {ex.Message}");
            }
        }

        /// <summary>
        /// Get next sequential admission number based on last student admission number
        /// </summary>
        public async Task<string> GetNextAdmissionNumberAsync()
        {
            try
            {
                // Get the last admission number from the database
                var lastAdmission = await _admissionRepository.FirstOrDefaultAsync(
                    x => !x.IsDeleted && !string.IsNullOrEmpty(x.AdmissionNumber),
                    x => x.OrderByDescending(a => a.AdmissionNumber));

                if (lastAdmission == null || string.IsNullOrEmpty(lastAdmission.AdmissionNumber))
                {
                    return "ADM-2024-001";
                }

                // Extract number from last admission and increment
                var parts = lastAdmission.AdmissionNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int number))
                {
                    var year = parts[1];
                    return $"ADM-{year}-{(number + 1):D3}";
                }

                // Fallback to current year if format is unexpected
                var currentYear = DateTime.Now.Year.ToString();
                return $"ADM-{currentYear}-001";
            }
            catch (Exception ex)
            {
                // Fallback in case of error
                var currentYear = DateTime.Now.Year.ToString();
                return $"ADM-{currentYear}-001";
            }
        }

        /// <summary>
        /// Get admission by ID with student information
        /// </summary>
        public async Task<BaseModel> GetAdmissionWithStudent(int admissionId)
        {
            try
            {
                var admission = await _admissionRepository.FirstOrDefaultAsync(
                    x => x.Id == admissionId && !x.IsDeleted,
                    includeProperties: x => x.Include(a => a.Student)
                        .ThenInclude(s => s.StudentInfo)
                        .Include(a => a.Class)
                        .Include(a => a.Section)
                        .Include(a => a.EntryTest));

                if (admission == null)
                {
                    return BaseModel.Failed("Admission not found");
                }

                var admissionModel = mapper.Map<AdmissionModel>(admission);
                
                // Set student name if student exists
                //if (admission.IsStudentCreated && admission.Student?.StudentInfo != null)
                //{
                //    admissionModel.StudentName = $"{admission.Student.StudentInfo.FirstName} {admission.Student.StudentInfo.LastName}".Trim();
                //}

                return BaseModel.Succeed(admissionModel, 1, "Admission retrieved successfully");
            }
            catch (Exception ex)
            {
                return BaseModel.Failed($"Error retrieving admission: {ex.Message}");
            }
        }

        /// <summary>
        /// Get admission with workflow data for student creation
        /// </summary>
        public async Task<BaseModel> GetAdmissionWithWorkflowData(int admissionId)
        {
            try
            {
                // Load admission with all navigation properties
                var admission = await _admissionRepository.FirstOrDefaultAsync(
                    x => x.Id == admissionId && !x.IsDeleted,
                    includeProperties: x => x
                        .Include(a => a.EntryTest)
                        .Include(a => a.Class)
                        .Include(a => a.Section));

                if (admission == null)
                {
                    return BaseModel.Failed("Admission not found");
                }

                // ✅ Manually load Application and Enquiry to avoid JsonIgnore issues
                PreAdmission? application = null;
                Enquiry? enquiry = null;

                if (admission.EntryTest != null)
                {
                    // Get Application (PreAdmission) from EntryTest using injected repository
                    application = await _preAdmissionRepository.FirstOrDefaultAsync(
                        x => x.Id == admission.EntryTest.ApplicationId && !x.IsDeleted);

                    if (application != null)
                    {
                        // Get Enquiry from Application using injected repository
                        enquiry = await _enquiryRepository.FirstOrDefaultAsync(
                            x => x.Id == application.EnquiryId && !x.IsDeleted,
                            includeProperties: e => e.Include(enq => enq.Class));
                    }
                }

                // Map entities to models
                var admissionModel = mapper.Map<AdmissionModel>(admission);
                var applicationModel = application != null ? mapper.Map<PreAdmissionModel>(application) : null;
                var enquiryModel = enquiry != null ? mapper.Map<EnquiryModel>(enquiry) : null;
                var entryTestModel = admission.EntryTest != null ? mapper.Map<EntryTestModel>(admission.EntryTest) : null;
                
                // ✅ Build workflow data with properly mapped models
                var workflowData = new
                {
                    Admission = admissionModel,
                    Enquiry = enquiryModel,
                    Application = applicationModel,
                    EntryTest = entryTestModel,
                    Class = admission.Class,
                    Section = admission.Section
                };

                _logger.LogInformation($"[AdmissionService] Workflow data prepared for admission {admissionId}");
                return BaseModel.Succeed(workflowData, 1, "Admission with workflow data retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionService] Error retrieving admission with workflow data: {ex.Message}");
                return BaseModel.Failed($"Error retrieving admission with workflow data: {ex.Message}");
            }
        }
    }
}

