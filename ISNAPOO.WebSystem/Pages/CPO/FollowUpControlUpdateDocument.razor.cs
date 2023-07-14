using Data.Models.Data.Control;
using DocuServiceReference;
using DocuWorkService;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;

namespace ISNAPOO.WebSystem.Pages.CPO
{
    partial class FollowUpControlUpdateDocument : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback CallBackAfterUploadFile { get; set; }

        #region Inject
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDocuService DocuService { get; set; }

        [Inject]
        public ITemplateDocumentService TemplateDocumentService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IControlService ControlService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }
        #endregion

        private ToastMsg toast;
        private SfDialog sfDialog = new SfDialog();
        private FollowUpControlDocumentVM model = new FollowUpControlDocumentVM();
        private FollowUpControlVM followUpControl = new FollowUpControlVM();
        private List<MemoryStream> fileStreams = null;
        private string fileName = string.Empty;
        private List<string> fileNames = new List<string>();
        private string CPOorCIPO = "ЦПО";
        private string NameAndOwner = string.Empty;
        private string documentType = string.Empty;
        private string lastDocumentType = string.Empty;
        private string AboutName = string.Empty;
        private string FolderPathName = string.Empty;
        private bool isBlankGenerated = false;

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(FollowUpControlDocumentVM _model, string licensingType)
        {
            this.model = _model;
            this.CPOorCIPO = licensingType;
            if (this.model.DS_OFFICIAL_ID.HasValue)
            {
                isBlankGenerated = true;
            }
            else
            {
                isBlankGenerated = false;
            }
            this.followUpControl = await this.ControlService.GetControlByIdFollowUpControlAsync(_model.IdFollowUpControl);
            if (CPOorCIPO == "ЦПО")
            {
                this.NameAndOwner = followUpControl.CandidateProvider.CPONameOwnerGrid;
                this.documentType = "CPOBlankReportPK";
                this.lastDocumentType = "CPO_ApplicationPK2"; // Констативен протокол от извършен последващ контрол
                this.AboutName = "Доклад на ЦПО за изпълнени препоръки от последващ контрол";
                this.FolderPathName = "CPOFollowUpControlDocuments";
            }
            else
            {
                this.NameAndOwner = followUpControl.CandidateProvider.CIPONameOwnerGrid;
                this.documentType = "CIPOBlankReportPK";
                this.lastDocumentType = "CIPO_ApplicationPK2"; // Констативен протокол от извършен последващ контрол
                this.AboutName = "Доклад на ЦИПО за изпълнени препоръки от последващ контрол";
                this.FolderPathName = "CIPOFollowUpControlDocuments";
            }
            await this.SetFileNameAsync();
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Save()
        {
            string msg = "Сигурни ли си сте, че искате да изпратите прикачения файл към НАПОО?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
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

                    if (this.model.HasUploadedFile)
                    {
                        if (this.model.IdFollowUpControlDocument != 0)
                        {
                            if (!this.model.DS_OFFICIAL_ID.HasValue)
                            {
                                if (fileStreams == null)
                                {
                                    this.fileStreams = new List<MemoryStream>();
                                    var names = this.model.FileName.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    foreach (var file in names)
                                    {
                                        var fileName = file.Split(($"\\{this.model.IdFollowUpControlDocument}\\"), StringSplitOptions.RemoveEmptyEntries).ToArray().LastOrDefault();
                                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<FollowUpControlDocument>(model.IdFollowUpControlDocument, fileName);
                                        this.fileStreams.Add(documentStream.MS!);
                                        this.fileNames.Add(fileName);
                                    }
                                }
                                FileData[] files = new FileData[fileStreams.Count];
                                for (int i = 0; i < files.Length; i++)
                                {
                                    files[i] = new FileData()
                                    {
                                        BinaryContent = fileStreams[i].ToArray(),
                                        Filename = fileNames[i]
                                    };
                                }
                                var kvDocumentType = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", documentType);
                                var kvDocumentType1 = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", lastDocumentType);
                                var indexUser = await DataSourceService.GetSettingByIntCodeAsync("IndexUserId");
                                RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                                {
                                    ExternalCode = kvDocumentType.DefaultValue2,
                                    RegisterUser = int.Parse(indexUser.SettingValue),
                                    RegisterUserSpecified = true
                                };

                                CorrespData corresp = new CorrespData()
                                {
                                    Names = this.NameAndOwner,
                                    EIK = this.followUpControl.CandidateProvider.PoviderBulstat,
                                    Phone = this.followUpControl.CandidateProvider.ProviderPhone,
                                    Email = this.followUpControl.CandidateProvider.ProviderEmail
                                };

                                DocData docs = new DocData()
                                {
                                    Otnosno = AboutName,
                                    Corresp = corresp,
                                    File = files,

                                };
                                var registerResult = await this.DocuService.RegisterDocumentAsync(registerDocumentParams, docs);

                                if (registerResult.HasErrorMessages)
                                {
                                    await this.ShowErrorAsync(string.Join(Environment.NewLine, registerResult.ListErrorMessages));
                                    return;
                                }
                                var documentResponse = registerResult.ResultContextObject;

                                this.model.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
                                this.model.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
                                this.model.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
                                this.model.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;


                                var result = await this.ControlService.UpdateControlDocument(this.model);
                                await this.ShowSuccessAsync("Докладът е изпратен успешно!");
                                await this.CallBackAfterUploadFile.InvokeAsync();
                                this.isVisible = false;
                                this.StateHasChanged();
                            }
                            else
                            {
                                await this.ShowErrorAsync("Вече е изпратен доклад към НАПОО!");
                            }
                        }
                        else
                        {
                            await this.ShowErrorAsync("Няма запис в БД!");
                        }
                    }
                    else
                    {
                        await this.ShowErrorAsync("Няма прикачен файл!");
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count >= 1)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    await this.SetNameForFilesWithSameFileNameAsync(args);

                    this.fileNames = args.Files.Select(x => $"{x.FileInfo.Name} ").ToList();
                    this.fileName = string.Join(Environment.NewLine, fileNames).Trim();
                    this.fileStreams = args.Files.Select(x => x.Stream).ToList();

                    var result = await this.UploadFileService.UploadFileFollowUpControlAdditionalDocumentAsync(this.fileStreams.ToArray(), this.fileName, this.FolderPathName, this.model.IdFollowUpControlDocument);

                    this.model = await this.ControlService.GetFollowUpControlDocumentByIdAsync(new FollowUpControlDocumentVM() { IdFollowUpControlDocument = this.model.IdFollowUpControlDocument });

                    await this.SetFileNameAsync();

                    this.StateHasChanged();

                    this.editContext = new EditContext(this.model);

                    await this.CallBackAfterUploadFile.InvokeAsync();
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task SetNameForFilesWithSameFileNameAsync(UploadChangeEventArgs args)
        {
            var settingFolder = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            foreach (var file in args.Files)
            {
                if (!string.IsNullOrEmpty(this.model.UploadedFileName))
                {
                    var path = settingFolder + "\\" + this.model.UploadedFileName;
                    var files = Directory.GetFiles(path);
                    if (files.Any(x => x.Contains(file.FileInfo.Name)))
                    {
                        var fileNameSplitted = file.FileInfo.Name.Split($".{file.FileInfo.Type}");

                        file.FileInfo.Name = fileNameSplitted.FirstOrDefault() + DateTime.Now.ToString("ddMMyyyyHHmmss") + "." + file.FileInfo.Type;
                    }
                }
            }
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (args.FilesData.Count == 1)
            {
                if (this.model.IdFollowUpControlDocument > 0)
                {
                    bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
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

                            var result = await this.UploadFileService.RemoveFileByIdAndFileNameAsync<FollowUpControlDocument>(this.model.IdFollowUpControlDocument, args.FilesData[0].Name);

                            this.model = await this.ControlService.GetFollowUpControlDocumentByIdAsync(this.model);

                            await this.SetFileNameAsync();

                            await this.CallBackAfterUploadFile.InvokeAsync();

                            this.StateHasChanged();
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

        private async Task OnRemove(string fileName)
        {
            if (this.model.IdFollowUpControlDocument > 0)
            {
                if (this.model.IdFollowUpControlDocument > 0)
                {
                    bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
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

                            var result = await this.UploadFileService.RemoveFileByIdAndFileNameAsync<FollowUpControlDocument>(this.model.IdFollowUpControlDocument, fileName);

                            this.model = await this.ControlService.GetFollowUpControlDocumentByIdAsync(this.model);

                            await this.SetFileNameAsync();

                            await this.CallBackAfterUploadFile.InvokeAsync();

                            this.StateHasChanged();
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

        private async Task OnDownloadClick(string fileName)
        {
            var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<FollowUpControlDocument>(model.IdFollowUpControlDocument, fileName);
            if (hasFile)
            {
                var documentStream = await this.UploadFileService.GetUploadedFileAsync<FollowUpControlDocument>(model.IdFollowUpControlDocument, fileName);

                if (documentStream.MS is not null)
                {
                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, fileName, documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            else
            {
                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                await this.ShowErrorAsync(msg);
            }
        }

        private async Task SetFileNameAsync()
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + this.model.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                this.model.FileName = files.Count() != 0 && !int.TryParse(files[0], out int test)
                    ? string.Join(Environment.NewLine, files)
                    : string.Empty;
            }
        }

        private async Task GenerateReport()
        {
            FollowUpControlDocumentVM followUpControlDocumentVM = new FollowUpControlDocumentVM();
            //Get resource document

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
            string documentName = string.Empty;
            documentName = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", this.documentType)).Result.IdKeyValue).TemplatePath;
            FileStream template = new FileStream($@"{resources_Folder}{documentName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);
            DocIORenderer render = new DocIORenderer();
            var kvDocumentType = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", this.documentType);
            var kvDocumentType1 = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", this.lastDocumentType);
            var documentBlank = (await this.ControlService.GetAllDocumentsAsync(this.model.IdFollowUpControl)).FirstOrDefault(d => d.IdDocumentType == kvDocumentType1.IdKeyValue);

            //Merge fields
            string[] fieldNames = new string[]
            {
                "DirectorFullName",
                "CPOorCIPO",
                "LocationCorrespondence",
                "LicenceNumber",
                "LicenceDate",
                "Official_number",
                "Official_date"

            };
            string[] fieldValues = new string[]
            {
                 this.followUpControl.CandidateProvider.DirectorFullName,
                 this.NameAndOwner,
                 this.followUpControl.CandidateProvider.LocationCorrespondence != null ? this.followUpControl.CandidateProvider.LocationCorrespondence.LocationName : string.Empty,
                 this.followUpControl.CandidateProvider.LicenceNumber,
                 this.followUpControl.CandidateProvider.LicenceDate.HasValue ? this.followUpControl.CandidateProvider.LicenceDate.Value.ToString("dd.MM.yyyy") : string.Empty,
                 documentBlank != null ? documentBlank.DS_OFFICIAL_DocNumber : string.Empty,
                 documentBlank != null && documentBlank.DS_OFFICIAL_DATE.HasValue ? documentBlank.DS_OFFICIAL_DATE.Value.ToString("dd.MM.yyyy") : string.Empty
            };

            document.MailMerge.Execute(fieldNames, fieldValues);


            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, FormatType.Docx);
                if (this.model.IdFollowUpControlDocument == 0)
                {
                    followUpControlDocumentVM.IdFollowUpControl = this.model.IdFollowUpControl;
                    followUpControlDocumentVM.IdDocumentType = kvDocumentType.IdKeyValue;
                    followUpControlDocumentVM.UploadedFileName = string.Empty;

                    this.model.IdFollowUpControlDocument = await this.ControlService.SaveControlDocument(followUpControlDocumentVM);
                }
                if (this.CPOorCIPO == "ЦПО")
                {
                    await this.JsRuntime.SaveAs("DokladCPO_preporuki_PK" + ".docx", stream.ToArray());
                }
                else
                {
                    await this.JsRuntime.SaveAs("DokladCIPO_preporuki_PK" + ".docx", stream.ToArray());

                }
                this.isBlankGenerated = true;
            }
        }
    }
}
