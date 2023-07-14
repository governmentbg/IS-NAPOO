using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Licensing;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.ViewModels.NAPOOCommon;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using DocuWorkService;
using DocuServiceReference;
using Data.Models.Data.DOC;

namespace ISNAPOO.WebSystem.Pages.Candidate.ChangeLicenzing
{
    public partial class ChangeProcedureDocuments : BlazorBaseComponent
    {
        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        #region Inject
        [Inject]
        public IProviderService providerService { get; set; }
        [Inject]
        public IDocuService docuService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        #endregion

        ToastMsg toast;

        private SfDialog sfDialog = new SfDialog();
        SfGrid<ProcedureDocumentVM> documentGrid = new SfGrid<ProcedureDocumentVM>();
        private IEnumerable<ProcedureDocumentVM> addedDocumentsSource = new List<ProcedureDocumentVM>();
        private List<ProcedureDocumentVM> selectedDocuments = new List<ProcedureDocumentVM>();
        List<string> errorMessages = new List<string>();
        private ProcedureDocumentVM newProcedureDoc;
        public async Task DocumentSelectedHandler(RowSelectEventArgs<ProcedureDocumentVM> args)
        {
            this.selectedDocuments = await this.documentGrid.GetSelectedRecordsAsync();
        }

        public async Task DocumentDeselectedHandler(RowDeselectEventArgs<ProcedureDocumentVM> args)
        {
            this.selectedDocuments = await this.documentGrid.GetSelectedRecordsAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            this.addedDocumentsSource = await this.providerService.GetAllProcedureDocumentsAsync(model);
            newProcedureDoc = new ProcedureDocumentVM();
        }

        public async Task RefreshGridDocuments()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            this.addedDocumentsSource = await this.providerService.GetAllProcedureDocumentsAsync(model);

            this.documentGrid.Refresh();

        }

        public async Task GetDocument(ProcedureDocumentVM procedureDocumentVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                string guid = string.Empty;
                FileData[] files;
                if (procedureDocumentVM.DS_OFFICIAL_ID != null)
                {
                    var contextResponse = await this.docuService.GetDocumentAsync((int)procedureDocumentVM.DS_OFFICIAL_ID, procedureDocumentVM.DS_OFFICIAL_GUID);

                    if (contextResponse.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                        return;
                    }

                    var doc = contextResponse.ResultContextObject;
                    files = doc.Doc.File;
                    guid = doc.Doc.GUID;
                }
                else
                {
                    var contextResponse = await this.docuService.GetDocumentAsync((int)procedureDocumentVM.DS_ID, procedureDocumentVM.DS_GUID);

                    if (contextResponse.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                        return;
                    }

                    var doc = contextResponse.ResultContextObject;

                    files = doc.Doc.File;
                    guid = doc.Doc.GUID;
                }
                if (files == null || files.Count() == 0)
                {
                    await this.ShowErrorAsync("Няма записани документи");
                }
                else
                {
                    foreach (var file in files)
                    {
                        var fileResponse = await docuService.GetFileAsync(file.FileID, guid);

                        await FileUtils.SaveAs(JsRuntime, file.Filename, fileResponse.File.BinaryContent.ToArray());
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        private async Task SendNotificationAsync()
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да приложите избраните документи по процедурата към изпращането на известие?");
            if (confirmed)
            {
                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    if (this.selectedDocuments.Any())
                    {
                        this.SpinnerShow();

                        await this.LoadDataForPersonsToSendNotificationToAsync("StartedProcedure", this.CandidateProviderVM.IdCandidate_Provider);

                        await this.OpenSendNotificationModal(true, this.personIds, this.selectedDocuments);
                    }
                    else
                    {
                        this.SpinnerShow();

                        await this.LoadDataForPersonsToSendNotificationToAsync("StartedProcedure", this.CandidateProviderVM.IdCandidate_Provider);

                        await this.OpenSendNotificationModal(true, this.personIds);
                    }
                }
                finally
                {
                    this.SpinnerHide();
                    this.loading = false;
                }
            }
        }

        public async Task OpenGetDocModal()
        {
            if (this.loading) return;
            try
            {
                this.loading = true;
                this.isVisible = true;
                this.newProcedureDoc.ApplicationNumber = string.Empty;
                this.newProcedureDoc.ApplicationDate = null;
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task Submit()
        {

        }
    }
}
