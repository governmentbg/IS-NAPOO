using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Common
{
    /// <summary>
    /// Настройки
    /// </summary>
    [Table("Setting")]
    [Display(Name = "Настройки")]
    public class Setting : IEntity
    {
        [Key]
        public int idSetting { get; set; }
        public int IdEntity => idSetting;

        [Required]
        [StringLength(DBStringLength.StringLength255)]
        public string SettingName { get; set; }


        [StringLength(DBStringLength.StringLength1000)]
        public string SettingDescription { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength50)]
        public string SettingIntCode { get; set; }


        [StringLength(DBStringLength.StringLength255)]
        public string SettingValue { get; set; }


        [StringLength(DBStringLength.StringLength50)]
        public string SettingClass { get; set; }


        
    }
}
