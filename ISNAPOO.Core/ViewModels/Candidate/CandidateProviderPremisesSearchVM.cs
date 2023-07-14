using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderPremisesSearchVM
    {
        public string? PremisesName { get; set; }
        public string Owner { get; set; }
        public string LicenseNumber { get; set; }
        public int? IdSpeciality { get; set; }
        public int? IdLocation { get; set; }
        public int idLocation { get; set; }
        public int? IdKvStatus { get; set; }
        public int? IdTypeOfEducation { get; set; }
        public int? IdComplianceDOC { get; set; }
        public int? IdOwnerShip { get; set; }
        public int idDistrict { get; set; }
        public int idMunicipality { get; set; }
        public int? IdProfession { get; set; }
        public DateTime? CreationDateFrom { get; set; }//Дата на създаване От
        public DateTime? CreationDateTo { get; set; }//Дата на създаване До
        public DateTime? ModifyDateFrom { get; set; }//Дата на последна актуализация От
        public DateTime? ModifyDateTo { get; set; }//Дата на последна актуализация До
        public bool IsNAPOOCheck { get; set; }//Извършена проверка от експерт на НАПОО
        public DateTime? NAPOOCheckDateFrom { get; set; }//Дата на проверка От
        public DateTime? NAPOOCheckDateTo { get; set; }//Дата на проверка До

    }
}
