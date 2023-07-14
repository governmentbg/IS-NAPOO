using System;
using Data.Models;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.SPPOO.Modals.ProfessionalDirection
{
    public partial class ProfessionalDirectionModal : BlazorBaseComponent
    {
        [Inject]
        public IProfessionalDirectionService ProffessionalDirectionService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
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
        DialogEffect AnimationEffect = DialogEffect.Zoom;
        bool showConfirmDialog = false;
        bool closeConfirmed = false;
        ProfessionalDirectionVM professionalDirectionVM = new ProfessionalDirectionVM();
        bool isDisabled = true;
        string orderType = string.Empty;
        bool isSubmitClicked = false;
        private string CreationDateStr = "";
        private string ModifyDateStr = "";
        EditContext editContext;
        int idOrder = 0;
        IEnumerable<KeyValueVM> orderChangeValues = new List<KeyValueVM>();
        IEnumerable<OrderVM> orders = new List<OrderVM>();
        IEnumerable<SPPOOTreeGridData> professionalDirections = new List<SPPOOTreeGridData>();
        List<OrderVM> addedOrders = new List<OrderVM>();
        SfGrid<OrderVM> ordersGrid = new SfGrid<OrderVM>();
        OrderVM orderToDelete = new OrderVM();

        KeyValueVM status = new KeyValueVM();

        ValidationMessageStore? messageStore;

        public override bool IsContextModified
        {
            get => this.editContext.IsModified();
        }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.professionalDirectionVM);
            this.isVisible = false;
            this.professionalDirectionVM = new ProfessionalDirectionVM();
        }

        public async Task OpenModal(ProfessionalDirectionVM proffessionalDirectionVM, string codeAndArea, int id, IEnumerable<SPPOOTreeGridData> professionalDirections)
        {
            var kvTypeChange = await this.DataSourceService.GetKeyValueByIntCodeAsync("SPPOOOrderChange", "Created");
            this.professionalDirections = professionalDirections;
            this.editContext = new EditContext(this.professionalDirectionVM);
            this.professionalDirectionVM = proffessionalDirectionVM;
            this.professionalDirectionVM.CodeAndArea = codeAndArea;
            this.professionalDirectionVM.IdArea = id;
            if (this.professionalDirectionVM.IdStatus == 0)
            {
                this.professionalDirectionVM.IdStatus = DataSourceService.GetWorkStatusID();
            }

            if (this.professionalDirectionVM.IdProfessionalDirection == 0)
            {
                this.professionalDirectionVM.IdTypeChange = kvTypeChange.IdKeyValue;
            }

            this.status = await this.DataSourceService.GetKeyValueByIdAsync(this.professionalDirectionVM.IdStatus);
            this.addedOrders = new List<OrderVM>();
            this.orderType = string.Empty;
            this.isVisible = true;
            this.idOrder = 0;
            this.professionalDirectionVM.IdTypeChange = 0;

            if (this.professionalDirectionVM.IdProfessionalDirection != 0)
            {
                List<int> idsOrders = new List<int>();

                foreach (var specialityOrder in this.professionalDirectionVM.ProfessionalDirectionOrders)
                {
                    idsOrders.Add(specialityOrder.IdSPPOOOrder);
                }

                List<OrderVM> orders = await this.OrderService.GetOrdersByIdsAsync(idsOrders);
                foreach (var order2 in orders)
                {
                    order2.IdTypeChange = this.professionalDirectionVM.ProfessionalDirectionOrders.FirstOrDefault(x => x.IdSPPOOOrder == order2.IdOrder).IdTypeChange;
                    KeyValueVM keyValueVM = await this.DataSourceService.GetKeyValueByIdAsync(order2.IdTypeChange);
                    order2.OrderType = keyValueVM.Name;
                    this.addedOrders.Add(order2);
                }

                if (this.professionalDirectionVM.ProfessionalDirectionOrders.Count != 0)
                {
                    if (this.status == null)
                    {
                        this.orderType = this.addedOrders.LastOrDefault().OrderType;

                        await this.HandleOrderType();
                    }
                }
            }

            this.orders = await this.OrderService.GetAllOrdersAsync();
            this.orderChangeValues = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.OrderChangeSPPOO);

            foreach (var order in this.orders)
            {
                order.OrderNumberWithOrderDate = $"{order.OrderNumber}/{order.OrderDate?.ToString("dd.MM.yyyy")} г.";
            }
            if (this.professionalDirectionVM.IdProfessionalDirection == 0)
            {
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                this.professionalDirectionVM.ModifyPersonName = "";
                this.professionalDirectionVM.CreatePersonName = "";
            }
            else
            {
                this.CreationDateStr = professionalDirectionVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = professionalDirectionVM.ModifyDate.ToString("dd.MM.yyyy");
                this.professionalDirectionVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.professionalDirectionVM.IdModifyUser);
                this.professionalDirectionVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.professionalDirectionVM.IdCreateUser);
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
                if (this.professionalDirectionVM.IdTypeChange == 0)
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
                        KeyValueVM keyValueVM = await this.DataSourceService.GetKeyValueByIdAsync(this.professionalDirectionVM.IdTypeChange);
                        addedOrder.OrderType = keyValueVM.Name;
                        addedOrder.IdTypeChange = this.professionalDirectionVM.IdTypeChange;
                        this.addedOrders.Add(addedOrder);
                        this.orderType = addedOrder.OrderType;

                        await this.HandleOrderType();

                        this.ordersGrid.Refresh();
                        this.idOrder = 0;
                        this.professionalDirectionVM.IdTypeChange = 0;
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
                    this.professionalDirectionVM.IdStatus = DataSourceService.GetWorkStatusID();
                }
            }
        }

        private async Task HandleOrderType()
        {
            switch (this.orderType)
            {
                case GlobalConstants.ORDER_ADD:
                    this.professionalDirectionVM.IdStatus = DataSourceService.GetActiveStatusID();
                    break;
                case GlobalConstants.ORDER_CHANGE:
                    this.professionalDirectionVM.IdStatus = DataSourceService.GetActiveStatusID();
                    break;
                case GlobalConstants.ORDER_REMOVE:
                    this.professionalDirectionVM.IdStatus = DataSourceService.GetRemoveStatusID();
                    break;
            }

            this.status = await this.DataSourceService.GetKeyValueByIdAsync(this.professionalDirectionVM.IdStatus);
        }

        private async Task SubmitHandler()
        {
            this.editContext = new EditContext(this.professionalDirectionVM);
            this.editContext.OnValidationRequested += ValidateCode;
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.addedOrders.Count > 0)
            {
                this.addedOrders.ForEach((order) =>
                {
                    if (!this.professionalDirectionVM.OrderVMs.Contains(order))
                    {
                        this.professionalDirectionVM.OrderVMs.Add(order);
                    }
                });
            }
            else
            {
                this.professionalDirectionVM.IdStatus = DataSourceService.GetWorkStatusID();
            }

            string msg = string.Empty;
            bool isValid = this.editContext.Validate();

            if (isValid)
            {
                this.SpinnerShow();
                this.isSubmitClicked = true;
                await this.UpdateAllOrdersForChildrenHandler();

                if (this.professionalDirectionVM.IdProfessionalDirection == 0)
                {
                    msg = await this.ProffessionalDirectionService.CreateProfessionalDirectionAsync(this.professionalDirectionVM);
                }
                else
                {
                    msg = await this.ProffessionalDirectionService.UpdateProfessionalDirectionAsync(this.professionalDirectionVM);
                }

                var areaInfo = this.professionalDirectionVM.CodeAndArea;
                this.professionalDirectionVM = await this.ProffessionalDirectionService.GetProfessionalDirectionByIdAsync(new ProfessionalDirectionVM() {IdProfessionalDirection = this.professionalDirectionVM.IdProfessionalDirection });
                this.professionalDirectionVM.CodeAndArea = areaInfo;
                this.CreationDateStr = professionalDirectionVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = professionalDirectionVM.ModifyDate.ToString("dd.MM.yyyy");
                this.professionalDirectionVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.professionalDirectionVM.IdModifyUser);
                this.professionalDirectionVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.professionalDirectionVM.IdCreateUser);

                this.DataSourceService.ReloadProfessionalDirections();

                await this.OnSubmit.InvokeAsync(msg);
                this.isSubmitClicked = false;
                this.SpinnerHide();
            }
        }

        private async Task UpdateAllOrdersForChildrenHandler()
        {
            if (this.professionalDirectionVM.UpdateAllOrders)
            {
                IEnumerable<ProfessionVM>
        childProfessions = await this.ProfessionService.GetAllActiveProfessionsByProfessionalDirectionIdAsync(this.professionalDirectionVM);

                if (childProfessions.Count() > 0)
                {
                    var lastOrder = this.addedOrders.LastOrDefault();

                    foreach (var profession in childProfessions)
                    {
                        if (this.addedOrders.Count > 0)
                        {
                            this.addedOrders.ForEach((order) =>
                            {
                                if (!profession.OrderVMs.Contains(order))
                                {
                                    profession.OrderVMs.Add(order);
                                }
                            });
                        }

                        switch (lastOrder.OrderType)
                        {
                            case GlobalConstants.ORDER_ADD:
                                profession.IdStatus = DataSourceService.GetActiveStatusID();
                                break;
                            case GlobalConstants.ORDER_CHANGE:
                                profession.IdStatus = DataSourceService.GetActiveStatusID();
                                break;
                            case GlobalConstants.ORDER_REMOVE:
                                profession.IdStatus = DataSourceService.GetRemoveStatusID();
                                break;
                        }

                        await this.ProfessionService.UpdateProfessionOrdersAsync(profession);

                        IEnumerable<SpecialityVM>
                            childSpecialities = await this.SpecialityService.GetAllActiveSpecialitiesByProfessionIdAsync(profession);

                        if (childSpecialities.Count() > 0)
                        {
                            foreach (var speciality in childSpecialities)
                            {
                                if (this.addedOrders.Count > 0)
                                {
                                    this.addedOrders.ForEach((order) =>
                                    {
                                        if (!speciality.OrderVMs.Contains(order))
                                        {
                                            speciality.OrderVMs.Add(order);
                                        }
                                    });
                                }

                                switch (lastOrder.OrderType)
                                {
                                    case GlobalConstants.ORDER_ADD:
                                        speciality.IdStatus = DataSourceService.GetActiveStatusID();
                                        break;
                                    case GlobalConstants.ORDER_CHANGE:
                                        speciality.IdStatus = DataSourceService.GetActiveStatusID();
                                        break;
                                    case GlobalConstants.ORDER_REMOVE:
                                        speciality.IdStatus = DataSourceService.GetRemoveStatusID();
                                        break;
                                }

                                await this.SpecialityService.UpdateSpecialityOrdersAsync(speciality);
                            }
                        }
                    }
                }
            }
        }

        private void ValidateCode(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            var professionalDirection = this.professionalDirections.FirstOrDefault(x => x.Code == this.professionalDirectionVM.Code && x.EntityId != this.professionalDirectionVM.IdProfessionalDirection && x.IdStatus == DataSourceService.GetActiveStatusID());

            if (professionalDirection is not null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.professionalDirectionVM, "Code");
                this.messageStore?.Add(fi, "Професионално направление с този код вече съществува!");
            }
        }

        private async Task SendNotificationAsync()
        {
            await this.LoadDataForPersonsToSendNotificationToAsync(SPPOOTypes.ProfessionalDirection, this.professionalDirectionVM.IdProfessionalDirection);

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
    }
}
