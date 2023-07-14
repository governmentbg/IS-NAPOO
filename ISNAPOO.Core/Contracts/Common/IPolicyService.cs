using Data.Models.Data.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IPolicyService
    {
        /// <summary>
        /// Списък с всички права в системата
        /// </summary>
        /// <returns></returns>
        static List<Policy> GetPolicies()
        {
            var policies = new List<Policy>();
            #region Достъп НАПОО

                #region Основни Обекти

                    #region СППОО
                    policies.Add(new Policy() { PolicyCode = "ShowSPPOOList", PolicyDescription = "СППОО - Списък на професиите за професионално образование и обучение" });
                    policies.Add(new Policy() { PolicyCode = "ViewSPPOOData", PolicyDescription = "СППОО - Преглед на СППОО" });
                    policies.Add(new Policy() { PolicyCode = "ManageSPPOOData", PolicyDescription = "СППОО - Позволава създаване,редактиране и изтриване на СППОО" });
                    #endregion
                    #region ДОС
                    policies.Add(new Policy() { PolicyCode = "ShowDOCList", PolicyDescription = "ДОС - Списък на ДОС" });
                    policies.Add(new Policy() { PolicyCode = "ViewDOCData", PolicyDescription = "ДОС - Преглед на ДОС" });
                    policies.Add(new Policy() { PolicyCode = "ManageDOCData", PolicyDescription = "ДОС - Създаване,редактиране и изтриване на ДОС" });
                    #endregion
                    #region Рамкови Програми
                    policies.Add(new Policy() { PolicyCode = "ShowFPList", PolicyDescription = "Рамкови Програми - Списък на Рамкови Програми" });
                    policies.Add(new Policy() { PolicyCode = "ViewFPData", PolicyDescription = "Рамкови Програми - Преглед на Рамкови Програми" });
                    policies.Add(new Policy() { PolicyCode = "ManageFPData", PolicyDescription = "Рамкови Програми - Създаване,редактиране и изтриване на Рамкови Програми" });
                    #endregion
                    #region Заповеди
                    policies.Add(new Policy() { PolicyCode = "ShowOrderList", PolicyDescription = "Заповеди - Списък на Заповеди" });
                    policies.Add(new Policy() { PolicyCode = "ViewOrderData", PolicyDescription = "Заповеди - Преглед на Заповеди" });
                    policies.Add(new Policy() { PolicyCode = "ManageOrderData", PolicyDescription = "Заповеди - Създаване,редактиране и изтриване на Заповеди" });
            #endregion
                    #region Наредби за правоспособност
            policies.Add(new Policy() { PolicyCode = "ShowLegalCapacityOrdinancesList", PolicyDescription = "Наредби за правоспособност - Списък на Наредби за правоспособност" });
            #endregion
            #endregion
                #region Лицензиране на ЦПО
                    
                    #region Форми за регистрация
            policies.Add(new Policy() { PolicyCode = "ShowRegistrationFormList", PolicyDescription = "Форми за регистрация - Списък на формите" });
                        policies.Add(new Policy() { PolicyCode = "ViewRegistrationFormData", PolicyDescription = "Форми за регистрация - Преглед на Формите" });
                        policies.Add(new Policy() { PolicyCode = "ManageRegistrationFormData", PolicyDescription = "Форми за регистрация - Позволава създаване,редактиране и изтриване на Формите" });
                    #endregion
                    #region Процедури
                        policies.Add(new Policy() { PolicyCode = "ShowApplicationList", PolicyDescription = "Процедури - Списък на Процедурите" });
                        policies.Add(new Policy() { PolicyCode = "ViewApplicationData", PolicyDescription = "Процедури - Преглед на Процедури" });
                        policies.Add(new Policy() { PolicyCode = "ManageApplicationData", PolicyDescription = "Процедури - Позволава създаване,редактиране и изтриване на Процедури" });
                        policies.Add(new Policy() { PolicyCode = "ShowAllCandidateProvider", PolicyDescription = "Позволява да се визуализрат всички Candidate_Provide" });

            #endregion
            #region Лицензирани ЦПО
            //policies.Add(new Policy() { PolicyCode = "ShowApplicationList", PolicyDescription = "Процедури - Списък на Процедурите" });
            //policies.Add(new Policy() { PolicyCode = "ViewApplicationData", PolicyDescription = "Процедури - Преглед на Процедури" });
            //policies.Add(new Policy() { PolicyCode = "ManageApplicationData", PolicyDescription = "Процедури - Позволава създаване,редактиране и изтриване на Процедури" });
            #endregion
            #region Изпратени известия от ИС на НАПОО
            policies.Add(new Policy() { PolicyCode = "ShowAdminNotificationList", PolicyDescription = "Списък с Уведомления/Известия" });
            #endregion
            #endregion
                #region Лицензиране на ЦИПО
                   #region Процедури
                   policies.Add(new Policy() { PolicyCode = "ShowCIPOApplicationList", PolicyDescription = "Процедури - Списък Заявления за лицензиране ЦИПО" });
                   #endregion
                #endregion
                #region Външни експерти и ЕК

            #region Външни експерти
            policies.Add(new Policy() { PolicyCode = "ShowExpertsList", PolicyDescription = "Външни експерти - Списък на Експерти" });
                        policies.Add(new Policy() { PolicyCode = "ViewExpertsData", PolicyDescription = "Външни експерти - Преглед на Експерти" });
                        policies.Add(new Policy() { PolicyCode = "ManageExpertsData", PolicyDescription = "Външни експерти - Позволава създаване,редактиране и изтриване на Експерти" });
            #endregion

            #endregion
                #region Регистри

                    #region Регистър на ЦПО/ЦИПО
                                policies.Add(new Policy() { PolicyCode = "ShowRegisterCPOList", PolicyDescription = "Регистър на ЦПО - Списък на ЦПО" });
                                policies.Add(new Policy() { PolicyCode = "ViewRegisterCPOData", PolicyDescription = "Регистър на ЦПО - Преглед на ЦПО" });
                                policies.Add(new Policy() { PolicyCode = "ManageRegisterCPOData", PolicyDescription = "Регистър на ЦПО - Позволава създаване,редактиране и изтриване на ЦПО" });
                    #endregion
                    #region Подали документи за лицензиране на ЦПО/ЦИПО
                                policies.Add(new Policy() { PolicyCode = "ShowSubmittedDocumentLicenseList", PolicyDescription = "Докумнети за лицензиране - Списък на документи" });
                                policies.Add(new Policy() { PolicyCode = "ViewSubmittedDocumentLicenseData", PolicyDescription = "Докумнети за лицензиране - Преглед на документа" });
                                policies.Add(new Policy() { PolicyCode = "ManageSubmittedDocumentLicenseData", PolicyDescription = "Докумнети за лицензиране - Позволава създаване,редактиране и изтриване на документа" });
                    #endregion
                    #region Регистър на Материално-техническите бази на ЦПО и ЦИПО
                                policies.Add(new Policy() { PolicyCode = "ShowRegisterMTBList", PolicyDescription = "Регистър на МТБ - Списък на МТБ" });
                                policies.Add(new Policy() { PolicyCode = "ViewRegisterMTBData", PolicyDescription = "Регистър на МТБ - Преглед на МТБ" });
                                policies.Add(new Policy() { PolicyCode = "ManageRegisterMTBData", PolicyDescription = "Регистър на МТБ - Позволава създаване,редактиране и изтриване на МТБ" });
                    #endregion
                    #region Регистър на преподавателите в ЦПО
                                policies.Add(new Policy() { PolicyCode = "ShowRegisterTrainerList", PolicyDescription = "Регистър на Преподаватели - Списък на Преподаватели" });
                                policies.Add(new Policy() { PolicyCode = "ViewRegisterTrainerData", PolicyDescription = "Регистър на Преподаватели - Преглед на Преподавател" });
                                policies.Add(new Policy() { PolicyCode = "ManageRegisterTrainerData", PolicyDescription = "Регистър на Преподаватели - Позволава създаване,редактиране и изтриване на Преподавател" });
                    #endregion
                    #region Регистър на ЕРУ по видове професионална подготовка
                                policies.Add(new Policy() { PolicyCode = "ShowRegisterERUList", PolicyDescription = "Регистър на ЕРУ - Списък на ЕРУ от активни ДОС" });
                                policies.Add(new Policy() { PolicyCode = "ViewRegisterERUData", PolicyDescription = "Регистър на ЕРУ - Преглед на ЕРУ" });
                                policies.Add(new Policy() { PolicyCode = "ManageRegisterERUData", PolicyDescription = "Регистър на ЕРУ - Позволава създаване,редактиране и изтриване на ЕРУ" });
                    #endregion

                #endregion
                #region Заявки за документация

                    #region Подадени заявки от ЦПО   
                    policies.Add(new Policy() { PolicyCode = "ShowNAPOODocumentRequestList", PolicyDescription = "Подадени заявки от ЦПО - Списък на Заявките" });
                    policies.Add(new Policy() { PolicyCode = "ViewNAPOODocumentRequestData", PolicyDescription = "Подадени заявки от ЦПО - Преглед на Заявките" });
                    policies.Add(new Policy() { PolicyCode = "ManageNAPOODocumentRequestData", PolicyDescription = "Подадени заявки от ЦПО - Позволава създаване,редактиране и изтриване на Заявките" });
                    #endregion
                    #region Обобщаване на заявките
                    policies.Add(new Policy() { PolicyCode = "ShowSummarizedRequestDocumentList", PolicyDescription = "Обобщаване на заявките - Списък на Заявките" });
                    policies.Add(new Policy() { PolicyCode = "ViewSummarizedRequestDocumentData", PolicyDescription = "Обобщаване на заявките - Преглед на Заявките" });
                    policies.Add(new Policy() { PolicyCode = "ManageSummarizedRequestDocumentData", PolicyDescription = "Обобщаване на заявките - Позволава създаване,редактиране и изтриване на Заявките" });
                    #endregion
                    #region Търсене на фабричен номер
                    policies.Add(new Policy() { PolicyCode = "ShowDocumentFabricNumbersSearchList", PolicyDescription = "Фабричен номер - Списък на Фабричните номера" });
                    policies.Add(new Policy() { PolicyCode = "ViewDocumentFabricNumbersSearchData", PolicyDescription = "Фабричен номер - Преглед на Фабричните номера" });
                    policies.Add(new Policy() { PolicyCode = "ManageDocumentFabricNumbersSearchData", PolicyDescription = "Фабричен номер - Позволава създаване,редактиране и изтриване на Фабричните номера" });
            #endregion
                    #region Налични документи в ЦПО
                    policies.Add(new Policy() { PolicyCode = "ShowControlDocumentList", PolicyDescription = "Списък на наличните документи в ЦПО" });
            #endregion
                    #region Справки печатница
                    policies.Add(new Policy() { PolicyCode = "ShowPrintingHouseReportList", PolicyDescription = "Обобщени заявки за документи от ЦПО - Списък на Заявките за печатницата на МОН" });
            #endregion
            #endregion
                #region Последващ контрол
                    policies.Add(new Policy() { PolicyCode = "ShowFollowUpControlList", PolicyDescription = "Последващ контрол - Списък на данните" });
                    policies.Add(new Policy() { PolicyCode = "ViewFollowUpControlData", PolicyDescription = "Последващ контрол - Преглед на данни" });
                    policies.Add(new Policy() { PolicyCode = "ManageFollowUpControlData", PolicyDescription = "Последващ контрол - Позволава създаване,редактиране и изтриване на данни" });
            #endregion
                #region Годишен отчет
                policies.Add(new Policy() { PolicyCode = "ManageSelfAssessmentReports", PolicyDescription = "Управление на докладите за самооценка" });
                policies.Add(new Policy() { PolicyCode = "ShowAnnualInfoList", PolicyDescription = "Списък на справките за годишен отчет" });
                policies.Add(new Policy() { PolicyCode = "ShowAllAnnualInfoList", PolicyDescription = "Списък на всички ЦПО справки за годишен отчет" });
            #endregion
                #region Рейтинг
                policies.Add(new Policy() { PolicyCode = "ShowRatingIndicatorList", PolicyDescription = "Настройка на индикатори за рейтинг ЦПО/ЦИПО - Списък на индикатори за рейтинг ЦПО/ЦИПО" });
                policies.Add(new Policy() { PolicyCode = "ManageRatingIndicatorList", PolicyDescription = "Настройка на индикатори за рейтинг ЦПО/ЦИПО - Управление на индикатори за рейтинг ЦПО/ЦИПО" });
                policies.Add(new Policy() { PolicyCode = "ManageRatingResultList", PolicyDescription = "Критерии за рейтинг и Индикатори ЦПО/ЦИПО - Управление на Критерии за рейтинг и Индикатори ЦПО/ЦИПО" });
            #endregion
                #region Доклади и отчети
                policies.Add(new Policy() { PolicyCode = "ManageReportNsiList", PolicyDescription = "Годишна информация НСИ - Управление на Годишна индормация НСИ" });
                #endregion
            #endregion
            #region Достъп ЦПО

            #region Профил на ЦПО

            #region Профил на ЦПО
            policies.Add(new Policy() { PolicyCode = "ManageCPOProfile", PolicyDescription = "Профил - Управление на Профил ЦПО" });
            policies.Add(new Policy() { PolicyCode = "ManageProfile", PolicyDescription = "Профил - Управление на Профил ЦПО/ЦИПО" });

            #endregion
            #region Заявление за лицензиране
            #endregion
            #region Изменение на лицензията
            #endregion
            #region Дубликат на лицензията
            #endregion
            #region Известия
            policies.Add(new Policy() { PolicyCode = "ShowNotificationList", PolicyDescription = "Общи - Списък с известия" });
                                policies.Add(new Policy() { PolicyCode = "ViewNotificationData", PolicyDescription = "Общи - Преглед на известия" });
                                policies.Add(new Policy() { PolicyCode = "ManageNotificationData", PolicyDescription = "Общи - Позволава създаване,редактиране и изтриване на известия" });
                            #endregion
                    #region Управление на достъпа
                        policies.Add(new Policy() { PolicyCode = "ShowProviderPersonList", PolicyDescription = "Достъп - Списък с потребители" });
                        policies.Add(new Policy() { PolicyCode = "ViewProviderPersonData", PolicyDescription = "Достъп - Преглед на потребител" });
                        policies.Add(new Policy() { PolicyCode = "ManageProviderPersonData", PolicyDescription = "Достъп - Позволава създаване,редактиране и изтриване на потребител" });
            #endregion
                    #region Електронна регистрация
            #endregion

            #endregion
                #region Заявки за документация 

                #region Налични документи в ЦПО
                        policies.Add(new Policy() { PolicyCode = "ShowNAPOOControlDocumentList", PolicyDescription = "Налични Документи - Списък с документи" });
                        policies.Add(new Policy() { PolicyCode = "ViewNAPOOControlDocumentData", PolicyDescription = "Налични Документи - Преглед на документ" });
                        policies.Add(new Policy() { PolicyCode = "ManageNAPOOControlDocumentData", PolicyDescription = "Налични Документи - Позволава създаване,редактиране и изтриване на документ" });
                #endregion
                #region Подаване на заявки за документация към НАПОО
                        policies.Add(new Policy() { PolicyCode = "ShowCPODocumentRequestList", PolicyDescription = "Заявки за Документи - Списък със заявки" });
                        policies.Add(new Policy() { PolicyCode = "ViewCPODocumentRequestData", PolicyDescription = "Заявки за Документи - Преглед на заявка" });
                        policies.Add(new Policy() { PolicyCode = "ManageCPODocumentRequestData", PolicyDescription = "Заявки за Документи - Позволава създаване,редактиране и изтриване на заявка" });
                #endregion
                #region Получаване на документи
                       // policies.Add(new Policy() { PolicyCode = "ShowRequestDocumentManagementList", PolicyDescription = "Налични Документи - Списък с документи" });
                       // policies.Add(new Policy() { PolicyCode = "ViewRequestDocumentManagementData", PolicyDescription = "Налични Документи - Преглед на документ" });
                       // policies.Add(new Policy() { PolicyCode = "ManageRequestDocumentManagementData", PolicyDescription = "Налични Документи - Позволава създаване,редактиране и изтриване на документ" });
                #endregion
                #region Предаване на документи на дадено ЦПО
                        policies.Add(new Policy() { PolicyCode = "ShowHandingOverDocumentList", PolicyDescription = "Предаване на Документи - Списък с предадени документи" });
                        policies.Add(new Policy() { PolicyCode = "ViewHandingOverDocumentData", PolicyDescription = "Предаване на Документи - Преглед на документ" });
                        policies.Add(new Policy() { PolicyCode = "ManageHandingOverDocumentData", PolicyDescription = "Предаване на Документи - Позволава създаване,редактиране и изтриване на документ" });
                #endregion
                #region Отчет на документите с фабрична номерация
                        policies.Add(new Policy() { PolicyCode = "ShowCPODocumentDestructionList", PolicyDescription = "Отчет на Документи - Списък с документи" });
                        policies.Add(new Policy() { PolicyCode = "ViewCPODocumentDestructionData", PolicyDescription = "Отчет на Документи - Преглед на документ" });
                        policies.Add(new Policy() { PolicyCode = "ManageCPODocumentDestructionData", PolicyDescription = "Отчет на Документи - Позволава създаване,редактиране и изтриване на документ" });
                #endregion
                #region Публикуване на документ на борса на документи
                        policies.Add(new Policy() { PolicyCode = "ShowDocumentOfferList", PolicyDescription = "Публикуване на Документи - Списък с документи" });
                        policies.Add(new Policy() { PolicyCode = "ViewDocumentOfferData", PolicyDescription = "Публикуване на Документи - Преглед на документ" });
                        policies.Add(new Policy() { PolicyCode = "ManageDocumentOfferData", PolicyDescription = "Публикуване на Документи - Позволава създаване,редактиране и изтриване на документ" });
            #endregion
            #region Борса на документи

            #endregion

            #endregion
            #region Годишна информация за ЦПО и ЦИПО
            policies.Add(new Policy() { PolicyCode = "ManageAnnualInfoCpoCipo", PolicyDescription = "Годишна информация - Годишна информация за ЦПО и ЦИПО" });
            #endregion
            #region Архив на курсове от стара ИС
            policies.Add(new Policy() { PolicyCode = "ShowArchiveTrainingCourseList", PolicyDescription = "Списък aрхив на курсове за обучение" });
            #endregion
            #region Курсове
            policies.Add(new Policy() { PolicyCode = "ShowTrainingCourseDuplicateList", PolicyDescription = "Списък на издадени дубликати на документи от курсове" });
            policies.Add(new Policy() { PolicyCode = "ManageTrainingCourseDuplicate", PolicyDescription = "Създаване на нов запис за издаден дубликат на документ от курсове" });
            #endregion
            #region Валидиране
            policies.Add(new Policy() { PolicyCode = "ShowTrainingValidationDuplicateList", PolicyDescription = "Списък на издадени дубликати на документи от процедури по валидиране" });
            policies.Add(new Policy() { PolicyCode = "ManageTrainingValidationDuplicate", PolicyDescription = "Създаване на нов запис за издаден дубликат на документ от процедури за валидиране" });
            #endregion
            #endregion
            #region Достъп ЦИПО
            #region Профил на ЦИПО
            #region Профил на ЦИПО
            policies.Add(new Policy() { PolicyCode = "ManageCIPOProfile", PolicyDescription = "Профил - Управление на Профил ЦИПО" });
            #endregion
            #endregion
            #region Консултиране
            policies.Add(new Policy() { PolicyCode = "ManageConsultingList", PolicyDescription = "Управление на информацията за консултирани лица" });
            #endregion
            #endregion
            #region Служебни Модули
            #region Роли
            policies.Add(new Policy() { PolicyCode = "ShowRolesList", PolicyDescription = "Роли - Списък на Ролите" });
            policies.Add(new Policy() { PolicyCode = "ViewRolesData", PolicyDescription = "Роли - Преглед на Ролите" });
            policies.Add(new Policy() { PolicyCode = "ManageRolesData", PolicyDescription = "Роли - Позволава създаване,редактиране и изтриване на Ролите" });
            #endregion
            #region Позволени действия
            policies.Add(new Policy() { PolicyCode = "ShowPolicyList", PolicyDescription = "Позволени действия - Списък на Позволените действия" });
            policies.Add(new Policy() { PolicyCode = "ViewPolicyData", PolicyDescription = "Позволени действия - Преглед на Позволените действия" });
            policies.Add(new Policy() { PolicyCode = "ManagePolicyData", PolicyDescription = "Позволени действия - Позволава създаване,редактиране и изтриване на Ролите" });
            #endregion
            #region Номенклатури
            policies.Add(new Policy() { PolicyCode = "ShowNomenclaturesList", PolicyDescription = "Номенклатури - Списък на Номенклатури" });
            policies.Add(new Policy() { PolicyCode = "ViewNomenclaturesData", PolicyDescription = "Номенклатури - Преглед на Номенклатури" });
            policies.Add(new Policy() { PolicyCode = "ManageNomenclaturesData", PolicyDescription = "Номенклатури - Позволава създаване,редактиране и изтриване на Номенклатури" });
            #endregion
            #region Настройки
            policies.Add(new Policy() { PolicyCode = "ShowSettingsList", PolicyDescription = "Настройки - Списък на Настройки" });
            policies.Add(new Policy() { PolicyCode = "ViewSettingsData", PolicyDescription = "Настройки - Преглед на Настройки" });
            policies.Add(new Policy() { PolicyCode = "ManageSettingsData", PolicyDescription = "Настройки - Позволава създаване,редактиране и изтриване на Настройки" });
            #endregion

            policies.Add(new Policy() { PolicyCode = "ShowPaymentList", PolicyDescription = "Плащания - Списък на Плащания" });
            policies.Add(new Policy() { PolicyCode = "ViewPaymentData", PolicyDescription = "Плащания - Преглед на Плащания" });
            policies.Add(new Policy() { PolicyCode = "ManagePaymentData", PolicyDescription = "Плащания - Позволава създаване,редактиране и изтриване на Плащания" });
           
            #region НКПД
            policies.Add(new Policy() { PolicyCode = "ShowNKPDList", PolicyDescription = "НКПД - Списък на НКПД" });
            policies.Add(new Policy() { PolicyCode = "ViewNKPDData", PolicyDescription = "НКПД - Преглед на НКПД" });
            policies.Add(new Policy() { PolicyCode = "ManageNKPDData", PolicyDescription = "НКПД - Позволава създаване,редактиране и изтриване на НКПД" });
            #endregion
            #region Нав. Меню
            policies.Add(new Policy() { PolicyCode = "ShowMenuNodesList", PolicyDescription = "Навигационно меню - Списък на Менюта" });
            policies.Add(new Policy() { PolicyCode = "ViewMenuNodesData", PolicyDescription = "Навигационно меню - Преглед на Меню" });
            policies.Add(new Policy() { PolicyCode = "ManageMenuNodesData", PolicyDescription = "Навигационно меню - Позволава създаване,редактиране и изтриване на Меню" });
            #endregion
            #region ЕКАТТЕ
            policies.Add(new Policy() { PolicyCode = "ShowEKATTEList", PolicyDescription = "ЕКАТТЕ - Списък на ЕКАТТЕ" });
            policies.Add(new Policy() { PolicyCode = "ViewEKATTEData", PolicyDescription = "ЕКАТТЕ - Преглед на ЕКАТТЕ" });
            policies.Add(new Policy() { PolicyCode = "ManageEKATTEData", PolicyDescription = "ЕКАТТЕ - Позволава създаване,редактиране и изтриване на ЕКАТТЕ" });
            #endregion
            #region Потребители
            policies.Add(new Policy() { PolicyCode = "ShowUserList", PolicyDescription = "Потребители - Списък на Потребители" });
            policies.Add(new Policy() { PolicyCode = "ViewUserData", PolicyDescription = "Потребители - Преглед на Потребители" });
            policies.Add(new Policy() { PolicyCode = "ManageUserData", PolicyDescription = "Потребители - Позволава създаване,редактиране и изтриване на ЕКАТТЕ" });
            #endregion
            #region Такси за лицезиране
            policies.Add(new Policy() { PolicyCode = "ShowProcedurePriceList", PolicyDescription = "Такси за лицезиране - Списък на Такси" });
            policies.Add(new Policy() { PolicyCode = "ViewProcedurePriceData", PolicyDescription = "Такси за лицезиране - Преглед на Такси" });
            policies.Add(new Policy() { PolicyCode = "ManageProcedurePriceData", PolicyDescription = "Такси за лицезиране - Позволава създаване,редактиране и изтриване на Такси" });
            #endregion
            #region Поддръжка документи
            policies.Add(new Policy() { PolicyCode = "ShowTemplateDocumentList", PolicyDescription = "Поддръжка документи - Списък на Документи" });
            policies.Add(new Policy() { PolicyCode = "ViewTemplateDocumentData", PolicyDescription = "Поддръжка документи - Преглед на Документи" });
            policies.Add(new Policy() { PolicyCode = "ManageTemplateDocumentData", PolicyDescription = "Поддръжка документи - Позволава създаване,редактиране и изтриване на Документи" });
            #endregion
            #region Документи по наредба 8
            policies.Add(new Policy() { PolicyCode = "ShowRegulationEightDocumentList", PolicyDescription = "Документи по Наредба 8 - покажи целия списък" });
            #endregion
            #region Анкети
            policies.Add(new Policy() { PolicyCode = "ShowSurveyList", PolicyDescription = "Създадени анкети - покажи целия списък" });
            #endregion

            #region Импорт на потребители
            policies.Add(new Policy() { PolicyCode = "ShowImportUsers", PolicyDescription = "Импорт на потребители - позволява достъп до функционалността за импорт на потребители в системата" });
            #endregion
            #endregion

            return policies;
        }

        Task<IEnumerable<PolicyVM>> GetAllPolicyAsync(PolicyVM policyVM);
        Task<IEnumerable<PolicyVM>> GetAllPolicyExceptAsync(List<RoleClaim> RoleClaims);
        Task<PolicyVM> getById(int idPolicy);
        Task MergePoliciesAsync();
        Task UpdatePolicy(PolicyVM policy);
    }
}
