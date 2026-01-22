using Microsoft.AspNetCore.Mvc;
using Application.ServiceContracts;
using Common.DTO.Response;
using Common.DTO.Request;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Presentation.Filters;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentAttendanceController : ControllerBase
    {
        private readonly IStudentAttendanceService _studentAttendanceService;

        public StudentAttendanceController(IStudentAttendanceService studentAttendanceService)
        {
            _studentAttendanceService = studentAttendanceService;
        }


        /// <summary>
        /// Mark attendance for students in a class and section
        /// </summary>
        /// <param name="request">Request containing attendance records</param>
        /// <returns>Success status</returns>
        [HttpPost("MarkAttendance")]
        public async Task<ActionResult<BaseModel>> MarkAttendance(
            [FromBody] MarkAttendanceRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                if (request.ClassId <= 0 || request.SectionId <= 0)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "ClassId and SectionId are required"
                    });
                }

                if (request.Records == null || request.Records.Count == 0)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Attendance records are required"
                    });
                }

                var result = await _studentAttendanceService.MarkAttendance(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get attendance statistics for a class and section
        /// </summary>
        /// <param name="request">Request containing ClassId, SectionId, and date range</param>
        /// <returns>Attendance statistics</returns>
        [HttpPost("GetAttendanceStatistics")]
        public async Task<ActionResult<BaseModel>> GetAttendanceStatistics(
            [FromBody] GetAttendanceStatisticsRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                if (request.ClassId <= 0 || request.SectionId <= 0)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "ClassId and SectionId are required"
                    });
                }

                var result = await _studentAttendanceService.GetAttendanceStatistics(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get all attendance records (for frontend caching - offline-first pattern)
        /// Returns all active attendance records
        /// </summary>
        /// <returns>List of all attendance records</returns>
        [HttpGet("GetAllAttendance")]
        public async Task<ActionResult<BaseModel>> GetAllAttendance()
        {
            try
            {
                var result = await _studentAttendanceService.GetAllAttendance();

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get attendance records by class and section for a specific date
        /// </summary>
        /// <param name="request">Request containing ClassId, SectionId, and AttendanceDate</param>
        /// <returns>List of attendance records</returns>
        [HttpPost("GetAttendanceByClassSection")]
        public async Task<ActionResult<BaseModel>> GetAttendanceByClassSection(
            [FromBody] GetAttendanceByClassSectionRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                if (request.ClassId <= 0 || request.SectionId <= 0)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "ClassId and SectionId are required"
                    });
                }

                var result = await _studentAttendanceService.GetAttendanceByClassSection(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get attendance records for a specific student within a date range
        /// </summary>
        /// <param name="request">Request containing StudentId, StartDate, and EndDate</param>
        /// <returns>List of attendance records for the student</returns>
        [HttpPost("GetAttendanceByStudent")]
        public async Task<ActionResult<BaseModel>> GetAttendanceByStudent(
            [FromBody] GetAttendanceByStudentRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                if (request.StudentId <= 0)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "StudentId is required"
                    });
                }

                var result = await _studentAttendanceService.GetAttendanceByStudent(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get all attendance records for a specific date across all classes
        /// </summary>
        /// <param name="request">Request containing AttendanceDate</param>
        /// <returns>List of attendance records for the date</returns>
        [HttpPost("GetAttendanceByDate")]
        public async Task<ActionResult<BaseModel>> GetAttendanceByDate(
            [FromBody] GetAttendanceByDateRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                var result = await _studentAttendanceService.GetAttendanceByDate(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Update a single attendance record
        /// </summary>
        /// <param name="id">Attendance record ID</param>
        /// <param name="request">Request containing updated attendance data</param>
        /// <returns>Updated attendance record</returns>
        [HttpPost("UpdateAttendance/{id}")]
        public async Task<ActionResult<BaseModel>> UpdateAttendance(
            int id, [FromBody] UpdateAttendanceRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                if (id <= 0)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Invalid attendance ID"
                    });
                }

                var result = await _studentAttendanceService.UpdateAttendance(id, request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Delete (soft delete) an attendance record
        /// </summary>
        /// <param name="id">Attendance record ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("DeleteAttendance/{id}")]
        public async Task<ActionResult<BaseModel>> DeleteAttendance(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Invalid attendance ID"
                    });
                }

                var result = await _studentAttendanceService.DeleteAttendance(id);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        // ========================================
        // PHASE 2: REPORTING ENDPOINTS
        // ========================================

        [HttpPost("GetMonthlyReport")]
        [Authorize]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<ActionResult<BaseModel>> GetMonthlyReport([FromBody] GetMonthlyReportRequest request)
        {
            try
            {
                var result = await _studentAttendanceService.GetMonthlyReportAsync(request);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred while generating monthly report: {ex.Message}"
                });
            }
        }

        [HttpPost("GetAttendancePercentage")]
        [Authorize]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<ActionResult<BaseModel>> GetAttendancePercentage([FromBody] GetAttendancePercentageRequest request)
        {
            try
            {
                var result = await _studentAttendanceService.GetAttendancePercentageAsync(request);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred while calculating attendance percentage: {ex.Message}"
                });
            }
        }

        [HttpPost("GetDefaulters")]
        [Authorize]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<ActionResult<BaseModel>> GetDefaulters([FromBody] GetDefaultersRequest request)
        {
            try
            {
                var result = await _studentAttendanceService.GetDefaultersAsync(request);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred while generating defaulters report: {ex.Message}"
                });
            }
        }

        [HttpPost("GetAttendanceSummary")]
        [Authorize]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<ActionResult<BaseModel>> GetAttendanceSummary([FromBody] GetAttendanceSummaryRequest request)
        {
            try
            {
                var result = await _studentAttendanceService.GetAttendanceSummaryAsync(request);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseModel
                {
                    Success = false,
                    Message = $"An error occurred while generating attendance summary: {ex.Message}"
                });
            }
        }

        [HttpPost("ExportAttendance")]
        [Authorize]
        [ServiceFilter(typeof(ValidateModelState))]
        public async Task<IActionResult> ExportAttendance([FromBody] ExportAttendanceRequest request)
        {
            try
            {
                var result = await _studentAttendanceService.ExportAttendanceAsync(request);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                var fileBytes = result.Data as byte[];
                
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return BadRequest(new BaseModel
                    {
                        Success = false,
                        Message = "Export file generation failed"
                    });
                }

                var fileName = request.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase)
                    ? $"Attendance_{DateTime.Now:yyyyMMdd}.pdf"
                    : $"Attendance_{DateTime.Now:yyyyMMdd}.xlsx";

                var contentType = request.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase)
                    ? "application/pdf"
                    : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while exporting attendance: {ex.Message}");
            }
        }
    }
}

