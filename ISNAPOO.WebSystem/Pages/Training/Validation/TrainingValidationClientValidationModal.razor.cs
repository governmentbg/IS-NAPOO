using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationClientValidationModal : BlazorBaseComponent
    {

        [Parameter]
        public ValidationClientVM ClientVM { get; set; }

        private SfAutoComplete<int?, SpecialityVM> sfAutoCompleteSpeciality = new SfAutoComplete<int?, SpecialityVM>();

        private List<FrameworkProgramVM> FrameworkSource;
        private List<SpecialityVM> SpecialitySource;
        private bool enableField = false;

        [Inject]
        IFrameworkProgramService frameworkProgramService { get; set; }
        [Inject]
        ISpecialityService specialityService { get; set; }
        [Inject]
        ITrainingService trainingService { get; set; }
        [Inject]
        IDataSourceService DataSourceService { get; set; }
        [Inject]
        ISpecialityService SpecialityService { get; set; }
        [Parameter]
        public EventCallback<int?> CallbackAfterSubmit { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = true;
        private ValidationMessageStore? messageStore;
        public List<string> validationMessages = new List<string>();

        public bool NoCourses = false;

        private int? IdSpeciality;
        private int? IdFrameworkProgram;

        [Parameter]
        public int PageType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(ClientVM);
            
            var candidateProviderSpecialities = (await this.trainingService.GetCandidateProviderSpecialitiesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider)).ToList();
            var idSpecialities = candidateProviderSpecialities.Select(x => x.IdSpeciality).ToList();
            var specialitiesFromDb = await this.SpecialityService.GetAllSpecialitiesIncludeAsync(new SpecialityVM());
            this.SpecialitySource = specialitiesFromDb.Where(x => idSpecialities.Contains(x.IdSpeciality)).OrderBy(x => x.CodeAsIntForOrderBy).ToList();

            IdSpeciality = this.ClientVM.IdSpeciality;
            IdFrameworkProgram = this.ClientVM.IdFrameworkProgram;
            if (ClientVM.IdFrameworkProgram != null)
            this.FrameworkSource = (await frameworkProgramService.GetAllFrameworkProgramsBySpecialityVQSIdAsync(ClientVM.FrameworkProgram.IdVQS)).ToList();

            if (ClientVM.IdFrameworkProgram != null && ClientVM.IdFrameworkProgram != 0)
                this.enableField = true;

            this.isVisible = true;
            this.StateHasChanged();
        }

        public async Task Submit()
        {
            validationMessages.Clear();
            this.ClientVM.IdSpeciality = this.IdSpeciality;
            this.ClientVM.IdFrameworkProgram = this.IdFrameworkProgram;
            this.editContext = new EditContext(ClientVM);

            this.editContext.OnValidationRequested += this.CheckFields;
            this.messageStore = new ValidationMessageStore(this.editContext);

            try
            {

                var IsValid = this.editContext.Validate();
                this.validationMessages.AddRange(editContext.GetValidationMessages());
                if (IsValid)
                {
                    var currentCourses = (await this.trainingService.GetAllCurrentTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider)).ToList();
                    var archivedCorses = (await this.trainingService.GetAllArchivedTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider)).ToList();
                    var completedCourses = (await this.trainingService.GetAllCompletedTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider)).ToList();

                    var IsInCurrentCourses = currentCourses.Where(x => x.Program.IdSpeciality == this.ClientVM.IdSpeciality).Any();
                    var IsInArchivedCourses = archivedCorses.Where(x => x.Program.IdSpeciality == this.ClientVM.IdSpeciality).Any();
                    var IsCompletedCourses = completedCourses.Where(x => x.Program.IdSpeciality == this.ClientVM.IdSpeciality).Any();

                    if (!IsInCurrentCourses && !IsInArchivedCourses && !IsCompletedCourses)
                    {
                        await this.ShowErrorAsync("Не можете да запишете валидаране на лицето за избраната специалност, защото в системата няма въведена информация за текущ/приключил курс за същата специалност!");
                        NoCourses = true;
                        this.ClientVM.IdSpeciality = null;
                        this.ClientVM.IdFrameworkProgram = null;
                    }
                    else
                    {
                        NoCourses = false;
                        ClientVM.FrameworkProgram = FrameworkSource.Where(x => x.IdFrameworkProgram == ClientVM.IdFrameworkProgram).First();
                        await trainingService.UpdateValidationClientAsync(ClientVM);
                        await CallbackAfterSubmit.InvokeAsync();
                    }
                }

            }catch(Exception e) { }
        }
        private void CheckFields(object? sender, ValidationRequestedEventArgs args)
        {
            if (ClientVM.IdFrameworkProgram == null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.ClientVM, "IdFrameworkProgram");
                this.messageStore?.Add(fi, "Полето 'Рамкова програма' е задължително!");
            }
            if (ClientVM.IdSpeciality == null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.ClientVM, "IdSpeciality");
                this.messageStore?.Add(fi, "Полето 'Специалност' е задължително!");
            }
        }

        //private async Task OnFilterSpeciality(FilteringEventArgs args)
        //{
        //    args.PreventDefaultAction = true;

        //    if (args.Text != null && args.Text.Length > 2)
        //    {
        //        try
        //        {
        //            var query = new Query().Where(new WhereFilter() { Field = "CodeAndName", Operator = "contains", value = args.Text, IgnoreCase = true });

        //            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

        //            await this.sfAutoCompleteSpeciality.FilterAsync(this.SpecialitySource, query);
        //        }
        //        catch (Exception e) { }


        //    }
        //}

        public async void SpecialityChangeHandler(ChangeEventArgs<int?, SpecialityVM> args)
        {
            bool notRelated = false;

            if (args.ItemData != null)
            {
                this.enableField = true;
                this.FrameworkSource = (await frameworkProgramService.GetAllFrameworkProgramsBySpecialityVQSIdAndIdTypeOfFrameworkAsync(args.ItemData.IdVQS,PageType)).ToList().OrderBy(x => x.Name).ToList();
                this.StateHasChanged();
                foreach (var framework in FrameworkSource)
                {
                    if (framework.IdFrameworkProgram == IdFrameworkProgram)
                    {
                        notRelated = false;
                        break;
                    }
                    else
                    {
                        notRelated = true;
                    }
                }

                if (notRelated || args.ItemData == null)
                {
                    IdFrameworkProgram = null;
                }

                OrderFrameworks();
            }
            else
            {
                IdFrameworkProgram = null;
                this.enableField = false;
            }

        }

        private void OrderFrameworks()
        {
            Regex regex = new Regex(@"^(?<name>.*?)\s*(?<number>[0-9]*)$");

            FrameworkSource = this.FrameworkSource
          .Select(item => new {
              value = item,
              match = regex.Match(item.Name),
          })
            .OrderBy(item => item.match.Groups["name"].Value)
            .ThenBy(item => item.match.Groups["number"].Value.Length)
            .ThenBy(item => item.match.Groups["number"].Value)
            .Select(item => item.value).ToList();
        }
    }
}
