using Data.Models.Common;
using Data.Models.Data.Control;
using DocuWorkService;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.Archive;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Control;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Control
{
    public class ControlService : BaseService, IControlService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly IDocuService docuService;
        private readonly IUploadFileService uploadFileService;
        private readonly IApplicationUserService applicationUserService;
        private readonly ILogger<ArchiveService> _logger;

        public ControlService(IRepository repository, ILogger<ArchiveService> _logger,
            IDataSourceService dataSourceService, 
            IDocuService docuService, 
            IUploadFileService uploadFileService,
            IApplicationUserService applicationUserService) : base(repository)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this.docuService = docuService;
            this.applicationUserService = applicationUserService;
            this._logger = _logger;
            this.uploadFileService = uploadFileService;
        }

        #region FollowUpControl
        public async Task<int> CreateControlAsync(FollowUpControlVM model)
        {
            var newControl = model.To<FollowUpControl>();

            await this.repository.AddAsync<FollowUpControl>(newControl);
            var result = await this.repository.SaveChangesAsync();

            return result;
        }

        public async Task<IEnumerable<FollowUpControlVM>> GetAllControlsAsync(string followUpControlType)
        {
            var kvFollowUpControlTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FollowUpControlType");
            var kvControlStatusesSource = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FollowUpControlStatus");
            var kvControlTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ControlType");
            var kvLicenseType = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", followUpControlType);
            var data = this.repository.All<FollowUpControl>().Include(c => c.CandidateProvider).Include(c => c.CandidateProvider.LocationCorrespondence);
            var dataVM = await data.To<FollowUpControlVM>(c => c.CandidateProvider.LocationCorrespondence, c => c.FollowUpControlExperts).Where(c => c.CandidateProvider.IdTypeLicense == kvLicenseType.IdKeyValue).ToListAsync();
            foreach (var item in dataVM)
            {
                item.FollowUpControlTypeName = kvFollowUpControlTypeSource.FirstOrDefault(c => c.IdKeyValue == item.IdFollowUpControlType).Name;
                item.ControlTypeName = kvControlTypeSource.FirstOrDefault(c => c.IdKeyValue == item.IdControlType).Name;
                item.StatusName = kvControlStatusesSource.FirstOrDefault(c => c.IdKeyValue == item.IdStatus.Value).Name;
            }

            return dataVM;
        }
        public async Task<IEnumerable<FollowUpControlVM>> GetAllControlsByIdCandidateProviderAsync(int idCandidateProvider, string licensingType)
        {
            var kvFollowUpControlTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FollowUpControlType");
            var kvControlStatusesSource = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FollowUpControlStatus")).Where(k => k.KeyValueIntCode == "Current" || k.KeyValueIntCode == "Finished").ToList();
            var kvCurrent = kvControlStatusesSource.FirstOrDefault(k => k.KeyValueIntCode == "Current" ).IdKeyValue;
            var kvFinished = kvControlStatusesSource.FirstOrDefault(k => k.KeyValueIntCode == "Finished").IdKeyValue;
            var kvControlTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ControlType");
            var kvLicenseType = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", licensingType);
            var data = this.repository.All<FollowUpControl>(f => f.IdCandidateProvider == idCandidateProvider && (f.IdStatus == kvCurrent || f.IdStatus == kvFinished)).Include(c => c.CandidateProvider).Include(c => c.CandidateProvider.LocationCorrespondence).Include(x => x.FollowUpControlDocuments); ;
            var dataVM = await data.To<FollowUpControlVM>(c => c.CandidateProvider.LocationCorrespondence, c => c.FollowUpControlExperts, x => x.FollowUpControlDocuments).Where(c => c.CandidateProvider.IdTypeLicense == kvLicenseType.IdKeyValue).ToListAsync();
            foreach (var item in dataVM)
            {
                item.FollowUpControlTypeName = kvFollowUpControlTypeSource.FirstOrDefault(c => c.IdKeyValue == item.IdFollowUpControlType).Name;
                item.ControlTypeName = kvControlTypeSource.FirstOrDefault(c => c.IdKeyValue == item.IdControlType).Name;
                item.StatusName = kvControlStatusesSource.FirstOrDefault(c => c.IdKeyValue == item.IdStatus.Value).Name;
            }

            return dataVM;
        }

        public async Task<FollowUpControlVM> GetControlByIdFollowUpControlAsync(int id)
        {
            var kvFollowUpControlTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FollowUpControlType");
            var kvControlTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ControlType");
            var control = await this.repository.AllReadonly<FollowUpControl>(e => e.IdFollowUpControl == id)
                .Include(c => c.CandidateProvider.LocationCorrespondence)
                .FirstOrDefaultAsync();

            this.repository.Detach<FollowUpControl>(control);

            FollowUpControlVM result =  control.To<FollowUpControlVM>();
            result.FollowUpControlTypeName = kvFollowUpControlTypeSource.FirstOrDefault(c => c.IdKeyValue == result.IdFollowUpControlType).Name;
            result.ControlTypeName = kvControlTypeSource.FirstOrDefault(c => c.IdKeyValue == result.IdControlType).Name;
            return result;
        }

        public async Task<ResultContext<FollowUpControlVM>> UpdateFollowUpControlAsync(ResultContext<FollowUpControlVM> resultContext)
        {
            int result = 0;
            if (resultContext.ResultContextObject.IdFollowUpControl == GlobalConstants.INVALID_ID_ZERO)
            {
                var newControl = resultContext.ResultContextObject.To<FollowUpControl>();

                await this.repository.AddAsync<FollowUpControl>(newControl);
                result = await this.repository.SaveChangesAsync();
                resultContext.ResultContextObject.IdFollowUpControl = newControl.IdFollowUpControl;

            }
            else
            {
                var updatedEntity = await this.GetByIdAsync<FollowUpControl>(resultContext.ResultContextObject.IdFollowUpControl);
                updatedEntity = resultContext.ResultContextObject.To<FollowUpControl>();
                this.repository.Detach<FollowUpControl>(updatedEntity);
                this.repository.Update(updatedEntity);

                result = await this.repository.SaveChangesAsync();
            }
            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }
            return resultContext;
        }
        #endregion

        #region FollowUpControlExpert
        public async Task<IEnumerable<FollowUpControlExpertVM>> GetAllControlExpertsByIdFollowUpControlAsync(int id)
        {
            IQueryable<FollowUpControlExpert> controlExperts = this.repository.All<FollowUpControlExpert>(e => e.IdFollowUpControl == id);

            var result = await controlExperts.To<FollowUpControlExpertVM>(c => c.Expert.Person).ToListAsync();

            return result;
        }

        public async Task<ResultContext<FollowUpControlExpertVM>> AddControlExpertAsync(FollowUpControlExpertVM controlExpert)
        {
            var newControlExpert = controlExpert.To<FollowUpControlExpert>();

            await this.repository.AddAsync<FollowUpControlExpert>(newControlExpert);
            var result = await this.repository.SaveChangesAsync();

            ResultContext<FollowUpControlExpertVM> resultContext = new ResultContext<FollowUpControlExpertVM>();
            resultContext.ResultContextObject = newControlExpert.To<FollowUpControlExpertVM>();
            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }
            return resultContext;
        }
        public async Task<ResultContext<FollowUpControlExpertVM>> DeleteControlExpertAsync(ResultContext<FollowUpControlExpertVM> resultContext)
        {
            await this.repository.HardDeleteAsync<FollowUpControlExpert>(resultContext.ResultContextObject.IdFollowUpControlExpert);

            var result = await this.repository.SaveChangesAsync();

            if (result > 0)
            {
                resultContext.AddMessage("Записът е изтрит успешно!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при изтриване в базата!");
            }

            return resultContext;
        }
        #endregion

        #region FollowUpControlDocument
        public async Task<int> SaveControlDocument(FollowUpControlDocumentVM followUpControlDocumentVM)
        {
            var createdEntity = followUpControlDocumentVM.To<FollowUpControlDocument>();
            await this.repository.AddAsync(createdEntity);
            await this.repository.SaveChangesAsync();
            return createdEntity.IdFollowUpControlDocument;
        }
        public async Task<int> UpdateControlDocument(FollowUpControlDocumentVM model)
        {
            var followUpControlDocumentFromDb = await this.GetByIdAsync<FollowUpControlDocument>(model.IdFollowUpControlDocument);
            followUpControlDocumentFromDb.DS_OFFICIAL_ID = model.DS_OFFICIAL_ID;
            followUpControlDocumentFromDb.DS_OFFICIAL_GUID = model.DS_OFFICIAL_GUID;
            followUpControlDocumentFromDb.DS_OFFICIAL_DATE = model.DS_OFFICIAL_DATE;
            followUpControlDocumentFromDb.DS_OFFICIAL_DocNumber = model.DS_OFFICIAL_DocNumber;
            this.repository.Detach<FollowUpControlDocument>(followUpControlDocumentFromDb);
            this.repository.Update(followUpControlDocumentFromDb);
            
            return await this.repository.SaveChangesAsync();
        }
        public async Task<IEnumerable<FollowUpControlDocumentVM>> GetAllDocumentsAsync(int idFollowUpControl)
        {
            IQueryable<FollowUpControlDocument> documents = this.repository.All<FollowUpControlDocument>(e => e.IdFollowUpControl == idFollowUpControl);

            var result = await documents.To<FollowUpControlDocumentVM>().ToListAsync();

            var kvDocumentType = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
            foreach (var item in result)
            {
                item.DocumentTypeName = kvDocumentType.FirstOrDefault(d => d.IdKeyValue == item.IdDocumentType).Description;
            }

            return result;
        }

        public async Task<ResultContext<NoResult>> SaveNewControlDocumentByNumberAndDate(FollowUpControlDocumentVM doc)
        {
            var resultContext = new ResultContext<NoResult>();

            var documentData = await docuService.GetIdentDocument(doc.ApplicationNumber, 0, doc.ApplicationDate.Value);

            var contextResponse = await this.docuService.GetDocumentAsync(documentData.DocIdent.First().DocID, documentData.DocIdent.First().GUID);

            var kvDocumentType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType"))
                .Where(x => !string.IsNullOrEmpty(x.DefaultValue6));

            if (contextResponse.HasErrorMessages)
            {
                resultContext.ListErrorMessages = contextResponse.ListErrorMessages;

                return resultContext;
            }

            var documentResponse = contextResponse.ResultContextObject;

            doc.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
            doc.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
            doc.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
            doc.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;

            doc.IdDocumentType = kvDocumentType.Where(x => Int32.Parse(x.DefaultValue6) == documentResponse.Doc.DocVidCode).First().IdKeyValue;

            await repository.AddAsync(doc.To<FollowUpControlDocument>());
            await repository.SaveChangesAsync();

            return resultContext;
        }

        public async Task<bool> DeleteControlDocumentbyId(int id)
        {
            try
            {
                await this.repository.HardDeleteAsync<FollowUpControlDocument>(id);
                await this.repository.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                return false;
            }
        }
        public async Task<FollowUpControlDocumentVM> GetFollowUpControlDocumentByIdAsync(FollowUpControlDocumentVM followUpControlDocument)
        {
            IQueryable<FollowUpControlDocument> controlDocuments = this.repository.AllReadonly<FollowUpControlDocument>(x => x.IdFollowUpControlDocument == followUpControlDocument.IdFollowUpControlDocument);

            return await controlDocuments.To<FollowUpControlDocumentVM>().FirstOrDefaultAsync();
        }
        #endregion

        #region FollowUpControlUploadedFile
        public async Task<IEnumerable<FollowUpControlUploadedFileVM>> GetAllUploadedFilesByIdFollowUpControl(int id)
        {
            IQueryable<FollowUpControlUploadedFile> controlExperts = this.repository.All<FollowUpControlUploadedFile>(e => e.IdFollowUpControl == id);

            var result = await controlExperts.To<FollowUpControlUploadedFileVM>().ToListAsync();
            foreach (var doc in result)
            {
                doc.CreatePersonName = await this.applicationUserService.GetApplicationUsersPersonNameAsync(doc.IdCreateUser);
            }

            return result;
        }

        public async Task<ResultContext<FollowUpControlUploadedFileVM>> CreateFollowUpControlUploadedFileAsync(FollowUpControlUploadedFileVM uploadedFile)
        {
            var newUploadedFile = uploadedFile.To<FollowUpControlUploadedFile>();
            await this.repository.AddAsync<FollowUpControlUploadedFile>(newUploadedFile);
            var result = await this.repository.SaveChangesAsync();

            ResultContext<FollowUpControlUploadedFileVM> resultContext = new ResultContext<FollowUpControlUploadedFileVM>();
            resultContext.ResultContextObject = newUploadedFile.To<FollowUpControlUploadedFileVM>();
            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }
            return resultContext;
        }

        public async Task<ResultContext<FollowUpControlUploadedFileVM>> UpdateFollowUpControlUploadedFileAsync(FollowUpControlUploadedFileVM model)
        {
            var resultContext = new ResultContext<FollowUpControlUploadedFileVM>();
            int result = 0;
            
                var updatedEnity = await this.GetByIdAsync<FollowUpControlUploadedFile>(model.IdFollowUpControlUploadedFile);
                updatedEnity = model.To<FollowUpControlUploadedFile>();
                this.repository.Detach<FollowUpControlUploadedFile>(updatedEnity);
                this.repository.Update(updatedEnity);

                result = await this.repository.SaveChangesAsync();
            resultContext.ResultContextObject = updatedEnity.To<FollowUpControlUploadedFileVM>();
            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }
            return resultContext;
        }
        public async Task<FollowUpControlUploadedFileVM> GetFollowUpControlUploadedFileById(int id)
        {
            var data = await this.repository.GetByIdAsync<FollowUpControlUploadedFile>(id);

            var resultVM = data.To<FollowUpControlUploadedFileVM>();

            return resultVM;
        }
        public async Task<ResultContext<FollowUpControlUploadedFileVM>> DeleteFollowUpControlUploadedFileAsync(FollowUpControlUploadedFileVM followUpControlUploadedFileVM)
        {
            var entity = await this.repository.GetByIdAsync<FollowUpControlUploadedFile>(followUpControlUploadedFileVM.IdFollowUpControlUploadedFile);
            this.repository.Detach<FollowUpControlUploadedFile>(entity);

            ResultContext<FollowUpControlUploadedFileVM> resultContext = new ResultContext<FollowUpControlUploadedFileVM>();

            try
            {
                this.repository.HardDelete<FollowUpControlUploadedFile>(entity);
                await this.repository.SaveChangesAsync();

                var settingsFolder = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var pathToFile = settingsFolder + "\\" + entity.UploadedFileName;
                if (!string.IsNullOrEmpty(entity.UploadedFileName))
                {
                    if (Directory.Exists(pathToFile))
                    {
                        Directory.Delete(pathToFile, true);
                    }
                }

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

        #endregion
    }
}
