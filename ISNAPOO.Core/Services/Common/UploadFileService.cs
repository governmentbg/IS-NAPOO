using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Training;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class UploadFileService : BaseService, IUploadFileService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly ILogger<UploadFileService> _logger;

        public UploadFileService(IRepository repository, 
            IDataSourceService dataSourceService,
            ILogger<UploadFileService> logger) : base(repository)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this._logger = logger;
        }

        #region Base CRUD methods
        public async Task<bool> CheckIfExistUploadedFileAsync<T>(int idEntity, string? fileName = null) where T : AbstractUploadFile
        {
            var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var updatedEnity = await this.repository.GetByIdAsync<T>(idEntity);

            if (updatedEnity.UploadedFileName == "Отвори документа")
            {
                return true;
            }
            else
            {
                string fileFullName = string.Empty;
                if (string.IsNullOrEmpty(fileName))
                {
                    fileFullName = settingResource + "\\" + updatedEnity.UploadedFileName;
                }
                else if (updatedEnity is Notification)
                {
                    fileFullName = settingResource + "\\" + "UploadedFiles" + "\\" + "TSA" + "\\" + "Notifications" + "\\" + idEntity.ToString() + "\\" + fileName;
                }
                else
                {
                    fileFullName = settingResource + "\\" + updatedEnity.UploadedFileName + fileName;
                }

                if (File.Exists(fileFullName))
                {
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<(MemoryStream? MS, string FileNameFromOldIS)> GetUploadedFileAsync<T>(int idEntity, string? fileName = null) where T : AbstractUploadFile
        {
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var updatedEnity = await this.repository.GetByIdAsync<T>(idEntity);

                if (updatedEnity.UploadedFileName == "Отвори документа")
                {
                    long oid = 0;
                    if (updatedEnity is CourseDocumentUploadedFile)
                    {
                        var castedEntity = updatedEnity as CourseDocumentUploadedFile;
                        oid = long.Parse(castedEntity.Oid);
                    }
                    else if (updatedEnity is CourseDocumentUploadedFile)
                    {
                        var castedEntity = updatedEnity as ValidationDocumentUploadedFile;
                        oid = long.Parse(castedEntity.Oid);
                    }
                    else
                    {
                        oid = long.Parse(updatedEnity.MigrationNote);
                    }

                    var linkSetting = (await this.dataSourceService.GetSettingByIntCodeAsync("NapooOldISDocsLink"));

                    System.Net.HttpWebRequest request = null;
                    System.Net.HttpWebResponse response = null;
                    request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create($"{linkSetting.SettingValue}{oid}");
                    request.Timeout = 30000;
                    request.UserAgent = ".NET Client";
                    response = (System.Net.HttpWebResponse)request.GetResponse();
                    var s = response.GetResponseStream();

                    var unencodedFileNameFromOldIS = response.Headers["Content-Disposition"];

                    unencodedFileNameFromOldIS = response.Headers["Content-Disposition"].Replace("attachment; filename=", String.Empty).Replace("\"", String.Empty);

                    var bytes = Encoding.UTF8.GetBytes(unencodedFileNameFromOldIS);

                    var encodedFileNameFromOldIS = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("ISO-8859-1"), bytes);
                    var fileNameFromOldIS = Encoding.UTF8.GetString(encodedFileNameFromOldIS);

                    var fileTempPath = settingResource + "\\" + "UploadedFiles" + "\\" + "TempFromOldIS";
                    if (!Directory.Exists(fileTempPath))
                    {
                        Directory.CreateDirectory(fileTempPath);
                    }

                    var guidedFileName = Guid.NewGuid() + fileNameFromOldIS;
                    var fileTempFullName = fileTempPath + "\\" + guidedFileName;
                    FileStream os = new FileStream(fileTempFullName, FileMode.Create, FileAccess.Write);
                    byte[] buff = new byte[102400000];
                    int c = 0;
                    while ((c = s.Read(buff, 0, 10400)) > 0)
                    {
                        os.Write(buff, 0, c);
                        os.Flush();
                    }

                    os.Close();
                    s.Close();

                    MemoryStream ms = new MemoryStream();
                    if (File.Exists(fileTempFullName))
                    {
                        using (ms = new MemoryStream())
                        {
                            using (FileStream file = new FileStream(fileTempFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                byte[] bytesArr = new byte[file.Length];
                                file.Read(bytesArr, 0, (int)file.Length);
                                ms.Write(bytesArr, 0, (int)file.Length);
                            }
                        }

                        File.Delete(fileTempFullName);
                    }

                    return (ms, fileNameFromOldIS);
                }
                else
                {
                    string fileFullName = string.Empty;
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileFullName = settingResource + "\\" + updatedEnity.UploadedFileName;
                    }
                    else if (updatedEnity is Notification)
                    {
                        fileFullName = settingResource + "\\" + "UploadedFiles" + "\\" + "TSA" + "\\" + "Notifications" + "\\" + idEntity + "\\" + fileName;
                    }
                    else
                    {
                        fileFullName = settingResource + "\\" + updatedEnity.UploadedFileName + fileName;
                    }

                    MemoryStream ms = new MemoryStream();
                    if (File.Exists(fileFullName))
                    {
                        using (ms = new MemoryStream())
                        {
                            using (FileStream file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                byte[] bytes = new byte[file.Length];
                                file.Read(bytes, 0, (int)file.Length);
                                ms.Write(bytes, 0, (int)file.Length);
                            }
                        }
                    }

                    return (ms, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return (null, string.Empty);
            }
        }

        public async Task<string> UploadFileAsync<T>(MemoryStream file, string fileName, string folderName, int idEntity) where T : AbstractUploadFile
        {
            var pathToFile = string.Empty;
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\{folderName}\\{idEntity}";
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

                if (idEntity != 0)
                {
                    var updatedEnity = await this.repository.GetByIdAsync<T>(idEntity);

                    updatedEnity.UploadedFileName = filePathMain + "\\" + fileName;

                    this.repository.Update(updatedEnity);
                    await this.repository.SaveChangesAsync();

                    pathToFile = updatedEnity.UploadedFileName;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return pathToFile;
        }

        public async Task<int> RemoveFileByIdAsync<T>(int idEntity) where T : AbstractUploadFile
        {
            int result = 0;
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;

                var updatedEnity = await this.repository.GetByIdAsync<T>(idEntity);
                var fileFullName = settingResource + "\\" + updatedEnity.UploadedFileName;
                if (File.Exists(fileFullName))
                {
                    File.Delete(fileFullName);
                }

                updatedEnity.UploadedFileName = "";

                this.repository.Update(updatedEnity);

                result = await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return result;
        }

        public async Task<int> RemoveFileByIdAndFileNameAsync<T>(int idEntity, string fileName) where T : AbstractUploadFile
        {
            int result = 0;
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;

                var updatedEnity = await this.repository.GetByIdAsync<T>(idEntity);

                var fileFullName = settingResource + "\\" + updatedEnity.UploadedFileName + fileName;
                if (File.Exists(fileFullName))
                {
                    File.Delete(fileFullName);
                }

                var pathToFilesFolder = updatedEnity.UploadedFileName.StartsWith("\\")
                    ? settingResource + updatedEnity.UploadedFileName
                    : settingResource + "\\" + updatedEnity.UploadedFileName;
                if (Directory.GetFiles(pathToFilesFolder).Length == 0)
                {
                    updatedEnity.UploadedFileName = "";
                    this.repository.Update(updatedEnity);
                    result = await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return result;
        }
        #endregion

        #region Candidate Provider
        public async Task<ResultContext<CandidateProviderTrainerDocumentVM>> UploadFileCandidateProviderTrainerDocumentAsync(MemoryStream[] files, string fileName, string folderName, int idEntity)
        {
            ResultContext<CandidateProviderTrainerDocumentVM> resultContext = new ResultContext<CandidateProviderTrainerDocumentVM>();

            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\{folderName}\\{idEntity}";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var counter = 0;
                foreach (var file in files)
                {
                    var nameFile = fileName.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[counter++];
                    var path = @"" + filePath + "\\" + nameFile;

                    using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        file.WriteTo(filestream);
                        filestream.Close();
                        file.Close();
                    }
                }

                CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM = new CandidateProviderTrainerDocumentVM();

                candidateProviderTrainerDocumentVM.UploadedFileName = filePathMain + "\\";
                candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument = idEntity;

                await this.UpdateTrainerDocumentFileNameAsync(candidateProviderTrainerDocumentVM);

                resultContext.ResultContextObject = candidateProviderTrainerDocumentVM;
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public async Task<ResultContext<CandidateProviderPremisesDocumentVM>> UploadFileCandidateProviderPremisesDocumentAsync(MemoryStream[] files, string fileName, string folderName, int idEntity)
        {
            ResultContext<CandidateProviderPremisesDocumentVM> resultContext = new ResultContext<CandidateProviderPremisesDocumentVM>();

            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\{folderName}\\{idEntity}";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var counter = 0;
                foreach (var file in files)
                {
                    var nameFile = fileName.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[counter++];
                    var path = @"" + filePath + "\\" + nameFile;

                    using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        file.WriteTo(filestream);
                        filestream.Close();
                        file.Close();
                    }
                }

                CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM = new CandidateProviderPremisesDocumentVM();

                candidateProviderPremisesDocumentVM.UploadedFileName = filePathMain + "\\";
                candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument = idEntity;

                await this.UpdatePremisesDocumentFileNameAsync(candidateProviderPremisesDocumentVM);

                resultContext.ResultContextObject = candidateProviderPremisesDocumentVM;
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public async Task<ResultContext<CandidateProviderDocumentVM>> UploadFileCandidateProviderDocumentAsync(MemoryStream[] files, string fileName, string folderName, int idEntity)
        {
            ResultContext<CandidateProviderDocumentVM> resultContext = new ResultContext<CandidateProviderDocumentVM>();

            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\{folderName}\\{idEntity}";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var counter = 0;
                foreach (var file in files)
                {
                    var nameFile = fileName.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[counter++];
                    var path = @"" + filePath + "\\" + nameFile;

                    using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        file.WriteTo(filestream);
                        filestream.Close();
                        file.Close();
                    }
                }

                CandidateProviderDocumentVM candidateProviderDocumentVM = new CandidateProviderDocumentVM();

                candidateProviderDocumentVM.UploadedFileName = filePathMain + "\\";
                candidateProviderDocumentVM.IdCandidateProviderDocument = idEntity;

                await this.UpdateCandidateProviderDocumentFileNameAsync(candidateProviderDocumentVM);

                resultContext.ResultContextObject = candidateProviderDocumentVM;
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public async Task UploadFormularCandidateProviderDocumentAsync(MemoryStream file, string fileName, string folderName, int idEntity, int IdType)
        {
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\{folderName}\\{idEntity}";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var nameFile = fileName.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[0];
                var path = @"" + filePath + "\\" + nameFile;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }


                CandidateProviderDocumentVM candidateProviderDocumentVM = new CandidateProviderDocumentVM();

                candidateProviderDocumentVM.UploadedFileName = filePathMain + "\\";
                candidateProviderDocumentVM.IdCandidateProviderDocument = idEntity;
                candidateProviderDocumentVM.IdDocumentType = IdType;
                candidateProviderDocumentVM.DocumentTitle = "Формуляр(Организация на работа)";

                var entityForDb = candidateProviderDocumentVM.To<CandidateProviderDocument>();
                entityForDb.IdCandidateProviderDocument = 0;
                entityForDb.CandidateProvider = null;
                entityForDb.IdCandidateProvider = idEntity;


                var db = this.repository.All<CandidateProviderDocument>().ToList().Where(x => x.IdCandidateProvider == idEntity && x.IdDocumentType == IdType).ToList().FirstOrDefault();
                if (db != null)
                {
                    db.IdDocumentType = entityForDb.IdDocumentType;
                    db.UploadedFileName = entityForDb.UploadedFileName;
                    db.DocumentTitle = entityForDb.DocumentTitle;
                    this.repository.Update(db);
                    await this.repository.SaveChangesAsync();
                }
                else
                {
                    await this.repository.AddAsync<CandidateProviderDocument>(entityForDb);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<int> UploadFileESignedApplicationAsync(MemoryStream file, CandidateProviderVM candidateProvider)
        {
            try
            {
                int result = 0;
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\CandidateProvider\\{candidateProvider.IdCandidate_Provider}";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + candidateProvider.ESignApplicationFileName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }

                if (candidateProvider.IdCandidate_Provider != 0)
                {
                    var updatedEnity = await this.repository.GetByIdAsync<CandidateProvider>(candidateProvider.IdCandidate_Provider);

                    updatedEnity.ESignApplicationFileName = path;

                    this.repository.Update(updatedEnity);
                    result = await this.repository.SaveChangesAsync();
                }

                return result;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> RemoveESignedApplicationFileByNameAsync(int idEntity, string fileName)
        {
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;

                var updatedEnity = await this.repository.GetByIdAsync<CandidateProvider>(idEntity);

                var fileFullName = settingResource + "\\" + "UploadedFiles" + "\\" + "CandidateProvider" + "\\" + idEntity + "\\" + fileName;

                if (File.Exists(fileFullName))
                {
                    File.Delete(fileFullName);
                }

                updatedEnity.ESignApplicationFileName = null;

                this.repository.Update<CandidateProvider>(updatedEnity);
                await this.repository.SaveChangesAsync();

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<MemoryStream> GetUploadedFileESignedApplicationAsync(CandidateProviderVM candidateProvider)
        {
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;

                string fileFullName = settingResource + "\\" + candidateProvider.ESignApplicationFileName;

                MemoryStream ms = new MemoryStream();
                if (File.Exists(fileFullName))
                {
                    using (ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);
                        }
                    }
                }

                return ms;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<MemoryStream> GetCurriculumTemplate()
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\Application";
            var fileFullName = $"{resources_Folder}\\Uchebna-programa-CPO.xlsx";

            MemoryStream ms = new MemoryStream();
            if (File.Exists(fileFullName))
            {
                using (ms = new MemoryStream())
                {
                    using (FileStream file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);
                    }
                }
            }

            return ms;
        }

        private async Task UpdateTrainerDocumentFileNameAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM)
        {
            var candidateProviderTrainerDocumentFromDb = await this.repository.GetByIdAsync<CandidateProviderTrainerDocument>(candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument);
            candidateProviderTrainerDocumentFromDb.UploadedFileName = candidateProviderTrainerDocumentVM.UploadedFileName;

            this.repository.Update<CandidateProviderTrainerDocument>(candidateProviderTrainerDocumentFromDb);
            await this.repository.SaveChangesAsync();
        }

        private async Task UpdatePremisesDocumentFileNameAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
        {
            var candidateProviderPremisesDocumentFromDb = await this.repository.GetByIdAsync<CandidateProviderPremisesDocument>(candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument);
            candidateProviderPremisesDocumentFromDb.UploadedFileName = candidateProviderPremisesDocumentVM.UploadedFileName;

            this.repository.Update<CandidateProviderPremisesDocument>(candidateProviderPremisesDocumentFromDb);
            await this.repository.SaveChangesAsync();
        }

        private async Task UpdateCandidateProviderDocumentFileNameAsync(CandidateProviderDocumentVM candidateProviderDocumentVM)
        {
            var candidateProviderDocumentFromDb = await this.repository.GetByIdAsync<CandidateProviderDocument>(candidateProviderDocumentVM.IdCandidateProviderDocument);
            candidateProviderDocumentFromDb.UploadedFileName = candidateProviderDocumentVM.UploadedFileName;

            this.repository.Update<CandidateProviderDocument>(candidateProviderDocumentFromDb);
            await this.repository.SaveChangesAsync();
        }
        #endregion

        #region Validation
        public async Task<MemoryStream> GetValidationClientCurriculumUploadedFileAsync(int idValidationClient)
        {
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var validationClient = await this.repository.GetByIdAsync<ValidationClient>(idValidationClient);
                var fileFullName = settingResource + "\\" + validationClient.UploadedCurriculumFileName;
                MemoryStream ms = new MemoryStream();
                if (File.Exists(fileFullName))
                {
                    using (ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);
                        }
                    }
                }

                return ms;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Timestamp
        public async Task UploadTimeStampFilesAsync(MemoryStream timeStampResponse, string timeStampResponseName, string notificationTextName, string notificationText, int idNotification)
        {
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\TSA\\Notifications\\{idNotification}";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + timeStampResponseName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    timeStampResponse.WriteTo(filestream);
                    filestream.Close();
                    timeStampResponse.Close();
                }

                path = @"" + filePath + "\\" + notificationTextName;
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(notificationText);
                writer.Flush();
                stream.Position = 0;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    stream.WriteTo(filestream);
                    filestream.Close();
                    stream.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Procedure
        public async Task<ResultContext<ProcedureExternalExpertVM>> UploadFileProcedureExternalExpertAsync(MemoryStream file, string fileName, string folderName, int idEntity)
        {
            ResultContext<ProcedureExternalExpertVM> resultContext = new ResultContext<ProcedureExternalExpertVM>();

            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\{folderName}\\{idEntity}";
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

                var modelForDb = await this.repository.GetByIdAsync<ProcedureExternalExpert>(idEntity);
                modelForDb.UploadedFileName = filePathMain + "\\" + fileName;
                modelForDb.UploadDate = DateTime.Now;

                this.repository.Update<ProcedureExternalExpert>(modelForDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject = modelForDb.To<ProcedureExternalExpertVM>();
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }
        #endregion

        #region NSI Report
        public async Task<MemoryStream> GetReportNSIZipFile(int year)
        {
            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;

                string fileFullName = settingResource + "\\" + "ReportNSI" + "\\" + year + "\\" + $"Spravka_{year}.zip";

                MemoryStream ms = new MemoryStream();
                if (File.Exists(fileFullName))
                {
                    using (ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);
                        }
                    }
                }

                return ms;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task UploadFileReportNSI(AnnualReportNSIVM annualReport)
        {

            try
            {
                var pathSetting = await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName");

                var path = $"{pathSetting.SettingValue}\\ReportNSI\\{annualReport.Year}";

                bool exists = System.IO.Directory.Exists(path);

                if (!exists)
                    System.IO.Directory.CreateDirectory(path);
                var fullPath = $"{path}\\{annualReport.FileName}";
                FileStream os = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);

                annualReport.memoryZipFile.WriteTo(os);

                os.Close();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region FollowUpControl
        public async Task<ResultContext<FollowUpControlDocumentVM>> UploadFileFollowUpControlAdditionalDocumentAsync(MemoryStream[] files, string fileName, string folderName, int idEntity)
        {
            ResultContext<FollowUpControlDocumentVM> resultContext = new ResultContext<FollowUpControlDocumentVM>();

            try
            {
                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\{folderName}\\{idEntity}";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var counter = 0;
                foreach (var file in files)
                {
                    var nameFile = fileName.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[counter++];
                    var path = @"" + filePath + "\\" + nameFile;

                    using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        file.WriteTo(filestream);
                        filestream.Close();
                        file.Close();
                    }
                }

                FollowUpControlDocumentVM followUpControlDocument = new FollowUpControlDocumentVM();

                followUpControlDocument.UploadedFileName = filePathMain + "\\";
                followUpControlDocument.IdFollowUpControlDocument = idEntity;

                await this.UpdateFollowUpControlDocumentFileNameAsync(followUpControlDocument);

                resultContext.ResultContextObject = followUpControlDocument;
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        private async Task UpdateFollowUpControlDocumentFileNameAsync(FollowUpControlDocumentVM followUpControlDocument)
        {
            var followUpControlDocumentFromDb = await this.repository.GetByIdAsync<FollowUpControlDocument>(followUpControlDocument.IdFollowUpControlDocument);
            followUpControlDocumentFromDb.UploadedFileName = followUpControlDocument.UploadedFileName;

            this.repository.Update<FollowUpControlDocument>(followUpControlDocumentFromDb);
            await this.repository.SaveChangesAsync();
        }

        #endregion
    }
}
