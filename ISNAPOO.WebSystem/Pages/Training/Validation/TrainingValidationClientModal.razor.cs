using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.RegularExpressions;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NuGet.Packaging;
using Org.BouncyCastle.Crypto.Prng;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationClientModal : BlazorBaseComponent
    {
        private ValidationVerificationSubmission verificationSubmission = new ValidationVerificationSubmission();
        private SfDialog sfDialog;

        private ValidationClientVM client;
        private ValidationClientCombinedVM model;

        private TrainingValidationClientDataModel ClientDataModal;
        private TrainingValidationClientValidationModal ClientValidationModal;
        private TrainingValidationClientCommisionModal ClientCommisionModal;
        private TrainingValidationClientFinishedDataModal ClientFinishedDataModal;
        private TrainingValidationClientOrdersList trainingValidationClientOrdersList;
        private ValidationClientTrainersModal ClientTrainerModal;
        private ValidationCurriculumModal validationCurriculumModal;
        private TrainingValidationClientPremisiesModal clientPremisiesModal;
        private TrainingValidationClientProtocolsModal ClientProtocols;
        private TrainingValidationClientEducationModal ClientEducationModal;
        private TrainingValidationClientCompetenciesModal ClientCompetenciesModal;
        private ValidationClientIssueDuplicate validationClientIssueDuplicate = new ValidationClientIssueDuplicate();
        private List<string> validations = new List<string>();
        private KeyValueVM kvIssueOfDuplicate = new KeyValueVM();
        private bool showUnsavedChangesConfirmDialog = false;
        private IEnumerable<KeyValueVM> kvSexSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvNationalitySource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvIndentTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvCourseFinished = new KeyValueVM();
        private KeyValueVM kvCourseCurrent = new KeyValueVM();
        private KeyValueVM kvEGN = new KeyValueVM();
        private KeyValueVM kvLNCh = new KeyValueVM();
        private KeyValueVM kvIDN = new KeyValueVM();
        private IEnumerable<KeyValueVM> finishedTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvCourseCompleted = new KeyValueVM();
        private KeyValueVM kvQualificationLevel = new KeyValueVM();
        private List<ValidationCommissionMemberVM> membersSource = new List<ValidationCommissionMemberVM>();
        private int selectedTab = 0;
        DocVM docVM = new DocVM();
        ValidationERUList eruList = new ValidationERUList();
        private ValidationClientCombinedVM finishedDataModel = new ValidationClientCombinedVM();
        private ValidationClientCombinedVM duplicateFinishedDataModel = new ValidationClientCombinedVM();
        private bool HideWhenSPK = false;
        private bool DisableNotificationButton = false;
        private bool areTabsEditable = true;
        private bool showIssueDuplicateTab = false;
        private bool entryFromRIDPKModule = false;
        private KeyValueVM kvCertificateForValidationOfProfessionalQualificationInPartOfAProfession;
        private KeyValueVM kvVocationalQualificationValidationCertificate;

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }
        [Parameter]
        public int PageType { get; set; }
        [Inject]
        public ILocationService LocationService { get; set; }
        [Inject]
        public ITrainingService TrainingService { get; set; }
        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITemplateDocumentService templateDocumentService { get; set; }
        [Inject]
        public Microsoft.JSInterop.IJSRuntime JS { get; set; }
        [Inject]
        public IDOCService DOCService { get; set; }

        public override bool IsContextModified
        {
            get
            {
                if (this.areTabsEditable && !this.entryFromRIDPKModule)
                {
                    return (ClientValidationModal != null && ClientValidationModal.IsEditContextModified())
                            || (ClientFinishedDataModal != null && ClientFinishedDataModal.IsEditContextModified());
                }

                return false;
            }
        }

        public async void openModal(ValidationClientVM client, int IdCourseType, bool areTabsEditable, bool entryFromRIDPKModule = false)
        {
            selectedTab = 0;

            this.entryFromRIDPKModule = entryFromRIDPKModule;

            this.validations.Clear();
            this.client = client;

            this.areTabsEditable = areTabsEditable;

            if (client.IdValidationClient != 0)
                await UpdateClient();
            else
                client.IdCandidateProvider = this.UserProps.IdCandidateProvider;

            this.editContext = new EditContext(this.client);

            client.IdCourseType = IdCourseType;
            if (this.client is not null)
            {
                DisableNotificationButton = this.client.DS_OFFICIAL_ID != null;
                this.model = await this.TrainingService.GetValidationClientCombinedVMByIdClientCourseAsync(this.client.IdValidationClient);
                this.membersSource = (await this.TrainingService.GetAllValidationCommissionMembersByClient(this.client.IdValidationClient)).ToList();
            }
            this.kvIssueOfDuplicate = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type6");
            this.kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvQualificationLevel = await this.DataSourceService.GetKeyValueByIntCodeAsync("QualificationLevel", "WithoutQualification_Update");
            this.kvLNCh = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
            this.kvIDN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "IDN");
            this.kvSexSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            this.kvNationalitySource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality");
            this.finishedTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType");
            this.kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.kvCourseCompleted = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
            this.kvCourseCurrent = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusNow");
            this.kvCourseFinished = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type5");

            if (this.client.Speciality is not null)
            {
                this.docVM = await this.DOCService.GetActiveDocByProfessionIdAsync(this.client.Speciality.Profession);
            }
            var spk = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfProfessionalQualifications");
            if (this.PageType == spk.IdKeyValue)
                this.HideWhenSPK = true;

            if (this.client.IsArchived)
            {
                // проверява дали има издаден дубликат на документ
                await this.IsDuplicateDocumentIssuedAsync();
            }
            else
            {
                this.showIssueDuplicateTab = true;
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        // проверява дали има издаден дубликат на документ
        private async Task IsDuplicateDocumentIssuedAsync()
        {
            this.showIssueDuplicateTab = await this.TrainingService.IsDuplicateIssuedByIdValidationClientAsync(this.client.IdClient);
        }

        private void GetValidationModelFromValidationClientIssueDuplicate(ValidationClientCombinedVM model)
        {
            this.duplicateFinishedDataModel = model;
        }

        public async Task Submit(bool showSuccessToast)
        {
            if (loading) return;

            try
            {
                this.loading = true;
                validations.Clear();

                await ClientDataModal.Submit();
                this.client = ClientDataModal.ClientVM;
                validations.AddRange(ClientDataModal.validationMessages);
                this.StateHasChanged();


                if (ClientValidationModal != null)
                {
                    await ClientValidationModal.Submit();
                    validations.AddRange(ClientValidationModal.validationMessages);
                    if (ClientValidationModal.NoCourses)
                        return;
                    this.StateHasChanged();
                }

                if (ClientFinishedDataModal != null && (ClientFinishedDataModal.IsEditContextModified() || ClientFinishedDataModal.errorMessages.Any()))
                {
                    ClientFinishedDataModal.SubmitHandler();
                    validations.AddRange(ClientFinishedDataModal.errorMessages);
                }

                if (this.validationClientIssueDuplicate != null && (this.validationClientIssueDuplicate.IsEditContextModified() || this.validationClientIssueDuplicate.errorMessages.Any()))
                {
                    this.validationClientIssueDuplicate.SubmitHandler();
                    validations.AddRange(this.validationClientIssueDuplicate.errorMessages);
                }

                if (!validations.Any())
                {
                    if (this.client.IdStatus == this.kvCourseCurrent.IdKeyValue && await this.TrainingService.IsCandidateProviderLicenceSuspendedAsync(this.client.IdCandidateProvider, this.client.StartDate!.Value))
                    {
                        await this.ShowErrorAsync("Не можете да създадете процедура по валидиране, тъй като в момента сте с временно отнета лицензия!");
                        this.SpinnerHide();
                        return;
                    }

                    if (showSuccessToast)
                    {
                        await this.ShowSuccessAsync("Записът е успешен!");
                        await this.CallbackAfterSubmit.InvokeAsync();
                    }

                    await UpdateClient();
                }
            }
            finally
            {
                this.loading = false;
            }
            this.StateHasChanged();
        }

        private async Task CompleteValidationBtn()
        {
            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да приключите процедурата по валидиране?");
            if (isConfirmed)
            {
                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    await this.Submit(false);

                    if (!this.validations.Any())
                    {
                        this.SpinnerShow();

                        var inputContext = new ResultContext<ValidationClientVM>();
                        inputContext.ResultContextObject = this.client;
                        inputContext = await this.TrainingService.CompleteValidationAsync(inputContext);
                        if (inputContext.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join(Environment.NewLine, inputContext.ListErrorMessages));
                        }
                        else
                        {
                            this.isVisible = false;

                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, inputContext.ListMessages));

                            await this.CallbackAfterSubmit.InvokeAsync();
                        }

                        this.SpinnerHide();
                    }
                }
                finally
                {
                    this.loading = false;
                }
            }
        }

        //private void Select(SelectingEventArgs args)
        //{

        //}ff

        //private async Task OnTabSelected(SelectEventArgs args)
        //{

        //}
        private async Task UpdateClient(int? id = null)
        {
            if (id.HasValue)
                client.IdValidationClient = id.Value;

            if (client.IdValidationClient != 0)
                this.client = await TrainingService.GetValidationClientByIdAsync(client.IdValidationClient);
            if (this.client.Speciality is not null)
            {
                this.docVM = await this.DOCService.GetActiveDocByProfessionIdAsync(this.client.Speciality.Profession);
            }

            await this.CallbackAfterSubmit.InvokeAsync();
            this.StateHasChanged();
        }

        public static int CalculateNumberOfLines(string text, float boxWidthInPoints, Font font)
        {
            var bitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(72, 72);



            using (var g = Graphics.FromImage(bitmap))
            {


                var stringFormat = StringFormat.GenericTypographic;
                stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                int charactersFitted;
                int linesFitted;
                g.MeasureString(text, font, new SizeF(boxWidthInPoints, 0), stringFormat, out charactersFitted,
                    out linesFitted);
                if (text.Trim().Length == 0)
                {
                    return linesFitted - 1;
                }
                return linesFitted;
            }

        }

        public async Task GetValidationMessage(List<string> messages)
        {
            this.validations.AddRange(messages);
        }

        public async Task SendDocumentForExam()
        {
            var saveDoc = true;

            if (HideWhenSPK)
            {
                if (this.client.ExamPracticeDate is null || this.client.ExamTheoryDate is null)
                {
                    await this.ShowErrorAsync("Моля, въведете дата за държавен изпит - част по теория и дата за държавен изпит - част по по практика!");
                    saveDoc = false;
                }
                else
                {
                    var dateDifferenceTheory = (this.client.ExamTheoryDate.Value - DateTime.Today).Days;
                    var dateDifferencePractice = (this.client.ExamPracticeDate.Value - DateTime.Today).Days;

                    if (dateDifferenceTheory < 7 || 7 > dateDifferencePractice)
                    {
                        await this.ShowErrorAsync("Не можете да изпратите известие за изпит към НАПОО, защото въведениета дати не отговарят на изискването да са поне 7 календарни дни преди датата на провеждане на изпита!");
                        saveDoc = false;
                    }
                }
            }
            else
            {
                if (this.client.ExamPracticeDate is null || this.client.ExamTheoryDate is null)
                {
                    await this.ShowErrorAsync("Моля, въведете дата за изпит - част по теория и дата за изпит - част по по практика!");
                    saveDoc = false;
                }
                else
                {
                    var dateDifferenceTheory = (this.client.ExamTheoryDate.Value - DateTime.Now).Days;
                    var dateDifferencePractice = (this.client.ExamPracticeDate.Value - DateTime.Now).Days;

                    if (dateDifferenceTheory < 7 || 7 > dateDifferencePractice)
                    {
                        await this.ShowErrorAsync("Не можете да изпратите известие за изпит към НАПОО, защото въведениета дати не отговарят на изискването да са поне 7 календарни дни преди датата на провеждане на изпита!");
                        saveDoc = false;
                    }
                }
            }
            if (saveDoc)
            {
                var result = await this.TrainingService.AddValidationDocumentToIS(this.client);
                if (!result.HasErrorMessages)
                    await this.ShowSuccessAsync("Записът е успешен!");
                else
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));


                this.DisableNotificationButton = true;
                await UpdateClient();
            }
        }

    }
}
