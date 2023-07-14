using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NapooMigration.Models
{
    public partial class napoo_jessieContext : DbContext
    {
        public napoo_jessieContext()
        {
        }

        public napoo_jessieContext(DbContextOptions<napoo_jessieContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccsAccesslog> AccsAccesslogs { get; set; } = null!;
        public virtual DbSet<AccsAdminlog> AccsAdminlogs { get; set; } = null!;
        public virtual DbSet<AccsBlockedip> AccsBlockedips { get; set; } = null!;
        public virtual DbSet<AccsFailure> AccsFailures { get; set; } = null!;
        public virtual DbSet<AccsMessage> AccsMessages { get; set; } = null!;
        public virtual DbSet<AccsWebuserdatum> AccsWebuserdata { get; set; } = null!;
        public virtual DbSet<AccsWebusergroup> AccsWebusergroups { get; set; } = null!;
        public virtual DbSet<ArchRefClientsCourse> ArchRefClientsCourses { get; set; } = null!;
        public virtual DbSet<ArchRefProviderPremisesSpeciality> ArchRefProviderPremisesSpecialities { get; set; } = null!;
        public virtual DbSet<ArchTbClient> ArchTbClients { get; set; } = null!;
        public virtual DbSet<ArchTbClientsCoursesDocument> ArchTbClientsCoursesDocuments { get; set; } = null!;
        public virtual DbSet<ArchTbCourse> ArchTbCourses { get; set; } = null!;
        public virtual DbSet<ArchTbCourseGroup> ArchTbCourseGroups { get; set; } = null!;
        public virtual DbSet<ArchTbCourseGroups40> ArchTbCourseGroups40s { get; set; } = null!;
        public virtual DbSet<ArchTbCourseGroupsDuplicate> ArchTbCourseGroupsDuplicates { get; set; } = null!;
        public virtual DbSet<ArchTbProvider> ArchTbProviders { get; set; } = null!;
        public virtual DbSet<ArchTbProviderPremise> ArchTbProviderPremises { get; set; } = null!;
        public virtual DbSet<ArchTbProviderSpecialitiesCurriculum> ArchTbProviderSpecialitiesCurricula { get; set; } = null!;
        public virtual DbSet<ArchTbProviderSpeciality> ArchTbProviderSpecialities { get; set; } = null!;
        public virtual DbSet<ArchTbTrainer> ArchTbTrainers { get; set; } = null!;
        public virtual DbSet<ArchTbTrainerQualification> ArchTbTrainerQualifications { get; set; } = null!;
        public virtual DbSet<CodeAssignType> CodeAssignTypes { get; set; } = null!;
        public virtual DbSet<CodeCandidateProvidersState> CodeCandidateProvidersStates { get; set; } = null!;
        public virtual DbSet<CodeCandidateType> CodeCandidateTypes { get; set; } = null!;
        public virtual DbSet<CodeCcontractType> CodeCcontractTypes { get; set; } = null!;
        public virtual DbSet<CodeCfinishedType> CodeCfinishedTypes { get; set; } = null!;
        public virtual DbSet<CodeCipoManagement> CodeCipoManagements { get; set; } = null!;
        public virtual DbSet<CodeCommMember> CodeCommMembers { get; set; } = null!;
        public virtual DbSet<CodeCommissionInstitutionType> CodeCommissionInstitutionTypes { get; set; } = null!;
        public virtual DbSet<CodeCorrection> CodeCorrections { get; set; } = null!;
        public virtual DbSet<CodeCourseEdForm> CodeCourseEdForms { get; set; } = null!;
        public virtual DbSet<CodeCourseFrCurr> CodeCourseFrCurrs { get; set; } = null!;
        public virtual DbSet<CodeCourseGroupRequiredDocumentsType> CodeCourseGroupRequiredDocumentsTypes { get; set; } = null!;
        public virtual DbSet<CodeCourseMeasureType> CodeCourseMeasureTypes { get; set; } = null!;
        public virtual DbSet<CodeCourseStatus> CodeCourseStatuses { get; set; } = null!;
        public virtual DbSet<CodeCourseType> CodeCourseTypes { get; set; } = null!;
        public virtual DbSet<CodeCpoManagement> CodeCpoManagements { get; set; } = null!;
        public virtual DbSet<CodeCurricHoursType> CodeCurricHoursTypes { get; set; } = null!;
        public virtual DbSet<CodeDocumentStatus> CodeDocumentStatuses { get; set; } = null!;
        public virtual DbSet<CodeDocumentStatusLock> CodeDocumentStatusLocks { get; set; } = null!;
        public virtual DbSet<CodeDocumentType> CodeDocumentTypes { get; set; } = null!;
        public virtual DbSet<CodeDocumentValidityCheck> CodeDocumentValidityChecks { get; set; } = null!;
        public virtual DbSet<CodeDocumentsManagement> CodeDocumentsManagements { get; set; } = null!;
        public virtual DbSet<CodeEducation> CodeEducations { get; set; } = null!;
        public virtual DbSet<CodeEgnType> CodeEgnTypes { get; set; } = null!;
        public virtual DbSet<CodeEkatte> CodeEkattes { get; set; } = null!;
        public virtual DbSet<CodeEkatteFull> CodeEkatteFulls { get; set; } = null!;
        public virtual DbSet<CodeEmploymentStatus> CodeEmploymentStatuses { get; set; } = null!;
        public virtual DbSet<CodeExpertType> CodeExpertTypes { get; set; } = null!;
        public virtual DbSet<CodeExtRegister> CodeExtRegisters { get; set; } = null!;
        public virtual DbSet<CodeGender> CodeGenders { get; set; } = null!;
        public virtual DbSet<CodeLicenceStatus> CodeLicenceStatuses { get; set; } = null!;
        public virtual DbSet<CodeLicenceStatusDetail> CodeLicenceStatusDetails { get; set; } = null!;
        public virtual DbSet<CodeMunicipality> CodeMunicipalities { get; set; } = null!;
        public virtual DbSet<CodeMunicipalityDetail> CodeMunicipalityDetails { get; set; } = null!;
        public virtual DbSet<CodeNationality> CodeNationalities { get; set; } = null!;
        public virtual DbSet<CodeNkpd> CodeNkpds { get; set; } = null!;
        public virtual DbSet<CodeObl> CodeObls { get; set; } = null!;
        public virtual DbSet<CodeOperation> CodeOperations { get; set; } = null!;
        public virtual DbSet<CodePremisesCorrespondence> CodePremisesCorrespondences { get; set; } = null!;
        public virtual DbSet<CodePremisesStatus> CodePremisesStatuses { get; set; } = null!;
        public virtual DbSet<CodePremisesType> CodePremisesTypes { get; set; } = null!;
        public virtual DbSet<CodePremisesUsage> CodePremisesUsages { get; set; } = null!;
        public virtual DbSet<CodeProcedure> CodeProcedures { get; set; } = null!;
        public virtual DbSet<CodeProcedureDecision> CodeProcedureDecisions { get; set; } = null!;
        public virtual DbSet<CodeProcedureStage> CodeProcedureStages { get; set; } = null!;
        public virtual DbSet<CodeProcedureStep> CodeProcedureSteps { get; set; } = null!;
        public virtual DbSet<CodeProceduresDocument> CodeProceduresDocuments { get; set; } = null!;
        public virtual DbSet<CodeProtokolType> CodeProtokolTypes { get; set; } = null!;
        public virtual DbSet<CodeProviderOwnership> CodeProviderOwnerships { get; set; } = null!;
        public virtual DbSet<CodeProviderRegistration> CodeProviderRegistrations { get; set; } = null!;
        public virtual DbSet<CodeProviderStatus> CodeProviderStatuses { get; set; } = null!;
        public virtual DbSet<CodeQualLevel> CodeQualLevels { get; set; } = null!;
        public virtual DbSet<CodeQualificationType> CodeQualificationTypes { get; set; } = null!;
        public virtual DbSet<CodeReceiveDocument> CodeReceiveDocuments { get; set; } = null!;
        public virtual DbSet<CodeReceiveType> CodeReceiveTypes { get; set; } = null!;
        public virtual DbSet<CodeRequestDocStatus> CodeRequestDocStatuses { get; set; } = null!;
        public virtual DbSet<CodeRequestDocType> CodeRequestDocTypes { get; set; } = null!;
        public virtual DbSet<CodeRequestDocsOperation> CodeRequestDocsOperations { get; set; } = null!;
        public virtual DbSet<CodeRequestDocsSeries> CodeRequestDocsSeries { get; set; } = null!;
        public virtual DbSet<CodeSpecialityCurriculumUpdateReason> CodeSpecialityCurriculumUpdateReasons { get; set; } = null!;
        public virtual DbSet<CodeSpecialityVq> CodeSpecialityVqs { get; set; } = null!;
        public virtual DbSet<CodeStageDocument> CodeStageDocuments { get; set; } = null!;
        public virtual DbSet<CodeStageDocumentType> CodeStageDocumentTypes { get; set; } = null!;
        public virtual DbSet<CodeTcontractType> CodeTcontractTypes { get; set; } = null!;
        public virtual DbSet<CodeTqualificationType> CodeTqualificationTypes { get; set; } = null!;
        public virtual DbSet<CodeTrainingType> CodeTrainingTypes { get; set; } = null!;
        public virtual DbSet<CodeUiControlType> CodeUiControlTypes { get; set; } = null!;
        public virtual DbSet<CodeUploadDocStatus> CodeUploadDocStatuses { get; set; } = null!;
        public virtual DbSet<CodeUploadDocType> CodeUploadDocTypes { get; set; } = null!;
        public virtual DbSet<CodeValidityCheckTarget> CodeValidityCheckTargets { get; set; } = null!;
        public virtual DbSet<CodeVetArea> CodeVetAreas { get; set; } = null!;
        public virtual DbSet<CodeVetGroup> CodeVetGroups { get; set; } = null!;
        public virtual DbSet<CodeVetList> CodeVetLists { get; set; } = null!;
        public virtual DbSet<CodeVetProfession> CodeVetProfessions { get; set; } = null!;
        public virtual DbSet<CodeVetSpeciality> CodeVetSpecialities { get; set; } = null!;
        public virtual DbSet<CodeVillageType> CodeVillageTypes { get; set; } = null!;
        public virtual DbSet<CodeVisitResult> CodeVisitResults { get; set; } = null!;
        public virtual DbSet<CodeWgdoiFunction> CodeWgdoiFunctions { get; set; } = null!;
        public virtual DbSet<IscsIisclientdatum> IscsIisclientdata { get; set; } = null!;
        public virtual DbSet<IscsIisserverdatum> IscsIisserverdata { get; set; } = null!;
        public virtual DbSet<MvRegisterClient> MvRegisterClients { get; set; } = null!;
        public virtual DbSet<RefArchExpert> RefArchExperts { get; set; } = null!;
        public virtual DbSet<RefArchExpertCommission> RefArchExpertCommissions { get; set; } = null!;
        public virtual DbSet<RefArchProcedureExpert> RefArchProcedureExperts { get; set; } = null!;
        public virtual DbSet<RefArchProcedureExpertCommission> RefArchProcedureExpertCommissions { get; set; } = null!;
        public virtual DbSet<RefArchProcedureProceduresDocument> RefArchProcedureProceduresDocuments { get; set; } = null!;
        public virtual DbSet<RefArchProcedureProviderPremisesSpeciality> RefArchProcedureProviderPremisesSpecialities { get; set; } = null!;
        public virtual DbSet<RefArchProceduresDocument> RefArchProceduresDocuments { get; set; } = null!;
        public virtual DbSet<RefCandidateProviderPremisesSpeciality> RefCandidateProviderPremisesSpecialities { get; set; } = null!;
        public virtual DbSet<RefCandidatesExpert> RefCandidatesExperts { get; set; } = null!;
        public virtual DbSet<RefCandidatesExpertCommission> RefCandidatesExpertCommissions { get; set; } = null!;
        public virtual DbSet<RefCandidatesProceduresDocument> RefCandidatesProceduresDocuments { get; set; } = null!;
        public virtual DbSet<RefCgCurricFile> RefCgCurricFiles { get; set; } = null!;
        public virtual DbSet<RefClientsCourse> RefClientsCourses { get; set; } = null!;
        public virtual DbSet<RefClientsCoursesDocumentsStatus> RefClientsCoursesDocumentsStatuses { get; set; } = null!;
        public virtual DbSet<RefCourseDocumentType> RefCourseDocumentTypes { get; set; } = null!;
        public virtual DbSet<RefCourseGroupPremise> RefCourseGroupPremises { get; set; } = null!;
        public virtual DbSet<RefCourseTypeFrCurr> RefCourseTypeFrCurrs { get; set; } = null!;
        public virtual DbSet<RefDoctypeReqdoctype> RefDoctypeReqdoctypes { get; set; } = null!;
        public virtual DbSet<RefDocumentStatusLock> RefDocumentStatusLocks { get; set; } = null!;
        public virtual DbSet<RefExpertsCommission> RefExpertsCommissions { get; set; } = null!;
        public virtual DbSet<RefExpertsDoi> RefExpertsDois { get; set; } = null!;
        public virtual DbSet<RefExpertsType> RefExpertsTypes { get; set; } = null!;
        public virtual DbSet<RefExpertsVetArea> RefExpertsVetAreas { get; set; } = null!;
        public virtual DbSet<RefFrCurrEdForm> RefFrCurrEdForms { get; set; } = null!;
        public virtual DbSet<RefFrCurrEducLevel> RefFrCurrEducLevels { get; set; } = null!;
        public virtual DbSet<RefFrCurrQualLevel> RefFrCurrQualLevels { get; set; } = null!;
        public virtual DbSet<RefMainExpert> RefMainExperts { get; set; } = null!;
        public virtual DbSet<RefMainExpertCommission> RefMainExpertCommissions { get; set; } = null!;
        public virtual DbSet<RefProcedureStep> RefProcedureSteps { get; set; } = null!;
        public virtual DbSet<RefProcedureStepStage> RefProcedureStepStages { get; set; } = null!;
        public virtual DbSet<RefProceduresDocument> RefProceduresDocuments { get; set; } = null!;
        public virtual DbSet<RefProviderPremisesSpeciality> RefProviderPremisesSpecialities { get; set; } = null!;
        public virtual DbSet<RefRequestDocStatus> RefRequestDocStatuses { get; set; } = null!;
        public virtual DbSet<RefRequestDocType> RefRequestDocTypes { get; set; } = null!;
        public virtual DbSet<RefRoleAclAction> RefRoleAclActions { get; set; } = null!;
        public virtual DbSet<RefRoleGroup> RefRoleGroups { get; set; } = null!;
        public virtual DbSet<RefRoleUser> RefRoleUsers { get; set; } = null!;
        public virtual DbSet<RefTrainersCourse> RefTrainersCourses { get; set; } = null!;
        public virtual DbSet<RefVetSpecialitiesNkpd> RefVetSpecialitiesNkpds { get; set; } = null!;
        public virtual DbSet<RefVisit> RefVisits { get; set; } = null!;
        public virtual DbSet<RefVisitsExpert> RefVisitsExperts { get; set; } = null!;
        public virtual DbSet<ReportClient> ReportClients { get; set; } = null!;
        public virtual DbSet<ReportCourse> ReportCourses { get; set; } = null!;
        public virtual DbSet<ReportCurriculum> ReportCurricula { get; set; } = null!;
        public virtual DbSet<ReportPremise> ReportPremises { get; set; } = null!;
        public virtual DbSet<ReportProvider> ReportProviders { get; set; } = null!;
        public virtual DbSet<ReportTrainersQualification> ReportTrainersQualifications { get; set; } = null!;
        public virtual DbSet<Ret> Rets { get; set; } = null!;
        public virtual DbSet<SysAcl> SysAcls { get; set; } = null!;
        public virtual DbSet<SysLock> SysLocks { get; set; } = null!;
        public virtual DbSet<SysMailLog> SysMailLogs { get; set; } = null!;
        public virtual DbSet<SysOperationLog> SysOperationLogs { get; set; } = null!;
        public virtual DbSet<SysSignLog> SysSignLogs { get; set; } = null!;
        public virtual DbSet<Systransliterate> Systransliterates { get; set; } = null!;
        public virtual DbSet<TbAclAction> TbAclActions { get; set; } = null!;
        public virtual DbSet<TbAnnualInfo> TbAnnualInfos { get; set; } = null!;
        public virtual DbSet<TbArchProcedureProvider> TbArchProcedureProviders { get; set; } = null!;
        public virtual DbSet<TbArchProcedureProviderPremise> TbArchProcedureProviderPremises { get; set; } = null!;
        public virtual DbSet<TbArchProcedureProviderPremisesRoom> TbArchProcedureProviderPremisesRooms { get; set; } = null!;
        public virtual DbSet<TbArchProcedureProviderSpecialitiesCurriculum> TbArchProcedureProviderSpecialitiesCurricula { get; set; } = null!;
        public virtual DbSet<TbArchProcedureProvidersCipoManagement> TbArchProcedureProvidersCipoManagements { get; set; } = null!;
        public virtual DbSet<TbArchProcedureProvidersCpoManagement> TbArchProcedureProvidersCpoManagements { get; set; } = null!;
        public virtual DbSet<TbArchProcedureProvidersDocumentsManagement> TbArchProcedureProvidersDocumentsManagements { get; set; } = null!;
        public virtual DbSet<TbArchProcedureProvidersSpeciality> TbArchProcedureProvidersSpecialities { get; set; } = null!;
        public virtual DbSet<TbArchProcedureTrainer> TbArchProcedureTrainers { get; set; } = null!;
        public virtual DbSet<TbArchProcedureTrainerDocument> TbArchProcedureTrainerDocuments { get; set; } = null!;
        public virtual DbSet<TbArchProcedureTrainerProfile> TbArchProcedureTrainerProfiles { get; set; } = null!;
        public virtual DbSet<TbArchProcedureTrainerQualification> TbArchProcedureTrainerQualifications { get; set; } = null!;
        public virtual DbSet<TbArchProvider> TbArchProviders { get; set; } = null!;
        public virtual DbSet<TbCandidateProvider> TbCandidateProviders { get; set; } = null!;
        public virtual DbSet<TbCandidateProviderPremise> TbCandidateProviderPremises { get; set; } = null!;
        public virtual DbSet<TbCandidateProviderPremisesRoom> TbCandidateProviderPremisesRooms { get; set; } = null!;
        public virtual DbSet<TbCandidateProviderSpecialitiesCurriculum> TbCandidateProviderSpecialitiesCurricula { get; set; } = null!;
        public virtual DbSet<TbCandidateProvidersCipoManagement> TbCandidateProvidersCipoManagements { get; set; } = null!;
        public virtual DbSet<TbCandidateProvidersCpoManagement> TbCandidateProvidersCpoManagements { get; set; } = null!;
        public virtual DbSet<TbCandidateProvidersDocumentsManagement> TbCandidateProvidersDocumentsManagements { get; set; } = null!;
        public virtual DbSet<TbCandidateProvidersSpeciality> TbCandidateProvidersSpecialities { get; set; } = null!;
        public virtual DbSet<TbCandidateProvidersState> TbCandidateProvidersStates { get; set; } = null!;
        public virtual DbSet<TbCandidateTrainer> TbCandidateTrainers { get; set; } = null!;
        public virtual DbSet<TbCandidateTrainerDocument> TbCandidateTrainerDocuments { get; set; } = null!;
        public virtual DbSet<TbCandidateTrainerProfile> TbCandidateTrainerProfiles { get; set; } = null!;
        public virtual DbSet<TbCandidateTrainerQualification> TbCandidateTrainerQualifications { get; set; } = null!;
        public virtual DbSet<TbClient> TbClients { get; set; } = null!;
        public virtual DbSet<TbClientsCoursesDocument> TbClientsCoursesDocuments { get; set; } = null!;
        public virtual DbSet<TbClientsRequiredDocument> TbClientsRequiredDocuments { get; set; } = null!;
        public virtual DbSet<TbCourse> TbCourses { get; set; } = null!;
        public virtual DbSet<TbCourse40Competence> TbCourse40Competences { get; set; } = null!;
        public virtual DbSet<TbCourseGroup> TbCourseGroups { get; set; } = null!;
        public virtual DbSet<TbCourseGroups40> TbCourseGroups40s { get; set; } = null!;
        public virtual DbSet<TbCourseGroupsDuplicate> TbCourseGroupsDuplicates { get; set; } = null!;
        public virtual DbSet<TbCurricModule> TbCurricModules { get; set; } = null!;
        public virtual DbSet<TbDoi> TbDois { get; set; } = null!;
        public virtual DbSet<TbDoiCommission> TbDoiCommissions { get; set; } = null!;
        public virtual DbSet<TbESigner> TbESigners { get; set; } = null!;
        public virtual DbSet<TbExpert> TbExperts { get; set; } = null!;
        public virtual DbSet<TbExpertCommission> TbExpertCommissions { get; set; } = null!;
        public virtual DbSet<TbGeneratedDocument> TbGeneratedDocuments { get; set; } = null!;
        public virtual DbSet<TbGroup> TbGroups { get; set; } = null!;
        public virtual DbSet<TbImportXml> TbImportXmls { get; set; } = null!;
        public virtual DbSet<TbNapooRequestDoc> TbNapooRequestDocs { get; set; } = null!;
        public virtual DbSet<TbPayment> TbPayments { get; set; } = null!;
        public virtual DbSet<TbProcedureDocument> TbProcedureDocuments { get; set; } = null!;
        public virtual DbSet<TbProcedurePrice> TbProcedurePrices { get; set; } = null!;
        public virtual DbSet<TbProcedureProgress> TbProcedureProgresses { get; set; } = null!;
        public virtual DbSet<TbProcedureProgressDocument> TbProcedureProgressDocuments { get; set; } = null!;
        public virtual DbSet<TbProvider> TbProviders { get; set; } = null!;
        public virtual DbSet<TbProviderActivity> TbProviderActivities { get; set; } = null!;
        public virtual DbSet<TbProviderPremise> TbProviderPremises { get; set; } = null!;
        public virtual DbSet<TbProviderPremisesRoom> TbProviderPremisesRooms { get; set; } = null!;
        public virtual DbSet<TbProviderSpecialitiesCurriculum> TbProviderSpecialitiesCurricula { get; set; } = null!;
        public virtual DbSet<TbProviderSpeciality> TbProviderSpecialities { get; set; } = null!;
        public virtual DbSet<TbProviderUploadedDoc> TbProviderUploadedDocs { get; set; } = null!;
        public virtual DbSet<TbProvidersCipoManagement> TbProvidersCipoManagements { get; set; } = null!;
        public virtual DbSet<TbProvidersCpoManagement> TbProvidersCpoManagements { get; set; } = null!;
        public virtual DbSet<TbProvidersDocsDashboard> TbProvidersDocsDashboards { get; set; } = null!;
        public virtual DbSet<TbProvidersDocsOffer> TbProvidersDocsOffers { get; set; } = null!;
        public virtual DbSet<TbProvidersDocumentsManagement> TbProvidersDocumentsManagements { get; set; } = null!;
        public virtual DbSet<TbProvidersLicenceChange> TbProvidersLicenceChanges { get; set; } = null!;
        public virtual DbSet<TbProvidersRequestDoc> TbProvidersRequestDocs { get; set; } = null!;
        public virtual DbSet<TbRequestDocsManagement> TbRequestDocsManagements { get; set; } = null!;
        public virtual DbSet<TbRequestDocsSn> TbRequestDocsSns { get; set; } = null!;
        public virtual DbSet<TbRole> TbRoles { get; set; } = null!;
        public virtual DbSet<TbSignContent> TbSignContents { get; set; } = null!;
        public virtual DbSet<TbStartedProcedure> TbStartedProcedures { get; set; } = null!;
        public virtual DbSet<TbStartedProcedureProgress> TbStartedProcedureProgresses { get; set; } = null!;
        public virtual DbSet<TbSurveyAnswer> TbSurveyAnswers { get; set; } = null!;
        public virtual DbSet<TbSurveyQuestion> TbSurveyQuestions { get; set; } = null!;
        public virtual DbSet<TbTrainer> TbTrainers { get; set; } = null!;
        public virtual DbSet<TbTrainerDocument> TbTrainerDocuments { get; set; } = null!;
        public virtual DbSet<TbTrainerProfile> TbTrainerProfiles { get; set; } = null!;
        public virtual DbSet<TbTrainerQualification> TbTrainerQualifications { get; set; } = null!;
        public virtual DbSet<TbUser> TbUsers { get; set; } = null!;
        public virtual DbSet<TbUserPress> TbUserPresses { get; set; } = null!;
        public virtual DbSet<TbVersion> TbVersions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=HV-DEVSERVER-03.smcon.com\\\\MSSQLSERVER2017; Database=nappo_jessie; User Id=napoo; Password=n@p00; MultipleActiveResultSets=true;",options=>options.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccsAccesslog>(entity =>
            {
                entity.HasKey(e => e.Sessionid)
                    .HasName("accs_accesslog_pkey")
                    .IsClustered(false);

                entity.ToTable("accs_accesslog");

                entity.Property(e => e.Sessionid)
                    .ValueGeneratedNever()
                    .HasColumnName("sessionid");

                entity.Property(e => e.Ggid).HasColumnName("ggid");

                entity.Property(e => e.Ip).HasColumnName("ip");

                entity.Property(e => e.Lgid).HasColumnName("lgid");

                entity.Property(e => e.Timeend).HasColumnName("timeend");

                entity.Property(e => e.Timestart).HasColumnName("timestart");

                entity.Property(e => e.Timeused).HasColumnName("timeused");

                entity.Property(e => e.Userid).HasColumnName("userid");
            });

            modelBuilder.Entity<AccsAdminlog>(entity =>
            {
                entity.HasKey(e => e.AlId)
                    .HasName("accs_adminlog_pkey")
                    .IsClustered(false);

                entity.ToTable("accs_adminlog");

                entity.Property(e => e.AlId).HasColumnName("al_id");

                entity.Property(e => e.Action).HasColumnName("action");

                entity.Property(e => e.Adata).HasColumnName("adata");

                entity.Property(e => e.Sessionid).HasColumnName("sessionid");

                entity.Property(e => e.T).HasColumnName("t");

                entity.Property(e => e.TUserid).HasColumnName("t_userid");
            });

            modelBuilder.Entity<AccsBlockedip>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("accs_blockedip");

                entity.Property(e => e.Ip).HasColumnName("ip");

                entity.Property(e => e.Lt).HasColumnName("lt");

                entity.Property(e => e.Nt).HasColumnName("nt");

                entity.Property(e => e.Sf).HasColumnName("sf");

                entity.Property(e => e.Tf).HasColumnName("tf");

                entity.Property(e => e.Ts).HasColumnName("ts");

                entity.Property(e => e.Un).HasColumnName("un");
            });

            modelBuilder.Entity<AccsFailure>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("accs_failures");

                entity.Property(e => e.Errorcode).HasColumnName("errorcode");

                entity.Property(e => e.Ip).HasColumnName("ip");

                entity.Property(e => e.T).HasColumnName("t");

                entity.Property(e => e.Uname).HasColumnName("uname");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<AccsMessage>(entity =>
            {
                entity.HasKey(e => e.Mask)
                    .HasName("accs_messages_pkey")
                    .IsClustered(false);

                entity.ToTable("accs_messages");

                entity.Property(e => e.Mask)
                    .ValueGeneratedNever()
                    .HasColumnName("mask");

                entity.Property(e => e.Msg).HasColumnName("msg");
            });

            modelBuilder.Entity<AccsWebuserdatum>(entity =>
            {
                entity.HasKey(e => e.Sessiondata)
                    .HasName("accs_webuserdata_pkey")
                    .IsClustered(false);

                entity.ToTable("accs_webuserdata");

                entity.Property(e => e.Sessiondata)
                    .HasMaxLength(45)
                    .HasColumnName("sessiondata");

                entity.Property(e => e.Adat).HasColumnName("adat");

                entity.Property(e => e.Firstlogin).HasColumnName("firstlogin");

                entity.Property(e => e.Lastlogin).HasColumnName("lastlogin");

                entity.Property(e => e.Nsec).HasColumnName("nsec");

                entity.Property(e => e.Udat).HasColumnName("udat");
            });

            modelBuilder.Entity<AccsWebusergroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("accs_webusergroups");

                entity.Property(e => e.GroupId).HasColumnName("group_id");

                entity.Property(e => e.GroupName)
                    .HasMaxLength(50)
                    .HasColumnName("group_name");

                entity.Property(e => e.Permissions)
                    .HasColumnName("permissions")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ArchRefClientsCourse>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_ref_clients_courses_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_ref_clients_courses");

                entity.HasIndex(e => e.IntCourseGroupId, "arch_int_course_group_id_int_year");

                entity.HasIndex(e => new { e.Id, e.IntYear }, "id_int_year");

                entity.HasIndex(e => new { e.IntClientId, e.IntYear }, "int_client_id_int_year_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.DtClientBirthDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_client_birth_date");

                entity.Property(e => e.DtCourseFinished)
                    .HasColumnType("date")
                    .HasColumnName("dt_course_finished");

                entity.Property(e => e.IntAssignTypeId).HasColumnName("int_assign_type_id");

                entity.Property(e => e.IntCfinishedTypeId).HasColumnName("int_cfinished_type_id");

                entity.Property(e => e.IntClientGender).HasColumnName("int_client_gender");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntEducationId).HasColumnName("int_education_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntQualLevel).HasColumnName("int_qual_level");

                entity.Property(e => e.IntQualVetArea).HasColumnName("int_qual_vet_area");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");

                entity.Property(e => e.VcFamilyName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_family_name");

                entity.Property(e => e.VcFirstName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_first_name");

                entity.Property(e => e.VcSecondName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_second_name");
            });

            modelBuilder.Entity<ArchRefProviderPremisesSpeciality>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_ref_provider_premises_specialities_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_ref_provider_premises_specialities");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.IntProviderPremiseId).HasColumnName("int_provider_premise_id");

                entity.Property(e => e.IntProviderPremiseSpecialityCorrespondence).HasColumnName("int_provider_premise_speciality_correspondence");

                entity.Property(e => e.IntProviderPremiseSpecialityUsage).HasColumnName("int_provider_premise_speciality_usage");

                entity.Property(e => e.IntProviderSpecialityId).HasColumnName("int_provider_speciality_id");
            });

            modelBuilder.Entity<ArchTbClient>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_clients_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_clients");

                entity.HasIndex(e => new { e.IntProviderId, e.VcEgn, e.IntYear }, "arch_clients_int_provider_egn_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL AND [int_year] IS NOT NULL)");

                entity.HasIndex(e => new { e.Id, e.IntYear }, "id_int_year_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.DtClientBirthDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_client_birth_date");

                entity.Property(e => e.IntClientGender).HasColumnName("int_client_gender");

                entity.Property(e => e.IntEducationId).HasColumnName("int_education_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.VcClientFamilyName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_client_family_name");

                entity.Property(e => e.VcClientFirstName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_client_first_name");

                entity.Property(e => e.VcClientSecondName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_client_second_name");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");
            });

            modelBuilder.Entity<ArchTbClientsCoursesDocument>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_clients_courses_documents_id")
                    .IsClustered(false);

                entity.ToTable("arch_tb_clients_courses_documents");

                entity.HasIndex(e => new { e.IntClientsCoursesId, e.IntYear }, "int_client_courses_id_int_year_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.Document1File).HasColumnName("document_1_file");

                entity.Property(e => e.Document2File).HasColumnName("document_2_file");

                entity.Property(e => e.DtDocumentDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_document_date");

                entity.Property(e => e.IntClientsCoursesId).HasColumnName("int_clients_courses_id");

                entity.Property(e => e.IntCourseFinishedYear).HasColumnName("int_course_finished_year");

                entity.Property(e => e.IntDocumentStatus).HasColumnName("int_document_status");

                entity.Property(e => e.IntDocumentTypeId).HasColumnName("int_document_type_id");

                entity.Property(e => e.NumPracticeResult)
                    .HasColumnType("numeric(3, 2)")
                    .HasColumnName("num_practice_result");

                entity.Property(e => e.NumTheoryResult)
                    .HasColumnType("numeric(3, 2)")
                    .HasColumnName("num_theory_result");

                entity.Property(e => e.VcDocumentPrnNo)
                    .HasMaxLength(50)
                    .HasColumnName("vc_document_prn_no");

                entity.Property(e => e.VcDocumentPrnSer)
                    .HasMaxLength(50)
                    .HasColumnName("vc_document_prn_ser");

                entity.Property(e => e.VcDocumentProt)
                    .HasMaxLength(50)
                    .HasColumnName("vc_document_prot");

                entity.Property(e => e.VcDocumentRegNo)
                    .HasMaxLength(50)
                    .HasColumnName("vc_document_reg_no");

                entity.Property(e => e.VcQualificatiojLevel)
                    .HasMaxLength(50)
                    .HasColumnName("vc_qualificatioj_level");

                entity.Property(e => e.VcQualificationName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_qualification_name");
            });

            modelBuilder.Entity<ArchTbCourse>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_courses_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_courses");

                entity.HasIndex(e => e.IntYear, "i_arch_tb_courses_int_year");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.IntCourseEducRequirement).HasColumnName("int_course_educ_requirement");

                entity.Property(e => e.IntCourseFrCurrId).HasColumnName("int_course_fr_curr_id");

                entity.Property(e => e.IntCourseNo).HasColumnName("int_course_no");

                entity.Property(e => e.IntCourseTypeId).HasColumnName("int_course_type_id");

                entity.Property(e => e.IntMandatoryHours).HasColumnName("int_mandatory_hours");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntSelectableHours).HasColumnName("int_selectable_hours");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.VcCourseAddNotes).HasColumnName("vc_course_add_notes");

                entity.Property(e => e.VcCourseName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_course_name");
            });

            modelBuilder.Entity<ArchTbCourseGroup>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_course_groups_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_course_groups");

                entity.HasIndex(e => new { e.Id, e.IntYear }, "arch_tb_course_groups_id_int_year_idx");

                entity.HasIndex(e => e.IntCourseId, "int_course_id_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.DtCourseSubscribeDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_course_subscribe_date");

                entity.Property(e => e.DtEndDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_end_date");

                entity.Property(e => e.DtExamPracticeDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_exam_practice_date");

                entity.Property(e => e.DtExamTheoryDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_exam_theory_date");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntAssignTypeId).HasColumnName("int_assign_type_id");

                entity.Property(e => e.IntCourseDuration).HasColumnName("int_course_duration");

                entity.Property(e => e.IntCourseEdFormId).HasColumnName("int_course_ed_form_id");

                entity.Property(e => e.IntCourseFrCurrId).HasColumnName("int_course_fr_curr_id");

                entity.Property(e => e.IntCourseId).HasColumnName("int_course_id");

                entity.Property(e => e.IntCourseMeasureTypeId).HasColumnName("int_course_measure_type_id");

                entity.Property(e => e.IntCourseStatusId).HasColumnName("int_course_status_id");

                entity.Property(e => e.IntCourseTypeId).HasColumnName("int_course_type_id");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntMandatoryHours).HasColumnName("int_mandatory_hours");

                entity.Property(e => e.IntPDisabilityCount).HasColumnName("int_p_disability_count");

                entity.Property(e => e.IntProviderPremise).HasColumnName("int_provider_premise");

                entity.Property(e => e.IntSelectableHours).HasColumnName("int_selectable_hours");

                entity.Property(e => e.NumCourseCost)
                    .HasColumnType("numeric(10, 2)")
                    .HasColumnName("num_course_cost");

                entity.Property(e => e.VcAdditionalNotes).HasColumnName("vc_additional_notes");

                entity.Property(e => e.VcCourseAssignType)
                    .HasMaxLength(255)
                    .HasColumnName("vc_course_assign_type");

                entity.Property(e => e.VcCourseGroupName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_course_group_name");

                entity.Property(e => e.VcCourseNotes).HasColumnName("vc_course_notes");

                entity.Property(e => e.VcExamCommMembers).HasColumnName("vc_exam_comm_members");
            });

            modelBuilder.Entity<ArchTbCourseGroups40>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_tb_course_groups_40_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_course_groups_40");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.DtEndDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_end_date");

                entity.Property(e => e.DtExamPracticeDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_exam_practice_date");

                entity.Property(e => e.DtExamTheoryDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_exam_theory_date");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntAssignTypeId).HasColumnName("int_assign_type_id");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCourseDuration).HasColumnName("int_course_duration");

                entity.Property(e => e.IntCourseEdFormId).HasColumnName("int_course_ed_form_id");

                entity.Property(e => e.IntCourseFrCurrId).HasColumnName("int_course_fr_curr_id");

                entity.Property(e => e.IntCourseTypeId).HasColumnName("int_course_type_id");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntMandatoryHours).HasColumnName("int_mandatory_hours");

                entity.Property(e => e.IntProviderPremise).HasColumnName("int_provider_premise");

                entity.Property(e => e.IntSelectableHours).HasColumnName("int_selectable_hours");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.NumCourseCost)
                    .HasColumnType("numeric(10, 2)")
                    .HasColumnName("num_course_cost");

                entity.Property(e => e.VcAdditionalNotes).HasColumnName("vc_additional_notes");

                entity.Property(e => e.VcExamCommMembers).HasColumnName("vc_exam_comm_members");
            });

            modelBuilder.Entity<ArchTbCourseGroupsDuplicate>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_tb_course_groups_duplicates_pk")
                    .IsClustered(false);

                entity.ToTable("arch_tb_course_groups_duplicates");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.DtEndDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_end_date");

                entity.Property(e => e.DtOriginalDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_original_date");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCodeDocumentTypeId).HasColumnName("int_code_document_type_id");

                entity.Property(e => e.IntCourseFinishedYear).HasColumnName("int_course_finished_year");

                entity.Property(e => e.IntOriginalDocumentId).HasColumnName("int_original_document_id");

                entity.Property(e => e.IntOriginalRefClientId).HasColumnName("int_original_ref_client_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.VcOriginalPrnNo)
                    .HasMaxLength(50)
                    .HasColumnName("vc_original_prn_no");

                entity.Property(e => e.VcOriginalRegNo)
                    .HasMaxLength(50)
                    .HasColumnName("vc_original_reg_no");
            });

            modelBuilder.Entity<ArchTbProvider>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_tb_providers_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_providers");

                entity.HasIndex(e => new { e.IntProviderBulstat, e.IntYear }, "arch_tb_providers_int_provider_bulstat_key")
                    .IsUnique()
                    .HasFilter("([int_provider_bulstat] IS NOT NULL AND [int_year] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.BoolIsBrra)
                    .HasColumnName("bool_is_brra")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtLicenceData)
                    .HasColumnType("date")
                    .HasColumnName("dt_licence_data");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntLicenceNumber).HasColumnName("int_licence_number");

                entity.Property(e => e.IntLicenceStatusId).HasColumnName("int_licence_status_id");

                entity.Property(e => e.IntLocalGroupId).HasColumnName("int_local_group_id");

                entity.Property(e => e.IntProviderBulstat)
                    .HasMaxLength(20)
                    .HasColumnName("int_provider_bulstat");

                entity.Property(e => e.IntProviderContactPersEkatteId).HasColumnName("int_provider_contact_pers_ekatte_id");

                entity.Property(e => e.IntProviderOwnershipId).HasColumnName("int_provider_ownership_id");

                entity.Property(e => e.IntProviderRegistrationId).HasColumnName("int_provider_registration_id");

                entity.Property(e => e.IntProviderStatusId).HasColumnName("int_provider_status_id");

                entity.Property(e => e.VcProviderAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_address");

                entity.Property(e => e.VcProviderContactPers)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers");

                entity.Property(e => e.VcProviderContactPersAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_address");

                entity.Property(e => e.VcProviderContactPersEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_email");

                entity.Property(e => e.VcProviderContactPersFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_fax");

                entity.Property(e => e.VcProviderContactPersPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone1");

                entity.Property(e => e.VcProviderContactPersPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone2");

                entity.Property(e => e.VcProviderContactPersZipcode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_provider_contact_pers_zipcode");

                entity.Property(e => e.VcProviderEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_email");

                entity.Property(e => e.VcProviderFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_fax");

                entity.Property(e => e.VcProviderManager)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_manager");

                entity.Property(e => e.VcProviderName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_name");

                entity.Property(e => e.VcProviderOwner)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_owner");

                entity.Property(e => e.VcProviderPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone1");

                entity.Property(e => e.VcProviderPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone2");

                entity.Property(e => e.VcProviderProfile).HasColumnName("vc_provider_profile");

                entity.Property(e => e.VcProviderWeb)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_web");

                entity.Property(e => e.VcZipCode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_zip_code");
            });

            modelBuilder.Entity<ArchTbProviderPremise>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_tb_provider_premises_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_provider_premises");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.BoolIsVisited).HasColumnName("bool_is_visited");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntProviderPremiseEkatte).HasColumnName("int_provider_premise_ekatte");

                entity.Property(e => e.IntProviderPremiseNo).HasColumnName("int_provider_premise_no");

                entity.Property(e => e.IntProviderPremiseStatus).HasColumnName("int_provider_premise_status");

                entity.Property(e => e.TxtProviderPremiseAddress).HasColumnName("txt_provider_premise_address");

                entity.Property(e => e.TxtProviderPremiseName).HasColumnName("txt_provider_premise_name");

                entity.Property(e => e.TxtProviderPremiseNotes).HasColumnName("txt_provider_premise_notes");
            });

            modelBuilder.Entity<ArchTbProviderSpecialitiesCurriculum>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_tb_provider_specialities_curriculum_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_provider_specialities_curriculum");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.BoolIsUpdated)
                    .HasColumnName("bool_is_updated")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtUpdateDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_update_date");

                entity.Property(e => e.IntProviderSpecialityId).HasColumnName("int_provider_speciality_id");

                entity.Property(e => e.IntSpecialityCurriculumUpdateReasonId).HasColumnName("int_speciality_curriculum_update_reason_id");

                entity.Property(e => e.VcFileName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_file_name");
            });

            modelBuilder.Entity<ArchTbProviderSpeciality>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_tb_provider_specialities_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_provider_specialities");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.BoolIsAdded)
                    .HasColumnName("bool_is_added")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BoolIsValid)
                    .HasColumnName("bool_is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DtLicenceData)
                    .HasColumnType("date")
                    .HasColumnName("dt_licence_data");

                entity.Property(e => e.IntLicenceProtNo)
                    .HasMaxLength(255)
                    .HasColumnName("int_licence_prot_no");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.TxtSpecialityNotes).HasColumnName("txt_speciality_notes");
            });

            modelBuilder.Entity<ArchTbTrainer>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_tb_trainers_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_trainers");

                entity.HasIndex(e => new { e.IntProviderId, e.VcEgn, e.IntYear }, "arch_tb_trainers_int_provider_egn_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL AND [int_year] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.BoolIsAndragog)
                    .HasColumnName("bool_is_andragog")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DtTcontractDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_tcontract_date");

                entity.Property(e => e.IntBirthYear).HasColumnName("int_birth_year");

                entity.Property(e => e.IntEducationId).HasColumnName("int_education_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntGenderId).HasColumnName("int_gender_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntTcontractTypeId).HasColumnName("int_tcontract_type_id");

                entity.Property(e => e.TxtEducationAcademicNotes).HasColumnName("txt_education_academic_notes");

                entity.Property(e => e.TxtEducationCertificateNotes).HasColumnName("txt_education_certificate_notes");

                entity.Property(e => e.TxtEducationSpecialityNotes).HasColumnName("txt_education_speciality_notes");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");

                entity.Property(e => e.VcEmail)
                    .HasMaxLength(50)
                    .HasColumnName("vc_email");

                entity.Property(e => e.VcTrainerFamilyName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_trainer_family_name");

                entity.Property(e => e.VcTrainerFirstName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_trainer_first_name");

                entity.Property(e => e.VcTrainerSecondName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_trainer_second_name");
            });

            modelBuilder.Entity<ArchTbTrainerQualification>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntYear })
                    .HasName("arch_tb_trainer_qualifications_pkey")
                    .IsClustered(false);

                entity.ToTable("arch_tb_trainer_qualifications");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntProfessionId).HasColumnName("int_profession_id");

                entity.Property(e => e.IntQualificationDuration).HasColumnName("int_qualification_duration");

                entity.Property(e => e.IntQualificationTypeId).HasColumnName("int_qualification_type_id");

                entity.Property(e => e.IntTqualificationTypeId).HasColumnName("int_tqualification_type_id");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.TxtQualificationName).HasColumnName("txt_qualification_name");
            });

            modelBuilder.Entity<CodeAssignType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_assign_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_assign_type");

                entity.HasIndex(e => e.VcAssignTypeName, "ndx_code_assign_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntOrderId).HasColumnName("int_order_id");

                entity.Property(e => e.VcAssignTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_assign_type_name");
            });

            modelBuilder.Entity<CodeCandidateProvidersState>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_candidate_providers_state_pkey")
                    .IsClustered(false);

                entity.ToTable("code_candidate_providers_state");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcCandidateProvidersStateName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_candidate_providers_state_name");
            });

            modelBuilder.Entity<CodeCandidateType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_candidate_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_candidate_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcCandidateTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_candidate_type_name");
            });

            modelBuilder.Entity<CodeCcontractType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_ccontract_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_ccontract_type");

                entity.HasIndex(e => e.VcCcontractTypeName, "ndx_code_ccontract_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcCcontractTypeName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_ccontract_type_name");
            });

            modelBuilder.Entity<CodeCfinishedType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_cfinished_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_cfinished_type");

                entity.HasIndex(e => e.VcCfinishedTypeName, "ndx_code_cfinished_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolFinished).HasColumnName("bool_finished");

                entity.Property(e => e.VcCfinishedTypeName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_cfinished_type_name");
            });

            modelBuilder.Entity<CodeCipoManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_cipo_management_pkey")
                    .IsClustered(false);

                entity.ToTable("code_cipo_management");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntDocumentsManagementId).HasColumnName("int_documents_management_id");

                entity.Property(e => e.IntUiControlType).HasColumnName("int_ui_control_type");

                entity.Property(e => e.VcCodeCipoManagementIdentAdd)
                    .HasMaxLength(50)
                    .HasColumnName("vc_code_cipo_management_ident_add");

                entity.Property(e => e.VcCodeCipoManagementIdentNew)
                    .HasMaxLength(50)
                    .HasColumnName("vc_code_cipo_management_ident_new");

                entity.Property(e => e.VcCodeCipoManagementName).HasColumnName("vc_code_cipo_management_name");

                entity.Property(e => e.VcExtraInfo)
                    .HasMaxLength(500)
                    .HasColumnName("vc_extra_info");

                entity.Property(e => e.VcListTable).HasColumnName("vc_list_table");

                entity.Property(e => e.VcPleaseText)
                    .HasMaxLength(300)
                    .HasColumnName("vc_please_text");

                entity.Property(e => e.Version).HasColumnName("version");
            });

            modelBuilder.Entity<CodeCommMember>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_comm_member_pkey")
                    .IsClustered(false);

                entity.ToTable("code_comm_member");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcCommMemberName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_comm_member_name");
            });

            modelBuilder.Entity<CodeCommissionInstitutionType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_commission_institution_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_commission_institution_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcCommissionInstitutionTypeName)
                    .HasMaxLength(110)
                    .HasColumnName("vc_commission_institution_type_name");

                entity.Property(e => e.VcShortName)
                    .HasMaxLength(110)
                    .HasColumnName("vc_short_name");
            });

            modelBuilder.Entity<CodeCorrection>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_correction_pkey")
                    .IsClustered(false);

                entity.ToTable("code_correction");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcCorrectionName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_correction_name");
            });

            modelBuilder.Entity<CodeCourseEdForm>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_course_ed_form_pkey")
                    .IsClustered(false);

                entity.ToTable("code_course_ed_form");

                entity.HasIndex(e => e.VcCourseEdFormName, "ndx_code_course_ed_form");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsValid)
                    .HasColumnName("bool_is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IntOrder).HasColumnName("int_order");

                entity.Property(e => e.VcCourseEdFormName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_course_ed_form_name");
            });

            modelBuilder.Entity<CodeCourseFrCurr>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_course_fr_curr_pkey")
                    .IsClustered(false);

                entity.ToTable("code_course_fr_curr");

                entity.HasIndex(e => e.VcCourseFrCurrName, "ndx_code_course_fr_curr");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolValid)
                    .HasColumnName("bool_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IntCourseType).HasColumnName("int_course_type");

                entity.Property(e => e.IntCourseValidationType).HasColumnName("int_course_validation_type");

                entity.Property(e => e.IntDurationMonths).HasColumnName("int_duration_months");

                entity.Property(e => e.IntMandatoryHours).HasColumnName("int_mandatory_hours");

                entity.Property(e => e.IntMinPercCommon).HasColumnName("int_min_perc_common");

                entity.Property(e => e.IntMinPercPractice).HasColumnName("int_min_perc_practice");

                entity.Property(e => e.IntOrder).HasColumnName("int_order");

                entity.Property(e => e.IntSelectableHours).HasColumnName("int_selectable_hours");

                entity.Property(e => e.IntTotalHours).HasColumnName("int_total_hours");

                entity.Property(e => e.IntVqs).HasColumnName("int_vqs");

                entity.Property(e => e.VcCourseFrCurrName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_course_fr_curr_name");

                entity.Property(e => e.VcDescInEdu)
                    .HasMaxLength(500)
                    .HasColumnName("vc_desc_in_edu");

                entity.Property(e => e.VcDescInQual)
                    .HasMaxLength(300)
                    .HasColumnName("vc_desc_in_qual");

                entity.Property(e => e.VcDescPerc)
                    .HasMaxLength(300)
                    .HasColumnName("vc_desc_perc");

                entity.Property(e => e.VcDescription)
                    .HasMaxLength(500)
                    .HasColumnName("vc_description");

                entity.Property(e => e.VcEdForms)
                    .HasMaxLength(200)
                    .HasColumnName("vc_ed_forms");

                entity.Property(e => e.VcShortDesc)
                    .HasMaxLength(100)
                    .HasColumnName("vc_short_desc");
            });

            modelBuilder.Entity<CodeCourseGroupRequiredDocumentsType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_course_group_required_documents_type_pk")
                    .IsClustered(false);

                entity.ToTable("code_course_group_required_documents_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolForClient)
                    .HasColumnName("bool_for_client")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BoolMandatory)
                    .HasColumnName("bool_mandatory")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CheckboxOrder).HasColumnName("checkbox_order");

                entity.Property(e => e.IntCourseType).HasColumnName("int_course_type");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VcCheckboxName)
                    .HasMaxLength(150)
                    .HasColumnName("vc_checkbox_name");

                entity.Property(e => e.VcDocumentType)
                    .HasMaxLength(100)
                    .HasColumnName("vc_document_type");
            });

            modelBuilder.Entity<CodeCourseMeasureType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_course_measure_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_course_measure_type");

                entity.HasIndex(e => e.VcCourseMeasureTypeName, "ndx_code_course_measure_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcCourseMeasureTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_course_measure_type_name");
            });

            modelBuilder.Entity<CodeCourseStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_course_status_pkey")
                    .IsClustered(false);

                entity.ToTable("code_course_status");

                entity.HasIndex(e => e.VcCourseStatusName, "ndx_code_course_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcCourseStatusName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_course_status_name");
            });

            modelBuilder.Entity<CodeCourseType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_course_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_course_type");

                entity.HasIndex(e => e.VcCourseTypeName, "ndx_code_course_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolGroup)
                    .HasColumnName("bool_group")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BoolHasFrCurr)
                    .HasColumnName("bool_has_fr_curr")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BoolHasSpeciality)
                    .HasColumnName("bool_has_speciality")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BoolRdpkCheck)
                    .HasColumnName("bool_rdpk_check")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BoolValid)
                    .HasColumnName("bool_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IntCfinishedTypeId).HasColumnName("int_cfinished_type_id");

                entity.Property(e => e.VcCourseTypeName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_course_type_name");

                entity.Property(e => e.VcCourseTypeNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_course_type_name_en");

                entity.Property(e => e.VcCourseTypeShort)
                    .HasMaxLength(155)
                    .HasColumnName("vc_course_type_short");
            });

            modelBuilder.Entity<CodeCpoManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_cpo_management_pkey")
                    .IsClustered(false);

                entity.ToTable("code_cpo_management");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntDocumentsManagementId).HasColumnName("int_documents_management_id");

                entity.Property(e => e.IntUiControlType).HasColumnName("int_ui_control_type");

                entity.Property(e => e.VcCodeCpoManagementIdentAdd)
                    .HasMaxLength(50)
                    .HasColumnName("vc_code_cpo_management_ident_add");

                entity.Property(e => e.VcCodeCpoManagementIdentNew)
                    .HasMaxLength(50)
                    .HasColumnName("vc_code_cpo_management_ident_new");

                entity.Property(e => e.VcCodeCpoManagementIdentP4)
                    .HasMaxLength(50)
                    .HasColumnName("vc_code_cpo_management_ident_p4");

                entity.Property(e => e.VcCodeCpoManagementName).HasColumnName("vc_code_cpo_management_name");

                entity.Property(e => e.VcExtraInfo)
                    .HasMaxLength(500)
                    .HasColumnName("vc_extra_info");

                entity.Property(e => e.VcListTable).HasColumnName("vc_list_table");

                entity.Property(e => e.VcPleaseText)
                    .HasMaxLength(100)
                    .HasColumnName("vc_please_text");

                entity.Property(e => e.Version).HasColumnName("version");
            });

            modelBuilder.Entity<CodeCurricHoursType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_curric_hours_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_curric_hours_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCodeTrainingTypeId).HasColumnName("int_code_training_type_id");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VcCurricHoursType)
                    .HasMaxLength(100)
                    .HasColumnName("vc_curric_hours_type");

                entity.Property(e => e.VcSectionCode)
                    .HasMaxLength(30)
                    .HasColumnName("vc_section_code");
            });

            modelBuilder.Entity<CodeDocumentStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_docuement_status_pk")
                    .IsClustered(false);

                entity.ToTable("code_document_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolLockAddClients).HasColumnName("bool_lock_add_clients");

                entity.Property(e => e.BoolLockAddDocument).HasColumnName("bool_lock_add_document");

                entity.Property(e => e.BoolLockAddProtokols).HasColumnName("bool_lock_add_protokols");

                entity.Property(e => e.BoolLockCg).HasColumnName("bool_lock_cg");

                entity.Property(e => e.BoolLockClient).HasColumnName("bool_lock_client");

                entity.Property(e => e.BoolLockClientDocs).HasColumnName("bool_lock_client_docs");

                entity.Property(e => e.BoolLockProtokols).HasColumnName("bool_lock_protokols");

                entity.Property(e => e.BoolLockRollback).HasColumnName("bool_lock_rollback");

                entity.Property(e => e.BoolWithNote).HasColumnName("bool_with_note");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VcButtonName)
                    .HasMaxLength(150)
                    .HasColumnName("vc_button_name");

                entity.Property(e => e.VcStatusName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_status_name");
            });

            modelBuilder.Entity<CodeDocumentStatusLock>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_document_status_locks_pk")
                    .IsClustered(false);

                entity.ToTable("code_document_status_locks");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcStatusLockName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_status_lock_name");
            });

            modelBuilder.Entity<CodeDocumentType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_document_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_document_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolHasFabNumber).HasColumnName("bool_has_fab_number");

                entity.Property(e => e.BoolHasFile).HasColumnName("bool_has_file");

                entity.Property(e => e.BoolHasMarks).HasColumnName("bool_has_marks");

                entity.Property(e => e.BoolHasQual).HasColumnName("bool_has_qual");

                entity.Property(e => e.BoolMoreThanOne).HasColumnName("bool_more_than_one");

                entity.Property(e => e.IntParentId).HasColumnName("int_parent_id");

                entity.Property(e => e.VcDocumentTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_document_type_name");

                entity.Property(e => e.VcDocumentTypeNameEn)
                    .HasMaxLength(100)
                    .HasColumnName("vc_document_type_name_en");
            });

            modelBuilder.Entity<CodeDocumentValidityCheck>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("code_document_validity_checks");

                entity.HasIndex(e => e.Id, "code_document_validity_checks_uk")
                    .IsUnique()
                    .HasFilter("([id] IS NOT NULL)");

                entity.Property(e => e.BoolDuplicate).HasColumnName("bool_duplicate");

                entity.Property(e => e.BoolIfRows0)
                    .HasColumnName("bool_if_rows0")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BoolMandatory).HasColumnName("bool_mandatory");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCodeValidityCheckTarget).HasColumnName("int_code_validity_check_target");

                entity.Property(e => e.IntCourseType).HasColumnName("int_course_type");

                entity.Property(e => e.IntDocumentTypeId).HasColumnName("int_document_type_id");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VcCondition)
                    .HasMaxLength(1000)
                    .HasColumnName("vc_condition");

                entity.Property(e => e.VcDescription)
                    .HasMaxLength(500)
                    .HasColumnName("vc_description");

                entity.Property(e => e.VcFunctionName)
                    .HasMaxLength(200)
                    .HasColumnName("vc_function_name");

                entity.Property(e => e.VcInFile)
                    .HasMaxLength(200)
                    .HasColumnName("vc_in_file");
            });

            modelBuilder.Entity<CodeDocumentsManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_documents_management_pkey")
                    .IsClustered(false);

                entity.ToTable("code_documents_management");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsBrra)
                    .HasColumnName("bool_is_brra")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BoolIsConditional)
                    .HasColumnName("bool_is_conditional")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BoolIsMandatory).HasColumnName("bool_is_mandatory");

                entity.Property(e => e.BoolIsNotBrra)
                    .HasColumnName("bool_is_not_brra")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BoolIsPrnOnly)
                    .HasColumnName("bool_is_prn_only")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BoolIsValid)
                    .HasColumnName("bool_is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IntCandidateTypeId).HasColumnName("int_candidate_type_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Seenbyexpert).HasColumnName("seenbyexpert");

                entity.Property(e => e.VcCondition).HasColumnName("vc_condition");

                entity.Property(e => e.VcDocumentsManagementName).HasColumnName("vc_documents_management_name");
            });

            modelBuilder.Entity<CodeEducation>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_education_pkey")
                    .IsClustered(false);

                entity.ToTable("code_education");

                entity.HasIndex(e => e.VcEducationName, "ndx_code_education");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntMonId).HasColumnName("int_mon_id");

                entity.Property(e => e.VcEducationName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_education_name");
            });

            modelBuilder.Entity<CodeEgnType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_egn_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_egn_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcEgnType)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn_type");
            });

            modelBuilder.Entity<CodeEkatte>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_ekatte_pkey")
                    .IsClustered(false);

                entity.ToTable("code_ekatte");

                entity.HasIndex(e => e.VcName, "ndx_code_ekatte");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.IntPostCode).HasColumnName("int_post_code");

                entity.Property(e => e.IntVillageTypeId).HasColumnName("int_village_type_id");

                entity.Property(e => e.VcCat)
                    .HasMaxLength(10)
                    .HasColumnName("vc_cat");

                entity.Property(e => e.VcHeight)
                    .HasMaxLength(10)
                    .HasColumnName("vc_height");

                entity.Property(e => e.VcKati)
                    .HasMaxLength(20)
                    .HasColumnName("vc_kati");

                entity.Property(e => e.VcName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_name");

                entity.Property(e => e.VcPhoneCode)
                    .HasMaxLength(15)
                    .HasColumnName("vc_phone_code");

                entity.Property(e => e.VcTextCode)
                    .HasMaxLength(5)
                    .HasColumnName("vc_text_code");
            });

            modelBuilder.Entity<CodeEkatteFull>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_ekatte_full_pkey")
                    .IsClustered(false);

                entity.ToTable("code_ekatte_full");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.IntPostCode).HasColumnName("int_post_code");

                entity.Property(e => e.IntVillageTypeId).HasColumnName("int_village_type_id");

                entity.Property(e => e.VcCat)
                    .HasMaxLength(10)
                    .HasColumnName("vc_cat");

                entity.Property(e => e.VcHeight)
                    .HasMaxLength(10)
                    .HasColumnName("vc_height");

                entity.Property(e => e.VcKati)
                    .HasMaxLength(20)
                    .HasColumnName("vc_kati");

                entity.Property(e => e.VcName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_name");

                entity.Property(e => e.VcPhoneCode)
                    .HasMaxLength(15)
                    .HasColumnName("vc_phone_code");

                entity.Property(e => e.VcTextCode)
                    .HasMaxLength(5)
                    .HasColumnName("vc_text_code");
            });

            modelBuilder.Entity<CodeEmploymentStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_employment_status_pkey")
                    .IsClustered(false);

                entity.ToTable("code_employment_status");

                entity.HasIndex(e => e.VcEmploymentStatusName, "ndx_code_employment_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcEmploymentStatusName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_employment_status_name");
            });

            modelBuilder.Entity<CodeExpertType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_expert_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_expert_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcExpertTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_expert_type_name");
            });

            modelBuilder.Entity<CodeExtRegister>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_ext_register_pkey")
                    .IsClustered(false);

                entity.ToTable("code_ext_register");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.VcRegisterName)
                    .HasMaxLength(250)
                    .HasColumnName("vc_register_name");
            });

            modelBuilder.Entity<CodeGender>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_gender_pkey")
                    .IsClustered(false);

                entity.ToTable("code_gender");

                entity.HasIndex(e => e.VcGender, "ndx_code_gender");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcGender)
                    .HasMaxLength(50)
                    .HasColumnName("vc_gender");
            });

            modelBuilder.Entity<CodeLicenceStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_licence_status_pkey")
                    .IsClustered(false);

                entity.ToTable("code_licence_status");

                entity.HasIndex(e => e.VcLicenceStatusName, "ndx_code_licence_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcLicStatusShortName)
                    .HasMaxLength(20)
                    .HasColumnName("vc_lic_status_short_name");

                entity.Property(e => e.VcLicStatusShortNameEn)
                    .HasMaxLength(20)
                    .HasColumnName("vc_lic_status_short_name_en");

                entity.Property(e => e.VcLicenceStatusName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_licence_status_name");

                entity.Property(e => e.VcLicenceStatusNameEn)
                    .HasMaxLength(50)
                    .HasColumnName("vc_licence_status_name_en");
            });

            modelBuilder.Entity<CodeLicenceStatusDetail>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_licence_status_details_pkey")
                    .IsClustered(false);

                entity.ToTable("code_licence_status_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcLicenceStatusDetailsName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_licence_status_details_name");

                entity.Property(e => e.VcLicenceStatusDetailsNameEn)
                    .HasMaxLength(50)
                    .HasColumnName("vc_licence_status_details_name_en");
            });

            modelBuilder.Entity<CodeMunicipality>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_municipality_pkey")
                    .IsClustered(false);

                entity.ToTable("code_municipality");

                entity.HasIndex(e => e.VcMunicipalityName, "ndx_code_municipality");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.VcMunicipalityCodeName)
                    .HasMaxLength(5)
                    .HasColumnName("vc_municipality_code_name");

                entity.Property(e => e.VcMunicipalityName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_municipality_name");
            });

            modelBuilder.Entity<CodeMunicipalityDetail>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_municipality_details_pkey")
                    .IsClustered(false);

                entity.ToTable("code_municipality_details");

                entity.HasIndex(e => e.VcMunicipalityDetailsName, "ndx_code_municipality_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.VcMunicipalityDetailsName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_municipality_details_name");
            });

            modelBuilder.Entity<CodeNationality>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_nationality_pkey")
                    .IsClustered(false);

                entity.ToTable("code_nationality");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsEuMember).HasColumnName("bool_is_eu_member");

                entity.Property(e => e.VcCountryName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_country_name");
            });

            modelBuilder.Entity<CodeNkpd>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_nkpd_pkey")
                    .IsClustered(false);

                entity.ToTable("code_nkpd");

                entity.HasIndex(e => new { e.IntNkpdId1, e.IntNkpdId2 }, "unique_int_nkpd_id")
                    .IsUnique()
                    .HasFilter("([int_nkpd_id1] IS NOT NULL AND [int_nkpd_id2] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntNkpdId1)
                    .HasMaxLength(4)
                    .HasColumnName("int_nkpd_id1");

                entity.Property(e => e.IntNkpdId2)
                    .HasMaxLength(4)
                    .HasColumnName("int_nkpd_id2");

                entity.Property(e => e.VcNkpdName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_nkpd_name");
            });

            modelBuilder.Entity<CodeObl>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_obl_pkey")
                    .IsClustered(false);

                entity.ToTable("code_obl");

                entity.HasIndex(e => e.VcOblName, "ndx_code_obl");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcOblCodeName)
                    .HasMaxLength(3)
                    .HasColumnName("vc_obl_code_name");

                entity.Property(e => e.VcOblName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_obl_name");
            });

            modelBuilder.Entity<CodeOperation>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_operation_pkey")
                    .IsClustered(false);

                entity.ToTable("code_operation");

                entity.HasIndex(e => e.VcName, "unique_vc_name")
                    .IsUnique()
                    .HasFilter("([vc_name] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntOrder).HasColumnName("int_order");

                entity.Property(e => e.VcName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_name");
            });

            modelBuilder.Entity<CodePremisesCorrespondence>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_premises_correspondence_pkey")
                    .IsClustered(false);

                entity.ToTable("code_premises_correspondence");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcPremisesCorrespondenceName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_premises_correspondence_name");
            });

            modelBuilder.Entity<CodePremisesStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_premises_status_pkey")
                    .IsClustered(false);

                entity.ToTable("code_premises_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcPremisesStatusName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_premises_status_name");
            });

            modelBuilder.Entity<CodePremisesType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_premises_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_premises_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcPremisesTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_premises_type_name");
            });

            modelBuilder.Entity<CodePremisesUsage>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_premises_usage_pkey")
                    .IsClustered(false);

                entity.ToTable("code_premises_usage");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcPremisesUsageName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_premises_usage_name");
            });

            modelBuilder.Entity<CodeProcedure>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_procedures_pkey")
                    .IsClustered(false);

                entity.ToTable("code_procedures");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<CodeProcedureDecision>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_procedure_decision_pkey")
                    .IsClustered(false);

                entity.ToTable("code_procedure_decision");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.VcProcedureDecision)
                    .HasMaxLength(255)
                    .HasColumnName("vc_procedure_decision");

                entity.Property(e => e.VcProcedureDecisionEn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_procedure_decision_en");
            });

            modelBuilder.Entity<CodeProcedureStage>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_procedure_stages_pkey")
                    .IsClustered(false);

                entity.ToTable("code_procedure_stages");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<CodeProcedureStep>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_procedure_steps_pkey")
                    .IsClustered(false);

                entity.ToTable("code_procedure_steps");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<CodeProceduresDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_procedures_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("code_procedures_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCandidateTypeId).HasColumnName("int_candidate_type_id");

                entity.Property(e => e.IntDocumentId).HasColumnName("int_document_id");

                entity.Property(e => e.IntOrderId).HasColumnName("int_order_id");

                entity.Property(e => e.IntRowId).HasColumnName("int_row_id");

                entity.Property(e => e.VcProceduresDocumentsName).HasColumnName("vc_procedures_documents_name");
            });

            modelBuilder.Entity<CodeProtokolType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_protokol_type_pk")
                    .IsClustered(false);

                entity.ToTable("code_protokol_type");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.BoolValid).HasColumnName("bool_valid");

                entity.Property(e => e.VcCodeName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_code_name");

                entity.Property(e => e.VcName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_name");
            });

            modelBuilder.Entity<CodeProviderOwnership>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_provider_ownership_pkey")
                    .IsClustered(false);

                entity.ToTable("code_provider_ownership");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcProviderOwnershipName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_provider_ownership_name");
            });

            modelBuilder.Entity<CodeProviderRegistration>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_provider_registration_pkey")
                    .IsClustered(false);

                entity.ToTable("code_provider_registration");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcProviderRegistrationTypeName)
                    .HasMaxLength(90)
                    .HasColumnName("vc_provider_registration_type_name");
            });

            modelBuilder.Entity<CodeProviderStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_provider_status_pkey")
                    .IsClustered(false);

                entity.ToTable("code_provider_status");

                entity.HasIndex(e => e.VcProviderStatusName, "ndx_code_provider_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsBrra).HasColumnName("is_brra");

                entity.Property(e => e.IsCpo).HasColumnName("is_cpo");

                entity.Property(e => e.VcProviderStatusName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_provider_status_name");
            });

            modelBuilder.Entity<CodeQualLevel>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_qual_level_pkey")
                    .IsClustered(false);

                entity.ToTable("code_qual_level");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolSameArea).HasColumnName("bool_same_area");

                entity.Property(e => e.BoolSameProf).HasColumnName("bool_same_prof");

                entity.Property(e => e.BoolSpkPart).HasColumnName("bool_spk_part");

                entity.Property(e => e.IntGradeSpk).HasColumnName("int_grade_spk");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VcQualLevelName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_qual_level_name");
            });

            modelBuilder.Entity<CodeQualificationType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_qualification_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_qualification_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcQualificationTypeName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_qualification_type_name");
            });

            modelBuilder.Entity<CodeReceiveDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_receive_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("code_receive_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcReceiveDocumentsName).HasColumnName("vc_receive_documents_name");
            });

            modelBuilder.Entity<CodeReceiveType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_receive_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_receive_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcReceiveTypeName).HasColumnName("vc_receive_type_name");
            });

            modelBuilder.Entity<CodeRequestDocStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_request_doc_status_pkey")
                    .IsClustered(false);

                entity.ToTable("code_request_doc_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcRequestDocStatusName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_request_doc_status_name");
            });

            modelBuilder.Entity<CodeRequestDocType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_request_doc_pkey")
                    .IsClustered(false);

                entity.ToTable("code_request_doc_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolHasSerialNumber).HasColumnName("bool_has_serial_number");

                entity.Property(e => e.BoolIsValid).HasColumnName("bool_is_valid");

                entity.Property(e => e.IntCodeDocumentTypeId).HasColumnName("int_code_document_type_id");

                entity.Property(e => e.IntCurrentPeriod).HasColumnName("int_current_period");

                entity.Property(e => e.IntDocPrice).HasColumnName("int_doc_price");

                entity.Property(e => e.IntOrderId).HasColumnName("int_order_id");

                entity.Property(e => e.IsDestroyable).HasColumnName("is_destroyable");

                entity.Property(e => e.VcRequestDocTypeName)
                    .HasMaxLength(2000)
                    .HasColumnName("vc_request_doc_type_name");

                entity.Property(e => e.VcRequestDocTypeOfficialNumber)
                    .HasMaxLength(50)
                    .HasColumnName("vc_request_doc_type_official_number");
            });

            modelBuilder.Entity<CodeRequestDocsOperation>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_request_docs_operation_pkey")
                    .IsClustered(false);

                entity.ToTable("code_request_docs_operation");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.VcRequestDocsOperationName)
                    .HasMaxLength(250)
                    .HasColumnName("vc_request_docs_operation_name");
            });

            modelBuilder.Entity<CodeRequestDocsSeries>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_request_docs_series_pkey")
                    .IsClustered(false);

                entity.ToTable("code_request_docs_series");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.IntDocType).HasColumnName("int_doc_type");

                entity.Property(e => e.IntDocYear).HasColumnName("int_doc_year");

                entity.Property(e => e.VcRequestDocTypeOfficialNumber)
                    .HasMaxLength(50)
                    .HasColumnName("vc_request_doc_type_official_number");

                entity.Property(e => e.VcSeriesName)
                    .HasMaxLength(250)
                    .HasColumnName("vc_series_name");
            });

            modelBuilder.Entity<CodeSpecialityCurriculumUpdateReason>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_speciality_curriculum_update_reason_pkey")
                    .IsClustered(false);

                entity.ToTable("code_speciality_curriculum_update_reason");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_name");
            });

            modelBuilder.Entity<CodeSpecialityVq>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_speciality_vqs_pk")
                    .IsClustered(false);

                entity.ToTable("code_speciality_vqs");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcVqsName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_vqs_name");

                entity.Property(e => e.VcVqsShortName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_vqs_short_name");
            });

            modelBuilder.Entity<CodeStageDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_stage_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("code_stage_documents");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CanBeMoreThanOne).HasColumnName("can_be_more_than_one");

                entity.Property(e => e.EDelivery).HasColumnName("e_delivery");

                entity.Property(e => e.HasSignedCopy).HasColumnName("has_signed_copy");

                entity.Property(e => e.Iorder).HasColumnName("iorder");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

                entity.Property(e => e.MnemCode)
                    .HasMaxLength(100)
                    .HasColumnName("mnem_code");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Parent).HasColumnName("parent");

                entity.Property(e => e.RefProcedureStepStageId).HasColumnName("ref_procedure_step_stage_id");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.Property(e => e.Uploadbyexpert).HasColumnName("uploadbyexpert");
            });

            modelBuilder.Entity<CodeStageDocumentType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_stage_document_types_pkey")
                    .IsClustered(false);

                entity.ToTable("code_stage_document_types");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<CodeTcontractType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_tcontract_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_tcontract_type");

                entity.HasIndex(e => e.VcTcontractTypeName, "ndx_code_tcontract_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcTcontractTypeName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_tcontract_type_name");
            });

            modelBuilder.Entity<CodeTqualificationType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_tqualification_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_tqualification_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcTqualificationTypeName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_tqualification_type_name");
            });

            modelBuilder.Entity<CodeTrainingType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_training_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_training_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolGroupMtb).HasColumnName("bool_group_mtb");

                entity.Property(e => e.BoolGroupTrainer).HasColumnName("bool_group_trainer");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VcSection)
                    .HasMaxLength(100)
                    .HasColumnName("vc_section");

                entity.Property(e => e.VcTrainingTypeName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_training_type_name");
            });

            modelBuilder.Entity<CodeUiControlType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_ui_control_type_pk")
                    .IsClustered(false);

                entity.ToTable("code_ui_control_type");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.VcFuriaName)
                    .HasMaxLength(90)
                    .HasColumnName("vc_furia_name");

                entity.Property(e => e.VcPleaseText)
                    .HasMaxLength(150)
                    .HasColumnName("vc_please_text");
            });

            modelBuilder.Entity<CodeUploadDocStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_upload_doc_status_pkey")
                    .IsClustered(false);

                entity.ToTable("code_upload_doc_status");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.VcDocStatusName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_doc_status_name");
            });

            modelBuilder.Entity<CodeUploadDocType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_upload_doc_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_upload_doc_type");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.BoolForCpo).HasColumnName("bool_for_cpo");

                entity.Property(e => e.BoolValid).HasColumnName("bool_valid");

                entity.Property(e => e.BoolYearDependent).HasColumnName("bool_year_dependent");

                entity.Property(e => e.VcDocTypeName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_doc_type_name");

                entity.Property(e => e.VcDocTypeNameShort)
                    .HasMaxLength(100)
                    .HasColumnName("vc_doc_type_name_short");
            });

            modelBuilder.Entity<CodeValidityCheckTarget>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_validity_check_target_pk")
                    .IsClustered(false);

                entity.ToTable("code_validity_check_target");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.VcName)
                    .HasMaxLength(200)
                    .HasColumnName("vc_name");
            });

            modelBuilder.Entity<CodeVetArea>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_vet_area_pkey")
                    .IsClustered(false);

                entity.ToTable("code_vet_area");

                entity.HasIndex(e => e.IntVetAreaNumber, "ndx_code_vet_area");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsValid)
                    .HasColumnName("bool_is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IntVetAreaCorrection).HasColumnName("int_vet_area_correction");

                entity.Property(e => e.IntVetAreaCorrectionParent).HasColumnName("int_vet_area_correction_parent");

                entity.Property(e => e.IntVetAreaNumber).HasColumnName("int_vet_area_number");

                entity.Property(e => e.IntVetGroupId).HasColumnName("int_vet_group_id");

                entity.Property(e => e.IntVetListId).HasColumnName("int_vet_list_id");

                entity.Property(e => e.VcVetAreaCorrectionNotes)
                    .HasMaxLength(255)
                    .HasColumnName("vc_vet_area_correction_notes");

                entity.Property(e => e.VcVetAreaName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_vet_area_name");

                entity.Property(e => e.VcVetAreaNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_vet_area_name_en");
            });

            modelBuilder.Entity<CodeVetGroup>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_vet_group_pkey")
                    .IsClustered(false);

                entity.ToTable("code_vet_group");

                entity.HasIndex(e => e.VcVetGroupName, "ndx_code_vet_group");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsValid)
                    .HasColumnName("bool_is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IntVetGroupCorrection).HasColumnName("int_vet_group_correction");

                entity.Property(e => e.IntVetGroupCorrectionParent).HasColumnName("int_vet_group_correction_parent");

                entity.Property(e => e.IntVetGroupNumber).HasColumnName("int_vet_group_number");

                entity.Property(e => e.IntVetListId).HasColumnName("int_vet_list_id");

                entity.Property(e => e.VcVetGroupCorrectionNotes)
                    .HasMaxLength(255)
                    .HasColumnName("vc_vet_group_correction_notes");

                entity.Property(e => e.VcVetGroupName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_vet_group_name");

                entity.Property(e => e.VcVetGroupNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_vet_group_name_en");
            });

            modelBuilder.Entity<CodeVetList>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_vet_list_pkey")
                    .IsClustered(false);

                entity.ToTable("code_vet_list");

                entity.HasIndex(e => e.VcVetListName, "ndx_code_vet_list");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsValid)
                    .HasColumnName("bool_is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VcVetListName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_vet_list_name");
            });

            modelBuilder.Entity<CodeVetProfession>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_vet_profession_pkey")
                    .IsClustered(false);

                entity.ToTable("code_vet_profession");

                entity.HasIndex(e => e.VcVetProfessionName, "ndx_code_vet_profession");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsValid)
                    .HasColumnName("bool_is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetGroupId).HasColumnName("int_vet_group_id");

                entity.Property(e => e.IntVetListId).HasColumnName("int_vet_list_id");

                entity.Property(e => e.IntVetProfessionCorrection).HasColumnName("int_vet_profession_correction");

                entity.Property(e => e.IntVetProfessionCorrectionParent).HasColumnName("int_vet_profession_correction_parent");

                entity.Property(e => e.IntVetProfessionNumber).HasColumnName("int_vet_profession_number");

                entity.Property(e => e.VcVetProfessionCorrectionNotes)
                    .HasMaxLength(255)
                    .HasColumnName("vc_vet_profession_correction_notes");

                entity.Property(e => e.VcVetProfessionName)
                    .HasMaxLength(200)
                    .HasColumnName("vc_vet_profession_name");

                entity.Property(e => e.VcVetProfessionNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_vet_profession_name_en");
            });

            modelBuilder.Entity<CodeVetSpeciality>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_vet_speciality_pkey")
                    .IsClustered(false);

                entity.ToTable("code_vet_speciality");

                entity.HasIndex(e => e.VcVetSpecialityName, "ndx_code_vet_speciality");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsValid)
                    .HasColumnName("bool_is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DtEndDateEvent)
                    .HasColumnType("date")
                    .HasColumnName("dt_end_date_event");

                entity.Property(e => e.DtStartDateEvent)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date_event");

                entity.Property(e => e.IntSpecialityVqs).HasColumnName("int_speciality_vqs");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetGroupId).HasColumnName("int_vet_group_id");

                entity.Property(e => e.IntVetListId).HasColumnName("int_vet_list_id");

                entity.Property(e => e.IntVetProfessionId).HasColumnName("int_vet_profession_id");

                entity.Property(e => e.IntVetSpecialityCorrection).HasColumnName("int_vet_speciality_correction");

                entity.Property(e => e.IntVetSpecialityCorrectionParent).HasColumnName("int_vet_speciality_correction_parent");

                entity.Property(e => e.IntVetSpecialityNumber).HasColumnName("int_vet_speciality_number");

                entity.Property(e => e.VcVetSpecialityCorrectionNotes)
                    .HasMaxLength(255)
                    .HasColumnName("vc_vet_speciality_correction_notes");

                entity.Property(e => e.VcVetSpecialityName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_vet_speciality_name");

                entity.Property(e => e.VcVetSpecialityNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_vet_speciality_name_en");
            });

            modelBuilder.Entity<CodeVillageType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_village_type_pkey")
                    .IsClustered(false);

                entity.ToTable("code_village_type");

                entity.HasIndex(e => e.VcVillageTypeName, "ndx_code_village_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcVillageTypeName)
                    .HasMaxLength(30)
                    .HasColumnName("vc_village_type_name");

                entity.Property(e => e.VcVillageTypeShortName)
                    .HasMaxLength(10)
                    .HasColumnName("vc_village_type_short_name");
            });

            modelBuilder.Entity<CodeVisitResult>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_visit_result_pkey")
                    .IsClustered(false);

                entity.ToTable("code_visit_result");

                entity.HasIndex(e => e.VcVisitResultName, "ndx_code_visit_result");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcVisitResultName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_visit_result_name");
            });

            modelBuilder.Entity<CodeWgdoiFunction>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("code_wgdoi_function_pkey")
                    .IsClustered(false);

                entity.ToTable("code_wgdoi_function");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcWgdoiFunctionName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_wgdoi_function_name");
            });

            modelBuilder.Entity<IscsIisclientdatum>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("iscs_iisclientdata");

                entity.Property(e => e.IisCdata)
                    .HasColumnName("iis_cdata")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IisData)
                    .HasMaxLength(500)
                    .HasColumnName("iis_data")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.IisLastRequest)
                    .HasMaxLength(12)
                    .HasColumnName("iis_last_request")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.IisServerKey)
                    .HasMaxLength(50)
                    .HasColumnName("iis_server_key")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.IisServerName)
                    .HasMaxLength(50)
                    .HasColumnName("iis_server_name");

                entity.Property(e => e.IisServerUrl)
                    .HasMaxLength(150)
                    .HasColumnName("iis_server_url");
            });

            modelBuilder.Entity<IscsIisserverdatum>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("iscs_iisserverdata");

                entity.Property(e => e.IisClient)
                    .HasMaxLength(50)
                    .HasColumnName("iis_client");

                entity.Property(e => e.IisClient0)
                    .HasMaxLength(50)
                    .HasColumnName("iis_client0");

                entity.Property(e => e.IisData)
                    .HasMaxLength(500)
                    .HasColumnName("iis_data")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.IisLastCdata)
                    .HasMaxLength(30)
                    .HasColumnName("iis_last_cdata")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.IisLastCrequest)
                    .HasMaxLength(12)
                    .HasColumnName("iis_last_crequest")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.IisLastRequest)
                    .HasMaxLength(12)
                    .HasColumnName("iis_last_request")
                    .HasDefaultValueSql("('0')");
            });

            modelBuilder.Entity<MvRegisterClient>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("mv_register_clients");

                entity.Property(e => e.DocumentId).HasColumnName("document_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntClientsCoursesId).HasColumnName("int_clients_courses_id");

                entity.Property(e => e.IntCourseFinishedYear).HasColumnName("int_course_finished_year");

                entity.Property(e => e.IntCourseTypeId).HasColumnName("int_course_type_id");

                entity.Property(e => e.IntEkkateId).HasColumnName("int_ekkate_id");

                entity.Property(e => e.IntLicenceNumber).HasColumnName("int_licence_number");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntSpecialityVqs).HasColumnName("int_speciality_vqs");

                entity.Property(e => e.IntVetAreaNumber).HasColumnName("int_vet_area_number");

                entity.Property(e => e.IntVetProfessionNumber).HasColumnName("int_vet_profession_number");

                entity.Property(e => e.IntVetSpecialityNumber).HasColumnName("int_vet_speciality_number");

                entity.Property(e => e.VcClientName).HasColumnName("vc_client_name");

                entity.Property(e => e.VcCourseTypeName).HasColumnName("vc_course_type_name");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");

                entity.Property(e => e.VcEgnType)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn_type");

                entity.Property(e => e.VcEkkateName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_ekkate_name");

                entity.Property(e => e.VcMunicipalityName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_municipality_name");

                entity.Property(e => e.VcOblName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_obl_name");

                entity.Property(e => e.VcProviderName).HasColumnName("vc_provider_name");

                entity.Property(e => e.VcSpecialityVqs).HasColumnName("vc_speciality_vqs");

                entity.Property(e => e.VcVetAreaName).HasColumnName("vc_vet_area_name");

                entity.Property(e => e.VcVetProfessionName).HasColumnName("vc_vet_profession_name");

                entity.Property(e => e.VcVetSpecialityName).HasColumnName("vc_vet_speciality_name");
            });

            modelBuilder.Entity<RefArchExpert>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_arch_experts_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_arch_experts");

                entity.HasIndex(e => new { e.IntArchProviderId, e.IntExpertId }, "ref_arch_experts_provider_expert_key")
                    .IsUnique()
                    .HasFilter("([int_arch_provider_id] IS NOT NULL AND [int_expert_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntArchProviderId).HasColumnName("int_arch_provider_id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");
            });

            modelBuilder.Entity<RefArchExpertCommission>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_arch_expert_commissions_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_arch_expert_commissions");

                entity.HasIndex(e => new { e.IntArchProviderId, e.IntExpertCommissionId }, "ref_arch_expert_commissions_provider_commission_key")
                    .IsUnique()
                    .HasFilter("([int_arch_provider_id] IS NOT NULL AND [int_expert_commission_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntArchProviderId).HasColumnName("int_arch_provider_id");

                entity.Property(e => e.IntExpertCommissionId).HasColumnName("int_expert_commission_id");
            });

            modelBuilder.Entity<RefArchProcedureExpert>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("ref_arch_procedure_experts_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_arch_procedure_experts");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetArea).HasColumnName("int_vet_area");
            });

            modelBuilder.Entity<RefArchProcedureExpertCommission>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("ref_arch_procedure_expert_commissions_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_arch_procedure_expert_commissions");

                entity.HasIndex(e => new { e.IntStartedProcedureId, e.IntProviderId, e.IntExpertCommissionId }, "ref_arch_procedure_expert_commissions_provider_commission_key")
                    .IsUnique()
                    .HasFilter("([int_started_procedure_id] IS NOT NULL AND [int_provider_id] IS NOT NULL AND [int_expert_commission_id] IS NOT NULL)");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.IntExpertCommissionId).HasColumnName("int_expert_commission_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");
            });

            modelBuilder.Entity<RefArchProcedureProceduresDocument>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("ref_arch_procedure_procedures_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_arch_procedure_procedures_documents");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntProceduresDocumentsId).HasColumnName("int_procedures_documents_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtProceduresDocumentsFile).HasColumnName("txt_procedures_documents_file");

                entity.Property(e => e.TxtProceduresDocumentsNotes).HasColumnName("txt_procedures_documents_notes");
            });

            modelBuilder.Entity<RefArchProcedureProviderPremisesSpeciality>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("ref_arch_procedure_provider_premises_specialities_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_arch_procedure_provider_premises_specialities");

                entity.HasIndex(e => new { e.IntProviderPremiseId, e.IntProviderSpecialityId }, "ref_arch_procedure_prov_premises_specialities_int_provider_prem")
                    .IsUnique()
                    .HasFilter("([int_provider_premise_id] IS NOT NULL AND [int_provider_speciality_id] IS NOT NULL)");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.IntProviderPremiseId).HasColumnName("int_provider_premise_id");

                entity.Property(e => e.IntProviderPremiseSpecialityCorrespondence).HasColumnName("int_provider_premise_speciality_correspondence");

                entity.Property(e => e.IntProviderPremiseSpecialityUsage).HasColumnName("int_provider_premise_speciality_usage");

                entity.Property(e => e.IntProviderSpecialityId).HasColumnName("int_provider_speciality_id");
            });

            modelBuilder.Entity<RefArchProceduresDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_arch_procedures_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_arch_procedures_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntArchProviderId).HasColumnName("int_arch_provider_id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntProceduresDocumentsId).HasColumnName("int_procedures_documents_id");

                entity.Property(e => e.TxtProceduresDocumentsFile).HasColumnName("txt_procedures_documents_file");

                entity.Property(e => e.TxtProceduresDocumentsNotes).HasColumnName("txt_procedures_documents_notes");
            });

            modelBuilder.Entity<RefCandidateProviderPremisesSpeciality>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_candidate_provider_premises_specialities_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_candidate_provider_premises_specialities");

                entity.HasIndex(e => new { e.IntProviderPremiseId, e.IntProviderSpecialityId }, "ref_cand_prov_premises_specialities_int_provider_premise_id_key")
                    .IsUnique()
                    .HasFilter("([int_provider_premise_id] IS NOT NULL AND [int_provider_speciality_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntProviderPremiseId).HasColumnName("int_provider_premise_id");

                entity.Property(e => e.IntProviderPremiseSpecialityCorrespondence).HasColumnName("int_provider_premise_speciality_correspondence");

                entity.Property(e => e.IntProviderPremiseSpecialityUsage).HasColumnName("int_provider_premise_speciality_usage");

                entity.Property(e => e.IntProviderSpecialityId).HasColumnName("int_provider_speciality_id");
            });

            modelBuilder.Entity<RefCandidatesExpert>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_candidates_experts_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_candidates_experts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetArea).HasColumnName("int_vet_area");
            });

            modelBuilder.Entity<RefCandidatesExpertCommission>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_candidates_expert_commissions_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_candidates_expert_commissions");

                entity.HasIndex(e => new { e.IntProviderId, e.IntExpertCommissionId }, "ref_candidates_expert_commissions_provider_commission_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [int_expert_commission_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntExpertCommissionId).HasColumnName("int_expert_commission_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");
            });

            modelBuilder.Entity<RefCandidatesProceduresDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_candidates_procedures_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_candidates_procedures_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntProceduresDocumentsId).HasColumnName("int_procedures_documents_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtProceduresDocumentsFile).HasColumnName("txt_procedures_documents_file");

                entity.Property(e => e.TxtProceduresDocumentsNotes).HasColumnName("txt_procedures_documents_notes");
            });

            modelBuilder.Entity<RefCgCurricFile>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_cg_curric_files_pk")
                    .IsClustered(false);

                entity.ToTable("ref_cg_curric_files");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntCourseId).HasColumnName("int_course_id");

                entity.Property(e => e.IntProviderSpecialitiesCurriculumId).HasColumnName("int_provider_specialities_curriculum_id");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<RefClientsCourse>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_clients_courses_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_clients_courses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtClientBirthDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_client_birth_date");

                entity.Property(e => e.DtCourseFinished)
                    .HasColumnType("date")
                    .HasColumnName("dt_course_finished");

                entity.Property(e => e.IntAssignTypeId).HasColumnName("int_assign_type_id");

                entity.Property(e => e.IntCfinishedTypeId).HasColumnName("int_cfinished_type_id");

                entity.Property(e => e.IntClientGender).HasColumnName("int_client_gender");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntEducationId).HasColumnName("int_education_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntQualLevel).HasColumnName("int_qual_level");

                entity.Property(e => e.IntQualVetArea).HasColumnName("int_qual_vet_area");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");

                entity.Property(e => e.VcFamilyName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_family_name");

                entity.Property(e => e.VcFirstName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_first_name");

                entity.Property(e => e.VcSecondName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_second_name");
            });

            modelBuilder.Entity<RefClientsCoursesDocumentsStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_clients_courses_documents_status_pk")
                    .IsClustered(false);

                entity.ToTable("ref_clients_courses_documents_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Dt)
                    .HasColumnName("dt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntClientCoursesDocumentsId).HasColumnName("int_client_courses_documents_id");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCourseGroup40Id).HasColumnName("int_course_group40_id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntDocumentStatus).HasColumnName("int_document_status");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntSignContentId).HasColumnName("int_sign_content_id");

                entity.Property(e => e.IntUserId).HasColumnName("int_user_id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

                entity.Property(e => e.VcNote).HasColumnName("vc_note");
            });

            modelBuilder.Entity<RefCourseDocumentType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_course_document_type_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_course_document_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCodeCourseType).HasColumnName("int_code_course_type");

                entity.Property(e => e.IntCodeDocumentType).HasColumnName("int_code_document_type");
            });

            modelBuilder.Entity<RefCourseGroupPremise>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_course_group_premises_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_course_group_premises");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntProviderPremiseId).HasColumnName("int_provider_premise_id");

                entity.Property(e => e.IntTrainingTypeId).HasColumnName("int_training_type_id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");
            });

            modelBuilder.Entity<RefCourseTypeFrCurr>(entity =>
            {
                entity.HasKey(e => new { e.IntCodeCourseTypeId, e.IntCodeCourseFrCurrId })
                    .HasName("ref_course_type_fr_curr_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_course_type_fr_curr");

                entity.Property(e => e.IntCodeCourseTypeId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("int_code_course_type_id");

                entity.Property(e => e.IntCodeCourseFrCurrId).HasColumnName("int_code_course_fr_curr_id");

                entity.Property(e => e.BoolValid)
                    .HasColumnName("bool_valid")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<RefDoctypeReqdoctype>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ref_doctype_reqdoctype");

                entity.HasIndex(e => new { e.IntRequestDocTypeId, e.IntDocTypeId }, "ref_doctype_reqdoctype_uk")
                    .IsUnique()
                    .HasFilter("([int_request_doc_type_id] IS NOT NULL AND [int_doc_type_id] IS NOT NULL)");

                entity.Property(e => e.IntDocTypeId).HasColumnName("int_doc_type_id");

                entity.Property(e => e.IntRequestDocTypeId).HasColumnName("int_request_doc_type_id");
            });

            modelBuilder.Entity<RefDocumentStatusLock>(entity =>
            {
                entity.HasKey(e => new { e.IntDocumentStatusId, e.IntCodeDocumentStatusLocksId, e.IntCodeValidityCheckTargetId })
                    .HasName("ref_document_status_locks_pk")
                    .IsClustered(false);

                entity.ToTable("ref_document_status_locks");

                entity.Property(e => e.IntDocumentStatusId).HasColumnName("int_document_status_id");

                entity.Property(e => e.IntCodeDocumentStatusLocksId).HasColumnName("int_code_document_status_locks_id");

                entity.Property(e => e.IntCodeValidityCheckTargetId).HasColumnName("int_code_validity_check_target_id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");
            });

            modelBuilder.Entity<RefExpertsCommission>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_experts_commissions_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_experts_commissions");

                entity.HasIndex(e => new { e.IntExpertId, e.IntExpCommId }, "ref_experts_commissions_int_expert_id_key")
                    .IsUnique()
                    .HasFilter("([int_expert_id] IS NOT NULL AND [int_exp_comm_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsChairman).HasColumnName("bool_is_chairman");

                entity.Property(e => e.DtExpertProtokolDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_expert_protokol_date");

                entity.Property(e => e.IntCommissionInstitutionType).HasColumnName("int_commission_institution_type");

                entity.Property(e => e.IntExpCommId).HasColumnName("int_exp_comm_id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.VcExpertInstitution)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_institution");

                entity.Property(e => e.VcExpertOccupation)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_occupation");

                entity.Property(e => e.VcExpertProtokol)
                    .HasMaxLength(50)
                    .HasColumnName("vc_expert_protokol");
            });

            modelBuilder.Entity<RefExpertsDoi>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_experts_doi_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_experts_doi");

                entity.HasIndex(e => new { e.IntExpertId, e.IntDoiCommId }, "ref_experts_doi_int_expert_id_key")
                    .IsUnique()
                    .HasFilter("([int_expert_id] IS NOT NULL AND [int_doi_comm_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntDoiCommId).HasColumnName("int_doi_comm_id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntWgdoiFunctionId).HasColumnName("int_wgdoi_function_id");
            });

            modelBuilder.Entity<RefExpertsType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_experts_types_id_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_experts_types");

                entity.HasIndex(e => new { e.IntExpertId, e.IntExpertTypeId }, "ref_experts_types_int_expert_id_key")
                    .IsUnique()
                    .HasFilter("([int_expert_id] IS NOT NULL AND [int_expert_type_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntExpertTypeId).HasColumnName("int_expert_type_id");

                entity.Property(e => e.VcPosition).HasColumnName("vc_position");
            });

            modelBuilder.Entity<RefExpertsVetArea>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_experts_vet_area_id_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_experts_vet_area");

                entity.HasIndex(e => new { e.IntExpertId, e.IntVetAreaId }, "ref_experts_vet_area_int_expert_id_key")
                    .IsUnique()
                    .HasFilter("([int_expert_id] IS NOT NULL AND [int_vet_area_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");
            });

            modelBuilder.Entity<RefFrCurrEdForm>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_fr_curr_ed_form_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_fr_curr_ed_form");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCodeCourseEdForm).HasColumnName("int_code_course_ed_form");

                entity.Property(e => e.IntCodeCourseFrCurrId).HasColumnName("int_code_course_fr_curr_id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");
            });

            modelBuilder.Entity<RefFrCurrEducLevel>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_fr_curr_educ_level_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_fr_curr_educ_level");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCodeCourseFrCurrId).HasColumnName("int_code_course_fr_curr_id");

                entity.Property(e => e.IntCodeEducation).HasColumnName("int_code_education");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");
            });

            modelBuilder.Entity<RefFrCurrQualLevel>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_fr_curr_qual_level_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_fr_curr_qual_level");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCodeCourseFrCurrId).HasColumnName("int_code_course_fr_curr_id");

                entity.Property(e => e.IntCodeQualLevel).HasColumnName("int_code_qual_level");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");
            });

            modelBuilder.Entity<RefMainExpert>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_main_experts_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_main_experts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetArea).HasColumnName("int_vet_area");
            });

            modelBuilder.Entity<RefMainExpertCommission>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_main_expert_commissions_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_main_expert_commissions");

                entity.HasIndex(e => new { e.IntProviderId, e.IntExpertCommissionId }, "ref_main_expert_commissions_provider_commission_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [int_expert_commission_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntExpertCommissionId).HasColumnName("int_expert_commission_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");
            });

            modelBuilder.Entity<RefProcedureStep>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_procedure_steps_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_procedure_steps");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Iorder).HasColumnName("iorder");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

                entity.Property(e => e.Label)
                    .HasMaxLength(255)
                    .HasColumnName("label");

                entity.Property(e => e.LabelEn)
                    .HasMaxLength(255)
                    .HasColumnName("label_en");

                entity.Property(e => e.LabelReg)
                    .HasMaxLength(300)
                    .HasColumnName("label_reg");

                entity.Property(e => e.LabelRegEn)
                    .HasMaxLength(300)
                    .HasColumnName("label_reg_en");

                entity.Property(e => e.ProcedureId).HasColumnName("procedure_id");

                entity.Property(e => e.StepId).HasColumnName("step_id");
            });

            modelBuilder.Entity<RefProcedureStepStage>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_procedure_step_stages_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_procedure_step_stages");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Iorder).HasColumnName("iorder");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

                entity.Property(e => e.Label)
                    .HasMaxLength(255)
                    .HasColumnName("label");

                entity.Property(e => e.ProcedureId).HasColumnName("procedure_id");

                entity.Property(e => e.StageId).HasColumnName("stage_id");

                entity.Property(e => e.StepId).HasColumnName("step_id");
            });

            modelBuilder.Entity<RefProceduresDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_procedures_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_procedures_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntProceduresDocumentsId).HasColumnName("int_procedures_documents_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtProceduresDocumentsFile).HasColumnName("txt_procedures_documents_file");

                entity.Property(e => e.TxtProceduresDocumentsNotes).HasColumnName("txt_procedures_documents_notes");
            });

            modelBuilder.Entity<RefProviderPremisesSpeciality>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_provider_premises_specialities_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_provider_premises_specialities");

                entity.HasIndex(e => new { e.IntProviderPremiseId, e.IntProviderSpecialityId }, "ref_provider_premises_specialities_int_provider_premise_id_key")
                    .IsUnique()
                    .HasFilter("([int_provider_premise_id] IS NOT NULL AND [int_provider_speciality_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntProviderPremiseId).HasColumnName("int_provider_premise_id");

                entity.Property(e => e.IntProviderPremiseSpecialityCorrespondence).HasColumnName("int_provider_premise_speciality_correspondence");

                entity.Property(e => e.IntProviderPremiseSpecialityUsage).HasColumnName("int_provider_premise_speciality_usage");

                entity.Property(e => e.IntProviderSpecialityId).HasColumnName("int_provider_speciality_id");
            });

            modelBuilder.Entity<RefRequestDocStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_request_doc_status_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_request_doc_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntRequestDocStatusId).HasColumnName("int_request_doc_status_id");

                entity.Property(e => e.IntRequestId).HasColumnName("int_request_id");

                entity.Property(e => e.Ts).HasColumnName("ts");
            });

            modelBuilder.Entity<RefRequestDocType>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_request_doc_type_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_request_doc_type");

                entity.HasIndex(e => new { e.IntRequestDocTypeId, e.IntRequestId }, "ref_request_doc_type_int_request_doc_type_id_int_request_i_key")
                    .IsUnique()
                    .HasFilter("([int_request_doc_type_id] IS NOT NULL AND [int_request_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntDocCount).HasColumnName("int_doc_count");

                entity.Property(e => e.IntNapooRequestId).HasColumnName("int_napoo_request_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntRequestDocTypeId).HasColumnName("int_request_doc_type_id");

                entity.Property(e => e.IntRequestId).HasColumnName("int_request_id");
            });

            modelBuilder.Entity<RefRoleAclAction>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_role_acl_actions_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_role_acl_actions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntAclActionId).HasColumnName("int_acl_action_id");

                entity.Property(e => e.IntRoleId).HasColumnName("int_role_id");
            });

            modelBuilder.Entity<RefRoleGroup>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_role_groups_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_role_groups");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntGroupId).HasColumnName("int_group_id");

                entity.Property(e => e.IntRoleId).HasColumnName("int_role_id");
            });

            modelBuilder.Entity<RefRoleUser>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_role_users_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_role_users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntRoleId).HasColumnName("int_role_id");

                entity.Property(e => e.IntUserId).HasColumnName("int_user_id");
            });

            modelBuilder.Entity<RefTrainersCourse>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_trainers_courses_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_trainers_courses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.IntTrainingTypeId).HasColumnName("int_training_type_id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");
            });

            modelBuilder.Entity<RefVetSpecialitiesNkpd>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_vet_specialities_nkpds_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_vet_specialities_nkpds");

                entity.HasIndex(e => new { e.IntVetSpecialityId, e.IntNkpdId }, "unique_int_nkpd_speciality")
                    .IsUnique()
                    .HasFilter("([int_vet_speciality_id] IS NOT NULL AND [int_nkpd_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntNkpdId).HasColumnName("int_nkpd_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");
            });

            modelBuilder.Entity<RefVisit>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_visits_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_visits");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtVisitDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_visit_date");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVisitNo).HasColumnName("int_visit_no");

                entity.Property(e => e.IntVisitResultId).HasColumnName("int_visit_result_id");

                entity.Property(e => e.VcVisitNotes).HasColumnName("vc_visit_notes");

                entity.Property(e => e.VcVisitProtDate)
                    .HasColumnType("date")
                    .HasColumnName("vc_visit_prot_date");

                entity.Property(e => e.VcVisitProtNo)
                    .HasMaxLength(50)
                    .HasColumnName("vc_visit_prot_no");

                entity.Property(e => e.VcVisitTheme)
                    .HasMaxLength(255)
                    .HasColumnName("vc_visit_theme");
            });

            modelBuilder.Entity<RefVisitsExpert>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("ref_visits_experts_pkey")
                    .IsClustered(false);

                entity.ToTable("ref_visits_experts");

                entity.HasIndex(e => new { e.IntVisitId, e.IntExpertId }, "ref_visits_experts_int_visit_id_key")
                    .IsUnique()
                    .HasFilter("([int_visit_id] IS NOT NULL AND [int_expert_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IntVisitId).HasColumnName("int_visit_id");
            });

            modelBuilder.Entity<ReportClient>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("report_clients");

                entity.Property(e => e.IntAssignTypeId).HasColumnName("int_assign_type_id");

                entity.Property(e => e.IntBirthYear).HasColumnName("int_birth_year");

                entity.Property(e => e.IntCfinishedTypeId).HasColumnName("int_cfinished_type_id");

                entity.Property(e => e.IntClientGender).HasColumnName("int_client_gender");

                entity.Property(e => e.IntClientStatus).HasColumnName("int_client_status");

                entity.Property(e => e.IntCourseDuration).HasColumnName("int_course_duration");

                entity.Property(e => e.IntCourseEdFormId).HasColumnName("int_course_ed_form_id");

                entity.Property(e => e.IntCourseFrCurrId).HasColumnName("int_course_fr_curr_id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntCourseMeasureTypeId).HasColumnName("int_course_measure_type_id");

                entity.Property(e => e.IntCourseTypeId).HasColumnName("int_course_type_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntNutsId).HasColumnName("int_nuts_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetGroupId).HasColumnName("int_vet_group_id");

                entity.Property(e => e.IntVetProfessionId).HasColumnName("int_vet_profession_id");

                entity.Property(e => e.IntVetQualificationLevel).HasColumnName("int_vet_qualification_level");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.NumCourseCost)
                    .HasColumnType("numeric(10, 2)")
                    .HasColumnName("num_course_cost");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");
            });

            modelBuilder.Entity<ReportCourse>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("report_courses");

                entity.Property(e => e.IntAssignTypeId).HasColumnName("int_assign_type_id");

                entity.Property(e => e.IntCourseDuration).HasColumnName("int_course_duration");

                entity.Property(e => e.IntCourseEdFormId).HasColumnName("int_course_ed_form_id");

                entity.Property(e => e.IntCourseFrCurrId).HasColumnName("int_course_fr_curr_id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntCourseMeasureTypeId).HasColumnName("int_course_measure_type_id");

                entity.Property(e => e.IntCourseStatusId).HasColumnName("int_course_status_id");

                entity.Property(e => e.IntCourseTypeId).HasColumnName("int_course_type_id");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntNutsId).HasColumnName("int_nuts_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetGroupId).HasColumnName("int_vet_group_id");

                entity.Property(e => e.IntVetProfessionId).HasColumnName("int_vet_profession_id");

                entity.Property(e => e.IntVetQualificationLevel).HasColumnName("int_vet_qualification_level");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.NumCourseCost)
                    .HasColumnType("numeric(10, 2)")
                    .HasColumnName("num_course_cost");
            });

            modelBuilder.Entity<ReportCurriculum>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("report_curricula");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntNutsId).HasColumnName("int_nuts_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntProviderOwnershipId).HasColumnName("int_provider_ownership_id");

                entity.Property(e => e.IntSpecialityCurriculumUpdateReasonId).HasColumnName("int_speciality_curriculum_update_reason_id");

                entity.Property(e => e.IntSpecialityId).HasColumnName("int_speciality_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetGroupId).HasColumnName("int_vet_group_id");

                entity.Property(e => e.IntVetProfessionId).HasColumnName("int_vet_profession_id");

                entity.Property(e => e.IntVetQualificationLevel).HasColumnName("int_vet_qualification_level");

                entity.Property(e => e.IntYear).HasColumnName("int_year");
            });

            modelBuilder.Entity<ReportPremise>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("report_premises");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntNutsId).HasColumnName("int_nuts_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntProviderOwnershipId).HasColumnName("int_provider_ownership_id");

                entity.Property(e => e.IntProviderPremiseNo).HasColumnName("int_provider_premise_no");

                entity.Property(e => e.IntProviderPremiseSpecialityCorrespondence).HasColumnName("int_provider_premise_speciality_correspondence");

                entity.Property(e => e.IntProviderPremiseStatus).HasColumnName("int_provider_premise_status");

                entity.Property(e => e.IntSpecialityId).HasColumnName("int_speciality_id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.VcProviderOwner)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_owner");
            });

            modelBuilder.Entity<ReportProvider>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("report_providers");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntLicenceNumber).HasColumnName("int_licence_number");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntNumClientsA).HasColumnName("int_num_clients_a");

                entity.Property(e => e.IntNumClientsB).HasColumnName("int_num_clients_b");

                entity.Property(e => e.IntNumClientsC).HasColumnName("int_num_clients_c");

                entity.Property(e => e.IntNumCoursesA).HasColumnName("int_num_courses_a");

                entity.Property(e => e.IntNumCoursesB).HasColumnName("int_num_courses_b");

                entity.Property(e => e.IntNutsId).HasColumnName("int_nuts_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.IntProviderBulstat)
                    .HasMaxLength(20)
                    .HasColumnName("int_provider_bulstat");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntProviderOwnershipId).HasColumnName("int_provider_ownership_id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.VcProviderOwner)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_owner");
            });

            modelBuilder.Entity<ReportTrainersQualification>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("report_trainers_qualification");

                entity.Property(e => e.IntBirthYear).HasColumnName("int_birth_year");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntGenderId).HasColumnName("int_gender_id");

                entity.Property(e => e.IntMunicipalityId).HasColumnName("int_municipality_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntNutsId).HasColumnName("int_nuts_id");

                entity.Property(e => e.IntOblId).HasColumnName("int_obl_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntProviderOwnershipId).HasColumnName("int_provider_ownership_id");

                entity.Property(e => e.IntQualificationDuration).HasColumnName("int_qualification_duration");

                entity.Property(e => e.IntQualificationTypeId).HasColumnName("int_qualification_type_id");

                entity.Property(e => e.IntTcontractTypeId).HasColumnName("int_tcontract_type_id");

                entity.Property(e => e.IntTqualificationTypeId).HasColumnName("int_tqualification_type_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetGroupId).HasColumnName("int_vet_group_id");

                entity.Property(e => e.IntVetProfessionId).HasColumnName("int_vet_profession_id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");
            });

            modelBuilder.Entity<Ret>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ret");

                entity.Property(e => e.Column).HasColumnName("?column?");
            });

            modelBuilder.Entity<SysAcl>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("sys_acl_pkey")
                    .IsClustered(false);

                entity.ToTable("sys_acl");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcAcl).HasColumnName("vc_acl");

                entity.Property(e => e.VcItemName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_item_name");

                entity.Property(e => e.VcObjectManager)
                    .HasMaxLength(255)
                    .HasColumnName("vc_object_manager");
            });

            modelBuilder.Entity<SysLock>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("sys_locks");

                entity.HasIndex(e => e.LockId, "i_sys_locks_lock_id");

                entity.HasIndex(e => e.SessionId, "i_sys_locks_session_id");

                entity.HasIndex(e => e.Ts, "i_sys_locks_ts");

                entity.Property(e => e.LockId)
                    .HasMaxLength(255)
                    .HasColumnName("lock_id");

                entity.Property(e => e.SessionId)
                    .HasMaxLength(255)
                    .HasColumnName("session_id");

                entity.Property(e => e.Ts).HasColumnName("ts");
            });

            modelBuilder.Entity<SysMailLog>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("sys_mail_log_pkey")
                    .IsClustered(false);

                entity.ToTable("sys_mail_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtMailDate).HasColumnName("dt_mail_date");

                entity.Property(e => e.IntMailType).HasColumnName("int_mail_type");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntUserId).HasColumnName("int_user_id");

                entity.Property(e => e.VcMailText).HasColumnName("vc_mail_text");
            });

            modelBuilder.Entity<SysOperationLog>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("sys_operation_log_pkey")
                    .IsClustered(false);

                entity.ToTable("sys_operation_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtDateTime).HasColumnName("dt_date_time");

                entity.Property(e => e.IntOperationId).HasColumnName("int_operation_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntUserId).HasColumnName("int_user_id");

                entity.Property(e => e.VcAdditionalInfo).HasColumnName("vc_additional_info");
            });

            modelBuilder.Entity<SysSignLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("sys_sign_log");

                entity.Property(e => e.DtEvent)
                    .HasColumnName("dt_event")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCourseGroup).HasColumnName("int_course_group");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntUserId).HasColumnName("int_user_id");

                entity.Property(e => e.VcCert).HasColumnName("vc_cert");

                entity.Property(e => e.VcCertEmail)
                    .HasMaxLength(200)
                    .HasColumnName("vc_cert_email");

                entity.Property(e => e.VcCertValidFrom)
                    .HasMaxLength(100)
                    .HasColumnName("vc_cert_valid_from");

                entity.Property(e => e.VcCertValidTo)
                    .HasMaxLength(100)
                    .HasColumnName("vc_cert_valid_to");

                entity.Property(e => e.VcError)
                    .HasMaxLength(500)
                    .HasColumnName("vc_error");

                entity.Property(e => e.VcIdNumber)
                    .HasMaxLength(100)
                    .HasColumnName("vc_id_number");
            });

            modelBuilder.Entity<Systransliterate>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("systransliterate");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Vcletterbg)
                    .HasMaxLength(5)
                    .HasColumnName("vcletterbg");

                entity.Property(e => e.Vcletterlat)
                    .HasMaxLength(5)
                    .HasColumnName("vcletterlat");
            });

            modelBuilder.Entity<TbAclAction>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_acl_actions_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_acl_actions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcActionName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_action_name");
            });

            modelBuilder.Entity<TbAnnualInfo>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_annual_info_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_annual_info");

                entity.HasIndex(e => new { e.IntYear, e.IntProviderId }, "tb_annual_info_int_year_int_provider_id_key")
                    .IsUnique()
                    .HasFilter("([int_year] IS NOT NULL AND [int_provider_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtTimestamp)
                    .HasColumnName("dt_timestamp")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntStatus)
                    .HasColumnName("int_status")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.VcEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_email");

                entity.Property(e => e.VcName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_name");

                entity.Property(e => e.VcPhone)
                    .HasMaxLength(255)
                    .HasColumnName("vc_phone");

                entity.Property(e => e.VcPosition)
                    .HasMaxLength(255)
                    .HasColumnName("vc_position");
            });

            modelBuilder.Entity<TbArchProcedureProvider>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_providers_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_providers");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.BoolIsBrra)
                    .HasColumnName("bool_is_brra")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtFilingSystemDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_filing_system_date");

                entity.Property(e => e.DtLicenceData)
                    .HasColumnType("date")
                    .HasColumnName("dt_licence_data");

                entity.Property(e => e.IntCodeCpoManagementVersion).HasColumnName("int_code_cpo_management_version");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntLicenceNumber).HasColumnName("int_licence_number");

                entity.Property(e => e.IntLicenceProtNo).HasColumnName("int_licence_prot_no");

                entity.Property(e => e.IntLocalGroupId).HasColumnName("int_local_group_id");

                entity.Property(e => e.IntOperationId).HasColumnName("int_operation_id");

                entity.Property(e => e.IntProcedureTypeId).HasColumnName("int_procedure_type_id");

                entity.Property(e => e.IntProviderBulstat)
                    .HasMaxLength(20)
                    .HasColumnName("int_provider_bulstat");

                entity.Property(e => e.IntProviderContactPersEkatteId).HasColumnName("int_provider_contact_pers_ekatte_id");

                entity.Property(e => e.IntProviderOwnershipId).HasColumnName("int_provider_ownership_id");

                entity.Property(e => e.IntProviderRegistrationId).HasColumnName("int_provider_registration_id");

                entity.Property(e => e.IntProviderStatusId).HasColumnName("int_provider_status_id");

                entity.Property(e => e.IntReceiveTypeId).HasColumnName("int_receive_type_id");

                entity.Property(e => e.IntStartedProcedureProgress).HasColumnName("int_started_procedure_progress");

                entity.Property(e => e.IsReturned).HasColumnName("is_returned");

                entity.Property(e => e.ProcedureId).HasColumnName("procedure_id");

                entity.Property(e => e.StageId).HasColumnName("stage_id");

                entity.Property(e => e.StepId).HasColumnName("step_id");

                entity.Property(e => e.Ts).HasColumnName("ts");

                entity.Property(e => e.VcFilingSystemNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_filing_system_number");

                entity.Property(e => e.VcProviderAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_address");

                entity.Property(e => e.VcProviderContactPers)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers");

                entity.Property(e => e.VcProviderContactPersAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_address");

                entity.Property(e => e.VcProviderContactPersEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_email");

                entity.Property(e => e.VcProviderContactPersFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_fax");

                entity.Property(e => e.VcProviderContactPersPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone1");

                entity.Property(e => e.VcProviderContactPersPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone2");

                entity.Property(e => e.VcProviderContactPersZipcode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_provider_contact_pers_zipcode");

                entity.Property(e => e.VcProviderEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_email");

                entity.Property(e => e.VcProviderFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_fax");

                entity.Property(e => e.VcProviderManager)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_manager");

                entity.Property(e => e.VcProviderName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_name");

                entity.Property(e => e.VcProviderOwner)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_owner");

                entity.Property(e => e.VcProviderPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone1");

                entity.Property(e => e.VcProviderPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone2");

                entity.Property(e => e.VcProviderProfile).HasColumnName("vc_provider_profile");

                entity.Property(e => e.VcProviderWeb)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_web");

                entity.Property(e => e.VcZipCode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_zip_code");
            });

            modelBuilder.Entity<TbArchProcedureProviderPremise>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_provider_premises_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_provider_premises");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.BoolIsVisited).HasColumnName("bool_is_visited");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntProviderPremiseEkatte).HasColumnName("int_provider_premise_ekatte");

                entity.Property(e => e.IntProviderPremiseNo).HasColumnName("int_provider_premise_no");

                entity.Property(e => e.IntProviderPremiseStatus).HasColumnName("int_provider_premise_status");

                entity.Property(e => e.TxtProviderPremiseAddress).HasColumnName("txt_provider_premise_address");

                entity.Property(e => e.TxtProviderPremiseName).HasColumnName("txt_provider_premise_name");

                entity.Property(e => e.TxtProviderPremiseNotes).HasColumnName("txt_provider_premise_notes");
            });

            modelBuilder.Entity<TbArchProcedureProviderPremisesRoom>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_provider_premises_rooms_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_provider_premises_rooms");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.IntPremiseId).HasColumnName("int_premise_id");

                entity.Property(e => e.IntProviderPremiseRoomArea).HasColumnName("int_provider_premise_room_area");

                entity.Property(e => e.IntProviderPremiseRoomNo).HasColumnName("int_provider_premise_room_no");

                entity.Property(e => e.IntProviderPremiseRoomType).HasColumnName("int_provider_premise_room_type");

                entity.Property(e => e.IntProviderPremiseRoomUsage).HasColumnName("int_provider_premise_room_usage");

                entity.Property(e => e.IntProviderPremiseRoomWorkplaces).HasColumnName("int_provider_premise_room_workplaces");

                entity.Property(e => e.TxtProviderPremiseRoomEquipment).HasColumnName("txt_provider_premise_room_equipment");

                entity.Property(e => e.TxtProviderPremiseRoomName).HasColumnName("txt_provider_premise_room_name");
            });

            modelBuilder.Entity<TbArchProcedureProviderSpecialitiesCurriculum>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_provider_specialities_curriculum_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_provider_specialities_curriculum");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.BoolIsUpdated)
                    .HasColumnName("bool_is_updated")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtUpdateDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_update_date");

                entity.Property(e => e.IntProcedureProviderSpecialityId).HasColumnName("int_procedure_provider_speciality_id");

                entity.Property(e => e.IntSpecialityCurriculumUpdateReasonId).HasColumnName("int_speciality_curriculum_update_reason_id");

                entity.Property(e => e.OidFile).HasColumnName("oid_file");

                entity.Property(e => e.VcFileName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_file_name");
            });

            modelBuilder.Entity<TbArchProcedureProvidersCipoManagement>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_providers_cipo_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_providers_cipo_management");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.IntCipoManagementId).HasColumnName("int_cipo_management_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtCipoManagementNotes).HasColumnName("txt_cipo_management_notes");
            });

            modelBuilder.Entity<TbArchProcedureProvidersCpoManagement>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_providers_cpo_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_providers_cpo_management");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.IntCpoManagementId).HasColumnName("int_cpo_management_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtCpoManagementNotes).HasColumnName("txt_cpo_management_notes");
            });

            modelBuilder.Entity<TbArchProcedureProvidersDocumentsManagement>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_providers_documents_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_providers_documents_management");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.IntDocumentsManagementId).HasColumnName("int_documents_management_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TsDocument)
                    .HasColumnName("ts_document")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TxtDocumentsManagementFile).HasColumnName("txt_documents_management_file");

                entity.Property(e => e.TxtDocumentsManagementTitle).HasColumnName("txt_documents_management_title");
            });

            modelBuilder.Entity<TbArchProcedureProvidersSpeciality>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_providers_specialities_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_providers_specialities");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.BoolIsAdded)
                    .HasColumnName("bool_is_added")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BoolIsApproved).HasColumnName("bool_is_approved");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.TxtSpecialityFile).HasColumnName("txt_speciality_file");

                entity.Property(e => e.TxtSpecialityNotes).HasColumnName("txt_speciality_notes");
            });

            modelBuilder.Entity<TbArchProcedureTrainer>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_trainers_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_trainers");

                entity.HasIndex(e => new { e.IntProviderId, e.VcEgn, e.IntStartedProcedureId }, "tb_arch_procedure_trainers_int_provider_egn_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL AND [int_started_procedure_id] IS NOT NULL)");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.BoolIsAndragog)
                    .HasColumnName("bool_is_andragog")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DtTcontractDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_tcontract_date");

                entity.Property(e => e.IntBirthYear).HasColumnName("int_birth_year");

                entity.Property(e => e.IntEducationId).HasColumnName("int_education_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntGenderId).HasColumnName("int_gender_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntTcontractTypeId).HasColumnName("int_tcontract_type_id");

                entity.Property(e => e.TxtEducationAcademicNotes).HasColumnName("txt_education_academic_notes");

                entity.Property(e => e.TxtEducationCertificateNotes).HasColumnName("txt_education_certificate_notes");

                entity.Property(e => e.TxtEducationSpecialityNotes).HasColumnName("txt_education_speciality_notes");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");

                entity.Property(e => e.VcEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_email");

                entity.Property(e => e.VcTrainerFamilyName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_trainer_family_name");

                entity.Property(e => e.VcTrainerFirstName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_trainer_first_name");

                entity.Property(e => e.VcTrainerSecondName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_trainer_second_name");
            });

            modelBuilder.Entity<TbArchProcedureTrainerDocument>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_trainer_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_trainer_documents");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.TxtDocumentsManagementFile).HasColumnName("txt_documents_management_file");

                entity.Property(e => e.TxtDocumentsManagementTitle).HasColumnName("txt_documents_management_title");
            });

            modelBuilder.Entity<TbArchProcedureTrainerProfile>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_trainer_profiles_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_trainer_profiles");

                entity.HasIndex(e => new { e.IntTrainerId, e.IntVetAreaId, e.IntVetSpecialityId }, "tb_arch_procedure_trainer_id_int_vet_speciality_id_key")
                    .IsUnique()
                    .HasFilter("([int_trainer_id] IS NOT NULL AND [int_vet_area_id] IS NOT NULL AND [int_vet_speciality_id] IS NOT NULL)");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.BoolVetAreaPractice).HasColumnName("bool_vet_area_practice");

                entity.Property(e => e.BoolVetAreaQualified).HasColumnName("bool_vet_area_qualified");

                entity.Property(e => e.BoolVetAreaTheory).HasColumnName("bool_vet_area_theory");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");
            });

            modelBuilder.Entity<TbArchProcedureTrainerQualification>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.IntStartedProcedureId })
                    .HasName("tb_arch_procedure_trainer_qualifications_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_procedure_trainer_qualifications");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntStartedProcedureId).HasColumnName("int_started_procedure_id");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntProfessionId).HasColumnName("int_profession_id");

                entity.Property(e => e.IntQualificationDuration).HasColumnName("int_qualification_duration");

                entity.Property(e => e.IntQualificationTypeId).HasColumnName("int_qualification_type_id");

                entity.Property(e => e.IntTqualificationTypeId).HasColumnName("int_tqualification_type_id");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.TxtQualificationName).HasColumnName("txt_qualification_name");
            });

            modelBuilder.Entity<TbArchProvider>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_arch_providers_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_arch_providers");

                entity.HasIndex(e => e.IntProviderBulstat, "tb_arch_providers_int_provider_bulstat_key")
                    .IsUnique()
                    .HasFilter("([int_provider_bulstat] IS NOT NULL)");

                entity.HasIndex(e => new { e.IntProviderNo, e.DtDecisionDate }, "tb_arch_providers_int_provider_no_key")
                    .IsUnique()
                    .HasFilter("([int_provider_no] IS NOT NULL AND [dt_decision_date] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtDecisionDate).HasColumnName("dt_decision_date");

                entity.Property(e => e.IntCandidateTypeId).HasColumnName("int_candidate_type_id");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntLicenceNumber).HasColumnName("int_licence_number");

                entity.Property(e => e.IntLocalGroupId).HasColumnName("int_local_group_id");

                entity.Property(e => e.IntOperationId).HasColumnName("int_operation_id");

                entity.Property(e => e.IntProviderBulstat)
                    .HasMaxLength(20)
                    .HasColumnName("int_provider_bulstat");

                entity.Property(e => e.IntProviderContactPersEkatteId).HasColumnName("int_provider_contact_pers_ekatte_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntProviderNo).HasColumnName("int_provider_no");

                entity.Property(e => e.IntProviderOwnershipId).HasColumnName("int_provider_ownership_id");

                entity.Property(e => e.IntProviderStatusId).HasColumnName("int_provider_status_id");

                entity.Property(e => e.VcProviderAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_address");

                entity.Property(e => e.VcProviderContactPers)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers");

                entity.Property(e => e.VcProviderContactPersAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_address");

                entity.Property(e => e.VcProviderContactPersEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_email");

                entity.Property(e => e.VcProviderContactPersFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_fax");

                entity.Property(e => e.VcProviderContactPersPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone1");

                entity.Property(e => e.VcProviderContactPersPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone2");

                entity.Property(e => e.VcProviderContactPersZipcode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_provider_contact_pers_zipcode");

                entity.Property(e => e.VcProviderEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_email");

                entity.Property(e => e.VcProviderFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_fax");

                entity.Property(e => e.VcProviderManager)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_manager");

                entity.Property(e => e.VcProviderOwner)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_owner");

                entity.Property(e => e.VcProviderPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone1");

                entity.Property(e => e.VcProviderPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone2");

                entity.Property(e => e.VcProviderWeb)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_web");

                entity.Property(e => e.VcZipCode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_zip_code");
            });

            modelBuilder.Entity<TbCandidateProvider>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_providers_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_providers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsBrra)
                    .HasColumnName("bool_is_brra")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtFilingSystemDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_filing_system_date");

                entity.Property(e => e.DtLicenceData)
                    .HasColumnType("date")
                    .HasColumnName("dt_licence_data");

                entity.Property(e => e.IntCandidateTypeId).HasColumnName("int_candidate_type_id");

                entity.Property(e => e.IntCodeCpoManagementVersion).HasColumnName("int_code_cpo_management_version");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntLicenceNumber).HasColumnName("int_licence_number");

                entity.Property(e => e.IntLicenceProtNo).HasColumnName("int_licence_prot_no");

                entity.Property(e => e.IntLocalGroupId).HasColumnName("int_local_group_id");

                entity.Property(e => e.IntOperationId).HasColumnName("int_operation_id");

                entity.Property(e => e.IntProviderBulstat)
                    .HasMaxLength(20)
                    .HasColumnName("int_provider_bulstat");

                entity.Property(e => e.IntProviderContactPersEkatteId).HasColumnName("int_provider_contact_pers_ekatte_id");

                entity.Property(e => e.IntProviderOwnershipId).HasColumnName("int_provider_ownership_id");

                entity.Property(e => e.IntProviderRegistrationId).HasColumnName("int_provider_registration_id");

                entity.Property(e => e.IntProviderStatusId).HasColumnName("int_provider_status_id");

                entity.Property(e => e.IntReceiveTypeId).HasColumnName("int_receive_type_id");

                entity.Property(e => e.IntStartedProcedureProgress).HasColumnName("int_started_procedure_progress");

                entity.Property(e => e.IntStartedProcedures).HasColumnName("int_started_procedures");

                entity.Property(e => e.IsReturned).HasColumnName("is_returned");

                entity.Property(e => e.ProcedureId).HasColumnName("procedure_id");

                entity.Property(e => e.StageId).HasColumnName("stage_id");

                entity.Property(e => e.StepId).HasColumnName("step_id");

                entity.Property(e => e.VcFilingSystemNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_filing_system_number");

                entity.Property(e => e.VcProviderAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_address");

                entity.Property(e => e.VcProviderContactPers)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers");

                entity.Property(e => e.VcProviderContactPersAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_address");

                entity.Property(e => e.VcProviderContactPersEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_email");

                entity.Property(e => e.VcProviderContactPersFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_fax");

                entity.Property(e => e.VcProviderContactPersPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone1");

                entity.Property(e => e.VcProviderContactPersPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone2");

                entity.Property(e => e.VcProviderContactPersZipcode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_provider_contact_pers_zipcode");

                entity.Property(e => e.VcProviderEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_email");

                entity.Property(e => e.VcProviderFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_fax");

                entity.Property(e => e.VcProviderManager)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_manager");

                entity.Property(e => e.VcProviderName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_name");

                entity.Property(e => e.VcProviderOwner)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_owner");

                entity.Property(e => e.VcProviderPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone1");

                entity.Property(e => e.VcProviderPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone2");

                entity.Property(e => e.VcProviderProfile).HasColumnName("vc_provider_profile");

                entity.Property(e => e.VcProviderWeb)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_web");

                entity.Property(e => e.VcZipCode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_zip_code");
            });

            modelBuilder.Entity<TbCandidateProviderPremise>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_provider_premises_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_provider_premises");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsVisited).HasColumnName("bool_is_visited");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntProviderPremiseEkatte).HasColumnName("int_provider_premise_ekatte");

                entity.Property(e => e.IntProviderPremiseNo).HasColumnName("int_provider_premise_no");

                entity.Property(e => e.IntProviderPremiseStatus).HasColumnName("int_provider_premise_status");

                entity.Property(e => e.TxtProviderPremiseAddress).HasColumnName("txt_provider_premise_address");

                entity.Property(e => e.TxtProviderPremiseName).HasColumnName("txt_provider_premise_name");

                entity.Property(e => e.TxtProviderPremiseNotes).HasColumnName("txt_provider_premise_notes");
            });

            modelBuilder.Entity<TbCandidateProviderPremisesRoom>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_provider_premises_rooms_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_provider_premises_rooms");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntPremiseId).HasColumnName("int_premise_id");

                entity.Property(e => e.IntProviderPremiseRoomArea).HasColumnName("int_provider_premise_room_area");

                entity.Property(e => e.IntProviderPremiseRoomNo).HasColumnName("int_provider_premise_room_no");

                entity.Property(e => e.IntProviderPremiseRoomType).HasColumnName("int_provider_premise_room_type");

                entity.Property(e => e.IntProviderPremiseRoomUsage).HasColumnName("int_provider_premise_room_usage");

                entity.Property(e => e.IntProviderPremiseRoomWorkplaces).HasColumnName("int_provider_premise_room_workplaces");

                entity.Property(e => e.TxtProviderPremiseRoomEquipment).HasColumnName("txt_provider_premise_room_equipment");

                entity.Property(e => e.TxtProviderPremiseRoomName).HasColumnName("txt_provider_premise_room_name");
            });

            modelBuilder.Entity<TbCandidateProviderSpecialitiesCurriculum>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_provider_specialities_curriculum_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_provider_specialities_curriculum");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsUpdated)
                    .HasColumnName("bool_is_updated")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtUpdateDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_update_date");

                entity.Property(e => e.IntCandidateProviderSpecialityId).HasColumnName("int_candidate_provider_speciality_id");

                entity.Property(e => e.IntSpecialityCurriculumUpdateReasonId).HasColumnName("int_speciality_curriculum_update_reason_id");

                entity.Property(e => e.OidFile).HasColumnName("oid_file");

                entity.Property(e => e.VcFileName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_file_name");
            });

            modelBuilder.Entity<TbCandidateProvidersCipoManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_providers_cipo_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_providers_cipo_management");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCipoManagementId).HasColumnName("int_cipo_management_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtCipoManagementNotes).HasColumnName("txt_cipo_management_notes");
            });

            modelBuilder.Entity<TbCandidateProvidersCpoManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_providers_cpo_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_providers_cpo_management");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCpoManagementId).HasColumnName("int_cpo_management_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtCpoManagementNotes).HasColumnName("txt_cpo_management_notes");
            });

            modelBuilder.Entity<TbCandidateProvidersDocumentsManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_providers_documents_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_providers_documents_management");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntDocumentsManagementId).HasColumnName("int_documents_management_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TsDocument)
                    .HasColumnName("ts_document")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TxtDocumentsManagementFile).HasColumnName("txt_documents_management_file");

                entity.Property(e => e.TxtDocumentsManagementTitle).HasColumnName("txt_documents_management_title");
            });

            modelBuilder.Entity<TbCandidateProvidersSpeciality>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_providers_specialities_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_providers_specialities");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsAdded)
                    .HasColumnName("bool_is_added")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BoolIsApproved).HasColumnName("bool_is_approved");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetSpecReplaced).HasColumnName("int_vet_spec_replaced");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.TxtSpecialityFile).HasColumnName("txt_speciality_file");

                entity.Property(e => e.TxtSpecialityNotes).HasColumnName("txt_speciality_notes");
            });

            modelBuilder.Entity<TbCandidateProvidersState>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_providers_state_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_providers_state");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtCandidateProvidersStateChange)
                    .HasColumnType("date")
                    .HasColumnName("dt_candidate_providers_state_change");

                entity.Property(e => e.IntCandidateProvidersStateId).HasColumnName("int_candidate_providers_state_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntUserId).HasColumnName("int_user_id");
            });

            modelBuilder.Entity<TbCandidateTrainer>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_trainers_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_trainers");

                entity.HasIndex(e => new { e.IntProviderId, e.VcEgn }, "tb_candidate_trainers_int_provider_egn_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsAndragog)
                    .HasColumnName("bool_is_andragog")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DtTcontractDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_tcontract_date");

                entity.Property(e => e.IntBirthYear).HasColumnName("int_birth_year");

                entity.Property(e => e.IntEducationId).HasColumnName("int_education_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntGenderId).HasColumnName("int_gender_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntTcontractTypeId).HasColumnName("int_tcontract_type_id");

                entity.Property(e => e.TxtEducationAcademicNotes).HasColumnName("txt_education_academic_notes");

                entity.Property(e => e.TxtEducationCertificateNotes).HasColumnName("txt_education_certificate_notes");

                entity.Property(e => e.TxtEducationSpecialityNotes).HasColumnName("txt_education_speciality_notes");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");

                entity.Property(e => e.VcEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_email");

                entity.Property(e => e.VcTrainerFamilyName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_trainer_family_name");

                entity.Property(e => e.VcTrainerFirstName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_trainer_first_name");

                entity.Property(e => e.VcTrainerSecondName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_trainer_second_name");
            });

            modelBuilder.Entity<TbCandidateTrainerDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_trainer_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_trainer_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.TxtDocumentsManagementFile).HasColumnName("txt_documents_management_file");

                entity.Property(e => e.TxtDocumentsManagementTitle).HasColumnName("txt_documents_management_title");
            });

            modelBuilder.Entity<TbCandidateTrainerProfile>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_trainer_profiles_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_trainer_profiles");

                entity.HasIndex(e => new { e.IntTrainerId, e.IntVetAreaId, e.IntVetSpecialityId }, "tb_candidate_trainer_id_int_vet_speciality_id_key")
                    .IsUnique()
                    .HasFilter("([int_trainer_id] IS NOT NULL AND [int_vet_area_id] IS NOT NULL AND [int_vet_speciality_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolVetAreaPractice).HasColumnName("bool_vet_area_practice");

                entity.Property(e => e.BoolVetAreaQualified).HasColumnName("bool_vet_area_qualified");

                entity.Property(e => e.BoolVetAreaTheory).HasColumnName("bool_vet_area_theory");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");
            });

            modelBuilder.Entity<TbCandidateTrainerQualification>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_candidate_trainer_qualifications_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_candidate_trainer_qualifications");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntProfessionId).HasColumnName("int_profession_id");

                entity.Property(e => e.IntQualificationDuration).HasColumnName("int_qualification_duration");

                entity.Property(e => e.IntQualificationTypeId).HasColumnName("int_qualification_type_id");

                entity.Property(e => e.IntTqualificationTypeId).HasColumnName("int_tqualification_type_id");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.TxtQualificationName).HasColumnName("txt_qualification_name");
            });

            modelBuilder.Entity<TbClient>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_clients_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_clients");

                entity.HasIndex(e => new { e.IntProviderId, e.VcEgn }, "tb_clients_int_provider_egn_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtClientBirthDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_client_birth_date");

                entity.Property(e => e.IntClientGender).HasColumnName("int_client_gender");

                entity.Property(e => e.IntEducationId).HasColumnName("int_education_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.VcClientFamilyName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_client_family_name");

                entity.Property(e => e.VcClientFirstName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_client_first_name");

                entity.Property(e => e.VcClientSecondName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_client_second_name");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");
            });

            modelBuilder.Entity<TbClientsCoursesDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_clients_courses_documents_id")
                    .IsClustered(false);

                entity.ToTable("tb_clients_courses_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Document1File).HasColumnName("document_1_file");

                entity.Property(e => e.Document2File).HasColumnName("document_2_file");

                entity.Property(e => e.DtDocumentDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_document_date");

                entity.Property(e => e.IntClientsCoursesId).HasColumnName("int_clients_courses_id");

                entity.Property(e => e.IntCourseFinishedYear).HasColumnName("int_course_finished_year");

                entity.Property(e => e.IntDocumentStatus)
                    .HasColumnName("int_document_status")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IntDocumentTypeId).HasColumnName("int_document_type_id");

                entity.Property(e => e.NumPracticeResult)
                    .HasColumnType("numeric(3, 2)")
                    .HasColumnName("num_practice_result");

                entity.Property(e => e.NumTheoryResult)
                    .HasColumnType("numeric(3, 2)")
                    .HasColumnName("num_theory_result");

                entity.Property(e => e.VcDocumentPrnNo)
                    .HasMaxLength(50)
                    .HasColumnName("vc_document_prn_no");

                entity.Property(e => e.VcDocumentPrnSer)
                    .HasMaxLength(50)
                    .HasColumnName("vc_document_prn_ser");

                entity.Property(e => e.VcDocumentProt)
                    .HasMaxLength(50)
                    .HasColumnName("vc_document_prot");

                entity.Property(e => e.VcDocumentRegNo)
                    .HasMaxLength(50)
                    .HasColumnName("vc_document_reg_no");

                entity.Property(e => e.VcQualificatiojLevel)
                    .HasMaxLength(50)
                    .HasColumnName("vc_qualificatioj_level");

                entity.Property(e => e.VcQualificationName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_qualification_name");
            });

            modelBuilder.Entity<TbClientsRequiredDocument>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tb_clients_required_documents");

                entity.Property(e => e.BoolBeforeDate).HasColumnName("bool_before_date");

                entity.Property(e => e.DtDocumentDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_document_date");

                entity.Property(e => e.DtDocumentOfficialDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_document_official_date");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCodeCourseGroupRequiredDocumentsTypeId).HasColumnName("int_code_course_group_required_documents_type_id");

                entity.Property(e => e.IntCodeEducationId).HasColumnName("int_code_education_id");

                entity.Property(e => e.IntCodeExtRegisterId).HasColumnName("int_code_ext_register_id");

                entity.Property(e => e.IntCodeQualLevelId).HasColumnName("int_code_qual_level_id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

                entity.Property(e => e.OidFile).HasColumnName("oid_file");

                entity.Property(e => e.VcDesciption)
                    .HasMaxLength(255)
                    .HasColumnName("vc_desciption");

                entity.Property(e => e.VcDocumentPrnNo)
                    .HasMaxLength(100)
                    .HasColumnName("vc_document_prn_no");

                entity.Property(e => e.VcDocumentRegNo)
                    .HasMaxLength(100)
                    .HasColumnName("vc_document_reg_no");
            });

            modelBuilder.Entity<TbCourse>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_courses_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_courses");

                entity.HasIndex(e => new { e.IntProviderId, e.IntVetSpecialityId }, "tb_courses_i1");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCourseEducRequirement).HasColumnName("int_course_educ_requirement");

                entity.Property(e => e.IntCourseFrCurrId).HasColumnName("int_course_fr_curr_id");

                entity.Property(e => e.IntCourseNo).HasColumnName("int_course_no");

                entity.Property(e => e.IntCourseTypeId).HasColumnName("int_course_type_id");

                entity.Property(e => e.IntMandatoryHours).HasColumnName("int_mandatory_hours");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntSelectableHours).HasColumnName("int_selectable_hours");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.VcCourseAddNotes).HasColumnName("vc_course_add_notes");

                entity.Property(e => e.VcCourseName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_course_name");
            });

            modelBuilder.Entity<TbCourse40Competence>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_course40_competences_pk")
                    .IsClustered(false);

                entity.ToTable("tb_course40_competences");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VcCompetence)
                    .HasMaxLength(200)
                    .HasColumnName("vc_competence");
            });

            modelBuilder.Entity<TbCourseGroup>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_course_groups_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_course_groups");

                entity.HasIndex(e => e.DtEndDate, "i_tb_course_groups_end_date");

                entity.HasIndex(e => e.DtStartDate, "i_tb_course_groups_start_date");

                entity.HasIndex(e => e.IntCourseId, "tb_course_groups_i1");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsArchived)
                    .HasColumnName("bool_is_archived")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtCourseSubscribeDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_course_subscribe_date");

                entity.Property(e => e.DtEndDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_end_date");

                entity.Property(e => e.DtExamPracticeDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_exam_practice_date");

                entity.Property(e => e.DtExamTheoryDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_exam_theory_date");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntAssignTypeId).HasColumnName("int_assign_type_id");

                entity.Property(e => e.IntCourseDuration).HasColumnName("int_course_duration");

                entity.Property(e => e.IntCourseEdFormId).HasColumnName("int_course_ed_form_id");

                entity.Property(e => e.IntCourseFrCurrId).HasColumnName("int_course_fr_curr_id");

                entity.Property(e => e.IntCourseId).HasColumnName("int_course_id");

                entity.Property(e => e.IntCourseMeasureTypeId).HasColumnName("int_course_measure_type_id");

                entity.Property(e => e.IntCourseStatusId).HasColumnName("int_course_status_id");

                entity.Property(e => e.IntCourseTypeId).HasColumnName("int_course_type_id");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntMandatoryHours).HasColumnName("int_mandatory_hours");

                entity.Property(e => e.IntPDisabilityCount).HasColumnName("int_p_disability_count");

                entity.Property(e => e.IntProviderPremise).HasColumnName("int_provider_premise");

                entity.Property(e => e.IntSelectableHours).HasColumnName("int_selectable_hours");

                entity.Property(e => e.NumCourseCost)
                    .HasColumnType("numeric(10, 2)")
                    .HasColumnName("num_course_cost");

                entity.Property(e => e.VcAdditionalNotes).HasColumnName("vc_additional_notes");

                entity.Property(e => e.VcCourseAssignType)
                    .HasMaxLength(255)
                    .HasColumnName("vc_course_assign_type");

                entity.Property(e => e.VcCourseGroupName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_course_group_name");

                entity.Property(e => e.VcCourseNotes).HasColumnName("vc_course_notes");

                entity.Property(e => e.VcExamCommMembers).HasColumnName("vc_exam_comm_members");
            });

            modelBuilder.Entity<TbCourseGroups40>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_course_groups_40_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_course_groups_40");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtEndDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_end_date");

                entity.Property(e => e.DtExamPracticeDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_exam_practice_date");

                entity.Property(e => e.DtExamTheoryDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_exam_theory_date");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntAssignTypeId).HasColumnName("int_assign_type_id");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCourseDuration).HasColumnName("int_course_duration");

                entity.Property(e => e.IntCourseEdFormId).HasColumnName("int_course_ed_form_id");

                entity.Property(e => e.IntCourseFrCurrId).HasColumnName("int_course_fr_curr_id");

                entity.Property(e => e.IntCourseTypeId).HasColumnName("int_course_type_id");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntMandatoryHours).HasColumnName("int_mandatory_hours");

                entity.Property(e => e.IntProviderPremise).HasColumnName("int_provider_premise");

                entity.Property(e => e.IntSelectableHours).HasColumnName("int_selectable_hours");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.NumCourseCost)
                    .HasColumnType("numeric(10, 2)")
                    .HasColumnName("num_course_cost");

                entity.Property(e => e.VcAdditionalNotes).HasColumnName("vc_additional_notes");

                entity.Property(e => e.VcExamCommMembers).HasColumnName("vc_exam_comm_members");
            });

            modelBuilder.Entity<TbCourseGroupsDuplicate>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_course_groups_duplicates_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_course_groups_duplicates");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtEndDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_end_date");

                entity.Property(e => e.DtOriginalDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_original_date");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCodeDocumentTypeId).HasColumnName("int_code_document_type_id");

                entity.Property(e => e.IntCourseFinishedYear).HasColumnName("int_course_finished_year");

                entity.Property(e => e.IntOriginalDocumentId).HasColumnName("int_original_document_id");

                entity.Property(e => e.IntOriginalRefClientId).HasColumnName("int_original_ref_client_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.VcOriginalPrnNo)
                    .HasMaxLength(50)
                    .HasColumnName("vc_original_prn_no");

                entity.Property(e => e.VcOriginalRegNo)
                    .HasMaxLength(50)
                    .HasColumnName("vc_original_reg_no");
            });

            modelBuilder.Entity<TbCurricModule>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_curric_modules_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_curric_modules");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntCourseId).HasColumnName("int_course_id");

                entity.Property(e => e.IntCurricHoursType).HasColumnName("int_curric_hours_type");

                entity.Property(e => e.IntCurricOrder)
                    .HasColumnName("int_curric_order")
                    .HasDefaultValueSql("((10))");

                entity.Property(e => e.IntHours).HasColumnName("int_hours");

                entity.Property(e => e.IntTrainingType).HasColumnName("int_training_type");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.VcModuleName)
                    .HasMaxLength(200)
                    .HasColumnName("vc_module_name");
            });

            modelBuilder.Entity<TbDoi>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_doi_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_doi");

                entity.HasIndex(e => e.IntDoiId, "unique_int_doi_id")
                    .IsUnique()
                    .HasFilter("([int_doi_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BSubmitted).HasColumnName("b_submitted");

                entity.Property(e => e.DtSubmittedDate).HasColumnName("dt_submitted_date");

                entity.Property(e => e.IntDoiId).HasColumnName("int_doi_id");

                entity.Property(e => e.VcComment)
                    .HasMaxLength(255)
                    .HasColumnName("vc_comment");

                entity.Property(e => e.VcDoiCertificateSupplement).HasColumnName("vc_doi_certificate_supplement");

                entity.Property(e => e.VcDoiCommission).HasColumnName("vc_doi_commission");

                entity.Property(e => e.VcDoiEducObjectives).HasColumnName("vc_doi_educ_objectives");

                entity.Property(e => e.VcDoiGeneralDescr).HasColumnName("vc_doi_general_descr");

                entity.Property(e => e.VcDoiJobCareer).HasColumnName("vc_doi_job_career");

                entity.Property(e => e.VcDoiJobProfile).HasColumnName("vc_doi_job_profile");

                entity.Property(e => e.VcDoiLrngOutcomes).HasColumnName("vc_doi_lrng_outcomes");

                entity.Property(e => e.VcDoiMtbUpdates).HasColumnName("vc_doi_mtb_updates");

                entity.Property(e => e.VcDoiPdfPath)
                    .HasMaxLength(255)
                    .HasColumnName("vc_doi_pdf_path");

                entity.Property(e => e.VcDoiRegualtion).HasColumnName("vc_doi_regualtion");

                entity.Property(e => e.VcSearch).HasColumnName("vc_search");
            });

            modelBuilder.Entity<TbDoiCommission>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_doi_commissions_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_doi_commissions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcDoiCommName)
                    .HasMaxLength(200)
                    .HasColumnName("vc_doi_comm_name");
            });

            modelBuilder.Entity<TbESigner>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("tb_e_signers");

                entity.HasIndex(e => new { e.IntProviderId, e.IntUserId, e.VcIdNumber }, "tb_e_signers_int_provider_id_int_user_id_vc_id_number_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [int_user_id] IS NOT NULL AND [vc_id_number] IS NOT NULL)");

                entity.Property(e => e.DtFrom)
                    .HasColumnName("dt_from")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DtTo).HasColumnName("dt_to");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntUserId).HasColumnName("int_user_id");

                entity.Property(e => e.VcDescription)
                    .HasMaxLength(200)
                    .HasColumnName("vc_description");

                entity.Property(e => e.VcIdNumber)
                    .HasMaxLength(100)
                    .HasColumnName("vc_id_number");

                entity.Property(e => e.VcNames)
                    .HasMaxLength(150)
                    .HasColumnName("vc_names");
            });

            modelBuilder.Entity<TbExpert>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_experts_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_experts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtBirthDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_birth_date");

                entity.Property(e => e.DtInceptionDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_inception_date");

                entity.Property(e => e.IntCommMemberId).HasColumnName("int_comm_member_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntExpertEkatteId).HasColumnName("int_expert_ekatte_id");

                entity.Property(e => e.IntExpertGenderId).HasColumnName("int_expert_gender_id");

                entity.Property(e => e.IntExpertUserId).HasColumnName("int_expert_user_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.TxtExpertCvFile).HasColumnName("txt_expert_cv_file");

                entity.Property(e => e.VcEducation)
                    .HasMaxLength(255)
                    .HasColumnName("vc_education");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");

                entity.Property(e => e.VcExpertAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_address");

                entity.Property(e => e.VcExpertEmail1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_email1");

                entity.Property(e => e.VcExpertEmail2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_email2");

                entity.Property(e => e.VcExpertFamilyName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_family_name");

                entity.Property(e => e.VcExpertFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_fax");

                entity.Property(e => e.VcExpertFirstName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_first_name");

                entity.Property(e => e.VcExpertOccupation)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_occupation");

                entity.Property(e => e.VcExpertPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_phone1");

                entity.Property(e => e.VcExpertPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_phone2");

                entity.Property(e => e.VcExpertSecondName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_expert_second_name");

                entity.Property(e => e.VcExpertZipcode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_expert_zipcode");

                entity.Property(e => e.VcInceptionOrder)
                    .HasMaxLength(50)
                    .HasColumnName("vc_inception_order");

                entity.Property(e => e.VcTitle)
                    .HasMaxLength(255)
                    .HasColumnName("vc_title");
            });

            modelBuilder.Entity<TbExpertCommission>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_expert_commissions_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_expert_commissions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcExpCommName)
                    .HasMaxLength(250)
                    .HasColumnName("vc_exp_comm_name");
            });

            modelBuilder.Entity<TbGeneratedDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_generated_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_generated_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCodeProceduresDocumentsId).HasColumnName("int_code_procedures_documents_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.OidFile).HasColumnName("oid_file");
            });

            modelBuilder.Entity<TbGroup>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_groups_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_groups");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntGroupType).HasColumnName("int_group_type");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.Property(e => e.VcGroupName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_group_name");

                entity.Property(e => e.VcShortName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_short_name");
            });

            modelBuilder.Entity<TbImportXml>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_import_xml_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_import_xml");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FileDate).HasColumnName("file_date");

                entity.Property(e => e.FileName)
                    .HasMaxLength(1000)
                    .HasColumnName("file_name");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntCourseId).HasColumnName("int_course_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");
            });

            modelBuilder.Entity<TbNapooRequestDoc>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_napoo_request_doc_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_napoo_request_doc");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsSent)
                    .HasColumnName("bool_is_sent")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtRequestDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_request_date");

                entity.Property(e => e.IntRequestYear).HasColumnName("int_request_year");

                entity.Property(e => e.OidRequestPdf).HasColumnName("oid_request_pdf");

                entity.Property(e => e.Ts).HasColumnName("ts");
            });

            modelBuilder.Entity<TbPayment>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_payments_pk")
                    .IsClustered(false);

                entity.ToTable("tb_payments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntProcedurePriceId).HasColumnName("int_procedure_price_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntSpecialitiesCount).HasColumnName("int_specialities_count");

                entity.Property(e => e.IntStartedProceduresId).HasColumnName("int_started_procedures_id");

                entity.Property(e => e.IntStatus).HasColumnName("int_status");

                entity.Property(e => e.IntSume).HasColumnName("int_sume");

                entity.Property(e => e.IntTransactionId).HasColumnName("int_transaction_id");

                entity.Property(e => e.TsGen)
                    .HasColumnName("ts_gen")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TsPayed).HasColumnName("ts_payed");

                entity.Property(e => e.VcText)
                    .HasMaxLength(400)
                    .HasColumnName("vc_text");
            });

            modelBuilder.Entity<TbProcedureDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_procedure_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_procedure_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DsDate)
                    .HasColumnType("date")
                    .HasColumnName("ds_date");

                entity.Property(e => e.DsId).HasColumnName("ds_id");

                entity.Property(e => e.DsOfficialDate).HasColumnName("ds_official_date");

                entity.Property(e => e.DsOfficialId)
                    .HasMaxLength(255)
                    .HasColumnName("ds_official_id");

                entity.Property(e => e.DsOfficialNo)
                    .HasMaxLength(255)
                    .HasColumnName("ds_official_no");

                entity.Property(e => e.DsPrep)
                    .HasMaxLength(255)
                    .HasColumnName("ds_prep");

                entity.Property(e => e.Extension)
                    .HasMaxLength(255)
                    .HasColumnName("extension");

                entity.Property(e => e.Filename)
                    .HasMaxLength(255)
                    .HasColumnName("filename");

                entity.Property(e => e.IntExpertId).HasColumnName("int_expert_id");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

                entity.Property(e => e.MimeType)
                    .HasMaxLength(255)
                    .HasColumnName("mime_type");

                entity.Property(e => e.OidFile).HasColumnName("oid_file");

                entity.Property(e => e.ProviderId).HasColumnName("provider_id");

                entity.Property(e => e.StageDocumentId).HasColumnName("stage_document_id");

                entity.Property(e => e.StartedProcedureId).HasColumnName("started_procedure_id");

                entity.Property(e => e.Ts).HasColumnName("ts");

                entity.Property(e => e.Uin).HasColumnName("uin");
            });

            modelBuilder.Entity<TbProcedurePrice>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_procedure_prices_pk")
                    .IsClustered(false);

                entity.ToTable("tb_procedure_prices");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolCipo).HasColumnName("bool_cipo");

                entity.Property(e => e.BoolCountDependant).HasColumnName("bool_count_dependant");

                entity.Property(e => e.BoolCpo).HasColumnName("bool_cpo");

                entity.Property(e => e.DtFrom)
                    .HasColumnType("date")
                    .HasColumnName("dt_from")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DtTo)
                    .HasColumnType("date")
                    .HasColumnName("dt_to");

                entity.Property(e => e.IntCountMax).HasColumnName("int_count_max");

                entity.Property(e => e.IntCountMin).HasColumnName("int_count_min");

                entity.Property(e => e.IntPaymentPeriod).HasColumnName("int_payment_period");

                entity.Property(e => e.IntPocedureId).HasColumnName("int_pocedure_id");

                entity.Property(e => e.IntPrice).HasColumnName("int_price");

                entity.Property(e => e.IntProcedureStepsId).HasColumnName("int_procedure_steps_id");

                entity.Property(e => e.VcName)
                    .HasMaxLength(400)
                    .HasColumnName("vc_name");
            });

            modelBuilder.Entity<TbProcedureProgress>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_procedure_progress_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_procedure_progress");

                entity.HasIndex(e => e.IntProviderId, "tb_procedure_progress_int_provider_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtDecisionDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_decision_date");

                entity.Property(e => e.DtSystemDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_system_date");

                entity.Property(e => e.IntDecisionProtocolNumber)
                    .HasMaxLength(255)
                    .HasColumnName("int_decision_protocol_number");

                entity.Property(e => e.IntProcedureDecisionId).HasColumnName("int_procedure_decision_id");

                entity.Property(e => e.IntProcedureNumber).HasColumnName("int_procedure_number");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.VcProviderOwner)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_owner");
            });

            modelBuilder.Entity<TbProcedureProgressDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_procedure_progress_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_procedure_progress_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntProcedureProgressId).HasColumnName("int_procedure_progress_id");

                entity.Property(e => e.VcDocumentFile).HasColumnName("vc_document_file");

                entity.Property(e => e.VcDocumentName).HasColumnName("vc_document_name");
            });

            modelBuilder.Entity<TbProvider>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_providers_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_providers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsBrra)
                    .HasColumnName("bool_is_brra")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtLicenceData)
                    .HasColumnType("date")
                    .HasColumnName("dt_licence_data");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntLicenceNumber).HasColumnName("int_licence_number");

                entity.Property(e => e.IntLicenceStatusId).HasColumnName("int_licence_status_id");

                entity.Property(e => e.IntLocalGroupId).HasColumnName("int_local_group_id");

                entity.Property(e => e.IntProviderBulstat)
                    .HasMaxLength(20)
                    .HasColumnName("int_provider_bulstat");

                entity.Property(e => e.IntProviderContactPersEkatteId).HasColumnName("int_provider_contact_pers_ekatte_id");

                entity.Property(e => e.IntProviderOwnershipId).HasColumnName("int_provider_ownership_id");

                entity.Property(e => e.IntProviderRegistrationId).HasColumnName("int_provider_registration_id");

                entity.Property(e => e.IntProviderStatusId).HasColumnName("int_provider_status_id");

                entity.Property(e => e.VcProviderAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_address");

                entity.Property(e => e.VcProviderContactPers)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers");

                entity.Property(e => e.VcProviderContactPersAddress)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_address");

                entity.Property(e => e.VcProviderContactPersEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_email");

                entity.Property(e => e.VcProviderContactPersFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_fax");

                entity.Property(e => e.VcProviderContactPersPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone1");

                entity.Property(e => e.VcProviderContactPersPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_contact_pers_phone2");

                entity.Property(e => e.VcProviderContactPersZipcode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_provider_contact_pers_zipcode");

                entity.Property(e => e.VcProviderEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_email");

                entity.Property(e => e.VcProviderFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_fax");

                entity.Property(e => e.VcProviderManager)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_manager");

                entity.Property(e => e.VcProviderName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_name");

                entity.Property(e => e.VcProviderOwner)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_owner");

                entity.Property(e => e.VcProviderPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone1");

                entity.Property(e => e.VcProviderPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone2");

                entity.Property(e => e.VcProviderProfile).HasColumnName("vc_provider_profile");

                entity.Property(e => e.VcProviderWeb)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_web");

                entity.Property(e => e.VcZipCode)
                    .HasMaxLength(4)
                    .HasColumnName("vc_zip_code");
            });

            modelBuilder.Entity<TbProviderActivity>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_provider_activities_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_provider_activities");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCurrentYear).HasColumnName("int_current_year");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtProviderActivities).HasColumnName("txt_provider_activities");
            });

            modelBuilder.Entity<TbProviderPremise>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_provider_premises_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_provider_premises");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsVisited).HasColumnName("bool_is_visited");

                entity.Property(e => e.ts).HasColumnName("ts");

                entity.Property(e => e.IntPreaz)
                    .HasColumnName("int_preaz")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntProviderPremiseEkatte).HasColumnName("int_provider_premise_ekatte");

                entity.Property(e => e.IntProviderPremiseNo).HasColumnName("int_provider_premise_no");

                entity.Property(e => e.IntProviderPremiseStatus).HasColumnName("int_provider_premise_status");

                entity.Property(e => e.TxtProviderPremiseAddress).HasColumnName("txt_provider_premise_address");

                entity.Property(e => e.TxtProviderPremiseName).HasColumnName("txt_provider_premise_name");

                entity.Property(e => e.TxtProviderPremiseNotes).HasColumnName("txt_provider_premise_notes");
            });

            modelBuilder.Entity<TbProviderPremisesRoom>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_provider_premises_rooms_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_provider_premises_rooms");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntPremiseId).HasColumnName("int_premise_id");

                entity.Property(e => e.IntProviderPremiseRoomArea).HasColumnName("int_provider_premise_room_area");

                entity.Property(e => e.IntProviderPremiseRoomNo).HasColumnName("int_provider_premise_room_no");

                entity.Property(e => e.IntProviderPremiseRoomType).HasColumnName("int_provider_premise_room_type");

                entity.Property(e => e.IntProviderPremiseRoomUsage).HasColumnName("int_provider_premise_room_usage");

                entity.Property(e => e.IntProviderPremiseRoomWorkplaces).HasColumnName("int_provider_premise_room_workplaces");

                entity.Property(e => e.TxtProviderPremiseRoomEquipment).HasColumnName("txt_provider_premise_room_equipment");

                entity.Property(e => e.TxtProviderPremiseRoomName).HasColumnName("txt_provider_premise_room_name");
            });

            modelBuilder.Entity<TbProviderSpecialitiesCurriculum>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_provider_specialities_curriculum_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_provider_specialities_curriculum");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsUpdated)
                    .HasColumnName("bool_is_updated")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtUpdateDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_update_date");

                entity.Property(e => e.IntProviderSpecialityId).HasColumnName("int_provider_speciality_id");

                entity.Property(e => e.IntSpecialityCurriculumUpdateReasonId).HasColumnName("int_speciality_curriculum_update_reason_id");

                entity.Property(e => e.OidFile).HasColumnName("oid_file");

                entity.Property(e => e.VcFileName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_file_name");
            });

            modelBuilder.Entity<TbProviderSpeciality>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_provider_specialities_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_provider_specialities");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsAdded)
                    .HasColumnName("bool_is_added")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BoolIsValid)
                    .HasColumnName("bool_is_valid")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DtLicenceData)
                    .HasColumnType("date")
                    .HasColumnName("dt_licence_data");

                entity.Property(e => e.IntLicenceProtNo)
                    .HasMaxLength(255)
                    .HasColumnName("int_licence_prot_no");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");

                entity.Property(e => e.TxtSpecialityFile).HasColumnName("txt_speciality_file");

                entity.Property(e => e.TxtSpecialityNotes).HasColumnName("txt_speciality_notes");
            });

            modelBuilder.Entity<TbProviderUploadedDoc>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_provider_uploaded_docs_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_provider_uploaded_docs");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BinFile).HasColumnName("bin_file");

                entity.Property(e => e.DtDocUploadDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_doc_upload_date");

                entity.Property(e => e.IntDocStatusId).HasColumnName("int_doc_status_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntUploadDocTypeId).HasColumnName("int_upload_doc_type_id");

                entity.Property(e => e.IntYear).HasColumnName("int_year");

                entity.Property(e => e.TxtDocDescription).HasColumnName("txt_doc_description");

                entity.Property(e => e.TxtFileName).HasColumnName("txt_file_name");

                entity.Property(e => e.TxtFileType).HasColumnName("txt_file_type");
            });

            modelBuilder.Entity<TbProvidersCipoManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_providers_cipo_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_providers_cipo_management");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCipoManagementId).HasColumnName("int_cipo_management_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtCipoManagementNotes).HasColumnName("txt_cipo_management_notes");
            });

            modelBuilder.Entity<TbProvidersCpoManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_providers_cpo_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_providers_cpo_management");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntCpoManagementId).HasColumnName("int_cpo_management_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TxtCpoManagementNotes).HasColumnName("txt_cpo_management_notes");
            });

            modelBuilder.Entity<TbProvidersDocsDashboard>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_providers_docs_dashboard_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_providers_docs_dashboard");

                entity.HasIndex(e => new { e.IntProviderId, e.IntDocTypeId, e.IntDocsYear }, "tb_providers_docs_dashboard_unique_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [int_doc_type_id] IS NOT NULL AND [int_docs_year] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolHasSerialNumber).HasColumnName("bool_has_serial_number");

                entity.Property(e => e.IntCntAvlb).HasColumnName("int_cnt_avlb");

                entity.Property(e => e.IntCntDstr).HasColumnName("int_cnt_dstr");

                entity.Property(e => e.IntCntNull).HasColumnName("int_cnt_null");

                entity.Property(e => e.IntCntPrnt).HasColumnName("int_cnt_prnt");

                entity.Property(e => e.IntCntRecv).HasColumnName("int_cnt_recv");

                entity.Property(e => e.IntCntSent).HasColumnName("int_cnt_sent");

                entity.Property(e => e.IntDocTypeId).HasColumnName("int_doc_type_id");

                entity.Property(e => e.IntDocsYear).HasColumnName("int_docs_year");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");
            });

            modelBuilder.Entity<TbProvidersDocsOffer>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_providers_docs_offers_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_providers_docs_offers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolOfferValid).HasColumnName("bool_offer_valid");

                entity.Property(e => e.DtClosed)
                    .HasColumnType("date")
                    .HasColumnName("dt_closed");

                entity.Property(e => e.DtOffered)
                    .HasColumnType("date")
                    .HasColumnName("dt_offered");

                entity.Property(e => e.IntCountOffered).HasColumnName("int_count_offered");

                entity.Property(e => e.IntDocTypeId).HasColumnName("int_doc_type_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntSeekOffer).HasColumnName("int_seek_offer");
            });

            modelBuilder.Entity<TbProvidersDocumentsManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_providers_documents_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_providers_documents_management");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntDocumentsManagementId).HasColumnName("int_documents_management_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.TsDocument)
                    .HasColumnName("ts_document")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TxtDocumentsManagementFile).HasColumnName("txt_documents_management_file");

                entity.Property(e => e.TxtDocumentsManagementTitle).HasColumnName("txt_documents_management_title");
            });

            modelBuilder.Entity<TbProvidersLicenceChange>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_licence_change_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_providers_licence_change");

                entity.HasIndex(e => new { e.IntProviderId, e.DtChangeDate }, "unique_provider_date")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [dt_change_date] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtChangeDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_change_date");

                entity.Property(e => e.DtInsertDate).HasColumnName("dt_insert_date");

                entity.Property(e => e.IntLicenceStatusDetailsId).HasColumnName("int_licence_status_details_id");

                entity.Property(e => e.IntLicenceStatusId).HasColumnName("int_licence_status_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.VcNotes).HasColumnName("vc_notes");

                entity.Property(e => e.VcNumberCommand)
                    .HasMaxLength(255)
                    .HasColumnName("vc_number_command");
            });

            modelBuilder.Entity<TbProvidersRequestDoc>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_providers_request_doc_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_providers_request_doc");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsSent)
                    .HasColumnName("bool_is_sent")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DtRequestDoc)
                    .HasColumnType("date")
                    .HasColumnName("dt_request_doc");

                entity.Property(e => e.IntCurrentYear).HasColumnName("int_current_year");

                entity.Property(e => e.IntNapooRequestId).HasColumnName("int_napoo_request_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.OidRequestPdf).HasColumnName("oid_request_pdf");

                entity.Property(e => e.Ts).HasColumnName("ts");

                entity.Property(e => e.VcAddress)
                    .HasMaxLength(1000)
                    .HasColumnName("vc_address");

                entity.Property(e => e.VcName)
                    .HasMaxLength(1000)
                    .HasColumnName("vc_name");

                entity.Property(e => e.VcPosition)
                    .HasMaxLength(1000)
                    .HasColumnName("vc_position");

                entity.Property(e => e.VcTelephone)
                    .HasMaxLength(1000)
                    .HasColumnName("vc_telephone");
            });

            modelBuilder.Entity<TbRequestDocsManagement>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_request_docs_management_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_request_docs_management");

                entity.HasIndex(e => new { e.IntProviderId, e.IntPartnerId, e.IntRequestId, e.IntRequestDocTypeId, e.IntReceiveDocsYear, e.DtReceiveDocsDate }, "tb_request_docs_management_unique_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [int_partner_id] IS NOT NULL AND [int_request_id] IS NOT NULL AND [int_request_doc_type_id] IS NOT NULL AND [int_receive_docs_year] IS NOT NULL AND [dt_receive_docs_date] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtReceiveDocsDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_receive_docs_date");

                entity.Property(e => e.IntNapooRequestId).HasColumnName("int_napoo_request_id");

                entity.Property(e => e.IntPartnerId).HasColumnName("int_partner_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntReceiveDocsCount).HasColumnName("int_receive_docs_count");

                entity.Property(e => e.IntReceiveDocsYear).HasColumnName("int_receive_docs_year");

                entity.Property(e => e.IntRequestDocTypeId).HasColumnName("int_request_doc_type_id");

                entity.Property(e => e.IntRequestDocsOperationId).HasColumnName("int_request_docs_operation_id");

                entity.Property(e => e.IntRequestId).HasColumnName("int_request_id");

                entity.Property(e => e.VcOrigTbClCourDocs)
                    .HasMaxLength(50)
                    .HasColumnName("vc_orig_tb_cl_cour_docs");

                entity.Property(e => e.VcTbProviderUploadedDocsIds)
                    .HasMaxLength(200)
                    .HasColumnName("vc_tb_provider_uploaded_docs_ids");
            });

            modelBuilder.Entity<TbRequestDocsSn>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_request_docs_sn_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_request_docs_sn");

                entity.HasIndex(e => new { e.IntProviderId, e.IntRequestDocsManagementId, e.IntRequestDocTypeId, e.IntReceiveDocsYear, e.VcRequestDocNumber, e.IntRequestDocsOperationId }, "tb_request_docs_sn_unique_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [int_request_docs_management_id] IS NOT NULL AND [int_request_doc_type_id] IS NOT NULL AND [int_receive_docs_year] IS NOT NULL AND [vc_request_doc_number] IS NOT NULL AND [int_request_docs_operation_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolChkFabn).HasColumnName("bool_chk_fabn");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntReceiveDocsYear).HasColumnName("int_receive_docs_year");

                entity.Property(e => e.IntRequestDocTypeId).HasColumnName("int_request_doc_type_id");

                entity.Property(e => e.IntRequestDocsManagementId).HasColumnName("int_request_docs_management_id");

                entity.Property(e => e.IntRequestDocsOperationId).HasColumnName("int_request_docs_operation_id");

                entity.Property(e => e.VcRequestDocNumber)
                    .HasMaxLength(50)
                    .HasColumnName("vc_request_doc_number");
            });

            modelBuilder.Entity<TbRole>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_roles_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_roles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.VcRoleName)
                    .HasMaxLength(255)
                    .HasColumnName("vc_role_name");
            });

            modelBuilder.Entity<TbSignContent>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_sign_content_pk")
                    .IsClustered(false);

                entity.ToTable("tb_sign_content");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Dttimestamp)
                    .HasColumnName("dttimestamp")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExtensionsSubjectAltName)
                    .HasMaxLength(500)
                    .HasColumnName("extensions_subject_alt_name");

                entity.Property(e => e.IntClientId).HasColumnName("int_client_id");

                entity.Property(e => e.IntCourseGroupId).HasColumnName("int_course_group_id");

                entity.Property(e => e.IntDocumentStatus).HasColumnName("int_document_status");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntUserId).HasColumnName("int_user_id");

                entity.Property(e => e.IssuerO)
                    .HasMaxLength(150)
                    .HasColumnName("issuer_o");

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(50)
                    .HasColumnName("serial_number");

                entity.Property(e => e.SignerBulstat)
                    .HasMaxLength(50)
                    .HasColumnName("signer_bulstat");

                entity.Property(e => e.SignerEgn)
                    .HasMaxLength(50)
                    .HasColumnName("signer_egn");

                entity.Property(e => e.SubjectC)
                    .HasMaxLength(100)
                    .HasColumnName("subject_c");

                entity.Property(e => e.SubjectCn)
                    .HasMaxLength(200)
                    .HasColumnName("subject_cn");

                entity.Property(e => e.SubjectE)
                    .HasMaxLength(150)
                    .HasColumnName("subject_e");

                entity.Property(e => e.SubjectEmailAddress)
                    .HasMaxLength(150)
                    .HasColumnName("subject_email_address");

                entity.Property(e => e.SubjectOu)
                    .HasMaxLength(100)
                    .HasColumnName("subject_ou");

                entity.Property(e => e.SubjectSerialNumber)
                    .HasMaxLength(100)
                    .HasColumnName("subject_serial_number");

                entity.Property(e => e.ValidFrom)
                    .HasMaxLength(50)
                    .HasColumnName("valid_from");

                entity.Property(e => e.ValidTo)
                    .HasMaxLength(50)
                    .HasColumnName("valid_to");

                entity.Property(e => e.VcContent).HasColumnName("vc_content");

                entity.Property(e => e.VcSignedContent).HasColumnName("vc_signed_content");
            });

            modelBuilder.Entity<TbStartedProcedure>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_started_procedures_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_started_procedures");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtChairmanReportDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_chairman_report_date");

                entity.Property(e => e.DtDeniedLicenseOrderDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_denied_license_order_date");

                entity.Property(e => e.DtExpertReportDeadline)
                    .HasColumnType("date")
                    .HasColumnName("dt_expert_report_deadline");

                entity.Property(e => e.DtIssuedLicenseOrderDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_issued_license_order_date");

                entity.Property(e => e.DtIssuesMailNumber)
                    .HasColumnType("date")
                    .HasColumnName("dt_issues_mail_number");

                entity.Property(e => e.DtLegalBookDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_legal_book_date");

                entity.Property(e => e.DtLicenseDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_license_date");

                entity.Property(e => e.DtLicenseExpertiseMailDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_license_expertise_mail_date");

                entity.Property(e => e.DtLicenseExpertiseOrderDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_license_expertise_order_date");

                entity.Property(e => e.DtMeetingDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_meeting_date");

                entity.Property(e => e.DtMeetingProtocolDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_meeting_protocol_date");

                entity.Property(e => e.DtNapooReportDeadline)
                    .HasColumnType("date")
                    .HasColumnName("dt_napoo_report_deadline");

                entity.Property(e => e.DtNegativeDeadline)
                    .HasColumnType("date")
                    .HasColumnName("dt_negative_deadline");

                entity.Property(e => e.DtNegativeOpinionDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_negative_opinion_date");

                entity.Property(e => e.DtReportReviewDeadline)
                    .HasColumnType("date")
                    .HasColumnName("dt_report_review_deadline");

                entity.Property(e => e.DtSummarizedReportDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_summarized_report_date");

                entity.Property(e => e.DtSummarizedReportDeadline)
                    .HasColumnType("date")
                    .HasColumnName("dt_summarized_report_deadline");

                entity.Property(e => e.IntCandidateTypeId).HasColumnName("int_candidate_type_id");

                entity.Property(e => e.IntEkatteId).HasColumnName("int_ekatte_id");

                entity.Property(e => e.IntProviderBulstat)
                    .HasMaxLength(20)
                    .HasColumnName("int_provider_bulstat");

                entity.Property(e => e.IntReceiveDocumentsId).HasColumnName("int_receive_documents_id");

                entity.Property(e => e.ProcedureId).HasColumnName("procedure_id");

                entity.Property(e => e.ProviderId).HasColumnName("provider_id");

                entity.Property(e => e.Ts).HasColumnName("ts");

                entity.Property(e => e.VcChairmanReportNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_chairman_report_number");

                entity.Property(e => e.VcDeniedLicenseOrderNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_denied_license_order_number");

                entity.Property(e => e.VcDeniedMailNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_denied_mail_number");

                entity.Property(e => e.VcIssuedLicenseOrderNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_issued_license_order_number");

                entity.Property(e => e.VcIssuesMailNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_issues_mail_number");

                entity.Property(e => e.VcLegalBookNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_legal_book_number");

                entity.Property(e => e.VcLicenseExpertiseMailNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_license_expertise_mail_number");

                entity.Property(e => e.VcLicenseExpertiseOrderNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_license_expertise_order_number");

                entity.Property(e => e.VcLicenseNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_license_number");

                entity.Property(e => e.VcLicensingMailOutgoingNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_licensing_mail_outgoing_number");

                entity.Property(e => e.VcMeetingHour)
                    .HasMaxLength(255)
                    .HasColumnName("vc_meeting_hour");

                entity.Property(e => e.VcMeetingProtocolNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_meeting_protocol_number");

                entity.Property(e => e.VcNegativeIssues).HasColumnName("vc_negative_issues");

                entity.Property(e => e.VcNegativeNeededDocuments).HasColumnName("vc_negative_needed_documents");

                entity.Property(e => e.VcNegativeOpinionNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_negative_opinion_number");

                entity.Property(e => e.VcNegativeReasons).HasColumnName("vc_negative_reasons");

                entity.Property(e => e.VcProviderEmail)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_email");

                entity.Property(e => e.VcProviderFax)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_fax");

                entity.Property(e => e.VcProviderManager)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_manager");

                entity.Property(e => e.VcProviderOwner)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_owner");

                entity.Property(e => e.VcProviderPhone1)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone1");

                entity.Property(e => e.VcProviderPhone2)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_phone2");

                entity.Property(e => e.VcProviderWeb)
                    .HasMaxLength(255)
                    .HasColumnName("vc_provider_web");

                entity.Property(e => e.VcSummarizedReportNumber)
                    .HasMaxLength(255)
                    .HasColumnName("vc_summarized_report_number");
            });

            modelBuilder.Entity<TbStartedProcedureProgress>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_started_procedure_progress_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_started_procedure_progress");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.StageId).HasColumnName("stage_id");

                entity.Property(e => e.StartedProcedureId).HasColumnName("started_procedure_id");

                entity.Property(e => e.StepId).HasColumnName("step_id");

                entity.Property(e => e.Ts).HasColumnName("ts");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<TbSurveyAnswer>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_survey_answer_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_survey_answer");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtTimestamp).HasColumnName("dt_timestamp");

                entity.Property(e => e.IntQuestionId).HasColumnName("int_question_id");

                entity.Property(e => e.IntUserId).HasColumnName("int_user_id");

                entity.Property(e => e.VcAnswerValue).HasColumnName("vc_answer_value");
            });

            modelBuilder.Entity<TbSurveyQuestion>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_survey_question_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_survey_question");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolQuestionActive).HasColumnName("bool_question_active");

                entity.Property(e => e.IntOrderId).HasColumnName("int_order_id");

                entity.Property(e => e.IntQuestionType).HasColumnName("int_question_type");

                entity.Property(e => e.VcQuestionText).HasColumnName("vc_question_text");
            });

            modelBuilder.Entity<TbTrainer>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_trainers_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_trainers");

                entity.HasIndex(e => new { e.IntProviderId, e.VcEgn }, "tb_trainers_int_provider_egn_key")
                    .IsUnique()
                    .HasFilter("([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolIsAndragog)
                    .HasColumnName("bool_is_andragog")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.DtTcontractDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_tcontract_date");

                entity.Property(e => e.IntBirthYear).HasColumnName("int_birth_year");

                entity.Property(e => e.IntEducationId).HasColumnName("int_education_id");

                entity.Property(e => e.IntEgnTypeId).HasColumnName("int_egn_type_id");

                entity.Property(e => e.IntGenderId).HasColumnName("int_gender_id");

                entity.Property(e => e.IntNationalityId).HasColumnName("int_nationality_id");

                entity.Property(e => e.IntProviderId).HasColumnName("int_provider_id");

                entity.Property(e => e.IntTcontractTypeId).HasColumnName("int_tcontract_type_id");

                entity.Property(e => e.TxtEducationAcademicNotes).HasColumnName("txt_education_academic_notes");

                entity.Property(e => e.TxtEducationCertificateNotes).HasColumnName("txt_education_certificate_notes");

                entity.Property(e => e.TxtEducationSpecialityNotes).HasColumnName("txt_education_speciality_notes");

                entity.Property(e => e.VcEgn)
                    .HasMaxLength(255)
                    .HasColumnName("vc_egn");

                entity.Property(e => e.VcEmail)
                    .HasMaxLength(50)
                    .HasColumnName("vc_email");

                entity.Property(e => e.VcTrainerFamilyName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_trainer_family_name");

                entity.Property(e => e.VcTrainerFirstName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_trainer_first_name");

                entity.Property(e => e.VcTrainerSecondName)
                    .HasMaxLength(50)
                    .HasColumnName("vc_trainer_second_name");
            });

            modelBuilder.Entity<TbTrainerDocument>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_trainer_documents_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_trainer_documents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.TxtDocumentsManagementFile).HasColumnName("txt_documents_management_file");

                entity.Property(e => e.TxtDocumentsManagementTitle).HasColumnName("txt_documents_management_title");
            });

            modelBuilder.Entity<TbTrainerProfile>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_trainer_profiles_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_trainer_profiles");

                entity.HasIndex(e => new { e.IntTrainerId, e.IntVetAreaId, e.IntVetSpecialityId }, "tb_trainer_id_int_vet_speciality_id_key")
                    .IsUnique()
                    .HasFilter("([int_trainer_id] IS NOT NULL AND [int_vet_area_id] IS NOT NULL AND [int_vet_speciality_id] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolVetAreaPractice).HasColumnName("bool_vet_area_practice");

                entity.Property(e => e.BoolVetAreaQualified).HasColumnName("bool_vet_area_qualified");

                entity.Property(e => e.BoolVetAreaTheory).HasColumnName("bool_vet_area_theory");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.IntVetAreaId).HasColumnName("int_vet_area_id");

                entity.Property(e => e.IntVetSpecialityId).HasColumnName("int_vet_speciality_id");
            });

            modelBuilder.Entity<TbTrainerQualification>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_trainer_qualifications_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_trainer_qualifications");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DtStartDate)
                    .HasColumnType("date")
                    .HasColumnName("dt_start_date");

                entity.Property(e => e.IntProfessionId).HasColumnName("int_profession_id");

                entity.Property(e => e.IntQualificationDuration).HasColumnName("int_qualification_duration");

                entity.Property(e => e.IntQualificationTypeId).HasColumnName("int_qualification_type_id");

                entity.Property(e => e.IntTqualificationTypeId).HasColumnName("int_tqualification_type_id");

                entity.Property(e => e.IntTrainerId).HasColumnName("int_trainer_id");

                entity.Property(e => e.TxtQualificationName).HasColumnName("txt_qualification_name");
            });

            modelBuilder.Entity<TbUser>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_users_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_users");

                entity.HasIndex(e => e.Unhs, "tb_users_unhs_index")
                    .IsUnique()
                    .HasFilter("([unhs] IS NOT NULL)");

                entity.HasIndex(e => e.Upwd, "tb_users_upwd_index")
                    .IsUnique()
                    .HasFilter("([upwd] IS NOT NULL)");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Adat).HasColumnName("adat");

                entity.Property(e => e.IntGlobalGroupId).HasColumnName("int_global_group_id");

                entity.Property(e => e.IntLocalGroupId).HasColumnName("int_local_group_id");

                entity.Property(e => e.Udat).HasColumnName("udat");

                entity.Property(e => e.Unhs)
                    .HasMaxLength(45)
                    .HasColumnName("unhs");

                entity.Property(e => e.Upwd)
                    .HasMaxLength(45)
                    .HasColumnName("upwd");

                entity.Property(e => e.VcFullname)
                    .HasMaxLength(255)
                    .HasColumnName("vc_fullname");
            });

            modelBuilder.Entity<TbUserPress>(entity =>
            {
                entity.HasKey(e => e.IntId)
                    .HasName("tb_user_press_pk")
                    .IsClustered(false);

                entity.ToTable("tb_user_press");

                entity.Property(e => e.IntId)
                    .ValueGeneratedNever()
                    .HasColumnName("int_id");

                entity.Property(e => e.VcName)
                    .HasMaxLength(100)
                    .HasColumnName("vc_name");

                entity.Property(e => e.VcObl)
                    .HasMaxLength(100)
                    .HasColumnName("vc_obl");

                entity.Property(e => e.VcPass)
                    .HasMaxLength(100)
                    .HasColumnName("vc_pass");

                entity.Property(e => e.VcUser)
                    .HasMaxLength(100)
                    .HasColumnName("vc_user");
            });

            modelBuilder.Entity<TbVersion>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("tb_version_pkey")
                    .IsClustered(false);

                entity.ToTable("tb_version");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.DtTimestamp)
                    .HasColumnName("dt_timestamp")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.VcComment).HasColumnName("vc_comment");
            });

            modelBuilder.HasSequence("arch_tb_course_groups_40_id_seq")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("arch_tb_course_groups_duplicates_id_seq")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("ref_course_type_fr_curr_int_code_course_fr_curr_id_seq")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("ref_course_type_fr_curr_int_code_course_type_id_seq")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_accs_adminlogid")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_accs_groupnames")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_accs_sessionid")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_arch_clients_courses_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_arch_courses_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_arch_courses_providers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_arch_experts_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_arch_providers_department_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_arch_providers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_arch_trainers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_assign_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_candidate_providers_state_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_candidate_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_ccontract_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_cfinished_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_cipo_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_comm_member_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_commission_institution_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_correction_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_course_ed_form_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_course_fr_curr_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_course_group_required_documents_type_id")
                .HasMin(1)
                .HasMax(2048);

            modelBuilder.HasSequence("seq_code_course_measure_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_course_status_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_course_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_cpo_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_curric_hours_type_id")
                .HasMin(1)
                .HasMax(900);

            modelBuilder.HasSequence("seq_code_document_status")
                .HasMin(1)
                .HasMax(900);

            modelBuilder.HasSequence("seq_code_document_status_locks_id")
                .HasMin(1)
                .HasMax(900);

            modelBuilder.HasSequence("seq_code_document_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_documents_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_education_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_egn_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_ekatte_full_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_ekatte_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_employment_status_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_expert_position_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_expert_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_gender_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_licence_status_details_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_licence_status_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_municipality_details_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_municipality_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_nationality_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_nkpd_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_obl_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_operation_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_premises_correspondence_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_premises_status_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_premises_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_premises_usage_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_procedures_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_provider_ownership_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_provider_registration_id")
                .HasMin(1)
                .HasMax(922337);

            modelBuilder.HasSequence("seq_code_provider_status_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_qual_level_id")
                .HasMin(1)
                .HasMax(900);

            modelBuilder.HasSequence("seq_code_qualification_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_request_doc_status_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_request_doc_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_speciality_curriculum_update_reason_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_speciality_vqs_id")
                .HasMin(1)
                .HasMax(100);

            modelBuilder.HasSequence("seq_code_tcontract_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_tqualification_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_training_type_id")
                .HasMin(1)
                .HasMax(900);

            modelBuilder.HasSequence("seq_code_validity_check_target")
                .HasMin(1)
                .HasMax(512);

            modelBuilder.HasSequence("seq_code_vet_area_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_vet_group_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_vet_list_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_vet_profession_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_vet_speciality_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_village_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_visit_result_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_code_wgdoi_function_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_groups_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_arch_expert_commissions_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_arch_experts_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_arch_procedure_expert_commissions_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_arch_procedure_experts_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_arch_procedure_procedures_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_arch_procedure_provider_premises_specialities_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_arch_procedures_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_candidates_experts_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_candidates_procedures_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_cg_curric_files_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_clients_courses_documents_status")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_clients_courses_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_clients_lab_offices_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_course_document_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_course_group_premises_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_courses_providers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_document_type_cfinished_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_experts_commissions_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_experts_doi_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_experts_types_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_experts_vet_area_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_fr_curr_ed_form_id")
                .HasMin(1)
                .HasMax(900);

            modelBuilder.HasSequence("seq_ref_fr_curr_educ_level_id")
                .HasMin(1)
                .HasMax(9000);

            modelBuilder.HasSequence("seq_ref_fr_curr_qual_level_id")
                .HasMin(1)
                .HasMax(9000);

            modelBuilder.HasSequence("seq_ref_main_expert_commissions_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_main_experts_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_procedures_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_provider_premises_specialities_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_request_doc_status_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_request_doc_type_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_role_acl_actions_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_role_groups_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_role_users_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_trainers_courses_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_vet_specialities_nkpds_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_visits_experts_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_ref_visits_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_sys_acl_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_sys_mail_log_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_sys_operation_log_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_sys_sign_log_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_acl_actions_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_acl_defaults_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_annual_info_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_provider_premises_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_provider_premises_rooms_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_provider_specialities_curriculum_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_provider_specialities_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_providers_cipo_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_providers_cpo_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_providers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_trainer_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_trainer_profiles_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_trainer_qualifications_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_procedure_trainers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_arch_providers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_candidate_providers_cipo_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_candidate_providers_cpo_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_candidate_providers_documents_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_candidate_providers_state_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_clients_courses_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_clients_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_clients_required_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_course_groups_40_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_course_groups_duplicates_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_course_groups_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_course40_competences_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_courses_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_curric_modules_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_doi_commissions_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_doi_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_e_signers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_expert_commissions_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_expert_profiles_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_experts_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_generated_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_import_xml_id")
                .StartsAt(10)
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_lab_offices_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_napoo_request_doc_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_payments_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_procedure_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_procedure_prices_id")
                .HasMin(1)
                .HasMax(1000);

            modelBuilder.HasSequence("seq_tb_procedure_progress_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_procedure_progress_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_provider_activities_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_provider_departments_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_provider_premises_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_provider_premises_rooms_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_provider_specialities_curriculum_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_provider_specialities_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_provider_uploaded_docs_id")
                .StartsAt(74)
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_providers_cipo_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_providers_cpo_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_providers_docs_dashboard_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_providers_docs_offers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_providers_documents_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_providers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_providers_licence_change_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_providers_request_doc_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_request_docs_management_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_request_docs_sn_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_roles_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_sign_content_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_started_procedure_progress_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_started_procedures_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_survey_answer_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_survey_question_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_trainer_documents_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_trainer_profiles_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_trainer_qualifications_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_tb_trainers_id")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("seq_users_id")
                .HasMin(1)
                .HasMax(2147483647);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
