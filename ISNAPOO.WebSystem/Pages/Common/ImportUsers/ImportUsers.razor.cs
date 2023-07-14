using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using ISNAPOO.Core.HelperClasses;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Common.ImportUsers
{
    public partial class ImportUsers : BlazorBaseComponent
    {
        private SfUploader sfUpload = new SfUploader();

        private ImportUsersVM model = new ImportUsersVM();
        private IEnumerable<KeyValueVM> importUsersTypeSource = new List<KeyValueVM>();
        private IEnumerable<ApplicationRoleVM> rolesSource = new List<ApplicationRoleVM>();
        private ValidationMessageStore? messageStore;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IImportUserService ImportUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);

            if (await this.IsInRole("ADMIN"))
            {
                this.importUsersTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ImportUsersType");
                this.rolesSource = (await this.ApplicationUserService.GetAllApplicationRoleAsync(new ApplicationRoleVM())).OrderBy(x => x.RoleName).ToList();

                this.isVisible = true;
                this.StateHasChanged();
            }
        }

        private void OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                this.model.UploadedFileName = args.Files[0].FileInfo.Name;
                this.model.UploadedFileStream = args.Files[0].Stream;
                this.StateHasChanged();
            }
        }

        private async Task ImportBtn()
        {
            bool isFileUploaded = !string.IsNullOrEmpty(this.model.UploadedFileName) && this.model.UploadedFileName != "#";
            if (!isFileUploaded)
            {
                await this.ShowErrorAsync("Моля, качете файл преди да импортирате!");
                return;
            }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateRolesAdded;

                if (this.editContext.Validate())
                {
                    var resultContext = await this.ImportUserService.ImportUsersAsync(this.model.UploadedFileStream, this.model.UploadedFileName, this.model.IdImportType!.Value);
                    await this.sfUpload.ClearAllAsync();
                    if (resultContext.HasErrorMessages)
                    {
                        this.model.UploadedFileStream = this.ImportUserService.CreateExcelWithErrors(resultContext);
                        await this.JsRuntime.SaveAs($"Errors_ImportUsers_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", this.model.UploadedFileStream.ToArray());

                        await this.ShowErrorAsync("Неуспешен импорт! Файлът, който се опитвате да импортирате не отговаря на изискванията на шаблона!");
                    }
                    else
                    {
                        var result = await this.ImportUserService.CreateUsersAsync(resultContext.ResultContextObject, this.model);
                        if (result.IsSuccessfull)
                        {
                            await this.ShowSuccessAsync("Импортът приключи успешно!");
                            await this.JsRuntime.SaveAs($"Danni_import_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", result.MS.ToArray());

                            this.isVisible = false;
                            this.StateHasChanged();
                        }
                        else
                        {
                            await this.ShowErrorAsync("Грешка при запис в базата данни!");
                        }
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void ValidateRolesAdded(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (!this.model.Roles.Any())
            {
                FieldIdentifier fi = new FieldIdentifier(this.model, "Roles");
                this.messageStore?.Add(fi, "Полето 'Роли' е задължително!");
            }
        }
    }
}
