using Microsoft.AspNetCore.Components;
using NapooMigration.Data;
using System;

namespace NapooMigration.Pages
{
    public partial class SPPOO
    {

        [Inject]
        public ImportService ImportService { get; set; }

        string inputValue = string.Empty;

        public void test()
        {
            this.ImportService.testGetDoc();
        }
   public void Spravka()
        {
            this.ImportService.Spravka();
        }
        public void importAll()
        {
            this.ImportService.ImportAllCandidateProvider(inputValue);
        }
        
        public void importAllNoDocuments()
        {
            this.ImportService.ImportAllCandidateProviderWithoutDocuents(inputValue);
        }
        public void btnImportSPPOO()
        {
            this.ImportService.ImportSPPOO();
        }
        public void btnImportNationality()
        {
            this.ImportService.ImportNationality();
        }
        public void btnImportCandidateProviderDocumentType()
        {
            this.ImportService.ImportCandidateProviderDocumentType();
        }
        public void btnImportDocuentsOnly()
        {
            this.ImportService.ImportDocuentsOnly();
        }
        
        public void btnImportExpertExpert()
        {
            this.ImportService.ImportExpertExpert();
        }
        
        public void btnUpdateCandidateProviderDocumentType()
        {
            this.ImportService.UpdateCandidateProviderDocumentType();
        }         
        public void btnUpdateTrainingClientCourseDocumentStatus()
        {
            this.ImportService.UpdateTrainingClientCourseDocumentStatus();
        } 
        
        public void btnImportSPPOOrder()
        {
            this.ImportService.ImportSPPOOrder();
        }
        public void btnUpdateOidValidationDocument()
        {
            this.ImportService.UpdateOidValidationDocument();
        }      
        public void btnUpdateOidClientCourseDocument()
        {
            this.ImportService.UpdateOidClientCourseDocument();
        }        
        public void btnImportExpertCommission()
        {
            this.ImportService.ImportExpertCommission();
        }
        
        public void btnImportNapooExpert()
        {
            this.ImportService.ImportNapooExpert();
        }
        public void btnImportProviders()
        {
            if(string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportProviders();
            else
                this.ImportService.ImportProviders(Int32.Parse(inputValue));
        }
        public void importDocumentsOnly()
        {
                this.ImportService.ImportAllCandidateProviderDocuentsOnly(inputValue);
        }
        public void btnImportCourseCandidateCurriculumModification()
        {
            if(string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportCourseCandidateCurriculumModification();
            else
                this.ImportService.ImportCourseCandidateCurriculumModification(Int32.Parse(inputValue));
        } 
        public void btnUpdateRequestProviderDocumentStatus()
        {
            if(string.IsNullOrEmpty(inputValue))
            this.ImportService.UpdateRequestProviderDocumentStatus();
            else
                this.ImportService.UpdateRequestProviderDocumentStatus(Int32.Parse(inputValue));
        }
        public void btnImportProviderProcedureDocuments()
        {
            if(string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportProviderProcedureDocuments();
            else
                this.ImportService.ImportProviderProcedureDocuments(Int32.Parse(inputValue));
        }
        public void btnUpdateSelfAssessmentStatus()
        {
            if(string.IsNullOrEmpty(inputValue))
            this.ImportService.UpdateSelfAssessmentStatus();
            else
                this.ImportService.UpdateSelfAssessmentStatus(Int32.Parse(inputValue));
        }
        public void btnRequestCandidateProviderDocumentMigrateDocuments()
        {
            if(string.IsNullOrEmpty(inputValue))
            this.ImportService.RequestCandidateProviderDocumentMigrateDocuments();
            else
                this.ImportService.RequestCandidateProviderDocumentMigrateDocuments(Int32.Parse(inputValue));
        }
        public void btnImportTrainingCourseSubject()
        {
            if(string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportTrainingCourseSubject();
            else
                this.ImportService.ImportTrainingCourseSubject(Int32.Parse(inputValue));
        }
        public void btnImportProviderLicenceChange()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportProviderLicenceChange();
            else
                this.ImportService.ImportProviderLicenceChange(Int32.Parse(inputValue));
        }      
        public void btnImportValidationCompetency()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportValidationCompetencies();
            else
                this.ImportService.ImportValidationCompetencies(Int32.Parse(inputValue));
        }
        public void btnImportPremisies()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportProviderPremisies();
            else
                this.ImportService.ImportProviderPremisies(Int32.Parse(inputValue));
        }
        public void btnImportArchTrainer()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportArchTrainer();
            else
                this.ImportService.ImportArchTrainer(Int32.Parse(inputValue));
        }
        public void btnImportCandidateCurriculumModification()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCandidateCurriculumModification();
            else
                this.ImportService.ImportCandidateCurriculumModification(Int32.Parse(inputValue));
        }
        public void btnImportCandidateCurriculumModificationId()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCandidateCurriculumModification();
            else
                this.ImportService.ImportCandidateCurriculumModification(Int32.Parse(inputValue));  
        }
        public void btnImportCurriculumModification()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCurriculumModification();
            else
                this.ImportService.ImportCurriculumModification(Int32.Parse(inputValue));
        }
        public void btnImportCandidatePremisies()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCandidatePremisies();
            else
                this.ImportService.ImportCandidatePremisies(Int32.Parse(inputValue));
        }
        public void btnImportProviderDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportProviderDocuments();
            else
                this.ImportService.ImportProviderDocuments(Int32.Parse(inputValue));
        }
        public void btnImportCandidateProviderDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCandidateProviderDocuments();
            else
                this.ImportService.ImportCandidateProviderDocuments(Int32.Parse(inputValue));
        }
        public void btnImportProcedures()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportProcedure();
            else
                this.ImportService.ImportProcedure(Int32.Parse(inputValue));
        }
        public void btnImportPremisiesRooms()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportPremisiesRooms();
            else
                this.ImportService.ImportPremisiesRooms(Int32.Parse(inputValue));
        }
        public void btnImportArchProviderTrainerQualifications()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportArchProviderTrainerQualifications();
            else
                this.ImportService.ImportArchProviderTrainerQualifications(Int32.Parse(inputValue));
        }
        public void btnImportArchTrainingCourse()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportArchTrainingCourse();
            else
                this.ImportService.ImportArchTrainingCourse(Int32.Parse(inputValue));
        }
        public void btnImportArchTrainingValidation()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportArchTrainingValidation();
            else
                this.ImportService.ImportArchTrainingValidation(Int32.Parse(inputValue));
        }
        public void btnImportCandidatePremisiesRooms()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCandidatePremisiesRooms();
            else
                this.ImportService.ImportCandidatePremisiesRooms(Int32.Parse(inputValue));
        }
        public void btnImportProvidersTrainers()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportProviderTrainer();
            else
                this.ImportService.ImportProviderTrainer(Int32.Parse(inputValue));
        }
        public void btnImportCandidateProvidersTrainers()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCandidateProviderTrainer();
            else
                this.ImportService.ImportCandidateProviderTrainer(Int32.Parse(inputValue));
        }
        public void btnImportCandidateProvidersTrainerQualifications()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCandidateProviderTrainerQualifications();
            else
                this.ImportService.ImportCandidateProviderTrainerQualifications(Int32.Parse(inputValue));
        }
        public void btnImportProvidersTrainerQualifications()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportProviderTrainerQualifications();
            else
                this.ImportService.ImportProviderTrainerQualifications(Int32.Parse(inputValue));
        }
        public void btnImportProvidersTrainerProfiles()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportProviderTrainerProfiles();
            else
                this.ImportService.ImportProviderTrainerProfiles(Int32.Parse(inputValue));
        }
        public void btnImportCandidateProvidersTrainerProfiles()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCandidateProviderTrainerProfiles();
            else
                this.ImportService.ImportCandidateProviderTrainerProfiles(Int32.Parse(inputValue));
        }
        public void btnImportCandidateProviderTrainerDocument()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportCandidateProviderTrainerDocument();
            else
                this.ImportService.ImportCandidateProviderTrainerDocument(Int32.Parse(inputValue));
        }
        public void btnImportProviderTrainerDocument()
        {
                this.ImportService.ImportProviderTrainerDocument();
        }
        public void btnImportExperts()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportExperts();

        }
        public void ImportCPOs()
        {
                this.ImportService.ImportCPOs();
        }
        public void btnImportDoc()
        {
                this.ImportService.ImportDOC();
        }
        public void btnImportRequestTypeOfRequestedDocuments()
        {
                this.ImportService.ImportRequestTypeOfRequestedDocuments();

        } 
        public void btnImportRequestDocumentSeries()
        {
                this.ImportService.ImportRequestDocumentSeries();
        }
        public void btnImportSppooFrameworkPrograms()
        {
                this.ImportService.ImportSppooFrameworkProgram();
        }
        public void btnImportTrainingProgram()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportTrainingProgram();
            else
                this.ImportService.ImportTrainingProgram(Int32.Parse(inputValue));
        }
        public void btnImportSelfAssessmentReport()
        {
                if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportSelfAssessmentReport();
            else
                this.ImportService.ImportSelfAssessmentReport(Int32.Parse(inputValue));
        }

        public void btnImportTrainingTrainerCourse()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportTrainerCourse();
            else
                this.ImportService.ImportTrainerCourse(Int32.Parse(inputValue));
        }
        public void btnImportTrainingPremisesCourse()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportTrainingPremisesCourses();
            else
                this.ImportService.ImportTrainingPremisesCourses(Int32.Parse(inputValue));
        }
        
        public void btnCandidateCurriculumModificationMigrateDomuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.CandidateCurriculumModificationMigrateDomuments();
            else
                this.ImportService.CandidateCurriculumModificationMigrateDomuments(Int32.Parse(inputValue));
        } 
        
        public void btnCandidateProviderDocumentMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.CandidateProviderDocumentMigrateDocuments();
            else
                this.ImportService.CandidateProviderDocumentMigrateDocuments(Int32.Parse(inputValue));
        }        
        public void btnCandidateProviderTrainerDocumentMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.CandidateProviderTrainerDocumentMigrateDocuments();
            else
                this.ImportService.CandidateProviderTrainerDocumentMigrateDocuments(Int32.Parse(inputValue));
        } 
        public void btnCandidateProviderProcedureDocumentMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.CandidateProviderProcedureDocumentMigrateDocuments();
            else
                this.ImportService.CandidateProviderProcedureDocumentMigrateDocuments(Int32.Parse(inputValue));
        }
        public void btnRequestDocumentManagementMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.RequestDocumentManagementMigrateDocuments();
            else
                this.ImportService.RequestDocumentManagementMigrateDocuments(Int32.Parse(inputValue));
        }
        
        public void btnRequestReportMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.RequestReportMigrateDocuments();
            else
                this.ImportService.RequestReportMigrateDocuments(Int32.Parse(inputValue));
        }            
        public void btnValidationClientDocumentsMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ValidationClientDocumentsMigrateDocuments();
            else
                this.ImportService.ValidationClientDocumentsMigrateDocuments(Int32.Parse(inputValue));
        }         
        public void btnClientCourseDocumentsMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ClientCourseDocumentsMigrateDocuments();
            else
                this.ImportService.ClientCourseDocumentsMigrateDocuments(Int32.Parse(inputValue));
        }           
        public void btnClientCourseRequiredDocumentsMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ClientCourseRequiredDocumentsMigrateDocuments();
            else
                this.ImportService.ClientCourseRequiredDocumentsMigrateDocuments(Int32.Parse(inputValue));
        }           
        public void btnValidationRequiredDocumentsMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ValidationRequiredDocumentsMigrateDocuments();
            else
                this.ImportService.ValidationRequiredDocumentsMigrateDocuments(Int32.Parse(inputValue));
        }         
        public void btnValidationProtocolMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ValidationProtocolMigrateDocuments();
            else
                this.ImportService.ValidationProtocolMigrateDocuments(Int32.Parse(inputValue));
        } 
        public void btnAnnualInfoMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.AnnualInfoMigrateDocuments();
            else
                this.ImportService.AnnualInfoMigrateDocuments(Int32.Parse(inputValue));
        }
        public void btnSelfAssessmentMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.SelfAssessmentMigrateDocuments();
            else
                this.ImportService.SelfAssessmentMigrateDocuments(Int32.Parse(inputValue));
        } 
        public void btnProviderRequestDocumentMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ProviderRequestDocumentMigrateDocuments();
            else
                this.ImportService.ProviderRequestDocumentMigrateDocuments(Int32.Parse(inputValue));
        } 
        public void btnCourseProtocolMigrateDocuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.CourseProtocolMigrateDocuments();
            else
                this.ImportService.CourseProtocolMigrateDocuments(Int32.Parse(inputValue));
        }
        public void btnImportTrainingCourse()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportTrainingCourse();
            else
                this.ImportService.ImportTrainingCourse(Int32.Parse(inputValue));
        }
        public void btnImportTrainingClient()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportTrainingClients();
            else
                this.ImportService.ImportTrainingClients(Int32.Parse(inputValue));
        }
        public void btnImportTrainingClientCourses()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportTrainingClientCourses();
            else
                this.ImportService.ImportTrainingClientCourses(Int32.Parse(inputValue));
        }
        public void btnImportTrainingCourse40()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.importTrainingCourse40();
        }
        public void btnImportRequestDocumentType()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportRequestRequestDocumentType();
            else
                this.ImportService.ImportRequestRequestDocumentType(Int32.Parse(inputValue));
        } 
        public void btnImportRequestProviderRequestDocument()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportRequestProviderRequestDocument();
            else
                this.ImportService.ImportRequestProviderRequestDocument(Int32.Parse(inputValue));
        }
        public void btnImportRequestDocumentManagement()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportRequestDocumentManagement();
            else
                this.ImportService.ImportRequestDocumentManagement(Int32.Parse(inputValue));
        }
        public void btnImportRequestDocumentStatus()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportRequestRequestDocumentStatus();
            else
                this.ImportService.ImportRequestRequestDocumentStatus(Int32.Parse(inputValue));
        }
        public void btnImportRequestDocumentSerialNumber()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportRequestDocumentSerialNumber();
            else
                this.ImportService.ImportRequestDocumentSerialNumber(Int32.Parse(inputValue));
        }
        
        public void btnImportRequestProviderDocumentOffer()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportRequestProviderDocumentOffer();
            else
                this.ImportService.ImportRequestProviderDocumentOffer(Int32.Parse(inputValue));
        }
        public void btnImportTrainingClientCourseDocument()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.ImportTrainingClientCourseDocument();
            else
                this.ImportService.ImportTrainingClientCourseDocument(Int32.Parse(inputValue));
        }
        public void btnImportTrainingCurriculumTheory()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportTrainingCurriculumTheory();
            else
                this.ImportService.ImportTrainingCurriculumTheory(Int32.Parse(inputValue));
        }
        public void btnImportTrainingClientCourseDocumentStatus()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportTrainingClientCourseDocumentStatus();
            else
                this.ImportService.ImportTrainingClientCourseDocumentStatus(Int32.Parse(inputValue));
        }
        public void btnImportTrainingCurriculumTheoryPractice()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportTrainingCurriculumPractice();
            else
                this.ImportService.ImportTrainingCurriculumPractice(Int32.Parse(inputValue));
        }
        public void btnImportRequestNapooRequestDoc()
        {
            this.ImportService.ImportRequestNapooRequestDoc();
        }
        public void btnImportCandidateProviderInactive()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportCandidateProviders();
            else
                this.ImportService.ImportCandidateProviders(Int32.Parse(inputValue));
        }
        public void btnImportCourseCommisionMembers()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportCourseCommisionMembers();
            else
                this.ImportService.ImportCourseCommisionMembers(Int32.Parse(inputValue));
        } 
        
        public void btnImportClientRequiredDocument()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportClientRequiredDocument();
            else
                this.ImportService.ImportClientRequiredDocument(Int32.Parse(inputValue));
        }
        public void btnImportPremisesSpecialities()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportPremisesSpecialities();
            else
                this.ImportService.ImportPremisesSpecialities(Int32.Parse(inputValue));
        } 
        public void btnImportCandidatePremisesSpecialities()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportCandidatePremisesSpecialities();
            else
                this.ImportService.ImportCandidatePremisesSpecialities(Int32.Parse(inputValue));
        }
        public void btnImportRequestReport()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportRequestReport();
            else
                this.ImportService.ImportRequestReport(Int32.Parse(inputValue));
        }
        
        public void btnImportArchProvider()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportArchProvider();
            else
                this.ImportService.ImportArchProvider(Int32.Parse(inputValue));
        }
        public void btnImportArchPremises()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportArchPremises();
            else
                this.ImportService.ImportArchPremises(Int32.Parse(inputValue));
        } 
        
        public void btnImportArchPremisesSpecialities()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportArchPremisesSpecialities();
            else
                this.ImportService.ImportArchPremisesSpecialities(Int32.Parse(inputValue));
        }

        public void btnImportAnnualInfo()
        {
            if (string.IsNullOrEmpty(inputValue))
            this.ImportService.ImportAnnualInfo();
            else
                this.ImportService.ImportAnnualInfo(Int32.Parse(inputValue));
        }

        public void btnImportCandidateCurriculumModificationMigrateDomuments()
        {
            if (string.IsNullOrEmpty(inputValue))
                this.ImportService.CandidateCurriculumModificationMigrateDomuments();
            else
                this.ImportService.CandidateCurriculumModificationMigrateDomuments(Int32.Parse(inputValue));
        }

    }
}
