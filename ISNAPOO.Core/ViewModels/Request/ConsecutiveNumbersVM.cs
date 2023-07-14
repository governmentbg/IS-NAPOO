using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class ConsecutiveNumbersVM
    {
        [Required(ErrorMessage = "Полето 'Начален номер' е задължително!")]
        [StringLength(10, ErrorMessage = "Полето 'Начален номер' може да съдържа до 10 числа!")]
        public string StartNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Краен номер' е задължително!")]
        [StringLength(10, ErrorMessage = "Полето 'Краен номер' може да съдържа до 10 числа!")]
        public string EndNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Брой поредни' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Брой поредни' може да има стойност от 1!")]
        public int CountConsecutiveNumbers { get; set; }
    }
}
