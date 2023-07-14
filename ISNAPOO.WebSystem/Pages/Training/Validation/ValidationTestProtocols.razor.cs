using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ISNAPOO.Core.ViewModels.SPPOO;
using Syncfusion.Blazor.Grids;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.EKATTE;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using ISNAPOO.Core.HelperClasses;
using Microsoft.CodeAnalysis;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class ValidationTestProtocols : BlazorBaseComponent
    {

        [Inject]
        public ITrainingService trainingService { get; set; }

        [Inject]
        public ISpecialityService specialityService { get; set; }

        [Inject]
        public IProfessionService professionService { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public ILocationService locationService { get; set; }                                              
        [Inject]
        public IMunicipalityService municipalityService { get; set; }
        [Inject]
        public IDistrictService districtService { get; set; }

        [Inject]
        public ICandidateProviderService providerService { get; set; }

        [Inject]
        public Microsoft.JSInterop.IJSRuntime JS { get; set; }

        [Inject]
        public ISettingService SettingService { get; set; }

        [Inject]
        public ITemplateDocumentService TemplateDocumentService { get; set; }

        private string ProtocolNumber = "";

        EditContext editContext;
        ValidationClientVM validationClientVM = new ValidationClientVM();
        ValidationProtocolVM validationProtocolVM = new ValidationProtocolVM();
        CourseProtocol1 courseProtocol1 = new CourseProtocol1();
        CourseProtocol2 courseProtocol2 = new CourseProtocol2();
        CourseProtocol2 courseProtocol3 = new CourseProtocol2();
        CourseProtocol4 courseProtocol4 = new CourseProtocol4();
        CourseProtocol5 courseProtocol5 = new CourseProtocol5();
        SfGrid<ValidationCommissionMemberVM> sfGrid = new SfGrid<ValidationCommissionMemberVM>();
        List<ValidationCommissionMemberVM> ClientsList = new List<ValidationCommissionMemberVM>();
        CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
        List<KeyValueVM> kvProtocolNumber = new List<KeyValueVM>();
        private string ProtocolTitle = "";
        private string settingsFolderPath = "";
        public string DateTimeNow = "";
        public string DocumentNumber = "";
        public int IdValidationProtocol;
        public async Task OpenModal(ValidationProtocolVM validationProtocolVM, int? IdChairman)
        {
            courseProtocol1 = new CourseProtocol1();
            courseProtocol2 = new CourseProtocol2();
            courseProtocol3 = new CourseProtocol2();
            courseProtocol4 = new CourseProtocol4();
            courseProtocol5 = new CourseProtocol5();
            try
            {
                this.validationProtocolVM = validationProtocolVM;
                this.validationClientVM = await this.trainingService.GetValidationClientByIdAsync(validationProtocolVM.IdValidationClient);
                this.kvProtocolNumber = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType", false)).ToList();
                this.DateTimeNow = validationProtocolVM.ValidationProtocolDate.Value.ToString(GlobalConstants.DATE_FORMAT);
                this.DocumentNumber = validationProtocolVM.ValidationProtocolNumber;
                this.ProtocolNumber = (await this.dataSourceService.GetKeyValueByIdAsync(validationProtocolVM.IdValidationProtocolType)).KeyValueIntCode;
                ProtocolTitle = this.kvProtocolNumber.First(x => x.KeyValueIntCode == this.ProtocolNumber).Name;
                if (IdChairman != null)
                {
                    this.courseProtocol4.IdChairman = (int)IdChairman;
                }
                switch (this.ProtocolNumber)
                {
                    case "3-79":
                        await GenerateCourseProtocol1();
                        break;
                    case "3-80p":
                        await GenerateCourseProtocol2();
                        break;
                    case "3-80t":
                        await GenerateCourseProtocol3();
                        break;
                    case "3-81B":
                        await GenerateCourseProtocol4();
                        break;
                    case "3-82":
                        await GenerateCourseProtocol5();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.ShowErrorAsync("Не е намерен курс с този номер.");
            }

        }

        protected override async Task OnInitializedAsync()
        {
            settingsFolderPath = (await this.SettingService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            this.isVisible = false;
        }
        private async Task OnClose()
        {
            this.isVisible = false;
            this.StateHasChanged();
        }

        #region 3-79
        private async Task GenerateCourseProtocol1()
        {
            try
            {
                courseProtocol1 = new CourseProtocol1();
                this.editContext = new EditContext(courseProtocol1);
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (validationClientVM.IdCandidateProvider == null ? 0 : validationClientVM.IdCandidateProvider) });
                int IdLocation = 0;
                IdLocation = (int)this.candidateProviderVM.IdLocation;
                courseProtocol1.ProviderName = candidateProviderVM.ProviderOwner;
                courseProtocol1.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol1.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol1.Clients.Add(this.validationClientVM);
                courseProtocol1.CourseCommissionMemberSource = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList();
                courseProtocol1.ChairmenCommisisonMembers = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList().Where(x => x.IsChairman == true).ToList();
                if (courseProtocol1.ChairmenCommisisonMembers.Count > 0)
                {
                    courseProtocol1.IdChairman = courseProtocol1.ChairmenCommisisonMembers.First().IdValidationCommissionMember;
                }
                courseProtocol1.CommisisonMembers = courseProtocol1.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != this.courseProtocol1.IdChairman).ToList();
                if (this.validationClientVM.IdSpeciality != null)
                {
                    var Speciality = await this.specialityService.GetSpecialityByIdAsync((int)this.validationClientVM.IdSpeciality);
                    var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                    courseProtocol1.SpecialityCodeAndName = Speciality.CodeAndName;
                    courseProtocol1.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                    courseProtocol1.ProfessionCodeAndName = Profession.CodeAndName;
                    courseProtocol1.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(this.validationClientVM.IdCourseType)).KeyValueIntCode == "ValidationOfProfessionalQualifications" ? "SPK" : "PartProfession";
                }
                courseProtocol1.Location = (await this.locationService.GetLocationByIdAsync(IdLocation));
                courseProtocol1.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol1.Location.idMunicipality));
                courseProtocol1.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol1.Municipality.idDistrict));
                courseProtocol1.DocumentNumber = this.DocumentNumber;
                courseProtocol1.DateTimeNow = this.DateTimeNow;
                courseProtocol1.Region = this.candidateProviderVM.RegionAdmin != null ? this.candidateProviderVM.RegionAdmin.RegionName : "";
                this.isVisible = true;
                this.StateHasChanged();
            }
            catch (Exception ex)
            {
                await this.ShowErrorAsync("Възникна грешка при генерирането на информация");
            }
        }
        private async Task SaveProtocol1()
        {
            if (courseProtocol1.CommisisonMembers.Count >= 2 && courseProtocol1.IdChairman != 0)
            {
                courseProtocol1.SelectedChairman = this.courseProtocol1.ChairmenCommisisonMembers.First(x => x.IdValidationCommissionMember == this.courseProtocol1.IdChairman).WholeName;
                string FileTemplateFilePath = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", (this.courseProtocol1.TypeOfProtocol == "SPK" ? "CPOTrainingCoursesProtocol_3-79" : "CPOTrainingCoursesProtocol_3-79_PartProfession"))).Result.IdKeyValue).TemplatePath;
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + FileTemplateFilePath;

                FileStream template = new FileStream($@"{resources_Folder}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(template, FormatType.Docx);

                string[] fieldNames = new string[]
                {
                "ProviderName",
                "Region",
                "LocationName",
                "MunicipalityName",
                "DocumentNumber",
                "DistrictName",
                "ProfessionCodeAndName",
                "SpecialityCodeAndName",
                "VQS",
                "VQSAcquisitionCourseName",
                "EducationForm",
                "DateTimeNow",
                "TypeOfProtocol",
                "SelectedChairman",
                "CPODirector"
                };

                string[] fieldValues = new string[]
                {
                courseProtocol1.ProviderName,
                courseProtocol1.Region,
                courseProtocol1.Location.LocationName,
                courseProtocol1.Municipality.MunicipalityName,
                courseProtocol1.DocumentNumber,
                courseProtocol1.District.DistrictName,
                courseProtocol1.ProfessionCodeAndName,
                courseProtocol1.SpecialityCodeAndName,
                courseProtocol1.VQS,
                courseProtocol1.VQSAcquisitionCourseName,
                courseProtocol1.EducationForm,
                this.DateTimeNow,
                courseProtocol1.TypeOfProtocol,
                courseProtocol1.SelectedChairman,
                courseProtocol1.ProviderManager
                };
                document.MailMerge.Execute(fieldNames, fieldValues);

                BookmarksNavigator bookNav = new BookmarksNavigator(document);
                bookNav.MoveToBookmark("MembersList", true, false);

                IWParagraphStyle paragraphStyle = document.AddParagraphStyle("MembersParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;

                IWParagraphStyle SignedParagraphStyle = document.AddParagraphStyle("MembersSignParagraph");
                SignedParagraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                SignedParagraphStyle.ParagraphFormat.Tabs.AddTab((float)446, TabJustification.Left, TabLeader.NoLeader);

                ListStyle membersListStyle = document.AddListStyle(ListType.Numbered, "MembersList");

                WListLevel memberstListLevel = membersListStyle.Levels[0];
                memberstListLevel.FollowCharacter = FollowCharacterType.Space;
                memberstListLevel.CharacterFormat.FontSize = (float)9.5;
                memberstListLevel.TextPosition = 1;

                WCharacterFormat membersCharacterFormat = SetMembersCharacterFormat(document);
                WCharacterFormat underLineCharactersFormat = SetUnderLineCharactersFormat(document);
                bookNav.MoveToBookmark("MembersList", true, false);
                IWParagraph temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol1.CommisisonMembers.Count; i++)
                {
                    var member = this.courseProtocol1.CommisisonMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                bookNav.MoveToBookmark("MembersList2", true, false);
                temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol1.CommisisonMembers.Count; i++)
                {
                    var member = this.courseProtocol1.CommisisonMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.................." + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име)" + "\t подпис" + "\n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersSignParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }


                bookNav.MoveToBookmark("AppcentCommisionMembersList", true, false);
                paragraphStyle = document.AddParagraphStyle("AppcentCommisionMembersParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;

                membersListStyle = document.AddListStyle(ListType.Numbered, "AppcentCommisionMembersList");

                memberstListLevel = membersListStyle.Levels[0];
                memberstListLevel.FollowCharacter = FollowCharacterType.Space;
                memberstListLevel.CharacterFormat.FontSize = (float)9.5;
                memberstListLevel.TextPosition = 1;

                for (int i = 0; i < this.courseProtocol1.AppcentCommisionMembers.Count; i++)
                {
                    var member = this.courseProtocol1.AppcentCommisionMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);


                    bookNav.InsertParagraph(membersParagraph);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("AppcentCommisionMembersParagraph");

                }

                Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                foreach (Syncfusion.DocIO.DLS.IWTable table in document.Sections[0].Tables)
                {
                    if (table.Title == "StudentsTable")
                    {
                        RowModel = table.Rows[2];
                        foreach (Syncfusion.DocIO.DLS.WTableCell item in RowModel.Cells)
                        {
                            for (int i = 0; i < item.Paragraphs.Count; i++)
                            {
                                item.Paragraphs.RemoveAt(i);
                            }
                        }
                        for (int i = table.Rows.Count; i > 2; --i)
                        {
                            table.Rows.Remove(table.Rows[i - 1]);
                        }
                        table.Rows.Add(RowModel);
                        for (int j = 1; j < courseProtocol1.Clients.Count(); j++)
                        {
                            table.AddRow(isCopyFormat: true);
                        }
                        if (courseProtocol1.Clients.Count > 0)
                        {
                            for (int x = 2; x < table.Rows.Count; x++)
                            {
                                table.Rows[x].Cells[0].AddParagraph().AppendText((x - 1).ToString() + ".");
                                table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol1.Clients[x - 2].WholeName);
                            }

                        }
                    }
                }

                MemoryStream stream = new MemoryStream();
                document.Save(stream, FormatType.Docx);
                template.Close();
                document.Close();
                await FileUtils.SaveAs(JS, "3-79.docx", stream.ToArray());
                this.isVisible = false;
                this.StateHasChanged();
            }
            else
            {
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия!");
            }
        }
        #endregion

        #region 3-80p
        private async Task GenerateCourseProtocol2()
        {
            try
            {
                courseProtocol2 = new CourseProtocol2();
                this.editContext = new EditContext(courseProtocol2);
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (validationClientVM.IdCandidateProvider == null ? 0 : validationClientVM.IdCandidateProvider) });
                int IdLocation = 0;
                IdLocation = (int)this.candidateProviderVM.IdLocation;
                courseProtocol2.ProviderName = candidateProviderVM.ProviderOwner;
                courseProtocol2.DateOfExam = this.validationClientVM != null ? this.validationClientVM.ExamPracticeDate != null ? this.validationClientVM.ExamPracticeDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "" : "";
                courseProtocol2.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol2.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol2.Clients.Add(this.validationClientVM);
                courseProtocol2.CourseCommissionMemberSource = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList();
                courseProtocol2.ChairmenCommisisonMembers = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList().Where(x => x.IsChairman == true).ToList();
                if (courseProtocol2.ChairmenCommisisonMembers.Count > 0)
                {
                    courseProtocol2.IdChairman = courseProtocol2.ChairmenCommisisonMembers.First().IdValidationCommissionMember;
                }
                courseProtocol2.CommisisonMembers = courseProtocol2.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != this.courseProtocol2.IdChairman).ToList();
                if (this.validationClientVM.IdSpeciality != null)
                {
                    var Speciality = await this.specialityService.GetSpecialityByIdAsync((int)this.validationClientVM.IdSpeciality);
                    var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                    courseProtocol2.SpecialityCodeAndName = Speciality.CodeAndName;
                    courseProtocol2.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                    courseProtocol2.ProfessionCodeAndName = Profession.CodeAndName;
                    courseProtocol2.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(this.validationClientVM.IdCourseType)).KeyValueIntCode == "ValidationOfProfessionalQualifications" ? "SPK" : "PartProfession";
                }
                // courseProtocol2.EducationForm = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdFormEducation)).Name;
                courseProtocol2.Location = (await this.locationService.GetLocationByIdAsync(IdLocation));
                courseProtocol2.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol2.Location.idMunicipality));
                courseProtocol2.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol2.Municipality.idDistrict));
                courseProtocol2.DocumentNumber = this.DocumentNumber;
                courseProtocol2.DateTimeNow = this.DateTimeNow;
                courseProtocol2.Region = this.candidateProviderVM.RegionAdmin != null ? this.candidateProviderVM.RegionAdmin.RegionName : "";
                this.isVisible = true;
                this.StateHasChanged();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Датата на изпита не е попълнена")
                {
                    await this.ShowErrorAsync(ex.Message);
                }
                else
                {
                    await this.ShowErrorAsync("Възникна грешка при генерирането на информация");
                }
            }
        }
        private async Task SaveProtocol2()
        {
            if (courseProtocol2.CommisisonMembers.Count >= 2 && courseProtocol2.IdChairman != 0)
            {
                courseProtocol2.SelectedChairman = this.courseProtocol2.ChairmenCommisisonMembers.First(x => x.IdValidationCommissionMember == this.courseProtocol2.IdChairman).WholeName;
                string FileTemplateFilePath = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", (this.courseProtocol2.TypeOfProtocol == "SPK" ? "CPOTrainingCoursesProtocol_3-80" : "CPOTrainingCoursesProtocol_3-80_PartProfession")).Result.IdKeyValue)).TemplatePath;
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + FileTemplateFilePath;
                FileStream template = new FileStream($@"{resources_Folder}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(template, FormatType.Docx);
                //foreach (var student in courseprotocol2.clients)
                //{
                //    var tempclient = await this.trainingservice.getclientcoursedocumentbyidclientcourseasync(student.idclientcourse) == null ? await this.trainingservice.getclientcoursedocumentbyidclientcourseasync(student.idclientcourse) : new clientcoursedocumentvm();
                //    courseprotocol2.studentpointspair.add(student, tempclient);
                //}

                BookmarksNavigator bookNav = new BookmarksNavigator(document);
                bookNav.MoveToBookmark("MembersList", true, false);

                IWParagraphStyle paragraphStyle = document.AddParagraphStyle("MembersParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;

                IWParagraphStyle SignedParagraphStyle = document.AddParagraphStyle("MembersSignParagraph");
                SignedParagraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                SignedParagraphStyle.ParagraphFormat.Tabs.AddTab((float)446, TabJustification.Left, TabLeader.NoLeader);

                ListStyle membersListStyle = document.AddListStyle(ListType.Numbered, "MembersList");

                WListLevel memberstListLevel = membersListStyle.Levels[0];
                memberstListLevel.FollowCharacter = FollowCharacterType.Space;
                memberstListLevel.CharacterFormat.FontSize = (float)9.5;
                memberstListLevel.TextPosition = 1;

                WCharacterFormat membersCharacterFormat = SetMembersCharacterFormat(document);
                WCharacterFormat underLineCharactersFormat = SetUnderLineCharactersFormat(document);
                bookNav.MoveToBookmark("MembersList", true, false);
                IWParagraph temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol2.CommisisonMembers.Count; i++)
                {
                    var member = this.courseProtocol2.CommisisonMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                bookNav.MoveToBookmark("MembersList2", true, false);
                temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol2.CommisisonMembers.Count; i++)
                {
                    var member = this.courseProtocol2.CommisisonMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.................." + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име)" + "\t подпис" + "\n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersSignParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                Syncfusion.DocIO.DLS.WTableRow RowModel = null;


                foreach (Syncfusion.DocIO.DLS.IWTable table in document.Sections[0].Tables)
                {
                    if (table.Title == "StudentsTable")
                    {
                        RowModel = table.Rows[1];
                        foreach (Syncfusion.DocIO.DLS.WTableCell item in RowModel.Cells)
                        {
                            for (int i = 0; i < item.Paragraphs.Count; i++)
                            {
                                item.Paragraphs.RemoveAt(i);
                            }
                        }
                        for (int i = table.Rows.Count; i > 1; --i)
                        {
                            table.Rows.Remove(table.Rows[i - 1]);
                        }
                        table.Rows.Add(RowModel);
                        for (int j = 1; j < courseProtocol2.Clients.Count(); j++)
                        {
                            table.AddRow(isCopyFormat: true);
                        }
                        for (int x = 1; x < table.Rows.Count; x++)
                        {
                            //var grade = courseProtocol2.StudentPointsPair[x - 1].Grade.ToString().Replace(".", ",").ToString();
                            table.Rows[x].Cells[0].AddParagraph().AppendText((x).ToString());
                            table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol2.Clients[x - 1].WholeName);                           
                            //table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol2.Clients.FirstOrDefault(y => y.IdClientCourse == courseProtocol2.StudentPointsPair[x - 1].IdClientCourse).WholeName);
                            //table.Rows[x].Cells[3].AddParagraph().AppendText(String.Format("{0:0.00}", double.Parse(grade) / double.Parse(vqs.DefaultValue3)));
                            //table.Rows[x].Cells[4].AddParagraph().AppendText(courseProtocol2.StudentPointsPair[x - 1].Grade.ToString());

                        }
                    }
                }

                string[] fieldNames = new string[]
                {
                "ProviderName",
                "Region",
                "LocationName",
                "DocumentNumber",
                "MunicipalityName",
                "DistrictName",
                "DateOfExam",
                "ProfessionCodeAndName",
                "SpecialityCodeAndName",
                "VQS",
                "CourseName",
                "Subject",
                "EducationForm",
                "DateTimeNow",
                "MarkMultiplier",
                "TypeOfProtocol",
                "SelectedChairman",
                "CPODirector"
                };

                string[] fieldValues = new string[]
                {
                courseProtocol2.ProviderName,
                courseProtocol2.Region,
                courseProtocol2.Location.LocationName,
                courseProtocol2.DocumentNumber,
                courseProtocol2.Municipality.MunicipalityName,
                courseProtocol2.District.DistrictName,
                courseProtocol2.DateOfExam,
                courseProtocol2.ProfessionCodeAndName,
                courseProtocol2.SpecialityCodeAndName,
                courseProtocol2.VQS,
                courseProtocol2.CourseName,
                courseProtocol2.Subject,
                courseProtocol2.EducationForm,
                this.DateTimeNow,
                courseProtocol2.MarkMultiplier,
                courseProtocol2.TypeOfProtocol,
                courseProtocol2.SelectedChairman,
                courseProtocol2.ProviderManager,
                };

                document.MailMerge.Execute(fieldNames, fieldValues);

                MemoryStream stream = new MemoryStream();
                document.Save(stream, FormatType.Docx);
                template.Close();
                document.Close();
                await FileUtils.SaveAs(JS, "3-80- с точки практика 1 2 и 3 СПК.docx", stream.ToArray());
                this.isVisible = false;
                this.StateHasChanged();
            }
            else
            {
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия!");
            }
        }
        #endregion

        #region 3-80t
        private async Task GenerateCourseProtocol3()
        {
            try
            {
                courseProtocol3 = new CourseProtocol2();
                this.editContext = new EditContext(courseProtocol3);
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (validationClientVM.IdCandidateProvider == null ? 0 : validationClientVM.IdCandidateProvider) });
                int IdLocation = 0;
                IdLocation = (int)this.candidateProviderVM.IdLocation;
                courseProtocol3.ProviderName = candidateProviderVM.ProviderOwner;
                courseProtocol3.DateOfExam = this.validationClientVM != null ? this.validationClientVM.ExamTheoryDate != null ? this.validationClientVM.ExamTheoryDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "" : "";
                courseProtocol3.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol3.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol3.Clients.Add(this.validationClientVM);
                courseProtocol3.CourseCommissionMemberSource = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList();
                courseProtocol3.ChairmenCommisisonMembers = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList().Where(x => x.IsChairman == true).ToList();
                if (courseProtocol3.ChairmenCommisisonMembers.Count > 0)
                {
                    courseProtocol3.IdChairman = courseProtocol3.ChairmenCommisisonMembers.First().IdValidationCommissionMember;
                }
                courseProtocol3.CommisisonMembers = courseProtocol3.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != this.courseProtocol3.IdChairman).ToList();
                if (this.validationClientVM.IdSpeciality != null)
                {
                    var Speciality = await this.specialityService.GetSpecialityByIdAsync((int)this.validationClientVM.IdSpeciality);
                    var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                    courseProtocol3.SpecialityCodeAndName = Speciality.CodeAndName;
                    courseProtocol3.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                    courseProtocol3.ProfessionCodeAndName = Profession.CodeAndName;
                    //courseProtocol3.EducationForm = (await this.dataSourceService.GetKeyValueByIdAsync(validationClientVM.IdFormEducation)).Name;
                    courseProtocol3.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(validationClientVM.IdCourseType)).KeyValueIntCode == "ValidationOfProfessionalQualifications" ? "SPK" : "PartProfession";
                }
                courseProtocol3.Location = (await this.locationService.GetLocationByIdAsync(IdLocation));
                courseProtocol3.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol3.Location.idMunicipality));
                courseProtocol3.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol3.Municipality.idDistrict));
                courseProtocol3.DocumentNumber = this.DocumentNumber;
                courseProtocol3.DateTimeNow = this.DateTimeNow;
                courseProtocol3.Region = this.candidateProviderVM.RegionAdmin != null ? this.candidateProviderVM.RegionAdmin.RegionName : "";
                this.isVisible = true;
                this.StateHasChanged();
            }
            catch (Exception ex)
            {
                await this.ShowErrorAsync("Възникна грешка при генерирането на информация");
            }
        }
        private async Task SaveProtocol3()
        {
            if (courseProtocol3.CommisisonMembers.Count >= 2 && courseProtocol3.IdChairman != 0)
            {
                courseProtocol3.SelectedChairman = this.courseProtocol3.ChairmenCommisisonMembers.First(x => x.IdValidationCommissionMember == this.courseProtocol3.IdChairman).WholeName;
                string FileTemplateFilePath = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", (this.courseProtocol3.TypeOfProtocol == "SPK" ? "CPOTrainingCoursesProtocol_3-80_Theory" : "CPOTrainingCoursesProtocol_3-80_Theory_PartProfession"))).Result.IdKeyValue).TemplatePath;
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + FileTemplateFilePath;
                FileStream template = new FileStream($@"{resources_Folder}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(template, FormatType.Docx);
                //foreach (var student in courseProtocol3.Clients)
                //{
                //    var tempClient = await this.trainingService.GetClientCourseDocumentByIdClientCourseAsync(student.IdClientCourse) == null ? await this.trainingService.GetClientCourseDocumentByIdClientCourseAsync(student.IdClientCourse) : new ClientCourseDocumentVM();
                //    courseProtocol3.StudentPointsPair.Add(student, tempClient);
                //}

                BookmarksNavigator bookNav = new BookmarksNavigator(document);
                bookNav.MoveToBookmark("MembersList", true, false);

                IWParagraphStyle paragraphStyle = document.AddParagraphStyle("MembersParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;

                IWParagraphStyle SignedParagraphStyle = document.AddParagraphStyle("MembersSignParagraph");
                SignedParagraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                SignedParagraphStyle.ParagraphFormat.Tabs.AddTab((float)446, TabJustification.Left, TabLeader.NoLeader);

                ListStyle membersListStyle = document.AddListStyle(ListType.Numbered, "MembersList");

                WListLevel memberstListLevel = membersListStyle.Levels[0];
                memberstListLevel.FollowCharacter = FollowCharacterType.Space;
                memberstListLevel.CharacterFormat.FontSize = (float)9.5;
                memberstListLevel.TextPosition = 1;

                WCharacterFormat membersCharacterFormat = SetMembersCharacterFormat(document);
                WCharacterFormat underLineCharactersFormat = SetUnderLineCharactersFormat(document);
                bookNav.MoveToBookmark("MembersList", true, false);
                IWParagraph temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol3.CommisisonMembers.Count; i++)
                {
                    var member = this.courseProtocol3.CommisisonMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                bookNav.MoveToBookmark("MembersList2", true, false);
                temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol3.CommisisonMembers.Count; i++)
                {
                    var member = this.courseProtocol3.CommisisonMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.................." + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име)" + "\t подпис" + "\n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersSignParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                foreach (Syncfusion.DocIO.DLS.IWTable table in document.Sections[0].Tables)
                {
                    if (table.Title == "StudentsTable")
                    {
                        RowModel = table.Rows[1];
                        foreach (Syncfusion.DocIO.DLS.WTableCell item in RowModel.Cells)
                        {
                            for (int i = 0; i < item.Paragraphs.Count; i++)
                            {
                                item.Paragraphs.RemoveAt(i);
                            }
                        }
                        for (int i = table.Rows.Count; i > 1; --i)
                        {
                            table.Rows.Remove(table.Rows[i - 1]);
                        }
                        table.Rows.Add(RowModel);
                        if (courseProtocol3.StudentPointsPair.Count() <= 0)
                        {
                            for (int j = 1; j < courseProtocol3.Clients.Count; j++)
                            {
                                table.AddRow(isCopyFormat: true);
                            }
                            for (int x = 1; x < table.Rows.Count; x++)
                            {
                                table.Rows[x].Cells[0].AddParagraph().AppendText((x).ToString());
                                table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol3.Clients[x - 1].WholeName);
                            }
                        }
                        else
                        {
                            for (int j = 1; j < courseProtocol3.Clients.Count(); j++)
                            {
                                table.AddRow(isCopyFormat: true);
                            }
                            for (int x = 1; x < table.Rows.Count; x++)
                            {
                                //var grade = courseProtocol3.StudentPointsPair[x - 1].Grade.ToString().Replace(".", ",").ToString();
                                table.Rows[x].Cells[0].AddParagraph().AppendText((x).ToString());
                                table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol3.Clients[x - 1].WholeName);
                                //table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol3.Clients.FirstOrDefault(y => y.IdClientCourse == courseProtocol3.StudentPointsPair[x - 1].IdClientCourse).WholeName);
                                //table.Rows[x].Cells[3].AddParagraph().AppendText(String.Format("{0:0.00}", double.Parse(grade) / double.Parse(vqs.DefaultValue3)));
                                //table.Rows[x].Cells[4].AddParagraph().AppendText(courseProtocol3.StudentPointsPair[x - 1].Grade.ToString());
                            }
                        }
                    }
                }

                string[] fieldNames = new string[]
                {
                "ProviderName",
                "Region",
                "LocationName",
                "MunicipalityName",
                "DistrictName",
                "DocumentNumber",
                "DateOfExam",
                "ProfessionCodeAndName",
                "SpecialityCodeAndName",
                "VQS",
                "CourseName",
                "Subject",
                "EducationForm",
                "DateTimeNow",
                "MarkMultiplier",
                "TypeOfProtocol",
                "SelectedChairman",
                "CPODirector"
                };

                string[] fieldValues = new string[]
                {
                courseProtocol3.ProviderName,
                courseProtocol3.Region,
                courseProtocol3.Location.LocationName,
                courseProtocol3.Municipality.MunicipalityName,
                courseProtocol3.District.DistrictName,
                courseProtocol3.DocumentNumber,
                courseProtocol3.DateOfExam,
                courseProtocol3.ProfessionCodeAndName,
                courseProtocol3.SpecialityCodeAndName,
                courseProtocol3.VQS,
                courseProtocol3.CourseName,
                courseProtocol3.Subject,
                courseProtocol3.EducationForm,
                this.DateTimeNow,
                courseProtocol3.MarkMultiplier,
                courseProtocol3.TypeOfProtocol,
                courseProtocol3.SelectedChairman,
                courseProtocol3.ProviderManager,
                };

                document.MailMerge.Execute(fieldNames, fieldValues);

                MemoryStream stream = new MemoryStream();
                document.Save(stream, FormatType.Docx);
                template.Close();
                document.Close();
                await FileUtils.SaveAs(JS, "3-80- с точки теория 1 2 3 СПК.docx", stream.ToArray());
                this.isVisible = false;
                this.StateHasChanged();
            }
            else
            {
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия!");
            }
        }
        #endregion

        #region 3-81B
        private async Task GenerateCourseProtocol4()
        {
            try
            {
                this.editContext = new EditContext(courseProtocol4);
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (validationClientVM.IdCandidateProvider == null ? 0 : validationClientVM.IdCandidateProvider) });
                int IdLocation = 0;
                IdLocation = (int)this.candidateProviderVM.IdLocation;
                courseProtocol4.ProviderName = candidateProviderVM.ProviderOwner;
                courseProtocol4.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol4.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol4.Clients.Add(this.validationClientVM);
                courseProtocol4.CourseCommissionMemberSource = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList();
                courseProtocol4.ChairmenCommisisonMembers = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList().Where(x => x.IsChairman == true).ToList();
                if (courseProtocol4.ChairmenCommisisonMembers.Count > 0 && courseProtocol4.IdChairman != 0)
                {
                    courseProtocol4.IdChairman = courseProtocol4.ChairmenCommisisonMembers.First().IdValidationCommissionMember;
                }
                courseProtocol4.CommisisonMembers = courseProtocol4.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != this.courseProtocol4.IdChairman).ToList();
                if (this.validationClientVM.IdSpeciality != null)
                {
                    var Speciality = await this.specialityService.GetSpecialityByIdAsync((int)this.validationClientVM.IdSpeciality);
                    var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                    courseProtocol4.SpecialityCodeAndName = Speciality.CodeAndName;
                    courseProtocol4.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                    courseProtocol4.ProfessionCodeAndName = Profession.CodeAndName;
                    //courseProtocol4.EducationForm = (await this.dataSourceService.GetKeyValueByIdAsync(validationClientVM.IdFormEducation)).Name;
                    courseProtocol4.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(validationClientVM.IdCourseType)).KeyValueIntCode == "ValidationOfProfessionalQualifications" ? "SPK" : "PartProfession";
                }
                courseProtocol4.Location = (await this.locationService.GetLocationByIdAsync(IdLocation));
                courseProtocol4.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol4.Location.idMunicipality));
                courseProtocol4.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol4.Municipality.idDistrict));
                courseProtocol4.DocumentNumber = this.DocumentNumber;
                courseProtocol4.DateTimeNow = this.DateTimeNow;
                courseProtocol4.Region = this.candidateProviderVM.RegionAdmin != null ? this.candidateProviderVM.RegionAdmin.RegionName : "";
                courseProtocol4.StudentPointPair = this.validationProtocolVM;
                this.isVisible = true;
                this.StateHasChanged();
            }
            catch (Exception ex)
            {
                await this.ShowErrorAsync("Възникна грешка при генерирането на информация");
            }
        }
        private async Task SaveProtocol4()
        {
            if (courseProtocol4.CommisisonMembers.Count >= 2 && courseProtocol4.IdChairman != 0)
            {
                courseProtocol4.SelectedChairman = this.courseProtocol4.ChairmenCommisisonMembers.First(x => x.IdValidationCommissionMember == this.courseProtocol4.IdChairman).WholeName;
                string FileTemplateFilePath = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", (this.courseProtocol4.TypeOfProtocol == "SPK" ? "CPOTrainingCoursesProtocol_3-81B" : "CPOTrainingCoursesProtocol_3-81B_PartProfession"))).Result.IdKeyValue).TemplatePath;
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + FileTemplateFilePath;
                FileStream template = new FileStream($@"{resources_Folder}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(template, FormatType.Docx);

                string[] fieldNames = new string[]
                {
                "ProviderName",
                "Region",
                "LocationName",
                "MunicipalityName",
                "DocumentNumber",
                "DistrictName",
                "ProfessionCodeAndName",
                "SpecialityCodeAndName",
                "VQS",
                "CourseName",
                "EducationForm",
                "DateTimeNow",
                "SelectedChairman",
                "CPODirector"
                };

                string[] fieldValues = new string[]
                {
                courseProtocol4.ProviderName,
                courseProtocol4.Region,
                courseProtocol4.Location.LocationName,
                courseProtocol4.Municipality.MunicipalityName,
                courseProtocol4.DocumentNumber,
                courseProtocol4.District.DistrictName,
                courseProtocol4.ProfessionCodeAndName,
                courseProtocol4.SpecialityCodeAndName,
                courseProtocol4.VQS,
                courseProtocol4.CourseName,
                courseProtocol4.EducationForm,
                this.DateTimeNow,
                courseProtocol4.SelectedChairman,
                courseProtocol4.ProviderManager,
                };
                document.MailMerge.Execute(fieldNames, fieldValues);

                BookmarksNavigator bookNav = new BookmarksNavigator(document);
                bookNav.MoveToBookmark("MembersList", true, false);

                IWParagraphStyle paragraphStyle = document.AddParagraphStyle("MembersParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;

                IWParagraphStyle SignedParagraphStyle = document.AddParagraphStyle("MembersSignParagraph");
                SignedParagraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                SignedParagraphStyle.ParagraphFormat.Tabs.AddTab((float)446, TabJustification.Left, TabLeader.NoLeader);

                ListStyle membersListStyle = document.AddListStyle(ListType.Numbered, "MembersList");

                WListLevel memberstListLevel = membersListStyle.Levels[0];
                memberstListLevel.FollowCharacter = FollowCharacterType.Space;
                memberstListLevel.CharacterFormat.FontSize = (float)9.5;
                memberstListLevel.TextPosition = 1;

                WCharacterFormat membersCharacterFormat = SetMembersCharacterFormat(document);
                WCharacterFormat underLineCharactersFormat = SetUnderLineCharactersFormat(document);
                bookNav.MoveToBookmark("MembersList", true, false);
                IWParagraph temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol4.CommisisonMembers.Count; i++)
                {
                    var member = this.courseProtocol4.CommisisonMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                bookNav.MoveToBookmark("MembersList2", true, false);
                temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol4.CommisisonMembers.Count; i++)
                {
                    var member = this.courseProtocol4.CommisisonMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.................." + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име)" + "\t подпис" + "\n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersSignParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }


                bookNav.MoveToBookmark("AppcentCommisionMembersList", true, false);
                paragraphStyle = document.AddParagraphStyle("AppcentCommisionMembersParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;

                membersListStyle = document.AddListStyle(ListType.Numbered, "AppcentCommisionMembersList");

                memberstListLevel = membersListStyle.Levels[0];
                memberstListLevel.FollowCharacter = FollowCharacterType.Space;
                memberstListLevel.CharacterFormat.FontSize = (float)9.5;
                memberstListLevel.TextPosition = 1;

                for (int i = 0; i < this.courseProtocol4.AppcentCommisionMembers.Count; i++)
                {
                    var member = this.courseProtocol4.AppcentCommisionMembers[i];
                    IWParagraph membersParagraph = new WParagraph(document);


                    bookNav.InsertParagraph(membersParagraph);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("AppcentCommisionMembersParagraph");

                }

                Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                foreach (Syncfusion.DocIO.DLS.IWTable table in document.Sections[0].Tables)
                {
                    if (table.Title == "StudentsTable")
                    {
                        RowModel = table.Rows[2];
                        foreach (Syncfusion.DocIO.DLS.WTableCell item in RowModel.Cells)
                        {
                            for (int i = 0; i < item.Paragraphs.Count; i++)
                            {
                                item.Paragraphs.RemoveAt(i);
                            }
                        }
                        for (int i = table.Rows.Count; i > 2; --i)
                        {
                            table.Rows.Remove(table.Rows[i - 1]);
                        }
                        table.Rows.Add(RowModel);
                        for (int j = 1; j < courseProtocol4.Clients.Count; j++)
                        {
                            table.AddRow(isCopyFormat: true);
                        }
                        for (int x = 2; x < table.Rows.Count; x++)
                        {
                            table.Rows[x].Cells[0].AddParagraph().AppendText((x - 1).ToString() + ".");
                            table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol4.Clients.FirstOrDefault(y => y.IdValidationClient == courseProtocol4.StudentPointPair.ValidationProtocolGrades.First().IdValidationClient).WholeName);
                            switch (courseProtocol4.StudentPointPair.ValidationProtocolGrades.First().Grade)
                            {
                                case >= 5.5:
                                    table.Rows[x].Cells[5].AddParagraph().AppendText("Отличен");
                                    break;
                                case >= 4.5:
                                    table.Rows[x].Cells[5].AddParagraph().AppendText("Много добър");
                                    break;
                                case >= 3.5:
                                    table.Rows[x].Cells[5].AddParagraph().AppendText("Добър");
                                    break;
                                case >= 2.5:
                                    table.Rows[x].Cells[5].AddParagraph().AppendText("Среден");
                                    break;
                                default:
                                    table.Rows[x].Cells[5].AddParagraph().AppendText("Слаб");
                                    break;
                            }

                            table.Rows[x].Cells[6].AddParagraph().AppendText(string.Format("{0:0.00}" , courseProtocol4.StudentPointPair.ValidationProtocolGrades.First().Grade));
                        }
                    }
                }

                MemoryStream stream = new MemoryStream();
                document.Save(stream, FormatType.Docx);
                template.Close();
                document.Close();
                await FileUtils.SaveAs(JS, "3-81B.docx", stream.ToArray());
                this.isVisible = false;
                this.StateHasChanged();
            }
            else
            {
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия!");
            }
        }
        #endregion

        #region 3-82
        private async Task GenerateCourseProtocol5()
        {
            try
            {
                courseProtocol5 = new CourseProtocol5();
                this.editContext = new EditContext(courseProtocol5);
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (validationClientVM.IdCandidateProvider == null ? 0 : validationClientVM.IdCandidateProvider) });
                int IdLocation = 0;
                IdLocation = (int)this.candidateProviderVM.IdLocation;
                courseProtocol5.ProviderName = candidateProviderVM.ProviderOwner;
                courseProtocol5.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol5.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol5.Clients.Add(this.validationClientVM);
                courseProtocol5.CourseCommissionMemberSource = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList();
                courseProtocol5.ChairmenCommisisonMembers = (await this.trainingService.GetAllValidationCommissionMembersByClient(validationClientVM.IdValidationClient)).ToList().Where(x => x.IsChairman == true).ToList();
                courseProtocol5.CommisisonMembers = courseProtocol5.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != this.courseProtocol5.IdChairman).ToList(); if (this.validationClientVM.IdSpeciality != null)
                {
                    var Speciality = await this.specialityService.GetSpecialityByIdAsync((int)this.validationClientVM.IdSpeciality);
                    var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                    courseProtocol5.SpecialityCodeAndName = Speciality.CodeAndName;
                    courseProtocol5.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                    courseProtocol5.ProfessionCodeAndName = Profession.CodeAndName;
                    courseProtocol5.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(validationClientVM.IdCourseType)).KeyValueIntCode == "ValidationOfProfessionalQualifications" ? "SPK" : "PartProfession";

                }
                //courseProtocol5.EducationForm = (await this.dataSourceService.GetKeyValueByIdAsync(validationClientVM.IdFormEducation)).Name;
                courseProtocol5.Location = (await this.locationService.GetLocationByIdAsync(IdLocation));
                courseProtocol5.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol5.Location.idMunicipality));
                courseProtocol5.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol5.Municipality.idDistrict));
                courseProtocol5.DocumentNumber = this.DocumentNumber;
                courseProtocol5.DateTimeNow = this.DateTimeNow;
                courseProtocol5.Region = this.candidateProviderVM.RegionAdmin != null ? this.candidateProviderVM.RegionAdmin.RegionName : "";
                this.isVisible = true;
                this.StateHasChanged();
            }
            catch (Exception ex)
            {
                await this.ShowErrorAsync("Възникна грешка при генерирането на информация");
            }
        }
        private async Task SaveProtocol5()
        {
            if (courseProtocol5.CommisisonMembers.Count >= 2)
            {
                //courseProtocol5.SelectedChairman = this.courseProtocol5.ChairmenCommisisonMembers.First(x => x.IdValidationCommissionMember == this.courseProtocol5.IdChairman).WholeName;
                string FileTemplateFilePath = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", (this.courseProtocol5.TypeOfProtocol == "SPK" ? "CPOTrainingCoursesProtocol_3-82" : "CPOTrainingCoursesProtocol_3-82_PartProfession"))).Result.IdKeyValue).TemplatePath;
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + FileTemplateFilePath;
                FileStream template = new FileStream($@"{resources_Folder}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(template, FormatType.Docx);

                string[] fieldNames = new string[]
                {
                "ProviderName",
                "Region",
                "LocationName",
                "MunicipalityName",
                "DocumentNumber",
                "DistrictName",
                "ProfessionCodeAndName",
                "SpecialityCodeAndName",
                "VQS",
                "VQSAcquisitionCourseName",
                "EducationForm",
                "DateTimeNow",
                "CPODirector",
                "SelectedChairman"
                };

                string[] fieldValues = new string[]
                {
                courseProtocol5.ProviderName,
                courseProtocol5.Region,
                courseProtocol5.Location.LocationName,
                courseProtocol5.Municipality.MunicipalityName,
                courseProtocol5.DocumentNumber,
                courseProtocol5.District.DistrictName,
                courseProtocol5.ProfessionCodeAndName,
                courseProtocol5.SpecialityCodeAndName,
                courseProtocol5.VQS,
                courseProtocol5.VQSAcquisitionCourseName,
                courseProtocol5.EducationForm,
                this.DateTimeNow,
                courseProtocol5.ProviderManager,
                courseProtocol5.SelectedChairman,
                };
                document.MailMerge.Execute(fieldNames, fieldValues);

                Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                foreach (Syncfusion.DocIO.DLS.IWTable table in document.Sections[0].Tables)
                {
                    if (table.Title == "StudentsTable")
                    {
                        RowModel = table.Rows[4];
                        foreach (Syncfusion.DocIO.DLS.WTableCell item in RowModel.Cells)
                        {
                            for (int i = 0; i < item.Paragraphs.Count; i++)
                            {
                                item.Paragraphs.RemoveAt(i);
                            }
                        }
                        for (int i = table.Rows.Count; i > 3; --i)
                        {
                            table.Rows.Remove(table.Rows[i - 1]);
                        }
                        table.Rows.Add(RowModel);
                        for (int j = 1; j < courseProtocol5.Clients.Count(); j++)
                        {
                            table.AddRow(isCopyFormat: true);
                        }
                        for (int x = 3; x < table.Rows.Count; x++)
                        {
                            table.Rows[x].Cells[0].AddParagraph().AppendText((x - 2).ToString() + ".");
                            table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol5.Clients[x - 3].WholeName);
                        }
                    }
                }

                RowModel = null;
                foreach (Syncfusion.DocIO.DLS.IWTable table in document.Sections[0].Tables)
                {
                    if (table.Title == "TeachersTable")
                    {
                        RowModel = table.Rows[1];
                        foreach (Syncfusion.DocIO.DLS.WTableCell item in RowModel.Cells)
                        {
                            for (int i = 0; i < item.Paragraphs.Count; i++)
                            {
                                item.Paragraphs.RemoveAt(i);
                            }
                        }
                        for (int i = table.Rows.Count; i > 1; --i)
                        {
                            table.Rows.Remove(table.Rows[i - 1]);
                        }
                        table.Rows.Add(RowModel);
                        for (int j = 1; j < courseProtocol5.CommisisonMembers.Count(); j++)
                        {
                            table.AddRow(isCopyFormat: true);
                        }
                        for (int x = 1; x < table.Rows.Count; x++)
                        {
                            table.Rows[x].Cells[0].AddParagraph().AppendText(x.ToString() + ". " + "От..........ч. .......... мин.");

                            table.Rows[x].Cells[1].AddParagraph().AppendText(x.ToString() + ". " + courseProtocol5.CommisisonMembers[x - 1].WholeName);

                            table.Rows[x].Cells[2].AddParagraph().AppendText(x.ToString() + ". .................");
                        }
                    }
                }
                MemoryStream stream = new MemoryStream();
                document.Save(stream, FormatType.Docx);
                template.Close();
                document.Close();
                await FileUtils.SaveAs(JS, "3-82.docx", stream.ToArray());
                this.isVisible = false;
                this.StateHasChanged();
            }
            else
            {
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия!");
            }
        }
        #endregion

        private static WCharacterFormat SetMembersCharacterFormat(WordDocument document)
        {
            WCharacterFormat membersCharFormat = new WCharacterFormat(document);
            membersCharFormat.FontName = "Verdana";
            membersCharFormat.FontSize = (float)9.5;
            membersCharFormat.Position = 0;
            membersCharFormat.Italic = false;
            membersCharFormat.Bold = false;

            return membersCharFormat;
        }
        private static WCharacterFormat SetUnderLineCharactersFormat(WordDocument document)
        {
            WCharacterFormat membersCharFormat = new WCharacterFormat(document);
            membersCharFormat.FontName = "Verdana";
            membersCharFormat.FontSize = (float)6.5;
            membersCharFormat.Position = 0;
            membersCharFormat.Italic = true;
            membersCharFormat.Bold = false;

            return membersCharFormat;
        }
        private async Task RemoveCommissionMember(ValidationCommissionMemberVM member, int id)
        {
            switch (this.ProtocolNumber)
            {
                case "3-79":
                    if (member.IdValidationCommissionMember != this.courseProtocol1.IdChairman)
                    {
                        this.courseProtocol1.AppcentCommisionMembers.Add(member);
                    }
                    this.courseProtocol1.CommisisonMembers.Remove(member);
                    break;
                case "3-80p":
                    if (member.IdValidationCommissionMember != this.courseProtocol2.IdChairman)
                    {
                        this.courseProtocol2.AppcentCommisionMembers.Add(member);

                    }
                    this.courseProtocol2.CommisisonMembers.Remove(member);
                    break;
                case "3-80t":
                    if (member.IdValidationCommissionMember != this.courseProtocol3.IdChairman)
                    {
                        this.courseProtocol3.AppcentCommisionMembers.Add(member);
                    }

                    this.courseProtocol3.CommisisonMembers.Remove(member);
                    break;
                case "3-81B":
                    if (member.IdValidationCommissionMember != this.courseProtocol4.IdChairman)
                    {
                        this.courseProtocol4.AppcentCommisionMembers.Add(member);
                    }
                    this.courseProtocol4.CommisisonMembers.Remove(member);
                    break;
                case "3-82":
                    if (member.IdValidationCommissionMember != this.courseProtocol5.IdChairman)
                    {
                        this.courseProtocol5.AppcentCommisionMembers.Add(member);
                    }
                    this.courseProtocol5.CommisisonMembers.Remove(member);
                    break;
                default:
                    break;
            }
            this.sfGrid.Refresh();
        }
        private async Task SelectChairman(SelectEventArgs<ValidationCommissionMemberVM> args)
        {
            switch (this.ProtocolNumber)
            {
                case "3-79":
                    this.courseProtocol1.CommisisonMembers = this.courseProtocol1.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != args.ItemData.IdValidationCommissionMember).ToList();
                    break;
                case "3-80p":
                    this.courseProtocol2.CommisisonMembers = this.courseProtocol2.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != args.ItemData.IdValidationCommissionMember).ToList();
                    break;
                case "3-80t":
                    this.courseProtocol3.CommisisonMembers = this.courseProtocol3.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != args.ItemData.IdValidationCommissionMember).ToList();
                    break;
                case "3-81B":
                    this.courseProtocol4.CommisisonMembers = this.courseProtocol4.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != args.ItemData.IdValidationCommissionMember).ToList();
                    break;
                case "3-82":
                    this.courseProtocol5.CommisisonMembers = this.courseProtocol5.CourseCommissionMemberSource.Where(x => x.IdValidationCommissionMember != args.ItemData.IdValidationCommissionMember).ToList();
                    break;
                default:
                    break;
            }
            this.sfGrid.Refresh();
        }
    }

    public class CourseProtocol1
    {
        public string CourseName { get; set; }
        public string ProviderName { get; set; }
        public string ProviderOwner { get; set; }
        public string ProviderManager { get; set; }

        public string Region { get; set; }
        public LocationVM Location { get; set; }
        public DistrictVM District { get; set; }
        public MunicipalityVM Municipality { get; set; }
        public string DocumentNumber { get; set; }
        public string ProfessionCodeAndName { get; set; }
        public string SpecialityCodeAndName { get; set; }
        public string VQS { get; set; }
        public string VQSAcquisitionCourseName { get; set; }
        public string EducationForm { get; set; }
        public int IdChairman { get; set; }
        public string TypeOfProtocol { get; set; }

        public string DateTimeNow = DateTime.Now.ToString(GlobalConstants.DATE_FORMAT);
        public string SelectedChairman { get; set; }

        public List<ValidationCommissionMemberVM> CommisisonMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> ChairmenCommisisonMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> CourseCommissionMemberSource = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> AppcentCommisionMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationClientVM> Clients = new List<ValidationClientVM>();
    }

    public class CourseProtocol2
    {
        public string CourseName { get; set; }
        public string ProviderName { get; set; }
        public string ProviderOwner { get; set; }
        public string ProviderManager { get; set; }
        public string Region { get; set; }
        public LocationVM Location { get; set; }
        public DistrictVM District { get; set; }
        public MunicipalityVM Municipality { get; set; }
        public string DocumentNumber { get; set; }
        public string ProfessionCodeAndName { get; set; }
        public string SpecialityCodeAndName { get; set; }
        public string VQS { get; set; }
        public string VQSAcquisitionCourseName { get; set; }

        public string Subject = "..............";
        public string EducationForm { get; set; }
        public int IdChairman { get; set; }
        public string TypeOfProtocol { get; set; }
        public string DateOfExam { get; set; }
        public string MarkMultiplier { get; set; }
        public string SelectedChairman { get; set; }

        public string DateTimeNow = DateTime.Now.ToString(GlobalConstants.DATE_FORMAT);

        public List<ValidationCommissionMemberVM> CommisisonMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> ChairmenCommisisonMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> CourseCommissionMemberSource = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> AppcentCommisionMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationClientVM> Clients = new List<ValidationClientVM>();

        public List<CourseProtocolGradeVM> StudentPointsPair = new List<CourseProtocolGradeVM>();
    }

    public class CourseProtocol4
    {
        public string CourseName { get; set; }
        public string ProviderName { get; set; }
        public string ProviderOwner { get; set; }
        public string ProviderManager { get; set; }
        public string Region { get; set; }
        public LocationVM Location { get; set; }
        public DistrictVM District { get; set; }
        public MunicipalityVM Municipality { get; set; }
        public string DocumentNumber { get; set; }
        public string ProfessionCodeAndName { get; set; }
        public string SpecialityCodeAndName { get; set; }
        public string VQS { get; set; }
        public string EducationForm { get; set; }
        public int IdChairman { get; set; }

        public string DateTimeNow = DateTime.Now.ToString(GlobalConstants.DATE_FORMAT);
        public string SelectedChairman { get; set; }
        public string TypeOfProtocol { get; set; }

        public List<ValidationCommissionMemberVM> CommisisonMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> ChairmenCommisisonMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> CourseCommissionMemberSource = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> AppcentCommisionMembers = new List<ValidationCommissionMemberVM>();

        public ValidationProtocolVM StudentPointPair = new ValidationProtocolVM();

        public List<ValidationClientVM> Clients = new List<ValidationClientVM>();
    }

    public class CourseProtocol5
    {
        public string CourseName { get; set; }
        public string ProviderName { get; set; }
        public string ProviderOwner { get; set; }
        public string ProviderManager { get; set; }
        public string Region { get; set; }
        public LocationVM Location { get; set; }
        public DistrictVM District { get; set; }
        public MunicipalityVM Municipality { get; set; }
        public string DocumentNumber { get; set; }
        public string ProfessionCodeAndName { get; set; }
        public string SpecialityCodeAndName { get; set; }
        public string VQS { get; set; }
        public string VQSAcquisitionCourseName { get; set; }
        public string EducationForm { get; set; }
        public int IdChairman { get; set; }
        public string TypeOfProtocol { get; set; }

        public string DateTimeNow = DateTime.Now.ToString(GlobalConstants.DATE_FORMAT);
        public string SelectedChairman { get; set; }

        public List<ValidationCommissionMemberVM> CommisisonMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> ChairmenCommisisonMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> CourseCommissionMemberSource = new List<ValidationCommissionMemberVM>();

        public List<ValidationCommissionMemberVM> AppcentCommisionMembers = new List<ValidationCommissionMemberVM>();

        public List<ValidationClientVM> Clients = new List<ValidationClientVM>();

        public List<ValidationClientVM> AppcentClients = new List<ValidationClientVM>();
    }
}

