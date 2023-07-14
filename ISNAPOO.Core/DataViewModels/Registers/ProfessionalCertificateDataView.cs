using Data.Models.Data.SPPOO;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.DataViewModels.Registers
{
    /// <summary>
    /// Регистър на издадените Удостоверения за професионално обучение
    /// </summary>
    public class ProfessionalCertificateDataView
    {

        public int IdClientCourseDocument { get; set; }


        public string FirstName { get; set; }
        public string? SecondName { get; set; }
        public string FamilyName { get; set; }


        /// <summary>
        /// Име на курсист
        /// </summary>
        public string FullName => !string.IsNullOrEmpty(this.SecondName) ? $"{this.FirstName} {this.SecondName} {this.FamilyName}" : $"{this.FirstName} {this.FamilyName}";



        /// <summary>
        ///  ЕГН/ЛНЧ/ИДН
        /// </summary>
        public string ClientIndent { get; set; }

        /// <summary>
        /// Лицензия
        /// </summary>
        public string LicenceNumber { get; set; }


        /// <summary>
        /// Дата на лицанзията
        /// </summary>
        public DateTime? LicenceDate { get; set; }



        /// <summary>
        /// Име на ЦПО,ЦИПО
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Наименование на юридическото лице
        /// </summary>
        public string ProviderOwner { get; set; }

        /// <summary>
        /// Упълномощено лице
        /// </summary>
        public string AttorneyName { get; set; }

        

        /// <summary>
        /// Лицензия с дата
        /// </summary>
        public string LicenceNumberWithDate { get {
                var licenceDateStr = LicenceDate.HasValue ? $"/{LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;
                return $"{LicenceNumber}{licenceDateStr}"; } 
        }

        /// <summary>
        /// ID ЦПО
        /// </summary>
        public int IdCandidateProvider { get; set; }

        /// <summary>
        /// ЦПО
        /// </summary>
        public string CPONameOwnerGrid => $"ЦПО {this.ProviderName} към {this.ProviderOwner}";




    
        /// <summary>
        /// Професия
        /// </summary>
        public string ProfessionCodeAndName { get; set; }

        /// <summary>
        /// Специалност
        /// </summary>
        public string SpecialityCodeAndNameAndVQS { get; set; }

        /// <summary>
        /// Курс
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Период на провеждане
        /// </summary>
        public string timeSpan
        {
            get
            {
                var startDateStr = StartDate.HasValue ? $"/{StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)}" : string.Empty;
                var endDateStr = EndDate.HasValue ? $"/{EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)}" : string.Empty;

                return $"{startDateStr} - {endDateStr} г.";
            }
        }

        /// <summary>
        /// Населено място
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Вид на обучение
        /// </summary>
        public string CourseTypeName { get; set; }

        /// <summary>
        /// документа за ПК
        /// </summary>
        public string DocumentRegNo { get; set; }

        /// <summary>
        /// Дата на регистрационен документ
        /// </summary>
        public DateTime? DocumentDate { get; set; }

        /// <summary>
        /// Очаквана дата за започване на курса
        /// </summary>
        public DateTime? StartDate { get; set; }


        /// <summary>
        /// Очаквана дата за завършване на курса
        /// </summary>
        public DateTime? EndDate { get; set; }


        /// <summary>
        /// Пол
        /// </summary>
        public int? ClientIdSex { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        public string ClientSexName { get; set; }


        /// <summary>
        /// Гражданство
        /// </summary>
        public int? ClientIdNationality { get; set; }

        /// <summary>
        /// Гражданство
        /// </summary>
        public string ClientNationalityName { get; set; }



        /// <summary>
        /// Вид на обучение
        /// IdKeyType	KeyTypeName	            KeyTypeIntCode 
        /// 1035	    Вид рамкова програма    TypeFrameworkProgram
        /// Професионално обучение за придобиване на СПК, Професионално обучение по част от професия, Обучение за ключови компетентности, Мотивационно обучение
        /// </summary>
        public int ProgramIdCourseType { get; set; }

        /// <summary>
        /// Документи за завършено обучение
        /// Сочи към ID на TypeOfRequestedDocument;
        /// </summary>
        public int IdDocumentType { get; set; }

        /// <summary>
        ///  Телефон
        /// </summary>
        public string ProviderPhone { get; set; }


        /// <summary>
        ///  Телефон
        /// </summary>
        public string ProviderEmail { get; set; }


        /// <summary>
        ///  Телефон
        /// </summary>
        public string ProviderAddressCorrespondence { get; set; }














    }
}
