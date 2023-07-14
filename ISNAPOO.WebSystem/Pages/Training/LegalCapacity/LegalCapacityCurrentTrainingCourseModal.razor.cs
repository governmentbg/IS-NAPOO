using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Grid;
using System.Data;
using Syncfusion.Drawing;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using ISNAPOO.Common.Constants;

namespace ISNAPOO.WebSystem.Pages.Training.LegalCapacity
{
    public partial class LegalCapacityCurrentTrainingCourseModal : BlazorBaseComponent
    {
        private CurrentCourseInformation currentCourseInformation = new CurrentCourseInformation();
        private CurrentCourseTrainingCurriculum currentCourseTrainingCurriculum = new CurrentCourseTrainingCurriculum();
        private CurrentCoursePremises currentCoursePremises = new CurrentCoursePremises();
        private CurrentCourseTrainers currentCourseTrainers = new CurrentCourseTrainers();
        private LegalCapacityCurrentCourseClients currentCourseClients = new LegalCapacityCurrentCourseClients();
        private CurrentCourseExam currentCourseExam = new CurrentCourseExam();
        private TrainingCourseStatistic currentCourseStatistic = new TrainingCourseStatistic();
        private TrainingCourseOrdersList trainingCourseOrdersList = new TrainingCourseOrdersList();
        private TestProtocols testProtocols = new TestProtocols();
        private ClientCourseSubjects clientCourseSubjects = new ClientCourseSubjects();
        private CourseProtocols courseProtocols = new CourseProtocols();

        private bool isEditable = true;
        private CourseVM courseVM = new CourseVM();
        private List<string> validationMessages = new List<string>();
        private int selectedTab = 0;
        private IEnumerable<KeyValueVM> courseStatusSource = new List<KeyValueVM>();
        private KeyValueVM kvCurrentCourse = new KeyValueVM();
        private IEnumerable<KeyValueVM> vqsSource = new List<KeyValueVM>();
        private bool hideBtnsConcurrentModal = false;
        private List<ClientCourseVM> clientsSource = new List<ClientCourseVM>();
        private int count;

        public override bool IsContextModified => this.currentCourseInformation.IsEditContextModified() || this.currentCourseExam.IsEditContextModified();

        [Parameter]
        public EventCallback CallBackAfterSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.courseVM);
        }

        public async Task OpenModal(CourseVM course, ConcurrencyInfo concurrencyInfo = null, bool isEdtiable = true)
        {
            this.selectedTab = 0;

            this.courseVM = course;
            this.isEditable = isEdtiable;
            if (this.courseVM.IdCourse != 0)
            {
                this.IdTrainingCourse = this.courseVM.IdCourse;
            }

            this.courseStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus");
            this.kvCurrentCourse = this.courseStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseStatusNow");
            this.vqsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            this.validationMessages.Clear();

            await this.SetCreateAndModifyInfoAsync();

            this.editContext = new EditContext(this.courseVM);

            this.isVisible = true;
            this.StateHasChanged();

            if (concurrencyInfo is not null && concurrencyInfo.IdPerson != this.UserProps.IdPerson)
            {
                this.hideBtnsConcurrentModal = true;
                this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfo);
            }
            else
            {
                this.hideBtnsConcurrentModal = false;
            }
        }

        private void Select(SelectingEventArgs args)
        {
            if (args.IsSwiped)
            {
                args.Cancel = true;
            }
        }

        private async Task ExportPdf()
        {
            int paragraphAfterSpacing = 8;
            int cellMargin = 8;

            PdfDocument doc = new PdfDocument();


            PdfPage page = doc.Pages.Add();

            PdfFont documentTitleFont = new PdfTrueTypeFont(@"C:\WINDOWS\FONTS\Arial.ttf", 12, PdfFontStyle.Bold);
            PdfFont documentSubtitleFont = new PdfTrueTypeFont(@"C:\WINDOWS\FONTS\Arial.ttf", 12);

            PdfFont documentContentFont = new PdfTrueTypeFont(@"C:\WINDOWS\FONTS\Arial.ttf", 11);
            PdfTextElement documentTitle = new PdfTextElement("Справка за курс", documentTitleFont, PdfBrushes.Black);
            PdfLayoutResult result = documentTitle.Draw(page, new PointF(0, 0));

            PdfTextElement courseTitle = new PdfTextElement("\nЦПО към " + this.courseVM.CandidateProvider.ProviderOwner, documentSubtitleFont, PdfBrushes.Black);
            PdfLayoutFormat format = new PdfLayoutFormat();
            format.Layout = PdfLayoutType.Paginate;
            //We add courseTitle to our document.
            result = courseTitle.Draw(page, new RectangleF(0, result.Bounds.Bottom + paragraphAfterSpacing, page.GetClientSize().Width, page.GetClientSize().Height), format);

            //Create a PdfGrid.
            PdfGrid pdfGrid = new PdfGrid();
            pdfGrid.Style.CellPadding.Left = cellMargin;
            pdfGrid.Style.CellPadding.Right = cellMargin;

            //Applying built-in style to the PDF grid
            //pdfGrid.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent1);

            //Assign data source.
            DataTable dataTable = new DataTable();
            //Add columns to the DataTable

            dataTable.Columns.Add("Професия / Специалност / СПК:");
            dataTable.Columns.Add(this.courseVM.Program.Speciality.Profession.Name + " " + this.courseVM.Program.Speciality.Profession.Code + " / " + this.courseVM.Program.Speciality.Name + " " + this.courseVM.Program.Speciality.Code + " / " + this.vqsSource.FirstOrDefault(x => x.IdKeyValue == this.courseVM.Program.Speciality.IdVQS).Name);//Add rows to the DataTable.
                                                                                                                                                                                                                                                                                                                                                     //  dataTable.Rows.Add(new object[] { "Професия / Специалност / СПК:", this.courseVM.Program.Speciality.Profession.Name + " / " + this.courseVM.Program.Speciality.Name + " / " + this.courseVM.Program.Speciality.VQS_Name });
            dataTable.Rows.Add(new object[] { "Наименование на програмата:", this.courseVM.Program.ProgramName });
            dataTable.Rows.Add(new object[] { "Вид на курса:", this.courseVM.TrainingCourseTypeName });
            dataTable.Rows.Add(new object[] { "Допълнителни бележки:", this.courseVM.Program.ProgramNote });
            dataTable.Rows.Add(new object[] { "Наименование на курса:", this.courseVM.CourseName });
            dataTable.Rows.Add(new object[] { "Населено място, в което се провеждат занятията:", this.courseVM.Location.DisplayJoinedNames });
            dataTable.Rows.Add(new object[] { "Вид:", this.courseVM.MeasureType.Name });
            dataTable.Rows.Add(new object[] { "Статус на курса:", this.courseVM.Status.Name });
            dataTable.Rows.Add(new object[] { "Основен източник на финансиране:", this.courseVM.AssignType.Name });
            dataTable.Rows.Add(new object[] { "Форма на обучение:", this.courseVM.FormEducation.Name });
            dataTable.Rows.Add(new object[] { "Продължителност на обучението (в часове):", this.courseVM.MandatoryHours });
            dataTable.Rows.Add(new object[] { "Цена (в лева за един курсист): ", this.courseVM.Cost });
            dataTable.Rows.Add(new object[] { "Други пояснения: ", this.courseVM.AdditionalNotes });
            dataTable.Rows.Add(new object[] { "Крайна дата за записване: ", this.courseVM.SubscribeDate.HasValue ? this.courseVM.SubscribeDate.Value.ToString("dd.MM.yyyy") : "" });
            dataTable.Rows.Add(new object[] { "Очаквана дата за започване на курса:", this.courseVM.StartDate.HasValue ? this.courseVM.StartDate.Value.ToString("dd.MM.yyyy") : "" });
            dataTable.Rows.Add(new object[] { "Очаквана дата за завършване на курса:", this.courseVM.EndDate.HasValue ? this.courseVM.EndDate.Value.ToString("dd.MM.yyyy") : "" });
            dataTable.Rows.Add(new object[] { "Очаквана дата за изпит по теория:", this.courseVM.ExamTheoryDate.HasValue ? this.courseVM.ExamTheoryDate.Value.ToString("dd.MM.yyyy") : "" });
            dataTable.Rows.Add(new object[] { "Очаквана дата за изпит по практика:", this.courseVM.ExamPracticeDate.HasValue ? this.courseVM.ExamPracticeDate.Value.ToString("dd.MM.yyyy") : "" });


            pdfGrid.DataSource = dataTable;

            //  pdfGrid.BeginCellLayout += PdfGrid_BeginCellLayout;
            pdfGrid.Style.Font = documentContentFont;
            DataTable clients = new DataTable();
            this.clientsSource = (await this.TrainingService.GetCourseClientsByIdCourseAsync(this.courseVM.IdCourse)).ToList();

            //Draw PDF grid into the PDF page

            result = pdfGrid.Draw(page, new PointF(0, result.Bounds.Bottom + paragraphAfterSpacing + 16));
            PdfStringFormat pdfStringFormat = new PdfStringFormat();
            pdfStringFormat.Alignment = PdfTextAlignment.Center;
            PdfTextElement courseParticipantsTitle = new PdfTextElement("\nСписък на курсистите в курса:", documentTitleFont, new PdfPen(Color.Black), PdfBrushes.Black, pdfStringFormat);
            result = courseParticipantsTitle.Draw(page, new RectangleF(0, result.Bounds.Bottom + paragraphAfterSpacing, page.GetClientSize().Width, page.GetClientSize().Height), format);

            int countRow = 0;
            foreach (var client in clientsSource)
            {
                countRow++;
                PdfTextElement courseParticipants = new PdfTextElement(countRow + ". " + client.FirstName + " " + client.SecondName + " " + client.FamilyName, documentContentFont, PdfBrushes.Black);
                result = courseParticipants.Draw(page, new RectangleF(0, result.Bounds.Bottom + paragraphAfterSpacing, page.GetClientSize().Width, page.GetClientSize().Height), format);
            }


            MemoryStream stream = new MemoryStream();

            //Saving the PDF document into the stream.
            doc.Save(stream);
            //Closing the PDF document.
            doc.Close(true);

            await this.JsRuntime.SaveAs("Справка.pdf", stream.ToArray());


        }

        //Method to remove table headers in PDF report
        private void PdfGrid_BeginCellLayout(object sender, PdfGridBeginCellLayoutEventArgs args)
        {
            count++;
            PdfGrid grid = (sender as PdfGrid);
            if (count <= grid.Headers.Count * grid.Columns.Count)
            {
                args.Skip = true;
            }
        }

        private async Task SubmitBtn(bool showToast)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.validationMessages.Clear();
                this.currentCourseInformation.SubmitHandler();
                this.validationMessages.AddRange(this.currentCourseInformation.GetValidationMessages());

                if (!this.validationMessages.Any())
                {
                    if (this.courseVM.IdStatus == this.kvCurrentCourse.IdKeyValue && await this.TrainingService.IsCandidateProviderLicenceSuspendedAsync(this.courseVM.IdCandidateProvider!.Value, this.courseVM.StartDate!.Value))
                    {
                        await this.ShowErrorAsync("Не можете да създадете курс за правоспособност, тъй като в момента сте с временно отнета лицензия!");
                        this.SpinnerHide();
                        return;
                    }

                    if (await this.IsAnnualReportSubmittedOrApprovedAsync())
                    {
                        await this.ShowErrorAsync($"Не можете да запишете текущ курс за правоспособност с 'Дата на завършване на курса' {this.courseVM.EndDate!.Value.Year} г., защото има данни за подаден годишен отчет за {this.courseVM.EndDate!.Value.Year} г.!");
                        this.SpinnerHide();
                        return;
                    }

                    if (await this.IsSpecialityFromSPPOORemovedWithOrderBeforeCourseStartDateAsync())
                    {
                        await this.ShowErrorAsync($"Не можете да запишете текущ курс за специалност, която е отпаднала от Списъка на професиите за професионално образование и обучение!");
                        this.SpinnerHide();
                        return;
                    }

                    var result = new ResultContext<CourseVM>();
                    result.ResultContextObject = this.courseVM;
                    result = await this.TrainingService.UpdateTrainingCourseAsync(result);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        if (showToast)
                        {
                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                        }

                        await this.SetCreateAndModifyInfoAsync();

                        await this.CallBackAfterSubmit.InvokeAsync();
                    }
                }
                else
                {
                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task CompleteTrainingCourseBtn()
        {
            string msg = "Сигурни ли сте, че искате да приключите текущия курс?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;



                    this.SpinnerShow();

                    var clientsDict = new Dictionary<string, string>();
                    var areAllRequiredDocsForClientsUploaded = await this.AreAllRequiredDocsForClientsUploadedAsync(clientsDict);
                    if (!areAllRequiredDocsForClientsUploaded)
                    {
                        await this.ShowErrorAsync("Не можете да приключите курса, защото не са добавени всички задължителни документи за курсистите! Моля, коригирайте грешките във файла!");

                        var excelStream = this.TrainingService.CreateExcelWithMissingRequiredDocumentsForClients(clientsDict);
                        await this.JsRuntime.SaveAs($"Spisak_neprikacheni_dokumenti_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", excelStream.ToArray());

                        this.SpinnerHide();
                        return;
                    }

                    this.loading = false;
                    await this.SubmitBtn(false);

                    if (!this.validationMessages.Any())
                    {
                        var inputContext = new ResultContext<CourseVM>();
                        inputContext.ResultContextObject = this.courseVM;
                        inputContext = await this.TrainingService.CompleteCurrentTrainingCourseAsync(inputContext);
                        if (inputContext.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join(Environment.NewLine, inputContext.ListErrorMessages));
                        }
                        else
                        {
                            this.isVisible = false;

                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, inputContext.ListMessages));

                            await this.CallBackAfterSubmit.InvokeAsync();
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

        private async Task<bool> AreAllRequiredDocsForClientsUploadedAsync(Dictionary<string, string> clientsDict)
        {
            var clientsFromCourse = await this.TrainingService.GetCourseClientsByIdCourseAsync(this.courseVM.IdCourse);
            var courseRequiredDocuments = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType"))
                .Where(x => x.DefaultValue3 != null && x.DefaultValue3.Contains("CPO") && x.DefaultValue1 != null && x.DefaultValue1.Contains("Required_CPO")).ToList();

            var counter = 1;
            foreach (var client in clientsFromCourse)
            {
                foreach (var document in courseRequiredDocuments)
                {
                    if (!client.ClientRequiredDocuments.Any(x => x.IdCourseRequiredDocumentType == document.IdKeyValue && !string.IsNullOrEmpty(x.UploadedFileName)))
                    {
                        clientsDict.Add(counter++ + "." + client.FullName, document.Name);
                    }
                }
            }

            return !clientsDict.Keys.Any();
        }

        private async Task<bool> IsAnnualReportSubmittedOrApprovedAsync()
        {
            return await this.TrainingService.IsAnnualReportSubmittedOrApprovedByIdCandidateProviderAndYearAsync(this.courseVM.IdCandidateProvider!.Value, this.courseVM.EndDate!.Value.Year);
        }

        private void GetValidationErrorsFromExamTab()
        {
            this.validationMessages.Clear();
            this.validationMessages.AddRange(this.currentCourseExam.GetValidationMessages());
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.courseVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.courseVM.IdModifyUser);
            this.courseVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.courseVM.IdCreateUser);
        }

        private async Task<bool> IsSpecialityFromSPPOORemovedWithOrderBeforeCourseStartDateAsync()
        {
            return await this.TrainingService.IsSpecialityFromSPPOORemovedWithOrderBeforeCourseStartDateAsync(this.courseVM.Program.IdSpeciality, this.courseVM.StartDate!.Value);
        }
    }
}
