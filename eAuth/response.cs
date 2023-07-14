using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace eAuth
{
	// using System.Xml.Serialization;
	// XmlSerializer serializer = new XmlSerializer(typeof(Response));
	// using (StringReader reader = new StringReader(xml))
	// {
	//    var test = (Response)serializer.Deserialize(reader);
	// }

	[XmlRoot(ElementName = "Issuer")]
	public class Issuer
	{

		[XmlAttribute(AttributeName = "Format")]
		public string Format { get; set; }

		[XmlAttribute(AttributeName = "saml2")]
		public string Saml2 { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "StatusCode")]
	public class StatusCode
	{

		[XmlAttribute(AttributeName = "Value")]
		public string Value { get; set; }
	}

	[XmlRoot(ElementName = "Status")]
	public class Status
	{

		[XmlElement(ElementName = "StatusCode")]
		public StatusCode StatusCode { get; set; }
	}

	[XmlRoot(ElementName = "CanonicalizationMethod")]
	public class CanonicalizationMethod
	{

		[XmlAttribute(AttributeName = "Algorithm")]
		public string Algorithm { get; set; }
	}

	[XmlRoot(ElementName = "SignatureMethod")]
	public class SignatureMethod
	{

		[XmlAttribute(AttributeName = "Algorithm")]
		public string Algorithm { get; set; }
	}

	[XmlRoot(ElementName = "Transform")]
	public class Transform
	{

		[XmlAttribute(AttributeName = "Algorithm")]
		public string Algorithm { get; set; }

		[XmlElement(ElementName = "InclusiveNamespaces")]
		public InclusiveNamespaces InclusiveNamespaces { get; set; }
	}

	[XmlRoot(ElementName = "InclusiveNamespaces")]
	public class InclusiveNamespaces
	{

		[XmlAttribute(AttributeName = "PrefixList")]
		public string PrefixList { get; set; }

		[XmlAttribute(AttributeName = "ec")]
		public string Ec { get; set; }
	}

	[XmlRoot(ElementName = "Transforms")]
	public class Transforms
	{

		[XmlElement(ElementName = "Transform")]
		public List<Transform> Transform { get; set; }
	}

	[XmlRoot(ElementName = "DigestMethod")]
	public class DigestMethod
	{

		[XmlAttribute(AttributeName = "Algorithm")]
		public string Algorithm { get; set; }
	}

	[XmlRoot(ElementName = "Reference")]
	public class Reference
	{

		[XmlElement(ElementName = "Transforms")]
		public Transforms Transforms { get; set; }

		[XmlElement(ElementName = "DigestMethod")]
		public DigestMethod DigestMethod { get; set; }

		[XmlElement(ElementName = "DigestValue")]
		public string DigestValue { get; set; }

		[XmlAttribute(AttributeName = "URI")]
		public string URI { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "SignedInfo")]
	public class SignedInfo
	{

		[XmlElement(ElementName = "CanonicalizationMethod")]
		public CanonicalizationMethod CanonicalizationMethod { get; set; }

		[XmlElement(ElementName = "SignatureMethod")]
		public SignatureMethod SignatureMethod { get; set; }

		[XmlElement(ElementName = "Reference")]
		public Reference Reference { get; set; }
	}

	[XmlRoot(ElementName = "Signature")]
	public class Signature
	{

		[XmlElement(ElementName = "SignedInfo")]
		public SignedInfo SignedInfo { get; set; }

		[XmlElement(ElementName = "SignatureValue")]
		public string SignatureValue { get; set; }

		[XmlAttribute(AttributeName = "ds")]
		public string Ds { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "NameID")]
	public class NameID
	{

		[XmlAttribute(AttributeName = "Format")]
		public string Format { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "SubjectConfirmationData")]
	public class SubjectConfirmationData
	{

		[XmlAttribute(AttributeName = "InResponseTo")]
		public string InResponseTo { get; set; }

		[XmlAttribute(AttributeName = "NotOnOrAfter")]
		public DateTime NotOnOrAfter { get; set; }

		[XmlAttribute(AttributeName = "Recipient")]
		public string Recipient { get; set; }
	}

	[XmlRoot(ElementName = "SubjectConfirmation")]
	public class SubjectConfirmation
	{

		[XmlElement(ElementName = "SubjectConfirmationData")]
		public SubjectConfirmationData SubjectConfirmationData { get; set; }

		[XmlAttribute(AttributeName = "Method")]
		public string Method { get; set; }
	}

	[XmlRoot(ElementName = "Subject")]
	public class Subject
	{

		[XmlElement(ElementName = "NameID")]
		public NameID NameID { get; set; }

		[XmlElement(ElementName = "SubjectConfirmation")]
		public SubjectConfirmation SubjectConfirmation { get; set; }
	}

	[XmlRoot(ElementName = "AudienceRestriction")]
	public class AudienceRestriction
	{

		[XmlElement(ElementName = "Audience")]
		public string Audience { get; set; }
	}

	[XmlRoot(ElementName = "Conditions")]
	public class Conditions
	{

		[XmlElement(ElementName = "AudienceRestriction")]
		public AudienceRestriction AudienceRestriction { get; set; }

		[XmlAttribute(AttributeName = "NotBefore")]
		public DateTime NotBefore { get; set; }

		[XmlAttribute(AttributeName = "NotOnOrAfter")]
		public DateTime NotOnOrAfter { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "AuthnContext")]
	public class AuthnContext
	{

		[XmlElement(ElementName = "AuthnContextClassRef")]
		public string AuthnContextClassRef { get; set; }

		[XmlElement(ElementName = "AuthenticatingAuthority")]
		public string AuthenticatingAuthority { get; set; }
	}

	[XmlRoot(ElementName = "AuthnStatement")]
	public class AuthnStatement
	{

		[XmlElement(ElementName = "AuthnContext")]
		public AuthnContext AuthnContext { get; set; }

		[XmlAttribute(AttributeName = "AuthnInstant")]
		public DateTime AuthnInstant { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "AttributeValue")]
	public class AttributeValue
	{

		[XmlAttribute(AttributeName = "xsi")]
		public string Xsi { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Attribute")]
	public class Attribute
	{

		[XmlElement(ElementName = "AttributeValue")]
		public AttributeValue AttributeValue { get; set; }

		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "NameFormat")]
		public string NameFormat { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "AttributeStatement")]
	public class AttributeStatement
	{

		[XmlElement(ElementName = "Attribute")]
		public List<Attribute> Attribute { get; set; }
	}

	[XmlRoot(ElementName = "Assertion")]
	public class Assertion
	{

		[XmlElement(ElementName = "Issuer")]
		public Issuer Issuer { get; set; }

		[XmlElement(ElementName = "Signature")]
		public Signature Signature { get; set; }

		[XmlElement(ElementName = "Subject")]
		public Subject Subject { get; set; }

		[XmlElement(ElementName = "Conditions")]
		public Conditions Conditions { get; set; }

		[XmlElement(ElementName = "AuthnStatement")]
		public AuthnStatement AuthnStatement { get; set; }

		[XmlElement(ElementName = "AttributeStatement")]
		public AttributeStatement AttributeStatement { get; set; }

		[XmlAttribute(AttributeName = "ID")]
		public string ID { get; set; }

		[XmlAttribute(AttributeName = "IssueInstant")]
		public DateTime IssueInstant { get; set; }

		[XmlAttribute(AttributeName = "Version")]
		public double Version { get; set; }

		[XmlAttribute(AttributeName = "saml2")]
		public string Saml2 { get; set; }

		[XmlAttribute(AttributeName = "xsd")]
		public string Xsd { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Response")]
	public class Response
	{

		[XmlElement(ElementName = "Issuer")]
		public Issuer Issuer { get; set; }

		[XmlElement(ElementName = "Status")]
		public Status Status { get; set; }

		[XmlElement(ElementName = "Assertion")]
		public Assertion Assertion { get; set; }

		[XmlAttribute(AttributeName = "Destination")]
		public string Destination { get; set; }

		[XmlAttribute(AttributeName = "ID")]
		public string ID { get; set; }

		[XmlAttribute(AttributeName = "InResponseTo")]
		public string InResponseTo { get; set; }

		[XmlAttribute(AttributeName = "IssueInstant")]
		public DateTime IssueInstant { get; set; }

		[XmlAttribute(AttributeName = "Version")]
		public double Version { get; set; }

		[XmlAttribute(AttributeName = "saml2p")]
		public string Saml2p { get; set; }

		[XmlText]
		public string Text { get; set; }
	}





}
