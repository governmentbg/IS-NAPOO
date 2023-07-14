using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Request;
using Data.Models.Data.Role;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Prng;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class ImportUserService : BaseService, IImportUserService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly IMailService mailService;
        private readonly ILogger<ImportUserService> _logger;
        private readonly IApplicationUserService applicationUserService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public ImportUserService(IRepository repository,
            IDataSourceService dataSourceService,
            ILogger<ImportUserService> logger,
            IApplicationUserService applicationUserService,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMailService mailService,
            AuthenticationStateProvider authStateProvider)
            : base(repository, authStateProvider)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this.mailService = mailService;
            this._logger = logger;
            this.applicationUserService = applicationUserService;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<ResultContext<List<PersonVM>>> ImportUsersAsync(MemoryStream file, string fileName, int idImportType)
        {
            ResultContext<List<PersonVM>> resultContext = new ResultContext<List<PersonVM>>();

            List<PersonVM> persons = new List<PersonVM>();
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportUsers";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + fileName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IApplication app = excelEngine.Excel;

                        IWorkbook workbook = app.Workbooks.Open(fileStream, ExcelOpenType.Automatic);

                        IWorksheet worksheet = workbook.Worksheets[0];

                        bool skipFirstRow = true;

                        var importUsersTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ImportUsersType");
                        var kvCPOTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "CPO");
                        var kvCIPOTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "CIPO");
                        var kvExternalExpertTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ExternalExperts");
                        var kvNAPOOExpertTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "NAPOOExperts");
                        var rowCounter = 2;
                        int counter = GlobalConstants.INVALID_ID_ZERO;
                        var licenceDict = new Dictionary<string, List<string>>();
                        foreach (var row in worksheet.Rows)
                        {
                            //Ако сме пропуснали 5 реда приемаме че документа е приключил и са останали празни редове
                            if (counter == 5)
                            {
                                break;
                            }

                            //Пропуска 1 ред който е с хедърите
                            if (skipFirstRow || string.IsNullOrEmpty(row.Cells[1].Value))
                            {
                                skipFirstRow = false;
                                counter++;
                                continue;
                            }

                            if (idImportType == kvCIPOTypeValue?.IdKeyValue || idImportType == kvCPOTypeValue?.IdKeyValue)
                            {
                                var person = new PersonVM();

                                var provider = row.Cells[2].Value.Trim();
                                string providerOwner = string.Empty;
                                if (string.IsNullOrEmpty(provider))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведено наименование на центъра!");
                                }
                                else
                                {
                                    if (provider.StartsWith("ЦПО към "))
                                    {
                                        providerOwner = provider.Split("ЦПО към ", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else if (provider.StartsWith("ЦПО "))
                                    {
                                        providerOwner = provider.Split("ЦПО ", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else if (provider.StartsWith("ЦИПО към "))
                                    {
                                        providerOwner = provider.Split("ЦИПО към ", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else if (provider.StartsWith("ЦИПО "))
                                    {
                                        providerOwner = provider.Split("ЦИПО ", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else if (provider.StartsWith("Център за професионално обучение към "))
                                    {
                                        providerOwner = provider.Split("Център за професионално обучение към ", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else if (provider.StartsWith("Център за професионално обучение "))
                                    {
                                        providerOwner = provider.Split("Център за професионално обучение ", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else if (provider.StartsWith("Център за информиране за професионално обучение към "))
                                    {
                                        providerOwner = provider.Split("Център за информиране за професионално обучение към ", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else if (provider.StartsWith("Център за информиране за професионално обучение "))
                                    {
                                        providerOwner = provider.Split("Център за информиране за професионално обучение ", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else
                                    {
                                        providerOwner = provider;
                                    }

                                    person.ProviderOwner = providerOwner;
                                }

                                var licenseNumber = row.Cells[3].Value.Trim();
                                if (string.IsNullOrEmpty(licenseNumber))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведен номер на лицензия!");
                                }
                                else
                                {
                                    long licenseNumberAsLong = 0;
                                    if (!long.TryParse(licenseNumber, out licenseNumberAsLong))
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter} няма въведен валиден номер на лицензия. Номерът може да съдържа само цифри!");
                                    }
                                    else
                                    {
                                        person.LicenseNumber = licenseNumber;
                                    }
                                }

                                var firtNameRepresentativeOne = row.Cells[4].Value.Trim();
                                if (string.IsNullOrEmpty(firtNameRepresentativeOne))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведено име на Представител 1!");
                                }
                                else
                                {
                                    if (!Regex.IsMatch(firtNameRepresentativeOne, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} име на Представител 1 може да съдържа само символи на кирилица!");
                                    }
                                    else
                                    {
                                        person.FirstName = firtNameRepresentativeOne;
                                    }
                                }

                                var familyNameRepresentativeOne = row.Cells[5].Value.Trim();
                                if (string.IsNullOrEmpty(familyNameRepresentativeOne))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведена фамилия на Представител 1!");
                                }
                                else
                                {
                                    if (!Regex.IsMatch(familyNameRepresentativeOne, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} фамилия на Представител 1 може да съдържа само символи на кирилица!");
                                    }
                                    else
                                    {
                                        person.FamilyName = familyNameRepresentativeOne;
                                    }
                                }

                                var phoneNumberRepresentativeOne = row.Cells[6].Value.Trim();
                                if (!string.IsNullOrEmpty(phoneNumberRepresentativeOne))
                                {
                                    var phoneNumber = string.Empty;
                                    if (phoneNumberRepresentativeOne.StartsWith("+"))
                                    {
                                        phoneNumber = phoneNumberRepresentativeOne.Split("+", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else
                                    {
                                        phoneNumber = phoneNumberRepresentativeOne;
                                    }

                                    person.Phone = phoneNumber;
                                }

                                var emailRepresentatvieOne = row.Cells[7].Value.Trim().ToLower();
                                if (string.IsNullOrEmpty(emailRepresentatvieOne))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведен имейл адрес на Представител 1!");
                                }
                                else
                                {
                                    var pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                                    if (Regex.IsMatch(emailRepresentatvieOne, pattern))
                                    {
                                        person.Email = emailRepresentatvieOne;
                                    }
                                    else
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за имейл адрес на Представител 1!");
                                    }
                                }

                                if (!string.IsNullOrEmpty(person.LicenseNumber) && !string.IsNullOrEmpty(person.Email))
                                {
                                    if (!licenceDict.ContainsKey(person.LicenseNumber))
                                    {
                                        licenceDict.Add(person.LicenseNumber, new List<string>());
                                    }

                                    if (licenceDict[person.LicenseNumber].Count < 2)
                                    {
                                        licenceDict[person.LicenseNumber].Add(person.Email);
                                    }
                                    else
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не може да бъде добавен Представител 1, защото за № на лицензия {person.LicenseNumber} има вече добавени двама представители!");
                                    }
                                }

                                persons.Add(person);

                                var firtNameRepresentativeTwo = row.Cells[8].Value.Trim();
                                var familyNameRepresentativeTwo = row.Cells[9].Value.Trim();
                                if (firtNameRepresentativeTwo != "-" && familyNameRepresentativeTwo != "-" && firtNameRepresentativeTwo != "-" && familyNameRepresentativeTwo != "-")
                                {
                                    person = new PersonVM();

                                    person.LicenseNumber = licenseNumber;
                                    person.ProviderOwner = providerOwner;

                                    if (!Regex.IsMatch(firtNameRepresentativeTwo, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} име на Представител 2 може да съдържа само символи на кирилица!");
                                    }
                                    else
                                    {
                                        person.FirstName = firtNameRepresentativeTwo;
                                    }

                                    if (!Regex.IsMatch(familyNameRepresentativeTwo, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} фамилия на Представител 2 може да съдържа само символи на кирилица!");
                                    }
                                    else
                                    {
                                        person.FamilyName = familyNameRepresentativeTwo;
                                    }

                                    var phoneNumberRepresentativeTwo = row.Cells[10].Value.Trim();
                                    if (!string.IsNullOrEmpty(phoneNumberRepresentativeTwo))
                                    {
                                        var phoneNumber = string.Empty;
                                        if (phoneNumberRepresentativeTwo.StartsWith("+"))
                                        {
                                            phoneNumber = phoneNumberRepresentativeTwo.Split("+", StringSplitOptions.RemoveEmptyEntries)[0];
                                        }
                                        else
                                        {
                                            phoneNumber = phoneNumberRepresentativeTwo;
                                        }

                                        person.Phone = phoneNumber;
                                    }

                                    var emailRepresentativeTwo = row.Cells[11].Value.Trim().ToLower();
                                    if (string.IsNullOrEmpty(emailRepresentativeTwo))
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter} няма въведен имейл адрес на Представител 2!");
                                    }
                                    else
                                    {
                                        var pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                                        if (Regex.IsMatch(emailRepresentativeTwo, pattern))
                                        {
                                            person.Email = emailRepresentativeTwo;
                                        }
                                        else
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за имейл адрес на Представител 2!");
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(person.LicenseNumber) && !string.IsNullOrEmpty(person.Email))
                                    {
                                        if (!licenceDict.ContainsKey(person.LicenseNumber))
                                        {
                                            licenceDict.Add(person.LicenseNumber, new List<string>());
                                        }

                                        if (licenceDict[person.LicenseNumber].Count < 2)
                                        {
                                            licenceDict[person.LicenseNumber].Add(person.Email);
                                        }
                                        else
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} не може да бъде добавен Представител 2, защото за № на лицензия {person.LicenseNumber} има вече добавени двама представители!");
                                        }
                                    }

                                    persons.Add(person);
                                }
                            }
                            else if (idImportType == kvExternalExpertTypeValue?.IdKeyValue)
                            {
                                var person = new PersonVM();

                                var fullName = row.Cells[1].Value.Trim();
                                string providerOwner = string.Empty;
                                if (string.IsNullOrEmpty(fullName))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведено име на външен експерт!");
                                }
                                else
                                {
                                    var fullNameAsArr = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                    if (fullNameAsArr[0] == "доц." && fullNameAsArr[1] == "д-р")
                                    {
                                        var title = $"{fullNameAsArr[0]} {fullNameAsArr[1]}";
                                        person.Title = title;
                                    }
                                    else if (fullNameAsArr[0] == "доц.")
                                    {
                                        person.Title = fullNameAsArr[0];
                                    }
                                    else
                                    {
                                        var firstName = string.Empty;
                                        var secondName = string.Empty;
                                        var familyName = string.Empty;
                                        if (fullNameAsArr.Length == 3)
                                        {
                                            if (fullNameAsArr[0] == "доц.")
                                            {
                                                firstName = fullNameAsArr[1];
                                                familyName = fullNameAsArr[2];
                                            }
                                            else
                                            {
                                                firstName = fullNameAsArr[0];
                                                secondName = fullNameAsArr[1];
                                                familyName = fullNameAsArr[2];
                                            }
                                        }
                                        else if (fullNameAsArr.Length == 2)
                                        {
                                            firstName = fullNameAsArr[0];
                                            familyName = fullNameAsArr[1];
                                        }
                                        else if (fullNameAsArr.Length == 5)
                                        {
                                            firstName = fullNameAsArr[2];
                                            secondName = fullNameAsArr[3];
                                            familyName = fullNameAsArr[4];
                                        }

                                        if (familyName.Contains(','))
                                        {
                                            var idx = familyName.IndexOf(',');
                                            familyName.Remove(idx, 1);
                                        }

                                        if (!Regex.IsMatch(firstName, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} име на външен експерт може да съдържа само символи на кирилица!");
                                        }
                                        else
                                        {
                                            person.FirstName = firstName;
                                        }

                                        if (!string.IsNullOrEmpty(secondName))
                                        {
                                            if (!Regex.IsMatch(secondName, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                            {
                                                resultContext.AddErrorMessage($"На ред {rowCounter} презиме на външен експерт може да съдържа само символи на кирилица!");
                                            }
                                            else
                                            {
                                                person.SecondName = secondName;
                                            }
                                        }

                                        if (!Regex.IsMatch(familyName, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} фамилия на външен експерт може да съдържа само символи на кирилица!");
                                        }
                                        else
                                        {
                                            person.FamilyName = familyName;
                                        }
                                    }
                                }

                                var email = row.Cells[4].Value.Trim().ToLower();
                                if (string.IsNullOrEmpty(email))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведен имейл адрес на външен експерт!");
                                }
                                else
                                {
                                    var pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                                    if (Regex.IsMatch(email, pattern))
                                    {
                                        person.Email = email;
                                    }
                                    else
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за имейл адрес на външен експерт!");
                                    }
                                }

                                var phone = row.Cells[5].Value.Trim().ToLower();
                                if (!string.IsNullOrEmpty(phone))
                                {
                                    person.Phone = phone;
                                }

                                person.Position = "Външен експерт";

                                persons.Add(person);
                            }
                            else if (idImportType == kvNAPOOExpertTypeValue?.IdKeyValue)
                            {
                                var person = new PersonVM();

                                var title = row.Cells[0].Value.Trim();
                                string providerOwner = string.Empty;
                                if (!string.IsNullOrEmpty(title))
                                {
                                    person.Title = title;
                                }

                                var firstName = row.Cells[1].Value.Trim();
                                if (string.IsNullOrEmpty(firstName))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведено име на експерт към НАПОО!");
                                }
                                else
                                {
                                    if (!Regex.IsMatch(firstName, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} име на експерт към НАПОО може да съдържа само символи на кирилица!");
                                    }
                                    else
                                    {
                                        person.FirstName = firstName;
                                    }
                                }

                                var familyName = row.Cells[2].Value.Trim();
                                if (string.IsNullOrEmpty(familyName))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена фамилия на експерт към НАПОО!");
                                }
                                else
                                {
                                    if (!Regex.IsMatch(familyName, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} fфамилия на експерт към НАПОО може да съдържа само символи на кирилица!");
                                    }
                                    else
                                    {
                                        person.FamilyName = familyName;
                                    }
                                }

                                var position = row.Cells[3].Value.Trim();
                                if (!string.IsNullOrEmpty(position))
                                {
                                    person.Position = position;
                                }

                                var phone = row.Cells[4].Value.Trim();
                                if (!string.IsNullOrEmpty(phone))
                                {
                                    person.Phone = phone;
                                }

                                var email = row.Cells[5].Value.Trim().ToLower();
                                if (string.IsNullOrEmpty(email))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведен имейл адрес на експерт към НАПОО!");
                                }
                                else
                                {
                                    var pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                                    if (Regex.IsMatch(email, pattern))
                                    {
                                        person.Email = email;
                                    }
                                    else
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за имейл адрес на експерт към НАПОО!");
                                    }
                                }

                                persons.Add(person);
                            }

                            rowCounter++;
                        }
                    }

                    if (persons.Any())
                    {
                        resultContext.AddMessage("Импортът приключи успешно!");
                    }

                    resultContext.ResultContextObject = persons;
                }
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public MemoryStream CreateExcelWithErrors(ResultContext<List<PersonVM>> resultContext)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 50;
                sheet.Range[$"A1"].Text = "Вид на грешките:";
                //sheet.Range[$"B1"].Text = "Позиция във файла";

                var rowCounter = 2;
                foreach (var item in resultContext.ListErrorMessages)
                {
                    //var splitMsg = item.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    //var msg = splitMsg[0].Trim();
                    //var cell = splitMsg[1].Trim();

                    //sheet.Range[$"A{rowCounter}"].Text = msg;
                    sheet.Range[$"A{rowCounter}"].Text = item;
                    //sheet.Range[$"B{rowCounter}"].Text = cell;

                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task<(MemoryStream? MS, bool IsSuccessfull)> CreateUsersAsync(List<PersonVM> persons, ImportUsersVM model)
        {
            MemoryStream? ms = null;
            bool isSuccessfull = false;
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var importInfoList = new List<string>();
                var importStart = DateTime.Now;
                var importUsersTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ImportUsersType");
                var kvCPOTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "CPO");
                var kvCIPOTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "CIPO");
                var kvExternalExpertTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ExternalExperts");
                var kvNAPOOExpertsTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "NAPOOExperts");
                var kvUserStatusActive = await this.dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active");
                var usersCount = 0;
                var providersCount = 0;
                CandidateProvider candidateProvider = null;
                CandidateProvider cipoCandidateProvider = null;
                var kvApplicationAccepted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureCompleted");
                var userID = await this.dataSourceService.GetSettingByIntCodeAsync("UserIDBindWithSystem");
                var userFromSystem = await this.userManager.FindByIdAsync(userID.SettingValue);
                var kvExpertTypeExternalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ExpertType", "ExternalExpert");
                var kvExpertActiveStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ExpertStatus", "ActiveExpert");
                if (model.IdImportType == kvCIPOTypeValue!.IdKeyValue || model.IdImportType == kvCPOTypeValue!.IdKeyValue)
                {
                    var idCandidateProviderForGetFromDb = model.IdImportType == kvCIPOTypeValue!.IdKeyValue
                        ? 1237
                        : 1165;

                    candidateProvider = await this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == idCandidateProviderForGetFromDb)
                        .Include(x => x.CandidateProviderSpecialities).ThenInclude(x => x.CandidateCurriculumModifications).ThenInclude(x => x.CandidateCurriculums).ThenInclude(x => x.CandidateCurriculumERUs).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerProfiles).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerQualifications).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerSpecialities).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesRooms).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesSpecialities).AsNoTracking()
                        .Include(x => x.CandidateProviderCIPOStructureAndActivities).AsNoTracking()
                        .Include(x => x.CandidateProviderCPOStructureAndActivities).AsNoTracking()
                        .Include(x => x.CandidateProviderDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderConsultings).AsNoTracking()
                        .Include(x => x.ProviderRequestDocuments).ThenInclude(x => x.RequestDocumentTypes).AsNoTracking()
                        .Include(x => x.ProviderRequestDocuments).ThenInclude(x => x.RequestDocumentStatuses).AsNoTracking()
                        .FirstOrDefaultAsync();
                }
                else if (model.IdImportType == kvExternalExpertTypeValue?.IdKeyValue)
                {
                    var kvDocTypeApplication11 = await this.dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application11");
                    var kvDocTypeApplication4 = await this.dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "Application4");
                    candidateProvider = await this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == 1162)
                        .Include(x => x.CandidateProviderSpecialities).ThenInclude(x => x.CandidateCurriculumModifications).ThenInclude(x => x.CandidateCurriculums).ThenInclude(x => x.CandidateCurriculumERUs).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerProfiles).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerQualifications).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerSpecialities).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesRooms).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesSpecialities).AsNoTracking()
                        .Include(x => x.CandidateProviderCPOStructureAndActivities).AsNoTracking()
                        .Include(x => x.CandidateProviderDocuments).AsNoTracking()
                        .Include(x => x.StartedProcedure).ThenInclude(x => x.StartedProcedureProgresses).AsNoTracking()
                        .Include(x => x.StartedProcedure).ThenInclude(x => x.ProcedureDocuments.Where(y => y.IdDocumentType != kvDocTypeApplication11.IdKeyValue && y.IdDocumentType != kvDocTypeApplication4.IdKeyValue)).AsNoTracking()
                        .FirstOrDefaultAsync();
                }
                else if (model.IdImportType == kvNAPOOExpertsTypeValue?.IdKeyValue)
                {
                    var kvDocSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "RequestedByCPOOrCIPO");
                    var kvDocTypeApplicationCPO = await this.dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "RequestLicensingCPO");
                    var kvDocTypeApplicationCIPO = await this.dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "RequestLicensingCIPO");
                    candidateProvider = await this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == 1162)
                        .Include(x => x.CandidateProviderSpecialities).ThenInclude(x => x.CandidateCurriculumModifications).ThenInclude(x => x.CandidateCurriculums).ThenInclude(x => x.CandidateCurriculumERUs).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerProfiles).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerQualifications).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerSpecialities).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesRooms).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesSpecialities).AsNoTracking()
                        .Include(x => x.CandidateProviderCPOStructureAndActivities).AsNoTracking()
                        .Include(x => x.CandidateProviderDocuments).AsNoTracking()
                        .Include(x => x.StartedProcedure).ThenInclude(x => x.StartedProcedureProgresses.Where(y => y.IdStep == kvDocSubmitted.IdKeyValue)).AsNoTracking()
                        .Include(x => x.StartedProcedure).ThenInclude(x => x.ProcedureDocuments.Where(y => y.IdDocumentType == kvDocTypeApplicationCPO.IdKeyValue)).AsNoTracking()
                        .FirstOrDefaultAsync();

                    cipoCandidateProvider = await this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == 1237)
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerProfiles).AsNoTracking()
                        .Include(x => x.CandidateProviderTrainers).ThenInclude(x => x.CandidateProviderTrainerQualifications).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderPremises).ThenInclude(x => x.CandidateProviderPremisesRooms).AsNoTracking()
                        .Include(x => x.CandidateProviderCIPOStructureAndActivities).AsNoTracking()
                        .Include(x => x.CandidateProviderDocuments).AsNoTracking()
                        .Include(x => x.CandidateProviderConsultings).AsNoTracking()
                        .Include(x => x.StartedProcedure).ThenInclude(x => x.StartedProcedureProgresses.Where(y => y.IdStep == kvDocSubmitted.IdKeyValue)).AsNoTracking()
                        .Include(x => x.StartedProcedure).ThenInclude(x => x.ProcedureDocuments.Where(y => y.IdDocumentType == kvDocTypeApplicationCIPO.IdKeyValue)).AsNoTracking()
                        .FirstOrDefaultAsync();
                }

                var personsFromDb = await this.repository.AllReadonly<Person>().ToListAsync();
                foreach (var person in persons)
                {
                    //if (personsFromDb.Any(x => x.Email == person.Email))
                    //{
                    //    continue;
                    //}

                    if (model.IdImportType == kvCIPOTypeValue!.IdKeyValue || model.IdImportType == kvCPOTypeValue!.IdKeyValue)
                    {
                        int idCandidateProvider = 0;

                        var candidateProviderFromDb = await this.repository.AllReadonly<CandidateProvider>(x => x.PoviderBulstat == person.Bulstat && x.LicenceNumber == person.LicenseNumber).FirstOrDefaultAsync();
                        if (candidateProviderFromDb is null)
                        {
                            if (string.IsNullOrEmpty(person.Bulstat))
                            {
                                var random = new Random();
                                var bulstat = random.Next(100000001, 999999999).ToString();
                                while (await this.repository.AllReadonly<CandidateProvider>(x => x.PoviderBulstat == bulstat).FirstOrDefaultAsync() != null)
                                {
                                    bulstat = random.Next(100000001, 999999999).ToString();
                                }

                                var peopleFromSameCPO = persons.Where(x => x.LicenseNumber == person.LicenseNumber).ToList();
                                peopleFromSameCPO.ForEach(x => x.Bulstat = bulstat);
                            }

                            var candidateProviderForDb = new CandidateProvider()
                            {
                                ProviderOwner = person.ProviderOwner,
                                PoviderBulstat = person.Bulstat,
                                ManagerName = $"{person.FirstName} {person.FamilyName}",
                                IdProviderRegistration = candidateProvider.IdProviderRegistration,
                                IdProviderOwnership = candidateProvider.IdProviderOwnership,
                                IdProviderStatus = candidateProvider.IdProviderStatus,
                                IdLocation = candidateProvider.IdLocation,
                                ProviderAddress = candidateProvider.ProviderAddress,
                                ZipCode = candidateProvider.ZipCode,
                                UploadedFileName = candidateProvider.UploadedFileName,
                                IdTypeLicense = candidateProvider.IdTypeLicense,
                                ApplicationNumber = candidateProvider.ApplicationNumber,
                                ApplicationDate = candidateProvider.ApplicationDate,
                                ProviderPhone = person.Phone,
                                ProviderFax = candidateProvider.ProviderFax,
                                ProviderWeb = candidateProvider.ProviderWeb,
                                ProviderEmail = person.Email,
                                AdditionalInfo = candidateProvider.AdditionalInfo,
                                OnlineTrainingInfo = candidateProvider.OnlineTrainingInfo,
                                PersonNameCorrespondence = $"{person.FirstName} {person.FamilyName}",
                                IdLocationCorrespondence = candidateProvider.IdLocationCorrespondence,
                                ProviderAddressCorrespondence = candidateProvider.ProviderAddressCorrespondence,
                                ZipCodeCorrespondence = candidateProvider.ZipCodeCorrespondence,
                                ProviderPhoneCorrespondence = person.Phone,
                                ProviderFaxCorrespondence = candidateProvider.ProviderFaxCorrespondence,
                                ProviderEmailCorrespondence = person.Email,
                                DateConfirmEMail = candidateProvider.DateConfirmEMail,
                                DateConfirmRequestNAPOO = candidateProvider.DateConfirmRequestNAPOO,
                                DateRequest = candidateProvider.DateRequest,
                                DueDateRequest = candidateProvider.DueDateRequest,
                                IdApplicationStatus = kvApplicationAccepted.IdKeyValue,
                                IdTypeApplication = candidateProvider.IdTypeApplication,
                                Indent = candidateProvider.Indent,
                                IdApplicationFiling = candidateProvider.IdApplicationFiling,
                                IdReceiveLicense = candidateProvider.IdReceiveLicense,
                                LicenceNumber = person.LicenseNumber,
                                LicenceDate = candidateProvider.LicenceDate,
                                IdRegistrationApplicationStatus = null,
                                Title = candidateProvider.Title,
                                IsActive = true,
                                UIN = candidateProvider.UIN,
                                IdLicenceStatus = candidateProvider.IdLicenceStatus,
                                IdRegionCorrespondence = candidateProvider.IdRegionCorrespondence,
                                DirectorFirstName = person.FirstName,
                                DirectorSecondName = person.FamilyName,
                                DirectorFamilyName = person.FamilyName,
                                IdRegionAdmin = candidateProvider.IdRegionAdmin,
                                Archive = candidateProvider.Archive,
                                AdditionalDocumentRequested = candidateProvider.AdditionalDocumentRequested,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProvider>(candidateProviderForDb);
                            await this.repository.SaveChangesAsync(false);

                            providersCount++;

                            idCandidateProvider = candidateProviderForDb.IdCandidate_Provider;

                            // инсъртва всички навързани таблици за специалности
                            foreach (var providerSpeciality in candidateProvider.CandidateProviderSpecialities)
                            {
                                var candidateProviderSpecialityForDb = new CandidateProviderSpeciality()
                                {
                                    IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                    IdSpeciality = providerSpeciality.IdSpeciality,
                                    IdFormEducation = providerSpeciality.IdFormEducation,
                                    IdFrameworkProgram = providerSpeciality.IdFrameworkProgram,
                                    LicenceData = providerSpeciality.LicenceData,
                                    LicenceProtNo = providerSpeciality.LicenceProtNo,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderSpeciality>(candidateProviderSpecialityForDb);
                                await this.repository.SaveChangesAsync(false);

                                foreach (var modification in providerSpeciality.CandidateCurriculumModifications)
                                {
                                    var modificationForDb = new CandidateCurriculumModification()
                                    {
                                        IdCandidateProviderSpeciality = candidateProviderSpecialityForDb.IdCandidateProviderSpeciality,
                                        IdModificationReason = modification.IdModificationReason,
                                        IdModificationStatus = modification.IdModificationStatus,
                                        ValidFromDate = modification.ValidFromDate,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateCurriculumModification>(modificationForDb);
                                    await this.repository.SaveChangesAsync(false);

                                    foreach (var curriculum in modification.CandidateCurriculums)
                                    {
                                        var curriculumForDb = new CandidateCurriculum()
                                        {
                                            IdCandidateProviderSpeciality = curriculum.IdCandidateProviderSpeciality,
                                            IdProfessionalTraining = curriculum.IdProfessionalTraining,
                                            Subject = curriculum.Subject,
                                            Topic = curriculum.Topic,
                                            Theory = curriculum.Theory,
                                            Practice = curriculum.Practice,
                                            IdCandidateCurriculumModification = modificationForDb.IdCandidateCurriculumModification,
                                            IdCreateUser = userFromSystem.IdUser,
                                            IdModifyUser = userFromSystem.IdUser,
                                            CreationDate = DateTime.Now,
                                            ModifyDate = DateTime.Now
                                        };

                                        await this.repository.AddAsync<CandidateCurriculum>(curriculumForDb);
                                        await this.repository.SaveChangesAsync(false);

                                        foreach (var eru in curriculum.CandidateCurriculumERUs)
                                        {
                                            var eruForDb = new CandidateCurriculumERU()
                                            {
                                                IdCandidateCurriculum = curriculumForDb.IdCandidateCurriculum,
                                                IdERU = eru.IdERU
                                            };

                                            await this.repository.AddAsync<CandidateCurriculumERU>(eruForDb);
                                            await this.repository.SaveChangesAsync(false);
                                        }
                                    }
                                }
                            }

                            // инсъртва всички навързани таблици за преподаватели/консултанти
                            foreach (var trainer in candidateProvider.CandidateProviderTrainers)
                            {
                                var trainerForDb = new CandidateProviderTrainer()
                                {
                                    IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                    FirstName = trainer.FirstName,
                                    SecondName = trainer.SecondName,
                                    FamilyName = trainer.FamilyName,
                                    IdIndentType = trainer.IdIndentType,
                                    Indent = trainer.Indent,
                                    BirthDate = trainer.BirthDate,
                                    IdSex = trainer.IdSex,
                                    IdNationality = trainer.IdNationality,
                                    Email = trainer.Email,
                                    IdEducation = trainer.IdEducation,
                                    EducationSpecialityNotes = trainer.EducationSpecialityNotes,
                                    EducationCertificateNotes = trainer.EducationCertificateNotes,
                                    EducationAcademicNotes = trainer.EducationAcademicNotes,
                                    IsAndragog = trainer.IsAndragog,
                                    IdContractType = trainer.IdContractType,
                                    ContractDate = trainer.ContractDate,
                                    IdStatus = trainer.IdStatus,
                                    DiplomaNumber = trainer.DiplomaNumber,
                                    InactiveDate = trainer.InactiveDate,
                                    ProfessionalQualificationCertificate = trainer.ProfessionalQualificationCertificate,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainer>(trainerForDb);
                                await this.repository.SaveChangesAsync(false);

                                foreach (var trainerSpeciality in trainer.CandidateProviderTrainerSpecialities)
                                {
                                    var trainerSpecialityForDb = new CandidateProviderTrainerSpeciality()
                                    {
                                        IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                        IdSpeciality = trainerSpeciality.IdSpeciality,
                                        IdUsage = trainerSpeciality.IdUsage,
                                        IdComplianceDOC = trainerSpeciality.IdComplianceDOC,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateProviderTrainerSpeciality>(trainerSpecialityForDb);
                                    await this.repository.SaveChangesAsync(false);
                                }

                                foreach (var trainerQualification in trainer.CandidateProviderTrainerQualifications)
                                {
                                    var trainerQualificationForDb = new CandidateProviderTrainerQualification()
                                    {
                                        IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                        QualificationName = trainerQualification.QualificationName,
                                        IdQualificationType = trainerQualification.IdQualificationType,
                                        IdProfession = trainerQualification.IdProfession,
                                        IdTrainingQualificationType = trainerQualification.IdTrainingQualificationType,
                                        QualificationDuration = trainerQualification.QualificationDuration,
                                        TrainingFrom = trainerQualification.TrainingFrom,
                                        TrainingTo = trainerQualification.TrainingTo,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateProviderTrainerQualification>(trainerQualificationForDb);
                                    await this.repository.SaveChangesAsync(false);
                                }

                                foreach (var trainerProfile in trainer.CandidateProviderTrainerProfiles)
                                {
                                    var trainerProfileForDb = new CandidateProviderTrainerProfile()
                                    {
                                        IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                        IdProfessionalDirection = trainerProfile.IdProfessionalDirection,
                                        IsProfessionalDirectionQualified = trainerProfile.IsProfessionalDirectionQualified,
                                        IsTheory = trainerProfile.IsTheory,
                                        IsPractice = trainerProfile.IsPractice,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateProviderTrainerProfile>(trainerProfileForDb);
                                    await this.repository.SaveChangesAsync(false);
                                }

                                foreach (var trainerDoc in trainer.CandidateProviderTrainerDocuments)
                                {
                                    var trainerDocForDb = new CandidateProviderTrainerDocument()
                                    {
                                        IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                        IdDocumentType = trainerDoc.IdDocumentType,
                                        DocumentTitle = trainerDoc.DocumentTitle,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateProviderTrainerDocument>(trainerDocForDb);
                                    await this.repository.SaveChangesAsync(false);
                                }
                            }

                            // инсъртва всички навързани таблици за МТБ
                            foreach (var premises in candidateProvider.CandidateProviderPremises)
                            {
                                var premisesForDb = new CandidateProviderPremises()
                                {
                                    IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                    PremisesName = premises.PremisesName,
                                    PremisesNote = premises.PremisesNote,
                                    IdLocation = premises.IdLocation,
                                    ProviderAddress = premises.ProviderAddress,
                                    ZipCode = premises.ZipCode,
                                    Phone = premises.Phone,
                                    IdOwnership = premises.IdOwnership,
                                    IdStatus = premises.IdStatus,
                                    InactiveDate = premises.InactiveDate,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderPremises>(premisesForDb);
                                await this.repository.SaveChangesAsync(false);

                                foreach (var premisesSpec in premises.CandidateProviderPremisesSpecialities)
                                {
                                    var premisesSpecialityForDb = new CandidateProviderPremisesSpeciality()
                                    {
                                        IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                        IdSpeciality = premisesSpec.IdSpeciality,
                                        IdUsage = premisesSpec.IdUsage,
                                        IdComplianceDOC = premisesSpec.IdComplianceDOC,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateProviderPremisesSpeciality>(premisesSpecialityForDb);
                                    await this.repository.SaveChangesAsync(false);
                                }

                                foreach (var premisesRoom in premises.CandidateProviderPremisesRooms)
                                {
                                    var premisesRoomForDb = new CandidateProviderPremisesRoom()
                                    {
                                        IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                        PremisesRoomName = premisesRoom.PremisesRoomName,
                                        Equipment = premisesRoom.Equipment,
                                        IdUsage = premisesRoom.IdUsage,
                                        IdPremisesType = premisesRoom.IdPremisesType,
                                        Area = premisesRoom.Area,
                                        Workplace = premisesRoom.Workplace,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateProviderPremisesRoom>(premisesRoomForDb);
                                    await this.repository.SaveChangesAsync(false);
                                }

                                foreach (var premisesDoc in premises.CandidateProviderPremisesDocuments)
                                {
                                    var premisesDocForDb = new CandidateProviderPremisesDocument()
                                    {
                                        IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                        IdDocumentType = premisesDoc.IdDocumentType,
                                        DocumentTitle = premisesDoc.DocumentTitle,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateProviderPremisesDocument>(premisesDocForDb);
                                    await this.repository.SaveChangesAsync(false);
                                }
                            }

                            // инсъртва всички навързани таблици за документи на CandidateProvider
                            foreach (var doc in candidateProvider.CandidateProviderDocuments)
                            {
                                var docForDb = new CandidateProviderDocument()
                                {
                                    IdCandidateProvider = candidateProviderForDb.IdCandidate_Provider,
                                    IdDocumentType = doc.IdDocumentType,
                                    DocumentTitle = doc.DocumentTitle,
                                    IsAdditionalDocument = doc.IsAdditionalDocument,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderDocument>(docForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            // инсъртва всички навързани таблици за консултации на CandidateProvider
                            foreach (var consulting in candidateProvider.CandidateProviderConsultings)
                            {
                                var consultingForDb = new CandidateProviderConsulting()
                                {
                                    IdCandidateProvider = candidateProviderForDb.IdCandidate_Provider,
                                    IdConsultingType = consulting.IdConsultingType,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderConsulting>(consultingForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            // инсъртва всички навързани таблици за структура на CPO
                            foreach (var structure in candidateProvider.CandidateProviderCPOStructureAndActivities)
                            {
                                var structureForDb = new CandidateProviderCPOStructureActivity()
                                {
                                    IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                    Management = structure.Management,
                                    OrganisationTrainingProcess = structure.OrganisationTrainingProcess,
                                    CompletionCertificationTraining = structure.CompletionCertificationTraining,
                                    InternalQualitySystem = structure.InternalQualitySystem,
                                    InformationProvisionMaintenance = structure.InformationProvisionMaintenance,
                                    TrainingDocumentation = structure.TrainingDocumentation,
                                    TeachersSelection = structure.TeachersSelection,
                                    MTBDescription = structure.MTBDescription,
                                    DataMaintenance = structure.DataMaintenance,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderCPOStructureActivity>(structureForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            // инсъртва всички навързани таблици за структура на CPO
                            foreach (var structure in candidateProvider.CandidateProviderCIPOStructureAndActivities)
                            {
                                var structureForDb = new CandidateProviderCIPOStructureActivity()
                                {
                                    IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                    Management = structure.Management,
                                    OrganisationInformationProcess = structure.OrganisationInformationProcess,
                                    InternalQualitySystem = structure.InternalQualitySystem,
                                    InformationProvisionMaintenance = structure.InformationProvisionMaintenance,
                                    TrainingDocumentation = structure.TrainingDocumentation,
                                    ConsultantsSelection = structure.ConsultantsSelection,
                                    MTBDescription = structure.MTBDescription,
                                    DataMaintenance = structure.DataMaintenance,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderCIPOStructureActivity>(structureForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            // инсъртва всички навързани таблици за заявки за документация
                            foreach (var request in candidateProvider!.ProviderRequestDocuments)
                            {
                                var requestForDb = new ProviderRequestDocument()
                                {
                                    IdCandidateProvider = idCandidateProvider,
                                    IdNAPOORequestDoc = request.IdNAPOORequestDoc,
                                    CurrentYear = request.CurrentYear,
                                    RequestDate = request.RequestDate,
                                    Position = request.Position,
                                    Name = request.Name,
                                    Address = request.Address,
                                    Telephone = request.Telephone,
                                    IsSent = request.IsSent,
                                    RequestNumber = await this.GetSequenceNextValue("RequestDocument"),
                                    IdLocationCorrespondence = request.IdLocationCorrespondence,
                                    UploadedFileName = string.Empty,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<ProviderRequestDocument>(requestForDb);
                                await this.repository.SaveChangesAsync(false);

                                foreach (var status in request.RequestDocumentStatuses)
                                {
                                    var statusForDb = new RequestDocumentStatus()
                                    {
                                        IdCandidateProvider = requestForDb.IdCandidateProvider,
                                        IdProviderRequestDocument = requestForDb.IdProviderRequestDocument,
                                        IdStatus = status.IdStatus,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<RequestDocumentStatus>(statusForDb);
                                    await this.repository.SaveChangesAsync(false);
                                }

                                foreach (var docType in request.RequestDocumentTypes)
                                {
                                    var docTypeForDb = new RequestDocumentType()
                                    {
                                        IdCandidateProvider = requestForDb.IdCandidateProvider,
                                        IdProviderRequestDocument = requestForDb.IdProviderRequestDocument,
                                        IdTypeOfRequestedDocument = docType.IdTypeOfRequestedDocument,
                                        DocumentCount = docType.DocumentCount,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<RequestDocumentType>(docTypeForDb);
                                    await this.repository.SaveChangesAsync(false);
                                }
                            }
                        }
                        else
                        {
                            idCandidateProvider = candidateProviderFromDb.IdCandidate_Provider;
                        }

                        var personForDb = new Person()
                        {
                            FirstName = person.FirstName,
                            FamilyName = person.FamilyName,
                            Phone = person.Phone,
                            Email = person.Email,
                            TaxOffice = string.Empty,
                            IsContractRegisterDocu = false,
                            IsSignContract = false,
                            Position = candidateProvider!.CandidateProviderSpecialities.Any() ? "Представител на ЦПО" : "Представител на ЦИПО",
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<Person>(personForDb);
                        await this.repository.SaveChangesAsync(false);

                        var applicationUser = new ApplicationUserVM()
                        {
                            IdPerson = personForDb.IdPerson,
                            Email = personForDb.Email!,
                            IdCandidateProvider = idCandidateProvider,
                            EIK = person.Bulstat,
                            IdUserStatus = kvUserStatusActive.IdKeyValue,
                            FirstName = personForDb.FirstName,
                            FamilyName = personForDb.FamilyName,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        var user = this.CreateUser();
                        user.IdPerson = applicationUser.IdPerson;
                        user.Email = applicationUser.Email;
                        applicationUser.Password = this.GenerateRandomPassword();
                        user.UserName = await this.GenerateUserName(applicationUser);
                        user.IdUser = (int)(await this.GetSequenceNextValue("APPLICATION_USER_ID"));
                        user.IdUserStatus = applicationUser.IdUserStatus;
                        user.CreationDate = DateTime.Now;
                        user.ModifyDate = DateTime.Now;
                        user.IdCreateUser = UserProps.UserId;
                        user.IdModifyUser = UserProps.UserId;

                        var result = await this.userManager.CreateAsync(user, applicationUser.Password);
                        applicationUser.UserName = user.UserName;
                        if (result.Succeeded)
                        {
                            if (applicationUser.IdCandidateProvider != 0)
                            {
                                await this.userManager.AddClaimAsync(user,
                                    new System.Security.Claims.Claim(
                                        GlobalConstants.ID_CANDIDATE_PROVIDER,
                                        applicationUser.IdCandidateProvider.ToString()));

                                await this.userManager.AddClaimAsync(user,
                                    new System.Security.Claims.Claim(
                                        GlobalConstants.ID_USER,
                                user.IdUser.ToString()));

                                await this.userManager.AddClaimAsync(user,
                                    new System.Security.Claims.Claim(
                                        GlobalConstants.ID_PERSON,
                                        applicationUser.IdPerson.ToString()));

                                await this.userManager.AddClaimAsync(user,
                                  new System.Security.Claims.Claim(
                                      GlobalConstants.PERSON_FULLNAME,
                                     applicationUser.FullName));

                                foreach (var role in model.Roles)
                                {
                                    await this.userManager.AddToRoleAsync(user, role.Name);
                                }
                            }

                            this._logger.LogInformation($"User created an account with UserName:{user.UserName}.");
                        }

                        var candidateProviderPersonForDb = new CandidateProviderPerson()
                        {
                            IdPerson = personForDb.IdPerson,
                            IdCandidate_Provider = idCandidateProvider,
                            IsAdministrator = true,
                            IsAllowedForNotification = true
                        };

                        await this.repository.AddAsync<CandidateProviderPerson>(candidateProviderPersonForDb);
                        await this.repository.SaveChangesAsync(false);

                        usersCount++;

                        string emailText = string.Empty;
                        if (model.AllowSentEmails)
                        {
                            emailText = await this.mailService.SendEmailNewRegistrationFromImport(new ResultContext<ApplicationUserVM>() { ResultContextObject = applicationUser });
                        }

                        importInfoList.Add($"Потребител: {user.UserName} , парола: {applicationUser.Password} , имейл: {applicationUser.Email} към център {person.ProviderOwner} , № на лицензия: {person.LicenseNumber}->{emailText}");
                    }
                    else if (model.IdImportType == kvExternalExpertTypeValue?.IdKeyValue)
                    {
                        var personForDb = new Person()
                        {
                            FirstName = person.FirstName,
                            SecondName = person.SecondName,
                            FamilyName = person.FamilyName,
                            Phone = person.Phone,
                            Email = person.Email,
                            TaxOffice = string.Empty,
                            IsContractRegisterDocu = false,
                            IsSignContract = false,
                            Position = person.Position,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<Person>(personForDb);
                        await this.repository.SaveChangesAsync(false);

                        var applicationUser = new ApplicationUserVM()
                        {
                            IdPerson = personForDb.IdPerson,
                            Email = personForDb.Email!,
                            EIK = string.Empty,
                            IdUserStatus = kvUserStatusActive.IdKeyValue,
                            FirstName = personForDb.FirstName,
                            FamilyName = personForDb.FamilyName,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        var user = this.CreateUser();
                        user.IdPerson = applicationUser.IdPerson;
                        user.Email = applicationUser.Email;
                        applicationUser.Password = this.GenerateRandomPassword();
                        user.UserName = await this.GenerateUserName(applicationUser);
                        user.IdUser = (int)(await this.GetSequenceNextValue("APPLICATION_USER_ID"));
                        user.IdUserStatus = applicationUser.IdUserStatus;
                        user.CreationDate = DateTime.Now;
                        user.ModifyDate = DateTime.Now;
                        user.IdCreateUser = UserProps.UserId;
                        user.IdModifyUser = UserProps.UserId;

                        var result = await this.userManager.CreateAsync(user, applicationUser.Password);
                        applicationUser.UserName = user.UserName;
                        if (result.Succeeded)
                        {
                            await this.userManager.AddClaimAsync(user,
                                new System.Security.Claims.Claim(
                                    GlobalConstants.ID_USER,
                            user.IdUser.ToString()));

                            await this.userManager.AddClaimAsync(user,
                                new System.Security.Claims.Claim(
                                    GlobalConstants.ID_PERSON,
                                    applicationUser.IdPerson.ToString()));

                            await this.userManager.AddClaimAsync(user,
                              new System.Security.Claims.Claim(
                                  GlobalConstants.PERSON_FULLNAME,
                                 applicationUser.FullName));

                            foreach (var role in model.Roles)
                            {
                                await this.userManager.AddToRoleAsync(user, role.Name);
                            }

                            this._logger.LogInformation($"User created an account with UserName:{user.UserName}.");
                        }

                        if (string.IsNullOrEmpty(person.Bulstat))
                        {
                            var random = new Random();
                            var bulstat = random.Next(100000001, 999999999).ToString();
                            while (await this.repository.AllReadonly<CandidateProvider>(x => x.PoviderBulstat == bulstat).FirstOrDefaultAsync() != null)
                            {
                                bulstat = random.Next(100000001, 999999999).ToString();
                            }

                            person.Bulstat = bulstat;
                        }

                        var startedProcedureForDb = new StartedProcedure()
                        {
                            TS = candidateProvider!.StartedProcedure.TS,
                            NapooReportDeadline = candidateProvider.StartedProcedure.NapooReportDeadline,
                            MeetingDate = candidateProvider.StartedProcedure.MeetingDate,
                            MeetingHour = candidateProvider.StartedProcedure.MeetingHour,
                            ExpertReportDeadline = DateTime.Now.AddDays(30),
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<StartedProcedure>(startedProcedureForDb);
                        await this.repository.SaveChangesAsync(false);

                        // инсъртва всички навързани таблици за прогрес на стартирана процедура
                        foreach (var progress in candidateProvider.StartedProcedure.StartedProcedureProgresses)
                        {
                            var progressForDb = new StartedProcedureProgress()
                            {
                                IdStartedProcedure = startedProcedureForDb.IdStartedProcedure,
                                IdStep = progress.IdStep,
                                StepDate = DateTime.Now,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<StartedProcedureProgress>(progressForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        // инсъртва всички навързани таблици за документи по процедура
                        foreach (var doc in candidateProvider.StartedProcedure.ProcedureDocuments)
                        {
                            var docForDb = new ProcedureDocument()
                            {
                                IdStartedProcedure = startedProcedureForDb.IdStartedProcedure,
                                IsValid = doc.IsValid,
                                IdDocumentType = doc.IdDocumentType,
                                DateAttachment = doc.DateAttachment,
                                MimeType = doc.MimeType,
                                Extension = doc.Extension,
                                UploadedFileName = doc.UploadedFileName,
                                DS_ID = doc.DS_ID,
                                DS_DATE = doc.DS_DATE,
                                DS_OFFICIAL_ID = doc.DS_OFFICIAL_ID,
                                DS_OFFICIAL_DATE = doc.DS_OFFICIAL_DATE,
                                DS_PREP = doc.DS_PREP,
                                DS_LINK = doc.DS_LINK,
                                DS_GUID = doc.DS_GUID,
                                DS_OFFICIAL_GUID = doc.DS_OFFICIAL_GUID,
                                DS_DocNumber = doc.DS_DocNumber,
                                DS_OFFICIAL_DocNumber = doc.DS_OFFICIAL_DocNumber,
                                IsFromDS = doc.IsFromDS,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<ProcedureDocument>(docForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        var kvApplicationFiledIn = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "RequestedByCPOOrCIPO");
                        var candidateProviderForDb = new CandidateProvider()
                        {
                            ProviderOwner = $"{candidateProvider!.ProviderOwner} {user.IdUser}",
                            ProviderName = $"{candidateProvider!.ProviderOwner} {user.IdUser}",
                            PoviderBulstat = person.Bulstat,
                            ManagerName = candidateProvider.ManagerName,
                            AttorneyName = candidateProvider.AttorneyName,
                            IdProviderRegistration = candidateProvider.IdProviderRegistration,
                            IdProviderOwnership = candidateProvider.IdProviderOwnership,
                            IdProviderStatus = candidateProvider.IdProviderStatus,
                            IdLocation = candidateProvider.IdLocation,
                            ProviderAddress = candidateProvider.ProviderAddress,
                            ZipCode = candidateProvider.ZipCode,
                            UploadedFileName = candidateProvider.UploadedFileName,
                            IdTypeLicense = candidateProvider.IdTypeLicense,
                            ApplicationNumber = candidateProvider.ApplicationNumber,
                            ApplicationDate = candidateProvider.ApplicationDate,
                            ProviderPhone = candidateProvider.ProviderPhone,
                            ProviderFax = candidateProvider.ProviderFax,
                            ProviderWeb = candidateProvider.ProviderWeb,
                            ProviderEmail = candidateProvider.ProviderEmail,
                            AdditionalInfo = candidateProvider.AdditionalInfo,
                            OnlineTrainingInfo = candidateProvider.OnlineTrainingInfo,
                            PersonNameCorrespondence = candidateProvider.PersonNameCorrespondence,
                            IdLocationCorrespondence = candidateProvider.IdLocationCorrespondence,
                            ProviderAddressCorrespondence = candidateProvider.ProviderAddressCorrespondence,
                            ZipCodeCorrespondence = candidateProvider.ZipCodeCorrespondence,
                            ProviderPhoneCorrespondence = candidateProvider.ProviderPhoneCorrespondence,
                            ProviderFaxCorrespondence = candidateProvider.ProviderFaxCorrespondence,
                            ProviderEmailCorrespondence = candidateProvider.ProviderEmailCorrespondence,
                            DateConfirmEMail = candidateProvider.DateConfirmEMail,
                            DateConfirmRequestNAPOO = candidateProvider.DateConfirmRequestNAPOO,
                            DateRequest = candidateProvider.DateRequest,
                            DueDateRequest = candidateProvider.DueDateRequest,
                            IdApplicationStatus = kvApplicationFiledIn.IdKeyValue,
                            IdTypeApplication = candidateProvider.IdTypeApplication,
                            Indent = candidateProvider.Indent,
                            IdApplicationFiling = candidateProvider.IdApplicationFiling,
                            IdReceiveLicense = candidateProvider.IdReceiveLicense,
                            LicenceNumber = null,
                            LicenceDate = null,
                            IdRegistrationApplicationStatus = null,
                            Title = candidateProvider.Title,
                            IsActive = true,
                            UIN = candidateProvider.UIN,
                            IdLicenceStatus = null,
                            IdRegionCorrespondence = candidateProvider.IdRegionCorrespondence,
                            DirectorFirstName = candidateProvider.DirectorFirstName,
                            DirectorSecondName = candidateProvider.DirectorSecondName,
                            DirectorFamilyName = candidateProvider.DirectorFamilyName,
                            IdRegionAdmin = candidateProvider.IdRegionAdmin,
                            Archive = candidateProvider.Archive,
                            AdditionalDocumentRequested = candidateProvider.AdditionalDocumentRequested,
                            IdStartedProcedure = startedProcedureForDb.IdStartedProcedure,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<CandidateProvider>(candidateProviderForDb);
                        await this.repository.SaveChangesAsync(false);

                        providersCount++;

                        // инсъртва всички навързани таблици за специалности
                        foreach (var providerSpeciality in candidateProvider.CandidateProviderSpecialities)
                        {
                            var candidateProviderSpecialityForDb = new CandidateProviderSpeciality()
                            {
                                IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                IdSpeciality = providerSpeciality.IdSpeciality,
                                IdFormEducation = providerSpeciality.IdFormEducation,
                                IdFrameworkProgram = providerSpeciality.IdFrameworkProgram,
                                LicenceData = providerSpeciality.LicenceData,
                                LicenceProtNo = providerSpeciality.LicenceProtNo,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderSpeciality>(candidateProviderSpecialityForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var modification in providerSpeciality.CandidateCurriculumModifications)
                            {
                                var modificationForDb = new CandidateCurriculumModification()
                                {
                                    IdCandidateProviderSpeciality = candidateProviderSpecialityForDb.IdCandidateProviderSpeciality,
                                    IdModificationReason = modification.IdModificationReason,
                                    IdModificationStatus = modification.IdModificationStatus,
                                    ValidFromDate = modification.ValidFromDate,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateCurriculumModification>(modificationForDb);
                                await this.repository.SaveChangesAsync(false);

                                foreach (var curriculum in modification.CandidateCurriculums)
                                {
                                    var curriculumForDb = new CandidateCurriculum()
                                    {
                                        IdCandidateProviderSpeciality = curriculum.IdCandidateProviderSpeciality,
                                        IdProfessionalTraining = curriculum.IdProfessionalTraining,
                                        Subject = curriculum.Subject,
                                        Topic = curriculum.Topic,
                                        Theory = curriculum.Theory,
                                        Practice = curriculum.Practice,
                                        IdCandidateCurriculumModification = modificationForDb.IdCandidateCurriculumModification,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateCurriculum>(curriculumForDb);
                                    await this.repository.SaveChangesAsync(false);

                                    foreach (var eru in curriculum.CandidateCurriculumERUs)
                                    {
                                        var eruForDb = new CandidateCurriculumERU()
                                        {
                                            IdCandidateCurriculum = curriculumForDb.IdCandidateCurriculum,
                                            IdERU = eru.IdERU
                                        };

                                        await this.repository.AddAsync<CandidateCurriculumERU>(eruForDb);
                                        await this.repository.SaveChangesAsync(false);
                                    }
                                }
                            }
                        }

                        // инсъртва всички навързани таблици за преподаватели/консултанти
                        foreach (var trainer in candidateProvider.CandidateProviderTrainers)
                        {
                            var trainerForDb = new CandidateProviderTrainer()
                            {
                                IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                FirstName = trainer.FirstName,
                                SecondName = trainer.SecondName,
                                FamilyName = trainer.FamilyName,
                                IdIndentType = trainer.IdIndentType,
                                Indent = trainer.Indent,
                                BirthDate = trainer.BirthDate,
                                IdSex = trainer.IdSex,
                                IdNationality = trainer.IdNationality,
                                Email = trainer.Email,
                                IdEducation = trainer.IdEducation,
                                EducationSpecialityNotes = trainer.EducationSpecialityNotes,
                                EducationCertificateNotes = trainer.EducationCertificateNotes,
                                EducationAcademicNotes = trainer.EducationAcademicNotes,
                                IsAndragog = trainer.IsAndragog,
                                IdContractType = trainer.IdContractType,
                                ContractDate = trainer.ContractDate,
                                IdStatus = trainer.IdStatus,
                                DiplomaNumber = trainer.DiplomaNumber,
                                InactiveDate = trainer.InactiveDate,
                                ProfessionalQualificationCertificate = trainer.ProfessionalQualificationCertificate,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderTrainer>(trainerForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var trainerSpeciality in trainer.CandidateProviderTrainerSpecialities)
                            {
                                var trainerSpecialityForDb = new CandidateProviderTrainerSpeciality()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    IdSpeciality = trainerSpeciality.IdSpeciality,
                                    IdUsage = trainerSpeciality.IdUsage,
                                    IdComplianceDOC = trainerSpeciality.IdComplianceDOC,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerSpeciality>(trainerSpecialityForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var trainerQualification in trainer.CandidateProviderTrainerQualifications)
                            {
                                var trainerQualificationForDb = new CandidateProviderTrainerQualification()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    QualificationName = trainerQualification.QualificationName,
                                    IdQualificationType = trainerQualification.IdQualificationType,
                                    IdProfession = trainerQualification.IdProfession,
                                    IdTrainingQualificationType = trainerQualification.IdTrainingQualificationType,
                                    QualificationDuration = trainerQualification.QualificationDuration,
                                    TrainingFrom = trainerQualification.TrainingFrom,
                                    TrainingTo = trainerQualification.TrainingTo,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerQualification>(trainerQualificationForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var trainerProfile in trainer.CandidateProviderTrainerProfiles)
                            {
                                var trainerProfileForDb = new CandidateProviderTrainerProfile()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    IdProfessionalDirection = trainerProfile.IdProfessionalDirection,
                                    IsProfessionalDirectionQualified = trainerProfile.IsProfessionalDirectionQualified,
                                    IsTheory = trainerProfile.IsTheory,
                                    IsPractice = trainerProfile.IsPractice,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerProfile>(trainerProfileForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var trainerDoc in trainer.CandidateProviderTrainerDocuments)
                            {
                                var trainerDocForDb = new CandidateProviderTrainerDocument()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    IdDocumentType = trainerDoc.IdDocumentType,
                                    DocumentTitle = trainerDoc.DocumentTitle,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerDocument>(trainerDocForDb);
                                await this.repository.SaveChangesAsync(false);
                            }
                        }

                        // инсъртва всички навързани таблици за МТБ
                        foreach (var premises in candidateProvider.CandidateProviderPremises)
                        {
                            var premisesForDb = new CandidateProviderPremises()
                            {
                                IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                PremisesName = premises.PremisesName,
                                PremisesNote = premises.PremisesNote,
                                IdLocation = premises.IdLocation,
                                ProviderAddress = premises.ProviderAddress,
                                ZipCode = premises.ZipCode,
                                Phone = premises.Phone,
                                IdOwnership = premises.IdOwnership,
                                IdStatus = premises.IdStatus,
                                InactiveDate = premises.InactiveDate,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderPremises>(premisesForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var premisesSpec in premises.CandidateProviderPremisesSpecialities)
                            {
                                var premisesSpecialityForDb = new CandidateProviderPremisesSpeciality()
                                {
                                    IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                    IdSpeciality = premisesSpec.IdSpeciality,
                                    IdUsage = premisesSpec.IdUsage,
                                    IdComplianceDOC = premisesSpec.IdComplianceDOC,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderPremisesSpeciality>(premisesSpecialityForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var premisesRoom in premises.CandidateProviderPremisesRooms)
                            {
                                var premisesRoomForDb = new CandidateProviderPremisesRoom()
                                {
                                    IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                    PremisesRoomName = premisesRoom.PremisesRoomName,
                                    Equipment = premisesRoom.Equipment,
                                    IdUsage = premisesRoom.IdUsage,
                                    IdPremisesType = premisesRoom.IdPremisesType,
                                    Area = premisesRoom.Area,
                                    Workplace = premisesRoom.Workplace,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderPremisesRoom>(premisesRoomForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var premisesDoc in premises.CandidateProviderPremisesDocuments)
                            {
                                var premisesDocForDb = new CandidateProviderPremisesDocument()
                                {
                                    IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                    IdDocumentType = premisesDoc.IdDocumentType,
                                    DocumentTitle = premisesDoc.DocumentTitle,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderPremisesDocument>(premisesDocForDb);
                                await this.repository.SaveChangesAsync(false);
                            }
                        }

                        // инсъртва всички навързани таблици за документи на CandidateProvider
                        foreach (var doc in candidateProvider.CandidateProviderDocuments)
                        {
                            var docForDb = new CandidateProviderDocument()
                            {
                                IdCandidateProvider = candidateProviderForDb.IdCandidate_Provider,
                                IdDocumentType = doc.IdDocumentType,
                                DocumentTitle = doc.DocumentTitle,
                                IsAdditionalDocument = doc.IsAdditionalDocument,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderDocument>(docForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        // инсъртва всички навързани таблици за структура на CPO
                        foreach (var structure in candidateProvider.CandidateProviderCPOStructureAndActivities)
                        {
                            var structureForDb = new CandidateProviderCPOStructureActivity()
                            {
                                IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                Management = structure.Management,
                                OrganisationTrainingProcess = structure.OrganisationTrainingProcess,
                                CompletionCertificationTraining = structure.CompletionCertificationTraining,
                                InternalQualitySystem = structure.InternalQualitySystem,
                                InformationProvisionMaintenance = structure.InformationProvisionMaintenance,
                                TrainingDocumentation = structure.TrainingDocumentation,
                                TeachersSelection = structure.TeachersSelection,
                                MTBDescription = structure.MTBDescription,
                                DataMaintenance = structure.DataMaintenance,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderCPOStructureActivity>(structureForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        usersCount++;

                        string emailText = string.Empty;
                        if (model.AllowSentEmails)
                        {
                            emailText = await this.mailService.SendEmailNewRegistrationFromImport(new ResultContext<ApplicationUserVM>() { ResultContextObject = applicationUser }, true);
                        }

                        var externalExpertForDb = new Expert()
                        {
                            IdPerson = personForDb.IdPerson,
                            IsCommissionExpert = false,
                            IsExternalExpert = true,
                            IsDOCExpert = false,
                            IsNapooExpert = false,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<Expert>(externalExpertForDb);
                        await this.repository.SaveChangesAsync(false);

                        var procedureExpertForDb = new ProcedureExternalExpert()
                        {
                            IdStartedProcedure = startedProcedureForDb.IdStartedProcedure,
                            IdExpert = externalExpertForDb.IdExpert,
                            IsActive = true,
                            IdProfessionalDirection = 1246,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<ProcedureExternalExpert>(procedureExpertForDb);
                        await this.repository.SaveChangesAsync(false);

                        var expertProfDirectionForDb = new ExpertProfessionalDirection()
                        {
                            IdExpertType = kvExpertTypeExternalValue.IdKeyValue,
                            IdProfessionalDirection = 1246,
                            IdStatus = kvExpertActiveStatusValue.IdKeyValue,
                            DateApprovalExternalExpert = DateTime.Now,
                            OrderNumber = $"№ 01/{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT)} г.",
                            IdExpert = externalExpertForDb.IdExpert,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<ExpertProfessionalDirection>(expertProfDirectionForDb);
                        await this.repository.SaveChangesAsync(false);

                        importInfoList.Add($"Потребител: {user.UserName} , парола: {applicationUser.Password} , имейл: {applicationUser.Email} с процедура към център {candidateProviderForDb.ProviderOwner}->{emailText}");
                    }
                    else if (model.IdImportType == kvNAPOOExpertsTypeValue?.IdKeyValue)
                    {
                        var personForDb = new Person()
                        {
                            FirstName = person.FirstName,
                            SecondName = person.SecondName,
                            FamilyName = person.FamilyName,
                            Phone = person.Phone,
                            Email = person.Email,
                            TaxOffice = string.Empty,
                            IsContractRegisterDocu = false,
                            IsSignContract = false,
                            Position = person.Position,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<Person>(personForDb);
                        await this.repository.SaveChangesAsync(false);

                        var applicationUser = new ApplicationUserVM()
                        {
                            IdPerson = personForDb.IdPerson,
                            Email = personForDb.Email!,
                            EIK = string.Empty,
                            IdUserStatus = kvUserStatusActive.IdKeyValue,
                            FirstName = personForDb.FirstName,
                            FamilyName = personForDb.FamilyName,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        var user = this.CreateUser();
                        user.IdPerson = applicationUser.IdPerson;
                        user.Email = applicationUser.Email;
                        applicationUser.Password = this.GenerateRandomPassword();
                        user.UserName = await this.GenerateUserName(applicationUser);
                        user.IdUser = (int)(await this.GetSequenceNextValue("APPLICATION_USER_ID"));
                        user.IdUserStatus = applicationUser.IdUserStatus;
                        user.CreationDate = DateTime.Now;
                        user.ModifyDate = DateTime.Now;
                        user.IdCreateUser = UserProps.UserId;
                        user.IdModifyUser = UserProps.UserId;

                        var result = await this.userManager.CreateAsync(user, applicationUser.Password);
                        applicationUser.UserName = user.UserName;
                        if (result.Succeeded)
                        {
                            await this.userManager.AddClaimAsync(user,
                                new System.Security.Claims.Claim(
                                    GlobalConstants.ID_USER,
                            user.IdUser.ToString()));

                            await this.userManager.AddClaimAsync(user,
                                new System.Security.Claims.Claim(
                                    GlobalConstants.ID_PERSON,
                                    applicationUser.IdPerson.ToString()));

                            await this.userManager.AddClaimAsync(user,
                              new System.Security.Claims.Claim(
                                  GlobalConstants.PERSON_FULLNAME,
                                 applicationUser.FullName));

                            foreach (var role in model.Roles)
                            {
                                await this.userManager.AddToRoleAsync(user, role.Name);
                            }

                            this._logger.LogInformation($"User created an account with UserName:{user.UserName}.");
                        }

                        var random = new Random();
                        var bulstat = random.Next(100000001, 999999999).ToString();
                        while (await this.repository.AllReadonly<CandidateProvider>(x => x.PoviderBulstat == bulstat).FirstOrDefaultAsync() != null)
                        {
                            bulstat = random.Next(100000001, 999999999).ToString();
                        }

                        person.Bulstat = bulstat;

                        var startedProcedureForDb = new StartedProcedure()
                        {
                            TS = candidateProvider!.StartedProcedure.TS,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<StartedProcedure>(startedProcedureForDb);
                        await this.repository.SaveChangesAsync(false);

                        // инсъртва всички навързани таблици за прогрес на стартирана процедура
                        foreach (var progress in candidateProvider.StartedProcedure.StartedProcedureProgresses)
                        {
                            var progressForDb = new StartedProcedureProgress()
                            {
                                IdStartedProcedure = startedProcedureForDb.IdStartedProcedure,
                                IdStep = progress.IdStep,
                                StepDate = DateTime.Now,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<StartedProcedureProgress>(progressForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        // инсъртва всички навързани таблици за документи по процедура
                        foreach (var doc in candidateProvider.StartedProcedure.ProcedureDocuments)
                        {
                            var docForDb = new ProcedureDocument()
                            {
                                IdStartedProcedure = startedProcedureForDb.IdStartedProcedure,
                                IsValid = doc.IsValid,
                                IdDocumentType = doc.IdDocumentType,
                                DateAttachment = doc.DateAttachment,
                                MimeType = doc.MimeType,
                                Extension = doc.Extension,
                                UploadedFileName = doc.UploadedFileName,
                                DS_ID = doc.DS_ID,
                                DS_DATE = doc.DS_DATE,
                                DS_OFFICIAL_ID = doc.DS_OFFICIAL_ID,
                                DS_OFFICIAL_DATE = doc.DS_OFFICIAL_DATE,
                                DS_PREP = doc.DS_PREP,
                                DS_LINK = doc.DS_LINK,
                                DS_GUID = doc.DS_GUID,
                                DS_OFFICIAL_GUID = doc.DS_OFFICIAL_GUID,
                                DS_DocNumber = doc.DS_DocNumber,
                                DS_OFFICIAL_DocNumber = doc.DS_OFFICIAL_DocNumber,
                                IsFromDS = doc.IsFromDS,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<ProcedureDocument>(docForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        var kvApplicationFiledIn = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "RequestedByCPOOrCIPO");
                        var candidateProviderForDb = new CandidateProvider()
                        {
                            ProviderOwner = $"{candidateProvider!.ProviderOwner} {user.IdUser}",
                            ProviderName = $"{candidateProvider!.ProviderOwner} {user.IdUser}",
                            PoviderBulstat = person.Bulstat,
                            ManagerName = candidateProvider.ManagerName,
                            AttorneyName = candidateProvider.AttorneyName,
                            IdProviderRegistration = candidateProvider.IdProviderRegistration,
                            IdProviderOwnership = candidateProvider.IdProviderOwnership,
                            IdProviderStatus = candidateProvider.IdProviderStatus,
                            IdLocation = candidateProvider.IdLocation,
                            ProviderAddress = candidateProvider.ProviderAddress,
                            ZipCode = candidateProvider.ZipCode,
                            UploadedFileName = candidateProvider.UploadedFileName,
                            IdTypeLicense = candidateProvider.IdTypeLicense,
                            ApplicationNumber = candidateProvider.ApplicationNumber,
                            ApplicationDate = candidateProvider.ApplicationDate,
                            ProviderPhone = candidateProvider.ProviderPhone,
                            ProviderFax = candidateProvider.ProviderFax,
                            ProviderWeb = candidateProvider.ProviderWeb,
                            ProviderEmail = candidateProvider.ProviderEmail,
                            AdditionalInfo = candidateProvider.AdditionalInfo,
                            OnlineTrainingInfo = candidateProvider.OnlineTrainingInfo,
                            PersonNameCorrespondence = candidateProvider.PersonNameCorrespondence,
                            IdLocationCorrespondence = candidateProvider.IdLocationCorrespondence,
                            ProviderAddressCorrespondence = candidateProvider.ProviderAddressCorrespondence,
                            ZipCodeCorrespondence = candidateProvider.ZipCodeCorrespondence,
                            ProviderPhoneCorrespondence = candidateProvider.ProviderPhoneCorrespondence,
                            ProviderFaxCorrespondence = candidateProvider.ProviderFaxCorrespondence,
                            ProviderEmailCorrespondence = candidateProvider.ProviderEmailCorrespondence,
                            DateConfirmEMail = candidateProvider.DateConfirmEMail,
                            DateConfirmRequestNAPOO = candidateProvider.DateConfirmRequestNAPOO,
                            DateRequest = candidateProvider.DateRequest,
                            DueDateRequest = candidateProvider.DueDateRequest,
                            IdApplicationStatus = kvApplicationFiledIn.IdKeyValue,
                            IdTypeApplication = candidateProvider.IdTypeApplication,
                            Indent = candidateProvider.Indent,
                            IdApplicationFiling = candidateProvider.IdApplicationFiling,
                            IdReceiveLicense = candidateProvider.IdReceiveLicense,
                            LicenceNumber = null,
                            LicenceDate = null,
                            IdRegistrationApplicationStatus = null,
                            Title = candidateProvider.Title,
                            IsActive = true,
                            UIN = candidateProvider.UIN,
                            IdLicenceStatus = null,
                            IdRegionCorrespondence = candidateProvider.IdRegionCorrespondence,
                            DirectorFirstName = candidateProvider.DirectorFirstName,
                            DirectorSecondName = candidateProvider.DirectorSecondName,
                            DirectorFamilyName = candidateProvider.DirectorFamilyName,
                            IdRegionAdmin = candidateProvider.IdRegionAdmin,
                            Archive = candidateProvider.Archive,
                            AdditionalDocumentRequested = candidateProvider.AdditionalDocumentRequested,
                            IdStartedProcedure = startedProcedureForDb.IdStartedProcedure,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<CandidateProvider>(candidateProviderForDb);
                        await this.repository.SaveChangesAsync(false);

                        providersCount++;

                        // инсъртва всички навързани таблици за специалности
                        foreach (var providerSpeciality in candidateProvider.CandidateProviderSpecialities)
                        {
                            var candidateProviderSpecialityForDb = new CandidateProviderSpeciality()
                            {
                                IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                IdSpeciality = providerSpeciality.IdSpeciality,
                                IdFormEducation = providerSpeciality.IdFormEducation,
                                IdFrameworkProgram = providerSpeciality.IdFrameworkProgram,
                                LicenceData = providerSpeciality.LicenceData,
                                LicenceProtNo = providerSpeciality.LicenceProtNo,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderSpeciality>(candidateProviderSpecialityForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var modification in providerSpeciality.CandidateCurriculumModifications)
                            {
                                var modificationForDb = new CandidateCurriculumModification()
                                {
                                    IdCandidateProviderSpeciality = candidateProviderSpecialityForDb.IdCandidateProviderSpeciality,
                                    IdModificationReason = modification.IdModificationReason,
                                    IdModificationStatus = modification.IdModificationStatus,
                                    ValidFromDate = modification.ValidFromDate,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateCurriculumModification>(modificationForDb);
                                await this.repository.SaveChangesAsync(false);

                                foreach (var curriculum in modification.CandidateCurriculums)
                                {
                                    var curriculumForDb = new CandidateCurriculum()
                                    {
                                        IdCandidateProviderSpeciality = curriculum.IdCandidateProviderSpeciality,
                                        IdProfessionalTraining = curriculum.IdProfessionalTraining,
                                        Subject = curriculum.Subject,
                                        Topic = curriculum.Topic,
                                        Theory = curriculum.Theory,
                                        Practice = curriculum.Practice,
                                        IdCandidateCurriculumModification = modificationForDb.IdCandidateCurriculumModification,
                                        IdCreateUser = userFromSystem.IdUser,
                                        IdModifyUser = userFromSystem.IdUser,
                                        CreationDate = DateTime.Now,
                                        ModifyDate = DateTime.Now
                                    };

                                    await this.repository.AddAsync<CandidateCurriculum>(curriculumForDb);
                                    await this.repository.SaveChangesAsync(false);

                                    foreach (var eru in curriculum.CandidateCurriculumERUs)
                                    {
                                        var eruForDb = new CandidateCurriculumERU()
                                        {
                                            IdCandidateCurriculum = curriculumForDb.IdCandidateCurriculum,
                                            IdERU = eru.IdERU
                                        };

                                        await this.repository.AddAsync<CandidateCurriculumERU>(eruForDb);
                                        await this.repository.SaveChangesAsync(false);
                                    }
                                }
                            }
                        }

                        // инсъртва всички навързани таблици за преподаватели/консултанти
                        foreach (var trainer in candidateProvider.CandidateProviderTrainers)
                        {
                            var trainerForDb = new CandidateProviderTrainer()
                            {
                                IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                FirstName = trainer.FirstName,
                                SecondName = trainer.SecondName,
                                FamilyName = trainer.FamilyName,
                                IdIndentType = trainer.IdIndentType,
                                Indent = trainer.Indent,
                                BirthDate = trainer.BirthDate,
                                IdSex = trainer.IdSex,
                                IdNationality = trainer.IdNationality,
                                Email = trainer.Email,
                                IdEducation = trainer.IdEducation,
                                EducationSpecialityNotes = trainer.EducationSpecialityNotes,
                                EducationCertificateNotes = trainer.EducationCertificateNotes,
                                EducationAcademicNotes = trainer.EducationAcademicNotes,
                                IsAndragog = trainer.IsAndragog,
                                IdContractType = trainer.IdContractType,
                                ContractDate = trainer.ContractDate,
                                IdStatus = trainer.IdStatus,
                                DiplomaNumber = trainer.DiplomaNumber,
                                InactiveDate = trainer.InactiveDate,
                                ProfessionalQualificationCertificate = trainer.ProfessionalQualificationCertificate,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderTrainer>(trainerForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var trainerSpeciality in trainer.CandidateProviderTrainerSpecialities)
                            {
                                var trainerSpecialityForDb = new CandidateProviderTrainerSpeciality()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    IdSpeciality = trainerSpeciality.IdSpeciality,
                                    IdUsage = trainerSpeciality.IdUsage,
                                    IdComplianceDOC = trainerSpeciality.IdComplianceDOC,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerSpeciality>(trainerSpecialityForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var trainerQualification in trainer.CandidateProviderTrainerQualifications)
                            {
                                var trainerQualificationForDb = new CandidateProviderTrainerQualification()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    QualificationName = trainerQualification.QualificationName,
                                    IdQualificationType = trainerQualification.IdQualificationType,
                                    IdProfession = trainerQualification.IdProfession,
                                    IdTrainingQualificationType = trainerQualification.IdTrainingQualificationType,
                                    QualificationDuration = trainerQualification.QualificationDuration,
                                    TrainingFrom = trainerQualification.TrainingFrom,
                                    TrainingTo = trainerQualification.TrainingTo,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerQualification>(trainerQualificationForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var trainerProfile in trainer.CandidateProviderTrainerProfiles)
                            {
                                var trainerProfileForDb = new CandidateProviderTrainerProfile()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    IdProfessionalDirection = trainerProfile.IdProfessionalDirection,
                                    IsProfessionalDirectionQualified = trainerProfile.IsProfessionalDirectionQualified,
                                    IsTheory = trainerProfile.IsTheory,
                                    IsPractice = trainerProfile.IsPractice,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerProfile>(trainerProfileForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var trainerDoc in trainer.CandidateProviderTrainerDocuments)
                            {
                                var trainerDocForDb = new CandidateProviderTrainerDocument()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    IdDocumentType = trainerDoc.IdDocumentType,
                                    DocumentTitle = trainerDoc.DocumentTitle,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerDocument>(trainerDocForDb);
                                await this.repository.SaveChangesAsync(false);
                            }
                        }

                        // инсъртва всички навързани таблици за МТБ
                        foreach (var premises in candidateProvider.CandidateProviderPremises)
                        {
                            var premisesForDb = new CandidateProviderPremises()
                            {
                                IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                PremisesName = premises.PremisesName,
                                PremisesNote = premises.PremisesNote,
                                IdLocation = premises.IdLocation,
                                ProviderAddress = premises.ProviderAddress,
                                ZipCode = premises.ZipCode,
                                Phone = premises.Phone,
                                IdOwnership = premises.IdOwnership,
                                IdStatus = premises.IdStatus,
                                InactiveDate = premises.InactiveDate,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderPremises>(premisesForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var premisesSpec in premises.CandidateProviderPremisesSpecialities)
                            {
                                var premisesSpecialityForDb = new CandidateProviderPremisesSpeciality()
                                {
                                    IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                    IdSpeciality = premisesSpec.IdSpeciality,
                                    IdUsage = premisesSpec.IdUsage,
                                    IdComplianceDOC = premisesSpec.IdComplianceDOC,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderPremisesSpeciality>(premisesSpecialityForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var premisesRoom in premises.CandidateProviderPremisesRooms)
                            {
                                var premisesRoomForDb = new CandidateProviderPremisesRoom()
                                {
                                    IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                    PremisesRoomName = premisesRoom.PremisesRoomName,
                                    Equipment = premisesRoom.Equipment,
                                    IdUsage = premisesRoom.IdUsage,
                                    IdPremisesType = premisesRoom.IdPremisesType,
                                    Area = premisesRoom.Area,
                                    Workplace = premisesRoom.Workplace,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderPremisesRoom>(premisesRoomForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var premisesDoc in premises.CandidateProviderPremisesDocuments)
                            {
                                var premisesDocForDb = new CandidateProviderPremisesDocument()
                                {
                                    IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                    IdDocumentType = premisesDoc.IdDocumentType,
                                    DocumentTitle = premisesDoc.DocumentTitle,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderPremisesDocument>(premisesDocForDb);
                                await this.repository.SaveChangesAsync(false);
                            }
                        }

                        // инсъртва всички навързани таблици за документи на CandidateProvider
                        foreach (var doc in candidateProvider.CandidateProviderDocuments)
                        {
                            var docForDb = new CandidateProviderDocument()
                            {
                                IdCandidateProvider = candidateProviderForDb.IdCandidate_Provider,
                                IdDocumentType = doc.IdDocumentType,
                                DocumentTitle = doc.DocumentTitle,
                                IsAdditionalDocument = doc.IsAdditionalDocument,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderDocument>(docForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        // инсъртва всички навързани таблици за структура на CPO
                        foreach (var structure in candidateProvider.CandidateProviderCPOStructureAndActivities)
                        {
                            var structureForDb = new CandidateProviderCPOStructureActivity()
                            {
                                IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                Management = structure.Management,
                                OrganisationTrainingProcess = structure.OrganisationTrainingProcess,
                                CompletionCertificationTraining = structure.CompletionCertificationTraining,
                                InternalQualitySystem = structure.InternalQualitySystem,
                                InformationProvisionMaintenance = structure.InformationProvisionMaintenance,
                                TrainingDocumentation = structure.TrainingDocumentation,
                                TeachersSelection = structure.TeachersSelection,
                                MTBDescription = structure.MTBDescription,
                                DataMaintenance = structure.DataMaintenance,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderCPOStructureActivity>(structureForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        var externalExpertForDb = new Expert()
                        {
                            IdPerson = personForDb.IdPerson,
                            IsCommissionExpert = false,
                            IsExternalExpert = false,
                            IsDOCExpert = false,
                            IsNapooExpert = true,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<Expert>(externalExpertForDb);
                        await this.repository.SaveChangesAsync(false);

                        var expertNAPOOForDb = new ExpertNapoo()
                        {
                            IdExpert = externalExpertForDb.IdExpert,
                            Occupation = person.Position,
                            IdStatus = kvExpertActiveStatusValue.IdKeyValue,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<ExpertNapoo>(expertNAPOOForDb);
                        await this.repository.SaveChangesAsync(false);

                        var procedureExpertForDb = new ProcedureExternalExpert()
                        {
                            IdStartedProcedure = startedProcedureForDb.IdStartedProcedure,
                            IdExpert = externalExpertForDb.IdExpert,
                            IsActive = true,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<ProcedureExternalExpert>(procedureExpertForDb);
                        await this.repository.SaveChangesAsync(false);

                        random = new Random();
                        bulstat = random.Next(100000001, 999999999).ToString();
                        while (await this.repository.AllReadonly<CandidateProvider>(x => x.PoviderBulstat == bulstat).FirstOrDefaultAsync() != null)
                        {
                            bulstat = random.Next(100000001, 999999999).ToString();
                        }

                        person.Bulstat = bulstat;

                        var cipoStartedProcedureForDb = new StartedProcedure()
                        {
                            TS = cipoCandidateProvider!.StartedProcedure.TS,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<StartedProcedure>(cipoStartedProcedureForDb);
                        await this.repository.SaveChangesAsync(false);

                        // инсъртва всички навързани таблици за прогрес на стартирана процедура
                        foreach (var progress in cipoCandidateProvider.StartedProcedure.StartedProcedureProgresses)
                        {
                            var progressForDb = new StartedProcedureProgress()
                            {
                                IdStartedProcedure = cipoStartedProcedureForDb.IdStartedProcedure,
                                IdStep = progress.IdStep,
                                StepDate = DateTime.Now,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<StartedProcedureProgress>(progressForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        // инсъртва всички навързани таблици за документи по процедура
                        foreach (var doc in cipoCandidateProvider.StartedProcedure.ProcedureDocuments)
                        {
                            var docForDb = new ProcedureDocument()
                            {
                                IdStartedProcedure = cipoStartedProcedureForDb.IdStartedProcedure,
                                IsValid = doc.IsValid,
                                IdDocumentType = doc.IdDocumentType,
                                DateAttachment = doc.DateAttachment,
                                MimeType = doc.MimeType,
                                Extension = doc.Extension,
                                UploadedFileName = doc.UploadedFileName,
                                DS_ID = doc.DS_ID,
                                DS_DATE = doc.DS_DATE,
                                DS_OFFICIAL_ID = doc.DS_OFFICIAL_ID,
                                DS_OFFICIAL_DATE = doc.DS_OFFICIAL_DATE,
                                DS_PREP = doc.DS_PREP,
                                DS_LINK = doc.DS_LINK,
                                DS_GUID = doc.DS_GUID,
                                DS_OFFICIAL_GUID = doc.DS_OFFICIAL_GUID,
                                DS_DocNumber = doc.DS_DocNumber,
                                DS_OFFICIAL_DocNumber = doc.DS_OFFICIAL_DocNumber,
                                IsFromDS = doc.IsFromDS,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<ProcedureDocument>(docForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        var cipoCandidateProviderForDb = new CandidateProvider()
                        {
                            ProviderOwner = $"{candidateProvider!.ProviderOwner} {user.IdUser}",
                            ProviderName = $"{candidateProvider!.ProviderOwner} {user.IdUser}",
                            PoviderBulstat = person.Bulstat,
                            ManagerName = cipoCandidateProvider.ManagerName,
                            AttorneyName = cipoCandidateProvider.AttorneyName,
                            IdProviderRegistration = cipoCandidateProvider.IdProviderRegistration,
                            IdProviderOwnership = cipoCandidateProvider.IdProviderOwnership,
                            IdProviderStatus = cipoCandidateProvider.IdProviderStatus,
                            IdLocation = cipoCandidateProvider.IdLocation,
                            ProviderAddress = cipoCandidateProvider.ProviderAddress,
                            ZipCode = cipoCandidateProvider.ZipCode,
                            UploadedFileName = cipoCandidateProvider.UploadedFileName,
                            IdTypeLicense = cipoCandidateProvider.IdTypeLicense,
                            ApplicationNumber = cipoCandidateProvider.ApplicationNumber,
                            ApplicationDate = cipoCandidateProvider.ApplicationDate,
                            ProviderPhone = cipoCandidateProvider.ProviderPhone,
                            ProviderFax = cipoCandidateProvider.ProviderFax,
                            ProviderWeb = cipoCandidateProvider.ProviderWeb,
                            ProviderEmail = cipoCandidateProvider.ProviderEmail,
                            AdditionalInfo = cipoCandidateProvider.AdditionalInfo,
                            OnlineTrainingInfo = cipoCandidateProvider.OnlineTrainingInfo,
                            PersonNameCorrespondence = cipoCandidateProvider.PersonNameCorrespondence,
                            IdLocationCorrespondence = cipoCandidateProvider.IdLocationCorrespondence,
                            ProviderAddressCorrespondence = cipoCandidateProvider.ProviderAddressCorrespondence,
                            ZipCodeCorrespondence = cipoCandidateProvider.ZipCodeCorrespondence,
                            ProviderPhoneCorrespondence = cipoCandidateProvider.ProviderPhoneCorrespondence,
                            ProviderFaxCorrespondence = cipoCandidateProvider.ProviderFaxCorrespondence,
                            ProviderEmailCorrespondence = cipoCandidateProvider.ProviderEmailCorrespondence,
                            DateConfirmEMail = cipoCandidateProvider.DateConfirmEMail,
                            DateConfirmRequestNAPOO = cipoCandidateProvider.DateConfirmRequestNAPOO,
                            DateRequest = cipoCandidateProvider.DateRequest,
                            DueDateRequest = cipoCandidateProvider.DueDateRequest,
                            IdApplicationStatus = kvApplicationFiledIn.IdKeyValue,
                            IdTypeApplication = cipoCandidateProvider.IdTypeApplication,
                            Indent = cipoCandidateProvider.Indent,
                            IdApplicationFiling = cipoCandidateProvider.IdApplicationFiling,
                            IdReceiveLicense = cipoCandidateProvider.IdReceiveLicense,
                            LicenceNumber = null,
                            LicenceDate = null,
                            IdRegistrationApplicationStatus = null,
                            Title = cipoCandidateProvider.Title,
                            IsActive = true,
                            UIN = cipoCandidateProvider.UIN,
                            IdLicenceStatus = null,
                            IdRegionCorrespondence = cipoCandidateProvider.IdRegionCorrespondence,
                            DirectorFirstName = cipoCandidateProvider.DirectorFirstName,
                            DirectorSecondName = cipoCandidateProvider.DirectorSecondName,
                            DirectorFamilyName = cipoCandidateProvider.DirectorFamilyName,
                            IdRegionAdmin = cipoCandidateProvider.IdRegionAdmin,
                            Archive = cipoCandidateProvider.Archive,
                            AdditionalDocumentRequested = cipoCandidateProvider.AdditionalDocumentRequested,
                            IdStartedProcedure = startedProcedureForDb.IdStartedProcedure,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<CandidateProvider>(cipoCandidateProviderForDb);
                        await this.repository.SaveChangesAsync(false);

                        providersCount++;

                        // инсъртва всички навързани таблици за преподаватели/консултанти
                        foreach (var trainer in cipoCandidateProvider.CandidateProviderTrainers)
                        {
                            var trainerForDb = new CandidateProviderTrainer()
                            {
                                IdCandidate_Provider = cipoCandidateProviderForDb.IdCandidate_Provider,
                                FirstName = trainer.FirstName,
                                SecondName = trainer.SecondName,
                                FamilyName = trainer.FamilyName,
                                IdIndentType = trainer.IdIndentType,
                                Indent = trainer.Indent,
                                BirthDate = trainer.BirthDate,
                                IdSex = trainer.IdSex,
                                IdNationality = trainer.IdNationality,
                                Email = trainer.Email,
                                IdEducation = trainer.IdEducation,
                                EducationSpecialityNotes = trainer.EducationSpecialityNotes,
                                EducationCertificateNotes = trainer.EducationCertificateNotes,
                                EducationAcademicNotes = trainer.EducationAcademicNotes,
                                IsAndragog = trainer.IsAndragog,
                                IdContractType = trainer.IdContractType,
                                ContractDate = trainer.ContractDate,
                                IdStatus = trainer.IdStatus,
                                DiplomaNumber = trainer.DiplomaNumber,
                                InactiveDate = trainer.InactiveDate,
                                ProfessionalQualificationCertificate = trainer.ProfessionalQualificationCertificate,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderTrainer>(trainerForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var trainerQualification in trainer.CandidateProviderTrainerQualifications)
                            {
                                var trainerQualificationForDb = new CandidateProviderTrainerQualification()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    QualificationName = trainerQualification.QualificationName,
                                    IdQualificationType = trainerQualification.IdQualificationType,
                                    IdProfession = trainerQualification.IdProfession,
                                    IdTrainingQualificationType = trainerQualification.IdTrainingQualificationType,
                                    QualificationDuration = trainerQualification.QualificationDuration,
                                    TrainingFrom = trainerQualification.TrainingFrom,
                                    TrainingTo = trainerQualification.TrainingTo,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerQualification>(trainerQualificationForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var trainerProfile in trainer.CandidateProviderTrainerProfiles)
                            {
                                var trainerProfileForDb = new CandidateProviderTrainerProfile()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    IdProfessionalDirection = trainerProfile.IdProfessionalDirection,
                                    IsProfessionalDirectionQualified = trainerProfile.IsProfessionalDirectionQualified,
                                    IsTheory = trainerProfile.IsTheory,
                                    IsPractice = trainerProfile.IsPractice,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerProfile>(trainerProfileForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var trainerDoc in trainer.CandidateProviderTrainerDocuments)
                            {
                                var trainerDocForDb = new CandidateProviderTrainerDocument()
                                {
                                    IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer,
                                    IdDocumentType = trainerDoc.IdDocumentType,
                                    DocumentTitle = trainerDoc.DocumentTitle,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderTrainerDocument>(trainerDocForDb);
                                await this.repository.SaveChangesAsync(false);
                            }
                        }

                        // инсъртва всички навързани таблици за МТБ
                        foreach (var premises in cipoCandidateProvider.CandidateProviderPremises)
                        {
                            var premisesForDb = new CandidateProviderPremises()
                            {
                                IdCandidate_Provider = cipoCandidateProviderForDb.IdCandidate_Provider,
                                PremisesName = premises.PremisesName,
                                PremisesNote = premises.PremisesNote,
                                IdLocation = premises.IdLocation,
                                ProviderAddress = premises.ProviderAddress,
                                ZipCode = premises.ZipCode,
                                Phone = premises.Phone,
                                IdOwnership = premises.IdOwnership,
                                IdStatus = premises.IdStatus,
                                InactiveDate = premises.InactiveDate,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderPremises>(premisesForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var premisesRoom in premises.CandidateProviderPremisesRooms)
                            {
                                var premisesRoomForDb = new CandidateProviderPremisesRoom()
                                {
                                    IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                    PremisesRoomName = premisesRoom.PremisesRoomName,
                                    Equipment = premisesRoom.Equipment,
                                    IdUsage = premisesRoom.IdUsage,
                                    IdPremisesType = premisesRoom.IdPremisesType,
                                    Area = premisesRoom.Area,
                                    Workplace = premisesRoom.Workplace,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderPremisesRoom>(premisesRoomForDb);
                                await this.repository.SaveChangesAsync(false);
                            }

                            foreach (var premisesDoc in premises.CandidateProviderPremisesDocuments)
                            {
                                var premisesDocForDb = new CandidateProviderPremisesDocument()
                                {
                                    IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises,
                                    IdDocumentType = premisesDoc.IdDocumentType,
                                    DocumentTitle = premisesDoc.DocumentTitle,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CandidateProviderPremisesDocument>(premisesDocForDb);
                                await this.repository.SaveChangesAsync(false);
                            }
                        }

                        // инсъртва всички навързани таблици за документи на CandidateProvider
                        foreach (var doc in cipoCandidateProvider.CandidateProviderDocuments)
                        {
                            var docForDb = new CandidateProviderDocument()
                            {
                                IdCandidateProvider = cipoCandidateProviderForDb.IdCandidate_Provider,
                                IdDocumentType = doc.IdDocumentType,
                                DocumentTitle = doc.DocumentTitle,
                                IsAdditionalDocument = doc.IsAdditionalDocument,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderDocument>(docForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        // инсъртва всички навързани таблици за структура на CIPO
                        foreach (var structure in cipoCandidateProvider.CandidateProviderCIPOStructureAndActivities)
                        {
                            var structureForDb = new CandidateProviderCIPOStructureActivity()
                            {
                                IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider,
                                Management = structure.Management,
                                OrganisationInformationProcess = structure.OrganisationInformationProcess,
                                InternalQualitySystem = structure.InternalQualitySystem,
                                InformationProvisionMaintenance = structure.InformationProvisionMaintenance,
                                TrainingDocumentation = structure.TrainingDocumentation,
                                ConsultantsSelection = structure.ConsultantsSelection,
                                MTBDescription = structure.MTBDescription,
                                DataMaintenance = structure.DataMaintenance,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderCIPOStructureActivity>(structureForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        // инсъртва всички навързани таблици за консултации на CandidateProvider
                        foreach (var consulting in cipoCandidateProvider.CandidateProviderConsultings)
                        {
                            var consultingForDb = new CandidateProviderConsulting()
                            {
                                IdCandidateProvider = cipoCandidateProviderForDb.IdCandidate_Provider,
                                IdConsultingType = consulting.IdConsultingType,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CandidateProviderConsulting>(consultingForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        var cipoProcedureExpertForDb = new ProcedureExternalExpert()
                        {
                            IdStartedProcedure = cipoStartedProcedureForDb.IdStartedProcedure,
                            IdExpert = externalExpertForDb.IdExpert,
                            IsActive = true,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<ProcedureExternalExpert>(cipoProcedureExpertForDb);
                        await this.repository.SaveChangesAsync(false);

                        string emailText = string.Empty;
                        if (model.AllowSentEmails)
                        {
                            emailText = await this.mailService.SendEmailNewRegistrationFromImport(new ResultContext<ApplicationUserVM>() { ResultContextObject = applicationUser }, true);
                        }

                        usersCount++;

                        importInfoList.Add($"Потребител: {user.UserName} , парола: {applicationUser.Password} , имейл: {applicationUser.Email}->{emailText}");
                    }
                }

                var users = usersCount == 1
                    ? "потребител"
                    : "потребителя";
                var providers = providersCount == 1
                    ? "център"
                    : "центъра";
                var importEnd = DateTime.Now;
                importInfoList.Insert(0, $"Импортът започна в {importStart.ToString("HH:mm:ss")} ч. и приключи в {importEnd.ToString("HH:mm:ss")} ч.");
                var countMsgToinsert = model.IdImportType == kvCPOTypeValue!.IdKeyValue || model.IdImportType == kvCIPOTypeValue!.IdKeyValue
                    ? $"Създадени са {usersCount} {users} към {providersCount} {providers}."
                    : $"Създадени са {usersCount} {users}.";
                importInfoList.Insert(1, countMsgToinsert);

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet sheet = workbook.Worksheets[0];

                    sheet.Range["A1"].ColumnWidth = 150;
                    sheet.Range[$"A1"].Text = "Данни от импорт:";
                    //sheet.Range[$"B1"].Text = "Позиция във файла";

                    if (model.AllowSentEmails)
                    {
                        sheet.Range["B1"].ColumnWidth = 35;
                        sheet.Range[$"B1"].Text = "Съдържание на имейл:";
                    }

                    var rowCounter = 2;
                    foreach (var msg in importInfoList)
                    {
                        var splittedMsg = msg.Split("->");
                        var info = splittedMsg[0];
                        sheet.Range[$"A{rowCounter}"].Text = info;
                        if (splittedMsg.Count() > 1)
                        {
                            var emailText = splittedMsg[1];
                            if (!string.IsNullOrEmpty(emailText))
                            {
                                sheet.Range[$"B{rowCounter}"].Text = emailText;
                            }
                        }

                        rowCounter++;

                        if (rowCounter == 4)
                        {
                            rowCounter++;
                        }
                    }

                    using (ms = new MemoryStream())
                    {
                        workbook.SaveAs(ms);
                    }
                }

                isSuccessfull = true;
            }
            catch (Exception ex)
            {
                isSuccessfull = false;
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return (ms, isSuccessfull);
        }

        public async Task CopyCoursesAsync()
        {
            try
            {
                var kvApplicationAccepted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureCompleted");
                var userID = await this.dataSourceService.GetSettingByIntCodeAsync("UserIDBindWithSystem");
                var userFromSystem = await this.userManager.FindByIdAsync(userID.SettingValue);
                var programsFromDb = await this.repository.AllReadonly<Program>(x => x.IdCandidateProvider == 1162).ToListAsync();
                var firstCandidateProviderSpecialities = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == 1165).Include(x => x.CandidateCurriculums).ToListAsync();
                var firstCandidateProviderPremises = await this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidate_Provider == 1165).ToListAsync();
                var clients = await this.repository.AllReadonly<Client>(x => x.IdCandidateProvider == 1162).ToListAsync();
                foreach (var client in clients)
                {
                    var clientForDb = new Client()
                    {
                        FirstName = client.FirstName,
                        SecondName = client.SecondName,
                        FamilyName = client.FamilyName,
                        IdCandidateProvider = 1165,
                        IdSex = client.IdSex,
                        IdIndentType = client.IdIndentType,
                        Indent = client.Indent,
                        BirthDate = client.BirthDate,
                        IdNationality = client.IdNationality,
                        IdProfessionalDirection = client.IdProfessionalDirection,
                        IdEducation = client.IdEducation,
                        IdCityOfBirth = client.IdCityOfBirth,
                        IdCountryOfBirth = client.IdCountryOfBirth,
                        FamilyNameEN = client.FamilyName,
                        FirstNameEN = client.FirstNameEN,
                        SecondNameEN = client.SecondNameEN,
                        IdCreateUser = userFromSystem.IdUser,
                        IdModifyUser = userFromSystem.IdUser,
                        CreationDate = DateTime.Now,
                        ModifyDate = DateTime.Now
                    };

                    await this.repository.AddAsync<Client>(clientForDb);
                    await this.repository.SaveChangesAsync(false);
                }

                foreach (var program in programsFromDb)
                {
                    var programForDb = new Program()
                    {
                        ProgramNumber = program.ProgramNumber,
                        ProgramName = program.ProgramName,
                        ProgramNote = program.ProgramNote,
                        IdCandidateProvider = 1165,
                        IdSpeciality = program.IdSpeciality,
                        IdFrameworkProgram = program.IdFrameworkProgram,
                        IdCourseType = program.IdCourseType,
                        MandatoryHours = program.MandatoryHours,
                        SelectableHours = program.SelectableHours,
                        IsDeleted = program.IsDeleted,
                        IdMinimumLevelEducation = program.IdMinimumLevelEducation,
                        IdLegalCapacityOrdinanceType = program.IdLegalCapacityOrdinanceType,
                        IdCreateUser = userFromSystem.IdUser,
                        IdModifyUser = userFromSystem.IdUser,
                        CreationDate = DateTime.Now,
                        ModifyDate = DateTime.Now
                    };

                    await this.repository.AddAsync<Program>(programForDb);
                    await this.repository.SaveChangesAsync(false);

                    var candidateProviderSpeciality = firstCandidateProviderSpecialities.FirstOrDefault(x => x.IdSpeciality == program.IdSpeciality);
                    var curriculums = await this.repository.AllReadonly<TrainingCurriculum>(x => x.IdProgram ==  program.IdProgram).Include(x => x.TrainingCurriculumERUs).ToListAsync();
                    foreach (var curriculum in curriculums)
                    {
                        var curriculumFromDb = candidateProviderSpeciality?.CandidateCurriculums.FirstOrDefault(x => x.IdProfessionalTraining == curriculum.IdProfessionalTraining && x.Subject == curriculum.Subject && x.Topic == curriculum.Topic);
                        var curriculumForDb = new TrainingCurriculum()
                        {
                            IdCandidateCurriculum = curriculumFromDb?.IdCandidateCurriculum,
                            IdCandidateProviderSpeciality = candidateProviderSpeciality!.IdCandidateProviderSpeciality,
                            IdProgram = programForDb.IdProgram,
                            IdCourse = curriculum.IdCourse,
                            IdProfessionalTraining = curriculum.IdProfessionalTraining,
                            Subject = curriculum.Subject,
                            Topic = curriculum.Topic,
                            Theory = curriculum.Theory,
                            Practice = curriculum.Practice,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<TrainingCurriculum>(curriculumForDb);
                        await this.repository.SaveChangesAsync(false);

                        foreach (var eru in curriculum.TrainingCurriculumERUs)
                        {
                            var eruForDb = new TrainingCurriculumERU()
                            {
                                IdTrainingCurriculum = curriculumForDb.IdTrainingCurriculum,
                                IdERU = eru.IdERU
                            };

                            await this.repository.AddAsync<TrainingCurriculumERU>(eruForDb);
                            await this.repository.SaveChangesAsync(false);
                        }
                    }

                    var courses = await this.repository.AllReadonly<Course>(x => x.IdProgram == program.IdProgram).Include(x => x.CandidateProviderPremises).ToListAsync();
                    foreach (var course in courses)
                    {
                        var premises = firstCandidateProviderPremises.FirstOrDefault(x => x.PremisesName == course.CandidateProviderPremises.PremisesName);
                        var courseForDb = new Course()
                        {
                            IdProgram = programForDb.IdProgram,
                            SubscribeDate = course.SubscribeDate,
                            CourseName = course.CourseName,
                            AdditionalNotes = course.AdditionalNotes,
                            IdStatus = course.IdStatus,
                            IdMeasureType = course.IdMeasureType,
                            IdAssignType = course.IdAssignType,
                            IdFormEducation = course.IdFormEducation,
                            IdLocation = course.IdLocation,
                            MandatoryHours = course.MandatoryHours,
                            SelectableHours = course.SelectableHours,
                            DurationHours = course.DurationHours,
                            Cost = course.Cost,
                            StartDate = course.StartDate,
                            EndDate = course.EndDate,
                            ExamTheoryDate = course.ExamTheoryDate,
                            ExamPracticeDate = course.ExamPracticeDate,
                            IdCandidateProviderPremises = premises!.IdCandidateProviderPremises,
                            DisabilityCount = course.DisabilityCount,
                            IdCandidateProvider = 1165,
                            IdTrainingCourseType = course.IdTrainingCourseType,
                            IsArchived = course.IsArchived,
                            CourseNameEN = course.CourseNameEN,
                            IdLegalCapacityOrdinanceType = course.IdLegalCapacityOrdinanceType,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync<Course>(courseForDb);
                        await this.repository.SaveChangesAsync(false);

                        var clientsFromCourse = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == course.IdCourse).ToListAsync();
                        foreach (var clientFromCourse in clientsFromCourse)
                        {
                            var clientFromDb = await this.repository.AllReadonly<Client>(x => x.IdCandidateProvider == 1165 && x.Indent == clientFromCourse.Indent).FirstOrDefaultAsync();
                            var clientCourseForDb = new ClientCourse()
                            {
                                IdClient = clientFromDb!.IdClient,
                                IdCourse = courseForDb.IdCourse,
                                FirstName = clientFromCourse.FirstName,
                                SecondName = clientFromCourse.SecondName,
                                FamilyName = clientFromCourse.FamilyName,
                                IdSex = clientFromCourse.IdSex,
                                IdIndentType = clientFromCourse.IdSex,
                                Indent = clientFromCourse.Indent,
                                BirthDate = clientFromCourse.BirthDate,
                                IdNationality = clientFromCourse.IdNationality,
                                IdProfessionalDirection = clientFromCourse.IdProfessionalDirection,
                                IdSpeciality = clientFromCourse.IdSpeciality,
                                IdEducation = clientFromCourse.IdEducation,
                                IdAssignType = clientFromCourse.IdAssignType,
                                IdFinishedType = clientFromCourse.IdFinishedType,
                                FinishedDate = clientFromCourse.FinishedDate,
                                IdQualificationLevel = clientFromCourse.IdQualificationLevel,
                                IdCityOfBirth = clientFromCourse.IdCityOfBirth,
                                IdCountryOfBirth = clientFromCourse.IdCountryOfBirth,
                                CourseJoinDate = clientFromCourse.CourseJoinDate,
                                Address = clientFromCourse.Address,
                                EmailAddress = clientFromCourse.Address,
                                Phone = clientFromCourse.Phone,
                                IsContactAllowed = clientFromCourse.IsContactAllowed,
                                IsDisabledPerson = clientFromCourse.IsDisabledPerson,
                                IsDisadvantagedPerson = clientFromCourse.IsDisadvantagedPerson,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<ClientCourse>(clientCourseForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        var subjectsFromCourse = await this.repository.AllReadonly<CourseSubject>(x => x.IdCourse == course.IdCourse).Include(x => x.CourseSubjectGrades).ThenInclude(x => x.ClientCourse).AsNoTracking().ToListAsync();
                        foreach (var subject in subjectsFromCourse)
                        {
                            var subjectForDb = new CourseSubject()
                            {
                                IdCourse = courseForDb.IdCourse,
                                IdProfessionalTraining = subject.IdProfessionalTraining,
                                Subject = subject.Subject,
                                PracticeHours = subject.PracticeHours,
                                TheoryHours = subject.TheoryHours,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CourseSubject>(subjectForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var grade in subject.CourseSubjectGrades)
                            {
                                var clientCourseFromDb = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == courseForDb.IdCourse && x.Indent == grade.ClientCourse.Indent).FirstOrDefaultAsync();
                                var gradeForDb = new CourseSubjectGrade()
                                {
                                    IdClientCourse = clientCourseFromDb!.IdClientCourse,
                                    IdCourseSubject = subjectForDb.IdCourseSubject,
                                    PracticeGrade = grade.PracticeGrade,
                                    TheoryGrade = grade.TheoryGrade,
                                    IdCreateUser = userFromSystem.IdUser,
                                    IdModifyUser = userFromSystem.IdUser,
                                    CreationDate = DateTime.Now,
                                    ModifyDate = DateTime.Now
                                };

                                await this.repository.AddAsync<CourseSubjectGrade>(gradeForDb);
                                await this.repository.SaveChangesAsync(false);
                            }
                        }

                        var commisionMembersFromCourse = await this.repository.AllReadonly<CourseCommissionMember>(x => x.IdCourse == course.IdCourse).ToListAsync();
                        foreach (var member in commisionMembersFromCourse)
                        {
                            var memberForDb = new CourseCommissionMember()
                            {
                                FirstName = member.FirstName,
                                SecondName = member.SecondName,
                                FamilyName = member.FamilyName,
                                IsChairman = member.IsChairman,
                                IdCourse = courseForDb.IdCourse,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<CourseCommissionMember>(memberForDb);
                            await this.repository.SaveChangesAsync(false);
                        }

                        var trainingCurriculumsFromCourse = await this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == course.IdCourse).Include(x => x.TrainingCurriculumERUs).ToListAsync();
                        foreach (var curriculum in trainingCurriculumsFromCourse)
                        {
                            var curriculumFromDb = candidateProviderSpeciality?.CandidateCurriculums.FirstOrDefault(x => x.IdProfessionalTraining == curriculum.IdProfessionalTraining && x.Subject == curriculum.Subject && x.Topic == curriculum.Topic);
                            var curriculumForDb = new TrainingCurriculum()
                            {
                                IdCandidateCurriculum = curriculumFromDb?.IdCandidateCurriculum,
                                IdCandidateProviderSpeciality = candidateProviderSpeciality!.IdCandidateProviderSpeciality,
                                IdProgram = programForDb.IdProgram,
                                IdCourse = courseForDb.IdCourse,
                                IdProfessionalTraining = curriculum.IdProfessionalTraining,
                                Subject = curriculum.Subject,
                                Topic = curriculum.Topic,
                                Theory = curriculum.Theory,
                                Practice = curriculum.Practice,
                                IdCreateUser = userFromSystem.IdUser,
                                IdModifyUser = userFromSystem.IdUser,
                                CreationDate = DateTime.Now,
                                ModifyDate = DateTime.Now
                            };

                            await this.repository.AddAsync<TrainingCurriculum>(curriculumForDb);
                            await this.repository.SaveChangesAsync(false);

                            foreach (var eru in curriculum.TrainingCurriculumERUs)
                            {
                                var eruForDb = new TrainingCurriculumERU()
                                {
                                    IdTrainingCurriculum = curriculumForDb.IdTrainingCurriculum,
                                    IdERU = eru.IdERU
                                };

                                await this.repository.AddAsync<TrainingCurriculumERU>(eruForDb);
                                await this.repository.SaveChangesAsync(false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        private string GenerateRandomPassword()
        {
            PasswordPolicySettingsParameters settingParams = PasswordPolicySettingsParameters.GetPasswordPolicySettingsParameters();

            string passwordChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" + settingParams.SpecialCharacters;
            string capLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowLetters = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "0123456789";

            char[] password = new char[settingParams.MinimumPasswordLength];

            Random rand = new Random();
            int position = rand.Next(0, settingParams.MinimumPasswordLength);

            for (int i = 0; i < settingParams.MinimumCapitalLetters; i++)
            {
                while (password[position] != '\0')
                {
                    position = rand.Next(0, settingParams.MinimumPasswordLength);

                }
                password[position] = capLetters[rand.Next(0, capLetters.Length - 1)];
                position = rand.Next(0, settingParams.MinimumPasswordLength);
            }

            for (int i = 0; i < settingParams.MinimumLowLetters; i++)
            {
                while (password[position] != '\0')
                {
                    position = rand.Next(0, settingParams.MinimumPasswordLength);

                }
                password[position] = lowLetters[rand.Next(0, lowLetters.Length - 1)];
                position = rand.Next(0, settingParams.MinimumPasswordLength);
            }


            for (int i = 0; i < settingParams.MinimumNumbers; i++)
            {
                while (password[position] != '\0')
                {
                    position = rand.Next(0, settingParams.MinimumPasswordLength);

                }
                password[position] = numbers[rand.Next(0, numbers.Length - 1)];
                position = rand.Next(0, settingParams.MinimumPasswordLength);
            }
            for (int i = 0; i < settingParams.MinimumSpecialCharacters; i++)
            {
                while (password[position] != '\0')
                {
                    position = rand.Next(0, settingParams.MinimumPasswordLength);

                }
                password[position] = settingParams.SpecialCharacters[rand.Next(0, settingParams.SpecialCharacters.Length - 1)];
                position = rand.Next(0, settingParams.MinimumPasswordLength);
            }

            for (int i = 0; i < password.Length; i++)
            {
                if (password[i] == '\0')
                {
                    password[i] = passwordChars[rand.Next(0, passwordChars.Length - 1)];
                }
            }
            return new string(password);

        }

        private async Task<string> GenerateUserName(ApplicationUserVM createUserVM)
        {
            string userName = string.Empty;

            string _firstName = BaseHelper.ConvertCyrToLatin(createUserVM.FirstName);
            string _lastName = BaseHelper.ConvertCyrToLatin(createUserVM.FamilyName);

            userName = _firstName.Substring(0, 1).ToLower() + _lastName.ToLower();

            if (string.IsNullOrEmpty(createUserVM.EIK))
            {
                createUserVM.EIK = "0000";
            }
            userName = userName + "_" + createUserVM.EIK.Substring(createUserVM.EIK.Length - 4);

            var seq = await this.GetSequenceNextValue("APPLICATIONUSER#" + userName);

            if (seq != 1)
            {
                userName = userName + seq;
            }

            return userName;

        }

        public async Task<ResultContext<List<PersonVM>>> ImportUsersAndLinkToCandidateProviderAsync(MemoryStream file, string fileName, int idImportType)
        {

            var importUsersTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ImportUsersType");
            var kvCPOTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "CPO");
            var kvCIPOTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "CIPO");


            ResultContext<List<PersonVM>> resultContext = new ResultContext<List<PersonVM>>();
            if (idImportType != kvCPOTypeValue.IdKeyValue && idImportType != kvCIPOTypeValue.IdKeyValue) return resultContext;

            List<PersonVM> persons = new List<PersonVM>();
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportUsers";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + fileName;

                if (!string.IsNullOrEmpty(fileName))
                {
                    using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        file.WriteTo(filestream);
                        filestream.Close();
                        file.Close();
                    }
                }
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IApplication app = excelEngine.Excel;

                        IWorkbook workbook = app.Workbooks.Open(fileStream, ExcelOpenType.Automatic);

                        IWorksheet worksheet = workbook.Worksheets[0];

                        bool skipFirstRow = true;

                        var kvExternalExpertTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ExternalExperts");
                        var kvEGN = await this.dataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
                        var rowCounter = 2;
                        int counter = GlobalConstants.INVALID_ID_ZERO;
                        var licenceDict = new Dictionary<string, List<string>>();
                        foreach (var row in worksheet.Rows)
                        {
                            if (counter == 5)
                            {
                                break;
                            }

                            if (skipFirstRow || string.IsNullOrEmpty(row.Cells[0].Value))
                            {
                                skipFirstRow = false;
                                counter++;
                                continue;
                            }

                            if (idImportType == kvCIPOTypeValue?.IdKeyValue || idImportType == kvCPOTypeValue?.IdKeyValue)
                            {
                                var person = new PersonVM();

                                var licenseNumber = row.Cells[0].Value.Trim();
                                if (string.IsNullOrEmpty(licenseNumber))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведен номер на лицензия!");
                                }
                                else
                                {
                                    long licenseNumberAsLong = 0;
                                    if (!long.TryParse(licenseNumber, out licenseNumberAsLong))
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter} няма въведен валиден номер на лицензия. Номерът може да съдържа само цифри!");
                                    }
                                    else
                                    {
                                        person.LicenseNumber = licenseNumber;
                                    }
                                }

                                var fullName = row.Cells[3].Value.Trim();
                                if (string.IsNullOrEmpty(fullName))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведено име на Представител!");
                                }
                                else
                                {
                                    var names = fullName.Split(" ").ToArray();

                                    var setNames = true;

                                    foreach (var name in names)
                                    {
                                        if (!Regex.IsMatch(name, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} име на Представител може да съдържа само символи на кирилица!");
                                            setNames = false;
                                        }
                                    }

                                    if (setNames)
                                    {
                                        if (names.Length == 2)
                                        {
                                            person.FirstName = names[0];
                                            person.FamilyName = names[1];
                                        }
                                        else
                                        {

                                            person.FirstName = names[0];
                                            person.SecondName = names[1];
                                            person.FamilyName = names[2];
                                        }
                                    }
                                }
                                var indent = row.Cells[4].Value.Trim();
                                if(string.IsNullOrEmpty(indent))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведено ЕГН!");
                                }else
                                {
                                    person.IdIndentType = kvEGN.IdKeyValue;
                                    person.Indent = indent;
                                }

                                var phoneNumberRepresentative = row.Cells[5].Value.Trim();
                                if (!string.IsNullOrEmpty(phoneNumberRepresentative))
                                {
                                    var phoneNumber = string.Empty;
                                    if (phoneNumberRepresentative.StartsWith("+"))
                                    {
                                        phoneNumber = phoneNumberRepresentative.Split("+", StringSplitOptions.RemoveEmptyEntries)[0];
                                    }
                                    else
                                    {
                                        phoneNumber = phoneNumberRepresentative;
                                    }

                                    person.Phone = phoneNumber;
                                }

                                var emailRepresentatvie = row.Cells[6].Value.Trim().ToLower();
                                if (string.IsNullOrEmpty(emailRepresentatvie))
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} няма въведен имейл адрес на Представител 1!");
                                }
                                else
                                {
                                    Regex regex = new Regex("(?:[a-z0-9!#$%&'*+\\=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+\\=?^_`{|}~-]+)*|\"\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\"\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");
                                    if (regex.IsMatch(emailRepresentatvie))
                                    {
                                        person.Email = emailRepresentatvie;
                                    }
                                    else
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за имейл адрес на Представител 1!");
                                    }
                                }

                                if (!string.IsNullOrEmpty(person.LicenseNumber) && !string.IsNullOrEmpty(person.Email))
                                {
                                    if (!licenceDict.ContainsKey(person.LicenseNumber))
                                    {
                                        licenceDict.Add(person.LicenseNumber, new List<string>());
                                    }

                                    if (licenceDict[person.LicenseNumber].Count < 2)
                                    {
                                        licenceDict[person.LicenseNumber].Add(person.Email);
                                    }
                                    else
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не може да бъде добавен Представител 1, защото за № на лицензия {person.LicenseNumber} има вече добавени двама представители!");
                                    }
                                }

                                persons.Add(person);

                                rowCounter++;
                            }
                        }

                        if (persons.Any())
                        {
                            resultContext.AddMessage("Импортът приключи успешно!");
                        }

                        resultContext.ResultContextObject = persons;
                    }
                }
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }
        
        public async Task<(MemoryStream? MS, bool IsSuccessfull)> CreateUsersForCandidateProvidersAsync(List<PersonVM> persons, ImportUsersVM model)
        {
            MemoryStream ms = new MemoryStream();
            bool IsSuccessfull = true;
            List<UserModel> users = new List<UserModel>();

            var personsDb = this.repository.All<Person>().ToList();


            var userID = await this.dataSourceService.GetSettingByIntCodeAsync("UserIDBindWithSystem");
            var userFromSystem = await this.userManager.FindByIdAsync(userID.SettingValue);
            var kvUserStatusActive = await this.dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active");
            var importUsersTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ImportUsersType");
            var kvCPOTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "CPO");
            var kvCIPOTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "CIPO");
            var kvExternalExpertTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ExternalExperts");
            var kvNAPOOExpertsTypeValue = importUsersTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "NAPOOExperts");
            try
            {
                if (model.IdImportType == kvCPOTypeValue.IdKeyValue || model.IdImportType == kvCIPOTypeValue.IdKeyValue)
                {
                    var candidateProvidersFromDB = this.repository.AllReadonly<CandidateProvider>().ToList();

                    foreach (var person in persons)
                    {
                        //if(personsDb.Any(x => x.Email.Equals(person.Email)))
                        //{
                        //    continue;
                        //} 

                        var candidate = this.repository.AllReadonly<CandidateProvider>().Where(x => x.LicenceNumber == person.LicenseNumber).Include(x => x.CandidateProviderSpecialities).First();
                        var idCandidateProvider = candidate.IdCandidate_Provider;

                        var personForDb = new Person()
                        {
                            FirstName = person.FirstName,
                            FamilyName = person.FamilyName,
                            Phone = person.Phone,
                            Email = person.Email,
                            TaxOffice = string.Empty,
                            IsContractRegisterDocu = false,
                            IsSignContract = false,
                            Position = candidate!.CandidateProviderSpecialities.Any() ? "Представител на ЦПО" : "Представител на ЦИПО",
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        await this.repository.AddAsync(personForDb);
                        await this.repository.SaveChangesAsync();

                        var applicationUser = new ApplicationUserVM()
                        {
                            IdPerson = personForDb.IdPerson,
                            Email = personForDb.Email!,
                            IdCandidateProvider = idCandidateProvider,
                            EIK = candidate.PoviderBulstat,
                            IdUserStatus = kvUserStatusActive.IdKeyValue,
                            FirstName = personForDb.FirstName,
                            FamilyName = personForDb.FamilyName,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        var user = this.CreateUser();
                        user.IdPerson = applicationUser.IdPerson;
                        user.Email = applicationUser.Email;
                        applicationUser.Password = this.GenerateRandomPassword();
                        user.UserName = await this.GenerateUserName(applicationUser);
                        user.IdUser = (int)(await this.GetSequenceNextValue("APPLICATION_USER_ID"));
                        user.IdUserStatus = applicationUser.IdUserStatus;
                        user.CreationDate = DateTime.Now;
                        user.ModifyDate = DateTime.Now;
                        user.IdCreateUser = UserProps.UserId;
                        user.IdModifyUser = UserProps.UserId;

                        var result = await this.userManager.CreateAsync(user, applicationUser.Password);
                        applicationUser.UserName = user.UserName;
                        if (result.Succeeded)
                        {
                            if (applicationUser.IdCandidateProvider != 0)
                            {
                                await this.userManager.AddClaimAsync(user,
                                    new System.Security.Claims.Claim(
                                        GlobalConstants.ID_CANDIDATE_PROVIDER,
                                        applicationUser.IdCandidateProvider.ToString()));

                                await this.userManager.AddClaimAsync(user,
                                    new System.Security.Claims.Claim(
                                        GlobalConstants.ID_USER,
                                user.IdUser.ToString()));

                                await this.userManager.AddClaimAsync(user,
                                    new System.Security.Claims.Claim(
                                        GlobalConstants.ID_PERSON,
                                        applicationUser.IdPerson.ToString()));

                                await this.userManager.AddClaimAsync(user,
                                  new System.Security.Claims.Claim(
                                      GlobalConstants.PERSON_FULLNAME,
                                     applicationUser.FullName));

                                foreach (var role in model.Roles)
                                {
                                    await this.userManager.AddToRoleAsync(user, role.Name);
                                }
                            }

                            this._logger.LogInformation($"User created an account with UserName:{user.UserName}.");
                            users.Add(new UserModel()
                            {
                                username = user.UserName,
                                password = applicationUser.Password,
                                CandidateProvider = candidate.ProviderOwner,
                                roles = String.Join(", ", model.Roles.Select(x => x.Name)).ToString()
                            });
                        }

                        var candidateProviderPersonForDb = new CandidateProviderPerson()
                        {
                            IdPerson = personForDb.IdPerson,
                            IdCandidate_Provider = idCandidateProvider,
                            IsAdministrator = true,
                            IsAllowedForNotification = true
                        };

                        string emailText = string.Empty;
                        if (model.AllowSentEmails)
                        {
                            emailText = await this.mailService.SendEmailNewRegistrationFromImportUsersForCandidateProvider(new ResultContext<ApplicationUserVM>() { ResultContextObject = applicationUser });
                        }
                        await this.repository.AddAsync<CandidateProviderPerson>(candidateProviderPersonForDb);
                        await this.repository.SaveChangesAsync(false);
                    }

                } 
                else
                {
                    var experts = new List<Expert>();
                    var people = new List<Person>();

                    if (model.IdImportType == kvExternalExpertTypeValue.IdKeyValue)
                        experts = this.repository.AllReadonly<Expert>().Where(x => x.IsExternalExpert).ToList();
                    else if (model.IdImportType == kvNAPOOExpertsTypeValue.IdKeyValue)
                        experts = this.repository.AllReadonly<Expert>().Where(x => x.IsNapooExpert).ToList();

                    var currentUsers = this.userManager.Users.ToList();

                    people = this.repository.AllReadonly<Person>().ToList().Where(x => experts.Any(z => z.IdPerson == x.IdPerson)).ToList();

                    foreach (var expert in experts)
                    {
                        var personFromDb = people.Where(x => x.IdPerson == expert.IdPerson).First();

                        var currentUser = currentUsers.Where(x => x.IdPerson == personFromDb.IdPerson).FirstOrDefault();

                        if (currentUser is not null) continue;

                        var applicationUser = new ApplicationUserVM()
                        {
                            IdPerson = personFromDb.IdPerson,
                            Email = personFromDb.Email is null ? "" : personFromDb.Email,
                            EIK = string.Empty,
                            IdUserStatus = kvUserStatusActive.IdKeyValue,
                            FirstName = personFromDb.FirstName,
                            FamilyName = personFromDb.FamilyName,
                            IdCreateUser = userFromSystem.IdUser,
                            IdModifyUser = userFromSystem.IdUser,
                            CreationDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };

                        var user = this.CreateUser();
                        user.IdPerson = applicationUser.IdPerson;
                        user.Email = applicationUser.Email;
                        applicationUser.Password = this.GenerateRandomPassword();
                        user.UserName = await this.GenerateUserName(applicationUser);
                        user.IdUser = (int)(await this.GetSequenceNextValue("APPLICATION_USER_ID"));
                        user.IdUserStatus = applicationUser.IdUserStatus;
                        user.CreationDate = DateTime.Now;
                        user.ModifyDate = DateTime.Now;
                        user.IdCreateUser = UserProps.UserId;
                        user.IdModifyUser = UserProps.UserId;

                        var result = await this.userManager.CreateAsync(user, applicationUser.Password);
                        applicationUser.UserName = user.UserName;
                        if (result.Succeeded)
                        {
                            await this.userManager.AddClaimAsync(user,
                                new System.Security.Claims.Claim(
                                    GlobalConstants.ID_USER,
                            user.IdUser.ToString()));

                            await this.userManager.AddClaimAsync(user,
                                new System.Security.Claims.Claim(
                                    GlobalConstants.ID_PERSON,
                                    applicationUser.IdPerson.ToString()));

                            await this.userManager.AddClaimAsync(user,
                              new System.Security.Claims.Claim(
                                  GlobalConstants.PERSON_FULLNAME,
                                 applicationUser.FullName));

                            foreach (var role in model.Roles)
                            {
                                await this.userManager.AddToRoleAsync(user, role.Name);
                            }

                            this._logger.LogInformation($"User created an account with UserName:{user.UserName}.");

                            string emailText = string.Empty;
                            if (model.AllowSentEmails)
                            {
                                emailText = await this.mailService.SendEmailNewRegistrationFromImportUsersForCandidateProvider(new ResultContext<ApplicationUserVM>() { ResultContextObject = applicationUser }, true);
                            }

                            users.Add(new UserModel()
                            {
                                username = user.UserName,
                                password = applicationUser.Password,
                                roles = String.Join(", ", model.Roles.Select(x => x.Name)).ToString()
                            });
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                IsSuccessfull = false;
            }

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];
                if (model.IdImportType == kvCPOTypeValue.IdKeyValue || model.IdImportType == kvCIPOTypeValue.IdKeyValue)
                {
                    sheet.Range["A1"].ColumnWidth = 150;
                    sheet.Range["A1"].Text = "Потребители:";
                    sheet.Range["B1"].Text = "Парола:";
                    sheet.Range["C1"].Text = "Роли:";
                    sheet.Range["D1"].Text = "ЦПО/ЦИПО:";
                    int row = 3;
                    foreach (var user in users)
                    {
                        sheet.Range[$"A{row}"].Text = user.username;
                        sheet.Range[$"B{row}"].Text = user.password;
                        sheet.Range[$"C{row}"].Text = user.roles;
                        sheet.Range[$"D{row}"].Text = user.CandidateProvider;
                        row++;
                    }

                    sheet.Range[$"A1:D{row}"].AutofitColumns();
                    sheet.Range[$"A1:D{row}"].BorderInside(ExcelLineStyle.Medium);
                    sheet.Range[$"A1:D{row}"].BorderAround(ExcelLineStyle.Medium);
                }else
                {
                    sheet.Range["A1"].ColumnWidth = 150;
                    sheet.Range["A1"].Text = "Потребители:";
                    sheet.Range["B1"].Text = "Парола:";
                    sheet.Range["C1"].Text = "Роли:";
                    int row = 3;
                    foreach (var user in users)
                    {
                        sheet.Range[$"A{row}"].Text = user.username;
                        sheet.Range[$"B{row}"].Text = user.password;
                        sheet.Range[$"C{row}"].Text = user.roles;
                        row++;
                    }

                    sheet.Range[$"A1:C{row}"].AutofitColumns();
                    sheet.Range[$"A1:C{row}"].BorderInside(ExcelLineStyle.Medium);
                    sheet.Range[$"A1:C{row}"].BorderAround(ExcelLineStyle.Medium);
                }
                workbook.SaveAs(ms);
            }

            return (ms, IsSuccessfull);
        }
    }

class UserModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string CandidateProvider { get; set; }

        public string roles { get; set; }
    }
}
