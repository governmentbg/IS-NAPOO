using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.NAPOOCommon
{
    public class ContactPersonData
    {
        public string FullName { get; set; }

        public string Sirname { get; set; }

        public string StreetName { get; set; }

        public string PostCode { get; set; }

        public string CityName { get; set; }

        public string District { get; set; }

        public string Municipality { get; set; }

        public string Email { get; set; }

        public List<string> TelephoneNumbers { get; set; }
    }
}
