public class PaymentSetting
{
    public int Id { get; set; }
    public string PaymentType { get; set; }
    public string ApiUsername { get; set; }
    public string ApiSecretKey { get; set; }
    public string Salt { get; set; }
    public string ApiPublishableKey { get; set; }
    public string ApiPassword { get; set; }
    public string ApiSignature { get; set; }
    public string ApiEmail { get; set; }
    public string PaypalDemo { get; set; }
    public string AccountNo { get; set; }
    public string IsActive { get; set; }
    public int GatewayMode { get; set; }
    public string PaytmWebsite { get; set; }
    public string PaytmIndustryType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
