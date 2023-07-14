using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Rating
{
    public class CandidateProviderIndicatorExcelExportVM
    {
        public CandidateProviderIndicatorExcelExportVM()
        {
            this.IndicatorsRangePairs = new Dictionary<KeyValueVM, int>();
            this.CandidateProviderIndicators = new List<CandidateProviderIndicatorVM>();
            this.candidateProviderVM = new CandidateProviderVM();
            this.CandidateProviderIndicatorsWeightPair = new Dictionary<CandidateProviderIndicatorVM, decimal>();
        }
        public CandidateProviderVM candidateProviderVM { get; set; }
        public List<CandidateProviderIndicatorVM> CandidateProviderIndicators { get; set; }
        public Dictionary<CandidateProviderIndicatorVM, decimal> CandidateProviderIndicatorsWeightPair { get; set; }
        public Dictionary<KeyValueVM, int> IndicatorsRangePairs { get; set; }
        public decimal TotalRating { get; set; } = 0;
        public int rowNumber { get; set; } = 0;
        public string sumCols { get; set; } = string.Empty;

        public void CalculateRating()
        {
            foreach (var result in this.CandidateProviderIndicatorsWeightPair)
            {
                this.TotalRating += result.Key.Points * result.Value;
            }
        }
    }
}
