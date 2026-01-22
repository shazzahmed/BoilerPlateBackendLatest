public class TemplateMarksheet
{
    public int Id { get; set; }
    public string Template { get; set; }
    public string Heading { get; set; }
    public string Title { get; set; }
    public string LeftLogo { get; set; }
    public string RightLogo { get; set; }
    public string ExamName { get; set; }
    public string SchoolName { get; set; }
    public string ExamCenter { get; set; }
    public string LeftSign { get; set; }
    public string MiddleSign { get; set; }
    public string RightSign { get; set; }
    public int ExamSession { get; set; }
    public int IsName { get; set; }
    public int IsFatherName { get; set; }
    public int IsMotherName { get; set; }
    public int IsDob { get; set; }
    public int IsAdmissionNo { get; set; }
    public int IsRollNo { get; set; }
    public int IsPhoto { get; set; }
    public int IsDivision { get; set; }
    public int IsCustomField { get; set; }
    public string BackgroundImg { get; set; }
    public string Date { get; set; }
    public int IsClass { get; set; }
    public int IsSection { get; set; }
    public string Content { get; set; }
    public string ContentFooter { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
