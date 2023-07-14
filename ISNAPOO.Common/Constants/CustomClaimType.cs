using System;
using System.Collections.Generic;
using System.Text;

namespace ISNAPOO.Common.Constants
{
    /// <summary>
    /// Видове стойности за ауторизиране на потребител
    /// </summary>
    public static class CustomClaimType
    {
        public static class IdStampit
        {
            public static string PersonalId = "urn:stampit:pid";

            public static string Organization = "urn:stampit:organization";

            public static string PublicKey = "urn:stampit:public_key";

            public static string Certificate = "urn:stampit:certificate";

            public static string CertificateNumber = "urn:stampit:certno";
        }

        /// <summary>
        /// id на съд
        /// </summary>
        public static string Court = "urn:io:court";

        /// <summary>
        /// Имена на потребител
        /// </summary>
        public static string FullName = "urn:io:full_name";
    }
}
