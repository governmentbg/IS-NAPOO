using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.SPPOO.Modals.Area
{
    public partial class AreaModal : BlazorBaseComponent
    {
        DialogEffect AnimationEffect = DialogEffect.Zoom;
        bool showConfirmDialog = false;
        bool closeConfirmed = false;
        AreaVM areaVM = new AreaVM();
        IEnumerable<SPPOOTreeGridData> areas;
        IEnumerable<KeyValueVM> statusSPOOOValues;
        EditContext editContext;
        bool isSubmitClicked = false;
        string CreationDateStr = "";
        string ModifyDateStr = "";
        ValidationMessageStore? messageStore;

        public override bool IsContextModified
        {
            get => this.editContext.IsModified();
        }

        [Parameter]
        public EventCallback<string> OnSubmit { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.areaVM);
            this.editContext.OnValidationRequested += ValidateCode;
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.isVisible = false;
        }

        public async Task OpenModal(AreaVM areaVM, IEnumerable<SPPOOTreeGridData> areas)
        {
            this.areas = areas;
            this.editContext = new EditContext(this.areaVM);
            this.statusSPOOOValues = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.StatusSPPOO);
            this.areaVM = areaVM;
            if (this.areaVM.IdArea == 0)
            {
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                this.areaVM.ModifyPersonName = "";
                this.areaVM.CreatePersonName = "";

            }
            else
            {
                this.CreationDateStr = this.areaVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.areaVM.ModifyDate.ToString("dd.MM.yyyy");
                this.areaVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.areaVM.IdModifyUser);
                this.areaVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.areaVM.IdCreateUser);
            }

            this.isVisible = true;
        }

        private async Task SubmitHandler()
        {
            this.editContext = new EditContext(this.areaVM);
            this.editContext.OnValidationRequested += ValidateCode;
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.EnableDataAnnotationsValidation();

            string msg = string.Empty;
            bool isValid = this.editContext.Validate();

            if (isValid)
            {
                this.SpinnerShow();
                this.isSubmitClicked = true;
                msg = await this.AreaService.UpdateAreaAsync(this.areaVM);
                this.areaVM = await this.AreaService.GetAreaByIdAsync(this.areaVM.IdArea);
                await this.OnSubmit.InvokeAsync(msg);
                this.CreationDateStr = this.areaVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.areaVM.ModifyDate.ToString("dd.MM.yyyy");
                this.areaVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.areaVM.IdModifyUser);
                this.areaVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.areaVM.IdCreateUser);

                this.DataSourceService.ReloadAreas();

                this.SpinnerHide();
            }

            this.isSubmitClicked = false;
        }

        private void ValidateCode(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            var area = this.areas.FirstOrDefault(x => x.Code == this.areaVM.Code && x.EntityId != this.areaVM.IdArea && x.IdStatus == DataSourceService.GetActiveStatusID());

            if (area is not null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.areaVM, "Code");
                this.messageStore?.Add(fi, "Област на образование с този код вече съществува!");
            }
        }

        private async Task SendNotificationAsync()
        {
            await this.LoadDataForPersonsToSendNotificationToAsync(SPPOOTypes.Area, this.areaVM.IdArea);

            if (!this.personIds.Any())
            {
                await this.ShowErrorAsync("Няма активни лицензирани ЦПО по избраните специалности!");
            }
            else
            {
                await this.OpenSendNotificationModal(true, this.personIds);
            }
        }
    }
}
