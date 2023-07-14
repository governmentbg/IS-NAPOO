using Data.Models.Common;
using Data.Models.Data.EGovPayment;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.Common.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using ISNAPOO.Core.Services.Common;
using static SkiaSharp.HarfBuzz.SKShaper;
using System.Reflection.Metadata;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Components;
using System.ServiceModel.Channels;
using Data.Models.Data.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using Data.Models.Data.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using Data.Models.Data.ProviderData;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.SPPOO;
using Data.Models.Data.SPPOO;
using Data.Models.Data.Archive;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;

namespace ISNAPOO.Core.Services.EGovPayment
{
    public class PaymentService : BaseService, IPaymentService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IApplicationUserService applicationUserService;

        public PaymentService(IRepository repository,
            IDataSourceService dataSourceService,
            ILogger<PaymentService> logger,
            IApplicationUserService applicationUserService,
            AuthenticationStateProvider authenticationStateProvider)
            : base(repository)
        {
            this.repository = repository;
            this.applicationUserService = applicationUserService;
            this.dataSourceService = dataSourceService;
            this._logger = logger;
            this.authenticationStateProvider = authenticationStateProvider;

            _logger.LogDebug("Стартиране на PaymentService.......");
        }
        public async Task<PaymentVM> GetPaymentAsync(int idPayment)
        {

            Payment data = await this.repository.GetByIdAsync<Payment>(idPayment);           
            var result = data.To<PaymentVM>();

            return result;
        }
        public async Task<IEnumerable<PaymentVM>> GetAllPaymentsAsync(int idCandidateProvider)
        {
            //var data = this.repository.AllReadonly<Payment>(x => x.IdCandidate_Provider == idCandidateProvider);
            
            var data = this.repository.AllReadonly<Payment>().Include(x=>x.CandidateProvider);
            var dataAsVM = await data.To<PaymentVM>(x => x.CandidateProvider).ToListAsync();

            //var indentTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            //foreach (var client in dataAsVM)
            //{
            //    var indent = indentTypeSource.FirstOrDefault(x => x.IdKeyValue == client.IdIndentType);
            //    if (indent is not null)
            //    {
            //        client.IndentType = indent.Name;
            //    }
            //}

            return dataAsVM;
        }
        public async Task<IEnumerable<PaymentVM>> GetPaymentsByCandidateProviderIdAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<Payment>(x => x.IdCandidate_Provider == idCandidateProvider).OrderByDescending(x => x.CreationDate);
            var dataAsVM = await data.To<PaymentVM>().ToListAsync();

            return dataAsVM;
        }
        public async Task<IEnumerable<PaymentVM>> GetAllPaymentsByApplicantUinAsync(string applicantUin)
        {
           
            var data = this.repository.AllReadonly<Payment>(x => x.ApplicantUin == applicantUin);
            var dataAsVM = await data.To<PaymentVM>().ToListAsync();
      
            return dataAsVM;
        }
        public async Task<IEnumerable<ProcedurePriceVM>> GetAllProcedurePrices()
        {

            var data = this.repository.AllReadonly<ProcedurePrice>();
            var dataAsVM = await data.To<ProcedurePriceVM>().ToListAsync();

            return dataAsVM;
        }


        public async Task<ResultContext<PaymentVM>> CreatePaymentAsync(ResultContext<PaymentVM> resultContext)
        {
            try
            {
                //za test
             //   var prepNewPayment = await this.PrepareNewPayment(resultContext.ResultContextObject.IdCandidate_Provider, "StartProcedureCIPO");//"FirstLicenzingCPO");
              //  resultContext = prepNewPayment;
                //za test

                resultContext.ResultContextObject.IdPayment = 0;

                var serviceProviderNamePayEGov = await dataSourceService.GetSettingByIntCodeAsync("ServiceProviderNamePayEGov");
                resultContext.ResultContextObject.ServiceProviderName = serviceProviderNamePayEGov.SettingValue;

                var serviceProviderBankPayEGov = await dataSourceService.GetSettingByIntCodeAsync("ServiceProviderBankPayEGov");          
                resultContext.ResultContextObject.ServiceProviderBank = serviceProviderBankPayEGov.SettingValue;

                var serviceProviderBICPayEGov = await dataSourceService.GetSettingByIntCodeAsync("ServiceProviderBICPayEGov");
                resultContext.ResultContextObject.ServiceProviderBIC = serviceProviderBICPayEGov.SettingValue;

                var serviceProviderIBANPayEGov = await dataSourceService.GetSettingByIntCodeAsync("ServiceProviderIBANPayEGov");
                resultContext.ResultContextObject.ServiceProviderIBAN = serviceProviderIBANPayEGov.SettingValue;

                var paymentTypeCodeEGov = await dataSourceService.GetSettingByIntCodeAsync("PaymentTypeCodeEGov");
                resultContext.ResultContextObject.PaymentTypeCode = paymentTypeCodeEGov.SettingValue;

                var currencyEGov = await dataSourceService.GetSettingByIntCodeAsync("CurrencyPayEGov");
                resultContext.ResultContextObject.Currency = currencyEGov.SettingValue;         
                resultContext.ResultContextObject.ReferenceType = (await dataSourceService.GetSettingByIntCodeAsync("PaymentReferenceTypeEGov")).SettingValue;
                resultContext.ResultContextObject.AdministrativeServiceUri = (await dataSourceService.GetSettingByIntCodeAsync("AdministrativeServiceUriEGov")).SettingValue;
                resultContext.ResultContextObject.AdministrativeServiceSupplierUri = (await dataSourceService.GetSettingByIntCodeAsync("AdministrativeServiceSupplierUriEGov")).SettingValue;
                resultContext.ResultContextObject.AdministrativeServiceNotificationURL = (await dataSourceService.GetSettingByIntCodeAsync("AdministrativeServiceNotificationURLEGov")).SettingValue;

                long refNum = 5000;
                refNum += await this.GetSequenceNextValue("paymentEGov");
                resultContext.ResultContextObject.ReferenceNumber = refNum.ToString();
                resultContext.ResultContextObject.ReferenceDate = DateTime.Now;
                var entity = resultContext.ResultContextObject.To<Payment>();
                entity.CandidateProvider = null;

                await this.repository.AddAsync(entity);
                var result = await this.repository.SaveChangesAsync();
                resultContext.ResultContextObject.IdPayment = entity.IdPayment;

                resultContext.AddMessage("Успешно добавено плащане");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }

        public async Task<ResultContext<PaymentVM>> UpdatePaymentAsync(ResultContext<PaymentVM> resultContext)
        {
            try
            {
  
                var entity = resultContext.ResultContextObject.To<Payment>();
                entity.CandidateProvider = null;
                this.repository.Update<Payment>(entity);
                var result = await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Успешна промяна");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }

        /// <summary>
        /// Създаване на заявка за плащане
        /// </summary>
        /// <param name="resultContext"></param>
        /// <returns></returns>
        public async Task<ResultContext<PaymentVM>> CreatePaymentToPayEGov(ResultContext<PaymentVM> resultContext)
        {
            ResultContext<Unacceptedreceiptjson> unacceptedreceiptjson = new ResultContext<Unacceptedreceiptjson>();
            ResultContext<Acceptedreceiptjson> acceptedreceiptjson = new ResultContext<Acceptedreceiptjson>();
           
            try
            {

                string baseURL = (await dataSourceService.GetSettingByIntCodeAsync("BaseURLPayEGov")).SettingValue;
                string url = baseURL + "paymentJson";

                string ClientId = (await dataSourceService.GetSettingByIntCodeAsync("ClientIdPayEGov")).SettingValue;

                string SecretKey = (await dataSourceService.GetSettingByIntCodeAsync("SecretKeyPayEGov")).SettingValue;
                
                resultContext = await CreatePaymentAsync(resultContext);
      
                string body = Convert.ToBase64String(
                   System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                   {                       
                       aisPaymentId = resultContext.ResultContextObject.IdPayment,
                       serviceProviderName = resultContext.ResultContextObject.ServiceProviderName,
                       serviceProviderBank = resultContext.ResultContextObject.ServiceProviderBank,
                       serviceProviderBIC = resultContext.ResultContextObject.ServiceProviderBIC,
                       serviceProviderIBAN = resultContext.ResultContextObject.ServiceProviderIBAN,
                       currency = resultContext.ResultContextObject.Currency,
                       paymentTypeCode = resultContext.ResultContextObject.PaymentTypeCode,
                       paymentAmount = resultContext.ResultContextObject.PaymentAmount,//.Value.ToString("#.00", System.Globalization.CultureInfo.InvariantCulture),
                       paymentReason = resultContext.ResultContextObject.PaymentReason,
                       applicantUinTypeId = resultContext.ResultContextObject.ApplicantUinIntDefVal,
                       applicantUin = resultContext.ResultContextObject.ApplicantUin,
                       applicantName = resultContext.ResultContextObject.ApplicantName,
                       paymentReferenceType = resultContext.ResultContextObject.ReferenceType,
                       paymentReferenceNumber = resultContext.ResultContextObject.ReferenceNumber,                  
                       paymentReferenceDate = resultContext.ResultContextObject.ReferenceDate.Value.ToString("s"),
                       expirationDate = resultContext.ResultContextObject.ExpirationDate.Value.ToString("s"),
                       additionalInformation = resultContext.ResultContextObject.AdditionalInformation,
                       administrativeServiceUri = resultContext.ResultContextObject.AdministrativeServiceUri,
                       administrativeServiceSupplierUri = resultContext.ResultContextObject.AdministrativeServiceSupplierUri,
                       administrativeServiceNotificationURL = resultContext.ResultContextObject.AdministrativeServiceNotificationURL
                   }))) ;
               
            
                string signature = BaseHelper.CalculateHmac(SecretKey, body);

                _logger.LogInformation($"ClientId:{ClientId}, Hmac:{signature}, Data:{body}");
              
                Dictionary<string, string> keyValues = new Dictionary<string, string>()
                {
                    { "clientId", ClientId},
                    { "hmac", signature },
                    { "data", body }
                };

                string content = null;

                using (HttpClient client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                  
                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(keyValues));

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            _logger.LogError("PayEGov StatusCode:" + response.StatusCode);
                             
                        }

                        content = await response.Content.ReadAsStringAsync();
                     
                        var receiptResult = JsonConvert.DeserializeObject<ReceiptResult>(content);

                        if (receiptResult.acceptedReceiptJson != null)
                        {
                            acceptedreceiptjson = new ResultContext<Acceptedreceiptjson>();
                            acceptedreceiptjson.ResultContextObject = receiptResult.acceptedReceiptJson;
                            resultContext = await this.UpdatePaymentAcceptedReceiptAsync(acceptedreceiptjson, resultContext);
                        }
                        else
                        {
                            resultContext.AddErrorMessage("Неуспешно създаване на заявка за плащане");
                            string strErrors = string.Join(" ", receiptResult.unacceptedReceiptJson.errors);
                            _logger.LogError("Create payment error: message: " + receiptResult.unacceptedReceiptJson.message + ", validationTime: " + receiptResult.unacceptedReceiptJson.validationTime + ", errors: " + strErrors);
                        }                                        
                    }
                    catch (Exception exp)
                    {
                        /*                     
                         */
                        throw exp;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }

        /// <summary>
        /// Запитване за заявки за плащане по идентификатор (JSON формат)
        /// </summary>
        /// <param name="recieptsIds"></param>
        /// <returns></returns>
        public async Task<PaymentById> GetPaymentsByIdJson(List<string> recieptsIds)
        {
            ResultContext<PaymentVM> resultContext = new ResultContext<PaymentVM>();
            PaymentById paymentById = new PaymentById();

            try
            {
                string baseURL = (await dataSourceService.GetSettingByIntCodeAsync("BaseURLPayEGov")).SettingValue;
                string url = baseURL + "paymentsByIdJson";

                string ClientId = (await dataSourceService.GetSettingByIntCodeAsync("ClientIdPayEGov")).SettingValue;

                string SecretKey = (await dataSourceService.GetSettingByIntCodeAsync("SecretKeyPayEGov")).SettingValue;


                //  resultContext = await CreatePaymentAsync(resultContext);


                string body = Convert.ToBase64String(
                   System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                   {
                       requestIds = recieptsIds //new List<string> { "23022781" }
                   })));


                string signature = BaseHelper.CalculateHmac(SecretKey, body);

                _logger.LogInformation($"ClientId:{ClientId}, Hmac:{signature}, Data:{body}");

                Dictionary<string, string> keyValues = new Dictionary<string, string>()
                {
                    { "clientId", ClientId},
                    { "hmac", signature },
                    { "data", body }
                };

                string content = null;

                using (HttpClient client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(keyValues));

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            _logger.LogError("PayEGov StatusCode:" + response.StatusCode);

                        }

                        content = await response.Content.ReadAsStringAsync();
                        paymentById = JsonConvert.DeserializeObject<PaymentById>(content);

                        /*
                        */
                    }
                    catch (Exception exp)
                    {

                        throw exp;
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return paymentById;
        }
        /// <summary>
        /// Запитване за заявки за плащане по идентификатор (JSON формат)
        /// </summary>
        /// <param name="applicantUinEik"></param>
        /// <returns></returns>
        public async Task<ResultContext<PaymentVM>> GetPaymentsByEikWithoutPendingJson(string applicantUinEik)
        {
            ResultContext<PaymentVM> resultContext = new ResultContext<PaymentVM>();
            try
            {
                string baseURL = (await dataSourceService.GetSettingByIntCodeAsync("BaseURLPayEGov")).SettingValue;
                string url = baseURL + "paymentStatusJson"; //"pendingPaymentStatusJson";

                string ClientId = (await dataSourceService.GetSettingByIntCodeAsync("ClientIdPayEGov")).SettingValue;

                string SecretKey = (await dataSourceService.GetSettingByIntCodeAsync("SecretKeyPayEGov")).SettingValue;


                //  resultContext = await CreatePaymentAsync(resultContext);


                string body = Convert.ToBase64String(
                   System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                   {
                       applicantUin = applicantUinEik //new List<string> { "7234438093602" , "7802041567", "8909187646" }
                   })));


                string signature = BaseHelper.CalculateHmac(SecretKey, body);

                _logger.LogInformation($"ClientId:{ClientId}, Hmac:{signature}, Data:{body}");

                Dictionary<string, string> keyValues = new Dictionary<string, string>()
                {
                    { "clientId", ClientId},
                    { "hmac", signature },
                    { "data", body }
                };

                string content = null;

                using (HttpClient client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(keyValues));

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            _logger.LogError("PayEGov StatusCode:" + response.StatusCode);

                        }

                        content = await response.Content.ReadAsStringAsync();

                    }
                    catch (Exception exp)
                    {

                        throw exp;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }

        /// <summary>
        /// Запиване за код за достъп до заявка за плащане
        /// </summary>
        /// <param name="recieptId"></param>
        /// <returns></returns>
        public async Task<ResultContext<PaymentVM>> GetAccessCode(string recieptId)
        {
            var resultContext = new ResultContext<PaymentVM>(); 
            try
            {
                string baseURL = (await dataSourceService.GetSettingByIntCodeAsync("BaseURLPayEGov")).SettingValue;
                string url = baseURL + "accessCode";

                string ClientId = (await dataSourceService.GetSettingByIntCodeAsync("ClientIdPayEGov")).SettingValue;

                string SecretKey = (await dataSourceService.GetSettingByIntCodeAsync("SecretKeyPayEGov")).SettingValue;


                //  resultContext = await CreatePaymentAsync(resultContext);


                string body = Convert.ToBase64String(
                   System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                   {
                       id = recieptId
                   })));

                string signature = BaseHelper.CalculateHmac(SecretKey, body);

                _logger.LogInformation($"ClientId:{ClientId}, Hmac:{signature}, Data:{body}");

                Dictionary<string, string> keyValues = new Dictionary<string, string>()
                {
                    { "clientId", ClientId},
                    { "hmac", signature },
                    { "data", body }
                };

                string content = null;

                using (HttpClient client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(keyValues));

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            _logger.LogError("PayEGov StatusCode:" + response.StatusCode);

                        }

                        content = await response.Content.ReadAsStringAsync();

                        var resAccessCode = JsonConvert.DeserializeObject<PaymentVM>(content);

                        resultContext.ResultContextObject = resAccessCode;

                        if (resAccessCode.AccessCode == null)
                        {
                            resultContext.AddErrorMessage("Системата не може да изкара код за достъп");
                            
                            _logger.LogError("Аccess code is null");
                        }            
                    }
                    catch (Exception exp)
                    {

                        throw exp;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }


        /// <summary>
        /// Запитване за статус на заявки за плащане по идентификатор
        /// </summary>
        /// <param name="resultContext"></param>
        /// <returns></returns>
        public async Task<ResultContext<PaymentStatus>> GetPaymentsStatus(ResultContext<PaymentVM> resultContext)
        {

            ResultContext<PaymentStatus> paymentStatusResult = new ResultContext<PaymentStatus>();

            try
            {
                string baseURL = (await dataSourceService.GetSettingByIntCodeAsync("BaseURLPayEGov")).SettingValue;
                string url = baseURL + "paymentsStatus";

                string ClientId = (await dataSourceService.GetSettingByIntCodeAsync("ClientIdPayEGov")).SettingValue;

                string SecretKey = (await dataSourceService.GetSettingByIntCodeAsync("SecretKeyPayEGov")).SettingValue;


                //  resultContext = await CreatePaymentAsync(resultContext);


                string body = Convert.ToBase64String(
                   System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                   {
                       requestIds = new List<string> { resultContext.ResultContextObject.ReceiptId }
                   })));

                string signature = BaseHelper.CalculateHmac(SecretKey, body);

                _logger.LogInformation($"ClientId:{ClientId}, Hmac:{signature}, Data:{body}");

                Dictionary<string, string> keyValues = new Dictionary<string, string>()
                {
                    { "clientId", ClientId},
                    { "hmac", signature },
                    { "data", body }
                };

                string content = null;

                using (HttpClient client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(keyValues));

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            _logger.LogError("PayEGov StatusCode:" + response.StatusCode);

                        }

                        content = await response.Content.ReadAsStringAsync();

                        var paymentStatus = JsonConvert.DeserializeObject<PaymentStatusResult>(content);

                        paymentStatusResult = new ResultContext<PaymentStatus>() { };

                        paymentStatusResult.ResultContextObject = paymentStatus.paymentStatuses.FirstOrDefault();

                        await this.UpdatePaymentStatusAsync(paymentStatusResult);

                        /*              
                        */
                    }
                    catch (Exception exp)
                    {

                        throw exp;
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                paymentStatusResult.AddErrorMessage(ex.Message);
            }
            return paymentStatusResult;
        }

        /// <summary>
        /// Отказване на заявка за плащане
        /// </summary>
        /// <param name="recieptId"></param>
        /// <returns></returns>
        public async Task<ResultContext<PaymentVM>> SuspendPaymentRequest(string recieptId)
        {
            var resultContext = new ResultContext<PaymentVM>();
            try
            {
                string baseURL = (await dataSourceService.GetSettingByIntCodeAsync("BaseURLPayEGov")).SettingValue;
                string url = baseURL + "suspendRequest";

                string ClientId = (await dataSourceService.GetSettingByIntCodeAsync("ClientIdPayEGov")).SettingValue;

                string SecretKey = (await dataSourceService.GetSettingByIntCodeAsync("SecretKeyPayEGov")).SettingValue;


                //  resultContext = await CreatePaymentAsync(resultContext);


                string body = Convert.ToBase64String(
                   System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                   {
                       id = recieptId
                   })));

                string signature = BaseHelper.CalculateHmac(SecretKey, body);

                _logger.LogInformation($"ClientId:{ClientId}, Hmac:{signature}, Data:{body}");

                Dictionary<string, string> keyValues = new Dictionary<string, string>()
                {
                    { "clientId", ClientId},
                    { "hmac", signature },
                    { "data", body }
                };

                string content = null;

                using (HttpClient client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(keyValues));

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            _logger.LogError("PayEGov StatusCode:" + response.StatusCode);

                        }

                        content = await response.Content.ReadAsStringAsync();
                        
                        var resultSuspendPayment = JsonConvert.DeserializeObject<PaymentVM>(content);
                        resultContext.ResultContextObject.message = resultSuspendPayment.message;

                        if (resultSuspendPayment.message == null)
                        {
                           resultContext = await this.UpdateSuspendPaymentStatusAsync(recieptId);
                        }
                        else
                        {
                            resultContext.AddErrorMessage("Заявката за плащане не може да се прекрати, не е на статус 'Очаква плащане'");
                            _logger.LogError($"The mеssage for result suspend payment is:{resultSuspendPayment.message}.");
                        }

                        /*
                         {
                              "message": "Unable to suspend payment request with status different from 'Pending'."
                          }
                        */
                    }
                    catch (Exception exp)
                    {

                        throw exp;
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
         }


        /// <summary>
        /// Отбелязване на заявка за плащане като платена
        /// </summary>
        /// <param name="resultContext"></param>
        /// <returns></returns>
        public async Task<ResultContext<PaymentVM>> SetStatusPaymentRequest(ResultContext<PaymentVM> resultContext)
        {
            try
            {
                string baseURL = (await dataSourceService.GetSettingByIntCodeAsync("BaseURLPayEGov")).SettingValue;
                string url = baseURL + "setStatusPaid";

                string ClientId = (await dataSourceService.GetSettingByIntCodeAsync("ClientIdPayEGov")).SettingValue;

                string SecretKey = (await dataSourceService.GetSettingByIntCodeAsync("SecretKeyPayEGov")).SettingValue;


                string body = Convert.ToBase64String(
                   System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                   {
                       id = resultContext.ResultContextObject.ReceiptId,
                       paymentMethod = resultContext.ResultContextObject.PaymentStatusDefVal, // 1 (Друг), 2 (На каса)
                       paymentDescription = resultContext.ResultContextObject.AdditionalInformation
                   })));


                string signature = BaseHelper.CalculateHmac(SecretKey, body);

                _logger.LogInformation($"ClientId:{ClientId}, Hmac:{signature}, Data:{body}");

                Dictionary<string, string> keyValues = new Dictionary<string, string>()
                {
                    { "clientId", ClientId},
                    { "hmac", signature },
                    { "data", body }
                };

                string content = null;

                using (HttpClient client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(keyValues));

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            _logger.LogError("PayEGov StatusCode:" + response.StatusCode);

                        }

                        content = await response.Content.ReadAsStringAsync();

                        var resultStatusPayment = JsonConvert.DeserializeObject<PaymentVM>(content);
                        resultContext.ResultContextObject.message = resultStatusPayment.message;

                        if (resultStatusPayment.message == null)
                        {
                            resultContext = await UpdatePaymentAsync(resultContext);
                        }
                        else
                        {
                            resultContext.AddErrorMessage("Статуса на заявката за плащане не може да се промени, не е на статус 'Очаква плащане'");
                            _logger.LogError($"The mеssage for result suspend payment is:{resultStatusPayment.message}.");
                        }
                       
                        /*
                         {
                              "message": "Unable to suspend payment request with status different from 'Pending'."
                          }
                        */

                    }
                    catch (Exception exp)
                    {

                        throw exp;
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }
        public async Task<ResultContext<PaymentVM>> UpdateSuspendPaymentStatusAsync(string recieptId)
        {
            ResultContext<PaymentVM> result = new ResultContext<PaymentVM>();

            try
            {
                int kvPaymentStatus = this.dataSourceService.GetKeyValueByIntCodeAsync("PaymentStatusPayEGov", "suspended").Result.IdKeyValue;
                var payment = this.repository.AllReadonly<Payment>().FirstOrDefault(
                    x => x.ReceiptId == recieptId);                              
                payment.IdPaymentStatus = kvPaymentStatus;
          
                this.repository.Update<Payment>(payment);
                await this.repository.SaveChangesAsync();

                result.ResultContextObject = payment.To<PaymentVM>();

                result.AddMessage("Успешна промяна");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage(ex.Message);
            }
            return result;
        }
        public async Task<ResultContext<PaymentVM>> UpdatePaymentStatusAsync(ResultContext<PaymentStatus> resultContext)
        {
            ResultContext<PaymentVM> result = new ResultContext<PaymentVM>();

            try
            {
               
                var payment = this.repository.AllReadonly<Payment>().FirstOrDefault(
                    x=>x.ReceiptId == resultContext.ResultContextObject.Id);
                //var nesto = resultContext.ResultContextObject.Status.ToLower();
                var kvPaymentStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("PaymentStatusPayEGov", resultContext.ResultContextObject.Status.ToLower());
                payment.IdPaymentStatus = kvPaymentStatus.IdKeyValue;
                payment.ChangeTime = resultContext.ResultContextObject.ChangeTime;

                if (resultContext.UserId.HasValue)
                {
                    payment.IdModifyUser = resultContext.UserId.Value;
                }
                else
                {
                    var user = await this.dataSourceService.GetSettingByIntCodeAsync("UserIDBindWithSystem");
                    var appUser = await applicationUserService.GetApplicationUsersById(user.SettingValue);
                    payment.IdModifyUser = appUser.IdUser;

                }

                this.repository.Update<Payment>(payment);                
                await this.repository.SaveChangesAsync();

                result.ResultContextObject = payment.To<PaymentVM>();

                resultContext.AddMessage("Успешна промяна");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return result;
        }
        public async Task<ResultContext<PaymentVM>> UpdatePaymentAcceptedReceiptAsync(ResultContext<Acceptedreceiptjson> acceptedreceiptjson, ResultContext<PaymentVM> resultContext)
        {
           // ResultContext<PaymentVM> result = new ResultContext<PaymentVM>();

            try
            {
                var payment = this.repository.AllReadonly<Payment>().FirstOrDefault(
                    x => x.IdPayment == resultContext.ResultContextObject.IdPayment);
          
                payment.ReceiptId = acceptedreceiptjson.ResultContextObject.id;
                payment.RegistrationTime = acceptedreceiptjson.ResultContextObject.registrationTime;

                this.repository.Update<Payment>(payment);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject = payment.To<PaymentVM>();

              //  resultContext.AddMessage("Успешна промяна");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }

        public async Task<ResultContext<PaymentVM>> PrepareNewPayment(int idCandidateProvider, string licensingFeeValue) 
        {

            ResultContext<PaymentVM> payment = new ResultContext<PaymentVM>();
            payment.ResultContextObject.IdCandidate_Provider = idCandidateProvider;
            CandidateProviderVM candidateProviderVm = new CandidateProviderVM();
            ProcedurePrice procedure = new ProcedurePrice();                      
            List<int> idProfessions = new List<int>();
            KeyValueVM appUinType, paymentStatus;

            try
            {

                var candidateProvider = await this.repository.GetByIdAsync<CandidateProvider>(idCandidateProvider);
                payment.ResultContextObject.ApplicantName = candidateProvider.ProviderOwner;
                payment.ResultContextObject.ApplicantUin = candidateProvider.PoviderBulstat;
                appUinType = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicantUinTypePayEGov", "BULSTAT");
                payment.ResultContextObject.ApplicantUinTypeId = appUinType.IdKeyValue;
                payment.ResultContextObject.ApplicantUinIntDefVal = appUinType.DefaultValue1;
                payment.ResultContextObject.IdPaymentStatus = this.dataSourceService.GetKeyValueByIntCodeAsync("PaymentStatusPayEGov", "pending").Result.IdKeyValue;
                var kvLicensingFee = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingFee", licensingFeeValue);
                var expDay = kvLicensingFee.DefaultValue1;
                var newDate = DateTime.Now;
                payment.ResultContextObject.ExpirationDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 23, 59, 59).AddDays(int.Parse(expDay));


                if (licensingFeeValue == "StartProcedureCPO")
                {
                    var providerSpecialities = this.repository.AllReadonly<CandidateProviderSpeciality>
                        (x => x.IdCandidate_Provider == idCandidateProvider)
                        .Include(x => x.Speciality)
                        .ThenInclude(x => x.Profession).Distinct();

                    foreach (var providerSpeciality in providerSpecialities)
                    {
                        int idprof = providerSpeciality.Speciality.Profession.IdProfession;

                        if (!idProfessions.Contains(idprof))
                        {
                            idProfessions.Add(idprof);
                        }
                    }
                    int profCount = idProfessions.Count();

                    procedure = this.repository.AllReadonly<ProcedurePrice>
                       (x => x.IdTypeApplication == kvLicensingFee.IdKeyValue
                       && ((x.ExpirationDateFrom <= newDate && !x.ExpirationDateTo.HasValue) ||
                       (x.ExpirationDateTo.HasValue && x.ExpirationDateFrom <= newDate && x.ExpirationDateTo >= newDate))
                       && ((x.CountProfessionsFrom <= profCount && !x.CountProfessionsTo.HasValue) ||
                       (x.CountProfessionsTo.HasValue && x.CountProfessionsFrom <= profCount && x.CountProfessionsTo >= profCount))).FirstOrDefault();

                }
                else
                {
                    procedure = this.repository.AllReadonly<ProcedurePrice>
                        (x => x.IdTypeApplication == kvLicensingFee.IdKeyValue
                        && ((x.ExpirationDateFrom <= newDate && !x.ExpirationDateTo.HasValue) ||
                        (x.ExpirationDateTo.HasValue && x.ExpirationDateFrom <= newDate && x.ExpirationDateTo >= newDate))).FirstOrDefault();

                }
                payment.ResultContextObject.IdProcedurePrice = procedure.IdProcedurePrice;
                payment.ResultContextObject.PaymentAmount = (double?)procedure.Price;
                payment.ResultContextObject.PaymentReason = procedure.Name;
                payment.ResultContextObject.AdditionalInformation = procedure.AdditionalInformation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                payment.AddErrorMessage("Има грешка при автоматичното въвеждане на нова заявка!");
            }        
            return payment;
        }
        public async Task<ResultContext<PaymentVM>> GetPaymentOrder(string recieptId)
        {
            ResultContext<PaymentVM> resultContext = new ResultContext<PaymentVM>();
            try
            {

                string baseURL = (await dataSourceService.GetSettingByIntCodeAsync("BaseURLPayEGov")).SettingValue;
                string url = "https://pay-test.egov.bg:44310/ais/paymentOrder";
                    //baseURL + "paymentOrder";

                string ClientId = (await dataSourceService.GetSettingByIntCodeAsync("ClientIdPayEGov")).SettingValue;

                string SecretKey = (await dataSourceService.GetSettingByIntCodeAsync("SecretKeyPayEGov")).SettingValue;


                string body = Convert.ToBase64String(
                   System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                   {
                       id = recieptId
                   })));


                string signature = BaseHelper.CalculateHmac(SecretKey, body);

                _logger.LogInformation($"ClientId:{ClientId}, Hmac:{signature}, Data:{body}");

                Dictionary<string, string> keyValues = new Dictionary<string, string>()
                {
                    { "clientId", ClientId},
                    { "hmac", signature },
                    { "data", body }
                };

                string content = null;

                using (HttpClient client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(keyValues));

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            _logger.LogError("PayEGov StatusCode:" + response.StatusCode);

                        }

                        content = await response.Content.ReadAsStringAsync();

                        var resPaymentOrder = JsonConvert.DeserializeObject<PaymentVM>(content);

                        resultContext.ResultContextObject = resPaymentOrder;

                        if (resPaymentOrder.PaymentOrderURl == null)
                        {
                            resultContext.AddErrorMessage("Системата не може да изкара платежно нареждане");

                            _logger.LogError("PaymentOrder is null");
                        }
                    }
                    catch (Exception exp)
                    {

                        throw exp;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }
        public async Task<int> GetProfCount(CandidateProviderVM candidateProviderVM)
        {
            var data = this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == candidateProviderVM.IdCandidate_Provider)
                .Include(x => x.Speciality)
                .ThenInclude(x => x.Profession).ToList();

            return data.Select(x => x.Speciality.Profession.IdProfession).Distinct().Count();
        }
    }
}

