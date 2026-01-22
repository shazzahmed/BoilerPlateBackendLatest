using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Application.ServiceContracts;
using Common.DTO.Response;
using Common.DTO.Request;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using static Common.Utilities.Enums;

namespace Infrastructure.Services.Services
{
    public class ExamMarksService : BaseService<ExamMarksModel, ExamMarks, int>, IExamMarksService
    {
        private readonly IExamMarksRepository _examMarksRepository;

        public ExamMarksService(
            IMapper mapper,
            IExamMarksRepository examMarksRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<ExamMarksService> logger
            ) : base(mapper, examMarksRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _examMarksRepository = examMarksRepository;
        }

        public async Task<BaseModel> GetMarksByExamSubjectAsync(int examSubjectId)
        {
            var marks = await _examMarksRepository.GetMarksByExamSubjectAsync(examSubjectId);
            return new BaseModel { Success = true, Data = marks, Total = marks.Count };
        }

        public async Task<BaseModel> GetMarksByStudentAsync(int studentId, int examId)
        {
            var marks = await _examMarksRepository.GetMarksByStudentAsync(studentId, examId);
            return new BaseModel { Success = true, Data = marks, Total = marks.Count };
        }

        public async Task<BaseModel> GetStudentExamMarksAsync(int examSubjectId, int studentId)
        {
            var marks = await _examMarksRepository.GetStudentExamMarksAsync(examSubjectId, studentId);
            return new BaseModel { Success = true, Data = marks };
        }

        public async Task<BaseModel> BulkCreateMarksAsync(BulkExamMarksCreateRequest request)
        {
            var marksList = new List<ExamMarks>();

            foreach (var studentMark in request.StudentMarks)
            {
                var marks = new ExamMarks
                {
                    ExamSubjectId = request.ExamSubjectId,
                    StudentId = studentMark.StudentId,
                    TheoryMarks = studentMark.TheoryMarks,
                    PracticalMarks = studentMark.PracticalMarks,
                    IsAbsent = studentMark.IsAbsent,
                    Remarks = studentMark.Remarks,
                    Status = MarksStatus.Draft,
                    CreatedAt = DateTime.Now,
                    CreatedAtUtc = DateTime.UtcNow
                };

                marksList.Add(marks);
            }

            var result = await _examMarksRepository.BulkCreateMarksAsync(marksList);

            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to create marks" };
            }

            return new BaseModel { Success = true, Message = "Marks created successfully" };
        }

        public async Task<BaseModel> BulkUpdateMarksAsync(List<ExamMarksUpdateRequest> requests)
        {
            var marksList = mapper.Map<List<ExamMarks>>(requests);
            var result = await _examMarksRepository.BulkUpdateMarksAsync(marksList);

            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to update marks" };
            }

            return new BaseModel { Success = true, Message = "Marks updated successfully" };
        }

        public async Task<BaseModel> VerifyMarksAsync(VerifyMarksRequest request)
        {
            var result = await _examMarksRepository.VerifyMarksAsync(request.MarksIds, request.VerifierRemarks);

            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to verify marks" };
            }

            return new BaseModel { Success = true, Message = "Marks verified successfully" };
        }

        public async Task<BaseModel> GetMarksByExamAsync(int examId)
        {
            var marks = await _examMarksRepository.GetMarksByExamAsync(examId);
            return new BaseModel { Success = true, Data = marks, Total = marks.Count };
        }

        public async Task<BaseModel> ImportMarksFromExcelAsync(ExamMarksImportRequest request)
        {
            // TODO: Implement Excel import logic
            // This would involve parsing the Base64 FileData, extracting student marks, and bulk creating/updating
            return new BaseModel { Success = false, Message = "Excel import not yet implemented" };
        }
    }
}


