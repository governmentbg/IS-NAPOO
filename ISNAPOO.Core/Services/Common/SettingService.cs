using Data.Models.Common;
using Data.Models.Data.Common;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class SettingService : BaseService, ISettingService
    {
        private static readonly SemaphoreLocker _locker = new SemaphoreLocker();
        private readonly IRepository repository;
     
        private List<SettingVM> cacheListSettingVM = new List<SettingVM>();

        public SettingService(IRepository repository) : base(repository)
        {
            this.repository = repository;
            
        }

        public async Task mergeSettingsAsync()
        {
            IEnumerable<Setting> settings = await this.repository.AllReadonly<Setting>().ToListAsync();

            var codeSettings = ISettingService.GetSettings();
            bool hasNewSettings = false;

            foreach (var codeSetting in codeSettings)
            {
                if (!settings.Any(x => x.SettingIntCode == codeSetting.SettingIntCode))
                {
                    Setting newSetting = new Setting()
                    {
                        SettingClass = codeSetting.SettingClass,
                        SettingDescription = codeSetting.SettingDescription,
                        SettingIntCode = codeSetting.SettingIntCode,
                        SettingName = codeSetting.SettingName,
                        SettingValue = codeSetting.SettingValue
                    };

                    await this.repository.AddAsync(newSetting);
                    hasNewSettings = true;
                }
            }
            if (hasNewSettings) await this.repository.SaveChangesAsync();
        }

        public async Task<int> CreateSetting(SettingVM model)
        {
            var result = model.To<Setting>();
            await this.repository.AddAsync<Setting>(result);
            return await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<SettingVM>> GetAllSettingsAsync(SettingVM filterSettingVM)
        {
            var data = this.repository.All<Setting>(FilterSettingValue(filterSettingVM));

            // var settings = this.repository.All<Setting>();
            // var keyTypes = this.repository.All<KeyType>();

            //      var result = settings.Join(
            //                      keyTypes, student => student.idSetting,standard => standard.IdModifyUser,(stud, stand) => new { Student = stud, Standard = stand }).ToList();

            //      SELECT[s].[idSetting], [s].[SettingClass], [s].[SettingDescription], [s].[SettingIntCode], [s].[SettingName], [s].[SettingValue], [k].[IdKeyType], [k].[CreationDate], [k].[Description], [k].[IdCreateUser], [k].[IdModifyUser], [k].[IsSystem], [k].[KeyTypeIntCode], [k].[KeyTypeName], [k].[ModifyDate]
            //FROM[Setting] AS[s]
            //INNER JOIN[KeyType] AS[k] ON[s].[idSetting] = [k].[IdModifyUser]
            var dataVM = await data.To<SettingVM>().ToListAsync();
            return dataVM.OrderBy(s => s.SettingName).ToList();
        }

        protected Expression<Func<Setting, bool>> FilterSettingValue(SettingVM model)
        {
            var predicate = PredicateBuilder.True<Setting>();

            if (!string.IsNullOrEmpty(model.SettingIntCode))
            {
                predicate = predicate.And(p => p.SettingIntCode.Contains(model.SettingIntCode));
            }

            if (!string.IsNullOrEmpty(model.SettingValue))
            {
                predicate = predicate.And(p => p.SettingValue.Contains(model.SettingValue));
            }

            if (!string.IsNullOrEmpty(model.SettingName))
            {
                predicate = predicate.And(p => p.SettingName.Contains(model.SettingName));
            }

            return predicate;
        }

        public async Task<ResultContext<SettingVM>> UpdateSettingeAsync(ResultContext<SettingVM> model)
        {
            ResultContext<SettingVM> outputContext = new ResultContext<SettingVM>();

            try
            {
                var updatedEnity = await this.repository.GetByIdAsync<Setting>(model.ResultContextObject.idSetting);
                updatedEnity = model.ResultContextObject.To<Setting>();
                this.repository.Update(updatedEnity);
                await this.repository.SaveChangesAsync();

                
            }
            catch (Exception e)
            {
                outputContext.AddErrorMessage("Неуспешен запис");
            }

            outputContext.AddMessage("Успешен запис");

            outputContext.ResultContextObject = model.ResultContextObject;

            return outputContext;
        }

        public async Task<SettingVM> GetSettingByIntCodeAsync(string intCode)
        {
            try
            {
                var predicate = PredicateBuilder.True<Setting>();
                predicate = predicate.And(p => p.SettingIntCode == intCode);
                var settings = this.repository.All<Setting>(predicate);

                var listVM = settings.To<SettingVM>().ToList();
                var setingVM = listVM.FirstOrDefault();

                return setingVM;
            }
            catch (Exception ex)
            {
                return new SettingVM();
            }
        }

        public async Task<EmailConfiguration> SetUpEmailConfiguration()
        {
            if (cacheListSettingVM.Count == 0)
            {
                var settings = this.repository.All<Setting>();
                cacheListSettingVM = settings.To<SettingVM>().ToList();
            }

            EmailConfiguration emailConfiguration = new EmailConfiguration();
            emailConfiguration.Port = int.Parse(cacheListSettingVM.First(s => s.SettingIntCode == "MailPort").SettingValue);
            emailConfiguration.MailServer = cacheListSettingVM.First(s => s.SettingIntCode == "MailServer").SettingValue;
            emailConfiguration.Email = cacheListSettingVM.First(s => s.SettingIntCode == "MailEmail").SettingValue;
            emailConfiguration.Password = cacheListSettingVM.First(s => s.SettingIntCode == "MailPassword").SettingValue;
            emailConfiguration.UseSSL = bool.Parse(cacheListSettingVM.First(s => s.SettingIntCode == "MailUseSSL").SettingValue);
            emailConfiguration.BccName = cacheListSettingVM.First(s => s.SettingIntCode == "MailBccName").SettingValue;
            emailConfiguration.BccEmail = cacheListSettingVM.First(s => s.SettingIntCode == "MailBccEmail").SettingValue;
            emailConfiguration.CcName = cacheListSettingVM.First(s => s.SettingIntCode == "MailCcName").SettingValue;
            emailConfiguration.CcEmail = cacheListSettingVM.First(s => s.SettingIntCode == "MailCcEmail").SettingValue;
            emailConfiguration.AllowSendMail = bool.Parse(cacheListSettingVM.First(s => s.SettingIntCode == "MailAllowSendMail").SettingValue);
            return emailConfiguration;
        }

        public async Task<ApplicationSetting> SetUpАpplicationSetting()
        {
            if (cacheListSettingVM.Count == 0)
            {
                var settings = this.repository.All<Setting>();
                cacheListSettingVM = settings.To<SettingVM>().ToList();
            }

            ApplicationSetting applicationSetting = new ApplicationSetting();
            applicationSetting.ResourcesFolderName = cacheListSettingVM.First(s => s.SettingIntCode == "AppSettingResourcesFolderName").SettingValue;
            applicationSetting.Host = cacheListSettingVM.First(s => s.SettingIntCode == "AppSettingHost").SettingValue;
            applicationSetting.Port = cacheListSettingVM.First(s => s.SettingIntCode == "AppSettingPort").SettingValue;
            applicationSetting.HttpScheme = cacheListSettingVM.First(s => s.SettingIntCode == "AppSettingHttpScheme").SettingValue;
            applicationSetting.InternalEAuthURL = cacheListSettingVM.First(s => s.SettingIntCode == "AppSettingInternalEAuthURL").SettingValue;
            applicationSetting.DocuServiceURL = cacheListSettingVM.First(s => s.SettingIntCode == "AppSettingDocuServiceURL").SettingValue;
            applicationSetting.EndpointConfigurationDocuService = cacheListSettingVM.First(s => s.SettingIntCode == "EndpointConfigurationDocuService").SettingValue;
            applicationSetting.RegixURL = cacheListSettingVM.First(s => s.SettingIntCode == "AppSettingRegixURL").SettingValue;

            return applicationSetting;
        }

        public async Task ReloadSettings()
        {
            await _locker.LockAsync(async () =>
            {
                cacheListSettingVM = new List<SettingVM>();
                var settings = this.repository.All<Setting>();
                cacheListSettingVM = await settings.To<SettingVM>().ToListAsync();

            });
        }
    }
}