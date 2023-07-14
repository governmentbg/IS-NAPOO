using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using ISNAPOO.Common.Framework;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class DocValidationERUModal : BlazorBaseComponent
    {
        private SfDialog sfDialog = new SfDialog();
        private SfGrid<ERUVM> eruGrid = new SfGrid<ERUVM>();
        private ToastMsg toast = new ToastMsg();

        private List<ERUVM> eruDataSource = new List<ERUVM>();
        private string docName = string.Empty;
        private List<ERUVM> selectedERUs = new List<ERUVM>();
        private bool eruSelected = false;
        private List<CandidateCurriculumVM> selectedCurriculums = new List<CandidateCurriculumVM>();
        private List<ValidationCurriculumVM> validationCurriculums = new List<ValidationCurriculumVM>();
        private IEnumerable<KeyValueVM> kvNKRLeveSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvEKRLeveSource = new List<KeyValueVM>();

        [Parameter]
        public EventCallback<List<ERUVM>> CallbackAfterERUSelected { get; set; }

        [Inject]
        public IDOCService DOCService { get; set; }

        [Inject]
        public IERUSpecialityService ERUSpecialityService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateCurriculumERUService CandidateCurriculumERUService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        public async Task OpenModal(SpecialityVM speciality, List<CandidateCurriculumVM> selectedCurriculums = null, List<ValidationCurriculumVM> validationCurriculums = null)
        {
            this.kvNKRLeveSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("NKRLevel");
            this.kvEKRLeveSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("EKRLevel");

            this.eruDataSource.Clear();
            if (selectedCurriculums is not null)
            {
                this.selectedCurriculums = selectedCurriculums;
            }
            else
            {
                this.validationCurriculums = validationCurriculums;
            }

            DocVM doc = await this.DOCService.GetDOCByIdAsync(new DocVM() { IdDOC = speciality.IdDOC ?? default });
            if (doc is not null)
            {
                this.docName = doc.Name;
            }

            await this.SetERUsData(speciality, doc);

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SetERUsData(SpecialityVM speciality, DocVM doc)
        {
            IEnumerable<ERUVM> docErus = new List<ERUVM>();

            if (doc is not null)
            {
                docErus = await this.DOCService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = doc.IdDOC });
            }

            IEnumerable<ERUSpecialityVM> eRUSpecialities = await this.ERUSpecialityService.GetAllERUsBySpecialityIdAsync(new ERUSpecialityVM() { IdSpeciality = speciality.IdSpeciality });
            List<int> eruIds = eRUSpecialities.Select(x => x.IdERU).ToList();
            IEnumerable<ERUVM> erus = await this.DOCService.GetERUsByIdsAsync(eruIds);
            this.eruDataSource.AddRange(erus);
            docErus = docErus.Where(x => x.ERUSpecialities.Count == 0);
            this.eruDataSource.AddRange(docErus);
            this.eruDataSource = this.eruDataSource.OrderBy(x => x.ERUIntCodeSplit).ToList();

            foreach (var eru in this.eruDataSource)
            {
                if (eru.IdNKRLevel != 0)
                {
                    eru.NKRLevelName = this.kvNKRLeveSource.FirstOrDefault(x => x.IdKeyValue == eru.IdNKRLevel).Name;
                }

                if (eru.IdEKRLevel != 0)
                {
                    eru.EKRLevelName = this.kvEKRLeveSource.FirstOrDefault(x => x.IdKeyValue == eru.IdEKRLevel).Name;
                }
            }
        }

        private async Task ERUSelectedHandler(RowSelectEventArgs<ERUVM> args)
        {
            this.eruSelected = true;
            this.selectedERUs.Clear();
            this.selectedERUs = await this.eruGrid.GetSelectedRecordsAsync();
        }

        private async Task ERUDeselectedHandler(RowDeselectEventArgs<ERUVM> args)
        {
            this.selectedERUs.Clear();
            this.selectedERUs = await this.eruGrid.GetSelectedRecordsAsync();

            if (!this.selectedERUs.Any())
            {
                this.eruSelected = false;
            }
        }

        private async Task AddSelectedERUsHandler()
        {
            if (!this.eruSelected)
            {
                await this.ShowErrorAsync("Моля, изберете ЕРУ!");
            }
            else
            {
                foreach (var selectedEru in this.selectedERUs)
                {
                    foreach (var selectedCurriculum in this.selectedCurriculums)
                    {
                        if (selectedEru.IdProfessionalTraining != selectedCurriculum.IdProfessionalTraining)
                        {
                            await this.ShowErrorAsync("Не можете да добавяте ЕРУ за различен вид професионална подготовка от ДОС!");
                            return;
                        }
                    }
                }

                var result = new ResultContext<NoResult>();
                if (this.selectedCurriculums.Any())
                {
                    result = await this.CandidateCurriculumERUService.AddERUsToCurriculumListAsync(this.selectedERUs, this.selectedCurriculums);
                }
                else
                {
                    result = await this.TrainingService.AddERUsToValidationCurriculumListAsync(this.selectedERUs, this.validationCurriculums);
                }

                if (!result.HasErrorMessages)
                {
                    await this.ShowSuccessAsync("Успешно добавяне на ЕРУ!");
                    await this.CallbackAfterERUSelected.InvokeAsync(this.selectedERUs);
                }
                else
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }

                this.isVisible = false;
            }
        }
    }
}
