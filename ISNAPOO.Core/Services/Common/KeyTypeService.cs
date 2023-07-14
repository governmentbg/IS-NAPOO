using AutoMapper;
using Data.Models.Common;
using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class KeyTypeService : BaseService, IKeyTypeService

    {
        private readonly IRepository repository;

        public KeyTypeService(IRepository repository) : base(repository)
        {
            this.repository = repository;
        }

        public async Task<string> CreateKeyType(KeyTypeVM model)
        {
            string msg = string.Empty;

        

            try
            {
                var data = model.To<KeyType>();

                await this.repository.AddAsync<KeyType>(data);

                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът e успешeн!";
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<string> DeleteKeyTypeAsync(KeyTypeVM model)
        {
            string msg = string.Empty;

            try
            {
                await this.repository.HardDeleteAsync<KeyType>(model.IdKeyType);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    msg = "Записът е изтрит успешно!";
                }
                else
                {
                    msg = "Грешка при изтриване в базата данни!";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }


        public async Task<IEnumerable<KeyTypeVM>> GetAllKeyTypesAsync()
        {
            var data = this.repository.All<KeyType>();

            return await data.To<KeyTypeVM>().ToListAsync();
        }
        public async Task<IEnumerable<KeyTypeVM>> GetAllKeyTypesIncludeKeyValuesAsync()
        {
            var data = this.repository.All<KeyType>().Include(k => k.KeyValues);

            return await data.To<KeyTypeVM>(x => x.KeyValues).ToListAsync();
        }

        public async Task<string> UpdateKeyTypeAsync(KeyTypeVM model)
        {
            string msg = string.Empty;

            var data = await this.repository.GetByIdAsync<KeyType>(model.IdKeyType);

            try
            {
                data.To<KeyTypeVM, KeyType>(model, data);

                this.repository.Update<KeyType>(data);
                var result = await this.repository.SaveChangesAsync();
                
                if (result > 0)
                {
                    msg = "Записът e успешeн!";
                }
                else
                {
                    msg = "Грешка при запис в базата данни!";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }
    }
}
