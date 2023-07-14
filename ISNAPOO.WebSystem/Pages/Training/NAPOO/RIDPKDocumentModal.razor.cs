using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using ISNAPOO.WebSystem.Pages.Training.Validation;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.NAPOO
{
    public partial class RIDPKDocumentModal : BlazorBaseComponent
    {
        private SfGrid<RIDPKDocumentVM> documentsGrid = new SfGrid<RIDPKDocumentVM>();
        private SubmissionCommentModal submissionCommentModal = new SubmissionCommentModal();
        private DocumentStatusModal documentStatusModal = new DocumentStatusModal();
        private CurrentTrainingCourseModal currentTrainingCourseModal = new CurrentTrainingCourseModal();
        private TrainingValidationClientModal validationModal = new TrainingValidationClientModal();
        private CurrentCourseTrainingCurriculumUploadedFilesModal uploadedFilesModal = new CurrentCourseTrainingCurriculumUploadedFilesModal();

        private IEnumerable<RIDPKDocumentVM> documentsSource = new List<RIDPKDocumentVM>();
        private string title = string.Empty;
        private string type = string.Empty;
        private RIDPKVM ridpkVM = new RIDPKVM();
        private KeyValueVM kvSPKCourseValue = new KeyValueVM();
        private string toolTipText = string.Empty;
        private string btnText = string.Empty;
        private double totalHours = 0;
        private double compulsoryHours = 0;
        private double nonCompulsoryHours = 0;
        private double a1Percentage = 0;
        private double a2a3PracticePercentage = 0;
        private string durationMonths = string.Empty;

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public async Task OpenModal(string type, RIDPKVM ridpkVM)
        {
            this.ridpkVM = ridpkVM;
            this.type = type;

            this.toolTipText = this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE ? "Преглед на курса" : "Преглед на валидирането";
            this.btnText = this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE ? "Преглед на курс" : "Преглед на валидиране";

            await this.LoadDataAsync();

            this.SetTitle();

            this.CalculateCoursePeriodInMonths();

            await this.CalculateCurriculumHours();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task LoadDataAsync()
        {
            this.documentsSource = await this.TrainingService.GetRIDPKDocumentsDataAsync(this.type, this.ridpkVM);
            this.StateHasChanged();
        }

        private void CalculateCoursePeriodInMonths()
        {
            var durationDays = this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE
                ? this.ridpkVM.Course.EndDate!.Value - this.ridpkVM.Course.StartDate!.Value
                : this.ridpkVM.ValidationClient.EndDate!.Value - this.ridpkVM.ValidationClient.StartDate!.Value;

            double months = Math.Round(durationDays.TotalDays / 30, 0);

            this.durationMonths = months == 1 ? "1 месец" : $"{months} месеца";
        }

        private void SetTitle()
        {
            this.title = this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE
                ? $"Данни за подадени документи за ПК от <span style=\"color: #ffffff;\">{this.ridpkVM.CandidateProvider.CPONameAndOwner}</span>"
                : $"Данни за подадени документи за ПК, свързани с валидиране, от <span style=\"color: #ffffff;\">{this.ridpkVM.CandidateProvider.CPONameAndOwner}</span>";
        }

        private async Task DownloadClientUploadedDocumentAsync(int idUploadedDocument)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var uploadedFile = await this.TrainingService.GetClientRequiredDocumentById(idUploadedDocument);
                if (uploadedFile is null || string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;
                    await this.ShowErrorAsync(msg);
                }
                else
                {
                    if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                    {
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ClientRequiredDocument>(idUploadedDocument);
                        if (hasFile)
                        {
                            var document = await this.UploadFileService.GetUploadedFileAsync<ClientRequiredDocument>(idUploadedDocument);
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
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DownloadProtocolUploadedDocumentAsync(int idProtocol)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var protocolDocument = await this.TrainingService.GetCourseProtocolWithoutIncludesByIdAsync(idProtocol);
                if (protocolDocument is not null && !string.IsNullOrEmpty(protocolDocument.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseProtocol>(idProtocol);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<CourseProtocol>(idProtocol);
                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, protocolDocument.FileName, document.MS!.ToArray());
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

        private async Task ApproveBtn()
        {
            var selectedRows = await this.documentsGrid.GetSelectedRecordsAsync();
            if (!selectedRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете поне един ред от списъка преди да одобрите!");
                return;
            }

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да одобрите избраните документи за ПК?");
            if (isConfirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    this.submissionCommentModal.OpenModal(GlobalConstants.RIDPK_OPERATION_APPROVE, null, selectedRows, this.type);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task ReturnBtn()
        {
            var selectedRows = await this.documentsGrid.GetSelectedRecordsAsync();
            if (!selectedRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете поне един ред от списъка преди да върнете към ЦПО!");
                return;
            }

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да върнете избраните документи за ПК към ЦПО?");
            if (isConfirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    foreach (var doc in selectedRows)
                    {
                        var isDocumentAlreadyReturned = await this.TrainingService.IsRIDPKDocumentAlreadyReturnedAsync(doc);
                        if (isDocumentAlreadyReturned)
                        {
                            await this.ShowErrorAsync($"Документ за ПК с фабричен номер '{doc.DocumentSerialNumber}' е връщан за корекция към ЦПО веднъж. Не можете да върнете същия документ за повторна корекция!");
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }
                    }

                    this.submissionCommentModal.OpenModal(GlobalConstants.RIDPK_OPERATION_RETURN, null, selectedRows, this.type);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task RejectBtn()
        {
            var selectedRows = await this.documentsGrid.GetSelectedRecordsAsync();
            if (!selectedRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете поне един ред от списъка преди да откажете публикуване в регистъра!");
                return;
            }

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да откажете за публикуване в регистъра избраните документи за ПК?");
            if (isConfirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    this.submissionCommentModal.OpenModal(GlobalConstants.RIDPK_OPERATION_REJECT, null, selectedRows, this.type);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task OpenCourseBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
                {
                    var courseFromDb = await this.TrainingService.GetTrainingCourseByIdAsync(this.ridpkVM.Course.IdCourse);

                    var clientCourseIds = this.documentsSource.Select(x => x.IdClientCourse).ToList();
                    await this.currentTrainingCourseModal.OpenModal(courseFromDb, null, false, clientCourseIds);
                }
                else
                {
                    var currentClient = await this.TrainingService.GetValidationClientByIdAsync(this.ridpkVM.ValidationClient.IdValidationClient);
                    this.kvSPKCourseValue = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfProfessionalQualifications");
                    this.validationModal.openModal(currentClient, kvSPKCourseValue.IdKeyValue, false, true);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task CalculateCurriculumHours()
        {
            this.totalHours = 0;
            this.a1Percentage = 0;
            this.compulsoryHours = 0;
            this.nonCompulsoryHours = 0;
            this.a2a3PracticePercentage = 0;

            double a1TotalHours = 0;
            double aTotalHours = 0;
            double specificProfessionTrainingHours = 0;
            double industryProfessionTrainingHours = 0;
            double extendedProfessionTrainingHours = 0;
            double a2a3PracticeHours = 0;

            if (this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
            {
                var curriculums = await this.TrainingService.GetTrainingCurriculumsWithoutAnythingIncludedByIdCourseAsync(this.ridpkVM.Course.IdCourse);
                foreach (var curriculum in curriculums)
                {
                    if (curriculum.ProfessionalTraining != "Б")
                    {
                        var practiceHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                        var theoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                        aTotalHours += (practiceHours + theoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "А1")
                    {
                        var a1PracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                        var a1TheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                        a1TotalHours += (a1PracticeHours + a1TheoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "А2")
                    {
                        double a2PracticeHours = 0;
                        double a2TheoryHours = 0;
                        if (curriculum.Practice.HasValue)
                        {
                            a2PracticeHours = curriculum.Practice.Value;
                        }
                        else
                        {
                            a2PracticeHours = 0;
                        }

                        if (curriculum.Theory.HasValue)
                        {
                            a2TheoryHours = curriculum.Theory.Value;
                        }
                        else
                        {
                            a2TheoryHours = 0;
                        }

                        a2a3PracticeHours += a2PracticeHours;
                        industryProfessionTrainingHours += (a2PracticeHours + a2TheoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "А3")
                    {
                        double a3PracticeHours = 0;
                        double a3TheoryHours = 0;
                        if (curriculum.Practice.HasValue)
                        {
                            a3PracticeHours = curriculum.Practice.Value;
                        }
                        else
                        {
                            a3PracticeHours = 0;
                        }

                        if (curriculum.Theory.HasValue)
                        {
                            a3TheoryHours = curriculum.Theory.Value;
                        }
                        else
                        {
                            a3TheoryHours = 0;
                        }

                        a2a3PracticeHours += a3PracticeHours;
                        specificProfessionTrainingHours += (a3PracticeHours + a3TheoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "Б")
                    {
                        var bPracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                        var bTheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                        extendedProfessionTrainingHours += (bPracticeHours + bTheoryHours);
                    }

                    if (curriculum.Theory.HasValue)
                    {
                        this.totalHours += curriculum.Theory.Value;
                    }

                    if (curriculum.Practice.HasValue)
                    {
                        this.totalHours += curriculum.Practice.Value;
                    }
                }
            }
            else
            {
                var curriculums = await this.TrainingService.GetValidationCurriculumsWithoutAnythingIncludedByIdCourseAsync(this.ridpkVM.ValidationClient.IdValidationClient);
                foreach (var curriculum in curriculums)
                {
                    if (curriculum.ProfessionalTraining != "Б")
                    {
                        var practiceHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                        var theoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                        aTotalHours += (practiceHours + theoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "А1")
                    {
                        var a1PracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                        var a1TheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                        a1TotalHours += (a1PracticeHours + a1TheoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "А2")
                    {
                        double a2PracticeHours = 0;
                        double a2TheoryHours = 0;
                        if (curriculum.Practice.HasValue)
                        {
                            a2PracticeHours = curriculum.Practice.Value;
                        }
                        else
                        {
                            a2PracticeHours = 0;
                        }

                        if (curriculum.Theory.HasValue)
                        {
                            a2TheoryHours = curriculum.Theory.Value;
                        }
                        else
                        {
                            a2TheoryHours = 0;
                        }

                        a2a3PracticeHours += a2PracticeHours;
                        industryProfessionTrainingHours += (a2PracticeHours + a2TheoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "А3")
                    {
                        double a3PracticeHours = 0;
                        double a3TheoryHours = 0;
                        if (curriculum.Practice.HasValue)
                        {
                            a3PracticeHours = curriculum.Practice.Value;
                        }
                        else
                        {
                            a3PracticeHours = 0;
                        }

                        if (curriculum.Theory.HasValue)
                        {
                            a3TheoryHours = curriculum.Theory.Value;
                        }
                        else
                        {
                            a3TheoryHours = 0;
                        }

                        a2a3PracticeHours += a3PracticeHours;
                        specificProfessionTrainingHours += (a3PracticeHours + a3TheoryHours);
                    }

                    if (curriculum.ProfessionalTraining == "Б")
                    {
                        var bPracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                        var bTheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                        extendedProfessionTrainingHours += (bPracticeHours + bTheoryHours);
                    }

                    if (curriculum.Theory.HasValue)
                    {
                        this.totalHours += curriculum.Theory.Value;
                    }

                    if (curriculum.Practice.HasValue)
                    {
                        this.totalHours += curriculum.Practice.Value;
                    }
                }
            }

            this.a1Percentage = (a1TotalHours / aTotalHours) * 100;
            this.a2a3PracticePercentage = (a2a3PracticeHours / (industryProfessionTrainingHours + specificProfessionTrainingHours)) * 100;
            this.nonCompulsoryHours = extendedProfessionTrainingHours;
            this.compulsoryHours = this.totalHours - this.nonCompulsoryHours;
        }

        private async Task UpdateDocumentsDataAfterDocumentSubmissionAsync()
        {
            await this.LoadDataAsync();

            if (!this.documentsSource.Any())
            {
                this.isVisible = false;
            }

            await this.CallbackAfterSubmit.InvokeAsync();
        }

        private async Task OpenFileWithDocumentBtn(RIDPKDocumentVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
                {
                    var uploadedFiles = await this.TrainingService.GetCourseDocumentUploadedFileByIdClientCourseDocumentAsync(model.IdEntity);
                    if (uploadedFiles.Any())
                    {
                        foreach (var doc in uploadedFiles)
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
                            else
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await this.ShowErrorAsync(msg);
                            }
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
                    var uploadedFiles = await this.TrainingService.GetValidationDocumentUploadedFileByIdValidationClientDocumentAsync(model.IdEntity);
                    if (uploadedFiles.Any())
                    {
                        foreach (var doc in uploadedFiles)
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
                            else
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await this.ShowErrorAsync(msg);
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
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenStatusHistoryBtn(RIDPKDocumentVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var courseDocType = this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE ? "Course" : "Validation";
                await this.documentStatusModal.OpenModal(model.IdEntity, courseDocType);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenCurriculumFilesModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
                {
                    await this.uploadedFilesModal.OpenModal(this.ridpkVM.Course);
                }
                else
                {
                    await this.uploadedFilesModal.OpenModal(null, this.ridpkVM.ValidationClient);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
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
                ExportProperties.FileName = $"DocumentList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.documentsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"DocumentList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.documentsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "DocumentRegNo", HeaderText = "Регистрационен номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentDate", HeaderText = "Дата", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentSerialNumber", HeaderText = "Фабричен номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentTypeName", HeaderText = "Вид на документа", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientFirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientSecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientFamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentStatusValue", HeaderText = "Статус", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
