using Data.Models.Data.Candidate;
using Data.Models.Data.SPPOO;
using Data.Models.Data.Training;
using Data.Models.DB;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Services.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Hosting;
using Syncfusion.ExcelExport;
using Training = Data.Models.Data.Training;

namespace ISNAPOO.WebSystem.Seeder
{
    public class TrainingSeeder
    {
        public static async Task TrainingDataSeeder(WebApplication applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var dataSourceService = serviceScope.ServiceProvider.GetService<IDataSourceService>();
                var locationService = serviceScope.ServiceProvider.GetService<ILocationService>();

                


                dataSourceService.ReloadKeyValue();
                dataSourceService.ReloadSettings();
                dataSourceService.ReloadKeyType();

                CandidateProvider candidateProviderTehnologika = context.CandidateProviders.First(cp => cp.PoviderBulstat == "201593301");
                CandidateProvider candidateProviderSMConsulta = context.CandidateProviders.First(cp => cp.PoviderBulstat == "201593301");
                Speciality specialityVegetableGrowing = context.Specialities.First(s => s.Code == "3451203");
                Profession profession = context.Professions.First(s => s.IdProfession == specialityVegetableGrowing.IdProfession);
                ProfessionalDirection professionalDirection = context.ProfessionalDirections.First(s => s.IdProfessionalDirection == profession.IdProfessionalDirection);
                CandidateProviderPremises candidateProviderPremises = context.CandidateProviderPremises.FirstOrDefault(c=>c.IdCandidate_Provider == candidateProviderTehnologika.IdCandidate_Provider);
                FrameworkProgram frameworkProgramA10 = context.FrameworkPrograms.First(fr => fr.Name == "А10");


                if (true)
                {

                    List<Course> courses = new List<Course>();
                    List<Client> clients = new List<Client>();
                    List<ClientCourse> ClientCourses = new List<ClientCourse>();
                    List<ClientCourseDocument> ClientCourseDocuments = new List<ClientCourseDocument>();



                    courses.Add(new Course()
                    {
                        SubscribeDate = DateTime.Now.AddDays(10),
                        CourseName = "Традиции и развитие в българското зеленчукопроизводство " + DateTime.Now.AddDays(15).ToString("dd.MM.yyyy"),
                        IdStatus = dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusUpcoming").Result.IdKeyValue,
                        IdMeasureType = dataSourceService.GetKeyValueByIntCodeAsync("MeasureType", "MeasureType1").Result.IdKeyValue,
                        IdAssignType = dataSourceService.GetKeyValueByIntCodeAsync("AssignType", "HumanResourcesDevelopmentProjects").Result.IdKeyValue,
                        IdFormEducation = dataSourceService.GetKeyValueByIntCodeAsync("FormEducation", "TypeEducationDaily").Result.IdKeyValue,
                        IdLocation = locationService.GetLocationByLocationIdAsync(1136).Result.idLocation, //1136 Балчик
                        MandatoryHours = 900,
                        SelectableHours = 60,
                        DurationHours = 960,
                        Cost = 750,
                        StartDate = DateTime.Now.AddDays(20),
                        EndDate = DateTime.Now.AddDays(40),
                        ExamTheoryDate = DateTime.Now.AddDays(45),
                        ExamPracticeDate = DateTime.Now.AddDays(47),
                        CandidateProviderPremises = candidateProviderPremises,
                    });

                    courses.Add(new Course()
                    {
                        SubscribeDate = DateTime.Now.AddDays(15),
                        CourseName = "Традиции и развитие в българското зеленчукопроизводство " + DateTime.Now.AddDays(15).ToString("dd.MM.yyyy"),
                        IdStatus = dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusUpcoming").Result.IdKeyValue,
                        IdMeasureType = dataSourceService.GetKeyValueByIntCodeAsync("MeasureType", "MeasureType2").Result.IdKeyValue,
                        IdAssignType = dataSourceService.GetKeyValueByIntCodeAsync("AssignType", "HumanResourcesDevelopmentProjects").Result.IdKeyValue,
                        IdFormEducation = dataSourceService.GetKeyValueByIntCodeAsync("FormEducation", "TypeEducationDaily").Result.IdKeyValue,
                        IdLocation = locationService.GetLocationByLocationIdAsync(1136).Result.idLocation, //1136 Балчик
                        MandatoryHours = 900,
                        SelectableHours = 60,
                        DurationHours = 960,
                        Cost = 750,
                        StartDate = DateTime.Now.AddDays(25),
                        EndDate = DateTime.Now.AddDays(45),
                        ExamTheoryDate = DateTime.Now.AddDays(50),
                        ExamPracticeDate = DateTime.Now.AddDays(53),
                        CandidateProviderPremises = candidateProviderPremises,
                    });

                   

                    clients.Add(new Client() 
                    { 
                        FirstName = "Иван",
                        SecondName = "Иванов",
                        FamilyName = "Иванов",
                        CandidateProvider = candidateProviderTehnologika,
                        IdSex = dataSourceService.GetKeyValueByIntCodeAsync("Sex", "Man").Result.IdKeyValue,
                        IdIndentType = dataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN").Result.IdKeyValue,
                        Indent = "211111111",
                        BirthDate = DateTime.Now.AddYears(-20),
                        IdNationality = dataSourceService.GetKeyValueByIntCodeAsync("Nationality", "BG").Result.IdKeyValue,                       
                        ProfessionalDirection = professionalDirection,
                        IdEducation = dataSourceService.GetKeyValueByIntCodeAsync("Education", "bachelor").Result.IdKeyValue,
                    });

                    clients.Add(new Client()
                    {
                        FirstName = "Петър",
                        SecondName = "Петров",
                        FamilyName = "Петров",
                        CandidateProvider = candidateProviderTehnologika,
                        IdSex = dataSourceService.GetKeyValueByIntCodeAsync("Sex", "Man").Result.IdKeyValue,
                        IdIndentType = dataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN").Result.IdKeyValue,
                        Indent = "211111112",
                        BirthDate = DateTime.Now.AddYears(-25),
                        IdNationality = dataSourceService.GetKeyValueByIntCodeAsync("Nationality", "BG").Result.IdKeyValue,
                        ProfessionalDirection = professionalDirection,
                        IdEducation = dataSourceService.GetKeyValueByIntCodeAsync("Education", "bachelor").Result.IdKeyValue,
                    });

                    clients.Add(new Client()
                    {
                        FirstName = "Димитър",
                        SecondName = "Димитров",
                        FamilyName = "Димитров",
                        CandidateProvider = candidateProviderTehnologika,
                        IdSex = dataSourceService.GetKeyValueByIntCodeAsync("Sex", "Man").Result.IdKeyValue,
                        IdIndentType = dataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN").Result.IdKeyValue,
                        Indent = "211111113",
                        BirthDate = DateTime.Now.AddYears(-30),
                        IdNationality = dataSourceService.GetKeyValueByIntCodeAsync("Nationality", "BG").Result.IdKeyValue,
                        ProfessionalDirection = professionalDirection,
                        IdEducation = dataSourceService.GetKeyValueByIntCodeAsync("Education", "bachelor").Result.IdKeyValue,
                    });

                    ClientCourses.Add(
                        new ClientCourse 
                        {
                            Course = courses[0],
                            Client = clients[0],
                                
                            FirstName = clients[0].FirstName,
                            SecondName = clients[0].SecondName,
                            FamilyName = clients[0].FamilyName,
                            IdSex = clients[0].IdSex,
                            IdIndentType = clients[0].IdIndentType, 
                            IdNationality = clients[0].IdNationality,   
                            IdEducation = clients[0].IdEducation,
                            BirthDate = clients[0].BirthDate,
                            ProfessionalDirection = clients[0].ProfessionalDirection,
                            Speciality = specialityVegetableGrowing,
                            IdAssignType = courses[0].IdAssignType,
                            IdFinishedType = dataSourceService.GetKeyValueByIntCodeAsync("Education", "Finished").Result.IdKeyValue,
                            FinishedDate = DateTime.Now.AddDays(40),
                            IdQualificationLevel = dataSourceService.GetKeyValueByIntCodeAsync("Qualification", "QualificationAreaEducation").Result.IdKeyValue,

                        });

                    ClientCourses.Add(
                        new ClientCourse
                        {
                            Course = courses[0],
                            Client = clients[1],

                            FirstName = clients[1].FirstName,
                            SecondName = clients[1].SecondName,
                            FamilyName = clients[1].FamilyName,
                            IdSex = clients[1].IdSex,
                            IdIndentType = clients[1].IdIndentType,
                            IdNationality = clients[1].IdNationality,
                            IdEducation = clients[1].IdEducation,
                            BirthDate = clients[1].BirthDate,
                            ProfessionalDirection = clients[1].ProfessionalDirection,
                            Speciality = specialityVegetableGrowing,
                            IdAssignType = courses[0].IdAssignType,
                            IdFinishedType = dataSourceService.GetKeyValueByIntCodeAsync("Education", "Finished").Result.IdKeyValue,
                            FinishedDate = DateTime.Now.AddDays(40),
                            IdQualificationLevel = dataSourceService.GetKeyValueByIntCodeAsync("Qualification", "QualificationAreaEducation").Result.IdKeyValue,
                        });


                    ClientCourseDocuments.Add(new ClientCourseDocument() 
                    { 
                        ClientCourse = ClientCourses[0],
                        IdDocumentType = dataSourceService.GetKeyValueByIntCodeAsync("Certificate", "CertificateSPK").Result.IdKeyValue,
                        FinishedYear = 2022,
                        DocumentPrnNo = "033391",
                        DocumentRegNo = "0033-0552",
                        DocumentDate = DateTime.Now,
                        DocumentProtocol = "00011",
                        TheoryResult = 6,
                        PracticeResult = 6,
                        QualificationName = "Работа с електрокари и мотокари в предприятията",
                        QualificationLevel = "Универсални високоповдигачи",
                        DocumentSerNo = "11",
                        IdDocumentStatus = dataSourceService.GetKeyValueByIntCodeAsync("ClientCourseDocumentStatus", "Submitted").Result.IdKeyValue,


                    });

                    ClientCourseDocuments.Add(new ClientCourseDocument()
                    {
                        ClientCourse = ClientCourses[1],
                        IdDocumentType = dataSourceService.GetKeyValueByIntCodeAsync("Certificate", "CertificateProfessional").Result.IdKeyValue,
                        FinishedYear = 2022,
                        DocumentPrnNo = "123123",
                        DocumentRegNo = "0033-1231312",
                        DocumentDate = DateTime.Now,
                        DocumentProtocol = "12313",
                        TheoryResult = 6,
                        PracticeResult = 6,
                        QualificationName = "Работа с електрокари и мотокари в предприятията",
                        QualificationLevel = "Универсални високоповдигачи",
                        DocumentSerNo = "11",
                        IdDocumentStatus = dataSourceService.GetKeyValueByIntCodeAsync("ClientCourseDocumentStatus", "Unsubmitted").Result.IdKeyValue,
                    });

                    context.Programs.Add(new Training.Program()
                    {
                        ProgramNumber = "1",
                        ProgramName = "Традиции и развитие в българското зеленчукопроизводство",
                        ProgramNote = "Предназначена за обучение на земеделски производители и заетите лица в техните стопанства",
                        CandidateProvider = candidateProviderTehnologika,
                        Speciality = specialityVegetableGrowing,
                        FrameworkProgram = frameworkProgramA10,
                        IdCourseType = dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification").Result.IdKeyValue,
                        MandatoryHours = 900,
                        SelectableHours = 60,
                        Courses = courses

                    });

                    context.Courses.AddRange(courses);
                    context.Clients.AddRange(clients);
                    context.ClientCourses.AddRange(ClientCourses);
                    context.ClientCourseDocuments.AddRange(ClientCourseDocuments);


                    context.SaveChanges();
                }
            }
        }
    }
}
