using System;
using System.Collections.Generic;
using System.Text;

namespace ISNAPOO.Common.HelperClasses
{
    public class ApplicationSetting
    {
        public string ResourcesFolderName { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string HttpScheme { get; set; }
        public string InternalEAuthURL { get; set; }
        public string DocuServiceURL { get; set; }
        public string EndpointConfigurationDocuService { get; set; }
        public string RegixURL { get; set; }



        public void ToCopy(ApplicationSetting applicationSetting)
        {
            this.Host = applicationSetting.Host;
            this.Port = applicationSetting.Port;
            this.HttpScheme = applicationSetting.HttpScheme;
            this.InternalEAuthURL = applicationSetting.InternalEAuthURL;
            this.ResourcesFolderName = applicationSetting.ResourcesFolderName;
            this.DocuServiceURL = applicationSetting.DocuServiceURL;
            this.RegixURL = applicationSetting.RegixURL;
            this.EndpointConfigurationDocuService = applicationSetting.EndpointConfigurationDocuService;
        }
    }
}
