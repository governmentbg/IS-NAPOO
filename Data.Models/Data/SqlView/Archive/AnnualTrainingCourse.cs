using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.SqlView.Archive
{
    public class AnnualTrainingCourse
    {
        [Key]
        public long IdAnnualTrainingCourse { get; set; }
        public string ProfessionCode { get; set; }
        public string SpecialityCode { get; set; }
        public string VQSName { get; set; }
        public string DistrictName { get; set; }
        public int CountIncludedMan { get; set; }
        public int CountIncludedWomen { get; set; }
        public int CountCertificateMan { get; set; }
        public int CountCertificateWomen { get; set; }
        public int Horarium { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cost { get; set; }
        public int CountTestimony { get; set; }
        public string SourceFunding { get; set; }
        public int CountDisabledPerson { get; set; }
        public int CountDisadvantagedPerson { get; set; }
    }
}
