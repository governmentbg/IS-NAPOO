using Data.Models.Common;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.SPPOO
{
    public class LegalCapacityOrdinanceService : BaseService, ILegalCapacityOrdinanceService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService DataSourceService;

        public LegalCapacityOrdinanceService(IRepository repository, IDataSourceService DataSourceService) : base(repository)
        {
            this.repository = repository;
            this.DataSourceService = DataSourceService;
        }

        public async Task<int> CreateOrdinance(LegalCapacityOrdinanceUploadedFileVM model)
        {
            var newOrdinance = model.To<LegalCapacityOrdinanceUploadedFile>();
            newOrdinance.UploadedFileName = string.Empty;
            var ordinances = this.repository.AllReadonly<LegalCapacityOrdinanceUploadedFile>();

            if (!ordinances.Any(o => o.IdLegalCapacityOrdinanceType == newOrdinance.IdLegalCapacityOrdinanceType))
            {

                await this.repository.AddAsync<LegalCapacityOrdinanceUploadedFile>(newOrdinance);
                var result = await this.repository.SaveChangesAsync();

                if (result == 1)
                {
                    model.IdLegalCapacityOrdinanceUploadedFile = newOrdinance.IdLegalCapacityOrdinanceUploadedFile;
                    return newOrdinance.IdLegalCapacityOrdinanceUploadedFile;
                }
            }
            else
            {
                return -1;
            }
           return GlobalConstants.INVALID_ID;
        }
        public async Task<int> DeleteOrdinanceAsync(LegalCapacityOrdinanceUploadedFileVM model)
        {
            var entity = await this.repository.GetByIdAsync<LegalCapacityOrdinanceUploadedFile>(model.IdLegalCapacityOrdinanceUploadedFile);
            this.repository.Detach<LegalCapacityOrdinanceUploadedFile>(entity);

            try
            {
                var settingsFolder = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var pathToFile = settingsFolder + "\\" + "UploadedFiles" + "\\" + "LegalCapacityOrdinances" + "\\" + entity.IdLegalCapacityOrdinanceUploadedFile + "\\" + entity.UploadedFileName;
                var directory = settingsFolder + "\\" + "UploadedFiles" + "\\" + "LegalCapacityOrdinances" + "\\" + entity.IdLegalCapacityOrdinanceUploadedFile;
                if (!string.IsNullOrEmpty(entity.UploadedFileName))
                {
                    if (File.Exists(pathToFile))
                    {
                        File.Delete(pathToFile);
                    }

                    if ((Directory.GetFiles(directory)).Length == 0)
                    {
                        Directory.Delete(directory, true);
                    }
                }

                this.repository.HardDelete<LegalCapacityOrdinanceUploadedFile>(entity);
                await this.repository.SaveChangesAsync();


            }
            catch (Exception ex)
            {
            }

            return 1;
        }

        public async Task<IEnumerable<LegalCapacityOrdinanceUploadedFileVM>> GetAllOrdinancesAsync()
        {
            var kvLegalCapacityOrdinanceTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LegalCapacityOrdinanceType");
            var data = this.repository.AllReadonly<LegalCapacityOrdinanceUploadedFile>();
            var dataVM = await data.To<LegalCapacityOrdinanceUploadedFileVM>().ToListAsync();
            foreach (var ordinance in dataVM)
            {
                ordinance.LegalCapacityOrdinanceTypeName = kvLegalCapacityOrdinanceTypeSource.FirstOrDefault(x => x.IdKeyValue == ordinance.IdLegalCapacityOrdinanceType).Name;
            }

            return dataVM;
        }
        public async Task<LegalCapacityOrdinanceUploadedFileVM> GetOrdinanceByIdAsync(int id)
        {
            var order = this.repository.AllReadonly<LegalCapacityOrdinanceUploadedFile>(x => x.IdLegalCapacityOrdinanceUploadedFile == id);

            return await order.To<LegalCapacityOrdinanceUploadedFileVM>().FirstOrDefaultAsync();
        }

        public async Task<int> UpdateOrdinanceAsync(LegalCapacityOrdinanceUploadedFileVM model)
        {
            var updatedEnity = await this.GetByIdAsync<LegalCapacityOrdinanceUploadedFile>(model.IdLegalCapacityOrdinanceUploadedFile);
            this.repository.Detach<LegalCapacityOrdinanceUploadedFile>(updatedEnity);
            
            var ordinances = this.repository.AllReadonly<LegalCapacityOrdinanceUploadedFile>();
            if (updatedEnity.IdLegalCapacityOrdinanceType != model.IdLegalCapacityOrdinanceType)
            {
                if (!ordinances.Any(o => o.IdLegalCapacityOrdinanceType == model.IdLegalCapacityOrdinanceType))
                {
                    updatedEnity = model.To<LegalCapacityOrdinanceUploadedFile>();


                    this.repository.Update(updatedEnity);
                    var result = await this.repository.SaveChangesAsync();
                    return updatedEnity.IdLegalCapacityOrdinanceUploadedFile;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                updatedEnity = model.To<LegalCapacityOrdinanceUploadedFile>();


                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();
                return updatedEnity.IdLegalCapacityOrdinanceUploadedFile;
            }


        }
    }
}
