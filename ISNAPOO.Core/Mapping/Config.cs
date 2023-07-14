using AutoMapper;
using Data.Models;
using Data.Models.Data.Archive;
using Data.Models.Data.Assessment;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.DOC;
using Data.Models.Data.EGovPayment;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Rating;
using Data.Models.Data.Request;
using Data.Models.Data.Role;
using Data.Models.Data.SPPOO;
using Data.Models.Data.Training;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.DOC.NKPD;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.Core.ViewModels.Rating;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ISNAPOO.Core.Mapping
{
    public class Config
    {
        private static bool initialized;

        public static IMapper Mapper { get; set; }

        public static void RegisterMappings(params Assembly[] assemblies)
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            var types = assemblies.SelectMany(n => n.GetExportedTypes()).ToList();

            var config = new MapperConfigurationExpression();
            config.CreateProfile("ReflectionProfile", configuration =>
            {
                #region Administration
                configuration.CreateMap<Setting, SettingVM>().ReverseMap();
                configuration.CreateMap<KeyType, KeyTypeVM>()
                .ForMember(dest => dest.KeyValues, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<KeyValue, KeyValueVM>().ReverseMap();
                configuration.CreateMap<ApplicationRole, ApplicationRoleVM>().ReverseMap();

                configuration.CreateMap<Policy, PolicyVM>().ReverseMap();

                configuration.CreateMap<Person, PersonVM>()
               .ForMember(dest => dest.Location, options => options.ExplicitExpansion())
               .ForMember(dest => dest.CandidateProviderPersons, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<ApplicationUser, ApplicationUserVM>()
                .ForMember(dest => dest.Roles, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<ScheduleProcessHistory, ScheduleProcessHistoryVM>()
                    .ReverseMap();
                #endregion

                #region ProviderData

                configuration.CreateMap<Provider, ProviderVM>()
                .ForMember(dest => dest.Location, options => options.ExplicitExpansion())
                .ForMember(dest => dest.LocationCorrespondence, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ProcedurePrice, ProcedurePriceVM>()
                .ReverseMap();

                configuration.CreateMap<ManagementDeadlineProcedure, ManagementDeadlineProcedureVM>()
                .ReverseMap();

                configuration.CreateMap<StartedProcedure, StartedProcedureVM>()
                .ForMember(dest => dest.StartedProcedureProgresses, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProcedureDocuments, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProcedureExternalExperts, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProcedureExpertCommissions, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CandidateProviders, options => options.ExplicitExpansion())
                .ForMember(dest => dest.NegativeIssues, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<StartedProcedureProgress, StartedProcedureProgressVM>()
                .ForMember(dest => dest.StartedProcedure, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ProcedureDocument, ProcedureDocumentVM>()
                .ForMember(dest => dest.StartedProcedure, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Expert, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProcedureDocumentNotifications, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ProcedureExternalExpert, ProcedureExternalExpertVM>()
                .ForMember(dest => dest.StartedProcedure, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Expert, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProfessionalDirection, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProcedureDocument, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<NegativeIssue, NegativeIssueVM>()
                .ForMember(dest => dest.StartedProcedure, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ProcedureExpertCommission, ProcedureExpertCommissionVM>()
                .ForMember(dest => dest.StartedProcedure, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ProcedureDocumentNotification, ProcedureDocumentNotificationVM>()
                    .ForMember(dest => dest.Notification, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ProcedureDocument, options => options.ExplicitExpansion())
                .ReverseMap();



                configuration.CreateMap<Payment, PaymentVM>()
                .ForMember(dest => dest.ProcedurePrice, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                .ReverseMap();


                #endregion

                #region DOS
                configuration.CreateMap<Data.Models.Data.DOC.DOC, DocVM>()
               .ForMember(dest => dest.Profession, options => options.ExplicitExpansion())
               .ForMember(dest => dest.ERUs, options => options.ExplicitExpansion())
               .ForMember(dest => dest.Specialities, options => options.ExplicitExpansion())
               .ForMember(dest => dest.DOCNKPDs, options => options.ExplicitExpansion())
               .ReverseMap();


                configuration.CreateMap<DOC_DOC_NKPD, DOCNKPDVM>()
                .ForMember(dest => dest.DOC, options => options.ExplicitExpansion())
                .ForMember(dest => dest.NKPD, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region NKPD
                configuration.CreateMap<SpecialityNKPD, SpecialityNKPDVM>()
                .ForMember(dest => dest.Speciality, options => options.ExplicitExpansion())
                .ForMember(dest => dest.NKPD, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<Data.Models.Data.DOC.NKPD, NKPDVM>()
                .ReverseMap();
                #endregion

                #region ERU
                configuration.CreateMap<ERU, ERUVM>()
                .ForMember(dest => dest.DOC, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ERUSpecialities, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Specialities, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CandidateCurriculumERUs, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ERUSpeciality, ERUSpecialityVM>()
                .ForMember(dest => dest.ERU, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Speciality, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region Expert
                configuration.CreateMap<Expert, ExpertVM>()
               .ForMember(dest => dest.ExpertProfessionalDirections, options => options.ExplicitExpansion())
               .ForMember(dest => dest.ExpertDocuments, options => options.ExplicitExpansion())
               .ForMember(dest => dest.ExpertExpertCommissions, options => options.ExplicitExpansion())
               .ForMember(dest => dest.ExpertDOCs, options => options.ExplicitExpansion())
               .ForMember(dest => dest.Person, options => options.ExplicitExpansion())
               .ForMember(dest => dest.ProcedureDocuments, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<ExpertProfessionalDirection, ExpertProfessionalDirectionVM>()
                .ForMember(dest => dest.Expert, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProfessionalDirection, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ExpertDocument, ExpertDocumentVM>()
                .ForMember(dest => dest.Expert, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ExpertExpertCommission, ExpertExpertCommissionVM>()
                .ForMember(dest => dest.Expert, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ExpertDOC, ExpertDOCVM>()
                .ForMember(dest => dest.Expert, options => options.ExplicitExpansion())
                .ForMember(dest => dest.DOC, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ExpertNapoo, ExpertNapooVM>()
                .ForMember(dest => dest.Expert, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region Control
                configuration.CreateMap<FollowUpControl, FollowUpControlVM>()
               .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
               .ForMember(dest => dest.FollowUpControlExperts, options => options.ExplicitExpansion())
               .ForMember(dest => dest.FollowUpControlDocuments, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<FollowUpControlExpert, FollowUpControlExpertVM>()
               .ForMember(dest => dest.FollowUpControl, options => options.ExplicitExpansion())
               .ForMember(dest => dest.Expert, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<FollowUpControlDocument, FollowUpControlDocumentVM>()
               .ForMember(dest => dest.FollowUpControl, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<FollowUpControlDocumentNotification, FollowUpControlDocumentNotificationVM>()
                    .ForMember(dest => dest.FollowUpControlDocument, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Notification, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<FollowUpControlUploadedFile, FollowUpControlUploadedFileVM>()
                    .ForMember(dest => dest.FollowUpControl, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region FrameworkProgram
                configuration.CreateMap<FrameworkProgram, FrameworkProgramVM>()
                .ForMember(dest => dest.FrameworkProgramFormEducations, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<FrameworkProgramFormEducation, FrameworkProgramFormEducationVM>()
                .ForMember(dest => dest.FrameworkProgram, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region TemplateDocument
                configuration.CreateMap<TemplateDocument, TemplateDocumentVM>()
                .ReverseMap();
                #endregion

                #region Order
                configuration.CreateMap<SpecialityOrder, SpecialityOrderVM>()
                .ForMember(dest => dest.Speciality, options => options.ExplicitExpansion())
                .ForMember(dest => dest.SPPOOOrder, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ProfessionalDirectionOrder, ProfessionalDirectionOrderVM>()
                .ForMember(dest => dest.SPPOOOrder, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProfessionalDirection, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ProfessionOrder, ProfessionOrderVM>()
                .ForMember(dest => dest.Profession, options => options.ExplicitExpansion())
                .ForMember(dest => dest.SPPOOOrder, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<SPPOOOrder, OrderVM>()
                .ReverseMap();

                configuration.CreateMap<LegalCapacityOrdinanceUploadedFile, LegalCapacityOrdinanceUploadedFileVM>()
                .ReverseMap();
                #endregion

                #region MenuNode
                configuration.CreateMap<MenuNode, MenuNodeVM>()
                .ReverseMap();

                configuration.CreateMap<MenuNodeRole, MenuNodeRoleVM>()
                    .ForMember(dest => dest.ApplicationRole, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.MenuNode, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region Notification
                configuration.CreateMap<Notification, NotificationVM>()
                   .ForMember(dest => dest.PersonFrom, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.PersonTo, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.ProcedureDocumentNotifications, options => options.ExplicitExpansion())
               .ReverseMap();
                #endregion

                #region EKATTE
                configuration.CreateMap<District, DistrictVM>()
                //.ForMember(dest => dest.Municipalities, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<Region, RegionVM>()
                .ForMember(dest => dest.Municipality, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<Location, LocationVM>()
                .ForMember(dest => dest.Municipality, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<Municipality, MunicipalityVM>()
                .ForMember(dest => dest.Locations, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Regions, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region SPPOO
                configuration.CreateMap<Area, AreaVM>()
               .ForMember(dest => dest.ProfessionalDirections, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<ProfessionalDirection, ProfessionalDirectionVM>()
                .ForMember(dest => dest.Professions, options => options.ExplicitExpansion())
                .ForMember(dest => dest.OrderVMs, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProfessionalDirectionOrders, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<Profession, ProfessionVM>()
                .ForMember(dest => dest.Specialities, options => options.ExplicitExpansion())
                .ForMember(dest => dest.OrderVMs, options => options.ExplicitExpansion())
                .ForMember(dest => dest.DocVMList, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProfessionOrders, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProfessionalDirection, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<Speciality, SpecialityVM>()
                .ForMember(dest => dest.OrderVMs, options => options.ExplicitExpansion())
                .ForMember(dest => dest.SpecialityNKPDs, options => options.ExplicitExpansion())
                .ForMember(dest => dest.SpecialityOrders, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Doc, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Profession, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CandidateProviderTrainerSpecialities, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CandidateProviderPremisesSpecialities, options => options.ExplicitExpansion())
                .ReverseMap();

                #endregion

                #region CandidateProvider
                configuration.CreateMap<CandidateProvider, CandidateProviderVM>()
                   .ForMember(dest => dest.Location, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.LocationCorrespondence, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.RegionCorrespondence, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderSpecialities, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderTrainers, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderDocuments, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.StartedProcedure, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderPersons, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.RequestReports, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.ReportUploadedDocs, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderActive, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.ProviderOwnership, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.AnnualInfos, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.Programs, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderConsultings, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.ValidationClients, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.SelfAssessmentReports, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.ProviderRequestDocuments, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.FollowUpControls, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.Courses, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.Payments, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<CandidateProviderPerson, CandidateProviderPersonVM>()
                  .ForMember(dest => dest.Person, options => options.ExplicitExpansion())
                  .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<CandidateProviderCPOStructureActivity, CandidateProviderCPOStructureActivityVM>()
                  .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<CandidateProviderConsulting, CandidateProviderConsultingVM>()
                  .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<CandidateProviderCIPOStructureActivity, CandidateProviderCIPOStructureActivityVM>()
                  .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<CandidateCurriculum, CandidateCurriculumVM>()
                    .ForMember(dest => dest.CandidateProviderSpeciality, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.SelectedERUs, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateCurriculumERUs, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateCurriculumModification, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateCurriculumModification, CandidateCurriculumModificationVM>()
                    .ForMember(dest => dest.CandidateProviderSpeciality, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateCurriculums, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderSpeciality, CandidateProviderSpecialityVM>()
                    .ForMember(dest => dest.CandidateProvider, opt => opt.ExplicitExpansion())
                    .ForMember(dest => dest.Speciality, opt => opt.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateCurriculums, opt => opt.ExplicitExpansion())
                    .ForMember(dest => dest.FrameworkProgram, opt => opt.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateCurriculumModifications, opt => opt.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateCurriculumERU, CandidateCurriculumERUVM>()
                    .ForMember(dest => dest.CandidateCurriculum, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ERU, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderTrainer, CandidateProviderTrainerVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderTrainerProfiles, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderTrainerQualifications, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderTrainerSpecialities, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderTrainerCheckings, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.SelectedSpecialities, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderTrainerDocuments, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderTrainerProfile, CandidateProviderTrainerProfileVM>()
                    .ForMember(dest => dest.CandidateProviderTrainer, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ProfessionalDirection, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderTrainerQualification, CandidateProviderTrainerQualificationVM>()
                    .ForMember(dest => dest.CandidateProviderTrainer, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Profession, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderTrainerDocument, CandidateProviderTrainerDocumentVM>()
                    .ForMember(dest => dest.CandidateProviderTrainer, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderPremises, CandidateProviderPremisesVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderPremisesRooms, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderPremisesCheckings, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Location, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderPremisesSpecialities, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.SelectedSpecialities, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderPremisesDocuments, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderPremisesRoom, CandidateProviderPremisesRoomVM>()
                    .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderPremisesDocument, CandidateProviderPremisesDocumentVM>()
                    .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderDocument, CandidateProviderDocumentVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderTrainerSpeciality, CandidateProviderTrainerSpecialityVM>()
                    .ForMember(dest => dest.CandidateProviderTrainer, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Speciality, options => options.ExplicitExpansion())
                .ReverseMap();


                configuration.CreateMap<CandidateProviderTrainerChecking, CandidateProviderTrainerCheckingVM>()
                    .ForMember(dest => dest.CandidateProviderTrainer, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderPremisesChecking, CandidateProviderPremisesCheckingVM>()
                   .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<CandidateProviderPremisesSpeciality, CandidateProviderPremisesSpecialityVM>()
                    .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Speciality, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CandidateProviderLicenceChange, CandidateProviderLicenceChangeVM>()
                  .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
               .ReverseMap();
                #endregion

                #region DocumentRequest
                configuration.CreateMap<TypeOfRequestedDocument, TypeOfRequestedDocumentVM>()
                    .ForMember(dest => dest.RequestDocumentTypes, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.DocumentSeries, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestDocumentManagements, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.DocumentSerialNumbers, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ClientCourseDocuments, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<RequestDocumentType, RequestDocumentTypeVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.TypeOfRequestedDocument, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ProviderRequestDocument, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestDocumentManagement, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<RequestDocumentStatus, RequestDocumentStatusVM>()
                    .ForMember(dest => dest.ProviderRequestDocument, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<NAPOORequestDoc, NAPOORequestDocVM>()
                    .ForMember(dest => dest.ProviderRequestDocuments, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ProviderRequestDocument, ProviderRequestDocumentVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.NAPOORequestDoc, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestDocumentStatuses, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestDocumentTypes, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.LocationCorrespondence, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestDocumentManagements, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ProviderDocumentOffer, ProviderDocumentOfferVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.TypeOfRequestedDocument, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<RequestDocumentManagement, RequestDocumentManagementVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderPartner, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.TypeOfRequestedDocument, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ProviderRequestDocument, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.DocumentSerialNumbers, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestDocumentTypes, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestDocumentManagementUploadedFiles, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestReport, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<DocumentSeries, DocumentSeriesVM>()
                    .ForMember(dest => dest.TypeOfRequestedDocument, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<DocumentSerialNumber, DocumentSerialNumberVM>()
                    .ForMember(dest => dest.TypeOfRequestedDocument, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestDocumentManagement, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestReport, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ClientCourseDocuments, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ValidationClientDocuments, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<RequestReport, RequestReportVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ReportUploadedDocs, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.DocumentSerialNumbers, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ReportUploadedDoc, ReportUploadedDocVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.RequestReport, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<RequestDocumentManagementUploadedFile, RequestDocumentManagementUploadedFileVM>()
                    .ForMember(dest => dest.RequestDocumentManagement, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region Training
                configuration.CreateMap<Client, ClientVM>()
                .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ProfessionalDirection, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ClientCourse, ClientCourseVM>()
                .ForMember(dest => dest.ProfessionalDirection, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Speciality, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ClientCourseDocuments, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Client, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ClientCourseSubjectGrades, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CourseProtocolGrades, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ClientCourseStatuses, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ClientRequiredDocuments, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ClientCourseDocument, ClientCourseDocumentVM>()
                .ForMember(dest => dest.ClientCourse, options => options.ExplicitExpansion())
                .ForMember(dest => dest.DocumentSerialNumber, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CourseProtocol, options => options.ExplicitExpansion())
                .ForMember(dest => dest.TypeOfRequestedDocument, options => options.ExplicitExpansion())
                .ForMember(dest => dest.OriginalClientCourseDocument, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CourseDocumentUploadedFiles, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<Course, CourseVM>()
               .ForMember(dest => dest.Program, options => options.ExplicitExpansion())
               .ForMember(dest => dest.Location, options => options.ExplicitExpansion())
               .ForMember(dest => dest.ClientCourses, options => options.ExplicitExpansion())
               .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
               .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
               .ForMember(dest => dest.CourseCommissionMembers, options => options.ExplicitExpansion())
               .ForMember(dest => dest.CourseSubjects, options => options.ExplicitExpansion())
               .ForMember(dest => dest.CourseProtocols, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<CourseChecking, CourseCheckingVM>()
               .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
               .ForMember(dest => dest.FollowUpControl, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<Program, ProgramVM>()
                .ForMember(dest => dest.Courses, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                .ForMember(dest => dest.FrameworkProgram, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Speciality, options => options.ExplicitExpansion())
                .ForMember(dest => dest.TrainingCurriculums, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<TrainerCourse, TrainerCourseVM>()
                .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CandidateProviderTrainer, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationTrainer, ValidationTrainerVM>()
                .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
                .ForMember(dest => dest.CandidateProviderTrainer, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationCompetency, ValidationCompetencyVM>()
               .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<CourseDocumentUploadedFile, CourseDocumentUploadedFileVM>()
                .ForMember(dest => dest.ClientCourseDocument, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<TrainingCurriculumERU, TrainingCurriculumERUVM>()
                    .ForMember(dest => dest.TrainingCurriculum, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ERU, options => options.ExplicitExpansion())
                .ReverseMap();
                
                configuration.CreateMap<ValidationCurriculumERU, ValidationCurriculumERUVM>()
                    .ForMember(dest => dest.ValidationCurriculum, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ERU, options => options.ExplicitExpansion())
                .ReverseMap();


                configuration.CreateMap<TrainingCurriculum, TrainingCurriculumVM>()
                    .ForMember(dest => dest.CandidateCurriculum, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderSpeciality, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Program, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.TrainingCurriculumERUs, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CourseSchedules, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationCurriculum, ValidationCurriculumVM>()
                    .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderSpeciality, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ValidationCurriculumERUs, options => options.ExplicitExpansion())
                    .ReverseMap();

                configuration.CreateMap<PremisesCourse, PremisesCourseVM>()
                    .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationPremises, ValidationPremisesVM>()
                    .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CourseCommissionMember, CourseCommissionMemberVM>()
                    .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CourseSubjectGrade, CourseSubjectGradeVM>()
                    .ForMember(dest => dest.ClientCourse, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CourseSubject, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CourseSubject, CourseSubjectVM>()
                    .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ClientCourseSubjectGrades, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CourseProtocol, CourseProtocolVM>()
                    .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CourseProtocolGrades, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ClientCourseDocuments, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CourseOrder, CourseOrderVM>()
                    .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<CourseProtocolGrade, CourseProtocolGradeVM>()
                    .ForMember(dest => dest.CourseProtocol, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ClientCourse, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationProtocolGrade, ValidationProtocolGradeVM>()
                    .ForMember(dest => dest.ValidationProtocol, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ClientCourseStatus, ClientCourseStatusVM>()
                    .ForMember(dest => dest.ClientCourse, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ClientRequiredDocument, ClientRequiredDocumentVM>()
                    .ForMember(dest => dest.Course, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ClientCourse, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationClientRequiredDocument, ValidationClientRequiredDocumentVM>()
                   .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<Consulting, ConsultingVM>()
                    .ForMember(dest => dest.ConsultingClient, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ConsultingClient, ConsultingClientVM>()
                    .ForMember(dest => dest.Client, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ConsultingTrainers, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ConsultingPremises, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ConsultingDocumentUploadedFiles, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ConsultingClientRequiredDocument, ConsultingClientRequiredDocumentVM>()
                    .ForMember(dest => dest.ConsultingClient, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ConsultingDocumentUploadedFile, ConsultingDocumentUploadedFileVM>()
                    .ForMember(dest => dest.ConsultingClient, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ConsultingTrainer, ConsultingTrainerVM>()
                    .ForMember(dest => dest.CandidateProviderTrainer, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ConsultingClient, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ConsultingPremises, ConsultingPremisesVM>()
                    .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ConsultingClient, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationClient, ValidationClientVM>()
                .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Client, options => options.ExplicitExpansion())
                .ForMember(dest => dest.Speciality, options => options.ExplicitExpansion())
                .ForMember(dest => dest.FrameworkProgram, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ValidationClientDocuments, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ValidationProtocols, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ValidationCommissionMembers, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ValidationClientRequiredDocuments, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ValidationProtocolGrades, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ValidationCurriculums, options => options.ExplicitExpansion())
                .ForMember(dest => dest.ValidationPremises, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationClientChecking, ValidationClientCheckingVM>()
                    .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.FollowUpControl, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationOrder, ValidationOrderVM>()
                    .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ValidationClientDocument, ValidationClientDocumentVM>()
               .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
               .ForMember(dest => dest.ValidationProtocol, options => options.ExplicitExpansion())
               .ForMember(dest => dest.DocumentSerialNumber, options => options.ExplicitExpansion())
               .ForMember(dest => dest.OriginalValidationClientDocument, options => options.ExplicitExpansion())
               .ForMember(dest => dest.TypeOfRequestedDocument, options => options.ExplicitExpansion())
               .ForMember(dest => dest.ValidationDocumentUploadedFiles, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<ValidationCommissionMember, ValidationCommissionMemberVM>()
               .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<ValidationDocumentUploadedFile, ValidationDocumentUploadedFileVM>()
               .ForMember(dest => dest.ValidationClientDocument, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<ValidationProtocol, ValidationProtocolVM>()
               .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
               .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<CourseSchedule, CourseScheduleVM>()
                    .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProviderTrainer, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.TrainingCurriculum, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region Archive
                configuration.CreateMap<AnnualInfo, AnnualInfoVM>()
                .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<AnnualInfoStatus, AnnualInfoStatusVM>()
               .ForMember(dest => dest.AnnualInfo, options => options.ExplicitExpansion())
               .ReverseMap();

                configuration.CreateMap<AnnualReportNSI, AnnualReportNSIVM>()
                .ReverseMap();

                configuration.CreateMap<RegiXLogRequest, RegiXLogRequestVM>()
                .ReverseMap();

                configuration.CreateMap<SelfAssessmentReport, SelfAssessmentReportVM>()
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.SelfAssessmentReportStatuses, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<SelfAssessmentReportStatus, SelfAssessmentReportStatusVM>()
                    .ForMember(dest => dest.IdSelfAssessmentReport, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ArchCandidateProvider, CandidateProviderVM>()
                   .ForMember(dest => dest.Location, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.LocationCorrespondence, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.RegionCorrespondence, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderSpecialities, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderTrainers, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderPremises, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderDocuments, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.StartedProcedure, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderPersons, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.RequestReports, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.ReportUploadedDocs, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderActive, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.ProviderOwnership, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.AnnualInfos, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.Programs, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.CandidateProviderConsultings, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.SelfAssessmentReports, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ArchCandidateProviderTrainer, CandidateProviderTrainerVM>()
                   .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                   .ForMember(dest => dest.ProfessionalQualificationCertificate, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<ArchCandidateProviderTrainerQualification, CandidateProviderTrainerQualificationVM>()
                .ReverseMap();

                configuration.CreateMap<ArchCandidateProviderPremises, CandidateProviderPremisesVM>()
                 .ForMember(dest => dest.CandidateProvider, opt => opt.ExplicitExpansion())
                 .ForMember(dest => dest.CandidateProviderPremisesSpecialities, options => options.ExplicitExpansion())
                 .ReverseMap();

                configuration.CreateMap<ArchCandidateProviderPremisesSpeciality, CandidateProviderPremisesSpecialityVM>()                 
                 .ReverseMap();

                configuration.CreateMap<ArchCandidateProviderSpeciality, CandidateProviderSpecialityVM>()
                 .ForMember(dest => dest.CandidateCurriculums, opt => opt.ExplicitExpansion())
                 .ForMember(dest => dest.CandidateProvider, opt => opt.ExplicitExpansion())
                 .ReverseMap();

                configuration.CreateMap<ArchCandidateCurriculum, CandidateCurriculumVM>()
                 .ReverseMap();

                #endregion

                #region Assessment
                configuration.CreateMap<Survey, SurveyVM>()
                    .ForMember(dest => dest.Questions, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.SurveyResults, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<Question, QuestionVM>()
                    .ForMember(dest => dest.Survey, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Answers, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.UserAnswers, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.UserAnswerOpens, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<Answer, AnswerVM>()
                    .ForMember(dest => dest.Question, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.UserAnswers, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<SurveyResult, SurveyResultVM>()
                    .ForMember(dest => dest.Survey, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ConsultingClient, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ClientCourse, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.ValidationClient, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.UserAnswerOpens, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<UserAnswer, UserAnswerVM>()
                    .ForMember(dest => dest.Answer, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.UserAnswerOpen, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Question, options => options.ExplicitExpansion())
                .ReverseMap();

                configuration.CreateMap<UserAnswerOpen, UserAnswerOpenVM>()
                    .ForMember(dest => dest.SurveyResult, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.Question, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.UserAnswers, options => options.ExplicitExpansion())
                .ReverseMap();
                #endregion

                #region Rating
                configuration.CreateMap<Indicator, IndicatorVM>()
                    .ForMember(dest => dest.IndicatorDetails, options => options.ExplicitExpansion())
                    .ReverseMap();

                configuration.CreateMap<CandidateProviderIndicator, CandidateProviderIndicatorVM>()
                    .ForMember(dest => dest.IndicatorDetails, options => options.ExplicitExpansion())
                    .ForMember(dest => dest.CandidateProvider, options => options.ExplicitExpansion());

                configuration.CreateMap<CandidateProviderIndicatorVM, CandidateProviderIndicator>()
                .ForMember(dest => dest.CandidateProvider, options => options.Ignore())
                .ForMember(dest => dest.Indicator, options => options.Ignore());
                #endregion

                #region AllowIP
                configuration.CreateMap<AllowIP, AllowIPVM>()              
                    .ReverseMap();
                #endregion

                #region EventLog
                configuration.CreateMap<EventLog, EventLogVM>()
                   .ReverseMap();
                #endregion

                //IMapTo<>
                foreach (var mapping in GetToMappings(types))
                {
                    configuration.CreateMap(mapping.Source, mapping.Destination);
                }

                //IMapFrom<>
                foreach (var mapping in GetFromMappings(types))
                {
                    config.CreateMap(mapping.Source, mapping.Destination);
                }

                //ICustomMappings
                foreach (var mapping in GetCustomMappings(types))
                {
                    mapping.CreateMappings(configuration);
                }
            });

            Mapper = new Mapper(new MapperConfiguration(config));
        }

        private static IEnumerable<TypesMap> GetToMappings(IEnumerable<Type> types)
        {
            var toMappings = from t in types
                             from i in t.GetTypeInfo().GetInterfaces()
                             where i.GetTypeInfo().IsGenericType &&
                                   i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                                   !t.GetTypeInfo().IsAbstract &&
                                   !t.GetTypeInfo().IsInterface
                             select new TypesMap
                             {
                                 Source = t,
                                 Destination = i.GetTypeInfo().GetGenericArguments()[0],
                             };

            return toMappings;
        }

        private static IEnumerable<TypesMap> GetFromMappings(IEnumerable<Type> types)
        {
            var fromMappings = from t in types
                               from i in t.GetTypeInfo().GetInterfaces()
                               where i.GetTypeInfo().IsGenericType &&
                                     i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                                     !t.GetTypeInfo().IsAbstract &&
                                     !t.GetTypeInfo().IsInterface
                               select new TypesMap
                               {
                                   Source = i.GetTypeInfo().GetGenericArguments()[0],
                                   Destination = t,
                               };

            return fromMappings;
        }

        private static IEnumerable<ICustomMappings> GetCustomMappings(IEnumerable<Type> types)
        {
            var customMaps = from t in types
                             from i in t.GetTypeInfo().GetInterfaces()
                             where typeof(ICustomMappings).GetTypeInfo().IsAssignableFrom(t) &&
                                   !t.GetTypeInfo().IsAbstract &&
                                   !t.GetTypeInfo().IsInterface
                             select (ICustomMappings)Activator.CreateInstance(t);

            return customMaps;
        }
    }
}
