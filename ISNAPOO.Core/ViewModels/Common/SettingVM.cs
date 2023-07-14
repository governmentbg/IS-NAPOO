using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class SettingVM : IMapFrom<Setting>, IMapTo<Setting>
    {


        [Key]
        public int idSetting { get; set; }


        [Required(ErrorMessage = "Полето \'Име\' е задължително")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Маклисмалната позволена дължина е 255 за полето 'Име' ")]       
        public string SettingName { get; set; }


        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'SettingDescription' не може да съдържа повече от 1000 символа.")]
        public string SettingDescription { get; set; }

        [Required(ErrorMessage = "Полето \'Код\' е задължително")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Маклисмалната позволена дължина е 50 за полето 'Код' ")]
        public string SettingIntCode { get; set; }


        [Required(ErrorMessage = "Полето \'Стойност\' е задължително")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Маклисмалната позволена дължина е 255 за полето 'Стойност' ")]
        public string SettingValue { get; set; }


        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Маклисмалната позволена дължина е 50 за полето 'Стойност' ")]
        public string SettingClass { get; set; }
    }
}
