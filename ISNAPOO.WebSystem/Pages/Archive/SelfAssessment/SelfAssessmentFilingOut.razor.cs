using System.Runtime.CompilerServices;
using Data.Models.Data.Assessment;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Archive.SelfAssessment
{
    public partial class SelfAssessmentFilingOut: BlazorBaseComponent
    {
        

        public SurveyResultVM surveyResultVM = new SurveyResultVM();
        private IEnumerable<KeyValueVM> kvQuestionTypeSource = new List<KeyValueVM>();
        private bool sendSurveyConfirmDlg = false;
        private bool sendSurveyConfirmed = true;
        private KeyValueVM kvOpenQuestion = new KeyValueVM();
        private KeyValueVM kvSingleOpenQuestion = new KeyValueVM();
        private KeyValueVM kvMultipleOpenQuestion = new KeyValueVM();
        private KeyValueVM kvSingleQuestion = new KeyValueVM();
        private KeyValueVM kvMultipleQuestion = new KeyValueVM();
        private List<UserAnswerModel> userAnswerModels = new List<UserAnswerModel>();
        private ValidationMessageStore? messageStore;
        private List<string> validationMessages = new List<string>();

        private Decimal TotalPoint=0;
        private string QualitativeAssessment = string.Empty;
        private string QualitativeAssessmentCSS = string.Empty;
        private int ExcellentPoint = 0;
        private int GoodPoint = 0;
        private int SatisfactoryPoint = 0;
        private Decimal MaxPoint = 0;

        private bool canEditReport = false;//Submitted, Approved

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }


        [Parameter]
        public SelfAssessmentReportVM SelfAssessmentReportVM { get; set; }

        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        private bool isLoaded = false;
        private bool ifSelfAssessmentSurveyForCPO = false;


        protected override async Task OnInitializedAsync()
        {

            await LoadModal();
        }

        private async Task LoadModal() 
        {
            this.isLoaded = true;
            this.FormTitle = "Годишен доклад за самооценяване";


            if (SelfAssessmentReportVM.SurveyResult == null) {
                this.isVisible = true;
                this.StateHasChanged();
                return;
            }

            if (SelfAssessmentReportVM.IdSurveyResult == null || SelfAssessmentReportVM.IdSurveyResult == 0)
            {
                var kvSurveyResultStatusSent = await this.DataSourceService.GetKeyValueByIntCodeAsync("SurveyResultStatusType", "Sent");


                this.surveyResultVM = await this.AssessmentService
                    .CreateSurveyResultForSelfAssessmentAsync(
                            SelfAssessmentReportVM.IdCandidateProvider,
                            SelfAssessmentReportVM.SurveyResult.Survey, kvSurveyResultStatusSent.IdKeyValue);

                SelfAssessmentReportVM.IdSurveyResult = this.surveyResultVM.IdSurveyResult;
                SelfAssessmentReportVM.StatusIntCode = "Created";



            }

            this.surveyResultVM = await this.AssessmentService.GetSurveyResultWithIncludesUserAnswerByIdAsync(SelfAssessmentReportVM.IdSurveyResult.Value);
           
            var kvSurveyTarget = await this.DataSourceService.GetKeyValueByIdAsync(this.surveyResultVM.Survey.IdSurveyTarget);

            if (kvSurveyTarget.KeyValueIntCode == "ForCPO") {
                ifSelfAssessmentSurveyForCPO = true;
            }

            
            this.canEditReport = (SelfAssessmentReportVM.StatusIntCode == "Created" || SelfAssessmentReportVM.StatusIntCode == "Returned") && this.UserProps.IdCandidateProvider != 0;


            this.editContext = new EditContext(this.surveyResultVM);


            foreach (var q in this.surveyResultVM.Survey.Questions)
            {
                q.AreaSelfAssessment = await this.DataSourceService.GetKeyValueByIdAsync(q.IdAreaSelfAssessment);
            }

            this.userAnswerModels.Clear();
            this.validationMessages.Clear();

            await this.LoadKVSourceDataAsync();



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
        public async void SaveHandler()
        {
            if (!this.isLoaded) 
            {
                await LoadModal();
            }            

            this.FormTitle = "Годишен доклад за самооценяване";
            if (this.loading)
            {
                this.SpinnerHide();

            }
            try
            {
                this.loading = true;


                if (this.canEditReport)
                {
                    this.sendSurveyConfirmed = false;

                    this.validationMessages.Clear();

                    this.editContext = new EditContext(this.surveyResultVM);
                    this.editContext.EnableDataAnnotationsValidation();
                    this.messageStore = new ValidationMessageStore(this.editContext);
                    //this.editContext.OnValidationRequested += this.ValidateRequiredQuestions;
                    this.editContext.OnValidationRequested += this.ValidateOpenAnswerLength;
                    if (this.editContext.Validate())
                    {
                        var result = await this.AssessmentService.CreateUserAnswersSelfAssessmentAsync(this.userAnswerModels, this.surveyResultVM);
                        if (!result.HasErrorMessages)
                        {
                            await this.AssessmentService.SetSurveyResultEndDateAndStatusToFiledAsync(this.surveyResultVM);

                            this.isVisible = false;
                            await LoadModal();
                            this.StateHasChanged();

                            //await this.ShowSuccessAsync(string.Join("", result.ListMessages));

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
            }
            finally
            {
                this.loading = false;
            }
        }
        public override async void SubmitHandler()
        {
            this.FormTitle = "Годишен доклад за самооценяване";
            if (this.loading)
            {
                this.SpinnerHide();
               
            }
            try
            {
                this.loading = true;

                 
                if (this.canEditReport)
                {
                    this.sendSurveyConfirmed = false;

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
            }
            finally
            {
                this.loading = false;
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
            this.TotalPoint = decimal.Zero;
            this.MaxPoint = decimal.Zero;

            this.ExcellentPoint = this.surveyResultVM.Survey.Excellent!.Value;
            this.GoodPoint = this.surveyResultVM.Survey.Good!.Value;
            this.SatisfactoryPoint = this.surveyResultVM.Survey.Satisfactory!.Value;



            foreach (var question in this.surveyResultVM.Survey.Questions)
            {

                if (question.Answers.Any(x => x.Points.HasValue)) {

                    decimal maxPoint = question.Answers.Max(x => x.Points).Value;
                    //string answerText = question.Answers.Where(x => x.Points == maxPoint).FirstOrDefault().Text;
                    this.MaxPoint += maxPoint;
                }
                


                string OpenAnswerText = string.Empty;
                var userAnswerOpens = this.surveyResultVM
                                 .UserAnswerOpens
                                 .Where(x => x.IdQuestion == question.IdQuestion);

                List<int> answerIds = new List<int>();

                Decimal points = decimal.Zero;

                if (userAnswerOpens.Count() > 0) {
                    answerIds = userAnswerOpens.SelectMany(x => x.UserAnswers.Where(y=>y.IdAnswer.HasValue).Select(y => y.IdAnswer.Value)).ToList();

                    OpenAnswerText = userAnswerOpens.FirstOrDefault().Text;

                    points = (Decimal)userAnswerOpens.Sum(x => x.UserAnswers.Sum(x => x.Points));


                    this.TotalPoint += points;

                }


              


                var userAnswerModel = new UserAnswerModel()
                {
                    IdQuestion = question.IdQuestion,
                    AnswerIds = answerIds,
                    OpenAnswerText = OpenAnswerText,
                    Points = points
                };
                 
                this.userAnswerModels.Add(userAnswerModel);
            }


            if (this.TotalPoint > this.ExcellentPoint) 
            {
                this.QualitativeAssessment = "отлично";
                this.QualitativeAssessmentCSS = "label label-success";
            }
            else if (this.TotalPoint > this.GoodPoint)
            {
                this.QualitativeAssessment = "добро";
                this.QualitativeAssessmentCSS = "label label-info";
            }
            else if (this.TotalPoint > this.SatisfactoryPoint)
            {
                this.QualitativeAssessment = "задоволително";
                this.QualitativeAssessmentCSS = "label label-warning";
            }
            else 
            {
                this.QualitativeAssessment = "незадоволително";
                this.QualitativeAssessmentCSS = "label label-danger";
            }

        }
        private void SetUserAnswerCheckbox(ListBoxChangeEventArgs<AnswerVM[], AnswerVM> args)

        {

            

        }
        private async Task SetUserAnswerCheckbox(object args)
        {
            int i = 0;
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

                        if (
                            (question.IdQuestType == this.kvSingleOpenQuestion.IdKeyValue || question.IdQuestType == this.kvSingleQuestion.IdKeyValue)
                            && !userAnswerModel.AnswerIds.Any())
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.surveyResultVM, "FeedBack");
                            this.messageStore?.Add(fi, $"На въпрос '{question.Text}' е задължително да се избере един отговор!");
                        }
                         
                        if ((question.IdQuestType == this.kvMultipleOpenQuestion.IdKeyValue || question.IdQuestType == this.kvMultipleQuestion.IdKeyValue)
                            && !userAnswerModel.AnswerIds.Any())
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
