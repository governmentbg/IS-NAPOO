using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class TrainingProgram : BlazorBaseComponent
    {
        private SfComboBox<int, SpecialityVM> specialitiesCB = new SfComboBox<int, SpecialityVM>();

        private List<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private IEnumerable<KeyValueVM> typeFrameworkProgramSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> legalCapacityOrdinanceTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> legalCapacityOrdinanceTypeSourceForSPKAndPP = new List<KeyValueVM>();
        private List<FrameworkProgramVM> frameworkProgramSource = new List<FrameworkProgramVM>();
        private bool isSpecialitySelected = false;
        private bool isTypeFrameworkProgramSelected = false;
        private bool isSpecialitySelectEnabled = true;
        private int idSpeciality = 0;
        private int idFrameworkProgram = 0;
        private string courseType = string.Empty;
        private ValidationMessageStore? messageStore;
        private string legalCapacityDescription = string.Empty;
        private KeyValueVM kvFireSafetyTraining = new KeyValueVM();
        private List<ProfessionVM> professionSource = new List<ProfessionVM>();
        private int idLegalCapacityOrdinanceType = 0;

        [Parameter]
        public ProgramVM ProgramVM { get; set; }

        [Parameter]
        public bool ShowLegalCapacityOrdinanceType { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IFrameworkProgramService FrameworkProgramService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.ProgramVM);
            this.FormTitle = "Програма за обучение";

            if (!this.ShowLegalCapacityOrdinanceType)
            {
                await this.LoadDataAsync();
            }
            else
            {
                await this.LoadDataAsync();
                this.isSpecialitySelectEnabled = false;
            }

            this.ProgramVM.IsProgramLegalCapacityOrdinance = this.ProgramVM.IdLegalCapacityOrdinanceType.HasValue;

            this.professionSource = this.DataSourceService.GetAllProfessionsList();

            if (!this.ShowLegalCapacityOrdinanceType)
            {
                await this.LoadLegalCapacityKVFromSPKAndPPAsync(this.ProgramVM.Speciality);
            }
            else
            {
                await this.LoadLegalCapacityOrdinanceTypeAsync();
            }

            if (this.ProgramVM.OldId.HasValue)
            {
                this.frameworkProgramSource = this.FrameworkProgramService.getAllFrameworkProgramsWithoutAsync();
            }

            this.SetLegalCapacityDescription();

            this.editContext.MarkAsUnmodified();
        }

        private void SetLegalCapacityDescription()
        {
            if (this.ProgramVM.IdLegalCapacityOrdinanceType.HasValue)
            {
                if (!this.ShowLegalCapacityOrdinanceType)
                {
                    var legalCapacityType = this.legalCapacityOrdinanceTypeSourceForSPKAndPP.FirstOrDefault(x => x.IdKeyValue == this.ProgramVM.IdLegalCapacityOrdinanceType.Value);
                    this.legalCapacityDescription = legalCapacityType!.Description;
                }
                else
                {
                    var legalCapacityType = this.legalCapacityOrdinanceTypeSource.FirstOrDefault(x => x.IdKeyValue == this.ProgramVM.IdLegalCapacityOrdinanceType.Value);
                    this.legalCapacityDescription = legalCapacityType!.Description;
                }
            }
        }

        private async Task OnLegalCapacityValueChanged(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (this.ShowLegalCapacityOrdinanceType && args.ItemData is not null)
            {
                this.legalCapacityDescription = args.ItemData.Description;
                this.isSpecialitySelectEnabled = true;
                await this.LoadDataAsync();
            }
            else
            {
                this.specialitiesSource.Clear();
                this.typeFrameworkProgramSource = new List<KeyValueVM>();
                this.legalCapacityDescription = string.Empty;
                this.isTypeFrameworkProgramSelected = false;
                this.isSpecialitySelected = false;
                this.isSpecialitySelectEnabled = false;

                this.StateHasChanged();
            }
        }

        private void OnLegalCapacityFromSPKAndPPValueChanged(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.ItemData is not null)
            {
                this.legalCapacityDescription = args.ItemData.Description;
            }
            else
            {
                this.legalCapacityDescription = string.Empty;
            }
        }

        private async Task LoadLegalCapacityOrdinanceTypeAsync()
        {
            this.legalCapacityOrdinanceTypeSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LegalCapacityOrdinanceType")).Where(x => !string.IsNullOrEmpty(x.DefaultValue1) && x.DefaultValue1 == "LegalCapacity");
            this.kvFireSafetyTraining = this.legalCapacityOrdinanceTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Type13");
        }

        private async Task LoadDataAsync()
        {
            await this.LoadSpecialitiesDataAsync();

            var kvLegalCapacityCourseType = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7");
            if (this.ShowLegalCapacityOrdinanceType)
            {
                this.typeFrameworkProgramSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).Where(x => x.IdKeyValue == kvLegalCapacityCourseType.IdKeyValue);
            }
            else
            {
                this.typeFrameworkProgramSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).Where(x => x.IdKeyValue == this.ProgramVM.IdCourseType);
            }

            this.courseType = this.typeFrameworkProgramSource.FirstOrDefault().Name;

            if (this.ProgramVM.Speciality != null)
            {
                this.idSpeciality = this.ProgramVM.Speciality.IdSpeciality;
                this.isSpecialitySelected = true;
            }

            if (this.ProgramVM.FrameworkProgram != null)
            {
                this.isTypeFrameworkProgramSelected = true;
                this.idFrameworkProgram = this.ProgramVM.FrameworkProgram.IdFrameworkProgram;
            }

            if (this.isTypeFrameworkProgramSelected && this.isSpecialitySelected)
            {
                if (this.ShowLegalCapacityOrdinanceType)
                {
                    this.frameworkProgramSource = (await this.TrainingService.GetFrameworkProgramByIdVQSAndByIdTypeFrameworkProgramAsync(this.ProgramVM.Speciality.IdVQS, kvLegalCapacityCourseType.IdKeyValue)).ToList();
                }
                else
                {
                    this.frameworkProgramSource = (await this.TrainingService.GetFrameworkProgramByIdVQSAndByIdTypeFrameworkProgramAsync(this.ProgramVM.Speciality.IdVQS, this.ProgramVM.IdCourseType)).ToList();
                }
            }

            await this.TypeFrameworkProgramSelectedHandler();
        }

        private async Task LoadSpecialitiesDataAsync()
        {
            var candidateProviderSpecialities = (await this.TrainingService.GetCandidateProviderSpecialitiesByIdCandidateProviderAsync(this.ProgramVM.IdCandidateProvider)).ToList();
            var idSpecialities = candidateProviderSpecialities.Select(x => x.IdSpeciality).ToList();
            var specialitiesFromDb = this.DataSourceService.GetAllSpecialitiesList().Where(x => x.IdStatus == this.DataSourceService.GetActiveStatusID());
            this.specialitiesSource = specialitiesFromDb.Where(x => idSpecialities.Contains(x.IdSpeciality)).OrderBy(x => x.CodeAsIntForOrderBy).ToList();
            if (this.ProgramVM.IdSpeciality != 0 && !this.specialitiesSource.Any(x => x.IdSpeciality == this.ProgramVM.IdSpeciality))
            {
                var speciality = this.DataSourceService.GetAllSpecialitiesList().FirstOrDefault(x => x.IdSpeciality == this.ProgramVM.IdSpeciality);
                if (speciality is not null)
                {
                    this.specialitiesSource.Add(speciality);

                    this.specialitiesSource = this.specialitiesSource.OrderBy(x => x.CodeAsIntForOrderBy).ToList();
                }
            }

            this.HandleSpecialitySourceWhenLegalCapacity();
        }

        private void HandleSpecialitySourceWhenLegalCapacity()
        {
            if (this.ShowLegalCapacityOrdinanceType)
            {
                var professionSource = this.DataSourceService.GetAllProfessionsList();
                for (int i = this.specialitiesSource.Count - 1; i > -1; i--)
                {
                    var speciality = this.specialitiesSource[i];
                    var profession = professionSource.FirstOrDefault(x => x.IdProfession == speciality.IdProfession);
                    if (this.ProgramVM.IdProgram == 0 && profession is not null && profession.IdLegalCapacityOrdinanceType != this.ProgramVM.IdLegalCapacityOrdinanceType)
                    {
                        this.specialitiesSource.RemoveAll(x => x.IdSpeciality == speciality.IdSpeciality);
                    }
                }

                this.StateHasChanged();
            }
        }

        private async Task SpecialitySelectedHandler(ChangeEventArgs<int, SpecialityVM> args)
        {
            if (this.ProgramVM.OldId.HasValue)
            {
                this.frameworkProgramSource = this.FrameworkProgramService.getAllFrameworkProgramsWithoutAsync();
            }
            else
            {
                if (args.ItemData is not null)
                {
                    this.isSpecialitySelected = true;

                    await this.LoadLegalCapacityKVFromSPKAndPPAsync(args.ItemData);

                    if (this.ProgramVM.IdCourseType != 0)
                    {
                        var specialityVQS = this.specialitiesSource.FirstOrDefault(x => x.IdSpeciality == this.ProgramVM.IdSpeciality);
                        if (specialityVQS is not null)
                        {
                            int idVQS = specialityVQS.IdVQS;
                            if (!this.ShowLegalCapacityOrdinanceType)
                            {
                                this.frameworkProgramSource = (await this.TrainingService.GetFrameworkProgramByIdVQSAndByIdTypeFrameworkProgramAsync(idVQS, this.ProgramVM.IdCourseType)).ToList();
                            }
                            else
                            {
                                var spkTypeFrameworkProgram = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");
                                this.frameworkProgramSource = (await this.TrainingService.GetFrameworkProgramByIdVQSAndByIdTypeFrameworkProgramAsync(idVQS, spkTypeFrameworkProgram.IdKeyValue)).ToList();
                            }
                        }
                    }
                    else
                    {
                        this.frameworkProgramSource.Clear();
                    }
                }
                else
                {
                    this.ProgramVM.Speciality = null;
                    this.idFrameworkProgram = 0;
                    this.idSpeciality = 0;
                    this.isSpecialitySelected = false;
                    this.legalCapacityOrdinanceTypeSourceForSPKAndPP = new List<KeyValueVM>();
                    this.frameworkProgramSource.Clear();
                }
            }
        }

        private async Task LoadLegalCapacityKVFromSPKAndPPAsync(SpecialityVM speciality)
        {
            if (!this.ShowLegalCapacityOrdinanceType && speciality is not null)
            {
                this.ProgramVM.Speciality = speciality;
                this.ProgramVM.Speciality.Profession = this.professionSource.FirstOrDefault(x => x.IdProfession == speciality.IdProfession);
                if (this.ProgramVM.Speciality.Profession is not null && this.ProgramVM.Speciality.Profession.IsPresupposeLegalCapacity && this.ProgramVM.Speciality.Profession.IdLegalCapacityOrdinanceType.HasValue)
                {
                    this.legalCapacityOrdinanceTypeSourceForSPKAndPP = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LegalCapacityOrdinanceType")).Where(x => x.IdKeyValue == this.ProgramVM.Speciality.Profession.IdLegalCapacityOrdinanceType || x.KeyValueIntCode == "Type13").ToList();
                }
                else
                {
                    this.legalCapacityOrdinanceTypeSourceForSPKAndPP = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LegalCapacityOrdinanceType")).Where(x => x.KeyValueIntCode == "Type13").ToList();
                }
            }
            else
            {
                this.legalCapacityOrdinanceTypeSourceForSPKAndPP = new List<KeyValueVM>();
            }
        }

        private async Task TypeFrameworkProgramSelectedHandler()
        {
            this.isTypeFrameworkProgramSelected = true;

            if (this.idSpeciality != 0)
            {
                var specialityVQS = this.specialitiesSource.FirstOrDefault(x => x.IdSpeciality == this.idSpeciality);
                int idVQS = 0;
                if (specialityVQS is not null)
                {
                    idVQS = specialityVQS.IdVQS;
                    var spkTypeFrameworkProgram = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");
                    if (this.ShowLegalCapacityOrdinanceType)
                    {
                        this.frameworkProgramSource = (await this.TrainingService.GetFrameworkProgramByIdVQSAndByIdTypeFrameworkProgramAsync(this.ProgramVM.Speciality.IdVQS, spkTypeFrameworkProgram.IdKeyValue)).ToList();
                    }
                    else
                    {
                        this.frameworkProgramSource = (await this.TrainingService.GetFrameworkProgramByIdVQSAndByIdTypeFrameworkProgramAsync(this.ProgramVM.Speciality.IdVQS, this.ProgramVM.IdCourseType)).ToList();
                    }
                }
            }
            else
            {
                this.frameworkProgramSource.Clear();
            }
        }

        public override void SubmitHandler()
        {
            this.editContext = new EditContext(this.ProgramVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidateLegalOrdinanceType;
            this.editContext.OnValidationRequested += this.ValidateProgramNumber;

            this.ProgramVM.FrameworkProgram = this.frameworkProgramSource.FirstOrDefault(x => x.IdFrameworkProgram == this.ProgramVM.IdFrameworkProgram)!;

            if (!this.ShowLegalCapacityOrdinanceType && !this.ProgramVM.IsProgramLegalCapacityOrdinance && this.ProgramVM.IdLegalCapacityOrdinanceType.HasValue)
            {
                this.ProgramVM.IdLegalCapacityOrdinanceType = null;
                this.legalCapacityDescription = string.Empty;
            }

            if (this.ShowLegalCapacityOrdinanceType)
            {
                this.ProgramVM.IdFrameworkProgram = 1;
            }

            this.editContext.Validate();

            if (this.ShowLegalCapacityOrdinanceType)
            {
                this.ProgramVM.IdFrameworkProgram = null;
            }
        }

        private void ValidateLegalOrdinanceType(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.ShowLegalCapacityOrdinanceType)
            {
                if (this.ProgramVM.IdLegalCapacityOrdinanceType is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ProgramVM, "IdLegalCapacityOrdinanceType");
                    this.messageStore?.Add(fi, $"Полето 'Правоспособност' е задължително!");
                }
            }
            else
            {
                if (this.ProgramVM.IsProgramLegalCapacityOrdinance && this.ProgramVM.IdLegalCapacityOrdinanceType is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ProgramVM, "IdLegalCapacityOrdinanceType");
                    this.messageStore?.Add(fi, $"Полето 'Правоспособност' е задължително!");
                }
            }
        }

        private void ValidateProgramNumber(object? sender, ValidationRequestedEventArgs args)
        {
            int pn = 0;
            if (!string.IsNullOrEmpty(this.ProgramVM.ProgramNumber) && int.TryParse(this.ProgramVM.ProgramNumber, out pn))
            {
                if (pn <= 0)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ProgramVM, "ProgramNumber");
                    this.messageStore?.Add(fi, $"Полето 'Номер на програмата' може да има стойност по-голяма от 0!");
                }
            }
        }
    }
}
