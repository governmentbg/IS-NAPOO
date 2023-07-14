using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class ValidationProtocolModal : BlazorBaseComponent
    {
        private SfUploader uploader = new SfUploader();

        private ValidationProtocolVM validationProtocol = new ValidationProtocolVM();
        private ValidationProtocolGradeVM protocolGrade = new ValidationProtocolGradeVM();
        private ValidationProtocolGradeVM model = new ValidationProtocolGradeVM();
        private ValidationTestProtocols validationTestProtocols = new ValidationTestProtocols();
        private IEnumerable<KeyValueVM> validationProtocolTypeSource = new List<KeyValueVM>();
        private ValidationMessageStore? messageStore;
        private List<string> validationMessages = new List<string>();
        private List<int> validProtocolTypes = new List<int>();
        private string courseName = string.Empty;
        private KeyValueVM kvProtocol381 = new KeyValueVM();
        private KeyValueVM kvProtocol380p = new KeyValueVM();
        private KeyValueVM kvProtocol380t = new KeyValueVM();
        private IEnumerable<ValidationCommissionMemberVM> validationCommissionMemberSource = new List<ValidationCommissionMemberVM>();
        private bool showDeleteConfirmDialog;
        private string fileNameForDeletion;

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.validationProtocol);
        }

        public async Task OpenModal(ValidationProtocolVM validationProtocol)
        {
            if (validationProtocol.IdValidationProtocol != 0)
                this.validationProtocol = await this.TrainingService.GetValidationProtocolByIdAsync(validationProtocol.IdValidationProtocol);
            else
                this.validationProtocol = validationProtocol;
            if (this.validationProtocol.ValidationProtocolGrades != null && this.validationProtocol.ValidationProtocolGrades.Any())
            {
                protocolGrade = this.validationProtocol.ValidationProtocolGrades.First();
                if(protocolGrade.Grade != null)
                protocolGrade.GradeAsStr = protocolGrade.Grade.ToString();
            }else
            {
                protocolGrade = new ValidationProtocolGradeVM();
            }
            this.protocolGrade.IdValidationProtocol = this.validationProtocol.IdValidationProtocol;

            this.validationProtocolTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType");
            this.kvProtocol381 = this.validationProtocolTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "3-81B");
            this.validationCommissionMemberSource = await this.TrainingService.GetAllValidationCommissionChairmansByClient(this.validationProtocol.IdValidationClient);

            this.editContext = new EditContext(this.validationProtocol);
            this.validationMessages.Clear();

            this.SetCourseName();

            this.SetProtocolTypeName();

            await this.SetCreateAndModifyInfoAsync();

            this.SetValidProtocolTypes();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void SetCourseName()
        {
            if (this.validationProtocol.ValidationClient != null)
            {
                this.courseName = this.validationProtocol.ValidationClient.FullName;
            }
            else
            {
                this.courseName = string.Empty;
            }
        }

        private void OnProtocolSelected(ChangeEventArgs<int, KeyValueVM> args)
        {
            if (args is not null && args.ItemData is not null)
            {
                var protocolType = args.ItemData.KeyValueIntCode;
                if (protocolType == "3-80p")
                {
                    this.validationProtocol.ValidationProtocolDate = this.validationProtocol.ValidationClient.ExamPracticeDate;
                }
                else if (protocolType == "3-80t")
                {
                    this.validationProtocol.ValidationProtocolDate = this.validationProtocol.ValidationClient.ExamTheoryDate;
                }
            }
        }

        private void SetProtocolTypeName()
        {
            if (this.validationProtocol.IdValidationProtocolType != 0)
            {
                var type = this.validationProtocolTypeSource.FirstOrDefault(x => x.IdKeyValue == this.validationProtocol.IdValidationProtocolType);
                if (type is not null)
                {
                    this.validationProtocol.ValidationProtocolTypeName = type.Name;
                }
            }
        }

        private void SetValidProtocolTypes()
        {
            kvProtocol380t = this.validationProtocolTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "3-80t");
            kvProtocol380p = this.validationProtocolTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "3-80p");
            if (kvProtocol380p is not null && kvProtocol380t is not null && kvProtocol381 is not null)
            {
                this.validProtocolTypes.Add(kvProtocol380p.IdKeyValue);
                this.validProtocolTypes.Add(kvProtocol380t.IdKeyValue);
                this.validProtocolTypes.Add(kvProtocol381.IdKeyValue);
            };
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;

            var result = await this.UploadFileService.UploadFileAsync<ValidationProtocol>(file, args.Files[0].FileInfo.Name, "ValidationProtocol", this.validationProtocol.IdValidationProtocol);
            if (!string.IsNullOrEmpty(result))
            {
                this.validationProtocol.UploadedFileName = result;
            }

            await this.CallbackAfterSubmit.InvokeAsync();

            await this.uploader.ClearAllAsync();

            this.StateHasChanged();
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.validationProtocol.UploadedFileName))
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationProtocol>(this.validationProtocol.IdValidationProtocol);
                    if (result == 1)
                    {
                        this.validationProtocol.UploadedFileName = null;
                    }

                    await this.CallbackAfterSubmit.InvokeAsync();

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.validationProtocol.UploadedFileName))
            {
                this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
                this.fileNameForDeletion = fileName;
                this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
            }
        }

        public async void ConfirmDeleteCallback()
        {

            var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationProtocol>(this.validationProtocol.IdValidationProtocol);
            if (result == 1)
            {
                this.validationProtocol.UploadedFileName = null;

            }
            await this.CallbackAfterSubmit.InvokeAsync();
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

                if (!string.IsNullOrEmpty(this.validationProtocol.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationProtocol>(this.validationProtocol.IdValidationProtocol);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<ValidationProtocol>(this.validationProtocol.IdValidationProtocol);
                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.validationProtocol.FileName, documentStream.MS!.ToArray());
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

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.validationProtocol.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.validationProtocol.IdModifyUser);
            this.validationProtocol.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.validationProtocol.IdCreateUser);
        }

        private async Task SubmitBtn(bool showToast)
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

                if(!string.IsNullOrEmpty(protocolGrade.GradeAsStr))
                await SaveGradeAsync(protocolGrade);

                this.editContext = new EditContext(this.validationProtocol);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateChairmanSelected;
                if (this.editContext.Validate() && !validationMessages.Any())
                {
                    var result = new ResultContext<ValidationProtocolVM>();
                    result.ResultContextObject = this.validationProtocol;
                    if (this.validationProtocol.IdValidationProtocol != 0)
                    {
                        result = await this.TrainingService.UpdateValidationProtocolAsync(result);
                    }
                    else
                    {
                        result = await this.TrainingService.CreateValidationProtocolAsync(result);
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.SetCourseName();

                        this.SetProtocolTypeName();

                        this.validationProtocol = result.ResultContextObject;
                        this.protocolGrade = this.validationProtocol.ValidationProtocolGrades.First();

                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                }
                else
                {
                    this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        private async Task SaveGradeAsync(ValidationProtocolGradeVM model)
        {
                try
                {
                    this.loading = true;

                    if (!string.IsNullOrEmpty(model.GradeAsStr))
                    {
                        this.model.GradeAsStr = model.GradeAsStr;

                        if (!this.IsGradeValid())
                        {
                            await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                        }
                        else
                        {
                            model.Grade = BaseHelper.ConvertToFloat(model.GradeAsStr, 2);
                            model.GradeAsStr = model.Grade.Value.ToString("f2");
                            model.Grade = Math.Round(model.Grade.Value, 2, MidpointRounding.AwayFromZero);

                            var result = new ResultContext<ValidationProtocolGradeVM>();
                            result.ResultContextObject = model;

                            result = await this.TrainingService.UpdateValidationProtocolGradeAsync(result);

                    }
                    }
                }
                finally
                {
                    //this.loading = false;
                }
          
        }
        private bool IsGradeValid()
        {
            this.validationMessages.Clear();

            if (!string.IsNullOrEmpty(this.model.GradeAsStr))
            {
                if (BaseHelper.ConvertToFloat(this.model.GradeAsStr, 2) == null)
                {
                    this.validationMessages.Add("Полето 'Оценка' може да бъде само число!");
                    return false;
                }
            }

            var value = BaseHelper.ConvertToFloat(this.model.GradeAsStr, 2);
            if (value < 2)
            {
                this.validationMessages.Add("Полето 'Оценка' не може да има стойност по-малка от 2.00!");
                return false;
            }

            if (value > 6)
            {
                this.validationMessages.Add("Полето 'Оценка' не може да има стойност по-голяма от 6.00!");
                return false;
            }

            if (value.ToString().Length > 4)
            {
                this.validationMessages.Add("Полето 'Оценка' не може да съдържа повече от 2 знака след десетичната запетая!");
                return false;
            }

            return true;
        }

        private void ValidateChairmanSelected(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.validationProtocol.IdValidationProtocolType == this.kvProtocol381.IdKeyValue && this.validationProtocol.IdValidationCommissionMember is null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.validationProtocol, "IdCourseCommissionMember");
                this.messageStore?.Add(fi, "Полето 'Председател на изпитна комисия' е задължително!");
            }
        }
    }
}
