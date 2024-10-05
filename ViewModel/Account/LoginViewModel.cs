using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.Account
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        public required string UserName { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
