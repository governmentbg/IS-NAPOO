using System;
using Data.Models.Data.Common;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Registers.RegisterCertificatesValidation
{
    public partial class RegisterCertificatesValidationModal : BlazorBaseComponent
    {
        SfDialog sfDialog;
        ClientCourseDocumentVM model = new ClientCourseDocumentVM();

        [Inject]
        IDataSourceService dataSourceService { get; set; }
        [Inject]
        ITrainingService trainingService { get; set; }
        [Inject]
        IUploadFileService uploadFileService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        private string modalHeaderDoc = string.Empty;
        private string modalHeaderName = string.Empty;
        private string header1 = string.Empty;
        private string header2 = string.Empty;
        private string header3 = string.Empty;
        private string header4 = string.Empty;
        private string timeSpan = string.Empty;

        private KeyValueVM sex = new KeyValueVM();
        private KeyValueVM nationality = new KeyValueVM();
        private KeyValueVM typeEducation = new KeyValueVM();
        private KeyValueVM typeDocument = new KeyValueVM();

        public async Task openModal(ClientCourseDocumentVM clientCourseDocumentVM)
        {
            this.model = clientCourseDocumentVM;

            header1 = $"Информация за ЦПО: {model.ClientCourse.Client.CandidateProvider.ProviderOwner}";
            header2 = $"Информация за лицето, на което е издаден документа за ПК: {model.ClientCourse.Client.FirstName} {model.ClientCourse.Client.FamilyName}";
            header3 = $"Информация за проведен курс: {model.ClientCourse.Course.CourseName}";
            header4 = $"Информация за документа за ПК: {model.DocumentRegNo}";
            if (model.DocumentDate != null)
                modalHeaderDoc = $"{model.DocumentRegNo}/{model.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT)}";
            else
                modalHeaderDoc = $"{model.DocumentRegNo}/";
            modalHeaderName = $"{model.ClientCourse.Client.FirstName} {model.ClientCourse.Client.FamilyName}";

            timeSpan = $"{model.ClientCourse.Course.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)}-{model.ClientCourse.Course.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)}";

            sex = await dataSourceService.GetKeyValueByIdAsync(model.ClientCourse.Client.IdSex);

            nationality = await dataSourceService.GetKeyValueByIdAsync(model.ClientCourse.Client.IdNationality);

            typeEducation = await dataSourceService.GetKeyValueByIdAsync(model.ClientCourse.Course.Program.IdCourseType);

            typeDocument = await dataSourceService.GetKeyValueByIdAsync(model.IdDocumentType);

            this.isVisible = true;

            this.StateHasChanged();
        }

        public async Task downloadDocuments()
        {
            this.SpinnerShow();

            if (loading) return;

            try
            {
                loading = true;
                List<CourseDocumentUploadedFileVM> docs = trainingService.getCourseDocumentUploadedFile(model.IdClientCourseDocument);

                foreach (var doc in docs)
                {
                    var hasFile = await this.uploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(doc.IdCourseDocumentUploadedFile);
                    if (hasFile)
                    {
                        var memoryStream = await this.uploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(doc.IdCourseDocumentUploadedFile);

                        if (!string.IsNullOrEmpty(memoryStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, memoryStream.FileNameFromOldIS, memoryStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, $"Certificate_{model.DocumentRegNo}.pdf", memoryStream.MS!.ToArray());
                        }
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                        await this.ShowErrorAsync(msg);
                    }
                   

                }
            }
            finally
            {
                loading = false;
            }

            this.SpinnerHide();
        }
    }
}
