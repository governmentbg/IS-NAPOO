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
namespace RegiX.Class.RDSO.GetDiplomaInfo.Request {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/MOMN/RDSO/DiplomaDocumentsRequest")]
    [System.Xml.Serialization.XmlRootAttribute("DiplomaDocumentsRequest", Namespace="http://egov.bg/RegiX/MOMN/RDSO/DiplomaDocumentsRequest", IsNullable=false)]
    public partial class DiplomaSearchType {
        
        private string studentIDField;
        
        private IdentifierType iDTypeField;
        
        private string documentNumberField;
        
        /// <remarks/>
        public string StudentID {
            get {
                return this.studentIDField;
            }
            set {
                this.studentIDField = value;
            }
        }
        
        /// <remarks/>
        public IdentifierType IDType {
            get {
                return this.iDTypeField;
            }
            set {
                this.iDTypeField = value;
            }
        }
        
        /// <remarks/>
        public string DocumentNumber {
            get {
                return this.documentNumberField;
            }
            set {
                this.documentNumberField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/MOMN/RDSO")]
    public enum IdentifierType {
        
        /// <remarks/>
        EGN,
        
        /// <remarks/>
        LNCh,
        
        /// <remarks/>
        IDN,
    }
}
