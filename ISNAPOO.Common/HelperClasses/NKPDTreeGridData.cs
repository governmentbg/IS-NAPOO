using static ISNAPOO.Common.Enums.EMIEnums;

namespace ISNAPOO.Common.HelperClasses
{
    public class NKPDTreeGridData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int? ParentId { get; set; }

        public int EntityId { get; set; }

        public string EntityType { get; set; }

        public NKPDLevel NKPDLevel { get; set; }

        public bool? IsParent { get; set; }
    }
}
