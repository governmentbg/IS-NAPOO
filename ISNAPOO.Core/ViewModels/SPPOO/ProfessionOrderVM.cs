using Data.Models.Data.SPPOO;
using ISNAPOO.Core.Mapping;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class ProfessionOrderVM : IMapFrom<ProfessionOrder>, IMapTo<ProfessionOrder>
    {
        public int IdProfessionOrder { get; set; }

        [Required]
        [Display(Name = "Професия")]
        public int IdProfession { get; set; }
        public ProfessionVM Profession { get; set; }

        [Required]
        [Display(Name = "Заповед")]
        public int IdSPPOOOrder { get; set; }
        public OrderVM SPPOOOrder { get; set; }

        [Required]
        [Display(Name = "Вид на промяната, KeyType - OrderTypeChange ")]
        public int IdTypeChange { get; set; }//Вписване (създаване),Промяна, Отпадане(заличаване)
    }
}
