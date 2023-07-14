using Data.Models.Data.Common;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.ProviderData
{


    /// <summary>
    /// Данни за Процедура за лицензиране - връзка с екпертна комисия
    /// </summary>
    [Table("Procedure_ExpertCommission")]
    [Display(Name = " Данни за Процедура за лицензиране - връзка с екпертна комисия")]
    public class ProcedureExpertCommission : IEntity
    {
        public ProcedureExpertCommission()
        {
        }

        [Key]
        public int IdProcedureExpertCommission { get; set; }
        public int IdEntity => IdProcedureExpertCommission;

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        [ForeignKey(nameof(StartedProcedure))]
        public int IdStartedProcedure { get; set; }
        public StartedProcedure StartedProcedure { get; set; }

        /// <summary>
        ///01. Науки за земята и добив и обогатяване на полезни изкопаеми
        ///02. Машиностроене, металообработване и металургия
        ///03. Електротехника и енергетика
        ///04. Информационни и комуникационни технологии, електроника и автоматизация
        ///05. Химически продукти, технологии и опазване на околната среда
        ///06. Моторни превозни средства, кораби и летателни апарати и транспортни услуги
        ///07. Архитектура и строителство
        ///08. Производство на храни и напитки
        /// </summary>
        [Display(Name = "Връзка с  номенклатура Eкспертни комисии")]       
        public int IdExpertCommission { get; set; }//[KeyTypeIntCode] = ExpertCommission




    }
}
