using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class ValidationClientCompetenciesModal : BlazorBaseComponent
    {

        ValidationCompetencyVM validationCompetencyVM = new ValidationCompetencyVM();
        private List<string> validationMessages = new List<string>();

        [Inject]
        public ITrainingService TrainingService { get; set; }
        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        public async Task OpenModal(ValidationCompetencyVM validationCompetencyVM)
        {
            this.validationCompetencyVM = validationCompetencyVM;
            this.editContext = new EditContext(this.validationCompetencyVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.isVisible = true;
            if (this.validationCompetencyVM.IdValidationClient != 0)
                await SetCreateAndModifyInfoAsync();
            
            this.StateHasChanged();
        }

        public async Task Submit()
        {
            if (loading) return;

            try
            {
                loading = true;
                this.validationMessages.Clear();

                var valid = editContext.Validate();

                if (valid)
                {
                    if (validationCompetencyVM.IdValidationCompetency != 0)
                    {
                        var result = await TrainingService.UpdateValidationCompetencyAsync(validationCompetencyVM);

                        if (result)
                            await this.ShowSuccessAsync("Успешно записан запис!");
                        else
                            await this.ShowErrorAsync("Проблем при запис в база данни!");
                    }
                    else
                    {
                        validationCompetencyVM = await TrainingService.CreateValidationCompetencyAsync(validationCompetencyVM);

                        if (validationCompetencyVM.IdValidationCompetency != 0)
                            await this.ShowSuccessAsync("Успешно записан запис!");
                        else
                            await this.ShowErrorAsync("Проблем при запис в база данни!");
                    }
                    await SetCreateAndModifyInfoAsync();
                    await CallbackAfterSubmit.InvokeAsync();
                }
                else
                {
                    validationMessages.AddRange(editContext.GetValidationMessages());
                }
            }finally
            {
                loading = false;
            }
        }

        public async Task SubmitAndContinueBtn()
        {
           
                await this.Submit();

                if (!this.validationMessages.Any())
                {
                    var idValidationClient = this.validationCompetencyVM.IdValidationClient;
                    this.validationCompetencyVM = new ValidationCompetencyVM()
                    {
                        IdValidationClient = idValidationClient,
                    };
                }    
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.validationCompetencyVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.validationCompetencyVM.IdModifyUser);
            this.validationCompetencyVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.validationCompetencyVM.IdCreateUser);
        }

    }
}
