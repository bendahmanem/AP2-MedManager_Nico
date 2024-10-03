using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel
{
    public class LoginViewModel
    {
        
        [Required(ErrorMessage = "The Username field is required.")]
        public required string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
