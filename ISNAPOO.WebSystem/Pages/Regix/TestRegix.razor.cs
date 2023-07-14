using System.Text;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using RegiX;
using RegiX.Class.AVBulstat2.GetStateOfPlay;
using RegiX.Class.AVTR.GetActualState;
using RegiX.Class.AVTR.GetActualStateV2;
using RegiX.Class.AVTR.GetValidUICInfo;
using RegiX.Class.AVTR.PersonInCompaniesSearch;
using RegiX.Class.GraoNBD.ValidPersonSearch;
using RegiX.Class.Nacid.RdpzsdRegisterEntriesSearch;
using RegiX.Class.NapooStudentDocuments.GetDocumentsByStudent;
using RegiX.Class.NKPD.GetNKPDAllData;
using RegiX.Class.RDSO.GetDiplomaInfo;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Regix
{
    public partial class TestRegix : BlazorBaseComponent
    {
        [Inject]
        public IRegiXService RegiXService { get; set; }

        public List<TestRegixResult> GridData { get; set; }
        SfGrid<TestRegixResult> Grid;

       

        private async Task TestSing()
        {
            string EIK = "201593301";
            string EGN = "2111111111";

            GridData = new List<TestRegixResult>();
            TestRegixResult regixResult = new TestRegixResult();

            #region Справка по код на БУЛСТАТ или по фирмено дело за актуално състояние на субект на БУЛСТАТ

            //      REGIX – Агенция по вписванията
            //      Справка по код на БУЛСТАТ или по фирмено дело за актуално състояние на субект на БУЛСТАТ
            //      TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.GetStateOfPlay
            //      GetStateOfPlayRequest / StateOfPlay
            //      https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API/GetStateOfPlay


            try
            {
                var callContext = await this.GetCallContextAsync(this.BulstatCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Справка по код на БУЛСТАТ или по фирмено дело за актуално състояние на субект на БУЛСТАТ";
                regixResult.Status = "Неуспешно";


                StateOfPlay stateOfPlay = RegiXService.GetStateOfPlay(EIK, "", "", 0, callContext);

                if (stateOfPlay != null)
                {
                    regixResult.Status = "Успешно ЕИК:" + EIK;
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion

            #region Справка по физическо лице за участие в търговски дружества
            ///  REGIX – Агенция по вписванията
            ///  Справка по физическо лице за участие в търговски дружества
            ///  TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.PersonInCompaniesSearch
            ///  https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/PersonInCompaniesSearch
            try
            {
                var callContext = await this.GetCallContextAsync(this.BulstatCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Справка по физическо лице за участие в търговски дружества";
                regixResult.Status = "Неуспешно";


                SearchParticipationInCompaniesResponseType searchParticipationInCompaniesResponseType = RegiXService.PersonInCompaniesSearch(EGN, callContext);

                if (searchParticipationInCompaniesResponseType != null)
                {
                    regixResult.Status = "Успешно ЕГН:" + EGN;
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion

            #region Справка за актуално състояние(v1)
            /// REGIX – Агенция по вписванията
            /// Справка за актуално състояние(v1)
            /// TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualState
            /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/GetActualState
            try
            {
                var callContext = await this.GetCallContextAsync(this.BulstatCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Справка за актуално състояние(v1)";
                regixResult.Status = "Неуспешно";


                ActualStateResponseType actualStateResponseType = RegiXService.GetActualState(EIK, callContext);

                if (actualStateResponseType != null)
                {
                    regixResult.Status = "Успешно ЕИК:" + EIK;
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion

            #region Справка за Валидност на ЕИК номер
            /// <summary>
            /// REGIX – Агенция по вписванията
            /// Справка за Валидност на ЕИК номер
            /// TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetValidUICInfo
            try
            {
                var callContext = await this.GetCallContextAsync(this.BulstatValidityCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Справка за Валидност на ЕИК номер";
                regixResult.Status = "Неуспешно";


                ValidUICResponseType validUICResponseType = RegiXService.GetValidUICInfo(EIK, callContext);

                if (validUICResponseType != null)
                {
                    regixResult.Status = "Успешно ЕИК:" + EIK;
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion

            #region Справка за актуално състояние за всички вписани обстоятелства(v2)
            /// REGIX – Агенция по вписванията
            /// Справка за актуално състояние за всички вписани обстоятелства(v2)
            /// TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualStateV2
            /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/GetActualStateV2

            try
            {
                var callContext = await this.GetCallContextAsync(this.BulstatValidityCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Справка за актуално състояние за всички вписани обстоятелства(v2)";
                regixResult.Status = "Неуспешно";


                ActualStateResponseV2 actualStateResponseV2 = RegiXService.GetActualStateV2(EIK, callContext);

                if (actualStateResponseV2 != null)
                {
                    regixResult.Status = "Успешно ЕИК:" + EIK;
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion

            #region Справка за издадените документи на лице по подаден идентификатор
            /// REGIX - НАПОО
            /// Справка за издадените документи на лице по подаден идентификатор
            /// TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI.GetDocumentsByStudent
            /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI/GetDocumentsByStudent

            try
            {
                var callContext = await this.GetCallContextAsync(this.BulstatValidityCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Справка за издадените документи на лице по подаден идентификатор";
                regixResult.Status = "Неуспешно";


                DocumentsByStudentResponse documentsByStudentResponse = RegiXService.GetDocumentsByStudent(EGN, callContext);

                if (documentsByStudentResponse != null)
                {
                    regixResult.Status = "Успешно ЕГН:" + EGN;
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion

            #region Справка за издаден документ на лице по подаден идентификатор и идентификационен (или регистрационен) номер
            /// REGIX - НАПОО
            /// Справка за издаден документ на лице по подаден идентификатор и идентификационен (или регистрационен) номер
            /// TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI.GetStudentDocument
            /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI/GetStudentDocument
            try
            {
                var callContext = await this.GetCallContextAsync(this.BulstatValidityCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Справка за издаден документ на лице по подаден идентификатор и идентификационен (или регистрационен) номер";
                regixResult.Status = "Неуспешно";



                var documentsByStudentResponse = RegiXService.GetStudentDocument(EGN, "123456", callContext);

                if (documentsByStudentResponse != null)
                {
                    regixResult.Status = "Успешно ЕГН:" + EGN + " Документ:" + "123456";
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion

            #region Регистърът на дипломи и свидетелства за завършено основно и средно образование и придобита степен на професионална квалификация
            /// REGIX - МОН (RDSO) 
            /// Регистърът на дипломи и свидетелства за завършено основно и средно образование и придобита степен на професионална квалификация
            /// Справка за диплома за средно образование на определено лице
            /// TechnoLogica.RegiX.RDSOAdapter.APIService.IRDSOAPI.GetDiplomaInfo
            /// https://info-regix.egov.bg/public/administrations/MON/registries/operations/TechnoLogica.RegiX.RDSOAdapter.APIService.IRDSOAPI/GetDiplomaInfo
            try
            {
                var callContext = await this.GetCallContextAsync(this.SecondarySchoolDiplomaCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Регистърът на дипломи и свидетелства за завършено основно и средно образование и придобита степен на професионална квалификация";
                regixResult.Status = "Неуспешно";

                RegiX.Class.RDSO.GetDiplomaInfo.Request.IdentifierType identifier = RegiX.Class.RDSO.GetDiplomaInfo.Request.IdentifierType.EGN;


                DiplomaDocumentsType diplomaDocumentsType = RegiXService.DiplomaDocumentsType(EGN, identifier, "46546", callContext);

                if (diplomaDocumentsType != null)
                {
                    regixResult.Status = "Успешно ЕГН:" + EGN + " Документ:" + "46546";
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion

            #region Справка за търсене на целия класификатор НКПД
            /// REGIX – МТСП
            /// ИС на НАПОО ще получава от REGIX данни за Национална класификация на професиите и длъжностите
            /// Справка за търсене на целия класификатор НКПД
            /// TechnoLogica.RegiX.NKPDAdapter.APIService.INKPDAPI.GetNKPDAllData
            /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.NKPDAdapter.APIService.INKPDAPI/GetNKPDAllData

            try
            {
                var callContext = await this.GetCallContextAsync(this.NKPDCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Справка за търсене на целия класификатор НКПД";
                regixResult.Status = "Неуспешно";


                AllNKPDDataType allNKPDDataType = RegiXService.GetNKPDAllData(DateTime.Now, callContext);

                if (allNKPDDataType != null)
                {
                    regixResult.Status = "Успешно";
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion


            #region Справка за валидност на физическо лице (Регистър на населението – Национална база данни „Население“).
            /// REGIX – МРРБ
            /// Справка за валидност на физическо лице
            /// Справка за валидност на физическо лице (Регистър на населението – Национална база данни „Население“).
            /// TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.ValidPersonSearch
            /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI/ValidPersonSearch

            try
            {
                var callContext = await this.GetCallContextAsync(this.CurrentStatusCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Справка за валидност на физическо лице (Регистър на населението – Национална база данни „Население“).";
                regixResult.Status = "Неуспешно";


                ValidPersonResponseType validPersonResponseType = RegiXService.ValidPersonResponseType(EGN, callContext);

                if (validPersonResponseType != null)
                {
                    regixResult.Status = "Успешно ЕГН:" + EGN;
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            }
            #endregion



            #region Регистър на всички действащи, прекъснали и завършили студенти и докторанти, по степени на обучение и по професионални направления
            /// REGIX – NCID
            /// Регистър на всички действащи, прекъснали и завършили студенти и докторанти, по степени на обучение и по професионални направления
            /// Изпълнява услуга по извличане на Справка за статистически и персонални данни и основание за приемане на студенти във висшите училища, техните основни звена и филиали
            /// Nacid.RegiX.RdpzsdAdapter.APIService.IRdpzsdAPI.RdpzsdRegisterEntriesSearch

            try
            {
                string Uan = "VK11454";
                string Uin = "6305156261";
                string IdNumber = "1234567";                
                string FullName = "Йордан Любомиров Чорбаджийски";
                var callContext = await this.GetCallContextAsync(this.CurrentStatusCheckKV);

                regixResult = new TestRegixResult();
                regixResult.Register = "Регистър на всички действащи, прекъснали и завършили студенти и докторанти, по степени на обучение и по професионални направления";
                regixResult.Status = "Неуспешно";


                RdpzsdRegisterEntriesResponseType rdpzsdRegisterEntriesResponseType = RegiXService.RdpzsdRegisterEntriesSearch(Uan, Uin, IdNumber, FullName, callContext);

                if (rdpzsdRegisterEntriesResponseType != null)
                {
                    regixResult.Status = "Успешно ЕАН:" + Uan;
                }

                GridData.Add(regixResult);
                Grid.Refresh();

            }
            catch (Exception ex)
            {
                regixResult.Status = ex.Message;
                GridData.Add(regixResult);
            } 
            #endregion




            Grid.Refresh();

            this.StateHasChanged();

        }
    }

    public class TestRegixResult 
    {
        public string Register { get; set; }
        public string Status { get; set; }
    }
}
