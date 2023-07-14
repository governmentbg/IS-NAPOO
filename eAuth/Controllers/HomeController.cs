using eAuth.Models;
using eAuth.Resources;
using eAuth.Utils;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static eAuth.Utils.SamlHelper;

namespace eAuth.Controllers
{
    public class HomeController : BaseController
    {

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        

        public IActionResult AuthenticateCertificate(
            string SamlResponse,
            string RelayState)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            try
            {
                if (string.IsNullOrEmpty(SamlResponse))
                {

                    string FailLoginRedirect = configuration.GetSection("AppSettings")["FailLoginRedirect"];
                    return Redirect(FailLoginRedirect);
                }

                string decodedStr = Encoding.UTF8.GetString(Convert.FromBase64String(SamlResponse));


                System.IO.File.WriteAllText(@"C:\Resources_NAPOO\eAuth\samlResponse." + DateTime.Now.Ticks + ".xml", decodedStr);

                CertificateAuthResponse response = new CertificateAuthResponse();

                switch (SamlHelper.SamlConfiguration.SamlVersion)
                {
                    case 1:
                        response = SamlHelper.ParseSaml2CertificateResult(decodedStr);
                        break;
                    case 2:
                        response = SamlHelper.ParseSaml2CertificateResultV2(decodedStr);
                        break;
                }

                ResultContext<TokenVM> currentToken = new ResultContext<TokenVM>();

                string relayStateStr = Encoding.UTF8.GetString(Convert.FromBase64String(RelayState));
                currentToken.ResultContextObject.Token = relayStateStr;

                ResultContext<TokenVM> resultContext = new ResultContext<TokenVM>();



                try
                {
                    resultContext = BaseHelper.GetDecodeToken(currentToken);
                }
                catch (Exception e)
                {

                    string FailLoginRedirect = configuration.GetSection("AppSettings")["FailLoginRedirect"];
                    return Redirect(FailLoginRedirect);
                }

                List<KeyValuePair<string, object>> keyValuePairsList = new List<KeyValuePair<string, object>>();
                keyValuePairsList.AddRange(resultContext.ResultContextObject.ListDecodeParams.Where(c => c.Key != "exp"));
                keyValuePairsList.Add(new KeyValuePair<string, object>("EGN", response.EGN));
                keyValuePairsList.Add(new KeyValuePair<string, object>("Email", response.Email));
                keyValuePairsList.Add(new KeyValuePair<string, object>("LatinNames", response.LatinNames));
                keyValuePairsList.Add(new KeyValuePair<string, object>("ResponseStatus", response.ResponseStatus.ToString()));
                keyValuePairsList.Add(new KeyValuePair<string, object>("PhoneNumber", response.PhoneNumber));

                var token = BaseHelper.GetTokenWithParams(keyValuePairsList, 1);



                string UrlEAuthEGovBGPage = configuration.GetSection("AppSettings")["UrlEAuthEGovBGPage"] ?? "http://hv-devserver-03:8181/EAuthEGovBG";

                return Redirect($"{UrlEAuthEGovBGPage}?token={token}");
            }
            catch (Exception)
            {

                string FailLoginRedirect = configuration.GetSection("AppSettings")["FailLoginRedirect"];
                return Redirect(FailLoginRedirect);
            }
            

        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> CertificateAuthV2(string token = null)
        {
            logger.Info("CertificateAuthV2 call");

            CertAuthViewModel model = new CertAuthViewModel();


            try
            {
                SAML2.Config.Saml2Config.Init(SamlHelper.getSaml2Config());
            }
            catch { }

            X509Certificate2 cert = SAML2.Config.Saml2Config.Current
                .ServiceProvider
                .SigningCertificate
                .GetCertificate();



            System.Xml.XmlDocument authnRequest =
                SamlHelper.GenerateKEPAuthnRequest(model);

            XmlElement e = authnRequest.DocumentElement;
            e.Prefix = "saml2p";

            // Remove the genre attribute.
            e.RemoveAttribute("xmlns:samlp");
            e.RemoveAttribute("xmlns:saml");
            e.RemoveAttribute("xmlns:egovbga");
            e.RemoveAttribute("ProviderName");


            foreach (XmlNode n in authnRequest.DocumentElement.ChildNodes)
            {
                if (n.Name == "saml:Issuer")
                {
                    n.Prefix = "saml2";
                }
                else if (n.Name == "samlp:Extensions")
                {
                    n.Prefix = "saml2p";
                }
                else if (n.Name == "samlp:NameIDPolicy")
                {
                    n.ParentNode.RemoveChild(n);
                }
            }

            authnRequest.Save(@"C:\Resources_NAPOO\eAuth\authnRequest.xml." + DateTime.Now.Ticks + ".xml");

            string signedXml = SamlHelper.SignXmlDocument(authnRequest, cert);



            string encodedStr = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(signedXml));

            model.EncodedRequest = encodedStr;


            if (string.IsNullOrEmpty(token)) 
            {
                token = "EmptyToken";
            }

            model.EncodedRelayState = !string.IsNullOrEmpty(token)
                ? Convert.ToBase64String(Encoding.UTF8.GetBytes(token))
                : string.Empty;



            return View("CertificateAuth", model);
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult Saml()
        {
            string saml = SamlHelper.GetSPMetdata();

            return Content(saml, "application/xml", System.Text.Encoding.UTF8);
        }


    }

}
