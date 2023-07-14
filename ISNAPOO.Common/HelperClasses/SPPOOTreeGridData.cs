using System.Collections.Generic;

namespace ISNAPOO.Common.HelperClasses
{
    public class SPPOOTreeGridData
    {
        public SPPOOTreeGridData()
        {
            this.OrderNumbers = new List<string>();
            this.Description = string.Empty;
            this.NKPDCodes = new List<string>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string AddOrder { get; set; }

        public string RemoveOrder { get; set; }

        public string ChangeOrder { get; set; }

        public string AddOrderName { get; set; }

        public string RemoveOrderName { get; set; }

        public string ChangeOrderName { get; set; }

        public int? ParentId { get; set; }

        public int EntityId { get; set; }

        public int? EntityParentId { get; set; }

        public string EntityType { get; set; }

        public int IdStatus { get; set; }

        public string Description { get; set; }

        public int IdVQS { get; set; }

        public int IdNKRLevel { get; set; }

        public int IdEKRLevel { get; set; }

        public List<string> OrderNumbers { get; set; }

        public List<string> NKPDCodes { get; set; }

        public bool IsStateProtectedSpecialties { get; set; }

        public bool IsShortageSpecialistsLaborMarket { get; set; }

        public bool IsPresupposeLegalCapacity { get; set; }

        public int IdLegalCapacityOrdinanceType { get; set; }

        public bool IsFilterResult { get; set; }

        public bool HasActiveChildren { get; set; }

        public bool HasChildren { get; set; }

        public bool IsFilteredResult { get; set; }
    }
}
