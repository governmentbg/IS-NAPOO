using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ISNAPOO.WebSystem.Pages.Assessment
{
    public partial class SurveyFilingOutModal : BlazorBaseComponent
    {
        private SurveyResultVM surveyResultVM = new SurveyResultVM();
        private IEnumerable<KeyValueVM> kvQuestionTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvOpenQuestion = new KeyValueVM();
        private KeyValueVM kvSingleOpenQuestion = new KeyValueVM();
        private KeyValueVM kvMultipleOpenQuestion = new KeyValueVM();
        private KeyValueVM kvSingleQuestion = new KeyValueVM();
        private KeyValueVM kvMultipleQuestion = new KeyValueVM();
        private List<UserAnswerModel> userAnswerModels = new List<UserAnswerModel>();
        private ValidationMessageStore? messageStore;
        private List<string> validationMessages = new List<string>();

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.surveyResultVM);
        }

        public async Task OpenModal(SurveyResultVM surveyResultVM)
        {
            this.editContext = new EditContext(this.surveyResultVM);

            this.userAnswerModels.Clear();
            this.validationMessages.Clear();

            await this.LoadKVSourceDataAsync();

            this.surveyResultVM = surveyResultVM;

            this.SetUserAnswerModelSource();

            if (this.surveyResultVM.StartDate is null)
            {
                await this.SetSurveyResultStartDateAsync();
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SetSurveyResultStartDateAsync()
        {
            await this.AssessmentService.SetSurveyResultStartDateAsync(this.surveyResultVM.IdSurveyResult);
        }

        private async Task LoadKVSourceDataAsync()
        {
            this.kvQuestionTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("QuestionType");
            this.kvOpenQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Open");
            this.kvSingleOpenQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "SingleOpen");
            this.kvMultipleOpenQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "MultipleOpen");
            this.kvMultipleQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Multiple");
            this.kvSingleQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Single");
        }

        private async Task SendSurveyBtn()
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изпратите отговорите от анкетата към НАПОО? След потвърждение няма да могат бъдат правени промени по попълнените отговори.");
            if (confirmed)
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

                    this.validationMessages.Clear();

                    this.editContext = new EditContext(this.surveyResultVM);
                    this.editContext.EnableDataAnnotationsValidation();
                    this.messageStore = new ValidationMessageStore(this.editContext);
                    this.editContext.OnValidationRequested += this.ValidateRequiredQuestions;
                    this.editContext.OnValidationRequested += this.ValidateOpenAnswerLength;
                    if (this.editContext.Validate())
                    {
                        var result = await this.AssessmentService.CreateUserAnswersAsync(this.userAnswerModels, this.surveyResultVM);
                        if (!result.HasErrorMessages)
                        {
                            await this.AssessmentService.SetSurveyResultEndDateAndStatusToFiledAsync(this.surveyResultVM);

                            this.isVisible = false;
                            this.StateHasChanged();

                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                            await this.CallbackAfterSubmit.InvokeAsync();
                        }
                        else
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
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

        private void SetUserAnswerModelSource()
        {
            foreach (var question in this.surveyResultVM.Survey.Questions)
            {
                var userAnswerModel = new UserAnswerModel()
                {
                    IdQuestion = question.IdQuestion
                };

                this.userAnswerModels.Add(userAnswerModel);
            }
        }

        private void ValidateRequiredQuestions(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            foreach (var question in this.surveyResultVM.Survey.Questions)
            {
                if (question.IsRequired)
                {
                    var userAnswerModel = this.userAnswerModels.FirstOrDefault(x => x.IdQuestion == question.IdQuestion);
                    if (userAnswerModel is not null)
                    {
                        if (question.IdQuestType == this.kvOpenQuestion.IdKeyValue && string.IsNullOrEmpty(userAnswerModel.OpenAnswerText))
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.surveyResultVM, "FeedBack");
                            this.messageStore?.Add(fi, $"На въпрос '{question.Text}' е задължително да се попълни отворен отговор!");
                        }

                        if (question.IdQuestType == this.kvSingleOpenQuestion.IdKeyValue && !userAnswerModel.AnswerIds.Any())
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.surveyResultVM, "FeedBack");
                            this.messageStore?.Add(fi, $"На въпрос '{question.Text}' е задължително да се попълни отворен отговор!");
                        }

                        if (question.IdQuestType == this.kvSingleOpenQuestion.IdKeyValue && string.IsNullOrEmpty(userAnswerModel.OpenAnswerText))
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.surveyResultVM, "FeedBack");
                            this.messageStore?.Add(fi, $"На въпрос '{question.Text}' е задължително да се избере един отговор!");
                        }

                        if (question.IdQuestType == this.kvMultipleOpenQuestion.IdKeyValue && !userAnswerModel.AnswerIds.Any())
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.surveyResultVM, "FeedBack");
                            this.messageStore?.Add(fi, $"На въпрос '{question.Text}' е задължително да се попълни отворен отговор!");
                        }

                        if (question.IdQuestType == this.kvMultipleOpenQuestion.IdKeyValue && string.IsNullOrEmpty(userAnswerModel.OpenAnswerText))
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.surveyResultVM, "FeedBack");
                            this.messageStore?.Add(fi, $"На въпрос '{question.Text}' е задължително да се избере поне един отговор!");
                        }

                        if (question.IdQuestType == this.kvSingleQuestion.IdKeyValue && !userAnswerModel.AnswerIds.Any())
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.surveyResultVM, "FeedBack");
                            this.messageStore?.Add(fi, $"На въпрос '{question.Text}' е задължително да се избере един отговор!");
                        }

                        if (question.IdQuestType == this.kvMultipleQuestion.IdKeyValue && !userAnswerModel.AnswerIds.Any())
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.surveyResultVM, "FeedBack");
                            this.messageStore?.Add(fi, $"На въпрос '{question.Text}' е задължително да се избере поне един отговор!");
                        }
                    }
                }
            }
        }

        private void ValidateOpenAnswerLength(object? sender, ValidationRequestedEventArgs args)
        {
            foreach (var question in this.surveyResultVM.Survey.Questions)
            {
                var userAnswerModel = this.userAnswerModels.FirstOrDefault(x => x.IdQuestion == question.IdQuestion);
                if (userAnswerModel is not null)
                {
                    if (!string.IsNullOrEmpty(userAnswerModel.OpenAnswerText) && userAnswerModel.OpenAnswerText.Length > DBStringLength.StringLength1000)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.surveyResultVM, "FeedBack");
                        this.messageStore?.Add(fi, $"Полето за отворен отговор на въпрос '{question.Text}' може да съдържа до {DBStringLength.StringLength1000} символа!");
                    }
                }
            }
        }
    }
}
