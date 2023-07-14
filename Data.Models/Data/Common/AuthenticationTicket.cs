using Data.Models.Data.ProviderData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Common
{
    public class AuthenticationTicket
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public byte[] Value { get; set; }

        public DateTimeOffset? LastActivity { get; set; }

        public DateTimeOffset? Expires { get; set; }
    }
}
