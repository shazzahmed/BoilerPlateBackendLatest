using Common.DTO.Request;
using System.ComponentModel;
using System.Text;

namespace SMSBACKEND.Infrastructure.Services.Services
{
    public interface IExcelParserService
    {
        Task<List<StudentImportRow>> ParseExcelFileAsync(Stream fileStream);
        Task<byte[]> GenerateImportTemplateAsync();
        Task<List<string>> ValidateExcelStructureAsync(Stream fileStream);
    }

    public class ExcelParserService : IExcelParserService
    {
        public ExcelParserService()
        {
        }

        public async Task<List<StudentImportRow>> ParseExcelFileAsync(Stream fileStream)
        {
            var students = new List<StudentImportRow>();
            
            // For now, we'll implement a simple CSV parser
            // In production, you might want to use a proper Excel library
            using var reader = new StreamReader(fileStream);
            var content = await reader.ReadToEndAsync();
            
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            if (lines.Length < 2)
                return students;

            // Skip header row
            for (int i = 1; i < lines.Length; i++)
            {
                var values = ParseCSVLine(lines[i]);
                
                if (values.Length < 32) // Ensure we have enough columns
                    continue;

                var student = new StudentImportRow
                {
                    RowNumber = i,
                    FullName = GetValue(values, 0),
                    DateOfBirth = ParseDate(GetValue(values, 1)),
                    Gender = GetValue(values, 2),
                    Class = GetValue(values, 3),
                    Section = GetValue(values, 4),
                    PhoneNo = GetValue(values, 5),
                    Email = GetValue(values, 6),
                    Address = GetValue(values, 7),
                    FatherName = GetValue(values, 8),
                    FatherCNIC = GetValue(values, 9),
                    FatherPhone = GetValue(values, 10),
                    MotherName = GetValue(values, 11),
                    MotherCNIC = GetValue(values, 12),
                    MotherPhone = GetValue(values, 13),
                    GuardianType = GetValue(values, 14),
                    GuardianName = GetValue(values, 15),
                    GuardianPhone = GetValue(values, 16),
                    GuardianEmail = GetValue(values, 17),
                    RollNo = GetValue(values, 18),
                    AdmissionNo = GetValue(values, 19),
                    AdmissionDate = ParseDate(GetValue(values, 20)),
                    MonthlyFees = ParseDecimal(GetValue(values, 21)),
                    AdmissionFees = ParseDecimal(GetValue(values, 22)),
                    SecurityDeposit = ParseDecimal(GetValue(values, 23)),
                    Height = GetValue(values, 24),
                    Weight = GetValue(values, 25),
                    MeasurementDate = ParseDate(GetValue(values, 26)),
                    BloodGroup = GetValue(values, 27),
                    Religion = GetValue(values, 28),
                    Cast = GetValue(values, 29),
                    MedicalHistory = GetValue(values, 30),
                    Notes = GetValue(values, 31)
                };

                students.Add(student);
            }

            return students;
        }

        public async Task<byte[]> GenerateImportTemplateAsync()
        {
            var csv = new StringBuilder();
            
            // Add headers
            var headers = new[]
            {
                "FullName", "DateOfBirth", "Gender", "Class", "Section", "PhoneNo", "Email", "Address",
                "FatherName", "FatherCNIC", "FatherPhone", "MotherName", "MotherCNIC", "MotherPhone",
                "GuardianType", "GuardianName", "GuardianPhone", "GuardianEmail", "RollNo", "AdmissionNo",
                "AdmissionDate", "MonthlyFees", "AdmissionFees", "SecurityDeposit", "Height", "Weight",
                "MeasurementDate", "BloodGroup", "Religion", "Cast", "MedicalHistory", "Notes"
            };

            csv.AppendLine(string.Join(",", headers));

            // Add sample data
            var sampleData = new[]
            {
                "John Doe", "2010-05-15", "Male", "1", "A", "03451234567", "john@email.com", "123 Main St",
                "Ahmed Ali", "12345-1234567-1", "03451234568", "Fatima Ali", "12345-1234567-2", "03451234569",
                "Father", "Ahmed Ali", "03451234568", "ahmed@email.com", "001", "ADM001", "2024-01-01",
                "5000", "10000", "5000", "5.2", "45", "2024-01-01", "O+", "Islam", "Syed", "None", "Good student"
            };

            csv.AppendLine(string.Join(",", sampleData));

            return await Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
        }

        public async Task<List<string>> ValidateExcelStructureAsync(Stream fileStream)
        {
            var errors = new List<string>();
            
            try
            {
                using var reader = new StreamReader(fileStream);
                var content = await reader.ReadToEndAsync();
                
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                if (lines.Length < 2)
                {
                    errors.Add("File must have at least a header row and one data row");
                    return errors;
                }

                var headerLine = lines[0];
                var headers = ParseCSVLine(headerLine);

                if (headers.Length < 32)
                {
                    errors.Add($"File must have at least 32 columns, found {headers.Length}");
                }

                // Check required headers
                var requiredHeaders = new[] { "FullName", "Class", "FatherName", "GuardianName" };
                for (int i = 0; i < requiredHeaders.Length && i < headers.Length; i++)
                {
                    if (string.IsNullOrEmpty(headers[i]) || !headers[i].Equals(requiredHeaders[i], StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add($"Required header '{requiredHeaders[i]}' not found in column {i + 1}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error reading file: {ex.Message}");
            }

            return errors;
        }

        private string[] ParseCSVLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;
            
            for (int i = 0; i < line.Length; i++)
            {
                var currentChar = line[i];
                if (currentChar == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current += '"';
                        i++; // Skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (currentChar == ',' && !inQuotes)
                {
                    result.Add(current.Trim());
                    current = "";
                }
                else
                {
                    current += currentChar;
                }
            }
            
            result.Add(current.Trim());
            return result.ToArray();
        }

        private string GetValue(string[] values, int index)
        {
            return index < values.Length ? values[index] : string.Empty;
        }

        private DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;

            if (DateTime.TryParse(dateString, out DateTime date))
                return date;

            return null;
        }

        private decimal? ParseDecimal(string valueString)
        {
            if (string.IsNullOrEmpty(valueString))
                return null;

            if (decimal.TryParse(valueString, out decimal value))
                return value;

            return null;
        }
    }
}
