using Data.Models.Data.Candidate;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.Core.ViewModels.Register;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Candidate
{
    public interface ICandidateProviderService
    {
        #region Candidate Provider
        Task<ResultContext<CandidateProviderVM>> CreateCandidateProvider(ResultContext<CandidateProviderVM> inputContext);

        Task<IEnumerable<CandidateProviderVM>> GetAllActiveProceduresForRegisterAsync(bool isCPO = true);

        Task<IEnumerable<CandidateProviderVM>> GetFirstLicencingCandidateProviderByIdCandidateProviderAsync(int idCandidateProvider, bool isCPO = true);

        Task<IEnumerable<CandidateProviderVM>> GetLicencingChangeCandidateProvidersByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderVM>> GetAllActiveProceduresAsync(bool isCPO = true);

        Task<IEnumerable<CandidateProviderVM>> GetAllCandidateProvidersAsync();

        Task<ResultContext<NoResult>> ChangeProviderToUserAsync(int idCandidateProvider);

        Task<ResultContext<NoResult>> UpdateCandidateProviderAsync(ResultContext<CandidateProviderVM> inputContext);

        Task<IEnumerable<CandidateProviderVM>> GetAllInаctiveCandidateProvidersByIdActiveCandidateProviderAsync(int idCandidateProvider);
        Task<IEnumerable<CandidateProviderVM>> GetAllActiveCandidateProvidersWithoutAnythingIncludedAsync(CandidateProviderVM model);

        Task<IEnumerable<CandidateProviderVM>> GetAllActiveCPOCandidateProvidersWithoutAnythingIncludedAsync();

        Task<IEnumerable<CandidateProviderSpecialityVM>> GetAllCandidateProviderSpecialitiesWithActualCurriculumsByIdCandidateProviderAsync(int idCandidateProvider);

        Task<int> CreateApplicationChangeCandidateProviderAsync(int idCandidateProvider, int idTypeApplication);

        Task ChangeCandidateProviderApplicationStatusAsync(int idCandidateProvider, int idApplicationStatus);

        Task<bool> IsOnlyOneProfileAdministratorByIdCandidateProviderAsync(int idCandidateProvider, int idPerson);

        Task<IEnumerable<CandidateProviderVM>> GetAllActiveCandidateProvidersWithoutIncludesAsync(string keyValueIntCode);

        Task<IEnumerable<CandidateProviderVM>> GetAllActiveCandidateProvidersAsync(string keyValueIntCode, string? applicationStatus = null);
        Task<List<CandidateProviderTrainerCheckingVM>> GetAllActiveCandidateProviderTrainerCheckingAsync(int IdCandidateProviderTrainer);
        Task<List<CandidateProviderTrainerCheckingVM>> GetAllActiveCandidateProviderTrainerCheckingByIdFollowUpControlAsync(int IdFollowUpControl);
        Task<IEnumerable<CandidateProviderVM>> GetAllActiveCandidateProvidersSpecialitiesAsync(string keyValueIntCode);

        Task<IEnumerable<CandidateProviderSpecialityVM>> GetProviderSpecialitiesWithProfessionIncludedByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderSpecialityVM>> GetProviderSpecialitiesWithoutIncludesByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderVM>> GetAllApplicationsByIdCandidateProviderAsync(int idCandidateProvider);
        Task<List<CandidateProviderTrainerProfileVM>> GetAllCandidateProviderTrainerProfilesByCandidateProviderTrainerIdAsync(int IdCandidateProviderTrainer);

        Task<IEnumerable<CandidateProviderVM>> GetAllCIPOApplicationsByIdCandidateProviderAsync(int idCandidateProvider);

        Task<CandidateProviderVM> GetCandidateProviderByIdAsync(CandidateProviderVM candidateProviderVM);

        Task<ResultContext<NoResult>> CreateCandidateProviderUserAsync(ResultContext<TokenVM> resultContext, bool isProfileAdministrator = false);

        Task<ResultContext<CandidateProviderVM>> CreateApplicationAsync(ResultContext<CandidateProviderVM> inputContext);

        Task<ResultContext<CandidateProviderVM>> CreateApplicationChangeAsync(ResultContext<CandidateProviderVM> inputContext);

        Task<IEnumerable<CandidateProviderVM>> GetAllExpertsAsync(CandidateProviderVM filterExpertVM);

        Task<ResultContext<List<CandidateProviderVM>>> ApproveRegistrationAsync(ResultContext<List<CandidateProviderVM>> inputContext);
        Task<ResultContext<List<CandidateProviderVM>>> RejectRegistrationAsync(ResultContext<List<CandidateProviderVM>> inputContext, string reason);

        Task<CandidateProviderSpecialityVM> GetCandidateProviderSpecialityBySpecialityIdAndCandidateProviderIdAsync(SpecialityVM specialityVM, CandidateProviderVM candidateProviderVM);

        Task<CandidateProviderVM> GetCandidateProviderWithoutAnythingIncludedByIdAsync(int id);

        Task<IEnumerable<CandidateProviderVM>> GetCandidateProvidersByListIdsAsync(List<int> ids);

        Task<MemoryStream> GenerateExcelReportForCandidateProviders(string year);

        Task<IEnumerable<CandidateProviderVM>> GetCandidateProvidersByListIdsWithIncludeAsync(List<int> ids);

        Task<List<CandidateProviderVM>> GetCandidateProvidersAsync(List<int> idPerson);
        Task<List<CandidateProviderPersonVM>> GetCandidateProviderPerson(int? idCandidateProvider);

        Task<bool> IsCandidateProviderPersonProfileAdministratorByIdPersonAsync(int idPerson, int idCandidateProvider);

        Task<CandidateProviderVM> GetActiveCandidateProviderByIdAsync(int idCandidateProvider);

        Task<string> ApproveChangedApplicationAsync(CandidateProviderVM candidateProviderVM, ProcedureDocumentVM procedureDocument);

        Task<string> SetCandidateProviderUINValueAsync(CandidateProviderVM candidateProviderVM);

        Task<IEnumerable<CandidateProviderSpecialityVM>> GetAllCandidateProviderSpecialitiesByIdCandidateProvider(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderVM>> FilterCandidateProvidersAsync(NAPOOCandidateProviderFilterVM nAPOOCandidateProviderFilterVM, string filterType, bool licenceDeactivated);

        Task SetCandidateProviderLicenseNumberAsync(CandidateProviderVM candidateProvider);

        Task SetCandidateProviderLicenseDateAsync(CandidateProviderVM candidateProvider);

        Task<List<CandidateProviderVM>> GetAllActiveCandidateProvidersTrainersByIdCandidateProviderAsync(int IdCanidateProvider);

        Task<IEnumerable<CandidateProviderVM>> GetAllCandidateProvidersWithLicenseDeactivatedAsync(string keyValueIntCode);

        Task<ResultContext<NoResult>> DeleteCandidateProviderSpecialityByIdAsync(int idCandidateProviderSpeciality);

        Task<bool> DoesApplicationChangeOnStatusDifferentFromProcedureCompletedExistAsync(int idCandidateProvider);

        List<int> GetCandidateProviderIdsBySpecialityIdsAndByIsActive(List<int> specialityIds);

        MemoryStream CreateExcelApplicationValidationErrors(ResultContext<CandidateProviderExcelVM> inputContext);

        Task<string> UpdateProviderAfterRegixCheckAsync(CandidateProviderVM candidateProvider, string address, string postCode, int idLocation, string providerOwner);

        Task<List<int?>> GetCandidateProviderIdApplicationFilingAndIdApplicationReceiveByIdCandidateProviderAsync(int idCandidateProvider);

        Task<CandidateProviderSpecialityVM> GetCandidateProviderSpecialityByIdCandidateProviderAndByIdSpecialityAsync(int idCandidateProvider, int idSpeciality);
        Task<ResultContext<CandidateProviderVM>> UpdateCandidateProviderApplicationNumber(ResultContext<CandidateProviderVM> candidateProvider);

        Task<bool> AreAnyCandidateProviderSpecialitiesByIdCandidateProviderAsync(int idCandidateProvider);

        Task<bool> AreAnyCandidateProviderTrainersByIdCandidateProviderAsync(int idCandidateProvider);

        Task<bool> AreAnyCandidateProviderPremisesByIdCandidateProviderAsync(int idCandidateProvider);

        Task<bool> AreAnyCandidateProviderDocumentsByIdCandidateProviderAsync(int idCandidateProvider);

        Task<CandidateProviderVM> GetActiveCandidateProviderWithLocationIncludedByIdAsync(int idCnadidateProvider);

        #endregion

        #region Curriculum
        Task<ResultContext<NoResult>> CreateCandidateProviderSpecialityAsync(int idCandidateProvider, int idSpeciality);

        Task<ResultContext<NoResult>> UpdateCandidateProviderSpecialityIdFrameworkAndIdFormEducationAsync(int idCandidateProviderSpeciality, int? idFrameworkProgram, int? idFormEducation);

        MemoryStream CreateExcelCurriculumValidationErrors(ResultContext<CandidateCurriculumExcelVM> resultContext, double compulsoryHours, double nonCompulsoryHours, double percentCompulsoryHours, double percentSpecificTraining);

        Task<List<string>> AreDOSChangesWithoutActualizationOfCurriculumsAsync(CandidateProviderVM candidateProvider);

        Task<MemoryStream> PrintCurriculumAsync(
            FrameworkProgramVM frameworkProgram, SpecialityVM speciality, double totalHours, double ATotalHours, double BTotalHours,
            string companyName, bool isInvalid, CandidateProviderSpecialityVM candidateProviderSpeciality, CandidateProviderVM candidateProvider, List<CandidateCurriculumVM> curriculums = null, List<TrainingCurriculumVM> trainingCurriculums = null, bool showInvalidCurriculumText = false);

        Task<MemoryStream> GenerateExcelReportCurriculum(string year);
        #endregion

        #region Document
        Task<CandidateProviderDocumentVM> GetCandidateProviderDocumentByIdAsync(CandidateProviderDocumentVM candidateProviderDocumentVM);

        Task<ResultContext<CandidateProviderDocumentVM>> CreateCandidateProviderDocumentAsync(CandidateProviderDocumentVM candidateProviderDocumentVM, bool isAdditionalDocument = false);

        Task<ResultContext<CandidateProviderDocumentVM>> DeleteCandidateProviderDocumentAsync(CandidateProviderDocumentVM candidateProviderDocumentVM);

        Task<ResultContext<CandidateProviderDocumentVM>> UpdateCandidateProviderDocumentAsync(CandidateProviderDocumentVM candidateProviderDocumentVM, bool isAdditionalDocument = false);

        Task<IEnumerable<CandidateProviderDocumentVM>> GetAllCandidateProviderDocumentsByCandidateProviderIdAsync(CandidateProviderDocumentVM candidateProviderDocumentVM);

        Task<IEnumerable<CandidateProviderDocumentsGridData>> SetDataForDocumentsGrid(int candidateProviderId, IEnumerable<KeyValueVM> kvProviderDocumentTypeSource,
            IEnumerable<KeyValueVM> kvMTBDocumentTypeSource, IEnumerable<KeyValueVM> kvTrainerDocumentTypeSource, bool isAdditionalDocument = false);
        #endregion

        #region Trainer
        Task<CandidateProviderTrainerVM> GetCandidateProviderTrainerByIdAsync(CandidateProviderTrainerVM candidateProviderTrainerVM);

        Task<CandidateProviderTrainerVM> GetCandidateProviderTrainerWithDocumentsByIdAsync(CandidateProviderTrainerVM candidateProviderTrainerVM);

        Task<IEnumerable<CandidateProviderTrainerVM>> GetCandidateProviderTrainersWithStatusByIdCandidateProviderAsync(int idCandidateProvider);

        IEnumerable<CandidateProviderTrainerVM> GetCandidateProviderTrainersByCandidateProviderId(CandidateProviderVM candidateProviderVM);

        Task<IEnumerable<CandidateProviderTrainerVM>> GetAllActiveTrainersByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderTrainerVM>> GetAllActiveCandidateProviderTrainersByCandidateProviderIdWithTrainerSpecialitiesIncludedAsync(int idCandidateProvider);
        Task<IEnumerable<CandidateProviderTrainerVM>> GetAllCandidateProviderTrainersByCandidateProviderIdWithTrainerSpecialitiesIncludedAsync(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderTrainerVM>> GetCandidateProviderTrainersByIdCandidateProviderAsync(int idCandidateProvider);

        Task<CandidateProviderTrainerVM> GetCandidateProviderTrainerWithoutIncludesByIdAsync(int idCandidateProviderTrainer);

        Task<CandidateProviderTrainerVM> GetCandidateProviderTrainerForRegisterByIdAsync(int idCandidateProviderTrainer);

        Task CreateCandidateProviderTrainerAsync(CandidateProviderTrainerVM model);

        Task UpdateCandidateProviderTrainerAsync(CandidateProviderTrainerVM model);
        #endregion

        #region Trainer Qualification
        Task<CandidateProviderTrainerQualificationVM> GetCandidateProviderTrainerQualificationByIdAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM);

        Task<IEnumerable<CandidateProviderTrainerQualificationVM>> GetAllCandidateProviderTrainerQualificationsByCandidateProviderTrainerIdAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM);

        Task<ResultContext<CandidateProviderTrainerQualificationVM>> UpdateCandidateProviderTrainerQualificationAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM);

        Task<ResultContext<CandidateProviderTrainerQualificationVM>> CreateCandidateProviderTrainerQualificationAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM);

        Task<ResultContext<CandidateProviderTrainerQualificationVM>> DeleteCandidateProviderTrainerQualificationAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM);

        Task<MemoryStream> GenerateExcelReportForCandidateProviderTrainerQualification(string year);

        #endregion

        #region Trainer Document
        Task<CandidateProviderTrainerDocumentVM> GetCandidateProviderTrainerDocumentByIdAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM);

        Task<ResultContext<CandidateProviderTrainerDocumentVM>> CreateCandidateProviderTrainerDocumentAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM);

        Task<ResultContext<CandidateProviderTrainerDocumentVM>> DeleteCandidateProviderTrainerDocumentAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM);

        Task<ResultContext<CandidateProviderTrainerDocumentVM>> UpdateCandidateProviderTrainerDocumentAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM);

        Task<IEnumerable<CandidateProviderTrainerDocumentVM>> GetAllCandidateProviderTrainerDocumentsByCandidateProviderTrainerIdAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM);

        Task<ResultContext<CandidateProviderTrainerCheckingVM>> CreateCandidateProviderTrainerCheckingAsync(ResultContext<CandidateProviderTrainerCheckingVM> resultContext);

        Task<ResultContext<CandidateProviderPremisesCheckingVM>> CreateCandidateProviderPremisesCheckingAsync(ResultContext<CandidateProviderPremisesCheckingVM> resultContext);

        #endregion

        #region Trainer Speciality
        Task<CandidateProviderTrainerSpecialityVM> GetCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderTrainerAsync(int idSpeciality, int idCandidateProviderTrainer, int idUsage);
        List<CandidateProviderTrainerSpecialityVM> GetCandidateProviderTrainerSpecialitiesWithSpecialitiesByIdCandidateProviderTrainer(int idCandidateProviderTrainer);
        Task<ResultContext<CandidateProviderTrainerCheckingVM>> DeleteCandidateProviderTrainerCheckingAsync(CandidateProviderTrainerCheckingVM candidateProviderTrainerCheckingVM);
        Task<ResultContext<CandidateProviderTrainerSpecialityVM>> DeleteCandidateProviderTrainerSpecialityAsync(CandidateProviderTrainerSpecialityVM candidateProviderTrainerSpecialityVM, int idUsage);

        Task<ResultContext<NoResult>> AddSpecialitiesToTrainerByListSpecialitiesAsync(List<CandidateProviderTrainerSpecialityVM> trainerSpecialities);
        #endregion

        #region Trainer Profile
        Task<ResultContext<CandidateProviderTrainerProfileVM>> DeleteCandidateProviderTrainerProfileAsync(CandidateProviderTrainerProfileVM candidateProviderTrainerSpecialityVM);

        Task<ResultContext<CandidateProviderTrainerProfileVM>> CreateCandidateProviderTrainerProfileAsync(ResultContext<CandidateProviderTrainerProfileVM> inputContext);
        #endregion

        #region Premises
        Task<CandidateProviderPremisesVM> GetCandidateProviderPremisesByIdAsync(CandidateProviderPremisesVM candidateProviderPremisesVM);

        Task<IEnumerable<CandidateProviderPremisesVM>> GetCandidateProviderPremisesByIdCandidateProviderAsync(CandidateProviderVM candidateProviderVM);
        
        Task<IEnumerable<CandidateProviderPremisesVM>> GetCandidateProviderPremisesByIdAsync(CandidateProviderVM candidateProvider);

        Task<CandidateProviderPremisesVM> GetCandidateProviderPremisesWithRoomsAndDocumentsByIdAsync(CandidateProviderPremisesVM candidateProviderPremisesVM);

        Task CreateCandidateProviderPremisesAsync(CandidateProviderPremisesVM model);

        Task UpdateCandidateProviderPremisesAsync(CandidateProviderPremisesVM model);

        Task<IEnumerable<CandidateProviderPremisesVM>> GetCandidateProviderPremisesWithStatusByIdCandidateProviderAsync(int idCandidateProvider);

        IEnumerable<CandidateProviderPremisesVM> GetCandidateProviderPremisesBySpeciality(CandidateProviderSpecialityVM candidateProviderSpeciality);

        Task<MemoryStream> GetExcelReportForCandidateProviderPremisies(string year);

        Task<IEnumerable<CandidateProviderPremisesVM>> GetAllActivePremisesByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderPremisesVM>> GetAllActiveCandidateProviderPremisesByCandidateProviderIdWithPremisesSpecialitiesIncludedAsync(int idCandidateProvider);
        Task<IEnumerable<CandidateProviderPremisesVM>> GetAllCandidateProviderPremisesByCandidateProviderIdWithPremisesSpecialitiesIncludedAsync(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderPremisesVM>> GetCandidateProviderPremisesWithAllIncludedByIdCandidateProviderAsync(int idCandidateProvider);
        #endregion

        #region Premises Room
        Task<CandidateProviderPremisesRoomVM> GetCandidateProviderPremisesRoomByIdAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM);

        Task<IEnumerable<CandidateProviderPremisesRoomVM>> GetAllCandidateProviderPremisesRoomsByCandidateProviderPremisesIdAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM);

        Task<ResultContext<CandidateProviderPremisesRoomVM>> UpdateCandidateProviderPremisesRoomAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM);

        Task<ResultContext<CandidateProviderPremisesRoomVM>> CreateCandidateProviderPremisesRoomAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM);

        Task<ResultContext<CandidateProviderPremisesRoomVM>> DeleteCandidateProviderPremisesRoomAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM);
        #endregion

        #region Premises Speciality
        Task<CandidateProviderPremisesSpecialityVM> GetCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderPremisesAsync(int idSpeciality, int idCandidateProviderPremises, int idUsage);
        List<CandidateProviderPremisesSpecialityVM> GetCandidateProviderPremisesSpecialitiesWithSpecialitiesByIdCandidateProviderPremises(int idCandidateProviderPremises);
        Task<CandidateProviderPremisesCheckingVM> GetCandidateProviderPremisesCheckingAsync(int IdCandidateProviderPremisesCheckings);
        Task<List<CandidateProviderPremisesCheckingVM>> GetAllActiveCandidateProviderPremisesCheckingAsync(int IdCandidateProviderPremises);
        Task<List<CandidateProviderPremisesCheckingVM>> GetAllActiveCandidateProviderPremisesCheckingByIdFollowUpControlAsync(int IdFollowUpControl);
        Task<ResultContext<CandidateProviderPremisesSpecialityVM>> DeleteCandidateProviderPremisesSpecialityAsync(CandidateProviderPremisesSpecialityVM candidateProviderPremisesSpecialityVM, int idUsage);

        Task<ResultContext<NoResult>> AddSpecialitiesToPremisesByListSpecialitiesAsync(List<CandidateProviderPremisesSpecialityVM> premisesSpecialities);

        Task<ResultContext<CandidateProviderPremisesCheckingVM>> DeleteCandidateProviderPremisesCheckingAsync(CandidateProviderPremisesCheckingVM candidateProviderPremisesCheckingVM);
        #endregion

        #region Premises Document
        Task<CandidateProviderPremisesDocumentVM> GetCandidateProviderPremisesDocumentByIdAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM);

        Task<ResultContext<CandidateProviderPremisesDocumentVM>> CreateCandidateProviderPremisesDocumentAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM);

        Task<ResultContext<CandidateProviderPremisesDocumentVM>> DeleteCandidateProviderPremisesDocumentAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM);

        Task<ResultContext<CandidateProviderPremisesDocumentVM>> UpdateCandidateProviderPremisesDocumentAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM);

        Task<IEnumerable<CandidateProviderPremisesDocumentVM>> GetAllCandidateProviderPremisesDocumentsByCandidateProviderPremisesIdAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM);
        Task<IEnumerable<CandidateProviderPremisesSpecialityVM>> GetCandidateProviderSpecialityByIdCandidateProviderPremisesAsync(int idCandidateProviderPremises);

        #endregion

        #region Specialities
        Task<IEnumerable<CandidateProviderSpecialityVM>> GetCandidateProviderAllPremisesSpecialititesByCandidateProviderId(CandidateProviderVM candidateProviderVm);
        #endregion

        #region Candidate Provider Person
        Task<List<CandidateProviderPersonVM>> GetAllCandidateProviderPersonAsync();

        Task UpdateCandidateProvider(CandidateProvider candidateProvider);
        Task UpdateCandidateProviderForAdditionalDocumentsRequestedAsync(CandidateProviderVM candidateProvider);

        Task<IEnumerable<CandidateProviderPersonVM>> GetAllActiveCandidateProviderPersonsByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderPersonVM>> GetAllCandidateProviderPersonsAllowedForNotificationsByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<CandidateProviderPersonVM>> GetAllCandidateProviderPersonsByIdCandidateProviderAsync(int idCandidateProvider);
        #endregion

        #region Candidate Provider CPO Structure and Activity
        Task<CandidateProviderCPOStructureActivityVM> GetCandidateProviderCPOStructureActivityByIdCandidateProviderAsync(int idCandidateProvider);

        Task<ResultContext<CandidateProviderCPOStructureActivityVM>> CreateCandidateProviderCPOStructureActivityAsync(ResultContext<CandidateProviderCPOStructureActivityVM> inputContext);

        Task<ResultContext<CandidateProviderCPOStructureActivityVM>> UpdateCandidateProviderCPOStructureActivityAsync(ResultContext<CandidateProviderCPOStructureActivityVM> inputContext);
        #endregion

        #region Candidate Provider CIPO Structure and Activity
        Task<CandidateProviderCIPOStructureActivityVM> GetCandidateProviderCIPOStructureActivityByIdCandidateProviderAsync(int idCandidateProvider);

        Task<ResultContext<CandidateProviderCIPOStructureActivityVM>> CreateCandidateProviderCIPOStructureActivityAsync(ResultContext<CandidateProviderCIPOStructureActivityVM> inputContext);

        Task<ResultContext<CandidateProviderCIPOStructureActivityVM>> UpdateCandidateProviderCIPOStructureActivityAsync(ResultContext<CandidateProviderCIPOStructureActivityVM> inputContext);
        Task<CandidateProviderLicenceChangeVM> GetCandidateProviderChangeLicenseByIdAsync(int idCandidateProvider, int IdCandidateProviderLicenceChange);
        Task<IEnumerable<CandidateProviderLicenceChangeVM>> GetCandidateProviderLicensesListByIdAsync(int idCandidateProvider);
        Task<ResultContext<CandidateProviderLicenceChangeVM>> CreateCandidateProviderLicenceChangeAsync(CandidateProviderLicenceChangeVM candidateProviderLicenceChangeVM);
        Task<ResultContext<CandidateProviderLicenceChangeVM>> UpdateCandidateProviderLicenceChangeAsync(CandidateProviderLicenceChangeVM candidateProviderLicenceChangeVM);
        #endregion

        #region Candidate Provider Consulting
        Task<IEnumerable<CandidateProviderConsultingVM>> GetAllCandidateProviderConsultingsByIdCandidateProviderAsync(int idCandidateProvider);
        Task<IEnumerable<ConsultingVM>> GetAllConsultingsByIdConsultingClientAsync(int idConsultingClient);

        Task<ResultContext<NoResult>> CreateCandidateProviderConsultingAsync(CandidateProviderConsultingVM model);
        Task<ResultContext<NoResult>> CreateConsultingAsync(ConsultingVM model);

        Task<ResultContext<NoResult>> DeleteCandidateProviderConsultingByIdAsync(int idCandidateProviderConsulting);
        Task<ResultContext<NoResult>> DeleteConsultingByIdAsync(int idConsulting);
        Task<MemoryStream> PrintValidationCurriculumAsync(FrameworkProgramVM frameworkProgramVM, SpecialityVM speciality, double totalHours, double compulsoryHours, double nonCompulsoryHours, string providerOwner, bool isCurriculumInvalid, CandidateProviderSpecialityVM candidateProviderSpecialityVM, CandidateProviderVM candidateProvider, List<CandidateCurriculumVM> curriculums = null, List<ValidationCurriculumVM> validationCurriculums = null, bool showInvalidCurriculumText = false);
        #endregion

        #region Curriculum modification
        Task<CandidateCurriculumModificationVM> GetCurriculumModificationByIdCandidateProviderSpecialityAndByIdModificationStatusAsync(int idCandidateProviderSpeciality, int idModificationStatus);

        Task<ResultContext<NoResult>> CreateCurriculumModificationEntryAsync(int idCandidateProviderSpeciality, int idModificationReason, DateTime validFromDate, bool isDOSChange = false);

        Task<IEnumerable<CandidateCurriculumVM>> GetCandidateCurriculumsByIdCandidateCurriculumModificationAsync(int idCandidateCurriculumModification);

        Task<CandidateCurriculumModificationVM> GetCandidateCurriculumModificationWhenApplicationByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality);

        Task<ResultContext<NoResult>> CancelCandidateCurriculumModificationAsync(int idCandidateCurriculumModification);

        Task<ResultContext<NoResult>> FinishCandidateCurriculumModificationAsync(int idCandidateCurriculumModification);

        Task<IEnumerable<CandidateCurriculumModificationVM>> GetAllCurriculumModificationsByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality);

        Task<IEnumerable<CandidateCurriculumVM>> GetActualCandidateCurriculumByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality);

        Task<CandidateCurriculumModificationVM> GetActualCandidateCurriculumModificationByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality);

        Task<bool> IsCurriculumUploadFileAllowedAsync(int idSpeciality, int idCandidateProvider);

        Task<CandidateCurriculumModificationVM> IsNewestValidFromDateByIdCandidateProviderSpecialityAndNewValidFromDateAsync(int idCandidateProviderSpeciality, DateTime validFromDate);

        Task<IEnumerable<CandidateCurriculumVM>> GetActualCandidateCurriculumWithERUIncludedByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality);
        #endregion

        #region Registers
        Task<IEnumerable<RegisterMTBVM>> GetAllMTBsForActiveCandidateProvidersAsync(PremisesFilterVM filter);

        Task<RegisterMTBVM> GetRegisterMTBVMByIdCandidateProviderPremisesAsync(int idCandidateProviderPremises);
        Task<CandidateProviderVM> GetOnlyCandidateProviderByIdAsync(int IdCandidateProvider);

        /// <summary>
        /// Get all candidate providers for AutoComplete
        /// </summary>
        /// <param name="licensingType">LicensingCPO | LicensingCIPO</param>
        /// <param name="showAllTypes">true | false</param>
        /// <returns></returns>
        Task<IEnumerable<CandidateProviderVM>> GetAllCandidateProvidersForAutoComplete(string licensingType, bool showAllTypes = false);

        Task<IEnumerable<RegisterTrainerVM>> GetCandidateProviderTrainersByFilterModelAsync(RegisterTrainerVM model);
        #endregion
    }
}
