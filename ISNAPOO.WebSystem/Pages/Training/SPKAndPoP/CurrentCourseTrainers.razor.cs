using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.DocIO.DLS;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CurrentCourseTrainers : BlazorBaseComponent
    {
        private SfGrid<TrainerCourseVM> trainersGrid = new SfGrid<TrainerCourseVM>();
        private SelectCourseTrainerModal selectCourseTrainerModal = new SelectCourseTrainerModal();

        private List<TrainerCourseVM> trainersSource = new List<TrainerCourseVM>();
        private TrainerCourseVM trainerCourseToDelete = new TrainerCourseVM();

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.trainersSource = (await this.TrainingService.GetAllTrainerCoursesByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
        }

        private async Task AddTrainerBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.selectCourseTrainerModal.OpenModal(this.CourseVM, this.trainersSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteTrainerBtn(TrainerCourseVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                
                this.trainerCourseToDelete = model;

                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
                if (isConfirmed)
                {
                   var result = new ResultContext<TrainerCourseVM>();
                    result.ResultContextObject = model;
                    result = await this.TrainingService.DeleteTrainerCourseAsync(result);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.trainersSource = (await this.TrainingService.GetAllTrainerCoursesByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
                        await this.trainersGrid.Refresh();
                        this.StateHasChanged();
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DownloadTrainerUploadedDocumentAsync(int idUploadedDocument)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var uploadedFile = await this.CandidateProviderService.GetCandidateProviderTrainerDocumentByIdAsync(new CandidateProviderTrainerDocumentVM() { IdCandidateProviderTrainerDocument = idUploadedDocument });
                if (uploadedFile is null || string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;
                    await this.ShowErrorAsync(msg);
                }
                else
                {
                    if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                    {
                        var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                        var fileFullName = settingResource + uploadedFile.UploadedFileName;
                        if (Directory.Exists(fileFullName))
                        {
                            var files = Directory.GetFiles(fileFullName);
                            files = files.Select(x => x.Split(($"\\{uploadedFile.IdCandidateProviderTrainerDocument}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                            var fileName = string.Join(Environment.NewLine, files);
                            var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderTrainerDocument>(uploadedFile.IdCandidateProviderTrainerDocument, fileName);
                            if (hasFile)
                            {
                                var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderTrainerDocument>(uploadedFile.IdCandidateProviderTrainerDocument, fileName);
                                if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                                {
                                    await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                                }
                                else
                                {
                                    await FileUtils.SaveAs(this.JsRuntime, fileName, documentStream.MS!.ToArray());
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
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterTrainerSelectAsync(Dictionary<int, List<CandidateProviderTrainerVM>> selectedTrainers)
        {
            this.SpinnerShow();

            var resultContext = new ResultContext<List<CandidateProviderTrainerVM>>();
            resultContext.ResultContextObject = selectedTrainers.FirstOrDefault().Value;

            resultContext = await this.TrainingService.CreateTrainingCourseTrainerByListCandidateProviderTrainerVMAsync(resultContext, this.CourseVM.IdCourse, selectedTrainers.FirstOrDefault().Key);
            if (resultContext.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                return;
            }

            await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

            this.trainersSource = (await this.TrainingService.GetAllTrainerCoursesByIdCourseAsync(this.CourseVM.IdCourse)).ToList();

            this.SpinnerHide();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = Syncfusion.Blazor.Grids.PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();

                PdfTheme Theme = new PdfTheme();
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                ExportProperties.FileName = $"CourseTrainersList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.trainersGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"CourseTrainersList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.trainersGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "TrainingTypeName", HeaderText = "Вид на провежданото обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.EducationValue", HeaderText = "ОКС", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.EducationSpecialityNotes", HeaderText = "Специалност по диплома", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
