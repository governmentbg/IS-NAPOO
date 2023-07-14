using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts
{
    /// <summary>    
    
    /// Description: Интерфейс за достъп до полетата 
    /// с начална и крайна дата
    /// </summary>
    public interface IDatePeriod
    {
        /// <summary>
        /// Начало на периода на валидност
        /// </summary>
        DateTime DateStart { get; set; }

        /// <summary>
        /// Край на периода на валидност
        /// Ако е NULL, е валидна след начална дата
        /// </summary>
        DateTime? DateEnd { get; set; }
    }
}
