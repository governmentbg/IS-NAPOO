using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class EventLogFilterModal : BlazorBaseComponent
    {
        private EventLogListFilterVM eventLogListFilterVM = new EventLogListFilterVM();
        private List<EventLogVM> eventLogsSource = new List<EventLogVM>();
        ValidationMessageStore? messageStore;
        [Inject]
        public IEventLogService eventLogService { get; set; }

        [Parameter]
        public EventCallback<List<EventLogVM>> CallbackAfterSubmit { get; set; }

        public void OpenModal()
        {
            this.editContext = new EditContext(this.eventLogListFilterVM);
            if (!this.eventLogListFilterVM.EventLogsFrom.HasValue)
            {
                this.eventLogListFilterVM.EventLogsFrom = DateTime.Now.AddHours(-1);
            }

            if (!this.eventLogListFilterVM.EventLogsTo.HasValue)
            {
                
                this.eventLogListFilterVM.EventLogsTo = DateTime.Now;
            }

           



            this.isVisible = true;
            this.StateHasChanged();
        }
        private async Task SearchBtn()
        {
            this.editContext = new EditContext(this.eventLogListFilterVM);
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidateValue;
            this.editContext.EnableDataAnnotationsValidation();

            if (this.editContext.Validate())
            {
                this.eventLogsSource = await this.eventLogService.GetEventLogsFromToDatePersonNameIPAsync(this.eventLogListFilterVM);

                this.isVisible = false;

                await this.CallbackAfterSubmit.InvokeAsync(this.eventLogsSource);
            }
        }
        private void ValidateValue(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (!string.IsNullOrEmpty(this.eventLogListFilterVM.PersonName) && this.eventLogListFilterVM.PersonName.Any(char.IsDigit))
            {
                FieldIdentifier fi = new FieldIdentifier(this.eventLogListFilterVM, "PersonName");
                this.messageStore?.Add(fi, "Полето 'Потребител' трябва да съдържа само букви!");
            }

            if(!this.eventLogListFilterVM.EventLogsFrom.HasValue)
            {
                FieldIdentifier fi = new FieldIdentifier(this.eventLogListFilterVM, "EventLogsFrom");
                this.messageStore?.Add(fi, "Полето 'Извършени действия от' е задължително!");
            }

            if (!this.eventLogListFilterVM.EventLogsTo.HasValue)
            {
                FieldIdentifier fi = new FieldIdentifier(this.eventLogListFilterVM, "EventLogsTo");
                this.messageStore?.Add(fi, "Полето 'Извършени действия до' е задължително!");
            }


            if (this.eventLogListFilterVM.EventLogsFrom.HasValue &&
                this.eventLogListFilterVM.EventLogsTo.HasValue &&
                this.eventLogListFilterVM.EventLogsFrom.Value  > this.eventLogListFilterVM.EventLogsTo.Value)
            {
                FieldIdentifier fi = new FieldIdentifier(this.eventLogListFilterVM, "EventLogsFrom");
                this.messageStore?.Add(fi, "Полето 'Извършени действия от' е по-малко от 'Извършени действия до'");
            }
        }
        private void ClearBtn()
        {
            
            this.eventLogListFilterVM = new EventLogListFilterVM();
            this.eventLogListFilterVM.EventLogsFrom = DateTime.Now.AddHours(-1);
            this.eventLogListFilterVM.EventLogsTo = DateTime.Now;
        }
    }
}
