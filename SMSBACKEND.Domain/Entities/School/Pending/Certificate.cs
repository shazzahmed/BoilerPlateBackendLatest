public class Certificate
{
    public int Id { get; set; }
    public string CertificateName { get; set; }
    public string CertificateText { get; set; }
    public string LeftHeader { get; set; }
    public string CenterHeader { get; set; }
    public string RightHeader { get; set; }
    public string LeftFooter { get; set; }
    public string RightFooter { get; set; }
    public string CenterFooter { get; set; }
    public string BackgroundImage { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedFor { get; set; }
    public bool Status { get; set; }
    public int HeaderHeight { get; set; }
    public int ContentHeight { get; set; }
    public int FooterHeight { get; set; }
    public int ContentWidth { get; set; }
    public bool EnableStudentImage { get; set; }
    public int EnableImageHeight { get; set; }
}
