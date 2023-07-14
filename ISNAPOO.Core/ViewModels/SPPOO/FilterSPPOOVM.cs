namespace ISNAPOO.Core.ViewModels.CPO
{
    public class FilterSPPOOVM
    {
        public FilterSPPOOVM()
        {
            this.Description = string.Empty;
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string NkpdCode { get; set; }

        public int IdStatus { get; set; }

        public string OrderNumber { get; set; }

        public bool IsPresupposeLegalCapacity { get; set; }

        public int IdVQS { get; set; }

        public int IdNKRLevel { get; set; }//1,2,3,4,5

        public int IdEKRLevel { get; set; }//1,2,3,4,5

        public bool IsStateProtectedSpecialties { get; set; }//(Да/Не)

        public bool IsShortageSpecialistsLaborMarket { get; set; }//(Да/Не)

        public int IdLegalCapacityOrdinanceType { get; set; }
    }
}
