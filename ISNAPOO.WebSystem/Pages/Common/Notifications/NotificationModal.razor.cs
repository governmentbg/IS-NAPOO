using Data.Models.Data.Common;
using DocuServiceReference;
using DocuWorkService;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Common.Notifications
{
    public partial class NotificationModal : BlazorBaseComponent
    {
        private SfGrid<ProcedureDocumentVM> documentGrid = new SfGrid<ProcedureDocumentVM>();
        private SfGrid<FollowUpControlDocumentVM> documentFollowUpControlGrid = new SfGrid<FollowUpControlDocumentVM>();

        private NotificationVM model = new NotificationVM();

        private bool IsOpenedFromSPPOOModule = false;
        private bool isInRoleNapooExpert = false;
        private List<ProcedureDocumentVM> files = new List<ProcedureDocumentVM>();
        private List<FollowUpControlDocumentVM> filesFollowUpControl = new List<FollowUpControlDocumentVM>();

        [Parameter]
        public EventCallback CallbackAfterNotificationCreated { get; set; }

        [Inject]
        public INotificationService NotificationService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IDocuService DocuService { get; set; }

        public override bool IsContextModified => this.editContext.IsModified();

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(NotificationVM _model, bool openFromSPPOOModule = false, List<int> personIds = null, List<ProcedureDocumentVM> procedureDocuments = null, List<FollowUpControlDocumentVM> followUpControlDocuments = null)
        {
            this.SpinnerShow();

            this.model = _model;

            this.IsOpenedFromSPPOOModule = openFromSPPOOModule;
            this.personIds = personIds;
            this.editContext = new EditContext(this.model);

            this.files.Clear();
            this.filesFollowUpControl.Clear();
            this.HandleUserRoles();
            await this.HandleProcedureDocumentsAsync(procedureDocuments);
            await this.HandleFollowUPControlDocumentsAsync(followUpControlDocuments);

            this.isVisible = true;

            this.SpinnerHide();

            this.StateHasChanged();
        }

        private async Task HandleProcedureDocumentsAsync(List<ProcedureDocumentVM> procedureDocuments)
        {
            if (procedureDocuments != null)
            {
                if (procedureDocuments.Any())
                {
                    this.files = procedureDocuments.ToList();
                }
            }
            else
            {
                if (this.model.IdNotification != 0)
                {
                    var procedureDocumentNotifications = await this.NotificationService.GetProcedureDocumentNotificationsByIdNotificationAsync(this.model.IdNotification);
                    if (procedureDocumentNotifications.Any())
                    {
                        var listProcedureDocumentIds = procedureDocumentNotifications.Select(x => x.IdProcedureDocument).ToList();
                        this.files = (await this.NotificationService.GetProcedureDocumentsByListProcedureDocumentsIdsAsync(listProcedureDocumentIds)).ToList();
                    }
                }
            }
        }

        private async Task HandleFollowUPControlDocumentsAsync(List<FollowUpControlDocumentVM> followUpControlDocuments)
        {
            if (followUpControlDocuments != null)
            {
                if (followUpControlDocuments.Any())
                {
                    this.filesFollowUpControl = followUpControlDocuments.ToList();
                }
            }
            else
            {
                if (this.model.IdNotification != 0)
                {
                    var followUpControlDocumentNotifications = await this.NotificationService.GetFollowUpControlDocumentNotificationsByIdNotificationAsync(this.model.IdNotification);
                    if (followUpControlDocumentNotifications.Any())
                    {
                        var listFollowUpControlDocumentIds = followUpControlDocumentNotifications.Select(x => x.IdFollowUpControlDocument).ToList();
                        this.filesFollowUpControl = (await this.NotificationService.GetFollowUpControlDocumentsByListProcedureDocumentsIdsAsync(listFollowUpControlDocumentIds)).ToList();
                    }
                }
            }
        }

        private async Task SubmitBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();

                if (this.editContext.Validate())
                {
                    var msg = await this.NotificationService.CreateNotificationForListOfPersonIdsAsync(this.model, this.personIds);
                    if (msg.Contains("Успешно"))
                    {
                        await this.NotificationService.CreateProcedureDocumentNotificationAsync(this.model.IdNotification, this.files);
                        await this.NotificationService.CreateFollowUpControlDocumentNotificationAsync(this.model.IdNotification, this.filesFollowUpControl);

                        await this.ShowSuccessAsync(msg);

                        await this.CallbackAfterNotificationCreated.InvokeAsync();

                        this.isVisible = false;
                    }
                    else
                    {
                        await this.ShowErrorAsync(msg);
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void HandleUserRoles()
        {
            var roles = this.GetUserRoles();
            if (roles.Contains("NAPOO_Expert"))
            {
                this.isInRoleNapooExpert = true;
            }
            else
            {
                this.isInRoleNapooExpert = false;
            }
        }

        private async Task DownloadTXTFile(bool isFileForNAPOO)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var fileName = isFileForNAPOO ? "NotificationForNAPOO.txt" : "NotificationForCPO.txt";
                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<Notification>(this.model.IdNotification, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<Notification>(this.model.IdNotification, fileName);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, "NotificationTXT.txt", documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DownloadTSRFile(bool isFileForNAPOO)
        {
            var fileName = isFileForNAPOO ? "TimeStampResponseForNAPOO.tsr" : "TimeStampResponseForCPO.tsr";

            var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<Notification>(this.model.IdNotification, fileName);
            if (hasFile)
            {
                var documentStream = await this.UploadFileService.GetUploadedFileAsync<Notification>(this.model.IdNotification, fileName);

                if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                {
                    await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                }
                else
                {
                    await FileUtils.SaveAs(this.JsRuntime, "TimeStampResponse.tsr", documentStream.MS!.ToArray());
                }
            }
            else
            {
                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                await this.ShowErrorAsync(msg);
            }
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
                    var contextResponse = await this.DocuService.GetDocumentAsync((int)procedureDocumentVM.DS_OFFICIAL_ID, procedureDocumentVM.DS_OFFICIAL_GUID);

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
                    var contextResponse = await this.DocuService.GetDocumentAsync((int)procedureDocumentVM.DS_ID, procedureDocumentVM.DS_GUID);

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
                        var fileResponse = await this.DocuService.GetFileAsync(file.FileID, guid);

                        await FileUtils.SaveAs(this.JsRuntime, file.Filename, fileResponse.File.BinaryContent.ToArray());
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        public async Task GetDocumentFollowUpControl(FollowUpControlDocumentVM followUpControlDocumentVM)
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
                    var contextResponse = await this.DocuService.GetDocumentAsync((int)followUpControlDocumentVM.DS_OFFICIAL_ID, followUpControlDocumentVM.DS_OFFICIAL_GUID);

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
                    var contextResponse = await this.DocuService.GetDocumentAsync((int)followUpControlDocumentVM.DS_ID, followUpControlDocumentVM.DS_GUID);

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
                        var fileResponse = await DocuService.GetFileAsync(file.FileID, guid);

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
    }
}
