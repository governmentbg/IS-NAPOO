using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.ProviderData;
using Data.Models.Data.SPPOO;
using Data.Models.DB;

using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using SoapServiceNAPOOweb.Models.WebIntegrationService.GetCpoLicencedSpecialities;
using SoapServiceNAPOOweb.Models.WebIntegrationService.GetSPPOOList;
using SoapServiceNAPOOweb.Models.WebIntegrationService.GetStatistics;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCipo;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCourses;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCpo;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchDocument;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchStudent;
using Data.Models.Data.SqlView.WebIntegrationService;

using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using RegiX.Class.AVBulstat2.GetStateOfPlay;
using Data.Models.Data.Training;
using DocuServiceReference;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.Common.HelperClasses;
using HarfBuzzSharp;
using Data.Models.Data.DOC;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Bunit.Extensions;
using System.Text;
using ISNAPOO.Common.Framework;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System;
using static ISNAPOO.Common.Enums.EMIEnums;
using System.Web;

namespace SoapServiceNAPOOweb.Services
{
    public class WebIntegrationService : IWebIntegrationService
    {
        private readonly ApplicationDbContext context;

        private string wsUsername = "smconsulta";
        private string wsPassword = "smcon12345";
        //тест ЦПО - Роял 2006 Номер на лицензия = 2013121098

        private readonly ILogger<WebIntegrationService> _logger;

        public WebIntegrationService(ApplicationDbContext applicationDbContext, ILogger<WebIntegrationService> logger)
        {
            this.context = applicationDbContext;

            this._logger = logger;


            this.wsUsername = this.context.Settings.Where(x => x.SettingIntCode == "wsUsernameWebIntegrationService").First().SettingValue;
            this.wsPassword = this.context.Settings.Where(x => x.SettingIntCode == "wsPasswordWebIntegrationService").First().SettingValue;


        }

        public searchCpoResponse searchCpo(string username, string password, int licence_status, string keywords, string language)
        {

            var paramResponse = new LoadNAPOOSearchResponseType();
            try
            {
                var kvSource = this.context.KeyValues.Where(x => x.KeyType.KeyTypeIntCode == "LicenseStatus").ToList();
                if (username == wsUsername && password == wsPassword)
                {
                    string kvLicense_StatusId = licence_status.ToString();
                    if (licence_status != 1 && licence_status != 3 && licence_status != 7 && licence_status != 8 && licence_status != 0)
                    {
                        return new searchCpoResponse(new LoadNAPOOSearchResponseType()
                        {
                            status = false,
                            message = "Невалиден статус на лицензия"
                        });
                    }

                    ///Remove special symbols
                    keywords = keywords.Replace("'", " ");
                    keywords = keywords.Replace(";", " ");
                    keywords = keywords.Replace("--", " ");
                    keywords = keywords.Replace("$", " ");
                    keywords = keywords.Replace("%", " ");
                    keywords = keywords.Replace("%", " ");

                    //var dataArray1 = context.Database.ExecuteSqlRaw("EXEC GetCPOSearch {0}, {1}, {2}", new object[3] { keywords, language, kvLicense_StatusId.ToString() });
                    var dataArray = context.NAPOOSearchs.FromSqlRaw("EXEC GetCPOSearch {0}, {1}, {2}", new object[3] { keywords, language, kvLicense_StatusId }).ToArray();


                    string cpoTitle = language.ToLower().Equals("en") ? "CVT" : "ЦПО";
                    string cpoTo = language.ToLower().Equals("en") ? "at" : "към";


                    foreach (var cpo in dataArray)
                    {
                        cpo.provider_name = $"{cpoTitle}{(!string.IsNullOrEmpty(cpo.provider_name) ? " " + cpo.provider_name : string.Empty)} {cpoTo} {cpo.ProviderOwner}";
                    }

                    var result = dataArray;
                    if (result.Any() && result != null)
                    {
                        paramResponse.status = true;
                        paramResponse.message = "Успешно!!!!";
                        paramResponse.data = result;
                    }
                    else
                    {
                        return new searchCpoResponse(new LoadNAPOOSearchResponseType()
                        {
                            status = false,
                            message = "Няма намерени ЦПО-та"
                        });
                    }

                }
                else
                {
                    paramResponse.status = false;
                    paramResponse.message = "Грешно потребителко име или парола!";
                }

                var res = new searchCpoResponse(paramResponse);
                return res;

            }
            catch (Exception ex)
            {
                paramResponse = new LoadNAPOOSearchResponseType();
                paramResponse.status = false;
                paramResponse.message = ex.Message;
                var res = new searchCpoResponse(paramResponse);
                return res;
            }
        }
        public getCpoLicencedSpecialitiesResponse getCpoLicencedSpecialities(string username, string password, int cpoId, string language)
        {
            try
            {
                var paramResponse = new NAPOOgetCpoLicencedSpecialitiesResponseType();
                if (username == wsUsername && password == wsPassword)
                {
                    if (language == "bg")
                    {
                        var list = (from cps in this.context.CandidateProviderSpecialities

                                    join spec in this.context.Specialities on cps.IdSpeciality equals spec.IdSpeciality

                                    join kvVQS in this.context.KeyValues on spec.IdVQS equals kvVQS.IdKeyValue

                                    join profession in this.context.Professions on spec.IdProfession equals profession.IdProfession

                                    join professionalDirection in this.context.ProfessionalDirections on profession.IdProfessionalDirection equals professionalDirection.IdProfessionalDirection

                                    join area in this.context.Areas on professionalDirection.IdArea equals area.IdArea

                                    where cps.IdCandidate_Provider == cpoId

                                    orderby spec.Code

                                    select new NAPOOgetCpoLicencedSpecialities
                                    {
                                        id = cps.IdCandidate_Provider,
                                        speciality_vqs = spec.IdVQS.ToString(),
                                        speciality_vqs_recoded = kvVQS.Name,
                                        vet_area_name = area.Name,
                                        vet_area_number = area.Code,
                                        vet_profession_name = profession.Name,
                                        vet_profession_number = profession.Code,
                                        vet_speciality_name = spec.Name,
                                        vet_speciality_number = spec.Code,
                                        vet_specialty_licence_data = cps.LicenceData != null ? cps.LicenceData.Value.ToString(GlobalConstants.DATE_FORMAT) : ""

                                    }


                           ).ToArray();


                        paramResponse.status = true;
                        paramResponse.message = "Успешно!!!!";
                        paramResponse.data = list;
                    }
                    else
                    {
                        var list = (from cps in this.context.CandidateProviderSpecialities

                                    join spec in this.context.Specialities on cps.IdSpeciality equals spec.IdSpeciality

                                    join kvVQS in this.context.KeyValues on spec.IdVQS equals kvVQS.IdKeyValue

                                    join profession in this.context.Professions on spec.IdProfession equals profession.IdProfession

                                    join professionalDirection in this.context.ProfessionalDirections on profession.IdProfessionalDirection equals professionalDirection.IdProfessionalDirection

                                    join area in this.context.Areas on professionalDirection.IdArea equals area.IdArea

                                    where cps.IdCandidate_Provider == cpoId

                                    orderby spec.Code

                                    select new NAPOOgetCpoLicencedSpecialities
                                    {
                                        id = cps.IdCandidate_Provider,
                                        speciality_vqs = spec.IdVQS.ToString(),
                                        speciality_vqs_recoded = kvVQS.NameEN,
                                        vet_area_name = area.NameEN,
                                        vet_area_number = area.Code,
                                        vet_profession_name = profession.NameEN,
                                        vet_profession_number = profession.Code,
                                        vet_speciality_name = spec.NameEN,
                                        vet_speciality_number = spec.Code,
                                        vet_specialty_licence_data = cps.LicenceData != null ? cps.LicenceData.Value.ToString(GlobalConstants.DATE_FORMAT) : ""

                                    }
                           ).ToArray();


                        paramResponse.status = true;
                        paramResponse.message = "Успешно!!!!";
                        paramResponse.data = list;
                    }
                }
                else
                {
                    paramResponse.status = false;
                    paramResponse.message = "Грешно потребителко име или парола!";

                }

                var respones = new getCpoLicencedSpecialitiesResponse(paramResponse);
                return respones;
            }
            catch (Exception ex)
            {
                var paramResponse = new NAPOOgetCpoLicencedSpecialitiesResponseType();
                paramResponse.status = false;
                paramResponse.message = ex.Message;
                var res = new getCpoLicencedSpecialitiesResponse(paramResponse);
                return res;

            }
        }
        public searchCipoResponse searchCipo(string username, string password, int licence_status, string keywords, string language)
        {
            var paramResponse = new LoadNAPOOSearchCipoResponseType();
            try
            {
                if (username == wsUsername && password == wsPassword)
                {
                    var kvSource = this.context.KeyValues.ToList();
                    string kvLicense_StatusId = licence_status.ToString();
                    if (licence_status != 1 && licence_status != 3 && licence_status != 7 && licence_status != 8 && licence_status != 0)
                    {
                        return new searchCipoResponse(new LoadNAPOOSearchCipoResponseType()
                        {
                            status = false,
                            message = "Невалиден статус на лицензия"
                        });
                    }
                    var dataArray = context.NAPOOSearchCipos.FromSqlRaw("EXEC GetCIPOSearch {0}, {1}, {2}", new object[3] { keywords, kvLicense_StatusId, language }).ToArray();

                    paramResponse.status = true;
                    paramResponse.message = "Успешно!!!!";
                    paramResponse.data = dataArray;
                }
                else
                {
                    paramResponse.status = false;
                    paramResponse.message = "Грешно потребителко име или парола!";
                }

                var res = new searchCipoResponse(paramResponse);
                return res;

            }
            catch (Exception ex)
            {

                paramResponse = new LoadNAPOOSearchCipoResponseType();
                paramResponse.status = false;
                paramResponse.message = ex.Message;
                var res = new searchCipoResponse(paramResponse);
                return res;

            }
        }
        public searchCoursesResponse searchCourses(string username, string password, string keywords, string language)
        {
            var paramResponse = new LoadNAPOOSearchCoursesResponseType();
            try
            {
                if (username == wsUsername && password == wsPassword)
                {
                    var list = context.NAPOOSearchCourses.FromSqlRaw("EXEC GetSearchCourses {0}, {1}", new object[2] { keywords, language }).ToArray();


                    paramResponse.status = true;
                    paramResponse.message = "Успешно!!!!";
                    paramResponse.data = list;
                }
                else
                {
                    paramResponse.status = false;
                    paramResponse.message = "Грешно потребителко име или парола!";
                }

                var res = new searchCoursesResponse(paramResponse);
                return res;
            }
            catch (Exception ex)
            {
                paramResponse = new LoadNAPOOSearchCoursesResponseType();
                paramResponse.status = false;
                paramResponse.message = ex.Message;
                var res = new searchCoursesResponse(paramResponse);
                return res;
            }
        }
        public searchStudentResponse searchStudent(string username, string password, string egn, string language)
        {
            var paramResponse = new LoadNAPOOSearchStudentResponseType();
            try
            {
                List<NAPOOSearchStudent> listStudentDocument = new List<NAPOOSearchStudent>();
                if (username == wsUsername && password == wsPassword)
                {
                    if (egn.Any(x => !char.IsNumber(x)))
                    {
                        paramResponse.status = true;
                        paramResponse.message = "За търсения ЕГН няма намерени данни.";
                        return new searchStudentResponse(paramResponse);
                    }


                    //Удостоверение за професионално обучение 3-37
                    //Удостоверение за валидиране на професионална квалификация по част от професия 3-37В
                    //Свидетелство за правоспособност 3-114

                    //НЕ Е НЕОБХОДИМО ВПИСВАНЕ В РЕГИСТЪРА
                    List<string> typeOfRequestedDocumentsNums_OUT_Register = new List<string>() { "3-37", "3-37В", "3-114" };


                    //Свидетелство за професионална квалификация - 3-54
                    //Свидетелство за валидиране на професионална квалификация - 3-54В
                    //Дубликат на свидетелство за професионална квалификация(универсален образец),3-54a
                    //Дубликат на свидетелство за валидиране на професионална квалификация - 3-54aB

                    //НЕОБХОДИМО ВПИСВАНЕ В РЕГИСТЪРА
                    List<string> typeOfRequestedDocumentsNums_IN_Register = new List<string>() { "3-54", "3-54В", "3-54a", "3-54aB" };


                    //Вписан в Регистъра
                    //Преглеждат се докумнети, които са вписани в Регистъра за тип документи 
                    var kvStatusEnteredInTheRegister = this.context.KeyValues.Where(
                                x => x.KeyType.KeyTypeIntCode == "ClientDocumentStatusType" &&
                                     x.KeyValueIntCode == "EnteredInTheRegister").First();

                    if (language == "bg")
                    {

                        var listClientCourseDocument = (from courseClient in this.context.ClientCourses

                                                        join course in this.context.Courses on courseClient.IdCourse equals course.IdCourse
                                                        join candidateProvider in this.context.CandidateProviders on course.IdCandidateProvider equals candidateProvider.IdCandidate_Provider
                                                        join program in this.context.Programs on course.IdProgram equals program.IdProgram
                                                        join speciality in this.context.Specialities on program.IdSpeciality equals speciality.IdSpeciality
                                                        join kvVQS in this.context.KeyValues on speciality.IdVQS equals kvVQS.IdKeyValue
                                                        join profession in this.context.Professions on speciality.IdProfession equals profession.IdProfession

                                                        join candidateLocation in this.context.Locations on candidateProvider.IdLocation equals candidateLocation.idLocation
                                                        join candidateMunicipality in this.context.Municipalities on candidateLocation.idMunicipality equals candidateMunicipality.idMunicipality
                                                        join candidateDistrict in this.context.Districts on candidateMunicipality.idDistrict equals candidateDistrict.idDistrict

                                                        join kvCourseType in this.context.KeyValues on course.IdTrainingCourseType equals kvCourseType.IdKeyValue into grkvCourseType
                                                        from kvTrainingType in grkvCourseType.DefaultIfEmpty()

                                                        join kvCourseFinishedType in this.context.KeyValues on courseClient.IdFinishedType equals kvCourseFinishedType.IdKeyValue

                                                        join clientCourseDocument in this.context.ClientCourseDocuments on courseClient.IdClientCourse equals clientCourseDocument.IdClientCourse
                                                        join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on clientCourseDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument


                                                        where courseClient.Indent == egn.Trim() && kvCourseFinishedType.KeyValueIntCode == "Type1"//Type1   Завършил с документ

                                                         && (
                                                                (clientCourseDocument.IdDocumentStatus == kvStatusEnteredInTheRegister.IdKeyValue &&
                                                                 typeOfRequestedDocumentsNums_IN_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)) ||

                                                                 typeOfRequestedDocumentsNums_OUT_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)

                                                              )

                                                        select new NAPOOSearchStudent()
                                                        {
                                                            client_id = courseClient.IdClientCourse,
                                                            client_name = (courseClient.FirstName != null ? courseClient.FirstName.Substring(0, 1) : string.Empty) + "." + (courseClient.SecondName != null ? courseClient.SecondName.Substring(0, 1) : string.Empty) + "." + (courseClient.FamilyName != null ? courseClient.FamilyName : string.Empty),
                                                            vc_egn = courseClient.Indent,
                                                            city_name = $"обл. {candidateDistrict.DistrictName}, общ. {candidateMunicipality.MunicipalityName}, {candidateLocation.LocationName}",    //обл. София-град, общ. Столична, гр. София           //Населено място на център за професионално обучение
                                                            course_group_id = course.IdCourse,
                                                            course_group_name = course.CourseName,
                                                            course_type_id = (int)(course != null ? course.IdTrainingCourseType != null ? course.IdTrainingCourseType : 0 : 0),
                                                            course_type_name = kvTrainingType != null ? kvTrainingType.Name != null ? kvTrainingType.Name : string.Empty : string.Empty,
                                                            provider_id = candidateProvider.IdCandidate_Provider,
                                                            provider_owner = candidateProvider.ProviderOwner,
                                                            licence_number = Int32.Parse(candidateProvider.LicenceNumber),
                                                            vet_speciality_number = Int32.Parse(speciality.Code),
                                                            vet_speciality_name = speciality.Name,
                                                            speciality_vqs = kvVQS.Order,
                                                            vet_profession_number = Int32.Parse(profession.Code),
                                                            vet_profession_name = profession.Name
                                                        }).ToList();


                        var listValidationClientDocument = (from validationClient in this.context.ValidationClients


                                                            join candidateProvider in this.context.CandidateProviders on validationClient.IdCandidateProvider equals candidateProvider.IdCandidate_Provider
                                                            join speciality in this.context.Specialities on validationClient.IdSpeciality equals speciality.IdSpeciality
                                                            join kvVQS in this.context.KeyValues on speciality.IdVQS equals kvVQS.IdKeyValue
                                                            join profession in this.context.Professions on speciality.IdProfession equals profession.IdProfession

                                                            join kvCourseType in this.context.KeyValues on validationClient.IdCourseType equals kvCourseType.IdKeyValue into grkvCourseType
                                                            from courseType in grkvCourseType.DefaultIfEmpty()

                                                            join candidateLocation in this.context.Locations on candidateProvider.IdLocation equals candidateLocation.idLocation
                                                            join candidateMunicipality in this.context.Municipalities on candidateLocation.idMunicipality equals candidateMunicipality.idMunicipality
                                                            join candidateDistrict in this.context.Districts on candidateMunicipality.idDistrict equals candidateDistrict.idDistrict


                                                            join kvCourseFinishedType in this.context.KeyValues on validationClient.IdFinishedType equals kvCourseFinishedType.IdKeyValue

                                                            join validationClientDocument in this.context.ValidationClientDocuments on validationClient.IdValidationClient equals validationClientDocument.IdValidationClient
                                                            join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on validationClientDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument


                                                            where validationClient.Indent == egn.Trim() && kvCourseFinishedType.KeyValueIntCode == "Type5" //Type5 Придобил СПК по реда на чл.40 от ЗПОО

                                                             && (
                                                              (validationClientDocument.IdDocumentStatus == kvStatusEnteredInTheRegister.IdKeyValue &&
                                                               typeOfRequestedDocumentsNums_IN_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)) ||


                                                               typeOfRequestedDocumentsNums_OUT_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)

                                                            )

                                                            select new NAPOOSearchStudent()
                                                            {
                                                                client_id = validationClient.IdValidationClient,
                                                                client_name = (validationClient.FirstName != null ? validationClient.FirstName.Substring(0, 1) : string.Empty) + "." + (validationClient.SecondName != null ? validationClient.SecondName.Substring(0, 1) : string.Empty) + "." + (validationClient.FamilyName != null ? validationClient.FamilyName : string.Empty),
                                                                vc_egn = validationClient.Indent,
                                                                city_name = $"обл. {candidateDistrict.DistrictName}, общ. {candidateMunicipality.MunicipalityName}, гр. {candidateLocation.LocationName}",    //обл. София-град, общ. Столична, гр. София           //Населено място на център за професионално обучение
                                                                course_group_id = 0,
                                                                course_group_name = String.Empty,
                                                                course_type_id = courseType != null ? courseType.IdKeyValue : 0,
                                                                course_type_name = courseType != null ? courseType.Name : string.Empty,
                                                                provider_id = candidateProvider.IdCandidate_Provider,
                                                                provider_owner = candidateProvider.ProviderOwner,
                                                                licence_number = Int32.Parse(candidateProvider.LicenceNumber),
                                                                vet_speciality_number = Int32.Parse(speciality.Code),
                                                                vet_speciality_name = speciality.Name,
                                                                speciality_vqs = kvVQS.Order,
                                                                vet_profession_number = Int32.Parse(profession.Code),
                                                                vet_profession_name = profession.Name
                                                            }).ToList();



                        listStudentDocument.AddRange(listClientCourseDocument);
                        listStudentDocument.AddRange(listValidationClientDocument);

                        if (listStudentDocument.Count() == 0)
                        {
                            paramResponse.status = false;
                            paramResponse.message = "За търсения ЕГН няма намерени данни.";
                            paramResponse.data = listStudentDocument.ToArray();
                        }
                        else
                        {
                            paramResponse.status = true;
                            paramResponse.message = "Успешно!!!!";
                            paramResponse.data = listStudentDocument.ToArray();
                        }
                    }
                    else
                    {  //ЕН
                        var listClientCourseDocument = (from courseClient in this.context.ClientCourses

                                                        join client in this.context.Clients on courseClient.IdClient equals client.IdClient
                                                        join course in this.context.Courses on courseClient.IdCourse equals course.IdCourse
                                                        join candidateProvider in this.context.CandidateProviders on course.IdCandidateProvider equals candidateProvider.IdCandidate_Provider
                                                        join program in this.context.Programs on course.IdProgram equals program.IdProgram
                                                        join speciality in this.context.Specialities on program.IdSpeciality equals speciality.IdSpeciality
                                                        join kvVQS in this.context.KeyValues on speciality.IdVQS equals kvVQS.IdKeyValue
                                                        join profession in this.context.Professions on speciality.IdProfession equals profession.IdProfession

                                                        join candidateLocation in this.context.Locations on candidateProvider.IdLocation equals candidateLocation.idLocation
                                                        join candidateMunicipality in this.context.Municipalities on candidateLocation.idMunicipality equals candidateMunicipality.idMunicipality
                                                        join candidateDistrict in this.context.Districts on candidateMunicipality.idDistrict equals candidateDistrict.idDistrict

                                                        join kvCourseType in this.context.KeyValues on course.IdTrainingCourseType equals kvCourseType.IdKeyValue into grkvCourseType
                                                        from kvTrainingType in grkvCourseType.DefaultIfEmpty()

                                                        where courseClient.Indent == egn.Trim()

                                                        select new NAPOOSearchStudent()
                                                        {
                                                            client_id = courseClient.IdClientCourse,
                                                            client_name = (client.FirstNameEN != null ? client.FirstNameEN.Substring(0, 1) : string.Empty) + "." + (client.SecondNameEN != null ? client.SecondNameEN.Substring(0, 1) : string.Empty) + "." + (client.FamilyNameEN != null ? client.FamilyNameEN : string.Empty),
                                                            vc_egn = courseClient.Indent,
                                                            city_name = $"obl. {candidateDistrict.DistrictNameEN}, obsht. {candidateMunicipality.MunicipalityNameEN}, gr. {candidateLocation.LocationNameEN}",    // obl. Sofiya-grad, obsht. Stolichna, gr. Sofiya //Населено място на център за професионално обучение
                                                            course_group_id = course.IdCourse,
                                                            course_group_name = course.CourseNameEN,
                                                            course_type_id = (int)(course != null ? course.IdTrainingCourseType != null ? course.IdTrainingCourseType : 0 : 0),
                                                            course_type_name = kvTrainingType != null ? kvTrainingType.NameEN != null ? kvTrainingType.NameEN : string.Empty : string.Empty,
                                                            provider_id = candidateProvider.IdCandidate_Provider,
                                                            provider_owner = candidateProvider.ProviderOwnerEN,
                                                            licence_number = Int32.Parse(candidateProvider.LicenceNumber),
                                                            vet_speciality_number = Int32.Parse(speciality.Code),
                                                            vet_speciality_name = speciality.NameEN,
                                                            speciality_vqs = kvVQS.Order,
                                                            vet_profession_number = Int32.Parse(profession.Code),
                                                            vet_profession_name = profession.NameEN
                                                        }).ToList();


                        var listValidationClientDocument = (from validationClient in this.context.ValidationClients

                                                            join client in this.context.Clients on validationClient.IdClient equals client.IdClient
                                                            join candidateProvider in this.context.CandidateProviders on validationClient.IdCandidateProvider equals candidateProvider.IdCandidate_Provider
                                                            join speciality in this.context.Specialities on validationClient.IdSpeciality equals speciality.IdSpeciality
                                                            join kvVQS in this.context.KeyValues on speciality.IdVQS equals kvVQS.IdKeyValue
                                                            join profession in this.context.Professions on speciality.IdProfession equals profession.IdProfession

                                                            join kvCourseType in this.context.KeyValues on validationClient.IdCourseType equals kvCourseType.IdKeyValue into grkvCourseType
                                                            from courseType in grkvCourseType.DefaultIfEmpty()

                                                            join candidateLocation in this.context.Locations on candidateProvider.IdLocation equals candidateLocation.idLocation
                                                            join candidateMunicipality in this.context.Municipalities on candidateLocation.idMunicipality equals candidateMunicipality.idMunicipality
                                                            join candidateDistrict in this.context.Districts on candidateMunicipality.idDistrict equals candidateDistrict.idDistrict

                                                            where validationClient.Indent == egn.Trim()



                                                            select new NAPOOSearchStudent()
                                                            {
                                                                client_id = validationClient.IdValidationClient,
                                                                client_name = (client.FirstNameEN != null ? client.FirstNameEN.Substring(0, 1) : string.Empty) + "." + (client.SecondNameEN != null ? client.SecondNameEN.Substring(0, 1) : string.Empty) + "." + (client.FamilyNameEN != null ? client.FamilyNameEN : string.Empty),
                                                                vc_egn = validationClient.Indent,
                                                                city_name = $"obl. {candidateDistrict.DistrictNameEN}, obsht. {candidateMunicipality.MunicipalityNameEN}, gr. {candidateLocation.LocationNameEN}",    //obl. Sofiya-grad, obsht. Stolichna, gr. Sofiya           //Населено място на център за професионално обучение
                                                                course_group_id = 0,
                                                                course_group_name = String.Empty,
                                                                course_type_id = courseType != null ? courseType.IdKeyValue : 0,
                                                                course_type_name = courseType != null ? courseType.NameEN : string.Empty,
                                                                provider_id = candidateProvider.IdCandidate_Provider,
                                                                provider_owner = candidateProvider.ProviderOwnerEN,
                                                                licence_number = Int32.Parse(candidateProvider.LicenceNumber),
                                                                vet_speciality_number = Int32.Parse(speciality.Code),
                                                                vet_speciality_name = speciality.NameEN,
                                                                speciality_vqs = kvVQS.Order,
                                                                vet_profession_number = Int32.Parse(profession.Code),
                                                                vet_profession_name = profession.NameEN
                                                            }).ToList();



                        listStudentDocument.AddRange(listClientCourseDocument);
                        listStudentDocument.AddRange(listValidationClientDocument);

                        if (listStudentDocument.Count() == 0)
                        {
                            paramResponse.status = false;
                            paramResponse.message = "No data for this personal number.";
                            paramResponse.data = listStudentDocument.ToArray();
                        }
                        else
                        {
                            paramResponse.status = true;
                            paramResponse.message = "Успешно!!!!";
                            paramResponse.data = listStudentDocument.ToArray();
                        }
                    }
                }
                else
                {
                    paramResponse.status = false;
                    paramResponse.message = "Грешно потребителко име или парола!";
                }

                var res = new searchStudentResponse(paramResponse);
                return res;
            }
            catch (Exception ex)
            {
                paramResponse = new LoadNAPOOSearchStudentResponseType();
                paramResponse.status = false;
                paramResponse.message = ex.Message;
                var res = new searchStudentResponse(paramResponse);
                return res;
            }
        }
        public getStatisticsResponse getStatistics(string username, string password, int year, int course_type)
        {
            var paramResponse = new LoadNAPOOStatisticsResponseType();
            try
            {
                if (username == wsUsername && password == wsPassword)
                {
                    var kvSource = this.context.KeyValues.ToList();
                    var course_type_decoded = 0;
                    if (course_type == 1)
                    {
                        course_type_decoded = kvSource.First(x => x.KeyValueIntCode == "ProfessionalQualification").IdKeyValue;
                    }
                    else if (course_type == 2)
                    {
                        course_type_decoded = kvSource.First(x => x.KeyValueIntCode == "PartProfession").IdKeyValue;
                    }
                    else
                    {
                        paramResponse.status = false;
                        paramResponse.message = "Невалиден вид на курс.";
                        var response = new getStatisticsResponse(paramResponse);
                        return response;
                    }

                    var list = context.NAPOOStatistics.FromSqlRaw<NAPOOStatistics>("EXEC GetStatisticsTest {0}, {1}", new object[2] { year, course_type_decoded }).ToArray();


                    paramResponse.status = true;
                    paramResponse.message = "Успешно!!!!";
                    paramResponse.data = list;
                }
                else
                {
                    paramResponse.status = false;
                    paramResponse.message = "Грешно потребителско име или парола!";
                }

                var res = new getStatisticsResponse(paramResponse);
                return res;
            }
            catch (Exception ex)
            {
                paramResponse = new LoadNAPOOStatisticsResponseType();
                paramResponse.status = false;
                paramResponse.message = ex.Message;
                var res = new getStatisticsResponse(paramResponse);
                return res;
            }
        }
        public searchDocumentResponse searchDocument(string username, string password, string egn, string document_id, string language)
        {

            this._logger.LogInformation($"Метод searchDocument параметри->username:{username} password:{password} egn:{egn} document_id:{document_id} language:{language}");

            var paramResponse = new LoadNAPOOSearchDocumentResponseType();
            try
            {

                if (username == wsUsername && password == wsPassword)
                {

                    //Удостоверение за професионално обучение 3-37
                    //Удостоверение за валидиране на професионална квалификация по част от професия 3-37В
                    //Свидетелство за правоспособност 3-114

                    //НЕ Е НЕОБХОДИМО ВПИСВАНЕ В РЕГИСТЪРА
                    List<string> typeOfRequestedDocumentsNums_OUT_Register = new List<string>() { "3-37", "3-37В", "3-114" };


                    //Свидетелство за професионална квалификация - 3-54
                    //Свидетелство за валидиране на професионална квалификация - 3-54В
                    //Дубликат на свидетелство за професионална квалификация(универсален образец),3-54a
                    //Дубликат на свидетелство за валидиране на професионална квалификация - 3-54aB

                    //НЕОБХОДИМО ВПИСВАНЕ В РЕГИСТЪРА
                    List<string> typeOfRequestedDocumentsNums_IN_Register = new List<string>() { "3-54", "3-54В", "3-54a", "3-54aB" };


                    //Вписан в Регистъра
                    //Преглеждат се докумнети, които са вписани в Регистъра за тип документи 
                    var kvStatusEnteredInTheRegister = this.context.KeyValues.Where(
                                x => x.KeyType.KeyTypeIntCode == "ClientDocumentStatusType" &&
                                     x.KeyValueIntCode == "EnteredInTheRegister").First();

                    var resourcesFolderName = this.context.Settings.Where(x => x.SettingIntCode == "ResourcesFolderName").First().SettingValue;
                    var linkSetting = this.context.Settings.Where(x => x.SettingIntCode == "NapooOldISDocsLink").First().SettingValue;

                    var Host = this.context.Settings.Where(x => x.SettingIntCode == "AppSettingHost").First().SettingValue;
                    var Port = this.context.Settings.Where(x => x.SettingIntCode == "AppSettingPort").First().SettingValue;
                    var HttpScheme = this.context.Settings.Where(x => x.SettingIntCode == "AppSettingHttpScheme").First().SettingValue;


                    //linkSetting = "https://localhost:7289/Document/DownloadClientDocument";
                    string linkDownloadClientDocument = $"{HttpScheme}://{Host}:{Port}/Document/DownloadClientDocument";
                    

                    var filePath = $"{resourcesFolderName}\\";

                    List<NAPOOSearchDocument> searchDocuments = new List<NAPOOSearchDocument>();


                    var listClientCourseDocument = (from courseClient in this.context.ClientCourses

                                                    join course in this.context.Courses on courseClient.IdCourse equals course.IdCourse
                                                    join candidateProvider in this.context.CandidateProviders on course.IdCandidateProvider equals candidateProvider.IdCandidate_Provider

                                                    join clientCourseDocument in this.context.ClientCourseDocuments on courseClient.IdClientCourse equals clientCourseDocument.IdClientCourse
                                                    join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on clientCourseDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument
                                                    join courseDocumentUploadedFiles in this.context.CourseDocumentUploadedFiles on clientCourseDocument.IdClientCourseDocument equals courseDocumentUploadedFiles.IdClientCourseDocument


                                                    where courseClient.Indent == egn.Trim()
                                                    //&& clientCourseDocument.DocumentRegNo == document_id
                                                    && (
                                                            (clientCourseDocument.IdDocumentStatus == kvStatusEnteredInTheRegister.IdKeyValue &&
                                                             typeOfRequestedDocumentsNums_IN_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)) ||

                                                             typeOfRequestedDocumentsNums_OUT_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)

                                                          )

                                                    select new NAPOOSearchDocument()
                                                    {
                                                        id = clientCourseDocument.IdClientCourseDocument,
                                                        vc_egn = egn,
                                                        document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,
                                                        document_type_name = (language == "en") ? typeOfRequestedDocuments.DocTypeNameEN : typeOfRequestedDocuments.DocTypeName,
                                                        document_prn_no = clientCourseDocument.DocumentPrnNo,
                                                        document_reg_no = clientCourseDocument.DocumentRegNo,
                                                        document_1_file_name = courseDocumentUploadedFiles.UploadedFileName,
                                                        //document_1_mime_type = String.Empty,
                                                        // document_1_file_contents = System.IO.File.ReadAllBytes(courseDocumentUploadedFiles.UploadedFileName) ,
                                                        licence_number = int.Parse(candidateProvider.LicenceNumber),
                                                        document_date = clientCourseDocument.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT),
                                                        Oid = courseDocumentUploadedFiles.Oid
                                                    }).ToList();

                    var listValidationClientDocument = (from validationClient in this.context.ValidationClients

                                                        join candidateProvider in this.context.CandidateProviders on validationClient.IdCandidateProvider equals candidateProvider.IdCandidate_Provider

                                                        join validationClientDocument in this.context.ValidationClientDocuments on validationClient.IdValidationClient equals validationClientDocument.IdValidationClient
                                                        join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on validationClientDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument
                                                        join validationDocumentUploadedFile in this.context.ValidationDocumentUploadedFiles on validationClientDocument.IdValidationClientDocument equals validationDocumentUploadedFile.IdValidationClientDocument

                                                        where validationClient.Indent == egn.Trim()
                                                        //     && validationClientDocument.DocumentRegNo == document_id
                                                        && (
                                                          (validationClientDocument.IdDocumentStatus == kvStatusEnteredInTheRegister.IdKeyValue &&
                                                           typeOfRequestedDocumentsNums_IN_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)) ||


                                                           typeOfRequestedDocumentsNums_OUT_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)

                                                        )

                                                        select new NAPOOSearchDocument()
                                                        {
                                                            id = validationClientDocument.IdValidationClientDocument,
                                                            vc_egn = egn,
                                                            document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,
                                                            document_type_name = (language == "en") ? typeOfRequestedDocuments.DocTypeNameEN : typeOfRequestedDocuments.DocTypeName,
                                                            document_prn_no = validationClientDocument.DocumentPrnNo,
                                                            document_reg_no = validationClientDocument.DocumentRegNo,
                                                            document_1_file_name = validationDocumentUploadedFile.UploadedFileName,
                                                            //document_1_mime_type = String.Empty,
                                                            //document_1_file_contents = System.IO.File.ReadAllBytes(courseDocumentUploadedFiles.UploadedFileName),
                                                            licence_number = int.Parse(candidateProvider.LicenceNumber),
                                                            document_date = validationClientDocument.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT),
                                                            Oid = validationDocumentUploadedFile.Oid
                                                        }).ToList();




                    this._logger.LogInformation($"{language}.listClientCourseDocument.COUNT:{listClientCourseDocument.Count()}");
                    this._logger.LogInformation($"{language}.listValidationClientDocument.COUNT:{listValidationClientDocument.Count()}");

                    searchDocuments.AddRange(listClientCourseDocument);
                    searchDocuments.AddRange(listValidationClientDocument);


                    Regex regex = new Regex("[^0-9]");

                    searchDocuments = searchDocuments.Where(x => regex.Replace(x.document_reg_no, "").Contains(regex.Replace(document_id, ""))).ToList();

                    foreach (var item in searchDocuments)
                    {
                        string TokenString = string.Empty;

                        if (item.document_1_file_name == "Отвори документа")
                        {
                            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
                            tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>()
                            {
                                new KeyValuePair<string, object>("IsMigrate", "false"),
                                new KeyValuePair<string, object>("document_1_file_name", ""),
                                new KeyValuePair<string, object>("oid", item.Oid)
                            };
                            TokenString = BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams);
                        }
                        else 
                        {

                            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
                            tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>()
                            {
                                new KeyValuePair<string, object>("IsMigrate", "true"),
                                new KeyValuePair<string, object>("document_1_file_name", item.document_1_file_name),
                                new KeyValuePair<string, object>("oid", item.Oid)
                            };
                            TokenString = BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams);
                        }


                        //https://localhost:7289/Document/DownloadClientDocument?TokenString=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjQ4NDI2MDY3NDksIm9pZCI6IjQyNzIzNzQiLCJkb2N1bWVudF8xX2ZpbGVfbmFtZSI6IlVwbG9hZGVkRmlsZXNcXENsaWVudENvdXJzZURvY3VtZW50XFw0NTA5ODZcXDAwMDAwMDAwMDFfMDcuMDYuMjAyMy5wZGYiLCJJc01pZ3JhdGUiOiJmYWxzZSJ9.J9pPfaZvcIn_8_Ty5O-zYQoHNVIh1WikYca3O8zwZu0
                        System.Net.HttpWebRequest request = null;
                        System.Net.HttpWebResponse response = null;
                         

                        request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create($"{linkDownloadClientDocument}?TokenString={TokenString}");
                        request.Timeout = 30000;
                        request.UserAgent = ".NET Client";
                        response = (System.Net.HttpWebResponse)request.GetResponse();
                        var responseStream = response.GetResponseStream();


                        var fileNameFromOldIS = response.Headers["Content-Disposition"].Replace("attachment; filename=", String.Empty).Replace("\"", String.Empty);

                        var fileName111 = HttpUtility.UrlDecode(fileNameFromOldIS, Encoding.UTF8).Split(';')[0];
                         
                       
                     
                        MemoryStream ms = new MemoryStream();
                        responseStream.CopyTo(ms);

                        item.document_1_file_name = Path.GetFileName(fileName111);
                        item.document_1_file_contents = ms.ToArray();
                        item.document_1_mime_type = MimeTypeMap.GetMimeType(fileName111);

                        responseStream.Close();




                        /*
                        if (item.document_1_file_name == "Отвори документа")
                        {
                            var oid = int.Parse(item.Oid);

                            System.Net.HttpWebRequest request = null;
                            System.Net.HttpWebResponse response = null;
                            request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create($"{linkSetting}{oid}");
                            request.Timeout = 30000;
                            request.UserAgent = ".NET Client";
                            response = (System.Net.HttpWebResponse)request.GetResponse();
                            var responseStream = response.GetResponseStream();

                            var fileNameFromOldIS = response.Headers["Content-Disposition"];

                            fileNameFromOldIS = response.Headers["Content-Disposition"].Replace("attachment; filename=", String.Empty).Replace("\"", String.Empty);

                            var bytes = Encoding.UTF8.GetBytes(fileNameFromOldIS);

                            var encoded = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("ISO-8859-1"), bytes);

                            fileNameFromOldIS = Encoding.UTF8.GetString(encoded);

                            MemoryStream ms = new MemoryStream();
                            responseStream.CopyTo(ms);
                             
                            item.document_1_file_name = Path.GetFileName(fileNameFromOldIS);
                            item.document_1_file_contents = ms.ToArray();
                            item.document_1_mime_type = MimeTypeMap.GetMimeType(fileNameFromOldIS);

                            responseStream.Close();

                        }
                        else
                        {

                            string fileName = $"{filePath}\\{item.document_1_file_name}";

                            if (File.Exists(fileName))
                            {
                                item.document_1_file_name = Path.GetFileName(item.document_1_file_name);
                                item.document_1_file_contents = System.IO.File.ReadAllBytes(fileName);
                                item.document_1_mime_type = MimeTypeMap.GetMimeType(item.document_1_file_name);
                            }

                        }
                        */



                    }



                    if (searchDocuments.Count() == 0)
                    {
                        paramResponse.status = false;
                        paramResponse.message = "За търсения ЕГН няма намерени данни.";
                        paramResponse.data = searchDocuments.ToArray();
                    }
                    else
                    {
                        paramResponse.status = true;
                        paramResponse.message = "Успешно!!!!";
                        paramResponse.data = searchDocuments.ToArray();
                    }

                    /*
                    if (language == "bg")
                    {
                       

                        var listClientCourseDocument = (from courseClient in this.context.ClientCourses

                                                        join course in this.context.Courses on courseClient.IdCourse equals course.IdCourse
                                                        join candidateProvider in this.context.CandidateProviders on course.IdCandidateProvider equals candidateProvider.IdCandidate_Provider

                                                        join clientCourseDocument in this.context.ClientCourseDocuments on courseClient.IdClientCourse equals clientCourseDocument.IdClientCourse
                                                        join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on clientCourseDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument
                                                        join courseDocumentUploadedFiles in this.context.CourseDocumentUploadedFiles on clientCourseDocument.IdClientCourseDocument equals courseDocumentUploadedFiles.IdClientCourseDocument


                                                        where courseClient.Indent == egn.Trim()
                                                        //&& clientCourseDocument.DocumentRegNo == document_id
                                                        && (
                                                                (clientCourseDocument.IdDocumentStatus == kvStatusEnteredInTheRegister.IdKeyValue &&
                                                                 typeOfRequestedDocumentsNums_IN_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)) ||

                                                                 typeOfRequestedDocumentsNums_OUT_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)

                                                              )

                                                        select new NAPOOSearchDocument()
                                                        {
                                                            id = clientCourseDocument.IdClientCourseDocument,
                                                            vc_egn = egn,
                                                            document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,
                                                            document_type_name = typeOfRequestedDocuments.DocTypeName,
                                                            document_prn_no = clientCourseDocument.DocumentPrnNo,
                                                            document_reg_no = clientCourseDocument.DocumentRegNo,
                                                            document_1_file_name = courseDocumentUploadedFiles.UploadedFileName,
                                                            //document_1_mime_type = String.Empty,
                                                            // document_1_file_contents = System.IO.File.ReadAllBytes(courseDocumentUploadedFiles.UploadedFileName) ,
                                                            licence_number = int.Parse(candidateProvider.LicenceNumber),
                                                            document_date = clientCourseDocument.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT),
                                                        }).ToList();

                        var listValidationClientDocument = (from validationClient in this.context.ValidationClients

                                                            join candidateProvider in this.context.CandidateProviders on validationClient.IdCandidateProvider equals candidateProvider.IdCandidate_Provider

                                                            join validationClientDocument in this.context.ValidationClientDocuments on validationClient.IdValidationClient equals validationClientDocument.IdValidationClient
                                                            join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on validationClientDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument
                                                            join validationDocumentUploadedFile in this.context.ValidationDocumentUploadedFiles on validationClientDocument.IdValidationClientDocument equals validationDocumentUploadedFile.IdValidationClientDocument

                                                            where validationClient.Indent == egn.Trim()
                                                       //     && validationClientDocument.DocumentRegNo == document_id
                                                            && (
                                                              (validationClientDocument.IdDocumentStatus == kvStatusEnteredInTheRegister.IdKeyValue &&
                                                               typeOfRequestedDocumentsNums_IN_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)) ||

                                                               
                                                               typeOfRequestedDocumentsNums_OUT_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)

                                                            )

                                                            select new NAPOOSearchDocument()
                                                            {
                                                                id = validationClientDocument.IdValidationClientDocument,
                                                                vc_egn = egn,
                                                                document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,
                                                                document_type_name = typeOfRequestedDocuments.DocTypeName,
                                                                document_prn_no = validationClientDocument.DocumentPrnNo,
                                                                document_reg_no = validationClientDocument.DocumentRegNo,
                                                                document_1_file_name = validationDocumentUploadedFile.UploadedFileName,
                                                                //document_1_mime_type = String.Empty,
                                                                //document_1_file_contents = System.IO.File.ReadAllBytes(courseDocumentUploadedFiles.UploadedFileName),
                                                                licence_number = int.Parse(candidateProvider.LicenceNumber),
                                                                document_date = validationClientDocument.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT),
                                                            }).ToList();




                        this._logger.LogInformation($"BG.listClientCourseDocument.COUNT:{listClientCourseDocument.Count()}");
                        this._logger.LogInformation($"BG.listValidationClientDocument.COUNT:{listValidationClientDocument.Count()}");

                        searchDocuments.AddRange(listClientCourseDocument);
                        searchDocuments.AddRange(listValidationClientDocument);


                        Regex regex = new Regex("[^0-9]");

                        searchDocuments = searchDocuments.Where(x => regex.Replace(x.document_reg_no, "").Contains(regex.Replace(document_id, ""))).ToList();

                        foreach (var item in searchDocuments)
                        {
                            string fileName = $"{filePath}\\{item.document_1_file_name}";

                            if (File.Exists(fileName)) 
                            {
                                item.document_1_file_name = Path.GetFileName(item.document_1_file_name);
                                item.document_1_file_contents = System.IO.File.ReadAllBytes(fileName);
                                item.document_1_mime_type = MimeTypeMap.GetMimeType(item.document_1_file_name);
                            } 
                        }
                         


                        if (searchDocuments.Count() == 0)
                        {
                            paramResponse.status = false;
                            paramResponse.message = "За търсения ЕГН няма намерени данни.";
                            paramResponse.data = searchDocuments.ToArray();
                        }
                        else {
                            paramResponse.status = true;
                            paramResponse.message = "Успешно!!!!";
                            paramResponse.data = searchDocuments.ToArray();
                        }

                       
                    }
                    else
                    {//EN 

                        var listClientCourseDocument = (from courseClient in this.context.ClientCourses

                                                        join course in this.context.Courses on courseClient.IdCourse equals course.IdCourse
                                                        join candidateProvider in this.context.CandidateProviders on course.IdCandidateProvider equals candidateProvider.IdCandidate_Provider

                                                        join clientCourseDocument in this.context.ClientCourseDocuments on courseClient.IdClientCourse equals clientCourseDocument.IdClientCourse
                                                        join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on clientCourseDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument
                                                        join courseDocumentUploadedFiles in this.context.CourseDocumentUploadedFiles on clientCourseDocument.IdClientCourseDocument equals courseDocumentUploadedFiles.IdClientCourseDocument

                                                        where courseClient.Indent == egn.Trim() &&
                                                              //clientCourseDocument.DocumentRegNo == document_id &&
                                                              (
                                                                (clientCourseDocument.IdDocumentStatus == kvStatusEnteredInTheRegister.IdKeyValue &&
                                                                 typeOfRequestedDocumentsNums_IN_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)) ||

                                                                 (clientCourseDocument.IdDocumentStatus == null &&
                                                                 typeOfRequestedDocumentsNums_OUT_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber))

                                                              )

                                                        select new NAPOOSearchDocument()
                                                        {
                                                            id = clientCourseDocument.IdClientCourseDocument,
                                                            vc_egn = egn,
                                                            document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,
                                                            document_type_name = typeOfRequestedDocuments.DocTypeNameEN,
                                                            document_prn_no = clientCourseDocument.DocumentPrnNo,
                                                            document_reg_no = clientCourseDocument.DocumentRegNo,
                                                            document_1_file_name = courseDocumentUploadedFiles.UploadedFileName,
                                                            //document_1_mime_type = String.Empty,
                                                            // document_1_file_contents = System.IO.File.ReadAllBytes(courseDocumentUploadedFiles.UploadedFileName) ,
                                                            licence_number = int.Parse(candidateProvider.LicenceNumber),
                                                            document_date = clientCourseDocument.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT),
                                                        }).ToList();

                        var listValidationClientDocument = (from validationClient in this.context.ValidationClients

                                                            join candidateProvider in this.context.CandidateProviders on validationClient.IdCandidateProvider equals candidateProvider.IdCandidate_Provider

                                                            join validationClientDocument in this.context.ValidationClientDocuments on validationClient.IdValidationClient equals validationClientDocument.IdValidationClient
                                                            join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on validationClientDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument
                                                            join validationDocumentUploadedFile in this.context.ValidationDocumentUploadedFiles on validationClientDocument.IdValidationClientDocument equals validationDocumentUploadedFile.IdValidationClientDocument


                                                            where validationClient.Indent == egn.Trim() &&
                                                               // validationClientDocument.DocumentRegNo == document_id &&
                                                                (
                                                                  (validationClientDocument.IdDocumentStatus == kvStatusEnteredInTheRegister.IdKeyValue &&
                                                                   typeOfRequestedDocumentsNums_IN_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)) ||

                                                                   (validationClientDocument.IdDocumentStatus == null &&
                                                                   typeOfRequestedDocumentsNums_OUT_Register.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber))

                                                                )

                                                            select new NAPOOSearchDocument()
                                                            {
                                                                id = validationClientDocument.IdValidationClientDocument,
                                                                vc_egn = egn,
                                                                document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,
                                                                document_type_name = typeOfRequestedDocuments.DocTypeNameEN,
                                                                document_prn_no = validationClientDocument.DocumentPrnNo,
                                                                document_reg_no = validationClientDocument.DocumentRegNo,
                                                                document_1_file_name = validationDocumentUploadedFile.UploadedFileName,
                                                                //document_1_mime_type = String.Empty,
                                                                // document_1_file_contents = System.IO.File.ReadAllBytes(courseDocumentUploadedFiles.UploadedFileName) ,
                                                                licence_number = int.Parse(candidateProvider.LicenceNumber),
                                                                document_date = validationClientDocument.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT),
                                                            }).ToList();


                        searchDocuments.AddRange(listClientCourseDocument);
                        searchDocuments.AddRange(listValidationClientDocument);


                        this._logger.LogInformation($"EN.listClientCourseDocument.COUNT:{listClientCourseDocument.Count()}");
                        this._logger.LogInformation($"EN.listValidationClientDocument.COUNT:{listValidationClientDocument.Count()}");


                        Regex regex = new Regex("[^0-9]");

                        //Премахват се всички символи с изключение на числата
                        searchDocuments = searchDocuments.Where(x => regex.Replace(x.document_reg_no, "").Contains(regex.Replace(document_id, ""))).ToList();

                        foreach (var item in searchDocuments)
                        {
                            string fileName = $"{filePath}\\{item.document_1_file_name}";

                            if (File.Exists(fileName))
                            {
                                item.document_1_file_name = Path.GetFileName(item.document_1_file_name);
                                item.document_1_file_contents = System.IO.File.ReadAllBytes(fileName);
                                item.document_1_mime_type = MimeTypeMap.GetMimeType(item.document_1_file_name);
                            }
                        }


                        if (searchDocuments.Count() == 0)
                        {
                            paramResponse.status = false;
                            paramResponse.message = "За търсения ЕГН няма намерени данни.";
                            paramResponse.data = searchDocuments.ToArray();
                        }
                        else
                        {
                            paramResponse.status = true;
                            paramResponse.message = "Успешно!!!!";
                            paramResponse.data = searchDocuments.ToArray();
                        }


                    }
                    */
                }
                else
                {
                    paramResponse.status = false;
                    paramResponse.message = "Грешно потребителко име или парола!";
                }
                return new searchDocumentResponse(paramResponse);
            }
            catch (Exception ex)
            {
                paramResponse = new LoadNAPOOSearchDocumentResponseType();
                paramResponse.status = false;
                paramResponse.message = ex.Message;
                return new searchDocumentResponse(paramResponse);
            }
        }
        public getSPPOOListResponse getSPPOOList(string username, string password)
        {
            var res = new getSPPOOListResponse();
            LoadSPPOOResponseType paramResponse = new LoadSPPOOResponseType();
            try
            {
                if (username == wsUsername && password == wsPassword)
                {
                    var AreasSource = (from Areas in this.context.Areas select Areas).ToList().Where(x => x.Code != "0" && x.Code != "99").ToList();
                    var ProfessionalDirectionsSource = (from ProfessionalDirections in this.context.ProfessionalDirections select ProfessionalDirections).ToList();
                    var ProfessionsSource = (from Professions in this.context.Professions select Professions).ToList();
                    var SpecialitiesSource = (from Specialities in this.context.Specialities select Specialities).ToList();
                    var SPPOO_OrdersSource = (from SPPOO_Orders in this.context.SPPOOOrders select SPPOO_Orders).ToList();
                    var SPPOO_ProfessionalDirection_OrdersSource = (from SPPOO_ProfessionalDirection_Orders in this.context.ProfessionalDirectionOrders select SPPOO_ProfessionalDirection_Orders).ToList();
                    var SPPOO_Profession_OrdersSource = (from SPPOO_Profession_Orders in this.context.ProfessionOrders select SPPOO_Profession_Orders).ToList();
                    var SPPOO_Speciality_OrdersSource = (from SPPOO_Speciality_Orders in this.context.SpecialityOrders select SPPOO_Speciality_Orders).ToList();
                    var KeyValuesSource = (from KeyValues in this.context.KeyValues select KeyValues).ToList();
                    vet_group[] vet_Groups = new vet_group[AreasSource.Count];

                    for (int y = 0; AreasSource.Count > y; y++)
                    {

                        var SPPOO = AreasSource[y];
                        vet_group temp_vet_Group = new vet_group()
                        {
                            vet_group_id = SPPOO.IdArea,
                            vet_group_name = SPPOO.Name,
                            vet_group_number = int.Parse(SPPOO.Code),
                            vet_group_correction_notes = "",
                            vet_group_is_valid = KeyValuesSource.Find(x => x.IdKeyValue == SPPOO.IdStatus).KeyValueIntCode == "Active" ? true : false,
                            vet_areas = new vet_area[SPPOO.ProfessionalDirections.Count]
                        };
                        if (ProfessionalDirectionsSource.Any(x => x.IdArea == SPPOO.IdArea))
                        {
                            for (int x = 0; SPPOO.ProfessionalDirections.Count > x; x++)
                            {
                                var ProfessionalDirectionList = ProfessionalDirectionsSource.Where(v => v.IdArea == SPPOO.IdArea).ToArray();
                                var Area = ProfessionalDirectionList[x];
                                string Areacorrection_note = "";
                                string PDChangeType = "";
                                if (SPPOO_ProfessionalDirection_OrdersSource.Any(x => x.IdProfessionalDirection == Area.IdProfessionalDirection))
                                {
                                    List<string> temp = new List<string>();
                                    List<string> sOrders = new List<string>();
                                    foreach (var IdOrder in SPPOO_ProfessionalDirection_OrdersSource.Where(v => v.IdProfessionalDirection == Area.IdProfessionalDirection))
                                    {
                                        if (SPPOO_OrdersSource.Any(v => v.IdOrder == IdOrder.IdSPPOOOrder))
                                        {
                                            sOrders = new List<string>();
                                            var order = SPPOO_OrdersSource.Find(v => v.IdOrder == IdOrder.IdSPPOOOrder);
                                            if (!String.IsNullOrEmpty(order.UploadedFileName))
                                            {
                                                sOrders.Add(order.UploadedFileName);
                                            }
                                            if (order.OrderDate != null)
                                            {
                                                sOrders.Add("/" + order.OrderDate.ToString(GlobalConstants.DATE_FORMAT));
                                            }
                                            PDChangeType = KeyValuesSource.Find(x => x.IdKeyValue == IdOrder.IdTypeChange).Name;
                                        }
                                        temp.Add(String.Join("", sOrders));
                                    }
                                    Areacorrection_note = String.Join(", ", temp);
                                }
                                vet_area temp_vet_Area = new vet_area()
                                {
                                    vet_area_id = Area.IdProfessionalDirection,
                                    vet_area_name = Area.Name + (PDChangeType == "" ? null : $"({PDChangeType})"),
                                    vet_area_number = int.Parse(Area.Code),
                                    vet_area_correction_notes = Areacorrection_note,
                                    vet_area_is_valid = KeyValuesSource.Find(x => x.IdKeyValue == Area.IdStatus).KeyValueIntCode == "Active" ? true : false,
                                    vet_professions = new vet_profession[Area.Professions.Count]
                                };
                                if (ProfessionsSource.Any(x => x.IdProfessionalDirection == Area.IdProfessionalDirection))
                                {
                                    for (int j = 0; Area.Professions.Count > j; j++)
                                    {
                                        var ProfessionList = ProfessionsSource.Where(v => v.IdProfessionalDirection == Area.IdProfessionalDirection).ToArray();
                                        var Profession = ProfessionList[j];
                                        string Professioncorrection_note = "";
                                        string ProfChangeType = "";
                                        if (SPPOO_Profession_OrdersSource.Any(x => x.IdProfession == Profession.IdProfession))
                                        {
                                            List<string> temp = new List<string>();
                                            List<string> sOrders = new List<string>();
                                            foreach (var IdOrder in SPPOO_Profession_OrdersSource.Where(v => v.IdProfession == Profession.IdProfession))
                                            {
                                                if (SPPOO_OrdersSource.Any(v => v.IdOrder == IdOrder.IdSPPOOOrder))
                                                {
                                                    sOrders = new List<string>();
                                                    var order = SPPOO_OrdersSource.Find(v => v.IdOrder == IdOrder.IdSPPOOOrder);
                                                    if (!String.IsNullOrEmpty(order.UploadedFileName))
                                                    {
                                                        sOrders.Add(order.UploadedFileName);
                                                    }
                                                    if (order.OrderDate != null)
                                                    {
                                                        sOrders.Add("/" + order.OrderDate.ToString(GlobalConstants.DATE_FORMAT));
                                                    }
                                                    ProfChangeType = KeyValuesSource.Find(x => x.IdKeyValue == IdOrder.IdTypeChange).Name;
                                                }
                                                temp.Add(String.Join("", sOrders));
                                            }
                                            Professioncorrection_note = String.Join(", ", temp);
                                        }
                                        vet_profession temp_vet_Profession = new vet_profession()
                                        {
                                            vet_proffesion_id = Profession.IdProfession,
                                            vet_profession_number = int.Parse(Profession.Code),
                                            vet_profession_name = Profession.Name + (ProfChangeType == "" ? null : $"({ProfChangeType})"),
                                            vet_profession_is_valid = KeyValuesSource.Find(x => x.IdKeyValue == Profession.IdStatus).KeyValueIntCode == "Active" ? true : false,
                                            vet_profession_correction_notes = Professioncorrection_note,
                                            vet_specialities = new vet_speciality[Profession.Specialities.Count]
                                        };
                                        if (SpecialitiesSource.Any(x => x.IdProfession == Profession.IdProfession))
                                        {
                                            for (int i = 0; Profession.Specialities.Count > i; i++)
                                            {
                                                var ListSpeciality = SpecialitiesSource.Where(v => v.IdProfession == Profession.IdProfession).ToArray();
                                                var Speciality = ListSpeciality[i];
                                                string SpecialityCorrection_note = "";
                                                DateTime? earliestDate = null;
                                                DateTime? oldestDate = null;
                                                string ChangeType = "";
                                                if (SPPOO_Speciality_OrdersSource.Any(x => x.IdSpeciality == Speciality.IdSpeciality))
                                                {
                                                    List<string> temp = new List<string>();
                                                    List<string> sOrders = new List<string>();
                                                    List<DateTime> dates = new List<DateTime>();
                                                    List<DateTime> Deletedates = new List<DateTime>();

                                                    foreach (var IdOrder in SPPOO_Speciality_OrdersSource.Where(v => v.IdSpeciality == Speciality.IdSpeciality))
                                                    {
                                                        if (SPPOO_OrdersSource.Any(v => v.IdOrder == IdOrder.IdSPPOOOrder))
                                                        {
                                                            sOrders = new List<string>();
                                                            var order = SPPOO_OrdersSource.Find(v => v.IdOrder == IdOrder.IdSPPOOOrder);
                                                            if (!String.IsNullOrEmpty(order.UploadedFileName))
                                                            {
                                                                sOrders.Add(order.UploadedFileName);
                                                            }
                                                            if (order.OrderDate != null)
                                                            {
                                                                if (KeyValuesSource.FirstOrDefault(x => x.IdKeyValue == (IdOrder.IdTypeChange == 0 ? 1023 : IdOrder.IdTypeChange)).KeyValueIntCode == "Created" || KeyValuesSource.FirstOrDefault(x => x.IdKeyValue == (IdOrder.IdTypeChange == 0 ? 1024 : IdOrder.IdTypeChange)).KeyValueIntCode == "Changed")
                                                                {
                                                                    dates.Add(order.OrderDate);
                                                                }
                                                                else
                                                                {
                                                                    Deletedates.Add(order.OrderDate);
                                                                }
                                                                sOrders.Add("/" + order.OrderDate.ToString(GlobalConstants.DATE_FORMAT));
                                                            }
                                                            ChangeType = IdOrder.IdTypeChange == 0 ? "" : KeyValuesSource.Find(x => x.IdKeyValue == IdOrder.IdTypeChange).Name;
                                                        }
                                                        temp.Add(String.Join("", sOrders));
                                                    }
                                                    SpecialityCorrection_note = String.Join(", ", temp);
                                                    if (dates.Any())
                                                    {
                                                        earliestDate = dates.OrderBy(a => a.Date).First();
                                                    }
                                                    if (Deletedates.Any())
                                                    {
                                                        oldestDate = Deletedates.OrderByDescending(a => a.Date).First();
                                                    }
                                                }
                                                vet_speciality temp_vet_Speciality = new vet_speciality()
                                                {
                                                    vet_speciality_name = Speciality.Name + (ChangeType == "" ? null : $"({ChangeType})"),
                                                    vet_speciality_id = Speciality.IdSpeciality,
                                                    vet_speciality_number = Int64.Parse(Speciality.Code),
                                                    vet_speciality_is_valid = KeyValuesSource.Find(x => x.IdKeyValue == Speciality.IdStatus).KeyValueIntCode == "Active" ? true : false,
                                                    vet_speciality_vqs = KeyValuesSource.Find(x => x.IdKeyValue == Speciality.IdVQS).Order,
                                                    vet_speciality_start_date = earliestDate == null ? null : earliestDate,
                                                    vet_speciality_end_date = oldestDate == null ? null : oldestDate,
                                                    vet_speciality_correction_notes = SpecialityCorrection_note

                                                };
                                                temp_vet_Profession.vet_specialities[i] = temp_vet_Speciality;
                                            }
                                        }
                                        temp_vet_Area.vet_professions[j] = temp_vet_Profession;
                                    }
                                }
                                temp_vet_Group.vet_areas[x] = temp_vet_Area;
                            }
                        }
                        vet_Groups[y] = temp_vet_Group;
                    }
                    var list = vet_Groups;

                    paramResponse.status = true;
                    paramResponse.message = "Успешно!!!!";
                    paramResponse.data = list;
                }
                else
                {
                    paramResponse.status = false;
                    paramResponse.message = "Грешно потребителко име или парола!";
                }
                return res = new getSPPOOListResponse(paramResponse);
            }
            catch (Exception ex)
            {
                paramResponse = new LoadSPPOOResponseType();
                paramResponse.status = false;
                paramResponse.message = ex.Message;
                return res = new getSPPOOListResponse(paramResponse);
            }
        }
    }
}
