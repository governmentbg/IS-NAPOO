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
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Archive.SelfAssessment
{
    public partial class SelfAssessmentQuestionModal : BlazorBaseComponent, IConcurrencyCheck<QuestionVM>
    {
        private QuestionVM questionVM = new QuestionVM();
        private List<string> validationMessages = new List<string>();
        private bool hideBtnsConcurrentModal = false;
        private IEnumerable<KeyValueVM> kvQuestionTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvAreaSelfAssessment = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvRatingIndicatorType = new List<KeyValueVM>();
        private KeyValueVM kvOpenQuestion = new KeyValueVM();
        private KeyValueVM kvSingleOpenQuestion = new KeyValueVM();
        private KeyValueVM kvMultipleOpenQuestion = new KeyValueVM();
        private EditContext answerEditContext;
        private AnswerVM[] answers;
        private ValidationMessageStore? messageStore;
        private AnswerVM answerVM = new AnswerVM();
        private string openQuestionText = "< тук ще е отговорът на отворения въпрос >";

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
            this.editContext = new EditContext(this.questionVM);
            this.answerEditContext = new EditContext(this.answerVM);
        }

        public async Task OpenModal(QuestionVM question, ConcurrencyInfo concurrencyInfo = null)
        {
            this.validationMessages.Clear();

            this.answers = null;

            this.questionVM = question;

            await this.LoadKVDataAsync();

            await this.SetCreateAndModifyInfoAsync();

            if (this.questionVM.IdQuestion != 0)
            {
                this.IdQuestion = this.questionVM.IdQuestion;
            }

            this.editContext = new EditContext(this.questionVM);
            this.answerEditContext = new EditContext(this.answerVM);

            if (this.questionVM.IdQuestion != 0)
            {
                await this.LoadAnswersDataAsync();
            }

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
            this.kvQuestionTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("QuestionType");
            this.kvAreaSelfAssessment = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AreaSelfAssessment");
            this.kvRatingIndicatorType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType");
            this.kvOpenQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Open");
            this.kvSingleOpenQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "SingleOpen");
            this.kvMultipleOpenQuestion = this.kvQuestionTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "MultipleOpen");
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.questionVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.questionVM.IdModifyUser);
            this.questionVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.questionVM.IdCreateUser);
        }

        private void OnQuestionTypeValueChanged(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.ItemData is not null)
            {
                if (args.ItemData.KeyValueIntCode == "Open")
                {
                    this.questionVM.AnswersCount = 1;
                }
                else
                {
                    this.questionVM.AnswersCount = null;
                }
            }
            else
            {
                this.questionVM.AnswersCount = null;
            }
        }

        private async Task SubmitBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.questionVM);
                this.editContext.EnableDataAnnotationsValidation();
                this.validationMessages.Clear();
                this.editContext.Validate();
                this.validationMessages.AddRange(this.editContext.GetValidationMessages());

                if (!this.validationMessages.Any())
                {
                    var result = new ResultContext<QuestionVM>();
                    result.ResultContextObject = this.questionVM;
                    if (this.questionVM.IdQuestion == 0)
                    {
                        result = await this.AssessmentService.CreateQuestionAsync(result);
                        if (!result.HasErrorMessages && this.questionVM.IdQuestType == this.kvOpenQuestion.IdKeyValue)
                        {
                            AnswerVM answer = new AnswerVM()
                            {
                                IdQuestion = this.questionVM.IdQuestion,
                                Text = string.Empty
                            };

                            var isValid = await this.AssessmentService.CreateAnswerAsync(answer);
                            if (!isValid)
                            {
                                await this.ShowErrorAsync("Грешка при запис в базата данни!");
                                this.SpinnerHide();
                                return;
                            }
                        }
                    }
                    else
                    {
                        result = await this.AssessmentService.UpdateQuestionAsync(result);

                        if (this.questionVM.IdQuestType != this.kvOpenQuestion.IdKeyValue)
                        {
                            var isValid = await this.SaveAnswersDataAsync();
                            if (!isValid)
                            {
                                await this.ShowErrorAsync("Грешка при запис в базата данни!");
                                this.SpinnerHide();
                                return;
                            }
                        }
                    }

                    if (!result.HasErrorMessages)
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        await this.SetCreateAndModifyInfoAsync();

                        if (this.answers is null)
                        {
                            this.GenerateAnswersArray(this.questionVM.AnswersCount!.Value);
                        }
                        else
                        {
                            await this.LoadAnswersDataAsync();
                        }

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

        private void GenerateAnswersArray(int count)
        {
            this.answers = new AnswerVM[count];
            this.answers = this.answers.Select(x => new AnswerVM()).ToArray();
        }

        private async Task LoadAnswersDataAsync()
        {
            if (this.answers == null)
            {
                this.answers = await this.AssessmentService.GetAnswersByIdQuestionAsync(this.questionVM.IdQuestion);
                if (this.answers.Length == 0)
                {
                    this.GenerateAnswersArray(this.questionVM.AnswersCount.Value);
                }
            }
        }

        private async Task<bool> SaveAnswersDataAsync()
        {
            this.answerEditContext = new EditContext(this.answerVM);
            this.answerEditContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.answerEditContext);
            this.answerEditContext.OnValidationRequested += this.ValidateAnswers;
            this.validationMessages.Clear();
            this.answerEditContext.Validate();
            this.validationMessages.AddRange(this.answerEditContext.GetValidationMessages());

            if (!this.validationMessages.Any())
            {
                bool isValid = false;
                foreach (var answer in this.answers)
                {
                    if (answer.IdAnswer == 0)
                    {
                        answer.IdQuestion = this.questionVM.IdQuestion;
                        isValid = await this.AssessmentService.CreateAnswerAsync(answer);
                    }
                    else
                    {
                        isValid = await this.AssessmentService.UpdateAnswerAsync(answer);
                    }
                }

                return isValid;
            }
            else
            {
                await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
            }

            return true;
        }

        private void ValidateAnswers(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.answers is not null && this.answers.Any())
            {
                foreach (var answer in this.answers)
                {
                    if (string.IsNullOrEmpty(answer.Text))
                    {
                        FieldIdentifier fi = new FieldIdentifier(answer, "Text");
                        this.messageStore?.Add(fi, $"Полето 'Отговор '{answer.Symbol}'' е задължително!");
                    }

                    if (!string.IsNullOrEmpty(answer.Text) && answer.Text.Length > DBStringLength.StringLength1000)
                    {
                        FieldIdentifier fi = new FieldIdentifier(answer, "Text");
                        this.messageStore?.Add(fi, $"Полето 'Отговор '{answer.Symbol}'' може да съдържа до {DBStringLength.StringLength1000} символа!");
                    }
                }
            }
        }
    }
}
