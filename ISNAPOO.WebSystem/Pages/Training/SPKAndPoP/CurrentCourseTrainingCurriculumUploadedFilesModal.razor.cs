using Data.Models.Data.Candidate;
using Data.Models.Data.Training;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CurrentCourseTrainingCurriculumUploadedFilesModal : BlazorBaseComponent
    {
        private SfGrid<TrainingCurriculumUploadedFileVM> filesGrid = new SfGrid<TrainingCurriculumUploadedFileVM>();

        private CourseVM courseVM = new CourseVM();
        private ValidationClientVM validationClientVM = new ValidationClientVM();
        private List<TrainingCurriculumUploadedFileVM> filesSource = new List<TrainingCurriculumUploadedFileVM>();
        private KeyValueVM kvCourseStatusCurrent = new KeyValueVM();
        private string title = string.Empty;
        private bool hideDeleteBtn = false;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public async Task OpenModal(CourseVM course = null, ValidationClientVM validationClient = null)
        {
            this.courseVM = course;
            this.validationClientVM = validationClient;

            this.title = this.courseVM is not null
                ? $"Данни за прикачени файлове с уч. план и уч. програми към курс <span style=\"color: #ffffff\">{this.courseVM.CourseName}</span>"
                : $"Данни за прикачени файлове с уч. план и уч. програми за валидирано лице <span style=\"color: #ffffff\">{this.validationClientVM.FullName}</span>";

            await this.LoadDataAsync();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task LoadDataAsync()
        {
            if (this.courseVM is not null)
            {
                if (this.courseVM.OldId.HasValue)
                {
                     this.filesSource = (await this.TrainingService.GetTrainingCurriculumUploadedFilesForOldCoursesByIdCourseAsync(this.courseVM.IdCourse)).ToList();
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.courseVM.UploadedFileName))
                    {
                        this.filesSource = new List<TrainingCurriculumUploadedFileVM>()
                        {
                            new TrainingCurriculumUploadedFileVM()
                            {
                                IdGrid = 1,
                                UploadedFileName = this.courseVM.UploadedFileName,
                                IdEntity = this.courseVM.IdCourse
                            }
                        };
                    }
                }
            }
            else
            {
                if (this.validationClientVM.OldId.HasValue)
                {
                    this.filesSource = (await this.TrainingService.GetValidationClientCurriculumUploadedFilesForOldValidationClientsByIdValidationClientAsync(this.validationClientVM.IdValidationClient)).ToList();
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.validationClientVM.UploadedCurriculumFileName))
                    {
                        this.filesSource = new List<TrainingCurriculumUploadedFileVM>()
                        {
                            new TrainingCurriculumUploadedFileVM()
                            {
                                IdGrid = 1,
                                UploadedFileName = this.validationClientVM.UploadedCurriculumFileName,
                                IdEntity = this.validationClientVM.IdValidationClient
                            }
                        };
                    }
                }
            }

            this.kvCourseStatusCurrent = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusNow");

            if (!this.GetUserRoles().Any(x => x == "CPO"))
            {
                this.hideDeleteBtn = true;
            }
            else
            {
                this.hideDeleteBtn = this.courseVM is not null
                ? this.courseVM.IdStatus != this.kvCourseStatusCurrent.IdKeyValue
                : this.validationClientVM.IdStatus != this.kvCourseStatusCurrent.IdKeyValue;
            }
        }

        private async Task DownloadFileBtn(TrainingCurriculumUploadedFileVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.courseVM is not null)
                {
                    if (this.courseVM.OldId.HasValue)
                    {
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateCurriculumModification>(model.IdEntity);
                        if (hasFile)
                        {
                            var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateCurriculumModification>(model.IdEntity);

                            if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                            {
                                await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                            }
                            else
                            {
                                await FileUtils.SaveAs(this.JsRuntime, model.FileName, documentStream.MS!.ToArray());
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
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<Course>(model.IdEntity);
                        if (hasFile)
                        {
                            var documentStream = await this.UploadFileService.GetUploadedFileAsync<Course>(model.IdEntity);

                            if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                            {
                                await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                            }
                            else
                            {
                                await FileUtils.SaveAs(this.JsRuntime, model.FileName, documentStream.MS!.ToArray());
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
                    if (this.validationClientVM.OldId.HasValue)
                    {
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateCurriculumModification>(model.IdEntity);
                        if (hasFile)
                        {
                            var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateCurriculumModification>(model.IdEntity);

                            if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                            {
                                await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                            }
                            else
                            {
                                await FileUtils.SaveAs(this.JsRuntime, model.FileName, documentStream.MS!.ToArray());
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
                        var documentStream = await this.UploadFileService.GetValidationClientCurriculumUploadedFileAsync(model.IdEntity);

                        await FileUtils.SaveAs(this.JsRuntime, model.FileName, documentStream.ToArray());
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteUploadedFileBtn(TrainingCurriculumUploadedFileVM model)
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
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

                    if (this.courseVM is not null)
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAsync<Course>(model.IdEntity);
                        if (result == 1)
                        {
                            model.UploadedFileName = string.Empty;
                        }

                        this.courseVM.UploadedFileName = string.Empty;
                    }
                    else
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationClient>(model.IdEntity);
                        if (result == 1)
                        {
                            model.UploadedFileName = string.Empty;
                        }

                        this.validationClientVM.UploadedCurriculumFileName = null;
                    }

                    this.filesSource.RemoveAll(x => x.IdEntity == model.IdEntity);
                    await this.filesGrid.Refresh();

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
}
