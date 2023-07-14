﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace RegiX.Class.NapooStudentDocuments.GetDocumentsByStudent
{
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://egov.bg/RegiX/NAPOO/DocumentsByStudentResponse")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://egov.bg/RegiX/NAPOO/DocumentsByStudentResponse", IsNullable=false)]
    public partial class DocumentsByStudentResponse {
        
        private StudentData[] studentDocumentField;
        
        private bool statusField;
        
        private bool statusFieldSpecified;
        
        private string messageField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StudentDocument")]
        public StudentData[] StudentDocument {
            get {
                return this.studentDocumentField;
            }
            set {
                this.studentDocumentField = value;
            }
        }
        
        /// <remarks/>
        public bool Status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool StatusSpecified {
            get {
                return this.statusFieldSpecified;
            }
            set {
                this.statusFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/NAPOO")]
    public partial class StudentData {
        
        private int clientIDField;
        
        private bool clientIDFieldSpecified;
        
        private string studentIdentifierField;
        
        private string firstNameField;
        
        private string middleNameField;
        
        private string lastNameField;
        
        private int licenceNumberField;
        
        private bool licenceNumberFieldSpecified;
        
        private string professionalEduCenterField;
        
        private string professionalEduCenterLocationField;
        
        private int documentTypeIDField;
        
        private bool documentTypeIDFieldSpecified;
        
        private string documentTypeField;
        
        private int educationTypeIDField;
        
        private bool educationTypeIDFieldSpecified;
        
        private string educationTypeField;
        
        private string professionCodeAndNameField;
        
        private string subjectCodeAndNameField;
        
        private int qualificationDegreeField;
        
        private bool qualificationDegreeFieldSpecified;
        
        private int graduationYearField;
        
        private bool graduationYearFieldSpecified;
        
        private string documentSeriesField;
        
        private string documentSerialNumberField;
        
        private string documentRegistrationNumberField;
        
        private System.DateTime documentIssueDateField;
        
        private bool documentIssueDateFieldSpecified;
        
        /// <remarks/>
        public int ClientID {
            get {
                return this.clientIDField;
            }
            set {
                this.clientIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ClientIDSpecified {
            get {
                return this.clientIDFieldSpecified;
            }
            set {
                this.clientIDFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string StudentIdentifier {
            get {
                return this.studentIdentifierField;
            }
            set {
                this.studentIdentifierField = value;
            }
        }
        
        /// <remarks/>
        public string FirstName {
            get {
                return this.firstNameField;
            }
            set {
                this.firstNameField = value;
            }
        }
        
        /// <remarks/>
        public string MiddleName {
            get {
                return this.middleNameField;
            }
            set {
                this.middleNameField = value;
            }
        }
        
        /// <remarks/>
        public string LastName {
            get {
                return this.lastNameField;
            }
            set {
                this.lastNameField = value;
            }
        }
        
        /// <remarks/>
        public int LicenceNumber {
            get {
                return this.licenceNumberField;
            }
            set {
                this.licenceNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LicenceNumberSpecified {
            get {
                return this.licenceNumberFieldSpecified;
            }
            set {
                this.licenceNumberFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string ProfessionalEduCenter {
            get {
                return this.professionalEduCenterField;
            }
            set {
                this.professionalEduCenterField = value;
            }
        }
        
        /// <remarks/>
        public string ProfessionalEduCenterLocation {
            get {
                return this.professionalEduCenterLocationField;
            }
            set {
                this.professionalEduCenterLocationField = value;
            }
        }
        
        /// <remarks/>
        public int DocumentTypeID {
            get {
                return this.documentTypeIDField;
            }
            set {
                this.documentTypeIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocumentTypeIDSpecified {
            get {
                return this.documentTypeIDFieldSpecified;
            }
            set {
                this.documentTypeIDFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string DocumentType {
            get {
                return this.documentTypeField;
            }
            set {
                this.documentTypeField = value;
            }
        }
        
        /// <remarks/>
        public int EducationTypeID {
            get {
                return this.educationTypeIDField;
            }
            set {
                this.educationTypeIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EducationTypeIDSpecified {
            get {
                return this.educationTypeIDFieldSpecified;
            }
            set {
                this.educationTypeIDFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string EducationType {
            get {
                return this.educationTypeField;
            }
            set {
                this.educationTypeField = value;
            }
        }
        
        /// <remarks/>
        public string ProfessionCodeAndName {
            get {
                return this.professionCodeAndNameField;
            }
            set {
                this.professionCodeAndNameField = value;
            }
        }
        
        /// <remarks/>
        public string SubjectCodeAndName {
            get {
                return this.subjectCodeAndNameField;
            }
            set {
                this.subjectCodeAndNameField = value;
            }
        }
        
        /// <remarks/>
        public int QualificationDegree {
            get {
                return this.qualificationDegreeField;
            }
            set {
                this.qualificationDegreeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QualificationDegreeSpecified {
            get {
                return this.qualificationDegreeFieldSpecified;
            }
            set {
                this.qualificationDegreeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public int GraduationYear {
            get {
                return this.graduationYearField;
            }
            set {
                this.graduationYearField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GraduationYearSpecified {
            get {
                return this.graduationYearFieldSpecified;
            }
            set {
                this.graduationYearFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public string DocumentSeries {
            get {
                return this.documentSeriesField;
            }
            set {
                this.documentSeriesField = value;
            }
        }
        
        /// <remarks/>
        public string DocumentSerialNumber {
            get {
                return this.documentSerialNumberField;
            }
            set {
                this.documentSerialNumberField = value;
            }
        }
        
        /// <remarks/>
        public string DocumentRegistrationNumber {
            get {
                return this.documentRegistrationNumberField;
            }
            set {
                this.documentRegistrationNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
        public System.DateTime DocumentIssueDate {
            get {
                return this.documentIssueDateField;
            }
            set {
                this.documentIssueDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DocumentIssueDateSpecified {
            get {
                return this.documentIssueDateFieldSpecified;
            }
            set {
                this.documentIssueDateFieldSpecified = value;
            }
        }
    }
}