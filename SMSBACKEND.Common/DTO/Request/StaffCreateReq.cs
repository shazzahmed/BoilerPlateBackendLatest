
using Common.DTO.Request;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class StaffCreateReq : BaseClass
    {
        public int Id { get; set; }
        public int LangId { get; set; }
        public string? UserId { get; set; }
        public string? Image { get; set; }
        public string? StaffId { get; set; }
        public string? CNIC { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName}.{LastName}";
        public DateTime Dob { get; set; }
        public UserRoles Role { get; set; }
        public int DepartmentId { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? ContactNo { get; set; }
        public string? PhoneNo { get; set; }
        public string? Religion { get; set; }
        public string? Zipcode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? EmergencyContactNo { get; set; }
        public string? LocalAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? Qualification { get; set; }
        public string? WorkExp { get; set; }
        public string? PreviousOrganization { get; set; }
        public DateTime DateOfLeaving { get; set; }
        public string? Note { get; set; }

        // Additional Address Information
        public string? StreetAddress { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }

        // Employment Information
        public string? Specialization { get; set; }
        public string? Designation { get; set; }
        public string? EmployeeCode { get; set; }

        // System Access
        public string? BiometricId { get; set; }
        public string? RfidCode { get; set; }

        // Multiple Roles Support
        public List<string> Roles { get; set; } = new List<string>();
        public string? BasicSalary { get; set; }
        public string? Allowances { get; set; }
        public string? Deductions { get; set; }
        public string? EpfNo { get; set; }
        public string? ContractType { get; set; }
        public string? Shift { get; set; }
        public string? Location { get; set; }
        public string? AccountTitle { get; set; }
        public string? BankAccountNo { get; set; }
        public string? BankName { get; set; }
        public string? IfscCode { get; set; }
        public string? BankBranch { get; set; }
        public string? Payscale { get; set; }
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? Linkedin { get; set; }
        public string? Instagram { get; set; }
        public string? Resume { get; set; }
        public string? JoiningLetter { get; set; }
        public string? ResignationLetter { get; set; }
        public string? OtherDocumentName { get; set; }
        public string? OtherDocumentFile { get; set; }
    }
}
