using ISNAPOO.Core.Contracts.DOC;

namespace ISNAPOO.Core.Services.Mailing
{
    using global::Data.Models.Data.ProviderData;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using ISNAPOO.Common.Framework;
    using ISNAPOO.Common.HelperClasses;
    using ISNAPOO.Core.Contracts.Mailing;
    using ISNAPOO.Core.ViewModels.Candidate;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using ISNAPOO.Core.ViewModels.Identity;
    using ISNAPOO.Core.ViewModels;
    using Data.Models.Data.Common;
    using Data.Models.Data.Candidate;
    using ISNAPOO.Core.ViewModels.Common;
    using Syncfusion.DocIO.DLS;
    using ISNAPOO.Core.ViewModels.Assessment;
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.Contracts.Common;

    public class MailService : IMailService
    {
        private readonly IConfiguration configuration;
        private readonly EmailConfiguration emailConfiguration;
        private readonly ApplicationSetting applicationSetting;
        private readonly IDataSourceService dataSourceService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<MailService> _logger;
        private readonly UserManager<ApplicationUser> userManager;


        private readonly string mailTemplates = @"Data\MailTemplates\{0}.html";

        //Mac os
        //private readonly string mailTemplates = @"Data/MailTemplates/{0}.html";        


        public MailService(IConfiguration configuration,
                ILogger<MailService> logger,
                IDataSourceService dataSourceService,
                UserManager<ApplicationUser> userManager,
                IOptions<EmailConfiguration> _emailConfiguration,
                IOptions<ApplicationSetting> _applicationSetting,
                IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this._logger = logger;
            emailConfiguration = _emailConfiguration.Value;
            applicationSetting = _applicationSetting.Value;
            this.httpContextAccessor = httpContextAccessor;
            this.dataSourceService = dataSourceService;
        }

        /// <summary>
        /// Изпращане на писмо с линк за забравена парола
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task SendForgotPasswordEmail(ResultContext<ApplicationUser> resultContext)
        {
            try
            {
                var microsoftToken = await userManager.GeneratePasswordResetTokenAsync(resultContext.ResultContextObject);


                ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("Token", HttpUtility.UrlEncode(microsoftToken)), new KeyValuePair<string, object>("Username", resultContext.ResultContextObject.UserName) };
                string token = BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams, 60);

                var template = File.ReadAllText(String.Format(mailTemplates, "ResetPasswordTemplate"));

                template = this.UpdatePlaceHoldersResetPassword(template, token);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailConfiguration.Email);
                    mail.To.Add(resultContext.ResultContextObject.Email);
                    mail.Subject = $"Възстановяване на парола за ИС на НАПОО";
                    mail.Body = template;
                    mail.IsBodyHtml = true;

                    await this.SendEmailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Проблем с изпращане на ел. писмо за смяна на парола");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.InnerException?.Message);
            }
        }


        /// <summary>
        /// Изпращане на e-mail
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        private async Task SendEmailAsync(MailMessage mail)
        {

            if (!emailConfiguration.AllowSendMail)
            {
                return;
            }

            using (SmtpClient smtp = new SmtpClient(emailConfiguration.MailServer, emailConfiguration.Port))
            {
                smtp.Credentials = new System.Net.NetworkCredential(emailConfiguration.Email, emailConfiguration.Password);
                smtp.EnableSsl = emailConfiguration.UseSSL;
                smtp.TargetName = emailConfiguration.TargetName;
                mail.BodyEncoding = Encoding.Default;
                try
                {
                    _logger.LogError(emailConfiguration.DebugEmailConfiguration());
                    await smtp.SendMailAsync(mail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.StackTrace);
                    _logger.LogError(ex.InnerException?.Message);

                }
            }
        }

        private string UpdatePlaceHoldersResetPassword(string template, string token)
        {

            string appDomain = GetAppDomain();

            string confirmationLink = "/RestartPassword?token={0}";

            var placeHolders = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("{{Link}}", string.Format(appDomain + confirmationLink, token))
                //new KeyValuePair<string, string>("{{Link}}", string.Format(appDomain + confirmationLink, HttpUtility.UrlEncode(token)))

            };


            foreach (var placeHolder in placeHolders)
            {
                if (template.Contains(placeHolder.Key))
                {
                    template = template.Replace(placeHolder.Key, placeHolder.Value);
                }
            }

            return template;
        }

        private string GetAppDomain()
        {
            string appDomain = string.Empty;

            if (httpContextAccessor != null && httpContextAccessor.HttpContext != null)
            {
                appDomain += httpContextAccessor.HttpContext.Request.Scheme;
                appDomain += "://" + httpContextAccessor.HttpContext.Request.Host.Host;
                appDomain += ":" + httpContextAccessor.HttpContext.Request.Host.Port;
            }
            else
            {
                appDomain += applicationSetting.HttpScheme;
                appDomain += "://" + applicationSetting.Host;
                appDomain += ":" + applicationSetting.Port;
            }

            return appDomain;
        }


        public async Task SendEmailNewRegistration(ResultContext<CandidateProviderVM> resultContext)
        {
            try
            {



                var template = File.ReadAllText(String.Format(mailTemplates, "NewRegistrationConfirmationTemplate"));
                template = this.UpdatePlaceHoldersNewRegistration(template, resultContext.ResultContextObject.Token);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailConfiguration.Email);
                    mail.To.Add(resultContext.ResultContextObject.ProviderEmail);
                    mail.Subject = $"Нова регистрация в сайта на НАПОО";
                    mail.Body = template;
                    mail.IsBodyHtml = true;

                    await this.SendEmailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.InnerException?.Message);
            }
        }
        public async Task SendEmailRejectRegistration(ResultContext<CandidateProviderVM> resultContext)
        {
            try
            {



                var template = File.ReadAllText(String.Format(mailTemplates, "RejectRegistrationTemplate"));
                template = this.UpdatePlaceHoldersRejectRegistration(template, resultContext.ResultContextObject.RejectionReason);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailConfiguration.Email);
                    mail.To.Add(resultContext.ResultContextObject.ProviderEmail);
                    mail.Subject = $"Отказана регистрация до информационната система на НАПОО";
                    mail.Body = template;
                    mail.IsBodyHtml = true;

                    await this.SendEmailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.InnerException?.Message);
            }
        }

        public async Task SendEmailForSurveyAsync(ResultContext<SurveyResultVM> resultContext, string emailAddress)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var endDate = $"{model.SurveyEndDate!.Value.Date.ToString(GlobalConstants.DATE_FORMAT)} г.";
                var template = File.ReadAllText(String.Format(mailTemplates, "SurveyEmailTemplate"));
                template = this.UpdatePlaceHoldersSurveyEmailTemplate(template, resultContext.ResultContextObject.Token, model.EmailTemplateText, endDate);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailConfiguration.Email);
                    mail.To.Add(emailAddress);
                    mail.Subject = model.EmailTemplateHeader;
                    mail.Body = template;
                    mail.IsBodyHtml = true;

                    await this.SendEmailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.InnerException?.Message);
            }
        }

        private string UpdatePlaceHoldersNewRegistration(string template, string token)
        {
            string appDomain = GetAppDomain();

            string confirmationLink = "/ConfirmNewRegistration?Token={0}";

            var placeHolders = new List<KeyValuePair<string, string>>()            {

                new KeyValuePair<string, string>("{{Link}}", string.Format(appDomain + confirmationLink, HttpUtility.UrlEncode(token)))
            };


            foreach (var placeHolder in placeHolders)
            {
                if (template.Contains(placeHolder.Key))
                {
                    template = template.Replace(placeHolder.Key, placeHolder.Value);
                }
            }

            return template;
        }

        private string UpdatePlaceHoldersSurveyEmailTemplate(string template, string token, string emailText, string endDate)
        {
            string appDomain = this.GetAppDomain();

            string surveyLink = "/SurveyFilingOut?Token={0}";

            var placeHolders = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("{{Link}}", string.Format(appDomain + surveyLink, HttpUtility.UrlEncode(token))),
                new KeyValuePair<string, string>("{{EmailText}}", string.Format(emailText)),
                new KeyValuePair<string, string>("{{SurveyEndDate}}", string.Format(endDate))
            };

            foreach (var placeHolder in placeHolders)
            {
                if (template.Contains(placeHolder.Key))
                {
                    template = template.Replace(placeHolder.Key, placeHolder.Value);
                }
            }

            return template;
        }

        public async Task SendEmailNewRegistrationUserPass(ResultContext<ApplicationUserVM> resultContext)
        {
            try
            {
                var template = File.ReadAllText(String.Format(mailTemplates, "NewRegistrationUserPass"));
                template = this.UpdatePlaceHoldersNewRegistrationUserPass(template, resultContext.ResultContextObject);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailConfiguration.Email);
                    mail.To.Add(resultContext.ResultContextObject.Email);
                    mail.Subject = $"Достъп до информационната система на НАПОО";
                    mail.Body = template;
                    mail.IsBodyHtml = true;

                    await this.SendEmailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.InnerException?.Message);
            }
        }

        public async Task<string> SendEmailNewRegistrationFromImport(ResultContext<ApplicationUserVM> resultContext, bool isImportExternalExpert = false)
        {
            string emailText = string.Empty;
            try
            {
                var templateName = isImportExternalExpert
                    ? "NewRegistrationEEFromImportUserPass"
                    : "NewRegistrationFromImportUserPass";
                var template = File.ReadAllText(String.Format(mailTemplates, templateName));
                var placeHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", resultContext.ResultContextObject.UserName),
                    new KeyValuePair<string, string>("{{Password}}", resultContext.ResultContextObject.Password),
                    new KeyValuePair<string, string>("{{Link}}", "https://is2.navet.government.bg"),
                    new KeyValuePair<string, string>("{{FirstName}}", resultContext.ResultContextObject.FirstName),
                };

                foreach (var placeHolder in placeHolders)
                {
                    if (template.Contains(placeHolder.Key))
                    {
                        template = template.Replace(placeHolder.Key, placeHolder.Value);
                    }
                }

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailConfiguration.Email);
                    mail.To.Add(resultContext.ResultContextObject.Email);
                    mail.Subject = "Достъп до тестова среда за самоподготовка в новата информационна система на НАПОО";
                    mail.Body = template;
                    mail.IsBodyHtml = true;

                    await this.SendEmailAsync(mail);
                }

                if (isImportExternalExpert)
                {
                    emailText = @$"Здравейте, {resultContext.ResultContextObject.FirstName}! 

Във връзка с обучение за новата ИС на НАПОО, приложено Ви изпращаме данни за потребителски акаунт и достъп до тестова среда за самоподготовка.

Адрес на тестовата среда на ИС: https://is2.navet.government.bg

Потребител: {resultContext.ResultContextObject.UserName}
Парола: {resultContext.ResultContextObject.Password}

Можете да промените Вашата парола след успешен вход в информационната система.

Поздрави,
Екипът на ""СМ-ИНДЕКС"" ДЗЗД";
                }
                else
                {
                    emailText = @$"Здравейте, {resultContext.ResultContextObject.FirstName}! 

Във връзка с направена от Вас регистрация и участие в обучение за новата ИС на НАПОО, приложено Ви изпращаме данни за потребителски акаунт и достъп до тестова среда за самоподготовка.

Адрес на тестовата среда на ИС: https://is2.navet.government.bg

Потребител: {resultContext.ResultContextObject.UserName}
Парола: {resultContext.ResultContextObject.Password}

Можете да промените Вашата парола след успешен вход в информационната система.

Поздрави,
Екипът на ""СМ-ИНДЕКС"" ДЗЗД";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.InnerException?.Message);
            }

            return emailText;
        }
        
        public async Task<string> SendEmailNewRegistrationFromImportUsersForCandidateProvider(ResultContext<ApplicationUserVM> resultContext, bool isImportExternalExpert = false)
        {
            string emailText = string.Empty;
            try
            {
                var templateName = isImportExternalExpert
                    ? "NewRegistrationFromImportUsersExpertToCandidateProvider"
                    : "NewRegistrationFromImportUsersToCandidateProvider";
                var template = File.ReadAllText(String.Format(mailTemplates, templateName));
                var placeHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", resultContext.ResultContextObject.UserName),
                    new KeyValuePair<string, string>("{{Password}}", resultContext.ResultContextObject.Password),
                    new KeyValuePair<string, string>("{{Link}}", "https://is.navet.government.bg"),
                    new KeyValuePair<string, string>("{{FirstName}}", resultContext.ResultContextObject.FirstName),
                };

                foreach (var placeHolder in placeHolders)
                {
                    if (template.Contains(placeHolder.Key))
                    {
                        template = template.Replace(placeHolder.Key, placeHolder.Value);
                    }
                }

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailConfiguration.Email);
                    mail.To.Add(resultContext.ResultContextObject.Email);
                    mail.Subject = "Достъп до информационната система на НАПОО";
                    mail.Body = template;
                    mail.IsBodyHtml = true;

                    await this.SendEmailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.InnerException?.Message);
            }

            return emailText;
        }

        public async Task SendUserNewPass(ResultContext<ApplicationUserVM> resultContext)
        {
            try
            {
                var template = File.ReadAllText(String.Format(mailTemplates, "UserNewPassword"));
                template = this.UpdatePlaceHoldersNewRegistrationUserPass(template, resultContext.ResultContextObject);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailConfiguration.Email);
                    mail.To.Add(resultContext.ResultContextObject.Email);
                    mail.Subject = $"Достъп до информационната система на НАПОО";
                    mail.Body = template;
                    mail.IsBodyHtml = true;

                    await this.SendEmailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.InnerException?.Message);
            }
        }

        private string UpdatePlaceHoldersNewRegistrationUserPass(string template, ApplicationUserVM createUserVM)
        {

            var placeHolders = new List<KeyValuePair<string, string>>()            {

               new KeyValuePair<string, string>("{{UserName}}", createUserVM.UserName),
                new KeyValuePair<string, string>("{{Password}}", createUserVM.Password)
            };


            foreach (var placeHolder in placeHolders)
            {
                if (template.Contains(placeHolder.Key))
                {
                    template = template.Replace(placeHolder.Key, placeHolder.Value);
                }
            }

            return template;
        }
        private string UpdatePlaceHoldersRejectRegistration(string template, string rejectReason)
        {

            var placeHolders = new List<KeyValuePair<string, string>>()            {

               new KeyValuePair<string, string>("{{RejectReason}}", rejectReason)
            };


            foreach (var placeHolder in placeHolders)
            {
                if (template.Contains(placeHolder.Key))
                {
                    template = template.Replace(placeHolder.Key, placeHolder.Value);
                }
            }

            return template;
        }

        public async Task SendMailForAwaitingConfirmationNapoo(List<Person> persons, ResultContext<CandidateProvider> resultContext)
        {

            foreach (var person in persons)
            {
                try
                {
                    var template = File.ReadAllText(String.Format(mailTemplates, "MailForAwaitingConfirmationNapoo"));
                    template = await this.UpdatePlaceHoldersForAwaitingConfirmationNapoo(template, person, resultContext.ResultContextObject);

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress(emailConfiguration.Email);
                        mail.To.Add(person.Email);
                        mail.Subject = $"Получена нова форма за електронна регистрация в ИС";
                        mail.Body = template;
                        mail.IsBodyHtml = true;

                        await this.SendEmailAsync(mail);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.StackTrace);
                    _logger.LogError(ex.InnerException?.Message);
                }
            }

        }

        private async Task<string> UpdatePlaceHoldersForAwaitingConfirmationNapoo(string template, Person person, CandidateProvider resultContextObject)
        {
            var kvApplicationType = await this.dataSourceService.GetKeyValueByIdAsync(resultContextObject.IdTypeLicense);
            var applicationType = kvApplicationType.KeyValueIntCode == "LicensingCPO"
                ? "ЦПО"
                : "ЦИПО";
            var placeHolders = new List<KeyValuePair<string, string>>()            {

               new KeyValuePair<string, string>("{{ProviderOwner}}", resultContextObject.ProviderOwner),
               new KeyValuePair<string, string>("{{ProviderBulstat}}", resultContextObject.PoviderBulstat),
               new KeyValuePair<string, string>("{{AttorneyName}}", resultContextObject.AttorneyName),
               new KeyValuePair<string, string>("{{ApplicationType}}", applicationType)
            };


            foreach (var placeHolder in placeHolders)
            {
                if (template.Contains(placeHolder.Key))
                {
                    template = template.Replace(placeHolder.Key, placeHolder.Value);
                }
            }

            return template;
        }

        public async Task SendEmailFromNotification(Person personTo, Person personFrom, Notification notification, string about, string notificationText)
        {

            try
            {
                var template = File.ReadAllText(String.Format(mailTemplates, "NotificationToEmail"));
                //  string name = repository.GetByIdAsync<Person>(notification.IdPersonFrom).Result.FirstName + " " + repository.GetByIdAsync<Person>(notification.IdPersonFrom).Result.FamilyName;
                template = this.UpdatePlaceHoldersNotificationToEmail(template, notification, personFrom.FirstName + " " + personFrom.FamilyName);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailConfiguration.Email);
                    mail.To.Add(personTo.Email);
                    mail.Subject = $"Ново известие";
                    mail.Body = template;
                    mail.IsBodyHtml = true;

                    await this.SendEmailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.InnerException?.Message);
            }
        }

        private string UpdatePlaceHoldersNotificationToEmail(string template, Notification notification, string personFromName)
        {
            var placeHolders = new List<KeyValuePair<string, string>>()            {

               new KeyValuePair<string, string>("{{About}}", notification.About),
               new KeyValuePair<string, string>("{{NotificationText}}", notification.NotificationText),
               new KeyValuePair<string, string>("{{SendDate}}", notification.SendDate.ToString()),
               new KeyValuePair<string, string>("{{PersonFrom}}", personFromName)
            };


            foreach (var placeHolder in placeHolders)
            {
                if (template.Contains(placeHolder.Key))
                {
                    template = template.Replace(placeHolder.Key, placeHolder.Value);
                }
            }

            return template;
        }



        public async Task SendCustomEmail(MailMessage mail)
        {

            mail.From = new MailAddress(emailConfiguration.Email);


            await this.SendEmailAsync(mail);

        }
    }
}
