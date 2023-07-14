using System.ComponentModel.DataAnnotations;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CurriculumModificationReasonModal : BlazorBaseComponent
    {
        private CurriculumModificationModal curriculumModificationModal = new CurriculumModificationModal();

        private CandidateProviderSpecialityVM candidateProviderSpecialityVM = new CandidateProviderSpecialityVM();
        private IEnumerable<KeyValueVM> kvModificationReasonsSource = new List<KeyValueVM>();
        private string title = string.Empty;
        private ValidationModel model = new ValidationModel();

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(CandidateProviderSpecialityVM candidateProviderSpecialityVM)
        {
            this.model = new ValidationModel();
            this.editContext = new EditContext(this.model);

            this.candidateProviderSpecialityVM = candidateProviderSpecialityVM;

            this.SetTitle();

            this.kvModificationReasonsSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CurriculumModificationReasonType"))
                .Where(x => string.IsNullOrEmpty(x.DefaultValue1))
                .OrderBy(x => x.Order)
                    .ToList();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void SetTitle()
        {
            var specialities = this.DataSourceService.GetAllSpecialitiesList();
            var speciality = specialities.FirstOrDefault(x => x.IdSpeciality == this.candidateProviderSpecialityVM.IdSpeciality);
            this.title = $"Промяна на учебен план и учебни програми за специалност <span style=\"color: #ffffff;\">{speciality!.CodeAndName}</span>";
        }

        private async Task SubmitBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();
                
                if (this.editContext.Validate())
                {
                    // Проверка за по-нови промени на учебен план с дата след model.ValidFromDate
                    var curriculumModification = await this.CandidateProviderService.IsNewestValidFromDateByIdCandidateProviderSpecialityAndNewValidFromDateAsync(this.candidateProviderSpecialityVM.IdCandidateProviderSpeciality, this.model.ValidFromDate!.Value);
                    if (curriculumModification is not null)
                    {
                        await this.ShowErrorAsync($"Не можете да въвеждате промени в учебния план и учебните програми с по-стара дата! В системата има въведен учебен план за същата специалност на статус 'Окончателен' от дата {curriculumModification.ValidFromDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                        this.loading = false;
                        this.SpinnerHide();
                        return;
                    }

                    var kvDOSChangeValue = this.kvModificationReasonsSource.FirstOrDefault(x => x.KeyValueIntCode == "DOSChange");
                    var result = await this.CandidateProviderService.CreateCurriculumModificationEntryAsync(this.candidateProviderSpecialityVM.IdCandidateProviderSpeciality, this.model.IdModificationReason!.Value, this.model.ValidFromDate!.Value, this.model.IdModificationReason == kvDOSChangeValue!.IdKeyValue);
                    if (!result.HasErrorMessages)
                    {
                        var specialities = this.DataSourceService.GetAllSpecialitiesList();
                        var speciality = specialities.FirstOrDefault(x => x.IdSpeciality == this.candidateProviderSpecialityVM.IdSpeciality);

                        this.isVisible = false;

                        await this.curriculumModificationModal.OpenModal(result.NewEntityId!.Value, this.candidateProviderSpecialityVM, speciality!);
                    }
                    else
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private class ValidationModel
        {
            [Required(ErrorMessage = "Полето 'Причина за промяна' е задължително!")]
            public int? IdModificationReason { get; set; }

            [Required(ErrorMessage = "Полето 'Дата на влизане в сила' е задължително!")]
            public DateTime? ValidFromDate { get; set; }
        }
    }
}
