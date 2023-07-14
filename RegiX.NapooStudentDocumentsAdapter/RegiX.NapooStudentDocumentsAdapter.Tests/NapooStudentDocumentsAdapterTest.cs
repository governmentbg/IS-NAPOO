using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechnoLogica.RegiX.NapooStudentDocumentsAdapter.AdapterService;
using TechnoLogica.RegiX.Adapters.TestUtils;
using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.Adapters.Common.Interfaces;
using TechnoLogica.RegiX.Common.DataContracts;
using System.IO;
using TechnoLogica.RegiX.Common.Utils;
using System.Xml;
using TechnoLogica.RegiX.Common.ObjectMapping;

namespace TechnoLogica.RegiX.NapooStudentDocumentsAdapter.Tests
{
    [TestClass]
    public class NapooStudentDocumentsAdapterTest : AdapterTest<AdapterService.NapooStudentDocumentsAdapter, INapooStudentDocumentsAdapter>
    {
        //[TestMethod]
        //public void NapooStudentDocumentsAdapter_CheckHealthCheckAndParamtersTest()
        //{
        //    NapooStudentDocumentsAdapter adapter = new NapooStudentDocumentsAdapter();
        //    //test GetParameters , and ConnectionString
        //    var connectionString = adapter.GetParameters().ParameterList.Where(p => p.Key == TechnoLogica.RegiX.Common.Constants.WebServiceUrlParameterKeyName).FirstOrDefault();
        //    //test SetParameter
        //    adapter.SetParameter(TechnoLogica.RegiX.Common.Constants.WebServiceUrlParameterKeyName, connectionString.CurrentValue);
        //    //test HCfunctions
        //    var hcFunctions = adapter.GetHealthCheckFunctions();
        //    string checkHealthFunctionResult = string.Empty;
        //    hcFunctions.HealthInfosList.ForEach(f =>
        //    {
        //        checkHealthFunctionResult = checkHealthFunctionResult + f.Key + ":" + adapter.CheckHealthFunction(f.Key) + ";\r\n";
        //    });
        //    using (StreamWriter outfile = new StreamWriter("CheckHealthFunctions.xml", false, System.Text.Encoding.UTF8))
        //    {
        //        outfile.Write(checkHealthFunctionResult);
        //    }
        //    Assert.IsTrue(true);
        //}

        //     [TestMethod]
        //     public void NapooStudentDocumentsAdapter_CheckRegisterConnection()
        //     {
        //AdapterService.NapooStudentDocumentsAdapter adapter = new AdapterService.NapooStudentDocumentsAdapter();
        //      //   adapter.ParameterStore = Composition.CompositionContainer.GetExportedValue<IParameterStore>();

        //         //adapter.SetParameter(Constants.WebServiceUrlParameterKeyName, "https://eauth3.smcon.com:8083/");


        //         string result = adapter.CheckRegisterConnection();
        //         Assert.IsTrue(result == TechnoLogica.RegiX.Common.Constants.ConnectionOk || result == TechnoLogica.RegiX.Common.Constants.ConnectionStringNotSet);
        //     }

        [TestMethod]
        public void GetDocumentsByStudent_Test()
        {
            TechnoLogica.RegiX.Common.DataContracts.CallContext context = new TechnoLogica.RegiX.Common.DataContracts.CallContext()
            {
                AdministrationOId = "https://trust-party-openid.com",
                LawReason = "�������� �� RegiX",
                ServiceURI = "1222-200030-12.12.2022",
                Remark = "���������",
                EmployeeAditionalIdentifier = "����� ����� 3",
                EmployeeIdentifier = "test@tesactivedirectory.com",
                EmployeeNames = "������ ���������� ����������",
                AdministrationName = "��������� ������� �� ��������",
                EmployeePosition = "������ ������� ����� \"�������� �� ������������\"",
                ResponsiblePersonIdentifier = "392309324",
                ServiceType = "�� ������������ �������"
            };
            string oldcontext = context.ToString();
			// "\"LawReason\":\"�������� �� RegiX\",\"ServiceURI\":\"123456789\",\�ServiceType\�:\��� ������������ �������\�\"Remark\":\"\",\"RequestorNames\":\"Ivelin Ivanov\",\"UserAdministrationName\":\"������������ �� ����������� � �������\",\� UserAdministrationID\�:\�10.20.503.4\�,\" RequestorIdentifier\":\"IIvanov@technologica.com\",\"RequestorIdentifierOptional\":\"123456\",\"UserPosition\":\"Developer\",\"EAuthToken\":\"https://trust-party-openid.com\"";
			AdapterService.NapooStudentDocumentsAdapter adapter = new AdapterService.NapooStudentDocumentsAdapter();
            var accessMatrix = AccessMatrix.CreateForType(typeof(DocumentsByStudentResponse));
            TechnoLogica.RegiX.NapooStudentDocumentsAdapter.DocumentsByStudentRequestType searchCriteria = new TechnoLogica.RegiX.NapooStudentDocumentsAdapter.DocumentsByStudentRequestType() { StudentIdentifier = "6702088980" };

            AdapterAdditionalParameters additionalParameters = new AdapterAdditionalParameters();
            additionalParameters.CallContext = new TechnoLogica.RegiX.Common.DataContracts.CallContext() { Remark = "RegiXTest" };// "\"LawReason\":\"�������� �� RegiX\",\"ServiceURI\":\"123456789\",�ServiceType�:��� ������������ �������\"Remark\":\"\",\"RequestorNames\":\"Ivelin Ivanov\",\"UserAdministrationName\":\"������������ �� ����������� � �������\",� UserAdministrationID�:�10.20.503.4�,\" RequestorIdentifier\":\"IIvanov@technologica.com\",\"RequestorIdentifierOptional\":\"123456\",\"UserPosition\":\"Developer\",\"EAuthToken\":\"https://trust-party-openid.com\"";
            additionalParameters.CitizenEGN = "8888888888";
            additionalParameters.ClientIPAddress = "111.111.111.111";
            additionalParameters.ConsumerCertificateThumbprint = "###";
            additionalParameters.EIDToken = "token123";
            additionalParameters.EmployeeEGN = "11111111111111";
            additionalParameters.OrganizationEIK = "12121212121";
            additionalParameters.OrganizationUnit = "qqqqqq";
            additionalParameters.SignResult = false;
            additionalParameters.ReturnAccessMatrix = true;

            var result = adapter.GetDocumentsByStudent(searchCriteria, accessMatrix, additionalParameters);
            string xml = result.XmlSerialize();
            using (StreamWriter outfile = new StreamWriter("GetDocumentsByStudent_Test.xml", false, System.Text.Encoding.UTF8))
            {
                outfile.Write(xml);
            }

            XsltUtils.RunXsltAndWriteHtml("NapooStudentDocumentsAdapter", "DocumentsByStudentRequest", result.Data.Request.XmlSerialize());
            XsltUtils.RunXsltAndWriteHtml("NapooStudentDocumentsAdapter", "DocumentsByStudentResponse", result.Data.Response.XmlSerialize());

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            if (SigningUtils.HasSignature(doc))
            {
                bool isValid = SigningUtils.ValidateXmlDocumentWithX509Certificate(doc);
                if (isValid)
                {
                    Assert.IsTrue(true);
                }
            }
            else
            {
                Assert.IsTrue(true);
            }
        }

        //[TestMethod]
        //public void GetDocumentsByStudent_NULL_Test()
        //{
        //    TechnoLogica.RegiX.Common.TransportObject.CallContext context = new TechnoLogica.RegiX.Common.TransportObject.CallContext()
        //    {
        //        AdministrationOId = "https://trust-party-openid.com",
        //        LawReason = "�������� �� RegiX",
        //        ServiceURI = "1222-200030-12.12.2022",
        //        Remark = "���������",
        //        EmployeeAditionalIdentifier = "����� ����� 3",
        //        EmployeeIdentifier = "test@tesactivedirectory.com",
        //        EmployeeNames = "������ ���������� ����������",
        //        AdministrationName = "��������� ������� �� ��������",
        //        EmployeePosition = "������ ������� ����� \"�������� �� ������������\"",
        //        ResponsiblePersonIdentifier = "392309324",
        //        ServiceType = "�� ������������ �������"
        //    };
        //    string oldcontext = context.ToString();
        //    // "\"LawReason\":\"�������� �� RegiX\",\"ServiceURI\":\"123456789\",\�ServiceType\�:\��� ������������ �������\�\"Remark\":\"\",\"RequestorNames\":\"Ivelin Ivanov\",\"UserAdministrationName\":\"������������ �� ����������� � �������\",\� UserAdministrationID\�:\�10.20.503.4\�,\" RequestorIdentifier\":\"IIvanov@technologica.com\",\"RequestorIdentifierOptional\":\"123456\",\"UserPosition\":\"Developer\",\"EAuthToken\":\"https://trust-party-openid.com\"";
        //    NapooStudentDocumentsAdapter adapter = new NapooStudentDocumentsAdapter();
        //    var accessMatrix = AccessMatrix.CreateForType(typeof(DocumentsByStudentResponse));
        //    TechnoLogica.RegiX.NapooStudentDocumentsAdapter.DocumentsByStudentRequestType searchCriteria = new TechnoLogica.RegiX.NapooStudentDocumentsAdapter.DocumentsByStudentRequestType() { StudentIdentifier = "1234567890" };

        //    AdapterAdditionalParameters additionalParameters = new AdapterAdditionalParameters();
        //    additionalParameters.CallContext = new TechnoLogica.RegiX.Common.TransportObject.CallContext() { Remark = "RegiXTest" };// "\"LawReason\":\"�������� �� RegiX\",\"ServiceURI\":\"123456789\",�ServiceType�:��� ������������ �������\"Remark\":\"\",\"RequestorNames\":\"Ivelin Ivanov\",\"UserAdministrationName\":\"������������ �� ����������� � �������\",� UserAdministrationID�:�10.20.503.4�,\" RequestorIdentifier\":\"IIvanov@technologica.com\",\"RequestorIdentifierOptional\":\"123456\",\"UserPosition\":\"Developer\",\"EAuthToken\":\"https://trust-party-openid.com\"";
        //    additionalParameters.CitizenEGN = "8888888888";
        //    additionalParameters.ClientIPAddress = "111.111.111.111";
        //    additionalParameters.ConsumerCertificateThumbprint = "###";
        //    additionalParameters.EIDToken = "token123";
        //    additionalParameters.EmployeeEGN = "11111111111111";
        //    additionalParameters.OrganizationEIK = "12121212121";
        //    additionalParameters.OrganizationUnit = "qqqqqq";
        //    additionalParameters.SignResult = true;
        //    additionalParameters.ReturnAccessMatrix = true;

        //    var result = adapter.GetDocumentsByStudent(searchCriteria, accessMatrix, additionalParameters);
        //    string xml = result.XmlSerialize(); ;
        //    using (StreamWriter outfile = new StreamWriter("GetDocumentsByStudent_NULL_Test.xml", false, System.Text.Encoding.UTF8))
        //    {
        //        outfile.Write(xml);
        //    }

        //    XsltUtils.RunXsltAndWriteHtml("NapooStudentDocumentsAdapter", "DocumentsByStudentRequest", result.Data.Request.XmlSerialize());
        //    XsltUtils.RunXsltAndWriteHtml("NapooStudentDocumentsAdapter", "DocumentsByStudentResponse", result.Data.Response.XmlSerialize());

        //    XmlDocument doc = new XmlDocument();
        //    doc.LoadXml(xml);
        //    if (SigningUtils.HasSignature(doc))
        //    {
        //        bool isValid = SigningUtils.ValidateXmlDocumentWithX509Certificate(doc.DocumentElement);
        //        if (isValid)
        //        {
        //            Assert.IsTrue(true);
        //        }
        //    }
        //    else
        //    {
        //        Assert.IsTrue(true);
        //    }
        //}


        [TestMethod]
        public void GetStudentDocument_Test()
        {
            TechnoLogica.RegiX.Common.DataContracts.CallContext context = new TechnoLogica.RegiX.Common.DataContracts.CallContext()
            {
                AdministrationOId = "https://trust-party-openid.com",
                LawReason = "�������� �� RegiX",
                ServiceURI = "1222-200030-12.12.2022",
                Remark = "���������",
                EmployeeAditionalIdentifier = "����� ����� 3",
                EmployeeIdentifier = "test@tesactivedirectory.com",
                EmployeeNames = "������ ���������� ����������",
                AdministrationName = "��������� ������� �� ��������",
                EmployeePosition = "������ ������� ����� \"�������� �� ������������\"",
                ResponsiblePersonIdentifier = "392309324",
                ServiceType = "�� ������������ �������"
            };
            string oldcontext = context.ToString();
			// "\"LawReason\":\"�������� �� RegiX\",\"ServiceURI\":\"123456789\",\�ServiceType\�:\��� ������������ �������\�\"Remark\":\"\",\"RequestorNames\":\"Ivelin Ivanov\",\"UserAdministrationName\":\"������������ �� ����������� � �������\",\� UserAdministrationID\�:\�10.20.503.4\�,\" RequestorIdentifier\":\"IIvanov@technologica.com\",\"RequestorIdentifierOptional\":\"123456\",\"UserPosition\":\"Developer\",\"EAuthToken\":\"https://trust-party-openid.com\"";
			AdapterService.NapooStudentDocumentsAdapter adapter = new AdapterService.NapooStudentDocumentsAdapter();
            //adapter.SetParameter("WebServiceUrlForGetStudentDocument", "http://172.23.105.82/Regix/Mockups/RegiX.NapooStudentDocumentsAdapterMockup/ServiceRefForMethod2Service.svc");
            var accessMatrix = AccessMatrix.CreateForType(typeof(DocumentsByStudentResponse));
            TechnoLogica.RegiX.NapooStudentDocumentsAdapter.StudentDocumentRequestType searchCriteria = new TechnoLogica.RegiX.NapooStudentDocumentsAdapter.StudentDocumentRequestType()
            {
                //DocumentRegistrationNumber = null,
                //StudentIdentifier = null
                DocumentRegistrationNumber = "102-1", 
                StudentIdentifier = "6702088980"
            };

            AdapterAdditionalParameters additionalParameters = new AdapterAdditionalParameters();
            additionalParameters.CallContext = new TechnoLogica.RegiX.Common.DataContracts.CallContext() { Remark = "RegiXTest" };
            additionalParameters.CitizenEGN = "8888888888";
            additionalParameters.ClientIPAddress = "111.111.111.111";
            additionalParameters.ConsumerCertificateThumbprint = "###";
            additionalParameters.EIDToken = "token123";
            additionalParameters.EmployeeEGN = "11111111111111";
            additionalParameters.OrganizationEIK = "12121212121";
            additionalParameters.OrganizationUnit = "qqqqqq";
            additionalParameters.SignResult = false;
            additionalParameters.ReturnAccessMatrix = true;

            var result = adapter.GetStudentDocument(searchCriteria, accessMatrix, additionalParameters);
            string xml = result.XmlSerialize(); ;
            using (StreamWriter outfile = new StreamWriter("GetStudentDocument_Test.xml", false, System.Text.Encoding.UTF8))
            {
                outfile.Write(xml);
            }

            XsltUtils.RunXsltAndWriteHtml("NapooStudentDocumentsAdapter", "DocumentsByStudentRequest", result.Data.Request.XmlSerialize());
            XsltUtils.RunXsltAndWriteHtml("NapooStudentDocumentsAdapter", "DocumentsByStudentResponse", result.Data.Response.XmlSerialize());

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            if (SigningUtils.HasSignature(doc))
            {
                bool isValid = SigningUtils.ValidateXmlDocumentWithX509Certificate(doc);
                if (isValid)
                {
                    Assert.IsTrue(true);
                }
            }
            else
            {
                Assert.IsTrue(true);
            }
        }

        //[TestMethod]
        //public void GetStudentDocument_NULL_Test()
        //{
        //    TechnoLogica.RegiX.Common.TransportObject.CallContext context = new TechnoLogica.RegiX.Common.TransportObject.CallContext()
        //    {
        //        AdministrationOId = "https://trust-party-openid.com",
        //        LawReason = "�������� �� RegiX",
        //        ServiceURI = "1222-200030-12.12.2022",
        //        Remark = "���������",
        //        EmployeeAditionalIdentifier = "����� ����� 3",
        //        EmployeeIdentifier = "test@tesactivedirectory.com",
        //        EmployeeNames = "������ ���������� ����������",
        //        AdministrationName = "��������� ������� �� ��������",
        //        EmployeePosition = "������ ������� ����� \"�������� �� ������������\"",
        //        ResponsiblePersonIdentifier = "392309324",
        //        ServiceType = "�� ������������ �������"
        //    };
        //    string oldcontext = context.ToString();
        //    // "\"LawReason\":\"�������� �� RegiX\",\"ServiceURI\":\"123456789\",\�ServiceType\�:\��� ������������ �������\�\"Remark\":\"\",\"RequestorNames\":\"Ivelin Ivanov\",\"UserAdministrationName\":\"������������ �� ����������� � �������\",\� UserAdministrationID\�:\�10.20.503.4\�,\" RequestorIdentifier\":\"IIvanov@technologica.com\",\"RequestorIdentifierOptional\":\"123456\",\"UserPosition\":\"Developer\",\"EAuthToken\":\"https://trust-party-openid.com\"";
        //    NapooStudentDocumentsAdapter adapter = new NapooStudentDocumentsAdapter();
        //    var accessMatrix = AccessMatrix.CreateForType(typeof(DocumentsByStudentResponse));
        //    TechnoLogica.RegiX.NapooStudentDocumentsAdapter.StudentDocumentRequestType searchCriteria = new TechnoLogica.RegiX.NapooStudentDocumentsAdapter.StudentDocumentRequestType() { DocumentRegistrationNumber = "658-159", StudentIdentifier = "1234567890" };

        //    AdapterAdditionalParameters additionalParameters = new AdapterAdditionalParameters();
        //    additionalParameters.CallContext = new TechnoLogica.RegiX.Common.TransportObject.CallContext();
        //    additionalParameters.CitizenEGN = "8888888888";
        //    additionalParameters.ClientIPAddress = "111.111.111.111";
        //    additionalParameters.ConsumerCertificateThumbprint = "###";
        //    additionalParameters.EIDToken = "token123";
        //    additionalParameters.EmployeeEGN = "11111111111111";
        //    additionalParameters.OrganizationEIK = "12121212121";
        //    additionalParameters.OrganizationUnit = "qqqqqq";
        //    additionalParameters.SignResult = true;
        //    additionalParameters.ReturnAccessMatrix = true;

        //    var result = adapter.GetStudentDocument(searchCriteria, accessMatrix, additionalParameters);
        //    string xml = result.XmlSerialize();
        //    using (StreamWriter outfile = new StreamWriter("GetStudentDocument_NULL_Test.xml", false, System.Text.Encoding.UTF8))
        //    {
        //        outfile.Write(xml);
        //    }

        //    XsltUtils.RunXsltAndWriteHtml("NapooStudentDocumentsAdapter", "DocumentsByStudentRequest", result.Data.Request.XmlSerialize());
        //    XsltUtils.RunXsltAndWriteHtml("NapooStudentDocumentsAdapter", "DocumentsByStudentResponse", result.Data.Response.XmlSerialize());

        //    XmlDocument doc = new XmlDocument();
        //    doc.LoadXml(xml);
        //    if (SigningUtils.HasSignature(doc))
        //    {
        //        bool isValid = SigningUtils.ValidateXmlDocumentWithX509Certificate(doc);
        //        if (isValid)
        //        {
        //            Assert.IsTrue(true);
        //        }
        //    }
        //    else
        //    {
        //        Assert.IsTrue(true);
        //    }
        //}
    }
}


