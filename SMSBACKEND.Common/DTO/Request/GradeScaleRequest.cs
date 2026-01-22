using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GradeScaleRequest
    {
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
        public bool? IsActive { get; set; }
    }

    public class GradeScaleCreateRequest
    {
        [Required]
        [MaxLength(100)]
        public string GradeScaleName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsDefault { get; set; } = false;

        public List<GradeCreateRequest> Grades { get; set; } = new List<GradeCreateRequest>();
    }

    public class GradeScaleUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string GradeScaleName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }
    }
}


