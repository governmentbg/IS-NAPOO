using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Registers.VocationalQualificationValidationCertificates
{
    public partial class VocationalQualificationValidationCertificateListModal
    {
        SfDialog sfDialog;
        ValidationClientVM model = new ValidationClientVM();
        ValidationClientDocumentVM validationClientDocument = new ValidationClientDocumentVM();
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
        private string timeSpan = string.Empty;
        private IEnumerable<KeyValueVM> trainingCourseTypeSource;
        private KeyValueVM sex = new KeyValueVM();
        private KeyValueVM nationality = new KeyValueVM();
        private KeyValueVM kvProfessionalTrainingForAcquiringTheSPK;
        public async Task openModal(ValidationClientVM validationClientVM)
        {
            this.model = validationClientVM;
            this.timeSpan = validationClientVM.timeSpan;
            this.trainingCourseTypeSource = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram").Result;
            this.kvProfessionalTrainingForAcquiringTheSPK = this.trainingCourseTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfProfessionalQualifications");
            validationClientDocument =
                model.ValidationClientDocuments.FirstOrDefault(x => x.IdValidationClient == model.IdValidationClient);
            validationClientDocument.DocumentTypeName = validationClientVM.IdCourseType == this.kvProfessionalTrainingForAcquiringTheSPK.IdKeyValue ? "Свидетелство за валидиране на професионална квалификация" : "Удостоверение за валидиране на професионална квалификация по част от професия";
            header1 = model.CandidateProvider.ProviderOwner;
            header2 = model.Client.FirstName + " " + model.Client.FamilyName;
            header3 = model.ValidationClientDocuments.FirstOrDefault(x => x.IdValidationClient == model.IdValidationClient).DocumentRegNo;
            if (validationClientDocument.DocumentDate != null)
                modalHeaderDoc = $"{validationClientDocument.DocumentRegNo}/{validationClientDocument.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT)}";
            else
                modalHeaderDoc = $"{validationClientDocument.DocumentRegNo}/";
            modalHeaderName = $"{model.Client.FirstName} {model.Client.FamilyName}";

            sex = await dataSourceService.GetKeyValueByIdAsync(model.Client.IdSex);

            nationality = await dataSourceService.GetKeyValueByIdAsync(model.Client.IdNationality);

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
                var docs = await this.trainingService.GetValidationDocumentUploadedFilesByIdValidationClientAsync(model.IdClient);
                foreach (var doc in docs)
                {
                    var hasFile = await this.uploadFileService.CheckIfExistUploadedFileAsync<ValidationDocumentUploadedFile>(doc.IdValidationDocumentUploadedFile);
                    if (hasFile)
                    {
                        var memoryStream = await this.uploadFileService.GetUploadedFileAsync<ValidationDocumentUploadedFile>(doc.IdValidationDocumentUploadedFile);

                        if (!string.IsNullOrEmpty(memoryStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, memoryStream.FileNameFromOldIS, memoryStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, $"Certificate_{validationClientDocument.DocumentRegNo}.pdf", memoryStream.MS!.ToArray());
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
