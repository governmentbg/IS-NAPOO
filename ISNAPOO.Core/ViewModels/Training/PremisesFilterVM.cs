using Data.Models.Data.SPPOO;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class PremisesFilterVM
    {
        public PremisesFilterVM()
        {
            specialities = new List<Speciality>();
        }

        public int? IdCandidateProvider { get; set; }
        private int IdCandidateProviderPremises { get; set; }

        public string? LicenceNumber { get; set; }
        public string? ProviderNameAndOwnerForRegister { get; set; }
        public string? PremisesName { get; set; }
        public string? LicenceTypeValue { get; set; }
        public int idDistrict { get; set; }
        public int idMunicipality { get; set; }
        public int idLocation { get; set; }
        public string? LocationName { get; set; }
        public string? ProviderAddress { get; set; }
        public int? IdOwnerShip { get; set; }
        public string? OwnershipValue { get; set; }
        public string Owner { get; set; }
        public int? IdKvStatus { get; set; }
        public int? IdProfession { get; set; }
        public int? IdTypeOfEducation { get; set; }
        public bool IsNAPOOCheck { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }
        public DateTime? ModifyDateFrom { get; set; }
        public DateTime? ModifyDateTo { get; set; }
        public DateTime? NAPOOCheckDateFrom { get; set; }
        public DateTime? NAPOOCheckDateTo { get; set; }

        public List<Speciality> specialities { get; set; }

        public bool IsEmpty()
        {
    return  IdCandidateProvider is null && IdCandidateProviderPremises == 0 && !CreationDateFrom.HasValue && !NAPOOCheckDateTo.HasValue && !NAPOOCheckDateFrom.HasValue! && !ModifyDateTo.HasValue && !ModifyDateFrom.HasValue
            && !CreationDateTo.HasValue && !IdProfession.HasValue && !IdTypeOfEducation.HasValue && !IdKvStatus.HasValue && IsNAPOOCheck == false
            && !IdOwnerShip.HasValue && idDistrict == 0 && idMunicipality == 0 && idLocation == 0 && string.IsNullOrEmpty(Owner)
            && string.IsNullOrEmpty(OwnershipValue) && string.IsNullOrEmpty(ProviderAddress) && string.IsNullOrEmpty(LocationName) && string.IsNullOrEmpty(LicenceTypeValue)
            && string.IsNullOrEmpty(PremisesName) && string.IsNullOrEmpty(ProviderNameAndOwnerForRegister) && string.IsNullOrEmpty(LicenceNumber) && !specialities.Any();
                
        }
    }
}
