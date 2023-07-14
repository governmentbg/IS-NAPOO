using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Identity
{
	public class ChangeInformationModel
	{
        [DisplayName("Email")]
        [Required(ErrorMessage = "Полето за \"Email\" е задължително!")]
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        public string Email { get; set; }

        [DisplayName("Длъжност")]
        [MaxLength(20, ErrorMessage = "Полето \"Длъжност\" не може да съдържа повече от 20 символа!")]
        //[Required(ErrorMessage = "Полето за \"Длъжност\" е задължително!")]
        public string Title { get; set; }
    }
}

