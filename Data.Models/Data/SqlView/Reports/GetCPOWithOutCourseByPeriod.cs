using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.SqlView.Reports
{
    public class GetCPOWithOutCourseByPeriod
    {
    
        [Key]
        public Guid Id{ get; set; }
        public int IdCandidate_Provider { get; set; }
        public int IdSpeciality { get; set; }
        public string SpecialityName { get; set; }
        public string ProfessionName { get; set; }
        public string ProviderName { get; set; }
    }
}
