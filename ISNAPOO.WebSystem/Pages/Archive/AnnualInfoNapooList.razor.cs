using System.Net.Mail;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Services.Mailing;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.WebSystem.Pages.Archive.CIPO;
using ISNAPOO.WebSystem.Pages.Archive.SelfAssessment;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Candidate.CIPO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Mail;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.SplitButtons;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualInfoNapooList : BlazorBaseComponent
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

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }


        private IEnumerable<CandidateProviderVM> annualInfoSource = new List<CandidateProviderVM>();
        public string LicensingType { get; set; }
        private CandidateProviderVM modelForFilterGrid { get; set; }
        private SfGrid<CandidateProviderVM> sfGrid = new SfGrid<CandidateProviderVM>();
        private ApplicationModal applicationModal = new ApplicationModal();
        private CIPOApplicationModal cipoApplicationModal = new CIPOApplicationModal();
        SendMailModal sendMailModal = new SendMailModal();
        private Model model = new Model();
        private List<CandidateProviderVM> selectedRows = new List<CandidateProviderVM>();
        private AnnualInfoReportDataModal reportDataModal = new AnnualInfoReportDataModal();
        private AnnualMTBModal mtbModal = new AnnualMTBModal();
        private AnnualStudentsModal studentModal = new AnnualStudentsModal();
        private AnnualTrainingValidationClientModal reportTrainingValidationCourse = new AnnualTrainingValidationClientModal();
        private AnnualTrainingCourseModal reporTrainingCourse = new AnnualTrainingCourseModal();
        private AnnualCurriculumModal curriculumModal = new AnnualCurriculumModal();
        private AnnualTrainerQualificationsModal trainerQualificationsModal = new AnnualTrainerQualificationsModal();
        private AnnualStudentByNationalityModal annualStudentByNationalityModal = new AnnualStudentByNationalityModal();
        private SelfAssessmentReportModal selfAssessmentReportModal = new SelfAssessmentReportModal();
        private AnnualInfoRejectModal annualInfoRejectModal = new AnnualInfoRejectModal();
        private AnnualInfoApproveModal annualInfoApproveModal = new AnnualInfoApproveModal();
        private HandleTotalConsultedClients handleTotalConsultedClients = new HandleTotalConsultedClients();
        private ISNAPOO.WebSystem.Pages.Archive.CIPO.AnnualConsulting annualConsultingModal = new ISNAPOO.WebSystem.Pages.Archive.CIPO.AnnualConsulting();
        private ToastMsg toast;
        private bool showFileInRequestConfirmDialog;
        //private bool isNAPOOEmployee = true;
        private IEnumerable<KeyValueVM> kvAnualInfoStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvSelfAssessmentStatusSource = new List<KeyValueVM>();

        protected override async Task OnInitializedAsync()
        {

            this.editContext = new EditContext(this.model);
            this.model.Year = DateTime.Now.Year - 1;
            modelForFilterGrid = new CandidateProviderVM();
            modelForFilterGrid.AnnualInfos.Add(new AnnualInfoVM() { Year = this.model.Year });
            this.LicensingType = string.Empty;
            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.Token = Token;
            tokenContext = BaseHelper.GetDecodeToken(tokenContext);
            this.LicensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "AnnualInfoType").Value.ToString();
            this.kvAnualInfoStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AnnualInfoStatusType");
            this.kvSelfAssessmentStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("SelfAssessmentReportStatus");
            showFileInRequestConfirmDialog = false;
        }
     
        private async Task UpdateAfterModalSubmit()
        {
           // await this.LoadReportsDataAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ReloadData();
            }
        }


        private async Task sendEmail()
        {
            await sendMailModal.openModal();
        }
        private async Task OpenRejectAnnualModal()
        {
            ResultContext<List<CandidateProviderVM>> currentContext = new ResultContext<List<CandidateProviderVM>>();
            currentContext.ResultContextObject.AddRange(sfGrid.SelectedRecords);
            // List<string> errorMessages = new List<string>();

            if (sfGrid.SelectedRecords.Count > GlobalConstants.INVALID_ID_ZERO)
            {
                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    //bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
                    //if (!hasPermission) { return; }


                    await this.annualInfoRejectModal.OpenModal(this.selectedRows, this.model.Year);
                }
                finally
                {
                    this.loading = false;
                }
            }
            else
            {
                toast.sfErrorToast.Content = "Моля първо изберете записи от таблицата!";
                await toast.sfErrorToast.ShowAsync();
            }
        }

        private async Task OpenApproveAnnualModal()
        {
            ResultContext<List<CandidateProviderVM>> currentContext = new ResultContext<List<CandidateProviderVM>>();
            currentContext.ResultContextObject.AddRange(sfGrid.SelectedRecords);

            if (sfGrid.SelectedRecords.Count > GlobalConstants.INVALID_ID_ZERO)
            {
                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    //bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
                    //if (!hasPermission) { return; }


                    await this.annualInfoApproveModal.OpenModal(this.selectedRows, this.model.Year);
                }
                finally
                {
                    this.loading = false;
                }
            }
            else
            {
                toast.sfErrorToast.Content = "Моля първо изберете записи от таблицата!";
                await toast.sfErrorToast.ShowAsync();
            }
        }

        private async Task sendEmails(MailMessage mail)
        {
            var rows = await sfGrid.GetSelectedRecordsAsync();

            foreach (var provider in rows)
            {
                if (provider.ProviderEmailCorrespondence != null)
                {
                    mail.To.Add(provider.ProviderEmailCorrespondence);
                }
            }
            await mailService.SendCustomEmail(mail);

            await this.ShowSuccessAsync("E-mail-ите са изпратени успешно!");
            await this.sfGrid.Refresh();
            this.SpinnerHide();
        }

        private async Task ReloadData()
        {
            if (this.model.Year.ToString().Length != 4)
            {
                await this.ShowErrorAsync("Моля, въведете валидна стойност в полето 'Година'!");
                return;
            }

            var kvAnualInfoStatusWorking = await this.DataSourceService.GetKeyValueByIntCodeAsync("AnnualInfoStatusType", "Working");
            var kvSelfAssessmentStatusCreated = await this.DataSourceService.GetKeyValueByIntCodeAsync("SelfAssessmentReportStatus", "Created");

            modelForFilterGrid = new CandidateProviderVM();
            modelForFilterGrid.AnnualInfos.Add(new AnnualInfoVM() { Year = this.model.Year });

            this.annualInfoSource = await this.ArchiveService.GetCandidateProviderАnnualInfoAndSelfAssessmentReportsAsync(this.modelForFilterGrid, this.UserProps.IdCandidateProvider);
            foreach (var candidateAnnualInfo in this.annualInfoSource)
            {
                var infoStatus = candidateAnnualInfo.AnnualInfos.FirstOrDefault(x => x.Year == model.Year && x.IdStatus != kvAnualInfoStatusWorking.IdKeyValue);
                if (infoStatus != null)
                {
                    var kvAnualInfoStatus = this.kvAnualInfoStatusSource.FirstOrDefault(x => x.IdKeyValue == infoStatus.IdStatus);
                    if (kvAnualInfoStatus != null)
                    {
                        candidateAnnualInfo.AnnualInfoStatusName = kvAnualInfoStatus.Name;
                        candidateAnnualInfo.AnnualInfoStatusIntCode = kvAnualInfoStatus.KeyValueIntCode;
                        candidateAnnualInfo.AnnualInfoDate = infoStatus.ModifyDate;
                    }
                    else
                    {
                        candidateAnnualInfo.SelfAssessmentReportStatusName = string.Empty;
                        candidateAnnualInfo.SelfAssessmentReportStatusIntCode = string.Empty;
                    }
                }
                else
                {
                    candidateAnnualInfo.AnnualInfoStatusName = string.Empty;
                    candidateAnnualInfo.AnnualInfoStatusIntCode = string.Empty;
                }

                var selfAssessmentReport = candidateAnnualInfo.SelfAssessmentReports.FirstOrDefault(x => x.Year == model.Year);
                if (selfAssessmentReport != null)
                {
                  //  var lastSelfAssessStatus = selfAssessmentReport.SelfAssessmentReportStatuses.OrderByDescending(x => x.CreationDate).First();

                    if (selfAssessmentReport.IdStatus != kvSelfAssessmentStatusCreated.IdKeyValue)
                    {
                        var kvlastSelfAssessStatus = this.kvSelfAssessmentStatusSource.FirstOrDefault(x => x.IdKeyValue == selfAssessmentReport.IdStatus);
                        if (kvlastSelfAssessStatus != null)
                        {
                            candidateAnnualInfo.SelfAssessmentReportStatusName = kvlastSelfAssessStatus.Name;
                            candidateAnnualInfo.SelfAssessmentReportStatusIntCode = kvlastSelfAssessStatus.KeyValueIntCode;
                            candidateAnnualInfo.SelfAssessmentReportStatusDate = selfAssessmentReport.ModifyDate;
                        }
                        else
                        {
                            candidateAnnualInfo.SelfAssessmentReportStatusName = string.Empty;
                            candidateAnnualInfo.SelfAssessmentReportStatusIntCode = string.Empty;
                        }
                    }                    
                }
                else
                {
                    candidateAnnualInfo.SelfAssessmentReportStatusName = string.Empty;
                    candidateAnnualInfo.SelfAssessmentReportStatusIntCode = string.Empty;

                }

            }

            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.Token = Token;
            tokenContext = BaseHelper.GetDecodeToken(tokenContext);
            this.LicensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "AnnualInfoType").Value.ToString();
         
            if (this.LicensingType == "InfoNAPOOCIPO")
            {
               var kvLicense = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO");
               this.annualInfoSource = this.annualInfoSource.Where(x => x.IdTypeLicense == kvLicense.IdKeyValue);
            }
            else if (this.LicensingType == "InfoNAPOOCPO")
            {
                    var kvLicense = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
                    this.annualInfoSource = this.annualInfoSource.Where(x => x.IdTypeLicense == kvLicense.IdKeyValue);
            }
            this.annualInfoSource = this.annualInfoSource.OrderBy(x => x.LicenceNumberString);

            //this.isNAPOOEmployee = this.UserProps.IdCandidateProvider == null || this.UserProps.IdCandidateProvider == 0;
            await this.sfGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task OpenProfileModalBtn(CandidateProviderVM candidateProviderVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.applicationModal.OpenModal(new CandidateProviderVM() { IdCandidate_Provider = candidateProviderVM.IdCandidate_Provider }, false, false);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenCipoApplicationModal(CandidateProviderVM candidateProviderVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.applicationModal.OpenModal(candidateProviderVM, false, false);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        private async Task OpenSelfAssessmentReportListModal(CandidateProviderVM candidateProviderVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var idSelfAssessment = candidateProviderVM.SelfAssessmentReports.FirstOrDefault(x => x.Year == model.Year) != null ? candidateProviderVM.SelfAssessmentReports.FirstOrDefault(x => x.Year == model.Year).IdSelfAssessmentReport : 0;
                if (idSelfAssessment != 0)
                {
                    var report = await this.ArchiveService.GetSelfAssessmentReportByIdAsync(idSelfAssessment);
                    await this.selfAssessmentReportModal.OpenModal(report);
                }
                else
                {
                    await this.ShowErrorAsync("Няма подаден годишен доклад за самооценка!");
                }
            }
            finally
            {
                this.loading = false;
               
            }

            this.SpinnerHide();
        }
        private async Task RowDeselectedHandler(RowDeselectEventArgs<CandidateProviderVM> args)
        {
            this.selectedRows.Clear();
            this.selectedRows = await this.sfGrid.GetSelectedRecordsAsync();
        }

        private async Task RowSelectedHandler(RowSelectEventArgs<CandidateProviderVM> args)
        {
            this.selectedRows.Clear();
            this.selectedRows = await this.sfGrid.GetSelectedRecordsAsync();
        }

        private async Task OnBtnSelectHandler(MenuEventArgs args)
        {
            if (!this.selectedRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете ред/редове!");
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
            if (!this.selectedRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете ред/редове!");
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
                default:
                    break;
            }
        }
        private async Task HandleAnnualTrainingValidationCourse()
        {
            if (this.model.Year == 0 || this.model.Year.ToString().Length != 4)
            {
                await this.ShowErrorAsync("Моля, въведете валидна стойност в полето 'Година'!");
                return;
            }

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                await this.reportTrainingValidationCourse.OpenModal(this.selectedRows, this.model.Year.ToString());

            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task HandleAnnualTrainingCourse(string report)
        {
            if (this.model.Year == 0 || this.model.Year.ToString().Length != 4)
            {
                await this.ShowErrorAsync("Моля, въведете валидна стойност в полето 'Година'!");
                return;
            }

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                await this.reporTrainingCourse.OpenModal(this.selectedRows, report, this.model.Year.ToString());

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

                if (this.model.Year.ToString().Length != 4)
                {
                    await this.ShowErrorAsync("Моля, въведете валидна стойност в полето 'Година'!");
                    return;
                }

                this.curriculumModal.OpenModal(this.selectedRows, this.model.Year, this.LicensingType);
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

                await this.mtbModal.OpenModal(this.selectedRows, this.model.Year, this.LicensingType);
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
                if (page == "Курсисти А: Курсисти по дата на раждане")
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

                if (this.model.Year.ToString().Length != 4)
                {
                    await this.ShowErrorAsync("Моля, въведете валидна стойност в полето 'Година'!");
                    return;
                }

                await this.trainerQualificationsModal.OpenModal(this.selectedRows, this.model.Year, this.LicensingType);
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
                    await this.handleTotalConsultedClients.OpenModal(this.selectedRows, page, this.model.Year);
                }
                else if (page == "Услуги предлагани от ЦИПО")
                {
                    await this.annualConsultingModal.OpenModal(this.selectedRows, page, this.model.Year);
                }
            }
            finally
            {
                this.loading = false;
            }
        }
        private class Model
        {
            public int Year { get; set; }
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = annualInfoSource.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "LicenceNumber", HeaderText = "Лицензия", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "LocationCorrespondence.LocationName", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "AnnualInfoStatusName", HeaderText = "Статус на годишния отчет", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "AnnualInfoDate", HeaderText = "Дата на отчета", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SelfAssessmentReportStatusName", HeaderText = "Статус на доклада за самооценка", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SelfAssessmentReportStatusDate", HeaderText = "Дата на доклада", Format = "d", Width = "80", TextAlign = TextAlign.Left });
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

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                this.sfGrid.AllowExcelExport = true;
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "LicenceNumber", HeaderText = "Лицензия", Width = "90", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CPONameOwnerGrid", HeaderText = "ЦПО", Width = "150", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "LocationCorrespondence.LocationName", HeaderText = "Населено място", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "AnnualInfoStatusName", HeaderText = "Статус на годишния отчет", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "AnnualInfoDate", HeaderText = "Дата на отчета", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SelfAssessmentReportStatusName", HeaderText = "Статус на доклада за самооценка", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SelfAssessmentReportStatusDate", HeaderText = "Дата на доклада", Format = "d", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"AnnualInfo_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<CandidateProviderVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdCandidate_Provider).Result.ToString();
            }
        }             
    }
}
