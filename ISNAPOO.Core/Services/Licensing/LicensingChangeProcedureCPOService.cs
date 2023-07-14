namespace ISNAPOO.Core.Services.Licensing
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Syncfusion.DocIO;
    using Syncfusion.DocIO.DLS;

    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.Contracts.Licensing;
    using ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc;
    using ISNAPOO.Core.ViewModels.SPPOO;
    using ISNAPOO.Core.ViewModels.Common;
    using System.Threading.Tasks;
    using ISNAPOO.Core.ViewModels.CPO.ProviderData;

    public class LicensingChangeProcedureCPOService : ILicensingChangeProcedureCPOService
    {

        public async Task<MemoryStream> GenerateApplication_1(CPOLicenseChangeApplication1 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ChiefExpert",
                "ApplicationNumber",
                "ApplicationInputDate",
                "CPOName",
                "CompanyName",
                "CityName",
                "LicenseNumber",
                "ExpertCommissionName",
                "ExpertCommissionReportTerm"
            };

            string[] fieldValues = new string[]
            {
                application.ChiefExpert,
                application.ApplicationNumber,
                application.ApplicationDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.LicenseNumber,
                application.ExpertCommissionName,
                application.ExpertCommissionReportTermFormatted
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ExpertsList", true, false);

            IWParagraph expertsParagraph = new WParagraph(document);

            bookNav.InsertParagraph(expertsParagraph);

            ListStyle expertsListStyle = document.AddListStyle(ListType.Bulleted, "ExpertsList");

            WListLevel expertListLevel = expertsListStyle.Levels[0];
            expertListLevel.FollowCharacter = FollowCharacterType.Tab;
            expertListLevel.CharacterFormat.FontSize = 12;
            expertListLevel.TextPosition = 0;

            GetAndPrintProcedureExperts(document, bookNav, application.ProcedureExternalExperts);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_2(CPOLicenseChangeApplication2 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            WSection section = document.Sections[0] as WSection;

            string[] fieldNames = new string[]
            {
                "OrderNumber",
                "OrderDate",
                "LicenseNumber",
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
                application.LicenseNumber,
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

            BookmarksNavigator bookNav = new BookmarksNavigator(document);

            bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("ExternalExperts", true, false);

            IWParagraph paragraph = new WParagraph(document);

            bookNav.InsertParagraph(paragraph);

            ListStyle expertsListStyle = document.AddListStyle(ListType.Bulleted, "ExpertsList");

            WListLevel expertListLevel = expertsListStyle.Levels[0];
            expertListLevel.FollowCharacter = FollowCharacterType.Tab;
            expertListLevel.CharacterFormat.FontSize = 12;
            expertListLevel.TextPosition = 0;

            GetAndPrintProcedureExperts(document, bookNav, application.ProcedureExternalExperts);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_3(CPOLicenseChangeApplication3 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContactPerson", "City", "PostCode", "StrName", "ContactPersonSirname",
                "ApplicationNumber", "ApplicationInputDate", "CpoName", "CompanyName", "CityName",
                "LicenseNumber", "OrderNumber", "OrderInputDate", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPerson.FullName,
                application.ContactPerson.CityName,
                application.ContactPerson.PostCode,
                application.ContactPerson.StreetName,
                application.ContactPersonSirname,
                application.ApplicationNumber,
                application.ApplicationInputDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.LicenseNumber,
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.ChiefExpert
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

            GetAndPrintProcedureExperts(document, bookNav, application.ProcedureExternalExperts);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_4(CPOLicenseChangeApplication4 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContractNumber", "DateOfDraft", "OrderNumber", "OrderInputDate", "CpoName",
                "CompanyName", "CityName", "ExternalExpertName", "EGN",
                "IdCardNumber", "IdCardIssueDate", "IdCardCityOfIssue", "AddressByIdCard",
                "TaxOffice", "CompanyId", "ExternalExpertProfessionalDirection", "ContractTerm"
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
                application.ExpertDataVM.ProfessionalDirectionStr,
                application.ContractTermFormatted
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_6(CPOLicenseChangeApplication6 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ChiefExpert", "LicenseNumber", "CpoName", "CompanyName", "CityName",
                "ApplicationNumber", "ApplicationInputDate"
            };

            string[] fieldValues = new string[]
            {
                application.ChiefExpert,
                application.LicenseNumber,
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
                expertsParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                bookNav.InsertParagraph(expertsParagraph);

                expertsParagraph.ListFormat.ApplyStyle("IssuesList");
            }

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_7(CPOLicenseChangeApplication7 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContactPerson", "City", "PostCode", "LicenseNumber", "CpoName", "CompanyName",
                "CityName", "ContactPersonSirname", "ChiefExpert", "TelephoneNumber", "EmailAddress"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPersonData.FullName,
                application.ContactPersonData.CityName,
                application.ContactPersonData.PostCode,
                application.LicenseNumber,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ContactPersonData.Sirname,
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
        public async Task<MemoryStream> GenerateApplication_8(CPOLicenseChangeApplication8 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ChiefExpert", "CpoName", "CompanyName",
                "CityName", "LicenseNumber", "ApplicationNumber",
                "ApplicationInputDate", "ReportNumber",
                "NotificationLetterNumber", "DueDate"
            };

            string[] fieldValues = new string[]
            {
                application.ChiefExpert,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.LicenseNumber,
                application.ApplicationNumber,
                application.ApplicationInputDateFormatted,
                application.ReportNumber,
                application.NotificationLetterNumber,
                application.DueDateFormatted
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_9(CPOLicenseChangeApplication9 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
               "OrderNumber", "OrderDate", "LicenseNumber", "CpoName", "CompanyName", "CityName", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.OrderNumber,
                application.OrderDateFormatted,
                application.LicenseNumber,
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
        public async Task<MemoryStream> GenerateApplication_10(CPOLicenseChangeApplication10 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContactPerson", "StrName", "City", "PostCode", "LicenseNumber",
                "CpoName", "CompanyName", "CityName", "ContactPersonSirname",
                "NotificationLetterNumber", "DueDate", "OrderNumber", "OrderDate", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPersonData.FullName,
                application.ContactPersonData.StreetName,
                application.ContactPersonData.CityName,
                application.ContactPersonData.PostCode,
                application.LicenseNumber,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ContactPersonData.Sirname,
                application.NotificationLetterNumber,
                application.DueDateFormatted,
                application.OrderNumber,
                application.OrderDateFormatted,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_11(CPOLicenseChangeApplication11 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            Dictionary<int, List<ScoreTableData>> tableData = application.ScoreTable;

            string[] fieldNames = new string[]
                {
                    "ExternalExpert", "LicenseNumber", "CpoName", "CompanyName", "CityName",
                    "ProfessionalDirection", "OrderNumber",
                    "OrderInputDate","FromDate","ToDate",
                    //"1.", "2.", "2.1.1.", "2.1.2.", "2.2.1.", "TotalScore",
                    //"MotivesForZeroScore"
                };

            string[] fieldValues = new string[]
                {
                    application.ExternalExpertDataVM.Person.FullName,
                    application.LicenseNumber,
                    application.CPOMainData.CPOName,
                    application.CPOMainData.CompanyName,
                    application.CPOMainData.CityName,
                    //string.Join(", ", application.ProfessionalDirections.Select(pd => pd.Name)),
                    application.ExternalExpertDataVM.ProfessionalDirectionStr,
                    application.OrderNumber,
                    application.OrderInputDateFormatted,
                    "......",
                    "......",
                    //tableData[1][0].Value.ToString("F"),
                    //tableData[2][0].Value.ToString("F"),
                    //tableData[2][1].Value.ToString("F"),
                    //tableData[2][2].Value.ToString("F"),
                    //tableData[2][3].Value.ToString("F"),
                    //application.TotalScore,
                    //application.MotivesForZeroScore
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
        public async Task<MemoryStream> GenerateApplication_13(CPOLicenseChangeApplication13 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "HeadOfExpertCommission", "NameOfExpertCommission", "DateOfMeeting", "DayOfWeek",
                "Time", "OrderNumber", "OrderDate", "CpoName", "CompanyName", "CityName", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.HeadOfExpertCommission,
                application.ExpertCommissionName,
                application.DateOfMeetingFormatted,
                application.DayOfWeek,
                application.MeetingHour,
                application.OrderNumber,
                application.OrderDateFormatted,
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
        public async Task<MemoryStream> GenerateApplication_14(CPOLicenseChangeApplication14 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContractNumber", "DateOfDraft", "NameOfExpertCommission", "ChiefExpert",
                "EGN", "IdCardNumber", "IdCardIssueDate", "IdCardCityOfIssue", "AddressByIdCard",
                "TaxOffice", "CpoName", "CompanyName", "CompanyId", "OrderNumber", "OrderDate"
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
                application.CPOMainData.CompanyId,
                application.OrderNumber,
                application.OrderDateFormatted,
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_15(CPOLicenseChangeApplication15 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContractNumber", "DateOfDraft", "ExpertCommissionName", "ExpertCommissionMemberName",
                "EGN", "IdCardNumber", "IdCardIssueDate", "IdCardCityOfIssue", "AddressByIdCard",
                "TaxOffice", "CpoName", "CompanyName", "CityName", "CompanyId", "OrderNumber", "OrderDate",
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
                application.OrderDateFormatted,
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_16(CPOLicenseChangeApplication16 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ProtocolNumber", "ExpertCommissionName", "DateOfDraft", "Time",
                "DistanceConnectionSoftwareName", "OrderNumber", "LicenseNumber",
                "CpoName", "CompanyName", "CityName", "OrderInputDate", "TotalScore",
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
                application.LicenseNumber,
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

            bookNav.MoveToBookmark("ExpertCommissionReviewResults", true, false);

            CreateScoreListByProfessionalDirection(document, application.ExpertCommissionScores);

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_17(CPOLicenseChangeApplication17 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ExpertCommissionChairman", "ExpertCommissionName", "OrderNumber", "OrderInputDate",
                "CpoName", "CompanyName", "CityName", "ApplicationNumber", "ApplicationInputDate",
                "ChiefExpert", "ProtocolNumber", "ProtocolInputDate", "TotalScore", "LicenseNumber"
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
                application.TotalScore,
                application.LicenseNumber
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            CreateScoreListByProfessionalDirection(document, application.ExpertCommissionScores);

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_18(CPOLicenseChangeApplication18 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ChiefExpert", "LicenseNumber", "CpoName", "CompanyName", "CityName", "OrderNumber",
                "OrderInputDate", "ExpertCommissionName", "ProtocolNumber", "ProtocolInputDate",
                "ExpertCommissionChairmanSirname", "ProfessionsCount",
                "SpecialitiesCount", "ReportNumber", "ReportInputDate", "ExpertCommissionChairman"
            };

            string[] fieldValues = new string[]
            {
                application.ChiefExpert,
                application.LicenseNumber,
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
        public async Task<MemoryStream> GenerateApplication_19(CPOLicenseChangeApplication19 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "OrderNumber",
                "ExpertCommissionName",
                "ReportNumber",
                "ReportInputDate",
                "ApplicationNumber",
                "ApplicationInputDate",
                "CpoName",
                "CompanyName",
                "CityName",
                "LicenseNumber",
                "ProfessionsCount",
                "SpecialitiesCount",
                "ChiefExpert"
            };

            string[] fieldValues = new string[] {
                application.OrderNumber,
                application.ExpertCommissionName,
                application.ReportNumber,
                application.ReportInputDateFormatted,
                application.ApplicationNumber,
                application.ApplicationInputDateFormatted,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.LicenseNumber,
                application.ProfessionsCount,
                application.SpecialitiesCount,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_20(CPOLicenseChangeApplication20 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "ContactPerson", "StrName", "City", "PostCode", "ContactPersonSirname",
                "LicenseNumber", "CpoName", "CompanyName", "CityName", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.ContactPersonData.FullName,
                application.ContactPersonData.StreetName,
                application.ContactPersonData.CityName,
                application.ContactPersonData.PostCode,
                application.ContactPersonData.Sirname,
                application.LicenseNumber,
                application.CPOMainData.CPOName,
                application.CPOMainData.CompanyName,
                application.CPOMainData.CityName,
                application.ChiefExpert
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            CreateProfessionalDirectionList(document, application.ProfessionalDirections);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            template.Close();
            document.Close();

            return stream;
        }
        public async Task<MemoryStream> GenerateApplication_21(CPOLicenseChangeApplication21 application, TemplateDocumentVM templateVM)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";

            FileStream template = new FileStream($@"{resources_Folder}{templateVM.TemplatePath}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);

            string[] fieldNames = new string[]
            {
                "OrderNumber","OrderDate", "ExpertCommissionName", "ProtocolNumber", "ProtocolInputDate", "LicenseNumber",
                "CpoName", "CompanyName", "CityName", "App19OrderNumber", "App19OrderInputDate", "ChiefExpert"
            };

            string[] fieldValues = new string[]
            {
                application.OrderNumber,
                application.OrderInputDateFormatted,
                application.ExpertCommissionName,
                application.ProtocolNumber,
                application.ProtocolInputDateFormatted,
                application.LicenseNumber,
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

        private static void CreateProfessionalDirectionList(WordDocument document, List<ProfessionalDirectionVM> professionalDirections)
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
            //paragraph.AppendBreak(BreakType.LineBreak);
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
                paragraph.AppendText($"Професионално направление: {profData.Name}, код {profData.Code}").ApplyCharacterFormat(profDirCharFormat);
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

        private static void CreateScoreListByProfessionalDirection(WordDocument document, Dictionary<string, string> expertCommissionScores)
        {
            BookmarksNavigator bookNav = new BookmarksNavigator(document);

            bookNav.MoveToBookmark("ExpertCommissionScores", true, false);

            foreach (var score in expertCommissionScores)
            {
                WParagraph scoresParagraph = new WParagraph(document);
                scoresParagraph.AppendBreak(BreakType.LineBreak);

                scoresParagraph.AppendText($"По професионално направление „{score.Key}“ обща оценка по всички критерии - {score.Value}")
                    .CharacterFormat.FontSize = 12;
                scoresParagraph.ParagraphFormat.AfterSpacing = 1;
                scoresParagraph.ParagraphFormat.BeforeSpacing = 1;

                scoresParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                bookNav.InsertParagraph(scoresParagraph);
            }
        }

        private static void GetAndPrintExperts(WordDocument document, BookmarksNavigator bookNav, List<Expert> experts)
        {
            Dictionary<string, List<string>> expertCollection = new Dictionary<string, List<string>>();

            foreach (var expert in experts)
            {
                string profDir = $"„{ expert.ProfessionalDirection }“";
                if (expertCollection.ContainsKey(expert.Name))
                {
                    expertCollection[expert.Name].Add(profDir);
                }
                else
                {
                    expertCollection.Add(expert.Name, new List<string> { profDir });
                }
            }

            PrintExperts(document, bookNav, expertCollection);
        }

        private static void GetAndPrintProcedureExperts(WordDocument document, BookmarksNavigator bookNav, List<ProcedureExternalExpertVM> experts)
        {
            Dictionary<string, List<string>> expertCollection = new Dictionary<string, List<string>>();

            foreach (var expert in experts)
            {
                string profDir = $"„{ expert.ProfessionalDirection.Name }“";
                if (expertCollection.ContainsKey(expert.Expert.Person.FullName))
                {
                    expertCollection[expert.Expert.Person.FullName].Add(profDir);
                }
                else
                {
                    expertCollection.Add(expert.Expert.Person.FullName, new List<string> { profDir });
                }
            }

            PrintExperts(document, bookNav, expertCollection);
        }

        private static void PrintExperts(WordDocument document, BookmarksNavigator bookNav, Dictionary<string, List<string>> expertCollection)
        {
            WCharacterFormat profDirCharStyle = SetProfessionalDirectionCharacterFormat(document);
            WCharacterFormat expertCharStyle = SetExpertsCharacterFormat(document);

            foreach (var expert in expertCollection)
            {
                IWParagraph profDirParagraph = new WParagraph(document);

                if (expert.Value.Count > 1)
                {
                    profDirParagraph.AppendText(@"По професионални направления: ").ApplyCharacterFormat(profDirCharStyle); ;

                    if (expert.Value.Count == 2)
                    {
                        profDirParagraph.AppendText(string.Join(" и ", expert.Value))
                            .ApplyCharacterFormat(profDirCharStyle);
                    }
                    else
                    {
                        profDirParagraph.AppendText(string.Join(", ", expert.Value.Take(expert.Value.Count() - 1)))
                            .ApplyCharacterFormat(profDirCharStyle);
                        profDirParagraph.AppendText($" и { expert.Value.Last() }")
                            .ApplyCharacterFormat(profDirCharStyle);
                    }
                }
                else
                {
                    profDirParagraph.AppendText("По професионалнo направлениe: ")
                        .ApplyCharacterFormat(profDirCharStyle);
                    profDirParagraph.AppendText($"{ expert.Value.First() }")
                        .ApplyCharacterFormat(profDirCharStyle);
                }

                profDirParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                bookNav.InsertParagraph(profDirParagraph);

                IWParagraph expertParagraph = new WParagraph(document);
                expertParagraph.ListFormat.ApplyStyle("ExpertsList");
                expertParagraph.AppendText($"{ expert.Key }").ApplyCharacterFormat(expertCharStyle);
                expertParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;

                bookNav.InsertParagraph(expertParagraph);
            }
        }

        private static WCharacterFormat SetProfessionalDirectionCharacterFormat(WordDocument document)
        {
            WCharacterFormat profDirCharFormat = new WCharacterFormat(document);
            profDirCharFormat.FontName = "Times New Roman";
            profDirCharFormat.FontSize = 12;
            profDirCharFormat.Position = 1;
            profDirCharFormat.Bold = true;

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
    }
}
