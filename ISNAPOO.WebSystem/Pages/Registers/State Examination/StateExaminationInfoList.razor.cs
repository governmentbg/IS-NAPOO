using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Register;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using System.Linq;
using System.Reflection;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Data.Models.Data.Common;

namespace ISNAPOO.WebSystem.Pages.Registers.State_Examination
{
    public partial class StateExaminationInfoList
    {
        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }

        [Inject]
        public ITrainingService trainingService { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        private CurrentTrainingCourseModal currentTrainingCourseModal = new CurrentTrainingCourseModal();
        public string CourseType { get; set; }

        SfGrid<CourseVM> sfGrid = new SfGrid<CourseVM>();
        List<CourseVM> coursesSource = new List<CourseVM>();
        List<CourseVM> tempFilterCourses = new List<CourseVM>();
        CourseVM tempCourseVM = new CourseVM();
        IEnumerable<KeyValueVM> kvCourseTypeSource;
        //RegisterProviderReportFilter reportFilter;
        StateExaminationInfoFilterList filterModal;
        ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
        protected override async Task OnInitializedAsync()
        {
             tokenContext.ResultContextObject.Token = Token;
             tokenContext = BaseHelper.GetDecodeToken(tokenContext);
        }

        public async Task FilterGrid()
        {
          await this.filterModal.OpenModal();
        }

        public async Task Filter(StateExaminationInfoFilterListVM model)
        { 

            this.CourseType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "StateExaminationInfoList").Value.ToString();

            this.kvCourseTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram", false);

            var temp = (await this.trainingService.getAllCourses(model, "FromStateExaminationPage"))
                .Where(x => x.IdTrainingCourseType == kvCourseTypeSource.First(x => x.KeyValueIntCode == this.CourseType).IdKeyValue)
                .ToList();

            var result = new List<CourseVM>();

            result.AddRange(coursesSource.Where(x => !x.ExamTheoryDate.HasValue && !x.ExamPracticeDate.HasValue).ToList());

            foreach (var course in temp.Where(x => x.ExamPracticeDate.HasValue || x.ExamTheoryDate.HasValue).ToList())
            {
                if (model.TrainingTypeIntCode == null)
                {
                    CourseVM tempCourseVM = new CourseVM();
                    foreach (var f in tempCourseVM.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                    {
                        f.SetValue(tempCourseVM, f.GetValue(course));
                    }
                    tempCourseVM.combined_date = course.ExamPracticeDate;
                    result.Add(tempCourseVM);
                    course.combined_date = course.ExamTheoryDate;                    
                }
                else if (model.TrainingTypeIntCode == "Theory")
                {
                    course.combined_date = course.ExamTheoryDate;
                   
                }
                else if (model.TrainingTypeIntCode == "Practice")
                {
                    course.combined_date = course.ExamPracticeDate;
                    
                }
                result.Add(course);
            }

            this.coursesSource = result.OrderByDescending(x => x.combined_date).ToList();
        }

        public async Task openModal(CourseVM course)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }

            try
            {
                this.loading = true;

                var courseFromDb = await this.trainingService.GetTrainingCourseByIdAsync(course.IdCourse);

                await this.currentTrainingCourseModal.OpenModal(courseFromDb, null, false);

            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = this.sfGrid.TotalItemCount;
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "CandidateProviderLicenseNumber", HeaderText = "Лицензия", Width = "80", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProviderName", HeaderText = "ЦПО", Width = "180", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CourseName", HeaderText = "Курс", Width = "100", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CourseTypeByDate", HeaderText = "Вид", Width = "50", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "combined_date_string", HeaderText = "Дата на изпит", Width = "80", Format = "C2", TextAlign = TextAlign.Left });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.PageOrientation = PageOrientation.Landscape;
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

                ExportProperties.FileName = $"Register_State_Examination_Info_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005

                ExportColumns.Add(new GridColumn() { Field = "CandidateProviderLicenseNumber", HeaderText = "Лицензия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProviderName", HeaderText = "ЦПО", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CourseName", HeaderText = "Курс", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CourseTypeByDate", HeaderText = "Вид", Width = "50", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "combined_date_string", HeaderText = "Дата на изпит", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"Register_State_Examination_Info_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
            //else
            //{
            //    var result = CreateExcelCurriculumValidationErrors();
            //    await this.JsRuntime.SaveAs($"Register_MTB_CPO&CIPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.csv", result.ToArray());
            //}
        }
    }
}
