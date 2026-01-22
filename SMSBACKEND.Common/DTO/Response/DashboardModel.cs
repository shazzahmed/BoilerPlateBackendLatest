using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Common.DTO.Response
{
    public class DashboardModel
    {
        public List<StudentModel> Students { get; set; } = new List<StudentModel>();
        public List<ModuleModel> Module { get; set; } = new List<ModuleModel>();
        public List<SharedModel> Progress { get; set; } = new List<SharedModel>();
        public List<SummaryModel> Overview { get; set; } = new List<SummaryModel>();
        public List<SharedModel> AmountSummary { get; set; } = new List<SharedModel>();
        public List<SharedModel> Roles { get; set; } = new List<SharedModel>();
        public TrailModel Trail { get; set; } = new TrailModel();
        public List<string> Permissions { get; set; } = new List<string>();
    }
    public class SummaryModel
    {
        public string Header { get; set; }
        public List<SharedModel> OverviewSummary { get; set; } = new List<SharedModel>();

    }
    public class TrailModel
    {
        public Boolean PremiumFeatures { get; set; }
        public DateTime TrialExpiresOn { get; set; }
        public Boolean IsTrialValid { get; set; }

    }
    public class ValuePercentageModel
    {
        public string Percentage { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class SharedModel
    {
        public string Description { get; set; }
        public string Percentage { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
    }
}
