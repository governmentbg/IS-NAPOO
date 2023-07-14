using Blazored.LocalStorage;
using Data.Models;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.SPPOO.Modals;
using ISNAPOO.WebSystem.Pages.SPPOO.Modals.Area;
using ISNAPOO.WebSystem.Pages.SPPOO.Modals.CPOSpeciality;
using ISNAPOO.WebSystem.Pages.SPPOO.Modals.Profession;
using ISNAPOO.WebSystem.Pages.SPPOO.Modals.ProfessionalDirection;
using ISNAPOO.WebSystem.Pages.SPPOO.Modals.Speciality;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.TreeGrid;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.SPPOO
{
    public partial class ProfessionList : BlazorBaseComponent
    {
        ToastMsg toast;
        SfTreeGrid<SPPOOTreeGridData> sfTreeGrid = new SfTreeGrid<SPPOOTreeGridData>();

        AreaModal areaModal = new AreaModal();
        ProfessionalDirectionModal proffessionalDirectionModal = new ProfessionalDirectionModal();
        ProfessionModal professionModal = new ProfessionModal();
        SpecialityModal specialityModal = new SpecialityModal();
        FilterSPPOOModal openFilterProfessionModal = new FilterSPPOOModal();
        CPOSpecialityModal cpoSpecialityModal = new CPOSpecialityModal();
        List<SPPOOTreeGridData> SPPOODataList = new List<SPPOOTreeGridData>();
        List<SPPOOTreeGridData> data = new List<SPPOOTreeGridData>();
        MenuItem selectedItem;
        string entityType;
        List<ContextMenuItemModel> contextMenuItemModels;
        List<int> list = new List<int>();
        int[] currentdata = new int[] { };
        int idForLocalStorage = 0;
        IEnumerable<KeyValueVM> spkValue;
        IEnumerable<OrderVM> orders;

        List<int> areaList = new List<int>();
        List<int> professionalDirectionList = new List<int>();
        List<int> professionList = new List<int>();
        List<int> specialityList = new List<int>();

        bool expandFilterResults = true;
        bool showAllResults = false;
        bool expandAllResults = false;
        bool submitTriggered = false;

        string entityTypeAfterConfirm = string.Empty;
        int entityIdAfterConfirm;

        IEnumerable<string> filterNames = new List<string>();

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IUploadFileService uploadService { get; set; }

        [Inject]
        public IAreaService AreaService { get; set; }

        [Inject]
        public IProfessionalDirectionService ProffessionalDirectionService { get; set; }

        [Inject]
        public IProfessionService ProfessionService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ISpecialityOrderService SpecialityOrderService { get; set; }

        [Inject]
        public INotificationService NotificationService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IOrderService OrderService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocalStorageService localStorage { get; set; }

        protected override void OnInitialized()
        {
            this.contextMenuItemModels = this.GenerateContextMenuButtons();
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.localStorage.RemoveItemAsync(GlobalConstants.LocalStorage_SPPOOTreeGridDataExpandedNode);

                this.SpinnerShow();

                this.data = await this.AreaService.LoadSPPOOData(this.areaList, this.professionalDirectionList, this.professionList, this.specialityList);
                this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();
                this.spkValue = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.SpkVQS, false, true);
                this.orders = await this.OrderService.GetAllOrdersAsync();

                await this.sfTreeGrid.RefreshAsync();

                this.SpinnerHide();
            }
        }

        //protected override async Task OnInitializedAsync()
        //{
        //    await this.localStorage.RemoveItemAsync(GlobalConstants.LocalStorage_SPPOOTreeGridDataExpandedNode);

        //    this.SpinnerShow();

        //    this.data = await this.AreaService.LoadSPPOOData(this.areaList, this.professionalDirectionList, this.professionList, this.specialityList);
        //    this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();
        //    this.spkValue = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.SpkVQS);
        //    this.orders = await this.OrderService.GetAllOrdersAsync();

        //    await this.sfTreeGrid.RefreshAsync();

        //    this.SpinnerHide();
        //}

        private async Task RefreshGrid()
        {
            this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();
            await this.sfTreeGrid.RefreshAsync();
            if (!this.expandAllResults)
            {
                await this.sfTreeGrid.CollapseAllAsync();
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdf"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                PdfTheme Theme = new PdfTheme();
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                ExportProperties.FileName = $"SPPOO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfTreeGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"SPPOO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;
                await this.sfTreeGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private async Task GenerateExportAsync()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                var result = this.AreaService.GenerateSPPOOReport(this.SPPOODataList, this.spkValue);
                await FileUtils.SaveAs(this.JsRuntime, $"SPPOO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", result.ToArray());
            }
            finally
            {
                this.SpinnerHide();
                this.loading = false;
            }
        }

        private async void OnDownloadClick(string fileName)
        {
            bool hasPermission = await CheckUserActionPermission("ViewSPPOOData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                OrderVM order = this.orders.FirstOrDefault(x => x.OrderNumber == fileName);

                if (order != null)
                {
                    var hasFile = await this.uploadService.CheckIfExistUploadedFileAsync<SPPOOOrder>(order.IdOrder);
                    if (hasFile)
                    {
                        var documentStream = await this.uploadService.GetUploadedFileAsync<SPPOOOrder>(order.IdOrder);

                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, order.FileName, documentStream.MS!.ToArray());
                        }
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

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

        private void CellInfoHandler(QueryCellInfoEventArgs<SPPOOTreeGridData> args)
        {
            if (args.Data.EntityType != SPPOOTypes.Speciality)
            {
                args.Cell.AddClass(new string[] { "bold-elements" });
            }

            if (args.Data.IdStatus != DataSourceService.GetActiveStatusID())
            {
                args.Cell.AddClass(new string[] { "text-danger" });
            }
        }

        private void RowDataBoundHandler(RowDataBoundEventArgs<SPPOOTreeGridData> args)
        {
            if (this.filterNames.Contains(args.Data.Name))
            {
                args.Row.AddClass(new string[] { "bold-elements", "bg-elements" });
            }
        }

        private async Task FilterHandler(Dictionary<string, HashSet<int>> values)
        {
            this.areaList = values[SPPOOTypes.Area].ToList().OrderBy(x => x).ToList();
            this.professionalDirectionList = values[SPPOOTypes.ProfessionalDirection].ToList().OrderBy(x => x).ToList();
            this.professionList = values[SPPOOTypes.Profession].ToList().OrderBy(x => x).ToList();
            this.specialityList = values[SPPOOTypes.Speciality].ToList().OrderBy(x => x).ToList();

            if (this.areaList.Count() > 0)
            {
                this.expandFilterResults = false;
                this.SPPOODataList = await this.AreaService.LoadSPPOOData(this.areaList, this.professionalDirectionList, this.professionList, this.specialityList);
            }
            else
            {
                this.SPPOODataList = new List<SPPOOTreeGridData>();
            }
        }

        // болд-ва и подчертава елементите, които отговарят на филтъра
        private async Task FilterHandlerForNames(IEnumerable<string> names)
        {
            this.filterNames = names;
            await this.sfTreeGrid.RefreshAsync();
        }

        private async Task OpenFilterModal()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var data = await this.AreaService.LoadSPPOOData(new List<int>(), new List<int>(), new List<int>(), new List<int>());
                await this.openFilterProfessionModal.OpenModal(data);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async void DataboundHandler(object args)
        {
            if (this.submitTriggered)
            {
                this.currentdata = (await localStorage.GetItemAsync<int[]>(GlobalConstants.LocalStorage_SPPOOTreeGridDataExpandedNode));

                if (this.currentdata != null && this.currentdata.Length > 0)
                {
                    this.list.AddRange(this.currentdata);
                }

                if (this.sfTreeGrid != null && this.localStorage != null)
                {
                    this.currentdata = await this.localStorage.GetItemAsync<int[]>(GlobalConstants.LocalStorage_SPPOOTreeGridDataExpandedNode);

                    if (this.currentdata != null)
                    {
                        for (var i = 0; i < this.currentdata.Length; i++)
                        {
                            await this.sfTreeGrid.ExpandByKeyAsync(currentdata[i]);
                        }
                    }

                    if (this.showAllResults)
                    {
                        if (this.showAllResults)
                        {
                            this.SPPOODataList = this.data;
                        }
                        else
                        {
                            this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();
                        }

                        if (this.expandAllResults)
                        {
                            if (this.showAllResults)
                            {
                                foreach (var entry in this.data)
                                {
                                    if (entry.HasChildren)
                                    {
                                        await this.sfTreeGrid.ExpandRowAsync(entry);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var entry in this.SPPOODataList)
                                {
                                    if (entry.HasActiveChildren)
                                    {
                                        await this.sfTreeGrid.ExpandRowAsync(entry);
                                    }
                                }
                            }
                        }
                    }

                    if (this.expandAllResults)
                    {
                        if (this.expandAllResults)
                        {
                            if (this.showAllResults)
                            {
                                foreach (var entry in this.data)
                                {
                                    if (entry.HasChildren)
                                    {
                                        await this.sfTreeGrid.ExpandRowAsync(entry);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var entry in this.SPPOODataList)
                                {
                                    if (entry.HasActiveChildren)
                                    {
                                        await this.sfTreeGrid.ExpandRowAsync(entry);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.showAllResults)
                            {
                                foreach (var entry in this.data)
                                {
                                    if (entry.HasChildren)
                                    {
                                        await this.sfTreeGrid.CollapseRowAsync(entry);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var entry in this.SPPOODataList)
                                {
                                    if (entry.HasActiveChildren)
                                    {
                                        await this.sfTreeGrid.CollapseRowAsync(entry);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var item in this.currentdata)
                {
                    await this.sfTreeGrid.ExpandByKeyAsync(item);
                }
            }
        }

        public async Task ExpandedHandler(RowExpandedEventArgs<SPPOOTreeGridData> args)
        {
            if (args.Data != null)
            {
                if (!this.list.Contains(args.Data.Id))
                {
                    this.list.Add(args.Data.Id);
                    this.currentdata = this.list.ToArray();
                }
            }
        }

        public async Task CollapsedHandler(RowCollapsedEventArgs<SPPOOTreeGridData> args)
        {
            if (args.Data != null)
            {
                var entityType = args.Data.EntityType;
                var id = args.Data.Id;
                var idsToRemove = new List<int>();
                if (entityType == SPPOOTypes.Area)
                {
                    var professionalDirections = this.SPPOODataList.Where(x => x.ParentId == id).Select(x => x.Id).ToList();
                    var professions = new List<int>();
                    foreach (var entry in professionalDirections)
                    {
                        var professionsFromEntry = this.SPPOODataList.Where(x => x.ParentId == entry).Select(x => x.Id);
                        professions.AddRange(professionsFromEntry);
                    }

                    professionalDirections.AddRange(professions);
                    idsToRemove = professionalDirections.ToList();
                }
                else if (entityType == SPPOOTypes.ProfessionalDirection)
                {
                    var professions = this.SPPOODataList.Where(x => x.ParentId == id).Select(x => x.Id).ToList();
                    idsToRemove = professions.ToList();
                }
                else if (entityType == SPPOOTypes.Profession)
                {
                    idsToRemove.Add(id);
                }

                foreach (var entry in idsToRemove)
                {
                    this.list.Remove(entry);
                }

                this.list.Remove(id);
                this.currentdata = this.list.ToArray();
            }
        }

        private void RowSelectedHandler(RowSelectEventArgs<SPPOOTreeGridData> args)
        {
            this.entityType = args.Data.EntityType;
        }

        private void RowDeselectedHandler()
        {
            this.entityType = null;
        }

        private void ContextMenuOpenHandler()
        {
            this.contextMenuItemModels = new List<ContextMenuItemModel>();
            this.contextMenuItemModels = this.GenerateContextMenuButtons();
        }

        private async Task ShowAllResultsHandler()
        {
            this.showAllResults = !this.showAllResults;

            if (this.showAllResults)
            {
                this.SPPOODataList = this.data;
            }
            else
            {
                this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();
            }

            foreach (var item in this.currentdata)
            {
                await this.sfTreeGrid.ExpandByKeyAsync(item);
                await this.sfTreeGrid.ExpandByKeyAsync(item);
            }
        }

        private async Task OnModalSubmit(string msg)
        {
            this.currentdata = new int[] { };
            this.list = new List<int>();

            await this.localStorage.RemoveItemAsync(GlobalConstants.LocalStorage_SPPOOTreeGridDataExpandedNode);

            this.submitTriggered = true;

            if (!this.list.Contains(this.idForLocalStorage))
            {
                this.list.Add(this.idForLocalStorage);

                var entryOne = this.data.FirstOrDefault(x => x.Id == this.idForLocalStorage);

                if (entryOne is not null && entryOne.ParentId != null)
                {
                    var parentOne = this.data.FirstOrDefault(x => x.Id == entryOne.ParentId);

                    if (parentOne is not null)
                    {
                        this.list.Add(parentOne.Id);
                    }

                    var entryTwo = this.data.FirstOrDefault(x => x.Id == parentOne.ParentId);

                    if (entryTwo != null)
                    {
                        this.list.Add(entryTwo.Id);

                        var entryThree = this.data.FirstOrDefault(x => x.Id == entryTwo.ParentId);

                        if (entryThree != null)
                        {
                            this.list.Add(entryThree.Id);
                        }
                    }
                }

                this.currentdata = this.list.ToArray();
            }

            await this.localStorage.SetItemAsync(GlobalConstants.LocalStorage_SPPOOTreeGridDataExpandedNode, this.currentdata);

            this.data = await this.AreaService.LoadSPPOOData(this.areaList, this.professionalDirectionList, this.professionList, this.specialityList);
            this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();

            if (msg.Contains("успешен"))
            {
                this.toast.sfSuccessToast.Content = msg;
                await toast.sfSuccessToast.ShowAsync();
            }
            else
            {
                this.toast.sfErrorToast.Content = msg;
                await this.toast.sfErrorToast.ShowAsync();
            }
        }

        private async Task ContextMenuClickHandler(ContextMenuClickEventArgs<SPPOOTreeGridData> args)
        {
            this.idForLocalStorage = args.RowInfo.RowData.Id;
            string professionName = args.RowInfo.RowData.Name;
            string buttonText = args.Item.Text;
            string entityType = args.RowInfo.RowData.EntityType;
            this.entityType = entityType;
            int entityId = args.RowInfo.RowData.EntityId;

            SPPOOTreeGridData current = this.SPPOODataList.FirstOrDefault(x => x.EntityId == entityId && x.EntityType == this.entityType);

            int? parentId = args.RowInfo.RowData.ParentId;
            SPPOOTreeGridData parent = this.SPPOODataList.FirstOrDefault(x => x.Id == parentId);
            string codeAndArea = string.Empty;
            int areaId = 0;

            int? parentParentId = 0;
            string codeAndProffessionalDirection = string.Empty;
            int proffessionalDirectionId = 0;

            int? parentParentParentId = 0;
            string codeAndProffession = string.Empty;
            int proffessionId = 0;

            if (parent != null)
            {
                areaId = parent.EntityId;
                codeAndProffessionalDirection = $"{parent.Code} {parent.Name}";

                parentParentId = parent.ParentId;
                SPPOOTreeGridData parentParent = this.SPPOODataList.FirstOrDefault(x => x.Id == parentParentId);

                if (parentParent != null)
                {
                    parentParentParentId = parentParent.ParentId;
                    SPPOOTreeGridData parentParentParent = this.SPPOODataList.FirstOrDefault(x => x.Id == parentParentParentId);

                    codeAndArea = $"{parentParent.Code} {parentParent.Name}";
                    proffessionalDirectionId = areaId;

                    if (parentParentParent != null)
                    {
                        parentParentParentId = parentParentParent.ParentId;
                        codeAndArea = $"{parentParentParent.Code} {parentParentParent.Name}";
                        codeAndProffessionalDirection = $"{parentParent.Code} {parentParent.Name}";
                        codeAndProffession = $"{parent.Code} {parent.Name}";
                        proffessionId = areaId;
                    }
                    else
                    {
                        codeAndProffession = $"{current.Code} {current.Name}";
                        proffessionId = current.EntityId;
                    }
                }
                else
                {
                    codeAndArea = $"{parent.Code} {parent.Name}";
                    codeAndProffessionalDirection = $"{current.Code} {current.Name}";
                    proffessionalDirectionId = current.EntityId;
                }
            }
            else
            {
                areaId = entityId;
                codeAndArea = $"{current.Code} {current.Name}";
            }

            if (buttonText == ContextMenu.EditButtonText)
            {
                await this.EditDataAsync(entityId, codeAndArea, areaId, codeAndProffessionalDirection, proffessionalDirectionId, codeAndProffession, proffessionId);
            }
            else if (buttonText.Contains("Добави"))
            {
                await this.AddData(codeAndArea, areaId, codeAndProffessionalDirection, proffessionalDirectionId, codeAndProffession, proffessionId, args.RowInfo.RowData.Id);
            }
            else if (buttonText == ContextMenu.DeleteButtonText)
            {
                await this.DeleteDataAsync(this.entityType, entityId);
            }
            else if (buttonText == ContextMenu.CPOButtonText)
            {
                await this.cpoSpecialityModal.OpenModal(entityId, this.entityType, codeAndArea, areaId, codeAndProffessionalDirection, proffessionalDirectionId, codeAndProffession, proffessionId);
            }
            else if (buttonText == ContextMenu.ShowAllElements)
            {
                await this.ExpandElements(this.idForLocalStorage, this.entityType);
            }
            else if (buttonText == ContextMenu.SendNotificationText)
            {
                await this.LoadDataForPersonsToSendNotificationToAsync(this.entityType, entityId);

                if (!this.personIds.Any())
                {
                    if (this.entityType != SPPOOTypes.Speciality)
                    {
                        await this.ShowErrorAsync("Няма активни лицензирани ЦПО по избраните специалности!");
                    }
                    else
                    {
                        await this.ShowErrorAsync("Няма активни лицензирани ЦПО по избраната специалност!");
                    }
                }
                else
                {
                    await this.OpenSendNotificationModal(true, this.personIds);
                }
            }
        }

        private async Task AddData(string codeAndArea, int areaId, string codeAndProffessionalDirection, int proffessionalDirectionId, string codeAndProffession, int proffessionId, int idFromGrid)
        {
            bool hasPermission = await CheckUserActionPermission("ManageSPPOOData", false);
            if (!hasPermission) { return; }

            if (this.entityType == SPPOOTypes.Area)
            {
                await this.proffessionalDirectionModal.OpenModal(new ProfessionalDirectionVM(), codeAndArea, areaId, this.data.Where(x => x.EntityType == SPPOOTypes.ProfessionalDirection));
            }
            else if (this.entityType == SPPOOTypes.ProfessionalDirection)
            {
                var code = this.SetNewProfessionCode(idFromGrid);

                await this.professionModal.OpenModal(new ProfessionVM() { Code = code }, codeAndArea, codeAndProffessionalDirection, areaId, proffessionalDirectionId, this.data.Where(x => x.EntityType == SPPOOTypes.Profession));
            }
            else if (this.entityType == SPPOOTypes.Profession)
            {
                var code = this.SetNewSpecialityCode(idFromGrid);

                await this.specialityModal.OpenModal(new SpecialityVM() { Code = code }, codeAndArea, codeAndProffessionalDirection, codeAndProffession, proffessionId, this.data.Where(x => x.EntityType == SPPOOTypes.Speciality));
            }
        }

        private async Task EditDataAsync(int entityId, string codeAndArea, int areaId, string codeAndProffessionalDirection, int proffessionalDirectionId, string codeAndProffession, int proffessionId)
        {
            bool hasPermission = await CheckUserActionPermission("ManageSPPOOData", false);
            if (!hasPermission) { return; }

            if (this.entityType == SPPOOTypes.Area)
            {
                AreaVM selectedAreaToEdit = await this.AreaService.GetAreaByIdAsync(entityId);
                await this.areaModal.OpenModal(selectedAreaToEdit, this.data.Where(x => x.EntityType == SPPOOTypes.Area));
            }
            else if (this.entityType == SPPOOTypes.ProfessionalDirection)
            {
                ProfessionalDirectionVM newProfDirection = new ProfessionalDirectionVM { IdProfessionalDirection = entityId };
                ProfessionalDirectionVM selectedProffessionalDirectionToEdit = await this.ProffessionalDirectionService.GetProfessionalDirectionByIdAsync(newProfDirection);
                await this.proffessionalDirectionModal.OpenModal(selectedProffessionalDirectionToEdit, codeAndArea, areaId, this.data.Where(x => x.EntityType == SPPOOTypes.ProfessionalDirection));
            }
            else if (this.entityType == SPPOOTypes.Profession)
            {
                ProfessionVM newProfessionVM = new ProfessionVM { IdProfession = entityId };
                ProfessionVM selectedProffessionToEdit = await this.ProfessionService.GetProfessionByIdAsync(newProfessionVM);
                await this.professionModal.OpenModal(selectedProffessionToEdit, codeAndArea, codeAndProffessionalDirection, areaId, proffessionalDirectionId, this.data.Where(x => x.EntityType == SPPOOTypes.Profession));
            }
            else if (this.entityType == SPPOOTypes.Speciality)
            {
                SpecialityVM newSpecialtyVM = new SpecialityVM() { IdSpeciality = entityId };
                SpecialityVM selectedSpecialityToEdit = await this.SpecialityService.GetSpecialityByIdAsync(newSpecialtyVM);
                await this.specialityModal.OpenModal(selectedSpecialityToEdit, codeAndArea, codeAndProffessionalDirection, codeAndProffession, proffessionId, this.data.Where(x => x.EntityType == SPPOOTypes.Speciality));
            }
        }

        private async Task DeleteDataAsync(string entityType, int entityId)
        {
            bool hasPermission = await CheckUserActionPermission("ManageSPPOOData", false);
            if (!hasPermission) { return; }

            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (confirmed)
            {
                this.entityTypeAfterConfirm = entityType;
                this.entityIdAfterConfirm = entityId;

                if (entityType == SPPOOTypes.Area)
                {
                    string result = await this.AreaService.DeleteAreaAsync(entityId);

                    if (result.Contains("успешно"))
                    {
                        this.toast.sfSuccessToast.Content = result;
                        await this.toast.sfSuccessToast.ShowAsync();
                        this.data = await this.AreaService.LoadSPPOOData(this.areaList, this.professionalDirectionList, this.professionList, this.specialityList);
                        this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();
                    }
                }
                else if (entityType == SPPOOTypes.ProfessionalDirection)
                {
                    string result = await this.ProffessionalDirectionService.DeleteProfessionalDirectionAsync(entityId);

                    if (result.Contains("успешно"))
                    {
                        this.toast.sfSuccessToast.Content = result;
                        await this.toast.sfSuccessToast.ShowAsync();
                        this.data = await this.AreaService.LoadSPPOOData(this.areaList, this.professionalDirectionList, this.professionList, this.specialityList);
                        this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();
                    }
                }
                else if (entityType == SPPOOTypes.Profession)
                {
                    string result = await this.ProfessionService.DeleteProfessionAsync(entityId);

                    if (result.Contains("успешно"))
                    {
                        this.toast.sfSuccessToast.Content = result;
                        await this.toast.sfSuccessToast.ShowAsync();
                        this.data = await this.AreaService.LoadSPPOOData(this.areaList, this.professionalDirectionList, this.professionList, this.specialityList);
                        this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();
                    }
                }
                else if (entityType == SPPOOTypes.Speciality)
                {
                    string result = await this.SpecialityService.DeleteSpecialityAsync(entityId);

                    if (result.Contains("успешно"))
                    {
                        this.toast.sfSuccessToast.Content = result;
                        await this.toast.sfSuccessToast.ShowAsync();
                        this.data = await this.AreaService.LoadSPPOOData(this.areaList, this.professionalDirectionList, this.professionList, this.specialityList);
                        this.SPPOODataList = data.Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();
                    }
                }
            }
        }

        private async Task ExpandElements(int entityId, string entityType)
        {
            var idsToExpand = new List<int>();
            var dataToUse = this.SPPOODataList.ToList();

            if (entityType == SPPOOTypes.Area)
            {
                var professionalDirections = dataToUse.Where(x => x.ParentId == entityId).Select(x => x.Id).ToList();
                var professions = new List<int>();
                foreach (var entry in professionalDirections)
                {
                    var professionsFromEntry = dataToUse.Where(x => x.ParentId == entry).Select(x => x.Id);
                    professions.AddRange(professionsFromEntry);
                }

                professionalDirections.AddRange(professions);
                idsToExpand = professionalDirections.ToList();
            }
            else if (entityType == SPPOOTypes.ProfessionalDirection)
            {
                var professions = dataToUse.Where(x => x.ParentId == entityId).Select(x => x.Id).ToList();
                idsToExpand = professions.ToList();
            }
            else if (entityType == SPPOOTypes.Profession)
            {
                idsToExpand.Add(entityId);
            }

            foreach (var id in idsToExpand)
            {
                if (!this.list.Contains(id))
                {
                    this.list.Add(id);
                    this.currentdata = this.list.ToArray();
                }

                await this.sfTreeGrid.ExpandByKeyAsync(id);
            }
        }

        private List<ContextMenuItemModel> GenerateContextMenuButtons()
        {
            List<ContextMenuItemModel> itemModels = new List<ContextMenuItemModel>();

            switch (this.entityType)
            {
                case SPPOOTypes.Area:
                    itemModels.Add(new ContextMenuItemModel()
                    {
                        Text = ContextMenu.ShowAllElements,
                        IconCss = ContextMenu.ShowAllElementsButtonCss,
                    });
                    itemModels.Add(new ContextMenuItemModel()
                    {
                        Separator = true
                    });
                    itemModels.Add(new ContextMenuItemModel()
                    {
                        Text = ContextMenu.AddProfessionalDirectionButtonText,
                        IconCss = ContextMenu.AddButtonCss,
                    });
                    break;

                case SPPOOTypes.ProfessionalDirection:
                    itemModels.Add(new ContextMenuItemModel()
                    {
                        Text = ContextMenu.ShowAllElements,
                        IconCss = ContextMenu.ShowAllElementsButtonCss,
                    });
                    itemModels.Add(new ContextMenuItemModel()
                    {
                        Separator = true
                    });
                    itemModels.Add(new ContextMenuItemModel()
                    {
                        Text = ContextMenu.AddProfessionButtonText,
                        IconCss = ContextMenu.AddButtonCss,
                    });
                    break;

                case SPPOOTypes.Profession:
                    itemModels.Add(new ContextMenuItemModel()
                    {
                        Text = ContextMenu.ShowAllElements,
                        IconCss = ContextMenu.ShowAllElementsButtonCss,
                    });
                    itemModels.Add(new ContextMenuItemModel()
                    {
                        Separator = true
                    });
                    itemModels.Add(new ContextMenuItemModel()
                    {
                        Text = ContextMenu.AddSpecialityButtonText,
                        IconCss = ContextMenu.AddButtonCss,
                    });
                    break;
            }

            itemModels.Add(new ContextMenuItemModel()
            {
                Text = ContextMenu.EditButtonText,
                IconCss = ContextMenu.EditButtonCss
            });

            //itemModels.Add(new ContextMenuItemModel()
            //{
            //    Text = ContextMenu.DeleteButtonText,
            //    IconCss = ContextMenu.DeleteButtonCss,
            //});

            itemModels.Add(new ContextMenuItemModel()
            {
                Separator = true
            });

            itemModels.Add(new ContextMenuItemModel()
            {
                Text = ContextMenu.CPOButtonText,
                IconCss = ContextMenu.CPOButtonCss,
            });

            itemModels.Add(new ContextMenuItemModel()
            {
                Separator = true
            });

            itemModels.Add(new ContextMenuItemModel()
            {
                Text = ContextMenu.SendNotificationText,
                IconCss = ContextMenu.SendNotificationCSS,
            });

            return itemModels;
        }

        private string SetNewProfessionCode(int id)
        {
            var code = string.Empty;
            var profDir = this.SPPOODataList.Where(x => x.Id == id).FirstOrDefault();
            var profDirProfChildren = this.data.Where(x => x.EntityParentId == profDir?.EntityId && x.IdStatus == DataSourceService.GetActiveStatusID());

            if (profDirProfChildren.Count() == 0)
            {
                code = profDir.Code + "010";
            }
            else
            {
                var lastProf = profDirProfChildren.LastOrDefault();
                code = (int.Parse(lastProf.Code) + 10).ToString();
            }

            return code;
        }

        private string SetNewSpecialityCode(int id)
        {
            var code = string.Empty;
            var profList = this.SPPOODataList.Where(x => x.Id == id);
            var prof = profList.FirstOrDefault();
            var profSpecChildren = this.data.Where(x => x.EntityParentId == prof?.EntityId && x.IdStatus == DataSourceService.GetActiveStatusID());

            if (profSpecChildren.Count() == 0)
            {
                code = prof.Code + 1;
            }
            else
            {
                var lastSpec = profSpecChildren.LastOrDefault();
                code = (int.Parse(lastSpec.Code) + 1).ToString();
            }

            return code;
        }
    }
}
