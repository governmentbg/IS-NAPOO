using Data.Models.Common;
using Data.Models.Data.ExternalExpertCommission;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.ExternalExpertCommission
{
    public class ExpertDocumentService : BaseService, IExpertDocumentService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly IUploadFileService uploadFileService;

        public ExpertDocumentService(IRepository repository, IDataSourceService dataSourceService, IUploadFileService uploadFileService) : base(repository)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this.uploadFileService = uploadFileService;
        }

        public async Task<ExpertDocumentVM> GetExpertDocumentByIdAsync(int id)
        {
            ExpertDocument expertDocument = await this.repository.GetByIdAsync<ExpertDocument>(id);
            this.repository.Detach<ExpertDocument>(expertDocument);

            return expertDocument.To<ExpertDocumentVM>();
        }

        public async Task<IEnumerable<ExpertDocumentVM>> GetExpertDocumentsByIdsAsync(List<int> ids)
        {
            IQueryable<ExpertDocument> expertDocuments = this.repository.All<ExpertDocument>(this.FilterExpertDocumentByIds(ids));

            return await expertDocuments.To<ExpertDocumentVM>().ToListAsync();
        }

        public async Task<IEnumerable<ExpertDocumentVM>> GetExpertDocumentsByExpertIdAsync(int idExpert)
        {
            IQueryable<ExpertDocument> expertDocuments = this.repository.All<ExpertDocument>(ed => ed.IdExpert == idExpert);

            var expertDocumentVMs = await expertDocuments.To<ExpertDocumentVM>().ToListAsync();

            var dataKvExpertDocumentType = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertDocumentType");

            foreach (ExpertDocumentVM expertDocumentVM in expertDocumentVMs)
            {
                var kvExpertDocumentType = dataKvExpertDocumentType.Where(w => w.IdKeyValue == expertDocumentVM.IdDocumentType).FirstOrDefault();
                if (kvExpertDocumentType != null)
                {
                    expertDocumentVM.DocumentTypeName = kvExpertDocumentType.Name;
                }
            }

            return expertDocumentVMs;
        }

        public async Task<int> CreateExpertDocumentAndUpload(ExpertDocumentVM model, MemoryStream file, string fileName)
        {
            var newExpertDocument = model.To<ExpertDocument>();


            await this.repository.AddAsync<ExpertDocument>(newExpertDocument);
            var result = await this.repository.SaveChangesAsync();

            if (result == 1)
            {
                var resultUpload = await this.uploadFileService.UploadFileAsync<ExpertDocument>(file, fileName, "ExpertDocument", newExpertDocument.IdExpertDocument);
                if (!string.IsNullOrEmpty(resultUpload))
                {
                    result = newExpertDocument.IdExpertDocument;
                }
                else
                {
                    result = 0;
                }
            }

            return result;
        }

        public async Task<IEnumerable<ExpertDocumentVM>> GetAllExpertDocumentsAsync()
        {
            var data = this.repository.All<ExpertDocument>();

            return await data.To<ExpertDocumentVM>().ToListAsync();
        }

        public async Task<IEnumerable<ExpertDocumentVM>> GetAllExpertDocumentsAsync(ExpertDocumentVM modelFilter)
        {
            var data = this.repository.All<ExpertDocument>(this.FilterExpertDocumentValue(modelFilter));

            return await data.To<ExpertDocumentVM>().ToListAsync();
        }

        protected Expression<Func<ExpertDocument, bool>> FilterExpertDocumentValue(ExpertDocumentVM modelFilter)
        {
            var predicate = PredicateBuilder.True<ExpertDocument>();

            if (modelFilter.IdDocumentType != GlobalConstants.INVALID_ID
                && modelFilter.IdDocumentType != GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdDocumentType == modelFilter.IdDocumentType);
            }

            return predicate;
        }

        protected Expression<Func<ExpertDocument, bool>> FilterExpertDocumentByIds(List<int> ids)
        {
            var predicate = PredicateBuilder.True<ExpertDocument>();

            predicate = predicate.And(n => ids.Contains(n.IdExpertDocument));

            return predicate;
        }

        public async Task<int> UpdateExpertDocumentAndUploadAsync(ExpertDocumentVM model, MemoryStream file, string fileName)
        {
            if (model.IdExpertDocument == 0)
            {
                return await this.CreateExpertDocumentAndUpload(model, file, fileName);
            }
            else
            {
                var updatedEnity = await this.GetByIdAsync<ExpertDocument>(model.IdExpertDocument);
                this.repository.Detach<ExpertDocument>(updatedEnity);

                updatedEnity = model.To<ExpertDocument>();


                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();

                if (result == 1)
                {
                    var resultUpload = await this.uploadFileService.UploadFileAsync<ExpertDocument>(file, fileName, "ExpertDocument", updatedEnity.IdExpertDocument);
                    if (string.IsNullOrEmpty(resultUpload))
                    {
                        result = 0;
                    }
                }

                return result;
            }
        }

        public async Task<int> UpdateExpertDocumentAsync(ExpertDocumentVM model)
        {
            try
            {

                var updatedEnity = await this.GetByIdAsync<ExpertDocument>(model.IdExpertDocument);
                this.repository.Detach<ExpertDocument>(updatedEnity);

                updatedEnity = model.To<ExpertDocument>();


                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public async Task<int> DeleteExpertDocumentAsync(ExpertDocumentVM modelToDelete)
        {
            await this.uploadFileService.RemoveFileByIdAsync<ExpertDocument>(modelToDelete.IdExpertDocument);

            await this.repository.HardDeleteAsync<ExpertDocument>(modelToDelete.IdExpertDocument);


            return await this.repository.SaveChangesAsync();
        }
    }
}
