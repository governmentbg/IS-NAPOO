using Data.Models.Data.SPPOO;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class FrameworkProgramFormEducationVM : IMapFrom<FrameworkProgramFormEducation>, IMapTo<FrameworkProgramFormEducation>
    {
        public int IdFrameworkProgramFormEducation { get; set; }

        [Required]
        [Display(Name = "Рамкова програма")]
        public int IdFrameworkProgram { get; set; }
        public FrameworkProgram FrameworkProgram { get; set; }

        [Required]
        [Display(Name = "Форма на обучение")]
        public int IdFormEducation { get; set; }//Дневна, Вечерна, Дистанционна, Индивидуална, самостоятелна, Задочна, Дуална
        public string FormEducationName { get; set; }
    }
}
