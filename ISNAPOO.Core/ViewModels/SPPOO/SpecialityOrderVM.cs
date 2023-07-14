using Data.Models.Data.SPPOO;
using ISNAPOO.Core.Mapping;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class SpecialityOrderVM : IMapTo<SpecialityOrder>, IMapFrom<SpecialityOrder>
    {
        public int IdSpecialityOrder { get; set; }

        [Required]
        [Display(Name = "Специалност")]
        public int IdSpeciality { get; set; }
        public SpecialityVM Speciality { get; set; }

        [Required]
        [Display(Name = "Заповед")]
        public int IdSPPOOOrder { get; set; }
        public OrderVM SPPOOOrder { get; set; }

        [Required]
        [Display(Name = "Вид на промяната, KeyType - OrderTypeChange ")]
        public int IdTypeChange { get; set; }//Вписване (създаване),Промяна, Отпадане(заличаване)
    }
}
