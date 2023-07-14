using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace ISNAPOO.Core.Services.Common
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly IRepository repository;
        private readonly ILogger<NotificationService> _logger;
        private readonly IDataSourceService dataSourceService;
        private readonly IKeyValueService keyValueService;
        private readonly IMailService MailService;
        private readonly ITimeStampService timeStampService;
        private readonly UserManager<ApplicationUser> userManager;
        public NotificationService(IRepository repository, ILogger<NotificationService> logger, IDataSourceService dataSourceService,
            IKeyValueService keyValueService, IMailService MailService,
            UserManager<ApplicationUser> userManager, AuthenticationStateProvider authenticationStateProvider,
            ITimeStampService timeStampService)
            : base(repository, authenticationStateProvider)
        {
            this.repository = repository;
            this._logger = logger;
            this.dataSourceService = dataSourceService;
            this.keyValueService = keyValueService;
            this.MailService = MailService;
            this.userManager = userManager;
            this.timeStampService = timeStampService;
        }

        public async Task<IEnumerable<NotificationVM>> GetAllNotificationsAsync()
        {
            var kvNotificationStatusList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("StatusNotification");

            var list = this.repository.All<Notification>();

            IEnumerable<NotificationVM> result = await list.To<NotificationVM>(n => n.PersonFrom, n => n.PersonTo).ToListAsync();

            foreach (var item in result)
            {
                item.StatusNotificationName = kvNotificationStatusList.FirstOrDefault(k => item.IdStatusNotification == k.IdKeyValue)?.Name ?? string.Empty;
            }

            return result;
        }

        public async Task<IEnumerable<NotificationVM>> GetAllNotificationsByPersonToAsync(int idPerson, bool isUnreadOnly)
        {
            var kvNotificationStatusList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("StatusNotification");

            if (isUnreadOnly)
            {
                var kvNotificationStatusUnread = await dataSourceService.GetKeyValueByIntCodeAsync("StatusNotification", "Unread");

                var list = this.repository.All<Notification>(p => p.IdPersonTo == idPerson && p.IdStatusNotification == kvNotificationStatusUnread.IdKeyValue);

                IEnumerable<NotificationVM> result = await list.To<NotificationVM>(n => n.PersonFrom, n => n.PersonTo).ToListAsync();

                foreach (var item in result)
                {
                    item.StatusNotificationName = kvNotificationStatusList.FirstOrDefault(k => item.IdStatusNotification == k.IdKeyValue)?.Name ?? string.Empty;
                }

                return result.OrderByDescending(x => x.SendDateOnly).ToList();
            }
            else
            {
                var list = this.repository.All<Notification>(p => p.IdPersonTo == idPerson);

                IEnumerable<NotificationVM> result = list.To<NotificationVM>(n => n.PersonFrom, n => n.PersonTo).ToList();

                foreach (var item in result)
                {
                    item.StatusNotificationName = kvNotificationStatusList.FirstOrDefault(k => item.IdStatusNotification == k.IdKeyValue)?.Name ?? string.Empty;
                }

                return result.OrderByDescending(x => x.SendDateOnly).ToList();
            }
        }

        public async Task<NotificationVM> GetNotificationByIdAsync(int idNotification)
        {

            var data = await this.repository.All<Notification>(x => x.IdNotification == idNotification)
                .Include(x => x.PersonFrom)
                .Include(x => x.PersonTo)
                .FirstOrDefaultAsync();

            this.repository.Detach<Notification>(data);

            NotificationVM notificationViewModel = data.To<NotificationVM>();

            var kvNotificationStatus = await dataSourceService.GetKeyValueByIdAsync(notificationViewModel.IdStatusNotification);

            notificationViewModel.StatusNotificationName = kvNotificationStatus.Name;


            return notificationViewModel;
        }

        public async Task<NotificationVM> GetNotificationAndChangeStatusByIdAsync(int idNotification)
        {
            var kvNotificationStatusRead = await dataSourceService.GetKeyValueByIntCodeAsync("StatusNotification", "Read");

            var data = this.repository.AllReadonly<Notification>(x => x.IdNotification == idNotification)
                .Include(x => x.PersonFrom)
                .Include(x => x.PersonTo)
                    .FirstOrDefault();

            this.repository.Detach<Notification>(data);

            var oldIdStatusNotification = data.IdStatusNotification;
            if (data.IdStatusNotification != kvNotificationStatusRead.IdKeyValue)
            {
                data.IdStatusNotification = kvNotificationStatusRead.IdKeyValue;
                data.ReviewDate = DateTime.Now;

                this.repository.Update(data);
                var result = await this.repository.SaveChangesAsync();
            }

            NotificationVM notificationViewModel = data.To<NotificationVM>();

            var kvNotificationStatus = await dataSourceService.GetKeyValueByIdAsync(notificationViewModel.IdStatusNotification);

            notificationViewModel.StatusNotificationName = kvNotificationStatus.Name;

            if (oldIdStatusNotification != kvNotificationStatusRead.IdKeyValue)
            {
                //await this.HandleTimeStampDataForNAPOOAsync(notificationViewModel);
            }

            return notificationViewModel;
        }

        public async Task<string> CreateNotificationsAsync(List<int> idsPersonsTo, string about, string notificationText)
        {
            string msg = string.Empty;
            var kvNotificationStatusUnread = await dataSourceService.GetKeyValueByIntCodeAsync("StatusNotification", "Unread");

            try
            {
                var systemUser = await this.userManager.FindByNameAsync("system.user");
                foreach (var id in idsPersonsTo)
                {
                    var model = new Notification();

                    model.About = about;
                    model.NotificationText = notificationText;
                    model.IdPersonFrom = systemUser.IdPerson;
                    model.IdPersonTo = id;
                    model.IdStatusNotification = kvNotificationStatusUnread.IdKeyValue;
                    model.SendDate = DateTime.Now;
                    model.ReviewDate = null;

                    await this.repository.AddAsync<Notification>(model);

                    var result = await this.repository.SaveChangesAsync();


                    ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                    tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("idNotification", model.IdNotification) };

                    model.Token = BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams);



                    this.repository.Update<Notification>(model);

                    await this.repository.SaveChangesAsync();
                    var convertToEmail = await dataSourceService.GetSettingByIntCodeAsync("NotificationToEmail");
                    if (bool.Parse(convertToEmail.SettingValue))
                    {


                        await MailService.SendEmailFromNotification(await this.repository.GetByIdAsync<Person>(id), await this.repository.GetByIdAsync<Person>(model.IdPersonFrom), model, about, notificationText);
                    }
                }

                return msg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return ex.Message;
            }

        }

        public async Task<string> CreateNotificationsByIdPersonFromAsync(List<int> idsPersonsTo, int idPersonFrom, string about, string notificationText)
        {
            string msg = string.Empty;
            var kvNotificationStatusUnread = await dataSourceService.GetKeyValueByIntCodeAsync("StatusNotification", "Unread");

            try
            {
                foreach (var id in idsPersonsTo)
                {
                    var model = new Notification();

                    model.About = about;
                    model.NotificationText = notificationText;
                    model.IdPersonFrom = idPersonFrom;
                    model.IdPersonTo = id;
                    model.IdStatusNotification = kvNotificationStatusUnread.IdKeyValue;
                    model.SendDate = DateTime.Now;
                    model.ReviewDate = null;

                    await this.repository.AddAsync<Notification>(model);

                    var result = await this.repository.SaveChangesAsync();


                    ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                    tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("idNotification", model.IdNotification) };

                    model.Token = BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams);



                    this.repository.Update<Notification>(model);

                    await this.repository.SaveChangesAsync();
                    var convertToEmail = await dataSourceService.GetSettingByIntCodeAsync("NotificationToEmail");
                    if (bool.Parse(convertToEmail.SettingValue))
                    {


                        await MailService.SendEmailFromNotification(await this.repository.GetByIdAsync<Person>(id), await this.repository.GetByIdAsync<Person>(model.IdPersonFrom), model, about, notificationText);
                    }
                }

                return msg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return ex.Message;
            }

        }

        public async Task<List<NotificationVM>> GetAllNotificationsForProviderTypeByPersonFromAsync(int idPerson, bool isCPO)
        {
            var kvNotificationStatusList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("StatusNotification");
            var kvLicenseTypeCPO = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");

            var personsFromProvider = isCPO
                ? await this.repository.AllReadonly<CandidateProvider>(x => x.IdTypeLicense == kvLicenseTypeCPO.IdKeyValue).To<CandidateProviderVM>(x => x.CandidateProviderPersons).ToListAsync()
                : await this.repository.AllReadonly<CandidateProvider>(x => x.IdTypeLicense != kvLicenseTypeCPO.IdKeyValue).To<CandidateProviderVM>(x => x.CandidateProviderPersons).ToListAsync();

            var personIdsList = personsFromProvider.SelectMany(x => x.CandidateProviderPersons.Select(y => y.IdPerson)).ToList();
            var data = this.repository.AllReadonly<Notification>().Where(x => x.IdPersonFrom == idPerson && x.IdPersonTo.HasValue && personIdsList.Contains(x.IdPersonTo!.Value)).To<NotificationVM>(x => x.PersonFrom, x => x.PersonTo).ToList();

            foreach (var notification in data)
            {
                notification.StatusNotificationName = kvNotificationStatusList.FirstOrDefault(k => notification.IdStatusNotification == k.IdKeyValue)?.Name ?? string.Empty;
            }

            return data;
        }

        public async Task<string> CreateNotificationForListOfPersonIdsAsync(NotificationVM notificationVM, List<int> ids)
        {
            var kvNotificationStatusUnread = await dataSourceService.GetKeyValueByIntCodeAsync("StatusNotification", "Unread");
            var msg = string.Empty;

            try
            {
                var userID = await dataSourceService.GetSettingByIntCodeAsync("UserIDBindWithSystem");
                ApplicationUser user = await userManager.FindByIdAsync(userID.SettingValue);
                foreach (var id in ids)
                {
                    notificationVM.IdNotification = 0;
                    var notificationForDb = notificationVM.To<Notification>();
                    notificationForDb.IdNotification = 0;
                    notificationForDb.SendDate = DateTime.Now;
                    notificationForDb.IdStatusNotification = kvNotificationStatusUnread.IdKeyValue;
                    notificationForDb.IdPersonFrom = user.IdPerson;
                    notificationForDb.IdPersonTo = id;
                    notificationForDb.ReviewDate = null;
                    notificationForDb.PersonFrom = null;
                    notificationForDb.PersonTo = null;

                    await this.repository.AddAsync<Notification>(notificationForDb);
                    await this.repository.SaveChangesAsync();

                    NotificationVM notification = new NotificationVM()
                    {
                        IdNotification = notificationForDb.IdNotification,
                        About = notificationForDb.About,
                        NotificationText = notificationForDb.NotificationText,
                        SendDate = notificationForDb.SendDate,
                    };

                    //await this.HandleTimeStampDataForCPOAsync(notification);

                    var result = await this.repository.SaveChangesAsync();
                    ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                    tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("idNotification", notificationForDb.IdNotification) };

                    notificationForDb.Token = BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams);

                    this.repository.Update<Notification>(notificationForDb);

                    await this.repository.SaveChangesAsync();

                    notificationVM.IdNotification = notificationForDb.IdNotification;

                    var convertToEmail = await dataSourceService.GetSettingByIntCodeAsync("NotificationToEmail");
                    if (bool.Parse(convertToEmail.SettingValue))
                    {
                        await MailService.SendEmailFromNotification(await this.repository.GetByIdAsync<Person>(id), await this.repository.GetByIdAsync<Person>(notificationForDb.IdPersonFrom), notificationForDb, notificationForDb.About, notificationForDb.NotificationText);
                    }

                    this.repository.Detach<Notification>(notificationForDb);
                }

                msg = "Успешно изпращане на писма!";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        private async Task HandleTimeStampDataForCPOAsync(NotificationVM notification)
        {
            await this.timeStampService.GenerateTimeStampFilesAsync(notification, true);
        }

        private async Task HandleTimeStampDataForNAPOOAsync(NotificationVM notification)
        {
            await this.timeStampService.GenerateTimeStampFilesAsync(notification, false);
        }

        public async Task<IEnumerable<int>> GetAllPersonIdsByCandidateProviderIdAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidate_Provider == idCandidateProvider && x.IsAllowedForNotification);

            return await data.Select(x => x.IdPerson).ToListAsync();
        }
        /// <summary>
        /// Изпраща известие на потребители, имащи право да одобряват нова заявка за регстрация
        /// </summary>
        /// <param name="resultContext"></param>
        /// <returns></returns>
        public async Task<ResultContext<CandidateProvider>> SendNotificationForAwaitingConfirmationNapoo(ResultContext<CandidateProvider> resultContext)
        {
            var personList = this.repository.ExecuteSQL<Person>("EXECUTE GetAllPersonsWithActiveUserByPolicy {0}, {1}", new object[2] { "ManageRegistrationFormData", "Active" }).ToList();

            ///TODO: 1.Да се съсзаде изветие
            await CreateNotificationsFromSystemAsync(
                personList.Select(p => p.IdPerson).ToList(),
                "Получена нова форма за електронна регистрация в ИС",
                "В ИС е получена нова форма за електронна регистрация, за която е необходимо одобрение от НАПОО. Юридическо лице: " + resultContext.ResultContextObject.ProviderOwner + ", ЕИК: " + resultContext.ResultContextObject.PoviderBulstat + " .");

            ///TODO: 2.Да се изпрати e-mail
            await MailService.SendMailForAwaitingConfirmationNapoo(personList, resultContext);


            return resultContext;


        }

        public async Task<string> CreateNotificationsFromSystemAsync(List<int> idsPersonsTo, string about, string notificationText)
        {
            string msg = string.Empty;
            var kvNotificationStatusUnread = await dataSourceService.GetKeyValueByIntCodeAsync("StatusNotification", "Unread");

            var userID = await dataSourceService.GetSettingByIntCodeAsync("UserIDBindWithSystem");
            ApplicationUser user = await userManager.FindByIdAsync(userID.SettingValue);


            try
            {
                foreach (var id in idsPersonsTo)
                {
                    var model = new Notification();

                    model.About = about;
                    model.NotificationText = notificationText;
                    model.IdPersonFrom = user.IdPerson;
                    model.IdPersonTo = id;
                    model.IdStatusNotification = kvNotificationStatusUnread.IdKeyValue;
                    model.SendDate = DateTime.Now;
                    model.ReviewDate = null;

                    await this.repository.AddAsync<Notification>(model);

                    var result = await this.repository.SaveChangesAsync();


                    ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                    tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("idNotification", model.IdNotification) };

                    model.Token = BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams);



                    this.repository.Update<Notification>(model);

                    await this.repository.SaveChangesAsync();
                }

                return msg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return ex.Message;
            }

        }

        #region Procedure document notifications
        public async Task<IEnumerable<ProcedureDocumentNotificationVM>> GetProcedureDocumentNotificationsByIdNotificationAsync(int idNotification)
        {
            var data = this.repository.AllReadonly<ProcedureDocumentNotification>(x => x.IdNotification == idNotification);

            return await data.To<ProcedureDocumentNotificationVM>().ToListAsync();
        }

        public async Task<IEnumerable<ProcedureDocumentVM>> GetProcedureDocumentsByListProcedureDocumentsIdsAsync(List<int> ids)
        {
            var data = this.repository.AllReadonly<ProcedureDocument>(x => ids.Contains(x.IdProcedureDocument));
            var dataAsVM = await data.To<ProcedureDocumentVM>().ToListAsync();

            var docTypeValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
            foreach (var doc in dataAsVM)
            {
                var type = docTypeValues.FirstOrDefault(d => d.IdKeyValue == (doc.IdDocumentType.HasValue ? doc.IdDocumentType.Value : GlobalConstants.INVALID_ID));
                if (type is not null)
                {
                    doc.DocumentTypeNameDescription = type.Name + " - " + type.Description;
                }
            }

            return dataAsVM;
        }

        public async Task CreateProcedureDocumentNotificationAsync(int idNotification, List<ProcedureDocumentVM> documents)
        {
            try
            {
                foreach (var doc in documents)
                {
                    ProcedureDocumentNotification dataForDb = new ProcedureDocumentNotification()
                    {
                        IdNotification = idNotification,
                        IdProcedureDocument = doc.IdProcedureDocument
                    };

                    await this.repository.AddAsync<ProcedureDocumentNotification>(dataForDb);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task<IEnumerable<NotificationVM>> GetAllNotificationsByIdStartedProcedureAsync(int idStartedProcedure)
        {
            var startedProcedure = this.repository.AllReadonly<StartedProcedure>(x => x.IdStartedProcedure == idStartedProcedure).Include(x => x.ProcedureDocuments).FirstOrDefault();
            var listIdsProcedureDocs = startedProcedure.ProcedureDocuments.Select(x => x.IdProcedureDocument).ToList();

            var procedureDocumentNotifications = this.repository.AllReadonly<ProcedureDocumentNotification>(x => listIdsProcedureDocs.Contains(x.IdProcedureDocument));
            var listIdsNotifications = procedureDocumentNotifications.Select(x => x.IdNotification).ToList();

            var data = this.repository.AllReadonly<Notification>(x => listIdsNotifications.Contains(x.IdNotification));
            var dataAsVM = await data.To<NotificationVM>(x => x.PersonFrom, x => x.PersonTo).ToListAsync();

            var kvNotificationStatusList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("StatusNotification");
            foreach (var item in dataAsVM)
            {
                item.StatusNotificationName = kvNotificationStatusList.FirstOrDefault(k => item.IdStatusNotification == k.IdKeyValue)?.Name ?? string.Empty;
            }

            return dataAsVM;
        }
        #endregion

        #region FollowUpControl document notifications
        public async Task<IEnumerable<FollowUpControlDocumentNotificationVM>> GetFollowUpControlDocumentNotificationsByIdNotificationAsync(int idNotification)
        {
            var data = this.repository.AllReadonly<FollowUpControlDocumentNotification>(x => x.IdNotification == idNotification);

            return await data.To<FollowUpControlDocumentNotificationVM>().ToListAsync();
        }
        public async Task<IEnumerable<FollowUpControlDocumentVM>> GetFollowUpControlDocumentsByListProcedureDocumentsIdsAsync(List<int> ids)
        {
            var data = this.repository.AllReadonly<FollowUpControlDocument>(x => ids.Contains(x.IdFollowUpControlDocument));
            var dataAsVM = await data.To<FollowUpControlDocumentVM>().ToListAsync();

            var kvDocumentType = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
            foreach (var item in dataAsVM)
            {
                item.DocumentTypeName = kvDocumentType.FirstOrDefault(d => d.IdKeyValue == item.IdDocumentType).Description;
            }

            return dataAsVM;
        }


        public async Task CreateFollowUpControlDocumentNotificationAsync(int idNotification, List<FollowUpControlDocumentVM> documents)
        {
            try
            {
                foreach (var doc in documents)
                {
                    FollowUpControlDocumentNotification dataForDb = new FollowUpControlDocumentNotification()
                    {
                        IdNotification = idNotification,
                        IdFollowUpControlDocument = doc.IdFollowUpControlDocument
                    };

                    await this.repository.AddAsync<FollowUpControlDocumentNotification>(dataForDb);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        #endregion
    }
}
