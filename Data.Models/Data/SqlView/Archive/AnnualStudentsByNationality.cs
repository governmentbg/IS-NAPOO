using System.ComponentModel.DataAnnotations;


namespace Data.Models.Data.SqlView.Archive
{
    public class AnnualStudentsByNationality
    {
        [Key]
        public long IdNationality { get; set; }
        public string Nationality { get; set; }
        public int CountIncludedMen { get; set; }
        public int CountCertifiedMen { get; set; }
        public int CountProfessionallyCertifiedMen { get; set; }
        public int CountIncludedPartOfProfessionMen { get; set; }
        public int GridId => (int)IdNationality;
    }
}
