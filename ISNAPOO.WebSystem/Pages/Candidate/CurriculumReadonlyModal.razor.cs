using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CurriculumReadonlyModal : BlazorBaseComponent
    {
        private SfGrid<CandidateCurriculumVM> curriculumsGrid = new SfGrid<CandidateCurriculumVM>();

        private List<CandidateCurriculumVM> addedCurriculums = new List<CandidateCurriculumVM>();
        private double totalHours = 0;
        private double compulsoryHours = 0;
        private double nonCompulsoryHours = 0;
        private double generalProfessionTrainingHours = 0;
        private double industryProfessionTrainingHours = 0;
        private double specificProfessionTrainingHours = 0;
        private double extendedProfessionTrainingHours = 0;
        private double practiceHours = 0;
        private double theoryHours = 0;
        private double? totalPracticeHours = 0;
        private double? totalTheoryHours = 0;
        private FrameworkProgramVM frameworkProgramVM = new FrameworkProgramVM();
        private SpecialityVM specialityVM = new SpecialityVM();
        private string title = string.Empty;

        public void OpenModal(List<CandidateCurriculumVM> curriculumsSource, SpecialityVM speciality, FrameworkProgramVM frameworkProgram)
        {
            this.specialityVM = speciality;
            this.frameworkProgramVM = frameworkProgram;
            this.addedCurriculums = curriculumsSource.ToList();

            this.title = $"Данни за учебен план и учебни програми за специалност <span style=\"color: #ffffff;\">{this.specialityVM.CodeAndName}</span>";

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void CustomizeCellHours(QueryCellInfoEventArgs<CandidateCurriculumVM> args)
        {
            if (args.Column.Field == "Theory")
            {
                args.Cell.AddClass(new string[] { "cell-orange" });
            }

            if (args.Column.Field == "Practice")
            {
                args.Cell.AddClass(new string[] { "cell-bluegreen" });
            }
        }

        // пресмята общият брой часове за учебна програма
        private void CalculateCurriculumHours()
        {
            this.ResetHours();

            foreach (var curriculum in this.addedCurriculums)
            {
                if (curriculum.Theory.HasValue)
                {
                    this.theoryHours = curriculum.Theory.Value;
                    this.totalTheoryHours += curriculum.Theory.Value;
                }
                else
                {
                    this.theoryHours = 0;
                }

                if (curriculum.Practice.HasValue)
                {
                    this.totalPracticeHours += curriculum.Practice.Value;
                }

                if (curriculum.ProfessionalTraining != "Б")
                {
                    if (curriculum.Practice.HasValue)
                    {
                        this.practiceHours += curriculum.Practice.Value;
                    }
                    else
                    {
                        this.practiceHours += 0;
                    }
                }

                if (curriculum.ProfessionalTraining == "Б")
                {
                    var bPracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                    var bTheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                    this.extendedProfessionTrainingHours += (bPracticeHours + bTheoryHours);
                }

                if (curriculum.ProfessionalTraining == "А1")
                {
                    var a1PracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                    var a2TheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                    this.generalProfessionTrainingHours += (a1PracticeHours + a2TheoryHours);
                }

                if (curriculum.ProfessionalTraining == "А2")
                {
                    double a2PracticeHours = 0;
                    if (curriculum.Practice.HasValue)
                    {
                        a2PracticeHours = curriculum.Practice.Value;
                    }
                    else
                    {
                        a2PracticeHours = 0;
                    }

                    this.industryProfessionTrainingHours += (a2PracticeHours + this.theoryHours);
                }

                if (curriculum.ProfessionalTraining == "А3")
                {
                    double a3PracticeHours = 0;
                    if (curriculum.Practice.HasValue)
                    {
                        a3PracticeHours = curriculum.Practice.Value;
                    }
                    else
                    {
                        a3PracticeHours = 0;
                    }

                    this.specificProfessionTrainingHours += (a3PracticeHours + this.theoryHours);
                }
            }

            this.totalHours += this.extendedProfessionTrainingHours + this.generalProfessionTrainingHours + industryProfessionTrainingHours + specificProfessionTrainingHours;
            this.nonCompulsoryHours = this.extendedProfessionTrainingHours;
            this.compulsoryHours = this.totalHours - this.nonCompulsoryHours;
        }

        // ресетва бройката с часовете от учебната програма
        private void ResetHours()
        {
            this.totalHours = 0;
            this.compulsoryHours = 0;
            this.nonCompulsoryHours = 0;
            this.generalProfessionTrainingHours = 0;
            this.industryProfessionTrainingHours = 0;
            this.specificProfessionTrainingHours = 0;
            this.extendedProfessionTrainingHours = 0;
            this.practiceHours = 0;
            this.theoryHours = 0;
            this.totalTheoryHours = 0;
            this.totalPracticeHours = 0;
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                ExportProperties.IncludeTemplateColumn = true;

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
                ExportProperties.FileName = $"Curriculum_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.curriculumsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Curriculum_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.Columns = this.SetGridColumnsForExport();
                ExportProperties.IncludeTemplateColumn = true;
                await this.curriculumsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ProfessionalTraining", HeaderText = "Раздел", TextAlign = TextAlign.Center });
            ExportColumns.Add(new GridColumn() { Field = "Subject", HeaderText = "Предмет", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Topic", HeaderText = "Тема", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Theory", HeaderText = "Теория", TextAlign = TextAlign.Center });
            ExportColumns.Add(new GridColumn() { Field = "Practice", HeaderText = "Практика", TextAlign = TextAlign.Center });
            ExportColumns.Add(new GridColumn() { Field = "ERUsForExport", HeaderText = "ЕРУ", TextAlign = TextAlign.Center, });

            return ExportColumns;
        }
    }
}
