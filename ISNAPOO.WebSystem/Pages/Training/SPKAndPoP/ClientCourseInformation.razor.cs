using System.Drawing;
using System.Runtime.ConstrainedExecution;
using Data.Models.Data.Training;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Org.BouncyCastle.Asn1.X500;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class ClientCourseInformation : BlazorBaseComponent
    {
        private SfAutoComplete<int?, LocationVM> autoCompleteLocation = new SfAutoComplete<int?, LocationVM>();

        private IEnumerable<KeyValueVM> kvIndentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvSexSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvAssingSource = new List<KeyValueVM>();
        private List<KeyValueVM> kvNationalitySource = new List<KeyValueVM>();
        public string identType = "ЕГН";
        private KeyValueVM kvEGN = new KeyValueVM();
        private KeyValueVM kvLNCh = new KeyValueVM();
        private KeyValueVM kvBGNationality = new KeyValueVM();
        private ValidationMessageStore? messageStore;
        private string content = "Проверка и зареждане на данни за курсиста от предишни курсове на обучение";
        public List<LocationVM> locationSource = new List<LocationVM>();
        private ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM finishedDataModel = new ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM();
        private ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM duplicateFinishedDataModel = new ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM();
        private IEnumerable<CourseSubjectVM> courseSubjectSource = new List<CourseSubjectVM>();
        private IEnumerable<KeyValueVM> professionalTrainingTypesSource;
        private List<TrainingCurriculumVM> addedCurriculums = new List<TrainingCurriculumVM>();
        private IEnumerable<KeyValueVM> professionalTrainingsSource;
        private double totalTheoryHours = 0;
        private double theoryHours = 0;
        private double totalPracticeHours = 0;
        private double practiceHours = 0;

        private KeyValueVM kvQualificationLevel = new KeyValueVM();
        private List<KeyValueVM> requiredDocumentTypesSource;
        private KeyValueVM kvUniversityDiploma;
        private KeyValueVM kvSecondarySchoolDiploma;
        private List<DocumentSerialNumberVM> documentSerialNumbersSource;
        private List<DocumentSeriesVM> documentSeriesSource;
        private double extendedProfessionTrainingHours;
        private double industryProfessionTrainingHours;
        private double generalProfessionTrainingHours;
        private double specificProfessionTrainingHours;
        private IEnumerable<CourseProtocolVM> protocolsSource;
        private bool isFinished;

        [Parameter]
        public ClientCourseVM ClientCourseVM { get; set; }

        [Parameter]
        public bool IsEditEnabled { get; set; } = false;

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public bool EntryFromLegalCapacityModule { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }
        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }
        [Inject]
        public Microsoft.JSInterop.IJSRuntime JS { get; set; }

        [Inject]
        public ITemplateDocumentService templateDocumentService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.ClientCourseVM);
            this.FormTitle = "Данни за курсист";

            this.kvNationalitySource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality")).OrderBy(x => x.Name).ToList();
            this.kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvQualificationLevel = await this.DataSourceService.GetKeyValueByIntCodeAsync("QualificationLevel", "WithoutQualification_Update");
            this.kvLNCh = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
            this.kvSexSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            this.requiredDocumentTypesSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType")).Where(x => x.DefaultValue1 != null && x.DefaultValue3.Contains("CPO")).ToList();

            this.kvSecondarySchoolDiploma = this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "SecondarySchoolDiploma");
            this.kvUniversityDiploma = this.requiredDocumentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "UniversityDiploma");
            this.kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.professionalTrainingTypesSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

            this.kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.kvSexSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            this.kvAssingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType");
            this.kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvLNCh = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
            this.finishedDataModel = await this.TrainingService.GetClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(this.ClientCourseVM.IdClientCourse);
            if (this.ClientCourseVM.IdClientCourse == 0)
            {
                this.ClientCourseVM.IdIndentType = this.kvEGN.IdKeyValue;
            }

            if (this.ClientCourseVM.IdIndentType.HasValue)
            {
                var ident = this.kvIndentTypeSource.FirstOrDefault(x => x.IdKeyValue == this.ClientCourseVM.IdIndentType.Value);
                if (ident is not null)
                {
                    this.identType = ident.Name;
                }
            }

            this.HandleOrderForNationalitiesSource();
            this.isFinished = this.finishedDataModel?.DocumentDate.HasValue ?? false;
            if (this.ClientCourseVM.IdCityOfBirth != null)
            {
                this.locationSource.Clear();
                LocationVM location = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.ClientCourseVM.IdCityOfBirth.Value);
                this.locationSource.Add(location);
            }

            if (!this.ClientCourseVM.CourseJoinDate.HasValue)
            {
                if (this.CourseVM.StartDate.HasValue)
                {
                    this.ClientCourseVM.CourseJoinDate = this.CourseVM.StartDate.Value;
                }
                else
                {
                    this.ClientCourseVM.CourseJoinDate = DateTime.Now;
                }
            }

            this.editContext.MarkAsUnmodified();
        }

        public override void SubmitHandler()
        {
            this.ClientCourseVM.IdSpeciality = this.CourseVM.Program.IdSpeciality;
            this.ClientCourseVM.IdProfessionalDirection = this.CourseVM.Program.Speciality.Profession.IdProfessionalDirection;

            this.editContext = new EditContext(this.ClientCourseVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.editContext.OnValidationRequested += this.ValidateEGN;
            this.editContext.OnValidationRequested += this.ValidateCountryAndCityOfBirth;
            this.editContext.OnValidationRequested += this.ValidateCourseJoinDate;
            this.editContext.OnValidationRequested += this.ValidateSecondName;
            this.editContext.OnValidationRequested += this.ValidateClientAge;
            this.messageStore = new ValidationMessageStore(this.editContext);

            this.editContext.Validate();
        }

        private void HandleOrderForNationalitiesSource()
        {
            var withoutNacionality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "Без гражданство");
            this.kvNationalitySource.Remove(withoutNacionality);
            this.kvNationalitySource.Add(withoutNacionality);
            var bgNationality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "България");
            this.kvNationalitySource.Remove(bgNationality);
            this.kvNationalitySource.Insert(0, bgNationality);
            this.kvNationalitySource.RemoveAll(x => x.Name == "");

            this.kvBGNationality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "България");
        }

        private async Task ExportPersonalFile()
        {
            this.SpinnerShow();
            try
            {
                if (this.finishedDataModel != null)
                {
                    if (this.finishedDataModel.DocumentTypeName != null)
                    {
                        if (this.finishedDataModel.DocumentTypeName.ToLower()
                                .Contains("свидетелство за професионална квалификация") || this.finishedDataModel
                                .DocumentTypeName.ToLower().Contains("удостоверение за професионално обучение"))
                        {
                            var documentType =
                                (await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType",
                                    "PersonalFile")).IdKeyValue;
                            var templateDocument =
                                (await this.templateDocumentService.GetAllTemplateDocumentsAsync(
                                    new TemplateDocumentVM()))
                                .FirstOrDefault(x => x.IdApplicationType == documentType);
                            var candidateProviderSpeciality =
                                await this.CandidateProviderService
                                    .GetCandidateProviderSpecialityByIdCandidateProviderAndByIdSpecialityAsync(
                                        this.CourseVM.CandidateProvider.IdCandidate_Provider,
                                        this.CourseVM.Program.Speciality.IdSpeciality);
                            var candidateCurriculumModification =
                                await this.CandidateProviderService
                                    .GetCandidateCurriculumModificationWhenApplicationByIdCandidateProviderSpecialityAsync(
                                        candidateProviderSpeciality.IdCandidateProviderSpeciality);

                            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
                            FileStream blueprint = new FileStream($@"{resources_Folder}{templateDocument.TemplatePath}",
                                FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            WordDocument document = new WordDocument(blueprint, FormatType.Docx);
                            LocationVM clientLocation = new LocationVM();
                            this.courseSubjectSource =
                                await this.TrainingService.GetAllCourseSubjectsByIdCourseAsync(this.CourseVM.IdCourse);
                            if (this.ClientCourseVM.IdCityOfBirth != null)
                            {
                                clientLocation =
                                    await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(
                                        this.ClientCourseVM.IdCityOfBirth.Value);
                            }

                            String subjects = "";
                            String hours = "";
                            int linesCount = 0;
                            int count = 0;
                            this.addedCurriculums =
                                (await this.TrainingService
                                    .GetTrainingCurriculumByIdCourseAsync(this.CourseVM.IdCourse))
                                .ToList();
                            this.professionalTrainingsSource =
                                await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

                            foreach (var curriculum in this.addedCurriculums)
                            {
                                count++;
                                subjects += count + ". " + curriculum.Subject + "\u000B";
                                linesCount += BaseHelper.CalculateNumberOfLines(curriculum.Subject.Trim(), 442f,
                                    new Font("Times New Roman", 11f));
                                hours += (curriculum.Practice ?? 0) + (curriculum.Theory ?? 0) + "\u000B" +
                                         string.Concat(Enumerable.Repeat("-\u000B",
                                             BaseHelper.CalculateNumberOfLines(curriculum.Subject.Trim(), 459f,
                                                 new Font("Times New Roman", 12f)) - 1));
                            }


                            CalculateCurriculumHours();


                            string[] fieldNames = new string[]
                            {
                                "InstitutionName", "Kati", "MunicipalityName", "Region", "DistrictName",
                                "PersonName", "EGN", "FID", "NationalityPerson", "BirthdayPerson", "CityPerson",
                                "MunicipalityPerson", "DistrictPerson", "AddressPerson", "YearCurriculumApproval",
                                "AdmissionDocument", "EducationType", "AdmissionDate", "Director", "RegNum2",
                                "Date2", "VocationalCertificateSeries", "CourseName", "CourseLength", "TotalHours",
                                "TheoryHours", "PracticeHours", "Subject", "Hours", "VocationalCertificateNumber",
                                "RegNum1", "Date1", "PersonName1", "EGN1", "CityPerson1", "MunicipalityPerson1",
                                "DistrictPerson1", "FID1", "NationalityPerson1", "Year1", "InstitutionName1",
                                "Kati1", "FormOfТraining1", "CourseLength1", "DocumentProtocol1", "DocumentDate1",
                                "SPK1", "Profession1", "Speciality1", "AssessedGradeName1", "AssessedGradeValue1",
                                "NKR1", "EKR1", "ChairmanOfPQC1", "Director1", "RegNum2", "Date2", "PersonName2",
                                "EGN2", "FID2", "NationalityPerson2", "CityPerson2", "MunicipalityPerson2",
                                "DistrictPerson2", "Year2", "Program2", "QualificationLevel2", "Profession2",
                                "Speciality2", "InstitutionName2", "Kati2", "FormOfТraining2", "CourseLength2",
                                "DocumentProtocol2", "DocumentDate2", "AssessedGradeName2", "AssessedGradeValue2",
                                "Subject2", "Hours2", "ChairmanOfPQC2", "Director2"
                            };

                            var protocols =
                                await this.TrainingService
                                    .GetAll381BProtocolsWhichHaveGradeAddedByIdCourseAndByIdClientCourseAsync(
                                        this.CourseVM.IdCourse, this.ClientCourseVM.IdClientCourse);
                            CourseCommissionMemberVM commissionMember = new CourseCommissionMemberVM();
                            if (protocols.Any())
                            {
                                commissionMember = this.CourseVM.CourseCommissionMembers.FirstOrDefault(
                                    commissionMember =>
                                        commissionMember.IdCourseCommissionMember ==
                                        protocols.FirstOrDefault().IdCourseCommissionMember);

                            }

                            string institutionName = "Център за професионално обучение ";

                            if (this.CourseVM.CandidateProvider != null)
                            {
                                if (string.IsNullOrEmpty(this.CourseVM.CandidateProvider.ProviderName))
                                {
                                    institutionName += "към ";
                                }
                                else
                                {
                                    if (this.CourseVM.CandidateProvider.ProviderName.StartsWith("ЦПО към") ||
                                        this.CourseVM.CandidateProvider.ProviderName.StartsWith(
                                            "Център за професионално обучение към "))
                                    {
                                        institutionName =
                                            this.CourseVM.CandidateProvider.ProviderName.Replace("ЦПО към",
                                                "Център за професионално обучение към ");
                                    }

                                    institutionName += this.CourseVM.CandidateProvider.ProviderName + " към ";
                                }

                                if (this.CourseVM.CandidateProvider.ProviderOwner != "" ||
                                    this.CourseVM.CandidateProvider.ProviderOwner != null)
                                {
                                    institutionName += this.CourseVM.CandidateProvider.ProviderOwner;
                                }
                            }

                            institutionName += string.Concat(Enumerable.Repeat("\u000B-",
                                2 - BaseHelper.CalculateNumberOfLines(institutionName.Trim(), 328f,
                                    new Font("Times New Roman", 12f, FontStyle.Bold))));
                            string kati = this.CourseVM.CandidateProvider?.Location?.kati ??
                                          "-" + "\u000B                                        -";
                            string municipalityName =
                                this.CourseVM.CandidateProvider?.Location?.Municipality?.MunicipalityName ?? "-";
                            string region = this.CourseVM.CandidateProvider?.Location?.Municipality?.Regions
                                                ?.FirstOrDefault(region =>
                                                    region.idMunicipality ==
                                                    this.CourseVM.CandidateProvider.Location.Municipality
                                                        .idMunicipality)
                                                ?.RegionName ??
                                            "-";
                            string districtName =
                                this.CourseVM.CandidateProvider?.Location?.Municipality?.District?.DistrictName ?? "-";
                            string nkr = this.CourseVM.Program.Speciality.IdNKRLevel == 0
                                ? "-"
                                : (await this.DataSourceService.GetKeyValueByIdAsync(
                                    this.CourseVM.Program.Speciality.IdNKRLevel))?.Name ?? "-";
                            string ekr = this.CourseVM.Program.Speciality.IdEKRLevel == 0
                                ? "-"
                                : (await this.DataSourceService.GetKeyValueByIdAsync(
                                    this.CourseVM.Program.Speciality.IdEKRLevel))?.Name ?? "-";
                            string director = string.IsNullOrEmpty(this.CourseVM.CandidateProvider?.DirectorFullName)
                                ? "-"
                                : this.CourseVM.CandidateProvider?.DirectorFirstName + " " +
                                  this.CourseVM.CandidateProvider?.DirectorFamilyName;
                            string personName =
                                $"{this.ClientCourseVM.FirstName} {this.ClientCourseVM.SecondName} {this.ClientCourseVM.FamilyName}";
                            string cityPerson = clientLocation?.kati ?? "-";
                            string municipalityPerson = clientLocation?.Municipality?.MunicipalityName ?? "-";
                            string districtPerson = clientLocation?.Municipality?.District?.DistrictName ?? "-";
                            string egn = "-";
                            string fid = "-";
                            string indent = this.ClientCourseVM.Indent ?? "-";
                            if (this.ClientCourseVM.IdIndentType == this.kvEGN.IdKeyValue)
                            {
                                egn = indent;
                            }
                            else
                            {
                                fid = indent;
                            }

                            string nationalityPerson = this.ClientCourseVM.IdCountryOfBirth != null
                                ? kvNationalitySource
                                    .FirstOrDefault(x => x.IdKeyValue == this.ClientCourseVM.IdCountryOfBirth)
                                    ?.Name ?? "-"
                                : "-";
                            string year = this.finishedDataModel.DocumentDate == null
                                ? "-"
                                : this.finishedDataModel.DocumentDate.Value.ToString("yyyy");
                            string formOfTraining = this.CourseVM.FormEducation.Name.ToLower();
                            string courseLength = this.CourseVM.StartDate.HasValue && this.CourseVM.EndDate.HasValue
                                ? BaseHelper.GetTotalMonths(this.CourseVM.StartDate.Value, this.CourseVM.EndDate.Value)
                                : "-";
                            string spk =
                                (await this.DataSourceService.GetKeyValueByIdAsync(this.CourseVM.Program.Speciality
                                    .IdVQS))
                                ?.DefaultValue1 ?? "-";
                            string profession =
                                (this.CourseVM.Program == null || this.CourseVM.Program.Speciality == null ||
                                 this.CourseVM.Program.Speciality.Profession == null)
                                    ? "-"
                                    : this.CourseVM.Program.Speciality.Profession.Name;
                            string speciality =
                                (this.CourseVM.Program == null || this.CourseVM.Program.Speciality == null)
                                    ? "-"
                                    : this.CourseVM.Program.Speciality.Name;
                            string chairmanOfPQC =
                                this.CourseVM.CourseCommissionMembers.Any() && protocols.Any() &&
                                commissionMember != null
                                    ? commissionMember.FullName
                                    : "-";
                            string assessedSubject = "Теория и практика";
                            string assessedGradeValue = !string.IsNullOrEmpty(this.finishedDataModel.FinalResult)
                                ? Double.Parse(this.finishedDataModel.FinalResult)
                                    .ToString("f2", System.Globalization.CultureInfo.InvariantCulture)
                                : "-";
                            string assessedGradeName = !string.IsNullOrEmpty(this.finishedDataModel.FinalResult)
                                ? BaseHelper.GetGradeName(Convert.ToDouble(this.finishedDataModel.FinalResult))
                                : "-";
                            subjects += string.Concat(Enumerable.Repeat("-\u000B",
                                31 - linesCount < 0 ? 0 : 31 - linesCount));
                            hours += string.Concat(Enumerable.Repeat("-\u000B",
                                31 - linesCount < 0 ? 0 : 31 - linesCount));
                            string qualificationLevel =
                                this.CourseVM?.Program?.FrameworkProgram?.IdQualificationLevel ==
                                this.kvQualificationLevel?.IdKeyValue
                                    ? "актуализиране, разширяване на професионалната квалификация"
                                    : "част от професия" ?? "-";
                            string pkExamDate = "-";
                            string spkExamDate = "-";
                            string absencesNumber = "-";


                            string courseName = this.CourseVM.CourseName;


                            string totalHours = (this.totalPracticeHours + this.totalTheoryHours).ToString();
                            string practiceHours = this.totalPracticeHours.ToString();
                            string theoryHours = this.totalTheoryHours.ToString();

                            string addressPerson = string.IsNullOrEmpty(this.ClientCourseVM.Address)
                                ? "-"
                                : this.ClientCourseVM.Address;
                            string yearCurriculumApproval =
                                string.IsNullOrEmpty(candidateCurriculumModification?.ValidFromDate?.ToString("yyyy"))
                                    ? "-"
                                    : candidateCurriculumModification?.ValidFromDate?.ToString("yyyy");
                            string educationType = char.ToLower(this.CourseVM.TrainingCourseTypeName[0]) +
                                                   this.CourseVM.TrainingCourseTypeName[1..];
                            string certificateNumber = "-";
                            string regNum = string.IsNullOrEmpty(this.finishedDataModel.DocumentRegNo)
                                ? "-"
                                : this.finishedDataModel.DocumentRegNo;
                            string date = this.finishedDataModel.DocumentDate == null
                                ? "-"
                                : this.finishedDataModel.DocumentDate.Value.ToString("dd.MM.yyyy");
                            string birthdayPerson = this.ClientCourseVM.BirthDate?.ToString("dd.MM.yyyy") ?? "-";
                            string documentProtocol = this.finishedDataModel.DocumentProtocol ?? "-";
                            this.protocolsSource =
                                await this.TrainingService.GetAllCourseProtocolsByIdCourseAsync(this.CourseVM.IdCourse);
                            string documentProtocolDate =
                                this.protocolsSource?.FirstOrDefault(x => x.CourseProtocolNumber == documentProtocol)
                                    ?.CourseProtocolDate?.ToString("dd.MM.yyyy")
                                ?? "-";
                            string program = this.CourseVM.Program?.FrameworkProgram?.Name ?? "-";

                            this.documentSeriesSource =
                                (await this.ProviderDocumentRequestService.GetAllDocumentSeriesAsync()).ToList();
                            string vocationalCertificateSeries = this.documentSeriesSource.FirstOrDefault(x =>
                                                                     x.IdTypeOfRequestedDocument == this
                                                                         .finishedDataModel
                                                                         .IdDocumentType)?.SeriesName ??
                                                                 "-";


                            string vocationalCertificateNumber = "-";

                            if (this.finishedDataModel.FinishedYear.HasValue)
                            {
                                if (this.finishedDataModel.FinishedYear.ToString().Length == 4 &&
                                    (this.finishedDataModel.FinishedYear.Value == DateTime.Now.Year ||
                                     this.finishedDataModel.FinishedYear.Value == DateTime.Now.Year - 1))
                                {
                                    CandidateProviderVM candidateProvider = new CandidateProviderVM()
                                    {
                                        IdCandidate_Provider = this.UserProps.IdCandidateProvider
                                    };
                                    this.documentSerialNumbersSource = this.ProviderDocumentRequestService
                                        .GetAllDocumentSerialNumbersByIdCandidateProviderByStatusReceivedByYearAndByIdCourseType(
                                            candidateProvider, this.finishedDataModel.FinishedYear.Value,
                                            this.CourseVM.IdTrainingCourseType.Value)
                                        .OrderBy(x => x.SerialNumberAsIntForOrderBy).ToList();
                                    if (this.finishedDataModel.IdDocumentSerialNumber.HasValue)
                                    {
                                        if (!this.documentSerialNumbersSource.Any(x =>
                                                x.IdDocumentSerialNumber ==
                                                this.finishedDataModel.IdDocumentSerialNumber.Value))
                                        {
                                            var serialNumber =
                                                (await this.ProviderDocumentRequestService
                                                    .GetDocumentSerialNumberByIdAndYearAsync(
                                                        this.finishedDataModel.IdDocumentSerialNumber.Value,
                                                        this.finishedDataModel.FinishedYear.Value)).SerialNumber;
                                            vocationalCertificateNumber =
                                                string.IsNullOrEmpty(serialNumber) ? "-" : serialNumber;
                                        }
                                    }
                                }
                            }

                            IEnumerable<ClientRequiredDocumentVM> clientDocuments =
                                await this.TrainingService.GetAllClientRequiredDocumentsByIdClientCourse(this
                                    .ClientCourseVM
                                    .IdClientCourse);

                            List<string> requiredDocuments = new List<string>();

                            if (clientDocuments.Any(x =>
                                    x.IdCourseRequiredDocumentType == this.kvSecondarySchoolDiploma.IdKeyValue))
                            {
                                requiredDocuments.Add(this.kvSecondarySchoolDiploma.Name.ToLower());
                            }

                            if (clientDocuments.Any(x =>
                                    x.IdCourseRequiredDocumentType == this.kvUniversityDiploma.IdKeyValue))
                            {
                                requiredDocuments.Add(this.kvUniversityDiploma.Name.ToLower());
                            }

                            string admissionDocument =
                                requiredDocuments.Count > 0 ? string.Join(", ", requiredDocuments) : "-";
                            string admissionDate = this.ClientCourseVM.CourseJoinDate?.ToString("dd.MM.yyyy") ?? "-";
                            bool isNotSPK = !(this.finishedDataModel.DocumentTypeName.ToLower()
                                .Contains("свидетелство за професионална квалификация"));

                            // If the document is not for SPK
                            string regNum2 = isNotSPK ? regNum : "-";
                            string date2 = isNotSPK ? date : "-";
                            string personName2 = isNotSPK ? personName : "-";
                            string egn2 = isNotSPK ? egn : "-";
                            string fid2 = isNotSPK ? fid : "-";
                            string nationalityPerson2 = isNotSPK ? nationalityPerson : "-";
                            string cityPerson2 = isNotSPK ? cityPerson : "-";
                            string municipalityPerson2 = isNotSPK ? municipalityPerson : "-";
                            string districtPerson2 = isNotSPK ? districtPerson : "-";
                            string year2 = isNotSPK ? year : "-";
                            string program2 = isNotSPK ? program : "-";
                            string qualificationLevel2 = isNotSPK ? qualificationLevel : "-";
                            string profession2 = isNotSPK ? profession : "-";
                            string speciality2 = isNotSPK ? speciality : "-";
                            string institutionName2 = isNotSPK ? institutionName : "-";
                            string kati2 = isNotSPK ? kati : "-";
                            string formOfTraining2 = isNotSPK ? formOfTraining : "-";
                            string courseLength2 = isNotSPK ? courseLength : "-";
                            string documentProtocol2 = isNotSPK ? documentProtocol : "-";
                            string documentDate2 = isNotSPK ? documentProtocolDate : "-";
                            string assessedGradeName2 = isNotSPK ? assessedGradeName : "-";
                            string assessedGradeValue2 = isNotSPK ? assessedGradeValue : "-";
                            string subject2 = isNotSPK ? subjects : "-";
                            string hours2 = isNotSPK ? hours : "-";
                            string chairmanOfPQC2 = isNotSPK ? chairmanOfPQC : "-";
                            string director2 = isNotSPK ? director : "-";

                            // If the document is for SPK
                            string vocationalCertificateNumber1 = isNotSPK ? "-" : vocationalCertificateNumber;
                            vocationalCertificateSeries = isNotSPK ? "-" : vocationalCertificateSeries;
                            string regNum1 = isNotSPK ? "-" : regNum;
                            string date1 = isNotSPK ? "-" : date;
                            string personName1 = isNotSPK ? "-" : personName;
                            string egn1 = isNotSPK ? "-" : egn;
                            string cityPerson1 = isNotSPK ? "-" : cityPerson;
                            string municipalityPerson1 = isNotSPK ? "-" : municipalityPerson;
                            string districtPerson1 = isNotSPK ? "-" : districtPerson;
                            string fid1 = isNotSPK ? "-" : fid;
                            string nationalityPerson1 = isNotSPK ? "-" : nationalityPerson;
                            string year1 = isNotSPK ? "-" : year;
                            string institutionName1 = isNotSPK ? "-" : institutionName;
                            string kati1 = isNotSPK ? "-" : kati;
                            string formOfTraining1 = isNotSPK ? "-" : formOfTraining;
                            string courseLength1 = isNotSPK ? "-" : courseLength;
                            string documentProtocol1 = isNotSPK ? "-" : documentProtocol;
                            string documentDate1 = isNotSPK ? "-" : documentProtocolDate;
                            string spk1 = isNotSPK ? "-" : spk;
                            string profession1 = isNotSPK ? "-" : profession;
                            string speciality1 = isNotSPK ? "-" : speciality;
                            string assessedGradeName1 = isNotSPK ? "-" : assessedGradeName;
                            string assessedGradeValue1 = isNotSPK ? "-" : assessedGradeValue;
                            string nkr1 = isNotSPK ? "-" : nkr;
                            string ekr1 = isNotSPK ? "-" : ekr;
                            string chairmanOfPQC1 = isNotSPK ? "-" : chairmanOfPQC;
                            string director1 = isNotSPK ? "-" : director;





                            string[] fieldValues = new string[]
                            {
                                //InstitutionName
                                institutionName,
                                //Kati
                                kati,
                                //MunicipalityName
                                municipalityName,
                                //Region
                                region,
                                //DistrictName
                                districtName,
                                //PersonName
                                personName,
                                //EGN
                                egn,
                                //FID
                                fid,
                                //NationalityPerson
                                nationalityPerson,
                                //BirthdayPerson
                                birthdayPerson,
                                //CityPerson
                                cityPerson,
                                //MunicipalityPerson
                                municipalityPerson,
                                //DistrictPerson
                                districtPerson,
                                //AddressPerson
                                addressPerson,
                                //YearCurriculumApproval
                                yearCurriculumApproval,
                                //AdmissionDocument
                                admissionDocument,
                                //EducationType
                                educationType,
                                //AdmissionDate
                                admissionDate,
                                //Director
                                director,
                                //RegNum2
                                regNum2,
                                //Date2
                                date2,
                                //VocationalCertificateSeries
                                vocationalCertificateSeries,
                                //CourseName
                                courseName,
                                //CourseLength
                                courseLength,
                                //TotalHours
                                totalHours,
                                //TheoryHours
                                theoryHours,
                                //PracticeHours
                                practiceHours,
                                //Subject
                                subjects,
                                //Hours
                                hours,
                                //VocationalCertificateNumber
                                vocationalCertificateNumber1,
                                //RegNum1
                                regNum1,
                                //Date1
                                date1,
                                //PersonName1
                                personName1,
                                //EGN1
                                egn1,
                                //CityPerson1
                                cityPerson1,
                                //MunicipalityPerson1
                                municipalityPerson1,
                                //DistrictPerson1
                                districtPerson1,
                                //FID1
                                fid1,
                                //NationalityPerson1
                                nationalityPerson1,
                                //Year1
                                year1,
                                //InstitutionName1
                                institutionName1,
                                //Kati1
                                kati1,
                                //FormOfTraining1
                                formOfTraining1,
                                //CourseLength1
                                courseLength1,
                                //DocumentProtocol1
                                documentProtocol1,
                                //DocumentDate1
                                documentDate1,
                                //SPK1
                                spk1,
                                //Profession1
                                profession1,
                                //Speciality1
                                speciality1,
                                //AssessedGradeName1
                                assessedGradeName1,
                                //AssessedGradeValue1
                                assessedGradeValue1,
                                //NKR1
                                nkr1,
                                //EKR1
                                ekr1,
                                //ChairmanOfPQC1
                                chairmanOfPQC1,
                                //Director1
                                director1,
                                //RegNum2
                                regNum2,
                                //Date2
                                date2,
                                //PersonName2
                                personName2,
                                //EGN2
                                egn2,
                                //FID2
                                fid2,
                                //NationalityPerson2
                                nationalityPerson2,
                                //CityPerson2
                                cityPerson2,
                                //MunicipalityPerson2
                                municipalityPerson2,
                                //DistrictPerson2
                                districtPerson2,
                                //Year2
                                year2,
                                //Program2
                                program2,
                                //QualificationLevel2
                                qualificationLevel2,
                                //Profession2
                                profession2,
                                //Speciality2
                                speciality2,
                                //InstitutionName2
                                institutionName2,
                                //Kati2
                                kati2,
                                //FormOfTraining2
                                formOfTraining2,
                                //CourseLength2
                                courseLength2,
                                //DocumentProtocol2
                                documentProtocol2,
                                //DocumentDate2
                                documentDate2,
                                //AssessedGradeName2
                                assessedGradeName2,
                                //AssessedGradeValue2
                                assessedGradeValue2,
                                //Subject2
                                subject2,
                                //Hours2
                                hours2,
                                //ChairmanOfPQC2
                                chairmanOfPQC2,
                                //Director2
                                director2
                            };

                            document.MailMerge.Execute(fieldNames,
                                fieldValues.Select(s => string.IsNullOrEmpty(s) ? "-" : s).ToArray());

                            if (!isNotSPK)
                            {
                                await CreateGradeListTable(document, 305f);
                            }

                            MemoryStream stream = new MemoryStream();
                            document.Save(stream, FormatType.Docx);
                            blueprint.Close();
                            document.Close();
                            await FileUtils.SaveAs(JS,
                                BaseHelper.ConvertCyrToLatin("ЛиченКартон_" + this.ClientCourseVM.FirstName + "_" +
                                                             this.ClientCourseVM.FamilyName) + ".docx",
                                stream.ToArray());
                        }



                        this.SpinnerHide();
                    }
                }
            }
            catch
            {
               await this.ShowErrorAsync("Неуспешно генериране на документ!");
            }
        }

        private async Task CreateGradeListTable(WordDocument document, float subjectCellWidth = 302f)
        {
            Syncfusion.Drawing.Font tableFont = new Syncfusion.Drawing.Font("Times New Roman", 11f);
            BookmarksNavigator bookNav = new BookmarksNavigator(document);

            bookNav.MoveToBookmark("Grades1Table", true, false);

            IWTable table1 = new WTable(document, true);

            table1.AutoFit(AutoFitType.FixedColumnWidth);

            var idKeyValueB = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "B").FirstOrDefault().IdKeyValue;
            var idKeyValueA1 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A1").FirstOrDefault().IdKeyValue;
            var idKeyValueA2 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A2").FirstOrDefault().IdKeyValue;
            var idKeyValueA3 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A3").FirstOrDefault().IdKeyValue;
            List<object> gradesList1 = new List<object>();
            List<object> gradesList2 = new List<object>();
            var trainingCurriculum = await this.TrainingService.GetTrainingCurriculumByIdCourseAsync(CourseVM.IdCourse);

 
            gradesList1.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA1).Where(x => x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));
            gradesList1.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA2).Where(x => x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

            gradesList1.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA3).Where(x => x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));


            if (courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA2).Any() || courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA3).Any())
            {
                gradesList1.Add("Учебна практика по:");
                gradesList1.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA2)
                    .Where(x => x.PracticeHours != 0).Where(x => !x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

                gradesList1.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA3)
                    .Where(x => x.PracticeHours != 0).Where(x => !x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

                gradesList1.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA2)
                    .Where(x => x.PracticeHours != 0).Where(x => x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

                gradesList1.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA3)
                    .Where(x => x.PracticeHours != 0).Where(x => x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

            }

            if (courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueB).Any())
            {
                gradesList2.Add("Разширена професионална подготовка:");
                gradesList2.Add("Теория:");
                gradesList2.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueB && x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));
                gradesList2.Add("Практика:");
                gradesList2.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueB && x.PracticeHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

            }

            int num = 0;
            int cellPadding = 12;
            float tableHeight = 0;
            bool isPractice = false;
            bool isSubListItem = false;
            for (int i = 0; i < gradesList1.Count(); i++)
            {
                if (gradesList1[i] is CourseSubjectVM courseSubject)
                {
                    var row = table1.AddRow(true, false);
                    row.HeightType = TableRowHeightType.Exactly;
                    var courseSubjectGrade =
                        await this.TrainingService
                            .GetClientCourseSubjectGradeByClientCourseIdAndByIdCourseSubjectAsync(
                                this.ClientCourseVM.IdClientCourse,
                                courseSubject.IdCourseSubject); ;

                    WTableCell cell;

                    string subject = courseSubject.Subject == null
                        ? "-"
                        : (isSubListItem && !courseSubject.Subject.ToLower().Contains("производствена практика") ? "– " : "") + courseSubject.Subject;

                    string lines = string.Concat(
                            Enumerable.Repeat("-\u000B", BaseHelper.CalculateNumberOfLines(subject == null
                                ? "-"
                                : subject, subjectCellWidth - cellPadding, new Font("Times New Roman", 11f)) - 1));

                    cell = row.AddCell();
                    IWTextRange textRange = cell.AddParagraph().AppendText(subject);
                    textRange.CharacterFormat.Font = tableFont;
                    cell.Width = subjectCellWidth;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    tableHeight += BaseHelper.CalculateNumberOfLines(subject == null
                        ? "-"
                        : subject, cell.Width - cellPadding, new Font("Times New Roman", 11f));

                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(lines + ((!isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade) == null
                        ? "-"
                        : BaseHelper.GetGradeName((!isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade).Value)));
                    textRange.CharacterFormat.Font = tableFont;
                    cell.Width = 96;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(lines + ((!isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade) == null
                        ? "-"
                        : (!isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade).Value.ToString("f2", System.Globalization.CultureInfo.InvariantCulture)));
                    textRange.CharacterFormat.Font = tableFont;
                    cell.Width = 84;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(lines + ((!isPractice ? courseSubject.TheoryHours : courseSubject.PracticeHours) != 0
                            ? (!isPractice ? courseSubject.TheoryHours : courseSubject.PracticeHours) + ""
                            : "-"));
                    textRange.CharacterFormat.Font = tableFont;
                    cell.Width = 78;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                }
                else if (gradesList1[i] is string category)
                {
                    if (category.Equals("Учебна практика по:"))
                    {
                        isPractice = true;
                        isSubListItem = true;
                    }
                    else if (category.Equals("Теория:"))
                    {
                        isPractice = false;
                        isSubListItem = true;
                    }
                    else if (category.Equals("Практика:"))
                    {
                        isPractice = true;
                        isSubListItem = true;
                    }
                    else
                    {
                        isSubListItem = false;
                    }

                    if (!(category.Equals("Теория:") || category.Equals("Практика:")))
                    {
                        var row = table1.AddRow(true, false);
                        row.HeightType = TableRowHeightType.Exactly;
                        WTableCell cell;

                        string lines = string.Concat(
                            Enumerable.Repeat("\u000B-",
                                BaseHelper.CalculateNumberOfLines(category, subjectCellWidth - cellPadding,
                                    new Font("Times New Roman", 11f, FontStyle.Bold)) - 1));
                        cell = row.AddCell();
                        IWTextRange textRange = cell.AddParagraph().AppendText(category);
                        textRange.CharacterFormat.Font = tableFont;
                        cell.Width = subjectCellWidth;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                        tableHeight += BaseHelper.CalculateNumberOfLines(category, cell.Width - cellPadding,
                            new Font("Times New Roman", 11f, FontStyle.Bold));

                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("-" + lines);
                        textRange.CharacterFormat.Font = tableFont;
                        cell.Width = 96;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("-" + lines);
                        textRange.CharacterFormat.Font = tableFont;
                        cell.Width = 84;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("-" + lines);
                        textRange.CharacterFormat.Font = tableFont;
                        cell.Width = 78;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    }
                }
            }

            table1.TableFormat.Borders.BorderType = BorderStyle.Cleared;
            bookNav.InsertTable(table1);

            bookNav.MoveToBookmark("Grades2Table", true, false);
            IWTable table2 = new WTable(document, true);
            tableHeight = 0;

            for (int i = num; i < gradesList2.Count(); i++)
            {
                if (gradesList2[i] is CourseSubjectVM courseSubject)
                {
                    var row = table2.AddRow(true, false);
                    var courseSubjectGrade =
                        await this.TrainingService
                            .GetClientCourseSubjectGradeByClientCourseIdAndByIdCourseSubjectAsync(
                                this.ClientCourseVM.IdClientCourse,
                                courseSubject.IdCourseSubject); ;

                    WTableCell cell;
                    string subject = courseSubject.Subject == null
                        ? "-"
                        : (isSubListItem && !courseSubject.Subject.ToLower().Contains("производствена практика") ? "– " : "") + courseSubject.Subject;

                    string lines = string.Concat(
                            Enumerable.Repeat("-\u000B", BaseHelper.CalculateNumberOfLines(subject == null
                                ? "-"
                                : subject, subjectCellWidth - cellPadding, new Font("Times New Roman", 11f)) - 1));

                    cell = row.AddCell();
                    IWTextRange textRange = cell.AddParagraph().AppendText(subject);
                    cell.Width = subjectCellWidth;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    tableHeight += BaseHelper.CalculateNumberOfLines(subject == null
                        ? "-"
                        : subject, cell.Width - cellPadding, new Font("Times New Roman", 11f));

                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(lines + ((!isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade) == null
                       ? "-"
                       : BaseHelper.GetGradeName((!isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade).Value)));
                    textRange.CharacterFormat.Font = tableFont;
                    cell.Width = 96;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(lines + ((!isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade) == null
                       ? "-"
                       : (!isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade).Value.ToString("f2", System.Globalization.CultureInfo.InvariantCulture)));
                    textRange.CharacterFormat.Font = tableFont;
                    cell.Width = 84;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(lines + ((!isPractice ? courseSubject.TheoryHours : courseSubject.PracticeHours) != 0
                           ? (!isPractice ? courseSubject.TheoryHours : courseSubject.PracticeHours) + ""
                           : "-"));
                    textRange.CharacterFormat.Font = tableFont;
                    cell.Width = 78;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                }
                else if (gradesList2[i] is string category)
                {
                    if (category.Equals("Учебна практика по:"))
                    {
                        isPractice = true;
                        isSubListItem = true;
                    }
                    else if (category.Equals("Теория:"))
                    {
                        isPractice = false;
                        isSubListItem = true;
                    }
                    else if (category.Equals("Практика:"))
                    {
                        isPractice = true;
                        isSubListItem = true;
                    }
                    else
                    {
                        isSubListItem = false;
                    }

                    if (!(category.Equals("Теория:") || category.Equals("Практика:")))
                    {
                        var row = table2.AddRow(true, false);
                        row.HeightType = TableRowHeightType.Exactly;
                        WTableCell cell;


                        string lines = string.Concat(
                            Enumerable.Repeat("\u000B-",
                                BaseHelper.CalculateNumberOfLines(category, subjectCellWidth - cellPadding,
                                    new Font("Times New Roman", 11f, FontStyle.Bold)) - 1));

                        cell = row.AddCell();
                        IWTextRange textRange = cell.AddParagraph().AppendText(category);
                        cell.Width = subjectCellWidth;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                        tableHeight += BaseHelper.CalculateNumberOfLines(category, cell.Width - cellPadding,
                            new Font("Times New Roman", 11f, FontStyle.Bold));

                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("-" + lines);
                        textRange.CharacterFormat.Font = tableFont;
                        cell.Width = 96;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("-" + lines);
                        textRange.CharacterFormat.Font = tableFont;
                        cell.Width = 84;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("-" + lines);
                        textRange.CharacterFormat.Font = tableFont;
                        cell.Width = 78;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    }

                }

            }






            table2.TableFormat.Borders.BorderType = BorderStyle.Cleared;

            bookNav.InsertTable(table2);
        }

        // ресетва бройката с часовете от учебната програма
        private void ResetHours()
        {
            this.generalProfessionTrainingHours = 0;
            this.industryProfessionTrainingHours = 0;
            this.specificProfessionTrainingHours = 0;
            this.extendedProfessionTrainingHours = 0;
            this.practiceHours = 0;
            this.theoryHours = 0;
            this.totalTheoryHours = 0;
            this.totalPracticeHours = 0;
        }
        // пресмята общият брой часове за учебна програма
        private void CalculateCurriculumHours()
        {
            this.ResetHours();

            foreach (var curriculum in this.addedCurriculums)
            {
                if (curriculum.Theory.HasValue)
                {
                    this.theoryHours = curriculum.Theory.Value;
                    this.totalTheoryHours += curriculum.Theory.Value;
                }
                else
                {
                    this.theoryHours = 0;
                }

                if (curriculum.Practice.HasValue)
                {
                    this.totalPracticeHours += curriculum.Practice.Value;
                }

                if (curriculum.ProfessionalTraining != "Б")
                {
                    if (curriculum.Practice.HasValue)
                    {
                        this.practiceHours += curriculum.Practice.Value;
                    }
                    else
                    {
                        this.practiceHours += 0;
                    }
                }

                if (curriculum.ProfessionalTraining == "Б")
                {
                    var bPracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                    var bTheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                    this.extendedProfessionTrainingHours += (bPracticeHours + bTheoryHours);
                }

                if (curriculum.ProfessionalTraining == "А1")
                {
                    var a1PracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                    var a2TheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                    this.generalProfessionTrainingHours += (a1PracticeHours + a2TheoryHours);
                }

                if (curriculum.ProfessionalTraining == "А2")
                {
                    double a2PracticeHours = 0;
                    if (curriculum.Practice.HasValue)
                    {
                        a2PracticeHours = curriculum.Practice.Value;
                    }
                    else
                    {
                        a2PracticeHours = 0;
                    }

                    this.industryProfessionTrainingHours += (a2PracticeHours + this.theoryHours);
                }

                if (curriculum.ProfessionalTraining == "А3")
                {
                    double a3PracticeHours = 0;
                    if (curriculum.Practice.HasValue)
                    {
                        a3PracticeHours = curriculum.Practice.Value;
                    }
                    else
                    {
                        a3PracticeHours = 0;
                    }

                    this.specificProfessionTrainingHours += (a3PracticeHours + this.theoryHours);
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

        private void IndentChanged(ChangeEventArgs args)
        {
            var indent = this.ClientCourseVM.Indent;
            if (indent != null)
            {
                if (this.ClientCourseVM.IdIndentType == this.kvEGN.IdKeyValue)
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

                        this.ClientCourseVM.BirthDate = BirthDate;

                        var beforeLastNumber = int.Parse(indent.Substring(indent.Length - 2, 1));

                        if (beforeLastNumber % 2 == 0)
                        {
                            this.ClientCourseVM.IdSex = this.kvSexSource.FirstOrDefault(x => x.Name == "Мъж").IdKeyValue;
                        }
                        else
                        {
                            this.ClientCourseVM.IdSex = this.kvSexSource.FirstOrDefault(x => x.Name == "Жена").IdKeyValue;
                        }
                    }
                    else
                    {
                        this.ClientCourseVM.BirthDate = null;
                        this.ClientCourseVM.IdSex = null;
                    }
                }
                else
                {
                    this.ClientCourseVM.BirthDate = null;
                    this.ClientCourseVM.IdSex = null;
                }
            }
        }

        private void ValidateEGN(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.ClientCourseVM.Indent != null)
            {
                this.ClientCourseVM.Indent = this.ClientCourseVM.Indent.Trim();

                if (this.ClientCourseVM.Indent.Length != 10)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ClientCourseVM, "Indent");
                    this.messageStore?.Add(fi, $"Полето {this.identType} трябва да съдържа 10 символа!");
                }
                else
                {
                    if (this.ClientCourseVM.IdIndentType == this.kvEGN.IdKeyValue)
                    {
                        var checkEGN = new BasicEGNValidation(this.ClientCourseVM.Indent);

                        if (!checkEGN.Validate())
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.ClientCourseVM, "Indent");
                            this.messageStore?.Add(fi, checkEGN.ErrorMessage);
                        }
                    }
                }
            }
            else
            {
                FieldIdentifier fi = new FieldIdentifier(this.ClientCourseVM, "Indent");
                this.messageStore?.Add(fi, $"Полето '{this.identType}' е задължително!");
            }
        }

        private void ValidateCountryAndCityOfBirth(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.ClientCourseVM.IdCountryOfBirth != null)
            {
                if (this.ClientCourseVM.IdCountryOfBirth == this.kvBGNationality.IdKeyValue && this.ClientCourseVM.IdCityOfBirth == null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ClientCourseVM, "IdCityOfBirth");
                    this.messageStore?.Add(fi, "Полето 'Месторождение (населено място) е задължително!'");
                }
            }
        }

        private void ValidateCourseJoinDate(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.ClientCourseVM.CourseJoinDate.HasValue)
            {
                if (this.ClientCourseVM.CourseJoinDate.Value > this.CourseVM.EndDate.Value)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ClientCourseVM, "CourseJoinDate");
                    this.messageStore?.Add(fi, $"Полето 'Дата на включване' не може да бъде след {this.CourseVM.EndDate.Value.ToString("dd.MM.yyyy")}г.!");
                    return;
                }
            }
        }

        private void ValidateSecondName(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.ClientCourseVM.IdIndentType == this.kvEGN.IdKeyValue)
            {
                if (string.IsNullOrEmpty(this.ClientCourseVM.SecondName))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ClientCourseVM, "SecondName");
                    this.messageStore?.Add(fi, $"Полето 'Презиме' е задължително!");
                }
            }
        }

        private void ValidateClientAge(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.ClientCourseVM.BirthDate.HasValue && this.CourseVM.StartDate.HasValue)
            {
                var difference = this.CourseVM.StartDate.Value.Year - this.ClientCourseVM.BirthDate.Value.Year;
                if (difference == 16)
                {
                    var startDate = new DateTime(DateTime.Now.Year, this.CourseVM.StartDate.Value.Month, this.CourseVM.StartDate.Value.Day);
                    var birthDate = new DateTime(DateTime.Now.Year, this.ClientCourseVM.BirthDate.Value.Month, this.ClientCourseVM.BirthDate.Value.Day);
                    if (startDate < birthDate)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.ClientCourseVM, "BirthDate");
                        this.messageStore?.Add(fi, "Не може да запишете за курса лице, което няма навършени 16 години към датата на започване на курса!");
                    }
                }

                if (difference < 16)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.ClientCourseVM, "BirthDate");
                    this.messageStore?.Add(fi, "Не може да запишете за курса лице, което няма навършени 16 години към датата на започване на курса!");
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

                var result = await this.TrainingService.GetClientByIdIndentTypeByIndentAndByIdCandidateProviderAsync(this.ClientCourseVM.IdIndentType.Value, this.ClientCourseVM.Indent, this.CourseVM.IdCandidateProvider.Value);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    var modelFromDb = result.ResultContextObject;
                    this.ClientCourseVM.FirstName = modelFromDb.FirstName;
                    this.ClientCourseVM.SecondName = modelFromDb.SecondName;
                    this.ClientCourseVM.FamilyName = modelFromDb.FamilyName;
                    this.ClientCourseVM.IdSex = modelFromDb.IdSex;
                    this.ClientCourseVM.IdIndentType = modelFromDb.IdIndentType;
                    this.ClientCourseVM.Indent = modelFromDb.Indent;
                    this.ClientCourseVM.BirthDate = modelFromDb.BirthDate;
                    this.ClientCourseVM.IdNationality = modelFromDb.IdNationality;
                    this.ClientCourseVM.IdCountryOfBirth = modelFromDb.IdCountryOfBirth;
                    this.ClientCourseVM.IdCityOfBirth = modelFromDb.IdCityOfBirth;

                    this.locationSource.Clear();

                    if (this.ClientCourseVM.IdCityOfBirth is not null)
                    {
                        LocationVM location = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.ClientCourseVM.IdCityOfBirth.Value);
                        this.locationSource.Add(location);
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnFilterCityOfBirth(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.locationSource = (List<LocationVM>)await this.LocationService.GetAllLocationsByKatiAsync(args.Text);
                }
                catch (Exception ex) { }

                var query = new Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.autoCompleteLocation.FilterAsync(this.locationSource, query);
            }
        }

        private void CountryOfBirthValueChangeHandler(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.ItemData == null)
            {
                this.ClientCourseVM.IdCityOfBirth = null;
            }
        }

        public void SetEditContextAsUnmodified()
        {
            if (this.editContext is not null)
                this.editContext.MarkAsUnmodified();
        }

        public bool GetIsEditContextModified()
        {
            if (this.editContext is not null)
            {
                return this.editContext.IsModified();
            }

            return false;
        }
    }
}
