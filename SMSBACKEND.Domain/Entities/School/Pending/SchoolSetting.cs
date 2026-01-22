public class SchoolSetting
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Biometric { get; set; } = 0;
    public string BiometricDevice { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public int? LangId { get; set; }
    public string Languages { get; set; }
    public string DiseCode { get; set; }
    public string DateFormat { get; set; }
    public string TimeFormat { get; set; }
    public string Currency { get; set; }
    public string CurrencySymbol { get; set; }
    public string IsRtl { get; set; } = "disabled";
    public int IsDuplicateFeesInvoice { get; set; } = 0;
    public string Timezone { get; set; } = "UTC";
    public int? SessionId { get; set; }
    public string CronSecretKey { get; set; }
    public string CurrencyPlace { get; set; } = "before_number";
    public string ClassTeacher { get; set; }
    public string StartMonth { get; set; }
    public int AttendenceType { get; set; } = 0;
    public string Image { get; set; }
    public string AdminLogo { get; set; }
    public string AdminSmallLogo { get; set; }
    public string Theme { get; set; } = "default.jpg";
    public int FeeDueDays { get; set; } = 0;
    public int AdmAutoInsert { get; set; } = 1;
    public string AdmPrefix { get; set; } = "ssadm19/20";
    public string AdmStartFrom { get; set; }
    public int AdmNoDigit { get; set; } = 6;
    public int AdmUpdateStatus { get; set; } = 0;
    public int StaffIdAutoInsert { get; set; } = 1;
    public string StaffIdPrefix { get; set; } = "staffss/19/20";
    public string StaffIdStartFrom { get; set; }
    public int StaffIdNoDigit { get; set; } = 6;
    public int StaffIdUpdateStatus { get; set; } = 0;
    public string IsActive { get; set; } = "no";
    public int OnlineAdmission { get; set; } = 0;
    public int IsBloodGroup { get; set; } = 1;
    public int IsStudentHouse { get; set; } = 1;
    public int RollNo { get; set; } = 1;
    public int Category { get; set; }
    public int Religion { get; set; } = 1;
    public int Cast { get; set; } = 1;
    public int City { get; set; } = 1;
    public int MobileNo { get; set; } = 1;
    public int PhoneNo { get; set; } = 1;
    public int PickupPerson { get; set; } = 1;
    public int StudentEmail { get; set; } = 1;
    public int AdmissionDate { get; set; } = 1;
    public int? AdmissionFees { get; set; }
    public int SecuirtyDeposit { get; set; }
    public int MonthlyFees { get; set; } = 1;
    public int Lastname { get; set; }
    public int StudentPhoto { get; set; } = 1;
    public int StudentHeight { get; set; } = 1;
    public int StudentWeight { get; set; } = 1;
    public int MeasurementDate { get; set; } = 1;
    public int FatherName { get; set; } = 1;
    public int FatherPhone { get; set; } = 1;
    public int FatherOccupation { get; set; } = 1;
    public int FatherPic { get; set; } = 1;
    public int MotherName { get; set; } = 1;
    public int MotherPhone { get; set; } = 1;
    public int MotherOccupation { get; set; } = 1;
    public int MotherPic { get; set; } = 1;
    public int GuardianRelation { get; set; } = 1;
    public int GuardianEmail { get; set; } = 1;
    public int GuardianPic { get; set; } = 1;
    public int GuardianAddress { get; set; } = 1;
    public int GuardianAreaAddress { get; set; } = 1;
    public int CurrentAddress { get; set; } = 1;
    public int PermanentAddress { get; set; } = 1;
    public int RouteList { get; set; } = 1;
    public int HostelId { get; set; } = 1;
    public int BankAccountNo { get; set; } = 1;
    public int NationalIdentificationNo { get; set; } = 1;
    public int LocalIdentificationNo { get; set; } = 1;
    public int Rte { get; set; } = 1;
    public int PreviousSchoolDetails { get; set; } = 1;
    public int StudentNote { get; set; } = 1;
    public int UploadDocuments { get; set; } = 1;
    public int StaffDesignation { get; set; } = 1;
    public int StaffDepartment { get; set; } = 1;
    public int StaffLastName { get; set; } = 1;
    public int StaffFatherName { get; set; } = 1;
    public int StaffMotherName { get; set; } = 1;
    public int StaffDateOfJoining { get; set; } = 1;
    public int StaffPhone { get; set; } = 1;
    public int StaffEmergencyContact { get; set; } = 1;
    public int StaffMaritalStatus { get; set; } = 1;
    public int StaffPhoto { get; set; } = 1;
    public int StaffCurrentAddress { get; set; } = 1;
    public int StaffPermanentAddress { get; set; } = 1;
    public int StaffQualification { get; set; } = 1;
    public int StaffWorkExperience { get; set; } = 1;
    public int StaffNote { get; set; } = 1;
    public int StaffEpfNo { get; set; } = 1;
    public int StaffBasicSalary { get; set; } = 1;
    public int StaffContractType { get; set; } = 1;
    public int StaffWorkShift { get; set; } = 1;
    public int StaffWorkLocation { get; set; } = 1;
    public int StaffLeaves { get; set; } = 1;
    public int StaffAccountDetails { get; set; } = 1;
    public int StaffSocialMedia { get; set; } = 1;
    public int StaffUploadDocuments { get; set; } = 1;
    public string MobileApiUrl { get; set; }
    public string AppPrimaryColorCode { get; set; }
    public string AppSecondaryColorCode { get; set; }
    public string AppLogo { get; set; }
    public int StudentProfileEdit { get; set; } = 0;
    public string ZoomApiKey { get; set; }
    public string ZoomApiSecret { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
