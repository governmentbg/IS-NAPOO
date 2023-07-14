using System;

namespace ISNAPOO.Core.ViewModels.Archive
{
    public class AnnualCurriculumsVM
    {
        public int Id { get; set; }

        public string CPOName { get; set; }

        public string Profession { get; set; }

        public string Speciality { get; set; }

        public string UpdateReason { get; set; }

        public DateTime? Date { get; set; }
    }
}
