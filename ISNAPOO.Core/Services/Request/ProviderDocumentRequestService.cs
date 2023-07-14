using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.DOC;
using Data.Models.Data.Request;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace ISNAPOO.Core.Services.Request
{

    public class ProviderDocumentRequestService : BaseService, IProviderDocumentRequestService
    {
        private readonly IRepository repository;
        private readonly ILocationService locationService;
        private readonly IDataSourceService dataSourceService;
        private readonly ILogger<ProviderDocumentRequestService> _logger;
        private readonly IApplicationUserService applicationUserService;

        #region KV Document operation fields
        private IEnumerable<KeyValueVM> kvActionType;
        private KeyValueVM kvReceived;
        private KeyValueVM kvSubmitted;
        private KeyValueVM kvPrinted;
        private KeyValueVM kvCancelled;
        private KeyValueVM kvDestroyed;
        private KeyValueVM kvAwaitingConfirmation;
        #endregion

        #region KV Request document status fields
        private IEnumerable<KeyValueVM> kvRequestDocumentStatus;
        private KeyValueVM kvCreated;
        private KeyValueVM kvFiledIn;
        private KeyValueVM kvProcessed;
        private KeyValueVM kvSummarized;
        private KeyValueVM kvFulfilled;
        #endregion

        #region KV Request report status fields
        private IEnumerable<KeyValueVM> kvRequestReportStatus;
        private KeyValueVM kvRequestReportSubmitted;
        private KeyValueVM kvRequestReportCreated;
        private KeyValueVM kvRequestReportApproved;
        private KeyValueVM kvRequestReportReturned;
        #endregion

        #region KV Document receive type
        private IEnumerable<KeyValueVM> kvDocumentReceiveType;
        private KeyValueVM kvDocumentReceivedFromMON;
        private KeyValueVM kvDocumentReceivedFromOtherCPO;
        #endregion

        public ProviderDocumentRequestService(IRepository repository, ILocationService locationService, IDataSourceService dataSourceService, ILogger<ProviderDocumentRequestService> logger, AuthenticationStateProvider authenticationStateProvider)
            : base(repository)
        {
            this.repository = repository;
            this.locationService = locationService;
            this.dataSourceService = dataSourceService;
            this._logger = logger;
            this.authenticationStateProvider = authenticationStateProvider;

            this.LoadKVSources();
        }

        // зарежда данни от номенклатури
        private void LoadKVSources()
        {
            #region Document Operation
            this.kvActionType = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType").Result;
            this.kvReceived = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Received");
            this.kvSubmitted = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Submitted");
            this.kvPrinted = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Printed");
            this.kvCancelled = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Cancelled");
            this.kvDestroyed = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Destroyed");
            this.kvAwaitingConfirmation = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "AwaitingConfirmation");
            #endregion

            #region Request Document status
            this.kvRequestDocumentStatus = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RequestDocumetStatus").Result;
            this.kvCreated = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Created");
            this.kvFiledIn = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Submitted");
            this.kvProcessed = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Processed");
            this.kvSummarized = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Summarized");
            this.kvFulfilled = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Fulfilled");
            #endregion

            #region Request report status
            this.kvRequestReportStatus = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RequestReportStatus").Result;
            this.kvRequestReportSubmitted = this.kvRequestReportStatus.FirstOrDefault(x => x.KeyValueIntCode == "Submitted");
            this.kvRequestReportCreated = this.kvRequestReportStatus.FirstOrDefault(x => x.KeyValueIntCode == "Created");
            this.kvRequestReportApproved = this.kvRequestReportStatus.FirstOrDefault(x => x.KeyValueIntCode == "Approved");
            this.kvRequestReportReturned = this.kvRequestReportStatus.FirstOrDefault(x => x.KeyValueIntCode == "Returned");
            #endregion

            #region Document receive type
            this.kvDocumentReceiveType = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("DocumentRequestReceiveType").Result;
            this.kvDocumentReceivedFromMON = this.kvDocumentReceiveType.FirstOrDefault(x => x.KeyValueIntCode == "PrintingHouse");
            this.kvDocumentReceivedFromOtherCPO = this.kvDocumentReceiveType.FirstOrDefault(x => x.KeyValueIntCode == "OtherCPO");
            #endregion
        }

        #region Provider request document
        public async Task<IEnumerable<ProviderRequestDocumentVM>> GetAllDocumentRequestsByCandidateProviderIdAsync(int idCandidateProvider)
        {
            return await this.repository.AllReadonly<ProviderRequestDocument>(x => x.IdCandidateProvider == idCandidateProvider).To<ProviderRequestDocumentVM>().OrderByDescending(x => x.RequestDate).ToListAsync();
        }

        public async Task LoadDataForDocumentTypesAsync(List<ProviderRequestDocumentVM> documents)
        {
            foreach (var doc in documents)
            {
                doc.RequestDocumentTypes = await this.repository.AllReadonly<RequestDocumentType>(x => x.IdProviderRequestDocument == doc.IdProviderRequestDocument).To<RequestDocumentTypeVM>().ToListAsync();
            }
        }

        public async Task<IEnumerable<ProviderRequestDocumentVM>> GetAllDocumentRequestsAsync()
        {
            return await this.repository.AllReadonly<ProviderRequestDocument>().To<ProviderRequestDocumentVM>(x => x.NAPOORequestDoc, x => x.CandidateProvider).OrderByDescending(x => x.RequestDate).ToListAsync();
        }

        public async Task<IEnumerable<ProviderRequestDocumentVM>> GetAllDocumentRequestsWhereStatusIsSummarizedAndByIdCandidateProviderAsync(int idCandidateProvider)
        {
            IQueryable<ProviderRequestDocument> requestDocuments = this.repository.AllReadonly<ProviderRequestDocument>(x => x.IdCandidateProvider == idCandidateProvider).Include(x => x.RequestDocumentStatuses).Include(x => x.RequestDocumentTypes);
            requestDocuments = requestDocuments.Where(x => x.RequestDocumentStatuses.OrderByDescending(x => x.IdStatus).FirstOrDefault().IdStatus == this.kvSummarized.IdKeyValue);
            var result = await requestDocuments.To<ProviderRequestDocumentVM>(x => x.RequestDocumentStatuses, x => x.RequestDocumentTypes).ToListAsync();

            return result;
        }

        public async Task<ResultContext<ProviderRequestDocumentVM>> CreateProviderRequestDocumentAsync(ResultContext<ProviderRequestDocumentVM> inputContext)
        {
            ResultContext<ProviderRequestDocumentVM> resultContext = new ResultContext<ProviderRequestDocumentVM>();

            var providerRequestDocumentVM = inputContext.ResultContextObject;

            try
            {
                if (providerRequestDocumentVM.IdProviderRequestDocument != 0)
                {
                    var entryFromDb = await this.repository.GetByIdAsync<ProviderRequestDocument>(providerRequestDocumentVM.IdProviderRequestDocument);

                    entryFromDb = providerRequestDocumentVM.To<ProviderRequestDocument>();
                    entryFromDb.NAPOORequestDoc = null;
                    entryFromDb.CandidateProvider = null;
                    entryFromDb.RequestDocumentStatuses = null;
                    entryFromDb.RequestDocumentTypes = null;


                    this.repository.Update<ProviderRequestDocument>(entryFromDb);
                    await this.repository.SaveChangesAsync();

                    providerRequestDocumentVM.IdCreateUser = entryFromDb.IdCreateUser;
                    providerRequestDocumentVM.CreationDate = entryFromDb.CreationDate;
                    providerRequestDocumentVM.IdModifyUser = entryFromDb.IdModifyUser;
                    providerRequestDocumentVM.ModifyDate = entryFromDb.ModifyDate;
                }
                else
                {
                    providerRequestDocumentVM.IdStatus = this.kvCreated.IdKeyValue;
                    var entryForDb = providerRequestDocumentVM.To<ProviderRequestDocument>();
                    entryForDb.NAPOORequestDoc = null;
                    entryForDb.CandidateProvider = null;
                    entryForDb.RequestDocumentStatuses = null;
                    entryForDb.RequestDocumentTypes = null;

                    await this.repository.AddAsync<ProviderRequestDocument>(entryForDb);
                    await this.repository.SaveChangesAsync();

                    providerRequestDocumentVM.IdProviderRequestDocument = entryForDb.IdProviderRequestDocument;
                    providerRequestDocumentVM.IdCreateUser = entryForDb.IdCreateUser;
                    providerRequestDocumentVM.CreationDate = entryForDb.CreationDate;
                    providerRequestDocumentVM.IdModifyUser = entryForDb.IdModifyUser;
                    providerRequestDocumentVM.ModifyDate = entryForDb.ModifyDate;
                    RequestDocumentStatusVM requestDocumentStatusVM = new RequestDocumentStatusVM()
                    {
                        IdCandidateProvider = providerRequestDocumentVM.IdCandidateProvider,
                        IdProviderRequestDocument = providerRequestDocumentVM.IdProviderRequestDocument,
                        IdStatus = this.kvCreated.IdKeyValue
                    };

                    await this.CreateRequestDocumentStatusAsync(requestDocumentStatusVM);

                    providerRequestDocumentVM.RequestStatus = "Създадена";
                }

                foreach (var entry in providerRequestDocumentVM.RequestDocumentTypes)
                {
                    if (entry.IdRequestDocumentType != 0)
                    {
                        var entryFromDb = await this.repository.GetByIdAsync<RequestDocumentType>(entry.IdRequestDocumentType);

                        entry.IdCreateUser = entryFromDb.IdCreateUser;
                        entry.CreationDate = entryFromDb.CreationDate;
                        entryFromDb = entry.To<RequestDocumentType>();
                        entryFromDb.CandidateProvider = null;
                        entryFromDb.ProviderRequestDocument = null;
                        entryFromDb.TypeOfRequestedDocument = null;


                        this.repository.Update<RequestDocumentType>(entryFromDb);
                        await this.repository.SaveChangesAsync();
                    }
                    else
                    {
                        var entryForDb = entry.To<RequestDocumentType>();
                        entryForDb.CandidateProvider = null;
                        entryForDb.ProviderRequestDocument = null;
                        entryForDb.TypeOfRequestedDocument = null;

                        entryForDb.IdProviderRequestDocument = providerRequestDocumentVM.IdProviderRequestDocument;

                        await this.repository.AddAsync<RequestDocumentType>(entryForDb);
                        await this.repository.SaveChangesAsync();

                        entry.IdRequestDocumentType = entryForDb.IdRequestDocumentType;
                        entry.IdProviderRequestDocument = providerRequestDocumentVM.IdProviderRequestDocument;
                    }
                }
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            if (!resultContext.HasErrorMessages)
            {
                resultContext.AddMessage("Записът е успешен!");
            }

            resultContext.ResultContextObject = providerRequestDocumentVM;
            return resultContext;
        }

        public async Task<ProviderRequestDocumentVM> GetProviderRequestDocumentByIdAsync(ProviderRequestDocumentVM providerRequestDocumentVM)
        {
            IQueryable<ProviderRequestDocument> data = this.repository.AllReadonly<ProviderRequestDocument>(x => x.IdProviderRequestDocument == providerRequestDocumentVM.IdProviderRequestDocument);
            var result = data.To<ProviderRequestDocumentVM>(x => x.RequestDocumentTypes, x => x.RequestDocumentStatuses);

            return await result.FirstOrDefaultAsync();
        }

        public async Task<ResultContext<ProviderRequestDocumentVM>> DeleteProviderRequestDocumentByIdAsync(ProviderRequestDocumentVM providerRequestDocumentVM)
        {
            ResultContext<ProviderRequestDocumentVM> resultContext = new ResultContext<ProviderRequestDocumentVM>();

            var data = await this.repository.GetByIdAsync<ProviderRequestDocument>(providerRequestDocumentVM.IdProviderRequestDocument);
            this.repository.Detach<ProviderRequestDocument>(data);

            var statuses = this.repository.AllReadonly<RequestDocumentStatus>(x => x.IdProviderRequestDocument == data.IdProviderRequestDocument);
            if (statuses.Any())
            {
                this.repository.HardDeleteRange<RequestDocumentStatus>(statuses);
                await this.repository.SaveChangesAsync();
            }

            var types = this.repository.AllReadonly<RequestDocumentType>(x => x.IdProviderRequestDocument == data.IdProviderRequestDocument);
            if (types.Any())
            {
                this.repository.HardDeleteRange<RequestDocumentType>(types);
                await this.repository.SaveChangesAsync();
            }

            try
            {
                await this.repository.HardDeleteAsync<ProviderRequestDocument>(data.IdProviderRequestDocument);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е изтрит успешно!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<ResultContext<ProviderRequestDocumentVM>> FileInProviderRequestDocumentAsync(ResultContext<ProviderRequestDocumentVM> inputContext)
        {
            ResultContext<ProviderRequestDocumentVM> resultContext = new ResultContext<ProviderRequestDocumentVM>();

            var providerRequestDocumentVM = inputContext.ResultContextObject;

            try
            {
                RequestDocumentStatusVM requestDocumentStatusVM = new RequestDocumentStatusVM()
                {
                    IdCandidateProvider = providerRequestDocumentVM.IdCandidateProvider,
                    IdProviderRequestDocument = providerRequestDocumentVM.IdProviderRequestDocument,
                    IdStatus = this.kvFiledIn.IdKeyValue
                };

                await this.CreateRequestDocumentStatusAsync(requestDocumentStatusVM);

                providerRequestDocumentVM.RequestStatus = "Подадена";

                resultContext.ListMessages.Add("Записът е успешен!");

                providerRequestDocumentVM.RequestDate = DateTime.Now;
                providerRequestDocumentVM.IdStatus = this.kvFiledIn.IdKeyValue;
                providerRequestDocumentVM.RequestNumber = await this.GetSequenceNextValue("RequestDocument");
                var entryForDb = providerRequestDocumentVM.To<ProviderRequestDocument>();
                entryForDb.RequestDocumentStatuses = null;
                entryForDb.RequestDocumentTypes = null;
                entryForDb.ModifyDate = DateTime.Now;

                this.repository.Update<ProviderRequestDocument>(entryForDb);
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<MemoryStream> PrintRequestDocumentAsync(ProviderRequestDocumentVM providerRequestDocumentVM, IEnumerable<TypeOfRequestedDocumentVM> typeOfRequestedDocuments, CandidateProviderVM providerVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\RequestDocument";

            FileStream template = new FileStream($@"{resources_Folder}\Zaqvka_dokumentaciq_CPO_20220713_v001.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Doc);
            WSection section = document.Sections[0];

            WTable table = section.Tables[0] as WTable;

            WCharacterFormat characterFormat = new WCharacterFormat(document);
            characterFormat.FontName = "Times New Roman";
            characterFormat.FontSize = 11;

            var counter = 2;
            foreach (var doc in typeOfRequestedDocuments.OrderBy(x => x.Order.Value))
            {
                WTableRow row = table.Rows[1].Clone();
                WTableCell firstCell = row.Cells[0];
                IWTextRange textRange1 = firstCell.AddParagraph().AppendText(doc.DocTypeOfficialNumber);
                textRange1.ApplyCharacterFormat(characterFormat);
                firstCell.Paragraphs.RemoveAt(0);
                firstCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                WTableCell secondCell = row.Cells[1];
                secondCell.Paragraphs.RemoveAt(0);
                IWTextRange textRange2 = secondCell.AddParagraph().AppendText(doc.DocTypeName);
                textRange2.ApplyCharacterFormat(characterFormat);
                secondCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                WTableCell thirdCell = row.Cells[2];
                var quantity = 0;
                var addedDoc = providerRequestDocumentVM.RequestDocumentTypes.FirstOrDefault(x => x.IdTypeOfRequestedDocument == doc.IdTypeOfRequestedDocument);
                if (addedDoc is not null)
                {
                    quantity = addedDoc.DocumentCount;
                }

                IWTextRange textRange3 = thirdCell.AddParagraph().AppendText(quantity.ToString());
                textRange3.ApplyCharacterFormat(characterFormat);
                thirdCell.Paragraphs.RemoveAt(0);
                thirdCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                table.Rows.Insert(counter++, row);
            }

            table.Rows.RemoveAt(1);

            LocationVM location = await this.locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(providerRequestDocumentVM.IdLocationCorrespondence ?? default);
            NAPOORequestDocVM nAPOORequestDocVM = await this.GetNAPOORequestDocumentByIdAsync(new NAPOORequestDocVM() { IdNAPOORequestDoc = providerRequestDocumentVM.IdNAPOORequestDoc ?? default });

            string[] fieldNames = new string[]
            {
                "CPORequestNumber", "CPORequestDate", "CurrentYear", "ExpertName", "CPOName", "Area",
                "Telephone", "Address", "LicenseNumber", "ProviderBulstat", "Name", "NAPOORequestNumber", "NAPOORequestDate"
            };

            string[] fieldValues = new string[]
            {
                providerRequestDocumentVM.RequestNumber.ToString(),
                providerRequestDocumentVM.RequestDate.Value.ToString("dd.MM.yyyy"),
                providerRequestDocumentVM.CurrentYear.ToString(),
                providerRequestDocumentVM.Name,
                providerVM.CPONameOwnerGrid,
                location.Municipality.District.DistrictName,
                providerRequestDocumentVM.Telephone,
                $"{location.LocationName}, {providerRequestDocumentVM.Address}",
                providerVM.LicenceNumber,
                providerVM.PoviderBulstat,
                providerRequestDocumentVM.Name,
                nAPOORequestDocVM != null ? nAPOORequestDocVM.NAPOORequestNumber.HasValue ? nAPOORequestDocVM.NAPOORequestNumber.ToString() : string.Empty : string.Empty,
                nAPOORequestDocVM != null ? nAPOORequestDocVM.RequestDate.HasValue ? $"{nAPOORequestDocVM.RequestDate.Value.ToString("dd.MM.yyyy")} г." : string.Empty : string.Empty
            };

            document.MailMerge.Execute(fieldNames, fieldValues);
            MemoryStream stream = new MemoryStream();
            DocIORenderer render = new DocIORenderer();
            render.Settings.ChartRenderingOptions.ImageFormat = Syncfusion.OfficeChart.ExportImageFormat.Jpeg;
            PdfDocument pdfDocument = render.ConvertToPDF(document);
            render.Dispose();
            document.Dispose();
            pdfDocument.Save(stream);
            pdfDocument.Close();
            template.Close();

            return stream;
        }

        public async Task<MemoryStream> GeneratePrintingTemplateAsync(List<TypeOfRequestedDocumentVM> typeOfRequestedDocuments, NAPOORequestDocVM nAPOORequestDocVM, bool printInPDF)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\NAPOO\RequestDocument";
            FileStream template = new FileStream($@"{resources_Folder}\Obobshteni_zaiavki_template.xlsx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook = application.Workbooks.Open(template, ExcelOpenType.Automatic);
            IWorksheet worksheet = workbook.Worksheets[0];

            worksheet.Rows[4].Text = $"№ {nAPOORequestDocVM.NAPOORequestNumber} от {nAPOORequestDocVM.RequestDate.Value.ToString("dd.MM.yyyy")}";

            Dictionary<string, Dictionary<string, int>> areasWithDocCounts = new Dictionary<string, Dictionary<string, int>>();
            foreach (var docType in typeOfRequestedDocuments)
            {
                areasWithDocCounts.Add(docType.DocTypeOfficialNumber, new Dictionary<string, int>()
                {
                    { "Благоевград", 0 },
                    { "Бургас", 0 },
                    { "Варна", 0 },
                    { "Велико Търново", 0 },
                    { "Видин", 0 },
                    { "Враца", 0 },
                    { "Габрово", 0 },
                    { "Добрич", 0 },
                    { "Кърджали", 0 },
                    { "Кюстендил", 0 },
                    { "Ловеч", 0 },
                    { "Монтана", 0 },
                    { "Пазарджик", 0 },
                    { "Перник", 0 },
                    { "Плевен", 0 },
                    { "Пловдив", 0 },
                    { "Разград", 0 },
                    { "Русе", 0 },
                    { "Силистра", 0 },
                    { "Сливен", 0 },
                    { "Смолян", 0 },
                    { "София-град", 0 },
                    { "София-област", 0 },
                    { "Стара Загора", 0 },
                    { "Търговище", 0 },
                    { "Хасково", 0 },
                    { "Шумен", 0 },
                    { "Ямбол", 0 }
                });
            }

            foreach (var providerRequest in nAPOORequestDocVM.ProviderRequestDocuments)
            {
                LocationVM location = await this.locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(providerRequest.IdLocationCorrespondence ?? default);
                location.Municipality.District.DistrictName = location.Municipality.District.DistrictName == "София (столица)" ? "София-град" : location.Municipality.District.DistrictName;
                location.Municipality.District.DistrictName = location.Municipality.District.DistrictName == "София" ? "София-област" : location.Municipality.District.DistrictName;
                foreach (var docType in providerRequest.RequestDocumentTypes)
                {
                    var doc = typeOfRequestedDocuments.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docType.IdTypeOfRequestedDocument);
                    areasWithDocCounts[doc.DocTypeOfficialNumber][location.Municipality.District.DistrictName] += docType.DocumentCount;
                }
            }

            IRange range;
            int rowCounter = 8;
            int priceRow = 31;
            foreach (var typeOfDocument in typeOfRequestedDocuments)
            {
                worksheet.InsertRow(rowCounter++);
                worksheet[rowCounter - 1, 1].Text = typeOfDocument.DocTypeOfficialNumber;
                range = worksheet[rowCounter - 1, 1];
                range.CellStyle.Font.Bold = true;

                worksheet[rowCounter - 1, priceRow].Text = typeOfDocument.Price.Value.ToString();

                int docCount = 0;
                foreach (var area in areasWithDocCounts[typeOfDocument.DocTypeOfficialNumber])
                {
                    if (area.Value > 0)
                    {
                        switch (area.Key)
                        {
                            case "Благоевград":
                                worksheet[rowCounter - 1, 2].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Бургас":
                                worksheet[rowCounter - 1, 3].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Варна":
                                worksheet[rowCounter - 1, 4].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Велико Търново":
                                worksheet[rowCounter - 1, 5].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Видин":
                                worksheet[rowCounter - 1, 6].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Враца":
                                worksheet[rowCounter - 1, 7].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Габрово":
                                worksheet[rowCounter - 1, 8].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Добрич":
                                worksheet[rowCounter - 1, 9].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Кърджали":
                                worksheet[rowCounter - 1, 10].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Кюстендил":
                                worksheet[rowCounter - 1, 11].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Ловеч":
                                worksheet[rowCounter - 1, 12].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Монтана":
                                worksheet[rowCounter - 1, 13].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Пазарджик":
                                worksheet[rowCounter - 1, 14].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Перник":
                                worksheet[rowCounter - 1, 15].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Плевен":
                                worksheet[rowCounter - 1, 16].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Пловдив":
                                worksheet[rowCounter - 1, 17].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Разград":
                                worksheet[rowCounter - 1, 18].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Русе":
                                worksheet[rowCounter - 1, 19].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Силистра":
                                worksheet[rowCounter - 1, 20].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Сливен":
                                worksheet[rowCounter - 1, 21].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Смолян":
                                worksheet[rowCounter - 1, 22].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "София-град":
                                worksheet[rowCounter - 1, 23].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "София-област":
                                worksheet[rowCounter - 1, 24].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Стара Загора":
                                worksheet[rowCounter - 1, 25].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Търговище":
                                worksheet[rowCounter - 1, 26].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Хасково":
                                worksheet[rowCounter - 1, 27].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Шумен":
                                worksheet[rowCounter - 1, 28].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            case "Ямбол":
                                worksheet[rowCounter - 1, 29].Text = area.Value.ToString();
                                docCount += area.Value;
                                break;
                            default:
                                break;
                        }
                    }
                }

                worksheet[rowCounter - 1, 30].Text = docCount.ToString();

                range = worksheet[rowCounter - 1, 32];
                range.CellStyle.Font.Bold = true;

                if (docCount != 0)
                {
                    decimal sum = docCount * (decimal)typeOfDocument.Price.Value;
                    worksheet[rowCounter - 1, 32].Text = $"{sum.ToString("f2")} лв.";
                }
                else
                {
                    worksheet[rowCounter - 1, 32].Text = "0 лв.";
                }
            }

            range = worksheet["A8:AF19"];
            range.BorderInside(ExcelLineStyle.Thin);
            range.BorderAround(ExcelLineStyle.Thin);
            range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;

            range = worksheet["A1:AF24"];
            range.CellStyle.Font.Size = 11.5;
            range.AutofitColumns();

            MemoryStream stream = new MemoryStream();

            if (!printInPDF)
            {
                workbook.SaveAs(stream);
            }
            else
            {
                XlsIORenderer renderer = new XlsIORenderer();
                PdfDocument pdfDocument = renderer.ConvertToPDF(workbook);
                pdfDocument.Save(stream);
                PdfLoadedDocument lDoc = new PdfLoadedDocument(stream);
                lDoc.Pages.RemoveAt(1);
                lDoc.Save(stream);
                lDoc.Close();
            }

            template.Dispose();
            excelEngine.Dispose();

            return stream;
        }

        public async Task UpdateProviderRequestDocumentUploadedFileNameAsync(int idProviderRequestDocument, string fileName)
        {
            var providerRequestDocumentFromDb = await this.repository.GetByIdAsync<ProviderRequestDocument>(idProviderRequestDocument);

            try
            {
                providerRequestDocumentFromDb.UploadedFileName = fileName;

                this.repository.Update<ProviderRequestDocument>(providerRequestDocumentFromDb);
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }
        #endregion

        #region Type of requested document
        public async Task<IEnumerable<TypeOfRequestedDocumentVM>> GetAllValidTypesOfRequestedDocumentAsync()
        {
            IQueryable<TypeOfRequestedDocument> requestDocuments = this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IsValid);
            var result = requestDocuments.To<TypeOfRequestedDocumentVM>().OrderBy(x => x.Order);

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<TypeOfRequestedDocumentVM>> GetAllTypesOfRequestedDocumentAsync()
        {
            IQueryable<TypeOfRequestedDocument> requestDocuments = this.repository.AllReadonly<TypeOfRequestedDocument>();
            var result = requestDocuments.To<TypeOfRequestedDocumentVM>().OrderBy(x => x.DocTypeOfficialNumber);

            return await result.OrderBy(x => x.Order).ToListAsync();
        }

        public async Task<TypeOfRequestedDocumentVM> GetTypeOfRequestedDocumentAsyncByDocTypeOfficialNumber(string docTypeOfficialNumber)
        {
            IQueryable<TypeOfRequestedDocument> requestDocuments = this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == docTypeOfficialNumber);
            var typeOfRequestedDocumentVM = await requestDocuments.To<TypeOfRequestedDocumentVM>().FirstOrDefaultAsync();

            return typeOfRequestedDocumentVM;
        }

        public async Task<TypeOfRequestedDocumentVM> GetTypeOfRequestedDocumentByIdAsync(TypeOfRequestedDocumentVM typeOfRequestedDocumentVM)
        {
            IQueryable<TypeOfRequestedDocument> typeOfRequestedDocuments = this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IdTypeOfRequestedDocument == typeOfRequestedDocumentVM.IdTypeOfRequestedDocument);

            return await typeOfRequestedDocuments.To<TypeOfRequestedDocumentVM>().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TypeOfRequestedDocumentVM>> GetTypesOfRequestedDocumentByListIdsAsync(List<int> ids)
        {
            IQueryable<TypeOfRequestedDocument> typeOfRequestedDocuments = this.repository.AllReadonly<TypeOfRequestedDocument>(x => ids.Contains(x.IdTypeOfRequestedDocument));

            return await typeOfRequestedDocuments.To<TypeOfRequestedDocumentVM>().OrderBy(x => x.Order).ToListAsync();
        }

        // записите, при които IdRequestDocumentManagement != null, са изключени от връщания резултат
        public async Task<IEnumerable<TypeOfRequestedDocumentVM>> GetTypesOfRequestedDocumentsByRequestNumberAsync(List<ProviderRequestDocumentVM> providerRequestDocumentsSource, long? requestNumber)
        {
            var providerRequestDocumentSource = providerRequestDocumentsSource.FirstOrDefault(x => x.RequestNumber == requestNumber);
            var listIdsTypeOfRequestedDocuments = providerRequestDocumentSource.RequestDocumentTypes.Where(x => x.IdRequestDocumentManagement == null).Select(x => x.IdTypeOfRequestedDocument).ToList();
            var typeOfRequestedDocuments = this.repository.AllReadonly<TypeOfRequestedDocument>(x => listIdsTypeOfRequestedDocuments.Contains(x.IdTypeOfRequestedDocument));

            return await typeOfRequestedDocuments.To<TypeOfRequestedDocumentVM>().OrderBy(x => x.Order).ToListAsync();
        }

        public async Task<ResultContext<TypeOfRequestedDocumentVM>> CreateTypeOfRequestedDocumentAsync(ResultContext<TypeOfRequestedDocumentVM> inputContext)
        {
            var resultContext = new ResultContext<TypeOfRequestedDocumentVM>();
            try
            {
                var entryForDb = inputContext.ResultContextObject.To<TypeOfRequestedDocument>();
                entryForDb.DocumentSerialNumbers = null;
                entryForDb.ProviderDocumentOffers = null;
                entryForDb.RequestDocumentManagements = null;
                entryForDb.RequestDocumentTypes = null;
                entryForDb.DocumentSeries = null;

                await this.repository.AddAsync<TypeOfRequestedDocument>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdTypeOfRequestedDocument = entryForDb.IdTypeOfRequestedDocument;
                resultContext.ResultContextObject.PriceAsStr = entryForDb.Price.ToString();

                resultContext.ListMessages.Add("Записът е успешен!");
            }
            catch (Exception ex)
            {
                resultContext.ListErrorMessages.Add("Грешка при запис в базата данни!");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<ResultContext<TypeOfRequestedDocumentVM>> UpdateTypeOfRequestedDocumentAsync(ResultContext<TypeOfRequestedDocumentVM> inputContext)
        {
            var resultContext = new ResultContext<TypeOfRequestedDocumentVM>();
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<TypeOfRequestedDocument>(inputContext.ResultContextObject.IdTypeOfRequestedDocument);
                inputContext.ResultContextObject.IdCreateUser = entryFromDb.IdCreateUser;
                inputContext.ResultContextObject.CreationDate = entryFromDb.CreationDate;

                entryFromDb = inputContext.ResultContextObject.To<TypeOfRequestedDocument>();
                entryFromDb.DocumentSerialNumbers = null;
                entryFromDb.ProviderDocumentOffers = null;
                entryFromDb.RequestDocumentManagements = null;
                entryFromDb.RequestDocumentTypes = null;
                entryFromDb.DocumentSeries = null;

                this.repository.Update<TypeOfRequestedDocument>(entryFromDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdTypeOfRequestedDocument = entryFromDb.IdTypeOfRequestedDocument;
                resultContext.ResultContextObject.PriceAsStr = entryFromDb.Price.ToString();

                resultContext.ListMessages.Add("Записът е успешен!");
            }
            catch (Exception ex)
            {
                resultContext.ListErrorMessages.Add("Грешка при запис в базата данни!");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<IEnumerable<TypeOfRequestedDocumentVM>> GetAllTypeOfRequestedDocsForLegalCapacityCourse()
        {
            var kvRegulation = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7");
            var data = this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IdCourseType == kvRegulation.IdKeyValue && x.IsValid);

            return await data.To<TypeOfRequestedDocumentVM>().ToListAsync();
        }
        #endregion

        #region Request document type
        public async Task<RequestDocumentTypeVM> GetRequestDocumentTypeByIdTypeOfRequestDocumentAndIdProviderRequestDocumentAsync(RequestDocumentTypeVM requestDocumentTypeVM)
        {
            IQueryable<RequestDocumentType> data = this.repository.AllReadonly<RequestDocumentType>(x => x.IdTypeOfRequestedDocument == requestDocumentTypeVM.IdTypeOfRequestedDocument && x.IdProviderRequestDocument == requestDocumentTypeVM.IdProviderRequestDocument);
            var result = data.To<RequestDocumentTypeVM>();

            return await result.FirstOrDefaultAsync();
        }

        public async Task<ResultContext<RequestDocumentTypeVM>> DeleteRequestDocumentTypeByIdAsync(RequestDocumentTypeVM requestDocumentTypeVM)
        {
            ResultContext<RequestDocumentTypeVM> resultContext = new ResultContext<RequestDocumentTypeVM>();

            var data = await this.repository.GetByIdAsync<RequestDocumentType>(requestDocumentTypeVM.IdRequestDocumentType);
            this.repository.Detach<RequestDocumentType>(data);

            try
            {
                await this.repository.HardDeleteAsync<RequestDocumentType>(data.IdRequestDocumentType);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е изтрит успешно!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task UpdateRequestDocumentTypeAsync(RequestDocumentTypeVM requestDocumentTypeVM)
        {
            try
            {
                var requestDocumentTypeForDb = await this.repository.GetByIdAsync<RequestDocumentType>(requestDocumentTypeVM.IdRequestDocumentType);
                this.repository.Detach<RequestDocumentType>(requestDocumentTypeForDb);
                requestDocumentTypeForDb = requestDocumentTypeVM.To<RequestDocumentType>();
                requestDocumentTypeForDb.ModifyDate = DateTime.Now;
                requestDocumentTypeForDb.CandidateProvider = null;
                requestDocumentTypeForDb.ProviderRequestDocument = null;
                requestDocumentTypeForDb.RequestDocumentManagement = null;
                requestDocumentTypeForDb.TypeOfRequestedDocument = null;

                this.repository.Update<RequestDocumentType>(requestDocumentTypeForDb);
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }
        #endregion

        #region Request document status
        public async Task CreateRequestDocumentStatusAsync(RequestDocumentStatusVM requestDocumentStatusVM)
        {
            var entryForDb = requestDocumentStatusVM.To<RequestDocumentStatus>();
            entryForDb.CandidateProvider = null;
            entryForDb.ProviderRequestDocument = null;


            await this.repository.AddAsync<RequestDocumentStatus>(entryForDb);
            await this.repository.SaveChangesAsync();
            this.repository.Detach<RequestDocumentStatus>(entryForDb);
        }

        public async Task<IEnumerable<RequestDocumentStatusVM>> GetRequestDocumentStatusesByRequestDocumentIdAsync(int idRequestDocument)
        {
            var data = this.repository.AllReadonly<RequestDocumentStatus>(x => x.IdProviderRequestDocument == idRequestDocument);

            return await data.To<RequestDocumentStatusVM>().ToListAsync();
        }
        #endregion

        #region NAPOO request document
        public async Task<IEnumerable<NAPOORequestDocVM>> GetAllNAPOORequestDocumentsAsync()
        {
            IQueryable<NAPOORequestDoc> data = this.repository.AllReadonly<NAPOORequestDoc>();

            return await data.To<NAPOORequestDocVM>(x => x.ProviderRequestDocuments, x => x.ProviderRequestDocuments.Select(y => y.RequestDocumentTypes), x => x.ProviderRequestDocuments).OrderByDescending(x => x.IdNAPOORequestDoc).ToListAsync();
        }

        public async Task<ResultContext<NAPOORequestDocVM>> CreateNAPOORequestDocumentAsync(ResultContext<NAPOORequestDocVM> inputContext)
        {
            ResultContext<NAPOORequestDocVM> resultContext = new ResultContext<NAPOORequestDocVM>();
            var napooRequestDocumentVM = inputContext.ResultContextObject;

            try
            {
                var napooRequestDocentryForDb = napooRequestDocumentVM.To<NAPOORequestDoc>();
                napooRequestDocentryForDb.ProviderRequestDocuments = null;

                if (napooRequestDocentryForDb.IdNAPOORequestDoc == 0)
                {


                    await this.repository.AddAsync<NAPOORequestDoc>(napooRequestDocentryForDb);
                    await this.repository.SaveChangesAsync();

                    this.repository.Detach<NAPOORequestDoc>(napooRequestDocentryForDb);

                    napooRequestDocumentVM.IdCreateUser = napooRequestDocentryForDb.IdCreateUser;
                    napooRequestDocumentVM.IdModifyUser = napooRequestDocentryForDb.IdModifyUser;
                    napooRequestDocumentVM.CreationDate = napooRequestDocentryForDb.CreationDate;
                    napooRequestDocumentVM.ModifyDate = napooRequestDocentryForDb.ModifyDate;
                }
                else
                {
                    var napooRequestDocFromDb = await this.repository.GetByIdAsync<NAPOORequestDoc>(napooRequestDocentryForDb.IdNAPOORequestDoc);
                    this.repository.Detach<NAPOORequestDoc>(napooRequestDocFromDb);



                    this.repository.Update<NAPOORequestDoc>(napooRequestDocFromDb);
                    await this.repository.SaveChangesAsync();
                    this.repository.Detach<NAPOORequestDoc>(napooRequestDocFromDb);

                    napooRequestDocumentVM.IdModifyUser = napooRequestDocFromDb.IdModifyUser;
                    napooRequestDocumentVM.ModifyDate = napooRequestDocFromDb.ModifyDate;
                }

                foreach (var providerRequest in napooRequestDocumentVM.ProviderRequestDocuments)
                {
                    var providerRequestFromDb = await this.repository.GetByIdAsync<ProviderRequestDocument>(providerRequest.IdProviderRequestDocument);
                    this.repository.Detach<ProviderRequestDocument>(providerRequestFromDb);

                    providerRequestFromDb.IdNAPOORequestDoc = napooRequestDocentryForDb.IdNAPOORequestDoc;
                    providerRequestFromDb.ModifyDate = DateTime.Now;
                    providerRequestFromDb.IdStatus = this.kvProcessed.IdKeyValue;

                    this.repository.Update<ProviderRequestDocument>(providerRequestFromDb);
                    await this.repository.SaveChangesAsync();
                    this.repository.Detach<ProviderRequestDocument>(providerRequestFromDb);

                    var requestDocumentStatusFromDb = this.repository.AllReadonly<RequestDocumentStatus>(x => x.IdProviderRequestDocument == providerRequestFromDb.IdProviderRequestDocument && x.IdStatus == this.kvProcessed.IdKeyValue).FirstOrDefault();

                    if (requestDocumentStatusFromDb is not null)
                    {


                        this.repository.Update<RequestDocumentStatus>(requestDocumentStatusFromDb);
                        await this.repository.SaveChangesAsync();

                        this.repository.Detach<RequestDocumentStatus>(requestDocumentStatusFromDb);
                    }
                    else
                    {
                        RequestDocumentStatus requestDocumentStatus = new RequestDocumentStatus()
                        {
                            IdStatus = this.kvProcessed.IdKeyValue,
                            IdCandidateProvider = providerRequestFromDb.IdCandidateProvider,
                            IdProviderRequestDocument = providerRequestFromDb.IdProviderRequestDocument

                        };

                        await this.repository.AddAsync<RequestDocumentStatus>(requestDocumentStatus);
                        await this.repository.SaveChangesAsync();
                        this.repository.Detach<RequestDocumentStatus>(requestDocumentStatus);
                    }
                }

                napooRequestDocumentVM.IdNAPOORequestDoc = napooRequestDocentryForDb.IdNAPOORequestDoc;
                resultContext.ResultContextObject = napooRequestDocumentVM;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<ResultContext<NAPOORequestDocVM>> GenerateFileForMONPrinting(ResultContext<NAPOORequestDocVM> inputContext)
        {
            var resultContext = new ResultContext<NAPOORequestDocVM>();
            var napooRequestDocumentForDb = inputContext.ResultContextObject;

            try
            {
                var napooRequestFromDb = await this.repository.GetByIdAsync<NAPOORequestDoc>(napooRequestDocumentForDb.IdNAPOORequestDoc);

                napooRequestFromDb = napooRequestDocumentForDb.To<NAPOORequestDoc>();
                napooRequestFromDb.ProviderRequestDocuments = null;
                napooRequestFromDb.ModifyDate = DateTime.Now;
                napooRequestFromDb.RequestDate = DateTime.Now;
                napooRequestFromDb.IsSent = true;
                napooRequestFromDb.NAPOORequestNumber = await this.GetSequenceNextValue("NAPOORequestNumber");

                this.repository.Update<NAPOORequestDoc>(napooRequestFromDb);
                await this.repository.SaveChangesAsync();

                foreach (var providerRequest in napooRequestDocumentForDb.ProviderRequestDocuments)
                {
                    var providerRequestFromDb = await this.repository.GetByIdAsync<ProviderRequestDocument>(providerRequest.IdProviderRequestDocument);
                    if (providerRequestFromDb is not null)
                    {
                        providerRequestFromDb.IdStatus = this.kvSummarized.IdKeyValue;

                        this.repository.Update<ProviderRequestDocument>(providerRequestFromDb);
                    }

                    RequestDocumentStatus requestDocumentStatus = new RequestDocumentStatus()
                    {
                        IdCandidateProvider = providerRequest.IdCandidateProvider,
                        IdProviderRequestDocument = providerRequest.IdProviderRequestDocument,
                        IdStatus = this.kvSummarized.IdKeyValue
                    };

                    await this.repository.AddAsync<RequestDocumentStatus>(requestDocumentStatus);
                    await this.repository.SaveChangesAsync();
                }

                resultContext.ResultContextObject = napooRequestFromDb.To<NAPOORequestDocVM>();
                resultContext.ResultContextObject.ProviderRequestDocuments = napooRequestDocumentForDb.ProviderRequestDocuments;
                inputContext.ResultContextObject.RequestDate = napooRequestFromDb.RequestDate;
                inputContext.ResultContextObject.IsSent = napooRequestFromDb.IsSent;
                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<NAPOORequestDocVM> GetNAPOORequestDocumentByIdAsync(NAPOORequestDocVM nAPOORequestDocVM)
        {
            IQueryable<NAPOORequestDoc> data = this.repository.AllReadonly<NAPOORequestDoc>(x => x.IdNAPOORequestDoc == nAPOORequestDocVM.IdNAPOORequestDoc);

            return await data.To<NAPOORequestDocVM>().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PrintingHouseReportVM>> GetPrintingHouseReportDataAsync()
        {
            List<PrintingHouseReportVM> data = new List<PrintingHouseReportVM>();

            var napooRequestDocuments = await this.repository.AllReadonly<NAPOORequestDoc>(x => x.NAPOORequestNumber.HasValue)
                .Include(x => x.ProviderRequestDocuments.Take(1))
                    .ThenInclude(x => x.LocationCorrespondence)
                        .ThenInclude(x => x.Municipality)
                            .ThenInclude(x => x.District)
                .AsNoTracking()
                .ToListAsync();

            var counter = 1;
            foreach (var entry in napooRequestDocuments)
            {
                foreach (var providerRequest in entry.ProviderRequestDocuments)
                {
                    var districtName = providerRequest.LocationCorrespondence.Municipality.District.DistrictName;
                    if (!data.Any(x => x.NAPOORequestNumber == entry.NAPOORequestNumber && x.District == districtName))
                    {
                        PrintingHouseReportVM report = new PrintingHouseReportVM()
                        {
                            Id = counter++,
                            District = districtName,
                            NAPOORequestNumber = entry.NAPOORequestNumber!.Value,
                            RequestDate = entry.RequestDate,
                            NAPOORequestDoc = entry.To<NAPOORequestDocVM>()
                        };

                        data.Add(report);
                    }
                }
            }

            data = data.OrderBy(x => x.District).ThenByDescending(x => x.NAPOORequestNumber).ToList();

            return data;
        }

        public async Task<NAPOORequestDocVM> GetNAPOORequestDocDataByIdNAPOORequestDocAsync(int idNAPOORequestDoc)
        {
            try
            {
                var napooRequestDocument = await this.repository.AllReadonly<NAPOORequestDoc>(x => x.IdNAPOORequestDoc == idNAPOORequestDoc)
                .To<NAPOORequestDocVM>(x => x.ProviderRequestDocuments.Select(y => y.LocationCorrespondence.Municipality.District),
                    x => x.ProviderRequestDocuments.Select(y => y.RequestDocumentTypes.Select(o => o.TypeOfRequestedDocument)),
                    x => x.ProviderRequestDocuments.Select(y => y.CandidateProvider),
                    x => x.ProviderRequestDocuments.Select(y => y.RequestDocumentStatuses))
                .FirstOrDefaultAsync();

                return napooRequestDocument;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return null;
        }

        public async Task UpdateNotificationSentStatusForNAPOORequestDocAsync(NAPOORequestDocVM nAPOORequestDoc)
        {
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<NAPOORequestDoc>(nAPOORequestDoc.IdNAPOORequestDoc);

                entryFromDb.IsNotificationSent = true;

                this.repository.Update<NAPOORequestDoc>(entryFromDb);
                await this.repository.SaveChangesAsync();

                nAPOORequestDoc.IsNotificationSent = entryFromDb.IsNotificationSent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }
        #endregion

        #region Document offer

        public async Task<IEnumerable<ProviderDocumentOfferVM>> GetAllProviderDocumentOffersAsync(ProviderDocumentOfferVM fiterModel)
        {
            IQueryable<ProviderDocumentOffer> data = this.repository.All<ProviderDocumentOffer>(FilterDocValue(fiterModel));
            var viewVMs = await data.To<ProviderDocumentOfferVM>(x => x.CandidateProvider.LocationCorrespondence, x => x.TypeOfRequestedDocument).ToListAsync();

            var offerTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("OfferType");

            foreach (var entry in viewVMs)
            {
                var offerType = offerTypes.FirstOrDefault(a => a.IdKeyValue == entry.IdOfferType);

                if (offerType is not null)
                {
                    entry.OfferTypeName = offerType.Name;
                }
            }

            return viewVMs;
        }

        protected Expression<Func<ProviderDocumentOffer, bool>> FilterDocValue(ProviderDocumentOfferVM model)
        {
            var predicate = PredicateBuilder.True<ProviderDocumentOffer>();

            if (model.OfferEndDate.HasValue && model.OfferEndDate > DateTime.MinValue)
            {
                predicate = predicate.And(p => !p.OfferEndDate.HasValue || p.OfferEndDate >= model.OfferEndDate);
            }

            if (model.IdCandidateProvider > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdCandidateProvider == model.IdCandidateProvider);
            }

            return predicate;
        }

        public async Task<ProviderDocumentOfferVM> GetProviderDocumentOfferByIdAsync(ProviderDocumentOfferVM model)
        {
            var data = await this.repository.All<ProviderDocumentOffer>(x => x.IdProviderDocumentOffer == model.IdProviderDocumentOffer)
                .Include(x => x.CandidateProvider).FirstOrDefaultAsync();

            this.repository.Detach<ProviderDocumentOffer>(data);

            ProviderDocumentOfferVM viewModel = data.To<ProviderDocumentOfferVM>();

            return viewModel;

        }

        public async Task<ResultContext<ProviderDocumentOfferVM>> SaveProviderDocumentOfferAsync(ResultContext<ProviderDocumentOfferVM> resultContext)
        {
            if (resultContext.ResultContextObject.IdProviderDocumentOffer == GlobalConstants.INVALID_ID_ZERO)
            {
                resultContext = await CreateProviderDocumentOfferAsync(resultContext);
            }
            else
            {
                resultContext = await UpdateProviderDocumentOfferAsync(resultContext);
            }

            return resultContext;
        }

        private async Task<ResultContext<ProviderDocumentOfferVM>> CreateProviderDocumentOfferAsync(ResultContext<ProviderDocumentOfferVM> resultContext)
        {
            var newPrice = resultContext.ResultContextObject.To<ProviderDocumentOffer>();



            await this.repository.AddAsync<ProviderDocumentOffer>(newPrice);
            int result = await this.repository.SaveChangesAsync();

            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
                resultContext.ResultContextObject.IdProviderDocumentOffer = newPrice.IdProviderDocumentOffer;
                resultContext.ResultContextObject.CreationDate = newPrice.CreationDate;
                resultContext.ResultContextObject.ModifyDate = newPrice.ModifyDate;
                resultContext.ResultContextObject.IdCreateUser = newPrice.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = newPrice.IdModifyUser;
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }

            return resultContext;
        }

        private async Task<ResultContext<ProviderDocumentOfferVM>> UpdateProviderDocumentOfferAsync(ResultContext<ProviderDocumentOfferVM> resultContext)
        {
            try
            {
                var updatedEnity = await this.GetByIdAsync<ProviderDocumentOffer>(resultContext.ResultContextObject.IdProviderDocumentOffer);
                this.repository.Detach<ProviderDocumentOffer>(updatedEnity);

                updatedEnity = resultContext.ResultContextObject.To<ProviderDocumentOffer>();


                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();
                resultContext.ResultContextObject.CreationDate = updatedEnity.CreationDate;
                resultContext.ResultContextObject.ModifyDate = updatedEnity.ModifyDate;
                resultContext.ResultContextObject.IdCreateUser = updatedEnity.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = updatedEnity.IdModifyUser;
                if (result > 0)
                {
                    resultContext.AddMessage("Записът e успешeн!");
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата!");
                }
            }
            catch (Exception еx)
            {
                _logger.LogError(еx.Message);
                _logger.LogError(еx.InnerException?.Message);
                _logger.LogError(еx.StackTrace);
            }

            return resultContext;
        }

        #endregion

        #region Request document management
        public async Task<ResultContext<NoResult>> AddDocumentSerialNumberToRequestDocumentManagementAsync(DocumentSerialNumberVM documentSerialNumber)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var entryForDb = documentSerialNumber.To<DocumentSerialNumber>();

                await this.repository.AddAsync<DocumentSerialNumber>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        protected Expression<Func<RequestDocumentManagement, bool>> FilterRequestDocumentManagement(RequestDocumentManagementVM model)
        {
            var predicate = PredicateBuilder.True<RequestDocumentManagement>();

            if (model.IdCandidateProvider != 0)
            {
                predicate = predicate.And(p => p.IdCandidateProvider == model.IdCandidateProvider);
            }

            return predicate;
        }

        public async Task<IEnumerable<RequestDocumentManagementControlModel>> GetDocumentsControlDataAsync(int idCandidateProvider)
        {
            var result = new List<RequestDocumentManagementControlModel>();

            var data = await this.GetAllRequestDocumentManagementsAsync(idCandidateProvider);

            int counter = 1;
            foreach (var entry in data)
            {
                if (entry.IdDocumentOperation == this.kvAwaitingConfirmation.IdKeyValue)
                {
                    continue;
                }

                var uniqueData = $"{entry.CandidateProvider.ProviderOwner}{entry.ReceiveDocumentYear}{entry.TypeOfRequestedDocument.NumberWithName}";
                if (!result.Any(x => x.CPONameDocumentYearAndDocumentType == uniqueData))
                {
                    RequestDocumentManagementControlModel model = new RequestDocumentManagementControlModel()
                    {
                        Id = counter++,
                        EntityId = entry.IdRequestDocumentManagement,
                        DocumentYear = entry.ReceiveDocumentYear.Value,
                        Provider = entry.CandidateProvider,
                        TypeOfRequestedDocument = entry.TypeOfRequestedDocument,
                        ProviderRequestDocument = entry.ProviderRequestDocument,
                        DocumentSerialNumbers = entry.DocumentSerialNumbers.ToList(),
                        RequestDocumentTypes = entry.RequestDocumentTypes.ToList()
                    };

                    result.Add(model);
                }

                var entryToUpdate = result.FirstOrDefault(x => x.CPONameDocumentYearAndDocumentType == uniqueData);
                if (entryToUpdate != null)
                {
                    foreach (var serialNumber in entry.DocumentSerialNumbers)
                    {
                        serialNumber.RequestDocumentManagement = entry;
                        foreach (var item in entry.DocumentSerialNumbers)
                        {
                            if (!entryToUpdate.DocumentSerialNumbers.Any(x => x.SerialNumber == item.SerialNumber && x.IdDocumentOperation == item.IdDocumentOperation))
                            {
                                entryToUpdate.DocumentSerialNumbers.Add(item);
                            }
                        }
                    }

                    foreach (var docType in entry.RequestDocumentTypes)
                    {
                        if (!entryToUpdate.RequestDocumentTypes.Any(x => x.IdTypeOfRequestedDocument == docType.IdTypeOfRequestedDocument))
                        {
                            entryToUpdate.DocumentSerialNumbers.AddRange(entry.DocumentSerialNumbers);
                        }
                    }

                    if (entry.IdDocumentOperation == this.kvReceived.IdKeyValue)
                    {
                        entryToUpdate.ReceivedCount += entry.DocumentCount;
                    }
                    else if (entry.IdDocumentOperation == this.kvSubmitted.IdKeyValue)
                    {
                        entryToUpdate.HandedOverCount += entry.DocumentCount;
                    }
                    else if (entry.IdDocumentOperation == this.kvPrinted.IdKeyValue)
                    {
                        entryToUpdate.PrintedCount += entry.DocumentCount;
                    }
                    else if (entry.IdDocumentOperation == this.kvCancelled.IdKeyValue)
                    {
                        entryToUpdate.CancelledCount += entry.DocumentCount;
                    }
                    else if (entry.IdDocumentOperation == this.kvDestroyed.IdKeyValue)
                    {
                        entryToUpdate.DestroyedCount += entry.DocumentCount;
                    }
                }
            }

            foreach (var entry in result)
            {
                entry.AvailableCount = entry.ReceivedCount - entry.HandedOverCount - entry.PrintedCount - entry.DestroyedCount;
            }

            return result;
        }

        public async Task<IEnumerable<RequestDocumentManagementVM>> GetAllRequestDocumentManagementsByIdCandidateProviderAndDocumentOperationReceivedAsync(int idCandidateProvider)
        {
            IQueryable<RequestDocumentManagement> data = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdCandidateProvider == idCandidateProvider && x.IdDocumentOperation == this.kvReceived.IdKeyValue);

            return await data.To<RequestDocumentManagementVM>(x => x.TypeOfRequestedDocument, x => x.CandidateProvider, x => x.CandidateProviderPartner, x => x.ProviderRequestDocument, x => x.DocumentSerialNumbers, x => x.RequestDocumentManagementUploadedFiles).ToListAsync();
        }

        public async Task<ResultContext<RequestDocumentManagementVM>> CreateRequestDocumentManagementAsync(ResultContext<RequestDocumentManagementVM> inputContext)
        {
            ResultContext<RequestDocumentManagementVM> outputContext = new ResultContext<RequestDocumentManagementVM>();
            var requestDocumentManagementVM = inputContext.ResultContextObject;

            try
            {
                var requestDocumentManagementForDb = requestDocumentManagementVM.To<RequestDocumentManagement>();
                requestDocumentManagementForDb.CandidateProvider = null;
                requestDocumentManagementForDb.CandidateProviderPartner = null;
                requestDocumentManagementForDb.ProviderRequestDocument = null;
                requestDocumentManagementForDb.TypeOfRequestedDocument = null;


                await this.repository.AddAsync<RequestDocumentManagement>(requestDocumentManagementForDb);
                await this.repository.SaveChangesAsync();
                this.repository.Detach<RequestDocumentManagement>(requestDocumentManagementForDb);

                if (requestDocumentManagementForDb.IdDocumentOperation == this.kvSubmitted.IdKeyValue)
                {
                    var requestDocumentManagementOnStatusAwaitingConfirmation = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdCandidateProvider == requestDocumentManagementForDb.IdCandidateProviderPartner
                        && x.IdCandidateProviderPartner == requestDocumentManagementForDb.IdCandidateProvider
                        && x.IdTypeOfRequestedDocument == requestDocumentManagementForDb.IdTypeOfRequestedDocument
                        && x.DocumentCount == requestDocumentManagementForDb.DocumentCount
                        && x.IdDocumentOperation == this.kvAwaitingConfirmation.IdKeyValue);

                    if (!requestDocumentManagementOnStatusAwaitingConfirmation.Any())
                    {
                        var requestDocumentManagementAwaitingConfirmationForDb = requestDocumentManagementVM.To<RequestDocumentManagement>();
                        requestDocumentManagementAwaitingConfirmationForDb.CandidateProvider = null;
                        requestDocumentManagementAwaitingConfirmationForDb.CandidateProviderPartner = null;
                        requestDocumentManagementAwaitingConfirmationForDb.ProviderRequestDocument = null;
                        requestDocumentManagementAwaitingConfirmationForDb.TypeOfRequestedDocument = null;
                        requestDocumentManagementAwaitingConfirmationForDb.IdCandidateProvider = requestDocumentManagementForDb.IdCandidateProviderPartner.Value;
                        requestDocumentManagementAwaitingConfirmationForDb.IdCandidateProviderPartner = requestDocumentManagementForDb.IdCandidateProvider;
                        requestDocumentManagementAwaitingConfirmationForDb.DocumentCount = requestDocumentManagementForDb.DocumentCount;

                        requestDocumentManagementAwaitingConfirmationForDb.IdDocumentOperation = kvAwaitingConfirmation.IdKeyValue;

                        await this.repository.AddAsync<RequestDocumentManagement>(requestDocumentManagementAwaitingConfirmationForDb);
                        await this.repository.SaveChangesAsync();
                    }
                }

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = requestDocumentManagementForDb.To<RequestDocumentManagementVM>();
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage("Грешка при запис в базата данни!");
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<RequestDocumentManagementVM>> UpdateRequestDocumentManagementAsync(ResultContext<RequestDocumentManagementVM> inputContext, bool alwaysAddNewDocumentSerialNumber = false, bool updateAfterReceiveFromOtherCPO = false)
        {
            ResultContext<RequestDocumentManagementVM> outputContext = new ResultContext<RequestDocumentManagementVM>();
            var requestDocumentManagementVM = inputContext.ResultContextObject;

            try
            {
                var requestDocumentManagementForDb = await this.repository.GetByIdAsync<RequestDocumentManagement>(requestDocumentManagementVM.IdRequestDocumentManagement);
                this.repository.Detach<RequestDocumentManagement>(requestDocumentManagementForDb);

                requestDocumentManagementVM.IdCreateUser = requestDocumentManagementForDb.IdCreateUser;
                requestDocumentManagementVM.CreationDate = requestDocumentManagementForDb.CreationDate;
                requestDocumentManagementVM.IdCandidateProvider = requestDocumentManagementForDb.IdCandidateProvider;
                requestDocumentManagementVM.IdCandidateProviderPartner = requestDocumentManagementForDb.IdCandidateProviderPartner;
                requestDocumentManagementVM.IdTypeOfRequestedDocument = requestDocumentManagementForDb.IdTypeOfRequestedDocument;
                requestDocumentManagementForDb = requestDocumentManagementVM.To<RequestDocumentManagement>();
                requestDocumentManagementForDb.CandidateProvider = null;
                requestDocumentManagementForDb.CandidateProviderPartner = null;
                requestDocumentManagementForDb.ProviderRequestDocument = null;
                requestDocumentManagementForDb.TypeOfRequestedDocument = null;
                requestDocumentManagementForDb.DocumentSerialNumbers = null;

                if (updateAfterReceiveFromOtherCPO)
                {
                    requestDocumentManagementForDb.IdDocumentOperation = this.kvReceived.IdKeyValue;
                }

                this.repository.Update<RequestDocumentManagement>(requestDocumentManagementForDb);
                await this.repository.SaveChangesAsync();
                this.repository.Detach<RequestDocumentManagement>(requestDocumentManagementForDb);

                var reqDocManagementPartnerProvider = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdCandidateProvider == requestDocumentManagementForDb.IdCandidateProviderPartner
                        && x.IdCandidateProviderPartner == requestDocumentManagementForDb.IdCandidateProvider
                        && x.IdTypeOfRequestedDocument == requestDocumentManagementForDb.IdTypeOfRequestedDocument
                        && x.ReceiveDocumentYear == requestDocumentManagementForDb.ReceiveDocumentYear
                        && x.IdDocumentOperation == this.kvAwaitingConfirmation.IdKeyValue).FirstOrDefault();

                if (reqDocManagementPartnerProvider is not null)
                {
                    reqDocManagementPartnerProvider.DocumentCount = requestDocumentManagementForDb.DocumentCount;

                    this.repository.Update<RequestDocumentManagement>(reqDocManagementPartnerProvider);
                    await this.repository.SaveChangesAsync();
                }

                if (!alwaysAddNewDocumentSerialNumber)
                {
                    if (!updateAfterReceiveFromOtherCPO)
                    {
                        await this.HandleDocumentSerialNumbers(requestDocumentManagementVM, true);
                    }
                    else
                    {
                        await this.HandleDocumentSerialNumbers(requestDocumentManagementVM, true);
                    }
                }
                else
                {
                    await this.HandleDocumentSerialNumbersWithExplicitAdding(requestDocumentManagementVM);
                }

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = requestDocumentManagementForDb.To<RequestDocumentManagementVM>();
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage("Грешка при запис в базата данни!");
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        private async Task HandleDocumentSerialNumbers(RequestDocumentManagementVM requestDocumentManagementVM, bool updateAfterDocumentSerialNumberReceivedFromOtherCPO = false)
        {
            foreach (var docSerialNumber in requestDocumentManagementVM.DocumentSerialNumbers)
            {
                if (docSerialNumber.IdDocumentSerialNumber == 0)
                {
                    var docSerialNumberForDb = docSerialNumber.To<DocumentSerialNumber>();

                    await this.repository.AddAsync<DocumentSerialNumber>(docSerialNumberForDb);
                    await this.repository.SaveChangesAsync();
                    this.repository.Detach<DocumentSerialNumber>(docSerialNumberForDb);

                    docSerialNumber.IdDocumentSerialNumber = docSerialNumberForDb.IdDocumentSerialNumber;
                }

                if (updateAfterDocumentSerialNumberReceivedFromOtherCPO)
                {
                    var docSerialNumberForDb = docSerialNumber.To<DocumentSerialNumber>();
                    docSerialNumberForDb.IdDocumentOperation = this.kvReceived.IdKeyValue;

                    this.repository.Update<DocumentSerialNumber>(docSerialNumberForDb);
                    await this.repository.SaveChangesAsync();
                    this.repository.Detach<DocumentSerialNumber>(docSerialNumberForDb);

                    docSerialNumber.IdDocumentSerialNumber = docSerialNumberForDb.IdDocumentSerialNumber;
                }
            }
        }

        private async Task HandleDocumentSerialNumbersWithExplicitAdding(RequestDocumentManagementVM requestDocumentManagementVM)
        {
            foreach (var docSerialNumber in requestDocumentManagementVM.DocumentSerialNumbers)
            {
                var serialNumberFromDb = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdDocumentOperation == this.kvSubmitted.IdKeyValue
                    && x.IdCandidateProvider == requestDocumentManagementVM.IdCandidateProvider
                    && x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument
                    && x.SerialNumber == docSerialNumber.SerialNumber
                    && x.IdRequestDocumentManagement == requestDocumentManagementVM.IdRequestDocumentManagement);

                if (!serialNumberFromDb.Any())
                {
                    var docSerialNumberForDb = docSerialNumber.To<DocumentSerialNumber>();
                    docSerialNumberForDb.TypeOfRequestedDocument = null;
                    docSerialNumberForDb.IdDocumentSerialNumber = 0;
                    docSerialNumberForDb.IdDocumentOperation = this.kvSubmitted.IdKeyValue;
                    docSerialNumberForDb.IdCandidateProvider = requestDocumentManagementVM.IdCandidateProvider;
                    docSerialNumberForDb.IdRequestDocumentManagement = requestDocumentManagementVM.IdRequestDocumentManagement;
                    docSerialNumberForDb.DocumentDate = DateTime.Now;

                    await this.repository.AddAsync<DocumentSerialNumber>(docSerialNumberForDb);
                    await this.repository.SaveChangesAsync();
                    this.repository.Detach<DocumentSerialNumber>(docSerialNumberForDb);

                    docSerialNumber.IdDocumentSerialNumber = docSerialNumberForDb.IdDocumentSerialNumber;

                    var serialNumberAwaitingConfirmationFromDb = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdDocumentOperation == this.kvAwaitingConfirmation.IdKeyValue
                    && x.IdCandidateProvider == requestDocumentManagementVM.IdCandidateProvider
                    && x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument
                    && x.SerialNumber == docSerialNumber.SerialNumber
                    && x.IdRequestDocumentManagement == requestDocumentManagementVM.IdRequestDocumentManagement);

                    if (!serialNumberAwaitingConfirmationFromDb.Any())
                    {
                        var docSerialNumberAwaitingConfirmationForDb = docSerialNumber.To<DocumentSerialNumber>();
                        docSerialNumberAwaitingConfirmationForDb.TypeOfRequestedDocument = null;
                        docSerialNumberAwaitingConfirmationForDb.IdDocumentSerialNumber = 0;
                        docSerialNumberAwaitingConfirmationForDb.IdDocumentOperation = this.kvAwaitingConfirmation.IdKeyValue;
                        docSerialNumberAwaitingConfirmationForDb.IdCandidateProvider = requestDocumentManagementVM.IdCandidateProviderPartner.Value;
                        docSerialNumberAwaitingConfirmationForDb.IdRequestDocumentManagement = requestDocumentManagementVM.IdRequestDocumentManagement + 1;
                        docSerialNumberAwaitingConfirmationForDb.DocumentDate = DateTime.Now;

                        await this.repository.AddAsync<DocumentSerialNumber>(docSerialNumberAwaitingConfirmationForDb);
                        await this.repository.SaveChangesAsync();
                        this.repository.Detach<DocumentSerialNumber>(docSerialNumberAwaitingConfirmationForDb);
                    }
                }
            }
        }

        public async Task<IEnumerable<RequestDocumentManagementVM>> GetAllRequestDocumentManagementsByDocumentOperationSubmittedAndByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdDocumentOperation == this.kvSubmitted.IdKeyValue && x.IdCandidateProvider == idCandidateProvider);

            return await data.To<RequestDocumentManagementVM>(x => x.DocumentSerialNumbers).ToListAsync();
        }

        public async Task<IEnumerable<RequestDocumentManagementVM>> GetAllRequestDocumentManagementsByDocumentOperationReceivedAndByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdDocumentOperation == this.kvReceived.IdKeyValue && x.IdCandidateProvider == idCandidateProvider);

            return await data.To<RequestDocumentManagementVM>().ToListAsync();
        }

        public int GetDocumentCountByDocumentOperationReceivedByProviderIdByTypeOfRequestedDocumentIdAndReceiveYear(RequestDocumentManagementVM requestDocumentManagementVM)
        {
            var data = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdCandidateProvider == requestDocumentManagementVM.IdCandidateProvider
                && x.IdTypeOfRequestedDocument == requestDocumentManagementVM.IdTypeOfRequestedDocument
                && x.ReceiveDocumentYear == requestDocumentManagementVM.ReceiveDocumentYear).ToList();

            var receivedDocManagement = data.Where(x => x.IdDocumentOperation == this.kvReceived.IdKeyValue);
            var sumReceived = receivedDocManagement.Sum(x => x.DocumentCount);
            var otherDocManagement = data.Where(x => x.IdDocumentOperation != this.kvReceived.IdKeyValue);
            var sumOther = otherDocManagement.Sum(x => x.DocumentCount);

            return sumReceived - sumOther;
        }

        public async Task<RequestDocumentManagementVM> GetRequestDocumentManagementByIdAsync(RequestDocumentManagementVM requestDocumentManagementVM)
        {
            var data = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdRequestDocumentManagement == requestDocumentManagementVM.IdRequestDocumentManagement);

            return await data.To<RequestDocumentManagementVM>(x => x.DocumentSerialNumbers).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RequestDocumentManagementVM>> GetAllRequestDocumentManagementsByIdProviderAndByDocumentOperationAwaitingConfirmationAsync(RequestDocumentManagementVM requestDocumentManagementVM)
        {
            var data = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdCandidateProvider == requestDocumentManagementVM.IdCandidateProvider
                && x.IdCandidateProviderPartner != null
                && x.IdDocumentOperation == this.kvAwaitingConfirmation.IdKeyValue);

            return await data.To<RequestDocumentManagementVM>().ToListAsync();
        }

        public async Task<RequestDocumentManagementVM> GetRequestDocumentManagementByProviderIdByProviderPartnerIdByIdTypeOfRequestedDocumentAndByDocumentOperationAwaitingConfirmationAsync(RequestDocumentManagementVM requestDocumentManagementVM)
        {
            var data = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdCandidateProvider == requestDocumentManagementVM.IdCandidateProvider
                && x.IdCandidateProviderPartner == requestDocumentManagementVM.IdCandidateProviderPartner
                && x.IdTypeOfRequestedDocument == requestDocumentManagementVM.IdTypeOfRequestedDocument
                && x.IdDocumentOperation == this.kvAwaitingConfirmation.IdKeyValue);

            return await data.To<RequestDocumentManagementVM>(x => x.DocumentSerialNumbers, x => x.TypeOfRequestedDocument).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateDocumentCountAfterDocumentSerialNumberDeletionByIdRequestDocumentManagementAsync(int idRequestDocumentManagement)
        {
            var data = await this.repository.GetByIdAsync<RequestDocumentManagement>(idRequestDocumentManagement);
            data.DocumentCount--;

            this.repository.Update<RequestDocumentManagement>(data);
            return await this.repository.SaveChangesAsync();
        }

        public async Task<MemoryStream> PrintHandingOverProtocolAsync(List<RequestDocumentManagementVM> requestDocumentManagements, List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource, List<DocumentSeriesVM> documentSeries)
        {
            var reqDocManagement = requestDocumentManagements.FirstOrDefault();
            documentSeries = documentSeries.Where(x => x.Year == reqDocManagement.ReceiveDocumentYear).ToList();

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\RequestDocument";

            FileStream template = new FileStream($@"{resources_Folder}\PPP_za_dokumenti_s_fabrichni_nomera.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Doc);
            WSection section = document.Sections[0];

            WTable table = section.Tables[0] as WTable;

            var typeOfDocsAsDict = new Dictionary<int, List<string>>();

            foreach (var entry in requestDocumentManagements)
            {
                if (entry.DocumentSerialNumbers.Any())
                {
                    foreach (var doc in entry.DocumentSerialNumbers.OrderBy(x => x.SerialNumberAsIntForOrderBy))
                    {
                        if (!typeOfDocsAsDict.ContainsKey(doc.IdTypeOfRequestedDocument))
                        {
                            typeOfDocsAsDict.Add(doc.IdTypeOfRequestedDocument, new List<string>());
                        }

                        typeOfDocsAsDict[doc.IdTypeOfRequestedDocument].Add(doc.SerialNumber);
                    }
                }
                else
                {
                    if (!typeOfDocsAsDict.ContainsKey(entry.IdTypeOfRequestedDocument))
                    {
                        typeOfDocsAsDict.Add(entry.IdTypeOfRequestedDocument, new List<string>());
                    }
                }
            }

            var counter = 2;
            foreach (var entry in typeOfDocsAsDict)
            {
                var resultList = this.CalculateConsecutiveFabricNumbers(entry.Value.Select(int.Parse).ToList());
                var typeOfDoc = typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key);
                var docSeries = documentSeries.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key && x.Year == reqDocManagement.ReceiveDocumentYear);
                var valueForDocseries = string.Empty;
                if (docSeries is not null)
                {
                    valueForDocseries = docSeries.SeriesName;
                }
                else
                {
                    valueForDocseries = string.Empty;
                }

                if (resultList.Any())
                {
                    foreach (var item in resultList)
                    {
                        WTableRow row = table.Rows[2].Clone();

                        WTableCell firstCell = row.Cells[0];
                        firstCell.Paragraphs.RemoveAt(0);
                        firstCell.AddParagraph().AppendText(typeOfDoc.DocTypeOfficialNumber);
                        firstCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                        WTableCell secondCell = row.Cells[1];
                        secondCell.Paragraphs.RemoveAt(0);
                        secondCell.AddParagraph().AppendText(typeOfDoc.DocTypeName);
                        secondCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                        WTableCell thirdCell = row.Cells[2];
                        thirdCell.Paragraphs.RemoveAt(0);
                        thirdCell.AddParagraph().AppendText(valueForDocseries);
                        thirdCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                        var fabricNumberValuesFromAlgorithm = item.Split(" -> ", StringSplitOptions.RemoveEmptyEntries).ToList();
                        WTableCell fourthCell = row.Cells[3];
                        fourthCell.Paragraphs.RemoveAt(0);
                        fourthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                        fourthCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                        WTableCell fifthCell = row.Cells[4];
                        fifthCell.Paragraphs.RemoveAt(0);
                        fifthCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                        int serialNumbersCount = 0;
                        if (resultList.Count == 1)
                        {
                            fifthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                            serialNumbersCount = 1;
                        }
                        else if (resultList.Count > 1)
                        {
                            if (fabricNumberValuesFromAlgorithm.Count == 1)
                            {
                                fifthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(item)));
                                serialNumbersCount = (int.Parse(item) - int.Parse(item)) + 1;
                            }
                            else
                            {
                                fifthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[1])));
                                serialNumbersCount = (int.Parse(fabricNumberValuesFromAlgorithm[1]) - int.Parse(fabricNumberValuesFromAlgorithm[0])) + 1;
                            }
                        }

                        if (serialNumbersCount == 0)
                        {
                            serialNumbersCount = 1;
                        }

                        WTableCell sixthCell = row.Cells[5];
                        sixthCell.Paragraphs.RemoveAt(0);
                        sixthCell.AddParagraph().AppendText(serialNumbersCount.ToString());
                        sixthCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                        table.Rows.Insert(counter++, row);
                    }
                }
                else
                {
                    WTableRow row = table.Rows[2].Clone();

                    WTableCell firstCell = row.Cells[0];
                    firstCell.Paragraphs.RemoveAt(0);
                    firstCell.AddParagraph().AppendText(typeOfDoc.DocTypeOfficialNumber);
                    firstCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                    WTableCell secondCell = row.Cells[1];
                    secondCell.Paragraphs.RemoveAt(0);
                    secondCell.AddParagraph().AppendText(typeOfDoc.DocTypeName);
                    secondCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                    WTableCell thirdCell = row.Cells[2];
                    thirdCell.Paragraphs.RemoveAt(0);
                    thirdCell.AddParagraph().AppendText(valueForDocseries);
                    thirdCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                    WTableCell sixthCell = row.Cells[5];
                    sixthCell.Paragraphs.RemoveAt(0);
                    sixthCell.AddParagraph().AppendText(reqDocManagement.DocumentCount.ToString());
                    sixthCell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

                    table.Rows.Insert(counter++, row);
                }
            }

            table.Rows.RemoveAt(counter);

            var providerLocation = reqDocManagement.CandidateProvider.IdLocationCorrespondence.HasValue ? await this.locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(reqDocManagement.CandidateProvider.IdLocationCorrespondence.Value) : new LocationVM();
            reqDocManagement.CandidateProvider.LocationCorrespondence = providerLocation;
            var providerPartnerLocation = reqDocManagement.CandidateProviderPartner.IdLocationCorrespondence.HasValue ? await this.locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(reqDocManagement.CandidateProviderPartner.IdLocationCorrespondence.Value) : new LocationVM();
            reqDocManagement.CandidateProviderPartner.LocationCorrespondence = providerPartnerLocation;

            string[] fieldNames = new string[]
            {
                "DocumentDate", "ProviderOwner", "ManagerName", "ProviderOwnerPartner", "LocationName", "MunicipalityName", "AreaName", "ManagerNamePartner"
            };

            string[] fieldValues = new string[]
            {
                reqDocManagement.DocumentDate.Value.ToString("dd.MM.yyyy"),
                $"ЦПО {reqDocManagement.CandidateProvider.ProviderName} към {reqDocManagement.CandidateProvider.ProviderOwner}",
                reqDocManagement.CandidateProvider.ManagerName,
                $"ЦПО {reqDocManagement.CandidateProviderPartner.ProviderName} към {reqDocManagement.CandidateProviderPartner.ProviderOwner}",
                reqDocManagement.CandidateProviderPartner.LocationCorrespondence.LocationName,
                reqDocManagement.CandidateProviderPartner.LocationCorrespondence.Municipality.MunicipalityName,
                reqDocManagement.CandidateProviderPartner.LocationCorrespondence.Municipality.District.DistrictName,
                reqDocManagement.CandidateProviderPartner.ManagerName
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            if (requestDocumentManagements.Any(x => x.TypeOfRequestedDocument.HasSerialNumber == false))
            {
                BookmarksNavigator bookmarksNavigator = new BookmarksNavigator(document);
                bookmarksNavigator.MoveToBookmark("FabricNumbers");
                bookmarksNavigator.DeleteBookmarkContent(true, true);
            }

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            document.Dispose();
            template.Close();

            return stream;
        }

        public async Task<RequestDocumentManagementVM> GetRequestDocumentManagementByIdAsync(int idRequestDocumentManagement)
        {
            var data = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdRequestDocumentManagement == idRequestDocumentManagement);

            return await data.To<RequestDocumentManagementVM>(x => x.TypeOfRequestedDocument, x => x.CandidateProvider, x => x.CandidateProviderPartner, x => x.ProviderRequestDocument, x => x.DocumentSerialNumbers, x => x.RequestDocumentManagementUploadedFiles).FirstOrDefaultAsync();
        }

        private async Task<IEnumerable<RequestDocumentManagementVM>> GetAllRequestDocumentManagementsAsync(int idCandidateProvider)
        {
            RequestDocumentManagementVM filter = new RequestDocumentManagementVM();

            IQueryable<RequestDocumentManagement> data = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdCandidateProvider == idCandidateProvider);

            return await data.To<RequestDocumentManagementVM>(x => x.TypeOfRequestedDocument, x => x.CandidateProvider, x => x.CandidateProviderPartner, x => x.ProviderRequestDocument, x => x.DocumentSerialNumbers).ToListAsync();
        }
        #endregion

        #region Request document management uploaded file
        public async Task<ResultContext<NoResult>> CreateRequestDocumentManagementUploadedFileByListRequestDocumentManagementAsync(List<RequestDocumentManagementVM> requestDocumentManagements, RequestDocumentManagementUploadedFileVM requestDocumentManagementUploadedFile)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var docManagement in requestDocumentManagements)
                {
                    var uploadedFile = await this.repository.AllReadonly<RequestDocumentManagementUploadedFile>(x => x.IdRequestDocumentManagement == docManagement.IdRequestDocumentManagement).FirstOrDefaultAsync();
                    if (uploadedFile is null)
                    {
                        RequestDocumentManagementUploadedFile requestDocumentManagement = new RequestDocumentManagementUploadedFile()
                        {
                            IdRequestDocumentManagement = docManagement.IdRequestDocumentManagement,
                            Description = requestDocumentManagementUploadedFile.Description,
                            UploadedFileName = string.Empty
                        };

                        await this.repository.AddAsync<RequestDocumentManagementUploadedFile>(requestDocumentManagement);
                    }
                    else
                    {
                        uploadedFile.Description = requestDocumentManagementUploadedFile.Description;

                        this.repository.Update<RequestDocumentManagementUploadedFile>(uploadedFile);
                    }
                }

                requestDocumentManagementUploadedFile.IdRequestDocumentManagementUploadedFile = 123321123;
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> CreateRequestDocumentManagementUploadedAsync(RequestDocumentManagementUploadedFileVM requestDocumentManagementUploadedFile)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var uploadedFileFromDb = await this.repository.AllReadonly<RequestDocumentManagementUploadedFile>(x => x.IdRequestDocumentManagement == requestDocumentManagementUploadedFile.IdRequestDocumentManagement).FirstOrDefaultAsync();
                if (uploadedFileFromDb is null)
                {
                    RequestDocumentManagementUploadedFile requestDocumentManagementUploaded = new RequestDocumentManagementUploadedFile()
                    {
                        IdRequestDocumentManagement = requestDocumentManagementUploadedFile.IdRequestDocumentManagement,
                        Description = requestDocumentManagementUploadedFile.Description,
                        UploadedFileName = string.Empty
                    };

                    await this.repository.AddAsync<RequestDocumentManagementUploadedFile>(requestDocumentManagementUploaded);
                    await this.repository.SaveChangesAsync();

                    requestDocumentManagementUploadedFile.IdRequestDocumentManagementUploadedFile = requestDocumentManagementUploaded.IdRequestDocumentManagementUploadedFile;
                }
                else
                {
                    uploadedFileFromDb.Description = requestDocumentManagementUploadedFile.Description;

                    this.repository.Update<RequestDocumentManagementUploadedFile>(uploadedFileFromDb);
                    await this.repository.SaveChangesAsync();

                    requestDocumentManagementUploadedFile.IdRequestDocumentManagementUploadedFile = uploadedFileFromDb.IdRequestDocumentManagementUploadedFile;
                }

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<RequestDocumentManagementUploadedFileVM> GetManagementUploadedFileByIdRequestDocumentManagementAsync(int idRequestDocumentManagement)
        {
            return await this.repository.AllReadonly<RequestDocumentManagementUploadedFile>(x => x.IdRequestDocumentManagement == idRequestDocumentManagement).To<RequestDocumentManagementUploadedFileVM>().FirstOrDefaultAsync();
        }
        #endregion

        #region Document series
        public async Task<IEnumerable<DocumentSeriesVM>> GetAllDocumentSeriesAsync()
        {
            var data = this.repository.AllReadonly<DocumentSeries>();

            return await data.To<DocumentSeriesVM>().ToListAsync();
        }
        public async Task<IEnumerable<DocumentSeriesVM>> GetAllDocumentSeriesIncludeAsync()
        {

            return await this.repository.All<DocumentSeries>().To<DocumentSeriesVM>(x => x.TypeOfRequestedDocument).OrderByDescending(x => x.Year).ThenBy(x => x.TypeOfRequestedDocument.Order).ToListAsync();
        }

        public async void SaveDocumentSeries(DocumentSeriesVM model)
        {
            var mappedModel = model.To<DocumentSeries>();
            mappedModel.TypeOfRequestedDocument = this.repository.All<TypeOfRequestedDocument>().Where(x => x.IdTypeOfRequestedDocument == model.IdTypeOfRequestedDocument).First();
            await this.repository.AddAsync(mappedModel);
            await this.repository.SaveChangesAsync();
        }
        public async Task<DocumentSeriesVM> GetDocumenSeriestByTypeAndYear(int IdTypeOfRequestedDocument, int? year)
        {
            var doc = await this.repository
                .All<DocumentSeries>()
                .Where(x => x.IdTypeOfRequestedDocument == IdTypeOfRequestedDocument && x.Year == year)
                .FirstOrDefaultAsync();

            if (doc != null)
                this.repository.Detach(doc);

            return doc == null ? null : doc.To<DocumentSeriesVM>();
        }

        public void UpdateDocumentSeries(DocumentSeriesVM model)
        {
            var doc = model.To<DocumentSeries>();

            this.repository.Detach(doc);

            this.repository.Update(doc);

            this.repository.SaveChanges();
        }
        #endregion

        #region Document serial number
        public async Task<IEnumerable<DocumentSerialNumberVM>> GetDocumentSerialNumbersWithOperationReceivedAndByTypeOfRequestedDocumentIdAndByIdCandidateProviderAsync(TypeOfRequestedDocumentVM typeOfRequestedDocument, int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdTypeOfRequestedDocument == typeOfRequestedDocument.IdTypeOfRequestedDocument && x.IdCandidateProvider == idCandidateProvider);
            var result = await data.To<DocumentSerialNumberVM>().ToListAsync();
            var serialNumberList = new List<DocumentSerialNumberVM>();
            //foreach (var serialNumber in result)
            //{
            //    if (serialNumber.IdDocumentOperation != this.kvReceived.IdKeyValue)
            //    {
            //        continue;
            //    }

            //    var searchedSerialNumber = result.Where(x => x.SerialNumber == serialNumber.SerialNumber);
            //    if (searchedSerialNumber.Count() == 1)
            //    {
            //        var serNum = searchedSerialNumber.FirstOrDefault();
            //        if (serNum.IdDocumentOperation == this.kvReceived.IdKeyValue)
            //        {
            //            serialNumberList.Add(serNum);
            //        }
            //    }
            //}

            foreach (var serialNumber in result)
            {
                if (!result.Any(x => x.SerialNumber == serialNumber.SerialNumber && x.IdDocumentOperation != this.kvReceived.IdKeyValue))
                {
                    serialNumberList.Add(serialNumber);
                }
            }

            return serialNumberList;
        }

        public async Task<IEnumerable<DocumentSerialNumberVM>> GetDocumentSerialNumbersWithOperationSubmittedAndOperationAwaitingConfirmationAndSerialNumberAsync(DocumentSerialNumberVM documentSerialNumberVM, int providerPartnerId)
        {
            var data = this.repository.AllReadonly<DocumentSerialNumber>(x => (x.IdDocumentOperation == this.kvSubmitted.IdKeyValue || x.IdDocumentOperation == this.kvAwaitingConfirmation.IdKeyValue)
                && x.IdTypeOfRequestedDocument == documentSerialNumberVM.IdTypeOfRequestedDocument
                && x.IdRequestDocumentManagement == documentSerialNumberVM.IdRequestDocumentManagement
                && x.SerialNumber == documentSerialNumberVM.SerialNumber
                && (x.IdCandidateProvider == documentSerialNumberVM.IdCandidateProvider || x.IdCandidateProvider == providerPartnerId));

            return await data.To<DocumentSerialNumberVM>().ToListAsync();
        }

        public async Task<IEnumerable<DocumentSerialNumberVM>> GetDocumentSerialNumbersWithOperationSubmittedByRequestDocumentManagementIdAndProviderIdAsync(DocumentSerialNumberVM documentSerialNumberVM)
        {
            var data = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdRequestDocumentManagement == documentSerialNumberVM.IdRequestDocumentManagement
                && x.IdCandidateProvider == documentSerialNumberVM.IdCandidateProvider
                && x.IdDocumentOperation == this.kvSubmitted.IdKeyValue);

            return await data.To<DocumentSerialNumberVM>().ToListAsync();
        }

        public async Task<ResultContext<DocumentSerialNumberVM>> DeleteDocumentSerialNumberAsync(ResultContext<DocumentSerialNumberVM> inputContext)
        {
            ResultContext<DocumentSerialNumberVM> resultContext = new ResultContext<DocumentSerialNumberVM>();
            var documentSerialNumberVM = inputContext.ResultContextObject;

            try
            {
                await this.repository.HardDeleteAsync<DocumentSerialNumber>(documentSerialNumberVM.IdDocumentSerialNumber);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е изтрит успешно!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<DocumentSerialNumberVM> GetDocumentSerialNumberByIdOperationAndIdDocumentSerialNumber(int idOperation, int idDocumentSerialNumber)
        {
            var docSerialNumber = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdDocumentSerialNumber == idDocumentSerialNumber && x.IdDocumentOperation == idOperation);

            return await docSerialNumber.To<DocumentSerialNumberVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<DocumentSerialNumberVM>> DeleteDocumentSerialNumbersByListIdsAsync(List<int> ids)
        {
            ResultContext<DocumentSerialNumberVM> resultContext = new ResultContext<DocumentSerialNumberVM>();

            try
            {
                foreach (var id in ids)
                {
                    await this.repository.HardDeleteAsync<DocumentSerialNumber>(id);
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е изтрит успешно!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<IEnumerable<DocumentSerialNumberVM>> FilterDocumentSerialNumbersAsync(NAPOODocumentSerialNumberFilterVM nAPOODocumentSerialNumberFilterVM)
        {
            var filter = PredicateBuilder.True<DocumentSerialNumber>();
            if (nAPOODocumentSerialNumberFilterVM.IdCandidateProvider != 0)
            {
                filter = filter.And(x => x.IdCandidateProvider == nAPOODocumentSerialNumberFilterVM.IdCandidateProvider);
            }

            if (!string.IsNullOrEmpty(nAPOODocumentSerialNumberFilterVM.SeriesName))
            {
                filter = filter.And(x => x.TypeOfRequestedDocument.DocumentSeries.Any(y => !string.IsNullOrEmpty(y.SeriesName) && y.SeriesName.ToLower().Contains(nAPOODocumentSerialNumberFilterVM.SeriesName.ToLower())));
            }

            if (!string.IsNullOrEmpty(nAPOODocumentSerialNumberFilterVM.SerialNumber))
            {
                filter = filter.And(x => x.SerialNumber.ToLower().Contains(nAPOODocumentSerialNumberFilterVM.SerialNumber.ToLower()));
            }

            if (nAPOODocumentSerialNumberFilterVM.IdTypeOfRequestedDocument != 0)
            {
                filter = filter.And(x => x.IdTypeOfRequestedDocument == nAPOODocumentSerialNumberFilterVM.IdTypeOfRequestedDocument);
            }

            if (nAPOODocumentSerialNumberFilterVM.DocumentYear.HasValue && nAPOODocumentSerialNumberFilterVM.DocumentYear.HasValue)
            {
                filter = filter.And(x => x.ReceiveDocumentYear == nAPOODocumentSerialNumberFilterVM.DocumentYear);
            }

            if (nAPOODocumentSerialNumberFilterVM.DocumentDate.HasValue)
            {
                filter = filter.And(x => x.DocumentDate.Date == nAPOODocumentSerialNumberFilterVM.DocumentDate.Value.Date);
            }

            if (nAPOODocumentSerialNumberFilterVM.IdDocumentOperation != 0)
            {
                filter = filter.And(x => x.IdDocumentOperation == nAPOODocumentSerialNumberFilterVM.IdDocumentOperation);
            }

            var documentSerialNumbers = await this.repository.AllReadonly<DocumentSerialNumber>(filter).To<DocumentSerialNumberVM>(x => x.CandidateProvider, 
                x => x.RequestDocumentManagement.CandidateProviderPartner, 
                x => x.RequestDocumentManagement.RequestDocumentManagementUploadedFiles,
                x => x.RequestReport.ReportUploadedDocs,
                x => x.TypeOfRequestedDocument.DocumentSeries,
                x => x.ClientCourseDocuments.Select(y => y.CourseDocumentUploadedFiles),
                x => x.ValidationClientDocuments.Select(y => y.ValidationDocumentUploadedFiles)).ToListAsync();

            return documentSerialNumbers.OrderByDescending(x => x.DocumentDate);
        }

        public async Task<string> GetClientNameByIdDocumentSerialNumberAsync(int idDocumentSerialNumber)
        {
            var data = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdDocumentSerialNumber == idDocumentSerialNumber).Include(x => x.ClientCourse).AsNoTracking().FirstOrDefaultAsync();
            if (data is not null && data.ClientCourse is not null)
            {
                return data.ClientCourse.To<ClientCourseVM>().FullName;
            }

            return string.Empty;
        }

        public async Task<IEnumerable<DocumentSerialNumberVM>> GetAllDocumentSerialNumbersByIdRequestDocumentManagementAsync(int idRequestDocumentManagement)
        {
            var data = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdRequestDocumentManagement == idRequestDocumentManagement);

            return await data.To<DocumentSerialNumberVM>().ToListAsync();
        }

        public async Task<ResultContext<NoResult>> AddConsecutiveDocumentSerialNumbersAsync(List<DocumentSerialNumberVM> documentSerials)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var docSerialNumber in documentSerials)
                {
                    var entryForDb = docSerialNumber.To<DocumentSerialNumber>();

                    await this.repository.AddAsync<DocumentSerialNumber>(entryForDb);
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<DocumentSerialNumberVM>> GetAllDocumentSerialNumbersByIdCandidateProviderAndStatusReceivedAndYear(CandidateProviderVM candidateProviderVM, int year)
        {
            var allDocumentSerialNumbers = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdCandidateProvider == candidateProviderVM.IdCandidate_Provider && x.ReceiveDocumentYear == year).ToList();
            var receivedDocumentSerialNumbers = allDocumentSerialNumbers.Where(x => x.IdDocumentOperation == this.kvReceived.IdKeyValue).ToList();
            List<DocumentSerialNumberVM> data = new List<DocumentSerialNumberVM>();
            foreach (var docSerialNumber in receivedDocumentSerialNumbers)
            {
                if (!allDocumentSerialNumbers.Any(x => x.SerialNumber == docSerialNumber.SerialNumber && x.IdDocumentOperation != this.kvReceived.IdKeyValue))
                {
                    var docSerialNumberAsVM = docSerialNumber.To<DocumentSerialNumberVM>();
                    var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == docSerialNumberAsVM.IdTypeOfRequestedDocument && x.Year == docSerialNumberAsVM.ReceiveDocumentYear).FirstOrDefaultAsync();
                    if (docSeries is not null)
                    {
                        docSerialNumberAsVM.DocumentSeriesName = docSeries.SeriesName;
                    }

                    data.Add(docSerialNumberAsVM);
                }
            }

            return data;
        }

        public IEnumerable<DocumentSerialNumberVM> GetAllDocumentSerialNumbersByIdCandidateProviderByStatusReceivedByYearAndByIdCourseType(CandidateProviderVM candidateProviderVM, int year, int idCourseType, bool isDuplicate = false, bool isDuplicateValidation = false)
        {
            var allDocumentSerialNumbers = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdCandidateProvider == candidateProviderVM.IdCandidate_Provider && x.ReceiveDocumentYear == year).Include(x => x.TypeOfRequestedDocument).ToList();
            var receivedDocumentSerialNumbers = allDocumentSerialNumbers.Where(x => x.IdDocumentOperation == this.kvReceived.IdKeyValue).ToList();
            List<DocumentSerialNumberVM> data = new List<DocumentSerialNumberVM>();
            foreach (var docSerialNumber in receivedDocumentSerialNumbers)
            {
                var otherOperationDoc = allDocumentSerialNumbers.FirstOrDefault(x => x.SerialNumber == docSerialNumber.SerialNumber && x.IdDocumentOperation != this.kvReceived.IdKeyValue && x.TypeOfRequestedDocument.IdTypeOfRequestedDocument == docSerialNumber.TypeOfRequestedDocument.IdTypeOfRequestedDocument);
                if (otherOperationDoc == null)
                {
                    data.Add(docSerialNumber.To<DocumentSerialNumberVM>());
                }
            }

            if (!isDuplicateValidation)
            {
                data = !isDuplicate ? data.Where(x => x.TypeOfRequestedDocument.IdCourseType == idCourseType).ToList() : data.Where(x => x.TypeOfRequestedDocument.DocTypeOfficialNumber == "3-54a").ToList();
            }
            else
            {
                data = data.Where(x => x.TypeOfRequestedDocument.DocTypeOfficialNumber == "3-54аВ").ToList();
            }

            return data;
        }

        public async Task<DocumentSerialNumberVM> GetDocumentSerialNumberByIdAndYearAsync(int idDocumentSerialNumber, int year)
        {
            var data = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdDocumentSerialNumber == idDocumentSerialNumber && x.ReceiveDocumentYear == year);

            return await data.To<DocumentSerialNumberVM>(x => x.TypeOfRequestedDocument).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DocumentSerialNumberVM>> GetAllDocumentSerialNumbersByIdCandidateProviderAndStatusSubmittedAndYear(CandidateProviderVM candidateProviderVM, int year)
        {
            var allDocumentSerialNumbers = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdCandidateProvider == candidateProviderVM.IdCandidate_Provider && x.ReceiveDocumentYear == year && x.IdDocumentOperation == this.kvSubmitted.IdKeyValue);
            var allDocumentSerialNumbersAsVM = await allDocumentSerialNumbers.To<DocumentSerialNumberVM>(x => x.TypeOfRequestedDocument).ToListAsync();
            foreach (var doc in allDocumentSerialNumbersAsVM)
            {
                var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == doc.IdTypeOfRequestedDocument && x.Year == doc.ReceiveDocumentYear).FirstOrDefaultAsync();
                if (docSeries is not null)
                {
                    doc.DocumentSeriesName = docSeries.SeriesName;
                }

                doc.DocumentOperationName = this.kvSubmitted.Name;
            }

            return allDocumentSerialNumbersAsVM;
        }

        public async Task<IEnumerable<DocumentSerialNumberVM>> GetAllDocumentSerialNumbersByIdCandidateProviderAndStatusPrintedAndYear(CandidateProviderVM candidateProviderVM, int year)
        {
            var allDocumentSerialNumbers = this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdCandidateProvider == candidateProviderVM.IdCandidate_Provider && x.ReceiveDocumentYear == year && x.IdDocumentOperation == this.kvPrinted.IdKeyValue);
            var allDocumentSerialNumbersAsVM = await allDocumentSerialNumbers.To<DocumentSerialNumberVM>(x => x.TypeOfRequestedDocument).ToListAsync();
            foreach (var doc in allDocumentSerialNumbersAsVM)
            {
                var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == doc.IdTypeOfRequestedDocument && x.Year == doc.ReceiveDocumentYear).FirstOrDefaultAsync();
                if (docSeries is not null)
                {
                    doc.DocumentSeriesName = docSeries.SeriesName;
                }

                doc.DocumentOperationName = this.kvPrinted.Name;
            }

            return allDocumentSerialNumbersAsVM;
        }

        #endregion

        #region Request report
        public async Task<IEnumerable<RequestReportVM>> GetAllRequestReportsByCandidateProviderIdAsync(CandidateProviderVM candidateProviderVM)
        {
            var data = this.repository.AllReadonly<RequestReport>(x => x.IdCandidateProvider == candidateProviderVM.IdCandidate_Provider);

            return await data.To<RequestReportVM>(x => x.CandidateProvider, x => x.DocumentSerialNumbers).ToListAsync();
        }

        public async Task<IEnumerable<RequestReportVM>> GetAllRequestReportsAsync()
        {
            var data = this.repository.AllReadonly<RequestReport>();

            return await data.To<RequestReportVM>(x => x.CandidateProvider, x => x.DocumentSerialNumbers).ToListAsync();
        }

        public async Task<RequestReportVM> GetRequestReportByIdAsync(RequestReportVM requestReportVM)
        {
            var data = this.repository.AllReadonly<RequestReport>(x => x.IdRequestReport == requestReportVM.IdRequestReport);

            return await data.To<RequestReportVM>(x => x.CandidateProvider, x => x.DocumentSerialNumbers, x => x.ReportUploadedDocs).FirstOrDefaultAsync();
        }

        public async Task<ResultContext<RequestReportVM>> CreateRequestReportAsync(ResultContext<RequestReportVM> inputContext)
        {
            ResultContext<RequestReportVM> resultContext = new ResultContext<RequestReportVM>();

            var requestReportVM = inputContext.ResultContextObject;

            try
            {
                var requestReportForDb = requestReportVM.To<RequestReport>();
                requestReportForDb.IdStatus = this.kvRequestReportCreated.IdKeyValue;
                requestReportForDb.CandidateProvider = null;
                requestReportForDb.ReportUploadedDocs = null;

                await this.repository.AddAsync<RequestReport>(requestReportForDb);
                await this.repository.SaveChangesAsync();

                requestReportVM.IdRequestReport = requestReportForDb.IdRequestReport;
                resultContext.ResultContextObject = requestReportVM;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<ResultContext<RequestReportVM>> UpdateRequestReportAsync(ResultContext<RequestReportVM> inputContext)
        {
            ResultContext<RequestReportVM> resultContext = new ResultContext<RequestReportVM>();

            var requestReportVM = inputContext.ResultContextObject;

            try
            {
                var requestReportForDb = await this.repository.GetByIdAsync<RequestReport>(requestReportVM.IdRequestReport);
                requestReportVM.IdCreateUser = requestReportForDb.IdCreateUser;
                requestReportVM.CreationDate = requestReportForDb.CreationDate;
                requestReportForDb = requestReportVM.To<RequestReport>();
                requestReportForDb.CandidateProvider = null;
                requestReportForDb.DocumentSerialNumbers = null;
                requestReportForDb.ReportUploadedDocs = null;

                this.repository.Update<RequestReport>(requestReportForDb);
                await this.repository.SaveChangesAsync();

                requestReportVM.IdRequestReport = requestReportForDb.IdRequestReport;

                await this.HandleDocumentSerialNumbersToDeleteAndCancelAsync(requestReportVM, requestReportForDb);

                resultContext.ResultContextObject = requestReportVM;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        private async Task HandleDocumentSerialNumbersToDeleteAndCancelAsync(RequestReportVM requestReportVM, RequestReport requestReportForDb)
        {
            foreach (var docSerialNumber in requestReportVM.DocumentSerialNumbers.OrderBy(x => x.IdDocumentOperation))
            {
                int idOperationType = 0;

                if (docSerialNumber.IdDocumentOperation == this.kvCancelled.IdKeyValue)
                {
                    idOperationType = this.kvCancelled.IdKeyValue;
                }
                else if (docSerialNumber.IdDocumentOperation == this.kvDestroyed.IdKeyValue)
                {
                    idOperationType = this.kvDestroyed.IdKeyValue;
                }

                var requestDocumentManagement = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdCandidateProvider == requestReportForDb.IdCandidateProvider
                    && x.IdCandidateProviderPartner == null
                    && x.IdProviderRequestDocument == null
                    && x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument
                    && x.IdDocumentOperation == idOperationType
                    && x.DocumentDate == requestReportForDb.DestructionDate
                    && x.ReceiveDocumentYear == docSerialNumber.ReceiveDocumentYear).FirstOrDefault();

                if (requestDocumentManagement is null)
                {
                    var requestDocumentManagementForDb = new RequestDocumentManagement()
                    {
                        IdCandidateProvider = requestReportForDb.IdCandidateProvider,
                        IdTypeOfRequestedDocument = docSerialNumber.IdTypeOfRequestedDocument,
                        DocumentCount = requestReportVM.DocumentSerialNumbers.Count,
                        DocumentDate = requestReportForDb.DestructionDate,
                        IdDocumentOperation = idOperationType,
                        ReceiveDocumentYear = docSerialNumber.ReceiveDocumentYear
                    };

                    await this.repository.AddAsync<RequestDocumentManagement>(requestDocumentManagementForDb);
                    await this.repository.SaveChangesAsync();

                    requestDocumentManagement = requestDocumentManagementForDb;
                }
                else
                {
                    var docCount = requestReportVM.DocumentSerialNumbers.Where(x => x.IdDocumentOperation == idOperationType).Count();
                    if (requestDocumentManagement.DocumentCount != docCount)
                    {
                        requestDocumentManagement.DocumentCount = docCount;
                        this.repository.Update<RequestDocumentManagement>(requestDocumentManagement);
                        await this.repository.SaveChangesAsync();
                    }
                }

                var documentSerialNumberFromDb = this.repository.AllReadonly<DocumentSerialNumber>(x => x.SerialNumber == docSerialNumber.SerialNumber
                        && x.ReceiveDocumentYear == docSerialNumber.ReceiveDocumentYear
                        && x.IdRequestDocumentManagement == requestDocumentManagement.IdRequestDocumentManagement
                        && x.IdDocumentOperation == idOperationType
                        && x.IdCandidateProvider == requestDocumentManagement.IdCandidateProvider
                        && x.IdTypeOfRequestedDocument == requestDocumentManagement.IdTypeOfRequestedDocument).FirstOrDefault();

                if (documentSerialNumberFromDb is null)
                {
                    var documentSerialNumberForDb = docSerialNumber.To<DocumentSerialNumber>();
                    documentSerialNumberForDb.IdDocumentSerialNumber = 0;
                    documentSerialNumberForDb.TypeOfRequestedDocument = null;
                    documentSerialNumberForDb.RequestReport = null;
                    documentSerialNumberForDb.CandidateProvider = null;
                    documentSerialNumberForDb.RequestDocumentManagement = null;
                    documentSerialNumberForDb.IdRequestReport = requestReportForDb.IdRequestReport;
                    documentSerialNumberForDb.IdRequestDocumentManagement = requestDocumentManagement.IdRequestDocumentManagement;

                    await this.repository.AddAsync<DocumentSerialNumber>(documentSerialNumberForDb);
                    await this.repository.SaveChangesAsync();

                    docSerialNumber.IdDocumentSerialNumber = documentSerialNumberForDb.IdDocumentSerialNumber;
                }
            }
        }

        public async Task<ResultContext<RequestReportVM>> FileInProviderRequestReportAsync(ResultContext<RequestReportVM> inputContext)
        {
            ResultContext<RequestReportVM> resultContext = new ResultContext<RequestReportVM>();

            var requestReportVM = inputContext.ResultContextObject;

            try
            {
                var requestReportFromDb = await this.repository.GetByIdAsync<RequestReport>(requestReportVM.IdRequestReport);
                requestReportVM.IdCreateUser = requestReportFromDb.IdCreateUser;
                requestReportVM.CreationDate = requestReportFromDb.CreationDate;
                requestReportFromDb = requestReportVM.To<RequestReport>();
                requestReportFromDb.IdStatus = this.kvRequestReportSubmitted.IdKeyValue;

                this.repository.Update<RequestReport>(requestReportFromDb);
                await this.repository.SaveChangesAsync();

                requestReportVM.IdStatus = requestReportFromDb.IdStatus;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public MemoryStream PrintProtocol(RequestReportVM requestReportVM, List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource)
        {
            var documentSeries = this.repository.AllReadonly<DocumentSeries>(x => x.Year == requestReportVM.Year).To<DocumentSeriesVM>().ToList();

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\RequestDocument";

            FileStream template = new FileStream($@"{resources_Folder}\Protokol_za_unishtojavane.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Doc);
            WSection section = document.Sections[0];

            WTable table = section.Tables[0] as WTable;

            var typeOfDocsAsDict = new Dictionary<int, List<string>>();

            foreach (var doc in requestReportVM.DocumentSerialNumbers.OrderBy(x => x.SerialNumberAsIntForOrderBy))
            {
                if (!typeOfDocsAsDict.ContainsKey(doc.IdTypeOfRequestedDocument))
                {
                    typeOfDocsAsDict.Add(doc.IdTypeOfRequestedDocument, new List<string>());
                }

                typeOfDocsAsDict[doc.IdTypeOfRequestedDocument].Add(doc.SerialNumber);
            }

            var counter = 2;
            foreach (var entry in typeOfDocsAsDict)
            {
                var resultList = this.CalculateConsecutiveFabricNumbers(entry.Value.Select(int.Parse).ToList());
                var typeOfDoc = typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key);
                var docSeries = documentSeries.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key && x.Year == requestReportVM.Year);
                var valueForDocseries = string.Empty;
                if (docSeries is not null)
                {
                    valueForDocseries = docSeries.SeriesName;
                }
                else
                {
                    valueForDocseries = string.Empty;
                }

                foreach (var item in resultList)
                {
                    WTableRow row = table.Rows[1].Clone();

                    WTableCell firstCell = row.Cells[0];
                    firstCell.Paragraphs.RemoveAt(0);
                    firstCell.AddParagraph().AppendText(typeOfDoc.DocTypeOfficialNumber);

                    WTableCell secondCell = row.Cells[1];
                    secondCell.Paragraphs.RemoveAt(0);
                    secondCell.AddParagraph().AppendText(valueForDocseries);

                    var fabricNumberValuesFromAlgorithm = item.Split(" -> ", StringSplitOptions.RemoveEmptyEntries).ToList();
                    WTableCell thirdCell = row.Cells[2];
                    thirdCell.Paragraphs.RemoveAt(0);
                    thirdCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));

                    int serialNumbersCount = 0;
                    WTableCell fifthCell = row.Cells[4];
                    fifthCell.Paragraphs.RemoveAt(0);
                    if (fabricNumberValuesFromAlgorithm.Count > 1)
                    {
                        WTableCell fourthCell = row.Cells[3];
                        fourthCell.Paragraphs.RemoveAt(0);
                        fourthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[1])));
                        serialNumbersCount = (int.Parse(fabricNumberValuesFromAlgorithm[1]) - int.Parse(fabricNumberValuesFromAlgorithm[0])) + 1;
                    }
                    else
                    {
                        WTableCell fourthCell = row.Cells[3];
                        fourthCell.Paragraphs.RemoveAt(0);
                        fourthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(item)));
                        serialNumbersCount = (int.Parse(item) - int.Parse(item)) + 1;
                    }

                    if (serialNumbersCount == 0)
                    {
                        serialNumbersCount = 1;
                    }

                    fifthCell.AddParagraph().AppendText(serialNumbersCount.ToString());

                    table.Rows.Insert(counter++, row);
                }

                BookmarksNavigator bookNav = new BookmarksNavigator(document);
                bookNav.MoveToBookmark("FabricNumbers");

                IWParagraph paragraph = new WParagraph(document);
                paragraph.AppendText($"Документи с фабрична номерация, номенклатурен номер {typeOfDoc.DocTypeOfficialNumber}, {entry.Value.Count} бр.:");
                bookNav.InsertParagraph(paragraph);

                IWTable newTable = new WTable(document, true);
                WTableRow newRow = newTable.AddRow();
                newRow.Height = 65;
                WTableCell newFirstCell = newRow.AddCell();
                WTableCell newSecondCell = newRow.AddCell();
                WTableCell newThirdCell = newRow.AddCell();

                var cellCounter = 1;
                var docCounter = 1;
                foreach (var docSerialNumber in entry.Value)
                {
                    if (cellCounter > 3)
                    {
                        newRow = newTable.AddRow();
                        newRow.Height = 65;
                        cellCounter = 1;
                    }

                    newRow.Cells[cellCounter - 1].AddParagraph().AppendText($"{docCounter++}. серия {valueForDocseries}, № {docSerialNumber}");
                    cellCounter++;
                }

                bookNav.InsertTable(newTable);
            }

            table.Rows.RemoveAt(1);


            string[] fieldNames = new string[]
            {
                "Date", "SchoolYear"
            };

            string[] fieldValues = new string[]
            {
                requestReportVM.DestructionDate.Value.ToString("dd.MM.yyyy"),
                $"{requestReportVM.Year}/{requestReportVM.Year + 1}"
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            document.Dispose();
            template.Close();

            return stream;
        }

        public async Task<MemoryStream> PrintReportAsync(RequestReportVM requestReportVM, List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource)
        {
            var documentSeries = this.repository.AllReadonly<DocumentSeries>(x => x.Year == requestReportVM.Year).To<DocumentSeriesVM>().ToList();

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\RequestDocument";

            FileStream template = new FileStream($@"{resources_Folder}\Otchet_za_unishtojavane.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Doc);
            WSection section = document.Sections[0];

            var documentManagementsData = this.repository.AllReadonly<RequestDocumentManagement>(x => x.IdCandidateProvider == requestReportVM.IdCandidateProvider
                    && x.ReceiveDocumentYear == requestReportVM.Year).To<RequestDocumentManagementVM>(x => x.DocumentSerialNumbers).ToList();
            var documentManagementsSourceReceivedFromMON = documentManagementsData.Where(x => x.IdDocumentRequestReceiveType == this.kvDocumentReceivedFromMON.IdKeyValue).ToList();
            var documentManagementsSourceReceivedFromOtherCPO = documentManagementsData.Where(x => x.IdDocumentRequestReceiveType == this.kvDocumentReceivedFromOtherCPO.IdKeyValue).ToList();
            var documentManagementsSourceHandedOver = documentManagementsData.Where(x => x.IdDocumentRequestReceiveType == this.kvSubmitted.IdKeyValue).ToList();
            var allDocSerialNumbers = new List<DocumentSerialNumberVM>();
            foreach (var entry in documentManagementsData)
            {
                allDocSerialNumbers.AddRange(entry.DocumentSerialNumbers);
            }

            var documentSerialNumbersReceivedFromMON = new List<DocumentSerialNumberVM>();
            foreach (var entry in documentManagementsSourceReceivedFromMON)
            {
                documentSerialNumbersReceivedFromMON.AddRange(entry.DocumentSerialNumbers);
            }

            var documentSerialNumbersReceivedFromOtherCPO = new List<DocumentSerialNumberVM>();
            foreach (var entry in documentManagementsSourceReceivedFromOtherCPO)
            {
                documentSerialNumbersReceivedFromOtherCPO.AddRange(entry.DocumentSerialNumbers);
            }

            var documentSerialNumbersHandedOver = new List<DocumentSerialNumberVM>();
            foreach (var entry in documentManagementsSourceHandedOver)
            {
                documentSerialNumbersHandedOver.AddRange(entry.DocumentSerialNumbers);
            }

            WTable tableReceived = section.Tables[0] as WTable;

            var typeOfReceivedDocsAsDict = new Dictionary<int, List<string>>();

            foreach (var entry in documentManagementsSourceReceivedFromMON)
            {
                foreach (var doc in entry.DocumentSerialNumbers.OrderBy(x => x.SerialNumberAsIntForOrderBy))
                {
                    if (!typeOfReceivedDocsAsDict.ContainsKey(doc.IdTypeOfRequestedDocument))
                    {
                        typeOfReceivedDocsAsDict.Add(doc.IdTypeOfRequestedDocument, new List<string>());
                    }

                    if (!typeOfReceivedDocsAsDict[doc.IdTypeOfRequestedDocument].Contains(doc.SerialNumber))
                    {
                        typeOfReceivedDocsAsDict[doc.IdTypeOfRequestedDocument].Add(doc.SerialNumber);
                    }
                }
            }

            var counter = 4;
            foreach (var entry in typeOfReceivedDocsAsDict)
            {
                var resultList = this.CalculateConsecutiveFabricNumbers(entry.Value.Select(int.Parse).ToList());
                var typeOfDoc = typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key);
                var docSeries = documentSeries.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key && x.Year == requestReportVM.Year);
                var valueForDocseries = string.Empty;
                if (docSeries is not null)
                {
                    valueForDocseries = docSeries.SeriesName;
                }
                else
                {
                    valueForDocseries = string.Empty;
                }

                foreach (var item in resultList)
                {
                    WTableRow row = tableReceived.Rows[3].Clone();

                    WTableCell firstCell = row.Cells[0];
                    firstCell.Paragraphs.RemoveAt(0);
                    firstCell.AddParagraph().AppendText(typeOfDoc.DocTypeOfficialNumber);

                    WTableCell secondCell = row.Cells[1];
                    secondCell.Paragraphs.RemoveAt(0);
                    secondCell.AddParagraph().AppendText(typeOfDoc.DocTypeName);

                    var documentManagement = documentManagementsSourceReceivedFromMON.LastOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key);

                    WTableCell fourthCell = row.Cells[3];
                    fourthCell.Paragraphs.RemoveAt(0);
                    fourthCell.AddParagraph().AppendText(valueForDocseries);

                    var fabricNumberValuesFromAlgorithm = item.Split(" -> ", StringSplitOptions.RemoveEmptyEntries).ToList();
                    WTableCell fifthCell = row.Cells[4];
                    fifthCell.Paragraphs.RemoveAt(0);

                    WTableCell sixthCell = row.Cells[5];
                    sixthCell.Paragraphs.RemoveAt(0);
                    int serialNumbersCount = 1;
                    if (fabricNumberValuesFromAlgorithm.Count == 1)
                    {
                        fifthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                        sixthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                    }
                    else if (fabricNumberValuesFromAlgorithm.Count > 1)
                    {
                        fifthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                        sixthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[1])));
                        serialNumbersCount += (int.Parse(fabricNumberValuesFromAlgorithm[1]) - int.Parse(fabricNumberValuesFromAlgorithm[0]));
                    }

                    WTableCell thirdCell = row.Cells[2];
                    thirdCell.Paragraphs.RemoveAt(0);
                    thirdCell.AddParagraph().AppendText(serialNumbersCount.ToString());

                    WTableCell seventhCell = row.Cells[6];
                    seventhCell.Paragraphs.RemoveAt(0);
                    seventhCell.AddParagraph().AppendText(serialNumbersCount.ToString());

                    WTableCell eigthCell = row.Cells[7];

                    WTableCell ninthCell = row.Cells[8];

                    WTableCell tenthCell = row.Cells[9];

                    if (fabricNumberValuesFromAlgorithm.Count > 1)
                    {
                        var cancelledCount = 0;
                        var destroyedCount = 0;
                        var printedCount = 0;
                        var difference = int.Parse(fabricNumberValuesFromAlgorithm[1]) - int.Parse(fabricNumberValuesFromAlgorithm[0]);
                        for (int i = 0; i <= difference; i++)
                        {
                            var serialNumber = int.Parse(fabricNumberValuesFromAlgorithm[0]) + i;
                            var documentSerialNumber = allDocSerialNumbers.LastOrDefault(x => x.SerialNumber == entry.Value.FirstOrDefault(x => x.Contains(serialNumber.ToString())));
                            if (documentSerialNumber.IdDocumentOperation == this.kvCancelled.IdKeyValue)
                            {
                                cancelledCount++;
                            }
                            else if (documentSerialNumber.IdDocumentOperation == this.kvDestroyed.IdKeyValue)
                            {
                                destroyedCount++;
                            }
                            else if (documentSerialNumber.IdDocumentOperation == this.kvPrinted.IdKeyValue)
                            {
                                printedCount++;
                            }
                        }

                        eigthCell.Paragraphs[0].Text = printedCount.ToString();
                        ninthCell.Paragraphs[0].Text = cancelledCount.ToString();
                        tenthCell.Paragraphs[0].Text = destroyedCount.ToString();
                    }
                    else
                    {
                        var docSerialNumber = allDocSerialNumbers.LastOrDefault(x => x.SerialNumber == entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                        if (docSerialNumber.IdDocumentOperation == this.kvCancelled.IdKeyValue)
                        {
                            ninthCell.Paragraphs[0].Text = "1";
                            tenthCell.Paragraphs[0].Text = "0";
                        }
                        else if (docSerialNumber.IdDocumentOperation == this.kvDestroyed.IdKeyValue)
                        {
                            ninthCell.Paragraphs[0].Text = "0";
                            tenthCell.Paragraphs[0].Text = "1";
                        }
                        else if (docSerialNumber.IdDocumentOperation == this.kvPrinted.IdKeyValue)
                        {
                            eigthCell.Paragraphs[0].Text = "1";
                        }
                        else
                        {
                            eigthCell.Paragraphs[0].Text = "0";
                            ninthCell.Paragraphs[0].Text = "0";
                            tenthCell.Paragraphs[0].Text = "0";
                        }
                    }

                    WTableCell eleventhCell = row.Cells[10];
                    eleventhCell.Paragraphs.RemoveAt(0);
                    var totalDestroyed = int.Parse(row.Cells[8].Paragraphs[0].Text.ToString()) + int.Parse(row.Cells[9].Paragraphs[0].Text.ToString());
                    eleventhCell.AddParagraph().AppendText(totalDestroyed.ToString());

                    WTableCell twelvethCell = row.Cells[11];
                    twelvethCell.Paragraphs.RemoveAt(0);

                    tableReceived.Rows.Insert(counter++, row);
                }
            }

            tableReceived.Rows.RemoveAt(3);

            tableReceived = section.Tables[1] as WTable;

            typeOfReceivedDocsAsDict = new Dictionary<int, List<string>>();

            foreach (var entry in documentManagementsSourceReceivedFromOtherCPO)
            {
                foreach (var doc in entry.DocumentSerialNumbers.OrderBy(x => x.SerialNumberAsIntForOrderBy))
                {
                    if (!typeOfReceivedDocsAsDict.ContainsKey(doc.IdTypeOfRequestedDocument))
                    {
                        typeOfReceivedDocsAsDict.Add(doc.IdTypeOfRequestedDocument, new List<string>());
                    }

                    typeOfReceivedDocsAsDict[doc.IdTypeOfRequestedDocument].Add(doc.SerialNumber);
                }
            }

            counter = 4;
            foreach (var entry in typeOfReceivedDocsAsDict)
            {
                var resultList = this.CalculateConsecutiveFabricNumbers(entry.Value.Select(int.Parse).ToList());
                var typeOfDoc = typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key);
                var docSeries = documentSeries.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key && x.Year == requestReportVM.Year);
                var valueForDocseries = string.Empty;
                if (docSeries is not null)
                {
                    valueForDocseries = docSeries.SeriesName;
                }
                else
                {
                    valueForDocseries = string.Empty;
                }

                foreach (var item in resultList)
                {
                    WTableRow row = tableReceived.Rows[3].Clone();

                    WTableCell firstCell = row.Cells[0];
                    firstCell.Paragraphs.RemoveAt(0);
                    firstCell.AddParagraph().AppendText(typeOfDoc.DocTypeOfficialNumber);

                    WTableCell secondCell = row.Cells[1];
                    secondCell.Paragraphs.RemoveAt(0);
                    secondCell.AddParagraph().AppendText(typeOfDoc.DocTypeName);

                    var documentManagement = documentManagementsSourceReceivedFromOtherCPO.LastOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key);

                    WTableCell fourthCell = row.Cells[2];
                    fourthCell.Paragraphs.RemoveAt(0);
                    fourthCell.AddParagraph().AppendText(valueForDocseries);

                    var fabricNumberValuesFromAlgorithm = item.Split(" -> ", StringSplitOptions.RemoveEmptyEntries).ToList();
                    WTableCell fifthCell = row.Cells[3];
                    fifthCell.Paragraphs.RemoveAt(0);

                    WTableCell sixthCell = row.Cells[4];
                    sixthCell.Paragraphs.RemoveAt(0);
                    int serialNumbersCount = 1;
                    if (fabricNumberValuesFromAlgorithm.Count == 1)
                    {
                        fifthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                        sixthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                    }
                    else if (fabricNumberValuesFromAlgorithm.Count > 1)
                    {
                        fifthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                        sixthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[1])));
                        serialNumbersCount += (int.Parse(fabricNumberValuesFromAlgorithm[1]) - int.Parse(fabricNumberValuesFromAlgorithm[0]));
                    }

                    WTableCell seventhCell = row.Cells[5];
                    seventhCell.Paragraphs.RemoveAt(0);
                    seventhCell.AddParagraph().AppendText(serialNumbersCount.ToString());

                    WTableCell eigthCell = row.Cells[6];

                    WTableCell ninthCell = row.Cells[7];
                    //ninthCell.Paragraphs.RemoveAt(0);

                    WTableCell tenthCell = row.Cells[8];
                    //tenthCell.Paragraphs.RemoveAt(0);

                    if (fabricNumberValuesFromAlgorithm.Count > 1)
                    {
                        var cancelledCount = 0;
                        var destroyedCount = 0;
                        var printedCount = 0;
                        var difference = int.Parse(fabricNumberValuesFromAlgorithm[1]) - int.Parse(fabricNumberValuesFromAlgorithm[0]);
                        for (int i = 0; i < difference; i++)
                        {
                            var serialNumber = int.Parse(fabricNumberValuesFromAlgorithm[0]) + i;
                            var documentSerialNumber = allDocSerialNumbers.LastOrDefault(x => x.SerialNumber == entry.Value.FirstOrDefault(x => x.Contains(serialNumber.ToString())));
                            if (documentSerialNumber.IdDocumentOperation == this.kvCancelled.IdKeyValue)
                            {
                                cancelledCount++;
                            }
                            else if (documentSerialNumber.IdDocumentOperation == this.kvDestroyed.IdKeyValue)
                            {
                                destroyedCount++;
                            }
                            else if (documentSerialNumber.IdDocumentOperation == this.kvPrinted.IdKeyValue)
                            {
                                printedCount++;
                            }
                            else
                            {
                                //ninthCell.AddParagraph().AppendText("0");
                                //tenthCell.AddParagraph().AppendText("0");
                                ninthCell.Paragraphs[0].Text = "0";
                                tenthCell.Paragraphs[0].Text = "1";
                            }
                        }

                        //ninthCell.AddParagraph().AppendText(cancelledCount.ToString());
                        //tenthCell.AddParagraph().AppendText(destroyedCount.ToString());
                        eigthCell.Paragraphs[0].Text = printedCount.ToString();
                        ninthCell.Paragraphs[0].Text = cancelledCount.ToString();
                        tenthCell.Paragraphs[0].Text = destroyedCount.ToString();
                    }
                    else
                    {
                        var docSerialNumber = allDocSerialNumbers.LastOrDefault(x => x.SerialNumber == entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                        if (docSerialNumber.IdDocumentOperation == this.kvCancelled.IdKeyValue)
                        {
                            //ninthCell.AddParagraph().AppendText("1");
                            //tenthCell.AddParagraph().AppendText("0");
                            ninthCell.Paragraphs[0].Text = "1";
                            tenthCell.Paragraphs[0].Text = "0";
                        }
                        else if (docSerialNumber.IdDocumentOperation == this.kvDestroyed.IdKeyValue)
                        {
                            //ninthCell.AddParagraph().AppendText("0");
                            //tenthCell.AddParagraph().AppendText("1");
                            ninthCell.Paragraphs[0].Text = "0";
                            tenthCell.Paragraphs[0].Text = "1";
                        }
                        else if (docSerialNumber.IdDocumentOperation == this.kvPrinted.IdKeyValue)
                        {
                            eigthCell.Paragraphs[0].Text = "1";
                        }
                        else
                        {
                            //ninthCell.AddParagraph().AppendText("0");
                            //tenthCell.AddParagraph().AppendText("0");
                            eigthCell.Paragraphs[0].Text = "0";
                            ninthCell.Paragraphs[0].Text = "0";
                            tenthCell.Paragraphs[0].Text = "1";
                        }
                    }

                    WTableCell eleventhCell = row.Cells[9];
                    eleventhCell.Paragraphs.RemoveAt(0);
                    var firstValue = !string.IsNullOrEmpty(row.Cells[7].Paragraphs[0].Text) ? int.Parse(row.Cells[7].Paragraphs[0].Text.ToString()) : 0;
                    var secondValue = !string.IsNullOrEmpty(row.Cells[8].Paragraphs[0].Text) ? int.Parse(row.Cells[8].Paragraphs[0].Text.ToString()) : 0;
                    var totalDestroyed = firstValue + secondValue;
                    eleventhCell.AddParagraph().AppendText(totalDestroyed.ToString());

                    WTableCell twelvethCell = row.Cells[10];
                    twelvethCell.Paragraphs.RemoveAt(0);

                    tableReceived.Rows.Insert(counter++, row);
                }
            }

            tableReceived.Rows.RemoveAt(3);

            WTable handedOverTable = section.Tables[2] as WTable;

            typeOfReceivedDocsAsDict = new Dictionary<int, List<string>>();

            foreach (var entry in documentManagementsSourceHandedOver)
            {
                foreach (var doc in entry.DocumentSerialNumbers.OrderBy(x => x.SerialNumberAsIntForOrderBy))
                {
                    if (!typeOfReceivedDocsAsDict.ContainsKey(doc.IdTypeOfRequestedDocument))
                    {
                        typeOfReceivedDocsAsDict.Add(doc.IdTypeOfRequestedDocument, new List<string>());
                    }

                    typeOfReceivedDocsAsDict[doc.IdTypeOfRequestedDocument].Add(doc.SerialNumber);
                }
            }

            counter = 4;
            foreach (var entry in typeOfReceivedDocsAsDict)
            {
                var resultList = this.CalculateConsecutiveFabricNumbers(entry.Value.Select(int.Parse).ToList());
                var typeOfDoc = typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key);
                var docSeries = documentSeries.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key && x.Year == requestReportVM.Year);
                var valueForDocseries = string.Empty;
                if (docSeries is not null)
                {
                    valueForDocseries = docSeries.SeriesName;
                }
                else
                {
                    valueForDocseries = string.Empty;
                }

                foreach (var item in resultList)
                {
                    WTableRow row = tableReceived.Rows[3].Clone();

                    WTableCell firstCell = row.Cells[0];
                    firstCell.Paragraphs.RemoveAt(0);
                    firstCell.AddParagraph().AppendText(typeOfDoc.DocTypeOfficialNumber);

                    WTableCell secondCell = row.Cells[1];
                    secondCell.Paragraphs.RemoveAt(0);
                    secondCell.AddParagraph().AppendText(typeOfDoc.DocTypeName);

                    var documentManagement = documentManagementsSourceHandedOver.LastOrDefault(x => x.IdTypeOfRequestedDocument == entry.Key);

                    WTableCell thirdCell = row.Cells[2];
                    thirdCell.Paragraphs.RemoveAt(0);
                    thirdCell.AddParagraph().AppendText(valueForDocseries);

                    var fabricNumberValuesFromAlgorithm = item.Split(" -> ", StringSplitOptions.RemoveEmptyEntries).ToList();
                    WTableCell fifthCell = row.Cells[3];
                    fifthCell.Paragraphs.RemoveAt(0);

                    WTableCell sixthCell = row.Cells[4];
                    sixthCell.Paragraphs.RemoveAt(0);
                    int serialNumbersCount = 1;
                    if (fabricNumberValuesFromAlgorithm.Count == 1)
                    {
                        fifthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                        sixthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                    }
                    else if (fabricNumberValuesFromAlgorithm.Count > 1)
                    {
                        fifthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[0])));
                        sixthCell.AddParagraph().AppendText(entry.Value.FirstOrDefault(x => x.Contains(fabricNumberValuesFromAlgorithm[1])));
                        serialNumbersCount += (int.Parse(fabricNumberValuesFromAlgorithm[1]) - int.Parse(fabricNumberValuesFromAlgorithm[0]));
                    }

                    WTableCell seventhCell = row.Cells[5];
                    seventhCell.Paragraphs.RemoveAt(0);
                    seventhCell.AddParagraph().AppendText(serialNumbersCount.ToString());

                    tableReceived.Rows.Insert(counter++, row);
                }
            }

            handedOverTable.Rows.RemoveAt(3);

            string[] fieldNames = new string[]
            {
                "ProviderFullName", "Location", "Municipality", "Area", "Date", "SchoolYear", "Total"
            };

            var candidateProvider = await this.repository.GetByIdAsync<CandidateProvider>(requestReportVM.IdCandidateProvider);
            var locationData = await this.locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(candidateProvider.IdLocationCorrespondence.Value);

            string[] fieldValues = new string[]
            {
                $"{candidateProvider.ProviderName} към {candidateProvider.ProviderOwner}",
                locationData.LocationName,
                locationData.Municipality.MunicipalityName,
                locationData.Municipality.District.DistrictName,
                requestReportVM.DestructionDate.Value.ToString("dd.MM.yyyy"),
                $"{requestReportVM.Year}/{requestReportVM.Year + 1}",
                string.Empty
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            document.Dispose();
            template.Close();

            return stream;
        }

        public async Task<ResultContext<NoResult>> DeleteRequestReportByIdAsync(int idRequestReport)
        {
            var result = new ResultContext<NoResult>();

            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<RequestReport>(idRequestReport);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<RequestReport>(entryFromDb.IdRequestReport);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<bool> DoesRequestReportForSameYearAlreadyExistAsync(int idCandidateProvider, int year)
        {
            return await this.repository.AllReadonly<RequestReport>(x => x.IdCandidateProvider == idCandidateProvider && x.Year == year).AnyAsync();
        }

        private List<string> CalculateConsecutiveFabricNumbers(List<int> fabricNumbers)
        {
            fabricNumbers = fabricNumbers.OrderBy(x => x).ToList();
            int length = 1;
            List<string> result = new List<string>();

            if (fabricNumbers.Count == 0)
            {
                return result;
            }

            for (int i = 1; i <= fabricNumbers.Count; i++)
            {
                if (i == fabricNumbers.Count || fabricNumbers[i] - fabricNumbers[i - 1] != 1)
                {
                    if (length == 1)
                    {
                        result.Add(string.Join("", fabricNumbers[i - length]));
                    }
                    else
                    {
                        result.Add(fabricNumbers[i - length] + " -> " + fabricNumbers[i - 1]);
                    }

                    length = 1;
                }
                else
                {
                    length++;
                }
            }

            return result;
        }
        #endregion

        #region Report uploaded document
        public async Task<ReportUploadedDocVM> GetReportUploadedDocumentByIdAsync(int idReportUploadedDoc)
        {
            IQueryable<ReportUploadedDoc> reportUploadedDocs = this.repository.AllReadonly<ReportUploadedDoc>(x => x.IdReportUploadedDoc == idReportUploadedDoc);

            return await reportUploadedDocs.To<ReportUploadedDocVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<ReportUploadedDocVM>> CreateReportUploadedDocumentAsync(ReportUploadedDocVM reportUploadedDocVM)
        {
            ResultContext<ReportUploadedDocVM> outputContext = new ResultContext<ReportUploadedDocVM>();

            try
            {
                var entryForDb = reportUploadedDocVM.To<ReportUploadedDoc>();
                entryForDb.CandidateProvider = null;
                entryForDb.RequestReport = null;

                await this.repository.AddAsync<ReportUploadedDoc>(entryForDb);
                await this.repository.SaveChangesAsync();

                outputContext.AddMessage("Записът е успешен!");
                reportUploadedDocVM.IdReportUploadedDoc = entryForDb.IdReportUploadedDoc;
                outputContext.ResultContextObject = reportUploadedDocVM;
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<ReportUploadedDocVM>> UpdateReportUploadedDocumentAsync(ReportUploadedDocVM reportUploadedDocVM)
        {
            ResultContext<ReportUploadedDocVM> outputContext = new ResultContext<ReportUploadedDocVM>();

            try
            {
                var entity = await this.repository.GetByIdAsync<ReportUploadedDoc>(reportUploadedDocVM.IdReportUploadedDoc);

                reportUploadedDocVM.IdCreateUser = entity.IdCreateUser;
                reportUploadedDocVM.CreationDate = entity.CreationDate;
                entity = reportUploadedDocVM.To<ReportUploadedDoc>();
                entity.CandidateProvider = null;
                entity.RequestReport = null;

                this.repository.Update<ReportUploadedDoc>(entity);
                await this.repository.SaveChangesAsync();

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = reportUploadedDocVM;
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<ReportUploadedDocVM>> DeleteReportUploadedDocumentAsync(ReportUploadedDocVM reportUploadedDocVM)
        {
            var entity = await this.repository.GetByIdAsync<ReportUploadedDoc>(reportUploadedDocVM.IdReportUploadedDoc);

            ResultContext<ReportUploadedDocVM> resultContext = new ResultContext<ReportUploadedDocVM>();

            try
            {
                this.repository.HardDelete<ReportUploadedDoc>(entity);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Документът беше изтрит успешно!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<IEnumerable<ReportUploadedDocVM>> GetAllReportUploadedDocumentsByRequestReportIdAsync(int idRequestReport)
        {
            IQueryable<ReportUploadedDoc> reportUploadedDocs = this.repository.AllReadonly<ReportUploadedDoc>(x => x.IdRequestReport == idRequestReport);
            var result = reportUploadedDocs.To<ReportUploadedDocVM>();

            return await result.ToListAsync();
        }
        #endregion
    }
}
