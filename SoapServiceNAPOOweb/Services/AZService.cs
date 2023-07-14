using Data.Models.Data.Candidate;
using Data.Models.Data.SPPOO;
using Data.Models.DB;

using SoapServiceNAPOOweb.Models.Az.getCipoData;
using SoapServiceNAPOOweb.Models.Az.getCPODataResponse;
using SoapServiceNAPOOweb.Models.Az.getMTB;
using SoapServiceNAPOOweb.Models.Az.getSPOOList;
using SoapServiceNAPOOweb.Models.Az.getTrainers;
using SoapServiceNAPOOweb.Models.Az.checkSpecialityStatus;

using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Common.Constants;
using Microsoft.Extensions.Logging;
using ISNAPOO.Core.XML.Course;
using Data.Models.Data.ProviderData;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;

namespace SoapServiceNAPOOweb.Services
{
    public class AZService : IAZService 
    {
        private readonly ApplicationDbContext context;
        private string wsUsername = "smconsulta";
        private string wsPassword = "smcon12345";

        private readonly ILogger<AZService> _logger;
        public AZService(ApplicationDbContext applicationDbContext, ILogger<AZService> logger)
        {
            context = applicationDbContext;
            this._logger = logger;

            this.wsUsername = this.context.Settings.Where(x => x.SettingIntCode == "wsUsernameAZService").First().SettingValue;
            this.wsPassword = this.context.Settings.Where(x => x.SettingIntCode == "wsPasswordAZService").First().SettingValue;
        }

        public LoadNAPOOgetSPPOOResponseType getSPPOOList(string username, string password, bool active)
        {
            try
            {


                this._logger.LogInformation($"Метод getSPPOOList параметри->username:{username} password:{password} active:{active}");


                if (username == wsUsername && password == wsPassword)
                {


                    var KeyValuesSource = this.context.KeyValues.ToList();
                    var Area = (from Areas in this.context.Areas select Areas).ToList();
                    var ProfessionalDirectionsSource = (from ProfessionalDirections in this.context.ProfessionalDirections select ProfessionalDirections).ToList();
                    var ProfessionsSource = (from Professions in this.context.Professions select Professions).ToList();
                    var SpecialitiesSource = (from Specialities in this.context.Specialities select Specialities).ToList();
                    var SPPOO_OrdersSource = (from SPPOO_Orders in this.context.SPPOOOrders select SPPOO_Orders).ToList();
                    var SPPOO_Speciality_OrdersSource = (from SPPOO_Speciality_Orders in this.context.SpecialityOrders select SPPOO_Speciality_Orders).ToList();

                    var data = new NAPOOgetSPPOO[SpecialitiesSource.Count];
                    for (int i = 0; i < SpecialitiesSource.Count; i++)
                    {
                        var Speciality = SpecialitiesSource[i];
                        DateTime? earliestDate = null;
                        DateTime? oldestDate = null;
                        if (SPPOO_Speciality_OrdersSource.Any(x => x.IdSpeciality == Speciality.IdSpeciality))
                        {
                            List<DateTime> dates = new List<DateTime>();
                            List<DateTime> Deletedates = new List<DateTime>();

                            foreach (var IdOrder in SPPOO_Speciality_OrdersSource.Where(v => v.IdSpeciality == Speciality.IdSpeciality))
                            {
                                if (SPPOO_OrdersSource.Any(v => v.IdOrder == IdOrder.IdSPPOOOrder))
                                {
                                    var order = SPPOO_OrdersSource.Find(v => v.IdOrder == IdOrder.IdSPPOOOrder);
                                    if (order.OrderDate != null && IdOrder.IdTypeChange != 0)
                                    {
                                        if (KeyValuesSource.Find(x => x.IdKeyValue == IdOrder.IdTypeChange).KeyValueIntCode == "Created" || KeyValuesSource.Find(x => x.IdKeyValue == IdOrder.IdTypeChange).KeyValueIntCode == "Changed")
                                        {
                                            dates.Add(order.OrderDate);
                                        }
                                        else
                                        {
                                            Deletedates.Add(order.OrderDate);
                                        }
                                    }
                                }
                            }
                            if (dates.Any())
                            {
                                earliestDate = dates.OrderBy(a => a.Date).First();
                            }
                            if (Deletedates.Any())
                            {
                                oldestDate = Deletedates.OrderByDescending(a => a.Date).First();
                            }
                        }
                        NAPOOgetSPPOO result = new NAPOOgetSPPOO()
                        {
                            vet_group_id = Area.First(x => x.IdArea == 
                            ProfessionalDirectionsSource.First(v => v.IdProfessionalDirection == 
                            ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).IdProfessionalDirection).IdArea).IdArea,

                            vet_group_name = Area.First(x => x.IdArea == 
                            ProfessionalDirectionsSource.First(v => v.IdProfessionalDirection == 
                            ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).IdProfessionalDirection).IdArea).Name,

                            vet_group_number = int.Parse(Area.First(x => x.IdArea == 
                            ProfessionalDirectionsSource.First(v => v.IdProfessionalDirection == 
                            ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).IdProfessionalDirection).IdArea).Code),

                            group_is_valid = KeyValuesSource.First(x => x.IdKeyValue == 
                            Area.First(x => x.IdArea == 
                            ProfessionalDirectionsSource.First(v => v.IdProfessionalDirection == 
                            ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).IdProfessionalDirection).IdArea).IdStatus).KeyValueIntCode == "Active" ? true : false,



                            vet_area_id = ProfessionalDirectionsSource.First(v => v.IdProfessionalDirection == 
                            ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).IdProfessionalDirection).IdProfessionalDirection,

                            vet_area_name = ProfessionalDirectionsSource.First(v => v.IdProfessionalDirection == 
                            ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).IdProfessionalDirection).Name,

                            vet_area_number = int.Parse(ProfessionalDirectionsSource.First(v => v.IdProfessionalDirection == 
                            ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).IdProfessionalDirection).Code),

                            area_is_valid = KeyValuesSource.First(x => x.IdKeyValue == 
                            ProfessionalDirectionsSource.First(v => v.IdProfessionalDirection == 
                            ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).IdProfessionalDirection).IdStatus).KeyValueIntCode == "Active" ? true : false,



                            vet_profession_id = ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).IdProfession,

                            vet_profession_name = ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).Name,

                            vet_profession_number = int.Parse(ProfessionsSource.First(y => y.IdProfession == 
                            Speciality.IdProfession).Code),

                            profession_is_valid = KeyValuesSource.First(x => x.IdKeyValue == 
                            ProfessionsSource.First(y => y.IdProfession ==
                            Speciality.IdProfession).IdStatus).KeyValueIntCode == "Active" ? true : false,



                            vet_speciality_id = Speciality.IdSpeciality,
                            vet_speciality_name = Speciality.Name,
                            vet_speciality_number = int.Parse(Speciality.Code),
                            speciality_is_valid = KeyValuesSource.First(x => x.IdKeyValue == Speciality.IdStatus).KeyValueIntCode == "Active" ? true : false,
                            vet_speciality_vqs = KeyValuesSource.First(x => x.IdKeyValue == Speciality.IdVQS).DefaultValue2,
                            speciality_start_date = earliestDate.HasValue ? earliestDate.Value: null,
                            speciality_end_date = oldestDate.HasValue ? oldestDate.Value : null,
                        };
                        data[i] = result;
                    }
                    return new LoadNAPOOgetSPPOOResponseType() { data = data.Where(x => 
                    x.area_is_valid == active 
                    && x.group_is_valid == active
                    && x.profession_is_valid == active 
                    && x.speciality_is_valid == active).ToArray(), message = "Успешно!!!!", status = true };
                    
                }
                else
                {

                    return new LoadNAPOOgetSPPOOResponseType()
                    {
                        status = false,
                        message = "Грешно потребителко име или парола!"
                    };
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError("Грешка в метода getSPPOOList");
                this._logger.LogError($"Exception.Message:{ex.Message}");
                this._logger.LogError($"Exception.StackTrace:{ex.StackTrace}");
           
                return new LoadNAPOOgetSPPOOResponseType() { message = ex.Message, status = false };
            }

        }

        public LoadNAPOOgetCpoDataResponseType getCPOData(string username, string password, string licence_number, string bulstat)
        {
            try
            {

                this._logger.LogInformation($"Метод getCPOData параметри->username:{username} password:{password} licence_number:{licence_number} bulstat:{bulstat}");



                if (username == wsUsername && password == wsPassword)
                {
                    var KeyValueSource = (from KeyValue in this.context.KeyValues select KeyValue).ToList();
                    var kvCpoLicensing = KeyValueSource.First(x => x.KeyValueIntCode == "LicensingCPO").IdKeyValue;

                    var LocationSource = this.context.Locations.ToList();

                    var MunicipalitySource = this.context.Municipalities.ToList();

                    var DistrictSource = this.context.Districts.ToList();



                    var candidateProvider = (from cp in this.context.CandidateProviders 
                                                                where cp.LicenceNumber == licence_number 
                                                                && cp.PoviderBulstat == bulstat 
                                                                && cp.IdTypeLicense == kvCpoLicensing
                                                                && cp.IsActive == true
                                                                select cp).FirstOrDefault();


                    if (candidateProvider == null) 
                    {
                        return new LoadNAPOOgetCpoDataResponseType()
                        {
                            status = false,
                            message = "Не съществува ЦПО с този булстат и номер на лиценз"
                        };
                    }
                    


                    var listProviderSpecialities = (
                                                            from cps in this.context.CandidateProviderSpecialities 
                                                            where cps.IdCandidate_Provider == candidateProvider.IdCandidate_Provider 
                                                            select cps).ToList();

                    var SpecialitySource = (from sp in this.context.Specialities 
                                            join cps in this.context.CandidateProviderSpecialities on sp.IdSpeciality equals cps.IdSpeciality
                                            where cps.IdCandidate_Provider == candidateProvider.IdCandidate_Provider
                                            select sp).ToList();

                    var ProfessionSource = (from profession in this.context.Professions 
                                            join sp in this.context.Specialities on profession.IdProfession equals sp.IdProfession
                                            join cps in this.context.CandidateProviderSpecialities on sp.IdSpeciality equals cps.IdSpeciality
                                            where cps.IdCandidate_Provider == candidateProvider.IdCandidate_Provider
                                            select profession
                                            ).ToList();
                   
                   

                    var SPPOO_Speciality_OrdersSource = (
                        from so in this.context.SpecialityOrders
                        join sp in this.context.Specialities on so.IdSpeciality equals sp.IdSpeciality
                        join cps in this.context.CandidateProviderSpecialities on sp.IdSpeciality equals cps.IdSpeciality
                        where cps.IdCandidate_Provider == candidateProvider.IdCandidate_Provider 
                        select so).ToList();

                    var SPPOO_OrdersSource = (
                        from o in this.context.SPPOOOrders
                        join so in this.context.SpecialityOrders on o.IdOrder equals so.IdSPPOOOrder
                        join sp in this.context.Specialities on so.IdSpeciality equals sp.IdSpeciality
                        join cps in this.context.CandidateProviderSpecialities on sp.IdSpeciality equals cps.IdSpeciality
                        where cps.IdCandidate_Provider == candidateProvider.IdCandidate_Provider                         
                        select o).ToList();


                    List<NAPOOgetCpoData> data = new List<NAPOOgetCpoData>();
                   
                    if (candidateProvider != null)
                    {


                        foreach (var Speciality in listProviderSpecialities)
                        {
                            DateTime? earliestDate = null;
                            DateTime? oldestDate = null;
                            if (SPPOO_Speciality_OrdersSource.Any(x => x.IdSpeciality == Speciality.IdSpeciality))
                            {
                                List<DateTime> dates = new List<DateTime>();
                                List<DateTime> Deletedates = new List<DateTime>();

                                foreach (var IdOrder in SPPOO_Speciality_OrdersSource.Where(v => v.IdSpeciality == Speciality.IdSpeciality))
                                {
                                    if (SPPOO_OrdersSource.Any(v => v.IdOrder == IdOrder.IdSPPOOOrder))
                                    {
                                        var order = SPPOO_OrdersSource.Find(v => v.IdOrder == IdOrder.IdSPPOOOrder);
                                        if (order.OrderDate != null && IdOrder.IdTypeChange != 0)
                                        {
                                            if (KeyValueSource.Find(x => x.IdKeyValue == IdOrder.IdTypeChange).KeyValueIntCode == "Created" || KeyValueSource.Find(x => x.IdKeyValue == IdOrder.IdTypeChange).KeyValueIntCode == "Changed")
                                            {
                                                dates.Add(order.OrderDate);
                                            }
                                            else
                                            {
                                                Deletedates.Add(order.OrderDate);
                                            }
                                        }
                                    }
                                }
                                if (dates.Any())
                                {
                                    earliestDate = dates.OrderBy(a => a.Date).First();
                                }
                                if (Deletedates.Any())
                                {
                                    oldestDate = Deletedates.OrderByDescending(a => a.Date).First();
                                }
                            }
                            foreach (var Profession in ProfessionSource.Where(x => x.IdProfession == Speciality.Speciality.IdProfession))
                            {
                                var temp = new NAPOOgetCpoData
                                {

                                    provider_id = candidateProvider.IdCandidate_Provider == null ? 0 : candidateProvider.IdCandidate_Provider,

                                    provider_status_id = candidateProvider.IdProviderStatus == null ? 0 : candidateProvider.IdProviderStatus,

                                    provider_status_name = candidateProvider.IdProviderStatus == null ? "" : KeyValueSource.First(x => x.IdKeyValue == candidateProvider.IdProviderStatus).Name,

                                    licence_number = candidateProvider.LicenceNumber == null ? 0 : int.Parse(candidateProvider.LicenceNumber),

                                    provider_bulstat = candidateProvider.PoviderBulstat == null ? 0 : int.Parse(candidateProvider.PoviderBulstat),

                                    provider_owner = candidateProvider.ProviderOwner == null ? "" : candidateProvider.ProviderOwner,

                                    provider_manager = candidateProvider.ManagerName == null ? "" : candidateProvider.ManagerName,
                                    //TODO Да се оправи
                                    licence_status = (int)(candidateProvider.IdLicenceStatus == null ? 0 : candidateProvider.IdLicenceStatus),//CandidateProvider.LicenceNumber == null ? 0: int.Parse(CandidateProvider.LicenceNumber),

                                    provider_city = candidateProvider.IdLocation == 0 ? "" : LocationSource.First(x => x.idLocation == candidateProvider.IdLocation).LocationName,

                                    provider_zip_code = candidateProvider.ZipCode == null ? "" : candidateProvider.ZipCode,

                                    provider_address = candidateProvider.ProviderAddress == null ? "" : candidateProvider.ProviderAddress,

                                    provider_phone1 = candidateProvider.ProviderPhone == null ? "" : candidateProvider.ProviderPhone,
                                    //TODO
                                    provider_phone2 = "",

                                    provider_fax = candidateProvider.ProviderFax == null ? "" : candidateProvider.ProviderFax,

                                    provider_web = candidateProvider.ProviderWeb == null ? "" : candidateProvider.ProviderWeb,

                                    provider_email = candidateProvider.ProviderEmail == null ? "" : candidateProvider.ProviderEmail,

                                    provider_contact_pers = candidateProvider.PersonNameCorrespondence == null ? "" : candidateProvider.PersonNameCorrespondence,

                                    contact_person_ekatte_name = candidateProvider.IdLocationCorrespondence == 0 ? "" : LocationSource.First(x => x.idLocation == candidateProvider.IdLocationCorrespondence).LocationName,

                                    provider_contact_pers_zipcode = candidateProvider.ZipCode == null ? 0 : int.Parse(candidateProvider.ZipCode),

                                    provider_contact_pers_address = candidateProvider.ProviderAddressCorrespondence == null ? "" : candidateProvider.ProviderAddressCorrespondence,

                                    provider_contact_pers_phone1 = candidateProvider.ProviderPhoneCorrespondence == null ? "" : candidateProvider.ProviderPhoneCorrespondence,
                                    //TODO
                                    provider_contact_pers_phone2 = "",

                                    provider_contact_pers_fax = candidateProvider.ProviderFaxCorrespondence == null ? "" : candidateProvider.ProviderFaxCorrespondence,

                                    //TODO: Да се тества
                                    provider_contact_pers_email = candidateProvider.ProviderEmailCorrespondence == null ? "" : candidateProvider.ProviderEmailCorrespondence,

                                    bool_is_brra = candidateProvider.IdProviderRegistration == 0 ? false : KeyValueSource.First(x => x.IdKeyValue == candidateProvider.IdProviderRegistration).KeyValueIntCode == "CommercialRegister" ? true : false,

                                    vc_provider_profile = candidateProvider.AdditionalInfo == null ? "" : candidateProvider.AdditionalInfo,

                                    vc_provider_name = candidateProvider.ProviderName == null ? "" : candidateProvider.ProviderName,

                                    //TODO: да се тества
                                    provider_licence_date = candidateProvider.LicenceDate == null ? "" : candidateProvider.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT),

                                    vet_profession_id = Profession.IdProfession == null ? 0 : Profession.IdProfession,

                                    vet_profession_number = Profession.Code == null ? 0 : int.Parse(Profession.Code),

                                    vet_profession_name = Profession.Name == null ? "" : Profession.Name,

                                    profession_is_valid = KeyValueSource.FirstOrDefault(x => x.IdKeyValue == Profession.IdStatus).KeyValueIntCode == "Active" ? true : false,

                                    vet_speciality_id = Speciality.IdSpeciality == null ? 0 : Speciality.IdSpeciality,

                                    vet_speciality_number = Speciality.Speciality.Code == null ? 0 : int.Parse(Speciality.Speciality.Code),

                                    vet_speciality_name = Speciality.Speciality.Name == null ? "" : Speciality.Speciality.Name,


                                    //TODO: Стария вариант е число 1, при нас 'първа'
                                    vet_speciality_vqs = Speciality.Speciality.IdVQS == 0 ? "" : KeyValueSource.First(x => x.IdKeyValue == Speciality.Speciality.IdVQS).DefaultValue2,

                                    speciality_is_valid = Speciality.Speciality.IdStatus == 0 ? false : KeyValueSource.First(x => x.IdKeyValue == Speciality.Speciality.IdStatus).KeyValueIntCode == "Active" ? true : false,

                                    speciality_date_start = earliestDate == null ? "" : earliestDate.Value.ToString(GlobalConstants.DATE_FORMAT),

                                    speciality_date_end = oldestDate == null ? "" : oldestDate.Value.ToString(GlobalConstants.DATE_FORMAT),

                                    //TODO: формата е 2014-06-27 при нас 27.06.2014
                                    speciality_date_licence = Speciality.LicenceData == null ? "" : Speciality.LicenceData.Value.ToString(GlobalConstants.DATE_FORMAT),

                                    provider_spec_is_valid = true,                                    
                                };
                                data.Add(temp);
                            }
                        }
                        return new LoadNAPOOgetCpoDataResponseType() { data = data.ToArray(), message = "Успешно!!!!", status = true };
                    }
                    else
                    {
                        return new LoadNAPOOgetCpoDataResponseType()
                        {
                            status = false,
                            message = "Не съществува ЦПО с този булстат и номер на лиценз"
                        };
                    }
                }
                else
                {
                    return new LoadNAPOOgetCpoDataResponseType()
                    {
                        status = false,
                        message = "Грешно потребителко име или парола!"
                    };
                }


            }
            catch (Exception ex)
            {

                this._logger.LogError("Грешка в метода getCPOData");
                this._logger.LogError($"Exception.Message:{ex.Message}");
                this._logger.LogError($"Exception.StackTrace:{ex.StackTrace}");

                return new LoadNAPOOgetCpoDataResponseType() { message = ex.ToString(), status = false };
            }
        }

        public LoadNAPOOgetCipoDataResponseType getCipoData(string username, string password, string licence_number, string bulstat)
        {
            try
            {
                if (username == wsUsername && password == wsPassword)
                {
                    var KeyValueSource = (from KeyValue in this.context.KeyValues select KeyValue).ToList();
                    var kvCpoLicensing = KeyValueSource.First(x => x.KeyValueIntCode == "LicensingCIPO").IdKeyValue;
                    var CandidateProvider = (CandidateProvider)(from CandidateProviderDb in this.context.CandidateProviders 
                                                                 where CandidateProviderDb.LicenceNumber == licence_number
                                                                 && CandidateProviderDb.PoviderBulstat == bulstat
                                                                 && CandidateProviderDb.IdTypeLicense == kvCpoLicensing
                                                                 && CandidateProviderDb.IsActive == true
                                                                 select CandidateProviderDb).FirstOrDefault();
                    var LocationSource = (from Location in this.context.Locations select Location).ToList();
                    var MunicipalitySource = (from Municipality in this.context.Municipalities select Municipality).ToList();
                    var DistrictSource = (from District in this.context.Districts select District).ToList();
                    NAPOOgetCipoData[] data = new NAPOOgetCipoData[1];
                    if (CandidateProvider != null)
                    {
                        data[0] = new NAPOOgetCipoData()
                        {
                            provider_id = CandidateProvider.IdCandidate_Provider  == null ? 0 : CandidateProvider.IdCandidate_Provider,

                            provider_status_id = CandidateProvider.IdProviderStatus == null ? 0 : CandidateProvider.IdProviderStatus,

                            provider_status_name = CandidateProvider.IdProviderStatus == null ? "" : KeyValueSource.First(x => x.IdKeyValue == CandidateProvider.IdProviderStatus).Name,

                            licence_number = CandidateProvider.LicenceNumber == null ? 0 : int.Parse(CandidateProvider.LicenceNumber),

                            provider_bulstat = CandidateProvider.PoviderBulstat == null ? 0 : int.Parse(CandidateProvider.PoviderBulstat),

                            provider_owner = CandidateProvider.ProviderOwner == null ? "" : CandidateProvider.ProviderOwner,

                            provider_manager = CandidateProvider.ManagerName == null ? "" : CandidateProvider.ManagerName,
                            //TODO
                            licence_status = CandidateProvider.IdLicenceStatus == null ? 0 : (int)CandidateProvider.IdLicenceStatus,

                            provider_city = CandidateProvider.IdLocation == 0 ? "" : LocationSource.First(x => x.idLocation == CandidateProvider.IdLocation).LocationName,

                            provider_zip_code = CandidateProvider.ZipCode == null ? "" : CandidateProvider.ZipCode,

                            provider_address = CandidateProvider.ProviderAddress == null ? "" : CandidateProvider.ProviderAddress,

                            provider_phone1 = CandidateProvider.ProviderPhone == null ? "" : CandidateProvider.ProviderPhone,

                            provider_phone2 = "",

                            provider_fax = CandidateProvider.ProviderFax == null ? "" : CandidateProvider.ProviderFax,

                            provider_web = CandidateProvider.ProviderWeb == null ? "" : CandidateProvider.ProviderWeb,

                            provider_email = CandidateProvider.ProviderEmail == null ? "" : CandidateProvider.ProviderEmail,

                            provider_contact_pers = CandidateProvider.PersonNameCorrespondence == null ? "" : CandidateProvider.PersonNameCorrespondence,

                            contact_person_ekatte_name = CandidateProvider.IdLocationCorrespondence == 0 ? "" : LocationSource.First(x => x.idLocation == CandidateProvider.IdLocationCorrespondence).LocationName,

                            provider_contact_pers_zipcode = CandidateProvider.ZipCodeCorrespondence == null ? "" : CandidateProvider.ZipCodeCorrespondence,

                            provider_contact_pers_address = CandidateProvider.ProviderAddressCorrespondence == null ? "" : CandidateProvider.ProviderAddressCorrespondence,

                            provider_contact_pers_phone1 = CandidateProvider.ProviderPhoneCorrespondence == null ? "" : CandidateProvider.ProviderPhoneCorrespondence,

                            provider_contact_pers_phone2 = "",

                            provider_contact_pers_fax = CandidateProvider.ProviderFaxCorrespondence == null ? "" : CandidateProvider.ProviderFaxCorrespondence,

                            provider_contact_pers_email = CandidateProvider.ProviderEmailCorrespondence == null ? "" : CandidateProvider.ProviderEmailCorrespondence,

                            bool_is_brra = CandidateProvider.IdProviderRegistration == 0 ? false : KeyValueSource.First(x => x.IdKeyValue == CandidateProvider.IdProviderRegistration).KeyValueIntCode == "CommercialRegister" ? true : false,

                            vc_provider_profile = CandidateProvider.AdditionalInfo == null ? "" : CandidateProvider.AdditionalInfo,

                            vc_provider_name = CandidateProvider.ProviderName == null ? "" : CandidateProvider.ProviderName,

                            provider_licence_date = CandidateProvider.LicenceDate,

                        };



                        return new LoadNAPOOgetCipoDataResponseType() { status = true, message = "Успешно !!!", data = data };
                    }
                    else
                    {
                        return new LoadNAPOOgetCipoDataResponseType()
                        {
                            status = false,
                            message = "Не съществува ЦИПО с този булстат и номер на лиценз"
                        };
                    }
                }
                else
                {
                    return new LoadNAPOOgetCipoDataResponseType()
                    {
                        status = false,
                        message = "Грешно потребителко име или парола!"
                    };
                }


            }
            catch (Exception ex)
            {
                return new LoadNAPOOgetCipoDataResponseType() { message = ex.ToString(), status = false };
            }
        }

        public LoadNAPOOgetTrainersDataResponseType getTrainers(string username, string password, int licence_number, string bulstat, int int_area)
        {
            try
            {
                if (username == wsUsername && password == wsPassword)
                {
                  

                    var trainers = from pd in this.context.ProfessionalDirections
                               join cptp in this.context.CandidateProviderTrainerProfiles on pd.IdProfessionalDirection equals cptp.IdProfessionalDirection
                               join cpt in this.context.CandidateProviderTrainers on cptp.IdCandidateProviderTrainer equals cpt.IdCandidateProviderTrainer
                               join cp in this.context.CandidateProviders on cpt.IdCandidate_Provider equals cp.IdCandidate_Provider
                               where pd.IdProfessionalDirection == int_area && 
                                      cp.PoviderBulstat == bulstat && 
                                      cp.LicenceNumber == licence_number.ToString() &&
                                      cp.IsActive == true &&
                                      cp.IdLicenceStatus != null
                               select new NAPOOgetTrainersData()
                               {
                                   id = cpt.IdCandidateProviderTrainer,
                                   trainer_name = cpt.FirstName + " " + cpt.SecondName + " " + cpt.FamilyName,
                                   egn = cpt.Indent,
                                   licence_number = int.Parse(cp.LicenceNumber),
                                   area_id = pd.IdProfessionalDirection,
                                   area_name = pd.Name,
                                   theory_practice = (cptp.IsPractice && cptp.IsTheory) ? 3 : (cptp.IsPractice ? 2 : (cptp.IsTheory ? 1 : 0)),//3 Теория и практика, 2 практика, 1 Теория, 0 не е посочено
                                   theory_practice_recoded = (cptp.IsPractice && cptp.IsTheory) ? "Теория и практика" : (cptp.IsPractice ? "практика" : (cptp.IsTheory ? "Теория" : "не е посочено")),//3 Теория и практика, 2 практика, 1 Теория, 0 не е посочено
                                   vet_area_qualified = cptp.IsProfessionalDirectionQualified ? 1 : 0,
                                   vet_area_qualified_recoded = cptp.IsProfessionalDirectionQualified ? "да" : "не"
                               };

                    //var ProffesionalDirectionSource = (from ProfiessionalDirections in this.context.ProfessionalDirections select ProfiessionalDirections).ToList();


                    
                     

                    return new LoadNAPOOgetTrainersDataResponseType()
                    { data = trainers.ToArray(), message = "Успешно", status = true };
                }
                else
                {
                    return new LoadNAPOOgetTrainersDataResponseType()
                    {
                        status = false,
                        message = "Грешно потребителко име или парола!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoadNAPOOgetTrainersDataResponseType() { message = ex.ToString(), status = false };
            }
        }

        public LoadNAPOOgetMTBDataResponseType getMTB(string username, string password, int licence_number, string bulstat)
        {
            try
            {
                if (username == wsUsername && password == wsPassword)
                {

                    var kvSource = this.context.KeyValues.Where(x => x.KeyType.KeyTypeIntCode == "TrainingType").ToList();

                    var kvTheoryTraining = kvSource.First(x => x.KeyValueIntCode == "TheoryTraining");//Обучение по теория
                    var kvPracticalTraining = kvSource.First(x => x.KeyValueIntCode == "PracticalTraining");//Обучение по практика
                    var kvTrainingInTheoryAndPractice = kvSource.First(x => x.KeyValueIntCode == "TrainingInTheoryAndPractice");//Обучение по теория и практика

                    

                    var mtbs = (from cpp in this.context.CandidateProviderPremises
                               join cp in this.context.CandidateProviders on cpp.IdCandidate_Provider equals cp.IdCandidate_Provider
                               join cpps in this.context.CandidateProviderPremisesSpecialities on cpp.IdCandidateProviderPremises equals cpps.IdCandidateProviderPremises
                               
                               join loc in this.context.Locations on cpp.IdLocation equals loc.idLocation into grLocation
                               from location in grLocation.DefaultIfEmpty()

                               where cp.PoviderBulstat == bulstat &&
                                     cp.LicenceNumber == licence_number.ToString() &&
                                     cp.IsActive == true &&
                                     cp.IdLicenceStatus != null

                               group cpps by new
                               {
                                   cpp.IdCandidateProviderPremises,
                                   cpp.PremisesName,
                                   cpp.IdLocation,
                                   cpp.ProviderAddress,
                                   cpp.ZipCode,
                                   //cpp.PremisesNote,
                                   cp.IdCandidate_Provider,
                                   cp.PoviderBulstat,
                                   cp.LicenceNumber,
                                   cpps.IdUsage,
                                   location.LocationName,
                                   location.LocationCode,
                                   location.PostCode,
                               } into premises

                                

                               select new NAPOOgetMTBData()
                               {
                                   id = premises.Key.IdCandidateProviderPremises,
                                   licence_number = licence_number,
                                   city = (premises.Key.IdLocation != null ? premises.Key.LocationName :String.Empty),
                                   provider_premise_ekatte = (premises.Key.IdLocation != null ? premises.Key.LocationCode : String.Empty),
                                   provider_premise_address = premises.Key.ProviderAddress,
                                   provider_premise_name = premises.Key.PremisesName,
                                   //TODO
                                   provider_premise_notes = "TEST",//premises.Key.PremisesNote,
                                   provider_id = premises.Key.IdCandidate_Provider,
                                   post_code = (premises.Key.IdLocation != null ? premises.Key.PostCode : 0),
                                   provider_premise_speciality_usage = 
                                    (
                                        premises.Any(x=>x.IdUsage == kvTrainingInTheoryAndPractice.IdKeyValue) ? 3 : 
                                        premises.Any(x => x.IdUsage == kvTheoryTraining.IdKeyValue) && premises.Any(x => x.IdUsage == kvPracticalTraining.IdKeyValue) ? 3 :
                                        premises.Any(x => x.IdUsage == kvTheoryTraining.IdKeyValue) ? 1 :
                                        premises.Any(x => x.IdUsage == kvPracticalTraining.IdKeyValue) ? 2 : 0    
                                     )

                               }).ToList();

                    //var CandidateProviderSource = (from CandidateProvider in this.context.CandidateProviders where CandidateProvider.LicenceNumber == licence_number.ToString() && CandidateProvider.PoviderBulstat == bulstat select CandidateProvider.IdCandidate_Provider).First();
                    //var PremisesSource = (from MTB in this.context.CandidateProviderPremises where MTB.IdCandidate_Provider == CandidateProviderSource select MTB).ToList();
                    //var PremisesSpecialitiesSource = (from MTBSpecialities in this.context.CandidateProviderPremisesSpecialities select MTBSpecialities).ToList();
                    //var LocationSource = (from Location in this.context.Locations select Location).ToList();
                    //var SpecialitiesSource = (from Speciality in this.context.Specialities select Speciality).ToList();
                    //List<NAPOOgetMTBData> mtbs = new List<NAPOOgetMTBData>();

                    //foreach (var premises in PremisesSource)
                    //{
                    //    var temp = new NAPOOgetMTBData
                    //    {
                    //        id = premises.IdCandidateProviderPremises,
                    //        licence_number = licence_number,
                    //        city = premises.IdLocation == 0 ? "" : LocationSource.First(x => x.idLocation == premises.IdLocation).LocationName,
                    //        provider_premise_ekatte = LocationSource.First(x => x.idLocation == premises.IdLocation).LocationCode,
                    //        provider_premise_address = premises.ProviderAddress,
                    //        provider_premise_name = premises.PremisesName,
                    //        provider_premise_notes = premises.PremisesNote,                         
                    //        provider_id = premises.IdCandidate_Provider,
                    //        post_code = int.Parse(premises.ZipCode),
                    //    };
                    //    if (SpecialitiesSource.Any(v => v.Code == int_speciality.ToString()))
                    //    {
                    //        if (PremisesSpecialitiesSource.Any(x => x.IdSpeciality == SpecialitiesSource.First(v => v.Code == int_speciality.ToString()).IdSpeciality && x.IdCandidateProviderPremises == premises.IdCandidateProviderPremises))
                    //        {
                    //            temp.provider_premise_speciality_usage = PremisesSpecialitiesSource.First(x => x.IdSpeciality == SpecialitiesSource.First(v => v.Code == int_speciality.ToString()).IdSpeciality && x.IdCandidateProviderPremises == premises.IdCandidateProviderPremises).IdUsage;
                    //        }
                    //    }
                    //    mtbs.Add(temp);

                    //}

                    var mtbDB = this.context.CandidateProviderPremises.Where(x => x.IdCandidate_Provider == mtbs.First().provider_id);

                    if (mtbDB.Any()) 
                    {
                        
                        foreach (var mtb in mtbs) 
                        { 
                            mtb.provider_premise_notes = mtbDB.FirstOrDefault(x => x.IdCandidateProviderPremises == mtb.id).PremisesNote;
                        }
                    }

                    return new LoadNAPOOgetMTBDataResponseType()
                    { 
                        data = mtbs.ToArray(), 
                        message = "Успешно", 
                        status = true 
                    };
                }
                else
                {
                    return new LoadNAPOOgetMTBDataResponseType()
                    {
                        status = false,
                        message = "Грешно потребителко име или парола!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoadNAPOOgetMTBDataResponseType() { message = ex.ToString(), status = false };
            }
        }

        public LoadNAPOOgcheckSpecialityStatusDataResponseType checkSpecialityStatus(string username, string password, int licence_number, string bulstat, int int_speciality)
        {
            try
            {
                if (username == wsUsername && password == wsPassword)
                {
                    List<NAPOOcheckSpecialityStatusData> result = new List<NAPOOcheckSpecialityStatusData>();
                    var CandidateProviderSource = (from candidateProvider in this.context.CandidateProviders

                                                   join candidateProviderSpec in this.context.CandidateProviderSpecialities on candidateProvider.IdCandidate_Provider equals candidateProviderSpec.IdCandidate_Provider

                                                   join spec in this.context.Specialities on candidateProviderSpec.IdSpeciality equals spec.IdSpeciality

                                                   where candidateProvider.LicenceNumber == licence_number.ToString() && candidateProvider.PoviderBulstat == bulstat 
                                               
                                                   select new CandidateProviderSpecialityVM {
                                                   Speciality_Code = spec.Code,
                                                   IdSpeciality = spec.IdSpeciality,
                                                   IdCandidate_Provider = candidateProvider.IdCandidate_Provider
                                                   });

                    if (CandidateProviderSource.Any(x => x.Speciality_Code == int_speciality.ToString()))
                    {
                        result.Add(new NAPOOcheckSpecialityStatusData() {is_valid = true });
                    }
                    else
                    {
                        result.Add(new NAPOOcheckSpecialityStatusData() { is_valid = false });
                    }

                    return new LoadNAPOOgcheckSpecialityStatusDataResponseType()
                    { data = result.ToArray(), message = "Успешно", status = true };
                }
                else
                {
                    return new LoadNAPOOgcheckSpecialityStatusDataResponseType()
                    {
                        status = false,
                        message = "Грешно потребителко име или парола!"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoadNAPOOgcheckSpecialityStatusDataResponseType() { message = ex.ToString(), status = false };
            }
        }
    }
}

