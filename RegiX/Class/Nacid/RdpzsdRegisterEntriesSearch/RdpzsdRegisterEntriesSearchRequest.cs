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
namespace RegiX.Class.Nacid.RdpzsdRegisterEntriesSearch {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/NACID/Rdpzsd/RdpzsdRegisterEntriesRequest")]
    [System.Xml.Serialization.XmlRootAttribute("RdpzsdRegisterEntriesRequest", Namespace="http://egov.bg/RegiX/NACID/Rdpzsd/RdpzsdRegisterEntriesRequest", IsNullable=false)]
    public partial class RdpzsdRegisterEntriesRequestType {
        
        private string uanField;
        
        private string uinField;
        
        private string idNumberField;
        
        private string fullNameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string Uan {
            get {
                return this.uanField;
            }
            set {
                this.uanField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string Uin {
            get {
                return this.uinField;
            }
            set {
                this.uinField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string IdNumber {
            get {
                return this.idNumberField;
            }
            set {
                this.idNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string FullName {
            get {
                return this.fullNameField;
            }
            set {
                this.fullNameField = value;
            }
        }
    }
}
