using Data.Models.Data.Candidate;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationVM>> GetAllNotificationsAsync();

        Task<NotificationVM> GetNotificationByIdAsync(int idNotification);

        Task<NotificationVM> GetNotificationAndChangeStatusByIdAsync(int idNotification);

        Task<IEnumerable<NotificationVM>> GetAllNotificationsByPersonToAsync(int idPerson, bool isUnread);

        Task<List<NotificationVM>> GetAllNotificationsForProviderTypeByPersonFromAsync(int idPerson, bool isCPO);

        Task<string> CreateNotificationsAsync(List<int> idsPersonsTo, string about, string notificationText);

        Task<string> CreateNotificationsByIdPersonFromAsync(List<int> idsPersonsTo, int idPersonFrom, string about, string notificationText);

        Task<IEnumerable<int>> GetAllPersonIdsByCandidateProviderIdAsync(int idCandidateProvider);

        Task<string> CreateNotificationForListOfPersonIdsAsync(NotificationVM notificationVM, List<int> ids);

        Task<ResultContext<CandidateProvider>> SendNotificationForAwaitingConfirmationNapoo(ResultContext<CandidateProvider> resultContext);

        #region Procedure document notifications
        Task<IEnumerable<ProcedureDocumentNotificationVM>> GetProcedureDocumentNotificationsByIdNotificationAsync(int idNotification);

        Task<IEnumerable<ProcedureDocumentVM>> GetProcedureDocumentsByListProcedureDocumentsIdsAsync(List<int> ids);

        Task CreateProcedureDocumentNotificationAsync(int idNotification, List<ProcedureDocumentVM> documents);

        Task<IEnumerable<NotificationVM>> GetAllNotificationsByIdStartedProcedureAsync(int idStartedProcedure);
        #endregion

        #region FollowUpControl document notifications
        Task<IEnumerable<FollowUpControlDocumentNotificationVM>> GetFollowUpControlDocumentNotificationsByIdNotificationAsync(int idNotification);
        Task<IEnumerable<FollowUpControlDocumentVM>> GetFollowUpControlDocumentsByListProcedureDocumentsIdsAsync(List<int> ids);
        Task CreateFollowUpControlDocumentNotificationAsync(int idNotification, List<FollowUpControlDocumentVM> documents);

        #endregion


    }
}
