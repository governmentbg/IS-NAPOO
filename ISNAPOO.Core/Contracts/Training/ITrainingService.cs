using Data.Models.Data.SqlView.Reports;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.Register;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.Core.XML.Course;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Training
{
    public interface ITrainingService
    {
        #region Training program
        Task<IEnumerable<ProgramVM>> GetAllActiveProgramsByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<ProgramVM>> GetAllActiveLegalCapacityProgramsByIdCandidateProviderAsync(int idCandidateProvider);

        Task<ResultContext<NoResult>> MarkProgramAsDeletedByIdProgramAsync(int idProgram);

        Task<IEnumerable<CandidateProviderSpecialityVM>> GetCandidateProviderSpecialitiesByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<FrameworkProgramVM>> GetFrameworkProgramByIdVQSAndByIdTypeFrameworkProgramAsync(int idVQS, int idTypeFrameworkProgram);

        Task<ResultContext<ProgramVM>> CreateTrainingProgramAsync(ResultContext<ProgramVM> inputContext, int? idCourseType = null);

        Task<ResultContext<ProgramVM>> UpdateTrainingProgramAsync(ResultContext<ProgramVM> inputContext);

        Task<ProgramVM> GetTrainingProgramByIdAsync(int idTrainingProgram);

        Task<ProgramVM> GetTrainingProgramByIdWithoutAnythingIncludedAsync(int idTrainingProgram);

        Task<ProgramVM> GetTrainingProgramByFrameworkProgramIdAsync(int idFrameworkProgram);

        Task UpdateTrainingProgramHoursByIdProgramAsync(int idProgram, int mandatoryHours, int selectableHours);

        Task<bool> DoesProgramWithSameNumberFrameworkProgramAndSpecialityExistAsync(ProgramVM program);

        public MemoryStream CreateExcelWithMissingRequiredDocumentsForClients(Dictionary<string, string> clientsDict);
        #endregion

        #region Training curriculum

        Task<TrainingCurriculumVM> GetTrainingCurriculumByIdAsync(int idTrainingCurriculum);
        Task<TrainingCurriculumCombinedVM> GetTrainingCurriculumCombinedByIdCourseAsync(int idCourse);

        Task<IEnumerable<TrainingCurriculumVM>> GetTrainingCurriculumByIdProgramAsync(int idProgram);

        Task<IEnumerable<TrainingCurriculumVM>> GetTrainingCurriculumByIdCourseAsync(int idCourse);

        Task<IEnumerable<TrainingCurriculumVM>> GetTrainingCurriculumsWithoutAnythingIncludedByIdCourseAsync(int idCourse);

        Task<IEnumerable<TrainingCurriculumVM>> GetTrainingCurriculumWithoutAnythingIncludedByIdCourseAsync(int idCourse);

        Task<IEnumerable<TrainingCurriculumUploadedFileVM>> GetTrainingCurriculumUploadedFilesForOldCoursesByIdCourseAsync(int idCourse);

        Task<ResultContext<NoResult>> DeleteTrainingCurriculumAsync(int idTrainingCurriculum);

        Task<ResultContext<TrainingCurriculumVM>> AddTrainingCurriculumAsync(ResultContext<TrainingCurriculumVM> inputContext, bool ignoreERUs = false);

        Task<ResultContext<TrainingCurriculumVM>> UpdateTrainingCurriculumAsync(ResultContext<TrainingCurriculumVM> inputContext);

        TrainingCurriculumERUVM GetTrainingCurriculumERUByIdTrainingCurriculumAndIdERU(int idTrainingCurriculum, int idEru);

        Task<ResultContext<NoResult>> DeleteTrainingCurriculumERUAsync(int idTrainingCurriculumERU);

        Task<CandidateProviderSpecialityVM> GetCandidateProviderSpecialityByIdCandidateProviderAndIdSpecialityAsync(int idCandidateProvider, int idSpeciality);

        Task<ResultContext<NoResult>> AddERUsToCurriculumListAsync(List<ERUVM> erus, List<TrainingCurriculumVM> curriculums);

        Task<ResultContext<List<TrainingCurriculumVM>>> ImportCurriculumAsync(MemoryStream file, string fileName);

        MemoryStream CreateExcelWithErrors(ResultContext<List<TrainingCurriculumVM>> resultContext);

        Task<int> GetIdCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderAsync(int idCandidateProvider, int idSpeciality);

        Task<ResultContext<NoResult>> DeleteListCandidateCurriculumAsync(List<TrainingCurriculumVM> trainingCurriculums);
        #endregion

        #region Training course
        Task<CourseVM> GetTrainingCourseByIdAsync(int idCourse);

        Task<CourseVM> GetTrainingCourseWithoutAnythingIncludedByIdAsync(int idCourse);

        Task<IEnumerable<CourseVM>> GetAllUpcomingTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider, int? idCourseType = null);

        Task<IEnumerable<CourseVM>> GetAllCurrentTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider, int? idCourseType = null);

        Task<IEnumerable<CourseVM>> GetAllCompletedTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider, int? idCourseType = null);

        Task<IEnumerable<CourseVM>> GetAllArchivedTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider, int? idCourseType = null);

        Task<IEnumerable<CourseVM>> GetAllCoursesWhichAreNotOnStatusUpcomingByIdCandidateProviderAsync(int idCandidateProvider);

        Task<string> AreDOSChangesWithoutActualizationOfCurriculumsAsync(CourseVM course);

        Task<ResultContext<NoResult>> DeleteTrainingCourseByIdAsync(int idCourse);

        Task<IEnumerable<CandidateProviderPremisesVM>> GetAllActiveCandidateProviderPremisesByIdCandidateProviderAndIdSpecialityAsync(int idCandidateProvider, int idSpeciality);

        Task<ResultContext<CourseVM>> CreateTrainingCourseAsync(ResultContext<CourseVM> inputContext);

        Task<ResultContext<CourseVM>> UpdateTrainingCourseAsync(ResultContext<CourseVM> inputContext);

        Task<ResultContext<CourseVM>> StartUpcomingTrainingCourseAsync(ResultContext<CourseVM> resultContext);

        Task<ResultContext<CourseVM>> CompleteCurrentTrainingCourseAsync(ResultContext<CourseVM> resultContext);

        Task UpdateTrainingCourseHoursByIdCourseAsync(int idCourse, int mandatoryHours, int selectableHours);

        Task<int> GetTrainingCourseIdTrainingTypeByIdCourseAsync(int idCourse);

        Task<IEnumerable<CourseVM>> GetAllArchivedAndOutOfOrderTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider);

        Task<ResultContext<CourseCollection>> ImportCourseAsync(MemoryStream file, string fileName);

        MemoryStream CreateExcelWithXMLImportValidationErrors(ResultContext<CourseCollection> resultContext);

        Task<ResultContext<CourseCollection>> ImportXMLCourseIntoDBAsync(ResultContext<CourseCollection> resultContext);

        Task<bool> IsAnnualReportSubmittedOrApprovedByIdCandidateProviderAndYearAsync(int idCandidateProvider, int year);

        Task<bool> IsAnyClientWithDocumentPresentByIdCourseAsync(int idCourse);

        Task<bool> IsSpecialityFromSPPOORemovedWithOrderBeforeCourseStartDateAsync(int idSpeciality, DateTime courseStartDate);

        Task<IEnumerable<CourseVM>> GetAllArchivedAndFinishedCoursesByIdCandidateProviderAndByIdCourseTypeAsync(int idCandidateProvider, int idCourseType);

        Task SetRIDPKCountAsync(List<CourseVM> courses);

        Task UpdateCourseFileNameAsync(int idCourse);

        Task<bool> IsCandidateProviderLicenceSuspendedAsync(int idCandidateProvider, DateTime courseStartDate);
        #endregion

        #region Training course checking
        Task<List<CourseCheckingVM>> GetAllActiveCourseCheckingsAsync(int IdCourse);
        Task<List<CourseCheckingVM>> GetAllActiveCourseCheckingsByIdFollowUpControlAsync(int IdCourse);
        Task<ResultContext<CourseCheckingVM>> CreateCourseCheckingAsync(ResultContext<CourseCheckingVM> resultContext);
        Task<ResultContext<CourseCheckingVM>> DeleteCourseCheckingAsync(CourseCheckingVM courseCheckingVM);
        #endregion

        #region Training premises course
        Task<IEnumerable<PremisesCourseVM>> GetAllPremisesCoursesByIdCourseAsync(int idCourse);

        Task<IEnumerable<CandidateProviderPremisesVM>> GetAllActiveCandidateProviderPremisesByIdTrainingTypeByIdSpecialityAndByIdCandidateProviderAsync(int idCandidateProvider, int idSpeciality, int idTrainingType, int idTrainingAndTheoryTypeId);

        Task<ResultContext<List<CandidateProviderPremisesVM>>> CreateTrainingCoursePremisesByListCandidateProviderPremisesVMAsync(ResultContext<List<CandidateProviderPremisesVM>> resultContext, int idCourse, int idTrainingType);

        Task<ResultContext<PremisesCourseVM>> DeletePremisesCourseAsync(ResultContext<PremisesCourseVM> resultContext);
        #endregion

        #region Training trainer course
        Task<IEnumerable<TrainerCourseVM>> GetAllTrainerCoursesByIdCourseAsync(int idCourse);

        Task<IEnumerable<TrainerCourseVM>> GetAllTrainerCoursesWithoutIncludesByIdCourseAsync(int idCourse);

        Task<ResultContext<TrainerCourseVM>> DeleteTrainerCourseAsync(ResultContext<TrainerCourseVM> resultContext);

        Task<ResultContext<List<CandidateProviderTrainerVM>>> CreateTrainingCourseTrainerByListCandidateProviderTrainerVMAsync(ResultContext<List<CandidateProviderTrainerVM>> resultContext, int idCourse, int idTrainingType);

        Task<IEnumerable<CandidateProviderTrainerVM>> GetAllActiveCandidateProviderTrainersByIdTrainingTypeByIdSpecialityAndByIdCandidateProviderAsync(int idCandidateProvider, int idSpeciality, int idTrainingType, int idTrainingAndTheoryTypeId);
        #endregion

        #region Training client course
        Task<IEnumerable<ClientCourseVM>> GetCourseClientsByIdCourseAsync(int idCourse);

        Task<IEnumerable<ClientCourseVM>> GetCourseClientsByIdCourseWithoutIncludesAsync(int idCourse);

        Task<IEnumerable<ClientCourseVM>> GetCourseClientsWithProtocolsAndDocsForDownloadByIdCourseAsync(int idCourse);

        Task<IEnumerable<ClientCourseVM>> GetCourseClientsByListIdsAsync(List<int> ids);

        Task<IEnumerable<ClientCourseVM>> GetAllClientCourseFromCoursesByIdCourseTypeAndIdCandidateProviderAsync(int idCandidateProvider, int idCourseType);

        Task<ResultContext<NoResult>> DeleteTrainingClientCourseByIdAsync(int idClientCourse);

        Task<ClientCourseVM> GetTrainingClientCourseByIdAsync(int idClientCourse);

        Task<ResultContext<ClientCourseVM>> CreateTrainingClientCourseAsync(ResultContext<ClientCourseVM> resultContext, int idCandidateProvider, int idTrainingCourseType);

        Task<ResultContext<ClientCourseVM>> UpdateTrainingClientCourseAsync(ResultContext<ClientCourseVM> resultContext, int idCandidateProvider, ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM = null, ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM = null, ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM = null);

        Task<ResultContext<ClientCourseVM>> UpdateLegalCapacityTrainingClientCourseAsync(ResultContext<ClientCourseVM> resultContext, int idCandidateProvider, ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM = null);

        Task<ResultContext<ClientVM>> GetClientByIdIndentTypeByIndentAndByIdCandidateProviderAsync(int idIndentType, string indent, int idCandidateProvider);

        Task<ResultContext<ClientCourseVM>> UpdateClientCoursesListFinishedDataAsync(ResultContext<ClientCourseVM> resultContext, List<ClientCourseVM> clientCourses);

        MemoryStream GetCourseClientsTemplate();

        Task<ResultContext<List<ClientCourseVM>>> ImportCourseClientsAsync(MemoryStream file, string fileName, CourseVM course, List<ClientCourseVM> addedClients);

        MemoryStream CreateClientCourseExcelWithErrors(ResultContext<List<ClientCourseVM>> resultContext);

        Task<bool> IsDocumentPresentAsync(int idClientCourse);

        Task<int?> GetTrainingClientCourseIdCityOfBirthByIdAsync(int idClientCourse);

        Task AddClientCourseDocumentStatusAsync(int idClientCourseDocument, int idStatus, string? comment = null);

        Task AddValidationClientDocumentStatusAsync(int idValidationClientDocument, int idStatus, string? comment = null);

        Task<IEnumerable<ClientCourseVM>> GetCourseClientsByIdCourseAndByIdCourseFinishedTypeAsync(int idCourse, int idCourseFinishedType);

        Task<bool> IsDuplicateIssuedByIdClientCourseAsync(int idClientCourse);
        #endregion

        #region Training course exam
        Task<IEnumerable<CourseCommissionMemberVM>> GetAllCourseCommissionMembersByIdCourseAsync(int idCourse);

        Task<ResultContext<CourseCommissionMemberVM>> CreateCourseCommissionMemberAsync(ResultContext<CourseCommissionMemberVM> resultContext);

        Task<ResultContext<NoResult>> DeleteCourseCommissionMemberByIdAsync(int idCourseCommissionMember);

        Task<IEnumerable<CourseCommissionMemberVM>> GetAllCourseCommissionChairmansByIdCourseAsync(int idCourse);

        Task<bool> IsChairmanAlreadyInProtocolAddedAsync(int idCourseCommissionMember, int idCourse);
        #endregion

        #region Training client course document
        Task<ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM> GetClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(int idClientCourse);

        Task<ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM> GetLegalCapacityClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(int idClientCourse);

        Task<ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM> GetClientCourseDuplicateClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(int idClientCourse);

        Task<IEnumerable<ClientCourseDocumentVM>> GetClientCourseDocumentsByIdCourseAsync(int idCourse);

        Task<ResultContext<NoResult>> SendDocumentsForVerificationAsync(List<ClientCourseDocumentVM> documents, string? comment);

        Task UpdateClientCourseDocumentStatusAsync(int idClientCourseDocument, int idDocumentStatus);

        Task<IEnumerable<ClientCourseDocumentVM>> GetAllIssuedDuplicatesFromCoursesByIdCandidateProviderAndByIdCourseTypeAsync(int idCandidateProvider, int idCourseType);

        Task<ResultContext<NoResult>> CreateDuplicateDocumentAsync(ResultContext<DuplicateIssueVM> inputContext);

        Task<ResultContext<NoResult>> UpdateDuplicateDocumentAsync(ResultContext<DuplicateIssueVM> inputContext);

        Task<IEnumerable<CourseDocumentUploadedFileVM>> GetAllClientCourseDocumentFilesAsync(int idClientCourse);

        Task<ClientCourseDocumentVM> GetClientCourseDocumentWithUploadedFilesByIdAsync(int idClientCourseDocument);

        Task<ClientCourseDocumentVM> GetClientCourseDocumentByIdClientCourseAsync(int idClientCourse);

        Task<ResultContext<ClientRequiredDocumentVM>> CreateClientRequiredDocumentAsync(ResultContext<ClientRequiredDocumentVM> resultContext);

        Task<ResultContext<ClientRequiredDocumentVM>> DeleteClientRequiredDocumentAsync(ClientRequiredDocumentVM resultContext);

        Task<ResultContext<ClientRequiredDocumentVM>> UpdateClientRequiredDocumentAsync(ResultContext<ClientRequiredDocumentVM> resultContext);

        Task<IEnumerable<ClientRequiredDocumentVM>> GetAllClientRequiredDocumentsByIdClientCourse(int id);

        Task<ClientRequiredDocumentVM> GetClientRequiredDocumentById(int id);

        Task UpdateValidationClientDocumentStatusAsync(int idValidationClientDocument, int idDocumentStatus);
        #endregion

        #region Training course subject
        Task<IEnumerable<CourseSubjectVM>> GetAllCourseSubjectsByIdCourseAsync(int idCourse);

        Task UpdateSubjectGradeAfterUpdateTrainingCurriculumAsync(int idCourse);

        Task<int> GetCourseSubjectEnteredTheoryGradesAsync(int idCourseSubject);

        Task<int> GetCourseSubjectEnteredPracticeGradesAsync(int idCourseSubject);
        #endregion

        #region Training client course subject grade
        Task<IEnumerable<CourseSubjectGradeVM>> GetClientCourseSubjectGradeListByClientCourseListIdsAndByIdCourseSubjectAsync(List<int> ids, int idCourseSubject);

        Task<CourseSubjectGradeVM> GetClientCourseSubjectGradeByClientCourseIdAndByIdCourseSubjectAsync(int id, int idCourseSubject);

        Task<ResultContext<CourseSubjectGradeVM>> UpdateClientCourseSubjectGradeAsync(ResultContext<CourseSubjectGradeVM> resultContext, bool isTheory);

        Task<bool> AreAllSubjectGradesForClientCourseAlreadyAddedByIdClientCourseAsync(int idClientCourse);
        #endregion

        #region Training course protocol
        Task<IEnumerable<CourseProtocolVM>> GetAllCourseProtocolsByIdCourseAsync(int idCourse);

        Task<CourseProtocolVM> GetCourseProtocolByIdAsync(int idCourseProtocol);

        Task<CourseProtocolVM> GetCourseProtocolWithoutIncludesByIdAsync(int idCourseProtocol);

        Task<ResultContext<CourseProtocolVM>> CreateCourseProtocolAsync(ResultContext<CourseProtocolVM> resultContext);

        Task<ResultContext<CourseProtocolVM>> UpdateCourseProtocolAsync(ResultContext<CourseProtocolVM> resultContext);

        Task<IEnumerable<ClientCourseVM>> GetAllClientsWhichAreNotAddedToProtocolByIdCourseAsync(int idCourse, List<CourseProtocolGradeVM> courseProtocolGrades);

        Task<bool> AreAllCourseClientsAlreadyAddedToCourseProtocolGradeByIdCourseAndByIdProtocolAsync(int idCourse, int idProtocol);

        Task<ResultContext<NoResult>> DeleteValidationProtocolByIdAsync(int idCourseProtocol);

        Task<IEnumerable<CourseProtocolVM>> GetAllCourseProtocolsByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<CourseProtocolVM>> GetAll381BProtocolsWhichHaveGradeAddedByIdCourseAndByIdClientCourseAsync(int idCourse, int idClientCourse);

        Task<bool> AreProtocols380TAnd380PAlreadyAddedByIdCourseAsync(int idCourse);

        Task<IEnumerable<CourseProtocolVM>> GetCourseProtocol381BByIdClientCourseAsync(int idClientCourse);

        #endregion

        #region Training course protocol grade
        Task<ResultContext<CourseProtocolVM>> AddAllCourseClientsToCourseProtocolGradeAsync(ResultContext<CourseProtocolVM> resultContext);
        Task<ResultContext<NoResult>> DeleteCourseProtocolByIdAsync(int idCourseProtocol);

        Task<IEnumerable<CourseProtocolGradeVM>> GetAllCourseProtocolGradesByIdProtocolAsync(int idProtocol);

        Task<ResultContext<CourseProtocolGradeVM>> UpdateCourseProtocolGradeAsync(ResultContext<CourseProtocolGradeVM> resultContext);

        Task<ResultContext<NoResult>> DeleteCourseProtocolGradeByIdAsync(int idCourseProtocolGrade);

        Task<ResultContext<NoResult>> AddCourseClientToCourseProtocolGradeAsync(int idClientCourse, int idCourseProtocol);

        Task<List<string>> GetTheoryAndPracticeGradesFromCourseProtocols380ByIdCourseAndIdCourseClient(int idCourse, int idClientCourse);

        #endregion

        #region Training course order
        Task<IEnumerable<CourseOrderVM>> GetAllCourseOrdersByIdCourseAsync(int idCourse);
        Task<ResultContext<CourseOrderVM>> CreateCourseOrderAsync(ResultContext<CourseOrderVM> resultContext);
        Task<ResultContext<CourseOrderVM>> UpdateCourseOrderAsync(ResultContext<CourseOrderVM> resultContext);
        Task<CourseOrderVM> GetCourseOrderByIdAsync(int idCourseOrder);
        Task<ResultContext<CourseOrderVM>> DeleteCourseOrderAsync(ResultContext<CourseOrderVM> resultContext);
        #endregion

        #region Consulting
        Task<IEnumerable<ConsultingVM>> GetAllConsultingByIdCandidateProviderAsync(int idCandidateProvider);
        #endregion

        #region Consulting client
        Task<IEnumerable<ConsultingClientVM>> GetAllConsultingClientsByIdCandidateProviderAsync(int idCandidateProvider);
        Task<ConsultingClientVM> GetConsultingClientByIdClientAsync(int idClient, int idCandidateProvider);
        Task<IEnumerable<ConsultingClientVM>> FilterConsultingClients(ConsultingClientVM filter, int userPropsIdCandidateProvider);
        Task<ResultContext<NoResult>> DeleteConsultingClientByIdAsync(int idConsultingClient);

        Task<ConsultingClientVM> GetConsultingClientByIdAsync(int idClientConsulting);

        Task<ConsultingFinishedDataVM> GetConsultingClientFinishedDataByIdConsultingClientAsync(int idConsultingClient);

        Task<ResultContext<ConsultingClientVM>> CreateConsultingClientAsync(ResultContext<ConsultingClientVM> resultContext, int idCandidateProvider);

        Task<ResultContext<ConsultingClientVM>> UpdateConsultingClientAsync(ResultContext<ConsultingClientVM> resultContext, int idCandidateProvider, ConsultingFinishedDataVM consultingFinishedDataVM = null);
        Task<IEnumerable<ConsultingClientRequiredDocumentVM>> GetAllConsultingClientRequiredDocumentsByIdConsultingClientAsync(int id);
        Task<ResultContext<ConsultingClientRequiredDocumentVM>> CreateConsultingClientRequiredDocumentAsync(ResultContext<ConsultingClientRequiredDocumentVM> resultContext);
        Task<ResultContext<ConsultingClientRequiredDocumentVM>> UpdateConsultingClientRequiredDocumentAsync(ResultContext<ConsultingClientRequiredDocumentVM> resultContext);
        Task<ConsultingClientRequiredDocumentVM> GetConsultingClientRequiredDocumentById(int id);
        Task<List<ConsultingDocumentUploadedFileVM>> GetConsultingClientDocumentUploadedFileById(int id);
        Task<ResultContext<ConsultingClientRequiredDocumentVM>> DeleteConsultingClientRequiredDocumentAsync(int idConsultingClient, int idConsultingDocumentUploadedFile);
        #endregion

        #region Consulting client premises
        Task<IEnumerable<ConsultingPremisesVM>> GetAllConsultingPremisesByIdConsultingClientAsync(int idConsultingClient);

        Task<ResultContext<ConsultingPremisesVM>> DeleteConsultingPremisesAsync(ResultContext<ConsultingPremisesVM> resultContext);

        Task<ResultContext<List<CandidateProviderPremisesVM>>> CreateConsultingPremisesByListCandidateProviderPremisesVMAsync(ResultContext<List<CandidateProviderPremisesVM>> resultContext, int idConsultingClient);

        #endregion

        #region Consulting client trainer
        Task<IEnumerable<ConsultingTrainerVM>> GetAllConsultingTrainersByIdConsultingClientAsync(int idConsultingClient);

        Task<ResultContext<ConsultingTrainerVM>> DeleteConsultingTrainerAsync(ResultContext<ConsultingTrainerVM> resultContext);

        Task<ResultContext<List<CandidateProviderTrainerVM>>> CreateConsultingTrainersByListCandidateProviderTrainerVMAsync(ResultContext<List<CandidateProviderTrainerVM>> resultContext, int idConsultingClient);
        #endregion

        #region Training course schedule
        Task<IEnumerable<CourseScheduleVM>> GetCourseSchedulesBydIdCourseAsync(int idCourse);

        Task<CourseScheduleVM> GetCourseScheduleByIdAsync(int idCourseSchedule);

        Task<ResultContext<NoResult>> DeleteCourseScheduleByIdAsync(int idCourseSchedule);

        Task<IEnumerable<CandidateProviderTrainerVM>> GetTrainersByIdTrainingTypeByIdCourseAsync(int idTrainingType, int idCourse);

        Task<IEnumerable<CandidateProviderPremisesVM>> GetPremisesByIdTrainingTypeByIdCourseAsync(int idTrainingType, int idCourse);

        Task<ResultContext<CourseScheduleVM>> CreateCourseScheduleAsync(ResultContext<CourseScheduleVM> resultContext);

        Task<ResultContext<CourseScheduleVM>> UpdateCourseScheduleAsync(ResultContext<CourseScheduleVM> resultContext);

        Task<IEnumerable<TrainingCurriculumVM>> GetAllTrainingCurriculumsByIdCourseAndByHoursTypeAsync(int idCourse, string hoursType);

        Task<ResultContext<NoResult>> AddPremisesToListCourseSchedulesAsync(int idCandidateProviderPremises, List<int> scheduleListIds);

        Task<ResultContext<NoResult>> AddTrainerToListCourseSchedulesAsync(int idCandidateProviderTrainer, List<int> scheduleListIds);

        Task<MemoryStream> PrintSchedulePlanAsync(List<CourseScheduleVM> addedSchedules, CandidateProviderVM candidateProvider, List<ClientCourseVM> clients, int idSpeciality, DateTime? courseStartDate);

        Task<MemoryStream> PrintScheduleProfessionalTrainingAsync(CourseVM course);

        Task<MemoryStream> GetCourseScheduleTemplateWithCurriculumsFilledByIdCourseAsync(int idCourse);

        Task<ResultContext<List<CourseScheduleVM>>> ImportCourseScheduleAsync(MemoryStream file, string fileName, CourseVM course);

        MemoryStream CreateCourseScheduleExcelWithErrors(ResultContext<List<CourseScheduleVM>> resultContext);

        Task CreateCourseSchedulesFromListAsync(List<CourseScheduleVM> schedules, int idCourse);

        Task<ResultContext<NoResult>> DeleteListCurriculumSchedulesAsync(List<CourseScheduleVM> schedules);

        #endregion

        #region RIDPK
        Task<IEnumerable<RIDPKVM>> GetListRIDPKVMOfSubmittedDocumentsForControlFromCPOAsync(string type);

        Task<bool> IsRIDPKDocumentAlreadyReturnedAsync(RIDPKDocumentVM document);

        Task<IEnumerable<RIDPKDocumentVM>> GetRIDPKDocumentsDataAsync(string type, RIDPKVM model);

        Task<ResultContext<NoResult>> ApproveRIDPKDocumentsAsync(string type, List<RIDPKDocumentVM> documents, string? comment);

        Task<ResultContext<NoResult>> ReturnRIDPKDocumentsAsync(string type, List<RIDPKDocumentVM> documents, string? comment);

        Task<IEnumerable<CourseDocumentUploadedFileVM>> GetCourseDocumentUploadedFileByIdClientCourseDocumentAsync(int idClientCourseDocument);

        Task<IEnumerable<ValidationDocumentUploadedFileVM>> GetValidationDocumentUploadedFileByIdValidationClientDocumentAsync(int idValidationClientDocument);

        Task<IEnumerable<DocumentStatusVM>> GetDocumentStatusesByIdAsync(int idEntity, string type);

        Task<ResultContext<NoResult>> RejectRIDPKDocumentsAsync(string type, List<RIDPKDocumentVM> documents, string? comment);

        Task<ResultContext<NoResult>> ChangeRIDPKStatusForListClientCourseDocumentIdsAsync(List<int> documentIds, int idDocumentStatus, string? comment);

        Task<ResultContext<NoResult>> ChangeRIDPKStatusForListValidationClientDocumentIdsAsync(List<int> documentIds, int idDocumentStatus, string? comment);
        #endregion

        #region Reports

        Task<List<CourseVM>> GetAllCoursesAsync(CoursesFilterVM filterModel);

        Task<List<DocumentsFromCPORegisterVM>> GetRIDPKDocumentsForRegisterAsync(TrainedPeopleFilterVM filterModel);

        Task<List<CourseVM>> filterCourses(CourseVM filter);
        List<ClientCourseDocumentVM> FilterClientCourseDocuments(ClientCourseDocumentVM model);
        Task<List<CourseVM>> getAllCourses(StateExaminationInfoFilterListVM modelFilterStateExam, string isFromStateExamPage);
        //Task<List<CourseVM>> GetFilterTrainingCourses(StateExaminationInfoFilterListVM modelFilterStateExam);
        Task<List<ClientCourseDocumentVM>> GetAllDocuments();
        List<CourseDocumentUploadedFileVM> getCourseDocumentUploadedFile(int idClientCourseDocument);
        List<ClientCourseDocumentVM> GetDocumentsByCandidateProvder(CandidateProviderVM candidate);
        Task<List<ClientVM>> GetAllCIPOClients();
        Task<List<ClientVM>> FilterClients(ClientFilterVM filter);
        Task<MemoryStream> GenerateExcelReportForCoursesAndClients(string year);
        Task<MemoryStream> getCoursesReportStream(CoursesFilterVM filter);
        Task<List<GetCPOWithOutCourseByPeriod>> GetCPOWithOutCourseByPeriod(CoursesFilterVM filter);
        Task<List<CourseVM>> filterCourses(CourseVM filter, string type, int idCandidateProvider);
        Task<List<ClientCourseVM>> GetCourseClientsByIdCourseStatisticsAsync(int idCourse);

        Task<List<ClientCourseVM>> GetAllTrainedPeopleFilterAsync(TrainedPeopleFilterVM filterModel);

        Task SetDocumentStatusForListDocumentsFromCPORegisterVMAsync(IEnumerable<DocumentsFromCPORegisterVM> docs);

        #endregion

        #region Validation
        Task<List<ValidationClientVM>> getAllValidationClients();
        Task<ValidationClientVM> CreateValidationClient(ValidationClientVM clientVM);
        Task<ValidationClientVM> GetValidationClientWithoutIncludesByIdAsync(int idValidationClient);
        Task<int> UpdateValidationDuplicateDocumentProtocolAsync(ValidationClientCombinedVM ValidationClientModel);
        Task<ValidationClientVM> GetValidationClientByIdAsync(int id);
        Task<ValidationClientVM> UpdateValidationClientAsync(ValidationClientVM clientVM);
        Task<IEnumerable<ValidationCommissionMemberVM>> GetAllValidationCommissionMembersByClient(int idValidationClient);
        Task<ResultContext<ValidationCommissionMemberVM>> CreateValidationCommissionMemberAsync(ResultContext<ValidationCommissionMemberVM> result);
        Task<bool> DeleteValidationClientCommissionMemberByIdAsync(int idValidationClient);
        Task<IEnumerable<ValidationTrainerVM>> GetAllValidationTrainersByIdValidationClientAsync(int idValidationClient);
        Task<ResultContext<ValidationTrainerVM>> DeleteValidationTrainerAsync(ResultContext<ValidationTrainerVM> result);
        Task<ResultContext<List<CandidateProviderTrainerVM>>> CreateValidationTrainerByListCandidateProviderTrainerVMAsync(ResultContext<List<CandidateProviderTrainerVM>> resultContext, int idValidationClient, int key);
        Task<List<ValidationPremisesVM>> GetAllValidationPremisiesByIdCourseAsync(int idValidationClient);
        Task<ResultContext<ValidationPremisesVM>> DeleteValidationPremisesAsync(ResultContext<ValidationPremisesVM> result);
        Task<ResultContext<List<CandidateProviderPremisesVM>>> CreateTrainingValidationPremisesByListCandidateProviderPremisesVMAsync(ResultContext<List<CandidateProviderPremisesVM>> resultContext, int idValidationClient, int key);
        Task<ValidationClientCombinedVM> GetValidationClientCombinedVMByIdClientCourseAsync(int idVaLidationClient);
        Task<IEnumerable<ValidationProtocolVM>> GetAll381BProtocolsAddedByIdValidationClientAsync(int idValidationClient);
        Task<int> UpdateValidationDocumentProtocolAsync(ValidationClientCombinedVM model);
        Task<ValidationClientCombinedVM> CreateValidationDocumentProtocolAsync(ValidationClientCombinedVM model);
        Task<List<string>> GetTheoryAndPracticeGradesFromValidationProtocols380ByIdCourseAndIdCourseClient(int idValidationClient);
        Task<ResultContext<ValidationProtocolVM>> UpdateValidationProtocolAsync(ResultContext<ValidationProtocolVM> result);
        Task<ResultContext<ValidationProtocolVM>> CreateValidationProtocolAsync(ResultContext<ValidationProtocolVM> result);
        Task<ResultContext<ValidationProtocolGradeVM>> UpdateValidationProtocolGradeAsync(ResultContext<ValidationProtocolGradeVM> result);
        Task<IEnumerable<ValidationProtocolVM>> GetAllValidationProtocolsByValidationClientId(int idValidationClient);
        Task<ValidationProtocolVM> GetValidationProtocolByIdAsync(int idValidationClient);
        Task<ResultContext<ValidationClientRequiredDocumentVM>> CreateValidationRequiredDocumentAsync(ResultContext<ValidationClientRequiredDocumentVM> result);
        Task<ResultContext<ValidationClientRequiredDocumentVM>> UpdateValidationRequiredDocumentAsync(ResultContext<ValidationClientRequiredDocumentVM> result);
        Task<ValidationClientRequiredDocumentVM> GetValidationRequiredDocumentById(int idValidationClientRequiredDocument);
        Task<IEnumerable<ValidationClientRequiredDocumentVM>> GetAllValidationRequiredDocumentsByIdClient(int idValidationClient);
        Task<List<ClientCourseDocumentVM>> GetAllDocumentsByIdTypeOfRequestedDocument(int idTypeOfRequestedDocument);
        Task<List<ClientCourseDocumentVM>> GetAllDocumentsByIdTypeOfRequestedDocument(int?[] idTypesOfRequestedDocument);
        Task<bool> UpdateValidationCompetencyAsync(ValidationCompetencyVM validationCompetencyVM);
        Task<ValidationCompetencyVM> CreateValidationCompetencyAsync(ValidationCompetencyVM validationCompetencyVM);
        Task<bool> DeleteValidationCompetencies(List<ValidationCompetencyVM> selected);
        Task<IEnumerable<ValidationCommissionMemberVM>> GetAllValidationCommissionChairmansByClient(int idValidationClient);
        Task<IEnumerable<ValidationOrderVM>> GetAllValidationOrdersByIdValidationClient(int idValidationClient);
        Task<ResultContext<ValidationOrderVM>> CreateValidationOrderAsync(ResultContext<ValidationOrderVM> resultContext);
        Task<ResultContext<ValidationOrderVM>> UpdateValidationOrderAsync(ResultContext<ValidationOrderVM> resultContext);
        Task<ValidationOrderVM> GetValidationOrderByIdAsync(int idValidationOrder);
        Task<ResultContext<ValidationOrderVM>> DeleteValidationOrderAsync(ResultContext<ValidationOrderVM> resultContext);
        Task<ValidationCurriculumVM> GetValidationCurriculumByIdAsync(int idValidationCurriculum);
        Task<ResultContext<ValidationCurriculumVM>> AddValidationCurriculumAsync(ResultContext<ValidationCurriculumVM> inputContext, bool ignoreERUs = false);
        Task<ResultContext<ValidationCurriculumVM>> UpdateValidationCurriculumAsync(ResultContext<ValidationCurriculumVM> inputContext);
        ValidationCurriculumERUVM GetValidationCurriculumERUByIdTrainingCurriculumAndIdERU(int idValidationCurriculum, int idERU);
        Task<ResultContext<NoResult>> DeleteValidationCurriculumERUAsync(int idValidationCurriculumERU);
        Task<ResultContext<NoResult>> AddERUsToValidationCurriculumListAsync(List<ERUVM> selectedERUs, List<ValidationCurriculumVM> validationCurriculums);
        Task<ResultContext<List<ValidationCurriculumVM>>> ImportValidationCurriculumAsync(MemoryStream uploadedFileStream, string uploadedFileName);
        MemoryStream CreateValidationExcelWithErrors(ResultContext<List<ValidationCurriculumVM>> resultContext);
        Task<IEnumerable<ValidationCurriculumVM>> GetValidationCurriculumByIdValidationClientAsync(int idValidationClient);
        Task<ResultContext<NoResult>> DeleteValidationCurriculumAsync(int idValidationCurriculum);
        Task<ResultContext<NoResult>> DeleteListCandidateValidationCurriculumAsync(List<ValidationCurriculumVM> selectedCurriculums);
        Task<IEnumerable<ValidationClientDocumentVM>> GetValidationClientDocumentsByIdValidationClientAsync(int idValidationClient);
        Task<ResultContext<NoResult>> SendValidationDocumentsForVerificationAsync(List<ValidationClientDocumentVM> documents, string? comment);
        Task<ResultContext<ValidationClientVM>> CompleteValidationAsync(ResultContext<ValidationClientVM> resultContext);
        Task<ValidationClientCombinedVM> GetValidationClientDuplicateCombinedVMByIdClientCourseAsync(int idValidationClient);
        Task<bool> IsRIDPKDocumentSubmittedOrEnteredInRegisterByIdValidationClientAsync(int idValidationClient);
        Task<ResultContext<NoResult>> AddValidationDocumentToIS(ValidationClientVM client);
        Task<bool> IsDuplicateIssuedByIdValidationClientAsync(int idValidationClient);
        Task<IEnumerable<ValidationClientDocumentVM>> GetAllIssuedDuplicatesFromValidationsByIdCandidateProviderAndByIdCourseTypeAsync(int idCandidateProvider, int idCourseType);
        Task<IEnumerable<ValidationClientVM>> GetAllArchivedAndFinishedValidationsByIdCandidateProviderByIdCourseTypeAndByIdFinishedTypeAsync(int idCandidateProvider, int idCourseType, int idFinishedType);
        Task<ResultContext<NoResult>> CreateValidationDuplicateDocumentAsync(ResultContext<DuplicateIssueVM> inputContext);
        Task<ResultContext<NoResult>> UpdateValidationDuplicateDocumentAsync(ResultContext<DuplicateIssueVM> inputContext);
        Task<IEnumerable<ValidationProtocolVM>> GetValidationProtocol381BByIdValidationClientAsync(int idValidationClient);
        Task UpdateValidationClientCurriculumFileNameAsync(int idValidationClient);
        Task<IEnumerable<TrainingCurriculumUploadedFileVM>> GetValidationClientCurriculumUploadedFilesForOldValidationClientsByIdValidationClientAsync(int idValidationClient);
        Task<ValidationClientDocumentVM> GetValidationClientDocumentWithUploadedFilesByIdAsync(int idValidationClientDocument);
        Task<List<ValidationClientVM>> FilterValidationClients(ValidationClientFIlterVM model);
        Task<ResultContext<ValidationClientRequiredDocumentVM>> DeleteValidationClientRequiredDocumentAsync(ValidationClientRequiredDocumentVM resultContext);

        Task<IEnumerable<ValidationCurriculumVM>> GetValidationCurriculumsWithoutAnythingIncludedByIdCourseAsync(int idValidationClient);

        Task<IEnumerable<ValidationDocumentUploadedFileVM>> GetValidationDocumentUploadedFilesByIdValidationClientAsync(int idValidationClient);

        #endregion

        #region ValidationClientChecking
        Task<List<ValidationClientCheckingVM>> GetAllActiveValidationClientCheckingsAsync(int IdValidationClient);
        Task<List<ValidationClientCheckingVM>> GetAllActiveValidationClientCheckingsByIdFollowUpControlAsync(int IdFollowUpControl);
        Task<ResultContext<ValidationClientCheckingVM>> CreateValidationClientCheckingAsync(ResultContext<ValidationClientCheckingVM> resultContext);
        Task<ResultContext<ValidationClientCheckingVM>> DeleteValidationClientCheckingAsync(ValidationClientCheckingVM validationClientCheckingVM);
        Task<List<ClientCourseDocumentVM>> GetAllProfessionalCertificateDocuments(TrainedPeopleFilterVM model);
        #endregion
    }
}

