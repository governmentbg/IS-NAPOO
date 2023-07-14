using Data.Models.Common;
using Data.Models.Data.Common;
using Data.Models.Data.Request;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class KeyValueService : BaseService, IKeyValueService
    {
        private readonly IRepository repository;
        private readonly ILogger<KeyValueService> _logger;
        

        public KeyValueService(
            IRepository repository, 
            ILogger<KeyValueService> logger, 
            AuthenticationStateProvider authenticationStateProvider) : base(repository, authenticationStateProvider)
        {
            this.repository = repository;
            this._logger = logger;
            
        }

        public async Task<KeyValueVM> GetKeyValueByIdAsync(int id)
        {
            KeyValue keyValue = await this.repository.GetByIdAsync<KeyValue>(id);

            return keyValue.To<KeyValueVM>();
        }

        public async Task<ResultContext<KeyValueVM>> CreateKeyValueAsync(ResultContext<KeyValueVM> inputContext)
        {
            try
            {
                var newKeyValue = inputContext.ResultContextObject.To<KeyValue>();
                var data = this.repository.All<KeyValue>();
                if(data.Any(k => k.IdKeyType == newKeyValue.IdKeyType && k.KeyValueIntCode == newKeyValue.KeyValueIntCode))
                {
                    inputContext.AddErrorMessage("Вече има въведена стойност със същия код за същата номенклатура!");
                    return inputContext;
                }

                await this.repository.AddAsync<KeyValue>(newKeyValue);

                int result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    inputContext.AddMessage("Записът e успешeн!");
                    inputContext.ResultContextObject.IdKeyValue = newKeyValue.IdKeyValue;
                }
                else
                {
                    inputContext.AddErrorMessage("Грешка при запис в базата!");
                }

                return inputContext;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage("Грешка при запис в базата!");
                return inputContext;
            }
        }


        public async Task<string> DeleteKeyValueAsync(KeyValueVM model)
        {
            string msg = string.Empty;

            try
            {
                await this.repository.HardDeleteAsync<KeyValue>(model.IdKeyValue);
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

        public async Task<IEnumerable<KeyValueVM>> GetAllAsync()
        {
            var data = this.repository.All<KeyValue>();

            return await data.To<KeyValueVM>().ToListAsync();
        }

        public async Task<IEnumerable<KeyValueVM>> GetAllAsync(KeyValueVM filter)
        {
            var data = this.repository.All<KeyValue>(FilterSettingValue(filter));

            return await data.To<KeyValueVM>().OrderBy(x => x.Order).ToListAsync();
        }

        protected Expression<Func<KeyValue, bool>> FilterSettingValue(KeyValueVM model)
        {
            var predicate = PredicateBuilder.True<KeyValue>();

            if (model.IdKeyType != 0)
            {
                predicate = predicate.And(p => p.IdKeyType == model.IdKeyType);
            }
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                predicate = predicate.And(p => p.Name.Contains(model.Name));
            }

            return predicate;
        }

        public async Task<ResultContext<KeyValueVM>> UpdateKeyValueAsync(ResultContext<KeyValueVM> inputContext)
        {
            ResultContext<KeyValueVM> resultContext = new ResultContext<KeyValueVM>();
            resultContext = inputContext;
            KeyValueVM model = inputContext.ResultContextObject;
            var data = await this.repository.GetByIdAsync<KeyValue>(model.IdKeyValue);
            if (data.KeyValueIntCode != model.KeyValueIntCode)
            {
                var dataSource = this.repository.All<KeyValue>();
                if (dataSource.Any(k => k.IdKeyType == data.IdKeyType && k.KeyValueIntCode == model.KeyValueIntCode))
                {
                    inputContext.AddErrorMessage("Вече има въведена стойност със същия код за същата номенклатура!");
                    return inputContext;
                }
            }
            data.Name = model.Name;
            data.Description = model.Name;
            
            try
            {
                data.To<KeyValueVM, KeyValue>(model, data);
 
                repository.Update<KeyValue>(data);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {                   
                    resultContext.AddMessage("Записът e успешeн!");                  
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата данни!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата!");
                return resultContext;
            }

            return resultContext;
        }

        public async Task<IEnumerable<KeyValueVM>> GetAllNKPDClassValuesViaKeyTypeIntCodeAsync(string keyTypeIntCode)
        {
            KeyType keyType = await this.repository.All<KeyType>(x => x.KeyTypeIntCode == keyTypeIntCode).FirstOrDefaultAsync();

            if (keyType != null)
            {
                IQueryable<KeyValue> keyValues = this.repository.All<KeyValue>(x => x.IdKeyType == keyType.IdKeyType);

                return await keyValues.To<KeyValueVM>().ToListAsync();
            }

            return null;
        }

        public Task<string> UpdateKeyValueAsync(KeyValueVM model)
        {
            throw new NotImplementedException();
        }
    }
}