using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.Contracts.Training;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.Contracts.Common;
using Microsoft.JSInterop;
using ISNAPOO.WebSystem.Resources;
using Syncfusion.PdfExport;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Training;
using Syncfusion.Blazor.Inputs;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;
using ISNAPOO.Common.Framework;
using Data.Models.Data.Training;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    partial class ClientCourseEducationModal : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback CallBackAfterSave { get; set; }

        [Parameter]
        public CourseVM CourseVM { get; set; }

        #region Inject
        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
        [Inject]
        public ITrainingService TrainingService { get; set; }
        [Inject]
        public IUploadFileService UploadFileService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public ILocService LocService { get; set; }
        #endregion

        private ClientRequiredDocumentVM model = new ClientRequiredDocumentVM();
        private SfUploader uploader = new SfUploader();
        private IEnumerable<KeyValueVM> requiredDocumentTypesSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> qualificationLevelsSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> formEducationsSource = new List<KeyValueVM>();
        private string CreationDateStr = string.Empty;
        private string ModifyDateStr = string.Empty;
        private List<int> filteredDocIds = new List<int>();
        private List<int> filteredDiplomaDocIds = new List<int>();
        private KeyValueVM kvProfesionalQualificationType = new KeyValueVM();
        private bool showDeleteConfirmDialog;
        private string fileNameForDeletion;

        protected override async Task OnInitializedAsync()
        {
            this.requiredDocumentTypesSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType")).Where(x => x.DefaultValue1 != null && x.DefaultValue3.Contains("CPO")).ToList();
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Report")!.IdKeyValue);
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "MedicalDocument")!.IdKeyValue);
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Other")!.IdKeyValue);
            this.filteredDiplomaDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "SecondarySchoolDiploma")!.IdKeyValue);
            this.filteredDiplomaDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "UniversityDiploma")!.IdKeyValue);
            this.kvProfesionalQualificationType = this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfesionalQualification");

            this.qualificationLevelsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MinimumQualificationLevel");
            if (this.CourseVM.Program is not null && this.CourseVM.Program.FrameworkProgram is not null)
            {
                if (this.CourseVM.Program.FrameworkProgram.Name.Contains("Е"))
                {
                    this.qualificationLevelsSource = this.qualificationLevelsSource.Where(q => q.DefaultValue3.Contains(this.CourseVM.Program.FrameworkProgram.Name));
                }
            }

            this.formEducationsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education");
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(ClientRequiredDocumentVM model)
        {
            this.model = model;
            if (model.IdClientRequiredDocument != 0)
            {
                this.CreationDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
            }
            else
            {
                this.CreationDateStr = string.Empty;
                this.ModifyDateStr = string.Empty;
            }
            this.editContext = new EditContext(this.model);
            this.isVisible = true;
            this.StateHasChanged();
        }
        private async Task SubmitAndContinueBtn()
        {
            await this.Save();

            if (!this.editContext.GetValidationMessages().Any())
            {
                var idClientCourse = this.model.IdClientCourse;
                var idCourse = this.model.IdCourse;

                this.model = new ClientRequiredDocumentVM() { IdClientCourse = idClientCourse, IdCourse = idCourse };
            }
        }
        public async Task Save()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();

                bool isValid = this.editContext.Validate();
                if (isValid)
                {
                    var result = new ResultContext<ClientRequiredDocumentVM>();
                    result.ResultContextObject = this.model;
                    if (result.ResultContextObject.IdClientRequiredDocument == 0)
                    {
                        result = await this.TrainingService.CreateClientRequiredDocumentAsync(result);
                    }
                    else
                    {
                        result = await this.TrainingService.UpdateClientRequiredDocumentAsync(result);
                    }
                    this.model = await this.TrainingService.GetClientRequiredDocumentById(result.ResultContextObject.IdClientRequiredDocument);

                    this.CreationDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                    this.ModifyDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                    this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                    this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                    }
                    await CallBackAfterSave.InvokeAsync();
                }
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }
        }
        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;

            var result = await this.UploadFileService.UploadFileAsync<ClientRequiredDocument>(file, args.Files[0].FileInfo.Name, "ClientCourseEducation", this.model.IdClientRequiredDocument);
            if (!string.IsNullOrEmpty(result))
            {
                this.model.UploadedFileName = result;
            }

            await CallBackAfterSave.InvokeAsync();

            await this.uploader.ClearAllAsync();

            this.StateHasChanged();
        }
        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ClientRequiredDocument>(this.model.IdClientRequiredDocument);
                    if (result == 1)
                    {
                        this.model.UploadedFileName = null;
                    }

                    await CallBackAfterSave.InvokeAsync();

                    this.StateHasChanged();
                }
            }
        }
        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
                this.fileNameForDeletion = fileName;
                this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
            }
        }
        public async void ConfirmDeleteCallback()
        {
            var result = await this.UploadFileService.RemoveFileByIdAsync<ClientRequiredDocument>(this.model.IdClientRequiredDocument);

            if (result == 1)
            {
                this.model.UploadedFileName = null;
            }
            await CallBackAfterSave.InvokeAsync();

            this.StateHasChanged();

            this.showDeleteConfirmDialog = false;

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

                if (!string.IsNullOrEmpty(this.model.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ClientRequiredDocument>(this.model.IdClientRequiredDocument);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<ClientRequiredDocument>(this.model.IdClientRequiredDocument);
                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.model.FileName, document.MS!.ToArray());
                        }
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                        await this.ShowErrorAsync(msg);
                    }
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

    }
}
