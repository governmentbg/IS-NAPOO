using System.Text.RegularExpressions;
using Data.Models.Data.Training;
using DocuWorkService;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationClientDataModel : BlazorBaseComponent
    {
        private SfAutoComplete<int?, LocationVM> sfAutoCompleteLocation = new SfAutoComplete<int?, LocationVM>();

        private List<KeyValueVM> IndentTypeSource;
        private List<KeyValueVM> SexSource;

        private List<LocationVM> LocationSource = new List<LocationVM>();
        IEnumerable<KeyValueVM> kvIndentTypeSource;
        private ValidationMessageStore? messageStore;
        public List<string> validationMessages = new List<string>();
        private List<KeyValueVM> kvNationalitySource = new List<KeyValueVM>();

        private KeyValueVM kvBGNationality;
        private KeyValueVM kvEGN;
        private KeyValueVM kvLNCh;
        private KeyValueVM kvNotificationValidation;
        private bool ShouldReplace;

        public string identType = "ЕГН";
        private SfUploader sfUploader;
        private MemoryStream file;
        private string FileName;
        private bool showDeleteConfirmDialog;
        private string fileNameForDeletion;
        private string documentLabel = string.Empty;

        [Parameter]
        public ValidationClientVM ClientVM { get; set; }

        [Inject]
        IDataSourceService dataSourceService { get; set; }
        [Inject]
        ILocationService locationService { get; set; }
        [Inject]
        ITrainingService trainingService { get; set; }
        [Inject]
        IUploadFileService uploadFileService { get; set; }
        [Inject]
        IJSRuntime jsRuntime { get; set; }
        [Inject]
        ILocService locService { get; set; }
        [Inject]
        IDocuService docuService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Parameter]
        public EventCallback<int?> CallbackAfterSubmit { get; set; }
        [Parameter]
        public bool IsSPK { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = true;
        protected override async Task OnInitializedAsync()
        {
            await HandleOrderForNationalitiesSourceAsync();
            this.editContext = new EditContext(ClientVM);
            this.ShouldReplace = false;
            if (ClientVM.IdValidationClient == 0)
            {
                ClientVM.IdNationality = this.kvBGNationality.IdKeyValue;
                ClientVM.IdCountryOfBirth = this.kvBGNationality.IdKeyValue;
            }
            else
            {
                await LoadModelDataAsync();
            }

            this.kvNotificationValidation = await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "NotificationForValidationClient");


            this.SexSource = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex")).ToList();

            //NationalitySource = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality")).ToList();

            this.IndentTypeSource = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType")).ToList();

            if (ClientVM.IdCityOfBirth != null)
            {
                this.LocationSource.Clear();
                LocationVM location = await locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(ClientVM.IdCityOfBirth.Value);
                this.LocationSource.Add(location);
            }

            this.kvIndentTypeSource = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");

            if (ClientVM.IdValidationClient == 0)
            {
                ClientVM.IdIndentType = this.kvEGN.IdKeyValue;
            }

            if (ClientVM.IdIndentType.HasValue)
            {
                var ident = this.kvIndentTypeSource.FirstOrDefault(x => x.IdKeyValue == ClientVM.IdIndentType.Value);
                if (ident is not null)
                {
                    this.identType = ident.Name;
                }
            }

            if (IsSPK)
                documentLabel = "държавен изпит";
            else
                documentLabel = "изпит";

            this.isVisible = true;
            StateHasChanged();
        }

        public async Task LoadModelDataAsync()
        {
            ClientVM = await trainingService.GetValidationClientByIdAsync(ClientVM.IdValidationClient);
        }

        private async Task HandleOrderForNationalitiesSourceAsync()
        {
            this.kvNationalitySource = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality")).OrderBy(x => x.Name).ToList();
            var withoutNacionality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "Без гражданство");
            this.kvNationalitySource.Remove(withoutNacionality);
            this.kvNationalitySource.Add(withoutNacionality);
            var bgNationality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "България");
            this.kvNationalitySource.Remove(bgNationality);
            this.kvNationalitySource.Insert(0, bgNationality);
            this.kvNationalitySource.RemoveAll(x => x.Name == "");

            this.kvBGNationality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "България");
            this.kvEGN = await dataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvLNCh = await dataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");

        }

        private async Task OnFilterLocationCorrespondence(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    var locationSource = (await locationService.GetAllLocationsByKatiAsync(args.Text)).ToList();


                    this.LocationSource = locationSource;

                    var query = new Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteLocation.FilterAsync(this.LocationSource, query);
                }
                catch (Exception)
                {

                }
            }
        }

        public async Task Submit()
        {
            this.validationMessages.Clear();
            this.editContext = new EditContext(ClientVM);

            this.editContext.OnValidationRequested += CheckRequiredFildsAndValidateEGN;
            this.messageStore = new ValidationMessageStore(this.editContext);

            try
            {
                bool isValid = this.editContext.Validate();
                this.validationMessages.AddRange(this.editContext.GetValidationMessages());

                if (isValid)
                {
                    ClientVM.IdCandidateProvider = UserProps.IdCandidateProvider;
                    if (ClientVM.IdValidationClient == 0)
                        ClientVM = await trainingService.CreateValidationClient(ClientVM);
                    else
                    {
                        await trainingService.UpdateValidationClientAsync(ClientVM);
                    }

                }
               await this.CallbackAfterSubmit.InvokeAsync(ClientVM.IdValidationClient);
            }
            catch (Exception) { }
        }

        private void IndentChanged(ChangeEventArgs args)
        {
            var indent = ClientVM.Indent;
            if (indent != null)
            {
                if (ClientVM.IdIndentType == this.kvEGN.IdKeyValue)
                {
                    indent = indent.Trim();

                    var checkEGN = new BasicEGNValidation(indent);

                    if (checkEGN.Validate())
                    {
                        char charLastDigit = indent[indent.Length - 2];
                        int lastDigit = Convert.ToInt32(new string(charLastDigit, 1));
                        int year = int.Parse(indent.Substring(0, 2));
                        int month = int.Parse(indent.Substring(2, 2));
                        int day = int.Parse(indent.Substring(4, 2));
                        if (month < 13)
                        {
                            year += 1900;
                        }
                        else if (month > 20 && month < 33)
                        {
                            year += 1800;
                            month -= 20;
                        }
                        else if (month > 40 && month < 53)
                        {
                            year += 2000;
                            month -= 40;
                        }
                        var BirthDate = new DateTime(year, month, day);

                        ClientVM.BirthDate = BirthDate;

                        var beforeLastNumber = int.Parse(indent.Substring(indent.Length - 2, 1));

                        if (beforeLastNumber % 2 == 0)
                        {
                            ClientVM.IdSex = this.SexSource.FirstOrDefault(x => x.Name == "Мъж").IdKeyValue;
                        }
                        else
                        {
                            ClientVM.IdSex = this.SexSource.FirstOrDefault(x => x.Name == "Жена").IdKeyValue;
                        }
                    }
                    else
                    {
                        ClientVM.BirthDate = null;
                        ClientVM.IdSex = null;
                    }
                }

            }
        }
        private async Task CheckForExistingClientAsync()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var result = await this.trainingService.GetClientByIdIndentTypeByIndentAndByIdCandidateProviderAsync(this.ClientVM.IdIndentType.Value, this.ClientVM.Indent, this.ClientVM.IdCandidateProvider);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    var modelFromDb = result.ResultContextObject;
                    this.ClientVM.FirstName = modelFromDb.FirstName;
                    this.ClientVM.SecondName = modelFromDb.SecondName;
                    this.ClientVM.FamilyName = modelFromDb.FamilyName;
                    this.ClientVM.IdSex = modelFromDb.IdSex;
                    this.ClientVM.IdIndentType = modelFromDb.IdIndentType;
                    this.ClientVM.Indent = modelFromDb.Indent;
                    this.ClientVM.BirthDate = modelFromDb.BirthDate;
                    this.ClientVM.IdNationality = modelFromDb.IdNationality;
                    this.ClientVM.IdCountryOfBirth = modelFromDb.IdCountryOfBirth;
                    this.ClientVM.IdCityOfBirth = modelFromDb.IdCityOfBirth;

                    this.LocationSource.Clear();

                    if (this.ClientVM.IdCityOfBirth is not null)
                    {
                        LocationVM location = await this.locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.ClientVM.IdCityOfBirth.Value);
                        this.LocationSource.Add(location);
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        private void CheckRequiredFildsAndValidateEGN(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();
            var kvEGN = this.kvIndentTypeSource.FirstOrDefault(kv => kv.KeyValueIntCode == "EGN");
            if (ClientVM.Indent != null)
                ClientVM.Indent = ClientVM.Indent.Trim();
            Regex regexCyrillic = new Regex(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+");

            if (string.IsNullOrEmpty(ClientVM.SecondName) && ClientVM.IdCountryOfBirth == this.kvBGNationality.IdKeyValue)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "SecondName");
                this.messageStore?.Add(fi, "Полето 'Презиме' е задължително!");
            }
            else if (!string.IsNullOrEmpty(ClientVM.SecondName))
            {
                Match matchSecondName = regexCyrillic.Match(ClientVM.SecondName);
                if (!matchSecondName.Success)
                {
                    FieldIdentifier fi = new FieldIdentifier(ClientVM, "SecondName");
                    this.messageStore?.Add(fi, "Полето 'Презиме' може да съдържа само текст на български език!");
                }
            }
            if (string.IsNullOrEmpty(ClientVM.FirstName))
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "FirstName");
                this.messageStore?.Add(fi, "Полето 'Име' е задължително!");
            }
            else
            {
                Match matchFirstName = regexCyrillic.Match(ClientVM.FirstName);
                if (!matchFirstName.Success)
                {
                    FieldIdentifier fi = new FieldIdentifier(ClientVM, "FirstName");
                    this.messageStore?.Add(fi, "Полето 'Име' може да съдържа само текст на български език!");
                }
            }
            if (string.IsNullOrEmpty(ClientVM.FamilyName))
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "FamilyName");
                this.messageStore?.Add(fi, "Полето 'Фамилия' е задължително!");
            }
            else
            {
                Match matchFamilyName = regexCyrillic.Match(ClientVM.FamilyName);
                if (!matchFamilyName.Success)
                {
                    FieldIdentifier fi = new FieldIdentifier(ClientVM, "FamilyName");
                    this.messageStore?.Add(fi, "Полето 'Фамилия' може да съдържа само текст на български език!");
                }
            }
            if (ClientVM.Cost == null)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "Cost");
                this.messageStore?.Add(fi, "Полето 'Цена' е задължително!");
            }
            if (ClientVM.IdSex == null)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "IdSex");
                this.messageStore?.Add(fi, "Полето 'Пол' е задължително!");
            }
            if (ClientVM.IdIndentType == null)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "IdIndentType");
                this.messageStore?.Add(fi, "Полето 'Вид на идентификатора' е задължително!");
            }
            if (string.IsNullOrEmpty(ClientVM.Indent))
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "Indent");
                this.messageStore?.Add(fi, "Полето 'ЕГН/ЛНЧ/ИДН' е задължително!");
            }
            if (ClientVM.BirthDate == null)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "BirthDate");
                this.messageStore?.Add(fi, "Полето 'Рождена дата' е задължително!");
            }
            if (ClientVM.IdNationality == null)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "IdNationality");
                this.messageStore?.Add(fi, "Полето 'Националност' е задължително!");
            }
            if (ClientVM.IdCountryOfBirth == null)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "IdCountryOfBirth");
                this.messageStore?.Add(fi, "Полето 'Държава' е задължително!");
            }
            if (ClientVM.IdCountryOfBirth == this.kvBGNationality.IdKeyValue && ClientVM.IdCityOfBirth == null)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "IdCityOfBirth");
                this.messageStore?.Add(fi, "Полето 'Месторождение (населено място)' е задължително!");
            }
            if (ClientVM.StartDate == null)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "StartDate");
                this.messageStore?.Add(fi, "Полето 'Дата на започване на процедурата' е задължително!");
            }
            if (ClientVM.EndDate == null)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "EndDate");
                this.messageStore?.Add(fi, "Полето 'Дата на приключване на процедурата' е задължително!");
            }
            if (ClientVM.StartDate > ClientVM.EndDate)
            {
                FieldIdentifier fi = new FieldIdentifier(ClientVM, "EndDate");
                this.messageStore?.Add(fi, "Не може 'Дата на започване на процедурата' да е по-голяма от 'Дата на приключване на процедурата'!");
            }
            //if (this.ClientVM.ExamTheoryDate == null)
            //{
            //    FieldIdentifier fi = new FieldIdentifier(this.ClientVM, "ExamTheoryDate");
            //    this.messageStore?.Add(fi, "Полето 'Теоретичен тест' е задължително!");
            //}
            //if (this.ClientVM.ExamPracticeDate == null)
            //{
            //    FieldIdentifier fi = new FieldIdentifier(this.ClientVM, "ExamPracticeDate");
            //    this.messageStore?.Add(fi, "Полето 'Практически тест' е задължително!");
            //}
            if (!string.IsNullOrEmpty(this.ClientVM.EmailAddress))
            {
                var pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                if (!(Regex.IsMatch(this.ClientVM.EmailAddress, pattern)))
                {
                    this.validationMessages.Add("Невалиден E-mail адрес!");
                }
            }
            if (kvEGN.IdKeyValue == ClientVM.IdIndentType)
            {
                if (ClientVM.Indent != null)
                {
                    ClientVM.Indent = ClientVM.Indent;

                    if (ClientVM.Indent.Length > 10)
                    {
                        FieldIdentifier fi = new FieldIdentifier(ClientVM, "Indent");
                        this.messageStore?.Add(fi, "Полето 'ЕГН/ЛНЧ/ИДН' трябва да съдържа 10 символа!");
                    }
                    else
                    {
                        var checkEGN = new BasicEGNValidation(ClientVM.Indent);

                        if (!checkEGN.Validate())
                        {
                            FieldIdentifier fi = new FieldIdentifier(ClientVM, "Indent");
                            this.messageStore?.Add(fi, checkEGN.ErrorMessage);
                        }
                    }
                }
            }
        }

        private void IdentValueChangedHandler(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.Value.HasValue)
            {
                if (args.Value == this.kvEGN.IdKeyValue)
                {
                    this.identType = "ЕГН";
                }
                else if (args.Value == this.kvLNCh.IdKeyValue)
                {
                    this.identType = "ЛНЧ";
                }
                else
                {
                    this.identType = "ИДН";
                }
            }
            else
            {
                this.identType = "ЕГН";
            }
        }
        private async Task UploadFile()
        {
            var result = await uploadFileService.UploadFileAsync<ValidationClient>(this.file, this.FileName, "ValidationClient", this.ClientVM.IdValidationClient);
            if (!string.IsNullOrEmpty(result))
            {
                this.ClientVM.UploadedFileName = result;
            }

            await this.sfUploader.ClearAllAsync();
            await Submit();
            await LoadModelDataAsync();

            this.file = new MemoryStream();

            await this.ShowSuccessAsync("Записът е успешен!");
            StateHasChanged();
        }
        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;
            this.file = file;
            this.FileName = args.Files[0].FileInfo.Name;
            if (string.IsNullOrEmpty(ClientVM.UploadedFileName))
            {
                await UploadFile();
            }
            else
            {
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да замените документа?");
                if (isConfirmed)
                {
                    await SaveDoc();
                    StateHasChanged();
                }                 
            }
        }

        private async Task SaveDoc()
        {
            await UploadFile();
            await CallbackAfterSubmit.InvokeAsync();
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(ClientVM.UploadedFileName))
            {
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли си сте, че искате да изтриете прикачения файл?");
                if (isConfirmed)
                {
                    var result = await this.uploadFileService.RemoveFileByIdAsync<ValidationClient>(this.ClientVM.IdValidationClient);
                    if (result == 1)
                    {
                        ClientVM.UploadedFileName = String.Empty;
                    }

                    StateHasChanged();
                }
            }
        }
        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(ClientVM.UploadedFileName))
            {
                this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
                this.fileNameForDeletion = fileName;
                this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
              
            }
        }

        public async void ConfirmDeleteCallback()
        {

            var result = await this.uploadFileService.RemoveFileByIdAsync<ValidationClient>(this.ClientVM.IdValidationClient);
            if (result == 1)
            {
                ClientVM.UploadedFileName = null;

            }
            await this.CallbackAfterSubmit.InvokeAsync();
            this.StateHasChanged();
            this.showDeleteConfirmDialog = false;
        }


        private async Task OnDownloadClick()
        {
            this.SpinnerShow();

            if (this.loading) return;
            try
            {
                this.loading = true;
                if (!string.IsNullOrEmpty(ClientVM.UploadedFileName))
                {
                    if (ClientVM.DS_OFFICIAL_ID is null)
                    {
                        var hasFile = await this.uploadFileService.CheckIfExistUploadedFileAsync<ValidationClient>(this.ClientVM.IdValidationClient);
                        if (hasFile)
                        {
                            var documentStream = await this.uploadFileService.GetUploadedFileAsync<ValidationClient>(this.ClientVM.IdValidationClient);
                            if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                            {
                                await FileUtils.SaveAs(this.jsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                            }
                            else
                            {
                                await FileUtils.SaveAs(this.jsRuntime, this.ClientVM.FileName, documentStream.MS!.ToArray());
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
                        var contextResponse = await this.docuService.GetDocumentAsync((int)ClientVM.DS_OFFICIAL_ID, ClientVM.DS_OFFICIAL_GUID);

                        if (contextResponse.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                            return;
                        }

                        var file = contextResponse.ResultContextObject;
                        
                        var documentStream = (await docuService.GetFileAsync(file.Doc.File.First().FileID, file.Doc.GUID)).File.BinaryContent;
                        await FileUtils.SaveAs(jsRuntime, ClientVM.UploadedFileName, documentStream.ToArray());
                    }
                }
                else
                {
                    var msg = locService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

        }
    }
}
