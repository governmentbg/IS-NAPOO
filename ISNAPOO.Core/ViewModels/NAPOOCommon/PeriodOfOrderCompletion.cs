using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.NAPOOCommon
{
    public class PeriodOfOrderCompletion
    {
        public DateTime FromDate { get; set; }
        public string FromDateFormatted { get { return FromDate.ToString(GlobalConstants.DATE_FORMAT); } }

        public DateTime ToDate { get; set; }
        public string ToDateFormatted { get { return ToDate.ToString(GlobalConstants.DATE_FORMAT); } }

    }
}
