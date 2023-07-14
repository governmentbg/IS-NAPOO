using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;
using ISNAPOO.Core.Contracts.Candidate;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Data;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CandidateProviderTrainerSearchModal : BlazorBaseComponent
    {
        private List<CandidateProviderTrainerVM> trainers = new List<CandidateProviderTrainerVM>();
        private List<SpecialityVM> specialities = new List<SpecialityVM>();
        private CandidateProviderSearchVM currentFilter = new CandidateProviderSearchVM();
        private SfAutoComplete<int, ProfessionVM> sfAutoCompleteProfession = new SfAutoComplete<int, ProfessionVM>();
        private SfAutoComplete<int, ProfessionalDirectionVM> sfAutoCompleteProfessionalDirection = new SfAutoComplete<int, ProfessionalDirectionVM>();
        private SfMultiSelect<List<SpecialityVM>, SpecialityVM> SpecialitiesMultiSelect = new SfMultiSelect<List<SpecialityVM>, SpecialityVM>();
        private bool isCPO = true;
        private CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
        private List<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private List<ProfessionVM> professionFiltered = new List<ProfessionVM>();
        private List<ProfessionVM> professionSource = new List<ProfessionVM>();
        private List<ProfessionalDirectionVM> professionalDirectionSource = new List<ProfessionalDirectionVM>();
        private IEnumerable<KeyValueVM> kvTypePracticeOrTheory;
        private IEnumerable<KeyValueVM> kvDocumentType;
        private IEnumerable<KeyValueVM> kvStatus;
        private IEnumerable<KeyValueVM> kvComplianceDOC;
        private IEnumerable<KeyValueVM> kvVQS;
        private IEnumerable<KeyValueVM> kvEducation;

        [Parameter]
        public EventCallback<List<CandidateProviderTrainerVM>> CallBackRefreshGrid { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IProfessionService ProfessionService { get; set; }

        [Inject]
        public IProfessionService ProfessionalDirectionService { get; set; }

        public async Task OpenModal(CandidateProviderVM CandidateProviderVM, bool isCPO)
        {
            this.candidateProviderVM = CandidateProviderVM;
            this.isCPO = isCPO;
            this.trainers.Clear();
            this.trainers = (await this.CandidateProviderService.GetAllActiveCandidateProvidersTrainersByIdCandidateProviderAsync(this.candidateProviderVM.IdCandidate_Provider)).FirstOrDefault().CandidateProviderTrainers.ToList();
            if (this.isCPO)
            {
                this.professionSource = (await ProfessionService.GetAllAsync()).ToList();
                this.professionalDirectionSource = (await ProfessionalDirectionService.GetAllProfessionalDirections()).ToList();
                this.kvTypePracticeOrTheory = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType", false);
                this.kvComplianceDOC = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ComplianceDOC", false);
                this.kvVQS = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS", false);
                this.professionFiltered = this.professionSource;
                this.specialitiesSource = (await this.SpecialityService.GetAllSpecialitiesAsync(new SpecialityVM())).ToList();
            }
            
            this.kvDocumentType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerContractType", false);
            this.kvStatus = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderTrainerStatus", false);
            this.kvEducation = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education", false);
            
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task OnFilterSpeciality(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => x.CodeAndName.ToLower().Contains(args.Text.ToLower())).ToList();

                    var query = new Query().Where(new WhereFilter() { Field = "CodeAndName", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.SpecialitiesMultiSelect.FilterAsync(this.specialities, query);
                }
                catch (Exception ex) { }
            }
        }

        private void OnFocusSpeciality()
        {
            if (!(currentFilter.IdProfession == null || currentFilter.IdProfession == 0))
            {
                this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => x.IdProfession == currentFilter.IdProfession).ToList();
            }
            else if (currentFilter.IdProfessionalDirection != null && currentFilter.IdProfessionalDirection != 0)
            {
                this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => this.professionalDirectionSource.Any(y => y.IdProfessionalDirection == this.professionSource.First(z => z.IdProfession == x.IdProfession).IdProfessionalDirection)).ToList();
            }
            else
            {
                this.specialities = specialitiesSource;
            }
        }

        private void OnProfessionSelect()
        {
            this.specialities = this.specialities.Where(x => x.IdProfession == this.currentFilter.IdProfession).ToList();
        }

        private void OnProfessionalDirectionSelect()
        {
            this.professionFiltered = this.professionSource.Where(x => x.IdProfessionalDirection == this.currentFilter.IdProfessionalDirection).ToList();
            this.currentFilter.IdProfession = this.professionFiltered.Any(x => x.IdProfession == this.currentFilter.IdProfession) ? this.currentFilter.IdProfession : 0;

            this.currentFilter.Specialities = this.currentFilter.Specialities.Where(x => this.professionFiltered.All(y => y.IdProfession == x.IdProfession)).ToList();
            this.specialities = this.specialities.Where(x => this.professionFiltered.Any(y => y.IdProfession == x.IdProfession)).ToList();
        }

        public async Task OnKvEducationtypeSelect()
        {
            if (currentFilter.kvPracticeOrTheory == null || currentFilter.kvPracticeOrTheory == 0)
            {
                this.currentFilter.kvVMPracticeOrTheory = null;
            }
            else
            {
                this.currentFilter.kvVMPracticeOrTheory = await this.DataSourceService.GetKeyValueByIdAsync(currentFilter.kvPracticeOrTheory);
            }
        }

        public void ClearFilter()
        {
            this.currentFilter = new CandidateProviderSearchVM();
            this.professionFiltered = this.professionSource;
            this.specialities = this.specialitiesSource;
        }

        public async Task ReloadTrainers()
        {
            this.trainers.Clear();
            this.trainers = (await this.CandidateProviderService.GetAllActiveCandidateProvidersTrainersByIdCandidateProviderAsync(this.candidateProviderVM.IdCandidate_Provider)).FirstOrDefault().CandidateProviderTrainers.ToList();
        }

        public async Task SumbitFilter()
        {
            await this.ReloadTrainers();
            this.trainers = this.trainers.Where(d =>
            (!String.IsNullOrEmpty(currentFilter.Name) ? d.FirstName.Trim().ToLower().ToString().Contains(currentFilter.Name.Trim().ToLower().ToString()) : true)
            && (!String.IsNullOrEmpty(currentFilter.MiddleName) ? !String.IsNullOrEmpty(d.SecondName) ? d.SecondName.Trim().ToLower().ToString().Contains(currentFilter.MiddleName.Trim().ToLower().ToString()) : false : true)
            && (!String.IsNullOrEmpty(currentFilter.FamilyName) ? d.FamilyName.Trim().ToLower().ToString().Contains(currentFilter.FamilyName.Trim().ToLower().ToString()) : true)
            && (!String.IsNullOrEmpty(currentFilter.Indent) ? !String.IsNullOrEmpty(d.Indent) ? d.Indent.Contains(currentFilter.Indent.TrimEnd().TrimStart().ToString()) : false : true)
            && ((!String.IsNullOrEmpty(currentFilter.EducationCertificateNotes) && !String.IsNullOrEmpty(d.EducationCertificateNotes)) ? d.EducationCertificateNotes.ToLower().TrimEnd().Contains(currentFilter.EducationCertificateNotes.ToLower().TrimEnd()) : true)
            && ((!String.IsNullOrEmpty(currentFilter.EducationSpecialityNotes) && !String.IsNullOrEmpty(d.EducationSpecialityNotes)) ? d.EducationSpecialityNotes.ToLower().TrimEnd().Contains(currentFilter.EducationSpecialityNotes.ToLower().TrimEnd()) : true)
            && ((!String.IsNullOrEmpty(currentFilter.EducationAcademicNotes) && !String.IsNullOrEmpty(d.EducationAcademicNotes)) ? d.EducationAcademicNotes.ToLower().TrimEnd().Contains(currentFilter.EducationAcademicNotes.ToLower().TrimEnd()) : true)
            && (currentFilter.IdComplianceDOC is not null && currentFilter.IdComplianceDOC != 0 ? d.CandidateProviderTrainerSpecialities.Any(x => x.IdComplianceDOC == currentFilter.IdComplianceDOC) : true)
            && (currentFilter.IdStatus != 0 ? d.IdStatus == currentFilter.IdStatus : true)
            && (currentFilter.IdContractType is not null && currentFilter.IdContractType != 0 ? d.IdContractType == this.currentFilter.IdContractType : true)
            && (currentFilter.IdEducation != 0 ? d.IdEducation == this.currentFilter.IdEducation : true)
            && (currentFilter.Specialities is not null && currentFilter.Specialities.Count != 0 ? d.CandidateProviderTrainerSpecialities.Any(y => this.currentFilter.Specialities.All(x => x.IdSpeciality == y.IdSpeciality)) : true)
            && ((currentFilter.Specialities is null || currentFilter.Specialities.Count == 0) && (currentFilter.IdProfession != 0) ? d.CandidateProviderTrainerSpecialities.Any(y => y.Speciality.IdProfession == currentFilter.IdProfession) : true)
            && ((currentFilter.IdProfessionalDirection != 0) && (currentFilter.Specialities is null || currentFilter.Specialities.Count == 0) && (currentFilter.IdProfession == 0) ? d.CandidateProviderTrainerSpecialities.Any(y => y.Speciality.Profession.IdProfessionalDirection == currentFilter.IdProfessionalDirection) : true)
            ).ToList();
            if (this.trainers.Any() && this.currentFilter.kvVMPracticeOrTheory != null && this.currentFilter.kvPracticeOrTheory != null)
            {
                var list = new List<CandidateProviderTrainerVM>();
                if (this.currentFilter.Specialities.Any())
                {
                    foreach (var speciality in currentFilter.Specialities)
                    {
                        foreach (var trainer in this.trainers)
                        {
                            if (trainer.CandidateProviderTrainerSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality))
                            {
                                foreach (var trainerSpeciality in trainer.CandidateProviderTrainerSpecialities.Where(x => x.IdSpeciality == speciality.IdSpeciality).ToList())
                                {
                                    if (this.currentFilter.kvVMPracticeOrTheory.KeyValueIntCode == "TrainingInTheoryAndPractice")
                                    {
                                        if (this.currentFilter.kvVMPracticeOrTheory.IdKeyValue == trainerSpeciality.IdUsage)
                                        {
                                            if (!list.Any(x => x.IdCandidateProviderTrainer == trainer.IdCandidateProviderTrainer))
                                            {
                                                list.Add(trainer);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (this.currentFilter.kvVMPracticeOrTheory.IdKeyValue == trainerSpeciality.IdUsage || this.kvTypePracticeOrTheory.First(x => x.KeyValueIntCode == "TrainingInTheoryAndPractice").IdKeyValue == trainerSpeciality.IdUsage)
                                        {
                                            if (!list.Any(x => x.IdCandidateProviderTrainer == trainer.IdCandidateProviderTrainer))
                                            {
                                                list.Add(trainer);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var trainer in this.trainers)
                    {
                        if (trainer.CandidateProviderTrainerSpecialities.Any())
                        {
                            foreach (var trainerSpeciality in trainer.CandidateProviderTrainerSpecialities)
                            {
                                if (this.currentFilter.kvVMPracticeOrTheory.KeyValueIntCode == "TrainingInTheoryAndPractice")
                                {
                                    if (this.currentFilter.kvVMPracticeOrTheory.IdKeyValue == trainerSpeciality.IdUsage)
                                    {
                                        if (!list.Any(x => x.IdCandidateProviderTrainer == trainer.IdCandidateProviderTrainer))
                                        {
                                            list.Add(trainer);
                                        }
                                    }
                                }
                                else
                                {
                                    if (this.currentFilter.kvVMPracticeOrTheory.IdKeyValue == trainerSpeciality.IdUsage || this.kvTypePracticeOrTheory.First(x => x.KeyValueIntCode == "TrainingInTheoryAndPractice").IdKeyValue == trainerSpeciality.IdUsage)
                                    {
                                        if (!list.Any(x => x.IdCandidateProviderTrainer == trainer.IdCandidateProviderTrainer))
                                        {
                                            list.Add(trainer);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                this.trainers = list;
            }
            await this.CallBackRefreshGrid.InvokeAsync(this.trainers.ToList());
            this.isVisible = false;
        }
    }
}
