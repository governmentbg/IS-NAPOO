using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;

using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;

using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

using ISNAPOO.Common.HelperClasses;
using Microsoft.AspNetCore.Components.Forms;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Syncfusion.Blazor.Popups;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.WebSystem.Pages.DOC;
using Syncfusion.Blazor.Grids;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Contracts.DOC;
using Data.Models;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.SPPOO.Modals.Profession
{
    public partial class ProfessionModal : BlazorBaseComponent
    {
        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public IProfessionService ProfessionService { get; set; }
        [Inject]
        public IOrderService OrderService { get; set; }
        [Inject]
        public IKeyValueService KeyValueService { get; set; }
        [Inject]
        public IDOCService DOCService { get; set; }
        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        [Inject]
        public ILocService LocService { get; set; }
        [Inject]
        public IUploadFileService UploadService { get; set; }
        [Inject]
        public IUploadFileService uploadService { get; set; }

        ToastMsg toast;
        SfGrid<OrderVM> ordersGrid = new SfGrid<OrderVM>();
        DialogEffect AnimationEffect = DialogEffect.Zoom;
        EditDOC editDOCModal = new EditDOC();

        bool showConfirmDialog = false;
        bool closeConfirmed = false;
        ProfessionVM professionVM = new ProfessionVM();
        bool isSubmitClicked = false;
        EditContext editContext;
        DocVM docVM = new DocVM();
        KeyValueVM status = new KeyValueVM();
        IEnumerable<SPPOOTreeGridData> professions;
        int idOrder;
        IEnumerable<OrderVM> orders = new List<OrderVM>();
        List<OrderVM> addedOrders = new List<OrderVM>();
        IEnumerable<KeyValueVM> orderChangeValues = new List<KeyValueVM>();
        string orderType = string.Empty;
        private string CreationDateStr = "";
        private string ModifyDateStr = "";
        bool isDisabled = true;
        OrderVM orderToDelete = new OrderVM();
        private IEnumerable<KeyValueVM> legalCapacityOrdinanceTypeSource = new List<KeyValueVM>();
        private string legalCapacityDescription = string.Empty;

        ValidationMessageStore? messageStore;

        public override bool IsContextModified
        {
            get => this.editContext.IsModified();
        }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.professionVM);
            this.isVisible = false;
            this.professionVM = new ProfessionVM();
        }

        private async Task OpenDOCModalHandler()
        {
            await this.editDOCModal.OpenModal(this.docVM);
        }

        private async Task DownloadDOCHandler()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.UploadService.CheckIfExistUploadedFileAsync<global::Data.Models.Data.DOC.DOC>(this.docVM.IdDOC);
                if (hasFile)
                {
                    var documentStream = await this.UploadService.GetUploadedFileAsync<global::Data.Models.Data.DOC.DOC>(docVM.IdDOC);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JSRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JSRuntime, this.docVM.FileName, documentStream.MS!.ToArray());
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

        public async Task OpenModal(ProfessionVM professionVM, string codeAndArea, string codeAndProffessionalDirection, int areaId, int proffessionalDirectionId, IEnumerable<SPPOOTreeGridData> professions)
        {
            var kvTypeChange = await this.DataSourceService.GetKeyValueByIntCodeAsync("SPPOOOrderChange", "Created");
            this.professions = professions;
            this.editContext = new EditContext(this.professionVM);
            this.professionVM = professionVM;
            this.professionVM.CodeAndArea = codeAndArea;
            this.professionVM.CodeAndProffessionalDirection = codeAndProffessionalDirection;
            this.professionVM.IdArea = areaId;
            this.professionVM.IdProfessionalDirection = proffessionalDirectionId;
            if (this.professionVM.IdStatus == 0)
            {
                this.professionVM.IdStatus = DataSourceService.GetWorkStatusID();
            }

            if (this.professionVM.IdProfession == 0)
            {
                this.professionVM.IdTypeChange = kvTypeChange.IdKeyValue;
            }

            this.legalCapacityOrdinanceTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LegalCapacityOrdinanceType");

            this.status = await this.DataSourceService.GetKeyValueByIdAsync(this.professionVM.IdStatus);
            this.isVisible = true;
            this.idOrder = 0;
            this.professionVM.IdTypeChange = 0;
            this.addedOrders = new List<OrderVM>();
            this.orderType = string.Empty;

            if (this.professionVM.IdProfession != 0)
            {
                List<int> idsOrders = new List<int>();

                foreach (var professionOrder in this.professionVM.ProfessionOrders)
                {
                    idsOrders.Add(professionOrder.IdSPPOOOrder);
                }

                List<OrderVM> orders = await this.OrderService.GetOrdersByIdsAsync(idsOrders);
                foreach (var order2 in orders)
                {
                    order2.IdTypeChange = this.professionVM.ProfessionOrders.FirstOrDefault(x => x.IdSPPOOOrder == order2.IdOrder).IdTypeChange;
                    KeyValueVM keyValueVM = await this.DataSourceService.GetKeyValueByIdAsync(order2.IdTypeChange);
                    order2.OrderType = keyValueVM.Name;
                    this.addedOrders.Add(order2);
                }

                if (this.professionVM.ProfessionOrders.Count != 0)
                {
                    if (this.status == null)
                    {
                        this.orderType = this.addedOrders.LastOrDefault().OrderType;

                        await this.HandleOrderType();
                    }
                }
            }

            this.orders = await this.OrderService.GetAllOrdersAsync();

            foreach (var order in this.orders)
            {
                order.OrderNumberWithOrderDate = $"{order.OrderNumber}/{order.OrderDate?.ToString("dd.MM.yyyy")} г.";
            }

            this.orderChangeValues = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.OrderChangeSPPOO);
            this.docVM = await this.DOCService.GetActiveDocByProfessionIdAsync(this.professionVM);
            if (this.professionVM.IdProfession == 0)
            {
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                this.professionVM.ModifyPersonName = "";
                this.professionVM.CreatePersonName = "";
            }
            else
            {
                this.CreationDateStr = this.professionVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.professionVM.ModifyDate.ToString("dd.MM.yyyy");
                this.professionVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.professionVM.IdModifyUser);
                this.professionVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.professionVM.IdCreateUser);
            }
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
                if (this.professionVM.IdTypeChange == 0)
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
                        KeyValueVM keyValueVM = await this.DataSourceService.GetKeyValueByIdAsync(this.professionVM.IdTypeChange);
                        addedOrder.OrderType = keyValueVM.Name;
                        addedOrder.IdTypeChange = this.professionVM.IdTypeChange;
                        this.addedOrders.Add(addedOrder);
                        this.orderType = addedOrder.OrderType;

                        await this.HandleOrderType();

                        this.ordersGrid.Refresh();
                        this.idOrder = 0;
                        this.professionVM.IdTypeChange = 0;
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
                    this.professionVM.IdStatus = DataSourceService.GetWorkStatusID();
                }
            }
        }

        private async Task HandleOrderType()
        {
            switch (this.orderType)
            {
                case GlobalConstants.ORDER_ADD:
                    this.professionVM.IdStatus = DataSourceService.GetActiveStatusID();
                    break;
                //case GlobalConstants.ORDER_CHANGE:
                //    this.professionVM.IdStatus = DataSourceService.GetWorkStatusID();
                //    break;
                case GlobalConstants.ORDER_REMOVE:
                    this.professionVM.IdStatus = DataSourceService.GetRemoveStatusID();
                    break;
            }

            this.status = await this.DataSourceService.GetKeyValueByIdAsync(this.professionVM.IdStatus);
        }

        private async Task SubmitHandler()
        {
            this.editContext = new EditContext(this.professionVM);
            this.editContext.OnValidationRequested += ValidateCode;
            this.editContext.OnValidationRequested += ValidateLegalOrdinanceTypeSelected;
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.addedOrders.Count > 0)
            {
                this.addedOrders.ForEach((order) =>
                {
                    if (!this.professionVM.OrderVMs.Contains(order))
                    {
                        this.professionVM.OrderVMs.Add(order);
                    }
                });
            }

            string msg = string.Empty;
            bool isValid = this.editContext.Validate();

            if (isValid)
            {
                this.SpinnerShow();
                this.isSubmitClicked = true;
                if (this.professionVM.IdProfession == 0)
                {
                    msg = await this.ProfessionService.CreateProfessionAsync(this.professionVM);
                }
                else
                {
                    msg = await this.ProfessionService.UpdateProfessionAsync(this.professionVM);
                }

                var areaInfo = this.professionVM.CodeAndArea;
                var professionalDirectionInfo = this.professionVM.CodeAndProffessionalDirection;
                this.professionVM = await this.ProfessionService.GetOnlyProfessionByIdAsync(this.professionVM.IdProfession);
                this.professionVM.CodeAndArea = areaInfo;
                this.professionVM.CodeAndProffessionalDirection = professionalDirectionInfo;
                this.CreationDateStr = this.professionVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.professionVM.ModifyDate.ToString("dd.MM.yyyy");
                this.professionVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.professionVM.IdModifyUser);
                this.professionVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.professionVM.IdCreateUser);

                this.DataSourceService.ReloadProfessions();

                await this.OnSubmit.InvokeAsync(msg);
                this.isSubmitClicked = false;
                this.SpinnerHide();
            }
        }

        private void ValidateCode(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            var profession = this.professions.FirstOrDefault(x => x.Code == this.professionVM.Code && x.EntityId != this.professionVM.IdProfession && x.IdStatus == DataSourceService.GetActiveStatusID());

            if (profession is not null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.professionVM, "Code");
                this.messageStore?.Add(fi, "Професия с този код вече съществува!");
            }
        }

        private void ValidateLegalOrdinanceTypeSelected(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.professionVM.IsPresupposeLegalCapacity && this.professionVM.IdLegalCapacityOrdinanceType is null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.professionVM, "IdLegalCapacityOrdinanceType");
                this.messageStore?.Add(fi, "Полето 'Наредба за правоспособност' е задължително!");
            }
        }

        private async Task SendNotificationAsync()
        {
            await this.LoadDataForPersonsToSendNotificationToAsync(SPPOOTypes.Profession, this.professionVM.IdProfession);

            if (!this.personIds.Any())
            {
                await this.ShowErrorAsync("Няма активни лицензирани ЦПО по избраните специалности!");
            }
            else
            {
                await this.OpenSendNotificationModal(true, this.personIds);
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
                        await FileUtils.SaveAs(this.JSRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JSRuntime, model.FileName, documentStream.MS!.ToArray());
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

        private void OnLegalCapacityValueChanged(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args is not null && args.ItemData is not null)
            {
                this.legalCapacityDescription = args.ItemData.Description;
            }
            else
            {
                this.legalCapacityDescription = string.Empty;
            }
        }
    }
}
