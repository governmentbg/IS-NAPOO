using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using RegiX;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Candidate.ChangeLicenzing
{
    public partial class ChangeCompletenessCheckModal : BlazorBaseComponent
    {
        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public EventCallback<ProcedureModal> CallbackRefreshDocumentsGrid { get; set; }

        #region Inject
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IRegiXService RegiXService { get; set; }
        [Inject]
        public IProviderService providerService { get; set; }
        [Inject]
        public IProfessionalDirectionService professionalDirectionService { get; set; }
        [Inject]
        public IExpertService expertService { get; set; }
        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }
        #endregion
        private bool isVisiblePositiveStep = false;
        private bool isVisibleNegativeStep = false;
        private bool isBtnDisabled = false;

        private DateTime? napooReportDeadline;
        private DateTime? expertReportDeadline;

        private ShowRegixDataModal regixData = new ShowRegixDataModal();

        SfGrid<ProcedureExternalExpertVM> externalExpertGrid = new SfGrid<ProcedureExternalExpertVM>();
        SfGrid<NegativeIssueVM> negativeIssueGrid = new SfGrid<NegativeIssueVM>();

        private ProcedureExternalExpertVM LeadingExpert = new ProcedureExternalExpertVM();
        private ProcedureExternalExpertVM procedureExternalExpert = new ProcedureExternalExpertVM();
        //Използва се само за филтър на таблицата с външни изпълнители
        private ProcedureExternalExpertVM filterGridProcedureExpertVM = new ProcedureExternalExpertVM();

        //Binds
        private IEnumerable<KeyValueVM> expertCommissionValues = new List<KeyValueVM>();
        private int idExpertCommission = GlobalConstants.INVALID_ID_ZERO;
        private ProcedureExpertCommissionVM commissionVM = null;
        private string NegativeIssueText = string.Empty;


        //Source
        private IEnumerable<ProcedureExternalExpertVM> addedExpertGridSource = new List<ProcedureExternalExpertVM>();
        private IEnumerable<NegativeIssueVM> addedNegativeIssueSource = new List<NegativeIssueVM>();
        private IEnumerable<ExpertVM> leadingExpertDatasource = new List<ExpertVM>();
        private IEnumerable<ExpertVM> externalExpertDatasource = new List<ExpertVM>();
        private IEnumerable<ProfessionalDirectionVM> professionalDirectionSource = new List<ProfessionalDirectionVM>();

        ToastMsg toast;

        protected override async Task OnInitializedAsync()
        {

            this.editContext = new EditContext(this.LeadingExpert);

            this.LeadingExpert.IsLeadingExpert = true;
            this.LeadingExpert.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            var listProcedureExperts = await this.providerService.GetAllProcedureExternalExpertsAsync(this.LeadingExpert);
            var expert = listProcedureExperts.FirstOrDefault();
            if (expert is not null)
            {
                this.LeadingExpert = expert;
            }

            //Вземаме експерти които са служители на напо за да ги покачим като водещ експерт
            var expertFilterVM = new ExpertVM();
            expertFilterVM.IsNapooExpert = true;
            this.leadingExpertDatasource = await this.expertService.GetAllExpertsAsync(expertFilterVM);


            //Трябва да се взимат само правилните направления
            this.professionalDirectionSource = await this.professionalDirectionService.GetAllProfessionalDirectionsByCandidateProviderIdAsync(this.CandidateProviderVM.IdCandidate_Provider);


            // Зареждане на грида с добавените външни експерти
            this.filterGridProcedureExpertVM.IsLeadingExpert = false;
            this.filterGridProcedureExpertVM.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
            this.addedExpertGridSource = await this.providerService.GetAllProcedureExternalExpertsAsync(this.filterGridProcedureExpertVM);

            //Зареждане на грида с Нередности
            this.addedNegativeIssueSource = await this.providerService.GetAllNegativeIssuesByIdStartedProcedureAsync(this.CandidateProviderVM.IdStartedProcedure.Value);


            //Зареждане на крайна дата за доклад на НАПОО
            var procedure = await this.providerService.GetStartedProcedureByIdAsync(this.CandidateProviderVM.IdStartedProcedure.Value);
            if (procedure is not null)
            {
                this.napooReportDeadline = procedure.NapooReportDeadline;
                this.expertReportDeadline = procedure.ExpertReportDeadline;
            }

            //Зареждане на експертна комисия
            this.expertCommissionValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertCommission");
            //Зареждане на полето ако има запис вече в базата
            this.commissionVM = await this.providerService.GetProcedureExpertCommissionByIdStartProcedureAsync(this.CandidateProviderVM.IdStartedProcedure.Value);
            if (this.commissionVM is not null)
            {
                this.idExpertCommission = this.commissionVM.IdExpertCommission;
            }


            await ShowHideAccordionItemAndButtons();
        }

        private async Task FilterExternalExpert(SelectEventArgs<ProfessionalDirectionVM> args)
        {
            this.procedureExternalExpert.IdExpert = GlobalConstants.INVALID_ID;

            var expertFilterVM = new ExpertVM();
            expertFilterVM.IdProfessionalDirectionFilter = args.ItemData.IdProfessionalDirection;
            this.externalExpertDatasource = await this.expertService.GetAllExpertsAsync(expertFilterVM);
            this.StateHasChanged();

        }

        public override void SubmitHandler()
        {
            this.editContext = new EditContext(this.LeadingExpert);
            this.editContext.EnableDataAnnotationsValidation();

            this.editContext.Validate();
        }

        public async Task Save()
        {
            var resultContext = new ResultContext<ProcedureExternalExpertVM>();
            resultContext.ResultContextObject = this.LeadingExpert;
            resultContext = await this.providerService.SaveProcedureExternalExpertAsync(resultContext);

            if (this.idExpertCommission > GlobalConstants.INVALID_ID_ZERO)
            {
                var contextCommision = new ResultContext<ProcedureExpertCommissionVM>();

                if (this.commissionVM != null)
                {
                    contextCommision.ResultContextObject.IdProcedureExpertCommission = this.commissionVM.IdProcedureExpertCommission;
                }
                contextCommision.ResultContextObject.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
                contextCommision.ResultContextObject.IdExpertCommission = this.idExpertCommission;
                await this.providerService.SaveProcedureExpertCommissionAsync(contextCommision);
            }

            if (this.napooReportDeadline.HasValue && this.napooReportDeadline.Value > DateTime.MinValue)
            {
                await this.providerService.UpdateStartedProcedureNapooDeadlineAsync(this.CandidateProviderVM.IdStartedProcedure.Value, this.napooReportDeadline.Value);
            }

            if (this.expertReportDeadline.HasValue && this.expertReportDeadline.Value > DateTime.MinValue)
            {
                await this.providerService.UpdateStartedProcedureExpertDeadlineAsync(this.CandidateProviderVM.IdStartedProcedure.Value, this.expertReportDeadline.Value);
            }


            if (resultContext.HasMessages)
            {
                toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                await toast.sfSuccessToast.ShowAsync();
                resultContext.ListMessages.Clear();
            }
            else
            {
                toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                await toast.sfErrorToast.ShowAsync();
                resultContext.ListErrorMessages.Clear();
            }
        }

        private async Task DeleteRowExternalExpert(ProcedureExternalExpertVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
                if (!hasPermission) { return; }

                var resultContext = new ResultContext<ProcedureExternalExpertVM>();
                resultContext.ResultContextObject = model;

                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли сте, че искате да изтриете отбелязаният ред?");
                if (isConfirmed)
                {
                    resultContext = await this.providerService.DeleteProcedureExternalExpertAsync(resultContext);

                    if (resultContext.HasMessages)
                    {
                        toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                        await toast.sfSuccessToast.ShowAsync();
                        resultContext.ListMessages.Clear();

                        //Рефреш на грида с добавените външни експерти
                        this.addedExpertGridSource = await this.providerService.GetAllProcedureExternalExpertsAsync(filterGridProcedureExpertVM);
                        this.externalExpertGrid.Refresh();
                    }
                    else
                    {
                        toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await toast.sfErrorToast.ShowAsync();
                        resultContext.ListErrorMessages.Clear();
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        private async Task AddExternalExpert()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
                if (!hasPermission) { return; }

                if (this.procedureExternalExpert.IdExpert > GlobalConstants.INVALID_ID_ZERO
                    && this.procedureExternalExpert.IdProfessionalDirection > GlobalConstants.INVALID_ID_ZERO)
                {
                    var resultContext = new ResultContext<ProcedureExternalExpertVM>();
                    resultContext.ResultContextObject = this.procedureExternalExpert;
                    resultContext.ResultContextObject.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
                    resultContext = await this.providerService.SaveProcedureExternalExpertAsync(resultContext);


                    if (resultContext.HasMessages)
                    {

                        //Рефреш на грида с добавените външни експерти
                        this.addedExpertGridSource = await this.providerService.GetAllProcedureExternalExpertsAsync(filterGridProcedureExpertVM);
                        this.externalExpertGrid.Refresh();
                        //Зачистване на полетата
                        this.procedureExternalExpert.IdExpert = GlobalConstants.INVALID_ID_ZERO;
                        this.procedureExternalExpert.IdProfessionalDirection = GlobalConstants.INVALID_ID_ZERO;
                        this.StateHasChanged();

                        toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                        await toast.sfSuccessToast.ShowAsync();
                        resultContext.ListMessages.Clear();

                    }
                    else
                    {
                        toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await toast.sfErrorToast.ShowAsync();
                        resultContext.ListErrorMessages.Clear();
                    }
                }
                else
                {
                    toast.sfErrorToast.Content = "Моля изберете 'Професионално направление' и 'Външен експерт' за да добавите!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        private async Task DeleteRowNegativeIssue(NegativeIssueVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var resultContext = new ResultContext<NegativeIssueVM>();
                resultContext.ResultContextObject = model;

                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли сте, че искате да изтриете отбелязаният ред?");
                if (isConfirmed)
                {
                    resultContext = await this.providerService.DeleteNegativeIssueAsync(resultContext);

                    if (resultContext.HasMessages)
                    {
                        toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                        await toast.sfSuccessToast.ShowAsync();
                        resultContext.ListMessages.Clear();

                        //Рефреш на грида с добавените нередности
                        this.addedNegativeIssueSource = await this.providerService.GetAllNegativeIssuesByIdStartedProcedureAsync(this.CandidateProviderVM.IdStartedProcedure.Value);
                        this.negativeIssueGrid.Refresh();
                    }
                    else
                    {
                        toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await toast.sfErrorToast.ShowAsync();
                        resultContext.ListErrorMessages.Clear();
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        private async Task AddNegativeIssue()
        {

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(this.NegativeIssueText))
                {
                    var resultContext = new ResultContext<NegativeIssueVM>();
                    resultContext.ResultContextObject.NegativeIssueText = this.NegativeIssueText;
                    resultContext.ResultContextObject.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
                    resultContext = await this.providerService.SaveNegativeIssueAsync(resultContext);


                    if (resultContext.HasMessages)
                    {
                        //Рефреш на грида с добавените нередности
                        this.addedNegativeIssueSource = await this.providerService.GetAllNegativeIssuesByIdStartedProcedureAsync(this.CandidateProviderVM.IdStartedProcedure.Value);
                        this.negativeIssueGrid.Refresh();
                        //Зачистване на полетата
                        this.NegativeIssueText = string.Empty;
                        this.StateHasChanged();

                        toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                        await toast.sfSuccessToast.ShowAsync();
                        resultContext.ListMessages.Clear();

                    }
                    else
                    {
                        toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await toast.sfErrorToast.ShowAsync();
                        resultContext.ListErrorMessages.Clear();
                    }
                }
                else
                {
                    toast.sfErrorToast.Content = "Моля напишете 'Нередност' за да добавите!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        private async Task PositiveStep()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
                if (!hasPermission) { return; }

                if (!this.isBtnDisabled)
                {
                    var resultContext = new ResultContext<StartedProcedureProgressVM>();
                    var kvStepPositive = await dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "LeadingExpertGavePositiveAssessment");

                    resultContext.ResultContextObject = new StartedProcedureProgressVM()
                    {


                        IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                        IdStep = kvStepPositive.IdKeyValue,
                        StepDate = DateTime.Now
                    };

                    resultContext = await this.providerService.InsertStartedProcedureProgressAsync(resultContext);

                    await this.providerService.SetApplicationStatusAfterPositiveAssessmentAsync(this.CandidateProviderVM.IdCandidate_Provider);

                    if (resultContext.HasMessages)
                    {
                        toast.sfSuccessToast.Content = "Успешно даване на оценка!";
                        await toast.sfSuccessToast.ShowAsync();
                        resultContext.ListMessages.Clear();
                    }
                    else
                    {
                        toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await toast.sfErrorToast.ShowAsync();
                        resultContext.ListErrorMessages.Clear();
                    }

                    await ShowHideAccordionItemAndButtons();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        private async Task NegativeStep()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
                if (!hasPermission) { return; }

                if (!this.isBtnDisabled)
                {
                    var resultContext = new ResultContext<StartedProcedureProgressVM>();
                    var kvStepNegative = await dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "LeadingExpertGaveNegativeAssessment");

                    resultContext.ResultContextObject = new StartedProcedureProgressVM()
                    {


                        IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                        IdStep = kvStepNegative.IdKeyValue,
                        StepDate = DateTime.Now
                    };

                    resultContext = await this.providerService.InsertStartedProcedureProgressAsync(resultContext);

                    if (resultContext.HasMessages)
                    {
                        toast.sfSuccessToast.Content = "Успешно даване на оценка!";
                        await toast.sfSuccessToast.ShowAsync();
                        resultContext.ListMessages.Clear();
                    }
                    else
                    {
                        toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await toast.sfErrorToast.ShowAsync();
                        resultContext.ListErrorMessages.Clear();
                    }

                    await ShowHideAccordionItemAndButtons();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task PrintReportPositive()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var model = new ProcedureDocumentVM();
                model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
                var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

                var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
                resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
                var kvDocTypeApplication1 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application1a");
                var kvDocTypeApplication2 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application2a");

                if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue) &&
                    !listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue))
                {
                    var doc1 = new ProcedureDocumentVM()
                    {
                        TypeLicensing = GlobalConstants.CHANCE_LICENSING,
                        IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                        IsValid = true,
                        IdDocumentType = kvDocTypeApplication1.IdKeyValue,
                    };
                    resultContext.ResultContextObject.Add(doc1);

                    var doc2 = new ProcedureDocumentVM()
                    {
                        TypeLicensing = GlobalConstants.CHANCE_LICENSING,
                        IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                        IsValid = true,
                        IdDocumentType = kvDocTypeApplication2.IdKeyValue,
                    };
                    resultContext.ResultContextObject.Add(doc2);


                    resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                    if (resultContext.HasMessages)
                    {
                        toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                        await toast.sfSuccessToast.ShowAsync();
                        resultContext.ListMessages.Clear();
                    }
                    else
                    {
                        toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await toast.sfErrorToast.ShowAsync();
                        resultContext.ListErrorMessages.Clear();
                    }

                    await CallbackRefreshDocumentsGrid.InvokeAsync();
                }
                else
                {
                    toast.sfErrorToast.Content = "Вече има готови документи за положителна оценка!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        private async Task Application3()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var model = new ProcedureDocumentVM();
                model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
                var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

                var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
                resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
                var kvDocTypeApplication3 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application3a");
                var kvDocTypeApplication2 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application2a");
                if (true/*listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue)
                    && listDocs.First(x => x.IdDocumentType == kvDocTypeApplication2.IdKeyValue).DS_OFFICIAL_DocNumber != null*/)
                {
                    if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication3.IdKeyValue))
                    {
                        var doc3 = new ProcedureDocumentVM()
                        {
                            TypeLicensing = GlobalConstants.CHANCE_LICENSING,
                            IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                            IsValid = true,
                            IdDocumentType = kvDocTypeApplication3.IdKeyValue,
                        };
                        resultContext.ResultContextObject.Add(doc3);


                        resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                        if (resultContext.HasMessages)
                        {
                            toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                            await toast.sfSuccessToast.ShowAsync();
                            resultContext.ListMessages.Clear();
                        }
                        else
                        {
                            toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                            await toast.sfErrorToast.ShowAsync();
                            resultContext.ListErrorMessages.Clear();
                        }

                        await CallbackRefreshDocumentsGrid.InvokeAsync();
                    }
                    else
                    {
                        toast.sfErrorToast.Content = "Вече има готови документи за Приложение 3!";
                        await toast.sfErrorToast.ShowAsync();
                    }
                }
                else
                {
                    toast.sfErrorToast.Content = "Няма въведен номер на Приложение 2";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }
        private async Task Application4()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var model = new ProcedureDocumentVM();
                model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
                var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

                var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
                resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
                var kvDocTypeApplication4 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application4a");
                if (true/*listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue)
                    && listDocs.First(x => x.IdDocumentType == kvDocTypeApplication2.IdKeyValue).DS_OFFICIAL_DocNumber != null*/)
                {
                    if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication4.IdKeyValue))
                    {
                        //Вземаме данните за може да създадем по 1 запис за всеки външен експерт
                        var data = await this.providerService.GetStartedProcedureByIdForGenerateDocumentAsync(this.CandidateProviderVM.IdStartedProcedure.Value);
                        var externalExperts = data.ProcedureExternalExperts.Where(pe => pe.IdProfessionalDirection != null).ToList();

                        foreach (var item in externalExperts)
                        {
                            var doc4 = new ProcedureDocumentVM()
                            {
                                TypeLicensing = GlobalConstants.CHANCE_LICENSING,
                                IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                                IsValid = true,
                                IdDocumentType = kvDocTypeApplication4.IdKeyValue,

                                IdExpert = item.IdExpert,
                            };
                            resultContext.ResultContextObject.Add(doc4);
                        }

                        resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                        if (resultContext.HasMessages)
                        {
                            toast.sfSuccessToast.Content = "Успешно създаване на документи!";
                            await toast.sfSuccessToast.ShowAsync();
                            resultContext.ListMessages.Clear();
                        }
                        else
                        {
                            toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                            await toast.sfErrorToast.ShowAsync();
                            resultContext.ListErrorMessages.Clear();
                        }

                        await CallbackRefreshDocumentsGrid.InvokeAsync();
                    }
                    else
                    {
                        toast.sfErrorToast.Content = "Вече има готови документи за Приложение 4!";
                        await toast.sfErrorToast.ShowAsync();
                    }
                }
                else
                {
                    toast.sfErrorToast.Content = "Няма въведен номер на Приложение 2";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }


        private async Task PrintReportNegative()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var model = new ProcedureDocumentVM();
                model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
                var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

                var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
                resultContext.ResultContextObject = new List<ProcedureDocumentVM>();
                var kvDocTypeApplication6 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application6a");
                var kvDocTypeApplication7 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application7a");

                if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication6.IdKeyValue) &&
                    !listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication7.IdKeyValue))
                {
                    //Създаваме документ с тип приложение 6
                    var doc6 = new ProcedureDocumentVM()
                    {
                        TypeLicensing = GlobalConstants.CHANCE_LICENSING,
                        IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                        IsValid = true,
                        IdDocumentType = kvDocTypeApplication6.IdKeyValue,
                    };
                    resultContext.ResultContextObject.Add(doc6);

                    //Създаваме документ с тип приложение 7
                    var doc7 = new ProcedureDocumentVM()
                    {
                        TypeLicensing = GlobalConstants.CHANCE_LICENSING,
                        IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                        IsValid = true,
                        IdDocumentType = kvDocTypeApplication7.IdKeyValue,
                    };
                    resultContext.ResultContextObject.Add(doc7);

                    resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);

                    if (resultContext.HasMessages)
                    {
                        toast.sfSuccessToast.Content = "Успешно създаване на документ!";
                        await toast.sfSuccessToast.ShowAsync();
                        resultContext.ListMessages.Clear();
                    }
                    else
                    {
                        toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await toast.sfErrorToast.ShowAsync();
                        resultContext.ListErrorMessages.Clear();
                    }

                    await CallbackRefreshDocumentsGrid.InvokeAsync();
                }
                else
                {
                    toast.sfErrorToast.Content = "Вече има готови документи за отрицателна оценка!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        private async Task LicenseDenial()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var model = new ProcedureDocumentVM();
                model.IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value;
                var listDocs = await this.providerService.GetAllProcedureDocumentsAsync(model);

                var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
                var kvDocTypeApplication8 = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application8a");

                if (!listDocs.Any(d => d.IdDocumentType == kvDocTypeApplication8.IdKeyValue))
                {
                    //Създаваме документ с тип приложение 8
                    var doc8 = new ProcedureDocumentVM()
                    {
                        TypeLicensing = GlobalConstants.CHANCE_LICENSING,
                        IdStartedProcedure = this.CandidateProviderVM.IdStartedProcedure.Value,
                        IsValid = true,
                        IdDocumentType = kvDocTypeApplication8.IdKeyValue,
                    };

                    resultContext.ResultContextObject.Add(doc8);

                    resultContext = await this.providerService.InsertProcedureDocumentFromListAsync(resultContext);


                    if (resultContext.HasMessages)
                    {
                        var kvLicenceDenied = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "RejectedApplicationLicensingNewCenter");
                        await this.CandidateProviderService.ChangeCandidateProviderApplicationStatusAsync(this.CandidateProviderVM.IdCandidate_Provider, kvLicenceDenied.IdKeyValue);

                        toast.sfSuccessToast.Content = "Успешно създаване на документ!";
                        await toast.sfSuccessToast.ShowAsync();
                        resultContext.ListMessages.Clear();
                    }
                    else
                    {
                        toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await toast.sfErrorToast.ShowAsync();
                        resultContext.ListErrorMessages.Clear();
                    }

                    await CallbackRefreshDocumentsGrid.InvokeAsync();
                }
                else
                {
                    toast.sfErrorToast.Content = "Вече има готов документ за отказ лицензия!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        public async Task CheckInRegix()
        {
            var callContext = await this.GetCallContextAsync(this.BulstatCheckKV);
            var actualStateResponseType = RegiXService.GetActualState(this.CandidateProviderVM.PoviderBulstat, callContext);
            await this.LogRegiXRequestAsync(callContext, actualStateResponseType != null);

            this.regixData.OpenModal(actualStateResponseType);
        }

        private async Task ShowHideAccordionItemAndButtons()
        {
            var allStartedProcedureProgress = await this.providerService.GetAllStartedProcedureProgressByIdStartProcedureAsync(this.CandidateProviderVM.IdStartedProcedure.Value);

            var kvStepPositive = await dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "LeadingExpertGavePositiveAssessment");
            var kvStepNegative = await dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "LeadingExpertGaveNegativeAssessment");

            if (allStartedProcedureProgress.Any(p => p.IdStep == kvStepPositive.IdKeyValue))
            {
                this.isVisiblePositiveStep = true;

            }
            else if (allStartedProcedureProgress.Any(p => p.IdStep == kvStepNegative.IdKeyValue))
            {
                this.isVisibleNegativeStep = true;

            }

            //Крием бутоните ако имаме дадена някаква оценка
            if (this.isVisiblePositiveStep || this.isVisibleNegativeStep)
            {
                this.isBtnDisabled = true;
            }

            this.StateHasChanged();
        }
    }
}
