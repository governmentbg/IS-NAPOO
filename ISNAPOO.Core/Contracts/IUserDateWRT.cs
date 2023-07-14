using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts
{
    public interface IUserDateWRT
    {
        string UserId { get; set; }
        DateTime DateWrt { get; set; }
    }
}
