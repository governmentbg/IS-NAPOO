using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.SqlView.Archive
{
    public class AnnualTrainingValidationClientCourse
    {
        [Key]
        public long IdAnnualTrainingValidationClient { get; set; }
        public string ProfessionCode { get; set; }
        public string SpecialityCode { get; set; }
        public string VQSName { get; set; }
        public string DistrictName { get; set; }
        public int CountValidationPK { get; set; }
        public int CountValidPO { get; set; }
        public int CountDisabledPerson { get; set; }
        public int CountDisadvantagedPerson { get; set; }
    }
}
