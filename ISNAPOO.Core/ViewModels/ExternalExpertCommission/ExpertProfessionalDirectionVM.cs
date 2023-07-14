using Data.Models.Data.ExternalExpertCommission;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.SPPOO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.ExternalExpertCommission
{
    public class ExpertProfessionalDirectionVM : IMapFrom<ExpertProfessionalDirection>, IMapTo<ExpertProfessionalDirection>
    {
        public ExpertProfessionalDirectionVM()
        {
            this.ExpertTypeName = string.Empty;
            this.StatusName = string.Empty;
        }

        
        public int IdExpertProfessionalDirection { get; set; }
                
        [Display(Name = "Експерт")]
        public int IdExpert { get; set; }
        public ExpertVM Expert { get; set; }

        [Display(Name = "Вид експерт")]//Външен експерт(ExternalExpert), Член на експертна комисия(MemberExpertCommission)
        [Range(1, int.MaxValue, ErrorMessage = "Изборът на 'Вид експерт' е задължителен")]
        public int IdExpertType { get; set; }
        public string ExpertTypeName { get; set; }

        [Display(Name = "Професионално направление")]
        [Range(1, int.MaxValue, ErrorMessage = "Полето 'Професионално направление' е задължително!")]
        public int IdProfessionalDirection { get; set; }
        public ProfessionalDirectionVM ProfessionalDirection { get; set; }
        public string ProfessionalDirectionName => (this.ProfessionalDirection != null ? this.ProfessionalDirection.Name : string.Empty);
        public string ProfessionalDirectionCode => (this.ProfessionalDirection != null ? this.ProfessionalDirection.Code : string.Empty);

        [Display(Name = "Статус")] //Активен/Неактивен
        [Range(1, int.MaxValue, ErrorMessage = "Изборът на 'Статус' е задължителен")]
        public int IdStatus { get; set; }
        public string StatusName { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Максималната позволена дължина е 512 за полето 'Коментар при промяна на статуса'")]
        [Display(Name = "Коментар при промяна на статуса")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на утвърждаване' е задължително!")]
        [Display(Name = "Дата на утвърждаване, като външен експерт")]
        public DateTime? DateApprovalExternalExpert { get; set; }

        [Required(ErrorMessage = "Полето 'Номер на заповед' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Максималната позволена дължина е 100 за полето 'Номер на заповед за утвърждаване, като външен експерт'")]
        [Display(Name = "Номер на заповед за утвърждаване, като външен експерт")]
        public string? OrderNumber { get; set; }

        [Display(Name = "Дата на заповедта с която е включен в ЕК")]
        public DateTime? DateOrderIncludedExpertCommission { get; set; }

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
    }
}
