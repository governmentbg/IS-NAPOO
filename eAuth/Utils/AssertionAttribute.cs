using eAuth.Enums;
using System;
using System.Security.Cryptography.X509Certificates;

namespace eAuth.Utils
{
    public class AssertionAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ResponseAttributes
    {
        public eCertResponseStatus ResponseStatus { get; set; }

        public string ResponseStatusMessage { get; set; }

        public string EGN { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string LatinNames { get; set; }
        public DateTime NotOnOrAfter { get; set; }
        public X509Certificate2 Certificate { get; set; }
    }
}