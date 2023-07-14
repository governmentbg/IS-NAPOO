using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.ExternalExpertCommission.Modals
{
    public partial class ParticipationCommisionList : BlazorBaseComponent
    {

        [Parameter]
        public EventCallback<ExpertExpertCommissionVM> CallbackAfterSave { get; set; }

        [Parameter]
        public ExpertVM parExpertVM { get; set; }


        [Parameter]
        public bool Disabled { get; set; } = true;

        [Parameter]
        public bool IsRegister { get; set; } = true;
        [Parameter]
        public bool IsEditable { get; set; } = true;

        #region Inject

        [Inject]
        public IExpertService expertService { get; set; }
        #endregion

        ParticipationCommisionModal participationCommisionModal = new ParticipationCommisionModal();

        SfGrid<ExpertExpertCommissionVM> documentGrid = new SfGrid<ExpertExpertCommissionVM>();
        private IEnumerable<ExpertExpertCommissionVM> addedCommisionSource = new List<ExpertExpertCommissionVM>();

        protected override async Task OnInitializedAsync()
        {
            this.addedCommisionSource = new List<ExpertExpertCommissionVM>();
        }

        public async Task OpenList(int idExpert)
        {
            this.SpinnerShow();
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                ExpertExpertCommissionVM filter = new ExpertExpertCommissionVM();
                filter.IdExpert = idExpert;
                if (idExpert != 0)
                {
                    this.addedCommisionSource = await this.expertService.GetAllExpertExpertCommissionsAsync(filter);
                }

                this.StateHasChanged();
            }
            finally
            {
                this.SpinnerHide();
                this.loading = false;
            }
        }
        private async Task SelectedRow(ExpertExpertCommissionVM expertExpertCommissionVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewExpertsData", false);
            if (!hasPermission) { return; }

            var model = await this.expertService.GetExpertExpertCommissionByIdAsync(expertExpertCommissionVM.IdExpertExpertCommission);

            this.participationCommisionModal.OpenModal(model);
        }

        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageExpertsData", false);
            if (!hasPermission) { return; }

            var model = new ExpertExpertCommissionVM();
            model.IdExpert = parExpertVM.IdExpert;
            this.participationCommisionModal.OpenModal(model);
        }

        public async Task RefreshGrid()
        {
            var filterVM = new ExpertExpertCommissionVM();
            filterVM.IdExpert = this.parExpertVM.IdExpert;
            this.addedCommisionSource = await this.expertService.GetAllExpertExpertCommissionsAsync(filterVM);
            await CallbackAfterSave.InvokeAsync(filterVM);
            this.documentGrid.Refresh();

        }

    }
}
