using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Syncfusion.Blazor.Popups;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Core.Contracts.Common;
using Microsoft.JSInterop;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.Common.Framework;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class AllowIPModal : BlazorBaseComponent
    {
        private SfDialog sfDialog;
        private AllowIPVM allowIPVM;
        private int selectedTab = 0;
        private ValidationMessageStore? validationMessageStore;    

        private List<string> validationMessages = new List<string>();

        [Inject]
        public IAllowIPService AllowIPService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Parameter]
        public EventCallback<bool> CallbackAfterSubmit { get; set; }

        public override bool IsContextModified => this.editContext.IsModified();

        public async Task openAllowIPModal(AllowIPVM allowIP)
        {

            selectedTab = 0;
        
            this.validationMessages.Clear();
            this.allowIPVM = allowIP;          
            this.editContext = new EditContext(this.allowIPVM);
            this.editContext.EnableDataAnnotationsValidation();

            this.isVisible = true;
            this.StateHasChanged();
        }
        public async Task Submit()
        {
            this.validationMessages.Clear();
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.editContext = new EditContext(this.allowIPVM);

                this.editContext.EnableDataAnnotationsValidation();
                this.validationMessageStore = new ValidationMessageStore(this.editContext);
               // this.editContext.OnValidationRequested += this.ValidateCode;

                if (this.editContext.Validate())
                {
                    this.SpinnerShow();
                    var result = new ResultContext<AllowIPVM>();

                    result.ResultContextObject = this.allowIPVM;
            
                    if (this.allowIPVM.idAllowIP == 0)
                    {                        
                        result = await this.AllowIPService.CreateAllowIPAsync(result);
                        this.allowIPVM = result.ResultContextObject;                      
                    }
                    else
                    { 
                            result = await this.AllowIPService.UpdateAllowIPAsync(result);                    
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                }
                else
                {
                    this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
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
