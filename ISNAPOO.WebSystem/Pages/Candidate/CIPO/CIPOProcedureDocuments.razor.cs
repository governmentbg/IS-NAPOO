using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using ISNAPOO.Core.Contracts;
using ISNAPOO.WebSystem.Pages.Common;
using DocuWorkService;
using DocuServiceReference;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOProcedureDocuments : BlazorBaseComponent
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
        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        #endregion

        ToastMsg toast;

        private SfDialog sfDialog = new SfDialog();
        SfGrid<ProcedureDocumentVM> documentGrid = new SfGrid<ProcedureDocumentVM>();
        private IEnumerable<ProcedureDocumentVM> addedDocumentsSource = new List<ProcedureDocumentVM>();
        private List<ProcedureDocumentVM> selectedDocuments = new List<ProcedureDocumentVM>();
        private IEnumerable<KeyValueVM> AllowedDocumentTypes;

        protected override async Task OnInitializedAsync()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;

            this.addedDocumentsSource = await this.providerService.GetAllProcedureDocumentsAsync(model);

            var UserRoles = this.GetUserRoles();
            if (!UserRoles.Any(x => x == "EXTERNAL_EXPERTS" || x == "EXPERT_COMMITTEES"))
            {
                this.AllowedDocumentTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType", false, false));
            }
            else
            {
                this.AllowedDocumentTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType", false, false))
                .Where(x => UserRoles.Any(y => (x.DefaultValue4 != null ? x.DefaultValue4.Split(",").ToList().Any(c => c == y) : false)));

                this.addedDocumentsSource = this.addedDocumentsSource.Where(x => this.AllowedDocumentTypes.Select(y => y.IdKeyValue).Any(c => c == x.IdDocumentType));
            }
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

        public async Task DocumentSelectedHandler(RowSelectEventArgs<ProcedureDocumentVM> args)
        {
            this.selectedDocuments = await this.documentGrid.GetSelectedRecordsAsync();
        }

        public async Task DocumentDeselectedHandler(RowDeselectEventArgs<ProcedureDocumentVM> args)
        {
            this.selectedDocuments = await this.documentGrid.GetSelectedRecordsAsync();
        }

        private async Task SendNotificationAsync()
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да приложите избраните документи по процедурата към изпращането на известие?");
            if (confirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    if (this.selectedDocuments.Any())
                    {
                        await this.LoadDataForPersonsToSendNotificationToAsync("StartedProcedure", this.CandidateProviderVM.IdCandidate_Provider);

                        await this.OpenSendNotificationModal(true, this.personIds, this.selectedDocuments);
                    }
                    else
                    {
                        await this.ShowErrorAsync("Моля, изберете документ/и, който/които да изпратите!");
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
}
