using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.ProviderData
{ 


    /// <summary>
    /// Лице връзка с CPO,CIPO - Обучаваща институция"
    /// </summary>
    [Table("ProviderPerson")]
    [Display(Name = "Лице връзка с CPO,CIPO - Обучаваща институция")]
    public class ProviderPerson : IEntity
    {
        public ProviderPerson()
        {

        }

        [Key]
        public int IdProviderPerson { get; set; }
        public int IdEntity => IdProviderPerson;

        [Display(Name = "Връзка с лице")]
        [ForeignKey(nameof(Person))]
        public int IdPerson { get; set; }
        public Person Person { get; set; }

        [Display(Name = "Връзка с  CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(Provider))]
        public int IdProvider { get; set; }
        public Provider Provider { get; set; }


    }
}
