using Data.Models.Data.ExternalExpertCommission;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.ExternalExpertCommission
{
    public class ExpertExpertCommissionVM : IMapFrom<ExpertExpertCommission>, IMapTo<ExpertExpertCommission>
    {
        public ExpertExpertCommissionVM()
        {
        }
        
        public int IdExpertExpertCommission { get; set; }


        [Required]
        [Display(Name = "Експерт")]
        public int IdExpert { get; set; }
        public virtual ExpertVM Expert { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Eкспертна комисия' е задължително!")]
        public int IdExpertCommission { get; set; }//KeyType -Eкспертни комисии( KeyTypeIntCode = ExpertCommission )

        public string ExpertCommissionName { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Роля' е задължително!")]
        public int IdRole { get; set; }//KeyType -Роля на експерта KeyTypeIntCode = ExpertRoleCommission: Прецедател, Член )

        public string RoleName { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Институция, която представя' може да съдържа до 512 символа!")]
        [Required(ErrorMessage = "Полето 'Институция, която представя' е задължително!")]
        public string Institution { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Длъжност' може да съдържа до 100 символа!")]
        [Required(ErrorMessage = "Полето 'Длъжност' е задължително!")]
        public string Occupation { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Протокол' може да съдържа до 100 символа!")]
        [Required(ErrorMessage = "Полето 'Протокол' е задължително!")]
        public string Protokol { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на Протокол' е задължително!")]
        public DateTime? ProtokolDate { get; set; }

        public DateOnly ProtokolDateOnly { get { return (ProtokolDate.HasValue ? DateOnly.FromDateTime(ProtokolDate.Value.Date) : DateOnly.MinValue); } }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Статус' е задължително!")]
        public int IdStatus { get; set; }//Активен/Неактивен

        public string StatusName { get; set; }
        public int ProcedureCount { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "История на промяната")]
        public string? Comment { get; set; }

        public int IdMemberType { get; set; }//Титуляр/Заместник
        public string MemberTypeString { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }
        
        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }
        #endregion
    }
}
