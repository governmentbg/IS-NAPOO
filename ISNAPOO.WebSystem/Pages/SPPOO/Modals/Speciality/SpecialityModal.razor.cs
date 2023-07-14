using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ISNAPOO.Core.Contracts.DOC.NKPD;
using ISNAPOO.Core.Contracts.DOC;
using Syncfusion.Blazor.Grids;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using Microsoft.AspNetCore.Components.Forms;
using ISNAPOO.WebSystem.Pages.DOC.NKPD;
using ISNAPOO.Core.ViewModels.DOC.NKPD;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Common.Constants;
using Syncfusion.Blazor.Popups;
using ISNAPOO.WebSystem.Pages.DOC;
using Syncfusion.Blazor.DropDowns;
using Data.Models;

namespace ISNAPOO.WebSystem.Pages.SPPOO.Modals.Speciality
{
    public partial class SpecialityModal : BlazorBaseComponent
    {
        [Inject]
        public IUploadFileService UploadService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public INKPDService NKPDService { get; set; }
        [Inject]
        public IDOCService DocService { get; set; }
        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        [Inject]
        public IOrderService OrderService { get; set; }
        [Inject]
        public IKeyValueService KeyValueService { get; set; }
        [Inject]
        public IProfessionService ProfessionService { get; set; }
        [Inject]
        public ISpecialityService SpecialityService { get; set; }
        [Inject]
        public ILocService LocService { get; set; }
        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
        [Inject]
        public IUploadFileService uploadService { get; set; }

        ToastMsg toast;
        SfGrid<OrderVM> ordersGrid = new SfGrid<OrderVM>();
        EditDOC editDOCModal = new EditDOC();
        DialogEffect AnimationEffect = DialogEffect.Zoom;

        bool showConfirmDialog = false;
        bool closeConfirmed = false;
        string orderType = string.Empty;
        string hideProfDescr = string.Empty;
        int idOrder = 0;
        SpecialityVM specialityVM;
        bool isDisabled = true;
        List<int> rowsIds = new List<int>();
        string professionName = string.Empty;
        bool isSubmitClicked = false;
        DocVM docVM = new DocVM();
        KeyValueVM status = new KeyValueVM();
        private string CreationDateStr = "";
        private string ModifyDateStr = "";
        OrderVM orderToDelete = new OrderVM();
        private bool IsNKR = false;
        int idNKPDToDelete = 0;

        IEnumerable<KeyValueVM> spkValue;
        IEnumerable<KeyValueVM> nkrValue;
        IEnumerable<KeyValueVM> ekrValue;
        IEnumerable<KeyValueVM> orderChangeValues;
        IEnumerable<DocVM> docs;
        IEnumerable<KeyValueVM> statusSPOOOValues;
        IEnumerable<SPPOOTreeGridData> specialities;

        List<NKPDVM> nkpds = new List<NKPDVM>();

        IEnumerable<OrderVM> orders;
        List<OrderVM> addedOrders = new List<OrderVM>();
        IEnumerable<NKPDVM> nKPDVMList = new List<NKPDVM>();
        SfGrid<NKPDVM> sfGrid = new SfGrid<NKPDVM>();

        NkpdSelectorModal nkpdSelectorModal = new NkpdSelectorModal();
        EditContext editContext;

        ValidationMessageStore? messageStore;

        public override bool IsContextModified
        {
            get => this.editContext.IsModified();
        }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        protected override void OnInitialized()
        {
            this.specialityVM = new SpecialityVM();
            this.editContext = new EditContext(this.specialityVM);
            this.isVisible = false;
            this.specialityVM = new SpecialityVM();
        }

        private async Task OpenDOCModalHandler(int? docId)
        {
            var docVM = await this.DocService.GetDOCByIdAsync(new DocVM() { IdDOC = docId ?? default });
            await this.editDOCModal.OpenModal(docVM);
        }

        private async Task DownloadDOCHandler(int? docId)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var docVM = await this.DocService.GetDOCByIdAsync(new DocVM() { IdDOC = docId ?? default });
                var hasFile = await this.UploadService.CheckIfExistUploadedFileAsync<global::Data.Models.Data.DOC.DOC>(docVM.IdDOC);
                if (hasFile)
                {
                    var documentStream = await this.UploadService.GetUploadedFileAsync<global::Data.Models.Data.DOC.DOC>(docVM.IdDOC);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, this.docVM.FileName, documentStream.MS!.ToArray());
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

        private async Task OnAddOrderClickHandler()
        {
            if (this.idOrder == 0)
            {
                var msg = this.LocService.GetLocalizedHtmlString("OrderNotChoosed").Value;

                this.toast.sfErrorToast.Content = msg;
                await this.toast.sfErrorToast.ShowAsync();
            }
            else
            {
                if (this.specialityVM.IdTypeChange == 0)
                {
                    var msg = this.LocService.GetLocalizedHtmlString("TypeOfChangeNotChoosed").Value;

                    this.toast.sfErrorToast.Content = msg;
                    await this.toast.sfErrorToast.ShowAsync();
                }
                else
                {
                    if (!this.addedOrders.Any(x => x.IdOrder == this.idOrder))
                    {
                        OrderVM addedOrder = await this.OrderService.GetOrderByIdAsync(this.idOrder);
                        KeyValueVM keyValueVM = await this.DataSourceService.GetKeyValueByIdAsync(this.specialityVM.IdTypeChange);
                        addedOrder.OrderType = keyValueVM.Name;
                        addedOrder.IdTypeChange = this.specialityVM.IdTypeChange;
                        this.addedOrders.Add(addedOrder);
                        this.orderType = addedOrder.OrderType;

                        await this.HandleOrderType();

                        this.ordersGrid.Refresh();
                        this.idOrder = 0;
                        this.specialityVM.IdTypeChange = 0;
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("OrderAlreadyAdded").Value;

                        this.toast.sfErrorToast.Content = msg;
                        await this.toast.sfErrorToast.ShowAsync();
                    }
                }
            }
        }
        private async Task OpenLinkNIP()
        {
            if (!string.IsNullOrEmpty(this.specialityVM.LinkNIP))
            {
                if (this.specialityVM.LinkNIP[0] != 'w')
                {
                    await JsRuntime.InvokeVoidAsync("open", $"{this.specialityVM.LinkNIP}", "_blank");
                }
                else
                {
                    await JsRuntime.InvokeVoidAsync("open", $"https://{this.specialityVM.LinkNIP}", "_blank");
                }
            }
        }
        private async Task OpenLinkMON()
        {
            if (!string.IsNullOrEmpty(this.specialityVM.LinkMON))
            {
                if (this.specialityVM.LinkMON[0] != 'w')
                {
                    await JsRuntime.InvokeVoidAsync("open", $"{this.specialityVM.LinkMON}", "_blank");
                }
                else
                {
                    await JsRuntime.InvokeVoidAsync("open", $"https://{this.specialityVM.LinkMON}", "_blank");
                }
            }
        }

        private async Task HandleOrderType()
        {
            switch (this.orderType)
            {
                case GlobalConstants.ORDER_ADD:
                    this.specialityVM.IdStatus = DataSourceService.GetActiveStatusID();
                    break;
                //case GlobalConstants.ORDER_CHANGE:
                //    this.specialityVM.IdStatus = DataSourceService.GetActiveStatusID();
                //    break;
                case GlobalConstants.ORDER_REMOVE:
                    this.specialityVM.IdStatus = DataSourceService.GetRemoveStatusID();
                    break;
            }

            this.status = await this.DataSourceService.GetKeyValueByIdAsync(this.specialityVM.IdStatus);
        }

        private async Task DeleteRowNkpd(int id)
        {
            this.idNKPDToDelete = id;
            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                this.rowsIds.Remove(id);
                toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                await toast.sfSuccessToast.ShowAsync();
                var nkpds = await this.NKPDService.GetNKPDsByIdsAsync(this.rowsIds);
                this.nKPDVMList = nkpds.OrderBy(n => n.Code);
            }
        }
        private async Task OnDownloadClick(OrderVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.uploadService.CheckIfExistUploadedFileAsync<SPPOOOrder>(model.IdOrder);
                if (hasFile)
                {
                    var documentStream = await this.uploadService.GetUploadedFileAsync<SPPOOOrder>(model.IdOrder);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, model.FileName, documentStream.MS!.ToArray());
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

        private async Task DeleteRowOrder(OrderVM orderVM)
        {
            this.orderToDelete = orderVM;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
               this.addedOrders.Remove(orderToDelete);
                toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                await toast.sfSuccessToast.ShowAsync();
                this.ordersGrid.Refresh();
                if (this.addedOrders.Count > 0)
                {
                    this.orderType = this.addedOrders.FirstOrDefault().OrderType;

                    await this.HandleOrderType();
                }

                if (this.addedOrders.Count == 0)
                {
                    this.orderType = string.Empty;
                    this.status.Name = "Работен";
                    this.specialityVM.IdStatus = DataSourceService.GetWorkStatusID();
                }
            }
        }

        private async Task OpenNkpdSelectorModal()
        {
            await this.nkpdSelectorModal.Open();
        }

        private async Task NKPDOnSelectHandler(List<int> rowsIds)
        {
            rowsIds.ForEach((nkpdId) =>
            {
                if (!this.rowsIds.Contains(nkpdId))
                {
                    this.rowsIds.Add(nkpdId);
                }
            });

            var nkpds = await this.NKPDService.GetNKPDsByIdsAsync(this.rowsIds);
            this.nKPDVMList = nkpds.OrderBy(n => n.Code);
        }

        private async Task OnDOCValueChange(ChangeEventArgs<int?, DocVM> args)
        {
            if (args.ItemData != null)
            {
                docVM = await this.DocService.GetDOCByIdAsync(args.ItemData);
                this.hideProfDescr = string.Empty;
            }
            else
            {
                docVM = new DocVM();
                this.hideProfDescr = "display: none";
            }
        }

        public async Task OpenModal(SpecialityVM specialityVM, string codeAndArea, string codeAndProffessionalDirection, string codeAndProffession, int proffessionId, IEnumerable<SPPOOTreeGridData> specialities)
        {
            var kvTypeChange = await this.DataSourceService.GetKeyValueByIntCodeAsync("SPPOOOrderChange", "Created");
            this.specialities = specialities;
            this.specialityVM = specialityVM;
            this.editContext = new EditContext(this.specialityVM);
            this.specialityVM.CodeAndArea = codeAndArea;
            this.specialityVM.CodeAndProfessionalDirection = codeAndProffessionalDirection;
            this.specialityVM.CodeAndProfession = codeAndProffession;
            this.specialityVM.IdProfession = proffessionId;
            if (this.specialityVM.IdStatus == 0)
            {
                this.specialityVM.IdStatus = DataSourceService.GetWorkStatusID();
            }

            if (this.specialityVM.IdSpeciality == 0)
            {
                this.specialityVM.IdTypeChange = kvTypeChange.IdKeyValue;
            }

            this.status = await this.DataSourceService.GetKeyValueByIdAsync(this.specialityVM.IdStatus);
            this.isVisible = true;
            this.docVM = new DocVM();
            this.addedOrders = new List<OrderVM>();
            this.rowsIds = new List<int>();
            this.nKPDVMList = new List<NKPDVM>();
            this.orderType = string.Empty;

            if (this.specialityVM.IdSpeciality != 0)
            {
                List<int> idsOrders = new List<int>();

                foreach (var specialityOrder in this.specialityVM.SpecialityOrders)
                {
                    idsOrders.Add(specialityOrder.IdSPPOOOrder);
                }

                List<OrderVM> orders = await this.OrderService.GetOrdersByIdsAsync(idsOrders);
                foreach (var order2 in orders)
                {
                    order2.IdTypeChange = this.specialityVM.SpecialityOrders.FirstOrDefault(x => x.IdSPPOOOrder == order2.IdOrder).IdTypeChange;
                    KeyValueVM keyValueVM = await this.DataSourceService.GetKeyValueByIdAsync(order2.IdTypeChange);
                    order2.OrderType = keyValueVM.Name;
                    this.addedOrders.Add(order2);
                }

                if (this.specialityVM.SpecialityOrders.Count != 0)
                {
                    if (this.status == null)
                    {
                        this.orderType = this.addedOrders.LastOrDefault().OrderType;

                        await this.HandleOrderType();
                    }
                }

                if (this.specialityVM.SpecialityNKPDs.Count != 0)
                {
                    List<int> idsNKPDs = new List<int>();

                    foreach (var specialityNKPD in this.specialityVM.SpecialityNKPDs)
                    {
                        idsNKPDs.Add(specialityNKPD.IdNKPD);
                    }

                    this.rowsIds.AddRange(idsNKPDs);
                    IEnumerable<NKPDVM> nKPDVMs = await this.NKPDService.GetNKPDsByIdsAsync(idsNKPDs);
                    this.nkpds = new List<NKPDVM>();

                    foreach (var nkpdVM in nKPDVMs)
                    {
                        this.nkpds.Add(nkpdVM);
                    }

                    this.nKPDVMList = this.nkpds.OrderBy(n => n.Code);
                }

                if (this.specialityVM.IdDOC != null)
                {
                    DocVM doc = new DocVM { IdDOC = this.specialityVM.IdDOC ?? default };
                    docVM = await this.DocService.GetDOCByIdAsync(doc);
                }
            }

            if (this.specialityVM.IdDOC == 0)
            {
                this.hideProfDescr = "display: none";
            }

            OrderVM order = new OrderVM();

            this.spkValue = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.SpkVQS);
            this.nkrValue = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.NKRLevel);
            this.ekrValue = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.EKRLevel);
            this.orderChangeValues = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.OrderChangeSPPOO);
            this.statusSPOOOValues = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.StatusSPPOO);
            this.orders = await this.OrderService.GetAllOrdersAsync();

            foreach (var orderFromOrders in this.orders)
            {
                orderFromOrders.OrderNumberWithOrderDate = $"{orderFromOrders.OrderNumber}/{orderFromOrders.OrderDate?.ToString("dd.MM.yyyy")} г.";
            }

            var allDOCs = await this.DocService.GetAllDocAsync();
            this.docs = allDOCs.Where(d => d.IdProfession == specialityVM.IdProfession).ToList();
            if (this.specialityVM.IdSpeciality == 0)
            {
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                this.specialityVM.ModifyPersonName = "";
                this.specialityVM.CreatePersonName = "";
            }
            else
            {
                this.CreationDateStr = this.specialityVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.specialityVM.ModifyDate.ToString("dd.MM.yyyy");
                this.specialityVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.specialityVM.IdModifyUser);
                this.specialityVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.specialityVM.IdCreateUser);
            }
        }
        private void ChangeBoth()
        {
            if (this.IsNKR)
            {
                this.IsNKR = false;
                if (specialityVM.IdNKRLevel != 0)
                {
                    string nkrKeyValueIntCode = nkrValue.FirstOrDefault(n => n.IdKeyValue == specialityVM.IdNKRLevel).KeyValueIntCode;
                    specialityVM.IdEKRLevel = ekrValue.FirstOrDefault(e => e.KeyValueIntCode == nkrKeyValueIntCode).IdKeyValue;
                }
                else
                {
                    specialityVM.IdEKRLevel = 0;
                }
            }
            else
            {
                if (specialityVM.IdEKRLevel != 0)
                {
                    string ekrKeyValueIntCode = ekrValue.FirstOrDefault(n => n.IdKeyValue == specialityVM.IdEKRLevel).KeyValueIntCode;
                    specialityVM.IdNKRLevel = nkrValue.FirstOrDefault(e => e.KeyValueIntCode == ekrKeyValueIntCode).IdKeyValue;
                }
                else
                {
                    specialityVM.IdNKRLevel = 0;
                }
            }

        }

        private async Task SubmitHandler()
        {

            this.editContext = new EditContext(this.specialityVM);
            this.editContext.OnValidationRequested += ValidateCode;
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.addedOrders.Count > 0)
            {
                this.addedOrders.ForEach((order) =>
                {
                    if (!this.specialityVM.OrderVMs.Contains(order))
                    {
                        this.specialityVM.OrderVMs.Add(order);
                    }
                });
            }

            if (this.nKPDVMList.Count() > 0)
            {
                foreach (var nkpd in this.nKPDVMList)
                {
                    if (!this.specialityVM.IdsNkpd.Contains(nkpd.IdNKPD))
                    {
                        this.specialityVM.IdsNkpd.Add(nkpd.IdNKPD);
                    }
                }
            }

            string msg = string.Empty;

            bool isValid = this.editContext.Validate();

            if (isValid)
            {
                this.SpinnerShow();
                isSubmitClicked = true;

                if (this.specialityVM.IdSpeciality == 0)
                {
                    msg = await this.SpecialityService.CreateSpecialityAsync(this.specialityVM);
                }
                else
                {
                    msg = await this.SpecialityService.UpdateSpecialityAsync(this.specialityVM);
                }

                var areaInfo = this.specialityVM.CodeAndArea;
                var pdInfo = this.specialityVM.CodeAndProfessionalDirection;
                var professionInfo = this.specialityVM.CodeAndProfession;
                this.specialityVM = await this.SpecialityService.GetSpecialityByIdAsync(this.specialityVM.IdSpeciality);
                this.specialityVM.CodeAndArea = areaInfo;
                this.specialityVM.CodeAndProfessionalDirection = pdInfo;
                this.specialityVM.CodeAndProfession = professionInfo;
                this.CreationDateStr = this.specialityVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.specialityVM.ModifyDate.ToString("dd.MM.yyyy");
                this.specialityVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.specialityVM.IdModifyUser);
                this.specialityVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.specialityVM.IdCreateUser);

                this.DataSourceService.ReloadSpecialities();

                await this.OnSubmit.InvokeAsync(msg);
                this.isSubmitClicked = false;
                this.SpinnerHide();
            }
        }

        private void ValidateCode(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            var speciality = this.specialities.FirstOrDefault(x => x.Code == this.specialityVM.Code && x.EntityId != this.specialityVM.IdSpeciality && x.IdStatus == DataSourceService.GetActiveStatusID());

            if (speciality is not null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.specialityVM, "Code");
                this.messageStore?.Add(fi, "Специалност с този код вече съществува!");
            }
        }

        private async Task SendNotificationAsync()
        {
            await this.LoadDataForPersonsToSendNotificationToAsync(SPPOOTypes.Speciality, this.specialityVM.IdSpeciality);

            if (!this.personIds.Any())
            {
                await this.ShowErrorAsync("Няма активни лицензирани ЦПО по избраната специалност!");
            }
            else
            {
                await this.OpenSendNotificationModal(true, this.personIds);
            }
        }
    }
}
