using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;

namespace ISNAPOO.WebSystem.Pages.Assessment
{
    public partial class SurveyFilingOut : BlazorBaseComponent
    {
        private SurveyFilingOutModal surveyFilingOutModal = new SurveyFilingOutModal();

        private SurveyResultVM surveyResultVM = new SurveyResultVM();
        private string msg = string.Empty;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await this.HandleTokenData();
        }

        private async Task HandleTokenData()
        {
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    var encryptedIdSurveyResult = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.TOKEN_SURVEYRESULT_KEY).Value.ToString();

                    if (!string.IsNullOrEmpty(encryptedIdSurveyResult))
                    {
                        var decryptedIdSurveyResult = BaseHelper.Decrypt(encryptedIdSurveyResult);
                        int idSurveyResult;
                        if (int.TryParse(decryptedIdSurveyResult, out idSurveyResult))
                        {
                            this.surveyResultVM = await this.AssessmentService.GetSurveyResultByIdAsync(idSurveyResult);
                            if (this.surveyResultVM is null)
                            {
                                this.msg = "Невалиден линк за попълване на анкета!";
                                this.StateHasChanged();
                            }
                            else
                            {
                                if (this.surveyResultVM.Survey.EndDate.HasValue && this.surveyResultVM.Survey.EndDate.Value.Date < DateTime.Now.Date)
                                {
                                    this.msg = "Невалиден линк за попълване на анкета!";
                                    this.StateHasChanged();
                                }
                                else
                                {
                                    var kvSurveyResultStatusSent = await this.DataSourceService.GetKeyValueByIntCodeAsync("SurveyResultStatusType", "Sent");
                                    if (this.surveyResultVM.IdStatus != kvSurveyResultStatusSent.IdKeyValue)
                                    {
                                        this.msg = "Тази анкета е вече попълнена!";
                                        this.StateHasChanged();
                                    }
                                    else
                                    {
                                        this.SpinnerShow();

                                        await this.surveyFilingOutModal.OpenModal(this.surveyResultVM);

                                        this.SpinnerHide();
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.msg = "Невалиден линк за попълване на анкета!";
                            this.StateHasChanged();
                        }
                    }
                    else
                    {
                        this.msg = "Невалиден линк за попълване на анкета!";
                        this.StateHasChanged();
                    }
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));

                    this.msg = "Невалиден линк за попълване на анкета!";
                    this.StateHasChanged();
                }
            }
        }

        private void UpdateAfterSurveyModalSubmit()
        {
            this.msg = "Благодарим Ви, че отделихте време за попълване на анкетата!";
            this.StateHasChanged();
        }
    }
}
