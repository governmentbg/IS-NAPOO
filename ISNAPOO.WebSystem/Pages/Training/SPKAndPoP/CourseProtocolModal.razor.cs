using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CourseProtocolModal : BlazorBaseComponent
    {
        private SfGrid<CourseProtocolGradeVM>? clientsGrid;
        private AddSingleClientCourseModal addSingleClientCourseModal = new AddSingleClientCourseModal();
        private TestProtocols testProtocols = new TestProtocols();
        private SfUploader uploader = new SfUploader();
        private IEnumerable<CourseProtocolVM> protocolsSource = new List<CourseProtocolVM>();
        private CurrentCourseExamModal currentCourseExam = new CurrentCourseExamModal(); 
        private List<CourseProtocolGradeVM> clientSource = new List<CourseProtocolGradeVM>();
        private CourseProtocolVM courseProtocolVM = new CourseProtocolVM();
        private CourseProtocolGradeVM model = new CourseProtocolGradeVM();
        private bool hideBtnsConcurrentModal = false;
        private IEnumerable<KeyValueVM> courseProtocolTypeSource = new List<KeyValueVM>();
        private ValidationMessageStore? messageStore;
        private List<string> validationMessages = new List<string>();
        private string courseName = string.Empty;
        private CourseProtocolGradeVM clientToDelete = new CourseProtocolGradeVM();
        private bool isDeletion = false;
        private bool isOpenedFromStateExam = false;
        private IEnumerable<CourseVM> coursesSource = new List<CourseVM>();
        private KeyValueVM kvProtocol381 = new KeyValueVM();
        private IEnumerable<CourseCommissionMemberVM> courseCommissionMemberSource = new List<CourseCommissionMemberVM>();
        private KeyValueVM kvCourseTypeSPK = new KeyValueVM();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Parameter] public bool IsEditable { get; set; } = true;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.courseProtocolVM);
            this.clientsGrid = new SfGrid<CourseProtocolGradeVM>();
        }

        public async Task OpenModal(CourseProtocolVM courseProtocol, ConcurrencyInfo concurrencyInfo = null, bool openedFromStateExam = false, bool isEditable = true)
        {
            this.courseProtocolTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType");
            this.kvProtocol381 = this.courseProtocolTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "3-81B");
            this.protocolsSource = await this.TrainingService.GetAllCourseProtocolsByIdCourseAsync(courseProtocol.IdCourse);

            this.kvCourseTypeSPK = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");

            this.editContext = new EditContext(this.courseProtocolVM);
            this.validationMessages.Clear();
            //this.IsEditable = isEditable;
            this.clientSource.Clear();

            this.isOpenedFromStateExam = openedFromStateExam;
            if (this.isOpenedFromStateExam)
            {
                this.coursesSource = await this.TrainingService.GetAllCoursesWhichAreNotOnStatusUpcomingByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider);
            }

            this.courseProtocolVM = courseProtocol;

            if (this.courseProtocolVM.IdCourse != 0)
            {
                this.courseCommissionMemberSource = await this.TrainingService.GetAllCourseCommissionChairmansByIdCourseAsync(courseProtocol.IdCourse);
            }

            this.SetCourseName();

            if (this.courseProtocolVM.IdCourseProtocol != 0)
            {
                await this.LoadClientsDataAsync();
            }

            this.SetProtocolTypeName();

            await this.SetCreateAndModifyInfoAsync();

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

            this.StateHasChanged();
        }

        private void SetCourseName()
        {
            if (this.courseProtocolVM.Course != null)
            {
                this.courseName = this.courseProtocolVM.Course.CourseName;
            }
            else
            {
                this.courseName = string.Empty;
            }
        }

        private async Task OnCourseSelected(ChangeEventArgs<int, CourseVM> args)
        {
            if (args is not null)
            {
                this.courseProtocolVM.Course = args.ItemData as CourseVM;
                if (this.courseProtocolVM.Course is not null)
                {
                    this.courseCommissionMemberSource = await this.TrainingService.GetAllCourseCommissionChairmansByIdCourseAsync(this.courseProtocolVM.Course.IdCourse);
                }
                else
                {
                    this.courseCommissionMemberSource = new List<CourseCommissionMemberVM>();
                }
            }
        }

        private void OnProtocolSelected(ChangeEventArgs<int, KeyValueVM> args)
        {
            if (args is not null && args.ItemData is not null)
            {
                var protocolType = args.ItemData.KeyValueIntCode;
                if (protocolType == "3-80p")
                {
                    this.courseProtocolVM.CourseProtocolDate = this.courseProtocolVM.Course.ExamPracticeDate;
                }
                else if (protocolType == "3-80t")
                {
                    this.courseProtocolVM.CourseProtocolDate = this.courseProtocolVM.Course.ExamTheoryDate;
                }
            }
        }

        private async Task AddClientCourseBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var allClientsAreAdded = await this.TrainingService.AreAllCourseClientsAlreadyAddedToCourseProtocolGradeByIdCourseAndByIdProtocolAsync(this.courseProtocolVM.IdCourse, this.courseProtocolVM.IdCourseProtocol);
                if (allClientsAreAdded)
                {
                    await this.ShowErrorAsync("Всички курсисти са вече добавени!");
                }
                else
                {
                    var clients = (await this.TrainingService.GetAllClientsWhichAreNotAddedToProtocolByIdCourseAsync(this.courseProtocolVM.IdCourse, this.clientSource)).ToList();
                    this.addSingleClientCourseModal.OpenModal(clients, this.courseProtocolVM.IdCourseProtocol);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddCommissionBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.currentCourseExam.OpenModal(this.courseProtocolVM.Course);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddAllCourseClientsBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var allClientsAreAdded = await this.TrainingService.AreAllCourseClientsAlreadyAddedToCourseProtocolGradeByIdCourseAndByIdProtocolAsync(this.courseProtocolVM.IdCourse, this.courseProtocolVM.IdCourseProtocol);
                if (allClientsAreAdded)
                {
                    await this.ShowErrorAsync("Всички курсисти са вече добавени!");
                }
                else
                {
                    await this.SubmitBtn(false);
                    if (!this.validationMessages.Any())
                    {
                        var result = new ResultContext<CourseProtocolVM>();
                        result.ResultContextObject = this.courseProtocolVM;
                        result = await this.TrainingService.AddAllCourseClientsToCourseProtocolGradeAsync(result);
                        if (result.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                        }
                        else
                        {
                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                            await this.LoadClientsDataAsync();
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

        private async Task DeleteClientBtn(CourseProtocolGradeVM model)
        {
            this.isDeletion = true;

            this.clientToDelete = model;

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {
                var result = await this.TrainingService.DeleteCourseProtocolGradeByIdAsync(model.IdCourseProtocolGrade);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                    await this.LoadClientsDataAsync();
                }
            }
        }

        private async Task LoadClientsDataAsync()
        {
            this.clientSource = (await this.TrainingService.GetAllCourseProtocolGradesByIdProtocolAsync(this.courseProtocolVM.IdCourseProtocol)).ToList();
            foreach (var client in this.clientSource)
            {
                if (client.Grade.HasValue)
                {
                    client.GradeAsStr = client.Grade.Value.ToString("f2");
                }
            }

            this.StateHasChanged();
        }

        private void SetProtocolTypeName()
        {
            if (this.courseProtocolVM.IdCourseProtocolType != 0)
            {
                var type = this.courseProtocolTypeSource.FirstOrDefault(x => x.IdKeyValue == this.courseProtocolVM.IdCourseProtocolType);
                if (type is not null)
                {
                    this.courseProtocolVM.CourseProtocolTypeName = type.Name;
                }
            }
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;

            var result = await this.UploadFileService.UploadFileAsync<CourseProtocol>(file, args.Files[0].FileInfo.Name, "CourseProtocol", this.courseProtocolVM.IdCourseProtocol);
            if (!string.IsNullOrEmpty(result))
            {
                this.courseProtocolVM.UploadedFileName = result;
            }

            await this.CallbackAfterSubmit.InvokeAsync();

            await this.uploader.ClearAllAsync();

            this.StateHasChanged();
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.courseProtocolVM.UploadedFileName))
            {
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли си сте, че искате да изтриете прикачения файл?");

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<CourseProtocol>(this.courseProtocolVM.IdCourseProtocol);
                    if (result == 1)
                    {
                        this.courseProtocolVM.UploadedFileName = null;
                    }

                    await this.CallbackAfterSubmit.InvokeAsync();

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.courseProtocolVM.UploadedFileName))
            {
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли си сте, че искате да изтриете прикачения файл?");

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<CourseProtocol>(this.courseProtocolVM.IdCourseProtocol);
                    if (result == 1)
                    {
                        this.courseProtocolVM.UploadedFileName = null;
                    }

                    await this.CallbackAfterSubmit.InvokeAsync();

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnDownloadClick()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(this.courseProtocolVM.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseProtocol>(this.courseProtocolVM.IdCourseProtocol);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<CourseProtocol>(this.courseProtocolVM.IdCourseProtocol);
                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.courseProtocolVM.FileName, document.MS!.ToArray());
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

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.courseProtocolVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.courseProtocolVM.IdModifyUser);
            this.courseProtocolVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.courseProtocolVM.IdCreateUser);
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

                this.editContext = new EditContext(this.courseProtocolVM);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateChairmanSelected;
                this.editContext.OnValidationRequested += this.ValidateProtocol381BDate;
                this.editContext.OnValidationRequested += this.ValidateProtocolNumber;
                if (this.editContext.Validate())
                {
                    var result = new ResultContext<CourseProtocolVM>();
                    result.ResultContextObject = this.courseProtocolVM;
                    if (this.courseProtocolVM.IdCourseProtocol != 0)
                    {
                        result = await this.TrainingService.UpdateCourseProtocolAsync(result);
                    }
                    else
                    {
                        if (this.courseProtocolVM.IdCourseProtocolType == this.kvProtocol381.IdKeyValue && !(await this.TrainingService.AreProtocols380TAnd380PAlreadyAddedByIdCourseAsync(this.courseProtocolVM.IdCourse)))
                        {
                            await this.ShowErrorAsync("Не можете да създадете протокол 3-81В преди да попълните данни за протоколи 3-80 Теория и 3-80 Практика!");
                            this.SpinnerHide();
                            return;
                        }

                        result = await this.TrainingService.CreateCourseProtocolAsync(result);
                    }

                    if (result.HasErrorMessages)
                    {
                        if (showToast)
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                        }
                    }
                    else
                    {
                        if (showToast)
                        {
                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                        }

                        this.SetCourseName();

                        this.SetProtocolTypeName();

                        await this.LoadClientsDataAsync();

                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                }
                else
                {
                    this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void OnGradeInput()
        {
            this.isDeletion = false;
        }

        private async Task SaveGradeAsync(CourseProtocolGradeVM model)
        {
            if (this.IsEditable)
            {
                if (!this.isDeletion)
                {
                    if (this.loading)
                    {
                        return;
                    }

                    try
                    {
                        this.loading = true;

                        if (!string.IsNullOrEmpty(model.GradeAsStr))
                        {
                            this.model.GradeAsStr = model.GradeAsStr;

                            if (!this.IsGradeValid())
                            {
                                await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                            }
                            else
                            {
                                model.Grade = BaseHelper.ConvertToFloat(model.GradeAsStr, 2);
                                model.GradeAsStr = model.Grade.Value.ToString("f2");
                                model.Grade = Math.Round(model.Grade.Value, 2, MidpointRounding.AwayFromZero);

                                var result = new ResultContext<CourseProtocolGradeVM>();
                                result.ResultContextObject = model;

                                result = await this.TrainingService.UpdateCourseProtocolGradeAsync(result);
                                if (result.HasErrorMessages)
                                {
                                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                                }
                                else
                                {
                                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                                }
                            }
                        }
                    }
                    finally

                    {
                        this.loading = false;
                    }
                }

            }
        }

        private bool IsGradeValid()
        {
            this.validationMessages.Clear();

            if (!string.IsNullOrEmpty(this.model.GradeAsStr))
            {
                if (BaseHelper.ConvertToFloat(this.model.GradeAsStr, 2) == null)
                {
                    this.validationMessages.Add("Полето 'Оценка' може да бъде само число!");
                    return false;
                }
            }

            var value = BaseHelper.ConvertToFloat(this.model.GradeAsStr, 2);
            if (value < 2)
            {
                this.validationMessages.Add("Полето 'Оценка' не може да има стойност по-малка от 2.00!");
                return false;
            }

            if (value > 6)
            {
                this.validationMessages.Add("Полето 'Оценка' не може да има стойност по-голяма от 6.00!");
                return false;
            }

            if (value.ToString().Length > 4)
            {
                this.validationMessages.Add("Полето 'Оценка' не може да съдържа повече от 2 знака след десетичната запетая!");
                return false;
            }

            return true;
        }

        private async Task UpdateAfterSingleClientAddedAsync(string message)
        {
            if (message.Contains("успешно"))
            {
                await this.LoadClientsDataAsync();

                await this.ShowSuccessAsync(message);
            }
            else
            {
                await this.ShowErrorAsync(message);
            }
        }

        private void ValidateChairmanSelected(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.courseProtocolVM.IdCourseProtocolType == this.kvProtocol381.IdKeyValue && this.courseProtocolVM.IdCourseCommissionMember is null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.courseProtocolVM, "IdCourseCommissionMember");
                this.messageStore?.Add(fi, "Полето 'Председател на изпитна комисия' е задължително!");
            }
        }

        private void ValidateProtocol381BDate(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.courseProtocolVM.IdCourseProtocolType == this.kvProtocol381.IdKeyValue && this.courseProtocolVM.CourseProtocolDate.HasValue && this.courseProtocolVM.Course.ExamPracticeDate.HasValue && this.courseProtocolVM.CourseProtocolDate.Value.Date < this.courseProtocolVM.Course.ExamPracticeDate.Value.Date)
            {
                var msg = this.courseProtocolVM.Course.IdTrainingCourseType == this.kvCourseTypeSPK.IdKeyValue 
                    ? $"Датата на протокол 3-81В не може да бъде преди '{this.courseProtocolVM.Course.ExamPracticeDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.' (Дата за държавен изпит - част по практика)!"
                    : $"Датата на протокол 3-81В не може да бъде преди '{this.courseProtocolVM.Course.ExamPracticeDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.' (Дата за изпит - част по практика)!";
                FieldIdentifier fi = new FieldIdentifier(this.courseProtocolVM, "CourseProtocolDate");
                this.messageStore?.Add(fi, msg);
            }
        }

        private void ValidateProtocolNumber(object? sender, ValidationRequestedEventArgs args)
        {
           
            if (this.protocolsSource.Any(x => x.CourseProtocolNumber?.Trim() == this.courseProtocolVM.CourseProtocolNumber?.Trim()))
            {
               
                FieldIdentifier fi = new FieldIdentifier(this.courseProtocolVM, "CourseProtocolNumber");
                this.messageStore?.Add(fi, "Протокол със същия номер вече е въведен!");
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
                ExportProperties.FileName = $"Course_Protocol_Clients_Grades_List_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.clientsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Course_Protocol_Clients_Grades_List_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.clientsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Grade", HeaderText = "Оценка", TextAlign = TextAlign.Left, Format = "f2" });

            return ExportColumns;
        }
    }
}
