using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.WebSystem.ValidationModels
{
    public class FormValidationModel
    {
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 30 characters long!")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 30 characters long!")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Country { get; set; }
    }
}
