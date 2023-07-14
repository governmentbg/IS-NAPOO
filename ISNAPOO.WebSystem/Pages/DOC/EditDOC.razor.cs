using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.WebSystem.Pages.DOC.ERU;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.WebSystem.Pages.DOC.ERU;
using Microsoft.AspNetCore.Components;

using Syncfusion.Blazor.Popups;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Common.Constants;
using Syncfusion.Blazor.Navigations;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ISNAPOO.WebSystem.Pages.DOC
{
    public partial class EditDOC : BlazorBaseComponent
    {

        [Inject] public IDOCService docService { get; set; }
        [Inject] public IProfessionService professionService { get; set; }
        [Inject] public Microsoft.JSInterop.IJSRuntime JsRuntime { get; set; }
        [Inject] public IApplicationUserService ApplicationUserService { get; set; }

        List<string> validationMessages = new List<string>();
        public string CreationDateStr { get; set; } = "";
        public string ModifyDateStr { get; set; } = "";

        public override bool IsContextModified
        {
            get => this.docData.IsEditContextModified();
        }

        private DialogEffect AnimationEffect = DialogEffect.Zoom;

        SfDialog sfDialog;
        private string dialogClass = "";
        DOCDataModal docData;
        DocVM model = new DocVM();
        ERUList eruList = new ERUList();

        private string ProfessionHeaderName = string.Empty;

        int selectedTab = 0;


        [Parameter]
        public EventCallback<DocVM> CallbackAfterSave { get; set; }

        public async Task OpenModal(DocVM _model)
        {
            validationMessages.Clear();
            bool hasPermission = await CheckUserActionPermission("ViewDOCData", false);
            if (!hasPermission) { return; }

            var selectedProffession = await this.professionService.GetOnlyProfessionByIdAsync(_model.IdProfession);
            this.ProfessionHeaderName = selectedProffession.ComboBoxName;

            this.model = _model;

            this.model.EditButtonClick = 1;
            if (this.model.IdDOC == 0)
            {
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                this.model.ModifyPersonName = "";
                this.model.CreatePersonName = "";
               
            }
            else
            {
                this.CreationDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
              
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void OnClose(BeforeCloseEventArgs args)
        {
            if (args.ClosedBy == "Close Icon")
            {
                args.Cancel = true;
                this.eruList.Clear();
                this.CancelClickedHandler();
            }
        }
        private void ModalClose()
        {
                this.eruList.Clear();
                this.CancelClickedHandler();
        }
        private void Select(SelectingEventArgs args)
        {
            if (args.IsSwiped)
            {
                args.Cancel = true;
            }
        }

        private async Task<Task> UpdateAfterSave(DocVM _model)
        {
            if (this.loading)
            {
                return Task.CompletedTask;
            }
            try
            {
                this.loading = true;
                this.model = _model;
                this.CreationDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
                var selectedProffession = await this.professionService.GetOnlyProfessionByIdAsync(this.model.IdProfession);
                this.ProfessionHeaderName = selectedProffession.ComboBoxName;

                await CallbackAfterSave.InvokeAsync(this.model);

                await this.JsRuntime.InvokeVoidAsync("scrollToErrors");

                return Task.CompletedTask;
            }
            finally
            {
                this.loading = false;
            }
        }

        protected async Task ValidationMsg(IEnumerable<string> messages)
        {
            validationMessages.Clear();
            validationMessages.AddRange(messages);
        }

        private async Task SendNotificationAsync()
        {
            await this.LoadDataForPersonsToSendNotificationToAsync("DOC", this.model.IdDOC);

            if (!this.personIds.Any())
            {
                await this.ShowErrorAsync("Няма лицензирани ЦПО за избраната(ите) специалност(и)!");
            }
            else
            {
                await this.OpenSendNotificationModal(true, this.personIds);
            }
        }

        //Може да се използва за запис на повече от 1 таб
        //private async void SaveModal()
        //{
        //    await this.docData.Save();

        //}

    }
}
