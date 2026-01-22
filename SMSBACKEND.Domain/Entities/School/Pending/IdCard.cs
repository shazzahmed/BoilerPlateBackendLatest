using System.ComponentModel.DataAnnotations;

public class IdCard
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; }

    [Required, MaxLength(100)]
    public string SchoolName { get; set; }

    [Required, MaxLength(500)]
    public string SchoolAddress { get; set; }

    [Required, MaxLength(100)]
    public string Background { get; set; }

    [Required, MaxLength(100)]
    public string Logo { get; set; }

    [Required, MaxLength(100)]
    public string SignImage { get; set; }

    [Required, MaxLength(100)]
    public string HeaderColor { get; set; }

    public bool EnableAdmissionNo { get; set; }
    public bool EnableStudentName { get; set; }
    public bool EnableClass { get; set; }
    public bool EnableFathersName { get; set; }
    public bool EnableMothersName { get; set; }
    public bool EnableAddress { get; set; }
    public bool EnablePhone { get; set; }
    public bool EnableDob { get; set; }
    public bool EnableBloodGroup { get; set; }
    public bool Status { get; set; }
}
