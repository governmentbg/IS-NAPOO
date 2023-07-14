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
using static SkiaSharp.HarfBuzz.SKShaper;
using Syncfusion.Compression.Zip;
using Microsoft.AspNetCore.Identity;
using Data.Models.Data.Candidate;
using DocuWorkService.ViewModel;
using MessagePack.Formatters;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ProcedureDocuments : BlazorBaseComponent
    {
        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsLicenceChange { get; set; }

        #region Inject
        [Inject]
        public IProviderService providerService { get; set; }
        [Inject]
        public IDocuService docuService { get; set; }
        [Inject]
        public IDataSourceService dataSourceService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IExpertService ExpertService { get; set; }
        #endregion

        ToastMsg toast;

        private SfDialog sfDialog = new SfDialog();
        SfGrid<ProcedureDocumentVM> documentGrid = new SfGrid<ProcedureDocumentVM>();
        private List<ProcedureDocumentVM> addedDocumentsSource = new List<ProcedureDocumentVM>();
        private IEnumerable<KeyValueVM> AllowedDocumentTypes;
        private List<ProcedureDocumentVM> selectedDocuments = new List<ProcedureDocumentVM>();
        List<string> errorMessages = new List<string>();
        private ProcedureDocumentVM newProcedureDoc = new ProcedureDocumentVM();
        List<KeyValueVM> kvDocTypeList = new List<KeyValueVM>();
        protected override async Task OnInitializedAsync()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;

            kvDocTypeList = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType")).ToList();
            var kvDocTypeApplication4 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application4");
            this.addedDocumentsSource = (await this.providerService.GetAllProcedureDocumentsAsync(model)).ToList();
            var UserRoles = this.GetUserRoles();
            if (UserRoles.Any(x => x.Contains("NAPOO")))
            {
                this.AllowedDocumentTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType", false, false));
            }
            else
            {
                if (UserRoles.Any(x => x == "EXPERT_COMMITTEES"))
                {
                    this.AllowedDocumentTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType"))
                        .Where(x => x.KeyValueIntCode == "RequestLicensingCPO" || x.KeyValueIntCode == "Application13"
                                || x.KeyValueIntCode == "Application16" || x.KeyValueIntCode == "Application17" || x.KeyValueIntCode == "Application11").ToList();

                    var allowedIds = this.AllowedDocumentTypes.Select(x => x.IdKeyValue).ToList();
                    this.addedDocumentsSource = (this.addedDocumentsSource.Where(x => x.IdDocumentType.HasValue && allowedIds.Contains(x.IdDocumentType.Value))).ToList();
                }
                else if (UserRoles.Any(x => x == "EXTERNAL_EXPERTS"))
                {
                    this.AllowedDocumentTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType"))
                        .Where(x => x.KeyValueIntCode == "RequestLicensingCPO" || x.KeyValueIntCode == "Application2" || x.KeyValueIntCode == "Application4").ToList();
                    var kvApplicationFourValue = this.AllowedDocumentTypes.FirstOrDefault(x => x.KeyValueIntCode == "Application4");
                    var allowedIds = this.AllowedDocumentTypes.Select(x => x.IdKeyValue).ToList();
                    var externalExpert = await this.ExpertService.GetExpertByIdPersonAsync(this.UserProps.IdPerson);
                    this.addedDocumentsSource = (this.addedDocumentsSource.Where(x => x.IdDocumentType.HasValue && allowedIds.Contains(x.IdDocumentType.Value))).ToList();
                    this.addedDocumentsSource.RemoveAll(x => x.IdDocumentType == kvApplicationFourValue!.IdKeyValue && x.IdExpert != externalExpert.IdExpert);
                }
            }
            var NullDateDocuments = this.addedDocumentsSource.Where(x => x.DS_DATEOnly == null).ToList().OrderBy(x => x.Order).ToList();
            this.addedDocumentsSource = this.addedDocumentsSource.Where(x => x.DS_DATEOnly != null).ToList().OrderBy(x => x.DS_DATEOnly).ThenBy(x => x.DocumentTypeNameDescription).ToList();
            this.addedDocumentsSource.AddRange(NullDateDocuments);
            int counter = 1;
            foreach (var item in addedDocumentsSource)
            {
                item.gridRowCounter = counter;
                counter++;
            }

                //if (!UserRoles.Any(x => x == "EXTERNAL_EXPERTS" || x == "EXPERT_COMMITTEES"))
                //{
                //    this.AllowedDocumentTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType", false, false));
                //}
                //else
                //{
                //    this.AllowedDocumentTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType", false, false))
                //    .Where(x => UserRoles.Any(y => (x.DefaultValue4 != null ? x.DefaultValue4.Split(",").ToList().Any(c => c == y) : false)));

                //    this.addedDocumentsSource = this.addedDocumentsSource.Where(x => this.AllowedDocumentTypes.Select(y => y.IdKeyValue).Any(c => c == x.IdDocumentType));
                //    if (UserRoles.Any(x => x == "EXTERNAL_EXPERTS"))
                //    {
                //        this.expertVM = await this.ExpertService.GetExpertByIdPersonAsync(this.UserProps.IdPerson);

                //        var filterDocsByExpertId = this.addedDocumentsSource.Where(x => x.IdDocumentType == kvDocTypeApplication4.IdKeyValue && x.IdExpert == expertVM.IdExpert).ToList();

                //        this.addedDocumentsSource = this.addedDocumentsSource.Where(x => x.IdDocumentType != kvDocTypeApplication4.IdKeyValue);
                //        filterDocsByExpertId.AddRange(addedDocumentsSource);
                //        this.addedDocumentsSource = filterDocsByExpertId;
                //    }
                //}
            }

        public async Task RefreshGridDocuments()
        {
            var model = new ProcedureDocumentVM();
            model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            this.addedDocumentsSource = (await this.providerService.GetAllProcedureDocumentsAsync(model))
                .Where(x => this.AllowedDocumentTypes.Select(y => y.IdKeyValue).Any(c => c == x.IdDocumentType)).ToList();

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
                //if (procedureDocumentVM.DS_OFFICIAL_ID != null)
                //{
                //    var contextResponse = await this.docuService.GetDocumentAsync((int)procedureDocumentVM.DS_OFFICIAL_ID, procedureDocumentVM.DS_OFFICIAL_GUID);

                //    if (contextResponse.HasErrorMessages)
                //    {
                //        await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                //        return;
                //    }

                //    var doc = contextResponse.ResultContextObject;
                //    files = doc.Doc.File;
                //    guid = doc.Doc.GUID;
                //}
                //else
                //{
                //    var contextResponse = await this.docuService.GetDocumentAsync((int)procedureDocumentVM.DS_ID, procedureDocumentVM.DS_GUID);

                //    if (contextResponse.HasErrorMessages)
                //    {
                //        await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                //        return;
                //    }

                //    var doc = contextResponse.ResultContextObject;

                //    files = doc.Doc.File;
                //    guid = doc.Doc.GUID;
                //}

                var VidCode = this.kvDocTypeList.Where(x => x.IdKeyValue == procedureDocumentVM.IdDocumentType).First();

                var DocumentParameters = new GetDocumentVM()
                {
                    DocID = procedureDocumentVM.DS_ID,
                    GUID = procedureDocumentVM.DS_GUID,
                    OfficialDocID = procedureDocumentVM.DS_OFFICIAL_ID,
                    OfficialGUID = procedureDocumentVM.DS_OFFICIAL_GUID,
                    DocDate = procedureDocumentVM.DS_DATE,
                    DocNumber = procedureDocumentVM.DS_DocNumber,
                    OfficialDocDate = procedureDocumentVM.DS_OFFICIAL_DATE,
                    OfficialDocNumber = procedureDocumentVM.DS_OFFICIAL_DocNumber,
                    VidCode = Int32.Parse(VidCode.DefaultValue6)
                };

                var contextResponse = await this.docuService.CheckAndGetDocument(DocumentParameters);

                if (contextResponse.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                    return;
                }

                var doc = contextResponse.ResultContextObject;

                files = doc.File;
                guid = doc.GUID;

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
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.selectedDocuments.Any())
                {
                    string msg = "Сигурни ли сте, че искате да приложите избраните документи по процедурата към изпращането на известие?";
                    bool isConfirmed = await this.ShowConfirmDialogAsync(msg);

                    if (isConfirmed)
                    {
                        this.SpinnerShow();

                        await this.LoadDataForPersonsToSendNotificationToAsync("StartedProcedure", this.CandidateProviderVM.IdCandidate_Provider);

                        await this.OpenSendNotificationModal(true, this.personIds, this.selectedDocuments);
                    }
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
        public async Task OpenGetDocModal()
        {
            if (this.loading) return;
            try
            {
                this.loading = true;
                this.isVisible = true;
                this.newProcedureDoc.ApplicationNumber = string.Empty;
                this.newProcedureDoc.DeloSerial = 0;
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
            if (this.loading) return;
            try
            {
                errorMessages.Clear();
                this.loading = true;
                var valid = this.CheckFields();

                if (valid)
                {
                    //newProcedureDoc.IsFromDS = true;
                    this.newProcedureDoc.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
                    var result = await this.providerService.SaveNewDocumentByNumberAndDate(newProcedureDoc);
                    if (!result.HasErrorMessages)
                        await this.ShowSuccessAsync("Успешно взет документ.");
                    else
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));

                    this.isVisible = false;
                    this.StateHasChanged();
                    await RefreshGridDocuments();
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                await this.ShowErrorAsync("Няма връзка с деловодната система.");
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

        private bool CheckFields()
        {
            var IsValid = true;

            if(this.newProcedureDoc.ApplicationDate is null)
            {
                IsValid = false;
                this.errorMessages.Add("Полето 'Дата' е задължително!");

            }
            if(string.IsNullOrEmpty(this.newProcedureDoc.ApplicationNumber))
            {
                IsValid = false;
                this.errorMessages.Add("Полето '№ на документ' е задължително!");
                
            }

            if (this.newProcedureDoc.DeloSerial < 1)
            {
                IsValid = false;
                this.errorMessages.Add("Полето 'Серия' е задължително!");
            }

            return IsValid;
        }

        private async Task DeleteDocument(ProcedureDocumentVM procedureDocumentVM)
        {
          var valid = await this.providerService.DeleteProcedureDocument(procedureDocumentVM);

            if (valid)
                await this.ShowSuccessAsync("Успешно изтрит документ");
            else
                await this.ShowErrorAsync("Не можете да изтриете този документ!");
            await RefreshGridDocuments();

        }
    }
}
