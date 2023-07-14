﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RegiXServiceReference
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RegiXServiceReference.IRegiXEntryPoint")]
    public interface IRegiXEntryPoint
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRegiXEntryPoint/Execute", ReplyAction="http://tempuri.org/IRegiXEntryPoint/ExecuteResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(AccessMatrixType))]
        RegiXServiceReference.ServiceExecuteResult Execute(RegiXServiceReference.ServiceRequestData request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRegiXEntryPoint/Execute", ReplyAction="http://tempuri.org/IRegiXEntryPoint/ExecuteResponse")]
        System.Threading.Tasks.Task<RegiXServiceReference.ServiceExecuteResult> ExecuteAsync(RegiXServiceReference.ServiceRequestData request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRegiXEntryPoint/CheckResult", ReplyAction="http://tempuri.org/IRegiXEntryPoint/CheckResultResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(AccessMatrixType))]
        RegiXServiceReference.ServiceResultData CheckResult(RegiXServiceReference.ServiceCheckResultArgument argument);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRegiXEntryPoint/CheckResult", ReplyAction="http://tempuri.org/IRegiXEntryPoint/CheckResultResponse")]
        System.Threading.Tasks.Task<RegiXServiceReference.ServiceResultData> CheckResultAsync(RegiXServiceReference.ServiceCheckResultArgument argument);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRegiXEntryPoint/ExecuteSynchronous", ReplyAction="http://tempuri.org/IRegiXEntryPoint/ExecuteSynchronousResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(AccessMatrixType))]
        RegiXServiceReference.ServiceResultData ExecuteSynchronous(RegiXServiceReference.ServiceRequestData request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRegiXEntryPoint/ExecuteSynchronous", ReplyAction="http://tempuri.org/IRegiXEntryPoint/ExecuteSynchronousResponse")]
        System.Threading.Tasks.Task<RegiXServiceReference.ServiceResultData> ExecuteSynchronousAsync(RegiXServiceReference.ServiceRequestData request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ServiceRequestData
    {
        
        private string operationField;
        
        private System.Xml.XmlElement argumentField;
        
        private string eIDTokenField;
        
        private CallContext callContextField;
        
        private string callbackURLField;
        
        private string employeeEGNField;
        
        private string citizenEGNField;
        
        private bool signResultField;
        
        private bool returnAccessMatrixField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Operation
        {
            get
            {
                return this.operationField;
            }
            set
            {
                this.operationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public System.Xml.XmlElement Argument
        {
            get
            {
                return this.argumentField;
            }
            set
            {
                this.argumentField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=2)]
        public string EIDToken
        {
            get
            {
                return this.eIDTokenField;
            }
            set
            {
                this.eIDTokenField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public CallContext CallContext
        {
            get
            {
                return this.callContextField;
            }
            set
            {
                this.callContextField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=4)]
        public string CallbackURL
        {
            get
            {
                return this.callbackURLField;
            }
            set
            {
                this.callbackURLField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=5)]
        public string EmployeeEGN
        {
            get
            {
                return this.employeeEGNField;
            }
            set
            {
                this.employeeEGNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=6)]
        public string CitizenEGN
        {
            get
            {
                return this.citizenEGNField;
            }
            set
            {
                this.citizenEGNField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public bool SignResult
        {
            get
            {
                return this.signResultField;
            }
            set
            {
                this.signResultField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public bool ReturnAccessMatrix
        {
            get
            {
                return this.returnAccessMatrixField;
            }
            set
            {
                this.returnAccessMatrixField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class CallContext
    {
        
        private string serviceURIField;
        
        private string serviceTypeField;
        
        private string employeeIdentifierField;
        
        private string employeeNamesField;
        
        private string employeeAditionalIdentifierField;
        
        private string employeePositionField;
        
        private string administrationOIdField;
        
        private string administrationNameField;
        
        private string responsiblePersonIdentifierField;
        
        private string lawReasonField;
        
        private string remarkField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string ServiceURI
        {
            get
            {
                return this.serviceURIField;
            }
            set
            {
                this.serviceURIField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string ServiceType
        {
            get
            {
                return this.serviceTypeField;
            }
            set
            {
                this.serviceTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=2)]
        public string EmployeeIdentifier
        {
            get
            {
                return this.employeeIdentifierField;
            }
            set
            {
                this.employeeIdentifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=3)]
        public string EmployeeNames
        {
            get
            {
                return this.employeeNamesField;
            }
            set
            {
                this.employeeNamesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=4)]
        public string EmployeeAditionalIdentifier
        {
            get
            {
                return this.employeeAditionalIdentifierField;
            }
            set
            {
                this.employeeAditionalIdentifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=5)]
        public string EmployeePosition
        {
            get
            {
                return this.employeePositionField;
            }
            set
            {
                this.employeePositionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=6)]
        public string AdministrationOId
        {
            get
            {
                return this.administrationOIdField;
            }
            set
            {
                this.administrationOIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=7)]
        public string AdministrationName
        {
            get
            {
                return this.administrationNameField;
            }
            set
            {
                this.administrationNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=8)]
        public string ResponsiblePersonIdentifier
        {
            get
            {
                return this.responsiblePersonIdentifierField;
            }
            set
            {
                this.responsiblePersonIdentifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=9)]
        public string LawReason
        {
            get
            {
                return this.lawReasonField;
            }
            set
            {
                this.lawReasonField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=10)]
        public string Remark
        {
            get
            {
                return this.remarkField;
            }
            set
            {
                this.remarkField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class AMPropertyType
    {
        
        private bool hasAccessField;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public bool HasAccess
        {
            get
            {
                return this.hasAccessField;
            }
            set
            {
                this.hasAccessField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class AccessMatrixType
    {
        
        private bool hasAccessField;
        
        private string nameField;
        
        private AMPropertyType[] propertiesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public bool HasAccess
        {
            get
            {
                return this.hasAccessField;
            }
            set
            {
                this.hasAccessField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=2)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Property", IsNullable=false)]
        public AMPropertyType[] Properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ResponseContainer
    {
        
        private System.Xml.XmlElement anyField;
        
        private string idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute(Order=0)]
        public System.Xml.XmlElement Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class RequestContainer
    {
        
        private System.Xml.XmlElement anyField;
        
        private string idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute(Order=0)]
        public System.Xml.XmlElement Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class DataContainer
    {
        
        private RequestContainer requestField;
        
        private ResponseContainer responseField;
        
        private DataContainerMatrix matrixField;
        
        private string idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public RequestContainer Request
        {
            get
            {
                return this.requestField;
            }
            set
            {
                this.requestField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public ResponseContainer Response
        {
            get
            {
                return this.responseField;
            }
            set
            {
                this.responseField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=2)]
        public DataContainerMatrix Matrix
        {
            get
            {
                return this.matrixField;
            }
            set
            {
                this.matrixField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/")]
    public partial class DataContainerMatrix : AccessMatrixType
    {
        
        private string idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ServiceResultData
    {
        
        private bool isReadyField;
        
        private DataContainer dataField;
        
        private System.Xml.XmlElement signatureField;
        
        private bool hasErrorField;
        
        private string errorField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public bool IsReady
        {
            get
            {
                return this.isReadyField;
            }
            set
            {
                this.isReadyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=1)]
        public DataContainer Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute(Name="Signature", Namespace="http://www.w3.org/2000/09/xmldsig#", Order=2)]
        public System.Xml.XmlElement Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public bool HasError
        {
            get
            {
                return this.hasErrorField;
            }
            set
            {
                this.hasErrorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=4)]
        public string Error
        {
            get
            {
                return this.errorField;
            }
            set
            {
                this.errorField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ServiceCheckResultArgument
    {
        
        private decimal serviceCallIDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public decimal ServiceCallID
        {
            get
            {
                return this.serviceCallIDField;
            }
            set
            {
                this.serviceCallIDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ServiceExecuteResult
    {
        
        private decimal serviceCallIDField;
        
        private bool hasErrorField;
        
        private string errorField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public decimal ServiceCallID
        {
            get
            {
                return this.serviceCallIDField;
            }
            set
            {
                this.serviceCallIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public bool HasError
        {
            get
            {
                return this.hasErrorField;
            }
            set
            {
                this.hasErrorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=2)]
        public string Error
        {
            get
            {
                return this.errorField;
            }
            set
            {
                this.errorField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public interface IRegiXEntryPointChannel : RegiXServiceReference.IRegiXEntryPoint, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public partial class RegiXEntryPointClient : System.ServiceModel.ClientBase<RegiXServiceReference.IRegiXEntryPoint>, RegiXServiceReference.IRegiXEntryPoint
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public RegiXEntryPointClient(EndpointConfiguration endpointConfiguration) : 
                base(RegiXEntryPointClient.GetBindingForEndpoint(endpointConfiguration), RegiXEntryPointClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public RegiXEntryPointClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(RegiXEntryPointClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public RegiXEntryPointClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(RegiXEntryPointClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public RegiXEntryPointClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public RegiXServiceReference.ServiceExecuteResult Execute(RegiXServiceReference.ServiceRequestData request)
        {
            return base.Channel.Execute(request);
        }
        
        public System.Threading.Tasks.Task<RegiXServiceReference.ServiceExecuteResult> ExecuteAsync(RegiXServiceReference.ServiceRequestData request)
        {
            return base.Channel.ExecuteAsync(request);
        }
        
        public RegiXServiceReference.ServiceResultData CheckResult(RegiXServiceReference.ServiceCheckResultArgument argument)
        {
            return base.Channel.CheckResult(argument);
        }
        
        public System.Threading.Tasks.Task<RegiXServiceReference.ServiceResultData> CheckResultAsync(RegiXServiceReference.ServiceCheckResultArgument argument)
        {
            return base.Channel.CheckResultAsync(argument);
        }
        
        public RegiXServiceReference.ServiceResultData ExecuteSynchronous(RegiXServiceReference.ServiceRequestData request)
        {
            return base.Channel.ExecuteSynchronous(request);
        }
        
        public System.Threading.Tasks.Task<RegiXServiceReference.ServiceResultData> ExecuteSynchronousAsync(RegiXServiceReference.ServiceRequestData request)
        {
            return base.Channel.ExecuteSynchronousAsync(request);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.WSHttpBinding_IRegiXEntryPoint))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpsTransportBindingElement httpsBindingElement = new System.ServiceModel.Channels.HttpsTransportBindingElement();
                httpsBindingElement.AllowCookies = true;
                httpsBindingElement.MaxBufferSize = int.MaxValue;
                httpsBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpsBindingElement);
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IRegiXEntryPoint))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                result.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Certificate;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.WSHttpBinding_IRegiXEntryPoint))
            {
                return new System.ServiceModel.EndpointAddress("https://regix-service-test.egov.bg/RegiX/RegiXEntryPoint.svc");
            }
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IRegiXEntryPoint))
            {
                return new System.ServiceModel.EndpointAddress("https://regix-service-test.egov.bg/RegiX/RegiXEntryPoint.svc/basic");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            WSHttpBinding_IRegiXEntryPoint,
            
            BasicHttpBinding_IRegiXEntryPoint,
        }
    }
}
