
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
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
using Data.Models.Data.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using System.Runtime.CompilerServices;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class TestProtocols : BlazorBaseComponent
    {
    #region DI
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

        [Inject]
        public IRegionService regionService { get; set; }

        #endregion
        private string ProtocolNumber = "";

        EditContext editContext;
        public CourseVM CourseVM = new CourseVM();
        CourseProtocol1 courseProtocol1 = new CourseProtocol1();
        CourseProtocol2 courseProtocol2 = new CourseProtocol2();
        CourseProtocol2 courseProtocol3 = new CourseProtocol2();
        CourseProtocol4 courseProtocol4 = new CourseProtocol4();
        CourseProtocol5 courseProtocol5 = new CourseProtocol5();
        SfGrid<CourseCommissionMemberVM> sfGrid = new SfGrid<CourseCommissionMemberVM>();
        List<ClientCourseVM> ClientsList = new List<ClientCourseVM>();
        CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
        List<KeyValueVM> kvProtocolNumber = new List<KeyValueVM>();
        private string ProtocolTitle = "";
        private string settingsFolderPath = "";
        public string DateTimeNow = "";
        public string DocumentNumber = "";
        private List<CourseProtocolGradeVM> courseProtocolGrades = new List<CourseProtocolGradeVM>();
        public async Task OpenModal(int ProtocolNumber, int CourseId, DateTime DateTimeNow, string DocumentNumber, List<CourseProtocolGradeVM> courseProtocolGradeVMs, int? IdSelectedChairman)
        {
            this.ClientsList = new List<ClientCourseVM>();
            courseProtocol1 = new CourseProtocol1();
            courseProtocol2 = new CourseProtocol2();
            courseProtocol3 = new CourseProtocol2();
            courseProtocol4 = new CourseProtocol4();
            courseProtocol5 = new CourseProtocol5();
            try
            {
                if (courseProtocolGradeVMs != null ? courseProtocolGradeVMs.Count == 0 : true)
                {
                    this.ClientsList = (await this.trainingService.GetCourseClientsByIdCourseAsync(CourseVM.IdCourse)).ToList();
                }
                else
                {
                    foreach (var protocolgrade in courseProtocolGradeVMs)
                    {
                        var temp = await this.trainingService.GetTrainingClientCourseByIdAsync(protocolgrade.IdClientCourse);
                        this.ClientsList.Add(temp);
                    }
                }
                this.kvProtocolNumber = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType", false)).ToList();
                this.DateTimeNow = DateTimeNow.ToString(GlobalConstants.DATE_FORMAT);
                this.DocumentNumber = DocumentNumber;
                this.ProtocolNumber = (await this.dataSourceService.GetKeyValueByIdAsync(ProtocolNumber)).KeyValueIntCode;
                this.courseProtocolGrades = courseProtocolGradeVMs;
                CourseVM = await this.trainingService.GetTrainingCourseByIdAsync(CourseId);
                ProtocolTitle = this.kvProtocolNumber.First(x => x.KeyValueIntCode == this.ProtocolNumber).Name;
                if (IdSelectedChairman != null)
                {
                    this.courseProtocol4.IdChairman = (int)IdSelectedChairman;
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
        #region 3-70
        private async Task GenerateCourseProtocol1()
        {
            try
            {
                courseProtocol1 = new CourseProtocol1();
                this.editContext = new EditContext(courseProtocol1);
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (CourseVM.IdCandidateProvider == null ? 0 : CourseVM.IdCandidateProvider).Value });
                if ((await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdTrainingCourseType)).KeyValueIntCode == "ProfessionalQualification")
                {

                }
                int IdLocationCorrespondence = 0;
                IdLocationCorrespondence = (int)this.candidateProviderVM.IdLocationCorrespondence;
                courseProtocol1.ProviderName = $"{candidateProviderVM.ProviderName} към {candidateProviderVM.ProviderOwner}";
                courseProtocol1.CourseName = CourseVM.CourseName;
                courseProtocol1.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol1.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol1.CourseCommissionMemberSource = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList();
                courseProtocol1.ChairmenCommisisonMembers = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList().Where(x => x.IsChairman == true).ToList();
                if (courseProtocol1.ChairmenCommisisonMembers.Count > 0)
                {
                    courseProtocol1.IdChairman = courseProtocol1.ChairmenCommisisonMembers.First().IdCourseCommissionMember;
                    if (courseProtocol1.IdChairman != 0 )
                    {
                        this.courseProtocol1.CommisisonMembers = this.courseProtocol1.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != this.courseProtocol1.IdChairman).ToList();
                    } 
                }
                courseProtocol1.Clients = this.ClientsList; //(await this.trainingService.GetCourseClientsByIdCourseAsync(CourseVM.IdCourse)).ToList();
                var CourseProgram = await this.trainingService.GetTrainingProgramByIdAsync(CourseVM.IdProgram);
                var Speciality = await this.specialityService.GetSpecialityByIdAsync(CourseProgram.IdSpeciality);
                var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                courseProtocol1.SpecialityCodeAndName = Speciality.CodeAndName;
                courseProtocol1.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                courseProtocol1.ProfessionCodeAndName = Profession.CodeAndName;
                courseProtocol1.EducationForm = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdFormEducation)).Name;
                courseProtocol1.VQSAcquisitionCourseName =  CourseVM.CourseName;
                courseProtocol1.Location = (await this.locationService.GetLocationByIdAsync(IdLocationCorrespondence));
                courseProtocol1.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol1.Location.idMunicipality));
                courseProtocol1.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol1.Municipality.idDistrict));
                courseProtocol1.DocumentNumber = this.DocumentNumber;
                courseProtocol1.DateTimeNow = this.DateTimeNow;
                courseProtocol1.Region = this.candidateProviderVM.IdRegionCorrespondence != null ? (await this.regionService.GetRegionByIdAsync((int)this.candidateProviderVM.IdRegionCorrespondence)).RegionName : "";
                courseProtocol1.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdTrainingCourseType)).KeyValueIntCode == "ProfessionalQualification" ? "SPK" : "PartProfession";
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
            if (courseProtocol1.CommisisonMembers.Count >= 2 && courseProtocol1.IdChairman != 0 && courseProtocol1.Clients.Count > 0)
            {
                courseProtocol1.SelectedChairman = this.courseProtocol1.ChairmenCommisisonMembers.First(x => x.IdCourseCommissionMember == this.courseProtocol1.IdChairman).WholeName;
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
                "SelectedChairman",
                "CPODirector"
                };

                string[] fieldValues = new string[]
                {
                courseProtocol1.ProviderName,
                courseProtocol1.Region,
                courseProtocol1.Location.LocationName,
                courseProtocol1.Municipality.MunicipalityName,
                this.DocumentNumber,
                courseProtocol1.District.DistrictName,
                courseProtocol1.ProfessionCodeAndName,
                courseProtocol1.SpecialityCodeAndName,
                courseProtocol1.VQS,
                courseProtocol1.VQSAcquisitionCourseName,
                courseProtocol1.EducationForm,
                this.DateTimeNow,
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
                for (int i = 0; i < this.courseProtocol1.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol1.IdChairman).ToList().Count; i++)
                {
                    var member = this.courseProtocol1.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol1.IdChairman).ToList()[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                bookNav.MoveToBookmark("MembersList2", true, false);
                temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol1.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol1.IdChairman).ToList().Count; i++)
                {
                    var member = this.courseProtocol1.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol1.IdChairman).ToList()[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.................." + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име)" + "\t подпис и печат" + "\n").ApplyCharacterFormat(underLineCharactersFormat);
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
                                table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol1.Clients[x - 2].FullName);
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
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия (1 председател и минимум 2-ма члена) и поне един курсист!");
            }
        }
        #endregion

        #region 3-80 Practice
        private async Task GenerateCourseProtocol2()
        {
            try
            {
                courseProtocol2 = new CourseProtocol2();
                this.editContext = new EditContext(courseProtocol2);
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (CourseVM.IdCandidateProvider == null ? 0 : CourseVM.IdCandidateProvider).Value });
                int IdLocationCorrespondence = 0;
                IdLocationCorrespondence = (int)this.candidateProviderVM.IdLocationCorrespondence;
                courseProtocol2.ProviderName = $"{candidateProviderVM.ProviderName} към {candidateProviderVM.ProviderOwner}";
                courseProtocol2.DateOfExam = this.CourseVM != null ? this.CourseVM.ExamPracticeDate != null ? this.CourseVM.ExamPracticeDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "" : "";
                courseProtocol2.CourseName = CourseVM.CourseName;
                courseProtocol2.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol2.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol2.CourseCommissionMemberSource = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList();
                courseProtocol2.ChairmenCommisisonMembers = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList().Where(x => x.IsChairman == true).ToList();
                if (courseProtocol2.ChairmenCommisisonMembers.Count > 0)
                {
                    courseProtocol2.IdChairman = courseProtocol2.ChairmenCommisisonMembers.First().IdCourseCommissionMember;
                    if (courseProtocol2.IdChairman != 0)
                    {
                        this.courseProtocol2.CommisisonMembers = this.courseProtocol2.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != this.courseProtocol2.IdChairman).ToList();
                    }
                }
                courseProtocol2.Clients = this.ClientsList;
                var CourseProgram = await this.trainingService.GetTrainingProgramByIdAsync(CourseVM.IdProgram);
                var Speciality = await this.specialityService.GetSpecialityByIdAsync(CourseProgram.IdSpeciality);
                var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                courseProtocol2.SpecialityCodeAndName = Speciality.CodeAndName;
                courseProtocol2.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                courseProtocol2.ProfessionCodeAndName = Profession.CodeAndName;
                courseProtocol2.EducationForm = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdFormEducation)).Name;
                courseProtocol2.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdTrainingCourseType)).KeyValueIntCode == "ProfessionalQualification" ? "SPK" : "PartProfession";
                courseProtocol2.Location = (await this.locationService.GetLocationByIdAsync(IdLocationCorrespondence));
                courseProtocol2.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol2.Location.idMunicipality));
                courseProtocol2.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol2.Municipality.idDistrict));
                courseProtocol2.DocumentNumber = this.DocumentNumber;
                courseProtocol2.DateTimeNow = this.DateTimeNow;
                courseProtocol2.StudentPointsPair = this.courseProtocolGrades;
                courseProtocol2.Region = this.candidateProviderVM.IdRegionCorrespondence != null ? (await this.regionService.GetRegionByIdAsync((int)this.candidateProviderVM.IdRegionCorrespondence)).RegionName : "";
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
            if (courseProtocol2.CommisisonMembers.Count >= 2 && courseProtocol2.IdChairman != 0 && courseProtocol2.Clients.Count > 0)
            {
                courseProtocol2.SelectedChairman = this.courseProtocol2.ChairmenCommisisonMembers.First(x => x.IdCourseCommissionMember == this.courseProtocol2.IdChairman).WholeName;
                string FileTemplateFilePath = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", (this.courseProtocol2.TypeOfProtocol == "SPK" ? "CPOTrainingCoursesProtocol_3-80" : "CPOTrainingCoursesProtocol_3-80_PartProfession"))).Result.IdKeyValue).TemplatePath;
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + FileTemplateFilePath;
                FileStream template = new FileStream($@"{resources_Folder}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(template, FormatType.Docx);
                //foreach (var student in courseProtocol2.Clients)
                //{
                //    var tempClient = await this.trainingService.GetClientCourseDocumentByIdClientCourseAsync(student.IdClientCourse) == null ? await this.trainingService.GetClientCourseDocumentByIdClientCourseAsync(student.IdClientCourse) : new ClientCourseDocumentVM();
                //    courseProtocol2.StudentPointsPair.Add(student, tempClient);
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
                for (int i = 0; i < this.courseProtocol2.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol2.IdChairman).ToList().Count; i++)
                {
                    var member = this.courseProtocol2.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol2.IdChairman).ToList()[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                bookNav.MoveToBookmark("MembersList2", true, false);
                temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol2.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol2.IdChairman).ToList().Count; i++)
                {
                    var member = this.courseProtocol2.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol2.IdChairman).ToList()[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.................." + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име)" + "\t подпис" + "\n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersSignParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                var CourseProgram = await this.trainingService.GetTrainingProgramByIdAsync(CourseVM.IdProgram);
                var Speciality = await this.specialityService.GetSpecialityByIdAsync(CourseProgram.IdSpeciality);
                var vqs = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS));
                courseProtocol2.MarkMultiplier = vqs.DefaultValue3.Remove(1, 1).ToString().Insert(1, ",").ToString();


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
                        for (int j = 1; j < courseProtocol2.StudentPointsPair.Count(); j++)
                        {
                            table.AddRow(isCopyFormat: true);
                        }
                        for (int x = 1; x < table.Rows.Count; x++)
                        {
                            table.Rows[x].Cells[0].AddParagraph().AppendText((x).ToString());
                            table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol2.Clients.FirstOrDefault(y => y.IdClientCourse == courseProtocol2.StudentPointsPair[x - 1].IdClientCourse).FullName);
                            //var grade = courseProtocol2.StudentPointsPair[x - 1].Grade.ToString().Replace(".",",").ToString();
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
                "SelectedChairman",
                "CPODirector"
                };

                string[] fieldValues = new string[]
                {
                courseProtocol2.ProviderName,
                courseProtocol2.Region,
                courseProtocol2.Location.LocationName,
                this.DocumentNumber,
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
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия (1 председател и минимум 2-ма члена) и поне един курсист!");
            }
        }
        #endregion

        #region 3-80 Theory
        private async Task GenerateCourseProtocol3()
        {
            try
            {
                courseProtocol3 = new CourseProtocol2();
                this.editContext = new EditContext(courseProtocol3);
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (CourseVM.IdCandidateProvider == null ? 0 : CourseVM.IdCandidateProvider).Value });
                int IdLocationCorrespondence = 0;
                IdLocationCorrespondence = (int)this.candidateProviderVM.IdLocationCorrespondence;
                courseProtocol3.ProviderName = $"{candidateProviderVM.ProviderName} към {candidateProviderVM.ProviderOwner}";
                courseProtocol3.DateOfExam = this.CourseVM != null ? this.CourseVM.ExamTheoryDate != null ? this.CourseVM.ExamTheoryDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "" : "";
                courseProtocol3.CourseName = CourseVM.CourseName;
                courseProtocol3.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol3.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol3.CourseCommissionMemberSource = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList();
                courseProtocol3.ChairmenCommisisonMembers = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList().Where(x => x.IsChairman == true).ToList();
                if (courseProtocol3.ChairmenCommisisonMembers.Count > 0)
                {
                    courseProtocol3.IdChairman = courseProtocol3.ChairmenCommisisonMembers.First().IdCourseCommissionMember;
                    if (courseProtocol3.IdChairman != 0)
                    {
                        this.courseProtocol3.CommisisonMembers = this.courseProtocol3.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != this.courseProtocol3.IdChairman).ToList();
                    }
                }
                courseProtocol3.Clients = this.ClientsList;
                var CourseProgram = await this.trainingService.GetTrainingProgramByIdAsync(CourseVM.IdProgram);
                var Speciality = await this.specialityService.GetSpecialityByIdAsync(CourseProgram.IdSpeciality);
                var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                courseProtocol3.SpecialityCodeAndName = Speciality.CodeAndName;
                courseProtocol3.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                courseProtocol3.ProfessionCodeAndName = Profession.CodeAndName;
                courseProtocol3.EducationForm = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdFormEducation)).Name;
                courseProtocol3.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdTrainingCourseType)).KeyValueIntCode == "ProfessionalQualification" ? "SPK" : "PartProfession";
                courseProtocol3.Location = (await this.locationService.GetLocationByIdAsync(IdLocationCorrespondence));
                courseProtocol3.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol3.Location.idMunicipality));
                courseProtocol3.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol3.Municipality.idDistrict));
                courseProtocol3.DocumentNumber = this.DocumentNumber;
                courseProtocol3.DateTimeNow = this.DateTimeNow;
                courseProtocol3.StudentPointsPair = this.courseProtocolGrades;
                courseProtocol3.Region = this.candidateProviderVM.IdRegionCorrespondence != null ? (await this.regionService.GetRegionByIdAsync((int)this.candidateProviderVM.IdRegionCorrespondence)).RegionName : "";
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
            if (courseProtocol3.CommisisonMembers.Count >= 2 && courseProtocol3.IdChairman != 0 && courseProtocol3.Clients.Count > 0)
            {
                courseProtocol3.SelectedChairman = this.courseProtocol3.ChairmenCommisisonMembers.First(x => x.IdCourseCommissionMember == this.courseProtocol3.IdChairman).WholeName;
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
                for (int i = 0; i < this.courseProtocol3.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol3.IdChairman).ToList().Count; i++)
                {
                    var member = this.courseProtocol3.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol3.IdChairman).ToList()[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                bookNav.MoveToBookmark("MembersList2", true, false);
                temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol3.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol3.IdChairman).ToList().Count; i++)
                {
                    var member = this.courseProtocol3.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol3.IdChairman).ToList()[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.................." + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име)" + "\t подпис" + "\n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersSignParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                Syncfusion.DocIO.DLS.WTableRow RowModel = null;
                var CourseProgram = await this.trainingService.GetTrainingProgramByIdAsync(CourseVM.IdProgram);
                var Speciality = await this.specialityService.GetSpecialityByIdAsync(CourseProgram.IdSpeciality);
                var vqs = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS));
                courseProtocol3.MarkMultiplier = vqs.DefaultValue3.Remove(1, 1).ToString().Insert(1, ",").ToString();

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
                                table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol3.Clients[x - 1].FullName);
                            }
                        }
                        else
                        {
                            for (int j = 1; j < courseProtocol3.StudentPointsPair.Count(); j++)
                            {
                                table.AddRow(isCopyFormat: true);
                            }
                            for (int x = 1; x < table.Rows.Count; x++)
                            {
                                table.Rows[x].Cells[0].AddParagraph().AppendText((x).ToString());
                                table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol3.Clients.FirstOrDefault(y => y.IdClientCourse == courseProtocol3.StudentPointsPair[x - 1].IdClientCourse).FullName);
                                //var grade = courseProtocol3.StudentPointsPair[x - 1].Grade.ToString().Replace(".", ",").ToString();
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
                this.DocumentNumber,
                courseProtocol3.DateOfExam,
                courseProtocol3.ProfessionCodeAndName,
                courseProtocol3.SpecialityCodeAndName,
                courseProtocol3.VQS,
                courseProtocol3.CourseName,
                courseProtocol3.Subject,
                courseProtocol3.EducationForm,
                this.DateTimeNow,
                courseProtocol3.MarkMultiplier,
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
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия (1 председател и минимум 2-ма члена) и поне един курсист!");
            }
        }
        #endregion

        #region 3-81B
        private async Task GenerateCourseProtocol4()
        {
            try
            {
                this.editContext = new EditContext(courseProtocol4);
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (CourseVM.IdCandidateProvider == null ? 0 : CourseVM.IdCandidateProvider).Value });
                int IdLocationCorrespondence = 0;
                IdLocationCorrespondence = (int)this.candidateProviderVM.IdLocationCorrespondence;
                courseProtocol4.ProviderName = $"{candidateProviderVM.ProviderName} към {candidateProviderVM.ProviderOwner}";
                courseProtocol4.CourseName = CourseVM.CourseName;
                courseProtocol4.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol4.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol4.CourseCommissionMemberSource = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList();
                courseProtocol4.ChairmenCommisisonMembers = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList().Where(x => x.IsChairman == true).ToList();
                courseProtocol4.Clients = this.ClientsList;
                if (courseProtocol4.IdChairman != 0)
                {
                    this.courseProtocol4.CommisisonMembers = this.courseProtocol4.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != this.courseProtocol4.IdChairman).ToList();
                }
                var CourseProgram = await this.trainingService.GetTrainingProgramByIdAsync(CourseVM.IdProgram);
                var Speciality = await this.specialityService.GetSpecialityByIdAsync(CourseProgram.IdSpeciality);
                var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                courseProtocol4.SpecialityCodeAndName = Speciality.CodeAndName;
                courseProtocol4.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                courseProtocol4.ProfessionCodeAndName = Profession.CodeAndName;
                courseProtocol4.EducationForm = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdFormEducation)).Name;
                courseProtocol4.Location = (await this.locationService.GetLocationByIdAsync(IdLocationCorrespondence));
                courseProtocol4.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol4.Location.idMunicipality));
                courseProtocol4.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol4.Municipality.idDistrict));
                courseProtocol4.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdTrainingCourseType)).KeyValueIntCode == "ProfessionalQualification" ? "SPK" : "PartProfession";
                courseProtocol4.DocumentNumber = this.DocumentNumber;
                courseProtocol4.DateTimeNow = this.DateTimeNow;
                courseProtocol4.StudentPointsPair = this.courseProtocolGrades;
                courseProtocol4.Region = this.candidateProviderVM.IdRegionCorrespondence != null ? (await this.regionService.GetRegionByIdAsync((int)this.candidateProviderVM.IdRegionCorrespondence)).RegionName : "";
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
            if (courseProtocol4.CommisisonMembers.Count >= 2 && courseProtocol4.IdChairman != 0 && courseProtocol4.Clients.Count > 0)
            {
                courseProtocol4.SelectedChairman = this.courseProtocol4.ChairmenCommisisonMembers.First(x => x.IdCourseCommissionMember == this.courseProtocol4.IdChairman).WholeName;
                string FileTemplateFilePath = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", (this.courseProtocol4.TypeOfProtocol == "SPK" ? "CPOTrainingCoursesProtocol_3-81B" : "CPOTrainingCoursesProtocol_3-81B_PartProfession"))).Result.IdKeyValue).TemplatePath;
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments" + FileTemplateFilePath;
                FileStream template = new FileStream($@"{resources_Folder}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(template, FormatType.Docx);
                template.Close();

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
                this.DocumentNumber,
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
                for (int i = 0; i < this.courseProtocol4.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol4.IdChairman).ToList().Count; i++)
                {
                    var member = this.courseProtocol4.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol4.IdChairman).ToList()[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име) \n").ApplyCharacterFormat(underLineCharactersFormat);
                    membersParagraph.ApplyStyle("MembersParagraph");

                    bookNav.InsertParagraph(membersParagraph);
                }

                bookNav.MoveToBookmark("MembersList2", true, false);
                temp = new WParagraph(document);
                temp.AppendText("\n");
                for (int i = 0; i < this.courseProtocol4.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol4.IdChairman).ToList().Count; i++)
                {
                    var member = this.courseProtocol4.CommisisonMembers.Where(x => x.IdCourseCommissionMember != this.courseProtocol4.IdChairman).ToList()[i];
                    IWParagraph membersParagraph = new WParagraph(document);

                    membersParagraph.AppendText((i + 1).ToString() + ". " + member.WholeName + "\t.................." + "\n").ApplyCharacterFormat(membersCharacterFormat);
                    membersParagraph.AppendText("       (собствено, бащино и фамилно име)" + "\t подпис и печат" + "\n").ApplyCharacterFormat(underLineCharactersFormat);
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
                        for (int j = 1; j < courseProtocol4.StudentPointsPair.Count; j++)
                        {
                            table.AddRow(isCopyFormat: true);
                        }
                        for (int x = 2; x < table.Rows.Count; x++)
                        {
                            table.Rows[x].Cells[0].AddParagraph().AppendText((x - 1).ToString() + ".");
                            table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol4.Clients.FirstOrDefault(y => y.IdClientCourse == courseProtocol4.StudentPointsPair[x - 2].IdClientCourse).FullName);
                            var grade = courseProtocol4.StudentPointsPair[x - 2].Grade.ToString().Replace(".", ",").ToString();
                            switch (courseProtocol4.StudentPointsPair[x - 2].Grade)
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

                            table.Rows[x].Cells[6].AddParagraph().AppendText(string.Format("{0:0.00}", courseProtocol4.StudentPointsPair[x - 2].Grade));
                        }
                    }
                }

                MemoryStream stream = new MemoryStream();
                document.Save(stream, FormatType.Docx);
                document.Close();
                await FileUtils.SaveAs(JS, "3-81B.docx", stream.ToArray());
                this.isVisible = false;
                this.StateHasChanged();
            }
            else
            {
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия (1 председател и минимум 2-ма члена) и поне един курсист!");
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
                this.candidateProviderVM = await this.providerService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = (CourseVM.IdCandidateProvider == null ? 0 : CourseVM.IdCandidateProvider).Value });
                int IdLocationCorrespondence = 0;
                IdLocationCorrespondence = (int)this.candidateProviderVM.IdLocationCorrespondence;
                courseProtocol5.ProviderName = $"{candidateProviderVM.ProviderName} към {candidateProviderVM.ProviderOwner}";
                courseProtocol5.CourseName = CourseVM.CourseName;
                courseProtocol5.ProviderOwner = candidateProviderVM.ProviderOwner;
                courseProtocol5.ProviderManager = candidateProviderVM.DirectorFullName;
                courseProtocol5.CourseCommissionMemberSource = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList();
                courseProtocol5.ChairmenCommisisonMembers = (await this.trainingService.GetAllCourseCommissionMembersByIdCourseAsync(CourseVM.IdCourse)).ToList().Where(x => x.IsChairman == true).ToList();
                if (courseProtocol5.ChairmenCommisisonMembers.Count > 0)
                {
                    courseProtocol5.IdChairman = courseProtocol5.ChairmenCommisisonMembers.First().IdCourseCommissionMember;
                    if (courseProtocol5.IdChairman != 0)
                    {
                        this.courseProtocol5.CommisisonMembers = this.courseProtocol5.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != this.courseProtocol5.IdChairman).ToList();
                    }
                }
                courseProtocol5.Clients = this.ClientsList; //(await this.trainingService.GetCourseClientsByIdCourseAsync(CourseVM.IdCourse)).ToList();//(await this.trainingService.GetCourseClientsByIdCourseAsync(CourseVM.IdCourse)).ToList();
                var CourseProgram = await this.trainingService.GetTrainingProgramByIdAsync(CourseVM.IdProgram);
                var Speciality = await this.specialityService.GetSpecialityByIdAsync(CourseProgram.IdSpeciality);
                var Profession = await this.professionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = Speciality.IdProfession });
                courseProtocol5.SpecialityCodeAndName = Speciality.CodeAndName;
                courseProtocol5.VQS = (await this.dataSourceService.GetKeyValueByIdAsync(Speciality.IdVQS)).Name;
                courseProtocol5.ProfessionCodeAndName = Profession.CodeAndName;
                courseProtocol5.EducationForm = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdFormEducation)).Name;
                courseProtocol5.TypeOfProtocol = (await this.dataSourceService.GetKeyValueByIdAsync(CourseVM.IdTrainingCourseType)).KeyValueIntCode == "ProfessionalQualification" ? "SPK" : "PartProfession";
                courseProtocol5.VQSAcquisitionCourseName =  CourseVM.CourseName;
                courseProtocol5.Location = (await this.locationService.GetLocationByIdAsync(IdLocationCorrespondence));
                courseProtocol5.Municipality = (await this.municipalityService.GetMunicipalityByIdAsync((int)courseProtocol5.Location.idMunicipality));
                courseProtocol5.District = (await this.districtService.GetDistrictByIdAsync((int)this.courseProtocol5.Municipality.idDistrict));
                courseProtocol5.DocumentNumber = this.DocumentNumber;
                courseProtocol5.DateTimeNow = this.DateTimeNow;
                courseProtocol5.Region = this.candidateProviderVM.IdRegionCorrespondence != null ? (await this.regionService.GetRegionByIdAsync((int)this.candidateProviderVM.IdRegionCorrespondence)).RegionName : "";
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
            if (courseProtocol5.CommisisonMembers.Count >= 2 && courseProtocol5.IdChairman != 0 && courseProtocol5.Clients.Count > 0)
            {
                courseProtocol5.SelectedChairman = this.courseProtocol5.ChairmenCommisisonMembers.First(x => x.IdCourseCommissionMember == this.courseProtocol5.IdChairman).WholeName;
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
                this.DocumentNumber,
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
                            table.Rows[x].Cells[1].AddParagraph().AppendText(courseProtocol5.Clients[x - 3].FullName);
                        }
                    }
                }

                RowModel = null;
                courseProtocol5.CommisisonMembers.AddRange(courseProtocol5.ChairmenCommisisonMembers);
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
                this.ShowErrorAsync("За да създадете протокол, следва да имате въведена комисия (1 председател и минимум 2-ма члена) и поне един курсист!");
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
        private async Task RemoveCommissionMember(CourseCommissionMemberVM member, int id)
        {
            switch (this.ProtocolNumber)
            {
                case "3-79":
                    if (member.IdCourseCommissionMember != this.courseProtocol1.IdChairman)
                    {
                        this.courseProtocol1.AppcentCommisionMembers.Add(member);
                    }
                    this.courseProtocol1.CommisisonMembers.Remove(member);
                    break;
                case "3-80p":
                    if (member.IdCourseCommissionMember != this.courseProtocol2.IdChairman)
                    {
                        this.courseProtocol2.AppcentCommisionMembers.Add(member);

                    }
                    this.courseProtocol2.CommisisonMembers.Remove(member);
                    break;
                case "3-80t":
                    if (member.IdCourseCommissionMember != this.courseProtocol3.IdChairman)
                    {
                        this.courseProtocol3.AppcentCommisionMembers.Add(member);
                    }

                    this.courseProtocol3.CommisisonMembers.Remove(member);
                    break;
                case "3-81B":
                    if (member.IdCourseCommissionMember != this.courseProtocol4.IdChairman)
                    {
                        this.courseProtocol4.AppcentCommisionMembers.Add(member);
                    }
                    this.courseProtocol4.CommisisonMembers.Remove(member);
                    break;
                case "3-82":
                    if (member.IdCourseCommissionMember != this.courseProtocol5.IdChairman)
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
        private async Task SelectChairman(SelectEventArgs<CourseCommissionMemberVM> args)
        {
            switch (this.ProtocolNumber)
            {
                case "3-79":
                    this.courseProtocol1.CommisisonMembers = this.courseProtocol1.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != args.ItemData.IdCourseCommissionMember).ToList();                    
                    break;
                case "3-80p":
                    this.courseProtocol2.CommisisonMembers = this.courseProtocol2.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != args.ItemData.IdCourseCommissionMember).ToList();
                    break;
                case "3-80t":
                    this.courseProtocol3.CommisisonMembers = this.courseProtocol3.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != args.ItemData.IdCourseCommissionMember).ToList();
                    break;
                case "3-81B":
                    this.courseProtocol4.CommisisonMembers = this.courseProtocol4.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != args.ItemData.IdCourseCommissionMember).ToList();
                    break;
                case "3-82":
                    this.courseProtocol5.CommisisonMembers = this.courseProtocol5.CourseCommissionMemberSource.Where(x => x.IdCourseCommissionMember != args.ItemData.IdCourseCommissionMember).ToList();
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

        public List<CourseCommissionMemberVM> CommisisonMembers = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> ChairmenCommisisonMembers = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> CourseCommissionMemberSource = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> AppcentCommisionMembers = new List<CourseCommissionMemberVM>();

        public List<ClientCourseVM> Clients = new List<ClientCourseVM>();
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

        public List<CourseCommissionMemberVM> CommisisonMembers = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> ChairmenCommisisonMembers = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> CourseCommissionMemberSource = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> AppcentCommisionMembers = new List<CourseCommissionMemberVM>();

        public List<ClientCourseVM> Clients = new List<ClientCourseVM>();

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

        public List<CourseCommissionMemberVM> CommisisonMembers = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> ChairmenCommisisonMembers = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> CourseCommissionMemberSource = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> AppcentCommisionMembers = new List<CourseCommissionMemberVM>();

        public List<CourseProtocolGradeVM> StudentPointsPair = new List<CourseProtocolGradeVM>();

        public List<ClientCourseVM> Clients = new List<ClientCourseVM>();
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

        public List<CourseCommissionMemberVM> CommisisonMembers = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> ChairmenCommisisonMembers = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> CourseCommissionMemberSource = new List<CourseCommissionMemberVM>();

        public List<CourseCommissionMemberVM> AppcentCommisionMembers = new List<CourseCommissionMemberVM>();

        public List<ClientCourseVM> Clients = new List<ClientCourseVM>();

        public List<ClientCourseVM> AppcentClients = new List<ClientCourseVM>();
    }
}
