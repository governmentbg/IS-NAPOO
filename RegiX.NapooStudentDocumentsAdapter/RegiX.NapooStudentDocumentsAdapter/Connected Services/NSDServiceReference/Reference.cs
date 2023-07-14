﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NSDServiceReference
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="StudentDocumentResponseType", Namespace="http://is.navet.government.bg/ws/")]
    public partial class StudentDocumentResponseType : object
    {
        
        private bool statusField;
        
        private string messageField;
        
        private NSDServiceReference.StudentDocument[] dataField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public bool status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public NSDServiceReference.StudentDocument[] data
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
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="StudentDocument", Namespace="http://is.navet.government.bg/ws/")]
    public partial class StudentDocument : object
    {
        
        private int client_idField;
        
        private string vc_egnField;
        
        private string first_nameField;
        
        private string second_nameField;
        
        private string family_nameField;
        
        private int licence_numberField;
        
        private string provider_ownerField;
        
        private string city_nameField;
        
        private int document_type_idField;
        
        private string document_type_nameField;
        
        private int course_type_idField;
        
        private string course_type_nameField;
        
        private string profession_nameField;
        
        private string speciality_nameField;
        
        private int speciality_vqsField;
        
        private int year_finishedField;
        
        private string document_prn_serField;
        
        private string document_prn_noField;
        
        private string document_reg_noField;
        
        private System.Nullable<System.DateTime> document_issue_dateField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int client_id
        {
            get
            {
                return this.client_idField;
            }
            set
            {
                this.client_idField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string vc_egn
        {
            get
            {
                return this.vc_egnField;
            }
            set
            {
                this.vc_egnField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string first_name
        {
            get
            {
                return this.first_nameField;
            }
            set
            {
                this.first_nameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string second_name
        {
            get
            {
                return this.second_nameField;
            }
            set
            {
                this.second_nameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string family_name
        {
            get
            {
                return this.family_nameField;
            }
            set
            {
                this.family_nameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=5)]
        public int licence_number
        {
            get
            {
                return this.licence_numberField;
            }
            set
            {
                this.licence_numberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string provider_owner
        {
            get
            {
                return this.provider_ownerField;
            }
            set
            {
                this.provider_ownerField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string city_name
        {
            get
            {
                return this.city_nameField;
            }
            set
            {
                this.city_nameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=8)]
        public int document_type_id
        {
            get
            {
                return this.document_type_idField;
            }
            set
            {
                this.document_type_idField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=9)]
        public string document_type_name
        {
            get
            {
                return this.document_type_nameField;
            }
            set
            {
                this.document_type_nameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=10)]
        public int course_type_id
        {
            get
            {
                return this.course_type_idField;
            }
            set
            {
                this.course_type_idField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=11)]
        public string course_type_name
        {
            get
            {
                return this.course_type_nameField;
            }
            set
            {
                this.course_type_nameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=12)]
        public string profession_name
        {
            get
            {
                return this.profession_nameField;
            }
            set
            {
                this.profession_nameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=13)]
        public string speciality_name
        {
            get
            {
                return this.speciality_nameField;
            }
            set
            {
                this.speciality_nameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=14)]
        public int speciality_vqs
        {
            get
            {
                return this.speciality_vqsField;
            }
            set
            {
                this.speciality_vqsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=15)]
        public int year_finished
        {
            get
            {
                return this.year_finishedField;
            }
            set
            {
                this.year_finishedField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=16)]
        public string document_prn_ser
        {
            get
            {
                return this.document_prn_serField;
            }
            set
            {
                this.document_prn_serField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=17)]
        public string document_prn_no
        {
            get
            {
                return this.document_prn_noField;
            }
            set
            {
                this.document_prn_noField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=18)]
        public string document_reg_no
        {
            get
            {
                return this.document_reg_noField;
            }
            set
            {
                this.document_reg_noField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=19)]
        public System.Nullable<System.DateTime> document_issue_date
        {
            get
            {
                return this.document_issue_dateField;
            }
            set
            {
                this.document_issue_dateField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DocumentsByStudentResponseType", Namespace="http://is.navet.government.bg/ws/")]
    public partial class DocumentsByStudentResponseType : object
    {
        
        private bool statusField;
        
        private string messageField;
        
        private NSDServiceReference.StudentDocument[] dataField;
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public bool status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public NSDServiceReference.StudentDocument[] data
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
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://is.navet.government.bg/ws/", ConfigurationName="NSDServiceReference.IData")]
    public interface IData
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:IEGOVService#egovSearchStudentDocument", ReplyAction="*")]
        System.Threading.Tasks.Task<NSDServiceReference.egovSearchStudentDocumentResponse> egovSearchStudentDocumentAsync(NSDServiceReference.egovSearchStudentDocumentRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:IEGOVService#egovSearchStudentDocumentByStudent", ReplyAction="*")]
        System.Threading.Tasks.Task<NSDServiceReference.egovSearchStudentDocumentByStudentResponse> egovSearchStudentDocumentByStudentAsync(NSDServiceReference.egovSearchStudentDocumentByStudentRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class egovSearchStudentDocumentRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="egovSearchStudentDocument", Namespace="http://is.navet.government.bg/ws/", Order=0)]
        public NSDServiceReference.egovSearchStudentDocumentRequestBody Body;
        
        public egovSearchStudentDocumentRequest()
        {
        }
        
        public egovSearchStudentDocumentRequest(NSDServiceReference.egovSearchStudentDocumentRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://is.navet.government.bg/ws/")]
    public partial class egovSearchStudentDocumentRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string identifier;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string document_no;
        
        public egovSearchStudentDocumentRequestBody()
        {
        }
        
        public egovSearchStudentDocumentRequestBody(string identifier, string document_no)
        {
            this.identifier = identifier;
            this.document_no = document_no;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class egovSearchStudentDocumentResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="egovSearchStudentDocumentResponse", Namespace="http://is.navet.government.bg/ws/", Order=0)]
        public NSDServiceReference.egovSearchStudentDocumentResponseBody Body;
        
        public egovSearchStudentDocumentResponse()
        {
        }
        
        public egovSearchStudentDocumentResponse(NSDServiceReference.egovSearchStudentDocumentResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://is.navet.government.bg/ws/")]
    public partial class egovSearchStudentDocumentResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public NSDServiceReference.StudentDocumentResponseType egovSearchStudentDocumentResult;
        
        public egovSearchStudentDocumentResponseBody()
        {
        }
        
        public egovSearchStudentDocumentResponseBody(NSDServiceReference.StudentDocumentResponseType egovSearchStudentDocumentResult)
        {
            this.egovSearchStudentDocumentResult = egovSearchStudentDocumentResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class egovSearchStudentDocumentByStudentRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="egovSearchStudentDocumentByStudent", Namespace="http://is.navet.government.bg/ws/", Order=0)]
        public NSDServiceReference.egovSearchStudentDocumentByStudentRequestBody Body;
        
        public egovSearchStudentDocumentByStudentRequest()
        {
        }
        
        public egovSearchStudentDocumentByStudentRequest(NSDServiceReference.egovSearchStudentDocumentByStudentRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://is.navet.government.bg/ws/")]
    public partial class egovSearchStudentDocumentByStudentRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string identifier;
        
        public egovSearchStudentDocumentByStudentRequestBody()
        {
        }
        
        public egovSearchStudentDocumentByStudentRequestBody(string identifier)
        {
            this.identifier = identifier;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class egovSearchStudentDocumentByStudentResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="egovSearchStudentDocumentByStudentResponse", Namespace="http://is.navet.government.bg/ws/", Order=0)]
        public NSDServiceReference.egovSearchStudentDocumentByStudentResponseBody Body;
        
        public egovSearchStudentDocumentByStudentResponse()
        {
        }
        
        public egovSearchStudentDocumentByStudentResponse(NSDServiceReference.egovSearchStudentDocumentByStudentResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://is.navet.government.bg/ws/")]
    public partial class egovSearchStudentDocumentByStudentResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public NSDServiceReference.DocumentsByStudentResponseType egovSearchStudentDocumentByStudentResult;
        
        public egovSearchStudentDocumentByStudentResponseBody()
        {
        }
        
        public egovSearchStudentDocumentByStudentResponseBody(NSDServiceReference.DocumentsByStudentResponseType egovSearchStudentDocumentByStudentResult)
        {
            this.egovSearchStudentDocumentByStudentResult = egovSearchStudentDocumentByStudentResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    public interface IDataChannel : NSDServiceReference.IData, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.1.0")]
    public partial class DataClient : System.ServiceModel.ClientBase<NSDServiceReference.IData>, NSDServiceReference.IData
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public DataClient() : 
                base(DataClient.GetDefaultBinding(), DataClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IData_soap.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DataClient(EndpointConfiguration endpointConfiguration) : 
                base(DataClient.GetBindingForEndpoint(endpointConfiguration), DataClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DataClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(DataClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DataClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(DataClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public DataClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<NSDServiceReference.egovSearchStudentDocumentResponse> NSDServiceReference.IData.egovSearchStudentDocumentAsync(NSDServiceReference.egovSearchStudentDocumentRequest request)
        {
            return base.Channel.egovSearchStudentDocumentAsync(request);
        }
        
        public System.Threading.Tasks.Task<NSDServiceReference.egovSearchStudentDocumentResponse> egovSearchStudentDocumentAsync(string identifier, string document_no)
        {
            NSDServiceReference.egovSearchStudentDocumentRequest inValue = new NSDServiceReference.egovSearchStudentDocumentRequest();
            inValue.Body = new NSDServiceReference.egovSearchStudentDocumentRequestBody();
            inValue.Body.identifier = identifier;
            inValue.Body.document_no = document_no;
            return ((NSDServiceReference.IData)(this)).egovSearchStudentDocumentAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<NSDServiceReference.egovSearchStudentDocumentByStudentResponse> NSDServiceReference.IData.egovSearchStudentDocumentByStudentAsync(NSDServiceReference.egovSearchStudentDocumentByStudentRequest request)
        {
            return base.Channel.egovSearchStudentDocumentByStudentAsync(request);
        }
        
        public System.Threading.Tasks.Task<NSDServiceReference.egovSearchStudentDocumentByStudentResponse> egovSearchStudentDocumentByStudentAsync(string identifier)
        {
            NSDServiceReference.egovSearchStudentDocumentByStudentRequest inValue = new NSDServiceReference.egovSearchStudentDocumentByStudentRequest();
            inValue.Body = new NSDServiceReference.egovSearchStudentDocumentByStudentRequestBody();
            inValue.Body.identifier = identifier;
            return ((NSDServiceReference.IData)(this)).egovSearchStudentDocumentByStudentAsync(inValue);
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
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IData_soap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IData_soap))
            {
                return new System.ServiceModel.EndpointAddress("https://localhost:7207/EGOVService.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return DataClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_IData_soap);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return DataClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_IData_soap);
        }
        
        public enum EndpointConfiguration
        {
            
            BasicHttpBinding_IData_soap,
        }
    }
}