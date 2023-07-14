using System;
using System.Collections.Generic;
using System.Text;

namespace ISNAPOO.Common.Constants
{
    public static class GlobalConstants
    {
        public const string DATE_FORMAT = "dd.MM.yyyy";
        public const string DATETIME_FORMAT = "dd.MM.yyyy HH:mm";
        public const string TIME_FORMAT = "hh:mm";
        public const string DATE_FORMAT_DELETED_FILE = "yyyyMMddhhmmss";

        #region DocumentTypeLicensing
        public const string CHANCE_LICENSING = "ChanceLicensing";
        public const string LICENSING_CPO = "LicensingCPO";
        public const string LICENSING_CIPO = "LicensingCIPO";
        #endregion

        #region TrainingCourse
        public const string ENTRY_FROM = "EntryFrom";
        public const string TRAINING_PROGRAMS_SPK = "TrainingProgramsSPK";
        public const string UPCOMING_COURSES_SPK = "UpcomingCoursesSPK";
        public const string CURRENT_COURSES_SPK = "CurrentCoursesSPK";
        public const string COMPLETED_COURSES_SPK = "CompletedCoursesSPK";
        public const string ARCHIVED_COURSES_SPK = "ArchivedCoursesSPK";
        public const string TRAINING_VALIDATION_SPK = "TrainingValidationSPK";

        public const string TRAINING_PROGRAMS_PP = "TrainingProgramsPP";
        public const string UPCOMING_COURSES_PP = "UpcominigCoursesPP";
        public const string CURRENT_COURSES_PP = "CurrentCoursesPP";
        public const string COMPLETED_COURSES_PP = "CompletedCoursesPP";
        public const string ARCHIVED_COURSES_PP = "ArchivedCoursesPP";
        public const string TRAINING_VALIDATION_PP = "TrainingValidationPP";

        public const string UPCOMING_COURSES_LC = "UpcomingCoursesLC";
        public const string CURRENT_COURSES_LC = "CurrentCoursesLC";
        public const string COMPLETED_COURSES_LC = "CompletedCoursesLC";
        public const string ARCHIVED_COURSES_LC = "ArchivedCoursesLC";

        public const string COURSE_GRADUATES_SPK = "CourseGraduatesSPK";
        public const string COURSE_GRADUATES_LC = "CourseGraduatesLC";
        public const string COURSE_GRADUATES_PP = "CourseGraduatesPP";

        public const string STATE_EXAM_SPK = "StateExamSPK";
        public const string STATE_EXAM_LC = "StateExamLC";
        public const string STATE_EXAM_PP = "StateExamPP";

        public const string COURSE_DUPLICATES_SPK = "CourseDuplicatesSPK";
        public const string VALIDATION_DUPLICATES_SPK = "ValidationDuplicatesSPK";
        public const string COURSE_DUPLICATES_PP = "CourseDuplicatesPP";
        public const string VALIDATION_DUPLICATES_PP = "ValidationDuplicatesPP";
        #endregion

        #region  Claims

        public const string ID_CANDIDATE_PROVIDER = "ID_CANDIDATE_PROVIDER";
        public const string ID_USER = "ID_USER";
        public const string ID_PERSON = "ID_PERSON";
        public const string PERSON_FULLNAME = "PERSON_FULLNAME";
        #endregion

        #region Order Type
        public const string ORDER_ADD = "Вписване";
        public const string ORDER_CHANGE = "Промяна";
        public const string ORDER_REMOVE = "Отпадане";
        #endregion

        #region ViewModel
        public const int MAX_VALUE_FOR_REQUIRED_ID = int.MaxValue;
        #endregion

        #region Constants Id
        public const string INVALID_ID_STRING = "-1";
        public const int INVALID_ID = -1;
        public const int INVALID_ID_ZERO = 0;
        public const string INVALID_ID_ZERO_STRING = "0";
        #endregion

        #region Constants not selected Value
        public const string NOT_SELECTED_LIST_VALUE_SHORT = "-";
        #endregion

        #region LocalStorage Data
        public const string LocalStorage_SPPOOTreeGridDataExpandedNode = "SPPOOTreeGridDataExpandedNode";

        public const string LocalStorage_MenuExpandedNodes = "MenuExpandedNodes";

        public const string LocalStorage_NKPDExpandedNodes = "NKPDExpandedNodes";

        public const string LocalStorage_Accessibility_Font = "Accessibility_FONT";

        public const string LocalStorage_Accessibility_Color = "Accessibility_COLOR";
        public const string LocalStorage_Accessibility_Underline_Links = "Accessibility_UNDERLINE_LINKS";

        #endregion

        #region Token constants

        public const string TOKEN_SECRET = "-->>>Аsk NAPOO<<<---"; // Тип за експертите

        public const int MAX_MINUTE_VALID_TOKEN = 52560000;//60*24*365*100 - 100 години
        public const int MAX_DEFAULT_FILE_SIZE = 5120;// 5 MB



        /// <summary>
        /// Време преди да се препрати към лоигин стартаницата в милисекудни
        /// </summary>
        public const int TIME_TO_REDIRECT_AFTER_REGISTRATION = 10000; 
        public const int MINUTE_ONE = 1;//една минута
        public const int MINUTE_FIVE = 5;//5 минути 
        public const int MINUTES_SIXTY = 60;//60 минути 


        public const string TOKEN_EXPERT_TYPE_KEY = "ExpertType"; // Тип за експертите
        public const string TOKEN_EXPERT_COMMISSIONS_VALUE = "ExpertCommissions"; // Експертна комисия
        public const string TOKEN_EXPERT_EXTERNAL_VALUE = "ExternalExperts"; //Външни експерти
        public const string TOKEN_EXPERT_NAPOO_EMPLOYEES_VALUE = "NapooEmployees"; //Служители на НАПОО
        public const string TOKEN_EXPERT_DOC_WORK_GROUP_VALUE = "DocWorkGroup"; //Работни групи/ Рецензенти на ДОС 

        public const string TOKEN_LICENSING_TYPE_KEY = "LicensingType"; //Вид на лицензията
        public const string TOKEN_LICENSING_CPO_VALUE = "LicensingCPO"; //Лицензиране на ЦПО
        public const string TOKEN_LICENSING_CIPO_VALUE = "LicensingCIPO"; //Лицензиране на ЦИПО
        public const string TOKEN_CIPO_VALUE = "LicensingCIPO"; //CIPO
        public const string TOKEN_CPO_VALUE = "LicensingCPO"; //CPO

        public const string TOKEN_LICENSING_CPO_APPLICATION_LICENSE_VALUE = "ApplicationCPOLicense"; //Заявление за лицензиране на ЦПО
        public const string TOKEN_LICENSING_CPO_APPLICATION_LICENSE_NAPOO_VALUE = "ApplicationCPOLicenseNAPOO"; //Подадени заявления за лицензиране на ЦПО към НАПОО
        public const string TOKEN_LICENSING_CIPO_APPLICATION_LICENSE_VALUE = "ApplicationCIPOLicense"; //Заявление за лицензиране на ЦИПО
        public const string TOKEN_LICENSING_CIPO_APPLICATION_LICENSE_NAPOO_VALUE = "ApplicationCIPOLicenseNAPOO"; //Подадени заявления за лицензиране на ЦИПО към НАПОО
        public const string TOKEN_LICENSING_MODIFICATION_LICENSE_VALUE = "ModificationLicense"; //Изменение на лицензията
        public const string TOKEN_CPO_PROFILE_VALUE = "ProfileCPO"; //Профил на ЦПО
        public const string TOKEN_CIPO_PROFILE_VALUE = "CIPOProfile"; //Профил на ЦИПО

        public const string TOKEN_DOCUMENT_OFFER_TYPE_KEY = "DocumentOffer"; //Вид на публикувани документи
        public const string TOKEN_BORSA_DOCUMENTS_VALUE = "BorsaDocuments"; //За меню борса с документи
        public const string TOKEN_BORSA_DOCUMENTS_CPO_VALUE = "BorsaDocumentsCPO"; //За меню публикуване на документ на борса

        public const string TOKEN_SURVEYRESULT_KEY = "Identifier"; // Ключ от токен за решаване на анкета
        public const string TOKEN_SURVEYLIST_GRADUATES_REALIZATION = "ClientRealisation"; // Ключ от токен за проследяване реализацията на завършилите ПО
        public const string TOKEN_CPO_SURVEYLIST_GRADUATES_REALIZATION = "CPOClientRealisation"; // Ключ от токен за проследяване реализацията на завършилите ПО
        public const string TOKEN_SURVEYLIST_MEASUREMENT_SATISFACTION = "ClientSatisfaction"; // Ключ от токен за измерване на степента на удовлетвореност
        public const string TOKEN_CPO_SURVEYLIST_MEASUREMENT_SATISFACTION = "CPOClientSatisfaction"; // Ключ от токен за измерване на степента на удовлетвореност

        public const string TOKEN_RIDPK_DOCUMENTLIST_COURSE = "RIDPKCourse"; // Ключ от токен за проверка на издаваните от ЦПО документи за ПК
        public const string TOKEN_RIDPK_DOCUMENTLIST_VALIDATION = "RIDPKValidation"; // Ключ от токен за проверка на издаваните от ЦПО документи за ПК, свързани с валидиране

        public const string TOKEN_ADMIN_NOTIFICATIONLIST_CPO = "AdminNotificationListCPO"; // Ключ от токен за проверка на на изпратените известия от ИС в лицензиране ЦПО
        public const string TOKEN_ADMIN_NOTIFICATIONLIST_CIPO = "CIPOAdminNotificationList"; // Ключ от токен за проверка на на изпратените известия от ИС в лицензиране ЦИПО


        #endregion

        #region РОЛИ
        public const string EXTERNAL_EXPERTS = "EXTERNAL_EXPERTS"; //Роля Външни експерти 
        public const string EXPERT_COMMITTEES = "EXPERT_COMMITTEES"; //Роля Експертни комисии
        public const string ACCESS_CPO = "ACCESS_CPO"; //Роля за ЦПО
        public const string NAPOO_Expert = "NAPOO_Expert";//Роля НАПОО служители
        public const string EXPERT_DOS = "EXPERT_DOS"; //Роля Рецензенти на ДОС
        #endregion

        #region File upload settings
        public const string ALLOWED_EXTENSIONS_ALL = ".pdf, .csv, .svg, .dxf, .jpeg, .jpg, .gif, .doc, .docx, .xls, .xlsx, .xlsm"; //Всички позволени разширения за файлове
        public const string ALLOWED_EXTENSIONS_EXCEL = ".xls, .xlsx, .xlsm"; //Всички позволени разширения за Excel файлове
        public const string ALLOWED_EXTENSIONS_WORD = ".doc, .docx"; //Всички позволени разширения за Word файлове
        public const string ALLOWED_EXTENSIONS_PDF = ".pdf"; //Всички позволени разширения за PDF файлове
        #endregion

        #region RIDPK Operations
        public const string RIDPK_OPERATION_APPROVE = "RIDPK_OPERATION_APPROVE";
        public const string RIDPK_OPERATION_REJECT = "RIDPK_OPERATION_REJECT";
        public const string RIDPK_OPERATION_RETURN = "RIDPK_OPERATION_RETURN";
        public const string RIDPK_OPERATION_FILE_IN = "RIDPK_OPERATION_FILE_IN";
        #endregion

        public const string RegexExpressionValidEmail = "(?:[a-z0-9!#$%&'*+\\=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+\\=?^_`{|}~-]+)*|\"\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\"\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";
        public const string RegexExpressionThreeNamesCyrilic = " *([А-яа-я-?]{2,}) +([А-яа-я]{2,}) * +([А-яа-я-?]{2,}) *";


        //URL encryption/decryption key and IV
        // The key used for generating the encrypted string
        public const string ENCRYPTION_KEY = "-->>>Аsk NAPOO<<<---";//random keyboard strokes		
        // The Initialization Vector for the DES encryption routine
        public static readonly byte[] ENCRYPTION_IV = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };//-->>>Аsk NAPOO<<<---


	}
}
