namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;
    using System.Collections.Generic;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;
    
    public class CPOLicenseChangeApplication11
    {
        public CPOLicenseChangeApplication11()
        {
            this.ScoreTable = new Dictionary<int, List<ScoreTableData>>();
        }

        //<summary>
        //  Данни за оценките на ВЕ
        //</summary>
        public Dictionary<int, List<ScoreTableData>> ScoreTable { get; set; }

        //<summary>
        //  Данни за ВЕ
        //</summary>
        public Expert ExternalExpertData { get; set; }

        public ExpertVM ExternalExpertDataVM { get; set; }

        //<summary>
        //  Номер на лицензия
        //</summary>
        public string LicenseNumber { get; set; }

        //<summary>
        //  Основни данни на ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Реф. номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        //<summary>
        //  Дата на входиране на заповед
        //</summary>
        public DateTime OrderInputDate { get; set; }
        public string OrderInputDateFormatted { get { return OrderInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Период за изпълнение на заповед
        //</summary>
        public PeriodOfOrderCompletion PeriodOfOrderCompletion { get; set; }

        //<summary>
        //  Списък с професионални направления
        //</summary>
        public List<ProfessionalDirectionVM> ProfessionalDirections { get; set; }

        //<summary>
        //  Мотиви за оценка "0"
        //</summary>
        public string MotivesForZeroScore { get; set; }

        //<summary>
        //  Общ резултат от оценяване
        //</summary>
        public string TotalScore { get; set; }

    }

    public class ScoreTableData
    {
        //<summary>
        //  Реф. номер на поле в таблицата с оценки
        //</summary>
        public string Id { get; set; }

        //<summary>
        //  Максимална стойност на оценките
        //</summary>
        public double MaxValue { get; set; }

        //<summary>
        //  Стойност на оценката
        //</summary>
        public double Value { get; set; }
    }
}
