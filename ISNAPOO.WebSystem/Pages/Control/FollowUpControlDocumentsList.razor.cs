using DocuServiceReference;
using DocuWorkService;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class FollowUpControlDocumentsList : BlazorBaseComponent
    {
        [Parameter]
        public bool IsEditable { get; set; }
        
        [Parameter]
        public FollowUpControlVM Model { get; set; }
        [Parameter]

        public bool IsCPO { get; set; } = true;
        #region Inject
        [Inject]
        public IControlService ControlService { get; set; }

        [Inject]
        public IDocuService docuService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        #endregion

        private SfGrid<FollowUpControlDocumentVM> sfGrid;
        private SfDialog sfDialog;
        private IEnumerable<FollowUpControlDocumentVM> documentsSource = new List<FollowUpControlDocumentVM>();
        private List<FollowUpControlDocumentVM> selectedDocuments = new List<FollowUpControlDocumentVM>();

        private FollowUpControlDocumentVM NewDoc = new FollowUpControlDocumentVM();
        List<string> errorMessages = new List<string>();
        private bool isUserInRoleNAPOO = false;
        private string CpoOrCipo = string.Empty;


        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(NewDoc);
            this.isUserInRoleNAPOO = await this.IsInRole("NAPOO_Expert");
            CpoOrCipo = IsCPO ? "ЦПО" : "ЦИПО";
            this.LoadData();
        }

        //public async Task OpenModal(FollowUpControlVM _Model)
        //{
        //    if (_Model.IdFollowUpControl != 0)
        //    {
        //        this.Model = _Model;
        //        NewDoc.IdFollowUpControl = Model.IdFollowUpControl;
        //        await UpdateTable();
        //    }
        //    editContext.EnableDataAnnotationsValidation();
        //    this.StateHasChanged();
        //}

        public async Task LoadData()
        {

            if (this.Model.IdFollowUpControl != 0)
            {
                NewDoc.IdFollowUpControl = this.Model.IdFollowUpControl;
                await UpdateTable();
            }
            editContext.EnableDataAnnotationsValidation();
            this.StateHasChanged();
        }

        public async Task UpdateTable()
        {
            this.documentsSource = await this.ControlService.GetAllDocumentsAsync(Model.IdFollowUpControl);
        }

        public async Task OpenGetDocModal()
        {
            if (this.loading) return;
            try
            {
                errorMessages.Clear();
                this.loading = true;
                this.isVisible = true;
                this.NewDoc.ApplicationNumber = string.Empty;
                this.NewDoc.ApplicationDate = null;
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }
        }

        public async Task DeleteDocument(FollowUpControlDocumentVM doc)
        {
            var result = await this.ControlService.DeleteControlDocumentbyId(doc.IdFollowUpControlDocument);
            if (result)
                await this.ShowSuccessAsync("Успешно изтрит документ");
            else
                await this.ShowErrorAsync("Не можете да изтриете този документ!");

            await UpdateTable();
        }

        public async Task Submit()
        {
            if (this.loading) return;
            try
            {
                this.loading = true;
                var valid = this.editContext.Validate();

                if (valid)
                {
                    NewDoc.IsFromDS = true;

                    var result = await this.ControlService.SaveNewControlDocumentByNumberAndDate(NewDoc);
                    if (!result.HasErrorMessages)
                        await this.ShowSuccessAsync("Успешно взет документ.");
                    else
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));

                    this.isVisible = false;
                    await UpdateTable();
                    this.StateHasChanged();
                }
                else
                    errorMessages.AddRange(editContext.GetValidationMessages());
            }
            catch (Exception e)
            {
                await this.ShowErrorAsync("Няма намерен документ в деловодната система!");
            }
            finally
            {
                this.loading = false;
            }
        }

        public async Task GetDocument(FollowUpControlDocumentVM followUpControlDocumentVM)
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
                if (followUpControlDocumentVM.DS_OFFICIAL_ID != null)
                {
                    var contextResponse = await this.docuService.GetDocumentAsync((int)followUpControlDocumentVM.DS_OFFICIAL_ID, followUpControlDocumentVM.DS_OFFICIAL_GUID);

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
                    var contextResponse = await this.docuService.GetDocumentAsync((int)followUpControlDocumentVM.DS_OFFICIAL_ID, followUpControlDocumentVM.DS_OFFICIAL_GUID);

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

        public async Task DocumentSelectedHandler(RowSelectEventArgs<FollowUpControlDocumentVM> args)
        {
            this.selectedDocuments = await this.sfGrid.GetSelectedRecordsAsync();
        }

        public async Task DocumentDeselectedHandler(RowDeselectEventArgs<FollowUpControlDocumentVM> args)
        {
            this.selectedDocuments = await this.sfGrid.GetSelectedRecordsAsync();
        }
        private async Task SendNotificationAsync()
        {
            string msg = "Сигурни ли сте, че искате да приложите избраните документи по процедурата към изпращането на известие?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
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

                            await this.LoadDataForPersonsToSendNotificationToAsync("StartedProcedure", this.Model.IdCandidateProvider);

                            await this.OpenSendNotificationModal(true, this.personIds, followUpControlDocuments: this.selectedDocuments);                        
                    }
                    else
                    {
                        await this.ShowErrorAsync("Моля, изберете документ/и, който/които да изпратите!");
                    }
                }
                finally
                {
                    this.SpinnerHide();
                    this.loading = false;
                }
            }
        }

    }
}
