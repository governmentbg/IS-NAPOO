using System.Text;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.Archive;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ISNAPOO.WebSystem.Controllers
{
    public class PaymentController : Controller
    {
        IPaymentService _paymentService;
        private readonly IDataSourceService _dataSourceService;
        public ILogger<PaymentController> _logger { get; set; }
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IDataSourceService dataSourceService)
        {
            this._paymentService = paymentService;
            this._dataSourceService = dataSourceService;
            this._logger = logger;
        }
 
        [HttpPost]
        public async Task<IActionResult> PayEGov(string ClientId, string Hmac, string Data)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation($"Request from PayEGov and Request IP:{ip}" );
            _logger.LogInformation($"ClientId:{ClientId}");
            _logger.LogInformation($"Hmac:{Hmac}");
            _logger.LogInformation($"Data:{Data}");
            

            string SecretKey = (await _dataSourceService.GetSettingByIntCodeAsync("SecretKeyPayEGov")).SettingValue;

            string signature = BaseHelper.CalculateHmac(SecretKey, Data);

            if (signature != Hmac) 
            {
                _logger.LogInformation($"The signature:{signature} is not equals to:{Hmac} for data:{Data}");
                return Json(new { success = "false" });
            }

            var paymentStatus = JsonConvert.DeserializeObject<PaymentStatus>(
                        Encoding.UTF8.GetString(Convert.FromBase64String(Data)));

            ResultContext<PaymentVM> resultContext = await this._paymentService.UpdatePaymentStatusAsync(
                new Common.Framework.ResultContext<PaymentStatus>() { ResultContextObject = paymentStatus }
                );

            
            if (!resultContext.HasErrorMessages)
            {
                _logger.LogInformation($"The payment with ReceiptId:{paymentStatus.Id} has been updated with status:{paymentStatus.Status}.");
            }
            else 
            {
                _logger.LogError($"The payment with ReceiptId:{paymentStatus.Id} HAS NOT BEEN UPDATED with status:{paymentStatus.Status}.");

                return Json(new { success = "false" });

            }
            


            return Json(new { success = "true"});
        }
    }
}
