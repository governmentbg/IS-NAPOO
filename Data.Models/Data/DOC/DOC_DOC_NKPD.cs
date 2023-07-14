using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.DOC
{ 

    /// <summary>
    /// ДОС връзка със NKPD
    /// </summary>
    [Table("DOC_DOC_NKPD")]
    [Display(Name = "ДОС връзка със NKPD")]
    public class DOC_DOC_NKPD
    {

        [Key]
        public int IdDOC_DOC_NKPD { get; set; }

        [Required]
        [Display(Name = "DOC")]
        [ForeignKey(nameof(DOC))]
        public int IdDOC { get; set; }
        public DOC DOC { get; set; }


        [Required]
        [Display(Name = "НКПД")]
        [ForeignKey(nameof(NKPD))]
        public int IdNKPD { get; set; }
        public NKPD NKPD { get; set; }
    }
}
