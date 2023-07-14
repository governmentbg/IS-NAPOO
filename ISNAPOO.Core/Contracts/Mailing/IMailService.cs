namespace ISNAPOO.Core.Contracts.Mailing
{
    using Data.Models.Data.Candidate;
    using Data.Models.Data.Common;
    using global::Data.Models.Data.ProviderData;
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Common.Framework;
    using ISNAPOO.Core.ViewModels.Assessment;
    using ISNAPOO.Core.ViewModels.Candidate;
    using ISNAPOO.Core.ViewModels.Identity;
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public interface IMailService
    {
        Task SendEmailNewRegistration(ResultContext<CandidateProviderVM> candidateProviderVM);
        Task SendEmailNewRegistrationUserPass(ResultContext<ApplicationUserVM> resultContext);
        Task SendEmailRejectRegistration(ResultContext<CandidateProviderVM> resultContext);
        Task SendUserNewPass(ResultContext<ApplicationUserVM> resultContext);
        Task SendForgotPasswordEmail(ResultContext<ApplicationUser> resultContext);
        Task SendMailForAwaitingConfirmationNapoo(List<Person> persons, ResultContext<CandidateProvider> resultContext) ;
        Task SendEmailFromNotification(Person personTo,Person personFrom, Notification model, string about, string notificationText);
        Task SendCustomEmail(MailMessage mail);

        Task SendEmailForSurveyAsync(ResultContext<SurveyResultVM> resultContext, string emailAddress);

        Task<string> SendEmailNewRegistrationFromImport(ResultContext<ApplicationUserVM> resultContext, bool isImportExternalExpert = false);
        Task<string> SendEmailNewRegistrationFromImportUsersForCandidateProvider(ResultContext<ApplicationUserVM> resultContext, bool isImportExternalExpert = false);
    }
}
