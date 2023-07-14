using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class EventLogListFilterVM
    {      
        public DateTime? EventLogsFrom { get; set; }    
        public DateTime? EventLogsTo { get; set;}
        public string IP { get; set;}
        public string PersonName { get; set;}
    }
}
