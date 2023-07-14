using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Services.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Registers
{
    public partial class RegisterProviderReportFilter : BlazorBaseComponent
    {
        CoursesFilterVM model;

        private SfAutoComplete<int, CandidateProviderVM> sfAutoCompleteCPO = new SfAutoComplete<int, CandidateProviderVM>();

        public List<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();
        
        [Inject]
        ICandidateProviderService candidateProviderService { get; set; }

        [Parameter]
        public EventCallback<CoursesFilterVM> CallbackAfterSubmit { get; set; }

        private ValidationMessageStore? messageStore;
        List<string> validationMessages = new List<string>();

        public async Task openModal()
        {
            messageStore?.Clear();
            validationMessages.Clear();
            this.candidateProviders = (await candidateProviderService.GetAllCandidateProvidersAsync()).ToList();

            this.isVisible = true;

            this.editContext = new EditContext(this.model);

            createModel();

            this.StateHasChanged();
        }

        private void createModel()
        {
            model = new CoursesFilterVM();
        }
        public async Task ClearFilter()
        {
            createModel();
        }

        public async Task OnFilterCPO(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {

                try
                {
                    var query = new Query().Where(new WhereFilter() { Field = "ProviderOwner", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteCPO.FilterAsync(this.candidateProviders, query);
                }
                catch (Exception e) { }

            }
        }

        public void Save()
        {
            messageStore?.Clear();
            validationMessages?.Clear();

            this.editContext.OnValidationRequested += this.CheckRequiredFild;
            this.messageStore = new ValidationMessageStore(this.editContext);

            this.validationMessages.AddRange(editContext.GetValidationMessages());

            var isValid = this.editContext.Validate();

            if(isValid)
            {
                CallbackAfterSubmit.InvokeAsync(model);

                this.isVisible = false;

                this.StateHasChanged();

            }else
            {
                this.validationMessages = this.editContext.GetValidationMessages().ToList();
            }
        }

        private void CheckRequiredFild(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (model.StartFrom == null)
            {
                FieldIdentifier fi = new FieldIdentifier(model, "startCourseFrom");
                this.messageStore?.Add(fi, "Полето 'Начална дата на провеждане от' е задължително!");
            }

            if (model.StartTo == null)
            {
                FieldIdentifier fi = new FieldIdentifier(model, "startCourseTo");
                this.messageStore?.Add(fi, "Полето 'Начална дата на провеждане до' е задължително!");
            }

            if (model.EndFrom == null)
            {
                FieldIdentifier fi = new FieldIdentifier(model, "endCourseFrom");
                this.messageStore?.Add(fi, "Полето 'Крайна дата на провеждане от' е задължително!");
            }

            if (model.EndTo == null)
            {
                FieldIdentifier fi = new FieldIdentifier(model, "endtCourseTo");
                this.messageStore?.Add(fi, "Полето 'Крайна дата на провеждане до' е задължително!");
            }
        }
    }
}
