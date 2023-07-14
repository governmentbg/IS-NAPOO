using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Training;
using DocuWorkService;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository repository;
        private readonly ILogger<ScheduleService> _logger;
        private readonly IDocuService docuService;
        private readonly IDataSourceService dataSourceService;
        private readonly INotificationService notificationService;
        private readonly ICandidateProviderService candidateProviderService;
        private readonly ITrainingService trainingService;
        private List<ProcedureDocumentVM> procedureDocuments;
        private Timer DocuWorkSchedular;
        private Timer ExecuteScheduler;
        private Setting timeGap;
        private Setting CheckForDocs;

        public ScheduleService(IRepository repository,
            ILogger<ScheduleService> logger,
            IDocuService docuService,
            IDataSourceService dataSourceService,
            INotificationService notificationService,
            ITrainingService trainingService,
            ICandidateProviderService candidateProviderService)
        {
            this.repository = repository;
            this._logger = logger;
            this.docuService = docuService;
            this.dataSourceService = dataSourceService;
            this.notificationService = notificationService;
            this.candidateProviderService = candidateProviderService;
            this.trainingService = trainingService;
        }

        public void OnStart()
        {
            this.DocuWorkSchedularStart();
            this.Execute();
        }

        private async void DocuWorkSchedularStart()
        {

            DocuWorkSchedular = new Timer(new TimerCallback(DocuWorkSchedularCallback));

            DateTime scheduledTime = DateTime.MinValue;

            ///TODO: intervalMinutes да се взема от Settigs

            timeGap = await dataSourceService.GetSettingByIntCodeAsync("ProcedureDocumentsSchedule");
            CheckForDocs = await dataSourceService.GetSettingByIntCodeAsync("ProcedureDocumentsCheck");

            int intervalMinutes = timeGap == null ? 1 : Int32.Parse(timeGap.SettingValue);

            scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
            if (DateTime.Now > scheduledTime)
            {
                scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
            }

            TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);

            int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);


            DocuWorkSchedular.Change(dueTime, Timeout.Infinite);

            ///TODO: Да има setting, който да спира/пуска проверката към деловодната система

            IQueryable<ProcedureDocument> procedureDocumentsQuery = this.repository
           .All<ProcedureDocument>()
           .Where(x => x.DS_OFFICIAL_ID == null && x.DS_OFFICIAL_GUID == null && x.DS_ID != null && x.DS_GUID != null);
            procedureDocuments = procedureDocumentsQuery.To<ProcedureDocumentVM>().ToList();
            if (procedureDocuments != null && procedureDocuments.Count() != 0 && CheckForDocs != null && bool.Parse(CheckForDocs.SettingValue))
            {
                foreach (var doc in procedureDocuments)
                {
                    try
                    {
                        var officialDoc = await docuService.GetDocOfficialByWorkAsync((int)doc.DS_ID, doc.DS_GUID);
                        if (officialDoc.OfficialDoc.DocID != 0)
                        {
                            doc.DS_OFFICIAL_ID = officialDoc.OfficialDoc.DocID;
                            doc.DS_OFFICIAL_GUID = officialDoc.OfficialDoc.GUID;
                            doc.DS_OFFICIAL_DATE = officialDoc.OfficialDoc.DocDate;
                            doc.DS_OFFICIAL_DocNumber = officialDoc.OfficialDoc.DocNumber;
                            var PD = doc.To<ProcedureDocument>();
                            this.repository.Update(PD);
                            this.repository.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation(e.InnerException?.Message);
                    }
                }
            }
            IQueryable<FollowUpControlDocument> followUpControlDocumentQuery = this.repository
           .All<FollowUpControlDocument>()
           .Where(x => x.DS_OFFICIAL_ID == null && x.DS_OFFICIAL_GUID == null && x.DS_ID != null && x.DS_GUID != null);
            var followUpControlDocument = followUpControlDocumentQuery.To<FollowUpControlDocumentVM>().ToList();
            if (followUpControlDocument != null && followUpControlDocument.Count() != 0 && CheckForDocs != null && bool.Parse(CheckForDocs.SettingValue))
            {
                foreach (var doc in followUpControlDocument)
                {
                    try
                    {
                        var officialDoc = await docuService.GetDocOfficialByWorkAsync((int)doc.DS_ID, doc.DS_GUID);
                        if (officialDoc.OfficialDoc.DocID != 0)
                        {
                            doc.DS_OFFICIAL_ID = officialDoc.OfficialDoc.DocID;
                            doc.DS_OFFICIAL_GUID = officialDoc.OfficialDoc.GUID;
                            doc.DS_OFFICIAL_DATE = officialDoc.OfficialDoc.DocDate;
                            doc.DS_OFFICIAL_DocNumber = officialDoc.OfficialDoc.DocNumber;
                            var PD = doc.To<FollowUpControlDocument>();
                            this.repository.Update(PD);
                            this.repository.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation(e.InnerException?.Message);
                    }
                }
            }

        }

        private void DocuWorkSchedularCallback(object e)
        {
            this.DocuWorkSchedularStart();
        }

        private void ExecuteCallback(object e)
        {
            this.Execute();
        }

        private async void Execute()
        {
            this.ExecuteScheduler = new Timer(new TimerCallback(this.ExecuteCallback));

            int dueTime = Convert.ToInt32(28800000);
            this.ExecuteScheduler.Change(dueTime, Timeout.Infinite);

            List<string> listExecuteDate = new List<string>();

            var lastProcessEntry = await this.GetLastScheduleProcessHistoryAsync();

            DateTime startDate = DateTime.Now;
            if (lastProcessEntry != null && lastProcessEntry.ExecuteDate.GetHashCode() != 0)
            {
                startDate = lastProcessEntry.ExecuteDate;
            }

            while (startDate.Date < DateTime.Now.Date)
            {
                startDate = startDate.AddDays(1);
                listExecuteDate.Add(startDate.ToString("dd.MM.yyyy"));
            }

            foreach (string executeDate in listExecuteDate)
            {
                DateTime tmpExecuteDate;
                DateTime.TryParse(executeDate, BaseHelper.GetDateTimeFormatInfo(), System.Globalization.DateTimeStyles.None, out tmpExecuteDate);

                var outputContext = await this.CreateScheduleProcessHistoryAsync(tmpExecuteDate);

                var exceptionMsgList = new List<string>();

                #region Notification send handlers
                try
                {
                    await this.CheckForDeadlineReachedRequestDocumentAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                    _logger.LogError(ex.StackTrace);
                    exceptionMsgList.Add(ex.Message);
                }

                try
                {
                    await this.CheckForDeadlineReachedDocumentReportAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                    _logger.LogError(ex.StackTrace);
                    exceptionMsgList.Add(ex.Message);
                }

                try
                {
                    await this.CheckForDeadlineReachedProcedureExpertsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                    _logger.LogError(ex.StackTrace);
                    exceptionMsgList.Add(ex.Message);
                }

                try
                {
                    await this.CheckForCourseRIDPKPeriodExpiredAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                    _logger.LogError(ex.StackTrace);
                    exceptionMsgList.Add(ex.Message);
                }

                try
                {
                    await this.CheckForValidationRIDPKPeriodExpiredAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                    _logger.LogError(ex.StackTrace);
                    exceptionMsgList.Add(ex.Message);
                }
                try
                {
                    await this.CheckForDeadlineValiadtionClientExamDates();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                    _logger.LogError(ex.StackTrace);
                    exceptionMsgList.Add(ex.Message);
                }
                #endregion

                await this.UpdateScheduleProcessHistoryAsync(outputContext.NewEntityId.Value, exceptionMsgList);
            }
        }
        private async Task CheckForDeadlineValiadtionClientExamDates()
        {
            var data = this.repository.AllReadonly<ValidationClient>().Where(x => x.DS_OFFICIAL_ID == null && (x.ExamPracticeDate.HasValue || x.ExamTheoryDate.HasValue)).To<ValidationClientVM>().ToList();
            var today = DateTime.Today;

            var setting = await this.dataSourceService.GetSettingByIntCodeAsync("DaysForValidationClientExamCheck");

            var notifications = new ConcurrentBag<NotificationVM>();

            var candidateIds = new List<int>();

            Parallel.ForEach(data, client =>
            {
                if (client.ExamTheoryDate is not null && (client.ExamTheoryDate.Value - today).Days == Int32.Parse(setting.SettingValue))
                {
                    var notification = new NotificationVM()
                    {
                        NotificationText = $"Наближава дата за изпит за валидиране по теория на {client.FullName}. Нямате изпратено известие към НАПОО.",
                        About = $"Наближава дата за изпит за валидиране по теория на {client.FirstName} {client.FamilyName}. Нямате изпратено известие към НАПОО.",
                        IdCandidateProvider = client.IdCandidateProvider
                    };
                    notifications.Add(notification);
                }

                if (client.ExamPracticeDate is not null && (client.ExamPracticeDate.Value - today).Days == Int32.Parse(setting.SettingValue))
                {
                    var notification = new NotificationVM()
                    {
                        NotificationText = $"Наближава дата за изпит за валидиране по практика на {client.FullName}. Нямате изпратено известие към НАПОО.",
                        About = $"Наближава дата за изпит за валидиране по практика на {client.FirstName} {client.FamilyName}. Нямате изпратено известие към НАПОО.",
                        IdCandidateProvider = client.IdCandidateProvider
                    };
                    notifications.Add(notification);
                }
            });

            foreach(var notification in notifications.ToList())
            {
                var candidateProviderPersons = await this.candidateProviderService.GetAllCandidateProviderPersonsAllowedForNotificationsByIdCandidateProviderAsync(notification.IdCandidateProvider.Value);

                var listPersonIds = candidateProviderPersons.Select(x => x.IdPerson).ToList();

                var msg = await this.notificationService.CreateNotificationForListOfPersonIdsAsync(notification, listPersonIds);
            }
        }
        private async Task<ScheduleProcessHistory> GetLastScheduleProcessHistoryAsync()
        {
            var data = await this.repository.GetLastEntryAsync<ScheduleProcessHistory>();
            return data;
        }

        private async Task<ISNAPOO.Common.Framework.ResultContext<ScheduleProcessHistory>> CreateScheduleProcessHistoryAsync(DateTime executeDate)
        {
            try
            {
                var outputContext = new ISNAPOO.Common.Framework.ResultContext<ScheduleProcessHistory>();
                var scheduleProcessEntryForDb = new ScheduleProcessHistory()
                {
                    ExecuteDate = executeDate,
                    RunTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    Successful = false,
                    Exception = string.Empty
                };

                await this.repository.AddAsync<ScheduleProcessHistory>(scheduleProcessEntryForDb);
                await this.repository.SaveChangesAsync();

                outputContext.NewEntityId = scheduleProcessEntryForDb.IdScheduleProcessHistory;

                return outputContext;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                return null;
            }
        }

        private async Task UpdateScheduleProcessHistoryAsync(int idScheduleProcessHistory, List<string> exceptionMsgList)
        {
            try
            {
                var entityFromDb = await this.repository.GetByIdAsync<ScheduleProcessHistory>(idScheduleProcessHistory);
                entityFromDb.Successful = !exceptionMsgList.Any() ? true : false;
                entityFromDb.RunTime = DateTime.Now;
                entityFromDb.EndTime = DateTime.Now;
                entityFromDb.Exception = !exceptionMsgList.Any() ? string.Empty : string.Join((", "), exceptionMsgList);

                this.repository.Update<ScheduleProcessHistory>(entityFromDb);
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        private async Task CheckForDeadlineReachedRequestDocumentAsync()
        {
            try
            {
                var term = int.Parse((await this.dataSourceService.GetSettingByIntCodeAsync("RequestDocumentTerm")).SettingValue);
                var documentRequestDeadline = DateTime.Parse((await this.dataSourceService.GetSettingByIntCodeAsync("RequestDocumentPeriod")).SettingValue);
                var dateToday = DateTime.Today;
                var difference = (documentRequestDeadline - dateToday).Days;

                if (difference == term)
                {
                    var activeCandidateProviders = await this.candidateProviderService.GetAllActiveCPOCandidateProvidersWithoutAnythingIncludedAsync();
                    var listCandidateProviderPersons = new List<CandidateProviderPersonVM>();
                    foreach (var provider in activeCandidateProviders)
                    {
                        var candidateProviderPersons = await this.candidateProviderService.GetAllCandidateProviderPersonsAllowedForNotificationsByIdCandidateProviderAsync(provider.IdCandidate_Provider);
                        listCandidateProviderPersons.AddRange(candidateProviderPersons);
                    }

                    var listPersonIds = listCandidateProviderPersons.Select(x => x.IdPerson).ToList();
                    var year = DateTime.Now.Year - 1;
                    var notificationVM = new NotificationVM()
                    {
                        NotificationText = $"Крайният срок за подаване на заявки за документация за {year}г. е {documentRequestDeadline.ToString("dd.MM.yyyy")}г.!",
                        About = $"Наближава крайният срок за подаване на заявки за документация за {year} година"
                    };

                    var msg = await this.notificationService.CreateNotificationForListOfPersonIdsAsync(notificationVM, listPersonIds);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        private async Task CheckForDeadlineReachedDocumentReportAsync()
        {
            try
            {
                var term = int.Parse((await this.dataSourceService.GetSettingByIntCodeAsync("RequestDocumentTerm")).SettingValue);
                var documentReportDeadline = DateTime.Parse((await this.dataSourceService.GetSettingByIntCodeAsync("DocumentReportPeriod")).SettingValue);
                var dateToday = DateTime.Now;
                var difference = (documentReportDeadline - dateToday).Days;

                if (difference == term)
                {

                    var activeCandidateProviders = await this.candidateProviderService.GetAllActiveCPOCandidateProvidersWithoutAnythingIncludedAsync();
                    var listCandidateProviderPersons = new List<CandidateProviderPersonVM>();
                    foreach (var provider in activeCandidateProviders)
                    {
                        var candidateProviderPersons = await this.candidateProviderService.GetAllCandidateProviderPersonsAllowedForNotificationsByIdCandidateProviderAsync(provider.IdCandidate_Provider);
                        listCandidateProviderPersons.AddRange(candidateProviderPersons);
                    }

                    var listPersonIds = listCandidateProviderPersons.Select(x => x.IdPerson).ToList();
                    var year = DateTime.Now.Year - 1;
                    var notificationVM = new NotificationVM()
                    {
                        NotificationText = $"Крайният срок за подаване на отчети за документи с фабрична номерация за {year}г. е {documentReportDeadline.ToString("dd.MM.yyyy")}г.!",
                        About = $"Наближава крайният срок за подаване на отчети за документи с фабрична номерация за {year} година"
                    };

                    var msg = await this.notificationService.CreateNotificationForListOfPersonIdsAsync(notificationVM, listPersonIds);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        private async Task CheckForDeadlineReachedProcedureExpertsAsync()
        {
            var procedureExternalExpertsList = this.repository.AllReadonly<ProcedureExternalExpert>()
                .Include(x => x.Expert).AsNoTracking()
                .Include(x => x.StartedProcedure).AsNoTracking();
            var candidateProviders = await this.candidateProviderService.GetAllCandidateProvidersAsync();

            var term = int.Parse((await this.dataSourceService.GetSettingByIntCodeAsync("RequestDocumentTerm")).SettingValue);

            var listIdsPerson = new List<int>();
            var procedureDict = new Dictionary<StartedProcedure, List<int>>();
            foreach (var entry in procedureExternalExpertsList)
            {
                if (!procedureDict.ContainsKey(entry.StartedProcedure))
                {
                    if (entry.StartedProcedure.ExpertReportDeadline.HasValue)
                    {
                        procedureDict.Add(entry.StartedProcedure, new List<int>());
                    }
                }

                if (entry.StartedProcedure.ExpertReportDeadline.HasValue && entry.Expert.IsExternalExpert && entry.Expert.IdPerson.HasValue)
                {
                    var deadline = DateTime.Parse(entry.StartedProcedure.ExpertReportDeadline.Value.ToString("dd.MM.yyyy"));
                    var dateToday = DateTime.Today;
                    var difference = (deadline - dateToday).Days;
                    if (difference == term)
                    {
                        procedureDict[entry.StartedProcedure].Add(entry.Expert.IdPerson.Value);
                    }
                }
            }

            foreach (var procedure in procedureDict)
            {
                if (procedure.Value.Any())
                {
                    var candidateProvider = candidateProviders.FirstOrDefault(x => x.IdStartedProcedure == procedure.Key.IdStartedProcedure);
                    var notificationVM = new NotificationVM()
                    {
                        NotificationText = $"Крайният срок за предоставяне на доклад по заявление за лицензиране на ЦПО към {candidateProvider.ProviderOwner} е {procedure.Key.ExpertReportDeadline.Value.ToString("dd.MM.yyyy")}г.!",
                        About = $"Наближава крайният срок за подаване на доклад по заявление за лицензиране"
                    };

                    var msg = await this.notificationService.CreateNotificationForListOfPersonIdsAsync(notificationVM, procedure.Value);
                }
            }
        }

        private async Task CheckForCourseRIDPKPeriodExpiredAsync()
        {
            var period = int.Parse((await this.dataSourceService.GetSettingByIntCodeAsync("RIDPKCorrectionPeriod")).SettingValue);
            var dtPeriodToCheck = DateTime.Now.AddDays(-(period + 2));
            var kvDocStatusReturned = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Returned");
            var kvDocStatusRejected = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Rejected");
            var clientCourseDocumentStatuses = await this.repository.AllReadonly<ClientCourseDocumentStatus>(x => x.CreationDate >= dtPeriodToCheck)
                .OrderByDescending(x => x.IdClientCourseDocumentStatus)
                .ToListAsync();
            var clientCourseDocDict = new Dictionary<int, List<int>>();
            foreach (var status in clientCourseDocumentStatuses)
            {
                if (!clientCourseDocDict.ContainsKey(status.IdClientCourseDocument))
                {
                    clientCourseDocDict.Add(status.IdClientCourseDocument, new List<int>());
                }

                clientCourseDocDict[status.IdClientCourseDocument].Add(status.IdClientDocumentStatus);
            }

            var listStatusIdsToReject = new List<int>();
            foreach (var kvp in clientCourseDocDict)
            {
                var statusValue = clientCourseDocDict[kvp.Key].FirstOrDefault();
                if (statusValue == kvDocStatusReturned.IdKeyValue)
                {
                    var docStatus = clientCourseDocumentStatuses.FirstOrDefault(x => x.IdClientCourseDocument == kvp.Key && x.IdClientDocumentStatus == kvDocStatusReturned.IdKeyValue);
                    if (docStatus is not null && ((DateTime.Now - docStatus.CreationDate).Days >= period + 1))
                    {
                        listStatusIdsToReject.Add(docStatus.IdClientCourseDocumentStatus);
                    }
                }
            }

            foreach (var idStatus in listStatusIdsToReject)
            {
                var courseDocStatus = await this.repository.AllReadonly<ClientCourseDocumentStatus>(x => x.IdClientCourseDocumentStatus == idStatus)
                    .Include(x => x.ClientCourseDocument)
                        .ThenInclude(x => x.ClientCourse)
                            .ThenInclude(x => x.Course)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (courseDocStatus is not null)
                {
                    await this.trainingService.UpdateClientCourseDocumentStatusAsync(courseDocStatus.ClientCourseDocument.IdClientCourseDocument, kvDocStatusRejected.IdKeyValue);

                    var comment = $"Служебно променен статус на \"Отказан\" поради изтичане на {period} - дневния срок за подаване за повторна проверка.";
                    await this.trainingService.AddClientCourseDocumentStatusAsync(courseDocStatus.ClientCourseDocument.IdClientCourseDocument, kvDocStatusRejected.IdKeyValue, comment);

                    var notificationVM = new NotificationVM()
                    {
                        NotificationText = $"Подаването на документи за вписване в РИДПК за курс {courseDocStatus.ClientCourseDocument.ClientCourse.Course.CourseName}, завършил на {courseDocStatus.ClientCourseDocument.ClientCourse.Course.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г. бе обработено с резултат РИДПК - отказан.\r\nКоментар за резултата от обработката: Служебно променен статус на \"Отказан\" поради изтичане на {period}-дневния срок за подаване за повторна проверка.\r\nИС на НАПОО",
                        About = "РИДПК - служебно отказан за вписване в регистъра"
                    };

                    var persons = await this.candidateProviderService.GetAllCandidateProviderPersonsAllowedForNotificationsByIdCandidateProviderAsync(courseDocStatus.ClientCourseDocument.ClientCourse.Course.IdCandidateProvider!.Value);
                    var listPersonIds = persons.Select(y => y.IdPerson).ToList();
                    listPersonIds.Add(courseDocStatus.IdCreateUser);

                    await this.notificationService.CreateNotificationForListOfPersonIdsAsync(notificationVM, listPersonIds);
                }
            }
        }

        private async Task CheckForValidationRIDPKPeriodExpiredAsync()
        {
            var period = int.Parse((await this.dataSourceService.GetSettingByIntCodeAsync("RIDPKCorrectionPeriod")).SettingValue);
            var dtPeriodToCheck = DateTime.Now.AddDays(-(period + 5));
            var kvDocStatusReturned = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Returned");
            var kvDocStatusRejected = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Rejected");
            var clientCourseDocumentStatuses = await this.repository.AllReadonly<ValidationClientDocumentStatus>(x => x.CreationDate >= dtPeriodToCheck)
                .OrderByDescending(x => x.IdValidationClientDocumentStatus)
                .ToListAsync();
            var clientCourseDocDict = new Dictionary<int, List<int>>();
            foreach (var status in clientCourseDocumentStatuses)
            {
                if (!clientCourseDocDict.ContainsKey(status.IdValidationClientDocument))
                {
                    clientCourseDocDict.Add(status.IdValidationClientDocument, new List<int>());
                }

                clientCourseDocDict[status.IdValidationClientDocument].Add(status.IdClientDocumentStatus);
            }

            var listStatusIdsToReject = new List<int>();
            foreach (var kvp in clientCourseDocDict)
            {
                var statusValue = clientCourseDocDict[kvp.Key].FirstOrDefault();
                if (statusValue == kvDocStatusReturned.IdKeyValue)
                {
                    var docStatus = clientCourseDocumentStatuses.FirstOrDefault(x => x.IdValidationClientDocument == kvp.Key && x.IdClientDocumentStatus == kvDocStatusReturned.IdKeyValue);
                    if (docStatus is not null && ((DateTime.Now - docStatus.CreationDate).Days >= period + 1))
                    {
                        listStatusIdsToReject.Add(docStatus.IdValidationClientDocumentStatus);
                    }
                }
            }

            foreach (var idStatus in listStatusIdsToReject)
            {
                var courseDocStatus = await this.repository.AllReadonly<ValidationClientDocumentStatus>(x => x.IdValidationClientDocumentStatus == idStatus)
                    .Include(x => x.ValidationClientDocument)
                        .ThenInclude(x => x.ValidationClient)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (courseDocStatus is not null)
                {
                    await this.trainingService.UpdateValidationClientDocumentStatusAsync(courseDocStatus.ValidationClientDocument.IdValidationClientDocument, kvDocStatusRejected.IdKeyValue);

                    var comment = $"Служебно променен статус на \"Отказан\" поради изтичане на {period} - дневния срок за подаване за повторна проверка.";
                    await this.trainingService.AddValidationClientDocumentStatusAsync(courseDocStatus.ValidationClientDocument.IdValidationClientDocument, kvDocStatusRejected.IdKeyValue, comment);

                    var notificationVM = new NotificationVM()
                    {
                        NotificationText = $"Подаването на документи за вписване в РИДПК за валидиране на {courseDocStatus.ValidationClientDocument.ValidationClient.FirstName} {courseDocStatus.ValidationClientDocument.ValidationClient.SecondName} {courseDocStatus.ValidationClientDocument.ValidationClient.FamilyName}, завършил на {courseDocStatus.ValidationClientDocument.ValidationClient.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г. бе обработено с резултат РИДПК - отказан.\r\nКоментар за резултата от обработката: Служебно променен статус на \"Отказан\" поради изтичане на {period}-дневния срок за подаване за повторна проверка.\r\nИС на НАПОО",
                        About = "РИДПК - служебно отказан за вписване в регистъра"
                    };

                    var persons = await this.candidateProviderService.GetAllCandidateProviderPersonsAllowedForNotificationsByIdCandidateProviderAsync(courseDocStatus.ValidationClientDocument.ValidationClient.IdCandidateProvider);
                    var listPersonIds = persons.Select(y => y.IdPerson).ToList();
                    listPersonIds.Add(courseDocStatus.IdCreateUser);

                    await this.notificationService.CreateNotificationForListOfPersonIdsAsync(notificationVM, listPersonIds);
                }
            }
        }
    }
}
