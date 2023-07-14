using Data.Models.Data.ProviderData;
using Data.Models.Data.Request;
using DocuServiceReference;
using DocuWorkService;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ExternalExpertDocumentModal : BlazorBaseComponent
    {
        private ProcedureExternalExpertVM procedureExternalExpertVM = new ProcedureExternalExpertVM();

        private CandidateProviderVM candidateProviderVM = new CandidateProviderVM();

        [Parameter]
        public EventCallback CallBackAfterDocumentSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IProviderService ProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IDocuService DocuService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.procedureExternalExpertVM);
        }

        public void OpenModal(ProcedureExternalExpertVM model, CandidateProviderVM candidate)
        {
            this.procedureExternalExpertVM = model;

            this.candidateProviderVM = candidate;

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task OnRemoveClick()
        {
            if (this.procedureExternalExpertVM.HasUploadedFile)
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");
                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ProcedureExternalExpert>(this.procedureExternalExpertVM.IdProcedureExternalExpert);
                    if (result == 1)
                    {
                        this.procedureExternalExpertVM = await this.ProviderService.GetProcedureExternalExpertByIdAsync(this.procedureExternalExpertVM.IdProcedureExternalExpert);

                        this.StateHasChanged();

                        this.editContext.MarkAsUnmodified();

                        await this.CallBackAfterDocumentSubmit.InvokeAsync();
                    }
                }
            }
        }

        private async Task OnDownloadClick()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ProcedureExternalExpert>(this.procedureExternalExpertVM.IdProcedureExternalExpert);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<ReportUploadedDoc>(this.procedureExternalExpertVM.IdProcedureExternalExpert);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, this.procedureExternalExpertVM.FileName, documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count == 1)
            {
                bool isConfirmed = true;
                if (args.FilesData[0].Name == this.procedureExternalExpertVM.FileName)
                {
                    isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");
                }

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ProcedureExternalExpert>(this.procedureExternalExpertVM.IdProcedureExternalExpert);
                    if (result == 1)
                    {
                        this.procedureExternalExpertVM = await this.ProviderService.GetProcedureExternalExpertByIdAsync(this.procedureExternalExpertVM.IdProcedureExternalExpert);

                        this.StateHasChanged();

                        this.editContext.MarkAsUnmodified();

                        await this.CallBackAfterDocumentSubmit.InvokeAsync();
                    }
                }
            }
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                var fileName = args.Files[0].FileInfo.Name;

                var result = await this.UploadFileService.UploadFileProcedureExternalExpertAsync(args.Files[0].Stream, fileName, "ProcedureExternalExpert", this.procedureExternalExpertVM.IdProcedureExternalExpert);

                this.procedureExternalExpertVM.UploadedFileName = result.ResultContextObject.UploadedFileName;

                this.procedureExternalExpertVM.UploadedFileStream = args.Files[0].Stream;

                this.StateHasChanged();

                this.editContext.MarkAsUnmodified();

                await this.CallBackAfterDocumentSubmit.InvokeAsync();
            }
        }

        private async Task GenerateApplication11()
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
                model.IdStartedProcedure = this.procedureExternalExpertVM.IdStartedProcedure;
                KeyValueVM kvDocTypeApplication11 = new KeyValueVM();
                var kvCIPO = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO");
                if (this.candidateProviderVM.IdTypeLicense == kvCIPO.IdKeyValue)
                {
                    kvDocTypeApplication11 = await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_Application11");
                }
                else
                {
                    kvDocTypeApplication11 = await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application11");
                }

                var application11 = await this.ProviderService.GetProcedureDocumentByIdStartedProcedureByIdDocumentTypeAndByIdExpertAsync(this.procedureExternalExpertVM.IdStartedProcedure, kvDocTypeApplication11.IdKeyValue, this.procedureExternalExpertVM.IdExpert);

                var resultContext = new ResultContext<List<ProcedureDocumentVM>>();
                resultContext.ResultContextObject = new List<ProcedureDocumentVM>();

                if (application11 is null)
                {
                    var doc11 = new ProcedureDocumentVM()
                    {
                        TypeLicensing = kvDocTypeApplication11.KeyValueIntCode == "Application11" ? GlobalConstants.LICENSING_CPO : GlobalConstants.LICENSING_CIPO,
                        IdStartedProcedure = this.procedureExternalExpertVM.IdStartedProcedure,
                        IsValid = true,
                        IdDocumentType = kvDocTypeApplication11.IdKeyValue,
                        IdExpert = procedureExternalExpertVM.IdExpert,
                    };

                    resultContext.ResultContextObject.Add(doc11);

                    resultContext = await this.ProviderService.InsertProcedureDocumentFromListAsync(resultContext);

                    if (resultContext.HasMessages)
                    {
                        await this.ShowSuccessAsync("Успешно създаване на документи!");
                        resultContext.ListMessages.Clear();

                        await this.GenerateAsFileApplication11Async(kvDocTypeApplication11);
                    }
                    else
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                        resultContext.ListErrorMessages.Clear();
                    }
                }
                else
                {
                    await this.GenerateAsFileApplication11Async(kvDocTypeApplication11);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task GenerateAsFileApplication11Async(KeyValueVM kvDocTypeApplication11)
        {
            string guid = string.Empty;
            FileData[] files;
            var procedureDocumentVM = await this.ProviderService.GetProcedureDocumentByIdStartedProcedureByIdDocumentTypeAndByIdExpertAsync(this.procedureExternalExpertVM.IdStartedProcedure, kvDocTypeApplication11.IdKeyValue, this.procedureExternalExpertVM.IdExpert);
            if (procedureDocumentVM.DS_OFFICIAL_ID != null)
            {
                var contextResponse = await this.DocuService.GetDocumentAsync((int)procedureDocumentVM.DS_OFFICIAL_ID, procedureDocumentVM.DS_OFFICIAL_GUID);

                if (contextResponse.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                    return;
                }

                var doc = contextResponse.ResultContextObject;
                files = doc.Doc.File;
                guid = doc.Doc.GUID;
            }
            else
            {
                var contextResponse = await this.DocuService.GetDocumentAsync((int)procedureDocumentVM.DS_ID, procedureDocumentVM.DS_GUID);

                if (contextResponse.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                    return;
                }

                var doc = contextResponse.ResultContextObject;
                files = doc.Doc.File;
                guid = doc.Doc.GUID;
            }

            if (files == null || files.Count() == 0)
            {
                await this.ShowErrorAsync("Няма записани документи");
            }
            else
            {
                foreach (var file in files)
                {
                    var fileResponse = await this.DocuService.GetFileAsync(file.FileID, guid);

                    await FileUtils.SaveAs(JsRuntime, file.Filename, fileResponse.File.BinaryContent.ToArray());
                }
            }
        }

        private async Task sendDoc()
        {
            if (!this.procedureExternalExpertVM.HasUploadedFile)
            {
                await this.ShowErrorAsync("Моля, прикачете файл преди да подадете към НАПОО!");
                return;
            }

            if (loading) return;

            try
            {
                loading = true;

                var indexUser = await DataSourceService.GetSettingByIntCodeAsync("IndexUserId");

                var kvDocTypeApplication11 = await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application11");


                RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                {
                    ExternalCode = kvDocTypeApplication11.DefaultValue2,
                    RegisterUser = Int32.Parse(indexUser.SettingValue),
                    RegisterUserSpecified = true
                };

                CorrespData corresp = new CorrespData()
                {
                    Names = candidateProviderVM.ProviderName,
                    EIK = candidateProviderVM.PoviderBulstat,
                    Phone = candidateProviderVM.ProviderPhone,
                    Email = candidateProviderVM.ProviderEmail
                };

                FileData[] files = new FileData[]
                     {
                            new FileData()
                            {
                            BinaryContent = procedureExternalExpertVM.UploadedFileStream.ToArray(),
                            Filename = procedureExternalExpertVM.UploadedFileName
                            }
                     };

                DocData docs = new DocData()
                {
                    Otnosno = $"Доклад_{procedureExternalExpertVM.Expert.Person.FirstName}_{procedureExternalExpertVM.Expert.Person.FamilyName}",
                    Corresp = corresp,
                    File = files,
                };


                var registerResult = await this.DocuService.RegisterDocumentAsync(registerDocumentParams, docs);

                if (registerResult.HasErrorMessages)
                {
                   await this.ShowErrorAsync(string.Join(Environment.NewLine,registerResult.ListErrorMessages));
                }
            }
            finally
            {
                loading = false;
            }
        }
    }
}
