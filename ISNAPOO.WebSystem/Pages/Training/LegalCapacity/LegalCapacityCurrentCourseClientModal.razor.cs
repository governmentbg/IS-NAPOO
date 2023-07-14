using System.Drawing.Imaging;
using System.Drawing;
using System.Text;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Services;
using ISNAPOO.Core.Services.DOC;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC.NKPD;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Navigations;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using ISNAPOO.Core.Contracts.DOC.NKPD;
using Syncfusion.DocIORenderer;
using ISNAPOO.Core.ViewModels.Common;

namespace ISNAPOO.WebSystem.Pages.Training.LegalCapacity
{
    public partial class LegalCapacityCurrentCourseClientModal : BlazorBaseComponent, IConcurrencyCheck<ClientCourseVM>
    {
        private ClientCourseInformation clientCourseInformation = new ClientCourseInformation();
        private ClientCourseEducation clientCourseEducation = new ClientCourseEducation();
        private LegalCapacityClientCourseIssueLegalCapacityOrdinance clientCourseIssueLegalCapacityOrdinance = new LegalCapacityClientCourseIssueLegalCapacityOrdinance();

        private ClientCourseVM clientCourseVM = new ClientCourseVM();
        private CourseVM courseVM = new CourseVM();
        private List<string> validationMessages = new List<string>();
        private int selectedTab = 0;
        private KeyValueVM kvQualificationLevel = new KeyValueVM();
        private List<ClientCourseVM> addedClientCourses = new List<ClientCourseVM>();
        private List<KeyValueVM> courseStatusSource = new List<KeyValueVM>();
        private KeyValueVM kvEGN = new KeyValueVM();
        private KeyValueVM kvLNCh = new KeyValueVM();
        private KeyValueVM kvIDN = new KeyValueVM();
        private KeyValueVM kvIssueOfDuplicate = new KeyValueVM();
        private IEnumerable<KeyValueVM> kvSexSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvNationalitySource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvIndentTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvFinishedWithDoc = new KeyValueVM();
        private KeyValueVM kvCourseFinished = new KeyValueVM();
        private bool disableNextBtn = false;
        private bool disablePreviousBtn = false;
        private bool hideBtnsConcurrentModal = false;
        private bool isDocumentPresent = false;
        private bool showClientBtns = true;
        private int nextId = 0;
        private int previousId = 0;
        private ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM legalCapacityOrdinanceDataModel = new ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM();
        private IEnumerable<CourseSubjectVM> courseSubjectSource = new List<CourseSubjectVM>();
        private KeyValueVM kvSPKDocType = new KeyValueVM();
        private IEnumerable<KeyValueVM> professionalTrainingTypesSource;
        private KeyValueVM kvCertificateOfVocationalTraining;

        public override bool IsContextModified => this.clientCourseInformation.IsEditContextModified() || this.clientCourseIssueLegalCapacityOrdinance.GetIsEditContextModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public Microsoft.JSInterop.IJSRuntime JS { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDOCService DocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public INKPDService NKPDService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITemplateDocumentService templateDocumentService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.clientCourseVM);
        }

        public async Task OpenModal(ClientCourseVM clientCourse, CourseVM course, List<ClientCourseVM> clientCourses, IEnumerable<KeyValueVM> courseStatusSource, bool isDocumentPresent = false, bool showClientBtns = true, ConcurrencyInfo concurrencyInfo = null)
        {
            this.showClientBtns = showClientBtns;
            this.isDocumentPresent = isDocumentPresent;

            this.validationMessages.Clear();
            this.selectedTab = 0;
            this.kvIssueOfDuplicate = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type6");
            this.clientCourseIssueLegalCapacityOrdinance.isTabRendered = false;
            this.editContext = new EditContext(this.clientCourseVM);
            this.kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvLNCh = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
            this.kvIDN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "IDN");
            this.kvQualificationLevel = await this.DataSourceService.GetKeyValueByIntCodeAsync("QualificationLevel", "WithoutQualification_Update");
            this.kvSPKDocType = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");
            this.kvSexSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            this.kvNationalitySource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality");

            this.kvFinishedWithDoc = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type1");

            this.kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.professionalTrainingTypesSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");


            this.clientCourseVM = clientCourse;
            if (this.clientCourseVM.IdClientCourse != 0)
            {
                this.IdClientCourse = this.clientCourseVM.IdClientCourse;
            }

            this.SetDefaultNationalityValuesForClientCourseVM();

            this.courseVM = course;
            this.addedClientCourses = clientCourses.ToList();

            this.editContext = new EditContext(this.clientCourseVM);

            await this.SetCreateAndModifyInfoAsync();

            this.SetButtonsState();

            this.isVisible = true;
            this.StateHasChanged();

            if (concurrencyInfo is not null && concurrencyInfo.IdPerson != this.UserProps.IdPerson)
            {
                this.hideBtnsConcurrentModal = true;
                this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfo);
            }
            else
            {
                this.hideBtnsConcurrentModal = false;
            }
        }

        private void Select(SelectingEventArgs args)
        {
            if (args.IsSwiped)
            {
                args.Cancel = true;
            }
        }

        private void SetDefaultNationalityValuesForClientCourseVM()
        {
            if (this.clientCourseVM.IdClientCourse == 0)
            {
                var kvNationalityBG = this.kvNationalitySource.FirstOrDefault(x => x.Name == "България");
                this.clientCourseVM.IdNationality = kvNationalityBG!.IdKeyValue;
                this.clientCourseVM.IdCountryOfBirth = kvNationalityBG!.IdKeyValue;
            }
        }

        private async Task SubmitBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.validationMessages.Clear();
                this.clientCourseInformation.SubmitHandler();
                this.validationMessages.AddRange(this.clientCourseInformation.GetValidationMessages());

                //if (this.courseVM.IdStatus == this.kvCourseFinished.IdKeyValue)
                //{

                if (this.clientCourseIssueLegalCapacityOrdinance.IsEditContextModified() || this.clientCourseIssueLegalCapacityOrdinance.isTabRendered)
                {
                    this.clientCourseIssueLegalCapacityOrdinance.SubmitHandler();
                    this.validationMessages.AddRange(this.clientCourseIssueLegalCapacityOrdinance.GetValidationMessages());
                }
                //}

                if (!this.validationMessages.Any())
                {
                    var result = new ResultContext<ClientCourseVM>();
                    result.ResultContextObject = this.clientCourseVM;
                    var addForConcurrencyCheck = false;
                    if (this.clientCourseVM.IdClientCourse == 0)
                    {
                        if (this.addedClientCourses.Any(x => x.Indent == this.clientCourseVM.Indent.Trim()))
                        {
                            await this.ShowErrorAsync($"Курсист {this.clientCourseVM.FullName} е вече добавен към текущия курс!");
                            return;
                        }

                        result = await this.TrainingService.CreateTrainingClientCourseAsync(result, this.courseVM.IdCandidateProvider.Value, this.courseVM.IdTrainingCourseType.Value);
                        addForConcurrencyCheck = true;
                    }
                    else
                    {
                        //if (this.courseVM.IdStatus == this.kvCourseFinished.IdKeyValue)
                        //{
                        result = await this.TrainingService.UpdateLegalCapacityTrainingClientCourseAsync(result, this.courseVM.IdCandidateProvider.Value, this.legalCapacityOrdinanceDataModel);
                        //}
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                        await this.SetCreateAndModifyInfoAsync();

                        this.SetButtonsState();

                        if (addForConcurrencyCheck)
                        {
                            await this.AddEntityIdAsCurrentlyOpened(this.clientCourseVM.IdClientCourse, "TrainingClientCourse");
                            this.IdClientCourse = this.clientCourseVM.IdClientCourse;
                        }

                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                }
                else
                {
                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SubmitAndContinueBtn()
        {
            await this.SubmitBtn();

            if (!this.validationMessages.Any())
            {
                var kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
                var kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");

                this.clientCourseVM = new ClientCourseVM()
                {
                    IdCourse = this.courseVM.IdCourse,
                    Course = this.courseVM,
                    IdIndentType = kvEGN.IdKeyValue
                };

                this.SetDefaultNationalityValuesForClientCourseVM();

                this.nextId = this.addedClientCourses.Count;
                this.disableNextBtn = true;
                this.previousId = this.addedClientCourses.Count - 1;

                if (!this.clientCourseVM.CourseJoinDate.HasValue)
                {
                    if (this.courseVM.StartDate.HasValue)
                    {
                        this.clientCourseVM.CourseJoinDate = this.courseVM.StartDate.Value;
                    }
                    else
                    {
                        this.clientCourseVM.CourseJoinDate = DateTime.Now;
                    }
                }
            }
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.clientCourseVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.clientCourseVM.IdModifyUser);
            this.clientCourseVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.clientCourseVM.IdCreateUser);
        }

        private async void NextClient()
        {
            if (this.IsContextModified)
            {
                bool confirmed = await this.ShowConfirmDialogAsync("Имате незапазени промени! Сигурни ли сте, че искате да смените курсиста?");
                if (confirmed)
                {
                    var cc = this.addedClientCourses.FirstOrDefault(x => x.IdClientCourse == this.clientCourseVM.IdClientCourse)!;
                    var currentIdx = this.addedClientCourses.IndexOf(cc);
                    this.nextId = currentIdx + 1;
                    if (this.nextId < this.addedClientCourses.Count)
                    {
                        this.SpinnerShow();

                        if (this.loading)
                        {
                            return;
                        }
                        try
                        {
                            this.loading = true;

                            this.RemoveEntityIdAsCurrentlyOpened(this.clientCourseVM.IdClientCourse, "TrainingClientCourse");

                            this.clientCourseVM = this.addedClientCourses[this.nextId];

                            this.SetDefaultNationalityValuesForClientCourseVM();

                            await this.SetIsDocumentPresentAsync();
                            if (!this.clientCourseVM.CourseJoinDate.HasValue)
                            {
                                if (this.courseVM.StartDate.HasValue)
                                {
                                    this.clientCourseVM.CourseJoinDate = this.courseVM.StartDate.Value;
                                }
                                else
                                {
                                    this.clientCourseVM.CourseJoinDate = DateTime.Now;
                                }
                            }

                            await this.LoadLegalCapacityOrdinanceTabAsync();

                            await this.LoadEducationTabAsync();

                            this.HandleIdentType();

                            await this.SetCreateAndModifyInfoAsync();

                            this.SetButtonsState();

                            await this.SetConcurrencyInfoAsync();

                            this.clientCourseVM.IdCityOfBirth = await this.TrainingService.GetTrainingClientCourseIdCityOfBirthByIdAsync(this.clientCourseVM.IdClientCourse);
                            if (this.clientCourseVM.IdCityOfBirth != null)
                            {
                                this.clientCourseInformation.locationSource.Clear();
                                LocationVM location = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.clientCourseVM.IdCityOfBirth.Value);
                                this.clientCourseInformation.locationSource.Add(location);
                            }

                            this.editContext = new EditContext(this.clientCourseVM);

                            this.clientCourseInformation.SetEditContextAsUnmodified();
                            this.clientCourseIssueLegalCapacityOrdinance.SetEditContextAsUnmodified();
                        }
                        finally
                        {
                            this.loading = false;
                        }

                        this.SpinnerHide();
                    }
                }
            }
            else
            {
                var cc = this.addedClientCourses.FirstOrDefault(x => x.IdClientCourse == this.clientCourseVM.IdClientCourse)!;
                var currentIdx = this.addedClientCourses.IndexOf(cc);
                this.nextId = currentIdx + 1;
                if (this.nextId < this.addedClientCourses.Count)
                {
                    this.SpinnerShow();

                    if (this.loading)
                    {
                        return;
                    }
                    try
                    {
                        this.loading = true;

                        this.RemoveEntityIdAsCurrentlyOpened(this.clientCourseVM.IdClientCourse, "TrainingClientCourse");

                        this.clientCourseVM = this.addedClientCourses[this.nextId];

                        this.SetDefaultNationalityValuesForClientCourseVM();

                        await this.SetIsDocumentPresentAsync();
                        if (!this.clientCourseVM.CourseJoinDate.HasValue)
                        {
                            if (this.courseVM.StartDate.HasValue)
                            {
                                this.clientCourseVM.CourseJoinDate = this.courseVM.StartDate.Value;
                            }
                            else
                            {
                                this.clientCourseVM.CourseJoinDate = DateTime.Now;
                            }
                        }

                        await this.LoadEducationTabAsync();

                        await this.LoadLegalCapacityOrdinanceTabAsync();

                        this.HandleIdentType();

                        await this.SetCreateAndModifyInfoAsync();

                        this.SetButtonsState();

                        await this.SetConcurrencyInfoAsync();

                        this.clientCourseVM.IdCityOfBirth = await this.TrainingService.GetTrainingClientCourseIdCityOfBirthByIdAsync(this.clientCourseVM.IdClientCourse);
                        if (this.clientCourseVM.IdCityOfBirth != null)
                        {
                            this.clientCourseInformation.locationSource.Clear();
                            LocationVM location = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.clientCourseVM.IdCityOfBirth.Value);
                            this.clientCourseInformation.locationSource.Add(location);
                        }

                        this.editContext = new EditContext(this.clientCourseVM);
                    }
                    finally
                    {
                        this.loading = false;
                    }

                    this.SpinnerHide();
                }
            }
        }

        private async void PreviousClient()
        {
            if (this.IsContextModified)
            {
                string msg = "Имате незапазени промени! Сигурни ли сте, че искате да смените курсиста?";
                bool confirmed = await this.ShowConfirmDialogAsync(msg);
                if (confirmed)
                {
                    var cc = this.addedClientCourses.FirstOrDefault(x => x.IdClientCourse == this.clientCourseVM.IdClientCourse)!;
                    var currentIdx = this.addedClientCourses.IndexOf(cc);
                    this.previousId = currentIdx - 1;
                    if (this.previousId >= 0)
                    {
                        this.SpinnerShow();

                        if (this.loading)
                        {
                            return;
                        }
                        try
                        {
                            this.loading = true;

                            this.RemoveEntityIdAsCurrentlyOpened(this.clientCourseVM.IdClientCourse, "TrainingClientCourse");

                            this.clientCourseVM = this.addedClientCourses[this.previousId];

                            this.SetDefaultNationalityValuesForClientCourseVM();

                            await this.SetIsDocumentPresentAsync();
                            if (!this.clientCourseVM.CourseJoinDate.HasValue)
                            {
                                if (this.courseVM.StartDate.HasValue)
                                {
                                    this.clientCourseVM.CourseJoinDate = this.courseVM.StartDate.Value;
                                }
                                else
                                {
                                    this.clientCourseVM.CourseJoinDate = DateTime.Now;
                                }
                            }

                            await this.LoadLegalCapacityOrdinanceTabAsync();

                            await this.LoadEducationTabAsync();

                            this.HandleIdentType();

                            await this.SetCreateAndModifyInfoAsync();

                            this.SetButtonsState();

                            await this.SetConcurrencyInfoAsync();

                            this.clientCourseVM.IdCityOfBirth = await this.TrainingService.GetTrainingClientCourseIdCityOfBirthByIdAsync(this.clientCourseVM.IdClientCourse);
                            if (this.clientCourseVM.IdCityOfBirth != null)
                            {
                                this.clientCourseInformation.locationSource.Clear();
                                LocationVM location = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.clientCourseVM.IdCityOfBirth.Value);
                                this.clientCourseInformation.locationSource.Add(location);
                            }

                            this.editContext = new EditContext(this.clientCourseVM);

                            this.clientCourseInformation.SetEditContextAsUnmodified();
                            this.clientCourseIssueLegalCapacityOrdinance.SetEditContextAsUnmodified();
                        }
                        finally
                        {
                            this.loading = false;
                        }

                        this.SpinnerHide();
                    }
                }
            }
            else
            {
                var cc = this.addedClientCourses.FirstOrDefault(x => x.IdClientCourse == this.clientCourseVM.IdClientCourse)!;
                var currentIdx = this.addedClientCourses.IndexOf(cc);
                if (currentIdx == -1)
                {
                    this.previousId = this.addedClientCourses.Count - 1;
                }
                else
                {
                    this.previousId = currentIdx - 1;
                }

                if (this.previousId >= 0)
                {
                    this.SpinnerShow();

                    if (this.loading)
                    {
                        return;
                    }
                    try
                    {
                        this.loading = true;

                        this.RemoveEntityIdAsCurrentlyOpened(this.clientCourseVM.IdClientCourse, "TrainingClientCourse");

                        this.clientCourseVM = this.addedClientCourses[this.previousId];

                        this.SetDefaultNationalityValuesForClientCourseVM();

                        await this.SetIsDocumentPresentAsync();
                        if (!this.clientCourseVM.CourseJoinDate.HasValue)
                        {
                            if (this.courseVM.StartDate.HasValue)
                            {
                                this.clientCourseVM.CourseJoinDate = this.courseVM.StartDate.Value;
                            }
                            else
                            {
                                this.clientCourseVM.CourseJoinDate = DateTime.Now;
                            }
                        }

                        await this.LoadEducationTabAsync();

                        await this.LoadLegalCapacityOrdinanceTabAsync();

                        this.HandleIdentType();

                        await this.SetCreateAndModifyInfoAsync();

                        this.SetButtonsState();

                        await this.SetConcurrencyInfoAsync();

                        this.clientCourseVM.IdCityOfBirth = await this.TrainingService.GetTrainingClientCourseIdCityOfBirthByIdAsync(this.clientCourseVM.IdClientCourse);
                        if (this.clientCourseVM.IdCityOfBirth != null)
                        {
                            this.clientCourseInformation.locationSource.Clear();
                            LocationVM location = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.clientCourseVM.IdCityOfBirth.Value);
                            this.clientCourseInformation.locationSource.Add(location);
                        }

                        this.editContext = new EditContext(this.clientCourseVM);
                    }
                    finally
                    {
                        this.loading = false;
                    }

                    this.SpinnerHide();
                }
            }
        }

        private void HandleIdentType()
        {
            if (this.clientCourseVM.IdIndentType.HasValue)
            {
                var ident = this.kvIndentTypeSource.FirstOrDefault(x => x.IdKeyValue == this.clientCourseVM.IdIndentType.Value);
                if (ident is not null)
                {
                    this.clientCourseInformation.identType = ident.Name;
                }
            }
        }

        private void SetButtonsState()
        {
            var cc = this.addedClientCourses.FirstOrDefault(x => x.IdClientCourse == this.clientCourseVM.IdClientCourse)!;
            var currentIdx = this.addedClientCourses.IndexOf(cc);
            this.nextId = currentIdx + 1;
            this.previousId = currentIdx - 1;

            if (this.nextId == this.addedClientCourses.Count)
            {
                this.disableNextBtn = true;
            }
            else
            {
                this.disableNextBtn = false;
            }

            if (this.previousId == -1)
            {
                this.disablePreviousBtn = true;
            }
            else
            {
                this.disablePreviousBtn = false;
            }
        }

        private async Task SetConcurrencyInfoAsync()
        {
            var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(this.clientCourseVM.IdClientCourse, "TrainingClientCourse");
            if (concurrencyInfoValue == null)
            {
                this.hideBtnsConcurrentModal = false;
                await this.AddEntityIdAsCurrentlyOpened(this.clientCourseVM.IdClientCourse, "TrainingClientCourse");
            }
            else if (concurrencyInfoValue != null && concurrencyInfoValue.IdPerson != this.UserProps.IdPerson)
            {
                this.hideBtnsConcurrentModal = true;
                this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfoValue);
            }
        }

        private void GetValidationModelFromClientCourseIssueLegalCapacityOrdinance(ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM model)
        {
            this.legalCapacityOrdinanceDataModel = model;
        }

        private async Task LoadEducationTabAsync()
        {
            if (this.selectedTab == 1)
            {
                await this.clientCourseEducation.LoadDataAsync();
            }
        }

        private async Task SaveProtocol2()
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
            string documentName = @"\CPOProtocols\3-80- с точки практика 1 2 и 3 СПК.docx";
            FileStream template = new FileStream($@"{resources_Folder}{documentName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();
            await FileUtils.SaveAs(JS, "test.docx", stream.ToArray());
        }

        private static WCharacterFormat SetMembersCharacterFormat(WordDocument document)
        {
            WCharacterFormat membersCharFormat = new WCharacterFormat(document);
            membersCharFormat.FontName = "Verdana";
            membersCharFormat.FontSize = (float)9.5;
            membersCharFormat.Position = 0;
            membersCharFormat.Italic = false;
            membersCharFormat.Bold = false;

            return membersCharFormat;
        }

        private static WCharacterFormat SetUnderLineCharactersFormat(WordDocument document)
        {
            WCharacterFormat membersCharFormat = new WCharacterFormat(document);
            membersCharFormat.FontName = "Verdana";
            membersCharFormat.FontSize = (float)6.5;
            membersCharFormat.Position = 0;
            membersCharFormat.Italic = true;
            membersCharFormat.Bold = false;

            return membersCharFormat;
        }

        private async Task LoadLegalCapacityOrdinanceTabAsync()
        {
            if (this.selectedTab == 2)
            {
                await this.clientCourseIssueLegalCapacityOrdinance.LoadModelDataAsync();
            }
        }

        private async Task SetIsDocumentPresentAsync()
        {
            this.isDocumentPresent = await this.TrainingService.IsDocumentPresentAsync(this.clientCourseVM.IdClientCourse);
        }
    }
}
