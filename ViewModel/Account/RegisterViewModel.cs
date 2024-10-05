using System.ComponentModel.DataAnnotations;

namespace MedManager.ViewModel.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public required string Nom { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public required string Prenom { get; set; }

        [Required]
        [Display(Name = "Ville")]
        public required string Ville { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public required string Password { get; set; }
    }

}
