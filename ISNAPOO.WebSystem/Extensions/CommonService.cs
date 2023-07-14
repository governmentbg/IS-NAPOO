using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Resources;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using JWT.Serializers;
using Microsoft.AspNetCore.Components;

namespace ISNAPOO.WebSystem.Extensions
{
    public class CommonService : ICommonService
    {

        private readonly ILocService locService;
        private readonly ILogger<CommonService> logger;

        //const string secret = "cAwKUwjBDoyBo0v4ryZpw55NG5U4YQAnkDO0LH5KXvQJEuyjCR2msY3ZGCGk7kb";

        public CommonService(ILogger<CommonService> _logger, ILocService _locService)
        {
            this.logger = _logger;
            this.locService = _locService;
        }

        public string GetTokenWithParams(ResultContext<TokenVM> currentContext, int minute = GlobalConstants.MAX_MINUTE_VALID_TOKEN)
        {
           
            string res = string.Empty;

            //res = JwtBuilder.Create()
            //          .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
            //          .WithSecret(GlobalConstants.TOKEN_SECRET)
            //          .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(hours).ToUnixTimeSeconds())
            //          .AddClaims(currentContext.ResultContextObject.ListDecodeParams)
            //          .Encode();
            res = BaseHelper.GetTokenWithParams(currentContext.ResultContextObject.ListDecodeParams, minute);


            return res;
        }

        public ResultContext<TokenVM> GetDecodeToken(ResultContext<TokenVM> currentContext)
        {

            try
            {
                //JWT.IJsonSerializer serializer = new JsonNetSerializer();
                //IDateTimeProvider provider = new UtcDateTimeProvider();
                //IJwtValidator validator = new JwtValidator(serializer, provider);
                //IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                //IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                //IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);



                //currentContext.ResultContextObject.DecodeToken = decoder.Decode(currentContext.ResultContextObject.Token, secret, verify: true);
                //currentContext.ResultContextObject.IsValid = true;

                //var deserializeObject = serializer.Deserialize<Newtonsoft.Json.Linq.JObject>(currentContext.ResultContextObject.DecodeToken);

                //foreach (var item in deserializeObject)
                //{
                //    currentContext.ResultContextObject.ListDecodeParams.Add(new KeyValuePair<string, object>(item.Key, item.Value));
                //}

                currentContext = BaseHelper.GetDecodeToken(currentContext);

            }
            catch (TokenExpiredException)
            {
                currentContext.ResultContextObject.IsValid = false;
                currentContext.AddMessage(locService.GetLocalizedHtmlString("LinkIsExpired"));
                currentContext.AddErrorMessage(locService.GetLocalizedHtmlString("TokenHasExpired"));
                logger.LogInformation(locService.GetLocalizedHtmlString("LinkIsExpired"));
                logger.LogInformation(locService.GetLocalizedHtmlString("TokenHasExpired"));
            }
            catch (SignatureVerificationException)
            {
                currentContext.ResultContextObject.IsValid = false;
                currentContext.AddMessage(locService.GetLocalizedHtmlString("LinkHasInvalidSignature"));
                currentContext.AddErrorMessage(locService.GetLocalizedHtmlString("TokenHasInvalidSignature"));
                logger.LogInformation(locService.GetLocalizedHtmlString("LinkHasInvalidSignature"));
                logger.LogInformation(locService.GetLocalizedHtmlString("TokenHasInvalidSignature"));

            }
            catch (Exception ee)
            {
                currentContext.ResultContextObject.IsValid = false;
                currentContext.AddMessage(locService.GetLocalizedHtmlString("LinkHasInvalidSignature"));
                currentContext.AddErrorMessage(locService.GetLocalizedHtmlString("TokenHasInvalidSignature"));
                logger.LogInformation(ee.Message);
                logger.LogInformation(ee.StackTrace);
                logger.LogInformation(ee.InnerException?.Message);

            }
            return currentContext;
        }
    }

    
}
