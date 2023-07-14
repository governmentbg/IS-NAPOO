using Data.Models.Data.DOC;
using Data.Models.Data.Training;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ISNAPOO.WebSystem.Pages.Registers.DocumentsFromCPO
{
    public partial class DocumentsFromCPOModal : BlazorBaseComponent
    {
        private DocumentsFromCPORegisterVM model = new DocumentsFromCPORegisterVM();
        private string thirdHeader = string.Empty;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(DocumentsFromCPORegisterVM model)
        {
            this.editContext = new EditContext(this.model);

            this.model = model;
            await this.LoadDataAsync();
            
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task LoadDataAsync()
        {
            this.thirdHeader = this.model.IsCourse
                ? $"Информация за проведен курс: <span style=\"color: #01a9ac !important;\">{this.model.CourseName}</span>"
                : $"Информация за проведена процедура по валидиране:";

            if (this.model.IdSex.HasValue)
            {
                this.model.SexValue = (await this.DataSourceService.GetKeyValueByIdAsync(this.model.IdSex.Value)).Name;
            }

            if (this.model.IdNationality.HasValue)
            {
                this.model.NationalityValue = (await this.DataSourceService.GetKeyValueByIdAsync(this.model.IdNationality.Value)).Name;
            }

            if (this.model.IdEducation.HasValue)
            {
                this.model.EducationValue = (await this.DataSourceService.GetKeyValueByIdAsync(this.model.IdEducation.Value)).Name;
            }

            if (this.model.IdDocumentType.HasValue)
            {
                this.model.DocumentTypeName = (await this.DataSourceService.GetKeyValueByIdAsync(this.model.IdDocumentType.Value)).Name;
            }
        }

        public async Task DownloadDocumentFileBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.model.IsCourse)
                {
                    var clientCourseDocFromDb = await this.TrainingService.GetClientCourseDocumentWithUploadedFilesByIdAsync(this.model.IdEntity);
                    if (clientCourseDocFromDb.OldId.HasValue)
                    {
                        if (!clientCourseDocFromDb.CourseDocumentUploadedFiles.Any())
                        {
                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                            await this.ShowErrorAsync(msg);
                        }
                        else
                        {
                            var docs = clientCourseDocFromDb.CourseDocumentUploadedFiles;
                            if (docs.Any())
                            {
                                foreach (var doc in docs)
                                {
                                    if (!string.IsNullOrEmpty(doc.UploadedFileName))
                                    {
                                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(doc.IdCourseDocumentUploadedFile);
                                        if (hasFile)
                                        {
                                            var document = await this.UploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(doc.IdCourseDocumentUploadedFile);
                                            if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                            {
                                                await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                            }
                                            else
                                            {
                                                await FileUtils.SaveAs(this.JsRuntime, doc.FileName, document.MS!.ToArray());
                                            }
                                        }
                                        else
                                        {
                                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                            await this.ShowErrorAsync(msg);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await this.ShowErrorAsync(msg);
                            }
                        }
                    }
                    else
                    {
                        var uploadedFile = clientCourseDocFromDb.CourseDocumentUploadedFiles.FirstOrDefault();
                        if (uploadedFile is not null)
                        {
                            if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                            {
                                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(uploadedFile.IdCourseDocumentUploadedFile);
                                if (hasFile)
                                {
                                    var document = await this.UploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(uploadedFile.IdCourseDocumentUploadedFile);
                                    if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                    {
                                        await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                    }
                                    else
                                    {
                                        await FileUtils.SaveAs(this.JsRuntime, uploadedFile.FileName, document.MS!.ToArray());
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
                        else
                        {
                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                            await this.ShowErrorAsync(msg);
                        }
                    }
                }
                else
                {
                    var validationClientDocFromDb = await this.TrainingService.GetValidationClientDocumentWithUploadedFilesByIdAsync(this.model.IdEntity);
                    if (validationClientDocFromDb.OldId.HasValue)
                    {
                        if (!validationClientDocFromDb.ValidationDocumentUploadedFiles.Any())
                        {
                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                            await this.ShowErrorAsync(msg);
                        }
                        else
                        {
                            var docs = validationClientDocFromDb.ValidationDocumentUploadedFiles;
                            if (docs.Any())
                            {
                                foreach (var doc in docs)
                                {
                                    if (!string.IsNullOrEmpty(doc.UploadedFileName))
                                    {
                                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationDocumentUploadedFile>(doc.IdValidationDocumentUploadedFile);
                                        if (hasFile)
                                        {
                                            var document = await this.UploadFileService.GetUploadedFileAsync<ValidationDocumentUploadedFile>(doc.IdValidationDocumentUploadedFile);
                                            if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                            {
                                                await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                            }
                                            else
                                            {
                                                await FileUtils.SaveAs(this.JsRuntime, doc.FileName, document.MS!.ToArray());
                                            }
                                        }
                                        else
                                        {
                                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                            await this.ShowErrorAsync(msg);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await this.ShowErrorAsync(msg);
                            }
                        }
                    }
                    else
                    {
                        var uploadedFile = validationClientDocFromDb.ValidationDocumentUploadedFiles.FirstOrDefault();
                        if (uploadedFile is not null)
                        {
                            if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                            {
                                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationDocumentUploadedFile>(uploadedFile.IdValidationDocumentUploadedFile);
                                if (hasFile)
                                {
                                    var document = await this.UploadFileService.GetUploadedFileAsync<ValidationDocumentUploadedFile>(uploadedFile.IdValidationDocumentUploadedFile);
                                    if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                    {
                                        await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                    }
                                    else
                                    {
                                        await FileUtils.SaveAs(this.JsRuntime, uploadedFile.FileName, document.MS!.ToArray());
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
                                var msg = LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await ShowErrorAsync(msg);
                            }
                        }
                        else
                        {
                            var msg = LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                            await ShowErrorAsync(msg);
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
    }
}

