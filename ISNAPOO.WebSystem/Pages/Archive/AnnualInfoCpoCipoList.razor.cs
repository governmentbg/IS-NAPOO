using System.Data;
using System.Net.Mail;
using Data.Models.Data.Archive;
using Data.Models.Data.EGovPayment;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Services.Mailing;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Archive.CIPO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Mail;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.SplitButtons;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualInfoCpoCipoList : BlazorBaseComponent
    {
        [Inject]
        public IArchiveService ArchiveService { get; set; }
        [Inject]
        public IMailService mailService { get; set; }
        [Inject]
        public ISettingService settingService { get; set; }
        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }
        [Inject]
        public IPersonService personService { get; set; }
        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        private IEnumerable<CandidateProviderVM> annualInfoSource = new List<CandidateProviderVM>();

        private List<AnnualInfoVM> archAnnualInfoSource = new List<AnnualInfoVM>();
        public string LicensingType { get; set; }
        private SfGrid<AnnualInfoVM> sfAnnualInfoGrid = new SfGrid<AnnualInfoVM>();
        private AnnualInfoVM rowAnnInfo = new AnnualInfoVM();
        SendMailModal sendMailModal = new SendMailModal();
        private Model model = new Model();
        private List<CandidateProviderVM> selectedRows = new List<CandidateProviderVM>();
        private List<AnnualInfoVM> selectedRow = new List<AnnualInfoVM>();
        private AnnualInfoCpoCipoStatusList annualInfoCpoCipoStatusList = new AnnualInfoCpoCipoStatusList();
        private AnnualMTBModal mtbModal = new AnnualMTBModal();
        private AnnualStudentsModal studentModal = new AnnualStudentsModal();
        private AnnualTrainingCourseModal reporTrainingCourse = new AnnualTrainingCourseModal();
        private AnnualCurriculumModal curriculumModal = new AnnualCurriculumModal();
        private AnnualTrainerQualificationsModal trainerQualificationsModal = new AnnualTrainerQualificationsModal();
        private AnnualStudentByNationalityModal annualStudentByNationalityModal = new AnnualStudentByNationalityModal();
        private AnnualTrainingValidationClientModal reportTrainingValidationCourse = new AnnualTrainingValidationClientModal();
        private HandleTotalConsultedClients handleTotalConsultedClients = new HandleTotalConsultedClients();
        private AnnualFinancing annualFinancing = new AnnualFinancing();
        private ISNAPOO.WebSystem.Pages.Archive.CIPO.AnnualConsulting annualConsultingModal = new ISNAPOO.WebSystem.Pages.Archive.CIPO.AnnualConsulting();
        private ToastMsg toast;
        private IEnumerable<KeyValueVM> kvAnualInfoStatusSource = new List<KeyValueVM>();
        private ResultContext<NoResult> result = new ResultContext<NoResult>();
        int unfinishedCoursesCount = 0;
        protected override async Task OnInitializedAsync()
        {

            this.editContext = new EditContext(this.model);          
            this.kvAnualInfoStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AnnualInfoStatusType");

            var tempProvider = await this.candidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(this.UserProps.IdCandidateProvider);
            this.LicensingType = (await this.DataSourceService.GetKeyValueByIdAsync(tempProvider.IdTypeLicense)).KeyValueIntCode;
        }     

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await ReloadData();
            }
        }
        private class Model
        {
           // public int Year { get; set; }
        }
        private async Task sendEmail()
        {
            await sendMailModal.openModal();
        }

        private async Task sendEmails(MailMessage mail)
        {
            //var rows = await sfGrid.GetSelectedRecordsAsync();

            //foreach (var provider in rows)
            //{
            //    if (provider.ProviderEmailCorrespondence != null)
            //    {
            //        mail.To.Add(provider.ProviderEmailCorrespondence);
            //    }
            //}
            //await mailService.SendCustomEmail(mail);

            //await this.ShowSuccessAsync("E-mail-ите са изпратени успешно!");
            //await this.sfGrid.Refresh();
            //this.SpinnerHide();
        }

        private async Task OpenReportConfirmModal()
        {
            if (!this.selectedRow.Any())
            {
                await this.ShowErrorAsync("Моля, изберете ред!");
                return;
            }
            int unfinishedCourses = await this.ArchiveService.GetNoFinishedCourses(rowAnnInfo.IdCandidateProvider, rowAnnInfo.Year);
            if (unfinishedCourses > 0)
            {
                bool isConfirmed = await this.ShowConfirmDialogAsync("Имате курсове, които не са със статус Приключили!\r\nСигурни ли сте, че искате да подадете отчета за годиншната информация за дейността към НАПОО?\r\nСлед подаване на отчета към НАПОО, няма да може да извършвате промени в попълнената информация.");
                if (isConfirmed)
                {
                    await this.sendReport();
                }
            }
            else
            {
                bool isConfirm = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да подадете отчета за годиншната информация за дейността към НАПОО? След подаване на отчета към НАПОО, няма да може да извършвате промени в попълнената информация.");
                if (isConfirm)
                {
                    await this.sendReport();
                }
            }                      
        }
        private async Task TakeYearAndCreateAnnualInfo()
        {
            
            var kvAnnualInfoStatus = await this.DataSourceService.GetKeyValueByIntCodeAsync("AnnualInfoStatusType", "Working");
            var year = DateTime.Now.Year;

            var dayMonthRangeForReportSetting = await DataSourceService.GetSettingByIntCodeAsync("DateRangeForReport");

            var dayMonth = dayMonthRangeForReportSetting.SettingValue.Split(".");

            var newDateTimeNow = DateTime.Now;

            var createFullRangeForReport = new DateTime(newDateTimeNow.Year, Convert.ToInt32(dayMonth[1]), Convert.ToInt32(dayMonth[0]), 00, 00, 00);

            if (createFullRangeForReport >= newDateTimeNow)
            {
                
                year = newDateTimeNow.AddYears(-1).Year;
            }

            var isAlreadySend = ArchiveService.GetAnnualInfoByIdCandProvAndYear(this.UserProps.IdCandidateProvider, year);
            if (isAlreadySend != null)
            {
                await this.ShowErrorAsync($"Вече имате създаден отчет за {DateTime.Now.Year} година!");
                this.isVisible = false;
                await ReloadData();
                
            }
            else
            {              

                AnnualInfoVM annualInfo = new AnnualInfoVM();

                annualInfo.IdCandidateProvider = this.UserProps.IdCandidateProvider;

                annualInfo.Year = year;

                var person = await this.personService.GetPersonByIdAsync(this.UserProps.IdPerson);

                annualInfo.Name = $"{person.FirstName} {person.FamilyName}";

                annualInfo.Title = person.Position;

                annualInfo.Phone = person.Phone;

                annualInfo.Email = person.Email;

                annualInfo.IdCreateUser = this.UserProps.UserId;

                annualInfo.CreationDate = DateTime.Now;

                annualInfo.IdModifyUser = this.UserProps.UserId;

                annualInfo.ModifyDate = DateTime.Now;

                annualInfo.IdStatus = kvAnnualInfoStatus.IdKeyValue;

                result = await ArchiveService.CreateAnnualInfoIdStatusAsync(annualInfo);

                result = await ArchiveService.SaveArchAnnualInfoStatus(annualInfo.IdAnnualInfo, kvAnnualInfoStatus.IdKeyValue, "");
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    this.ShowSuccessAsync(string.Join(Environment.NewLine, $"Успешно създаден отчет за {DateTime.Now.Year} година."));
                }           
            }
            await this.ReloadData();
            this.StateHasChanged();
        }
        private async Task ReloadData()
        {           
            this.archAnnualInfoSource = await this.ArchiveService.GetAllАnnualInfoForCandidateProviderAsync(this.UserProps.IdCandidateProvider);

            foreach (var archAnualInfo in this.archAnnualInfoSource)
            {
                if (archAnualInfo.AnnualInfoStatuses.Any())
                {
                    var infoStatus = archAnualInfo.AnnualInfoStatuses.OrderByDescending(x => x.CreationDate).First();
                    var kvAnualInfoStatus = this.kvAnualInfoStatusSource.FirstOrDefault(x => x.IdKeyValue == infoStatus.IdStatus);
                    archAnualInfo.StatusName = kvAnualInfoStatus.Name;
                    archAnualInfo.StatusIntCode = kvAnualInfoStatus.KeyValueIntCode;
                    archAnualInfo.CommentAnnualInfoStatus = infoStatus.Comment;
                    archAnualInfo.CreationDate = infoStatus.CreationDate;
                }                              
            }          
            await this.sfAnnualInfoGrid.Refresh();
            this.StateHasChanged();
        }
        private async Task SelectedAnnualInfo(AnnualInfoVM annualInfoVM)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
                if (!hasPermission) { return; }

                await this.annualInfoCpoCipoStatusList.OpenModal(annualInfoVM);
            }
            finally
            {
                this.loading = false;
            }
        }
        private async Task RowSelectingHandler()
        {
            var selectedAnnualRow = await this.sfAnnualInfoGrid.GetSelectedRecordsAsync();
            if (selectedAnnualRow.Any())
            {
                await this.sfAnnualInfoGrid.ClearRowSelectionAsync();
            }
        }
        private async Task RowSelectedHandler(RowSelectEventArgs<AnnualInfoVM> args)
        {
            this.selectedRow.Clear();
            this.selectedRow = await this.sfAnnualInfoGrid.GetSelectedRecordsAsync();

            rowAnnInfo = new AnnualInfoVM();
            rowAnnInfo = selectedRow.FirstOrDefault();
            selectedRows.Clear();
            selectedRows.Add(rowAnnInfo.CandidateProvider);
        }
        private async Task RowDeselectedHandler(RowDeselectEventArgs<AnnualInfoVM> args)
        {
            this.selectedRow.Clear();
            this.selectedRow = await this.sfAnnualInfoGrid.GetSelectedRecordsAsync();
        }

        private async Task OnBtnSelectHandler(MenuEventArgs args)
        {
            if (!this.selectedRow.Any())
            {
                await this.ShowErrorAsync("Моля, изберете ред!");
                return;
            }

            var btnText = args.Item.Text;
            switch (btnText)

            {
                case ("Материално-техническа база"):
                    await this.HandleMTBReport();
                    break;
                case ("Актуализиране на учебните планове и програми"):
                    await this.HandleCurriculumsReport();
                    break;
                case ("Курсисти А: Курсисти по дата на раждане"):
                    await this.HandleStudentReport("Курсисти А: Курсисти по дата на раждане");
                    break;
                case ("Курсисти Б: Курсисти по гражданство"):
                    await this.HandleStudentReport("Курсисти Б: Курсисти по гражданство");
                    break;
                case ("Повишаване на квалификацията на обучителите"):
                    await this.HandleTrainerQualificationsReport();
                    break;
                case ("Обучение A: курсове за придобиване на СПК"):
                    await this.HandleAnnualTrainingCourse("SPK");
                    break;
                case ("Обучение B: курсове по част от професията"):
                    await this.HandleAnnualTrainingCourse("PartProfession");
                    break;
                case ("Обучение C: придобили СПК чрез валидиране"):
                    await this.HandleAnnualTrainingValidationCourse();
                    break;
                case ("Обучение D: други курсове"):
                    await this.HandleAnnualTrainingCourse("OtherCourses");
                    break;
                case ("Обучение Е: курсове за правоспособност"):
                    await this.HandleAnnualTrainingCourse("CourseRegulation1And7");
                    break;
                default:
                    break;
            }
        }
        private async Task OnCIPOBtnSelectHandler(MenuEventArgs args)
        {
            if (!this.selectedRow.Any())
            {
                await this.ShowErrorAsync("Моля, изберете ред!");
                return;
            }

            var btnText = args.Item.Text;
            switch (btnText)

            {
                case ("Данни за клиентите на ЦИПО"):
                    await this.HandleClients("Данни за клиентите на ЦИПО");
                    break;
                case ("Услуги предлагани от ЦИПО"):
                    await this.HandleClients("Услуги предлагани от ЦИПО");
                    break;
                case ("Източници на финансиране"):
                    await this.HandleClients("Източници на финансиране");
                    break;
                default:
                    break;
            }
        }

            private async Task HandleAnnualTrainingCourse(string report)
        {
        

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

           
                await this.reporTrainingCourse.OpenModal(this.selectedRows, report, rowAnnInfo.Year.ToString());
                
            }
            finally
            {
                this.loading = false;
            }
        }
        private async Task HandleAnnualTrainingValidationCourse()
        {
         
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                await this.reportTrainingValidationCourse.OpenModal(this.selectedRows, rowAnnInfo.Year.ToString());

            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task HandleCurriculumsReport()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.curriculumModal.OpenModal(this.selectedRows, rowAnnInfo.Year, "CipoiliCPO");
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task HandleMTBReport()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.mtbModal.OpenModal(this.selectedRows, rowAnnInfo.Year, "CipoiliCPO");
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task HandleStudentReport(string page)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;


                if ( page == "Курсисти А: Курсисти по дата на раждане")
                {
                    await this.studentModal.OpenModal(this.selectedRows);
                }
                else if (page == "Курсисти Б: Курсисти по гражданство")
                {
                    await this.annualStudentByNationalityModal.OpenModal(this.selectedRows);
                }
            }
            finally
            {
                this.loading = false;
            }
        }
        private async Task HandleTrainerQualificationsReport()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;


                await this.trainerQualificationsModal.OpenModal(this.selectedRows, rowAnnInfo.Year, "CipoiliCPO");
            }
            finally
            {
                this.loading = false;
            }
        }
        private async Task HandleClients(string page)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;


                if (page == "Данни за клиентите на ЦИПО")
                {
                    await this.handleTotalConsultedClients.OpenModal(this.selectedRows, page, rowAnnInfo.Year);
                }
                else if (page == "Услуги предлагани от ЦИПО")
                {
                    await this.annualConsultingModal.OpenModal(this.selectedRows, page, rowAnnInfo.Year);
                }
                else if (page == "Източници на финансиране")
                {
                    await this.annualFinancing.OpenModal(this.selectedRows, page, rowAnnInfo.Year);
                }
            }
            finally
            {
                this.loading = false;
            }
        }
    
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfAnnualInfoGrid_pdfexport")
            {
                int temp = sfAnnualInfoGrid.PageSettings.PageSize;
                sfAnnualInfoGrid.PageSettings.PageSize = annualInfoSource.Count();
                await sfAnnualInfoGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "180", TextAlign = TextAlign.Left });
                if (this.LicensingType == "LicensingCPO")
                {
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "180", TextAlign = TextAlign.Left });
                }
                else if (this.LicensingType == "LicensingCIPO")
                {
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CIPONameOwnerGrid", HeaderText = "ЦИПО", Width = "180", TextAlign = TextAlign.Left });
                }
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Подал отчета", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Title", HeaderText = "Длъжност", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Year", HeaderText = "Отчет за година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StatusName", HeaderText = "Статус на отчета", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ModifyDate", HeaderText = "Дата на промяна на статуса", Format = "d", Width = "80", TextAlign = TextAlign.Left });                
                ExportColumns.Add(new GridColumn() { Field = "CommentAnnualInfoStatus", HeaderText = "Коментар", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"AnnualInfo_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".pdf";

                await this.sfAnnualInfoGrid.ExportToPdfAsync(ExportProperties);
                sfAnnualInfoGrid.PageSettings.PageSize = temp;
                await sfAnnualInfoGrid.Refresh();
            }
            else if (args.Item.Id == "sfAnnualInfoGrid_excelexport")
            {
                this.sfAnnualInfoGrid.AllowExcelExport = true;
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "90", TextAlign = TextAlign.Left });
                if (this.LicensingType == "LicensingCPO")
                {
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "180", TextAlign = TextAlign.Left });
                }
                else if (this.LicensingType == "LicensingCIPO")
                {
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CIPONameOwnerGrid", HeaderText = "ЦИПО", Width = "180", TextAlign = TextAlign.Left });
                }
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Подал отчета", Width = "150", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Title", HeaderText = "Длъжност", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Year", HeaderText = "Отчет за година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StatusName", HeaderText = "Статус на отчета", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ModifyDate", HeaderText = "Дата на промяна на статуса", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CommentAnnualInfoStatus", HeaderText = "Коментар", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"AnnualInfo_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".xlsx";
                await this.sfAnnualInfoGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<AnnualInfoVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfAnnualInfoGrid, args.Data.IdAnnualInfo).Result.ToString();
            }
        }

        public async Task sendReport()
        {

            var kvAnnInfoSubmittedStatus = await this.DataSourceService.GetKeyValueByIntCodeAsync("AnnualInfoStatusType", "Submitted");
            var kvAnnInfoApprovedStatus = await this.DataSourceService.GetKeyValueByIntCodeAsync("AnnualInfoStatusType", "Approved");
            var kvCipoLicense = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO");

            var isArchiveOrNot = "MakeIsArchiveTrue";

            if (selectedRow.Any())
            {           
                if (kvAnnInfoSubmittedStatus.IdKeyValue == rowAnnInfo.IdStatus || kvAnnInfoApprovedStatus.IdKeyValue == rowAnnInfo.IdStatus)
                {
                    await this.ShowErrorAsync($"Подадения отчет за {rowAnnInfo.Year} година не е на статус Работен или Върнат!");
                }
                else
                {                    
                    this.SpinnerShow();
                    result = await this.ArchiveService.UpdateFinishedCoursesStatusIsArchive(rowAnnInfo.IdCandidateProvider, isArchiveOrNot, rowAnnInfo.Year);
                    result = await this.ArchiveService.UpdateFinishedValidationClientCoursesStatusIsArchive(rowAnnInfo.IdCandidateProvider, isArchiveOrNot, rowAnnInfo.Year);
                    if (rowAnnInfo.CandidateProvider.IdTypeLicense == kvCipoLicense.IdKeyValue)
                    {
                        result = await this.ArchiveService.UpdateTrainingConsultingClientStatusIsArchive(rowAnnInfo.IdCandidateProvider, isArchiveOrNot);
                    }
                    result = await this.ArchiveService.DoAnnualArhiveToCandidateProvider(rowAnnInfo.IdCandidateProvider, rowAnnInfo.Year);
                    result = await this.ArchiveService.SaveArchAnnualInfoStatus(rowAnnInfo.IdAnnualInfo, kvAnnInfoSubmittedStatus.IdKeyValue,"");
                    rowAnnInfo.IdStatus = kvAnnInfoSubmittedStatus.IdKeyValue;
                    result = await this.ArchiveService.UpdateAnnualInfo(rowAnnInfo, "Submite");

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        this.SpinnerHide();
                        this.ShowSuccessAsync(string.Join(Environment.NewLine, $"Успешно подаден отчет за {rowAnnInfo.Year} година."));
                        await ReloadData();
                    }
                }   

                this.StateHasChanged();
            }
        }
    }
}

