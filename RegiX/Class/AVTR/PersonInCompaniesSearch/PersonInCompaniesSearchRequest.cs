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
namespace RegiX.Class.AVTR.PersonInCompaniesSearch.Request {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    [System.Xml.Serialization.XmlRootAttribute("TestStatusType", Namespace="http://egov.bg/RegiX/AV/TR", IsNullable=false)]
    public enum StatusType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Нова партида")]
        Новапартида,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Пререгистрирана партида")]
        Пререгистриранапартида,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Нова закрита партида")]
        Новазакритапартида,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Пререгистрирана закрита партида")]
        Пререгистрираназакритапартида,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR/SearchParticipationInCompaniesRequest")]
    [System.Xml.Serialization.XmlRootAttribute("SearchParticipationInCompaniesRequest", Namespace="http://egov.bg/RegiX/AV/TR/SearchParticipationInCompaniesRequest", IsNullable=false)]
    public partial class SearchParticipationInCompaniesRequestType {
        
        private string eGNField;
        
        /// <remarks/>
        public string EGN {
            get {
                return this.eGNField;
            }
            set {
                this.eGNField = value;
            }
        }
    }
}
