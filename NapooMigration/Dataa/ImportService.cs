using Data.Models;
using Data.Models.Data.Archive;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.DOC;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Request;
using Data.Models.Data.SPPOO;
using Data.Models.Data.Training;
using Data.Models.DB;
using Data.Models.Migrations;
using EFCore.BulkExtensions;
using HarfBuzzSharp;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.Core.XML.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using NapooMigration.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Prng;
using RegiX.Class.AVTR.GetActualStateV2;
using Syncfusion.Compression.Zip;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using CandidateProviderPremisesChecking = Data.Models.Data.Candidate.CandidateProviderPremisesChecking;
using Expert = Data.Models.Data.ExternalExpertCommission.Expert;
using TrainingProgram = Data.Models.Data.Training.Program;

namespace NapooMigration.Data
{
    public class ImportService
    {

        private readonly napoo_jessieContext _jessieContextContext;
        private readonly ApplicationDbContext _ApplicationDbContext;
        private readonly KeyValue kvSPPOStatusActive;
        private readonly KeyValue kvSPPOStatusInactive;
        private readonly List<KeyValue> keyValues;
        private readonly List<KeyType> keyTypes;
        private readonly ApplicationUser modifyUser;
        private readonly Setting recourceFolderSetting;
        private readonly Setting linkSetting;
        private readonly Setting takeSetting;
        private DateTime startDate;
        private DateTime endDate;
        //NAPOO Locations
        private List<CodeEkatte> locations;
        public IJSRuntime JsRuntime { get; set; }

        ILogger<ImportService> logger;

        List<SPPOOOrder> orders;

        public ImportService(napoo_jessieContext jessieContextContext, ApplicationDbContext dbContext, ILogger<ImportService> logger)
        {

            _jessieContextContext = jessieContextContext;

            _ApplicationDbContext = dbContext;

            kvSPPOStatusActive = (from kv in _ApplicationDbContext.KeyValues
                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                  where kv.KeyValueIntCode == "Active" && kt.KeyTypeIntCode == "StatusSPPOO"
                                  select kv).First();

            kvSPPOStatusInactive = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kv.KeyValueIntCode == "Inactive" && kt.KeyTypeIntCode == "StatusSPPOO"
                                    select kv).First();

            locations = _jessieContextContext.CodeEkattes.AsNoTracking().Where(x => x.Id != 0).ToList();

            keyTypes = _ApplicationDbContext.KeyTypes.ToList();

            keyValues = _ApplicationDbContext.KeyValues.Include(x => x.KeyType).ToList();

            var setting = _ApplicationDbContext.Settings.Where(x => x.SettingIntCode.Equals("UserIDBindWithSystem")).First();

            modifyUser = _ApplicationDbContext.Users.Where(x => x.Id.Equals(setting.SettingValue)).First();

            recourceFolderSetting = _ApplicationDbContext.Settings.Where(x => x.SettingIntCode.Equals("ResourcesFolderName")).First();
            linkSetting = _ApplicationDbContext.Settings.Where(x => x.SettingIntCode.Equals("NapooOldISDocsLink")).First();
            takeSetting = _ApplicationDbContext.Settings.Where(x => x.SettingIntCode.Equals("TakeNumberForMigration")).First();

            orders = new List<SPPOOOrder>();
            this.logger = logger;

            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;
        }

        public IEnumerable<CodeVetArea> GetAccsMessage()
        {
            var accs_messages = _jessieContextContext.CodeVetAreas.Take(20).ToList();

            return accs_messages;


        }

        public IEnumerable<CodeVetGroup> GetCodeVetGroups()
        {
            var res = _jessieContextContext.CodeVetGroups.ToList();

            return res;


        }


        public IEnumerable<Setting> GetSettings()
        {
            var res = _ApplicationDbContext.Settings.ToList();

            return res;


        }

        public async void testGetDoc()
        {

            var list = new List<int>() { 4030844, 4030838, 4030842, 4030849, 4030837, 4030841, 4030843, 4030857, 4030852, 4030850, 4030840, 4030839, 4030867 };

            for (int i = 0; i < 10; i++)
            {
                Parallel.ForEach(list, entity =>
                {
                    SaveDocument(entity, $"\\test{i}");

                });
            }
            //SaveDocument(4030844, $"\\test");
            // string pattern = "[\\p{IsCyrillic} ]+$";

            // Regex regex = new Regex(pattern);


            //foreach(var member in toMatch)
            // {
            //     var text = member.Replace("\r","");

            //     var result = regex.Match(text);

            //     if(result.Success)
            //     {
            //         var firstName = result.Value;

            //     }
            //}

            //string fileUrl = "http://10.10.10.79/furia/Projects/napoo/index.new.php?application=AJAXDataProvider&invoke=CustomEventsManager.customReturnOidFile(4170312)";
            ////string fileUrl = "https://is.navet.government.bg/furia/Projects/napoo/index.new.php?application=AJAXDataProvider&invoke=CustomEventsManager.customReturnOidFile(4170312)";
            //string savePath = "C:\\Resources_NAPOO_COPY\\file.jpg";

            //using var httpClient = new HttpClient();

            //HttpResponseMessage response = await httpClient.GetAsync(fileUrl);

            //if (response.IsSuccessStatusCode)
            //{
            //    using Stream fileStream = await response.Content.ReadAsStreamAsync();


            //    using FileStream outputStream = File.Create(savePath);
            //    await fileStream.CopyToAsync(outputStream);

            //    this.logger.LogInformation("Успешен запис");
            //}
            //else
            //{
            //    this.logger.LogInformation("НЕуспешен запис");
            //}



            //var doc = this._jessieContextContext.TbProviderUploadedDocs.Where(x => x.IntProviderId == 2929).First();

            //using (var fs = new FileStream($"{this.recourceFolderSetting.SettingValue}\\test.pdf", FileMode.Create, FileAccess.Write))
            //{
            //    fs.Write(doc.BinFile, 0, doc.BinFile.Length);
            //}
        }

        public void Spravka()
        {
            logger.LogInformation("Започната миграция на всички таблици без документи");

            //Tables before migration
            ImportProviderDocuments(null, false);
            ImportCandidateProviderDocuments(null, false);
            ImportCurriculumModification(null, false);
            ImportCandidateCurriculumModification(null, false);
            ImportProviderLicenceChange();
            ImportProviderProcedureDocuments(null, false);
            //Trainer
            ImportProviderTrainer();
            ImportCandidateProviderTrainer();
            ImportProviderTrainerQualifications();
            ImportCandidateProviderTrainerQualifications();
            ImportProviderTrainerProfiles();
            ImportCandidateProviderTrainerProfiles();
            ImportProviderTrainerDocument(null, false);
            ImportCandidateProviderTrainerDocument(null, false);

            //Premises
            ImportProviderPremisies();
            ImportCandidatePremisies();
            ImportPremisiesRooms();
            ImportCandidatePremisiesRooms();
            ImportPremisesSpecialities();
            ImportCandidatePremisesSpecialities();

            //Request
            ImportRequestProviderRequestDocument(null, false);
            ImportRequestRequestDocumentStatus();
            ImportRequestRequestDocumentType();
            ImportRequestReport(null, false);
            ImportRequestDocumentManagement(null, false);
            ImportRequestDocumentSerialNumber();

            //Training
            ImportTrainingProgram();
            ImportTrainingCourse();
            ImportTrainingClients();
            ImportTrainingClientCourses();
            ImportClientRequiredDocument(null, false);
            ImportTrainingClientCourseDocument(null, false);
            ImportTrainingClientCourseDocumentStatus();
            ImportTrainingCurriculumTheory();
            ImportTrainingCurriculumPractice();
            ImportTrainingCourseSubject();
            ImportCourseCommisionMembers();
            ImportTrainerCourse();
            ImportTrainingPremisesCourses();
            ImportValidationCompetencies();
            ImportCourseCandidateCurriculumModification();

            //Arch
            ImportArchProvider();
            ImportArchPremises();
            ImportArchPremisesSpecialities();
            ImportArchTrainer();
            ImportArchProviderTrainerQualifications();
            ImportArchTrainingCourse();
            ImportArchTrainingValidation();
            ImportAnnualInfo(null, false);
            ImportSelfAssessmentReport(null, false);

            logger.LogInformation("Приключена миграция на всички таблици без документи");
        }

        public void ImportAllCandidateProvider(string OldId)
        {

            logger.LogInformation("Започната миграция за едно ЦПО/ЦИПО");

            var id = Int32.Parse(OldId);

            //Tables before migration
            //ImportSPPOO();
            //ImportRequestNapooRequestDoc(false);
            //ImportRequestDocumentSeries();
            //ImportRequestTypeOfRequestedDocuments();
            //ImportSppooFrameworkProgram();

            //Candidates
            //ImportProcedure(id);
            //ImportProviders(id);
            //ImportCandidateProviders(id);

            ImportProviderDocuments(id);
            ImportCandidateProviderDocuments(id);
            ImportCurriculumModification(id);
            ImportCandidateCurriculumModification(id);
            ImportProviderLicenceChange(id);

            //Trainer
            ImportProviderTrainer(id);
            ImportCandidateProviderTrainer(id);
            ImportProviderTrainerQualifications(id);
            ImportCandidateProviderTrainerQualifications(id);
            ImportProviderTrainerProfiles(id);
            ImportCandidateProviderTrainerProfiles(id);
            ImportProviderTrainerDocument(id);
            ImportCandidateProviderTrainerDocument(id);

            //Premises
            ImportProviderPremisies(id);
            ImportCandidatePremisies(id);
            ImportPremisiesRooms(id);
            ImportCandidatePremisiesRooms(id);
            ImportPremisesSpecialities(id);
            ImportCandidatePremisesSpecialities(id);

            //Request
            ImportRequestProviderRequestDocument(id);
            ImportRequestRequestDocumentStatus(id);
            ImportRequestRequestDocumentType(id);
            ImportRequestReport(id);
            ImportRequestDocumentManagement(id);
            ImportRequestDocumentSerialNumber(id);
            ImportRequestProviderDocumentOffer(id);

            //Training
            ImportTrainingProgram(id);
            ImportTrainingCourse(id);
            ImportTrainingClients(id);
            ImportTrainingClientCourses(id);
            ImportClientRequiredDocument(id);
            ImportTrainingClientCourseDocument(id);
            ImportTrainingClientCourseDocumentStatus(id);
            ImportTrainingCurriculumTheory(id);
            ImportTrainingCurriculumPractice(id);
            ImportCourseCommisionMembers(id);
            ImportTrainerCourse(id);
            ImportTrainingPremisesCourses(id);
            ImportValidationCompetencies(id);

            //Arch
            ImportArchProvider(id);
            ImportArchPremises(id);
            ImportArchPremisesSpecialities(id);
            ImportArchTrainer(id);
            ImportArchProviderTrainerQualifications(id);
            ImportArchTrainingCourse(id);
            ImportArchTrainingValidation(id);
            ImportAnnualInfo(id);
            ImportSelfAssessmentReport(id);
            logger.LogInformation("Приключи миграция за едно ЦПО/ЦИПО");
        }
        public void ImportAllCandidateProviderWithoutDocuents(string OldId)
        {

            logger.LogInformation("Започната миграция за едно ЦПО/ЦИПО");

            var id = Int32.Parse(OldId);

            //Tables before migration
            //ImportSPPOO();
            //ImportRequestNapooRequestDoc(false);
            //ImportRequestDocumentSeries();
            //ImportRequestTypeOfRequestedDocuments();
            //ImportSppooFrameworkProgram();

            //Candidates
            //ImportProcedure(id);
            //ImportProviders(id);
            //ImportCandidateProviders(id);

            ImportProviderDocuments(id, false);
            ImportCandidateProviderDocuments(id, false);
            ImportCurriculumModification(id, false);
            ImportCandidateCurriculumModification(id, false);
            ImportProviderLicenceChange(id);
            ImportProviderProcedureDocuments(id, false);
            //Trainer
            ImportProviderTrainer(id);
            ImportCandidateProviderTrainer(id);
            ImportProviderTrainerQualifications(id);
            ImportCandidateProviderTrainerQualifications(id);
            ImportProviderTrainerProfiles(id);
            ImportCandidateProviderTrainerProfiles(id);
            ImportProviderTrainerDocument(id, false);
            ImportCandidateProviderTrainerDocument(id, false);

            //Premises
            ImportProviderPremisies(id);
            ImportCandidatePremisies(id);
            ImportPremisiesRooms(id);
            ImportCandidatePremisiesRooms(id);
            ImportPremisesSpecialities(id);
            ImportCandidatePremisesSpecialities(id);

            //Request
            ImportRequestProviderRequestDocument(id, false);
            ImportRequestRequestDocumentStatus(id);
            ImportRequestRequestDocumentType(id);
            ImportRequestReport(id, false);
            ImportRequestDocumentManagement(id, false);
            ImportRequestDocumentSerialNumber(id);

            //Training
            ImportTrainingProgram(id);
            ImportTrainingCourse(id);
            ImportTrainingClients(id);
            ImportTrainingClientCourses(id);
            ImportClientRequiredDocument(id, false);
            ImportTrainingClientCourseDocument(id, false);
            ImportTrainingClientCourseDocumentStatus(id);
            ImportTrainingCurriculumTheory(id);
            ImportTrainingCurriculumPractice(id);
            ImportTrainingCourseSubject(id);
            ImportCourseCommisionMembers(id);
            ImportTrainerCourse(id);
            ImportTrainingPremisesCourses(id);
            ImportValidationCompetencies(id);
            ImportCourseCandidateCurriculumModification(id);

            //Arch
            ImportArchProvider(id);
            ImportArchPremises(id);
            ImportArchPremisesSpecialities(id);
            ImportArchTrainer(id);
            ImportArchProviderTrainerQualifications(id);
            ImportArchTrainingCourse(id);
            ImportArchTrainingValidation(id);
            ImportAnnualInfo(id);
            ImportSelfAssessmentReport(id);
            logger.LogInformation("Приключи миграция за едно ЦПО/ЦИПО");
        }
        public void ImportAllCandidateProviderDocuentsOnly(string OldId)
        {

            logger.LogInformation("Започната миграция на документи за едно ЦПО/ЦИПО");

            var id = Int32.Parse(OldId);

            CandidateCurriculumModificationMigrateDomuments(id);
            CandidateProviderDocumentMigrateDocuments(id);
            CandidateProviderTrainerDocumentMigrateDocuments(id);
            CandidateProviderProcedureDocumentMigrateDocuments(id);
            RequestDocumentManagementMigrateDocuments(id);
            RequestReportMigrateDocuments(id);
            AnnualInfoMigrateDocuments(id);
            SelfAssessmentMigrateDocuments(id);
            ProviderRequestDocumentMigrateDocuments(id);
            CourseProtocolMigrateDocuments(id);
            ValidationProtocolMigrateDocuments(id);
            ValidationRequiredDocumentsMigrateDocuments(id);
            ClientCourseRequiredDocumentsMigrateDocuments(id);
            ClientCourseDocumentsMigrateDocuments(id);
            ValidationClientDocumentsMigrateDocuments(id);


            logger.LogInformation("Приключи миграция на документи за едно ЦПО/ЦИПО");
        }

        public void ImportDocuentsOnly()
        {

            logger.LogInformation("Започната миграция на документи за едно ЦПО/ЦИПО");

            //CandidateCurriculumModificationMigrateDomuments();
            //CandidateProviderDocumentMigrateDocuments();
            //CandidateProviderTrainerDocumentMigrateDocuments();
            //CandidateProviderProcedureDocumentMigrateDocuments();

            //RequestDocumentManagementMigrateDocuments();
            RequestReportMigrateDocuments();
            AnnualInfoMigrateDocuments();
            SelfAssessmentMigrateDocuments();

            //ProviderRequestDocumentMigrateDocuments();
            //CourseProtocolMigrateDocuments();
            //ValidationProtocolMigrateDocuments();
            //ValidationRequiredDocumentsMigrateDocuments();
            //ClientCourseRequiredDocumentsMigrateDocuments();

            ClientCourseDocumentsMigrateDocuments();
            ValidationClientDocumentsMigrateDocuments();


            logger.LogInformation("Приключи миграция на документи за едно ЦПО/ЦИПО");
        }
        //Роял - 2006 ЕООД - 1664
        //Телерик Академия ООД - 3499
        //"СофтУни ЦПО" ЕООД - 2929
        //„Рувекс” АД - 894
        //"Петър Йовчев и синове" ООД - 209
        //"АВС-Е" ЕООД - 138
        //"Асарел-Медет" АД - 262
        //„Вега Василеви” ЕООД, гр.Ловеч - 1197
        //Далия 22 - 3611
        //сдружение "ХИПОКРАТ" - 1112
        //ЦИПО Никанор ООД - 1777
        //ЦПО "НИКАНОР" ООД - 96
        //Зелени идеи към Лернен ЕООД - 3832 
        //ЦПО към „КВАЛИФИКАЦИЯ“ ЕООД - 1506
        //"Сайбър Сървисис" ЕООД - 3665
        //"АХАД - БИЗНЕС УНИВЕРСАЛ" ЕООД - 227
        //СТРОНГ БАЙ САЙЪНС ООД - 3559
        //ЦИПО към АИКБ Консулт ЕООД - 2095
        //Инспайър Кепитъл ООД - 3556
        //Устойчив свят ЕООД - 3839
        public void ImportCPOs()
        {

            logger.LogInformation("Започна миграция за всички ЦПО/ЦИПО");

            var ids = new List<int>()
            {
                 3556, 1664, 2929, 894, 209, 138, 262, 1197, 3611, 1112, 1777, 96, 3665, 227, 3559, 3832, 1506, 2095
            };

            foreach (var id in ids)
            {
                logger.LogInformation("Започната миграция за едно ЦПО/ЦИПО Id: " + id);

                ImportAllCandidateProvider(id.ToString());

                ////Tables before migration
                //ImportProviderDocuments(id);
                //ImportCandidateProviderDocuments(id);
                //ImportCurriculumModification(id);
                //ImportCandidateCurriculumModification(id);
                //ImportProviderLicenceChange(id);

                ////Trainer
                //ImportProviderTrainer(id);
                //ImportCandidateProviderTrainer(id);
                //ImportProviderTrainerQualifications(id);
                //ImportCandidateProviderTrainerQualifications(id);
                //ImportProviderTrainerProfiles(id);
                //ImportCandidateProviderTrainerProfiles(id);
                //ImportProviderTrainerDocument(id);
                //ImportCandidateProviderTrainerDocument(id);

                ////Premises
                //ImportProviderPremisies(id);
                //ImportCandidatePremisies(id);
                //ImportPremisiesRooms(id);
                //ImportCandidatePremisiesRooms(id);
                //ImportPremisesSpecialities(id);
                //ImportCandidatePremisesSpecialities(id);

                ////Request
                //ImportRequestProviderRequestDocument(id);
                //ImportRequestRequestDocumentStatus(id);
                //ImportRequestRequestDocumentType(id);
                //ImportRequestReport(id);
                //ImportRequestDocumentManagement(id);
                //ImportRequestDocumentSerialNumber(id);

                ////Training
                //ImportTrainingProgram(id);
                //ImportTrainingCourse(id);
                //ImportTrainingClients(id);
                //ImportTrainingClientCourses(id);
                //ImportClientRequiredDocument(id);
                //ImportTrainingClientCourseDocument(id);
                //ImportTrainingClientCourseDocumentStatus(id);
                //ImportTrainingCurriculumTheory(id);
                //ImportTrainingCurriculumPractice(id);
                //ImportCourseCommisionMembers(id);
                //ImportTrainerCourse(id);
                //ImportTrainingPremisesCourses(id);
                //ImportValidationCompetencies(id);
                //ImportCourseCandidateCurriculumModification(id);

                ////Arch
                //ImportArchProvider(id);
                //ImportArchPremises(id);
                //ImportArchPremisesSpecialities(id);
                //ImportArchTrainer(id);
                //ImportArchProviderTrainerQualifications(id);
                //ImportArchTrainingCourse(id);
                //ImportArchTrainingValidation(id);
                //ImportAnnualInfo(id);
                //ImportSelfAssessmentReport(id);


                logger.LogInformation("Приключи миграция за едно ЦПО/ЦИПО Id: " + id);
            }
            logger.LogInformation("Приключи миграция за всички ЦПО/ЦИПО");

        }

        #region KeyValue
        public void ImportNationality()
        {
            //code_nationality -> keyValue
            var code_nationality = _jessieContextContext.CodeNationalities.ToList();
            var keyType = keyTypes.Where(x => x.KeyTypeIntCode.Equals("Nationality")).First();
            int i = 0;

            foreach (var nationality in code_nationality)
            {
                KeyValue keyValue = new KeyValue()
                {
                    KeyType = keyType,
                    Name = nationality.VcCountryName,
                    DefaultValue1 = (bool)nationality.BoolIsEuMember ? "EU" : null,
                    KeyValueIntCode = nationality.VcCountryName,
                    Order = ++i,
                    CreationDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    IdModifyUser = modifyUser.IdUser,
                    IdCreateUser = modifyUser.IdUser

                };
                _ApplicationDbContext.Add(keyValue);
            }
            _ApplicationDbContext.SaveChanges();
        }
        public void ImportCandidateProviderDocumentType()
        {
            var code_documents_management = this._jessieContextContext.CodeDocumentsManagements.ToList();

            var keyType = this._ApplicationDbContext.KeyTypes.Where(x => x.KeyTypeIntCode.Equals("CandidateProviderDocumentType")).First();

            ConcurrentBag<KeyValue> keyValues = new ConcurrentBag<KeyValue>();

            Parallel.ForEach(code_documents_management, docs =>
            {
                KeyValue kv = new KeyValue();

                kv.Name = docs.VcDocumentsManagementName;

                kv.Order = (int)docs.OrderId;

                if (docs.VcDocumentsManagementName.Length <= 255)
                    kv.KeyValueIntCode = docs.VcDocumentsManagementName;
                else
                    kv.KeyValueIntCode = docs.VcDocumentsManagementName.Substring(0, 255);

                kv.DefaultValue2 = docs.Id.ToString();

                kv.IdKeyType = keyType.IdKeyType;

                keyValues.Add(kv);
            });

            this._ApplicationDbContext.BulkInsert(keyValues.ToList());
        }
        #endregion

        #region Procedure
        public void ImportProcedure(int? OldId = null)
        {
            LogStrartInformation("ImportProcedure");

            //OldId = 1;

            List<StartedProcedure> startedProcedures = new List<StartedProcedure>();

            List<CandidateProvider> candidateProviders = new List<CandidateProvider>();

            var tb_started_procedures = new List<TbStartedProcedure>();

            if (OldId is null)
                tb_started_procedures = _jessieContextContext.TbStartedProcedures.OrderBy(x => x.Id).ToList();
            else
                tb_started_procedures = _jessieContextContext.TbStartedProcedures.Where(x => x.ProviderId == OldId).OrderBy(x => x.Id).ToList();

            var tb_started_procedures_progress = _jessieContextContext.TbStartedProcedureProgresses.ToList();

            var tb_arch_procedure_provider = _jessieContextContext.TbArchProcedureProviders.ToList();

            var tb_providers = _jessieContextContext.TbProviders.ToList();

            var tb_candidate_providers = _jessieContextContext.TbCandidateProviders.ToList();

            var dbLocations = _ApplicationDbContext.Locations.ToList();

            var tb_procedure_documents = _jessieContextContext.TbProcedureDocuments.Where(x => (x.StageDocumentId == 100 || x.StageDocumentId == 200 || x.StageDocumentId == 300 || x.StageDocumentId == 500) && x.IsValid == true && !string.IsNullOrEmpty(x.Filename)).ToList();

            var ApplicationStatuses = (from kv in _ApplicationDbContext.KeyValues
                                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                       where kt.KeyTypeIntCode == "ApplicationStatus" && !string.IsNullOrEmpty(kv.DefaultValue2) && !kv.KeyValueIntCode.Equals("RequestedByCPOOrCIPO")
                                       select kv).To<KeyValueVM>().ToList();

            var ProviderStatuses = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "ProviderStatus"
                                    select kv).To<KeyValueVM>().ToList();

            var LicenseStatuses = (from kv in _ApplicationDbContext.KeyValues
                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                   where kt.KeyTypeIntCode == "LicenseStatus" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                   select kv).To<KeyValueVM>().ToList();

            var ownership = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "ProviderOwnership"
                             select kv).To<KeyValueVM>().ToList();

            var registeredAt = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "ProviderRegisteredAt"
                                select kv).To<KeyValueVM>().ToList();

            var LicensingTypes = (from kv in _ApplicationDbContext.KeyValues
                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                  where kt.KeyTypeIntCode == "LicensingType"
                                  select kv).To<KeyValueVM>().ToList();

            var TypeApplications = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "TypeApplication"
                                    select kv).To<KeyValueVM>().ToList();

            var ReceiveLicenseTypes = (from kv in _ApplicationDbContext.KeyValues
                                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                       where kt.KeyTypeIntCode == "ReceiveLicenseType"
                                       select kv).To<KeyValueVM>().ToList();

            var ApplicationFilingTypes = (from kv in _ApplicationDbContext.KeyValues
                                          join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                          where kt.KeyTypeIntCode == "ApplicationFilingType" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                          select kv).To<KeyValueVM>().ToList();

            var DocumentsFilledSubmittedToNAPOO = new List<int>() { 1, 6 };

            var RejectedApplicationLicensingNewCenter = new List<int>() { 3, 12 };

            var DocumentsFilledSubmittedToNAPOOKV = (from kv in _ApplicationDbContext.KeyValues
                                                     join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                     where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode.Equals("RequestedByCPOOrCIPO")
                                                     select kv).To<KeyValueVM>().First();

            var RejectedApplicationLicensingNewCenterKV = (from kv in _ApplicationDbContext.KeyValues
                                                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                           where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode.Equals("RejectedApplicationLicensingNewCenter")
                                                           select kv).To<KeyValueVM>().First();

            var ProcedureTerminatedByCenterKV = (from kv in _ApplicationDbContext.KeyValues
                                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                 where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode.Equals("ProcedureTerminatedByCenter")
                                                 select kv).To<KeyValueVM>().First();


            //Parallel.ForEach(tb_started_procedures, procedure =>

            foreach (var procedure in tb_started_procedures)
            {
                try
                {
                    StartedProcedureVM startedProcedure = new StartedProcedureVM();

                    startedProcedure.TS = procedure.Ts;

                    startedProcedure.NapooReportDeadline = procedure.DtNapooReportDeadline;

                    startedProcedure.MeetingDate = procedure.DtMeetingDate;

                    startedProcedure.MeetingHour = procedure.VcMeetingHour;

                    startedProcedure.LicenseNumber = procedure.VcLicenseNumber;

                    startedProcedure.LicenseDate = procedure.DtLicenseDate;

                    startedProcedure.ExpertReportDeadline = procedure.DtExpertReportDeadline;

                    startedProcedure.IdCreateUser = modifyUser.IdUser;

                    startedProcedure.IdModifyUser = modifyUser.IdUser;

                    startedProcedure.ModifyDate = DateTime.Now;

                    startedProcedure.OldId = procedure.Id;

                    startedProcedure.CreationDate = DateTime.Now;

                    var archProcedure = tb_arch_procedure_provider.Where(x => x.Id == procedure.ProviderId && x.IntStartedProcedureId == procedure.Id).FirstOrDefault();
                    // проверява дали има запис на конкретната процедура в архивна таблицата 
                    if (archProcedure is not null)
                    {
                        CandidateProviderVM candidateProvider = new CandidateProviderVM();

                        candidateProvider.OldId = archProcedure.Id;

                        var provStatus = ProviderStatuses
                            .Where(x => int.Parse(x.DefaultValue2) == archProcedure.IntProviderStatusId)
                            .First();

                        candidateProvider.IdProviderStatus = provStatus.IdKeyValue;

                        candidateProvider.IdTypeLicense = provStatus.KeyValueIntCode.Contains("CPO") ? LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCPO")).First().IdKeyValue : LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCIPO")).First().IdKeyValue;

                        //candidateProvider.IdLicenceStatus = LicenseStatuses
                        //    .Where(x => int.Parse(x.DefaultValue2) == archProcedure.IntLicenceStatusId)
                        //    .First()
                        //    .IdKeyValue;

                        candidateProvider.LicenceNumber = archProcedure.IntLicenceNumber.ToString();

                        if (archProcedure.VcProviderOwner != null)
                            candidateProvider.ProviderOwner = archProcedure.VcProviderOwner;

                        if (archProcedure.IntProviderBulstat != null)
                            candidateProvider.PoviderBulstat = archProcedure.IntProviderBulstat;
                        else
                            candidateProvider.PoviderBulstat = "";

                        if (archProcedure.IntEkatteId != null && archProcedure.IntEkatteId != 0)

                            candidateProvider.IdLocation = dbLocations
                                .Where(x => x.LocationCode.Equals(locations
                                .Where(m => m.Id == archProcedure.IntEkatteId)
                                .First().VcTextCode))
                                .First()
                                .To<LocationVM>().idLocation;



                        if (archProcedure.VcZipCode != null)
                            candidateProvider.ZipCode = archProcedure.VcZipCode;
                        else
                            candidateProvider.ZipCode = "";

                        if (archProcedure.VcProviderAddress != null)
                            candidateProvider.ProviderAddress = archProcedure.VcProviderAddress.Replace("\\", "");
                        else
                            candidateProvider.ProviderAddress = "";

                        if (string.IsNullOrEmpty(archProcedure.VcProviderPhone1))
                            candidateProvider.ProviderPhone = "";
                        else if (!string.IsNullOrEmpty(archProcedure.VcProviderPhone1) && !string.IsNullOrEmpty(archProcedure.VcProviderPhone2))
                            candidateProvider.ProviderPhone = $"{archProcedure.VcProviderPhone1}; {archProcedure.VcProviderPhone2}";
                        else
                            candidateProvider.ProviderPhone = archProcedure.VcProviderPhone1;


                        candidateProvider.ProviderFax = archProcedure.VcProviderFax;

                        candidateProvider.ProviderWeb = archProcedure.VcProviderWeb;

                        candidateProvider.ProviderEmail = archProcedure.VcProviderEmail;

                        if (!string.IsNullOrEmpty(archProcedure.VcProviderName))
                            candidateProvider.ProviderName = archProcedure.VcProviderName;
                        else
                            candidateProvider.ProviderName = "";

                        candidateProvider.PersonNameCorrespondence = archProcedure.VcProviderContactPers;

                        if (archProcedure.IntEkatteId != null)
                            candidateProvider.IdLocation = dbLocations
                                 .Where(x => x.LocationCode.Equals(locations
                                 .Where(m => m.Id == archProcedure.IntEkatteId)
                                 .First().VcTextCode))
                                 .First()
                                 .To<LocationVM>().idLocation;

                        candidateProvider.ZipCodeCorrespondence = archProcedure.VcProviderContactPersZipcode;

                        if (archProcedure.VcProviderContactPersAddress is not null)
                            candidateProvider.ProviderAddressCorrespondence = archProcedure.VcProviderContactPersAddress.Replace("\\", "");

                        candidateProvider.RejectionReason = procedure.VcNegativeReasons;

                        if (archProcedure.IntProviderContactPersEkatteId != null)
                            candidateProvider.IdLocationCorrespondence = dbLocations
                                  .Where(x => x.LocationCode.Equals(locations
                                 .Where(m => m.Id == archProcedure.IntProviderContactPersEkatteId)
                                 .First().VcTextCode))
                                 .First()
                                 .To<LocationVM>().idLocation;

                        if (string.IsNullOrEmpty(archProcedure.VcProviderContactPersPhone1))
                            candidateProvider.ProviderPhoneCorrespondence = "";
                        else if (!string.IsNullOrEmpty(archProcedure.VcProviderContactPersPhone1) && !string.IsNullOrEmpty(archProcedure.VcProviderContactPersPhone2))
                            candidateProvider.ProviderPhoneCorrespondence = $"{archProcedure.VcProviderContactPersPhone1}; {archProcedure.VcProviderContactPersPhone2}";
                        else
                            candidateProvider.ProviderPhoneCorrespondence = archProcedure.VcProviderContactPersPhone1;


                        candidateProvider.ProviderFaxCorrespondence = archProcedure.VcProviderContactPersFax;

                        candidateProvider.ProviderEmailCorrespondence = archProcedure.VcProviderContactPersEmail;

                        if (archProcedure.IntProviderOwnershipId != null)
                            candidateProvider.IdProviderOwnership = ownership
                                .Where(x => int.Parse(x.DefaultValue2) == archProcedure.IntProviderOwnershipId)
                                .First()
                                .IdKeyValue;

                        if (archProcedure.VcProviderManager != null)
                            candidateProvider.ManagerName = archProcedure.VcProviderManager;



                        candidateProvider.LicenceDate = archProcedure.DtLicenceData;

                        if (archProcedure.IntProviderRegistrationId != null)
                            candidateProvider.IdProviderRegistration = registeredAt
                                .Where(x => int.Parse(x.DefaultValue2) == archProcedure.IntProviderRegistrationId)
                                .First()
                                .IdKeyValue;


                        candidateProvider.IdModifyUser = modifyUser.IdUser;
                        candidateProvider.IdCreateUser = modifyUser.IdUser;
                        candidateProvider.ModifyDate = DateTime.Now;
                        candidateProvider.CreationDate = DateTime.Now;

                        var dbCandidate = candidateProvider.To<CandidateProvider>();

                        dbCandidate.MigrationNote = "ap";

                        var candidateExist = candidateProviders.Where(x => x.OldId == archProcedure.Id && x.IsActive).FirstOrDefault();

                        if (candidateExist is null)
                        {
                            dbCandidate.IsActive = true;
                            dbCandidate.IdCandidateProviderActive = null;
                        }
                        else
                        {
                            dbCandidate.IsActive = false;
                            dbCandidate.CandidateProviderActive = candidateExist;
                        }

                        dbCandidate.IdTypeApplication = TypeApplications
                            .Where(x => Int32.Parse(x.DefaultValue2) == archProcedure.ProcedureId)
                            .First()
                            .IdKeyValue;

                        //Да решим какво ще правим с дължината на полето
                        //dbCandidate.AdditionalInfo = archProcedure.VcProviderProfile;

                        var appStatus = new KeyValueVM();

                        if (DocumentsFilledSubmittedToNAPOO.Contains((int)archProcedure.StepId))
                        {
                            appStatus = DocumentsFilledSubmittedToNAPOOKV;
                        }
                        else if (RejectedApplicationLicensingNewCenter.Contains((int)archProcedure.StepId))
                        {
                            appStatus = RejectedApplicationLicensingNewCenterKV;
                        }
                        else
                        {
                            appStatus = ApplicationStatuses.Where(x => Int32.Parse(x.DefaultValue2) == archProcedure.StepId).First();
                        }


                        dbCandidate.IdApplicationStatus = appStatus.IdKeyValue;

                        //dbCandidate.ApplicationNumber = archProcedure.VcFilingSystemNumber;

                        //dbCandidate.ApplicationDate = archProcedure.DtFilingSystemDate;

                        var doc = tb_procedure_documents.Where(x => x.StartedProcedureId == procedure.Id).FirstOrDefault();
                        if (doc is not null)
                        {
                            dbCandidate.ApplicationNumber = doc.DsOfficialNo;
                            dbCandidate.ApplicationDate = doc.DsOfficialDate;
                        }


                        if (archProcedure.IntReceiveTypeId != null)
                            dbCandidate.IdReceiveLicense = ReceiveLicenseTypes
                                .Where(x => Int32.Parse(x.DefaultValue2) == archProcedure.IntReceiveTypeId)
                                .First().IdKeyValue;

                        dbCandidate.StartedProcedure = startedProcedure.To<StartedProcedure>();

                        dbCandidate.ProviderOwnerEN = ConvertCyrToLatin(dbCandidate.ProviderOwner);
                        if (dbCandidate.ProviderName is not null)
                            dbCandidate.ProviderNameEN = ConvertCyrToLatin(dbCandidate.ProviderName);
                        if (dbCandidate.ProviderAddressCorrespondence is not null)
                            dbCandidate.ProviderAddressCorrespondenceEN = ConvertCyrToLatin(dbCandidate.ProviderAddressCorrespondence);
                        if (dbCandidate.PersonNameCorrespondence is not null)
                            dbCandidate.PersonNameCorrespondenceEN = ConvertCyrToLatin(dbCandidate.PersonNameCorrespondence);
                        if (dbCandidate.ProviderAddress is not null)
                            dbCandidate.ProviderAddressEN = ConvertCyrToLatin(dbCandidate.ProviderAddress);

                        candidateProviders.Add(dbCandidate);

                        _ApplicationDbContext.Add(dbCandidate);
                        _ApplicationDbContext.SaveChanges();
                    }
                    else
                    {
                        //Ако няма запис в архивна таблица проверяваме дали има запис в tb_candidate_providers
                        var isInCandidateProvider = tb_candidate_providers.Where(x => x.IntStartedProcedures == procedure.Id).FirstOrDefault();
                        if (isInCandidateProvider is null)
                        {
                            //Ако няма попълваме от tb_started_procedure
                            var candidateExist = candidateProviders.Where(x => x.OldId == procedure.ProviderId && x.IsActive).FirstOrDefault();

                            var dbCandidate = new CandidateProvider();

                            dbCandidate.IdTypeApplication = TypeApplications
                                .Where(x => Int32.Parse(x.DefaultValue2) == procedure.IntCandidateTypeId)
                                .First()
                                .IdKeyValue;

                            dbCandidate.ProviderOwner = procedure.VcProviderOwner;

                            dbCandidate.PoviderBulstat = procedure.IntProviderBulstat;

                            dbCandidate.ProviderPhone = procedure.VcProviderPhone1;

                            dbCandidate.ProviderPhoneCorrespondence = procedure.VcProviderPhone2;

                            dbCandidate.ProviderEmail = procedure.VcProviderEmail;

                            dbCandidate.ProviderFax = procedure.VcProviderFax;

                            dbCandidate.ProviderWeb = procedure.VcProviderWeb;

                            dbCandidate.ManagerName = procedure.VcProviderManager;

                            dbCandidate.LicenceNumber = procedure.VcLicenseNumber;

                            dbCandidate.LicenceDate = procedure.DtLicenseDate;

                            dbCandidate.ProviderName = "";

                            dbCandidate.ProviderAddress = "";

                            dbCandidate.ZipCode = "";

                            dbCandidate.OldId = (int)procedure.ProviderId;

                            dbCandidate.ModifyDate = DateTime.Now;

                            dbCandidate.CreationDate = DateTime.Now;

                            if (procedure.IntEkatteId != null)
                                dbCandidate.IdLocation = dbLocations
                                     .Where(x => x.LocationCode.Equals(locations
                                     .Where(m => m.Id == procedure.IntEkatteId)
                                     .First().VcTextCode))
                                     .First()
                                     .To<LocationVM>().idLocation;


                            dbCandidate.IdModifyUser = modifyUser.IdUser;

                            dbCandidate.IdCreateUser = modifyUser.IdUser;

                            dbCandidate.MigrationNote = "sp";

                            if (dbCandidate.ProviderOwner is not null)
                                dbCandidate.ProviderOwnerEN = ConvertCyrToLatin(dbCandidate.ProviderOwner);
                            if (dbCandidate.ProviderName is not null)
                                dbCandidate.ProviderNameEN = ConvertCyrToLatin(dbCandidate.ProviderName);
                            if (dbCandidate.ProviderAddressCorrespondence is not null)
                                dbCandidate.ProviderAddressCorrespondenceEN = ConvertCyrToLatin(dbCandidate.ProviderAddressCorrespondence);
                            if (dbCandidate.PersonNameCorrespondence is not null)
                                dbCandidate.PersonNameCorrespondenceEN = ConvertCyrToLatin(dbCandidate.PersonNameCorrespondence);
                            if (dbCandidate.ProviderAddress is not null)
                                dbCandidate.ProviderAddressEN = ConvertCyrToLatin(dbCandidate.ProviderAddress);

                            var doc = tb_procedure_documents.Where(x => x.StartedProcedureId == procedure.Id).FirstOrDefault();
                            if (doc is not null)
                            {
                                dbCandidate.ApplicationNumber = doc.DsOfficialNo;
                                dbCandidate.ApplicationDate = doc.DsOfficialDate;
                            }

                            dbCandidate.StartedProcedure = startedProcedure.To<StartedProcedure>();

                            if (candidateExist is null)
                            {
                                dbCandidate.IsActive = true;
                                dbCandidate.IdCandidateProviderActive = null;
                            }
                            else
                            {
                                dbCandidate.IsActive = false;
                                dbCandidate.IdCandidateProviderActive = candidateExist.IdCandidate_Provider;
                            }

                            candidateProviders.Add(dbCandidate);
                            _ApplicationDbContext.Add(dbCandidate);
                            _ApplicationDbContext.SaveChanges();
                        }
                        else
                        {
                            //Ако има запазваме само процедура
                            startedProcedures.Add(startedProcedure.To<StartedProcedure>());
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.logger.LogError("Гръмна метод ImportProcedure(Първи foreach). Запис с Id: " + procedure.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            };

            //_ApplicationDbContext.BulkInsert(startedProcedures.ToList());
            //_ApplicationDbContext.BulkSaveChanges();
            _ApplicationDbContext.AddRange(startedProcedures);
            _ApplicationDbContext.SaveChanges();

            //candidateProviders = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive).AsNoTracking().ToList();

            //ConcurrentBag<CandidateProvider> NewCandidateProviders = new ConcurrentBag<CandidateProvider>();
            //Parallel.ForEach(candidateProviders, candidate =>
            //{
            //    var inactiveCandidate = candidate.To<CandidateProviderVM>();
            //    if(candidate.OldId == 1)
            //    {

            //    }
            //    inactiveCandidate.IdCandidate_Provider = 0;
            //    inactiveCandidate.IsActive = false;
            //    inactiveCandidate.IdCandidateProviderActive = candidate.IdCandidate_Provider;
            //    var procedureId = candidate.IdStartedProcedure;
            //    candidate.IdStartedProcedure = null;
            //    inactiveCandidate.IdStartedProcedure = procedureId;

            //    var provider = tb_providers.Where(x => x.Id == candidate.OldId).FirstOrDefault();

            //    if (provider is not null)
            //    {
            //        var provStatus = ProviderStatuses
            //                .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderStatusId)
            //        .First();

            //        candidate.IdProviderStatus = provStatus.IdKeyValue;

            //        candidate.IdTypeLicense = provStatus.KeyValueIntCode.Contains("CPO") ? LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCPO")).First().IdKeyValue : LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCIPO")).First().IdKeyValue;

            //        candidate.IdLicenceStatus = LicenseStatuses
            //            .Where(x => int.Parse(x.DefaultValue2) == provider.IntLicenceStatusId)
            //            .First()
            //        .IdKeyValue;

            //        candidate.LicenceNumber = provider.IntLicenceNumber.ToString();

            //        if (provider.VcProviderOwner != null)
            //            candidate.ProviderOwner = provider.VcProviderOwner;

            //        if (provider.IntProviderBulstat != null)
            //            candidate.PoviderBulstat = provider.IntProviderBulstat;
            //        else
            //            candidate.PoviderBulstat = "";

            //        // int_local_group_id predpolagame che sa idta na useri ????????


            //        if (provider.IntEkatteId != null && provider.IntEkatteId != 0)

            //            candidate.IdLocation = dbLocations
            //                .Where(x => x.LocationCode.Equals(locations
            //                .Where(m => m.Id == provider.IntEkatteId)
            //                .First().VcTextCode))
            //                .First()
            //                .To<LocationVM>().idLocation;



            //        if (provider.VcZipCode != null)
            //            candidate.ZipCode = provider.VcZipCode;
            //        else
            //            candidate.ZipCode = "";

            //        if (provider.VcProviderAddress != null)
            //            candidate.ProviderAddress = provider.VcProviderAddress;
            //        else
            //            candidate.ProviderAddress = "";

            //        if (provider.VcProviderPhone2 == String.Empty)
            //            candidate.ProviderPhone = provider.VcProviderPhone1;
            //        else
            //            candidate.ProviderPhone = $"{provider.VcProviderPhone1}; {provider.VcProviderPhone2}";

            //        candidate.ProviderFax = provider.VcProviderFax;

            //        candidate.ProviderWeb = provider.VcProviderWeb;

            //        candidate.ProviderEmail = provider.VcProviderEmail;

            //        candidate.ProviderName = provider.VcProviderName;

            //        candidate.PersonNameCorrespondence = provider.VcProviderContactPers;

            //        if (provider.IntProviderContactPersEkatteId != null)
            //            candidate.IdLocation = dbLocations
            //                 .Where(x => x.LocationCode.Equals(locations
            //                 .Where(m => m.Id == provider.IntEkatteId)
            //                 .First().VcTextCode))
            //                 .First()
            //            .To<LocationVM>().idLocation;

            //        candidate.ZipCodeCorrespondence = provider.VcProviderContactPersZipcode;

            //        candidate.ProviderAddressCorrespondence = provider.VcProviderContactPersAddress;

            //        if (provider.IntProviderContactPersEkatteId != null)
            //            candidate.IdLocationCorrespondence = dbLocations
            //                .Where(x => x.kati.Equals(locations
            //                .Where(m => m.Id == provider.IntEkatteId)
            //                .First().VcName))
            //                .First()
            //                .To<LocationVM>().idLocation;

            //        if (provider.VcProviderContactPersPhone2 == String.Empty)
            //            candidate.ProviderPhoneCorrespondence = provider.VcProviderContactPersPhone1;
            //        else
            //            candidate.ProviderPhoneCorrespondence = $"{provider.VcProviderContactPersPhone1}; {provider.VcProviderContactPersPhone2}";

            //        candidate.ProviderFaxCorrespondence = provider.VcProviderContactPersFax;

            //        candidate.ProviderEmailCorrespondence = provider.VcProviderContactPersEmail;

            //        if (provider.IntProviderOwnershipId != null)
            //            candidate.IdProviderOwnership = ownership
            //                .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderOwnershipId)
            //                .First()
            //                .IdKeyValue;

            //        if (provider.VcProviderManager != null)
            //            candidate.ManagerName = provider.VcProviderManager;

            //        candidate.LicenceDate = provider.DtLicenceData;

            //        if (provider.IntProviderRegistrationId != null)
            //            candidate.IdProviderRegistration = registeredAt
            //                .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderRegistrationId)
            //                .First()
            //                .IdKeyValue;

            //        NewCandidateProviders.Add(inactiveCandidate.To<CandidateProvider>());
            //    }
            //});

            //this._ApplicationDbContext.BulkUpdate(candidateProviders);
            //this._ApplicationDbContext.BulkInsert(NewCandidateProviders.ToList());

            ConcurrentBag<StartedProcedureProgress> startedProcedureProgresses = new ConcurrentBag<StartedProcedureProgress>();
            var procedures = new List<StartedProcedure>();
            if (OldId is null)
                procedures = _ApplicationDbContext.StartedProcedures.ToList();
            else
                procedures = _ApplicationDbContext.StartedProcedures.Include(x => x.CandidateProviders).Where(x => x.CandidateProviders.First().OldId == OldId).ToList();

            var exceptions = new ConcurrentQueue<Exception>();

            Parallel.ForEach(tb_started_procedures_progress, progress =>
            {
                try
                {
                    StartedProcedureProgressVM procedure = new StartedProcedureProgressVM();
                    var pro = procedures.Where(x => x.OldId == progress.StartedProcedureId).FirstOrDefault();
                    if (pro is not null)
                    {
                        procedure.IdStartedProcedure = pro.IdStartedProcedure;
                        if (DocumentsFilledSubmittedToNAPOO.Contains((int)progress.StepId))
                        {
                            procedure.IdStep = DocumentsFilledSubmittedToNAPOOKV.IdKeyValue;
                        }
                        else if (RejectedApplicationLicensingNewCenter.Contains((int)progress.StepId))
                        {
                            procedure.IdStep = RejectedApplicationLicensingNewCenterKV.IdKeyValue;
                        }
                        else
                        {
                            procedure.IdStep = ApplicationStatuses.Where(x => Int32.Parse(x.DefaultValue2) == progress.StepId).First().IdKeyValue;
                        }

                        procedure.StepDate = progress.Ts;

                        procedure.ModifyDate = DateTime.Now;

                        procedure.CreationDate = DateTime.Now;

                        procedure.IdModifyUser = modifyUser.IdUser;

                        procedure.IdCreateUser = modifyUser.IdUser;

                        startedProcedureProgresses.Add(procedure.To<StartedProcedureProgress>());
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportProcedure(Първи Parallel.Foreach). Запис с Id = " + progress.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }

            });

            _ApplicationDbContext.BulkInsert(startedProcedureProgresses.ToList());

            var updateIdApplicationStatus = startedProcedureProgresses.GroupBy(x => x.IdStartedProcedure)
                .SelectMany(m => m.Where(s => s.StepDate == m.Max(m => m.StepDate)))
                .Distinct().ToList();

            var updatedCandidateProvides = new ConcurrentBag<CandidateProvider>();
            var candidates = new List<CandidateProvider>();
            if (OldId is null)
                candidates = this._ApplicationDbContext.CandidateProviders.ToList();
            else
                candidates = this._ApplicationDbContext.CandidateProviders.Where(x => x.OldId == OldId).ToList();

            foreach (var status in updateIdApplicationStatus)
            {
                var currentProvider = candidates.Where(x => x.IdStartedProcedure == status.IdStartedProcedure).FirstOrDefault();

                if (currentProvider is not null)
                {
                    try
                    {
                        currentProvider.IdApplicationStatus = status.IdStep;
                        this._ApplicationDbContext.Update(currentProvider);
                        this._ApplicationDbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportProcedure(Втори foreach). Запис с Id = " + status.IdStartedProcedureProgress);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                }
            }

            //if (OldId is null)
            //{
            //    var candidatesSP = this._ApplicationDbContext.CandidateProviders.Where(x => x.MigrationNote.Equals("sp") && x.IsActive).ToList();

            //    var spToUpdate = new ConcurrentBag<CandidateProvider>();
            //    var spToSave = new ConcurrentBag<CandidateProvider>();

            //    Parallel.ForEach(candidatesSP, candidate =>
            //    {
            //        var inactive = candidate.To<CandidateProviderVM>();

            //        if (inactive.IdApplicationStatus != ProcedureTerminatedByCenterKV.IdKeyValue && inactive.IdApplicationStatus != RejectedApplicationLicensingNewCenterKV.IdKeyValue)
            //        {
            //            inactive.IdApplicationStatus = RejectedApplicationLicensingNewCenterKV.IdKeyValue;
            //        }

            //        inactive.IsActive = false;

            //        inactive.IdCandidate_Provider = 0;

            //        inactive.IdCandidateProviderActive = candidate.IdCandidate_Provider;

            //        inactive.MigrationNote = "spn";

            //        candidate.IdStartedProcedure = null;
            //        candidate.IdApplicationStatus = null;
            //        candidate.IdTypeApplication = null;
            //        candidate.ApplicationNumber = null;
            //        candidate.ApplicationDate = null;
            //        candidate.IdReceiveLicense = null;

            //        var inactiveForDB = inactive.To<CandidateProvider>();

            //        inactiveForDB.Location = null;
            //        inactiveForDB.LocationCorrespondence = null;
            //        inactiveForDB.RegionAdmin = null;
            //        inactiveForDB.RegionCorrespondence = null;
            //        inactiveForDB.CandidateProviderActive = null;
            //        inactiveForDB.LocationCorrespondence = null;
            //        inactiveForDB.CandidateProviderSpecialities = null;
            //        inactiveForDB.CandidateProviderPeople = null;
            //        inactiveForDB.CandidateProviderTrainers = null;
            //        inactiveForDB.CandidateProviderStatuses = null;
            //        inactiveForDB.CandidateProviderPremises = null;
            //        inactiveForDB.CandidateProviderDocuments = null;
            //        inactiveForDB.RequestReports = null;
            //        inactiveForDB.ProviderDocumentOffers = null;
            //        inactiveForDB.AnnualInfos = null;
            //        inactiveForDB.Programs = null;
            //        inactiveForDB.Courses = null;
            //        inactiveForDB.CandidateProviderConsultings = null;
            //        inactiveForDB.ValidationClients = null;
            //        inactiveForDB.SelfAssessmentReports = null;
            //        inactiveForDB.CandidateProviderCPOStructureAndActivities = null;
            //        inactiveForDB.CandidateProviderCIPOStructureAndActivities = null;
            //        inactiveForDB.ProviderRequestDocuments = null;
            //        inactiveForDB.IdStartedProcedure = null;

            //        spToSave.Add(inactiveForDB);
            //        spToUpdate.Add(candidate);
            //    });

            //    this._ApplicationDbContext.BulkInsert(spToSave.ToList());
            //    this._ApplicationDbContext.BulkUpdate(spToUpdate.ToList());
            //}

            LogEndInformation("ImportProcedure");
        }
        #endregion

        #region CandidateProvider
        public void ImportProviderPremisies(int? OldId = null)
        {
            LogStrartInformation("ImportProviderPremisies");

            var activeStatus = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "MaterialTechnicalBaseStatus" && kv.KeyValueIntCode == "Active"
                                select kv).To<KeyValueVM>().First();

            var MaterialTechnicalBaseOwnerships = (from kv in _ApplicationDbContext.KeyValues
                                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                   where kt.KeyTypeIntCode == "MaterialTechnicalBaseOwnership"
                                                   select kv).To<KeyValueVM>().ToList();

            var tb_provider_premises = new List<TbProviderPremise>();

            if (OldId is null)
                tb_provider_premises = _jessieContextContext.TbProviderPremises.ToList();
            else
                tb_provider_premises = _jessieContextContext.TbProviderPremises.Where(x => x.IntProviderId == OldId).ToList();

            var candidates = _ApplicationDbContext.CandidateProviders.Where(x => !x.MigrationNote.Equals("cp")).ToList();

            var location = _ApplicationDbContext.Locations.ToList();

            ConcurrentBag<CandidateProviderPremises> candidateProviderPremises = new ConcurrentBag<CandidateProviderPremises>();

            //foreach (var providerPremise in tb_provider_premises)
            Parallel.ForEach(tb_provider_premises, providerPremise =>
            {
                try
                {
                    CandidateProviderPremisesVM premises = new CandidateProviderPremisesVM();

                    if (providerPremise.IntProviderId is not null)
                    {
                        var candidate = candidates.Where(x => x.OldId == providerPremise.IntProviderId).First();

                        premises.IdCandidate_Provider = candidate.IdCandidate_Provider;

                        premises.ZipCode = candidate.ZipCode;


                        premises.OldId = (int)providerPremise.Id;
                        //if (providerPremise.TxtProviderPremiseName != null)
                        //    premises.PremisesName = $"{providerPremise.IntProviderPremiseNo} {providerPremise.TxtProviderPremiseName.Trim()}";
                        //else
                        //    premises.PremisesName = $"{providerPremise.IntProviderPremiseNo} {providerPremise.TxtProviderPremiseName}";
                        if (providerPremise.TxtProviderPremiseName != null)
                            premises.PremisesName = providerPremise.TxtProviderPremiseName;
                        else
                            premises.PremisesName = "";

                        if (providerPremise.TxtProviderPremiseNotes != null)
                            premises.PremisesNote = providerPremise.TxtProviderPremiseNotes.Trim();
                        else
                            premises.PremisesNote = providerPremise.TxtProviderPremiseNotes;

                        if (providerPremise.IntProviderPremiseEkatte != null && providerPremise.IntProviderPremiseEkatte != 0)
                            premises.IdLocation = location
                                 .Where(x => x.LocationCode.Equals(locations
                                 .Where(m => m.Id == providerPremise.IntProviderPremiseEkatte)
                                 .First().VcTextCode))
                                 .First()
                                 .To<LocationVM>().idLocation;

                        if (providerPremise.TxtProviderPremiseAddress != null)
                            premises.ProviderAddress = providerPremise.TxtProviderPremiseAddress.Trim().Replace("\\", "");
                        else
                            premises.ProviderAddress = "";

                        if (providerPremise.IntProviderPremiseStatus != null)
                            premises.IdOwnership = MaterialTechnicalBaseOwnerships.Where(x => int.Parse(x.DefaultValue2) == providerPremise.IntProviderPremiseStatus).First().IdKeyValue;


                        providerPremise.TxtProviderPremiseAddress = providerPremise.TxtProviderPremiseAddress;

                        if (providerPremise.ts.HasValue)
                        {
                            premises.ModifyDate = providerPremise.ts.Value;
                            premises.CreationDate = providerPremise.ts.Value;
                        }
                        else
                        {
                            premises.ModifyDate = DateTime.Now;
                            premises.CreationDate = DateTime.Now;

                        }

                        premises.IdModifyUser = modifyUser.IdUser;

                        premises.IdCreateUser = modifyUser.IdUser;

                        premises.IdStatus = activeStatus.IdKeyValue;

                        candidateProviderPremises.Add(premises.To<CandidateProviderPremises>());
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportProviderPremisies(Първи Parallel.ForEach). Запис с Id = " + providerPremise.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(candidateProviderPremises.ToList());

            var premises = this._ApplicationDbContext.CandidateProviderPremises.ToList();

            ConcurrentBag<CandidateProviderPremisesChecking> checkings = new ConcurrentBag<CandidateProviderPremisesChecking>();
            Parallel.ForEach(tb_provider_premises, providerPremise =>
            {
                try
                {
                    CandidateProviderPremisesChecking checking = new CandidateProviderPremisesChecking();
                    if (providerPremise.BoolIsVisited.HasValue && providerPremise.BoolIsVisited == true)
                    {
                        var premise = premises.Where(x => x.OldId == providerPremise.Id).First();

                        checking.IdCandidateProviderPremises = premise.IdCandidateProviderPremises;

                        checking.CheckDone = providerPremise.BoolIsVisited.Value;

                        checking.Comment = "Базата е посетена от експерт на НАПОО. Информацията е прехвърлена от старата ИС на НАПОО към посочената дата.";

                        checking.CheckingDate = DateTime.Now;

                        checking.ModifyDate = DateTime.Now;

                        checking.CreationDate = DateTime.Now;

                        checking.IdModifyUser = modifyUser.IdUser;

                        checking.IdCreateUser = modifyUser.IdUser;

                        checkings.Add(checking);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportProviderPremisies(Втори Parallel.ForEach). Запис с Id = " + providerPremise.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(checkings.ToList());

            LogEndInformation("ImportProviderPremisies");
        }

        public void ImportCandidatePremisies(int? OldId = null)
        {
            LogStrartInformation("ImportCandidatePremisies");

            var activeStatus = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "MaterialTechnicalBaseStatus" && kv.KeyValueIntCode == "Active"
                                select kv).To<KeyValueVM>().First();

            var MaterialTechnicalBaseOwnerships = (from kv in _ApplicationDbContext.KeyValues
                                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                   where kt.KeyTypeIntCode == "MaterialTechnicalBaseOwnership"
                                                   select kv).To<KeyValueVM>().ToList();

            var tb_provider_premises = new List<TbCandidateProviderPremise>();

            if (OldId is null)
                tb_provider_premises = _jessieContextContext.TbCandidateProviderPremises.ToList();
            else
                tb_provider_premises = _jessieContextContext.TbCandidateProviderPremises.Where(x => x.IntProviderId == OldId).ToList();

            var candidates = _ApplicationDbContext.CandidateProviders.Where(x => x.MigrationNote.Equals("cp")).ToList();

            var location = _ApplicationDbContext.Locations.ToList();

            ConcurrentBag<CandidateProviderPremises> candidateProviderPremises = new ConcurrentBag<CandidateProviderPremises>();

            //foreach (var providerPremise in tb_provider_premises)
            Parallel.ForEach(tb_provider_premises, providerPremise =>
            {
                try
                {
                    CandidateProviderPremisesVM premises = new CandidateProviderPremisesVM();

                    if (providerPremise.IntProviderId != null)
                    {
                        var candidate = candidates.Where(x => x.OldId == providerPremise.IntProviderId).First();

                        premises.IdCandidate_Provider = candidate.IdCandidate_Provider;

                        premises.ZipCode = candidate.ZipCode;


                        premises.OldId = (int)providerPremise.Id;
                        //if (providerPremise.TxtProviderPremiseName != null)
                        //    premises.PremisesName = $"{providerPremise.IntProviderPremiseNo} {providerPremise.TxtProviderPremiseName.Trim()}";
                        //else
                        //    premises.PremisesName = $"{providerPremise.IntProviderPremiseNo} {providerPremise.TxtProviderPremiseName}";

                        if (providerPremise.TxtProviderPremiseName is not null)
                            premises.PremisesName = providerPremise.TxtProviderPremiseName;
                        else
                            premises.PremisesName = "";

                        if (providerPremise.TxtProviderPremiseNotes != null)
                            premises.PremisesNote = providerPremise.TxtProviderPremiseNotes.Trim();
                        else
                            premises.PremisesNote = providerPremise.TxtProviderPremiseNotes;

                        if (providerPremise.IntProviderPremiseEkatte != null && providerPremise.IntProviderPremiseEkatte != 0)
                            premises.IdLocation = location
                                  .Where(x => x.LocationCode.Equals(locations
                                  .Where(m => m.Id == providerPremise.IntProviderPremiseEkatte)
                                  .First().VcTextCode))
                                  .First()
                                  .To<LocationVM>().idLocation;

                        if (providerPremise.TxtProviderPremiseAddress != null)
                            premises.ProviderAddress = providerPremise.TxtProviderPremiseAddress.Replace("\\", "");
                        else
                            premises.ProviderAddress = "";

                        if (providerPremise.IntProviderPremiseStatus != null)
                            premises.IdOwnership = MaterialTechnicalBaseOwnerships.Where(x => int.Parse(x.DefaultValue2) == providerPremise.IntProviderPremiseStatus).First().IdKeyValue;

                        providerPremise.TxtProviderPremiseAddress = providerPremise.TxtProviderPremiseAddress;

                        premises.CreationDate = DateTime.Now;

                        premises.IdModifyUser = modifyUser.IdUser;

                        premises.IdCreateUser = modifyUser.IdUser;

                        premises.IdStatus = activeStatus.IdKeyValue;

                        premises.MigrationNote = "cp";

                        candidateProviderPremises.Add(premises.To<CandidateProviderPremises>());
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportCandidatePremisies(Първи Parallel.ForEach). Запис с Id = " + providerPremise.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(candidateProviderPremises.ToList());

            var premises = this._ApplicationDbContext.CandidateProviderPremises.Where(x => x.MigrationNote.Equals("cp")).ToList();

            ConcurrentBag<CandidateProviderPremisesChecking> checkings = new ConcurrentBag<CandidateProviderPremisesChecking>();
            Parallel.ForEach(tb_provider_premises, providerPremise =>
            {
                try
                {
                    CandidateProviderPremisesChecking checking = new CandidateProviderPremisesChecking();
                    if (providerPremise.BoolIsVisited.HasValue && providerPremise.BoolIsVisited == true)
                    {
                        var premise = premises.Where(x => x.OldId == providerPremise.Id).First();

                        checking.IdCandidateProviderPremises = premise.IdCandidateProviderPremises;

                        checking.CheckDone = providerPremise.BoolIsVisited.Value;

                        checking.Comment = "Базата е посетена от експерт на НАПОО. Информацията е прехвърлена от старата ИС на НАПОО към посочената дата.";

                        checking.CheckingDate = DateTime.Now;

                        checking.ModifyDate = DateTime.Now;

                        checking.CreationDate = DateTime.Now;

                        checking.IdModifyUser = modifyUser.IdUser;

                        checking.IdCreateUser = modifyUser.IdUser;

                        checking.MigrationNote = "cp";

                        checkings.Add(checking);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportCandidatePremisies(Първи Parallel.ForEach). Запис с Id = " + providerPremise.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(checkings.ToList());

            LogEndInformation("ImportCandidatePremisies");

        }

        public void ImportPremisiesRooms(int? OldId = null)
        {
            LogStrartInformation("ImportPremisiesRooms");

            var TrainingTypes = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "TrainingType"
                                 select kv).To<KeyValueVM>().ToList();

            var RoomTypes = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "RoomType"
                             select kv).To<KeyValueVM>().ToList();

            var tb_provider_premises_rooms = new List<TbProviderPremisesRoom>();

            if (OldId is null)
            {
                tb_provider_premises_rooms = _jessieContextContext.TbProviderPremisesRooms.ToList();
            }
            else
            {
                var tb_provider_premises = _jessieContextContext.TbProviderPremises.Where(x => x.IntProviderId == OldId).ToList();
                if (tb_provider_premises.Count != 0)
                    tb_provider_premises_rooms = _jessieContextContext.TbProviderPremisesRooms.ToList().Where(x => tb_provider_premises.Any(z => z.Id == x.IntPremiseId)).ToList();
            }
            var candidateProviderPremises = _ApplicationDbContext.CandidateProviderPremises.Where(x => !x.MigrationNote.Equals("cp")).ToList();

            ConcurrentBag<CandidateProviderPremisesRoom> rooms = new ConcurrentBag<CandidateProviderPremisesRoom>();

            //foreach (var premisesRoom in tb_provider_premises_rooms)
            Parallel.ForEach(tb_provider_premises_rooms, premisesRoom =>
            {
                try
                {
                    if (premisesRoom.IntPremiseId != null)
                    {
                        CandidateProviderPremisesRoomVM premises = new CandidateProviderPremisesRoomVM();
                        //premisesRoom.IntPremiseId = null

                        premises.IdCandidateProviderPremises = candidateProviderPremises.Where(x => x.OldId == premisesRoom.IntPremiseId).First().IdCandidateProviderPremises;

                        if (premisesRoom.TxtProviderPremiseRoomName != null)
                            premises.PremisesRoomName = premisesRoom.TxtProviderPremiseRoomName;
                        else
                            premises.PremisesRoomName = "";


                        premises.Equipment = premisesRoom.TxtProviderPremiseRoomEquipment;
                        if (premisesRoom.IntProviderPremiseRoomUsage != null)
                            premises.IdUsage = TrainingTypes.Where(x => int.Parse(x.DefaultValue2) == premisesRoom.IntProviderPremiseRoomUsage).First().IdKeyValue;
                        if (premisesRoom.IntProviderPremiseRoomType != null)
                            premises.IdPremisesType = RoomTypes.Where(x => int.Parse(x.DefaultValue2) == premisesRoom.IntProviderPremiseRoomType).First().IdKeyValue;

                        premises.Area = premisesRoom.IntProviderPremiseRoomArea;

                        premises.Workplace = premisesRoom.IntProviderPremiseRoomWorkplaces;

                        premises.IdCreateUser = modifyUser.IdUser;

                        premises.IdModifyUser = modifyUser.IdUser;

                        premises.ModifyDate = DateTime.Now;

                        premises.CreationDate = DateTime.Now;

                        premises.OldId = (int)premisesRoom.Id;

                        rooms.Add(premises.To<CandidateProviderPremisesRoom>());
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportPremisiesRooms(Първи Parallel.ForEach). Запис с Id = " + premisesRoom.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }

            });

            _ApplicationDbContext.BulkInsert(rooms.ToList());
            LogEndInformation("ImportPremisiesRooms");
        }

        public void ImportCandidatePremisiesRooms(int? OldId = null)
        {
            LogStrartInformation("ImportCandidatePremisiesRooms");

            var TrainingTypes = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "TrainingType"
                                 select kv).To<KeyValueVM>().ToList();

            var RoomTypes = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "RoomType"
                             select kv).To<KeyValueVM>().ToList();

            var tb_candidate_provider_premises_rooms = new List<TbCandidateProviderPremisesRoom>();

            if (OldId is null)
            {
                tb_candidate_provider_premises_rooms = _jessieContextContext.TbCandidateProviderPremisesRooms.ToList();
            }
            else
            {
                var tb_provider_premises = _jessieContextContext.TbCandidateProviderPremises.Where(x => x.IntProviderId == OldId).ToList();
                if (tb_provider_premises.Count != 0)
                    tb_candidate_provider_premises_rooms = _jessieContextContext.TbCandidateProviderPremisesRooms.ToList().Where(x => tb_provider_premises.Any(z => z.Id == x.IntPremiseId)).ToList();
            }

            var candidateProviderPremises = _ApplicationDbContext.CandidateProviderPremises.Where(x => x.MigrationNote.Equals("cp")).ToList();

            ConcurrentBag<CandidateProviderPremisesRoom> rooms = new ConcurrentBag<CandidateProviderPremisesRoom>();

            //foreach (var premisesRoom in tb_provider_premises_rooms)
            Parallel.ForEach(tb_candidate_provider_premises_rooms, premisesRoom =>
            {
                try
                {
                    if (premisesRoom.IntPremiseId != null)
                    {
                        CandidateProviderPremisesRoomVM premises = new CandidateProviderPremisesRoomVM();
                        //premisesRoom.IntPremiseId = null

                        var premisie = candidateProviderPremises.Where(x => x.OldId == premisesRoom.IntPremiseId).FirstOrDefault();

                        if (premisie != null)
                        {
                            premises.IdCandidateProviderPremises = premisie.IdCandidateProviderPremises;
                            if (premisesRoom.TxtProviderPremiseRoomName != null)
                                premises.PremisesRoomName = premisesRoom.TxtProviderPremiseRoomName;
                            else
                                premises.PremisesRoomName = "";


                            premises.Equipment = premisesRoom.TxtProviderPremiseRoomEquipment;
                            if (premisesRoom.IntProviderPremiseRoomUsage != null)
                                premises.IdUsage = TrainingTypes.Where(x => int.Parse(x.DefaultValue2) == premisesRoom.IntProviderPremiseRoomUsage).First().IdKeyValue;
                            if (premisesRoom.IntProviderPremiseRoomType != null)
                                premises.IdPremisesType = RoomTypes.Where(x => int.Parse(x.DefaultValue2) == premisesRoom.IntProviderPremiseRoomType).First().IdKeyValue;

                            premises.Area = premisesRoom.IntProviderPremiseRoomArea;

                            premises.Workplace = premisesRoom.IntProviderPremiseRoomWorkplaces;

                            premises.IdCreateUser = modifyUser.IdUser;

                            premises.IdModifyUser = modifyUser.IdUser;

                            premises.ModifyDate = DateTime.Now;

                            premises.CreationDate = DateTime.Now;

                            premises.MigrationNote = "cp";

                            premises.OldId = (int)premisesRoom.Id;

                            rooms.Add(premises.To<CandidateProviderPremisesRoom>());
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportCandidatePremisiesRooms(Първи Parallel.ForEach). Запис с Id = " + premisesRoom.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(rooms.ToList());

            LogEndInformation("ImportCandidatePremisiesRooms");
        }

        public void ImportPremisesSpecialities(int? OldId = null)
        {
            LogStrartInformation("ImportPremisesSpecialities");

            var ref_provider_premises_specialities = new List<RefProviderPremisesSpeciality>();
            if (OldId is null)
            {
                ref_provider_premises_specialities = this._jessieContextContext.RefProviderPremisesSpecialities.ToList();
            }
            else
            {
                var tb_provider_premises = _jessieContextContext.TbProviderPremises.Where(x => x.IntProviderId == OldId).ToList();
                if (tb_provider_premises.Count != 0)
                    ref_provider_premises_specialities = _jessieContextContext.RefProviderPremisesSpecialities.ToList().Where(x => tb_provider_premises.Any(z => z.Id == x.IntProviderPremiseId)).ToList();

            }
            var specialities = this._ApplicationDbContext.CandidateProviderSpecialities.ToList();

            var premisises = this._ApplicationDbContext.CandidateProviderPremises.Where(x => !x.MigrationNote.Equals("cp")).ToList();

            //TrainingType
            var trainingTypes = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "TrainingType"
                                 select kv).To<KeyValueVM>().ToList();

            var complianceDOC = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "ComplianceDOC"
                                 select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<CandidateProviderPremisesSpeciality> premisesSpecialities = new ConcurrentBag<CandidateProviderPremisesSpeciality>();

            Parallel.ForEach(ref_provider_premises_specialities, premisie =>
            {
                try
                {
                    CandidateProviderPremisesSpeciality providerPremisesSpeciality = new CandidateProviderPremisesSpeciality();

                    providerPremisesSpeciality.IdSpeciality = specialities.Where(x => x.OldId == premisie.IntProviderSpecialityId).First().IdSpeciality;

                    providerPremisesSpeciality.IdCandidateProviderPremises = premisises.Where(x => x.OldId == premisie.IntProviderPremiseId).First().IdCandidateProviderPremises;

                    if (premisie.IntProviderPremiseSpecialityUsage is not null)
                    {
                        providerPremisesSpeciality.IdUsage = trainingTypes.Where(x => Int32.Parse(x.DefaultValue2) == premisie.IntProviderPremiseSpecialityUsage).First().IdKeyValue;
                    }
                    if (premisie.IntProviderPremiseSpecialityCorrespondence is not null)
                    {
                        providerPremisesSpeciality.IdComplianceDOC = complianceDOC.Where(x => Int32.Parse(x.DefaultValue2) == premisie.IntProviderPremiseSpecialityCorrespondence).First().IdKeyValue;
                    }

                    providerPremisesSpeciality.ModifyDate = DateTime.Now;

                    providerPremisesSpeciality.CreationDate = DateTime.Now;

                    providerPremisesSpeciality.IdCreateUser = modifyUser.IdUser;

                    providerPremisesSpeciality.IdModifyUser = modifyUser.IdUser;

                    premisesSpecialities.Add(providerPremisesSpeciality);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportPremisesSpecialities(Първи Parallel.ForEach). Запис с Id = " + premisie.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(premisesSpecialities.ToList());

            LogEndInformation("ImportPremisesSpecialities");
        }

        public void ImportCandidatePremisesSpecialities(int? OldId = null)
        {
            LogStrartInformation("ImportCandidatePremisesSpecialities");

            var ref_candidate_provider_premises_specialities = new List<RefCandidateProviderPremisesSpeciality>();

            if (OldId is null)
            {
                ref_candidate_provider_premises_specialities = this._jessieContextContext.RefCandidateProviderPremisesSpecialities.ToList();
            }
            else
            {
                var tb_provider_premises = _jessieContextContext.TbProviderPremises.Where(x => x.IntProviderId == OldId).ToList();
                if (tb_provider_premises.Count != 0)
                    ref_candidate_provider_premises_specialities = _jessieContextContext.RefCandidateProviderPremisesSpecialities.ToList().Where(x => tb_provider_premises.Any(z => z.Id == x.IntProviderPremiseId)).ToList();

            }
            var specialities = this._ApplicationDbContext.CandidateProviderSpecialities.ToList();

            var premisises = this._ApplicationDbContext.CandidateProviderPremises.Where(x => x.MigrationNote.Equals("cp")).ToList();

            //TrainingType
            var trainingTypes = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "TrainingType"
                                 select kv).To<KeyValueVM>().ToList();

            var complianceDOC = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "ComplianceDOC"
                                 select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<CandidateProviderPremisesSpeciality> premisesSpecialities = new ConcurrentBag<CandidateProviderPremisesSpeciality>();

            Parallel.ForEach(ref_candidate_provider_premises_specialities, premisie =>
            {
                try
                {
                    CandidateProviderPremisesSpeciality providerPremisesSpeciality = new CandidateProviderPremisesSpeciality();

                    providerPremisesSpeciality.IdSpeciality = specialities.Where(x => x.OldId == premisie.IntProviderSpecialityId).First().IdSpeciality;

                    providerPremisesSpeciality.IdCandidateProviderPremises = premisises.Where(x => x.OldId == premisie.IntProviderPremiseId).First().IdCandidateProviderPremises;

                    if (premisie.IntProviderPremiseSpecialityUsage is not null)
                    {
                        providerPremisesSpeciality.IdUsage = trainingTypes.Where(x => Int32.Parse(x.DefaultValue2) == premisie.IntProviderPremiseSpecialityUsage).First().IdKeyValue;
                    }
                    if (premisie.IntProviderPremiseSpecialityCorrespondence is not null)
                    {
                        providerPremisesSpeciality.IdUsage = complianceDOC.Where(x => Int32.Parse(x.DefaultValue2) == premisie.IntProviderPremiseSpecialityCorrespondence).First().IdKeyValue;
                    }

                    providerPremisesSpeciality.ModifyDate = DateTime.Now;

                    providerPremisesSpeciality.CreationDate = DateTime.Now;

                    providerPremisesSpeciality.IdCreateUser = modifyUser.IdUser;

                    providerPremisesSpeciality.IdModifyUser = modifyUser.IdUser;

                    premisesSpecialities.Add(providerPremisesSpeciality);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportCandidatePremisesSpecialities(Първи Parallel.ForEach). Запис с Id = " + premisie.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(premisesSpecialities.ToList());

            LogEndInformation("ImportCandidatePremisesSpecialities");
        }

        public void ImportProviderTrainer(int? OldId = null)
        {
            LogStrartInformation("ImportProviderTrainer");

            var tb_trainers = new List<TbTrainer>();
            if (OldId is null)
                tb_trainers = _jessieContextContext.TbTrainers.ToList();
            else
                tb_trainers = _jessieContextContext.TbTrainers.Where(x => x.IntProviderId == OldId).ToList();


            var CandidateProviders = _ApplicationDbContext.CandidateProviders.Where(x => !x.MigrationNote.Equals("cp")).ToList();

            var activeStatus = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "CandidateProviderTrainerStatus" && kv.KeyValueIntCode == "Active"
                                select kv).To<KeyValueVM>().First().IdKeyValue;

            var indentTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "IndentType"
                               select kv).To<KeyValueVM>().ToList();

            var sex = (from kv in _ApplicationDbContext.KeyValues
                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                       where kt.KeyTypeIntCode == "Sex"
                       select kv).To<KeyValueVM>().ToList();

            var nationality = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "Nationality"
                               select kv).To<KeyValueVM>().ToList();

            var education = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "Education" && !string.IsNullOrEmpty(kv.DefaultValue2)
                             select kv).To<KeyValueVM>().ToList();

            var contactTypes = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "TrainerContractType"
                                select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<CandidateProviderTrainer> trainers = new ConcurrentBag<CandidateProviderTrainer>();

            //foreach (var tb in tb_trainers)
            Parallel.ForEach(tb_trainers, tb =>
            {
                try
                {
                    CandidateProviderTrainerVM trainer = new CandidateProviderTrainerVM();

                    if (tb.IntProviderId != null && tb.IntProviderId != 0)
                        trainer.IdCandidate_Provider = CandidateProviders.Where(x => x.OldId == tb.IntProviderId && x.IsActive).First().IdCandidate_Provider;

                    if (tb.VcTrainerFirstName != null)
                        trainer.FirstName = tb.VcTrainerFirstName;
                    else
                        trainer.FirstName = "";

                    trainer.SecondName = tb.VcTrainerSecondName;

                    if (tb.VcTrainerFamilyName != null)
                        trainer.FamilyName = tb.VcTrainerFamilyName;
                    else
                        trainer.FamilyName = "";

                    if (tb.IntEgnTypeId != null)
                    {
                        var indentType = indentTypes.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntEgnTypeId).First();
                        trainer.IdIndentType = indentType.IdKeyValue;

                        try
                        {
                            if (indentType.KeyValueIntCode.Equals("EGN") && tb.VcEgn != null)
                                trainer.BirthDate = GetBirthDate(tb.VcEgn);
                        }
                        catch (Exception e) { }
                    }
                    if (tb.VcEgn != null && tb.VcEgn.Length <= 10)
                        trainer.Indent = tb.VcEgn;
                    else if (tb.VcEgn == null)
                        trainer.Indent = null;
                    else
                        trainer.Indent = "long";

                    if (tb.IntGenderId != null && tb.IntGenderId != 0)
                        trainer.IdSex = sex.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntGenderId).First().IdKeyValue;

                    if (tb.IntNationalityId != null && tb.IntNationalityId != 0)
                        trainer.IdNationality = nationality.Where(x => x.Order == tb.IntNationalityId).First().IdKeyValue;

                    if (!string.IsNullOrEmpty(tb.VcEmail))
                        trainer.Email = tb.VcEmail;

                    trainer.ContractDate = tb.DtTcontractDate;

                    trainer.IdStatus = activeStatus;

                    if (tb.IntEducationId != null && tb.IntEducationId != 0)
                    {
                        if (tb.IntEducationId == 24) tb.IntEducationId = 30;
                        trainer.IdEducation = education.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntEducationId).First().IdKeyValue;
                    }
                    trainer.EducationSpecialityNotes = tb.TxtEducationSpecialityNotes;

                    trainer.EducationCertificateNotes = tb.TxtEducationCertificateNotes;

                    trainer.EducationAcademicNotes = tb.TxtEducationAcademicNotes;

                    if (tb.IntTcontractTypeId != null && tb.IntTcontractTypeId != 0)
                        trainer.IdContractType = contactTypes.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntTcontractTypeId).First().IdKeyValue;

                    if (tb.BoolIsAndragog != null)
                        trainer.IsAndragog = (bool)tb.BoolIsAndragog;

                    trainer.OldId = (int)tb.Id;

                    trainer.ModifyDate = DateTime.Now;

                    trainer.CreationDate = DateTime.Now;

                    trainer.IdCreateUser = modifyUser.IdUser;

                    trainer.IdModifyUser = modifyUser.IdUser;

                    trainers.Add(trainer.To<CandidateProviderTrainer>());
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportProviderTrainer(Първи Parallel.ForEach). Запис с Id = " + tb.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(trainers.ToList());
            LogEndInformation("ImportProviderTrainer");
        }

        public void ImportCandidateProviderTrainer(int? OldId = null)
        {
            LogStrartInformation("ImportCandidateProviderTrainer");

            var tb_candidate_trainers = new List<TbCandidateTrainer>();
            if (OldId is null)
                tb_candidate_trainers = _jessieContextContext.TbCandidateTrainers.ToList();
            else
                tb_candidate_trainers = _jessieContextContext.TbCandidateTrainers.Where(x => x.IntProviderId == OldId).ToList();

            var CandidateProviders = _ApplicationDbContext.CandidateProviders.Where(x => x.MigrationNote.Equals("cp"))
                .ToList().OrderByDescending(x => x.IdCandidate_Provider);

            var indentTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "IndentType"
                               select kv).To<KeyValueVM>().ToList();

            var sex = (from kv in _ApplicationDbContext.KeyValues
                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                       where kt.KeyTypeIntCode == "Sex"
                       select kv).To<KeyValueVM>().ToList();

            var nationality = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "Nationality"
                               select kv).To<KeyValueVM>().ToList();

            var education = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "Education" && !string.IsNullOrEmpty(kv.DefaultValue2)
                             select kv).To<KeyValueVM>().ToList();

            var contactTypes = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "TrainerContractType"
                                select kv).To<KeyValueVM>().ToList();

            var activeStatus = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "CandidateProviderTrainerStatus" && kv.KeyValueIntCode == "Active"
                                select kv).To<KeyValueVM>().First().IdKeyValue;

            ConcurrentBag<CandidateProviderTrainer> trainers = new ConcurrentBag<CandidateProviderTrainer>();

            //foreach (var tb in tb_trainers)
            Parallel.ForEach(tb_candidate_trainers, tb =>
            {
                try
                {
                    CandidateProviderTrainerVM trainer = new CandidateProviderTrainerVM();

                    if (tb.IntProviderId != null && tb.IntProviderId != 0)
                        trainer.IdCandidate_Provider = CandidateProviders.Where(x => x.OldId == tb.IntProviderId).First().IdCandidate_Provider;

                    if (tb.VcTrainerFirstName != null)
                        trainer.FirstName = tb.VcTrainerFirstName;
                    else
                        trainer.FirstName = "";

                    trainer.SecondName = tb.VcTrainerSecondName;

                    if (tb.VcTrainerFamilyName != null)
                        trainer.FamilyName = tb.VcTrainerFamilyName;
                    else
                        trainer.FamilyName = "";

                    if (tb.IntEgnTypeId != null)
                    {
                        var indentType = indentTypes.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntEgnTypeId).First();
                        trainer.IdIndentType = indentType.IdKeyValue;

                        try
                        {
                            if (indentType.KeyValueIntCode.Equals("EGN") && tb.VcEgn != null)
                                trainer.BirthDate = GetBirthDate(tb.VcEgn);
                        }
                        catch (Exception e) { }
                    }
                    if (tb.VcEgn != null && tb.VcEgn.Length <= 10)
                        trainer.Indent = tb.VcEgn;
                    else if (tb.VcEgn == null)
                        trainer.Indent = null;
                    else
                        trainer.Indent = "long";

                    if (tb.IntGenderId != null && tb.IntGenderId != 0)
                        trainer.IdSex = sex.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntGenderId).First().IdKeyValue;

                    if (tb.IntNationalityId != null && tb.IntNationalityId != 0)
                        trainer.IdNationality = nationality.Where(x => x.Order == tb.IntNationalityId).First().IdKeyValue;

                    if (!string.IsNullOrEmpty(tb.VcEmail))
                        trainer.Email = tb.VcEmail;

                    if (tb.IntEducationId != null && tb.IntEducationId != 0)
                    {
                        if (tb.IntEducationId == 24)
                            tb.IntEducationId = 30;

                        trainer.IdEducation = education.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntEducationId).First().IdKeyValue;
                    }

                    trainer.IdStatus = activeStatus;

                    trainer.EducationSpecialityNotes = tb.TxtEducationSpecialityNotes;

                    trainer.EducationCertificateNotes = tb.TxtEducationCertificateNotes;

                    trainer.EducationAcademicNotes = tb.TxtEducationAcademicNotes;

                    if (tb.IntTcontractTypeId != null && tb.IntTcontractTypeId != 0)
                        trainer.IdContractType = contactTypes.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntTcontractTypeId).First().IdKeyValue;

                    if (tb.BoolIsAndragog != null)
                        trainer.IsAndragog = (bool)tb.BoolIsAndragog;

                    trainer.OldId = (int)tb.Id;

                    trainer.ModifyDate = DateTime.Now;

                    trainer.CreationDate = DateTime.Now;

                    trainer.IdCreateUser = modifyUser.IdUser;

                    trainer.IdModifyUser = modifyUser.IdUser;

                    trainer.ContractDate = tb.DtTcontractDate;

                    trainer.MigrationNote = "cp";

                    trainers.Add(trainer.To<CandidateProviderTrainer>());
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportCandidateProviderTrainer(Първи Parallel.ForEach). Запис с Id = " + tb.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(trainers.ToList());
            LogEndInformation("ImportCandidateProviderTrainer");

        }

        public void ImportCandidateProviders(int? OldId = null)
        {
            LogStrartInformation("ImportCandidateProviders");

            List<TbCandidateProvider> tb_providers;
            if (OldId is null)
                tb_providers = this._jessieContextContext.TbCandidateProviders.ToList();
            else
                tb_providers = this._jessieContextContext.TbCandidateProviders.Where(x => x.Id == OldId).ToList();
            var cps = this._ApplicationDbContext.CandidateProviders.ToList();

            var dbLocations = _ApplicationDbContext.Locations.ToList();

            //KeyValues 
            var ProviderStatuses = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "ProviderStatus"
                                    select kv).To<KeyValueVM>().ToList();
            var LicenseStatuses = (from kv in _ApplicationDbContext.KeyValues
                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                   where kt.KeyTypeIntCode == "LicenseStatus"
                                   select kv).To<KeyValueVM>().ToList();

            var ownership = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "ProviderOwnership"
                             select kv).To<KeyValueVM>().ToList();

            var registeredAt = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "ProviderRegisteredAt"
                                select kv).To<KeyValueVM>().ToList();

            var MaterialTechnicalBaseStatuses = (from kv in _ApplicationDbContext.KeyValues
                                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                 where kt.KeyTypeIntCode == "MaterialTechnicalBaseStatus" && kv.DefaultValue1.Trim() != String.Empty
                                                 select kv).To<KeyValueVM>().ToList();

            var TypeApplications = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "TypeApplication"
                                    select kv).To<KeyValueVM>().ToList();


            var LicensingTypes = (from kv in _ApplicationDbContext.KeyValues
                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                  where kt.KeyTypeIntCode == "LicensingType"
                                  select kv).To<KeyValueVM>().ToList();

            var ApplicationFilingTypes = (from kv in _ApplicationDbContext.KeyValues
                                          join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                          where kt.KeyTypeIntCode == "ApplicationFilingType" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                          select kv).To<KeyValueVM>().ToList();

            var ReceiveLicenseTypes = (from kv in _ApplicationDbContext.KeyValues
                                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                       where kt.KeyTypeIntCode == "ReceiveLicenseType"
                                       select kv).To<KeyValueVM>().ToList();


            var ApplicationStatuses = (from kv in _ApplicationDbContext.KeyValues
                                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                       where kt.KeyTypeIntCode == "ApplicationStatus" && !string.IsNullOrEmpty(kv.DefaultValue2.Trim()) && !kv.KeyValueIntCode.Equals("RequestedByCPOOrCIPO")
                                       select kv).To<KeyValueVM>().ToList();

            var startedProcedures = _ApplicationDbContext.StartedProcedures.ToList();

            var tb_started_prcedures = _jessieContextContext.TbStartedProcedures.ToList();

            var tb_procedure_documents = _jessieContextContext.TbProcedureDocuments.Where(x => (x.StageDocumentId == 100 || x.StageDocumentId == 200 || x.StageDocumentId == 300 || x.StageDocumentId == 500) && x.IsValid == true && !string.IsNullOrEmpty(x.Filename)).ToList();

            var DocumentsFilledSubmittedToNAPOOKV = (from kv in _ApplicationDbContext.KeyValues
                                                     join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                     where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode == "RequestedByCPOOrCIPO"
                                                     select kv).To<KeyValueVM>().First();

            var ProcedureWasRegisteredInKeepingSystemKV = (from kv in _ApplicationDbContext.KeyValues
                                                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                           where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode == "ProcedureWasRegisteredInKeepingSystem"
                                                           select kv).To<KeyValueVM>().First();

            var PreparationDocumentationLicensingKV = (from kv in _ApplicationDbContext.KeyValues
                                                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                       where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode == "PreparationDocumentationLicensing"
                                                       select kv).To<KeyValueVM>().First();

            var AdministrativeCheckKV = (from kv in _ApplicationDbContext.KeyValues
                                         join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                         where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode == "AdministrativeCheck"
                                         select kv).To<KeyValueVM>().First();

            var ExpertCommissionAssessmentKV = (from kv in _ApplicationDbContext.KeyValues
                                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode == "ExpertCommissionAssessment"
                                                select kv).To<KeyValueVM>().First();

            var LeadingExpertGavePositiveAssessmentKV = (from kv in _ApplicationDbContext.KeyValues
                                                         join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                         where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode == "LeadingExpertGavePositiveAssessment"
                                                         select kv).To<KeyValueVM>().First();

            var LicensingExpertiseStartedKV = (from kv in _ApplicationDbContext.KeyValues
                                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                               where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode == "LicensingExpertiseStarted"
                                               select kv).To<KeyValueVM>().First();

            var ProcedureTerminatedByCenterKV = (from kv in _ApplicationDbContext.KeyValues
                                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                 where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode == "ProcedureTerminatedByCenter"
                                                 select kv).To<KeyValueVM>().First();

            var RejectedApplicationLicensingNewCenterKV = (from kv in _ApplicationDbContext.KeyValues
                                                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                           where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode == "RejectedApplicationLicensingNewCenter"
                                                           select kv).To<KeyValueVM>().First();


            var DocumentsFilledSubmittedToNAPOO = new List<long>() { 28, 29, 43, 44, 31, 20, 34, 49 };

            var ProcedureWasRegisteredInKeepingSystem = new List<long>() { 45, 74, 30, 75 };

            var PreparationDocumentationLicensing = new List<long> { 6, 7, 13, 18, 19, 33, 48, 12, 32, 47, 4, 5, 11, 16, 17 };

            var AdministrativeCheck = new List<long>() { 21, 35, 50 };

            var ExpertCommissionAssessment = new List<long>() { 22, 36, 51, 72, 73 };

            var LeadingExpertGavePositiveAssessment = new List<long>() { 23, 37, 52 };

            var LicensingExpertiseStarted = new List<long>() { 24, 38, 53, 10 };

            var ProcedureTerminatedByCenter = new List<long>() { 69, 70, 71, 62, 63, 64, 65, 66, 67, 68 };

            var RejectedApplicationLicensingNewCenter = new List<long>() { 46 };

            ConcurrentBag<CandidateProvider> candidates = new ConcurrentBag<CandidateProvider>();

            //foreach (var provider in tb_providers)
            Parallel.ForEach(tb_providers, provider =>
            {
                CandidateProviderVM candidateProvider = new CandidateProviderVM();
                if (provider.Id != 0)
                {
                    try
                    {
                        candidateProvider.OldId = provider.Id;

                        if (provider.IntProviderStatusId != null)
                        {
                            var provStatus = ProviderStatuses
                             .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderStatusId)
                             .First();

                            candidateProvider.IdProviderStatus = provStatus.IdKeyValue;

                            candidateProvider.IdTypeLicense = provStatus.KeyValueIntCode.Contains("CPO") ? LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCPO")).First().IdKeyValue : LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCIPO")).First().IdKeyValue;

                        }
                        else
                            candidateProvider.IdProviderStatus = 0;

                        candidateProvider.LicenceNumber = provider.IntLicenceNumber.ToString();

                        if (provider.VcProviderOwner != null)
                            candidateProvider.ProviderOwner = provider.VcProviderOwner;

                        if (provider.IntProviderBulstat != null)
                            candidateProvider.PoviderBulstat = provider.IntProviderBulstat;
                        else
                            candidateProvider.PoviderBulstat = "";

                        // int_local_group_id predpolagame che sa idta na useri ????????

                        if (provider.IntEkatteId != null && provider.IntEkatteId != 0)
                            candidateProvider.IdLocation = dbLocations
                           .Where(x => x.LocationCode.Equals(locations
                                 .Where(m => m.Id == provider.IntEkatteId)
                                 .First().VcTextCode))
                                 .First()
                                 .To<LocationVM>().idLocation;


                        if (provider.VcZipCode != null)
                            candidateProvider.ZipCode = provider.VcZipCode;
                        else
                            candidateProvider.ZipCode = "";

                        if (provider.VcProviderAddress != null)
                            candidateProvider.ProviderAddress = provider.VcProviderAddress.Replace("\\", "");
                        else
                            candidateProvider.ProviderAddress = "";

                        if (provider.StepId is not null)
                        {
                            if (provider.StepId == 1 || provider.StepId == 6)
                            {
                                candidateProvider.IdApplicationStatus = DocumentsFilledSubmittedToNAPOOKV.IdKeyValue;
                            }
                            else if (RejectedApplicationLicensingNewCenter.Contains((int)provider.StepId))
                            {
                                candidateProvider.IdApplicationStatus = RejectedApplicationLicensingNewCenterKV.IdKeyValue;
                            }
                            else
                            {
                                candidateProvider.IdApplicationStatus = ApplicationStatuses
                                .Where(x => Int32.Parse(x.DefaultValue2) == provider.StepId)
                                .First().IdKeyValue;

                            }
                        }
                        else
                        {
                            var oldStatusId = (int)provider.IntOperationId;


                            if (DocumentsFilledSubmittedToNAPOO.Contains(oldStatusId))
                                candidateProvider.IdApplicationStatus = DocumentsFilledSubmittedToNAPOOKV.IdKeyValue;
                            else if (ProcedureWasRegisteredInKeepingSystem.Contains(oldStatusId))
                                candidateProvider.IdApplicationStatus = ProcedureWasRegisteredInKeepingSystemKV.IdKeyValue;

                            else if (PreparationDocumentationLicensing.Contains(oldStatusId))
                                candidateProvider.IdApplicationStatus = PreparationDocumentationLicensingKV.IdKeyValue;

                            else if (AdministrativeCheck.Contains(oldStatusId))
                                candidateProvider.IdApplicationStatus = AdministrativeCheckKV.IdKeyValue;

                            else if (ExpertCommissionAssessment.Contains(oldStatusId))
                                candidateProvider.IdApplicationStatus = ExpertCommissionAssessmentKV.IdKeyValue;

                            else if (LeadingExpertGavePositiveAssessment.Contains(oldStatusId))
                                candidateProvider.IdApplicationStatus = LeadingExpertGavePositiveAssessmentKV.IdKeyValue;

                            else if (LicensingExpertiseStarted.Contains(oldStatusId))
                                candidateProvider.IdApplicationStatus = LicensingExpertiseStartedKV.IdKeyValue;

                            else if (ProcedureTerminatedByCenter.Contains(oldStatusId))
                                candidateProvider.IdApplicationStatus = ProcedureTerminatedByCenterKV.IdKeyValue;
                            else if (RejectedApplicationLicensingNewCenter.Contains(oldStatusId))
                                candidateProvider.IdApplicationStatus = RejectedApplicationLicensingNewCenterKV.IdKeyValue;
                        }

                        if (string.IsNullOrEmpty(provider.VcProviderPhone1))
                            candidateProvider.ProviderPhone = "";
                        else if (!string.IsNullOrEmpty(provider.VcProviderPhone1) && !string.IsNullOrEmpty(provider.VcProviderPhone2))
                            candidateProvider.ProviderPhone = $"{provider.VcProviderPhone1}; {provider.VcProviderPhone2}";
                        else
                            candidateProvider.ProviderPhone = provider.VcProviderPhone1;

                        if (provider.IntReceiveTypeId is not null && provider.IntReceiveTypeId != 0)
                            candidateProvider.IdReceiveLicense = ReceiveLicenseTypes
                            .Where(x => Int32.Parse(x.DefaultValue2) == provider.IntReceiveTypeId)
                            .First().IdKeyValue;

                        candidateProvider.ProviderFax = provider.VcProviderFax;

                        candidateProvider.ProviderWeb = provider.VcProviderWeb;

                        if (!string.IsNullOrEmpty(provider.VcProviderProfile))
                        {
                            if (provider.VcProviderProfile.Length > 512)
                                candidateProvider.AdditionalInfo = provider.VcProviderProfile.Substring(0, 511);
                            else
                                candidateProvider.AdditionalInfo = provider.VcProviderProfile;
                        }

                        candidateProvider.ProviderEmail = provider.VcProviderEmail;

                        candidateProvider.ManagerName = provider.VcProviderManager;

                        candidateProvider.ProviderName = provider.VcProviderName;

                        if (provider.IntCandidateTypeId != null)
                            candidateProvider.IdTypeApplication = TypeApplications.Where(x => Int32.Parse(x.DefaultValue2) == provider.IntCandidateTypeId).First().IdKeyValue;

                        candidateProvider.PersonNameCorrespondence = provider.VcProviderContactPers;

                        if (provider.IntProviderContactPersEkatteId != null)
                            candidateProvider.IdLocationCorrespondence = dbLocations
                                .Where(x => x.LocationCode.Equals(locations
                                 .Where(m => m.Id == provider.IntProviderContactPersEkatteId)
                                 .First().VcTextCode))
                                 .First()
                                 .To<LocationVM>().idLocation;

                        candidateProvider.ZipCodeCorrespondence = provider.VcProviderContactPersZipcode;

                        if (provider.VcProviderContactPersAddress is not null)
                            candidateProvider.ProviderAddressCorrespondence = provider.VcProviderContactPersAddress.Replace("\\", "");


                        if (string.IsNullOrEmpty(provider.VcProviderContactPersPhone1))
                            candidateProvider.ProviderPhoneCorrespondence = "";
                        else if (!string.IsNullOrEmpty(provider.VcProviderContactPersPhone1) && !string.IsNullOrEmpty(provider.VcProviderContactPersPhone2))
                            candidateProvider.ProviderPhoneCorrespondence = $"{provider.VcProviderContactPersPhone1}; {provider.VcProviderContactPersPhone2}";
                        else
                            candidateProvider.ProviderPhoneCorrespondence = provider.VcProviderContactPersPhone1;

                        candidateProvider.ProviderFaxCorrespondence = provider.VcProviderContactPersFax;

                        candidateProvider.ProviderEmailCorrespondence = provider.VcProviderContactPersEmail;

                        if (provider.IntProviderOwnershipId != null)
                            candidateProvider.IdProviderOwnership = ownership
                                .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderOwnershipId)
                                .First()
                                .IdKeyValue;

                        if (provider.VcProviderManager != null)
                            candidateProvider.ManagerName = provider.VcProviderManager;

                        candidateProvider.LicenceDate = provider.DtLicenceData;

                        if (provider.IntProviderRegistrationId != null)
                            candidateProvider.IdProviderRegistration = registeredAt
                                .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderRegistrationId)
                                .First()
                                .IdKeyValue;

                        var procedure = new StartedProcedure();
                        if (provider.IntStartedProcedures != null)
                        {
                            procedure = startedProcedures
                           .Where(m => m.OldId == provider.IntStartedProcedures).FirstOrDefault();

                            var startedProcedure = tb_started_prcedures.Where(x => x.Id == provider.IntStartedProcedures).First();

                            candidateProvider.IdTypeApplication = TypeApplications
                            .Where(x => int.Parse(x.DefaultValue2) == startedProcedure.IntCandidateTypeId)
                            .First().IdKeyValue;

                            candidateProvider.IdStartedProcedure = procedure == null ? null : procedure.IdStartedProcedure;

                            candidateProvider.RejectionReason = startedProcedure.VcNegativeReasons;
                        }

                        var cp = cps.Where(x => x.OldId == provider.Id && x.IsActive == true).FirstOrDefault();

                        if (cp is not null)
                        {
                            candidateProvider.IdCandidateProviderActive = cp.IdCandidate_Provider;
                            candidateProvider.IsActive = false;
                        }
                        else
                        {
                            candidateProvider.IsActive = true;
                        }

                        if (!string.IsNullOrEmpty(provider.VcFilingSystemNumber))
                        {
                            candidateProvider.ApplicationNumber = provider.VcFilingSystemNumber;
                            candidateProvider.ApplicationDate = provider.DtFilingSystemDate;
                        }
                        else if (provider.IntStartedProcedures is not null)
                        {
                            var OldProcedure = tb_started_prcedures.Where(x => x.Id == procedure.OldId).First();

                            var document = tb_procedure_documents.Where(x => x.StartedProcedureId == OldProcedure.Id).FirstOrDefault();
                            if (document is not null)
                            {
                                candidateProvider.ApplicationNumber = document.DsOfficialNo;
                                candidateProvider.ApplicationDate = document.DsOfficialDate;
                            }
                        }

                        candidateProvider.ProviderOwnerEN = ConvertCyrToLatin(candidateProvider.ProviderOwner);
                        if (candidateProvider.ProviderName is not null)
                            candidateProvider.ProviderNameEN = ConvertCyrToLatin(candidateProvider.ProviderName);
                        if (candidateProvider.ProviderAddressCorrespondence is not null)
                            candidateProvider.ProviderAddressCorrespondenceEN = ConvertCyrToLatin(candidateProvider.ProviderAddressCorrespondence);
                        if (candidateProvider.PersonNameCorrespondence is not null)
                            candidateProvider.PersonNameCorrespondenceEN = ConvertCyrToLatin(candidateProvider.PersonNameCorrespondence);
                        if (candidateProvider.ProviderAddress is not null)
                            candidateProvider.ProviderAddressEN = ConvertCyrToLatin(candidateProvider.ProviderAddress);

                        candidateProvider.IdModifyUser = modifyUser.IdUser;
                        candidateProvider.IdCreateUser = modifyUser.IdUser;
                        candidateProvider.ModifyDate = DateTime.Now;
                        candidateProvider.CreationDate = DateTime.Now;
                        var dbCandidate = candidateProvider.To<CandidateProvider>();
                        dbCandidate.MigrationNote = "cp";
                        candidates.Add(dbCandidate);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportCandidateProviders(Първи Parallel.ForEach). Запис с Id = " + provider.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                }
            });

            _ApplicationDbContext.BulkInsert(candidates.ToList());

            var tb_candidate_providers_specialities = new List<TbCandidateProvidersSpeciality>();

            if (OldId is null)
                tb_candidate_providers_specialities = _jessieContextContext.TbCandidateProvidersSpecialities.ToList();
            else
                tb_candidate_providers_specialities = _jessieContextContext.TbCandidateProvidersSpecialities.Where(x => x.IntProviderId == OldId).ToList();

            var specialities = _ApplicationDbContext.Specialities.ToList();

            var candidateProviders = _ApplicationDbContext.CandidateProviders.Where(x => x.MigrationNote.Equals("cp")).ToList();

            ConcurrentBag<CandidateProviderSpeciality> candidateSpecialities = new ConcurrentBag<CandidateProviderSpeciality>();

            Parallel.ForEach(tb_candidate_providers_specialities, providerSpeciality =>
            {
                if (providerSpeciality.IntProviderId != null)
                {
                    try
                    {
                        var ps = new CandidateProviderSpecialityVM();
                        var candidate = candidateProviders.Where(x => x.OldId == providerSpeciality.IntProviderId).FirstOrDefault();
                        if (candidate is not null)
                        {
                            ps.IdCandidate_Provider = candidate.IdCandidate_Provider;

                            Speciality speciality = specialities.Where(x => x.OldId == providerSpeciality.IntVetSpecialityId).First();

                            ps.OldId = (int?)providerSpeciality.Id;

                            ps.IdSpeciality = speciality.IdSpeciality;

                            ps.IdCreateUser = modifyUser.IdUser;

                            ps.IdModifyUser = modifyUser.IdUser;

                            ps.ModifyDate = DateTime.Now;

                            ps.CreationDate = DateTime.Now;

                            CandidateProviderSpeciality candidateProviderSpeciality = ps.To<CandidateProviderSpeciality>();
                            candidateProviderSpeciality.Speciality = speciality;

                            candidateSpecialities.Add(candidateProviderSpeciality);

                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportCandidateProviders(Втори Parallel.ForEach). Запис с Id = " + providerSpeciality.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                }
            });

            _ApplicationDbContext.BulkInsert(candidateSpecialities.ToList());

            LogEndInformation("ImportCandidateProviders");
        }

        public void ImportProviders(int? OldId = null)
        {
            LogStrartInformation("ImportProviders");

            //OldId = 1;

            //tb_provideres
            var tb_providers = new List<TbProvider>();
            if (OldId is null)
                tb_providers = this._jessieContextContext.TbProviders.ToList();
            else
                tb_providers = this._jessieContextContext.TbProviders.Where(x => x.Id == OldId).ToList();

            //KeyValues 
            var ProviderStatuses = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "ProviderStatus"
                                    select kv).To<KeyValueVM>().ToList();

            var LicenseStatuses = (from kv in _ApplicationDbContext.KeyValues
                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                   where kt.KeyTypeIntCode == "LicenseStatus" && !string.IsNullOrEmpty(kv.DefaultValue2.Trim())
                                   select kv).To<KeyValueVM>().ToList();

            var ownership = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "ProviderOwnership"
                             select kv).To<KeyValueVM>().ToList();

            var registeredAt = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "ProviderRegisteredAt"
                                select kv).To<KeyValueVM>().ToList();

            var LicensingTypes = (from kv in _ApplicationDbContext.KeyValues
                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                  where kt.KeyTypeIntCode == "LicensingType"
                                  select kv).To<KeyValueVM>().ToList();

            var TypeApplications = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "TypeApplication"
                                    select kv).To<KeyValueVM>().ToList();


            var startedProcedures = _ApplicationDbContext.StartedProcedures.ToList();

            var tb_started_prcedures = _jessieContextContext.TbStartedProcedures.OrderByDescending(x => x.Ts).ToList();

            var dbLocations = _ApplicationDbContext.Locations.ToList();

            var tb_candidate_providers = this._jessieContextContext.TbCandidateProviders.ToList();

            var currentProviders = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive).ToList();

            this._ApplicationDbContext.ChangeTracker.Clear();

            var DocumentsFilledSubmittedToNAPOO = new List<int>() { 1, 6 };

            var RejectedApplicationLicensingNewCenter = new List<int>() { 3, 12 };

            var DocumentsFilledSubmittedToNAPOOKV = (from kv in _ApplicationDbContext.KeyValues
                                                     join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                     where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode.Equals("RequestedByCPOOrCIPO")
                                                     select kv).To<KeyValueVM>().First();

            var RejectedApplicationLicensingNewCenterKV = (from kv in _ApplicationDbContext.KeyValues
                                                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                           where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode.Equals("RejectedApplicationLicensingNewCenter")
                                                           select kv).To<KeyValueVM>().First();

            var ProcedureTerminatedByCenterKV = (from kv in _ApplicationDbContext.KeyValues
                                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                 where kt.KeyTypeIntCode == "ApplicationStatus" && kv.KeyValueIntCode.Equals("ProcedureTerminatedByCenter")
                                                 select kv).To<KeyValueVM>().First();

            ConcurrentBag<int> excludeProcedureIds = new ConcurrentBag<int>();
            ConcurrentBag<int> excludeProviderIds = new ConcurrentBag<int>();

            Parallel.ForEach(tb_candidate_providers, procedures =>
            {
                if (procedures.IntStartedProcedures != 0 && procedures.IntStartedProcedures != null)
                    excludeProcedureIds.Add((int)procedures.IntStartedProcedures);
            });

            List<int> excludeIds = excludeProcedureIds.ToList();
            ConcurrentBag<CandidateProvider> candidatesNew = new ConcurrentBag<CandidateProvider>();
            ConcurrentBag<CandidateProvider> candidatesUpdate = new ConcurrentBag<CandidateProvider>();

            Parallel.ForEach(tb_providers, provider =>
            {
                try
                {
                    if (provider.Id == 1)
                    {
                    }
                    var newRecord = false;

                    var candidateProvider = currentProviders.Where(x => x.OldId == provider.Id).FirstOrDefault();

                    if (candidateProvider is not null)
                    {
                        var inactiveCandidateProvider = candidateProvider.To<CandidateProviderVM>();
                        inactiveCandidateProvider.IdCandidateProviderActive = candidateProvider.IdCandidate_Provider;
                        var candidateOldId = candidateProvider.IdCandidate_Provider;
                        inactiveCandidateProvider.IdCandidate_Provider = 0;
                        inactiveCandidateProvider.IsActive = false;
                        inactiveCandidateProvider.MigrationNote = "ip";
                        candidatesNew.Add(inactiveCandidateProvider.To<CandidateProvider>());
                        candidateProvider = new CandidateProvider();
                        candidateProvider.IdCandidate_Provider = candidateOldId;
                        candidateProvider.MigrationNote = "tp";
                    }
                    else
                    {
                        newRecord = true;
                        candidateProvider = new CandidateProvider();
                        candidateProvider.MigrationNote = "np";
                    }

                    candidateProvider.IdStartedProcedure = null;
                    candidateProvider.IdApplicationStatus = null;
                    candidateProvider.IdTypeApplication = null;
                    candidateProvider.ApplicationNumber = null;
                    candidateProvider.ApplicationDate = null;
                    candidateProvider.IdReceiveLicense = null;

                    var provStatus = ProviderStatuses
                        .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderStatusId)
                        .First();

                    candidateProvider.IdProviderStatus = provStatus.IdKeyValue;

                    candidateProvider.IdTypeLicense = provStatus.KeyValueIntCode.Contains("CPO") ? LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCPO")).First().IdKeyValue : LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCIPO")).First().IdKeyValue;

                    candidateProvider.IdLicenceStatus = LicenseStatuses
                        .Where(x => int.Parse(x.DefaultValue2) == provider.IntLicenceStatusId)
                        .First()
                        .IdKeyValue;

                    candidateProvider.LicenceNumber = provider.IntLicenceNumber.ToString();

                    if (provider.VcProviderOwner != null)
                        candidateProvider.ProviderOwner = provider.VcProviderOwner;

                    if (provider.IntProviderBulstat != null)
                        candidateProvider.PoviderBulstat = provider.IntProviderBulstat;
                    else
                        candidateProvider.PoviderBulstat = "";

                    // int_local_group_id predpolagame che sa idta na useri ????????


                    if (provider.IntEkatteId != null && provider.IntEkatteId != 0)
                        candidateProvider.IdLocation = dbLocations
                            .Where(x => x.LocationCode.Equals(locations
                            .Where(m => m.Id == provider.IntEkatteId)
                            .First().VcTextCode))
                            .First()
                            .To<LocationVM>().idLocation;

                    if (!string.IsNullOrEmpty(provider.VcProviderProfile))
                    {
                        if (provider.VcProviderProfile.Length > 512)
                            candidateProvider.AdditionalInfo = provider.VcProviderProfile.Substring(0, 511);
                        else
                            candidateProvider.AdditionalInfo = provider.VcProviderProfile;
                    }
                    if (provider.VcZipCode != null)
                        candidateProvider.ZipCode = provider.VcZipCode;
                    else
                        candidateProvider.ZipCode = "";

                    if (provider.VcProviderAddress != null)
                        candidateProvider.ProviderAddress = provider.VcProviderAddress.Replace("\\", "");
                    else
                        candidateProvider.ProviderAddress = "";

                    if (string.IsNullOrEmpty(provider.VcProviderPhone1))
                        candidateProvider.ProviderPhone = "";
                    else if (!string.IsNullOrEmpty(provider.VcProviderPhone1) && !string.IsNullOrEmpty(provider.VcProviderPhone2) && !provider.VcProviderPhone1.Equals(provider.VcProviderPhone2))
                        candidateProvider.ProviderPhone = $"{provider.VcProviderPhone1}; {provider.VcProviderPhone2}";
                    else
                        candidateProvider.ProviderPhone = provider.VcProviderPhone1;

                    candidateProvider.ProviderFax = provider.VcProviderFax;

                    candidateProvider.ProviderWeb = provider.VcProviderWeb;

                    candidateProvider.ProviderEmail = provider.VcProviderEmail;
                    if (!string.IsNullOrEmpty(provider.VcProviderName) && !provider.VcProviderName.Equals("null"))
                        candidateProvider.ProviderName = provider.VcProviderName;
                    else
                        candidateProvider.ProviderName = null;

                    candidateProvider.PersonNameCorrespondence = provider.VcProviderContactPers;

                    candidateProvider.ZipCodeCorrespondence = provider.VcProviderContactPersZipcode;

                    candidateProvider.ProviderAddressCorrespondence = provider.VcProviderContactPersAddress;

                    if (provider.IntProviderContactPersEkatteId != null)
                        candidateProvider.IdLocationCorrespondence = dbLocations
                           .Where(x => x.LocationCode.Equals(locations
                                     .Where(m => m.Id == provider.IntProviderContactPersEkatteId)
                                     .First().VcTextCode))
                                     .First()
                                     .To<LocationVM>().idLocation;


                    if (string.IsNullOrEmpty(provider.VcProviderContactPersPhone1))
                        candidateProvider.ProviderPhoneCorrespondence = "";
                    else if (!string.IsNullOrEmpty(provider.VcProviderContactPersPhone1) && !string.IsNullOrEmpty(provider.VcProviderContactPersPhone2))
                        candidateProvider.ProviderPhoneCorrespondence = $"{provider.VcProviderContactPersPhone1}; {provider.VcProviderContactPersPhone2}";
                    else
                        candidateProvider.ProviderPhoneCorrespondence = provider.VcProviderContactPersPhone1;

                    candidateProvider.ProviderFaxCorrespondence = provider.VcProviderContactPersFax;

                    candidateProvider.ProviderEmailCorrespondence = provider.VcProviderContactPersEmail;

                    if (provider.IntProviderOwnershipId != null)
                        candidateProvider.IdProviderOwnership = ownership
                            .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderOwnershipId)
                            .First()
                            .IdKeyValue;

                    if (provider.VcProviderManager != null)
                        candidateProvider.ManagerName = provider.VcProviderManager;

                    candidateProvider.LicenceDate = provider.DtLicenceData;

                    if (provider.IntProviderRegistrationId != null)
                        candidateProvider.IdProviderRegistration = registeredAt
                            .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderRegistrationId)
                            .First()
                            .IdKeyValue;

                    candidateProvider.OldId = (int)provider.Id;

                    candidateProvider.IdModifyUser = modifyUser.IdUser;
                    candidateProvider.IdCreateUser = modifyUser.IdUser;
                    candidateProvider.ModifyDate = DateTime.Now;
                    candidateProvider.CreationDate = DateTime.Now;
                    candidateProvider.IsActive = true;

                    candidateProvider.ProviderOwnerEN = ConvertCyrToLatin(candidateProvider.ProviderOwner);
                    if (candidateProvider.ProviderName is not null)
                        candidateProvider.ProviderNameEN = ConvertCyrToLatin(candidateProvider.ProviderName);
                    if (candidateProvider.ProviderAddressCorrespondence is not null)
                        candidateProvider.ProviderAddressCorrespondenceEN = ConvertCyrToLatin(candidateProvider.ProviderAddressCorrespondence);
                    if (candidateProvider.PersonNameCorrespondence is not null)
                        candidateProvider.PersonNameCorrespondenceEN = ConvertCyrToLatin(candidateProvider.PersonNameCorrespondence);
                    if (candidateProvider.ProviderAddress is not null)
                        candidateProvider.ProviderAddressEN = ConvertCyrToLatin(candidateProvider.ProviderAddress);
                    if (!newRecord)
                    {
                        candidatesUpdate.Add(candidateProvider);
                    }
                    else
                    {
                        candidatesNew.Add(candidateProvider);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportProviders(Първи Parallel.ForEach). Запис с Id = " + provider.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.UpdateRange(candidatesUpdate.ToList());
            _ApplicationDbContext.SaveChanges();
            _ApplicationDbContext.BulkInsert(candidatesNew.ToList());

            var tb_provider_specialities = new List<TbProviderSpeciality>();
            if (OldId is null)
                tb_provider_specialities = _jessieContextContext.TbProviderSpecialities.ToList();
            else
                tb_provider_specialities = _jessieContextContext.TbProviderSpecialities.Where(x => x.IntProviderId == OldId).ToList();

            var specialities = _ApplicationDbContext.Specialities.To<SpecialityVM>().AsNoTracking().ToList();
            var candidatesFromDB = new List<CandidateProvider>();

            candidatesFromDB = _ApplicationDbContext.CandidateProviders.Where(x => x.IsActive).ToList();


            List<CandidateProviderSpeciality> specialitiesCandidates = new List<CandidateProviderSpeciality>();

            foreach (var providerSpeciality in tb_provider_specialities)
            {
                try
                {
                    var ps = new CandidateProviderSpecialityVM();
                    var candidate = candidatesFromDB.Where(x => x.OldId == providerSpeciality.IntProviderId).FirstOrDefault();
                    if (candidate is not null)
                    {
                        ps.IdCandidate_Provider = candidate.IdCandidate_Provider;

                        var speciality = specialities.Where(x => x.OldId == providerSpeciality.IntVetSpecialityId).First();

                        ps.IdSpeciality = speciality.IdSpeciality;

                        ps.OldId = (int?)providerSpeciality.Id;

                        ps.LicenceProtNo = $"№ {providerSpeciality.IntLicenceProtNo}";

                        ps.LicenceData = providerSpeciality.DtLicenceData;

                        ps.IdCreateUser = modifyUser.IdUser;

                        ps.IdModifyUser = modifyUser.IdUser;

                        ps.ModifyDate = DateTime.Now;

                        ps.CreationDate = DateTime.Now;

                        var db = ps.To<CandidateProviderSpeciality>();

                        specialitiesCandidates.Add(db);

                        if (!candidate.LicenceDate.HasValue || candidate.LicenceDate > providerSpeciality.DtLicenceData)
                        {
                            candidate.LicenceDate = providerSpeciality.DtLicenceData;
                            _ApplicationDbContext.Update(candidate);
                            _ApplicationDbContext.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportProviders(Първи foreach). Запис с Id = " + providerSpeciality.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            }
            _ApplicationDbContext.BulkInsert(specialitiesCandidates);

            if (OldId is null)
            {
                var candidatesAP = this._ApplicationDbContext.CandidateProviders.Where(x => x.MigrationNote.Equals("ap") && x.IsActive).ToList();

                var apToUpdate = new ConcurrentBag<CandidateProvider>();
                var apToSave = new ConcurrentBag<CandidateProvider>();

                Parallel.ForEach(candidatesAP, candidate =>
                {
                    var inactive = candidate.To<CandidateProviderVM>();

                    inactive.IsActive = false;

                    inactive.IdCandidate_Provider = 0;

                    inactive.IdCandidateProviderActive = candidate.IdCandidate_Provider;

                    inactive.MigrationNote = "apn";

                    candidate.IdStartedProcedure = null;
                    candidate.IdApplicationStatus = null;
                    candidate.IdTypeApplication = null;
                    candidate.ApplicationNumber = null;
                    candidate.ApplicationDate = null;
                    candidate.IdReceiveLicense = null;

                    candidate.IdStartedProcedure = null;
                    candidate.IdApplicationStatus = null;
                    candidate.IdTypeApplication = null;
                    candidate.ApplicationNumber = null;
                    candidate.ApplicationDate = null;
                    candidate.IdReceiveLicense = null;

                    var inactiveForDB = inactive.To<CandidateProvider>();

                    inactiveForDB.Location = null;
                    inactiveForDB.LocationCorrespondence = null;
                    inactiveForDB.RegionAdmin = null;
                    inactiveForDB.RegionCorrespondence = null;
                    inactiveForDB.CandidateProviderActive = null;
                    inactiveForDB.LocationCorrespondence = null;
                    inactiveForDB.CandidateProviderSpecialities = null;
                    inactiveForDB.CandidateProviderPersons = null;
                    inactiveForDB.CandidateProviderTrainers = null;
                    inactiveForDB.CandidateProviderStatuses = null;
                    inactiveForDB.CandidateProviderPremises = null;
                    inactiveForDB.CandidateProviderDocuments = null;
                    inactiveForDB.RequestReports = null;
                    inactiveForDB.ProviderDocumentOffers = null;
                    inactiveForDB.AnnualInfos = null;
                    inactiveForDB.Programs = null;
                    inactiveForDB.Courses = null;
                    inactiveForDB.CandidateProviderConsultings = null;
                    inactiveForDB.ValidationClients = null;
                    inactiveForDB.SelfAssessmentReports = null;
                    inactiveForDB.CandidateProviderCPOStructureAndActivities = null;
                    inactiveForDB.CandidateProviderCIPOStructureAndActivities = null;
                    inactiveForDB.ProviderRequestDocuments = null;
                    inactiveForDB.StartedProcedure = null;

                    apToSave.Add(inactiveForDB);
                    apToUpdate.Add(candidate);
                });

                this._ApplicationDbContext.BulkInsert(apToSave.ToList());
                this._ApplicationDbContext.BulkUpdate(apToUpdate.ToList());

                var candidatesSP = this._ApplicationDbContext.CandidateProviders.Where(x => x.MigrationNote.Equals("sp") && x.IsActive).ToList();

                var spToUpdate = new ConcurrentBag<CandidateProvider>();
                var spToSave = new ConcurrentBag<CandidateProvider>();

                Parallel.ForEach(candidatesSP, candidate =>
                {
                    var inactive = candidate.To<CandidateProviderVM>();

                    if (inactive.IdApplicationStatus != ProcedureTerminatedByCenterKV.IdKeyValue && inactive.IdApplicationStatus != RejectedApplicationLicensingNewCenterKV.IdKeyValue)
                    {
                        inactive.IdApplicationStatus = RejectedApplicationLicensingNewCenterKV.IdKeyValue;
                    }

                    inactive.IsActive = false;

                    inactive.IdCandidate_Provider = 0;

                    inactive.IdCandidateProviderActive = candidate.IdCandidate_Provider;

                    inactive.MigrationNote = "spn";

                    candidate.IdStartedProcedure = null;
                    candidate.IdApplicationStatus = null;
                    candidate.IdTypeApplication = null;
                    candidate.ApplicationNumber = null;
                    candidate.ApplicationDate = null;
                    candidate.IdReceiveLicense = null;

                    var inactiveForDB = inactive.To<CandidateProvider>();

                    inactiveForDB.Location = null;
                    inactiveForDB.LocationCorrespondence = null;
                    inactiveForDB.RegionAdmin = null;
                    inactiveForDB.RegionCorrespondence = null;
                    inactiveForDB.CandidateProviderActive = null;
                    inactiveForDB.LocationCorrespondence = null;
                    inactiveForDB.CandidateProviderSpecialities = null;
                    inactiveForDB.CandidateProviderPersons = null;
                    inactiveForDB.CandidateProviderTrainers = null;
                    inactiveForDB.CandidateProviderStatuses = null;
                    inactiveForDB.CandidateProviderPremises = null;
                    inactiveForDB.CandidateProviderDocuments = null;
                    inactiveForDB.RequestReports = null;
                    inactiveForDB.ProviderDocumentOffers = null;
                    inactiveForDB.AnnualInfos = null;
                    inactiveForDB.Programs = null;
                    inactiveForDB.Courses = null;
                    inactiveForDB.CandidateProviderConsultings = null;
                    inactiveForDB.ValidationClients = null;
                    inactiveForDB.SelfAssessmentReports = null;
                    inactiveForDB.CandidateProviderCPOStructureAndActivities = null;
                    inactiveForDB.CandidateProviderCIPOStructureAndActivities = null;
                    inactiveForDB.ProviderRequestDocuments = null;
                    inactiveForDB.StartedProcedure = null;

                    spToSave.Add(inactiveForDB);
                    spToUpdate.Add(candidate);
                });

                this._ApplicationDbContext.BulkInsert(spToSave.ToList());
                this._ApplicationDbContext.BulkUpdate(spToUpdate.ToList());
            }

            LogEndInformation("ImportProviders");
        }

        public void ImportProviderTrainerQualifications(int? OldId = null)
        {
            LogStrartInformation("ImportProviderTrainerQualifications");

            var QualificationTypes = (from kv in _ApplicationDbContext.KeyValues
                                      join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                      where kt.KeyTypeIntCode == "QualificationType"
                                      select kv).To<KeyValueVM>().ToList();

            var TrainingQualificationTypes = (from kv in _ApplicationDbContext.KeyValues
                                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                              where kt.KeyTypeIntCode == "TrainingQualificationType"
                                              select kv).To<KeyValueVM>().ToList();

            var tb_trainer_qualifications = new List<TbTrainerQualification>();

            if (OldId is null)
            {
                tb_trainer_qualifications = _jessieContextContext.TbTrainerQualifications.ToList();
            }
            else
            {
                var trainers = _jessieContextContext.TbTrainers.Where(x => x.IntProviderId == OldId).ToList();
                if (trainers.Count != 0)
                    tb_trainer_qualifications = _jessieContextContext.TbTrainerQualifications.ToList().Where(x => trainers.Any(z => z.Id == x.IntTrainerId.Value)).ToList();
            }

            var CandidateProviderTrainers = _ApplicationDbContext.CandidateProviderTrainers.Where(x => string.IsNullOrEmpty(x.MigrationNote)).ToList();

            var SPPOProfession = _ApplicationDbContext.Professions.ToList();

            ConcurrentBag<CandidateProviderTrainerQualification> qualifications = new ConcurrentBag<CandidateProviderTrainerQualification>();

            Parallel.ForEach(tb_trainer_qualifications, tq =>
            {
                try
                {
                    CandidateProviderTrainerQualificationVM trainer = new CandidateProviderTrainerQualificationVM();

                    if (tq.IntTrainerId != null && tq.IntTrainerId != 0)
                        trainer.IdCandidateProviderTrainer = CandidateProviderTrainers.Where(x => x.OldId == tq.IntTrainerId).First().IdCandidateProviderTrainer;

                    if (tq.TxtQualificationName != null)
                        trainer.QualificationName = tq.TxtQualificationName;
                    else
                        trainer.QualificationName = "";

                    if (tq.IntQualificationTypeId != null && tq.IntQualificationTypeId != 0)
                        trainer.IdQualificationType = QualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == tq.IntQualificationTypeId).First().IdKeyValue;

                    if (tq.IntProfessionId != null && tq.IntProfessionId != 0)
                    {
                        trainer.IdProfession = SPPOProfession.Where(x => x.OldId == tq.IntProfessionId).First().IdProfession;
                    }

                    if (tq.IntTqualificationTypeId != null && tq.IntTqualificationTypeId != 0)
                    {
                        trainer.IdTrainingQualificationType = TrainingQualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == tq.IntTqualificationTypeId).First().IdKeyValue;
                    }

                    trainer.QualificationDuration = tq.IntQualificationDuration;

                    trainer.TrainingFrom = tq.DtStartDate;

                    trainer.OldId = (int)tq.Id;

                    trainer.ModifyDate = DateTime.Now;

                    trainer.CreationDate = DateTime.Now;

                    trainer.IdCreateUser = modifyUser.IdUser;

                    trainer.IdModifyUser = modifyUser.IdUser;

                    var db = trainer.To<CandidateProviderTrainerQualification>();

                    db.Profession = null;

                    qualifications.Add(db);
                }
                catch (Exception ex)
                {

                    this.logger.LogInformation("Гръмна метод ImportProviderTrainerQualifications(Първи Parallel.ForEach). Запис с Id = " + tq.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(qualifications.ToList());
            LogEndInformation("ImportProviderTrainerQualifications");

        }

        public void ImportCandidateProviderTrainerQualifications(int? OldId = null)
        {
            LogStrartInformation("ImportCandidateProviderTrainerQualifications");

            var QualificationTypes = (from kv in _ApplicationDbContext.KeyValues
                                      join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                      where kt.KeyTypeIntCode == "QualificationType"
                                      select kv).To<KeyValueVM>().ToList();

            var TrainingQualificationTypes = (from kv in _ApplicationDbContext.KeyValues
                                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                              where kt.KeyTypeIntCode == "TrainingQualificationType"
                                              select kv).To<KeyValueVM>().ToList();
            var tb_trainer_qualifications = new List<TbCandidateTrainerQualification>();

            if (OldId is null)
            {
                tb_trainer_qualifications = _jessieContextContext.TbCandidateTrainerQualifications.ToList();
            }
            else
            {
                var trainers = _jessieContextContext.TbTrainers.Where(x => x.IntProviderId == OldId).ToList();
                if (trainers.Count != 0)
                    tb_trainer_qualifications = _jessieContextContext.TbCandidateTrainerQualifications.ToList().Where(x => trainers.Any(z => z.Id == x.IntTrainerId)).ToList();
            }

            var CandidateProviderTrainers = _ApplicationDbContext.CandidateProviderTrainers.Where(x => !string.IsNullOrEmpty(x.MigrationNote)).ToList();

            var SPPOProfession = _ApplicationDbContext.Professions.ToList();

            ConcurrentBag<CandidateProviderTrainerQualification> qualifications = new ConcurrentBag<CandidateProviderTrainerQualification>();

            Parallel.ForEach(tb_trainer_qualifications, tq =>
            {
                try
                {
                    CandidateProviderTrainerQualificationVM trainer = new CandidateProviderTrainerQualificationVM();

                    if (tq.IntTrainerId != null && tq.IntTrainerId != 0)
                        trainer.IdCandidateProviderTrainer = CandidateProviderTrainers.Where(x => x.OldId == tq.IntTrainerId).First().IdCandidateProviderTrainer;

                    if (tq.TxtQualificationName != null)
                        trainer.QualificationName = tq.TxtQualificationName;
                    else
                        trainer.QualificationName = "0";

                    if (tq.IntQualificationTypeId != null && tq.IntQualificationTypeId != 0)
                        trainer.IdQualificationType = QualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == tq.IntQualificationTypeId).First().IdKeyValue;

                    if (tq.IntProfessionId != null && tq.IntProfessionId != 0)
                    {
                        trainer.IdProfession = SPPOProfession.Where(x => x.OldId == tq.IntProfessionId).First().IdProfession;
                    }

                    if (tq.IntTqualificationTypeId != null && tq.IntTqualificationTypeId != 0)
                    {
                        trainer.IdTrainingQualificationType = TrainingQualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == tq.IntTqualificationTypeId).First().IdKeyValue;
                    }

                    trainer.QualificationDuration = tq.IntQualificationDuration;

                    trainer.TrainingFrom = tq.DtStartDate;

                    trainer.OldId = (int)tq.Id;

                    trainer.ModifyDate = DateTime.Now;

                    trainer.CreationDate = DateTime.Now;

                    trainer.IdCreateUser = modifyUser.IdUser;

                    trainer.IdModifyUser = modifyUser.IdUser;

                    trainer.MigrationNote = "cp";

                    var db = trainer.To<CandidateProviderTrainerQualification>();

                    db.Profession = null;

                    qualifications.Add(db);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportCandidateProviderTrainerQualifications(Първи Parallel.ForEach). Запис с Id = " + tq.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(qualifications.ToList());

            LogEndInformation("ImportCandidateProviderTrainerQualifications");

        }

        public void ImportProviderTrainerProfiles(int? OldId = null)
        {
            LogStrartInformation("ImportProviderTrainerProfiles");

            var ProfessionalDirection = _ApplicationDbContext.ProfessionalDirections.ToList();

            var candidateProviderTrainers = _ApplicationDbContext.CandidateProviderTrainers.Where(x => string.IsNullOrEmpty(x.MigrationNote)).ToList();

            var tb_trainer_profiles = new List<TbTrainerProfile>();

            if (OldId is null)
            {
                tb_trainer_profiles = _jessieContextContext.TbTrainerProfiles.ToList();

            }
            else
            {
                var trainers = _jessieContextContext.TbTrainers.Where(x => x.IntProviderId == OldId).ToList();
                if (trainers.Count != 0)
                    tb_trainer_profiles = _jessieContextContext.TbTrainerProfiles.ToList().Where(x => trainers.Any(z => x.IntTrainerId == z.Id)).ToList();
            }

            ConcurrentBag<CandidateProviderTrainerProfile> profiles = new ConcurrentBag<CandidateProviderTrainerProfile>();
            //Escape-ваме празните IntVetAreaId за задължителни в нашата таблица
            Parallel.ForEach(tb_trainer_profiles, tp =>
            {
                try
                {
                    if (tp.IntVetAreaId != null && tp.IntVetAreaId != 0)
                    {
                        CandidateProviderTrainerProfileVM trainer = new CandidateProviderTrainerProfileVM();

                        trainer.IdCandidateProviderTrainer = candidateProviderTrainers.Where(x => x.OldId == tp.IntTrainerId).First().IdCandidateProviderTrainer;

                        trainer.IdProfessionalDirection = ProfessionalDirection.Where(x => x.OldId == tp.IntVetAreaId).First().IdProfessionalDirection;
                        if (tp.BoolVetAreaTheory != null)
                            trainer.IsTheory = (bool)tp.BoolVetAreaTheory;
                        else
                            trainer.IsTheory = false;

                        if (tp.BoolVetAreaPractice != null)
                            trainer.IsPractice = (bool)tp.BoolVetAreaPractice;
                        else
                            trainer.IsPractice = false;

                        if (tp.BoolVetAreaQualified != null)
                            trainer.IsProfessionalDirectionQualified = (bool)tp.BoolVetAreaQualified;
                        else
                            trainer.IsProfessionalDirectionQualified = false;

                        trainer.OldId = (int)tp.Id;

                        trainer.ModifyDate = DateTime.Now;

                        trainer.CreationDate = DateTime.Now;

                        trainer.IdCreateUser = modifyUser.IdUser;

                        trainer.IdModifyUser = modifyUser.IdUser;

                        profiles.Add(trainer.To<CandidateProviderTrainerProfile>());
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportProviderTrainerProfiles(Първи Parallel.ForEach). Запис с Id = " + tp.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(profiles.ToList());

            LogEndInformation("ImportProviderTrainerProfiles");
        }

        public void ImportCandidateProviderTrainerProfiles(int? OldId = null)
        {
            LogStrartInformation("ImportCandidateProviderTrainerProfiles");

            var ProfessionalDirection = _ApplicationDbContext.ProfessionalDirections.ToList();

            var candidateProviderTrainers = _ApplicationDbContext.CandidateProviderTrainers.Where(x => !string.IsNullOrEmpty(x.MigrationNote)).ToList();

            var tb_candidate_trainer_profiles = new List<TbCandidateTrainerProfile>();

            if (OldId is null)
            {
                tb_candidate_trainer_profiles = _jessieContextContext.TbCandidateTrainerProfiles.ToList();
            }
            else
            {
                var trainers = _jessieContextContext.TbTrainers.Where(x => x.IntProviderId == OldId).ToList();
                if (trainers.Count != 0)
                    tb_candidate_trainer_profiles = _jessieContextContext.TbCandidateTrainerProfiles.ToList().Where(x => trainers.Any(z => z.Id == x.IntTrainerId)).ToList();
            }

            ConcurrentBag<CandidateProviderTrainerProfile> profiles = new ConcurrentBag<CandidateProviderTrainerProfile>();
            //Escape-ваме празните IntVetAreaId за задължителни в нашата таблица
            Parallel.ForEach(tb_candidate_trainer_profiles, tp =>
            {
                try
                {
                    if (tp.IntVetAreaId != null && tp.IntVetAreaId != 0)
                    {
                        CandidateProviderTrainerProfileVM trainer = new CandidateProviderTrainerProfileVM();

                        trainer.IdCandidateProviderTrainer = candidateProviderTrainers.Where(x => x.OldId == tp.IntTrainerId).First().IdCandidateProviderTrainer;

                        trainer.IdProfessionalDirection = ProfessionalDirection.Where(x => x.OldId == tp.IntVetAreaId).First().IdProfessionalDirection;
                        if (tp.BoolVetAreaTheory != null)
                            trainer.IsTheory = (bool)tp.BoolVetAreaTheory;
                        else
                            trainer.IsTheory = false;

                        if (tp.BoolVetAreaPractice != null)
                            trainer.IsPractice = (bool)tp.BoolVetAreaPractice;
                        else
                            trainer.IsPractice = false;

                        if (tp.BoolVetAreaQualified != null)
                            trainer.IsProfessionalDirectionQualified = (bool)tp.BoolVetAreaQualified;
                        else
                            trainer.IsProfessionalDirectionQualified = false;

                        trainer.OldId = (int)tp.Id;

                        trainer.ModifyDate = DateTime.Now;

                        trainer.CreationDate = DateTime.Now;

                        trainer.IdCreateUser = modifyUser.IdUser;

                        trainer.IdModifyUser = modifyUser.IdUser;

                        trainer.MigrationNote = "cp";

                        profiles.Add(trainer.To<CandidateProviderTrainerProfile>());
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportCandidateProviderTrainerProfiles(Първи Parallel.ForEach). Запис с Id = " + tp.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(profiles.ToList());

            LogEndInformation("ImportCandidateProviderTrainerProfiles");
        }

        public void ImportProviderTrainerDocument(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            LogStrartInformation("ImportProviderTrainerDocument");


            var tb_trainer_documents = new List<TbTrainerDocument>();

            if (OldId is null)
            {
                tb_trainer_documents = _jessieContextContext.TbTrainerDocuments.ToList();
            }
            else
            {
                var ctrainers = _jessieContextContext.TbTrainers.Where(x => x.IntProviderId == OldId).ToList();
                if (ctrainers.Count != 0)
                    tb_trainer_documents = _jessieContextContext.TbTrainerDocuments.ToList().Where(x => ctrainers.Any(z => x.IntTrainerId == z.Id)).ToList();
            }

            var trainers = _ApplicationDbContext.CandidateProviderTrainers.Where(x => string.IsNullOrEmpty(x.MigrationNote)).ToList();

            var DocType = (from kv in _ApplicationDbContext.KeyValues
                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                           where kt.KeyTypeIntCode == "TrainerDocumentType" && kv.KeyValueIntCode == "MigratedTrainerDocumentType"
                           select kv).To<KeyValueVM>().First().IdKeyValue;

            ConcurrentBag<CandidateProviderTrainerDocument> docs = new ConcurrentBag<CandidateProviderTrainerDocument>();

            if (!OnlyDocuments)
            {
                //var firstCP = this._ApplicationDbContext.CandidateProviders
                //    .Where(x => x.IdCandidate_Provider == 170701)
                //    .First();
                //var secondCP = this._ApplicationDbContext.CandidateProviders
                //    .Where(x => x.IdCandidate_Provider == 171226)
                //    .First();
                //var thirdCP = this._ApplicationDbContext.CandidateProviders
                //    .Where(x => x.IdCandidate_Provider == 170504)
                //    .First();

                //var trainerss = this._jessieContextContext.TbTrainers
                //    .Where(x => x.IntProviderId == firstCP.OldId || x.IntProviderId == secondCP.OldId || x.IntProviderId == thirdCP.OldId)
                //    .ToList();


                Parallel.ForEach(tb_trainer_documents, doc =>
                //foreach (var trainer in trainerss)
                {
                    try
                    {
                        //var docss = tb_trainer_documents.Where(x => x.IntTrainerId == trainer.Id).ToList();
                        //foreach (var doc in docss)
                        //{
                        CandidateProviderTrainerDocumentVM docVM = new CandidateProviderTrainerDocumentVM();

                        docVM.IdCandidateProviderTrainer = trainers.Where(x => x.OldId == doc.IntTrainerId).First().IdCandidateProviderTrainer;

                        docVM.DocumentTitle = doc.TxtDocumentsManagementTitle;

                        docVM.MigrationNote = doc.TxtDocumentsManagementFile.ToString();

                        docVM.IdDocumentType = DocType;

                        docVM.ModifyDate = DateTime.Now;

                        docVM.CreationDate = DateTime.Now;

                        docVM.IdCreateUser = modifyUser.IdUser;

                        docVM.IdModifyUser = modifyUser.IdUser;

                        docVM.OldId = (int?)doc.Id;

                        docs.Add(docVM.To<CandidateProviderTrainerDocument>());
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportProviderTrainerDocument(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                    //}
                    //}
                });

                _ApplicationDbContext.BulkInsert(docs.ToList());
            }

            LogEndInformation("ImportProviderTrainerDocument");

            if (MigrateDocuments)
            {
                //var trainerDocs = new List<CandidateProviderTrainerDocument>();

                //if (OldId is null)
                //    trainerDocs = this._ApplicationDbContext.CandidateProviderTrainerDocuments.ToList();
                //else
                //{
                //    var ctrainers = this._ApplicationDbContext.CandidateProviderTrainers.Include(x => x.CandidateProvider).Where(x =>
                //    x.CandidateProvider.OldId == OldId &&
                //    x.CandidateProvider.IsActive &&
                //   !x.CandidateProvider.MigrationNote.Equals("cp")).ToList();

                //    trainerDocs = this._ApplicationDbContext
                //        .CandidateProviderTrainerDocuments
                //        .ToList()
                //        .Where(x => ctrainers.Any(z => z.IdCandidateProviderTrainer == x.IdCandidateProviderTrainer))
                //        .ToList();
                //}

                //docs = new ConcurrentBag<CandidateProviderTrainerDocument>();

                //var exceptions = new ConcurrentQueue<string>();

                //foreach (var doc in trainerDocs)
                //{
                //    try
                //    {
                //        var url = $"\\UploadedFiles\\CandidateProviderTrainerDocument\\{doc.IdCandidateProviderTrainerDocument}\\";
                //        if (!string.IsNullOrEmpty(doc.MigrationNote))
                //        {
                //            SaveDocument(Int32.Parse(doc.MigrationNote), url);
                //            doc.UploadedFileName = url;

                //            docs.Add(doc);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        exceptions.Enqueue(doc.MigrationNote);
                //    }
                //};

                //foreach (var ex in exceptions)
                //{
                //    logger.LogError($"Error OID: {ex}");
                //}

                //this._ApplicationDbContext.BulkUpdate(docs.ToList());
                if (OldId is null)
                    CandidateProviderTrainerDocumentMigrateDocuments();
                else
                    CandidateProviderTrainerDocumentMigrateDocuments(OldId);
            }
        }

        public void ImportCandidateProviderTrainerDocument(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            LogStrartInformation("ImportCandidateProviderTrainerDocument");

            var DocType = (from kv in _ApplicationDbContext.KeyValues
                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                           where kt.KeyTypeIntCode == "TrainerDocumentType" && kv.KeyValueIntCode == "MigratedTrainerDocumentType"
                           select kv).To<KeyValueVM>().First().IdKeyValue;
            var tb_candidate_trainer_documents = new List<TbCandidateTrainerDocument>();

            if (OldId is null)
            {
                tb_candidate_trainer_documents = _jessieContextContext.TbCandidateTrainerDocuments.ToList();
            }
            else
            {
                var ctrainers = _jessieContextContext.TbTrainers.Where(x => x.IntProviderId == OldId).ToList();
                if (ctrainers.Count != 0)
                    tb_candidate_trainer_documents = _jessieContextContext.TbCandidateTrainerDocuments.ToList().Where(x => ctrainers.Any(z => x.IntTrainerId == z.Id)).ToList();
            }

            var trainers = _ApplicationDbContext.CandidateProviderTrainers.Where(x => !string.IsNullOrEmpty(x.MigrationNote)).ToList();

            ConcurrentBag<CandidateProviderTrainerDocument> docs = new ConcurrentBag<CandidateProviderTrainerDocument>();
            if (!OnlyDocuments)
            {
                Parallel.ForEach(tb_candidate_trainer_documents, doc =>
                {
                    try
                    {
                        CandidateProviderTrainerDocumentVM docVM = new CandidateProviderTrainerDocumentVM();

                        docVM.IdCandidateProviderTrainer = trainers.Where(x => x.OldId == doc.IntTrainerId).First().IdCandidateProviderTrainer;

                        docVM.DocumentTitle = doc.TxtDocumentsManagementTitle;

                        docVM.MigrationNote = doc.TxtDocumentsManagementFile.ToString();

                        docVM.IdDocumentType = DocType;

                        docVM.ModifyDate = DateTime.Now;

                        docVM.CreationDate = DateTime.Now;

                        docVM.IdCreateUser = modifyUser.IdUser;

                        docVM.OldId = (int?)doc.Id;

                        docVM.IdModifyUser = modifyUser.IdUser;

                        docs.Add(docVM.To<CandidateProviderTrainerDocument>());
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportCandidateProviderTrainerDocument(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });

                _ApplicationDbContext.BulkInsert(docs.ToList());
            }

            LogEndInformation("ImportCandidateProviderTrainerDocument");

            if (MigrateDocuments)
            {
                //var trainerDocs = new List<CandidateProviderTrainerDocument>();

                //if (OldId is null)
                //    trainerDocs = this._ApplicationDbContext.CandidateProviderTrainerDocuments.ToList();
                //else
                //{
                //    var ctrainers = this._ApplicationDbContext.CandidateProviderTrainers.Include(x => x.CandidateProvider).Where(x =>
                //    x.CandidateProvider.OldId == OldId &&
                //    x.CandidateProvider.IsActive &&
                //   x.CandidateProvider.MigrationNote.Equals("cp")).ToList();

                //    trainerDocs = this._ApplicationDbContext
                //        .CandidateProviderTrainerDocuments
                //        .ToList()
                //        .Where(x => ctrainers.Any(z => z.IdCandidateProviderTrainer == x.IdCandidateProviderTrainer))
                //        .ToList();
                //}

                //docs = new ConcurrentBag<CandidateProviderTrainerDocument>();

                //foreach (var doc in trainerDocs)
                //{
                //    var url = $"\\UploadedFiles\\CandidateProviderTrainerDocument\\{doc.IdCandidateProviderTrainerDocument}\\";
                //    if (!string.IsNullOrEmpty(doc.MigrationNote))
                //    {
                //        SaveDocument(Int32.Parse(doc.MigrationNote), url);
                //        doc.UploadedFileName = url;

                //        docs.Add(doc);
                //    }

                //};

                //this._ApplicationDbContext.BulkUpdate(docs.ToList());
                if (OldId is null)
                    CandidateProviderTrainerDocumentMigrateDocuments();
                else
                    CandidateProviderTrainerDocumentMigrateDocuments(OldId);
            }
        }

        public void ImportProviderDocuments(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {

            LogStrartInformation("ImportProviderDocuments");
            //var firstCP = this._ApplicationDbContext.CandidateProviders
            //    .Where(x => x.IdCandidate_Provider == 170701)
            //    .First();
            ////var secondCP = this._ApplicationDbContext.CandidateProviders
            ////    .Where(x => x.IdCandidate_Provider == 171226)
            ////    .First();
            //var thirdCP = this._ApplicationDbContext.CandidateProviders
            //    .Where(x => x.IdCandidate_Provider == 170504)
            //    .First();
            //var forthCP = this._ApplicationDbContext.CandidateProviders
            //                .Where(x => x.IdCandidate_Provider == 173376)
            //                .First();
            //var candidates = new List<CandidateProvider>() { firstCP, thirdCP, forthCP };

            var RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenter = new List<int>()
            {
                66, 610, 4, 47, 204, 19, 452, 40, 402
            };

            var FeePaidDocument = new List<int>()
            {
                53, 71, 22, 8, 28, 16
            };

            var ReportAdministrativeCenter = new List<int>()
            {
                409
            };

            var DocumentsForeignCompany = new List<int>()
            {
                410, 54, 72, 208, 428, 459

            };

            var RegulationsForTheOrganizationAndOperationOfCIPO = new List<int>()
            {
                66, 610, 4, 47, 204, 19, 452
            };

            var ConsultingDocumentation = new List<int>()
            {
                453
            };

            var TemplateFormuliarCPO = new List<int>()
            {
                401, 421, 491, 61, 41
            };

            var TemplateFormuliarCIPO = new List<int>()
            {
                451
            };

            var RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenterKV = (from kv in _ApplicationDbContext.KeyValues
                                                                                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                                                              where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenter"
                                                                                              select kv).To<KeyValueVM>().First().IdKeyValue;

            var FeePaidDocumentKV = (from kv in _ApplicationDbContext.KeyValues
                                     join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                     where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "FeePaidDocument"
                                     select kv).To<KeyValueVM>().First().IdKeyValue;
            var DocumentsForeignCompanyKV = (from kv in _ApplicationDbContext.KeyValues
                                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                             where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "DocumentsForeignCompany"
                                             select kv).To<KeyValueVM>().First().IdKeyValue;

            var RegulationsForTheOrganizationAndOperationOfCIPOKV = (from kv in _ApplicationDbContext.KeyValues
                                                                     join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                                     where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "RegulationsForTheOrganizationAndOperationOfCIPO"
                                                                     select kv).To<KeyValueVM>().First().IdKeyValue;

            var ConsultingDocumentationKV = (from kv in _ApplicationDbContext.KeyValues
                                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                             where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "ConsultingDocumentation"
                                             select kv).To<KeyValueVM>().First().IdKeyValue;

            var TemplateFormuliarCPOKV = (from kv in _ApplicationDbContext.KeyValues
                                          join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                          where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "TemplateFormularCPO"
                                          select kv).To<KeyValueVM>().First().IdKeyValue;

            var TemplateFormuliarCIPOKV = (from kv in _ApplicationDbContext.KeyValues
                                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                           where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "TemplateFormularCIPO"
                                           select kv).To<KeyValueVM>().First().IdKeyValue;


            var candidates = this._ApplicationDbContext.CandidateProviders.Where(x => !x.MigrationNote.Equals("cp") && x.IsActive).ToList();

            var DocType = (from kv in _ApplicationDbContext.KeyValues
                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                           where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "MigratedCandidateProviderDocumentType"
                           select kv).To<KeyValueVM>().First().IdKeyValue;

            var tb_providers_documents_management = new List<TbProvidersDocumentsManagement>();
            if (OldId is null)
                tb_providers_documents_management = this._jessieContextContext.TbProvidersDocumentsManagements.ToList();
            else
                tb_providers_documents_management = this._jessieContextContext.TbProvidersDocumentsManagements.Where(x => x.IntProviderId == OldId).ToList();

            if (!OnlyDocuments)
            {
                ConcurrentBag<CandidateProviderDocument> docs = new ConcurrentBag<CandidateProviderDocument>();

                Parallel.ForEach(tb_providers_documents_management, doc =>
                {
                    try
                    {
                        if (doc.TxtDocumentsManagementFile is not null && doc.IntProviderId is not null)
                        {
                            CandidateProviderDocument candidateProviderDocument = new CandidateProviderDocument();

                            candidateProviderDocument.IdCandidateProvider = candidates.Where(x => x.OldId == doc.IntProviderId).First().IdCandidate_Provider;

                            candidateProviderDocument.CreationDate = doc.TsDocument.Value;

                            candidateProviderDocument.DocumentTitle = doc.TxtDocumentsManagementTitle;


                            candidateProviderDocument.OldId = (int)doc.Id;

                            candidateProviderDocument.MigrationNote = doc.TxtDocumentsManagementFile.ToString();

                            if (doc.IntDocumentsManagementId is null)
                                candidateProviderDocument.IdDocumentType = DocType;
                            else
                            if (RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenter.Contains((int)doc.IntDocumentsManagementId))
                                candidateProviderDocument.IdDocumentType = RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenterKV;
                            else if (FeePaidDocument.Contains((int)doc.IntDocumentsManagementId))
                                candidateProviderDocument.IdDocumentType = FeePaidDocumentKV;
                            else if (DocumentsForeignCompany.Contains((int)doc.IntDocumentsManagementId))
                                candidateProviderDocument.IdDocumentType = DocumentsForeignCompanyKV;
                            else if (RegulationsForTheOrganizationAndOperationOfCIPO.Contains((int)doc.IntDocumentsManagementId))
                                candidateProviderDocument.IdDocumentType = RegulationsForTheOrganizationAndOperationOfCIPOKV;
                            else if (ConsultingDocumentation.Contains((int)doc.IntDocumentsManagementId))
                                candidateProviderDocument.IdDocumentType = ConsultingDocumentationKV;
                            else if (TemplateFormuliarCPO.Contains((int)doc.IntDocumentsManagementId))
                                candidateProviderDocument.IdDocumentType = TemplateFormuliarCPOKV;
                            else if (TemplateFormuliarCIPO.Contains((int)doc.IntDocumentsManagementId))
                                candidateProviderDocument.IdDocumentType = TemplateFormuliarCIPOKV;
                            else
                                candidateProviderDocument.IdDocumentType = DocType;

                            candidateProviderDocument.ModifyDate = DateTime.Now;

                            candidateProviderDocument.IdCreateUser = modifyUser.IdUser;

                            candidateProviderDocument.IdModifyUser = modifyUser.IdUser;

                            docs.Add(candidateProviderDocument);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportProviderDocuments(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });

                this._ApplicationDbContext.BulkInsert(docs.ToList());
            }

            LogEndInformation("ImportProviderDocuments");

            if (MigrateDocuments)
            {
                //var ProviderDocuments = new List<CandidateProviderDocument>();
                //if (OldId is null)
                //{
                //    ProviderDocuments = this._ApplicationDbContext.CandidateProviderDocuments.ToList();
                //}
                //else
                //{
                //    var providers = this._ApplicationDbContext.CandidateProviders.Where(x => x.OldId == OldId && x.IsActive && !x.MigrationNote.Equals("cp")).ToList();
                //    if (providers.Count != 0)
                //        ProviderDocuments = this._ApplicationDbContext.CandidateProviderDocuments.ToList().Where(x => providers.All(z => z.IdCandidate_Provider == x.IdCandidateProvider)).ToList();
                //}
                //ConcurrentBag<CandidateProviderDocument> docsUpdate = new ConcurrentBag<CandidateProviderDocument>();

                //int i = 1;

                //foreach (var doc in ProviderDocuments)
                //{
                //    try
                //    {
                //        var url = $"\\UploadedFiles\\CandidateProviderDocument\\{doc.IdCandidateProviderDocument}\\";

                //        var docFromNapoo = tb_providers_documents_management.Where(x => x.Id == doc.OldId).First();

                //        if (!string.IsNullOrEmpty(docFromNapoo.TxtDocumentsManagementFile.ToString()))
                //        {
                //            SaveDocument(int.Parse(docFromNapoo.TxtDocumentsManagementFile.ToString()), url);

                //            doc.UploadedFileName = $"{url}";
                //            docsUpdate.Add(doc);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        this.logger.LogInformation("Гръмна метод ImportProviderDocuments(Първи foreach). Запис с Id = " + doc.IdCandidateProviderDocument);
                //        this.logger.LogError(ex.Message);
                //        this.logger.LogError(ex.InnerException?.Message);
                //        this.logger.LogError(ex.StackTrace);
                //    }
                //};
                //this._ApplicationDbContext.BulkUpdate(docsUpdate.ToList());
                if (OldId is null)
                    CandidateProviderDocumentMigrateDocuments();
                else
                    CandidateProviderDocumentMigrateDocuments(OldId);
            }
        }

        public void ImportCandidateProviderDocuments(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            LogStrartInformation("ImportCandidateProviderDocuments");

            var RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenter = new List<int>()
            {
                66, 610, 4, 47, 204, 19, 452, 40
            };

            var FeePaidDocument = new List<int>()
            {
                53, 71, 22, 8, 28, 16
            };

            var ReportAdministrativeCenter = new List<int>()
            {
                409
            };

            var DocumentsForeignCompany = new List<int>()
            {
                410, 54, 72, 208, 428, 459

            };

            var RegulationsForTheOrganizationAndOperationOfCIPO = new List<int>()
            {
                66, 610, 4, 47, 204, 19, 452, 402
            };

            var ConsultingDocumentation = new List<int>()
            {
                453
            };

            var TemplateFormuliarCPO = new List<int>()
            {
                401, 421, 491, 61, 41
            };

            var TemplateFormuliarCIPO = new List<int>()
            {
                451
            };

            var RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenterKV = (from kv in _ApplicationDbContext.KeyValues
                                                                                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                                                              where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenter"
                                                                                              select kv).To<KeyValueVM>().First().IdKeyValue;

            var FeePaidDocumentKV = (from kv in _ApplicationDbContext.KeyValues
                                     join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                     where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "FeePaidDocument"
                                     select kv).To<KeyValueVM>().First().IdKeyValue;
            var DocumentsForeignCompanyKV = (from kv in _ApplicationDbContext.KeyValues
                                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                             where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "DocumentsForeignCompany"
                                             select kv).To<KeyValueVM>().First().IdKeyValue;

            var RegulationsForTheOrganizationAndOperationOfCIPOKV = (from kv in _ApplicationDbContext.KeyValues
                                                                     join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                                     where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "RegulationsForTheOrganizationAndOperationOfCIPO"
                                                                     select kv).To<KeyValueVM>().First().IdKeyValue;

            var ConsultingDocumentationKV = (from kv in _ApplicationDbContext.KeyValues
                                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                             where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "ConsultingDocumentation"
                                             select kv).To<KeyValueVM>().First().IdKeyValue;

            var TemplateFormuliarCPOKV = (from kv in _ApplicationDbContext.KeyValues
                                          join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                          where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "TemplateFormularCPO"
                                          select kv).To<KeyValueVM>().First().IdKeyValue;

            var TemplateFormuliarCIPOKV = (from kv in _ApplicationDbContext.KeyValues
                                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                           where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "TemplateFormularCIPO"
                                           select kv).To<KeyValueVM>().First().IdKeyValue;


            var candidates = this._ApplicationDbContext.CandidateProviders.Where(x => x.MigrationNote.Equals("cp")).ToList();

            var DocType = (from kv in _ApplicationDbContext.KeyValues
                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                           where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode == "MigratedCandidateProviderDocumentType"
                           select kv).To<KeyValueVM>().First().IdKeyValue;

            var tb_providers_documents_management = new List<TbCandidateProvidersDocumentsManagement>();
            if (OldId is null)
                tb_providers_documents_management = this._jessieContextContext.TbCandidateProvidersDocumentsManagements.ToList();
            else
                tb_providers_documents_management = this._jessieContextContext.TbCandidateProvidersDocumentsManagements.Where(x => x.IntProviderId == OldId).ToList();


            ConcurrentBag<CandidateProviderDocument> docs = new ConcurrentBag<CandidateProviderDocument>();
            if (!OnlyDocuments)
            {
                Parallel.ForEach(tb_providers_documents_management, doc =>
                {
                    try
                    {
                        if (doc.IntProviderId is not null)
                        {
                            CandidateProviderDocument candidateProviderDocument = new CandidateProviderDocument();

                            candidateProviderDocument.IdCandidateProvider = candidates.Where(x => x.OldId == doc.IntProviderId).First().IdCandidate_Provider;

                            candidateProviderDocument.CreationDate = doc.TsDocument.Value;

                            candidateProviderDocument.DocumentTitle = doc.TxtDocumentsManagementTitle;

                            candidateProviderDocument.OldId = (int)doc.Id;

                            candidateProviderDocument.MigrationNote = doc.TxtDocumentsManagementFile.ToString();
                            if (doc.IntDocumentsManagementId is not null)
                            {
                                if (RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenter.Contains((int)doc.IntDocumentsManagementId))
                                    candidateProviderDocument.IdDocumentType = RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenterKV;
                                else if (FeePaidDocument.Contains((int)doc.IntDocumentsManagementId))
                                    candidateProviderDocument.IdDocumentType = FeePaidDocumentKV;
                                else if (DocumentsForeignCompany.Contains((int)doc.IntDocumentsManagementId))
                                    candidateProviderDocument.IdDocumentType = DocumentsForeignCompanyKV;
                                else if (RegulationsForTheOrganizationAndOperationOfCIPO.Contains((int)doc.IntDocumentsManagementId))
                                    candidateProviderDocument.IdDocumentType = RegulationsForTheOrganizationAndOperationOfCIPOKV;
                                else if (ConsultingDocumentation.Contains((int)doc.IntDocumentsManagementId))
                                    candidateProviderDocument.IdDocumentType = ConsultingDocumentationKV;
                                else if (TemplateFormuliarCPO.Contains((int)doc.IntDocumentsManagementId))
                                    candidateProviderDocument.IdDocumentType = TemplateFormuliarCPOKV;
                                else if (TemplateFormuliarCIPO.Contains((int)doc.IntDocumentsManagementId))
                                    candidateProviderDocument.IdDocumentType = TemplateFormuliarCIPOKV;
                                else
                                    candidateProviderDocument.IdDocumentType = DocType;
                            }
                            else
                            {
                                candidateProviderDocument.IdDocumentType = DocType;
                            }

                            candidateProviderDocument.ModifyDate = DateTime.Now;


                            candidateProviderDocument.IdCreateUser = modifyUser.IdUser;

                            candidateProviderDocument.IdModifyUser = modifyUser.IdUser;

                            docs.Add(candidateProviderDocument);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportCandidateProviderDocuments(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });

                this._ApplicationDbContext.BulkInsert(docs.ToList());
            }

            LogEndInformation("ImportCandidateProviderDocuments");

            if (MigrateDocuments)
            {
                //var ProviderDocuments = new List<CandidateProviderDocument>();
                //if (OldId is null)
                //{
                //    ProviderDocuments = this._ApplicationDbContext.CandidateProviderDocuments.ToList();
                //}
                //else
                //{
                //    var providers = this._ApplicationDbContext.CandidateProviders.Where(x => x.OldId == OldId && x.MigrationNote.Equals("cp")).ToList();
                //    if (providers.Count != 0)
                //        ProviderDocuments = this._ApplicationDbContext.CandidateProviderDocuments.ToList().Where(x => providers.All(z => z.IdCandidate_Provider == x.IdCandidateProvider)).ToList();
                //}
                //ConcurrentBag<CandidateProviderDocument> docsUpdate = new ConcurrentBag<CandidateProviderDocument>();

                //int i = 1;

                //foreach (var doc in ProviderDocuments)
                //{
                //    try
                //    {
                //        var url = $"\\UploadedFiles\\CandidateProviderDocument\\{doc.IdCandidateProviderDocument}\\";

                //        var docFromNapoo = tb_providers_documents_management.Where(x => x.Id == doc.OldId).First();

                //        if (!string.IsNullOrEmpty(docFromNapoo.TxtDocumentsManagementFile.ToString()))
                //        {
                //            SaveDocument(int.Parse(docFromNapoo.TxtDocumentsManagementFile.ToString()), url);

                //            doc.UploadedFileName = $"{url}";
                //            docsUpdate.Add(doc);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        this.logger.LogInformation("Гръмна метод ImportCandidateProviderDocuments(Първи foreach). Запис с Id = " + doc.IdCandidateProviderDocument);
                //        this.logger.LogError(ex.Message);
                //        this.logger.LogError(ex.InnerException?.Message);
                //        this.logger.LogError(ex.StackTrace);
                //    }

                //};
                //this._ApplicationDbContext.BulkUpdate(docsUpdate.ToList());
                if (OldId is null)
                    CandidateProviderDocumentMigrateDocuments();
                else
                    CandidateProviderDocumentMigrateDocuments(OldId);
            }
        }

        public void ImportCurriculumModification(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            LogStrartInformation("ImportCurriculumModification");

            var tb_provider_specialities_curriculum = new List<TbProviderSpecialitiesCurriculum>();

            if (OldId is null)
            {
                tb_provider_specialities_curriculum = this._jessieContextContext.TbProviderSpecialitiesCurricula.Where(x => x.IntProviderSpecialityId != null).ToList();
            }
            else
            {
                var specialityProvider = this._jessieContextContext.TbProviderSpecialities.Where(x => x.IntProviderId == OldId).ToList();
                tb_provider_specialities_curriculum = this._jessieContextContext.TbProviderSpecialitiesCurricula.ToList().Where(x => specialityProvider.Any(z => z.Id == x.IntProviderSpecialityId)).ToList();
            }
            var candidateSpecialities = this._ApplicationDbContext.CandidateProviderSpecialities.ToList();

            var CandidateProviderDocumentTypes = (from kv in _ApplicationDbContext.KeyValues
                                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                  where kt.KeyTypeIntCode == "CurriculumModificationReasonType" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                                  select kv).To<KeyValueVM>().ToList();

            var FirstLicensingKV = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "CurriculumModificationReasonType" && kv.KeyValueIntCode == "FirstLicensing"
                                    select kv).To<KeyValueVM>().First();

            var FinalKV = (from kv in _ApplicationDbContext.KeyValues
                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                           where kt.KeyTypeIntCode == "CurriculumModificationStatusType" && kv.KeyValueIntCode == "Final"
                           select kv).To<KeyValueVM>().First();

            ConcurrentBag<CandidateCurriculumModification> currics = new ConcurrentBag<CandidateCurriculumModification>();
            if (!OnlyDocuments)
            {
                Parallel.ForEach(tb_provider_specialities_curriculum, curric =>
                {
                    try
                    {
                        CandidateCurriculumModification curriculumModification = new CandidateCurriculumModification();

                        curriculumModification.IdCandidateProviderSpeciality = candidateSpecialities
                        .Where(x => x.OldId == curric.IntProviderSpecialityId)
                        .First().IdCandidateProviderSpeciality;

                        if (curric.BoolIsUpdated.Value)
                        {
                            curriculumModification.ValidFromDate = curric.DtUpdateDate;

                            if (curric.IntSpecialityCurriculumUpdateReasonId is null)
                            {
                                curriculumModification.IdModificationReason = FirstLicensingKV.IdKeyValue;
                            }
                            else
                            {
                                curriculumModification.IdModificationReason = CandidateProviderDocumentTypes
                                .Where(x => Int32.Parse(x.DefaultValue2) == curric.IntSpecialityCurriculumUpdateReasonId)
                                .First().IdKeyValue;
                            }
                        }

                        curriculumModification.IdModificationStatus = FinalKV.IdKeyValue;

                        curriculumModification.ModifyDate = DateTime.Now;

                        curriculumModification.CreationDate = DateTime.Now;

                        curriculumModification.IdCreateUser = modifyUser.IdUser;

                        curriculumModification.IdModifyUser = modifyUser.IdUser;

                        curriculumModification.MigrationNote = curric.OidFile.ToString();

                        curriculumModification.OldId = (int)curric.Id;

                        currics.Add(curriculumModification);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportCurriculumModification(Първи Parallel.ForEach). Запис с Id = " + curric.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });

                _ApplicationDbContext.BulkInsert(currics.ToList());
            }

            LogEndInformation("ImportCurriculumModification");

            if (MigrateDocuments)
            {
                //var curriculumns = new List<CandidateCurriculumModification>();
                //if (OldId is null)
                //{
                //    curriculumns = this._ApplicationDbContext.CandidateCurriculumModification.ToList();
                //}
                //else
                //{
                //    curriculumns = this._ApplicationDbContext.CandidateCurriculumModification
                //        .Include(x => x.CandidateProviderSpeciality.CandidateProvider)
                //        .Where(x => x.CandidateProviderSpeciality.CandidateProvider.OldId == OldId).ToList();
                //}

                //List<CandidateCurriculumModification> curricsForUpdate = new List<CandidateCurriculumModification>();

                //foreach (var curr in curriculumns)
                //{
                //    try
                //    {
                //        var url = $"\\UploadedFiles\\CandidateCurriculumModification\\{curr.IdCandidateCurriculumModification}\\";

                //        var doc = tb_provider_specialities_curriculum.Where(x => x.Id == curr.OldId).First();

                //        if (doc.OidFile is not null)
                //        {
                //            url = $"{url}{SaveDocument((int)doc.OidFile, url)}";

                //            curr.UploadedFileName = url;
                //            curr.CandidateProviderSpeciality = null;
                //            curricsForUpdate.Add(curr);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        this.logger.LogInformation("Гръмна метод ImportCurriculumModification(Първи foreach). Запис с Id = " + curr.IdCandidateCurriculumModification);
                //        this.logger.LogError(ex.Message);
                //        this.logger.LogError(ex.InnerException?.Message);
                //        this.logger.LogError(ex.StackTrace);
                //    }
                //}
                //_ApplicationDbContext.BulkUpdate(curricsForUpdate);
                if (OldId is null)
                    CandidateCurriculumModificationMigrateDomuments();
                else
                    CandidateCurriculumModificationMigrateDomuments(OldId);
            }
        }

        public void ImportCandidateCurriculumModification(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            //var tb_candidate_provider_specialities_curriculum = this._jessieContextContext.TbCandidateProviderSpecialitiesCurricula.Where(x => x.IntCandidateProviderSpecialityId != null).ToList();
            LogStrartInformation("ImportCandidateCurriculumModification");

            var tb_candidate_provider_specialities_curriculum = new List<TbCandidateProviderSpecialitiesCurriculum>();

            if (OldId is null)
            {
                tb_candidate_provider_specialities_curriculum = this._jessieContextContext.TbCandidateProviderSpecialitiesCurricula.Where(x => x.IntCandidateProviderSpecialityId != null).ToList();
            }
            else
            {
                var specialityProvider = this._jessieContextContext.TbCandidateProvidersSpecialities.Where(x => x.IntProviderId == OldId).ToList();
                tb_candidate_provider_specialities_curriculum = this._jessieContextContext.TbCandidateProviderSpecialitiesCurricula.ToList().Where(x => specialityProvider.Any(z => z.Id == x.IntCandidateProviderSpecialityId)).ToList();
            }

            var candidateSpecialities = this._ApplicationDbContext.CandidateProviderSpecialities.ToList();

            var CandidateProviderDocumentTypes = (from kv in _ApplicationDbContext.KeyValues
                                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                  where kt.KeyTypeIntCode == "CurriculumModificationReasonType" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                                  select kv).To<KeyValueVM>().ToList();

            var FirstLicensingKV = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "CurriculumModificationReasonType" && kv.KeyValueIntCode == "FirstLicensing"
                                    select kv).To<KeyValueVM>().First();

            var FinalKV = (from kv in _ApplicationDbContext.KeyValues
                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                           where kt.KeyTypeIntCode == "CurriculumModificationStatusType" && kv.KeyValueIntCode == "Final"
                           select kv).To<KeyValueVM>().First();

            ConcurrentBag<CandidateCurriculumModification> currics = new ConcurrentBag<CandidateCurriculumModification>();
            if (!OnlyDocuments)
            {
                Parallel.ForEach(tb_candidate_provider_specialities_curriculum, curric =>
                {
                    try
                    {
                        CandidateCurriculumModification curriculumModification = new CandidateCurriculumModification();

                        curriculumModification.IdCandidateProviderSpeciality = candidateSpecialities
                        .Where(x => x.OldId == curric.IntCandidateProviderSpecialityId)
                        .First().IdCandidateProviderSpeciality;

                        if (curric.BoolIsUpdated.Value)
                        {
                            curriculumModification.ValidFromDate = curric.DtUpdateDate;

                            if (curric.IntSpecialityCurriculumUpdateReasonId is null)
                            {
                                curriculumModification.IdModificationReason = FirstLicensingKV.IdKeyValue;
                            }
                            else
                            {
                                curriculumModification.IdModificationReason = CandidateProviderDocumentTypes
                                .Where(x => Int32.Parse(x.DefaultValue2) == curric.IntSpecialityCurriculumUpdateReasonId)
                                .First().IdKeyValue;
                            }
                        }

                        curriculumModification.IdModificationStatus = FinalKV.IdKeyValue;

                        curriculumModification.ModifyDate = DateTime.Now;

                        curriculumModification.CreationDate = DateTime.Now;

                        curriculumModification.IdCreateUser = modifyUser.IdUser;

                        curriculumModification.IdModifyUser = modifyUser.IdUser;

                        curriculumModification.MigrationNote = curric.OidFile.ToString();

                        curriculumModification.OldId = (int?)curric.Id;

                        currics.Add(curriculumModification);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportCandidateCurriculumModification(Първи Parallel.ForEach). Запис с Id = " + curric.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }

                });

                _ApplicationDbContext.BulkInsert(currics.ToList());
            }

            LogEndInformation("ImportCandidateCurriculumModification");

            if (MigrateDocuments)
            {
                //var curriculumns = new List<CandidateCurriculumModification>();

                //if (OldId is null)
                //{
                //    curriculumns = this._ApplicationDbContext.CandidateCurriculumModification.ToList();
                //}
                //else
                //{
                //    curriculumns = this._ApplicationDbContext.CandidateCurriculumModification
                //        .Include(x => x.CandidateProviderSpeciality.CandidateProvider)
                //        .Where(x => x.CandidateProviderSpeciality.CandidateProvider.OldId == OldId).ToList();
                //}
                //List<CandidateCurriculumModification> curricsForUpdate = new List<CandidateCurriculumModification>();

                //foreach (var curr in curriculumns)
                //{
                //    try
                //    {
                //        var url = $"\\UploadedFiles\\CandidateCurriculumModification\\{curr.IdCandidateCurriculumModification}\\";

                //        var doc = tb_candidate_provider_specialities_curriculum.Where(x => x.Id == curr.OldId).First();

                //        if (doc.OidFile is not null)
                //        {
                //            url = $"{url}{SaveDocument((int)doc.OidFile, url)}";

                //            curr.UploadedFileName = url;
                //            curricsForUpdate.Add(curr);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        this.logger.LogInformation("Гръмна метод ImportCandidateCurriculumModification(Първи foreach). Запис с Id = " + curr.IdCandidateCurriculumModification);
                //        this.logger.LogError(ex.Message);
                //        this.logger.LogError(ex.InnerException?.Message);
                //        this.logger.LogError(ex.StackTrace);
                //    }
                //}

                //_ApplicationDbContext.BulkUpdate(curricsForUpdate);
                if (OldId is null)
                    CandidateCurriculumModificationMigrateDomuments();
                else
                    CandidateCurriculumModificationMigrateDomuments(OldId);
            }
        }

        public void ImportProviderLicenceChange(int? OldId = null)
        {
            LogStrartInformation("ImportProviderLicenceChange");

            var tb_providers_licence_change = new List<TbProvidersLicenceChange>();
            if (OldId is null)
                tb_providers_licence_change = this._jessieContextContext.TbProvidersLicenceChanges.ToList();
            else
                tb_providers_licence_change = this._jessieContextContext.TbProvidersLicenceChanges.Where(x => x.IntProviderId == OldId).ToList();

            ConcurrentBag<CandidateProviderLicenceChange> providerLicenceChanges = new ConcurrentBag<CandidateProviderLicenceChange>();

            var LicenseStatuses = (from kv in _ApplicationDbContext.KeyValues
                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                   where kt.KeyTypeIntCode == "LicenseStatus" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                   select kv).To<KeyValueVM>().ToList();

            var LicenceStatusDetails = (from kv in _ApplicationDbContext.KeyValues
                                        join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                        where kt.KeyTypeIntCode == "LicenceStatusDetail"
                                        select kv).To<KeyValueVM>().ToList();

            var candidates = this._ApplicationDbContext.CandidateProviders.Where(x => !x.MigrationNote!.Equals("cp")).ToList();

            Parallel.ForEach(tb_providers_licence_change, licenceChange =>
            {
                try
                {
                    var candidate = candidates.Where(x => x.OldId == licenceChange.IntProviderId).FirstOrDefault();

                    if (candidate is not null)
                    {
                        CandidateProviderLicenceChange licence = new CandidateProviderLicenceChange();

                        licence.IdStatus = LicenseStatuses
                        .Where(x => Int32.Parse(x.DefaultValue2!) == licenceChange.IntLicenceStatusId)
                        .First().IdKeyValue;

                        licence.IdCandidate_Provider = candidate.IdCandidate_Provider;

                        licence.ChangeDate = licenceChange.DtChangeDate!.Value;
                        if (licenceChange.IntLicenceStatusDetailsId is not null && licenceChange.IntLicenceStatusDetailsId != 0)
                            licence.IdLicenceStatusDetail = LicenceStatusDetails
                            .Where(x => Int32.Parse(x.DefaultValue2!) == licenceChange.IntLicenceStatusDetailsId)
                            .First().IdKeyValue;

                        licence.NumberCommand = licenceChange.VcNumberCommand;

                        licence.Notes = licenceChange.VcNotes;

                        licence.OldId = (int)licenceChange.Id;

                        licence.CreationDate = licenceChange.DtInsertDate!.Value;

                        licence.ModifyDate = DateTime.Now;

                        licence.IdModifyUser = this.modifyUser.IdUser;

                        licence.IdCreateUser = this.modifyUser.IdUser;

                        providerLicenceChanges.Add(licence);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportProviderLicenceChange(Първи Parallel.ForEach). Запис с Id = " + licenceChange.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            this._ApplicationDbContext.BulkInsert(providerLicenceChanges.ToList());
            LogEndInformation("ImportProviderLicenceChange");
        }

        public void ImportProviderProcedureDocuments(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            LogStrartInformation("ImportProviderProcedureDocuments");

            var tb_procedure_documents = new List<TbProcedureDocument>();

            if (OldId is null)
            {
                tb_procedure_documents = this._jessieContextContext.TbProcedureDocuments.ToList();
            }
            else
            {
                tb_procedure_documents = this._jessieContextContext.TbProcedureDocuments.Where(x => x.ProviderId == OldId).ToList();
            }

            var ProcedureDocumentTypes = (from kv in _ApplicationDbContext.KeyValues
                                          join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                          where kt.KeyTypeIntCode == "ProcedureDocumentType" && !string.IsNullOrEmpty(kv.DefaultValue5)
                                          select kv).To<KeyValueVM>().ToList();

            var OldIsKV = (from kv in _ApplicationDbContext.KeyValues
                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                           where kt.KeyTypeIntCode == "ProcedureDocumentType" && kv.KeyValueIntCode == "OldIS"
                           select kv).To<KeyValueVM>().First();

            var procedureDocuments = new ConcurrentBag<ProcedureDocument>();

            var startedProcedures = this._ApplicationDbContext.StartedProcedures.ToList();

            var experts = this._ApplicationDbContext.Experts.ToList();

            var code_stage_documents = this._jessieContextContext.CodeStageDocuments.ToList();

            Parallel.ForEach(tb_procedure_documents, doc =>
            {
                try
                {
                    if (doc.IsValid!.Value && doc.StartedProcedureId is not null)
                    {
                        var ProcedureDocument = new ProcedureDocument();

                        ProcedureDocument.IdStartedProcedure = startedProcedures.Where(x => x.OldId == doc.StartedProcedureId).First().IdStartedProcedure;

                        ProcedureDocument.IsValid = doc.IsValid!.Value;

                        ProcedureDocument.CreationDate = doc.Ts!.Value;

                        ProcedureDocument.ModifyDate = doc.Ts!.Value;

                        ProcedureDocument.MimeType = doc.MimeType;

                        ProcedureDocument.Extension = doc.Extension;

                        ProcedureDocument.DS_DATE = doc.DsDate;

                        ProcedureDocument.DS_ID = (int?)doc.DsId;

                        if(doc.Uin.HasValue)
                        ProcedureDocument.DS_DocNumber = doc.Uin.ToString();

                        if (!string.IsNullOrEmpty(doc.DsOfficialId) && !doc.DsOfficialId.ToLower().Equals("null"))
                            ProcedureDocument.DS_OFFICIAL_ID = Int32.Parse(doc.DsOfficialId);

                        if (!string.IsNullOrEmpty(doc.DsOfficialNo) && !doc.DsOfficialNo.ToLower().Equals("null"))
                            ProcedureDocument.DS_OFFICIAL_DocNumber = doc.DsOfficialNo;

                        if (!string.IsNullOrEmpty(doc.DsPrep) && !doc.DsPrep.ToLower().Equals("null"))
                            ProcedureDocument.DS_PREP = doc.DsPrep;

                        ProcedureDocument.DS_OFFICIAL_DATE = doc.DsOfficialDate;

                        if (doc.IntExpertId.HasValue)
                        {
                            var expert = experts.Where(x => x.OldId == doc.IntExpertId).FirstOrDefault();
                            if (expert is not null)
                                ProcedureDocument.IdExpert = expert.IdExpert;

                        }

                        ProcedureDocument.OldId = (int?)doc.Id;

                        if (doc.OidFile != null)
                            ProcedureDocument.MigrationNote = doc.OidFile.ToString();

                        if (doc.StageDocumentId is not null)
                        {
                            var oldKv = code_stage_documents.Where(x => x.Id == doc.StageDocumentId).First();

                            var kv = ProcedureDocumentTypes.Where(x => x.DefaultValue5.Split(',', StringSplitOptions.TrimEntries).Contains(oldKv.MnemCode)).FirstOrDefault();

                            if (kv is null)
                                ProcedureDocument.IdDocumentType = OldIsKV.IdKeyValue;
                            else
                                ProcedureDocument.IdDocumentType = kv.IdKeyValue;
                        }
                        else
                        {
                            ProcedureDocument.IdDocumentType = OldIsKV.IdKeyValue;
                        }
                        procedureDocuments.Add(ProcedureDocument);

                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportProviderProcedureDocuments(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(procedureDocuments.ToList());

            if (MigrateDocuments)
            {
                if (OldId is null)
                    RequestCandidateProviderDocumentMigrateDocuments();
                else
                    RequestCandidateProviderDocumentMigrateDocuments(OldId);
            }

            LogEndInformation("ImportProviderProcedureDocuments");

            if (MigrateDocuments)
            {
                //var currentProcedureDocuments = new List<ProcedureDocument>();

                //if (OldId is null)
                //{
                //    currentProcedureDocuments = this._ApplicationDbContext.ProcedureDocuments.ToList();
                //}
                //else
                //{
                //    currentProcedureDocuments = (from pd in this._ApplicationDbContext.ProcedureDocuments
                //                                 join sp in this._ApplicationDbContext.StartedProcedures on pd.IdStartedProcedure equals sp.IdStartedProcedure
                //                                 join cp in this._ApplicationDbContext.CandidateProviders on sp.IdCandidate_Provider equals cp.IdCandidate_Provider
                //                                 where cp.OldId == OldId
                //                                 select pd).ToList();
                //}
                //var docForUpdate = new List<ProcedureDocument>();
                //foreach (var doc in currentProcedureDocuments)
                //{
                //    try
                //    {
                //        var url = $"\\UploadedFiles\\ProviderProcedureDocument\\{doc.IdProcedureDocument}\\";


                //        if (doc.MigrationNote is not null)
                //        {
                //            url = $"{SaveDocument(Int32.Parse(doc.MigrationNote), url)}";

                //            doc.UploadedFileName = url;

                //            docForUpdate.Add(doc);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        this.logger.LogInformation("Гръмна метод ImportProviderProcedureDocuments(Първи foreach). Запис с Id = " + doc.IdProcedureDocument);
                //        this.logger.LogError(ex.Message);
                //        this.logger.LogError(ex.InnerException?.Message);
                //        this.logger.LogError(ex.StackTrace);
                //    }
                //}

                if (OldId is null)
                    CandidateProviderProcedureDocumentMigrateDocuments();
                else
                    CandidateProviderProcedureDocumentMigrateDocuments(OldId);
            }
        }

        public void UpdateCandidateProviderDocumentType()
        {
            LogStrartInformation("UpdateCandidateProviderDocumentType");

            var tb_providers_documents_management = this._jessieContextContext
                .TbProvidersDocumentsManagements
                .Where(x => x.IntDocumentsManagementId == 402).ToList();

            var WrongKeyValue = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode.Equals("RegulationsForTheOrganizationAndOperationOfCIPO")
                                 select kv).To<KeyValueVM>().First();

            var CorrectKeyValue = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "CandidateProviderDocumentType" && kv.KeyValueIntCode.Equals("RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenter")
                                 select kv).To<KeyValueVM>().First();

            var documentsToUpdate = this._ApplicationDbContext.CandidateProviderDocuments.Where(x => x.IdDocumentType == WrongKeyValue.IdKeyValue).ToList();

            var UpdatedDocuments = new ConcurrentBag<CandidateProviderDocument>();

            Parallel.ForEach(tb_providers_documents_management, doc =>
            {
                var updateDocument = documentsToUpdate.Where(x => x.OldId == doc.Id).First();

                updateDocument.IdDocumentType = CorrectKeyValue.IdKeyValue;

                UpdatedDocuments.Add(updateDocument);
            });

            this._ApplicationDbContext.BulkUpdate(UpdatedDocuments.ToList());

            LogEndInformation("UpdateCandidateProviderDocumentType");
        }

        #region Arch
        public void ImportArchProvider(int? OldId = null)
        {
            LogStrartInformation("ImportArchProvider");

            //tb_provideres
            var tb_providers = new List<ArchTbProvider>();
            if (OldId is null)
                tb_providers = this._jessieContextContext.ArchTbProviders.ToList();
            else
                tb_providers = this._jessieContextContext.ArchTbProviders.Where(x => x.Id == OldId).ToList();

            //KeyValues 
            var ProviderStatuses = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "ProviderStatus"
                                    select kv).To<KeyValueVM>().ToList();

            var LicenseStatuses = (from kv in _ApplicationDbContext.KeyValues
                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                   where kt.KeyTypeIntCode == "LicenseStatus" && !string.IsNullOrEmpty(kv.DefaultValue2.Trim())
                                   select kv).To<KeyValueVM>().ToList();

            var ownership = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "ProviderOwnership"
                             select kv).To<KeyValueVM>().ToList();

            var registeredAt = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "ProviderRegisteredAt"
                                select kv).To<KeyValueVM>().ToList();

            var LicensingTypes = (from kv in _ApplicationDbContext.KeyValues
                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                  where kt.KeyTypeIntCode == "LicensingType"
                                  select kv).To<KeyValueVM>().ToList();

            var TypeApplications = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "TypeApplication"
                                    select kv).To<KeyValueVM>().ToList();

            var dbLocations = _ApplicationDbContext.Locations.ToList();

            var providers = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive && !x.MigrationNote.Equals("cp")).ToList();

            ConcurrentBag<ArchCandidateProvider> candidatesNew = new ConcurrentBag<ArchCandidateProvider>();

            Parallel.ForEach(tb_providers, provider =>
            {
                try
                {

                    var candidateProvider = new ArchCandidateProvider();

                    candidateProvider.IdCandidate_Provider = providers.Where(x => x.OldId == provider.Id).First().IdCandidate_Provider;

                    candidateProvider.Year = (int)provider.IntYear;

                    candidateProvider.IdStartedProcedure = null;

                    var provStatus = ProviderStatuses
                        .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderStatusId)
                        .First();

                    candidateProvider.IdProviderStatus = provStatus.IdKeyValue;

                    candidateProvider.IdTypeLicense = provStatus.KeyValueIntCode.Contains("CPO") ? LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCPO")).First().IdKeyValue : LicensingTypes.Where(x => x.KeyValueIntCode.Equals("LicensingCIPO")).First().IdKeyValue;

                    if (provider.IntLicenceStatusId is not null)
                        candidateProvider.IdLicenceStatus = LicenseStatuses
                            .Where(x => int.Parse(x.DefaultValue2) == provider.IntLicenceStatusId)
                            .First()
                            .IdKeyValue;

                    candidateProvider.LicenceNumber = provider.IntLicenceNumber.ToString();

                    if (provider.VcProviderOwner != null)
                        candidateProvider.ProviderOwner = provider.VcProviderOwner;

                    if (provider.IntProviderBulstat != null)
                        candidateProvider.PoviderBulstat = provider.IntProviderBulstat;
                    else
                        candidateProvider.PoviderBulstat = "";

                    if (provider.IntEkatteId != null && provider.IntEkatteId != 0)
                        candidateProvider.IdLocation = dbLocations
                            .Where(x => x.LocationCode.Equals(locations
                            .Where(m => m.Id == provider.IntEkatteId)
                            .First().VcTextCode))
                            .First()
                            .To<LocationVM>().idLocation;

                    if (provider.VcZipCode != null)
                        candidateProvider.ZipCode = provider.VcZipCode;
                    else
                        candidateProvider.ZipCode = "";

                    if (provider.VcProviderAddress != null)
                        candidateProvider.ProviderAddress = provider.VcProviderAddress.Replace("\\", "");
                    else
                        candidateProvider.ProviderAddress = "";

                    if (string.IsNullOrEmpty(provider.VcProviderPhone1))
                        candidateProvider.ProviderPhone = "";
                    else if (!string.IsNullOrEmpty(provider.VcProviderPhone1) && !string.IsNullOrEmpty(provider.VcProviderPhone2))
                        candidateProvider.ProviderPhone = $"{provider.VcProviderPhone1}; {provider.VcProviderPhone2}";
                    else
                        candidateProvider.ProviderPhone = provider.VcProviderPhone1;

                    candidateProvider.ProviderFax = provider.VcProviderFax;

                    candidateProvider.ProviderWeb = provider.VcProviderWeb;

                    candidateProvider.ProviderEmail = provider.VcProviderEmail;

                    candidateProvider.ProviderName = provider.VcProviderName;

                    candidateProvider.PersonNameCorrespondence = provider.VcProviderContactPers;

                    candidateProvider.ZipCodeCorrespondence = provider.VcProviderContactPersZipcode;

                    candidateProvider.ProviderAddressCorrespondence = provider.VcProviderContactPersAddress;

                    if (!string.IsNullOrEmpty(provider.VcProviderOwner))
                        candidateProvider.ProviderOwnerEN = ConvertCyrToLatin(provider.VcProviderOwner);

                    if (provider.IntProviderContactPersEkatteId != null)
                        candidateProvider.IdLocationCorrespondence = dbLocations
                           .Where(x => x.LocationCode.Equals(locations
                                     .Where(m => m.Id == provider.IntProviderContactPersEkatteId)
                                     .First().VcTextCode))
                                     .First()
                                     .To<LocationVM>().idLocation;


                    if (string.IsNullOrEmpty(provider.VcProviderContactPersPhone1))
                        candidateProvider.ProviderPhoneCorrespondence = "";
                    else if (!string.IsNullOrEmpty(provider.VcProviderContactPersPhone1) && !string.IsNullOrEmpty(provider.VcProviderContactPersPhone2))
                        candidateProvider.ProviderPhoneCorrespondence = $"{provider.VcProviderContactPersPhone1}; {provider.VcProviderContactPersPhone2}";
                    else
                        candidateProvider.ProviderPhoneCorrespondence = provider.VcProviderContactPersPhone1;

                    candidateProvider.ProviderFaxCorrespondence = provider.VcProviderContactPersFax;

                    candidateProvider.ProviderEmailCorrespondence = provider.VcProviderContactPersEmail;

                    if (provider.IntProviderOwnershipId != null)
                        candidateProvider.IdProviderOwnership = ownership
                            .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderOwnershipId)
                            .First()
                            .IdKeyValue;

                    if (provider.VcProviderManager != null)
                        candidateProvider.ManagerName = provider.VcProviderManager;

                    candidateProvider.LicenceDate = provider.DtLicenceData;

                    if (provider.IntProviderRegistrationId != null)
                        candidateProvider.IdProviderRegistration = registeredAt
                            .Where(x => int.Parse(x.DefaultValue2) == provider.IntProviderRegistrationId)
                            .First()
                            .IdKeyValue;

                    candidateProvider.OldId = (int)provider.Id;

                    candidateProvider.IdModifyUser = modifyUser.IdUser;
                    candidateProvider.IdCreateUser = modifyUser.IdUser;
                    candidateProvider.ModifyDate = DateTime.Now;
                    candidateProvider.CreationDate = DateTime.Now;
                    candidateProvider.IsActive = true;

                    candidatesNew.Add(candidateProvider);

                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportArchProvider(Първи Parallel.ForEach). Запис с Id = " + provider.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(candidatesNew.ToList());

            var arch_tb_provider_specialities = new List<ArchTbProviderSpeciality>();

            if (OldId is null)
                arch_tb_provider_specialities = _jessieContextContext.ArchTbProviderSpecialities.ToList();
            else
                arch_tb_provider_specialities = _jessieContextContext.ArchTbProviderSpecialities.Where(x => x.IntProviderId == OldId).ToList();

            var specialities = _ApplicationDbContext.Specialities.ToList();

            var candidateProviders = _ApplicationDbContext.CandidateProviders.Where(x => !x.MigrationNote.Equals("cp") && x.IsActive).ToList();

            var archProviders = _ApplicationDbContext.ArchCandidateProviders.ToList();

            var candidateProviderSpecialities = _ApplicationDbContext.CandidateProviderSpecialities.ToList();

            ConcurrentBag<ArchCandidateProviderSpeciality> candidateSpecialities = new ConcurrentBag<ArchCandidateProviderSpeciality>();

            Parallel.ForEach(arch_tb_provider_specialities, providerSpeciality =>
            {
                if (providerSpeciality.IntProviderId != null)
                {
                    try
                    {
                        var ps = new ArchCandidateProviderSpeciality();
                        var candidate = candidateProviders.Where(x => x.OldId == providerSpeciality.IntProviderId).FirstOrDefault();
                        if (candidate is not null)
                        {
                            var archProvider = archProviders.Where(x => x.OldId == providerSpeciality.IntProviderId && x.Year == providerSpeciality.IntYear).First();

                            ps.IdCandidate_Provider = candidate.IdCandidate_Provider;

                            ps.IdArchCandidateProvider = archProvider.IdArchCandidateProvider;

                            Speciality speciality = specialities.Where(x => x.OldId == providerSpeciality.IntVetSpecialityId).First();

                            var candidateSpeciality = candidateProviderSpecialities
                            .Where(x => x.IdCandidate_Provider == candidate.IdCandidate_Provider && x.IdSpeciality == speciality.IdSpeciality)
                            .FirstOrDefault();

                            if (candidateSpeciality is not null)
                                ps.IdCandidateProviderSpeciality = candidateSpeciality.IdCandidateProviderSpeciality;

                            ps.LicenceProtNo = providerSpeciality.IntLicenceProtNo;

                            ps.LicenceData = providerSpeciality.DtLicenceData;

                            ps.OldId = (int?)providerSpeciality.Id;

                            ps.IdSpeciality = speciality.IdSpeciality;

                            ps.IdCreateUser = modifyUser.IdUser;

                            ps.IdModifyUser = modifyUser.IdUser;

                            ps.ModifyDate = DateTime.Now;

                            ps.CreationDate = DateTime.Now;

                            ArchCandidateProviderSpeciality candidateProviderSpeciality = ps;
                            candidateProviderSpeciality.Speciality = speciality;

                            candidateSpecialities.Add(candidateProviderSpeciality);

                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportArchProvider(Втори Parallel.ForEach). Запис с Id = " + providerSpeciality.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                }
            });

            _ApplicationDbContext.BulkInsert(candidateSpecialities.ToList());

            LogEndInformation("ImportArchProvider");
        }

        public void ImportArchPremises(int? OldId = null)
        {
            LogStrartInformation("ImportArchPremises");

            var activeStatus = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "MaterialTechnicalBaseStatus" && kv.KeyValueIntCode == "Active"
                                select kv).To<KeyValueVM>().First();

            var MaterialTechnicalBaseOwnerships = (from kv in _ApplicationDbContext.KeyValues
                                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                   where kt.KeyTypeIntCode == "MaterialTechnicalBaseOwnership"
                                                   select kv).To<KeyValueVM>().ToList();

            var arch_tb_provider_premises = new List<ArchTbProviderPremise>();

            if (OldId is null)
                arch_tb_provider_premises = _jessieContextContext.ArchTbProviderPremises.ToList();
            else
                arch_tb_provider_premises = _jessieContextContext.ArchTbProviderPremises.Where(x => x.IntProviderId == OldId).ToList();

            var candidates = _ApplicationDbContext.CandidateProviders.Where(x => x.IsActive && !x.MigrationNote.Equals("cp")).ToList();

            var archCandidates = this._ApplicationDbContext.ArchCandidateProviders.ToList();

            var currentPremises = this._ApplicationDbContext.CandidateProviderPremises.ToList();

            var location = _ApplicationDbContext.Locations.ToList();

            ConcurrentBag<ArchCandidateProviderPremises> candidateProviderPremises = new ConcurrentBag<ArchCandidateProviderPremises>();

            //foreach (var providerPremise in tb_provider_premises)
            Parallel.ForEach(arch_tb_provider_premises, providerPremise =>
            {
                try
                {
                    ArchCandidateProviderPremises premises = new ArchCandidateProviderPremises();

                    if (providerPremise.IntProviderId is not null)
                    {
                        var candidate = candidates.Where(x => x.OldId == providerPremise.IntProviderId).First();

                        var archCandidate = archCandidates.Where(x => x.IdCandidate_Provider == candidate.IdCandidate_Provider && x.Year == providerPremise.IntYear).First();

                        var existingPremise = currentPremises.Where(x => x.OldId == providerPremise.Id).FirstOrDefault();

                        if (existingPremise is not null)
                            premises.IdCandidateProviderPremises = existingPremise.IdCandidateProviderPremises;

                        premises.IdCandidate_Provider = candidate.IdCandidate_Provider;

                        premises.IdArchCandidateProvider = archCandidate.IdArchCandidateProvider;

                        premises.Phone = candidate.ProviderPhone;

                        premises.ZipCode = candidate.ZipCode;


                        premises.OldId = (int)providerPremise.Id;
                        //if (providerPremise.TxtProviderPremiseName != null)
                        //    premises.PremisesName = $"{providerPremise.IntProviderPremiseNo} {providerPremise.TxtProviderPremiseName.Trim()}";
                        //else
                        //    premises.PremisesName = $"{providerPremise.IntProviderPremiseNo} {providerPremise.TxtProviderPremiseName}";
                        if (providerPremise.TxtProviderPremiseName != null)
                            premises.PremisesName = providerPremise.TxtProviderPremiseName;
                        else
                            premises.PremisesName = "";

                        if (providerPremise.TxtProviderPremiseNotes != null)
                            premises.PremisesNote = providerPremise.TxtProviderPremiseNotes.Trim();
                        else
                            premises.PremisesNote = providerPremise.TxtProviderPremiseNotes;

                        if (providerPremise.IntProviderPremiseEkatte != null && providerPremise.IntProviderPremiseEkatte != 0)
                            premises.IdLocation = location
                                 .Where(x => x.LocationCode.Equals(locations
                                 .Where(m => m.Id == providerPremise.IntProviderPremiseEkatte)
                                 .First().VcTextCode))
                                 .First()
                                 .To<LocationVM>().idLocation;

                        if (providerPremise.TxtProviderPremiseAddress != null)
                            premises.ProviderAddress = providerPremise.TxtProviderPremiseAddress.Trim().Replace("\\", "");
                        else
                            premises.ProviderAddress = "";

                        if (providerPremise.IntProviderPremiseStatus != null)
                            premises.IdOwnership = MaterialTechnicalBaseOwnerships.Where(x => int.Parse(x.DefaultValue2) == providerPremise.IntProviderPremiseStatus).First().IdKeyValue;

                        premises.CreationDate = new DateTime((int)providerPremise.IntYear, 12, 31);

                        providerPremise.TxtProviderPremiseAddress = providerPremise.TxtProviderPremiseAddress;

                        premises.IdModifyUser = modifyUser.IdUser;

                        premises.IdCreateUser = modifyUser.IdUser;

                        premises.IdStatus = activeStatus.IdKeyValue;

                        candidateProviderPremises.Add(premises);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportArchPremises(Първи Parallel.ForEach). Запис с Id = " + providerPremise.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(candidateProviderPremises.ToList());

            //var premises = this._ApplicationDbContext.ArchCandidateProviderPremises.ToList();

            //ConcurrentBag<CandidateProviderPremisesChecking> checkings = new ConcurrentBag<CandidateProviderPremisesChecking>();
            //Parallel.ForEach(arch_tb_provider_premises, providerPremise =>
            //{
            //    try
            //    {
            //        CandidateProviderPremisesChecking checking = new CandidateProviderPremisesChecking();
            //        if (providerPremise.BoolIsVisited.HasValue && providerPremise.BoolIsVisited == true)
            //        {
            //            var premise = premises.Where(x => x.OldId == providerPremise.Id).First();

            //            checking.IdCandidateProviderPremises = premise.IdCandidateProviderPremises;

            //            checking.CheckDone = providerPremise.BoolIsVisited.Value;

            //            checking.Comment = "Базата е посетена от експерт на НАПОО. Информацията е прехвърлена от старата ИС на НАПОО към посочената дата.";

            //            checking.CheckingDate = DateTime.Now;

            //            checking.ModifyDate = DateTime.Now;

            //            checking.CreationDate = DateTime.Now;

            //            checking.IdModifyUser = modifyUser.IdUser;

            //            checking.IdCreateUser = modifyUser.IdUser;

            //            checkings.Add(checking);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        this.logger.LogInformation("Гръмна метод ImportArchPremises(Втори Parallel.ForEach). Запис с Id = " + providerPremise.Id);
            //        this.logger.LogError(ex.Message);
            //        this.logger.LogError(ex.InnerException?.Message);
            //        this.logger.LogError(ex.StackTrace);
            //    }
            //});

            //this._ApplicationDbContext.BulkInsert(checkings.ToList());
            //this._ApplicationDbContext.SaveChanges();

            LogEndInformation("ImportArchPremises");
        }

        public void ImportArchPremisesSpecialities(int? OldId = null)
        {
            LogStrartInformation("ImportArchPremisesSpecialities");

            var arch_ref_provider_premises_specialities = new List<ArchRefProviderPremisesSpeciality>();
            if (OldId is null)
            {
                arch_ref_provider_premises_specialities = this._jessieContextContext.ArchRefProviderPremisesSpecialities.ToList();
            }
            else
            {
                var tb_provider_premises = _jessieContextContext.TbProviderPremises.Where(x => x.IntProviderId == OldId).ToList();
                if (tb_provider_premises.Count != 0)
                    arch_ref_provider_premises_specialities = _jessieContextContext.ArchRefProviderPremisesSpecialities.ToList().Where(x => tb_provider_premises.Any(z => z.Id == x.IntProviderPremiseId)).ToList();

            }
            var specialities = this._ApplicationDbContext.CandidateProviderSpecialities.ToList();

            var premisses = this._ApplicationDbContext.ArchCandidateProviderPremises.Where(x => !x.MigrationNote.Equals("cp")).Include(x => x.ArchCandidateProvider).ToList();

            var archCandidate = this._ApplicationDbContext.ArchCandidateProviders.ToList();

            var archSpecialities = this._ApplicationDbContext.ArchCandidateProviderSpecialities.Include(x => x.ArchCandidateProvider).ToList();

            //TrainingType
            var trainingTypes = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "TrainingType"
                                 select kv).To<KeyValueVM>().ToList();

            var complianceDOC = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "ComplianceDOC"
                                 select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<ArchCandidateProviderPremisesSpeciality> premisesSpecialities = new ConcurrentBag<ArchCandidateProviderPremisesSpeciality>();

            Parallel.ForEach(arch_ref_provider_premises_specialities, premisie =>
            {
                try
                {
                    ArchCandidateProviderPremisesSpeciality providerPremisesSpeciality = new ArchCandidateProviderPremisesSpeciality();

                    var archPremises = premisses.Where(x => x.OldId == premisie.IntProviderPremiseId && x.ArchCandidateProvider.Year == premisie.IntYear).First();

                    var archSpeciality = archSpecialities.Where(x => x.OldId == premisie.IntProviderSpecialityId && x.ArchCandidateProvider.Year == premisie.IntYear).First();

                    providerPremisesSpeciality.IdArchCandidateProviderPremises = archPremises.IdArchCandidateProviderPremises;

                    providerPremisesSpeciality.IdCandidateProviderPremises = archPremises.IdCandidateProviderPremises;

                    providerPremisesSpeciality.IdCandidateProviderPremisesSpeciality = archSpeciality.IdArchCandidateProviderSpeciality;

                    providerPremisesSpeciality.IdSpeciality = archSpeciality.IdSpeciality;

                    if (premisie.IntProviderPremiseSpecialityUsage is not null)
                    {
                        providerPremisesSpeciality.IdUsage = trainingTypes.Where(x => Int32.Parse(x.DefaultValue2) == premisie.IntProviderPremiseSpecialityUsage).First().IdKeyValue;
                    }
                    if (premisie.IntProviderPremiseSpecialityCorrespondence is not null)
                    {
                        providerPremisesSpeciality.IdComplianceDOC = complianceDOC.Where(x => Int32.Parse(x.DefaultValue2) == premisie.IntProviderPremiseSpecialityCorrespondence).First().IdKeyValue;
                    }

                    var date = new DateTime((int)premisie.IntYear, 12, 31);

                    providerPremisesSpeciality.CreationDate = date;

                    providerPremisesSpeciality.ModifyDate = date;

                    providerPremisesSpeciality.IdModifyUser = modifyUser.IdUser;

                    providerPremisesSpeciality.IdCreateUser = modifyUser.IdUser;

                    providerPremisesSpeciality.OldId = (int?)premisie.Id;

                    premisesSpecialities.Add(providerPremisesSpeciality);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportArchPremisesSpecialities(Първи Parallel.ForEach). Запис с Id = " + premisie.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(premisesSpecialities.ToList());

            LogEndInformation("ImportArchPremisesSpecialities");
        }

        public void ImportArchTrainer(int? OldId = null)
        {
            LogStrartInformation("ImportArchTrainer");

            var arch_tb_trainers = new List<ArchTbTrainer>();

            if (OldId is null)
                arch_tb_trainers = _jessieContextContext.ArchTbTrainers.ToList();
            else
                arch_tb_trainers = _jessieContextContext.ArchTbTrainers.Where(x => x.IntProviderId == OldId).ToList();

            var CandidateProviders = _ApplicationDbContext.CandidateProviders.Where(x => !x.MigrationNote.Equals("cp") && x.IsActive).ToList();

            var ArchCandidates = _ApplicationDbContext.ArchCandidateProviders.ToList();

            var providerTrainers = this._ApplicationDbContext.CandidateProviderTrainers.Where(x => !x.MigrationNote.Equals("cp")).ToList();

            var activeStatus = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "CandidateProviderTrainerStatus" && kv.KeyValueIntCode == "Active"
                                select kv).To<KeyValueVM>().First().IdKeyValue;

            var indentTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "IndentType"
                               select kv).To<KeyValueVM>().ToList();

            var sex = (from kv in _ApplicationDbContext.KeyValues
                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                       where kt.KeyTypeIntCode == "Sex"
                       select kv).To<KeyValueVM>().ToList();

            var nationality = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "Nationality"
                               select kv).To<KeyValueVM>().ToList();

            var education = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "Education" && !string.IsNullOrEmpty(kv.DefaultValue2)
                             select kv).To<KeyValueVM>().ToList();

            var contactTypes = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "TrainerContractType"
                                select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<ArchCandidateProviderTrainer> trainers = new ConcurrentBag<ArchCandidateProviderTrainer>();

            //foreach (var tb in tb_trainers)
            Parallel.ForEach(arch_tb_trainers, tb =>
            {
                try
                {
                    ArchCandidateProviderTrainer trainer = new ArchCandidateProviderTrainer();

                    var archProvider = ArchCandidates.Where(x => x.OldId == tb.IntProviderId && x.Year == tb.IntYear).First();

                    trainer.IdCandidate_Provider = archProvider.IdCandidate_Provider;

                    trainer.IdArchCandidateProvider = archProvider.IdArchCandidateProvider;

                    var providerTrainer = providerTrainers.Where(x => x.OldId == tb.Id).FirstOrDefault();

                    if (providerTrainer is not null)
                    {
                        trainer.IdCandidateProviderTrainer = providerTrainer.IdCandidateProviderTrainer;
                    }

                    if (tb.IntProviderId != null && tb.IntProviderId != 0)
                        trainer.IdCandidate_Provider = CandidateProviders.Where(x => x.OldId == tb.IntProviderId && x.IsActive).First().IdCandidate_Provider;

                    if (tb.VcTrainerFirstName != null)
                        trainer.FirstName = tb.VcTrainerFirstName;
                    else
                        trainer.FirstName = "";

                    trainer.SecondName = tb.VcTrainerSecondName;

                    if (tb.VcTrainerFamilyName != null)
                        trainer.FamilyName = tb.VcTrainerFamilyName;
                    else
                        trainer.FamilyName = "";

                    if (tb.IntEgnTypeId != null)
                    {
                        var indentType = indentTypes.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntEgnTypeId).First();
                        trainer.IdIndentType = indentType.IdKeyValue;

                        try
                        {
                            if (indentType.KeyValueIntCode.Equals("EGN") && tb.VcEgn != null)
                                trainer.BirthDate = GetBirthDate(tb.VcEgn);
                        }
                        catch (Exception e) { }
                    }
                    if (tb.VcEgn != null && tb.VcEgn.Length <= 10)
                        trainer.Indent = tb.VcEgn;
                    else if (tb.VcEgn == null)
                        trainer.Indent = null;
                    else
                        trainer.Indent = "long";

                    if (tb.IntGenderId != null && tb.IntGenderId != 0)
                        trainer.IdSex = sex.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntGenderId).First().IdKeyValue;

                    if (tb.IntNationalityId != null && tb.IntNationalityId != 0)
                        trainer.IdNationality = nationality.Where(x => x.Order == tb.IntNationalityId).First().IdKeyValue;

                    trainer.Email = tb.VcEmail;

                    trainer.ContractDate = tb.DtTcontractDate;

                    trainer.IdStatus = activeStatus;

                    if (tb.IntEducationId != null && tb.IntEducationId != 0)
                    {
                        if (tb.IntEducationId == 24) tb.IntEducationId = 30;
                        trainer.IdEducation = education.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntEducationId).First().IdKeyValue;
                    }
                    trainer.EducationSpecialityNotes = tb.TxtEducationSpecialityNotes;

                    trainer.EducationCertificateNotes = tb.TxtEducationCertificateNotes;

                    trainer.EducationAcademicNotes = tb.TxtEducationAcademicNotes;

                    if (tb.IntTcontractTypeId != null && tb.IntTcontractTypeId != 0)
                        trainer.IdContractType = contactTypes.Where(x => Int32.Parse(x.DefaultValue2) == tb.IntTcontractTypeId).First().IdKeyValue;

                    if (tb.BoolIsAndragog != null)
                        trainer.IsAndragog = (bool)tb.BoolIsAndragog;

                    trainer.OldId = (int)tb.Id;

                    trainer.ModifyDate = DateTime.Now;

                    trainer.CreationDate = DateTime.Now;

                    trainer.IdCreateUser = modifyUser.IdUser;

                    trainer.IdModifyUser = modifyUser.IdUser;

                    trainers.Add(trainer);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportArchTrainer(Първи Parallel.ForEach). Запис с Id = " + tb.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(trainers.ToList());
            LogEndInformation("ImportArchTrainer");
        }

        public void ImportArchProviderTrainerQualifications(int? OldId = null)
        {
            LogStrartInformation("ImportArchProviderTrainerQualifications");

            var QualificationTypes = (from kv in _ApplicationDbContext.KeyValues
                                      join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                      where kt.KeyTypeIntCode == "QualificationType"
                                      select kv).To<KeyValueVM>().ToList();

            var TrainingQualificationTypes = (from kv in _ApplicationDbContext.KeyValues
                                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                              where kt.KeyTypeIntCode == "TrainingQualificationType"
                                              select kv).To<KeyValueVM>().ToList();

            var arch_tb_trainer_qualifications = new List<ArchTbTrainerQualification>();

            if (OldId is null)
            {
                arch_tb_trainer_qualifications = _jessieContextContext.ArchTbTrainerQualifications.ToList();
            }
            else
            {
                var trainers = _jessieContextContext.TbTrainers.Where(x => x.IntProviderId == OldId).ToList();
                var archTrainers = _jessieContextContext.ArchTbTrainers.ToList().Where(x => trainers.Any(z => z.Id == x.Id)).ToList();
                if (archTrainers.Count != 0)
                    arch_tb_trainer_qualifications = _jessieContextContext.ArchTbTrainerQualifications.ToList().Where(x => trainers.Any(z => z.Id == x.IntTrainerId.Value)).ToList();
            }

            var ProviderQualification = this._ApplicationDbContext.CandidateProviderTrainerQualifications.Where(x => string.IsNullOrEmpty(x.MigrationNote)).ToList();

            var ArchTrainers = this._ApplicationDbContext.ArchCandidateProviderTrainers.Include(x => x.ArchCandidateProvider).ToList();

            var CandidateProviderTrainers = _ApplicationDbContext.CandidateProviderTrainers.Where(x => string.IsNullOrEmpty(x.MigrationNote)).ToList();

            var SPPOProfession = _ApplicationDbContext.Professions.ToList();

            ConcurrentBag<ArchCandidateProviderTrainerQualification> qualifications = new ConcurrentBag<ArchCandidateProviderTrainerQualification>();

            Parallel.ForEach(arch_tb_trainer_qualifications, tq =>
            {
                try
                {
                    ArchCandidateProviderTrainerQualification trainer = new ArchCandidateProviderTrainerQualification();

                    var archTrainer = ArchTrainers.Where(x => x.OldId == tq.IntTrainerId && x.ArchCandidateProvider.Year == tq.IntYear).First();

                    trainer.IdArchCandidateProviderTrainer = archTrainer.IdArchCandidateProviderTrainer;

                    trainer.IdCandidateProviderTrainer = archTrainer.IdCandidateProviderTrainer;

                    var qualification = ProviderQualification.Where(x => x.OldId == tq.Id).FirstOrDefault();

                    if (qualification is not null)
                        trainer.IdCandidateProviderTrainerQualification = qualification.IdCandidateProviderTrainerQualification;

                    if (tq.TxtQualificationName != null)
                        trainer.QualificationName = tq.TxtQualificationName;
                    else
                        trainer.QualificationName = "0";

                    if (tq.IntQualificationTypeId != null && tq.IntQualificationTypeId != 0)
                        trainer.IdQualificationType = QualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == tq.IntQualificationTypeId).First().IdKeyValue;

                    if (tq.IntProfessionId != null && tq.IntProfessionId != 0)
                    {
                        trainer.IdProfession = SPPOProfession.Where(x => x.OldId == tq.IntProfessionId).First().IdProfession;
                    }

                    if (tq.IntTqualificationTypeId != null && tq.IntTqualificationTypeId != 0)
                    {
                        trainer.IdTrainingQualificationType = TrainingQualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == tq.IntTqualificationTypeId).First().IdKeyValue;
                    }

                    trainer.QualificationDuration = tq.IntQualificationDuration;

                    trainer.TrainingFrom = tq.DtStartDate;

                    trainer.OldId = (int)tq.Id;

                    trainer.ModifyDate = DateTime.Now;

                    trainer.CreationDate = DateTime.Now;

                    trainer.IdCreateUser = modifyUser.IdUser;

                    trainer.IdModifyUser = modifyUser.IdUser;

                    trainer.Profession = null;

                    qualifications.Add(trainer);
                }
                catch (Exception ex)
                {

                    this.logger.LogInformation("Гръмна метод ImportArchProviderTrainerQualifications(Първи Parallel.ForEach). Запис с Id = " + tq.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(qualifications.ToList());
            LogEndInformation("ImportArchProviderTrainerQualifications");

        }
        #endregion

        #endregion

        #region Expert
        public void ImportExperts()
        {
            LogStrartInformation("ImportExperts");

            var tb_experts = _jessieContextContext.TbExperts.ToList();

            var ref_experts_types = _jessieContextContext.RefExpertsTypes.ToList();

            var ref_experts_vet_area = _jessieContextContext.RefExpertsVetAreas.ToList();

            var ref_experts_commissions = _jessieContextContext.RefExpertsCommissions.ToList();

            var code_commission_institution_type = _jessieContextContext.CodeCommissionInstitutionTypes.ToList();

            var ref_experts_doi = this._jessieContextContext.RefExpertsDois.ToList().OrderByDescending(x => x.Id).ToList();

            var tb_doi_commissions = this._jessieContextContext.TbDoiCommissions.ToList();

            var Locations = _ApplicationDbContext.Locations.ToList();

            var ProfessionalDirection = _ApplicationDbContext.ProfessionalDirections.ToList();

            var DOC = this._ApplicationDbContext.DOCs.Where(x => x.OldId != null).ToList();

            //NOTMAPPED -> txt_expert_cv_file
            var sex = (from kv in _ApplicationDbContext.KeyValues
                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                       where kt.KeyTypeIntCode == "Sex"
                       select kv).To<KeyValueVM>().ToList();

            var ActiveExpert = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "ExpertStatus" && kv.KeyValueIntCode == "ActiveExpert"
                                select kv).To<KeyValueVM>().First();

            var ExternalExpert = (from kv in _ApplicationDbContext.KeyValues
                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                  where kt.KeyTypeIntCode == "ExpertType" && kv.KeyValueIntCode == "ExternalExpert"
                                  select kv).To<KeyValueVM>().First();

            var indentTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "IndentType"
                               select kv).To<KeyValueVM>().ToList();

            var ExpertRoleCommissions = (from kv in _ApplicationDbContext.KeyValues
                                         join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                         where kt.KeyTypeIntCode == "ExpertRoleCommission"
                                         select kv).To<KeyValueVM>().ToList();

            var ExpertCommissions = (from kv in _ApplicationDbContext.KeyValues
                                     join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                     where kt.KeyTypeIntCode == "ExpertCommission"
                                     select kv).To<KeyValueVM>().ToList();

            var CandidateProviderTrainerStatuses = (from kv in _ApplicationDbContext.KeyValues
                                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                    where kt.KeyTypeIntCode == "CandidateProviderTrainerStatus"
                                                    select kv).To<KeyValueVM>().ToList();

            foreach (var ex in tb_experts)
            {
                ExpertVM expert = new ExpertVM();
                expert.Person = new PersonVM();

                //Creating person
                if (ex.VcExpertFirstName != null)
                    expert.Person.FirstName = ex.VcExpertFirstName;
                else
                    expert.Person.FirstName = "";

                if (ex.VcExpertSecondName != null)
                    expert.Person.SecondName = ex.VcExpertSecondName;
                else
                    expert.Person.SecondName = "";

                if (ex.VcExpertFamilyName != null)
                    expert.Person.FamilyName = ex.VcExpertFamilyName;
                else
                    expert.Person.FamilyName = "";

                if (ex.IntExpertGenderId != null && ex.IntExpertGenderId != 0)
                {
                    expert.Person.IdSex = sex.Where(x => Int32.Parse(x.DefaultValue2) == ex.IntExpertGenderId).First().IdKeyValue;
                }

                expert.Person.Indent = ex.VcEgn;
                if (ex.IntEgnTypeId != null)
                {
                    var indentType = indentTypes.Where(x => Int32.Parse(x.DefaultValue2) == ex.IntEgnTypeId).First();
                    expert.Person.IdIndentType = indentType.IdKeyValue;
                    try
                    {
                        if (indentType.KeyValueIntCode.Equals("EGN") && ex.VcEgn != null)
                            expert.Person.BirthDate = GetBirthDate(ex.VcEgn);
                    }
                    catch (Exception e) { }
                }
                if (ex.IntExpertEkatteId != null && ex.IntExpertEkatteId != 0)
                    expert.Person.IdLocation = Locations
                            .Where(x => x.kati.Equals(locations
                            .Where(m => m.Id == ex.IntExpertEkatteId)
                            .First().VcName))
                            .First().idLocation;

                expert.Person.Address = ex.VcExpertAddress;

                expert.Person.PostCode = ex.VcExpertZipcode;

                if (string.IsNullOrEmpty(ex.VcExpertPhone1))
                    expert.Person.Phone = "";
                else if (!string.IsNullOrEmpty(ex.VcExpertPhone1) && !string.IsNullOrEmpty(ex.VcExpertPhone2))
                    expert.Person.Phone = $"{ex.VcExpertPhone1}; {ex.VcExpertPhone2}";
                else
                    expert.Person.Phone = ex.VcExpertPhone1;

                expert.Person.Email = ex.VcExpertEmail1;

                expert.Person.Position = ex.VcTitle;
                expert.Person.Title = ex.VcTitle;

                expert.Person.ModifyDate = DateTime.Now;

                expert.Person.CreationDate = DateTime.Now;

                expert.Person.IdCreateUser = modifyUser.IdUser;

                expert.Person.IdModifyUser = modifyUser.IdUser;

                expert.OldId = (int)ex.Id;

                var typeExpert = ref_experts_types.Where(x => x.IntExpertId == expert.OldId).FirstOrDefault();

                if (typeExpert != null)
                {
                    switch (typeExpert.IntExpertTypeId)
                    {
                        case 2:
                        case 3:
                            var directions = ref_experts_vet_area.Where(x => x.IntExpertId == ex.Id);

                            expert.IsExternalExpert = true;

                            var expertForDB = expert.To<Expert>();

                            this._ApplicationDbContext.Add(expertForDB);
                            this._ApplicationDbContext.SaveChanges();
                            foreach (var direction in directions)
                            {
                                if (direction.IntVetAreaId != null)
                                {
                                    ExpertProfessionalDirectionVM expertProfessional = new ExpertProfessionalDirectionVM();

                                    expertProfessional.OrderNumber = ex.VcInceptionOrder;

                                    expertProfessional.IdStatus = ActiveExpert.IdKeyValue;

                                    expertProfessional.DateApprovalExternalExpert = ex.DtInceptionDate;

                                    expertProfessional.IdExpert = expertForDB.IdExpert;

                                    expertProfessional.IdExpertType = ExternalExpert.IdKeyValue;

                                    expertProfessional.IdProfessionalDirection = ProfessionalDirection.Where(x => x.OldId == direction.IntVetAreaId).First().IdProfessionalDirection;

                                    expertProfessional.ModifyDate = DateTime.Now;

                                    expertProfessional.CreationDate = DateTime.Now;

                                    expertProfessional.IdCreateUser = modifyUser.IdUser;

                                    expertProfessional.IdModifyUser = modifyUser.IdUser;

                                    var dbProf = expertProfessional.To<ExpertProfessionalDirection>();

                                    dbProf.Expert = null;

                                    this._ApplicationDbContext.Add(dbProf);
                                }
                            }

                            break;

                        case 1:
                            expert.IsNapooExpert = true;
                            this._ApplicationDbContext.Add(expert.To<Expert>());
                            break;

                        case 4:
                            expert.IsCommissionExpert = true;

                            ExpertExpertCommissionVM expertExpertCommission = new ExpertExpertCommissionVM();

                            var commision = ref_experts_commissions.Where(x => x.IntExpertId == expert.OldId).FirstOrDefault();
                            if (commision != null)
                            {

                                expertExpertCommission.IdExpertCommission = ExpertCommissions.Where(x => Int32.Parse(x.DefaultValue2) == commision.IntExpCommId).First().IdKeyValue;

                                if (commision.BoolIsChairman != null && commision.BoolIsChairman == true)
                                    expertExpertCommission.IdRole = ExpertRoleCommissions.Where(x => x.KeyValueIntCode == "Chairman").First().IdKeyValue;
                                else
                                    expertExpertCommission.IdRole = ExpertRoleCommissions.Where(x => x.KeyValueIntCode == "Member").First().IdKeyValue;

                                if (commision.IntCommissionInstitutionType != null)
                                {
                                    var Institution = code_commission_institution_type.Where(x => x.Id == commision.IntCommissionInstitutionType).FirstOrDefault();

                                    if (!string.IsNullOrEmpty(commision.VcExpertInstitution))
                                        expertExpertCommission.Institution = $"{Institution.VcCommissionInstitutionTypeName}; {commision.VcExpertInstitution}";
                                    else
                                        expertExpertCommission.Institution = Institution.VcCommissionInstitutionTypeName;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(commision.VcExpertInstitution))
                                        expertExpertCommission.Institution = commision.VcExpertInstitution;
                                    else
                                        expertExpertCommission.Institution = string.Empty;
                                }

                                if (commision.VcExpertOccupation != null)
                                    expertExpertCommission.Occupation = commision.VcExpertOccupation;
                                else
                                    expertExpertCommission.Occupation = string.Empty;

                                if (commision.VcExpertProtokol != null)
                                    expertExpertCommission.Protokol = commision.VcExpertProtokol;
                                else
                                    expertExpertCommission.Protokol = string.Empty;

                                if (commision.DtExpertProtokolDate != null)
                                    expertExpertCommission.ProtokolDate = (DateTime)commision.DtExpertProtokolDate;

                                expertExpertCommission.IdStatus = CandidateProviderTrainerStatuses
                                    .Where(x => x.KeyValueIntCode
                                    .Equals("Active")).First()
                                    .IdKeyValue;
                            }
                            else
                            {
                                expertExpertCommission.Institution = string.Empty;
                                expertExpertCommission.Occupation = string.Empty;
                                expertExpertCommission.Protokol = string.Empty;
                            }
                            expertExpertCommission.Expert = expert;

                            expertExpertCommission.ModifyDate = DateTime.Now;

                            expertExpertCommission.CreationDate = DateTime.Now;

                            expertExpertCommission.IdCreateUser = modifyUser.IdUser;

                            expertExpertCommission.IdModifyUser = modifyUser.IdUser;

                            _ApplicationDbContext.Add(expertExpertCommission.To<ExpertExpertCommission>());
                            break;

                        case 5:

                            var doc = ref_experts_doi.Where(x => x.IntExpertId == ex.Id).FirstOrDefault();

                            if (doc is not null)
                            {
                                expert.IsDOCExpert = true;

                                ExpertDOCVM expertDoc = new ExpertDOCVM();

                                expertDoc.OrderNumber = ex.VcInceptionOrder;

                                expertDoc.DateOrder = ex.DtInceptionDate;

                                if (ex.IsDeleted != null && ex.IsDeleted == true)
                                    expertDoc.IdStatus = CandidateProviderTrainerStatuses
                                        .Where(x => x.KeyValueIntCode
                                        .Equals("Active")).First()
                                        .IdKeyValue;
                                else
                                    expertDoc.IdStatus = CandidateProviderTrainerStatuses
                                       .Where(x => x.KeyValueIntCode
                                       .Equals("Inactive")).First()
                                       .IdKeyValue;

                                expertDoc.Expert = expert;

                                var doi = tb_doi_commissions.Where(x => x.Id == doc.IntDoiCommId).First();

                                expertDoc.IdDOC = DOC.Where(x => x.OldId == doi.Id).First().IdDOC;


                                expertDoc.ModifyDate = DateTime.Now;

                                expertDoc.CreationDate = DateTime.Now;

                                expertDoc.IdCreateUser = modifyUser.IdUser;

                                expertDoc.IdModifyUser = modifyUser.IdUser;

                                var db = expertDoc.To<ExpertDOC>();

                                db.DOC = null;

                                _ApplicationDbContext.Add(db);
                            }
                            else
                            {
                                this._ApplicationDbContext.Add(expert.To<Expert>());
                            }
                            break;

                        default:
                            _ApplicationDbContext.Add(expert.To<Expert>());
                            break;
                    }
                }

                try
                {
                    _ApplicationDbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    this.logger.LogInformation("Гръмна метод ImportExperts(Първи foreach). Запис с Id = " + ex.Id);
                    this.logger.LogError(e.Message);
                    this.logger.LogError(e.InnerException?.Message);
                    this.logger.LogError(e.StackTrace);
                }

            }
            LogEndInformation("ImportExperts");
        }
        //Те са за да навържем DOCExperts
        public void ImportDOC()
        {
            LogStrartInformation("ImportDOC");

            var profession = this._ApplicationDbContext.Professions.Where(x => x.Code.Equals("0")).First();

            var tb_doi_commissions = this._jessieContextContext.TbDoiCommissions.ToList();

            var doc = new ConcurrentBag<DOC>();

            var DraftStatus = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "StatusSPPOO" && kv.KeyValueIntCode == "Draft"
                               select kv).To<KeyValueVM>().First();

            Parallel.ForEach(tb_doi_commissions, doi =>
            {
                DOC currentDoc = new DOC();

                currentDoc.Name = doi.VcDoiCommName;

                currentDoc.IdProfession = profession.IdProfession;

                currentDoc.RequirementsCandidates = string.Empty;

                currentDoc.DescriptionProfession = string.Empty;

                currentDoc.RequirementsMaterialBase = string.Empty;

                currentDoc.RequirementsТrainers = string.Empty;

                currentDoc.UploadedFileName = string.Empty;

                currentDoc.Regulation = string.Empty;

                currentDoc.EndDate = DateTime.MinValue;

                currentDoc.IdStatus = DraftStatus.IdKeyValue;

                currentDoc.IsDOI = false;

                currentDoc.NewspaperNumber = string.Empty;

                currentDoc.PublicationDate = DateTime.MinValue;

                currentDoc.ModifyDate = DateTime.Now;

                currentDoc.CreationDate = DateTime.Now;

                currentDoc.IdCreateUser = modifyUser.IdUser;

                currentDoc.IdModifyUser = modifyUser.IdUser;

                currentDoc.OldId = (int)doi.Id;

                doc.Add(currentDoc);
            });

            _ApplicationDbContext.BulkInsert(doc.ToList());

            LogEndInformation("ImportDOC");
        }

        public void ImportExpertExpert()
        {
            LogStrartInformation("ImportExpertExpert");

            var ref_candidates_experts = this._jessieContextContext.RefCandidatesExperts.ToList();
            var ref_arch_procedure_experts = this._jessieContextContext.RefArchProcedureExperts.ToList();

            var procedureExternalExperts = new ConcurrentBag<ProcedureExternalExpert>();

            var candidateProviders = this._ApplicationDbContext.CandidateProviders.Where(x => x.IdStartedProcedure != null).ToList();

            var startedProcedures = this._ApplicationDbContext.StartedProcedures.ToList();

            var experts = this._ApplicationDbContext.Experts.ToList();

            var professionalDirections = this._ApplicationDbContext.ProfessionalDirections.ToList();

            Parallel.ForEach(ref_candidates_experts, Expert =>
            {
                try
                {
                    var Candidates = candidateProviders.Where(x => x.OldId == Expert.IntProviderId).ToList();

                    foreach (var Candidate in Candidates)
                    {
                        var ProcedureExpert = new ProcedureExternalExpert();

                        ProcedureExpert.IdExpert = experts.Where(x => x.OldId == Expert.IntExpertId).First().IdExpert;

                        ProcedureExpert.IdStartedProcedure = Candidate.IdStartedProcedure!.Value;

                        if (Expert.IntVetArea.HasValue)
                            ProcedureExpert.IdProfessionalDirection = professionalDirections.Where(x => x.OldId == Expert.IntVetArea).First().IdProfessionalDirection;

                        ProcedureExpert.IsActive = true;

                        ProcedureExpert.ModifyDate = DateTime.Now;

                        ProcedureExpert.CreationDate = DateTime.Now;

                        ProcedureExpert.IdCreateUser = modifyUser.IdUser;

                        ProcedureExpert.IdModifyUser = modifyUser.IdUser;

                        procedureExternalExperts.Add(ProcedureExpert);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportExpertExpert(Първи Parallel.ForEach). Запис с Id = " + Expert.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            Parallel.ForEach(ref_arch_procedure_experts, Expert =>
            {
                try
                {
                    if (Expert.IntExpertId is not null)
                    {
                        var ProcedureExpert = new ProcedureExternalExpert();

                        ProcedureExpert.IdExpert = experts.Where(x => x.OldId == Expert.IntExpertId).First().IdExpert;

                        ProcedureExpert.IdStartedProcedure = startedProcedures.Where(x => x.OldId == Expert.IntStartedProcedureId).First().IdStartedProcedure;

                        if (Expert.IntVetArea.HasValue)
                            ProcedureExpert.IdProfessionalDirection = professionalDirections.Where(x => x.OldId == Expert.IntVetArea).First().IdProfessionalDirection;

                        ProcedureExpert.IsActive = true;

                        ProcedureExpert.ModifyDate = DateTime.Now;

                        ProcedureExpert.CreationDate = DateTime.Now;

                        ProcedureExpert.IdCreateUser = modifyUser.IdUser;

                        ProcedureExpert.IdModifyUser = modifyUser.IdUser;

                        procedureExternalExperts.Add(ProcedureExpert);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportExpertExpert(Втори Parallel.ForEach). Запис с Id = " + Expert.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(procedureExternalExperts.ToList());



            LogEndInformation("ImportExpertExpert");
        }

        public void ImportExpertCommission()
        {
            LogStrartInformation("ImportExpertCommission");

            var ref_candidates_expert_commissions = this._jessieContextContext.RefCandidatesExpertCommissions.ToList();

            var ref_arch_procedure_expert_commissions = this._jessieContextContext.RefArchProcedureExpertCommissions.ToList();

            var ref_experts_commissions = this._jessieContextContext.RefExpertsCommissions.ToList();

            var CandidateProviders = this._ApplicationDbContext.CandidateProviders.Where(x => x.IdStartedProcedure != null).ToList();

            var startedProcedure = this._ApplicationDbContext.StartedProcedures.ToList();

            var ExpertCommissions = (from kv in _ApplicationDbContext.KeyValues
                                     join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                     where kt.KeyTypeIntCode == "ExpertCommission"
                                     select kv).To<KeyValueVM>().ToList();

            var ProcedureExpertCommissions = new ConcurrentBag<ProcedureExpertCommission>();

            Parallel.ForEach(ref_candidates_expert_commissions, commission =>
            {
                try
                {
                    var candidates = CandidateProviders.Where(x => x.OldId == commission.IntProviderId).ToList();

                    foreach (var candidate in candidates)
                    {
                        var procedureExpertCommission = new ProcedureExpertCommission();

                        procedureExpertCommission.IdStartedProcedure = candidate.IdStartedProcedure!.Value;

                        procedureExpertCommission.IdExpertCommission = ExpertCommissions
                        .Where(x => Int32.Parse(x.DefaultValue2) == commission.IntExpertCommissionId)
                        .First().IdKeyValue;

                        ProcedureExpertCommissions.Add(procedureExpertCommission);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportExpertCommission(Първи Parallel.ForEach). Запис с Id = " + commission.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }

            });

            Parallel.ForEach(ref_arch_procedure_expert_commissions, commission =>
            {
                try
                {
                    var procedureExpertCommission = new ProcedureExpertCommission();

                    procedureExpertCommission.IdStartedProcedure = startedProcedure
                .Where(x => x.OldId == commission.IntStartedProcedureId)
                .First().IdStartedProcedure;

                    procedureExpertCommission.IdExpertCommission = ExpertCommissions
                    .Where(x => Int32.Parse(x.DefaultValue2) == commission.IntExpertCommissionId)
                    .First().IdKeyValue;

                    ProcedureExpertCommissions.Add(procedureExpertCommission);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportExpertCommission(Втори Parallel.ForEach). Запис с Id = " + commission.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });


            Parallel.ForEach(ref_experts_commissions, comission =>
            {
                var procedureExpertCommission = new ExpertExpertCommission();

            });

            this._ApplicationDbContext.BulkInsert(ProcedureExpertCommissions.ToList());


            LogEndInformation("|ImportExpertCommission");
        }

        public void ImportNapooExpert()
        {
            LogStrartInformation("ImportNapooExpert");

            var napooExperts = this._ApplicationDbContext.Experts.Where(x => x.IsNapooExpert).ToList();

            var ref_experts_types = this._jessieContextContext.RefExpertsTypes.ToList();

            var expert = new ConcurrentBag<ExpertNapoo>();

            var ActiveExpert = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "ExpertStatus" && kv.KeyValueIntCode == "ActiveExpert"
                                  select kv).To<KeyValueVM>().First();

            Parallel.ForEach(napooExperts, napooExpert =>
            {
                var ExpertNapoo = new ExpertNapoo();

                var type = ref_experts_types.Where(x => x.IntExpertId == napooExpert.OldId).FirstOrDefault();

                ExpertNapoo.IdExpert = napooExpert.IdExpert;

                if(type is not null)
                ExpertNapoo.Occupation = type.VcPosition;

                ExpertNapoo.AppointmentDate = DateTime.Now;

                ExpertNapoo.IdStatus = ActiveExpert.IdKeyValue;

                ExpertNapoo.ModifyDate = DateTime.Now;

                ExpertNapoo.CreationDate = DateTime.Now;

                ExpertNapoo.IdCreateUser = modifyUser.IdUser;

                ExpertNapoo.IdModifyUser = modifyUser.IdUser;

                expert.Add(ExpertNapoo);
            });

            this._ApplicationDbContext.BulkInsert(expert.ToList());

            LogEndInformation("ImportNapooExpert");
        }
        #endregion

        #region SPPOO

        internal void ImportSPPOO()
        {
            LogStrartInformation("ImportSPPOO");
            //var res = _ApplicationDbContext.Settings.ToList();

            //code_vet_group =>SPPOO_Area

            if (this._ApplicationDbContext.Areas.Any())
            {
                this._ApplicationDbContext.Areas.BatchDelete();
            }

            var code_vet_group = _jessieContextContext.CodeVetGroups.ToList();
            List<AreaVM> SppooAreas = new List<AreaVM>();
            var test = _ApplicationDbContext.Areas.ToList();
            foreach (var SA in code_vet_group)
            {
                AreaVM SppooArea = new AreaVM();

                if (SA.IntVetGroupNumber != null)
                    SppooArea.Code = SA.IntVetGroupNumber.ToString();
                else
                    SppooArea.Code = "";

                if (SA.VcVetGroupName != null)
                    SppooArea.Name = SA.VcVetGroupName;
                else
                    SppooArea.Name = "";

                if (SA.VcVetGroupNameEn != null)
                    SppooArea.NameEN = SA.VcVetGroupNameEn;

                else
                    SppooArea.NameEN = ConvertCyrToLatin(SppooArea.Name);


                if (SA.BoolIsValid != null && (bool)SA.BoolIsValid == true)
                    SppooArea.IdStatus = kvSPPOStatusActive.IdKeyValue;
                else
                    SppooArea.IdStatus = kvSPPOStatusInactive.IdKeyValue;

                SppooArea.CreationDate = DateTime.Now;

                SppooArea.IdModifyUser = modifyUser.IdUser;

                SppooArea.IdCreateUser = modifyUser.IdUser;

                SppooArea.ModifyDate = DateTime.Now;

                SppooArea.oldId = SA.Id;
                SppooAreas.Add(SppooArea);
            }

            foreach (var area in SppooAreas)
            {
                var model = area.To<Area>();
                _ApplicationDbContext.Add(model);
                _ApplicationDbContext.SaveChanges();
                _ApplicationDbContext.ChangeTracker.Clear();
                area.IdArea = model.IdArea;
            }


            //code_vet_area  =>SPPOO_ProfessionalDirection
            var code_vet_area = _jessieContextContext.CodeVetAreas.ToList();
            List<ProfessionalDirectionVM> ProfessionalDirections = new List<ProfessionalDirectionVM>();


            foreach (var SPD in code_vet_area)
            {
                ProfessionalDirectionVM SppoProfessionalDirection = new ProfessionalDirectionVM();

                var FK = code_vet_group.Where(x => x.Id == SPD.IntVetGroupId).First();

                SppoProfessionalDirection.Area = SppooAreas.Where(x => x.oldId == FK.Id).FirstOrDefault();
                if (SPD.IntVetAreaNumber != null)
                    SppoProfessionalDirection.Code = SPD.IntVetAreaNumber.ToString();
                else
                    SppoProfessionalDirection.Code = "";

                if (SPD.VcVetAreaName != null)
                    SppoProfessionalDirection.Name = SPD.VcVetAreaName;
                else
                    SppoProfessionalDirection.Name = "";
                if (SPD.VcVetAreaNameEn != null)
                    SppoProfessionalDirection.NameEN = SPD.VcVetAreaNameEn;
                else
                    SppoProfessionalDirection.NameEN = ConvertCyrToLatin(SppoProfessionalDirection.Name);

                SppoProfessionalDirection.CreationDate = DateTime.Now;

                if (SPD.BoolIsValid != null && (bool)SPD.BoolIsValid == true)
                    SppoProfessionalDirection.IdStatus = kvSPPOStatusActive.IdKeyValue;
                else
                    SppoProfessionalDirection.IdStatus = kvSPPOStatusInactive.IdKeyValue;

                SppoProfessionalDirection.IdModifyUser = modifyUser.IdUser;

                SppoProfessionalDirection.IdCreateUser = modifyUser.IdUser;

                SppoProfessionalDirection.ModifyDate = DateTime.Now;

                SppoProfessionalDirection.CreationDate = DateTime.Now;

                SppoProfessionalDirection.oldId = SPD.Id;


                var PD = SppoProfessionalDirection.To<ProfessionalDirection>();
                PD.Area = _ApplicationDbContext.Areas.Where(x => x.IdArea == SppoProfessionalDirection.Area.IdArea).First();
                _ApplicationDbContext.Add(PD);
                _ApplicationDbContext.SaveChanges();
                SppoProfessionalDirection.IdProfessionalDirection = PD.IdProfessionalDirection;
                _ApplicationDbContext.ChangeTracker.Clear();
                ProfessionalDirections.Add(SppoProfessionalDirection);


                if (SPD.VcVetAreaCorrectionNotes != null)
                {
                    OrderModel oldOrder = new OrderModel(SPD.VcVetAreaCorrectionNotes);

                    SPPOOOrder order = GetOrder(oldOrder.Order);
                    if (order.OrderNumber.Equals(string.Empty) && !string.IsNullOrEmpty(oldOrder.Date))
                    {
                        order.OrderNumber = oldOrder.Order;

                        if (!oldOrder.Date.Equals(""))

                            order.OrderDate = DateTime.ParseExact(oldOrder.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        order.IdModifyUser = modifyUser.IdUser;

                        order.UploadedFileName = SPD.VcVetAreaCorrectionNotes;

                        order.ModifyDate = DateTime.Now;

                        order.CreationDate = DateTime.Now;

                        order.IdCreateUser = modifyUser.IdUser;

                        orders.Add(order);
                        _ApplicationDbContext.Add(order);
                        _ApplicationDbContext.SaveChanges();


                        ProfessionalDirectionOrder professionalDirectionOrder = new ProfessionalDirectionOrder();

                        professionalDirectionOrder.ProfessionalDirection = _ApplicationDbContext.ProfessionalDirections.Where(x => x.IdProfessionalDirection == SppoProfessionalDirection.IdProfessionalDirection).First();

                        professionalDirectionOrder.SPPOOOrder = _ApplicationDbContext.SPPOOOrders.Where(x => x.IdOrder == order.IdOrder).First();

                        if (SPD.IntVetAreaCorrection == 1)
                        {
                            professionalDirectionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Created") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SPD.IntVetAreaCorrection == 2 || SPD.IntVetAreaCorrection == 4)
                        {
                            professionalDirectionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Changed") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SPD.IntVetAreaCorrection == 3)
                        {
                            professionalDirectionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Deleted") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;

                        }
                        _ApplicationDbContext.Add(professionalDirectionOrder);
                        _ApplicationDbContext.SaveChanges();
                    }
                }
            }

            //code_vet_profession =>SPPOO_Profession
            var code_vet_profession = _jessieContextContext.CodeVetProfessions.ToList();
            List<ProfessionVM> professions = new List<ProfessionVM>();

            foreach (var SP in code_vet_profession)
            {
                ProfessionVM SppooProfession = new ProfessionVM();

                SppooProfession.Code = SP.IntVetProfessionNumber.ToString();

                var FK = code_vet_area.Where(x => x.Id == SP.IntVetAreaId).First();

                SppooProfession.ProfessionalDirection = ProfessionalDirections.Where(x => x.oldId == FK.Id).FirstOrDefault();

                if (SP.VcVetProfessionName != null)
                    SppooProfession.Name = SP.VcVetProfessionName;
                else
                    SppooProfession.Name = "";

                if (SP.VcVetProfessionNameEn != null)
                    SppooProfession.NameEN = SP.VcVetProfessionNameEn;
                else
                    SppooProfession.NameEN = ConvertCyrToLatin(SppooProfession.Name);

                SppooProfession.CreationDate = DateTime.Now;

                SppooProfession.IdModifyUser = modifyUser.IdUser;

                SppooProfession.IdCreateUser = modifyUser.IdUser;

                SppooProfession.ModifyDate = DateTime.Now;

                SppooProfession.oldId = SP.Id;

                if (SP.BoolIsValid != null && (bool)SP.BoolIsValid == true)
                    SppooProfession.IdStatus = kvSPPOStatusActive.IdKeyValue;
                else
                    SppooProfession.IdStatus = kvSPPOStatusInactive.IdKeyValue;


                var P = SppooProfession.To<Profession>();
                P.ProfessionalDirection = _ApplicationDbContext.ProfessionalDirections.Where(x => x.IdProfessionalDirection == SppooProfession.ProfessionalDirection.IdProfessionalDirection).First();
                _ApplicationDbContext.Add(P);
                _ApplicationDbContext.SaveChanges();
                _ApplicationDbContext.ChangeTracker.Clear();
                SppooProfession.IdProfession = P.IdProfession;
                professions.Add(SppooProfession);

                if (SP.VcVetProfessionCorrectionNotes != null)
                {
                    OrderModel oldOrder = new OrderModel(SP.VcVetProfessionCorrectionNotes);

                    SPPOOOrder order = GetOrder(oldOrder.Order);
                    if (order.OrderNumber.Equals(string.Empty) && !string.IsNullOrEmpty(oldOrder.Date))
                    {
                        order.OrderNumber = oldOrder.Order;

                        if (!oldOrder.Date.Equals(""))
                            order.OrderDate = DateTime.ParseExact(oldOrder.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        order.IdModifyUser = modifyUser.IdUser;

                        order.UploadedFileName = SP.VcVetProfessionCorrectionNotes;

                        order.IdCreateUser = modifyUser.IdUser;

                        order.ModifyDate = DateTime.Now;

                        order.CreationDate = DateTime.Now;

                        orders.Add(order);
                        _ApplicationDbContext.Add(order);
                        _ApplicationDbContext.SaveChanges();

                        ProfessionOrder professionOrder = new ProfessionOrder();

                        professionOrder.Profession = _ApplicationDbContext.Professions.Where(x => x.IdProfession == SppooProfession.IdProfession).First();

                        professionOrder.SPPOOOrder = _ApplicationDbContext.SPPOOOrders.Where(x => x.IdOrder == order.IdOrder).First();

                        if (SP.IntVetProfessionCorrection == 1)
                        {
                            professionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Created") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SP.IntVetProfessionCorrection == 2 || SP.IntVetProfessionCorrection == 4)
                        {
                            professionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Changed") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SP.IntVetProfessionCorrection == 3)
                        {
                            professionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Deleted") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;

                        }

                        _ApplicationDbContext.Add(professionOrder);
                        _ApplicationDbContext.SaveChanges();
                    }
                }
            }


            //code_vet_speciality =>SPPOO_Speciality
            var code_vet_speciality = _jessieContextContext.CodeVetSpecialities.ToList();
            List<SpecialityVM> specialities = new List<SpecialityVM>();

            foreach (var SS in code_vet_speciality)
            {
                SpecialityVM SppooSpeciality = new SpecialityVM();
                var FK = code_vet_profession.Where(x => x.Id == SS.IntVetProfessionId).First();

                SppooSpeciality.Profession = professions.Where(x => x.oldId == FK.Id).First();

                if (SS.IntVetSpecialityNumber != null)
                    SppooSpeciality.Code = SS.IntVetSpecialityNumber.ToString();
                else
                    SppooSpeciality.Code = "";

                if (SS.VcVetSpecialityName != null)
                    SppooSpeciality.Name = SS.VcVetSpecialityName;
                else
                    SppooSpeciality.Name = "";

                SppooSpeciality.OldId = (long)SS.Id;

                if (SS.VcVetSpecialityNameEn != null)
                    SppooSpeciality.NameEN = SS.VcVetSpecialityNameEn;
                else
                    SppooSpeciality.NameEN = ConvertCyrToLatin(SppooSpeciality.Name);

                SppooSpeciality.CreationDate = DateTime.Now;
                if (SS.BoolIsValid != null && SS.BoolIsValid == true)
                    SppooSpeciality.IdStatus = kvSPPOStatusActive.IdKeyValue;
                else
                    SppooSpeciality.IdStatus = kvSPPOStatusInactive.IdKeyValue;

                SppooSpeciality.Doc = null;

                SppooSpeciality.LinkNIP = "#";

                SppooSpeciality.LinkMON = "#";

                //SppooSpeciality.IdModifyUser = modifyUser.IdUser;

                //SppooSpeciality.IdCreateUser = modifyUser.IdUser;

                SppooSpeciality.ModifyDate = DateTime.Now;

                if (SS.IntSpecialityVqs == 1)
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "I_VQS").First()).IdKeyValue;
                }
                else if (SS.IntSpecialityVqs == 2)
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "II_VQS").First()).IdKeyValue;
                }
                else if (SS.IntSpecialityVqs == 3)
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "III_VQS").First()).IdKeyValue;
                }
                else if (SS.IntSpecialityVqs == 4)
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "IV_VQS").First()).IdKeyValue;
                }
                else
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "V_VQS").First()).IdKeyValue;
                }


                if (SppooSpeciality.Name != null)
                {
                    var S = SppooSpeciality.To<Speciality>();
                    S.Profession = _ApplicationDbContext.Professions.Where(x => x.IdProfession == SppooSpeciality.Profession.IdProfession).First();
                    _ApplicationDbContext.Add(S);
                    _ApplicationDbContext.SaveChanges();
                    _ApplicationDbContext.ChangeTracker.Clear();
                    SppooSpeciality.IdSpeciality = S.IdSpeciality;
                    specialities.Add(SppooSpeciality);

                }
                if (SS.VcVetSpecialityCorrectionNotes != null)
                {
                    OrderModel oldOrder = new OrderModel(SS.VcVetSpecialityCorrectionNotes);

                    SPPOOOrder order = GetOrder(oldOrder.Order);
                    if (order.OrderNumber.Equals(string.Empty) && !string.IsNullOrEmpty(oldOrder.Date))
                    {
                        order.OrderNumber = oldOrder.Order;

                        if (!oldOrder.Date.Equals(""))
                            order.OrderDate = DateTime.ParseExact(oldOrder.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        //order.IdModifyUser = modifyUser.IdUser;

                        order.UploadedFileName = SS.VcVetSpecialityCorrectionNotes;

                        order.ModifyDate = DateTime.Now;

                        //order.IdCreateUser = modifyUser.IdUser;

                        order.CreationDate = DateTime.Now;

                        orders.Add(order);
                        _ApplicationDbContext.Add(order);
                        _ApplicationDbContext.SaveChanges();

                        SpecialityOrder specialityOrder = new SpecialityOrder();

                        specialityOrder.SPPOOOrder = _ApplicationDbContext.SPPOOOrders.Where(x => x.IdOrder == order.IdOrder).First();

                        specialityOrder.Speciality = _ApplicationDbContext.Specialities.Where(x => x.IdSpeciality == SppooSpeciality.IdSpeciality).First();

                        if (SS.IntVetSpecialityCorrection == 1)
                        {
                            specialityOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Created") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SS.IntVetSpecialityCorrection == 2 || SS.IntVetSpecialityCorrection == 4)
                        {
                            specialityOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Changed") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SS.IntVetSpecialityCorrection == 3)
                        {
                            specialityOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Deleted") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;

                        }
                        _ApplicationDbContext.Add(specialityOrder);
                        _ApplicationDbContext.SaveChanges();
                    }
                }

            }


            //var kvSPPOStatusActive = (from kv in _ApplicationDbContext.KeyValues
            //                          join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
            //                          where kv.KeyValueIntCode == "Active" && kt.KeyTypeIntCode == "StatusSPPOO"
            //                          select kv).ToList();



            //var kvSPPOStatusActive1 = _ApplicationDbContext
            //                        .KeyValues.Include(kv => kv.KeyType)
            //                        .Where(kv => kv.KeyValueIntCode == "Active" && kv.KeyType.KeyTypeIntCode == "StatusSPPOO")
            //                        .ToList();

            LogEndInformation("ImportSPPOO");

        }

        void importSpeciality()
        {
            var code_vet_speciality = _jessieContextContext.CodeVetSpecialities.ToList();
            var code_vet_profession = _jessieContextContext.CodeVetProfessions.ToList();

            var professions = _ApplicationDbContext.Professions.To<ProfessionVM>();
            List<SpecialityVM> specialities = new List<SpecialityVM>();

            foreach (var SS in code_vet_speciality)
            {
                SpecialityVM SppooSpeciality = new SpecialityVM();
                var FK = code_vet_profession.Where(x => x.Id == SS.IntVetProfessionId).First();

                SppooSpeciality.Profession = professions.Where(x => x.oldId == FK.Id).First();

                if (SS.IntVetSpecialityNumber != null)
                    SppooSpeciality.Code = SS.IntVetSpecialityNumber.ToString();
                else
                    SppooSpeciality.Code = "";

                if (SS.VcVetSpecialityName != null)
                    SppooSpeciality.Name = SS.VcVetSpecialityName;
                else
                    SppooSpeciality.Name = "";

                SppooSpeciality.OldId = (long)SS.Id;

                if (SS.VcVetSpecialityNameEn != null)
                    SppooSpeciality.NameEN = SS.VcVetSpecialityNameEn;
                else
                    SppooSpeciality.NameEN = ConvertCyrToLatin(SppooSpeciality.Name);

                SppooSpeciality.CreationDate = DateTime.Now;
                if (SS.BoolIsValid != null && SS.BoolIsValid == true)
                    SppooSpeciality.IdStatus = kvSPPOStatusActive.IdKeyValue;
                else
                    SppooSpeciality.IdStatus = kvSPPOStatusInactive.IdKeyValue;

                SppooSpeciality.IdModifyUser = modifyUser.IdUser;

                SppooSpeciality.Doc = null;

                SppooSpeciality.LinkNIP = "#";

                SppooSpeciality.LinkMON = "#";

                SppooSpeciality.IdCreateUser = modifyUser.IdUser;

                SppooSpeciality.ModifyDate = DateTime.Now;

                if (SS.IntSpecialityVqs == 1)
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "I_VQS").First()).IdKeyValue;
                }
                else if (SS.IntSpecialityVqs == 2)
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "II_VQS").First()).IdKeyValue;
                }
                else if (SS.IntSpecialityVqs == 3)
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "III_VQS").First()).IdKeyValue;
                }
                else if (SS.IntSpecialityVqs == 4)
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "IV_VQS").First()).IdKeyValue;
                }
                else
                {
                    SppooSpeciality.IdVQS = (keyValues.Where(x => x.KeyValueIntCode == "V_VQS").First()).IdKeyValue;
                }


                if (SppooSpeciality.Name != null)
                {
                    var S = SppooSpeciality.To<Speciality>();
                    S.Profession = _ApplicationDbContext.Professions.Where(x => x.IdProfession == SppooSpeciality.Profession.IdProfession).First();
                    _ApplicationDbContext.Add(S);
                    _ApplicationDbContext.SaveChanges();
                    _ApplicationDbContext.ChangeTracker.Clear();
                    SppooSpeciality.IdSpeciality = S.IdSpeciality;
                    specialities.Add(SppooSpeciality);

                }
                if (SS.VcVetSpecialityCorrectionNotes != null)
                {
                    OrderModel oldOrder = new OrderModel(SS.VcVetSpecialityCorrectionNotes);

                    SPPOOOrder order = GetOrder(oldOrder.Order);
                    if (order.OrderNumber.Equals(string.Empty))
                    {
                        order.OrderNumber = oldOrder.Order;

                        if (!oldOrder.Date.Equals(""))
                            order.OrderDate = DateTime.ParseExact(oldOrder.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        order.IdModifyUser = modifyUser.IdUser;

                        order.UploadedFileName = SS.VcVetSpecialityCorrectionNotes;

                        order.ModifyDate = DateTime.Now;

                        order.IdCreateUser = modifyUser.IdUser;

                        order.CreationDate = DateTime.Now;

                        orders.Add(order);
                        _ApplicationDbContext.Add(order);
                        _ApplicationDbContext.SaveChanges();
                    }

                    SpecialityOrder specialityOrder = new SpecialityOrder();

                    specialityOrder.SPPOOOrder = _ApplicationDbContext.SPPOOOrders.Where(x => x.IdOrder == order.IdOrder).First();

                    specialityOrder.Speciality = _ApplicationDbContext.Specialities.Where(x => x.IdSpeciality == SppooSpeciality.IdSpeciality).First();

                    if (SS.IntVetSpecialityCorrection == 1)
                    {
                        specialityOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Created") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                    }
                    else if (SS.IntVetSpecialityCorrection == 2 || SS.IntVetSpecialityCorrection == 4)
                    {
                        specialityOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Changed") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                    }
                    else if (SS.IntVetSpecialityCorrection == 3)
                    {
                        specialityOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Deleted") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;

                    }
                    _ApplicationDbContext.Add(specialityOrder);
                    _ApplicationDbContext.SaveChanges();
                }

            }
        }

        public void ImportSppooFrameworkProgram()
        {
            LogStrartInformation("ImportSppooFrameworkProgram");

            if (this._ApplicationDbContext.FrameworkPrograms.Any())
            {
                this._ApplicationDbContext.FrameworkPrograms.BatchDelete();
            }

            var code_course_fr_curr = _jessieContextContext.CodeCourseFrCurrs.ToList();

            var TypeFrameworkPrograms = (from kv in _ApplicationDbContext.KeyValues
                                         join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                         where kt.KeyTypeIntCode == "TypeFrameworkProgram"
                                         select kv).To<KeyValueVM>().ToList();

            var MinimumLevelEducations = (from kv in _ApplicationDbContext.KeyValues
                                          join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                          where kt.KeyTypeIntCode == "MinimumLevelEducation"
                                          select kv).To<KeyValueVM>().ToList();

            var TrainingPeriods = (from kv in _ApplicationDbContext.KeyValues
                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                   where kt.KeyTypeIntCode == "TrainingPeriod"
                                   select kv).To<KeyValueVM>().ToList();

            var VQS = (from kv in _ApplicationDbContext.KeyValues
                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                       where kt.KeyTypeIntCode == "VQS"
                       select kv).To<KeyValueVM>().ToList();

            var StatusTemplates = (from kv in _ApplicationDbContext.KeyValues
                                   join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                   where kt.KeyTypeIntCode == "StatusTemplate"
                                   select kv).To<KeyValueVM>().ToList();

            var FormEducations = (from kv in _ApplicationDbContext.KeyValues
                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                  where kt.KeyTypeIntCode == "FormEducation"
                                  select kv).To<KeyValueVM>().ToList();

            var WithoutQualification = (from kv in _ApplicationDbContext.KeyValues
                                        join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                        where kv.KeyValueIntCode == "WithoutQualification" && kt.KeyTypeIntCode == "QualificationLevel"
                                        select kv).First();

            foreach (var cc in code_course_fr_curr)
            {
                FrameworkProgramVM program = new FrameworkProgramVM();

                if (cc.VcCourseFrCurrName != null)
                {
                    program.Name = cc.VcCourseFrCurrName;

                    if (cc.IntCourseType != null)
                        program.IdTypeFrameworkProgram = TypeFrameworkPrograms
                            .Where(x => Int32.Parse(x.DefaultValue2) == cc.IntCourseType)
                            .First()
                            .IdKeyValue;

                    if (cc.VcDescInEdu != null)
                        if (cc.VcDescInEdu.Contains("Х"))
                        {
                            program.IdMinimumLevelEducation = MinimumLevelEducations
                                .Where(x => x.KeyValueIntCode.Equals("MinimumLevelEducation2"))
                                .First()
                                .IdKeyValue;
                        }
                        else if (cc.VcDescInEdu.StartsWith('З'))
                        {
                            program.IdMinimumLevelEducation = MinimumLevelEducations
                                .Where(x => x.KeyValueIntCode.Equals("MinimumLevelEducation1"))
                                .First()
                                .IdKeyValue;
                        }
                        else
                        {
                            program.IdMinimumLevelEducation = MinimumLevelEducations
                               .Where(x => x.KeyValueIntCode.Equals("MinimumLevelEducation3"))
                               .First()
                               .IdKeyValue;
                        }
                    if (cc.IntDurationMonths != null)
                    {
                        switch (cc.IntDurationMonths)
                        {
                            case 6:
                                program.IdTrainingPeriod = TrainingPeriods
                                    .Where(x => x.KeyValueIntCode.Equals("SixM"))
                                    .First()
                                    .IdKeyValue;
                                break;

                            case 12:
                                program.IdTrainingPeriod = TrainingPeriods
                                   .Where(x => x.KeyValueIntCode.Equals("TwelveM"))
                                   .First()
                                   .IdKeyValue;
                                break;

                            case 18:
                                program.IdTrainingPeriod = TrainingPeriods
                                   .Where(x => x.KeyValueIntCode.Equals("EighteenM"))
                                   .First()
                                   .IdKeyValue;
                                break;
                        }
                    }
                    else
                    {
                        program.IdTrainingPeriod = TrainingPeriods
                                  .Where(x => x.KeyValueIntCode.Equals("NoneDoc"))
                                  .First()
                                  .IdKeyValue;
                    }
                    if (cc.IntMandatoryHours != null)
                        program.SectionА = (double)cc.IntMandatoryHours;

                    if (cc.IntSelectableHours != null)
                        program.SectionB = (double)cc.IntSelectableHours;

                    if (cc.IntMinPercPractice != null)
                        program.Practice = (double)cc.IntMinPercPractice;

                    if (cc.IntVqs != null)
                    {
                        switch (cc.IntVqs)
                        {
                            case 1:
                                program.IdVQS = VQS
                                    .Where(x => x.KeyValueIntCode.Equals("I_VQS"))
                                    .First()
                                    .IdKeyValue;
                                break;
                            case 2:
                                program.IdVQS = VQS
                                   .Where(x => x.KeyValueIntCode.Equals("II_VQS"))
                                   .First()
                                   .IdKeyValue;
                                break;
                            case 3:
                                program.IdVQS = VQS
                                   .Where(x => x.KeyValueIntCode.Equals("III_VQS"))
                                   .First()
                                   .IdKeyValue;
                                break;
                            case 4:
                                program.IdVQS = VQS
                                   .Where(x => x.KeyValueIntCode.Equals("IV_VQS"))
                                   .First()
                                   .IdKeyValue;
                                break;
                            case 5:
                                program.IdVQS = VQS
                                   .Where(x => x.KeyValueIntCode.Equals("V_VQS"))
                                   .First()
                                   .IdKeyValue;
                                break;

                        }
                    }
                    if (cc.IntMinPercCommon != null)
                        program.SectionА1 = (double)cc.IntMinPercCommon;

                    program.IdQualificationLevel = WithoutQualification.IdKeyValue;

                    if (cc.VcDescInQual != null)
                        program.MinimumLevelQualification = cc.VcDescInQual;
                    else
                        program.MinimumLevelQualification = "";

                    if (cc.VcShortDesc != null)
                        program.ShortDescription = cc.VcShortDesc;
                    else
                        program.ShortDescription = "";

                    if (cc.VcDescription != null)
                        program.Description = cc.VcDescription;
                    else
                        program.Description = "";

                    if (cc.BoolValid != null && cc.BoolValid == true)
                    {
                        program.IdStatus = StatusTemplates.Where(x => x.KeyValueIntCode.Equals("Active")).First().IdKeyValue;
                        program.IsValid = (bool)cc.BoolValid;
                    }
                    else
                    {
                        program.IdStatus = StatusTemplates.Where(x => x.KeyValueIntCode.Equals("Inactive")).First().IdKeyValue;
                        program.IsValid = false;
                    }
                    program.OldId = (int?)cc.Id;

                    program.ModifyDate = DateTime.Now;

                    program.CreationDate = DateTime.Now;

                    program.IdCreateUser = modifyUser.IdUser;

                    program.IdModifyUser = modifyUser.IdUser;

                    program.CompletionVocationalTraining = String.Empty;
                    program.ExplanatoryNotes = String.Empty;

                    var forDB = program.To<FrameworkProgram>();
                    _ApplicationDbContext.Add(forDB);
                    _ApplicationDbContext.SaveChanges();

                    if (cc.VcEdForms != null && cc.VcEdForms != "")
                        foreach (var type in cc.VcEdForms.Split(","))
                        {
                            FrameworkProgramFormEducationVM formEducation = new FrameworkProgramFormEducationVM();

                            formEducation.IdFrameworkProgram = forDB.IdFrameworkProgram;

                            var key = FormEducations.Where(x => x.Name.ToLower().Trim().Equals(type.ToLower().Trim())).FirstOrDefault();

                            if (key != null)
                                formEducation.IdFormEducation = key.IdKeyValue;
                            else
                                formEducation.IdFormEducation = FormEducations.Where(x => x.KeyValueIntCode.Equals("Type7")).First().IdKeyValue;

                            _ApplicationDbContext.Add(formEducation.To<FrameworkProgramFormEducation>());
                            _ApplicationDbContext.SaveChanges();
                        }
                }
                else if (cc.Id == 11)
                {
                    program.OldId = (int?)cc.Id;


                }
            }

            LogEndInformation("ImportSppooFrameworkProgram");
        }

        #endregion

        public void ImportSPPOOrder()
        {
            LogStrartInformation("ImportSPPOOrder");

            if (this._ApplicationDbContext.ProfessionalDirectionOrders.Any())
            {
                this._ApplicationDbContext.ProfessionalDirectionOrders.BatchDelete();
            }

            if (this._ApplicationDbContext.SpecialityOrders.Any())
            {
                this._ApplicationDbContext.SpecialityOrders.BatchDelete();
            }

            if (this._ApplicationDbContext.ProfessionOrders.Any())
            {
                this._ApplicationDbContext.ProfessionOrders.BatchDelete();
            }

            if (this._ApplicationDbContext.SPPOOOrders.Any())
            {
                this._ApplicationDbContext.SPPOOOrders.BatchDelete();
            }

            var code_vet_area = _jessieContextContext.CodeVetAreas.ToList();
            var professionDirection = this._ApplicationDbContext.ProfessionalDirections.ToList();

            foreach (var SPD in code_vet_area)
            {
                var SppoProfessionalDirection = professionDirection.Where(x => x.OldId == SPD.Id).First();

                if (SPD.VcVetAreaCorrectionNotes != null)
                {
                    OrderModel oldOrder = new OrderModel(SPD.VcVetAreaCorrectionNotes);

                    SPPOOOrder order = GetOrder(oldOrder.Order);
                    if (order.OrderNumber.Equals(string.Empty) && !string.IsNullOrEmpty(oldOrder.Date))
                    {
                        order.OrderNumber = oldOrder.Order;

                        if (!oldOrder.Date.Equals(""))

                            order.OrderDate = DateTime.ParseExact(oldOrder.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        order.IdModifyUser = modifyUser.IdUser;

                        order.UploadedFileName = SPD.VcVetAreaCorrectionNotes;

                        order.ModifyDate = DateTime.Now;

                        order.CreationDate = DateTime.Now;

                        order.IdCreateUser = modifyUser.IdUser;

                        orders.Add(order);
                        _ApplicationDbContext.Add(order);
                        _ApplicationDbContext.SaveChanges();
                    }

                    if (order.IdOrder != 0)
                    {
                        ProfessionalDirectionOrder professionalDirectionOrder = new ProfessionalDirectionOrder();

                        professionalDirectionOrder.ProfessionalDirection = _ApplicationDbContext.ProfessionalDirections.Where(x => x.IdProfessionalDirection == SppoProfessionalDirection.IdProfessionalDirection).First();

                        professionalDirectionOrder.SPPOOOrder = _ApplicationDbContext.SPPOOOrders.Where(x => x.IdOrder == order.IdOrder).First();

                        if (SPD.IntVetAreaCorrection == 1)
                        {
                            professionalDirectionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Created") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SPD.IntVetAreaCorrection == 2 || SPD.IntVetAreaCorrection == 4)
                        {
                            professionalDirectionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Changed") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SPD.IntVetAreaCorrection == 3)
                        {
                            professionalDirectionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Deleted") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;

                        }
                        _ApplicationDbContext.Add(professionalDirectionOrder);
                        _ApplicationDbContext.SaveChanges();
                    }
                }
            }

            var code_vet_profession = _jessieContextContext.CodeVetProfessions.ToList();
            var professions = this._ApplicationDbContext.Professions.ToList();

            foreach (var SP in code_vet_profession)
            {
                var SppooProfession = professions.Where(x => x.OldId == SP.Id).First();

                if (SP.VcVetProfessionCorrectionNotes != null)
                {
                    OrderModel oldOrder = new OrderModel(SP.VcVetProfessionCorrectionNotes);

                    SPPOOOrder order = GetOrder(oldOrder.Order);
                    if (order.OrderNumber.Equals(string.Empty) && !string.IsNullOrEmpty(oldOrder.Date))
                    {
                        order.OrderNumber = oldOrder.Order;

                        if (!oldOrder.Date.Equals(""))
                            order.OrderDate = DateTime.ParseExact(oldOrder.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        order.IdModifyUser = modifyUser.IdUser;

                        order.UploadedFileName = SP.VcVetProfessionCorrectionNotes;

                        order.IdCreateUser = modifyUser.IdUser;

                        order.ModifyDate = DateTime.Now;

                        order.CreationDate = DateTime.Now;

                        orders.Add(order);
                        _ApplicationDbContext.Add(order);
                        _ApplicationDbContext.SaveChanges();
                    }

                    if (order.IdOrder != 0)
                    {
                        ProfessionOrder professionOrder = new ProfessionOrder();

                        professionOrder.Profession = _ApplicationDbContext.Professions.Where(x => x.IdProfession == SppooProfession.IdProfession).First();

                        professionOrder.SPPOOOrder = _ApplicationDbContext.SPPOOOrders.Where(x => x.IdOrder == order.IdOrder).First();

                        if (SP.IntVetProfessionCorrection == 1)
                        {
                            professionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Created") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SP.IntVetProfessionCorrection == 2 || SP.IntVetProfessionCorrection == 4)
                        {
                            professionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Changed") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SP.IntVetProfessionCorrection == 3)
                        {
                            professionOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Deleted") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;

                        }

                        _ApplicationDbContext.Add(professionOrder);
                        _ApplicationDbContext.SaveChanges();
                    }
                }
            }

            var code_vet_speciality = _jessieContextContext.CodeVetSpecialities.ToList();
            var specialities = this._ApplicationDbContext.Specialities.ToList();

            foreach (var SS in code_vet_speciality)
            {

                var SppooSpeciality = specialities.Where(x => x.OldId == SS.Id).First();

                if (SS.VcVetSpecialityCorrectionNotes != null)
                {
                    OrderModel oldOrder = new OrderModel(SS.VcVetSpecialityCorrectionNotes);

                    SPPOOOrder order = GetOrder(oldOrder.Order);
                    if (order.OrderNumber.Equals(string.Empty) && !string.IsNullOrEmpty(oldOrder.Date))
                    {
                        order.OrderNumber = oldOrder.Order;

                        if (!oldOrder.Date.Equals(""))
                            order.OrderDate = DateTime.ParseExact(oldOrder.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        //order.IdModifyUser = modifyUser.IdUser;

                        order.UploadedFileName = SS.VcVetSpecialityCorrectionNotes;

                        order.ModifyDate = DateTime.Now;

                        //order.IdCreateUser = modifyUser.IdUser;

                        order.CreationDate = DateTime.Now;

                        orders.Add(order);
                        _ApplicationDbContext.Add(order);
                        _ApplicationDbContext.SaveChanges();
                    }

                    if (order.IdOrder != 0)
                    {
                        SpecialityOrder specialityOrder = new SpecialityOrder();

                        specialityOrder.SPPOOOrder = _ApplicationDbContext.SPPOOOrders.Where(x => x.IdOrder == order.IdOrder).First();

                        specialityOrder.Speciality = _ApplicationDbContext.Specialities.Where(x => x.IdSpeciality == SppooSpeciality.IdSpeciality).First();

                        if (SS.IntVetSpecialityCorrection == 1)
                        {
                            specialityOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Created") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SS.IntVetSpecialityCorrection == 2 || SS.IntVetSpecialityCorrection == 4)
                        {
                            specialityOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Changed") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;
                        }
                        else if (SS.IntVetSpecialityCorrection == 3)
                        {
                            specialityOrder.IdTypeChange = (keyValues.Where(x => x.KeyValueIntCode.Equals("Deleted") && x.KeyType.KeyTypeIntCode.Equals("SPPOOOrderChange")).First()).IdKeyValue;

                        }
                        _ApplicationDbContext.Add(specialityOrder);
                        _ApplicationDbContext.SaveChanges();
                    }
                }
            }

            LogEndInformation("ImportSPPOOrder");
        }

        #region Request
        public void ImportRequestTypeOfRequestedDocuments()
        {
            LogStrartInformation("ImportRequestTypeOfRequestedDocuments");
            if (this._ApplicationDbContext.TypeOfRequestedDocuments.Any())
            {
                this._ApplicationDbContext.TypeOfRequestedDocuments.BatchDelete();
            }
            var TypeFrameworkPrograms = (from kv in _ApplicationDbContext.KeyValues
                                         join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                         where kt.KeyTypeIntCode == "TypeFrameworkProgram"
                                         select kv).To<KeyValueVM>().ToList();

            var code_request_doc_type = _jessieContextContext.CodeRequestDocTypes.ToList();

            ConcurrentBag<TypeOfRequestedDocument> types = new ConcurrentBag<TypeOfRequestedDocument>();

            Parallel.ForEach(code_request_doc_type, dt =>
            {
                try
                {
                    TypeOfRequestedDocument docType = new TypeOfRequestedDocument();

                    docType.DocTypeOfficialNumber = dt.VcRequestDocTypeOfficialNumber;

                    docType.DocTypeName = dt.VcRequestDocTypeName;

                    if (dt.BoolIsValid != null)
                        docType.IsValid = dt.BoolIsValid.Value;
                    else
                        docType.IsValid = false;

                    if (dt.IntCurrentPeriod != null)
                        docType.CurrentPeriod = dt.IntCurrentPeriod.Value;
                    else
                        docType.CurrentPeriod = 0;

                    docType.Price = dt.IntDocPrice.Value;

                    docType.Order = dt.IntOrderId.Value;

                    if (dt.BoolHasSerialNumber != null)
                        docType.HasSerialNumber = dt.BoolHasSerialNumber.Value;
                    else
                        docType.HasSerialNumber = false;

                    docType.IsDestroyable = dt.IsDestroyable;

                    docType.ModifyDate = DateTime.Now;

                    docType.CreationDate = DateTime.Now;

                    docType.IdCreateUser = modifyUser.IdUser;

                    docType.IdModifyUser = modifyUser.IdUser;

                    docType.OldId = (int?)dt.Id;

                    switch (dt.VcRequestDocTypeOfficialNumber.Trim())
                    {
                        case "3-114":
                            docType.IdCourseType = TypeFrameworkPrograms.Where(x => x.KeyValueIntCode.Equals("CourseRegulation1And7")).First().IdKeyValue;
                            docType.MigrationNote = "3";
                            break;
                        case "3-116":
                            docType.IdCourseType = TypeFrameworkPrograms.Where(x => x.KeyValueIntCode.Equals("CourseRegulation1And7")).First().IdKeyValue;
                            break;
                        case "3-37":
                            docType.IdCourseType = TypeFrameworkPrograms.Where(x => x.KeyValueIntCode.Equals("PartProfession")).First().IdKeyValue;
                            docType.MigrationNote = "2";
                            break;
                        case "3-37В":
                            docType.IdCourseType = TypeFrameworkPrograms.Where(x => x.KeyValueIntCode.Equals("ValidationOfPartOfProfession")).First().IdKeyValue;
                            docType.MigrationNote = "10";
                            break;
                        case "3-54":
                            docType.IdCourseType = TypeFrameworkPrograms.Where(x => x.KeyValueIntCode.Equals("ProfessionalQualification")).First().IdKeyValue;
                            docType.MigrationNote = "1";
                            break;
                        case "3-54aB":
                            docType.IdCourseType = TypeFrameworkPrograms.Where(x => x.KeyValueIntCode.Equals("IssueOfDuplicate")).First().IdKeyValue;
                            docType.MigrationNote = "12";
                            break;
                        case "3-54В":
                            docType.IdCourseType = TypeFrameworkPrograms.Where(x => x.KeyValueIntCode.Equals("ValidationOfProfessionalQualifications")).First().IdKeyValue;
                            docType.MigrationNote = "9";
                            break;
                        case "3-54a":
                            docType.IdCourseType = TypeFrameworkPrograms.Where(x => x.KeyValueIntCode.Equals("IssueOfDuplicate")).First().IdKeyValue;
                            docType.MigrationNote = "11";
                            break;
                    }

                    types.Add(docType);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestTypeOfRequestedDocuments(Първи Parallel.ForEach). Запис с Id = " + dt.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            this._ApplicationDbContext.BulkInsert(types.ToList());

            var IdsToMigrate = new List<long>() { 5, 8, 6, 4, 7 };

            var code_document_type = this._jessieContextContext.CodeDocumentTypes.ToList();

            var order = this._ApplicationDbContext.TypeOfRequestedDocuments.Count();

            var bonusTypes = new ConcurrentBag<TypeOfRequestedDocument>();

            Parallel.ForEach(code_document_type, type =>
            {
                if (IdsToMigrate.Contains(type.Id))
                {
                    TypeOfRequestedDocument docType = new TypeOfRequestedDocument();

                    docType.DocTypeOfficialNumber = null;

                    docType.DocTypeName = type.VcDocumentTypeName;

                    docType.IsValid = false;

                    docType.CurrentPeriod = 0;

                    docType.Price = 0;

                    docType.HasSerialNumber = false;

                    docType.IsDestroyable = false;

                    docType.IdCourseType = null;

                    docType.Order = ++order;

                    docType.MigrationNote = type.Id.ToString();

                    bonusTypes.Add(docType);

                }
            });

            _ApplicationDbContext.BulkInsert(bonusTypes.ToList());

            LogEndInformation("ImportRequestTypeOfRequestedDocuments");
        }

        public void ImportRequestRequestDocumentStatus(int? OldId = null)
        {
            LogStrartInformation("ImportRequestRequestDocumentStatus");

            var ref_request_doc_status = new List<RefRequestDocStatus>();

            var ProviderRequestDocument = new List<ProviderRequestDocument>();

            if (OldId is null)
            {
                ProviderRequestDocument = _ApplicationDbContext.ProviderRequestDocuments.ToList();
                ref_request_doc_status = _jessieContextContext.RefRequestDocStatuses.ToList();
            }
            else
            {
                ProviderRequestDocument = (from prd in this._ApplicationDbContext.ProviderRequestDocuments
                                           join cp in this._ApplicationDbContext.CandidateProviders on prd.IdCandidateProvider equals cp.IdCandidateProviderActive
                                           where cp.OldId == OldId
                                           select prd).ToList();

                ref_request_doc_status = _jessieContextContext.RefRequestDocStatuses.Where(x => x.IntProviderId == OldId).ToList();
            }
            var candidates = _ApplicationDbContext.CandidateProviders.Where(x => x.IsActive && !x.MigrationNote.Equals("cp")).ToList();



            var statuses = (from kv in _ApplicationDbContext.KeyValues
                            join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                            where kt.KeyTypeIntCode == "RequestDocumetStatus"
                            select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<RequestDocumentStatus> DocStatuses = new ConcurrentBag<RequestDocumentStatus>();

            Parallel.ForEach(ref_request_doc_status, doc =>
            {
                try
                {
                    RequestDocumentStatusVM requestDocumentStatus = new RequestDocumentStatusVM();

                    requestDocumentStatus.IdCandidateProvider = candidates.Where(x => x.OldId == doc.IntProviderId).First().IdCandidate_Provider;

                    requestDocumentStatus.IdProviderRequestDocument = ProviderRequestDocument.Where(x => x.OldId == doc.IntRequestId).First().IdProviderRequestDocument;

                    requestDocumentStatus.IdStatus = statuses.Where(x => Int32.Parse(x.DefaultValue2) == doc.IntRequestDocStatusId).First().IdKeyValue;

                    requestDocumentStatus.ModifyDate = DateTime.Now;

                    requestDocumentStatus.CreationDate = doc.Ts.Value;

                    requestDocumentStatus.IdCreateUser = modifyUser.IdUser;

                    requestDocumentStatus.IdModifyUser = modifyUser.IdUser;

                    requestDocumentStatus.OldId = (int?)doc.Id;

                    DocStatuses.Add(requestDocumentStatus.To<RequestDocumentStatus>());
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestRequestDocumentStatus(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(DocStatuses.ToList());

            LogEndInformation("ImportRequestRequestDocumentStatus");

            UpdateRequestProviderDocumentStatus();
        }
        public void UpdateRequestProviderDocumentStatus(int? OldId = null)
        {
            LogStrartInformation("UpdateRequestProviderDocumentStatus");

            var ProviderRequestDocument = new List<ProviderRequestDocument>();
            if (OldId is null)
            {
                ProviderRequestDocument = _ApplicationDbContext.ProviderRequestDocuments.ToList();
            }
            else
            {
                ProviderRequestDocument = (from prd in this._ApplicationDbContext.ProviderRequestDocuments
                                           join cp in this._ApplicationDbContext.CandidateProviders on prd.IdCandidateProvider equals cp.IdCandidateProviderActive
                                           where cp.OldId == OldId
                                           select prd).ToList();
            }

            var StatusesFromDB = this._ApplicationDbContext.RequestDocumentStatuses.OrderByDescending(x => x.OldId).ToList();

            var ProviderRequestDocumentsToUpdate = new ConcurrentBag<ProviderRequestDocument>();

            Parallel.ForEach(ProviderRequestDocument, doc =>
            {
                var status = StatusesFromDB.Where(x => x.IdProviderRequestDocument == doc.IdProviderRequestDocument).FirstOrDefault();

                if (status is not null)
                {
                    doc.IdStatus = status.IdStatus;

                    ProviderRequestDocumentsToUpdate.Add(doc);
                }
            });

            this._ApplicationDbContext.BulkUpdate(ProviderRequestDocumentsToUpdate.ToList());
            LogEndInformation("UpdateRequestProviderDocumentStatus");
        }
        public void ImportRequestDocumentSeries()
        {
            LogStrartInformation("ImportRequestDocumentSeries");

            if (this._ApplicationDbContext.DocumentSeries.Any())
            {
                this._ApplicationDbContext.DocumentSeries.BatchDelete();
            }

            var code_request_docs_series = _jessieContextContext.CodeRequestDocsSeries.ToList();

            var typeOfDocuments = _ApplicationDbContext.TypeOfRequestedDocuments.ToList();

            var docSeries = new ConcurrentBag<DocumentSeries>();

            Parallel.ForEach(code_request_docs_series, ds =>
            {
                try
                {
                    DocumentSeries document = new DocumentSeries();

                    document.Year = ds.IntDocYear.Value;

                    document.SeriesName = ds.VcSeriesName;

                    document.ModifyDate = DateTime.Now;

                    document.CreationDate = DateTime.Now;

                    document.IdCreateUser = modifyUser.IdUser;

                    document.IdModifyUser = modifyUser.IdUser;

                    document.OldId = (int?)ds.Id;

                    if (ds.IntDocType != null)
                        document.IdTypeOfRequestedDocument = typeOfDocuments.Where(x => x.OldId == ds.IntDocType).First().IdTypeOfRequestedDocument;

                    docSeries.Add(document);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestDocumentSeries(Първи Parallel.ForEach). Запис с Id = " + ds);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(docSeries.ToList());
            LogEndInformation("ImportRequestDocumentSeries");
        }

        public void ImportRequestNapooRequestDoc(bool MigrateDocuments = true)
        {
            LogStrartInformation("ImportRequestNapooRequestDoc");

            if (this._ApplicationDbContext.NAPOORequestDocs.Any())
            {
                this._ApplicationDbContext.NAPOORequestDocs.BatchDelete();
            }

            var tb_napoo_request_doc = _jessieContextContext.TbNapooRequestDocs.ToList();

            var NAPOORequestDocs = new ConcurrentBag<NAPOORequestDoc>();

            Parallel.ForEach(tb_napoo_request_doc, rd =>
            {
                try
                {
                    NAPOORequestDoc requestDoc = new NAPOORequestDoc();

                    requestDoc.RequestDate = rd.DtRequestDate;

                    requestDoc.RequestYear = rd.IntRequestYear;

                    if (rd.BoolIsSent != null)
                        requestDoc.IsSent = (bool)rd.BoolIsSent;
                    else
                        requestDoc.IsSent = false;

                    requestDoc.ModifyDate = DateTime.Now;

                    requestDoc.CreationDate = DateTime.Now;

                    requestDoc.IdCreateUser = modifyUser.IdUser;

                    requestDoc.IdModifyUser = modifyUser.IdUser;

                    requestDoc.OldId = (int)rd.Id;

                    requestDoc.NAPOORequestNumber = rd.Id;

                    requestDoc.UploadedFileName = "Not implemented";

                    requestDoc.MigrationNote = rd.OidRequestPdf.ToString();

                    requestDoc.CreationDate = rd.Ts.Value;

                    NAPOORequestDocs.Add(requestDoc);

                    //var url = $"\\UploadedFiles\\Documents\\NapooRequestDoc\\{db.IdNAPOORequestDoc}";
                    //if (rd.OidRequestPdf != null)
                    //{
                    //    SaveDocument((int)rd.OidRequestPdf, url);
                    //    db.UploadedFileName = url;

                    //    _ApplicationDbContext.Update(db);
                    //    _ApplicationDbContext.SaveChanges();
                    //}
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestNapooRequestDoc(Първи Parallel.ForEach). Запис с Id = " + rd.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(NAPOORequestDocs.ToList());

            var docs = this._ApplicationDbContext.NAPOORequestDocs.ToList();

            var updateDocs = new List<NAPOORequestDoc>();

            if (MigrateDocuments)
            {
                foreach (var doc in docs)
                {
                    try
                    {
                        var url = $"\\UploadedFiles\\NAPOORequestDoc\\{doc.IdNAPOORequestDoc}\\";
                        if (!string.IsNullOrEmpty(doc.MigrationNote))
                        {
                            SaveDocument(Int32.Parse(doc.MigrationNote), url);
                            doc.UploadedFileName = url;

                            updateDocs.Add(doc);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                };
                _ApplicationDbContext.BulkUpdate(updateDocs);
            }
            LogEndInformation("ImportRequestNapooRequestDoc");
        }

        public void ImportRequestProviderRequestDocument(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            LogStrartInformation("ImportRequestProviderRequestDocument");

            var tb_providers_request_doc = new List<TbProvidersRequestDoc>();

            var candidates = _ApplicationDbContext.CandidateProviders.Where(x => !x.MigrationNote.Equals("cp") && x.IsActive).ToList();

            var NAPOORequestDocs = _ApplicationDbContext.NAPOORequestDocs.ToList();
            if (OldId is null)
            {
                tb_providers_request_doc = _jessieContextContext.TbProvidersRequestDocs.ToList();
            }
            else
            {
                var IsProvider = candidates.Where(x => x.OldId == OldId).FirstOrDefault();
                if (IsProvider is null)
                    return;

                tb_providers_request_doc = _jessieContextContext.TbProvidersRequestDocs.Where(x => x.IntProviderId == OldId).ToList();
            }

            var requestDocs = new ConcurrentBag<ProviderRequestDocument>();
            if (!OnlyDocuments)
            {
                Parallel.ForEach(tb_providers_request_doc, rd =>
                {
                    try
                    {
                        ProviderRequestDocument providerRequest = new ProviderRequestDocument();

                        var provider = candidates.Where(x => x.OldId == rd.IntProviderId).First();

                        providerRequest.IdCandidateProvider = provider.IdCandidate_Provider;

                        if (rd.IntNapooRequestId != null)
                            providerRequest.IdNAPOORequestDoc = NAPOORequestDocs.Where(x => x.OldId == rd.IntNapooRequestId).First().IdNAPOORequestDoc;

                        providerRequest.CurrentYear = rd.IntCurrentYear;

                        providerRequest.RequestDate = rd.DtRequestDoc;

                        if (rd.VcPosition != null)
                            providerRequest.Position = rd.VcPosition;
                        else
                            providerRequest.Position = "";
                        if (rd.VcName != null)
                            providerRequest.Name = rd.VcName;
                        else
                            providerRequest.Name = "";

                        if (rd.VcAddress != null)
                            providerRequest.Address = rd.VcAddress;
                        else
                            providerRequest.Address = "";

                        if (rd.VcTelephone != null)
                            providerRequest.Telephone = rd.VcTelephone;
                        else
                            providerRequest.Telephone = "";

                        if (rd.BoolIsSent != null)
                            providerRequest.IsSent = (bool)rd.BoolIsSent;
                        else
                            providerRequest.IsSent = false;

                        providerRequest.ModifyDate = DateTime.Now;

                        providerRequest.CreationDate = rd.Ts.Value;

                        providerRequest.IdCreateUser = modifyUser.IdUser;

                        providerRequest.IdModifyUser = modifyUser.IdUser;

                        providerRequest.OldId = (int)rd.Id;

                        providerRequest.IdLocationCorrespondence = provider.IdLocationCorrespondence;

                        providerRequest.UploadedFileName = "";

                        providerRequest.MigrationNote = rd.OidRequestPdf.ToString();

                        providerRequest.RequestNumber = rd.Id;

                        //Да разберем какво да правим с файловете от тяхната база данни
                        //providerRequest.UploadedFileName = "Not Implemented";
                        //var providerDB = providerRequest.To<ProviderRequestDocument>();
                        //_ApplicationDbContext.Add(providerDB);
                        //_ApplicationDbContext.SaveChanges();
                        //var url = $"\\UploadedFiles\\Documents\\ProviderRequestDocuments\\{providerDB.IdProviderRequestDocument}";
                        //if (rd.OidRequestPdf != null)
                        //{
                        //    SaveDocument((int)rd.OidRequestPdf, url);
                        //    providerDB.UploadedFileName = url;

                        //    _ApplicationDbContext.Update(providerDB);
                        //    _ApplicationDbContext.SaveChanges();
                        //}

                        requestDocs.Add(providerRequest);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportProviderDocuments(Първи Parallel.ForEach). Запис с Id = " + rd.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                this._ApplicationDbContext.BulkInsert(requestDocs.ToList());
            }

            LogEndInformation("ImportRequestProviderRequestDocument");

            if (MigrateDocuments)
            {
                //var docs = new List<ProviderRequestDocument>();
                //if (OldId is null)
                //{
                //    docs = this._ApplicationDbContext.ProviderRequestDocuments.ToList();
                //}
                //else
                //{
                //    var provider = this._ApplicationDbContext.CandidateProviders.Where(x => x.OldId == OldId && x.IsActive && !x.MigrationNote.Equals("cp")).First();

                //    docs = this._ApplicationDbContext.ProviderRequestDocuments.Where(x => x.IdCandidateProvider == provider.IdCandidate_Provider).ToList();
                //}

                //var updateDocs = new List<ProviderRequestDocument>();

                //foreach (var doc in docs)
                //{
                //    try
                //    {
                //        var url = $"\\UploadedFiles\\ProviderRequestDocument\\{doc.IdProviderRequestDocument}\\";
                //        if (!string.IsNullOrEmpty(doc.MigrationNote))
                //        {
                //            SaveDocument(Int32.Parse(doc.MigrationNote), url);
                //            doc.UploadedFileName = url;

                //            updateDocs.Add(doc);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        this.logger.LogInformation("Гръмна метод ImportRequestProviderRequestDocument(Първи foreach). Запис с Id = " + doc.IdProviderRequestDocument);
                //        this.logger.LogError(ex.Message);
                //        this.logger.LogError(ex.InnerException?.Message);
                //        this.logger.LogError(ex.StackTrace);
                //    }
                //};

                //_ApplicationDbContext.BulkUpdate(updateDocs);
                if (OldId is null)
                    ProviderRequestDocumentMigrateDocuments();
                else
                    ProviderRequestDocumentMigrateDocuments(OldId);
            }
        }

        public void ImportRequestRequestDocumentType(int? OldId = null)
        {
            LogStrartInformation("ImportRequestRequestDocumentType");

            var ref_request_doc_type = new List<RefRequestDocType>();
            if (OldId is null)
                ref_request_doc_type = _jessieContextContext.RefRequestDocTypes.ToList();
            else
                ref_request_doc_type = _jessieContextContext.RefRequestDocTypes.Where(x => x.IntProviderId == OldId).ToList();

            var candidates = _ApplicationDbContext.CandidateProviders.Where(x => x.IsActive && !x.MigrationNote.Equals("cp")).ToList();

            var TypeOfRequestedDocuments = _ApplicationDbContext.TypeOfRequestedDocuments.ToList();

            var ProviderRequestDocuments = _ApplicationDbContext.ProviderRequestDocuments.ToList();

            var RequestDocumentManagements = this._ApplicationDbContext.RequestDocumentManagements.ToList().OrderByDescending(x => x.DocumentDate).ToList();

            var ActionTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "ActionType"
                               select kv).To<KeyValueVM>().ToList();

            var Recieved = ActionTypes.Where(x => x.DefaultValue2.Equals("1")).First();

            var AwaitingConfirmation = ActionTypes.Where(x => x.DefaultValue2.Equals("6")).First();

            ConcurrentBag<RequestDocumentType> docTypes = new ConcurrentBag<RequestDocumentType>();

            Parallel.ForEach(ref_request_doc_type, dt =>
            {
                try
                {
                    RequestDocumentType documentType = new RequestDocumentType();

                    var requestDoc = ProviderRequestDocuments
                        .Where(x => x.OldId == dt.IntRequestId)
                        .First();

                    var management = RequestDocumentManagements
                    .Where(x => (x.IdDocumentOperation == Int32.Parse(Recieved.DefaultValue2) || x.IdDocumentOperation == Int32.Parse(AwaitingConfirmation.DefaultValue2)) &&
                    x.IdProviderRequestDocument == requestDoc.IdProviderRequestDocument)
                    .FirstOrDefault();

                    if (management is not null)
                        documentType.IdRequestDocumentManagement = management.IdRequestDocumentManagement;

                    documentType.IdCandidateProvider = candidates
                        .Where(x => x.OldId == dt.IntProviderId)
                        .First()
                        .IdCandidate_Provider;

                    documentType.IdProviderRequestDocument = requestDoc
                        .IdProviderRequestDocument;

                    documentType.IdTypeOfRequestedDocument = TypeOfRequestedDocuments
                        .Where(x => x.OldId == dt.IntRequestDocTypeId)
                        .First()
                        .IdTypeOfRequestedDocument;

                    if (dt.IntDocCount != null)
                        documentType.DocumentCount = (int)dt.IntDocCount;
                    else
                        documentType.DocumentCount = 0;

                    documentType.ModifyDate = DateTime.Now;

                    documentType.CreationDate = DateTime.Now;

                    documentType.IdCreateUser = modifyUser.IdUser;

                    documentType.IdModifyUser = modifyUser.IdUser;

                    documentType.OldId = (int)dt.Id;

                    docTypes.Add(documentType);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestRequestDocumentType(Първи Parallel.ForEach). Запис с Id = " + dt.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(docTypes.ToList());

            LogEndInformation("ImportRequestRequestDocumentType");

        }

        public void ImportRequestDocumentManagement(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            //OldId = 2929;

            LogStrartInformation("ImportRequestDocumentManagement");

            var tb_request_docs_management = new List<TbRequestDocsManagement>();

            if (OldId is null)
                tb_request_docs_management = _jessieContextContext.TbRequestDocsManagements.ToList();
            else
                tb_request_docs_management = _jessieContextContext.TbRequestDocsManagements.Where(x => x.IntProviderId == OldId).ToList();

            var candidates = _ApplicationDbContext.CandidateProviders.ToList();

            var ProviderRequestDocuments = _ApplicationDbContext.ProviderRequestDocuments.ToList();

            var TypeOfRequestedDocuments = _ApplicationDbContext.TypeOfRequestedDocuments.ToList();

            var tb_request_docs_sn = _jessieContextContext.TbRequestDocsSns.ToList();

            var ActionTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "ActionType"
                               select kv).To<KeyValueVM>().ToList();

            var TypeReportUploadedDocuments = (from kv in _ApplicationDbContext.KeyValues
                                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                               where kt.KeyTypeIntCode == "TypeReportUploadedDocument" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                               select kv).To<KeyValueVM>().ToList();

            var DocumentRequestReceiveTypes = (from kv in _ApplicationDbContext.KeyValues
                                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                               where kt.KeyTypeIntCode == "DocumentRequestReceiveType"
                                               select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<RequestDocumentManagement> docs = new ConcurrentBag<RequestDocumentManagement>();
            if (!OnlyDocuments)
            {
                Parallel.ForEach(tb_request_docs_management, doc =>
                {
                    try
                    {
                        RequestDocumentManagementVM documentManagement = new RequestDocumentManagementVM();

                        if (doc.IntRequestId != null && doc.IntRequestId != 0)
                            documentManagement.IdProviderRequestDocument = ProviderRequestDocuments
                                .Where(x => x.OldId == doc.IntRequestId)
                                .First()
                                .IdProviderRequestDocument;

                        documentManagement.IdCandidateProvider = candidates
                            .Where(x => x.OldId == doc.IntProviderId)
                            .First()
                            .IdCandidate_Provider;

                        documentManagement.IdTypeOfRequestedDocument = TypeOfRequestedDocuments
                            .Where(x => x.OldId == doc.IntRequestDocTypeId)
                            .First()
                            .IdTypeOfRequestedDocument;

                        if (doc.IntReceiveDocsCount != null)
                            documentManagement.DocumentCount = (int)doc.IntReceiveDocsCount;
                        else
                            documentManagement.DocumentCount = 0;

                        documentManagement.DocumentDate = doc.DtReceiveDocsDate;


                        documentManagement.IdDocumentOperation = ActionTypes
                            .Where(x => Int32.Parse(x.DefaultValue2) == doc.IntRequestDocsOperationId)
                            .First()
                        .IdKeyValue;

                        documentManagement.ReceiveDocumentYear = doc.IntReceiveDocsYear;

                        documentManagement.ModifyDate = DateTime.Now;

                        documentManagement.CreationDate = DateTime.Now;

                        documentManagement.IdCreateUser = modifyUser.IdUser;

                        documentManagement.IdModifyUser = modifyUser.IdUser;

                        documentManagement.OldId = (int)doc.Id;

                        if (doc.IntRequestDocsOperationId == 1 || doc.IntRequestDocsOperationId == 2 || doc.IntRequestDocsOperationId == 6)
                        {
                            if (doc.IntPartnerId != null && doc.IntPartnerId != 0)
                            {
                                documentManagement.IdCandidateProviderPartner = candidates
                                    .Where(x => x.OldId == doc.IntPartnerId)
                                    .First().IdCandidate_Provider;
                            }
                            else
                            {
                                if (doc.IntRequestDocsOperationId == 1)
                                {
                                    documentManagement.IdDocumentRequestReceiveType = DocumentRequestReceiveTypes.Where(x => x.KeyValueIntCode.Equals("PrintingHouse")).First().IdKeyValue;
                                }
                                else
                                {
                                    documentManagement.IdDocumentRequestReceiveType = DocumentRequestReceiveTypes.Where(x => x.KeyValueIntCode.Equals("OtherCPO")).First().IdKeyValue;
                                }
                            }
                        }
                        documentManagement.MigrationNote = doc.VcTbProviderUploadedDocsIds;
                        if (doc.IntRequestDocsOperationId != 6 || doc.IntPartnerId != 0)
                            docs.Add(documentManagement.To<RequestDocumentManagement>());
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportRequestDocumentManagement(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });

                _ApplicationDbContext.BulkInsert(docs.ToList());
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var management = new List<RequestDocumentManagement>();
            if (OldId is null)
                management = _ApplicationDbContext.RequestDocumentManagements.Include(x => x.CandidateProvider).ToList();
            else
                management = _ApplicationDbContext.RequestDocumentManagements.Include(x => x.CandidateProvider).Where(x => x.CandidateProvider.OldId == OldId).ToList();

            var ManagementUploadedFiles = new ConcurrentBag<RequestDocumentManagementUploadedFile>();

            var submited = (from kv in _ApplicationDbContext.KeyValues
                            join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                            where kt.KeyTypeIntCode == "RequestReportStatus" && kv.KeyValueIntCode == "Approved"
                            select kv).To<KeyValueVM>().First();

            var tb_provider_uploaded_docs = this._jessieContextContext.TbProviderUploadedDocs;

            Parallel.ForEach(management, currentManagement =>
            {
                try
                {
                    var sn = tb_request_docs_sn.Where(x => x.IntRequestDocsManagementId == currentManagement.OldId).FirstOrDefault();
                    var type = new KeyValueVM();
                    if (sn is null)
                        type = ActionTypes
                       .Where(x => x.IdKeyValue == currentManagement.IdDocumentOperation)
                       .First();
                    else
                        type = ActionTypes
                        .Where(x => Int32.Parse(x.DefaultValue2) == sn.IntRequestDocsOperationId)
                        .First();

                    if (type.KeyValueIntCode.Equals("Received") || type.KeyValueIntCode.Equals("Submitted") || type.KeyValueIntCode.Equals("AwaitingConfirmation"))
                    {
                        if (currentManagement.MigrationNote != null)
                        {
                            foreach (var oid in currentManagement.MigrationNote.Split(","))
                            {
                                if (!string.IsNullOrEmpty(oid))
                                {
                                    RequestDocumentManagementUploadedFile file = new RequestDocumentManagementUploadedFile();

                                    file.ModifyDate = DateTime.Now;

                                    file.CreationDate = DateTime.Now;

                                    file.IdCreateUser = modifyUser.IdUser;

                                    file.IdModifyUser = modifyUser.IdUser;

                                    file.IdRequestDocumentManagement = currentManagement.IdRequestDocumentManagement;

                                    file.RequestDocumentManagement = null;

                                    file.MigrationNote = oid;
                                    file.UploadedFileName = "";

                                    ManagementUploadedFiles.Add(file);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestDocumentManagement(Първи foreach). Запис с Id = " + currentManagement.IdRequestDocumentManagement);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(ManagementUploadedFiles.ToList());

            LogEndInformation("ImportRequestDocumentManagement");

            if (MigrateDocuments)
            {
                //var managementFromDB = new List<RequestDocumentManagementUploadedFile>();

                //if (OldId is null)
                //    managementFromDB = _ApplicationDbContext.RequestDocumentManagementUploadedFiles.Include(x => x.RequestDocumentManagement.CandidateProvider).ToList();
                //else
                //    managementFromDB = _ApplicationDbContext.RequestDocumentManagementUploadedFiles.Include(x => x.RequestDocumentManagement.CandidateProvider).Where(x => x.RequestDocumentManagement.CandidateProvider.OldId == OldId).ToList();

                ////                                            var url = $"\\UploadedFiles\\RequestDocumentManagementUploadedFile\\{file.IdRequestDocumentManagementUploadedFile}";
                ////var doc = tb_provider_uploaded_docs.Where(x => x.Id == int.Parse(oid)).First();

                ////SaveBinDocument(url, doc.BinFile, doc.TxtFileName);
                ////file.UploadedFileName = url + "\\" + doc.TxtFileName;

                ////file.Description = doc.TxtFileName;

                ////file.CreationDate = doc.DtDocUploadDate.Value;

                //var ManagementForUpdate = new List<RequestDocumentManagementUploadedFile>();

                //foreach (var currentManagement in managementFromDB)
                //{
                //    var url = $"\\UploadedFiles\\RequestDocumentManagementUploadedFile\\{currentManagement.IdRequestDocumentManagementUploadedFile}";
                //    var doc = tb_provider_uploaded_docs.Where(x => x.Id == int.Parse(currentManagement.MigrationNote)).First();

                //    SaveBinDocument(url, doc.BinFile, doc.TxtFileName);
                //    currentManagement.UploadedFileName = url + "\\" + doc.TxtFileName;

                //    currentManagement.Description = doc.TxtFileName;

                //    currentManagement.CreationDate = doc.DtDocUploadDate.Value;

                //    ManagementForUpdate.Add(currentManagement);
                //}

                //this._ApplicationDbContext.BulkUpdate(ManagementForUpdate.ToList());
                if (OldId is null)
                    RequestDocumentManagementMigrateDocuments();
                else
                    RequestDocumentManagementMigrateDocuments(OldId);
            }
        }

        public void ImportRequestReport(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            //OldId = 1664;
            LogStrartInformation("ImportRequestReport");

            var TypeReportUploadedDocument = (from kv in _ApplicationDbContext.KeyValues
                                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                              where kt.KeyTypeIntCode == "TypeReportUploadedDocument" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                              select kv).To<KeyValueVM>().ToList();

            var RequestReportStatus = (from kv in _ApplicationDbContext.KeyValues
                                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                       where kt.KeyTypeIntCode == "RequestReportStatus" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                       select kv).To<KeyValueVM>().ToList();

            var Approved = (from kv in _ApplicationDbContext.KeyValues
                            join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                            where kt.KeyTypeIntCode == "RequestReportStatus" && kv.KeyValueIntCode == "Approved"
                            select kv).To<KeyValueVM>().First();

            var tb_provider_uploaded_docs = new List<TbProviderUploadedDoc>();
            if (OldId is null)
                tb_provider_uploaded_docs = this._jessieContextContext.TbProviderUploadedDocs
                    .Where(x => x.IntUploadDocTypeId == 1 || x.IntUploadDocTypeId == 2)
                    .ToList().OrderBy(x => x.Id).ToList();
            else
                tb_provider_uploaded_docs = this._jessieContextContext.TbProviderUploadedDocs
                    .Where(x => x.IntProviderId == OldId && (x.IntUploadDocTypeId == 1 || x.IntUploadDocTypeId == 2))
                    .ToList().OrderBy(x => x.Id).ToList();

            var candidates = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive && !x.MigrationNote.Contains("cp")).ToList();

            ConcurrentBag<ReportUploadedDoc> docs = new ConcurrentBag<ReportUploadedDoc>();
            if (!OnlyDocuments)
            {
                Parallel.ForEach(tb_provider_uploaded_docs, doc =>
                {
                    try
                    {
                        ReportUploadedDoc reportUploadedDoc = new ReportUploadedDoc();

                        reportUploadedDoc.IdTypeReportUploadedDocument = TypeReportUploadedDocument.Where(x => Int32.Parse(x.DefaultValue2) == doc.IntUploadDocTypeId).First().IdKeyValue;

                        reportUploadedDoc.Description = doc.TxtDocDescription;

                        reportUploadedDoc.CreationDate = doc.DtDocUploadDate.Value;

                        reportUploadedDoc.IdCandidateProvider = candidates.Where(x => x.OldId == doc.IntProviderId).First().IdCandidate_Provider;

                        reportUploadedDoc.MigrationNote = doc.IntYear.ToString();

                        reportUploadedDoc.OldId = (int?)doc.Id;

                        reportUploadedDoc.ModifyDate = DateTime.Now;

                        reportUploadedDoc.IdCreateUser = modifyUser.IdUser;

                        reportUploadedDoc.IdModifyUser = modifyUser.IdUser;

                        docs.Add(reportUploadedDoc);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportRequestReport(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });

                _ApplicationDbContext.BulkInsert(docs.ToList());
            }
            var reportDocuments = new List<ReportUploadedDoc>();

            var reportDocUploadsToUpdate = new List<ReportUploadedDoc>();

            var requestReports = new List<RequestReport>();

            if (OldId is null)
                reportDocuments = this._ApplicationDbContext.ReportUploadedDocs.ToList().OrderBy(x => x.CreationDate).ToList();
            else
                reportDocuments = this._ApplicationDbContext.ReportUploadedDocs.Include(x => x.CandidateProvider).Where(x => x.CandidateProvider.OldId == OldId).ToList().OrderBy(x => x.CreationDate).ToList();

            //var reportsForUpdate = new List<RequestReport>();

            foreach (var reportDoc in reportDocuments)
            {
                try
                {
                    var reportExist = requestReports
                        .Where(x => x.IdCandidateProvider == reportDoc.IdCandidateProvider && x.Year == Int32.Parse(reportDoc.MigrationNote))
                        .FirstOrDefault();

                    if (reportExist is not null) continue;

                    //if (reportExist is not null)
                    //{
                    //var tb_provider_uploaded_doc = tb_provider_uploaded_docs.Where(x => x.Id == reportDoc.OldId).First();

                    //reportDoc.IdRequestReport = reportExist.IdRequestReport;

                    //if (Approved.IdKeyValue != reportExist.IdStatus)
                    //{
                    //    reportExist.IdStatus = RequestReportStatus.Where(x => Int32.Parse(x.DefaultValue2) == tb_provider_uploaded_doc.IntDocStatusId).First().IdKeyValue;
                    //}

                    //reportExist.DestructionDate = tb_provider_uploaded_doc.DtDocUploadDate.Value;

                    //this._ApplicationDbContext.Update(reportExist);

                    //}
                    //else
                    //{
                    RequestReport request = new RequestReport();

                    request.Year = Int32.Parse(reportDoc.MigrationNote);

                    request.IdCandidateProvider = reportDoc.IdCandidateProvider;

                    request.DestructionDate = reportDoc.CreationDate;

                    var tb_provider_uploaded_doc = tb_provider_uploaded_docs.Where(x => x.Id == reportDoc.OldId).First();

                    request.IdStatus = RequestReportStatus.Where(x => Int32.Parse(x.DefaultValue2) == tb_provider_uploaded_doc.IntDocStatusId).First().IdKeyValue;

                    request.ModifyDate = DateTime.Now;

                    request.CreationDate = DateTime.Now;

                    request.IdCreateUser = modifyUser.IdUser;

                    request.IdModifyUser = modifyUser.IdUser;

                    request.OldId = (int)tb_provider_uploaded_doc.Id;

                    requestReports.Add(request);

                    //this._ApplicationDbContext.Add(request);
                    //this._ApplicationDbContext.SaveChanges();

                    //reportDoc.IdRequestReport = request.IdRequestReport;
                    //}

                    //if (MigrateDocuments)
                    //{
                    //    var url = $"\\UploadedFiles\\ReportUploadedDoc\\{reportDoc.IdReportUploadedDoc}";
                    //    var doc = tb_provider_uploaded_docs.Where(x => x.Id == reportDoc.OldId).First();

                    //    SaveBinDocument(url, doc.BinFile, doc.TxtFileName);
                    //    reportDoc.UploadedFileName = url+"\\"+doc.TxtFileName;

                    //    //this._ApplicationDbContext.Update(reportDoc);
                    //    //this._ApplicationDbContext.SaveChanges();
                    //    reportDocUploadsToUpdate.Add(reportDoc);
                    //}
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestReport(Първи foreach). Запис с Id = " + reportDoc.IdReportUploadedDoc);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }

            }

            this._ApplicationDbContext.BulkInsert(requestReports);

            LogEndInformation("ImportRequestReport");

            if (MigrateDocuments)
            {
                //var requestReportsFromDb = new List<RequestReport>();

                //if (OldId is null)
                //    requestReportsFromDb = this._ApplicationDbContext.RequestReports.ToList();
                //else
                //    requestReportsFromDb = (from r in this._ApplicationDbContext.RequestReports
                //                            join cp in this._ApplicationDbContext.CandidateProviders on r.IdCandidateProvider equals cp.IdCandidate_Provider
                //                            where cp.OldId == OldId
                //                            select r).ToList();

                //foreach (var report in requestReportsFromDb)
                //{
                //    var reportDocumentsList = reportDocuments.Where(x => x.IdCandidateProvider == report.IdCandidateProvider && report.Year == Int32.Parse(x.MigrationNote)).ToList();

                //    foreach (var doc in reportDocumentsList)
                //    {
                //        doc.IdRequestReport = report.IdRequestReport;
                //        if (MigrateDocuments)
                //        {
                //            var url = $"\\UploadedFiles\\ReportUploadedDoc\\{doc.IdReportUploadedDoc}";
                //            var uploadedDoc = tb_provider_uploaded_docs.Where(x => x.Id == doc.OldId).First();

                //            SaveBinDocument(url, uploadedDoc.BinFile, uploadedDoc.TxtFileName);
                //            doc.UploadedFileName = url + "\\" + uploadedDoc.TxtFileName;

                //        }
                //        reportDocUploadsToUpdate.Add(doc);

                //    }
                //}

                //this._ApplicationDbContext.BulkUpdate(reportDocUploadsToUpdate);
                if (OldId is null)
                    RequestReportMigrateDocuments();
                else
                    RequestReportMigrateDocuments(OldId);
            }
        }

        public void ImportRequestDocumentSerialNumber(int? OldId = null)
        {
            LogStrartInformation("ImportRequestDocumentSerialNumber");

            var tb_request_docs_sn = new List<TbRequestDocsSn>();
            if (OldId is null)
                tb_request_docs_sn = _jessieContextContext.TbRequestDocsSns.ToList();
            else
                tb_request_docs_sn = _jessieContextContext.TbRequestDocsSns.Where(x => x.IntProviderId == OldId).ToList();

            var RequestDocumentManagement = _ApplicationDbContext.RequestDocumentManagements.ToList();

            var candidates = _ApplicationDbContext.CandidateProviders.ToList();

            var TypeOfRequestedDocuments = _ApplicationDbContext.TypeOfRequestedDocuments.ToList();

            var reports = this._ApplicationDbContext.RequestReports.ToList();

            var ActionTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "ActionType"
                               select kv).To<KeyValueVM>().ToList();

            var Cancelled = ActionTypes.Where(x => x.KeyValueIntCode.Equals("Cancelled")).First();
            var Destroyed = ActionTypes.Where(x => x.KeyValueIntCode.Equals("Destroyed")).First();

            ConcurrentBag<DocumentSerialNumber> docs = new ConcurrentBag<DocumentSerialNumber>();

            Parallel.ForEach(tb_request_docs_sn, doc =>
            {
                try
                {
                    DocumentSerialNumberVM documentSerial = new DocumentSerialNumberVM();

                    var managment = RequestDocumentManagement
                        .Where(x => x.OldId == doc.IntRequestDocsManagementId)
                        .First();

                    var candidate = new CandidateProvider();

                    if (doc.IntProviderId is not null)
                        candidate = candidates.Where(x => x.OldId == doc.IntProviderId).First();
                    else
                        candidate = candidates.Where(x => x.IdCandidate_Provider == managment.IdCandidateProvider).First();

                    if (doc.IntRequestDocsOperationId == Int32.Parse(Cancelled.DefaultValue2) || doc.IntRequestDocsOperationId == Int32.Parse(Destroyed.DefaultValue2))
                    {
                        var report = reports
                        .Where(x => x.IdCandidateProvider == candidate.IdCandidate_Provider && x.Year - doc.IntReceiveDocsYear.Value == 1)
                        .FirstOrDefault();

                        if (report is not null)
                        {
                            documentSerial.IdRequestReport = report.IdRequestReport;
                        }
                    }

                    documentSerial.IdRequestDocumentManagement = managment.IdRequestDocumentManagement;

                    if (doc.IntProviderId != null)
                        documentSerial.IdCandidateProvider = candidates
                            .Where(x => x.OldId == doc.IntProviderId)
                            .First()
                            .IdCandidate_Provider;

                    documentSerial.IdTypeOfRequestedDocument = TypeOfRequestedDocuments
                        .Where(x => x.OldId == doc.IntRequestDocTypeId)
                        .First()
                        .IdTypeOfRequestedDocument;

                    if (doc.VcRequestDocNumber != null)
                        documentSerial.SerialNumber = doc.VcRequestDocNumber;
                    else
                        documentSerial.SerialNumber = "";

                    documentSerial.IdDocumentOperation = ActionTypes
                        .Where(x => Int32.Parse(x.DefaultValue2) == doc.IntRequestDocsOperationId)
                        .First()
                        .IdKeyValue;

                    if (doc.IntReceiveDocsYear != null)
                        documentSerial.ReceiveDocumentYear = (int)doc.IntReceiveDocsYear;
                    else
                        documentSerial.ReceiveDocumentYear = 0;

                    documentSerial.DocumentDate = managment.DocumentDate;

                    documentSerial.ModifyDate = DateTime.Now;

                    documentSerial.CreationDate = DateTime.Now;

                    documentSerial.IdCreateUser = modifyUser.IdUser;

                    documentSerial.IdModifyUser = modifyUser.IdUser;

                    var db = documentSerial.To<DocumentSerialNumber>();

                    db.OldId = (int?)doc.Id;

                    docs.Add(db);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestDocumentSerialNumber(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(docs.ToList());

            LogEndInformation("ImportRequestDocumentSerialNumber");

        }

        public void ImportRequestProviderDocumentOffer(int? OldId = null)
        {
            LogStrartInformation("ImportRequestProviderDocumentOffer");

            var tb_providers_docs_offers = new List<TbProvidersDocsOffer>();

            if (OldId is null)
                tb_providers_docs_offers = this._jessieContextContext.TbProvidersDocsOffers.ToList();
            else
                tb_providers_docs_offers = this._jessieContextContext.TbProvidersDocsOffers.Where(x => x.IntProviderId == OldId).ToList();

            var Candidates = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive).ToList();

            var OfferTypes = (from kv in _ApplicationDbContext.KeyValues
                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                              where kt.KeyTypeIntCode == "OfferType"
                              select kv).To<KeyValueVM>().ToList();

            var DocumentOffers = new ConcurrentBag<ProviderDocumentOffer>();

            var TypeOfReqestDocuments = this._ApplicationDbContext.TypeOfRequestedDocuments.ToList();

            Parallel.ForEach(tb_providers_docs_offers, offer =>
            {
                try
                {
                    var documentOffer = new ProviderDocumentOffer();

                    documentOffer.IdCandidateProvider = Candidates.Where(x => x.OldId == offer.IntProviderId).First().IdCandidate_Provider;

                    documentOffer.IdOfferType = OfferTypes.Where(x => Int32.Parse(x.DefaultValue2) == offer.IntSeekOffer).First().IdKeyValue;

                    documentOffer.IdTypeOfRequestedDocument = TypeOfReqestDocuments.Where(x => x.OldId == offer.IntDocTypeId).First().IdTypeOfRequestedDocument;

                    documentOffer.CountOffered = offer.IntCountOffered!.Value;

                    documentOffer.OfferStartDate = offer.DtOffered!.Value;

                    documentOffer.OfferEndDate = offer.DtClosed;

                    documentOffer.ModifyDate = DateTime.Now;

                    documentOffer.CreationDate = DateTime.Now;

                    documentOffer.IdCreateUser = modifyUser.IdUser;

                    documentOffer.IdModifyUser = modifyUser.IdUser;

                    documentOffer.OldId = (int?)offer.Id;

                    DocumentOffers.Add(documentOffer);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestProviderDocumentOffer(Първи Parallel.ForEach). Запис с Id = " + offer.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(DocumentOffers.ToList());

            LogEndInformation("ImportRequestProviderDocumentOffer");
        }

        public void ImportRequestCandidateProviderDocument(int? OldId = null)
        {
            LogStrartInformation("ImportRequestCandidateProviderDocument");
            var tb_provider_uploaded_docs = new List<TbProviderUploadedDoc>();

            if (OldId is null)
                tb_provider_uploaded_docs = this._jessieContextContext.TbProviderUploadedDocs.Select(o => new TbProviderUploadedDoc
                {
                    Id = o.Id,
                    IntUploadDocTypeId = o.IntUploadDocTypeId,
                    IntYear = o.IntYear,
                    TxtDocDescription = o.TxtDocDescription,
                    IntDocStatusId = o.IntDocStatusId,
                    DtDocUploadDate = o.DtDocUploadDate,
                    TxtFileName = o.TxtFileName,
                    IntProviderId = o.IntProviderId
                }).Where(x => x.IntUploadDocTypeId == 99).ToList();
            else
                tb_provider_uploaded_docs = this._jessieContextContext.TbProviderUploadedDocs.Select(o => new TbProviderUploadedDoc
                {
                    Id = o.Id,
                    IntUploadDocTypeId = o.IntUploadDocTypeId,
                    IntYear = o.IntYear,
                    TxtDocDescription = o.TxtDocDescription,
                    IntDocStatusId = o.IntDocStatusId,
                    DtDocUploadDate = o.DtDocUploadDate,
                    TxtFileName = o.TxtFileName,
                    IntProviderId = o.IntProviderId
                }).Where(x => x.IntProviderId == OldId && (x.IntUploadDocTypeId == 99)).ToList();


            var candidates = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive).ToList();

            var documents = new List<RequestCandidateProviderDocument>();

            Parallel.ForEach(tb_provider_uploaded_docs, doc =>
            {
                try
                {
                    var document = new RequestCandidateProviderDocument();

                    document.IdCandidateProvider = candidates.Where(x => x.OldId == doc.IntProviderId).First().IdCandidate_Provider;

                    document.ModifyDate = DateTime.Now;

                    document.CreationDate = DateTime.Now;

                    document.IdCreateUser = modifyUser.IdUser;

                    document.IdModifyUser = modifyUser.IdUser;

                    document.OldId = (int?)doc.Id;

                    documents.Add(document);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportRequestCandidateProviderDocument(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(documents.ToList());

            LogEndInformation("ImportRequestCandidateProviderDocument");
        }
        #endregion

        #region Training
        public void ImportTrainingProgram(int? OldId = null)
        {
            //OldId = 2929;
            LogStrartInformation("ImportTrainingProgram");

            var tb_courses = new List<TbCourse>();

            if (OldId is null)
                tb_courses = _jessieContextContext.TbCourses.ToList();
            else
                tb_courses = _jessieContextContext.TbCourses.Where(x => x.IntProviderId == OldId).ToList();

            var candidates = _ApplicationDbContext.CandidateProviders.ToList();

            var specialities = _ApplicationDbContext.Specialities.ToList();

            var frameworkPrograms = _ApplicationDbContext.FrameworkPrograms.ToList();

            var TrainingCourseType = (from kv in _ApplicationDbContext.KeyValues
                                      join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                      where kt.KeyTypeIntCode == "TypeFrameworkProgram"
                                      select kv).To<KeyValueVM>().ToList();

            var Education = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "Education" && !string.IsNullOrEmpty(kv.DefaultValue2.Trim())
                             select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<TrainingProgram> programList = new ConcurrentBag<TrainingProgram>();

            Parallel.ForEach(tb_courses, course =>
            {
                try
                {
                    TrainingProgram program = new TrainingProgram();

                    program.ProgramNumber = course.IntCourseNo.ToString();

                    if (course.VcCourseName != null)
                        program.ProgramName = course.VcCourseName;
                    else
                        program.ProgramName = "";

                    program.ProgramNote = course.VcCourseAddNotes;

                    program.IdCandidateProvider = candidates.Where(x => x.OldId == course.IntProviderId).First().IdCandidate_Provider;

                    program.IdSpeciality = specialities.Where(x => x.OldId == course.IntVetSpecialityId).First().IdSpeciality;

                    if (course.IntCourseFrCurrId != null && course.IntCourseFrCurrId != 0)
                        program.IdFrameworkProgram = frameworkPrograms.Where(x => x.OldId == course.IntCourseFrCurrId).First().IdFrameworkProgram;

                    if (course.IntCourseTypeId != null && course.IntCourseTypeId != 0)
                        program.IdCourseType = TrainingCourseType.Where(x => Int32.Parse(x.DefaultValue2) == course.IntCourseTypeId).First().IdKeyValue;

                    if (course.IntMandatoryHours != null)
                        program.MandatoryHours = course.IntMandatoryHours.Value;
                    else
                        program.MandatoryHours = 0;

                    if (course.IntSelectableHours != null)
                        program.SelectableHours = course.IntSelectableHours.Value;
                    else
                        program.SelectableHours = 0;

                    if (course.IsDeleted != null)
                        program.IsDeleted = course.IsDeleted.Value;
                    else
                        program.IsDeleted = false;

                    program.ModifyDate = DateTime.Now;

                    program.CreationDate = DateTime.Now;

                    program.IdCreateUser = modifyUser.IdUser;

                    program.IdModifyUser = modifyUser.IdUser;

                    //if (course.IntCourseEducRequirement != null && course.IntCourseEducRequirement != 0)
                    //{
                    //    course.IntCourseEducRequirement = 30;
                    //    program.IdMinimumLevelEducation = Education.Where(x => Int32.Parse(x.DefaultValue2) == course.IntCourseEducRequirement).First().IdKeyValue;
                    //}
                    //else
                    //    program.IdMinimumLevelEducation = 0;

                    program.OldId = (int?)course.Id;

                    programList.Add(program);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingProgram(Първи Parallel.ForEach). Запис с Id = " + course.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(programList.ToList());

            LogEndInformation("ImportTrainingProgram");
        }
        public void ImportTrainingCourse(int? OldId = null)
        {
            //OldId = 1582;
            LogStrartInformation("ImportTrainingCourse");

            var tb_course_groups = new List<TbCourseGroup>();
            if (OldId is null)
            {
                tb_course_groups = _jessieContextContext.TbCourseGroups.ToList();
            }
            else
            {
                //var progs = _jessieContextContext.TbCourses.Where(x => x.IntProviderId == OldId).ToList();
                //tb_course_groups = _jessieContextContext.TbCourseGroups.ToList().Where(x => progs.Any(z => z.Id == x.IntCourseId)).ToList();
                tb_course_groups = (from cg in this._jessieContextContext.TbCourseGroups
                                    join c in this._jessieContextContext.TbCourses on cg.IntCourseId equals c.Id
                                    where c.IntProviderId == OldId
                                    select cg).ToList();
            }

            var programs = _ApplicationDbContext.Programs.ToList();

            var locationDB = _ApplicationDbContext.Locations.ToList();

            var premisies = _ApplicationDbContext.CandidateProviderPremises.ToList();

            var candidates = _ApplicationDbContext.CandidateProviders.ToList();

            var code_course_ed_form = _jessieContextContext.CodeCourseEdForms.ToList();

            var frameworkPrograms = _ApplicationDbContext.FrameworkPrograms.ToList();

            var ListForProgramsToValidate = new ConcurrentBag<TrainingProgram>();
            var ListForCourseToValidate = new ConcurrentBag<Course>();

            var statuses = (from kv in _ApplicationDbContext.KeyValues
                            join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                            where kt.KeyTypeIntCode == "CourseStatus"
                            select kv).To<KeyValueVM>().ToList();

            var measureType = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "MeasureType"
                               select kv).To<KeyValueVM>().ToList();

            var assignType = (from kv in _ApplicationDbContext.KeyValues
                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                              where kt.KeyTypeIntCode == "AssignType"
                              select kv).To<KeyValueVM>().ToList();

            var formEducation = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "FormEducation"
                                 select kv).To<KeyValueVM>().ToList();

            var CourseType = (from kv in _ApplicationDbContext.KeyValues
                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                              where kt.KeyTypeIntCode == "TypeFrameworkProgram"
                              select kv).To<KeyValueVM>().ToList();


            ConcurrentBag<Course> courseList = new ConcurrentBag<Course>();

            Parallel.ForEach(tb_course_groups, group =>
            {
                try
                {
                    Course courseVM = new Course();

                    var program = programs.Where(x => x.OldId == group.IntCourseId).First();

                    var frameworkProgram = frameworkPrograms.Where(x => x.OldId == group.IntCourseFrCurrId).FirstOrDefault();

                    courseVM.SubscribeDate = group.DtCourseSubscribeDate;

                    courseVM.IdProgram = program.IdProgram;

                    if (group.VcCourseGroupName != null)
                        courseVM.CourseName = group.VcCourseGroupName;
                    else
                        courseVM.CourseName = "";
                    if (group.VcAdditionalNotes != "null")
                        courseVM.AdditionalNotes = group.VcAdditionalNotes;
                    else
                        courseVM.AdditionalNotes = "";

                    courseVM.IdStatus = statuses.Where(x => Int32.Parse(x.DefaultValue2) == group.IntCourseStatusId).First().IdKeyValue;

                    if (group.IntCourseMeasureTypeId != null && group.IntCourseMeasureTypeId != 0)
                        courseVM.IdMeasureType = measureType.Where(x => Int32.Parse(x.DefaultValue2) == group.IntCourseMeasureTypeId).First().IdKeyValue;

                    if (group.IntCourseEdFormId != null && group.IntCourseEdFormId != 0)
                    {
                        var type = code_course_ed_form.Where(x => x.Id == group.IntCourseEdFormId).First().VcCourseEdFormName;

                        var key = formEducation.Where(x => x.Name.ToLower().Trim().Equals(type.ToLower().Trim())).FirstOrDefault();

                        if (key != null)
                            courseVM.IdFormEducation = key.IdKeyValue;
                        else
                            courseVM.IdFormEducation = formEducation.Where(x => x.KeyValueIntCode.Equals("Type7")).First().IdKeyValue;
                    }

                    if (group.IntEkatteId != null && group.IntEkatteId != 0)
                        courseVM.IdLocation = locationDB
                             .Where(x => x.LocationCode.Equals(locations
                                 .Where(m => m.Id == group.IntEkatteId)
                                 .First().VcTextCode))
                                 .First()
                                 .To<LocationVM>().idLocation;
                    courseVM.MandatoryHours = group.IntMandatoryHours;

                    courseVM.SelectableHours = group.IntSelectableHours;

                    courseVM.DurationHours = group.IntCourseDuration;

                    courseVM.Cost = group.NumCourseCost;

                    courseVM.StartDate = group.DtStartDate;

                    courseVM.EndDate = group.DtEndDate;

                    courseVM.ExamTheoryDate = group.DtExamTheoryDate;

                    if (group.IntAssignTypeId != null && group.IntAssignTypeId != 0)
                        courseVM.IdAssignType = assignType.Where(x => Int32.Parse(x.DefaultValue2) == group.IntAssignTypeId).First().IdKeyValue;

                    if (group.IntProviderPremise != null)
                        courseVM.IdCandidateProviderPremises = premisies.Where(x => x.OldId == group.IntProviderPremise).First().IdCandidateProviderPremises;

                    courseVM.DisabilityCount = group.IntPDisabilityCount;

                    courseVM.IdCandidateProvider = program.IdCandidateProvider;

                    if (group.IntCourseTypeId != null)
                        courseVM.IdTrainingCourseType = CourseType.Where(x => Int32.Parse(x.DefaultValue2) == group.IntCourseTypeId).First().IdKeyValue;
                    else
                    {
                        courseVM.IdTrainingCourseType = program.IdCourseType;
                    }

                    courseVM.CourseNameEN = ConvertCyrToLatin(courseVM.CourseName);

                    courseVM.ExamPracticeDate = group.DtExamPracticeDate;

                    courseVM.IdModifyUser = modifyUser.IdUser;

                    courseVM.IdCreateUser = modifyUser.IdUser;

                    courseVM.ModifyDate = DateTime.Now;

                    courseVM.CreationDate = DateTime.Now;

                    courseVM.OldId = (int)group.Id;

                    //_ApplicationDbContext.Add(course);
                    //_ApplicationDbContext.SaveChanges();
                    if (frameworkProgram is not null && program.IdFrameworkProgram != frameworkProgram.IdFrameworkProgram)
                        ListForCourseToValidate.Add(courseVM);
                    else
                        courseList.Add(courseVM);

                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingCourse(Първи Parallel.ForEach). Запис с Id = " + group.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            var programsToCreate = new List<TrainingProgram>();

            foreach (var course in ListForCourseToValidate)
            {
                try
                {
                    var group = tb_course_groups.Where(x => x.Id == course.OldId).First();

                    var frameworkProgram = frameworkPrograms.Where(x => x.OldId == group.IntCourseFrCurrId).First();

                    var program = programs.Where(x => x.OldId == group.IntCourseId).First();

                    if (program.IdFrameworkProgram == frameworkProgram.IdFrameworkProgram)
                    {
                        course.IdProgram = program.IdProgram;
                    }
                    else
                    {
                        var ProgramExist = ListForProgramsToValidate.Where(x => x.OldId == group.IntCourseId && x.IdFrameworkProgram == frameworkProgram.IdFrameworkProgram).FirstOrDefault();
                        if (ProgramExist is null)
                        {
                            var programVM = program.To<ProgramVM>();
                            programVM.IdProgram = 0;
                            programVM.IdFrameworkProgram = frameworkProgram.IdFrameworkProgram;
                            programVM.FrameworkProgram = null;
                            programVM.CandidateProvider = null;
                            programVM.SelectableHours = program.SelectableHours;
                            programVM.MandatoryHours = program.MandatoryHours;
                            var prog = programVM.To<TrainingProgram>();
                            prog.IsService = true;
                            prog.OldId = (int)group.IntCourseId;
                            prog.Speciality = null;
                            course.IdProgram = null;
                            //this._ApplicationDbContext.Add(prog);
                            //this._ApplicationDbContext.SaveChanges();
                            //course.IdProgram = prog.IdProgram;
                            ListForProgramsToValidate.Add(prog);
                        }
                        //else
                        //{
                        //    course.IdProgram = ProgramExist.IdProgram;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingCourse(Първи foreach). Запис с Id = " + course.IdCourse);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            }
            this._ApplicationDbContext.BulkInsert(ListForProgramsToValidate.ToList());

            var programsFromDb = new List<TrainingProgram>();

            if (OldId is null)
                programsFromDb = this._ApplicationDbContext.Programs.ToList();
            else
                programsFromDb = (from p in this._ApplicationDbContext.Programs
                                  join cp in this._ApplicationDbContext.CandidateProviders on p.IdCandidateProvider equals cp.IdCandidate_Provider
                                  where cp.OldId == OldId
                                  select p).ToList();

            foreach (var course in ListForCourseToValidate)
            {
                if (course.IdProgram is not null) continue;

                var group = tb_course_groups.Where(x => x.Id == course.OldId).First();

                var program = programs.Where(x => x.OldId == group.IntCourseId).First();

                var frameworkProgram = frameworkPrograms.Where(x => x.OldId == group.IntCourseFrCurrId).First();

                var Program = programsFromDb.Where(x => x.OldId == group.IntCourseId && x.IdFrameworkProgram == frameworkProgram.IdFrameworkProgram).First();

                course.IdProgram = Program.IdProgram;
            }

            _ApplicationDbContext.BulkInsert(courseList.ToList());
            _ApplicationDbContext.BulkInsert(ListForCourseToValidate.ToList());

            LogEndInformation("ImportTrainingCourse");
        }
        //Най вероятно няма да се ползва
        public void importTrainingCourse40()
        {
            var tb_course_groups = _jessieContextContext.TbCourseGroups40s.ToList();

            var code_course_ed_form = _jessieContextContext.CodeCourseEdForms.ToList();

            var programs = _ApplicationDbContext.Programs.ToList();

            var speciality = _ApplicationDbContext.Specialities.ToList();

            var locationDB = _ApplicationDbContext.Locations.ToList();

            var premisies = _ApplicationDbContext.CandidateProviderPremises.ToList();

            var candidates = _ApplicationDbContext.CandidateProviders.ToList();

            var ref_clients_courses = _jessieContextContext.RefClientsCourses.ToList();

            var clients = _ApplicationDbContext.Clients.ToList();

            var frameworkPrograms = _ApplicationDbContext.FrameworkPrograms.ToList();

            var statuses = (from kv in _ApplicationDbContext.KeyValues
                            join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                            where kt.KeyTypeIntCode == "CourseStatus"
                            select kv).To<KeyValueVM>().ToList();

            var measureType = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "MeasureType"
                               select kv).To<KeyValueVM>().ToList();

            var assignType = (from kv in _ApplicationDbContext.KeyValues
                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                              where kt.KeyTypeIntCode == "AssignType"
                              select kv).To<KeyValueVM>().ToList();

            var formEducation = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "FormEducation"
                                 select kv).To<KeyValueVM>().ToList();

            var CourseType = (from kv in _ApplicationDbContext.KeyValues
                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                              where kt.KeyTypeIntCode == "TypeFrameworkProgram"
                              select kv).To<KeyValueVM>().ToList();

            var kvSPK = CourseType.Where(x => x.KeyValueIntCode.Equals("ProfessionalQualification")).First();
            var kvPartProfession = CourseType.Where(x => x.KeyValueIntCode.Equals("PartProfession")).First();

            ConcurrentBag<ValidationClient> courseList = new ConcurrentBag<ValidationClient>();

            RequestMeasure requestMeasureGlobal = new RequestMeasure("Първи foreach");

            Parallel.ForEach(tb_course_groups, group =>
            {
                ValidationClientVM client = new ValidationClientVM();

                if (group.IntEkatteId != null && group.IntEkatteId != 0)
                    client.IdCityOfBirth = locationDB
                        .Where(x => x.kati.Equals(locations
                        .Where(m => m.Id == group.IntEkatteId)
                        .First().VcName))
                        .First()
                        .To<LocationVM>().idLocation;

                client.Cost = group.NumCourseCost;

                client.StartDate = group.DtStartDate;

                client.EndDate = group.DtEndDate;

                client.ExamTheoryDate = group.DtExamTheoryDate;

                client.ExamPracticeDate = group.DtExamPracticeDate;

                if (group.IntVetSpecialityId != null)
                    client.IdSpeciality = speciality.Where(x => x.OldId == group.IntVetSpecialityId).First().IdSpeciality;

                if (group.IntCourseTypeId == 11)
                    client.IdCourseType = kvSPK.IdKeyValue;
                else
                    client.IdCourseType = kvPartProfession.IdKeyValue;

                //IntClientId -> ref_clients_courses
                var cc = ref_clients_courses.Where(x => x.Id == group.IntClientId).First();

                var cl = clients.Where(x => x.OldId == cc.IntClientId).First();

                client.IdClient = cl.IdClient;

                client.FirstName = cl.FirstName;

                client.FamilyName = cl.FamilyName;

                client.SecondName = cl.SecondName;

                client.IdSex = cl.IdSex;

                client.IdIndentType = cl.IdIndentType;

                client.Indent = cl.Indent;

                client.BirthDate = cl.BirthDate;

                client.IdNationality = cl.IdNationality;

                client.IdCandidateProvider = cl.IdCandidateProvider;
                if (group.IntCourseFrCurrId != null && group.IntCourseFrCurrId != 0)
                    client.IdFrameworkProgram = frameworkPrograms
                    .Where(x => x.OldId == group.IntCourseFrCurrId)
                    .First().IdFrameworkProgram;

                if (group.IntAssignTypeId != null && group.IntAssignTypeId != 0)
                    client.IdAssignType = assignType
                    .Where(x => int.Parse(x.DefaultValue2) == group.IntAssignTypeId)
                    .First().IdKeyValue;

                client.StartDate = group.DtStartDate;

                client.EndDate = group.DtEndDate;

                client.IdModifyUser = modifyUser.IdUser;

                client.IdCreateUser = modifyUser.IdUser;

                client.ModifyDate = DateTime.Now;

                client.CreationDate = DateTime.Now;

                client.OldId = (int)cc.Id;

                //_ApplicationDbContext.Add(course);
                //_ApplicationDbContext.SaveChanges();

                courseList.Add(client.To<ValidationClient>());
            });

            _ApplicationDbContext.BulkInsert(courseList.ToList());
        }
        public void ImportCourseCommisionMembers(int? OldId = null)
        {
            LogStrartInformation("ImportCourseCommisionMembers");

            //OldId = 1664;
            var courses = _ApplicationDbContext.Courses.ToList();
            var tb_course_groups = new List<TbCourseGroup>();
            if (OldId is null)
            {
                tb_course_groups = _jessieContextContext.TbCourseGroups.ToList();
            }
            else
            {
                var tb_course = this._jessieContextContext.TbCourses.Where(x => x.IntProviderId == OldId).ToList();
                tb_course_groups = _jessieContextContext.TbCourseGroups.ToList().Where(x => tb_course.Any(z => z.Id == x.IntCourseId)).ToList();
            }

            ConcurrentBag<CourseCommissionMember> members = new ConcurrentBag<CourseCommissionMember>();

            Parallel.ForEach(tb_course_groups, group =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(group.VcExamCommMembers))
                    {
                        CourseCommissionMember comisionMember = new CourseCommissionMember();

                        comisionMember.FirstName = "";

                        comisionMember.FamilyName = "";

                        comisionMember.CommissionMembersFromOldIS = group.VcExamCommMembers;

                        comisionMember.IdModifyUser = modifyUser.IdUser;

                        comisionMember.IdCreateUser = modifyUser.IdUser;

                        comisionMember.ModifyDate = DateTime.Now;

                        comisionMember.CreationDate = DateTime.Now;

                        comisionMember.IdCourse = courses.Where(x => x.OldId == group.Id).First().IdCourse;

                        members.Add(comisionMember);

                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportCourseCommisionMembers(Първи Parallel.ForEach). Запис с Id = " + group.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            try
            {
                _ApplicationDbContext.BulkInsert(members.ToList());
            }
            catch (Exception ex) { }
            LogEndInformation("ImportCourseCommisionMembers");
        }
        public void ImportTrainingCurriculumTheory(int? OldId = null)
        {
            LogStrartInformation("ImportTrainingCurriculumTheory");

            //OldId = 1664;

            var CandidateProviderSpeciality = _ApplicationDbContext.CandidateProviderSpecialities.ToList();
            var programs = _ApplicationDbContext.Programs.ToList();
            var course = _ApplicationDbContext.Courses.ToList();
            var validationClients = _ApplicationDbContext.ValidationClients.ToList();
            var code_curric_hours_type = _jessieContextContext.CodeCurricHoursTypes.ToList();

            var ProfessionalTraining = (from kv in _ApplicationDbContext.KeyValues
                                        join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                        where kt.KeyTypeIntCode == "ProfessionalTraining"
                                        select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<TrainingCurriculum> courseCurriculum = new ConcurrentBag<TrainingCurriculum>();
            ConcurrentBag<int> counter = new ConcurrentBag<int>();

            var theoryModules = _jessieContextContext.TbCurricModules.Where(x => x.IntTrainingType != 2 && x.IsValid.Value)
                .ToList()
                .Select(x => new
                {
                    Id = x.Id,
                    IntTrainingType = x.IntTrainingType,
                    IntCourseId = x.IntCourseId,
                    IntCourseGroupId = x.IntCourseGroupId,
                    VcModuleName = x.VcModuleName,
                    IntCurricHoursType = x.IntCurricHoursType,
                    IntHours = x.IntHours,
                    IntCurricOrder = x.IntCurricOrder
                })
                .GroupBy(e => new { e.IntCourseId, e.IntCourseGroupId, e.VcModuleName })
                .Select(g => new
                {
                    IntCurricOrder = g.Select(x => x.IntCurricOrder).First(),
                    IntCourseId = g.Key.IntCourseId,
                    IntCourseGroupId = g.Key.IntCourseGroupId,
                    VcModuleName = g.Key.VcModuleName,
                    IntHours = g.Sum(x => x.IntHours),
                    IntTrainingType = g.Select(x => x.IntTrainingType).First(),
                    IntCurricHoursType = g.Select(x => x.IntCurricHoursType).First(),
                    OldIds = String.Join(" ", g.Select(x => x.Id).ToArray())
                }).ToList();

            if (OldId is not null)
            {
                var courses = this._jessieContextContext.TbCourses.Where(x => x.IntProviderId == OldId).ToList();

                theoryModules = theoryModules.Where(x => courses.Any(z => z.Id == x.IntCourseId)).ToList();
                //theoryModules = theoryModules.Where(x => x.IntCourseGroupId == 157846).ToList();
            }
            //Gurmi TrainingCourse = 176391

            Parallel.ForEach(theoryModules, module =>
            {

                try
                {
                    var program = programs.Where(x => x.OldId == module.IntCourseId).First();
                    var candidateSpeciality = CandidateProviderSpeciality.Where(x => x.IdSpeciality == program.IdSpeciality && x.IdCandidate_Provider == program.IdCandidateProvider).FirstOrDefault();
                    if (candidateSpeciality is not null)
                    {
                        //if (module.IntCourseGroupId is not null)
                        //{
                        TrainingCurriculum curriculum = new TrainingCurriculum();

                        curriculum.Theory = 0;

                        curriculum.IdProgram = programs.Where(x => x.OldId == module.IntCourseId).First().IdProgram;

                        if (module.IntCourseGroupId is not null)
                            curriculum.IdCourse = course.Where(x => x.OldId == module.IntCourseGroupId).First().IdCourse;

                        curriculum.Subject = module.VcModuleName.Trim();

                        curriculum.Topic = module.VcModuleName.Trim();

                        var training = ProfessionalTraining.Where(x => x.DefaultValue2.Contains(module.IntCurricHoursType.ToString())).First();

                        curriculum.IdProfessionalTraining = training.IdKeyValue;

                        curriculum.Theory = module.IntHours;

                        curriculum.MigrationNote = module.OldIds;

                        curriculum.Order = module.IntCurricOrder;

                        curriculum.IdModifyUser = modifyUser.IdUser;

                        curriculum.IdCreateUser = modifyUser.IdUser;

                        curriculum.ModifyDate = DateTime.Now;

                        curriculum.CreationDate = DateTime.Now;

                        curriculum.IdCandidateProviderSpeciality = candidateSpeciality.IdCandidateProviderSpeciality;

                        courseCurriculum.Add(curriculum);
                    }
                    //else
                    //{
                    //    ValidationCurriculum curriculum = new ValidationCurriculum();

                    //    curriculum.IdValidationClient = validationClients.Where(x => x.OldId == module.IntCourseId).First().IdValidationClient;

                    //    curriculum.Subject = module.VcModuleName;

                    //    curriculum.Topic = module.VcModuleName;

                    //    if (module.IntCurricHoursType != 1 && module.IntCurricHoursType != 9)
                    //    {
                    //        var training = ProfessionalTraining.Where(x => x.DefaultValue2.Contains(module.IntCurricHoursType.ToString())).First();

                    //        curriculum.IdProfessionalTraining = training.IdKeyType;

                    //        curriculum.Theory = module.IntHours;
                    //    }
                    //    else if (module.IntCurricHoursType == 1)
                    //    {
                    //        curriculum.Theory += module.IntHours;
                    //        curriculum.IdProfessionalTraining = ProfessionalTraining.Where(x => x.KeyValueIntCode == "A1").First().IdKeyValue;
                    //    }
                    //    else
                    //    {
                    //        curriculum.Theory += module.IntHours;
                    //        curriculum.IdProfessionalTraining = ProfessionalTraining.Where(x => x.KeyValueIntCode == "B").First().IdKeyValue;
                    //    }

                    //    curriculum.MigrationNote = module.OldIds;

                    //    curriculum.IdModifyUser = modifyUser.IdUser;

                    //    curriculum.IdCreateUser = modifyUser.IdUser;

                    //    curriculum.ModifyDate = DateTime.Now;

                    //    curriculum.CreationDate = DateTime.Now;

                    //    curriculum.IdCandidateProviderSpeciality = candidateSpeciality.IdCandidateProviderSpeciality;

                    //    validationCurriculam.Add(curriculum);
                    //}
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingCurriculumTheory(Първи Parallel.ForEach). Запис с Ids = " + module.OldIds);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(courseCurriculum.ToList());
            //_ApplicationDbContext.BulkInsert(validationCurriculam.ToList());

            LogEndInformation("ImportTrainingCurriculumTheory");

        }
        public void ImportTrainingCurriculumPractice(int? OldId = null)
        {
            LogStrartInformation("ImportTrainingCurriculumPractice");

            //OldId = 1664;

            var CandidateProviderSpeciality = _ApplicationDbContext.CandidateProviderSpecialities.ToList();
            var programs = _ApplicationDbContext.Programs.ToList();
            var course = _ApplicationDbContext.Courses.ToList();
            var code_curric_hours_type = _jessieContextContext.CodeCurricHoursTypes.ToList();

            var ProfessionalTraining = (from kv in _ApplicationDbContext.KeyValues
                                        join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                        where kt.KeyTypeIntCode == "ProfessionalTraining"
                                        select kv).To<KeyValueVM>().ToList();

            var practiceModules = _jessieContextContext.TbCurricModules.Where(x => x.IntTrainingType == 2 && x.IsValid.Value)
                .ToList()
                .Select(x => new
                {
                    Id = x.Id,
                    IntTrainingType = x.IntTrainingType,
                    IntCourseId = x.IntCourseId,
                    IntCourseGroupId = x.IntCourseGroupId,
                    VcModuleName = x.VcModuleName,
                    IntCurricHoursType = x.IntCurricHoursType,
                    IntHours = x.IntHours,
                    IntCurricOrder = x.IntCurricOrder

                })
                .GroupBy(e => new { e.IntCourseId, e.IntCourseGroupId, e.VcModuleName })
                .Select(g => new
                {
                    IntCurricOrder = g.Select(x => x.IntCurricOrder).First(),
                    IntCourseId = g.Key.IntCourseId,
                    IntCourseGroupId = g.Key.IntCourseGroupId,
                    VcModuleName = g.Key.VcModuleName.Trim(),
                    IntHours = g.Sum(x => x.IntHours),
                    IntTrainingType = g.Select(x => x.IntTrainingType).First(),
                    IntCurricHoursType = g.Select(x => x.IntCurricHoursType).First(),
                    OldIds = String.Join(" ", g.Select(x => x.Id).ToArray())
                }).ToList();

            if (OldId is not null)
            {
                var courses = this._jessieContextContext.TbCourses.Where(x => x.IntProviderId == OldId).ToList();

                practiceModules = practiceModules.Where(x => courses.Any(z => z.Id == x.IntCourseId)).ToList();
            }

            var currentCurriculums = _ApplicationDbContext.TrainingCurriculums.Include(x => x.Course.Program).ToList();

            ConcurrentBag<TrainingCurriculum> curriculam = new ConcurrentBag<TrainingCurriculum>();

            ConcurrentBag<TrainingCurriculum> curriculamUpdate = new ConcurrentBag<TrainingCurriculum>();


            Parallel.ForEach(practiceModules, module =>
            {
                try
                {

                    var curr = currentCurriculums.Where(x => ((x.Course == null && module.IntCourseGroupId == null) || (x.Course != null && x.Course.OldId == module.IntCourseGroupId)) &&
                                                       (x.Course is not null && x.Course.Program is not null && x.Course.Program.OldId == module.IntCourseId && x.Subject.Trim() == module.VcModuleName.Trim()))
                                                       .FirstOrDefault();

                    if (curr is null)
                        curr = currentCurriculums.Where(x => ((x.Course == null && module.IntCourseGroupId == null) || (x.Course != null && x.Course.OldId == module.IntCourseGroupId)) &&
                                                       (x.Program is not null && x.Program.OldId == module.IntCourseId && x.Subject.Trim() == module.VcModuleName.Trim()))
                                                       .FirstOrDefault();

                    if (curr == null)
                    {
                        TrainingCurriculum curriculum = new TrainingCurriculum();

                        curriculum.Practice = 0;
                        curriculum.Theory = 0;

                        var program = programs.Where(x => x.OldId == module.IntCourseId).First();
                        var candidateSpeciality = CandidateProviderSpeciality.Where(x => x.IdSpeciality == program.IdSpeciality && x.IdCandidate_Provider == program.IdCandidateProvider).FirstOrDefault();
                        if (candidateSpeciality is not null)
                        {
                            curriculum.IdProgram = program.IdProgram;

                            if (module.IntCourseGroupId != null)
                                curriculum.IdCourse = course.Where(x => x.OldId == module.IntCourseGroupId).First().IdCourse;

                            curriculum.Subject = module.VcModuleName;

                            curriculum.Topic = module.VcModuleName;

                            var training = ProfessionalTraining.Where(x => x.DefaultValue2.Contains(module.IntCurricHoursType.ToString())).First();

                            curriculum.IdProfessionalTraining = training.IdKeyValue;

                            curriculum.Practice = module.IntHours;

                            curriculum.IdModifyUser = modifyUser.IdUser;

                            curriculum.IdCreateUser = modifyUser.IdUser;

                            curriculum.Order = module.IntCurricOrder;

                            curriculum.ModifyDate = DateTime.Now;

                            curriculum.CreationDate = DateTime.Now;

                            curriculum.MigrationNote = String.Join(" ", module.OldIds.ToArray());

                            curriculum.IdCandidateProviderSpeciality = candidateSpeciality.IdCandidateProviderSpeciality;

                            curriculam.Add(curriculum);
                        }
                    }
                    else
                    {
                        curr.Practice = module.IntHours;
                        curr.CandidateProviderSpeciality = null;
                        curr.CandidateCurriculum = null;
                        curr.Course = null;
                        curr.Program = null;
                        curriculamUpdate.Add(curr);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingCurriculumPractice(Първи Parallel.ForEach). Запис с Id = " + module.OldIds);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(curriculam.ToList());

            _ApplicationDbContext.BulkUpdate(curriculamUpdate.ToList());

            LogEndInformation("ImportTrainingCurriculumPractice");
        }
        public void ImportTrainerCourse(int? OldId = null)
        {
            //OldId = 1582;
            LogStrartInformation("ImportTrainerCourse");

            var ref_trainers_courses = new List<RefTrainersCourse>();
            if (OldId is null)
            {
                ref_trainers_courses = _jessieContextContext.RefTrainersCourses.ToList();
            }
            else
            {
                var tr = this._jessieContextContext.TbTrainers.Where(x => x.IntProviderId == OldId).ToList();

                ref_trainers_courses = _jessieContextContext.RefTrainersCourses.ToList().Where(x => tr.Any(z => z.Id == x.IntTrainerId)).ToList();
            }
            var trainers = _ApplicationDbContext.CandidateProviderTrainers.ToList();
            var courses = _ApplicationDbContext.Courses.ToList();

            var TrainingType = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "TrainingType"
                                select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<TrainerCourse> trainerCourses = new ConcurrentBag<TrainerCourse>();

            Parallel.ForEach(ref_trainers_courses, refTrainersCourse =>
            {
                try
                {
                    if (refTrainersCourse.IntTrainerId is not null && refTrainersCourse.IsValid.HasValue && refTrainersCourse.IsValid.Value)
                    {
                        TrainerCourse trainerCourse = new TrainerCourse();

                        trainerCourse.IdTrainer = trainers.Where(x => x.OldId == refTrainersCourse.IntTrainerId).First().IdCandidateProviderTrainer;

                        trainerCourse.IdCourse = courses.Where(x => x.OldId == refTrainersCourse.IntCourseGroupId).First().IdCourse;
                        if (refTrainersCourse.IntTrainingTypeId is not null)
                            trainerCourse.IdТraininType = TrainingType.Where(x => Int32.Parse(x.DefaultValue2) == refTrainersCourse.IntTrainingTypeId).First().IdKeyValue;

                        trainerCourse.IdModifyUser = modifyUser.IdUser;

                        trainerCourse.OldId = (int?)refTrainersCourse.Id;

                        trainerCourse.IdCreateUser = modifyUser.IdUser;

                        trainerCourse.ModifyDate = DateTime.Now;

                        trainerCourse.CreationDate = DateTime.Now;

                        trainerCourses.Add(trainerCourse);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainerCourse(Първи Parallel.ForEach). Запис с Id = " + refTrainersCourse.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(trainerCourses.ToList());

            LogEndInformation("ImportTrainerCourse");

        }
        public void ImportTrainingPremisesCourses(int? OldId = null)
        {
            LogStrartInformation("ImportTrainingPremisesCourses");

            var ref_course_group_premises = new List<RefCourseGroupPremise>();

            if (OldId is null)
            {
                ref_course_group_premises = _jessieContextContext.RefCourseGroupPremises.ToList();
            }
            else
            {
                var providerPremises = this._jessieContextContext.TbProviderPremises.Where(x => x.IntProviderId == OldId).ToList();
                ref_course_group_premises = this._jessieContextContext.RefCourseGroupPremises.ToList().Where(x => providerPremises.Any(z => z.Id == x.IntProviderPremiseId)).ToList();
            }

            var premises = _ApplicationDbContext.CandidateProviderPremises.ToList();
            var courses = _ApplicationDbContext.Courses.ToList();

            var TypeForEducation = (from kv in _ApplicationDbContext.KeyValues
                                    join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                    where kt.KeyTypeIntCode == "TrainingType"
                                    select kv).To<KeyValueVM>().ToList();

            ConcurrentBag<PremisesCourse> premisesCourses = new ConcurrentBag<PremisesCourse>();

            Parallel.ForEach(ref_course_group_premises, refCourseGroupPremise =>
            {
                try
                {
                    if (refCourseGroupPremise.IsValid.Value)
                    {
                        PremisesCourse premisesCourse = new PremisesCourse();

                        premisesCourse.IdPremises = premises.Where(x => x.OldId == refCourseGroupPremise.IntProviderPremiseId).First().IdCandidateProviderPremises;

                        premisesCourse.IdCourse = courses.Where(x => x.OldId == refCourseGroupPremise.IntCourseGroupId).First().IdCourse;

                        premisesCourse.IdТraininType = TypeForEducation.Where(x => Int32.Parse(x.DefaultValue2) == refCourseGroupPremise.IntTrainingTypeId).First().IdKeyValue;

                        premisesCourse.IdCreateUser = modifyUser.IdUser;

                        premisesCourse.ModifyDate = DateTime.Now;

                        premisesCourse.CreationDate = DateTime.Now;

                        premisesCourse.OldId = (int)refCourseGroupPremise.Id;

                        premisesCourses.Add(premisesCourse);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingPremisesCourses(Първи Parallel.ForEach). Запис с Id = " + refCourseGroupPremise.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(premisesCourses.ToList());

            LogEndInformation("ImportTrainingPremisesCourses");

        }
        public void ImportTrainingClients(int? OldId = null)
        {
            LogStrartInformation("ImportTrainingClients");

            //OldId = 2929;
            var sex = (from kv in _ApplicationDbContext.KeyValues
                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                       where kt.KeyTypeIntCode == "Sex"
                       select kv).To<KeyValueVM>().ToList();

            var IndentTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "IndentType"
                               select kv).To<KeyValueVM>().ToList();

            var Nationalities = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "Nationality"
                                 select kv).To<KeyValueVM>().ToList();

            var Educations = (from kv in _ApplicationDbContext.KeyValues
                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                              where kt.KeyTypeIntCode == "Education" && !string.IsNullOrEmpty(kv.DefaultValue2)
                              select kv).To<KeyValueVM>().ToList();

            var tb_clients = new List<TbClient>();
            if (OldId is null)
                tb_clients = _jessieContextContext.TbClients.ToList();
            else
                tb_clients = _jessieContextContext.TbClients.Where(x => x.IntProviderId == OldId).ToList();

            var providers = _ApplicationDbContext.CandidateProviders.ToList();

            var professionalDirection = _ApplicationDbContext.ProfessionalDirections.ToList();

            ConcurrentBag<Client> clients = new ConcurrentBag<Client>();

            Parallel.ForEach(tb_clients, c =>
            {
                try
                {
                    ClientVM client = new ClientVM();

                    if (c.VcClientFirstName != null)
                        client.FirstName = c.VcClientFirstName;
                    else
                        client.FirstName = "";

                    client.SecondName = c.VcClientSecondName;

                    if (c.VcClientFamilyName != null)
                        client.FamilyName = c.VcClientFamilyName;
                    else
                        client.FamilyName = "";

                    client.FamilyNameEN = ConvertCyrToLatin(client.FamilyName);
                    client.FirstNameEN = ConvertCyrToLatin(client.FirstName);

                    client.IdCandidateProvider = providers.Where(x => x.OldId == c.IntProviderId).First().IdCandidate_Provider;

                    if (c.IntClientGender != null && c.IntClientGender != 0)
                        client.IdSex = sex.Where(x => Int32.Parse(x.DefaultValue2) == c.IntClientGender).First().IdKeyValue;

                    if (c.IntEducationId != null)
                    {
                        if (c.IntEducationId == 24)
                            c.IntEducationId = 30;
                        client.IdEducation = Educations.Where(x => Int32.Parse(x.DefaultValue2) == c.IntEducationId).First().IdKeyValue;
                    }
                    if (c.IntEgnTypeId != null)
                        client.IdIndentType = IndentTypes.Where(x => Int32.Parse(x.DefaultValue2) == c.IntEgnTypeId).First().IdKeyValue;

                    client.Indent = c.VcEgn;

                    if (c.IntNationalityId != null && c.IntNationalityId != 0)
                        client.IdNationality = Nationalities.Where(x => x.Order == c.IntNationalityId).First().IdKeyValue;

                    client.BirthDate = c.DtClientBirthDate;
                    if (c.IntVetAreaId != null)
                        client.IdProfessionalDirection = professionalDirection.Where(x => x.OldId == c.IntVetAreaId).First().IdProfessionalDirection;

                    client.FirstNameEN = ConvertCyrToLatin(client.FirstName);
                    if (client.SecondName is not null)
                        client.SecondNameEN = ConvertCyrToLatin(client.SecondName);
                    client.FamilyNameEN = ConvertCyrToLatin(client.FamilyName);

                    client.IdModifyUser = modifyUser.IdUser;

                    client.IdCreateUser = modifyUser.IdUser;

                    client.ModifyDate = DateTime.Now;

                    client.CreationDate = DateTime.Now;

                    client.OldId = (int?)c.Id;

                    clients.Add(client.To<Client>());
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingClients(Първи Parallel.ForEach). Запис с Id = " + c.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            _ApplicationDbContext.BulkInsert(clients.ToList());

            LogEndInformation("ImportTrainingClients");
        }
        public void ImportTrainingClientCourses(int? OldId = null)
        {
            //OldId = 1664;

            LogStrartInformation("ImportTrainingClientCourses");

            var sex = (from kv in _ApplicationDbContext.KeyValues
                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                       where kt.KeyTypeIntCode == "Sex"
                       select kv).To<KeyValueVM>().ToList();

            var Educations = (from kv in _ApplicationDbContext.KeyValues
                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                              where kt.KeyTypeIntCode == "Education" && !string.IsNullOrEmpty(kv.DefaultValue2)
                              select kv).To<KeyValueVM>().ToList();

            var IndentTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "IndentType"
                               select kv).To<KeyValueVM>().ToList();

            var AssignTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "AssignType" && !string.IsNullOrEmpty(kv.DefaultValue2)
                               select kv).To<KeyValueVM>().ToList();

            var Nationalities = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "Nationality"
                                 select kv).To<KeyValueVM>().ToList();

            var QualificationTypes = (from kv in _ApplicationDbContext.KeyValues
                                      join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                      where kt.KeyTypeIntCode == "MinimumQualificationLevel"
                                      select kv).To<KeyValueVM>().ToList();

            var CourseFinishedTypes = (from kv in _ApplicationDbContext.KeyValues
                                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                       where kt.KeyTypeIntCode == "CourseFinishedType"
                                       select kv).To<KeyValueVM>().ToList();

            var tb_clients_courses_documents = _jessieContextContext.TbClientsCoursesDocuments.ToList();

            var ref_clients_courses = new List<RefClientsCourse>();
            var clients = new List<Client>();
            var courses = new List<Course>();
            if (OldId is null)
            {
                ref_clients_courses = _jessieContextContext.RefClientsCourses.ToList();
                clients = _ApplicationDbContext.Clients.ToList();
                courses = _ApplicationDbContext.Courses.ToList();
            }
            else
            {
                //var cli = _jessieContextContext.TbClients.Where(x => x.IntProviderId == OldId).ToList();
                ////ref_clients_courses = _jessieContextContext.RefClientsCourses.ToList().Where(x => cli.Any(z => z.Id == x.IntClientId)).ToList();
                //var refclients = this._jessieContextContext.RefClientsCourses.ToList();
                //var listForClients = new ConcurrentBag<RefClientsCourse>();
                //Parallel.ForEach(refclients, cl =>
                //{
                //    var DoesExist = cli.Where(x => x.Id == cl.IntClientId).FirstOrDefault();
                //    if (DoesExist is not null)
                //        listForClients.Add(cl);
                //});

                //ref_clients_courses = listForClients.ToList();
                ref_clients_courses = (from refCC in this._jessieContextContext.RefClientsCourses
                                       join c in this._jessieContextContext.TbClients on refCC.IntClientId equals c.Id
                                       where c.IntProviderId == OldId
                                       select refCC
                                 ).ToList();

                clients = (from c in this._ApplicationDbContext.Clients
                           join cp in this._ApplicationDbContext.CandidateProviders on c.IdCandidateProvider equals cp.IdCandidate_Provider
                           where cp.OldId == OldId
                           select c).ToList();

                courses = (from c in this._ApplicationDbContext.Courses
                           join cp in this._ApplicationDbContext.CandidateProviders on c.IdCandidateProvider equals cp.IdCandidate_Provider
                           where cp.OldId == OldId
                           select c).ToList();

            }


            var frameworkPrograms = this._ApplicationDbContext.FrameworkPrograms.ToList();

            var professionalDirection = _ApplicationDbContext.ProfessionalDirections.ToList();

            var speciality = _ApplicationDbContext.Specialities.ToList();

            var tb_course_groups = _jessieContextContext.TbCourseGroups40s.ToList();

            var CourseType = (from kv in _ApplicationDbContext.KeyValues
                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                              where kt.KeyTypeIntCode == "TypeFrameworkProgram"
                              select kv).To<KeyValueVM>().ToList();

            var kvVSPK = CourseType.Where(x => x.KeyValueIntCode.Equals("ValidationOfProfessionalQualifications")).First();
            var kvVPartProfession = CourseType.Where(x => x.KeyValueIntCode.Equals("ValidationOfPartOfProfession")).First();

            ConcurrentBag<ClientCourse> clientCourses = new ConcurrentBag<ClientCourse>();
            ConcurrentBag<ValidationClient> validationClients = new ConcurrentBag<ValidationClient>();

            Parallel.ForEach(ref_clients_courses, clientCourse =>
            {
                try
                {
                    if (clientCourse.IntCourseGroupId != null && clientCourse.IntCourseGroupId != 0)
                    {
                        ClientCourse course = new ClientCourse();

                        var trainingCourse = courses.Where(x => x.OldId == clientCourse.IntCourseGroupId).First();

                        course.IdCourse = trainingCourse.IdCourse;

                        var client = clients.Where(x => x.OldId == clientCourse.IntClientId).First();
                        course.IdClient = client.IdClient;

                        if (!string.IsNullOrEmpty(clientCourse.VcFirstName))
                            course.FirstName = clientCourse.VcFirstName;
                        else
                            course.FirstName = client.FirstName;

                        if (!string.IsNullOrEmpty(clientCourse.VcSecondName))
                            course.SecondName = clientCourse.VcSecondName;
                        else
                            course.SecondName = client.SecondName;

                        if (!string.IsNullOrEmpty(clientCourse.VcFamilyName))
                            course.FamilyName = clientCourse.VcFamilyName;
                        else
                            course.FamilyName = client.FamilyName;

                        if (clientCourse.IntClientGender != null && clientCourse.IntClientGender != 0)
                            course.IdSex = sex.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntClientGender).First().IdKeyValue;
                        else
                            course.IdSex = client.IdSex;

                        if (clientCourse.IntEgnTypeId != null)
                            course.IdIndentType = IndentTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntEgnTypeId).First().IdKeyValue;
                        else
                            course.IdIndentType = client.IdIndentType;

                        if (!string.IsNullOrEmpty(clientCourse.VcEgn))
                            course.Indent = clientCourse.VcEgn;
                        else
                            course.Indent = client.Indent;

                        if (clientCourse.DtClientBirthDate.HasValue)
                            course.BirthDate = clientCourse.DtClientBirthDate;
                        else
                            course.BirthDate = client.BirthDate;

                        if (clientCourse.IntNationalityId != null && clientCourse.IntNationalityId != 0)
                            course.IdNationality = Nationalities.Where(x => Int32.Parse(x.DefaultValue1) == clientCourse.IntNationalityId).First().IdKeyValue;
                        else
                            course.IdNationality = client.IdNationality;

                        if (clientCourse.IntVetAreaId != null && clientCourse.IntVetAreaId != 0)
                            course.IdProfessionalDirection = professionalDirection.Where(x => x.OldId == clientCourse.IntVetAreaId).First().IdProfessionalDirection;

                        if (clientCourse.IntVetSpecialityId != null && clientCourse.IntVetSpecialityId != 0)
                            course.IdSpeciality = speciality.Where(x => x.OldId == clientCourse.IntVetSpecialityId).First().IdSpeciality;

                        if (clientCourse.IntEducationId != null && clientCourse.IntEducationId != 0)
                        {
                            if (clientCourse.IntEducationId == 24)
                                clientCourse.IntEducationId = 30;

                            course.IdEducation = Educations.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntEducationId).First().IdKeyValue;
                        }
                        if (clientCourse.IntAssignTypeId != null && clientCourse.IntAssignTypeId != 0)
                            course.IdAssignType = AssignTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntAssignTypeId).First().IdKeyValue;
                        else
                            course.IdAssignType = trainingCourse.IdAssignType;

                        if (clientCourse.IntCfinishedTypeId != null && clientCourse.IntCfinishedTypeId != 0)
                            course.IdFinishedType = CourseFinishedTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntCfinishedTypeId).First().IdKeyValue;

                        course.FinishedDate = clientCourse.DtCourseFinished;

                        if (clientCourse.IntQualLevel != null)
                        {
                            if (clientCourse.IntQualLevel == 2)
                                clientCourse.IntQualLevel = 1;

                            course.IdQualificationLevel = QualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntQualLevel).First().IdKeyValue;
                        }

                        course.IdModifyUser = modifyUser.IdUser;

                        course.IdCreateUser = modifyUser.IdUser;

                        course.ModifyDate = DateTime.Now;

                        course.CreationDate = DateTime.Now;

                        course.OldId = (int?)clientCourse.Id;

                        clientCourses.Add(course);
                    }
                    else
                    {
                        ValidationClient validationClient = new ValidationClient();

                        var vcInGroup = tb_course_groups.Where(x => x.IntClientId == clientCourse.Id).FirstOrDefault();
                        var hasDoc = tb_clients_courses_documents.Where(x => x.IntClientsCoursesId == clientCourse.Id).FirstOrDefault(x => x.IntDocumentTypeId == 2 || x.IntDocumentTypeId == 1);
                        var client = clients.Where(x => x.OldId == clientCourse.IntClientId).First();

                        validationClient.IdClient = client.IdClient;

                        if (clientCourse.IntClientGender != null && clientCourse.IntClientGender != 0)
                            validationClient.IdSex = sex.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntClientGender).First().IdKeyValue;
                        else if (client.IdSex.HasValue)
                            validationClient.IdSex = client.IdSex;

                        if (clientCourse.IntEgnTypeId != null)
                            validationClient.IdIndentType = IndentTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntEgnTypeId).First().IdKeyValue;

                        validationClient.IdCandidateProvider = client.IdCandidateProvider;

                        if (vcInGroup is not null && vcInGroup.IntVetSpecialityId.HasValue)
                            validationClient.IdSpeciality = speciality.Where(x => x.OldId == vcInGroup.IntVetSpecialityId).First().IdSpeciality;
                        else if (clientCourse.IntVetSpecialityId != null && clientCourse.IntVetSpecialityId != 0)
                            validationClient.IdSpeciality = speciality.Where(x => x.OldId == clientCourse.IntVetSpecialityId).First().IdSpeciality;

                        if (clientCourse.IntQualLevel != null)
                        {
                            if (clientCourse.IntQualLevel == 2)
                                clientCourse.IntQualLevel = 1;

                            validationClient.IdQualificationLevel = QualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntQualLevel).First().IdKeyValue;
                        }

                        if (clientCourse.IntCfinishedTypeId != null && clientCourse.IntCfinishedTypeId != 0)
                            validationClient.IdFinishedType = CourseFinishedTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntCfinishedTypeId).First().IdKeyValue;

                        if (!string.IsNullOrEmpty(clientCourse.VcEgn))
                            validationClient.Indent = clientCourse.VcEgn;
                        else
                            validationClient.Indent = client.Indent;

                        if (clientCourse.IntEgnTypeId != null)
                            validationClient.IdIndentType = IndentTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntEgnTypeId).First().IdKeyValue;
                        else
                            validationClient.IdIndentType = client.IdIndentType;

                        if (clientCourse.DtClientBirthDate.HasValue)
                            validationClient.BirthDate = clientCourse.DtClientBirthDate;
                        else
                            validationClient.BirthDate = client.BirthDate;

                        if (clientCourse.IntNationalityId != null && clientCourse.IntNationalityId != 0)
                            validationClient.IdNationality = Nationalities.Where(x => Int32.Parse(x.DefaultValue1) == clientCourse.IntNationalityId).First().IdKeyValue;

                        if (clientCourse.IntAssignTypeId != null && clientCourse.IntAssignTypeId != 0)
                            validationClient.IdAssignType = AssignTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntAssignTypeId).First().IdKeyValue;

                        if (vcInGroup is not null && vcInGroup.IntCourseFrCurrId != null && vcInGroup.IntCourseFrCurrId != 0)
                            validationClient.IdFrameworkProgram = frameworkPrograms
                            .Where(x => x.OldId == vcInGroup.IntCourseFrCurrId)
                            .First().IdFrameworkProgram;

                        if (vcInGroup is not null)
                        {
                            if (vcInGroup.IntCourseTypeId == 11)
                                validationClient.IdCourseType = kvVSPK.IdKeyValue;
                            else
                                validationClient.IdCourseType = kvVPartProfession.IdKeyValue;

                            validationClient.StartDate = vcInGroup.DtStartDate;

                            validationClient.ExamTheoryDate = vcInGroup.DtExamTheoryDate;

                            validationClient.ExamPracticeDate = vcInGroup.DtExamPracticeDate;

                            validationClient.Cost = vcInGroup.NumCourseCost;
                        }
                        else if (hasDoc is not null)
                        {
                            if (hasDoc.IntDocumentTypeId == 2)
                                validationClient.IdCourseType = kvVPartProfession.IdKeyValue;
                            else
                                validationClient.IdCourseType = kvVSPK.IdKeyValue;
                        }

                        if (clientCourse.DtCourseFinished.HasValue)
                            validationClient.EndDate = clientCourse.DtCourseFinished;
                        else if (vcInGroup is not null && vcInGroup.DtExamTheoryDate.HasValue)
                            validationClient.EndDate = vcInGroup.DtExamTheoryDate.Value;
                        else if (vcInGroup is not null && vcInGroup.DtExamPracticeDate.HasValue)
                            validationClient.EndDate = vcInGroup.DtExamPracticeDate.Value;
                        else if (hasDoc is not null && hasDoc.DtDocumentDate.HasValue)
                            validationClient.EndDate = hasDoc.DtDocumentDate.Value;


                        if (!string.IsNullOrEmpty(clientCourse.VcFirstName))
                            validationClient.FirstName = clientCourse.VcFirstName;
                        else
                            validationClient.FirstName = client.FirstName;

                        if (!string.IsNullOrEmpty(clientCourse.VcSecondName))
                            validationClient.SecondName = clientCourse.VcSecondName;
                        else
                            validationClient.SecondName = client.SecondName;

                        if (!string.IsNullOrEmpty(clientCourse.VcFamilyName))
                            validationClient.FamilyName = clientCourse.VcFamilyName;
                        else
                            validationClient.FamilyName = client.FamilyName;

                        validationClient.IdModifyUser = modifyUser.IdUser;

                        validationClient.IdCreateUser = modifyUser.IdUser;

                        validationClient.ModifyDate = DateTime.Now;

                        validationClient.CreationDate = DateTime.Now;

                        validationClient.IdCountryOfBirth = client.IdCountryOfBirth;

                        validationClient.IdCityOfBirth = client.IdCityOfBirth;

                        validationClient.OldId = (int?)clientCourse.Id;

                        validationClients.Add(validationClient);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingClientCourses(Първи Parallel.ForEach). Запис с Id = " + clientCourse.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });
            try
            {
                _ApplicationDbContext.BulkInsert(clientCourses.ToList());
                _ApplicationDbContext.BulkInsert(validationClients.ToList());
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("Гръмна метод ImportTrainingClientCourses(Запазване на записи).");
                this.logger.LogError(ex.Message);
                this.logger.LogError(ex.InnerException?.Message);
                this.logger.LogError(ex.StackTrace);
            }

            LogEndInformation("ImportTrainingClientCourses");
        }

        //Най вероятно няма да се ползва

        public void importTrainingValidationClient()
        {
            var sex = (from kv in _ApplicationDbContext.KeyValues
                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                       where kt.KeyTypeIntCode == "Sex"
                       select kv).To<KeyValueVM>().ToList();

            var Educations = (from kv in _ApplicationDbContext.KeyValues
                              join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                              where kt.KeyTypeIntCode == "Education" && !string.IsNullOrEmpty(kv.DefaultValue2)
                              select kv).To<KeyValueVM>().ToList();

            var IndentTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "IndentType"
                               select kv).To<KeyValueVM>().ToList();

            var AssignTypes = (from kv in _ApplicationDbContext.KeyValues
                               join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                               where kt.KeyTypeIntCode == "AssignType" && !string.IsNullOrEmpty(kv.DefaultValue2)
                               select kv).To<KeyValueVM>().ToList();

            var Nationalities = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "Nationality"
                                 select kv).To<KeyValueVM>().ToList();

            var QualificationTypes = (from kv in _ApplicationDbContext.KeyValues
                                      join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                      where kt.KeyTypeIntCode == "MinimumQualificationLevel"
                                      select kv).To<KeyValueVM>().ToList();

            var CourseFinishedTypes = (from kv in _ApplicationDbContext.KeyValues
                                       join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                       where kt.KeyTypeIntCode == "CourseFinishedType"
                                       select kv).To<KeyValueVM>().ToList();


            var ref_clients_courses = _jessieContextContext.RefClientsCourses.ToList();

            var clients = _ApplicationDbContext.Clients.ToList();

            var courses = _ApplicationDbContext.Courses.ToList();

            var professionalDirection = _ApplicationDbContext.ProfessionalDirections.ToList();

            var speciality = _ApplicationDbContext.Specialities.ToList();

            ConcurrentBag<ValidationClient> validationClients = new ConcurrentBag<ValidationClient>();

            Parallel.ForEach(ref_clients_courses, clientCourse =>
            {
                ValidationClient course = new ValidationClient();

                if (clientCourse.IntCourseGroupId == null)
                {
                    course.IdClient = clients.Where(x => x.OldId == clientCourse.IntClientId).First().IdClient;

                    if (clientCourse.VcFirstName != null)
                        course.FirstName = clientCourse.VcFirstName;
                    else
                        course.FirstName = "";

                    course.SecondName = clientCourse.VcSecondName;

                    if (clientCourse.VcFamilyName != null)
                        course.FamilyName = clientCourse.VcFamilyName;
                    else
                        course.FamilyName = "";

                    if (clientCourse.IntClientGender != null && clientCourse.IntClientGender != 0)
                        course.IdSex = sex.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntClientGender).First().IdKeyValue;

                    if (clientCourse.IntEgnTypeId != null)
                        course.IdIndentType = IndentTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntEgnTypeId).First().IdKeyValue;

                    course.Indent = clientCourse.VcEgn;

                    course.BirthDate = clientCourse.DtClientBirthDate;

                    if (clientCourse.IntNationalityId != null && clientCourse.IntNationalityId != 0)
                        course.IdNationality = Nationalities.Where(x => x.Order == clientCourse.IntNationalityId).First().IdKeyValue;

                    if (clientCourse.IntVetSpecialityId != null && clientCourse.IntVetSpecialityId != 0)
                        course.IdSpeciality = speciality.Where(x => x.OldId == clientCourse.IntVetSpecialityId).First().IdSpeciality;

                    if (clientCourse.IntAssignTypeId != null && clientCourse.IntAssignTypeId != 0)
                        course.IdAssignType = AssignTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntAssignTypeId).First().IdKeyValue;

                    if (clientCourse.IntCfinishedTypeId != null && clientCourse.IntCfinishedTypeId != 0)
                        course.IdFinishedType = CourseFinishedTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntCfinishedTypeId).First().IdKeyValue;

                    if (clientCourse.IntQualLevel != null)
                    {
                        if (clientCourse.IntQualLevel == 2)
                            clientCourse.IntQualLevel = 1;

                        course.IdQualificationLevel = QualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientCourse.IntQualLevel).First().IdKeyValue;
                    }

                    course.IdModifyUser = modifyUser.IdUser;

                    course.IdCreateUser = modifyUser.IdUser;

                    course.ModifyDate = DateTime.Now;

                    course.CreationDate = DateTime.Now;

                    course.OldId = (int?)clientCourse.Id;

                    validationClients.Add(course.To<ValidationClient>());
                }
                {

                }
            });
            _ApplicationDbContext.BulkInsert(validationClients.ToList());
        }

        public void ImportTrainingClientCourseDocument(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {

            LogStrartInformation("ImportTrainingClientCourseDocument");


            //MigrateDocuments = false;
            //OldId = 1664;
            //var tb_clients_courses_documents = _jessieContextContext.TbClientsCoursesDocuments.ToList();
            var clientCourses = new List<ClientCourse>();

            var tb_request_docs_management = new List<TbRequestDocsManagement>();

            var tb_clients_courses_documents = new List<TbClientsCoursesDocument>();

            var validationClients = new List<ValidationClient>();
            if (OldId is null)
            {
                tb_clients_courses_documents = _jessieContextContext.TbClientsCoursesDocuments.ToList();
                tb_request_docs_management = _jessieContextContext.TbRequestDocsManagements.Where(x => x.IntRequestDocsOperationId == 3).ToList();
                clientCourses = this._ApplicationDbContext.ClientCourses.ToList();
                validationClients = this._ApplicationDbContext.ValidationClients.ToList();
            }
            else
            {
                //tb_request_docs_management = _jessieContextContext.TbRequestDocsManagements.Where(x => x.IntRequestDocsOperationId == 3 && x.IntProviderId == OldId).ToList();

                //var cli = _jessieContextContext.TbClients.Where(x => x.IntProviderId == OldId).ToList();

                //var courses = _jessieContextContext.RefClientsCourses.ToList();

                //var ref_clients_courses = new ConcurrentBag<RefClientsCourse>();

                //Parallel.ForEach(cli, client =>
                //{
                //    var c = courses.Where(x => x.IntClientId == client.Id).ToList();
                //    if (c.Any())
                //    {
                //        foreach (var course in c)
                //        {
                //            ref_clients_courses.Add(course);
                //        }
                //    }
                //});

                //var documents = this._jessieContextContext.TbClientsCoursesDocuments.ToList();

                //var tccd = new ConcurrentBag<TbClientsCoursesDocument>();

                //Parallel.ForEach(ref_clients_courses, c =>
                //{
                //    var d = documents.Where(x => x.IntClientsCoursesId == c.Id).ToList();
                //    if (d.Any())
                //    {
                //        foreach (var document in d)
                //        {
                //            tccd.Add(document);
                //        }
                //    }
                //});
                //clientCourses = this._ApplicationDbContext.ClientCourses
                //    .Include(x => x.Client.CandidateProvider).Where(x => x.Client.CandidateProvider.OldId == OldId).ToList();

                //tb_clients_courses_documents = tccd.ToList();
                //_ApplicationDbContext.ClientCourses.ToList();
                tb_clients_courses_documents = (from tccd in this._jessieContextContext.TbClientsCoursesDocuments
                                                join tcc in this._jessieContextContext.RefClientsCourses on tccd.IntClientsCoursesId equals tcc.Id
                                                join tc in this._jessieContextContext.TbClients on tcc.IntClientId equals tc.Id
                                                where tc.IntProviderId == OldId
                                                select tccd).ToList();

                clientCourses = (from cc in this._ApplicationDbContext.ClientCourses
                                 join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                                 join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                                 where cp.OldId == OldId
                                 select cc).ToList();

                tb_request_docs_management = this._jessieContextContext.TbRequestDocsManagements.Where(x => x.IntProviderId == OldId).ToList();

                validationClients = (from cc in this._ApplicationDbContext.ValidationClients
                                     join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                                     join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                                     where cp.OldId == OldId
                                     select cc).ToList();
            }

            var courses = this._ApplicationDbContext.Courses.ToList();

            var documentSerialNumbers = _ApplicationDbContext.DocumentSerialNumbers.Include(x => x.RequestDocumentManagement).ToList();

            var typeOfRequestedDocuments = _ApplicationDbContext.TypeOfRequestedDocuments.Where(x => !string.IsNullOrEmpty(x.MigrationNote)).ToList();

            var protocols = this._ApplicationDbContext.CourseProtocols.ToList();

            var ClientCourseDocumentStatuses = (from kv in _ApplicationDbContext.KeyValues
                                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                where kt.KeyTypeIntCode == "ClientDocumentStatusType"
                                                select kv).To<KeyValueVM>().ToList();

            var EnteredInTheRegisterKV = (from kv in _ApplicationDbContext.KeyValues
                                          join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                          where kt.KeyTypeIntCode == "ClientDocumentStatusType" && kv.KeyValueIntCode.Equals("EnteredInTheRegister")
                                          select kv).To<KeyValueVM>().First();

            var partProfession = (from kv in _ApplicationDbContext.KeyValues
                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                  where kt.KeyTypeIntCode == "TypeFrameworkProgram" && kv.KeyValueIntCode.Equals("PartProfession")
                                  select kv).To<KeyValueVM>().First();

            var professionalQualification = (from kv in _ApplicationDbContext.KeyValues
                                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                             where kt.KeyTypeIntCode == "TypeFrameworkProgram" && kv.KeyValueIntCode.Equals("ProfessionalQualification")
                                             select kv).To<KeyValueVM>().First();

            var ValidationOfPartOfProfession = (from kv in _ApplicationDbContext.KeyValues
                                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                where kt.KeyTypeIntCode == "TypeFrameworkProgram" && kv.KeyValueIntCode.Equals("ValidationOfPartOfProfession")
                                                select kv).To<KeyValueVM>().First();

            var printed = (from kv in _ApplicationDbContext.KeyValues
                           join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                           where kt.KeyTypeIntCode == "ActionType" && kv.KeyValueIntCode.Equals("Printed")
                           select kv).To<KeyValueVM>().First();

            var received = (from kv in _ApplicationDbContext.KeyValues
                            join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                            where kt.KeyTypeIntCode == "ActionType" && kv.KeyValueIntCode.Equals("Received")
                            select kv).To<KeyValueVM>().First();

            var ProtocolType = (from kv in _ApplicationDbContext.KeyValues
                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                where kt.KeyTypeIntCode == "CourseProtocolType" && kv.KeyValueIntCode.Equals("3-81B")
                                select kv).To<KeyValueVM>().First();

            var PrintingHouse = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "DocumentRequestReceiveType" && kv.KeyValueIntCode.Equals("PrintingHouse")
                                 select kv).To<KeyValueVM>().First();

            string pattern = "[0-9]+$";

            Regex regex = new Regex(pattern);

            ConcurrentBag<ClientCourseDocument> docs = new ConcurrentBag<ClientCourseDocument>();
            ConcurrentBag<ValidationClientDocument> docsv = new ConcurrentBag<ValidationClientDocument>();

            ConcurrentBag<DocumentSerialNumber> serialNumbers = new ConcurrentBag<DocumentSerialNumber>();
            ConcurrentBag<RequestDocumentManagement> documentManagements = new ConcurrentBag<RequestDocumentManagement>();
            ConcurrentBag<CourseProtocol> protocolsToUpdate = new ConcurrentBag<CourseProtocol>();
            if (!OnlyDocuments)
            {
                Parallel.ForEach(tb_clients_courses_documents, doc =>
                {
                    try
                    {
                        if (doc.IntClientsCoursesId is not null)
                        {
                            var cc = clientCourses.Where(x => x.OldId == doc.IntClientsCoursesId).FirstOrDefault();

                            var DocManagement = tb_request_docs_management.Where(x => x.IntPartnerId == doc.IntClientsCoursesId).FirstOrDefault();
                            var SerialNumber = new DocumentSerialNumber();
                            if (DocManagement is not null)
                                SerialNumber = documentSerialNumbers.Where(x => x.RequestDocumentManagement.OldId == DocManagement.Id).FirstOrDefault();

                            if (cc is not null)
                            {
                                ClientCourseDocument clientCourse = new ClientCourseDocument();


                                if (doc.IntDocumentTypeId != null)
                                {
                                    var cer = typeOfRequestedDocuments.Where(x => Int32.Parse(x.MigrationNote) == doc.IntDocumentTypeId).First();
                                    clientCourse.IdTypeOfRequestedDocument = cer.IdTypeOfRequestedDocument;

                                    clientCourse.IdDocumentType = cer.IdCourseType;
                                }
                                else
                                {
                                    var course = courses.Where(x => x.IdCourse == cc.IdCourse).First();

                                    if (course.IdTrainingCourseType == partProfession.IdKeyValue)
                                    {
                                        clientCourse.IdTypeOfRequestedDocument = typeOfRequestedDocuments.Where(x => x.MigrationNote == "2").First().IdTypeOfRequestedDocument;
                                        clientCourse.IdDocumentType = partProfession.IdKeyValue;
                                    }
                                    else
                                    {

                                        clientCourse.IdTypeOfRequestedDocument = typeOfRequestedDocuments.Where(x => x.MigrationNote == "1").First().IdTypeOfRequestedDocument;
                                        clientCourse.IdDocumentType = professionalQualification.IdKeyValue;
                                    }
                                }
                                clientCourse.IdClientCourse = cc.IdClientCourse;

                                clientCourse.FinishedYear = doc.IntCourseFinishedYear;

                                clientCourse.DocumentPrnNo = doc.VcDocumentPrnNo;

                                clientCourse.DocumentRegNo = doc.VcDocumentRegNo;

                                clientCourse.DocumentDate = doc.DtDocumentDate;

                                clientCourse.TheoryResult = doc.NumTheoryResult;

                                clientCourse.PracticeResult = doc.NumPracticeResult;

                                if (doc.NumTheoryResult.HasValue && !doc.NumPracticeResult.HasValue)
                                    clientCourse.FinalResult = doc.NumTheoryResult;
                                else if (!doc.NumTheoryResult.HasValue && doc.NumPracticeResult.HasValue)
                                    clientCourse.FinalResult = doc.NumPracticeResult;

                                clientCourse.QualificationName = doc.VcQualificationName;

                                clientCourse.QualificationLevel = doc.VcQualificatiojLevel;

                                clientCourse.DocumentSerNo = doc.VcDocumentPrnSer;

                                clientCourse.DocumentProtocol = doc.VcDocumentProt;

                                var protocol = protocols.Where(x => x.IdCourse == cc.IdCourse && x.IdCourseProtocolType == ProtocolType.IdKeyValue).FirstOrDefault();

                                if (protocol is not null)
                                {
                                    clientCourse.IdCourseProtocol = protocol.IdCourseProtocol;

                                    //if (!string.IsNullOrEmpty(doc.VcDocumentProt) && string.IsNullOrEmpty(protocol.CourseProtocolNumber))
                                    //{
                                    //    protocol.CourseProtocolNumber = doc.VcDocumentProt;

                                    //    protocolsToUpdate.Add(protocol);
                                    //}
                                }

                                if (doc.IntDocumentStatus != null && clientCourse.IdDocumentType != partProfession.IdKeyValue)
                                    clientCourse.IdDocumentStatus = ClientCourseDocumentStatuses.Where(x => Int32.Parse(x.DefaultValue2) == doc.IntDocumentStatus).First().IdKeyValue;
                                else
                                    clientCourse.IdDocumentStatus = EnteredInTheRegisterKV.IdKeyValue;

                                clientCourse.IdModifyUser = modifyUser.IdUser;

                                clientCourse.IdCreateUser = modifyUser.IdUser;

                                clientCourse.ModifyDate = DateTime.Now;

                                if (doc.DtDocumentDate.HasValue)
                                    clientCourse.CreationDate = doc.DtDocumentDate.Value;

                                clientCourse.OldId = (int)doc.Id;

                                clientCourse.DocumentSerialNumber = null;

                                clientCourse.ClientCourse = null;

                                if (SerialNumber != null && SerialNumber.IdDocumentSerialNumber != 0)
                                {
                                    clientCourse.IdDocumentSerialNumber = SerialNumber.IdDocumentSerialNumber;
                                }
                                else if (!string.IsNullOrEmpty(clientCourse.DocumentPrnNo))
                                {
                                    var match = regex.Match(clientCourse.DocumentPrnNo);

                                    if (match.Success)
                                    {
                                        var number = documentSerialNumbers.Where(x => x.SerialNumber.Equals(match.Value) && x.ReceiveDocumentYear == clientCourse.FinishedYear).FirstOrDefault();

                                        if (number is not null)
                                        {
                                            clientCourse.IdDocumentSerialNumber = number.IdDocumentSerialNumber;
                                        }
                                        else
                                        {
                                            var course = courses.Where(x => x.IdCourse == cc.IdCourse).First();

                                            DocumentSerialNumber newNumber = new DocumentSerialNumber();

                                            newNumber.IdCandidateProvider = course.IdCandidateProvider.Value;

                                            newNumber.IdTypeOfRequestedDocument = clientCourse.IdTypeOfRequestedDocument.Value;

                                            if (clientCourse.DocumentDate.HasValue)
                                                newNumber.DocumentDate = clientCourse.DocumentDate.Value;
                                            else
                                                newNumber.DocumentDate = DateTime.Now;

                                            newNumber.SerialNumber = match.Value;

                                            newNumber.IdDocumentOperation = printed.IdKeyValue;

                                            if (clientCourse.FinishedYear.HasValue)
                                                newNumber.ReceiveDocumentYear = clientCourse.FinishedYear.Value;
                                            else
                                                newNumber.ReceiveDocumentYear = 2011;

                                            newNumber.IdModifyUser = modifyUser.IdUser;

                                            newNumber.IdCreateUser = modifyUser.IdUser;

                                            newNumber.ModifyDate = DateTime.Now;

                                            newNumber.CreationDate = DateTime.Now;

                                            newNumber.MigrationNote = doc.Id.ToString();

                                            serialNumbers.Add(newNumber);

                                            RequestDocumentManagement management = new RequestDocumentManagement();


                                            management.IdCandidateProvider = course.IdCandidateProvider.Value;

                                            management.IdTypeOfRequestedDocument = clientCourse.IdTypeOfRequestedDocument.Value;

                                            if (clientCourse.DocumentDate.HasValue)
                                                management.DocumentDate = clientCourse.DocumentDate.Value;
                                            else
                                                management.DocumentDate = DateTime.Now;

                                            management.DocumentCount = 1;

                                            management.IdDocumentOperation = printed.IdKeyValue;

                                            if (clientCourse.FinishedYear.HasValue)
                                                management.ReceiveDocumentYear = clientCourse.FinishedYear.Value;
                                            else
                                                management.ReceiveDocumentYear = 2011;

                                            management.IdDocumentRequestReceiveType = PrintingHouse.IdKeyValue;

                                            management.IdModifyUser = modifyUser.IdUser;

                                            management.IdCreateUser = modifyUser.IdUser;

                                            management.ModifyDate = DateTime.Now;

                                            management.CreationDate = DateTime.Now;

                                            management.MigrationNote = doc.Id.ToString();

                                            documentManagements.Add(management);
                                        }
                                    }

                                }
                                docs.Add(clientCourse);
                            }
                            else
                            {
                                var vc = validationClients.Where(x => x.OldId == doc.IntClientsCoursesId).First();

                                ValidationClientDocument validation = new ValidationClientDocument();

                                if (doc.IntDocumentTypeId != null)
                                {
                                    var cer = typeOfRequestedDocuments.Where(x => Int32.Parse(x.MigrationNote) == doc.IntDocumentTypeId).First();
                                    validation.IdDocumentType = cer.IdCourseType;
                                    validation.IdTypeOfRequestedDocument = cer.IdTypeOfRequestedDocument;
                                }

                                validation.IdValidationClient = vc.IdValidationClient;

                                validation.FinishedYear = doc.IntCourseFinishedYear;

                                validation.DocumentPrnNo = doc.VcDocumentPrnNo;

                                validation.DocumentRegNo = doc.VcDocumentRegNo;

                                validation.DocumentDate = doc.DtDocumentDate;

                                validation.TheoryResult = doc.NumTheoryResult;

                                validation.PracticeResult = doc.NumPracticeResult;

                                //validation.QualificationName = doc.VcQualificationName;

                                validation.QualificationLevel = doc.VcQualificatiojLevel;

                                //validation.DocumentSerNo = doc.VcDocumentPrnSer;

                                if (doc.IntDocumentStatus != null && validation.IdDocumentType != ValidationOfPartOfProfession.IdKeyValue)
                                    validation.IdDocumentStatus = ClientCourseDocumentStatuses.Where(x => Int32.Parse(x.DefaultValue2) == doc.IntDocumentStatus).First().IdKeyValue;
                                else
                                    validation.IdDocumentStatus = EnteredInTheRegisterKV.IdKeyValue;

                                validation.IdModifyUser = modifyUser.IdUser;

                                validation.IdCreateUser = modifyUser.IdUser;

                                validation.ModifyDate = DateTime.Now;

                                validation.CreationDate = DateTime.Now;

                                validation.OldId = (int)doc.Id;

                                if (SerialNumber != null && SerialNumber.IdDocumentSerialNumber != 0)
                                {
                                    validation.IdDocumentSerialNumber = SerialNumber.IdDocumentSerialNumber;
                                }
                                else if (!string.IsNullOrEmpty(validation.DocumentPrnNo))
                                {
                                    var match = regex.Match(validation.DocumentPrnNo);

                                    if (match.Success)
                                    {
                                        var number = documentSerialNumbers.Where(x => x.SerialNumber.Equals(match.Value) && x.ReceiveDocumentYear == validation.FinishedYear).FirstOrDefault();

                                        if (number is not null)
                                        {
                                            validation.IdDocumentSerialNumber = number.IdDocumentSerialNumber;
                                        }
                                        else
                                        {
                                            DocumentSerialNumber newNumber = new DocumentSerialNumber();

                                            newNumber.IdCandidateProvider = vc.IdCandidateProvider;

                                            newNumber.IdTypeOfRequestedDocument = validation.IdTypeOfRequestedDocument!.Value;

                                            if (validation.DocumentDate.HasValue)
                                                newNumber.DocumentDate = validation.DocumentDate.Value;
                                            else
                                                newNumber.DocumentDate = DateTime.Now;

                                            newNumber.SerialNumber = match.Value;

                                            newNumber.IdDocumentOperation = printed.IdKeyValue;

                                            if (validation.FinishedYear.HasValue)
                                                newNumber.ReceiveDocumentYear = validation.FinishedYear.Value;
                                            else
                                                newNumber.ReceiveDocumentYear = 2011;

                                            newNumber.IdModifyUser = modifyUser.IdUser;

                                            newNumber.IdCreateUser = modifyUser.IdUser;

                                            newNumber.ModifyDate = DateTime.Now;

                                            newNumber.CreationDate = DateTime.Now;

                                            newNumber.MigrationNote = doc.Id.ToString();

                                            serialNumbers.Add(newNumber);

                                            RequestDocumentManagement management = new RequestDocumentManagement();

                                            management.IdCandidateProvider = vc.IdCandidateProvider;

                                            management.IdTypeOfRequestedDocument = validation.IdTypeOfRequestedDocument.Value;

                                            if (validation.DocumentDate.HasValue)
                                                management.DocumentDate = validation.DocumentDate.Value;
                                            else
                                                management.DocumentDate = DateTime.Now;

                                            management.DocumentCount = 1;

                                            management.IdDocumentOperation = printed.IdKeyValue;

                                            if (validation.FinishedYear.HasValue)
                                                management.ReceiveDocumentYear = validation.FinishedYear.Value;
                                            else
                                                management.ReceiveDocumentYear = 2011;

                                            management.IdDocumentRequestReceiveType = PrintingHouse.IdKeyValue;

                                            management.IdModifyUser = modifyUser.IdUser;

                                            management.IdCreateUser = modifyUser.IdUser;

                                            management.ModifyDate = DateTime.Now;

                                            management.CreationDate = DateTime.Now;

                                            management.MigrationNote = doc.Id.ToString();

                                            documentManagements.Add(management);
                                        }
                                    }

                                }
                                validation.DocumentSerialNumber = null;

                                validation.ValidationClient = null;



                                docsv.Add(validation);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportTrainingClientCourseDocument(Първи Parallel.ForEach). Запис с Id = " + doc.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                _ApplicationDbContext.BulkInsert(docs.ToList());
                _ApplicationDbContext.BulkInsert(docsv.ToList());
                _ApplicationDbContext.BulkInsert(documentManagements.ToList());
                _ApplicationDbContext.BulkUpdate(protocolsToUpdate.ToList());

                var cdocs = new List<ClientCourseDocument>();
                var vdocs = new List<ValidationClientDocument>();

                var requestDocumentManagements = this._ApplicationDbContext.RequestDocumentManagements.Where(x => !string.IsNullOrEmpty(x.MigrationNote) && x.OldId == null).ToList();

                if (OldId is null)
                {
                    cdocs = this._ApplicationDbContext.ClientCourseDocuments.ToList();

                    vdocs = this._ApplicationDbContext.ValidationClientDocuments.ToList();
                }
                else
                {
                    cdocs = (from cd in this._ApplicationDbContext.ClientCourseDocuments
                             join cc in this._ApplicationDbContext.ClientCourses on cd.IdClientCourse equals cc.IdClientCourse
                             join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                             join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                             where cp.OldId == OldId
                             select cd).ToList();

                    vdocs = (from cd in this._ApplicationDbContext.ValidationClientDocuments
                             join cc in this._ApplicationDbContext.ValidationClients on cd.IdValidationClient equals cc.IdValidationClient
                             join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                             join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                             where cp.OldId == OldId
                             select cd).ToList();
                }

                var documentSerialNumberForDB = new ConcurrentBag<DocumentSerialNumber>();

                ConcurrentBag<ClientCourseDocument> clientCourseDocumentsForUpdate = new ConcurrentBag<ClientCourseDocument>();
                ConcurrentBag<ValidationClientDocument> validationClientDocumentsForUpdate = new ConcurrentBag<ValidationClientDocument>();

                Parallel.ForEach(serialNumbers.ToList(), serialNumber =>
                {
                    var docManagement = requestDocumentManagements.Where(x => x.MigrationNote == serialNumber.MigrationNote).First();

                    serialNumber.IdRequestDocumentManagement = docManagement.IdRequestDocumentManagement;

                    var serialNumberCopy = serialNumber.To<DocumentSerialNumberVM>();

                    serialNumberCopy.IdDocumentOperation = received.IdKeyValue;

                    documentSerialNumberForDB.Add(serialNumber);
                    documentSerialNumberForDB.Add(serialNumberCopy.To<DocumentSerialNumber>());
                });

                _ApplicationDbContext.BulkInsert(documentSerialNumberForDB.ToList());

                var serialNumbersFromDB = this._ApplicationDbContext.DocumentSerialNumbers.Where(x => !string.IsNullOrEmpty(x.MigrationNote) && x.IdDocumentOperation == printed.IdKeyValue).ToList();

                Parallel.ForEach(serialNumbersFromDB, serialNumber =>
                {
                    var clientCourseDocument = cdocs.Where(x => x.OldId == Int32.Parse(serialNumber.MigrationNote)).FirstOrDefault();

                    if (clientCourseDocument is not null)
                    {
                        clientCourseDocument.IdDocumentSerialNumber = serialNumber.IdDocumentSerialNumber;

                        clientCourseDocumentsForUpdate.Add(clientCourseDocument);
                    }
                    else
                    {
                        var validationClientDocument = vdocs.Where(x => x.OldId == Int32.Parse(serialNumber.MigrationNote)).FirstOrDefault();

                        if (validationClientDocument is not null)
                        {
                            validationClientDocument.IdDocumentSerialNumber = serialNumber.IdDocumentSerialNumber;

                            validationClientDocumentsForUpdate.Add(validationClientDocument);
                        }
                    }
                });

                this._ApplicationDbContext.BulkUpdate(validationClientDocumentsForUpdate.ToList());
                this._ApplicationDbContext.BulkUpdate(clientCourseDocumentsForUpdate.ToList());
            }

            var course = new List<ClientCourseDocument>();

            var validation = new List<ValidationClientDocument>();
            if (OldId is null)
            {
                course = this._ApplicationDbContext.ClientCourseDocuments.ToList();

                validation = this._ApplicationDbContext.ValidationClientDocuments.ToList();
            }
            else
            {
                //course = this._ApplicationDbContext.ClientCourseDocuments.Include(x => x.ClientCourse.Client.CandidateProvider).ToList().Where(x => x.ClientCourse.Client.CandidateProvider.OldId == OldId).ToList();
                course = (from cd in this._ApplicationDbContext.ClientCourseDocuments
                          join cc in this._ApplicationDbContext.ClientCourses on cd.IdClientCourse equals cc.IdClientCourse
                          join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                          join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                          where cp.OldId == OldId
                          select cd).ToList();

                validation = (from cd in this._ApplicationDbContext.ValidationClientDocuments
                              join cc in this._ApplicationDbContext.ValidationClients on cd.IdValidationClient equals cc.IdValidationClient
                              join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                              join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                              where cp.OldId == OldId
                              select cd).ToList();
            }

            var saveDocsCourse = new ConcurrentBag<CourseDocumentUploadedFile>();
            var saveDocsValidation = new ConcurrentBag<ValidationDocumentUploadedFile>();

            var updatedDocsCourse = new List<CourseDocumentUploadedFile>();
            var updatedDocsValidation = new List<ValidationDocumentUploadedFile>();

            Parallel.ForEach(course, cDoc =>
            {
                var doc = tb_clients_courses_documents.Where(x => x.Id == cDoc.OldId).FirstOrDefault();
                if (doc is not null)
                {
                    if (doc.Document1File is not null)
                    {
                        CourseDocumentUploadedFile file = new CourseDocumentUploadedFile();

                        file.IdModifyUser = modifyUser.IdUser;

                        file.IdCreateUser = modifyUser.IdUser;

                        file.ModifyDate = DateTime.Now;

                        if (doc.DtDocumentDate.HasValue)
                            file.CreationDate = doc.DtDocumentDate.Value;

                        file.UploadedFileName = "";

                        file.IdClientCourseDocument = cDoc.IdClientCourseDocument;

                        file.OldId = (int)doc.Id;

                        file.MigrationNote = "1";

                        saveDocsCourse.Add(file);

                        //this._ApplicationDbContext.Add<CourseDocumentUploadedFile>(file);

                        //this._ApplicationDbContext.SaveChanges();

                        //try
                        //{
                        //    var url = $"\\UploadedFiles\\ClientCourseDocument\\{file.IdCourseDocumentUploadedFile}\\";

                        //    var name = SaveDocument((int)doc.Document1File.Value, url);
                        //    file.UploadedFileName = name;

                        //    updatedDocsCourse.Add(file);
                        //}
                        //catch (Exception ex)
                        //{
                        //}
                    }
                    if (doc.Document2File is not null)
                    {
                        CourseDocumentUploadedFile file = new CourseDocumentUploadedFile();

                        file.IdModifyUser = modifyUser.IdUser;

                        file.IdCreateUser = modifyUser.IdUser;

                        file.ModifyDate = DateTime.Now;

                        if (doc.DtDocumentDate.HasValue)
                            file.CreationDate = doc.DtDocumentDate.Value;

                        file.UploadedFileName = "";

                        file.IdClientCourseDocument = cDoc.IdClientCourseDocument;

                        file.OldId = (int)doc.Id;

                        file.MigrationNote = "2";

                        saveDocsCourse.Add(file);

                        //this._ApplicationDbContext.Add(file);

                        //this._ApplicationDbContext.SaveChanges();

                        //try
                        //{
                        //    var url = $"\\UploadedFiles\\ClientCourseDocument\\{file.IdCourseDocumentUploadedFile}\\";

                        //    var name = SaveDocument((int)doc.Document2File.Value, url);
                        //    file.UploadedFileName = name;

                        //    updatedDocsCourse.Add(file);
                        //}
                        //catch (Exception ex)
                        //{
                        //}
                    }
                }
            });

            Parallel.ForEach(validation, vDoc =>
            {
                var doc = tb_clients_courses_documents.Where(x => x.Id == vDoc.OldId).First();

                if (doc.Document1File is not null)
                {
                    ValidationDocumentUploadedFile file = new ValidationDocumentUploadedFile();

                    file.IdModifyUser = modifyUser.IdUser;

                    file.IdCreateUser = modifyUser.IdUser;

                    file.ModifyDate = DateTime.Now;

                    if (doc.DtDocumentDate.HasValue)
                        file.CreationDate = doc.DtDocumentDate.Value;

                    file.IdValidationClientDocument = vDoc.IdValidationClientDocument;

                    file.UploadedFileName = "";

                    file.OldId = (int)doc.Id;

                    file.MigrationNote = "1";

                    saveDocsValidation.Add(file);

                    //this._ApplicationDbContext.Add(file);

                    //this._ApplicationDbContext.SaveChanges();

                    //try
                    //{
                    //    var url = $"\\UploadedFiles\\ValidationDocument\\{file.IdValidationDocumentUploadedFile}\\";

                    //    var name = SaveDocument((int)doc.Document1File.Value, url);
                    //    file.UploadedFileName = name;

                    //    updatedDocsValidation.Add(file);
                    //}
                    //catch (Exception ex)
                    //{
                    //}
                }
                if (doc.Document2File is not null)
                {
                    ValidationDocumentUploadedFile file = new ValidationDocumentUploadedFile();

                    file.IdModifyUser = modifyUser.IdUser;

                    file.IdCreateUser = modifyUser.IdUser;

                    file.ModifyDate = DateTime.Now;

                    if (doc.DtDocumentDate.HasValue)
                        file.CreationDate = doc.DtDocumentDate.Value;

                    file.IdValidationClientDocument = vDoc.IdValidationClientDocument;

                    file.UploadedFileName = "";

                    file.OldId = (int)doc.Id;

                    file.MigrationNote = "2";

                    saveDocsValidation.Add(file);

                    //this._ApplicationDbContext.Add(file);

                    //this._ApplicationDbContext.SaveChanges();

                    //try
                    //{
                    //    var url = $"\\UploadedFiles\\ClientCourseDocument\\{file.IdValidationDocumentUploadedFile}\\";

                    //    var name = SaveDocument((int)doc.Document2File.Value, url);
                    //    file.UploadedFileName = name;

                    //    updatedDocsValidation.Add(file);
                    //}
                    //catch (Exception ex)
                    //{
                    //}
                }
            });
            this._ApplicationDbContext.BulkInsert(saveDocsCourse.ToList());
            this._ApplicationDbContext.BulkInsert(saveDocsValidation.ToList());

            if (!OnlyDocuments)
            {
                var clientCorusesWithoutDoc = this._ApplicationDbContext.ClientCourses.Include(x => x.ClientCourseDocuments).Where(x => !x.ClientCourseDocuments.Any()).ToList();
                var ValidationClientsWithoutDoc = this._ApplicationDbContext.ValidationClients.Include(x => x.ValidationClientDocuments).Where(x => !x.ValidationClientDocuments.Any()).ToList();

                ConcurrentBag<ClientCourseDocument> courseDocuments = new ConcurrentBag<ClientCourseDocument>();
                ConcurrentBag<ValidationClientDocument> validationDocuments = new ConcurrentBag<ValidationClientDocument>();

                Parallel.ForEach(clientCorusesWithoutDoc, doc =>
                {
                    ClientCourseDocument clientCourseDocument = new ClientCourseDocument();
                    clientCourseDocument.IdClientCourse = doc.IdClientCourse;
                    clientCourseDocument.ClientCourse = null;
                    courseDocuments.Add(clientCourseDocument);
                });

                Parallel.ForEach(validationDocuments, doc =>
                {
                    ValidationClientDocument validationDocument = new ValidationClientDocument();
                    validationDocument.IdValidationClient = doc.IdValidationClient;
                    validationDocument.ValidationClient = null;
                    validationDocuments.Add(validationDocument);
                });

                this._ApplicationDbContext.BulkInsert(courseDocuments.ToList());
                this._ApplicationDbContext.BulkInsert(validationDocuments.ToList());
            }
            LogEndInformation("ImportTrainingClientCourseDocument");

            if (MigrateDocuments)
            {

                if (OldId is null)
                {
                    ClientCourseDocumentsMigrateDocuments();
                    ValidationClientDocumentsMigrateDocuments();
                }
                else
                {
                    ClientCourseDocumentsMigrateDocuments(OldId);
                    ValidationClientDocumentsMigrateDocuments(OldId);
                }

                //var DocsClientCourse = new List<CourseDocumentUploadedFile>();
                //var DocsValidationClient = new List<ValidationDocumentUploadedFile>();

                //if (OldId is null)
                //{
                //    DocsClientCourse = this._ApplicationDbContext.CourseDocumentUploadedFiles.ToList();

                //    DocsValidationClient = this._ApplicationDbContext.ValidationDocumentUploadedFiles.ToList();
                //}
                //else
                //{
                //    DocsClientCourse = (from f in this._ApplicationDbContext.CourseDocumentUploadedFiles
                //                        join cd in this._ApplicationDbContext.ClientCourseDocuments on f.IdClientCourseDocument equals cd.IdClientCourseDocument
                //                        join cc in this._ApplicationDbContext.ClientCourses on cd.IdClientCourse equals cc.IdClientCourse
                //                        join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                //                        join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                //                        where cp.OldId == OldId
                //                        select f).ToList();

                //    DocsValidationClient = (from f in this._ApplicationDbContext.ValidationDocumentUploadedFiles
                //                            join cd in this._ApplicationDbContext.ValidationClientDocuments on f.IdValidationClientDocument equals cd.IdValidationClientDocument
                //                            join cc in this._ApplicationDbContext.ValidationClients on cd.IdValidationClient equals cc.IdValidationClient
                //                            join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                //                            join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                //                            where cp.OldId == OldId
                //                            select f).ToList();
                //}

                //foreach (var file in DocsClientCourse)
                //{
                //    var doc = tb_clients_courses_documents.Where(x => x.Id == file.OldId).First();

                //    if (file.MigrationNote.Equals("1"))
                //    {
                //        try
                //        {
                //            var url = $"\\UploadedFiles\\ClientCourseDocument\\{file.IdCourseDocumentUploadedFile}\\";

                //            var name = SaveDocument((int)doc.Document1File.Value, url);
                //            file.UploadedFileName = name;

                //            updatedDocsCourse.Add(file);
                //        }
                //        catch (Exception ex)
                //        {
                //        }
                //    }
                //    else
                //    {
                //        try
                //        {
                //            var url = $"\\UploadedFiles\\ClientCourseDocument\\{file.IdCourseDocumentUploadedFile}\\";

                //            var name = SaveDocument((int)doc.Document2File.Value, url);
                //            file.UploadedFileName = name;

                //            updatedDocsCourse.Add(file);
                //        }
                //        catch (Exception ex)
                //        {
                //        }
                //    }
                //}

                //foreach (var file in DocsValidationClient)
                //{
                //    var doc = tb_clients_courses_documents.Where(x => x.Id == file.OldId).First();

                //    if (file.MigrationNote.Equals("1"))
                //    {
                //        try
                //        {
                //            var url = $"\\UploadedFiles\\ValidationDocument\\{file.IdValidationDocumentUploadedFile}\\";

                //            var name = SaveDocument((int)doc.Document1File.Value, url);
                //            file.UploadedFileName = name;

                //            updatedDocsValidation.Add(file);
                //        }
                //        catch (Exception ex)
                //        {
                //        }
                //    }
                //    else
                //    {
                //        try
                //        {
                //            var url = $"\\UploadedFiles\\ValidationDocument\\{file.IdValidationDocumentUploadedFile}\\";

                //            var name = SaveDocument((int)doc.Document2File.Value, url);
                //            file.UploadedFileName = name;

                //            updatedDocsValidation.Add(file);
                //        }
                //        catch (Exception ex)
                //        {
                //        }
                //    }
                //}

                //this._ApplicationDbContext.BulkUpdate(updatedDocsCourse.ToList());
                //this._ApplicationDbContext.BulkUpdate(updatedDocsValidation.ToList());

            }

        }

        //TODO: РАДО КАТО ГО НАПРАВИШ МИ КАЖИ !!!

        public void ImportTrainingClientCourseDocumentStatus(int? OldId = null)
        {
            LogStrartInformation("ImportTrainingClientCourseDocumentStatus");

            //OldId = 933;

            var ref_clients_courses_documents_status = new List<RefClientsCoursesDocumentsStatus>();
            if (OldId is null)
                ref_clients_courses_documents_status = _jessieContextContext.RefClientsCoursesDocumentsStatuses.ToList();
            else
                ref_clients_courses_documents_status = _jessieContextContext.RefClientsCoursesDocumentsStatuses.Where(x => x.IntProviderId == OldId).ToList();

            var courseDocuments = _ApplicationDbContext.ClientCourseDocuments.ToList();

            var validationDocuments = _ApplicationDbContext.ValidationClientDocuments.ToList();

            var ClientDocumentStatusTypes = (from kv in _ApplicationDbContext.KeyValues
                                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                             where kt.KeyTypeIntCode == "ClientDocumentStatusType"
                                             select kv).To<KeyValueVM>().ToList();


            ConcurrentBag<ValidationClientDocumentStatus> validationStatuses = new ConcurrentBag<ValidationClientDocumentStatus>();
            ConcurrentBag<ClientCourseDocumentStatus> courseCtatuses = new ConcurrentBag<ClientCourseDocumentStatus>();

            Parallel.ForEach(ref_clients_courses_documents_status, stat =>
            {
                try
                {
                    if (stat.IntCourseGroupId is null)
                    {
                        ValidationClientDocumentStatus status = new ValidationClientDocumentStatus();

                        status.IdClientDocumentStatus = ClientDocumentStatusTypes.Where(x => Int32.Parse(x.DefaultValue2) == stat.IntDocumentStatus).First().IdKeyValue;

                        status.IdValidationClientDocument = validationDocuments.Where(x => x.OldId == stat.IntClientCoursesDocumentsId).First().IdValidationClientDocument;

                        status.SubmissionComment = stat.VcNote;

                        status.IdModifyUser = modifyUser.IdUser;

                        status.IdCreateUser = modifyUser.IdUser;

                        status.ModifyDate = DateTime.Now;

                        status.CreationDate = stat.Dt!.Value;

                        status.OldId = (int)stat.Id;

                        validationStatuses.Add(status);
                    }
                    else
                    {
                        ClientCourseDocumentStatus status = new ClientCourseDocumentStatus();

                        status.IdClientDocumentStatus = ClientDocumentStatusTypes.Where(x => Int32.Parse(x.DefaultValue2) == stat.IntDocumentStatus).First().IdKeyValue;

                        status.IdClientCourseDocument = courseDocuments.Where(x => x.OldId == stat.IntClientCoursesDocumentsId).First().IdClientCourseDocument;

                        status.SubmissionComment = stat.VcNote;

                        status.IdModifyUser = modifyUser.IdUser;

                        status.IdCreateUser = modifyUser.IdUser;

                        status.ModifyDate = DateTime.Now;

                        status.CreationDate = stat.Dt!.Value;

                        status.OldId = (int)stat.Id;

                        courseCtatuses.Add(status);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingClientCourseDocumentStatus(Първи Parallel.ForEach). Запис с Id = " + stat.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(validationStatuses.ToList());
            this._ApplicationDbContext.BulkInsert(courseCtatuses.ToList());

            LogEndInformation("ImportTrainingClientCourseDocumentStatus");
        }
        // РАДО ОПРАВИ ТОЗИ МЕТОД - АЛЕКС :)/ ГОТОВО -Радо (Може да има промени)
        //Най вероятно няма да се ползва
        public void ImportCandidateProviderManagementDocument()
        {
            var tb_candidate_trainer_documents = _jessieContextContext.TbCandidateTrainerDocuments.ToList();

            var trainers = _ApplicationDbContext.CandidateProviderTrainers.ToList();

            ConcurrentBag<CandidateProviderTrainerDocument> docs = new ConcurrentBag<CandidateProviderTrainerDocument>();

            Parallel.ForEach(tb_candidate_trainer_documents, doc =>
            {
                CandidateProviderTrainerDocumentVM docVM = new CandidateProviderTrainerDocumentVM();

                docVM.IdCandidateProviderTrainer = trainers.Where(x => x.OldId == doc.IntTrainerId).First().IdCandidateProviderTrainer;

                docVM.DocumentTitle = doc.TxtDocumentsManagementTitle;

                docs.Add(docVM.To<CandidateProviderTrainerDocument>());
            });

            _ApplicationDbContext.BulkInsert(docs.ToList());
        }
        public void ImportClientRequiredDocument(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            LogStrartInformation("ImportClientRequiredDocument");

            var tb_clients_required_documents = new List<TbClientsRequiredDocument>();

            //OldId = 1664;

            if (OldId is null)
            {
                tb_clients_required_documents = this._jessieContextContext.TbClientsRequiredDocuments.ToList();
            }
            else
            {
                //tb_clients_required_documents = (from tcrd in this._jessieContextContext.TbClientsRequiredDocuments
                //                                 join tcg in this._jessieContextContext.TbCourseGroups on tcrd.IntCourseGroupId equals tcg.Id
                //                                 join tc in this._jessieContextContext.TbCourses on tcg.IntCourseId equals tc.Id
                //                                 join rcc in this._jessieContextContext.RefClientsCourses on tcrd.IntClientId equals rcc.Id
                //                                 join t in this._jessieContextContext.TbClients on rcc.IntClientId equals t.Id
                //                                 where tc.IntProviderId == OldId || t.IntProviderId == OldId
                //                                 select tcrd).ToList();

                var tb_clients_required_documents1 = (from tcrd in this._jessieContextContext.TbClientsRequiredDocuments
                                                      join tcg in this._jessieContextContext.TbCourseGroups on tcrd.IntCourseGroupId equals tcg.Id
                                                      join tc in this._jessieContextContext.TbCourses on tcg.IntCourseId equals tc.Id
                                                      where tc.IntProviderId == OldId
                                                      select tcrd).ToList();

                var tb_clients_required_documents2 = (from tcrd in this._jessieContextContext.TbClientsRequiredDocuments
                                                      join rcc in this._jessieContextContext.RefClientsCourses on tcrd.IntClientId equals rcc.Id
                                                      join t in this._jessieContextContext.TbClients on rcc.IntClientId equals t.Id
                                                      where t.IntProviderId == OldId && tcrd.IntCourseGroupId == null
                                                      select tcrd).ToList();

                tb_clients_required_documents.AddRange(tb_clients_required_documents1);
                tb_clients_required_documents.AddRange(tb_clients_required_documents2);
            }

            var ClientCourseDocumentTypes = (from kv in _ApplicationDbContext.KeyValues
                                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                             where kt.KeyTypeIntCode == "ClientCourseDocumentType"
                                             select kv).To<KeyValueVM>().ToList();

            var Education = (from kv in _ApplicationDbContext.KeyValues
                             join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                             where kt.KeyTypeIntCode == "Education" && !string.IsNullOrEmpty(kv.DefaultValue2)
                             select kv).To<KeyValueVM>().ToList();

            var QualificationTypes = (from kv in _ApplicationDbContext.KeyValues
                                      join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                      where kt.KeyTypeIntCode == "MinimumQualificationLevel"
                                      select kv).To<KeyValueVM>().ToList();

            var ProtocolTypes = (from kv in _ApplicationDbContext.KeyValues
                                 join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                 where kt.KeyTypeIntCode == "CourseProtocolType"
                                 select kv).To<KeyValueVM>().ToList();

            var ProtocolType = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("Protocol")).First();

            var courses = this._ApplicationDbContext.Courses.ToList();
            var clients = this._ApplicationDbContext.Clients.ToList();
            var ClientCourses = this._ApplicationDbContext.ClientCourses.ToList();
            var validationClients = this._ApplicationDbContext.ValidationClients.ToList();

            var protocols = new ConcurrentBag<CourseProtocol>();
            var validationProtocols = new ConcurrentBag<ValidationProtocol>();
            var requiredDocuments = new ConcurrentBag<ClientRequiredDocument>();
            var requiredValidationDocuments = new ConcurrentBag<ValidationClientRequiredDocument>();


            var reports = new List<long>()
            {
                3, 4
            };

            var medicalDocuments = new List<long>()
            {
                1,5,6
            };

            var informationCard = new List<long>()
            {
                11, 12
            };

            var planGraph = new List<long>()
            {
                13, 14
            };

            var others = new List<long>()
            {
                7,8
            };


            // Protocols


            var validationProtocolId = new List<long>()
            {
                9,10
            };

            var p379 = new List<long>()
            {
                15,20
            };

            var p380t = new List<long>()
            {
                23,24, 32, 34
            };

            var p380p = new List<long>()
            {
                25,26,33,35
            };

            var p381B = new List<long>()
            {
                2,19
            };

            var p382 = new List<long>()
            {
                17,22
            };
            if (!OnlyDocuments)
            {
                Parallel.ForEach(tb_clients_required_documents, clientRequest =>
                {
                    try
                    {
                        if (clientRequest.IsValid!.Value)
                        {
                            if (clientRequest.IntClientId is not null)
                            {
                                var clientCourse = ClientCourses.Where(x => x.OldId == clientRequest.IntClientId).FirstOrDefault();

                                if (clientCourse is not null)
                                {
                                    var document = new ClientRequiredDocument();

                                    document.IdClientCourse = clientCourse.IdClientCourse;

                                    document.IdCourse = clientCourse.IdCourse;

                                    if (clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId is null)

                                        document.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("DocumentForEducation")).First().IdKeyValue;

                                    else if (reports.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        document.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("Report")).First().IdKeyValue;

                                    else if (medicalDocuments.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        document.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("MedicalDocument")).First().IdKeyValue;

                                    else if (informationCard.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        document.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("InformationCard")).First().IdKeyValue;

                                    else if (planGraph.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        document.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("PlanGraph")).First().IdKeyValue;
                                    else if (others.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        document.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("Other")).First().IdKeyValue;

                                    if (clientRequest.IntCodeEducationId is not null)
                                        document.IdEducation = Education.Where(x => Int32.Parse(x.DefaultValue2) == clientRequest.IntCodeEducationId).First().IdKeyValue;

                                    document.DocumentPrnNo = clientRequest.VcDocumentPrnNo;

                                    document.DocumentRegNo = clientRequest.VcDocumentRegNo;

                                    document.DocumentDate = clientRequest.DtDocumentDate;

                                    document.DocumentOfficialDate = clientRequest.DtDocumentOfficialDate;

                                    document.Description = clientRequest.VcDesciption;

                                    document.IsValid = clientRequest.IsValid!.Value;

                                    document.IsBeforeDate = clientRequest.BoolBeforeDate!.Value;

                                    if (clientRequest.IntCodeQualLevelId is not null)
                                        document.IdMinimumQualificationLevel = QualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientRequest.IntCodeQualLevelId).First().IdKeyValue;

                                    document.OldId = (int?)clientRequest.Id;

                                    if (clientRequest.OidFile is not null)
                                        document.MigrationNote = clientRequest.OidFile.ToString();

                                    document.UploadedFileName = "";

                                    document.IdModifyUser = modifyUser.IdUser;

                                    document.IdCreateUser = modifyUser.IdUser;

                                    document.ModifyDate = DateTime.Now;

                                    document.CreationDate = DateTime.Now;

                                    requiredDocuments.Add(document);
                                }
                                else if (clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId is not null && validationProtocolId.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId.Value))
                                {
                                    var protocol = new ValidationProtocol();

                                    var validationClient = validationClients.Where(x => x.OldId == clientRequest.IntClientId).First();

                                    protocol.IdValidationClient = validationClient.IdValidationClient;

                                    protocol.IdCandidateProvider = validationClient.IdCandidateProvider;

                                    protocol.ValidationProtocolNumber = String.Empty;

                                    protocol.ValidationProtocolDate = clientRequest.DtDocumentDate;

                                    protocol.IdValidationProtocolType = ProtocolType.IdKeyValue;

                                    protocol.Description = clientRequest.VcDesciption;

                                    protocol.UploadedFileName = "";

                                    protocol.IdModifyUser = modifyUser.IdUser;

                                    protocol.IdCreateUser = modifyUser.IdUser;

                                    protocol.ModifyDate = DateTime.Now;

                                    protocol.CreationDate = DateTime.Now;

                                    protocol.OldId = (int?)clientRequest.Id;

                                    if (clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId is not null)
                                    {
                                        if (p379.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                            protocol.IdValidationProtocol = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-79")).First().IdKeyValue;
                                        else if (p382.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                            protocol.IdValidationProtocol = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-82")).First().IdKeyValue;
                                        else if (p380t.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                            protocol.IdValidationProtocol = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-80t")).First().IdKeyValue;
                                        else if (p380p.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                            protocol.IdValidationProtocol = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-80p")).First().IdKeyValue;
                                        else if (p381B.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                            protocol.IdValidationProtocol = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-81B")).First().IdKeyValue;
                                        else
                                            protocol.IdValidationProtocol = ProtocolType.IdKeyValue;
                                    }

                                    if (clientRequest.OidFile is not null)
                                        protocol.MigrationNote = clientRequest.OidFile.ToString();

                                    validationProtocols.Add(protocol);
                                }
                                else
                                {
                                    var requiredDocument = new ValidationClientRequiredDocument();

                                    var validationClient = validationClients.Where(x => x.OldId == clientRequest.IntClientId).First();

                                    requiredDocument.IdValidationClient = validationClient.IdValidationClient;

                                    if (clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId is null)

                                        requiredDocument.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("DocumentForEducation")).First().IdKeyValue;

                                    else if (reports.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        requiredDocument.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("Report")).First().IdKeyValue;

                                    else if (medicalDocuments.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        requiredDocument.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("MedicalDocument")).First().IdKeyValue;

                                    else if (informationCard.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        requiredDocument.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("InformationCard")).First().IdKeyValue;

                                    else if (planGraph.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        requiredDocument.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("PlanGraph")).First().IdKeyValue;

                                    else if (others.Contains(clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId!.Value))

                                        requiredDocument.IdCourseRequiredDocumentType = ClientCourseDocumentTypes.Where(x => x.KeyValueIntCode.Equals("Other")).First().IdKeyValue;

                                    if (clientRequest.IntCodeEducationId is not null)
                                        requiredDocument.IdEducation = Education.Where(x => Int32.Parse(x.DefaultValue2) == clientRequest.IntCodeEducationId).First().IdKeyValue;

                                    requiredDocument.DocumentPrnNo = clientRequest.VcDocumentPrnNo;

                                    requiredDocument.DocumentRegNo = clientRequest.VcDocumentRegNo;

                                    requiredDocument.DocumentDate = clientRequest.DtDocumentDate;

                                    requiredDocument.DocumentOfficialDate = clientRequest.DtDocumentOfficialDate;

                                    requiredDocument.Description = clientRequest.VcDesciption;

                                    requiredDocument.IsValid = clientRequest.IsValid!.Value;

                                    requiredDocument.IsBeforeDate = clientRequest.BoolBeforeDate!.Value;

                                    if (clientRequest.IntCodeQualLevelId is not null)
                                        requiredDocument.IdMinimumQualificationLevel = QualificationTypes.Where(x => Int32.Parse(x.DefaultValue2) == clientRequest.IntCodeQualLevelId).First().IdKeyValue;

                                    requiredDocument.OldId = (int?)clientRequest.Id;

                                    if (clientRequest.OidFile is not null)
                                        requiredDocument.MigrationNote = clientRequest.OidFile.ToString();

                                    requiredDocument.UploadedFileName = "";

                                    requiredDocument.IdModifyUser = modifyUser.IdUser;

                                    requiredDocument.IdCreateUser = modifyUser.IdUser;

                                    requiredDocument.ModifyDate = DateTime.Now;

                                    requiredDocument.CreationDate = DateTime.Now;

                                    requiredValidationDocuments.Add(requiredDocument);
                                }
                            }
                            else
                            {
                                var protocol = new CourseProtocol();

                                var course = courses.Where(x => x.OldId == clientRequest.IntCourseGroupId).First();

                                protocol.IdCourse = course.IdCourse;

                                protocol.IdCandidateProvider = course.IdCandidateProvider!.Value;

                                protocol.CourseProtocolNumber = String.Empty;

                                protocol.CourseProtocolDate = clientRequest.DtDocumentDate;

                                //protocol.IdCourseProtocolType = ProtocolType.IdKeyValue;

                                protocol.Description = clientRequest.VcDesciption;

                                protocol.UploadedFileName = "";

                                protocol.IdModifyUser = modifyUser.IdUser;

                                protocol.IdCreateUser = modifyUser.IdUser;

                                protocol.ModifyDate = DateTime.Now;

                                protocol.CreationDate = DateTime.Now;

                                protocol.OldId = (int?)clientRequest.Id;

                                if (clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId is not null)
                                {
                                    if (p379.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                        protocol.IdCourseProtocolType = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-79")).First().IdKeyValue;
                                    else if (p382.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                        protocol.IdCourseProtocolType = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-82")).First().IdKeyValue;
                                    else if (p380t.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                        protocol.IdCourseProtocolType = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-80t")).First().IdKeyValue;
                                    else if (p380p.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                        protocol.IdCourseProtocolType = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-80p")).First().IdKeyValue;
                                    else if (p381B.Contains((long)clientRequest.IntCodeCourseGroupRequiredDocumentsTypeId))
                                        protocol.IdCourseProtocolType = ProtocolTypes.Where(x => x.KeyValueIntCode.Equals("3-81B")).First().IdKeyValue;
                                    else
                                        protocol.IdCourseProtocolType = ProtocolType.IdKeyValue;
                                }
                                else
                                {
                                    protocol.IdCourseProtocolType = ProtocolType.IdKeyValue;
                                }

                                if (clientRequest.OidFile is not null)
                                    protocol.MigrationNote = clientRequest.OidFile.ToString();

                                protocols.Add(protocol);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportClientRequiredDocument(Първи Parallel.ForEach). Запис с Id = " + clientRequest.Id);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });

                _ApplicationDbContext.BulkInsert(protocols.ToList());
                _ApplicationDbContext.BulkInsert(validationProtocols.ToList());
                _ApplicationDbContext.BulkInsert(requiredValidationDocuments.ToList());
                _ApplicationDbContext.BulkInsert(requiredDocuments.ToList());
            }

            LogEndInformation("ImportClientRequiredDocument");

            if (MigrateDocuments)
            {
                //var validationProtocolDocuments = new List<ValidationProtocol>();
                //var courseProtocolDocuments = new List<CourseProtocol>();
                //var courseRequiredDocuments = new List<ClientRequiredDocument>();
                //var validationRequiredDocuments = new List<ValidationClientRequiredDocument>();
                ////this._ApplicationDbContext.ValidationProtocols.ToList();
                //if (OldId is null)
                //{
                //    validationProtocolDocuments = this._ApplicationDbContext.ValidationProtocols.ToList();
                //    courseProtocolDocuments = this._ApplicationDbContext.CourseProtocols.ToList();
                //    courseRequiredDocuments = this._ApplicationDbContext.ClientRequiredDocuments.ToList();
                //    validationRequiredDocuments = this._ApplicationDbContext.ValidationClientRequiredDocuments.ToList();
                //}
                //else
                //{
                //    validationProtocolDocuments = (from vp in this._ApplicationDbContext.ValidationProtocols
                //                                   join vc in this._ApplicationDbContext.ValidationClients on vp.IdValidationClient equals vc.IdValidationClient
                //                                   join cp in this._ApplicationDbContext.CandidateProviders on vc.IdCandidateProvider equals cp.IdCandidate_Provider
                //                                   where cp.OldId == OldId
                //                                   select vp).ToList();

                //    courseProtocolDocuments = (from vp in this._ApplicationDbContext.CourseProtocols
                //                               join vc in this._ApplicationDbContext.Courses on vp.IdCourse equals vc.IdCourse
                //                               join cp in this._ApplicationDbContext.CandidateProviders on vc.IdCandidateProvider equals cp.IdCandidate_Provider
                //                               where cp.OldId == OldId
                //                               select vp).ToList();

                //    validationRequiredDocuments = (from vd in this._ApplicationDbContext.ValidationClientRequiredDocuments
                //                                   join vc in this._ApplicationDbContext.ValidationClients on vd.IdValidationClient equals vc.IdValidationClient
                //                                   join cp in this._ApplicationDbContext.CandidateProviders on vc.IdCandidateProvider equals cp.IdCandidate_Provider
                //                                   where cp.OldId == OldId
                //                                   select vd).ToList();

                //    courseRequiredDocuments = (from vd in this._ApplicationDbContext.ClientRequiredDocuments
                //                               join vc in this._ApplicationDbContext.Courses on vd.IdCourse equals vc.IdCourse
                //                               join cp in this._ApplicationDbContext.CandidateProviders on vc.IdCandidateProvider equals cp.IdCandidate_Provider
                //                               where cp.OldId == OldId
                //                               select vd).ToList();
                //}

                //var updatedValidationProtocolDocuments = new List<ValidationProtocol>();
                //var updatedCourseProtocolDocuments = new List<CourseProtocol>();
                //var updatedCourseRequiredDocuments = new List<ClientRequiredDocument>();
                //var updatedValidationRequiredDocuments = new List<ValidationClientRequiredDocument>();

                //foreach (var protocol in validationProtocolDocuments)
                //{
                //    var url = $"\\UploadedFiles\\ValidationProtocol\\{protocol.IdValidationProtocol}\\";
                //    if (!string.IsNullOrEmpty(protocol.MigrationNote))
                //    {
                //        var name = SaveDocument(Int32.Parse(protocol.MigrationNote), url);
                //        protocol.UploadedFileName = name;

                //        updatedValidationProtocolDocuments.Add(protocol);
                //    }
                //}

                //foreach (var protocol in courseProtocolDocuments)
                //{
                //    var url = $"\\UploadedFiles\\CourseProtocol\\{protocol.IdCourseProtocol}\\";
                //    if (!string.IsNullOrEmpty(protocol.MigrationNote))
                //    {
                //        var name = SaveDocument(Int32.Parse(protocol.MigrationNote), url);
                //        protocol.UploadedFileName = name;

                //        updatedCourseProtocolDocuments.Add(protocol);
                //    }
                //}

                //foreach (var doc in validationRequiredDocuments)
                //{
                //    var url = $"\\UploadedFiles\\ValidationEducation\\{doc.IdValidationClientRequiredDocument}\\";
                //    if (!string.IsNullOrEmpty(doc.MigrationNote))
                //    {
                //        var name = SaveDocument(Int32.Parse(doc.MigrationNote), url);
                //        doc.UploadedFileName = name;

                //        updatedValidationRequiredDocuments.Add(doc);
                //    }
                //}

                //foreach (var doc in courseRequiredDocuments)
                //{
                //    var url = $"\\UploadedFiles\\ClientCourseEducation\\{doc.IdClientRequiredDocument}\\";
                //    if (!string.IsNullOrEmpty(doc.MigrationNote))
                //    {
                //        var name = SaveDocument(Int32.Parse(doc.MigrationNote), url);
                //        doc.UploadedFileName = name;

                //        updatedCourseRequiredDocuments.Add(doc);
                //    }
                //}

                //_ApplicationDbContext.BulkUpdate(updatedValidationProtocolDocuments);
                //_ApplicationDbContext.BulkUpdate(updatedCourseProtocolDocuments);
                //_ApplicationDbContext.BulkUpdate(updatedCourseRequiredDocuments);
                //_ApplicationDbContext.BulkUpdate(updatedValidationRequiredDocuments);
                if (OldId is null)
                {
                    ValidationProtocolMigrateDocuments();
                    CourseProtocolMigrateDocuments();
                    ClientCourseRequiredDocumentsMigrateDocuments();
                    ValidationRequiredDocumentsMigrateDocuments();
                }
                else
                {
                    ValidationProtocolMigrateDocuments(OldId);
                    CourseProtocolMigrateDocuments(OldId);
                    ClientCourseRequiredDocumentsMigrateDocuments(OldId);
                    ValidationRequiredDocumentsMigrateDocuments(OldId);
                }
            }
        }
        public void ImportValidationCompetencies(int? OldId = null)
        {
            LogStrartInformation("ImportValidationCompetencies");

            //OldId = 1664;

            var tb_course40_competences = new List<TbCourse40Competence>();

            var provider = this._ApplicationDbContext
            .CandidateProviders.Where(x => x.IsActive && !x.MigrationNote.Equals("cp") && x.OldId == OldId)
            .FirstOrDefault();

            if (provider is null) return;

            if (OldId is null)
            {
                tb_course40_competences = this._jessieContextContext.TbCourse40Competences.ToList();
            }
            else
            {
                tb_course40_competences = (from cc in this._jessieContextContext.TbCourse40Competences
                                           join rcc in this._jessieContextContext.RefClientsCourses on cc.IntClientId equals rcc.Id
                                           join tc in this._jessieContextContext.TbClients on rcc.IntClientId equals tc.Id
                                           where tc.IntProviderId == OldId
                                           select cc).ToList();
            }

            var competencies = new ConcurrentBag<ValidationCompetency>();

            var validationClients = this._ApplicationDbContext.ValidationClients.ToList();

            Parallel.ForEach(tb_course40_competences, competency =>
            {
                var validationCompetency = new ValidationCompetency();

                var validationClient = validationClients.Where(x => x.OldId == competency.IntClientId).First();

                validationCompetency.IdValidationClient = validationClient.IdValidationClient;

                validationCompetency.CompetencyNumber = 0;

                validationCompetency.Competency = competency.VcCompetence;

                validationCompetency.IsCompetencyRecognized = true;

                competencies.Add(validationCompetency);
            });

            _ApplicationDbContext.BulkInsert(competencies.ToList());

            if (OldId is null)
            {
                validationClients = this._ApplicationDbContext.ValidationClients.Include(x => x.ValidationCompetencies).ToList();
            }
            else
            {


                validationClients = this._ApplicationDbContext.ValidationClients.Where(x => x.IdCandidateProvider == provider.IdCandidate_Provider).Include(x => x.ValidationCompetencies).ToList();

            }

            var competenciesToUpdate = new ConcurrentBag<ValidationCompetency>();

            Parallel.ForEach(validationClients, client =>
            {
                int index = 0;

                if (client.ValidationCompetencies.Any())
                {
                    foreach (var competency in client.ValidationCompetencies)
                    {
                        competency.CompetencyNumber = ++index;

                        competenciesToUpdate.Add(competency);
                    }
                }
            });

            _ApplicationDbContext.BulkUpdate(competenciesToUpdate.ToList());

            LogEndInformation("ImportValidationCompetencies");

        }
        public void ImportCourseCandidateCurriculumModification(int? OldId = null)
        {
            LogStrartInformation("ImportCourseCandidateCurriculumModification");

            var ref_cg_curric_files = new List<RefCgCurricFile>();

            if (OldId is null)
                ref_cg_curric_files = this._jessieContextContext.RefCgCurricFiles.ToList();
            else
                ref_cg_curric_files = (from rccf in this._jessieContextContext.RefCgCurricFiles
                                       join tc in this._jessieContextContext.TbCourses on rccf.IntCourseId equals tc.Id
                                       where tc.IntProviderId == OldId
                                       select rccf).ToList();

            var courses = this._ApplicationDbContext.Courses.ToList();
            var currics = this._ApplicationDbContext.CandidateCurriculumModification.ToList();

            var courseModifications = new ConcurrentBag<CourseCandidateCurriculumModification>();

            Parallel.ForEach(ref_cg_curric_files, file =>
            {
                try
                {
                    if (file.IsValid!.Value)
                    {
                        var courseCurriculumModification = new CourseCandidateCurriculumModification();

                        courseCurriculumModification.IdCourse = courses.Where(x => x.OldId == file.IntCourseGroupId).First().IdCourse;

                        courseCurriculumModification.IdCandidateCurriculumModification = currics
                        .Where(x => x.OldId == file.IntProviderSpecialitiesCurriculumId).First().IdCandidateCurriculumModification;

                        courseModifications.Add(courseCurriculumModification);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportCourseCandidateCurriculumModification(Първи Parallel.ForEach). Запис с Id = " + file.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(courseModifications.ToList());

            LogEndInformation("ImportCourseCandidateCurriculumModification");
        }
        public void ImportTrainingCourseSubject(int? OldId = null)
        {
            LogStrartInformation("ImportTrainingCourseSubject");

            var TrainingCurriculum = new List<TrainingCurriculum>();

            if (OldId is null)
                TrainingCurriculum = this._ApplicationDbContext.TrainingCurriculums.ToList();
            else
                TrainingCurriculum = (from tc in this._ApplicationDbContext.TrainingCurriculums
                                      join c in this._ApplicationDbContext.Courses on tc.IdCourse equals c.IdCourse
                                      join cp in this._ApplicationDbContext.CandidateProviders on c.IdCandidateProvider equals cp.IdCandidate_Provider
                                      where cp.OldId == OldId
                                      select tc).ToList();

            var subjects = new ConcurrentBag<CourseSubject>();

            Parallel.ForEach(TrainingCurriculum, currics =>
            {
                try
                {
                    if (currics.IdCourse is not null)
                    {
                        CourseSubject subject = new CourseSubject();

                        subject.IdCourse = currics.IdCourse!.Value;

                        subject.IdProfessionalTraining = currics.IdProfessionalTraining;

                        subject.Subject = currics.Subject;
                        if (currics.Practice.HasValue)
                            subject.PracticeHours = currics.Practice.Value;
                        else
                            subject.PracticeHours = 0;

                        if (currics.Theory.HasValue)
                            subject.TheoryHours = currics.Theory.Value;
                        else
                            subject.TheoryHours = 0;

                        subjects.Add(subject);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingCourseSubject(Първи Parallel.ForEach). Запис с Id = " + currics.IdCandidateCurriculum);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            this._ApplicationDbContext.BulkInsert(subjects.ToList());

            LogEndInformation("ImportTrainingCourseSubject");
        }

        public void UpdateOidClientCourseDocument()
        {
            LogStrartInformation("UpdateOidClientCourseDocument");

            var CourseUploadedFiles = this._ApplicationDbContext.CourseDocumentUploadedFiles.ToList();

            var tb_clients_courses_documents = this._jessieContextContext.TbClientsCoursesDocuments.ToList();

            var coursesToUpdate = new ConcurrentBag<CourseDocumentUploadedFile>();

            Parallel.ForEach(CourseUploadedFiles, doc =>
            {
                var docFromOldDb = tb_clients_courses_documents.Where(x => x.Id == doc.OldId).First();

                if (doc.MigrationNote.Equals("1"))
                    doc.Oid = docFromOldDb.Document1File.ToString();
                else
                    doc.Oid = docFromOldDb.Document2File.ToString();

                coursesToUpdate.Add(doc);
            });

            this._ApplicationDbContext.BulkUpdate(coursesToUpdate.ToList());

            LogEndInformation("UpdateOidClientCourseDocument");
        }

        public void UpdateOidValidationDocument()
        {
            LogStrartInformation("UpdateOidValidationDocument");

            var ValidationUploadedFiles = this._ApplicationDbContext.ValidationDocumentUploadedFiles.ToList();

            var tb_clients_courses_documents = this._jessieContextContext.TbClientsCoursesDocuments.ToList();

            var documentToUpdate = new ConcurrentBag<ValidationDocumentUploadedFile>();

            Parallel.ForEach(ValidationUploadedFiles, doc =>
            {
                var docFromOldDb = tb_clients_courses_documents.Where(x => x.Id == doc.OldId).First();

                if (doc.MigrationNote.Equals("1"))
                    doc.Oid = docFromOldDb.Document1File.ToString();
                else
                    doc.Oid = docFromOldDb.Document2File.ToString();

                documentToUpdate.Add(doc);
            });

            this._ApplicationDbContext.BulkUpdate(documentToUpdate.ToList());

            LogEndInformation("UpdateOidValidationDocument");
        }

        public void UpdateTrainingClientCourseDocumentStatus()
        {
            LogStrartInformation("UpdateTrainingClientCourseDocumentStatus");
            var clientCourseDocumentStatuses = this._ApplicationDbContext.ClientCourseDocumentStatuses.ToList();

            var documentsToUpdate = (from rccds in this._jessieContextContext.RefClientsCoursesDocumentsStatuses
                                     join tu in this._jessieContextContext.TbUsers on rccds.IntUserId equals tu.Id
                                     where tu.IntGlobalGroupId == 4
                                     select rccds).ToList();

            var oldUsers = this._jessieContextContext.TbUsers.ToList();

            var experts = this._ApplicationDbContext.Experts.Include(x => x.Person).ToList();

            var appUsers = this._ApplicationDbContext.Users.ToList();

            var updatedDocuments = new ConcurrentBag<ClientCourseDocumentStatus>();

            Parallel.ForEach(documentsToUpdate, docToUpdate =>
            {
                var oldUser = oldUsers.Where(x => x.Id == docToUpdate.IntUserId).First();

                var names = oldUser.VcFullname.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                
                var expert = new Expert();

                var currentExpers = experts.Where(x => x.Person.FirstName.Contains(names[0]));

                if (names.Length == 2)
                {
                    expert = currentExpers.Where(x => x.Person.FamilyName.Contains(names[1])).FirstOrDefault();
                    if(expert is null)
                        expert = currentExpers.Where(x => x.Person.SecondName is not null && x.Person.SecondName.Contains(names[1])).FirstOrDefault();
                }
                else
                {
                    expert = currentExpers.Where(x => x.Person.SecondName is not null && x.Person.SecondName.Contains(names[1]) && x.Person.FamilyName.Contains(names[2])).FirstOrDefault();
                }

                if (expert is null || expert.IdExpert == 0 || !expert.IsNapooExpert) return;

                var clientCourseDocStatus = clientCourseDocumentStatuses.Where(x => x.OldId == docToUpdate.Id).FirstOrDefault();

                if (clientCourseDocStatus is null) return;

                var currentUser = appUsers.Where(x => x.IdPerson == expert.IdPerson).First();

                clientCourseDocStatus.IdCreateUser = currentUser.IdUser;

                clientCourseDocStatus.IdModifyUser = currentUser.IdUser;

                updatedDocuments.Add(clientCourseDocStatus);
            });

            this._ApplicationDbContext.UpdateRange(updatedDocuments.ToList());
            this._ApplicationDbContext.SaveChanges();
            LogEndInformation("UpdateTrainingClientCourseDocumentStatus");
        }

        #region Arch

        public void ImportArchTrainingCourse(int? OldId = null)
        {
            //OldId = 1664;
            LogStrartInformation("ImportArchTrainingCourse");

            var tb_course_groups = new List<TbCourseGroup>();
            if (OldId is null)
            {
                tb_course_groups = _jessieContextContext.TbCourseGroups.ToList();
            }
            else
            {
                var progs = _jessieContextContext.TbCourses.Where(x => x.IntProviderId == OldId).ToList();
                tb_course_groups = _jessieContextContext.TbCourseGroups.ToList().Where(x => progs.Any(z => z.Id == x.IntCourseId)).ToList();
            }

            var arch_tb_course_groups = _jessieContextContext.ArchTbCourseGroups.ToList();

            var courses = this._ApplicationDbContext.Courses.ToList();

            ConcurrentBag<Course> updateList = new ConcurrentBag<Course>();

            Parallel.ForEach(tb_course_groups, group =>
            {
                try
                {
                    var currentYearMinus3 = DateTime.Now.Year - 3;

                    if (group.DtEndDate.HasValue && currentYearMinus3 > group.DtEndDate.Value.Year)
                    {
                        var course = courses.Where(x => x.OldId == group.Id).First();

                        course.IsArchived = true;
                        updateList.Add(course);
                    }
                    else
                    {
                        if (group.DtEndDate.HasValue)
                        {
                            var arch = arch_tb_course_groups.Where(x => x.Id == group.Id && x.IntCourseStatusId == 3 && x.IntYear == group.DtEndDate.Value.Year).FirstOrDefault();
                            if (arch is not null)
                            {
                                var updateCourse = courses.Where(x => x.OldId == arch.Id).First();

                                updateCourse.IsArchived = true;

                                updateCourse.ClientCourses = null;
                                updateCourse.CourseCommissionMembers = null;
                                updateCourse.CourseSubjects = null;
                                updateCourse.CourseProtocols = null;
                                updateCourse.ClientRequiredDocuments = null;
                                updateCourse.CourseOrders = null;
                                updateCourse.CandidateProvider = null;
                                updateCourse.Location = null;
                                updateCourse.Program = null;
                                updateCourse.CandidateProviderPremises = null;

                                updateList.Add(updateCourse);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportTrainingCourse(Първи Parallel.ForEach). Запис с Id = " + group.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkUpdate(updateList.ToList());

            LogEndInformation("ImportArchTrainingCourse");
        }

        public void ImportArchTrainingValidation(int? OldId = null)
        {
            //OldId = 1664;

            LogStrartInformation("ImportArchTrainingValidation");

            var arch_ref_clients_courses = this._jessieContextContext.ArchRefClientsCourses.ToList();

            var validationClients = this._ApplicationDbContext.ValidationClients.ToList();

            var ref_clients_courses = new List<RefClientsCourse>();

            if (OldId is null)
            {
                ref_clients_courses = this._jessieContextContext.RefClientsCourses.Where(x => x.IntCourseGroupId == null).ToList();
            }
            else
            {
                //var cli = _jessieContextContext.TbClients.Where(x => x.IntProviderId == OldId).ToList();
                //var refclients = this._jessieContextContext.RefClientsCourses.ToList();
                //var listForClients = new ConcurrentBag<RefClientsCourse>();
                //Parallel.ForEach(refclients, cl =>
                //{
                //    if (cl.IntCourseGroupId is null)
                //    {
                //        var DoesExist = cli.Where(x => x.Id == cl.IntClientId).FirstOrDefault();
                //        if (DoesExist is not null)
                //            listForClients.Add(cl);
                //    }
                //});

                //ref_clients_courses = listForClients.ToList();

                //arch_ref_clients_courses = 

                ref_clients_courses = (from rcc in this._jessieContextContext.RefClientsCourses
                                       join tc in this._jessieContextContext.TbClients on rcc.IntClientId equals tc.Id
                                       where rcc.IntCourseGroupId == null && tc.IntProviderId == OldId
                                       select rcc).ToList();
            }

            ConcurrentBag<ValidationClient> updateList = new ConcurrentBag<ValidationClient>();

            Parallel.ForEach(ref_clients_courses, client =>
            {
                var dateMinusYear = DateTime.Now.AddYears(-1);
                var dateMinus2Years = DateTime.Now.AddYears(-2);

                var arch = arch_ref_clients_courses.Where(x => x.DtCourseFinished < dateMinusYear && x.Id == client.IntClientId).FirstOrDefault();

                if (arch is not null || client.DtCourseFinished < dateMinus2Years)
                {
                    var validationClient = validationClients.Where(x => x.OldId == client.Id).First();

                    validationClient.IsArchived = true;
                    updateList.Add(validationClient);
                }
            });

            this._ApplicationDbContext.BulkUpdate(updateList.ToList());

            LogEndInformation("ImportArchTrainingValidation");
        }

        #endregion

        #endregion

        #region Arch

        public void ImportAnnualInfo(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {
            //OldId = 2929;
            LogStrartInformation("ImportAnnualInfo");



            var approvedStatus = (from kv in _ApplicationDbContext.KeyValues
                                  join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                  where kt.KeyTypeIntCode == "AnnualInfoStatusType" && kv.KeyValueIntCode == "Approved"
                                  select kv).To<KeyValueVM>().First();

            var candidates = _ApplicationDbContext.CandidateProviders.ToList();

            var tb_annual_info = new List<TbAnnualInfo>();

            if (OldId is null)
            {
                tb_annual_info = _jessieContextContext.TbAnnualInfos.ToList();

                if (this._ApplicationDbContext.AnnualInfoStatuses.Any())
                {
                    this._ApplicationDbContext.AnnualInfoStatuses.BatchDelete();
                }

                if (this._ApplicationDbContext.AnnualInfos.Any())
                {
                    this._ApplicationDbContext.AnnualInfos.BatchDelete();
                }
            }
            else
            {
                tb_annual_info = _jessieContextContext.TbAnnualInfos.Where(x => x.IntProviderId == OldId).ToList();
            }



            ConcurrentBag<AnnualInfo> annualInfoList = new ConcurrentBag<AnnualInfo>();

            Parallel.ForEach(tb_annual_info, annual_info =>
            {
                try
                {

                    var candidate = candidates.Where(x => x.OldId == annual_info.IntProviderId && x.IsActive && !x.MigrationNote.Equals("cp")).FirstOrDefault();


                    if (candidate == null)
                    {
                        return;
                    }

                    AnnualInfo annualInfo = new AnnualInfo();
                    annualInfo.IdCandidateProvider = candidate.IdCandidate_Provider;
                    annualInfo.Year = (int)annual_info.IntYear;
                    annualInfo.Name = annual_info.VcName;
                    annualInfo.Title = annual_info.VcPosition;
                    annualInfo.Phone = annual_info.VcPhone;
                    annualInfo.Email = annual_info.VcEmail;
                    annualInfo.CreationDate = annual_info.DtTimestamp.Value;
                    annualInfo.ModifyDate = annual_info.DtTimestamp.Value;
                    annualInfo.IdCreateUser = modifyUser.IdUser;
                    annualInfo.IdModifyUser = modifyUser.IdUser;
                    annualInfo.IdStatus = approvedStatus.IdKeyValue;



                    annualInfoList.Add(annualInfo);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод ImportAnnualInfo( Parallel.ForEach). Запис с Id = " + annual_info.Id);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            });

            _ApplicationDbContext.BulkInsert(annualInfoList.ToList());



            ConcurrentBag<AnnualInfoStatus> annualInfoStatusList = new ConcurrentBag<AnnualInfoStatus>();

            if (OldId is null)
            {

                var inserteedAnnualInfos = _ApplicationDbContext.AnnualInfos;

                Parallel.ForEach(inserteedAnnualInfos, annualInfo =>
                {
                    AnnualInfoStatus annualInfoStatus = new AnnualInfoStatus();

                    annualInfoStatus.IdAnnualInfo = annualInfo.IdAnnualInfo;
                    annualInfoStatus.CreationDate = annualInfo.CreationDate;
                    annualInfoStatus.ModifyDate = annualInfo.ModifyDate;
                    annualInfoStatus.IdCreateUser = modifyUser.IdUser;
                    annualInfoStatus.IdModifyUser = modifyUser.IdUser;
                    annualInfoStatus.UploadedFileName = "";
                    //annualInfoStatus.Comment = "Миграция от ИС на НАПОО";
                    annualInfoStatus.IdStatus = approvedStatus.IdKeyValue;

                    annualInfoStatusList.Add(annualInfoStatus);

                });

            }
            else
            {
                var candidate = candidates.Where(x => x.OldId == OldId).FirstOrDefault();

                var annualInfos = _ApplicationDbContext.AnnualInfos.Where(x => x.IdCandidateProvider == candidate.IdCandidate_Provider).ToList();

                Parallel.ForEach(annualInfos, annualInfo =>
                {
                    AnnualInfoStatus annualInfoStatus = new AnnualInfoStatus();

                    annualInfoStatus.IdAnnualInfo = annualInfo.IdAnnualInfo;
                    annualInfoStatus.CreationDate = annualInfo.CreationDate;
                    annualInfoStatus.ModifyDate = annualInfo.ModifyDate;
                    annualInfoStatus.IdCreateUser = modifyUser.IdUser;
                    annualInfoStatus.IdModifyUser = modifyUser.IdUser;
                    annualInfoStatus.UploadedFileName = "";
                    //annualInfoStatus.Comment = "Миграция от ИС на НАПОО";
                    annualInfoStatus.IdStatus = approvedStatus.IdKeyValue;

                    annualInfoStatusList.Add(annualInfoStatus);
                });
            }

            _ApplicationDbContext.BulkInsert(annualInfoStatusList.ToList());

            var annualInfoCIPO = new List<TbProviderUploadedDoc>();

            if (OldId is null)
                annualInfoCIPO = this._jessieContextContext.TbProviderUploadedDocs.Select(o => new TbProviderUploadedDoc
                {
                    Id = o.Id,
                    IntUploadDocTypeId = o.IntUploadDocTypeId,
                    IntYear = o.IntYear,
                    TxtDocDescription = o.TxtDocDescription,
                    IntDocStatusId = o.IntDocStatusId,
                    DtDocUploadDate = o.DtDocUploadDate,
                    TxtFileName = o.TxtFileName,
                    IntProviderId = o.IntProviderId
                })
                .Where(x => x.IntUploadDocTypeId == 14).ToList();
            else
                annualInfoCIPO = this._jessieContextContext.TbProviderUploadedDocs.Select(o => new TbProviderUploadedDoc
                {
                    Id = o.Id,
                    IntUploadDocTypeId = o.IntUploadDocTypeId,
                    IntYear = o.IntYear,
                    TxtDocDescription = o.TxtDocDescription,
                    IntDocStatusId = o.IntDocStatusId,
                    DtDocUploadDate = o.DtDocUploadDate,
                    TxtFileName = o.TxtFileName,
                    IntProviderId = o.IntProviderId
                })
                .Where(x => x.IntUploadDocTypeId == 14 && x.IntProviderId == OldId).ToList();

            annualInfoList = new ConcurrentBag<AnnualInfo>();


            Parallel.ForEach(annualInfoCIPO, annualInfo =>
            {
                AnnualInfo annual = new AnnualInfo();

                var candidate = candidates.Where(x => x.OldId == annualInfo.IntProviderId).FirstOrDefault();


                if (candidate == null)
                {
                    return;
                }

                AnnualInfo info = new AnnualInfo();
                info.IdCandidateProvider = candidate.IdCandidate_Provider;
                info.Year = (int)annualInfo.IntYear;
                info.Name = annualInfo.TxtDocDescription;
                info.CreationDate = annualInfo.DtDocUploadDate.Value;
                info.ModifyDate = annualInfo.DtDocUploadDate.Value;
                info.IdCreateUser = modifyUser.IdUser;
                info.IdModifyUser = modifyUser.IdUser;
                info.IdStatus = approvedStatus.IdKeyValue;
                info.MigrationNote = "cipo";
                info.OldId = (int?)annualInfo.Id;

                annualInfoList.Add(info);
            });

            _ApplicationDbContext.BulkInsert(annualInfoList.ToList());
            annualInfoStatusList = new ConcurrentBag<AnnualInfoStatus>();
            if (OldId is null)
            {

                var inserteedAnnualInfos = (from ai in this._ApplicationDbContext.AnnualInfos
                                            join cp in this._ApplicationDbContext.CandidateProviders on ai.IdCandidateProvider equals cp.IdCandidate_Provider
                                            where ai.MigrationNote.Equals("cipo")
                                            select ai).ToList();

                Parallel.ForEach(inserteedAnnualInfos, info =>
                {
                    var candidate = candidates.Where(x => x.IdCandidate_Provider == info.IdCandidateProvider).FirstOrDefault();

                    var infos = annualInfoCIPO.Where(x => x.IntYear == info.Year && x.IntProviderId == candidate.OldId).ToList();

                    foreach (var annualInfo in infos)
                    {
                        AnnualInfoStatus annualInfoStatus = new AnnualInfoStatus();

                        annualInfoStatus.IdAnnualInfo = info.IdAnnualInfo;
                        annualInfoStatus.CreationDate = info.CreationDate;
                        annualInfoStatus.ModifyDate = info.ModifyDate;
                        annualInfoStatus.IdCreateUser = modifyUser.IdUser;
                        annualInfoStatus.IdModifyUser = modifyUser.IdUser;
                        //annualInfoStatus.Comment = "Миграция от ИС на НАПОО";
                        annualInfoStatus.IdStatus = approvedStatus.IdKeyValue;
                        annualInfoStatus.OldId = (int?)annualInfo.Id;
                        annualInfoStatus.MigrationNote = "cipo";
                        annualInfoStatus.UploadedFileName = "";
                        annualInfoStatusList.Add(annualInfoStatus);
                    }
                });

            }
            else
            {
                var annualInfos = (from ai in this._ApplicationDbContext.AnnualInfos
                                   join cp in this._ApplicationDbContext.CandidateProviders on ai.IdCandidateProvider equals cp.IdCandidate_Provider
                                   where cp.OldId == OldId
                                   select ai).ToList();

                Parallel.ForEach(annualInfos, info =>
                {
                    var infos = annualInfoCIPO.Where(x => x.IntYear == info.Year && x.IntProviderId == OldId).ToList();
                    foreach (var annualInfo in infos)
                    {
                        AnnualInfoStatus annualInfoStatus = new AnnualInfoStatus();

                        annualInfoStatus.IdAnnualInfo = info.IdAnnualInfo;
                        annualInfoStatus.CreationDate = info.CreationDate;
                        annualInfoStatus.ModifyDate = info.ModifyDate;
                        annualInfoStatus.IdCreateUser = modifyUser.IdUser;
                        annualInfoStatus.IdModifyUser = modifyUser.IdUser;
                        //annualInfoStatus.Comment = "Миграция от ИС на НАПОО";
                        annualInfoStatus.IdStatus = approvedStatus.IdKeyValue;
                        annualInfoStatus.MigrationNote = "cipo";
                        annualInfoStatus.UploadedFileName = "";
                        annualInfoStatus.OldId = (int?)annualInfo.Id;
                        annualInfoStatusList.Add(annualInfoStatus);
                    }
                });
            }
            this._ApplicationDbContext.BulkInsert(annualInfoStatusList.ToList());

            LogEndInformation("ImportAnnualInfo");

            if (MigrateDocuments && annualInfoCIPO.Any())
            {
                //var statusesForDocument = this._ApplicationDbContext.AnnualInfoStatuses
                //    .Where(x => !string.IsNullOrEmpty(x.MigrationNote)).ToList();

                //var updateStatuses = new List<AnnualInfoStatus>();

                //foreach(var status in statusesForDocument)
                //{
                //    var doc = annualInfoCIPO.Where(x => x.Id == status.OldId).First();

                //    if (doc.BinFile is not null)
                //    {
                //        var url = $"\\UploadedFiles\\AnnualInfoStatus\\{status.IdAnnualInfo}";

                //        SaveBinDocument(url, doc.BinFile, doc.TxtFileName);
                //        status.UploadedFileName = doc.TxtFileName;

                //        updateStatuses.Add(status);
                //    }
                //}

                //this._ApplicationDbContext.BulkUpdate(updateStatuses);
                if (OldId is null)
                    AnnualInfoMigrateDocuments();
                else
                    AnnualInfoMigrateDocuments(OldId);
            }

        }

        public void ImportSelfAssessmentReport(int? OldId = null, bool MigrateDocuments = true, bool OnlyDocuments = false)
        {

            LogStrartInformation("ImportSelfAssessmentReport");

            var selfAssessmentReports = new List<TbProviderUploadedDoc>();

            var SelfAssessmentReportStatuses = (from kv in _ApplicationDbContext.KeyValues
                                                join kt in _ApplicationDbContext.KeyTypes on kv.IdKeyType equals kt.IdKeyType
                                                where kt.KeyTypeIntCode == "SelfAssessmentReportStatus" && !string.IsNullOrEmpty(kv.DefaultValue2)
                                                select kv).To<KeyValueVM>().ToList();

            if (OldId is null)
                selfAssessmentReports = this._jessieContextContext.TbProviderUploadedDocs.Select(o => new TbProviderUploadedDoc
                {
                    Id = o.Id,
                    IntUploadDocTypeId = o.IntUploadDocTypeId,
                    IntYear = o.IntYear,
                    TxtDocDescription = o.TxtDocDescription,
                    IntDocStatusId = o.IntDocStatusId,
                    DtDocUploadDate = o.DtDocUploadDate,
                    TxtFileName = o.TxtFileName,
                    IntProviderId = o.IntProviderId
                }).Where(x => x.IntUploadDocTypeId == 3 || x.IntUploadDocTypeId == 13).ToList();
            else
                selfAssessmentReports = this._jessieContextContext.TbProviderUploadedDocs.Select(o => new TbProviderUploadedDoc
                {
                    Id = o.Id,
                    IntUploadDocTypeId = o.IntUploadDocTypeId,
                    IntYear = o.IntYear,
                    TxtDocDescription = o.TxtDocDescription,
                    IntDocStatusId = o.IntDocStatusId,
                    DtDocUploadDate = o.DtDocUploadDate,
                    TxtFileName = o.TxtFileName,
                    IntProviderId = o.IntProviderId
                }).Where(x => x.IntProviderId == OldId && (x.IntUploadDocTypeId == 3 || x.IntUploadDocTypeId == 13)).ToList();

            var candidateProvider = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive).ToList();

            var reports = new ConcurrentBag<SelfAssessmentReport>();
            if (!OnlyDocuments)
            {
                Parallel.ForEach(selfAssessmentReports, report =>
                {
                    if (report.IntDocStatusId != 2) return;

                    SelfAssessmentReport selfAssessmentReport = new SelfAssessmentReport();

                    if (report.IntYear.HasValue)
                        selfAssessmentReport.Year = (int)report.IntYear!.Value;
                    else
                        selfAssessmentReport.Year = report.DtDocUploadDate!.Value.Year - 1;

                    selfAssessmentReport.IdCandidateProvider = candidateProvider.Where(x => x.OldId == report.IntProviderId).First().IdCandidate_Provider;

                    selfAssessmentReport.FilingDate = report.DtDocUploadDate;

                    selfAssessmentReport.IdModifyUser = modifyUser.IdUser;

                    selfAssessmentReport.IdCreateUser = modifyUser.IdUser;

                    selfAssessmentReport.ModifyDate = DateTime.Now;

                    selfAssessmentReport.CreationDate = report.DtDocUploadDate!.Value;

                    selfAssessmentReport.MigrationNote = report.IntProviderId.ToString();

                    selfAssessmentReport.OldId = (int?)report.Id;

                    reports.Add(selfAssessmentReport);
                });

                this._ApplicationDbContext.BulkInsert(reports.ToList());

                var currentReports = new List<SelfAssessmentReport>();

                if (OldId is null)
                    currentReports = this._ApplicationDbContext.SelfAssessmentReports.ToList();
                else
                    currentReports = (from sar in this._ApplicationDbContext.SelfAssessmentReports
                                      join cp in this._ApplicationDbContext.CandidateProviders on sar.IdCandidateProvider equals cp.IdCandidate_Provider
                                      where cp.OldId == OldId
                                      select sar).ToList();

                var statusesForDb = new ConcurrentBag<SelfAssessmentReportStatus>();

                Parallel.ForEach(currentReports, report =>
                {
                    var statuses = selfAssessmentReports
                    .Where(x => x.IntProviderId == Int32.Parse(report.MigrationNote) && x.IntYear == report.Year)
                    .ToList();

                    foreach (var status in statuses)
                    {
                        SelfAssessmentReportStatus reportStatus = new SelfAssessmentReportStatus();

                        reportStatus.IdSelfAssessmentReport = report.IdSelfAssessmentReport;

                        reportStatus.IdStatus = SelfAssessmentReportStatuses.Where(x => Int32.Parse(x.DefaultValue2) == status.IntDocStatusId).First().IdKeyValue;

                        if (status.TxtDocDescription.Length < 1000)
                            reportStatus.Comment = status.TxtDocDescription;
                        else
                            reportStatus.Comment = status.TxtDocDescription.Substring(0, 1000);

                        reportStatus.CreationDate = status.DtDocUploadDate!.Value;

                        reportStatus.IdModifyUser = modifyUser.IdUser;

                        reportStatus.IdCreateUser = modifyUser.IdUser;

                        reportStatus.ModifyDate = DateTime.Now;

                        reportStatus.OldId = (int?)status.Id;

                        reportStatus.UploadedFileName = String.Empty;

                        statusesForDb.Add(reportStatus);
                    }
                });

                this._ApplicationDbContext.BulkInsert(statusesForDb.ToList());
            }
            var statusesFromDb = new List<SelfAssessmentReportStatus>();

            LogEndInformation("ImportSelfAssessmentReport");

            if (MigrateDocuments)
            {
                //if (OldId is null)
                //    statusesFromDb = this._ApplicationDbContext.SelfAssessmentReportStatuses.ToList();
                //else
                //    statusesFromDb = (from s in this._ApplicationDbContext.SelfAssessmentReportStatuses
                //                      join sar in this._ApplicationDbContext.SelfAssessmentReports on s.IdSelfAssessmentReport equals sar.IdSelfAssessmentReport
                //                      join cp in this._ApplicationDbContext.CandidateProviders on sar.IdCandidateProvider equals cp.IdCandidate_Provider
                //                      where cp.OldId == OldId
                //                      select s).ToList();
                //var updateStatuses = new ConcurrentBag<SelfAssessmentReportStatus>();

                //foreach (var status in statusesFromDb)
                //{
                //    var doc = selfAssessmentReports.Where(x => x.Id == status.OldId).First();

                //    if (doc.BinFile is not null)
                //    {
                //        var url = $"\\UploadedFiles\\SelfAssessmentReportStatus\\{status.IdSelfAssessmentReport}";

                //        SaveBinDocument(url, doc.BinFile, doc.TxtFileName);
                //        status.UploadedFileName = url + "\\" + doc.TxtFileName;

                //        updateStatuses.Add(status);
                //    }
                //}

                //this._ApplicationDbContext.BulkUpdate(updateStatuses.ToList());
                if (OldId is null)
                    SelfAssessmentMigrateDocuments();
                else
                    SelfAssessmentMigrateDocuments(OldId);
            }

            if (OldId is null)
                UpdateSelfAssessmentStatus();
            else
                UpdateSelfAssessmentStatus(OldId);
        }

        public void UpdateSelfAssessmentStatus(int? OldId = null)
        {
            LogStrartInformation("UpdateSelfAssessmentStatus");

            var selfAssessment = new List<SelfAssessmentReport>();
            var selfAssessmentStatus = new List<SelfAssessmentReportStatus>();

            if (OldId is null)
            {
                selfAssessment = this._ApplicationDbContext.SelfAssessmentReports.ToList();
                selfAssessmentStatus = this._ApplicationDbContext.SelfAssessmentReportStatuses.OrderByDescending(x => x.CreationDate).ToList();
            }
            else
            {
                selfAssessment = (from sar in this._ApplicationDbContext.SelfAssessmentReports
                                  join cp in this._ApplicationDbContext.CandidateProviders on sar.IdCandidateProvider equals cp.IdCandidate_Provider
                                  where cp.OldId == OldId
                                  select sar).ToList();

                selfAssessmentStatus = (from sars in this._ApplicationDbContext.SelfAssessmentReportStatuses
                                        join sar in this._ApplicationDbContext.SelfAssessmentReports on sars.IdSelfAssessmentReport equals sar.IdSelfAssessmentReport
                                        join cp in this._ApplicationDbContext.CandidateProviders on sar.IdCandidateProvider equals cp.IdCandidate_Provider
                                        where cp.OldId == OldId
                                        select sars).OrderByDescending(x => x.CreationDate).ToList();
            }

            var selfAssessmentToUpdate = new List<SelfAssessmentReport>();

            Parallel.ForEach(selfAssessment, assessment =>
            {
                var status = selfAssessmentStatus.Where(x => x.IdSelfAssessmentReport == assessment.IdSelfAssessmentReport).FirstOrDefault();

                if (status is not null)
                {
                    assessment.IdStatus = status.IdStatus;

                    selfAssessmentToUpdate.Add(assessment);
                }
            });

            this._ApplicationDbContext.BulkUpdate(selfAssessmentToUpdate);
            LogEndInformation("UpdateSelfAssessmentStatus");
        }
        #endregion

        #region Documents Only
        public void CandidateCurriculumModificationMigrateDomuments(int? OldId = null)
        {
            LogStrartInformation("CandidateCurriculumModificationMigrateDomuments");
            ConcurrentBag<CandidateCurriculumModification> curricsForUpdate = new ConcurrentBag<CandidateCurriculumModification>();

            var tb_candidate_provider_specialities_curriculum = new List<TbCandidateProviderSpecialitiesCurriculum>();

            var curriculumns = new List<CandidateCurriculumModification>();
            if (OldId is null)
            {
                curriculumns = this._ApplicationDbContext.CandidateCurriculumModification.Where(x => string.IsNullOrEmpty(x.UploadedFileName)).ToList();
            }
            else
            {
                curriculumns = this._ApplicationDbContext.CandidateCurriculumModification
                    .Include(x => x.CandidateProviderSpeciality.CandidateProvider)
                    .Where(x => x.CandidateProviderSpeciality.CandidateProvider.OldId == OldId && string.IsNullOrEmpty(x.UploadedFileName)).ToList();
            }

            var tb_provider_specialities_curriculum = new List<TbProviderSpecialitiesCurriculum>();

            if (OldId is null)
            {
                tb_provider_specialities_curriculum = this._jessieContextContext.TbProviderSpecialitiesCurricula.Where(x => x.IntProviderSpecialityId != null).ToList();
                tb_candidate_provider_specialities_curriculum = this._jessieContextContext.TbCandidateProviderSpecialitiesCurricula.Where(x => x.IntCandidateProviderSpecialityId != null).ToList();
            }
            else
            {
                var specialityProvider = this._jessieContextContext.TbProviderSpecialities.Where(x => x.IntProviderId == OldId).ToList();
                var specialityCandidateProvider = this._jessieContextContext.TbCandidateProvidersSpecialities.Where(x => x.IntProviderId == OldId).ToList();
                tb_provider_specialities_curriculum = this._jessieContextContext.TbProviderSpecialitiesCurricula.ToList().Where(x => specialityProvider.Any(z => z.Id == x.IntProviderSpecialityId)).ToList();
                tb_candidate_provider_specialities_curriculum = this._jessieContextContext.TbCandidateProviderSpecialitiesCurricula.ToList().Where(x => specialityCandidateProvider.Any(z => z.Id == x.IntCandidateProviderSpecialityId)).ToList();

            }


            int Skip = 0;
            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(curriculumns.Count() / Take) + 1;


            for (int i = 0; i < group; i++)
            {
                curricsForUpdate.Clear();

                this.logger.LogInformation($"CandidateCurriculumModificationMigrateDomuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->CandidateCurriculumModification.OldId:{string.Join(",", curriculumns.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");



                Parallel.ForEach(curriculumns.Skip(Skip).Take(Take), curr =>
                {

                    try
                    {
                        var url = $"\\UploadedFiles\\CandidateCurriculumModification\\{curr.IdCandidateCurriculumModification}\\";

                        var doc = tb_provider_specialities_curriculum.Where(x => x.Id == curr.OldId).FirstOrDefault();
                        if (doc is not null)
                        {
                            if (doc.OidFile is not null)
                            {
                                url += $"{url}{SaveDocument((int)doc.OidFile, url)}";

                                curr.UploadedFileName = url;
                                curr.CandidateProviderSpeciality = null;
                                curricsForUpdate.Add(curr);
                            }
                        }
                        else
                        {
                            var url2 = $"\\UploadedFiles\\CandidateCurriculumModification\\{curr.IdCandidateCurriculumModification}\\";

                            var doc2 = tb_candidate_provider_specialities_curriculum.Where(x => x.Id == curr.OldId).First();

                            if (doc2.OidFile is not null)
                            {
                                var name = $"{url2}{SaveDocument((int)doc2.OidFile, url2)}";

                                curr.UploadedFileName = url2+name;
                                curricsForUpdate.Add(curr);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод CandidateCurriculumModificationMigrateDomuments(Първи foreach). Запис с Id = " + curr.IdCandidateCurriculumModification);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }


                    // Console.WriteLine($"{number}");

                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(curricsForUpdate.ToList());
            }

            LogEndInformation("CandidateCurriculumModificationMigrateDomuments");

        }

        public void CandidateProviderDocumentMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("CandidateProviderDocumentMigrateDocuments");
            ConcurrentBag<CandidateProviderDocument> providersForUpdate = new ConcurrentBag<CandidateProviderDocument>();

            var providerDocuments = new List<CandidateProviderDocument>();

            if (OldId is null)
            {
                providerDocuments = this._ApplicationDbContext.CandidateProviderDocuments
                    .Where(x => string.IsNullOrEmpty(x.UploadedFileName)).ToList();
            }
            else
            {
                providerDocuments = (from pd in this._ApplicationDbContext.CandidateProviderDocuments
                                     join cp in this._ApplicationDbContext.CandidateProviders on pd.IdCandidateProvider equals cp.IdCandidate_Provider
                                     where cp.OldId == OldId
                                     select pd).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(providerDocuments.Count() / Take) + 1;

            var tb_providers_documents_management = new List<TbProvidersDocumentsManagement>();
            var tb_candidate_providers_documents_management = new List<TbCandidateProvidersDocumentsManagement>();
            if (OldId is null)
            {

                tb_providers_documents_management = this._jessieContextContext.TbProvidersDocumentsManagements.ToList();
                tb_candidate_providers_documents_management = this._jessieContextContext.TbCandidateProvidersDocumentsManagements.ToList();
            }
            else
            {
                tb_providers_documents_management = this._jessieContextContext.TbProvidersDocumentsManagements.Where(x => x.IntProviderId == OldId).ToList();
                tb_candidate_providers_documents_management = this._jessieContextContext.TbCandidateProvidersDocumentsManagements.Where(x => x.IntProviderId == OldId).ToList();

            }


            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"CandidateProviderDocumentMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->providerDocuments.OldId:{string.Join(",", providerDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                providersForUpdate.Clear();

                Parallel.ForEach(providerDocuments.Skip(Skip).Take(Take), doc =>
                {

                    try
                    {
                        var url = $"\\UploadedFiles\\CandidateProviderDocument\\{doc.IdCandidateProviderDocument}\\";

                        var oid = string.Empty;

                        var docFromNapoo = tb_providers_documents_management.Where(x => x.Id == doc.OldId).FirstOrDefault();

                        if (docFromNapoo is not null)
                        {
                            oid = docFromNapoo.TxtDocumentsManagementFile.ToString();
                        }
                        else
                        {
                            var docFromCandidateNapoo = tb_candidate_providers_documents_management.Where(x => x.Id == doc.OldId).First();

                            oid = docFromCandidateNapoo.TxtDocumentsManagementFile.ToString();
                        }

                        if (!string.IsNullOrEmpty(oid))
                        {
                            var name = SaveDocument(int.Parse(oid), url);

                            doc.UploadedFileName = $"{url}{name}";
                            providersForUpdate.Add(doc);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ImportProviderDocuments(Първи foreach). Запис с Id = " + doc.IdCandidateProviderDocument);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(providersForUpdate.ToList());

            }

            LogEndInformation("CandidateProviderDocumentMigrateDocuments");
        }

        public void CandidateProviderTrainerDocumentMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("CandidateProviderTrainerDocumentMigrateDocuments");
            ConcurrentBag<CandidateProviderTrainerDocument> providersTrainerDocumentForUpdate = new ConcurrentBag<CandidateProviderTrainerDocument>();

            var trainerDocuments = new List<CandidateProviderTrainerDocument>();

            if (OldId is null)
            {
                trainerDocuments = this._ApplicationDbContext.CandidateProviderTrainerDocuments
                    .Where(x => !string.IsNullOrEmpty(x.UploadedFileName) && x.UploadedFileName.Equals("Отвори документа")).ToList();
            }
            else
            {
                trainerDocuments = (from pd in this._ApplicationDbContext.CandidateProviderTrainerDocuments
                                    join t in this._ApplicationDbContext.CandidateProviderTrainers on pd.IdCandidateProviderTrainer equals t.IdCandidateProviderTrainer
                                    join cp in this._ApplicationDbContext.CandidateProviders on t.IdCandidate_Provider equals cp.IdCandidate_Provider
                                    where cp.OldId == OldId && !string.IsNullOrEmpty(pd.UploadedFileName) && pd.UploadedFileName.Equals("Отвори документа")
                                    select pd).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(trainerDocuments.Count() / Take) + 1;

            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"CandidateProviderTrainerDocumentMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->trainerDocuments.OldId:{string.Join(",", trainerDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                providersTrainerDocumentForUpdate.Clear();

                Parallel.ForEach(trainerDocuments.Skip(Skip).Take(Take), doc =>
                {

                    try
                    {
                        var url = $"\\UploadedFiles\\CandidateProviderTrainerDocument\\{doc.IdCandidateProviderTrainerDocument}\\";
                        if (!string.IsNullOrEmpty(doc.MigrationNote))
                        {
                            var name = SaveDocument(Int32.Parse(doc.MigrationNote), url);
                            doc.UploadedFileName = url.Substring(1)+name;

                            providersTrainerDocumentForUpdate.Add(doc);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод CandidateProviderTrainerDocumentMigrateDocuments(Първи foreach). Запис с Id = " + doc.IdCandidateProviderTrainerDocument);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(providersTrainerDocumentForUpdate.ToList());

            }

            LogEndInformation("CandidateProviderTrainerDocumentMigrateDocuments");
        }

        public void CandidateProviderProcedureDocumentMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("CandidateProviderProcedureDocumentMigrateDocuments");
            ConcurrentBag<ProcedureDocument> procedureDocumentForUpdate = new ConcurrentBag<ProcedureDocument>();

            var procedureDocuments = new List<ProcedureDocument>();

            if (OldId is null)
            {
                procedureDocuments = this._ApplicationDbContext.ProcedureDocuments
                    .Where(x => !string.IsNullOrEmpty(x.UploadedFileName) && x.UploadedFileName.Equals("Отвори документа") && !string.IsNullOrEmpty(x.MigrationNote)).ToList();
            }
            else
            {
                procedureDocuments = (from pd in this._ApplicationDbContext.ProcedureDocuments
                                      join sp in this._ApplicationDbContext.StartedProcedures on pd.IdStartedProcedure equals sp.IdStartedProcedure
                                      join cp in this._ApplicationDbContext.CandidateProviders on sp.IdCandidate_Provider equals cp.IdCandidate_Provider
                                      where cp.OldId == OldId && !string.IsNullOrEmpty(pd.UploadedFileName) && pd.UploadedFileName.Equals("Отвори документа") && !string.IsNullOrEmpty(pd.MigrationNote)
                                      select pd).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(procedureDocuments.Count() / Take) + 1;

            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"CandidateProviderProcedureDocumentMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->procedureDocuments.OldId:{string.Join(",", procedureDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                procedureDocumentForUpdate.Clear();

                Parallel.ForEach(procedureDocuments.Skip(Skip).Take(Take), doc =>
                {

                    try
                    {
                        var url = $"\\UploadedFiles\\ProviderProcedureDocument\\{doc.IdProcedureDocument}\\";

                        if (doc.MigrationNote is not null)
                        {
                            url += $"{SaveDocument(Int32.Parse(doc.MigrationNote), url)}";

                            doc.UploadedFileName = url.Substring(1);

                            procedureDocumentForUpdate.Add(doc);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод CandidateProviderProcedureDocumentMigrateDocuments(Първи foreach). Запис с Id = " + doc.IdProcedureDocument);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(procedureDocumentForUpdate.ToList());

            }

            LogEndInformation("CandidateProviderProcedureDocumentMigrateDocuments");
        }

        public void RequestDocumentManagementMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("RequestDocumentManagementMigrateDocuments");
            ConcurrentBag<RequestDocumentManagementUploadedFile> requestDocumentManagementUploadedFileForUpdate = new ConcurrentBag<RequestDocumentManagementUploadedFile>();

            var requestDocumentManagements = new List<RequestDocumentManagementUploadedFile>();

            if (OldId is null)
            {
                requestDocumentManagements = this._ApplicationDbContext.RequestDocumentManagementUploadedFiles
                    .Where(x => x.UploadedFileName == "Отвори документа").ToList();
            }
            else
            {
                requestDocumentManagements = (from rdmuf in this._ApplicationDbContext.RequestDocumentManagementUploadedFiles
                                              join rdm in this._ApplicationDbContext.RequestDocumentManagements on rdmuf.IdRequestDocumentManagement equals rdm.IdRequestDocumentManagement
                                              join cp in this._ApplicationDbContext.CandidateProviders on rdm.IdCandidateProvider equals cp.IdCandidate_Provider
                                              where cp.OldId == OldId && rdmuf.UploadedFileName == "Отвори документа"
                                              select rdmuf).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(requestDocumentManagements.Count() / Take) + 1;

            var tb_provider_uploaded_docs = this._jessieContextContext.TbProviderUploadedDocs;

            this.logger.LogInformation($"-->requestDocumentManagements.OldId:{string.Join(",", requestDocumentManagements.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

            foreach (var currentManagement in requestDocumentManagements)
            {
                try
                {
                    var url = $"\\UploadedFiles\\RequestDocumentManagementUploadedFile\\{currentManagement.IdRequestDocumentManagementUploadedFile}";
                    var doc = tb_provider_uploaded_docs.Where(x => x.Id == int.Parse(currentManagement.MigrationNote)).First();

                    SaveBinDocument(url, doc.BinFile, doc.TxtFileName);
                    currentManagement.UploadedFileName = url.Substring(1)+"\\"+doc.TxtFileName;

                    currentManagement.Description = doc.TxtFileName;

                    currentManagement.CreationDate = doc.DtDocUploadDate.Value;

                    requestDocumentManagementUploadedFileForUpdate.Add(currentManagement);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод RequestDocumentManagementMigrateDocuments(Първи foreach). Запис с Id = " + currentManagement.IdRequestDocumentManagementUploadedFile);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }


            }

            this._ApplicationDbContext.BulkUpdate(requestDocumentManagementUploadedFileForUpdate.ToList());

            LogEndInformation("RequestDocumentManagementMigrateDocuments");
        }

        public void RequestCandidateProviderDocumentMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("RequestDocumentManagementMigrateDocuments");
            ConcurrentBag<RequestCandidateProviderDocument> requestCandidateProviderDocumentForUpdate = new ConcurrentBag<RequestCandidateProviderDocument>();

            var requestCandidateProviderDocument = new List<RequestCandidateProviderDocument>();

            if (OldId is null)
            {
                requestCandidateProviderDocument = this._ApplicationDbContext.RequestCandidateProviderDocuments
                    .Where(x => x.UploadedFileName == "Отвори документа").ToList();
            }
            else
            {
                requestCandidateProviderDocument = (from rcpd in this._ApplicationDbContext.RequestCandidateProviderDocuments
                                                    join cp in this._ApplicationDbContext.CandidateProviders on rcpd.IdCandidateProvider equals cp.IdCandidate_Provider
                                                    where cp.OldId == OldId && rcpd.UploadedFileName == "Отвори документа"
                                                    select rcpd).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(requestCandidateProviderDocument.Count() / Take) + 1;

            var tb_provider_uploaded_docs = this._jessieContextContext.TbProviderUploadedDocs;

            foreach (var currentDocument in requestCandidateProviderDocument)
            {
                try
                {
                    var url = $"\\UploadedFiles\\RequestClientCourseDocument\\{currentDocument.IdRequestCandidateProviderDocument}";
                    var doc = tb_provider_uploaded_docs.Where(x => x.Id == currentDocument.OldId).First();

                    SaveBinDocument(url, doc.BinFile, doc.TxtFileName);
                    currentDocument.UploadedFileName = url.Substring(1) +"\\"+doc.TxtFileName;


                    requestCandidateProviderDocumentForUpdate.Add(currentDocument);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод RequestDocumentManagementMigrateDocuments(Първи foreach). Запис с Id = " + currentDocument.IdRequestCandidateProviderDocument);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }


            }

            this._ApplicationDbContext.BulkUpdate(requestCandidateProviderDocumentForUpdate.ToList());

            LogEndInformation("RequestDocumentManagementMigrateDocuments");
        }
        public void RequestReportMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("RequestReportMigrateDocuments");
            ConcurrentBag<ReportUploadedDoc> requestReportUploadedFileForUpdate = new ConcurrentBag<ReportUploadedDoc>();

            var requestReportDocuments = new List<ReportUploadedDoc>();

            if (OldId is null)
            {
                requestReportDocuments = this._ApplicationDbContext.ReportUploadedDocs
                    .Where(x => string.IsNullOrEmpty(x.UploadedFileName)).ToList();
            }
            else
            {
                requestReportDocuments = (from rud in this._ApplicationDbContext.ReportUploadedDocs
                                          join rr in this._ApplicationDbContext.RequestReports on rud.IdRequestReport equals rr.IdRequestReport
                                          join cp in this._ApplicationDbContext.CandidateProviders on rr.IdCandidateProvider equals cp.IdCandidate_Provider
                                          where cp.OldId == OldId && string.IsNullOrEmpty(rud.UploadedFileName)
                                          select rud).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(requestReportDocuments.Count() / Take) + 1;

            var tb_provider_uploaded_docs = this._jessieContextContext.TbProviderUploadedDocs;


            foreach (var doc in requestReportDocuments)
            {
                try
                {
                    var url = $"\\UploadedFiles\\ReportUploadedDoc\\{doc.IdReportUploadedDoc}";
                    var uploadedDoc = tb_provider_uploaded_docs.Where(x => x.Id == doc.OldId).First();

                    SaveBinDocument(url, uploadedDoc.BinFile, uploadedDoc.TxtFileName);
                    doc.UploadedFileName = url.Substring(1) +"\\"+uploadedDoc.TxtFileName;

                    requestReportUploadedFileForUpdate.Add(doc);
                }
                catch (Exception ex)
                {
                    this.logger.LogInformation("Гръмна метод RequestReportMigrateDocuments(Първи foreach). Запис с Id = " + doc.IdRequestReport);
                    this.logger.LogError(ex.Message);
                    this.logger.LogError(ex.InnerException?.Message);
                    this.logger.LogError(ex.StackTrace);
                }
            }

            this._ApplicationDbContext.BulkUpdate(requestReportUploadedFileForUpdate.ToList());

            LogEndInformation("RequestReportMigrateDocuments");
        }

        public void AnnualInfoMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("AnnualInfoMigrateDocuments");
            ConcurrentBag<AnnualInfoStatus> annualInfoStatusForUpdate = new ConcurrentBag<AnnualInfoStatus>();

            var annualInfoStatusDocuments = new List<AnnualInfoStatus>();

            var annualInfoCIPO = new List<TbProviderUploadedDoc>();

            var providers = new List<CandidateProvider>();

            if (OldId is null)
            {
                providers = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive).ToList();
                //annualInfoCIPO = this._jessieContextContext.TbProviderUploadedDocs
                //                    .Where(x => x.IntUploadDocTypeId == 14).ToList();

                //annualInfoStatusDocuments = this._ApplicationDbContext.AnnualInfoStatuses
                //    .Where(x => string.IsNullOrEmpty(x.UploadedFileName) && !string.IsNullOrEmpty(x.MigrationNote)).ToList();
            }
            else
            {

                providers = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive && x.OldId == OldId).ToList();
            }

            foreach (var provider in providers)
            {

                annualInfoStatusForUpdate.Clear();

                annualInfoCIPO = this._jessieContextContext.TbProviderUploadedDocs
                               .Where(x => x.IntUploadDocTypeId == 14 && x.IntProviderId == provider.OldId).ToList();

                if (!annualInfoCIPO.Any()) continue;

                annualInfoStatusDocuments = (from rud in this._ApplicationDbContext.AnnualInfoStatuses
                                             join ai in this._ApplicationDbContext.AnnualInfos on rud.IdAnnualInfo equals ai.IdAnnualInfo
                                             join cp in this._ApplicationDbContext.CandidateProviders on ai.IdCandidateProvider equals cp.IdCandidate_Provider
                                             where cp.OldId == provider.OldId && string.IsNullOrEmpty(rud.UploadedFileName)
                                             select rud).ToList();


                int Skip = 0;

                int Take = Int32.Parse(takeSetting.SettingValue);

                int group = (int)(annualInfoStatusDocuments.Count() / Take) + 1;

                var tb_provider_uploaded_docs = this._jessieContextContext.TbProviderUploadedDocs;

                foreach (var status in annualInfoStatusDocuments)
                {

                    try
                    {
                        var doc = annualInfoCIPO.Where(x => x.Id == status.OldId).First();

                        if (doc.BinFile is not null)
                        {
                            var url = $"\\UploadedFiles\\AnnualInfoStatus\\{status.IdAnnualInfoStatus}";

                            SaveBinDocument(url, doc.BinFile, doc.TxtFileName);
                            status.UploadedFileName = url.Substring(1) + "\\" + doc.TxtFileName;

                            annualInfoStatusForUpdate.Add(status);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод AnnualInfoMigrateDocuments(Първи foreach). Запис с Id = " + status.IdAnnualInfoStatus);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }

                }
                _ApplicationDbContext.BulkUpdate(annualInfoStatusForUpdate.ToList());
            }

            LogEndInformation("AnnualInfoMigrateDocuments");
        }

        public void SelfAssessmentMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("SelfAssessmentMigrateDocuments");
            ConcurrentBag<SelfAssessmentReportStatus> selfAssessmentStatusForUpdate = new ConcurrentBag<SelfAssessmentReportStatus>();

            var selfAssessmentStatusDocuments = new List<SelfAssessmentReportStatus>();
            var selfAssessmentReports = new List<TbProviderUploadedDoc>();

            var providers = new List<CandidateProvider>();

            if (OldId is null)
            {
                providers = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive).ToList();
                //selfAssessmentReports = this._jessieContextContext.TbProviderUploadedDocs.Where(x => x.IntUploadDocTypeId == 3 || x.IntUploadDocTypeId == 13).ToList();

                //selfAssessmentStatusDocuments = this._ApplicationDbContext.SelfAssessmentReportStatuses
                //    .Where(x => string.IsNullOrEmpty(x.UploadedFileName) && !string.IsNullOrEmpty(x.MigrationNote)).ToList();
            }
            else
            {
                providers = this._ApplicationDbContext.CandidateProviders.Where(x => x.IsActive && x.OldId == OldId).ToList();

                //selfAssessmentReports = this._jessieContextContext.TbProviderUploadedDocs.Where(x => x.IntProviderId == OldId && (x.IntUploadDocTypeId == 3 || x.IntUploadDocTypeId == 13)).ToList();

                //selfAssessmentStatusDocuments = (from rud in this._ApplicationDbContext.SelfAssessmentReportStatuses
                //                                 join ai in this._ApplicationDbContext.SelfAssessmentReports on rud.IdSelfAssessmentReport equals ai.IdSelfAssessmentReport
                //                                 join cp in this._ApplicationDbContext.CandidateProviders on ai.IdCandidateProvider equals cp.IdCandidate_Provider
                //                                 where cp.OldId == OldId && string.IsNullOrEmpty(rud.UploadedFileName)
                //                                 select rud).ToList();
            }
            foreach (var provider in providers)
            {

                selfAssessmentReports = this._jessieContextContext.TbProviderUploadedDocs.Where(x => x.IntProviderId == provider.OldId && (x.IntUploadDocTypeId == 3 || x.IntUploadDocTypeId == 13)).ToList();

                selfAssessmentStatusDocuments = (from rud in this._ApplicationDbContext.SelfAssessmentReportStatuses
                                                 join ai in this._ApplicationDbContext.SelfAssessmentReports on rud.IdSelfAssessmentReport equals ai.IdSelfAssessmentReport
                                                 join cp in this._ApplicationDbContext.CandidateProviders on ai.IdCandidateProvider equals cp.IdCandidate_Provider
                                                 where cp.OldId == provider.OldId && string.IsNullOrEmpty(rud.UploadedFileName)
                                                 select rud).ToList();
                int Skip = 0;

                int Take = Int32.Parse(takeSetting.SettingValue);

                int group = (int)(selfAssessmentStatusDocuments.Count() / Take) + 1;

                for (int i = 0; i < group; i++)
                {
                    this.logger.LogInformation($"SelfAssessmentMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                    this.logger.LogInformation($"-->selfAssessmentStatusDocuments.OldId:{string.Join(",", selfAssessmentStatusDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                    selfAssessmentStatusForUpdate.Clear();

                    Parallel.ForEach(selfAssessmentStatusDocuments.Skip(Skip).Take(Take), status =>
                    {

                        try
                        {
                            var doc = selfAssessmentReports.Where(x => x.Id == status.OldId).First();

                            if (doc.BinFile is not null)
                            {
                                var url = $"\\UploadedFiles\\SelfAssessmentReportStatus\\{status.IdSelfAssessmentReport}";

                                SaveBinDocument(url, doc.BinFile, doc.TxtFileName);
                                status.UploadedFileName = url.Substring(1) + "\\"+doc.TxtFileName;

                                selfAssessmentStatusForUpdate.Add(status);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.logger.LogInformation("Гръмна метод SelfAssessmentMigrateDocuments(Първи foreach). Запис с Id = " + status.IdSelfAssessmentReportStatus);
                            this.logger.LogError(ex.Message);
                            this.logger.LogError(ex.InnerException?.Message);
                            this.logger.LogError(ex.StackTrace);
                        }
                    });
                    Skip = Skip + Take;

                    _ApplicationDbContext.BulkUpdate(selfAssessmentStatusForUpdate.ToList());

                }
            }

            LogEndInformation("SelfAssessmentMigrateDocuments");
        }

        public void ProviderRequestDocumentMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("ProviderRequestDocumentMigrateDocuments");
            ConcurrentBag<ProviderRequestDocument> providerRequestDocumentsForUpdate = new ConcurrentBag<ProviderRequestDocument>();

            var providerRequestDocuments = new List<ProviderRequestDocument>();

            if (OldId is null)
            {
                providerRequestDocuments = this._ApplicationDbContext.ProviderRequestDocuments
                    .Where(x => x.UploadedFileName.Equals("Отвори документа")).ToList();
            }
            else
            {
                providerRequestDocuments = (from prd in this._ApplicationDbContext.ProviderRequestDocuments
                                            join cp in this._ApplicationDbContext.CandidateProviders on prd.IdCandidateProvider equals cp.IdCandidate_Provider
                                            where cp.OldId == OldId && prd.UploadedFileName.Equals("Отвори документа")
                                            select prd).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(providerRequestDocuments.Count() / Take) + 1;

            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"ProviderRequestDocumentMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->providerRequestDocuments.OldId:{string.Join(",", providerRequestDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                providerRequestDocumentsForUpdate.Clear();

                Parallel.ForEach(providerRequestDocuments.Skip(Skip).Take(Take), doc =>
                {

                    try
                    {
                        var url = $"\\UploadedFiles\\ProviderRequestDocument\\{doc.IdProviderRequestDocument}\\";
                        if (doc.MigrationNote != null)
                        {
                            var name = SaveDocument(Int32.Parse(doc.MigrationNote), url);
                            doc.UploadedFileName = url+name;
                        }

                        providerRequestDocumentsForUpdate.Add(doc);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ProviderRequestDocumentMigrateDocuments(Първи foreach). Запис с Id = " + doc.IdProviderRequestDocument);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(providerRequestDocumentsForUpdate.ToList());

            }

            LogEndInformation("ProviderRequestDocumentMigrateDocuments");
        }

        public void CourseProtocolMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("CourseProtocolMigrateDocuments");
            ConcurrentBag<CourseProtocol> courseProtocolForUpdate = new ConcurrentBag<CourseProtocol>();

            var courseProtocolDocuments = new List<CourseProtocol>();

            if (OldId is null)
            {
                courseProtocolDocuments = this._ApplicationDbContext.CourseProtocols
                    .Where(x => x.UploadedFileName.Equals("Отвори документа")).ToList();
            }
            else
            {
                courseProtocolDocuments = (from vp in this._ApplicationDbContext.CourseProtocols
                                           join vc in this._ApplicationDbContext.Courses on vp.IdCourse equals vc.IdCourse
                                           join cp in this._ApplicationDbContext.CandidateProviders on vc.IdCandidateProvider equals cp.IdCandidate_Provider
                                           where cp.OldId == OldId && vp.UploadedFileName.Equals("Отвори документа")
                                           select vp).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(courseProtocolDocuments.Count() / Take) + 1;

            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"CourseProtocolMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->courseProtocolDocuments.OldId:{string.Join(",", courseProtocolDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                courseProtocolForUpdate.Clear();

                Parallel.ForEach(courseProtocolDocuments.Skip(Skip).Take(Take), protocol =>
                {

                    try
                    {
                        var url = $"\\UploadedFiles\\CourseProtocol\\{protocol.IdCourseProtocol}\\";
                        if (protocol.MigrationNote != null)
                        {
                            var name = SaveDocument(Int32.Parse(protocol.MigrationNote), url);
                            protocol.UploadedFileName = url.Substring(1)+name;

                            courseProtocolForUpdate.Add(protocol);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод CourseProtocolMigrateDocuments(Първи foreach). Запис с Id = " + protocol.IdCourseProtocol);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(courseProtocolForUpdate.ToList());

            }

            LogEndInformation("CourseProtocolMigrateDocuments");
        }

        public void ValidationProtocolMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("ValidationProtocolMigrateDocuments");
            ConcurrentBag<ValidationProtocol> validationProtocolForUpdate = new ConcurrentBag<ValidationProtocol>();

            var validationProtocolDocuments = new List<ValidationProtocol>();

            if (OldId is null)
            {
                validationProtocolDocuments = this._ApplicationDbContext.ValidationProtocols
                    .Where(x => x.UploadedFileName.Equals("Отвори документа")).ToList();
            }
            else
            {
                validationProtocolDocuments = (from vp in this._ApplicationDbContext.ValidationProtocols
                                               join vc in this._ApplicationDbContext.ValidationClients on vp.IdValidationClient equals vc.IdValidationClient
                                               join cp in this._ApplicationDbContext.CandidateProviders on vc.IdCandidateProvider equals cp.IdCandidate_Provider
                                               where cp.OldId == OldId && vp.UploadedFileName.Equals("Отвори документа")
                                               select vp).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(validationProtocolDocuments.Count() / Take) + 1;

            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"ValidationProtocolMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->validationProtocolDocuments.OldId:{string.Join(",", validationProtocolDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                validationProtocolForUpdate.Clear();

                Parallel.ForEach(validationProtocolDocuments.Skip(Skip).Take(Take), protocol =>
                {
                    try
                    {
                        var url = $"\\UploadedFiles\\ValidationProtocol\\{protocol.IdValidationProtocol}\\";
                        if (protocol.MigrationNote != null)
                        {
                            var name = SaveDocument(Int32.Parse(protocol.MigrationNote), url);
                            protocol.UploadedFileName = url.Substring(1)+name;

                            validationProtocolForUpdate.Add(protocol);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ValidationProtocolMigrateDocuments(Първи foreach). Запис с Id = " + protocol.IdValidationProtocol);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(validationProtocolForUpdate.ToList());

            }

            LogEndInformation("ValidationProtocolMigrateDocuments");
        }

        public void ValidationRequiredDocumentsMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("ValidationRequiredDocumentsMigrateDocuments");
            ConcurrentBag<ValidationClientRequiredDocument> validationRequredClientDocumentsForUpdate = new ConcurrentBag<ValidationClientRequiredDocument>();

            var validationRequiredDocuments = new List<ValidationClientRequiredDocument>();

            if (OldId is null)
            {
                validationRequiredDocuments = this._ApplicationDbContext.ValidationClientRequiredDocuments
                    .Where(x => x.UploadedFileName.Equals("Отвори документа")).ToList();
            }
            else
            {
                validationRequiredDocuments = (from vd in this._ApplicationDbContext.ValidationClientRequiredDocuments
                                               join vc in this._ApplicationDbContext.ValidationClients on vd.IdValidationClient equals vc.IdValidationClient
                                               join cp in this._ApplicationDbContext.CandidateProviders on vc.IdCandidateProvider equals cp.IdCandidate_Provider
                                               where cp.OldId == OldId && vd.UploadedFileName.Equals("Отвори документа")
                                               select vd).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(validationRequiredDocuments.Count() / Take) + 1;

            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"ValidationRequiredDocumentsMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->validationRequiredDocuments.OldId:{string.Join(",", validationRequiredDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                validationRequredClientDocumentsForUpdate.Clear();

                Parallel.ForEach(validationRequiredDocuments.Skip(Skip).Take(Take), doc =>
                {
                    try
                    {
                        var url = $"\\UploadedFiles\\ValidationEducation\\{doc.IdValidationClientRequiredDocument}\\";
                        if (doc.MigrationNote != null)
                        {
                            var name = SaveDocument(Int32.Parse(doc.MigrationNote), url);
                            doc.UploadedFileName = url.Substring(1)+name;

                            validationRequredClientDocumentsForUpdate.Add(doc);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ValidationRequiredDocumentsMigrateDocuments(Първи foreach). Запис с Id = " + doc.IdValidationClientRequiredDocument);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(validationRequredClientDocumentsForUpdate.ToList());

            }

            LogEndInformation("ValidationRequiredDocumentsMigrateDocuments");
        }

        public void ClientCourseRequiredDocumentsMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("ClientCourseRequiredDocumentsMigrateDocuments");
            ConcurrentBag<ClientRequiredDocument> clientRequiredDocumentForUpdate = new ConcurrentBag<ClientRequiredDocument>();

            var clientRequiredDocuments = new List<ClientRequiredDocument>();

            if (OldId is null)
            {
                clientRequiredDocuments = this._ApplicationDbContext.ClientRequiredDocuments
                    .Where(x => x.UploadedFileName.Equals("Отвори документа")).ToList();
            }
            else
            {
                clientRequiredDocuments = (from vd in this._ApplicationDbContext.ClientRequiredDocuments
                                           join vc in this._ApplicationDbContext.Courses on vd.IdCourse equals vc.IdCourse
                                           join cp in this._ApplicationDbContext.CandidateProviders on vc.IdCandidateProvider equals cp.IdCandidate_Provider
                                           where cp.OldId == OldId && vd.UploadedFileName.Equals("Отвори документа")
                                           select vd).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(clientRequiredDocuments.Count() / Take) + 1;

            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"ClientCourseRequiredDocumentsMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->clientRequiredDocuments.OldId:{string.Join(",", clientRequiredDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                clientRequiredDocumentForUpdate.Clear();

                Parallel.ForEach(clientRequiredDocuments.Skip(Skip).Take(Take), doc =>
                {
                    try
                    {
                        var url = $"\\UploadedFiles\\ClientCourseEducation\\{doc.IdClientRequiredDocument}\\";
                        if (doc.MigrationNote != null)
                        {
                            var name = SaveDocument(Int32.Parse(doc.MigrationNote), url);
                            doc.UploadedFileName = url.Substring(1)+name;

                            clientRequiredDocumentForUpdate.Add(doc);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ClientCourseRequiredDocumentsMigrateDocuments(Първи foreach). Запис с Id = " + doc.IdClientRequiredDocument);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(clientRequiredDocumentForUpdate.ToList());

            }

            LogEndInformation("ClientCourseRequiredDocumentsMigrateDocuments");
        }

        public void ClientCourseDocumentsMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("ClientCourseDocumentsMigrateDocuments");
            ConcurrentBag<CourseDocumentUploadedFile> clientCourseDocumentForUpdate = new ConcurrentBag<CourseDocumentUploadedFile>();

            var clientCourseDocuments = new List<CourseDocumentUploadedFile>();
            var tb_clients_courses_documents = new List<TbClientsCoursesDocument>();

            if (OldId is null)
            {
                clientCourseDocuments = this._ApplicationDbContext.CourseDocumentUploadedFiles
                    .Where(x => x.UploadedFileName == "Отвори документа").ToList();

                tb_clients_courses_documents = _jessieContextContext.TbClientsCoursesDocuments.ToList();

            }
            else
            {
                clientCourseDocuments = (from f in this._ApplicationDbContext.CourseDocumentUploadedFiles
                                         join cd in this._ApplicationDbContext.ClientCourseDocuments on f.IdClientCourseDocument equals cd.IdClientCourseDocument
                                         join cc in this._ApplicationDbContext.ClientCourses on cd.IdClientCourse equals cc.IdClientCourse
                                         join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                                         join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                                         where cp.OldId == OldId && f.UploadedFileName == "Отвори документа"
                                         select f).ToList();

                tb_clients_courses_documents = (from tccd in this._jessieContextContext.TbClientsCoursesDocuments
                                                join tcc in this._jessieContextContext.RefClientsCourses on tccd.IntClientsCoursesId equals tcc.Id
                                                join tc in this._jessieContextContext.TbClients on tcc.IntClientId equals tc.Id
                                                where tc.IntProviderId == OldId
                                                select tccd).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(clientCourseDocuments.Count() / Take) + 1;

            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"ClientCourseDocumentsMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->clientCourseDocuments.OldId:{string.Join(",", clientCourseDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                clientCourseDocumentForUpdate.Clear();

                Parallel.ForEach(clientCourseDocuments.Skip(Skip).Take(Take), file =>
                {
                    try
                    {
                        var doc = tb_clients_courses_documents.Where(x => x.Id == file.OldId).First();

                        if (file.MigrationNote.Equals("1"))
                        {

                            var url = $"\\UploadedFiles\\ClientCourseDocument\\{file.IdCourseDocumentUploadedFile}\\";

                            var name = SaveDocument((int)doc.Document1File.Value, url);

                            file.UploadedFileName = url.Substring(1)+name;

                            clientCourseDocumentForUpdate.Add(file);


                        }
                        else
                        {

                            var url = $"\\UploadedFiles\\ClientCourseDocument\\{file.IdCourseDocumentUploadedFile}\\";

                            var name = SaveDocument((int)doc.Document2File.Value, url);

                            file.UploadedFileName = url.Substring(1)+name;

                            clientCourseDocumentForUpdate.Add(file);

                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ClientCourseDocumentsMigrateDocuments(Първи foreach). Запис с Id = " + file.IdClientCourseDocument);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(clientCourseDocumentForUpdate.ToList());

            }

            LogEndInformation("ClientCourseDocumentsMigrateDocuments");
        }

        public void ValidationClientDocumentsMigrateDocuments(int? OldId = null)
        {
            LogStrartInformation("ValidationClientDocumentsMigrateDocuments");
            ConcurrentBag<ValidationDocumentUploadedFile> validationDocumentForUpdate = new ConcurrentBag<ValidationDocumentUploadedFile>();

            var validationUploadedFileDocuments = new List<ValidationDocumentUploadedFile>();
            var tb_clients_courses_documents = new List<TbClientsCoursesDocument>();

            if (OldId is null)
            {
                validationUploadedFileDocuments = this._ApplicationDbContext.ValidationDocumentUploadedFiles
                    .Where(x => string.IsNullOrEmpty(x.UploadedFileName)).ToList();

                tb_clients_courses_documents = _jessieContextContext.TbClientsCoursesDocuments.ToList();

            }
            else
            {
                validationUploadedFileDocuments = (from f in this._ApplicationDbContext.ValidationDocumentUploadedFiles
                                                   join cd in this._ApplicationDbContext.ValidationClientDocuments on f.IdValidationClientDocument equals cd.IdValidationClientDocument
                                                   join cc in this._ApplicationDbContext.ValidationClients on cd.IdValidationClient equals cc.IdValidationClient
                                                   join tc in this._ApplicationDbContext.Clients on cc.IdClient equals tc.IdClient
                                                   join cp in this._ApplicationDbContext.CandidateProviders on tc.IdCandidateProvider equals cp.IdCandidate_Provider
                                                   where cp.OldId == OldId && f.UploadedFileName == "Отвори документа"
                                                   select f).ToList();

                tb_clients_courses_documents = (from tccd in this._jessieContextContext.TbClientsCoursesDocuments
                                                join tcc in this._jessieContextContext.RefClientsCourses on tccd.IntClientsCoursesId equals tcc.Id
                                                join tc in this._jessieContextContext.TbClients on tcc.IntClientId equals tc.Id
                                                where tc.IntProviderId == OldId
                                                select tccd).ToList();
            }

            int Skip = 0;

            int Take = Int32.Parse(takeSetting.SettingValue);

            int group = (int)(validationUploadedFileDocuments.Count() / Take) + 1;

            for (int i = 0; i < group; i++)
            {
                this.logger.LogInformation($"ValidationClientDocumentsMigrateDocuments------->Skip: {Skip} Take: {Take}<-------------------------------");
                this.logger.LogInformation($"-->clientCourseDocuments.OldId:{string.Join(",", validationUploadedFileDocuments.Select(x => x.OldId).Skip(Skip).Take(Take))}<--");

                validationDocumentForUpdate.Clear();

                Parallel.ForEach(validationUploadedFileDocuments.Skip(Skip).Take(Take), file =>
                {
                    try
                    {
                        var doc = tb_clients_courses_documents.Where(x => x.Id == file.OldId).First();

                        if (file.MigrationNote.Equals("1"))
                        {

                            var url = $"\\UploadedFiles\\ValidationDocument\\{file.IdValidationDocumentUploadedFile}\\";

                            var name = SaveDocument((int)doc.Document1File.Value, url);
                            file.UploadedFileName = url.Substring(1)+name;

                            validationDocumentForUpdate.Add(file);

                        }
                        else
                        {

                            var url = $"\\UploadedFiles\\ValidationDocument\\{file.IdValidationDocumentUploadedFile}\\";

                            var name = SaveDocument((int)doc.Document2File.Value, url);
                            file.UploadedFileName = url.Substring(1);

                            validationDocumentForUpdate.Add(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogInformation("Гръмна метод ValidationClientDocumentsMigrateDocuments(Първи foreach). Запис с Id = " + file.IdValidationDocumentUploadedFile);
                        this.logger.LogError(ex.Message);
                        this.logger.LogError(ex.InnerException?.Message);
                        this.logger.LogError(ex.StackTrace);
                    }
                });
                Skip = Skip + Take;

                _ApplicationDbContext.BulkUpdate(validationDocumentForUpdate.ToList());

            }
            LogEndInformation("ValidationClientDocumentsMigrateDocuments");
        }

        #endregion

        #region Others
        private string SaveDocument(int oid, string path)
        {
            try
            {
                Thread.Sleep(50);
                System.Net.HttpWebRequest request = null;
                System.Net.HttpWebResponse response = null;
                request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create($"{linkSetting.SettingValue}{oid}");
                request.Timeout = 30000;
                request.UserAgent = ".NET Client";
                response = (System.Net.HttpWebResponse)request.GetResponse();
                var s = response.GetResponseStream();

                path = $"{this.recourceFolderSetting.SettingValue}{path}";

                bool exists = System.IO.Directory.Exists(path);

                if (!exists)
                    System.IO.Directory.CreateDirectory(path);
                string fullPath = "";

                var fileName = response.Headers["Content-Disposition"];

                fileName = response.Headers["Content-Disposition"].Replace("attachment; filename=", String.Empty).Replace("\"", String.Empty);

                var bytes = Encoding.UTF8.GetBytes(fileName);

                var encoded = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("ISO-8859-1"), bytes);

                string sConvertedString = Encoding.UTF8.GetString(encoded);

                fullPath = $"{path}\\{sConvertedString}";

                FileStream os = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[102400000];
                int c = 0;
                while ((c = s.Read(buff, 0, 10400)) > 0)
                {
                    os.Write(buff, 0, c);
                    os.Flush();
                }
                os.Close();
                s.Close();

                return sConvertedString;

            }
            catch (Exception ex)
            {
                this.logger.LogError("Файлът може би не съществува! OID:" + oid);
                this.logger.LogError(ex.Message);
                this.logger.LogError(ex.InnerException?.Message);
                this.logger.LogError(ex.StackTrace);
                return "";
            }

        }

        private string SaveDocument2(int oid, string path)
        {

            System.Net.HttpWebRequest request = null;
            System.Net.HttpWebResponse response = null;
            //request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create($"{linkSetting.SettingValue}({oid})");
            request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create($"{linkSetting.SettingValue}?oid={oid}");
            request.Timeout = 30000;
            request.UserAgent = ".NET Client";
            response = (System.Net.HttpWebResponse)request.GetResponse();
            var s = response.GetResponseStream();

            path = $"{this.recourceFolderSetting.SettingValue}{path}";

            bool exists = System.IO.Directory.Exists(path);

            if (!exists)
                System.IO.Directory.CreateDirectory(path);
            string fullPath = "";

            var fileName = response.Headers["Content-Disposition"];

            fileName = response.Headers["Content-Disposition"].Replace("attachment; filename=", String.Empty).Replace("\"", String.Empty);

            var bytes = Encoding.UTF8.GetBytes(fileName);

            var encoded = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("ISO-8859-1"), bytes);

            string sConvertedString = Encoding.UTF8.GetString(encoded);

            fullPath = $"{path}\\{sConvertedString}";

            FileStream os = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {

                byte[] buff = new byte[10240000];
                int c = 0;
                while ((c = s.Read(buff, 0, 10400)) > 0)
                {
                    os.Write(buff, 0, c);
                    os.Flush();
                }

            }
            catch (Exception e)
            {

                Console.WriteLine($"oid: {oid}");
                Console.WriteLine($"Error message {e.Message}");
            }
            finally
            {
                os.Close();
                s.Close();

            }

            return sConvertedString;


        }

        private void SaveBinDocument(string path, byte[] bin, string fileName)
        {
            var fullPath = $"{this.recourceFolderSetting.SettingValue}\\{path}";
            bool exists = System.IO.Directory.Exists(path);

            if (!exists)
                System.IO.Directory.CreateDirectory(fullPath);

            //FileStream os = new FileStream($"{this.recourceFolderSetting.SettingValue}\\{path}", FileMode.OpenOrCreate, FileAccess.Write);

            //var stream = File.Create(fullPath);
            //stream.Write(bin, 0, bin.Length);
            //stream.Close();

            var file = new MemoryStream(bin, 0, bin.Length, false, true);

            using (FileStream filestream = new FileStream($"{fullPath}\\{fileName}", FileMode.Create, FileAccess.Write))
            {
                file.WriteTo(filestream);
                filestream.Close();
                file.Close();
            }
        }

        static public string ConvertCyrToLatin(string arg)
        {
            StringBuilder builder = new StringBuilder(arg, arg.Length + 10);
            //convert lower-case letters
            builder.Replace("а", "a");
            builder.Replace("б", "b");
            builder.Replace("в", "v");
            builder.Replace("г", "g");
            builder.Replace("д", "d");
            builder.Replace("е", "e");
            builder.Replace("ж", "zh");
            builder.Replace("з", "z");
            builder.Replace("и", "i");
            builder.Replace("й", "y");
            builder.Replace("к", "k");
            builder.Replace("л", "l");
            builder.Replace("м", "m");
            builder.Replace("н", "n");
            builder.Replace("о", "o");
            builder.Replace("п", "p");
            builder.Replace("р", "r");
            builder.Replace("с", "s");
            builder.Replace("т", "t");
            builder.Replace("у", "u");
            builder.Replace("ф", "f");
            builder.Replace("х", "h");
            builder.Replace("ц", "ts");
            builder.Replace("ч", "ch");
            builder.Replace("ш", "sh");
            builder.Replace("щ", "sht");
            builder.Replace("ъ", "a");
            builder.Replace("ь", "y");
            builder.Replace("ю", "yu");
            builder.Replace("я", "ya");

            //convert upper-case letters
            builder.Replace("А", "A");
            builder.Replace("Б", "B");
            builder.Replace("В", "V");
            builder.Replace("Г", "G");
            builder.Replace("Д", "D");
            builder.Replace("Е", "E");
            builder.Replace("Ж", "Zh");
            builder.Replace("З", "Z");
            builder.Replace("И", "I");
            builder.Replace("Й", "Y");
            builder.Replace("К", "K");
            builder.Replace("Л", "L");
            builder.Replace("М", "M");
            builder.Replace("Н", "N");
            builder.Replace("О", "O");
            builder.Replace("П", "P");
            builder.Replace("Р", "R");
            builder.Replace("С", "S");
            builder.Replace("Т", "T");
            builder.Replace("У", "U");
            builder.Replace("Ф", "F");
            builder.Replace("Х", "H");
            builder.Replace("Ц", "Ts");
            builder.Replace("Ч", "Ch");
            builder.Replace("Ш", "Sh");
            builder.Replace("Щ", "Sht");
            builder.Replace("Ъ", "A");
            builder.Replace("Ь", "Y");
            builder.Replace("Ю", "Yu");
            builder.Replace("Я", "Ya");
            builder.Replace("-", "-");


            return builder.ToString();
        }

        private DateTime? GetBirthDate(string indent)
        {
            string EGN = "";
            BasicEGNValidation validation;
            if (!string.IsNullOrEmpty(indent))
            {
                indent = indent.Trim();
                EGN = indent;
                validation = new BasicEGNValidation(EGN);
                if (validation.Validate())
                {
                    char charLastDigit = EGN[EGN.Length - 2];
                    int lastDigit = Convert.ToInt32(new string(charLastDigit, 1));
                    int year = int.Parse(EGN.Substring(0, 2));
                    int month = int.Parse(EGN.Substring(2, 2));
                    int day = int.Parse(EGN.Substring(4, 2));
                    if (month < 13)
                    {
                        year += 1900;
                    }
                    else if (month > 20 && month < 33)
                    {
                        year += 1800;
                        month -= 20;
                    }
                    else if (month > 40 && month < 53)
                    {
                        year += 2000;
                        month -= 40;
                    }
                    var dt = new DateTime(year, month, day);
                    return dt;
                }
            }
            return null;
        }

        private SPPOOOrder GetOrder(string order)
        {
            foreach (var ord in orders)
            {
                if (ord.OrderNumber.Equals(order))
                {
                    return ord;
                }
            }
            return new SPPOOOrder();
        }

        private void LogStrartInformation(string MethodName)
        {
            this.startDate = DateTime.Now;
            this.logger.LogInformation($"{startDate.ToString("dd-MM-yyyy HH:mm:ss")} Започнат метод за миграция {MethodName}");
        }

        private void LogEndInformation(string MethodName)
        {
            this.endDate = DateTime.Now;

            this.logger.LogInformation($"{endDate.ToString("dd-MM-yyyy HH:mm:ss")} Приключи метод за миграция {MethodName}");

            TimeSpan span = this.endDate.Subtract(startDate);

            this.logger.LogInformation($"{MethodName}-------> Duration days: {span.Days} hours: {span.Hours} minutes:{span.Minutes} seconds: {span.Seconds}");
        }
        #endregion
    }
}