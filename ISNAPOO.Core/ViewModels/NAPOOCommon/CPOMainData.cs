using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.NAPOOCommon
{
    public class CPOMainData
    {
        /// <summary>
        /// Име на ценър за професионално обучение (ЦПО)
        /// </summary>
        public string CPOName { get; set; }


        /// <summary>
        /// Юридическо лице на ЦПО
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// ЕИК/БУЛСТАТ
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// Град
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Представител на компания
        /// </summary>
        public string CompanyRepresentative { get; set; }
    }
}
