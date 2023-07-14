using System.Text.RegularExpressions;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Navigations;

namespace ISNAPOO.WebSystem.Pages.Training.Consulting
{
    public partial class ConsultingClientModal : BlazorBaseComponent
    {
        private ConsultingClientInformation consultingClientInformation = new ConsultingClientInformation();
        private ConsultingClientDocumentsList consultingClientDocumentsList = new ConsultingClientDocumentsList();
        private ConsultingClientFinishedData consultingClientFinishedData = new ConsultingClientFinishedData();
        private ConsultingPremises consultingPremises = new ConsultingPremises();
        private ConsultingConsultants consultingConsultants = new ConsultingConsultants();

        private ConsultingClientVM consultingClientVM = new ConsultingClientVM();
        private List<string> validationMessages = new List<string>();
        private bool hideBtnsConcurrentModal = false;
        private int selectedTab = 0;

        public override bool IsContextModified => this.consultingClientInformation.IsEditContextModified() || this.consultingClientFinishedData.IsEditContextModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.consultingClientVM);
        }

        public async Task OpenModal(ConsultingClientVM model, ConcurrencyInfo concurrencyInfo = null)
        {
            this.editContext = new EditContext(this.consultingClientVM);
            this.consultingClientVM = model;
            this.consultingClientFinishedData.isTabRendered = false;
            this.validationMessages.Clear();

            await this.SetCreateAndModifyInfoAsync();

            this.selectedTab = 0;
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

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.consultingClientVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.consultingClientVM.IdModifyUser);
            this.consultingClientVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.consultingClientVM.IdCreateUser);
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
                this.validationMessages.Clear();
                this.editContext = new EditContext(this.consultingClientVM);
                ConsultingFinishedDataVM consultingFinishedData = null;
                this.consultingClientInformation.SubmitHandler();
                this.validationMessages.AddRange(this.consultingClientInformation.GetValidationMessages());

                if (this.consultingClientFinishedData.IsEditContextModified() || this.consultingClientFinishedData.isTabRendered)
                {
                    this.consultingClientFinishedData.SubmitHandler();
                    this.validationMessages.AddRange(this.consultingClientFinishedData.GetValidationMessages());

                    consultingFinishedData = new ConsultingFinishedDataVM()
                    {
                        IdConsultingClient = this.consultingClientVM.IdConsultingClient,
                        IdFinishedType = this.consultingClientVM.IdFinishedType,
                        UploadedFileName = this.consultingClientVM.UploadedFileName,
                    };
                }
                if (!string.IsNullOrEmpty(this.consultingClientVM.EmailAddress))
                {
                    var pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                    if (!(Regex.IsMatch(this.consultingClientVM.EmailAddress, pattern)))
                    {
                        this.validationMessages.Add("Невалиден E-mail адрес!");
                    }
                }
                if (!this.validationMessages.Any())
                {
                    var result = new ResultContext<ConsultingClientVM>();
                    result.ResultContextObject = this.consultingClientVM;
                    var addForConcurrencyCheck = false;
                    if (this.consultingClientVM.IdConsultingClient == 0)
                    {
                        result = await this.TrainingService.CreateConsultingClientAsync(result, this.UserProps.IdCandidateProvider);
                        this.consultingClientVM = result.ResultContextObject;
                        addForConcurrencyCheck = true;
                    }
                    else
                    {
                        result = await this.TrainingService.UpdateConsultingClientAsync(result, this.UserProps.IdCandidateProvider, consultingFinishedData);
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                        await this.SetCreateAndModifyInfoAsync();

                        if (addForConcurrencyCheck)
                        {
                            await this.AddEntityIdAsCurrentlyOpened(this.consultingClientVM.IdConsultingClient, "Consulting");
                            this.IdConsultingClient = this.consultingClientVM.IdConsultingClient;
                        }

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
    }
}
