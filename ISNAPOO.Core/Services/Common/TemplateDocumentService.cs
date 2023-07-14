namespace ISNAPOO.Core.Services.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Data.Models.Common;
    using Data.Models.Data.Common;

    using ISNAPOO.Core.Contracts.Common;
    using ISNAPOO.Core.HelperClasses;
    using ISNAPOO.Core.Mapping;
    using ISNAPOO.Core.ViewModels.Common;
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.Common.ValidationModels;
    using Syncfusion.DocIO.DLS;

    public class TemplateDocumentService : BaseService, ITemplateDocumentService
    {
        private readonly IRepository repository;
        private readonly IKeyValueService keyValueService;
        private readonly IDataSourceService dataSourceService;
        private readonly string resourceFolder;

        public TemplateDocumentService(IRepository repository, IKeyValueService keyValueService, IDataSourceService dataSourceService) : base(repository)
        {
            this.repository = repository;
            this.keyValueService = keyValueService;
            this.dataSourceService = dataSourceService;

            this.resourceFolder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
        }

        public async Task<int> CreateTemplateDocument(TemplateDocumentVM model)
        {
            var newTemplate = model.To<TemplateDocument>();

            if (string.IsNullOrEmpty(newTemplate.TemplatePath))
            {
                newTemplate.TemplatePath = "#";
            }

            if (string.IsNullOrEmpty(model.TemplateDescription))
            {
                newTemplate.TemplateDescription = string.Empty;
            }

            newTemplate.UploadedFileName = "#";

            await this.repository.AddAsync<TemplateDocument>(newTemplate);
            var result = await this.repository.SaveChangesAsync();

            if (result == 1)
            {
                result = newTemplate.idTemplateDocument;
            }

            return result;
        }

        public async Task<IEnumerable<TemplateDocumentVM>> GetAllTemplateDocumentsWithoutDeletedAsync(TemplateDocumentVM filter)
        {
            KeyValueVM deletedStatusModel = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusTemplate", "Deleted");

            int deletedStatusCode = deletedStatusModel.IdKeyValue;

            IQueryable<TemplateDocument> data = this.repository.All<TemplateDocument>(FilterTemplateDocumentsValue(filter)).Where(x => x.IdStatus != deletedStatusCode);

            var keyValues = this.dataSourceService.GetAllKeyValueList();

            List<TemplateDocumentVM> result = await data.To<TemplateDocumentVM>().ToListAsync();

            result.ForEach(r =>
            {
                r.ModuleName = keyValues.Where(k => r.IdModule == k.IdKeyValue).FirstOrDefault().Name;
                r.StatusName = keyValues.Where(k => r.IdStatus == k.IdKeyValue).FirstOrDefault().Name;
                r.ApplicationTypeName = keyValues.Where(k => r.IdApplicationType == k.IdKeyValue).FirstOrDefault().Name;
            });

            return result;
        }

        public async Task<IEnumerable<TemplateDocumentVM>> GetAllTemplateDocumentsAsync(TemplateDocumentVM filter)
        {
            var docTypeValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");

            IQueryable<TemplateDocument> data = this.repository.All<TemplateDocument>(FilterTemplateDocumentsValue(filter));

            List<TemplateDocumentVM> result = await data.To<TemplateDocumentVM>().ToListAsync();

            foreach (var item in result)
            {
                var type = docTypeValues.FirstOrDefault(d => d.IdKeyValue == item.IdApplicationType);
                if (type is not null)
                {
                    item.ApplicationTypeIntCode = type.KeyValueIntCode;
                }
            }

            return result;
        }

        protected Expression<Func<TemplateDocument, bool>> FilterTemplateDocumentsValue(TemplateDocumentVM model)
        {
            var predicate = PredicateBuilder.True<TemplateDocument>();

            if (!string.IsNullOrEmpty(model.TemplateName))
            {
                predicate = predicate.And(p => p.TemplateName.Contains(model.TemplateName));
            }

            if (model.IdModule != 0)
            {
                predicate = predicate.And(p => p.IdModule == model.IdModule);
            }

            if (model.IdApplicationType != 0)
            {
                predicate = predicate.And(p => p.IdApplicationType == model.IdApplicationType);
            }

            if (model.IdStatus != 0)
            {
                predicate = predicate.And(p => p.IdStatus == model.IdStatus);
            }

            return predicate;
        }

        public async Task<bool> CheckIfExistUploadedFileAsync(TemplateDocumentVM model)
        {
            var fileFullName = resourceFolder + "\\" + model.TemplatePath;

            if (File.Exists(fileFullName))
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<TemplateDocumentVM> GetTemplateDocumentByIdAsync(int id)
        {
            TemplateDocument template = await this.repository.GetByIdAsync<TemplateDocument>(id);
            this.repository.Detach<TemplateDocument>(template);

            return template.To<TemplateDocumentVM>();
        }


        public async Task<int> UploadFileAsync(MemoryStream file, string fileName, TemplateDocumentVM model)
        {
            try
            {
                var module = await dataSourceService.GetKeyValueByIdAsync(model.IdModule);
                var application = await dataSourceService.GetKeyValueByIdAsync(model.IdApplicationType);

                string moduleName = module.KeyValueIntCode;
                string applicationName = application.KeyValueIntCode;

                var filePathMain = "\\" + moduleName + "\\" + applicationName + "\\";
                var filePath = resourceFolder + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + fileName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }

                var updatedEnity = await this.GetByIdAsync<TemplateDocument>(model.idTemplateDocument);
                this.repository.Detach<TemplateDocument>(updatedEnity);

                updatedEnity.TemplatePath = filePathMain + fileName;
                updatedEnity.UploadedFileName = fileName;


                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTemplateDocumentsAsync(TemplateDocumentVM model)
        {
            if (model.idTemplateDocument == 0)
            {
                var result = await this.CreateTemplateDocument(model);
                return result;
            }
            else
            {
                var updatedEnity = await this.GetByIdAsync<TemplateDocument>(model.idTemplateDocument);
                updatedEnity = model.To<TemplateDocument>();
                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();

                //this.repository.Detach<TemplateDocument>(updatedEnity);



                return result;
            }
        }

        public async Task<int> RemoveFileAsync(string fileName, TemplateDocumentVM model)
        {
            try
            {
                if (fileName == "" || fileName == model.FileName)
                {
                    var fileFullName = resourceFolder + "\\" + model.TemplatePath;

                    if (File.Exists(fileFullName))
                    {
                        File.Delete(fileFullName);
                    }

                    var updatedEnity = await this.GetByIdAsync<TemplateDocument>(model.idTemplateDocument);
                    this.repository.Detach<TemplateDocument>(updatedEnity);

                    updatedEnity.TemplatePath = "#";
                    updatedEnity.UploadedFileName = "#";


                    this.repository.Update(updatedEnity);
                    var result = await this.repository.SaveChangesAsync();

                    return result;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public MemoryStream GetUploadedFile(TemplateDocumentVM model)
        {
            var fileFullName = resourceFolder + model.TemplatePath;

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

        public async Task RemoveTemplateDocument(TemplateDocumentVM templateDocument)
        {


            var documentToDelete = await this.repository.GetByIdAsync<TemplateDocument>(templateDocument.idTemplateDocument);


            this.repository.Detach<TemplateDocument>(documentToDelete);

            if (documentToDelete.UploadedFileName != "#" || documentToDelete.TemplatePath != "#")
            {
                try
                {
                    var templateVM = documentToDelete.To<TemplateDocumentVM>();

                    string extension = Path.GetExtension(resourceFolder + documentToDelete.TemplatePath);

                    MemoryStream file = GetUploadedFile(templateVM);

                    string templateNameWithoutExtension = documentToDelete.UploadedFileName.Substring(0, documentToDelete.UploadedFileName.Length - extension.Length);

                    string newTemplateUploadedFileName = ($"{templateNameWithoutExtension} - deleted {DateTime.UtcNow.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}{extension}");

                    var oldFilePath = resourceFolder + documentToDelete.TemplatePath;
                    var newFilePath = @"" + resourceFolder + documentToDelete.TemplatePath.Substring(0, documentToDelete.TemplatePath.Length - documentToDelete.UploadedFileName.Length) + newTemplateUploadedFileName;

                    File.Copy(oldFilePath, newFilePath);
                    File.Delete(oldFilePath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }


                KeyValueVM deletedStatusModel = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusTemplate", "Deleted");

                int deletedStatusCode = deletedStatusModel.IdKeyValue;

                documentToDelete.IdStatus = deletedStatusCode;

                this.repository.Update(documentToDelete);
            }

            await this.repository.SaveChangesAsync();
        }
    }
}