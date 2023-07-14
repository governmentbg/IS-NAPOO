using System.Collections.Generic;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderExcelVM
    {
        public CandidateProviderExcelVM()
        {
            this.ProviderSpecialities = new List<ProviderSpecialityExcelVM>();
            this.AddedTrainers = new List<TrainersExcelVM>();
            this.AddedMTBs = new List<MTBExcelVM>();
            this.MissingTrainerSpecialities = new List<string>();
            this.MissingMTBSpecialities = new List<string>();
            this.InactiveSpecialities = new List<string>();
        }

        #region Training institution
        public bool ProviderRegistrationHasValue { get; set; }

        public bool ProviderOwnershipHasValue { get; set; }

        public bool ProviderEmailHasValue { get; set; }

        public bool ProviderNameHasValue { get; set; }

        public bool ProviderStatusHasValue { get; set; }

        public bool LocationCorrespondenceHasValue { get; set; }

        public bool ZipCodeCorrespondenceHasValue { get; set; }

        public bool PersonNameCorrespondenceHasValue { get; set; }

        public bool PersonsForNotificationsHasValue { get; set; }

        public bool ProviderPhoneCorrespondenceHasValue { get; set; }

        public bool ProviderEmailCorrespondenceHasValue { get; set; }
        #endregion

        #region Specialities
        public bool CandidateProviderSpecialitiesHasValue { get; set; }

        public List<string> InactiveSpecialities { get; set; }

        public List<ProviderSpecialityExcelVM> ProviderSpecialities { get; set; }
        #endregion

        #region Structure and activities
        public bool OrganisationTrainingProcessHasValue { get; set; }

        public bool CompletionCertificationTrainingHasValue { get; set; }

        public bool InternalQualitySystemHasValue { get; set; }

        public bool InformationProvisionMaintenanceHasValue { get; set; }

        public bool TrainingDocumentationHasValue { get; set; }

        public bool TeachersSelectionHasValue { get; set; }

        public bool MTBDescriptionHasValue { get; set; }

        public bool DataMaintenanceHasValue { get; set; }

        public bool ManagementHasValue { get; set; }

        public bool OrganisationInformationProcessHasValue { get; set; }

        public bool ConsultantsSelectionHasValue { get; set; }
        #endregion

        #region Trainers
        public bool AddedTrainersHasValue { get; set; }

        public List<TrainersExcelVM> AddedTrainers { get; set; }

        //public bool AllSpecialitiesInTrainersAreCovered { get; set; }

        public List<string> MissingTrainerSpecialities { get; set; }
        #endregion

        #region MTBs
        public bool AddedMTBsHasValue { get; set; }

        public List<MTBExcelVM> AddedMTBs { get; set; }

        //public bool AllSpecialitiesInMTBAreCovered { get; set; }

        public List<string> MissingMTBSpecialities { get; set; }

        public bool IsDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfessionAdded { get; set; }
        #endregion

        #region Form application
        public bool IsFormApplicationValid { get; set; }

        public string FormApplicationMissingFields { get; set; }
        #endregion

        #region Documents
        public bool IsRegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenterAdded { get; set; }

        public bool IsFeePaidDocumentAdded { get; set; }

        public bool IsProcedureDocumentAdded { get; set; }
        #endregion

        #region Application status
        public bool ReceiveLicenseHasValue { get; set; }

        public bool ApplicationFilingHasValue { get; set; }

        public bool ESignedApplicationFileIsMissing { get; set; }
        #endregion

        public string ProviderType { get; set; }

        public bool IsApplicationValid()
        {
            var providerSpecialityWithoutFrameworkProgram = this.ProviderSpecialities.Any(x => !x.FrameworkProgramHasValue);
            var providerSpecialityWithoutEducationFrom = this.ProviderSpecialities.Any(x => !x.EducationFormHasValue);
            var providerSpecialityCurriculumValid = this.ProviderSpecialities.Any(x => !x.IsCurriculumValid);
            var trainerWithoutSpeciality = this.AddedTrainers.Any(x => !x.IsSpecialityAdded);
            var trainerWithoutCertificate = this.AddedTrainers.Any(x => !x.IsCertificateAdded);
            var trainerWithoutAutobiography = this.AddedTrainers.Any(x => !x.IsAutobiographyAdded);
            var trainerWithoutDeclaration = this.AddedTrainers.Any(x => !x.IsDeclarationOfConsentAdded);
            var trainerWithoutDiploma = this.AddedTrainers.Any(x => !x.IsRetrainingDiplomaAdded);
            var mtbWithoutSpeciality = this.AddedMTBs.Any(x => !x.IsSpecialityAdded);
            var mtbWithoutDocumentForComplianceWithFireSafetyRulesAndRegulations = this.AddedMTBs.Any(x => !x.IsDocumentForComplianceWithFireSafetyRulesAndRegulationsAdded);
            var mtbWithoutDocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements = this.AddedMTBs.Any(x => !x.IsDocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsAdded);
            var mtbWithoutDocumentsForThePresenceOfMTBInAccordanceWithTheDOS = this.AddedMTBs.Any(x => !x.IsDocumentsForThePresenceOfMTBInAccordanceWithTheDOSAdded);
            var mtbWithoutDocumentsForThePresenceOfMTBCIPO = this.AddedMTBs.Any(x => !x.IsDocumentsForThePresenceOfMTBCIPOAdded);
            var mtbWithoutDocumentsForThePresenceOfMTBInAccordanceCIPOAdded = this.AddedMTBs.Any(x => !x.IsDocumentsForThePresenceOfMTBInAccordanceCIPOAdded);

            if (this.ProviderType == "CPO")
            {
                return this.ProviderRegistrationHasValue && this.ProviderOwnershipHasValue && this.ProviderEmailHasValue && this.IsDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfessionAdded
                && this.ProviderEmailHasValue && this.ProviderNameHasValue && this.ProviderStatusHasValue
                && this.LocationCorrespondenceHasValue && this.ZipCodeCorrespondenceHasValue && this.PersonNameCorrespondenceHasValue
                && this.PersonsForNotificationsHasValue && this.ProviderPhoneCorrespondenceHasValue && this.ProviderEmailCorrespondenceHasValue
                && this.CandidateProviderSpecialitiesHasValue && !this.InactiveSpecialities.Any() && !providerSpecialityWithoutFrameworkProgram
                && !providerSpecialityWithoutEducationFrom && !providerSpecialityCurriculumValid && this.OrganisationTrainingProcessHasValue && this.ManagementHasValue
                && this.CompletionCertificationTrainingHasValue && this.InternalQualitySystemHasValue && this.InformationProvisionMaintenanceHasValue
                && this.TrainingDocumentationHasValue && this.TeachersSelectionHasValue && this.MTBDescriptionHasValue && this.DataMaintenanceHasValue
                && this.AddedTrainersHasValue && !this.MissingTrainerSpecialities.Any() && !trainerWithoutSpeciality && !trainerWithoutCertificate
                && !trainerWithoutAutobiography && !trainerWithoutDeclaration && !trainerWithoutDiploma && this.AddedMTBsHasValue && this.IsFormApplicationValid
                && !this.MissingMTBSpecialities.Any() && !mtbWithoutSpeciality && !mtbWithoutDocumentForComplianceWithFireSafetyRulesAndRegulations
                && !mtbWithoutDocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements && !mtbWithoutDocumentsForThePresenceOfMTBInAccordanceWithTheDOS
                && this.IsRegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenterAdded && this.IsFeePaidDocumentAdded && this.ReceiveLicenseHasValue && this.ApplicationFilingHasValue;
            }
            else
            {
                return this.ProviderRegistrationHasValue && this.ProviderOwnershipHasValue && this.ProviderEmailHasValue
                && this.ProviderEmailHasValue && this.ProviderNameHasValue && this.ProviderStatusHasValue
                && this.LocationCorrespondenceHasValue && this.ZipCodeCorrespondenceHasValue && this.PersonNameCorrespondenceHasValue
                && this.PersonsForNotificationsHasValue && this.ProviderPhoneCorrespondenceHasValue && this.ProviderEmailCorrespondenceHasValue
                && this.ManagementHasValue && this.InternalQualitySystemHasValue && this.InformationProvisionMaintenanceHasValue && this.IsFormApplicationValid
                && this.TrainingDocumentationHasValue && this.ConsultantsSelectionHasValue && this.MTBDescriptionHasValue && this.DataMaintenanceHasValue
                && this.AddedTrainersHasValue && !trainerWithoutCertificate
                && !trainerWithoutAutobiography && !trainerWithoutDeclaration && !trainerWithoutDiploma
                && !mtbWithoutDocumentForComplianceWithFireSafetyRulesAndRegulations && this.AddedMTBsHasValue
                && !mtbWithoutDocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements
                && this.IsProcedureDocumentAdded && this.ReceiveLicenseHasValue && this.ApplicationFilingHasValue && !mtbWithoutDocumentsForThePresenceOfMTBCIPO && !mtbWithoutDocumentsForThePresenceOfMTBInAccordanceCIPOAdded;
            }
        }
    }

    public class ProviderSpecialityExcelVM
    {
        public string Speciality { get; set; }

        public bool FrameworkProgramHasValue { get; set; }

        public bool EducationFormHasValue { get; set; }

        public bool IsCurriculumValid { get; set; }

        public CandidateCurriculumExcelVM CurriculumExcelVM { get; set; }
    }

    public class TrainersExcelVM
    {
        public string FullName { get; set; }

        public bool IsSpecialityAdded { get; set; }

        public bool IsCertificateAdded { get; set; }

        public bool IsAutobiographyAdded { get; set; }

        public bool IsDeclarationOfConsentAdded { get; set; }

        public bool IsRetrainingDiplomaAdded { get; set; }
    }

    public class MTBExcelVM
    {
        public string Name { get; set; }

        public bool IsSpecialityAdded { get; set; }

        public bool IsDocumentForComplianceWithFireSafetyRulesAndRegulationsAdded { get; set; }

        public bool IsDocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsAdded { get; set; }

        public bool IsDocumentsForThePresenceOfMTBInAccordanceWithTheDOSAdded { get; set; }

        public bool IsDocumentsForThePresenceOfMTBCIPOAdded { get; set; }

        public bool IsDocumentsForThePresenceOfMTBInAccordanceCIPOAdded { get; set; }
    }
}
