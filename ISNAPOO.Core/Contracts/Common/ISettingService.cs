using Data.Models.Data.Common;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using RegiX.Class.AVBulstat2.GetStateOfPlay;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface ISettingService :  IBaseService
    {
        static List<Setting> GetSettings()
        {
            var settings = new List<Setting>();

            #region PasswordPolicySettingsParameters
            //Require Digit
            settings.Add(new Setting() { SettingName = "Минимум цифри", SettingDescription = "Минимум цифра в паролата.", SettingIntCode = "MinimumNumbers", SettingValue = "2", SettingClass = "int" });
            //Require Lowercase
            settings.Add(new Setting() { SettingName = "Разрешени символи", SettingDescription = "Разрешени специални символи в паролата.", SettingIntCode = "SpecialCharacters", SettingValue = "~@#$%^&*_<>!?|~^/+{}();", SettingClass = "string" });
            //Require NonAlphanumeric
            settings.Add(new Setting() { SettingName = "Минимум големи букви", SettingDescription = "Минимум големи букви в паролата.", SettingIntCode = "MinimumCapitalLetters", SettingValue = "2", SettingClass = "int" });
            //Require UpperCase
            settings.Add(new Setting() { SettingName = "Минимум малки букви", SettingDescription = "Минимум малки букви в паролата.", SettingIntCode = "MinimumLowLetters", SettingValue = "2", SettingClass = "int" });
            //Required Length
            settings.Add(new Setting() { SettingName = "Минимум дължина", SettingDescription = "Минимум дължина за една парола.", SettingIntCode = "MinimumPasswordLength", SettingValue = "10", SettingClass = "int" });
            //Required Unique Characters
            settings.Add(new Setting() { SettingName = "Минимален уникални символи", SettingDescription = "Минимум уникални символи за една парола.", SettingIntCode = "MinimumSpecialCharacters", SettingValue = "2", SettingClass = "int" });

            #endregion

            settings.Add(new Setting() { SettingName = "ID на потребител асоциран със системата", SettingDescription = "ID на потребител асоциран със системата. Висчки автоматични действия ще се изпълняват под това ID", SettingIntCode = "UserIDBindWithSystem", SettingValue = "-1", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Преобразуване на известие към имейл", SettingDescription = "Указва дали известието да се преобразува в имейл.", SettingIntCode = "NotificationToEmail", SettingValue = "true", SettingClass = "bool" });

            #region EmailConfigurationSettings
            //MailServer
            settings.Add(new Setting() { SettingName = "Сървър за електронна поща", SettingDescription = "Дефинира сървъра за електронната поща", SettingIntCode = "MailServer", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            //MailPort
            settings.Add(new Setting() { SettingName = "Порт на електронната поща", SettingDescription = "Дефинира порта на електронната поща", SettingIntCode = "MailPort", SettingValue = "587", SettingClass = "int" });
            //MailName
            settings.Add(new Setting() { SettingName = "Име на електронна поща", SettingDescription = "Дефинира името на електронната поща", SettingIntCode = "MailName", SettingValue = "SMC Developer", SettingClass = "string" });
            //MailEmail
            settings.Add(new Setting() { SettingName = "Имейл на електронна поща", SettingDescription = "Дефинира имейла на електронната поща", SettingIntCode = "MailEmail", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            //MailPassword
            settings.Add(new Setting() { SettingName = "Парола на електронна поща", SettingDescription = "Дефинира паролата на електронната поща", SettingIntCode = "MailPassword", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            //MailUseSSL
            settings.Add(new Setting() { SettingName = "Използване на SSL за поща", SettingDescription = "Представлява използването на SSL за поща", SettingIntCode = "MailUseSSL", SettingValue = "false", SettingClass = "bool" });
            //MailBccName
            settings.Add(new Setting() { SettingName = "Име на получател на скрито копие", SettingDescription = "Дефинира името на получателя на скритото копие", SettingIntCode = "MailBccName", SettingValue = "", SettingClass = "string" });
            //MailBccEmail
            settings.Add(new Setting() { SettingName = "Имейл на получател на скрито копие", SettingDescription = "Дефинира имейла на получателя на скритото копие", SettingIntCode = "MailBccEmail", SettingValue = "", SettingClass = "string" });
            //MailCcName
            settings.Add(new Setting() { SettingName = "Име на получател на явно копие", SettingDescription = "Дефинира името на получателя на явното копие", SettingIntCode = "MailCcName", SettingValue = "", SettingClass = "string" });
            //MailCcEmail
            settings.Add(new Setting() { SettingName = "Имейл на получател на явно копие", SettingDescription = "Дефинира имейла на получателя на явното копие", SettingIntCode = "MailCcEmail", SettingValue = "", SettingClass = "string" });
            //MailAllowSendMail
            settings.Add(new Setting() { SettingName = "Позволение за изпращане на имейл", SettingDescription = "Дефинира позволение за изпращане на имейл", SettingIntCode = "MailAllowSendMail", SettingValue = "true", SettingClass = "bool" });
            #endregion

            #region Time Stamp Authority
            settings.Add(new Setting() { SettingName = "Сървър за получаване на Time Stamp Request", SettingDescription = "Указва кой сървър да се ползва за POST заявка за генериране на TimeStampRequest", SettingIntCode = "TSA", SettingValue = "https://freetsa.org/tsr", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Потребител за сървър за получаване на Time Stamp Request ", SettingDescription = "Указва потребителя за POST заявка за генериране на TimeStampRequest", SettingIntCode = "TSAUser", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Парола за сървър за получаване на Time Stamp Request ", SettingDescription = "Указва паролата за POST заявка за генериране на TimeStampRequest", SettingIntCode = "TSAPassword", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });

            #endregion

            #region ApplicationSetting
            //ResourcesFolderName
            settings.Add(new Setting() { SettingName = "Папка с ресурси", SettingDescription = "Дефинира местоположенитето на папката с ресурси", SettingIntCode = "AppSettingResourcesFolderName", SettingValue = "C:\\Resources_NAPOO\\Logs\\log-{Date}.txt", SettingClass = "string" });
            //Host
            settings.Add(new Setting() { SettingName = "Хост", SettingDescription = "Дефинира хоста на приложението", SettingIntCode = "AppSettingHost", SettingValue = "localhost", SettingClass = "string" });
            //Port
            settings.Add(new Setting() { SettingName = "Порт", SettingDescription = "Дефинира порта на приложението", SettingIntCode = "AppSettingPort", SettingValue = "44395", SettingClass = "string" });
            //HttpScheme
            settings.Add(new Setting() { SettingName = "Http схема", SettingDescription = "Дефинира http схемата", SettingIntCode = "AppSettingHttpScheme", SettingValue = "https", SettingClass = "string" });
            //InternalEAuthURL
            settings.Add(new Setting() { SettingName = "Вътрешен EAuth URL адрес", SettingDescription = "Дефинира вътрешения EAuth URL адрес", SettingIntCode = "AppSettingInternalEAuthURL", SettingValue = "https://eauth2.smcon.com/Home/CertificateAuthV2", SettingClass = "string" });
            
            settings.Add(new Setting() { SettingName = "Връзка към деловодна система", SettingDescription = "Връзка към деловодна система", SettingIntCode = "AppSettingDocuServiceURL", SettingValue = "https://test.indexbg.bg/DocuWork/DocuService", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Връзка към деловодна система", SettingDescription = "Настройка за крайна точка. Оказва дали да се ползва HTTP (DocuServicePortHTTP) или HTTPS(DocuServicePort).", SettingIntCode = "EndpointConfigurationDocuService", SettingValue = "DocuServicePortHTTP", SettingClass = "string" });
            

            settings.Add(new Setting() { SettingName = "Връзка към RegiX", SettingDescription = "Връзка към RegiX", SettingIntCode = "AppSettingRegixURL", SettingValue = "https://regix-service-test.egov.bg/RegiX/RegiXEntryPoint.svc/basic", SettingClass = "string" });

            #endregion

            #region FileUploadSettings
            settings.Add(new Setting() { SettingName = "Максимален размер на файл", SettingDescription = "Максимален размер на файл за качване в ИС в байтове", SettingIntCode = "MaxSizeFileUpload", SettingValue = "5000000", SettingClass = "double" });
            settings.Add(new Setting() { SettingName = "Минимален размер на файл", SettingDescription = "Минимален размер на файл за качване в ИС в байтове", SettingIntCode = "MinSizeFileUpload", SettingValue = "1", SettingClass = "double" });
            settings.Add(new Setting() { SettingName = "Максимален брой файлове за прикачване", SettingDescription = "Максимален брой файлове за прикачване", SettingIntCode = "MaxFilesCount", SettingValue = "5", SettingClass = "int" });
            #endregion

            #region RequestDocumentModule
            settings.Add(new Setting() { SettingName = "Период за подаване на заявка за документация", SettingDescription = "Период за подаване на заявка за документация (ако датата на подаване е в срок - календарната година е текущата - 1. Ако не е в срок - календарната година е текущата.)", SettingIntCode = "RequestDocumentPeriod", SettingValue = "30.04", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Период за подаване на отчет за документация", SettingDescription = "Период за подаване на отчет за документация", SettingIntCode = "DocumentReportPeriod", SettingValue = "30.04", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Срок за изпращане на известие преди изтичане на краен срок", SettingDescription = "Срок за изпращане на известие преди изтичане на краен срок за подаване на заявка за документация и подаване на отчет за документация", SettingIntCode = "RequestDocumentTerm", SettingValue = "7", SettingClass = "int" });
            #endregion

            #region RegiXLog
            settings.Add(new Setting() { SettingName = "Наименование на администрацията, ползваща системата", SettingDescription = "Наименование на администрацията, ползваща системата", SettingIntCode = "AdministrationName", SettingValue = "НАПОО", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Идентификационен код на администрация (oID от eAuth)", SettingDescription = "Идентификационен код на администрация (oID от eAuth)", SettingIntCode = "AdministrationOId", SettingValue = "2.16.100.1.1.23.1.3", SettingClass = "string" });
            #endregion

            #region RIDPK deadline
            settings.Add(new Setting() { SettingName = "Срок (в дни) за коригиране на данни за документ за ПК", SettingDescription = "Срок (в дни) за коригиране на данни за документ за ПК", SettingIntCode = "RIDPKCorrectionPeriod", SettingValue = "14", SettingClass = "int" });
            #endregion


            settings.Add(new Setting() { SettingName = "Време на активна сесисята", SettingDescription = "Време на активна сесисята", SettingIntCode = "IdleTimeout", SettingValue = "60", SettingClass = "int" });
            
            settings.Add(new Setting() { SettingName = "Време на активен линк за потвърждение на е-маил", SettingDescription = "Определя колко минути е валиден линка за потърждаване на email адрес", SettingIntCode = "MinutesForEmailValidation", SettingValue = "60", SettingClass = "int" });

            settings.Add(new Setting() { SettingName = "Акаунт за деловодна система", SettingDescription = "ID на профил за деловодна система", SettingIntCode = "IndexUserId", SettingValue = "2642", SettingClass = "int" });

            settings.Add(new Setting() { SettingName = "Роли за лицензирано ЦПО", SettingDescription = "Ролите които ще се дадът на създадените акаунти разделени със ,", SettingIntCode = "LicensedCPO", SettingValue = "CPO", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Роли за лицензирано ЦИПО", SettingDescription = "Ролите които ще се дадът на създадените акаунти разделени със ,", SettingIntCode = "LicensedCIPO", SettingValue = "CIPO", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Роли за не лицензирано ЦПО", SettingDescription = "Ролите които ще се дадът на създадените акаунти разделени със ,", SettingIntCode = "UnicensedCPO", SettingValue = "Candidate_CPO", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Роли за не лицензирано ЦИПО", SettingDescription = "Ролите които ще се дадът на създадените акаунти разделени със ,", SettingIntCode = "UnicensedCIPO", SettingValue = "Candidate_CIPO", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Портове за BISS", SettingDescription = "Портовете които ще се ползват за връзка с BISS", SettingIntCode = "BISSPorts", SettingValue = "53952,53953,53954,53955", SettingClass = "string" });

            settings.Add(new Setting() { SettingName = "Използване на ел.подпис", SettingDescription = "Използване на ел.подпис", SettingIntCode = "Use_E_Signature", SettingValue = "false", SettingClass = "bool" });

            settings.Add(new Setting() { SettingName = "Брой неуспешни опити", SettingDescription = "Колко неуспешни опита може да има един user преди да бъде заключен акаунта му", SettingIntCode = "MaxFailedAccessAttempts", SettingValue = "3", SettingClass = "int" });

            settings.Add(new Setting() { SettingName = "Времетраене за заключване на акаунт", SettingDescription = "Времетраене за заключване на акаунт(Минути)", SettingIntCode = "DefaultLockoutTimeSpan", SettingValue = "2", SettingClass = "int" });

            settings.Add(new Setting() { SettingName = "Брой на разрешени активни профили към ЦПО", SettingDescription = "Брой на разрешени активни профили към ЦПО", SettingIntCode = "MaxNumberOfActiveUsersForCPO", SettingValue = "5", SettingClass = "int" });

            settings.Add(new Setting() { SettingName = "Интервал за предаване на отчет", SettingDescription = "Интервал за предаване на отчет (Дата - дата)", SettingIntCode = "DateRangeForReport", SettingValue = "01.01.2022 - 31.12.2022", SettingClass = "date" });
            
            settings.Add(new Setting() { SettingName = "Време за проверяване на документи", SettingDescription = "През колко време да се проверява дали документите са официални", SettingIntCode = "ProcedureDocumentsSchedule", SettingValue = "120", SettingClass = "int" });
            
            settings.Add(new Setting() { SettingName = "Проверка за официални документи", SettingDescription = "Да се проверяват ли документите дали са официални", SettingIntCode = "ProcedureDocumentsCheck", SettingValue = "true", SettingClass = "bool" });
            
            settings.Add(new Setting() { SettingName = "Интервал за проверка на изпити за валидирани клиенти", SettingDescription = "Интервал за проверка на изпити за валидирани клиенти (Дни)", SettingIntCode = "DaysForValidationClientExamCheck", SettingValue = "10", SettingClass = "int" });
            
            settings.Add(new Setting() { SettingName = "Линк за взимане на документи по oid", SettingDescription = "Линк за взимане на документи по oid", SettingIntCode = "NapooOldISDocsLink", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            
            settings.Add(new Setting() { SettingName = "Брой изпращане на request-и при миграция", SettingDescription = "Брой изпращане на request-и за взимане на документи при миграция", SettingIntCode = "TakeNumberForMigration", SettingValue = "50", SettingClass = "int" });

            #region Pay - EGOV
            settings.Add(new Setting() { SettingName = "Доставчик на ЕАУ", SettingDescription = "Доставчик на ЕАУ", SettingIntCode = "ServiceProviderNamePayEGov", SettingValue = "Национална агенция за професионално образование и обучение", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Име на банката, в която е сметката на доставчика на ЕАУ", SettingDescription = "Име на банката, в която е сметката на доставчика на ЕАУ", SettingIntCode = "ServiceProviderBankPayEGov", SettingValue = "UNICREDIT BULBANK AD", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "BIC код на сметката на доставчика на ЕАУ", SettingDescription = "BIC код на сметката на доставчика на ЕАУ", SettingIntCode = "ServiceProviderBICPayEGov", SettingValue = "UNCRBGSF", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "IBAN код на сметката на доставчика на ЕАУ", SettingDescription = "IBAN код на сметката на доставчика на ЕАУ", SettingIntCode = "ServiceProviderIBANPayEGov", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "код на плащане", SettingDescription = "код на плащане", SettingIntCode = "PaymentTypeCodeEGov", SettingValue = "9", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "тип на документ(референтен документ за плащане)", SettingDescription = "тип на документ(референтен документ за плащане)", SettingIntCode = "PaymentReferenceTypeEGov", SettingValue = "9", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "УРИ на ЕАУ", SettingDescription = "УРИ на ЕАУ", SettingIntCode = "AdministrativeServiceUriEGov", SettingValue = "https://eauth3.smcon.com:8083", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "УРИ на доставчик на ЕАУ", SettingDescription = "УРИ на доставчик на ЕАУ", SettingIntCode = "AdministrativeServiceSupplierUriEGov", SettingValue = "https://eauth3.smcon.com:8083", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "URL за нотификации при смяна на статус на задължение", SettingDescription = "URL за нотификации при смяна на статус на задължение", SettingIntCode = "AdministrativeServiceNotificationURLEGov", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Валута за EGov плащания", SettingDescription = "Валута за EGov плащания", SettingIntCode = "CurrencyPayEGov", SettingValue = "BGN", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Идентификатор на заявка за плащане", SettingDescription = "Идентификатор на заявка за плащане", SettingIntCode = "ClientIdPayEGov", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Секретен ключ за EGov", SettingDescription = "Секретен ключ за EGov", SettingIntCode = "SecretKeyPayEGov", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Тест URL за EGov", SettingDescription = "Тест URL за EGov", SettingIntCode = "BaseURLPayEGov", SettingValue = "https://pay-test.egov.bg:44310/api/v1/eService/", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Дата на изтичане на заявката за плащане", SettingDescription = "Дата на изтичане на заявката за плащане", SettingIntCode = "ExpirationDatePayEGov", SettingValue = "1", SettingClass = "int" });
            settings.Add(new Setting() { SettingName = "Тест URL за плащане", SettingDescription = "Тест URL за плащане", SettingIntCode = "LoginURLForPayEGov", SettingValue = "https://pay-test.egov.bg/", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Тест URL за плащане с код", SettingDescription = "Тест URL за плащане с код", SettingIntCode = "LoginWithCodeURLForPayEGov", SettingValue = "https://pay-test.egov.bg/Home/AccessByCode/", SettingClass = "string" });
            #endregion

            #region Web Services
            
            settings.Add(new Setting() { SettingName = "Потребител Web Service:WebIntegrationService", SettingDescription = "Потребител Web Service:WebIntegrationService", SettingIntCode = "wsUsernameWebIntegrationService", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Парола Web Service:WebIntegrationService", SettingDescription = "Парола Web Service:WebIntegrationService", SettingIntCode = "wsPasswordWebIntegrationService", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });


            settings.Add(new Setting() { SettingName = "Потребител Web Service:AZService", SettingDescription = "Потребител Web Service:AZService", SettingIntCode = "wsUsernameAZService", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });
            settings.Add(new Setting() { SettingName = "Парола Web Service:AZService", SettingDescription = "Парола Web Service:AZService", SettingIntCode = "wsPasswordAZService", SettingValue = "-->>>Аsk NAPOO<<<---", SettingClass = "string" });

         

            #endregion

            return settings;
        }

        Task mergeSettingsAsync();

        Task<int> CreateSetting(SettingVM model);

        Task<IEnumerable<SettingVM>> GetAllSettingsAsync(SettingVM filterSettingVM);

        //Task UpdateSettingeAsync(SettingVM model);
        Task<ResultContext<SettingVM>> UpdateSettingeAsync(ResultContext<SettingVM> model);

        Task<SettingVM> GetSettingByIntCodeAsync( string intCode);

        Task<EmailConfiguration> SetUpEmailConfiguration();
        Task<ApplicationSetting> SetUpАpplicationSetting();
        Task ReloadSettings();
    }
}
