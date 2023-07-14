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
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationClientCommisionModal : BlazorBaseComponent
    {
        [Parameter]
        public ValidationClientVM ClientVM { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = true;

        private SfGrid<ValidationCommissionMemberVM> sfGrid;

        private ValidationCommissionMemberVM model = new ValidationCommissionMemberVM();
        private List<ValidationCommissionMemberVM> membersSource = new List<ValidationCommissionMemberVM>();
        public List<string> validationMessages = new List<string>();
        private ValidationCommissionMemberVM memberToDelete = new ValidationCommissionMemberVM();
        [Parameter]
        public EventCallback<List<string>> CallbackAfterSubmit { get; set; }
        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);

            this.membersSource = (await TrainingService.GetAllValidationCommissionMembersByClient(ClientVM.IdValidationClient)).ToList();

        }

        private async Task AddMemberBtn()
        {
            this.validationMessages.Clear();
            SpinnerShow();

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
                    this.model.IdValidationClient = ClientVM.IdValidationClient;

                    if (this.membersSource.Any(x => x.FirstName.ToLower() == this.model.FirstName.ToLower() && x.SecondName!.ToLower() == this.model.SecondName.ToLower() && x.FamilyName.ToLower() == this.model.FamilyName.ToLower()))
                    {
                        await this.ShowErrorAsync("Вече има добавен член на изпитна комисия със същото име, презиме и фамилия!");
                        return;
                    }

                    var result = new ResultContext<ValidationCommissionMemberVM>();
                    result.ResultContextObject = this.model;
                    result = await TrainingService.CreateValidationCommissionMemberAsync(result);
                    if (result.HasErrorMessages)
                    {
                        await ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.model = new ValidationCommissionMemberVM();

                        this.membersSource = (await TrainingService.GetAllValidationCommissionMembersByClient(ClientVM.IdValidationClient)).ToList();
                        await this.sfGrid.Refresh();
                        StateHasChanged();
                    }
                }
                else
                {
                    await CallbackAfterSubmit.InvokeAsync(this.editContext.GetValidationMessages().ToList());
                }

            }
            finally
            {
                this.loading = false;
            }

            SpinnerHide();
        }

        private async Task DeleteMemberBtn(ValidationCommissionMemberVM member)
        {
            SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
               
                this.memberToDelete = member;

                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
                if (isConfirmed)
                {
                    var IsDeleted = await TrainingService.DeleteValidationClientCommissionMemberByIdAsync(this.memberToDelete.IdValidationCommissionMember);

                    this.membersSource = (await TrainingService.GetAllValidationCommissionMembersByClient(ClientVM.IdValidationClient)).ToList();
                    await this.sfGrid.Refresh();
                    StateHasChanged();

                    if (!IsDeleted)
                        await ShowErrorAsync("Не можете да изтриете председателя от списъка! Вече има генериран протокол с избрания председател!");
                }
            }
            catch (Exception)
            {
                await ShowErrorAsync("Грешка при изтриване от базата данни! ");
            }
            finally
            {
                this.loading = false;
            }

            SpinnerHide();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();

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

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"CourseCommissionMemberList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
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
