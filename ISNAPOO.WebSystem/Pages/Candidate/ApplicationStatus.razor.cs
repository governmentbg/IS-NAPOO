using System.Text;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Licensing;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.BISS;
using ISNAPOO.WebSystem.Pages.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using RegiX;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ApplicationStatus : BlazorBaseComponent
    {
        private IEnumerable<KeyValueVM> kvReceiveLicenseSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationFilingSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvVQSSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvLicensingSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvProcedureDocumentTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvLicensing = new KeyValueVM();
        private ProcedureDocumentVM procedureDocumentVM = new ProcedureDocumentVM();
        private string licenseType = string.Empty;
        private bool isBtnDisabled = false;
        private List<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private IEnumerable<KeyValueVM> professionalTrainingsSource = new List<KeyValueVM>();
        private bool isApplicationValid = false;
        private FormApplicationModal form = new FormApplicationModal();
        private PaymentFeeListModal paymentFeeListModal = new PaymentFeeListModal();
        private int idKvESign = 0;

        public bool showCertificateData { get; set; } = false;

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsCPO { get; set; }

        [Parameter]
        public bool IsUserProfileAdministrator { get; set; }

        [Parameter]
        public bool DisableFieldsWhenApplicationIsNotDocPreparation { get; set; }

        [Parameter]
        public EventCallback CallbackAfterStartedProcedure { get; set; }

        [Inject]
        public ILicensingProcedureDocumentCPOService LicensingCPOService { get; set; }

        [Inject]
        public ILicensingProcedureDocumentCIPOService LicensingCIPOService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProviderService ProviderService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IDOCService DOCService { get; set; }

        [Inject]
        public IFrameworkProgramService FrameworkProgramService { get; set; }

        [Inject]
        public IRegiXService RegiXService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public ITemplateDocumentService TemplateDocumentService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(new Model());
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.kvVQSSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                this.kvLicensingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType");
                this.kvApplicationStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
                this.kvProcedureDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
                this.kvLicensing = this.IsCPO
                    ? this.kvProcedureDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "RequestLicensingCPO")
                    : this.kvProcedureDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "RequestLicensingCIPO");

                if (this.IsCPO)
                {
                    this.specialitiesSource = this.DataSourceService.GetAllSpecialitiesList();
                }

                this.kvApplicationFilingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationFilingType");
                this.kvReceiveLicenseSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ReceiveLicenseType");
                this.idKvESign = this.kvApplicationFilingSource.FirstOrDefault(x => x.KeyValueIntCode == "ThroughESignature").IdKeyValue;

                if (this.CandidateProviderVM.IdStartedProcedure is not null)
                {
                    await this.SetProcedureDocumentDataAsync();
                }

                if (this.CandidateProviderVM.IdTypeLicense != 0)
                {
                    this.licenseType = this.kvLicensingSource.FirstOrDefault(x => x.IdKeyValue == this.CandidateProviderVM.IdTypeLicense).Name;
                }

                this.ShowHideButtons();

                this.StateHasChanged();
            }
        }

        private async Task SetProcedureDocumentDataAsync()
        {
            this.procedureDocumentVM = await this.ProviderService.GetProcedureDocumentByIdStartedProcedureAndIdDocumentTypeAsync(this.CandidateProviderVM.IdStartedProcedure.Value, this.kvLicensing.IdKeyValue);
            if (this.procedureDocumentVM is not null)
            {
                this.CandidateProviderVM.ApplicationDate = this.procedureDocumentVM.DS_OFFICIAL_DATE;
                this.CandidateProviderVM.ApplicationNumber = this.procedureDocumentVM.DS_OFFICIAL_DocNumber;
            }
        }

        private async Task PrintApplication()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!this.CandidateProviderVM.IdApplicationFiling.HasValue && !this.CandidateProviderVM.IdReceiveLicense.HasValue)
                {
                    await this.ShowErrorAsync("Моля, изберете изберете начин на получаване на административен акт и лицензия и начин на подаване на заявление и документ за платена държавна такса!");
                }
                else if (!this.CandidateProviderVM.IdApplicationFiling.HasValue)
                {
                    await this.ShowErrorAsync("Моля, изберете начин на подаване на заявление и документ за платена държавна такса!");
                }
                else if (!this.CandidateProviderVM.IdReceiveLicense.HasValue)
                {
                    await this.ShowErrorAsync("Моля, изберете начин на получаване на административен акт и лицензия!");
                }
                else
                {
                    if (!this.CandidateProviderVM.UIN.HasValue)
                    {
                        var result = await this.CandidateProviderService.SetCandidateProviderUINValueAsync(this.CandidateProviderVM);

                        if (!string.IsNullOrEmpty(result))
                        {
                            await this.ShowErrorAsync(result);
                        }
                    }

                    var kvActiveStatusTemplate = await this.DataSourceService.GetKeyValueByIntCodeAsync("StatusTemplate", "Active");
                    var kvApplicationType = this.IsCPO
                        ? await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "RequestLicensingCPO")
                        : await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "RequestLicensingCIPO");
                    var filterTemplateVM = new TemplateDocumentVM()
                    {
                        IdStatus = kvActiveStatusTemplate.IdKeyValue,
                        IdApplicationType = kvApplicationType.IdKeyValue,
                    };

                    var listTemplates = await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(filterTemplateVM);
                    var template = listTemplates.FirstOrDefault();

                    var file = this.IsCPO
                        ? await this.LicensingCPOService.GenerateLicensingApplication(this.CandidateProviderVM, this.kvApplicationFilingSource, this.kvReceiveLicenseSource, this.kvVQSSource, template)
                        : await this.LicensingCIPOService.GenerateLicensingApplication(this.CandidateProviderVM, this.kvApplicationFilingSource, this.kvReceiveLicenseSource, this.kvVQSSource);

                    var saveFileName = this.IsCPO
                        ? "Zaqvlenie-Licenzirane-CPO.pdf"
                        : "Zaqvlenie-Licenzirane-CIPO.pdf";
                    await FileUtils.SaveAs(this.JsRuntime, saveFileName, file.ToArray());
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SignApplication()
        {
            SignHelper helper = new SignHelper();

            string stringToBeSigned = "ТОВА Е ЗА ПОДПИС";

            var stringToBeSignedBytes = Encoding.UTF8.GetBytes(stringToBeSigned);

            Sign signData = helper.GetSignedValues(stringToBeSigned);


            var signatures = await this.JsRuntime.InvokeAsync<string>("sign", Convert.ToBase64String(stringToBeSignedBytes), signData.signedContents, signData.certificateBase64String, (object)null);


            var Use_E_Signature = await this.DataSourceService.GetSettingByIntCodeAsync("Use_E_Signature");

            if (bool.Parse(Use_E_Signature.SettingValue))
            {

            }


        }

        private async Task StartProcedure()
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да подадете заявлението към деловодната система на НАПОО? След подаване на заявлението няма да можете да правите промени във въведената информация.");
            if (confirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    if (!this.isBtnDisabled)
                    {
                        //this.isApplicationValid = false;
                        //if (this.IsCPO)
                        //{
                        //    await this.ValidateCPOApplicationAsync(false);
                        //}
                        //else
                        //{
                        //    await this.ValidateCIPOApplicationAsync(false);
                        //}

                        //if (this.isApplicationValid)
                        //{
                            await this.form.GetFile();

                            var resultContext = new ResultContext<CandidateProviderVM>();

                            resultContext.ResultContextObject = this.CandidateProviderVM;

                            bool isApplicationSentViaIS = this.CandidateProviderVM.IdApplicationFiling == this.idKvESign ? true : false;
                            resultContext = await this.ProviderService.StartProcedureAsync(resultContext, isApplicationSentViaIS, this.IsCPO);
                            if (resultContext.HasMessages)
                            {
                                if (this.CandidateProviderVM.IdStartedProcedure is not null)
                                {
                                    await this.SetProcedureDocumentDataAsync();
                                }

                                this.isBtnDisabled = true;

                                await this.ShowSuccessAsync(string.Join("", resultContext.ListMessages));

                                await this.CallbackAfterStartedProcedure.InvokeAsync();
                            }
                            else
                            {
                                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                                resultContext.ListErrorMessages.Clear();
                            }
                        //}
                    }
                    else
                    {
                        await this.ShowErrorAsync("Вече има подадени документи към НАПОО!");
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task ValidateCPOApplicationAsync(bool showSuccessToast)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();
                CandidateProviderExcelVM candidateProviderExcelVM = new CandidateProviderExcelVM();
                this.CandidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(this.CandidateProviderVM);
                this.professionalTrainingsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

                if (this.CandidateProviderVM.IdProviderRegistration != 0)
                {
                    candidateProviderExcelVM.ProviderRegistrationHasValue = true;
                }

                if (this.CandidateProviderVM.IdProviderOwnership != 0)
                {
                    candidateProviderExcelVM.ProviderOwnershipHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ProviderEmail))
                {
                    candidateProviderExcelVM.ProviderEmailHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ProviderName))
                {
                    candidateProviderExcelVM.ProviderNameHasValue = true;
                }

                if (this.CandidateProviderVM.IdProviderStatus != 0)
                {
                    candidateProviderExcelVM.ProviderStatusHasValue = true;
                }

                if (this.CandidateProviderVM.IdLocationCorrespondence is not null)
                {
                    candidateProviderExcelVM.LocationCorrespondenceHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ZipCodeCorrespondence))
                {
                    candidateProviderExcelVM.ZipCodeCorrespondenceHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.PersonNameCorrespondence))
                {
                    candidateProviderExcelVM.PersonNameCorrespondenceHasValue = true;
                }

                if (this.CandidateProviderVM.PersonsForNotifications.Any())
                {
                    candidateProviderExcelVM.PersonsForNotificationsHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ProviderPhoneCorrespondence))
                {
                    candidateProviderExcelVM.ProviderPhoneCorrespondenceHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ProviderEmailCorrespondence))
                {
                    candidateProviderExcelVM.ProviderEmailCorrespondenceHasValue = true;
                }

                if (this.CandidateProviderVM.CandidateProviderSpecialities.Any())
                {
                    candidateProviderExcelVM.CandidateProviderSpecialitiesHasValue = true;
                }

                if (this.CandidateProviderVM.CandidateProviderSpecialities is not null)
                {
                    foreach (var speciality in this.CandidateProviderVM.CandidateProviderSpecialities)
                    {
                        var specialityFromDb = this.specialitiesSource.FirstOrDefault(x => x.IdSpeciality == speciality.IdSpeciality);
                        if (specialityFromDb is not null)
                        {
                            if (specialityFromDb.IdStatus != DataSourceService.GetActiveStatusID())
                            {
                                candidateProviderExcelVM.InactiveSpecialities.Add($"{specialityFromDb.Code} {specialityFromDb.Name}");
                            }
                        }
                    }

                    foreach (var providerSpeciality in this.CandidateProviderVM.CandidateProviderSpecialities)
                    {
                        var speciality = this.specialitiesSource.FirstOrDefault(x => x.IdSpeciality == providerSpeciality.IdSpeciality);
                        if (speciality is not null)
                        {
                            var resultFromValidation = await this.ValidateCurriculumAsync(speciality, providerSpeciality);

                            ProviderSpecialityExcelVM modelProv = new ProviderSpecialityExcelVM()
                            {
                                Speciality = $"{speciality.Code} {speciality.Name}",
                                FrameworkProgramHasValue = providerSpeciality.IdFrameworkProgram is not null ? true : false,
                                EducationFormHasValue = providerSpeciality.IdFormEducation is not null ? true : false,
                                IsCurriculumValid = resultFromValidation.Values.FirstOrDefault(),
                                CurriculumExcelVM = resultFromValidation.Keys.FirstOrDefault()
                            };

                            candidateProviderExcelVM.ProviderSpecialities.Add(modelProv);
                        }
                    }
                }

                var model = await this.CandidateProviderService.GetCandidateProviderCPOStructureActivityByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider);
                if (model is not null)
                {
                    if (!string.IsNullOrEmpty(model.Management))
                    {
                        candidateProviderExcelVM.ManagementHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.OrganisationTrainingProcess))
                    {
                        candidateProviderExcelVM.OrganisationTrainingProcessHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.CompletionCertificationTraining))
                    {
                        candidateProviderExcelVM.CompletionCertificationTrainingHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.InternalQualitySystem))
                    {
                        candidateProviderExcelVM.InternalQualitySystemHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.InformationProvisionMaintenance))
                    {
                        candidateProviderExcelVM.InformationProvisionMaintenanceHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.TrainingDocumentation))
                    {
                        candidateProviderExcelVM.TrainingDocumentationHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.TeachersSelection))
                    {
                        candidateProviderExcelVM.TeachersSelectionHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.MTBDescription))
                    {
                        candidateProviderExcelVM.MTBDescriptionHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.DataMaintenance))
                    {
                        candidateProviderExcelVM.DataMaintenanceHasValue = true;
                    }
                }

                if (this.CandidateProviderVM.CandidateProviderTrainers is not null)
                {
                    if (this.CandidateProviderVM.CandidateProviderTrainers.Any())
                    {
                        candidateProviderExcelVM.AddedTrainersHasValue = true;

                        var kvTrainerStatusActive = await this.DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");
                        var trainerDocumentTypes = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType");
                        var certificateKv = trainerDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "Certificate");
                        var autobiographyKv = trainerDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "Autobiography");
                        var declarationOfConsentKv = trainerDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "DeclarationOfConsent");
                        var retrainingDiplomaKv = trainerDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "RetrainingDiploma");
                        var specialitiesAddedIds = new List<int>();
                        foreach (var trainer in this.CandidateProviderVM.CandidateProviderTrainers.Where(x => x.IdStatus == kvTrainerStatusActive.IdKeyValue))
                        {
                            var certificate = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == certificateKv.IdKeyValue && x.HasUploadedFile);
                            var autobiography = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == autobiographyKv.IdKeyValue && x.HasUploadedFile);
                            var declaration = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == declarationOfConsentKv.IdKeyValue && x.HasUploadedFile);
                            var diploma = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == retrainingDiplomaKv.IdKeyValue && x.HasUploadedFile);
                            TrainersExcelVM modelTrainer = new TrainersExcelVM()
                            {
                                FullName = $"{trainer.FirstName} {trainer.SecondName} {trainer.FamilyName}",
                                IsSpecialityAdded = trainer.CandidateProviderTrainerSpecialities.Any(),
                                IsCertificateAdded = certificateKv.DefaultValue3 != null ? certificate != null : true,
                                IsAutobiographyAdded = autobiographyKv.DefaultValue3 != null ? autobiography != null : true,
                                IsDeclarationOfConsentAdded = declarationOfConsentKv.DefaultValue3 != null ? declaration != null : true,
                                IsRetrainingDiplomaAdded = retrainingDiplomaKv.DefaultValue3 != null ? diploma != null : true
                            };

                            candidateProviderExcelVM.AddedTrainers.Add(modelTrainer);

                            foreach (var speciality in trainer.CandidateProviderTrainerSpecialities)
                            {
                                if (!specialitiesAddedIds.Any(x => x == speciality.IdSpeciality))
                                {
                                    specialitiesAddedIds.Add(speciality.IdSpeciality);
                                }
                            }
                        }

                        foreach (var providerSpecialtiy in this.CandidateProviderVM.CandidateProviderSpecialities)
                        {
                            if (!specialitiesAddedIds.Any(x => x == providerSpecialtiy.IdSpeciality))
                            {
                                var speciality = this.specialitiesSource.FirstOrDefault(x => x.IdSpeciality == providerSpecialtiy.IdSpeciality);
                                candidateProviderExcelVM.MissingTrainerSpecialities.Add($"{speciality.Code} {speciality.Name}");
                            }
                        }
                    }
                }

                if (this.CandidateProviderVM.CandidateProviderPremises is not null)
                {
                    if (this.CandidateProviderVM.CandidateProviderPremises.Any())
                    {
                        candidateProviderExcelVM.AddedMTBsHasValue = true;

                        var mtbDocumentTypes = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType");
                        var documentForComplianceWithFireSafetyRulesAndRegulationsKv = mtbDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "DocumentForComplianceWithFireSafetyRulesAndRegulations").IdKeyValue;
                        var documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsKv = mtbDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements").IdKeyValue;
                        var documentsForThePresenceOfMTBInAccordanceWithTheDOSKv = mtbDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsForThePresenceOfMTBInAccordanceWithTheDOS").IdKeyValue;
                        var dDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfessionKv = mtbDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfession").IdKeyValue;
                        var specialitiesAddedIds = new List<int>();
                        candidateProviderExcelVM.IsDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfessionAdded = this.CandidateProviderVM.CandidateProviderPremises.Any(x => x.CandidateProviderPremisesDocuments.Any(y => y.IdDocumentType == dDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfessionKv));
                        foreach (var mtb in this.CandidateProviderVM.CandidateProviderPremises)
                        {
                            var documentForComplianceWithFireSafetyRulesAndRegulations = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == documentForComplianceWithFireSafetyRulesAndRegulationsKv && x.HasUploadedFile);
                            var documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsKv && x.HasUploadedFile);
                            var documentsForThePresenceOfMTBInAccordanceWithTheDOS = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == documentsForThePresenceOfMTBInAccordanceWithTheDOSKv && x.HasUploadedFile);
                            MTBExcelVM modelMTB = new MTBExcelVM()
                            {
                                Name = mtb.PremisesName,
                                IsSpecialityAdded = mtb.CandidateProviderPremisesSpecialities.Any(),
                                IsDocumentForComplianceWithFireSafetyRulesAndRegulationsAdded = documentForComplianceWithFireSafetyRulesAndRegulations != null,
                                IsDocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsAdded = documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements != null,
                                IsDocumentsForThePresenceOfMTBInAccordanceWithTheDOSAdded = documentsForThePresenceOfMTBInAccordanceWithTheDOS != null,
                            };

                            candidateProviderExcelVM.AddedMTBs.Add(modelMTB);

                            foreach (var speciality in mtb.CandidateProviderPremisesSpecialities)
                            {
                                if (!specialitiesAddedIds.Any(x => x == speciality.IdSpeciality))
                                {
                                    specialitiesAddedIds.Add(speciality.IdSpeciality);
                                }
                            }
                        }

                        foreach (var providerSpecialtiy in this.CandidateProviderVM.CandidateProviderSpecialities)
                        {
                            if (!specialitiesAddedIds.Any(x => x == providerSpecialtiy.IdSpeciality))
                            {
                                var speciality = this.specialitiesSource.FirstOrDefault(x => x.IdSpeciality == providerSpecialtiy.IdSpeciality);
                                candidateProviderExcelVM.MissingMTBSpecialities.Add($"{speciality.Code} {speciality.Name}");
                            }
                        }
                    }
                }

                if (this.CandidateProviderVM.CandidateProviderDocuments is not null)
                {
                    var documentTypesKv = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderDocumentType");
                    var regulationsKv = documentTypesKv.FirstOrDefault(x => x.KeyValueIntCode == "RegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenter").IdKeyValue;
                    var feePaidKv = documentTypesKv.FirstOrDefault(x => x.KeyValueIntCode == "FeePaidDocument").IdKeyValue;

                    var regulationsDoc = this.CandidateProviderVM.CandidateProviderDocuments.FirstOrDefault(x => x.IdDocumentType == regulationsKv && x.HasUploadedFile);
                    var feePaidDoc = this.CandidateProviderVM.CandidateProviderDocuments.FirstOrDefault(x => x.IdDocumentType == feePaidKv && x.HasUploadedFile);
                    if (regulationsDoc is not null)
                    {
                        candidateProviderExcelVM.IsRegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenterAdded = true;
                    }

                    if (feePaidDoc is not null)
                    {
                        candidateProviderExcelVM.IsFeePaidDocumentAdded = true;
                    }
                }

                if (this.CandidateProviderVM.IdApplicationFiling is not null)
                {
                    candidateProviderExcelVM.ApplicationFilingHasValue = true;
                    var kvESigned = this.kvApplicationFilingSource.FirstOrDefault(x => x.KeyValueIntCode == "ThroughESignature").IdKeyValue;
                    if (this.CandidateProviderVM.IdApplicationFiling == kvESigned)
                    {
                        if (string.IsNullOrEmpty(this.CandidateProviderVM.ESignApplicationFileName))
                        {
                            candidateProviderExcelVM.ESignedApplicationFileIsMissing = true;
                        }
                    }
                }

                if (this.CandidateProviderVM.IdReceiveLicense is not null)
                {
                    candidateProviderExcelVM.ReceiveLicenseHasValue = true;
                }

                if (this.CandidateProviderVM.IdApplicationFiling is not null)
                {
                    candidateProviderExcelVM.ApplicationFilingHasValue = true;
                    var kvESigned = this.kvApplicationFilingSource.FirstOrDefault(x => x.KeyValueIntCode == "ThroughESignature").IdKeyValue;
                    if (this.CandidateProviderVM.IdApplicationFiling == kvESigned)
                    {
                        if (string.IsNullOrEmpty(this.CandidateProviderVM.ESignApplicationFileName))
                        {
                            candidateProviderExcelVM.ESignedApplicationFileIsMissing = true;
                        }
                    }
                }

                candidateProviderExcelVM.IsFormApplicationValid = await this.form.IsDocumentFilled(this.CandidateProviderVM.IdCandidate_Provider);
                if (!candidateProviderExcelVM.IsFormApplicationValid)
                {
                    candidateProviderExcelVM.FormApplicationMissingFields = await this.form.MissingFieldsDocumentFilled();
                }

                candidateProviderExcelVM.ProviderType = "CPO";
                var validationResult = candidateProviderExcelVM.IsApplicationValid();
                if (validationResult)
                {
                    if (showSuccessToast)
                    {
                        await this.ShowSuccessAsync("Попълнените данни в заявлението за лицензиране отговарят на минималните изисквания. Валидирането е успешно!");
                    }

                    this.isApplicationValid = true;
                }
                else
                {
                    var resultObject = new ResultContext<CandidateProviderExcelVM>();
                    resultObject.ResultContextObject = candidateProviderExcelVM;
                    var result = this.CandidateProviderService.CreateExcelApplicationValidationErrors(resultObject);
                    await this.ShowErrorAsync("Попълнените данни в заявлението за лицензиране не отговарят на заложените минимални изисквания! Моля, отстранете грешките във файла! Валидирането е неуспешно!");
                    await this.JsRuntime.SaveAs($"Errors_Application_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", result.ToArray());
                    this.isApplicationValid = false;
                }

                //await this.HandleRegixDataCheckAsync();
            }
            finally
            {
                this.loading = false;
                this.SpinnerHide();
            }
        }

        private async Task ValidateCIPOApplicationAsync(bool showSuccessToast)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                CandidateProviderExcelVM candidateProviderExcelVM = new CandidateProviderExcelVM();
                this.CandidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(this.CandidateProviderVM);

                if (this.CandidateProviderVM.IdProviderRegistration != 0)
                {
                    candidateProviderExcelVM.ProviderRegistrationHasValue = true;
                }

                if (this.CandidateProviderVM.IdProviderOwnership != 0)
                {
                    candidateProviderExcelVM.ProviderOwnershipHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ProviderEmail))
                {
                    candidateProviderExcelVM.ProviderEmailHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ProviderName))
                {
                    candidateProviderExcelVM.ProviderNameHasValue = true;
                }

                if (this.CandidateProviderVM.IdProviderStatus != 0)
                {
                    candidateProviderExcelVM.ProviderStatusHasValue = true;
                }

                if (this.CandidateProviderVM.IdLocationCorrespondence is not null)
                {
                    candidateProviderExcelVM.LocationCorrespondenceHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ZipCodeCorrespondence))
                {
                    candidateProviderExcelVM.ZipCodeCorrespondenceHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.PersonNameCorrespondence))
                {
                    candidateProviderExcelVM.PersonNameCorrespondenceHasValue = true;
                }

                if (this.CandidateProviderVM.PersonsForNotifications.Any())
                {
                    candidateProviderExcelVM.PersonsForNotificationsHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ProviderPhoneCorrespondence))
                {
                    candidateProviderExcelVM.ProviderPhoneCorrespondenceHasValue = true;
                }

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ProviderEmailCorrespondence))
                {
                    candidateProviderExcelVM.ProviderEmailCorrespondenceHasValue = true;
                }

                if (this.CandidateProviderVM.CandidateProviderSpecialities.Any())
                {
                    candidateProviderExcelVM.CandidateProviderSpecialitiesHasValue = true;
                }

                var model = await this.CandidateProviderService.GetCandidateProviderCIPOStructureActivityByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider);
                if (model is not null)
                {
                    if (!string.IsNullOrEmpty(model.Management))
                    {
                        candidateProviderExcelVM.ManagementHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.OrganisationInformationProcess))
                    {
                        candidateProviderExcelVM.OrganisationInformationProcessHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.InternalQualitySystem))
                    {
                        candidateProviderExcelVM.InternalQualitySystemHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.InformationProvisionMaintenance))
                    {
                        candidateProviderExcelVM.InformationProvisionMaintenanceHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.TrainingDocumentation))
                    {
                        candidateProviderExcelVM.TrainingDocumentationHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.ConsultantsSelection))
                    {
                        candidateProviderExcelVM.ConsultantsSelectionHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.MTBDescription))
                    {
                        candidateProviderExcelVM.MTBDescriptionHasValue = true;
                    }

                    if (!string.IsNullOrEmpty(model.DataMaintenance))
                    {
                        candidateProviderExcelVM.DataMaintenanceHasValue = true;
                    }
                }

                if (this.CandidateProviderVM.CandidateProviderTrainers is not null)
                {
                    if (this.CandidateProviderVM.CandidateProviderTrainers.Any())
                    {
                        candidateProviderExcelVM.AddedTrainersHasValue = true;

                        var kvTrainerStatusActive = await this.DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");
                        var trainerDocumentTypes = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType");
                        var certificateKv = trainerDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "Certificate");
                        var autobiographyKv = trainerDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "Autobiography");
                        var declarationOfConsentKv = trainerDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "DeclarationOfConsent");
                        var retrainingDiplomaKv = trainerDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "RetrainingDiploma");
                        var specialitiesAddedIds = new List<int>();
                        foreach (var trainer in this.CandidateProviderVM.CandidateProviderTrainers.Where(x => x.IdStatus == kvTrainerStatusActive.IdKeyValue))
                        {
                            var certificate = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == certificateKv.IdKeyValue && x.HasUploadedFile);
                            var autobiography = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == autobiographyKv.IdKeyValue && x.HasUploadedFile);
                            var declaration = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == declarationOfConsentKv.IdKeyValue && x.HasUploadedFile);
                            var diploma = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == retrainingDiplomaKv.IdKeyValue && x.HasUploadedFile);
                            TrainersExcelVM modelTrainer = new TrainersExcelVM()
                            {
                                FullName = $"{trainer.FirstName} {trainer.SecondName} {trainer.FamilyName}",
                                IsCertificateAdded = certificateKv.DefaultValue3 != null ? certificate != null : true,
                                IsAutobiographyAdded = autobiographyKv.DefaultValue3 != null ? autobiography != null : true,
                                IsDeclarationOfConsentAdded = declarationOfConsentKv.DefaultValue3 != null ? declaration != null : true,
                                IsRetrainingDiplomaAdded = retrainingDiplomaKv.DefaultValue3 != null ? diploma != null : true
                            };

                            candidateProviderExcelVM.AddedTrainers.Add(modelTrainer);
                        }
                    }
                }

                if (this.CandidateProviderVM.CandidateProviderPremises is not null)
                {
                    if (this.CandidateProviderVM.CandidateProviderPremises.Any())
                    {
                        candidateProviderExcelVM.AddedMTBsHasValue = true;

                        var kvDocumentTypeSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType")).Where(x => x.DefaultValue3 != null & x.DefaultValue3!.Contains("CIPO")).ToList();
                        var docTypeFour = kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsForThePresenceOfMTBCIPO").IdKeyValue;
                        var docTypeThree = kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsForThePresenceOfMTBInAccordanceCIPO").IdKeyValue;
                        var docTypeTwo = kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements").IdKeyValue;
                        var docTypeOne = kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentForComplianceWithFireSafetyRulesAndRegulations").IdKeyValue;

                        foreach (var mtb in this.CandidateProviderVM.CandidateProviderPremises)
                        {
                            var documentForComplianceWithFireSafetyRulesAndRegulations = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == docTypeOne && x.HasUploadedFile);
                            var documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == docTypeTwo && x.HasUploadedFile);
                            var documentsForThePresenceOfMTBInAccordanceCIPO = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == docTypeThree && x.HasUploadedFile);
                            var documentsForThePresenceOfMTBCIPO = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == docTypeFour && x.HasUploadedFile);
                            MTBExcelVM modelMTB = new MTBExcelVM()
                            {
                                Name = mtb.PremisesName,
                                IsDocumentForComplianceWithFireSafetyRulesAndRegulationsAdded = documentForComplianceWithFireSafetyRulesAndRegulations != null,
                                IsDocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsAdded = documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements != null,
                                IsDocumentsForThePresenceOfMTBInAccordanceCIPOAdded = documentsForThePresenceOfMTBInAccordanceCIPO != null,
                                IsDocumentsForThePresenceOfMTBCIPOAdded = documentsForThePresenceOfMTBCIPO != null
                            };

                            candidateProviderExcelVM.AddedMTBs.Add(modelMTB);
                        }
                    }
                }

                if (this.CandidateProviderVM.CandidateProviderDocuments is not null)
                {
                    var documentTypesKv = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderDocumentType");
                    var procedureDocKv = documentTypesKv.FirstOrDefault(x => x.KeyValueIntCode == "ProcedureDocuments").IdKeyValue;

                    var procedureDoc = this.CandidateProviderVM.CandidateProviderDocuments.FirstOrDefault(x => x.IdDocumentType == procedureDocKv && x.HasUploadedFile);
                    if (procedureDoc is not null)
                    {
                        candidateProviderExcelVM.IsProcedureDocumentAdded = true;
                    }
                }

                if (this.CandidateProviderVM.IdApplicationFiling is not null)
                {
                    candidateProviderExcelVM.ApplicationFilingHasValue = true;
                    var kvESigned = this.kvApplicationFilingSource.FirstOrDefault(x => x.KeyValueIntCode == "ThroughESignature").IdKeyValue;
                    if (this.CandidateProviderVM.IdApplicationFiling == kvESigned)
                    {
                        if (string.IsNullOrEmpty(this.CandidateProviderVM.ESignApplicationFileName))
                        {
                            candidateProviderExcelVM.ESignedApplicationFileIsMissing = true;
                        }
                    }
                }

                if (this.CandidateProviderVM.IdReceiveLicense is not null)
                {
                    candidateProviderExcelVM.ReceiveLicenseHasValue = true;
                }

                candidateProviderExcelVM.IsFormApplicationValid = await this.form.IsDocumentFilled(this.CandidateProviderVM.IdCandidate_Provider);
                if (!candidateProviderExcelVM.IsFormApplicationValid)
                {
                    candidateProviderExcelVM.FormApplicationMissingFields = await this.form.MissingFieldsDocumentFilled();
                }

                candidateProviderExcelVM.ProviderType = "CIPO";
                var validationResult = candidateProviderExcelVM.IsApplicationValid();
                if (validationResult)
                {
                    if (showSuccessToast)
                    {
                        await this.ShowSuccessAsync("Попълнените данни в заявлението за лицензиране отговарят на минималните изисквания. Валидирането е успешно!");
                    }

                    this.isApplicationValid = true;
                }
                else
                {
                    var resultObject = new ResultContext<CandidateProviderExcelVM>();
                    resultObject.ResultContextObject = candidateProviderExcelVM;
                    var result = this.CandidateProviderService.CreateExcelApplicationValidationErrors(resultObject);
                    await this.ShowErrorAsync("Попълнените данни в заявлението за лицензиране не отговарят на заложените минимални изисквания! Моля, отстранете грешките във файла! Валидирането е неуспешно!");
                    await this.JsRuntime.SaveAs($"Errors_Application_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", result.ToArray());
                    this.isApplicationValid = false;
                }

                //await this.HandleRegixDataCheckAsync();
            }
            finally
            {
                this.loading = false;
                this.SpinnerHide();
            }
        }

        private async Task HandleRegixDataCheckAsync()
        {
            var callContext = await this.GetCallContextAsync(this.BulstatCheckKV);//todo: Да се добавят и remark,serviceType,serviceURI 
            var actualStateResponseType = this.RegiXService.GetActualState(this.CandidateProviderVM.PoviderBulstat, callContext);
            await this.LogRegiXRequestAsync(callContext, actualStateResponseType != null);

            var company = actualStateResponseType.Company;
            var seatLocation = await this.LocationService.GetAllLocationsAsync(new LocationVM() { LocationCode = actualStateResponseType.Seat.Address.SettlementEKATTE });
            var idLocation = seatLocation?.FirstOrDefault().idLocation;
            var postCode = actualStateResponseType.Seat.Address.PostCode;
            var address = this.RegiXService.GetFormattedAddress(actualStateResponseType.Seat.Address);

            if (company != this.CandidateProviderVM.ProviderOwner || idLocation != this.CandidateProviderVM.IdLocation
                || postCode != this.CandidateProviderVM.ZipCode || address != this.CandidateProviderVM.ProviderAddress)
            {
                var result = await this.CandidateProviderService.UpdateProviderAfterRegixCheckAsync(this.CandidateProviderVM, address, postCode, idLocation.Value, company);
                if (result.Contains("Грешка"))
                {
                    await this.ShowErrorAsync(result);
                }
                else
                {
                    await this.ShowSuccessAsync(result);
                }
            }
        }

        private async Task<Dictionary<CandidateCurriculumExcelVM, bool>> ValidateCurriculumAsync(SpecialityVM speciality, CandidateProviderSpecialityVM candidateProviderSpeciality)
        {
            Dictionary<CandidateCurriculumExcelVM, bool> data = new Dictionary<CandidateCurriculumExcelVM, bool>();
            CandidateCurriculumExcelVM candidateCurriculumExcelVM = new CandidateCurriculumExcelVM();
            data.Add(candidateCurriculumExcelVM, false);
            var professionalTrainingId = (await this.DataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", "B")).IdKeyValue;
            IEnumerable<ERUVM> erusFromDoc = new List<ERUVM>();
            IEnumerable<ERUVM> erusFromSpeciality = new List<ERUVM>();
            erusFromSpeciality = await this.DOCService.GetAllERUsByIdSpecialityAsync(speciality.IdSpeciality);

            if (speciality.IdDOC != null)
            {
                erusFromDoc = await this.DOCService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = speciality.IdDOC ?? default });
            }

            var addedCurriculums = await this.CandidateProviderService.GetActualCandidateCurriculumWithERUIncludedByIdCandidateProviderSpecialityAsync(candidateProviderSpeciality.IdCandidateProviderSpeciality);
            if (addedCurriculums.Any())
            {
                var erus = new HashSet<ERUVM>();
                foreach (var curriculum in addedCurriculums)
                {
                    if (curriculum is not null && curriculum.CandidateCurriculumERUs is not null)
                    {
                        foreach (var candidateCurriculumERU in curriculum.CandidateCurriculumERUs)
                        {
                            if (candidateCurriculumERU is not null && erus is not null && !erus.Any(x => x.IdERU == candidateCurriculumERU.IdERU))
                            {
                                if (candidateCurriculumERU.ERU is not null)
                                {
                                    erus.Add(candidateCurriculumERU.ERU);
                                }
                            }
                        }
                    }
                }

                var docSource = await this.DOCService.GetAllActiveDocAsync();

                double totalHours = 0;
                double theoryHours = 0;
                double practiceHours = 0;
                double extendedProfessionTrainingHours = 0;
                double generalProfessionTrainingHours = 0;
                double industryProfessionTrainingHours = 0;
                double specificProfessionTrainingHours = 0;
                foreach (var curriculum in addedCurriculums)
                {
                    var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == curriculum.IdProfessionalTraining).DefaultValue1;
                    curriculum.ProfessionalTraining = value;

                    if (erusFromSpeciality.Any())
                    {
                        if (curriculum.IdProfessionalTraining != professionalTrainingId)
                        {
                            if (!curriculum.CandidateCurriculumERUs.Any())
                            {
                                candidateCurriculumExcelVM.MissingTopicERUs.Add($"Няма добавена Единица резултат от учене (ЕРУ) към темата!->Тема: {curriculum.Topic}");
                            }
                        }

                    }
                    else
                    {
                        if (speciality.IdDOC.HasValue)
                        {
                            var doc = docSource.FirstOrDefault(x => x.IdDOC == speciality.IdDOC.Value);
                            if (doc is not null)
                            {
                                if (curriculum.IdProfessionalTraining != professionalTrainingId)
                                {
                                    if (!doc.IsDOI && !curriculum.CandidateCurriculumERUs.Any())
                                    {
                                        candidateCurriculumExcelVM.MissingTopicERUs.Add($"Няма добавена Единица резултат от учене (ЕРУ) от ДОС към темата!->Тема: {curriculum.Topic}");
                                    }
                                }
                            }
                        }
                    }

                    if (curriculum.Theory.HasValue)
                    {
                        theoryHours = curriculum.Theory.Value;
                    }
                    else
                    {
                        theoryHours = 0;
                    }

                    if (curriculum.ProfessionalTraining != "Б")
                    {
                        if (curriculum.Practice.HasValue)
                        {
                            practiceHours += curriculum.Practice.Value;
                        }
                        else
                        {
                            practiceHours += 0;
                        }
                    }

                    if (curriculum.ProfessionalTraining == "Б")
                    {
                        var bPracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                        var bTheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                        extendedProfessionTrainingHours += (bPracticeHours + bTheoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "А1")
                    {
                        var a1PracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                        var a2TheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                        generalProfessionTrainingHours += (a1PracticeHours + a2TheoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "А2")
                    {
                        double a2PracticeHours = 0;
                        if (curriculum.Practice.HasValue)
                        {
                            a2PracticeHours = curriculum.Practice.Value;
                        }
                        else
                        {
                            a2PracticeHours = 0;
                        }

                        industryProfessionTrainingHours += (a2PracticeHours + theoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "А3")
                    {
                        double a3PracticeHours = 0;
                        if (curriculum.Practice.HasValue)
                        {
                            a3PracticeHours = curriculum.Practice.Value;
                        }
                        else
                        {
                            a3PracticeHours = 0;
                        }

                        specificProfessionTrainingHours += (a3PracticeHours + theoryHours);
                    }
                }

                if (erusFromSpeciality.Any())
                {
                    if (erus.Count != erusFromSpeciality.Count())
                    {
                        var missingErus = erusFromSpeciality.Where(x => erus.All(y => y.IdERU != x.IdERU)).ToList();

                        foreach (var missingEru in missingErus)
                        {
                            candidateCurriculumExcelVM.MissingDOCERUs.Add($"Единицата резултат от учене (ЕРУ) не е добавена към нито една тема!->ЕРУ: {missingEru.Code}");
                        }
                    }
                }
                else
                {
                    if (speciality.IdDOC.HasValue)
                    {
                        var doc = docSource.FirstOrDefault(x => x.IdDOC == speciality.IdDOC.Value);
                        if (doc is not null)
                        {
                            if (erus.Count != erusFromDoc.Count())
                            {
                                var missingErus = erusFromDoc.Where(x => erus.All(y => y.IdERU != x.IdERU)).ToList();

                                foreach (var missingEru in missingErus)
                                {
                                    if (!doc.IsDOI)
                                    {
                                        candidateCurriculumExcelVM.MissingDOCERUs.Add($"Единицата резултат от учене (ЕРУ) от ДОС не е добавена към нито една тема!->ЕРУ: {missingEru.Code}");
                                    }
                                }
                            }
                        }
                    }
                }

                totalHours += extendedProfessionTrainingHours + generalProfessionTrainingHours + industryProfessionTrainingHours + specificProfessionTrainingHours;
                candidateCurriculumExcelVM.NonCompulsoryHours = extendedProfessionTrainingHours;
                candidateCurriculumExcelVM.CompulsoryHours = totalHours - candidateCurriculumExcelVM.NonCompulsoryHours;

                if (candidateProviderSpeciality.IdFrameworkProgram is not null)
                {
                    var frameworkProgramVM = await this.FrameworkProgramService.GetFrameworkPgoramByIdWithFormEducationsIncludedAsync(new FrameworkProgramVM() { IdFrameworkProgram = candidateProviderSpeciality.IdFrameworkProgram.Value });

                    if (frameworkProgramVM.SectionА > candidateCurriculumExcelVM.CompulsoryHours)
                    {
                        candidateCurriculumExcelVM.MinimumCompulsoryHoursNotReached = true;
                    }

                    if (frameworkProgramVM.SectionB > candidateCurriculumExcelVM.NonCompulsoryHours)
                    {
                        candidateCurriculumExcelVM.MinimumChoosableHoursNotReached = true;
                    }

                    candidateCurriculumExcelVM.PercentCompulsoryHours = (generalProfessionTrainingHours / candidateCurriculumExcelVM.CompulsoryHours) * 100;
                    if (frameworkProgramVM.SectionА1 < candidateCurriculumExcelVM.PercentCompulsoryHours)
                    {
                        candidateCurriculumExcelVM.MaximumPercentNotReached = true;
                    }

                    candidateCurriculumExcelVM.PercentSpecificTraining = (practiceHours / (industryProfessionTrainingHours + specificProfessionTrainingHours)) * 100;
                    if (frameworkProgramVM.Practice > candidateCurriculumExcelVM.PercentSpecificTraining)
                    {
                        candidateCurriculumExcelVM.MinimumPercentNotReached = true;
                    }
                }
                else
                {
                    candidateCurriculumExcelVM.FrameworkProgramNotAdded = true;
                }
            }
            else
            {
                candidateCurriculumExcelVM.CurriculumNotAdded = true;
            }

            if (!(candidateCurriculumExcelVM.MinimumPercentNotReached
                || candidateCurriculumExcelVM.MaximumPercentNotReached
                || candidateCurriculumExcelVM.MinimumChoosableHoursNotReached
                || candidateCurriculumExcelVM.MinimumCompulsoryHoursNotReached
                || candidateCurriculumExcelVM.MissingDOCERUs.Any()
                || candidateCurriculumExcelVM.MissingTopicERUs.Any()))
            {
                data[candidateCurriculumExcelVM] = true;
            }

            return data;
        }

        private void ShowHideButtons()
        {
            //Замразяване на бутона за подаване на документи
            if (this.CandidateProviderVM.IdStartedProcedure.HasValue)
            {
                this.isBtnDisabled = true;
            }
            else
            {
                this.isBtnDisabled = false;
            }
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.CandidateProviderVM.ESignApplicationFileName))
            {
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли си сте, че искате да изтриете прикачения файл?");
                if (isConfirmed)
                {
                    this.SpinnerShow();

                    if (this.loading)
                    {
                        return;
                    }
                    try
                    {
                        this.loading = true;

                        var result = await this.UploadFileService.RemoveESignedApplicationFileByNameAsync(this.CandidateProviderVM.IdCandidate_Provider, this.CandidateProviderVM.ESignApplicationFileName);
                        if (result == 1)
                        {
                            this.CandidateProviderVM.ESignApplicationFileName = null;
                        }

                        this.StateHasChanged();
                    }
                    finally
                    {
                        this.loading = false;
                    }

                    this.SpinnerHide();
                }
            }
        }

        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.CandidateProviderVM.ESignApplicationFileName))
            {
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли си сте, че искате да изтриете прикачения файл?");
                if (isConfirmed)
                {
                    this.SpinnerShow();

                    if (this.loading)
                    {
                        return;
                    }
                    try
                    {
                        this.loading = true;

                        var result = await this.UploadFileService.RemoveESignedApplicationFileByNameAsync(this.CandidateProviderVM.IdCandidate_Provider, fileName);
                        if (result == 1)
                        {
                            this.CandidateProviderVM.ESignApplicationFileName = null;
                        }

                        this.StateHasChanged();
                    }
                    finally
                    {
                        this.loading = false;
                    }

                    this.SpinnerHide();
                }
            }
        }

        private async Task OnDownloadClick()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(this.CandidateProviderVM.ESignApplicationFileName))
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileESignedApplicationAsync(this.CandidateProviderVM);

                    await FileUtils.SaveAs(this.JsRuntime, this.CandidateProviderVM.ESignApplicationFileName, documentStream.ToArray());
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var file = args.Files[0].Stream;
                this.CandidateProviderVM.ESignApplicationFileName = args.Files[0].FileInfo.Name;

                var result = await this.UploadFileService.UploadFileESignedApplicationAsync(file, this.CandidateProviderVM);

                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenPaymentFeeListModal()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.CandidateProviderVM.fromPage = "ApplicationStatus";
                await this.paymentFeeListModal.openPaymentFeeList(this.CandidateProviderVM);
            }
            finally
            {
                this.loading = false;
            }
        }

        private class Model { }
    }
}
