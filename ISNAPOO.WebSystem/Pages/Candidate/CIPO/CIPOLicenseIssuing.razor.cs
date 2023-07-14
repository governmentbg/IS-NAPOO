using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Licensing;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOLicenseIssuing : BlazorBaseComponent
    {
        private bool disableCompleteProcedureBtn = false;

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Inject]
        public ILicensingProcedureDocumentCIPOService LicensingProcedureDocumentCIPOService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProviderService ProviderService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.CandidateProviderVM);

            // ако CP е лицензиран - замразява бутона за приключване на процедурата
            if (!this.CandidateProviderVM.IdApplicationStatus.HasValue)
            {
                this.disableCompleteProcedureBtn = true;
            }
        }

        public override void SubmitHandler()
        {
            this.editContext = new EditContext(this.CandidateProviderVM);
            this.editContext.EnableDataAnnotationsValidation();

            this.editContext.Validate();
        }

        private async Task ApproveLicensingBtn()
        {
            if (!this.disableCompleteProcedureBtn)
            {
                bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да приключите процедурата? След потвърждение няма да могат да бъдат извършвани други промени.");
                if (confirmed)
                {
                    this.SpinnerShow();

                    if (this.loading)
                    {
                        return;
                    }
                    try
                    {
                        this.loading = true;

                        var kvDocApplication18 = await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application19");
                        var procedureDoc = await this.ProviderService.GetProcedureDocumentByIdStartedProcedureAndIdDocumentTypeAsync(this.CandidateProviderVM.IdStartedProcedure!.Value, kvDocApplication18.IdKeyValue);
                        if (procedureDoc is null)
                        {
                            await this.ShowErrorAsync("Не можете да приключите процедурата, ако няма издадено 'Приложение 19'!");
                            this.loading = false;
                            this.SpinnerHide();
                            return;
                        }

                        var result = await this.CandidateProviderService.ApproveChangedApplicationAsync(this.CandidateProviderVM, procedureDoc);

                        if (result.Contains("успешен"))
                        {
                            await this.ShowSuccessAsync(result);

                            this.disableCompleteProcedureBtn = true;
                        }
                        else
                        {
                            await this.ShowErrorAsync(result);
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

        private async Task GenerateLicensingTemplateBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.CandidateProviderVM.LicenceDate == null)
                {
                    await this.ShowErrorAsync("Моля, въведете дата на лицензиране!");
                    this.SpinnerHide();
                    return;
                }

                await this.CandidateProviderService.SetCandidateProviderLicenseDateAsync(this.CandidateProviderVM);

                if (string.IsNullOrEmpty(this.CandidateProviderVM.LicenceNumber))
                {
                    var licenseNumber = await this.LicensingProcedureDocumentCIPOService.GenerateLicenseNumberAsync(this.CandidateProviderVM);
                    var licenseNumberAsStr = licenseNumber.ToString();
                    this.CandidateProviderVM.LicenceNumber = licenseNumberAsStr;
                    await this.CandidateProviderService.SetCandidateProviderLicenseNumberAsync(this.CandidateProviderVM);
                }

                var kvProcedureDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
                var kvApplication19 = kvProcedureDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "CIPO_Application19");
                var procedureDoc = await this.ProviderService.GetProcedureDocumentByIdStartedProcedureAndIdDocumentTypeAsync(this.CandidateProviderVM.IdStartedProcedure.Value, kvApplication19.IdKeyValue);
                if (procedureDoc is null)
                {
                    await this.ShowErrorAsync($"Няма създадена заповед за лицензиране на ЦИПО към {this.CandidateProviderVM.ProviderOwner}!");
                    this.SpinnerHide();
                    return;
                }

                var file = await this.LicensingProcedureDocumentCIPOService.GenerateFirstLicensingAsync(this.CandidateProviderVM, procedureDoc);
                await FileUtils.SaveAs(this.JsRuntime, "Licenzia_CIPO_0712.docx", file.ToArray());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
