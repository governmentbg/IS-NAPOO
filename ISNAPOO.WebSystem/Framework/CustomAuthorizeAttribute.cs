
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISNAPOO.WebSystem.Framework
{
        /// <summary>
    /// Specifies that the class or method that this attribute is applied to requires the specified authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizeData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAuthorizeAttribute"/> class.
        /// </summary>
        public CustomAuthorizeAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAuthorizeAttribute"/> class with the specified policy.
        /// </summary>
        /// <param name="policy">The name of the policy to require for authorization.</param>
        public CustomAuthorizeAttribute(string policy)
        {
            Policy = policy;
        }

        /// <summary>
        /// Gets or sets the policy name that determines access to the resource.
        /// </summary>
        public string? Policy { get; set; }

        /// <summary>
        /// Gets or sets a comma delimited list of roles that are allowed to access the resource.
        /// </summary>
        public string? Roles { get; set; }

        /// <summary>
        /// Gets or sets a comma delimited list of schemes from which user information is constructed.
        /// </summary>
        public string? AuthenticationSchemes { get; set; }
    }
}
