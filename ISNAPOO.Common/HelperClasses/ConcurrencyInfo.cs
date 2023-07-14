using System;

namespace ISNAPOO.Common.HelperClasses
{
    public class ConcurrencyInfo 
    {
        public int IdPerson { get; set; }

        public int IdEntity { get; set; }

        public string PersonFirstName { get; set; }

        public string PersonFamilyName { get; set; }

        public string EntityType { get; set; }

        public DateTime? TimeOpened { get; set; }

        public string FullName => $"{this.PersonFirstName} {this.PersonFamilyName}";
    }
}
