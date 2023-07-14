
using ISNAPOO.WebSystem.Pages.Framework;

using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.ExternalExpertCommission.Modals.Expert;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Common.Constants;
using Syncfusion.PdfExport;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Data;

using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.WebSystem.Pages.Candidate.CIPO.ProcedureModalReports;

namespace ISNAPOO.WebSystem.Pages.ExternalExpertCommission
{
    public partial class ExpertsList : BlazorBaseComponent
    {
        #region Inject Services

        [Inject]
        public ICommonService CommonService { get; set; }
        [Inject]
        public IExpertService ExpertService { get; set; }
        [Inject]
        public IPersonService PersonService { get; set; }
        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        [Inject]
        public ILocationService LocationService { get; set; }
        [Inject]
        public IProfessionalDirectionService ProfessionalDirectionService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        #endregion

        ToastMsg toast;
        SfDialog sfFilter;
        SfGrid<ExpertVM> expertsGrid;
        public Query localDataQuery = new Query().Take(100);

        public string Header { get; set; }
        private bool IsVisibleAddModal = false;
        private string ExpertType = string.Empty;
        private string dialogClass = string.Empty;
        private string fileName = string.Empty;
        private string contentReport = string.Empty;
        private bool IsRegister = false;
        private string LicensingType = string.Empty;

        ExpertVM model = new ExpertVM();
        EditContext editFilterContext;
        ExternalExpertsReport externalExpertsReport = new ExternalExpertsReport();
        ExpertCommissionsReport expertCommissionsReport = new ExpertCommissionsReport();
        ExpertModal expertModal = new ExpertModal();

        IEnumerable<ExpertVM> allExperts;
        IEnumerable<KeyValueVM> kvIndentTypeSourceFlr;
        IEnumerable<KeyValueVM> kvSexSourceFlr;
        IEnumerable<LocationVM> locationSourceAll = new List<LocationVM>();
        IEnumerable<KeyValueVM> kvExpertTypeSourceFlr;
        IEnumerable<ProfessionalDirectionVM> professionalDirectionSourceFlr;
        IEnumerable<KeyValueVM> kvStatusSourceFlr;

        List<ExpertVM> experts;
        List<ExpertVM> selectedExpertsList;



        protected override async Task OnInitializedAsync()
        {
            this.LicensingType = string.Empty;
            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.Token = Token;
            tokenContext = BaseHelper.GetDecodeToken(tokenContext);


            this.LicensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "ExpertType").Value.ToString();
            if (this.LicensingType == "ExternalExperts")
            {
                if (tokenContext.ResultContextObject.ListDecodeParams.Any(l => l.Key == "MenuNode" && l.Value.ToString() == "RegisterExternalExperts"))
                {
                    this.IsRegister = true;
                    Header = "Регистър на външни експерти";
                }
                else
                {
                    Header = "Външни експерти";
                }

                contentReport = "Справка за заетостта на външните експерти";
                fileName = $"Vynshni_eksperti_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
            }
            else if (this.LicensingType == "ExpertCommissions")
            {
                if (tokenContext.ResultContextObject.ListDecodeParams.Any(l => l.Key == "MenuNode" && l.Value.ToString() == "RegisterExpertCommissions"))
                {
                    this.IsRegister = true;
                    Header = "Регистър на членовете на експертни комисии";
                }
                else
                {
                    Header = "Експертни комисии";
                }
                contentReport = "Справка експертни комисии";
                fileName = $"Ekspertni_komisii_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
            }
            else if (this.LicensingType == "DocWorkGroup")
            {
                if (tokenContext.ResultContextObject.ListDecodeParams.Any(l => l.Key == "MenuNode" && l.Value.ToString() == "RegisterDocWorkGroup"))
                {
                    this.IsRegister = true;
                    Header = "Регистър на работни групи/ рецензенти на ДОС";
                }
                else
                {
                    Header = "Рецензенти на ДОС";
                }
                fileName = $"Rabotni_grupi_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
            }
            else if (this.LicensingType == "NapooEmployees")
            {

                if (tokenContext.ResultContextObject.ListDecodeParams.Any(l => l.Key == "MenuNode" && l.Value.ToString() == "RegisterNapooEmployees"))
                {
                    this.IsRegister = true;
                    Header = "Регистър на служители на НАПОО";
                }
                else
                {
                    Header = "Експерти към НАПОО";
                }
                fileName = $"Slujiteli_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
            }
            try
            {
                this.sfFilter = new SfDialog();
                this.editFilterContext = new EditContext(this.model);
                this.selectedExpertsList = new List<ExpertVM>();

                //var result = await this.JsRuntime.InvokeAsync<bool>("isPreRendering");
                //IsConnected = true;

                //if (await this.IsSecondInitAsync(this.JsRuntime))
                //{
                //}

                this.kvIndentTypeSourceFlr = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType", true);

                this.kvSexSourceFlr = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex", true);

                this.locationSourceAll = await this.LocationService.GetAllLocationsJoinedAsync(new LocationVM());

                this.kvExpertTypeSourceFlr = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertType", true);

                this.professionalDirectionSourceFlr = await this.ProfessionalDirectionService.GetAllProfessionalDirectionsAsync(new ProfessionalDirectionVM());

                this.kvStatusSourceFlr = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertStatus", true);
            }
            catch (Exception)
            {

            }
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    this.ExpertType = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.TOKEN_EXPERT_TYPE_KEY).Value.ToString();

                    var expertVM = this.PersonService.SetExpertTypeFieldsToModel(this.ExpertType);
                    this.model = SetExpertTypeToModelFromModelForFiler(this.model, expertVM);
                    this.allExperts = await this.ExpertService.GetAllExpertsAsync(this.model);
                    this.experts = allExperts.ToList();
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, currentContext.ListErrorMessages);
                    toast.sfErrorToast.ShowAsync();
                }

            }
        }

        private async Task SelectedRow(ExpertVM _model, bool isEditable = true)
        {
            bool hasPermission = await CheckUserActionPermission("ViewExpertsData", false);
            if (!hasPermission) { return; }

            await this.expertModal.OpenModal(_model, this.ExpertType, this.LicensingType, false, isEditable);
        }

        private async Task<Task> UpdateAfterSave(ExpertVM _model)
        {
            var expertVM = this.PersonService.SetExpertTypeFieldsToModel(this.ExpertType);
            this.model = SetExpertTypeToModelFromModelForFiler(this.model, expertVM);
            this.allExperts = await this.ExpertService.GetAllExpertsAsync(this.model);
            this.experts = allExperts.ToList();

            return Task.CompletedTask;
        }
        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageExpertsData", false);
            if (!hasPermission) { return; }

            await this.expertModal.OpenModal(new ExpertVM(), this.ExpertType, this.LicensingType, true);
        }
        private async void EX()
        {
            ExcelExportProperties ExportProperties = new ExcelExportProperties();
            ExportProperties.FileName = $"Expert_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";

            await this.expertsGrid.ExcelExport(ExportProperties);
        }
        private async Task Search()
        {
            this.IsVisibleAddModal = false;

            var expertVM = this.PersonService.SetExpertTypeFieldsToModel(this.ExpertType);
            this.model = SetExpertTypeToModelFromModelForFiler(this.model, expertVM);
            this.allExperts = await this.ExpertService.GetAllExpertsAsync(this.model);
            this.experts = allExperts.ToList();
        }

        private void ShowFilter()
        {
            this.IsVisibleAddModal = true;
        }

        private void ClearFilter()
        {
            this.model = new ExpertVM();
            this.editFilterContext = new EditContext(this.model);
        }

        private void Cancel()
        {
            this.IsVisibleAddModal = false;
        }

        private void RowSelected(RowSelectEventArgs<ExpertVM> selectArgs)
        {
            this.selectedExpertsList.Add(selectArgs.Data);
        }

        private async Task RowDeselected(RowDeselectEventArgs<ExpertVM> selectArgs)
        {
            ExpertVM template = selectArgs.Data;

            this.selectedExpertsList.Remove(template);
        }

        public async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "expertsGrid_pdfexport")
            {
                int temp = expertsGrid.PageSettings.PageSize;
                expertsGrid.PageSettings.PageSize = experts.Count();
                await expertsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                PdfTheme Theme = new PdfTheme();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "Person.FirstName", HeaderText = "Име", Width = "180", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Person.SecondName", HeaderText = "Презиме", Width = "80", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Person.FamilyName", HeaderText = "Фамилия", Width = "80", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Person.Phone", HeaderText = "Телефон", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Person.Email", HeaderText = "E-mail", Width = "80", TextAlign = TextAlign.Left });
                if (this.LicensingType == "ExternalExperts")
                {
                    ExportColumns.Add(new GridColumn() { Field = "ProfessionalDirectionsInfo", HeaderText = "Професионални направления", Width = "80", TextAlign = TextAlign.Left });
                }
                if (this.LicensingType == "ExpertCommissions")
                {
                    ExportColumns.Add(new GridColumn() { Field = "CommissionsInfo", HeaderText = "Експертни комисии", Width = "80", TextAlign = TextAlign.Left, });
                }
                if (this.LicensingType == "DocWorkGroup")
                {
                    ExportColumns.Add(new GridColumn() { Field = "DOCsInfo", HeaderText = "РГ/Рецензенти на ДОС", Width = "80", TextAlign = TextAlign.Left, });
                }
                if (this.LicensingType == "NapooEmployees")
                {
                    ExportColumns.Add(new GridColumn() { Field = "IsNapooExpert", HeaderText = "Служител", Width = "80", TextAlign = TextAlign.Left, });
                }
                ExportColumns.Add(new GridColumn() { Field = "Person.PasswordResetDate", HeaderText = "E-mail на", Format = "d", Width = "80", TextAlign = TextAlign.Left, });

#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String },/*you fonts famiy in form of base64string*/
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String },/*you fonts famiy in form of base64string*/
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                //this.Grid.PdfExport(ExportProperties);
                ExportProperties.FileName = fileName + ".pdf";
                await this.expertsGrid.PdfExport(ExportProperties);
                expertsGrid.PageSettings.PageSize = temp;
                await expertsGrid.Refresh();
            }
            else if (args.Item.Id == "expertsGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = fileName + ".xlsx";
                await this.expertsGrid.ExcelExport(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ExpertVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(expertsGrid, args.Data.IdExpert).Result.ToString();
            }
        }
        private async Task DeleteSelected()
        {
            bool isSelectedTemplatesListEmpty = this.selectedExpertsList.Count == 0;

            if (isSelectedTemplatesListEmpty)
            {
                await this.JsRuntime.InvokeVoidAsync("alert", "Няма отбелязани редове от таблицата.");
            }
            else
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли сте, че искате да изтриете отбелязаните редове?");
                if (isConfirmed)
                {
                    await this.ExpertService.DeleteExpertAsync(this.selectedExpertsList);
                    toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                    await toast.sfSuccessToast.ShowAsync();
                }

                this.selectedExpertsList.Clear();
            }

            var expertVM = this.PersonService.SetExpertTypeFieldsToModel(this.ExpertType);
            this.model = SetExpertTypeToModelFromModelForFiler(this.model, expertVM);
            this.allExperts = await this.ExpertService.GetAllExpertsAsync(this.model);
            this.experts = allExperts.ToList();
            this.StateHasChanged();
        }

        private ExpertVM SetExpertTypeToModelFromModelForFiler(ExpertVM targetVM, ExpertVM fromModelVM)
        {
            targetVM.IsExternalExpert = fromModelVM.IsExternalExpert;
            targetVM.IsCommissionExpert = fromModelVM.IsCommissionExpert;
            targetVM.IsNapooExpert = fromModelVM.IsNapooExpert;
            targetVM.IsDOCExpert = fromModelVM.IsDOCExpert;

            return targetVM;
        }
        private void OpenReport()
        {
            if (this.LicensingType == "ExternalExperts")
            {
                this.externalExpertsReport.OpenModal(0, 0, 0, "All");
            }
            else
            {
                this.expertCommissionsReport.OpenModal(0);
            }
        }
    }
}
