using System;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.Services.Training;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Compression.Zip;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class ValidationClientDocumentModal : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback CallBackAfterSave { get; set; }

        [Parameter]
        public ValidationClientVM ClientVM { get; set; }

        private ValidationClientRequiredDocumentVM model = new ValidationClientRequiredDocumentVM();
        private ValidationClientVM clientVM = new ValidationClientVM();
        private SfUploader uploader = new SfUploader();
        private IEnumerable<KeyValueVM> requiredDocumentTypesSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> qualificationLevelsSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> formEducationsSource = new List<KeyValueVM>();
        private string CreationDateStr = string.Empty;
        private string ModifyDateStr = string.Empty;
        private List<int> filteredDocIds = new List<int>();
        private List<int> filteredDiplomaDocIds = new List<int>();
        private KeyValueVM kvProfesionalQualificationType = new KeyValueVM();
        private bool showDeleteConfirmDialog;
        private string fileNameForDeletion;

        //        @inject IDataSourceService DataSourceService
        //@inject IApplicationUserService ApplicationUserService
        //@inject ITrainingService TrainingService
        //@inject IUploadFileService UploadFileService
        //@inject IJSRuntime JsRuntime
        //@inject ILocService LocService

       // [Parameter]
        //public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        IDataSourceService DataSourceService { get; set; }
        [Inject]
        IApplicationUserService ApplicationUserService { get; set; }
        [Inject]
        ITrainingService TrainingService { get; set; }
        [Inject]
        IUploadFileService UploadFileService { get; set; }
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        [Inject]
        ILocService LocService { get; set; }
        [Inject]
        ITemplateDocumentService templateDocumentService { get; set; }
        [Inject]
        IDOCService dOCService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.requiredDocumentTypesSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType")).OrderBy(x => x.Order)/*.where(x => x.defaultvalue3.contains("cpo"))*/.ToList();
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Report")!.IdKeyValue);
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "MedicalDocument")!.IdKeyValue);
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Other")!.IdKeyValue);
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "InformationCard")!.IdKeyValue);
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ClientFormular")!.IdKeyValue);
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "SumTable")!.IdKeyValue);
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ComparisonTable")!.IdKeyValue);
            this.filteredDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "PlanGraph")!.IdKeyValue);
            this.filteredDiplomaDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "SecondarySchoolDiploma")!.IdKeyValue);
            this.filteredDiplomaDocIds.Add(this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "UniversityDiploma")!.IdKeyValue);
            this.kvProfesionalQualificationType = this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfesionalQualification");
            this.qualificationLevelsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MinimumQualificationLevel");
            this.formEducationsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education");
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(ValidationClientRequiredDocumentVM model)
        {
            this.clientVM = await this.TrainingService.GetValidationClientByIdAsync(this.ClientVM.IdValidationClient);
            if (this.clientVM.FrameworkProgram != null && this.clientVM.FrameworkProgram.Name.Contains("Е"))
            {
                this.qualificationLevelsSource = this.qualificationLevelsSource.Where(q => q.DefaultValue3.Contains(this.clientVM.FrameworkProgram.Name));
            }
            this.model = model;
            if (model.IdValidationClientRequiredDocument != 0)
            {
                this.CreationDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
            }
            else
            {
                this.CreationDateStr = string.Empty;
                this.ModifyDateStr = string.Empty;
            }

            if ((await this.TrainingService.GetAllValidationRequiredDocumentsByIdClient(this.model.IdValidationClient)).Any(x => x.IdValidationClientRequiredDocument == this.model.IdValidationClientRequiredDocument))
            {
                var temp = (await this.TrainingService.GetAllValidationRequiredDocumentsByIdClient(this.model.IdValidationClient)).First(x => x.IdValidationClientRequiredDocument == this.model.IdValidationClientRequiredDocument);
                var kvTempId = new KeyValueVM();
                if (temp != null)
                {
                    var temp2 = await this.DataSourceService.GetKeyValueByIdAsync(temp.IdCourseRequiredDocumentType);
                    this.model.IdCourseRequiredDocumentType = temp2.IdKeyValue;
                }
            }
            this.editContext = new EditContext(this.model);
            this.isVisible = true;
            this.StateHasChanged();
        }
        public async Task Save()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();

                bool isValid = this.editContext.Validate();
                if (isValid)
                {
                    var result = new ResultContext<ValidationClientRequiredDocumentVM>();
                    result.ResultContextObject = this.model;
                    if (result.ResultContextObject.IdValidationClientRequiredDocument == 0)
                    {
                        result = await this.TrainingService.CreateValidationRequiredDocumentAsync(result);
                    }
                    else
                    {
                        result = await this.TrainingService.UpdateValidationRequiredDocumentAsync(result);
                    }
                    this.model = await this.TrainingService.GetValidationRequiredDocumentById(result.ResultContextObject.IdValidationClientRequiredDocument);

                    this.CreationDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                    this.ModifyDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                    this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                    this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                    }
                    await CallBackAfterSave.InvokeAsync();
                }
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }
        }
        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;

            var result = await this.UploadFileService.UploadFileAsync<ValidationClientRequiredDocument>(file, args.Files[0].FileInfo.Name, "ValidationEducation", this.model.IdValidationClientRequiredDocument);
            if (!string.IsNullOrEmpty(result))
            {
                this.model.UploadedFileName = result;
            }

            await CallBackAfterSave.InvokeAsync();

            await this.uploader.ClearAllAsync();

            this.StateHasChanged();
        }
        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationClientRequiredDocument>(this.model.IdValidationClientRequiredDocument);
                    if (result == 1)
                    {
                        this.model.UploadedFileName = null;
                    }

                    await CallBackAfterSave.InvokeAsync();

                    this.StateHasChanged();
                }
            }
        }
        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
                this.fileNameForDeletion = fileName;
                this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
            }
        }
        public async void ConfirmDeleteCallback()
        {

            var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationClientRequiredDocument>(this.model.IdValidationClientRequiredDocument);
            if (result == 1)
            {
                this.model.UploadedFileName = null;

            }
            await CallBackAfterSave.InvokeAsync();
            this.StateHasChanged();
            this.showDeleteConfirmDialog = false;
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

                if (!string.IsNullOrEmpty(this.model.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationClientRequiredDocument>(this.model.IdValidationClientRequiredDocument);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<ValidationClientRequiredDocument>(this.model.IdValidationClientRequiredDocument);
                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.model.FileName, documentStream.MS!.ToArray());
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
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        private async Task GenerateApplication()
        {
            try
            {
                var docType = await this.DataSourceService.GetKeyValueByIdAsync(this.model.IdCourseRequiredDocumentType);
                if (docType.KeyValueIntCode == "ClientFormular")
                {
                    var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + (await this.templateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application3CandidateFormular")).Result.IdKeyValue).TemplatePath;
                    if (filePath != null)
                    {
                        FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(fileStream, Syncfusion.DocIO.FormatType.Docx);
                        fileStream.Close();
                        var validationClientRequiredDocuments = (await this.TrainingService.GetAllValidationRequiredDocumentsByIdClient(ClientVM.IdValidationClient));

                        string[] fieldNames = new string[]
                        {
                        "ClientFullName",
                        "BirthDate",
                        "Phone",
                        };

                        string[] fieldValues = new string[]
                        {
                        ClientVM.WholeName,
                        ClientVM.BirthDate.Value.ToString(GlobalConstants.DATE_FORMAT),
                        ClientVM.Phone,
                        };
                        document.MailMerge.Execute(fieldNames, fieldValues);

                        MemoryStream stream = new MemoryStream();
                        document.Save(stream, Syncfusion.DocIO.FormatType.Docx);


                        await FileUtils.SaveAs(JsRuntime, $"{BaseHelper.ConvertCyrToLatin(this.ClientVM.FullName)}-Application3.docx", stream.ToArray());
                    }
                    else
                    {
                        this.ShowErrorAsync("Не същестува шаблон за избрания тип документ");
                    }
                }
                else if (docType.KeyValueIntCode == "InformationCard")
                {
                    var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + (await this.templateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application2IC")).Result.IdKeyValue).TemplatePath;
                    if (filePath != null)
                    {
                        FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(fileStream, Syncfusion.DocIO.FormatType.Docx);
                        fileStream.Close();

                        string[] fieldNames = new string[]
                        {
                        "ClientFullName",
                        "ProfessionCode",
                        "ProfessionName",
                        "SpecialityCode",
                        "SpecialityName",
                        "Date",
                        };

                        string[] fieldValues = new string[]
                        {
                        ClientVM.WholeName,
                        ClientVM.Speciality.Profession.Code,
                        ClientVM.Speciality.Profession.Name,
                        ClientVM.Speciality.Code,
                        ClientVM.Speciality.Name,
                        DateTime.Now.Date.ToString(GlobalConstants.DATE_FORMAT)
                        };
                        document.MailMerge.Execute(fieldNames, fieldValues);
                        MemoryStream stream = new MemoryStream();
                        document.Save(stream, Syncfusion.DocIO.FormatType.Docx);

                        await FileUtils.SaveAs(JsRuntime, $"{BaseHelper.ConvertCyrToLatin(this.ClientVM.FullName)}-Application2.docx", stream.ToArray());
                    }
                    else
                    {
                        this.ShowErrorAsync("Не същестува шаблон за избрания тип документ");
                    }
                }
                else if (docType.KeyValueIntCode == "SumTable")
                {
                    var doc = await this.dOCService.GetActiveDocByProfessionIdAsync(new ProfessionVM() { IdProfession = ClientVM.Speciality.IdProfession });
                    var erus = /*doc != null ?*/ (await this.dOCService.GetAllERUsByIdSpecialityAsync(ClientVM.Speciality.IdSpeciality)).ToList() /*: null*/;
                    var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + (await this.templateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application7ComparisonTable")).Result.IdKeyValue).TemplatePath;
                    if (filePath != null)
                    {
                        FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(fileStream, Syncfusion.DocIO.FormatType.Docx);
                        fileStream.Close();

                        if (erus != null)
                        {
                            foreach (WTable table in document.LastSection.Tables)
                            {
                                if (table.Title.Contains("DOITable"))
                                {
                                    document.LastSection.Body.ChildEntities.Remove(table);
                                }
                                if (table.Title.Contains("DOCTable"))
                                {
                                    int rowCount = 0;
                                    foreach (var eru in erus)
                                    {
                                        rowCount++;
                                        var row = table.Rows.LastItem as WTableRow;
                                        row.Cells[0].AddParagraph().AppendText(eru.Code);
                                        row.Cells[1].AddParagraph().AppendText(eru.Name);
                                        if (rowCount < erus.Count)
                                        {
                                            table.AddRow(isCopyFormat: true);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (WTable table in document.LastSection.Tables)
                            {
                                if (table.Title.Contains("DOCTable"))
                                {
                                    document.LastSection.Body.ChildEntities.Remove(table);
                                }
                            }
                        }

                        BookmarksNavigator bookmark = new BookmarksNavigator(document);

                        IWParagraphStyle paragraphStyle = document.AddParagraphStyle("MembersParagraph");
                        paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;

                        IWParagraphStyle SignedParagraphStyle = document.AddParagraphStyle("MembersSignParagraph");
                        SignedParagraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                        SignedParagraphStyle.ParagraphFormat.Tabs.AddTab((float)507, TabJustification.Left, TabLeader.NoLeader);

                        ListStyle membersListStyle = document.AddListStyle(ListType.Numbered, "MembersList");

                        WListLevel memberstListLevel = membersListStyle.Levels[0];
                        memberstListLevel.FollowCharacter = FollowCharacterType.Space;
                        memberstListLevel.CharacterFormat.FontSize = (float)9;
                        memberstListLevel.TextPosition = 1;

                        WCharacterFormat membersCharacterFormat = SetMembersCharacterFormat(document);
                        WCharacterFormat underLineCharactersFormat = SetUnderLineCharactersFormat(document);
                        bookmark.MoveToBookmark("MembersList", true, false);
                        IWParagraph temp = new WParagraph(document);
                        temp.AppendText("\n");
                        for (int i = 0; i < this.ClientVM.ValidationCommissionMembers.Where(x => x.IsChairman != true).ToList().Count; i++)
                        {
                            var member = this.ClientVM.ValidationCommissionMembers.Where(x => x.IsChairman != true).ToList()[i];
                            IWParagraph membersParagraph = new WParagraph(document);

                            membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.......................................\n").ApplyCharacterFormat(membersCharacterFormat);
                            membersParagraph.AppendText("      (име, презиме, фамилия) " + "\t \t(подпис) \n").ApplyCharacterFormat(underLineCharactersFormat);
                            membersParagraph.ApplyStyle("MembersSignParagraph");

                            bookmark.InsertParagraph(membersParagraph);
                        }

                        var chairman = this.ClientVM.ValidationCommissionMembers.ToList().FirstOrDefault(x => x.IsChairman == true);
                        var typeOfcourse = await this.DataSourceService.GetKeyValueByIdAsync(this.ClientVM.IdCourseType);
                        var vqs = await this.DataSourceService.GetKeyValueByIdAsync(this.ClientVM.Speciality.IdVQS);

                        string[] fieldNames = new string[]
                        {
                        "ClientFullName",
                        "ProfessionCode",
                        "ProfessionName",
                        "SpecialityCode",
                        "SpecialityName",
                        "VQS",
                        //"Partial",
                        "DOCorDOI",
                        "Chairman",
                        "Date",
                        };

                        string[] fieldValues = new string[]
                        {
                        ClientVM.WholeName,
                        ClientVM.Speciality.Profession.Code,
                        ClientVM.Speciality.Profession.Name,
                        ClientVM.Speciality.Code,
                        ClientVM.Speciality.Name,
                        vqs.Name,
                        //"..............."/*(typeOfcourse.KeyValueIntCode == "PartProfession") ? vqs.Name : "...................................."*/,
                        doc != null ? ($"Държавен образователен стандарт за придобиване на квалификация по професия (ДОС), утвърдено с {doc.Regulation} от {doc.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г. ")  : ("Държавно образователно изискване за  придобиване на квалификация по професия (ДОИ),утвърдено с Наредба № ..................... от ................ г."),
                        chairman != null ? chairman.FullName : "",
                        DateTime.Now.Date.ToString(GlobalConstants.DATE_FORMAT)
                        };
                        document.MailMerge.Execute(fieldNames, fieldValues);
                        MemoryStream stream = new MemoryStream();
                        document.Save(stream, Syncfusion.DocIO.FormatType.Docx);

                        await FileUtils.SaveAs(JsRuntime, $"{BaseHelper.ConvertCyrToLatin(this.ClientVM.FullName)}-Application7.docx", stream.ToArray());
                    }
                    else
                    {
                        this.ShowErrorAsync("Не същестува шаблон за избрания тип документ");
                    }
                }
                else if (docType.KeyValueIntCode == "ComparisonTable")
                {
                    if (this.ClientVM.IdSpeciality != null)
                    {
                        var doc = await this.dOCService.GetActiveDocByProfessionIdAsync(new ProfessionVM() { IdProfession = ClientVM.Speciality.IdProfession });
                        var erus = /*doc != null ?*/ (await this.dOCService.GetAllERUsByIdSpecialityAsync(ClientVM.Speciality.IdSpeciality)).ToList() /*: null*/;
                        var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + (await this.templateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application4Table")).Result.IdKeyValue).TemplatePath;
                        if (filePath != null)
                        {
                            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                            Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(fileStream, Syncfusion.DocIO.FormatType.Docx);
                            fileStream.Close();

                            Syncfusion.DocIO.DLS.IWTable tableTemplate = new Syncfusion.DocIO.DLS.WTable(document);

                            foreach (Syncfusion.DocIO.DLS.IWTable table in document.Sections[0].Tables)
                            {
                                if (table != null && table.Title != null)
                                {
                                    if (table.Title.Contains("ERUTable"))
                                    {
                                        tableTemplate = table;
                                        document.LastSection.Body.ChildEntities.Remove(table);
                                    }
                                    if (doc != null)
                                    {
                                        if (table.Title.Contains("DOITable"))
                                        {
                                            document.LastSection.Body.ChildEntities.Remove(table);
                                        }

                                    }
                                }
                            }

                            BookmarksNavigator bookmark = new BookmarksNavigator(document);
                            bookmark.MoveToBookmark("InsertTable", true, false);

                            if (doc != null)
                            {
                                foreach (var eru in erus)
                                {
                                    bookmark.InsertTable(tableTemplate.Clone() as WTable);
                                    foreach (WParagraph paragraph in document.LastSection.Tables[document.LastSection.Tables.Count - 2].Rows[0].Cells[0].Paragraphs)
                                    {
                                        if (paragraph.Text == "ERUNAME")
                                        {
                                            paragraph.Text = eru.CodeWithName;
                                        }
                                    }
                                    bookmark.InsertParagraph(new WParagraph(document));
                                }
                            }


                            IWParagraphStyle paragraphStyle = document.AddParagraphStyle("MembersParagraph");
                            paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;

                            IWParagraphStyle SignedParagraphStyle = document.AddParagraphStyle("MembersSignParagraph");
                            SignedParagraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                            SignedParagraphStyle.ParagraphFormat.Tabs.AddTab((float)507, TabJustification.Left, TabLeader.NoLeader);

                            ListStyle membersListStyle = document.AddListStyle(ListType.Numbered, "MembersList");

                            WListLevel memberstListLevel = membersListStyle.Levels[0];
                            memberstListLevel.FollowCharacter = FollowCharacterType.Space;
                            memberstListLevel.CharacterFormat.FontSize = (float)9;
                            memberstListLevel.TextPosition = 1;

                            WCharacterFormat membersCharacterFormat = SetMembersCharacterFormat(document);
                            WCharacterFormat underLineCharactersFormat = SetUnderLineCharactersFormat(document);
                            bookmark.MoveToBookmark("MembersList", true, false);
                            IWParagraph temp = new WParagraph(document);
                            temp.AppendText("\n");
                            for (int i = 0; i < this.ClientVM.ValidationCommissionMembers.Where(x => x.IsChairman != true).ToList().Count; i++)
                            {
                                var member = this.ClientVM.ValidationCommissionMembers.Where(x => x.IsChairman != true).ToList()[i];
                                IWParagraph membersParagraph = new WParagraph(document);

                                membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.......................................\n").ApplyCharacterFormat(membersCharacterFormat);
                                membersParagraph.AppendText("      (име, презиме, фамилия) " + "\t \t(подпис) \n").ApplyCharacterFormat(underLineCharactersFormat);
                                membersParagraph.ApplyStyle("MembersSignParagraph");

                                bookmark.InsertParagraph(membersParagraph);
                            }

                            var chairman = this.ClientVM.ValidationCommissionMembers.ToList().FirstOrDefault(x => x.IsChairman == true);
                            var typeOfcourse = await this.DataSourceService.GetKeyValueByIdAsync(this.ClientVM.IdCourseType);
                            var vqs = await this.DataSourceService.GetKeyValueByIdAsync(this.ClientVM.Speciality.IdVQS);

                            string[] fieldNames = new string[]
                            {
                        "ClientFullName",
                        "ProfessionCode",
                        "ProfessionName",
                        "SpecialityCode",
                        "SpecialityName",
                        "VQS",
                        "Partial",
                        "DOCorDOI",
                        "Chairman",
                        "Date",
                            };

                            string[] fieldValues = new string[]
                            {
                        ClientVM.WholeName,
                        ClientVM.Speciality.Profession.Code,
                        ClientVM.Speciality.Profession.Name,
                        ClientVM.Speciality.Code,
                        ClientVM.Speciality.Name,
                        vqs.Name,
                        "..............."/*(typeOfcourse.KeyValueIntCode == "PartProfession") ? vqs.Name : "...................................."*/,
                        doc != null ? ($"Държавен образователен стандарт за придобиване на квалификация по професия (ДОС), утвърдено с {doc.Regulation} от {doc.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г. ")  : ("Държавно образователно изискване за  придобиване на квалификация по професия (ДОИ),утвърдено с Наредба № ..................... от ................ г."),
                        chairman is not null ? chairman.FullName : string.Empty,
                        DateTime.Now.Date.ToString(GlobalConstants.DATE_FORMAT)
                            };
                            document.MailMerge.Execute(fieldNames, fieldValues);
                            MemoryStream stream = new MemoryStream();
                            document.Save(stream, Syncfusion.DocIO.FormatType.Docx);

                            await FileUtils.SaveAs(JsRuntime, $"{BaseHelper.ConvertCyrToLatin(this.ClientVM.FullName)}-Application4.docx", stream.ToArray());
                        }
                        else
                        {
                            this.ShowErrorAsync("Не същестува шаблон за избрания тип документ");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private static WCharacterFormat SetMembersCharacterFormat(WordDocument document)
        {
            WCharacterFormat membersCharFormat = new WCharacterFormat(document);
            membersCharFormat.FontName = "Calibri";
            membersCharFormat.FontSize = (float)11;
            membersCharFormat.Position = 0;
            membersCharFormat.Italic = false;
            membersCharFormat.Bold = false;

            return membersCharFormat;
        }
        private static WCharacterFormat SetUnderLineCharactersFormat(WordDocument document)
        {
            WCharacterFormat membersCharFormat = new WCharacterFormat(document);
            membersCharFormat.FontName = "Calibri";
            membersCharFormat.FontSize = (float)9;
            membersCharFormat.Position = 0;
            membersCharFormat.Italic = false;
            membersCharFormat.Bold = false;

            return membersCharFormat;

        }
    }
}
