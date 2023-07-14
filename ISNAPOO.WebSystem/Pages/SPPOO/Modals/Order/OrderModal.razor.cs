using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.SPPOO;
using System.IO;
using System.ComponentModel.DataAnnotations;
using ISNAPOO.WebSystem.Resources;
using global::Data.Models;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.HelperClasses;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.SPPOO.Modals.Order
{
    public partial class OrderModal : BlazorBaseComponent
    {
        [Inject]
        Microsoft.JSInterop.IJSRuntime JsRuntime { get; set; }

        [Inject]
        IOrderService orderService { get; set; }

        [Inject]
        IUploadFileService uploadService { get; set; }

        [Inject]
        IApplicationUserService ApplicationUserService { get; set; }


        OrderVM model = new OrderVM();
        public override bool IsContextModified => this.editContext.IsModified();
        private string CreationDateStr = "";
        private string ModifyDateStr = "";

        [Parameter]
        public EventCallback<OrderVM> CallbackAfterSave { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);
        }

        public async void OpenModal(OrderVM _model)
        {
            this.isVisible = true;
            if (_model.IdOrder != 0)
            {
                this.model = await this.orderService.GetOrderByIdAsync(_model.IdOrder);
                CreationDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                ModifyDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
            }
            else
            {
                this.model = _model;
                ModifyDateStr = "";
                CreationDateStr = "";
                this.model.CreatePersonName = "";
                this.model.ModifyPersonName = "";
            }

            this.editContext = new EditContext(this.model);
            this.StateHasChanged();
        }

        private async Task Save()
        {
            bool hasPermission = await CheckUserActionPermission("ManageOrderData", false);
            if (!hasPermission) { return; }

            this.editContext = new EditContext(this.model);
            this.editContext.EnableDataAnnotationsValidation();

            var result = 0;


            bool isValid = this.editContext.Validate();

            if (isValid)
            {
                this.SpinnerShow();
                //Проверка дали има други активни със същата професия
                var haveActiveOrder = await this.orderService.CheckForDublicateOrder(this.model.OrderDate, this.model.OrderNumber, this.model.IdOrder);

                if (haveActiveOrder)
                {

                    await this.ShowErrorAsync("В базата съществува заповед със същия номер и дата!");

                    this.editContext = new EditContext(this.model);
                    return;
                }

                result = await this.orderService.UpdateOrderAsync(this.model);

                if (result > 0)
                {

                    await this.ShowSuccessAsync("Записът e успешeн!");
                }
                var currentOrder = await orderService.GetOrderByIdAsync(result);
                CreationDateStr = currentOrder.CreationDate.ToString("dd.MM.yyyy");
                ModifyDateStr = currentOrder.ModifyDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(currentOrder.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(currentOrder.IdCreateUser);

                this.editContext = new EditContext(this.model);
                await CallbackAfterSave.InvokeAsync(this.model);
            }
        }


        private async Task OnChange(UploadChangeEventArgs args)
        {
            bool hasPermission = await CheckUserActionPermission("ManageOrderData", false);
            if (!hasPermission) { return; }

            if (this.model.IdOrder > 0)
            {
                if (args.Files.Count == 1)
                {
                    bool isConfirmed = true;

                    var resCheck = await this.uploadService.CheckIfExistUploadedFileAsync<SPPOOOrder>(this.model.IdOrder);
                    if (resCheck)
                    {
                        isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "За избраната заповед вече има прикачен файл. Искате ли да го подмените?");
                    }

                    if (isConfirmed)
                    {
                        var fileName = args.Files[0].FileInfo.Name;

                        var result = await this.uploadService.UploadFileAsync<SPPOOOrder>(args.Files[0].Stream, fileName, "Order", this.model.IdOrder);

                        if (!string.IsNullOrEmpty(result))
                        {
                            this.model.UploadedFileName = result;

                            this.StateHasChanged();
                        }
                    }

                    this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                    this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
                    this.editContext = new EditContext(this.model);
                }
            }
        }

        private async Task OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count == 1)
            {
                if (this.model.IdOrder > 0)
                {
                    bool isConfirmed = true;
                    if (args.FilesData[0].Name == this.model.FileName)
                    {
                        isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");
                    }

                    if (isConfirmed)
                    {
                        var result = await this.uploadService.RemoveFileByIdAsync<SPPOOOrder>(this.model.IdOrder);

                        if (result == 1)
                        {
                            this.model = await this.orderService.GetOrderByIdAsync(this.model.IdOrder);

                            this.StateHasChanged();
                        }
                    }
                }
            }
        }

        private async Task OnDownloadClick()
        {
            bool hasPermission = await CheckUserActionPermission("ViewOrderData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.uploadService.CheckIfExistUploadedFileAsync<SPPOOOrder>(this.model.IdOrder);
                if (hasFile)
                {
                    var documentStream = await this.uploadService.GetUploadedFileAsync<SPPOOOrder>(this.model.IdOrder);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, this.model.FileName, documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    await this.JsRuntime.InvokeVoidAsync("alert", "Файлът, който се опитвате да свалите, не съществува!");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnRemoveClick()
        {
            bool hasPermission = await CheckUserActionPermission("ManageOrderData", false);
            if (!hasPermission) { return; }

            if (this.model.IdOrder > 0)
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");
                if (isConfirmed)
                {
                    var result = await this.uploadService.RemoveFileByIdAsync<SPPOOOrder>(this.model.IdOrder);

                    if (result == 1)
                    {
                        this.model = await this.orderService.GetOrderByIdAsync(this.model.IdOrder);

                        this.StateHasChanged();
                    }
                }

                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
                this.editContext = new EditContext(this.model);
            }
        }
    }
}
