using System.Collections.Generic;

namespace eAuth.Utils
{
    public class Saml2Serialize
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Certificate
        {
            public string FileName { get; set; }
            public string Password { get; set; }
        }

        public class IdentityProviderConfiguration
        {
            public string EntityId { get; set; }
            public string Name { get; set; }
            public string ForceAuth { get; set; }
            public string IsPassive { get; set; }
            public string SingleSignOnService { get; set; }
            public string SingleSignOutService { get; set; }
            public string ArtifactResolveService { get; set; }
            public Certificate Certificate { get; set; }
        }

        public class Root
        {
            public Saml2 Saml2 { get; set; }
        }

        public class Saml2
        {
            public ServiceProviderConfiguration ServiceProviderConfiguration { get; set; }
            public List<IdentityProviderConfiguration> IdentityProviderConfiguration { get; set; }
        }

        public class ServiceProviderConfiguration
        {
            public string EntityId { get; set; }
            public string Name { get; set; }
            public string AssertionConsumerServiceUrl { get; set; }
            public string SingleLogoutResponseServiceUrl { get; set; }
            public bool OmitAssertionSignatureCheck { get; set; }
            public Certificate Certificate { get; set; }
        }


    }
}
