using System;
using System.Collections.Generic;
using System.Text;

namespace ISNAPOO.Common.Enums
{
    public static class EMIEnums
    {
        public enum GroupSecuritySettings
        {
            Admin,
            Common,
            Share
        }

        public enum SecuritySettings
        {
            #region menu Administration

            /***********************************/
            /* Действия за ИП Адрес*/
            /**********************************/
            EditIpAddress,
            ShowIpAddressList,
            ViewIpAddress,
            EditMenu,

            /***********************************/
            /* Действия за Модул*/
            /**********************************/
            EditModule,
            ShowModuleList,

            /***********************************/
            /* Действия за Администриране*/
            /**********************************/
            EditPermittedAction,
            EditRole,
            EditSetting,
            EditUser,
            EditGroup,
            EditKeyValue,
            EditNotification,
            EditLogger,
            ViewKeyTypeKeyValue,
            ShowAllNotification,

            #endregion

          

            

           
     

        

       
        }

        public enum ApplicationSettings
        {
            SystemShortName,
            MakeLogInDB,
            ResourceFolderPath,
            WebResourcesFolderName,            
            UseCurrentRequestScheme,
            UseCurrentRequestPort,
            HTTPPortForResource,
            NotificationRefreshDefaultPeriod,
            CronProcessStart,
            CronProcessStartPeriod,
            MailServer,
            MailServerPort,
            MailFrom,
            MailFromPassword,
            EmailEnableSsl,
            DefaultBccEmail,
            DefaultCcEmail,
            SendExternalMail,
            ExternalMailAuthenticate,
            DomainServerName,
            DomainUserName,
            DomainUserPass
        }

        public enum ApplicationSettingsClass
        {
            String,
            Integer,
            EMail,
            Boolean,
            Double
        }

        public enum MessageType
        {
            Message = 0,
            Error = 1
        }

        public enum NKPDLevel 
        {
            ClassCode,
            SubclassCode,
            GroupCode,
            IndividualGroup,
            NkpdCode
        }
    }
}
