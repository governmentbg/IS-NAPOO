using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.Consulting
{
    public partial class ConsultingClientInformation : BlazorBaseComponent
    {
        private SfGrid<ConsultingVM> consultingsGrid = new SfGrid<ConsultingVM>();

        private IEnumerable<KeyValueVM> kvIndentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvSexSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvAssingSource = new List<KeyValueVM>();
        private List<KeyValueVM> kvConsultingTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvConsultingReceiveTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvRegistrationAtLabourOfficeType = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvAimAtCIPOServicesType = new List<KeyValueVM>();
        private List<KeyValueVM> kvAllConsultingTypeSource = new List<KeyValueVM>();
        private IEnumerable<ConsultingVM> consultingsSource = new List<ConsultingVM>();
        private List<KeyValueVM> kvNationalitySource = new List<KeyValueVM>();
        public string identType = "ЕГН";
        private KeyValueVM kvEGN = new KeyValueVM();
        private KeyValueVM kvLNCh = new KeyValueVM();
        private KeyValueVM kvBGNationality = new KeyValueVM();
        private ValidationMessageStore? messageStore;
        private string content = "Проверка и зареждане на данни за курсиста от предишни курсове на обучение";
        private bool IsDateValid = true;
        private ConsultingVM consultingToDelete = new ConsultingVM();
        private ConsultingVM consultingVM = new ConsultingVM();
        private bool isStudentTrue = false;
        private bool isStudentFalse = false;
        private bool isEmployedPersonTrue = false;
        private bool isEmployedPersonFalse = false;
        private bool isConslutingClientArchived = false;

        [Parameter]
        public ConsultingClientVM ConsultingClientVM { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.ConsultingClientVM);
            this.FormTitle = "Данни за консултирано лице";

            this.kvConsultingReceiveTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingReceiveType");
            this.kvRegistrationAtLabourOfficeType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RegistrationAtLabourOfficeType");
            this.kvAimAtCIPOServicesType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AimAtCIPOServicesType");
            this.kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.kvSexSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            this.kvAssingSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType")).Where(x => x.DefaultValue1 != null && x.DefaultValue1 == "CIPO").ToList();
            this.kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvLNCh = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
            
            await this.LoadConsultingsDataAsync();
            await this.LoadConsultingTypesAsync();
            this.CheckForAlreadyAddedConsultingTypes();

            await this.HandleOrderForNationalitiesSourceAsync();

            if (this.ConsultingClientVM.IdConsultingClient == 0)
            {
                this.ConsultingClientVM.IdIndentType = this.kvEGN.IdKeyValue;
            }

            if (this.ConsultingClientVM.IdIndentType.HasValue)
            {
                var ident = this.kvIndentTypeSource.FirstOrDefault(x => x.IdKeyValue == this.ConsultingClientVM.IdIndentType.Value);
                if (ident is not null)
                {
                    this.identType = ident.Name;
                }
            }         

            this.SetIsStudentValue();
            this.SetIsEmployedPersonValue();
        }

        private void SetIsStudentValue()
        {
            if (this.ConsultingClientVM.IdConsultingClient == 0)
            {
                this.ConsultingClientVM.IsStudent = null;
            }

            if (this.ConsultingClientVM.IsStudent is not null)
            {
                if (this.ConsultingClientVM.IsStudent.Value)
                {
                    this.isStudentTrue = true;
                }
                else
                {
                    this.isStudentFalse = true;
                }
            }
        }

        private void SetIsEmployedPersonValue()
        {
            if (this.ConsultingClientVM.IdConsultingClient == 0)
            {
                this.ConsultingClientVM.IsEmployedPerson = null;
            }

            if (this.ConsultingClientVM.IsEmployedPerson is not null)
            {
                if (this.ConsultingClientVM.IsEmployedPerson.Value)
                {
                    this.isEmployedPersonTrue = true;
                }
                else
                {
                    this.isEmployedPersonFalse = true;
                }
            }
        }

        private void OnStudentTrueValueChange(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            if (this.isStudentFalse)
            {
                this.isStudentFalse = false;
            }
        }

        private void OnStudentFalseValueChange(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            if (this.isStudentTrue)
            {
                this.isStudentTrue = false;
            }
        }

        private void OnIsEmployedPersonTrueValueChange(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            if (this.isEmployedPersonFalse)
            {
                this.isEmployedPersonFalse = false;
                this.ConsultingClientVM.IdRegistrationAtLabourOfficeType = null;
            }
        }

        private void OnIsEmployedPersonFalseValueChange(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            if (this.isEmployedPersonTrue)
            {
                this.isEmployedPersonTrue = false;
            }
        }

        private async Task HandleOrderForNationalitiesSourceAsync()
        {
            this.kvNationalitySource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality")).OrderBy(x => x.Name).ToList();
            var withoutNacionality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "Без гражданство");
            this.kvNationalitySource.Remove(withoutNacionality);
            this.kvNationalitySource.Add(withoutNacionality);
            var bgNationality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "България");
            this.kvNationalitySource.Remove(bgNationality);
            this.kvNationalitySource.Insert(0, bgNationality);
            this.kvNationalitySource.RemoveAll(x => x.Name == "");

            this.kvBGNationality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "България");
        }

        private async Task LoadConsultingsDataAsync()
        {
            this.consultingsSource = await this.CandidateProviderService.GetAllConsultingsByIdConsultingClientAsync(this.ConsultingClientVM.IdConsultingClient);
        }

        private void IdentValueChangedHandler(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.Value.HasValue)
            {
                if (args.Value == this.kvEGN.IdKeyValue)
                {
                    this.identType = "ЕГН";
                }
                else if (args.Value == this.kvLNCh.IdKeyValue)
                {
                    this.identType = "ЛНЧ";
                }
                else
                {
                    this.identType = "ИДН";
                }
            }
            else
            {
                this.identType = "ЕГН";
            }
        }

        private void IndentChanged(ChangeEventArgs args)
        {
            var indent = this.ConsultingClientVM.Indent;
            if (indent != null)
            {
                if (this.ConsultingClientVM.IdIndentType == this.kvEGN.IdKeyValue)
                {
                    indent = indent.Trim();

                    var checkEGN = new BasicEGNValidation(indent);

                    if (checkEGN.Validate())
                    {
                        char charLastDigit = indent[indent.Length - 2];
                        int lastDigit = Convert.ToInt32(new string(charLastDigit, 1));
                        int year = int.Parse(indent.Substring(0, 2));
                        int month = int.Parse(indent.Substring(2, 2));
                        int day = int.Parse(indent.Substring(4, 2));
                        if (month < 13)
                        {
                            year += 1900;
                        }
                        else if (month > 20 && month < 33)
                        {
                            year += 1800;
                            month -= 20;
                        }
                        else if (month > 40 && month < 53)
                        {
                            year += 2000;
                            month -= 40;
                        }
                        var BirthDate = new DateTime(year, month, day);

                        this.ConsultingClientVM.BirthDate = BirthDate;

                        var beforeLastNumber = int.Parse(indent.Substring(indent.Length - 2, 1));

                        if (beforeLastNumber % 2 == 0)
                        {
                            this.ConsultingClientVM.IdSex = this.kvSexSource.FirstOrDefault(x => x.Name == "Мъж").IdKeyValue;
                        }
                        else
                        {
                            this.ConsultingClientVM.IdSex = this.kvSexSource.FirstOrDefault(x => x.Name == "Жена").IdKeyValue;
                        }
                    }
                    else
                    {
                        this.ConsultingClientVM.BirthDate = null;
                        this.ConsultingClientVM.IdSex = null;
                    }
                }
                else
                {
                    this.ConsultingClientVM.BirthDate = null;
                    this.ConsultingClientVM.IdSex = null;
                }
            }
        }

        private async Task CheckForExistingClientAsync()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var result = await this.TrainingService.GetClientByIdIndentTypeByIndentAndByIdCandidateProviderAsync(this.ConsultingClientVM.IdIndentType.Value, this.ConsultingClientVM.Indent, this.UserProps.IdCandidateProvider);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    var modelFromDb = result.ResultContextObject;
                    this.ConsultingClientVM.FirstName = modelFromDb.FirstName;
                    this.ConsultingClientVM.SecondName = modelFromDb.SecondName;
                    this.ConsultingClientVM.FamilyName = modelFromDb.FamilyName;
                    this.ConsultingClientVM.IdSex = modelFromDb.IdSex;
                    this.ConsultingClientVM.IdIndentType = modelFromDb.IdIndentType;
                    this.ConsultingClientVM.Indent = modelFromDb.Indent;
                    this.ConsultingClientVM.BirthDate = modelFromDb.BirthDate;
                    this.ConsultingClientVM.IdNationality = modelFromDb.IdNationality;
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public override async void SubmitHandler()
        {
            if (this.isStudentTrue)
            {
                this.ConsultingClientVM.IsStudent = true;
            }
            else if (this.isStudentFalse)
            {
                this.ConsultingClientVM.IsStudent = false;
            }
            else
            {
                this.ConsultingClientVM.IsStudent = null;
            }

            if (this.isEmployedPersonTrue)
            {
                this.ConsultingClientVM.IsEmployedPerson = true;
                this.ConsultingClientVM.IdRegistrationAtLabourOfficeType = null;
            }
            else if (this.isEmployedPersonFalse)
            {
                this.ConsultingClientVM.IsEmployedPerson = false;
            }
            else
            {
                this.ConsultingClientVM.IsEmployedPerson = null;
            }

            this.editContext = new EditContext(this.ConsultingClientVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.editContext.OnValidationRequested += this.ValidateEGN;
            this.editContext.OnValidationRequested += this.ValidateSecondName;
            this.editContext.OnValidationRequested += this.ValidateStartAndEndDate;
            this.editContext.OnValidationRequested += this.ValidateLabourOfficeIfNotEmployedName;
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.Validate();
        }

        private void DateValid()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime(); ;
            if (this.ConsultingClientVM.StartDate.HasValue)
            {
                startDate = this.ConsultingClientVM.StartDate.Value.Date;
            }
            if (this.ConsultingClientVM.EndDate.HasValue)
            {
                endDate = this.ConsultingClientVM.EndDate.Value.Date;

            }
            int result = DateTime.Compare(startDate, endDate);

            if (result > 0 && this.ConsultingClientVM.EndDate.HasValue && this.ConsultingClientVM.StartDate.HasValue)
            {
                this.IsDateValid = false;
            }
            else
            {
                this.IsDateValid = true;
            }
        }

        private void ValidateEGN(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.ConsultingClientVM.Indent != null)
            {
                this.ConsultingClientVM.Indent = this.ConsultingClientVM.Indent.Trim();

                if (this.ConsultingClientVM.Indent.Length != 10)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ConsultingClientVM, "Indent");
                    this.messageStore?.Add(fi, $"Полето {this.identType} трябва да съдържа 10 символа!");
                }
                else
                {
                    if (this.ConsultingClientVM.IdIndentType == this.kvEGN.IdKeyValue)
                    {
                        var checkEGN = new BasicEGNValidation(this.ConsultingClientVM.Indent);

                        if (!checkEGN.Validate())
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.ConsultingClientVM, "Indent");
                            this.messageStore?.Add(fi, checkEGN.ErrorMessage);
                        }
                    }
                }
            }
            else
            {
                FieldIdentifier fi = new FieldIdentifier(this.ConsultingClientVM, "Indent");
                this.messageStore?.Add(fi, $"Полето '{this.identType}' е задължително!");
            }
        }

        private void ValidateSecondName(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.ConsultingClientVM.IdIndentType == this.kvEGN.IdKeyValue)
            {
                if (string.IsNullOrEmpty(this.ConsultingClientVM.SecondName))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ConsultingClientVM, "SecondName");
                    this.messageStore?.Add(fi, $"Полето 'Презиме' е задължително!");
                }
            }
        }

        private void ValidateStartAndEndDate(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.ConsultingClientVM.StartDate.HasValue && this.ConsultingClientVM.EndDate.HasValue)
            {
                this.DateValid();
                if (!IsDateValid)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ConsultingClientVM, "StartDate");
                    this.messageStore?.Add(fi, $"Въведената дата в полето 'Дата на стартиране' не може да е след 'Дата на приключване'!");
                }
            }
        }

        private void ValidateLabourOfficeIfNotEmployedName(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.isEmployedPersonFalse)
            {
                if (this.ConsultingClientVM.IdRegistrationAtLabourOfficeType is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ConsultingClientVM, "IdRegistrationAtLabourOfficeType");
                    this.messageStore?.Add(fi, $"Полето 'Регистрация в бюрото по труда' е задължително!");
                }
            }
        }

        private async Task AddConsultingTypeBtn()
        {
            if (this.consultingVM.IdConsultingType == 0)
            {
                await this.ShowErrorAsync("Моля, изберете вид на услугата!");
                return;
            }

            if (this.consultingVM.IdConsultingReceiveType is null)
            {
                await this.ShowErrorAsync("Моля, изберете начин на предоставяне!");
                return;
            }

            if (this.consultingVM.Cost is null)
            {
                await this.ShowErrorAsync("Моля, въведете цена (в лева)!");
                return;
            }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.consultingVM.IdConsultingClient = this.ConsultingClientVM.IdConsultingClient;

                var result = await this.CandidateProviderService.CreateConsultingAsync(this.consultingVM);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    this.consultingVM = new ConsultingVM();

                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                    await this.LoadConsultingsDataAsync();
                    this.CheckForAlreadyAddedConsultingTypes();

                    this.StateHasChanged();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void CheckForAlreadyAddedConsultingTypes(bool IsDelete = false)
        {
            if (this.consultingsSource.Any())
            {
                this.kvConsultingTypeSource = this.kvAllConsultingTypeSource;

                foreach (var consulting in this.consultingsSource)
                {
                    this.kvConsultingTypeSource = this.kvConsultingTypeSource.Where(x => x.IdKeyValue != consulting.IdConsultingType).ToList();
                }
            }
            else
            {
                this.kvConsultingTypeSource = this.kvAllConsultingTypeSource;
            }

            this.StateHasChanged();
        }

        private async Task LoadConsultingTypesAsync()
        {
            var candidateProviderConsulting = await this.CandidateProviderService.GetAllCandidateProviderConsultingsByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider);
            var kvConsultingTypes = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType")).ToList();
            if (candidateProviderConsulting.Any())
            {
                foreach (var consulting in candidateProviderConsulting)
                {
                    if (!this.kvAllConsultingTypeSource.Any(c => c.IdKeyValue == consulting.IdConsultingType))
                    {
                        this.kvAllConsultingTypeSource.Add(kvConsultingTypes.FirstOrDefault(x => x.IdKeyValue == consulting.IdConsultingType));
                    }
                }
            }
        }

        private async Task DeleteConsultingTypeBtn(ConsultingVM model)
        {
            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;
                    this.consultingToDelete = model;

                        var result = await this.CandidateProviderService.DeleteConsultingByIdAsync(this.consultingToDelete.IdConsulting);
                        if (result.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                        }
                        else
                        {
                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                            await this.LoadConsultingsDataAsync();
                            this.CheckForAlreadyAddedConsultingTypes(true);

                            this.StateHasChanged();
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
}
