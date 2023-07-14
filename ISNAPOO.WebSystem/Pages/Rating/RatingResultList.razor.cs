using System.Linq;
using Data.Models.Migrations;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Services.Rating;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Rating;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Data;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Data.Models.Data.Rating;
using Syncfusion.XlsIO;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.HelperClasses;
using Syncfusion.ExcelExport;
using Syncfusion.Blazor.RichTextEditor.Internal;
using System.Drawing;
using System.Text.RegularExpressions;
using ISNAPOO.Common.HelperClasses;

namespace ISNAPOO.WebSystem.Pages.Rating
{
    public partial class RatingResultList
    {
        [Inject]
        IRatingService ratingService { get; set; }

        [Inject]
        IDataSourceService dataSourceService { get; set; }
        [Inject]
        ICandidateProviderService candidateProviderService { get; set; }
        [Inject]
        public IProfessionService ProfessionService { get; set; }
        [Inject]
        public IProfessionService ProfessionalDirectionService { get; set; }
        [Inject]
        public ISpecialityService SpecialityService { get; set; }
        [Inject]
        public IDistrictService districtService { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }

        SfGrid<KeyValueVM> sfGrid = new SfGrid<KeyValueVM>();
        SfComboBox<KeyValueVM, KeyValueVM> combobox = new SfComboBox<KeyValueVM, KeyValueVM>();

        List<IndicatorVM> IndicatorSource { get; set; }
        List<IndicatorVM> AllIndicators { get; set; }
        KeyValueVM selectedIndicator = new KeyValueVM();
        KeyValueVM gridselectedIndicator = new KeyValueVM();
        List<KeyValueVM> selectedIndicators = new List<KeyValueVM>();
        List<KeyValueVM> FilteredIndicators = new List<KeyValueVM>();
        Dictionary<KeyValueVM, decimal> kVWeightPair = new Dictionary<KeyValueVM, decimal>();

        List<CandidateProviderVM> candidateProviderSource = new List<CandidateProviderVM>();
        List<CandidateProviderVM> FilteredCandidateProviders = new List<CandidateProviderVM>();
        private List<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private List<SpecialityVM> specialities = new List<SpecialityVM>();
        private List<ProfessionVM> professionFiltered = new List<ProfessionVM>();
        private List<ProfessionVM> professionSource = new List<ProfessionVM>();
        private List<ProfessionalDirectionVM> professionalDirectionSource = new List<ProfessionalDirectionVM>();
        private CandidateProviderSearchVM currentFilter = new CandidateProviderSearchVM();
        private List<DistrictVM> districtSource = new List<DistrictVM>();
        private IEnumerable<KeyValueVM> VqsSource;
        private List<CandidateProviderIndicatorExcelExportVM> providerIndicatorExcelExportVM = new List<CandidateProviderIndicatorExcelExportVM>();
        List<KeyValueVM> consultingTypes = new List<KeyValueVM>();

        private SfAutoComplete<DistrictVM, DistrictVM> sfAutoCompleteDistrict = new SfAutoComplete<DistrictVM, DistrictVM>();
        private SfMultiSelect<List<SpecialityVM>, SpecialityVM> SpecialitiesMultiSelect = new SfMultiSelect<List<SpecialityVM>, SpecialityVM>();

        private int Year { get; set; } = DateTime.Now.Year;
        private bool IsYearSelected { get; set; } = false;
        private string LicensingType { get; set; }
        protected override async Task OnInitializedAsync()
        {
            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.Token = Token;
            tokenContext = BaseHelper.GetDecodeToken(tokenContext);
            this.LicensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "RatingResultList").Value.ToString();
            this.AllIndicators = (await this.ratingService.GetAllIndicatorsAsync(new IndicatorVM())).ToList();
            this.candidateProviderSource = (await this.candidateProviderService.GetAllActiveCandidateProvidersAsync(this.LicensingType, "ProcedureCompleted")).ToList();
            this.professionSource = (await ProfessionService.GetAllActiveProfessionsAsync()).ToList().OrderBy(x => x.CodeAndName).ToList();
            this.professionSource.Remove(this.professionSource.FirstOrDefault(x => x.Code == "0"));
            this.professionFiltered = this.professionSource;
            this.professionalDirectionSource = (await ProfessionalDirectionService.GetAllActiveProfessionalDirectionsAsync()).ToList().OrderBy(x => x.DisplayNameAndCode).ToList();
            this.professionalDirectionSource.Remove(professionalDirectionSource.FirstOrDefault());
            this.specialitiesSource = (await this.SpecialityService.GetAllActiveSpecialitiesAsync()).OrderBy(x => x.CodeAndName).ToList();
            this.specialitiesSource.Remove(this.specialitiesSource.FirstOrDefault(x => x.Code == "0"));
            this.districtSource = (await this.districtService.GetAllDistrictsAsync()).ToList();
            this.VqsSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS", false);
            this.consultingTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType", false)).ToList();
            await ReloadData();
        }

        public async Task ReloadData()
        {
            this.FilteredIndicators = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType", false)).Where(x => x.DefaultValue2.Contains((this.LicensingType == "LicensingCPO" ? "CPO" : "CIPO"))).ToList().OrderBy(x => x.Name).ToList();
            this.selectedIndicators = new List<KeyValueVM>();
            this.gridselectedIndicator = new KeyValueVM();
            this.IndicatorSource = new List<IndicatorVM>();
            if (this.IsYearSelected)
            {
                await this.sfGrid.Refresh();
            }
            this.IsYearSelected = true;
        }

        #region CandidateProviders
        private async Task OnFilterLocationCorrespondence(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.districtSource = (List<DistrictVM>)(await this.districtService.GetAllDistrictsAsync()).ToList().Where(x => x.DistrictName.Contains(args.Text));
                }
                catch (Exception ex) { }

                var query = new Query().Where(new WhereFilter() { Field = "DistrictName", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.sfAutoCompleteDistrict.FilterAsync(this.districtSource, query);
            }
        }
        private async Task OnFilterSpeciality(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => x.CodeAndName.ToLower().Contains(args.Text.ToLower())).ToList();

                    var query = new Query().Where(new WhereFilter() { Field = "CodeAndName", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.SpecialitiesMultiSelect.FilterAsync(this.specialities, query);
                }
                catch (Exception ex) { }
            }
        }
        private async Task OnFocusSpeciality()
        {
            if (!(currentFilter.IdProfession == null || currentFilter.IdProfession == 0))
            {
                this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => x.IdProfession == currentFilter.IdProfession).ToList();
            }
            else if (currentFilter.IdProfessionalDirection != null && currentFilter.IdProfessionalDirection != 0)
            {
                this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => this.professionalDirectionSource.Any(y => y.IdProfessionalDirection == this.professionSource.First(z => z.IdProfession == x.IdProfession).IdProfessionalDirection)).ToList();
            }
            else
            {
                this.specialities = specialitiesSource;
            }
        }
        private async Task OnProfessionSelect()
        {
            this.specialities = this.specialities.Where(x => x.IdProfession == this.currentFilter.IdProfession).ToList();
        }
        private async Task OnProfessionalDirectionSelect()
        {
            if (this.currentFilter.IdProfessionalDirection != null && this.currentFilter.IdProfessionalDirection != 0)
            {
                this.professionFiltered = this.professionSource.Where(x => x.IdProfessionalDirection == this.currentFilter.IdProfessionalDirection).ToList();
                this.currentFilter.IdProfession = this.professionFiltered.Any(x => x.IdProfession == this.currentFilter.IdProfession) ? this.currentFilter.IdProfession : 0;

                this.currentFilter.Specialities = this.currentFilter.Specialities.Where(x => this.professionFiltered.All(y => y.IdProfession == x.IdProfession)).ToList();
                this.specialities = this.specialities.Where(x => this.professionFiltered.Any(y => y.IdProfession == x.IdProfession)).ToList();
            }
            else
            {
                this.professionFiltered = this.professionSource;
            }
        }
        #endregion

        #region Indicators
        public async Task FilterIndicators()
        {
            IsYearSelected = true;
            if (IsYearSelected)
            {
                this.FilteredIndicators = this.FilteredIndicators;//this.FilteredIndicators.Where(x => this.AllIndicators.Any(y => y.IdIndicatorType == x.IdKeyValue && y.Year == this.Year)).ToList();
                this.combobox.RefreshDataAsync();
                this.sfGrid.Refresh();
            }
        }
        public async Task AddIndicator()
        {
            if (this.selectedIndicator != null && this.selectedIndicator.IdKeyValue != 0)
            {
                this.kVWeightPair.Add(this.selectedIndicator, 0);
                this.selectedIndicators.Add(this.selectedIndicator);
                this.FilteredIndicators.Remove(this.selectedIndicator);
                this.FilteredIndicators = this.FilteredIndicators.OrderBy(x => x.Name).ToList();
                this.IndicatorSource.AddRange((await this.ratingService.GetIndicatorsByYearAndTypeIdKeyValueAsync(this.Year, this.selectedIndicator.IdKeyValue)));
                this.selectedIndicator = new KeyValueVM();
                await this.combobox.RefreshDataAsync();
                this.sfGrid.Refresh();
            }
            else
            {
                await this.ShowErrorAsync("Изберете индикатор");
            }

        }
        public async Task RemoveIndicator(KeyValueVM _indicator)
        {
            this.selectedIndicators.Remove(_indicator);
            this.FilteredIndicators.Add(_indicator);
            this.kVWeightPair.Remove(_indicator);
            this.IndicatorSource.RemoveAll(x => x.Year == this.Year && x.IdIndicatorType == _indicator.IdKeyValue);
            this.FilteredIndicators = this.FilteredIndicators.OrderBy(x => x.Name).ToList();
            await this.combobox.RefreshDataAsync();
            this.sfGrid.Refresh();
        }
        public async void SetIndicatorsWeight()
        {
            foreach (var indicator in this.IndicatorSource.Where(x => x.IdIndicatorType == this.gridselectedIndicator.IdKeyValue && x.Year == this.Year).ToList())
            {
                indicator.Weight = this.kVWeightPair.First(x => x.Key == this.gridselectedIndicator).Value;
            }
        }
        #endregion

        #region Export
        public async Task ValidateData()
        {
            if (!this.selectedIndicators.All(x => this.AllIndicators.Any(y => y.IdIndicatorType == x.IdKeyValue && y.Year == this.Year)))
            {
                this.ShowErrorAsync("Има добавен индикатор без диапазони!");
            }
            else
            {
                await this.CalculateData();
            }
        }
        public async Task CalculateData()
        {
            if (this.IndicatorSource.Count > 0)
            {
                this.SpinnerShow();
                var candidateProviders = new List<CandidateProviderVM>();
                if (this.LicensingType == "LicensingCPO")
                {
                    candidateProviders = this.candidateProviderSource.Where(d =>
                    (currentFilter.Specialities is not null && currentFilter.Specialities.Count != 0 ? d.CandidateProviderSpecialities.Any(y => this.currentFilter.Specialities.All(x => x.IdSpeciality == y.IdSpeciality)) : true)
                    && ((currentFilter.Specialities is null || currentFilter.Specialities.Count == 0) && (currentFilter.IdProfession != 0) ? d.CandidateProviderSpecialities.Any(y => y.Speciality.IdProfession == currentFilter.IdProfession) : true)
                    && ((currentFilter.IdProfessionalDirection != 0) && (currentFilter.Specialities is null || currentFilter.Specialities.Count == 0) && (currentFilter.IdProfession == 0) ? d.CandidateProviderSpecialities.Any(y => y.Speciality.Profession.IdProfessionalDirection == currentFilter.IdProfessionalDirection) : true
                    && ((currentFilter.DistrictVM != null && currentFilter.DistrictVM.idDistrict != 0) ? d.Location.Municipality.District.idDistrict == currentFilter.DistrictVM.idDistrict : true))
                    ).ToList();
                }
                else
                {
                    candidateProviders = this.candidateProviderSource.Where(d =>
                    ((currentFilter.DistrictVM != null && currentFilter.DistrictVM.idDistrict != 0) ? d.Location.Municipality.District.idDistrict == currentFilter.DistrictVM.idDistrict : true)
                    && ((currentFilter.IdConsulting != null && currentFilter.IdConsulting != 0) ? (d.CandidateProviderConsultings.Count != 0 && d.CandidateProviderConsultings.Any(x => x.IdConsultingType == currentFilter.IdConsulting)) : true)
                    ).ToList();
                }

                this.providerIndicatorExcelExportVM = new List<CandidateProviderIndicatorExcelExportVM>();
                var ProviderPointsSource = (await this.ratingService.GetAllResultsAsync()).ToList();
                foreach (var Provider in candidateProviders)
                {
                    var temp = new CandidateProviderIndicatorExcelExportVM()
                    {
                        candidateProviderVM = Provider,
                        CandidateProviderIndicators = ProviderPointsSource.Where(x => x.IdCandidate_Provider == Provider.IdCandidate_Provider).ToList()
                    };
                    if (this.IndicatorSource.Any())
                    {
                        var IndicatorWeightPair = new Dictionary<CandidateProviderIndicatorVM, decimal>();
                        var MainList = IndicatorSource.Where(x => this.selectedIndicators.Any(y => y.IdKeyValue == x.IdIndicatorType) && Year == x.Year)
                            .Where(x => temp.CandidateProviderIndicators.Any(y => y.IdIndicatorType == x.IdIndicatorType && Year == y.Year)).ToList();
                        foreach (var Indicator in MainList)
                        {
                            var list = temp.CandidateProviderIndicators.Where(x => x.IdIndicatorType == Indicator.IdIndicatorType).ToList();
                            foreach (var ProviderIndicator in list)
                            {
                                //var tempList = IndicatorSource.Where(x => this.selectedIndicators.Any(y => y.IdKeyValue == x.IdIndicatorType));
                                //    var smth = tempList.Any(x => x.IdIndicatorType == ProviderIndicator.IdIndicatorType);
                                //if (smth)
                                //{
                                if (!IndicatorWeightPair.Keys.Any(x => x == ProviderIndicator))
                                {

                                    if (ProviderIndicator.IdIndicator != null)
                                    {
                                        IndicatorWeightPair.Add(ProviderIndicator, Indicator.Weight);
                                    }
                                    if (ProviderIndicator.IdIndicator == null)
                                    {
                                        IndicatorWeightPair.Add(ProviderIndicator, 1);
                                    }
                                }
                                //}
                            }
                            temp.CandidateProviderIndicatorsWeightPair = IndicatorWeightPair;
                        }
                    }
                    this.providerIndicatorExcelExportVM.Add(temp);
                }
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet sheet = workbook.Worksheets[0];

                    #region Styles
                    IStyle HeaderStyle = workbook.Styles.Add("DarkHeaderStyle"); ;
                    HeaderStyle.Color = Syncfusion.Drawing.Color.FromArgb(0, 188, 212);
                    HeaderStyle.Font.Color = ExcelKnownColors.White;
                    HeaderStyle.Font.Bold = true;
                    HeaderStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    HeaderStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    HeaderStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    HeaderStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    HeaderStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    HeaderStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                    IStyle BodyStyle = workbook.Styles.Add("BodyStyle");
                    BodyStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    BodyStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    BodyStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    BodyStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    IStyle TableColStyle = workbook.Styles.Add("TableColStyle");
                    TableColStyle.Color = Syncfusion.Drawing.Color.LightGray;
                    TableColStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    TableColStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    TableColStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    TableColStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    IStyle FilterBody = workbook.Styles.Add("FilterBody");
                    FilterBody.Font.RGBColor = Syncfusion.Drawing.Color.FromArgb(0, 188, 212);
                    FilterBody.Font.Bold = true;
                    FilterBody.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    FilterBody.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    FilterBody.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    FilterBody.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    sheet.EnableSheetCalculations();

                    #endregion

                    int FilterRow = 1;

                    #region Filter
                    var group = sheet.Range[FilterRow, 1, FilterRow, 2].Group(ExcelGroupBy.ByColumns);
                    group.ColumnWidth = 40;
                    group.Text = "Филтър";
                    group.CellStyle = HeaderStyle;
                    group.Merge();
                    group.Ungroup(ExcelGroupBy.ByColumns);

                    group = sheet.Range[FilterRow + 1, 1, FilterRow + 1, 2].Group(ExcelGroupBy.ByColumns);
                    group.ColumnWidth = 40;
                    group.Text = $"Година: " + this.Year.ToString();
                    group.CellStyle = FilterBody;
                    group.Merge();
                    group.Ungroup(ExcelGroupBy.ByColumns);

                    if (this.LicensingType == "LicensingCPO")
                    {
                        group = sheet.Range[FilterRow + 2, 1, FilterRow + 2, 2].Group(ExcelGroupBy.ByColumns);
                        group.ColumnWidth = 40;
                        group.Text = $"Професионално направление: " + ((this.currentFilter.IdProfessionalDirection != null && this.currentFilter.IdProfessionalDirection != 0) ? this.professionalDirectionSource.FirstOrDefault(x => x.IdProfessionalDirection == this.currentFilter.IdProfessionalDirection).Name : "Всички");
                        group.CellStyle = FilterBody;
                        group.Merge();
                        group.Ungroup(ExcelGroupBy.ByColumns);

                        group = sheet.Range[FilterRow + 3, 1, FilterRow + 3, 2].Group(ExcelGroupBy.ByColumns);
                        group.ColumnWidth = 40;
                        group.Text = $"Професия: " + ((this.currentFilter.IdProfession != null && this.currentFilter.IdProfession != 0) ? this.professionSource.FirstOrDefault(x => x.IdProfession == this.currentFilter.IdProfession).Name : "Всички");
                        group.CellStyle = FilterBody;
                        group.Merge();
                        group.Ungroup(ExcelGroupBy.ByColumns);

                        group = sheet.Range[FilterRow + 4, 1, FilterRow + 4, 2].Group(ExcelGroupBy.ByColumns);
                        group.ColumnWidth = 40;
                        group.Text = $"Специалност/и: " + ((this.currentFilter.Specialities != null && this.currentFilter.Specialities.Count != 0) ? String.Join(", ", this.currentFilter.Specialities.Select(x => x.Name)) : "Всички");
                        group.CellStyle = FilterBody;
                        group.Merge();
                        group.Ungroup(ExcelGroupBy.ByColumns);

                        group = sheet.Range[FilterRow + 5, 1, FilterRow + 5, 2].Group(ExcelGroupBy.ByColumns);
                        group.ColumnWidth = 40;
                        group.Text = $"СПК: " + ((this.currentFilter.VQS != null && this.currentFilter.VQS != 0) ? this.VqsSource.FirstOrDefault(x => x.IdKeyValue == this.currentFilter.VQS).Name : "Всички");
                        group.CellStyle = FilterBody;
                        group.Merge();
                        group.Ungroup(ExcelGroupBy.ByColumns);

                        group = sheet.Range[FilterRow + 6, 1, FilterRow + 6, 2].Group(ExcelGroupBy.ByColumns);
                        group.ColumnWidth = 40;
                        group.Text = $"Област: " + (this.currentFilter.DistrictVM != null ? this.currentFilter.DistrictVM.DistrictName : "Всички");
                        group.CellStyle = FilterBody;
                        group.Merge();
                        group.Ungroup(ExcelGroupBy.ByColumns);
                    }
                    else
                    {
                        //group = sheet.Range[FilterRow + 2, 1, FilterRow + 2, 2].Group(ExcelGroupBy.ByColumns);
                        //group.ColumnWidth = 40;
                        //group.Text = $"Област: " + (this.currentFilter.DistrictVM != null ? this.currentFilter.DistrictVM.DistrictName : "Всички");
                        //group.CellStyle = FilterBody;
                        //group.Merge();
                        //group.Ungroup(ExcelGroupBy.ByColumns);

                        group = sheet.Range[FilterRow + 2, 1, FilterRow + 2, 2].Group(ExcelGroupBy.ByColumns);
                        group.ColumnWidth = 40;
                        group.Text = $"Услуга: " + (this.currentFilter.IdConsulting != null ? this.consultingTypes.First(x => x.IdKeyValue == this.currentFilter.IdConsulting).Name : "Всички");
                        group.CellStyle = FilterBody;
                        group.Merge();
                        group.Ungroup(ExcelGroupBy.ByColumns);
                    }
                    #endregion

                    #region Static Header
                    int TableHeaderRow = 0;
                    if (this.LicensingType == "LicensingCPO")
                    {
                        TableHeaderRow = FilterRow + 8;
                    }
                    else
                    {
                        TableHeaderRow = FilterRow + 4;
                    }
                    group = sheet.Range[TableHeaderRow, 1, TableHeaderRow + 2, 1].Group(ExcelGroupBy.ByColumns);
                    group.ColumnWidth = 70;
                    group.Text = (this.LicensingType == "LicensingCPO" ? "Център за професионално обучение" : "Център за информация и професионално ориентиране");
                    group.CellStyle = HeaderStyle;
                    group.Merge();
                    group.Ungroup(ExcelGroupBy.ByColumns);

                    group = sheet.Range[TableHeaderRow, 2, TableHeaderRow + 2, 2].Group(ExcelGroupBy.ByColumns);
                    group.Merge();
                    group.ColumnWidth = 20;
                    group.Text = "Рейтинг";
                    group.CellStyle = HeaderStyle;
                    group.Ungroup(ExcelGroupBy.ByColumns);

                    group = sheet.Range[TableHeaderRow + 2, 1, TableHeaderRow + 2, 2].Group(ExcelGroupBy.ByColumns);
                    group.CellStyle = HeaderStyle;
                    group.Ungroup(ExcelGroupBy.ByColumns);

                    group = sheet.Range[TableHeaderRow + 2, 1, TableHeaderRow + 2, 2].Group(ExcelGroupBy.ByColumns);
                    group.CellStyle = HeaderStyle;
                    group.Ungroup(ExcelGroupBy.ByColumns);
                    #endregion


                    foreach (var Provider in this.providerIndicatorExcelExportVM)
                    {
                        foreach (var Result in Provider.CandidateProviderIndicators)
                        {
                            if (Result.IdIndicator != null)
                            {
                                Result.Indicator = this.AllIndicators.First(x => x.IdIndicator == Result.IdIndicator);
                            }
                            else
                            {
                                Result.Indicator = new IndicatorVM() { Weight = 0 };
                            }
                        }
                        Provider.CalculateRating();
                    }
                    this.providerIndicatorExcelExportVM = this.providerIndicatorExcelExportVM.OrderByDescending(x => x.TotalRating).ToList();

                    #region TableIndicatorsHeader

                    Dictionary<KeyValueVM, int> IndicatorRangePair = new Dictionary<KeyValueVM, int>();

                    int col = 3;
                    foreach (var indicator in this.selectedIndicators)
                    {

                        IndicatorRangePair.Add(indicator, col);
                        col = col + 3;
                    }

                    foreach (var indicator in IndicatorRangePair)
                    {

                        group = sheet.Range[TableHeaderRow + 1, indicator.Value, TableHeaderRow + 1, indicator.Value + 2];
                        group.Merge();
                        group.Text = indicator.Key.Name;
                        group.CellStyle = HeaderStyle;
                        group.Ungroup(ExcelGroupBy.ByColumns);


                        sheet.Range[TableHeaderRow + 2, indicator.Value].Text = "Точки";
                        sheet.Range[TableHeaderRow + 2, indicator.Value + 1].Text = "Тежест";
                        sheet.Range[TableHeaderRow + 2, indicator.Value + 2].Text = "Общо";
                        sheet.Range[TableHeaderRow + 2, indicator.Value].CellStyle = HeaderStyle;
                        sheet.Range[TableHeaderRow + 2, indicator.Value + 1].CellStyle = HeaderStyle;
                        sheet.Range[TableHeaderRow + 2, indicator.Value + 2].CellStyle = HeaderStyle;
                    }
                    #endregion

                    #region Filling dynamic info
                    int rowCount = TableHeaderRow + 3;
                    foreach (var Provider in this.providerIndicatorExcelExportVM)
                    {
                        sheet.Range[$"A{rowCount}"].Text = Provider.candidateProviderVM.LicenceNumber + " " + $"{(this.LicensingType == "LicensingCPO" ? "ЦПО" : "ЦИПО")} {Provider.candidateProviderVM.ProviderName} към {Provider.candidateProviderVM.ProviderOwner}";
                        foreach (var CandidateIndicator in Provider.CandidateProviderIndicatorsWeightPair.Where(x => IndicatorRangePair.Any(y => y.Key.IdKeyValue == x.Key.IdIndicatorType)).ToList())
                        {
                            int StartingCol = IndicatorRangePair.First(x => x.Key.IdKeyValue == CandidateIndicator.Key.IdIndicatorType).Value;
                            sheet.Range[rowCount, StartingCol].Value2 = CandidateIndicator.Key.Points;
                            sheet.Range[rowCount, StartingCol + 1].Value2 = CandidateIndicator.Value;
                            sheet.Range[rowCount, StartingCol + 2].Value2
                                = (CandidateIndicator.Value * CandidateIndicator.Key.Points);
                        }
                        foreach (var Indicator in this.IndicatorSource.Where(x => !Provider.CandidateProviderIndicatorsWeightPair.Any(y => y.Key.IdIndicatorType == x.IdIndicatorType)).ToList())
                        {
                            int StartingCol = IndicatorRangePair.First(x => x.Key.IdKeyValue == Indicator.IdIndicatorType).Value;
                            sheet.Range[rowCount, StartingCol].Value2 = 0;
                            sheet.Range[rowCount, StartingCol + 1].Value2 = Indicator.Weight;
                            sheet.Range[rowCount, StartingCol + 2].Value2 = 0;
                        }

                        sheet.Range[$"B{rowCount}"].Value2 = Provider.TotalRating;
                        if (rowCount % 2 == 1)
                        {
                            sheet.Range[rowCount, 1, rowCount, IndicatorRangePair.Last().Value + 2].CellStyle = TableColStyle;
                        }
                        else
                        {
                            sheet.Range[rowCount, 1, rowCount, IndicatorRangePair.Last().Value + 2].CellStyle = BodyStyle;
                        }
                        rowCount++;
                    }

                    group = sheet.Range[TableHeaderRow, 3, TableHeaderRow, IndicatorRangePair.Last().Value + 2].Group(ExcelGroupBy.ByColumns);
                    group.Merge();
                    group.Text = "Индикатор";
                    group.CellStyle = HeaderStyle;
                    group.Ungroup(ExcelGroupBy.ByColumns);

                    sheet.Range[TableHeaderRow + 1, 1, TableHeaderRow + 1, IndicatorRangePair.Last().Value + 2].RowHeight = 48;
                    sheet.Range[TableHeaderRow + 1, 1, TableHeaderRow + 1, IndicatorRangePair.Last().Value + 2].WrapText = true;

                    #endregion

                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        await this.JS.SaveAs($"Result_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT)}.xlsx", stream.ToArray());
                    }
                }
                this.SpinnerHide();
            }
            else
            {
                this.ShowErrorAsync("Изберете поне един индикатор");
            }
        }
        #endregion
    }
}
