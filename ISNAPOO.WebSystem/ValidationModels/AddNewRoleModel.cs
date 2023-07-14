using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.WebSystem.ValidationModels
{
    public class AddNewRoleModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Role name must be between 3 and 20 characters long!")]
        public string RoleName { get; set; }
    }
}
