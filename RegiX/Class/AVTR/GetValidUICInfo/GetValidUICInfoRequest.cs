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
namespace RegiX.Class.AVTR.GetValidUICInfo.Request {
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR/ValidUICRequest")]
    [System.Xml.Serialization.XmlRootAttribute("ValidUICRequest", Namespace="http://egov.bg/RegiX/AV/TR/ValidUICRequest", IsNullable=false)]
    public partial class ValidUICRequestType {
        
        private string uICField;
        
        /// <remarks/>
        public string UIC {
            get {
                return this.uICField;
            }
            set {
                this.uICField = value;
            }
        }
    }
}
