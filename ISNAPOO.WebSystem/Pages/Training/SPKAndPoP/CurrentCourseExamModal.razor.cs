using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Compression.Zip;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CurrentCourseExamModal : BlazorBaseComponent
    {
        private SfGrid<CourseCommissionMemberVM> membersGrid = new SfGrid<CourseCommissionMemberVM>();

        private CourseCommissionMemberVM model = new CourseCommissionMemberVM();
        private List<CourseCommissionMemberVM> membersSource = new List<CourseCommissionMemberVM>();

        private CourseVM CourseVM = new CourseVM();

        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Parameter]
        public EventCallback CallbackAfterInvalidEditContext { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.isVisible = false;
            this.editContext = new EditContext(this.model);
            this.editContext.MarkAsUnmodified();
        }

        public async Task OpenModal(CourseVM courseVM)
        {
            this.CourseVM = new CourseVM();
            this.CourseVM = courseVM;
            this.membersSource = (await this.TrainingService.GetAllCourseCommissionMembersByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task AddMemberBtn()
        {
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

                if (this.editContext.Validate())
                {
                    this.model.FirstName = this.model.FirstName.Trim();
                    this.model.SecondName = this.model.SecondName!.Trim();
                    this.model.FamilyName = this.model.FamilyName.Trim();
                    this.model.IdCourse = this.CourseVM.IdCourse;

                    if (this.membersSource.Any(x => x.FirstName.ToLower() == this.model.FirstName.ToLower() && x.SecondName!.ToLower() == this.model.SecondName.ToLower() && x.FamilyName.ToLower() == this.model.FamilyName.ToLower()))
                    {
                        await this.ShowErrorAsync("Вече има добавен член на изпитна комисия със същото име, презиме и фамилия!");
                        return;
                    }

                    var result = new ResultContext<CourseCommissionMemberVM>();
                    result.ResultContextObject = this.model;
                    result = await this.TrainingService.CreateCourseCommissionMemberAsync(result);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.model = new CourseCommissionMemberVM();

                        this.membersSource = (await this.TrainingService.GetAllCourseCommissionMembersByIdCourseAsync(this.CourseVM.IdCourse)).ToList();

                        this.editContext = new EditContext(this.model);

                        await this.CallbackAfterInvalidEditContext.InvokeAsync();

                        await this.membersGrid.Refresh();
                        this.StateHasChanged();
                    }
                }
                else
                {
                    await this.CallbackAfterInvalidEditContext.InvokeAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteMemberBtn(CourseCommissionMemberVM member)
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

                    if (member.IsChairman)
                    {
                        var isChairmanAlreadyInProtocolAdded = await this.TrainingService.IsChairmanAlreadyInProtocolAddedAsync(member.IdCourseCommissionMember, member.IdCourse);
                        if (isChairmanAlreadyInProtocolAdded)
                        {
                            await this.ShowErrorAsync("Не можете да изтриете председателя от списъка! Вече има генериран протокол с избрания председател!");
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }
                    }

                    var result = await this.TrainingService.DeleteCourseCommissionMemberByIdAsync(member.IdCourseCommissionMember);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.membersSource = (await this.TrainingService.GetAllCourseCommissionMembersByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
                        await this.membersGrid.Refresh();
                        this.StateHasChanged();
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
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
                ExportProperties.FileName = $"CourseCommissionMemberList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.membersGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"CourseCommissionMemberList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.membersGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "IsChairmanAsStr", HeaderText = "Председател", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
