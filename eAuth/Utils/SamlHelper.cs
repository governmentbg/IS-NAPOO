using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Web.Administration;
using SAML2;
using SAML2.Config;
using SAML2.Schema.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using eAuth.Enums;
using eAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net.Http;
using SAML2.Utils;

namespace eAuth.Utils
{
    public class SamlHelper
    {

        private static ILog logger = LogManager.GetLogger("SamlHelper");


        public static class SamlConfiguration
        {
            public static int SamlVersion { get; }
            public static string LoginUrl { get; }
            public static string ReturnUrl { get; }
            public static string TargetUrl { get; }
            public static string ProviderName { get; }
            public static string ProviderId { get; }
            public static string ExtServiceId { get; }
            public static string ExtProviderId { get; }
            public static string RequestIssuerValue { get; }
            public static string GetPfxCertificate { get; }
            public static byte[] GetCertificate { get; }


            static SamlConfiguration()
            {
                IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();
                //var Default = configuration.GetSection("AppSettings")["bg.gov.eAuth.providerId"];
                ProviderId = configuration.GetSection("AppSettings")["bg.gov.eAuth.providerId"];
                ExtServiceId = configuration.GetSection("AppSettings")["bg.gov.eAuth.extService"];
                ExtProviderId = configuration.GetSection("AppSettings")["bg.gov.eAuth.extProvider"];

                SamlVersion = Int32.Parse(configuration.GetSection("AppSettings")["bg.gov.eAuth.version"] ?? "1");
                LoginUrl = configuration.GetSection("AppSettings")[$"bg.gov.eAuth.{SamlVersion}.loginUrl".Replace("..", ".")];
                ReturnUrl = configuration.GetSection("AppSettings")[$"bg.gov.eAuth.{SamlVersion}.returnUrl".Replace("..", ".")];
                TargetUrl = configuration.GetSection("AppSettings")[$"bg.gov.eAuth.{SamlVersion}.targetUrl".Replace("..", ".")];
                ProviderName = configuration.GetSection("AppSettings")[$"bg.gov.eAuth.{SamlVersion}.providerName".Replace("..", ".")];

                RequestIssuerValue = configuration.GetSection("AppSettings")[$"RequestIssuerValue".Replace("..", ".")];
            }

        }


        /// <summary>
        /// Saml AuthnRequest to eAuthenticator
        /// </summary>
        /// <param name="returnURL"></param>
        /// <returns></returns>
        internal static XmlDocument GenerateKEPAuthnRequest(Models.CertAuthViewModel requestInfo)
        {
            SAML2.Saml20AuthnRequest req = new SAML2.Saml20AuthnRequest();
            req.ProtocolBinding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";
            req.Request.ProviderName = requestInfo.ProviderName;
            req.Request.Issuer.SPProvidedID = requestInfo.ProviderId;
            //req.Request.Issuer.Value = requestInfo.ReturnUrl;
            req.Request.Issuer.Value = AppSettings.GetConfigurationKey("RequestIssuerValue");
            req.Request.IssueInstant = DateTime.Now;
            req.Request.Destination = requestInfo.TargetUrl;
            req.Request.ForceAuthn = false;
            req.Request.IsPassive = false;
            req.Request.AssertionConsumerServiceUrl = requestInfo.ReturnUrl;

            System.Xml.XmlElement[] ext = GetExtensions(requestInfo.ExtServiceId, requestInfo.ExtProviderId);
            req.Request.Extensions = new SAML2.Schema.Protocol.Extensions();
            req.Request.Extensions.Any = ext;
            var namespaces = new XmlSerializerNamespaces(SamlSerialization.XmlNamespaces);

            XmlDocument resp = new XmlDocument();
            switch (SamlHelper.SamlConfiguration.SamlVersion)
            {
                case 1:
                    namespaces.Add("egovbga", "urn:bg:egov:eauth:1.0:saml:ext");
                    break;
                case 2:
                    //req.Request.Issuer.Value = SAML2.Config.Saml2Config.Current.AllowedAudienceUris.FirstOrDefault();
                    req.Request.Issuer.Value = SamlHelper.SamlConfiguration.RequestIssuerValue; //"https://edelivery-test.egov.bg/metadata/info/saml"

                    req.Request.NameIdPolicy = new NameIdPolicy() { AllowCreate = true };
                    namespaces.Add("egovbga", "urn:bg:egov:eauth:2.0:saml:ext");
                    break;
            }

            resp = SamlSerialization.Serialize(req.Request, namespaces);

            //remove the declaration
            if (resp.FirstChild is XmlDeclaration)
            {
                resp.RemoveChild(resp.FirstChild);
            }

            return resp;
        }


        /// <summary>
        /// Generata saml AuthNRequest extensions
        /// </summary>
        /// <param name="service"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        private static System.Xml.XmlElement[] GetExtensions(string service, string provider)
        {
            var extensionStr = string.Empty;
            switch (SamlConfiguration.SamlVersion)
            {
                case 1:
                    extensionStr = String.Format(@"<egovbga:RequestedService><egovbga:Service>{0}</egovbga:Service><egovbga:Provider>{1}</egovbga:Provider></egovbga:RequestedService>", service, provider);
                    break;
                case 2:
                    StringBuilder sb = new StringBuilder();

                    sb.AppendFormat(@"<egovbga:RequestedService><egovbga:Service>{0}</egovbga:Service><egovbga:Provider>{1}</egovbga:Provider><egovbga:LevelOfAssurance>SUBSTANTIAL</egovbga:LevelOfAssurance></egovbga:RequestedService>", service, provider);
                    sb.Append(@"<egovbga:RequestedAttributes xmlns:egovbga=""urn:bg:egov:eauth:2.0:saml:ext"">");
                    sb.Append(@"<egovbga:RequestedAttribute FriendlyName=""latinName"" Name=""urn:egov:bg:eauth:2.0:attributes:latinName"" NameFormat=""urn:oasis:names:tc:saml2:2.0:attrname-format:uri"" isRequired=""true"">");
                    sb.Append(@"<egovbga:AttributeValue>urn:egov:bg:eauth:2.0:attributes:latinName</egovbga:AttributeValue></egovbga:RequestedAttribute>");
                    sb.Append(@"<egovbga:RequestedAttribute FriendlyName=""birthName"" Name=""urn:egov:bg:eauth:2.0:attributes:birthName"" NameFormat=""urn:oasis:names:tc:saml2:2.0:attrname-format:uri"" isRequired=""true"">");
                    sb.Append(@"<egovbga:AttributeValue>urn:egov:bg:eauth:2.0:attributes:birthName</egovbga:AttributeValue></egovbga:RequestedAttribute>");
                    sb.Append(@"<egovbga:RequestedAttribute FriendlyName=""email"" Name=""urn:egov:bg:eauth:2.0:attributes:email"" NameFormat=""urn:oasis:names:tc:saml2:2.0:attrname-format:uri""	isRequired=""true"">");
                    sb.Append(@"<egovbga:AttributeValue>urn:egov:bg:eauth:2.0:attributes:email</egovbga:AttributeValue></egovbga:RequestedAttribute>");
                    sb.Append(@"<egovbga:RequestedAttribute FriendlyName=""phone"" Name=""urn:egov:bg:eauth:2.0:attributes:phone"" NameFormat=""urn:oasis:names:tc:saml2:2.0:attrname-format:uri"" isRequired=""true"">");
                    sb.Append(@"<egovbga:AttributeValue>urn:egov:bg:eauth:2.0:attributes:phone</egovbga:AttributeValue></egovbga:RequestedAttribute>");
                    sb.Append(@"<egovbga:RequestedAttribute FriendlyName=""gender"" Name=""urn:egov:bg:eauth:2.0:attributes:gender"" NameFormat=""urn:oasis:names:tc:saml2:2.0:attrname-format:uri"" isRequired=""true"">");
                    sb.Append(@"<egovbga:AttributeValue>urn:egov:bg:eauth:2.0:attributes:gender</egovbga:AttributeValue></egovbga:RequestedAttribute>");
                    sb.Append(@"<egovbga:RequestedAttribute FriendlyName=""dateOfBirth"" Name=""urn:egov:bg:eauth:2.0:attributes:dateOfBirth"" NameFormat=""urn:oasis:names:tc:saml2:2.0:attrname-format:uri"" isRequired=""true"">");
                    sb.Append(@"<egovbga:AttributeValue>urn:egov:bg:eauth:2.0:attributes:dateOfBirth</egovbga:AttributeValue></egovbga:RequestedAttribute>");
                    sb.Append(@"<egovbga:RequestedAttribute FriendlyName=""placeOfBirth"" Name=""urn:egov:bg:eauth:2.0:attributes:placeOfBirth"" NameFormat=""urn:oasis:names:tc:saml2:2.0:attrname-format:uri"" isRequired=""true"">");
                    sb.Append(@"<egovbga:AttributeValue>urn:egov:bg:eauth:2.0:attributes:placeOfBirth</egovbga:AttributeValue></egovbga:RequestedAttribute>");
                    sb.Append(@"<egovbga:RequestedAttribute FriendlyName=""canonicalResidenceAddress"" Name=""urn:egov:bg:eauth:2.0:attributes:canonicalResidenceAddress"" NameFormat=""urn:oasis:names:tc:saml2:2.0:attrname-format:uri"" isRequired=""true"">");
                    sb.Append(@"<egovbga:AttributeValue>urn:egov:bg:eauth:2.0:attributes:canonicalResidenceAddress</egovbga:AttributeValue></egovbga:RequestedAttribute>");
                    sb.Append(@"</egovbga:RequestedAttributes>");
                    extensionStr = sb.ToString();
                    break;
            }

            var doc = new System.Xml.XmlDocument();
            using (var sr = new System.IO.StringReader($"<root>{extensionStr}</root>"))
            using (var xtr = new System.Xml.XmlTextReader(sr) { Namespaces = false })
            {
                doc.Load(xtr);
            }
            return doc.DocumentElement.ChildNodes.Cast<XmlElement>().ToArray();
        }


        internal static string GetSPMetdata()
        {
            //byte[] certificate = Convert.FromBase64String(SamlHelper.GetPfxCertificate());
            //X509Certificate2 cert = new X509Certificate2(certificate, "12345678", X509KeyStorageFlags.Exportable);
            //X509Certificate2 cert = new X509Certificate2("smcon.com.2022-01-07.pfx", "12345678", X509KeyStorageFlags.Exportable);
            //X509Certificate2 cert = new X509Certificate2(AppSettings.GetConfigurationKey("Certificate"), "12345678", X509KeyStorageFlags.Exportable);

            try
            {
                SAML2.Config.Saml2Config.Init(SamlHelper.getSaml2Config());
            }
            catch { }

            X509Certificate2 cert = SAML2.Config.Saml2Config.Current
                .ServiceProvider
                .SigningCertificate
                .GetCertificate();

            var keyinfo = new KeyInfo();
            //var keyClause = new KeyInfoX509Data(SAML2.Config.Saml2Config.Current.ServiceProvider.SigningCertificate.GetCertificate(), X509IncludeOption.EndCertOnly);
            var keyClause = new KeyInfoX509Data(cert, X509IncludeOption.EndCertOnly);
            keyinfo.AddClause(keyClause);

            try
            {
                SAML2.Config.Saml2Config.Init(getSaml2Config());
            }
            catch { }

            var doc = new Saml20MetadataDocument(SAML2.Config.Saml2Config.Current, keyinfo, true);

            //to remove
            File.WriteAllText(@"C:\Resources_NAPOO\eAuth\Metadata.saml.eAuth.xml", doc.ToXml(Encoding.UTF8));


            return doc.ToXml(Encoding.UTF8);
        }


        /// <summary>
        /// Get Saml2 from appconfig.json and convert it to SAML2.Config.Saml2Config
        /// </summary>
        /// <returns></returns>
        public static Saml2Config getSaml2Config()
        {
            //get saml2 from appsettings.json
            AppSettings.Saml2 saml2 = AppSettings.GetSaml2("Saml2");

            SAML2.Config.Saml2Config saml2Config = new SAML2.Config.Saml2Config();

            saml2Config.AllowedAudienceUris = new List<string>();
            saml2Config.AllowedAudienceUris = saml2.AllowedAudienceUris;
            saml2Config.CommonDomainCookie.Enabled = saml2.CommonDomainCookie.Enabled;

            SAML2.Config.ServiceProviderConfig ServiceProvider = new SAML2.Config.ServiceProviderConfig();
            SAML2.Config.Certificate cer = new SAML2.Config.Certificate();
            cer.FindValue = saml2.ServiceProvider.SigningCertificate.FindValue;
            cer.StoreLocation = (StoreLocation)saml2.ServiceProvider.SigningCertificate.StoreLocation;
            cer.StoreName = (StoreName)saml2.ServiceProvider.SigningCertificate.StoreName;
            cer.X509FindType = (X509FindType)saml2.ServiceProvider.SigningCertificate.X509FindType;
            ServiceProvider.SigningCertificate = cer;

            ServiceProviderEndpoint serviceProviderEndpoint;

            foreach (var endpoint in saml2.ServiceProvider.Endpoints)
            {
                serviceProviderEndpoint = new ServiceProviderEndpoint();
                serviceProviderEndpoint.Type = (EndpointType)endpoint.Type;
                serviceProviderEndpoint.LocalPath = endpoint.LocalPath;
                serviceProviderEndpoint.RedirectUrl = endpoint.RedirectUrl;
                ServiceProvider.Endpoints.Add(serviceProviderEndpoint);
            }

            ServiceProvider.Id = saml2.ServiceProvider.Id;
            ServiceProvider.Server = saml2.ServiceProvider.Server;

            saml2Config.ServiceProvider = ServiceProvider;

            SAML2.Config.IdentityProvider identityProvider;

            foreach (var provider in saml2.IdentityProviders)
            {
                identityProvider = new SAML2.Config.IdentityProvider();
                identityProvider.Name = provider.Name;
                identityProvider.Id = provider.Id;

                IdentityProviderEndpoint identityProviderEndpoint;

                foreach (var endpoint in provider.Endpoints)
                {
                    identityProviderEndpoint = new IdentityProviderEndpoint();
                    identityProviderEndpoint.Type = (EndpointType)endpoint.Type;
                    identityProviderEndpoint.Url = endpoint.Url;
                    identityProviderEndpoint.Binding = (BindingType)endpoint.Binding;
                    identityProvider.Endpoints.Add(identityProviderEndpoint);
                }

                foreach (var certificateValidation in provider.CertificateValidations)
                {
                    identityProvider.CertificateValidations.Add(certificateValidation);
                }

                saml2Config.IdentityProviders.Add(identityProvider);
            }

            MetadataConfig metadata = new MetadataConfig();
            metadata.Lifetime = TimeSpan.FromSeconds(saml2.Metadata.Lifetime.Seconds); //TimeSpan.FromSeconds(15);

            metadata.Organization = new Organization();
            metadata.Organization.Name = saml2.Metadata.Organization.Name;
            metadata.Organization.DisplayName = saml2.Metadata.Organization.DisplayName;
            metadata.Organization.Url = saml2.Metadata.Organization.Url;

            foreach (var metadataContact in saml2.Metadata.Contacts)
            {
                Contact contact = new Contact();
                contact.Type = (ContactType)metadataContact.Type;
                contact.Company = metadataContact.Company;
                contact.GivenName = metadataContact.GivenName;
                contact.SurName = metadataContact.SurName;
                contact.Email = metadataContact.Email;
                contact.Phone = metadataContact.Phone;
                metadata.Contacts.Add(contact);
            }

            SAML2.Config.Attribute attribute;

            foreach (var requestedAttributes in saml2.Metadata.RequestedAttributes)
            {
                attribute = new SAML2.Config.Attribute();
                attribute.Name = requestedAttributes.Name;
                attribute.IsRequired = requestedAttributes.IsRequired;
                metadata.RequestedAttributes.Add(attribute);
            }

            saml2Config.Metadata = metadata;

            //Serialize and Deserialize saml2Config to prepare json for convert to class (https://json2csharp.com/)
            //var serJ = System.Text.Json.JsonSerializer.Serialize(saml2Config);
            //var deserJ = JsonConvert.DeserializeObject(serJ);


            return saml2Config;
        }


        /// <summary>
        /// Sign an xml document with the provided certificate
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="pathToCertificate"></param>
        /// <returns></returns>
        internal static string SignXmlDocument(XmlDocument xmlDocument, X509Certificate2 certificate)
        {
            //get the reference to be signed
            string reference = xmlDocument.DocumentElement.GetAttribute("ID");

            var signedDocument = SignDocument(xmlDocument, certificate, "");
            //var signedDocument = SignDocument(xmlDocument, certificate, reference);

            //to remove
            //signedDocument.Save("signedDocument.xml");
            signedDocument.Save(@"C:\Resources_NAPOO\eAuth\signedAuthnRequest.xml." + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xml");

            var signedDocumentString = SignedDocumentToString(signedDocument);
            //bool verified = VerifyXmlFile(signedDocumentString, "Signature", "ds");
            bool verified = VerifyXmlFile(signedDocumentString, "Signature");
            if (!verified)
            {
                //ElmahLogger.Instance.Info("Can not verify signed AuthNRequest xml");
            }
            return signedDocumentString;
        }


        /// <summary>
        /// Sign an xml document
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cert"></param>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        private static XmlDocument SignDocument(XmlDocument doc, X509Certificate2 cert, string referenceId)
        {
            SignedXml xml = new SignedXml(doc)
            {
                SignedInfo =  {
                                CanonicalizationMethod = SignedXml.XmlDsigExcC14NWithCommentsTransformUrl,
                                SignatureMethod = SignedXml.XmlDsigRSASHA256Url //XmlDsigRSASHA1Url
                            },
                SigningKey = cert.PrivateKey,

            };



            System.Security.Cryptography.Xml.Reference reference = new System.Security.Cryptography.Xml.Reference(referenceId);
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());
            xml.AddReference(reference);

            xml.KeyInfo = new KeyInfo();
            var keyInfoData = new KeyInfoX509Data(cert, X509IncludeOption.EndCertOnly);
            keyInfoData.AddIssuerSerial(cert.Issuer, cert.SerialNumber);
            keyInfoData.AddSubjectName(cert.Subject);
            xml.KeyInfo.AddClause(keyInfoData);
            xml.ComputeSignature();

            //XmlElement signature = xml.GetXml();

            //SetPrefix("ds", signature);

            //// Load modified signature back
            //xml.LoadXml(signature);

            //// this is workaround for overcoming a bug in the library
            //xml.SignedInfo.References.Clear();

            //xml.AddReference(reference);
            //xml.KeyInfo.AddClause(keyInfoData);

            //// Recompute the signature
            //xml.ComputeSignature();
            //string recomputedSignature = Convert.ToBase64String(xml.SignatureValue);

            ////signatureXml = xml.GetXml();

            //// Replace value of the signature with recomputed one
            //ReplaceSignature(signature, recomputedSignature);

            //// Append the signature to the XML document. 
            //doc.DocumentElement.InsertAfter(doc.ImportNode(signature, true), doc.DocumentElement.FirstChild);

            var signatureXml = xml.GetXml();

            //signatureXml
            if (doc.DocumentElement != null)
            {
                XmlNodeList elementsByTagName = doc.DocumentElement.GetElementsByTagName("Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
                System.Xml.XmlNode parentNode = elementsByTagName[0].ParentNode;
                if (parentNode != null)
                {
                    parentNode.InsertAfter(doc.ImportNode(signatureXml, true), elementsByTagName[0]);
                }
            }
            return doc;
        }


        /// <summary>
        /// Replace signature with recomputed signature
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="newValue"></param>
        private static void ReplaceSignature(XmlElement signature, string newValue)
        {
            if (signature == null) throw new ArgumentNullException(nameof(signature));
            if (signature.OwnerDocument == null) throw new ArgumentException("No owner document", nameof(signature));

            XmlNamespaceManager nsm = new XmlNamespaceManager(signature.OwnerDocument.NameTable);
            nsm.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);

            XmlNode signatureValue = signature.SelectSingleNode("ds:SignatureValue", nsm);
            if (signatureValue == null)
                throw new Exception("Signature does not contain 'ds:SignatureValue'");

            signatureValue.InnerXml = newValue;
        }


        /// <summary>
        /// Write prefix to signature (ie "ds")
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="node"></param>
        private static void SetPrefix(string prefix, XmlNode node)
        {
            node.Prefix = prefix;
            foreach (XmlNode n in node.ChildNodes)
            {
                SetPrefix(prefix, n);
            }
        }


        /// <summary>
        /// Writes a document to a string
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string SignedDocumentToString(XmlDocument doc)
        {
            // Save the signed XML document to a file specified 
            // using the passed string.
            StringBuilder sb = new StringBuilder();
            using (TextWriter sw = new StringWriter(sb))
            {
                using (XmlTextWriter xmltw = new XmlTextWriter(sw))
                {
                    doc.WriteTo(xmltw);
                }
            }
            return sb.ToString();

        }


        //public static Boolean VerifyXmlFile(XmlDocument xmlDoc, RSA Key, string prefix)
        //{
        //    SignedXml signedXml = new SignedXml(xmlDoc);

        //    //Get the <ds:Signature /> element
        //    XmlElement xmlSignature = (XmlElement)xmlDoc.GetElementsByTagName(prefix + ":Signature")[0];

        //    //Undo what we did after signing
        //    AssignNameSpacePrefixToElementTree(xmlSignature, "");

        //    //Now it will pass verification.
        //    signedXml.LoadXml(xmlSignature);
        //    return signedXml.CheckSignature(Key);
        //}


        public static Boolean VerifyXmlFile(String xmlString, string signatureTag, string prefix)
        {
            // Create a new XML document.
            XmlDocument xmlDocument = new XmlDocument();

            // Format using white spaces.
            //xmlDocument.PreserveWhitespace = true;

            // Load the passed XML file into the document. 
            xmlDocument.LoadXml(xmlString);

            // Create a new SignedXml object and pass it 
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDocument.DocumentElement);

            // Find the "Signature" node and create a new 
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDocument.GetElementsByTagName(prefix + signatureTag);

            // Load the signature node.
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result. 
            return signedXml.CheckSignature();

        }


        public static Boolean VerifyXmlFile(String xmlString, string signatureTag)
        {
            // Create a new XML document.
            XmlDocument xmlDocument = new XmlDocument();

            // Format using white spaces.
            //xmlDocument.PreserveWhitespace = true;

            // Load the passed XML file into the document. 
            xmlDocument.LoadXml(xmlString);

            // Create a new SignedXml object and pass it 
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDocument.DocumentElement);

            // Find the "Signature" node and create a new 
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDocument.GetElementsByTagName(signatureTag);

            // Load the signature node.
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result. 
            return signedXml.CheckSignature();

        }


        /// <summary>
        /// Parce cert samlp:Response object
        /// </summary>
        /// <param name="SamlResponse"></param>
        /// <returns></returns>
        internal static CertificateAuthResponse ParseSaml2CertificateResult(string SamlResponse)
        {
            var certAuthResponce = new CertificateAuthResponse();
            //ElmahLogger.Instance.Info("eAuth saml response: " + SamlResponse);
            if (string.IsNullOrEmpty(SamlResponse))
            {
                throw new ArgumentNullException("SamlResponse");
            }
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(SamlResponse);
            }
            catch (Exception ex)
            {
                //ElmahLogger.Instance.Error(ex, "Can not load samlresponse object!");
                certAuthResponce.ResponseStatus = Enums.eCertResponseStatus.InvalidResponseXML;
                return certAuthResponce;
            }

            bool valid = VerifyXmlFile(SamlResponse, "ds:Signature");
            if (!valid)
            {

                //ElmahLogger.Instance.Error("SamlResponse Invalid Signature!");
                certAuthResponce.ResponseStatus = Enums.eCertResponseStatus.InvalidSignature;
                return certAuthResponce;
            }

            try
            {
                var responseElement = doc.DocumentElement;

                var samlNS = new XmlNamespaceManager(doc.NameTable);
                samlNS.AddNamespace("egovbga", "urn:bg:egov:eauth:1.0:saml:ext");
                samlNS.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
                samlNS.AddNamespace("saml2", "urn:oasis:names:tc:SAML:2.0:assertion");
                samlNS.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
                samlNS.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
                var statusCode = responseElement.SelectSingleNode("//samlp:Status/samlp:StatusCode", samlNS);
                if (statusCode != null)
                {
                    var statusCodeValue = statusCode.Attributes["Value"].Value;
                    var innerStatusCode = statusCode.SelectSingleNode("samlp:StatusCode", samlNS);
                    if (innerStatusCode != null)
                    {
                        statusCodeValue = innerStatusCode.Attributes["Value"].Value;
                    }
                    var statusMessage = responseElement.SelectSingleNode("//samlp:Status/samlp:StatusMessage", samlNS);
                    certAuthResponce.ResponseStatusMessage = statusMessage != null ? HttpUtility.HtmlDecode(statusMessage.InnerText) : string.Empty;
                    //certAuthResponce.ResponseStatus = GetResponseStatusFromCode(statusCodeValue, certAuthResponce.ResponseStatusMessage);
                }

                if (certAuthResponce.ResponseStatus != Enums.eCertResponseStatus.Success)
                {
                    return certAuthResponce;
                }

                //successful result => get data
                var subjectNode = responseElement.SelectSingleNode("//saml2:Subject", samlNS);
                if (subjectNode != null)
                {
                    //get egn
                    var egn = subjectNode.SelectSingleNode("saml2:NameID[@NameQualifier='urn:egov:bg:eauth:1.0:attributes:eIdentifier:EGN']", samlNS);
                    if (egn != null)
                    {
                        certAuthResponce.EGN = egn.InnerText;
                        certAuthResponce.DateOfBirth = TextHelper.GetBirthDateFromEGN(certAuthResponce.EGN);
                    }
                }

                if (String.IsNullOrEmpty(certAuthResponce.EGN))
                {
                    //ElmahLogger.Instance.Error("Can not extract egn from response! Can not continue process for authentication!");
                    certAuthResponce.ResponseStatus = Enums.eCertResponseStatus.MissingEGN;
                    return certAuthResponce;
                }

                //get phone and email if any
                var attributes = responseElement.SelectSingleNode("//saml2:AttributeStatement", samlNS);
                if (attributes != null)
                {
                    var phone = attributes.SelectSingleNode("saml2:Attribute[@Name='urn:egov:bg:eauth:1.0:attributes:phone']/saml2:AttributeValue", samlNS);
                    if (phone != null) certAuthResponce.PhoneNumber = phone.InnerText;
                    var email = attributes.SelectSingleNode("saml2:Attribute[@Name='urn:egov:bg:eauth:1.0:attributes:eMail']/saml2:AttributeValue", samlNS);
                    if (email != null) certAuthResponce.Email = email.InnerText;
                    var latinName = attributes.SelectSingleNode("saml2:Attribute[@Name='urn:egov:bg:eauth:1.0:attributes:personNamesLatin']/saml2:AttributeValue", samlNS);
                    if (latinName != null) certAuthResponce.LatinNames = latinName.InnerText;
                }

                return certAuthResponce;
            }
            catch (Exception ex)
            {
                //ElmahLogger.Instance.Error(ex, "eAuth response Parsing Error!");
                throw;
            }
        }


        /// <summary>
        /// Parse saml2.0 response
        /// </summary>
        /// <param name="decodedStr"></param>
        /// <returns></returns>
        internal static CertificateAuthResponse ParseSaml2CertificateResultV2(string decodedStr, bool validateDate = true)
        {
            var response = new CertificateAuthResponse()
            {
                ResponseStatus = eCertResponseStatus.Success
            };
            var responseV2 = SAML2.Utils.Serialization.DeserializeFromXmlString<SAML2.Schema.Protocol.Response>(decodedStr);
            if (responseV2 == null)
            {
                response.ResponseStatus = eCertResponseStatus.InvalidResponseXML;
                return response;
            }

            //Save Assertion to file
            XmlDocument assertion = new XmlDocument();
            var assertionElement = ExtractAssertion(decodedStr);
            if (assertionElement == null)
            {
                logger.Error("Can not extract asserion element from " + decodedStr);
                //return false;
            }

            try
            {
                assertion.LoadXml(assertionElement.OuterXml);
                assertion.Save(@"C:\Resources_NAPOO\eAuth\samlAssertion_decode." + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xml");
            }
            catch
            {
                logger.Error("Can not extract asserion element from " + decodedStr);
            }


            if (responseV2.Status.StatusCode.Value != Saml20Constants.StatusCodes.Success)
            {
                logger.Error("Saml20 status not Success: Status code is: " + responseV2.Status.StatusCode.Value);
                response.ResponseStatus = eCertResponseStatus.AuthenticationFailed;
                response.ResponseStatusMessage = responseV2.Status.StatusMessage ?? responseV2.Status.StatusCode.Value;
                return response;
            }

            Saml20Assertion assertation = null;//responseV2.Items[0] as SAML2.Schema.Core.Assertion;
            //validation
            if (!ValidateAssertationSignature(decodedStr, out assertation))
            {
                //logger.Error("Saml20 signature can not be validated!");
                response.ResponseStatus = eCertResponseStatus.InvalidSignature;
                return response;
            }

            List<AssertionAttribute> assertationList = null;
            ResponseAttributes responseAttributes = null;
            Saml20Assertion saml20Assertion = null;

            if (!SamlHelper.GetAssertation(decodedStr, out assertationList, out responseAttributes, out saml20Assertion))
            {
                logger.Error("Cannot get Assertion Element!");
                return response;
            }

            assertation = saml20Assertion;

            var nw = DateTime.UtcNow;
            if (validateDate && responseAttributes?.NotOnOrAfter != null && responseAttributes.NotOnOrAfter < nw)
            {
                logger.Error($"SAML2 parse response: assertation.Conditions.NotOnOrAfter value is {responseAttributes.NotOnOrAfter} is exceeded by current time: {nw}");
                //expired response
                response.ResponseStatus = eCertResponseStatus.InvalidResponseXML;
                return response;
            }
            var attributes = assertation.Attributes;
            var personalIdentifier = attributes.SingleOrDefault(x => x.Name == "urn:egov:bg:eauth:2.0:attributes:personIdentifier")?.AttributeValue?.FirstOrDefault();
            if (!string.IsNullOrEmpty(personalIdentifier))
            {
                response.EGN = personalIdentifier.Split(new char[] { '-', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries).Last();
            }

            if (string.IsNullOrEmpty(response.EGN))
            {
                response.ResponseStatus = eCertResponseStatus.MissingEGN;
                return response;
            }

            var names = attributes.SingleOrDefault(x => x.Name == "urn:egov:bg:eauth:2.0:attributes:personName")?.AttributeValue?.FirstOrDefault();
            if (!string.IsNullOrEmpty(names))
            {
                response.LatinNames = names;
            }


            var email = attributes.SingleOrDefault(x => x.Name == "urn:egov:bg:eauth:2.0:attributes:email")?.AttributeValue?.FirstOrDefault();
            if (!string.IsNullOrEmpty(email))
            {
                response.Email = email;
            }


            var phone = attributes.SingleOrDefault(x => x.Name == "urn:egov:bg:eauth:2.0:attributes:phone")?.AttributeValue?.FirstOrDefault();
            if (!string.IsNullOrEmpty(phone))
            {
                response.PhoneNumber = phone;
            }

            var dob = attributes.SingleOrDefault(x => x.Name == "urn:egov:bg:eauth:2.0:attributes:dateOfBirth")?.AttributeValue?.FirstOrDefault();
            if (!string.IsNullOrEmpty(dob))
            {
                response.DateOfBirth = DateTime.Parse(dob);
            }
            return response;
        }

        /// <summary>
        /// Get Assertion
        /// </summary>
        /// <param name="decodedStr"></param>
        /// <param name="assertionList"></param>
        /// <param name="responseAttributes"></param>
        /// <param name="saml20Assertion"></param>
        /// <returns></returns>
        public static bool GetAssertation(string decodedStr,
            out List<AssertionAttribute> assertionList,
            out ResponseAttributes responseAttributes,
            out Saml20Assertion saml20Assertion)
        {
            assertionList = new List<AssertionAttribute>();

            responseAttributes = new ResponseAttributes();

            saml20Assertion = new Saml20Assertion();
            saml20Assertion.Attributes = new List<SAML2.Schema.Core.SamlAttribute>();

            XmlDocument responseXML = new XmlDocument { PreserveWhitespace = true };
            responseXML.LoadXml(decodedStr);

            assertionList.Add(new AssertionAttribute
            {
                Name = responseXML.GetElementsByTagName("saml2p:StatusCode")[0].Name.Replace("saml2p:", ""),
                Value = responseXML.GetElementsByTagName("saml2p:StatusCode")[0].Attributes[0].Value.Replace("urn:oasis:names:tc:SAML:2.0:status:", "")
            });

            if (responseXML.GetElementsByTagName("saml2p:StatusCode")[0].Attributes[0].Value.Replace("urn:oasis:names:tc:SAML:2.0:status:", "") == "Success")
                responseAttributes.ResponseStatus = eCertResponseStatus.Success;
            else
                responseAttributes.ResponseStatus = eCertResponseStatus.AuthenticationFailed;

            XmlDocument assertion = new XmlDocument();
            var assertionElement = ExtractAssertion(decodedStr);
            if (assertionElement == null)
            {
                logger.Error("Can not extract asserion element from " + decodedStr);
                return false;
            }

            assertion.LoadXml(assertionElement.OuterXml);

            //assertionAttributes = new List<AssertionAttribute>();

            assertionList.Add(new AssertionAttribute
            {
                Name = assertion.GetElementsByTagName("saml2:NameID")[0].Name.Replace("saml2:", ""),
                Value = assertion.GetElementsByTagName("saml2:NameID")[0].InnerText
            });

            assertionList.Add(new AssertionAttribute
            {
                Name = assertion.GetElementsByTagName("saml2:AuthnStatement")[0].Attributes[0].Name,
                Value = assertion.GetElementsByTagName("saml2:AuthnStatement")[0].Attributes[0].InnerText
            });


            assertionList.Add(new AssertionAttribute
            {
                Name = assertion.GetElementsByTagName("saml2:AuthenticatingAuthority")[0].Name.Replace("saml2:", ""),
                Value = assertion.GetElementsByTagName("saml2:AuthenticatingAuthority")[0].InnerText
            });


            foreach (XmlAttribute xmlAttribute in assertion.GetElementsByTagName("saml2:SubjectConfirmationData")[0].Attributes)
            {
                assertionList.Add(new AssertionAttribute
                {
                    Name = xmlAttribute.Name,
                    Value = xmlAttribute.Value
                });

                if (xmlAttribute.Name == "NotOnOrAfter")
                {
                    responseAttributes.NotOnOrAfter = Convert.ToDateTime(xmlAttribute.Value);
                }

            }

            foreach (XmlNode childNode in assertion.GetElementsByTagName("saml2:AttributeStatement")[0].ChildNodes)
            {
                assertionList.Add(new AssertionAttribute
                {
                    Name = childNode.Attributes[0].InnerText.Replace("urn:egov:bg:eauth:2.0:attributes:", ""),
                    Value = childNode.InnerText
                });

                List<string> attributes = new List<string>();
                attributes.Add(childNode.InnerText);

                try
                {
                    saml20Assertion.Attributes.Add(new SAML2.Schema.Core.SamlAttribute
                    {
                        Name = childNode?.Attributes[0].InnerText,
                        AttributeValue = new string[] { childNode?.InnerText.ToString() }
                    });
                }
                catch
                {

                }


                //saml20Assertion.Attributes.

                if (childNode.Attributes[0].InnerText.Replace("urn:egov:bg:eauth:2.0:attributes:", "") == "personName")
                    responseAttributes.LatinNames = childNode.InnerText;

                if (childNode.Attributes[0].InnerText.Replace("urn:egov:bg:eauth:2.0:attributes:", "") == "personIdentifier")
                    responseAttributes.EGN = childNode.InnerText.Split(new char[] { '-', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries).Last();

                if (childNode.Attributes[0].InnerText.Replace("urn:egov:bg:eauth:2.0:attributes:", "") == "email")
                    responseAttributes.Email = childNode.InnerText;

                if (childNode.Attributes[0].InnerText.Replace("urn:egov:bg:eauth:2.0:attributes:", "") == "X509")
                {
                    try
                    {
                        string sX5019 = System.Uri.UnescapeDataString(childNode.InnerText)
                                                .Replace("\n", "")
                                                .Replace("-----BEGIN CERTIFICATE-----", "")
                                                .Replace("-----END CERTIFICATE-----", "");
                        byte[] cerBytes = Convert.FromBase64String(sX5019);
                        responseAttributes.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(cerBytes);
                    }
                    catch
                    {
                        responseAttributes.Certificate = null;
                    }
                }



            }

            return true;

        }


        /// <summary>
        /// Validate SAML2 assertation signature
        /// </summary>
        /// <param name="decodedStr"></param>
        /// <returns></returns>
        public static bool ValidateAssertationSignature(string decodedStr, out Saml20Assertion assertion)
        {
            assertion = null;
            var assertionElement = ExtractAssertion(decodedStr);
            if (assertionElement == null)
            {
                logger.Error("Can not extract asserion element from " + decodedStr);
                return false;
            }

            return true;


            //var issuer = GetIssuer(assertionElement);
            //var endpoint = RetrieveIDPConfiguration(issuer);
            //var endpoint2 = SamlHelper.GetSPMetdata();

            //X509Certificate2 cert =
            //    SAML2.Config.Saml2Config.Current.ServiceProvider.SigningCertificate.GetCertificate();

            //List<AsymmetricAlgorithm> listAsymmetricAlgorithm = new List<AsymmetricAlgorithm>(new AsymmetricAlgorithm[1]
            //{
            //    cert.PublicKey.Key
            //});

            //try
            //{
            //    bool boolResult;
            //    boolResult = CheckSignature(assertionElement, cert.PublicKey.Key);

            //    //assertion = new Saml20Assertion(assertionElement, null, SAML2.Config.Saml2Config.Current.AssertionProfile.AssertionValidator, true);
            //    //assertion = new Saml20Assertion(assertionElement, _GetTrustedSigners(), null, false);

            //    boolResult = CheckSignatureGM(assertionElement, cert.PublicKey.Key);


            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}

        }


        /// <summary>
        /// Get trusted signers
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="identityProvider"></param>
        /// <returns></returns>
        private static IEnumerable<AsymmetricAlgorithm> GetTrustedSigners(ICollection<SAML2.Schema.Metadata.KeyDescriptor> keys, SAML2.Config.IdentityProvider identityProvider)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            var result = new List<AsymmetricAlgorithm>(keys.Count);
            foreach (var keyDescriptor in keys)
            {
                foreach (KeyInfoClause clause in (KeyInfo)keyDescriptor.KeyInfo)
                {
                    var key = XmlSignatureUtils.ExtractKey(clause);
                    result.Add(key);
                }
            }

            return result;
        }


        /// <summary>
        /// Extract assertion element from 
        /// </summary>
        /// <param name="decodedStr"></param>
        /// <returns></returns>
        private static XmlElement ExtractAssertion(string decodedStr)
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(decodedStr);

            var assertionList = doc.DocumentElement.GetElementsByTagName(SAML2.Schema.Core.Assertion.ElementName, Saml20Constants.Assertion);
            if (assertionList.Count == 1)
            {
                var assertionElement = (XmlElement)assertionList[0];
                return assertionElement;
            }

            //try encrypted
            var encryptedAssList = doc.GetElementsByTagName(EncryptedAssertion.ElementName, Saml20Constants.Assertion);
            if (encryptedAssList.Count == 1)
            {
                var privateKey = (RSA)SAML2.Config.Saml2Config.Current.ServiceProvider.SigningCertificate.GetCertificate().PrivateKey;

                var encryptedAssertion = new Saml20EncryptedAssertion(privateKey);
                encryptedAssertion.LoadXml((XmlElement)encryptedAssList[0]);
                // Act
                encryptedAssertion.Decrypt();
                return encryptedAssertion.Assertion.DocumentElement;
            }

            return null;
        }


        /// <summary>
        /// Retrieves the name of the issuer from an XmlElement containing an assertion.
        /// </summary>
        /// <param name="assertion">An XmlElement containing an assertion</param>
        /// <returns>The identifier of the Issuer</returns>
        private static string GetIssuer(XmlElement assertion)
        {
            var result = string.Empty;
            var list = assertion.GetElementsByTagName("Issuer", Saml20Constants.Assertion);
            if (list.Count > 0)
            {
                var issuer = (XmlElement)list[0];
                result = issuer.InnerText;
            }

            return result;
        }


        /// <summary>
        /// Looks through the Identity Provider configurations and
        /// </summary>
        /// <param name="idpId">The identity provider id.</param>
        /// <returns>The <see cref="IdentityProviderElement"/>.</returns>
        private static SAML2.Config.IdentityProvider RetrieveIDPConfiguration(string idpId)
        {
            //var config = Saml2Section.GetConfig();
            var config = getSaml2Config();
            return config.IdentityProviders.FirstOrDefault(x => x.Id == idpId);
        }


        /// <summary>
        /// Metadata fetcher module
        /// </summary>
        internal class MetaDataHelper
        {
            private ILog logger;
            public MetaDataHelper(ILog logParam)
            {
                logger = logParam;
            }

            /// <summary>
            /// Load idp metadata from url
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            private Saml20MetadataDocument LoadIdPMetadata(string url)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var uri = new Uri(url);
                        var metadataStr = client.GetStringAsync(uri).Result;

                        var doc = new XmlDocument { PreserveWhitespace = true };
                        doc.LoadXml(metadataStr);

                        logger.Info($"Loaded metadata for url {url}: metadata:{metadataStr}");
                        // init metadata
                        var metadata = new Saml20MetadataDocument(doc);
                        return metadata;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Can not load IDP metadata from url " + url, ex);
                    return null;
                }
            }

        }

        internal static byte[] GetCertificate(string fileName)
        {
            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);

            //Build the File Path.
            string path = Path.Combine("wwwroot", "cert/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return bytes;
        }

        internal static class AppSettings
        {
            public static string GetConfigurationKey(string Key)
            {
                var fileFullPath = "appsettings.json";// base.Source.FileProvider.GetFileInfo(base.Source.Path).PhysicalPath;
                string json = File.ReadAllText(fileFullPath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                string Value = jsonObj["AppSettings"][Key];
                //string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                //File.WriteAllText(fileFullPath, output);

                return Value;
            }

            public static string GetConfigurationSection(string SectionName)
            {
                var fileFullPath = "appsettings.json";// base.Source.FileProvider.GetFileInfo(base.Source.Path).PhysicalPath;
                string json = File.ReadAllText(fileFullPath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                string Value = jsonObj["SectionName"];
                //string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                //File.WriteAllText(fileFullPath, output);

                return Value;
            }

            public static Saml2 GetSaml2Section(string SectionName)
            {
                var fileFullPath = "appsettings.json";// base.Source.FileProvider.GetFileInfo(base.Source.Path).PhysicalPath;
                string json = File.ReadAllText(fileFullPath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);


                return JsonConvert.DeserializeObject<Saml2>(JsonConvert.SerializeObject(jsonObj[SectionName]));
            }

            public static string GetConfigurationKey(string Section, string Key)
            {
                var fileFullPath = "appsettings.json";// base.Source.FileProvider.GetFileInfo(base.Source.Path).PhysicalPath;
                string json = File.ReadAllText(fileFullPath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                string Value = jsonObj[Section][Key];
                //string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                //File.WriteAllText(fileFullPath, output);


                return Value;
            }

            internal static Saml2 GetSaml2(string SectionName)
            {
                var fileFullPath = "appsettings.json";// base.Source.FileProvider.GetFileInfo(base.Source.Path).PhysicalPath;
                string json = File.ReadAllText(fileFullPath);
                dynamic jsonObj = JsonConvert.DeserializeObject(json);


                return JsonConvert.DeserializeObject<Saml2>(JsonConvert.SerializeObject(jsonObj[SectionName]));
            }

            #region "Десериализиране на "Saml2" секция от appsettings.json

            //Generated by https://json2csharp.com/
            // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

            /// <summary>
            /// Основен метод за десериализиране на "Saml2" секция от appsettings.json
            /// </summary>
            public class Saml2
            {
                public List<object> Actions { get; set; }
                public List<string> AllowedAudienceUris { get; set; }
                public AssertionProfile AssertionProfile { get; set; }
                public CommonDomainCookie CommonDomainCookie { get; set; }
                public List<IdentityProvider> IdentityProviders { get; set; }
                public object IdentityProviderSelectionUrl { get; set; }
                public Logging Logging { get; set; }
                public Metadata Metadata { get; set; }
                public ServiceProvider ServiceProvider { get; set; }
                public State State { get; set; }
            }

            public class AssertionProfile
            {
                public object AssertionValidator { get; set; }
            }

            public class CommonDomainCookie
            {
                public bool Enabled { get; set; }
                public object LocalReaderEndpoint { get; set; }
            }

            public class Contact
            {
                public string Company { get; set; }
                public string Email { get; set; }
                public string GivenName { get; set; }
                public string Phone { get; set; }
                public string SurName { get; set; }
                public int Type { get; set; }
            }

            public class Endpoint
            {
                public int Binding { get; set; }
                public object ForceProtocolBinding { get; set; }
                public object TokenAccessor { get; set; }
                public int Type { get; set; }
                public string Url { get; set; }
                public int Index { get; set; }
                public string LocalPath { get; set; }
                public string RedirectUrl { get; set; }
            }

            public class IdentityProvider
            {
                public object Metadata { get; set; }
                public bool AllowUnsolicitedResponses { get; set; }
                public bool Default { get; set; }
                public bool ForceAuth { get; set; }
                public string Id { get; set; }
                public bool IsPassive { get; set; }
                public string Name { get; set; }
                public bool OmitAssertionSignatureCheck { get; set; }
                public bool QuirksMode { get; set; }
                public object ResponseEncoding { get; set; }
                public object ArtifactResolution { get; set; }
                public object AttributeQuery { get; set; }
                public List<string> CertificateValidations { get; set; }
                public CommonDomainCookie CommonDomainCookie { get; set; }
                public List<Endpoint> Endpoints { get; set; }
                public object LogoutEndpoint { get; set; }
                public object SignOnEndpoint { get; set; }
                public object PersistentPseudonym { get; set; }
            }

            public class Lifetime
            {
                public int Ticks { get; set; }
                public int Days { get; set; }
                public int Hours { get; set; }
                public int Milliseconds { get; set; }
                public int Minutes { get; set; }
                public int Seconds { get; set; }
                public double TotalDays { get; set; }
                public double TotalHours { get; set; }
                public int TotalMilliseconds { get; set; }
                public double TotalMinutes { get; set; }
                public int TotalSeconds { get; set; }
            }

            public class Logging
            {
                public object LoggingFactory { get; set; }
            }

            public class LogoutEndpoint
            {
                public int Binding { get; set; }
                public int Index { get; set; }
                public string LocalPath { get; set; }
                public string RedirectUrl { get; set; }
                public int Type { get; set; }
            }

            public class Metadata
            {
                public bool ExcludeArtifactEndpoints { get; set; }
                public Lifetime Lifetime { get; set; }
                public List<Contact> Contacts { get; set; }
                public Organization Organization { get; set; }
                public List<RequestedAttribute> RequestedAttributes { get; set; }
            }

            public class Organization
            {
                public string DisplayName { get; set; }
                public string Name { get; set; }
                public string Url { get; set; }
            }

            public class RequestedAttribute
            {
                public bool IsRequired { get; set; }
                public string Name { get; set; }
            }

            public class ServiceProvider
            {
                public string Id { get; set; }
                public string Server { get; set; }
                public int AuthenticationContextComparison { get; set; }
                public List<object> AuthenticationContexts { get; set; }
                public List<Endpoint> Endpoints { get; set; }
                public LogoutEndpoint LogoutEndpoint { get; set; }
                public SignOnEndpoint SignOnEndpoint { get; set; }
                public bool NameIdFormatAllowCreate { get; set; }
                public List<object> NameIdFormats { get; set; }
                public SigningCertificate SigningCertificate { get; set; }
            }

            public class Settings
            {
            }

            public class SigningCertificate
            {
                public string FindValue { get; set; }
                public int StoreLocation { get; set; }
                public int StoreName { get; set; }
                public bool ValidOnly { get; set; }
                public int X509FindType { get; set; }
            }

            public class SignOnEndpoint
            {
                public int Binding { get; set; }
                public int Index { get; set; }
                public string LocalPath { get; set; }
                public string RedirectUrl { get; set; }
                public int Type { get; set; }
            }

            public class State
            {
                public object StateServiceFactory { get; set; }
                public Settings Settings { get; set; }
            }

            #endregion

        }


    }
}
