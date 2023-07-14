using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    partial class ValidationClientCheckingsList : BlazorBaseComponent
    {

        #region Inject 
        [Inject]
        public ICandidateProviderService CandidateProviderService {get; set;}

        [Inject]
        public IDataSourceService DataSourceService {get; set;}

        [Inject]
        public IJSRuntime JsRuntime {get; set;}

        [Inject]
        public ILocationService LocationService {get; set;}

        [Inject]
        public IApplicationUserService ApplicationUserService {get; set;}

        [Inject]
        public ITrainingService TrainingService { get; set;} 

        #endregion

        private List<ValidationClientCheckingVM> validationCheckings = new List<ValidationClientCheckingVM>();
        private SfGrid<ValidationClientCheckingVM> sfGrid = new SfGrid<ValidationClientCheckingVM>();
        private string ValidationPersonName = string.Empty;
        public async Task OpenModal(int IdValidation, string? Name)
        {
            this.ValidationPersonName = Name;
            var temp = await this.TrainingService.GetAllActiveValidationClientCheckingsAsync(IdValidation); ;
            this.validationCheckings = temp.Count > 0 ? temp : new List<ValidationClientCheckingVM>();
            this.isVisible = true;
            this.StateHasChanged();
        }
        private async Task<string> GetPersonName(int PersonId)
        {
            return await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(PersonId);
        }
    }
}
