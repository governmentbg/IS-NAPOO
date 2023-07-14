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
namespace RegiX.Class.AVTR.GetActualState {
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR/ActualStateResponse")]
    [System.Xml.Serialization.XmlRootAttribute("ActualStateResponse", Namespace="http://egov.bg/RegiX/AV/TR/ActualStateResponse", IsNullable=false)]
    public partial class ActualStateResponseType {
        
        private StatusType statusField;
        
        private bool statusFieldSpecified;
        
        private string uICField;
        
        private string companyField;
        
        private LegalFormType legalFormField;
        
        private string transliterationField;
        
        private SeatType seatField;
        
        private AddressType seatForCorrespondenceField;
        
        private SubjectOfActivityType subjectOfActivityField;
        
        private NKIDType subjectOfActivityNKIDField;
        
        private string wayOfManagementField;
        
        private string wayOfRepresentationField;
        
        private string termsOfPartnershipField;
        
        private string termOfExistingField;
        
        private string specialConditionsField;
        
        private string hiddenNonMonetaryDepositField;
        
        private string sharePaymentResponsibilityField;
        
        private string concededEstateValueField;
        
        private string cessationOfTradeField;
        
        private string addemptionOfTraderField;
        
        private AddemptionOfTraderType addemptionOfTraderSeatChangeField;
        
        private CapitalAmountType fundsField;
        
        private ShareType[] sharesField;
        
        private CapitalAmountType minimumAmountField;
        
        private CapitalAmountType depositedFundsField;
        
        private NonMonetaryDepositType[] nonMonetaryDepositsField;
        
        private string buyBackDecisionField;
        
        private MandateType boardOfDirectorsMandateField;
        
        private MandateType administrativeBoardMandateField;
        
        private MandateType boardOfManagersMandateField;
        
        private MandateType boardOfManagers2MandateField;
        
        private MandateType leadingBoardMandateField;
        
        private MandateType supervisingBoardMandateField;
        
        private MandateType supervisingBoard2MandateField;
        
        private MandateType controllingBoardMandateField;
        
        private DetailType[] detailsField;
        
        private System.DateTime dataValidForDateField;
        
        private bool dataValidForDateFieldSpecified;
        
        private LiquidationOrInsolvency liquidationOrInsolvencyField;
        
        private bool liquidationOrInsolvencyFieldSpecified;
        
        /// <remarks/>
        public StatusType Status {
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
        public string UIC {
            get {
                return this.uICField;
            }
            set {
                this.uICField = value;
            }
        }
        
        /// <remarks/>
        public string Company {
            get {
                return this.companyField;
            }
            set {
                this.companyField = value;
            }
        }
        
        /// <remarks/>
        public LegalFormType LegalForm {
            get {
                return this.legalFormField;
            }
            set {
                this.legalFormField = value;
            }
        }
        
        /// <remarks/>
        public string Transliteration {
            get {
                return this.transliterationField;
            }
            set {
                this.transliterationField = value;
            }
        }
        
        /// <remarks/>
        public SeatType Seat {
            get {
                return this.seatField;
            }
            set {
                this.seatField = value;
            }
        }
        
        /// <remarks/>
        public AddressType SeatForCorrespondence {
            get {
                return this.seatForCorrespondenceField;
            }
            set {
                this.seatForCorrespondenceField = value;
            }
        }
        
        /// <remarks/>
        public SubjectOfActivityType SubjectOfActivity {
            get {
                return this.subjectOfActivityField;
            }
            set {
                this.subjectOfActivityField = value;
            }
        }
        
        /// <remarks/>
        public NKIDType SubjectOfActivityNKID {
            get {
                return this.subjectOfActivityNKIDField;
            }
            set {
                this.subjectOfActivityNKIDField = value;
            }
        }
        
        /// <remarks/>
        public string WayOfManagement {
            get {
                return this.wayOfManagementField;
            }
            set {
                this.wayOfManagementField = value;
            }
        }
        
        /// <remarks/>
        public string WayOfRepresentation {
            get {
                return this.wayOfRepresentationField;
            }
            set {
                this.wayOfRepresentationField = value;
            }
        }
        
        /// <remarks/>
        public string TermsOfPartnership {
            get {
                return this.termsOfPartnershipField;
            }
            set {
                this.termsOfPartnershipField = value;
            }
        }
        
        /// <remarks/>
        public string TermOfExisting {
            get {
                return this.termOfExistingField;
            }
            set {
                this.termOfExistingField = value;
            }
        }
        
        /// <remarks/>
        public string SpecialConditions {
            get {
                return this.specialConditionsField;
            }
            set {
                this.specialConditionsField = value;
            }
        }
        
        /// <remarks/>
        public string HiddenNonMonetaryDeposit {
            get {
                return this.hiddenNonMonetaryDepositField;
            }
            set {
                this.hiddenNonMonetaryDepositField = value;
            }
        }
        
        /// <remarks/>
        public string SharePaymentResponsibility {
            get {
                return this.sharePaymentResponsibilityField;
            }
            set {
                this.sharePaymentResponsibilityField = value;
            }
        }
        
        /// <remarks/>
        public string ConcededEstateValue {
            get {
                return this.concededEstateValueField;
            }
            set {
                this.concededEstateValueField = value;
            }
        }
        
        /// <remarks/>
        public string CessationOfTrade {
            get {
                return this.cessationOfTradeField;
            }
            set {
                this.cessationOfTradeField = value;
            }
        }
        
        /// <remarks/>
        public string AddemptionOfTrader {
            get {
                return this.addemptionOfTraderField;
            }
            set {
                this.addemptionOfTraderField = value;
            }
        }
        
        /// <remarks/>
        public AddemptionOfTraderType AddemptionOfTraderSeatChange {
            get {
                return this.addemptionOfTraderSeatChangeField;
            }
            set {
                this.addemptionOfTraderSeatChangeField = value;
            }
        }
        
        /// <remarks/>
        public CapitalAmountType Funds {
            get {
                return this.fundsField;
            }
            set {
                this.fundsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Share", Namespace="http://egov.bg/RegiX/AV/TR", IsNullable=false)]
        public ShareType[] Shares {
            get {
                return this.sharesField;
            }
            set {
                this.sharesField = value;
            }
        }
        
        /// <remarks/>
        public CapitalAmountType MinimumAmount {
            get {
                return this.minimumAmountField;
            }
            set {
                this.minimumAmountField = value;
            }
        }
        
        /// <remarks/>
        public CapitalAmountType DepositedFunds {
            get {
                return this.depositedFundsField;
            }
            set {
                this.depositedFundsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("NonMonetaryDeposit", Namespace="http://egov.bg/RegiX/AV/TR", IsNullable=false)]
        public NonMonetaryDepositType[] NonMonetaryDeposits {
            get {
                return this.nonMonetaryDepositsField;
            }
            set {
                this.nonMonetaryDepositsField = value;
            }
        }
        
        /// <remarks/>
        public string BuyBackDecision {
            get {
                return this.buyBackDecisionField;
            }
            set {
                this.buyBackDecisionField = value;
            }
        }
        
        /// <remarks/>
        public MandateType BoardOfDirectorsMandate {
            get {
                return this.boardOfDirectorsMandateField;
            }
            set {
                this.boardOfDirectorsMandateField = value;
            }
        }
        
        /// <remarks/>
        public MandateType AdministrativeBoardMandate {
            get {
                return this.administrativeBoardMandateField;
            }
            set {
                this.administrativeBoardMandateField = value;
            }
        }
        
        /// <remarks/>
        public MandateType BoardOfManagersMandate {
            get {
                return this.boardOfManagersMandateField;
            }
            set {
                this.boardOfManagersMandateField = value;
            }
        }
        
        /// <remarks/>
        public MandateType BoardOfManagers2Mandate {
            get {
                return this.boardOfManagers2MandateField;
            }
            set {
                this.boardOfManagers2MandateField = value;
            }
        }
        
        /// <remarks/>
        public MandateType LeadingBoardMandate {
            get {
                return this.leadingBoardMandateField;
            }
            set {
                this.leadingBoardMandateField = value;
            }
        }
        
        /// <remarks/>
        public MandateType SupervisingBoardMandate {
            get {
                return this.supervisingBoardMandateField;
            }
            set {
                this.supervisingBoardMandateField = value;
            }
        }
        
        /// <remarks/>
        public MandateType SupervisingBoard2Mandate {
            get {
                return this.supervisingBoard2MandateField;
            }
            set {
                this.supervisingBoard2MandateField = value;
            }
        }
        
        /// <remarks/>
        public MandateType ControllingBoardMandate {
            get {
                return this.controllingBoardMandateField;
            }
            set {
                this.controllingBoardMandateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Detail", Namespace="http://egov.bg/RegiX/AV/TR", IsNullable=false)]
        public DetailType[] Details {
            get {
                return this.detailsField;
            }
            set {
                this.detailsField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime DataValidForDate {
            get {
                return this.dataValidForDateField;
            }
            set {
                this.dataValidForDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DataValidForDateSpecified {
            get {
                return this.dataValidForDateFieldSpecified;
            }
            set {
                this.dataValidForDateFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public LiquidationOrInsolvency LiquidationOrInsolvency {
            get {
                return this.liquidationOrInsolvencyField;
            }
            set {
                this.liquidationOrInsolvencyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LiquidationOrInsolvencySpecified {
            get {
                return this.liquidationOrInsolvencyFieldSpecified;
            }
            set {
                this.liquidationOrInsolvencyFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class LegalFormType {
        
        private string legalFormAbbrField;
        
        private string legalFormNameField;
        
        /// <remarks/>
        public string LegalFormAbbr {
            get {
                return this.legalFormAbbrField;
            }
            set {
                this.legalFormAbbrField = value;
            }
        }
        
        /// <remarks/>
        public string LegalFormName {
            get {
                return this.legalFormNameField;
            }
            set {
                this.legalFormNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class SubjectType {
        
        private string indentField;
        
        private string nameField;
        
        private IndentTypeType indentTypeField;
        
        private bool indentTypeFieldSpecified;
        
        /// <remarks/>
        public string Indent {
            get {
                return this.indentField;
            }
            set {
                this.indentField = value;
            }
        }
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public IndentTypeType IndentType {
            get {
                return this.indentTypeField;
            }
            set {
                this.indentTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IndentTypeSpecified {
            get {
                return this.indentTypeFieldSpecified;
            }
            set {
                this.indentTypeFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public enum IndentTypeType {
        
        /// <remarks/>
        EGN,
        
        /// <remarks/>
        UIC,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class DetailType {
        
        private string fieldNameField;
        
        private string fieldCodeField;
        
        private string fieldOrderField;
        
        private SubjectType subjectField;
        
        /// <remarks/>
        public string FieldName {
            get {
                return this.fieldNameField;
            }
            set {
                this.fieldNameField = value;
            }
        }
        
        /// <remarks/>
        public string FieldCode {
            get {
                return this.fieldCodeField;
            }
            set {
                this.fieldCodeField = value;
            }
        }
        
        /// <remarks/>
        public string FieldOrder {
            get {
                return this.fieldOrderField;
            }
            set {
                this.fieldOrderField = value;
            }
        }
        
        /// <remarks/>
        public SubjectType Subject {
            get {
                return this.subjectField;
            }
            set {
                this.subjectField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class MandateType {
        
        private string typeField;
        
        private string mandateValueField;
        
        /// <remarks/>
        public string Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        public string MandateValue {
            get {
                return this.mandateValueField;
            }
            set {
                this.mandateValueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class NonMonetaryDepositType {
        
        private string descriptionField;
        
        private string valueField;
        
        /// <remarks/>
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class ShareType {
        
        private string typeField;
        
        private string countField;
        
        private string nominalValueField;
        
        /// <remarks/>
        public string Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        public string Count {
            get {
                return this.countField;
            }
            set {
                this.countField = value;
            }
        }
        
        /// <remarks/>
        public string NominalValue {
            get {
                return this.nominalValueField;
            }
            set {
                this.nominalValueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class CapitalAmountType {
        
        private string valueField;
        
        private string euroField;
        
        /// <remarks/>
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        public string Euro {
            get {
                return this.euroField;
            }
            set {
                this.euroField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class AddemptionOfTraderType {
        
        private AddressType addressField;
        
        private ContactsType contactsField;
        
        private string competentAuthorityForRegistrationField;
        
        private string registrationNumberField;
        
        /// <remarks/>
        public AddressType Address {
            get {
                return this.addressField;
            }
            set {
                this.addressField = value;
            }
        }
        
        /// <remarks/>
        public ContactsType Contacts {
            get {
                return this.contactsField;
            }
            set {
                this.contactsField = value;
            }
        }
        
        /// <remarks/>
        public string CompetentAuthorityForRegistration {
            get {
                return this.competentAuthorityForRegistrationField;
            }
            set {
                this.competentAuthorityForRegistrationField = value;
            }
        }
        
        /// <remarks/>
        public string RegistrationNumber {
            get {
                return this.registrationNumberField;
            }
            set {
                this.registrationNumberField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class AddressType {
        
        private string countryCodeField;
        
        private string countryField;
        
        private string isForeignField;
        
        private string districtEkatteField;
        
        private string districtField;
        
        private string municipalityEkatteField;
        
        private string municipalityField;
        
        private string settlementEKATTEField;
        
        private string settlementField;
        
        private string areaField;
        
        private string areaEkatteField;
        
        private string postCodeField;
        
        private string foreignPlaceField;
        
        private string housingEstateField;
        
        private string streetField;
        
        private string streetNumberField;
        
        private string blockField;
        
        private string entranceField;
        
        private string floorField;
        
        private string apartmentField;
        
        /// <remarks/>
        public string CountryCode {
            get {
                return this.countryCodeField;
            }
            set {
                this.countryCodeField = value;
            }
        }
        
        /// <remarks/>
        public string Country {
            get {
                return this.countryField;
            }
            set {
                this.countryField = value;
            }
        }
        
        /// <remarks/>
        public string IsForeign {
            get {
                return this.isForeignField;
            }
            set {
                this.isForeignField = value;
            }
        }
        
        /// <remarks/>
        public string DistrictEkatte {
            get {
                return this.districtEkatteField;
            }
            set {
                this.districtEkatteField = value;
            }
        }
        
        /// <remarks/>
        public string District {
            get {
                return this.districtField;
            }
            set {
                this.districtField = value;
            }
        }
        
        /// <remarks/>
        public string MunicipalityEkatte {
            get {
                return this.municipalityEkatteField;
            }
            set {
                this.municipalityEkatteField = value;
            }
        }
        
        /// <remarks/>
        public string Municipality {
            get {
                return this.municipalityField;
            }
            set {
                this.municipalityField = value;
            }
        }
        
        /// <remarks/>
        public string SettlementEKATTE {
            get {
                return this.settlementEKATTEField;
            }
            set {
                this.settlementEKATTEField = value;
            }
        }
        
        /// <remarks/>
        public string Settlement {
            get {
                return this.settlementField;
            }
            set {
                this.settlementField = value;
            }
        }
        
        /// <remarks/>
        public string Area {
            get {
                return this.areaField;
            }
            set {
                this.areaField = value;
            }
        }
        
        /// <remarks/>
        public string AreaEkatte {
            get {
                return this.areaEkatteField;
            }
            set {
                this.areaEkatteField = value;
            }
        }
        
        /// <remarks/>
        public string PostCode {
            get {
                return this.postCodeField;
            }
            set {
                this.postCodeField = value;
            }
        }
        
        /// <remarks/>
        public string ForeignPlace {
            get {
                return this.foreignPlaceField;
            }
            set {
                this.foreignPlaceField = value;
            }
        }
        
        /// <remarks/>
        public string HousingEstate {
            get {
                return this.housingEstateField;
            }
            set {
                this.housingEstateField = value;
            }
        }
        
        /// <remarks/>
        public string Street {
            get {
                return this.streetField;
            }
            set {
                this.streetField = value;
            }
        }
        
        /// <remarks/>
        public string StreetNumber {
            get {
                return this.streetNumberField;
            }
            set {
                this.streetNumberField = value;
            }
        }
        
        /// <remarks/>
        public string Block {
            get {
                return this.blockField;
            }
            set {
                this.blockField = value;
            }
        }
        
        /// <remarks/>
        public string Entrance {
            get {
                return this.entranceField;
            }
            set {
                this.entranceField = value;
            }
        }
        
        /// <remarks/>
        public string Floor {
            get {
                return this.floorField;
            }
            set {
                this.floorField = value;
            }
        }
        
        /// <remarks/>
        public string Apartment {
            get {
                return this.apartmentField;
            }
            set {
                this.apartmentField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class ContactsType {
        
        private string phoneField;
        
        private string faxField;
        
        private string eMailField;
        
        private string uRLField;
        
        /// <remarks/>
        public string Phone {
            get {
                return this.phoneField;
            }
            set {
                this.phoneField = value;
            }
        }
        
        /// <remarks/>
        public string Fax {
            get {
                return this.faxField;
            }
            set {
                this.faxField = value;
            }
        }
        
        /// <remarks/>
        public string EMail {
            get {
                return this.eMailField;
            }
            set {
                this.eMailField = value;
            }
        }
        
        /// <remarks/>
        public string URL {
            get {
                return this.uRLField;
            }
            set {
                this.uRLField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class NKIDType {
        
        private string nKIDcodeField;
        
        private string nKIDnameField;
        
        /// <remarks/>
        public string NKIDcode {
            get {
                return this.nKIDcodeField;
            }
            set {
                this.nKIDcodeField = value;
            }
        }
        
        /// <remarks/>
        public string NKIDname {
            get {
                return this.nKIDnameField;
            }
            set {
                this.nKIDnameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class SubjectOfActivityType {
        
        private string subjectField;
        
        private bool isBankField;
        
        private bool isBankFieldSpecified;
        
        private bool isInsurerField;
        
        private bool isInsurerFieldSpecified;
        
        /// <remarks/>
        public string Subject {
            get {
                return this.subjectField;
            }
            set {
                this.subjectField = value;
            }
        }
        
        /// <remarks/>
        public bool IsBank {
            get {
                return this.isBankField;
            }
            set {
                this.isBankField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsBankSpecified {
            get {
                return this.isBankFieldSpecified;
            }
            set {
                this.isBankFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public bool IsInsurer {
            get {
                return this.isInsurerField;
            }
            set {
                this.isInsurerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsInsurerSpecified {
            get {
                return this.isInsurerFieldSpecified;
            }
            set {
                this.isInsurerFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR")]
    public partial class SeatType {
        
        private AddressType addressField;
        
        private ContactsType contactsField;
        
        /// <remarks/>
        public AddressType Address {
            get {
                return this.addressField;
            }
            set {
                this.addressField = value;
            }
        }
        
        /// <remarks/>
        public ContactsType Contacts {
            get {
                return this.contactsField;
            }
            set {
                this.contactsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://egov.bg/RegiX/AV/TR/ActualStateResponse")]
    public enum LiquidationOrInsolvency {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("В ликвидация")]
        Вликвидация,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("В несъстоятелност")]
        Внесъстоятелност,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("В несъстоятелност (на II инстанция)")]
        ВнесъстоятелностнаIIинстанция,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("В несъстоятелност (на III инстанция)")]
        ВнесъстоятелностнаIIIинстанция,
    }
}
