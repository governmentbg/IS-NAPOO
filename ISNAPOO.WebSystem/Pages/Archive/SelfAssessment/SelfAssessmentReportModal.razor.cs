using System.Collections.Generic;
using System.Drawing.Printing;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Services.Rating;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Assessment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;

namespace ISNAPOO.WebSystem.Pages.Archive.SelfAssessment
{
    public partial class SelfAssessmentReportModal : BlazorBaseComponent, IConcurrencyCheck<SelfAssessmentReportVM>
    {
        private SelfAssessmentReportMainData selfAssessmentReportMainData = new SelfAssessmentReportMainData();

        private SelfAssessmentFilingOut selfAssessmentFilingOut = new SelfAssessmentFilingOut();

        private List<SelfAssessmentSummaryProfessionalTrainingVM> summarySource = new List<SelfAssessmentSummaryProfessionalTrainingVM>();
        private List<SelfAssessmentSummaryProfessionalTrainingVM> summarySourceGroup1 = new List<SelfAssessmentSummaryProfessionalTrainingVM>();
        private List<SelfAssessmentSummaryProfessionalTrainingVM> summarySourceGroup2 = new List<SelfAssessmentSummaryProfessionalTrainingVM>();
        private List<SelfAssessmentSummaryProfessionalTrainingVM> summarySourceGroup3 = new List<SelfAssessmentSummaryProfessionalTrainingVM>();
        private SfGrid<SelfAssessmentSummaryProfessionalTrainingVM> summaryGrid1;
        private SfGrid<SelfAssessmentSummaryProfessionalTrainingVM> summaryGrid2;
        private SfGrid<SelfAssessmentSummaryProfessionalTrainingVM> summaryGrid3;
        private string[] Group = (new string[] { "ProfessionalTrainingIndicatorGroup" });

        private SurveyResultVM surveyResultVM = new SurveyResultVM();
        private SelfAssessmentApproveRejectModal selfAssessmentApproveRejectModal = new SelfAssessmentApproveRejectModal();
        private SelfAssessmentReportVM selfAssessmentReportVM = new SelfAssessmentReportVM();
        private List<string> validationMessages = new List<string>();
        private int selectedTab = 0;   
        private bool hideBtnsConcurrentModal = false;
        private bool hideFileInReportBtn = false;
        private bool hideSubmitBtn = false;
        private bool showAppRejBtn = false;
        private IEnumerable<KeyValueVM> kvSelfAssessmentReportSource = new List<KeyValueVM>();
        private KeyValueVM keyValueSubmited = new KeyValueVM();
        private string title = string.Empty;
        private string cpoCipoName = string.Empty;
        private bool showTabTrainingConsulting = false;

        private CandidateProviderVM currentCandidateProviderVM { get; set; }



        public override bool IsContextModified => this.selfAssessmentReportMainData.IsEditContextModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IArchiveService ArchiveService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        [Inject]
        public IRatingService ratingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        private string tabTitleTrainingConsulting = string.Empty;

        public KeyValueVM kvTypeLicense { get; set; }
        protected override async void OnInitialized()
        {
            this.editContext = new EditContext(this.selfAssessmentReportVM); 
        }

         

        public async Task OpenModal(SelfAssessmentReportVM selfAssessmentReport, ConcurrencyInfo concurrencyInfo = null)
        {

            summarySource.Clear();
            this.selectedTab = 0;
            
            this.selfAssessmentReportVM = selfAssessmentReport;
            this.IdSelfAssessmentReport = this.selfAssessmentReportVM.IdSelfAssessmentReport;
             
            this.selfAssessmentFilingOut.SelfAssessmentReportVM = this.selfAssessmentReportVM;

            this.currentCandidateProviderVM = await this.CandidateProviderService.GetOnlyCandidateProviderByIdAsync(selfAssessmentReport.IdCandidateProvider);
            
            this.kvTypeLicense = await this.DataSourceService.GetKeyValueByIdAsync(this.currentCandidateProviderVM.IdTypeLicense);

            this.cpoCipoName = string.Empty;

            if (this.kvTypeLicense.KeyValueIntCode == "LicensingCPO")
            {
                this.showTabTrainingConsulting = true;
                this.tabTitleTrainingConsulting = "Обща информация за проведено ПО и Валидиране";
                this.cpoCipoName = this.currentCandidateProviderVM.CPONameAndOwner;
            }
            else if (this.kvTypeLicense.KeyValueIntCode == "LicensingCIPO")
            {
                this.showTabTrainingConsulting = false;
                this.tabTitleTrainingConsulting = "Обща информация за консултирани лица";
                this.cpoCipoName = this.currentCandidateProviderVM.CIPONameAndOwner;
            }
            else {
                this.showTabTrainingConsulting = false;
                this.tabTitleTrainingConsulting = "Обща информация";
            }

            await this.IsShowApproveRejectBtns();

            this.validationMessages.Clear();

            await this.SetCreateAndModifyInfoAsync();

            this.kvSelfAssessmentReportSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("SelfAssessmentReportStatus");
            this.HandleDisabledStatesAfterReportStatus();

            this.editContext = new EditContext(this.selfAssessmentReportVM);

            this.SetTitle();

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

            var indicators = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType");


            if (this.kvTypeLicense.KeyValueIntCode == "LicensingCPO")
            {
                await LoadSummarySourceCPOAsync();
            }
            else if (this.kvTypeLicense.KeyValueIntCode == "LicensingCIPO")
            {
                await LoadSummarySourceCIPOAsync();
            }         
        }
        private async Task IsShowApproveRejectBtns()
        {
            this.showAppRejBtn = false;
            this.keyValueSubmited = await this.DataSourceService.GetKeyValueByIntCodeAsync("SelfAssessmentReportStatus", "Submitted");

            if (this.selfAssessmentReportVM.IdStatus == keyValueSubmited.IdKeyValue && this.UserProps.IdCandidateProvider == 0)
            {
                this.showAppRejBtn = true;
            }
        }
        private async Task LoadSummarySourceCPOAsync() 
        {

            summarySource.Clear();
            var count = await this.ratingService.CountDegreeProfessionalQualificationAsync(this.selfAssessmentReportVM.IdCandidateProvider, this.selfAssessmentReportVM.Year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 1,
                ProfessionalTrainingIndicatorName = "придобили степен на професионална квалификация",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });


            count = await this.ratingService.CountProfessionalQualificationPartProfession(this.selfAssessmentReportVM.IdCandidateProvider, this.selfAssessmentReportVM.Year);

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 2,
                ProfessionalTrainingIndicatorName = "придобили професионална квалификация по част от професията",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });


            count = await this.ratingService.CountDegreeProfessionalQualificationValidation(this.selfAssessmentReportVM.IdCandidateProvider, this.selfAssessmentReportVM.Year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {

                ProfessionalTrainingIndicatorId = 3,
                ProfessionalTrainingIndicatorName = "придобили степен на професионална квалификация чрез валидиране",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });


            count = await this.ratingService.CountProfessionalQualificationPartProfessionValidation(this.selfAssessmentReportVM.IdCandidateProvider, this.selfAssessmentReportVM.Year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {

                ProfessionalTrainingIndicatorId = 4,
                ProfessionalTrainingIndicatorName = "придобили професионална квалификация по част от професията чрез валидиране",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });


            ///TODO: Да се смята стойността на "получили правоспособност"
            count = await this.ratingService.CountCourseCompetenceUnderRegulationsOneAndSeven(this.selfAssessmentReportVM.IdCandidateProvider, this.selfAssessmentReportVM.Year);

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {

                ProfessionalTrainingIndicatorId = 5,
                ProfessionalTrainingIndicatorName = "получили правоспособност",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });



            count = await this.ratingService.CountProfessionalEducationFollowingNextYear(this.selfAssessmentReportVM.IdCandidateProvider, this.selfAssessmentReportVM.Year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {

                ProfessionalTrainingIndicatorId = 5,
                ProfessionalTrainingIndicatorName = "продължаващи професионално обучение през следващата година",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });

            count = await this.ratingService.CountNonSuccessfulVocationalTraining(this.selfAssessmentReportVM.IdCandidateProvider, this.selfAssessmentReportVM.Year);

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 6,
                ProfessionalTrainingIndicatorName = "незавършили успешно професионалното обучение:",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });


            count = await this.ratingService.CountAbandonedTrainingStarted(this.selfAssessmentReportVM.IdCandidateProvider, this.selfAssessmentReportVM.Year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 7,
                ProfessionalTrainingIndicatorName = "отказали се от започнатото професионално обучение:",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });

            summarySourceGroup1 = summarySource.Where(x => x.ProfessionalTrainingIndicatorGroup == "Group1").ToList();
            summarySourceGroup2 = summarySource.Where(x => x.ProfessionalTrainingIndicatorGroup == "Group2").ToList();
        }

        private async Task LoadSummarySourceCIPOAsync()
        {

            summarySource.Clear();

            var count = 0;

           
            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 1,
                ProfessionalTrainingIndicatorName = "Брой клиенти с националност България",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = 7
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 2,
                ProfessionalTrainingIndicatorName = "Брой клиенти с националност страна-членка от Европейския съюз",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = 2
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 3,
                ProfessionalTrainingIndicatorName = "Брой клиенти с националност трета страна",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = 1
            });


           
             

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 1,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Информиране и самоинформиране“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 1
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 2,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Кариерно консултиране“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 3
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 3,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Оценка на случай“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 2
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 4,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Активиране и мотивиране“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 5,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Психологическо подпомагане“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 2
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 6,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Застъпничество“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 4
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 7,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Групи за взаимопомощ“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 8,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Управление на таланти“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 9,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Менторство“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 1
            });


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 10,
                ProfessionalTrainingIndicatorName = "Средна възраст",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 42
            });


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 11,
                ProfessionalTrainingIndicatorName = "Средна стойност на услугата в български лева",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 642
            });



            summarySourceGroup1 = summarySource.Where(x => x.ProfessionalTrainingIndicatorGroup == "Group1").ToList();
            summarySourceGroup2 = summarySource.Where(x => x.ProfessionalTrainingIndicatorGroup == "Group2").ToList();
            summarySourceGroup3 = summarySource.Where(x => x.ProfessionalTrainingIndicatorGroup == "Group3").ToList();
        }

        private void SetTitle()
        {
            this.title = this.selfAssessmentReportVM.IdSelfAssessmentReport != 0 ? $"Данни за доклад за самооценка на <span style =\"color: #ffffff;\">{this.cpoCipoName}</span> за <span style =\"color: #ffffff;\">{this.selfAssessmentReportVM.Year} г.</span>, Статус <span style =\"color: #ffffff;\">{this.selfAssessmentReportVM.Status}</span>" : "Данни за доклад за самооценка";
        }

        private void HandleDisabledStatesAfterReportStatus()
        {


            if (this.UserProps.IdCandidateProvider == 0) 
            {
                this.hideFileInReportBtn = true;
                this.hideBtnsConcurrentModal = true;
                this.hideSubmitBtn = true;
            }
            else if (this.selfAssessmentReportVM.SelfAssessmentReportStatuses.Any())
            {
                var lastStatus = this.selfAssessmentReportVM.SelfAssessmentReportStatuses.LastOrDefault()!;
                var kvApproved = this.kvSelfAssessmentReportSource.FirstOrDefault(x => x.KeyValueIntCode == "Approved")!;
                var kvSubmitted = this.kvSelfAssessmentReportSource.FirstOrDefault(x => x.KeyValueIntCode == "Submitted")!;
                if (lastStatus.IdStatus == kvApproved.IdKeyValue || lastStatus.IdStatus == kvSubmitted.IdKeyValue)
                {
                    this.hideFileInReportBtn = true;
                    this.hideSubmitBtn = true;
                }
                else
                {
                    this.hideFileInReportBtn = false;
                    this.hideSubmitBtn = false;
                }
            }
            else
            {
                this.hideFileInReportBtn = true;
                this.hideSubmitBtn = false;
            }
        }

        private void Select(SelectingEventArgs args)
        {
            if (args.IsSwiped)
            {
                args.Cancel = true;
            }
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.selfAssessmentReportVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.selfAssessmentReportVM.IdModifyUser);
            this.selfAssessmentReportVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.selfAssessmentReportVM.IdCreateUser);
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
                this.selfAssessmentReportMainData.SubmitHandler();
                this.validationMessages.AddRange(this.selfAssessmentReportMainData.GetValidationMessages());

                this.selfAssessmentFilingOut.SaveHandler();
                this.validationMessages.AddRange(this.selfAssessmentFilingOut.GetValidationMessages());

                if (!this.validationMessages.Any())
                {
                    var result = new ResultContext<SelfAssessmentReportVM>();
                    result.ResultContextObject = this.selfAssessmentReportVM;
                    if (this.selfAssessmentReportVM.IdSelfAssessmentReport == 0)
                    {
                        result = await this.ArchiveService.CreateSelfAssessmentReportAsync(result);

                    }
                    else
                    {
                        result = await this.ArchiveService.UpdateSelfAssessmentReportAsync(result);
                    }

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

                        this.IdSelfAssessmentReport = this.selfAssessmentReportVM.IdSelfAssessmentReport;

                        this.SetTitle();

                        this.selfAssessmentReportMainData.RealoadStatusGrid();

                        this.HandleDisabledStatesAfterReportStatus();

                        await this.SetCreateAndModifyInfoAsync();

                        await this.CallbackAfterSubmit.InvokeAsync();
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

        private async Task CallbackAfterRejectApprove() 
        {
            await this.CallbackAfterSubmit.InvokeAsync();
            List<SelfAssessmentReportStatusVM> selfAssessmentReportStatusVMs = new List<SelfAssessmentReportStatusVM>();
            selfAssessmentReportStatusVMs = await this.ArchiveService.GetSelfAssessmentReportStatuses(selfAssessmentReportVM.IdSelfAssessmentReport);
            selfAssessmentReportVM.SelfAssessmentReportStatuses = selfAssessmentReportStatusVMs;

            foreach (var status in selfAssessmentReportVM.SelfAssessmentReportStatuses)
            {
                var kvStatus = await this.DataSourceService.GetKeyValueByIdAsync(status.IdStatus);
                status.StatusValue = kvStatus.Name;
                status.StatusValueIntCode = kvStatus.KeyValueIntCode; ;

                status.StatusDate = status.ModifyDate;
                status.PersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(status.IdCreateUser);

                var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var fileFullName = settingResource + "\\UploadedFiles\\SelfAssessmentReportStatus\\" + selfAssessmentReportVM.IdSelfAssessmentReport;
                if (Directory.Exists(fileFullName))
                {
                    var files = Directory.GetFiles(fileFullName);
                    files = files.Select(x => x.Split(($"\\{selfAssessmentReportVM.IdSelfAssessmentReport}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                    status.FileName = string.Join(Environment.NewLine, files);
                }
            }
                this.showAppRejBtn = false;
        }

        private async Task FileInReportBtn()
        {
            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;
               
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да подадете доклада за самооценка към НАПОО? След подаване на доклада за самооценка към НАПОО, няма да може да извършвате промени в попълнената информация.");
                if (isConfirmed)
                {
                    this.validationMessages.Clear();
                    this.selfAssessmentReportMainData.SubmitHandler();
                    this.validationMessages.AddRange(this.selfAssessmentReportMainData.GetValidationMessages());

                    //this.selfAssessmentFilingOut.SaveHandler();
                    this.selfAssessmentFilingOut.SubmitHandler();
                    this.validationMessages.AddRange(this.selfAssessmentFilingOut.GetValidationMessages());
                    //await this.SubmitBtn(false);

                    if (!this.validationMessages.Any())
                    {
                        var inputContext = new ResultContext<SelfAssessmentReportVM>();
                        inputContext.ResultContextObject = this.selfAssessmentReportVM;
                        inputContext = await this.ArchiveService.FileInSelfAssessmentReportAsync(inputContext);
                        if (inputContext.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join(Environment.NewLine, inputContext.ListErrorMessages));
                        }
                        else
                        {
                            this.isVisible = false;

                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, inputContext.ListMessages));

                            this.HandleDisabledStatesAfterReportStatus();

                            await this.CallbackAfterSubmit.InvokeAsync();
                        }
                    }
                }
            }
            finally
            {
                this.loading = false;
            }
        }

        private void UpdateAfterSurveyModalSubmit()
        {
           
            this.StateHasChanged();
        }

        private async Task OpenRejectSelfAssModal()
        {
            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;
                await this.selfAssessmentApproveRejectModal.OpenModal(this.selfAssessmentReportVM, "Reject", null);
            
            }
            finally
            {
                this.loading = false;
            }
        }
        private async Task OpenApproveSelfAssModal()
        {
            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                await this.selfAssessmentApproveRejectModal.OpenModal(this.selfAssessmentReportVM, "Approve", null);
            }
            finally
            {
                this.loading = false;
            }
        }
    }
}
