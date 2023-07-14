using System.ComponentModel.DataAnnotations;

namespace Data.Models.Data.SqlView.Archive
{
    public class AnnualStudents
    {
        [Key]
        public long IdAnnualStudent { get; set; }
        public int CountIncludedMan_I_VQS                { get; set; }
        public int CountIncludedWomen_I_VQS              { get; set; }
                        
        public int CountIncludedMan_II_VQS               { get; set; }
        public int CountIncludedWomen_II_VQS             { get; set; }
                                
        public int CountIncludedMan_III_VQS              { get; set; }
        public int CountIncludedWomen_III_VQS            { get; set; }
                                
        public int CountIncludedMan_IV_VQS               { get; set; }
        public int CountIncludedWomen_IV_VQS             { get; set; }
                               
        public int CountCertificateMan_I_VQS             { get; set; }
        public int CountCertificateWomen_I_VQS           { get; set; }
                                    
        public int CountCertificateMan_II_VQS            { get; set; }
        public int CountCertificateWomen_II_VQS          { get; set; }
                             
        public int CountCertificateMan_III_VQS           { get; set; }
        public int CountCertificateWomen_III_VQS         { get; set; }
                         
        public int CountCertificateMan_IV_VQS            { get; set; }
        public int CountCertificateWomen_IV_VQS          { get; set; }

        public string BirthDate { get; set; }
    }
}
