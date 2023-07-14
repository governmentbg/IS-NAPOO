using ISNAPOO.Core.Contracts.Licensing;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.EKATTE;
using Data.Models.Common;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc;
using ISNAPOO.Core.ViewModels.SPPOO;
using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;

namespace ISNAPOO.Core.Services.Licensing
{
    public class LicensingProcedureDocumentCIPOService : BaseService, ILicensingProcedureDocumentCIPOService
    {
        private readonly ICandidateProviderService candidateProviderService;
        private readonly ILocationService locationService;
        private readonly IRepository repository;

        public LicensingProcedureDocumentCIPOService(ICandidateProviderService candidateProviderService,
            ILocationService locationService, IRepository repository) 
            : base (repository)

        {
            this.repository = repository;
            this.candidateProviderService = candidateProviderService;
            this.locationService = locationService;
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
                "CIPOName",
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
                "CipoName",
                "CompanyName",
                "CityName",
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

        public async Task<MemoryStream> GenerateApplication_4(CPOLicensingApplication4 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContractNumber", "DateOfDraft", "OrderNumber", "OrderInputDate", "CipoName",
                "CompanyName", "CompanyCityName", "ExternalExpertName", "EGN",
                "IdCardNumber", "IdCardIssueDate", "IdCardCityOfIssue", "AddressByIdCard",
                "TaxOffice", "CompanyId", "ContractTerm"
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
                "CipoName",
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
                "ContactPersonSirname",
                "CipoName",
                "CompanyName",
                "CityName",
                "ChiefExpert",
            };

            string[] fieldValues = new string[]
            {
                application.ContactPersonData.FullName,
                application.ContactPersonData.CityName,
                application.ContactPersonData.PostCode,
                application.ContactPersonData.Sirname,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ChiefExpert,
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
                "CipoName"
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
                "CipoName",
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
                "CipoName",
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
                    "ExternalExpert", "CipoName", "CompanyName", "CityName", "OrderNumber",
                    "OrderInputDate","FromDate","ToDate",
                    //"1.", "1.1.1.", "1.1.2.", "1.2.1.", "1.2.2.", "1.2.3.", "1.2.4.",
                    //"2.",
                    //"3.", "3.1.1.", "3.1.2.", "3.2.1.",
                    //"4.", "4.1.1.", "4.1.2.", "4.1.3.", "4.2.1.",
                    "MeanScore"
                };

            string[] fieldValues = new string[]
                {
                    application.ExternalExpertDataVM.Person.FullName,
                    application.CPOMainData.CPOName,
                    application.CPOMainData.CompanyName,
                    application.CPOMainData.CityName,
                    application.OrderNumber,
                    application.OrderInputDateFormatted,
                    application.PeriodOfOrderCompletion.FromDateFormatted,
                    application.PeriodOfOrderCompletion.ToDateFormatted,
                    //tableData[1][0].Value.ToString("n2"),
                    //tableData[1][1].Value.ToString("n2"),
                    //tableData[1][2].Value.ToString("n2"),
                    //tableData[1][3].Value.ToString("n2"),
                    //tableData[1][4].Value.ToString("n2"),
                    //tableData[1][5].Value.ToString("n2"),
                    //tableData[1][6].Value.ToString("n2"),
                    //tableData[2][0].Value.ToString("n2"),
                    //tableData[3][0].Value.ToString("n2"),
                    //tableData[3][1].Value.ToString("n2"),
                    //tableData[3][2].Value.ToString("n2"),
                    //tableData[3][3].Value.ToString("n2"),
                    //tableData[4][0].Value.ToString("n2"),
                    //tableData[4][1].Value.ToString("n2"),
                    //tableData[4][2].Value.ToString("n2"),
                    //tableData[4][3].Value.ToString("n2"),
                    //tableData[4][4].Value.ToString("n2"),
                    application.MeanScore
                };

            document.MailMerge.Execute(fieldNames, fieldValues);

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
                "Time", "OrderNumber", "OrderInputDate", "CipoName", "CompanyName", "CityName", "ChiefExpert"
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
                "TaxOffice", "CipoName", "CompanyName", "CityName", "CompanyId", "OrderNumber", "OrderInputDate"
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
                "TaxOffice", "CipoName", "CompanyName", "CityName", "CompanyId", "OrderNumber", "OrderInputDate"
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
                "DistanceConnectionSoftwareName", "OrderNumber", "CipoName",
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

            //bookNav.MoveToBookmark("ExpertCommissionScores", true, false);

            //foreach (var score in application.ExpertCommissionScores)
            //{
            //    WParagraph scoresParagraph = new WParagraph(document);
            //    scoresParagraph.AppendText($"По професионално направление „{score.Key}“ обща оценка по всички критерии - {score.Value}")
            //        .CharacterFormat.FontSize = 12;
            //    scoresParagraph.ParagraphFormat.AfterSpacing = 1;
            //    scoresParagraph.ParagraphFormat.BeforeSpacing = 1;

            //    scoresParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;

            //    bookNav.InsertParagraph(scoresParagraph);
            //}

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
                "CipoName", "CompanyName", "CityName", "ApplicationNumber", "ApplicationInputDate",
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

            //bookNav.MoveToBookmark("ExpertCommissionScores", true, false);

            //foreach (var score in application.ExpertCommissionScores)
            //{
            //    WParagraph scoresParagraph = new WParagraph(document);
            //    scoresParagraph.AppendText($"По професионално направление „{score.Key}“ обща оценка по всички критерии - {score.Value}")
            //        .CharacterFormat.FontSize = 12;
            //    scoresParagraph.ParagraphFormat.AfterSpacing = 1;
            //    scoresParagraph.ParagraphFormat.BeforeSpacing = 1;

            //    scoresParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;

            //    bookNav.InsertParagraph(scoresParagraph);
            //}

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
                "ChiefExpert", "CipoName", "CompanyName", "CityName", "OrderNumber",
                "OrderInputDate", "ExpertCommissionName", "ProtocolNumber", "ProtocolInputDate",
                "ExpertCommissionChairmanSirname", "ReportNumber", "ReportInputDate", "ExpertCommissionChairman"
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
                application.ReportNumber,
                application.ReportInputDateFormatted,
                application.ExpertCommissionChairmanFullName
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

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
                "CipoName",
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
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

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
                "CipoName", "CompanyName", "CityName", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPersonData.FullName,
                application.ContactPersonData.StreetName,
                application.ContactPersonData.CityName,
                application.ContactPersonData.PostCode,
                application.ContactPersonData.Sirname.ToUpper(),
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

        public async Task<MemoryStream> GenerateApplication_21(CPOLicensingApplication21 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "OrderNumber", "OrderInputDate", "ExpertCommissionName", "ProtocolNumber", "д",
                "CipoName", "CompanyName", "CityName", "App19OrderNumber", "App19OrderInputDate", "ChiefExpert"
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

            try
            {
                bookNav.MoveToBookmark("ExpertCommissionChairman", true, false);
            }
            catch (System.Exception)
            {
                throw new System.Exception("Bookmark not found!");
            }

            WTable table = new WTable(document);
            table.TableFormat.Borders.BorderType = BorderStyle.Cleared;

            WCharacterFormat nameColumnCharStyle = new WCharacterFormat(document);
            nameColumnCharStyle.FontSize = 12;
            Syncfusion.Drawing.Font font = new Syncfusion.Drawing.Font() { FontFamilyName = "Times New Roman", Name = "Times New Roman", Size = 12 } ;
            nameColumnCharStyle.Font = font;
           

            table.ResetCells(1, 2);

            table.Rows[0].Height = 20;
            table[0, 0].AddParagraph().AppendText(application.ExpertCommissionChairman).ApplyCharacterFormat(nameColumnCharStyle);

            IWParagraph remunerationCell = table[0, 1].AddParagraph();
            remunerationCell.AppendText("55,00 лв.").ApplyCharacterFormat(nameColumnCharStyle);
            remunerationCell.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;

            bookNav.InsertTable(table);

            try
            {
                bookNav.MoveToBookmark("MembersList", true, false);
            }
            catch (System.Exception)
            {
                throw new System.Exception("Bookmark not found!");
            }

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

        public async Task<MemoryStream> GenerateLicensingApplication(CandidateProviderVM candidateProvider, IEnumerable<KeyValueVM> kvApplicationFilingSource, IEnumerable<KeyValueVM> kvReceiveLicenseSource, IEnumerable<KeyValueVM> kvVQSSource)
        {
            var applicationFiling = candidateProvider.IdApplicationFiling;
            var receiveLicense = candidateProvider.IdReceiveLicense;
            candidateProvider = await this.candidateProviderService.GetCandidateProviderByIdAsync(candidateProvider);
            candidateProvider.IdApplicationFiling = applicationFiling;
            candidateProvider.IdReceiveLicense = receiveLicense;

            LocationVM locationCorrespondence = await this.locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(candidateProvider.IdLocationCorrespondence.Value);
            LocationVM locationAdmin = await this.locationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(candidateProvider.IdLocation.Value);

            var resources_Folder = Directory.GetCurrentDirectory() + @"/wwwroot/Templates/CIPO/Application";

            FileStream template = new FileStream($@"{resources_Folder}/Zayavlenie_CIPO_ЯВ-09.06.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

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
                "ProviderOwner", "ProviderName", "Bulstat", "ManagerName", "Location",
                "Website", "LocationCorrespondence", "District", "Municipality", "ZIPCode", "Address",
                "Contacts", "Email", "PersonCorrespondence", "Date", "Name", "UIN"
            };

            string[] fieldValues = new string[]
            {
                candidateProvider.ProviderOwner,
                candidateProvider.ProviderName,
                candidateProvider.PoviderBulstat,
                candidateProvider.AttorneyName ?? candidateProvider.ManagerName,
                locationAdmin.LocationName,
                candidateProvider.ProviderWeb,
                locationCorrespondence.LocationName,
                locationCorrespondence.Municipality.District.DistrictName,
                locationCorrespondence.Municipality.MunicipalityName,
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

        public async Task<long> GenerateLicenseNumberAsync(CandidateProviderVM candidateProvider)
        {
            var dateValue = candidateProvider.LicenceDate.Value.ToString("yyyy");
            var providerTypeInfo = "14";

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
            var resources_Folder = Directory.GetCurrentDirectory() + @"/wwwroot/Templates/CIPO/Application";

            FileStream template = new FileStream($@"{resources_Folder}/Licenzia_CIPO_0712.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

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
    }
}
