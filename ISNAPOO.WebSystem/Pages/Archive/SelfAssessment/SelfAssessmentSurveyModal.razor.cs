using Data.Models.Data.Assessment;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ISNAPOO.WebSystem.Pages.Archive.SelfAssessment
{
    public partial class SelfAssessmentSurveyModal : BlazorBaseComponent, IConcurrencyCheck<SurveyVM>
    {
        private SelfAssessmentQuestionModal questionModal = new SelfAssessmentQuestionModal();
        private SelfAssessmentSurveyEmailTemplateModal surveyEmailTemplateModal = new SelfAssessmentSurveyEmailTemplateModal();

        private SurveyVM surveyVM = new SurveyVM();
        private bool hideBtnsConcurrentModal = false;
        private List<string> validationMessages = new List<string>();
        private IEnumerable<KeyValueVM> kvSurveyTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvQuestionTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvFrameworkProgramSource = new List<KeyValueVM>();
        private ValidationMessageStore? messageStore;
        private KeyValueVM kvOpenQuestion = new KeyValueVM();
        private KeyValueVM kvSingleOpenQuestion = new KeyValueVM();
        private KeyValueVM kvMultipleOpenQuestion = new KeyValueVM();
        private KeyValueVM kvSurveyStatusCreated = new KeyValueVM();
        private string openQuestionText = "< тук ще е отговорът на отворения въпрос >";
        private QuestionVM questionToDelete = new QuestionVM();
        private bool sendSurveyConfirmDlg = false;
        private bool sendSurveyConfirmed = false;
        private int questionCounter = 1;
        private bool ifSelfAssessmentSurveyReady = false;
        private bool copySurveyConfirmed = false;
        private bool copySurveyConfirmDlg = false;
        private Int32  copyYear = DateTime.Now.Year;
        private bool ifSelfAssessmentSurveyForCPO = false;

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.surveyVM);
            
        }

        public async Task OpenModal(SurveyVM survey, ConcurrencyInfo concurrencyInfo = null)
        {
            var kvSurveyTarget = await this.DataSourceService.GetKeyValueByIdAsync(survey.IdSurveyTarget);

            if (kvSurveyTarget.KeyValueIntCode == "ForCPO")
            {
                ifSelfAssessmentSurveyForCPO = true;
            }
            


            this.validationMessages.Clear();
            ifSelfAssessmentSurveyReady = false;
            foreach (var q in survey.Questions) 
            {
                q.AreaSelfAssessment = await this.DataSourceService.GetKeyValueByIdAsync(q.IdAreaSelfAssessment);
            }

            if (survey.SurveyResults.Any())
            {
                ifSelfAssessmentSurveyReady = true;
            }

            this.surveyVM = survey;

            await this.LoadKVDataAsync();
             
            await this.SetCreateAndModifyInfoAsync();

            if (this.surveyVM.IdSurvey != 0)
            {
                this.IdSurvey = this.surveyVM.IdSurvey;
            }
            else
            {
                this.surveyVM.IdSurveyStatus = this.kvSurveyStatusCreated.IdKeyValue;
            }

            this.editContext = new EditContext(this.surveyVM);

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

        private async Task LoadKVDataAsync()
        {
            this.kvSurveyTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("SurveyType");
            this.kvQuestionTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("QuestionType");
            this.kvOpenQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Open");
            this.kvSingleOpenQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "SingleOpen");
            this.kvMultipleOpenQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "MultipleOpen");
            this.kvFrameworkProgramSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram"))
                .Where(x => x.DefaultValue3 != null)
                .OrderBy(x => x.Name)
                .ToList();

            this.kvSurveyStatusCreated = await this.DataSourceService.GetKeyValueByIntCodeAsync("SurveyStatusType", "Created");
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

                this.editContext = new EditContext(this.surveyVM);
                this.editContext.EnableDataAnnotationsValidation();
                this.editContext.OnValidationRequested += this.ValidateTrainingFromDate;
                this.editContext.OnValidationRequested += this.ValidateStartDate;
                this.editContext.OnValidationRequested += this.ValidateEndDate;
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.validationMessages.Clear();
                this.editContext.Validate();
                this.validationMessages.AddRange(this.editContext.GetValidationMessages());

                if (!this.validationMessages.Any())
                {
                    var result = new ResultContext<SurveyVM>();
                    result.ResultContextObject = this.surveyVM;
                    if (this.surveyVM.IdSurvey == 0)
                    {
                        result = await this.AssessmentService.CreateSurveyAsync(result);
                    }
                    else
                    {
                        result = await this.AssessmentService.UpdateSurveyAsync(result);
                    }

                    if (!result.HasErrorMessages)
                    {
                        if (showToast)
                        {
                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                        }

                        await this.SetCreateAndModifyInfoAsync();

                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                    else
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
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

        private async Task SendSurveyBtn()
        {
            if (this.surveyVM.EndDate.HasValue && this.surveyVM.EndDate.Value.Date <= DateTime.Now.Date)
            {
                await this.ShowErrorAsync($"Не можете да изпратите анкета за попълване с 'Дата на активност до' по-рано от {DateTime.Now.AddDays(1).Date.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                return;
            }

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                this.sendSurveyConfirmDlg = !this.sendSurveyConfirmDlg;

                if (this.sendSurveyConfirmed)
                {
                    this.sendSurveyConfirmed = false;
                    await this.SubmitBtn(false);

                    if (!this.editContext.GetValidationMessages().Any())
                    {
                        this.surveyEmailTemplateModal.OpenModal(this.surveyVM);
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void ResendSurveyBtn()
        {
            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                this.surveyEmailTemplateModal.OpenModal(this.surveyVM, true);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.surveyVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.surveyVM.IdModifyUser);
            this.surveyVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.surveyVM.IdCreateUser);
        }

        private async Task AddQuestionBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var questionOrder = await this.AssessmentService.GetNextQuestionOrderByIdSurveyAsync(this.surveyVM.IdSurvey);
                var question = new QuestionVM()
                {
                    IdSurvey = this.surveyVM.IdSurvey,
                    Order = questionOrder
                };

                await this.questionModal.OpenModal(question);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterQuestionModalSubmitAsync()
        {
            this.surveyVM = await this.AssessmentService.GetSurveyByIdAsync(this.surveyVM.IdSurvey);
            await this.SetCreateAndModifyInfoAsync();


            foreach (var q in surveyVM.Questions)
            {
                q.AreaSelfAssessment = await this.DataSourceService.GetKeyValueByIdAsync(q.IdAreaSelfAssessment);
            }
            this.surveyVM.TrainingPeriodFrom = new DateTime(this.surveyVM.Year.Value, 1, 1);
            this.surveyVM.TrainingPeriodTo = new DateTime(this.surveyVM.Year.Value, 12, 31);
            this.surveyVM.StartDate = new DateTime(this.surveyVM.Year.Value, 1, 1);
            this.surveyVM.EndDate = new DateTime(this.surveyVM.Year.Value, 12, 31);
                    


            this.StateHasChanged();
        }

        private string GetSymbolByOrder(int order)
        {
            string symbol = string.Empty;
            if (order == 0)
            {
                symbol = "A";
            }
            else if (order == 1)
            {
                symbol = "Б";
            }
            else if (order == 2)
            {
                symbol = "В";
            }
            else if (order == 3)
            {
                symbol = "Г";
            }
            else if (order == 4)
            {
                symbol = "Д";
            }
            else if (order == 5)
            {
                symbol = "Е";
            }
            else if (order == 6)
            {
                symbol = "Ж";
            }
            else if (order == 7)
            {
                symbol = "З";
            }
            else if (order == 8)
            {
                symbol = "И";
            }
            else if (order == 9)
            {
                symbol = "Й";
            }
            else if (order == 10)
            {
                symbol = "К";
            }
            else if (order == 11)
            {
                symbol = "Л";
            }
            else if (order == 12)
            {
                symbol = "М";
            }
            else if (order == 13)
            {
                symbol = "Н";
            }
            else if (order == 14)
            {
                symbol = "О";
            }
            else if (order == 15)
            {
                symbol = "П";
            }
            else if (order == 16)
            {
                symbol = "Р";
            }
            else if (order == 17)
            {
                symbol = "С";
            }

            return symbol;
        }

        private async Task DeleteQuestionBtn(QuestionVM question)
        {
            bool isConfirmed = await this.ShowConfirmDialogAsync("Внимание!\r\nСигурни ли сте, че искате да изтриете избрания запис?");         
            this.questionToDelete = question;

            if (isConfirmed)
            {
                var result = await this.AssessmentService.DeleteQuestionByIdAsync(question.IdQuestion);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    await this.UpdateAfterQuestionModalSubmitAsync();
                }
            }
        }

        private async Task EditQuestionBtn(QuestionVM question)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var questionFromDb = await this.AssessmentService.GetQuestionByIdAsync(question.IdQuestion);
                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(question.IdQuestion, "Question");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(question.IdQuestion, "Question");
                }

                await this.questionModal.OpenModal(questionFromDb, concurrencyInfoValue);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void CloseAfterEmailTemplateModalSubmit()
        {
            this.isVisible = false;
        }

        private void ValidateTrainingFromDate(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.surveyVM.TrainingPeriodFrom.HasValue && this.surveyVM.TrainingPeriodTo.HasValue)
            {
                if (this.surveyVM.TrainingPeriodFrom.Value > this.surveyVM.TrainingPeriodTo.Value)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.surveyVM, "TrainingPeriodFrom");
                    this.messageStore?.Add(fi, $"Полето 'Период на обучение от' може да има стойност само преди {this.surveyVM.TrainingPeriodTo.Value.ToString(GlobalConstants.DATE_FORMAT)} г.! ('Период на обучение до')");
                }
            }
        }

        private void ValidateStartDate(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.surveyVM.StartDate.HasValue && this.surveyVM.EndDate.HasValue)
            {
                if (this.surveyVM.StartDate.Value > this.surveyVM.EndDate.Value)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.surveyVM, "StartDate");
                    this.messageStore?.Add(fi, $"Полето 'Дата на активност от' може да има стойност само преди {this.surveyVM.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г. ('Дата на активност до')!");
                }
            }
        }

        private void ValidateEndDate(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.surveyVM.StartDate.HasValue && this.surveyVM.EndDate.HasValue)
            {
                if (this.surveyVM.EndDate.Value < this.surveyVM.StartDate.Value)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.surveyVM, "EndDate");
                    this.messageStore?.Add(fi, $"Полето 'Дата на активност до' може да има стойност само след {this.surveyVM.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г. ('Дата на активност от')!");
                }
            }
        }

        public void OnChange(Syncfusion.Blazor.Inputs.ChangeEventArgs<int?> args)
        {
            copyYear = (int)args.Value;
            StateHasChanged();
        }

        private async Task CopySurveyBtn()
        {
            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                this.copySurveyConfirmDlg = !this.copySurveyConfirmDlg;

                if (this.copySurveyConfirmed)
                {
                    this.copySurveyConfirmed = false;

                    if (this.surveyVM.IdSurveyStatus == this.kvSurveyStatusCreated.IdKeyValue)
                    {
                        await this.SubmitBtn(false);

                        if (!this.editContext.GetValidationMessages().Any())
                        {
                            var result = await this.AssessmentService.CopySurveyByIdSurveyAsync(this.surveyVM.IdSurvey, this.copyYear);
                            if (result.HasErrorMessages)
                            {
                                await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                            }
                            else
                            {
                                await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                                this.surveyVM = await this.AssessmentService.GetSurveyByIdAsync(result.NewEntityId!.Value);

                                foreach (var q in this.surveyVM.Questions)
                                {
                                    q.AreaSelfAssessment = await this.DataSourceService.GetKeyValueByIdAsync(q.IdAreaSelfAssessment);
                                }
                                this.ifSelfAssessmentSurveyReady = false;

                                this.IdSurvey = this.surveyVM.IdSurvey;

                                await this.SetCreateAndModifyInfoAsync();

                                await this.CallbackAfterSubmit.InvokeAsync();

                                this.StateHasChanged();
                            }
                        }
                    }
                    else
                    {
                        var result = await this.AssessmentService.CopySurveyByIdSurveyAsync(this.surveyVM.IdSurvey);

                       


                        if (result.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                        }
                        else
                        {
                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                            this.surveyVM = await this.AssessmentService.GetSurveyByIdAsync(result.NewEntityId!.Value);

                            foreach (var q in this.surveyVM.Questions)
                            {
                                q.AreaSelfAssessment = await this.DataSourceService.GetKeyValueByIdAsync(q.IdAreaSelfAssessment);
                            }
                            this.ifSelfAssessmentSurveyReady = false;

                            this.IdSurvey = this.surveyVM.IdSurvey;

                            await this.SetCreateAndModifyInfoAsync();

                            await this.CallbackAfterSubmit.InvokeAsync();

                            this.StateHasChanged();
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
    }
}
