using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Framework
{
    public class UserProps
    {
        public UserProps()
        {
            UserId = 0;
            IPAddress = "0.0.0.0";

		}
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int IdCandidateProvider { get; set; }
        public int UserId { get; set; }
        public int IdPerson { get; set; }
        public string PersonName { get; set; }
        public string IPAddress { get; set; }
        public string BrowserInformation { get; set; }
        public string CurrentAction { get; set; }
        public string CurrentActionDescription { get; set; }
        public string CurrentUrl { get; set; }
        public string CurrentMenu { get; set; }

    }
}
