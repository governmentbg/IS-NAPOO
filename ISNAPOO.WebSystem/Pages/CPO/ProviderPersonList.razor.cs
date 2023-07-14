using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.CPO
{
    public partial class ProviderPersonList : BlazorBaseComponent
    {
        [Inject]
        public IProviderService providerService { get; set; }
        [Inject]
        public ISettingService settingService { get; set; }
        [Inject]
        public IDataSourceService dataSourceService { get; set; }
        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }
        [Inject]
        public ICommonService commonService { get; set; }

        private ToastMsg toast = new ToastMsg();
        private ProviderPersonModal providerPersonModal = new ProviderPersonModal();
        private SfGrid<CandidateProviderPersonVM> sfGridProviderPerson = new SfGrid<CandidateProviderPersonVM>();
        private List<CandidateProviderPersonVM> providerPersonSource = new List<CandidateProviderPersonVM>();
        private IEnumerable<CandidateProviderPersonVM> allProviderPerson;
        SettingVM MaxActiveUsersSetting;
        KeyValueVM ActiveStatus;
        CandidateProviderVM candidateProvider = new CandidateProviderVM();
        private CandidateProviderPersonVM modelForFilterGrid { get; set; }
        // private CandidateProviderVM CPO = new CandidateProviderVM();

        string currentProviderName = string.Empty;
        string type = string.Empty;
        string header = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            
            this.modelForFilterGrid = new CandidateProviderPersonVM()
            {
                IdCandidate_Provider = this.UserProps.IdCandidateProvider,
            };
            this.allProviderPerson = await this.providerService.GetAllCandidateProviderPersonsAsync(this.modelForFilterGrid);
            if (allProviderPerson.Any(p => p.IdCandidate_Provider == this.UserProps.IdCandidateProvider))
            {
                candidateProvider = await this.candidateProviderService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = this.UserProps.IdCandidateProvider });
            }
            this.providerPersonSource = this.allProviderPerson.ToList();

            if (providerPersonSource.FirstOrDefault() != null) 
            { 
                currentProviderName = providerPersonSource.First().CandidateProvider.ProviderName;
            }
            MaxActiveUsersSetting = await settingService.GetSettingByIntCodeAsync("MaxNumberOfActiveUsersForCPO");
            ActiveStatus = await dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active");

            await HandleTokenData();

            await HandleHeader();
        }

        public async Task HandleTokenData()
        {
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.commonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    this.type = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();
                }
                else
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        public async Task HandleHeader()
        {
            switch(type)
            {
                case "CIPO":
                    header = candidateProvider.CIPONameOwnerGrid;
                    break;
                case "CPO":
                    header = candidateProvider.CPONameOwnerGrid;
                    break;
            }
        }

        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageProviderPersonData", false);
            if (!hasPermission) { return; }

            if (providerPersonSource.FirstOrDefault() == null) 
            {
                await ShowErrorAsync("Няма ЦПО/ЦИПО, което да е прикачено към вашия профил.");
                return;
            }

            var count = providerPersonSource.Where(x => x.Status != null && x.Status.IdKeyValue == ActiveStatus.IdKeyValue).Count();
            var maxCount = Int32.Parse(MaxActiveUsersSetting.SettingValue);
            if (maxCount <= count)
            {
                await ShowErrorAsync($"Не можете да поддържате повече от {MaxActiveUsersSetting.SettingValue} активни профила в управление на потребителите!");

                return;
            }

            var model = new CandidateProviderPersonVM();

            model.CandidateProvider = providerPersonSource.First().CandidateProvider;
            await this.providerPersonModal.OpenModal(model,type);
        }

        private async Task SelectedRow(CandidateProviderPersonVM model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewProviderPersonData", false);
            if (!hasPermission) { return; }
            
            var personVM = await this.providerService.GetCandidateProviderPersonByIdAsync(model.IdCandidateProviderPerson);
            personVM.Person.IdApplicationUser = model.Person.IdApplicationUser;
            personVM.CandidateProvider = model.CandidateProvider;
            personVM.IdCreateUser = personVM.Person.IdCreateUser;
            personVM.IdModifyUser = personVM.Person.IdModifyUser;
            personVM.ModifyDate = personVM.Person.ModifyDate;
            personVM.CreationDate = personVM.Person.CreationDate;
            await this.providerPersonModal.OpenModal(personVM,type);
        }

        private async Task OnApplicationSubmit(ResultContext<CandidateProviderPersonVM> resultContext)
        {
            if (!resultContext.HasErrorMessages)
            {
                toast.sfSuccessToast.Content = "Записът е успешен";
                await toast.sfSuccessToast.ShowAsync();
                this.allProviderPerson = await this.providerService.GetAllCandidateProviderPersonsAsync(this.modelForFilterGrid); 
                this.providerPersonSource = this.allProviderPerson.ToList();
                await this.sfGridProviderPerson.Refresh();
                resultContext.ListMessages.Clear();
            }
            else
            {
                toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                await toast.sfErrorToast.ShowAsync();
                resultContext.ListErrorMessages.Clear();
            }
        }
    }
}
