namespace ISNAPOO.Core.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    
    public class ResetPasswordModel
    {
        [DataType(DataType.Password)]
        [DisplayName("Нова парола")]
        [Required(ErrorMessage = "Полето за \"Потвърждение на парола\" е задължително")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Потвърждение на парола")]
        [Compare("NewPassword", ErrorMessage = "Предоставените пароли не съвпадат.")]
        public string ConfirmPassword { get; set; }
    }
}
