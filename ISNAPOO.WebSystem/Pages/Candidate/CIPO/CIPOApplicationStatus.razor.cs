using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Licensing;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Resources;
using Syncfusion.Blazor.Inputs;
using ISNAPOO.Core.Contracts.EKATTE;
using RegiX;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.WebSystem.Pages.EGovPayment;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOApplicationStatus : BlazorBaseComponent
    {
        private IEnumerable<KeyValueVM> kvReceiveLicenseSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationFilingSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvVQSSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvLicensingSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvProcedureDocumentTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvCIPOLicensing = new KeyValueVM();
        private ProcedureDocumentVM procedureDocumentVM = new ProcedureDocumentVM();
        private CIPOFormApplicationModal form = new CIPOFormApplicationModal();
        private PaymentFeeListModal paymentFeeListModal = new PaymentFeeListModal();
        private string licenseType = string.Empty;
        private string applicationStatus = string.Empty;
        private bool isBtnDisabled = false;
        private bool isUserInRoleNAPOO = false;
        private bool isApplicationValid = false;
        public int idKvESign = 0;

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public EventCallback<ResultContext<CandidateProviderVM>> CallbackAfterSubmit { get; set; }

        [Parameter]
        public bool DisableAllFields { get; set; }

        [Inject]
        public IProviderService providerService { get; set; }

        [Inject]
        public ILicensingProcedureDocumentCIPOService LicensingService { get; set; }

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
        public IRegiXService RegiXService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerShow();

            this.isUserInRoleNAPOO = await this.IsInRole("NAPOO_Expert");

            this.editContext = new EditContext(new Model());

            this.kvApplicationFilingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationFilingType");
            this.idKvESign = this.kvApplicationFilingSource.FirstOrDefault(x => x.KeyValueIntCode == "ThroughESignature").IdKeyValue;
            this.kvReceiveLicenseSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ReceiveLicenseType");
            this.kvVQSSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            this.kvLicensingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType");
            this.kvApplicationStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
            this.kvProcedureDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
            this.kvCIPOLicensing = this.kvProcedureDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "RequestLicensingCIPO");

            if (this.CandidateProviderVM.IdStartedProcedure is not null)
            {
                await this.SetProcedureDocumentDataAsync();
            }

            if (this.CandidateProviderVM.IdApplicationStatus != null)
            {
                var status = this.kvApplicationStatusSource.FirstOrDefault(x => x.IdKeyValue == this.CandidateProviderVM.IdApplicationStatus);

                if (status is not null)
                {
                    this.applicationStatus = status.Name;
                }
            }

            if (this.CandidateProviderVM.IdTypeLicense != 0)
            {
                this.licenseType = this.kvLicensingSource.FirstOrDefault(x => x.IdKeyValue == this.CandidateProviderVM.IdTypeLicense).Name;
            }

            await this.ShowHideButtons();

            this.SpinnerHide();
        }

        private async Task SetProcedureDocumentDataAsync()
        {
            this.procedureDocumentVM = await this.ProviderService.GetProcedureDocumentByIdStartedProcedureAndIdDocumentTypeAsync(this.CandidateProviderVM.IdStartedProcedure.Value, this.kvCIPOLicensing.IdKeyValue);
            this.CandidateProviderVM.ApplicationDate = this.procedureDocumentVM.DS_OFFICIAL_DATE;
            this.CandidateProviderVM.ApplicationNumber = this.procedureDocumentVM.DS_OFFICIAL_DocNumber;
        }

        private async Task PrintApplication()
        {
            this.SpinnerShow();

            if (this.CandidateProviderVM.IdApplicationFiling == null && this.CandidateProviderVM.IdReceiveLicense == null)
            {
                await this.ShowErrorAsync("Моля, изберете изберете начин на получаване на административен акт и лицензия и начин на подаване на заявление и документ за платена държавна такса!");
            }
            else if (this.CandidateProviderVM.IdApplicationFiling == null)
            {
                await this.ShowErrorAsync("Моля, изберете начин на подаване на заявление и документ за платена държавна такса!");
            }
            else if (this.CandidateProviderVM.IdReceiveLicense == null)
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

                var file = await this.LicensingService.GenerateLicensingApplication(this.CandidateProviderVM, this.kvApplicationFilingSource, this.kvReceiveLicenseSource, this.kvVQSSource);

                await FileUtils.SaveAs(this.JsRuntime, "Zaqvlenie-Licenzirane-CIPO.pdf", file.ToArray());
            }

            this.SpinnerHide();
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
                        this.isApplicationValid = false;
                        await this.ValidateApplicationAsync(false);
                        if (this.isApplicationValid)
                        {
                            var resultContext = new ResultContext<CandidateProviderVM>();
                            resultContext.ResultContextObject = this.CandidateProviderVM;
                            await this.form.GetFile();

                            bool isApplicationSentViaIS = this.CandidateProviderVM.IdApplicationFiling == this.idKvESign ? true : false;
                            resultContext = await providerService.StartCIPOProcedureAsync(resultContext, isApplicationSentViaIS);

                            if (resultContext.HasMessages)
                            {
                                this.isBtnDisabled = true;
                                await this.CallbackAfterSubmit.InvokeAsync(resultContext);
                                //await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                                resultContext.ListMessages.Clear();
                            }
                            else
                            {
                                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                                resultContext.ListErrorMessages.Clear();
                            }
                        }
                    }
                    else
                    {
                        await this.ShowErrorAsync("Вече са подадени документи към НАПОО!");
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task ShowHideButtons()
        {
            //Замразяване на бутона за подаване на документи
            if (this.CandidateProviderVM.IdStartedProcedure != null)
            {
                this.isBtnDisabled = true;
            }
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.CandidateProviderVM.ESignApplicationFileName))
            {

                string msg = "Сигурни ли си сте, че искате да изтриете прикачения файл?";
                bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveESignedApplicationFileByNameAsync(this.CandidateProviderVM.IdCandidate_Provider, this.CandidateProviderVM.ESignApplicationFileName);
                    if (result == 1)
                    {
                        this.CandidateProviderVM.ESignApplicationFileName = null;
                    }

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.CandidateProviderVM.ESignApplicationFileName))
            {
                string msg = "Сигурни ли си сте, че искате да изтриете прикачения файл?";
                bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveESignedApplicationFileByNameAsync(this.CandidateProviderVM.IdCandidate_Provider, fileName);
                    if (result == 1)
                    {
                        this.CandidateProviderVM.ESignApplicationFileName = null;
                    }

                    this.StateHasChanged();
                }
            }
        }

        private async void OnDownloadClick()
        {
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

        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;
            this.CandidateProviderVM.ESignApplicationFileName = args.Files[0].FileInfo.Name;

            var result = await this.UploadFileService.UploadFileESignedApplicationAsync(file, this.CandidateProviderVM);

            this.StateHasChanged();
        }

        private async Task ValidateApplicationAsync(bool showSuccessToast)
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
            var callContext = await this.GetCallContextAsync(this.BulstatCheckKV);
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
        private async Task OpenPaymentFeeListModal()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.CandidateProviderVM.fromPage = "CIPOApplicationStatus";
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
