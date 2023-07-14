namespace ISNAPOO.Core.ViewModels.Identity
{

    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordModel
    {
        [DataType(DataType.Password)]
        [DisplayName("Стара парола")]
        [Required(ErrorMessage = "Полето за \"Стара парола\" е задължително!")]
        public string OldPassword { get; set; }
        
        [DataType(DataType.Password)]
        [DisplayName("Нова парола")]
        [Required(ErrorMessage = "Полето за \"Нова парола\" е задължително!")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Потвърждение на парола")]
        [Compare("NewPassword", ErrorMessage = "Предоставените пароли не съвпадат!")]
        [Required(ErrorMessage = "Полето за \"Потвърждение на парола\" е задължително!")]
        public string ConfirmPassword { get; set; }

        public string UserName { get; set; }
    }
}
