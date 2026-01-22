using System.Collections.Generic;

namespace Common.Utilities.StaticClasses
{
    public static class AppPermissions
    {
        public const string Add = "ADD";
        public const string Edit = "EDIT";
        public const string Delete = "DELETE";
        public const string View = "VIEW";
        public const string Import = "IMPORT";
        public const string Export = "EXPORT";
        public const string Approve = "APPROVE";
        public const string Assign = "ASSIGN";

        public static List<string> All => new() { Add, Edit, Delete, View, Import, Export, Approve, Assign };
    }

}
