using Data.Models.Data.ProviderData;
using Data.Models.Data.Request;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ISNAPOO.Core.ViewModels.Request
{
    public class ProviderDocumentOfferVM : IMapFrom<ProviderDocumentOffer>, IMapTo<ProviderDocumentOffer>
    {
        public ProviderDocumentOfferVM()
        {
            this.OfferStartDate = DateTime.Now;
        }

        [Key]
        public int IdProviderDocumentOffer { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'ЦПО' е задължително!")]
        [Display(Name = "Връзка с  CPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }
        public CandidateProviderVM CandidateProvider { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на оферта' е задължително!")]
        [Display(Name = "Вид на оферта")]
        public int IdOfferType { get; set; } //Номенклатура (Търся, Предлагам)
        public string OfferTypeName { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на документ' е задължително!")]
        [Display(Name = "Връзка с Тип документ")]
        public int IdTypeOfRequestedDocument { get; set; }
        public TypeOfRequestedDocumentVM TypeOfRequestedDocument { get; set; }

        [Required(ErrorMessage = "Полето 'Брой документи' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Моля, въведете положителна стойност за брой документи!")]
        [Display(Name = "Брой документи")]
        public int CountOffered { get; set; }

        [Required(ErrorMessage = "Полето 'Начална дата на офертата' е задължително!")]
        [Display(Name = "Начална дата на офертата")]
        public DateTime? OfferStartDate { get; set; }

        [Required(ErrorMessage = "Полето 'Крайна дата на офертата' е задължително!")]
        [Display(Name = "Крайна дата на офертата")]
        public DateTime? OfferEndDate { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }
        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }
        #endregion
    }
}
