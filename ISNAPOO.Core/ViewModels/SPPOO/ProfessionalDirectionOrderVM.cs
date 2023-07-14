using Data.Models.Data.SPPOO;
using ISNAPOO.Core.Mapping;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class ProfessionalDirectionOrderVM : IMapTo<ProfessionalDirectionOrder>, IMapFrom<ProfessionalDirectionOrder>
    {
        public int IdSpecialityOrder { get; set; }

        [Required]
        [Display(Name = "Професионално направление")]
        public int IdProfessionalDirection { get; set; }
        public ProfessionalDirectionVM ProfessionalDirection { get; set; }

        [Required]
        [Display(Name = "Заповед")]
        public int IdSPPOOOrder { get; set; }
        public OrderVM SPPOOOrder { get; set; }

        [Required]
        [Display(Name = "Вид на промяната, KeyType - OrderTypeChange ")]
        public int IdTypeChange { get; set; }//Вписване (създаване),Промяна, Отпадане(заличаване)
    }
}
