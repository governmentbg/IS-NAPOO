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
using Data.Models.Data.SqlView.Archive;
using Data.Models.Data.SqlView.Reports;
using Data.Models.Data.SqlView.WebIntegrationService;
using Data.Models.Data.Training;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Data.Models.DB
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<DocSpeciality>()
            //    .HasOne(pt => pt.Speciality)
            //    .WithMany(p => p.DocSpecialities)
            //    .HasForeignKey(pt => pt.IdSpeciality);

            //builder.Entity<DocSpeciality>()
            //    .HasOne(pt => pt.Doc)
            //    .WithMany(t => t.DocSpecialities)
            //    .HasForeignKey(pt => pt.IdDOC);

            //Игнорираме таблица в миграцията
            builder.Entity<NAPOOSearch>().Metadata.SetIsTableExcludedFromMigrations(true);
            builder.Entity<NAPOOSearchCipo>().Metadata.SetIsTableExcludedFromMigrations(true);
            builder.Entity<NAPOOSearchCourses>().Metadata.SetIsTableExcludedFromMigrations(true);
            builder.Entity<NAPOOStatistics>().Metadata.SetIsTableExcludedFromMigrations(true);
            builder.Entity<AnnualTrainingCourse>().Metadata.SetIsTableExcludedFromMigrations(true);
            builder.Entity<AnnualStudents>().Metadata.SetIsTableExcludedFromMigrations(true);
            builder.Entity<AnnualStudentsByNationality>().Metadata.SetIsTableExcludedFromMigrations(true);
            builder.Entity<StudentDocumentVM>().Metadata.SetIsTableExcludedFromMigrations(true);    
            

            builder.Entity<GetCPOWithOutCourseByPeriod>().Metadata.SetIsTableExcludedFromMigrations(true);
        }

        #region Providers
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderPerson> ProviderPersons { get; set; }

        
        #endregion

        #region Common
        public DbSet<KeyValue> KeyValues { get; set; }
        public DbSet<KeyType> KeyTypes { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AllowIP> AllowIPs { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Municipality> Municipalities { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Location> Locations { get; set; }        
        public DbSet<MenuNode> MenuNodes { get; set; }
        public DbSet<MenuNodeRole> MenuNodeRoles { get; set; }
        public DbSet<TemplateDocument> TemplateDocuments { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<ScheduleProcessHistory> ScheduleProcessHistories { get; set; }
        public DbSet<AuthenticationTicket> AuthenticationTickets { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
		



		#endregion

		#region SPPOO

		public DbSet<Area> Areas { get; set; }
        public DbSet<ProfessionalDirection> ProfessionalDirections { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<SPPOOOrder> SPPOOOrders { get; set; }
        public DbSet<SpecialityNKPD> SpecialityNKPDs { get; set; }
        public DbSet<SpecialityOrder> SpecialityOrders { get; set; }
        public DbSet<FrameworkProgram> FrameworkPrograms { get; set; }
        public DbSet<ProfessionOrder> ProfessionOrders { get; set; }
        public DbSet<ProfessionalDirectionOrder> ProfessionalDirectionOrders { get; set; }
        public DbSet<FrameworkProgramFormEducation> FrameworkProgramFormEducations { get; set; }
        public DbSet<LegalCapacityOrdinanceUploadedFile> LegalCapacityOrdinanceUploadedFiles { get; set; }


        #endregion

        #region DOC
        public DbSet<Data.DOC.DOC> DOCs { get; set; }
        public DbSet<ERU> ERUs { get; set; }
        public DbSet<ERUSpeciality> ERUSpecialities { get; set; }
               
        public DbSet<NKPD> NKPDs { get; set; }

        public DbSet<DOC_DOC_NKPD> DOC_DOC_NKPDs { get; set; }

        

        #endregion

        #region ExternalExpertCommission
        public DbSet<Expert> Experts { get; set; }
        public DbSet<ExpertProfessionalDirection> ExpertProfessionalDirections { get; set; }
        public DbSet<ExpertDocument> ExpertDocuments { get; set; }
        public DbSet<ExpertExpertCommission> ExpertExpertCommissions { get; set; }
        public DbSet<ExpertDOC> ExpertDOCs { get; set; }

        public DbSet<ExpertNapoo> ExpertNapoos { get; set; }

        




        #endregion

        #region Лицензиране на ЦПО,ЦИПО

        public DbSet<CandidateProvider> CandidateProviders { get; set; }
        public DbSet<CandidateProviderPerson> CandidateProviderPersons { get; set; }
        public DbSet<CandidateProviderSpeciality> CandidateProviderSpecialities { get; set; }
        public DbSet<CandidateProviderTrainerChecking> CandidateProviderTrainerCheckings { get; set; } 
        public DbSet<CandidateCurriculum> CandidateCurriculums { get; set; }
        public DbSet<CandidateCurriculumModification> CandidateCurriculumModification { get; set; }
        public DbSet<CandidateProviderTrainer> CandidateProviderTrainers { get; set; }
        public DbSet<CandidateProviderTrainerProfile> CandidateProviderTrainerProfiles { get; set; }
        public DbSet<CandidateProviderTrainerQualification> CandidateProviderTrainerQualifications { get; set; }
        public DbSet<CandidateProviderTrainerDocument> CandidateProviderTrainerDocuments { get; set; }
        public DbSet<CandidateProviderStatus> CandidateProviderStatuses { get; set; }
        public DbSet<CandidateProviderPremises> CandidateProviderPremises { get; set; }
        public DbSet<CandidateProviderConsulting> CandidateProviderConsultings { get; set; }
        public DbSet<CandidateProviderPremisesRoom> CandidateProviderPremisesRooms { get; set; }
        public DbSet<CandidateProviderPremisesDocument> CandidateProviderPremisesDocuments { get; set; }
        public DbSet<CandidateProviderPremisesChecking> CandidateProviderPremisesCheckings { get; set; }
        public DbSet<CandidateProviderDocument> CandidateProviderDocuments { get; set; }
        public DbSet<CandidateProviderTrainerSpeciality> CandidateProviderTrainerSpecialities { get; set; }
        public DbSet<CandidateProviderPremisesSpeciality> CandidateProviderPremisesSpecialities { get; set; }
        public DbSet<CandidateProviderCPOStructureActivity> CandidateProviderCPOStructureActivities { get; set; }
        public DbSet<CandidateProviderCIPOStructureActivity> CandidateProviderCIPOStructureActivities { get; set; }
        public DbSet<CandidateProviderLicenceChange> CandidateProviderLicenceChanges { get; set; }

        public DbSet<ProcedurePrice> ProcedurePrices { get; set; }
        public DbSet<StartedProcedure> StartedProcedures { get; set; }
        public DbSet<StartedProcedureProgress> StartedProcedureProgresses { get; set; }
        public DbSet<ProcedureDocument> ProcedureDocuments { get; set; }
        public DbSet<ProcedureDocumentNotification> ProcedureDocumentNotifications { get; set; }
        public DbSet<ProcedureExternalExpert> ProcedureExternalExperts { get; set; }
        public DbSet<ProcedureExpertCommission> ProcedureExpertCommissions { get; set; }
        public DbSet<NegativeIssue> NegativeIssues { get; set; }
        public DbSet<ManagementDeadlineProcedure> ManagementDeadlineProcedures { get; set; }
        public DbSet<Payment> Payments { get; set; }

        







        #endregion

        #region Request
        public DbSet<NAPOORequestDoc> NAPOORequestDocs { get; set; } 
        public DbSet<ProviderRequestDocument> ProviderRequestDocuments { get; set; } 
        public DbSet<RequestDocumentStatus> RequestDocumentStatuses { get; set; } 
        public DbSet<TypeOfRequestedDocument> TypeOfRequestedDocuments { get; set; } 
        public DbSet<RequestDocumentType> RequestDocumentTypes { get; set; } 
        public DbSet<ProviderDocumentOffer> ProviderDocumentOffers { get; set; } 
        public DbSet<RequestDocumentManagement> RequestDocumentManagements { get; set; }
        public DbSet<RequestDocumentManagementUploadedFile> RequestDocumentManagementUploadedFiles { get; set; }
        public DbSet<RequestCandidateProviderDocument> RequestCandidateProviderDocuments { get; set; }
        public DbSet<DocumentSerialNumber> DocumentSerialNumbers { get; set; }
        public DbSet<DocumentSeries> DocumentSeries { get; set; }
        public DbSet<ReportUploadedDoc> ReportUploadedDocs { get; set; }
        public DbSet<RequestReport> RequestReports { get; set; }
        #endregion

        #region Training
        public DbSet<Program> Programs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCandidateCurriculumModification> CourseCandidateCurriculumModifications { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientCourse> ClientCourses { get; set; }
        public DbSet<CourseOrder> CourseOrders { get; set; }
        public DbSet<TrainerCourse> TrainerCourses { get; set; }
        public DbSet<PremisesCourse> PremisesCourses { get; set; }
        public DbSet<ClientCourseDocument> ClientCourseDocuments { get; set; } 
        public DbSet<CourseDocumentUploadedFile> CourseDocumentUploadedFiles { get; set; }
        public DbSet<TrainingCurriculum> TrainingCurriculums { get; set; }
        public DbSet<TrainingCurriculumERU> TrainingCurriculumERUs { get; set; }
        public DbSet<ClientRequiredDocument> ClientRequiredDocuments { get; set; }
        public DbSet<CourseSubject> CourseSubjects { get; set; }
        public DbSet<CourseSubjectGrade> CourseSubjectGrades { get; set; }
        public DbSet<CourseProtocol> CourseProtocols { get; set; }
        public DbSet<CourseProtocolGrade> CourseProtocolGrades { get; set; }
        public DbSet<ClientCourseStatus> ClientCourseStatuses { get; set; }
        public DbSet<Consulting> Consultings { get; set; }
        public DbSet<ConsultingClient> ConsultingClients { get; set; }
        public DbSet<ConsultingDocumentUploadedFile> ConsultingDocumentUploadedFiles { get; set; }
        public DbSet<ConsultingTrainer> ConsultingTrainers { get; set; }
        public DbSet<ConsultingPremises> ConsultingPremises { get; set; }
        public DbSet<ConsultingClientRequiredDocument> ConsultingClientRequiredDocuments { get; set; }
        public DbSet<CourseSchedule> CurriculumSchedules { get; set; }
        public DbSet<ValidationClient> ValidationClients { get; set; }
        public DbSet<ValidationClientDocument> ValidationClientDocuments { get; set; }
        public DbSet<ValidationProtocol> ValidationProtocols { get; set; }
        public DbSet<ValidationCommissionMember> ValidationCommissionMembers { get; set; }
        public DbSet<ValidationDocumentUploadedFile> ValidationDocumentUploadedFiles { get; set; }
        public DbSet<ValidationClientDocumentStatus> ValidationClientDocumentStatuses { get; set; }
        public DbSet<ValidationTrainer> ValidationTrainers { get; set; }
        public DbSet<ValidationPremises> ValidationPremises { get; set; }
        public DbSet<ValidationClientRequiredDocument> ValidationClientRequiredDocuments { get; set; }
        public DbSet<ValidationOrder> ValidationOrders { get; set; }
        public DbSet<ValidationProtocolGrade> ValidationProtocolGrades { get; set; }
        public DbSet<ClientCourseDocumentStatus> ClientCourseDocumentStatuses { get; set; }
        public DbSet<CourseChecking> CourseCheckings { get; set; }
        public DbSet<ValidationCompetency> ValidationCompetencies { get; set; }
        public DbSet<ValidationCurriculum> ValidationCurriculums { get; set; }
        public DbSet<ValidationClientCandidateCurriculumModification> ValidationClientCandidateCurriculumModifications { get; set; }
        public DbSet<ValidationCurriculumERU> ValidationCurriculumERUs { get; set; }
        public DbSet<ValidationClientChecking> ValidationClientCheckings { get; set; }

        #endregion

        #region Archive
        public DbSet<AnnualInfo> AnnualInfos { get; set; }
        public DbSet<AnnualInfoStatus> AnnualInfoStatuses { get; set; }
        public DbSet<AnnualReportNSI> AnnualReportNSIs { get; set; }
        public DbSet<RegiXLogRequest> RegiXLogRequests { get; set; }
        public DbSet<RegiXPersonResponse> RegiXPersonResponses { get; set; }
        public DbSet<SelfAssessmentReport> SelfAssessmentReports { get; set; }
        public DbSet<SelfAssessmentReportStatus> SelfAssessmentReportStatuses { get; set; }
        public DbSet<ArchCandidateProvider> ArchCandidateProviders { get; set; }
        public DbSet<ArchCandidateProviderTrainer> ArchCandidateProviderTrainers { get; set; }
        public DbSet<ArchCandidateProviderTrainerQualification> ArchCandidateProviderTrainerQualifications { get; set; }
        public DbSet<ArchCandidateProviderPremises> ArchCandidateProviderPremises { get; set; }
        public DbSet<ArchCandidateProviderPremisesSpeciality> ArchCandidateProviderPremisesSpecialities { get; set; }
        public DbSet<ArchCandidateProviderSpeciality> ArchCandidateProviderSpecialities { get; set; }
        public DbSet<ArchCandidateCurriculum> ArchCandidateCurriculums { get; set; }

        



        #endregion

        #region Follow-up control
        public DbSet<FollowUpControl> FollowUpControls { get; set; }

        public DbSet<FollowUpControlExpert> FollowUpControlExperts { get; set; }

        public DbSet<FollowUpControlDocument> FollowUpControlDocuments { get; set; }

        public DbSet<FollowUpControlDocumentNotification> FollowUpControlDocumentNotifications { get; set; }

        public DbSet<FollowUpControlUploadedFile> FollowUpControlUploadedFiles { get; set; }

        #endregion

        #region Assessment

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<UserAnswerOpen> UserAnswerOpens { get; set; }
        public DbSet<SurveyResult> SurveyResults { get; set; }


        #endregion

        #region Rating

        

        public DbSet<Indicator> Indicators { get; set; }
        
        public DbSet<CandidateProviderIndicator> CandidateProviderIndicators { get; set; }

        #endregion

        #region SqlView
        public DbSet<NAPOOSearch> NAPOOSearchs { get; set; }
        public DbSet<NAPOOSearchCipo> NAPOOSearchCipos { get; set; }
        public DbSet<NAPOOSearchCourses> NAPOOSearchCourses { get; set; }
        public DbSet<NAPOOStatistics> NAPOOStatistics { get; set; }
        public DbSet<AnnualTrainingCourse> AnnualTrainingCourses { get; set; }
        public DbSet<AnnualStudents> AnnualStudents { get; set; }
        public DbSet<AnnualStudentsByNationality> AnnualStudentsByNationality { get; set; }
        public DbSet<StudentDocumentVM> StudentDocuments { get; set; }
        #endregion
    }
}
