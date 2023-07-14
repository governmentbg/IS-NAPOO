using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class NAPOOAddExtraProviderRequestDocumentsModal : BlazorBaseComponent
    {
        private SfGrid<ProviderRequestDocumentVM> sfGrid = new SfGrid<ProviderRequestDocumentVM>();
        private SfDialog sfDialog = new SfDialog();
        private ToastMsg toast = new ToastMsg();

        private List<ProviderRequestDocumentVM> providerRequestDocumentsSource = new List<ProviderRequestDocumentVM>();
        private List<ProviderRequestDocumentVM> selectedRequestsSource = new List<ProviderRequestDocumentVM>();
        private CandidateProviderVM providerVM = new CandidateProviderVM();

        [Parameter]
        public EventCallback<List<ProviderRequestDocumentVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        public async Task OpenModal(List<ProviderRequestDocumentVM> providerRequestDocuments, List<ProviderRequestDocumentVM> originalProviderRequestDocuments)
        {
            this.providerVM = await this.CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(this.UserProps.IdCandidateProvider);

            foreach (var docRequest in providerRequestDocuments)
            {
                originalProviderRequestDocuments.RemoveAll(x => x.IdProviderRequestDocument == docRequest.IdProviderRequestDocument);
            }

            if (providerRequestDocuments.Any())
            {
                this.providerRequestDocumentsSource = originalProviderRequestDocuments.Where(x => x.RequestStatus == "Подадена" && x.CurrentYear == providerRequestDocuments.FirstOrDefault().CurrentYear).ToList();
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task AddProviderRequestDocument()
        {
            bool hasPermission = await CheckUserActionPermission("ManageSummarizedRequestDocumentData", false);
            if (!hasPermission) { return; }

            if (!this.selectedRequestsSource.Any())
            {
                this.toast.sfErrorToast.Content = "Моля, изберете заявка/заявки!";
                await this.toast.sfErrorToast.ShowAsync();
                return;
            }

            await this.CallbackAfterSubmit.InvokeAsync(this.selectedRequestsSource);

            this.isVisible = false;
            this.StateHasChanged();
        }

        private async Task RequestDeselected(RowDeselectEventArgs<ProviderRequestDocumentVM> args)
        {
            await this.HandleRowSelection();
        }

        private async Task RequestSelected(RowSelectEventArgs<ProviderRequestDocumentVM> args)
        {
            await this.HandleRowSelection();
        }

        private async Task HandleRowSelection()
        {
            this.selectedRequestsSource.Clear();
            this.selectedRequestsSource = await this.sfGrid.GetSelectedRecordsAsync();
        }
    }
}
