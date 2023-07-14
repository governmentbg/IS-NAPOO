
namespace ISNAPOO.Core.Services.Licensing
{
    using System.IO;

    using Syncfusion.DocIO;
    using Syncfusion.DocIO.DLS;

    using ISNAPOO.Core.Contracts.Licensing;
    using ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc;
    using ISNAPOO.Core.ViewModels.SPPOO;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ISNAPOO.Core.Contracts.Candidate;
    using ISNAPOO.Core.ViewModels.Candidate;
    using ISNAPOO.Core.Services.Candidate;
    using ISNAPOO.Core.ViewModels.EKATTE;
    using ISNAPOO.Core.Contracts.EKATTE;
    using ISNAPOO.Core.Contracts.SPPOO;
    using System.Linq;
    using ISNAPOO.Core.ViewModels.Common;
    using ISNAPOO.Core.ViewModels.Common.ValidationModels;
    using System;
    using Syncfusion.DocIO;
    using Syncfusion.DocIO.DLS;
    using Syncfusion.DocIORenderer;
    using Syncfusion.Pdf;
    using Syncfusion.OfficeChart;
    using Data.Models.Common;
    using ISNAPOO.Core.Contracts.Common;
    using System.Threading.Tasks.Sources;
    using ISNAPOO.Core.ViewModels.CPO.ProviderData;
    using Data.Models.Data.Common;
    using Syncfusion.XlsIO;
    using Data.Models.Data.Candidate;
    using Data.Models.Data.SPPOO;
    using Data.Models.Data.DOC;
    using ISNAPOO.Common.Constants;

    public class LicensingProcedureDocumentCPOService : BaseService, ILicensingProcedureDocumentCPOService
    {
        private IRepository repository;
        private readonly ICandidateProviderService candidateProviderService;
        private readonly ILocationService locationService;
        private readonly IMunicipalityService municipalityService;
        private readonly IDistrictService districtService;
        private readonly IProfessionService professionService;
        private readonly IProfessionalDirectionService professionalDirectionService;
        private readonly IDataSourceService dataSourceService;
        private readonly Dictionary<string, string> locations;
        private static List<string> romanNumerals = new List<string>() { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        private static List<int> numerals = new List<int>() { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

        public LicensingProcedureDocumentCPOService(IRepository repository, ICandidateProviderService candidateProviderService,
            ILocationService locationService,
            IMunicipalityService municipalityService,
            IDistrictService districtService,
            IProfessionService professionService,
            IProfessionalDirectionService professionalDirectionService, IDataSourceService dataSourceService)
            : base(repository)
        {
            this.repository = repository;
            this.candidateProviderService = candidateProviderService;
            this.locationService = locationService;
            this.locations = new Dictionary<string, string>();
            this.municipalityService = municipalityService;
            this.districtService = districtService;
            this.professionService = professionService;
            this.professionalDirectionService = professionalDirectionService;
            this.dataSourceService = dataSourceService;
        }

        private static string ToRomanNumeral(int number)
        {
            var romanNumeral = string.Empty;
            while (number > 0)
            {
                // find biggest numeral that is less than equal to number
                var index = numerals.FindIndex(x => x <= number);
                // subtract it's value from your number
                number -= numerals[index];
                // tack it onto the end of your roman numeral
                romanNumeral += romanNumerals[index];
            }
            return romanNumeral;
        }

        public MemoryStream GenerateApplication_1(CPOLicensingApplication1 application)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\LicensingProcedureDoc";

            FileStream template = new FileStream($@"{resources_Folder}\Prilojenie 1 - doklad dlajnostno lize.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ChiefExpert",
                "ApplicationNumber",
                "ApplicationInputDate",
                "CPOName",
                "CompanyName",
                "CityName",
                "ExpertCommission",
                "ExpertCommissionReportTerm"
            };

            string[] fieldValues = new string[]
            {
                application.ChiefExpert,
                application.ApplicationNumber,
                application.ApplicationInputDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ExpertCommissionName,
                application.DeadlineFormatted
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ExternalExperts", true, false);

            IWParagraph expertsParagraph = new WParagraph(document);

            bookNav.InsertParagraph(expertsParagraph);

            ListStyle expertsListStyle = document.AddListStyle(ListType.Bulleted, "ExpertsList");

            WListLevel expertListLevel = expertsListStyle.Levels[0];
            expertListLevel.FollowCharacter = FollowCharacterType.Tab;
            expertListLevel.CharacterFormat.FontSize = 12;
            expertListLevel.TextPosition = 0;

            WCharacterFormat profDirCharFormat = SetProfessionalDirectionCharacterFormat(document);

            WCharacterFormat expertsCharFormat = SetExpertsCharacterFormat(document);

            foreach (var expertList in application.ExternalExperts)
            {
                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText($"По професионално направление „{expertList.ProfessionalDirection}”")
                    .ApplyCharacterFormat(profDirCharFormat);

                expertsParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;

                bookNav.InsertParagraph(expertsParagraph);

                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText($"{expertList.Name}")
                    .ApplyCharacterFormat(expertsCharFormat);

                bookNav.InsertParagraph(expertsParagraph);
                expertsParagraph.ListFormat.ApplyStyle("ExpertsList");

                expertsParagraph.AppendBreak(BreakType.LineBreak);
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_1(CPOLicensingApplication1 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ChiefExpert",
                "ApplicationNumber",
                "ApplicationInputDate",
                "CpoName",
                "CompanyName",
                "CityName",
                "ExpertCommission",
                "ExpertCommissionReportTerm"
            };

            string[] fieldValues = new string[]
            {
                application.ChiefExpert,
                application.ApplicationNumber,
                application.ApplicationInputDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ExpertCommissionName,
                application.DeadlineFormatted
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            //CreateProfessionalDirectionList(document, application.ProfessionalDirections);
            CreateProfessionalDirectionListTable(document, application.ProfessionalDirections);

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ExternalExperts", true, false);

            IWParagraph expertsParagraph = new WParagraph(document);

            bookNav.InsertParagraph(expertsParagraph);

            ListStyle expertsListStyle = document.AddListStyle(ListType.Bulleted, "ExpertsList");

            WListLevel expertListLevel = expertsListStyle.Levels[0];
            expertListLevel.FollowCharacter = FollowCharacterType.Tab;
            expertListLevel.CharacterFormat.FontSize = 12;
            expertListLevel.TextPosition = 0;

            WCharacterFormat profDirCharFormat = SetProfessionalDirectionCharacterFormat(document);

            WCharacterFormat expertsCharFormat = SetExpertsCharacterFormat(document);

            foreach (var expertList in application.ProcedureExternalExperts)
            {
                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText($"По професионално направление „{expertList.ProfessionalDirection.Name}”")
                    .ApplyCharacterFormat(profDirCharFormat);

                expertsParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;

                bookNav.InsertParagraph(expertsParagraph);

                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText($"{expertList.Expert.Person.FullName}")
                    .ApplyCharacterFormat(expertsCharFormat);

                bookNav.InsertParagraph(expertsParagraph);
                expertsParagraph.ListFormat.ApplyStyle("ExpertsList");

                expertsParagraph.AppendBreak(BreakType.LineBreak);
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_2(CPOLicensingApplication2 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            //WSection section = document.Sections[0] as WSection;

            string[] fieldNames = new string[]
            {
                "OrderNumber",
                "OrderDate",
                "CpoName",
                "CompanyName",
                "CityName",
                "ProfessionsCount",
                "SpecialitiesCount",
                "ExternalExpertCommissionReportTerm",
                "ExpertCommissionName",
                "ExpertCommissionReportTerm",
                "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.OrderNumber,
                application.OrderDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ProfessionsCount.ToString(),
                application.SpecialtiesCount.ToString(),
                application.ExternalExpertCommissionReportTermFormatted,
                application.ExpertCommissionName,
                application.ExpertCommissionReportTermFormatted,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            WCharacterFormat profDirCharStyle = new WCharacterFormat(document);
            profDirCharStyle.FontSize = 12;
            profDirCharStyle.Bold = true;
            profDirCharStyle.Position = 0;

            WCharacterFormat expertCharStyle = new WCharacterFormat(document);
            expertCharStyle.FontSize = 12;
            expertCharStyle.Bold = false;
            expertCharStyle.Italic = true;
            expertCharStyle.Position = 0;

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ExternalExperts", true, false);

            IWParagraph expertsParagraph = new WParagraph(document);

            bookNav.InsertParagraph(expertsParagraph);

            ListStyle expertsListStyle = document.AddListStyle(ListType.Bulleted, "ExpertsList");

            WListLevel expertListLevel = expertsListStyle.Levels[0];
            expertListLevel.FollowCharacter = FollowCharacterType.Tab;
            expertListLevel.CharacterFormat.FontSize = 12;
            expertListLevel.TextPosition = 0;

            foreach (var expert in application.ProcedureExternalExperts)
            {
                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText($"По професионално направление „{expert.ProfessionalDirection.Name}”")
                    .ApplyCharacterFormat(profDirCharStyle);

                expertsParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                bookNav.InsertParagraph(expertsParagraph);

                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText($"{expert.Expert.Person.FullName}")
                    .ApplyCharacterFormat(expertCharStyle);
                expertsParagraph.ListFormat.ApplyStyle("ExpertsList");

                bookNav.InsertParagraph(expertsParagraph);
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_3(CPOLicensingApplication3 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContactPerson",
                "City",
                "PostCode",
                "StrName",
                "ContactPersonSirname",
                "ApplicationNumber",
                "ApplicationInputDate",
                "CompanyName",
                "CityName",
                "OrderNumber",
                "OrderInputDate",
                "ChiefExpert",
                "IntegerTax",
                "StringTax"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPerson.FullName,
                application.ContactPerson.CityName,
                application.ContactPerson.PostCode,
                application.ContactPerson.StreetName,
                application.ContactPerson.Sirname,
                application.ApplicationNumber,
                application.ApplicationInputDateFormatted,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.ChiefExpert,
                application.IntegerTax,
                application.StringTax
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            WCharacterFormat profDirCharStyle = SetProfessionalDirectionCharacterFormat(document);

            WCharacterFormat expertCharStyle = SetExpertsCharacterFormat(document);

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ExternalExperts", true, false);

            IWParagraph expertsParagraph = new WParagraph(document);

            bookNav.InsertParagraph(expertsParagraph);

            ListStyle expertsListStyle = document.AddListStyle(ListType.Bulleted, "ExpertsList");

            WListLevel expertListLevel = expertsListStyle.Levels[0];
            expertListLevel.FollowCharacter = FollowCharacterType.Tab;
            expertListLevel.CharacterFormat.FontSize = 12;
            expertListLevel.TextPosition = 0;

            foreach (var expert in application.ProcedureExternalExperts)
            {
                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText($"По професионално направление {expert.ProfessionalDirection.Code} „{expert.ProfessionalDirection.Name}”")
                    .ApplyCharacterFormat(profDirCharStyle);

                expertsParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                bookNav.InsertParagraph(expertsParagraph);

                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText($"{expert.Expert.Person.FullName}")
                    .ApplyCharacterFormat(expertCharStyle);
                expertsParagraph.ListFormat.ApplyStyle("ExpertsList");

                bookNav.InsertParagraph(expertsParagraph);
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_4(CPOLicensingApplication4 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContractNumber", "DateOfDraft", "OrderNumber", "OrderInputDate", "CpoName",
                "CompanyName", "CityName", "ExternalExpertName", "EGN",
                "IdCardNumber", "IdCardIssueDate", "IdCardCityOfIssue", "AddressByIdCard",
                "TaxOffice", "CompanyId", "ContractTerm", "ExternalExpertProfessionalDirection"
            };

            string[] fieldValues = new string[]
            {
                application.ContractNumber,
                application.DateOfDraftFormatted,
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ExpertDataVM.Person.FullName,
                application.ExpertDataVM.Person.Indent,
                application.ExpertDataVM.Person.PersonalID,
                application.ExpertDataVM.Person.PersonalIDDateFromFormated,
                application.ExpertDataVM.Person.PersonalIDIssueBy,
                application.ExpertDataVM.Person.Address,
                application.ExpertDataVM.Person.TaxOffice,
                application.CPOMainData.CompanyId,
                application.ContractTermFormatted,
                application.ExpertDataVM.ProfessionalDirectionStr
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_5(CPOLicensingApplication5 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContactPerson", "StrName", "City", "PostCode", "IntegerTax",
                "StringTax", "ContactPersonSirname", "OutputNumber", "OutputDate",
                "ChiefExpert", "TelephoneNumber", "EmailAddress"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPersonData.FullName,
                application.ContactPersonData.StreetName,
                application.ContactPersonData.CityName,
                application.ContactPersonData.PostCode,
                application.IntegerTax,
                application.StringTax,
                application.ContactPersonData.Sirname,
                application.OutputNumber,
                application.OutputDateFormatted,
                application.ChiefExpert,
                application.TelephoneNumber,
                application.EmailAddress
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_6(CPOLicensingApplication6 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ChiefExpert",
                "CpoName",
                "CompanyName",
                "CityName",
                "ApplicationNumber",
                "ApplicationInputDate"
            };

            string[] fieldValues = new string[]
            {
                application.ChiefExpert,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ApplicationNumber,
                application.ApplicationInputDataFormatted
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            WCharacterFormat issueCharStyle = new WCharacterFormat(document);
            issueCharStyle.FontName = "Times New Roman";
            issueCharStyle.FontSize = 12;
            issueCharStyle.Bold = false;
            issueCharStyle.Position = 0;

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("Issues", true, false);

            IWParagraph expertsParagraph = new WParagraph(document);

            bookNav.InsertParagraph(expertsParagraph);

            ListStyle expertsListStyle = document.AddListStyle(ListType.Bulleted, "IssuesList");

            WListLevel expertListLevel = expertsListStyle.Levels[0];
            expertListLevel.FollowCharacter = FollowCharacterType.Tab;
            expertListLevel.CharacterFormat.FontSize = 12;

            var issuesCount = application.Issues.Count;

            foreach (var issue in application.Issues)
            {
                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText(issue)
                    .ApplyCharacterFormat(issueCharStyle);

                bookNav.InsertParagraph(expertsParagraph);

                expertsParagraph.ListFormat.ApplyStyle("IssuesList");

                if (application.Issues[issuesCount - 1] != issue)
                {
                    expertsParagraph.AppendBreak(BreakType.LineBreak);
                }
            }

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_7(CPOLicensingApplication7 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContactPerson",
                "City",
                "PostCode",
                "StrName",
                "ContactPersonSirname",
                "CpoName",
                "CompanyName",
                "CityName",
                "ChiefExpert",
                "TelephoneNumber",
                "EmailAddress"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPersonData.FullName,
                application.ContactPersonData.CityName,
                application.ContactPersonData.PostCode,
                application.ContactPersonData.StreetName,
                application.ContactPersonData.Sirname,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ChiefExpert,
                application.TelephoneNumber,
                application.EmailAddress
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            WCharacterFormat issueCharStyle = new WCharacterFormat(document);
            issueCharStyle.FontName = "Times New Roman";
            issueCharStyle.FontSize = 12;
            issueCharStyle.Bold = false;

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("Issues", true, false);

            IWParagraph expertsParagraph = new WParagraph(document);

            bookNav.InsertParagraph(expertsParagraph);

            ListStyle expertsListStyle = document.AddListStyle(ListType.Bulleted, "IssuesList");

            WListLevel expertListLevel = expertsListStyle.Levels[0];
            expertListLevel.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
            expertListLevel.FollowCharacter = FollowCharacterType.Tab;
            expertListLevel.ParagraphFormat.LeftIndent = 0;
            expertListLevel.CharacterFormat.FontSize = 12;

            var issuesCount = application.Issues.Count;

            foreach (var issue in application.Issues)
            {
                expertsParagraph = new WParagraph(document);
                expertsParagraph.AppendText(issue)
                    .ApplyCharacterFormat(issueCharStyle);

                expertsParagraph.ListFormat.ApplyStyle("IssuesList");

                bookNav.InsertParagraph(expertsParagraph);

                expertsParagraph = new WParagraph(document);
                bookNav.InsertParagraph(expertsParagraph);
            }

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_8(CPOLicensingApplication8 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ChiefExpert",
                "NotificationLetterNumber",
                "NotificationLetterOutputDate",
                "CompanyName",
                "CityName",
                "ApplicationNumber",
                "ApplicationInputDate",
                "ReportNumber",
                "ReportInputDate",
                "DueDate",
                "CpoName"
            };

            string[] fieldValues = new string[]
            {
                application.ChiefExpert,
                application.NotificationLetterNumber,
                application.NotificationLetterOutputDateFormatted,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ApplicationNumber,
                application.ApplicationInputDateFormatted,
                application.ReportNumber,
                application.ReportInputDateFormatted,
                application.DueDateAddOneMonthFormatted,
                application.CPOMainData.CPOName
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_9(CPOLicensingApplication9 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
               "OrderNumber",
                "OrderInputDate",
                "CpoName",
                "CompanyName",
                "CityName",
                "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_10(CPOLicensingApplication10 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContactPerson",
                "City",
                "PostCode",
                "CpoName",
                "CompanyName",
                "CityName",
                "ContactPersonSirname",
                "NotificationLetterNumber",
                "NotificationLetterOutputDate",
                "DueDate",
                "OrderNumber",
                "OrderInputDate",
                "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPersonData.FullName,
                application.ContactPersonData.CityName,
                application.ContactPersonData.PostCode,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ContactPersonData.Sirname,
                application.NotificationLetterNumber,
                application.NotificationLetterOutputDateFormatted,
                application.DueDateFormatted,
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_11(CPOLicensingApplication11 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            Dictionary<int, List<ScoreTableData>> tableData = application.ScoreTable;

            string[] fieldNames = new string[]
                {
                    "ExternalExpert", "CpoName", "CompanyName", "CityName",
                    "ProfessionalDirection", "OrderNumber",
                    "OrderInputDate","FromDate","ToDate",
                    "MeanScore"
                };

            string[] fieldValues = new string[]                                                                                                                                                
                {
                    application.ExternalExpertDataVM.Person.FullName,
                    application.CPOMainData.CPOName,
                    application.CPOMainData.CompanyName,
                    application.CPOMainData.CityName,
                    application.ExternalExpertDataVM.ProfessionalDirectionStr,
                    application.OrderNumber,
                    application.OrderInputDateFormatted,
                    application.PeriodOfOrderCompletion.FromDateFormatted,
                    application.PeriodOfOrderCompletion.ToDateFormatted,                               
                    application.MeanScore
                };                                                                                                            

            document.MailMerge.Execute(fieldNames, fieldValues);

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            Syncfusion.DocIO.DLS.IWTable tableTemplate = new Syncfusion.DocIO.DLS.WTable(document);
            foreach (WTable table in document.LastSection.Tables)
            {
                if (table.Title == "ResultTable")
                {
                    tableTemplate = table;
                    document.LastSection.Body.ChildEntities.Remove(table);
                }
            }            

            BookmarksNavigator bookmark = new BookmarksNavigator(document);
            bookmark.MoveToBookmark("InsertTable", true, false);

            foreach (var direction in application.ProfessionalDirections)
            {
                bookmark.InsertParagraph(new WParagraph(document) { Text = $"Професионално направление {direction.Name}, код {direction.Code}" });
                foreach (var profession in direction.Professions)
                {
                    bookmark.InsertParagraph(new WParagraph(document) { Text = $"Професия {profession.Name}, код {profession.Code}" });
                    foreach (var speciality in profession.Specialities)
                    {
                        bookmark.InsertParagraph(new WParagraph(document) { Text = $"Специалност {speciality.Name}, код {speciality.Code}" });
                    }
                    bookmark.InsertTable(tableTemplate.Clone() as WTable);
                    bookmark.InsertParagraph(new WParagraph(document));
                }
            }

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_13(CPOLicensingApplication13 application, TemplateDocumentVM templateVM)
        {

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "HeadOfExpertCommission", "NameOfExpertCommission", "DateOfMeeting", "DayOfWeek",
                "Time", "OrderNumber", "OrderInputDate", "CpoName", "CompanyName", "CityName", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.HeadOfExpertCommission,
                application.ExpertCommissionName,
                application.DateOfMeetingFormatted,
                application.DayOfWeek,
                application.MeetingHour,
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ExpertCommissionMembers");

            foreach (var member in application.ExpertCommissionMembers)
            {
                WParagraph paragraph = new WParagraph(document);
                paragraph.ParagraphFormat.AfterSpacing = 1;
                paragraph.ParagraphFormat.BeforeSpacing = 1;
                paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                paragraph.AppendText(member);

                bookNav.InsertParagraph(paragraph);
            }

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_14(CPOLicensingApplication14 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContractNumber", "DateOfDraft", "NameOfExpertCommission", "ChiefExpert",
                "EGN", "IdCardNumber", "IdCardIssueDate", "IdCardCityOfIssue", "AddressByIdCard",
                "TaxOffice", "CpoName", "CompanyName", "CityName", "CompanyId", "OrderNumber", "OrderInputDate"
            };

            string[] fieldValues = new string[]
            {
                application.ContractNumber,
                application.DateOfDraftFormatted,
                application.ExpertCommissionName,
                application.ExpertDataVM.Person.FullName,
                application.ExpertDataVM.Person.Indent,
                application.ExpertDataVM.Person.PersonalID,
                application.ExpertDataVM.Person.PersonalIDDateFromFormated,
                application.ExpertDataVM.Person.PersonalIDIssueBy,
                application.ExpertDataVM.Person.Address,
                application.ExpertDataVM.Person.TaxOffice,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.CPOMainData.CompanyId,
                application.OrderNumber,
                application.OrderInputDateFormatted
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_15(CPOLicensingApplication15 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContractNumber", "DateOfDraft", "ExpertCommissionName", "ExpertCommissionMemberName",
                "EGN", "IdCardNumber", "IdCardIssueDate", "IdCardCityOfIssue", "AddressByIdCard",
                "TaxOffice", "CpoName", "CompanyName", "CityName", "CompanyId", "OrderNumber", "OrderInputDate"
            };

            string[] fieldValues = new string[]
            {
                application.ContractNumber,
                application.DateOfDraftFormatted,
                application.ExpertCommissionName,
                application.ExpertDataVM.Person.FullName,
                application.ExpertDataVM.Person.Indent,
                application.ExpertDataVM.Person.PersonalID,
                application.ExpertDataVM.Person.PersonalIDDateFromFormated,
                application.ExpertDataVM.Person.PersonalIDIssueBy,
                application.ExpertDataVM.Person.Address,
                application.ExpertDataVM.Person.TaxOffice,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.CPOMainData.CompanyId,
                application.OrderNumber,
                application.OrderInputDateFormatted
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_16(CPOLicensingApplication16 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ProtocolNumber", "ExpertCommissionName", "DateOfDraft", "Time",
                "DistanceConnectionSoftwareName", "OrderNumber", "CpoName",
                "CompanyName", "CityName", "OrderInputDate", "TotalScore",
                "ChiefOfExpertCommission", "Protocoler"
            };

            string[] fieldValues = new string[]
            {
                application.ProtcolNumber,
                application.ExpertCommissionName,
                application.DateOfDraftFormatted,
                application.MeetingHour,
                application.DistanceConnectionSoftwareName,
                application.OrderNumber,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.OrderInputDateFormatted,
                application.TotalScore,
                application.ChiefOfExpertCommission,
                application.Protocoler
            };

            MemoryStream stream = new MemoryStream();

            document.MailMerge.Execute(fieldNames, fieldValues);

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("MeetingAttendance", true, false);

            foreach (string member in application.MeetingAttendance)
            {
                WParagraph memberParagraph = new WParagraph(document);
                memberParagraph.ParagraphFormat.AfterSpacing = 1;
                memberParagraph.ParagraphFormat.BeforeSpacing = 1;

                memberParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                memberParagraph.AppendText(member).CharacterFormat.FontSize = 12;

                bookNav.InsertParagraph(memberParagraph);
            }

            //bookNav.MoveToBookmark("ExpertCommissionReviewResults", true, false);

            //WParagraph resultsParagraph = new WParagraph(document);
            //resultsParagraph.AppendText("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do " +
            //    "eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, " +
            //    "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
            //    "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat " +
            //    "nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia " +
            //    "deserunt mollit anim id est laborum.").CharacterFormat.FontSize = 12;
            //resultsParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;

            //bookNav.InsertParagraph(resultsParagraph);

            bookNav.MoveToBookmark("ExpertCommissionScores", true, false);

            foreach (var score in application.ExpertCommissionScores)
            {
                WParagraph scoresParagraph = new WParagraph(document);
                scoresParagraph.AppendText($"По професионално направление „{score.Key}“ обща оценка по всички критерии - {score.Value}")
                    .CharacterFormat.FontSize = 12;
                scoresParagraph.ParagraphFormat.AfterSpacing = 1;
                scoresParagraph.ParagraphFormat.BeforeSpacing = 1;

                scoresParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;

                bookNav.InsertParagraph(scoresParagraph);
            }

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_17(CPOLicensingApplication17 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ExpertCommissionChairman", "ExpertCommissionName", "OrderNumber", "OrderInputDate",
                "CpoName", "CompanyName", "CityName", "ApplicationNumber", "ApplicationInputDate",
                "ChiefExpert", "ProtocolNumber", "ProtocolInputDate", "TotalScore"
            };

            string[] fieldValues = new string[]
            {
                application.ExpertCommissionChairman,
                application.ExpertCommissionName,
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ApplicationNumber,
                application.ApplicationInputDateFormatted,
                application.ChiefExpert,
                application.ProtocolNumber,
                application.ProtocolInputDateFormatted,
                application.TotalScore
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            BookmarksNavigator bookNav = new BookmarksNavigator(document);

            bookNav.MoveToBookmark("ExpertCommissionScores", true, false);

            foreach (var score in application.ExpertCommissionScores)
            {
                WParagraph scoresParagraph = new WParagraph(document);
                scoresParagraph.AppendText($"По професионално направление „{score.Key}“ обща оценка по всички критерии - {score.Value}")
                    .CharacterFormat.FontSize = 12;
                scoresParagraph.ParagraphFormat.AfterSpacing = 1;
                scoresParagraph.ParagraphFormat.BeforeSpacing = 1;

                scoresParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;

                bookNav.InsertParagraph(scoresParagraph);
            }

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_18(CPOLicensingApplication18 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ChiefExpert", "CpoName", "CompanyName", "CityName", "OrderNumber",
                "OrderInputDate", "ExpertCommissionName", "ProtocolNumber", "ProtocolInputDate",
                "ExpertCommissionChairmanSirname", "ProfessionsCount",
                "SpecialtiesCount", "ReportNumber", "ReportInputDate", "ExpertCommissionChairman"
            };

            string[] fieldValues = new string[]
            {
                application.ChiefExpert,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.ExpertCommissionName,
                application.ProtocolNumber,
                application.ProtocolInputDateFormatted,
                application.ExpertCommissionChairmanSirname,
                application.ProfessionsCount,
                application.SpecialitiesCount,
                application.ReportNumber,
                application.ReportInputDateFormatted,
                application.ExpertCommissionChairmanFullName
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_19(CPOLicensingApplication19 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "OrderNumber",
                "OrderInputDate",
                "ExpertCommissionName",
                "ReportNumber",
                "ReportInputDate",
                "ApplicationNumber",
                "ApplicationInputDate",
                "CompanyName",
                "CityName",
                "LicenseNumber",
                "CpoName",
                "ProfessionsCount",
                "SpecialtiesCount",
                "ChiefExpert"
            };

            string[] fieldValues = new string[] {
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.ExpertCommissionName,
                application.ReportNumber,
                application.ReportInputDateFormatted,
                application.ApplicationNumber,
                application.ApplicationInputDateFormatted,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.LicenseNumber,
                application.CPOMainData.CPOName,
                application.ProfessionsCount,
                application.SpecialitiesCount,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            CreateProfessionalDirectionListTable(document, application.ProfessionalDirections);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_20(CPOLicensingApplication20 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContactPerson", "StrName", "City", "PostCode", "ContactPersonSirname",
                "CpoName", "CompanyName", "CityName", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPersonData.FullName,
                application.ContactPersonData.StreetName,
                application.ContactPersonData.CityName,
                application.ContactPersonData.PostCode,
                application.ContactPersonData.Sirname,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            CreateProfessionalDirectionListTable(document, application.ProfessionalDirections);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateApplication_21(CPOLicensingApplication21 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "OrderNumber", "OrderInputDate", "ExpertCommissionName", "ProtocolNumber", "ProtocolInputDate",
                "CpoName", "CompanyName", "CityName", "App19OrderNumber", "App19OrderInputDate", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.ExpertCommissionName,
                application.ProtocolNumber,
                application.ProtocolInputDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.App19OrderNumber,
                application.App19OrderInputDateFormatted,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            BookmarksNavigator bookNav = new BookmarksNavigator(document);

            bookNav.MoveToBookmark("ExpertCommissionChairman", true, false);

            WTable table = new WTable(document);
            table.TableFormat.Borders.BorderType = BorderStyle.Cleared;

            WCharacterFormat nameColumnCharStyle = new WCharacterFormat(document);
            nameColumnCharStyle.FontSize = 12;

            table.ResetCells(1, 2);

            table.Rows[0].Height = 20;
            table[0, 0].AddParagraph().AppendText(application.ExpertCommissionChairman).ApplyCharacterFormat(nameColumnCharStyle);

            IWParagraph remunerationCell = table[0, 1].AddParagraph();
            remunerationCell.AppendText("55,00 лв.").ApplyCharacterFormat(nameColumnCharStyle);
            remunerationCell.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;

            bookNav.InsertTable(table);

            bookNav.MoveToBookmark("MembersList", true, false);



            table = new WTable(document);
            table.TableFormat.Borders.BorderType = BorderStyle.Cleared;

            List<string> members = application.MemberList;

            table.ResetCells(members.Count, 2);

            for (int i = 0; i < members.Count; i++)
            {
                table.Rows[i].Height = 20;
                table[i, 0].AddParagraph().AppendText(members[i]).ApplyCharacterFormat(nameColumnCharStyle);

                remunerationCell = table[i, 1].AddParagraph();
                remunerationCell.AppendText("40,00 лв.").ApplyCharacterFormat(nameColumnCharStyle);
                remunerationCell.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
            }

            bookNav.InsertTable(table);

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateLicensingApplication(CandidateProviderVM candidateProvider, IEnumerable<KeyValueVM> kvApplicationFilingSource, IEnumerable<KeyValueVM> kvReceiveLicenseSource, IEnumerable<KeyValueVM> kvVQSSource, TemplateDocumentVM templateDoc = null)
        {
            var applicationFiling = candidateProvider.IdApplicationFiling;
            var receiveLicense = candidateProvider.IdReceiveLicense;
            candidateProvider = await this.candidateProviderService.GetCandidateProviderByIdAsync(candidateProvider);
            candidateProvider.IdApplicationFiling = applicationFiling;
            candidateProvider.IdReceiveLicense = receiveLicense;

            var listProfessionIds = new List<int>();
            foreach (var speciality in candidateProvider.CandidateProviderSpecialities)
            {
                if (!listProfessionIds.Contains(speciality.Speciality.IdProfession))
                {
                    listProfessionIds.Add(speciality.Speciality.IdProfession);
                }
            }

            var professions = (await this.professionService.GetProfessionsByIdsAsync(listProfessionIds)).ToList();
            var listProfDirIds = new List<int>();
            foreach (var profession in professions)
            {
                if (!listProfDirIds.Contains(profession.IdProfessionalDirection))
                {
                    listProfDirIds.Add(profession.IdProfessionalDirection);
                }
            }

            var profDirections = (await this.professionalDirectionService.GetProfessionalDirectionsByIdsAsync(listProfDirIds)).ToList();

            LocationVM location = await this.locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(candidateProvider.IdLocationCorrespondence.Value);

            FileStream template;
            if (templateDoc is not null)
            {
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

                template = new FileStream($@"{resources_Folder}{templateDoc.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            else
            {
                var resources_Folder = Directory.GetCurrentDirectory() + @"/wwwroot/Templates/CPO/Application";

                template = new FileStream($@"{resources_Folder}/Zaqvlenie-Licenzirane-CPO.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            WordDocument document = new WordDocument(template, FormatType.Automatic);

            var name = string.Empty;
            var nameValue = candidateProvider.AttorneyName ?? candidateProvider.ManagerName;
            var nameAsArr = nameValue?.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (nameAsArr?.Length > 2)
            {
                name = nameAsArr[0] + " " + nameAsArr[2];
            }
            else
            {
                name = nameValue;
            }

            string[] fieldNames = new string[]
            {
                "CompanyName", "CPOName", "CompanyId", "CompanyRepresentative", "CompanyCity",
                "City", "District", "Municipality", "PostCode", "Blvd/StrName", "TelephoneNumber",
                "Email", "ContactPersonFullName", "Date", "Name", "UIN"
            };

            string[] fieldValues = new string[]
            {
                candidateProvider.ProviderOwner,
                candidateProvider.ProviderName,
                candidateProvider.PoviderBulstat,
                candidateProvider.AttorneyName ?? candidateProvider.ManagerName,
                location.LocationName,
                location.LocationName,
                location.Municipality.District.DistrictName,
                location.Municipality.MunicipalityName,
                candidateProvider.ZipCodeCorrespondence,
                candidateProvider.ProviderAddressCorrespondence,
                candidateProvider.ProviderPhone,
                candidateProvider.ProviderEmail,
                candidateProvider.PersonNameCorrespondence,
                DateTime.Now.ToString("dd.MM.yyyy"),
                name,
                candidateProvider.UIN.Value.ToString()
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ProfessionalDirectionsList", true, false);

            IWParagraph paragraph = new WParagraph(document);
            paragraph.AppendBreak(BreakType.LineBreak);
            paragraph.AppendBreak(BreakType.LineBreak);
            bookNav.InsertParagraph(paragraph);

            WCharacterFormat profDirCharFormat = SetProfessionalDirectionCharacterFormat(document);
            WCharacterFormat specialtiesDataFormat = SetSpecialitiesDataCharacterFormat(document);

            var romanValue = 1;

            IWTable table = new WTable(document, true);
            WTableRow row = table.AddRow(true);
            WTableCell cell = row.AddCell();
            var rowCounter = 0;

            for (int firstLevel = 0; firstLevel < profDirections.Count(); firstLevel++)
            {
                row = table.AddRow(true);
                ProfessionalDirectionVM profData = profDirections[firstLevel];
                profData.NumericValue = ToRomanNumeral(romanValue++);

                table.Rows[rowCounter++].Cells[0].AddParagraph().AppendText($"{profData.NumericValue}. Професионално направление: {profData.Name}, код {profData.Code}");

                var professionChildrenFromProfDir = professions.Where(x => x.IdProfessionalDirection == profData.IdProfessionalDirection).ToList();
                var professionCounter = 1;
                var specialityFirstCounter = 0;

                for (int secondLevel = 0; secondLevel < professionChildrenFromProfDir.Count; secondLevel++)
                {
                    WTableRow rowSecond = table.AddRow();

                    ProfessionVM prof = professionChildrenFromProfDir[secondLevel];

                    table.Rows[rowCounter++].Cells[0].AddParagraph().AppendText($"{professionCounter++}. Професия: {prof.Name}, код {prof.Code}");
                    var specialitiesChildrenFromProfession = candidateProvider.CandidateProviderSpecialities.Where(x => x.Speciality.IdProfession == prof.IdProfession).ToList();
                    var specialitySecondCounter = 1;
                    specialityFirstCounter++;

                    for (int thirdLevel = 0; thirdLevel < specialitiesChildrenFromProfession.Count; thirdLevel++)
                    {
                        WTableRow rowThird = table.AddRow();

                        var spec = specialitiesChildrenFromProfession[thirdLevel].Speciality;
                        if (spec.IdVQS != 0)
                        {
                            spec.VQS_Name = kvVQSSource.FirstOrDefault(x => x.IdKeyValue == spec.IdVQS).Name;
                        }

                        table.Rows[rowCounter++].Cells[0].AddParagraph().AppendText($"{specialityFirstCounter}.{specialitySecondCounter++}. Специалност: {spec.Name}, код {spec.Code}, {spec.VQS_Name}.");
                    }
                }
            }

            table.Rows.RemoveAt(rowCounter);
            bookNav.InsertTable(table);

            bookNav.MoveToBookmark("DataTable", true, true);
            bookNav.DeleteBookmarkContent(true);

            bookNav.MoveToBookmark("ApplicationFiling", true, true);
            bookNav.DeleteBookmarkContent(true);

            bookNav.InsertParagraph(new WParagraph(document));

            foreach (var filingType in kvApplicationFilingSource)
            {
                var selectedFilingType = kvApplicationFilingSource.FirstOrDefault(x => x.IdKeyValue == candidateProvider.IdApplicationFiling);
                WParagraph paragraphTest = new WParagraph(document);
                WCheckBox checkbox = paragraphTest.AppendCheckBox();
                if (filingType.IdKeyValue == selectedFilingType.IdKeyValue)
                {
                    checkbox.Checked = true;
                }
                else
                {
                    checkbox.Checked = false;
                }

                paragraphTest.AppendText(filingType.Description);

                bookNav.InsertParagraph(paragraphTest);
            }

            bookNav.MoveToBookmark("ReceiveLicense", true, true);
            bookNav.DeleteBookmarkContent(true);

            bookNav.InsertParagraph(new WParagraph(document));

            foreach (var receiveType in kvReceiveLicenseSource)
            {
                var selectedReceiveType = kvReceiveLicenseSource.FirstOrDefault(x => x.IdKeyValue == candidateProvider.IdReceiveLicense);
                WParagraph paragraphTest2 = new WParagraph(document);
                WCheckBox checkbox = paragraphTest2.AppendCheckBox();
                if (receiveType.IdKeyValue == selectedReceiveType.IdKeyValue)
                {
                    checkbox.Checked = true;
                }
                else
                {
                    checkbox.Checked = false;
                }

                paragraphTest2.AppendText(receiveType.Description);

                bookNav.InsertParagraph(paragraphTest2);
            }

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

        private void CreateProfessionalDirectionListTable(WordDocument document, List<ProfessionalDirectionVM> professionalDirections)
        {
            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ProfessionalDirectionsList", true, false);

            IWParagraph paragraph = new WParagraph(document);
            paragraph.AppendBreak(BreakType.LineBreak);
            paragraph.AppendBreak(BreakType.LineBreak);
            bookNav.InsertParagraph(paragraph);

            WCharacterFormat profDirCharFormat = SetProfessionalDirectionCharacterFormat(document);
            WCharacterFormat specialtiesDataFormat = SetSpecialitiesDataCharacterFormat(document);

            var romanValue = 1;

            IWTable table = new WTable(document, true);
            WTableRow row = table.AddRow(true);
            WTableCell cell = row.AddCell();
            var rowCounter = 0;

            for (int firstLevel = 0; firstLevel < professionalDirections.Count(); firstLevel++)
            {
                row = table.AddRow(true);
                ProfessionalDirectionVM profData = professionalDirections[firstLevel];
                profData.NumericValue = ToRomanNumeral(romanValue++);

                table.Rows[rowCounter].Cells[0].AddParagraph().AppendText($"{profData.NumericValue}. Професионално направление: {profData.Name}, код {profData.Code}{Environment.NewLine}")
                    .ApplyCharacterFormat(profDirCharFormat);

                var professionCounter = 1;
                var specialityFirstCounter = 0;

                for (int secondLevel = 0; secondLevel < profData.Professions.Count; secondLevel++)
                {
                    //WTableRow rowSecond = table.AddRow();

                    ProfessionVM prof = profData.Professions[secondLevel];

                    table.Rows[rowCounter].Cells[0].AddParagraph().AppendText($"{professionCounter++}. Професия: {prof.Name}, код {prof.Code}{Environment.NewLine}")
                        .ApplyCharacterFormat(profDirCharFormat);

                    var specialitySecondCounter = 1;
                    specialityFirstCounter++;

                    for (int thirdLevel = 0; thirdLevel < prof.Specialities.Count; thirdLevel++)
                    {
                        //WTableRow rowThird = table.AddRow();

                        var spec = prof.Specialities[thirdLevel];

                        table.Rows[rowCounter].Cells[0].AddParagraph().AppendText($"{specialityFirstCounter}.{specialitySecondCounter++}. Специалност: {spec.Name}, код {spec.Code}, {spec.VQS_Name}.")
                            .ApplyCharacterFormat(specialtiesDataFormat);
                    }
                }

                rowCounter++;
            }

            table.Rows.RemoveAt(rowCounter);
            bookNav.InsertTable(table);
        }

        private void CreateProfessionalDirectionList(WordDocument document, List<ProfessionalDirectionVM> professionalDirections)
        {
            ListStyle listStyle = document.AddListStyle(ListType.Numbered, "ProfessionalDirectionListStyle");

            WListLevel levelOne = listStyle.Levels[0];
            levelOne.FollowCharacter = FollowCharacterType.Space;
            levelOne.PatternType = ListPatternType.UpRoman;
            levelOne.CharacterFormat.FontSize = 12;
            levelOne.CharacterFormat.Bold = true;
            levelOne.NumberSuffix = ". ";
            levelOne.TextPosition = 0;
            levelOne.StartAt = 1;

            WListLevel levelTwo = listStyle.Levels[1];
            levelTwo.FollowCharacter = FollowCharacterType.Space;
            levelTwo.NumberAlignment = ListNumberAlignment.Left;
            levelTwo.PatternType = ListPatternType.Arabic;
            levelTwo.CharacterFormat.FontSize = 12;
            levelTwo.CharacterFormat.Bold = true;
            levelTwo.NumberSuffix = ". ";
            levelTwo.TextPosition = 0;
            levelTwo.StartAt = 1;

            WListLevel levelThree = listStyle.Levels[2];
            levelThree.FollowCharacter = FollowCharacterType.Space;
            levelThree.NumberAlignment = ListNumberAlignment.Left;
            levelThree.PatternType = ListPatternType.Arabic;
            levelThree.CharacterFormat.FontSize = 12;
            levelThree.CharacterFormat.Bold = false;
            levelThree.NumberSuffix = ".";
            levelThree.TextPosition = 0;
            levelThree.StartAt = 1;

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ProfessionalDirectionsList", true, false);

            IWParagraph paragraph = new WParagraph(document);
            paragraph.AppendBreak(BreakType.LineBreak);
            paragraph.AppendBreak(BreakType.LineBreak);
            bookNav.InsertParagraph(paragraph);

            WCharacterFormat profDirCharFormat = SetProfessionalDirectionCharacterFormat(document);

            WCharacterFormat specialtiesDataFormat = SetSpecialitiesDataCharacterFormat(document);

            for (int firstLevel = 0; firstLevel < professionalDirections.Count; firstLevel++)
            {
                ProfessionalDirectionVM profData = professionalDirections[firstLevel];

                paragraph = new WParagraph(document);
                paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                paragraph.ListFormat.ApplyStyle("ProfessionalDirectionListStyle");
                paragraph.ListFormat.ListLevelNumber = 0;

                //Поставя текст на мястото на новия параграф и прилага предварително създаден формат на текста
                paragraph.AppendText($"{profData.NumericValue} Професионално направление: {profData.Name}, код {profData.Code}").ApplyCharacterFormat(profDirCharFormat);
                //paragraph.ParagraphFormat.LineSpacing = 9f;
                //Прилага стилът със съответното име към нивото в списъка
                paragraph.ListFormat.ApplyStyle("ProfessionalDirectionListStyle");
                paragraph.ListFormat.ContinueListNumbering();

                bookNav.InsertParagraph(paragraph);

                for (int secondLevel = 0; secondLevel < profData.Professions.Count; secondLevel++)
                {
                    //Създава второто ниво в таблицата
                    paragraph = new WParagraph(document);
                    paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    paragraph.ListFormat.IncreaseIndentLevel();

                    ProfessionVM prof = profData.Professions[secondLevel];

                    //Поставя текст на мястото на новия параграф и прилага предварително създаден формат на текста
                    paragraph.ListFormat.ApplyStyle("ProfessionalDirectionListStyle");
                    paragraph.AppendText($"Професия: {prof.Name}, код {prof.Code}").ApplyCharacterFormat(profDirCharFormat);

                    paragraph.ListFormat.ListLevelNumber = 1;

                    paragraph.ListFormat.ContinueListNumbering();
                    bookNav.InsertParagraph(paragraph);

                    for (int thirdLevel = 0; thirdLevel < prof.Specialities.Count; thirdLevel++)
                    {
                        SpecialityVM spec = prof.Specialities[thirdLevel];

                        //Създава третото ниво в таблицата
                        paragraph = new WParagraph(document);
                        paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                        paragraph.ListFormat.IncreaseIndentLevel();

                        if (thirdLevel == 0)
                        {
                            paragraph.ListFormat.RestartNumbering = true;
                        }

                        paragraph.ListFormat.ListLevelNumber = 2;

                        levelThree.NumberPrefix = "\u0001.";
                        paragraph.ListFormat.ApplyStyle("ProfessionalDirectionListStyle");
                        //Поставя текст на мястото на новия параграф и прилага предварително създаден формат на текста
                        paragraph.AppendText($"Специалност: {spec.Name}, код {spec.Code}, {spec.VQS_Name}")
                            .ApplyCharacterFormat(specialtiesDataFormat);

                        bookNav.InsertParagraph(paragraph);

                        paragraph.ListFormat.ContinueListNumbering();
                    }
                }

                paragraph = new WParagraph(document);
                paragraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;

                paragraph.ListFormat.ListLevelNumber = 0;
                paragraph.ListFormat.ContinueListNumbering();
            }
        }

        private static WCharacterFormat SetProfessionalDirectionCharacterFormat(WordDocument document)
        {
            WCharacterFormat profDirCharFormat = new WCharacterFormat(document);
            profDirCharFormat.FontName = "Times New Roman";
            profDirCharFormat.FontSize = 12;
            profDirCharFormat.Position = 1;
            profDirCharFormat.Bold = false;

            return profDirCharFormat;
        }

        private static WCharacterFormat SetSpecialitiesDataCharacterFormat(WordDocument document)
        {
            WCharacterFormat specialtiesDataFormat = new WCharacterFormat(document);
            specialtiesDataFormat.FontName = "Times New Roman";
            specialtiesDataFormat.FontSize = 12;
            specialtiesDataFormat.Position = 1;
            specialtiesDataFormat.Bold = false;

            return specialtiesDataFormat;
        }

        private static WCharacterFormat SetExpertsCharacterFormat(WordDocument document)
        {
            WCharacterFormat expertsCharFormat = new WCharacterFormat(document);
            expertsCharFormat.FontName = "Times New Roman";
            expertsCharFormat.FontSize = 12;
            expertsCharFormat.Position = 1;
            expertsCharFormat.Italic = true;
            expertsCharFormat.Bold = false;

            return expertsCharFormat;
        }

        public Task<MemoryStream> GenerateLicensingApplication(CandidateProviderVM candidateProvider)
        {
            throw new NotImplementedException();
        }

        public async Task<long> GenerateLicenseNumberAsync(CandidateProviderVM candidateProvider)
        {
            var dateValue = candidateProvider.LicenceDate.Value.ToString("yyyy");
            var providerTypeInfo = "12";

            var valueForSequenceCheck = "LicenseNumber" + dateValue + providerTypeInfo;
            var value = await this.GetSequenceNextValue("LicenseNumber");
            var licenseNumber = string.Empty;
            if (value < 10)
            {
                licenseNumber = valueForSequenceCheck + "000";
            }
            else if (value >= 10 && value < 100)
            {
                licenseNumber = valueForSequenceCheck + "00";
            }
            else if (value >= 100 && value < 1000)
            {
                licenseNumber = valueForSequenceCheck + "0";
            }
            else
            {
                licenseNumber = valueForSequenceCheck;
            }

            licenseNumber += value.ToString();
            licenseNumber = licenseNumber.Substring(13, licenseNumber.Length - 13);

            return long.Parse(licenseNumber);
        }

        public async Task<MemoryStream> GenerateFirstLicensingAsync(CandidateProviderVM candidateProvider, ProcedureDocumentVM procedureDocument)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"/wwwroot/Templates/CPO/Application";

            FileStream template = new FileStream($@"{resources_Folder}/Obrazec_licenziq_CPO1.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            WordDocument document = new WordDocument(template, FormatType.Automatic);

            var location = await this.locationService.GetLocationByIdAsync(candidateProvider.IdLocation.Value);
            var addressJoined = string.Empty;
            if (candidateProvider.IdRegionAdmin.HasValue)
            {
                var regionInformation = await this.repository.GetByIdAsync<Region>(candidateProvider.IdRegionAdmin!.Value);
                if (regionInformation is not null)
                {
                    addressJoined = $"{location.LocationName}, р-н {regionInformation.RegionName}, {candidateProvider.ProviderAddress}";
                }
            }
            else
            {
                addressJoined = $"{location.LocationName}, {candidateProvider.ProviderAddress}";
            }

            string[] fieldNames = new string[]
            {
                "LicenseNumber", "Order", "ProviderOwner", "Location", "Bulstat"
            };

            string[] fieldValues = new string[]
            {
                $"{candidateProvider.LicenceNumber}/{candidateProvider.LicenceDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.",
                $"{procedureDocument.DS_OFFICIAL_DocNumber}/{procedureDocument.DS_OFFICIAL_DATE.Value.ToString("dd.MM.yyyy")}г.",
                candidateProvider.ProviderOwner,
                addressJoined,
                candidateProvider.PoviderBulstat
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            document.Dispose();
            template.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateLicensingApplicationFirstLicenseAsync(CandidateProviderVM candidateProvider, ProcedureDocumentVM procedureDocument)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"/wwwroot/Templates/CPO/Application";

            FileStream template = new FileStream($@"{resources_Folder}/Obrazec_prilojenie_licenziq (1).docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            WordDocument document = new WordDocument(template, FormatType.Automatic);

            var location = await this.locationService.GetLocationByIdAsync(candidateProvider.IdLocation.Value);
            var professions = this.dataSourceService.GetAllProfessionsList();
            var kvVQSSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            var kvVQSI = kvVQSSource.FirstOrDefault(x => x.KeyValueIntCode == "I_VQS");
            var kvVQSII = kvVQSSource.FirstOrDefault(x => x.KeyValueIntCode == "II_VQS");
            var kvVQSIII = kvVQSSource.FirstOrDefault(x => x.KeyValueIntCode == "III_VQS");
            var kvVQSIV = kvVQSSource.FirstOrDefault(x => x.KeyValueIntCode == "IV_VQS");
            var addressJoined = string.Empty;
            if (candidateProvider.IdRegionAdmin.HasValue)
            {
                var regionInformation = await this.repository.GetByIdAsync<Region>(candidateProvider.IdRegionAdmin!.Value);
                if (regionInformation is not null)
                {
                    addressJoined = $"{location.LocationName}, р-н {regionInformation.RegionName}, {candidateProvider.ProviderAddress}";
                }
            }
            else
            {
                addressJoined = $"{location.LocationName}, {candidateProvider.ProviderAddress}";
            }

            string[] fieldNames = new string[]
            {
                "LicenseNumber", "Order", "ProviderOwner", "Location", "Bulstat"
            };

            string[] fieldValues = new string[]
            {
                candidateProvider.LicenceNumber,
                $"{procedureDocument.DS_OFFICIAL_DocNumber}/{procedureDocument.DS_OFFICIAL_DATE.Value.ToString("dd.MM.yyyy")}г.",
                $"\"{candidateProvider.ProviderOwner}\"",
                addressJoined,
                candidateProvider.PoviderBulstat
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            var professionsAndSpecialities = new Dictionary<int, List<SpecialityVM>>();
            foreach (var candidateProviderSpeciality in candidateProvider.CandidateProviderSpecialities)
            {
                if (!professionsAndSpecialities.ContainsKey(candidateProviderSpeciality.Speciality.IdProfession))
                {
                    professionsAndSpecialities.Add(candidateProviderSpeciality.Speciality.IdProfession, new List<SpecialityVM>());
                }

                professionsAndSpecialities[candidateProviderSpeciality.Speciality.IdProfession].Add(candidateProviderSpeciality.Speciality);
            }

            WTable table = new WTable(document, false);
            table.TableFormat.Borders.BorderType = BorderStyle.None;

            var professionCounter = 1;
            WTableRow row = table.AddRow();
            WTableCell cell = row.AddCell();

            foreach (var entry in professionsAndSpecialities)
            {
                var profession = professions.FirstOrDefault(x => x.IdProfession == entry.Key);
                IWTextRange textRange = cell.AddParagraph().AppendText($"{professionCounter}. Професия \"{profession.Name}\", код {profession.Code}");
                textRange.CharacterFormat.FontName = "Monotype Corsiva";
                textRange.CharacterFormat.FontSize = 14;
                textRange.CharacterFormat.Bold = true;

                table.Rows.Add(row);

                var specialityCounter = 1;
                foreach (var speciality in entry.Value.OrderBy(x => x.Code))
                {
                    var spkValue = string.Empty;
                    if (speciality.IdVQS == kvVQSI.IdKeyValue)
                    {
                        spkValue = "първа";
                    }
                    else if (speciality.IdVQS == kvVQSII.IdKeyValue)
                    {
                        spkValue = "втора";
                    }
                    else if (speciality.IdVQS == kvVQSIII.IdKeyValue)
                    {
                        spkValue = "трета";
                    }
                    else
                    {
                        spkValue = "четвърта";
                    }

                    IWTextRange textRange2 = cell.AddParagraph().AppendText($"    {professionCounter}.{specialityCounter++}. Специалност \"{speciality.Name}\", код {speciality.Code}, {spkValue} степен на професионална квалификация.");
                    textRange2.CharacterFormat.FontName = "Monotype Corsiva";
                    textRange2.CharacterFormat.FontSize = 14;
                }

                professionCounter++;
            }

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("SpecialitiesData", true, false);
            bookNav.InsertTable(table);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            document.Dispose();
            template.Close();

            return stream;
        }

        public async Task<MemoryStream> GenerateLicensingApplicationLicenseChangeAsync(CandidateProviderVM candidateProvider, ProcedureDocumentVM procedureDocument)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"/wwwroot/Templates/CPO/Application";

            FileStream template = new FileStream($@"{resources_Folder}/Obrazec_prilojenie_licenziq (2).docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            WordDocument document = new WordDocument(template, FormatType.Automatic);

            var location = await this.locationService.GetLocationByIdAsync(candidateProvider.IdLocation.Value);
            var professions = this.dataSourceService.GetAllProfessionsList();
            var kvVQSSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            var kvVQSI = kvVQSSource.FirstOrDefault(x => x.KeyValueIntCode == "I_VQS");
            var kvVQSII = kvVQSSource.FirstOrDefault(x => x.KeyValueIntCode == "II_VQS");
            var kvVQSIII = kvVQSSource.FirstOrDefault(x => x.KeyValueIntCode == "III_VQS");
            var kvVQSIV = kvVQSSource.FirstOrDefault(x => x.KeyValueIntCode == "IV_VQS");
            var addressJoined = string.Empty;
            if (candidateProvider.IdRegionAdmin.HasValue)
            {
                var regionInformation = await this.repository.GetByIdAsync<Region>(candidateProvider.IdRegionAdmin!.Value);
                if (regionInformation is not null)
                {
                    addressJoined = $"{location.LocationName}, р-н {regionInformation.RegionName}, {candidateProvider.ProviderAddress}";
                }
            }
            else
            {
                addressJoined = $"{location.LocationName}, {candidateProvider.ProviderAddress}";
            }

            string[] fieldNames = new string[]
            {
                "LicenseNumber", "Order", "ProviderOwner", "Location", "Bulstat"
            };

            string[] fieldValues = new string[]
            {
                candidateProvider.LicenceNumber,
                $"{procedureDocument.DS_OFFICIAL_DocNumber}/{procedureDocument.DS_OFFICIAL_DATE.Value.ToString("dd.MM.yyyy")}г.",
                $"\"{candidateProvider.ProviderOwner}\"",
                addressJoined,
                candidateProvider.PoviderBulstat
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            var professionsAndSpecialities = new Dictionary<int, List<SpecialityVM>>();
            foreach (var candidateProviderSpeciality in candidateProvider.CandidateProviderSpecialities)
            {
                if (!professionsAndSpecialities.ContainsKey(candidateProviderSpeciality.Speciality.IdProfession))
                {
                    professionsAndSpecialities.Add(candidateProviderSpeciality.Speciality.IdProfession, new List<SpecialityVM>());
                }

                professionsAndSpecialities[candidateProviderSpeciality.Speciality.IdProfession].Add(candidateProviderSpeciality.Speciality);
            }

            WTable table = new WTable(document, false);
            table.TableFormat.Borders.BorderType = BorderStyle.None;

            var professionCounter = 1;
            WTableRow row = table.AddRow();
            WTableCell cell = row.AddCell();

            foreach (var entry in professionsAndSpecialities)
            {
                var profession = professions.FirstOrDefault(x => x.IdProfession == entry.Key);
                IWTextRange textRange = cell.AddParagraph().AppendText($"{professionCounter}. Професия \"{profession.Name}\", код {profession.Code}");
                textRange.CharacterFormat.FontName = "Monotype Corsiva";
                textRange.CharacterFormat.FontSize = 14;
                textRange.CharacterFormat.Bold = true;

                table.Rows.Add(row);

                var specialityCounter = 1;
                foreach (var speciality in entry.Value.OrderBy(x => x.Code))
                {
                    var spkValue = string.Empty;
                    if (speciality.IdVQS == kvVQSI.IdKeyValue)
                    {
                        spkValue = "първа";
                    }
                    else if (speciality.IdVQS == kvVQSII.IdKeyValue)
                    {
                        spkValue = "втора";
                    }
                    else if (speciality.IdVQS == kvVQSIII.IdKeyValue)
                    {
                        spkValue = "трета";
                    }
                    else
                    {
                        spkValue = "четвърта";
                    }

                    IWTextRange textRange2 = cell.AddParagraph().AppendText($"    {professionCounter}.{specialityCounter++}. Специалност \"{speciality.Name}\", код {speciality.Code}, {spkValue} степен на професионална квалификация.");
                    textRange2.CharacterFormat.FontName = "Monotype Corsiva";
                    textRange2.CharacterFormat.FontSize = 14;
                }

                professionCounter++;
            }

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("SpecialitiesData", true, false);
            bookNav.InsertTable(table);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            document.Dispose();
            template.Close();

            return stream;
        }

        public MemoryStream GenerateSpecialitiesReference(CandidateProviderVM candidateProvider)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 60;
                sheet.Range["A1"].Text = "Специалност";
                sheet.Range["B1"].ColumnWidth = 60;
                sheet.Range["B1"].Text = "Професия";
                sheet.Range["C1"].ColumnWidth = 60;
                sheet.Range["C1"].Text = "Професионално направление";
                sheet.Range["D1"].ColumnWidth = 60;
                sheet.Range["D1"].Text = "Област";

                IRange range = sheet.Range["A1"];
                IRichTextString boldText = range.RichText;
                IFont boldFont = workbook.CreateFont();

                boldFont.Bold = true;
                boldText.SetFont(0, sheet.Range["A1"].Text.Length, boldFont);

                IRange range2 = sheet.Range["B1"];
                IRichTextString boldText2 = range2.RichText;
                IFont boldFont2 = workbook.CreateFont();

                boldFont2.Bold = true;
                boldText2.SetFont(0, sheet.Range["B1"].Text.Length, boldFont2);

                IRange range3 = sheet.Range["C1"];
                IRichTextString boldText3 = range3.RichText;
                IFont boldFont3 = workbook.CreateFont();

                boldFont3.Bold = true;
                boldText3.SetFont(0, sheet.Range["C1"].Text.Length, boldFont3);

                IRange range4 = sheet.Range["D1"];
                IRichTextString boldText4 = range4.RichText;
                IFont boldFont4 = workbook.CreateFont();

                boldFont4.Bold = true;
                boldText4.SetFont(0, sheet.Range["D1"].Text.Length, boldFont4);

                var professionalDirectionsSource = this.dataSourceService.GetAllProfessionalDirectionsList();
                var professionsSource = this.dataSourceService.GetAllProfessionsList();
                var areasSource = this.dataSourceService.GetAllAreasList();

                var rowCounter = 2;
                foreach (var providerSpeciality in candidateProvider.CandidateProviderSpecialities)
                {
                    var profession = professionsSource.FirstOrDefault(x => x.IdProfession == providerSpeciality.Speciality.IdProfession);
                    var professionalDirection = professionalDirectionsSource.FirstOrDefault(x => x.IdProfessionalDirection == profession.IdProfessionalDirection);
                    var area = areasSource.FirstOrDefault(x => x.IdArea == professionalDirection.IdArea);
                    sheet.Range[$"A{rowCounter}"].Text = $"{providerSpeciality.Speciality.Code} {providerSpeciality.Speciality.Name}";
                    sheet.Range[$"B{rowCounter}"].Text = $"{profession.Code} {profession.Name}";
                    sheet.Range[$"C{rowCounter}"].Text = $"{professionalDirection.Code} {professionalDirection.Name}";
                    sheet.Range[$"D{rowCounter++}"].Text = $"{area.Code} {area.Name}";
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }
    }
}
