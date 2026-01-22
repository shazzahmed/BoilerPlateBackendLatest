using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities.StaticClasses
{
    public static class EntityDependencyMap
    {
        public static readonly Dictionary<string, List<string>> Dependencies = new()
    {
        { "Section", new List<string> { "Class", "ClassSection", 
            "ClassTeacher", "TeacherSubjects", "StudentClassAssignment", "StudentAttendance", "Timetable" } },
        { "Subject", new List<string> { "Class","SubjectClass", 
            "TeacherSubjects", "Timetable", "Staff" } },
        { "Class", new List<string> { "SubjectClass", "ClassSection", "ClassTeacher", "TeacherSubjects", "StudentClassAssignment", "StudentAttendance", "Timetable", "Student" } },
        { "ExpenseHead", new List<string> { "Expense" } },
        { "IncomeHead", new List<string> { "Income" } },
        { "Department", new List<string> { "Staff" } },
        { "Staff", new List<string> { "ClassTeacher", "TeacherSubjects", "Timetable" } },
        { "SchoolHouse", new List<string> { "Student" } },
        { "StudentCategory", new List<string> { "Student" } },
        { "Student", new List<string> { "FeeAssignment", "FeeTransaction", "StudentClassAssignment", "StudentAttendance" } },
        { "FeeDiscount", new List<string> { "FeeAssignment", "FeeGroupFeeType", "FeeGroupFeeType" } },
        { "FeeType", new List<string> { "FeeAssignment", "FeeGroupFeeType", "FeeGroupFeeType", "FeeTransaction" } },
        { "FeeGroup", new List<string> { "FeeAssignment", "FeeGroupFeeType", "FeeGroupFeeType", "FeeTransaction" } },
        { "TeacherSubjects", new List<string> { "Timetable" } },
        { "FeeAssignment", new List<string> { "FeeTransaction", "Student" } },
        { "FeeTransaction", new List<string> { "FeeAssignment", "Student" } },
        { "FeeGroupFeeType", new List<string> { "FeeAssignment" } },
        { "PreAdmission", new List<string> { "Enquiry" } },
        { "EntryTest", new List<string> { "Enquiry" } },
        { "Admission", new List<string> { "Enquiry", "Student" } },
        // Add more mappings as needed
    };
    }

}
