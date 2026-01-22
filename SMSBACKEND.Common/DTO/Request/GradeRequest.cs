using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GradeRequest
    {
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
        public int? GradeScaleId { get; set; }
    }

    public class GradeCreateRequest
    {
        [Required]
        public int GradeScaleId { get; set; }

        [Required]
        [MaxLength(10)]
        public string GradeName { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public decimal MinPercentage { get; set; } = 0;

        [Required]
        [Range(0, 100)]
        public decimal MaxPercentage { get; set; } = 0;

        [Range(0, 10)]
        public decimal GradePoint { get; set; } = 0;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Color { get; set; } = "#000000";

        public int DisplayOrder { get; set; } = 0;
    }

    public class GradeUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int GradeScaleId { get; set; }

        [Required]
        [MaxLength(10)]
        public string GradeName { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public decimal MinPercentage { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal MaxPercentage { get; set; }

        [Range(0, 10)]
        public decimal GradePoint { get; set; }

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Color { get; set; } = "#000000";

        public int DisplayOrder { get; set; }
    }
}


