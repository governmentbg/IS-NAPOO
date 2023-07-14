using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Licensing;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class LicenseIssuing : BlazorBaseComponent
    {
        private bool disableCompleteProcedureBtn = false;

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsLicenceChange { get; set; }

        [Inject]
        public ILicensingProcedureDocumentCPOService LicensingProcedureDocumentCPOService { get; set; }

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

        private async Task ReferenceSpecialitiesBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!this.CandidateProviderVM.CandidateProviderSpecialities.Any())
                {
                    await this.ShowErrorAsync("Към заявлението за лицензиране няма добавени специалности!");
                    this.SpinnerHide();
                    return;
                }

                var file = this.LicensingProcedureDocumentCPOService.GenerateSpecialitiesReference(this.CandidateProviderVM);
                await FileUtils.SaveAs(this.JsRuntime, "Spravka_specialnosti.xlsx", file.ToArray());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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

                        var kvDocApplication18 = await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application19");
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

        private async Task LicensingApplicationBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var kvProcedureDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
                var kvApplication19 = kvProcedureDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Application19");
                var kvTypeApplicationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
                var kvFirstLicenzing = kvTypeApplicationSource.FirstOrDefault(x => x.KeyValueIntCode == "FirstLicenzing");

                var procedureDoc = await this.ProviderService.GetProcedureDocumentByIdStartedProcedureAndIdDocumentTypeAsync(this.CandidateProviderVM.IdStartedProcedure.Value, kvApplication19.IdKeyValue);
                if (procedureDoc is null)
                {
                    await this.ShowErrorAsync($"Няма създадена заповед за лицензиране на ЦПО към {this.CandidateProviderVM.ProviderOwner}!");
                    this.SpinnerHide();
                    return;
                }

                if (this.CandidateProviderVM.IdTypeApplication == kvFirstLicenzing.IdKeyValue)
                {
                    var file = await this.LicensingProcedureDocumentCPOService.GenerateLicensingApplicationFirstLicenseAsync(this.CandidateProviderVM, procedureDoc);
                    await FileUtils.SaveAs(this.JsRuntime, "Obrazec_prilojenie_licenziq (1).docx", file.ToArray());
                }
                else
                {
                    var file = await this.LicensingProcedureDocumentCPOService.GenerateLicensingApplicationLicenseChangeAsync(this.CandidateProviderVM, procedureDoc);
                    await FileUtils.SaveAs(this.JsRuntime, "Obrazec_prilojenie_licenziq (2).docx", file.ToArray());
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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
                    var licenseNumber = await this.LicensingProcedureDocumentCPOService.GenerateLicenseNumberAsync(this.CandidateProviderVM);
                    var licenseNumberAsStr = licenseNumber.ToString();
                    this.CandidateProviderVM.LicenceNumber = licenseNumberAsStr;
                    await this.CandidateProviderService.SetCandidateProviderLicenseNumberAsync(this.CandidateProviderVM);
                }

                var kvProcedureDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
                var kvApplication19 = kvProcedureDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Application19");
                var procedureDoc = await this.ProviderService.GetProcedureDocumentByIdStartedProcedureAndIdDocumentTypeAsync(this.CandidateProviderVM.IdStartedProcedure.Value, kvApplication19.IdKeyValue);
                if (procedureDoc is null || procedureDoc.DS_OFFICIAL_DocNumber == null)
                {
                    await this.ShowErrorAsync($"Няма създадена заповед за лицензиране на ЦПО към {this.CandidateProviderVM.ProviderOwner}!");
                    this.SpinnerHide();
                    return;
                }

                var file = await this.LicensingProcedureDocumentCPOService.GenerateFirstLicensingAsync(this.CandidateProviderVM, procedureDoc);
                await FileUtils.SaveAs(this.JsRuntime, "Obrazec_licenziq_CPO1.docx", file.ToArray());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
