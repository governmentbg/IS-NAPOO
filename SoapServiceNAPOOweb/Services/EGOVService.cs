using Data.Models.Data.SPPOO;
using Data.Models.Data.SqlView.WebIntegrationService;
using Data.Models.Data.Training;
using Data.Models.DB;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using SoapServiceNAPOOweb.Models.EGOV;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SoapServiceNAPOOweb.Services
{
    public class Data : IData
    {
        private readonly ApplicationDbContext context;

        public Data(ApplicationDbContext applicationDbContext)
        {
            this.context = applicationDbContext;
        }
        public StudentDocumentResponseType egovSearchStudentDocument(string identifier, string document_no)
        {
            StudentDocumentResponseType response = new StudentDocumentResponseType();

            //Свидетелство за професионална квалификация - 3-54
            //Свидетелство за валидиране на професионална квалификация - 3-54В
            //Дубликат на свидетелство за професионална квалификация(универсален образец),3-54a
            //Дубликат на свидетелство за валидиране на професионална квалификация - 3-54aB

            List<string> typeOfRequestedDocumentsNums = new List<string>() { "3-54", "3-54В", "3-54a", "3-54aB" };

            List<StudentDocument> listStudentDocument = new List<StudentDocument>();

            var listClientCourseDocument = (from courseClient in this.context.ClientCourses

                                            join course in this.context.Courses on courseClient.IdCourse equals course.IdCourse
                                            join candidateProvider in this.context.CandidateProviders on course.IdCandidateProvider equals candidateProvider.IdCandidate_Provider
                                            join program in this.context.Programs on course.IdProgram equals program.IdProgram
                                            join speciality in this.context.Specialities on program.IdSpeciality equals speciality.IdSpeciality
                                            join kvVQS in this.context.KeyValues on speciality.IdVQS equals kvVQS.IdKeyValue
                                            join profession in this.context.Professions on speciality.IdProfession equals profession.IdProfession

                                            join kvCourseType in this.context.KeyValues on course.IdTrainingCourseType equals kvCourseType.IdKeyValue into grkvCourseType
                                            from courseType in grkvCourseType.DefaultIfEmpty()

                                            join clientCourseDocument in this.context.ClientCourseDocuments on courseClient.IdClientCourse equals clientCourseDocument.IdClientCourse
                                            join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on clientCourseDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument

                                            join candidateLocation in this.context.Locations on candidateProvider.IdLocation equals candidateLocation.idLocation
                                            join candidateMunicipality in this.context.Municipalities on candidateLocation.idMunicipality equals candidateMunicipality.idMunicipality
                                            join candidateDistrict in this.context.Districts on candidateMunicipality.idDistrict equals candidateDistrict.idDistrict

                                            where courseClient.Indent == identifier.Trim() &&
                                                typeOfRequestedDocumentsNums.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber) 
                                                 //&&
                                                 //clientCourseDocument.DocumentRegNo != null &&
                                                 //clientCourseDocument.DocumentRegNo.Contains(document_no)

                                            select new StudentDocument()
                                            {
                                                client_id = courseClient.IdClientCourse,                                                                            //Уникален номер на обучаемия от информационната система на НАПОО
                                                vc_egn = courseClient.Indent,                                                                               // ЕГН, ЛНЧ или друг вид идентификатор на лицето
                                                first_name = courseClient.FirstName,                                                                        //Лично име
                                                second_name = courseClient.SecondName,                                                                      //Бащино име
                                                family_name = courseClient.FamilyName,                                                                      //Фамилия
                                                licence_number = int.Parse(candidateProvider.LicenceNumber),                                                //Идентификатор на документ на курсист в регистъра
                                                provider_owner = candidateProvider.ProviderOwner,                                                           //Наименование на център за професионално обучение
                                                city_name = $"обл. {candidateDistrict.DistrictName}, общ. {candidateMunicipality.MunicipalityName}, {candidateLocation.LocationName}",//обл. София-град, общ. Столична, гр. София           //Населено място на център за професионално обучение
                                                document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,                                      //Уникален идентификатор на вида на документа
                                                document_type_name = typeOfRequestedDocuments.DocTypeName,                                                  //Вид на документа
                                                course_type_id = courseType != null ? courseType.IdKeyValue : 0,                                            //Уникален идентификатор на вида на обучението
                                                course_type_name = courseType != null ? courseType.Name : string.Empty,                                     //Вид на обучението
                                                profession_name = $"{profession.Code} {profession.Name}",                                                   //Код и наименование на професията	
                                                speciality_name = $"{speciality.Code} {speciality.Name}",                                                   //Код и наименование на специалността
                                                speciality_vqs = kvVQS.Order,                                                                               //Придобита степен на професионална квалификация
                                                year_finished = clientCourseDocument.FinishedYear.HasValue ? clientCourseDocument.FinishedYear.Value : 0,   //Година на завършване на курсиста
                                                                                                                                                            //document_prn_ser = clientCourseDocument.DocumentPrnNo,Серия на документ взема се от DocumentPrnNo			//Серия на документа
                                                document_prn_no = clientCourseDocument.DocumentPrnNo,                                                       //Сериен номер на документа
                                                document_reg_no = clientCourseDocument.DocumentRegNo,                                                       //Регистрационен номер на документа
                                                document_issue_date = clientCourseDocument.DocumentDate.HasValue ? clientCourseDocument.DocumentDate : null //Дата на издаване на документа
                                            }).ToList();



            var listValidationClientDocument = (from validationClient in this.context.ValidationClients


                                                join candidateProvider in this.context.CandidateProviders on validationClient.IdCandidateProvider equals candidateProvider.IdCandidate_Provider
                                                join speciality in this.context.Specialities on validationClient.IdSpeciality equals speciality.IdSpeciality
                                                join kvVQS in this.context.KeyValues on speciality.IdVQS equals kvVQS.IdKeyValue
                                                join profession in this.context.Professions on speciality.IdProfession equals profession.IdProfession

                                                join kvCourseType in this.context.KeyValues on validationClient.IdCourseType equals kvCourseType.IdKeyValue into grkvCourseType
                                                from courseType in grkvCourseType.DefaultIfEmpty()

                                                join validationClientDocument in this.context.ValidationClientDocuments on validationClient.IdValidationClient equals validationClientDocument.IdValidationClient
                                                join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on validationClientDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument

                                                join candidateLocation in this.context.Locations on candidateProvider.IdLocation equals candidateLocation.idLocation
                                                join candidateMunicipality in this.context.Municipalities on candidateLocation.idMunicipality equals candidateMunicipality.idMunicipality
                                                join candidateDistrict in this.context.Districts on candidateMunicipality.idDistrict equals candidateDistrict.idDistrict

                                                where validationClient.Indent == identifier.Trim() &&
                                                      typeOfRequestedDocumentsNums.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber) 
                                                      //&&
                                                      //validationClientDocument.DocumentRegNo != null &&
                                                      //validationClientDocument.DocumentRegNo.Contains(document_no)

                                                select new StudentDocument()
                                                {
                                                    client_id = validationClient.IdValidationClient,                                                            //Уникален номер на обучаемия от информационната система на НАПОО
                                                    vc_egn = validationClient.Indent,                                                                           // ЕГН, ЛНЧ или друг вид идентификатор на лицето
                                                    first_name = validationClient.FirstName,                                                                    //Лично име
                                                    second_name = validationClient.SecondName,                                                                  //Бащино име
                                                    family_name = validationClient.FamilyName,                                                                  //Фамилия
                                                    licence_number = int.Parse(candidateProvider.LicenceNumber),                                                //Идентификатор на документ на курсист в регистъра
                                                    provider_owner = candidateProvider.ProviderOwner,                                                           //Наименование на център за професионално обучение
                                                    city_name = $"обл. {candidateDistrict.DistrictName}, общ. {candidateMunicipality.MunicipalityName}, {candidateLocation.LocationName}",//обл. София-град, общ. Столична, гр. София           //Населено място на център за професионално обучение
                                                    document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,                                      //Уникален идентификатор на вида на документа
                                                    document_type_name = typeOfRequestedDocuments.DocTypeName,                                                  //Вид на документа
                                                    course_type_id = courseType != null ? courseType.IdKeyValue : 0,                                            //Уникален идентификатор на вида на обучението
                                                    course_type_name = courseType != null ? courseType.Name : string.Empty,                                     //Вид на обучението
                                                    profession_name = $"{profession.Code} {profession.Name}",                                                   //Код и наименование на професията	
                                                    speciality_name = $"{speciality.Code} {speciality.Name}",                                                   //Код и наименование на специалността
                                                    speciality_vqs = kvVQS.Order,                                                                               //Придобита степен на професионална квалификация
                                                    year_finished = validationClientDocument.FinishedYear.HasValue ? validationClientDocument.FinishedYear.Value : 0,   //Година на завършване на курсиста
                                                                                                                                                                        //document_prn_ser = clientCourseDocument.DocumentPrnNo,Серия на документ взема се от DocumentPrnNo			//document_prn_ser = clientCourseDocument.DocumentPrnNo,Серия на документ взема се от DocumentPrnNo			//Серия на документа
                                                    document_prn_no = validationClientDocument.DocumentPrnNo,                                                   //Сериен номер на документа
                                                    document_reg_no = validationClientDocument.DocumentRegNo,                                                   //Регистрационен номер на документа
                                                    document_issue_date = validationClientDocument.DocumentDate.HasValue ? validationClientDocument.DocumentDate : null //Дата на издаване на документа
                                                }).ToList();



            listStudentDocument.AddRange(listClientCourseDocument);
            listStudentDocument.AddRange(listValidationClientDocument);

            Regex regex = new Regex("[^0-9]");

            //Премахват се всички символи с изключение на числата
            listStudentDocument = listStudentDocument.Where(x => regex.Replace(x.document_reg_no, "").Contains(regex.Replace(document_no, ""))).ToList();


            ///document_prn_ser = Серия на документ взема се от DocumentPrnNo
            foreach (var item in listStudentDocument.Where(x => !string.IsNullOrEmpty(x.document_prn_no)))
            {
                var docParts = item.document_prn_no.Split('/');
                if (docParts.Length > 0) item.document_prn_ser = docParts[0];
            }


            response.data = listStudentDocument;
            response.status = true;
            response.message = "Успешно!!!!";

            return response;
        }

        public DocumentsByStudentResponseType egovSearchStudentDocumentByStudent(string identifier)
        {
            DocumentsByStudentResponseType response = new DocumentsByStudentResponseType();

            //Свидетелство за професионална квалификация - 3-54
            //Свидетелство за валидиране на професионална квалификация - 3-54В
            //Дубликат на свидетелство за професионална квалификация(универсален образец),3-54a
            //Дубликат на свидетелство за валидиране на професионална квалификация - 3-54aB

            List<string> typeOfRequestedDocumentsNums = new List<string>() { "3-54", "3-54В", "3-54a", "3-54aB" };

            List<StudentDocument> listStudentDocument = new List<StudentDocument>();

            var listClientCourseDocument = (from courseClient in this.context.ClientCourses

                                            join course in this.context.Courses on courseClient.IdCourse equals course.IdCourse
                                            join candidateProvider in this.context.CandidateProviders on course.IdCandidateProvider equals candidateProvider.IdCandidate_Provider
                                            join program in this.context.Programs on course.IdProgram equals program.IdProgram
                                            join speciality in this.context.Specialities on program.IdSpeciality equals speciality.IdSpeciality
                                            join kvVQS in this.context.KeyValues on speciality.IdVQS equals kvVQS.IdKeyValue
                                            join profession in this.context.Professions on speciality.IdProfession equals profession.IdProfession

                                            join kvCourseType in this.context.KeyValues on course.IdTrainingCourseType equals kvCourseType.IdKeyValue into grkvCourseType
                                            from courseType in grkvCourseType.DefaultIfEmpty()

                                            join clientCourseDocument in this.context.ClientCourseDocuments on courseClient.IdClientCourse equals clientCourseDocument.IdClientCourse
                                            join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on clientCourseDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument

                                            join candidateLocation in this.context.Locations on candidateProvider.IdLocation equals candidateLocation.idLocation
                                            join candidateMunicipality in this.context.Municipalities on candidateLocation.idMunicipality equals candidateMunicipality.idMunicipality
                                            join candidateDistrict in this.context.Districts on candidateMunicipality.idDistrict equals candidateDistrict.idDistrict

                                            where courseClient.Indent == identifier.Trim() &&
                                                  typeOfRequestedDocumentsNums.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)

                                            select new StudentDocument()
                                            {
                                                client_id = courseClient.IdClientCourse,                                                                            //Уникален номер на обучаемия от информационната система на НАПОО
                                                vc_egn = courseClient.Indent,                                                                               // ЕГН, ЛНЧ или друг вид идентификатор на лицето
                                                first_name = courseClient.FirstName,                                                                        //Лично име
                                                second_name = courseClient.SecondName,                                                                      //Бащино име
                                                family_name = courseClient.FamilyName,                                                                      //Фамилия
                                                licence_number = int.Parse(candidateProvider.LicenceNumber),                                                //Идентификатор на документ на курсист в регистъра
                                                provider_owner = candidateProvider.ProviderOwner,                                                           //Наименование на център за професионално обучение
                                                city_name = $"обл. {candidateDistrict.DistrictName}, общ. {candidateMunicipality.MunicipalityName}, {candidateLocation.LocationName}",//обл. София-град, общ. Столична, гр. София           //Населено място на център за професионално обучение
                                                document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,                                      //Уникален идентификатор на вида на документа
                                                document_type_name = typeOfRequestedDocuments.DocTypeName,                                                  //Вид на документа
                                                course_type_id = courseType != null ? courseType.IdKeyValue : 0,                                            //Уникален идентификатор на вида на обучението
                                                course_type_name = courseType != null ? courseType.Name : string.Empty,                                     //Вид на обучението
                                                profession_name = $"{profession.Code} {profession.Name}",                                                   //Код и наименование на професията	
                                                speciality_name = $"{speciality.Code} {speciality.Name}",                                                   //Код и наименование на специалността
                                                speciality_vqs = kvVQS.Order,                                                                               //Придобита степен на професионална квалификация
                                                year_finished = clientCourseDocument.FinishedYear.HasValue ? clientCourseDocument.FinishedYear.Value : 0,   //Година на завършване на курсиста
                                                                                                                                                            //document_prn_ser = clientCourseDocument.DocumentPrnNo,Серия на документ взема се от DocumentPrnNo			//Серия на документа
                                                document_prn_no = clientCourseDocument.DocumentPrnNo,                                                       //Сериен номер на документа
                                                document_reg_no = clientCourseDocument.DocumentRegNo,                                                       //Регистрационен номер на документа
                                                document_issue_date = clientCourseDocument.DocumentDate.HasValue ? clientCourseDocument.DocumentDate : null //Дата на издаване на документа
                                            }).ToList();



            var listValidationClientDocument = (from validationClient in this.context.ValidationClients


                                                join candidateProvider in this.context.CandidateProviders on validationClient.IdCandidateProvider equals candidateProvider.IdCandidate_Provider
                                                join speciality in this.context.Specialities on validationClient.IdSpeciality equals speciality.IdSpeciality
                                                join kvVQS in this.context.KeyValues on speciality.IdVQS equals kvVQS.IdKeyValue
                                                join profession in this.context.Professions on speciality.IdProfession equals profession.IdProfession

                                                join kvCourseType in this.context.KeyValues on validationClient.IdCourseType equals kvCourseType.IdKeyValue into grkvCourseType
                                                from courseType in grkvCourseType.DefaultIfEmpty()

                                                join validationClientDocument in this.context.ValidationClientDocuments on validationClient.IdValidationClient equals validationClientDocument.IdValidationClient
                                                join typeOfRequestedDocuments in this.context.TypeOfRequestedDocuments on validationClientDocument.IdTypeOfRequestedDocument equals typeOfRequestedDocuments.IdTypeOfRequestedDocument

                                                join candidateLocation in this.context.Locations on candidateProvider.IdLocation equals candidateLocation.idLocation
                                                join candidateMunicipality in this.context.Municipalities on candidateLocation.idMunicipality equals candidateMunicipality.idMunicipality
                                                join candidateDistrict in this.context.Districts on candidateMunicipality.idDistrict equals candidateDistrict.idDistrict

                                                where validationClient.Indent == identifier.Trim() &&
                                                      typeOfRequestedDocumentsNums.Contains(typeOfRequestedDocuments.DocTypeOfficialNumber)

                                                select new StudentDocument()
                                                {
                                                    client_id = validationClient.IdValidationClient,                                                            //Уникален номер на обучаемия от информационната система на НАПОО
                                                    vc_egn = validationClient.Indent,                                                                           // ЕГН, ЛНЧ или друг вид идентификатор на лицето
                                                    first_name = validationClient.FirstName,                                                                    //Лично име
                                                    second_name = validationClient.SecondName,                                                                  //Бащино име
                                                    family_name = validationClient.FamilyName,                                                                  //Фамилия
                                                    licence_number = int.Parse(candidateProvider.LicenceNumber),                                                //Идентификатор на документ на курсист в регистъра
                                                    provider_owner = candidateProvider.ProviderOwner,                                                           //Наименование на център за професионално обучение
                                                    city_name = $"обл. {candidateDistrict.DistrictName}, общ. {candidateMunicipality.MunicipalityName}, {candidateLocation.LocationName}",//обл. София-град, общ. Столична, гр. София           //Населено място на център за професионално обучение                                                    
                                                    document_type_id = typeOfRequestedDocuments.IdTypeOfRequestedDocument,                                      //Уникален идентификатор на вида на документа
                                                    document_type_name = typeOfRequestedDocuments.DocTypeName,                                                  //Вид на документа
                                                    course_type_id = courseType != null ? courseType.IdKeyValue : 0,                                            //Уникален идентификатор на вида на обучението
                                                    course_type_name = courseType != null ? courseType.Name : string.Empty,                                     //Вид на обучението
                                                    profession_name = $"{profession.Code} {profession.Name}",                                                   //Код и наименование на професията	
                                                    speciality_name = $"{speciality.Code} {speciality.Name}",                                                   //Код и наименование на специалността
                                                    speciality_vqs = kvVQS.Order,                                                                               //Придобита степен на професионална квалификация
                                                    year_finished = validationClientDocument.FinishedYear.HasValue ? validationClientDocument.FinishedYear.Value : 0,   //Година на завършване на курсиста
                                                                                                                                                                        //document_prn_ser = clientCourseDocument.DocumentPrnNo,Серия на документ взема се от DocumentPrnNo			//Серия на документа
                                                    document_prn_no = validationClientDocument.DocumentPrnNo,                                                       //Сериен номер на документа
                                                    document_reg_no = validationClientDocument.DocumentRegNo,                                                       //Регистрационен номер на документа
                                                    document_issue_date = validationClientDocument.DocumentDate.HasValue ? validationClientDocument.DocumentDate : null //Дата на издаване на документа
                                                }).ToList();



            listStudentDocument.AddRange(listClientCourseDocument);
            listStudentDocument.AddRange(listValidationClientDocument);


            ///document_prn_ser = Серия на документ взема се от DocumentPrnNo
            foreach (var item in listStudentDocument.Where(x => !string.IsNullOrEmpty(x.document_prn_no)))
            {
                var docParts = item.document_prn_no.Split('/');
                if (docParts.Length > 0) item.document_prn_ser = docParts[0];
            }


            response.data = listStudentDocument;
            response.status = true;
            response.message = "Успешно!!!!";


            return response;
        }
    }

    public class StudentDocument
    {
        public int client_id { get; set; }

        public string vc_egn { get; set; }

        public string first_name { get; set; }

        public string second_name { get; set; }

        public string family_name { get; set; }

        public int licence_number { get; set; }

        public string provider_owner { get; set; }

        public string city_name { get; set; }

        public int document_type_id { get; set; }

        public string document_type_name { get; set; }

        public int course_type_id { get; set; }

        public string course_type_name { get; set; }

        public string profession_name { get; set; }

        public string speciality_name { get; set; }

        public int speciality_vqs { get; set; }

        public int year_finished { get; set; }

        public string document_prn_ser { get; set; }

        public string document_prn_no { get; set; }

        public string document_reg_no { get; set; }
        public System.DateTime? document_issue_date { get; set; }
    }
}
