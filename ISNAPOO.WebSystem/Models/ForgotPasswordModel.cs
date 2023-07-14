namespace ISNAPOO.Core.ViewModels
{
    using ISNAPOO.Common.Constants;
    using System.ComponentModel.DataAnnotations;
    
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Полето за \"Потребителско име\" е задължително!")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Твърде дълго потребителско име!")]
        [Display(Name = "Потребителско име")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Полето за \"E-mail\" е задължително!")]
        [Display(Name = "Е-поща")]
        public string Email { get; set; }

        public bool EmailSent { get; set; }
    }
}
