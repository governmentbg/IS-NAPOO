using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Framework
{
    public interface IModifiable
    {

        int IdCreateUser { get; set; }
        DateTime CreationDate { get; set; }

        int IdModifyUser { get; set; }
        DateTime ModifyDate { get; set; }
    }
    public class Mod
    {
        public int IdModifyUser { get; set; }
        public DateTime ModifyDate { get; set; }

        public string State { get; set; }
    }
}
