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
namespace RegiX.Class.GraoNBD.ValidPersonSearch
{
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/GRAO/NBD/ValidPersonResponse")]
    [System.Xml.Serialization.XmlRootAttribute("ValidPersonResponse", Namespace="http://egov.bg/RegiX/GRAO/NBD/ValidPersonResponse", IsNullable=false)]
    public partial class ValidPersonResponseType {
        
        private string firstNameField;
        
        private string surNameField;
        
        private string familyNameField;
        
        private System.DateTime birthDateField;
        
        private bool birthDateFieldSpecified;
        
        private System.DateTime deathDateField;
        
        private bool deathDateFieldSpecified;
        
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
        public string SurName {
            get {
                return this.surNameField;
            }
            set {
                this.surNameField = value;
            }
        }
        
        /// <remarks/>
        public string FamilyName {
            get {
                return this.familyNameField;
            }
            set {
                this.familyNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
        public System.DateTime BirthDate {
            get {
                return this.birthDateField;
            }
            set {
                this.birthDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BirthDateSpecified {
            get {
                return this.birthDateFieldSpecified;
            }
            set {
                this.birthDateFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
        public System.DateTime DeathDate {
            get {
                return this.deathDateField;
            }
            set {
                this.deathDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DeathDateSpecified {
            get {
                return this.deathDateFieldSpecified;
            }
            set {
                this.deathDateFieldSpecified = value;
            }
        }
    }
}
